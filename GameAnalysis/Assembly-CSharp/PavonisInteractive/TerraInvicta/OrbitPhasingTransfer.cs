using System;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000795 RID: 1941
	public class OrbitPhasingTransfer : ImpulseTransfer
	{
		// Token: 0x06003E43 RID: 15939 RVA: 0x001905A0 File Offset: 0x0018E7A0
		public TransferResult Solve(TIDateTime startTime, int numOrbits, bool goForward, ITransferTarget iOrigin, ITransferTarget iDestination, TISpaceFleetState destinationFleet, TINaturalSpaceObjectState commonBarycenter, OrbitalElementsState originOrbit, OrbitalElementsState destinationOrbit, OrbitalElementsState originInitialOrbit, TINaturalSpaceObjectState originInitialBarycenter, OrbitalElementsState destFinalOrbit, TINaturalSpaceObjectState destFinalBarycenter, double fleetAcceleration_mps2)
		{
			if (numOrbits < 1)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			double meanRadius_m = commonBarycenter.meanRadius_m;
			double hillRadius_m = commonBarycenter.hillRadius_m;
			double semiMajorAxis_m = destinationOrbit.semiMajorAxis_m;
			if (semiMajorAxis_m < meanRadius_m)
			{
				Log.Error(string.Concat(new string[]
				{
					"OrbitPhasingTransfer: the target orbit's radius (",
					semiMajorAxis_m.ToString(),
					"m) was inside the radius of the body it is orbiting (",
					meanRadius_m.ToString(),
					"m)."
				}), Array.Empty<object>());
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			ValueTuple<double, double> valueTuple = this.GetMicrothrustDurationAndGravityCost_s(originInitialBarycenter, originInitialOrbit, commonBarycenter, fleetAcceleration_mps2);
			this.originMicrothrustDuration_s = valueTuple.Item1;
			this.originGravityTax_s = valueTuple.Item2;
			valueTuple = this.GetMicrothrustDurationAndGravityCost_s(destFinalBarycenter, destFinalOrbit, commonBarycenter, fleetAcceleration_mps2);
			this.destinationMicrothrustDuration_s = valueTuple.Item1;
			this.destinationGravityTax_s = valueTuple.Item2;
			double num = OrbitPhasingTransfer.CalculateLongitudeDelta_Rad(originOrbit, destinationOrbit, commonBarycenter.mass_kg);
			double num2 = OrbitPhasingTransfer.CalculateLongitude_Rad(originOrbit, commonBarycenter.mass_kg);
			double mu = commonBarycenter.mu;
			double num3 = 6.283185307179586 * Mathd.Sqrt(semiMajorAxis_m * semiMajorAxis_m * semiMajorAxis_m / mu);
			double num5;
			if (goForward)
			{
				double num4 = num3 * num / 6.283185307179586;
				num5 = num3 - num4 / (double)numOrbits;
			}
			else
			{
				double num6 = num3 * (6.283185307179586 - num) / 6.283185307179586;
				num5 = num3 + num6 / (double)numOrbits;
			}
			double num7 = Mathd.Pow(mu * num5 * num5 / 39.47841760435743, 0.3333333333333333);
			double num8 = semiMajorAxis_m;
			double num9 = semiMajorAxis_m;
			double num10;
			double num11;
			if (goForward)
			{
				num9 = 2.0 * num7 - num8;
				if (num9 < meanRadius_m)
				{
					return new TransferResult(TransferResult.Outcome.Fail_WouldCollideWithBody, num9, meanRadius_m);
				}
				num10 = Mathd.ClampRadiansTwoPI(num2 - destinationOrbit.longAscendingNode_Rad + 3.141592653589793);
				num11 = 3.141592653589793;
			}
			else
			{
				num8 = 2.0 * num7 - num9;
				if (num8 > hillRadius_m && numOrbits > 1)
				{
					return new TransferResult(TransferResult.Outcome.Fail_WouldExceedHillRadius, num8, hillRadius_m);
				}
				num10 = Mathd.ClampRadiansTwoPI(num2 - destinationOrbit.longAscendingNode_Rad);
				num11 = 0.0;
			}
			double num12 = Mathd.Sqrt(mu / semiMajorAxis_m);
			double num13 = Mathd.Sqrt(mu * (2.0 / semiMajorAxis_m - 1.0 / num7));
			base.boost_DV_mps = Mathd.Abs(num12 - num13);
			base.decel_DV_mps = base.boost_DV_mps;
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			this.burn_duration_s = base.boost_DV_mps / fleetAcceleration_mps2;
			double num14 = originInitialOrbit.OrbitalPeriod(originInitialBarycenter.mass_kg);
			if (this.burn_duration_s * 2.0 > num14)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanHalfOrbit, this.burn_duration_s, num14);
			}
			double num15 = destinationOrbit.OrbitalPeriod(commonBarycenter.mass_kg);
			if (this.burn_duration_s * 2.0 > num15)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanHalfOrbit, this.burn_duration_s, num15);
			}
			this.transitDuration_s = num5 * (double)numOrbits + this.burn_duration_s;
			if (this.burn_duration_s * 2.0 > num5)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanHalfOrbit, this.burn_duration_s, num5);
			}
			double num16 = Mathd.Max(this.burn_duration_s / 2.0, this.originMicrothrustDuration_s);
			double num17 = Mathd.Max(this.burn_duration_s / 2.0, this.destinationMicrothrustDuration_s);
			this.transitDuration_s = num5 * (double)numOrbits + num16 + num17;
			IMobileAsset mobileAsset = iOrigin as IMobileAsset;
			bool? flag;
			if (mobileAsset == null)
			{
				flag = null;
			}
			else
			{
				TIFactionState faction = mobileAsset.faction;
				flag = ((faction != null) ? new bool?(faction.IsAlienFaction) : null);
			}
			bool? flag2 = flag;
			double num18 = (flag2.GetValueOrDefault() ? 78892310.0 : 78892310.0);
			if (this.transitDuration_s > num18)
			{
				return new TransferResult(TransferResult.Outcome.Fail_ExceedsMaxDuration, num18, 0.0);
			}
			num10 += this.burn_duration_s * num12 / (2.0 * semiMajorAxis_m);
			this.launchTime = new TIDateTime(startTime);
			this.arrivalTime = new TIDateTime(this.launchTime, this.transitDuration_s + num16 + num17);
			TIDateTime tidateTime = new TIDateTime(startTime);
			tidateTime.AddSeconds(this.burn_duration_s / 2.0);
			double num19 = 1.0 - num9 / num7;
			double longAscendingNode_Rad = destinationOrbit.longAscendingNode_Rad;
			double inclination_Rad = destinationOrbit.inclination_Rad;
			this._transferOrbit = new OrbitalElementsState(longAscendingNode_Rad, num10, inclination_Rad, num7, num19, num11, tidateTime.ExportTime());
			base.boost_DV_mps = (Mathd.Max(this.burn_duration_s / 2.0 + this.originMicrothrustDuration_s, this.burn_duration_s) + this.originGravityTax_s) * fleetAcceleration_mps2;
			base.decel_DV_mps = (Mathd.Max(this.burn_duration_s / 2.0 + this.destinationMicrothrustDuration_s, this.burn_duration_s) + this.destinationGravityTax_s) * fleetAcceleration_mps2;
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
		}

		// Token: 0x06003E44 RID: 15940 RVA: 0x00190ACC File Offset: 0x0018ECCC
		private static bool TryToGenerateDestinationOrbitGivenPossibleFleet(TIDateTime startTime, int numOrbits, ITransferTarget iOrigin, ITransferTarget iDestination, TISpaceFleetState destinationFleet, TINaturalSpaceObjectState commonBarycenter, out bool isTargetingDestinationFleetTransferDestination, out OrbitalElementsState destinationOrbit)
		{
			isTargetingDestinationFleetTransferDestination = false;
			if (!(destinationFleet == null))
			{
				IMobileAsset mobileAsset = iOrigin as IMobileAsset;
				if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destinationFleet, (mobileAsset != null) ? mobileAsset.faction : null))
				{
					if (!destinationFleet.trajectory.launched && iDestination.barycenter().FindCommonBarycenter(iOrigin.barycenter()) == commonBarycenter && Mathd.Approximately(iDestination.common_a_m(commonBarycenter), iOrigin.common_a_m(commonBarycenter)) && destinationFleet.trajectory.launchTime.DifferenceInSeconds(startTime) >= iDestination.common_period_days(commonBarycenter) * 86400.0 * (double)numOrbits)
					{
						if (iDestination.barycenter() == commonBarycenter)
						{
							destinationOrbit = new OrbitalElementsState(destinationFleet);
							return true;
						}
						if (iDestination.barycenterBarycenter() == commonBarycenter)
						{
							destinationOrbit = new OrbitalElementsState(iDestination.barycenter());
							return true;
						}
						destinationOrbit = new OrbitalElementsState(iDestination.barycenterBarycenter());
						return true;
					}
					else
					{
						OrbitalElementsState orbitalElementsState;
						TINaturalSpaceObjectState tinaturalSpaceObjectState;
						bool flag;
						destinationFleet.getOrbitalElementsState(new TIDateTime(destinationFleet.trajectory.finalArrivalTime, 1.0), out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
						if (!flag)
						{
							Debug.LogError("OrbitPhasingTransfer.Solve: destination fleet lacks a mean anomaly.");
						}
						if (tinaturalSpaceObjectState.FindCommonBarycenter(commonBarycenter) != commonBarycenter)
						{
							destinationOrbit = default(OrbitalElementsState);
							return false;
						}
						if (tinaturalSpaceObjectState == commonBarycenter)
						{
							destinationOrbit = orbitalElementsState;
						}
						else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
						{
							destinationOrbit = new OrbitalElementsState(tinaturalSpaceObjectState);
						}
						else
						{
							destinationOrbit = new OrbitalElementsState(tinaturalSpaceObjectState.barycenter);
						}
						if (!Mathd.Approximately(destinationOrbit.semiMajorAxis_m, iOrigin.a_m()))
						{
							destinationOrbit = default(OrbitalElementsState);
							return false;
						}
						if (numOrbits < 2147483647 && iOrigin.common_period_days(commonBarycenter) * 86400.0 * (double)(numOrbits + 1) < destinationFleet.trajectory.finalArrivalTime.DifferenceInSeconds(startTime))
						{
							destinationOrbit = default(OrbitalElementsState);
							return false;
						}
						isTargetingDestinationFleetTransferDestination = true;
						return true;
					}
				}
			}
			TINaturalSpaceObjectState barycenter;
			bool flag2;
			iDestination.getOrbitalElementsState(startTime, out destinationOrbit, out barycenter, out flag2);
			if (barycenter == commonBarycenter && !flag2)
			{
				return false;
			}
			if (barycenter != commonBarycenter)
			{
				if (barycenter.isSun)
				{
					return false;
				}
				destinationOrbit = new OrbitalElementsState(barycenter);
				barycenter = barycenter.barycenter;
			}
			if (barycenter != commonBarycenter)
			{
				if (barycenter.isSun)
				{
					return false;
				}
				destinationOrbit = new OrbitalElementsState(barycenter);
			}
			return true;
		}

		// Token: 0x06003E45 RID: 15941 RVA: 0x00190D34 File Offset: 0x0018EF34
		public static double CalculateLongitudeDelta_Rad(OrbitalElementsState start, OrbitalElementsState end, double barycenterMass_kg)
		{
			double num = OrbitPhasingTransfer.CalculateLongitude_Rad(end, barycenterMass_kg);
			double num2 = OrbitPhasingTransfer.CalculateLongitude_Rad(start, barycenterMass_kg);
			return Mathd.ClampRadiansTwoPI(end.longAscendingNode_Rad + end.argPeriapsis_Rad + num - num2);
		}

		// Token: 0x06003E46 RID: 15942 RVA: 0x00190D68 File Offset: 0x0018EF68
		public static double CalculateLongitude_Rad(OrbitalElementsState orbit, double barycenterMass_kg)
		{
			DateTime dateTime = TITimeState.Now().ExportTime();
			double num = orbit.MeanAnomalyAtTime_Rad(dateTime, barycenterMass_kg);
			return orbit.longAscendingNode_Rad + orbit.argPeriapsis_Rad + num;
		}

		// Token: 0x06003E47 RID: 15943 RVA: 0x00190D9C File Offset: 0x0018EF9C
		public static int CalculateMinOrbitsGivenAcceleration(OrbitalElementsState start, OrbitalElementsState end, TINaturalSpaceObjectState barycenter, double fleetAcceleration_mps2, bool isForward)
		{
			double num = end.OrbitalPeriod(barycenter.mass_kg);
			double semiMajorAxis_m = end.semiMajorAxis_m;
			double num2 = OrbitPhasingTransfer.CalculateLongitudeDelta_Rad(start, end, barycenter.mass_kg);
			double mu = barycenter.mu;
			double num3 = Mathd.Sqrt(mu / end.semiMajorAxis_m);
			return OrbitPhasingTransfer.CalculateMinOrbitsGivenAcceleration(num, semiMajorAxis_m, num3, num2, mu, fleetAcceleration_mps2, isForward);
		}

		// Token: 0x06003E48 RID: 15944 RVA: 0x00190DEC File Offset: 0x0018EFEC
		public static int CalculateMinOrbitsGivenAcceleration(double orbitPeriod_s, double orbitRadius_m, double orbitSpeed_mps, double angleToTravel_Rad, double mu, double fleetAcceleration_mps, bool isForward)
		{
			angleToTravel_Rad = Mathd.ClampRadiansTwoPI(angleToTravel_Rad);
			if (!isForward)
			{
				angleToTravel_Rad = 6.283185307179586 - angleToTravel_Rad;
			}
			double num = orbitPeriod_s / 2.0;
			double num2 = fleetAcceleration_mps * num;
			double num3 = orbitSpeed_mps + (isForward ? (-num2) : num2);
			double num4 = 1.0 / (2.0 / orbitRadius_m - num3 * num3 / mu);
			double num5 = Mathd.Sqrt(39.47841760435743 * num4 * num4 * num4 / mu);
			double num6 = orbitPeriod_s * angleToTravel_Rad / 6.283185307179586;
			double num7 = orbitPeriod_s - num5 * (double)(isForward ? 1 : (-1));
			return Mathd.CeilToInt(num6 / num7);
		}

		// Token: 0x06003E49 RID: 15945 RVA: 0x00190E8C File Offset: 0x0018F08C
		[return: TupleElementNames(new string[] { "microthrustDuration_s", "gravityCost_s" })]
		private ValueTuple<double, double> GetMicrothrustDurationAndGravityCost_s(TINaturalSpaceObjectState targetBarycenter, OrbitalElementsState targetOrbit, TINaturalSpaceObjectState phasingBarycenter, double fleetAcceleration_mps2)
		{
			if (targetBarycenter == phasingBarycenter)
			{
				return new ValueTuple<double, double>(0.0, 0.0);
			}
			if (targetBarycenter.barycenter != phasingBarycenter)
			{
				TINaturalSpaceObjectState barycenter = targetBarycenter.barycenter;
				if (((barycenter != null) ? barycenter.barycenter : null) != phasingBarycenter)
				{
					Log.Error("Attempting to orbit phase around a barycenter that we are not orbiting, even indirectly.", Array.Empty<object>());
					return new ValueTuple<double, double>(0.0, 0.0);
				}
			}
			double num = 0.0;
			double num2 = 0.0;
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, targetBarycenter.mu, targetBarycenter.sphereOfInfluence_m);
			double num3 = Mathd.Sqrt(targetBarycenter.mu / targetOrbit.semiMajorAxis_m);
			if (targetOrbit.semiMajorAxis_m < microthrustSphere.Radius_m)
			{
				num = microthrustSphere.GetDuration_s(num3);
				if (!microthrustSphere.IsLimitedBySphereOfInfluence)
				{
					num2 = targetBarycenter.localEscapeVelocity_mps(microthrustSphere.Radius_m) - microthrustSphere.OrbitalVelocityAtSphere_mps;
				}
			}
			else
			{
				num2 = targetBarycenter.localEscapeVelocity_mps(targetOrbit.semiMajorAxis_m) - num3;
			}
			if (targetBarycenter.barycenter == phasingBarycenter)
			{
				return new ValueTuple<double, double>(num, num2 / fleetAcceleration_mps2);
			}
			MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, targetBarycenter.barycenter.mu, targetBarycenter.barycenter.sphereOfInfluence_m);
			double num4 = Mathd.Sqrt(targetBarycenter.barycenter.mu / targetBarycenter.semiMajorAxis_m);
			if (targetBarycenter.semiMajorAxis_m < microthrustSphere2.Radius_m)
			{
				num += microthrustSphere2.GetDuration_s(num3);
				if (!microthrustSphere2.IsLimitedBySphereOfInfluence)
				{
					num2 += targetBarycenter.barycenter.localEscapeVelocity_mps(microthrustSphere2.Radius_m) - microthrustSphere2.OrbitalVelocityAtSphere_mps;
				}
			}
			else
			{
				num2 += targetBarycenter.barycenter.localEscapeVelocity_mps(targetBarycenter.semiMajorAxis_m) - num4;
			}
			return new ValueTuple<double, double>(num, num2 / fleetAcceleration_mps2);
		}

		// Token: 0x040026CE RID: 9934
		public double originMicrothrustDuration_s;

		// Token: 0x040026CF RID: 9935
		public double destinationMicrothrustDuration_s;

		// Token: 0x040026D0 RID: 9936
		public double burn_duration_s;

		// Token: 0x040026D1 RID: 9937
		public double originGravityTax_s;

		// Token: 0x040026D2 RID: 9938
		public double destinationGravityTax_s;

		// Token: 0x040026D3 RID: 9939
		public bool isGoingForward;
	}
}
