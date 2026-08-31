using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F2 RID: 2034
	public class AerobreakCalculator
	{
		// Token: 0x17000E8B RID: 3723
		// (get) Token: 0x060049EA RID: 18922 RVA: 0x001F0C0F File Offset: 0x001EEE0F
		// (set) Token: 0x060049EB RID: 18923 RVA: 0x001F0C17 File Offset: 0x001EEE17
		public OrbitalElementsState hohmannOrbit { get; private set; }

		// Token: 0x17000E8C RID: 3724
		// (get) Token: 0x060049EC RID: 18924 RVA: 0x001F0C20 File Offset: 0x001EEE20
		public TIDateTime aerobreakTime
		{
			get
			{
				return new TIDateTime(this.hohmannOrbit.epoch);
			}
		}

		// Token: 0x17000E8D RID: 3725
		// (get) Token: 0x060049ED RID: 18925 RVA: 0x001F0C32 File Offset: 0x001EEE32
		public TIDateTime arrivalBurnTime
		{
			get
			{
				return new TIDateTime(this.hohmannOrbit.epoch, this.hohmannDuration_s);
			}
		}

		// Token: 0x17000E8E RID: 3726
		// (get) Token: 0x060049EE RID: 18926 RVA: 0x001F0C4A File Offset: 0x001EEE4A
		// (set) Token: 0x060049EF RID: 18927 RVA: 0x001F0C52 File Offset: 0x001EEE52
		public double arrivalBurnDV_mps { get; private set; }

		// Token: 0x17000E8F RID: 3727
		// (get) Token: 0x060049F0 RID: 18928 RVA: 0x001F0C5B File Offset: 0x001EEE5B
		// (set) Token: 0x060049F1 RID: 18929 RVA: 0x001F0C63 File Offset: 0x001EEE63
		public double hohmannDuration_s { get; private set; }

		// Token: 0x060049F2 RID: 18930 RVA: 0x001F0C6C File Offset: 0x001EEE6C
		public void Solve(TINaturalSpaceObjectState destinationBarycenter, OrbitalElementsState destinationOrbit, Vector3d approachVelocity, double? targetMeanAnomalyAtAerobreakTime_Rad, TIDateTime aerobreakTime, bool isPlayer = false)
		{
			double meanRadius_m = destinationBarycenter.meanRadius_m;
			double semiMajorAxis_m = destinationOrbit.semiMajorAxis_m;
			double num = (meanRadius_m + semiMajorAxis_m) / 2.0;
			double num2 = (meanRadius_m - semiMajorAxis_m) / (meanRadius_m + semiMajorAxis_m);
			this.hohmannDuration_s = 3.141592653589793 * Mathd.Sqrt(num * num * num / destinationBarycenter.mu);
			Vector3d normalVector = destinationOrbit.normalVector;
			Vector3d vector3d = Vector3d.Cross(approachVelocity, normalVector);
			Vector3d vector3d2 = vector3d.normalized * meanRadius_m;
			TIDateTime tidateTime = new TIDateTime(aerobreakTime);
			double num3 = destinationOrbit.OrbitalPeriod(destinationBarycenter.mass_kg);
			Vector3d vector3d3 = Vector3d.Cross(normalVector, approachVelocity) * semiMajorAxis_m;
			if (targetMeanAnomalyAtAerobreakTime_Rad != null)
			{
				TIDateTime tidateTime2 = new TIDateTime(aerobreakTime, this.hohmannDuration_s);
				double num4 = TISpaceAssetState.CalculateMeanAnomalyFromPosition(destinationOrbit, destinationBarycenter, vector3d3, tidateTime2, isPlayer) - targetMeanAnomalyAtAerobreakTime_Rad.GetValueOrDefault();
				if (num4 < 0.0)
				{
					num4 += 6.283185307179586;
				}
				double num5 = 6.283185307179586 / num3;
				double num6 = num4 / num5;
				tidateTime.AddSeconds(num6);
			}
			vector3d = Vector3d.Cross(vector3d2, approachVelocity);
			Vector3d normalized = vector3d.normalized;
			double num7 = Mathd.Acos(normalized.z);
			double num8 = Mathd.Acos(normalized.y / Mathd.Sin(num7));
			num8 = ((normalized.x > 0.0) ? (3.141592653589793 - num8) : (3.141592653589793 + num8));
			vector3d = Vector3d.Cross(new Vector3d(Mathd.Cos(num8), Mathd.Sin(num8), 0.0), vector3d2.normalized);
			double num9 = Vector3d.Dot(in vector3d, in normalized);
			double num10 = ((num9 > 0.0) ? Mathd.Asin(num9) : (3.141592653589793 - Mathd.Asin(num9)));
			if (num10 < 0.0)
			{
				num10 += 6.283185307179586;
			}
			this.hohmannOrbit = new OrbitalElementsState(num8, num10, num7, num, num2, 0.0, tidateTime);
			double num11 = semiMajorAxis_m * 6.283185307179586 / num3;
			vector3d = Vector3d.Cross(normalVector, vector3d3);
			Vector3d vector3d4 = num11 * vector3d.normalized;
			double num12 = Mathd.Sqrt(destinationBarycenter.mu / (2.0 / num + 1.0 / semiMajorAxis_m));
			vector3d = Vector3d.Cross(normalized, vector3d3);
			Vector3d vector3d5 = num12 * vector3d.normalized;
			vector3d = vector3d4 - vector3d5;
			this.arrivalBurnDV_mps = vector3d.magnitude;
		}
	}
}
