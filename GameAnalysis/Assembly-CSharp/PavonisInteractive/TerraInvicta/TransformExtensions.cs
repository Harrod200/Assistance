using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007FC RID: 2044
	public static class TransformExtensions
	{
		// Token: 0x06004A37 RID: 18999 RVA: 0x001F2444 File Offset: 0x001F0644
		public static void DestroyChildren(this Transform transform)
		{
			if (transform.childCount == 0)
			{
				return;
			}
			foreach (object obj in transform)
			{
				global::UnityEngine.Object.Destroy(((Transform)obj).gameObject);
			}
		}
	}
}
