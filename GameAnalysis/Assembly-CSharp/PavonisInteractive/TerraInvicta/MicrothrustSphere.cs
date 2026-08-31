using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000793 RID: 1939
	public class MicrothrustSphere
	{
		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06003E23 RID: 15907 RVA: 0x0018FF9E File Offset: 0x0018E19E
		public double Radius_m { get; }

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06003E24 RID: 15908 RVA: 0x0018FFA6 File Offset: 0x0018E1A6
		public double OrbitalVelocityAtSphere_mps { get; }

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x06003E25 RID: 15909 RVA: 0x0018FFAE File Offset: 0x0018E1AE
		public double Mu { get; }

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x06003E26 RID: 15910 RVA: 0x0018FFB6 File Offset: 0x0018E1B6
		public bool IsLimitedBySphereOfInfluence { get; }

		// Token: 0x06003E27 RID: 15911 RVA: 0x0018FFC0 File Offset: 0x0018E1C0
		public MicrothrustSphere(double fleetAcceleration_mps2, double mu, double sphereOfInfluence_m)
		{
			this.FleetAcceleration_mps2 = fleetAcceleration_mps2;
			this.Mu = mu;
			double num = Mathd.Sqrt(mu / (this.FleetAcceleration_mps2 * 2.0));
			this.IsLimitedBySphereOfInfluence = num > sphereOfInfluence_m;
			this.Radius_m = (this.IsLimitedBySphereOfInfluence ? sphereOfInfluence_m : num);
			this.OrbitalVelocityAtSphere_mps = Mathd.Sqrt(mu / this.Radius_m);
		}

		// Token: 0x06003E28 RID: 15912 RVA: 0x00190028 File Offset: 0x0018E228
		public double GetDuration_s(double velocity_mps)
		{
			return Mathd.Max(velocity_mps - this.OrbitalVelocityAtSphere_mps, 0.0) / this.FleetAcceleration_mps2;
		}

		// Token: 0x06003E29 RID: 15913 RVA: 0x00190048 File Offset: 0x0018E248
		public double GetAnomalyDelta_Rad(double velocity_mps)
		{
			if (velocity_mps < this.OrbitalVelocityAtSphere_mps)
			{
				return 0.0;
			}
			double num = Mathd.Abs(this.FourthPower(velocity_mps) - this.FourthPower(this.OrbitalVelocityAtSphere_mps)) / (4.0 * this.FleetAcceleration_mps2 * this.Mu);
			num %= 6.283185307179586;
			if (num < 0.0)
			{
				num += 6.283185307179586;
			}
			return num;
		}

		// Token: 0x06003E2A RID: 15914 RVA: 0x001900BF File Offset: 0x0018E2BF
		public double GetDeltaV_mps(double velocity_mps)
		{
			return Mathd.Abs(velocity_mps - this.OrbitalVelocityAtSphere_mps);
		}

		// Token: 0x06003E2B RID: 15915 RVA: 0x001900CE File Offset: 0x0018E2CE
		private double FourthPower(double x)
		{
			return x * x * x * x;
		}

		// Token: 0x040026BF RID: 9919
		public const double ACCELERATION_MULTIPLIER = 2.0;

		// Token: 0x040026C2 RID: 9922
		public double FleetAcceleration_mps2;
	}
}
