using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007CC RID: 1996
	public class Trajectory_Impulse : Trajectory_WithOrbitalElements
	{
		// Token: 0x17000E25 RID: 3621
		// (get) Token: 0x06004794 RID: 18324 RVA: 0x001D36C1 File Offset: 0x001D18C1
		public override TrajectoryModel GetTrajectoryModel
		{
			get
			{
				return TrajectoryModel.Impulse;
			}
		}

		// Token: 0x06004795 RID: 18325 RVA: 0x001D36C4 File Offset: 0x001D18C4
		public override string GetDisplayName()
		{
			return Loc.T("UI.Operations.Impulse");
		}

		// Token: 0x06004796 RID: 18326 RVA: 0x001D36D0 File Offset: 0x001D18D0
		[return: TupleElementNames(new string[] { "start", "domain" })]
		public override List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> GetTrajectoryDomainsOverTime()
		{
			return new List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>
			{
				new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(base.launchTime, Trajectory.TrajectoryDomain.Impulse)
			};
		}

		// Token: 0x06004797 RID: 18327 RVA: 0x001D36EC File Offset: 0x001D18EC
		public override bool CantManeuver(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			if (time < base.launchTime || time > base.arrivalTime)
			{
				return false;
			}
			TIDateTime tidateTime = new TIDateTime(base.launchTime, this.prepositionDuration_s);
			TIDateTime tidateTime2 = new TIDateTime(tidateTime, this.boostDuration_s);
			TIDateTime tidateTime3 = new TIDateTime(base.arrivalTime, -this.captureDuration_s);
			TIDateTime tidateTime4 = new TIDateTime(tidateTime3, -this.decelDuration_s);
			return time >= tidateTime4 && time <= tidateTime3;
		}

		// Token: 0x06004798 RID: 18328 RVA: 0x001D377C File Offset: 0x001D197C
		public override void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TrajectorySolver solver, double fleetCruiseAcceleration_mps2)
		{
			base.fleetCruiseAcceleration_mps2 = fleetCruiseAcceleration_mps2;
			ImpulseTransfer impulseTransfer = solver as ImpulseTransfer;
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, impulseTransfer.launchTime, impulseTransfer.transitDuration_s, false);
			this.transferOrbit = impulseTransfer.transferOrbit;
			this.boostDV_mps = impulseTransfer.boost_DV_mps;
			this.decelDV_mps = impulseTransfer.decel_DV_mps;
			this.prepositionDuration_s = 0.0;
			this.boostDuration_s = this.boostDV_mps / fleetCruiseAcceleration_mps2;
			this.decelDuration_s = this.decelDV_mps / fleetCruiseAcceleration_mps2;
			this.captureDuration_s = 0.0;
			this.coastDuration_s = impulseTransfer.transitDuration_s - this.prepositionDuration_s - this.boostDuration_s - this.decelDuration_s - this.captureDuration_s;
			base.duration = base.BuildSingleTrajectory_SetDuration(base.duration_s);
			this.GenerateBurnParameters(impulseTransfer);
		}

		// Token: 0x06004799 RID: 18329 RVA: 0x001D3854 File Offset: 0x001D1A54
		private void GenerateBurnParameters(ImpulseTransfer impulseSolver)
		{
			TIDateTime tidateTime = new TIDateTime(base.launchTime, this.prepositionDuration_s);
			TIDateTime tidateTime2 = new TIDateTime(tidateTime, this.boostDuration_s);
			CartesianState cartesianState = base.fleet.tryToGetGlobalCartesianState(tidateTime).GetValueOrDefault() - base.commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime);
			CartesianState cartesianState2 = this.TransferOrbitCartesianStateAtTime(tidateTime2) - base.commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime2);
			this.boost.startPosition = cartesianState.position;
			this.boost.endPosition = cartesianState2.position;
			this.boost.startVelocityControlPoint = cartesianState.position + cartesianState.velocity * this.boostDuration_s / 3.0;
			this.boost.endVelocityControlPoint = cartesianState2.position - cartesianState2.velocity * this.boostDuration_s / 3.0;
			TIDateTime tidateTime3 = new TIDateTime(base.arrivalTime, -this.captureDuration_s);
			TIDateTime tidateTime4 = new TIDateTime(tidateTime3, -this.decelDuration_s);
			TIDateTime tidateTime5 = new TIDateTime(tidateTime3, -this.decelDuration_s * 0.5);
			Vector3d position = this.TransferOrbitCartesianStateAtTime(tidateTime5).position;
			OrbitalElementsState orbitalElementsState = base.destinationOrbit.ToOrbitalElementsState(tidateTime5, 0.0);
			orbitalElementsState.meanAnomalyAtEpoch_Rad = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, base.commonBarycenter, position - base.commonBarycenter.GetGlobalPositionAtTime(tidateTime5), tidateTime5, base.fleet.faction.isActivePlayer);
			CartesianState cartesianState3 = this.TransferOrbitCartesianStateAtTime(tidateTime4) - base.commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime4);
			CartesianState cartesianState4 = orbitalElementsState.ToCartesianStateAtTime(tidateTime3.ExportTime(), base.destinationOrbit.barycenter.mass_kg) + base.destinationOrbit.barycenter.ToGlobalCartesianStateAtTime(tidateTime3) - base.commonBarycenter.ToGlobalCartesianStateAtTime(tidateTime3);
			Vector3d xzy = (base.commonBarycenter.SpatialRotation * cartesianState4.positionDisplay).xzy;
			Vector3d xzy2 = (base.commonBarycenter.SpatialRotation * cartesianState4.velocityDisplay).xzy;
			this.decel.startPosition = cartesianState3.position;
			this.decel.endPosition = xzy;
			this.decel.startVelocityControlPoint = cartesianState3.position + cartesianState3.velocity * this.decelDuration_s / 3.0;
			this.decel.endVelocityControlPoint = xzy - xzy2 * this.decelDuration_s / 3.0;
		}

		// Token: 0x0600479A RID: 18330 RVA: 0x001D3B18 File Offset: 0x001D1D18
		public override bool isPlausible()
		{
			double num = this.boost.MaxAccelerationDuringBurn_mps2(this.boostDuration_s);
			if (num > (double)base.fleet.cruiseAcceleration_mps2 * 2.0)
			{
				Log.Error(string.Concat(new string[]
				{
					"Impulse trajectory implausible: boost phase requires acceleration of ",
					num.ToString(),
					"m/s2 when the fleet can only achieve ",
					base.fleet.cruiseAcceleration_mps2.ToString(),
					"m/s2."
				}), Array.Empty<object>());
				return false;
			}
			double num2 = this.decel.MaxAccelerationDuringBurn_mps2(this.decelDuration_s);
			if (num2 > (double)base.fleet.cruiseAcceleration_mps2 * 2.0)
			{
				Log.Error(string.Concat(new string[]
				{
					"Impulse trajectory implausible: decel phase requires acceleration of ",
					num2.ToString(),
					"m/s2 when the fleet can only achieve ",
					base.fleet.cruiseAcceleration_mps2.ToString(),
					"m/s2."
				}), Array.Empty<object>());
				return false;
			}
			if (this.transferOrbit.eccentricity == 1.0)
			{
				Log.Error("Impulse trajectory implausible: coast stage eccentricity is exactly 1.  We don't handle parabolic trajectories.", Array.Empty<object>());
				return false;
			}
			if (this.transferOrbit.eccentricity < 1.0 && this.transferOrbit.semiMajorAxis_m >= 0.0)
			{
				Log.Error(string.Concat(new string[]
				{
					"Impulse trajectory implausible: coast eccentricity is ",
					this.transferOrbit.eccentricity.ToString(),
					" implying an elliptical orbit, while the semi major axis is ",
					this.transferOrbit.semiMajorAxis_m.ToString(),
					"m which only makes sense for a hyperbolic trajectory."
				}), Array.Empty<object>());
				return false;
			}
			if (this.transferOrbit.eccentricity > 1.0 && this.transferOrbit.semiMajorAxis_m <= 0.0)
			{
				Log.Error(string.Concat(new string[]
				{
					"Impulse trajectory implausible: coast eccentricity is ",
					this.transferOrbit.eccentricity.ToString(),
					" implying a hyperbolic trajectory, while the semi major axis is ",
					this.transferOrbit.semiMajorAxis_m.ToString(),
					"m which only makes sense for an elliptical orbit."
				}), Array.Empty<object>());
				return false;
			}
			return true;
		}

		// Token: 0x0600479B RID: 18331 RVA: 0x001D3D44 File Offset: 0x001D1F44
		public override TINaturalSpaceObjectState GetBarycenterAtTime(TIDateTime time)
		{
			if (time < base.launchTime || time > base.arrivalTime)
			{
				return base.GetBarycenterAtTime(time);
			}
			double num;
			double num2;
			TrajectoryPhase trajectoryPhase = base.GetTrajectoryPhase(base.assignedTime, base.launchTime, time, false, out num, out num2);
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			if (base.targetingFleet)
			{
				if (base.destinationFleet.transferAssigned)
				{
					tinaturalSpaceObjectState = base.destinationFleet.trajectory.GetBarycenterAtTime(base.arrivalTime);
				}
				else
				{
					tinaturalSpaceObjectState = base.destinationFleet.barycenter;
				}
			}
			else if (base.destination == null)
			{
				tinaturalSpaceObjectState = this.GetBarycenterAtTime(new TIDateTime(base.arrivalTime, -1.0));
			}
			else
			{
				tinaturalSpaceObjectState = base.destination.barycenter;
			}
			switch (trajectoryPhase)
			{
			case TrajectoryPhase.Loiter:
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
				if (base.originOrbit != null && base.originOrbit.barycenter != null)
				{
					return base.originOrbit.barycenter;
				}
				if (base.fleet.barycenter() != base.commonBarycenter && base.fleet.barycenter() != tinaturalSpaceObjectState)
				{
					return base.fleet.barycenter();
				}
				return base.commonBarycenter;
			case TrajectoryPhase.Deceleration:
			case TrajectoryPhase.Capture:
			case TrajectoryPhase.Arrive:
				return tinaturalSpaceObjectState;
			}
			return base.commonBarycenter;
		}

		// Token: 0x0600479C RID: 18332 RVA: 0x001D3E98 File Offset: 0x001D2098
		public override CartesianState ToGlobalCartesianStateAtTime(TIDateTime timeToCheck)
		{
			double num;
			double num2;
			Vector3d vector3d;
			switch (base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, false, out num, out num2))
			{
			case TrajectoryPhase.Loiter:
				vector3d = default(Vector3d);
				goto IL_0081;
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
				vector3d = this.boost.VelocityInBurn(num2, this.boostDuration_s);
				goto IL_0081;
			case TrajectoryPhase.Coast:
				return this.TransferOrbitCartesianStateAtTime(timeToCheck);
			case TrajectoryPhase.Deceleration:
				vector3d = this.decel.VelocityInBurn(num2, this.boostDuration_s);
				goto IL_0081;
			}
			return base.DestinationCartesianStateAtTime(timeToCheck);
			IL_0081:
			vector3d += base.commonBarycenter.ToGlobalCartesianStateAtTime(timeToCheck).velocity;
			bool flag;
			return new CartesianState(this.PositionAtTime(timeToCheck, false, out flag), vector3d);
		}

		// Token: 0x0600479D RID: 18333 RVA: 0x001D3F50 File Offset: 0x001D2150
		public override OrbitalElementsState GetOrbitalElementsAtTime(TIDateTime timeToCheck, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			if ((timeToCheck < base.launchTime && base.fleet.ref_orbit != null) || timeToCheck > base.arrivalTime)
			{
				return base.GetOrbitalElementsAtTime(timeToCheck, precision);
			}
			double num;
			double num2;
			switch (base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, false, out num, out num2))
			{
			case TrajectoryPhase.Loiter:
			{
				string[] array = new string[5];
				array[0] = "Trajectory_Impulse.GetOrbitalElementsAtTime(";
				array[1] = ((timeToCheck != null) ? timeToCheck.ToString() : null);
				array[2] = "): called before launch (";
				int num3 = 3;
				TIDateTime launchTime = base.launchTime;
				array[num3] = ((launchTime != null) ? launchTime.ToString() : null);
				array[4] = ")";
				Debug.LogError(string.Concat(array));
				if (base.originOrbit != null)
				{
					return new OrbitalElementsState(base.originOrbit, 0.0, timeToCheck);
				}
				return base.GetOrbitalElementsAtTime(base.launchTime);
			}
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
			{
				double num4 = timeToCheck.DifferenceInSeconds(base.launchTime);
				Vector3d vector3d = this.boost.LocationInBurn(num4, this.boostDuration_s);
				Vector3d vector3d2 = this.boost.VelocityInBurn(num4, this.boostDuration_s);
				CartesianState cartesianState = new CartesianState(vector3d, vector3d2);
				cartesianState = Quaterniond.Inverse(base.commonBarycenter.SpatialRotation) * cartesianState.xzy;
				return cartesianState.ToOrbitalElementsState(base.commonBarycenter.mu, new DateTime?(timeToCheck.ExportTime()));
			}
			case TrajectoryPhase.Coast:
				return this.transferOrbit;
			case TrajectoryPhase.Deceleration:
			{
				double num5 = this.decelDuration_s - base.arrivalTime.DifferenceInSeconds(timeToCheck);
				Vector3d vector3d3 = this.decel.LocationInBurn(num5, this.decelDuration_s);
				Vector3d vector3d4 = this.decel.VelocityInBurn(num5, this.decelDuration_s);
				CartesianState cartesianState2 = new CartesianState(vector3d3, vector3d4);
				cartesianState2 = Quaterniond.Inverse(base.commonBarycenter.SpatialRotation) * cartesianState2.xzy;
				return cartesianState2.ToOrbitalElementsState(base.commonBarycenter.mu, new DateTime?(timeToCheck.ExportTime()));
			}
			}
			string[] array2 = new string[5];
			array2[0] = "Trajectory_Impulse.GetOrbitalElementsAtTime(";
			array2[1] = ((timeToCheck != null) ? timeToCheck.ToString() : null);
			array2[2] = "): called after arrival (";
			int num6 = 3;
			TIDateTime arrivalTime = base.arrivalTime;
			array2[num6] = ((arrivalTime != null) ? arrivalTime.ToString() : null);
			array2[4] = ")";
			Debug.LogError(string.Concat(array2));
			return new OrbitalElementsState(base.destinationOrbit, 0.0, base.arrivalTime);
		}

		// Token: 0x0600479E RID: 18334 RVA: 0x001D41C4 File Offset: 0x001D23C4
		public override double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
		{
			if (timeToCheck < base.launchTime || timeToCheck > base.arrivalTime)
			{
				return base.getDistFromBarycenterAtTime_m(timeToCheck, out barycenter);
			}
			double num;
			double num2;
			TrajectoryPhase trajectoryPhase = base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, false, out num, out num2);
			barycenter = base.commonBarycenter;
			switch (trajectoryPhase)
			{
			case TrajectoryPhase.Loiter:
			{
				string[] array = new string[5];
				array[0] = "Trajectory_Impulse.GetOrbitalElementsAtTime(";
				array[1] = ((timeToCheck != null) ? timeToCheck.ToString() : null);
				array[2] = "): called before launch (";
				int num3 = 3;
				TIDateTime launchTime = base.launchTime;
				array[num3] = ((launchTime != null) ? launchTime.ToString() : null);
				array[4] = ")";
				Debug.LogError(string.Concat(array));
				barycenter = this.GetBarycenterAtTime(timeToCheck);
				return 0.0;
			}
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
			{
				double num4 = timeToCheck.DifferenceInSeconds(base.launchTime);
				return this.boost.LocationInBurn(num4, this.boostDuration_s).magnitude;
			}
			case TrajectoryPhase.Coast:
				return this.transferOrbit.semiMajorAxis_m;
			case TrajectoryPhase.Deceleration:
			{
				double num5 = this.decelDuration_s - base.arrivalTime.DifferenceInSeconds(timeToCheck);
				return this.decel.LocationInBurn(num5, this.decelDuration_s).magnitude;
			}
			}
			string[] array2 = new string[5];
			array2[0] = "Trajectory_Impulse.GetOrbitalElementsAtTime(";
			array2[1] = ((timeToCheck != null) ? timeToCheck.ToString() : null);
			array2[2] = "): called after arrival (";
			int num6 = 3;
			TIDateTime arrivalTime = base.arrivalTime;
			array2[num6] = ((arrivalTime != null) ? arrivalTime.ToString() : null);
			array2[4] = ")";
			Debug.LogError(string.Concat(array2));
			barycenter = this.GetBarycenterAtTime(timeToCheck);
			return 0.0;
		}

		// Token: 0x0600479F RID: 18335 RVA: 0x001D4364 File Offset: 0x001D2564
		public override Vector3d PositionAtTime(TIDateTime timeToCheck, bool setPosition, out bool arrived)
		{
			double num;
			double num2;
			TrajectoryPhase trajectoryPhase = base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, setPosition, out num, out num2);
			arrived = false;
			switch (trajectoryPhase)
			{
			case TrajectoryPhase.Loiter:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
				}
				return base.fleet.GetGlobalPositionAtTime(timeToCheck);
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
				if (setPosition)
				{
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					base.fleet.SetAccelerationPhaseStatus(true, false, false);
					Mathd.Min(num2 * base.fleetCruiseAcceleration_mps2, this.boostDV_mps);
					if (!this.freeDVTransfer)
					{
						this.UpdateDVconsumed(this.boostDV_mps);
					}
				}
				return this.boost.LocationInBurn(num2, this.boostDuration_s) + base.commonBarycenter.GetGlobalPositionAtTime(timeToCheck);
			case TrajectoryPhase.Coast:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					if (!this.freeDVTransfer)
					{
						this.UpdateDVconsumed(this.boostDV_mps);
					}
				}
				return this.TransferOrbitCartesianStateAtTime(timeToCheck).position;
			case TrajectoryPhase.Deceleration:
			{
				double num3 = num2 - this.boostDuration_s - this.coastDuration_s;
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(true, false, false);
					double num4 = this.boostDV_mps + Mathd.Min(num3 * base.fleetCruiseAcceleration_mps2, this.decelDV_mps);
					if (!this.freeDVTransfer)
					{
						this.UpdateDVconsumed(num4);
					}
				}
				return this.decel.LocationInBurn(num3, this.decelDuration_s) + base.commonBarycenter.GetGlobalPositionAtTime(timeToCheck);
			}
			}
			if (setPosition)
			{
				base.fleet.SetAccelerationPhaseStatus(false, false, false);
				base.fleet.SetDecelerationPhaseStatus(false, false, false);
				if (!this.freeDVTransfer)
				{
					this.UpdateDVconsumed(this.boostDV_mps + this.decelDV_mps);
				}
			}
			arrived = true;
			return this.TransferOrbitCartesianStateAtTime(timeToCheck).position;
		}

		// Token: 0x060047A0 RID: 18336 RVA: 0x001D4554 File Offset: 0x001D2754
		private CartesianState TransferOrbitCartesianStateAtTime(TIDateTime time)
		{
			CartesianState cartesianState = this.transferOrbit.ToCartesianStateAtTime(time.ExportTime(), base.commonBarycenter.mass_kg);
			Vector3d xzy = (base.commonBarycenter.SpatialRotation * cartesianState.positionDisplay).xzy;
			Vector3d xzy2 = (base.commonBarycenter.SpatialRotation * cartesianState.velocityDisplay).xzy;
			return base.commonBarycenter.ToGlobalCartesianStateAtTime(time) + new CartesianState(xzy, xzy2);
		}

		// Token: 0x060047A1 RID: 18337 RVA: 0x001D45D8 File Offset: 0x001D27D8
		private void UpdateDVconsumed(double DVconsumed_mps)
		{
			double num = base.fleet.fleetTrajectoryData.initialDeltaV_mps - (double)base.fleet.currentDeltaV_mps;
			float DVToConsume_kps = (float)(DVconsumed_mps - num) / 1000f;
			if (DVToConsume_kps > 0f)
			{
				base.fleet.ships.ForEach(delegate(TISpaceShipState x)
				{
					x.ConsumeDeltaV(DVToConsume_kps, false);
				});
			}
		}

		// Token: 0x060047A2 RID: 18338 RVA: 0x001D4644 File Offset: 0x001D2844
		public override TIDateTime getOrbitEndTime()
		{
			double num = TITimeState.Now().DifferenceInSeconds(base.launchTime);
			double num2 = this.loiterDuration_s + this.prepositionDuration_s + this.boostDuration_s;
			double num3 = num2 + this.coastDuration_s;
			if (num2 <= num && num <= num3)
			{
				return new TIDateTime(base.launchTime, num3);
			}
			return base.getOrbitEndTime();
		}

		// Token: 0x060047A3 RID: 18339 RVA: 0x001D4699 File Offset: 0x001D2899
		public override bool isInImpulse(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			return time >= base.launchTime && time <= base.arrivalTime;
		}

		// Token: 0x060047A4 RID: 18340 RVA: 0x001D46C8 File Offset: 0x001D28C8
		public override string deepDump()
		{
			string text = "   Trajectory_Impulse:\n";
			base.appendCommonDeepDump(ref text);
			text = string.Concat(new string[]
			{
				text,
				"    boost duration = ",
				this.boostDuration_s.ToString(),
				"s\n",
				this.boost.deepDump(),
				"    decel duration = ",
				this.decelDuration_s.ToString(),
				"s\n",
				this.decel.deepDump(),
				"    coast orbital parameters:\n     semi-major axis             = ",
				this.transferOrbit.semiMajorAxis_m.ToString(),
				"m\n     eccentricity                = ",
				this.transferOrbit.eccentricity.ToString(),
				"\n     longitude of ascending node = ",
				this.transferOrbit.longAscendingNode_Rad.ToString(),
				"rad\n     inclination                 = ",
				this.transferOrbit.inclination_Rad.ToString(),
				"rad\n     argument of periapsis       = ",
				this.transferOrbit.argPeriapsis_Rad.ToString(),
				"rad\n     mean anomaly at epoch       = ",
				this.transferOrbit.meanAnomalyAtEpoch_Rad.ToString(),
				"rad\n     epoch                       = ",
				this.transferOrbit.epoch.ToString(),
				"\n"
			});
			base.appendCommonDeepDumpPostscript(ref text);
			return text;
		}

		// Token: 0x0400297E RID: 10622
		private bool freeDVTransfer;

		// Token: 0x0400297F RID: 10623
		private BurnBezierDescription boost = new BurnBezierDescription();

		// Token: 0x04002980 RID: 10624
		private BurnBezierDescription decel = new BurnBezierDescription();
	}
}
