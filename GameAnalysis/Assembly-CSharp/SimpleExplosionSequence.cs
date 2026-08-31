using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class SimpleExplosionSequence : AbstractEffectController
{
	// Token: 0x060000BB RID: 187 RVA: 0x000067FC File Offset: 0x000049FC
	private void Awake()
	{
		for (int i = 0; i < this.m_explosionTargets.Count; i++)
		{
			int num = global::UnityEngine.Random.Range(i, this.m_explosionTargets.Count);
			GameObject gameObject = this.m_explosionTargets[i];
			this.m_explosionTargets[i] = this.m_explosionTargets[num];
			this.m_explosionTargets[num] = gameObject;
		}
	}

	// Token: 0x060000BC RID: 188 RVA: 0x00006863 File Offset: 0x00004A63
	public override void CleanUp()
	{
		this.m_playingEffects.Clear();
		this.m_targetIndex = 0;
		this.m_elapsed = 0f;
	}

	// Token: 0x060000BD RID: 189 RVA: 0x00006882 File Offset: 0x00004A82
	protected override void OnPlay()
	{
		this.m_targetIndex = 0;
		this.m_elapsed = 0f;
		this.CreateExplosion();
	}

	// Token: 0x060000BE RID: 190 RVA: 0x0000689C File Offset: 0x00004A9C
	protected override void OnStop()
	{
		for (int i = this.m_playingEffects.Count - 1; i >= 0; i--)
		{
			this.m_playingEffects[i].Stop();
		}
	}

	// Token: 0x060000BF RID: 191 RVA: 0x000068D4 File Offset: 0x00004AD4
	protected override void OnUpdate(float deltaTime)
	{
		if (this.m_targetIndex >= this.m_explosionTargets.Count)
		{
			if (this.m_playingEffects.Count == 0)
			{
				base.EffectCompleted();
				return;
			}
		}
		else
		{
			this.m_elapsed += deltaTime;
			if (this.m_elapsed >= this.m_delay)
			{
				this.CreateExplosion();
				this.m_elapsed = 0f;
			}
		}
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00006935 File Offset: 0x00004B35
	protected override void OnPause()
	{
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x00006937 File Offset: 0x00004B37
	protected override void OnUnPause()
	{
	}

	// Token: 0x060000C2 RID: 194 RVA: 0x0000693C File Offset: 0x00004B3C
	private void CreateExplosion()
	{
		if (this.m_targetIndex >= this.m_explosionTargets.Count)
		{
			return;
		}
		this.m_delay = global::UnityEngine.Random.Range(this.m_minDelay, this.m_maxDelay);
		List<GameObject> explosionTargets = this.m_explosionTargets;
		int targetIndex = this.m_targetIndex;
		this.m_targetIndex = targetIndex + 1;
		GameObject gameObject = explosionTargets[targetIndex];
		if (!gameObject.activeInHierarchy)
		{
			return;
		}
		GameObject gameObject2 = new GameObject("Explosion (Generated)");
		gameObject2.transform.localScale = base.transform.lossyScale;
		gameObject2.transform.SetParent(gameObject.transform, true);
		gameObject2.transform.localPosition = Vector3.zero;
		BurnUpEffect burnUp = gameObject2.AddComponent<BurnUpEffect>();
		burnUp.m_burnUpMaterial = this.m_burnUpEffect.m_burnUpMaterial;
		burnUp.m_duration = this.m_burnUpEffect.m_duration;
		burnUp.m_destroyTargetsOnComplete = this.m_burnUpEffect.m_destroyTargetsOnComplete;
		burnUp.SetTargetObjects(new GameObject[] { gameObject }, true);
		burnUp.OnCompleted += delegate
		{
			this.m_playingEffects.Remove(burnUp);
		};
		this.m_playingEffects.Add(burnUp);
		burnUp.Play();
		ParticleGroupEffect explosion = gameObject2.AddComponent<ParticleGroupEffect>();
		explosion.m_particleGroup = this.m_explosionEffect;
		explosion.m_spawnAnchor = gameObject2.transform;
		explosion.OnCompleted += delegate
		{
			this.m_playingEffects.Remove(explosion);
		};
		this.m_playingEffects.Add(explosion);
		explosion.Play();
	}

	// Token: 0x040000A0 RID: 160
	[SerializeField]
	private ParticleGroup m_explosionEffect;

	// Token: 0x040000A1 RID: 161
	[SerializeField]
	private BurnUpEffect m_burnUpEffect;

	// Token: 0x040000A2 RID: 162
	[SerializeField]
	private List<GameObject> m_explosionTargets;

	// Token: 0x040000A3 RID: 163
	[SerializeField]
	private float m_minDelay = 0.05f;

	// Token: 0x040000A4 RID: 164
	[SerializeField]
	private float m_maxDelay = 0.15f;

	// Token: 0x040000A5 RID: 165
	private float m_delay;

	// Token: 0x040000A6 RID: 166
	private float m_elapsed;

	// Token: 0x040000A7 RID: 167
	private int m_targetIndex;

	// Token: 0x040000A8 RID: 168
	private List<AbstractEffectController> m_playingEffects = new List<AbstractEffectController>();
}
