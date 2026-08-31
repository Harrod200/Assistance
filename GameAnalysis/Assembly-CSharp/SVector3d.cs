using System;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public struct SVector3d
{
	// Token: 0x06001769 RID: 5993 RVA: 0x00079B0E File Offset: 0x00077D0E
	public SVector3d(double radius, double polar, double azimuth)
	{
		this.radius = radius;
		this.polar = polar;
		this.azimuth = azimuth;
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x00079B28 File Offset: 0x00077D28
	public Vector3d ToCartesian()
	{
		double num = this.radius * Mathd.Cos(this.azimuth) * Mathd.Sin(this.polar);
		double num2 = this.radius * Mathd.Sin(this.azimuth) * Mathd.Sin(this.polar);
		double num3 = this.radius * Mathd.Cos(this.polar);
		return new Vector3d(num, num2, num3);
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x00079B8C File Offset: 0x00077D8C
	public static SVector3d ToSpherical(Vector3d v)
	{
		if (v.x == 0.0)
		{
			v.x = 1.401298E-45;
		}
		double num = Mathd.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		double num2 = Mathd.Atan(v.z / v.x);
		if (v.x < 0.0)
		{
			num2 += 3.141592653589793;
		}
		double num3 = Mathd.Asin(v.y / num);
		return new SVector3d(num, num2, num3);
	}

	// Token: 0x040015B9 RID: 5561
	public double radius;

	// Token: 0x040015BA RID: 5562
	public double polar;

	// Token: 0x040015BB RID: 5563
	public double azimuth;
}
