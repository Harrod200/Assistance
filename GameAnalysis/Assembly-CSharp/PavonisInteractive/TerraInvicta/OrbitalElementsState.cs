using System;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F0 RID: 2032
	public struct OrbitalElementsState
	{
		// Token: 0x17000E84 RID: 3716
		// (get) Token: 0x060048FE RID: 18686 RVA: 0x001E01B0 File Offset: 0x001DE3B0
		public Vector3d normalVector
		{
			get
			{
				return this.GetOrbitNormalVector();
			}
		}

		// Token: 0x17000E85 RID: 3717
		// (get) Token: 0x060048FF RID: 18687 RVA: 0x001E01B8 File Offset: 0x001DE3B8
		public Vector3d ascendingNodeVector
		{
			get
			{
				return this.AscendingNodeDirection();
			}
		}

		// Token: 0x17000E86 RID: 3718
		// (get) Token: 0x06004900 RID: 18688 RVA: 0x001E01C0 File Offset: 0x001DE3C0
		public Vector3d periapsisVector
		{
			get
			{
				return this.PeriapsisPosition();
			}
		}

		// Token: 0x17000E87 RID: 3719
		// (get) Token: 0x06004901 RID: 18689 RVA: 0x001E01C8 File Offset: 0x001DE3C8
		public Vector3d eccentricVector
		{
			get
			{
				return this.periapsisVector.normalized * this.eccentricity;
			}
		}

		// Token: 0x17000E88 RID: 3720
		// (get) Token: 0x06004902 RID: 18690 RVA: 0x001E01EE File Offset: 0x001DE3EE
		public double periapsis_m
		{
			get
			{
				return this.semiMajorAxis_m * (1.0 - this.eccentricity);
			}
		}

		// Token: 0x17000E89 RID: 3721
		// (get) Token: 0x06004903 RID: 18691 RVA: 0x001E0207 File Offset: 0x001DE407
		public double apoapsis_m
		{
			get
			{
				return this.semiMajorAxis_m * (1.0 + this.eccentricity);
			}
		}

		// Token: 0x06004904 RID: 18692 RVA: 0x001E0220 File Offset: 0x001DE420
		public OrbitalElementsState(double longAscendingNode_Rad, double argPeriapsis_Rad, double inclination_Rad, double semiMajorAxis_m, double ecc, double meanAnomalyAtEpoch_Rad, DateTime epoch)
		{
			this = default(OrbitalElementsState);
			this.longAscendingNode_Rad = longAscendingNode_Rad;
			this.argPeriapsis_Rad = argPeriapsis_Rad;
			this.inclination_Rad = inclination_Rad;
			this.semiMajorAxis_m = semiMajorAxis_m;
			this.eccentricity = ecc;
			this.meanAnomalyAtEpoch_Rad = meanAnomalyAtEpoch_Rad;
			this.epoch = epoch;
		}

		// Token: 0x06004905 RID: 18693 RVA: 0x001E025E File Offset: 0x001DE45E
		public OrbitalElementsState(double longAscendingNode_Rad, double argPeriapsis_Rad, double inclination_Rad, double semiMajorAxis_m, double ecc, double meanAnomalyAtEpoch_Rad, TIDateTime epoch)
		{
			this = new OrbitalElementsState(longAscendingNode_Rad, argPeriapsis_Rad, inclination_Rad, semiMajorAxis_m, ecc, meanAnomalyAtEpoch_Rad, epoch.ExportTime());
		}

		// Token: 0x06004906 RID: 18694 RVA: 0x001E0276 File Offset: 0x001DE476
		public OrbitalElementsState(OrbitalElementsState orbit)
		{
			this = new OrbitalElementsState(orbit.longAscendingNode_Rad, orbit.argPeriapsis_Rad, orbit.inclination_Rad, orbit.semiMajorAxis_m, orbit.eccentricity, orbit.meanAnomalyAtEpoch_Rad, orbit.epoch);
		}

		// Token: 0x06004907 RID: 18695 RVA: 0x001E02A8 File Offset: 0x001DE4A8
		public OrbitalElementsState(OrbitalElementsState orbit, double meanAnomalyAtEpoch_Rad, TIDateTime epoch)
		{
			this = new OrbitalElementsState(orbit);
			this.meanAnomalyAtEpoch_Rad = meanAnomalyAtEpoch_Rad;
			this.epoch = epoch.ExportTime();
		}

		// Token: 0x06004908 RID: 18696 RVA: 0x001E02C4 File Offset: 0x001DE4C4
		public OrbitalElementsState(Orbit orbit)
		{
			this = new OrbitalElementsState(orbit.LongitudeAscendingNode_Rad, orbit.ArgumentPeriapsis_Rad, orbit.Inclination_Rad, orbit.SemimajorAxis_m, orbit.Eccentricity, orbit.MeanAnomalyAtEpoch_Rad, orbit.Epoch);
		}

		// Token: 0x06004909 RID: 18697 RVA: 0x001E02F6 File Offset: 0x001DE4F6
		public OrbitalElementsState(ITransferTarget target, double meanAnomalyAtEpoch_Rad, TIDateTime epoch)
		{
			this = new OrbitalElementsState(target.Ω_rad(), target.ω_rad(), target.i_rad(), target.a_m(), target.e(), meanAnomalyAtEpoch_Rad, epoch);
		}

		// Token: 0x0600490A RID: 18698 RVA: 0x001E031E File Offset: 0x001DE51E
		public OrbitalElementsState(TIOrbitState orbit, double meanAnomalyAtEpoch_Rad, DateTime epoch)
		{
			this = new OrbitalElementsState(orbit.longitudeAscendingNode_Rad, orbit.argPeriapsis_Rad, orbit.inclination_Rad, orbit.semiMajorAxis_m, orbit.eccentricity, meanAnomalyAtEpoch_Rad, epoch);
		}

		// Token: 0x0600490B RID: 18699 RVA: 0x001E0346 File Offset: 0x001DE546
		public OrbitalElementsState(TISpaceObjectState spaceObject)
		{
			this = new OrbitalElementsState(spaceObject.longAscendingNode_Rad, spaceObject.argPeriapsis_Rad, spaceObject.inclination_Rad, spaceObject.semiMajorAxis_m, spaceObject.ecc, spaceObject.meanAnomalyAtEpoch_Rad, spaceObject.epoch_DateTime.ExportTime());
		}

		// Token: 0x0600490C RID: 18700 RVA: 0x001E037D File Offset: 0x001DE57D
		public OrbitalElementsState(TISpaceFleetState fleet)
		{
			this = new OrbitalElementsState(fleet);
		}

		// Token: 0x0600490D RID: 18701 RVA: 0x001E0388 File Offset: 0x001DE588
		public OrbitalElementsState(IMobileAsset fleet)
		{
			this = new OrbitalElementsState(fleet.Ω_rad(), fleet.ω_rad(), fleet.i_rad(), fleet.a_m(), fleet.e(), fleet.M0_rad(), new TIDateTime().SetTime(fleet.common_t0_jy(fleet.barycenter())));
		}

		// Token: 0x0600490E RID: 18702 RVA: 0x001E03D8 File Offset: 0x001DE5D8
		public double MeanAnomalyAtTime_Rad(DateTime time, double barycenterMass_kg)
		{
			double totalSeconds = (time - this.epoch).TotalSeconds;
			if (this.eccentricity < 1.0)
			{
				return Mathd.Normalize_Rad(this.meanAnomalyAtEpoch_Rad + 6.283185307179586 * (totalSeconds / this.OrbitalPeriod(barycenterMass_kg)));
			}
			double num = 6.67384E-11 * barycenterMass_kg;
			return totalSeconds * Mathd.Sqrt(num / (-this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m));
		}

		// Token: 0x0600490F RID: 18703 RVA: 0x001E0455 File Offset: 0x001DE655
		public double MeanLongitudeAtTime_Rad(DateTime time, double barycenterMass_kg)
		{
			return this.longAscendingNode_Rad + this.argPeriapsis_Rad + this.MeanAnomalyAtTime_Rad(time, barycenterMass_kg);
		}

		// Token: 0x06004910 RID: 18704 RVA: 0x001E0470 File Offset: 0x001DE670
		public double TrueAnomalyAtTime_Rad(DateTime time, double barycenterMass_kg)
		{
			double num = this.MeanAnomalyAtTime_Rad(time, barycenterMass_kg);
			double num3;
			int num4;
			double num6;
			if (this.eccentricity < 1.0)
			{
				double num2 = Mathd.Min(this.eccentricity, 0.9);
				num3 = num;
				num4 = 0;
				double num5;
				do
				{
					num5 = num3;
					num3 = num5 - (num5 - this.eccentricity * Mathd.Sin(num5) - num) / (1.0 - num2 * Mathd.Cos(num5));
				}
				while (Mathd.Abs(num3 - num5) >= 1E-06 && num4++ < 1000);
				num6 = 2.0 * Mathd.Atan2(Mathd.Sqrt(1.0 + this.eccentricity) * Mathd.Sin(num3 / 2.0), Mathd.Sqrt(1.0 - this.eccentricity) * Mathd.Cos(num3 / 2.0));
				return Mathd.Normalize_Rad(num6);
			}
			if (num > 10.0)
			{
				num3 = Mathd.Log(num / this.eccentricity);
			}
			else if (num < -10.0)
			{
				num3 = -Mathd.Log(-num / this.eccentricity);
			}
			else
			{
				num3 = num;
			}
			num4 = 0;
			double num7;
			do
			{
				num7 = num3;
				num3 = num7 - (this.eccentricity * Mathd.Sinh(num7) - num7 - num) / (this.eccentricity * Mathd.Cosh(num7) - 1.0);
			}
			while (Mathd.Abs(num3 - num7) >= 1E-06 && num4++ < 1000);
			num6 = Mathd.Acos((Mathd.Cosh(num3) - this.eccentricity) / (1.0 - this.eccentricity * Mathd.Cosh(num3)));
			if (num3 == 0.0)
			{
				num6 = 0.0;
			}
			if (num3 < 0.0)
			{
				num6 = -num6;
			}
			return num6;
		}

		// Token: 0x06004911 RID: 18705 RVA: 0x001E064C File Offset: 0x001DE84C
		public DateTime NextTimeAtMeanAnomaly(double meanAnomaly_rad, DateTime earliestTime, double barycenterMass_kg)
		{
			if (this.semiMajorAxis_m == 0.0)
			{
				Log.Error("NextTimeAtMeanAnomaly: semi major axis was " + this.semiMajorAxis_m.ToString() + " meters", Array.Empty<object>());
			}
			if (double.IsInfinity(barycenterMass_kg) || double.IsNaN(barycenterMass_kg))
			{
				Log.Error("NextTimeAtMeanAnomaly: barycenter mass was " + barycenterMass_kg.ToString() + " kg", Array.Empty<object>());
				barycenterMass_kg = 1.891E+30;
			}
			if (double.IsInfinity(meanAnomaly_rad) || double.IsNaN(meanAnomaly_rad))
			{
				Log.Error("NextTimeAtMeanAnomaly: target mean anomaly was " + meanAnomaly_rad.ToString() + " radians", Array.Empty<object>());
				meanAnomaly_rad = 0.0;
			}
			double totalSeconds = (earliestTime - this.epoch).TotalSeconds;
			if (this.eccentricity < 1.0)
			{
				double num = Mathd.Clamp(this.OrbitalPeriod(barycenterMass_kg), 1.0, 315569240.0);
				int num2 = Mathd.FloorToInt(totalSeconds / num);
				double num3 = Mathd.ClampRadiansTwoPI(meanAnomaly_rad - this.meanAnomalyAtEpoch_Rad) * num / 6.283185307179586;
				DateTime dateTime = this.epoch.AddSeconds((double)num2 * num + num3);
				if (dateTime < earliestTime)
				{
					dateTime = dateTime.AddSeconds(num);
				}
				return dateTime;
			}
			return this.TimeAtMeanAnomaly_Hyperbola(meanAnomaly_rad, barycenterMass_kg);
		}

		// Token: 0x06004912 RID: 18706 RVA: 0x001E07A0 File Offset: 0x001DE9A0
		public DateTime PreviousTimeAtMeanAnomaly(double meanAnomaly_rad, DateTime latestTime, double barycenterMass_kg)
		{
			if (this.eccentricity < 1.0)
			{
				return this.NextTimeAtMeanAnomaly(meanAnomaly_rad, latestTime, barycenterMass_kg).AddSeconds(-this.OrbitalPeriod(barycenterMass_kg));
			}
			return this.TimeAtMeanAnomaly_Hyperbola(meanAnomaly_rad, barycenterMass_kg);
		}

		// Token: 0x06004913 RID: 18707 RVA: 0x001E07E0 File Offset: 0x001DE9E0
		private DateTime TimeAtMeanAnomaly_Hyperbola(double meanAnomaly_rad, double barycenterMass_kg)
		{
			if (this.semiMajorAxis_m >= 0.0)
			{
				Log.Error("TimeAtMeanAnomaly_Hyperbola: semi-major axis was positive: " + this.semiMajorAxis_m.ToString(), Array.Empty<object>());
				this.semiMajorAxis_m = -this.semiMajorAxis_m;
			}
			double num = 6.67384E-11 * barycenterMass_kg;
			double num2 = Mathd.Sqrt(-this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m / num);
			double num3 = this.meanAnomalyAtEpoch_Rad * num2;
			if (num3 > 200000000000.0 || num3 < -200000000000.0 || double.IsNaN(num3))
			{
				Log.Error("TimeAtMeanAnomaly_Hyperbola: time since epoch was more than 6 thousand years: " + num3.ToString() + " seconds.", Array.Empty<object>());
				if (num3 < 0.0)
				{
					num3 = -200000000000.0;
				}
				else
				{
					num3 = 200000000000.0;
				}
			}
			DateTime dateTime = this.epoch.AddSeconds(-num3);
			double num4 = meanAnomaly_rad * num2;
			if (num4 > 200000000000.0 || num4 < -200000000000.0 || double.IsNaN(num4))
			{
				Log.Error("TimeAtMeanAnomaly_Hyperbola: time since periapsis was more than 6 thousand years: " + num4.ToString() + " seconds.", Array.Empty<object>());
				if (num4 < 0.0)
				{
					num4 = -200000000000.0;
				}
				else
				{
					num4 = 200000000000.0;
				}
			}
			return dateTime.AddSeconds(num4);
		}

		// Token: 0x06004914 RID: 18708 RVA: 0x001E0944 File Offset: 0x001DEB44
		public CartesianState ToCartesianStateAtTime(DateTime time, double barycenterMass_kg)
		{
			double num = this.semiMajorAxis_m;
			double num2 = this.eccentricity;
			double num3 = this.meanAnomalyAtEpoch_Rad;
			Vector3d zero = Vector3d.zero;
			Vector3d zero2 = Vector3d.zero;
			double totalSeconds = (time - this.epoch).TotalSeconds;
			if (num2 < 1.0)
			{
				num3 += 6.283185307179586 * (totalSeconds / this.OrbitalPeriod(barycenterMass_kg));
				num3 = Mathd.Normalize_Rad(num3);
			}
			else
			{
				double num4 = Mathd.Sqrt(6.67384E-11 * barycenterMass_kg / Mathd.Pow(-num, 3.0));
				num3 += num4 * totalSeconds;
			}
			return this.ToCartesianStateAtMeanAnomaly(num3, barycenterMass_kg);
		}

		// Token: 0x06004915 RID: 18709 RVA: 0x001E09E4 File Offset: 0x001DEBE4
		public CartesianState ToCartesianStateAtMeanAnomaly(double meanAnomaly_Rad, double barycenterMass_kg)
		{
			double num = this.longAscendingNode_Rad;
			double num2 = this.argPeriapsis_Rad;
			double num3 = this.inclination_Rad;
			double num4 = this.semiMajorAxis_m;
			double num5 = this.eccentricity;
			Vector3d vector3d = Vector3d.zero;
			Vector3d vector3d2 = Vector3d.zero;
			if (num5 < 1.0)
			{
				double num6 = this.GetEccentricAnomalyFromMeanAnomaly(meanAnomaly_Rad);
				double num7 = this.GetTrueAnomalyFromEccentricAnomaly(num6);
				double num8 = num4 * (1.0 - Mathd.Pow(num5, 2.0)) / (1.0 + num5 * Mathd.Cos(num7));
				double num9 = num7 + num2;
				num9 = Mathd.Normalize_Rad(num9);
				Vector3d vector3d3;
				Vector3d vector3d4;
				this.AnglesToCartesianState(num, num9, num3, out vector3d3, out vector3d4);
				vector3d = vector3d3 * num8;
				if (barycenterMass_kg != 0.0)
				{
					double num10 = Mathd.Atan(num5 * Mathd.Sin(num7) / (1.0 + num5 * Mathd.Cos(num7)));
					Vector3d vector3d5 = vector3d4 * Mathd.Cos(num10) + Vector3d.Scale(vector3d3 * Mathd.Sin(num10), new Vector3d(-1.0, -1.0, 1.0));
					double num11 = Mathd.Sqrt(6.67384E-11 * barycenterMass_kg * (2.0 / vector3d.magnitude - 1.0 / num4));
					vector3d2 = Vector3d.Scale(vector3d5 * num11, new Vector3d(-1.0, -1.0, 1.0));
				}
			}
			else
			{
				double num6 = this.GetEccentricAnomalyFromMeanAnomaly(meanAnomaly_Rad);
				double num7 = this.GetTrueAnomalyFromEccentricAnomaly(num6);
				double num8 = num4 * (1.0 - num5 * num5) / (1.0 + num5 * Mathd.Cos(num7));
				double num9 = num7 + num2;
				num9 = Mathd.Normalize_Rad(num9);
				Vector3d vector3d6;
				Vector3d vector3d7;
				this.AnglesToCartesianState(num, num9, num3, out vector3d6, out vector3d7);
				vector3d = vector3d6 * num8;
				if (barycenterMass_kg != 0.0)
				{
					double num12 = Mathd.Atan(num5 * Mathd.Sin(num7) / (1.0 + num5 * Mathd.Cos(num7)));
					Vector3d vector3d8 = vector3d7 * Mathd.Cos(num12) + Vector3d.Scale(vector3d6 * Mathd.Sin(num12), new Vector3d(-1.0, -1.0, 1.0));
					double num13 = Mathd.Sqrt(6.67384E-11 * barycenterMass_kg * (2.0 / vector3d.magnitude - 1.0 / num4));
					vector3d2 = Vector3d.Scale(vector3d8 * num13, new Vector3d(-1.0, -1.0, 1.0));
				}
			}
			return new CartesianState(vector3d, vector3d2);
		}

		// Token: 0x06004916 RID: 18710 RVA: 0x001E0CB8 File Offset: 0x001DEEB8
		public double GetTrueAnomalyFromEccentricAnomaly(double eccentricAnomaly_Rad)
		{
			if (eccentricAnomaly_Rad == 0.0)
			{
				return 0.0;
			}
			double num;
			if (this.eccentricity < 1.0)
			{
				num = 2.0 * Mathd.Atan2(Mathd.Sqrt(1.0 + this.eccentricity) * Mathd.Sin(eccentricAnomaly_Rad / 2.0), Mathd.Sqrt(1.0 - this.eccentricity) * Mathd.Cos(eccentricAnomaly_Rad / 2.0));
				return Mathd.ClampRadiansTwoPI(num);
			}
			num = Mathd.Acos((Mathd.Cosh(eccentricAnomaly_Rad) - this.eccentricity) / (1.0 - this.eccentricity * Mathd.Cosh(eccentricAnomaly_Rad)));
			if (eccentricAnomaly_Rad < 0.0)
			{
				num = -num;
			}
			return num;
		}

		// Token: 0x06004917 RID: 18711 RVA: 0x001E0D8C File Offset: 0x001DEF8C
		public double GetEccentricAnomalyFromTrueAnomaly(double trueAnomaly_Rad)
		{
			if (trueAnomaly_Rad == 0.0)
			{
				return 0.0;
			}
			if (this.eccentricity < 1.0)
			{
				return 2.0 * Mathd.Atan2(Mathd.Sqrt(1.0 - this.eccentricity) * Mathd.Sin(trueAnomaly_Rad / 2.0), Mathd.Sqrt(1.0 + this.eccentricity) * Mathd.Cos(trueAnomaly_Rad / 2.0));
			}
			double num = Mathd.ACosh((Mathd.Cos(trueAnomaly_Rad) - 1.0) / (this.eccentricity * (Mathd.Cos(trueAnomaly_Rad) - 1.0)));
			if (trueAnomaly_Rad < 0.0)
			{
				num = -num;
			}
			return num;
		}

		// Token: 0x06004918 RID: 18712 RVA: 0x001E0E5C File Offset: 0x001DF05C
		public double GetEccentricAnomalyFromMeanAnomaly(double meanAnomaly_Rad)
		{
			double num2;
			if (this.eccentricity < 1.0)
			{
				double num = Mathd.Min(this.eccentricity, 0.9);
				num2 = meanAnomaly_Rad;
				int num3 = 0;
				do
				{
					double num4 = num2;
					num2 = num4 - (num4 - this.eccentricity * Mathd.Sin(num4) - meanAnomaly_Rad) / (1.0 - num * Mathd.Cos(num4));
					if (Mathd.Abs(num2 - num4) < 1E-06)
					{
						break;
					}
				}
				while (num3++ < 1000);
			}
			else
			{
				if (meanAnomaly_Rad > 10.0)
				{
					num2 = Mathd.Log(meanAnomaly_Rad / this.eccentricity);
				}
				else if (meanAnomaly_Rad < -10.0)
				{
					num2 = -Mathd.Log(-meanAnomaly_Rad / this.eccentricity);
				}
				else
				{
					num2 = meanAnomaly_Rad;
				}
				int num5 = 0;
				double num6;
				do
				{
					num6 = num2;
					num2 = num6 - (this.eccentricity * Mathd.Sinh(num6) - num6 - meanAnomaly_Rad) / (this.eccentricity * Mathd.Cosh(num6) - 1.0);
				}
				while (Mathd.Abs(num2 - num6) >= 1E-06 && num5++ < 1000);
			}
			return num2;
		}

		// Token: 0x06004919 RID: 18713 RVA: 0x001E0F75 File Offset: 0x001DF175
		public double OrbitalPeriod(double barycenterMass_kg)
		{
			return 6.283185307179586 * Mathd.Sqrt(this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m / (6.67384E-11 * barycenterMass_kg));
		}

		// Token: 0x0600491A RID: 18714 RVA: 0x001E0FA8 File Offset: 0x001DF1A8
		private void AnglesToCartesianState(double omega, double u, double i, out Vector3d position, out Vector3d velocity)
		{
			double num = Mathd.Sin(u);
			double num2 = Mathd.Cos(u);
			double num3 = Mathd.Sin(i);
			double num4 = Mathd.Cos(i);
			double num5 = Mathd.Sin(omega);
			double num6 = Mathd.Cos(omega);
			double num7 = num * num4;
			double num8 = num6 * num2 - num5 * num7;
			double num9 = num5 * num2 + num6 * num7;
			double num10 = num * num3;
			position = new Vector3d(num8, num9, num10);
			double num11 = num2 * num4;
			double num12 = num6 * num + num5 * num11;
			double num13 = num5 * num - num6 * num11;
			double num14 = num2 * num3;
			velocity = new Vector3d(num12, num13, num14);
		}

		// Token: 0x0600491B RID: 18715 RVA: 0x001E104C File Offset: 0x001DF24C
		private Vector3d GetOrbitNormalVector()
		{
			Vector3d vector3d = this.AscendingNodeDirection();
			double num = Mathd.Cos(this.inclination_Rad);
			double num2 = Mathd.Sin(this.inclination_Rad);
			return new Vector3d(vector3d.y * num2, -vector3d.x * num2, num);
		}

		// Token: 0x0600491C RID: 18716 RVA: 0x001E108F File Offset: 0x001DF28F
		private Vector3d AscendingNodeDirection()
		{
			return new Vector3d(Mathd.Cos(this.longAscendingNode_Rad), Mathd.Sin(this.longAscendingNode_Rad), 0.0);
		}

		// Token: 0x0600491D RID: 18717 RVA: 0x001E10B8 File Offset: 0x001DF2B8
		private Vector3d PeriapsisPosition()
		{
			double num = this.semiMajorAxis_m - this.semiMajorAxis_m * this.eccentricity;
			return this.PeriapsisDirection() * num;
		}

		// Token: 0x0600491E RID: 18718 RVA: 0x001E10E8 File Offset: 0x001DF2E8
		public Vector3d PeriapsisDirection()
		{
			Vector3d vector3d = this.AscendingNodeDirection();
			Vector3d orbitNormalVector = this.GetOrbitNormalVector();
			return vector3d * Mathd.Cos(this.argPeriapsis_Rad) + Vector3d.Cross(orbitNormalVector, vector3d) * Mathd.Sin(this.argPeriapsis_Rad);
		}

		// Token: 0x0600491F RID: 18719 RVA: 0x001E1130 File Offset: 0x001DF330
		public ValueTuple<double, double>? GetMeanAnomalyWhenAtRadius(double radius_m, TINaturalSpaceObjectState barycenter)
		{
			if (this.eccentricity < 1.0)
			{
				if (this.periapsis_m > radius_m || this.apoapsis_m < radius_m)
				{
					return null;
				}
				if (this.periapsis_m == radius_m)
				{
					return new ValueTuple<double, double>?(new ValueTuple<double, double>(0.0, 0.0));
				}
				if (this.apoapsis_m == radius_m)
				{
					return new ValueTuple<double, double>?(new ValueTuple<double, double>(3.141592653589793, 3.141592653589793));
				}
				double num = Mathd.Acos((this.semiMajorAxis_m - this.semiMajorAxis_m * this.eccentricity * this.eccentricity - radius_m) / (this.eccentricity * radius_m));
				double num2 = 2.0 * Mathd.Atan(Mathd.Sqrt((1.0 - this.eccentricity) / (1.0 + this.eccentricity)) * Mathd.Tan(num / 2.0));
				double num3 = num2 - this.eccentricity * Mathd.Sin(num2);
				return new ValueTuple<double, double>?(new ValueTuple<double, double>(num3, -num3));
			}
			else
			{
				double num4 = Mathd.Acos(this.semiMajorAxis_m * (1.0 - this.eccentricity * this.eccentricity) / (this.eccentricity * radius_m) - 1.0 / this.eccentricity);
				if (double.IsNaN(num4))
				{
					return null;
				}
				double num5 = 2.0 * Mathd.Atanh(Mathd.Sqrt((this.eccentricity - 1.0) / (this.eccentricity + 1.0)) * Mathd.Tan(num4 / 2.0));
				double num6 = this.eccentricity * Mathd.Sinh(num5) - num5;
				return new ValueTuple<double, double>?(new ValueTuple<double, double>(num6, -num6));
			}
		}

		// Token: 0x06004920 RID: 18720 RVA: 0x001E1300 File Offset: 0x001DF500
		public bool Approximately(OrbitalElementsState b, double barycenterMass_kg = 0.0)
		{
			if (!Mathd.Approximately(this.semiMajorAxis_m, b.semiMajorAxis_m))
			{
				return false;
			}
			if (Mathd.Abs(this.eccentricity - b.eccentricity) > 1E-05)
			{
				return false;
			}
			if (!Mathd.Approximately(this.inclination_Rad, b.inclination_Rad))
			{
				return false;
			}
			if (!Mathd.Approximately(this.inclination_Rad, 0.0))
			{
				if (!Mathd.Approximately(this.longAscendingNode_Rad, b.longAscendingNode_Rad))
				{
					return false;
				}
				if (!Mathd.Approximately(this.eccentricity, 0.0) && !Mathd.Approximately(this.argPeriapsis_Rad, b.argPeriapsis_Rad))
				{
					return false;
				}
			}
			else if (this.eccentricity < 0.0 && !Mathd.Approximately(this.longAscendingNode_Rad, b.longAscendingNode_Rad))
			{
				return false;
			}
			return true;
		}

		// Token: 0x06004921 RID: 18721 RVA: 0x001E13D4 File Offset: 0x001DF5D4
		public double MeanAnomalyWhenClosestToVelocity_Rad(Vector3d localVelocity_mps)
		{
			if (this.eccentricity >= 1.0)
			{
				Log.Error("Cannot find mean anomaly when closest to velocity in hyperbolic case.  ecc = " + this.eccentricity.ToString(), Array.Empty<object>());
				return 0.0;
			}
			Vector3d normalVector = this.normalVector;
			Vector3d vector3d = this.PeriapsisDirection();
			Vector3d vector3d2 = Vector3d.Cross(normalVector, vector3d);
			double num = Vector3d.Dot(in localVelocity_mps, in vector3d);
			double num2 = Mathd.Atan2(Vector3d.Dot(in localVelocity_mps, in vector3d2) / Mathd.Sqrt(1.0 - this.eccentricity * this.eccentricity), num) - 1.5707963267948966;
			double meanAnomalyFromEccentricAnomaly = this.GetMeanAnomalyFromEccentricAnomaly(num2);
			if (double.IsNaN(meanAnomalyFromEccentricAnomaly) || double.IsInfinity(meanAnomalyFromEccentricAnomaly))
			{
				Log.Error(string.Concat(new string[]
				{
					"MeanAnomalyWhenClosestToVelocity_Rad: calculated mean anomaly was ",
					meanAnomalyFromEccentricAnomaly.ToString(),
					" radians\nlocalVelocity_mps = ",
					localVelocity_mps.ToString(),
					" m/s\nnormalDirection = ",
					normalVector.ToString(),
					"\nperiapsisDireciton = ",
					vector3d.ToString(),
					"\neccentricity = ",
					this.eccentricity.ToString()
				}), Array.Empty<object>());
				return 0.0;
			}
			return meanAnomalyFromEccentricAnomaly;
		}

		// Token: 0x06004922 RID: 18722 RVA: 0x001E1528 File Offset: 0x001DF728
		public double MeanAnomalyWhenClosestToPosition_Rad(Vector3d localPosition_m)
		{
			if (this.eccentricity >= 1.0)
			{
				Log.Error("Cannot find mean anomaly when closest to velocity in hyperbolic case.  ecc = " + this.eccentricity.ToString(), Array.Empty<object>());
				return 0.0;
			}
			Vector3d normalVector = this.normalVector;
			Vector3d vector3d = this.PeriapsisDirection();
			Vector3d vector3d2 = Vector3d.Cross(normalVector, vector3d);
			double num = Vector3d.Dot(in localPosition_m, in vector3d);
			double num2 = Mathd.Atan2(Vector3d.Dot(in localPosition_m, in vector3d2) / Mathd.Sqrt(1.0 - this.eccentricity * this.eccentricity), num);
			double meanAnomalyFromEccentricAnomaly = this.GetMeanAnomalyFromEccentricAnomaly(num2);
			if (double.IsNaN(meanAnomalyFromEccentricAnomaly) || double.IsInfinity(meanAnomalyFromEccentricAnomaly))
			{
				Log.Error(string.Concat(new string[]
				{
					"MeanAnomalyWhenClosestToPosition_Rad: calculated mean anomaly was ",
					meanAnomalyFromEccentricAnomaly.ToString(),
					" radians\nlocalPosition_mps = ",
					localPosition_m.ToString(),
					" m\nnormalDirection = ",
					normalVector.ToString(),
					"\nperiapsisDireciton = ",
					vector3d.ToString(),
					"\neccentricity = ",
					this.eccentricity.ToString()
				}), Array.Empty<object>());
				return 0.0;
			}
			return meanAnomalyFromEccentricAnomaly;
		}

		// Token: 0x06004923 RID: 18723 RVA: 0x001E166F File Offset: 0x001DF86F
		public double GetMeanAnomalyFromEccentricAnomaly(double eccentricAnomaly_Rad)
		{
			return eccentricAnomaly_Rad - this.eccentricity * Mathd.Sin(eccentricAnomaly_Rad);
		}

		// Token: 0x04002AD1 RID: 10961
		public DateTime epoch;

		// Token: 0x04002AD2 RID: 10962
		public double longAscendingNode_Rad;

		// Token: 0x04002AD3 RID: 10963
		public double argPeriapsis_Rad;

		// Token: 0x04002AD4 RID: 10964
		public double inclination_Rad;

		// Token: 0x04002AD5 RID: 10965
		public double semiMajorAxis_m;

		// Token: 0x04002AD6 RID: 10966
		public double eccentricity;

		// Token: 0x04002AD7 RID: 10967
		public double meanAnomalyAtEpoch_Rad;
	}
}
