using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005AF RID: 1455
	public class SpaceCombatCameraBlend : MonoBehaviour
	{
		// Token: 0x0600277A RID: 10106 RVA: 0x000D8208 File Offset: 0x000D6408
		private void OnPreRender()
		{
			if (this._renderTexture == null && !this.isDestroyed)
			{
				this._renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 1, RenderTextureFormat.ARGB32);
				if (this._additiveCamera != null)
				{
					this._additiveCamera.targetTexture = this._renderTexture;
				}
			}
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x000D8261 File Offset: 0x000D6461
		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (this._material != null)
			{
				this._material.SetTexture("_SecondTex", this._renderTexture);
				Graphics.Blit(src, dest, this._material);
			}
		}

		// Token: 0x0600277C RID: 10108 RVA: 0x000D8294 File Offset: 0x000D6494
		public void SetUp(Material mat, Camera cam)
		{
			this._material = mat;
			this._additiveCamera = cam;
			this.isDestroyed = false;
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x000D82AB File Offset: 0x000D64AB
		public void CleanUp()
		{
			this.isDestroyed = true;
			if (this._additiveCamera != null)
			{
				this._additiveCamera.targetTexture = null;
			}
			RenderTexture.ReleaseTemporary(this._renderTexture);
		}

		// Token: 0x0600277E RID: 10110 RVA: 0x000D82D9 File Offset: 0x000D64D9
		private void OnDestroy()
		{
			if (!this.isDestroyed)
			{
				this.CleanUp();
				Log.Warn("SpaceCombatCameraBlend: Camera Blend effect was not cleaned up properly. Temporary memory has been released.", Array.Empty<object>());
			}
		}

		// Token: 0x04001D61 RID: 7521
		public Material _material;

		// Token: 0x04001D62 RID: 7522
		public Camera _additiveCamera;

		// Token: 0x04001D63 RID: 7523
		private bool isDestroyed;

		// Token: 0x04001D64 RID: 7524
		private RenderTexture _renderTexture;
	}
}
