using System;
using UnityEngine;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007FD RID: 2045
	public class CameraCleanup : MonoBehaviour
	{
		// Token: 0x06004A38 RID: 19000 RVA: 0x001F24A4 File Offset: 0x001F06A4
		private void OnDisable()
		{
			this.CleanupTexture();
		}

		// Token: 0x06004A39 RID: 19001 RVA: 0x001F24AC File Offset: 0x001F06AC
		private void OnDestroy()
		{
			this.CleanupTexture();
		}

		// Token: 0x06004A3A RID: 19002 RVA: 0x001F24B4 File Offset: 0x001F06B4
		private void CleanupTexture()
		{
			Camera component = base.GetComponent<Camera>();
			if (component != null && component.targetTexture != null)
			{
				RenderTexture targetTexture = component.targetTexture;
				component.targetTexture = null;
				targetTexture.Release();
			}
			VideoPlayer component2 = base.GetComponent<VideoPlayer>();
			if (component2 != null && component2.targetTexture != null)
			{
				RenderTexture targetTexture2 = component2.targetTexture;
				component2.targetTexture = null;
				targetTexture2.Release();
			}
		}
	}
}
