using System;
using UnityEngine;

// Token: 0x02000452 RID: 1106
public struct SVector3
{
	// Token: 0x06001766 RID: 5990 RVA: 0x000799F3 File Offset: 0x00077BF3
	public SVector3(float radius, float polar, float azimuth)
	{
		this.radius = radius;
		this.polar = polar;
		this.azimuth = azimuth;
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x00079A0C File Offset: 0x00077C0C
	public Vector3 ToCartesian()
	{
		float num = (float)((double)this.radius * Mathd.Cos((double)this.azimuth) * Mathd.Sin((double)this.polar));
		double num2 = (double)this.radius * Mathd.Sin((double)this.azimuth) * Mathd.Sin((double)this.polar);
		double num3 = (double)this.radius * Mathd.Cos((double)this.polar);
		return new Vector3(num, (float)num2, (float)num3);
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x00079A7C File Offset: 0x00077C7C
	public static SVector3 ToSpherical(Vector3 v)
	{
		if (v.x == 0f)
		{
			v.x = Mathf.Epsilon;
		}
		float num = Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		float num2 = Mathf.Atan2(v.z, v.x);
		if (v.x < 0f)
		{
			num2 += 3.1415927f;
		}
		float num3 = Mathf.Asin(v.y / num);
		return new SVector3(num, num2, num3);
	}

	// Token: 0x040015B6 RID: 5558
	public float radius;

	// Token: 0x040015B7 RID: 5559
	public float polar;

	// Token: 0x040015B8 RID: 5560
	public float azimuth;
}
