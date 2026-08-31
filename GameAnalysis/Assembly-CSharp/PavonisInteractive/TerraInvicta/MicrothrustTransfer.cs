using System;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000794 RID: 1940
	internal class MicrothrustTransfer : TrajectorySolver
	{
		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x06003E2C RID: 15916 RVA: 0x001900D7 File Offset: 0x0018E2D7
		// (set) Token: 0x06003E2D RID: 15917 RVA: 0x001900DF File Offset: 0x0018E2DF
		public double initialOrbit_m { get; private set; }

		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x06003E2E RID: 15918 RVA: 0x001900E8 File Offset: 0x0018E2E8
		// (set) Token: 0x06003E2F RID: 15919 RVA: 0x001900F0 File Offset: 0x0018E2F0
		public double destinationOrbit_m { get; private set; }

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x06003E30 RID: 15920 RVA: 0x001900F9 File Offset: 0x0018E2F9
		// (set) Token: 0x06003E31 RID: 15921 RVA: 0x00190101 File Offset: 0x0018E301
		public double initialInclination_rad { get; private set; }

		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x06003E32 RID: 15922 RVA: 0x0019010A File Offset: 0x0018E30A
		// (set) Token: 0x06003E33 RID: 15923 RVA: 0x00190112 File Offset: 0x0018E312
		public double destinationInclination_rad { get; private set; }

		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x06003E34 RID: 15924 RVA: 0x0019011B File Offset: 0x0018E31B
		// (set) Token: 0x06003E35 RID: 15925 RVA: 0x00190123 File Offset: 0x0018E323
		public bool ascending { get; private set; }

		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x06003E36 RID: 15926 RVA: 0x0019012C File Offset: 0x0018E32C
		// (set) Token: 0x06003E37 RID: 15927 RVA: 0x00190134 File Offset: 0x0018E334
		public double initialVelocity_mps { get; private set; }

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x06003E38 RID: 15928 RVA: 0x0019013D File Offset: 0x0018E33D
		// (set) Token: 0x06003E39 RID: 15929 RVA: 0x00190145 File Offset: 0x0018E345
		public double boostDuration_s { get; private set; }

		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x06003E3A RID: 15930 RVA: 0x0019014E File Offset: 0x0018E34E
		// (set) Token: 0x06003E3B RID: 15931 RVA: 0x00190156 File Offset: 0x0018E356
		public double decelDuration_s { get; private set; }

		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06003E3C RID: 15932 RVA: 0x0019015F File Offset: 0x0018E35F
		// (set) Token: 0x06003E3D RID: 15933 RVA: 0x00190167 File Offset: 0x0018E367
		public TINaturalSpaceObjectState commonBarycenter { get; private set; }

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06003E3E RID: 15934 RVA: 0x00190170 File Offset: 0x0018E370
		public override TIDateTime arrivalTime
		{
			get
			{
				return new TIDateTime(this.launchTime, this.transitDuration_s);
			}
		}

		// Token: 0x06003E3F RID: 15935 RVA: 0x00190184 File Offset: 0x0018E384
		public void Solve(TIDateTime launchTime, ITransferTarget originValue, OrbitalElementsState destinationOrbit, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2)
		{
			this.Solve(launchTime, originValue, destinationOrbit, null, commonBarycenter, fleetAcceleration_mps2, TITimeState.Now());
		}

		// Token: 0x06003E40 RID: 15936 RVA: 0x001901AC File Offset: 0x0018E3AC
		public void Solve(TIDateTime launchTime, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, TIDateTime earliestArrivalTime)
		{
			if (earliestArrivalTime == null)
			{
				earliestArrivalTime = TITimeState.Now();
			}
			double? num;
			if (destinationValue is TIOrbitState)
			{
				num = null;
			}
			else
			{
				num = new double?(destinationValue.common_M_rad(commonBarycenter, earliestArrivalTime));
			}
			TIDateTime tidateTime = earliestArrivalTime;
			IMobileAsset mobileAsset = originValue as IMobileAsset;
			if (!MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destinationValue as TISpaceFleetState, (mobileAsset != null) ? mobileAsset.faction : null))
			{
				tidateTime = TITimeState.Now();
			}
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			destinationValue.getOrbitalElementsState(tidateTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			if (tinaturalSpaceObjectState != commonBarycenter)
			{
				if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
				{
					orbitalElementsState = new OrbitalElementsState(tinaturalSpaceObjectState);
				}
				else
				{
					TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
					if (((barycenter != null) ? barycenter.barycenter : null) == commonBarycenter)
					{
						orbitalElementsState = new OrbitalElementsState(tinaturalSpaceObjectState.barycenter);
					}
					else
					{
						Log.Error("commonBarycenter is not common to destination barycenter.", Array.Empty<object>());
					}
				}
			}
			this.Solve(launchTime, originValue, orbitalElementsState, num, commonBarycenter, fleetAcceleration_mps2, earliestArrivalTime);
		}

		// Token: 0x06003E41 RID: 15937 RVA: 0x00190294 File Offset: 0x0018E494
		public void Solve(TIDateTime launchTime, ITransferTarget originValue, OrbitalElementsState destinationOrbit, double? destinationMeanAnomalyAtEarliestArrivalTime_rad, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, TIDateTime earliestArrivalTime)
		{
			if (destinationOrbit.eccentricity >= 1.0 || destinationOrbit.semiMajorAxis_m <= 0.0)
			{
				Log.Error("Microthrust spiral destination is hyperbolic -- this cannot be solved.", Array.Empty<object>());
			}
			this.launchTime = launchTime;
			this.initialOrbit_m = originValue.common_a_m(commonBarycenter);
			this.initialInclination_rad = originValue.common_i_rad(commonBarycenter);
			this.initialVelocity_mps = Mathd.Sqrt(commonBarycenter.mu / this.initialOrbit_m);
			this.destinationOrbit_m = destinationOrbit.semiMajorAxis_m;
			this.destinationInclination_rad = destinationOrbit.inclination_Rad;
			double num = Mathd.Sqrt(commonBarycenter.mu / this.destinationOrbit_m);
			double num2 = num - this.initialVelocity_mps;
			double num3 = Mathd.Abs(this.destinationInclination_rad - this.initialInclination_rad);
			if (this.destinationOrbit_m > this.initialOrbit_m)
			{
				this.ascending = true;
				base.decel_DV_mps = Mathd.Abs(num2) + 2.0 * num * Mathd.Sin(num3 / 2.0);
				this.decelDuration_s = base.decel_DV_mps / fleetAcceleration_mps2;
				this.transitDuration_s = this.decelDuration_s;
				base.DV_mps = base.decel_DV_mps;
			}
			else
			{
				this.ascending = false;
				base.boost_DV_mps = Mathd.Abs(num2) + 2.0 * this.initialVelocity_mps * Mathd.Sin(num3 / 2.0);
				this.boostDuration_s = base.boost_DV_mps / fleetAcceleration_mps2;
				this.transitDuration_s = this.boostDuration_s;
				base.DV_mps = base.boost_DV_mps;
			}
			if (destinationMeanAnomalyAtEarliestArrivalTime_rad == null)
			{
				return;
			}
			double num4 = destinationMeanAnomalyAtEarliestArrivalTime_rad.Value;
			double num5 = Mathd.Sqrt(commonBarycenter.mu / (this.initialOrbit_m * this.initialOrbit_m * this.initialOrbit_m));
			double num6 = Mathd.Sqrt(commonBarycenter.mu / (this.destinationOrbit_m * this.destinationOrbit_m * this.destinationOrbit_m));
			TIDateTime tidateTime = launchTime;
			launchTime = new TIDateTime(earliestArrivalTime, -this.transitDuration_s);
			if (launchTime < tidateTime)
			{
				launchTime = tidateTime;
				TIDateTime tidateTime2 = earliestArrivalTime;
				earliestArrivalTime = new TIDateTime(launchTime, this.transitDuration_s);
				num4 += num6 * earliestArrivalTime.DifferenceInSeconds(tidateTime2);
			}
			num4 = Mathd.ClampRadiansTwoPI(num4);
			double num7 = Mathd.ClampRadiansTwoPI(originValue.common_M_rad(commonBarycenter, launchTime));
			double num8 = destinationOrbit.longAscendingNode_Rad - originValue.common_Ω_rad(commonBarycenter) + destinationOrbit.argPeriapsis_Rad - originValue.common_ω_rad(commonBarycenter);
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, commonBarycenter.mu, commonBarycenter.sphereOfInfluence_m);
			double num9 = microthrustSphere.GetAnomalyDelta_Rad(num) - microthrustSphere.GetAnomalyDelta_Rad(this.initialVelocity_mps);
			double num10 = Mathd.ClampRadiansTwoPI(num7 + num8 + num9);
			double num11 = Mathd.ClampRadiansTwoPI(num4 - num10);
			double num12 = num6 - num5;
			if (num12 < 0.0)
			{
				num11 -= 6.283185307179586;
			}
			double num13 = num11 / num12;
			if (double.IsNaN(num13) || double.IsInfinity(num13) || num13 > 31556924.0)
			{
				Log.Error("We're using a pure microthrust spiral to reach an asset and the synodic period is so long that we can't synch with its mean anomaly.", Array.Empty<object>());
				return;
			}
			this.launchTime = new TIDateTime(launchTime, num13);
		}
	}
}
