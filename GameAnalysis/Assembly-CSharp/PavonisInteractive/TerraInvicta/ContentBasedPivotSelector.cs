using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008E0 RID: 2272
	public class ContentBasedPivotSelector : MonoBehaviour
	{
		// Token: 0x06005792 RID: 22418 RVA: 0x00284498 File Offset: 0x00282698
		private void LateUpdate()
		{
			this.content.pivot = ((this.content.sizeDelta.y < 0f) ? new Vector2(0.5f, this.defaultPivot) : new Vector2(0.5f, this.overflowPivot));
		}

		// Token: 0x04003F46 RID: 16198
		public RectTransform content;

		// Token: 0x04003F47 RID: 16199
		[Range(0f, 1f)]
		public float defaultPivot;

		// Token: 0x04003F48 RID: 16200
		[Range(0f, 1f)]
		public float overflowPivot;
	}
}
