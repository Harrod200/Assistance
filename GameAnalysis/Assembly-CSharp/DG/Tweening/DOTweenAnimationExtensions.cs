using System;
using UnityEngine;

namespace DG.Tweening
{
	// Token: 0x02000546 RID: 1350
	public static class DOTweenAnimationExtensions
	{
		// Token: 0x060022A4 RID: 8868 RVA: 0x000B3BEA File Offset: 0x000B1DEA
		public static bool IsSameOrSubclassOf<T>(this Component t)
		{
			return t is T;
		}
	}
}
