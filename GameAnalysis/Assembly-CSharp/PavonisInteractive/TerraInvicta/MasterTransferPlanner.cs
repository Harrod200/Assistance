using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000792 RID: 1938
	public class MasterTransferPlanner : MonoBehaviour
	{
		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06003DD0 RID: 15824 RVA: 0x001853CC File Offset: 0x001835CC
		public static MasterTransferPlanner Instance
		{
			get
			{
				return MasterTransferPlanner.s_instance;
			}
		}

		// Token: 0x06003DD1 RID: 15825 RVA: 0x001853D3 File Offset: 0x001835D3
		public void Start()
		{
			MasterTransferPlanner.s_instance = this;
		}

		// Token: 0x06003DD2 RID: 15826 RVA: 0x001853DC File Offset: 0x001835DC
		public void LateUpdate()
		{
			if (MasterTransferPlanner.requestActive && MasterTransferPlanner.request.done)
			{
				MasterTransferPlanner.queue.Dequeue();
				MasterTransferPlanner.requestActive = false;
			}
			if (!MasterTransferPlanner.requestActive && MasterTransferPlanner.queue.Count > 0)
			{
				MasterTransferPlanner.queue.Peek();
				MasterTransferPlanner.requestActive = true;
			}
		}

		// Token: 0x06003DD3 RID: 15827 RVA: 0x00185434 File Offset: 0x00183634
		public static bool FleetQueuedForTrajectories(TISpaceFleetState fleet)
		{
			using (Queue<MasterTransferPlanner.TrajectoryQueue>.Enumerator enumerator = MasterTransferPlanner.queue.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.fleet == fleet)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003DD4 RID: 15828 RVA: 0x00185494 File Offset: 0x00183694
		private static double GetStartDuration_s(TISpaceObjectState source, TISpaceObjectState destination, double acceleration_mps2, double deltaV_mps, double min_Distance_m, TrajectoryModel trajectoryModel)
		{
			double magnitude = source.ToGlobalCartesianStateAtTime(TITimeState.Now()).velocity.magnitude;
			switch (trajectoryModel)
			{
			case TrajectoryModel.LinearPlaceholder:
			case TrajectoryModel.Torch:
			{
				double num = Mathd.Sqrt(2.0 * min_Distance_m / acceleration_mps2) - min_Distance_m / magnitude;
				double num2 = ((num < 86400.0) ? 7200.0 : 86400.0);
				if (num <= num2)
				{
					return num2;
				}
				return Mathd.Round(num / num2) * num2;
			}
			case TrajectoryModel.Impulse:
			{
				double num = min_Distance_m / (deltaV_mps + magnitude);
				double num2 = ((num < 86400.0) ? 10800.0 : 86400.0);
				if (num <= num2)
				{
					return num2;
				}
				return Mathd.Round(num / num2) * num2;
			}
			}
			return 0.0;
		}

		// Token: 0x06003DD5 RID: 15829 RVA: 0x0018556C File Offset: 0x0018376C
		public static double GetEstimatedTransferTime_s(TISpaceAssetState asset, TISpaceObjectState destination, double acceleration_mps2, double deltaV_mps, out bool impossible)
		{
			TISpaceGameState tispaceGameState = destination;
			if (destination.isNaturalSpaceObjectState || (destination.isHabState && destination.ref_hab.IsBase) || destination.isHabSiteState || destination.isRegionState)
			{
				tispaceGameState = destination.ref_naturalSpaceObject.orbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.semiMajorAxis_km);
			}
			if (destination.isSpaceFleetState && destination.ref_fleet.inTransfer && destination.ref_fleet.trajectory.destination != null)
			{
				tispaceGameState = destination.ref_fleet.trajectory.destination;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			ITransferTarget transferTarget;
			ITransferTarget transferTarget2;
			TISpaceObjectState.FindRelevantOrbitingObjectsForTransfer(asset, tispaceGameState, out tinaturalSpaceObjectState, out transferTarget, out transferTarget2);
			double num = transferTarget.common_a_m(tinaturalSpaceObjectState);
			double num2 = transferTarget2.common_a_m(tinaturalSpaceObjectState);
			double num3 = tinaturalSpaceObjectState.localEscapeVelocity_mps(num) * 2.0;
			double num4 = num3 / (2.0 * acceleration_mps2);
			TrajectoryModel trajectoryModel = TrajectoryModel.Impulse;
			if (asset.ref_orbit == destination.ref_orbit || Mathd.Approximately(transferTarget.common_a_m(tinaturalSpaceObjectState), transferTarget2.common_a_m(tinaturalSpaceObjectState)))
			{
				if (deltaV_mps > num3)
				{
					trajectoryModel = TrajectoryModel.Torch;
				}
			}
			else
			{
				double num5 = Mathd.Max(tinaturalSpaceObjectState.localAccelerationDueToGravity_ms2(num), tinaturalSpaceObjectState.localAccelerationDueToGravity_ms2(num2));
				if (acceleration_mps2 < num5)
				{
					TISpaceFleetState tispaceFleetState = asset as TISpaceFleetState;
					if (tispaceFleetState != null && !tispaceFleetState.transferAssigned)
					{
						trajectoryModel = TrajectoryModel.Microthrust;
						goto IL_01A1;
					}
				}
				double num6 = 1.0;
				if (tinaturalSpaceObjectState.isSun && num / 149597870700.0 > 9.0)
				{
					num6 = num / 149597870700.0;
				}
				if (deltaV_mps > num3 && num4 < 86400.0 * num6)
				{
					trajectoryModel = TrajectoryModel.Torch;
				}
			}
			IL_01A1:
			switch (trajectoryModel)
			{
			case TrajectoryModel.LinearPlaceholder:
			case TrajectoryModel.Torch:
				impossible = false;
				return (double)MasterTransferPlanner.GetLinearDuration_s(asset, tispaceGameState.ref_spaceObject, acceleration_mps2, deltaV_mps);
			default:
				impossible = TISpaceObjectState.GenericTransferDeltaV_mps(asset, tispaceGameState, false) > deltaV_mps;
				return TISpaceObjectState.GenericTransferTime_s(asset.faction, asset, tispaceGameState);
			case TrajectoryModel.Microthrust:
			{
				if (acceleration_mps2 <= 0.0 || deltaV_mps <= 0.0)
				{
					impossible = true;
					return double.PositiveInfinity;
				}
				MicrothrustTransfer microthrustTransfer = new MicrothrustTransfer();
				TIDateTime tidateTime = (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(transferTarget2 as TISpaceFleetState, asset.faction) ? new TIDateTime((transferTarget2 as TISpaceFleetState).trajectory.arrivalTime, 1.0) : null);
				microthrustTransfer.Solve(TITimeState.Now(), transferTarget, transferTarget2, tinaturalSpaceObjectState, acceleration_mps2, tidateTime);
				impossible = microthrustTransfer.DV_mps > deltaV_mps;
				return microthrustTransfer.transitDuration_s;
			}
			}
		}

		// Token: 0x06003DD6 RID: 15830 RVA: 0x001857F4 File Offset: 0x001839F4
		private static float GetLinearDuration_s(TISpaceAssetState asset, TISpaceObjectState destination, double acceleration_mps2, double deltaV_mps)
		{
			double num = TISpaceObjectState.AverageDistanceBetweenTwoSpaceObjects_m(asset, destination);
			double num2 = deltaV_mps / 2.0;
			double num3 = num / 2.0;
			double num4 = num2 / acceleration_mps2;
			double num5 = Mathd.Min(Mathd.Sqrt(2.0 * num3 / acceleration_mps2), num4);
			double num6 = num5;
			double num7 = acceleration_mps2 * num5;
			double num8 = 0.5 * acceleration_mps2 * num5 * num5;
			double num9 = num8;
			double num10 = Mathd.Max(Mathd.Max(num - num8 - num9, 0.0) / ((num7 == 0.0) ? 1.0 : num7), 0.0);
			return (float)(num5 + num10 + num6);
		}

		// Token: 0x06003DD7 RID: 15831 RVA: 0x001858A4 File Offset: 0x00183AA4
		public static List<TrajectoryModel> GetTrajectoryModelsForConditions(IMobileAsset fleet, TIGameState destination, bool forcePlaceholder, double acc_mps2, double DV_mps, out TINaturalSpaceObjectState commonBarycenter, out ITransferTarget originValue, out ITransferTarget destinationValue)
		{
			List<TrajectoryModel> list = new List<TrajectoryModel>();
			originValue = fleet;
			ITransferTarget transferTarget2;
			if (!destination.isSpaceAssetState)
			{
				ITransferTarget transferTarget = destination.ref_orbit;
				transferTarget2 = transferTarget;
			}
			else
			{
				ITransferTarget transferTarget = destination.ref_spaceAsset;
				transferTarget2 = transferTarget;
			}
			destinationValue = transferTarget2;
			MasterTransferPlanner.SimplifiedPositions simplifiedPositions = MasterTransferPlanner.GetSimplifiedPositions(originValue, destinationValue, null, null);
			commonBarycenter = simplifiedPositions.commonBarycenter;
			if (!forcePlaceholder)
			{
				double originDistToCommonBarycenter_m = simplifiedPositions.originDistToCommonBarycenter_m;
				double destinationDistToCommonBarycenter_m = simplifiedPositions.destinationDistToCommonBarycenter_m;
				double num = commonBarycenter.localEscapeVelocity_mps(originDistToCommonBarycenter_m) * 2.0;
				double num2 = num / (2.0 * acc_mps2);
				if (fleet.ref_orbit != null && (fleet.ref_orbit == destination.ref_orbit || Mathd.Approximately(originDistToCommonBarycenter_m, destinationDistToCommonBarycenter_m)))
				{
					bool flag = 6.283185307179586 * Mathd.Sqrt(originDistToCommonBarycenter_m * originDistToCommonBarycenter_m * originDistToCommonBarycenter_m / commonBarycenter.mu) < 63113848.0;
					if (flag)
					{
						list.Add(TrajectoryModel.OrbitPhasing);
					}
					MicrothrustSphere microthrustSphere = new MicrothrustSphere((double)fleet.cruiseAcceleration_mps2, commonBarycenter.mu, commonBarycenter.sphereOfInfluence_m);
					if (!flag || microthrustSphere.Radius_m < originDistToCommonBarycenter_m || MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination as TISpaceFleetState, fleet.faction))
					{
						list.Add(TrajectoryModel.ImpulseMicrothrustHybrid);
					}
				}
				else
				{
					list.Add(TrajectoryModel.ImpulseMicrothrustHybrid);
					if (fleet.ref_orbit != null && destination.ref_orbit != null && fleet.ref_orbit.barycenter == destination.ref_orbit.barycenter && (Mathd.Abs(fleet.ref_orbit.inclination_Rad - destination.ref_orbit.inclination_Rad) > 0.1 || fleet.ref_orbit.inclination_Rad * Mathd.Abs(fleet.ref_orbit.longitudeAscendingNode_Rad - destination.ref_orbit.longitudeAscendingNode_Rad) > 0.05))
					{
						list.Add(TrajectoryModel.InclinationChange);
					}
				}
				TISpaceFleetState tispaceFleetState = fleet as TISpaceFleetState;
				if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination as TISpaceFleetState, fleet.faction) && (!fleet.transferAssigned || !(tispaceFleetState != null) || !(tispaceFleetState.trajectory.launchTime < TITimeState.Now())) && ((MasterTransferPlanner.DoesOrbitMatch(fleet, destination.ref_fleet.trajectory.originOrbit) && destination.ref_fleet.trajectory.launchTime > TITimeState.Now()) || MasterTransferPlanner.DoesOrbitMatch(fleet, destination.ref_fleet.trajectory.destinationOrbit)) && !list.Contains(TrajectoryModel.OrbitPhasing) && !destination.ref_fleet.barycenter.isLagrangePointState)
				{
					new MicrothrustSphere((double)fleet.cruiseAcceleration_mps2, fleet.ref_orbit.barycenter.mu, fleet.ref_orbit.barycenter.sphereOfInfluence_m);
					list.Add(TrajectoryModel.OrbitPhasing);
				}
				if (fleet.transferAssigned && tispaceFleetState != null && !list.Contains(TrajectoryModel.OrbitPhasing) && !tispaceFleetState.trajectory.destroyOnArrival && !tispaceFleetState.trajectory.destination.barycenter.isLagrangePointState)
				{
					if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination as TISpaceFleetState, fleet.faction))
					{
						if (destination.ref_fleet.trajectory.destination.ref_orbit == tispaceFleetState.trajectory.destination.ref_orbit)
						{
							list.Add(TrajectoryModel.OrbitPhasing);
						}
					}
					else if (destination.ref_orbit == tispaceFleetState.trajectory.destination.ref_orbit)
					{
						list.Add(TrajectoryModel.OrbitPhasing);
					}
				}
				if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination.ref_fleet, fleet.faction))
				{
					TIOrbitState tiorbitState = destination.ref_fleet.trajectory.destination as TIOrbitState;
					if (tiorbitState != null && !MasterTransferPlanner.DoesOrbitMatch(fleet, destination.ref_fleet.trajectory.destinationOrbit) && destination.ref_fleet.trajectory.destination.barycenter == fleet.barycenter())
					{
						TINaturalSpaceObjectState tinaturalSpaceObjectState = fleet.barycenter();
						MicrothrustSphere microthrustSphere2 = new MicrothrustSphere((double)fleet.cruiseAcceleration_mps2, tinaturalSpaceObjectState.mu, tinaturalSpaceObjectState.sphereOfInfluence_m);
						if (tiorbitState.semiMajorAxis_m < microthrustSphere2.Radius_m)
						{
							TIOrbitState ref_orbit = fleet.ref_orbit;
							double? num3 = ((ref_orbit != null) ? new double?(ref_orbit.semiMajorAxis_m) : null);
							double radius_m = microthrustSphere2.Radius_m;
							if (((num3.GetValueOrDefault() < radius_m) & (num3 != null)) && !list.Contains(TrajectoryModel.Microthrust))
							{
								list.Add(TrajectoryModel.Microthrust);
							}
						}
					}
				}
				if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination.ref_fleet, fleet.faction) && !list.Contains(TrajectoryModel.ImpulseMicrothrustHybrid))
				{
					list.Add(TrajectoryModel.ImpulseMicrothrustHybrid);
				}
			}
			else
			{
				list.Add(TrajectoryModel.LinearPlaceholder);
			}
			return list;
		}

		// Token: 0x06003DD8 RID: 15832 RVA: 0x00185D48 File Offset: 0x00183F48
		private static bool DoesOrbitMatch(IMobileAsset origin, TIOrbitState destination)
		{
			return origin != null && !(destination == null) && (origin.barycenter() == destination.barycenter && Mathd.Approximately(origin.a_m(), destination.semiMajorAxis_m) && Mathd.Approximately(origin.i_rad(), destination.inclination_Rad) && (origin.i_rad() == 0.0 || Mathd.Approximately(origin.Ω_rad(), destination.longitudeAscendingNode_Rad)) && Mathd.Approximately(origin.e(), destination.eccentricity)) && (origin.e() == 0.0 || Mathd.Approximately(origin.Ω_rad() + origin.ω_rad(), destination.longitudeAscendingNode_Rad + destination.argPeriapsis_Rad));
		}

		// Token: 0x06003DD9 RID: 15833 RVA: 0x00185E0C File Offset: 0x0018400C
		private static bool IsInMicrothrustDomain(ITransferTarget target, double fleetAcceleration_mps2, TINaturalSpaceObjectState commonBarycenter, TIFactionState ourFaction)
		{
			TISpaceFleetState tispaceFleetState = target as TISpaceFleetState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState2;
			double num;
			if (tispaceFleetState != null && MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, ourFaction))
			{
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				double distFromBarycenterAtTime_m = tispaceFleetState.trajectory.getDistFromBarycenterAtTime_m(tispaceFleetState.trajectory.arrivalTime, out tinaturalSpaceObjectState);
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState.mu, tinaturalSpaceObjectState.sphereOfInfluence_m);
				if (distFromBarycenterAtTime_m < microthrustSphere.Radius_m)
				{
					return true;
				}
				TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
				if (barycenter != null)
				{
					MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, barycenter.mu, barycenter.sphereOfInfluence_m);
					if (tinaturalSpaceObjectState.semiMajorAxis_m < microthrustSphere2.Radius_m)
					{
						return true;
					}
					TINaturalSpaceObjectState barycenter2 = barycenter.barycenter;
					if (barycenter2 != null)
					{
						MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(fleetAcceleration_mps2, barycenter2.mu, barycenter2.sphereOfInfluence_m);
						if (barycenter.semiMajorAxis_m < microthrustSphere3.Radius_m)
						{
							return true;
						}
					}
				}
				num = tispaceFleetState.trajectory.getDistFromBarycenterAtTime_m(TITimeState.Now(), out tinaturalSpaceObjectState2);
			}
			else
			{
				tinaturalSpaceObjectState2 = target.barycenter();
				num = target.a_m();
			}
			MicrothrustSphere microthrustSphere4 = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState2.mu, tinaturalSpaceObjectState2.sphereOfInfluence_m);
			if (num < microthrustSphere4.Radius_m)
			{
				return true;
			}
			if (tinaturalSpaceObjectState2 == commonBarycenter || tinaturalSpaceObjectState2.barycenter == null)
			{
				return false;
			}
			MicrothrustSphere microthrustSphere5 = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState2.barycenter.mu, tinaturalSpaceObjectState2.barycenter.sphereOfInfluence_m);
			if (tinaturalSpaceObjectState2.semiMajorAxis_m < microthrustSphere5.Radius_m)
			{
				return true;
			}
			if (tinaturalSpaceObjectState2.barycenter == commonBarycenter || tinaturalSpaceObjectState2.barycenter.barycenter == null)
			{
				return false;
			}
			MicrothrustSphere microthrustSphere6 = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState2.barycenter.barycenter.mu, tinaturalSpaceObjectState2.barycenter.barycenter.sphereOfInfluence_m);
			return tinaturalSpaceObjectState2.barycenter.semiMajorAxis_m < microthrustSphere6.Radius_m;
		}

		// Token: 0x06003DDA RID: 15834 RVA: 0x00185FCC File Offset: 0x001841CC
		public static TransferResult RequestTrajectories(IMobileAsset fleet, TIGameState destination, int requestSize, Action<Trajectory[]> callback, out double lowestDVFound_kps, bool usePlaceholderTrajectories = false, bool stopOnFirstSuccess = false, double sampleSizeMultiplier = 1.0)
		{
			TISpaceFleetState tispaceFleetState = destination as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.GetFleetsWeAreIntercepting(false).Contains(fleet))
			{
				lowestDVFound_kps = -1.0;
				return new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptThatWouldCauseTargetingLoop, 0.0, 0.0);
			}
			if (destination is TIOrbitState && !fleet.transferAssigned && fleet.ref_orbit == destination)
			{
				Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
				lowestDVFound_kps = 0.0;
				if (fleet.ref_orbit == null)
				{
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				trajectory_Patched.BuildEmptyTrajectory(fleet, TITimeState.Now(), null);
				Trajectory[] array = new Trajectory[] { trajectory_Patched };
				callback(array);
				return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
			}
			else
			{
				if (!(destination is TIOrbitState))
				{
					TISpaceAssetState tispaceAssetState = destination as TISpaceAssetState;
					if (tispaceAssetState != null && !MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination as TISpaceFleetState, fleet.faction) && !fleet.transferAssigned)
					{
						OrbitalElementsState orbitalElementsState;
						TINaturalSpaceObjectState tinaturalSpaceObjectState;
						bool flag;
						tispaceAssetState.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
						OrbitalElementsState orbitalElementsState2;
						TINaturalSpaceObjectState tinaturalSpaceObjectState2;
						bool flag2;
						fleet.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag2);
						double magnitude = (fleet.ToGlobalCartesianStateAtTime(TITimeState.Now()).position - tispaceAssetState.ToGlobalCartesianStateAtTime(TITimeState.Now()).position).magnitude;
						if (tinaturalSpaceObjectState2 == tinaturalSpaceObjectState && orbitalElementsState2.Approximately(orbitalElementsState, 0.0) && magnitude < 5000.0)
						{
							Trajectory_Patched trajectory_Patched2 = new Trajectory_Patched();
							lowestDVFound_kps = 0.0;
							if (fleet.ref_orbit == null)
							{
								return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
							}
							trajectory_Patched2.BuildEmptyTrajectory(fleet, TITimeState.Now(), tispaceAssetState);
							Trajectory[] array2 = new Trajectory[] { trajectory_Patched2 };
							callback(array2);
							return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
						}
					}
				}
				TIDateTime tidateTime = null;
				bool flag3 = false;
				lowestDVFound_kps = double.PositiveInfinity;
				TransferResult transferResult = null;
				bool flag4 = false;
				StreamWriter streamWriter = null;
				TISpaceGameState tispaceGameState;
				if (destination.isNaturalSpaceObjectState || (destination.isHabState && destination.ref_hab.IsBase) || destination.isHabSiteState || destination.isRegionState)
				{
					tispaceGameState = destination.ref_naturalSpaceObject.orbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.semiMajorAxis_km);
				}
				else
				{
					tispaceGameState = destination as TISpaceGameState;
				}
				float cruiseAcceleration_mps = fleet.cruiseAcceleration_mps2;
				float currentDeltaV_mps = fleet.currentDeltaV_mps;
				double num = TISpaceObjectState.MinDistanceBetweenTwoSpaceObjects_m(fleet, tispaceGameState.ref_spaceObject);
				TINaturalSpaceObjectState tinaturalSpaceObjectState3;
				ITransferTarget transferTarget;
				ITransferTarget transferTarget2;
				List<TrajectoryModel> trajectoryModelsForConditions = MasterTransferPlanner.GetTrajectoryModelsForConditions(fleet, tispaceGameState, usePlaceholderTrajectories, (double)cruiseAcceleration_mps, (double)currentDeltaV_mps, out tinaturalSpaceObjectState3, out transferTarget, out transferTarget2);
				if (tispaceGameState.isSpaceFleetState && tispaceGameState.ref_fleet.inTransfer)
				{
					tidateTime = tispaceGameState.ref_fleet.trajectory.arrivalTime;
					flag3 = true;
				}
				TIDateTime tidateTime2 = TITimeState.Now();
				List<Trajectory> list = new List<Trajectory>();
				foreach (TrajectoryModel trajectoryModel in trajectoryModelsForConditions)
				{
					double startDuration_s = MasterTransferPlanner.GetStartDuration_s(fleet.barycenter(), tispaceGameState.barycenter, (double)cruiseAcceleration_mps, (double)currentDeltaV_mps, num, trajectoryModel);
					switch (trajectoryModel)
					{
					case TrajectoryModel.Impulse:
						MasterTransferPlanner.CalculateImpulseTransfers(ref list, ref lowestDVFound_kps, requestSize, fleet, (double)currentDeltaV_mps, (double)cruiseAcceleration_mps, transferTarget, tispaceGameState, transferTarget2, tinaturalSpaceObjectState3, startDuration_s, flag3, tidateTime, false, stopOnFirstSuccess);
						break;
					case TrajectoryModel.Microthrust:
					{
						TransferResult transferResult2 = MasterTransferPlanner.CalculateMicrothrustTransfer(ref list, ref lowestDVFound_kps, fleet, (double)currentDeltaV_mps, (double)cruiseAcceleration_mps, transferTarget, tispaceGameState, transferTarget2, tinaturalSpaceObjectState3, tidateTime2);
						transferResult = TransferResult.Best(transferResult, transferResult2);
						break;
					}
					case TrajectoryModel.ImpulseMicrothrustHybrid:
					{
						MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params calculateImpulseMicrothrustHybridTransfer_Params = new MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params
						{
							requestSize = requestSize,
							sampleSizeMultiplier = sampleSizeMultiplier,
							fleet = fleet,
							fleetDeltaV_mps = (double)currentDeltaV_mps,
							fleetAcceleration_mps2 = (double)cruiseAcceleration_mps,
							originValue = transferTarget,
							sDestination = tispaceGameState,
							destinationValue = transferTarget2,
							commonBarycenter = tinaturalSpaceObjectState3,
							now = tidateTime2,
							log = streamWriter,
							stopOnFirstSuccess = stopOnFirstSuccess
						};
						TransferResult transferResult3 = MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfers(ref list, ref lowestDVFound_kps, calculateImpulseMicrothrustHybridTransfer_Params);
						transferResult = TransferResult.Best(transferResult, transferResult3);
						break;
					}
					case TrajectoryModel.Torch:
						MasterTransferPlanner.CalculateTorchTransfers(ref list, ref lowestDVFound_kps, requestSize, fleet, (double)currentDeltaV_mps, (double)cruiseAcceleration_mps, transferTarget, tispaceGameState, transferTarget2, tinaturalSpaceObjectState3, startDuration_s, flag3, tidateTime, stopOnFirstSuccess);
						break;
					case TrajectoryModel.OrbitPhasing:
					{
						int num2 = Mathd.CeilToInt((double)requestSize * sampleSizeMultiplier);
						TransferResult transferResult4 = MasterTransferPlanner.CalculateOrbitPhasingTransfers(ref list, ref lowestDVFound_kps, num2, fleet, (double)currentDeltaV_mps, (double)cruiseAcceleration_mps, transferTarget, tispaceGameState, transferTarget2, tinaturalSpaceObjectState3, stopOnFirstSuccess);
						transferResult = TransferResult.Best(transferResult, transferResult4);
						break;
					}
					case TrajectoryModel.InclinationChange:
					{
						TransferResult transferResult5 = MasterTransferPlanner.CalculateInclinationChangeTransfers(ref list, ref lowestDVFound_kps, sampleSizeMultiplier, fleet, (double)currentDeltaV_mps, (double)cruiseAcceleration_mps, transferTarget, tispaceGameState, transferTarget2, tinaturalSpaceObjectState3, stopOnFirstSuccess);
						transferResult = TransferResult.Best(transferResult, transferResult5);
						break;
					}
					}
					if (stopOnFirstSuccess && list.Count > 0)
					{
						break;
					}
				}
				if (double.IsInfinity(lowestDVFound_kps))
				{
					if (list.Count<Trajectory>() > 0)
					{
						lowestDVFound_kps = list.Min<Trajectory>((Trajectory x) => x.DV_kps);
					}
					else if (!transferResult.TryGetMinimumDVneeded_kps(out lowestDVFound_kps))
					{
						lowestDVFound_kps = -1.0;
					}
				}
				if (destination != null)
				{
					TISpaceFleetState tispaceFleetState2 = fleet as TISpaceFleetState;
					TIGameState tigameState;
					if (tispaceFleetState2 == null)
					{
						tigameState = null;
					}
					else
					{
						Trajectory trajectory4 = tispaceFleetState2.trajectory;
						tigameState = ((trajectory4 != null) ? trajectory4.destination : null);
					}
					if (tigameState == destination)
					{
						TISpaceFleetState tispaceFleetState3 = fleet as TISpaceFleetState;
						Trajectory_Patched trajectory_Patched3 = tispaceFleetState3.trajectory as Trajectory_Patched;
						Trajectory trajectory2 = Trajectory_Patched.BuildTruncatedTrajectory(tispaceFleetState3, trajectory_Patched3, TITimeState.Now());
						list.Add(trajectory2);
						transferResult = TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0));
					}
				}
				TISpaceFleetState sFleet = fleet as TISpaceFleetState;
				if (sFleet != null && sFleet.inTransfer)
				{
					list.ForEach(delegate(Trajectory x)
					{
						MasterTransferPlanner.AddRemnantsOfExistingTransfer(x as Trajectory_Patched, sFleet.trajectory as Trajectory_Patched);
					});
				}
				list.ForEach(delegate(Trajectory x)
				{
					Trajectory_Patched trajectory_Patched4 = x as Trajectory_Patched;
					if (trajectory_Patched4 == null)
					{
						return;
					}
					trajectory_Patched4.Segments.RemoveAll((Trajectory_Patched.IPatchSegment y) => y == null);
				});
				if (list.Count > 0)
				{
					if (destination.isSpaceFleetState)
					{
						TISpaceFleetState tispaceFleetState4 = destination as TISpaceFleetState;
						if (tispaceFleetState4 == null)
						{
							Debug.LogWarning("Destination said it was a space fleet, but it isn't.");
						}
						else if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState4, fleet.faction))
						{
							double num3 = double.PositiveInfinity;
							List<Trajectory> list2 = new List<Trajectory>();
							foreach (Trajectory trajectory3 in list)
							{
								double num4 = tispaceFleetState4.trajectory.RemainingDVatTime_mps(trajectory3.arrivalTime);
								trajectory3.DV_targetFleet_mps = num4;
								num3 = Mathd.Min(num3, trajectory3.DV_mps);
								if (trajectory3.DV_mps > (double)fleet.currentDeltaV_mps)
								{
									list2.Add(trajectory3);
								}
							}
							list = list.Except<Trajectory>(list2).ToList<Trajectory>();
							list.Count<Trajectory>();
							if (num3 != double.PositiveInfinity)
							{
								lowestDVFound_kps = num3 / 1000.0;
							}
							if (list.Count == 0)
							{
								transferResult = new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, num3, 0.0);
							}
						}
					}
					if (lowestDVFound_kps == double.PositiveInfinity)
					{
						lowestDVFound_kps = -1.0;
					}
					int count = list.Count;
					List<Trajectory> list3 = list.Where<Trajectory>((Trajectory trajectory) => trajectory.duration_s > MasterTransferPlanner.TransferDurationHardCap(fleet.faction)).ToList<Trajectory>();
					list = list.Except<Trajectory>(list3).ToList<Trajectory>();
					if (list.Count == 0 && count > 0)
					{
						transferResult = new TransferResult(TransferResult.Outcome.Fail_ExceedsMaxDuration, MasterTransferPlanner.TransferDurationHardCap(fleet.faction), 0.0);
					}
					list3 = new List<Trajectory>();
					for (int i = 0; i < list.Count; i++)
					{
						for (int j = 0; j < list.Count; j++)
						{
							if (i != j)
							{
								if (Mathd.Approximately(list[i].DV_mps, list[j].DV_mps) && Mathd.Approximately(list[i].arrivalTime.ToJulianDateInSeconds(), list[j].arrivalTime.ToJulianDateInSeconds()))
								{
									if (i > j)
									{
										list3.Add(list[i]);
										break;
									}
								}
								else if (list[i].DV_mps >= list[j].DV_mps && list[i].arrivalTime >= list[j].arrivalTime)
								{
									list3.Add(list[i]);
									break;
								}
							}
						}
					}
					list = list.Except<Trajectory>(list3).ToList<Trajectory>();
				}
				if (list.Count > 0)
				{
					flag4 = true;
				}
				callback(list.ToArray());
				if (transferResult == null)
				{
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				if (transferResult.Result == TransferResult.Outcome.Success && !flag4)
				{
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				return transferResult;
			}
		}

		// Token: 0x06003DDB RID: 15835 RVA: 0x001869D4 File Offset: 0x00184BD4
		private static double microthrustDelays(TINaturalSpaceObjectState barycenter, double semiMajorAxis_m, double fleetAcceleration_mps2)
		{
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, barycenter.mu, barycenter.sphereOfInfluence_m);
			double num = Mathd.Sqrt(barycenter.mu / semiMajorAxis_m);
			return microthrustSphere.GetDuration_s(num);
		}

		// Token: 0x06003DDC RID: 15836 RVA: 0x00186A07 File Offset: 0x00184C07
		public static bool DoWeKnowThatFleetIsTransfering(TISpaceFleetState fleet, TIFactionState ourFaction)
		{
			return !(fleet == null) && fleet.DoIKnowThisFleetIsTransfering(ourFaction);
		}

		// Token: 0x06003DDD RID: 15837 RVA: 0x00186A1B File Offset: 0x00184C1B
		protected static void AddRemnantsOfExistingTransfer(Trajectory_Patched trajectory, Trajectory_Patched oldTrajectory)
		{
			if (trajectory == null || oldTrajectory == null)
			{
				return;
			}
			trajectory.AddRemnantsOfExistingTransfer(oldTrajectory);
		}

		// Token: 0x06003DDE RID: 15838 RVA: 0x00186A2C File Offset: 0x00184C2C
		private static TransferResult CalculateImpulseMicrothrustHybridTransfers(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param)
		{
			MasterTransferPlanner.<>c__DisplayClass32_0 CS$<>8__locals1 = new MasterTransferPlanner.<>c__DisplayClass32_0();
			CS$<>8__locals1.param = param;
			int count = candidateTrajectories.Count;
			CS$<>8__locals1.isDestinationOrbit = CS$<>8__locals1.param.destinationValue is TIOrbitState;
			CS$<>8__locals1.targetFleet = null;
			CS$<>8__locals1.originFleet = CS$<>8__locals1.param.fleet as TISpaceFleetState;
			CS$<>8__locals1.destinationIsTransferingFleet = false;
			CS$<>8__locals1.originFleetIsInTransfer = false;
			CS$<>8__locals1.originFleetTrajectory = null;
			CS$<>8__locals1.isPlayer = CS$<>8__locals1.param.fleet.faction.isActivePlayer;
			TISpaceFleetState tispaceFleetState = CS$<>8__locals1.param.destinationValue as TISpaceFleetState;
			TISpaceFleetState originFleet = CS$<>8__locals1.originFleet;
			if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, (originFleet != null) ? originFleet.faction : null))
			{
				CS$<>8__locals1.targetFleet = CS$<>8__locals1.param.destinationValue as TISpaceFleetState;
				CS$<>8__locals1.destinationIsTransferingFleet = true;
			}
			if (CS$<>8__locals1.originFleet != null && CS$<>8__locals1.originFleet.inTransfer)
			{
				CS$<>8__locals1.originFleetIsInTransfer = true;
				CS$<>8__locals1.originFleetTrajectory = CS$<>8__locals1.originFleet.trajectory;
			}
			TISpaceFleetState tispaceFleetState2 = CS$<>8__locals1.param.fleet as TISpaceFleetState;
			TINaturalSpaceObjectState commonBarycenter;
			if (tispaceFleetState2 != null && tispaceFleetState2.transferAssigned && tispaceFleetState2.trajectory.launchTime <= CS$<>8__locals1.param.now)
			{
				MasterTransferPlanner.SimplifiedPositions simplifiedPositions = MasterTransferPlanner.GetSimplifiedPositions(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.now, null);
				TINaturalSpaceObjectState originLocalBarycenter = simplifiedPositions.originLocalBarycenter;
				double originDistToLocalBarycenter_m = simplifiedPositions.originDistToLocalBarycenter_m;
				commonBarycenter = simplifiedPositions.commonBarycenter;
				double num = 0.0;
				double radius_m = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, commonBarycenter.mu, commonBarycenter.sphereOfInfluence_m).Radius_m;
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, simplifiedPositions.destinationLocalBarycenter.mu, simplifiedPositions.destinationLocalBarycenter.sphereOfInfluence_m);
				if (!CS$<>8__locals1.destinationIsTransferingFleet)
				{
					if (simplifiedPositions.destinationDistToLocalBarycenter_m < microthrustSphere.Radius_m)
					{
						double num2 = Mathd.Sqrt(simplifiedPositions.destinationLocalBarycenter.mu / simplifiedPositions.destinationDistToLocalBarycenter_m);
						num = microthrustSphere.GetDuration_s(num2);
					}
					if (simplifiedPositions.destinationLocalBarycenter != simplifiedPositions.commonBarycenter)
					{
						MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, simplifiedPositions.destinationLocalBarycenter.barycenter.mu, simplifiedPositions.destinationLocalBarycenter.barycenter.sphereOfInfluence_m);
						if (simplifiedPositions.destinationLocalBarycenter.semiMajorAxis_m < microthrustSphere2.Radius_m)
						{
							double num3 = Mathd.Sqrt(simplifiedPositions.destinationLocalBarycenter.barycenter.mu / simplifiedPositions.destinationLocalBarycenter.semiMajorAxis_m);
							num += microthrustSphere2.GetDuration_s(num3);
						}
						if (simplifiedPositions.destinationLocalBarycenter.barycenter != simplifiedPositions.commonBarycenter)
						{
							MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, simplifiedPositions.commonBarycenter.mu, simplifiedPositions.commonBarycenter.semiMajorAxis_m);
							if (simplifiedPositions.destinationLocalBarycenter.barycenter.semiMajorAxis_m < microthrustSphere3.Radius_m)
							{
								double num4 = Mathd.Sqrt(simplifiedPositions.commonBarycenter.mu / simplifiedPositions.destinationLocalBarycenter.barycenter.semiMajorAxis_m);
								num += microthrustSphere3.GetDuration_s(num4);
							}
						}
					}
				}
				CS$<>8__locals1.hybridTransferType = new MasterTransferPlanner.IdentifyHybridTransferType_Result
				{
					isMicrothrustOnly = false,
					isGoingOut = (simplifiedPositions.originDistToCommonBarycenter_m < simplifiedPositions.destinationDistToCommonBarycenter_m),
					outspiralDuration_s = 0.0,
					inspiralDuration_s = num,
					commonMicrothrustRadius_m = radius_m
				};
			}
			else if (CS$<>8__locals1.destinationIsTransferingFleet)
			{
				TIDateTime tidateTime = new TIDateTime(CS$<>8__locals1.targetFleet.trajectory.arrivalTime, 1.0);
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				double num5 = CS$<>8__locals1.targetFleet.trajectory.getDistFromBarycenterAtTime_m(tidateTime, out tinaturalSpaceObjectState);
				if (CS$<>8__locals1.targetFleet.trajectory.exitsSolarSystem)
				{
					num5 = double.PositiveInfinity;
				}
				if (tinaturalSpaceObjectState == CS$<>8__locals1.param.originValue.barycenter())
				{
					if (CS$<>8__locals1.param.now >= CS$<>8__locals1.targetFleet.trajectory.launchTime)
					{
						num5 = CS$<>8__locals1.targetFleet.trajectory.getDistFromBarycenterAtTime_m(CS$<>8__locals1.param.now, out tinaturalSpaceObjectState);
					}
					else
					{
						tinaturalSpaceObjectState = CS$<>8__locals1.param.destinationValue.barycenter();
						num5 = CS$<>8__locals1.param.destinationValue.a_m();
					}
				}
				TINaturalSpaceObjectState tinaturalSpaceObjectState2 = CS$<>8__locals1.param.originValue.barycenter().FindCommonBarycenter(tinaturalSpaceObjectState);
				CS$<>8__locals1.hybridTransferType = MasterTransferPlanner.IdentifyHybridTransferType(CS$<>8__locals1.param.originValue.a_m(), CS$<>8__locals1.param.originValue.barycenter(), num5, tinaturalSpaceObjectState, tinaturalSpaceObjectState2, CS$<>8__locals1.param.fleetAcceleration_mps2);
			}
			else
			{
				TINaturalSpaceObjectState tinaturalSpaceObjectState3 = CS$<>8__locals1.param.originValue.barycenter().FindCommonBarycenter(CS$<>8__locals1.param.destinationValue.barycenter());
				CS$<>8__locals1.hybridTransferType = MasterTransferPlanner.IdentifyHybridTransferType(CS$<>8__locals1.param.originValue.a_m(), CS$<>8__locals1.param.originValue.barycenter(), CS$<>8__locals1.param.destinationValue.a_m(), CS$<>8__locals1.param.destinationValue.barycenter(), tinaturalSpaceObjectState3, CS$<>8__locals1.param.fleetAcceleration_mps2);
			}
			if (CS$<>8__locals1.hybridTransferType.totalMicrothrustDuration_s > 78892310.0)
			{
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, CS$<>8__locals1.hybridTransferType.totalMicrothrustDuration_s, 78892310.0);
			}
			if (CS$<>8__locals1.hybridTransferType.isMicrothrustOnly && !CS$<>8__locals1.destinationIsTransferingFleet && !CS$<>8__locals1.originFleetIsInTransfer)
			{
				TransferResult transferResult = new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				double totalMicrothrustDuration_s = CS$<>8__locals1.hybridTransferType.totalMicrothrustDuration_s;
				if (CS$<>8__locals1.param.commonBarycenter == CS$<>8__locals1.param.originValue.barycenter() && CS$<>8__locals1.param.commonBarycenter == CS$<>8__locals1.param.destinationValue.barycenter())
				{
					if ((CS$<>8__locals1.param.originValue.i_rad() < 0.01 && CS$<>8__locals1.param.destinationValue.i_rad() < 0.01) || (Mathd.Abs(CS$<>8__locals1.param.originValue.i_rad() - CS$<>8__locals1.param.destinationValue.i_rad()) < 0.01 && Mathd.Abs(CS$<>8__locals1.param.originValue.Ω_rad() - CS$<>8__locals1.param.originValue.Ω_rad()) < 0.01))
					{
						TIDateTime tidateTime2 = CS$<>8__locals1.param.now;
						if (!CS$<>8__locals1.isDestinationOrbit && !Mathd.Approximately(CS$<>8__locals1.param.originValue.a_m(), CS$<>8__locals1.param.destinationValue.a_m()))
						{
							OrbitalElementsState orbitalElementsState;
							TINaturalSpaceObjectState tinaturalSpaceObjectState4;
							bool flag;
							MasterTransferPlanner.GetOriginOrbitalElementsState(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.now, out orbitalElementsState, out tinaturalSpaceObjectState4, out flag);
							OrbitalElementsState orbitalElementsState2;
							CS$<>8__locals1.param.destinationValue.getOrbitalElementsState(CS$<>8__locals1.param.now, out orbitalElementsState2, out tinaturalSpaceObjectState4, out flag);
							MicrothrustSphere microthrustSphere4 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.param.commonBarycenter.mu, CS$<>8__locals1.param.commonBarycenter.sphereOfInfluence_m);
							double num6 = Mathd.Sqrt(CS$<>8__locals1.param.commonBarycenter.mu / orbitalElementsState.semiMajorAxis_m);
							double num7 = Mathd.Sqrt(CS$<>8__locals1.param.commonBarycenter.mu / orbitalElementsState2.semiMajorAxis_m);
							double num8 = Mathd.Abs(microthrustSphere4.GetAnomalyDelta_Rad(num7) - microthrustSphere4.GetAnomalyDelta_Rad(num6));
							double num9 = Mathd.Abs(microthrustSphere4.GetDuration_s(num7) - microthrustSphere4.GetDuration_s(num6));
							if (double.IsInfinity(num9) || double.IsNaN(num9) || num9 > 78892310.0)
							{
								return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, num9, 78892310.0);
							}
							TIDateTime tidateTime3 = new TIDateTime(CS$<>8__locals1.param.now, num9);
							double num10 = orbitalElementsState.longAscendingNode_Rad + orbitalElementsState.argPeriapsis_Rad + orbitalElementsState.MeanAnomalyAtTime_Rad(CS$<>8__locals1.param.now.ExportTime(), CS$<>8__locals1.param.commonBarycenter.mass_kg);
							double num11 = orbitalElementsState2.longAscendingNode_Rad + orbitalElementsState2.argPeriapsis_Rad + orbitalElementsState2.MeanAnomalyAtTime_Rad(tidateTime3.ExportTime(), CS$<>8__locals1.param.commonBarycenter.mass_kg);
							double num12 = Mathd.ClampRadiansTwoPI(num10 + num8 - num11);
							double num13 = 6.283185307179586 / orbitalElementsState.OrbitalPeriod(CS$<>8__locals1.param.commonBarycenter.mass_kg);
							double num14 = 6.283185307179586 / orbitalElementsState2.OrbitalPeriod(CS$<>8__locals1.param.commonBarycenter.mass_kg) - num13;
							if (num14 < 0.0)
							{
								num12 = 6.283185307179586 - num12;
								num14 = -num14;
							}
							double num15 = num12 / num14;
							if (double.IsInfinity(num15) || double.IsNaN(num15) || num15 > 300000000.0)
							{
								Log.Error("Single-barycenter transfer to a fleet with a hyperbolic trajectory that claims to not be in a transfer.  We are failing out in order to avoid a CTD.", Array.Empty<object>());
								CS$<>8__locals1.targetFleet = CS$<>8__locals1.param.destinationValue as TISpaceFleetState;
								if (CS$<>8__locals1.targetFleet == null)
								{
									return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
								}
								CS$<>8__locals1.destinationIsTransferingFleet = true;
								tidateTime2 = CS$<>8__locals1.param.now;
							}
							else
							{
								tidateTime2 = new TIDateTime(CS$<>8__locals1.param.now, num15);
							}
						}
						if (!CS$<>8__locals1.destinationIsTransferingFleet)
						{
							MicrothrustTransfer microthrustTransfer = new MicrothrustTransfer();
							microthrustTransfer.Solve(tidateTime2, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, null);
							TIDateTime launchTime = microthrustTransfer.launchTime;
							TIDateTime arrivalTime15 = microthrustTransfer.arrivalTime;
							TINaturalSpaceObjectState tinaturalSpaceObjectState4;
							bool flag;
							OrbitalElementsState orbitalElementsState3;
							CS$<>8__locals1.param.fleet.getOrbitalElementsState(launchTime, out orbitalElementsState3, out tinaturalSpaceObjectState4, out flag);
							ITransferTarget destinationValue = CS$<>8__locals1.param.destinationValue;
							TISpaceFleetState originFleet2 = CS$<>8__locals1.originFleet;
							Trajectory.GetDestinationLocalOrbitalElementsAtTime(destinationValue, (originFleet2 != null) ? originFleet2.faction : null, arrivalTime15, null, 0.0);
							lowestDVFound_kps = Mathd.Min(lowestDVFound_kps, microthrustTransfer.DV_mps / 1000.0);
							if (microthrustTransfer.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
							{
								return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, microthrustTransfer.DV_mps, 0.0);
							}
							Trajectory_Microthrust trajectory_Microthrust = new Trajectory_Microthrust();
							trajectory_Microthrust.BuildSingleTrajectory(CS$<>8__locals1.param.fleet, CS$<>8__locals1.param.sDestination, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.commonBarycenter, microthrustTransfer, CS$<>8__locals1.param.fleetAcceleration_mps2);
							if (trajectory_Microthrust.DV_kps < lowestDVFound_kps)
							{
								lowestDVFound_kps = trajectory_Microthrust.DV_kps;
							}
							if (trajectory_Microthrust.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
							{
								return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, trajectory_Microthrust.DV_mps, 0.0);
							}
							candidateTrajectories.Add(trajectory_Microthrust);
							StreamWriter log = CS$<>8__locals1.param.log;
							if (log != null)
							{
								string[] array = new string[12];
								array[0] = "SUCCESS,MicrothrustOnlyNoTransferingFleetSingleBarycenterNoInclinationChange,";
								int num16 = 1;
								TIDateTime launchTime2 = microthrustTransfer.launchTime;
								array[num16] = ((launchTime2 != null) ? launchTime2.ToString() : null);
								array[2] = ",";
								int num17 = 3;
								TIDateTime arrivalTime2 = microthrustTransfer.arrivalTime;
								array[num17] = ((arrivalTime2 != null) ? arrivalTime2.ToString() : null);
								array[4] = ",";
								array[5] = microthrustTransfer.DV_mps.ToString();
								array[6] = ",";
								int num18 = 7;
								TIDateTime launchTime3 = trajectory_Microthrust.launchTime;
								array[num18] = ((launchTime3 != null) ? launchTime3.ToString() : null);
								array[8] = ",";
								int num19 = 9;
								TIDateTime arrivalTime3 = trajectory_Microthrust.arrivalTime;
								array[num19] = ((arrivalTime3 != null) ? arrivalTime3.ToString() : null);
								array[10] = ",";
								array[11] = trajectory_Microthrust.DV_mps.ToString();
								log.WriteLine(string.Concat(array));
							}
						}
					}
					else
					{
						TIDateTime tidateTime4 = new TIDateTime(CS$<>8__locals1.param.now);
						double num20 = CS$<>8__locals1.param.destinationValue.period_days() * 86400.0 * 0.5;
						MicrothrustSphere microthrustSphere5 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.param.commonBarycenter.mu, CS$<>8__locals1.param.commonBarycenter.sphereOfInfluence_m);
						double num21 = Mathd.Sqrt(CS$<>8__locals1.param.commonBarycenter.mu / CS$<>8__locals1.param.originValue.a_m());
						double num22 = Mathd.Sqrt(CS$<>8__locals1.param.commonBarycenter.mu / CS$<>8__locals1.param.destinationValue.a_m());
						double num23 = Mathd.Abs(microthrustSphere5.GetDuration_s(num21) - microthrustSphere5.GetDuration_s(num22));
						if (double.IsInfinity(num23) || double.IsNaN(num23) || num23 > 78892310.0)
						{
							return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, num23, 78892310.0);
						}
						TIDateTime tidateTime5 = new TIDateTime(CS$<>8__locals1.param.now, num23);
						bool isPlayer = CS$<>8__locals1.isPlayer;
						tidateTime5.AddSeconds(num20);
						ITransferTarget destinationValue2 = CS$<>8__locals1.param.destinationValue;
						TISpaceFleetState originFleet3 = CS$<>8__locals1.originFleet;
						ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool> destinationLocalOrbitalElementsAtTime = Trajectory.GetDestinationLocalOrbitalElementsAtTime(destinationValue2, (originFleet3 != null) ? originFleet3.faction : null, tidateTime5, CS$<>8__locals1.param.now, 0.0);
						OrbitalElementsState item = destinationLocalOrbitalElementsAtTime.Item1;
						TINaturalSpaceObjectState item2 = destinationLocalOrbitalElementsAtTime.Item2;
						if (item2 != CS$<>8__locals1.param.commonBarycenter)
						{
							if (item2.barycenter == CS$<>8__locals1.param.commonBarycenter)
							{
								item = new OrbitalElementsState(item2);
							}
							else
							{
								item = new OrbitalElementsState(item2.barycenter);
							}
						}
						PatchedTransfer patchedTransfer = new PatchedTransfer();
						PatchedTransfer patchedTransfer2 = patchedTransfer;
						TIDateTime tidateTime6 = tidateTime4;
						TIDateTime tidateTime7 = tidateTime5;
						ITransferTarget originValue = CS$<>8__locals1.param.originValue;
						OrbitalElementsState orbitalElementsState4 = item;
						TINaturalSpaceObjectState commonBarycenter2 = CS$<>8__locals1.param.commonBarycenter;
						TINaturalSpaceObjectState commonBarycenter3 = CS$<>8__locals1.param.commonBarycenter;
						double fleetAcceleration_mps = CS$<>8__locals1.param.fleetAcceleration_mps2;
						bool isDestinationOrbit = CS$<>8__locals1.isDestinationOrbit;
						PatchedTransfer.InternalTransferType internalTransferType = PatchedTransfer.InternalTransferType.Lambert;
						TISpaceFleetState targetFleet = CS$<>8__locals1.targetFleet;
						TIDateTime tidateTime8;
						if (targetFleet == null)
						{
							tidateTime8 = null;
						}
						else
						{
							Trajectory trajectory = targetFleet.trajectory;
							tidateTime8 = ((trajectory != null) ? trajectory.arrivalTime : null);
						}
						TransferResult transferResult2 = patchedTransfer2.Solve(tidateTime6, tidateTime7, originValue, orbitalElementsState4, commonBarycenter2, commonBarycenter3, fleetAcceleration_mps, isDestinationOrbit, internalTransferType, tidateTime8);
						if (transferResult2.Result != TransferResult.Outcome.Success)
						{
							return transferResult2;
						}
						double dv_mps = patchedTransfer.DV_mps;
						bool flag2 = true;
						if (patchedTransfer.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
						{
							StreamWriter log2 = CS$<>8__locals1.param.log;
							if (log2 != null)
							{
								string[] array2 = new string[10];
								array2[0] = "FAIL: not enough DV has ";
								array2[1] = CS$<>8__locals1.param.fleetDeltaV_mps.ToString();
								array2[2] = "m/s needs ";
								array2[3] = patchedTransfer.DV_mps.ToString();
								array2[4] = "m/s,MicrothrustOnlyNoTransferingFleetSingleBarycenterInclinationChange,";
								int num24 = 5;
								TIDateTime launchTime4 = patchedTransfer.launchTime;
								array2[num24] = ((launchTime4 != null) ? launchTime4.ToString() : null);
								array2[6] = ",";
								int num25 = 7;
								TIDateTime arrivalTime4 = patchedTransfer.arrivalTime;
								array2[num25] = ((arrivalTime4 != null) ? arrivalTime4.ToString() : null);
								array2[8] = ",";
								array2[9] = patchedTransfer.DV_mps.ToString();
								log2.WriteLine(string.Concat(array2));
							}
							transferResult = TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, patchedTransfer.DV_mps, 0.0));
							flag2 = false;
						}
						Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
						trajectory_Patched.BuildSingleTrajectory(CS$<>8__locals1.param.fleet, CS$<>8__locals1.param.sDestination, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.commonBarycenter, patchedTransfer, CS$<>8__locals1.param.fleetAcceleration_mps2);
						if (trajectory_Patched.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
						{
							StreamWriter log3 = CS$<>8__locals1.param.log;
							if (log3 != null)
							{
								string[] array3 = new string[16];
								array3[0] = "FAIL: not enough DV has ";
								array3[1] = CS$<>8__locals1.param.fleetDeltaV_mps.ToString();
								array3[2] = "m/s needs";
								array3[3] = trajectory_Patched.DV_mps.ToString();
								array3[4] = "m/s,MicrothrustOnlyNoTransferingFleetSingleBarycenterInclinationChange,";
								int num26 = 5;
								TIDateTime launchTime5 = patchedTransfer.launchTime;
								array3[num26] = ((launchTime5 != null) ? launchTime5.ToString() : null);
								array3[6] = ",";
								int num27 = 7;
								TIDateTime arrivalTime5 = patchedTransfer.arrivalTime;
								array3[num27] = ((arrivalTime5 != null) ? arrivalTime5.ToString() : null);
								array3[8] = ",";
								array3[9] = patchedTransfer.DV_mps.ToString();
								array3[10] = ",";
								int num28 = 11;
								TIDateTime launchTime6 = trajectory_Patched.launchTime;
								array3[num28] = ((launchTime6 != null) ? launchTime6.ToString() : null);
								array3[12] = ",";
								int num29 = 13;
								TIDateTime arrivalTime6 = trajectory_Patched.arrivalTime;
								array3[num29] = ((arrivalTime6 != null) ? arrivalTime6.ToString() : null);
								array3[14] = ",";
								array3[15] = trajectory_Patched.DV_mps.ToString();
								log3.WriteLine(string.Concat(array3));
							}
							transferResult = TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, trajectory_Patched.DV_mps, 0.0));
							flag2 = false;
						}
						if (flag2)
						{
							candidateTrajectories.Add(trajectory_Patched);
							StreamWriter log4 = CS$<>8__locals1.param.log;
							if (log4 != null)
							{
								string[] array4 = new string[12];
								array4[0] = "SUCCESS,MicrothrustOnlyNoTransferingFleetSingleBarycenterInclinationChange,";
								int num30 = 1;
								TIDateTime launchTime7 = patchedTransfer.launchTime;
								array4[num30] = ((launchTime7 != null) ? launchTime7.ToString() : null);
								array4[2] = ",";
								int num31 = 3;
								TIDateTime arrivalTime7 = patchedTransfer.arrivalTime;
								array4[num31] = ((arrivalTime7 != null) ? arrivalTime7.ToString() : null);
								array4[4] = ",";
								array4[5] = patchedTransfer.DV_mps.ToString();
								array4[6] = ",";
								int num32 = 7;
								TIDateTime launchTime8 = trajectory_Patched.launchTime;
								array4[num32] = ((launchTime8 != null) ? launchTime8.ToString() : null);
								array4[8] = ",";
								int num33 = 9;
								TIDateTime arrivalTime8 = trajectory_Patched.arrivalTime;
								array4[num33] = ((arrivalTime8 != null) ? arrivalTime8.ToString() : null);
								array4[10] = ",";
								array4[11] = trajectory_Patched.DV_mps.ToString();
								log4.WriteLine(string.Concat(array4));
							}
						}
						lowestDVFound_kps = Mathd.Min(lowestDVFound_kps, dv_mps / 1000.0);
					}
				}
				else
				{
					TIDateTime tidateTime9 = new TIDateTime(CS$<>8__locals1.param.now);
					TIDateTime tidateTime10 = new TIDateTime(CS$<>8__locals1.param.now, totalMicrothrustDuration_s);
					if (CS$<>8__locals1.destinationIsTransferingFleet && tidateTime10 > CS$<>8__locals1.targetFleet.trajectory.launchTime && tidateTime10 < CS$<>8__locals1.targetFleet.trajectory.arrivalTime)
					{
						StreamWriter log5 = CS$<>8__locals1.param.log;
						if (log5 != null)
						{
							string text = "ERROR: in microthrust during fleet intercept,MicrothrustOnlyNoTransferingFleetMultiBarycenter,";
							TIDateTime tidateTime11 = tidateTime9;
							string text2 = ((tidateTime11 != null) ? tidateTime11.ToString() : null);
							string text3 = ",";
							TIDateTime tidateTime12 = tidateTime10;
							log5.WriteLine(text + text2 + text3 + ((tidateTime12 != null) ? tidateTime12.ToString() : null));
						}
						CartesianState cartesianState;
						TINaturalSpaceObjectState tinaturalSpaceObjectState5;
						CS$<>8__locals1.targetFleet.tryToGetLocalCartesianState(tidateTime10, out cartesianState, out tinaturalSpaceObjectState5);
						return new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptInMicrothrust, tinaturalSpaceObjectState5.mu, cartesianState.position.magnitude);
					}
					OrbitalElementsState orbitalElementsState5 = new OrbitalElementsState(CS$<>8__locals1.param.destinationValue, 0.0, tidateTime10);
					PatchedTransfer patchedTransfer3 = new PatchedTransfer();
					TransferResult transferResult3 = patchedTransfer3.Solve(tidateTime9, tidateTime10, CS$<>8__locals1.param.originValue, orbitalElementsState5, CS$<>8__locals1.param.destinationValue.barycenter(), CS$<>8__locals1.param.commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.isDestinationOrbit, PatchedTransfer.InternalTransferType.Lambert, null);
					if (transferResult3.Result != TransferResult.Outcome.Success)
					{
						StreamWriter log6 = CS$<>8__locals1.param.log;
						if (log6 != null)
						{
							string[] array5 = new string[8];
							array5[0] = "FAIL: PatchedTransfer failed,MicrothrustOnlyNoTransferingFleetMultiBarycenter,";
							int num34 = 1;
							TIDateTime tidateTime13 = tidateTime9;
							array5[num34] = ((tidateTime13 != null) ? tidateTime13.ToString() : null);
							array5[2] = ",";
							int num35 = 3;
							TIDateTime tidateTime14 = tidateTime10;
							array5[num35] = ((tidateTime14 != null) ? tidateTime14.ToString() : null);
							array5[4] = ",";
							array5[5] = patchedTransfer3.DV_mps.ToString();
							array5[6] = ", ";
							int num36 = 7;
							TransferResult transferResult4 = transferResult3;
							array5[num36] = ((transferResult4 != null) ? transferResult4.ToString() : null);
							log6.WriteLine(string.Concat(array5));
						}
						return transferResult3;
					}
					if (CS$<>8__locals1.destinationIsTransferingFleet && patchedTransfer3.arrivalTime > CS$<>8__locals1.targetFleet.trajectory.launchTime && patchedTransfer3.arrivalTime < CS$<>8__locals1.targetFleet.trajectory.arrivalTime)
					{
						StreamWriter log7 = CS$<>8__locals1.param.log;
						if (log7 != null)
						{
							string[] array6 = new string[6];
							array6[0] = "ERROR: in microthrust during fleet intercept,MicrothrustOnlyNoTransferingFleetMultiBarycenter,";
							int num37 = 1;
							TIDateTime launchTime9 = patchedTransfer3.launchTime;
							array6[num37] = ((launchTime9 != null) ? launchTime9.ToString() : null);
							array6[2] = ",";
							int num38 = 3;
							TIDateTime arrivalTime9 = patchedTransfer3.arrivalTime;
							array6[num38] = ((arrivalTime9 != null) ? arrivalTime9.ToString() : null);
							array6[4] = ",";
							array6[5] = patchedTransfer3.DV_mps.ToString();
							log7.WriteLine(string.Concat(array6));
						}
						CartesianState cartesianState2;
						TINaturalSpaceObjectState tinaturalSpaceObjectState6;
						CS$<>8__locals1.targetFleet.tryToGetLocalCartesianState(tidateTime10, out cartesianState2, out tinaturalSpaceObjectState6);
						return new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptInMicrothrust, tinaturalSpaceObjectState6.mu, cartesianState2.position.magnitude);
					}
					lowestDVFound_kps = Mathd.Min(lowestDVFound_kps, patchedTransfer3.DV_mps / 1000.0);
					if (patchedTransfer3.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
					{
						StreamWriter log8 = CS$<>8__locals1.param.log;
						if (log8 != null)
						{
							string[] array7 = new string[10];
							array7[0] = "FAIL: not enough DV has ";
							array7[1] = CS$<>8__locals1.param.fleetDeltaV_mps.ToString();
							array7[2] = "m/s needs";
							array7[3] = patchedTransfer3.DV_mps.ToString();
							array7[4] = "m/s,MicrothrustOnlyNoTransferingFleetMultiBarycenter,";
							int num39 = 5;
							TIDateTime launchTime10 = patchedTransfer3.launchTime;
							array7[num39] = ((launchTime10 != null) ? launchTime10.ToString() : null);
							array7[6] = ",";
							int num40 = 7;
							TIDateTime arrivalTime10 = patchedTransfer3.arrivalTime;
							array7[num40] = ((arrivalTime10 != null) ? arrivalTime10.ToString() : null);
							array7[8] = ",";
							array7[9] = patchedTransfer3.DV_mps.ToString();
							log8.WriteLine(string.Concat(array7));
						}
						return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, patchedTransfer3.DV_mps, 0.0);
					}
					Trajectory_Patched trajectory_Patched2 = new Trajectory_Patched();
					trajectory_Patched2.BuildSingleTrajectory(CS$<>8__locals1.param.fleet, CS$<>8__locals1.param.sDestination, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.commonBarycenter, patchedTransfer3, CS$<>8__locals1.param.fleetAcceleration_mps2);
					if (trajectory_Patched2.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
					{
						StreamWriter log9 = CS$<>8__locals1.param.log;
						if (log9 != null)
						{
							string[] array8 = new string[16];
							array8[0] = "FAIL: not enough DV has ";
							array8[1] = CS$<>8__locals1.param.fleetDeltaV_mps.ToString();
							array8[2] = "m/s needs";
							array8[3] = trajectory_Patched2.DV_mps.ToString();
							array8[4] = "m/s,MicrothrustOnlyNoTransferingFleetMultiBarycenter,";
							int num41 = 5;
							TIDateTime launchTime11 = patchedTransfer3.launchTime;
							array8[num41] = ((launchTime11 != null) ? launchTime11.ToString() : null);
							array8[6] = ",";
							int num42 = 7;
							TIDateTime arrivalTime11 = patchedTransfer3.arrivalTime;
							array8[num42] = ((arrivalTime11 != null) ? arrivalTime11.ToString() : null);
							array8[8] = ",";
							array8[9] = patchedTransfer3.DV_mps.ToString();
							array8[10] = ",";
							int num43 = 11;
							TIDateTime launchTime12 = trajectory_Patched2.launchTime;
							array8[num43] = ((launchTime12 != null) ? launchTime12.ToString() : null);
							array8[12] = ",";
							int num44 = 13;
							TIDateTime arrivalTime12 = trajectory_Patched2.arrivalTime;
							array8[num44] = ((arrivalTime12 != null) ? arrivalTime12.ToString() : null);
							array8[14] = ",";
							array8[15] = trajectory_Patched2.DV_mps.ToString();
							log9.WriteLine(string.Concat(array8));
						}
						return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, trajectory_Patched2.DV_mps, 0.0);
					}
					candidateTrajectories.Add(trajectory_Patched2);
					StreamWriter log10 = CS$<>8__locals1.param.log;
					if (log10 != null)
					{
						string[] array9 = new string[12];
						array9[0] = "SUCCESS,MicrothrustOnlyNoTransferingFleetMultiBarycenter,";
						int num45 = 1;
						TIDateTime launchTime13 = patchedTransfer3.launchTime;
						array9[num45] = ((launchTime13 != null) ? launchTime13.ToString() : null);
						array9[2] = ",";
						int num46 = 3;
						TIDateTime arrivalTime13 = patchedTransfer3.arrivalTime;
						array9[num46] = ((arrivalTime13 != null) ? arrivalTime13.ToString() : null);
						array9[4] = ",";
						array9[5] = patchedTransfer3.DV_mps.ToString();
						array9[6] = ",";
						int num47 = 7;
						TIDateTime launchTime14 = trajectory_Patched2.launchTime;
						array9[num47] = ((launchTime14 != null) ? launchTime14.ToString() : null);
						array9[8] = ",";
						int num48 = 9;
						TIDateTime arrivalTime14 = trajectory_Patched2.arrivalTime;
						array9[num48] = ((arrivalTime14 != null) ? arrivalTime14.ToString() : null);
						array9[10] = ",";
						array9[11] = trajectory_Patched2.DV_mps.ToString();
						log10.WriteLine(string.Concat(array9));
					}
				}
				if (!CS$<>8__locals1.destinationIsTransferingFleet)
				{
					if (candidateTrajectories.Count > count)
					{
						return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
					}
					if (transferResult.Result == TransferResult.Outcome.Success)
					{
						return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
					}
					return transferResult;
				}
			}
			if (!(!CS$<>8__locals1.hybridTransferType.isMicrothrustOnly | CS$<>8__locals1.destinationIsTransferingFleet | CS$<>8__locals1.originFleetIsInTransfer))
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			CS$<>8__locals1.destFinalFleet = new ValueTuple<TINaturalSpaceObjectState, double>(null, 0.0);
			if (CS$<>8__locals1.destinationIsTransferingFleet)
			{
				TINaturalSpaceObjectState tinaturalSpaceObjectState7;
				double distFromBarycenterAtTime_m = CS$<>8__locals1.targetFleet.trajectory.getDistFromBarycenterAtTime_m(new TIDateTime(CS$<>8__locals1.targetFleet.trajectory.arrivalTime, 10.0), out tinaturalSpaceObjectState7);
				CS$<>8__locals1.destFinalFleet = new ValueTuple<TINaturalSpaceObjectState, double>(tinaturalSpaceObjectState7, distFromBarycenterAtTime_m);
			}
			double num49;
			if (!CS$<>8__locals1.originFleetIsInTransfer && !CS$<>8__locals1.destinationIsTransferingFleet)
			{
				if (CS$<>8__locals1.param.commonBarycenter.isLagrangePointState)
				{
					CartesianState? cartesianState3;
					Vector3d vector3d = ((CS$<>8__locals1.param.originValue.tryToGetGlobalCartesianState(CS$<>8__locals1.param.now) != null) ? cartesianState3.GetValueOrDefault().ToLocal(CS$<>8__locals1.param.commonBarycenter, CS$<>8__locals1.param.now).position : default(Vector3d));
					double magnitude = (Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.now, CS$<>8__locals1.param.commonBarycenter, CS$<>8__locals1.param.fleet.faction, CS$<>8__locals1.param.now, 0.0).position - vector3d).magnitude;
					num49 = MasterTransferPlanner.LagrangeOnlyMaxDuration_s(CS$<>8__locals1.param.fleetAcceleration_mps2, magnitude, CS$<>8__locals1.param.fleetDeltaV_mps);
				}
				else
				{
					num49 = MasterTransferPlanner.GetMaxDurationOfTransfer(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.param.now);
				}
			}
			else
			{
				if (CS$<>8__locals1.destinationIsTransferingFleet && (CS$<>8__locals1.targetFleet.trajectory.exitsSolarSystem || CS$<>8__locals1.targetFleet.trajectory.endsInCrash))
				{
					num49 = CS$<>8__locals1.targetFleet.trajectory.arrivalTime.DifferenceInSeconds(CS$<>8__locals1.param.now) * 0.9;
				}
				else
				{
					if (CS$<>8__locals1.destinationIsTransferingFleet)
					{
						TISpaceGameState destination = CS$<>8__locals1.targetFleet.trajectory.destination;
						if (destination == null || !destination.isOrbitState)
						{
							num49 = CS$<>8__locals1.targetFleet.trajectory.arrivalTime.DifferenceInSeconds(CS$<>8__locals1.param.now) - 1.0;
							goto IL_1D5D;
						}
					}
					Trajectory originFleetTrajectory = CS$<>8__locals1.originFleetTrajectory;
					ValueTuple<TIOrbitState, TIDateTime> valueTuple = ((originFleetTrajectory != null) ? originFleetTrajectory.getFinalOrbitAndArrivalTime() : new ValueTuple<TIOrbitState, TIDateTime>(CS$<>8__locals1.param.fleet.ref_orbit, CS$<>8__locals1.param.now));
					TISpaceFleetState targetFleet2 = CS$<>8__locals1.targetFleet;
					object obj;
					if (targetFleet2 == null)
					{
						obj = null;
					}
					else
					{
						Trajectory trajectory2 = targetFleet2.trajectory;
						obj = ((trajectory2 != null) ? trajectory2.destination : null);
					}
					TIOrbitState tiorbitState = (obj as TIOrbitState) ?? CS$<>8__locals1.param.sDestination.ref_orbit;
					TISpaceFleetState targetFleet3 = CS$<>8__locals1.targetFleet;
					object obj2;
					if (targetFleet3 == null)
					{
						obj2 = null;
					}
					else
					{
						Trajectory trajectory3 = targetFleet3.trajectory;
						obj2 = ((trajectory3 != null) ? trajectory3.destination : null);
					}
					if (obj2 is TISpaceAssetState)
					{
						TISpaceFleetState targetFleet4 = CS$<>8__locals1.targetFleet;
						if (((targetFleet4 != null) ? targetFleet4.faction : null) != CS$<>8__locals1.param.fleet.faction)
						{
							num49 = CS$<>8__locals1.targetFleet.trajectory.arrivalTime.DifferenceInSeconds(CS$<>8__locals1.param.now) - 1.0;
							goto IL_1D5D;
						}
					}
					if (valueTuple.Item1 == tiorbitState.ref_orbit)
					{
						num49 = valueTuple.Item2.DifferenceInSeconds(CS$<>8__locals1.param.now) + tiorbitState.period_s;
						if (CS$<>8__locals1.destinationIsTransferingFleet)
						{
							num49 = Mathd.Max(num49, CS$<>8__locals1.targetFleet.trajectory.arrivalTime.DifferenceInSeconds(CS$<>8__locals1.param.now) + tiorbitState.period_s);
						}
					}
					else if (valueTuple.Item1 != null)
					{
						if (CS$<>8__locals1.destinationIsTransferingFleet)
						{
							ValueTuple<TINaturalSpaceObjectState, double> barycenterAndRadiusOfFleetAtArrival = MasterTransferPlanner.GetBarycenterAndRadiusOfFleetAtArrival(CS$<>8__locals1.targetFleet);
							num49 = TIDateTime.Max(valueTuple.Item2, CS$<>8__locals1.targetFleet.trajectory.arrivalTime).DifferenceInSeconds(CS$<>8__locals1.param.now) + MasterTransferPlanner.GetMaxDurationOfTransfer(valueTuple.Item1.barycenter, valueTuple.Item1.semiMajorAxis_m, barycenterAndRadiusOfFleetAtArrival.Item1, barycenterAndRadiusOfFleetAtArrival.Item2, CS$<>8__locals1.param.fleetAcceleration_mps2, true);
						}
						else
						{
							num49 = valueTuple.Item2.DifferenceInSeconds(CS$<>8__locals1.param.now) + MasterTransferPlanner.GetMaxDurationOfTransfer(valueTuple.Item1.barycenter, valueTuple.Item1.semiMajorAxis_m, CS$<>8__locals1.param.sDestination.ref_orbit.barycenter, CS$<>8__locals1.param.sDestination.ref_orbit.semiMajorAxis_m, CS$<>8__locals1.param.fleetAcceleration_mps2, true);
						}
					}
					else
					{
						CartesianState cartesianState4;
						TINaturalSpaceObjectState tinaturalSpaceObjectState8;
						CS$<>8__locals1.param.fleet.tryToGetLocalCartesianState(CS$<>8__locals1.param.now, out cartesianState4, out tinaturalSpaceObjectState8);
						num49 = MasterTransferPlanner.GetMaxDurationOfTransfer(tinaturalSpaceObjectState8, cartesianState4.position.magnitude, CS$<>8__locals1.param.sDestination.ref_orbit.barycenter, CS$<>8__locals1.param.sDestination.ref_orbit.semiMajorAxis_m, CS$<>8__locals1.param.fleetAcceleration_mps2, true);
					}
				}
				IL_1D5D:
				num49 = Mathd.Min(num49, MasterTransferPlanner.TransferDurationHardCap(CS$<>8__locals1.param.fleet.faction));
			}
			double num50 = ((CS$<>8__locals1.destinationIsTransferingFleet | CS$<>8__locals1.originFleetIsInTransfer) ? 0.0 : CS$<>8__locals1.hybridTransferType.totalMicrothrustDuration_s);
			int num51 = Mathd.CeilToInt((double)(CS$<>8__locals1.isPlayer ? 60 : 30) * CS$<>8__locals1.param.sampleSizeMultiplier);
			int num52 = Mathd.Max(Mathd.Min(num51, Mathd.CeilToInt((num49 - num50) / 3600.0)), 10);
			CS$<>8__locals1.arrivalTimeStep_s = num49 / (double)num52;
			new TIDateTime(CS$<>8__locals1.param.now, num50 + CS$<>8__locals1.arrivalTimeStep_s);
			MasterTransferPlanner.<>c__DisplayClass32_0 CS$<>8__locals2 = CS$<>8__locals1;
			TIDateTime tidateTime15;
			if (!CS$<>8__locals1.destinationIsTransferingFleet)
			{
				tidateTime15 = null;
			}
			else
			{
				TISpaceFleetState targetFleet5 = CS$<>8__locals1.targetFleet;
				if (targetFleet5 == null)
				{
					tidateTime15 = null;
				}
				else
				{
					Trajectory trajectory4 = targetFleet5.trajectory;
					tidateTime15 = ((trajectory4 != null) ? trajectory4.arrivalTime : null);
				}
			}
			CS$<>8__locals2.destinationFleetArrivalTime = tidateTime15;
			CS$<>8__locals1.destinationFleetArrivalTimePlusALittle = ((CS$<>8__locals1.destinationFleetArrivalTime == null) ? null : new TIDateTime(CS$<>8__locals1.destinationFleetArrivalTime, 600.0));
			TIDateTime tidateTime16;
			if (!CS$<>8__locals1.destinationIsTransferingFleet)
			{
				tidateTime16 = null;
			}
			else
			{
				TISpaceFleetState targetFleet6 = CS$<>8__locals1.targetFleet;
				if (targetFleet6 == null)
				{
					tidateTime16 = null;
				}
				else
				{
					Trajectory trajectory5 = targetFleet6.trajectory;
					tidateTime16 = ((trajectory5 != null) ? trajectory5.launchTime : null);
				}
			}
			TIDateTime tidateTime17 = tidateTime16;
			TIDateTime tidateTime18 = ((tidateTime17 == null) ? null : new TIDateTime(tidateTime17, -600.0));
			List<TIDateTime> list = new List<TIDateTime>();
			if (CS$<>8__locals1.destinationFleetArrivalTime != null)
			{
				list.Add(CS$<>8__locals1.destinationFleetArrivalTimePlusALittle);
			}
			if (tidateTime18 != null && tidateTime18 > CS$<>8__locals1.param.now)
			{
				list.Add(tidateTime18);
			}
			TIDateTime tidateTime19 = new TIDateTime(CS$<>8__locals1.param.now, num49);
			bool flag3 = CS$<>8__locals1.param.destinationValue.barycenter().FindCommonBarycenter(CS$<>8__locals1.param.fleet.barycenter()) == CS$<>8__locals1.param.destinationValue.barycenter();
			MasterTransferPlanner.HohmannTiming hohmannTiming = new MasterTransferPlanner.HohmannTiming();
			if (!CS$<>8__locals1.destinationIsTransferingFleet && (!CS$<>8__locals1.isDestinationOrbit || !flag3) && !CS$<>8__locals1.originFleetIsInTransfer)
			{
				bool flag;
				OrbitalElementsState orbitalElementsState6;
				TINaturalSpaceObjectState tinaturalSpaceObjectState9;
				CS$<>8__locals1.param.fleet.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState6, out tinaturalSpaceObjectState9, out flag);
				OrbitalElementsState orbitalElementsState7;
				TINaturalSpaceObjectState tinaturalSpaceObjectState10;
				CS$<>8__locals1.param.destinationValue.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState7, out tinaturalSpaceObjectState10, out flag);
				if (tinaturalSpaceObjectState9 != tinaturalSpaceObjectState10 || !Mathd.Approximately(orbitalElementsState6.semiMajorAxis_m, orbitalElementsState7.semiMajorAxis_m))
				{
					TIDateTime bestHohmannArrivalTime = MasterTransferPlanner.GetBestHohmannArrivalTime(orbitalElementsState6, orbitalElementsState7, tinaturalSpaceObjectState9, tinaturalSpaceObjectState10, TITimeState.Now(), (double)CS$<>8__locals1.param.fleet.cruiseAcceleration_mps2, CS$<>8__locals1.param.fleet.faction, out hohmannTiming);
					if (bestHohmannArrivalTime <= tidateTime19)
					{
						list.Add(bestHohmannArrivalTime);
					}
				}
			}
			if (CS$<>8__locals1.destinationIsTransferingFleet && MasterTransferPlanner.DoesOrbitMatch(CS$<>8__locals1.param.fleet, CS$<>8__locals1.targetFleet.trajectory.destinationOrbit))
			{
				tidateTime19 = CS$<>8__locals1.targetFleet.trajectory.arrivalTime;
			}
			candidateTrajectories.AddRange(MasterTransferPlanner.LoopOverArrivalTimes(num51, CS$<>8__locals1.param.sampleSizeMultiplier, new TIDateTime(CS$<>8__locals1.param.now, num50), tidateTime19, list, hohmannTiming, CS$<>8__locals1.param.fleetDeltaV_mps, CS$<>8__locals1.param.stopOnFirstSuccess, CS$<>8__locals1.param.fleet.faction, out CS$<>8__locals1.bestResult, out CS$<>8__locals1.lowestDVFound_mps, delegate(TIDateTime arrivalTime, [TupleElementNames(new string[] { "launchTime", "stepSize_s" })] ValueTuple<TIDateTime, double>? lockedLaunchTime)
			{
				TransferResult transferResult5 = new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				MasterTransferPlanner.SimplifiedPositions simplifiedPositions2 = MasterTransferPlanner.GetSimplifiedPositions(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.now, arrivalTime);
				MasterTransferPlanner.IdentifyHybridTransferType_Result identifyHybridTransferType_Result = MasterTransferPlanner.IdentifyHybridTransferType(simplifiedPositions2, CS$<>8__locals1.param.fleetAcceleration_mps2);
				if (identifyHybridTransferType_Result.totalMicrothrustDuration_s > 78892310.0)
				{
					Log.Warn("Necessary microthrust duration exceeds 2 years.  Aborting attempt early.", Array.Empty<object>());
					CS$<>8__locals1.bestResult = new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, identifyHybridTransferType_Result.totalMicrothrustDuration_s, 78892310.0);
					CS$<>8__locals1.lowestDVFound_mps = double.PositiveInfinity;
					return new ValueTuple<TransferResult, Trajectory_Patched>(CS$<>8__locals1.bestResult, null);
				}
				if (arrivalTime > CS$<>8__locals1.destinationFleetArrivalTime && arrivalTime.DifferenceInSeconds(CS$<>8__locals1.destinationFleetArrivalTime) < CS$<>8__locals1.arrivalTimeStep_s)
				{
					arrivalTime = new TIDateTime(CS$<>8__locals1.destinationFleetArrivalTimePlusALittle);
				}
				ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState> relevantBarycentersAtTime = MasterTransferPlanner.GetRelevantBarycentersAtTime(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.now, arrivalTime);
				TINaturalSpaceObjectState sourceBarycenter = relevantBarycentersAtTime.Item1;
				TINaturalSpaceObjectState destinationBarycenter = relevantBarycentersAtTime.Item2;
				TINaturalSpaceObjectState commonBarycenter = relevantBarycentersAtTime.Item3;
				if (CS$<>8__locals1.destinationIsTransferingFleet && arrivalTime > CS$<>8__locals1.targetFleet.trajectory.launchTime)
				{
					if (arrivalTime < CS$<>8__locals1.targetFleet.trajectory.arrivalTime)
					{
						double magnitude2 = (CS$<>8__locals1.targetFleet.trajectory.ToGlobalCartesianStateAtTime(arrivalTime).position - destinationBarycenter.ToGlobalCartesianStateAtTime(arrivalTime).position).magnitude;
						MicrothrustSphere microthrustSphere6 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, destinationBarycenter.mu, destinationBarycenter.sphereOfInfluence_m);
						if ((microthrustSphere6.IsLimitedBySphereOfInfluence || microthrustSphere6.Radius_m > magnitude2) && arrivalTime < CS$<>8__locals1.targetFleet.trajectory.arrivalTime)
						{
							StreamWriter log11 = CS$<>8__locals1.param.log;
							if (log11 != null)
							{
								string text4 = "FAIL: in microthrust during fleet intercept,Hybrid,unknownLaunchTime,";
								TIDateTime arrivalTime16 = arrivalTime;
								log11.WriteLine(text4 + ((arrivalTime16 != null) ? arrivalTime16.ToString() : null));
							}
							transferResult5 = TransferResult.Best(transferResult5, new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptInMicrothrust, destinationBarycenter.mu, magnitude2));
							return new ValueTuple<TransferResult, Trajectory_Patched>(transferResult5, null);
						}
					}
					else if (CS$<>8__locals1.targetFleet.trajectory.destinationOrbit == null)
					{
						StreamWriter log12 = CS$<>8__locals1.param.log;
						if (log12 != null)
						{
							string text5 = "FAIL: attempted fleet intercept after target arrived,Hybrid,unknownLaunchTime,";
							TIDateTime arrivalTime17 = arrivalTime;
							log12.WriteLine(text5 + ((arrivalTime17 != null) ? arrivalTime17.ToString() : null));
						}
						transferResult5 = TransferResult.Best(transferResult5, new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptAfterArrivalAtAsset, arrivalTime.DifferenceInSeconds(CS$<>8__locals1.targetFleet.trajectory.arrivalTime), 0.0));
						return new ValueTuple<TransferResult, Trajectory_Patched>(transferResult5, null);
					}
				}
				bool flag4 = sourceBarycenter == commonBarycenter;
				bool flag5 = destinationBarycenter == commonBarycenter;
				if (flag4 && flag5)
				{
					TISpaceFleetState tispaceFleetState3 = CS$<>8__locals1.param.fleet as TISpaceFleetState;
					if (tispaceFleetState3 != null && tispaceFleetState3.transferAssigned && tispaceFleetState3.trajectory.destination != null && tispaceFleetState3.trajectory.destination.barycenter != commonBarycenter)
					{
						TISpaceGameState destination2 = tispaceFleetState3.trajectory.destination;
						TINaturalSpaceObjectState tinaturalSpaceObjectState11 = ((destination2 != null) ? destination2.barycenter : null);
						TINaturalSpaceObjectState tinaturalSpaceObjectState12 = tinaturalSpaceObjectState11.FindCommonBarycenter(destinationBarycenter);
						flag4 = tinaturalSpaceObjectState11 == tinaturalSpaceObjectState12;
						flag5 = destinationBarycenter == tinaturalSpaceObjectState12;
					}
				}
				ValueTuple<Trajectory_Patched, double> valueTuple2 = new ValueTuple<Trajectory_Patched, double>(null, double.PositiveInfinity);
				if (flag4)
				{
					if (flag5)
					{
						if (CS$<>8__locals1.isDestinationOrbit)
						{
							List<TIDateTime> list2 = new List<TIDateTime>();
							if (lockedLaunchTime != null)
							{
								list2.Add((lockedLaunchTime != null) ? lockedLaunchTime.GetValueOrDefault().Item1 : null);
							}
							else if (CS$<>8__locals1.originFleetIsInTransfer)
							{
								list2.AddRange(MasterTransferPlanner.LaunchTimesToTestOnActiveTrajectory(CS$<>8__locals1.originFleetTrajectory, CS$<>8__locals1.param.now, arrivalTime));
							}
							else
							{
								list2.Add(new TIDateTime(CS$<>8__locals1.param.now));
							}
							using (List<TIDateTime>.Enumerator enumerator = list2.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									TIDateTime tidateTime20 = enumerator.Current;
									MasterTransferPlanner.SimplifiedPositions simplifiedPositions3 = MasterTransferPlanner.GetSimplifiedPositions(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, tidateTime20, arrivalTime);
									double num53 = MasterTransferPlanner.HohmannFirstBurnDuration_s(CS$<>8__locals1.param.fleetAcceleration_mps2, simplifiedPositions3.originDistToCommonBarycenter_m, simplifiedPositions3.destinationDistToCommonBarycenter_m, simplifiedPositions3.commonBarycenter.mu) * 2.0;
									double num54 = 0.0;
									MicrothrustSphere microthrustSphere7 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, simplifiedPositions3.commonBarycenter.mu, simplifiedPositions3.commonBarycenter.sphereOfInfluence_m);
									double originDistToCommonBarycenter_m = simplifiedPositions3.originDistToCommonBarycenter_m;
									if (originDistToCommonBarycenter_m < microthrustSphere7.Radius_m)
									{
										double num55 = Mathd.Sqrt(simplifiedPositions3.commonBarycenter.mu / originDistToCommonBarycenter_m);
										num54 = microthrustSphere7.GetAnomalyDelta_Rad(num55);
										num53 = Mathd.Max(0.0, num53 - microthrustSphere7.GetDuration_s(num55));
									}
									tidateTime20.AddSeconds(num53);
									TIDateTime tidateTime21 = new TIDateTime(arrivalTime, num53);
									double num56 = 0.0;
									TISpaceFleetState tispaceFleetState4 = CS$<>8__locals1.param.fleet as TISpaceFleetState;
									if (tispaceFleetState4 != null && tispaceFleetState4.transferAssigned && tispaceFleetState4.trajectory.launchTime < tidateTime20)
									{
										double num57 = tispaceFleetState4.trajectory.GetOrbitalElementsAtTime(tidateTime20).MeanAnomalyAtTime_Rad(tidateTime20.ExportTime(), tispaceFleetState4.trajectory.GetBarycenterAtTime(tidateTime20).mass_kg);
									}
									else
									{
										double num57 = CS$<>8__locals1.param.fleet.meanAnomaly_Rad(tidateTime20);
										double num58 = CS$<>8__locals1.param.originValue.Ω_rad() - CS$<>8__locals1.param.destinationValue.Ω_rad() + CS$<>8__locals1.param.originValue.ω_rad() - CS$<>8__locals1.param.destinationValue.ω_rad();
										num56 = 3.141592653589793 + num57 + num58 + num54;
									}
									double num59 = num56 + 2.0943951023931953;
									double num60 = num56 + 4.1887902047863905;
									double num61 = num56 + 6.283185307179586;
									double[] array10 = new double[] { num56, num59, num60 };
									ValueTuple<PatchedTransfer, double> valueTuple3 = new ValueTuple<PatchedTransfer, double>(null, 0.0);
									ValueTuple<PatchedTransfer, double> valueTuple4 = new ValueTuple<PatchedTransfer, double>(null, 0.0);
									int i = 0;
									while (i < 3)
									{
										if (CS$<>8__locals1.param.aerobreaking || CS$<>8__locals1.param.unsafeAerobreaking)
										{
											ValueTuple<AerobreakInfo, AerobreakInfo> valueTuple5;
											MasterTransferPlanner.AerobreakPrecalculator(CS$<>8__locals1.param, tidateTime20, tidateTime21, out valueTuple5, true, array10[i]);
										}
										OrbitalElementsState orbitalElementsState8 = new OrbitalElementsState(CS$<>8__locals1.param.destinationValue, array10[i], tidateTime21);
										PatchedTransfer patchedTransfer4 = new PatchedTransfer();
										TransferResult transferResult6 = patchedTransfer4.Solve(tidateTime20, tidateTime21, CS$<>8__locals1.param.originValue, orbitalElementsState8, destinationBarycenter, commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.isDestinationOrbit, PatchedTransfer.InternalTransferType.Lambert, null);
										if (transferResult6.Result == TransferResult.Outcome.Success)
										{
											goto IL_08DB;
										}
										transferResult5 = TransferResult.Best(transferResult5, transferResult6);
										StreamWriter log13 = CS$<>8__locals1.param.log;
										if (log13 != null)
										{
											string[] array11 = new string[8];
											array11[0] = "FAIL,HybridSourceIsCommonDestinationIsCommonAndOrbit_Lambert,";
											array11[1] = (array10[i] * 57.29577951308232).ToString();
											array11[2] = "°,";
											int num62 = 3;
											TIDateTime tidateTime22 = tidateTime20;
											array11[num62] = ((tidateTime22 != null) ? tidateTime22.ToString() : null);
											array11[4] = ",";
											int num63 = 5;
											TIDateTime tidateTime23 = tidateTime21;
											array11[num63] = ((tidateTime23 != null) ? tidateTime23.ToString() : null);
											array11[6] = ",";
											array11[7] = transferResult6.ToString();
											log13.WriteLine(string.Concat(array11));
										}
										TransferResult transferResult7 = patchedTransfer4.Solve(tidateTime20, tidateTime21, CS$<>8__locals1.param.originValue, orbitalElementsState8, destinationBarycenter, commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.isDestinationOrbit, PatchedTransfer.InternalTransferType.Torch, null);
										if (transferResult7.Result == TransferResult.Outcome.Success)
										{
											goto IL_08DB;
										}
										transferResult5 = TransferResult.Best(transferResult5, transferResult7);
										StreamWriter log14 = CS$<>8__locals1.param.log;
										if (log14 != null)
										{
											string[] array12 = new string[8];
											array12[0] = "FAIL,HybridSourceIsCommonDestinationIsCommonAndOrbit_Torch,";
											array12[1] = (array10[i] * 57.29577951308232).ToString();
											array12[2] = "°,";
											int num64 = 3;
											TIDateTime tidateTime24 = tidateTime20;
											array12[num64] = ((tidateTime24 != null) ? tidateTime24.ToString() : null);
											array12[4] = ",";
											int num65 = 5;
											TIDateTime tidateTime25 = tidateTime21;
											array12[num65] = ((tidateTime25 != null) ? tidateTime25.ToString() : null);
											array12[6] = ",";
											array12[7] = transferResult7.ToString();
											log14.WriteLine(string.Concat(array12));
										}
										IL_0A87:
										i++;
										continue;
										IL_08DB:
										if (CS$<>8__locals1.param.stopOnFirstSuccess)
										{
											Trajectory_Patched trajectory_Patched3 = new Trajectory_Patched();
											trajectory_Patched3.BuildSingleTrajectory(CS$<>8__locals1.param.fleet, CS$<>8__locals1.param.sDestination, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, commonBarycenter, patchedTransfer4, CS$<>8__locals1.param.fleetAcceleration_mps2);
											return new ValueTuple<TransferResult, Trajectory_Patched>(transferResult6, trajectory_Patched3);
										}
										StreamWriter log15 = CS$<>8__locals1.param.log;
										if (log15 != null)
										{
											string[] array13 = new string[12];
											array13[0] = "SUCCESS,HybridSourceIsCommonDestinationIsCommonAndOrbit,";
											array13[1] = (array10[i] * 57.29577951308232).ToString();
											array13[2] = "°,";
											int num66 = 3;
											TIDateTime tidateTime26 = tidateTime20;
											array13[num66] = ((tidateTime26 != null) ? tidateTime26.ToString() : null);
											array13[4] = ",";
											int num67 = 5;
											TIDateTime tidateTime27 = tidateTime21;
											array13[num67] = ((tidateTime27 != null) ? tidateTime27.ToString() : null);
											array13[6] = ",";
											int num68 = 7;
											TIDateTime launchTime15 = patchedTransfer4.launchTime;
											array13[num68] = ((launchTime15 != null) ? launchTime15.ToString() : null);
											array13[8] = ",";
											int num69 = 9;
											TIDateTime arrivalTime18 = patchedTransfer4.arrivalTime;
											array13[num69] = ((arrivalTime18 != null) ? arrivalTime18.ToString() : null);
											array13[10] = ",";
											array13[11] = patchedTransfer4.DV_mps.ToString();
											log15.WriteLine(string.Concat(array13));
										}
										if (valueTuple3.Item1 == null)
										{
											valueTuple3 = new ValueTuple<PatchedTransfer, double>(patchedTransfer4, array10[i]);
											goto IL_0A87;
										}
										if (valueTuple3.Item1.DV_mps > patchedTransfer4.DV_mps)
										{
											valueTuple4 = valueTuple3;
											valueTuple3 = new ValueTuple<PatchedTransfer, double>(patchedTransfer4, array10[i]);
											goto IL_0A87;
										}
										if (valueTuple4.Item1 == null || valueTuple4.Item1.DV_mps > patchedTransfer4.DV_mps)
										{
											valueTuple4 = new ValueTuple<PatchedTransfer, double>(patchedTransfer4, array10[i]);
											goto IL_0A87;
										}
										goto IL_0A87;
									}
									if (valueTuple3.Item1 != null)
									{
										if (valueTuple3.Item2 == 0.0 && valueTuple4.Item2 == num60)
										{
											valueTuple3.Item2 = num61;
										}
										else if (valueTuple4.Item2 == 0.0 && valueTuple3.Item2 == num60)
										{
											valueTuple4.Item2 = num61;
										}
										int j = Mathd.CeilToInt((double)(CS$<>8__locals1.isPlayer ? 5 : 5) * CS$<>8__locals1.param.sampleSizeMultiplier);
										if (valueTuple4.Item1 == null)
										{
											double num70 = 2.0943951023931953;
											while (j > 0)
											{
												if (valueTuple4.Item1 != null)
												{
													break;
												}
												j--;
												num70 /= 2.0;
												double[] array14 = new double[]
												{
													valueTuple3.Item2 - num70,
													valueTuple3.Item2 + num70
												};
												int k = 0;
												while (k < 2)
												{
													OrbitalElementsState orbitalElementsState9 = new OrbitalElementsState(CS$<>8__locals1.param.destinationValue, array14[k], tidateTime21);
													PatchedTransfer patchedTransfer5 = new PatchedTransfer();
													TransferResult transferResult8 = patchedTransfer5.Solve(tidateTime20, tidateTime21, CS$<>8__locals1.param.originValue, orbitalElementsState9, destinationBarycenter, commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.isDestinationOrbit, PatchedTransfer.InternalTransferType.Lambert, null);
													if (transferResult8.Result == TransferResult.Outcome.Success)
													{
														goto IL_0D27;
													}
													StreamWriter log16 = CS$<>8__locals1.param.log;
													if (log16 != null)
													{
														string[] array15 = new string[8];
														array15[0] = "FAIL,HybridSourceIsCommonDestinationIsCommonAndOrbit_Lambert,";
														array15[1] = (array14[k] * 57.29577951308232).ToString();
														array15[2] = "°,";
														int num71 = 3;
														TIDateTime tidateTime28 = tidateTime20;
														array15[num71] = ((tidateTime28 != null) ? tidateTime28.ToString() : null);
														array15[4] = ",";
														int num72 = 5;
														TIDateTime tidateTime29 = tidateTime21;
														array15[num72] = ((tidateTime29 != null) ? tidateTime29.ToString() : null);
														array15[6] = ",";
														array15[7] = transferResult8.ToString();
														log16.WriteLine(string.Concat(array15));
													}
													if (patchedTransfer5.Solve(tidateTime20, tidateTime21, CS$<>8__locals1.param.originValue, orbitalElementsState9, destinationBarycenter, commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.isDestinationOrbit, PatchedTransfer.InternalTransferType.Torch, null).Result == TransferResult.Outcome.Success)
													{
														goto IL_0D27;
													}
													StreamWriter log17 = CS$<>8__locals1.param.log;
													if (log17 != null)
													{
														string[] array16 = new string[8];
														array16[0] = "FAIL,HybridSourceIsCommonDestinationIsCommonAndOrbit_Torch,";
														array16[1] = (array14[k] * 57.29577951308232).ToString();
														array16[2] = "°,";
														int num73 = 3;
														TIDateTime tidateTime30 = tidateTime20;
														array16[num73] = ((tidateTime30 != null) ? tidateTime30.ToString() : null);
														array16[4] = ",";
														int num74 = 5;
														TIDateTime tidateTime31 = tidateTime21;
														array16[num74] = ((tidateTime31 != null) ? tidateTime31.ToString() : null);
														array16[6] = ",";
														array16[7] = transferResult8.ToString();
														log17.WriteLine(string.Concat(array16));
													}
													IL_0E27:
													k++;
													continue;
													IL_0D27:
													StreamWriter log18 = CS$<>8__locals1.param.log;
													if (log18 != null)
													{
														string[] array17 = new string[12];
														array17[0] = "SUCCESS,HybridSourceIsCommonDestinationIsCommonAndOrbit,";
														array17[1] = (array14[k] * 57.29577951308232).ToString();
														array17[2] = "°,";
														int num75 = 3;
														TIDateTime tidateTime32 = tidateTime20;
														array17[num75] = ((tidateTime32 != null) ? tidateTime32.ToString() : null);
														array17[4] = ",";
														int num76 = 5;
														TIDateTime tidateTime33 = tidateTime21;
														array17[num76] = ((tidateTime33 != null) ? tidateTime33.ToString() : null);
														array17[6] = ",";
														int num77 = 7;
														TIDateTime launchTime16 = patchedTransfer5.launchTime;
														array17[num77] = ((launchTime16 != null) ? launchTime16.ToString() : null);
														array17[8] = ",";
														int num78 = 9;
														TIDateTime arrivalTime19 = patchedTransfer5.arrivalTime;
														array17[num78] = ((arrivalTime19 != null) ? arrivalTime19.ToString() : null);
														array17[10] = ",";
														array17[11] = patchedTransfer5.DV_mps.ToString();
														log18.WriteLine(string.Concat(array17));
													}
													if (valueTuple4.Item1 == null || valueTuple4.Item1.DV_mps < patchedTransfer5.DV_mps)
													{
														valueTuple4 = new ValueTuple<PatchedTransfer, double>(patchedTransfer5, array14[k]);
														goto IL_0E27;
													}
													goto IL_0E27;
												}
												if (valueTuple4.Item1 != null && valueTuple3.Item1.DV_mps > valueTuple4.Item1.DV_mps)
												{
													ValueTuple<PatchedTransfer, double> valueTuple6 = valueTuple3;
													valueTuple3 = valueTuple4;
													valueTuple4 = valueTuple6;
												}
											}
										}
										while (j > 0)
										{
											j--;
											double num79 = (valueTuple3.Item2 + valueTuple4.Item2) / 2.0;
											OrbitalElementsState orbitalElementsState10 = new OrbitalElementsState(CS$<>8__locals1.param.destinationValue, num79, tidateTime21);
											PatchedTransfer patchedTransfer6 = new PatchedTransfer();
											TransferResult transferResult9 = patchedTransfer6.Solve(tidateTime20, tidateTime21, CS$<>8__locals1.param.originValue, orbitalElementsState10, destinationBarycenter, commonBarycenter, CS$<>8__locals1.param.fleetAcceleration_mps2, CS$<>8__locals1.isDestinationOrbit, PatchedTransfer.InternalTransferType.Lambert, null);
											if (transferResult9.Result != TransferResult.Outcome.Success)
											{
												transferResult5 = TransferResult.Best(transferResult5, transferResult9);
												StreamWriter log19 = CS$<>8__locals1.param.log;
												if (log19 == null)
												{
													break;
												}
												string[] array18 = new string[8];
												array18[0] = "FAIL,HybridSourceIsCommonDestinationIsCommonAndOrbit,";
												array18[1] = (num79 * 57.29577951308232).ToString();
												array18[2] = "°,";
												int num80 = 3;
												TIDateTime tidateTime34 = tidateTime20;
												array18[num80] = ((tidateTime34 != null) ? tidateTime34.ToString() : null);
												array18[4] = ",";
												int num81 = 5;
												TIDateTime tidateTime35 = tidateTime21;
												array18[num81] = ((tidateTime35 != null) ? tidateTime35.ToString() : null);
												array18[6] = ",";
												array18[7] = transferResult9.ToString();
												log19.WriteLine(string.Concat(array18));
												break;
											}
											else
											{
												StreamWriter log20 = CS$<>8__locals1.param.log;
												if (log20 != null)
												{
													string[] array19 = new string[12];
													array19[0] = "SUCCESS,HybridSourceIsCommonDestinationIsCommonAndOrbit,";
													array19[1] = (num79 * 57.29577951308232).ToString();
													array19[2] = "°,";
													int num82 = 3;
													TIDateTime tidateTime36 = tidateTime20;
													array19[num82] = ((tidateTime36 != null) ? tidateTime36.ToString() : null);
													array19[4] = ",";
													int num83 = 5;
													TIDateTime tidateTime37 = tidateTime21;
													array19[num83] = ((tidateTime37 != null) ? tidateTime37.ToString() : null);
													array19[6] = ",";
													int num84 = 7;
													TIDateTime launchTime17 = patchedTransfer6.launchTime;
													array19[num84] = ((launchTime17 != null) ? launchTime17.ToString() : null);
													array19[8] = ",";
													int num85 = 9;
													TIDateTime arrivalTime20 = patchedTransfer6.arrivalTime;
													array19[num85] = ((arrivalTime20 != null) ? arrivalTime20.ToString() : null);
													array19[10] = ",";
													array19[11] = patchedTransfer6.DV_mps.ToString();
													log20.WriteLine(string.Concat(array19));
												}
												if (patchedTransfer6.DV_mps < valueTuple3.Item1.DV_mps)
												{
													valueTuple4 = valueTuple3;
													valueTuple3 = new ValueTuple<PatchedTransfer, double>(patchedTransfer6, num79);
												}
												else
												{
													valueTuple4 = new ValueTuple<PatchedTransfer, double>(patchedTransfer6, num79);
												}
											}
										}
										if (valueTuple3.Item1.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps || valueTuple3.Item1.launchTime < TITimeState.Now())
										{
											valueTuple2 = new ValueTuple<Trajectory_Patched, double>(null, valueTuple3.Item1.DV_mps);
											StreamWriter log21 = CS$<>8__locals1.param.log;
											if (log21 != null)
											{
												string[] array20 = new string[10];
												array20[0] = "FAIL: not enough DV has ";
												array20[1] = CS$<>8__locals1.param.fleetDeltaV_mps.ToString();
												array20[2] = "m/s needs";
												array20[3] = valueTuple3.Item1.DV_mps.ToString();
												array20[4] = "m/s,HybridSourceIsCommonDestinationIsCommonAndOrbit,";
												int num86 = 5;
												TIDateTime launchTime18 = valueTuple3.Item1.launchTime;
												array20[num86] = ((launchTime18 != null) ? launchTime18.ToString() : null);
												array20[6] = ",";
												int num87 = 7;
												TIDateTime arrivalTime21 = valueTuple3.Item1.arrivalTime;
												array20[num87] = ((arrivalTime21 != null) ? arrivalTime21.ToString() : null);
												array20[8] = ",";
												array20[9] = valueTuple3.Item1.DV_mps.ToString();
												log21.WriteLine(string.Concat(array20));
											}
										}
										else
										{
											Trajectory_Patched trajectory_Patched4 = new Trajectory_Patched();
											trajectory_Patched4.BuildSingleTrajectory(CS$<>8__locals1.param.fleet, CS$<>8__locals1.param.sDestination, CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, commonBarycenter, valueTuple3.Item1, CS$<>8__locals1.param.fleetAcceleration_mps2);
											valueTuple2 = new ValueTuple<Trajectory_Patched, double>(trajectory_Patched4, valueTuple3.Item1.DV_mps);
											StreamWriter log22 = CS$<>8__locals1.param.log;
											if (log22 != null)
											{
												string[] array21 = new string[12];
												array21[0] = "SUCCESS,HybridSourceIsCommonDestinationIsCommonAndOrbit,";
												int num88 = 1;
												TIDateTime launchTime19 = valueTuple3.Item1.launchTime;
												array21[num88] = ((launchTime19 != null) ? launchTime19.ToString() : null);
												array21[2] = ",";
												int num89 = 3;
												TIDateTime arrivalTime22 = valueTuple3.Item1.arrivalTime;
												array21[num89] = ((arrivalTime22 != null) ? arrivalTime22.ToString() : null);
												array21[4] = ",";
												array21[5] = valueTuple3.Item1.DV_mps.ToString();
												array21[6] = ",";
												int num90 = 7;
												TIDateTime launchTime20 = trajectory_Patched4.launchTime;
												array21[num90] = ((launchTime20 != null) ? launchTime20.ToString() : null);
												array21[8] = ",";
												int num91 = 9;
												TIDateTime arrivalTime23 = trajectory_Patched4.arrivalTime;
												array21[num91] = ((arrivalTime23 != null) ? arrivalTime23.ToString() : null);
												array21[10] = ",";
												array21[11] = trajectory_Patched4.DV_mps.ToString();
												log22.WriteLine(string.Concat(array21));
											}
										}
									}
								}
								goto IL_1BBE;
							}
						}
						double originDistToCommonBarycenter_m2 = simplifiedPositions2.originDistToCommonBarycenter_m;
						double num92 = MasterTransferPlanner.HohmannDuration_s(simplifiedPositions2);
						double totalMicrothrustDuration_s2 = MasterTransferPlanner.IdentifyHybridTransferType(simplifiedPositions2, CS$<>8__locals1.param.fleetAcceleration_mps2).totalMicrothrustDuration_s;
						if (totalMicrothrustDuration_s2 > 78892310.0)
						{
							return new ValueTuple<TransferResult, Trajectory_Patched>(new TransferResult(TransferResult.Outcome.Fail_ExceedsMaxDuration, 78892310.0, 0.0), null);
						}
						TIDateTime tidateTime38;
						if (arrivalTime.DifferenceInSeconds(CS$<>8__locals1.param.now) < num92)
						{
							tidateTime38 = new TIDateTime(CS$<>8__locals1.param.now);
						}
						else
						{
							tidateTime38 = new TIDateTime(arrivalTime, -num92 - totalMicrothrustDuration_s2);
						}
						TIDateTime tidateTime39 = new TIDateTime(CS$<>8__locals1.param.now);
						if (tidateTime38 < tidateTime39)
						{
							tidateTime38 = new TIDateTime(tidateTime39);
						}
						if (CS$<>8__locals1.originFleetIsInTransfer)
						{
							tidateTime38 = MasterTransferPlanner.BestLaunchTimeFromActiveTrajectory(CS$<>8__locals1.originFleetTrajectory, tidateTime39, arrivalTime);
						}
						ValueTuple<TransferResult, Trajectory_Patched, double> valueTuple7 = MasterTransferPlanner.OptimizeLaunchTime(tidateTime38, tidateTime39, arrivalTime, arrivalTime, lockedLaunchTime, CS$<>8__locals1.param, "HybridSourceIsCommonDestinationIsCommonAndAsset", delegate(TIDateTime givenLaunchTime, TIDateTime givenArrivalTime, double stepSize_s0)
						{
							if (CS$<>8__locals1.param.aerobreaking || CS$<>8__locals1.param.unsafeAerobreaking)
							{
								ValueTuple<AerobreakInfo, AerobreakInfo> valueTuple12;
								MasterTransferPlanner.AerobreakPrecalculator(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime, out valueTuple12, false, 0.0);
							}
							ITransferTarget destinationValue3 = CS$<>8__locals1.param.destinationValue;
							TISpaceFleetState originFleet4 = CS$<>8__locals1.originFleet;
							OrbitalElementsState item3 = Trajectory.GetDestinationLocalOrbitalElementsAtTime(destinationValue3, (originFleet4 != null) ? originFleet4.faction : null, givenArrivalTime, CS$<>8__locals1.param.now, 0.0).Item1;
							double num94 = item3.MeanAnomalyAtTime_Rad(givenArrivalTime.ExportTime(), commonBarycenter.mass_kg);
							return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num94, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime), num94, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime));
						});
						if (CS$<>8__locals1.param.stopOnFirstSuccess && valueTuple7.Item1.Result == TransferResult.Outcome.Success)
						{
							return new ValueTuple<TransferResult, Trajectory_Patched>(valueTuple7.Item1, valueTuple7.Item2);
						}
						transferResult5 = valueTuple7.Item1;
						valueTuple2 = new ValueTuple<Trajectory_Patched, double>(valueTuple7.Item2, valueTuple7.Item3);
					}
					else
					{
						arrivalTime.DifferenceInSeconds(TITimeState.Now());
						TIDateTime tidateTime40;
						TIDateTime tidateTime41;
						TIDateTime tidateTime42;
						if (!CS$<>8__locals1.destinationIsTransferingFleet)
						{
							ValueTuple<TIDateTime, TIDateTime, TIDateTime> valueTuple8 = MasterTransferPlanner.CalculateLaunchTiming(CS$<>8__locals1.param, arrivalTime, commonBarycenter, CS$<>8__locals1.hybridTransferType);
							tidateTime40 = valueTuple8.Item1;
							tidateTime41 = valueTuple8.Item2;
							tidateTime42 = valueTuple8.Item3;
						}
						else
						{
							ValueTuple<TIDateTime, TIDateTime, TIDateTime> valueTuple8 = MasterTransferPlanner.CalculateLaunchTimingWhenDestinationIsTransferingFleet(CS$<>8__locals1.param, arrivalTime, commonBarycenter, CS$<>8__locals1.destFinalFleet);
							tidateTime40 = valueTuple8.Item1;
							tidateTime41 = valueTuple8.Item2;
							tidateTime42 = valueTuple8.Item3;
						}
						if (CS$<>8__locals1.originFleetIsInTransfer)
						{
							tidateTime41 = MasterTransferPlanner.BestLaunchTimeFromActiveTrajectory(CS$<>8__locals1.originFleetTrajectory, tidateTime40, arrivalTime);
						}
						ValueTuple<double, double, bool> finalInspiral2;
						if (CS$<>8__locals1.destinationIsTransferingFleet)
						{
							finalInspiral2 = MasterTransferPlanner.TerminalSpiralAnomaly_Rad(CS$<>8__locals1.destFinalFleet.Item1, CS$<>8__locals1.destFinalFleet.Item2, CS$<>8__locals1.param.fleetAcceleration_mps2);
						}
						else
						{
							finalInspiral2 = MasterTransferPlanner.TerminalSpiralAnomaly_Rad(CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.fleetAcceleration_mps2);
						}
						ValueTuple<TransferResult, Trajectory_Patched, double> valueTuple9 = MasterTransferPlanner.OptimizeLaunchTime(tidateTime41, tidateTime40, tidateTime42, arrivalTime, lockedLaunchTime, CS$<>8__locals1.param, "HybridSourceIsCommonDestinationIsn't", delegate(TIDateTime givenLaunchTime, TIDateTime givenArrivalTime, double stepSize_s)
						{
							if (CS$<>8__locals1.param.aerobreaking || CS$<>8__locals1.param.unsafeAerobreaking)
							{
								ValueTuple<AerobreakInfo, AerobreakInfo> valueTuple13;
								if (MasterTransferPlanner.AerobreakPrecalculator(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime, out valueTuple13, false, 0.0))
								{
									string text7 = "aerobreak time: ";
									AerobreakInfo item4 = valueTuple13.Item1;
									string text8;
									if (item4 == null)
									{
										text8 = null;
									}
									else
									{
										TIDateTime arrivalTime25 = item4.arrivalTime;
										text8 = ((arrivalTime25 != null) ? arrivalTime25.ToString() : null);
									}
									Log.Debug(text7 + text8, Array.Empty<object>());
								}
								else
								{
									Log.Debug("will not aerobreak", Array.Empty<object>());
								}
							}
							if (CS$<>8__locals1.isDestinationOrbit && CS$<>8__locals1.param.destinationValue.period_days() * 86400.0 < stepSize_s)
							{
								double num95 = CS$<>8__locals1.param.destinationValue.common_M_rad(CS$<>8__locals1.param.destinationValue.barycenter(), givenArrivalTime);
								return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num95, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime), num95, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime));
							}
							if (CS$<>8__locals1.destinationIsTransferingFleet && CS$<>8__locals1.targetFleet.trajectory.launchTime < givenArrivalTime && CS$<>8__locals1.targetFleet.trajectory.arrivalTime > givenArrivalTime)
							{
								OrbitalElementsState orbitalElementsState11;
								bool flag8;
								CS$<>8__locals1.targetFleet.getOrbitalElementsState(givenArrivalTime, out orbitalElementsState11, out destinationBarycenter, out flag8);
								double num96 = orbitalElementsState11.MeanAnomalyAtTime_Rad(givenArrivalTime.ExportTime(), destinationBarycenter.mass_kg);
								return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num96, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime), num96, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime));
							}
							MasterTransferPlanner.SimplifiedPositions simplifiedPositions4 = MasterTransferPlanner.GetSimplifiedPositions(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, givenLaunchTime, givenArrivalTime);
							double num97;
							double num98;
							if (finalInspiral2.Item3 && simplifiedPositions4.destinationLocalBarycenter != simplifiedPositions4.commonBarycenter)
							{
								bool flag9 = destinationBarycenter.barycenter == commonBarycenter && simplifiedPositions4.destinationDistToCommonBarycenter_m < simplifiedPositions4.originDistToCommonBarycenter_m;
								num97 = MasterTransferPlanner.GetMeanAnomalyWhenFurthestFromOrClosestToParentBarycenter(CS$<>8__locals1.param.destinationValue, givenArrivalTime, !flag9, CS$<>8__locals1.isPlayer);
								num97 += finalInspiral2.Item1;
								num98 = num97;
							}
							else
							{
								num97 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForLambert(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime).Item2, CS$<>8__locals1.param.destinationValue, givenArrivalTime, CS$<>8__locals1.isPlayer);
								num97 += finalInspiral2.Item1;
								bool flag8;
								OrbitalElementsState orbitalElementsState12;
								TINaturalSpaceObjectState tinaturalSpaceObjectState13;
								CS$<>8__locals1.param.fleet.getOrbitalElementsState(givenLaunchTime, out orbitalElementsState12, out tinaturalSpaceObjectState13, out flag8);
								OrbitalElementsState item5 = Trajectory.GetDestinationLocalOrbitalElementsAtTime(CS$<>8__locals1.param.destinationValue, CS$<>8__locals1.param.fleet.faction, arrivalTime, CS$<>8__locals1.param.now, 0.0).Item1;
								item5.semiMajorAxis_m = finalInspiral2.Item2;
								num98 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(orbitalElementsState12, tinaturalSpaceObjectState13, item5, destinationBarycenter, commonBarycenter, givenLaunchTime, givenArrivalTime).Item2, item5, destinationBarycenter.barycenter, givenArrivalTime, CS$<>8__locals1.isPlayer);
								num98 += finalInspiral2.Item1;
							}
							if (CS$<>8__locals1.isDestinationOrbit)
							{
								return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num97, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime), num98, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime));
							}
							TIDateTime tidateTime52 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenArrivalTime, num97, CS$<>8__locals1.param.destinationValue);
							TIDateTime tidateTime53 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenArrivalTime, num98, CS$<>8__locals1.param.destinationValue);
							tidateTime52 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime52, givenLaunchTime, CS$<>8__locals1.param.destinationValue.period_days() * 86400.0);
							tidateTime53 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime53, givenLaunchTime, CS$<>8__locals1.param.destinationValue.period_days() * 86400.0);
							if (tidateTime52.DifferenceInSeconds(givenArrivalTime) > stepSize_s)
							{
								tidateTime52 = new TIDateTime(givenArrivalTime);
							}
							if (tidateTime53.DifferenceInSeconds(givenArrivalTime) > stepSize_s)
							{
								tidateTime53 = new TIDateTime(givenArrivalTime);
							}
							return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num97, new TIDateTime(givenLaunchTime), tidateTime52, num98, new TIDateTime(givenLaunchTime), tidateTime53);
						});
						if (CS$<>8__locals1.param.stopOnFirstSuccess && valueTuple9.Item1.Result == TransferResult.Outcome.Success)
						{
							return new ValueTuple<TransferResult, Trajectory_Patched>(valueTuple9.Item1, valueTuple9.Item2);
						}
						transferResult5 = TransferResult.Best(transferResult5, valueTuple9.Item1);
						valueTuple2 = new ValueTuple<Trajectory_Patched, double>(valueTuple9.Item2, valueTuple9.Item3);
					}
				}
				else if (flag5)
				{
					ValueTuple<double, double, bool> initialOutspiral2 = new ValueTuple<double, double, bool>(0.0, 0.0, false);
					if (!simplifiedPositions2.originLocalBarycenter.isSun)
					{
						initialOutspiral2 = MasterTransferPlanner.TerminalSpiralAnomaly_Rad(simplifiedPositions2.originLocalBarycenter, simplifiedPositions2.originDistToLocalBarycenter_m, CS$<>8__locals1.param.fleetAcceleration_mps2);
					}
					bool flag6 = simplifiedPositions2.originLocalBarycenter.barycenter == commonBarycenter && simplifiedPositions2.originDistToCommonBarycenter_m < simplifiedPositions2.destinationDistToCommonBarycenter_m;
					double multispiralLaunchMeanAnomaly_Rad = (CS$<>8__locals1.originFleetIsInTransfer ? 0.0 : MasterTransferPlanner.GetMeanAnomalyWhenFurthestFromOrClosestToParentBarycenter(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.now, !flag6, CS$<>8__locals1.isPlayer));
					multispiralLaunchMeanAnomaly_Rad -= initialOutspiral2.Item1;
					if (CS$<>8__locals1.isDestinationOrbit)
					{
						List<TIDateTime> list3 = new List<TIDateTime>();
						if (CS$<>8__locals1.originFleetIsInTransfer)
						{
							list3.AddRange(MasterTransferPlanner.LaunchTimesToTestOnActiveTrajectory(CS$<>8__locals1.originFleetTrajectory, CS$<>8__locals1.param.now, arrivalTime));
						}
						else
						{
							list3.Add(new TIDateTime(CS$<>8__locals1.param.now));
						}
						using (List<TIDateTime>.Enumerator enumerator = list3.GetEnumerator())
						{
							Func<TIDateTime, TIDateTime, double, ValueTuple<TIDateTime, TIDateTime>> <>9__3;
							while (enumerator.MoveNext())
							{
								TIDateTime tidateTime43 = enumerator.Current;
								double num93 = MasterTransferPlanner.HohmannFirstBurnDuration_s(CS$<>8__locals1.param.fleetAcceleration_mps2, simplifiedPositions2.originDistToCommonBarycenter_m, simplifiedPositions2.destinationDistToCommonBarycenter_m, simplifiedPositions2.commonBarycenter.mu) * 2.0;
								tidateTime43.AddSeconds(num93);
								TIDateTime tidateTime44 = new TIDateTime(arrivalTime, num93);
								TIDateTime tidateTime45 = tidateTime44;
								MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param2 = CS$<>8__locals1.param;
								string text6 = "HybridSourceIsn'tCommonDestinationIsAndIsOrbit";
								Func<TIDateTime, TIDateTime, double, ValueTuple<TIDateTime, TIDateTime>> func;
								if ((func = <>9__3) == null)
								{
									func = (<>9__3 = delegate(TIDateTime givenLaunchTime, TIDateTime givenArrivalTime, double givenMeanAnomaly_Rad)
									{
										if (CS$<>8__locals1.param.aerobreaking || CS$<>8__locals1.param.unsafeAerobreaking)
										{
											ValueTuple<AerobreakInfo, AerobreakInfo> valueTuple14;
											MasterTransferPlanner.AerobreakPrecalculator(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime, out valueTuple14, false, 0.0);
										}
										if (sourceBarycenter.isLagrangePointState)
										{
											return new ValueTuple<TIDateTime, TIDateTime>(givenLaunchTime, givenLaunchTime);
										}
										if (CS$<>8__locals1.originFleetIsInTransfer)
										{
											return new ValueTuple<TIDateTime, TIDateTime>(givenLaunchTime, givenLaunchTime);
										}
										double num99;
										double num100;
										if (initialOutspiral2.Item3)
										{
											num99 = multispiralLaunchMeanAnomaly_Rad;
											num100 = multispiralLaunchMeanAnomaly_Rad;
										}
										else
										{
											num99 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForLambert(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime).Item1, CS$<>8__locals1.param.originValue, givenLaunchTime, CS$<>8__locals1.isPlayer);
											num99 -= initialOutspiral2.Item1;
											num100 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, commonBarycenter, givenLaunchTime, givenArrivalTime).Item1, CS$<>8__locals1.param.originValue, givenLaunchTime, CS$<>8__locals1.isPlayer);
											num100 -= initialOutspiral2.Item1;
										}
										double num101 = CS$<>8__locals1.param.originValue.period_days() * 86400.0;
										double num102 = givenArrivalTime.DifferenceInSeconds(givenLaunchTime) * 0.1;
										TIDateTime tidateTime54 = new TIDateTime(givenLaunchTime, num102);
										TIDateTime tidateTime55 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenLaunchTime, num99, CS$<>8__locals1.param.fleet.meanAnomaly_Rad(givenLaunchTime), CS$<>8__locals1.param.originValue.period_days());
										tidateTime55 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime55, givenLaunchTime, num101);
										if (tidateTime55 > tidateTime54)
										{
											if (num102 < num101 / 2.0)
											{
												tidateTime55 = givenLaunchTime;
											}
											else
											{
												tidateTime55 = tidateTime54;
											}
										}
										TIDateTime tidateTime56 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenLaunchTime, num100, CS$<>8__locals1.param.fleet.meanAnomaly_Rad(givenLaunchTime), CS$<>8__locals1.param.originValue.period_days());
										tidateTime56 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime56, givenLaunchTime, num101);
										if (tidateTime56 > tidateTime54)
										{
											if (num102 < num101 / 2.0)
											{
												tidateTime56 = givenLaunchTime;
											}
											else
											{
												tidateTime56 = tidateTime54;
											}
										}
										return new ValueTuple<TIDateTime, TIDateTime>(tidateTime55, tidateTime56);
									});
								}
								valueTuple2 = MasterTransferPlanner.OptimizeArrivalMeanAnomaly(tidateTime43, tidateTime45, param2, text6, func);
								if (CS$<>8__locals1.param.stopOnFirstSuccess && valueTuple2.Item1 != null && valueTuple2.Item2 <= CS$<>8__locals1.param.fleetDeltaV_mps)
								{
									return new ValueTuple<TransferResult, Trajectory_Patched>(new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0), valueTuple2.Item1);
								}
							}
							goto IL_1BBE;
						}
					}
					TIDateTime tidateTime46;
					TIDateTime tidateTime47;
					TIDateTime tidateTime48;
					if (!CS$<>8__locals1.destinationIsTransferingFleet)
					{
						ValueTuple<TIDateTime, TIDateTime, TIDateTime> valueTuple8 = MasterTransferPlanner.CalculateLaunchTiming(CS$<>8__locals1.param, arrivalTime, commonBarycenter, CS$<>8__locals1.hybridTransferType);
						tidateTime46 = valueTuple8.Item1;
						tidateTime47 = valueTuple8.Item2;
						tidateTime48 = valueTuple8.Item3;
					}
					else
					{
						ValueTuple<TIDateTime, TIDateTime, TIDateTime> valueTuple8 = MasterTransferPlanner.CalculateLaunchTimingWhenDestinationIsTransferingFleet(CS$<>8__locals1.param, arrivalTime, commonBarycenter, CS$<>8__locals1.destFinalFleet);
						tidateTime46 = valueTuple8.Item1;
						tidateTime47 = valueTuple8.Item2;
						tidateTime48 = valueTuple8.Item3;
					}
					if (CS$<>8__locals1.originFleetIsInTransfer)
					{
						tidateTime47 = MasterTransferPlanner.BestLaunchTimeFromActiveTrajectory(CS$<>8__locals1.originFleetTrajectory, tidateTime46, arrivalTime);
					}
					ValueTuple<TransferResult, Trajectory_Patched, double> valueTuple10 = MasterTransferPlanner.OptimizeLaunchTime(tidateTime47, tidateTime46, tidateTime48, arrivalTime, lockedLaunchTime, CS$<>8__locals1.param, "HybridSourceIsn'tCommonDestinationIsAndIsAsset", delegate(TIDateTime givenLaunchTime, TIDateTime givenArrivalTime, double stepSize_s)
					{
						if (CS$<>8__locals1.param.aerobreaking || CS$<>8__locals1.param.unsafeAerobreaking)
						{
							ValueTuple<AerobreakInfo, AerobreakInfo> valueTuple15;
							MasterTransferPlanner.AerobreakPrecalculator(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime, out valueTuple15, false, 0.0);
						}
						TISpaceFleetState tispaceFleetState5 = CS$<>8__locals1.param.destinationValue as TISpaceFleetState;
						double num103;
						if (tispaceFleetState5 != null && !MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState5, CS$<>8__locals1.param.fleet.faction))
						{
							num103 = tispaceFleetState5.meanAnomaly_Rad(arrivalTime);
						}
						else
						{
							num103 = CS$<>8__locals1.param.destinationValue.common_M_rad(destinationBarycenter, givenArrivalTime);
						}
						if (CS$<>8__locals1.originFleetIsInTransfer)
						{
							return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num103, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime), num103, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime));
						}
						if (stepSize_s < CS$<>8__locals1.param.originValue.period_days() * 86400.0)
						{
							return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num103, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime), num103, new TIDateTime(givenLaunchTime), new TIDateTime(givenArrivalTime));
						}
						double num104;
						double num105;
						if (initialOutspiral2.Item3)
						{
							num104 = multispiralLaunchMeanAnomaly_Rad;
							num105 = multispiralLaunchMeanAnomaly_Rad;
						}
						else
						{
							num104 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForLambert(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime).Item1, CS$<>8__locals1.param.originValue, givenLaunchTime, CS$<>8__locals1.isPlayer);
							num104 -= initialOutspiral2.Item1;
							num105 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, commonBarycenter, givenLaunchTime, givenArrivalTime).Item1, CS$<>8__locals1.param.originValue, givenLaunchTime, CS$<>8__locals1.isPlayer);
						}
						TIDateTime tidateTime57 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenLaunchTime, num104, CS$<>8__locals1.param.fleet.meanAnomaly_Rad(givenLaunchTime), CS$<>8__locals1.param.originValue.period_days());
						tidateTime57 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime57, givenLaunchTime, CS$<>8__locals1.param.originValue.period_days() * 86400.0);
						TIDateTime tidateTime58 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenLaunchTime, num105, CS$<>8__locals1.param.fleet.meanAnomaly_Rad(givenLaunchTime), CS$<>8__locals1.param.originValue.period_days());
						tidateTime58 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime58, givenLaunchTime, CS$<>8__locals1.param.originValue.period_days() * 86400.0);
						if (tidateTime57.DifferenceInSeconds(givenLaunchTime) > stepSize_s)
						{
							tidateTime57 = new TIDateTime(givenLaunchTime);
						}
						if (tidateTime58.DifferenceInSeconds(givenLaunchTime) > stepSize_s)
						{
							tidateTime58 = new TIDateTime(givenLaunchTime);
						}
						return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num103, tidateTime57, new TIDateTime(givenArrivalTime), num103, tidateTime58, new TIDateTime(givenArrivalTime));
					});
					if (CS$<>8__locals1.param.stopOnFirstSuccess && valueTuple10.Item1.Result == TransferResult.Outcome.Success)
					{
						return new ValueTuple<TransferResult, Trajectory_Patched>(valueTuple10.Item1, valueTuple10.Item2);
					}
					transferResult5 = TransferResult.Best(transferResult5, valueTuple10.Item1);
					valueTuple2 = new ValueTuple<Trajectory_Patched, double>(valueTuple10.Item2, valueTuple10.Item3);
				}
				else
				{
					TIDateTime tidateTime49;
					TIDateTime tidateTime50;
					TIDateTime tidateTime51;
					if (!CS$<>8__locals1.destinationIsTransferingFleet)
					{
						ValueTuple<TIDateTime, TIDateTime, TIDateTime> valueTuple8 = MasterTransferPlanner.CalculateLaunchTiming(CS$<>8__locals1.param, arrivalTime, commonBarycenter, CS$<>8__locals1.hybridTransferType);
						tidateTime49 = valueTuple8.Item1;
						tidateTime50 = valueTuple8.Item2;
						tidateTime51 = valueTuple8.Item3;
					}
					else
					{
						ValueTuple<TIDateTime, TIDateTime, TIDateTime> valueTuple8 = MasterTransferPlanner.CalculateLaunchTimingWhenDestinationIsTransferingFleet(CS$<>8__locals1.param, arrivalTime, commonBarycenter, CS$<>8__locals1.destFinalFleet);
						tidateTime49 = valueTuple8.Item1;
						tidateTime50 = valueTuple8.Item2;
						tidateTime51 = valueTuple8.Item3;
					}
					if (CS$<>8__locals1.originFleetIsInTransfer)
					{
						tidateTime50 = MasterTransferPlanner.BestLaunchTimeFromActiveTrajectory(CS$<>8__locals1.originFleetTrajectory, tidateTime49, arrivalTime);
					}
					ValueTuple<double, double, bool> initialOutspiral = new ValueTuple<double, double, bool>(0.0, 0.0, false);
					if (!simplifiedPositions2.originLocalBarycenter.isSun)
					{
						initialOutspiral = MasterTransferPlanner.TerminalSpiralAnomaly_Rad(simplifiedPositions2.originLocalBarycenter, simplifiedPositions2.originDistToLocalBarycenter_m, CS$<>8__locals1.param.fleetAcceleration_mps2);
					}
					ValueTuple<double, double, bool> finalInspiral = new ValueTuple<double, double, bool>(0.0, 0.0, false);
					if (!simplifiedPositions2.destinationLocalBarycenter.isSun)
					{
						finalInspiral = MasterTransferPlanner.TerminalSpiralAnomaly_Rad(simplifiedPositions2.destinationLocalBarycenter, simplifiedPositions2.destinationDistToLocalBarycenter_m, CS$<>8__locals1.param.fleetAcceleration_mps2);
					}
					ValueTuple<TransferResult, Trajectory_Patched, double> valueTuple11 = MasterTransferPlanner.OptimizeLaunchTime(tidateTime50, tidateTime49, tidateTime51, arrivalTime, lockedLaunchTime, CS$<>8__locals1.param, "HybridSourceAndDestinationAren'tCommon", delegate(TIDateTime givenLaunchTime, TIDateTime givenArrivalTime, double stepSize_s)
					{
						if (CS$<>8__locals1.param.aerobreaking || CS$<>8__locals1.param.unsafeAerobreaking)
						{
							ValueTuple<AerobreakInfo, AerobreakInfo> valueTuple16;
							MasterTransferPlanner.AerobreakPrecalculator(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime, out valueTuple16, false, 0.0);
						}
						double num106 = 0.0;
						double num107 = 0.0;
						bool flag10 = CS$<>8__locals1.param.originValue.period_days() * 86400.0 < stepSize_s && !CS$<>8__locals1.originFleetIsInTransfer;
						bool flag11 = CS$<>8__locals1.param.destinationValue.period_days() * 86400.0 < stepSize_s && (!CS$<>8__locals1.destinationIsTransferingFleet || !(CS$<>8__locals1.targetFleet.trajectory.launchTime < givenArrivalTime) || !(CS$<>8__locals1.targetFleet.trajectory.arrivalTime > givenArrivalTime));
						MasterTransferPlanner.SimplifiedPositions simplifiedPositions5 = MasterTransferPlanner.GetSimplifiedPositions(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, givenLaunchTime, givenArrivalTime);
						if (initialOutspiral.Item3 && simplifiedPositions5.originLocalBarycenter != simplifiedPositions5.commonBarycenter)
						{
							bool flag12 = simplifiedPositions5.originLocalBarycenter.barycenter == simplifiedPositions5.commonBarycenter && simplifiedPositions5.originDistToCommonBarycenter_m < simplifiedPositions5.destinationDistToCommonBarycenter_m;
							num106 = MasterTransferPlanner.GetMeanAnomalyWhenFurthestFromOrClosestToParentBarycenter(CS$<>8__locals1.param.originValue, givenLaunchTime, flag12, CS$<>8__locals1.isPlayer);
							num106 -= initialOutspiral.Item1;
							num107 = num106;
						}
						double num108 = 0.0;
						double num109 = 0.0;
						if (finalInspiral.Item3 && simplifiedPositions5.destinationLocalBarycenter != simplifiedPositions5.commonBarycenter)
						{
							bool flag13 = simplifiedPositions5.destinationLocalBarycenter.barycenter == simplifiedPositions5.commonBarycenter && simplifiedPositions5.destinationDistToCommonBarycenter_m < simplifiedPositions5.originDistToCommonBarycenter_m;
							num108 = MasterTransferPlanner.GetMeanAnomalyWhenFurthestFromOrClosestToParentBarycenter(CS$<>8__locals1.param.destinationValue, givenArrivalTime, !flag13, CS$<>8__locals1.isPlayer);
							num108 -= finalInspiral.Item1;
							num109 = num108;
						}
						if (!initialOutspiral.Item3 || !finalInspiral.Item3)
						{
							ValueTuple<Vector3d, Vector3d> valueTuple17 = MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForLambert(CS$<>8__locals1.param, givenLaunchTime, givenArrivalTime);
							ValueTuple<Vector3d, Vector3d> valueTuple18 = MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(CS$<>8__locals1.param.originValue, CS$<>8__locals1.param.destinationValue, commonBarycenter, givenLaunchTime, givenArrivalTime);
							if (!initialOutspiral.Item3 && !CS$<>8__locals1.originFleetIsInTransfer)
							{
								num106 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(valueTuple17.Item1, CS$<>8__locals1.param.originValue, givenLaunchTime, CS$<>8__locals1.isPlayer);
								num106 -= initialOutspiral.Item1;
								num107 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(valueTuple18.Item1, CS$<>8__locals1.param.originValue, givenLaunchTime, CS$<>8__locals1.isPlayer);
								num107 -= initialOutspiral.Item1;
							}
							if (!finalInspiral.Item3 && !CS$<>8__locals1.destinationIsTransferingFleet)
							{
								num108 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(valueTuple17.Item2, CS$<>8__locals1.param.destinationValue, givenArrivalTime, CS$<>8__locals1.isPlayer);
								num108 -= finalInspiral.Item1;
								num109 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(valueTuple18.Item2, CS$<>8__locals1.param.destinationValue, givenArrivalTime, CS$<>8__locals1.isPlayer);
								num109 -= finalInspiral.Item1;
							}
						}
						TIDateTime tidateTime59;
						TIDateTime tidateTime60;
						if (flag10)
						{
							tidateTime59 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenLaunchTime, num106, CS$<>8__locals1.param.fleet.meanAnomaly_Rad(givenLaunchTime), CS$<>8__locals1.param.originValue.period_days());
							tidateTime59 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime59, givenLaunchTime, CS$<>8__locals1.param.originValue.period_days() * 86400.0);
							tidateTime60 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenLaunchTime, num107, CS$<>8__locals1.param.fleet.meanAnomaly_Rad(givenLaunchTime), CS$<>8__locals1.param.originValue.period_days());
							tidateTime60 = MasterTransferPlanner.AdvanceTimePastDeadlineInIncrements(tidateTime60, givenLaunchTime, CS$<>8__locals1.param.originValue.period_days() * 86400.0);
							if (tidateTime59.DifferenceInSeconds(givenLaunchTime) > stepSize_s)
							{
								tidateTime59 = new TIDateTime(givenLaunchTime);
							}
							if (tidateTime60.DifferenceInSeconds(givenLaunchTime) > stepSize_s)
							{
								tidateTime60 = new TIDateTime(givenLaunchTime);
							}
						}
						else
						{
							tidateTime59 = new TIDateTime(givenLaunchTime);
							tidateTime60 = new TIDateTime(givenLaunchTime);
						}
						TIDateTime tidateTime61 = new TIDateTime(givenArrivalTime);
						TIDateTime tidateTime62 = new TIDateTime(givenArrivalTime);
						if (!CS$<>8__locals1.isDestinationOrbit && flag11)
						{
							tidateTime61 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenArrivalTime, num108, CS$<>8__locals1.param.destinationValue);
							tidateTime62 = MasterTransferPlanner.TimeWhenAtMeanAnomaly(givenArrivalTime, num109, CS$<>8__locals1.param.destinationValue);
							if (tidateTime61.DifferenceInSeconds(givenArrivalTime) > stepSize_s)
							{
								tidateTime61 = new TIDateTime(givenArrivalTime);
							}
							if (tidateTime62.DifferenceInSeconds(givenArrivalTime) > stepSize_s)
							{
								tidateTime62 = new TIDateTime(givenArrivalTime);
							}
						}
						return new ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>(num108, tidateTime59, tidateTime61, num109, tidateTime60, tidateTime62);
					});
					if (CS$<>8__locals1.param.stopOnFirstSuccess && valueTuple11.Item1.Result == TransferResult.Outcome.Success)
					{
						return new ValueTuple<TransferResult, Trajectory_Patched>(valueTuple11.Item1, valueTuple11.Item2);
					}
					transferResult5 = TransferResult.Best(transferResult5, valueTuple11.Item1);
					valueTuple2 = new ValueTuple<Trajectory_Patched, double>(valueTuple11.Item2, valueTuple11.Item3);
				}
				IL_1BBE:
				bool flag7 = true;
				if (valueTuple2.Item1 != null)
				{
					TIDateTime arrivalTime24 = valueTuple2.Item1.arrivalTime;
					if (CS$<>8__locals1.destinationIsTransferingFleet && arrivalTime24 > CS$<>8__locals1.targetFleet.trajectory.launchTime)
					{
						destinationBarycenter = CS$<>8__locals1.targetFleet.trajectory.GetBarycenterAtTime(arrivalTime24);
						double magnitude3 = (CS$<>8__locals1.targetFleet.trajectory.ToGlobalCartesianStateAtTime(arrivalTime24).position - destinationBarycenter.ToGlobalCartesianStateAtTime(arrivalTime24).position).magnitude;
						MicrothrustSphere microthrustSphere8 = new MicrothrustSphere(CS$<>8__locals1.param.fleetAcceleration_mps2, destinationBarycenter.mu, destinationBarycenter.sphereOfInfluence_m);
						if (microthrustSphere8.IsLimitedBySphereOfInfluence || (microthrustSphere8.Radius_m > magnitude3 && arrivalTime24 < CS$<>8__locals1.targetFleet.trajectory.arrivalTime))
						{
							flag7 = false;
							transferResult5 = TransferResult.Best(transferResult5, new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptInMicrothrust, destinationBarycenter.mu, magnitude3));
						}
					}
				}
				if (flag7 && valueTuple2.Item1 != null && valueTuple2.Item1.DV_mps > CS$<>8__locals1.param.fleetDeltaV_mps)
				{
					transferResult5 = TransferResult.Best(transferResult5, new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple2.Item1.DV_mps, 0.0));
				}
				return new ValueTuple<TransferResult, Trajectory_Patched>(transferResult5, valueTuple2.Item1);
			}));
			lowestDVFound_kps = CS$<>8__locals1.lowestDVFound_mps / 1000.0;
			if (candidateTrajectories.Count > count)
			{
				return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
			}
			if (CS$<>8__locals1.bestResult.Result == TransferResult.Outcome.Success)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			return CS$<>8__locals1.bestResult;
		}

		// Token: 0x06003DDF RID: 15839 RVA: 0x00188B78 File Offset: 0x00186D78
		public static void GetOriginOrbitalElementsState(ITransferTarget originValue, TIDateTime time, out OrbitalElementsState orbitalElements, out TINaturalSpaceObjectState barycenter, out bool meanAnomalyIsGood)
		{
			TISpaceFleetState tispaceFleetState = originValue as TISpaceFleetState;
			if (tispaceFleetState == null || !tispaceFleetState.transferAssigned || !(tispaceFleetState.trajectory.arrivalTime < time) || !(tispaceFleetState.trajectory.destinationFleet != null))
			{
				originValue.getOrbitalElementsState(time, out orbitalElements, out barycenter, out meanAnomalyIsGood);
				return;
			}
			if (!(tispaceFleetState.trajectory.destinationOrbit != null))
			{
				orbitalElements = tispaceFleetState.trajectory.GetOrbitalElementsAtTime(tispaceFleetState.trajectory.arrivalTime);
				barycenter = tispaceFleetState.trajectory.GetBarycenterAtTime(tispaceFleetState.trajectory.arrivalTime);
				meanAnomalyIsGood = true;
				return;
			}
			if (tispaceFleetState.trajectory.destinationOrbitMeanAnomalyAtEpoch != null && tispaceFleetState.trajectory.destinationOrbitEpoch != null)
			{
				orbitalElements = tispaceFleetState.trajectory.destinationOrbit.ToOrbitalElementsState(tispaceFleetState.trajectory.destinationOrbitEpoch, tispaceFleetState.trajectory.destinationOrbitMeanAnomalyAtEpoch.Value);
				barycenter = tispaceFleetState.trajectory.destinationOrbit.barycenter;
				meanAnomalyIsGood = true;
				return;
			}
			Vector3d position = tispaceFleetState.trajectory.ToGlobalCartesianStateAtTime(tispaceFleetState.trajectory.arrivalTime).ToLocal(tispaceFleetState.trajectory.destinationOrbit.barycenter, tispaceFleetState.trajectory.arrivalTime).position;
			double num = TISpaceAssetState.CalculateMeanAnomalyFromPosition(tispaceFleetState.trajectory.destinationOrbit, position, tispaceFleetState.trajectory.arrivalTime, tispaceFleetState.faction.isActivePlayer);
			orbitalElements = tispaceFleetState.trajectory.destinationOrbit.ToOrbitalElementsState(tispaceFleetState.trajectory.arrivalTime, num);
			barycenter = tispaceFleetState.trajectory.destinationOrbit.barycenter;
			meanAnomalyIsGood = true;
		}

		// Token: 0x06003DE0 RID: 15840 RVA: 0x00188D34 File Offset: 0x00186F34
		private static List<TIDateTime> FindFutureApsidesTimes(Trajectory trajectory, TIDateTime earliestTime, TIDateTime latestTime)
		{
			List<TIDateTime> list = new List<TIDateTime>();
			Trajectory_Patched trajectory_Patched = trajectory as Trajectory_Patched;
			if (trajectory_Patched != null)
			{
				for (int i = 0; i < trajectory_Patched.Segments.Count; i++)
				{
					Trajectory_Patched.IPatchSegment patchSegment = trajectory_Patched.Segments[i];
					if (patchSegment.startTime > latestTime)
					{
						break;
					}
					if (patchSegment is Trajectory_Patched.OrbitLERPSegment || patchSegment is Trajectory_Patched.OrbitSegment)
					{
						TIDateTime tidateTime = ((trajectory_Patched.Segments.Count > i + 1) ? trajectory_Patched.Segments[i + 1].startTime : trajectory_Patched.arrivalTime);
						TIDateTime tidateTime2 = new TIDateTime(patchSegment.startTime, tidateTime.DifferenceInSeconds(patchSegment.startTime) / 2.0);
						OrbitalElementsState orbitalElementsState = patchSegment.OrbitalElementsAtTime(tidateTime2);
						tidateTime = TIDateTime.Min(tidateTime, latestTime);
						TIDateTime tidateTime3 = TIDateTime.Min(patchSegment.startTime, earliestTime);
						TIDateTime tidateTime4 = new TIDateTime(orbitalElementsState.NextTimeAtMeanAnomaly(0.0, tidateTime3.ExportTime(), patchSegment.barycenter.mass_kg));
						if (orbitalElementsState.eccentricity < 1.0)
						{
							TIDateTime tidateTime5 = new TIDateTime(tidateTime4, -orbitalElementsState.OrbitalPeriod(patchSegment.barycenter.mass_kg) / 2.0);
							if (tidateTime5 > tidateTime3)
							{
								tidateTime4 = tidateTime5;
							}
						}
						if (tidateTime4 < tidateTime && tidateTime4 > tidateTime3)
						{
							list.Add(tidateTime4);
							if (list.Count >= 10)
							{
								break;
							}
							if (orbitalElementsState.eccentricity < 1.0)
							{
								double num = orbitalElementsState.OrbitalPeriod(patchSegment.barycenter.mass_kg) / 2.0;
								while (list.Count < 10)
								{
									tidateTime4 = new TIDateTime(tidateTime4, num);
									if (tidateTime4 >= tidateTime)
									{
										break;
									}
									list.Add(tidateTime4);
								}
							}
						}
					}
				}
			}
			if (list.Count < 10 && trajectory.arrivalTime < latestTime && !trajectory.destroyOnArrival && trajectory.targetingOrbit)
			{
				TIOrbitState destinationOrbit = trajectory.destinationOrbit;
				OrbitalElementsState orbitalElementsState2 = destinationOrbit.ToOrbitalElementsState(trajectory.arrivalTime, 0.0);
				Vector3d position = trajectory.ToGlobalCartesianStateAtTime(trajectory.arrivalTime).ToLocal(destinationOrbit.barycenter, trajectory.arrivalTime).position;
				double num2 = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState2, destinationOrbit.barycenter, position, trajectory.arrivalTime, TISpaceAssetState.MeanAnomalyPrecision.Player);
				orbitalElementsState2.meanAnomalyAtEpoch_Rad = num2;
				TIDateTime tidateTime6 = new TIDateTime(orbitalElementsState2.NextTimeAtMeanAnomaly(0.0, trajectory.arrivalTime.ExportTime(), destinationOrbit.barycenter.mass_kg));
				double num3 = orbitalElementsState2.OrbitalPeriod(destinationOrbit.barycenter.mass_kg) / 2.0;
				TIDateTime tidateTime7 = new TIDateTime(tidateTime6, -num3);
				if (tidateTime7 >= trajectory.arrivalTime)
				{
					tidateTime6 = tidateTime7;
				}
				if (tidateTime6 < latestTime)
				{
					list.Add(tidateTime6);
					while (list.Count < 10)
					{
						tidateTime6 = new TIDateTime(tidateTime6, num3);
						if (tidateTime6 >= latestTime)
						{
							break;
						}
						list.Add(tidateTime6);
					}
				}
			}
			return list;
		}

		// Token: 0x06003DE1 RID: 15841 RVA: 0x00189050 File Offset: 0x00187250
		[return: TupleElementNames(new string[] { "lambertAerobreak", "torchAerobreak" })]
		private static ValueTuple<AerobreakInfo, AerobreakInfo> AerocaptureToOrbit(MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, TIDateTime launchTime, TIDateTime proposedArrivalTime, TISpaceBodyState aerocaptureBarycenter, TINaturalSpaceObjectState commonBarycenter, CartesianState initialCartesian, CartesianState aerocaptureCartesian, OrbitalElementsState destinationOrbitalElements)
		{
			if (!aerocaptureBarycenter.supportsAerocapture)
			{
				return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
			}
			double num = aerocaptureBarycenter.meanRadius_m + aerocaptureBarycenter.template.atmosphereScaleHeight_km * 1000.0;
			if (aerocaptureBarycenter == commonBarycenter)
			{
				OrbitalElementsState orbitalElementsState = initialCartesian.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(launchTime.ExportTime()));
				if (orbitalElementsState.eccentricity >= 1.0)
				{
					if (Vector3d.Dot(in initialCartesian.position, in initialCartesian.velocity) > 0.0)
					{
						return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					}
				}
				else
				{
					if (initialCartesian.position.magnitude <= aerocaptureCartesian.position.magnitude)
					{
						return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					}
					if (!MasterTransferPlanner.isAerobreakingBetterThanHohmann(orbitalElementsState.semiMajorAxis_m, destinationOrbitalElements.semiMajorAxis_m, num))
					{
						return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					}
				}
			}
			LambertEquations lambertEquations = default(LambertEquations);
			LambertEquations lambertEquations2 = default(LambertEquations);
			double num2 = proposedArrivalTime.DifferenceInSeconds(launchTime);
			lambertEquations.SolveLambert(num2, initialCartesian, aerocaptureCartesian, commonBarycenter.mu, false, true);
			lambertEquations2.SolveLambert(num2, initialCartesian, aerocaptureCartesian, commonBarycenter.mu, false, true);
			Vector3d burn = ((lambertEquations.burn0.magnitude + lambertEquations.burn1.magnitude <= lambertEquations2.burn0.magnitude + lambertEquations2.burn1.magnitude) ? lambertEquations : lambertEquations2).burn1;
			double num3 = (num + destinationOrbitalElements.semiMajorAxis_m) / 2.0;
			double num4 = 3.141592653589793 * Mathd.Sqrt(num3 * num3 * num3 / aerocaptureBarycenter.mu);
			double num5 = Mathd.ClampRadiansTwoPI(MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(-burn, param.destinationValue, proposedArrivalTime, param.fleet.faction.isActivePlayer) + destinationOrbitalElements.argPeriapsis_Rad);
			OrbitalElementsState orbitalElementsState2 = new OrbitalElementsState
			{
				longAscendingNode_Rad = destinationOrbitalElements.longAscendingNode_Rad,
				inclination_Rad = destinationOrbitalElements.inclination_Rad,
				semiMajorAxis_m = num3,
				eccentricity = 1.0 - num / num3,
				meanAnomalyAtEpoch_Rad = 0.0,
				epoch = proposedArrivalTime.ExportTime(),
				argPeriapsis_Rad = num5
			};
			double num6 = Mathd.Sqrt(aerocaptureBarycenter.mu * (2.0 / destinationOrbitalElements.semiMajorAxis_m - 1.0 / num3));
			double num7 = (Mathd.Sqrt(aerocaptureBarycenter.mu / destinationOrbitalElements.semiMajorAxis_m) - num6) / (double)param.fleet.cruiseAcceleration_mps2;
			MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP = null;
			if (num7 > num4)
			{
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(param.fleetAcceleration_mps2, aerocaptureBarycenter.mu, aerocaptureBarycenter.sphereOfInfluence_m);
				double num8 = Mathd.Sqrt(aerocaptureBarycenter.mu / orbitalElementsState2.semiMajorAxis_m);
				double num9 = Mathd.Sqrt(aerocaptureBarycenter.mu / destinationOrbitalElements.semiMajorAxis_m);
				double num10 = Mathd.Abs(microthrustSphere.GetDuration_s(num9) - microthrustSphere.GetDuration_s(num8));
				Vector3d normalized = orbitalElementsState2.periapsisVector.normalized;
				Vector3d normalized2 = destinationOrbitalElements.periapsisVector.normalized;
				Vector3d vector3d = orbitalElementsState2.eccentricity * normalized;
				Vector3d vector3d2 = destinationOrbitalElements.eccentricity * normalized2;
				double magnitude = (vector3d - vector3d2).magnitude;
				double num11 = num10 * (1.0 + magnitude);
				double num12 = param.fleetAcceleration_mps2 * num10 / num11;
				MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(num12, aerocaptureBarycenter.mu, aerocaptureBarycenter.sphereOfInfluence_m);
				double num13 = (num8 - num9) / num12;
				double num14 = Mathd.Abs(microthrustSphere2.GetAnomalyDelta_Rad(num9) - microthrustSphere2.GetAnomalyDelta_Rad(num8));
				double num15 = orbitalElementsState2.longAscendingNode_Rad + orbitalElementsState2.argPeriapsis_Rad - destinationOrbitalElements.longAscendingNode_Rad - destinationOrbitalElements.argPeriapsis_Rad + num14;
				microthrustTransferSegmentLERP = new MicrothrustTransferSegmentLERP
				{
					startTime = proposedArrivalTime,
					endTime = new TIDateTime(proposedArrivalTime, num13),
					barycenter = aerocaptureBarycenter,
					trueFleetAcceleration_mps2 = param.fleetAcceleration_mps2,
					effectiveFleetAcceleration_mps2 = num12,
					start = new MicrothrustTransferLERPvalues(num3, orbitalElementsState2.meanAnomalyAtEpoch_Rad, orbitalElementsState2.eccentricity, orbitalElementsState2.longAscendingNode_Rad, orbitalElementsState2.inclination_Rad, orbitalElementsState2.argPeriapsis_Rad, 0.0, 0.0, 0.0),
					end = new MicrothrustTransferLERPvalues(destinationOrbitalElements.semiMajorAxis_m, num15, destinationOrbitalElements.eccentricity, destinationOrbitalElements.longAscendingNode_Rad, destinationOrbitalElements.inclination_Rad, destinationOrbitalElements.argPeriapsis_Rad, 0.0, 0.0, 0.0)
				};
			}
			AerobreakInfo aerobreakInfo = new AerobreakInfo();
			aerobreakInfo.barycenter = aerocaptureBarycenter;
			aerobreakInfo.hohmannOrbit = orbitalElementsState2;
			aerobreakInfo.aerobreakTime = proposedArrivalTime;
			aerobreakInfo.arrivalTime = new TIDateTime(proposedArrivalTime, num4);
			aerobreakInfo.microthrustSpiral = microthrustTransferSegmentLERP;
			TorchTransfer torchTransfer = new TorchTransfer();
			bool flag;
			torchTransfer.Solve(launchTime, num2, param.fleetAcceleration_mps2, initialCartesian, aerocaptureCartesian, commonBarycenter, param.fleetDeltaV_mps, out flag, true);
			double num16 = Mathd.ClampRadiansTwoPI(MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(-(torchTransfer.decelerationVector_mps2 * torchTransfer.decelDuration_s), param.destinationValue, proposedArrivalTime, param.fleet.faction.isActivePlayer) + destinationOrbitalElements.argPeriapsis_Rad);
			OrbitalElementsState orbitalElementsState3 = new OrbitalElementsState(orbitalElementsState2);
			orbitalElementsState3.argPeriapsis_Rad = num16;
			AerobreakInfo aerobreakInfo2 = aerobreakInfo.Copy();
			aerobreakInfo2.hohmannOrbit = orbitalElementsState3;
			if (aerobreakInfo2.microthrustSpiral != null)
			{
				aerobreakInfo2.microthrustSpiral.start.argPeriapsis_Rad = orbitalElementsState3.argPeriapsis_Rad;
				aerobreakInfo2.microthrustSpiral.end.meanAnomaly_Rad += orbitalElementsState3.argPeriapsis_Rad - orbitalElementsState2.argPeriapsis_Rad;
			}
			return new ValueTuple<AerobreakInfo, AerobreakInfo>(aerobreakInfo, aerobreakInfo2);
		}

		// Token: 0x06003DE2 RID: 15842 RVA: 0x001895F0 File Offset: 0x001877F0
		private static bool AerobreakPrecalculator(MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, TIDateTime launchTime, TIDateTime proposedArrivalTime, [TupleElementNames(new string[] { "lambertAerobreak", "torchAerobreak" })] out ValueTuple<AerobreakInfo, AerobreakInfo> aerobreakResults, bool useAerobreakMeanAnomaly_Rad = false, double aerobreakMeanAnomaly_Rad = 0.0)
		{
			CartesianState cartesianState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			param.fleet.tryToGetLocalCartesianState(launchTime, out cartesianState, out tinaturalSpaceObjectState);
			TISpaceFleetState tispaceFleetState = param.destinationValue as TISpaceFleetState;
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState2;
			bool flag;
			if (tispaceFleetState != null && MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, param.fleet.faction))
			{
				tispaceFleetState.getOrbitalElementsState(proposedArrivalTime, out orbitalElementsState, out tinaturalSpaceObjectState2, out flag);
			}
			else
			{
				param.destinationValue.getOrbitalElementsState(param.now, out orbitalElementsState, out tinaturalSpaceObjectState2, out flag);
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState3 = tinaturalSpaceObjectState.FindCommonBarycenter(tinaturalSpaceObjectState2);
			TIOrbitState tiorbitState = null;
			TISpaceBodyState tispaceBodyState2;
			OrbitalElementsState orbitalElementsState2;
			if (tinaturalSpaceObjectState2.supportsAerocapture)
			{
				TISpaceBodyState tispaceBodyState = tinaturalSpaceObjectState2 as TISpaceBodyState;
				if (tispaceBodyState != null)
				{
					tispaceBodyState2 = tispaceBodyState;
					orbitalElementsState2 = default(OrbitalElementsState);
					goto IL_00FE;
				}
			}
			TISpaceBodyState tispaceBodyState3 = tinaturalSpaceObjectState2.barycenter as TISpaceBodyState;
			if (tispaceBodyState3 == null || !tispaceBodyState3.supportsAerocapture)
			{
				aerobreakResults = new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
				return false;
			}
			tispaceBodyState2 = tispaceBodyState3;
			if (flag)
			{
				orbitalElementsState2 = orbitalElementsState;
			}
			else
			{
				tiorbitState = param.destinationValue as TIOrbitState;
				orbitalElementsState2 = default(OrbitalElementsState);
				if (tiorbitState == null)
				{
					Log.Error("AerobreakPrecalculator: destination has no mean anomaly around a moon, but also isn't an orbit state.  Aborting aerobreak.", Array.Empty<object>());
					aerobreakResults = new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					return false;
				}
			}
			IL_00FE:
			CartesianState cartesianState2;
			if (tinaturalSpaceObjectState == tinaturalSpaceObjectState3)
			{
				cartesianState2 = cartesianState;
			}
			else if (tinaturalSpaceObjectState.barycenter == tinaturalSpaceObjectState3)
			{
				cartesianState2 = tinaturalSpaceObjectState.ToLocalCartesianStateAtTime(launchTime);
			}
			else
			{
				if (tinaturalSpaceObjectState.barycenter == null)
				{
					Log.Error("AerobreakPrecalculator: common barycenter was not common to origin.  Aborting aerocapture.", Array.Empty<object>());
					aerobreakResults = new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					return false;
				}
				cartesianState2 = tinaturalSpaceObjectState.barycenter.ToLocalCartesianStateAtTime(launchTime);
			}
			CartesianState cartesianState3;
			if (tispaceBodyState2 == tinaturalSpaceObjectState3)
			{
				if (useAerobreakMeanAnomaly_Rad)
				{
					Vector3d position = orbitalElementsState.ToCartesianStateAtMeanAnomaly(aerobreakMeanAnomaly_Rad, tispaceBodyState2.mass_kg).position;
					double num = (tispaceBodyState2.meanRadius_km + tispaceBodyState2.template.atmosphereScaleHeight_km) * 1000.0;
					Vector3d vector3d = position.normalized * num;
					cartesianState3 = new CartesianState(vector3d, Vector3d.zero);
				}
				else if (tinaturalSpaceObjectState2.barycenter == tispaceBodyState2)
				{
					double num2 = (tinaturalSpaceObjectState2.semiMajorAxis_m + tispaceBodyState2.meanRadius_m + tispaceBodyState2.template.atmosphereScaleHeight_km * 1000.0) / 2.0;
					double num3 = 3.141592653589793 * Mathd.Sqrt(num2 * num2 * num2 / tispaceBodyState2.mu);
					TIDateTime tidateTime = new TIDateTime(proposedArrivalTime, num3);
					Vector3d vector3d2 = -tinaturalSpaceObjectState2.ToLocalCartesianStateAtTime(tidateTime).position.normalized * (tispaceBodyState2.meanRadius_m + tispaceBodyState2.template.atmosphereScaleHeight_km * 1000.0);
					cartesianState3 = new CartesianState(vector3d2, Vector3d.zero);
				}
				else
				{
					Vector3d vector3d3 = -cartesianState2.position;
					Vector3d normalVector = orbitalElementsState.normalVector;
					Vector3d normalized = (vector3d3 - normalVector * Vector3d.Dot(in vector3d3, in normalVector)).normalized;
					double num4 = (tispaceBodyState2.meanRadius_km + tispaceBodyState2.template.atmosphereScaleHeight_km) * 1000.0;
					Vector3d vector3d4 = normalized * num4;
					cartesianState3 = new CartesianState(vector3d4, Vector3d.zero);
				}
			}
			else if (tispaceBodyState2.barycenter == tinaturalSpaceObjectState3)
			{
				cartesianState3 = tispaceBodyState2.ToLocalCartesianStateAtTime(proposedArrivalTime);
			}
			else
			{
				if (tispaceBodyState2.barycenter == null)
				{
					Log.Error("AerobreakPrecalculator: common barycenter was not common to aerocapture barycenter.  Aborting aerocapture.", Array.Empty<object>());
					aerobreakResults = new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					return false;
				}
				cartesianState3 = tispaceBodyState2.barycenter.ToLocalCartesianStateAtTime(proposedArrivalTime);
			}
			if (tinaturalSpaceObjectState2 != tispaceBodyState2)
			{
				orbitalElementsState = new OrbitalElementsState(tinaturalSpaceObjectState2);
			}
			TIOrbitState tiorbitState2 = param.destinationValue as TIOrbitState;
			if (tiorbitState2 == null)
			{
				aerobreakResults = MasterTransferPlanner.AerocaptureToFixedMeanAnomaly(param, launchTime, proposedArrivalTime, tispaceBodyState2, tinaturalSpaceObjectState3, cartesianState2, cartesianState3, orbitalElementsState, tiorbitState, orbitalElementsState2);
				return aerobreakResults.Item1 != null || aerobreakResults.Item2 != null;
			}
			if (tinaturalSpaceObjectState2 == tispaceBodyState2)
			{
				aerobreakResults = MasterTransferPlanner.AerocaptureToOrbit(param, launchTime, proposedArrivalTime, tispaceBodyState2, tinaturalSpaceObjectState3, cartesianState2, cartesianState3, orbitalElementsState);
				return aerobreakResults.Item1 != null || aerobreakResults.Item2 != null;
			}
			aerobreakResults = MasterTransferPlanner.AerocaptureToFixedMeanAnomaly(param, launchTime, proposedArrivalTime, tispaceBodyState2, tinaturalSpaceObjectState3, cartesianState2, cartesianState3, new OrbitalElementsState(tinaturalSpaceObjectState2), tiorbitState2, default(OrbitalElementsState));
			return aerobreakResults.Item1 != null || aerobreakResults.Item2 != null;
		}

		// Token: 0x06003DE3 RID: 15843 RVA: 0x00189A18 File Offset: 0x00187C18
		[return: TupleElementNames(new string[] { "lambertAerobreak", "torchAerobreak" })]
		private static ValueTuple<AerobreakInfo, AerobreakInfo> AerocaptureToFixedMeanAnomaly(MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, TIDateTime launchTime, TIDateTime proposedArrivalTime, TISpaceBodyState aerocaptureBarycenter, TINaturalSpaceObjectState commonBarycenter, CartesianState initialCartesian, CartesianState aerocaptureCartesian, OrbitalElementsState destinationOrbitalElementsAroundAerocaptureBarycenter, TIOrbitState destinationOrbitAroundMoon = null, OrbitalElementsState destinationOrbitalElementsAroundMoon = default(OrbitalElementsState))
		{
			if (!aerocaptureBarycenter.supportsAerocapture)
			{
				return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
			}
			if (aerocaptureBarycenter == commonBarycenter)
			{
				OrbitalElementsState orbitalElementsState = initialCartesian.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(launchTime.ExportTime()));
				if (orbitalElementsState.eccentricity >= 1.0)
				{
					if (Vector3d.Dot(in initialCartesian.position, in initialCartesian.velocity) > 0.0)
					{
						return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					}
				}
				else
				{
					if (initialCartesian.position.magnitude <= aerocaptureCartesian.position.magnitude)
					{
						return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					}
					if (!MasterTransferPlanner.isAerobreakingBetterThanHohmann(orbitalElementsState.semiMajorAxis_m, destinationOrbitalElementsAroundAerocaptureBarycenter.semiMajorAxis_m, aerocaptureBarycenter.meanRadius_m))
					{
						return new ValueTuple<AerobreakInfo, AerobreakInfo>(null, null);
					}
				}
			}
			LambertEquations lambertEquations = default(LambertEquations);
			LambertEquations lambertEquations2 = default(LambertEquations);
			double num = proposedArrivalTime.DifferenceInSeconds(launchTime);
			lambertEquations.SolveLambert(num, initialCartesian, aerocaptureCartesian, commonBarycenter.mu, false, true);
			lambertEquations2.SolveLambert(num, initialCartesian, aerocaptureCartesian, commonBarycenter.mu, false, true);
			Vector3d burn = ((lambertEquations.burn0.magnitude + lambertEquations.burn1.magnitude <= lambertEquations2.burn0.magnitude + lambertEquations2.burn0.magnitude) ? lambertEquations : lambertEquations2).burn1;
			double num2 = (aerocaptureBarycenter.meanRadius_m + destinationOrbitalElementsAroundAerocaptureBarycenter.semiMajorAxis_m) / 2.0;
			double num3 = 3.141592653589793 * Mathd.Sqrt(num2 * num2 * num2 / aerocaptureBarycenter.mu);
			TIDateTime tidateTime = new TIDateTime(proposedArrivalTime, num3);
			double num4 = Mathd.ClampRadiansTwoPI(MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(-burn, destinationOrbitalElementsAroundAerocaptureBarycenter, aerocaptureBarycenter, proposedArrivalTime, param.fleet.faction.isActivePlayer) + destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad);
			TorchTransfer torchTransfer = new TorchTransfer();
			bool flag;
			torchTransfer.Solve(launchTime, num, param.fleetAcceleration_mps2, initialCartesian, aerocaptureCartesian, commonBarycenter, param.fleetDeltaV_mps, out flag, true);
			double num5 = Mathd.ClampRadiansTwoPI(MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(-(torchTransfer.decelerationVector_mps2 * torchTransfer.decelDuration_s), destinationOrbitalElementsAroundAerocaptureBarycenter, aerocaptureBarycenter, proposedArrivalTime, param.fleet.faction.isActivePlayer) + destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad);
			double num6 = Mathd.ClampRadiansTwoPI(num4 + 3.141592653589793 - destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad);
			DateTime dateTime = destinationOrbitalElementsAroundAerocaptureBarycenter.NextTimeAtMeanAnomaly(num6, tidateTime.ExportTime(), aerocaptureBarycenter.mass_kg).AddSeconds(-num3);
			double num7 = Mathd.ClampRadiansTwoPI(num5 + 3.141592653589793 - destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad);
			DateTime dateTime2 = destinationOrbitalElementsAroundAerocaptureBarycenter.NextTimeAtMeanAnomaly(num7, proposedArrivalTime.ExportTime(), aerocaptureBarycenter.mass_kg).AddSeconds(-num3);
			OrbitalElementsState orbitalElementsState2 = new OrbitalElementsState
			{
				longAscendingNode_Rad = destinationOrbitalElementsAroundAerocaptureBarycenter.longAscendingNode_Rad,
				inclination_Rad = destinationOrbitalElementsAroundAerocaptureBarycenter.inclination_Rad,
				semiMajorAxis_m = num2,
				eccentricity = 1.0 - aerocaptureBarycenter.meanRadius_m / num2,
				meanAnomalyAtEpoch_Rad = 0.0,
				epoch = dateTime,
				argPeriapsis_Rad = num4
			};
			OrbitalElementsState orbitalElementsState3 = new OrbitalElementsState(orbitalElementsState2);
			orbitalElementsState3.epoch = dateTime2;
			orbitalElementsState3.argPeriapsis_Rad = num5;
			AerobreakInfo aerobreakInfo = new AerobreakInfo
			{
				barycenter = aerocaptureBarycenter,
				hohmannOrbit = orbitalElementsState2,
				aerobreakTime = new TIDateTime(dateTime),
				arrivalTime = new TIDateTime(dateTime, num3),
				microthrustSpiral = null
			};
			AerobreakInfo aerobreakInfo2 = new AerobreakInfo
			{
				barycenter = aerocaptureBarycenter,
				hohmannOrbit = orbitalElementsState3,
				aerobreakTime = new TIDateTime(dateTime2),
				arrivalTime = new TIDateTime(dateTime2, num3),
				microthrustSpiral = null
			};
			double num8 = Mathd.Sqrt(aerocaptureBarycenter.mu * (2.0 / destinationOrbitalElementsAroundAerocaptureBarycenter.semiMajorAxis_m - 1.0 / num2));
			if ((Mathd.Sqrt(aerocaptureBarycenter.mu / destinationOrbitalElementsAroundAerocaptureBarycenter.semiMajorAxis_m) - num8) / (double)param.fleet.cruiseAcceleration_mps2 > num3)
			{
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(param.fleetAcceleration_mps2, aerocaptureBarycenter.mu, aerocaptureBarycenter.sphereOfInfluence_m);
				double num9 = Mathd.Sqrt(aerocaptureBarycenter.mu / orbitalElementsState2.semiMajorAxis_m);
				double num10 = Mathd.Sqrt(aerocaptureBarycenter.mu / destinationOrbitalElementsAroundAerocaptureBarycenter.semiMajorAxis_m);
				double num11 = Mathd.Abs(microthrustSphere.GetDuration_s(num10) - microthrustSphere.GetDuration_s(num9));
				Vector3d normalized = orbitalElementsState2.periapsisVector.normalized;
				Vector3d normalized2 = orbitalElementsState3.periapsisVector.normalized;
				Vector3d normalized3 = destinationOrbitalElementsAroundAerocaptureBarycenter.periapsisVector.normalized;
				Vector3d vector3d = orbitalElementsState2.eccentricity * normalized;
				Vector3d vector3d2 = orbitalElementsState3.eccentricity * normalized2;
				Vector3d vector3d3 = destinationOrbitalElementsAroundAerocaptureBarycenter.eccentricity * normalized3;
				double magnitude = (vector3d - vector3d3).magnitude;
				double magnitude2 = (vector3d2 - vector3d3).magnitude;
				double num12 = num11 * (1.0 + magnitude);
				double num13 = num11 * (1.0 + magnitude2);
				double num14 = param.fleetAcceleration_mps2 * num11 / num12;
				double num15 = param.fleetAcceleration_mps2 * num11 / num13;
				MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(num14, aerocaptureBarycenter.mu, aerocaptureBarycenter.sphereOfInfluence_m);
				MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(num15, aerocaptureBarycenter.mu, aerocaptureBarycenter.sphereOfInfluence_m);
				double num16 = Mathd.Abs(microthrustSphere2.GetAnomalyDelta_Rad(num10) - microthrustSphere2.GetAnomalyDelta_Rad(num9));
				double num17 = Mathd.Abs(microthrustSphere3.GetAnomalyDelta_Rad(num10) - microthrustSphere3.GetAnomalyDelta_Rad(num9));
				double num18 = orbitalElementsState2.longAscendingNode_Rad + orbitalElementsState2.argPeriapsis_Rad - destinationOrbitalElementsAroundAerocaptureBarycenter.longAscendingNode_Rad - destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad + num16;
				double num19 = orbitalElementsState3.longAscendingNode_Rad + orbitalElementsState3.argPeriapsis_Rad - destinationOrbitalElementsAroundAerocaptureBarycenter.longAscendingNode_Rad - destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad + num17;
				DateTime dateTime3 = destinationOrbitalElementsAroundAerocaptureBarycenter.NextTimeAtMeanAnomaly(num18, tidateTime.ExportTime().AddSeconds(num12), aerocaptureBarycenter.mass_kg).AddSeconds(-num12);
				orbitalElementsState2.epoch = dateTime3;
				DateTime dateTime4 = destinationOrbitalElementsAroundAerocaptureBarycenter.NextTimeAtMeanAnomaly(num19, proposedArrivalTime.ExportTime().AddSeconds(num13), aerocaptureBarycenter.mass_kg).AddSeconds(-num13);
				orbitalElementsState3.epoch = dateTime4;
				aerobreakInfo.microthrustSpiral = new MicrothrustTransferSegmentLERP
				{
					startTime = new TIDateTime(dateTime3),
					endTime = new TIDateTime(dateTime3, num12),
					barycenter = aerocaptureBarycenter,
					trueFleetAcceleration_mps2 = param.fleetAcceleration_mps2,
					effectiveFleetAcceleration_mps2 = num14,
					start = new MicrothrustTransferLERPvalues(num2, orbitalElementsState2.meanAnomalyAtEpoch_Rad, orbitalElementsState2.eccentricity, orbitalElementsState2.longAscendingNode_Rad, orbitalElementsState2.inclination_Rad, orbitalElementsState2.argPeriapsis_Rad, 0.0, 0.0, 0.0),
					end = new MicrothrustTransferLERPvalues(destinationOrbitalElementsAroundAerocaptureBarycenter.semiMajorAxis_m, num18, destinationOrbitalElementsAroundAerocaptureBarycenter.eccentricity, destinationOrbitalElementsAroundAerocaptureBarycenter.longAscendingNode_Rad, destinationOrbitalElementsAroundAerocaptureBarycenter.inclination_Rad, destinationOrbitalElementsAroundAerocaptureBarycenter.argPeriapsis_Rad, 0.0, 0.0, 0.0)
				};
				aerobreakInfo2.aerobreakTime = new TIDateTime(dateTime4);
				aerobreakInfo2.microthrustSpiral = aerobreakInfo.microthrustSpiral.Copy();
				aerobreakInfo2.microthrustSpiral.startTime = new TIDateTime(dateTime4);
				aerobreakInfo2.microthrustSpiral.endTime = new TIDateTime(dateTime4, num12);
				aerobreakInfo2.microthrustSpiral.effectiveFleetAcceleration_mps2 = num15;
				aerobreakInfo2.microthrustSpiral.start.argPeriapsis_Rad = orbitalElementsState3.argPeriapsis_Rad;
				aerobreakInfo2.microthrustSpiral.end.meanAnomaly_Rad = num19;
			}
			return new ValueTuple<AerobreakInfo, AerobreakInfo>(aerobreakInfo, aerobreakInfo2);
		}

		// Token: 0x06003DE4 RID: 15844 RVA: 0x0018A1B0 File Offset: 0x001883B0
		private static bool isAerobreakingBetterThanHohmann(double initialRadius_m, double finalRadius_m, double aerobreakRadius_m)
		{
			if (initialRadius_m <= finalRadius_m)
			{
				return false;
			}
			double num = initialRadius_m / finalRadius_m;
			double num2 = aerobreakRadius_m / finalRadius_m;
			double num3 = Mathd.Sqrt(1.0 / num);
			double num4 = num3 - Mathd.Sqrt(2.0 / num - 2.0 / (num + 1.0));
			double num5 = Mathd.Sqrt(2.0 - 2.0 / (num + 1.0)) - 1.0;
			double num6 = num3 - Mathd.Sqrt(2.0 / num - 2.0 / (num + num2));
			double num7 = 1.0 - Mathd.Sqrt(2.0 - 2.0 / (1.0 + num2));
			double num8 = num4 + num5;
			return num6 + num7 < num8;
		}

		// Token: 0x06003DE5 RID: 15845 RVA: 0x0018A290 File Offset: 0x00188490
		private static TIDateTime BestLaunchTimeFromActiveTrajectory(Trajectory trajectory, TIDateTime earliestTime, TIDateTime latestTime)
		{
			TIDateTime tidateTime = (trajectory.destroyOnArrival ? TIDateTime.Min(latestTime, trajectory.arrivalTime) : latestTime);
			TIDateTime tidateTime2 = new TIDateTime(earliestTime, tidateTime.DifferenceInSeconds(earliestTime) / 2.0);
			List<TIDateTime> list = MasterTransferPlanner.FindFutureApsidesTimes(trajectory, earliestTime, latestTime);
			if (list.Count == 0)
			{
				return tidateTime2;
			}
			return TIDateTime.Max(TIDateTime.Min(list[0], tidateTime2), TITimeState.Now());
		}

		// Token: 0x06003DE6 RID: 15846 RVA: 0x0018A2F7 File Offset: 0x001884F7
		private static List<TIDateTime> LaunchTimesToTestOnActiveTrajectory(Trajectory trajectory, TIDateTime now, TIDateTime arrivalTime)
		{
			List<TIDateTime> list = MasterTransferPlanner.FindFutureApsidesTimes(trajectory, now, arrivalTime);
			list.Add(now);
			return list;
		}

		// Token: 0x06003DE7 RID: 15847 RVA: 0x0018A308 File Offset: 0x00188508
		private static double LagrangeOnlyMaxDuration_s(double accleration_mps2, double distance_m, double DV_mps)
		{
			double num = 10.0 * distance_m / DV_mps + 2.0 * accleration_mps2 / (DV_mps * 10.0);
			double num2 = (-accleration_mps2 + Mathd.Sqrt(accleration_mps2 * (accleration_mps2 + 792.0 * distance_m))) / 396.0;
			double num3 = (distance_m - num2) / num2 + num2 / accleration_mps2 * 2.0;
			return Mathd.Max(num, num3);
		}

		// Token: 0x06003DE8 RID: 15848 RVA: 0x0018A378 File Offset: 0x00188578
		[return: TupleElementNames(new string[] { "barycenter", "radius_m" })]
		private static ValueTuple<TINaturalSpaceObjectState, double> GetBarycenterAndRadiusOfFleetAtArrival(TISpaceFleetState fleet)
		{
			if (!fleet.transferAssigned)
			{
				return new ValueTuple<TINaturalSpaceObjectState, double>(fleet.barycenter, fleet.semiMajorAxis_m);
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			double distFromBarycenterAtTime_m = fleet.trajectory.getDistFromBarycenterAtTime_m(new TIDateTime(fleet.trajectory.arrivalTime, 1.0), out tinaturalSpaceObjectState);
			return new ValueTuple<TINaturalSpaceObjectState, double>(tinaturalSpaceObjectState, distFromBarycenterAtTime_m);
		}

		// Token: 0x06003DE9 RID: 15849 RVA: 0x0018A3D0 File Offset: 0x001885D0
		private static double GetCommonSemiMajorAxis_m([TupleElementNames(new string[] { "barycenter", "radius_m" })] ValueTuple<TINaturalSpaceObjectState, double> target, TINaturalSpaceObjectState commonBarycenter)
		{
			if (target.Item1 == commonBarycenter)
			{
				return target.Item2;
			}
			if (target.Item1.barycenter == commonBarycenter)
			{
				return target.Item1.semiMajorAxis_m;
			}
			return target.Item1.barycenter.semiMajorAxis_m;
		}

		// Token: 0x06003DEA RID: 15850 RVA: 0x0018A424 File Offset: 0x00188624
		private static double GetRadiusFromGameState(TISpaceGameState state)
		{
			if (state.isOrbitState)
			{
				return state.ref_orbit.semiMajorAxis_m;
			}
			if (state.isHabState)
			{
				TIHabState ref_hab = state.ref_hab;
				if (ref_hab.IsStation)
				{
					return ref_hab.ref_orbit.semiMajorAxis_m;
				}
			}
			if (state.isSpaceObjectState)
			{
				return state.ref_spaceObject.semiMajorAxis_m;
			}
			Debug.LogError("MasterTransferPlanner:GetRadiusFromGameState(): state has no radius: " + state.ToString());
			return 0.0;
		}

		// Token: 0x06003DEB RID: 15851 RVA: 0x0018A49C File Offset: 0x0018869C
		private static TIDateTime GetBestHohmannArrivalTime(OrbitalElementsState startLocalOrbit, OrbitalElementsState endLocalOrbit, TINaturalSpaceObjectState startBarycenter, TINaturalSpaceObjectState endBarycenter, TIDateTime now, double fleetAcceleration_mps2, TIFactionState faction, out MasterTransferPlanner.HohmannTiming laterHohmannTransfers)
		{
			if (startLocalOrbit.semiMajorAxis_m <= 0.0)
			{
				Log.Error("Hyperbolic initial orbit when calculating best Hohmann timing.", Array.Empty<object>());
			}
			if (endLocalOrbit.semiMajorAxis_m <= 0.0)
			{
				Log.Error("Hyperbolic final orbit when calculating best Hohmann timing.", Array.Empty<object>());
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState = startBarycenter.FindCommonBarycenter(endBarycenter);
			double num = 0.0;
			if (startBarycenter != tinaturalSpaceObjectState)
			{
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, startBarycenter.mu, startBarycenter.sphereOfInfluence_m);
				if (microthrustSphere.Radius_m > startLocalOrbit.semiMajorAxis_m)
				{
					double num2 = Mathd.Sqrt(startBarycenter.mu / startLocalOrbit.semiMajorAxis_m);
					num += microthrustSphere.GetDuration_s(num2);
				}
				if (startBarycenter.barycenter != tinaturalSpaceObjectState)
				{
					MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, startBarycenter.barycenter.mu, startBarycenter.barycenter.sphereOfInfluence_m);
					if (microthrustSphere2.Radius_m > startBarycenter.semiMajorAxis_m)
					{
						double num3 = Mathd.Sqrt(startBarycenter.barycenter.mu / startBarycenter.semiMajorAxis_m);
						num += microthrustSphere2.GetDuration_s(num3);
					}
				}
			}
			double num4 = 0.0;
			if (endBarycenter != tinaturalSpaceObjectState)
			{
				MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(fleetAcceleration_mps2, endBarycenter.mu, endBarycenter.sphereOfInfluence_m);
				if (microthrustSphere3.Radius_m > endLocalOrbit.semiMajorAxis_m)
				{
					double num5 = Mathd.Sqrt(endBarycenter.mu / endLocalOrbit.semiMajorAxis_m);
					num4 += microthrustSphere3.GetDuration_s(num5);
				}
				if (endBarycenter.barycenter != tinaturalSpaceObjectState)
				{
					MicrothrustSphere microthrustSphere4 = new MicrothrustSphere(fleetAcceleration_mps2, endBarycenter.barycenter.mu, endBarycenter.barycenter.sphereOfInfluence_m);
					if (microthrustSphere4.Radius_m > endBarycenter.semiMajorAxis_m)
					{
						double num6 = Mathd.Sqrt(endBarycenter.barycenter.mu / endBarycenter.semiMajorAxis_m);
						num4 += microthrustSphere4.GetDuration_s(num6);
					}
				}
			}
			OrbitalElementsState orbitalElementsState = ((startBarycenter == tinaturalSpaceObjectState) ? startLocalOrbit : ((startBarycenter.barycenter == tinaturalSpaceObjectState) ? new OrbitalElementsState(startBarycenter) : new OrbitalElementsState(startBarycenter.barycenter)));
			OrbitalElementsState orbitalElementsState2 = ((endBarycenter == tinaturalSpaceObjectState) ? endLocalOrbit : ((endBarycenter.barycenter == tinaturalSpaceObjectState) ? new OrbitalElementsState(endBarycenter) : new OrbitalElementsState(endBarycenter.barycenter)));
			bool flag = orbitalElementsState2.semiMajorAxis_m > orbitalElementsState.semiMajorAxis_m;
			MicrothrustSphere microthrustSphere5 = new MicrothrustSphere(fleetAcceleration_mps2, tinaturalSpaceObjectState.mu, tinaturalSpaceObjectState.sphereOfInfluence_m);
			if (microthrustSphere5.Radius_m > Mathd.Max(orbitalElementsState.semiMajorAxis_m, orbitalElementsState2.semiMajorAxis_m))
			{
				Log.Error("Microthrust-only transfer; there is no possible Hohmann timing.", Array.Empty<object>());
				double num7 = microthrustSphere5.GetDuration_s(orbitalElementsState.semiMajorAxis_m) - microthrustSphere5.GetDuration_s(orbitalElementsState2.semiMajorAxis_m);
				if (num7 < 0.0)
				{
					num7 = -num7;
				}
				TIDateTime tidateTime = new TIDateTime(now, num + num4 + num7);
				laterHohmannTransfers = new MasterTransferPlanner.HohmannTiming
				{
					initialHohmannArrivalTime = tidateTime
				};
				return tidateTime;
			}
			DateTime dateTime = now.ExportTime();
			DateTime dateTime2 = dateTime + new TimeSpan(0, 0, 1);
			double num8 = 0.0;
			double num9 = orbitalElementsState.semiMajorAxis_m;
			double num10 = orbitalElementsState.MeanLongitudeAtTime_Rad(dateTime, tinaturalSpaceObjectState.mass_kg);
			double num11 = Mathd.ClampRadiansPI(orbitalElementsState.MeanLongitudeAtTime_Rad(dateTime2, tinaturalSpaceObjectState.mass_kg) - num10);
			double num12 = 0.0;
			double num13 = orbitalElementsState2.semiMajorAxis_m;
			double num14 = orbitalElementsState2.MeanLongitudeAtTime_Rad(dateTime, tinaturalSpaceObjectState.mass_kg);
			double num15 = Mathd.ClampRadiansPI(orbitalElementsState2.MeanLongitudeAtTime_Rad(dateTime2, tinaturalSpaceObjectState.mass_kg) - num14);
			double num16 = num15 - num11;
			if (flag && microthrustSphere5.Radius_m > orbitalElementsState.semiMajorAxis_m)
			{
				double num17 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / orbitalElementsState.semiMajorAxis_m);
				num8 = microthrustSphere5.GetDuration_s(num17);
				num9 = microthrustSphere5.Radius_m;
				num10 += microthrustSphere5.GetAnomalyDelta_Rad(num17);
			}
			if (!flag && microthrustSphere5.Radius_m > orbitalElementsState2.semiMajorAxis_m)
			{
				double num18 = Mathd.Sqrt(tinaturalSpaceObjectState.mu / orbitalElementsState2.semiMajorAxis_m);
				num12 = microthrustSphere5.GetDuration_s(num18);
				num13 = microthrustSphere5.Radius_m;
				num14 -= microthrustSphere5.GetAnomalyDelta_Rad(num18);
			}
			double num19 = (num9 + num13) / 2.0;
			double num20 = 3.141592653589793 * Mathd.Sqrt(num19 * num19 * num19 / tinaturalSpaceObjectState.mu);
			double num21 = num14 + num15 * num20;
			double num22 = Mathd.ClampRadiansTwoPI(num10 + 3.141592653589793 - num21);
			if (num16 < 0.0)
			{
				num22 -= 6.283185307179586;
			}
			double num23 = num22 / num16;
			double num24 = num20 + num8 + num12 + num + num4;
			if (num24 + num23 > MasterTransferPlanner.TransferDurationHardCap(faction))
			{
				TIDateTime tidateTime2 = new TIDateTime(now, MasterTransferPlanner.TransferDurationHardCap(faction) * 2.0);
				laterHohmannTransfers = new MasterTransferPlanner.HohmannTiming
				{
					initialHohmannArrivalTime = tidateTime2
				};
				return tidateTime2;
			}
			TIDateTime tidateTime3 = new TIDateTime(now, num24 + num23);
			Vector3d normalVector = orbitalElementsState.normalVector;
			Vector3d normalVector2 = orbitalElementsState2.normalVector;
			if ((in normalVector) == (in normalVector2))
			{
				laterHohmannTransfers = new MasterTransferPlanner.HohmannTiming
				{
					initialHohmannArrivalTime = tidateTime3
				};
			}
			else
			{
				TIDateTime tidateTime4 = new TIDateTime(now, MasterTransferPlanner.TransferDurationHardCap(faction));
				OrbitalElementsState orbitalElementsState3;
				TIDateTime tidateTime5;
				Vector3d vector3d;
				if (orbitalElementsState.semiMajorAxis_m < orbitalElementsState2.semiMajorAxis_m)
				{
					orbitalElementsState3 = orbitalElementsState2;
					tidateTime5 = tidateTime3;
					vector3d = orbitalElementsState.normalVector;
				}
				else
				{
					orbitalElementsState3 = orbitalElementsState;
					tidateTime5 = new TIDateTime(tidateTime3, -num24);
					vector3d = orbitalElementsState2.normalVector;
				}
				double num25 = Mathd.Abs(Vector3d.Dot(in orbitalElementsState3.ToCartesianStateAtTime(tidateTime5.ExportTime(), tinaturalSpaceObjectState.mass_kg).position, in vector3d));
				double num26 = num25;
				bool flag2 = false;
				double num27 = 1.0 / Mathd.Abs(1.0 / orbitalElementsState.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg) - 1.0 / orbitalElementsState2.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg));
				int num28 = -1;
				int num29 = -1;
				int num30 = 1;
				while (num30 <= 1000 && num27 <= 31556924.0)
				{
					TIDateTime tidateTime6 = new TIDateTime(tidateTime5, (double)num30 * num27);
					if (tidateTime6 > tidateTime4)
					{
						break;
					}
					double num31 = Mathd.Abs(Vector3d.Dot(in orbitalElementsState3.ToCartesianStateAtTime(tidateTime6.ExportTime(), tinaturalSpaceObjectState.mass_kg).position, in vector3d));
					if (num31 < num26)
					{
						flag2 = true;
						if (num31 < num25)
						{
							if (num28 < 1)
							{
								num28 = num30;
							}
							num29 = num30;
						}
					}
					else if (flag2)
					{
						break;
					}
					num26 = num31;
					num30++;
				}
				laterHohmannTransfers = new MasterTransferPlanner.HohmannTiming
				{
					initialHohmannArrivalTime = tidateTime3,
					transferDuration_s = num24,
					synodicPeriod_s = num27,
					firstHohmannAfterInitial = num28,
					lastHohmannAfterInitial = num29
				};
			}
			return tidateTime3;
		}

		// Token: 0x06003DEC RID: 15852 RVA: 0x0018AB18 File Offset: 0x00188D18
		private static double GetMaxDurationOfTransfer(ITransferTarget start, ITransferTarget end, double fleetAcceleration_mps2, TIDateTime now)
		{
			IMobileAsset mobileAsset = start as IMobileAsset;
			if (mobileAsset != null)
			{
				bool isAlienFaction = mobileAsset.faction.IsAlienFaction;
			}
			bool flag = !(end is TIOrbitState) || end.barycenter() != start.barycenter();
			TISpaceFleetState tispaceFleetState = end as TISpaceFleetState;
			if (tispaceFleetState != null)
			{
				TISpaceFleetState tispaceFleetState2 = tispaceFleetState;
				IMobileAsset mobileAsset2 = start as IMobileAsset;
				if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState2, (mobileAsset2 != null) ? mobileAsset2.faction : null))
				{
					CartesianState cartesianState;
					TINaturalSpaceObjectState tinaturalSpaceObjectState;
					bool flag2 = start.tryToGetLocalCartesianState(TITimeState.Now(), out cartesianState, out tinaturalSpaceObjectState);
					double num = cartesianState.position.magnitude;
					CartesianState cartesianState2;
					TINaturalSpaceObjectState tinaturalSpaceObjectState2;
					bool flag3 = end.tryToGetLocalCartesianState(TITimeState.Now(), out cartesianState2, out tinaturalSpaceObjectState2);
					double num2 = cartesianState2.position.magnitude;
					TIDateTime arrivalTime = tispaceFleetState.trajectory.arrivalTime;
					CartesianState cartesianState3;
					TINaturalSpaceObjectState tinaturalSpaceObjectState3;
					bool flag4 = end.tryToGetLocalCartesianState(arrivalTime, out cartesianState3, out tinaturalSpaceObjectState3);
					double num3 = cartesianState3.position.magnitude;
					if (!flag2)
					{
						OrbitalElementsState orbitalElementsState;
						bool flag5;
						start.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState, out tinaturalSpaceObjectState, out flag5);
						num = orbitalElementsState.semiMajorAxis_m;
					}
					if (!flag3)
					{
						bool flag5;
						OrbitalElementsState orbitalElementsState2;
						end.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag5);
						num2 = orbitalElementsState2.semiMajorAxis_m;
					}
					if (!flag4)
					{
						bool flag5;
						OrbitalElementsState orbitalElementsState3;
						end.getOrbitalElementsState(arrivalTime, out orbitalElementsState3, out tinaturalSpaceObjectState3, out flag5);
						num3 = orbitalElementsState3.semiMajorAxis_m;
					}
					double maxDurationOfTransfer = MasterTransferPlanner.GetMaxDurationOfTransfer(tinaturalSpaceObjectState, num, tinaturalSpaceObjectState2, num2, fleetAcceleration_mps2, flag);
					double maxDurationOfTransfer2 = MasterTransferPlanner.GetMaxDurationOfTransfer(tinaturalSpaceObjectState, num, tinaturalSpaceObjectState3, num3, fleetAcceleration_mps2, flag);
					double num4 = Mathd.Max(maxDurationOfTransfer, maxDurationOfTransfer2);
					IMobileAsset mobileAsset3 = start as IMobileAsset;
					return Mathd.Min(num4, MasterTransferPlanner.TransferDurationHardCap((mobileAsset3 != null) ? mobileAsset3.faction : null));
				}
			}
			double maxDurationOfTransfer3 = MasterTransferPlanner.GetMaxDurationOfTransfer(start.barycenter(), start.a_m(), end.barycenter(), end.a_m(), fleetAcceleration_mps2, flag);
			IMobileAsset mobileAsset4 = start as IMobileAsset;
			return Mathd.Min(maxDurationOfTransfer3, MasterTransferPlanner.TransferDurationHardCap((mobileAsset4 != null) ? mobileAsset4.faction : null));
		}

		// Token: 0x06003DED RID: 15853 RVA: 0x0018ACBC File Offset: 0x00188EBC
		private static double GetMaxDurationOfTransfer(TINaturalSpaceObjectState startBarycenter, double startRadius_m, TINaturalSpaceObjectState endBarycenter, double endRadius_m, double fleetAcceleration_mps2, bool includeSynodicPeriod)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = startBarycenter.FindCommonBarycenter(endBarycenter);
			MasterTransferPlanner.IdentifyHybridTransferType_Result identifyHybridTransferType_Result = MasterTransferPlanner.IdentifyHybridTransferType(startRadius_m, startBarycenter, endRadius_m, endBarycenter, tinaturalSpaceObjectState, fleetAcceleration_mps2);
			double num = ((startBarycenter == tinaturalSpaceObjectState) ? startRadius_m : ((startBarycenter.barycenter == tinaturalSpaceObjectState) ? startBarycenter.semiMajorAxis_m : startBarycenter.barycenter.semiMajorAxis_m));
			double num2 = ((endBarycenter == tinaturalSpaceObjectState) ? endRadius_m : ((endBarycenter.barycenter == tinaturalSpaceObjectState) ? endBarycenter.semiMajorAxis_m : endBarycenter.barycenter.semiMajorAxis_m));
			if (num < identifyHybridTransferType_Result.commonMicrothrustRadius_m && num2 < identifyHybridTransferType_Result.commonMicrothrustRadius_m)
			{
				return identifyHybridTransferType_Result.totalMicrothrustDuration_s;
			}
			num = Mathd.Max(num, identifyHybridTransferType_Result.commonMicrothrustRadius_m);
			num2 = Mathd.Max(num2, identifyHybridTransferType_Result.commonMicrothrustRadius_m);
			double num3 = MasterTransferPlanner.HohmannDuration_s(num, num2, tinaturalSpaceObjectState.mu);
			double num4 = (includeSynodicPeriod ? MasterTransferPlanner.SynodicPeriod_s(num, num2, tinaturalSpaceObjectState.mu) : 0.0);
			if (double.IsInfinity(num4))
			{
				num4 = 0.0;
			}
			double totalMicrothrustDuration_s = identifyHybridTransferType_Result.totalMicrothrustDuration_s;
			return num3 + num4 + totalMicrothrustDuration_s;
		}

		// Token: 0x06003DEE RID: 15854 RVA: 0x0018ADBC File Offset: 0x00188FBC
		private static double GetMeanAnomalyAtTime(ITransferTarget targetValue, TIDateTime time)
		{
			TIDateTime tidateTime = new TIDateTime();
			tidateTime.SetTime(targetValue.t0_jy());
			double num = time.DifferenceInDays(tidateTime) / targetValue.period_days();
			return targetValue.M0_rad() + 6.283185307179586 * (num % 1.0);
		}

		// Token: 0x06003DEF RID: 15855 RVA: 0x0018AE08 File Offset: 0x00189008
		private static TIDateTime TimeWhenAtMeanAnomaly(TIDateTime approximateTime, double targetAnomaly_Rad, ITransferTarget targetValue)
		{
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			targetValue.getOrbitalElementsState(approximateTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			return new TIDateTime(orbitalElementsState.NextTimeAtMeanAnomaly(targetAnomaly_Rad, approximateTime.ExportTime(), tinaturalSpaceObjectState.mass_kg));
		}

		// Token: 0x06003DF0 RID: 15856 RVA: 0x0018AE3C File Offset: 0x0018903C
		private static TIDateTime TimeWhenAtMeanAnomaly(TIDateTime approximateTime, double targetAnomaly_Rad, double anomalyAtApproximateTime_Rad, double orbitPeriod_d)
		{
			double num = MasterTransferPlanner.NormalizeAngleNearZero_Rad((targetAnomaly_Rad - anomalyAtApproximateTime_Rad) % 6.283185307179586) * orbitPeriod_d / 6.283185307179586 * 86400.0;
			return new TIDateTime(approximateTime, num);
		}

		// Token: 0x06003DF1 RID: 15857 RVA: 0x0018AE7C File Offset: 0x0018907C
		private static TIDateTime AdvanceTimePastDeadlineInIncrements(TIDateTime time, TIDateTime deadline, double increment_s)
		{
			if (time > deadline)
			{
				return time;
			}
			int num = Mathd.CeilToInt(deadline.DifferenceInSeconds(time) / increment_s);
			return new TIDateTime(time, (double)num * increment_s);
		}

		// Token: 0x06003DF2 RID: 15858 RVA: 0x0018AEB0 File Offset: 0x001890B0
		[return: TupleElementNames(new string[] { "earliestLaunchTime", "expectedBestLaunchTime", "latestLaunchTime" })]
		private static ValueTuple<TIDateTime, TIDateTime, TIDateTime> CalculateLaunchTiming(MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, TIDateTime arrivalTime, TINaturalSpaceObjectState commonBarycenter, MasterTransferPlanner.IdentifyHybridTransferType_Result hybridTransferType)
		{
			MasterTransferPlanner.SimplifiedPositions simplifiedPositions = MasterTransferPlanner.GetSimplifiedPositions(param.originValue, param.destinationValue, param.now, arrivalTime);
			double originDistToCommonBarycenter_m = simplifiedPositions.originDistToCommonBarycenter_m;
			double destinationDistToCommonBarycenter_m = simplifiedPositions.destinationDistToCommonBarycenter_m;
			double num = MasterTransferPlanner.SynodicPeriod_s(originDistToCommonBarycenter_m, destinationDistToCommonBarycenter_m, simplifiedPositions.commonBarycenter.mu);
			double totalMicrothrustDuration_s = hybridTransferType.totalMicrothrustDuration_s;
			double num2 = MasterTransferPlanner.HohmannDuration_s(originDistToCommonBarycenter_m, destinationDistToCommonBarycenter_m, simplifiedPositions.commonBarycenter.mu);
			return MasterTransferPlanner.CalculateLaunchTiming(param.now, arrivalTime, num2, totalMicrothrustDuration_s, num);
		}

		// Token: 0x06003DF3 RID: 15859 RVA: 0x0018AF20 File Offset: 0x00189120
		[return: TupleElementNames(new string[] { "earliestLaunchTime", "expectedBestLaunchTime", "latestLaunchTime" })]
		private static ValueTuple<TIDateTime, TIDateTime, TIDateTime> CalculateLaunchTimingWhenDestinationIsTransferingFleet(MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, TIDateTime arrivalTime, TINaturalSpaceObjectState commonBarycenter, [TupleElementNames(new string[] { "barycenter", "radius_m" })] ValueTuple<TINaturalSpaceObjectState, double> eventualDestination)
		{
			Trajectory trajectory = (param.destinationValue as TISpaceFleetState).trajectory;
			if (trajectory.endsInCrash || trajectory.exitsSolarSystem)
			{
				return new ValueTuple<TIDateTime, TIDateTime, TIDateTime>(param.now, param.now, new TIDateTime(param.now, arrivalTime.DifferenceInSeconds(param.now) * 0.9));
			}
			MasterTransferPlanner.SimplifiedPositions simplifiedPositions = MasterTransferPlanner.GetSimplifiedPositions(param.originValue, param.destinationValue, param.now, arrivalTime);
			double originDistToCommonBarycenter_m = simplifiedPositions.originDistToCommonBarycenter_m;
			double destinationDistToCommonBarycenter_m = simplifiedPositions.destinationDistToCommonBarycenter_m;
			double num = 0.0;
			double totalMicrothrustDuration_s = MasterTransferPlanner.IdentifyHybridTransferType(simplifiedPositions, param.fleetAcceleration_mps2).totalMicrothrustDuration_s;
			double num2 = MasterTransferPlanner.HohmannDuration_s(originDistToCommonBarycenter_m, destinationDistToCommonBarycenter_m, simplifiedPositions.commonBarycenter.mu);
			return MasterTransferPlanner.CalculateLaunchTiming(param.now, arrivalTime, num2, totalMicrothrustDuration_s, num);
		}

		// Token: 0x06003DF4 RID: 15860 RVA: 0x0018AFE8 File Offset: 0x001891E8
		[return: TupleElementNames(new string[] { "earliestLaunchTime", "expectedBestLaunchTime", "latestLaunchTime" })]
		private static ValueTuple<TIDateTime, TIDateTime, TIDateTime> CalculateLaunchTiming(TIDateTime now, TIDateTime arrivalTime, double hohmannDuration_s, double expectedMicrothrustDelays_s, double synodicPeriod_s)
		{
			TIDateTime tidateTime;
			if (arrivalTime.DifferenceInSeconds(now) < hohmannDuration_s)
			{
				tidateTime = new TIDateTime(now);
			}
			else
			{
				tidateTime = new TIDateTime(arrivalTime, -hohmannDuration_s - expectedMicrothrustDelays_s);
			}
			TIDateTime tidateTime2 = new TIDateTime(now);
			if (tidateTime < tidateTime2)
			{
				tidateTime = new TIDateTime(tidateTime2);
			}
			double num = (arrivalTime.DifferenceInSeconds(now) - expectedMicrothrustDelays_s) * 0.9;
			TIDateTime tidateTime3 = new TIDateTime(now, (arrivalTime.DifferenceInSeconds(now) - Mathd.Min(synodicPeriod_s, num) - expectedMicrothrustDelays_s) * 0.9);
			return new ValueTuple<TIDateTime, TIDateTime, TIDateTime>(tidateTime2, tidateTime, tidateTime3);
		}

		// Token: 0x06003DF5 RID: 15861 RVA: 0x0018B06C File Offset: 0x0018926C
		[return: TupleElementNames(new string[] { "startOrbitalVelocity", "endOrbitalVelocity" })]
		private static ValueTuple<Vector3d, Vector3d> CalculateIdealOrbitalVelocitiesForLambert(MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, TIDateTime launchTime, TIDateTime arrivalTime)
		{
			ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState> relevantBarycentersAtTime = MasterTransferPlanner.GetRelevantBarycentersAtTime(param.originValue, param.destinationValue, launchTime, arrivalTime);
			TINaturalSpaceObjectState item = relevantBarycentersAtTime.Item1;
			TINaturalSpaceObjectState item2 = relevantBarycentersAtTime.Item3;
			CartesianState cartesianState = param.originValue.relevantGlobalCartesianState(item2, launchTime).ToLocal(item2, launchTime);
			CartesianState destinationCartesianAroundCommonBarycenterAtTime = Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(param.destinationValue, arrivalTime, item2, param.fleet.faction, null, 0.0);
			LambertEquations lambertEquations = default(LambertEquations);
			double num = lambertEquations.SolveLambert(arrivalTime.DifferenceInSeconds(launchTime), cartesianState, destinationCartesianAroundCommonBarycenterAtTime, item2.mu, false, true);
			LambertEquations lambertEquations2 = default(LambertEquations);
			double num2 = lambertEquations2.SolveLambert(arrivalTime.DifferenceInSeconds(launchTime), cartesianState, destinationCartesianAroundCommonBarycenterAtTime, item2.mu, true, true);
			LambertEquations lambertEquations3 = ((num < num2) ? lambertEquations : lambertEquations2);
			CartesianState cartesianState2 = cartesianState + new CartesianState(Vector3d.zero, lambertEquations3.burn0);
			CartesianState cartesianState3 = destinationCartesianAroundCommonBarycenterAtTime + new CartesianState(Vector3d.zero, lambertEquations3.burn1);
			ref CartesianState ptr = cartesianState2.ChangeReferenceFrame(item2, item, launchTime);
			CartesianState cartesianState4 = cartesianState3.ChangeReferenceFrame(item2, item, arrivalTime);
			return new ValueTuple<Vector3d, Vector3d>(ptr.velocity, cartesianState4.velocity);
		}

		// Token: 0x06003DF6 RID: 15862 RVA: 0x0018B17C File Offset: 0x0018937C
		[return: TupleElementNames(new string[] { "anomaly_Rad", "radius_m", "isMultiSpiral" })]
		private static ValueTuple<double, double, bool> TerminalSpiralAnomaly_Rad(ITransferTarget transferTarget, double fleetAcceleration_mps2)
		{
			return MasterTransferPlanner.TerminalSpiralAnomaly_Rad(transferTarget.barycenter(), transferTarget.a_m(), fleetAcceleration_mps2);
		}

		// Token: 0x06003DF7 RID: 15863 RVA: 0x0018B190 File Offset: 0x00189390
		[return: TupleElementNames(new string[] { "anomaly_Rad", "radius_m", "isMultiSpiral" })]
		private static ValueTuple<double, double, bool> TerminalSpiralAnomaly_Rad(TINaturalSpaceObjectState targetBarycenter, double targetSemiMajorAxis_m, double fleetAcceleration_mps2)
		{
			if (targetBarycenter == null)
			{
				Log.Error("TerminalSpiralAnomaly_Rad: target barycenter was null", Array.Empty<object>());
				return new ValueTuple<double, double, bool>(0.0, 149598023000.0, false);
			}
			bool flag;
			if (targetBarycenter.isSun)
			{
				Debug.LogError("TerminalSpiralAnomaly_Rad() shouldn't be called when the transfer target is directly orbiting the Sun.");
				flag = false;
			}
			else
			{
				TINaturalSpaceObjectState barycenter = targetBarycenter.barycenter;
				MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, barycenter.mu, barycenter.sphereOfInfluence_m);
				flag = targetBarycenter.semiMajorAxis_m < microthrustSphere.Radius_m;
			}
			if (fleetAcceleration_mps2 == 0.0)
			{
				Log.Error("TerminalSpiralAnomaly_Rad() called with 0 acceleration fleet.", Array.Empty<object>());
				fleetAcceleration_mps2 = 0.1;
			}
			MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, targetBarycenter.mu, targetBarycenter.sphereOfInfluence_m);
			double num = Mathd.Sqrt(targetBarycenter.mu / targetSemiMajorAxis_m);
			double num2 = microthrustSphere2.GetAnomalyDelta_Rad(num);
			double num3 = microthrustSphere2.Radius_m;
			if (double.IsNaN(num2) || double.IsInfinity(num2))
			{
				Log.Error("TerminalSpiralAnomaly_Rad: calculated anomaly was " + num2.ToString() + " radians\nbarycenter was " + targetBarycenter.displayName, Array.Empty<object>());
				num2 = 0.0;
			}
			if (double.IsNaN(num3) || double.IsInfinity(num3) || num3 == 0.0)
			{
				Log.Error("TerminalSpiralAnomaly_Rad: calculated terminal inspiral radius was " + num3.ToString() + " m\nbarycenter was " + targetBarycenter.displayName, Array.Empty<object>());
				TIOrbitState tiorbitState = targetBarycenter.orbits.FirstOrDefault<TIOrbitState>();
				num3 = ((tiorbitState != null) ? tiorbitState.semiMajorAxis_m : 149598023000.0);
			}
			return new ValueTuple<double, double, bool>(num2, num3, flag);
		}

		// Token: 0x06003DF8 RID: 15864 RVA: 0x0018B310 File Offset: 0x00189510
		private static double GetMeanAnomalyWhenFurthestFromOrClosestToParentBarycenter(ITransferTarget transferTarget, TIDateTime time, bool furthest, bool isPlayer)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = transferTarget.barycenter();
			TISpaceFleetState tispaceFleetState = transferTarget as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime < time)
			{
				tinaturalSpaceObjectState = tispaceFleetState.trajectory.GetBarycenterAtTime(time);
			}
			if (tinaturalSpaceObjectState.isSun)
			{
				Debug.LogError("MasterTransferPlanner.GetMeanAnomalyWhenFurthestFromParentBarycenter(): transferTarget's barycenter is the Sun, and thus has no parent barycenter.");
				return 0.0;
			}
			Vector3d vector3d = (tinaturalSpaceObjectState.GetGlobalPositionAtTime(time) - tinaturalSpaceObjectState.barycenter.GetGlobalPositionAtTime(time)).normalized * tinaturalSpaceObjectState.hillRadius_m;
			if (!furthest)
			{
				vector3d = -vector3d;
			}
			double num = TISpaceAssetState.CalculateMeanAnomalyFromPosition(transferTarget, vector3d, time, isPlayer);
			if (double.IsNaN(num) || double.IsInfinity(num))
			{
				Log.Error(string.Concat(new string[]
				{
					"GetMeanAnomalyWhenFurthestFromOrClosestToParentBarycenter: calculated mean anomaly was ",
					num.ToString(),
					" radians\ntargetPos = ",
					vector3d.ToString(),
					"\ntime = ",
					(time != null) ? time.ToString() : null,
					"\nisPlayer = ",
					isPlayer.ToString()
				}), Array.Empty<object>());
				return 0.0;
			}
			return num;
		}

		// Token: 0x06003DF9 RID: 15865 RVA: 0x0018B438 File Offset: 0x00189638
		[return: TupleElementNames(new string[] { "startOrbitalVelocity", "endOrbitalVelocity" })]
		private static ValueTuple<Vector3d, Vector3d> CalculateIdealOrbitalVelocitiesForTorch(ITransferTarget start, ITransferTarget destination, TINaturalSpaceObjectState commonBarycenter, TIDateTime launchTime, TIDateTime arrivalTime)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = ((start.barycenter() == commonBarycenter) ? commonBarycenter : start.barycenterBarycenter());
			CartesianState cartesianState = start.relevantGlobalCartesianState(tinaturalSpaceObjectState, launchTime) - commonBarycenter.ToGlobalCartesianStateAtTime(launchTime);
			IMobileAsset mobileAsset = start as IMobileAsset;
			ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool> destinationLocalOrbitalElementsAtTime = Trajectory.GetDestinationLocalOrbitalElementsAtTime(destination, (mobileAsset != null) ? mobileAsset.faction : null, arrivalTime, null, 0.0);
			CartesianState cartesianState2;
			if (destinationLocalOrbitalElementsAtTime.Item3)
			{
				cartesianState2 = destinationLocalOrbitalElementsAtTime.Item1.ToCartesianStateAtTime(arrivalTime.ExportTime(), destinationLocalOrbitalElementsAtTime.Item2.mass_kg).ToGlobal(destinationLocalOrbitalElementsAtTime.Item2, arrivalTime);
			}
			else
			{
				cartesianState2 = destinationLocalOrbitalElementsAtTime.Item2.ToGlobalCartesianStateAtTime(arrivalTime);
			}
			CartesianState cartesianState3 = cartesianState2 - commonBarycenter.ToGlobalCartesianStateAtTime(launchTime);
			Vector3d normalized = (cartesianState3.position - cartesianState.position).normalized;
			Vector3d normalVector = new OrbitalElementsState(start, 0.0, TITimeState.Now()).normalVector;
			Vector3d normalVector2 = new OrbitalElementsState(destination, 0.0, TITimeState.Now()).normalVector;
			double num = Mathd.Sqrt(start.barycenter().mu / start.a_m());
			double num2 = Mathd.Sqrt(destination.barycenter().mu / destination.a_m());
			Vector3d xzy = (Quaterniond.Inverse(start.barycenter().SpatialRotation) * cartesianState.velocity.xzy).xzy;
			Vector3d xzy2 = (Quaterniond.Inverse(destination.barycenter().SpatialRotation) * cartesianState3.velocity.xzy).xzy;
			Vector3d xzy3 = (Quaterniond.Inverse(start.barycenter().SpatialRotation) * normalized.xzy).xzy;
			Vector3d xzy4 = (Quaterniond.Inverse(destination.barycenter().SpatialRotation) * normalized.xzy).xzy;
			Vector3d vector3d = MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(normalVector, xzy3, xzy, num);
			Vector3d vector3d2 = MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(normalVector2, xzy4, xzy2, num2);
			return new ValueTuple<Vector3d, Vector3d>(vector3d, vector3d2);
		}

		// Token: 0x06003DFA RID: 15866 RVA: 0x0018B640 File Offset: 0x00189840
		[return: TupleElementNames(new string[] { "startOrbitalVelocity", "endOrbitalVelocity" })]
		private static ValueTuple<Vector3d, Vector3d> CalculateIdealOrbitalVelocitiesForTorch(OrbitalElementsState startOrbit, TINaturalSpaceObjectState startBarycenter, OrbitalElementsState destinationOrbit, TINaturalSpaceObjectState destinationBarycenter, TINaturalSpaceObjectState commonBarycenter, TIDateTime launchTime, TIDateTime arrivalTime)
		{
			CartesianState cartesianState;
			if (startBarycenter == commonBarycenter)
			{
				cartesianState = startOrbit.ToCartesianStateAtTime(launchTime.ExportTime(), startBarycenter.mass_kg);
				cartesianState = (startBarycenter.SpatialRotation * cartesianState.xzy).xzy;
			}
			else
			{
				cartesianState = startBarycenter.ToGlobalCartesianStateAtTime(launchTime) - commonBarycenter.ToGlobalCartesianStateAtTime(launchTime);
			}
			CartesianState cartesianState2;
			if (destinationBarycenter == commonBarycenter)
			{
				cartesianState2 = destinationOrbit.ToCartesianStateAtTime(arrivalTime.ExportTime(), destinationBarycenter.mass_kg);
				cartesianState2 = (destinationBarycenter.SpatialRotation * cartesianState2.xzy).xzy;
			}
			else
			{
				cartesianState2 = destinationBarycenter.ToGlobalCartesianStateAtTime(arrivalTime) - commonBarycenter.ToGlobalCartesianStateAtTime(arrivalTime);
			}
			Vector3d normalized = (cartesianState2.position - cartesianState.position).normalized;
			Vector3d normalVector = startOrbit.normalVector;
			Vector3d normalVector2 = destinationOrbit.normalVector;
			double num = Mathd.Sqrt(startBarycenter.mu / startOrbit.semiMajorAxis_m);
			double num2 = Mathd.Sqrt(destinationBarycenter.mu / destinationOrbit.semiMajorAxis_m);
			Vector3d xzy = (Quaterniond.Inverse(startBarycenter.SpatialRotation) * cartesianState.velocity.xzy).xzy;
			Vector3d xzy2 = (Quaterniond.Inverse(destinationBarycenter.SpatialRotation) * cartesianState2.velocity.xzy).xzy;
			Vector3d xzy3 = (Quaterniond.Inverse(startBarycenter.SpatialRotation) * normalized.xzy).xzy;
			Vector3d xzy4 = (Quaterniond.Inverse(destinationBarycenter.SpatialRotation) * normalized.xzy).xzy;
			Vector3d vector3d = MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(normalVector, xzy3, xzy, num);
			Vector3d vector3d2 = MasterTransferPlanner.CalculateIdealOrbitalVelocitiesForTorch(normalVector2, xzy4, xzy2, num2);
			return new ValueTuple<Vector3d, Vector3d>(vector3d, vector3d2);
		}

		// Token: 0x06003DFB RID: 15867 RVA: 0x0018B7FC File Offset: 0x001899FC
		private static Vector3d CalculateIdealOrbitalVelocitiesForTorch(Vector3d orbitNormal, Vector3d idealDirection, Vector3d residualVelocity, double orbitSpeed)
		{
			Vector3d vector3d = Vector3d.Cross(residualVelocity, orbitNormal);
			Vector3d vector3d2 = Vector3d.Cross(idealDirection, orbitNormal);
			Vector3d normalized = vector3d2.normalized;
			vector3d2 = vector3d.normalized;
			double num = Mathd.Acos(Vector3d.Dot(in vector3d2, in normalized));
			double num2 = Mathd.Asin(vector3d.magnitude * Mathd.Sin(num) / orbitSpeed);
			if (!double.IsNaN(num2))
			{
				double num3 = -num2;
				double num4 = 3.141592653589793 - num3;
				double num5 = -num4;
				return new List<ValueTuple<double, Vector3d>>
				{
					new ValueTuple<double, Vector3d>(num2, MasterTransferPlanner.RodrequesRotationFormula(normalized, orbitNormal, num2) * orbitSpeed),
					new ValueTuple<double, Vector3d>(num3, MasterTransferPlanner.RodrequesRotationFormula(normalized, orbitNormal, num3) * orbitSpeed),
					new ValueTuple<double, Vector3d>(num4, MasterTransferPlanner.RodrequesRotationFormula(normalized, orbitNormal, num4) * orbitSpeed),
					new ValueTuple<double, Vector3d>(num5, MasterTransferPlanner.RodrequesRotationFormula(normalized, orbitNormal, num5) * orbitSpeed)
				}.MaxBy<ValueTuple<double, Vector3d>, double>(delegate([TupleElementNames(new string[] { "angle", "orbitVelocity" })] ValueTuple<double, Vector3d> x)
				{
					Vector3d vector3d4 = residualVelocity + x.Item2;
					return Vector3d.Dot(in vector3d4, in idealDirection);
				}).Item2;
			}
			Vector3d vector3d3 = Vector3d.Dot(in residualVelocity, in idealDirection) * idealDirection;
			return -(residualVelocity - vector3d3).normalized * orbitSpeed;
		}

		// Token: 0x06003DFC RID: 15868 RVA: 0x0018B960 File Offset: 0x00189B60
		private static Vector3d RodrequesRotationFormula(Vector3d originalVector, Vector3d rotationAxis, double angle_Rad)
		{
			Vector3d vector3d = originalVector * Mathd.Cos(angle_Rad);
			Vector3d vector3d2 = Vector3d.Cross(rotationAxis, originalVector) * Mathd.Sin(angle_Rad);
			Vector3d vector3d3 = rotationAxis * (Vector3d.Dot(in rotationAxis, in originalVector) * (1.0 - Mathd.Cos(angle_Rad)));
			return vector3d + vector3d2 + vector3d3;
		}

		// Token: 0x06003DFD RID: 15869 RVA: 0x0018B9BC File Offset: 0x00189BBC
		private static double EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(Vector3d velocity, ITransferTarget transferTarget, TIDateTime time, bool isPlayer)
		{
			OrbitalElementsState orbitalElementsState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			bool flag;
			transferTarget.getOrbitalElementsState(time, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
			return MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(velocity, orbitalElementsState, tinaturalSpaceObjectState, time, isPlayer);
		}

		// Token: 0x06003DFE RID: 15870 RVA: 0x0018B9E0 File Offset: 0x00189BE0
		private static double EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(Vector3d velocity, OrbitalElementsState orbitalElements, TINaturalSpaceObjectState barycenter, TIDateTime time, bool isPlayer)
		{
			return orbitalElements.MeanAnomalyWhenClosestToVelocity_Rad(velocity);
		}

		// Token: 0x06003DFF RID: 15871 RVA: 0x0018B9EC File Offset: 0x00189BEC
		private static MasterTransferPlanner.SimplifiedPositions GetSimplifiedPositions(ITransferTarget origin, ITransferTarget destination, TIDateTime time = null, TIDateTime arrivalTime = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			MasterTransferPlanner.SimplifiedPositions simplifiedPositions = new MasterTransferPlanner.SimplifiedPositions();
			TISpaceFleetState tispaceFleetState = origin as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && time >= tispaceFleetState.trajectory.launchTime)
			{
				simplifiedPositions.originDistToLocalBarycenter_m = tispaceFleetState.trajectory.getDistFromBarycenterAtTime_m(time, out simplifiedPositions.originLocalBarycenter);
			}
			else
			{
				simplifiedPositions.originLocalBarycenter = origin.barycenter();
				simplifiedPositions.originDistToLocalBarycenter_m = origin.a_m();
			}
			if (arrivalTime == null)
			{
				arrivalTime = time;
			}
			TISpaceFleetState tispaceFleetState2 = destination as TISpaceFleetState;
			if (tispaceFleetState2 != null && tispaceFleetState2.transferAssigned && arrivalTime >= tispaceFleetState2.trajectory.launchTime)
			{
				simplifiedPositions.destinationDistToLocalBarycenter_m = tispaceFleetState2.trajectory.getDistFromBarycenterAtTime_m(arrivalTime, out simplifiedPositions.destinationLocalBarycenter);
			}
			else
			{
				simplifiedPositions.destinationLocalBarycenter = destination.barycenter();
				simplifiedPositions.destinationDistToLocalBarycenter_m = destination.a_m();
			}
			simplifiedPositions.commonBarycenter = simplifiedPositions.originLocalBarycenter.FindCommonBarycenter(simplifiedPositions.destinationLocalBarycenter);
			if (simplifiedPositions.originLocalBarycenter == simplifiedPositions.commonBarycenter)
			{
				simplifiedPositions.originDistToCommonBarycenter_m = simplifiedPositions.originDistToLocalBarycenter_m;
			}
			else if (simplifiedPositions.originLocalBarycenter.barycenter == simplifiedPositions.commonBarycenter)
			{
				simplifiedPositions.originDistToCommonBarycenter_m = simplifiedPositions.originLocalBarycenter.semiMajorAxis_m;
			}
			else
			{
				simplifiedPositions.originDistToCommonBarycenter_m = simplifiedPositions.originLocalBarycenter.barycenter.semiMajorAxis_m;
			}
			if (simplifiedPositions.destinationLocalBarycenter == simplifiedPositions.commonBarycenter)
			{
				simplifiedPositions.destinationDistToCommonBarycenter_m = simplifiedPositions.destinationDistToLocalBarycenter_m;
			}
			else if (simplifiedPositions.destinationLocalBarycenter.barycenter == simplifiedPositions.commonBarycenter)
			{
				simplifiedPositions.destinationDistToCommonBarycenter_m = simplifiedPositions.destinationLocalBarycenter.semiMajorAxis_m;
			}
			else
			{
				simplifiedPositions.destinationDistToCommonBarycenter_m = simplifiedPositions.destinationLocalBarycenter.barycenter.semiMajorAxis_m;
			}
			return simplifiedPositions;
		}

		// Token: 0x06003E00 RID: 15872 RVA: 0x0018BBA8 File Offset: 0x00189DA8
		private static List<Trajectory_Patched> LoopOverArrivalTimes(int totalTests, double sampleSizeMultiplier, TIDateTime earliestTime, TIDateTime latestTime, List<TIDateTime> timesThatMustBeTested, MasterTransferPlanner.HohmannTiming additionalHohmannTimesToTest, double maxDV_mps, bool stopOnFirstSuccess, TIFactionState faction, out TransferResult result, out double lowestDVfound_mps, [TupleElementNames(new string[] { "launchTime", "stepSize_s", "result", "trajectory" })] Func<TIDateTime, ValueTuple<TIDateTime, double>?, ValueTuple<TransferResult, Trajectory_Patched>> findBestSolution)
		{
			int count = timesThatMustBeTested.Count;
			int num = (totalTests - count) * 3 / 4;
			int num2 = totalTests - count - num;
			double num3 = latestTime.DifferenceInSeconds(earliestTime);
			List<TIDateTime> list = new List<TIDateTime>();
			double num4 = Mathd.Exp(1.0) - 1.0;
			for (int i = 1; i <= num; i++)
			{
				double num5 = (Mathd.Exp((double)i / (double)num) - 1.0) / num4 * num3;
				list.Add(new TIDateTime(earliestTime, num5));
			}
			List<TIDateTime> list2 = new List<TIDateTime>();
			foreach (TIDateTime tidateTime in timesThatMustBeTested)
			{
				if (list.Contains(tidateTime))
				{
					list2.Add(tidateTime);
				}
				else if (!(tidateTime > latestTime) && !(tidateTime < list[0]))
				{
					int j = 0;
					while (j < list.Count)
					{
						if (list[j] > tidateTime)
						{
							if (j == 0)
							{
								break;
							}
							double num6 = list[j].DifferenceInSeconds(list[j - 1]);
							double num7 = tidateTime.DifferenceInSeconds(list[j - 1]) / num6;
							if (num7 < 0.2)
							{
								list2.Add(list[j - 1]);
								break;
							}
							if (num7 > 0.8)
							{
								list2.Add(list[j]);
								break;
							}
							break;
						}
						else
						{
							j++;
						}
					}
				}
			}
			list = list.Except<TIDateTime>(list2).Union<TIDateTime>(timesThatMustBeTested).ToList<TIDateTime>();
			list.Sort();
			num2 = totalTests - list.Count;
			List<ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>> list3 = new List<ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>>();
			list.Reverse();
			foreach (TIDateTime tidateTime2 in list)
			{
				ValueTuple<TransferResult, Trajectory_Patched> valueTuple = findBestSolution(tidateTime2, null);
				if (stopOnFirstSuccess && valueTuple.Item2 != null && valueTuple.Item2.DV_mps <= maxDV_mps)
				{
					result = new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
					lowestDVfound_mps = valueTuple.Item2.DV_mps;
					return new List<Trajectory_Patched> { valueTuple.Item2 };
				}
				if (valueTuple.Item1.Result == TransferResult.Outcome.Success && valueTuple.Item2.DV_mps > maxDV_mps)
				{
					valueTuple.Item1 = new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple.Item2.DV_mps, 0.0);
				}
				list3.Add(new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime2, valueTuple.Item1, valueTuple.Item2));
				if (valueTuple.Item1.Result == TransferResult.Outcome.Fail_ArrivalBeforeLaunch)
				{
					break;
				}
				if (valueTuple.Item1.Result == TransferResult.Outcome.Fail_AttemptedFleetInterceptThatWouldCauseTargetingLoop)
				{
					break;
				}
			}
			list3.Reverse();
			List<ValueTuple<TIDateTime, TIDateTime>> hohmannTimings = additionalHohmannTimesToTest.GetHohmannTimings(sampleSizeMultiplier);
			TIDateTime tidateTime3 = new TIDateTime(TITimeState.Now(), MasterTransferPlanner.TransferDurationHardCap(faction));
			foreach (ValueTuple<TIDateTime, TIDateTime> valueTuple2 in hohmannTimings)
			{
				if (!(valueTuple2.Item2 > tidateTime3))
				{
					ValueTuple<TransferResult, Trajectory_Patched> valueTuple3 = findBestSolution(valueTuple2.Item2, new ValueTuple<TIDateTime, double>?(new ValueTuple<TIDateTime, double>(valueTuple2.Item1, additionalHohmannTimesToTest.synodicPeriod_s)));
					if (valueTuple3.Item1.Result == TransferResult.Outcome.Success && valueTuple3.Item2.DV_mps > maxDV_mps)
					{
						valueTuple3.Item1 = new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple3.Item2.DV_mps, 0.0);
					}
					list3.Add(new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(valueTuple2.Item2, valueTuple3.Item1, valueTuple3.Item2));
				}
			}
			ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple4 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(latestTime, new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null);
			ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple5 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(earliestTime, new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null);
			int k = 0;
			while (k < list3.Count<ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>>())
			{
				if (list3[k].Item3 != null && list3[k].Item3.DV_mps <= maxDV_mps)
				{
					valueTuple4 = list3[k];
					if (k > 0)
					{
						valueTuple5 = list3[k - 1];
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
			ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple6 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(latestTime, new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null);
			ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple7 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(earliestTime, new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null);
			ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple8 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(latestTime, new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null);
			for (int l = 0; l < list3.Count<ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>>(); l++)
			{
				if (list3[l].Item3 != null && (valueTuple6.Item3 == null || list3[l].Item3.DV_mps < valueTuple6.Item3.DV_mps))
				{
					valueTuple6 = list3[l];
					if (l > 0)
					{
						valueTuple7 = list3[l - 1];
					}
					if (l < list3.Count<ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>>() - 1)
					{
						valueTuple8 = list3[l + 1];
					}
					else
					{
						valueTuple8 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(latestTime, new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null);
					}
				}
			}
			if (valueTuple6.Item3 == null)
			{
				TransferResult transferResult = new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				foreach (ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple9 in list3)
				{
					transferResult = TransferResult.Best(transferResult, valueTuple9.Item2);
				}
				result = transferResult;
				lowestDVfound_mps = double.PositiveInfinity;
				return new List<Trajectory_Patched>();
			}
			bool flag = false;
			bool flag2 = false;
			if (stopOnFirstSuccess)
			{
				flag = true;
			}
			int num8 = 0;
			int num9 = 0;
			while (num8 < num2 && num9 < 100 && (!flag || !flag2))
			{
				if (num9 % 2 == 0)
				{
					if (flag || valueTuple4.Item3 == null || valueTuple4.Item3.DV_mps > maxDV_mps * 0.95)
					{
						num8--;
						flag = true;
					}
					else
					{
						TIDateTime tidateTime4 = new TIDateTime(valueTuple5.Item1, valueTuple4.Item1.DifferenceInSeconds(valueTuple5.Item1) / 2.0);
						ValueTuple<TransferResult, Trajectory_Patched> valueTuple10 = findBestSolution(tidateTime4, null);
						if (stopOnFirstSuccess && valueTuple10.Item2 != null && valueTuple10.Item2.DV_mps <= maxDV_mps)
						{
							result = new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
							lowestDVfound_mps = valueTuple10.Item2.DV_mps;
							return new List<Trajectory_Patched> { valueTuple10.Item2 };
						}
						list3.Add(new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime4, valueTuple10.Item1, valueTuple10.Item2));
						if (valueTuple10.Item2 != null && valueTuple10.Item2.DV_mps < maxDV_mps)
						{
							if (Mathd.Abs(valueTuple10.Item2.arrivalTime.DifferenceInSeconds(tidateTime4)) > valueTuple4.Item1.DifferenceInSeconds(valueTuple5.Item1) / 2.0)
							{
								flag = true;
							}
							valueTuple4 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime4, valueTuple10.Item1, valueTuple10.Item2);
						}
						else
						{
							valueTuple5 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime4, valueTuple10.Item1, valueTuple10.Item2);
						}
					}
				}
				else
				{
					if (flag2)
					{
						num8--;
						break;
					}
					double num10 = ((valueTuple7.Item3 == null) ? double.PositiveInfinity : (valueTuple7.Item3.DV_mps / valueTuple6.Item1.DifferenceInSeconds(valueTuple7.Item1)));
					double num11 = ((valueTuple8.Item3 == null) ? double.PositiveInfinity : (valueTuple8.Item3.DV_mps / valueTuple6.Item1.DifferenceInSeconds(valueTuple8.Item1)));
					ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple11 = ((num10 < num11) ? valueTuple7 : valueTuple8);
					ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> valueTuple12 = ((valueTuple11.Item1 == valueTuple7.Item1) ? valueTuple8 : valueTuple7);
					TIDateTime tidateTime5 = new TIDateTime(valueTuple6.Item1, valueTuple11.Item1.DifferenceInSeconds(valueTuple6.Item1) / 2.0);
					ValueTuple<TransferResult, Trajectory_Patched> valueTuple13 = findBestSolution(tidateTime5, null);
					if (valueTuple13.Item1.Result == TransferResult.Outcome.Success && valueTuple13.Item2.DV_mps > maxDV_mps)
					{
						valueTuple13.Item1 = new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple13.Item2.DV_mps, 0.0);
					}
					list3.Add(new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime5, valueTuple13.Item1, valueTuple13.Item2));
					if (valueTuple13.Item2 != null && valueTuple13.Item2.DV_mps < valueTuple6.Item3.DV_mps)
					{
						if (valueTuple11.Item1 < tidateTime5)
						{
							valueTuple7 = valueTuple11;
							valueTuple8 = valueTuple6;
						}
						else
						{
							valueTuple7 = valueTuple6;
							valueTuple8 = valueTuple11;
						}
						valueTuple6 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime5, valueTuple13.Item1, valueTuple13.Item2);
						if (stopOnFirstSuccess && valueTuple13.Item2 != null && valueTuple13.Item2.DV_mps <= maxDV_mps)
						{
							result = new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
							lowestDVfound_mps = valueTuple13.Item2.DV_mps;
							return new List<Trajectory_Patched> { valueTuple13.Item2 };
						}
						list3.Add(new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime5, valueTuple13.Item1, valueTuple13.Item2));
					}
					else
					{
						num8++;
						TIDateTime tidateTime6 = new TIDateTime(valueTuple6.Item1, valueTuple12.Item1.DifferenceInSeconds(valueTuple6.Item1) / 2.0);
						ValueTuple<TransferResult, Trajectory_Patched> valueTuple14 = findBestSolution(tidateTime6, null);
						if (valueTuple14.Item1.Result == TransferResult.Outcome.Success && valueTuple14.Item2.DV_mps > maxDV_mps)
						{
							valueTuple14.Item1 = new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple14.Item2.DV_mps, 0.0);
						}
						if (valueTuple14.Item2 != null && valueTuple14.Item2.DV_mps < valueTuple6.Item3.DV_mps)
						{
							if (valueTuple12.Item1 < tidateTime5)
							{
								valueTuple7 = valueTuple12;
								valueTuple8 = valueTuple6;
							}
							else
							{
								valueTuple7 = valueTuple6;
								valueTuple8 = valueTuple12;
							}
							valueTuple6 = new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime6, valueTuple14.Item1, valueTuple14.Item2);
							if (stopOnFirstSuccess && valueTuple14.Item2 != null && valueTuple14.Item2.DV_mps <= maxDV_mps)
							{
								result = new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
								lowestDVfound_mps = valueTuple14.Item2.DV_mps;
								return new List<Trajectory_Patched> { valueTuple14.Item2 };
							}
							list3.Add(new ValueTuple<TIDateTime, TransferResult, Trajectory_Patched>(tidateTime6, valueTuple14.Item1, valueTuple14.Item2));
						}
						else if (valueTuple12.Item1 < tidateTime5)
						{
							valueTuple7 = valueTuple12;
							valueTuple8 = valueTuple11;
						}
						else
						{
							valueTuple7 = valueTuple11;
							valueTuple8 = valueTuple12;
						}
					}
					double num12 = valueTuple6.Item1.DifferenceInSeconds(valueTuple6.Item3.arrivalTime);
					if (Mathd.Abs(valueTuple7.Item1.DifferenceInSeconds(valueTuple6.Item1)) < num12 || Mathd.Abs(valueTuple8.Item1.DifferenceInSeconds(valueTuple6.Item1)) < num12)
					{
						flag2 = true;
					}
				}
				num8++;
				num9++;
			}
			result = list3.Aggregate(new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), (TransferResult best, [TupleElementNames(new string[] { "arrivalTime", "result", "trajectory" })] ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> next) => best = TransferResult.Best(best, next.Item2));
			lowestDVfound_mps = list3.Aggregate(double.PositiveInfinity, delegate(double best, [TupleElementNames(new string[] { "arrivalTime", "result", "trajectory" })] ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> next)
			{
				double num13 = best;
				Trajectory_Patched item = next.Item3;
				return best = Mathd.Min(num13, (item != null) ? item.DV_mps : double.PositiveInfinity);
			});
			if (lowestDVfound_mps == double.PositiveInfinity)
			{
				lowestDVfound_mps = list3.Aggregate(lowestDVfound_mps, delegate(double best, [TupleElementNames(new string[] { "arrivalTime", "result", "trajectory" })] ValueTuple<TIDateTime, TransferResult, Trajectory_Patched> next)
				{
					double num14;
					return best = Mathd.Min(best, next.Item2.TryGetMinimumDVneeded_mps(out num14) ? num14 : double.PositiveInfinity);
				});
			}
			return (from x in list3
				where x.Item3 != null && x.Item3.DV_mps <= maxDV_mps
				orderby x.Item3.arrivalTime
				select x.Item3).ToList<Trajectory_Patched>();
		}

		// Token: 0x06003E01 RID: 15873 RVA: 0x0018C924 File Offset: 0x0018AB24
		[return: TupleElementNames(new string[] { "output", "launchTime", "result", "transfer", "commonBarycenter" })]
		private static ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime> BestTransferResult([TupleElementNames(new string[] { "output", "launchTime", "result", "transfer", "commonBarycenter" })] ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime> a, [TupleElementNames(new string[] { "output", "launchTime", "result", "transfer", "commonBarycenter" })] ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime> b)
		{
			if (a.Item1.Item1.Result == TransferResult.Outcome.Success && b.Item1.Item1.Result == TransferResult.Outcome.Success)
			{
				if (a.Item1.Item2.DV_mps < b.Item1.Item2.DV_mps)
				{
					return a;
				}
				return b;
			}
			else
			{
				if (TransferResult.Best(a.Item1.Item1, b.Item1.Item1) == a.Item1.Item1)
				{
					return a;
				}
				return b;
			}
		}

		// Token: 0x06003E02 RID: 15874 RVA: 0x0018C9A8 File Offset: 0x0018ABA8
		[return: TupleElementNames(new string[] { "result", "trajectory", "bestDV_mps" })]
		private static ValueTuple<TransferResult, Trajectory_Patched, double> OptimizeLaunchTime(TIDateTime expectedBestLaunchTime, TIDateTime earliestLaunchTime, TIDateTime latestLaunchTime, TIDateTime arrivalTime, [TupleElementNames(new string[] { "launchTime", "stepSize_s" })] ValueTuple<TIDateTime, double>? lockedLaunchTime, MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, string transferType, [TupleElementNames(new string[] { "lambertAnomalyAtArrival_Rad", "lambertLaunchTime", "lambertArrivalTime", "torchAnomalyAtArrival_Rad", "torchLaunchTime", "torchArrivalTime" })] Func<TIDateTime, TIDateTime, double, ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>> calcMeanAnomalyLaunchAndArrivalTime)
		{
			if (arrivalTime <= earliestLaunchTime)
			{
				return new ValueTuple<TransferResult, Trajectory_Patched, double>(new TransferResult(TransferResult.Outcome.Fail_ArrivalBeforeLaunch, earliestLaunchTime.DifferenceInSeconds(arrivalTime), 0.0), null, double.PositiveInfinity);
			}
			if (expectedBestLaunchTime >= arrivalTime)
			{
				expectedBestLaunchTime = earliestLaunchTime;
			}
			if (latestLaunchTime >= arrivalTime)
			{
				latestLaunchTime = new TIDateTime(arrivalTime, -1.0);
			}
			double num = Mathd.Max(latestLaunchTime.DifferenceInSeconds(expectedBestLaunchTime), expectedBestLaunchTime.DifferenceInSeconds(earliestLaunchTime));
			if (lockedLaunchTime != null)
			{
				expectedBestLaunchTime = ((lockedLaunchTime != null) ? lockedLaunchTime.GetValueOrDefault().Item1 : null);
				num = ((lockedLaunchTime != null) ? lockedLaunchTime.GetValueOrDefault().Item2 : 0.0);
			}
			ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime> valueTuple = new ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime>(MasterTransferPlanner.TryGetTransfer(expectedBestLaunchTime, arrivalTime, earliestLaunchTime, param, transferType, num, calcMeanAnomalyLaunchAndArrivalTime), expectedBestLaunchTime);
			if (lockedLaunchTime != null)
			{
				if (valueTuple.Item1.Item2 == null)
				{
					if (valueTuple.Item1.Item1.Result == TransferResult.Outcome.Success)
					{
						valueTuple.Item1.Item1 = new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
					}
					return new ValueTuple<TransferResult, Trajectory_Patched, double>(valueTuple.Item1.Item1, null, double.PositiveInfinity);
				}
				if (valueTuple.Item1.Item2.DV_mps > param.fleetDeltaV_mps)
				{
					StreamWriter log = param.log;
					if (log != null)
					{
						string[] array = new string[12];
						array[0] = "FAIL: insufficient DV,";
						array[1] = transferType;
						array[2] = ",";
						int num2 = 3;
						TIDateTime launchTime = valueTuple.Item1.Item2.launchTime;
						array[num2] = ((launchTime != null) ? launchTime.ToString() : null);
						array[4] = ",";
						int num3 = 5;
						TIDateTime arrivalTime2 = valueTuple.Item1.Item2.arrivalTime;
						array[num3] = ((arrivalTime2 != null) ? arrivalTime2.ToString() : null);
						array[6] = ",";
						array[7] = valueTuple.Item1.Item2.DV_mps.ToString();
						array[8] = ",";
						array[9] = valueTuple.Item1.Item2.boost_DV_mps.ToString();
						array[10] = ",";
						array[11] = valueTuple.Item1.Item2.decel_DV_mps.ToString();
						log.WriteLine(string.Concat(array));
					}
					return new ValueTuple<TransferResult, Trajectory_Patched, double>(new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple.Item1.Item2.DV_mps, 0.0), null, valueTuple.Item1.Item2.DV_mps);
				}
				Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
				trajectory_Patched.BuildSingleTrajectory(param.fleet, param.sDestination, param.originValue, param.destinationValue, valueTuple.Item1.Item3, valueTuple.Item1.Item2, param.fleetAcceleration_mps2);
				return new ValueTuple<TransferResult, Trajectory_Patched, double>(valueTuple.Item1.Item1, trajectory_Patched, valueTuple.Item1.Item2.DV_mps);
			}
			else
			{
				TIDateTime tidateTime = null;
				TISpaceFleetState tispaceFleetState = param.originValue as TISpaceFleetState;
				if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.destroyOnArrival)
				{
					tidateTime = tispaceFleetState.trajectory.arrivalTime;
				}
				if (valueTuple.Item1.Item2 != null && valueTuple.Item1.Item2.launchTime < earliestLaunchTime)
				{
					valueTuple.Item1.Item1 = new TransferResult(TransferResult.Outcome.Fail_LaunchInPast, valueTuple.Item1.Item2.launchTime.DifferenceInSeconds(earliestLaunchTime), valueTuple.Item1.Item2.boost_DV_mps);
					valueTuple.Item1.Item2 = null;
				}
				int i = Mathd.CeilToInt((double)(param.fleet.faction.isActivePlayer ? 7 : 5) * param.sampleSizeMultiplier);
				while (i > 0)
				{
					i--;
					num /= 2.0;
					if (num < 600.0)
					{
						break;
					}
					TIDateTime tidateTime2 = new TIDateTime(valueTuple.Item2, -num);
					TIDateTime tidateTime3 = new TIDateTime(valueTuple.Item2, num);
					if (tidateTime2 >= earliestLaunchTime && tidateTime2 < arrivalTime)
					{
						ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState> valueTuple2 = MasterTransferPlanner.TryGetTransfer(tidateTime2, arrivalTime, earliestLaunchTime, param, transferType, num, calcMeanAnomalyLaunchAndArrivalTime);
						valueTuple = MasterTransferPlanner.BestTransferResult(valueTuple, new ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime>(valueTuple2, tidateTime2));
						if (valueTuple2.Item2 != null && (valueTuple.Item1.Item2 == null || valueTuple2.Item2.DV_mps < valueTuple.Item1.Item2.DV_mps) && valueTuple2.Item2.launchTime < valueTuple2.Item2.arrivalTime && param.stopOnFirstSuccess && valueTuple2.Item2.DV_mps <= param.fleetDeltaV_mps)
						{
							break;
						}
					}
					if (tidateTime3 >= earliestLaunchTime && tidateTime3 < arrivalTime && (tidateTime == null || tidateTime3 < tidateTime))
					{
						ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState> valueTuple3 = MasterTransferPlanner.TryGetTransfer(tidateTime3, arrivalTime, earliestLaunchTime, param, transferType, num, calcMeanAnomalyLaunchAndArrivalTime);
						valueTuple = MasterTransferPlanner.BestTransferResult(valueTuple, new ValueTuple<ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>, TIDateTime>(valueTuple3, tidateTime3));
						if (valueTuple3.Item2 != null && valueTuple3.Item2.launchTime > earliestLaunchTime && (valueTuple.Item1.Item2 == null || valueTuple3.Item2.DV_mps < valueTuple.Item1.Item2.DV_mps) && valueTuple3.Item2.launchTime < valueTuple3.Item2.arrivalTime && param.stopOnFirstSuccess && valueTuple3.Item2.DV_mps <= param.fleetDeltaV_mps)
						{
							break;
						}
					}
				}
				if (valueTuple.Item1.Item2 == null)
				{
					StreamWriter log2 = param.log;
					if (log2 != null)
					{
						string text = "FAIL,";
						string text2 = ", result:, ";
						TransferResult item = valueTuple.Item1.Item1;
						log2.WriteLine(text + transferType + text2 + ((item != null) ? item.ToString() : null));
					}
					return new ValueTuple<TransferResult, Trajectory_Patched, double>(valueTuple.Item1.Item1, null, double.PositiveInfinity);
				}
				if (valueTuple.Item1.Item2.DV_mps > param.fleetDeltaV_mps)
				{
					StreamWriter log3 = param.log;
					if (log3 != null)
					{
						string[] array2 = new string[12];
						array2[0] = "FAIL: insufficient DV,";
						array2[1] = transferType;
						array2[2] = ",";
						int num4 = 3;
						TIDateTime launchTime2 = valueTuple.Item1.Item2.launchTime;
						array2[num4] = ((launchTime2 != null) ? launchTime2.ToString() : null);
						array2[4] = ",";
						int num5 = 5;
						TIDateTime arrivalTime3 = valueTuple.Item1.Item2.arrivalTime;
						array2[num5] = ((arrivalTime3 != null) ? arrivalTime3.ToString() : null);
						array2[6] = ",";
						array2[7] = valueTuple.Item1.Item2.DV_mps.ToString();
						array2[8] = ",";
						array2[9] = valueTuple.Item1.Item2.boost_DV_mps.ToString();
						array2[10] = ",";
						array2[11] = valueTuple.Item1.Item2.decel_DV_mps.ToString();
						log3.WriteLine(string.Concat(array2));
					}
					return new ValueTuple<TransferResult, Trajectory_Patched, double>(new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, valueTuple.Item1.Item2.DV_mps, 0.0), null, valueTuple.Item1.Item2.DV_mps);
				}
				if (valueTuple.Item1.Item2.arrivalTime <= valueTuple.Item1.Item2.launchTime)
				{
					StreamWriter log4 = param.log;
					if (log4 != null)
					{
						string[] array3 = new string[6];
						array3[0] = "FAIL: launch and arrival times matched,";
						array3[1] = transferType;
						array3[2] = ",";
						int num6 = 3;
						TIDateTime launchTime3 = valueTuple.Item1.Item2.launchTime;
						array3[num6] = ((launchTime3 != null) ? launchTime3.ToString() : null);
						array3[4] = ",";
						int num7 = 5;
						TIDateTime arrivalTime4 = valueTuple.Item1.Item2.arrivalTime;
						array3[num7] = ((arrivalTime4 != null) ? arrivalTime4.ToString() : null);
						log4.WriteLine(string.Concat(array3));
					}
					return new ValueTuple<TransferResult, Trajectory_Patched, double>(new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null, valueTuple.Item1.Item2.DV_mps);
				}
				Trajectory_Patched trajectory_Patched2 = new Trajectory_Patched();
				trajectory_Patched2.BuildSingleTrajectory(param.fleet, param.sDestination, param.originValue, param.destinationValue, valueTuple.Item1.Item3, valueTuple.Item1.Item2, param.fleetAcceleration_mps2);
				StreamWriter log5 = param.log;
				if (log5 != null)
				{
					string[] array4 = new string[20];
					array4[0] = "SUCCESS,";
					array4[1] = transferType;
					array4[2] = ",";
					int num8 = 3;
					TIDateTime launchTime4 = valueTuple.Item1.Item2.launchTime;
					array4[num8] = ((launchTime4 != null) ? launchTime4.ToString() : null);
					array4[4] = ",";
					int num9 = 5;
					TIDateTime arrivalTime5 = valueTuple.Item1.Item2.arrivalTime;
					array4[num9] = ((arrivalTime5 != null) ? arrivalTime5.ToString() : null);
					array4[6] = ",";
					array4[7] = valueTuple.Item1.Item2.DV_mps.ToString();
					array4[8] = ",";
					int num10 = 9;
					TIDateTime launchTime5 = trajectory_Patched2.launchTime;
					array4[num10] = ((launchTime5 != null) ? launchTime5.ToString() : null);
					array4[10] = ",";
					int num11 = 11;
					TIDateTime arrivalTime6 = trajectory_Patched2.arrivalTime;
					array4[num11] = ((arrivalTime6 != null) ? arrivalTime6.ToString() : null);
					array4[12] = ",";
					array4[13] = trajectory_Patched2.DV_mps.ToString();
					array4[14] = ",";
					array4[15] = valueTuple.Item1.Item2.boost_DV_mps.ToString();
					array4[16] = ",";
					array4[17] = valueTuple.Item1.Item2.decel_DV_mps.ToString();
					array4[18] = ",";
					array4[19] = trajectory_Patched2.DumpSegments();
					log5.WriteLine(string.Concat(array4));
				}
				return new ValueTuple<TransferResult, Trajectory_Patched, double>(new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0), trajectory_Patched2, valueTuple.Item1.Item2.DV_mps);
			}
		}

		// Token: 0x06003E03 RID: 15875 RVA: 0x0018D364 File Offset: 0x0018B564
		[return: TupleElementNames(new string[] { "result", "transfer", "commonBarycenter" })]
		private static ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState> TryGetTransfer(TIDateTime attemptedLaunchTime, TIDateTime arrivalTime, TIDateTime earliestLaunchTime, MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, string transferType, double stepSize_s, [TupleElementNames(new string[] { "lambertAnomalyAtArrival_Rad", "lambertLaunchTime", "lambertArrivalTime", "torchAnomalyAtArrival_Rad", "torchLaunchTime", "torchArrivalTime" })] Func<TIDateTime, TIDateTime, double, ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime>> calcMeanAnomalyLaunchAndArrivalTime)
		{
			TransferResult transferResult = null;
			TransferResult transferResult2 = null;
			ValueTuple<double, TIDateTime, TIDateTime, double, TIDateTime, TIDateTime> valueTuple = calcMeanAnomalyLaunchAndArrivalTime(attemptedLaunchTime, arrivalTime, stepSize_s);
			TIDateTime tidateTime = new TIDateTime(valueTuple.Item3);
			PatchedTransfer patchedTransfer = new PatchedTransfer();
			TISpaceFleetState tispaceFleetState = param.destinationValue as TISpaceFleetState;
			bool flag = MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, param.fleet.faction);
			if (flag)
			{
				MasterTransferPlanner.DoesOrbitMatch(param.fleet, tispaceFleetState.trajectory.originOrbit);
			}
			if (flag)
			{
				MasterTransferPlanner.DoesOrbitMatch(param.fleet, tispaceFleetState.trajectory.destinationOrbit);
			}
			if (flag && tidateTime > tispaceFleetState.trajectory.arrivalTime && !tispaceFleetState.trajectory.targetingOrbit)
			{
				transferResult = new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptAfterArrivalAtAsset, tidateTime.DifferenceInSeconds(tispaceFleetState.trajectory.arrivalTime), 0.0);
			}
			else
			{
				bool flag2 = true;
				if (flag && MasterTransferPlanner.DoesOrbitMatch(param.fleet, tispaceFleetState.trajectory.originOrbit) && valueTuple.Item3 < tispaceFleetState.trajectory.launchTime)
				{
					flag2 = false;
				}
				if (flag && MasterTransferPlanner.DoesOrbitMatch(param.fleet, tispaceFleetState.trajectory.destinationOrbit) && valueTuple.Item3 > tispaceFleetState.trajectory.arrivalTime)
				{
					flag2 = false;
				}
				if (param.commonBarycenter.isLagrangePointState)
				{
					flag2 = false;
				}
				if (flag2)
				{
					TIDateTime tidateTime2 = (flag ? tidateTime : TITimeState.Now());
					ITransferTarget destinationValue = param.destinationValue;
					TISpaceFleetState tispaceFleetState2 = param.originValue as TISpaceFleetState;
					ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool> destinationLocalOrbitalElementsAtTime = Trajectory.GetDestinationLocalOrbitalElementsAtTime(destinationValue, (tispaceFleetState2 != null) ? tispaceFleetState2.faction : null, tidateTime2, null, valueTuple.Item1);
					OrbitalElementsState item = destinationLocalOrbitalElementsAtTime.Item1;
					TINaturalSpaceObjectState item2 = destinationLocalOrbitalElementsAtTime.Item2;
					OrbitalElementsState orbitalElementsState;
					TINaturalSpaceObjectState tinaturalSpaceObjectState;
					bool flag3;
					MasterTransferPlanner.GetOriginOrbitalElementsState(param.originValue, valueTuple.Item2, out orbitalElementsState, out tinaturalSpaceObjectState, out flag3);
					TINaturalSpaceObjectState tinaturalSpaceObjectState2 = item2.FindCommonBarycenter(tinaturalSpaceObjectState);
					PatchedTransfer patchedTransfer2 = patchedTransfer;
					TIDateTime item3 = valueTuple.Item2;
					TIDateTime tidateTime3 = tidateTime;
					ITransferTarget originValue = param.originValue;
					OrbitalElementsState orbitalElementsState2 = item;
					TINaturalSpaceObjectState tinaturalSpaceObjectState3 = item2;
					TINaturalSpaceObjectState tinaturalSpaceObjectState4 = tinaturalSpaceObjectState2;
					double fleetAcceleration_mps = param.fleetAcceleration_mps2;
					bool flag4 = false;
					PatchedTransfer.InternalTransferType internalTransferType = PatchedTransfer.InternalTransferType.Lambert;
					TIDateTime tidateTime4;
					if (tispaceFleetState == null)
					{
						tidateTime4 = null;
					}
					else
					{
						Trajectory trajectory = tispaceFleetState.trajectory;
						tidateTime4 = ((trajectory != null) ? trajectory.arrivalTime : null);
					}
					transferResult = patchedTransfer2.Solve(item3, tidateTime3, originValue, orbitalElementsState2, tinaturalSpaceObjectState3, tinaturalSpaceObjectState4, fleetAcceleration_mps, flag4, internalTransferType, tidateTime4);
					if (transferResult.Result == TransferResult.Outcome.Success)
					{
						if (patchedTransfer.launchTime >= earliestLaunchTime)
						{
							StreamWriter log = param.log;
							if (log != null)
							{
								string[] array = new string[8];
								array[0] = "SUCCESS,";
								array[1] = transferType;
								array[2] = "Lambert,";
								int num = 3;
								TIDateTime launchTime = patchedTransfer.launchTime;
								array[num] = ((launchTime != null) ? launchTime.ToString() : null);
								array[4] = ",";
								int num2 = 5;
								TIDateTime arrivalTime2 = patchedTransfer.arrivalTime;
								array[num2] = ((arrivalTime2 != null) ? arrivalTime2.ToString() : null);
								array[6] = ",";
								array[7] = patchedTransfer.DV_mps.ToString();
								log.WriteLine(string.Concat(array));
							}
							return new ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>(transferResult, patchedTransfer, tinaturalSpaceObjectState2);
						}
						StreamWriter log2 = param.log;
						if (log2 != null)
						{
							string[] array2 = new string[10];
							array2[0] = "FAIL: patched transfer failed due to launch time being to early,";
							array2[1] = transferType;
							array2[2] = "Lambert,";
							int num3 = 3;
							TIDateTime item4 = valueTuple.Item2;
							array2[num3] = ((item4 != null) ? item4.ToString() : null);
							array2[4] = ",";
							int num4 = 5;
							TIDateTime item5 = valueTuple.Item3;
							array2[num4] = ((item5 != null) ? item5.ToString() : null);
							array2[6] = ",";
							array2[7] = patchedTransfer.DV_mps.ToString();
							array2[8] = ",earliest launch time = ";
							array2[9] = ((earliestLaunchTime != null) ? earliestLaunchTime.ToString() : null);
							log2.WriteLine(string.Concat(array2));
						}
					}
					else
					{
						StreamWriter log3 = param.log;
						if (log3 != null)
						{
							string[] array3 = new string[10];
							array3[0] = "FAIL: patched transfer failed,";
							array3[1] = transferType;
							array3[2] = "Lambert,";
							int num5 = 3;
							TIDateTime item6 = valueTuple.Item2;
							array3[num5] = ((item6 != null) ? item6.ToString() : null);
							array3[4] = ",";
							int num6 = 5;
							TIDateTime item7 = valueTuple.Item3;
							array3[num6] = ((item7 != null) ? item7.ToString() : null);
							array3[6] = ",";
							array3[7] = patchedTransfer.DV_mps.ToString();
							array3[8] = ",";
							int num7 = 9;
							TransferResult transferResult3 = transferResult;
							array3[num7] = ((transferResult3 != null) ? transferResult3.ToString() : null);
							log3.WriteLine(string.Concat(array3));
						}
					}
				}
			}
			if (transferResult == null || transferResult.Result != TransferResult.Outcome.Success)
			{
				bool flag5 = true;
				if (flag && MasterTransferPlanner.DoesOrbitMatch(param.fleet, tispaceFleetState.trajectory.originOrbit) && valueTuple.Item6 < tispaceFleetState.trajectory.launchTime)
				{
					flag5 = false;
				}
				if (flag && MasterTransferPlanner.DoesOrbitMatch(param.fleet, tispaceFleetState.trajectory.destinationOrbit) && valueTuple.Item6 > tispaceFleetState.trajectory.arrivalTime)
				{
					flag5 = false;
				}
				if (flag5)
				{
					TIDateTime tidateTime5 = new TIDateTime(valueTuple.Item6);
					if (flag && tidateTime5 > tispaceFleetState.trajectory.arrivalTime && !tispaceFleetState.trajectory.targetingOrbit)
					{
						return new ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>(TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Fail_AttemptedFleetInterceptAfterArrivalAtAsset, tidateTime5.DifferenceInSeconds(tispaceFleetState.trajectory.arrivalTime), 0.0)), null, null);
					}
					TIDateTime tidateTime6 = (flag ? tidateTime5 : TITimeState.Now());
					ITransferTarget destinationValue2 = param.destinationValue;
					TISpaceFleetState tispaceFleetState3 = param.originValue as TISpaceFleetState;
					ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool> destinationLocalOrbitalElementsAtTime2 = Trajectory.GetDestinationLocalOrbitalElementsAtTime(destinationValue2, (tispaceFleetState3 != null) ? tispaceFleetState3.faction : null, tidateTime6, param.now, valueTuple.Item4);
					OrbitalElementsState item8 = destinationLocalOrbitalElementsAtTime2.Item1;
					TINaturalSpaceObjectState item9 = destinationLocalOrbitalElementsAtTime2.Item2;
					bool flag3;
					OrbitalElementsState orbitalElementsState3;
					TINaturalSpaceObjectState tinaturalSpaceObjectState5;
					MasterTransferPlanner.GetOriginOrbitalElementsState(param.originValue, valueTuple.Item5, out orbitalElementsState3, out tinaturalSpaceObjectState5, out flag3);
					TINaturalSpaceObjectState tinaturalSpaceObjectState6 = item9.FindCommonBarycenter(tinaturalSpaceObjectState5);
					transferResult2 = patchedTransfer.Solve(valueTuple.Item5, tidateTime5, param.originValue, item8, item9, tinaturalSpaceObjectState6, param.fleetAcceleration_mps2, false, PatchedTransfer.InternalTransferType.Torch, null);
					if (transferResult2.Result == TransferResult.Outcome.Success && patchedTransfer.launchTime >= earliestLaunchTime)
					{
						StreamWriter log4 = param.log;
						if (log4 != null)
						{
							string[] array4 = new string[8];
							array4[0] = "SUCCESS,";
							array4[1] = transferType;
							array4[2] = "Torch,";
							int num8 = 3;
							TIDateTime launchTime2 = patchedTransfer.launchTime;
							array4[num8] = ((launchTime2 != null) ? launchTime2.ToString() : null);
							array4[4] = ",";
							int num9 = 5;
							TIDateTime arrivalTime3 = patchedTransfer.arrivalTime;
							array4[num9] = ((arrivalTime3 != null) ? arrivalTime3.ToString() : null);
							array4[6] = ",";
							array4[7] = patchedTransfer.DV_mps.ToString();
							log4.WriteLine(string.Concat(array4));
						}
						return new ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>(transferResult2, patchedTransfer, tinaturalSpaceObjectState6);
					}
					StreamWriter log5 = param.log;
					if (log5 != null)
					{
						string[] array5 = new string[10];
						array5[0] = "FAIL: patched transfer failed,";
						array5[1] = transferType;
						array5[2] = "Torch,";
						int num10 = 3;
						TIDateTime item10 = valueTuple.Item5;
						array5[num10] = ((item10 != null) ? item10.ToString() : null);
						array5[4] = ",";
						int num11 = 5;
						TIDateTime item11 = valueTuple.Item6;
						array5[num11] = ((item11 != null) ? item11.ToString() : null);
						array5[6] = ",";
						array5[7] = patchedTransfer.DV_mps.ToString();
						array5[8] = ",";
						int num12 = 9;
						TransferResult transferResult4 = transferResult2;
						array5[num12] = ((transferResult4 != null) ? transferResult4.ToString() : null);
						log5.WriteLine(string.Concat(array5));
					}
				}
			}
			if (transferResult == null && transferResult2 == null)
			{
				return new ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>(new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0), null, null);
			}
			return new ValueTuple<TransferResult, PatchedTransfer, TINaturalSpaceObjectState>(TransferResult.Best(transferResult, transferResult2), null, null);
		}

		// Token: 0x06003E04 RID: 15876 RVA: 0x0018DA54 File Offset: 0x0018BC54
		private static CartesianState GlobalToLocal(CartesianState state, TINaturalSpaceObjectState localBarycenter, TIDateTime time)
		{
			CartesianState cartesianState = state - localBarycenter.ToGlobalCartesianStateAtTime(time);
			cartesianState.position = (Quaterniond.Inverse(localBarycenter.SpatialRotation) * cartesianState.position.xzy).xzy;
			cartesianState.velocity = (Quaterniond.Inverse(localBarycenter.SpatialRotation) * cartesianState.velocity.xzy).xzy;
			return cartesianState;
		}

		// Token: 0x06003E05 RID: 15877 RVA: 0x0018DAC8 File Offset: 0x0018BCC8
		[return: TupleElementNames(new string[] { "lambertLaunchTime", "lambertArrivalTime", "lambertDestinationOrbit", "torchLaunchTime", "torchArrivalTime", "torchDestinationOrbit" })]
		private static ValueTuple<TIDateTime, TIDateTime, OrbitalElementsState, TIDateTime, TIDateTime, OrbitalElementsState> CreateTransferParamsForOptimizeArrivalMeanAnomaly(TIDateTime launchTime, TIDateTime arrivalTime, double meanAnomaly, ITransferTarget destination, [TupleElementNames(new string[] { "lambert", "torch" })] Func<TIDateTime, TIDateTime, double, ValueTuple<TIDateTime, TIDateTime>> calcLaunchTime)
		{
			ValueTuple<TIDateTime, TIDateTime> valueTuple = calcLaunchTime(launchTime, arrivalTime, meanAnomaly);
			OrbitalElementsState orbitalElementsState = new OrbitalElementsState(destination, meanAnomaly, arrivalTime);
			OrbitalElementsState orbitalElementsState2 = orbitalElementsState;
			TIDateTime tidateTime = arrivalTime;
			TIDateTime tidateTime2 = arrivalTime;
			if (valueTuple.Item1 != launchTime)
			{
				tidateTime = new TIDateTime(arrivalTime, valueTuple.Item1.DifferenceInSeconds(launchTime));
				orbitalElementsState = new OrbitalElementsState(destination, meanAnomaly, tidateTime);
			}
			if (valueTuple.Item2 != launchTime)
			{
				tidateTime2 = new TIDateTime(arrivalTime, valueTuple.Item2.DifferenceInSeconds(launchTime));
				orbitalElementsState2 = new OrbitalElementsState(destination, meanAnomaly, tidateTime2);
			}
			return new ValueTuple<TIDateTime, TIDateTime, OrbitalElementsState, TIDateTime, TIDateTime, OrbitalElementsState>(valueTuple.Item1, tidateTime, orbitalElementsState, valueTuple.Item2, tidateTime2, orbitalElementsState2);
		}

		// Token: 0x06003E06 RID: 15878 RVA: 0x0018DB60 File Offset: 0x0018BD60
		private static void TestArrivalMeanAnomaly([TupleElementNames(new string[] { "transfer", "meanAnomaly_Rad" })] ref ValueTuple<PatchedTransfer, double> alignment, TIDateTime launchTime, TIDateTime arrivalTime, MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, string transferType, [TupleElementNames(new string[] { "lambert", "torch" })] Func<TIDateTime, TIDateTime, double, ValueTuple<TIDateTime, TIDateTime>> calcLaunchTime)
		{
			ValueTuple<TIDateTime, TIDateTime, OrbitalElementsState, TIDateTime, TIDateTime, OrbitalElementsState> valueTuple = MasterTransferPlanner.CreateTransferParamsForOptimizeArrivalMeanAnomaly(launchTime, arrivalTime, alignment.Item2, param.destinationValue, calcLaunchTime);
			PatchedTransfer patchedTransfer = new PatchedTransfer();
			TransferResult transferResult = patchedTransfer.Solve(valueTuple.Item1, valueTuple.Item2, param.originValue, valueTuple.Item3, param.destinationValue.barycenter(), param.commonBarycenter, param.fleetAcceleration_mps2, false, PatchedTransfer.InternalTransferType.Lambert, null);
			if (transferResult.Result != TransferResult.Outcome.Success)
			{
				StreamWriter log = param.log;
				if (log != null)
				{
					string[] array = new string[12];
					array[0] = "FAIL: patched transfer failed,";
					array[1] = transferType;
					array[2] = "Lambert-";
					array[3] = (alignment.Item2 * 57.29577951308232).ToString();
					array[4] = "°,";
					int num = 5;
					TIDateTime item = valueTuple.Item1;
					array[num] = ((item != null) ? item.ToString() : null);
					array[6] = ",";
					int num2 = 7;
					TIDateTime item2 = valueTuple.Item2;
					array[num2] = ((item2 != null) ? item2.ToString() : null);
					array[8] = ",";
					array[9] = patchedTransfer.DV_mps.ToString();
					array[10] = ",";
					int num3 = 11;
					TransferResult transferResult2 = transferResult;
					array[num3] = ((transferResult2 != null) ? transferResult2.ToString() : null);
					log.WriteLine(string.Concat(array));
				}
				transferResult = patchedTransfer.Solve(valueTuple.Item4, valueTuple.Item5, param.originValue, valueTuple.Item6, param.destinationValue.barycenter(), param.commonBarycenter, param.fleetAcceleration_mps2, false, PatchedTransfer.InternalTransferType.Torch, null);
				if (transferResult.Result == TransferResult.Outcome.Success)
				{
					StreamWriter log2 = param.log;
					if (log2 != null)
					{
						string[] array2 = new string[10];
						array2[0] = "SUCCESS,";
						array2[1] = transferType;
						array2[2] = "Torch-";
						array2[3] = (alignment.Item2 * 57.29577951308232).ToString();
						array2[4] = "°,";
						int num4 = 5;
						TIDateTime item3 = valueTuple.Item4;
						array2[num4] = ((item3 != null) ? item3.ToString() : null);
						array2[6] = ",";
						int num5 = 7;
						TIDateTime item4 = valueTuple.Item5;
						array2[num5] = ((item4 != null) ? item4.ToString() : null);
						array2[8] = ",";
						array2[9] = patchedTransfer.DV_mps.ToString();
						log2.WriteLine(string.Concat(array2));
					}
				}
				else
				{
					StreamWriter log3 = param.log;
					if (log3 != null)
					{
						string[] array3 = new string[12];
						array3[0] = "FAIL: patched transfer failed,";
						array3[1] = transferType;
						array3[2] = "Torch-";
						array3[3] = (alignment.Item2 * 57.29577951308232).ToString();
						array3[4] = "°,";
						int num6 = 5;
						TIDateTime item5 = valueTuple.Item4;
						array3[num6] = ((item5 != null) ? item5.ToString() : null);
						array3[6] = ",";
						int num7 = 7;
						TIDateTime item6 = valueTuple.Item5;
						array3[num7] = ((item6 != null) ? item6.ToString() : null);
						array3[8] = ",";
						array3[9] = patchedTransfer.DV_mps.ToString();
						array3[10] = ",";
						int num8 = 11;
						TransferResult transferResult3 = transferResult;
						array3[num8] = ((transferResult3 != null) ? transferResult3.ToString() : null);
						log3.WriteLine(string.Concat(array3));
					}
				}
			}
			else
			{
				StreamWriter log4 = param.log;
				if (log4 != null)
				{
					string[] array4 = new string[10];
					array4[0] = "SUCCESS,";
					array4[1] = transferType;
					array4[2] = "Lambert-";
					array4[3] = (alignment.Item2 * 57.29577951308232).ToString();
					array4[4] = "°,";
					int num9 = 5;
					TIDateTime item7 = valueTuple.Item1;
					array4[num9] = ((item7 != null) ? item7.ToString() : null);
					array4[6] = ",";
					int num10 = 7;
					TIDateTime item8 = valueTuple.Item2;
					array4[num10] = ((item8 != null) ? item8.ToString() : null);
					array4[8] = ",";
					array4[9] = patchedTransfer.DV_mps.ToString();
					log4.WriteLine(string.Concat(array4));
				}
			}
			if (transferResult.Result == TransferResult.Outcome.Success)
			{
				alignment.Item1 = patchedTransfer;
			}
		}

		// Token: 0x06003E07 RID: 15879 RVA: 0x0018DEF0 File Offset: 0x0018C0F0
		[return: TupleElementNames(new string[] { "trajectory", "bestDV_mps" })]
		private static ValueTuple<Trajectory_Patched, double> OptimizeArrivalMeanAnomaly(TIDateTime launchTime, TIDateTime arrivalTime, MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param, string transferType, [TupleElementNames(new string[] { "lambert", "torch" })] Func<TIDateTime, TIDateTime, double, ValueTuple<TIDateTime, TIDateTime>> calcLaunchTime)
		{
			if (!(param.destinationValue is TIOrbitState))
			{
				Log.Error("OptimizeArrivalMeanAnomaly() was called with a non-orbit destination.  The destination orbital information is likely to be incorrect.", Array.Empty<object>());
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState = param.originValue.barycenter();
			TISpaceFleetState tispaceFleetState = param.originValue as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned)
			{
				tinaturalSpaceObjectState = tispaceFleetState.trajectory.GetBarycenterAtTime(launchTime);
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState2 = param.destinationValue.barycenter();
			TISpaceFleetState tispaceFleetState2 = param.destinationValue as TISpaceFleetState;
			if (tispaceFleetState2 != null && tispaceFleetState2.transferAssigned && tispaceFleetState2.trajectory.launchTime < arrivalTime)
			{
				tinaturalSpaceObjectState2 = tispaceFleetState2.trajectory.GetBarycenterAtTime(arrivalTime);
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState3 = tinaturalSpaceObjectState.FindCommonBarycenter(tinaturalSpaceObjectState2);
			double num = param.originValue.common_M_rad(tinaturalSpaceObjectState3, launchTime) + 3.141592653589793 + param.destinationValue.common_Ω_rad(tinaturalSpaceObjectState3) - param.originValue.common_Ω_rad(tinaturalSpaceObjectState3) + param.destinationValue.common_ω_rad(tinaturalSpaceObjectState3) - param.originValue.common_ω_rad(tinaturalSpaceObjectState3);
			ValueTuple<PatchedTransfer, double> valueTuple = new ValueTuple<PatchedTransfer, double>(null, 0.0);
			ValueTuple<PatchedTransfer, double> valueTuple2 = new ValueTuple<PatchedTransfer, double>(null, num);
			MasterTransferPlanner.TestArrivalMeanAnomaly(ref valueTuple2, launchTime, arrivalTime, param, transferType, calcLaunchTime);
			bool flag = false;
			if (param.stopOnFirstSuccess && valueTuple2.Item1 != null && valueTuple2.Item1.DV_mps <= param.fleetDeltaV_mps)
			{
				flag = true;
				valueTuple = valueTuple2;
			}
			ValueTuple<PatchedTransfer, double> valueTuple3 = new ValueTuple<PatchedTransfer, double>(null, valueTuple2.Item2 + 2.0943951023931953);
			if (!flag)
			{
				MasterTransferPlanner.TestArrivalMeanAnomaly(ref valueTuple3, launchTime, arrivalTime, param, transferType, calcLaunchTime);
				if (param.stopOnFirstSuccess && valueTuple3.Item1 != null && valueTuple3.Item1.DV_mps <= param.fleetDeltaV_mps)
				{
					flag = true;
					valueTuple = valueTuple3;
				}
			}
			ValueTuple<PatchedTransfer, double> valueTuple4 = new ValueTuple<PatchedTransfer, double>(null, valueTuple3.Item2 + 2.0943951023931953);
			if (!flag)
			{
				MasterTransferPlanner.TestArrivalMeanAnomaly(ref valueTuple4, launchTime, arrivalTime, param, transferType, calcLaunchTime);
				if (param.stopOnFirstSuccess && valueTuple4.Item1 != null && valueTuple4.Item1.DV_mps <= param.fleetDeltaV_mps)
				{
					flag = true;
					valueTuple = valueTuple4;
				}
			}
			if (!flag)
			{
				ValueTuple<PatchedTransfer, double> valueTuple5 = new ValueTuple<PatchedTransfer, double>(null, 0.0);
				if (valueTuple2.Item1 != null)
				{
					valueTuple = valueTuple2;
				}
				if (valueTuple3.Item1 != null)
				{
					if (valueTuple.Item1 != null && valueTuple.Item1.DV_mps < valueTuple3.Item1.DV_mps)
					{
						valueTuple5 = valueTuple3;
					}
					else
					{
						valueTuple5 = valueTuple;
						valueTuple = valueTuple3;
					}
				}
				if (valueTuple4.Item1 != null)
				{
					if (valueTuple.Item1 != null && valueTuple.Item1.DV_mps < valueTuple4.Item1.DV_mps)
					{
						if (valueTuple5.Item1 == null || valueTuple5.Item1.DV_mps > valueTuple4.Item1.DV_mps)
						{
							valueTuple5 = valueTuple4;
						}
					}
					else
					{
						valueTuple5 = valueTuple;
						valueTuple = valueTuple4;
					}
				}
				ValueTuple<PatchedTransfer, double> valueTuple6 = valueTuple;
				PatchedTransfer patchedTransfer = valueTuple6.Item1;
				double num2 = valueTuple6.Item2;
				ValueTuple<PatchedTransfer, double> valueTuple7 = valueTuple4;
				PatchedTransfer patchedTransfer2 = valueTuple7.Item1;
				double num3 = valueTuple7.Item2;
				if (patchedTransfer == patchedTransfer2 && num2 == num3)
				{
					ValueTuple<PatchedTransfer, double> valueTuple8 = valueTuple5;
					patchedTransfer2 = valueTuple8.Item1;
					num3 = valueTuple8.Item2;
					ValueTuple<PatchedTransfer, double> valueTuple9 = valueTuple2;
					patchedTransfer = valueTuple9.Item1;
					num2 = valueTuple9.Item2;
					if (patchedTransfer2 == patchedTransfer && num3 == num2)
					{
						valueTuple5.Item2 += 6.283185307179586;
						goto IL_038B;
					}
				}
				ValueTuple<PatchedTransfer, double> valueTuple10 = valueTuple;
				patchedTransfer = valueTuple10.Item1;
				num2 = valueTuple10.Item2;
				ValueTuple<PatchedTransfer, double> valueTuple11 = valueTuple2;
				patchedTransfer2 = valueTuple11.Item1;
				num3 = valueTuple11.Item2;
				if (patchedTransfer == patchedTransfer2 && num2 == num3)
				{
					ValueTuple<PatchedTransfer, double> valueTuple12 = valueTuple;
					patchedTransfer2 = valueTuple12.Item1;
					num3 = valueTuple12.Item2;
					ValueTuple<PatchedTransfer, double> valueTuple13 = valueTuple4;
					patchedTransfer = valueTuple13.Item1;
					num2 = valueTuple13.Item2;
					if (patchedTransfer2 == patchedTransfer && num3 == num2)
					{
						valueTuple5.Item2 -= 6.283185307179586;
					}
				}
				IL_038B:
				if (valueTuple.Item1 == null)
				{
					double positiveInfinity = double.PositiveInfinity;
					return new ValueTuple<Trajectory_Patched, double>(null, positiveInfinity);
				}
				int i = (param.fleet.faction.isActivePlayer ? 5 : 5);
				double num4 = 1.0471975511965976;
				while (i > 0 && valueTuple5.Item1 == null)
				{
					i--;
					ValueTuple<PatchedTransfer, double> valueTuple14 = new ValueTuple<PatchedTransfer, double>(null, valueTuple.Item2 + num4);
					MasterTransferPlanner.TestArrivalMeanAnomaly(ref valueTuple14, launchTime, arrivalTime, param, transferType, calcLaunchTime);
					if (param.stopOnFirstSuccess && valueTuple14.Item1 != null && valueTuple14.Item1.DV_mps <= param.fleetDeltaV_mps)
					{
						flag = true;
						valueTuple = valueTuple14;
						break;
					}
					if (valueTuple14.Item1 != null && (valueTuple5.Item1 == null || valueTuple14.Item1.DV_mps < valueTuple5.Item1.DV_mps))
					{
						valueTuple5 = valueTuple14;
					}
					ValueTuple<PatchedTransfer, double> valueTuple15 = new ValueTuple<PatchedTransfer, double>(null, valueTuple.Item2 - num4);
					MasterTransferPlanner.TestArrivalMeanAnomaly(ref valueTuple15, launchTime, arrivalTime, param, transferType, calcLaunchTime);
					if (param.stopOnFirstSuccess && valueTuple15.Item1 != null && valueTuple15.Item1.DV_mps <= param.fleetDeltaV_mps)
					{
						flag = true;
						valueTuple = valueTuple15;
						break;
					}
					if (valueTuple15.Item1 != null && (valueTuple5.Item1 == null || valueTuple15.Item1.DV_mps < valueTuple5.Item1.DV_mps))
					{
						valueTuple5 = valueTuple15;
					}
					if (valueTuple5.Item1 != null && valueTuple5.Item1.DV_mps < valueTuple.Item1.DV_mps)
					{
						ValueTuple<PatchedTransfer, double> valueTuple16 = valueTuple;
						valueTuple = valueTuple5;
						valueTuple5 = valueTuple16;
					}
					num4 /= 2.0;
				}
				if (!flag)
				{
					while (i > 0)
					{
						i--;
						ValueTuple<PatchedTransfer, double> valueTuple17 = new ValueTuple<PatchedTransfer, double>(null, (valueTuple.Item2 + valueTuple5.Item2) / 2.0);
						MasterTransferPlanner.TestArrivalMeanAnomaly(ref valueTuple17, launchTime, arrivalTime, param, transferType, calcLaunchTime);
						if (param.stopOnFirstSuccess && valueTuple17.Item1 != null && valueTuple17.Item1.DV_mps <= param.fleetDeltaV_mps)
						{
							valueTuple = valueTuple17;
							break;
						}
						if (valueTuple17.Item1 != null)
						{
							if (valueTuple17.Item1.DV_mps < valueTuple.Item1.DV_mps)
							{
								valueTuple5 = valueTuple;
								valueTuple = valueTuple17;
							}
							else if (valueTuple5.Item1 == null || valueTuple17.Item1.DV_mps < valueTuple5.Item1.DV_mps)
							{
								valueTuple5 = valueTuple17;
							}
						}
						num4 /= 2.0;
					}
				}
			}
			Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
			trajectory_Patched.BuildSingleTrajectory(param.fleet, param.sDestination, param.originValue, param.destinationValue, param.commonBarycenter, valueTuple.Item1, param.fleetAcceleration_mps2);
			StreamWriter log = param.log;
			if (log != null)
			{
				string[] array = new string[18];
				array[0] = "SUCCESS,";
				array[1] = transferType;
				array[2] = "-";
				array[3] = (valueTuple.Item2 * 57.29577951308232).ToString();
				array[4] = "°,";
				int num5 = 5;
				TIDateTime launchTime2 = valueTuple.Item1.launchTime;
				array[num5] = ((launchTime2 != null) ? launchTime2.ToString() : null);
				array[6] = ",";
				int num6 = 7;
				TIDateTime arrivalTime2 = valueTuple.Item1.arrivalTime;
				array[num6] = ((arrivalTime2 != null) ? arrivalTime2.ToString() : null);
				array[8] = ",";
				array[9] = valueTuple.Item1.DV_mps.ToString();
				array[10] = ",";
				int num7 = 11;
				TIDateTime launchTime3 = trajectory_Patched.launchTime;
				array[num7] = ((launchTime3 != null) ? launchTime3.ToString() : null);
				array[12] = ",";
				int num8 = 13;
				TIDateTime arrivalTime3 = trajectory_Patched.arrivalTime;
				array[num8] = ((arrivalTime3 != null) ? arrivalTime3.ToString() : null);
				array[14] = ",";
				array[15] = trajectory_Patched.DV_mps.ToString();
				array[16] = ",";
				array[17] = trajectory_Patched.DumpSegments();
				log.WriteLine(string.Concat(array));
			}
			return new ValueTuple<Trajectory_Patched, double>(trajectory_Patched, valueTuple.Item1.DV_mps);
		}

		// Token: 0x06003E08 RID: 15880 RVA: 0x0018E667 File Offset: 0x0018C867
		private static double TransferDurationHardCap(TIFactionState faction)
		{
			if (faction == null || !faction.IsAlienFaction)
			{
				return 78892310.0;
			}
			return 78892310.0;
		}

		// Token: 0x06003E09 RID: 15881 RVA: 0x0018E688 File Offset: 0x0018C888
		[return: TupleElementNames(new string[] { "sourceBarycenter", "destinationBarycenter", "commonBarycenter" })]
		private static ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState> GetRelevantBarycentersAtTime(ITransferTarget source, ITransferTarget destination, TIDateTime launchTime, TIDateTime arrivalTime)
		{
			TISpaceFleetState tispaceFleetState = source as TISpaceFleetState;
			TINaturalSpaceObjectState item = Trajectory.GetDestinationLocalOrbitalElementsAtTime(destination, (tispaceFleetState != null) ? tispaceFleetState.faction : null, arrivalTime, null, 0.0).Item2;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime < launchTime)
			{
				tinaturalSpaceObjectState = tispaceFleetState.trajectory.GetBarycenterAtTime(launchTime);
			}
			else
			{
				tinaturalSpaceObjectState = source.barycenter();
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState2 = tinaturalSpaceObjectState.FindCommonBarycenter(item);
			return new ValueTuple<TINaturalSpaceObjectState, TINaturalSpaceObjectState, TINaturalSpaceObjectState>(tinaturalSpaceObjectState, item, tinaturalSpaceObjectState2);
		}

		// Token: 0x06003E0A RID: 15882 RVA: 0x0018E70C File Offset: 0x0018C90C
		public static double NormalizeAngleNearZero_Rad(double angle)
		{
			angle %= 6.283185307179586;
			if (angle > 3.141592653589793)
			{
				angle -= 6.283185307179586;
			}
			if (angle < -3.141592653589793)
			{
				angle += 6.283185307179586;
			}
			return angle;
		}

		// Token: 0x06003E0B RID: 15883 RVA: 0x0018E759 File Offset: 0x0018C959
		public static double NormalizeAngleNearPi_Rad(double angle)
		{
			angle %= 6.283185307179586;
			if (angle < 0.0)
			{
				angle += 6.283185307179586;
			}
			return angle;
		}

		// Token: 0x06003E0C RID: 15884 RVA: 0x0018E782 File Offset: 0x0018C982
		private static double MeanAnomalyToSeconds(double meanAnomaly_Rad, double orbitalPeriod_s)
		{
			return meanAnomaly_Rad * orbitalPeriod_s / 6.283185307179586;
		}

		// Token: 0x06003E0D RID: 15885 RVA: 0x0018E794 File Offset: 0x0018C994
		private static double FindMeanAnomalyWhereOrbitVelocityMatchesThrustDirection(OrbitalElementsState orbit, Vector3d thrustVector, TINaturalSpaceObjectState barycenter, TIDateTime time, bool isPlayer)
		{
			Vector3d normalVector = orbit.normalVector;
			Vector3d vector3d = Vector3d.Cross(thrustVector, normalVector).normalized * barycenter.hillRadius_m;
			return TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbit, barycenter, vector3d, time, isPlayer);
		}

		// Token: 0x06003E0E RID: 15886 RVA: 0x0018E7D0 File Offset: 0x0018C9D0
		[return: TupleElementNames(new string[] { "initialIdealLocalMeanAnomaly", "finalIdealLocalMeanAnomaly" })]
		private static ValueTuple<double, double> EstimateIdealLocalMeanAnomalies(TIDateTime launchTime, TIDateTime arrivalTime, double commonMeanAnomalyAtArrival_Rad, MasterTransferPlanner.CalculateImpulseMicrothrustHybridTransfer_Params param)
		{
			OrbitalElementsState orbitalElementsState = new OrbitalElementsState(param.destinationValue, commonMeanAnomalyAtArrival_Rad, arrivalTime);
			LambertEquations lambertEquations = default(LambertEquations);
			double num = lambertEquations.SolveLambert(arrivalTime.DifferenceInDays(launchTime), param.originValue.relevantGlobalCartesianState(param.commonBarycenter, launchTime), orbitalElementsState.ToCartesianStateAtTime(arrivalTime.ExportTime(), param.commonBarycenter.mass_kg), param.commonBarycenter.mu, false, true);
			LambertEquations lambertEquations2 = default(LambertEquations);
			double num2 = lambertEquations2.SolveLambert(arrivalTime.DifferenceInDays(launchTime), param.originValue.relevantGlobalCartesianState(param.commonBarycenter, launchTime), orbitalElementsState.ToCartesianStateAtTime(arrivalTime.ExportTime(), param.commonBarycenter.mass_kg), param.commonBarycenter.mu, true, true);
			LambertEquations lambertEquations3 = ((num >= num2) ? lambertEquations : lambertEquations2);
			bool isActivePlayer = param.fleet.faction.isActivePlayer;
			double num3 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(lambertEquations3.burn0, param.originValue, launchTime, isActivePlayer);
			double num4 = MasterTransferPlanner.EstimateMeanAnomalyWhenMotionIsClosestToGivenVelocity(lambertEquations3.burn1, param.destinationValue, arrivalTime, isActivePlayer);
			return new ValueTuple<double, double>(num3, num4);
		}

		// Token: 0x06003E0F RID: 15887 RVA: 0x0018E8D8 File Offset: 0x0018CAD8
		private static void UpdateMicrothrustStatisticsForBarycenter(out double microthrustDuration_s, out double microthrustAnomalyDelta_Rad, out double microthrustRadius_m, TINaturalSpaceObjectState barycenter, double relevantSemiMajorAxis_m, double fleetAccleration_mps2)
		{
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAccleration_mps2, barycenter.mu, barycenter.sphereOfInfluence_m);
			double num = Mathd.Sqrt(barycenter.mu / relevantSemiMajorAxis_m);
			microthrustDuration_s = microthrustSphere.GetDuration_s(num);
			microthrustAnomalyDelta_Rad = microthrustSphere.GetAnomalyDelta_Rad(num);
			microthrustRadius_m = microthrustSphere.Radius_m;
		}

		// Token: 0x06003E10 RID: 15888 RVA: 0x0018E922 File Offset: 0x0018CB22
		private static MasterTransferPlanner.IdentifyHybridTransferType_Result IdentifyHybridTransferType(MasterTransferPlanner.SimplifiedPositions simplified, double fleetAcceleration_mps2)
		{
			return MasterTransferPlanner.IdentifyHybridTransferType(simplified.originDistToLocalBarycenter_m, simplified.originLocalBarycenter, simplified.destinationDistToCommonBarycenter_m, simplified.destinationLocalBarycenter, simplified.commonBarycenter, fleetAcceleration_mps2);
		}

		// Token: 0x06003E11 RID: 15889 RVA: 0x0018E948 File Offset: 0x0018CB48
		private static MasterTransferPlanner.IdentifyHybridTransferType_Result IdentifyHybridTransferType(double startSemiMajorAxis_m, TINaturalSpaceObjectState startBarycenter, double endSemiMajorAxis_m, TINaturalSpaceObjectState endBarycenter, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2)
		{
			MasterTransferPlanner.IdentifyHybridTransferType_Result identifyHybridTransferType_Result = new MasterTransferPlanner.IdentifyHybridTransferType_Result();
			double num;
			if (startBarycenter == commonBarycenter)
			{
				num = ((startSemiMajorAxis_m >= 0.0) ? startSemiMajorAxis_m : double.PositiveInfinity);
			}
			else if (startBarycenter.barycenter == commonBarycenter)
			{
				num = startBarycenter.semiMajorAxis_m;
			}
			else
			{
				num = startBarycenter.barycenter.semiMajorAxis_m;
			}
			double num2;
			if (endBarycenter == commonBarycenter)
			{
				num2 = ((endSemiMajorAxis_m >= 0.0) ? endSemiMajorAxis_m : double.PositiveInfinity);
			}
			else if (endBarycenter.barycenter == commonBarycenter)
			{
				num2 = endBarycenter.semiMajorAxis_m;
			}
			else
			{
				num2 = endBarycenter.barycenter.semiMajorAxis_m;
			}
			identifyHybridTransferType_Result.isGoingOut = num < num2;
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, commonBarycenter.mu, commonBarycenter.sphereOfInfluence_m);
			identifyHybridTransferType_Result.commonMicrothrustRadius_m = microthrustSphere.Radius_m;
			identifyHybridTransferType_Result.isMicrothrustOnly = num < microthrustSphere.Radius_m && num2 < microthrustSphere.Radius_m;
			identifyHybridTransferType_Result.outspiralDuration_s = 0.0;
			identifyHybridTransferType_Result.inspiralDuration_s = 0.0;
			double num3 = Mathd.Sqrt(commonBarycenter.mu / num);
			double num4 = Mathd.Sqrt(commonBarycenter.mu / num2);
			if (identifyHybridTransferType_Result.isMicrothrustOnly)
			{
				if (identifyHybridTransferType_Result.isGoingOut)
				{
					identifyHybridTransferType_Result.outspiralDuration_s = microthrustSphere.GetDuration_s(num3) - microthrustSphere.GetDuration_s(num4);
				}
				else
				{
					identifyHybridTransferType_Result.inspiralDuration_s = microthrustSphere.GetDuration_s(num4) - microthrustSphere.GetDuration_s(num3);
				}
			}
			else if (num < microthrustSphere.Radius_m)
			{
				identifyHybridTransferType_Result.outspiralDuration_s = microthrustSphere.GetDuration_s(num3);
			}
			else if (num2 < microthrustSphere.Radius_m)
			{
				identifyHybridTransferType_Result.inspiralDuration_s = microthrustSphere.GetDuration_s(num4);
			}
			if (startBarycenter != commonBarycenter)
			{
				MicrothrustSphere microthrustSphere2 = new MicrothrustSphere(fleetAcceleration_mps2, startBarycenter.mu, startBarycenter.sphereOfInfluence_m);
				if (startSemiMajorAxis_m < microthrustSphere2.Radius_m)
				{
					double num5 = Mathd.Sqrt(startBarycenter.mu / startSemiMajorAxis_m);
					identifyHybridTransferType_Result.outspiralDuration_s += microthrustSphere2.GetDuration_s(num5);
				}
				if (startBarycenter.barycenter != commonBarycenter)
				{
					double semiMajorAxis_m = startBarycenter.semiMajorAxis_m;
					MicrothrustSphere microthrustSphere3 = new MicrothrustSphere(fleetAcceleration_mps2, startBarycenter.barycenter.mu, startBarycenter.barycenter.sphereOfInfluence_m);
					if (semiMajorAxis_m < microthrustSphere3.Radius_m)
					{
						double num6 = Mathd.Sqrt(startBarycenter.barycenter.mu / semiMajorAxis_m);
						identifyHybridTransferType_Result.outspiralDuration_s += microthrustSphere3.GetDuration_s(num6);
					}
				}
			}
			if (endBarycenter != commonBarycenter)
			{
				MicrothrustSphere microthrustSphere4 = new MicrothrustSphere(fleetAcceleration_mps2, endBarycenter.mu, endBarycenter.sphereOfInfluence_m);
				if (endSemiMajorAxis_m < microthrustSphere4.Radius_m)
				{
					double num7 = Mathd.Sqrt(endBarycenter.mu / endSemiMajorAxis_m);
					identifyHybridTransferType_Result.inspiralDuration_s += microthrustSphere4.GetDuration_s(num7);
				}
				if (endBarycenter.barycenter != commonBarycenter)
				{
					double semiMajorAxis_m2 = endBarycenter.semiMajorAxis_m;
					MicrothrustSphere microthrustSphere5 = new MicrothrustSphere(fleetAcceleration_mps2, endBarycenter.barycenter.mu, endBarycenter.barycenter.sphereOfInfluence_m);
					if (semiMajorAxis_m2 < microthrustSphere5.Radius_m)
					{
						double num8 = Mathd.Sqrt(endBarycenter.barycenter.mu / semiMajorAxis_m2);
						identifyHybridTransferType_Result.inspiralDuration_s += microthrustSphere5.GetDuration_s(num8);
					}
				}
			}
			return identifyHybridTransferType_Result;
		}

		// Token: 0x06003E12 RID: 15890 RVA: 0x0018EC60 File Offset: 0x0018CE60
		private static TransferResult CalculateMicrothrustTransfer(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, IMobileAsset fleet, double fleetDeltaV_mps, double fleetAcceleration_mps2, ITransferTarget originValue, TISpaceGameState sDestination, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TIDateTime now)
		{
			MicrothrustTransfer microthrustTransfer = new MicrothrustTransfer();
			TISpaceFleetState tispaceFleetState = sDestination as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.trajectory != null && MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, fleet.faction) && tispaceFleetState.trajectory.destination.barycenter == fleet.barycenter() && tispaceFleetState.trajectory.destination.ref_orbit != null)
			{
				TIDateTime tidateTime = new TIDateTime(tispaceFleetState.trajectory.arrivalTime, 1.0);
				microthrustTransfer.Solve(now, originValue, tispaceFleetState, fleet.barycenter(), fleetAcceleration_mps2, tidateTime);
				commonBarycenter = fleet.barycenter();
			}
			else
			{
				microthrustTransfer.Solve(now, originValue, destinationValue, commonBarycenter, fleetAcceleration_mps2, TITimeState.Now());
			}
			lowestDVFound_kps = Mathd.Min(microthrustTransfer.DV_mps / 1000.0, lowestDVFound_kps);
			if (microthrustTransfer.DV_mps > fleetDeltaV_mps)
			{
				return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, microthrustTransfer.DV_mps, 0.0);
			}
			Trajectory_Microthrust trajectory_Microthrust = new Trajectory_Microthrust();
			trajectory_Microthrust.BuildSingleTrajectory(fleet, sDestination, originValue, destinationValue, commonBarycenter, microthrustTransfer, fleetAcceleration_mps2);
			if (trajectory_Microthrust.DV_mps <= fleetDeltaV_mps)
			{
				candidateTrajectories.Add(trajectory_Microthrust);
				return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
			}
			return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, trajectory_Microthrust.DV_mps, 0.0);
		}

		// Token: 0x06003E13 RID: 15891 RVA: 0x0018EDA8 File Offset: 0x0018CFA8
		private static void CalculateImpulseTransfers(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, int requestSize, IMobileAsset fleet, double fleetDeltaV_mps, double fleetAcceleration_mps2, ITransferTarget originValue, TISpaceGameState sDestination, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, double minAllowedDuration_s, bool useCap, TIDateTime capDate, bool orbitWalking, bool stopOnFirstSuccess = false)
		{
			double num;
			double num2;
			if (commonBarycenter.isSun)
			{
				num = 864000.0;
				num2 = 604800.0;
			}
			else
			{
				num = 43200.0;
				num2 = (orbitWalking ? (86400.0 * Mathd.Min(originValue.period_days(), 80.0) / 6.0) : 21600.0);
			}
			TIDateTime tidateTime = new TIDateTime();
			List<Trajectory_Impulse> list = new List<Trajectory_Impulse>();
			TwoBurnLambertTransfer twoBurnLambertTransfer = new TwoBurnLambertTransfer();
			double num3 = MasterTransferPlanner.HohmannFirstBurnDuration_s(fleet, originValue, destinationValue, commonBarycenter) * 0.6;
			bool flag = sDestination is TIOrbitState && fleet.barycenter() == sDestination.barycenter;
			for (int i = 0; i < ((orbitWalking || flag) ? 1 : (requestSize / 2)); i++)
			{
				if (i != -1)
				{
					if (i != 0)
					{
						tidateTime.AddSeconds(num);
					}
					else
					{
						tidateTime = TITimeState.Now();
						tidateTime.AddSeconds(num3);
					}
				}
				else
				{
					double num4;
					tidateTime = TINaturalSpaceObjectState.GetNextHohmannLaunchWindowDate(fleet.faction, fleet.barycenter(), sDestination.ref_naturalSpaceObject, TITimeState.Now(), out num4);
					tidateTime.AddSeconds(num3);
				}
				for (int j = 0; j < (orbitWalking ? requestSize : (requestSize / 2)); j++)
				{
					for (int k = 0; k < (flag ? (requestSize / 2) : 1); k++)
					{
						TIDateTime tidateTime2 = new TIDateTime(tidateTime);
						double num5 = minAllowedDuration_s + (double)j * num2;
						tidateTime2.AddSeconds(num5);
						if (useCap && tidateTime2 >= capDate)
						{
							break;
						}
						TransferResult transferResult;
						if (flag)
						{
							double num6 = fleet.Ω_rad() + fleet.ω_rad() + fleet.meanAnomaly_Rad(tidateTime) - destinationValue.ω_rad() - destinationValue.Ω_rad();
							double num7 = num6 + 3.141592653589793;
							double num8 = num7 - (double)k * (num7 - num6) / (double)(requestSize / 2);
							transferResult = twoBurnLambertTransfer.Solve(tidateTime, tidateTime2, num5, originValue, sDestination as TIOrbitState, num8, commonBarycenter, (double)fleet.cruiseAcceleration_mps2);
						}
						else
						{
							transferResult = twoBurnLambertTransfer.Solve(tidateTime, tidateTime2, num5, originValue, destinationValue, commonBarycenter, (double)fleet.cruiseAcceleration_mps2);
						}
						if (transferResult.Result == TransferResult.Outcome.Success && !(twoBurnLambertTransfer.launchTime < TITimeState.Now()))
						{
							TimeSpan timeSpan = twoBurnLambertTransfer.arrivalTime - twoBurnLambertTransfer.launchTime;
							if (!(new TimeSpan(0, 0, Mathd.CeilToInt((twoBurnLambertTransfer.boost_DV_mps + twoBurnLambertTransfer.decel_DV_mps) / (double)fleet.cruiseAcceleration_mps2)) >= timeSpan))
							{
								lowestDVFound_kps = Mathd.Min(twoBurnLambertTransfer.DV_mps / 1000.0, lowestDVFound_kps);
								if ((double)fleet.currentDeltaV_mps >= twoBurnLambertTransfer.DV_mps && twoBurnLambertTransfer.transferOrbit.eccentricity < 1.0)
								{
									bool flag2 = false;
									for (int l = 0; l < list.Count; l++)
									{
										Trajectory_Impulse trajectory_Impulse = list[l];
										if (trajectory_Impulse.DV_mps < twoBurnLambertTransfer.DV_mps && trajectory_Impulse.arrivalTime <= tidateTime2)
										{
											flag2 = true;
											break;
										}
									}
									if (!flag2)
									{
										if (twoBurnLambertTransfer.transferOrbit.semiMajorAxis_m * (1.0 - twoBurnLambertTransfer.transferOrbit.eccentricity) <= commonBarycenter.meanRadius_m)
										{
											TIDateTime tidateTime3 = new TIDateTime(twoBurnLambertTransfer.transferOrbit.epoch);
											double num9 = twoBurnLambertTransfer.launchTime.DifferenceInSeconds(tidateTime3);
											double num10 = 6.283185307179586 * Mathd.Sqrt(Mathd.Pow(twoBurnLambertTransfer.transferOrbit.semiMajorAxis_m, 3.0) / commonBarycenter.mu);
											double num11 = twoBurnLambertTransfer.transferOrbit.meanAnomalyAtEpoch_Rad * num10 / 6.283185307179586;
											double num12 = (num9 + num11) % num10;
											if (num12 < 0.0)
											{
												num12 += num10;
											}
											double num13 = num10 - num12;
											TIDateTime tidateTime4 = new TIDateTime(twoBurnLambertTransfer.launchTime);
											tidateTime4.AddSeconds(num13);
											if (twoBurnLambertTransfer.arrivalTime > tidateTime4)
											{
												goto IL_03DC;
											}
										}
										Trajectory_Impulse trajectory_Impulse2 = new Trajectory_Impulse();
										trajectory_Impulse2.BuildSingleTrajectory(fleet, sDestination, originValue, destinationValue, commonBarycenter, twoBurnLambertTransfer, fleetAcceleration_mps2);
										list.Add(trajectory_Impulse2);
										if (stopOnFirstSuccess)
										{
											candidateTrajectories.AddRange(list);
											return;
										}
									}
								}
							}
						}
						IL_03DC:;
					}
				}
			}
			candidateTrajectories.AddRange(list);
		}

		// Token: 0x06003E14 RID: 15892 RVA: 0x0018F1E4 File Offset: 0x0018D3E4
		private static void CalculateTorchTransfers(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, int requestSize, IMobileAsset fleet, double fleetDeltaV_mps, double fleetAcceleration_mps2, ITransferTarget originValue, TISpaceGameState sDestination, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, double minAllowedDuration_s, bool useCap, TIDateTime capDate, bool stopOnFirstSuccess = false)
		{
			TorchTransfer torchTransfer = new TorchTransfer();
			List<Trajectory_Torch> list = new List<Trajectory_Torch>();
			TIDateTime tidateTime = TITimeState.Now();
			int num = requestSize;
			double num2;
			if (commonBarycenter.isSun)
			{
				if (Mathd.Max(originValue.common_a_m(commonBarycenter), destinationValue.common_a_m(commonBarycenter)) / 149597870700.0 > 5.0)
				{
					num *= 2;
				}
				num2 = 604800.0;
			}
			else
			{
				double num3 = Mathd.Max(originValue.common_a_m(commonBarycenter), destinationValue.common_a_m(commonBarycenter)) / 1000.0;
				if (num3 < 100000.0)
				{
					num2 = 10800.0;
				}
				else if (num3 < 1000000.0)
				{
					num2 = 21600.0;
				}
				else if (num3 < 10000000.0)
				{
					num2 = 54000.0;
				}
				else
				{
					num2 = 86400.0;
					num *= 2;
				}
			}
			for (int i = 0; i < num; i++)
			{
				double num4 = minAllowedDuration_s + (double)i * num2;
				TIDateTime tidateTime2 = TITimeState.Now();
				tidateTime2.AddSeconds(num4);
				if (useCap && tidateTime2 >= capDate)
				{
					break;
				}
				bool flag;
				if (torchTransfer.Solve(tidateTime, num4, fleetAcceleration_mps2, originValue, destinationValue, commonBarycenter, fleetDeltaV_mps, out flag).Result == TransferResult.Outcome.Success)
				{
					lowestDVFound_kps = Mathd.Min(torchTransfer.DV_mps / 1000.0, lowestDVFound_kps);
					if (flag)
					{
						bool flag2 = false;
						for (int j = 0; j < list.Count; j++)
						{
							if (list[j].DV_mps < torchTransfer.DV_mps && list[j].duration_s <= num4)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							Trajectory_Torch trajectory_Torch = new Trajectory_Torch();
							trajectory_Torch.BuildSingleTrajectory(fleet, sDestination, originValue, destinationValue, commonBarycenter, torchTransfer, fleetAcceleration_mps2);
							list.Add(trajectory_Torch);
							if (stopOnFirstSuccess)
							{
								candidateTrajectories.AddRange(list);
								return;
							}
						}
					}
				}
			}
			candidateTrajectories.AddRange(list);
		}

		// Token: 0x06003E15 RID: 15893 RVA: 0x0018F3C8 File Offset: 0x0018D5C8
		private static TransferResult CalculateOrbitPhasingTransfers(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, int requestSize, IMobileAsset fleet, double fleetDeltaV_mps, double fleetAcceleration_mps2, ITransferTarget originValue, TISpaceGameState sDestination, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, bool stopAfterFirstSuccess = false)
		{
			TISpaceFleetState tispaceFleetState = fleet as TISpaceFleetState;
			Trajectory trajectory = ((tispaceFleetState != null) ? tispaceFleetState.trajectory : null);
			Trajectory trajectory2;
			if (!MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destinationValue as TISpaceFleetState, fleet.faction))
			{
				trajectory2 = null;
			}
			else
			{
				TISpaceFleetState tispaceFleetState2 = destinationValue as TISpaceFleetState;
				trajectory2 = ((tispaceFleetState2 != null) ? tispaceFleetState2.trajectory : null);
			}
			Trajectory trajectory3 = trajectory2;
			TITimeState.Now();
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (trajectory == null)
			{
				if (trajectory3 == null)
				{
					tinaturalSpaceObjectState = commonBarycenter;
				}
				else
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState2;
					if (fleet == null)
					{
						tinaturalSpaceObjectState2 = null;
					}
					else
					{
						TIOrbitState ref_orbit = fleet.ref_orbit;
						tinaturalSpaceObjectState2 = ((ref_orbit != null) ? ref_orbit.barycenter : null);
					}
					tinaturalSpaceObjectState = tinaturalSpaceObjectState2;
					if (tinaturalSpaceObjectState != null)
					{
						tinaturalSpaceObjectState = fleet.ref_orbit.barycenter.FindCommonBarycenter(tinaturalSpaceObjectState);
					}
				}
			}
			else if (trajectory3 == null)
			{
				tinaturalSpaceObjectState = trajectory.destination.ref_orbit.barycenter;
				if (tinaturalSpaceObjectState != null)
				{
					TIOrbitState ref_orbit2 = sDestination.ref_orbit;
					TINaturalSpaceObjectState tinaturalSpaceObjectState3;
					if (ref_orbit2 == null)
					{
						tinaturalSpaceObjectState3 = null;
					}
					else
					{
						TINaturalSpaceObjectState barycenter = ref_orbit2.barycenter;
						tinaturalSpaceObjectState3 = ((barycenter != null) ? barycenter.FindCommonBarycenter(tinaturalSpaceObjectState) : null);
					}
					tinaturalSpaceObjectState = tinaturalSpaceObjectState3;
				}
			}
			else
			{
				tinaturalSpaceObjectState = trajectory.destination.ref_orbit.barycenter;
				if (tinaturalSpaceObjectState != null)
				{
					TISpaceGameState destination = trajectory3.destination;
					TINaturalSpaceObjectState tinaturalSpaceObjectState4;
					if (destination == null)
					{
						tinaturalSpaceObjectState4 = null;
					}
					else
					{
						TIOrbitState ref_orbit3 = destination.ref_orbit;
						if (ref_orbit3 == null)
						{
							tinaturalSpaceObjectState4 = null;
						}
						else
						{
							TINaturalSpaceObjectState barycenter2 = ref_orbit3.barycenter;
							tinaturalSpaceObjectState4 = ((barycenter2 != null) ? barycenter2.FindCommonBarycenter(tinaturalSpaceObjectState) : null);
						}
					}
					tinaturalSpaceObjectState = tinaturalSpaceObjectState4;
				}
			}
			if (tinaturalSpaceObjectState == null)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			return MasterTransferPlanner.CalculateOrbitPhasingTransfers_(ref candidateTrajectories, ref lowestDVFound_kps, requestSize, fleet, fleetDeltaV_mps, fleetAcceleration_mps2, originValue, sDestination, destinationValue, tinaturalSpaceObjectState, stopAfterFirstSuccess);
		}

		// Token: 0x06003E16 RID: 15894 RVA: 0x0018F528 File Offset: 0x0018D728
		private static MasterTransferPlanner.OrbitPhasingConstraints OrbitPhasing_GetPhasingOrbitsAndTiming(IMobileAsset fleet, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter)
		{
			OrbitalElementsState orbitalElementsState = default(OrbitalElementsState);
			TIDateTime tidateTime = TITimeState.Now();
			double num = (fleet.faction.IsAlienFaction ? 78892310.0 : 78892310.0);
			TIDateTime tidateTime2 = new TIDateTime(TITimeState.Now(), num);
			TINaturalSpaceObjectState tinaturalSpaceObjectState = null;
			TISpaceFleetState tispaceFleetState = fleet as TISpaceFleetState;
			TIDateTime tidateTime3;
			OrbitalElementsState orbitalElementsState2;
			TINaturalSpaceObjectState tinaturalSpaceObjectState2;
			if (tispaceFleetState != null && MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, fleet.faction))
			{
				tidateTime3 = new TIDateTime(tispaceFleetState.trajectory.arrivalTime);
				TIOrbitState destinationOrbit = tispaceFleetState.trajectory.destinationOrbit;
				double destinationMeanAnomalyAtArrival = tispaceFleetState.trajectory.getDestinationMeanAnomalyAtArrival();
				TIDateTime arrivalTime = tispaceFleetState.trajectory.arrivalTime;
				orbitalElementsState2 = new OrbitalElementsState(destinationOrbit, destinationMeanAnomalyAtArrival, arrivalTime);
				tinaturalSpaceObjectState2 = tispaceFleetState.trajectory.destinationOrbit.barycenter;
			}
			else
			{
				tidateTime3 = TITimeState.Now();
				orbitalElementsState2 = fleet.ref_orbit.ToOrbitalElementsState(fleet.epoch_DateTime, fleet.meanAnomaly_Rad(fleet.epoch_DateTime));
				tinaturalSpaceObjectState2 = fleet.barycenter();
			}
			OrbitalElementsState orbitalElementsState3 = orbitalElementsState2;
			if (tinaturalSpaceObjectState2 != commonBarycenter)
			{
				if (((tinaturalSpaceObjectState2 != null) ? tinaturalSpaceObjectState2.barycenter : null) == commonBarycenter)
				{
					orbitalElementsState2 = new OrbitalElementsState(tinaturalSpaceObjectState2);
				}
				else
				{
					TIGameState tigameState;
					if (tinaturalSpaceObjectState2 == null)
					{
						tigameState = null;
					}
					else
					{
						TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState2.barycenter;
						tigameState = ((barycenter != null) ? barycenter.barycenter : null);
					}
					if (!(tigameState == commonBarycenter))
					{
						return new MasterTransferPlanner.OrbitPhasingConstraints();
					}
					orbitalElementsState2 = new OrbitalElementsState(tinaturalSpaceObjectState2.barycenter);
				}
			}
			TISpaceAssetState tispaceAssetState = destinationValue as TISpaceAssetState;
			if (tispaceAssetState == null)
			{
				if (tinaturalSpaceObjectState2 == commonBarycenter)
				{
					return new MasterTransferPlanner.OrbitPhasingConstraints();
				}
				TIOrbitState tiorbitState = destinationValue as TIOrbitState;
				if (tiorbitState == null)
				{
					return new MasterTransferPlanner.OrbitPhasingConstraints();
				}
				orbitalElementsState = tiorbitState.ToOrbitalElementsState(TITimeState.Now(), 0.0);
				tinaturalSpaceObjectState = destinationValue.barycenter();
			}
			else
			{
				TISpaceFleetState tispaceFleetState2 = tispaceAssetState as TISpaceFleetState;
				if (tispaceFleetState2 != null && MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState2, fleet.faction))
				{
					bool flag = false;
					TIOrbitState tiorbitState2 = tispaceFleetState2.trajectory.destination as TIOrbitState;
					if (tiorbitState2 != null)
					{
						tinaturalSpaceObjectState = tispaceFleetState2.trajectory.destinationOrbit.barycenter;
						if (!(tinaturalSpaceObjectState == commonBarycenter) && !(((tinaturalSpaceObjectState != null) ? tinaturalSpaceObjectState.barycenter : null) == commonBarycenter))
						{
							TIGameState tigameState2;
							if (tinaturalSpaceObjectState == null)
							{
								tigameState2 = null;
							}
							else
							{
								TINaturalSpaceObjectState barycenter2 = tinaturalSpaceObjectState.barycenter;
								tigameState2 = ((barycenter2 != null) ? barycenter2.barycenter : null);
							}
							if (!(tigameState2 == commonBarycenter))
							{
								goto IL_029F;
							}
						}
						if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(fleet as TISpaceFleetState, fleet.faction) || Mathd.Approximately(fleet.common_a_m(commonBarycenter), ((ITransferTarget)tiorbitState2).common_a_m(commonBarycenter)))
						{
							tidateTime = tispaceFleetState2.trajectory.arrivalTime;
							TIOrbitState destinationOrbit2 = tispaceFleetState2.trajectory.destinationOrbit;
							double destinationMeanAnomalyAtArrival2 = tispaceFleetState2.trajectory.getDestinationMeanAnomalyAtArrival();
							orbitalElementsState = new OrbitalElementsState(destinationOrbit2, destinationMeanAnomalyAtArrival2, tidateTime);
							flag = true;
						}
					}
					IL_029F:
					if (!flag && tispaceFleetState2.trajectory.launchTime > TITimeState.Now())
					{
						tinaturalSpaceObjectState = tispaceFleetState2.barycenter;
						if (!(tinaturalSpaceObjectState == commonBarycenter) && !(((tinaturalSpaceObjectState != null) ? tinaturalSpaceObjectState.barycenter : null) == commonBarycenter))
						{
							TIGameState tigameState3;
							if (tinaturalSpaceObjectState == null)
							{
								tigameState3 = null;
							}
							else
							{
								TINaturalSpaceObjectState barycenter3 = tinaturalSpaceObjectState.barycenter;
								tigameState3 = ((barycenter3 != null) ? barycenter3.barycenter : null);
							}
							if (!(tigameState3 == commonBarycenter))
							{
								goto IL_0329;
							}
						}
						tidateTime = TITimeState.Now();
						orbitalElementsState = new OrbitalElementsState(tispaceFleetState2);
						tidateTime2 = tispaceFleetState2.trajectory.launchTime;
						flag = true;
					}
					IL_0329:
					if (!flag)
					{
						return new MasterTransferPlanner.OrbitPhasingConstraints();
					}
				}
				else
				{
					tidateTime = TITimeState.Now();
					orbitalElementsState = new OrbitalElementsState(tispaceAssetState);
					tinaturalSpaceObjectState = tispaceAssetState.barycenter;
				}
			}
			OrbitalElementsState orbitalElementsState4 = orbitalElementsState;
			if (tinaturalSpaceObjectState != commonBarycenter)
			{
				if (((tinaturalSpaceObjectState != null) ? tinaturalSpaceObjectState.barycenter : null) == commonBarycenter)
				{
					orbitalElementsState = new OrbitalElementsState(tinaturalSpaceObjectState);
				}
				else
				{
					TIGameState tigameState4;
					if (tinaturalSpaceObjectState == null)
					{
						tigameState4 = null;
					}
					else
					{
						TINaturalSpaceObjectState barycenter4 = tinaturalSpaceObjectState.barycenter;
						tigameState4 = ((barycenter4 != null) ? barycenter4.barycenter : null);
					}
					if (!(tigameState4 == commonBarycenter))
					{
						return new MasterTransferPlanner.OrbitPhasingConstraints();
					}
					orbitalElementsState = new OrbitalElementsState(tinaturalSpaceObjectState.barycenter);
				}
			}
			return new MasterTransferPlanner.OrbitPhasingConstraints
			{
				succeeded = true,
				ourOrbit = orbitalElementsState2,
				destOrbit = orbitalElementsState,
				earliestLaunchTime = tidateTime3,
				earliestArrivalTime = tidateTime,
				latestArrivalTime = tidateTime2,
				ourInitialOrbit = orbitalElementsState3,
				ourInitialBarycenter = tinaturalSpaceObjectState2,
				destFinalOrbit = orbitalElementsState4,
				destFinalBarycenter = tinaturalSpaceObjectState
			};
		}

		// Token: 0x06003E17 RID: 15895 RVA: 0x0018F938 File Offset: 0x0018DB38
		private static TransferResult CalculateOrbitPhasingTransfers_(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, int requestSize, IMobileAsset fleet, double fleetDeltaV_mps, double fleetAcceleration_mps2, ITransferTarget originValue, TISpaceGameState sDestination, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, bool stopAfterFirstSuccess = false)
		{
			MasterTransferPlanner.OrbitPhasingConstraints orbitPhasingConstraints = MasterTransferPlanner.OrbitPhasing_GetPhasingOrbitsAndTiming(fleet, destinationValue, commonBarycenter);
			if (!orbitPhasingConstraints.succeeded)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			List<Trajectory> list = new List<Trajectory>();
			OrbitPhasingTransfer orbitPhasingTransfer = new OrbitPhasingTransfer();
			double num = orbitPhasingConstraints.destOrbit.OrbitalPeriod(commonBarycenter.mass_kg);
			double num2 = orbitPhasingConstraints.earliestArrivalTime.DifferenceInSeconds(orbitPhasingConstraints.earliestLaunchTime);
			double num3 = orbitPhasingConstraints.latestArrivalTime.DifferenceInSeconds(orbitPhasingConstraints.earliestLaunchTime);
			int num4 = Mathd.FloorToInt(num3 / num);
			int num5 = Mathd.Max(1, Mathd.CeilToInt(num2 / num));
			if (num5 > num4)
			{
				return new TransferResult(TransferResult.Outcome.Fail_ExceedsMaxDuration, num3, 0.0);
			}
			TransferResult transferResult = null;
			int num6 = Mathd.Max(num5, OrbitPhasingTransfer.CalculateMinOrbitsGivenAcceleration(orbitPhasingConstraints.ourOrbit, orbitPhasingConstraints.destOrbit, commonBarycenter, fleetAcceleration_mps2, true));
			int num7 = Mathd.Max(num5, OrbitPhasingTransfer.CalculateMinOrbitsGivenAcceleration(orbitPhasingConstraints.ourOrbit, orbitPhasingConstraints.destOrbit, commonBarycenter, fleetAcceleration_mps2, false));
			int num8 = Mathd.Min(num4, num6 + requestSize / 2);
			int num9 = Mathd.Min(num4, num7 + requestSize / 2);
			while (num6 <= num8 || num7 <= num9)
			{
				int i = 0;
				while (i <= 1)
				{
					int num10;
					if (i > 0)
					{
						if (num6 <= num8)
						{
							num10 = num6;
							goto IL_0134;
						}
					}
					else if (num7 <= num9)
					{
						num10 = num7;
						goto IL_0134;
					}
					IL_02FF:
					i++;
					continue;
					IL_0134:
					TransferResult transferResult2 = orbitPhasingTransfer.Solve(orbitPhasingConstraints.earliestLaunchTime, num10, i > 0, originValue, destinationValue, destinationValue as TISpaceFleetState, commonBarycenter, orbitPhasingConstraints.ourOrbit, orbitPhasingConstraints.destOrbit, orbitPhasingConstraints.ourInitialOrbit, orbitPhasingConstraints.ourInitialBarycenter, orbitPhasingConstraints.destFinalOrbit, orbitPhasingConstraints.destFinalBarycenter, (double)fleet.cruiseAcceleration_mps2);
					if (transferResult2.Result != TransferResult.Outcome.Success)
					{
						transferResult = TransferResult.Best(transferResult, transferResult2);
						goto IL_02FF;
					}
					if (orbitPhasingTransfer.DV_mps > fleetDeltaV_mps)
					{
						lowestDVFound_kps = Mathd.Min(orbitPhasingTransfer.DV_mps / 1000.0, lowestDVFound_kps);
						transferResult = TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, orbitPhasingTransfer.DV_mps, 0.0));
						goto IL_02FF;
					}
					Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
					trajectory_Patched.BuildSingleOrbitPhasingTrajectory(fleet, sDestination, originValue, destinationValue, commonBarycenter, orbitPhasingTransfer, fleetAcceleration_mps2, orbitPhasingConstraints.ourInitialOrbit, orbitPhasingConstraints.destFinalOrbit, orbitPhasingConstraints.ourInitialBarycenter, orbitPhasingConstraints.destFinalBarycenter);
					Trajectory trajectory = trajectory_Patched;
					if (stopAfterFirstSuccess)
					{
						candidateTrajectories.Add(trajectory_Patched);
						return transferResult2;
					}
					bool flag = false;
					foreach (Trajectory trajectory2 in list)
					{
						if (trajectory.arrivalTime >= trajectory2.arrivalTime && trajectory.DV_mps >= trajectory2.DV_mps)
						{
							flag = true;
							break;
						}
					}
					if (!flag && trajectory.DV_mps < fleetDeltaV_mps)
					{
						list.Add(trajectory);
					}
					if (trajectory.DV_mps > fleetDeltaV_mps)
					{
						transferResult = TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, trajectory.DV_mps, 0.0));
					}
					else if (transferResult == null || transferResult.Result > TransferResult.Outcome.Success)
					{
						transferResult = new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
					}
					lowestDVFound_kps = Mathd.Min(trajectory.DV_kps, lowestDVFound_kps);
					goto IL_02FF;
				}
				num6++;
				num7++;
			}
			candidateTrajectories.AddRange(list);
			if (transferResult == null)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			return transferResult;
		}

		// Token: 0x06003E18 RID: 15896 RVA: 0x0018FCA8 File Offset: 0x0018DEA8
		private static TransferResult CalculateInclinationChangeTransfers(ref List<Trajectory> candidateTrajectories, ref double lowestDVFound_kps, double sampleSizeMultiplier, IMobileAsset fleet, double fleetDeltaV_mps, double fleetAcceleration_mps2, ITransferTarget originValue, TISpaceGameState sDestination, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, bool stopOnFirstSuccess = false)
		{
			TransferResult transferResult = null;
			bool flag = destinationValue is TIOrbitState;
			InclinationChangeTransfer inclinationChangeTransfer = new InclinationChangeTransfer();
			double num = double.PositiveInfinity;
			int num2 = Mathd.CeilToInt(30.0 * sampleSizeMultiplier);
			for (int i = 1; i < num2; i++)
			{
				TransferResult transferResult2 = inclinationChangeTransfer.Solve(TITimeState.Now(), (double)i, originValue, destinationValue, commonBarycenter, (double)fleet.cruiseAcceleration_mps2, flag, null, null, false);
				if (transferResult2.Result != TransferResult.Outcome.Success)
				{
					transferResult = TransferResult.Best(transferResult, transferResult2);
				}
				else
				{
					if (num < inclinationChangeTransfer.DV_mps)
					{
						break;
					}
					num = inclinationChangeTransfer.DV_mps;
					Trajectory_Patched trajectory_Patched = new Trajectory_Patched();
					trajectory_Patched.BuildSingleTrajectory(fleet, sDestination, originValue, destinationValue, commonBarycenter, inclinationChangeTransfer, (double)fleet.cruiseAcceleration_mps2);
					if (trajectory_Patched.DV_mps <= fleetDeltaV_mps)
					{
						transferResult = transferResult2;
						candidateTrajectories.Add(trajectory_Patched);
						if (stopOnFirstSuccess)
						{
							return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
						}
					}
					else
					{
						transferResult = TransferResult.Best(transferResult, new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, trajectory_Patched.DV_mps, 0.0));
					}
				}
			}
			if (transferResult == null)
			{
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			return transferResult;
		}

		// Token: 0x06003E19 RID: 15897 RVA: 0x0018FDE8 File Offset: 0x0018DFE8
		private static double HohmannFirstBurnDuration_s(IMobileAsset fleet, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter)
		{
			double num = (double)fleet.cruiseAcceleration_mps2;
			MasterTransferPlanner.SimplifiedPositions simplifiedPositions = MasterTransferPlanner.GetSimplifiedPositions(originValue, destinationValue, TITimeState.Now(), null);
			double originDistToCommonBarycenter_m = simplifiedPositions.originDistToCommonBarycenter_m;
			double destinationDistToCommonBarycenter_m = simplifiedPositions.destinationDistToCommonBarycenter_m;
			double mu = simplifiedPositions.commonBarycenter.mu;
			return MasterTransferPlanner.HohmannFirstBurnDuration_s(num, originDistToCommonBarycenter_m, destinationDistToCommonBarycenter_m, mu);
		}

		// Token: 0x06003E1A RID: 15898 RVA: 0x0018FE2A File Offset: 0x0018E02A
		private static double HohmannFirstBurnDuration_s(double fleetAcceleration_mps, double startOrbitalRadius_m, double destinationSemiMajorAxis_m, double mu)
		{
			return MasterTransferPlanner.HohmannFirstBurnDV_mps(startOrbitalRadius_m, destinationSemiMajorAxis_m, mu) / fleetAcceleration_mps;
		}

		// Token: 0x06003E1B RID: 15899 RVA: 0x0018FE36 File Offset: 0x0018E036
		private static double HohmannFirstBurnDV_mps(double startOrbitalRadius_m, double destinationSemiMajorAxis_m, double mu)
		{
			return Mathd.Abs(Mathd.Sqrt(mu / startOrbitalRadius_m) * (Mathd.Sqrt(2.0 * destinationSemiMajorAxis_m / (startOrbitalRadius_m + destinationSemiMajorAxis_m)) - 1.0));
		}

		// Token: 0x06003E1C RID: 15900 RVA: 0x0018FE64 File Offset: 0x0018E064
		private static double HohmannFinalBurnDV_mps(double startOrbitalRadius_m, double destinationSemiMajorAxis_m, double mu)
		{
			return Mathd.Abs(Mathd.Sqrt(mu / destinationSemiMajorAxis_m) * (1.0 - Mathd.Sqrt(2.0 * startOrbitalRadius_m / (startOrbitalRadius_m + destinationSemiMajorAxis_m))));
		}

		// Token: 0x06003E1D RID: 15901 RVA: 0x0018FE92 File Offset: 0x0018E092
		public static double HohmannTotalDV_mps(double startOrbitalRadius_m, double destinationSemiMajorAxis_m, double mu)
		{
			return MasterTransferPlanner.HohmannFirstBurnDV_mps(startOrbitalRadius_m, destinationSemiMajorAxis_m, mu) + MasterTransferPlanner.HohmannFinalBurnDV_mps(startOrbitalRadius_m, destinationSemiMajorAxis_m, mu);
		}

		// Token: 0x06003E1E RID: 15902 RVA: 0x0018FEA8 File Offset: 0x0018E0A8
		private static double HohmannDuration_s(double startSemiMajorAxis_m, double endSemiMajorAxis_m, double mu)
		{
			double num = (startSemiMajorAxis_m + endSemiMajorAxis_m) / 2.0;
			return 3.141592653589793 * Mathd.Sqrt(num * num * num / mu);
		}

		// Token: 0x06003E1F RID: 15903 RVA: 0x0018FED9 File Offset: 0x0018E0D9
		private static double HohmannDuration_s(MasterTransferPlanner.SimplifiedPositions simplified)
		{
			return MasterTransferPlanner.HohmannDuration_s(simplified.originDistToCommonBarycenter_m, simplified.destinationDistToCommonBarycenter_m, simplified.commonBarycenter.mu);
		}

		// Token: 0x06003E20 RID: 15904 RVA: 0x0018FEF8 File Offset: 0x0018E0F8
		public static double SynodicPeriod_s(double semiMajorAxis1_m, double semiMajorAxis2_m, double mu)
		{
			if (semiMajorAxis1_m == semiMajorAxis2_m)
			{
				return double.PositiveInfinity;
			}
			double num = 6.283185307179586 * Mathd.Sqrt(semiMajorAxis1_m * semiMajorAxis1_m * semiMajorAxis1_m / mu);
			double num2 = 6.283185307179586 * Mathd.Sqrt(semiMajorAxis2_m * semiMajorAxis2_m * semiMajorAxis2_m / mu);
			if (Mathd.Approximately(num, num2))
			{
				return Mathd.Max(num, num2) * 10.0;
			}
			return Mathd.Min(Mathd.Abs(num * num2 / (num - num2)), Mathd.Max(num, num2) * 10.0);
		}

		// Token: 0x040026B0 RID: 9904
		private const int ARRIVAL_TIME_ITERATIONS_PLAYER = 60;

		// Token: 0x040026B1 RID: 9905
		private const int ARRIVAL_TIME_ITERATIONS_AI = 30;

		// Token: 0x040026B2 RID: 9906
		private const int ARRIVAL_TIME_ITERATIONS_MIN = 10;

		// Token: 0x040026B3 RID: 9907
		private const int LAUNCH_TIME_ITERATIONS_PLAYER = 7;

		// Token: 0x040026B4 RID: 9908
		private const int LAUNCH_TIME_ITERATIONS_AI = 5;

		// Token: 0x040026B5 RID: 9909
		private const int ARRIVAL_ANOMALY_ITERATIONS_PLAYER = 5;

		// Token: 0x040026B6 RID: 9910
		private const int ARRIVAL_ANOMALY_ITERATIONS_AI = 5;

		// Token: 0x040026B7 RID: 9911
		public const double MAX_MICROTHRUST_DURATION = 78892310.0;

		// Token: 0x040026B8 RID: 9912
		public const double MAX_TRANSFER_DURATION = 78892310.0;

		// Token: 0x040026B9 RID: 9913
		public const double MAX_TRANSFER_DURATION_ALIEN = 78892310.0;

		// Token: 0x040026BA RID: 9914
		protected static Queue<MasterTransferPlanner.TrajectoryQueue> queue = new Queue<MasterTransferPlanner.TrajectoryQueue>();

		// Token: 0x040026BB RID: 9915
		protected static AsyncGPUReadbackRequest request;

		// Token: 0x040026BC RID: 9916
		protected static bool requestActive = false;

		// Token: 0x040026BD RID: 9917
		private static MasterTransferPlanner s_instance = null;

		// Token: 0x040026BE RID: 9918
		private const string dumpFile = "TrajectoriesDump.txt";

		// Token: 0x02000EC5 RID: 3781
		protected struct TrajectoryQueue
		{
			// Token: 0x060079CC RID: 31180 RVA: 0x0031AA28 File Offset: 0x00318C28
			public TrajectoryQueue(TISpaceFleetState fleet, TIGameState destination, TINaturalSpaceObjectState commonBarycenter, ITransferTarget relevantOriginOrbitalElements, ITransferTarget relevantDestinationOrbitalElements, int sweepRange, Action<Trajectory[]> callback)
			{
				this.fleet = fleet;
				this.destination = destination;
				this.sweepRange = sweepRange;
				this.callback = callback;
				this.commonBarycenter = commonBarycenter;
				this.commonBarycenter_mu = (float)commonBarycenter.mu;
				this.relevantOriginOrbitalElements = relevantOriginOrbitalElements;
				this.relevantDestinationOrbitalElements = relevantDestinationOrbitalElements;
			}

			// Token: 0x04005A41 RID: 23105
			public TISpaceFleetState fleet;

			// Token: 0x04005A42 RID: 23106
			public TIGameState destination;

			// Token: 0x04005A43 RID: 23107
			public TINaturalSpaceObjectState commonBarycenter;

			// Token: 0x04005A44 RID: 23108
			public float commonBarycenter_mu;

			// Token: 0x04005A45 RID: 23109
			public ITransferTarget relevantOriginOrbitalElements;

			// Token: 0x04005A46 RID: 23110
			public ITransferTarget relevantDestinationOrbitalElements;

			// Token: 0x04005A47 RID: 23111
			public int sweepRange;

			// Token: 0x04005A48 RID: 23112
			public Action<Trajectory[]> callback;
		}

		// Token: 0x02000EC6 RID: 3782
		private class CalculateImpulseMicrothrustHybridTransfer_Params
		{
			// Token: 0x04005A49 RID: 23113
			public int requestSize;

			// Token: 0x04005A4A RID: 23114
			public double sampleSizeMultiplier;

			// Token: 0x04005A4B RID: 23115
			public IMobileAsset fleet;

			// Token: 0x04005A4C RID: 23116
			public double fleetDeltaV_mps;

			// Token: 0x04005A4D RID: 23117
			public double fleetAcceleration_mps2;

			// Token: 0x04005A4E RID: 23118
			public ITransferTarget originValue;

			// Token: 0x04005A4F RID: 23119
			public TISpaceGameState sDestination;

			// Token: 0x04005A50 RID: 23120
			public ITransferTarget destinationValue;

			// Token: 0x04005A51 RID: 23121
			public TINaturalSpaceObjectState commonBarycenter;

			// Token: 0x04005A52 RID: 23122
			public TIDateTime now;

			// Token: 0x04005A53 RID: 23123
			public bool aerobreaking;

			// Token: 0x04005A54 RID: 23124
			public bool unsafeAerobreaking;

			// Token: 0x04005A55 RID: 23125
			public StreamWriter log;

			// Token: 0x04005A56 RID: 23126
			public bool stopOnFirstSuccess;
		}

		// Token: 0x02000EC7 RID: 3783
		protected class HohmannTiming
		{
			// Token: 0x060079CE RID: 31182 RVA: 0x0031AA80 File Offset: 0x00318C80
			[return: TupleElementNames(new string[] { "launchTime", "arrivalTime" })]
			public List<ValueTuple<TIDateTime, TIDateTime>> GetHohmannTimings(double sampleSizeMultiplier)
			{
				List<ValueTuple<TIDateTime, TIDateTime>> list = new List<ValueTuple<TIDateTime, TIDateTime>>();
				if (this.firstHohmannAfterInitial <= 0 || this.lastHohmannAfterInitial <= 0)
				{
					return list;
				}
				int num = Mathd.CeilToInt(10.0 * sampleSizeMultiplier);
				int num2 = Mathd.Max((this.lastHohmannAfterInitial - this.firstHohmannAfterInitial + 1 - 1) / num, 1);
				for (int i = Mathd.Max(this.lastHohmannAfterInitial - num * num2, this.firstHohmannAfterInitial); i <= this.lastHohmannAfterInitial; i += num2)
				{
					TIDateTime tidateTime = new TIDateTime(this.initialHohmannArrivalTime, this.synodicPeriod_s * (double)i);
					TIDateTime tidateTime2 = new TIDateTime(tidateTime, -this.transferDuration_s);
					list.Add(new ValueTuple<TIDateTime, TIDateTime>(tidateTime2, tidateTime));
				}
				return list;
			}

			// Token: 0x04005A57 RID: 23127
			public TIDateTime initialHohmannArrivalTime;

			// Token: 0x04005A58 RID: 23128
			public double transferDuration_s;

			// Token: 0x04005A59 RID: 23129
			public double synodicPeriod_s;

			// Token: 0x04005A5A RID: 23130
			public int firstHohmannAfterInitial;

			// Token: 0x04005A5B RID: 23131
			public int lastHohmannAfterInitial;
		}

		// Token: 0x02000EC8 RID: 3784
		private class SimplifiedPositions
		{
			// Token: 0x04005A5C RID: 23132
			public double originDistToLocalBarycenter_m;

			// Token: 0x04005A5D RID: 23133
			public TINaturalSpaceObjectState originLocalBarycenter;

			// Token: 0x04005A5E RID: 23134
			public double destinationDistToLocalBarycenter_m;

			// Token: 0x04005A5F RID: 23135
			public TINaturalSpaceObjectState destinationLocalBarycenter;

			// Token: 0x04005A60 RID: 23136
			public double originDistToCommonBarycenter_m;

			// Token: 0x04005A61 RID: 23137
			public double destinationDistToCommonBarycenter_m;

			// Token: 0x04005A62 RID: 23138
			public TINaturalSpaceObjectState commonBarycenter;
		}

		// Token: 0x02000EC9 RID: 3785
		private class IdentifyHybridTransferType_Result
		{
			// Token: 0x170011CE RID: 4558
			// (get) Token: 0x060079D1 RID: 31185 RVA: 0x0031AB3D File Offset: 0x00318D3D
			public double totalMicrothrustDuration_s
			{
				get
				{
					return this.outspiralDuration_s + this.inspiralDuration_s;
				}
			}

			// Token: 0x04005A63 RID: 23139
			public bool isMicrothrustOnly;

			// Token: 0x04005A64 RID: 23140
			public bool isGoingOut;

			// Token: 0x04005A65 RID: 23141
			public double outspiralDuration_s;

			// Token: 0x04005A66 RID: 23142
			public double inspiralDuration_s;

			// Token: 0x04005A67 RID: 23143
			public double commonMicrothrustRadius_m;
		}

		// Token: 0x02000ECA RID: 3786
		private class MicrothrustStatistics
		{
			// Token: 0x170011CF RID: 4559
			// (get) Token: 0x060079D3 RID: 31187 RVA: 0x0031AB54 File Offset: 0x00318D54
			public double outspiralDuration_s
			{
				get
				{
					return this.startDepth2MicrothrustDuration_s + this.startDepth1MicrothrustDuration_s + (this.isCommonGoingOut ? this.commonMicrothrustDuration_s : 0.0);
				}
			}

			// Token: 0x170011D0 RID: 4560
			// (get) Token: 0x060079D4 RID: 31188 RVA: 0x0031AB7D File Offset: 0x00318D7D
			public double inspiralDuration_s
			{
				get
				{
					return this.endDepth2MicrothrustDuration_s + this.endDepth1MicrothrustDuration_s + (this.isCommonGoingOut ? 0.0 : this.commonMicrothrustDuration_s);
				}
			}

			// Token: 0x170011D1 RID: 4561
			// (get) Token: 0x060079D5 RID: 31189 RVA: 0x0031ABA6 File Offset: 0x00318DA6
			public double totalMicrothrustDuration_s
			{
				get
				{
					return this.outspiralDuration_s + this.inspiralDuration_s;
				}
			}

			// Token: 0x04005A68 RID: 23144
			public bool isMicrothrustOnly;

			// Token: 0x04005A69 RID: 23145
			public bool isCommonGoingOut;

			// Token: 0x04005A6A RID: 23146
			public double commonMicrothrustDuration_s;

			// Token: 0x04005A6B RID: 23147
			public double commonMicrothrustAnomalyDelta_Rad;

			// Token: 0x04005A6C RID: 23148
			public double startDepth1MicrothrustDuration_s;

			// Token: 0x04005A6D RID: 23149
			public double startDepth1MicrothrustAnomalyDelta_Rad;

			// Token: 0x04005A6E RID: 23150
			public double startDepth1Radius_m;

			// Token: 0x04005A6F RID: 23151
			public double startDepth2MicrothrustDuration_s;

			// Token: 0x04005A70 RID: 23152
			public double startDepth2MicrothrustAnomalyDelta_Rad;

			// Token: 0x04005A71 RID: 23153
			public double startDepth2Radius_m;

			// Token: 0x04005A72 RID: 23154
			public double endDepth1MicrothrustDuration_s;

			// Token: 0x04005A73 RID: 23155
			public double endDepth1MicrothrustAnomalyDelta_Rad;

			// Token: 0x04005A74 RID: 23156
			public double endDepth1Radius_m;

			// Token: 0x04005A75 RID: 23157
			public double endDepth2MicrothrustDuration_s;

			// Token: 0x04005A76 RID: 23158
			public double endDepth2MicrothrustAnomalyDelta_Rad;

			// Token: 0x04005A77 RID: 23159
			public double endDepth2Radius_m;
		}

		// Token: 0x02000ECB RID: 3787
		private class OrbitPhasingConstraints
		{
			// Token: 0x04005A78 RID: 23160
			public bool succeeded;

			// Token: 0x04005A79 RID: 23161
			public OrbitalElementsState ourOrbit;

			// Token: 0x04005A7A RID: 23162
			public OrbitalElementsState destOrbit;

			// Token: 0x04005A7B RID: 23163
			public TIDateTime earliestLaunchTime;

			// Token: 0x04005A7C RID: 23164
			public TIDateTime earliestArrivalTime;

			// Token: 0x04005A7D RID: 23165
			public TIDateTime latestArrivalTime;

			// Token: 0x04005A7E RID: 23166
			public OrbitalElementsState ourInitialOrbit;

			// Token: 0x04005A7F RID: 23167
			public TINaturalSpaceObjectState ourInitialBarycenter;

			// Token: 0x04005A80 RID: 23168
			public OrbitalElementsState destFinalOrbit;

			// Token: 0x04005A81 RID: 23169
			public TINaturalSpaceObjectState destFinalBarycenter;
		}

		// Token: 0x02000ECC RID: 3788
		private class TransferCalculatorParameters
		{
			// Token: 0x060079D8 RID: 31192 RVA: 0x0031ABC8 File Offset: 0x00318DC8
			public bool Verify()
			{
				if (this.origin == null)
				{
					Log.Warn("MasterTransferPlanner: TransferCalculatorParameters: no origin specified.", Array.Empty<object>());
					return false;
				}
				if (this.destination == null)
				{
					Log.Warn("MasterTransferPlanner: TransferCalculatorParameters: no destination specified.", Array.Empty<object>());
					return false;
				}
				if (this.startTime == null)
				{
					Log.Warn("MasterTransferPlanner: TransferCalculatorParameters: no start time specified.", Array.Empty<object>());
					return false;
				}
				if (this.arrivalCap != null && this.arrivalCap <= this.startTime)
				{
					Log.Warn("MasterTransferPlanner: TransferCalculatorParameters: maximum arrival time is not after start time.", Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x04005A82 RID: 23170
			public ITransferTarget origin;

			// Token: 0x04005A83 RID: 23171
			public ITransferTarget destination;

			// Token: 0x04005A84 RID: 23172
			public double fleetAcceleration_mps;

			// Token: 0x04005A85 RID: 23173
			public double fleetDV_mps2;

			// Token: 0x04005A86 RID: 23174
			public int requestSize;

			// Token: 0x04005A87 RID: 23175
			public TIDateTime startTime;

			// Token: 0x04005A88 RID: 23176
			public TIDateTime arrivalCap;
		}
	}
}
