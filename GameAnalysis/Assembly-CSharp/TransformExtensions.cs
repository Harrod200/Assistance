using System;
using UnityEngine;

// Token: 0x02000439 RID: 1081
public static class TransformExtensions
{
	// Token: 0x0600166C RID: 5740 RVA: 0x0007278C File Offset: 0x0007098C
	public static void SetLayer(this Transform trans, int layer, bool includeChildren = true)
	{
		trans.gameObject.layer = layer;
		if (includeChildren)
		{
			foreach (object obj in trans)
			{
				((Transform)obj).SetLayer(layer, true);
			}
		}
	}
}
