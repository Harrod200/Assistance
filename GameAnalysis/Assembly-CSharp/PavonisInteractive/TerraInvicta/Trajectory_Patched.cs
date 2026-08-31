using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007CF RID: 1999
	public class Trajectory_Patched : Trajectory
	{
		// Token: 0x17000E28 RID: 3624
		// (get) Token: 0x060047C0 RID: 18368 RVA: 0x001D5BB3 File Offset: 0x001D3DB3
		public override TrajectoryModel GetTrajectoryModel
		{
			get
			{
				return TrajectoryModel.Patched;
			}
		}

		// Token: 0x17000E29 RID: 3625
		// (get) Token: 0x060047C1 RID: 18369 RVA: 0x001D5BB7 File Offset: 0x001D3DB7
		public override double DV_mps
		{
			get
			{
				return this.Segments.Sum<Trajectory_Patched.IPatchSegment>(delegate(Trajectory_Patched.IPatchSegment x)
				{
					if (!(x.startTime >= base.assignedTime))
					{
						return x.DV_mps - x.DVConsumedByTime(base.assignedTime);
					}
					return x.DV_mps;
				}) + this.DV_targetFleet_mps;
			}
		}

		// Token: 0x060047C2 RID: 18370 RVA: 0x001D5BD8 File Offset: 0x001D3DD8
		public bool BuildInterimTrajectory(TISpaceFleetState fleet, Trajectory_Patched oldTrajectory, Trajectory_Patched newTrajectory, TIDateTime startOfInterimTrajectory, TIDateTime middleOfBurn, double burnDuration_s, OrbitalElementsState orbitAtStart, TINaturalSpaceObjectState barycenter, double fleetAcceleration_mps2)
		{
			if (startOfInterimTrajectory > middleOfBurn)
			{
				Log.Error(string.Concat(new string[]
				{
					"Trajectory_Patched.BuildInterimTrajectory: we're trying to perform a burn at ",
					(middleOfBurn != null) ? middleOfBurn.ToString() : null,
					"which is in our past (now is ",
					(startOfInterimTrajectory != null) ? startOfInterimTrajectory.ToString() : null,
					")"
				}), Array.Empty<object>());
			}
			this.Segments = new List<Trajectory_Patched.IPatchSegment>();
			TIDateTime tidateTime = new TIDateTime(middleOfBurn, -burnDuration_s / 2.0);
			TIDateTime tidateTime2 = new TIDateTime(middleOfBurn, burnDuration_s / 2.0);
			CartesianState cartesianState = oldTrajectory.ToGlobalCartesianStateAtTime(tidateTime).ToLocal(barycenter, tidateTime);
			CartesianState cartesianState2 = newTrajectory.ToGlobalCartesianStateAtTime(tidateTime2).ToLocal(barycenter, tidateTime2);
			bool flag = this.BuildInterimTrajectory_Common(fleet, oldTrajectory, tidateTime, tidateTime2, cartesianState, cartesianState2, startOfInterimTrajectory, orbitAtStart, barycenter, fleetAcceleration_mps2, newTrajectory.destination);
			this.nextTrajectory = newTrajectory;
			if (this.nextTrajectory.fleet != base.fleet)
			{
				this.nextTrajectory = this.nextTrajectory.ShallowCopy(base.fleet);
			}
			return flag;
		}

		// Token: 0x060047C3 RID: 18371 RVA: 0x001D5CE8 File Offset: 0x001D3EE8
		public bool BuildInterimTrajectory(TISpaceFleetState fleet, Trajectory_Patched oldTrajectory, OrbitalElementsState newOrbit, TIDateTime startOfInterimTrajectory, TIDateTime middleOfBurn, double burnDuration_s, OrbitalElementsState orbitAtStart, TINaturalSpaceObjectState barycenter, double fleetAcceleration_mps2)
		{
			if (startOfInterimTrajectory > middleOfBurn)
			{
				Log.Error(string.Concat(new string[]
				{
					"Trajectory_Patched.BuildInterimTrajectory: we're trying to perform a burn at ",
					(middleOfBurn != null) ? middleOfBurn.ToString() : null,
					"which is in our past (now is ",
					(startOfInterimTrajectory != null) ? startOfInterimTrajectory.ToString() : null,
					")"
				}), Array.Empty<object>());
			}
			this.Segments = new List<Trajectory_Patched.IPatchSegment>();
			TIDateTime tidateTime = new TIDateTime(middleOfBurn, -burnDuration_s / 2.0);
			TIDateTime tidateTime2 = new TIDateTime(middleOfBurn, burnDuration_s / 2.0);
			CartesianState cartesianState = oldTrajectory.ToGlobalCartesianStateAtTime(tidateTime).ToLocal(barycenter, tidateTime);
			CartesianState cartesianState2 = newOrbit.ToCartesianStateAtTime(tidateTime2.ExportTime(), barycenter.mass_kg);
			base.destinationOrbit = TIAdHocOrbitState.CreateAdHocOrbitState(barycenter, newOrbit, fleet);
			bool flag = this.BuildInterimTrajectory_Common(fleet, oldTrajectory, tidateTime, tidateTime2, cartesianState, cartesianState2, startOfInterimTrajectory, orbitAtStart, barycenter, fleetAcceleration_mps2, base.destinationOrbit);
			ValueTuple<double, double>? meanAnomalyWhenAtRadius = newOrbit.GetMeanAnomalyWhenAtRadius(cartesianState2.position.magnitude, barycenter);
			this.destinationOrbitEpoch = tidateTime2;
			this.destinationOrbitMeanAnomalyAtEpoch = new double?(((Vector3d.Dot(in cartesianState2.velocity, in cartesianState2.position) < 0.0) ? ((meanAnomalyWhenAtRadius != null) ? new double?(meanAnomalyWhenAtRadius.GetValueOrDefault().Item2) : null) : ((meanAnomalyWhenAtRadius != null) ? new double?(meanAnomalyWhenAtRadius.GetValueOrDefault().Item1) : null)) ?? ((cartesianState2.position.magnitude < newOrbit.semiMajorAxis_m) ? 0.0 : 3.141592653589793));
			return flag;
		}

		// Token: 0x060047C4 RID: 18372 RVA: 0x001D5EAC File Offset: 0x001D40AC
		public static Trajectory BuildTruncatedTrajectory(TISpaceFleetState fleet, Trajectory_Patched oldTrajectory, TIDateTime removeEverythingBeforeThisTime)
		{
			Trajectory trajectory = oldTrajectory.ShallowCopy(fleet);
			Trajectory_Patched trajectory_Patched = trajectory as Trajectory_Patched;
			if (trajectory_Patched != null)
			{
				List<Trajectory_Patched.IPatchSegment> segments = trajectory_Patched.Segments;
				trajectory_Patched.Segments = new List<Trajectory_Patched.IPatchSegment>();
				trajectory_Patched.Segments.AddRange(segments);
				Trajectory_Patched.IPatchSegment patchSegment = trajectory_Patched.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime < removeEverythingBeforeThisTime);
				trajectory_Patched.Segments.RemoveAll((Trajectory_Patched.IPatchSegment x) => x.startTime < removeEverythingBeforeThisTime);
				Trajectory_Patched.IPatchSegment patchSegment2 = trajectory_Patched.Segments.FirstOrDefault<Trajectory_Patched.IPatchSegment>();
				if (((patchSegment2 != null) ? patchSegment2.startTime : null) != removeEverythingBeforeThisTime)
				{
					Trajectory_Patched.ISupportsReducedCopy supportsReducedCopy = patchSegment as Trajectory_Patched.ISupportsReducedCopy;
					if (supportsReducedCopy != null)
					{
						trajectory_Patched.Segments.Insert(0, supportsReducedCopy.ReducedCopy(removeEverythingBeforeThisTime, supportsReducedCopy.endTime));
					}
					else
					{
						trajectory_Patched.Segments.Insert(0, patchSegment);
					}
				}
				trajectory_Patched.launchTime = TITimeState.Now();
				trajectory_Patched.assignedTime = TITimeState.Now();
				trajectory_Patched.duration = trajectory_Patched.arrivalTime.ExportTime() - trajectory_Patched.assignedTime.ExportTime();
			}
			return trajectory;
		}

		// Token: 0x060047C5 RID: 18373 RVA: 0x001D5FC0 File Offset: 0x001D41C0
		private bool BuildInterimTrajectory_Common(TISpaceFleetState fleet, Trajectory_Patched oldTrajectory, TIDateTime startOfBurn, TIDateTime endOfBurn, CartesianState cartesianBeforeBurn, CartesianState cartesianAfterBurn, TIDateTime startOfInterimTrajectory, OrbitalElementsState orbitAtStart, TINaturalSpaceObjectState barycenter, double fleetAcceleration_mps2, TISpaceGameState destination)
		{
			double num = endOfBurn.DifferenceInSeconds(startOfBurn);
			BurnBezierDescription burnBezierDescription = new BurnBezierDescription(cartesianBeforeBurn, cartesianAfterBurn, num);
			if (startOfBurn > startOfInterimTrajectory)
			{
				int num2 = 0;
				while (num2 < oldTrajectory.Segments.Count && !(oldTrajectory.Segments[num2].startTime > startOfBurn))
				{
					if (oldTrajectory.Segments.Count <= num2 + 1 || !(oldTrajectory.Segments[num2 + 1].startTime < startOfInterimTrajectory))
					{
						this.Segments.Add(oldTrajectory.Segments[num2]);
					}
					num2++;
				}
			}
			if (oldTrajectory.nextTrajectory != null && startOfBurn > oldTrajectory.nextTrajectory.launchTime)
			{
				Trajectory_Patched trajectory_Patched = oldTrajectory.nextTrajectory as Trajectory_Patched;
				if (trajectory_Patched != null)
				{
					int num3 = 0;
					while (num3 < trajectory_Patched.Segments.Count && !(trajectory_Patched.Segments[num3].startTime > startOfBurn))
					{
						if (trajectory_Patched.Segments.Count <= num3 + 1 || !(trajectory_Patched.Segments[num3 + 1].startTime < startOfInterimTrajectory))
						{
							this.Segments.Add(trajectory_Patched.Segments[num3]);
						}
						num3++;
					}
				}
			}
			if (barycenter == null)
			{
				barycenter = this.Segments.Last<Trajectory_Patched.IPatchSegment>().barycenter;
				Debug.LogError("Interim trajectory lacked a barycenter.  Defaulting to " + ((barycenter != null) ? barycenter.displayName : null));
			}
			this.Segments.Add(new Trajectory_Patched.BurnSegment
			{
				startTime = startOfBurn,
				burnDuration_s = num,
				fleetAccel_mps2 = fleetAcceleration_mps2,
				isBoost = true,
				barycenter = barycenter,
				burnDescription = burnBezierDescription
			});
			TIDateTime tidateTime = TITimeState.Now();
			double num4 = endOfBurn.DifferenceInSeconds(startOfInterimTrajectory);
			this.exitsSolarSystem = false;
			base.fleetCruiseAcceleration_mps2 = fleetAcceleration_mps2;
			base.launchTime = tidateTime;
			base.arrivalTime = new TIDateTime(endOfBurn);
			base.BuildSingleTrajectory_Common(fleet, destination, barycenter, tidateTime, num4, true);
			base.assignedTime = tidateTime;
			this.loiterDuration_s = startOfBurn.DifferenceInSeconds(tidateTime);
			this.prepositionDuration_s = 0.0;
			this.boostDuration_s = num;
			this.decelDuration_s = 0.0;
			this.captureDuration_s = 0.0;
			this.coastDuration_s = base.launchTime.DifferenceInSeconds(base.assignedTime);
			base.duration = TimeSpan.FromSeconds(num4);
			base.originOrbit = oldTrajectory.originOrbit;
			if (oldTrajectory.destination != null)
			{
				this.originalDestinationSunOrbiter = (oldTrajectory.involuntary ? oldTrajectory.originalDestinationSunOrbiter : oldTrajectory.destination.ref_spaceObject.GetSunOrbitingRelatedObject);
			}
			else
			{
				this.originalDestinationSunOrbiter = (oldTrajectory.endsInCrash ? oldTrajectory.collisionTarget : oldTrajectory.commonBarycenter);
			}
			return true;
		}

		// Token: 0x060047C6 RID: 18374 RVA: 0x001D6298 File Offset: 0x001D4498
		public bool BuildCoastTrajectory(TISpaceFleetState fleet, Trajectory oldTrajectory, TIDateTime timeCoastStarts, OrbitalElementsState orbitAtStart, TINaturalSpaceObjectState barycenterAtStart)
		{
			this.involuntary = true;
			this.Segments = new List<Trajectory_Patched.IPatchSegment>();
			ValueTuple<TINaturalSpaceObjectState, OrbitalElementsState, TIDateTime, bool, bool> valueTuple = this.BuildCoastTrajectoryAroundBarycenter(oldTrajectory, timeCoastStarts, orbitAtStart, barycenterAtStart);
			TINaturalSpaceObjectState item = valueTuple.Item1;
			OrbitalElementsState item2 = valueTuple.Item2;
			TIDateTime item3 = valueTuple.Item3;
			bool item4 = valueTuple.Item4;
			bool item5 = valueTuple.Item5;
			if (this.Segments.Count == 0)
			{
				return false;
			}
			TISpaceObjectState tispaceObjectState;
			if (!oldTrajectory.involuntary)
			{
				TISpaceGameState destination = oldTrajectory.destination;
				tispaceObjectState = ((destination != null) ? destination.ref_spaceObject.GetSunOrbitingRelatedObject : null) ?? oldTrajectory.commonBarycenter;
			}
			else
			{
				tispaceObjectState = oldTrajectory.originalDestinationSunOrbiter;
			}
			this.originalDestinationSunOrbiter = tispaceObjectState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState = barycenterAtStart;
			foreach (Trajectory_Patched.IPatchSegment patchSegment in this.Segments)
			{
				tinaturalSpaceObjectState = tinaturalSpaceObjectState.FindCommonBarycenter(patchSegment.barycenter);
			}
			TISpaceGameState tispaceGameState = null;
			if (!item4 && !item5)
			{
				tispaceGameState = TIAdHocOrbitState.CreateAdHocOrbitState(item, item2, fleet);
			}
			double num = item3.DifferenceInSeconds(timeCoastStarts);
			this.exitsSolarSystem = item5;
			base.BuildSingleTrajectory_Common(fleet, tispaceGameState, tinaturalSpaceObjectState, timeCoastStarts, num, false);
			base.fleetCruiseAcceleration_mps2 = (double)fleet.cruiseAcceleration_mps2;
			base.launchTime = timeCoastStarts;
			base.arrivalTime = new TIDateTime(item3);
			base.assignedTime = TITimeState.Now();
			this.loiterDuration_s = base.launchTime.DifferenceInSeconds(base.assignedTime);
			this.prepositionDuration_s = 0.0;
			this.boostDuration_s = 0.0;
			this.decelDuration_s = 0.0;
			this.captureDuration_s = 0.0;
			this.coastDuration_s = base.arrivalTime.DifferenceInSeconds(base.launchTime);
			base.duration = TimeSpan.FromSeconds(base.arrivalTime.DifferenceInSeconds(TITimeState.Now()));
			base.originOrbit = oldTrajectory.originOrbit;
			return true;
		}

		// Token: 0x060047C7 RID: 18375 RVA: 0x001D6474 File Offset: 0x001D4674
		[return: TupleElementNames(new string[] { "finalBarycenter", "finalOrbit", "endTime", "endsInCrash", "leavesSolarSystem" })]
		private ValueTuple<TINaturalSpaceObjectState, OrbitalElementsState, TIDateTime, bool, bool> BuildCoastTrajectoryAroundBarycenter(Trajectory oldTrajectory, TIDateTime startTime, OrbitalElementsState orbit, TINaturalSpaceObjectState barycenter)
		{
			ValueTuple<double, double>? meanAnomalyWhenAtRadius = orbit.GetMeanAnomalyWhenAtRadius(barycenter.meanRadius_m, barycenter);
			ValueTuple<double, double>? valueTuple = null;
			TIDateTime tidateTime = null;
			if (!barycenter.isSun)
			{
				CartesianState cartesianState = orbit.ToCartesianStateAtTime(startTime.ExportTime(), barycenter.mass_kg);
				if (cartesianState.position.magnitude > barycenter.sphereOfInfluence_m)
				{
					tidateTime = startTime;
				}
				else
				{
					valueTuple = orbit.GetMeanAnomalyWhenAtRadius(barycenter.sphereOfInfluence_m, barycenter);
					if (orbit.eccentricity > 1.0 && valueTuple == null)
					{
						Log.Error("Trajectory_Patched.BuildCoastTrajectoryAroundBarycenter: we couldn't find a mean anomaly when leaving the sphere of influence, despite being hyperbolic.  sphere of influence = " + barycenter.sphereOfInfluence_m.ToString() + "m, eccentricity = " + orbit.eccentricity.ToString(), Array.Empty<object>());
					}
				}
			}
			TIDateTime tidateTime2 = null;
			if (meanAnomalyWhenAtRadius != null)
			{
				tidateTime2 = new TIDateTime(orbit.NextTimeAtMeanAnomaly(((meanAnomalyWhenAtRadius != null) ? new double?(meanAnomalyWhenAtRadius.GetValueOrDefault().Item2) : null).Value, startTime.ExportTime(), barycenter.mass_kg));
				if (tidateTime2 < startTime)
				{
					tidateTime2 = null;
				}
			}
			if (valueTuple != null)
			{
				tidateTime = new TIDateTime(orbit.NextTimeAtMeanAnomaly(((valueTuple != null) ? new double?(valueTuple.GetValueOrDefault().Item1) : null).Value, startTime.ExportTime(), barycenter.mass_kg));
			}
			TIDateTime tidateTime3 = null;
			if (barycenter.isSun)
			{
				ValueTuple<double, double>? meanAnomalyWhenAtRadius2 = orbit.GetMeanAnomalyWhenAtRadius(12000000000000.0, barycenter);
				if (meanAnomalyWhenAtRadius2 != null)
				{
					tidateTime3 = new TIDateTime(orbit.NextTimeAtMeanAnomaly(((meanAnomalyWhenAtRadius2 != null) ? new double?(meanAnomalyWhenAtRadius2.GetValueOrDefault().Item1) : null).Value, startTime.ExportTime(), barycenter.mass_kg));
				}
			}
			if (!(barycenter is TISpaceBodyState))
			{
				tidateTime2 = null;
			}
			if (tidateTime < tidateTime2 || tidateTime3 < tidateTime2)
			{
				tidateTime2 = null;
			}
			if (tidateTime2 < tidateTime || tidateTime3 < tidateTime)
			{
				tidateTime = null;
			}
			if (tidateTime2 < tidateTime3 || tidateTime < tidateTime3)
			{
				tidateTime3 = null;
			}
			Trajectory_Patched.IPatchSegment patchSegment;
			if (orbit.eccentricity < 1.0)
			{
				patchSegment = new Trajectory_Patched.OrbitSegment
				{
					barycenter = barycenter,
					orbit = orbit,
					startTime = startTime
				};
			}
			else
			{
				patchSegment = new Trajectory_Patched.HyperbolicOrbitSegment
				{
					barycenter = barycenter,
					orbit = orbit,
					startTime = startTime
				};
			}
			if (tidateTime != null)
			{
				this.Segments.Add(patchSegment);
				CartesianState cartesianState = orbit.ToCartesianStateAtTime(tidateTime.ExportTime(), barycenter.mass_kg);
				OrbitalElementsState orbitalElementsState = cartesianState.ChangeReferenceFrame(barycenter, barycenter.barycenter, tidateTime).ToOrbitalElementsState(barycenter.barycenter.mu, new DateTime?(tidateTime.ExportTime()));
				return this.BuildCoastTrajectoryAroundBarycenter(oldTrajectory, tidateTime, orbitalElementsState, barycenter.barycenter);
			}
			if (tidateTime2 != null)
			{
				this.Segments.Add(patchSegment);
				this.collisionTarget = (TISpaceBodyState)barycenter;
				return new ValueTuple<TINaturalSpaceObjectState, OrbitalElementsState, TIDateTime, bool, bool>(barycenter, orbit, tidateTime2, true, false);
			}
			if (tidateTime3 != null)
			{
				this.Segments.Add(patchSegment);
				return new ValueTuple<TINaturalSpaceObjectState, OrbitalElementsState, TIDateTime, bool, bool>(barycenter, orbit, tidateTime3, false, true);
			}
			return new ValueTuple<TINaturalSpaceObjectState, OrbitalElementsState, TIDateTime, bool, bool>(barycenter, orbit, startTime, false, false);
		}

		// Token: 0x060047C8 RID: 18376 RVA: 0x001D67C8 File Offset: 0x001D49C8
		public void BuildSingleOrbitPhasingTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, OrbitPhasingTransfer solver, double fleetAcceleration_mps2, OrbitalElementsState originOrbit, OrbitalElementsState destOrbit, TINaturalSpaceObjectState originBarycenter, TINaturalSpaceObjectState destBarycenter)
		{
			if (commonBarycenter == null)
			{
				commonBarycenter = originValue.barycenter().FindCommonBarycenter(destinationValue.barycenter());
				Debug.LogError("No common barycenter when making an orbit phasing trajectory.  Defaulting to " + ((commonBarycenter != null) ? commonBarycenter.displayName : null));
			}
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, solver.launchTime, solver.transitDuration_s, false);
			base.fleetCruiseAcceleration_mps2 = fleetAcceleration_mps2;
			base.launchTime = TIDateTime.Max(solver.launchTime, base.assignedTime);
			base.arrivalTime = new TIDateTime(solver.arrivalTime);
			this.loiterDuration_s = 0.0;
			this.prepositionDuration_s = 0.0;
			this.boostDuration_s = Mathd.Max(solver.burn_duration_s / 2.0 + solver.originMicrothrustDuration_s, solver.burn_duration_s) + solver.originGravityTax_s;
			this.decelDuration_s = Mathd.Max(solver.burn_duration_s / 2.0 + solver.destinationMicrothrustDuration_s, solver.burn_duration_s) + solver.destinationGravityTax_s;
			this.coastDuration_s = solver.transitDuration_s - this.boostDuration_s - this.decelDuration_s;
			this.captureDuration_s = 0.0;
			base.duration = TimeSpan.FromSeconds(solver.transitDuration_s);
			List<Trajectory_Patched.MicrothrustSegment> list = new List<Trajectory_Patched.MicrothrustSegment>();
			CartesianState cartesianState;
			TIDateTime tidateTime;
			OrbitalElementsState orbitalElementsState;
			if (solver.originMicrothrustDuration_s > 0.0)
			{
				list.AddRange(this.CreateMicrothrustSegmentsForOrbitPhasing(originOrbit, originBarycenter, solver.launchTime, commonBarycenter, fleetAcceleration_mps2, true, out cartesianState, out tidateTime));
				orbitalElementsState = cartesianState.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime.ExportTime()));
			}
			else
			{
				tidateTime = new TIDateTime(solver.launchTime);
				orbitalElementsState = originOrbit;
				cartesianState = originOrbit.ToCartesianStateAtTime(tidateTime.ExportTime(), originBarycenter.mass_kg);
				if (originBarycenter != commonBarycenter)
				{
					cartesianState.velocity = Vector3d.zero;
					cartesianState = cartesianState.ChangeReferenceFrame(originBarycenter, commonBarycenter, tidateTime);
					orbitalElementsState = cartesianState.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime.ExportTime()));
				}
			}
			List<Trajectory_Patched.MicrothrustSegment> list2 = new List<Trajectory_Patched.MicrothrustSegment>();
			CartesianState cartesianState2;
			TIDateTime tidateTime2;
			OrbitalElementsState orbitalElementsState2;
			if (solver.destinationMicrothrustDuration_s > 0.0)
			{
				list2.AddRange(this.CreateMicrothrustSegmentsForOrbitPhasing(destOrbit, destBarycenter, solver.arrivalTime, commonBarycenter, fleetAcceleration_mps2, false, out cartesianState2, out tidateTime2));
				orbitalElementsState2 = cartesianState2.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime2.ExportTime()));
			}
			else
			{
				tidateTime2 = new TIDateTime(solver.arrivalTime);
				orbitalElementsState2 = destOrbit;
				cartesianState2 = destOrbit.ToCartesianStateAtTime(tidateTime2.ExportTime(), destBarycenter.mass_kg);
				if (destBarycenter != commonBarycenter)
				{
					cartesianState2.velocity = Vector3d.zero;
					cartesianState2 = cartesianState2.ChangeReferenceFrame(destBarycenter, commonBarycenter, tidateTime2);
					orbitalElementsState2 = cartesianState2.ToOrbitalElementsState(commonBarycenter.mu, new DateTime?(tidateTime2.ExportTime()));
				}
			}
			double num = solver.transferOrbit.semiMajorAxis_m * 2.0;
			double magnitude = cartesianState.position.magnitude;
			double num2 = num - magnitude;
			OrbitalElementsState orbitalElementsState3 = new OrbitalElementsState
			{
				epoch = tidateTime.ExportTime(),
				longAscendingNode_Rad = orbitalElementsState.longAscendingNode_Rad,
				inclination_Rad = orbitalElementsState.inclination_Rad,
				argPeriapsis_Rad = Mathd.ClampRadiansTwoPI(orbitalElementsState.argPeriapsis_Rad + orbitalElementsState.TrueAnomalyAtTime_Rad(tidateTime.ExportTime(), commonBarycenter.mass_kg) + (solver.isGoingForward ? 3.141592653589793 : 0.0)),
				meanAnomalyAtEpoch_Rad = (solver.isGoingForward ? 3.141592653589793 : 0.0),
				semiMajorAxis_m = solver.transferOrbit.semiMajorAxis_m,
				eccentricity = Mathd.Abs(magnitude - num2) / num
			};
			OrbitalElementsState orbitalElementsState4 = orbitalElementsState3;
			double magnitude2 = cartesianState2.position.magnitude;
			double num3 = num - magnitude2;
			orbitalElementsState3 = new OrbitalElementsState
			{
				epoch = tidateTime2.ExportTime(),
				longAscendingNode_Rad = orbitalElementsState2.longAscendingNode_Rad,
				inclination_Rad = orbitalElementsState2.inclination_Rad,
				argPeriapsis_Rad = Mathd.ClampRadiansTwoPI(orbitalElementsState2.argPeriapsis_Rad + orbitalElementsState2.TrueAnomalyAtTime_Rad(tidateTime2.ExportTime(), commonBarycenter.mass_kg) + (solver.isGoingForward ? 3.141592653589793 : 0.0)),
				meanAnomalyAtEpoch_Rad = (solver.isGoingForward ? 3.141592653589793 : 0.0),
				semiMajorAxis_m = solver.transferOrbit.semiMajorAxis_m,
				eccentricity = Mathd.Abs(magnitude2 - num3) / num
			};
			OrbitalElementsState orbitalElementsState5 = orbitalElementsState3;
			DateTime epoch = orbitalElementsState4.epoch;
			orbitalElementsState5.meanAnomalyAtEpoch_Rad = orbitalElementsState5.MeanAnomalyAtTime_Rad(epoch, commonBarycenter.mass_kg);
			orbitalElementsState5.epoch = epoch;
			Trajectory_Patched.OrbitLERPSegment orbitLERPSegment = new Trajectory_Patched.OrbitLERPSegment
			{
				startTime = tidateTime,
				endTime = tidateTime2,
				initialOrbit = orbitalElementsState4,
				finalOrbit = orbitalElementsState5,
				barycenter = commonBarycenter,
				isImpulse = true,
				isOrbitPhasing = true
			};
			TIDateTime initialBurnStartTime = new TIDateTime(tidateTime, -solver.burn_duration_s / 2.0);
			Trajectory_Patched.MicrothrustSegment microthrustSegment = list.LastOrDefault<Trajectory_Patched.MicrothrustSegment>((Trajectory_Patched.MicrothrustSegment x) => x.startTime <= initialBurnStartTime);
			CartesianState cartesianState3;
			if (microthrustSegment == null)
			{
				list = new List<Trajectory_Patched.MicrothrustSegment>();
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				originValue.tryToGetLocalCartesianState(initialBurnStartTime, out cartesianState3, out tinaturalSpaceObjectState);
				cartesianState3.ChangeReferenceFrame(tinaturalSpaceObjectState, commonBarycenter, initialBurnStartTime);
			}
			else
			{
				if (list.Count == 2 && microthrustSegment == list.First<Trajectory_Patched.MicrothrustSegment>())
				{
					list.RemoveAt(1);
				}
				cartesianState3 = microthrustSegment.CartesianStateAtTime(initialBurnStartTime, commonBarycenter);
				microthrustSegment.endTime = initialBurnStartTime;
			}
			double num4 = solver.burn_duration_s + solver.originGravityTax_s;
			TIDateTime tidateTime3 = new TIDateTime(initialBurnStartTime, num4);
			CartesianState cartesianState4 = orbitLERPSegment.TrueCartesianStateAtTime(tidateTime3, commonBarycenter);
			BurnBezierDescription burnBezierDescription = new BurnBezierDescription(cartesianState3, cartesianState4, num4);
			Trajectory_Patched.BurnSegment burnSegment = new Trajectory_Patched.BurnSegment
			{
				startTime = initialBurnStartTime,
				burnDuration_s = num4,
				fleetAccel_mps2 = fleetAcceleration_mps2,
				isBoost = true,
				isImpulse = true,
				isOrbitPhasing = true,
				barycenter = commonBarycenter,
				burnDescription = burnBezierDescription
			};
			TIDateTime tidateTime4 = new TIDateTime(tidateTime2, solver.destinationGravityTax_s - solver.burn_duration_s / 2.0);
			double num5 = solver.burn_duration_s + solver.destinationGravityTax_s;
			TIDateTime finalBurnEndTime = new TIDateTime(tidateTime4, num5);
			Trajectory_Patched.MicrothrustSegment microthrustSegment2 = list2.LastOrDefault<Trajectory_Patched.MicrothrustSegment>((Trajectory_Patched.MicrothrustSegment x) => x.endTime > finalBurnEndTime);
			CartesianState cartesianState5;
			if (microthrustSegment2 == null)
			{
				list2 = new List<Trajectory_Patched.MicrothrustSegment>();
				cartesianState5 = Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(destinationValue, finalBurnEndTime, commonBarycenter, fleet.faction, null, 0.0);
			}
			else
			{
				if (list2.Count == 2 && microthrustSegment2 == list2.First<Trajectory_Patched.MicrothrustSegment>())
				{
					list2.RemoveAt(1);
				}
				cartesianState5 = microthrustSegment2.CartesianStateAtTime(finalBurnEndTime, commonBarycenter);
				OrbitalElementsState orbitalElementsState6 = microthrustSegment2.OrbitalElementsAtTime(finalBurnEndTime);
				microthrustSegment2.initialMeanAnomaly_rad = orbitalElementsState6.MeanAnomalyAtTime_Rad(finalBurnEndTime.ExportTime(), microthrustSegment2.barycenter.mass_kg);
				microthrustSegment2.initialVelocity_mps = Mathd.Sqrt(microthrustSegment2.barycenter.mu / orbitalElementsState6.semiMajorAxis_m);
				microthrustSegment2.startTime = finalBurnEndTime;
				microthrustSegment2.startTime = finalBurnEndTime;
			}
			BurnBezierDescription burnBezierDescription2 = new BurnBezierDescription(orbitLERPSegment.TrueCartesianStateAtTime(tidateTime4, commonBarycenter), cartesianState5, solver.burn_duration_s + solver.destinationGravityTax_s);
			Trajectory_Patched.BurnSegment burnSegment2 = new Trajectory_Patched.BurnSegment
			{
				startTime = tidateTime4,
				burnDuration_s = solver.burn_duration_s + solver.destinationGravityTax_s,
				fleetAccel_mps2 = fleetAcceleration_mps2,
				isBoost = false,
				isImpulse = true,
				isOrbitPhasing = true,
				barycenter = commonBarycenter,
				burnDescription = burnBezierDescription2
			};
			this.Segments = new List<Trajectory_Patched.IPatchSegment>();
			this.Segments.AddRange(list);
			this.Segments.Add(burnSegment);
			this.Segments.Add(orbitLERPSegment);
			this.Segments.Add(burnSegment2);
			list2.Reverse();
			this.Segments.AddRange(list2);
			this.UpdateDestinationOrbitWhenTargetingFleetInMotion();
			this.boostDV_mps = this.Segments.Sum<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.boostDV_mps);
			this.decelDV_mps = this.Segments.Sum<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.decelDV_mps);
		}

		// Token: 0x060047C9 RID: 18377 RVA: 0x001D703C File Offset: 0x001D523C
		public void BuildEmptyTrajectory(IMobileAsset fleet, TIDateTime transferTime, TISpaceGameState destination = null)
		{
			TIOrbitState ref_orbit = fleet.ref_orbit;
			base.BuildSingleTrajectory_Common(fleet, (destination == null) ? ref_orbit : destination, ref_orbit.barycenter, transferTime, 0.0, false);
			base.fleetCruiseAcceleration_mps2 = (double)fleet.cruiseAcceleration_mps2;
			base.launchTime = TIDateTime.Max(transferTime, base.assignedTime);
			base.arrivalTime = transferTime;
			this.loiterDuration_s = base.launchTime.DifferenceInSeconds(base.assignedTime);
			this.prepositionDuration_s = 0.0;
			this.boostDuration_s = 0.0;
			this.decelDuration_s = 0.0;
			this.captureDuration_s = 0.0;
			this.coastDuration_s = 0.0;
			base.duration = TimeSpan.FromSeconds(base.arrivalTime.DifferenceInSeconds(TITimeState.Now()));
			this.boostDV_mps = 0.0;
			this.decelDV_mps = 0.0;
			this.Segments = new List<Trajectory_Patched.IPatchSegment>
			{
				new Trajectory_Patched.OrbitSegment
				{
					startTime = transferTime,
					barycenter = ref_orbit.barycenter,
					orbit = ref_orbit.ToOrbitalElementsState(transferTime, fleet.meanAnomaly_Rad(transferTime))
				}
			};
		}

		// Token: 0x060047CA RID: 18378 RVA: 0x001D7174 File Offset: 0x001D5374
		public void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, InclinationChangeTransfer solver, double fleetAcceleration_mps2)
		{
			if (commonBarycenter == null)
			{
				commonBarycenter = originValue.barycenter().FindCommonBarycenter(destination.barycenter);
				Debug.LogError("No common barycenter when making a patched trajectory.  Defaulting to " + ((commonBarycenter != null) ? commonBarycenter.displayName : null));
			}
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, solver.launchTime, solver.transitDuration_s, false);
			base.fleetCruiseAcceleration_mps2 = fleetAcceleration_mps2;
			base.launchTime = TIDateTime.Max(solver.launchTime, base.assignedTime);
			base.arrivalTime = new TIDateTime(solver.arrivalTime);
			this.loiterDuration_s = base.launchTime.DifferenceInSeconds(base.assignedTime);
			this.prepositionDuration_s = 0.0;
			this.boostDuration_s = solver.boost_DV_mps / fleetAcceleration_mps2;
			this.decelDuration_s = solver.decel_DV_mps / fleetAcceleration_mps2;
			this.captureDuration_s = 0.0;
			this.coastDuration_s = base.arrivalTime.DifferenceInSeconds(base.launchTime) - this.boostDuration_s - this.decelDuration_s;
			base.duration = TimeSpan.FromSeconds(base.arrivalTime.DifferenceInSeconds(TITimeState.Now()));
			double num = solver.intermediate_burn_DV / base.fleetCruiseAcceleration_mps2;
			TIDateTime tidateTime = new TIDateTime(solver.intermediateBurnTime, -num / 2.0);
			TIDateTime tidateTime2 = new TIDateTime(solver.intermediateBurnTime, num / 2.0);
			TIDateTime launchTime = solver.launchTime;
			TIDateTime tidateTime3 = new TIDateTime(launchTime, this.boostDuration_s);
			TIDateTime arrivalTime = solver.arrivalTime;
			TIDateTime tidateTime4 = new TIDateTime(arrivalTime, -this.decelDuration_s);
			Trajectory_Patched.OrbitSegment orbitSegment = new Trajectory_Patched.OrbitSegment
			{
				startTime = tidateTime3,
				barycenter = commonBarycenter,
				orbit = solver.outgoingOrbit,
				isImpulse = true
			};
			Trajectory_Patched.OrbitSegment orbitSegment2 = new Trajectory_Patched.OrbitSegment
			{
				startTime = tidateTime2,
				barycenter = commonBarycenter,
				orbit = solver.incomingOrbit,
				isImpulse = true
			};
			CartesianState cartesianState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			fleet.tryToGetLocalCartesianState(launchTime, out cartesianState, out tinaturalSpaceObjectState);
			cartesianState = cartesianState.ChangeReferenceFrame(tinaturalSpaceObjectState, commonBarycenter, launchTime);
			CartesianState cartesianState2 = orbitSegment.CartesianStateAtTime(tidateTime3, commonBarycenter);
			Trajectory_Patched.BurnSegment burnSegment = new Trajectory_Patched.BurnSegment
			{
				startTime = launchTime,
				burnDuration_s = this.boostDuration_s,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = true,
				isImpulse = true,
				barycenter = commonBarycenter,
				burnDescription = new BurnBezierDescription
				{
					startPosition = cartesianState.positionDisplay,
					endPosition = cartesianState2.positionDisplay,
					startVelocityControlPoint = cartesianState.positionDisplay + cartesianState.velocityDisplay * this.boostDuration_s / 3.0,
					endVelocityControlPoint = cartesianState2.positionDisplay - cartesianState2.velocityDisplay * this.boostDuration_s / 3.0
				}
			};
			CartesianState cartesianState3 = orbitSegment2.CartesianStateAtTime(tidateTime4, commonBarycenter);
			TIOrbitState tiorbitState = destinationValue as TIOrbitState;
			CartesianState cartesianState4;
			if (tiorbitState != null)
			{
				TIDateTime tidateTime5 = new TIDateTime(base.arrivalTime, -this.decelDuration_s / 2.0);
				Vector3d position = orbitSegment2.CartesianStateAtTime(tidateTime5, commonBarycenter).position;
				double num2 = TISpaceAssetState.CalculateMeanAnomalyFromPosition(new OrbitalElementsState(tiorbitState, 0.0, tidateTime5.ExportTime()), tiorbitState.barycenter, position, tidateTime5, fleet.faction.isActivePlayer);
				cartesianState4 = tiorbitState.ToOrbitalElementsState(tidateTime5, num2).ToCartesianStateAtTime(arrivalTime.ExportTime(), tiorbitState.barycenter.mass_kg).ChangeReferenceFrame(tiorbitState.barycenter, commonBarycenter, arrivalTime);
			}
			else
			{
				cartesianState4 = Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(destinationValue, arrivalTime, commonBarycenter, fleet.faction, null, 0.0);
			}
			Trajectory_Patched.BurnSegment burnSegment2 = new Trajectory_Patched.BurnSegment
			{
				startTime = tidateTime4,
				burnDuration_s = this.decelDuration_s,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = false,
				isImpulse = true,
				barycenter = commonBarycenter,
				burnDescription = new BurnBezierDescription
				{
					startPosition = cartesianState3.positionDisplay,
					endPosition = cartesianState4.positionDisplay,
					startVelocityControlPoint = cartesianState3.positionDisplay + cartesianState3.velocityDisplay * this.decelDuration_s / 3.0,
					endVelocityControlPoint = cartesianState4.positionDisplay - cartesianState4.velocityDisplay * this.decelDuration_s / 3.0
				}
			};
			CartesianState cartesianState5 = orbitSegment.GlobalCartesianStateAtTime(tidateTime) - commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime);
			cartesianState5.position = commonBarycenter.SpatialRotation * cartesianState5.position;
			cartesianState5.velocity = commonBarycenter.SpatialRotation * cartesianState5.velocity;
			CartesianState cartesianState6 = orbitSegment2.GlobalCartesianStateAtTime(tidateTime2) - commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime2);
			cartesianState6.position = commonBarycenter.SpatialRotation * cartesianState6.position;
			cartesianState6.velocity = commonBarycenter.SpatialRotation * cartesianState6.velocity;
			Trajectory_Patched.BurnSegment burnSegment3 = new Trajectory_Patched.BurnSegment
			{
				startTime = tidateTime,
				burnDuration_s = num,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = (destinationValue.a_m() > originValue.a_m()),
				isImpulse = true,
				barycenter = commonBarycenter,
				burnDescription = new BurnBezierDescription
				{
					startPosition = cartesianState5.positionDisplay,
					endPosition = cartesianState6.positionDisplay,
					startVelocityControlPoint = cartesianState5.positionDisplay + cartesianState5.velocityDisplay * num / 3.0,
					endVelocityControlPoint = cartesianState6.positionDisplay - cartesianState6.velocityDisplay * num / 3.0
				}
			};
			List<Trajectory_Patched.BurnSegment> list = this.AdjustBurnSegmentToAvoidCollision(burnSegment3, commonBarycenter);
			this.Segments = new List<Trajectory_Patched.IPatchSegment> { burnSegment, orbitSegment };
			this.Segments.AddRange(list);
			this.Segments.Add(orbitSegment2);
			this.Segments.Add(burnSegment2);
			TIDateTime startTime = this.Segments[0].startTime;
			TIDateTime tidateTime6 = solver.arrivalTime;
			Trajectory_Patched.IPatchSegmentWithEndTime patchSegmentWithEndTime = this.Segments[this.Segments.Count - 1] as Trajectory_Patched.IPatchSegmentWithEndTime;
			if (patchSegmentWithEndTime != null)
			{
				tidateTime6 = patchSegmentWithEndTime.endTime;
			}
			this.CorrectLaunchAndArrivalTime(startTime, tidateTime6, fleet.faction);
			this.UpdateDestinationOrbitWhenTargetingFleetInMotion();
			this.boostDV_mps = this.Segments.Sum<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.boostDV_mps);
			this.decelDV_mps = this.Segments.Sum<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.decelDV_mps);
		}

		// Token: 0x060047CB RID: 18379 RVA: 0x001D7854 File Offset: 0x001D5A54
		public override void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TrajectorySolver solver, double fleetCruiseAcceleration_mps2)
		{
			PatchedTransfer patchedTransfer = solver as PatchedTransfer;
			if (commonBarycenter == null)
			{
				commonBarycenter = originValue.barycenter().FindCommonBarycenter(destination.barycenter);
				Debug.LogError("Patched transfer lacked a common barycenter.  Defaulting to " + ((commonBarycenter != null) ? commonBarycenter.displayName : null));
			}
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, patchedTransfer.launchTime, patchedTransfer.transitDuration_s, false);
			this.boostDV_mps = patchedTransfer.boost_DV_mps;
			this.decelDV_mps = patchedTransfer.decel_DV_mps;
			this.boostDuration_s = patchedTransfer.boost_DV_mps / fleetCruiseAcceleration_mps2;
			this.decelDuration_s = patchedTransfer.decel_DV_mps / fleetCruiseAcceleration_mps2;
			base.fleetCruiseAcceleration_mps2 = fleetCruiseAcceleration_mps2;
			this.Segments = new List<Trajectory_Patched.IPatchSegment>();
			TISpaceFleetState tispaceFleetState = fleet as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime <= TITimeState.Now())
			{
				Trajectory_Patched trajectory_Patched = tispaceFleetState.trajectory as Trajectory_Patched;
				if (trajectory_Patched != null)
				{
					TIDateTime launchTime = solver.launchTime;
					TIDateTime tidateTime = TITimeState.Now();
					List<Trajectory_Patched.IPatchSegment> list = new List<Trajectory_Patched.IPatchSegment>();
					foreach (Trajectory_Patched.IPatchSegment patchSegment in trajectory_Patched.Segments)
					{
						if (patchSegment.startTime < tidateTime)
						{
							list.Clear();
						}
						if (patchSegment.startTime > launchTime)
						{
							break;
						}
						list.Add(patchSegment);
					}
					list.ForEach(delegate(Trajectory_Patched.IPatchSegment x)
					{
						x.interruptible = true;
					});
					if (list.Count<Trajectory_Patched.IPatchSegment>() > 0)
					{
						Trajectory_Patched.ISupportsReducedCopy supportsReducedCopy = list[0] as Trajectory_Patched.ISupportsReducedCopy;
						if (supportsReducedCopy != null)
						{
							list[0] = supportsReducedCopy.ReducedCopy(tidateTime, supportsReducedCopy.endTime);
						}
						int num = list.Count<Trajectory_Patched.IPatchSegment>() - 1;
						Trajectory_Patched.ISupportsReducedCopy supportsReducedCopy2 = list[num] as Trajectory_Patched.ISupportsReducedCopy;
						if (supportsReducedCopy2 != null && supportsReducedCopy2.endTime >= launchTime)
						{
							list[num] = supportsReducedCopy2.ReducedCopy(supportsReducedCopy2.startTime, launchTime);
						}
					}
					this.Segments.AddRange(list);
					if (trajectory_Patched.arrivalTime < launchTime)
					{
						OrbitalElementsState orbitalElementsAtTime = trajectory_Patched.GetOrbitalElementsAtTime(trajectory_Patched.arrivalTime);
						TINaturalSpaceObjectState barycenterAtTime = trajectory_Patched.GetBarycenterAtTime(trajectory_Patched.arrivalTime);
						if (orbitalElementsAtTime.eccentricity < 1.0)
						{
							this.Segments.Add(new Trajectory_Patched.OrbitSegment
							{
								startTime = trajectory_Patched.arrivalTime,
								barycenter = barycenterAtTime,
								orbit = orbitalElementsAtTime,
								interruptible = true
							});
						}
						else
						{
							this.Segments.Add(new Trajectory_Patched.HyperbolicOrbitSegment
							{
								startTime = trajectory_Patched.arrivalTime,
								barycenter = barycenterAtTime,
								orbit = orbitalElementsAtTime,
								interruptible = true
							});
						}
					}
					base.launchTime = tidateTime;
				}
			}
			TIDateTime tidateTime2 = TITimeState.Now();
			for (int i = 0; i < patchedTransfer.transferSegments.Count; i++)
			{
				IPatchedTransferSegment patchedTransferSegment = patchedTransfer.transferSegments[i];
				MicrothrustTransferSegment microthrustTransferSegment = patchedTransferSegment as MicrothrustTransferSegment;
				if (microthrustTransferSegment != null)
				{
					Trajectory_Patched.MicrothrustSegment microthrustSegment = this.CreateMicrothrustSegment(microthrustTransferSegment, tidateTime2);
					if (microthrustSegment != null)
					{
						this.Segments.Add(microthrustSegment);
					}
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP = patchedTransferSegment as MicrothrustTransferSegmentLERP;
					if (microthrustTransferSegmentLERP != null)
					{
						Trajectory_Patched.MicrothrustLERPSegment microthrustLERPSegment = this.CreateMicrothrustLERPSegment(microthrustTransferSegmentLERP, tidateTime2);
						if (microthrustLERPSegment != null)
						{
							this.Segments.Add(microthrustLERPSegment);
						}
					}
					else
					{
						ImpulseTransferSegment impulseTransferSegment = patchedTransferSegment as ImpulseTransferSegment;
						if (impulseTransferSegment != null)
						{
							ValueTuple<TIDateTime, TIDateTime> burnTimeBounds = this.GetBurnTimeBounds(patchedTransfer, i, destinationValue);
							List<Trajectory_Patched.IPatchSegment> list2 = this.CreateImpulseSegments(impulseTransferSegment, patchedTransfer.transferSegments, ref i, originValue, destinationValue, ref tidateTime2, burnTimeBounds);
							if (list2 != null)
							{
								this.Segments.AddRange(list2);
							}
						}
						else
						{
							BurnTransferSegment burnTransferSegment = patchedTransferSegment as BurnTransferSegment;
							if (burnTransferSegment != null)
							{
								List<Trajectory_Patched.IPatchSegment> list3 = this.CreateBurnSegments(burnTransferSegment, patchedTransfer.transferSegments, ref i, originValue, destinationValue, ref tidateTime2);
								if (list3 != null)
								{
									this.Segments.AddRange(list3);
								}
							}
							else
							{
								ThreeImpulseTransferSegment threeImpulseTransferSegment = patchedTransferSegment as ThreeImpulseTransferSegment;
								if (threeImpulseTransferSegment != null)
								{
									ValueTuple<TIDateTime, TIDateTime> burnTimeBounds2 = this.GetBurnTimeBounds(patchedTransfer, i, destinationValue);
									List<Trajectory_Patched.IPatchSegment> list4 = this.CreateThreeImpulseTransferSegments(threeImpulseTransferSegment, patchedTransfer.transferSegments, ref i, originValue, destinationValue, ref tidateTime2, burnTimeBounds2);
									if (list4 != null)
									{
										this.Segments.AddRange(list4);
									}
								}
								else
								{
									TorchTransferSegment torchTransferSegment = patchedTransferSegment as TorchTransferSegment;
									if (torchTransferSegment != null)
									{
										Trajectory_Patched.IPatchSegment patchSegment2 = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>();
										Vector3d vector3d;
										if (patchSegment2 == null)
										{
											vector3d = fleet.GetGlobalPositionAtTime(patchedTransferSegment.startTime);
										}
										else
										{
											vector3d = patchSegment2.GlobalPositionAtTime(patchedTransferSegment.startTime);
										}
										Trajectory_Patched.MicrothrustSegment microthrustSegment2 = null;
										if (i + 1 < patchedTransfer.transferSegments.Count)
										{
											MicrothrustTransferSegment microthrustTransferSegment2 = patchedTransfer.transferSegments[i + 1] as MicrothrustTransferSegment;
											microthrustSegment2 = this.CreateMicrothrustSegment(microthrustTransferSegment2, torchTransferSegment.endTime);
										}
										List<Trajectory_Patched.IPatchSegment> list5 = this.CreateTorchSegments(torchTransferSegment, vector3d, microthrustSegment2, destinationValue, originValue.barycenter());
										if (list5 != null)
										{
											this.Segments.AddRange(list5);
											if (microthrustSegment2 != null)
											{
												this.Segments.Add(microthrustSegment2);
												i++;
											}
										}
									}
								}
							}
						}
					}
				}
				base.duration = TimeSpan.FromSeconds(base.arrivalTime.DifferenceInSeconds(TITimeState.Now()));
			}
			TIDateTime tidateTime3 = this.Segments[this.Segments.Count - 1].startTime;
			for (int j = this.Segments.Count - 2; j >= 0; j--)
			{
				if (this.Segments[j].startTime >= tidateTime3)
				{
					this.Segments.RemoveAt(j);
				}
				else
				{
					tidateTime3 = this.Segments[j].startTime;
				}
			}
			List<int> list6 = new List<int>();
			TIDateTime tidateTime4 = this.Segments[0].startTime;
			for (int k = 0; k < this.Segments.Count; k++)
			{
				TIDateTime tidateTime5 = base.arrivalTime;
				if (k < this.Segments.Count - 1)
				{
					tidateTime5 = this.Segments[k + 1].startTime;
				}
				Trajectory_Patched.BurnSegment burnSegment = this.Segments[k] as Trajectory_Patched.BurnSegment;
				if (burnSegment != null)
				{
					if (tidateTime4 >= burnSegment.endTime)
					{
						list6.Add(k);
					}
					else if (!burnSegment.interruptible)
					{
						tidateTime4 = burnSegment.endTime;
					}
				}
				else if (tidateTime4 >= tidateTime5)
				{
					list6.Add(k);
				}
			}
			if (list6.Count == this.Segments.Count)
			{
				Log.Error("Trajectory_Patched: attempting to remove all segments due to previous segments ending after they end, or the segment having zero duration.  Trajectories without segments would fail, so we won't delete any.", Array.Empty<object>());
			}
			else
			{
				for (int l = list6.Count - 1; l >= 0; l--)
				{
					this.Segments.RemoveAt(list6[l]);
				}
			}
			TIDateTime tidateTime6 = patchedTransfer.arrivalTime;
			Trajectory_Patched.MicrothrustSegment microthrustSegment3 = this.Segments[this.Segments.Count - 1] as Trajectory_Patched.MicrothrustSegment;
			if (microthrustSegment3 != null && base.destinationFleet != null && MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(base.destinationFleet, fleet.faction) && base.destinationFleet.trajectory.arrivalTime > microthrustSegment3.endTime && base.destinationFleet.trajectory.destination.ref_orbit != null)
			{
				Trajectory_Patched.OrbitSegment orbitSegment = new Trajectory_Patched.OrbitSegment
				{
					startTime = microthrustSegment3.endTime,
					barycenter = microthrustSegment3.barycenter,
					orbit = base.destinationFleet.trajectory.destination.ref_orbit.ToOrbitalElementsState(base.destinationFleet.trajectory.arrivalTime, base.destinationFleet.trajectory.getDestinationMeanAnomalyAtArrival())
				};
				this.Segments.Add(orbitSegment);
				tidateTime6 = base.destinationFleet.trajectory.arrivalTime;
			}
			TIDateTime startTime = this.Segments[0].startTime;
			Trajectory_Patched.IPatchSegmentWithEndTime patchSegmentWithEndTime = this.Segments[this.Segments.Count - 1] as Trajectory_Patched.IPatchSegmentWithEndTime;
			if (patchSegmentWithEndTime != null)
			{
				tidateTime6 = patchSegmentWithEndTime.endTime;
			}
			this.CorrectLaunchAndArrivalTime(startTime, tidateTime6, fleet.faction);
			this.UpdateDestinationOrbitWhenTargetingFleetInMotion();
			this.boostDV_mps = this.Segments.Sum<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.boostDV_mps);
			this.decelDV_mps = this.Segments.Sum<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.decelDV_mps);
		}

		// Token: 0x060047CC RID: 18380 RVA: 0x001D80CC File Offset: 0x001D62CC
		public void RecalculateCommonBarycenter()
		{
			base.commonBarycenter = this.Segments.Aggregate(this.Segments[0].barycenter, (TINaturalSpaceObjectState a, Trajectory_Patched.IPatchSegment b) => a.FindCommonBarycenter(b.barycenter));
		}

		// Token: 0x060047CD RID: 18381 RVA: 0x001D811C File Offset: 0x001D631C
		private void CorrectLaunchAndArrivalTime(TIDateTime newLaunchTime, TIDateTime newArrivalTime, TIFactionState ourFaction)
		{
			base.launchTime = new TIDateTime(newLaunchTime);
			this.loiterDuration_s = base.launchTime.DifferenceInSeconds(base.assignedTime);
			base.arrivalTime = new TIDateTime(newArrivalTime);
			base.launchPosition = base.fleet.GetGlobalPositionAtTime(base.launchTime);
			base.destinationPosition = base.DestinationPositionAtTime(base.arrivalTime, ourFaction);
		}

		// Token: 0x060047CE RID: 18382 RVA: 0x001D8182 File Offset: 0x001D6382
		private double OrbitPeriod_s(double a, double mu)
		{
			return 6.283185307179586 * Mathd.Sqrt(a * a * a / mu);
		}

		// Token: 0x060047CF RID: 18383 RVA: 0x001D819C File Offset: 0x001D639C
		[return: TupleElementNames(new string[] { "earliestBurnStartTime", "latestBurnEndTime" })]
		private ValueTuple<TIDateTime, TIDateTime> GetBurnTimeBounds(PatchedTransfer patchedSolver, int segmentIndex, ITransferTarget destinationValue)
		{
			TIDateTime tidateTime = null;
			TIDateTime tidateTime2 = null;
			IPatchedTransferSegment patchedTransferSegment = patchedSolver.transferSegments[segmentIndex];
			if (segmentIndex == 0)
			{
				OrbitalElementsState orbitalElementsState;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				bool flag;
				base.fleet.getOrbitalElementsState(patchedTransferSegment.startTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
				if (orbitalElementsState.eccentricity < 1.0 && orbitalElementsState.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg) < 315569240.0)
				{
					tidateTime = new TIDateTime(patchedTransferSegment.startTime, -orbitalElementsState.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg) / 4.0);
				}
			}
			else
			{
				IPatchedTransferSegment patchedTransferSegment2 = patchedSolver.transferSegments[segmentIndex - 1];
				bool flag2 = false;
				MicrothrustTransferSegment microthrustTransferSegment = patchedTransferSegment2 as MicrothrustTransferSegment;
				if (microthrustTransferSegment != null)
				{
					flag2 = true;
					double mu = microthrustTransferSegment.barycenter.mu;
					double endRadius_m = microthrustTransferSegment.endRadius_m;
					tidateTime = new TIDateTime(patchedTransferSegment2.endTime, -this.OrbitPeriod_s(endRadius_m, mu) / 4.0);
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP = patchedTransferSegment2 as MicrothrustTransferSegmentLERP;
					if (microthrustTransferSegmentLERP != null)
					{
						flag2 = true;
						double mu2 = microthrustTransferSegmentLERP.barycenter.mu;
						double radius_m = microthrustTransferSegmentLERP.end.radius_m;
						tidateTime = new TIDateTime(patchedTransferSegment2.endTime, -this.OrbitPeriod_s(radius_m, mu2) / 4.0);
					}
					else
					{
						tidateTime = new TIDateTime(patchedTransferSegment2.endTime);
					}
				}
				if (flag2 && segmentIndex >= 2)
				{
					IPatchedTransferSegment patchedTransferSegment3 = patchedSolver.transferSegments[segmentIndex - 2];
					MicrothrustTransferSegment microthrustTransferSegment2 = patchedTransferSegment3 as MicrothrustTransferSegment;
					if (microthrustTransferSegment2 != null)
					{
						double mu3 = microthrustTransferSegment2.barycenter.mu;
						double endRadius_m2 = microthrustTransferSegment2.endRadius_m;
						tidateTime = TIDateTime.Max(tidateTime, new TIDateTime(patchedTransferSegment3.endTime, -this.OrbitPeriod_s(endRadius_m2, mu3) / 4.0));
					}
					else
					{
						MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP2 = patchedTransferSegment3 as MicrothrustTransferSegmentLERP;
						if (microthrustTransferSegmentLERP2 != null)
						{
							double mu4 = microthrustTransferSegmentLERP2.barycenter.mu;
							double radius_m2 = microthrustTransferSegmentLERP2.end.radius_m;
							tidateTime = TIDateTime.Max(tidateTime, new TIDateTime(patchedTransferSegment3.endTime, -this.OrbitPeriod_s(radius_m2, mu4) / 4.0));
						}
						else
						{
							tidateTime = TIDateTime.Max(tidateTime, new TIDateTime(patchedTransferSegment3.endTime));
						}
					}
				}
			}
			if (tidateTime == null)
			{
				tidateTime = TITimeState.Now();
			}
			else
			{
				tidateTime = TIDateTime.Max(tidateTime, TITimeState.Now());
			}
			if (segmentIndex >= patchedSolver.transferSegments.Count - 1)
			{
				OrbitalElementsState orbitalElementsState2;
				TINaturalSpaceObjectState tinaturalSpaceObjectState2;
				if (base.destinationFleet != null && base.destinationFleet.transferAssigned && base.destinationFleet.trajectory.launchTime < patchedTransferSegment.endTime && base.destinationFleet.trajectory.launchTime >= TITimeState.Now())
				{
					bool flag;
					base.destinationFleet.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag);
				}
				else
				{
					bool flag;
					destinationValue.getOrbitalElementsState(patchedTransferSegment.endTime, out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag);
				}
				if (!tinaturalSpaceObjectState2.isLagrangePointState && orbitalElementsState2.eccentricity < 1.0)
				{
					double num = orbitalElementsState2.OrbitalPeriod(tinaturalSpaceObjectState2.mass_kg);
					if (!double.IsNaN(num) && !double.IsInfinity(num) && num < 631138480.0)
					{
						tidateTime2 = new TIDateTime(patchedTransferSegment.endTime, num / 4.0);
					}
				}
			}
			else
			{
				IPatchedTransferSegment patchedTransferSegment4 = patchedSolver.transferSegments[segmentIndex + 1];
				bool flag3 = false;
				MicrothrustTransferSegment microthrustTransferSegment3 = patchedTransferSegment4 as MicrothrustTransferSegment;
				if (microthrustTransferSegment3 != null)
				{
					flag3 = true;
					double mu5 = microthrustTransferSegment3.barycenter.mu;
					double startRadius_m = microthrustTransferSegment3.startRadius_m;
					tidateTime2 = new TIDateTime(patchedTransferSegment4.startTime, this.OrbitPeriod_s(startRadius_m, mu5) / 4.0);
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP3 = patchedTransferSegment4 as MicrothrustTransferSegmentLERP;
					if (microthrustTransferSegmentLERP3 != null)
					{
						flag3 = true;
						double mu6 = microthrustTransferSegmentLERP3.barycenter.mu;
						double radius_m3 = microthrustTransferSegmentLERP3.start.radius_m;
						tidateTime2 = new TIDateTime(patchedTransferSegment4.startTime, this.OrbitPeriod_s(radius_m3, mu6) / 4.0);
					}
					else
					{
						tidateTime2 = new TIDateTime(patchedTransferSegment4.startTime);
					}
				}
				if (flag3 && segmentIndex < patchedSolver.transferSegments.Count - 2)
				{
					IPatchedTransferSegment patchedTransferSegment5 = patchedSolver.transferSegments[segmentIndex + 2];
					MicrothrustTransferSegment microthrustTransferSegment4 = patchedTransferSegment5 as MicrothrustTransferSegment;
					if (microthrustTransferSegment4 != null)
					{
						double mu7 = microthrustTransferSegment4.barycenter.mu;
						double startRadius_m2 = microthrustTransferSegment4.startRadius_m;
						tidateTime2 = TIDateTime.Min(tidateTime2, new TIDateTime(patchedTransferSegment5.startTime, this.OrbitPeriod_s(startRadius_m2, mu7) / 4.0));
					}
					else
					{
						MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP4 = patchedTransferSegment5 as MicrothrustTransferSegmentLERP;
						if (microthrustTransferSegmentLERP4 != null)
						{
							double mu8 = microthrustTransferSegmentLERP4.barycenter.mu;
							double radius_m4 = microthrustTransferSegmentLERP4.start.radius_m;
							tidateTime2 = TIDateTime.Min(tidateTime2, new TIDateTime(patchedTransferSegment5.startTime, this.OrbitPeriod_s(radius_m4, mu8) / 4.0));
						}
						else
						{
							tidateTime2 = TIDateTime.Min(tidateTime2, new TIDateTime(patchedTransferSegment5.startTime));
						}
					}
				}
			}
			return new ValueTuple<TIDateTime, TIDateTime>(tidateTime, tidateTime2);
		}

		// Token: 0x060047D0 RID: 18384 RVA: 0x001D867C File Offset: 0x001D687C
		[return: TupleElementNames(new string[] { "beforeOrbitPeriod_s", "afterOrbitPeriod_s" })]
		private ValueTuple<double, double> GetOrbitPeriodsBeforeAndAfterSegment(PatchedTransfer patchedSolver, int segmentIndex, ITransferTarget destinationValue)
		{
			IPatchedTransferSegment patchedTransferSegment = patchedSolver.transferSegments[segmentIndex];
			double num = double.PositiveInfinity;
			if (segmentIndex == 0)
			{
				OrbitalElementsState orbitalElementsState;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				bool flag;
				base.fleet.getOrbitalElementsState(patchedTransferSegment.startTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
				if (orbitalElementsState.eccentricity < 1.0)
				{
					num = orbitalElementsState.OrbitalPeriod(tinaturalSpaceObjectState.mass_kg);
				}
			}
			else
			{
				MicrothrustTransferSegment microthrustTransferSegment = patchedSolver.transferSegments[segmentIndex - 1] as MicrothrustTransferSegment;
				if (microthrustTransferSegment != null)
				{
					double mu = microthrustTransferSegment.barycenter.mu;
					double endRadius_m = microthrustTransferSegment.endRadius_m;
					num = 6.283185307179586 * Mathd.Sqrt(endRadius_m * endRadius_m * endRadius_m / mu);
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP = patchedSolver.transferSegments[segmentIndex - 1] as MicrothrustTransferSegmentLERP;
					if (microthrustTransferSegmentLERP != null)
					{
						double mu2 = microthrustTransferSegmentLERP.barycenter.mu;
						double radius_m = microthrustTransferSegmentLERP.end.radius_m;
						num = 6.283185307179586 * Mathd.Sqrt(radius_m * radius_m * radius_m / mu2);
					}
				}
			}
			double num2 = double.PositiveInfinity;
			if (segmentIndex == patchedSolver.transferSegments.Count - 1)
			{
				if (base.destinationFleet != null && base.destinationFleet.transferAssigned && base.destinationFleet.trajectory.launchTime < patchedTransferSegment.endTime && base.destinationFleet.trajectory.launchTime >= TITimeState.Now())
				{
					bool flag;
					OrbitalElementsState orbitalElementsState2;
					TINaturalSpaceObjectState tinaturalSpaceObjectState2;
					base.destinationFleet.getOrbitalElementsState(TITimeState.Now(), out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag);
					if (orbitalElementsState2.eccentricity < 1.0)
					{
						num2 = orbitalElementsState2.OrbitalPeriod(tinaturalSpaceObjectState2.mass_kg);
					}
				}
				else
				{
					bool flag;
					OrbitalElementsState orbitalElementsState3;
					TINaturalSpaceObjectState tinaturalSpaceObjectState3;
					destinationValue.getOrbitalElementsState(patchedTransferSegment.endTime, out orbitalElementsState3, out tinaturalSpaceObjectState3, out flag);
					if (orbitalElementsState3.eccentricity < 1.0)
					{
						num2 = orbitalElementsState3.OrbitalPeriod(tinaturalSpaceObjectState3.mass_kg);
					}
				}
			}
			else
			{
				MicrothrustTransferSegment microthrustTransferSegment2 = patchedSolver.transferSegments[segmentIndex + 1] as MicrothrustTransferSegment;
				if (microthrustTransferSegment2 != null)
				{
					double mu3 = microthrustTransferSegment2.barycenter.mu;
					double startRadius_m = microthrustTransferSegment2.startRadius_m;
					num2 = 6.283185307179586 * Mathd.Sqrt(startRadius_m * startRadius_m * startRadius_m / mu3);
				}
				else
				{
					MicrothrustTransferSegmentLERP microthrustTransferSegmentLERP2 = patchedSolver.transferSegments[segmentIndex + 1] as MicrothrustTransferSegmentLERP;
					if (microthrustTransferSegmentLERP2 != null)
					{
						double mu4 = microthrustTransferSegmentLERP2.barycenter.mu;
						double radius_m2 = microthrustTransferSegmentLERP2.start.radius_m;
						num2 = 6.283185307179586 * Mathd.Sqrt(radius_m2 * radius_m2 * radius_m2 / mu4);
					}
				}
			}
			return new ValueTuple<double, double>(num, num2);
		}

		// Token: 0x060047D1 RID: 18385 RVA: 0x001D8914 File Offset: 0x001D6B14
		private void UpdateDestinationOrbitWhenTargetingFleetInMotion()
		{
			TISpaceGameState destination = base.destination;
			TISpaceFleetState tispaceFleetState = ((destination != null) ? destination.ref_fleet : null);
			if (tispaceFleetState != null && tispaceFleetState.trajectory != null && tispaceFleetState.trajectory.arrivalTime < base.arrivalTime)
			{
				base.destinationOrbit = tispaceFleetState.trajectory.destinationOrbit;
			}
		}

		// Token: 0x060047D2 RID: 18386 RVA: 0x001D8970 File Offset: 0x001D6B70
		private List<Trajectory_Patched.MicrothrustSegment> CreateMicrothrustSegmentsForOrbitPhasing(OrbitalElementsState orbitAtTerminalTime, TINaturalSpaceObjectState barycenterAtTerminalTime, TIDateTime terminalTime, TINaturalSpaceObjectState commonBarycenter, double fleetAcceleration_mps2, bool isGoingOut, out CartesianState cartesianStateWithRespectToCommonBarycenter, out TIDateTime centralEndOfMicrothrustTime)
		{
			List<Trajectory_Patched.MicrothrustSegment> list = new List<Trajectory_Patched.MicrothrustSegment>();
			bool flag = false;
			if (barycenterAtTerminalTime == commonBarycenter)
			{
				flag = true;
			}
			if (barycenterAtTerminalTime.barycenter != commonBarycenter)
			{
				TINaturalSpaceObjectState barycenter = barycenterAtTerminalTime.barycenter;
				if (((barycenter != null) ? barycenter.barycenter : null) != commonBarycenter)
				{
					Log.Error("Common barycenter is not common.", Array.Empty<object>());
					flag = true;
				}
			}
			if (orbitAtTerminalTime.semiMajorAxis_m <= 0.0)
			{
				Log.Error("Hyperbolic microthrust attempted.", Array.Empty<object>());
				flag = true;
			}
			if (flag)
			{
				centralEndOfMicrothrustTime = terminalTime;
				cartesianStateWithRespectToCommonBarycenter = orbitAtTerminalTime.ToCartesianStateAtTime(terminalTime.ExportTime(), barycenterAtTerminalTime.mass_kg).ChangeReferenceFrame(barycenterAtTerminalTime, commonBarycenter, terminalTime);
				return list;
			}
			MicrothrustSphere microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, barycenterAtTerminalTime.mu, barycenterAtTerminalTime.sphereOfInfluence_m);
			CartesianState cartesianState;
			if (orbitAtTerminalTime.semiMajorAxis_m < microthrustSphere.Radius_m)
			{
				double num = Mathd.Sqrt(barycenterAtTerminalTime.mu / orbitAtTerminalTime.semiMajorAxis_m);
				TIDateTime tidateTime = new TIDateTime(terminalTime);
				TIDateTime tidateTime2 = new TIDateTime(terminalTime, microthrustSphere.GetDuration_s(num) * (double)(isGoingOut ? 1 : (-1)));
				Trajectory_Patched.MicrothrustSegment microthrustSegment = new Trajectory_Patched.MicrothrustSegment
				{
					startTime = (isGoingOut ? tidateTime : tidateTime2),
					endTime = (isGoingOut ? tidateTime2 : tidateTime),
					epochTime = new TIDateTime(terminalTime),
					barycenter = barycenterAtTerminalTime,
					eccentricity = orbitAtTerminalTime.eccentricity,
					ascendingNode_rad = orbitAtTerminalTime.longAscendingNode_Rad,
					inclination_rad = orbitAtTerminalTime.inclination_Rad,
					argP_rad = orbitAtTerminalTime.argPeriapsis_Rad,
					initialVelocity_mps = num,
					initialMeanAnomaly_rad = orbitAtTerminalTime.MeanAnomalyAtTime_Rad(terminalTime.ExportTime(), barycenterAtTerminalTime.mu),
					fleetCruiseAcceleration_mps2 = (isGoingOut ? base.fleetCruiseAcceleration_mps2 : (-base.fleetCruiseAcceleration_mps2))
				};
				list.Add(microthrustSegment);
				terminalTime = tidateTime2;
				cartesianState = microthrustSegment.CartesianStateAtTime(terminalTime, barycenterAtTerminalTime);
				cartesianState.velocity = new Vector3d(0f, 0f, 0f);
				PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState, barycenterAtTerminalTime, terminalTime);
				barycenterAtTerminalTime = barycenterAtTerminalTime.barycenter;
			}
			else
			{
				cartesianState = orbitAtTerminalTime.ToCartesianStateAtTime(terminalTime.ExportTime(), barycenterAtTerminalTime.mass_kg);
				PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState, barycenterAtTerminalTime, terminalTime);
				barycenterAtTerminalTime = barycenterAtTerminalTime.barycenter;
			}
			if (barycenterAtTerminalTime != commonBarycenter)
			{
				microthrustSphere = new MicrothrustSphere(fleetAcceleration_mps2, barycenterAtTerminalTime.mu, barycenterAtTerminalTime.sphereOfInfluence_m);
				double magnitude = cartesianState.position.magnitude;
				if (magnitude < microthrustSphere.Radius_m)
				{
					double num2 = Mathd.Sqrt(barycenterAtTerminalTime.mu / magnitude);
					TIDateTime tidateTime3 = new TIDateTime(terminalTime);
					TIDateTime tidateTime4 = new TIDateTime(terminalTime, microthrustSphere.GetDuration_s(num2) * (double)(isGoingOut ? 1 : (-1)));
					Trajectory_Patched.MicrothrustSegment microthrustSegment2 = new Trajectory_Patched.MicrothrustSegment
					{
						startTime = (isGoingOut ? tidateTime3 : tidateTime4),
						endTime = (isGoingOut ? tidateTime4 : tidateTime3),
						epochTime = new TIDateTime(terminalTime),
						barycenter = barycenterAtTerminalTime,
						eccentricity = orbitAtTerminalTime.eccentricity,
						ascendingNode_rad = orbitAtTerminalTime.longAscendingNode_Rad,
						inclination_rad = orbitAtTerminalTime.inclination_Rad,
						argP_rad = orbitAtTerminalTime.argPeriapsis_Rad,
						initialVelocity_mps = num2,
						initialMeanAnomaly_rad = orbitAtTerminalTime.MeanAnomalyAtTime_Rad(terminalTime.ExportTime(), barycenterAtTerminalTime.mu),
						fleetCruiseAcceleration_mps2 = (isGoingOut ? base.fleetCruiseAcceleration_mps2 : (-base.fleetCruiseAcceleration_mps2))
					};
					list.Add(microthrustSegment2);
					terminalTime = tidateTime4;
					cartesianState = microthrustSegment2.CartesianStateAtTime(terminalTime, barycenterAtTerminalTime);
					cartesianState.velocity = new Vector3d(0f, 0f, 0f);
					PatchedTransfer.MoveCartesianStateOutOneBarycenter(ref cartesianState, barycenterAtTerminalTime, terminalTime);
					barycenterAtTerminalTime = barycenterAtTerminalTime.barycenter;
				}
			}
			if (barycenterAtTerminalTime != commonBarycenter)
			{
				Log.Error("Trajectory_Patched: orbit phasing: CreateMicrothrustSegments(): after raising the barycenter twice, we haven't reached the common barycenter.  Should not be possible in our solar system.", Array.Empty<object>());
			}
			cartesianStateWithRespectToCommonBarycenter = cartesianState;
			centralEndOfMicrothrustTime = terminalTime;
			return list;
		}

		// Token: 0x060047D3 RID: 18387 RVA: 0x001D8D00 File Offset: 0x001D6F00
		private Trajectory_Patched.MicrothrustSegment CreateMicrothrustSegment(MicrothrustTransferSegment segment, TIDateTime prevBurnEndTime)
		{
			if (prevBurnEndTime > segment.endTime)
			{
				return null;
			}
			bool flag = segment.startRadius_m < segment.endRadius_m;
			Trajectory_Patched.MicrothrustSegment microthrustSegment = new Trajectory_Patched.MicrothrustSegment
			{
				startTime = segment.startTime,
				endTime = segment.endTime,
				epochTime = segment.startTime,
				barycenter = segment.barycenter,
				eccentricity = segment.eccentricity,
				ascendingNode_rad = segment.ascendingNode_rad,
				inclination_rad = segment.inclination_rad,
				argP_rad = segment.argP_rad,
				initialVelocity_mps = Mathd.Sqrt(segment.barycenter.mu / segment.startRadius_m),
				initialMeanAnomaly_rad = segment.startAnomaly_Rad,
				fleetCruiseAcceleration_mps2 = (flag ? base.fleetCruiseAcceleration_mps2 : (-base.fleetCruiseAcceleration_mps2)),
				interruptible = true
			};
			if (prevBurnEndTime > microthrustSegment.startTime)
			{
				microthrustSegment.startTime = prevBurnEndTime;
			}
			return microthrustSegment;
		}

		// Token: 0x060047D4 RID: 18388 RVA: 0x001D8DF0 File Offset: 0x001D6FF0
		private Trajectory_Patched.MicrothrustLERPSegment CreateMicrothrustLERPSegment(MicrothrustTransferSegmentLERP segment, TIDateTime prevBurnEndTime)
		{
			if (prevBurnEndTime > segment.endTime)
			{
				return null;
			}
			bool flag = segment.start.radius_m < segment.end.radius_m;
			double num = segment.endTime.DifferenceInSeconds(segment.startTime);
			double num2 = ((segment.effectiveFleetAcceleration_mps2 == 0.0) ? base.fleetCruiseAcceleration_mps2 : segment.effectiveFleetAcceleration_mps2);
			double num3 = ((segment.trueFleetAcceleration_mps2 == 0.0) ? base.fleetCruiseAcceleration_mps2 : segment.trueFleetAcceleration_mps2);
			Trajectory_Patched.MicrothrustLERPSegment microthrustLERPSegment = new Trajectory_Patched.MicrothrustLERPSegment
			{
				startTime = segment.startTime,
				endTime = segment.endTime,
				epochTime = segment.startTime,
				barycenter = segment.barycenter,
				eccentricity = segment.start.eccentricity,
				endEccentricity = segment.end.eccentricity,
				ascendingNode_rad = segment.start.ascendingNode_Rad,
				endAscendingNode_rad = segment.end.ascendingNode_Rad,
				inclination_rad = segment.start.inclination_Rad,
				endInclination_rad = segment.end.inclination_Rad,
				argP_rad = segment.start.argPeriapsis_Rad,
				endArgP_rad = segment.end.argPeriapsis_Rad,
				initialVelocity_mps = Mathd.Sqrt(segment.barycenter.mu / segment.start.radius_m),
				initialMeanAnomaly_rad = segment.start.meanAnomaly_Rad,
				fleetCruiseAcceleration_mps2 = (flag ? num2 : (-num2)),
				trueFleetAccleration_mps2 = num3,
				startRadiusCorrection_m = segment.start.radiusCorrection_m,
				endRadiusCorrection_m = segment.end.radiusCorrection_m,
				startAnomalyCorrection_rad = segment.start.meanAnomalyCorrection_Rad,
				endAnomalyCorrection_rad = segment.end.meanAnomalyCorrection_Rad,
				startAnomalySpeedCorrectionControlPoint_rad = segment.start.meanAnomalySpeedCorrection_RadPerSec * num / 3.0,
				endAnomalySpeedCorrectionControlPoint_rad = segment.end.meanAnomalySpeedCorrection_RadPerSec * num / 3.0,
				interruptible = true
			};
			if (prevBurnEndTime > microthrustLERPSegment.startTime)
			{
				microthrustLERPSegment.startTime = prevBurnEndTime;
			}
			return microthrustLERPSegment;
		}

		// Token: 0x060047D5 RID: 18389 RVA: 0x001D9020 File Offset: 0x001D7220
		[return: TupleElementNames(new string[] { "newEarliestStartTime", "additionalBurnDuration_s" })]
		private ValueTuple<TIDateTime, double> EarliestBurnStartTimeGivenExistingSegments(TIDateTime earliestStartTime)
		{
			double num = 0.0;
			int i = this.Segments.Count<Trajectory_Patched.IPatchSegment>() - 1;
			while (i >= 0)
			{
				if (this.Segments[i].interruptible)
				{
					Trajectory_Patched.IPatchSegmentWithEndTime patchSegmentWithEndTime = this.Segments[i] as Trajectory_Patched.IPatchSegmentWithEndTime;
					if (patchSegmentWithEndTime != null && patchSegmentWithEndTime.DV_mps > 0.0)
					{
						num += patchSegmentWithEndTime.endTime.DifferenceInSeconds(TIDateTime.Max(patchSegmentWithEndTime.startTime, earliestStartTime));
					}
					if (this.Segments[i].startTime <= earliestStartTime)
					{
						return new ValueTuple<TIDateTime, double>(earliestStartTime, num);
					}
					i--;
				}
				else
				{
					Trajectory_Patched.IPatchSegmentWithEndTime patchSegmentWithEndTime2 = this.Segments[i] as Trajectory_Patched.IPatchSegmentWithEndTime;
					if (patchSegmentWithEndTime2 != null)
					{
						return new ValueTuple<TIDateTime, double>(TIDateTime.Max(patchSegmentWithEndTime2.endTime, earliestStartTime), num);
					}
					return new ValueTuple<TIDateTime, double>(TIDateTime.Max(this.Segments[i].startTime, earliestStartTime), num);
				}
			}
			return new ValueTuple<TIDateTime, double>(earliestStartTime, num);
		}

		// Token: 0x060047D6 RID: 18390 RVA: 0x001D9118 File Offset: 0x001D7318
		private List<Trajectory_Patched.IPatchSegment> CreateImpulseSegments(ImpulseTransferSegment segment, List<IPatchedTransferSegment> rawSegments, ref int segmentIndex, ITransferTarget originValue, ITransferTarget destinationValue, ref TIDateTime prevBurnEndTime, [TupleElementNames(new string[] { "earliestBurnTime", "latestBurnTime" })] ValueTuple<TIDateTime, TIDateTime> burnTimeBounds)
		{
			TINaturalSpaceObjectState barycenter = segment.barycenter;
			double boost_DV_mps = segment.lambert.boost_DV_mps;
			double decel_DV_mps = segment.lambert.decel_DV_mps;
			double num = boost_DV_mps / base.fleetCruiseAcceleration_mps2;
			double num2 = decel_DV_mps / base.fleetCruiseAcceleration_mps2;
			TIDateTime boostStartTime = ((burnTimeBounds.Item1 == null) ? new TIDateTime(segment.startTime) : TIDateTime.Max(new TIDateTime(segment.startTime), burnTimeBounds.Item1));
			this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.interruptible && x.startTime < boostStartTime);
			double item = this.EarliestBurnStartTimeGivenExistingSegments(boostStartTime).Item2;
			num += item;
			TIDateTime tidateTime = ((burnTimeBounds.Item2 == null) ? new TIDateTime(segment.endTime) : TIDateTime.Min(new TIDateTime(segment.endTime), burnTimeBounds.Item2));
			bool flag = false;
			IPatchedTransferSegment patchedTransferSegment = null;
			foreach (IPatchedTransferSegment patchedTransferSegment2 in rawSegments)
			{
				if (patchedTransferSegment2 == segment)
				{
					flag = true;
				}
				else if (flag)
				{
					patchedTransferSegment = patchedTransferSegment2;
					break;
				}
			}
			if ((patchedTransferSegment is MicrothrustTransferSegment || patchedTransferSegment is MicrothrustTransferSegmentLERP) && patchedTransferSegment.startTime < tidateTime)
			{
				num2 += TIDateTime.Min(tidateTime, patchedTransferSegment.endTime).DifferenceInSeconds(patchedTransferSegment.startTime);
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.FindLast((Trajectory_Patched.IPatchSegment x) => x.startTime < boostStartTime);
			CartesianState cartesianState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (patchSegment != null)
			{
				cartesianState = patchSegment.GlobalCartesianStateAtTime(boostStartTime).ToLocal(barycenter, boostStartTime);
				tinaturalSpaceObjectState = patchSegment.barycenter;
				Trajectory_Patched.BurnSegment burnSegment = patchSegment as Trajectory_Patched.BurnSegment;
				if (burnSegment != null)
				{
					TINaturalSpaceObjectState barycenter2 = burnSegment.barycenter;
					CartesianState cartesianState2 = burnSegment.GlobalCartesianStateAtTime(burnSegment.startTime).ToLocal(barycenter2, burnSegment.startTime);
					CartesianState cartesianState3 = burnSegment.GlobalCartesianStateAtTime(boostStartTime).ToLocal(barycenter2, boostStartTime);
					double num3 = boostStartTime.DifferenceInSeconds(burnSegment.startTime);
					Trajectory_Patched.BurnSegment burnSegment2 = new Trajectory_Patched.BurnSegment
					{
						startTime = burnSegment.startTime,
						burnDuration_s = num3,
						barycenter = barycenter2,
						burnDescription = new BurnBezierDescription(cartesianState2, cartesianState3, num3)
					};
					for (int i = 0; i < this.Segments.Count; i++)
					{
						if (this.Segments[i] == patchSegment)
						{
							this.Segments[i] = burnSegment2;
						}
					}
				}
				else
				{
					Trajectory_Patched.IPatchSegmentWithEndTime patchSegmentWithEndTime = patchSegment as Trajectory_Patched.IPatchSegmentWithEndTime;
					if (patchSegmentWithEndTime != null)
					{
						patchSegmentWithEndTime.endTime = boostStartTime;
					}
				}
			}
			else
			{
				CartesianState? cartesianState4;
				cartesianState = ((base.fleet.tryToGetGlobalCartesianState(boostStartTime) != null) ? cartesianState4.GetValueOrDefault().ToLocal(barycenter, boostStartTime) : default(CartesianState));
				tinaturalSpaceObjectState = base.fleet.localBarycenter(boostStartTime);
			}
			TIDateTime tidateTime2 = new TIDateTime(boostStartTime, num);
			bool flag2 = segment.lambert.transferOrbit.eccentricity < 1.0;
			Trajectory_Patched.IPatchSegment patchSegment2;
			if (segment.lambert.transferOrbit.eccentricity < 1.0)
			{
				patchSegment2 = new Trajectory_Patched.OrbitSegment
				{
					startTime = tidateTime2,
					barycenter = barycenter,
					orbit = segment.lambert.transferOrbit,
					isImpulse = flag2
				};
			}
			else
			{
				patchSegment2 = new Trajectory_Patched.HyperbolicOrbitSegment
				{
					startTime = tidateTime2,
					barycenter = barycenter,
					orbit = segment.lambert.transferOrbit,
					isTorch = !flag2
				};
			}
			CartesianState cartesianState5 = patchSegment2.GlobalCartesianStateAtTime(tidateTime2).ToLocal(barycenter, tidateTime2);
			Trajectory_Patched.MicrothrustSegment microthrustSegment = null;
			for (int j = segmentIndex + 1; j < rawSegments.Count; j++)
			{
				if (rawSegments[j].startTime <= tidateTime)
				{
					microthrustSegment = this.CreateMicrothrustSegment(rawSegments[j] as MicrothrustTransferSegment, tidateTime);
					segmentIndex = j;
				}
			}
			CartesianState cartesianState6;
			TINaturalSpaceObjectState tinaturalSpaceObjectState2;
			if (microthrustSegment == null)
			{
				TIOrbitState tiorbitState = destinationValue as TIOrbitState;
				if (tiorbitState != null)
				{
					TIDateTime tidateTime3 = new TIDateTime(tidateTime, -num2 * 0.5);
					Vector3d position = patchSegment2.GlobalCartesianStateAtTime(tidateTime3).ToLocal(tiorbitState.barycenter, tidateTime3).position;
					OrbitalElementsState orbitalElementsState = tiorbitState.ToOrbitalElementsState(tidateTime3, 0.0);
					orbitalElementsState.meanAnomalyAtEpoch_Rad = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, tiorbitState.barycenter, position, tidateTime3, base.fleet.faction.isActivePlayer);
					orbitalElementsState.epoch = tidateTime3.ExportTime();
					cartesianState6 = orbitalElementsState.ToCartesianStateAtTime(tidateTime.ExportTime(), tiorbitState.barycenter.mass_kg).ChangeReferenceFrame(tiorbitState.barycenter, barycenter, tidateTime);
				}
				else
				{
					cartesianState6 = Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(destinationValue, tidateTime, base.commonBarycenter, base.fleet.faction, null, 0.0);
				}
				segmentIndex = rawSegments.Count;
				if (base.destinationFleet != null && base.destinationFleet.transferAssigned && base.destinationFleet.trajectory.launchTime <= TITimeState.Now())
				{
					tinaturalSpaceObjectState2 = base.destinationFleet.trajectory.GetBarycenterAtTime(tidateTime);
				}
				else
				{
					tinaturalSpaceObjectState2 = destinationValue.barycenter();
				}
			}
			else
			{
				cartesianState6 = microthrustSegment.GlobalCartesianStateAtTime(tidateTime).ToLocal(barycenter, tidateTime);
				tinaturalSpaceObjectState2 = microthrustSegment.barycenter;
				num2 += tidateTime.DifferenceInSeconds(microthrustSegment.startTime);
			}
			TIDateTime tidateTime4 = new TIDateTime(tidateTime, -num2);
			CartesianState cartesianState7 = patchSegment2.GlobalCartesianStateAtTime(tidateTime4).ToLocal(barycenter, tidateTime4);
			Trajectory_Patched.BurnSegment burnSegment3 = new Trajectory_Patched.BurnSegment
			{
				startTime = boostStartTime,
				burnDuration_s = num,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = true,
				isImpulse = flag2,
				isTorch = !flag2,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription(cartesianState, cartesianState5, num)
			};
			List<Trajectory_Patched.BurnSegment> list = this.AdjustBurnSegmentToAvoidCollision(burnSegment3, tinaturalSpaceObjectState);
			Trajectory_Patched.BurnSegment burnSegment4 = new Trajectory_Patched.BurnSegment
			{
				startTime = tidateTime4,
				burnDuration_s = num2,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = false,
				isImpulse = flag2,
				isTorch = !flag2,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription(cartesianState7, cartesianState6, num2)
			};
			List<Trajectory_Patched.BurnSegment> list2 = this.AdjustBurnSegmentToAvoidCollision(burnSegment4, tinaturalSpaceObjectState2);
			prevBurnEndTime = tidateTime;
			List<Trajectory_Patched.IPatchSegment> list3 = this.AdjustImpulseTrajectoryToReachPeriapsis(list, patchSegment2, list2);
			if (microthrustSegment != null)
			{
				list3.Add(microthrustSegment);
			}
			return list3;
		}

		// Token: 0x060047D7 RID: 18391 RVA: 0x001D97CC File Offset: 0x001D79CC
		private List<Trajectory_Patched.IPatchSegment> AdjustImpulseTrajectoryToReachPeriapsis(List<Trajectory_Patched.BurnSegment> boostSegments, Trajectory_Patched.IPatchSegment orbitSegment, List<Trajectory_Patched.BurnSegment> decelSegments)
		{
			Trajectory_Patched.OrbitSegment orbitSegment2 = orbitSegment as Trajectory_Patched.OrbitSegment;
			if (orbitSegment2 == null || boostSegments.Count < 1 || decelSegments.Count < 1)
			{
				Log.Error(string.Concat(new string[]
				{
					"Failed to adjust impulse trajectory to reach periapsis.  orbitSegment = ",
					(orbitSegment != null) ? orbitSegment.ToString() : null,
					", boost and decel count = ",
					boostSegments.Count.ToString(),
					" and ",
					decelSegments.Count.ToString()
				}), Array.Empty<object>());
				List<Trajectory_Patched.IPatchSegment> list = new List<Trajectory_Patched.IPatchSegment>();
				list.AddRange(boostSegments);
				list.Add(orbitSegment);
				list.AddRange(decelSegments);
				return list;
			}
			Trajectory_Patched.BurnSegment burnSegment = boostSegments.Last<Trajectory_Patched.BurnSegment>();
			Trajectory_Patched.BurnSegment burnSegment2 = decelSegments.First<Trajectory_Patched.BurnSegment>();
			OrbitalElementsState orbit = orbitSegment2.orbit;
			TINaturalSpaceObjectState barycenter = orbitSegment.barycenter;
			TIDateTime tidateTime = new TIDateTime(orbit.NextTimeAtMeanAnomaly(0.0, burnSegment.startTime.ExportTime(), barycenter.mass_kg));
			CartesianState cartesianState = orbit.ToCartesianStateAtMeanAnomaly(0.0, barycenter.mass_kg);
			Vector3d vector3d = burnSegment.burnDescription.LocationInBurn(tidateTime.DifferenceInSeconds(burnSegment.startTime), burnSegment.burnDuration_s);
			if (tidateTime > burnSegment.startTime && tidateTime < burnSegment.endTime && (vector3d.sqrMagnitude < cartesianState.position.sqrMagnitude || Vector3d.Dot(in vector3d, in cartesianState.position) < 0.0))
			{
				CartesianState cartesianState2 = burnSegment.GlobalCartesianStateAtTime(burnSegment.startTime).ToLocal(burnSegment.barycenter, burnSegment.startTime);
				CartesianState cartesianState3 = cartesianState.ChangeReferenceFrame(barycenter, burnSegment.barycenter, tidateTime);
				CartesianState cartesianState4 = burnSegment.GlobalCartesianStateAtTime(burnSegment.endTime).ToLocal(barycenter, burnSegment.endTime);
				double num = tidateTime.DifferenceInSeconds(burnSegment.startTime);
				double num2 = burnSegment.endTime.DifferenceInSeconds(tidateTime);
				Trajectory_Patched.BurnSegment burnSegment3 = new Trajectory_Patched.BurnSegment
				{
					startTime = burnSegment.startTime,
					burnDuration_s = num,
					fleetAccel_mps2 = burnSegment.fleetAccel_mps2,
					isBoost = burnSegment.isBoost,
					barycenter = burnSegment.barycenter,
					burnDescription = new BurnBezierDescription(cartesianState2, cartesianState3, num)
				};
				Trajectory_Patched.BurnSegment burnSegment4 = new Trajectory_Patched.BurnSegment
				{
					startTime = tidateTime,
					burnDuration_s = num2,
					fleetAccel_mps2 = burnSegment.fleetAccel_mps2,
					isBoost = burnSegment.isBoost,
					barycenter = barycenter,
					burnDescription = new BurnBezierDescription(cartesianState, cartesianState4, num2)
				};
				List<Trajectory_Patched.BurnSegment> range = boostSegments.GetRange(0, boostSegments.Count - 1);
				List<Trajectory_Patched.IPatchSegment> list2 = new List<Trajectory_Patched.IPatchSegment>();
				list2.AddRange(range);
				list2.Add(burnSegment3);
				list2.Add(burnSegment4);
				list2.Add(orbitSegment);
				list2.AddRange(decelSegments);
				return list2;
			}
			Vector3d vector3d2 = burnSegment2.burnDescription.LocationInBurn(tidateTime.DifferenceInSeconds(burnSegment2.startTime), burnSegment2.burnDuration_s);
			if (tidateTime > burnSegment2.startTime && tidateTime < burnSegment2.endTime && (vector3d2.sqrMagnitude < cartesianState.position.sqrMagnitude || Vector3d.Dot(in vector3d2, in cartesianState.position) < 0.0))
			{
				CartesianState cartesianState5 = burnSegment2.GlobalCartesianStateAtTime(burnSegment2.startTime).ToLocal(barycenter, burnSegment2.startTime);
				CartesianState cartesianState6 = burnSegment2.GlobalCartesianStateAtTime(burnSegment2.endTime).ToLocal(burnSegment2.barycenter, burnSegment2.endTime);
				CartesianState cartesianState7 = cartesianState.ChangeReferenceFrame(barycenter, burnSegment2.barycenter, tidateTime);
				double num3 = tidateTime.DifferenceInSeconds(burnSegment2.startTime);
				double num4 = burnSegment2.endTime.DifferenceInSeconds(tidateTime);
				Trajectory_Patched.BurnSegment burnSegment5 = new Trajectory_Patched.BurnSegment
				{
					startTime = burnSegment2.startTime,
					burnDuration_s = num3,
					fleetAccel_mps2 = burnSegment2.fleetAccel_mps2,
					isBoost = burnSegment2.isBoost,
					barycenter = barycenter,
					burnDescription = new BurnBezierDescription(cartesianState5, cartesianState, num3)
				};
				Trajectory_Patched.BurnSegment burnSegment6 = new Trajectory_Patched.BurnSegment
				{
					startTime = tidateTime,
					burnDuration_s = num4,
					fleetAccel_mps2 = burnSegment2.fleetAccel_mps2,
					isBoost = burnSegment2.isBoost,
					barycenter = burnSegment2.barycenter,
					burnDescription = new BurnBezierDescription(cartesianState7, cartesianState6, num4)
				};
				List<Trajectory_Patched.BurnSegment> range2 = decelSegments.GetRange(1, decelSegments.Count - 1);
				List<Trajectory_Patched.IPatchSegment> list3 = new List<Trajectory_Patched.IPatchSegment>();
				list3.AddRange(boostSegments);
				list3.Add(orbitSegment);
				list3.Add(burnSegment5);
				list3.Add(burnSegment6);
				list3.AddRange(range2);
				return list3;
			}
			List<Trajectory_Patched.IPatchSegment> list4 = new List<Trajectory_Patched.IPatchSegment>();
			list4.AddRange(boostSegments);
			list4.Add(orbitSegment);
			list4.AddRange(decelSegments);
			return list4;
		}

		// Token: 0x060047D8 RID: 18392 RVA: 0x001D9C64 File Offset: 0x001D7E64
		private List<Trajectory_Patched.BurnSegment> AdjustBurnSegmentToAvoidCollision(Trajectory_Patched.BurnSegment originalBurnSegment, TINaturalSpaceObjectState collisionObject)
		{
			Vector3d vector3d = originalBurnSegment.GlobalCartesianStateAtTime(originalBurnSegment.startTime).position - collisionObject.GetGlobalPositionAtTime(originalBurnSegment.startTime);
			ValueTuple<TIDateTime, double> valueTuple = new ValueTuple<TIDateTime, double>(originalBurnSegment.startTime, vector3d.magnitude);
			Vector3d vector3d2 = originalBurnSegment.GlobalCartesianStateAtTime(originalBurnSegment.endTime).position - collisionObject.GetGlobalPositionAtTime(originalBurnSegment.endTime);
			ValueTuple<TIDateTime, double> valueTuple2 = new ValueTuple<TIDateTime, double>(originalBurnSegment.endTime, vector3d2.magnitude);
			for (int i = 0; i < 6; i++)
			{
				TIDateTime tidateTime = new TIDateTime(valueTuple.Item1, valueTuple2.Item1.DifferenceInSeconds(valueTuple.Item1) / 2.0);
				ValueTuple<TIDateTime, double> valueTuple3 = new ValueTuple<TIDateTime, double>(tidateTime, (originalBurnSegment.GlobalCartesianStateAtTime(tidateTime).position - collisionObject.GetGlobalPositionAtTime(tidateTime)).magnitude);
				if (valueTuple.Item2 < valueTuple2.Item2)
				{
					valueTuple2 = valueTuple3;
				}
				else
				{
					valueTuple = valueTuple3;
				}
			}
			ValueTuple<TIDateTime, double> valueTuple4 = ((valueTuple.Item2 < valueTuple2.Item2) ? valueTuple : valueTuple2);
			double num = 100000.0 + collisionObject.meanRadius_m;
			if (valueTuple4.Item2 >= num)
			{
				return new List<Trajectory_Patched.BurnSegment> { originalBurnSegment };
			}
			CartesianState cartesianState = originalBurnSegment.GlobalCartesianStateAtTime(valueTuple4.Item1).ToLocal(collisionObject, valueTuple4.Item1);
			cartesianState.position = cartesianState.position.normalized * num;
			CartesianState cartesianState2 = cartesianState.ChangeReferenceFrame(collisionObject, originalBurnSegment.barycenter, valueTuple4.Item1);
			CartesianState cartesianState3 = originalBurnSegment.GlobalCartesianStateAtTime(originalBurnSegment.startTime).ToLocal(originalBurnSegment.barycenter, originalBurnSegment.startTime);
			CartesianState cartesianState4 = originalBurnSegment.GlobalCartesianStateAtTime(originalBurnSegment.endTime).ToLocal(originalBurnSegment.barycenter, originalBurnSegment.endTime);
			double num2 = valueTuple4.Item1.DifferenceInSeconds(originalBurnSegment.startTime);
			Trajectory_Patched.BurnSegment burnSegment2;
			if (num2 > 0.0)
			{
				Trajectory_Patched.BurnSegment burnSegment = new Trajectory_Patched.BurnSegment();
				burnSegment.startTime = originalBurnSegment.startTime;
				burnSegment.burnDuration_s = num2;
				burnSegment.fleetAccel_mps2 = originalBurnSegment.fleetAccel_mps2;
				burnSegment.isBoost = originalBurnSegment.isBoost;
				burnSegment.barycenter = originalBurnSegment.barycenter;
				burnSegment2 = burnSegment;
				burnSegment.burnDescription = new BurnBezierDescription(cartesianState3, cartesianState2, num2);
			}
			else
			{
				burnSegment2 = null;
			}
			Trajectory_Patched.BurnSegment burnSegment3 = burnSegment2;
			double num3 = originalBurnSegment.endTime.DifferenceInSeconds(valueTuple4.Item1);
			Trajectory_Patched.BurnSegment burnSegment5;
			if (num3 > 0.0)
			{
				Trajectory_Patched.BurnSegment burnSegment4 = new Trajectory_Patched.BurnSegment();
				burnSegment4.startTime = valueTuple4.Item1;
				burnSegment4.burnDuration_s = num3;
				burnSegment4.fleetAccel_mps2 = originalBurnSegment.fleetAccel_mps2;
				burnSegment4.isBoost = originalBurnSegment.isBoost;
				burnSegment4.barycenter = originalBurnSegment.barycenter;
				burnSegment5 = burnSegment4;
				burnSegment4.burnDescription = new BurnBezierDescription(cartesianState2, cartesianState4, num3);
			}
			else
			{
				burnSegment5 = null;
			}
			Trajectory_Patched.BurnSegment burnSegment6 = burnSegment5;
			if (burnSegment3 == null && burnSegment6 == null)
			{
				return new List<Trajectory_Patched.BurnSegment>();
			}
			if (burnSegment3 == null)
			{
				return new List<Trajectory_Patched.BurnSegment> { burnSegment6 };
			}
			if (burnSegment6 == null)
			{
				return new List<Trajectory_Patched.BurnSegment> { burnSegment3 };
			}
			return new List<Trajectory_Patched.BurnSegment> { burnSegment3, burnSegment6 };
		}

		// Token: 0x060047D9 RID: 18393 RVA: 0x001D9F5C File Offset: 0x001D815C
		private List<Trajectory_Patched.IPatchSegment> CreateBurnSegments(BurnTransferSegment segment, List<IPatchedTransferSegment> rawSegments, ref int segmentIndex, ITransferTarget originValue, ITransferTarget destinationValue, ref TIDateTime prevBurnEndTime)
		{
			TINaturalSpaceObjectState barycenter = segment.barycenter;
			double dv_mps = segment.DV_mps;
			double duration_s = segment.duration_s;
			TIDateTime midpointTime = segment.midpointTime;
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.FindLast((Trajectory_Patched.IPatchSegment x) => x.startTime < segment.startTime);
			CartesianState cartesianState;
			Vector3d vector3d;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (patchSegment == null)
			{
				cartesianState = base.fleet.ToGlobalCartesianStateAtTime(segment.startTime) - barycenter.ToGlobalCartesianStateAtTime(segment.startTime);
				cartesianState.position = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState.position.xzy).xzy;
				cartesianState.velocity = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState.velocity.xzy).xzy;
				vector3d = (base.fleet.ToGlobalCartesianStateAtTime(midpointTime) - barycenter.ToGlobalCartesianStateAtTime(midpointTime)).position;
				vector3d = (Quaterniond.Inverse(barycenter.SpatialRotation) * vector3d.xzy).xzy;
				tinaturalSpaceObjectState = base.fleet.localBarycenter(segment.startTime);
			}
			else
			{
				Trajectory_Patched.MicrothrustSegment microthrustSegment = patchSegment as Trajectory_Patched.MicrothrustSegment;
				cartesianState = microthrustSegment.GlobalCartesianStateAtTime(segment.startTime) - barycenter.ToGlobalCartesianStateAtTime(segment.startTime);
				cartesianState.position = barycenter.SpatialRotation * cartesianState.position;
				cartesianState.velocity = barycenter.SpatialRotation * cartesianState.velocity;
				vector3d = (microthrustSegment.GlobalCartesianStateAtTime(midpointTime) - barycenter.ToGlobalCartesianStateAtTime(midpointTime)).position;
				microthrustSegment.endTime = segment.startTime;
				tinaturalSpaceObjectState = microthrustSegment.barycenter;
			}
			Trajectory_Patched.MicrothrustSegment microthrustSegment2 = null;
			for (int i = segmentIndex + 1; i < rawSegments.Count; i++)
			{
				if (rawSegments[i].startTime <= segment.endTime)
				{
					microthrustSegment2 = this.CreateMicrothrustSegment(rawSegments[i] as MicrothrustTransferSegment, segment.endTime);
					segmentIndex = i;
				}
			}
			CartesianState cartesianState2;
			if (microthrustSegment2 == null)
			{
				TIOrbitState tiorbitState = destinationValue as TIOrbitState;
				if (tiorbitState != null)
				{
					OrbitalElementsState orbitalElementsState = tiorbitState.ToOrbitalElementsState(midpointTime, 0.0);
					orbitalElementsState.meanAnomalyAtEpoch_Rad = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, barycenter, vector3d - barycenter.GetGlobalPositionAtTime(midpointTime), midpointTime, base.fleet.faction.isActivePlayer);
					cartesianState2 = orbitalElementsState.ToCartesianStateAtTime(segment.endTime.ExportTime(), tiorbitState.barycenter.mass_kg);
					cartesianState2.position = (tiorbitState.barycenter.SpatialRotation * cartesianState2.position.xzy).xzy;
					cartesianState2.velocity = (tiorbitState.barycenter.SpatialRotation * cartesianState2.velocity.xzy).xzy;
					cartesianState2 += tiorbitState.barycenter.ToGlobalCartesianStateAtTime(segment.endTime);
					cartesianState2 -= barycenter.ToGlobalCartesianStateAtTime(segment.endTime);
					cartesianState2.position = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState2.position.xzy).xzy;
					cartesianState2.velocity = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState2.velocity.xzy).xzy;
				}
				else
				{
					if (base.destinationFleet != null && base.destinationFleet.transferAssigned && base.destinationFleet.trajectory.launchTime <= TITimeState.Now())
					{
						base.destinationFleet.trajectory.GetBarycenterAtTime(segment.endTime);
					}
					else
					{
						destinationValue.barycenter();
					}
					TISpaceFleetState tispaceFleetState = destinationValue as TISpaceFleetState;
					if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime < segment.endTime)
					{
						tispaceFleetState.trajectory.GetBarycenterAtTime(segment.endTime);
					}
					cartesianState2 = Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(destinationValue, segment.endTime, barycenter, base.fleet.faction, null, 0.0);
				}
				segmentIndex = rawSegments.Count;
			}
			else
			{
				cartesianState2 = microthrustSegment2.GlobalCartesianStateAtTime(segment.endTime) - barycenter.ToGlobalCartesianStateAtTime(segment.endTime);
				cartesianState2 = barycenter.SpatialRotation * cartesianState2;
				microthrustSegment2.startTime = segment.endTime;
			}
			Trajectory_Patched.BurnSegment burnSegment = new Trajectory_Patched.BurnSegment
			{
				startTime = segment.startTime,
				burnDuration_s = duration_s,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = true,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription(cartesianState, cartesianState2, duration_s)
			};
			List<Trajectory_Patched.BurnSegment> list = this.AdjustBurnSegmentToAvoidCollision(burnSegment, tinaturalSpaceObjectState);
			List<Trajectory_Patched.IPatchSegment> list2 = new List<Trajectory_Patched.IPatchSegment>();
			list2.AddRange(list);
			if (microthrustSegment2 != null)
			{
				list2.Add(microthrustSegment2);
			}
			return list2;
		}

		// Token: 0x060047DA RID: 18394 RVA: 0x001DA4B0 File Offset: 0x001D86B0
		private List<Trajectory_Patched.IPatchSegment> CreateThreeImpulseTransferSegments(ThreeImpulseTransferSegment segment, List<IPatchedTransferSegment> rawSegments, ref int segmentIndex, ITransferTarget originValue, ITransferTarget destinationValue, ref TIDateTime prevBurnEndTime, [TupleElementNames(new string[] { "earliestBurnTime", "latestBurnTime" })] ValueTuple<TIDateTime, TIDateTime> burnTimeBounds)
		{
			TINaturalSpaceObjectState barycenter = segment.barycenter;
			TIDateTime burn0startTime = ((burnTimeBounds.Item1 == null) ? new TIDateTime(segment.startTime) : TIDateTime.Max(new TIDateTime(segment.startTime), burnTimeBounds.Item1));
			TIDateTime tidateTime = ((burnTimeBounds.Item2 == null) ? new TIDateTime(segment.endTime) : TIDateTime.Min(new TIDateTime(segment.endTime), burnTimeBounds.Item2));
			IPatchedTransferSegment patchedTransferSegment = null;
			IPatchedTransferSegment patchedTransferSegment2 = null;
			bool flag = false;
			foreach (IPatchedTransferSegment patchedTransferSegment3 in rawSegments)
			{
				if (flag)
				{
					patchedTransferSegment2 = patchedTransferSegment3;
					break;
				}
				if (segment == patchedTransferSegment3)
				{
					flag = true;
				}
				else
				{
					patchedTransferSegment = patchedTransferSegment3;
				}
			}
			double num = 0.0;
			if ((patchedTransferSegment is MicrothrustTransferSegment || patchedTransferSegment is MicrothrustTransferSegmentLERP) && burn0startTime < patchedTransferSegment.endTime)
			{
				num = patchedTransferSegment.endTime.DifferenceInSeconds(burn0startTime);
			}
			double num2 = 0.0;
			if ((patchedTransferSegment2 is MicrothrustTransferSegment || patchedTransferSegment2 is MicrothrustTransferSegmentLERP) && tidateTime > patchedTransferSegment2.startTime)
			{
				num2 = tidateTime.DifferenceInSeconds(patchedTransferSegment2.endTime);
			}
			TIDateTime tidateTime2 = new TIDateTime(burn0startTime, segment.burn0_duration_s + num);
			TIDateTime tidateTime3 = new TIDateTime(tidateTime, -segment.burn2_duration_s - num2);
			Trajectory_Patched.OrbitSegment orbitSegment = new Trajectory_Patched.OrbitSegment
			{
				startTime = tidateTime2,
				barycenter = barycenter,
				orbit = segment.orbit01,
				isImpulse = true
			};
			Trajectory_Patched.OrbitSegment orbitSegment2 = new Trajectory_Patched.OrbitSegment
			{
				startTime = segment.burn1_endTime,
				barycenter = barycenter,
				orbit = segment.orbit12,
				isImpulse = true
			};
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.FindLast((Trajectory_Patched.IPatchSegment x) => x.startTime < burn0startTime);
			CartesianState cartesianState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (patchSegment == null)
			{
				cartesianState = base.fleet.ToGlobalCartesianStateAtTime(burn0startTime) - barycenter.ToGlobalCartesianStateAtTime(burn0startTime);
				cartesianState.position = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState.position.xzy).xzy;
				cartesianState.velocity = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState.velocity.xzy).xzy;
				tinaturalSpaceObjectState = base.fleet.localBarycenter(burn0startTime);
			}
			else
			{
				Trajectory_Patched.MicrothrustSegment microthrustSegment = patchSegment as Trajectory_Patched.MicrothrustSegment;
				cartesianState = microthrustSegment.GlobalCartesianStateAtTime(burn0startTime) - microthrustSegment.barycenter.ToGlobalCartesianStateAtTime(burn0startTime);
				microthrustSegment.endTime = burn0startTime;
				cartesianState.position = barycenter.SpatialRotation * cartesianState.position;
				cartesianState.velocity = barycenter.SpatialRotation * cartesianState.velocity;
				cartesianState += microthrustSegment.barycenter.ToGlobalCartesianStateAtTime(burn0startTime) - barycenter.ToGlobalCartesianStateAtTime(burn0startTime);
				tinaturalSpaceObjectState = microthrustSegment.barycenter;
			}
			CartesianState cartesianState2 = orbitSegment.GlobalCartesianStateAtTime(tidateTime2) - barycenter.ToGlobalCartesianStateAtTime(tidateTime2);
			cartesianState2 = barycenter.SpatialRotation * cartesianState2;
			CartesianState cartesianState3 = orbitSegment.GlobalCartesianStateAtTime(segment.burn1_startTime) - barycenter.ToGlobalCartesianStateAtTime(segment.burn1_startTime);
			cartesianState3 = barycenter.SpatialRotation * cartesianState3;
			CartesianState cartesianState4 = orbitSegment2.GlobalCartesianStateAtTime(segment.burn1_endTime) - barycenter.ToGlobalCartesianStateAtTime(segment.burn1_endTime);
			cartesianState4 = barycenter.SpatialRotation * cartesianState4;
			CartesianState cartesianState5 = orbitSegment2.GlobalCartesianStateAtTime(tidateTime3) - barycenter.ToGlobalCartesianStateAtTime(tidateTime3);
			cartesianState5 = barycenter.SpatialRotation * cartesianState5;
			Trajectory_Patched.MicrothrustSegment microthrustSegment2 = null;
			for (int i = segmentIndex + 1; i < rawSegments.Count; i++)
			{
				if (rawSegments[i].startTime <= tidateTime)
				{
					microthrustSegment2 = this.CreateMicrothrustSegment(rawSegments[i] as MicrothrustTransferSegment, tidateTime);
					segmentIndex = i;
				}
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState2;
			CartesianState cartesianState6;
			if (microthrustSegment2 == null)
			{
				tinaturalSpaceObjectState2 = destinationValue.barycenter();
				TISpaceFleetState tispaceFleetState = destinationValue as TISpaceFleetState;
				if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime < tidateTime)
				{
					tinaturalSpaceObjectState2 = tispaceFleetState.trajectory.GetBarycenterAtTime(tidateTime);
				}
				TIOrbitState tiorbitState = destinationValue as TIOrbitState;
				if (tiorbitState != null)
				{
					TIDateTime tidateTime4 = new TIDateTime(tidateTime, -this.decelDuration_s * 0.5);
					Vector3d position = orbitSegment2.GlobalCartesianStateAtTime(tidateTime4).position;
					OrbitalElementsState orbitalElementsState = tiorbitState.ToOrbitalElementsState(tidateTime4, 0.0);
					orbitalElementsState.meanAnomalyAtEpoch_Rad = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, barycenter, position - barycenter.GetGlobalPositionAtTime(tidateTime4), tidateTime4, base.fleet.faction.isActivePlayer);
					cartesianState6 = orbitalElementsState.ToCartesianStateAtTime(tidateTime.ExportTime(), tiorbitState.barycenter.mass_kg);
					cartesianState6.position = (tiorbitState.barycenter.SpatialRotation * cartesianState6.position.xzy).xzy;
					cartesianState6.velocity = (tiorbitState.barycenter.SpatialRotation * cartesianState6.velocity.xzy).xzy;
					cartesianState6 += tiorbitState.barycenter.ToGlobalCartesianStateAtTime(tidateTime);
					cartesianState6 -= barycenter.ToGlobalCartesianStateAtTime(tidateTime);
					cartesianState6.position = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState6.position.xzy).xzy;
					cartesianState6.velocity = (Quaterniond.Inverse(barycenter.SpatialRotation) * cartesianState6.velocity.xzy).xzy;
				}
				else
				{
					cartesianState6 = Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(destinationValue, tidateTime, tinaturalSpaceObjectState2, base.fleet.faction, null, 0.0);
					cartesianState6 = barycenter.SpatialRotation * cartesianState6;
				}
				segmentIndex = rawSegments.Count;
			}
			else
			{
				cartesianState6 = microthrustSegment2.GlobalCartesianStateAtTime(tidateTime) - barycenter.ToGlobalCartesianStateAtTime(tidateTime);
				cartesianState6 = barycenter.SpatialRotation * cartesianState6;
				tinaturalSpaceObjectState2 = microthrustSegment2.barycenter;
			}
			Trajectory_Patched.BurnSegment burnSegment = new Trajectory_Patched.BurnSegment
			{
				startTime = burn0startTime,
				burnDuration_s = segment.burn0_duration_s,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = true,
				isImpulse = true,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription(cartesianState, cartesianState2, segment.burn0_duration_s)
			};
			Trajectory_Patched.BurnSegment burnSegment2 = new Trajectory_Patched.BurnSegment
			{
				startTime = segment.burn1_startTime,
				burnDuration_s = segment.burn1_duration_s,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = true,
				isImpulse = true,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription(cartesianState3, cartesianState4, segment.burn1_duration_s)
			};
			Trajectory_Patched.BurnSegment burnSegment3 = new Trajectory_Patched.BurnSegment
			{
				startTime = tidateTime3,
				burnDuration_s = segment.burn2_duration_s,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = false,
				isImpulse = true,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription(cartesianState5, cartesianState6, segment.burn2_duration_s)
			};
			List<Trajectory_Patched.BurnSegment> list = this.AdjustBurnSegmentToAvoidCollision(burnSegment, tinaturalSpaceObjectState);
			List<Trajectory_Patched.BurnSegment> list2 = this.AdjustBurnSegmentToAvoidCollision(burnSegment2, barycenter);
			List<Trajectory_Patched.BurnSegment> list3 = this.AdjustBurnSegmentToAvoidCollision(burnSegment3, tinaturalSpaceObjectState2);
			prevBurnEndTime = tidateTime;
			List<Trajectory_Patched.IPatchSegment> list4 = new List<Trajectory_Patched.IPatchSegment>();
			list4.AddRange(list);
			list4.Add(orbitSegment);
			list4.AddRange(list2);
			list4.Add(orbitSegment2);
			list4.AddRange(list3);
			if (microthrustSegment2 != null)
			{
				list4.Add(microthrustSegment2);
			}
			return list4;
		}

		// Token: 0x060047DB RID: 18395 RVA: 0x001DAC80 File Offset: 0x001D8E80
		private List<Trajectory_Patched.IPatchSegment> CreateTorchSegments(TorchTransferSegment segment, Vector3d globalStartPosition, Trajectory_Patched.MicrothrustSegment nextSegment, ITransferTarget destinationValue, TINaturalSpaceObjectState sourceBarycenter)
		{
			if (nextSegment != null)
			{
				return this.CreateTorchSegments(segment, globalStartPosition, nextSegment.GlobalCartesianStateAtTime(segment.endTime), sourceBarycenter, destinationValue.barycenter());
			}
			TIOrbitState tiorbitState = destinationValue as TIOrbitState;
			if (tiorbitState != null)
			{
				double accelDuration_s = segment.torch.accelDuration_s;
				double decelDuration_s = segment.torch.decelDuration_s;
				double transitDuration_s = segment.torch.transitDuration_s;
				double num = transitDuration_s - accelDuration_s - decelDuration_s;
				Vector3d vector3d = segment.torch.accelerationVector_mps2 * accelDuration_s;
				Vector3d vector3d2 = segment.torch.initialVelocityVector_mps * transitDuration_s + 0.5 * segment.torch.accelerationVector_mps2 * accelDuration_s * accelDuration_s + vector3d * (num + decelDuration_s) + 0.5 * segment.torch.decelerationVector_mps2 * decelDuration_s * decelDuration_s;
				Vector3d vector3d3 = globalStartPosition - segment.barycenter.GetGlobalPositionAtTime(segment.startTime);
				vector3d3 = (Quaterniond.Inverse(segment.barycenter.SpatialRotation) * vector3d3.xzy).xzy;
				Vector3d vector3d4 = vector3d3 + vector3d2;
				OrbitalElementsState orbitalElementsState = tiorbitState.ToOrbitalElementsState(segment.endTime, 0.0);
				orbitalElementsState.meanAnomalyAtEpoch_Rad = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, segment.barycenter, vector3d4, segment.endTime, base.fleet.faction.isActivePlayer);
				CartesianState cartesianState = orbitalElementsState.ToCartesianStateAtTime(segment.endTime.ExportTime(), tiorbitState.barycenter.mass_kg);
				cartesianState.position = (tiorbitState.barycenter.SpatialRotation * cartesianState.position.xzy).xzy;
				cartesianState.velocity = (tiorbitState.barycenter.SpatialRotation * cartesianState.velocity.xzy).xzy;
				cartesianState += tiorbitState.barycenter.ToGlobalCartesianStateAtTime(segment.endTime);
				return this.CreateTorchSegments(segment, globalStartPosition, cartesianState, sourceBarycenter, destinationValue.barycenter());
			}
			destinationValue.barycenter();
			CartesianState cartesianState2;
			if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destinationValue as TISpaceFleetState, base.fleet.faction))
			{
				cartesianState2 = (destinationValue as TISpaceFleetState).tryToGetGlobalCartesianState(segment.endTime).Value;
			}
			else
			{
				OrbitalElementsState orbitalElementsState2;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				bool flag;
				destinationValue.getOrbitalElementsState(segment.endTime, out orbitalElementsState2, out tinaturalSpaceObjectState, out flag);
				cartesianState2 = orbitalElementsState2.ToCartesianStateAtTime(segment.endTime.ExportTime(), tinaturalSpaceObjectState.mass_kg).ToGlobal(tinaturalSpaceObjectState, segment.endTime);
			}
			return this.CreateTorchSegments(segment, globalStartPosition, cartesianState2, sourceBarycenter, destinationValue.barycenter());
		}

		// Token: 0x060047DC RID: 18396 RVA: 0x001DAF38 File Offset: 0x001D9138
		private List<Trajectory_Patched.IPatchSegment> CreateTorchSegments(TorchTransferSegment segment, Vector3d globalStartPosition, CartesianState globalEndState, TINaturalSpaceObjectState sourceBarycenter, TINaturalSpaceObjectState destinationBarycenter)
		{
			CartesianState cartesianState = new CartesianState(globalStartPosition, segment.initialGlobalVelocity_mps);
			CartesianState cartesianState2 = cartesianState.ToLocal(segment.barycenter, segment.startTime);
			CartesianState cartesianState3 = globalEndState.ToLocal(segment.barycenter, segment.endTime);
			double num = segment.torch.accelDuration_s;
			double num2 = segment.torch.decelDuration_s;
			double num3 = segment.torch.coastDuration_s;
			Vector3d vector3d = cartesianState2.velocity * num + 0.5 * segment.torch.accelerationVector_mps2 * num * num;
			Vector3d vector3d2 = (cartesianState3.velocity - segment.torch.decelerationVector_mps2 * num2) * num2 + 0.5 * segment.torch.decelerationVector_mps2 * num2 * num2;
			Vector3d vector3d3 = cartesianState2.position + vector3d;
			Vector3d vector3d4 = cartesianState3.position - vector3d2;
			Vector3d vector3d5 = (vector3d4 - vector3d3) / num3;
			CartesianState xzy = new CartesianState(vector3d3, vector3d5);
			CartesianState xzy2 = new CartesianState(vector3d4, vector3d5);
			TIDateTime startTime = segment.startTime;
			TIDateTime endTime = segment.endTime;
			cartesianState2 = cartesianState2.xzy;
			xzy = xzy.xzy;
			xzy2 = xzy2.xzy;
			cartesianState3 = cartesianState3.xzy;
			num += segment.initialGravwellDuration_s;
			xzy.position += vector3d5 * segment.initialGravwellDuration_s;
			num2 += segment.finalGravwellDuration_s;
			xzy2.position -= vector3d5 * segment.finalGravwellDuration_s;
			num3 -= segment.initialGravwellDuration_s + segment.finalGravwellDuration_s;
			if (num2 + num > segment.torch.transitDuration_s)
			{
				double num4 = num2 + num - segment.torch.transitDuration_s;
				num2 -= num4 * 0.5;
				num -= num4 * 0.5;
				num3 = 0.0;
			}
			Trajectory_Patched.BurnSegment burnSegment = new Trajectory_Patched.BurnSegment
			{
				startTime = startTime,
				burnDuration_s = num,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = true,
				isTorch = true,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription
				{
					startPosition = cartesianState2.position,
					endPosition = xzy.position,
					startVelocityControlPoint = cartesianState2.position + cartesianState2.velocity * num / 3.0,
					endVelocityControlPoint = xzy.position - xzy.velocity * num / 3.0
				}
			};
			Trajectory_Patched.BurnSegment burnSegment2 = new Trajectory_Patched.BurnSegment
			{
				startTime = new TIDateTime(endTime, -num2),
				burnDuration_s = num2,
				fleetAccel_mps2 = base.fleetCruiseAcceleration_mps2,
				isBoost = false,
				isTorch = true,
				barycenter = segment.barycenter,
				burnDescription = new BurnBezierDescription
				{
					startPosition = xzy2.position,
					endPosition = cartesianState3.position,
					startVelocityControlPoint = xzy2.position + xzy2.velocity * num2 / 3.0,
					endVelocityControlPoint = cartesianState3.position - cartesianState3.velocity * num2 / 3.0
				}
			};
			List<Trajectory_Patched.BurnSegment> list = this.AdjustBurnSegmentToAvoidCollision(burnSegment, sourceBarycenter);
			List<Trajectory_Patched.BurnSegment> list2 = this.AdjustBurnSegmentToAvoidCollision(burnSegment2, destinationBarycenter);
			Trajectory_Patched.TorchCoastSegment torchCoastSegment = new Trajectory_Patched.TorchCoastSegment
			{
				startTime = new TIDateTime(startTime, num),
				barycenter = segment.barycenter,
				duration_s = num3,
				startPosition = xzy.position.xzy,
				endPosition = xzy2.position.xzy,
				isTorch = true
			};
			List<Trajectory_Patched.IPatchSegment> list3 = new List<Trajectory_Patched.IPatchSegment>();
			list3.AddRange(list);
			list3.Add(torchCoastSegment);
			list3.AddRange(list2);
			return list3;
		}

		// Token: 0x060047DD RID: 18397 RVA: 0x001DB364 File Offset: 0x001D9564
		public void AddRemnantsOfExistingTransfer(Trajectory_Patched oldTrajectory)
		{
			if (oldTrajectory == null)
			{
				return;
			}
			TIDateTime tidateTime = TITimeState.Now();
			if (tidateTime > oldTrajectory.arrivalTime)
			{
				return;
			}
			List<Trajectory_Patched.IPatchSegment> list = new List<Trajectory_Patched.IPatchSegment>();
			for (int i = 0; i < oldTrajectory.Segments.Count<Trajectory_Patched.IPatchSegment>(); i++)
			{
				if (oldTrajectory.Segments[i].startTime <= tidateTime)
				{
					list.Clear();
				}
				if (oldTrajectory.Segments[i].startTime > base.launchTime)
				{
					break;
				}
				list.Add(oldTrajectory.Segments[i]);
			}
			if (list.Count<Trajectory_Patched.IPatchSegment>() == 0)
			{
				return;
			}
			Trajectory_Patched.ISupportsReducedCopy supportsReducedCopy = list[0] as Trajectory_Patched.ISupportsReducedCopy;
			if (supportsReducedCopy != null)
			{
				if (supportsReducedCopy.endTime <= tidateTime)
				{
					list.RemoveAt(0);
				}
				else
				{
					list[0] = supportsReducedCopy.ReducedCopy(tidateTime, supportsReducedCopy.endTime);
				}
			}
			if (oldTrajectory.arrivalTime < base.launchTime && oldTrajectory.destinationOrbit != null)
			{
				TIOrbitState destinationOrbit = oldTrajectory.destinationOrbit;
				double destinationMeanAnomalyAtArrival = oldTrajectory.getDestinationMeanAnomalyAtArrival();
				Trajectory_Patched.OrbitSegment orbitSegment = new Trajectory_Patched.OrbitSegment
				{
					startTime = oldTrajectory.arrivalTime,
					barycenter = destinationOrbit.barycenter,
					orbit = destinationOrbit.ToOrbitalElementsState(oldTrajectory.arrivalTime, destinationMeanAnomalyAtArrival)
				};
				list.Add(orbitSegment);
			}
			int num = list.Count<Trajectory_Patched.IPatchSegment>() - 1;
			if (num >= 0)
			{
				if (list[num].startTime >= base.launchTime)
				{
					list.RemoveAt(num);
				}
				else
				{
					Trajectory_Patched.ISupportsReducedCopy supportsReducedCopy2 = list[num] as Trajectory_Patched.ISupportsReducedCopy;
					if (supportsReducedCopy2 != null)
					{
						list[num] = supportsReducedCopy2.ReducedCopy(supportsReducedCopy2.startTime, base.launchTime);
					}
				}
			}
			this.Segments.InsertRange(0, list);
			base.launchTime = tidateTime;
			this.RecalculateCommonBarycenter();
		}

		// Token: 0x060047DE RID: 18398 RVA: 0x001DB524 File Offset: 0x001D9724
		public override bool isPlausible()
		{
			if (base.launchTime > base.arrivalTime)
			{
				string[] array = new string[5];
				array[0] = "Patched trajectory implausible: trajectory arrives (";
				int num = 1;
				TIDateTime arrivalTime = base.arrivalTime;
				array[num] = ((arrivalTime != null) ? arrivalTime.ToString() : null);
				array[2] = ") before it launches (";
				int num2 = 3;
				TIDateTime launchTime = base.launchTime;
				array[num2] = ((launchTime != null) ? launchTime.ToString() : null);
				array[4] = ").";
				Log.Error(string.Concat(array), Array.Empty<object>());
				return false;
			}
			if (this.Segments.Count > 0)
			{
				TIDateTime startTime = this.Segments.First<Trajectory_Patched.IPatchSegment>().startTime;
				if (base.launchTime != startTime)
				{
					string[] array2 = new string[5];
					array2[0] = "Patched trajectory implausible: launch time (";
					int num3 = 1;
					TIDateTime launchTime2 = base.launchTime;
					array2[num3] = ((launchTime2 != null) ? launchTime2.ToString() : null);
					array2[2] = ") does not match initial segment start time (";
					int num4 = 3;
					TIDateTime tidateTime = startTime;
					array2[num4] = ((tidateTime != null) ? tidateTime.ToString() : null);
					array2[4] = ").";
					Log.Error(string.Concat(array2), Array.Empty<object>());
					return false;
				}
				TIDateTime startTime2 = this.Segments.Last<Trajectory_Patched.IPatchSegment>().startTime;
				if (base.arrivalTime < startTime2)
				{
					string[] array3 = new string[5];
					array3[0] = "Patched trajectory implausible: arrival time (";
					int num5 = 1;
					TIDateTime arrivalTime2 = base.arrivalTime;
					array3[num5] = ((arrivalTime2 != null) ? arrivalTime2.ToString() : null);
					array3[2] = ") is before final segment starts (";
					int num6 = 3;
					TIDateTime tidateTime2 = startTime2;
					array3[num6] = ((tidateTime2 != null) ? tidateTime2.ToString() : null);
					array3[4] = ").";
					Log.Error(string.Concat(array3), Array.Empty<object>());
					return false;
				}
			}
			TIDateTime tidateTime3 = base.launchTime;
			foreach (Trajectory_Patched.IPatchSegment patchSegment in this.Segments)
			{
				if (patchSegment.startTime < tidateTime3)
				{
					Log.Error("Patched trajectory implausible: a later segment started before an earlier one.", Array.Empty<object>());
					return false;
				}
				tidateTime3 = patchSegment.startTime;
				if (!patchSegment.isPlausible(base.fleet))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060047DF RID: 18399 RVA: 0x001DB720 File Offset: 0x001D9920
		public override string GetDisplayName()
		{
			if (this.Segments.Any<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x is Trajectory_Patched.MicrothrustSegment))
			{
				return Loc.T("UI.Operations.Microthrust");
			}
			if (this.Segments.Any<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x is Trajectory_Patched.TorchCoastSegment))
			{
				return Loc.T("UI.Operations.Torch");
			}
			return Loc.T("UI.Operations.Impulse");
		}

		// Token: 0x060047E0 RID: 18400 RVA: 0x001DB7A8 File Offset: 0x001D99A8
		public override TINaturalSpaceObjectState GetBarycenterAtTime(TIDateTime time)
		{
			if (time < base.launchTime || time > base.arrivalTime)
			{
				return base.GetBarycenterAtTime(time);
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime <= time);
			if (patchSegment == null)
			{
				return base.GetBarycenterAtTime(time);
			}
			return patchSegment.barycenter;
		}

		// Token: 0x060047E1 RID: 18401 RVA: 0x001DB824 File Offset: 0x001D9A24
		public override OrbitalElementsState GetOrbitalElementsAtTime(TIDateTime time, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			if ((time < base.launchTime && base.fleet.ref_orbit != null) || time > base.arrivalTime)
			{
				return base.GetOrbitalElementsAtTime(time, precision);
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime <= time);
			if (patchSegment == null)
			{
				return base.originOrbit.ToOrbitalElementsState(base.launchTime, 0.0);
			}
			return patchSegment.OrbitalElementsAtTime(time);
		}

		// Token: 0x060047E2 RID: 18402 RVA: 0x001DB8C8 File Offset: 0x001D9AC8
		public override bool isInMicrothrust(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			if (time < base.launchTime)
			{
				return false;
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime <= time);
			return patchSegment != null && patchSegment is Trajectory_Patched.MicrothrustSegment;
		}

		// Token: 0x060047E3 RID: 18403 RVA: 0x001DB938 File Offset: 0x001D9B38
		[return: TupleElementNames(new string[] { "start", "domain" })]
		public override List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> GetTrajectoryDomainsOverTime()
		{
			List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> list = new List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>();
			foreach (Trajectory_Patched.IPatchSegment patchSegment in this.Segments)
			{
				if (patchSegment is Trajectory_Patched.MicrothrustSegment)
				{
					list.Add(new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(patchSegment.startTime, Trajectory.TrajectoryDomain.Microthrust));
				}
				else if (patchSegment.isOrbitPhasing)
				{
					list.Add(new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(patchSegment.startTime, Trajectory.TrajectoryDomain.OrbitPhasing));
				}
				else if (patchSegment.isImpulse)
				{
					list.Add(new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(patchSegment.startTime, Trajectory.TrajectoryDomain.Impulse));
				}
				else if (patchSegment.isTorch)
				{
					list.Add(new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(patchSegment.startTime, Trajectory.TrajectoryDomain.Torch));
				}
				else if (patchSegment is Trajectory_Patched.BurnSegment)
				{
					Debug.LogWarning("Trajectory_Patched.GetTrajectoryDomainsOverTime: we found a burn segment that wasn't impulse, torch, or orbit phasing.");
				}
				else
				{
					list.Add(new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(patchSegment.startTime, Trajectory.TrajectoryDomain.Orbit));
				}
			}
			List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> list2 = new List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>();
			foreach (ValueTuple<TIDateTime, Trajectory.TrajectoryDomain> valueTuple in list)
			{
				if (list2.Count == 0 || list2.Last<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>().Item2 != valueTuple.Item2)
				{
					list2.Add(valueTuple);
				}
			}
			return list2;
		}

		// Token: 0x060047E4 RID: 18404 RVA: 0x001DBA8C File Offset: 0x001D9C8C
		public override bool CantManeuver(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			if (time < base.launchTime)
			{
				return false;
			}
			int num = this.Segments.Count;
			int num2 = 0;
			while (num2 < this.Segments.Count && this.Segments[num2].startTime <= time)
			{
				num = num2;
				num2++;
			}
			if (num == this.Segments.Count || time > base.arrivalTime)
			{
				return this.nextTrajectory != null && this.nextTrajectory.CantManeuver(time);
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments[num];
			if (patchSegment is Trajectory_Patched.MicrothrustSegment)
			{
				return true;
			}
			if (patchSegment is Trajectory_Patched.BurnSegment && !(patchSegment is Trajectory_Patched.FalseBurnSegment))
			{
				int num3 = num;
				while (num3 < this.Segments.Count && ((this.Segments[num3] is Trajectory_Patched.BurnSegment && !(this.Segments[num3] is Trajectory_Patched.FalseBurnSegment)) || this.Segments[num3] is Trajectory_Patched.MicrothrustSegment))
				{
					if (num3 == this.Segments.Count - 1)
					{
						return true;
					}
					num3++;
				}
			}
			return patchSegment.isOrbitPhasing;
		}

		// Token: 0x060047E5 RID: 18405 RVA: 0x001DBBBC File Offset: 0x001D9DBC
		public override bool isInImpulse(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			if (time < base.launchTime)
			{
				return false;
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime <= time);
			return patchSegment != null && patchSegment.isImpulse;
		}

		// Token: 0x060047E6 RID: 18406 RVA: 0x001DBC28 File Offset: 0x001D9E28
		public override double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
		{
			if (timeToCheck < base.launchTime || timeToCheck > base.arrivalTime)
			{
				return base.getDistFromBarycenterAtTime_m(timeToCheck, out barycenter);
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime <= timeToCheck);
			if (patchSegment == null)
			{
				return base.getDistFromBarycenterAtTime_m(timeToCheck, out barycenter);
			}
			return patchSegment.getDistFromBarycenterAtTime_m(timeToCheck, out barycenter);
		}

		// Token: 0x060047E7 RID: 18407 RVA: 0x001DBCAC File Offset: 0x001D9EAC
		public override double RemainingDVatTime_mps(TIDateTime time)
		{
			if (time < base.launchTime)
			{
				return this.DV_mps;
			}
			if (!(time > base.arrivalTime))
			{
				double num = 0.0;
				Trajectory_Patched.IPatchSegment patchSegment = null;
				bool flag = false;
				foreach (Trajectory_Patched.IPatchSegment patchSegment2 in this.Segments)
				{
					if (patchSegment2.startTime > time)
					{
						if (!flag)
						{
							flag = true;
							if (patchSegment != null)
							{
								num = patchSegment.DVConsumedByTime(base.arrivalTime) - patchSegment.DVConsumedByTime(time);
							}
						}
						num += patchSegment2.DVConsumedByTime(base.arrivalTime);
					}
					patchSegment = patchSegment2;
				}
				if (!flag && patchSegment != null)
				{
					num += patchSegment.DVConsumedByTime(base.arrivalTime) - patchSegment.DVConsumedByTime(time);
				}
				num += base.PostTransferDVfromTargetFleet_mps();
				return num;
			}
			if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(base.destinationFleet, base.fleet.faction))
			{
				return base.destinationFleet.trajectory.RemainingDVatTime_mps(time);
			}
			return 0.0;
		}

		// Token: 0x060047E8 RID: 18408 RVA: 0x001DBDC4 File Offset: 0x001D9FC4
		public override CartesianState ToGlobalCartesianStateAtTime(TIDateTime timeToCheck)
		{
			if (timeToCheck > base.arrivalTime)
			{
				CartesianState cartesianState;
				try
				{
					cartesianState = base.DestinationCartesianStateAtTime(timeToCheck);
				}
				catch
				{
					cartesianState = this.ToGlobalCartesianStateAtTime(new TIDateTime(base.arrivalTime, -1.0));
				}
				return cartesianState;
			}
			Trajectory_Patched.IPatchSegment patchSegment = this.Segments.LastOrDefault<Trajectory_Patched.IPatchSegment>((Trajectory_Patched.IPatchSegment x) => x.startTime <= timeToCheck);
			if (patchSegment == null)
			{
				return default(CartesianState);
			}
			return patchSegment.GlobalCartesianStateAtTime(timeToCheck);
		}

		// Token: 0x060047E9 RID: 18409 RVA: 0x001DBE64 File Offset: 0x001DA064
		public override Vector3d PositionAtTime(TIDateTime timeToCheck, bool setPosition, out bool arrived)
		{
			Trajectory_Patched.IPatchSegment patchSegment = null;
			double num = 0.0;
			Trajectory_Patched.ThrustPhase thrustPhase = Trajectory_Patched.ThrustPhase.Loitering;
			Vector3d vector3d = default(Vector3d);
			foreach (Trajectory_Patched.IPatchSegment patchSegment2 in this.Segments)
			{
				if (patchSegment2.startTime > timeToCheck)
				{
					break;
				}
				if (setPosition)
				{
					num += patchSegment2.DVConsumedByTime(timeToCheck);
				}
				patchSegment = patchSegment2;
			}
			if (patchSegment == null)
			{
				thrustPhase = Trajectory_Patched.ThrustPhase.Loitering;
			}
			else
			{
				vector3d = patchSegment.GlobalPositionAtTime(timeToCheck);
				this.UpdateThrustPhase(ref thrustPhase, patchSegment.GetThrustPhaseAtTime(timeToCheck));
				if (setPosition && this.launched)
				{
					double num2 = base.fleet.fleetTrajectoryData.initialDeltaV_mps - (double)base.fleet.currentDeltaV_mps;
					float DVToConsume_kps = (float)(num - num2) / 1000f;
					if (DVToConsume_kps > 0f)
					{
						base.fleet.ships.ForEach(delegate(TISpaceShipState x)
						{
							x.ConsumeDeltaV(DVToConsume_kps, false);
						});
					}
				}
			}
			arrived = timeToCheck > base.arrivalTime;
			if (setPosition)
			{
				this.UpdateFleetAccelerationPhaseStatus(thrustPhase, arrived);
			}
			if (thrustPhase == Trajectory_Patched.ThrustPhase.Loitering)
			{
				return base.fleet.ToGlobalCartesianStateAtTime(timeToCheck).position;
			}
			return vector3d;
		}

		// Token: 0x060047EA RID: 18410 RVA: 0x001DBFAC File Offset: 0x001DA1AC
		private TIDateTime Min(TIDateTime a, TIDateTime b)
		{
			if (!(a > b))
			{
				return a;
			}
			return b;
		}

		// Token: 0x060047EB RID: 18411 RVA: 0x001DBFBA File Offset: 0x001DA1BA
		private void UpdateThrustPhase(ref Trajectory_Patched.ThrustPhase thrustPhase, Trajectory_Patched.ThrustPhase newThrustPhase)
		{
			if (newThrustPhase != Trajectory_Patched.ThrustPhase.ContinuePreviousBurn)
			{
				thrustPhase = newThrustPhase;
				return;
			}
			if (thrustPhase == Trajectory_Patched.ThrustPhase.Loitering)
			{
				thrustPhase = Trajectory_Patched.ThrustPhase.Accelerating;
				return;
			}
			if (thrustPhase == Trajectory_Patched.ThrustPhase.Coasting)
			{
				thrustPhase = Trajectory_Patched.ThrustPhase.Decelerating;
			}
		}

		// Token: 0x060047EC RID: 18412 RVA: 0x001DBFD8 File Offset: 0x001DA1D8
		private void UpdateFleetAccelerationPhaseStatus(Trajectory_Patched.ThrustPhase thrustPhase, bool finishedTransfer)
		{
			if (finishedTransfer)
			{
				base.fleet.SetAccelerationPhaseStatus(false, false, false);
				base.fleet.SetDecelerationPhaseStatus(false, false, false);
				return;
			}
			if (thrustPhase == Trajectory_Patched.ThrustPhase.Accelerating)
			{
				base.fleet.SetAccelerationPhaseStatus(true, false, false);
				base.fleet.SetDecelerationPhaseStatus(false, false, false);
				return;
			}
			if (thrustPhase == Trajectory_Patched.ThrustPhase.Decelerating)
			{
				base.fleet.SetAccelerationPhaseStatus(false, false, false);
				base.fleet.SetDecelerationPhaseStatus(true, false, false);
				return;
			}
			base.fleet.SetAccelerationPhaseStatus(false, false, false);
			base.fleet.SetDecelerationPhaseStatus(false, false, false);
		}

		// Token: 0x060047ED RID: 18413 RVA: 0x001DC064 File Offset: 0x001DA264
		public override string deepDump()
		{
			string text = "   Trajectory_Patched:\n";
			base.appendCommonDeepDump(ref text);
			for (int i = 0; i < this.Segments.Count; i++)
			{
				text = string.Concat(new string[]
				{
					text,
					"    segment ",
					i.ToString(),
					" ",
					this.Segments[i].deepDump()
				});
			}
			base.appendCommonDeepDumpPostscript(ref text);
			return text;
		}

		// Token: 0x060047EE RID: 18414 RVA: 0x001DC0DC File Offset: 0x001DA2DC
		public string DumpSegments()
		{
			return string.Join(",", this.Segments.Select<Trajectory_Patched.IPatchSegment, string>((Trajectory_Patched.IPatchSegment x) => x.DumpSegment()));
		}

		// Token: 0x060047EF RID: 18415 RVA: 0x001DC114 File Offset: 0x001DA314
		public override TIDateTime getOrbitEndTime()
		{
			Trajectory_Patched.IPatchSegment patchSegment = null;
			Trajectory_Patched.IPatchSegment patchSegment2 = null;
			foreach (Trajectory_Patched.IPatchSegment patchSegment3 in this.Segments)
			{
				if (patchSegment3.startTime > TITimeState.Now())
				{
					patchSegment2 = patchSegment3;
					break;
				}
				patchSegment = patchSegment3;
			}
			if (patchSegment == null)
			{
				return base.getOrbitEndTime();
			}
			if (!(patchSegment is Trajectory_Patched.OrbitSegment) && !(patchSegment is Trajectory_Patched.OrbitLERPSegment))
			{
				return base.getOrbitEndTime();
			}
			if (patchSegment2 == null)
			{
				Debug.LogWarning("Trajectory_Patched.getOrbitEndTime(): the last segment is a coasting orbit segment.  There should be a segment between this and the destination (such as a burn).");
				return base.arrivalTime;
			}
			return patchSegment2.startTime;
		}

		// Token: 0x04002992 RID: 10642
		public List<Trajectory_Patched.IPatchSegment> Segments;

		// Token: 0x02000F7B RID: 3963
		public enum ThrustPhase
		{
			// Token: 0x04005E64 RID: 24164
			Accelerating,
			// Token: 0x04005E65 RID: 24165
			Decelerating,
			// Token: 0x04005E66 RID: 24166
			Coasting,
			// Token: 0x04005E67 RID: 24167
			Loitering,
			// Token: 0x04005E68 RID: 24168
			ContinuePreviousBurn
		}

		// Token: 0x02000F7C RID: 3964
		public interface IPatchSegment
		{
			// Token: 0x1700121A RID: 4634
			// (get) Token: 0x06007E5F RID: 32351
			// (set) Token: 0x06007E60 RID: 32352
			TIDateTime startTime { get; set; }

			// Token: 0x1700121B RID: 4635
			// (get) Token: 0x06007E61 RID: 32353
			TINaturalSpaceObjectState barycenter { get; }

			// Token: 0x06007E62 RID: 32354
			Vector3d GlobalPositionAtTime(TIDateTime timeToCheck);

			// Token: 0x06007E63 RID: 32355
			CartesianState GlobalCartesianStateAtTime(TIDateTime timeToCheck);

			// Token: 0x06007E64 RID: 32356
			double DVConsumedByTime(TIDateTime timeToCheck);

			// Token: 0x06007E65 RID: 32357
			Trajectory_Patched.ThrustPhase GetThrustPhaseAtTime(TIDateTime timeToCheck);

			// Token: 0x06007E66 RID: 32358
			OrbitalElementsState OrbitalElementsAtTime(TIDateTime time);

			// Token: 0x06007E67 RID: 32359
			double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter);

			// Token: 0x06007E68 RID: 32360
			bool isPlausible(IMobileAsset fleet);

			// Token: 0x1700121C RID: 4636
			// (get) Token: 0x06007E69 RID: 32361
			double boostDV_mps { get; }

			// Token: 0x1700121D RID: 4637
			// (get) Token: 0x06007E6A RID: 32362
			double decelDV_mps { get; }

			// Token: 0x1700121E RID: 4638
			// (get) Token: 0x06007E6B RID: 32363
			double DV_mps { get; }

			// Token: 0x1700121F RID: 4639
			// (get) Token: 0x06007E6C RID: 32364
			// (set) Token: 0x06007E6D RID: 32365
			bool isImpulse { get; set; }

			// Token: 0x17001220 RID: 4640
			// (get) Token: 0x06007E6E RID: 32366
			// (set) Token: 0x06007E6F RID: 32367
			bool isTorch { get; set; }

			// Token: 0x17001221 RID: 4641
			// (get) Token: 0x06007E70 RID: 32368
			// (set) Token: 0x06007E71 RID: 32369
			bool isOrbitPhasing { get; set; }

			// Token: 0x06007E72 RID: 32370
			string DumpSegment();

			// Token: 0x06007E73 RID: 32371
			string deepDump();

			// Token: 0x17001222 RID: 4642
			// (get) Token: 0x06007E74 RID: 32372
			// (set) Token: 0x06007E75 RID: 32373
			bool interruptible { get; set; }
		}

		// Token: 0x02000F7D RID: 3965
		public interface IPatchSegmentWithEndTime : Trajectory_Patched.IPatchSegment
		{
			// Token: 0x17001223 RID: 4643
			// (get) Token: 0x06007E76 RID: 32374
			// (set) Token: 0x06007E77 RID: 32375
			TIDateTime endTime { get; set; }
		}

		// Token: 0x02000F7E RID: 3966
		public interface ISupportsReducedCopy : Trajectory_Patched.IPatchSegmentWithEndTime, Trajectory_Patched.IPatchSegment
		{
			// Token: 0x06007E78 RID: 32376
			Trajectory_Patched.ISupportsReducedCopy ReducedCopy(TIDateTime newStartTime, TIDateTime newEndTime);
		}

		// Token: 0x02000F7F RID: 3967
		public class MicrothrustLERPSegment : Trajectory_Patched.MicrothrustSegment
		{
			// Token: 0x17001224 RID: 4644
			// (get) Token: 0x06007E79 RID: 32377 RVA: 0x00324DD6 File Offset: 0x00322FD6
			public override double DV_mps
			{
				get
				{
					return Mathd.Abs(base.endTime.DifferenceInSeconds(base.startTime) * ((this.trueFleetAccleration_mps2 != 0.0) ? this.trueFleetAccleration_mps2 : this.fleetCruiseAcceleration_mps2));
				}
			}

			// Token: 0x06007E7A RID: 32378 RVA: 0x00324E10 File Offset: 0x00323010
			public override OrbitalElementsState OrbitalElementsAtTime(TIDateTime timeToCheck)
			{
				OrbitalElementsState orbitalElementsState = base.OrbitalElementsAtTime(timeToCheck);
				double num = timeToCheck.DifferenceInSeconds(base.startTime) / base.endTime.DifferenceInSeconds(this.epochTime);
				orbitalElementsState.eccentricity = Mathd.Lerp(this.eccentricity, this.endEccentricity, num);
				double num2 = Mathd.BerpRadians(this.ascendingNode_rad, this.endAscendingNode_rad, num);
				orbitalElementsState.longAscendingNode_Rad = num2;
				orbitalElementsState.inclination_Rad = Mathd.LerpRadians(this.inclination_rad, this.endInclination_rad, num);
				double num3 = Mathd.BerpRadians(this.argP_rad, this.endArgP_rad, num);
				orbitalElementsState.argPeriapsis_Rad = num3;
				double num4 = Mathd.BerpRadians(this.startAnomalyCorrection_rad, this.endAnomalyCorrection_rad, num);
				double num5 = num2 - this.ascendingNode_rad + num3 - this.argP_rad;
				orbitalElementsState.meanAnomalyAtEpoch_Rad += num4 - num5;
				double mu = base.barycenter.mu;
				double num6 = mu / (this.initialVelocity_mps * this.initialVelocity_mps);
				double num7 = this.initialVelocity_mps - this.fleetCruiseAcceleration_mps2 * base.endTime.DifferenceInSeconds(this.epochTime);
				double num8 = mu / (num7 * num7);
				double num9 = num6 + this.startRadiusCorrection_m;
				double num10 = num8 + this.endRadiusCorrection_m;
				double num11 = num9 / num6;
				double num12 = num10 / num8;
				orbitalElementsState.semiMajorAxis_m *= Mathd.Lerp(num11, num12, num);
				return orbitalElementsState;
			}

			// Token: 0x06007E7B RID: 32379 RVA: 0x00324F60 File Offset: 0x00323160
			public override double DVConsumedByTime(TIDateTime timeToCheck)
			{
				double num = base.endTime.DifferenceInSeconds(base.startTime);
				double num2 = Mathd.Clamp(timeToCheck.DifferenceInSeconds(base.startTime), 0.0, num);
				return Mathd.Abs(this.trueFleetAccleration_mps2 * num2);
			}

			// Token: 0x06007E7C RID: 32380 RVA: 0x00324FA8 File Offset: 0x003231A8
			public override Trajectory_Patched.ISupportsReducedCopy ReducedCopy(TIDateTime newStartTime, TIDateTime newEndTime)
			{
				if (newStartTime < base.startTime || newStartTime >= base.endTime)
				{
					string[] array = new string[7];
					array[0] = "MicrothrustSegment.ReducedCopy: newStartTime (";
					int num = 1;
					TIDateTime tidateTime = newStartTime;
					array[num] = ((tidateTime != null) ? tidateTime.ToString() : null);
					array[2] = ") out of bounds (";
					int num2 = 3;
					TIDateTime startTime = base.startTime;
					array[num2] = ((startTime != null) ? startTime.ToString() : null);
					array[4] = " - ";
					int num3 = 5;
					TIDateTime endTime = base.endTime;
					array[num3] = ((endTime != null) ? endTime.ToString() : null);
					array[6] = ").";
					Log.Error(string.Concat(array), Array.Empty<object>());
					newStartTime = base.startTime;
				}
				if (newEndTime <= newStartTime || newEndTime > base.endTime)
				{
					string[] array2 = new string[7];
					array2[0] = "MicrothrustSegment.ReducedCopy: newEndTime (";
					int num4 = 1;
					TIDateTime tidateTime2 = newEndTime;
					array2[num4] = ((tidateTime2 != null) ? tidateTime2.ToString() : null);
					array2[2] = ") out of bounds (";
					int num5 = 3;
					TIDateTime tidateTime3 = newStartTime;
					array2[num5] = ((tidateTime3 != null) ? tidateTime3.ToString() : null);
					array2[4] = " - ";
					int num6 = 5;
					TIDateTime endTime2 = base.endTime;
					array2[num6] = ((endTime2 != null) ? endTime2.ToString() : null);
					array2[6] = ").";
					Log.Error(string.Concat(array2), Array.Empty<object>());
					newEndTime = base.startTime;
				}
				Trajectory_Patched.MicrothrustLERPSegment microthrustLERPSegment = (Trajectory_Patched.MicrothrustLERPSegment)base.MemberwiseClone();
				microthrustLERPSegment.startTime = newStartTime;
				microthrustLERPSegment.endTime = newEndTime;
				return microthrustLERPSegment;
			}

			// Token: 0x06007E7D RID: 32381 RVA: 0x003250EC File Offset: 0x003232EC
			public override bool isPlausible(IMobileAsset fleet)
			{
				if (!base.isPlausible(fleet))
				{
					return false;
				}
				if (this.endEccentricity <= 1.0)
				{
					Log.Error("Microthrust LERP segment implausible: final eccentricity is " + this.endEccentricity.ToString() + " which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				double semiMajorAxis_m = this.OrbitalElementsAtTime(base.startTime).semiMajorAxis_m;
				if (semiMajorAxis_m <= 0.0)
				{
					Log.Error(string.Concat(new string[]
					{
						"Microthrust LERP segment implausible: initial semi major axis is negative: ",
						semiMajorAxis_m.ToString(),
						"m which implies a hyperbolic 'orbit'.  Raw start radius is ",
						(base.barycenter.mu / (this.initialVelocity_mps * this.initialVelocity_mps)).ToString(),
						"m and start radius correction is ",
						this.startRadiusCorrection_m.ToString(),
						"m."
					}), Array.Empty<object>());
					return false;
				}
				double semiMajorAxis_m2 = this.OrbitalElementsAtTime(base.endTime).semiMajorAxis_m;
				if (semiMajorAxis_m2 <= 0.0)
				{
					double num = this.initialVelocity_mps - this.fleetCruiseAcceleration_mps2 * base.endTime.DifferenceInSeconds(base.startTime);
					Log.Error(string.Concat(new string[]
					{
						"Microthrust LERP segment implausible: final semi major axis is negative: ",
						semiMajorAxis_m2.ToString(),
						"m which implies a hyperbolic 'orbit'.  Raw final radius is ",
						(base.barycenter.mu / (num * num)).ToString(),
						"m and end radius correction is ",
						this.endRadiusCorrection_m.ToString(),
						"m."
					}), Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007E7E RID: 32382 RVA: 0x00325278 File Offset: 0x00323478
			public override string DumpSegment()
			{
				string[] array = new string[8];
				array[0] = "microthrustLERP,";
				array[1] = base.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = base.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",";
				int num2 = 5;
				TIDateTime endTime = base.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[6] = ",";
				array[7] = this.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007E7F RID: 32383 RVA: 0x003252FC File Offset: 0x003234FC
			public override string deepDump()
			{
				string[] array = new string[47];
				array[0] = "microthrust LERP segment:\n     start time           = ";
				int num = 1;
				TIDateTime startTime = base.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     end time             = ";
				int num2 = 3;
				TIDateTime endTime = base.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[4] = "\n     epoch time           = ";
				int num3 = 5;
				TIDateTime epochTime = this.epochTime;
				array[num3] = ((epochTime != null) ? epochTime.ToString() : null);
				array[6] = "\n     barycenter           = ";
				int num4 = 7;
				TINaturalSpaceObjectState barycenter = base.barycenter;
				array[num4] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[8] = "\n     fleet cruise accel   = ";
				array[9] = this.fleetCruiseAcceleration_mps2.ToString();
				array[10] = "m/s2\n     true fleet accel     = ";
				array[11] = this.trueFleetAccleration_mps2.ToString();
				array[12] = "m/s2\n     is ascending?        = ";
				array[13] = base.isAscending.ToString();
				array[14] = "\n     initial velocity     = ";
				array[15] = this.initialVelocity_mps.ToString();
				array[16] = "m/s\n     initial mean anomaly = ";
				array[17] = this.initialMeanAnomaly_rad.ToString();
				array[18] = "rad\n     start radius correction     = ";
				array[19] = this.startRadiusCorrection_m.ToString();
				array[20] = "m\n     start eccentricity          = ";
				array[21] = this.eccentricity.ToString();
				array[22] = "\n     start inclination           = ";
				array[23] = this.inclination_rad.ToString();
				array[24] = "\n     start long ascending node   = ";
				array[25] = this.ascendingNode_rad.ToString();
				array[26] = "rad\n     start argument of periapsis = ";
				array[27] = this.argP_rad.ToString();
				array[28] = "rad\n     end radius correction     = ";
				array[29] = this.endRadiusCorrection_m.ToString();
				array[30] = "m\n     end eccentricity          = ";
				array[31] = this.endEccentricity.ToString();
				array[32] = "\n     end inclination           = ";
				array[33] = this.endInclination_rad.ToString();
				array[34] = "rad\n     end long ascending node   = ";
				array[35] = this.endAscendingNode_rad.ToString();
				array[36] = "rad\n     end argument of periapsis = ";
				array[37] = this.endArgP_rad.ToString();
				array[38] = "rad\n     anomaly correction Bezier:\n      start anomaly correction                     = ";
				array[39] = this.startAnomalyCorrection_rad.ToString();
				array[40] = "rad\n      start anomaly speed correction control point = ";
				array[41] = this.startAnomalySpeedCorrectionControlPoint_rad.ToString();
				array[42] = "rad\n      end anomaly speed correction control point   = ";
				array[43] = this.endAnomalySpeedCorrectionControlPoint_rad.ToString();
				array[44] = "rad\n      end anomaly correction                       = ";
				array[45] = this.endAnomalyCorrection_rad.ToString();
				array[46] = "rad\n";
				return string.Concat(array);
			}

			// Token: 0x04005E69 RID: 24169
			public double endEccentricity;

			// Token: 0x04005E6A RID: 24170
			public double endAscendingNode_rad;

			// Token: 0x04005E6B RID: 24171
			public double endInclination_rad;

			// Token: 0x04005E6C RID: 24172
			public double endArgP_rad;

			// Token: 0x04005E6D RID: 24173
			public double startRadiusCorrection_m;

			// Token: 0x04005E6E RID: 24174
			public double endRadiusCorrection_m;

			// Token: 0x04005E6F RID: 24175
			public double startAnomalyCorrection_rad;

			// Token: 0x04005E70 RID: 24176
			public double endAnomalyCorrection_rad;

			// Token: 0x04005E71 RID: 24177
			public double startAnomalySpeedCorrectionControlPoint_rad;

			// Token: 0x04005E72 RID: 24178
			public double endAnomalySpeedCorrectionControlPoint_rad;

			// Token: 0x04005E73 RID: 24179
			public double trueFleetAccleration_mps2;
		}

		// Token: 0x02000F80 RID: 3968
		public class MicrothrustSegment : Trajectory_Patched.IPatchSegmentWithEndTime, Trajectory_Patched.IPatchSegment, Trajectory_Patched.ISupportsReducedCopy
		{
			// Token: 0x17001225 RID: 4645
			// (get) Token: 0x06007E81 RID: 32385 RVA: 0x0032556D File Offset: 0x0032376D
			// (set) Token: 0x06007E82 RID: 32386 RVA: 0x00325575 File Offset: 0x00323775
			public TIDateTime startTime { get; set; }

			// Token: 0x17001226 RID: 4646
			// (get) Token: 0x06007E83 RID: 32387 RVA: 0x0032557E File Offset: 0x0032377E
			// (set) Token: 0x06007E84 RID: 32388 RVA: 0x00325586 File Offset: 0x00323786
			public TIDateTime endTime { get; set; }

			// Token: 0x17001227 RID: 4647
			// (get) Token: 0x06007E85 RID: 32389 RVA: 0x0032558F File Offset: 0x0032378F
			// (set) Token: 0x06007E86 RID: 32390 RVA: 0x00325597 File Offset: 0x00323797
			public TINaturalSpaceObjectState barycenter { get; set; }

			// Token: 0x17001228 RID: 4648
			// (get) Token: 0x06007E87 RID: 32391 RVA: 0x003255A0 File Offset: 0x003237A0
			public bool isAscending
			{
				get
				{
					return this.fleetCruiseAcceleration_mps2 > 0.0;
				}
			}

			// Token: 0x17001229 RID: 4649
			// (get) Token: 0x06007E88 RID: 32392 RVA: 0x003255B3 File Offset: 0x003237B3
			public double boostDV_mps
			{
				get
				{
					if (!this.isAscending)
					{
						return 0.0;
					}
					return this.DV_mps;
				}
			}

			// Token: 0x1700122A RID: 4650
			// (get) Token: 0x06007E89 RID: 32393 RVA: 0x003255CD File Offset: 0x003237CD
			public double decelDV_mps
			{
				get
				{
					if (!this.isAscending)
					{
						return this.DV_mps;
					}
					return 0.0;
				}
			}

			// Token: 0x1700122B RID: 4651
			// (get) Token: 0x06007E8A RID: 32394 RVA: 0x003255E7 File Offset: 0x003237E7
			public virtual double DV_mps
			{
				get
				{
					return Mathd.Abs(this.endTime.DifferenceInSeconds(this.startTime) * this.fleetCruiseAcceleration_mps2);
				}
			}

			// Token: 0x1700122C RID: 4652
			// (get) Token: 0x06007E8B RID: 32395 RVA: 0x00325606 File Offset: 0x00323806
			// (set) Token: 0x06007E8C RID: 32396 RVA: 0x0032560E File Offset: 0x0032380E
			public bool isImpulse { get; set; }

			// Token: 0x1700122D RID: 4653
			// (get) Token: 0x06007E8D RID: 32397 RVA: 0x00325617 File Offset: 0x00323817
			// (set) Token: 0x06007E8E RID: 32398 RVA: 0x0032561F File Offset: 0x0032381F
			public bool isTorch { get; set; }

			// Token: 0x1700122E RID: 4654
			// (get) Token: 0x06007E8F RID: 32399 RVA: 0x00325628 File Offset: 0x00323828
			// (set) Token: 0x06007E90 RID: 32400 RVA: 0x00325630 File Offset: 0x00323830
			public bool isOrbitPhasing { get; set; }

			// Token: 0x1700122F RID: 4655
			// (get) Token: 0x06007E91 RID: 32401 RVA: 0x00325639 File Offset: 0x00323839
			// (set) Token: 0x06007E92 RID: 32402 RVA: 0x00325641 File Offset: 0x00323841
			public bool interruptible { get; set; }

			// Token: 0x06007E93 RID: 32403 RVA: 0x0032564C File Offset: 0x0032384C
			public virtual Trajectory_Patched.ISupportsReducedCopy ReducedCopy(TIDateTime newStartTime, TIDateTime newEndTime)
			{
				if (newStartTime < this.startTime || newStartTime >= this.endTime)
				{
					string[] array = new string[7];
					array[0] = "MicrothrustSegment.ReducedCopy: newStartTime (";
					int num = 1;
					TIDateTime tidateTime = newStartTime;
					array[num] = ((tidateTime != null) ? tidateTime.ToString() : null);
					array[2] = ") out of bounds (";
					int num2 = 3;
					TIDateTime startTime = this.startTime;
					array[num2] = ((startTime != null) ? startTime.ToString() : null);
					array[4] = " - ";
					int num3 = 5;
					TIDateTime endTime = this.endTime;
					array[num3] = ((endTime != null) ? endTime.ToString() : null);
					array[6] = ").";
					Log.Error(string.Concat(array), Array.Empty<object>());
					newStartTime = this.startTime;
				}
				if (newEndTime <= newStartTime || newEndTime > this.endTime)
				{
					string[] array2 = new string[7];
					array2[0] = "MicrothrustSegment.ReducedCopy: newEndTime (";
					int num4 = 1;
					TIDateTime tidateTime2 = newEndTime;
					array2[num4] = ((tidateTime2 != null) ? tidateTime2.ToString() : null);
					array2[2] = ") out of bounds (";
					int num5 = 3;
					TIDateTime tidateTime3 = newStartTime;
					array2[num5] = ((tidateTime3 != null) ? tidateTime3.ToString() : null);
					array2[4] = " - ";
					int num6 = 5;
					TIDateTime endTime2 = this.endTime;
					array2[num6] = ((endTime2 != null) ? endTime2.ToString() : null);
					array2[6] = ").";
					Log.Error(string.Concat(array2), Array.Empty<object>());
					newEndTime = this.startTime;
				}
				Trajectory_Patched.MicrothrustSegment microthrustSegment = (Trajectory_Patched.MicrothrustSegment)base.MemberwiseClone();
				microthrustSegment.startTime = newStartTime;
				microthrustSegment.endTime = newEndTime;
				return microthrustSegment;
			}

			// Token: 0x06007E94 RID: 32404 RVA: 0x0032578E File Offset: 0x0032398E
			public Vector3d GlobalPositionAtTime(TIDateTime timeToCheck)
			{
				return this.GlobalCartesianStateAtTime(timeToCheck).position;
			}

			// Token: 0x06007E95 RID: 32405 RVA: 0x0032579C File Offset: 0x0032399C
			public virtual CartesianState GlobalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				CartesianState cartesianState = this.LocalCartesianStateAtTime(timeToCheck);
				Vector3d xzy = (this.barycenter.SpatialRotation * cartesianState.positionDisplay).xzy;
				Vector3d xzy2 = (this.barycenter.SpatialRotation * cartesianState.velocityDisplay).xzy;
				return this.barycenter.ToGlobalCartesianStateAtTime(timeToCheck) + new CartesianState(xzy, xzy2);
			}

			// Token: 0x06007E96 RID: 32406 RVA: 0x0032580C File Offset: 0x00323A0C
			public CartesianState CartesianStateAtTime(TIDateTime timeToCheck, TISpaceObjectState barycenter)
			{
				CartesianState cartesianState = this.LocalCartesianStateAtTime(timeToCheck);
				if (this.barycenter == barycenter)
				{
					return cartesianState;
				}
				Vector3d xzy = (this.barycenter.SpatialRotation * cartesianState.positionDisplay).xzy;
				Vector3d xzy2 = (this.barycenter.SpatialRotation * cartesianState.velocityDisplay).xzy;
				if (barycenter == this.barycenter.barycenter)
				{
					return this.barycenter.ToLocalCartesianStateAtTime(timeToCheck) + new CartesianState(xzy, xzy2);
				}
				return this.barycenter.ToGlobalCartesianStateAtTime(timeToCheck) + new CartesianState(xzy, xzy2);
			}

			// Token: 0x06007E97 RID: 32407 RVA: 0x003258B8 File Offset: 0x00323AB8
			public CartesianState LocalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				return this.OrbitalElementsAtTime(timeToCheck).ToCartesianStateAtTime(timeToCheck.ExportTime(), this.barycenter.mass_kg);
			}

			// Token: 0x06007E98 RID: 32408 RVA: 0x003258E8 File Offset: 0x00323AE8
			public virtual OrbitalElementsState OrbitalElementsAtTime(TIDateTime timeToCheck)
			{
				double num = timeToCheck.DifferenceInSeconds(this.epochTime);
				double mu = this.barycenter.mu;
				double num2 = this.initialVelocity_mps - this.fleetCruiseAcceleration_mps2 * num;
				double num3 = mu / (num2 * num2);
				return new OrbitalElementsState
				{
					meanAnomalyAtEpoch_Rad = this.initialMeanAnomaly_rad + (this.FourthPower(this.initialVelocity_mps) - this.FourthPower(num2)) / (4.0 * this.fleetCruiseAcceleration_mps2 * mu),
					semiMajorAxis_m = num3,
					eccentricity = this.eccentricity,
					inclination_Rad = this.inclination_rad,
					longAscendingNode_Rad = this.ascendingNode_rad,
					argPeriapsis_Rad = this.argP_rad,
					epoch = timeToCheck.ExportTime()
				};
			}

			// Token: 0x06007E99 RID: 32409 RVA: 0x003259AC File Offset: 0x00323BAC
			public double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
			{
				double num = timeToCheck.DifferenceInSeconds(this.epochTime);
				barycenter = this.barycenter;
				double num2 = this.initialVelocity_mps - this.fleetCruiseAcceleration_mps2 * num;
				return barycenter.mu / (num2 * num2);
			}

			// Token: 0x06007E9A RID: 32410 RVA: 0x003259EC File Offset: 0x00323BEC
			public virtual double DVConsumedByTime(TIDateTime timeToCheck)
			{
				double num = this.endTime.DifferenceInSeconds(this.startTime);
				double num2 = Mathd.Clamp(timeToCheck.DifferenceInSeconds(this.startTime), 0.0, num);
				return Mathd.Abs(this.fleetCruiseAcceleration_mps2 * num2);
			}

			// Token: 0x06007E9B RID: 32411 RVA: 0x00325A34 File Offset: 0x00323C34
			public Trajectory_Patched.ThrustPhase GetThrustPhaseAtTime(TIDateTime timeToCheck)
			{
				if (this.isAscending)
				{
					return Trajectory_Patched.ThrustPhase.Accelerating;
				}
				return Trajectory_Patched.ThrustPhase.Decelerating;
			}

			// Token: 0x06007E9C RID: 32412 RVA: 0x00325A44 File Offset: 0x00323C44
			public virtual bool isPlausible(IMobileAsset fleet)
			{
				if (this.eccentricity <= 0.0)
				{
					Log.Error("Microthrust segment implausible: eccentricity is " + this.eccentricity.ToString() + " which implies a hyperbolic 'orbit'.", Array.Empty<object>());
				}
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				double distFromBarycenterAtTime_m = this.getDistFromBarycenterAtTime_m(this.startTime, out tinaturalSpaceObjectState);
				double num = Mathd.Abs(this.getDistFromBarycenterAtTime_m(new TIDateTime(this.startTime, 1.0), out tinaturalSpaceObjectState) - distFromBarycenterAtTime_m);
				double num2 = Mathd.Sqrt(this.barycenter.mu / distFromBarycenterAtTime_m);
				if (num / num2 > 1.0)
				{
					Log.Error(string.Concat(new string[]
					{
						"Microthrust segment implausible: initial vertical motion is ",
						num.ToString(),
						"mps and horizontal motion is ",
						num2.ToString(),
						"mps with a ratio of ",
						(num / num2).ToString(),
						" which exceeds the maximum plausible of ",
						1.0.ToString(),
						"."
					}), Array.Empty<object>());
					return false;
				}
				double distFromBarycenterAtTime_m2 = this.getDistFromBarycenterAtTime_m(this.endTime, out tinaturalSpaceObjectState);
				double distFromBarycenterAtTime_m3 = this.getDistFromBarycenterAtTime_m(new TIDateTime(this.endTime, -1.0), out tinaturalSpaceObjectState);
				double num3 = Mathd.Abs(distFromBarycenterAtTime_m2 - distFromBarycenterAtTime_m3);
				double num4 = Mathd.Sqrt(this.barycenter.mu / distFromBarycenterAtTime_m2);
				if (num3 / num4 > 1.0)
				{
					Log.Error(string.Concat(new string[]
					{
						"Microthrust segment implausible: final vertical motion is ",
						num3.ToString(),
						"mps and horizontal motion is ",
						num4.ToString(),
						"mps with a ratio of ",
						(num3 / num4).ToString(),
						"which exceeds the maximum plausible of ",
						1.0.ToString(),
						"."
					}), Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007E9D RID: 32413 RVA: 0x00325C2C File Offset: 0x00323E2C
			public virtual string DumpSegment()
			{
				string[] array = new string[8];
				array[0] = "microthrust,";
				array[1] = this.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",";
				int num2 = 5;
				TIDateTime endTime = this.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[6] = ",";
				array[7] = this.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007E9E RID: 32414 RVA: 0x00325CB0 File Offset: 0x00323EB0
			public virtual string deepDump()
			{
				string[] array = new string[25];
				array[0] = "microthrust segment:\n     start time            = ";
				int num = 1;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     end time              = ";
				int num2 = 3;
				TIDateTime endTime = this.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[4] = "\n     epoch time            = ";
				int num3 = 5;
				TIDateTime tidateTime = this.epochTime;
				array[num3] = ((tidateTime != null) ? tidateTime.ToString() : null);
				array[6] = "\n     barycenter            = ";
				int num4 = 7;
				TINaturalSpaceObjectState barycenter = this.barycenter;
				array[num4] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[8] = "\n     fleet cruise accel    = ";
				array[9] = this.fleetCruiseAcceleration_mps2.ToString();
				array[10] = "m/s2\n     is ascending?         = ";
				array[11] = this.isAscending.ToString();
				array[12] = "\n     initial velocity      = ";
				array[13] = this.initialVelocity_mps.ToString();
				array[14] = "m/s\n     initial mean anomaly  = ";
				array[15] = this.initialMeanAnomaly_rad.ToString();
				array[16] = "rad\n     eccentricity          = ";
				array[17] = this.eccentricity.ToString();
				array[18] = "\n     inclination           = ";
				array[19] = this.inclination_rad.ToString();
				array[20] = "rad\n     long ascending node   = ";
				array[21] = this.ascendingNode_rad.ToString();
				array[22] = "rad\n     argument of periapsis = ";
				array[23] = this.argP_rad.ToString();
				array[24] = "rad\n";
				return string.Concat(array);
			}

			// Token: 0x06007E9F RID: 32415 RVA: 0x00325E11 File Offset: 0x00324011
			private double FourthPower(double x)
			{
				return x * x * x * x;
			}

			// Token: 0x04005E76 RID: 24182
			public TIDateTime epochTime;

			// Token: 0x04005E78 RID: 24184
			public double eccentricity;

			// Token: 0x04005E79 RID: 24185
			public double ascendingNode_rad;

			// Token: 0x04005E7A RID: 24186
			public double inclination_rad;

			// Token: 0x04005E7B RID: 24187
			public double argP_rad;

			// Token: 0x04005E7C RID: 24188
			public double initialVelocity_mps;

			// Token: 0x04005E7D RID: 24189
			public double initialMeanAnomaly_rad;

			// Token: 0x04005E7E RID: 24190
			public double fleetCruiseAcceleration_mps2;
		}

		// Token: 0x02000F81 RID: 3969
		public class OrbitLERPSegment : Trajectory_Patched.IPatchSegmentWithEndTime, Trajectory_Patched.IPatchSegment
		{
			// Token: 0x17001230 RID: 4656
			// (get) Token: 0x06007EA1 RID: 32417 RVA: 0x00325E22 File Offset: 0x00324022
			// (set) Token: 0x06007EA2 RID: 32418 RVA: 0x00325E2A File Offset: 0x0032402A
			public TIDateTime startTime { get; set; }

			// Token: 0x17001231 RID: 4657
			// (get) Token: 0x06007EA3 RID: 32419 RVA: 0x00325E33 File Offset: 0x00324033
			// (set) Token: 0x06007EA4 RID: 32420 RVA: 0x00325E3B File Offset: 0x0032403B
			public TIDateTime endTime { get; set; }

			// Token: 0x17001232 RID: 4658
			// (get) Token: 0x06007EA5 RID: 32421 RVA: 0x00325E44 File Offset: 0x00324044
			// (set) Token: 0x06007EA6 RID: 32422 RVA: 0x00325E4C File Offset: 0x0032404C
			public TINaturalSpaceObjectState barycenter { get; set; }

			// Token: 0x17001233 RID: 4659
			// (get) Token: 0x06007EA7 RID: 32423 RVA: 0x00325E55 File Offset: 0x00324055
			public double boostDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x17001234 RID: 4660
			// (get) Token: 0x06007EA8 RID: 32424 RVA: 0x00325E60 File Offset: 0x00324060
			public double decelDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x17001235 RID: 4661
			// (get) Token: 0x06007EA9 RID: 32425 RVA: 0x00325E6B File Offset: 0x0032406B
			public double DV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x17001236 RID: 4662
			// (get) Token: 0x06007EAA RID: 32426 RVA: 0x00325E76 File Offset: 0x00324076
			// (set) Token: 0x06007EAB RID: 32427 RVA: 0x00325E7E File Offset: 0x0032407E
			public bool isImpulse { get; set; }

			// Token: 0x17001237 RID: 4663
			// (get) Token: 0x06007EAC RID: 32428 RVA: 0x00325E87 File Offset: 0x00324087
			// (set) Token: 0x06007EAD RID: 32429 RVA: 0x00325E8F File Offset: 0x0032408F
			public bool isTorch { get; set; }

			// Token: 0x17001238 RID: 4664
			// (get) Token: 0x06007EAE RID: 32430 RVA: 0x00325E98 File Offset: 0x00324098
			// (set) Token: 0x06007EAF RID: 32431 RVA: 0x00325EA0 File Offset: 0x003240A0
			public bool isOrbitPhasing { get; set; }

			// Token: 0x17001239 RID: 4665
			// (get) Token: 0x06007EB0 RID: 32432 RVA: 0x00325EA9 File Offset: 0x003240A9
			// (set) Token: 0x06007EB1 RID: 32433 RVA: 0x00325EAC File Offset: 0x003240AC
			public bool interruptible
			{
				get
				{
					return true;
				}
				set
				{
				}
			}

			// Token: 0x06007EB2 RID: 32434 RVA: 0x00325EAE File Offset: 0x003240AE
			public Vector3d GlobalPositionAtTime(TIDateTime timeToCheck)
			{
				return this.GlobalCartesianStateAtTime(timeToCheck).position;
			}

			// Token: 0x06007EB3 RID: 32435 RVA: 0x00325EBC File Offset: 0x003240BC
			public CartesianState GlobalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				return this.OrbitalElementsAtTime(timeToCheck).ToCartesianStateAtTime(timeToCheck.ExportTime(), this.barycenter.mass_kg).ToGlobal(this.barycenter, timeToCheck);
			}

			// Token: 0x06007EB4 RID: 32436 RVA: 0x00325EF8 File Offset: 0x003240F8
			public CartesianState CartesianStateAtTime(TIDateTime timeToCheck, TISpaceObjectState barycenter)
			{
				CartesianState cartesianState = this.OrbitalElementsAtTime(timeToCheck).ToCartesianStateAtTime(timeToCheck.ExportTime(), barycenter.mass_kg);
				if (barycenter == this.barycenter)
				{
					return cartesianState;
				}
				return cartesianState.ChangeReferenceFrame(this.barycenter, barycenter, timeToCheck);
			}

			// Token: 0x06007EB5 RID: 32437 RVA: 0x00325F40 File Offset: 0x00324140
			public CartesianState TrueCartesianStateAtTime(TIDateTime timeToCheck, TISpaceObjectState barycenter)
			{
				TIDateTime tidateTime = new TIDateTime(timeToCheck, -0.5);
				TIDateTime tidateTime2 = new TIDateTime(timeToCheck, 0.5);
				Vector3d position = this.CartesianStateAtTime(timeToCheck, barycenter).position;
				Vector3d vector3d = this.CartesianStateAtTime(tidateTime2, barycenter).position - this.CartesianStateAtTime(tidateTime, barycenter).position;
				CartesianState cartesianState = new CartesianState(position, vector3d);
				if (barycenter == this.barycenter)
				{
					return cartesianState;
				}
				return cartesianState.ChangeReferenceFrame(this.barycenter, barycenter, timeToCheck);
			}

			// Token: 0x06007EB6 RID: 32438 RVA: 0x00325FC4 File Offset: 0x003241C4
			public CartesianState TrueGlobalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				TIDateTime tidateTime = new TIDateTime(timeToCheck, -0.5);
				TIDateTime tidateTime2 = new TIDateTime(timeToCheck, 0.5);
				Vector3d position = this.CartesianStateAtTime(timeToCheck, this.barycenter).position;
				Vector3d vector3d = this.CartesianStateAtTime(tidateTime2, this.barycenter).position - this.CartesianStateAtTime(tidateTime, this.barycenter).position;
				CartesianState cartesianState = new CartesianState(position, vector3d);
				return cartesianState.ToGlobal(this.barycenter, timeToCheck);
			}

			// Token: 0x06007EB7 RID: 32439 RVA: 0x00326048 File Offset: 0x00324248
			public OrbitalElementsState OrbitalElementsAtTime(TIDateTime time)
			{
				double num = Mathd.Clamp01(time.DifferenceInSeconds(this.startTime) / this.endTime.DifferenceInSeconds(this.startTime));
				return new OrbitalElementsState
				{
					epoch = this.initialOrbit.epoch,
					longAscendingNode_Rad = Mathd.BerpRadians(this.initialOrbit.longAscendingNode_Rad, this.finalOrbit.longAscendingNode_Rad, num),
					argPeriapsis_Rad = Mathd.BerpRadians(this.initialOrbit.argPeriapsis_Rad, this.finalOrbit.argPeriapsis_Rad, num),
					inclination_Rad = Mathd.BerpRadians(this.initialOrbit.inclination_Rad, this.finalOrbit.inclination_Rad, num),
					semiMajorAxis_m = Mathd.Berp(this.initialOrbit.semiMajorAxis_m, this.finalOrbit.semiMajorAxis_m, num),
					eccentricity = Mathd.Berp(this.initialOrbit.eccentricity, this.finalOrbit.eccentricity, num),
					meanAnomalyAtEpoch_Rad = Mathd.Berp(this.initialOrbit.meanAnomalyAtEpoch_Rad, this.finalOrbit.meanAnomalyAtEpoch_Rad, num)
				};
			}

			// Token: 0x06007EB8 RID: 32440 RVA: 0x00326166 File Offset: 0x00324366
			public double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
			{
				barycenter = this.barycenter;
				return this.OrbitalElementsAtTime(timeToCheck).semiMajorAxis_m;
			}

			// Token: 0x06007EB9 RID: 32441 RVA: 0x0032617C File Offset: 0x0032437C
			public double DVConsumedByTime(TIDateTime timeToCheck)
			{
				return 0.0;
			}

			// Token: 0x06007EBA RID: 32442 RVA: 0x00326187 File Offset: 0x00324387
			public Trajectory_Patched.ThrustPhase GetThrustPhaseAtTime(TIDateTime timeToCheck)
			{
				return Trajectory_Patched.ThrustPhase.Coasting;
			}

			// Token: 0x06007EBB RID: 32443 RVA: 0x0032618C File Offset: 0x0032438C
			public bool isPlausible(IMobileAsset fleet)
			{
				if (this.initialOrbit.eccentricity >= 1.0)
				{
					Log.Error("Orbit LERP segment implausible: initial orbit's eccentricity is " + this.initialOrbit.eccentricity.ToString() + " which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				if (this.finalOrbit.eccentricity >= 1.0)
				{
					Log.Error("Orbit LERP segment implausible: final orbit's eccentricity is " + this.finalOrbit.eccentricity.ToString() + " which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				if (this.initialOrbit.semiMajorAxis_m <= 0.0)
				{
					Log.Error("Orbit LERP segment implausible: intial orbit's semi major axis is " + this.initialOrbit.semiMajorAxis_m.ToString() + "m which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				if (this.finalOrbit.semiMajorAxis_m <= 0.0)
				{
					Log.Error("Orbit LERP segment implausible: final orbit's semi major axis is " + this.finalOrbit.semiMajorAxis_m.ToString() + "m which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007EBC RID: 32444 RVA: 0x003262A0 File Offset: 0x003244A0
			public string DumpSegment()
			{
				string[] array = new string[8];
				array[0] = "orbitLERP,";
				array[1] = this.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",";
				int num2 = 5;
				TIDateTime endTime = this.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[6] = ",";
				array[7] = this.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007EBD RID: 32445 RVA: 0x00326324 File Offset: 0x00324524
			public virtual string deepDump()
			{
				string[] array = new string[37];
				array[0] = "orbit LERP segment:\n     barycenter = ";
				int num = 1;
				TINaturalSpaceObjectState barycenter = this.barycenter;
				array[num] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[2] = "\n     start time = ";
				int num2 = 3;
				TIDateTime startTime = this.startTime;
				array[num2] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = "\n     end time   = ";
				int num3 = 5;
				TIDateTime endTime = this.endTime;
				array[num3] = ((endTime != null) ? endTime.ToString() : null);
				array[6] = "\n     barycenter = ";
				array[7] = this.barycenter.displayName;
				array[8] = "\n     start semi major axis       = ";
				array[9] = this.initialOrbit.semiMajorAxis_m.ToString();
				array[10] = "m\n     start eccentricity          = ";
				array[11] = this.initialOrbit.eccentricity.ToString();
				array[12] = "\n     start long ascending node   = ";
				array[13] = this.initialOrbit.longAscendingNode_Rad.ToString();
				array[14] = "rad\n     start inclination           = ";
				array[15] = this.initialOrbit.inclination_Rad.ToString();
				array[16] = "rad\n     start arg periapsis         = ";
				array[17] = this.initialOrbit.argPeriapsis_Rad.ToString();
				array[18] = "rad\n     start mean anomaly at epoch = ";
				array[19] = this.initialOrbit.meanAnomalyAtEpoch_Rad.ToString();
				array[20] = "rad\n     start epoch                 = ";
				array[21] = this.initialOrbit.epoch.ToString();
				array[22] = "\n     end semi major axis       = ";
				array[23] = this.finalOrbit.semiMajorAxis_m.ToString();
				array[24] = "m\n     end eccentricity          = ";
				array[25] = this.finalOrbit.eccentricity.ToString();
				array[26] = "\n     end long ascending node   = ";
				array[27] = this.finalOrbit.longAscendingNode_Rad.ToString();
				array[28] = "rad\n     end inclination           = ";
				array[29] = this.finalOrbit.inclination_Rad.ToString();
				array[30] = "rad\n     end arg periapsis         = ";
				array[31] = this.finalOrbit.argPeriapsis_Rad.ToString();
				array[32] = "rad\n     end mean anomaly at epoch = ";
				array[33] = this.finalOrbit.meanAnomalyAtEpoch_Rad.ToString();
				array[34] = "rad\n     end epoch                 = ";
				array[35] = this.finalOrbit.epoch.ToString();
				array[36] = "\n";
				return string.Concat(array);
			}

			// Token: 0x04005E85 RID: 24197
			public OrbitalElementsState initialOrbit;

			// Token: 0x04005E86 RID: 24198
			public OrbitalElementsState finalOrbit;
		}

		// Token: 0x02000F82 RID: 3970
		public class HyperbolicOrbitSegment : Trajectory_Patched.OrbitSegment
		{
			// Token: 0x06007EBF RID: 32447 RVA: 0x0032655C File Offset: 0x0032475C
			public override double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
			{
				barycenter = base.barycenter;
				return base.CartesianStateAtTime(timeToCheck, base.barycenter).position.magnitude;
			}

			// Token: 0x06007EC0 RID: 32448 RVA: 0x0032658C File Offset: 0x0032478C
			public override bool isPlausible(IMobileAsset fleet)
			{
				if (this.orbit.eccentricity <= 1.0)
				{
					Log.Error("Hyperbolic 'orbit' segment implausible: eccentricity was " + this.orbit.eccentricity.ToString() + " which implies that the trajectory is elliptical.", Array.Empty<object>());
					return false;
				}
				if (this.orbit.semiMajorAxis_m >= 0.0)
				{
					Log.Error("Hyperbolic 'orbit' segment implausible: semi major axis was " + this.orbit.semiMajorAxis_m.ToString() + "m which implies that the trajectory is elliptical.", Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007EC1 RID: 32449 RVA: 0x0032661C File Offset: 0x0032481C
			public override string DumpSegment()
			{
				string[] array = new string[6];
				array[0] = "hyperbolic orbit,";
				array[1] = base.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = base.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",unknownEndTime,";
				array[5] = base.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007EC2 RID: 32450 RVA: 0x00326680 File Offset: 0x00324880
			public override string deepDump()
			{
				string[] array = new string[19];
				array[0] = "hyperbolic orbit segment:\n     start time = ";
				int num = 1;
				TIDateTime startTime = base.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     barycenter = ";
				int num2 = 3;
				TINaturalSpaceObjectState barycenter = base.barycenter;
				array[num2] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[4] = "\n     semi major axis       = ";
				array[5] = this.orbit.semiMajorAxis_m.ToString();
				array[6] = "m\n     eccentricity          = ";
				array[7] = this.orbit.eccentricity.ToString();
				array[8] = "\n     long ascending node   = ";
				array[9] = this.orbit.longAscendingNode_Rad.ToString();
				array[10] = "rad\n     inclination           = ";
				array[11] = this.orbit.inclination_Rad.ToString();
				array[12] = "rad\n     arg periapsis         = ";
				array[13] = this.orbit.argPeriapsis_Rad.ToString();
				array[14] = "rad\n     mean anomaly at epoch = ";
				array[15] = this.orbit.meanAnomalyAtEpoch_Rad.ToString();
				array[16] = "rad\n     epoch                 = ";
				array[17] = this.orbit.epoch.ToString();
				array[18] = "\n";
				return string.Concat(array);
			}
		}

		// Token: 0x02000F83 RID: 3971
		public class OrbitSegment : Trajectory_Patched.IPatchSegment
		{
			// Token: 0x1700123A RID: 4666
			// (get) Token: 0x06007EC4 RID: 32452 RVA: 0x003267B3 File Offset: 0x003249B3
			// (set) Token: 0x06007EC5 RID: 32453 RVA: 0x003267BB File Offset: 0x003249BB
			public TIDateTime startTime { get; set; }

			// Token: 0x1700123B RID: 4667
			// (get) Token: 0x06007EC6 RID: 32454 RVA: 0x003267C4 File Offset: 0x003249C4
			// (set) Token: 0x06007EC7 RID: 32455 RVA: 0x003267CC File Offset: 0x003249CC
			public TINaturalSpaceObjectState barycenter { get; set; }

			// Token: 0x1700123C RID: 4668
			// (get) Token: 0x06007EC8 RID: 32456 RVA: 0x003267D5 File Offset: 0x003249D5
			public double boostDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x1700123D RID: 4669
			// (get) Token: 0x06007EC9 RID: 32457 RVA: 0x003267E0 File Offset: 0x003249E0
			public double decelDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x1700123E RID: 4670
			// (get) Token: 0x06007ECA RID: 32458 RVA: 0x003267EB File Offset: 0x003249EB
			public double DV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x1700123F RID: 4671
			// (get) Token: 0x06007ECB RID: 32459 RVA: 0x003267F6 File Offset: 0x003249F6
			// (set) Token: 0x06007ECC RID: 32460 RVA: 0x003267FE File Offset: 0x003249FE
			public bool isImpulse { get; set; }

			// Token: 0x17001240 RID: 4672
			// (get) Token: 0x06007ECD RID: 32461 RVA: 0x00326807 File Offset: 0x00324A07
			// (set) Token: 0x06007ECE RID: 32462 RVA: 0x0032680F File Offset: 0x00324A0F
			public bool isTorch { get; set; }

			// Token: 0x17001241 RID: 4673
			// (get) Token: 0x06007ECF RID: 32463 RVA: 0x00326818 File Offset: 0x00324A18
			// (set) Token: 0x06007ED0 RID: 32464 RVA: 0x00326820 File Offset: 0x00324A20
			public bool isOrbitPhasing { get; set; }

			// Token: 0x17001242 RID: 4674
			// (get) Token: 0x06007ED1 RID: 32465 RVA: 0x00326829 File Offset: 0x00324A29
			// (set) Token: 0x06007ED2 RID: 32466 RVA: 0x0032682C File Offset: 0x00324A2C
			public bool interruptible
			{
				get
				{
					return true;
				}
				set
				{
				}
			}

			// Token: 0x06007ED3 RID: 32467 RVA: 0x0032682E File Offset: 0x00324A2E
			public Vector3d GlobalPositionAtTime(TIDateTime timeToCheck)
			{
				return this.GlobalCartesianStateAtTime(timeToCheck).position;
			}

			// Token: 0x06007ED4 RID: 32468 RVA: 0x0032683C File Offset: 0x00324A3C
			public CartesianState GlobalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				return this.orbit.ToCartesianStateAtTime(timeToCheck.ExportTime(), this.barycenter.mass_kg).ToGlobal(this.barycenter, timeToCheck);
			}

			// Token: 0x06007ED5 RID: 32469 RVA: 0x00326874 File Offset: 0x00324A74
			public CartesianState CartesianStateAtTime(TIDateTime timeToCheck, TISpaceObjectState barycenter)
			{
				CartesianState cartesianState = this.orbit.ToCartesianStateAtTime(timeToCheck.ExportTime(), barycenter.mass_kg);
				if (barycenter == this.barycenter)
				{
					return cartesianState;
				}
				return cartesianState.ChangeReferenceFrame(this.barycenter, barycenter, timeToCheck);
			}

			// Token: 0x06007ED6 RID: 32470 RVA: 0x003268B8 File Offset: 0x00324AB8
			public OrbitalElementsState OrbitalElementsAtTime(TIDateTime _)
			{
				return new OrbitalElementsState(this.orbit);
			}

			// Token: 0x06007ED7 RID: 32471 RVA: 0x003268C5 File Offset: 0x00324AC5
			public virtual double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
			{
				barycenter = this.barycenter;
				return this.orbit.semiMajorAxis_m;
			}

			// Token: 0x06007ED8 RID: 32472 RVA: 0x003268DA File Offset: 0x00324ADA
			public double DVConsumedByTime(TIDateTime timeToCheck)
			{
				return 0.0;
			}

			// Token: 0x06007ED9 RID: 32473 RVA: 0x003268E5 File Offset: 0x00324AE5
			public Trajectory_Patched.ThrustPhase GetThrustPhaseAtTime(TIDateTime timeToCheck)
			{
				return Trajectory_Patched.ThrustPhase.Coasting;
			}

			// Token: 0x06007EDA RID: 32474 RVA: 0x003268E8 File Offset: 0x00324AE8
			public virtual bool isPlausible(IMobileAsset fleet)
			{
				if (this.orbit.eccentricity >= 1.0)
				{
					Log.Error("Orbit segment implausible: eccentricity was " + this.orbit.eccentricity.ToString() + " which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				if (this.orbit.semiMajorAxis_m <= 0.0)
				{
					Log.Error("Orbit segment implausible: semi major axis was " + this.orbit.semiMajorAxis_m.ToString() + "m which implies a hyperbolic 'orbit'.", Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007EDB RID: 32475 RVA: 0x00326978 File Offset: 0x00324B78
			public virtual string DumpSegment()
			{
				string[] array = new string[6];
				array[0] = "orbit,";
				array[1] = this.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",unknownEndTime,";
				array[5] = this.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007EDC RID: 32476 RVA: 0x003269DC File Offset: 0x00324BDC
			public virtual string deepDump()
			{
				string[] array = new string[19];
				array[0] = "orbit segment:\n     start time = ";
				int num = 1;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     barycenter = ";
				int num2 = 3;
				TINaturalSpaceObjectState barycenter = this.barycenter;
				array[num2] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[4] = "\n     semi major axis       = ";
				array[5] = this.orbit.semiMajorAxis_m.ToString();
				array[6] = "m\n     eccentricity          = ";
				array[7] = this.orbit.eccentricity.ToString();
				array[8] = "\n     long ascending node   = ";
				array[9] = this.orbit.longAscendingNode_Rad.ToString();
				array[10] = "rad\n     inclination           = ";
				array[11] = this.orbit.inclination_Rad.ToString();
				array[12] = "rad\n     arg periapsis         = ";
				array[13] = this.orbit.argPeriapsis_Rad.ToString();
				array[14] = "rad\n     mean anomaly at epoch = ";
				array[15] = this.orbit.meanAnomalyAtEpoch_Rad.ToString();
				array[16] = "rad\n     epoch                 = ";
				array[17] = this.orbit.epoch.ToString();
				array[18] = "\n";
				return string.Concat(array);
			}

			// Token: 0x04005E8D RID: 24205
			public OrbitalElementsState orbit;
		}

		// Token: 0x02000F84 RID: 3972
		public class BurnSegment : Trajectory_Patched.IPatchSegmentWithEndTime, Trajectory_Patched.IPatchSegment, Trajectory_Patched.ISupportsReducedCopy
		{
			// Token: 0x17001243 RID: 4675
			// (get) Token: 0x06007EDE RID: 32478 RVA: 0x00326B0F File Offset: 0x00324D0F
			// (set) Token: 0x06007EDF RID: 32479 RVA: 0x00326B17 File Offset: 0x00324D17
			public TIDateTime startTime { get; set; }

			// Token: 0x17001244 RID: 4676
			// (get) Token: 0x06007EE0 RID: 32480 RVA: 0x00326B20 File Offset: 0x00324D20
			// (set) Token: 0x06007EE1 RID: 32481 RVA: 0x00326B28 File Offset: 0x00324D28
			public bool isImpulse { get; set; }

			// Token: 0x17001245 RID: 4677
			// (get) Token: 0x06007EE2 RID: 32482 RVA: 0x00326B31 File Offset: 0x00324D31
			// (set) Token: 0x06007EE3 RID: 32483 RVA: 0x00326B39 File Offset: 0x00324D39
			public bool isTorch { get; set; }

			// Token: 0x17001246 RID: 4678
			// (get) Token: 0x06007EE4 RID: 32484 RVA: 0x00326B42 File Offset: 0x00324D42
			// (set) Token: 0x06007EE5 RID: 32485 RVA: 0x00326B4A File Offset: 0x00324D4A
			public bool isOrbitPhasing { get; set; }

			// Token: 0x17001247 RID: 4679
			// (get) Token: 0x06007EE6 RID: 32486 RVA: 0x00326B53 File Offset: 0x00324D53
			public virtual double boostDV_mps
			{
				get
				{
					if (!this.isBoost)
					{
						return 0.0;
					}
					return this.DV_mps;
				}
			}

			// Token: 0x17001248 RID: 4680
			// (get) Token: 0x06007EE7 RID: 32487 RVA: 0x00326B6D File Offset: 0x00324D6D
			public virtual double decelDV_mps
			{
				get
				{
					if (!this.isBoost)
					{
						return this.DV_mps;
					}
					return 0.0;
				}
			}

			// Token: 0x17001249 RID: 4681
			// (get) Token: 0x06007EE8 RID: 32488 RVA: 0x00326B87 File Offset: 0x00324D87
			public virtual double DV_mps
			{
				get
				{
					return this.burnDuration_s * this.fleetAccel_mps2;
				}
			}

			// Token: 0x1700124A RID: 4682
			// (get) Token: 0x06007EE9 RID: 32489 RVA: 0x00326B96 File Offset: 0x00324D96
			// (set) Token: 0x06007EEA RID: 32490 RVA: 0x00326B9E File Offset: 0x00324D9E
			public bool interruptible { get; set; }

			// Token: 0x1700124B RID: 4683
			// (get) Token: 0x06007EEB RID: 32491 RVA: 0x00326BA7 File Offset: 0x00324DA7
			// (set) Token: 0x06007EEC RID: 32492 RVA: 0x00326BC0 File Offset: 0x00324DC0
			public TIDateTime endTime
			{
				get
				{
					TIDateTime tidateTime = new TIDateTime(this.startTime);
					tidateTime.AddSeconds(this.burnDuration_s);
					return tidateTime;
				}
				set
				{
					this.burnDuration_s = value.DifferenceInSeconds(this.startTime);
				}
			}

			// Token: 0x1700124C RID: 4684
			// (get) Token: 0x06007EED RID: 32493 RVA: 0x00326BD4 File Offset: 0x00324DD4
			// (set) Token: 0x06007EEE RID: 32494 RVA: 0x00326BDC File Offset: 0x00324DDC
			public TINaturalSpaceObjectState barycenter { get; set; }

			// Token: 0x06007EEF RID: 32495 RVA: 0x00326BE8 File Offset: 0x00324DE8
			public Trajectory_Patched.ISupportsReducedCopy ReducedCopy(TIDateTime newStartTime, TIDateTime newEndTime)
			{
				TIDateTime startTime = this.startTime;
				TIDateTime endTime = this.endTime;
				if (newStartTime < startTime || newStartTime >= endTime)
				{
					string[] array = new string[7];
					array[0] = "BurnSegment: new start time (";
					int num = 1;
					TIDateTime tidateTime = newStartTime;
					array[num] = ((tidateTime != null) ? tidateTime.ToString() : null);
					array[2] = ") is out of bounds (";
					int num2 = 3;
					TIDateTime tidateTime2 = startTime;
					array[num2] = ((tidateTime2 != null) ? tidateTime2.ToString() : null);
					array[4] = " - ";
					int num3 = 5;
					TIDateTime tidateTime3 = endTime;
					array[num3] = ((tidateTime3 != null) ? tidateTime3.ToString() : null);
					array[6] = ").";
					Log.Error(string.Concat(array), Array.Empty<object>());
					newStartTime = startTime;
				}
				if (newEndTime <= newStartTime || newEndTime > endTime)
				{
					string[] array2 = new string[7];
					array2[0] = "BurnSegment: new start time (";
					int num4 = 1;
					TIDateTime tidateTime4 = newEndTime;
					array2[num4] = ((tidateTime4 != null) ? tidateTime4.ToString() : null);
					array2[2] = ") is out of bounds (";
					int num5 = 3;
					TIDateTime tidateTime5 = newStartTime;
					array2[num5] = ((tidateTime5 != null) ? tidateTime5.ToString() : null);
					array2[4] = " - ";
					int num6 = 5;
					TIDateTime tidateTime6 = endTime;
					array2[num6] = ((tidateTime6 != null) ? tidateTime6.ToString() : null);
					array2[6] = ").";
					Log.Error(string.Concat(array2), Array.Empty<object>());
					newEndTime = endTime;
				}
				double num7 = newStartTime.DifferenceInSeconds(startTime);
				double num8 = newEndTime.DifferenceInSeconds(startTime);
				BurnBezierDescription burnBezierDescription = this.burnDescription;
				CartesianState cartesianState = new CartesianState(burnBezierDescription.LocationInBurn(num7, this.burnDuration_s).xzy, burnBezierDescription.VelocityInBurn(num7, this.burnDuration_s).xzy);
				CartesianState cartesianState2 = new CartesianState(burnBezierDescription.LocationInBurn(num8, this.burnDuration_s).xzy, burnBezierDescription.VelocityInBurn(num8, this.burnDuration_s).xzy);
				double num9 = num8 - num7;
				BurnBezierDescription burnBezierDescription2 = new BurnBezierDescription(cartesianState, cartesianState2, num9);
				return new Trajectory_Patched.BurnSegment
				{
					startTime = newStartTime,
					fleetAccel_mps2 = this.fleetAccel_mps2,
					isBoost = this.isBoost,
					isImpulse = this.isImpulse,
					isTorch = this.isTorch,
					isOrbitPhasing = this.isOrbitPhasing,
					interruptible = this.interruptible,
					barycenter = this.barycenter,
					burnDuration_s = num9,
					burnDescription = burnBezierDescription2
				};
			}

			// Token: 0x06007EF0 RID: 32496 RVA: 0x00326DF8 File Offset: 0x00324FF8
			public CartesianState GlobalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				double num = timeToCheck.DifferenceInSeconds(this.startTime);
				CartesianState cartesianState = new CartesianState(this.burnDescription.LocationInBurn(num, this.burnDuration_s), this.burnDescription.VelocityInBurn(num, this.burnDuration_s));
				return (this.barycenter.SpatialRotation * cartesianState).xzy + this.barycenter.ToGlobalCartesianStateAtTime(timeToCheck);
			}

			// Token: 0x06007EF1 RID: 32497 RVA: 0x00326E68 File Offset: 0x00325068
			public Vector3d GlobalPositionAtTime(TIDateTime timeToCheck)
			{
				double num = timeToCheck.DifferenceInSeconds(this.startTime);
				Vector3d vector3d = this.burnDescription.LocationInBurn(num, this.burnDuration_s);
				return (this.barycenter.SpatialRotation * vector3d).xzy + this.barycenter.ToGlobalCartesianStateAtTime(timeToCheck).position;
			}

			// Token: 0x06007EF2 RID: 32498 RVA: 0x00326EC4 File Offset: 0x003250C4
			public virtual double DVConsumedByTime(TIDateTime timeToCheck)
			{
				double num = Mathd.Clamp(timeToCheck.DifferenceInSeconds(this.startTime), 0.0, this.burnDuration_s);
				return this.fleetAccel_mps2 * num;
			}

			// Token: 0x06007EF3 RID: 32499 RVA: 0x00326EFA File Offset: 0x003250FA
			public Trajectory_Patched.ThrustPhase GetThrustPhaseAtTime(TIDateTime timeToCheck)
			{
				if (this.isBoost)
				{
					return Trajectory_Patched.ThrustPhase.Accelerating;
				}
				return Trajectory_Patched.ThrustPhase.Decelerating;
			}

			// Token: 0x06007EF4 RID: 32500 RVA: 0x00326F08 File Offset: 0x00325108
			public OrbitalElementsState OrbitalElementsAtTime(TIDateTime timeToCheck)
			{
				Mathd.Clamp(timeToCheck.DifferenceInSeconds(this.startTime), 0.0, this.burnDuration_s);
				return this.GlobalCartesianStateAtTime(timeToCheck).ToLocal(this.barycenter, timeToCheck).ToOrbitalElementsState(this.barycenter.mu, new DateTime?(timeToCheck.ExportTime()));
			}

			// Token: 0x06007EF5 RID: 32501 RVA: 0x00326F6C File Offset: 0x0032516C
			public double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
			{
				barycenter = this.barycenter;
				double num = timeToCheck.DifferenceInSeconds(this.startTime);
				return this.burnDescription.LocationInBurn(num, this.burnDuration_s).magnitude;
			}

			// Token: 0x06007EF6 RID: 32502 RVA: 0x00326FA8 File Offset: 0x003251A8
			public virtual bool isPlausible(IMobileAsset fleet)
			{
				double num = this.burnDescription.MaxAccelerationDuringBurn_mps2(this.burnDuration_s);
				if (num > (double)fleet.cruiseAcceleration_mps2 * 2.0)
				{
					Log.Error("Burn segment implausible: maximum acceleration is " + num.ToString() + "m/s2, which exceeds the fleet'scruise acceleration of " + fleet.cruiseAcceleration_mps2.ToString(), Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007EF7 RID: 32503 RVA: 0x0032700C File Offset: 0x0032520C
			public virtual string DumpSegment()
			{
				string[] array = new string[8];
				array[0] = "burn,";
				array[1] = this.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",";
				int num2 = 5;
				TIDateTime endTime = this.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[6] = ",";
				array[7] = this.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007EF8 RID: 32504 RVA: 0x00327090 File Offset: 0x00325290
			public virtual string deepDump()
			{
				string[] array = new string[10];
				array[0] = "burn segment:\n     start time = ";
				int num = 1;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     end time   = ";
				int num2 = 3;
				TIDateTime endTime = this.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[4] = "\n     barycenter = ";
				array[5] = this.barycenter.displayName;
				array[6] = "\n     duration   = ";
				array[7] = this.burnDuration_s.ToString();
				array[8] = "\n";
				array[9] = this.burnDescription.deepDump();
				return string.Concat(array);
			}

			// Token: 0x04005E92 RID: 24210
			public double burnDuration_s;

			// Token: 0x04005E93 RID: 24211
			public double fleetAccel_mps2;

			// Token: 0x04005E94 RID: 24212
			public bool isBoost;

			// Token: 0x04005E9A RID: 24218
			public BurnBezierDescription burnDescription;
		}

		// Token: 0x02000F85 RID: 3973
		public class FalseBurnSegment : Trajectory_Patched.BurnSegment
		{
			// Token: 0x1700124D RID: 4685
			// (get) Token: 0x06007EFA RID: 32506 RVA: 0x0032712E File Offset: 0x0032532E
			public override double DV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x1700124E RID: 4686
			// (get) Token: 0x06007EFB RID: 32507 RVA: 0x00327139 File Offset: 0x00325339
			public override double boostDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x1700124F RID: 4687
			// (get) Token: 0x06007EFC RID: 32508 RVA: 0x00327144 File Offset: 0x00325344
			public override double decelDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x06007EFD RID: 32509 RVA: 0x0032714F File Offset: 0x0032534F
			public override double DVConsumedByTime(TIDateTime timeToCheck)
			{
				return 0.0;
			}

			// Token: 0x06007EFE RID: 32510 RVA: 0x0032715C File Offset: 0x0032535C
			public override string DumpSegment()
			{
				string[] array = new string[6];
				array[0] = "false burn,";
				array[1] = base.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = base.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",";
				int num2 = 5;
				TIDateTime endTime = base.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				return string.Concat(array);
			}

			// Token: 0x06007EFF RID: 32511 RVA: 0x003271C4 File Offset: 0x003253C4
			public override bool isPlausible(IMobileAsset fleet)
			{
				double magnitude = this.burnDescription.LocationInBurn(0.0, 1.0).magnitude;
				double num = base.barycenter.localAccelerationDueToGravity_ms2(magnitude);
				double num2 = this.burnDescription.InitialAcceleration(this.burnDuration_s);
				if (num2 > num * 2.0)
				{
					Log.Error(string.Concat(new string[]
					{
						"False burn segment implausible: the initial acceleration is ",
						num2.ToString(),
						"m/s2 but the acceleration from the local barycenter (",
						base.barycenter.displayName,
						") at that distance (",
						magnitude.ToString(),
						"m) is only ",
						num.ToString(),
						"m/s2."
					}), Array.Empty<object>());
					return false;
				}
				double magnitude2 = this.burnDescription.LocationInBurn(1.0, 1.0).magnitude;
				double num3 = base.barycenter.localAccelerationDueToGravity_ms2(magnitude2);
				double num4 = this.burnDescription.FinalAcceleration(this.burnDuration_s);
				if (num4 > num3 * 2.0)
				{
					Log.Error(string.Concat(new string[]
					{
						"False burn segment implausible: the final acceleration is ",
						num4.ToString(),
						"m/s2 but the acceleration from the local barycenter (",
						base.barycenter.displayName,
						") at that distance (",
						magnitude2.ToString(),
						"m) is only ",
						num3.ToString(),
						"m/s2."
					}), Array.Empty<object>());
					return false;
				}
				return true;
			}

			// Token: 0x06007F00 RID: 32512 RVA: 0x00327358 File Offset: 0x00325558
			public override string deepDump()
			{
				string[] array = new string[10];
				array[0] = "false burn segment:\n     start time = ";
				int num = 1;
				TIDateTime startTime = base.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     end time   = ";
				int num2 = 3;
				TIDateTime endTime = base.endTime;
				array[num2] = ((endTime != null) ? endTime.ToString() : null);
				array[4] = "\n     barycenter = ";
				array[5] = base.barycenter.displayName;
				array[6] = "\n     duration   = ";
				array[7] = this.burnDuration_s.ToString();
				array[8] = "\n";
				array[9] = this.burnDescription.deepDump();
				return string.Concat(array);
			}
		}

		// Token: 0x02000F86 RID: 3974
		public class TorchCoastSegment : Trajectory_Patched.IPatchSegment
		{
			// Token: 0x17001250 RID: 4688
			// (get) Token: 0x06007F02 RID: 32514 RVA: 0x003273F6 File Offset: 0x003255F6
			// (set) Token: 0x06007F03 RID: 32515 RVA: 0x003273FE File Offset: 0x003255FE
			public TIDateTime startTime { get; set; }

			// Token: 0x17001251 RID: 4689
			// (get) Token: 0x06007F04 RID: 32516 RVA: 0x00327407 File Offset: 0x00325607
			// (set) Token: 0x06007F05 RID: 32517 RVA: 0x0032740F File Offset: 0x0032560F
			public TINaturalSpaceObjectState barycenter { get; set; }

			// Token: 0x17001252 RID: 4690
			// (get) Token: 0x06007F06 RID: 32518 RVA: 0x00327418 File Offset: 0x00325618
			public double boostDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x17001253 RID: 4691
			// (get) Token: 0x06007F07 RID: 32519 RVA: 0x00327423 File Offset: 0x00325623
			public double decelDV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x17001254 RID: 4692
			// (get) Token: 0x06007F08 RID: 32520 RVA: 0x0032742E File Offset: 0x0032562E
			public double DV_mps
			{
				get
				{
					return 0.0;
				}
			}

			// Token: 0x17001255 RID: 4693
			// (get) Token: 0x06007F09 RID: 32521 RVA: 0x00327439 File Offset: 0x00325639
			// (set) Token: 0x06007F0A RID: 32522 RVA: 0x00327441 File Offset: 0x00325641
			public bool isImpulse { get; set; }

			// Token: 0x17001256 RID: 4694
			// (get) Token: 0x06007F0B RID: 32523 RVA: 0x0032744A File Offset: 0x0032564A
			// (set) Token: 0x06007F0C RID: 32524 RVA: 0x00327452 File Offset: 0x00325652
			public bool isTorch { get; set; }

			// Token: 0x17001257 RID: 4695
			// (get) Token: 0x06007F0D RID: 32525 RVA: 0x0032745B File Offset: 0x0032565B
			// (set) Token: 0x06007F0E RID: 32526 RVA: 0x00327463 File Offset: 0x00325663
			public bool isOrbitPhasing { get; set; }

			// Token: 0x17001258 RID: 4696
			// (get) Token: 0x06007F0F RID: 32527 RVA: 0x0032746C File Offset: 0x0032566C
			// (set) Token: 0x06007F10 RID: 32528 RVA: 0x0032746F File Offset: 0x0032566F
			public bool interruptible
			{
				get
				{
					return true;
				}
				set
				{
				}
			}

			// Token: 0x06007F11 RID: 32529 RVA: 0x00327471 File Offset: 0x00325671
			public double DVConsumedByTime(TIDateTime timeToCheck)
			{
				return 0.0;
			}

			// Token: 0x06007F12 RID: 32530 RVA: 0x0032747C File Offset: 0x0032567C
			public Trajectory_Patched.ThrustPhase GetThrustPhaseAtTime(TIDateTime timeToCheck)
			{
				return Trajectory_Patched.ThrustPhase.Coasting;
			}

			// Token: 0x06007F13 RID: 32531 RVA: 0x00327480 File Offset: 0x00325680
			public CartesianState GlobalCartesianStateAtTime(TIDateTime timeToCheck)
			{
				Vector3d vector3d = this.LocalVelocity();
				Vector3d vector3d2 = (this.barycenter.SpatialRotation * vector3d.xzy).xzy + this.barycenter.ToGlobalCartesianStateAtTime(timeToCheck).velocity;
				return new CartesianState(this.GlobalPositionAtTime(timeToCheck), vector3d2);
			}

			// Token: 0x06007F14 RID: 32532 RVA: 0x003274D8 File Offset: 0x003256D8
			public Vector3d GlobalPositionAtTime(TIDateTime timeToCheck)
			{
				Vector3d vector3d = this.LocalPositionAtTime(timeToCheck);
				return (this.barycenter.SpatialRotation * vector3d.xzy).xzy + this.barycenter.ToGlobalCartesianStateAtTime(timeToCheck).position;
			}

			// Token: 0x06007F15 RID: 32533 RVA: 0x00327524 File Offset: 0x00325724
			public OrbitalElementsState OrbitalElementsAtTime(TIDateTime timeToCheck)
			{
				CartesianState cartesianState = new CartesianState(this.LocalPositionAtTime(timeToCheck), this.LocalVelocity());
				return cartesianState.ToOrbitalElementsState(this.barycenter.mu, new DateTime?(timeToCheck.ExportTime()));
			}

			// Token: 0x06007F16 RID: 32534 RVA: 0x00327562 File Offset: 0x00325762
			private Vector3d LocalVelocity()
			{
				return (this.endPosition - this.startPosition) / this.duration_s;
			}

			// Token: 0x06007F17 RID: 32535 RVA: 0x00327580 File Offset: 0x00325780
			private Vector3d LocalPositionAtTime(TIDateTime timeToCheck)
			{
				double num = timeToCheck.DifferenceInSeconds(this.startTime);
				return this.startPosition + (this.endPosition - this.startPosition) * num / this.duration_s;
			}

			// Token: 0x06007F18 RID: 32536 RVA: 0x003275C8 File Offset: 0x003257C8
			public double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
			{
				barycenter = this.barycenter;
				timeToCheck.DifferenceInSeconds(this.startTime);
				return this.LocalPositionAtTime(timeToCheck).magnitude;
			}

			// Token: 0x06007F19 RID: 32537 RVA: 0x003275F9 File Offset: 0x003257F9
			public bool isPlausible(IMobileAsset fleet)
			{
				return true;
			}

			// Token: 0x06007F1A RID: 32538 RVA: 0x003275FC File Offset: 0x003257FC
			public string DumpSegment()
			{
				string[] array = new string[6];
				array[0] = "torchCoast,";
				array[1] = this.barycenter.displayName;
				array[2] = ",";
				int num = 3;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[4] = ",unknownEndTime,";
				array[5] = this.DV_mps.ToString();
				return string.Concat(array);
			}

			// Token: 0x06007F1B RID: 32539 RVA: 0x00327660 File Offset: 0x00325860
			public string deepDump()
			{
				string[] array = new string[11];
				array[0] = "torch coast segment:\n     start time = ";
				int num = 1;
				TIDateTime startTime = this.startTime;
				array[num] = ((startTime != null) ? startTime.ToString() : null);
				array[2] = "\n     barycenter = ";
				int num2 = 3;
				TINaturalSpaceObjectState barycenter = this.barycenter;
				array[num2] = ((barycenter != null) ? barycenter.displayName : null) ?? "null";
				array[4] = "\n     duration      = ";
				array[5] = this.duration_s.ToString();
				array[6] = "s\n     startPosition = ";
				array[7] = this.startPosition.ToString();
				array[8] = "\n     endPosition   = ";
				array[9] = this.endPosition.ToString();
				array[10] = "\n";
				return string.Concat(array);
			}

			// Token: 0x04005E9D RID: 24221
			public double duration_s;

			// Token: 0x04005E9E RID: 24222
			public Vector3d startPosition;

			// Token: 0x04005E9F RID: 24223
			public Vector3d endPosition;
		}
	}
}
