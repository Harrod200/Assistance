using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008F6 RID: 2294
	public class UIPixelPerfectScrollViewOptimizer : MonoBehaviour
	{
		// Token: 0x060057F4 RID: 22516 RVA: 0x00285898 File Offset: 0x00283A98
		public void UpdatePixelPerfect()
		{
			foreach (Transform transform in this.scrollContent.Children())
			{
				Canvas component = transform.GetComponent<Canvas>();
				RectTransform component2 = transform.GetComponent<RectTransform>();
				if (component != null && component2 != null)
				{
					component.pixelPerfect = this.scrollContent.localPosition.y < component2.sizeDelta.y + Mathf.Abs(component2.localPosition.y) && this.scrollContent.localPosition.y + base.GetComponent<RectTransform>().rect.height > -component2.sizeDelta.y + Mathf.Abs(component2.localPosition.y);
				}
			}
		}

		// Token: 0x04003F7A RID: 16250
		public RectTransform scrollContent;
	}
}
