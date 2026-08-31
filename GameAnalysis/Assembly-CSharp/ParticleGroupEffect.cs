using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class ParticleGroupEffect : AbstractEffectController
{
	// Token: 0x060000AB RID: 171 RVA: 0x000061A8 File Offset: 0x000043A8
	private void Awake()
	{
		if (this.m_spawnAnchor != null && this.m_spawnAnchor != base.transform)
		{
			this.spawnPosition = this.m_spawnAnchor.InverseTransformPoint(base.transform.position);
			this.spawnRotation = Quaternion.Inverse(this.m_spawnAnchor.rotation) * base.transform.rotation;
			this.spawnScale = Vector3.Scale(base.transform.localScale, new Vector3(1f / this.m_spawnAnchor.localScale.x, 1f / this.m_spawnAnchor.localScale.y, 1f / this.m_spawnAnchor.localScale.z));
		}
	}

	// Token: 0x060000AC RID: 172 RVA: 0x0000627C File Offset: 0x0000447C
	protected override void OnPlay()
	{
		this.CleanUp();
		if (this.m_particleGroup == null)
		{
			Debug.LogWarning("m_particleGroup " + base.name + " is null");
			return;
		}
		ParticleSystem[] particleSystems = this.m_particleGroup.particleSystems;
		for (int i = 0; i < particleSystems.Length; i++)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(particleSystems[i].gameObject, this.m_spawnAnchor);
			gameObject.transform.localPosition = this.spawnPosition;
			gameObject.transform.rotation = this.spawnRotation;
			gameObject.transform.localScale = this.spawnScale;
			if (!gameObject)
			{
				Debug.LogError("Error initializing particle system");
			}
			else
			{
				this.m_particleInstances.Add(gameObject);
				foreach (ParticleSystem particleSystem in gameObject.GetComponentsInChildren<ParticleSystem>())
				{
					if (particleSystem.main.loop)
					{
						this.m_loopingEffects.Add(particleSystem);
					}
					particleSystem.Play();
					this.m_playingEffects.Add(particleSystem);
				}
			}
		}
		if (this.m_loopDurationOverride > 0f)
		{
			base.StartCoroutine(this.StopLoopedEffects(this.m_loopDurationOverride));
		}
	}

	// Token: 0x060000AD RID: 173 RVA: 0x000063B4 File Offset: 0x000045B4
	public override void CleanUp()
	{
		this.m_loopingEffects.Clear();
		this.m_playingEffects.Clear();
		foreach (GameObject gameObject in this.m_particleInstances)
		{
			global::UnityEngine.Object.Destroy(gameObject);
		}
		this.m_particleInstances.Clear();
	}

	// Token: 0x060000AE RID: 174 RVA: 0x00006428 File Offset: 0x00004628
	protected override void OnUpdate(float deltaTime)
	{
		for (int i = this.m_playingEffects.Count - 1; i >= 0; i--)
		{
			ParticleSystem particleSystem = this.m_playingEffects[i];
			if (!particleSystem.isPlaying || (particleSystem.time > particleSystem.main.duration && particleSystem.particleCount == 0))
			{
				particleSystem.Stop();
				this.m_playingEffects.RemoveAt(i);
			}
		}
		if (this.m_playingEffects.Count == 0)
		{
			base.EffectCompleted();
		}
	}

	// Token: 0x060000AF RID: 175 RVA: 0x000064A8 File Offset: 0x000046A8
	protected override void OnStop()
	{
		foreach (ParticleSystem particleSystem in this.m_loopingEffects)
		{
			particleSystem.main.loop = false;
		}
	}

	// Token: 0x060000B0 RID: 176 RVA: 0x00006504 File Offset: 0x00004704
	protected override void OnPause()
	{
		for (int i = this.m_playingEffects.Count - 1; i >= 0; i--)
		{
			this.m_playingEffects[i].Pause();
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x0000653C File Offset: 0x0000473C
	protected override void OnUnPause()
	{
		for (int i = this.m_playingEffects.Count - 1; i >= 0; i--)
		{
			this.m_playingEffects[i].Play();
		}
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00006572 File Offset: 0x00004772
	private IEnumerator StopLoopedEffects(float duration)
	{
		yield return new WaitForSeconds(duration);
		using (List<ParticleSystem>.Enumerator enumerator = this.m_loopingEffects.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ParticleSystem particleSystem = enumerator.Current;
				particleSystem.main.loop = false;
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x04000091 RID: 145
	public ParticleGroup m_particleGroup;

	// Token: 0x04000092 RID: 146
	public Transform m_spawnAnchor;

	// Token: 0x04000093 RID: 147
	[Tooltip("If set greater than 0, will stop all looping particle systems after the given duration")]
	public float m_loopDurationOverride;

	// Token: 0x04000094 RID: 148
	private List<GameObject> m_particleInstances = new List<GameObject>();

	// Token: 0x04000095 RID: 149
	private List<ParticleSystem> m_loopingEffects = new List<ParticleSystem>();

	// Token: 0x04000096 RID: 150
	private List<ParticleSystem> m_playingEffects = new List<ParticleSystem>();

	// Token: 0x04000097 RID: 151
	private Vector3 spawnPosition = Vector3.zero;

	// Token: 0x04000098 RID: 152
	private Quaternion spawnRotation = Quaternion.identity;

	// Token: 0x04000099 RID: 153
	private Vector3 spawnScale = Vector3.one;
}
