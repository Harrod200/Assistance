using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x02000924 RID: 2340
	public static class DragManager
	{
		// Token: 0x06005971 RID: 22897 RVA: 0x00291012 File Offset: 0x0028F212
		public static void ResetCurrentItem()
		{
			if (DragManager.currentItem != null)
			{
				DragManager.currentItem.Reset();
				DragManager.currentItem = null;
			}
			DragManager.canDropCurrentItem = false;
			DragManager.currentDragItemType = DragItemType.NONE;
		}

		// Token: 0x06005972 RID: 22898 RVA: 0x0029103D File Offset: 0x0028F23D
		public static void DestroyCurrentItem()
		{
			if (DragManager.currentItem != null)
			{
				global::UnityEngine.Object.Destroy(DragManager.currentItem.gameObject, 1f);
				DragManager.currentItem = null;
			}
			DragManager.canDropCurrentItem = false;
			DragManager.currentDragItemType = DragItemType.NONE;
		}

		// Token: 0x0400409E RID: 16542
		public static DragItem currentItem;

		// Token: 0x0400409F RID: 16543
		public static bool canDropCurrentItem;

		// Token: 0x040040A0 RID: 16544
		public static DragItemType currentDragItemType;
	}
}
