using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000723 RID: 1827
	public struct CouncilorIllustrationData
	{
		// Token: 0x06002D45 RID: 11589 RVA: 0x000F9A00 File Offset: 0x000F7C00
		public Vector3 GetIllustrationLocalPosition(Image image, Vector3 originalPositon)
		{
			float num = -image.rectTransform.sizeDelta.x * this.offset;
			Vector3 vector = new Vector3(num, 0f, 0f);
			return originalPositon + vector;
		}

		// Token: 0x040021B5 RID: 8629
		public string illustrationPath;

		// Token: 0x040021B6 RID: 8630
		public float offset;
	}
}
