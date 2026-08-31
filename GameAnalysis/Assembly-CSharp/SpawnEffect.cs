using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class SpawnEffect : MonoBehaviour
{
	// Token: 0x06000061 RID: 97 RVA: 0x00005110 File Offset: 0x00003310
	private void Start()
	{
		this.shaderProperty = Shader.PropertyToID("_cutoff");
		this._renderer = base.GetComponent<Renderer>();
		this.ps = base.GetComponentInChildren<ParticleSystem>();
		this.ps.main.duration = this.spawnEffectTime;
		this.ps.Play();
	}

	// Token: 0x06000062 RID: 98 RVA: 0x0000516C File Offset: 0x0000336C
	private void Update()
	{
		if (this.timer < this.spawnEffectTime + this.pause)
		{
			this.timer += Time.deltaTime;
		}
		else
		{
			this.ps.Play();
			this.timer = 0f;
		}
		this._renderer.material.SetFloat(this.shaderProperty, this.fadeIn.Evaluate(Mathf.InverseLerp(0f, this.spawnEffectTime, this.timer)));
	}

	// Token: 0x04000055 RID: 85
	public float spawnEffectTime = 2f;

	// Token: 0x04000056 RID: 86
	public float pause = 1f;

	// Token: 0x04000057 RID: 87
	public AnimationCurve fadeIn;

	// Token: 0x04000058 RID: 88
	private ParticleSystem ps;

	// Token: 0x04000059 RID: 89
	private float timer;

	// Token: 0x0400005A RID: 90
	private Renderer _renderer;

	// Token: 0x0400005B RID: 91
	private int shaderProperty;
}
