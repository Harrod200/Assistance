using System;
using UnityEngine;

// Token: 0x02000405 RID: 1029
public static class Vector3Extension
{
	// Token: 0x0600152A RID: 5418 RVA: 0x00067154 File Offset: 0x00065354
	public static string ToDetailedString(this Vector3 v3)
	{
		return string.Format("({0}, {1}, {2})", v3.x, v3.y, v3.z);
	}
}
