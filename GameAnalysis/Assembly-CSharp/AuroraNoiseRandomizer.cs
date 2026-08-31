using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class AuroraNoiseRandomizer : MonoBehaviour
{
	// Token: 0x060000DF RID: 223 RVA: 0x00007A1F File Offset: 0x00005C1F
	private void Awake()
	{
		this._renderer = base.GetComponent<Renderer>();
		this._propID = Shader.PropertyToID(this.targetProperty);
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00007A40 File Offset: 0x00005C40
	private void Start()
	{
		if (this.randomize = true)
		{
			this.AssignRandomNoise();
		}
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00007A60 File Offset: 0x00005C60
	public void AssignRandomNoise()
	{
		if (this._renderer == null)
		{
			this._renderer = base.GetComponent<Renderer>();
		}
		if (this.noiseTextures == null || this.noiseTextures.Length == 0)
		{
			Debug.LogWarning(base.name + ": No noise textures assigned.", this);
			return;
		}
		int num = global::UnityEngine.Random.Range(0, this.noiseTextures.Length);
		Texture2D texture2D = this.noiseTextures[num];
		this._renderer.materials[0].SetTexture(this._propID, texture2D);
	}

	// Token: 0x040000CD RID: 205
	[Header("Noise Array")]
	public Texture2D[] noiseTextures;

	// Token: 0x040000CE RID: 206
	[Header("Target")]
	public string targetProperty = "_NoiseA";

	// Token: 0x040000CF RID: 207
	public bool randomize = true;

	// Token: 0x040000D0 RID: 208
	private Renderer _renderer;

	// Token: 0x040000D1 RID: 209
	private int _propID;
}
