using System;
using UnityEngine;

namespace ModelShark
{
	// Token: 0x020004B4 RID: 1204
	public static class CanvasHelper
	{
		// Token: 0x06001B0C RID: 6924 RVA: 0x00092DD2 File Offset: 0x00090FD2
		public static Canvas GetRootCanvas()
		{
			if (CanvasHelper.CachedRootCanvas == null)
			{
				CanvasHelper.CachedRootCanvas = GameObject.Find("Dummy Canvas For Tooltip Manager").GetComponent<Canvas>();
			}
			return CanvasHelper.CachedRootCanvas;
		}

		// Token: 0x06001B0D RID: 6925 RVA: 0x00092DFA File Offset: 0x00090FFA
		public static Canvas GetHabCanvas()
		{
			return GameObject.Find("HabitatsScreenController").GetComponent<Canvas>();
		}

		// Token: 0x0400171D RID: 5917
		public static Canvas CachedRootCanvas;
	}
}
