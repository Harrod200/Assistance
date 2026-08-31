using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009FF RID: 2559
	public class Polar
	{
		// Token: 0x0600625A RID: 25178 RVA: 0x002E1EF8 File Offset: 0x002E00F8
		public static Vector3d ToCartesian(double radius, double inclination, double azimuth)
		{
			double num = radius * Mathd.Sin(inclination * 0.01745329238474369) * Mathd.Cos(azimuth * 0.01745329238474369);
			double num2 = radius * Mathd.Cos(inclination * 0.01745329238474369);
			double num3 = radius * Mathd.Sin(inclination * 0.01745329238474369) * Mathd.Sin(azimuth * 0.01745329238474369);
			return new Vector3d(num, num2, num3);
		}

		// Token: 0x0600625B RID: 25179 RVA: 0x002E1F66 File Offset: 0x002E0166
		public Polar(double radius, double inclination, double azimuth)
		{
			this.radius = radius;
			this.inclination = inclination;
			this.azimuth = azimuth;
		}

		// Token: 0x0600625C RID: 25180 RVA: 0x002E1F83 File Offset: 0x002E0183
		public Vector3d ToCartesian()
		{
			return Polar.ToCartesian(this.radius, this.inclination, this.azimuth);
		}

		// Token: 0x04004519 RID: 17689
		public double radius;

		// Token: 0x0400451A RID: 17690
		public double inclination;

		// Token: 0x0400451B RID: 17691
		public double azimuth;
	}
}
