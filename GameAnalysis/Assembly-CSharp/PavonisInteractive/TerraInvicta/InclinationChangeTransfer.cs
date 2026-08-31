using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000790 RID: 1936
	public class InclinationChangeTransfer : TrajectorySolver
	{
		// Token: 0x06003DBC RID: 15804 RVA: 0x00184248 File Offset: 0x00182448
		public TransferResult Solve(TIDateTime startTime, double durationInDestinationOrbits, ITransferTarget iOrigin, ITransferTarget iDestination, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, bool anyMeanAnomalyAtArrival, double? startMeanAnomalyChangePerSecond_radPerSec = null, double? endMeanAnomalyChangePerSecond_radPerSec = null, bool iterating = false)
		{
			TISpaceAssetState tispaceAssetState = iOrigin as TISpaceAssetState;
			double num;
			TIDateTime tidateTime;
			if (tispaceAssetState != null)
			{
				num = tispaceAssetState.meanAnomalyAtEpoch_Rad;
				tidateTime = tispaceAssetState.epoch_DateTime;
			}
			else
			{
				num = iOrigin.common_M0_rad(commonBarycenter);
				tidateTime = new TIDateTime().SetTime(iOrigin.common_t0_jy(commonBarycenter));
			}
			TISpaceAssetState tispaceAssetState2 = iDestination as TISpaceAssetState;
			double num2;
			TIDateTime tidateTime2;
			if (tispaceAssetState2 != null)
			{
				num2 = tispaceAssetState2.meanAnomalyAtEpoch_Rad;
				tidateTime2 = tispaceAssetState2.epoch_DateTime;
			}
			else if (iDestination is TIOrbitState)
			{
				num2 = 0.0;
				tidateTime2 = startTime;
			}
			else
			{
				num2 = iDestination.common_M0_rad(commonBarycenter);
				tidateTime2 = new TIDateTime().SetTime(iDestination.common_t0_jy(commonBarycenter));
			}
			return this.Solve(startTime, durationInDestinationOrbits, new OrbitalElementsState(iOrigin, num, tidateTime), new OrbitalElementsState(iDestination, num2, tidateTime2), commonBarycenter, fleetAcceleration_mps2, anyMeanAnomalyAtArrival, startMeanAnomalyChangePerSecond_radPerSec, endMeanAnomalyChangePerSecond_radPerSec, iterating);
		}

		// Token: 0x06003DBD RID: 15805 RVA: 0x00184308 File Offset: 0x00182508
		public TransferResult Solve(TIDateTime startTime, double durationInDestinationOrbits, OrbitalElementsState origin, OrbitalElementsState destination, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, bool anyMeanAnomalyAtArrival, double? startMeanAnomalyChangePerSecond_radPerSec = null, double? endMeanAnomalyChangePerSecond_radPerSec = null, bool iterating = false)
		{
			double meanRadius_m = commonBarycenter.meanRadius_m;
			double hillRadius_m = commonBarycenter.hillRadius_m;
			Vector3d normalVector = origin.normalVector;
			Vector3d normalVector2 = destination.normalVector;
			Vector3d vector3d = Vector3d.Cross(normalVector, normalVector2);
			if (vector3d.sqrMagnitude == 0.0)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			double num = Mathd.Asin(vector3d.magnitude);
			vector3d = vector3d.normalized;
			Vector3d ascendingNodeVector = origin.ascendingNodeVector;
			Vector3d ascendingNodeVector2 = destination.ascendingNodeVector;
			double num2 = (Mathd.Asin(Vector3d.Cross(vector3d, ascendingNodeVector).magnitude) - origin.argPeriapsis_Rad + 12.566370614359172) % 6.283185307179586;
			double num3 = (num2 + 3.141592653589793 + 6.283185307179586) % 6.283185307179586;
			double num4 = (Mathd.Asin(Vector3d.Cross(vector3d, ascendingNodeVector2).magnitude) - destination.argPeriapsis_Rad + 12.566370614359172) % 6.283185307179586;
			double num5 = (num4 + 3.141592653589793 + 6.283185307179586) % 6.283185307179586;
			double num6 = origin.OrbitalPeriod(commonBarycenter.mass_kg);
			double num7 = destination.OrbitalPeriod(commonBarycenter.mass_kg);
			double num8 = origin.MeanAnomalyAtTime_Rad(startTime.ExportTime(), commonBarycenter.mass_kg);
			double num9 = (num2 - num8) % 6.283185307179586;
			if (num9 < 0.0)
			{
				num9 = 6.283185307179586 + num9;
			}
			double num10 = (num3 - num8) % 6.283185307179586;
			if (num10 < 0.0)
			{
				num10 = 6.283185307179586 + num10;
			}
			double num11 = num6 / 6.283185307179586;
			if (startMeanAnomalyChangePerSecond_radPerSec != null)
			{
				num11 = startMeanAnomalyChangePerSecond_radPerSec.Value;
			}
			double num12 = num9 * num11;
			double num13 = num10 * num11;
			bool flag = num12 < num13;
			double num14 = (flag ? num12 : num13);
			TIDateTime tidateTime = new TIDateTime(startTime, num14);
			double num15 = 0.0;
			if (!anyMeanAnomalyAtArrival)
			{
				double num16 = num7 / 6.283185307179586;
				if (endMeanAnomalyChangePerSecond_radPerSec != null)
				{
					num16 = endMeanAnomalyChangePerSecond_radPerSec.Value;
				}
				double num17 = destination.MeanAnomalyAtTime_Rad(tidateTime.ExportTime(), commonBarycenter.mass_kg);
				num15 = ((flag ? num4 : num5) - num17 + 6.283185307179586) % 6.283185307179586 * num16;
			}
			double num18 = durationInDestinationOrbits * num7 + num15;
			TIDateTime tidateTime2 = new TIDateTime(tidateTime, num18);
			double num19 = 2.0 * Mathd.Pow(commonBarycenter.mu * num18 * num18 / 39.47841760435743, 0.3333333333333333) - (origin.semiMajorAxis_m + destination.semiMajorAxis_m) / 2.0;
			bool flag2 = num19 > origin.semiMajorAxis_m;
			bool flag3 = num19 > destination.semiMajorAxis_m;
			if (num19 > hillRadius_m)
			{
				return new TransferResult(TransferResult.Outcome.Fail_WouldExceedHillRadius, num19, hillRadius_m);
			}
			if (num19 < meanRadius_m)
			{
				return new TransferResult(TransferResult.Outcome.Fail_WouldCollideWithBody, num19, meanRadius_m);
			}
			double num20 = (origin.semiMajorAxis_m + num19) / 2.0;
			double num21 = (destination.semiMajorAxis_m + num19) / 2.0;
			double num22 = Mathd.Sqrt(commonBarycenter.mu * (2.0 / origin.semiMajorAxis_m - 1.0 / num20));
			double num23 = Mathd.Sqrt(commonBarycenter.mu * (2.0 / destination.semiMajorAxis_m - 1.0 / num21));
			double num24 = Mathd.Sqrt(commonBarycenter.mu / origin.semiMajorAxis_m);
			double num25 = Mathd.Sqrt(commonBarycenter.mu / destination.semiMajorAxis_m);
			double num26 = Mathd.Abs(num22 - num24);
			double num27 = Mathd.Abs(num23 - num25);
			double num28 = num26 / fleetAcceleration_mps2;
			double num29 = num27 / fleetAcceleration_mps2;
			double num30 = Mathd.Sqrt(commonBarycenter.mu * (2.0 / num19 - 1.0 / num20));
			double num31 = Mathd.Sqrt(commonBarycenter.mu * (2.0 / num19 - 1.0 / num21));
			double num32 = Mathd.Sqrt(num30 * num30 + num31 * num31 - 2.0 * num30 * num31 * Mathd.Cos(num));
			double num33 = num32 / fleetAcceleration_mps2;
			if (num33 * 2.0 > num18)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanHalfOrbit, num33, num18);
			}
			double num34 = 3.141592653589793 * Mathd.Sqrt(num20 * num20 * num20 / commonBarycenter.mu);
			double num35 = 3.141592653589793 * Mathd.Sqrt(num21 * num21 * num21 / commonBarycenter.mu);
			base.DV_mps = num26 + num32 + num27;
			base.boost_DV_mps = num26;
			base.decel_DV_mps = num27;
			this.intermediate_burn_DV = num32;
			this.intermediateBurnTime = new TIDateTime(tidateTime, num34);
			double num36 = base.boost_DV_mps / fleetAcceleration_mps2;
			this.launchTime = new TIDateTime(tidateTime, -num36 / 2.0);
			double num37 = base.decel_DV_mps / fleetAcceleration_mps2;
			this.arrivalTime = new TIDateTime(tidateTime2, num37 / 2.0);
			this.transitDuration_s = this.arrivalTime.DifferenceInSeconds(this.launchTime);
			if (!(this.launchTime < startTime))
			{
				double num38 = (flag ? num2 : num3);
				if (!flag2)
				{
					num38 += 3.141592653589793;
				}
				this.outgoingOrbit = new OrbitalElementsState(origin.longAscendingNode_Rad, num38 + origin.argPeriapsis_Rad, origin.inclination_Rad, num20, this.eccentricity(num19, origin.semiMajorAxis_m), flag2 ? 0.0 : 3.141592653589793, tidateTime.ExportTime());
				double num39 = (flag ? num4 : num5);
				if (flag3)
				{
					num39 += 3.141592653589793;
				}
				this.incomingOrbit = new OrbitalElementsState(destination.longAscendingNode_Rad, num39 + destination.argPeriapsis_Rad, destination.inclination_Rad, num21, this.eccentricity(num19, destination.semiMajorAxis_m), flag3 ? 0.0 : 3.141592653589793, tidateTime2.ExportTime());
				return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
			}
			if (iterating)
			{
				return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, this.launchTime.DifferenceInSeconds(startTime), num36);
			}
			TIDateTime tidateTime3 = new TIDateTime(tidateTime, 1.0);
			return this.Solve(tidateTime3, durationInDestinationOrbits, origin, destination, commonBarycenter, fleetAcceleration_mps2, anyMeanAnomalyAtArrival, startMeanAnomalyChangePerSecond_radPerSec, endMeanAnomalyChangePerSecond_radPerSec, true);
		}

		// Token: 0x06003DBE RID: 15806 RVA: 0x0018499F File Offset: 0x00182B9F
		private double eccentricity(double apoapsis, double periapsis)
		{
			return (apoapsis - periapsis) / (apoapsis + periapsis);
		}

		// Token: 0x040026A5 RID: 9893
		public TIDateTime intermediateBurnTime;

		// Token: 0x040026A6 RID: 9894
		public double intermediate_burn_DV;

		// Token: 0x040026A7 RID: 9895
		public OrbitalElementsState outgoingOrbit;

		// Token: 0x040026A8 RID: 9896
		public OrbitalElementsState incomingOrbit;
	}
}
