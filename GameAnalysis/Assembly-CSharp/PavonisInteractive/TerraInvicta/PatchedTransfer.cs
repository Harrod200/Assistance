using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000796 RID: 1942
	public class PatchedTransfer : TrajectorySolver
	{
		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x06003E4B RID: 15947 RVA: 0x00191043 File Offset: 0x0018F243
		// (set) Token: 0x06003E4C RID: 15948 RVA: 0x0019104B File Offset: 0x0018F24B
		public List<IPatchedTransferSegment> transferSegments { get; private set; }

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x06003E4D RID: 15949 RVA: 0x00191054 File Offset: 0x0018F254
		// (set) Token: 0x06003E4E RID: 15950 RVA: 0x00191093 File Offset: 0x0018F293
		public override TIDateTime launchTime
		{
			get
			{
				if (this.transferSegments.Count == 0)
				{
					return new TIDateTime();
				}
				return this.transferSegments.Min<IPatchedTransferSegment, TIDateTime>((IPatchedTransferSegment x) => x.startTime);
			}
			protected set
			{
			}
		}

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x06003E4F RID: 15951 RVA: 0x00191095 File Offset: 0x0018F295
		// (set) Token: 0x06003E50 RID: 15952 RVA: 0x001910D4 File Offset: 0x0018F2D4
		public override TIDateTime arrivalTime
		{
			get
			{
				if (this.transferSegments.Count == 0)
				{
					return new TIDateTime();
				}
				return this.transferSegments.Max<IPatchedTransferSegment, TIDateTime>((IPatchedTransferSegment x) => x.endTime);
			}
			protected set
			{
			}
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06003E51 RID: 15953 RVA: 0x001910D6 File Offset: 0x0018F2D6
		// (set) Token: 0x06003E52 RID: 15954 RVA: 0x001910E9 File Offset: 0x0018F2E9
		public override double transitDuration_s
		{
			get
			{
				return this.arrivalTime.DifferenceInSeconds(this.launchTime);
			}
			protected set
			{
			}
		}

		// Token: 0x06003E53 RID: 15955 RVA: 0x001910EC File Offset: 0x0018F2EC
		public TransferResult Solve(TIDateTime launchTime, TIDateTime arrivalTime, ITransferTarget originValue, OrbitalElementsState destinationOrbitElements, TINaturalSpaceObjectState destinationBarycenter, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, bool anyMeanAnomalyAtArrival, PatchedTransfer.InternalTransferType internalTransferType, TIDateTime earliestArrivalTimeForMicrothrustOnly = null)
		{
			this.transferSegments = new List<IPatchedTransferSegment>();
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			MasterTransferPlanner.GetOriginOrbitalElementsState(originValue, launchTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			if (tinaturalSpaceObjectState == destinationBarycenter)
			{
				return this.SolveSingleBarycenter(launchTime, arrivalTime, originValue, destinationOrbitElements, fleetAcceleration_mps2, anyMeanAnomalyAtArrival, internalTransferType, earliestArrivalTimeForMicrothrustOnly);
			}
			return this.SolveMultiBarycenter(launchTime, arrivalTime, originValue, destinationOrbitElements, destinationBarycenter, commonBarycenter, fleetAcceleration_mps2, anyMeanAnomalyAtArrival, internalTransferType);
		}

		// Token: 0x06003E54 RID: 15956 RVA: 0x00191144 File Offset: 0x0018F344
		private TransferResult SolveSingleBarycenter(TIDateTime launchTime, TIDateTime targetArrivalTime, ITransferTarget originValue, OrbitalElementsState destinationOrbitElements, double fleetAcceleration_mps2, bool anyMeanAnomalyAtArrival, PatchedTransfer.InternalTransferType internalTransferType, TIDateTime earliestArrivalTimeForMicrothrustOnly = null)
		{
			if (targetArrivalTime <= launchTime)
			{
				return new TransferResult(TransferResult.Outcome.Fail_ArrivalBeforeLaunch, launchTime.DifferenceInSeconds(targetArrivalTime), 0.0);
			}
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			originValue.getOrbitalElementsState(launchTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			bool flag2 = orbitalElementsState.eccentricity >= 1.0;
			bool flag3 = destinationOrbitElements.eccentricity >= 1.0;
			TINaturalSpaceObjectState tinaturalSpaceObjectState2 = tinaturalSpaceObjectState;
			double mu = tinaturalSpaceObjectState2.mu;
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, mu, tinaturalSpaceObjectState2.sphereOfInfluence_m);
			double num = Mathd.Sqrt(mu / orbitalElementsState.semiMajorAxis_m);
			double num2 = Mathd.Sqrt(mu / destinationOrbitElements.semiMajorAxis_m);
			double num3 = orbitalElementsState.semiMajorAxis_m;
			if (flag2)
			{
				CartesianState cartesianState = orbitalElementsState.ToCartesianStateAtTime(launchTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
				num3 = cartesianState.position.magnitude;
			}
			double num4 = destinationOrbitElements.semiMajorAxis_m;
			if (flag3)
			{
				num4 = destinationOrbitElements.ToCartesianStateAtTime(targetArrivalTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg).position.magnitude;
			}
			if (num3 < microthrustSphere.Radius_m)
			{
				TISpaceFleetState tispaceFleetState = originValue as TISpaceFleetState;
				TIDateTime tidateTime;
				if (tispaceFleetState == null)
				{
					tidateTime = null;
				}
				else
				{
					Trajectory trajectory = tispaceFleetState.trajectory;
					tidateTime = ((trajectory != null) ? trajectory.arrivalTime : null);
				}
				if (!(tidateTime > launchTime))
				{
					if (num4 < microthrustSphere.Radius_m)
					{
						if ((destinationOrbitElements.inclination_Rad == 0.0 && orbitalElementsState.inclination_Rad == 0.0) || (Mathd.Approximately(destinationOrbitElements.inclination_Rad, orbitalElementsState.inclination_Rad) && Mathd.Approximately(destinationOrbitElements.longAscendingNode_Rad, orbitalElementsState.longAscendingNode_Rad) && Mathd.Approximately(destinationOrbitElements.eccentricity, orbitalElementsState.eccentricity)))
						{
							return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
						}
						if (orbitalElementsState.longAscendingNode_Rad == destinationOrbitElements.longAscendingNode_Rad && orbitalElementsState.argPeriapsis_Rad == destinationOrbitElements.argPeriapsis_Rad)
						{
							return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
						}
						if (flag2 || flag3)
						{
							double num5 = destinationOrbitElements.eccentricity;
							if (flag2)
							{
								num5 = orbitalElementsState.eccentricity;
							}
							return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, num5, 0.0);
						}
						TISpaceFleetState tispaceFleetState2 = originValue as TISpaceFleetState;
						if (tispaceFleetState2 != null && tispaceFleetState2.trajectory != null && tispaceFleetState2.trajectory.launchTime < launchTime && tispaceFleetState2.trajectory.destination.barycenter == tinaturalSpaceObjectState)
						{
							launchTime = new TIDateTime(tispaceFleetState2.trajectory.arrivalTime, 1.0);
							originValue.getOrbitalElementsState(launchTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
						}
						else
						{
							launchTime = TITimeState.Now();
						}
						double num6 = Mathd.Abs(microthrustSphere.GetDuration_s(num) - microthrustSphere.GetDuration_s(num2));
						if (tinaturalSpaceObjectState != tinaturalSpaceObjectState2)
						{
							Debug.LogError("PatchedTransfer: microthrust only inclination change: barycenters don't agree: common = " + tinaturalSpaceObjectState2.displayName + ", origin = " + tinaturalSpaceObjectState.displayName);
						}
						double num7 = Mathd.Min(num, num2);
						double num8 = (orbitalElementsState.normalVector - destinationOrbitElements.normalVector).magnitude * num7;
						CartesianState cartesianState = orbitalElementsState.ToCartesianStateAtTime(orbitalElementsState.epoch, tinaturalSpaceObjectState2.mass_kg);
						Vector3d normalized = cartesianState.position.normalized;
						cartesianState = destinationOrbitElements.ToCartesianStateAtTime(destinationOrbitElements.epoch, tinaturalSpaceObjectState2.mass_kg);
						Vector3d normalized2 = cartesianState.position.normalized;
						Vector3d vector3d = orbitalElementsState.eccentricity * normalized;
						Vector3d vector3d2 = destinationOrbitElements.eccentricity * normalized2;
						double num9 = (vector3d - vector3d2).magnitude * num7;
						double num10 = (num8 + num9) / fleetAcceleration_mps2;
						double num11 = fleetAcceleration_mps2 * num6 / (num6 + num10);
						MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(num11, tinaturalSpaceObjectState2.mu, tinaturalSpaceObjectState2.sphereOfInfluence_m);
						double num12 = Mathd.Abs(microthrustSphere2.GetAnomalyDelta_Rad(num2) - microthrustSphere2.GetAnomalyDelta_Rad(num));
						double num13 = Mathd.Abs(microthrustSphere2.GetDuration_s(num2) - microthrustSphere2.GetDuration_s(num));
						double num14 = orbitalElementsState.MeanAnomalyAtTime_Rad(launchTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg) + orbitalElementsState.longAscendingNode_Rad + orbitalElementsState.argPeriapsis_Rad - destinationOrbitElements.longAscendingNode_Rad - destinationOrbitElements.argPeriapsis_Rad + num12;
						MicrothrustTransferLERPvalues microthrustTransferLERPvalues = new MicrothrustTransferLERPvalues(orbitalElementsState.semiMajorAxis_m, orbitalElementsState.MeanAnomalyAtTime_Rad(launchTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg), orbitalElementsState.eccentricity, orbitalElementsState.longAscendingNode_Rad, orbitalElementsState.inclination_Rad, orbitalElementsState.argPeriapsis_Rad, 0.0, 0.0, 0.0);
						MicrothrustTransferLERPvalues microthrustTransferLERPvalues2 = new MicrothrustTransferLERPvalues(destinationOrbitElements.semiMajorAxis_m, num14, destinationOrbitElements.eccentricity, destinationOrbitElements.longAscendingNode_Rad, destinationOrbitElements.inclination_Rad, destinationOrbitElements.argPeriapsis_Rad, 0.0, 0.0, 0.0);
						if (microthrustTransferLERPvalues2.inclination_Rad == 0.0)
						{
							microthrustTransferLERPvalues2.ascendingNode_Rad = microthrustTransferLERPvalues.ascendingNode_Rad;
						}
						if (microthrustTransferLERPvalues.inclination_Rad == 0.0)
						{
							microthrustTransferLERPvalues.ascendingNode_Rad = microthrustTransferLERPvalues2.ascendingNode_Rad;
						}
						if (microthrustTransferLERPvalues2.eccentricity == 0.0)
						{
							microthrustTransferLERPvalues2.argPeriapsis_Rad = microthrustTransferLERPvalues.argPeriapsis_Rad + microthrustTransferLERPvalues.ascendingNode_Rad - microthrustTransferLERPvalues2.ascendingNode_Rad;
						}
						if (microthrustTransferLERPvalues.eccentricity == 0.0)
						{
							microthrustTransferLERPvalues.argPeriapsis_Rad = microthrustTransferLERPvalues2.argPeriapsis_Rad + microthrustTransferLERPvalues2.ascendingNode_Rad - microthrustTransferLERPvalues.ascendingNode_Rad;
						}
						microthrustTransferLERPvalues2.meanAnomaly_Rad = microthrustTransferLERPvalues.meanAnomaly_Rad + num12 + microthrustTransferLERPvalues.ascendingNode_Rad + microthrustTransferLERPvalues.argPeriapsis_Rad - microthrustTransferLERPvalues2.ascendingNode_Rad - microthrustTransferLERPvalues2.argPeriapsis_Rad;
						MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP = new MicrothrustTransferSegmentLERP
						{
							start = microthrustTransferLERPvalues,
							end = microthrustTransferLERPvalues2,
							startTime = new TIDateTime(launchTime),
							endTime = new TIDateTime(launchTime, num13),
							barycenter = tinaturalSpaceObjectState2,
							effectiveFleetAcceleration_mps2 = num11,
							trueFleetAcceleration_mps2 = fleetAcceleration_mps2
						};
						if (!anyMeanAnomalyAtArrival)
						{
							if (microthrustTransferSegmentLERP.endTime < targetArrivalTime)
							{
								launchTime = new TIDateTime(targetArrivalTime, -num13);
								microthrustTransferSegmentLERP.startTime = launchTime;
								microthrustTransferSegmentLERP.endTime = new TIDateTime(targetArrivalTime);
							}
							double num15 = Mathd.ClampRadiansTwoPI(orbitalElementsState.MeanAnomalyAtTime_Rad(orbitalElementsState.epoch.AddSeconds(1.0), tinaturalSpaceObjectState2.mass_kg) - orbitalElementsState.meanAnomalyAtEpoch_Rad);
							double num16 = Mathd.ClampRadiansPI(Mathd.ClampRadiansTwoPI(destinationOrbitElements.MeanAnomalyAtTime_Rad(destinationOrbitElements.epoch.AddSeconds(1.0), tinaturalSpaceObjectState2.mass_kg) - destinationOrbitElements.meanAnomalyAtEpoch_Rad) - num15);
							double num17 = orbitalElementsState.MeanAnomalyAtTime_Rad(launchTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
							double num18 = destinationOrbitElements.MeanAnomalyAtTime_Rad(microthrustTransferSegmentLERP.endTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
							double num19 = num17 + num12;
							double num20 = Mathd.ClampRadiansTwoPI(num18 - num19);
							if (num16 < 0.0)
							{
								num20 -= 6.283185307179586;
							}
							double num21 = num20 / num16;
							if (num21 > 78892310.0 || num21 < -78892310.0)
							{
								return new TransferResult(TransferResult.Outcome.Fail_ExceedsMaxDuration, 78892310.0, 0.0);
							}
							microthrustTransferSegmentLERP.startTime.AddSeconds(num21);
							microthrustTransferSegmentLERP.endTime.AddSeconds(num21);
							if (microthrustTransferSegmentLERP.endTime < earliestArrivalTimeForMicrothrustOnly)
							{
								double num22 = 6.283185307179586 / num16;
								double num23 = Mathd.Ceil(earliestArrivalTimeForMicrothrustOnly.DifferenceInSeconds(microthrustTransferSegmentLERP.endTime) / num22) * num22;
								microthrustTransferSegmentLERP.startTime.AddSeconds(num23);
								microthrustTransferSegmentLERP.endTime.AddSeconds(num23);
							}
							microthrustTransferSegmentLERP.start.meanAnomaly_Rad = orbitalElementsState.MeanAnomalyAtTime_Rad(microthrustTransferSegmentLERP.startTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
							microthrustTransferSegmentLERP.end.meanAnomaly_Rad = destinationOrbitElements.MeanAnomalyAtTime_Rad(microthrustTransferSegmentLERP.endTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
						}
						this.transferSegments.Add(microthrustTransferSegmentLERP);
						if (num3 < num4)
						{
							base.boost_DV_mps = microthrustTransferSegmentLERP.DV_mps;
						}
						else
						{
							base.decel_DV_mps = microthrustTransferSegmentLERP.DV_mps;
						}
						base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
						goto IL_0FAD;
					}
					else
					{
						if (orbitalElementsState.eccentricity >= 1.0)
						{
							return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, orbitalElementsState.eccentricity, 0.0);
						}
						TIDateTime tidateTime2 = new TIDateTime(launchTime);
						tidateTime2.AddSeconds(microthrustSphere.GetDuration_s(num));
						if (tidateTime2 > targetArrivalTime)
						{
							return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, microthrustSphere.GetDuration_s(num), targetArrivalTime.DifferenceInSeconds(launchTime));
						}
						MicrothrustTransferSegment microthrustTransferSegment = new MicrothrustTransferSegment
						{
							startTime = new TIDateTime(launchTime),
							endTime = tidateTime2,
							barycenter = tinaturalSpaceObjectState2,
							startRadius_m = orbitalElementsState.semiMajorAxis_m,
							endRadius_m = microthrustSphere.Radius_m,
							startAnomaly_Rad = orbitalElementsState.MeanAnomalyAtTime_Rad(launchTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg),
							anomalyDelta_Rad = microthrustSphere.GetAnomalyDelta_Rad(num),
							eccentricity = orbitalElementsState.eccentricity,
							ascendingNode_rad = orbitalElementsState.longAscendingNode_Rad,
							inclination_rad = orbitalElementsState.inclination_Rad,
							argP_rad = orbitalElementsState.argPeriapsis_Rad
						};
						OrbitalElementsState orbitalElementsState2 = new OrbitalElementsState(orbitalElementsState, microthrustTransferSegment.endAnomaly, tidateTime2);
						orbitalElementsState2.semiMajorAxis_m = microthrustTransferSegment.endRadius_m;
						CartesianState cartesianState2 = orbitalElementsState2.ToCartesianStateAtTime(tidateTime2.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
						if (internalTransferType != PatchedTransfer.InternalTransferType.Lambert)
						{
							if (internalTransferType != PatchedTransfer.InternalTransferType.Torch)
							{
								Debug.LogError("PatchedTransfer: did not recognize internal transfer type: " + internalTransferType.ToString());
								return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
							}
							CartesianState cartesianState3 = cartesianState2 + tinaturalSpaceObjectState2.ToGlobalCartesianStateAtTime(tidateTime2);
							CartesianState cartesianState4 = destinationOrbitElements.ToCartesianStateAtTime(targetArrivalTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
							cartesianState4 *= Quaterniond.Inverse(tinaturalSpaceObjectState2.SpatialRotation);
							cartesianState4 += tinaturalSpaceObjectState2.ToGlobalCartesianStateAtTime(targetArrivalTime);
							TorchTransfer torchTransfer = new TorchTransfer();
							double num24 = targetArrivalTime.DifferenceInSeconds(tidateTime2);
							if (num24 <= 0.0)
							{
								return new TransferResult(TransferResult.Outcome.Fail_ArrivalBeforeLaunch, num24, 0.0);
							}
							TransferResult transferResult = torchTransfer.Solve(tidateTime2, num24, fleetAcceleration_mps2, cartesianState3, cartesianState4, tinaturalSpaceObjectState2, double.PositiveInfinity, out flag, false);
							if (transferResult.Result != TransferResult.Outcome.Success)
							{
								return transferResult;
							}
							TorchTransferSegment torchTransferSegment = new TorchTransferSegment
							{
								torch = torchTransfer,
								barycenter = tinaturalSpaceObjectState2,
								initialGravwellDuration_s = 0.0,
								finalGravwellDuration_s = 0.0,
								fleetAcceleration_mps = fleetAcceleration_mps2,
								initialGlobalVelocity_mps = cartesianState3.velocity,
								finalGlobalVelocity_mps = cartesianState4.velocity
							};
							this.transferSegments.Add(microthrustTransferSegment);
							this.transferSegments.Add(torchTransferSegment);
							base.boost_DV_mps = microthrustTransferSegment.DV_mps + torchTransfer.boost_DV_mps + torchTransferSegment.initialGravwellDuration_s * fleetAcceleration_mps2;
							base.decel_DV_mps = torchTransfer.decel_DV_mps + torchTransferSegment.finalGravwellDuration_s * fleetAcceleration_mps2;
							base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
							goto IL_0FAD;
						}
						else
						{
							CartesianState cartesianState5 = destinationOrbitElements.ToCartesianStateAtTime(targetArrivalTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
							TwoBurnLambertTransfer twoBurnLambertTransfer = new TwoBurnLambertTransfer();
							TransferResult transferResult2 = twoBurnLambertTransfer.SolveCartesian(tidateTime2, targetArrivalTime, targetArrivalTime.DifferenceInSeconds(tidateTime2), cartesianState2, cartesianState5, tinaturalSpaceObjectState2, fleetAcceleration_mps2);
							if (transferResult2.Result != TransferResult.Outcome.Success)
							{
								return transferResult2;
							}
							if (twoBurnLambertTransfer.launchTime < TITimeState.Now())
							{
								double num25 = TITimeState.Now().DifferenceInSeconds(twoBurnLambertTransfer.launchTime);
								double num26 = twoBurnLambertTransfer.boost_DV_mps / fleetAcceleration_mps2;
								return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, num25, num26);
							}
							ImpulseTransferSegment impulseTransferSegment = new ImpulseTransferSegment
							{
								lambert = twoBurnLambertTransfer,
								barycenter = tinaturalSpaceObjectState2
							};
							this.transferSegments.Add(microthrustTransferSegment);
							this.transferSegments.Add(impulseTransferSegment);
							base.boost_DV_mps = microthrustTransferSegment.DV_mps + twoBurnLambertTransfer.boost_DV_mps;
							base.decel_DV_mps = twoBurnLambertTransfer.decel_DV_mps;
							base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
							goto IL_0FAD;
						}
					}
				}
			}
			TIDateTime tidateTime3 = new TIDateTime(targetArrivalTime);
			tidateTime3.AddSeconds(-microthrustSphere.GetDuration_s(num2));
			if (tidateTime3 < launchTime)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, microthrustSphere.GetDuration_s(num2), targetArrivalTime.DifferenceInSeconds(launchTime));
			}
			double num27 = destinationOrbitElements.MeanAnomalyAtTime_Rad(targetArrivalTime.ExportTime(), tinaturalSpaceObjectState2.mass_kg) - microthrustSphere.GetAnomalyDelta_Rad(num2);
			if (num27 < 0.0)
			{
				num27 += 6.283185307179586;
			}
			MicrothrustTransferSegment microthrustTransferSegment2 = new MicrothrustTransferSegment
			{
				startTime = tidateTime3,
				endTime = new TIDateTime(targetArrivalTime),
				barycenter = tinaturalSpaceObjectState2,
				startRadius_m = microthrustSphere.Radius_m,
				endRadius_m = destinationOrbitElements.semiMajorAxis_m,
				startAnomaly_Rad = num27,
				anomalyDelta_Rad = microthrustSphere.GetAnomalyDelta_Rad(num2),
				eccentricity = destinationOrbitElements.eccentricity,
				ascendingNode_rad = destinationOrbitElements.longAscendingNode_Rad,
				inclination_rad = destinationOrbitElements.inclination_Rad,
				argP_rad = destinationOrbitElements.argPeriapsis_Rad
			};
			bool flag4 = microthrustTransferSegment2.startTime != microthrustTransferSegment2.endTime;
			if (flag3 && flag4)
			{
				return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, destinationOrbitElements.eccentricity, 0.0);
			}
			OrbitalElementsState orbitalElementsState3 = new OrbitalElementsState(destinationOrbitElements);
			if (flag4)
			{
				orbitalElementsState3.semiMajorAxis_m = microthrustTransferSegment2.startRadius_m;
				orbitalElementsState3.meanAnomalyAtEpoch_Rad = microthrustTransferSegment2.startAnomaly_Rad;
				orbitalElementsState3.epoch = tidateTime3.ExportTime();
			}
			CartesianState cartesianState6 = orbitalElementsState3.ToCartesianStateAtTime(tidateTime3.ExportTime(), tinaturalSpaceObjectState2.mass_kg);
			CartesianState valueOrDefault = originValue.tryToGetGlobalCartesianState(launchTime).GetValueOrDefault();
			if (internalTransferType != PatchedTransfer.InternalTransferType.Lambert)
			{
				if (internalTransferType != PatchedTransfer.InternalTransferType.Torch)
				{
					Debug.LogError("PatchedTransfer: did not recognize internal transfer type: " + internalTransferType.ToString());
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				CartesianState cartesianState7 = cartesianState6.ToGlobal(tinaturalSpaceObjectState2, tidateTime3);
				TorchTransfer torchTransfer2 = new TorchTransfer();
				TransferResult transferResult3 = torchTransfer2.Solve(launchTime, tidateTime3.DifferenceInSeconds(launchTime), fleetAcceleration_mps2, valueOrDefault, cartesianState7, tinaturalSpaceObjectState2, double.PositiveInfinity, out flag, false);
				if (transferResult3.Result != TransferResult.Outcome.Success)
				{
					return transferResult3;
				}
				TorchTransferSegment torchTransferSegment2 = new TorchTransferSegment
				{
					torch = torchTransfer2,
					barycenter = tinaturalSpaceObjectState2,
					initialGravwellDuration_s = 0.0,
					finalGravwellDuration_s = 0.0,
					fleetAcceleration_mps = fleetAcceleration_mps2,
					initialGlobalVelocity_mps = valueOrDefault.velocity,
					finalGlobalVelocity_mps = cartesianState7.velocity
				};
				this.transferSegments.Add(torchTransferSegment2);
				if (flag4)
				{
					this.transferSegments.Add(microthrustTransferSegment2);
				}
				base.boost_DV_mps = torchTransfer2.boost_DV_mps + torchTransferSegment2.initialGravwellDuration_s * fleetAcceleration_mps2;
				base.decel_DV_mps = torchTransfer2.decel_DV_mps + torchTransferSegment2.finalGravwellDuration_s * fleetAcceleration_mps2 + (flag4 ? microthrustTransferSegment2.DV_mps : 0.0);
				base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			}
			else
			{
				CartesianState cartesianState8 = valueOrDefault.ToLocal(tinaturalSpaceObjectState2, launchTime);
				TwoBurnLambertTransfer twoBurnLambertTransfer2 = new TwoBurnLambertTransfer();
				TransferResult transferResult4 = twoBurnLambertTransfer2.SolveCartesian(launchTime, tidateTime3, tidateTime3.DifferenceInSeconds(launchTime), cartesianState8, cartesianState6, tinaturalSpaceObjectState2, fleetAcceleration_mps2);
				if (transferResult4.Result != TransferResult.Outcome.Success)
				{
					return transferResult4;
				}
				if (twoBurnLambertTransfer2.launchTime < TITimeState.Now())
				{
					double num28 = TITimeState.Now().DifferenceInSeconds(twoBurnLambertTransfer2.launchTime);
					double num29 = twoBurnLambertTransfer2.boost_DV_mps / fleetAcceleration_mps2;
					return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, num28, num29);
				}
				ImpulseTransferSegment impulseTransferSegment2 = new ImpulseTransferSegment
				{
					lambert = twoBurnLambertTransfer2,
					barycenter = tinaturalSpaceObjectState2
				};
				this.transferSegments.Add(impulseTransferSegment2);
				if (flag4)
				{
					this.transferSegments.Add(microthrustTransferSegment2);
				}
				base.boost_DV_mps = twoBurnLambertTransfer2.boost_DV_mps;
				base.decel_DV_mps = twoBurnLambertTransfer2.decel_DV_mps + (flag4 ? microthrustTransferSegment2.DV_mps : 0.0);
				base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			}
			IL_0FAD:
			return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
		}

		// Token: 0x06003E55 RID: 15957 RVA: 0x00192118 File Offset: 0x00190318
		private TransferResult SolveMultiBarycenter(TIDateTime launchTime, TIDateTime arrivalTime, ITransferTarget originValue, OrbitalElementsState destinationOrbitElements, TINaturalSpaceObjectState destinationBarycenter, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, bool anyMeanAnomalyAtArrival, PatchedTransfer.InternalTransferType internalTransferType)
		{
			if (arrivalTime <= launchTime)
			{
				return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, launchTime.DifferenceInSeconds(arrivalTime), 0.0);
			}
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			MasterTransferPlanner.GetOriginOrbitalElementsState(originValue, launchTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			List<IPatchedTransferSegment> list = new List<IPatchedTransferSegment>();
			List<IPatchedTransferSegment> list2 = new List<IPatchedTransferSegment>();
			Vector3d vector3d = default(Vector3d);
			Vector3d vector3d2 = default(Vector3d);
			TIDateTime tidateTime = new TIDateTime(launchTime);
			TIDateTime tidateTime2 = new TIDateTime(arrivalTime);
			double num = orbitalElementsState.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg);
			double num2 = destinationOrbitElements.OrbitalPeriod(destinationBarycenter.mass_kg);
			CartesianState cartesianState = orbitalElementsState.ToCartesianStateAtTime(launchTime.ExportTime(), tinaturalSpaceObjectState.mass_kg);
			Vector3d vector3d3;
			double num3;
			if (tinaturalSpaceObjectState == commonBarycenter)
			{
				vector3d3 = new Vector3d(0f, 0f, 0f);
				num3 = 0.0;
			}
			else
			{
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState.mu, tinaturalSpaceObjectState.sphereOfInfluence_m);
				if (orbitalElementsState.semiMajorAxis_m >= microthrustSphere.Radius_m || orbitalElementsState.semiMajorAxis_m <= 0.0)
				{
					vector3d3 = cartesianState.velocity;
					num3 = tinaturalSpaceObjectState.localEscapeVelocity_mps(cartesianState.position.magnitude);
				}
				else
				{
					double num4 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / orbitalElementsState.semiMajorAxis_m);
					tidateTime.AddSeconds(microthrustSphere.GetDuration_s(num4));
					MicrothrustTransferSegment microthrustTransferSegment = new MicrothrustTransferSegment
					{
						startTime = new TIDateTime(launchTime),
						endTime = new TIDateTime(tidateTime),
						barycenter = tinaturalSpaceObjectState,
						startRadius_m = orbitalElementsState.semiMajorAxis_m,
						endRadius_m = microthrustSphere.Radius_m,
						startAnomaly_Rad = orbitalElementsState.MeanAnomalyAtTime_Rad(launchTime.ExportTime(), tinaturalSpaceObjectState.mass_kg),
						anomalyDelta_Rad = microthrustSphere.GetAnomalyDelta_Rad(num4),
						eccentricity = orbitalElementsState.eccentricity,
						ascendingNode_rad = orbitalElementsState.longAscendingNode_Rad,
						inclination_rad = orbitalElementsState.inclination_Rad,
						argP_rad = orbitalElementsState.argPeriapsis_Rad
					};
					list.Add(microthrustTransferSegment);
					OrbitalElementsState orbitalElementsState2 = new OrbitalElementsState(orbitalElementsState);
					orbitalElementsState2.meanAnomalyAtEpoch_Rad = microthrustTransferSegment.endAnomaly;
					orbitalElementsState2.epoch = tidateTime.ExportTime();
					orbitalElementsState2.semiMajorAxis_m = microthrustTransferSegment.endRadius_m;
					cartesianState = orbitalElementsState2.ToCartesianStateAtTime(tidateTime.ExportTime(), tinaturalSpaceObjectState.mass_kg);
					num = orbitalElementsState2.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg);
					if (microthrustSphere.IsLimitedBySphereOfInfluence)
					{
						vector3d3 = new Vector3d(0f, 0f, 0f);
						num3 = 0.0;
					}
					else
					{
						vector3d3 = cartesianState.velocity;
						num3 = tinaturalSpaceObjectState.localEscapeVelocity_mps(cartesianState.position.magnitude);
					}
				}
				if (vector3d3.magnitude > num3)
				{
					if (orbitalElementsState.eccentricity >= 1.0)
					{
						Vector3d vector3d4 = cartesianState.velocity.normalized * tinaturalSpaceObjectState.localEscapeVelocity_mps(cartesianState.position.magnitude);
						vector3d = cartesianState.velocity - vector3d4;
						if (cartesianState.velocity.sqrMagnitude < vector3d4.sqrMagnitude)
						{
							vector3d = new Vector3d(0f, 0f, 0f);
						}
					}
					vector3d3 = new Vector3d(0f, 0f, 0f);
					num3 = 0.0;
				}
				cartesianState.velocity = new Vector3d(vector3d);
				PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState, tinaturalSpaceObjectState, tidateTime);
				if (tinaturalSpaceObjectState.barycenter != commonBarycenter)
				{
					TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
					MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, barycenter.mu, barycenter.sphereOfInfluence_m);
					TIDateTime tidateTime3 = new TIDateTime(tidateTime);
					OrbitalElementsState orbitalElementsState3 = cartesianState.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime.ExportTime()));
					if (orbitalElementsState3.semiMajorAxis_m >= microthrustSphere2.Radius_m || orbitalElementsState3.semiMajorAxis_m <= 0.0)
					{
						vector3d3 += cartesianState.velocity;
						num3 += barycenter.localEscapeVelocity_mps(cartesianState.position.magnitude);
					}
					else
					{
						num3 -= vector3d3.magnitude;
						vector3d3 = new Vector3d(0f, 0f, 0f);
						double num5 = Mathd.Sqrt(barycenter.mu / orbitalElementsState3.semiMajorAxis_m);
						tidateTime.AddSeconds(microthrustSphere2.GetDuration_s(num5));
						MicrothrustTransferSegment microthrustTransferSegment2 = new MicrothrustTransferSegment
						{
							startTime = tidateTime3,
							endTime = new TIDateTime(tidateTime),
							barycenter = barycenter,
							startRadius_m = tinaturalSpaceObjectState.semiMajorAxis_m,
							endRadius_m = microthrustSphere2.Radius_m,
							startAnomaly_Rad = orbitalElementsState3.MeanAnomalyAtTime_Rad(tidateTime3.ExportTime(), barycenter.mass_kg),
							anomalyDelta_Rad = microthrustSphere2.GetAnomalyDelta_Rad(num5),
							eccentricity = orbitalElementsState3.eccentricity,
							ascendingNode_rad = orbitalElementsState3.longAscendingNode_Rad,
							inclination_rad = orbitalElementsState3.inclination_Rad,
							argP_rad = orbitalElementsState3.argPeriapsis_Rad
						};
						list.Add(microthrustTransferSegment2);
						OrbitalElementsState orbitalElementsState4 = new OrbitalElementsState(orbitalElementsState3);
						orbitalElementsState4.semiMajorAxis_m = microthrustTransferSegment2.endRadius_m;
						orbitalElementsState4.meanAnomalyAtEpoch_Rad = microthrustTransferSegment2.endAnomaly;
						orbitalElementsState4.epoch = tidateTime.ExportTime();
						cartesianState = orbitalElementsState4.ToCartesianStateAtTime(tidateTime.ExportTime(), barycenter.mass_kg);
						num = orbitalElementsState4.OrbitalPeriod(barycenter.mass_kg);
						if (!microthrustSphere2.IsLimitedBySphereOfInfluence)
						{
							vector3d3 += cartesianState.velocity;
							num3 += barycenter.localEscapeVelocity_mps(cartesianState.position.magnitude);
						}
					}
					if (orbitalElementsState3.eccentricity > 1.0)
					{
						Vector3d vector3d5 = cartesianState.velocity.normalized * barycenter.localEscapeVelocity_mps(cartesianState.position.magnitude);
						vector3d = cartesianState.velocity - vector3d5;
						if (cartesianState.velocity.sqrMagnitude < vector3d5.sqrMagnitude)
						{
							vector3d = new Vector3d(0f, 0f, 0f);
						}
					}
					else
					{
						vector3d = new Vector3d(0f, 0f, 0f);
					}
					cartesianState.velocity = vector3d;
					PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState, barycenter, tidateTime);
					if (vector3d3.magnitude > num3)
					{
						num3 = 0.0;
						vector3d3 = new Vector3d(0f, 0f, 0f);
					}
				}
			}
			CartesianState cartesianState2 = destinationOrbitElements.ToCartesianStateAtTime(tidateTime2.ExportTime(), destinationBarycenter.mass_kg);
			Vector3d vector3d6;
			double num6;
			if (destinationBarycenter == commonBarycenter)
			{
				vector3d6 = new Vector3d(0f, 0f, 0f);
				num6 = 0.0;
			}
			else
			{
				MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(fleetAcceleration_mps2, destinationBarycenter.mu, destinationBarycenter.sphereOfInfluence_m);
				double num7 = Mathd.Sqrt(destinationBarycenter.mu / destinationOrbitElements.semiMajorAxis_m);
				if (destinationOrbitElements.semiMajorAxis_m >= microthrustSphere3.Radius_m || destinationOrbitElements.semiMajorAxis_m <= 0.0)
				{
					vector3d6 = cartesianState2.velocity;
					num6 = destinationBarycenter.localEscapeVelocity_mps(cartesianState2.position.magnitude);
				}
				else
				{
					if (destinationOrbitElements.eccentricity >= 1.0)
					{
						return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, destinationOrbitElements.eccentricity, 0.0);
					}
					tidateTime2.AddSeconds(-microthrustSphere3.GetDuration_s(num7));
					double anomalyDelta_Rad = microthrustSphere3.GetAnomalyDelta_Rad(num7);
					double num8 = destinationOrbitElements.MeanAnomalyAtTime_Rad(arrivalTime.ExportTime(), destinationBarycenter.mass_kg) - anomalyDelta_Rad;
					list2.Add(new MicrothrustTransferSegment
					{
						startTime = tidateTime2,
						endTime = arrivalTime,
						barycenter = destinationBarycenter,
						startRadius_m = microthrustSphere3.Radius_m,
						endRadius_m = destinationOrbitElements.semiMajorAxis_m,
						startAnomaly_Rad = num8,
						anomalyDelta_Rad = anomalyDelta_Rad,
						eccentricity = destinationOrbitElements.eccentricity,
						ascendingNode_rad = destinationOrbitElements.longAscendingNode_Rad,
						inclination_rad = destinationOrbitElements.inclination_Rad,
						argP_rad = destinationOrbitElements.argPeriapsis_Rad
					});
					OrbitalElementsState orbitalElementsState5 = new OrbitalElementsState(destinationOrbitElements);
					orbitalElementsState5.semiMajorAxis_m = microthrustSphere3.Radius_m;
					orbitalElementsState5.meanAnomalyAtEpoch_Rad = num8;
					orbitalElementsState5.epoch = tidateTime2.ExportTime();
					cartesianState2 = orbitalElementsState5.ToCartesianStateAtTime(tidateTime2.ExportTime(), destinationBarycenter.mass_kg);
					num2 = orbitalElementsState5.OrbitalPeriod(destinationBarycenter.mass_kg);
					if (microthrustSphere3.IsLimitedBySphereOfInfluence)
					{
						vector3d6 = new Vector3d(0f, 0f, 0f);
						num6 = 0.0;
					}
					else
					{
						vector3d6 = cartesianState2.velocity;
						num6 = destinationBarycenter.localEscapeVelocity_mps(cartesianState2.position.magnitude);
					}
				}
				if (vector3d6.magnitude > num6)
				{
					if (destinationOrbitElements.eccentricity >= 1.0)
					{
						Vector3d vector3d7 = cartesianState2.velocity.normalized * destinationBarycenter.localEscapeVelocity_mps(cartesianState2.position.magnitude);
						vector3d2 = cartesianState2.velocity - vector3d7;
						if (cartesianState2.velocity.sqrMagnitude < vector3d7.sqrMagnitude)
						{
							vector3d2 = default(Vector3d);
						}
					}
					vector3d6 = new Vector3d(0f, 0f, 0f);
					num6 = 0.0;
				}
				cartesianState2.velocity = vector3d2;
				PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState2, destinationBarycenter, tidateTime2);
				if (destinationBarycenter.barycenter != commonBarycenter)
				{
					TINaturalSpaceObjectState barycenter2 = destinationBarycenter.barycenter;
					MicrothrustSphere microthrustSphere4 = new MicrothrustSphere(fleetAcceleration_mps2, barycenter2.mu, barycenter2.sphereOfInfluence_m);
					TIDateTime tidateTime4 = new TIDateTime(tidateTime2);
					OrbitalElementsState orbitalElementsState6 = cartesianState2.ToOrbitalElementsState(barycenter2.mu, new DateTime?(tidateTime2.ExportTime()));
					if (orbitalElementsState6.semiMajorAxis_m >= microthrustSphere4.Radius_m || orbitalElementsState6.eccentricity >= 1.0)
					{
						vector3d6 += cartesianState2.velocity;
						num6 += destinationBarycenter.barycenter.localEscapeVelocity_mps(cartesianState2.position.magnitude);
					}
					else
					{
						num6 -= vector3d6.magnitude;
						vector3d6 = new Vector3d(0f, 0f, 0f);
						double num9 = Mathd.Sqrt(barycenter2.mu / orbitalElementsState6.semiMajorAxis_m);
						tidateTime2.AddSeconds(-microthrustSphere4.GetDuration_s(num9));
						double anomalyDelta_Rad2 = microthrustSphere4.GetAnomalyDelta_Rad(num7);
						double num10 = orbitalElementsState6.MeanAnomalyAtTime_Rad(tidateTime4.ExportTime(), barycenter2.mu) - anomalyDelta_Rad2;
						MicrothrustTransferSegment microthrustTransferSegment3 = new MicrothrustTransferSegment
						{
							startTime = new TIDateTime(tidateTime2),
							endTime = tidateTime4,
							barycenter = barycenter2,
							startRadius_m = microthrustSphere4.Radius_m,
							endRadius_m = destinationBarycenter.semiMajorAxis_m,
							startAnomaly_Rad = num10,
							anomalyDelta_Rad = anomalyDelta_Rad2,
							eccentricity = orbitalElementsState6.eccentricity,
							ascendingNode_rad = orbitalElementsState6.longAscendingNode_Rad,
							inclination_rad = orbitalElementsState6.inclination_Rad,
							argP_rad = orbitalElementsState6.argPeriapsis_Rad
						};
						list2.Add(microthrustTransferSegment3);
						OrbitalElementsState orbitalElementsState7 = new OrbitalElementsState(orbitalElementsState6);
						orbitalElementsState7.semiMajorAxis_m = microthrustTransferSegment3.startRadius_m;
						orbitalElementsState7.meanAnomalyAtEpoch_Rad = microthrustTransferSegment3.startAnomaly_Rad;
						orbitalElementsState7.epoch = tidateTime2.ExportTime();
						cartesianState2 = orbitalElementsState7.ToCartesianStateAtTime(tidateTime2.ExportTime(), barycenter2.mass_kg);
						num2 = orbitalElementsState7.OrbitalPeriod(barycenter2.mass_kg);
						if (!microthrustSphere4.IsLimitedBySphereOfInfluence)
						{
							vector3d6 += cartesianState2.velocity;
							num6 += destinationBarycenter.barycenter.localEscapeVelocity_mps(cartesianState2.position.magnitude);
						}
					}
					if (orbitalElementsState6.eccentricity > 1.0)
					{
						Vector3d vector3d8 = cartesianState2.velocity.normalized * barycenter2.localEscapeVelocity_mps(cartesianState2.position.magnitude);
						vector3d2 = cartesianState2.velocity - vector3d8;
						if (cartesianState2.velocity.sqrMagnitude < vector3d8.sqrMagnitude)
						{
							vector3d2 = new Vector3d(0f, 0f, 0f);
						}
					}
					else
					{
						vector3d = new Vector3d(0f, 0f, 0f);
					}
					cartesianState2.velocity = vector3d;
					PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState2, barycenter2, tidateTime2);
					if (vector3d6.magnitude > num6)
					{
						vector3d6 = new Vector3d(0f, 0f, 0f);
						num6 = 0.0;
					}
				}
			}
			double magnitude = cartesianState.position.magnitude;
			double magnitude2 = cartesianState2.position.magnitude;
			MicrothrustSphere microthrustSphere5 = new MicrothrustSphere(fleetAcceleration_mps2, commonBarycenter.mu, commonBarycenter.sphereOfInfluence_m);
			if (magnitude <= microthrustSphere5.Radius_m)
			{
				num3 -= vector3d3.magnitude;
				vector3d3 = new Vector3d(0f, 0f, 0f);
			}
			if (magnitude2 <= microthrustSphere5.Radius_m)
			{
				num6 -= vector3d6.magnitude;
				vector3d6 = new Vector3d(0f, 0f, 0f);
			}
			bool flag2 = true;
			bool flag3;
			if (tinaturalSpaceObjectState == commonBarycenter)
			{
				flag3 = orbitalElementsState.eccentricity < 1.0 && orbitalElementsState.semiMajorAxis_m < microthrustSphere5.Radius_m;
			}
			else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
			{
				flag3 = tinaturalSpaceObjectState.semiMajorAxis_m < microthrustSphere5.Radius_m;
			}
			else
			{
				TINaturalSpaceObjectState barycenter3 = tinaturalSpaceObjectState.barycenter;
				flag3 = ((barycenter3 != null) ? barycenter3.barycenter : null) == commonBarycenter && tinaturalSpaceObjectState.barycenter.semiMajorAxis_m < microthrustSphere5.Radius_m;
			}
			bool flag4 = ((destinationBarycenter == commonBarycenter) ? (destinationOrbitElements.semiMajorAxis_m < microthrustSphere5.Radius_m && destinationOrbitElements.eccentricity < 1.0) : ((destinationBarycenter.barycenter == commonBarycenter) ? (destinationBarycenter.semiMajorAxis_m < microthrustSphere5.Radius_m && destinationBarycenter.ecc < 1.0) : (destinationBarycenter.barycenter.semiMajorAxis_m < microthrustSphere5.Radius_m && destinationBarycenter.barycenter.ecc < 1.0)));
			if (flag3 && flag4)
			{
				return this.SolveMultiBarycenterMicrothrustOnly(launchTime, arrivalTime, originValue, destinationOrbitElements, destinationBarycenter, commonBarycenter, fleetAcceleration_mps2, anyMeanAnomalyAtArrival);
			}
			if (flag3)
			{
				OrbitalElementsState orbitalElementsState8 = cartesianState.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime.ExportTime()));
				if (orbitalElementsState8.eccentricity >= 1.0)
				{
					return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, orbitalElementsState8.eccentricity, 0.0);
				}
				TIDateTime tidateTime5 = new TIDateTime(tidateTime);
				OrbitalElementsState orbitalElementsState9 = new OrbitalElementsState(orbitalElementsState8);
				double num11 = Mathd.Sqrt(commonBarycenter.mu / orbitalElementsState9.semiMajorAxis_m);
				tidateTime.AddSeconds(microthrustSphere5.GetDuration_s(num11));
				MicrothrustTransferSegment microthrustTransferSegment4 = new MicrothrustTransferSegment
				{
					startTime = tidateTime5,
					endTime = tidateTime,
					barycenter = commonBarycenter,
					startRadius_m = orbitalElementsState9.semiMajorAxis_m,
					endRadius_m = microthrustSphere5.Radius_m,
					startAnomaly_Rad = orbitalElementsState9.MeanAnomalyAtTime_Rad(tidateTime5.ExportTime(), commonBarycenter.mass_kg),
					anomalyDelta_Rad = microthrustSphere5.GetAnomalyDelta_Rad(num11),
					eccentricity = orbitalElementsState9.eccentricity,
					ascendingNode_rad = orbitalElementsState9.longAscendingNode_Rad,
					inclination_rad = orbitalElementsState9.inclination_Rad,
					argP_rad = orbitalElementsState9.argPeriapsis_Rad
				};
				list.Add(microthrustTransferSegment4);
				OrbitalElementsState orbitalElementsState10 = new OrbitalElementsState(orbitalElementsState9);
				orbitalElementsState10.semiMajorAxis_m = microthrustTransferSegment4.endRadius_m;
				orbitalElementsState10.meanAnomalyAtEpoch_Rad = microthrustTransferSegment4.endAnomaly;
				orbitalElementsState10.epoch = tidateTime.ExportTime();
				cartesianState = orbitalElementsState10.ToCartesianStateAtTime(tidateTime.ExportTime(), commonBarycenter.mass_kg);
				num = orbitalElementsState10.OrbitalPeriod(commonBarycenter.mass_kg);
			}
			else if (flag4)
			{
				TIDateTime tidateTime6 = new TIDateTime(tidateTime2);
				OrbitalElementsState orbitalElementsState11 = cartesianState2.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime2.ExportTime()));
				if (orbitalElementsState11.eccentricity >= 1.0)
				{
					return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, orbitalElementsState11.eccentricity, 0.0);
				}
				OrbitalElementsState orbitalElementsState12 = new OrbitalElementsState(orbitalElementsState11);
				double num12 = Mathd.Sqrt(commonBarycenter.mu / orbitalElementsState12.semiMajorAxis_m);
				tidateTime2.AddSeconds(-microthrustSphere5.GetDuration_s(num12));
				double anomalyDelta_Rad3 = microthrustSphere5.GetAnomalyDelta_Rad(num12);
				double num13 = orbitalElementsState12.MeanAnomalyAtTime_Rad(tidateTime6.ExportTime(), commonBarycenter.mass_kg) - anomalyDelta_Rad3;
				MicrothrustTransferSegment microthrustTransferSegment5 = new MicrothrustTransferSegment
				{
					startTime = new TIDateTime(tidateTime2),
					endTime = tidateTime6,
					barycenter = commonBarycenter,
					startRadius_m = microthrustSphere5.Radius_m,
					endRadius_m = orbitalElementsState12.semiMajorAxis_m,
					startAnomaly_Rad = num13,
					anomalyDelta_Rad = anomalyDelta_Rad3,
					eccentricity = orbitalElementsState12.eccentricity,
					ascendingNode_rad = orbitalElementsState12.longAscendingNode_Rad,
					inclination_rad = orbitalElementsState12.inclination_Rad,
					argP_rad = orbitalElementsState12.argPeriapsis_Rad
				};
				list2.Add(microthrustTransferSegment5);
				OrbitalElementsState orbitalElementsState13 = new OrbitalElementsState(orbitalElementsState12);
				orbitalElementsState13.semiMajorAxis_m = microthrustTransferSegment5.startRadius_m;
				orbitalElementsState13.meanAnomalyAtEpoch_Rad = microthrustTransferSegment5.startAnomaly_Rad;
				orbitalElementsState13.epoch = tidateTime2.ExportTime();
				cartesianState2 = orbitalElementsState13.ToCartesianStateAtTime(tidateTime2.ExportTime(), commonBarycenter.mass_kg);
				num2 = orbitalElementsState13.OrbitalPeriod(commonBarycenter.mass_kg);
			}
			base.boost_DV_mps = list.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.DV_mps);
			base.decel_DV_mps = list2.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.DV_mps);
			this.transferSegments = list;
			if (tidateTime2 < tidateTime)
			{
				double num14 = list.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.endTime.DifferenceInSeconds(x.startTime));
				num14 += list2.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.endTime.DifferenceInSeconds(x.startTime));
				return new TransferResult(TransferResult.Outcome.Fail_CoastPhaseEndsBeforeItStarts, tidateTime.DifferenceInSeconds(tidateTime2), num14);
			}
			if (flag2)
			{
				if (internalTransferType != PatchedTransfer.InternalTransferType.Lambert)
				{
					if (internalTransferType != PatchedTransfer.InternalTransferType.Torch)
					{
						Debug.LogError("PatchedTransfer: did not recognize internal transfer type: " + internalTransferType.ToString());
						return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
					}
					TorchTransfer torchTransfer = new TorchTransfer();
					double num15 = tidateTime2.DifferenceInSeconds(tidateTime);
					if (num15 <= 0.0)
					{
						return new TransferResult(TransferResult.Outcome.Fail_ArrivalBeforeLaunch, num15, 0.0);
					}
					CartesianState cartesianState3 = Quaterniond.Inverse(commonBarycenter.SpatialRotation) * cartesianState.xzy;
					CartesianState cartesianState4 = cartesianState3.xzy + commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime);
					cartesianState3 = Quaterniond.Inverse(commonBarycenter.SpatialRotation) * cartesianState2.xzy;
					CartesianState cartesianState5 = cartesianState3.xzy + commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime2);
					bool flag5;
					TransferResult transferResult = torchTransfer.Solve(tidateTime, num15, fleetAcceleration_mps2, cartesianState4, cartesianState5, commonBarycenter, double.PositiveInfinity, out flag5, false);
					if (transferResult.Result != TransferResult.Outcome.Success)
					{
						return transferResult;
					}
					cartesianState3 = cartesianState.ChangeReferenceFrame(commonBarycenter, tinaturalSpaceObjectState, torchTransfer.launchTime);
					double magnitude3 = cartesianState3.position.magnitude;
					cartesianState3 = cartesianState2.ChangeReferenceFrame(commonBarycenter, destinationBarycenter, torchTransfer.arrivalTime);
					double magnitude4 = cartesianState3.position.magnitude;
					double num16 = this.GravityTaxForTorch_mps(torchTransfer.coastVelocity_mps.magnitude, magnitude3, tinaturalSpaceObjectState, commonBarycenter, torchTransfer.launchTime);
					double num17 = this.GravityTaxForTorch_mps(torchTransfer.coastVelocity_mps.magnitude, magnitude4, destinationBarycenter, commonBarycenter, torchTransfer.arrivalTime);
					double num18 = num16 / fleetAcceleration_mps2;
					double num19 = num17 / fleetAcceleration_mps2;
					if (num18 + num19 > torchTransfer.coastDuration_s)
					{
						return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, num18 + num19 + torchTransfer.accelDuration_s + torchTransfer.decelDuration_s, torchTransfer.coastDuration_s + torchTransfer.accelDuration_s + torchTransfer.decelDuration_s);
					}
					TorchTransferSegment torchTransferSegment = new TorchTransferSegment
					{
						torch = torchTransfer,
						barycenter = commonBarycenter,
						initialGravwellDuration_s = num18,
						finalGravwellDuration_s = num19,
						fleetAcceleration_mps = fleetAcceleration_mps2,
						initialGlobalVelocity_mps = cartesianState4.velocity,
						finalGlobalVelocity_mps = cartesianState5.velocity
					};
					this.transferSegments.Add(torchTransferSegment);
					base.boost_DV_mps += torchTransfer.boost_DV_mps + torchTransferSegment.initialGravwellDuration_s * fleetAcceleration_mps2;
					base.decel_DV_mps += torchTransfer.decel_DV_mps + torchTransferSegment.finalGravwellDuration_s * fleetAcceleration_mps2;
				}
				else
				{
					TwoBurnLambertTransfer twoBurnLambertTransfer = new TwoBurnLambertTransfer();
					TransferResult transferResult2 = twoBurnLambertTransfer.SolveCartesian(tidateTime, tidateTime2, tidateTime2.DifferenceInSeconds(tidateTime), cartesianState, cartesianState2, commonBarycenter, fleetAcceleration_mps2);
					if (transferResult2.Result != TransferResult.Outcome.Success)
					{
						return transferResult2;
					}
					if (twoBurnLambertTransfer.launchTime < TITimeState.Now())
					{
						double num20 = TITimeState.Now().DifferenceInSeconds(twoBurnLambertTransfer.launchTime);
						double num21 = twoBurnLambertTransfer.boost_DV_mps / fleetAcceleration_mps2;
						return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, num20, num21);
					}
					CartesianState cartesianState6 = twoBurnLambertTransfer.transferOrbit.ToCartesianStateAtTime(tidateTime.ExportTime(), commonBarycenter.mass_kg);
					CartesianState cartesianState7 = twoBurnLambertTransfer.transferOrbit.ToCartesianStateAtTime(tidateTime2.ExportTime(), commonBarycenter.mass_kg);
					double num22 = this.GravityTaxForLambert_mps(cartesianState6.velocity.magnitude, cartesianState6.position.magnitude, tinaturalSpaceObjectState, commonBarycenter, tidateTime);
					double num23 = this.GravityTaxForLambert_mps(cartesianState7.velocity.magnitude, cartesianState7.position.magnitude, destinationBarycenter, commonBarycenter, tidateTime2);
					transferResult2 = twoBurnLambertTransfer.ModifyDV(num22, num23, fleetAcceleration_mps2);
					if (transferResult2.Result != TransferResult.Outcome.Success)
					{
						return transferResult2;
					}
					if (twoBurnLambertTransfer.launchTime < TITimeState.Now())
					{
						double num24 = TITimeState.Now().DifferenceInSeconds(twoBurnLambertTransfer.launchTime);
						double num25 = twoBurnLambertTransfer.boost_DV_mps / fleetAcceleration_mps2;
						return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, num24, num25);
					}
					double num26 = twoBurnLambertTransfer.boost_DV_mps / fleetAcceleration_mps2;
					double num27 = twoBurnLambertTransfer.decel_DV_mps / fleetAcceleration_mps2;
					if (num26 * 2.0 > num || num27 * 2.0 > num2)
					{
						double num28 = num26 / num;
						double num29 = num27 / num2;
					}
					TIDateTime tidateTime7 = (list.Any<IPatchedTransferSegment>() ? tidateTime : new TIDateTime(tidateTime, -num26 / 2.0));
					double num30 = (list2.Any<IPatchedTransferSegment>() ? tidateTime2 : new TIDateTime(tidateTime2, num27 / 2.0)).DifferenceInSeconds(tidateTime7);
					if (num26 + num27 > num30)
					{
						double num31 = list.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.endTime.DifferenceInSeconds(x.startTime));
						double num32 = list2.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.endTime.DifferenceInSeconds(x.startTime));
						double num33 = num30 + num31 + num32;
						double num34 = num26 + num27 + num31 + num32;
						return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, num34, num33);
					}
					ImpulseTransferSegment impulseTransferSegment = new ImpulseTransferSegment
					{
						lambert = twoBurnLambertTransfer,
						barycenter = commonBarycenter
					};
					this.transferSegments.Add(impulseTransferSegment);
					base.boost_DV_mps += twoBurnLambertTransfer.boost_DV_mps;
					base.decel_DV_mps += twoBurnLambertTransfer.decel_DV_mps;
				}
			}
			list2.Reverse();
			this.transferSegments.AddRange(list2);
			TIDateTime startTime = this.transferSegments.Min<IPatchedTransferSegment, TIDateTime>((IPatchedTransferSegment x) => x.startTime);
			if (startTime < TITimeState.Now())
			{
				double num35 = this.transferSegments.First<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.startTime == startTime).DV_mps / fleetAcceleration_mps2;
				return new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, startTime.DifferenceInSeconds(TITimeState.Now()), num35);
			}
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
		}

		// Token: 0x06003E56 RID: 15958 RVA: 0x001938DC File Offset: 0x00191ADC
		private double GravityTaxForTorch_mps(double coastSpeedAroundCommonBarycenter_mps, double distanceFromLocalBarycenter_m, TINaturalSpaceObjectState localBarycenter, TINaturalSpaceObjectState commonBarycenter, TIDateTime burnTime)
		{
			double num = this.TotalGravityTaxForLambertSquared_m2ps2(coastSpeedAroundCommonBarycenter_mps, distanceFromLocalBarycenter_m, localBarycenter, commonBarycenter, burnTime);
			TINaturalSpaceObjectState tinaturalSpaceObjectState = ((localBarycenter == commonBarycenter) ? null : ((localBarycenter.barycenter == commonBarycenter) ? localBarycenter : localBarycenter.barycenter));
			if (tinaturalSpaceObjectState != null)
			{
				double num2 = commonBarycenter.localEscapeVelocity_mps(tinaturalSpaceObjectState.ToLocalCartesianStateAtTime(burnTime).position.magnitude);
				num += num2 * num2;
			}
			else
			{
				double num3 = commonBarycenter.localEscapeVelocity_mps(distanceFromLocalBarycenter_m);
				num += num3 * num3;
			}
			return Mathd.Sqrt(num) - coastSpeedAroundCommonBarycenter_mps;
		}

		// Token: 0x06003E57 RID: 15959 RVA: 0x00193965 File Offset: 0x00191B65
		private double GravityTaxForLambert_mps(double speedAroundCommonBarycenterAfterBurn_mps, double distanceFromLocalBarycenter_m, TINaturalSpaceObjectState localBarycenter, TINaturalSpaceObjectState commonBarycenter, TIDateTime burnTime)
		{
			return Mathd.Sqrt(this.TotalGravityTaxForLambertSquared_m2ps2(speedAroundCommonBarycenterAfterBurn_mps, distanceFromLocalBarycenter_m, localBarycenter, commonBarycenter, burnTime)) - speedAroundCommonBarycenterAfterBurn_mps;
		}

		// Token: 0x06003E58 RID: 15960 RVA: 0x0019397C File Offset: 0x00191B7C
		private double TotalGravityTaxForLambertSquared_m2ps2(double speedAroundCommonBarycenterAfterBurn_mps, double distanceFromLocalBarycenter_m, TINaturalSpaceObjectState localBarycenter, TINaturalSpaceObjectState commonBarycenter, TIDateTime burnTime)
		{
			if (localBarycenter == null || commonBarycenter == null || burnTime == null)
			{
				Log.Error("Necessary input was null.", Array.Empty<object>());
				return speedAroundCommonBarycenterAfterBurn_mps * speedAroundCommonBarycenterAfterBurn_mps;
			}
			if (localBarycenter != commonBarycenter && localBarycenter.barycenter != commonBarycenter)
			{
				TINaturalSpaceObjectState barycenter = localBarycenter.barycenter;
				if (((barycenter != null) ? barycenter.barycenter : null) != commonBarycenter)
				{
					Log.Error("commonBarycenter was not common to localBarycenter.", Array.Empty<object>());
					return speedAroundCommonBarycenterAfterBurn_mps * speedAroundCommonBarycenterAfterBurn_mps;
				}
			}
			if (speedAroundCommonBarycenterAfterBurn_mps <= 0.0)
			{
				Log.Error("Negative or zero speed after burn: " + speedAroundCommonBarycenterAfterBurn_mps.ToString() + " m/s", Array.Empty<object>());
				return speedAroundCommonBarycenterAfterBurn_mps * speedAroundCommonBarycenterAfterBurn_mps;
			}
			double num = speedAroundCommonBarycenterAfterBurn_mps * speedAroundCommonBarycenterAfterBurn_mps;
			if (localBarycenter != commonBarycenter)
			{
				double num2 = localBarycenter.localEscapeVelocity_mps(distanceFromLocalBarycenter_m);
				num += num2 * num2;
				if (localBarycenter.barycenter != commonBarycenter)
				{
					double num3 = localBarycenter.barycenter.localEscapeVelocity_mps(localBarycenter.ToLocalCartesianStateAtTime(burnTime).position.magnitude);
					num += num3 * num3;
				}
			}
			return num;
		}

		// Token: 0x06003E59 RID: 15961 RVA: 0x00193A84 File Offset: 0x00191C84
		private TransferResult SolveMultiBarycenterMicrothrustOnly(TIDateTime launchTime, TIDateTime arrivalTime, ITransferTarget originValue, OrbitalElementsState destinationOrbitElements, TINaturalSpaceObjectState destinationBarycenter, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, bool anyMeanAnomalyAtArrival)
		{
			if (destinationOrbitElements.eccentricity >= 1.0)
			{
				return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, destinationOrbitElements.eccentricity, 0.0);
			}
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			originValue.getOrbitalElementsState(launchTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			if (orbitalElementsState.eccentricity >= 1.0)
			{
				return new TransferResult(TransferResult.Outcome.Fail_HyperbolicMicrothrust, orbitalElementsState.eccentricity, 0.0);
			}
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, commonBarycenter.mu, commonBarycenter.sphereOfInfluence_m);
			double num;
			if (tinaturalSpaceObjectState == commonBarycenter)
			{
				num = orbitalElementsState.semiMajorAxis_m;
			}
			else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
			{
				num = tinaturalSpaceObjectState.semiMajorAxis_m;
			}
			else
			{
				TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
				if (!(((barycenter != null) ? barycenter.barycenter : null) == commonBarycenter))
				{
					string[] array = new string[5];
					array[0] = "Multibarycenter microthrust: the origin's barycenter (";
					int num2 = 1;
					TINaturalSpaceObjectState tinaturalSpaceObjectState2 = tinaturalSpaceObjectState;
					array[num2] = ((tinaturalSpaceObjectState2 != null) ? tinaturalSpaceObjectState2.ToString() : null);
					array[2] = ") was not within the sphere of influence of the common barycenter (";
					array[3] = ((commonBarycenter != null) ? commonBarycenter.ToString() : null);
					array[4] = ")";
					Log.Error(string.Concat(array), Array.Empty<object>());
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				num = tinaturalSpaceObjectState.barycenter.semiMajorAxis_m;
			}
			double num3;
			if (destinationBarycenter == commonBarycenter)
			{
				num3 = destinationOrbitElements.semiMajorAxis_m;
			}
			else if (destinationBarycenter.barycenter == commonBarycenter)
			{
				num3 = destinationBarycenter.semiMajorAxis_m;
			}
			else
			{
				TINaturalSpaceObjectState barycenter2 = destinationBarycenter.barycenter;
				if (!(((barycenter2 != null) ? barycenter2.barycenter : null) == commonBarycenter))
				{
					Log.Error(string.Concat(new string[]
					{
						"Multibarycenter microthrust: the destination's barycenter (",
						(destinationBarycenter != null) ? destinationBarycenter.ToString() : null,
						") was now within the sphere of influence of the common barycenter (",
						(commonBarycenter != null) ? commonBarycenter.ToString() : null,
						")"
					}), Array.Empty<object>());
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				num3 = destinationBarycenter.barycenter.semiMajorAxis_m;
			}
			double num4 = Mathd.Sqrt(commonBarycenter.mu / num);
			double num5 = Mathd.Sqrt(commonBarycenter.mu / num3);
			double num6 = Mathd.Abs(microthrustSphere.GetDuration_s(num4) - microthrustSphere.GetDuration_s(num5));
			double num7 = microthrustSphere.GetAnomalyDelta_Rad(num4) - microthrustSphere.GetAnomalyDelta_Rad(num5);
			if (num > num3)
			{
				num7 = -num7;
			}
			num7 = MasterTransferPlanner.NormalizeAngleNearPi_Rad(num7);
			ValueTuple<MicrothrustSphere, double, double> valueTuple = new ValueTuple<MicrothrustSphere, double, double>(microthrustSphere, num6, num7);
			ValueTuple<MicrothrustSphere, double, double> valueTuple2 = new ValueTuple<MicrothrustSphere, double, double>(null, 0.0, 0.0);
			ValueTuple<MicrothrustSphere, double, double> valueTuple3 = new ValueTuple<MicrothrustSphere, double, double>(null, 0.0, 0.0);
			ValueTuple<MicrothrustSphere, double, double> valueTuple4 = new ValueTuple<MicrothrustSphere, double, double>(null, 0.0, 0.0);
			ValueTuple<MicrothrustSphere, double, double> valueTuple5 = new ValueTuple<MicrothrustSphere, double, double>(null, 0.0, 0.0);
			if (tinaturalSpaceObjectState != commonBarycenter)
			{
				MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState.mu, tinaturalSpaceObjectState.sphereOfInfluence_m);
				double num8 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / orbitalElementsState.semiMajorAxis_m);
				double duration_s = microthrustSphere2.GetDuration_s(num8);
				double anomalyDelta_Rad = microthrustSphere2.GetAnomalyDelta_Rad(num8);
				valueTuple2 = new ValueTuple<MicrothrustSphere, double, double>(microthrustSphere2, duration_s, anomalyDelta_Rad);
				if (tinaturalSpaceObjectState.barycenter != commonBarycenter)
				{
					MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState.barycenter.mu, tinaturalSpaceObjectState.barycenter.sphereOfInfluence_m);
					double num9 = Mathd.Sqrt(tinaturalSpaceObjectState.barycenter.mu / tinaturalSpaceObjectState.semiMajorAxis_m);
					double duration_s2 = microthrustSphere3.GetDuration_s(num9);
					double anomalyDelta_Rad2 = microthrustSphere3.GetAnomalyDelta_Rad(num9);
					valueTuple3 = new ValueTuple<MicrothrustSphere, double, double>(microthrustSphere3, duration_s2, anomalyDelta_Rad2);
				}
			}
			if (destinationBarycenter != commonBarycenter)
			{
				MicrothrustSphere microthrustSphere4 = new MicrothrustSphere(fleetAcceleration_mps2, destinationBarycenter.mu, destinationBarycenter.sphereOfInfluence_m);
				double num10 = Mathd.Sqrt(destinationBarycenter.mu / destinationOrbitElements.semiMajorAxis_m);
				double duration_s3 = microthrustSphere4.GetDuration_s(num10);
				double anomalyDelta_Rad3 = microthrustSphere4.GetAnomalyDelta_Rad(num10);
				valueTuple5 = new ValueTuple<MicrothrustSphere, double, double>(microthrustSphere4, duration_s3, anomalyDelta_Rad3);
				if (destinationBarycenter.barycenter != commonBarycenter)
				{
					MicrothrustSphere microthrustSphere5 = new MicrothrustSphere(fleetAcceleration_mps2, destinationBarycenter.barycenter.mu, destinationBarycenter.barycenter.sphereOfInfluence_m);
					double num11 = Mathd.Sqrt(destinationBarycenter.barycenter.mu / destinationBarycenter.semiMajorAxis_m);
					double duration_s4 = microthrustSphere5.GetDuration_s(num11);
					double anomalyDelta_Rad4 = microthrustSphere5.GetAnomalyDelta_Rad(num11);
					valueTuple4 = new ValueTuple<MicrothrustSphere, double, double>(microthrustSphere5, duration_s4, anomalyDelta_Rad4);
				}
			}
			TIDateTime tidateTime = new TIDateTime(launchTime, valueTuple2.Item2 + valueTuple3.Item2);
			double num12 = num;
			TIDateTime tidateTime2 = launchTime;
			TIDateTime tidateTime3 = new TIDateTime(launchTime, valueTuple2.Item2 + valueTuple3.Item2 + valueTuple.Item2 + valueTuple4.Item2 + valueTuple5.Item2);
			TIDateTime tidateTime4;
			double num13;
			if (destinationBarycenter == commonBarycenter && anyMeanAnomalyAtArrival)
			{
				tidateTime4 = tidateTime;
				num13 = destinationOrbitElements.semiMajorAxis_m;
				if (tinaturalSpaceObjectState == commonBarycenter)
				{
					orbitalElementsState.MeanAnomalyAtTime_Rad(tidateTime4.ExportTime(), tinaturalSpaceObjectState.mass_kg);
				}
				else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
				{
					tinaturalSpaceObjectState.meanAnomaly_Rad(tidateTime4);
				}
				else
				{
					tinaturalSpaceObjectState.barycenter.meanAnomaly_Rad(tidateTime4);
				}
			}
			else
			{
				double num14;
				if (tinaturalSpaceObjectState == commonBarycenter)
				{
					num14 = orbitalElementsState.MeanLongitudeAtTime_Rad(tidateTime.ExportTime(), tinaturalSpaceObjectState.mass_kg);
				}
				else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
				{
					num14 = tinaturalSpaceObjectState.meanLongitudeAtTime_Rad(tidateTime);
					double argPeriapsis_Rad = tinaturalSpaceObjectState.argPeriapsis_Rad;
					double longAscendingNode_Rad = tinaturalSpaceObjectState.longAscendingNode_Rad;
				}
				else
				{
					num14 = tinaturalSpaceObjectState.barycenter.meanLongitudeAtTime_Rad(tidateTime);
					double argPeriapsis_Rad2 = tinaturalSpaceObjectState.barycenter.argPeriapsis_Rad;
					double longAscendingNode_Rad2 = tinaturalSpaceObjectState.barycenter.longAscendingNode_Rad;
				}
				double num15 = Mathd.Sqrt(commonBarycenter.mu / PatchedTransfer.Cubed(num));
				TIDateTime tidateTime5 = new TIDateTime(tidateTime, valueTuple.Item2);
				double num16;
				double num17;
				if (destinationBarycenter == commonBarycenter)
				{
					num16 = destinationOrbitElements.MeanLongitudeAtTime_Rad(tidateTime5.ExportTime(), destinationBarycenter.mass_kg);
					double semiMajorAxis_m = destinationOrbitElements.semiMajorAxis_m;
					num17 = Mathd.Sqrt(commonBarycenter.mu / (semiMajorAxis_m * semiMajorAxis_m * semiMajorAxis_m));
					num13 = destinationOrbitElements.semiMajorAxis_m;
				}
				else if (destinationBarycenter.barycenter == commonBarycenter)
				{
					num16 = destinationBarycenter.meanLongitudeAtTime_Rad(tidateTime5);
					double semiMajorAxis_m2 = destinationBarycenter.semiMajorAxis_m;
					num17 = Mathd.Sqrt(commonBarycenter.mu / (semiMajorAxis_m2 * semiMajorAxis_m2 * semiMajorAxis_m2));
					num13 = destinationBarycenter.semiMajorAxis_m;
				}
				else
				{
					num16 = destinationBarycenter.barycenter.meanLongitudeAtTime_Rad(tidateTime5);
					double semiMajorAxis_m3 = destinationBarycenter.barycenter.semiMajorAxis_m;
					num17 = Mathd.Sqrt(commonBarycenter.mu / (semiMajorAxis_m3 * semiMajorAxis_m3 * semiMajorAxis_m3));
					num13 = destinationBarycenter.barycenter.semiMajorAxis_m;
				}
				double num18 = Mathd.ClampRadiansTwoPI(num16 - num14 - valueTuple.Item3);
				double num19 = num15 - num17;
				double num20;
				if (num19 > 0.0)
				{
					num20 = num18 / num19;
				}
				else
				{
					num20 = (num18 - 6.283185307179586) / num19;
				}
				if (num20 > 94670772.0 || double.IsInfinity(num20) || double.IsNaN(num20))
				{
					double num21 = Mathd.Pow(commonBarycenter.mu / (num15 * num15), 0.3333333333333333);
					double num22 = Mathd.Pow(commonBarycenter.mu / (num17 * num17), 0.3333333333333333);
					double num23 = (num21 + num22) / 2.0;
					double num24 = commonBarycenter.mu / (num23 * num23 * 2.0);
					return new TransferResult(TransferResult.Outcome.Fail_InsufficientAcceleration, num24, 0.0);
				}
				tidateTime4 = new TIDateTime(tidateTime, num20);
				tidateTime2 = new TIDateTime(launchTime, num20);
				tidateTime3 = new TIDateTime(tidateTime3, num20);
				if (tinaturalSpaceObjectState == commonBarycenter)
				{
					orbitalElementsState.MeanAnomalyAtTime_Rad(tidateTime4.ExportTime(), tinaturalSpaceObjectState.mass_kg);
				}
				else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
				{
					tinaturalSpaceObjectState.meanAnomaly_Rad(tidateTime4);
				}
				else
				{
					tinaturalSpaceObjectState.barycenter.meanAnomaly_Rad(tidateTime4);
				}
			}
			MasterTransferPlanner.SynodicPeriod_s(num12, num13, commonBarycenter.mu);
			MicrothrustTransferLERPvalues microthrustTransferLERPvalues;
			if (tinaturalSpaceObjectState == commonBarycenter)
			{
				microthrustTransferLERPvalues = new MicrothrustTransferLERPvalues(orbitalElementsState.semiMajorAxis_m, orbitalElementsState.MeanAnomalyAtTime_Rad(tidateTime4.ExportTime(), commonBarycenter.mass_kg), orbitalElementsState.eccentricity, orbitalElementsState.longAscendingNode_Rad, orbitalElementsState.inclination_Rad, orbitalElementsState.argPeriapsis_Rad, 0.0, 0.0, 0.0);
			}
			else
			{
				TIDateTime tidateTime6 = ((tinaturalSpaceObjectState.barycenter == commonBarycenter) ? tidateTime4 : new TIDateTime(tidateTime2, valueTuple2.Item2));
				MicrothrustTransferSegment microthrustTransferSegment = new MicrothrustTransferSegment
				{
					startTime = tidateTime2,
					endTime = tidateTime6,
					barycenter = tinaturalSpaceObjectState,
					startAnomaly_Rad = orbitalElementsState.MeanAnomalyAtTime_Rad(tidateTime2.ExportTime(), tinaturalSpaceObjectState.mass_kg),
					anomalyDelta_Rad = valueTuple2.Item3,
					startRadius_m = orbitalElementsState.semiMajorAxis_m,
					endRadius_m = valueTuple2.Item1.Radius_m,
					eccentricity = orbitalElementsState.eccentricity,
					ascendingNode_rad = orbitalElementsState.longAscendingNode_Rad,
					inclination_rad = orbitalElementsState.inclination_Rad,
					argP_rad = orbitalElementsState.argPeriapsis_Rad
				};
				this.transferSegments.Add(microthrustTransferSegment);
				CartesianState cartesianState = new OrbitalElementsState
				{
					epoch = tidateTime6.ExportTime(),
					longAscendingNode_Rad = orbitalElementsState.longAscendingNode_Rad,
					argPeriapsis_Rad = orbitalElementsState.argPeriapsis_Rad,
					inclination_Rad = orbitalElementsState.inclination_Rad,
					semiMajorAxis_m = valueTuple2.Item1.Radius_m,
					eccentricity = orbitalElementsState.eccentricity,
					meanAnomalyAtEpoch_Rad = orbitalElementsState.MeanAnomalyAtTime_Rad(tidateTime2.ExportTime(), tinaturalSpaceObjectState.mass_kg) + valueTuple2.Item3
				}.ToCartesianStateAtTime(tidateTime6.ExportTime(), tinaturalSpaceObjectState.mass_kg);
				PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState, tinaturalSpaceObjectState, tidateTime6);
				OrbitalElementsState orbitalElementsState2 = cartesianState.ToOrbitalElementsState(tinaturalSpaceObjectState.barycenter.mu, new DateTime?(tidateTime6.ExportTime()));
				double num25 = Mathd.Sqrt(tinaturalSpaceObjectState.barycenter.mu / tinaturalSpaceObjectState.semiMajorAxis_m) - cartesianState.velocity.magnitude;
				double num26 = num25 * 6.283185307179586 / tinaturalSpaceObjectState.semiMajorAxis_m;
				if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
				{
					double semiMajorAxis_m4 = tinaturalSpaceObjectState.semiMajorAxis_m;
					double num27 = orbitalElementsState2.MeanAnomalyAtTime_Rad(tidateTime4.ExportTime(), commonBarycenter.mass_kg);
					double eccentricity = orbitalElementsState2.eccentricity;
					double longAscendingNode_Rad3 = orbitalElementsState2.longAscendingNode_Rad;
					double inclination_Rad = orbitalElementsState2.inclination_Rad;
					double argPeriapsis_Rad3 = orbitalElementsState2.argPeriapsis_Rad;
					double num28 = 0.0;
					double magnitude = cartesianState.position.magnitude;
					CartesianState cartesianState2 = tinaturalSpaceObjectState.ToLocalCartesianStateAtTime(tidateTime6);
					microthrustTransferLERPvalues = new MicrothrustTransferLERPvalues(semiMajorAxis_m4, num27, eccentricity, longAscendingNode_Rad3, inclination_Rad, argPeriapsis_Rad3, num28, magnitude - cartesianState2.position.magnitude, num26);
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP = new MicrothrustTransferSegmentLERP();
					microthrustTransferSegmentLERP.startTime = tidateTime6;
					microthrustTransferSegmentLERP.endTime = tidateTime4;
					microthrustTransferSegmentLERP.barycenter = tinaturalSpaceObjectState.barycenter;
					double semiMajorAxis_m5 = tinaturalSpaceObjectState.semiMajorAxis_m;
					double meanAnomalyAtEpoch_Rad = orbitalElementsState2.meanAnomalyAtEpoch_Rad;
					double eccentricity2 = orbitalElementsState2.eccentricity;
					double longAscendingNode_Rad4 = orbitalElementsState2.longAscendingNode_Rad;
					double inclination_Rad2 = orbitalElementsState2.inclination_Rad;
					double argPeriapsis_Rad4 = orbitalElementsState2.argPeriapsis_Rad;
					double num29 = 0.0;
					double magnitude2 = cartesianState.position.magnitude;
					CartesianState cartesianState2 = tinaturalSpaceObjectState.ToLocalCartesianStateAtTime(tidateTime4);
					microthrustTransferSegmentLERP.start = new MicrothrustTransferLERPvalues(semiMajorAxis_m5, meanAnomalyAtEpoch_Rad, eccentricity2, longAscendingNode_Rad4, inclination_Rad2, argPeriapsis_Rad4, num29, magnitude2 - cartesianState2.position.magnitude, num26);
					microthrustTransferSegmentLERP.end = new MicrothrustTransferLERPvalues(valueTuple3.Item1.Radius_m, orbitalElementsState2.meanAnomalyAtEpoch_Rad + valueTuple3.Item3, orbitalElementsState2.eccentricity, orbitalElementsState2.longAscendingNode_Rad, orbitalElementsState2.inclination_Rad, orbitalElementsState2.argPeriapsis_Rad, 0.0, 0.0, 0.0);
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP2 = microthrustTransferSegmentLERP;
					this.transferSegments.Add(microthrustTransferSegmentLERP2);
					CartesianState cartesianState3 = new OrbitalElementsState
					{
						epoch = tidateTime4.ExportTime(),
						longAscendingNode_Rad = orbitalElementsState2.longAscendingNode_Rad,
						argPeriapsis_Rad = orbitalElementsState2.argPeriapsis_Rad,
						inclination_Rad = orbitalElementsState2.inclination_Rad,
						semiMajorAxis_m = valueTuple2.Item1.Radius_m,
						eccentricity = orbitalElementsState2.eccentricity,
						meanAnomalyAtEpoch_Rad = orbitalElementsState2.meanAnomalyAtEpoch_Rad + valueTuple3.Item3
					}.ToCartesianStateAtTime(tidateTime4.ExportTime(), tinaturalSpaceObjectState.barycenter.mass_kg);
					PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState3, tinaturalSpaceObjectState.barycenter, tidateTime4);
					OrbitalElementsState orbitalElementsState3 = cartesianState3.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime4.ExportTime()));
					Mathd.Sqrt(commonBarycenter.mu / tinaturalSpaceObjectState.barycenter.semiMajorAxis_m);
					double magnitude3 = cartesianState3.velocity.magnitude;
					double num30 = num25 * 6.283185307179586 / tinaturalSpaceObjectState.barycenter.semiMajorAxis_m;
					microthrustTransferLERPvalues = new MicrothrustTransferLERPvalues(tinaturalSpaceObjectState.barycenter.semiMajorAxis_m, orbitalElementsState3.MeanAnomalyAtTime_Rad(tidateTime4.ExportTime(), commonBarycenter.mass_kg), orbitalElementsState3.eccentricity, orbitalElementsState3.longAscendingNode_Rad, orbitalElementsState3.inclination_Rad, orbitalElementsState3.argPeriapsis_Rad, 0.0, orbitalElementsState3.semiMajorAxis_m - tinaturalSpaceObjectState.barycenter.semiMajorAxis_m, num30);
				}
			}
			microthrustTransferLERPvalues.meanAnomaly_Rad = MasterTransferPlanner.NormalizeAngleNearPi_Rad(microthrustTransferLERPvalues.meanAnomaly_Rad);
			microthrustTransferLERPvalues.meanAnomalyCorrection_Rad = MasterTransferPlanner.NormalizeAngleNearZero_Rad(microthrustTransferLERPvalues.meanAnomalyCorrection_Rad);
			MicrothrustTransferSegment microthrustTransferSegment2 = null;
			MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP3 = null;
			TIDateTime tidateTime7 = new TIDateTime(tidateTime4, valueTuple.Item2);
			MicrothrustTransferLERPvalues microthrustTransferLERPvalues2;
			if (destinationBarycenter == commonBarycenter)
			{
				double num31 = microthrustTransferLERPvalues.meanAnomaly_Rad + valueTuple.Item3;
				double num32 = destinationOrbitElements.longAscendingNode_Rad + destinationOrbitElements.argPeriapsis_Rad - microthrustTransferLERPvalues.ascendingNode_Rad - microthrustTransferLERPvalues.argPeriapsis_Rad;
				double num33;
				if (anyMeanAnomalyAtArrival)
				{
					num33 = num32;
				}
				else
				{
					double num34 = microthrustTransferLERPvalues.ascendingNode_Rad + microthrustTransferLERPvalues.argPeriapsis_Rad + microthrustTransferLERPvalues.meanAnomaly_Rad + valueTuple.Item3;
					num33 = Mathd.ClampRadiansPI(destinationOrbitElements.MeanLongitudeAtTime_Rad(tidateTime7.ExportTime(), commonBarycenter.mass_kg) - num34);
				}
				microthrustTransferLERPvalues2 = new MicrothrustTransferLERPvalues(destinationOrbitElements.semiMajorAxis_m, num31, destinationOrbitElements.eccentricity, destinationOrbitElements.longAscendingNode_Rad, destinationOrbitElements.inclination_Rad, destinationOrbitElements.argPeriapsis_Rad, num33, 0.0, 0.0);
			}
			else
			{
				TIDateTime tidateTime8 = new TIDateTime(tidateTime3, -valueTuple5.Item2);
				double num35;
				if (anyMeanAnomalyAtArrival)
				{
					num35 = 0.0;
				}
				else
				{
					num35 = destinationOrbitElements.MeanAnomalyAtTime_Rad(tidateTime8.ExportTime(), destinationBarycenter.mass_kg);
				}
				microthrustTransferSegment2 = new MicrothrustTransferSegment
				{
					startTime = tidateTime8,
					endTime = tidateTime3,
					barycenter = destinationBarycenter,
					startAnomaly_Rad = num35 - valueTuple5.Item3,
					anomalyDelta_Rad = valueTuple5.Item3,
					startRadius_m = valueTuple5.Item1.Radius_m,
					endRadius_m = destinationOrbitElements.semiMajorAxis_m,
					eccentricity = destinationOrbitElements.eccentricity,
					ascendingNode_rad = destinationOrbitElements.longAscendingNode_Rad,
					inclination_rad = destinationOrbitElements.inclination_Rad,
					argP_rad = destinationOrbitElements.argPeriapsis_Rad
				};
				OrbitalElementsState orbitalElementsState4 = new OrbitalElementsState(destinationOrbitElements);
				orbitalElementsState4.semiMajorAxis_m = valueTuple5.Item1.Radius_m;
				orbitalElementsState4.epoch = tidateTime8.ExportTime();
				orbitalElementsState4.meanAnomalyAtEpoch_Rad = microthrustTransferSegment2.startAnomaly_Rad;
				CartesianState cartesianState4 = orbitalElementsState4.ToCartesianStateAtTime(tidateTime8.ExportTime(), destinationBarycenter.mass_kg);
				PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState4, destinationBarycenter, tidateTime8);
				OrbitalElementsState orbitalElementsState5 = new OrbitalElementsState(destinationBarycenter);
				double num36 = (Mathd.Sqrt(destinationBarycenter.barycenter.mu / destinationBarycenter.semiMajorAxis_m) - cartesianState4.velocity.magnitude) / destinationBarycenter.semiMajorAxis_m;
				double magnitude4 = cartesianState4.position.magnitude;
				if (destinationBarycenter.barycenter == commonBarycenter)
				{
					double num37 = microthrustTransferLERPvalues.ascendingNode_Rad + microthrustTransferLERPvalues.argPeriapsis_Rad + microthrustTransferLERPvalues.meanAnomaly_Rad + valueTuple.Item3;
					double num38 = Mathd.ClampRadiansPI(orbitalElementsState5.MeanAnomalyWhenClosestToPosition_Rad(cartesianState4.position) + orbitalElementsState5.longAscendingNode_Rad + orbitalElementsState5.argPeriapsis_Rad - num37);
					double semiMajorAxis_m6 = destinationBarycenter.semiMajorAxis_m;
					double num39 = microthrustTransferLERPvalues.meanAnomaly_Rad + valueTuple.Item3;
					double eccentricity3 = orbitalElementsState5.eccentricity;
					double longAscendingNode_Rad5 = orbitalElementsState5.longAscendingNode_Rad;
					double inclination_Rad3 = orbitalElementsState5.inclination_Rad;
					double argPeriapsis_Rad5 = orbitalElementsState5.argPeriapsis_Rad;
					double num40 = num38;
					double num41 = magnitude4;
					CartesianState cartesianState2 = destinationBarycenter.ToLocalCartesianStateAtTime(tidateTime8);
					microthrustTransferLERPvalues2 = new MicrothrustTransferLERPvalues(semiMajorAxis_m6, num39, eccentricity3, longAscendingNode_Rad5, inclination_Rad3, argPeriapsis_Rad5, num40, num41 - cartesianState2.position.magnitude, num36);
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP4 = new MicrothrustTransferSegmentLERP();
					microthrustTransferSegmentLERP4.startTime = tidateTime7;
					microthrustTransferSegmentLERP4.endTime = tidateTime8;
					microthrustTransferSegmentLERP4.barycenter = destinationBarycenter.barycenter;
					microthrustTransferSegmentLERP4.start = new MicrothrustTransferLERPvalues(valueTuple4.Item1.Radius_m, orbitalElementsState5.meanAnomalyAtEpoch_Rad - valueTuple4.Item3, orbitalElementsState5.eccentricity, orbitalElementsState5.longAscendingNode_Rad, orbitalElementsState5.inclination_Rad, orbitalElementsState5.argPeriapsis_Rad, 0.0, 0.0, 0.0);
					double semiMajorAxis_m7 = destinationBarycenter.semiMajorAxis_m;
					double meanAnomalyAtEpoch_Rad2 = orbitalElementsState5.meanAnomalyAtEpoch_Rad;
					double eccentricity4 = orbitalElementsState5.eccentricity;
					double longAscendingNode_Rad6 = orbitalElementsState5.longAscendingNode_Rad;
					double inclination_Rad4 = orbitalElementsState5.inclination_Rad;
					double argPeriapsis_Rad6 = orbitalElementsState5.argPeriapsis_Rad;
					double num42 = 0.0;
					double num43 = magnitude4;
					CartesianState cartesianState2 = destinationBarycenter.ToLocalCartesianStateAtTime(tidateTime8);
					microthrustTransferSegmentLERP4.end = new MicrothrustTransferLERPvalues(semiMajorAxis_m7, meanAnomalyAtEpoch_Rad2, eccentricity4, longAscendingNode_Rad6, inclination_Rad4, argPeriapsis_Rad6, num42, num43 - cartesianState2.position.magnitude, num36);
					microthrustTransferSegmentLERP3 = microthrustTransferSegmentLERP4;
					CartesianState cartesianState5 = new OrbitalElementsState
					{
						epoch = tidateTime7.ExportTime(),
						longAscendingNode_Rad = orbitalElementsState5.longAscendingNode_Rad,
						argPeriapsis_Rad = orbitalElementsState5.argPeriapsis_Rad,
						inclination_Rad = orbitalElementsState5.inclination_Rad,
						semiMajorAxis_m = orbitalElementsState5.semiMajorAxis_m,
						eccentricity = orbitalElementsState5.eccentricity,
						meanAnomalyAtEpoch_Rad = orbitalElementsState5.meanAnomalyAtEpoch_Rad - valueTuple4.Item3
					}.ToCartesianStateAtTime(tidateTime7.ExportTime(), destinationBarycenter.barycenter.mass_kg);
					PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState5, destinationBarycenter.barycenter, tidateTime7);
					OrbitalElementsState orbitalElementsState6 = new OrbitalElementsState(destinationBarycenter.barycenter);
					double num44 = microthrustTransferLERPvalues.ascendingNode_Rad + microthrustTransferLERPvalues.argPeriapsis_Rad + microthrustTransferLERPvalues.meanAnomaly_Rad + valueTuple.Item3;
					double num45 = Mathd.ClampRadiansPI(orbitalElementsState6.MeanLongitudeAtTime_Rad(tidateTime7.ExportTime(), commonBarycenter.mass_kg) - num44);
					double num46 = (Mathd.Sqrt(commonBarycenter.mu / destinationBarycenter.barycenter.semiMajorAxis_m) - cartesianState5.velocity.magnitude) * 6.283185307179586 / destinationBarycenter.barycenter.semiMajorAxis_m;
					double semiMajorAxis_m8 = destinationBarycenter.barycenter.semiMajorAxis_m;
					double num47 = microthrustTransferLERPvalues.meanAnomaly_Rad + valueTuple.Item3;
					double eccentricity5 = orbitalElementsState6.eccentricity;
					double longAscendingNode_Rad7 = orbitalElementsState6.longAscendingNode_Rad;
					double inclination_Rad5 = orbitalElementsState6.inclination_Rad;
					double argPeriapsis_Rad7 = orbitalElementsState6.argPeriapsis_Rad;
					double num48 = num45;
					double magnitude5 = cartesianState5.position.magnitude;
					cartesianState2 = destinationBarycenter.barycenter.ToLocalCartesianStateAtTime(tidateTime7);
					microthrustTransferLERPvalues2 = new MicrothrustTransferLERPvalues(semiMajorAxis_m8, num47, eccentricity5, longAscendingNode_Rad7, inclination_Rad5, argPeriapsis_Rad7, num48, magnitude5 - cartesianState2.position.magnitude, num46);
				}
			}
			MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP5 = new MicrothrustTransferSegmentLERP
			{
				startTime = tidateTime4,
				endTime = tidateTime7,
				barycenter = commonBarycenter,
				start = microthrustTransferLERPvalues,
				end = microthrustTransferLERPvalues2
			};
			base.boost_DV_mps = this.transferSegments.Sum<IPatchedTransferSegment>((IPatchedTransferSegment x) => x.DV_mps);
			base.decel_DV_mps = 0.0;
			this.transferSegments.Add(microthrustTransferSegmentLERP5);
			if (microthrustTransferSegmentLERP3 != null)
			{
				this.transferSegments.Add(microthrustTransferSegmentLERP3);
			}
			if (microthrustTransferSegment2 != null)
			{
				this.transferSegments.Add(microthrustTransferSegment2);
			}
			if (microthrustTransferSegmentLERP5.start.radius_m < microthrustTransferSegmentLERP5.end.radius_m)
			{
				base.boost_DV_mps += microthrustTransferSegmentLERP5.DV_mps;
			}
			else
			{
				base.decel_DV_mps += microthrustTransferSegmentLERP5.DV_mps;
			}
			base.decel_DV_mps += ((microthrustTransferSegmentLERP3 != null) ? microthrustTransferSegmentLERP3.DV_mps : 0.0) + ((microthrustTransferSegment2 != null) ? microthrustTransferSegment2.DV_mps : 0.0);
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
		}

		// Token: 0x06003E5A RID: 15962 RVA: 0x00194E34 File Offset: 0x00193034
		[return: TupleElementNames(new string[] { "meanAnomalyCorrection_Rad", "meanAnomalySpeedCorrection_RadPS", "distanceCorrection_m" })]
		private ValueTuple<double, double, double> CalculateMicrothrustLERPcorrections(OrbitalElementsState microthrustOrbit, CartesianState actualCartesian, TIDateTime timeOfJunction, TINaturalSpaceObjectState barycenter)
		{
			double num = microthrustOrbit.MeanAnomalyAtTime_Rad(timeOfJunction.ExportTime(), barycenter.mass_kg);
			double num2 = microthrustOrbit.MeanAnomalyWhenClosestToPosition_Rad(actualCartesian.position);
			double num3 = num2 - num;
			CartesianState cartesianState = microthrustOrbit.ToCartesianStateAtMeanAnomaly(num2, barycenter.mass_kg);
			Vector3d vector3d = actualCartesian.velocity - cartesianState.velocity;
			Vector3d vector3d2 = cartesianState.velocity.normalized;
			double num4 = Vector3d.Dot(in vector3d, in vector3d2) / microthrustOrbit.semiMajorAxis_m;
			vector3d = actualCartesian.position - cartesianState.position;
			vector3d2 = cartesianState.position.normalized;
			double num5 = Vector3d.Dot(in vector3d, in vector3d2);
			return new ValueTuple<double, double, double>(num3, num4, num5);
		}

		// Token: 0x06003E5B RID: 15963 RVA: 0x00194EDD File Offset: 0x001930DD
		private static double Cubed(double a)
		{
			return a * a * a;
		}

		// Token: 0x06003E5C RID: 15964 RVA: 0x00194EE4 File Offset: 0x001930E4
		public static void MoveCartesianStateOutOneBarycenter(ref CartesianState cartesianState, TINaturalSpaceObjectState currentBarycenter, TIDateTime time)
		{
			cartesianState = cartesianState.ChangeReferenceFrame(currentBarycenter, currentBarycenter.barycenter, time);
		}

		// Token: 0x06003E5D RID: 15965 RVA: 0x00194EFC File Offset: 0x001930FC
		private double AdditionalVelocityNeededForEscape_mps(ITransferTarget originValue, TINaturalSpaceObjectState relevantBarycenter, TIDateTime launchTime)
		{
			CartesianState cartesianState = originValue.relevantGlobalCartesianState(relevantBarycenter, launchTime) - relevantBarycenter.ToGlobalCartesianStateAtTime(launchTime);
			return this.AdditionalVelocityNeededForEscape_mps(cartesianState, relevantBarycenter, launchTime);
		}

		// Token: 0x06003E5E RID: 15966 RVA: 0x00194F28 File Offset: 0x00193128
		private double AdditionalVelocityNeededForEscape_mps(OrbitalElementsState orbit, TINaturalSpaceObjectState orbitBarycenter, TINaturalSpaceObjectState relevantBarycenter, TIDateTime time)
		{
			CartesianState cartesianState;
			if (orbitBarycenter == relevantBarycenter)
			{
				cartesianState = orbit.ToCartesianStateAtTime(time.ExportTime(), orbitBarycenter.mass_kg);
			}
			else if (orbitBarycenter.barycenter == relevantBarycenter)
			{
				cartesianState = orbitBarycenter.ToLocalCartesianStateAtTime(time);
			}
			else
			{
				cartesianState = orbitBarycenter.barycenter.ToLocalCartesianStateAtTime(time);
			}
			return this.AdditionalVelocityNeededForEscape_mps(cartesianState, relevantBarycenter, time);
		}

		// Token: 0x06003E5F RID: 15967 RVA: 0x00194F88 File Offset: 0x00193188
		private double AdditionalVelocityNeededForEscape_mps(CartesianState relevantState, TINaturalSpaceObjectState relevantBarycenter, TIDateTime time)
		{
			double magnitude = relevantState.position.magnitude;
			double magnitude2 = relevantState.velocity.magnitude;
			return Mathd.Max(0.0, relevantBarycenter.localEscapeVelocity_mps(magnitude) - magnitude2);
		}

		// Token: 0x02000ED7 RID: 3799
		public enum InternalTransferType
		{
			// Token: 0x04005AB7 RID: 23223
			Lambert,
			// Token: 0x04005AB8 RID: 23224
			Torch,
			// Token: 0x04005AB9 RID: 23225
			OrbitPhasing
		}
	}
}
