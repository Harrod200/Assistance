using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000020 RID: 32
public class LargeExplosionEffectController : AbstractEffectController
{
	// Token: 0x060000D6 RID: 214 RVA: 0x00007684 File Offset: 0x00005884
	private void Awake()
	{
		if (this.postProcessing != null)
		{
			GameObject gameObject = new GameObject("_ExplosionPostProcessing");
			gameObject.layer |= LayerMask.NameToLayer("PostProcess");
			gameObject.transform.SetParent(base.transform);
			this.postProcessingVolume = gameObject.AddComponent<PostProcessVolume>();
			this.postProcessingVolume.profile = this.postProcessing;
			this.postProcessingVolume.isGlobal = true;
			this.postProcessingVolume.priority = this.postProcessPriority;
			this.postProcessingTimeScale = this.postProcessCurve.keys[this.postProcessCurve.length - 1].time / this.postProcessDuration;
			this.postProcessingVolume.enabled = false;
			this.postProcessingVolume.gameObject.SetActive(false);
		}
		this.CleanUp();
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00007761 File Offset: 0x00005961
	private void OnDestroy()
	{
		if (this.postProcessingVolume != null)
		{
			global::UnityEngine.Object.Destroy(this.postProcessingVolume.gameObject);
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00007781 File Offset: 0x00005981
	public override void CleanUp()
	{
		if (this.postProcessingVolume != null)
		{
			this.postProcessingVolume.weight = this.postProcessCurve.Evaluate(0f);
		}
		this.progress = 0f;
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000077B7 File Offset: 0x000059B7
	protected override void OnPlay()
	{
		this.explosionParticleSystem.Play();
		if (this.postProcessingVolume)
		{
			this.postProcessingVolume.enabled = true;
			this.postProcessingVolume.gameObject.SetActive(true);
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x000077EE File Offset: 0x000059EE
	protected override void OnStop()
	{
		this.explosionParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		if (this.postProcessingVolume)
		{
			this.postProcessingVolume.enabled = false;
			this.postProcessingVolume.gameObject.SetActive(false);
		}
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00007828 File Offset: 0x00005A28
	protected override void OnUpdate(float deltaTime)
	{
		if (this.postProcessingVolume)
		{
			this.progress += deltaTime * this.postProcessingTimeScale;
			float num = this.postProcessCurve.Evaluate(this.progress);
			Camera main = Camera.main;
			Vector3 vector = main.transform.position - base.transform.position;
			float num2 = this.postProcessMaxDistance * Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			float num3 = 1f - Mathf.Clamp01(vector.magnitude / num2);
			num3 = Mathf.Clamp01(Mathf.Pow(num3, 0.25f));
			float num4 = Vector3.Angle(main.transform.forward, (-vector).normalized);
			float num5 = 1f - Mathf.Clamp01(num4 / this.postProcessMaxAngle - 0.3f);
			num5 = Mathf.Clamp01(Mathf.Pow(num5, 2f));
			this.postProcessingVolume.weight = num5 * num3 * num;
		}
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00007965 File Offset: 0x00005B65
	protected override void OnPause()
	{
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00007967 File Offset: 0x00005B67
	protected override void OnUnPause()
	{
	}

	// Token: 0x040000C1 RID: 193
	[SerializeField]
	[Header("Explosion")]
	private ParticleSystem explosionParticleSystem;

	// Token: 0x040000C2 RID: 194
	[SerializeField]
	private float durationVariance = 0.2f;

	// Token: 0x040000C3 RID: 195
	[SerializeField]
	[Header("Post Processing")]
	private PostProcessProfile postProcessing;

	// Token: 0x040000C4 RID: 196
	[SerializeField]
	[DisplayName("Priority")]
	private float postProcessPriority = 10f;

	// Token: 0x040000C5 RID: 197
	[SerializeField]
	[DisplayName("Duration")]
	private float postProcessDuration = 1f;

	// Token: 0x040000C6 RID: 198
	[SerializeField]
	[DisplayName("Max Distance")]
	private float postProcessMaxDistance = 1000f;

	// Token: 0x040000C7 RID: 199
	[SerializeField]
	[DisplayName("Max Angle")]
	private float postProcessMaxAngle = 100f;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	[DisplayName("Strength Curve")]
	private AnimationCurve postProcessCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x040000C9 RID: 201
	private PostProcessVolume postProcessingVolume;

	// Token: 0x040000CA RID: 202
	private float postProcessingTimeScale = 1f;

	// Token: 0x040000CB RID: 203
	private float progress;

	// Token: 0x040000CC RID: 204
	private HashSet<Camera> affectedCameras = new HashSet<Camera>();
}
