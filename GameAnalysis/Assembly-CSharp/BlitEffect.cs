using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

// Token: 0x02000022 RID: 34
[ExecuteInEditMode]
public class BlitEffect : MonoBehaviour
{
	// Token: 0x060000E3 RID: 227 RVA: 0x00007AFA File Offset: 0x00005CFA
	private void Start()
	{
		this._camera.depthTextureMode = DepthTextureMode.DepthNormals;
		this._camera.forceIntoRenderTexture = false;
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00007B14 File Offset: 0x00005D14
	private void OnPreRender()
	{
		this._tempRenderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 32, GraphicsFormat.R32G32B32A32_SFloat);
		this._camera.targetTexture = this._tempRenderTexture;
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x00007B40 File Offset: 0x00005D40
	private void OnPostRender()
	{
		this._camera.targetTexture = null;
		this._postProcessingMat.SetTexture("_MainTex", this._tempRenderTexture);
		this._postProcessingMat.SetVector("_Point", this._target.position);
		this._postProcessingMat.SetVector("_Direction", this._target.forward);
		Vector3 vector = this._camera.WorldToScreenPoint(this._target.position);
		vector.y *= -1f;
		vector.y += (float)Screen.height;
		this._postProcessingMat.SetVector("_ScreenPoint", vector);
		Graphics.Blit(this._tempRenderTexture, null, this._postProcessingMat);
		RenderTexture.ReleaseTemporary(this._tempRenderTexture);
	}

	// Token: 0x040000D2 RID: 210
	public Transform _target;

	// Token: 0x040000D3 RID: 211
	public Camera _camera;

	// Token: 0x040000D4 RID: 212
	public Material _postProcessingMat;

	// Token: 0x040000D5 RID: 213
	private RenderTexture _tempRenderTexture;
}
