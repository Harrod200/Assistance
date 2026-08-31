using System;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007FA RID: 2042
	public static class RectTransformExtensions
	{
		// Token: 0x06004A2E RID: 18990 RVA: 0x001F23A8 File Offset: 0x001F05A8
		public static void ForceRebuildRecursive(this RectTransform rt)
		{
			foreach (object obj in rt)
			{
				((RectTransform)obj).ForceRebuildRecursive();
			}
			LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
		}
	}
}
