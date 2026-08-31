using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A3 RID: 2211
	public class DynamicRenderTexture : MonoBehaviour
	{
		// Token: 0x0600537B RID: 21371 RVA: 0x002562F8 File Offset: 0x002544F8
		private void Start()
		{
			if (this.thisCamera == null)
			{
				this.thisCamera = base.GetComponent<Camera>();
			}
			if (this.thisCamera.targetTexture != null)
			{
				this.thisCamera.targetTexture.Release();
			}
			int num = this.originalSizeX;
			int num2 = this.originalSizeY;
			if (Screen.currentResolution.height * Screen.currentResolution.width < 3700000)
			{
				num = this.originalSizeX * 2 / 3;
				num2 = this.originalSizeY * 2 / 3;
			}
			if (Screen.currentResolution.height * Screen.currentResolution.width < 2100000)
			{
				num = this.originalSizeX / 2;
				num2 = this.originalSizeY / 2;
			}
			this.thisCamera.targetTexture = new RenderTexture(num, num2, 24);
		}

		// Token: 0x0600537C RID: 21372 RVA: 0x002563D0 File Offset: 0x002545D0
		private void OnDisable()
		{
		}

		// Token: 0x04003946 RID: 14662
		public Camera thisCamera;

		// Token: 0x04003947 RID: 14663
		public int originalSizeX = 1536;

		// Token: 0x04003948 RID: 14664
		public int originalSizeY = 768;
	}
}
