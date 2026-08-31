using System;
using UnityEngine;

namespace Pixelplacement
{
	// Token: 0x0200051F RID: 1311
	public class LayerMaskHelper
	{
		// Token: 0x06002052 RID: 8274 RVA: 0x000A81EB File Offset: 0x000A63EB
		public static int OnlyIncluding(params int[] layers)
		{
			return LayerMaskHelper.MakeMask(layers);
		}

		// Token: 0x06002053 RID: 8275 RVA: 0x000A81F3 File Offset: 0x000A63F3
		public static int Everything()
		{
			return -1;
		}

		// Token: 0x06002054 RID: 8276 RVA: 0x000A81F6 File Offset: 0x000A63F6
		public static int Default()
		{
			return 1;
		}

		// Token: 0x06002055 RID: 8277 RVA: 0x000A81F9 File Offset: 0x000A63F9
		public static int Nothing()
		{
			return 0;
		}

		// Token: 0x06002056 RID: 8278 RVA: 0x000A81FC File Offset: 0x000A63FC
		public static int EverythingBut(params int[] layers)
		{
			return ~LayerMaskHelper.MakeMask(layers);
		}

		// Token: 0x06002057 RID: 8279 RVA: 0x000A8205 File Offset: 0x000A6405
		public static bool ContainsLayer(LayerMask layerMask, int layer)
		{
			return (layerMask.value & (1 << layer)) != 0;
		}

		// Token: 0x06002058 RID: 8280 RVA: 0x000A8218 File Offset: 0x000A6418
		private static int MakeMask(params int[] layers)
		{
			int num = 0;
			foreach (int num2 in layers)
			{
				num |= 1 << num2;
			}
			return num;
		}
	}
}
