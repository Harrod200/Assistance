using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C9 RID: 1993
	public class Trajectory_Torch : Trajectory
	{
		// Token: 0x17000E20 RID: 3616
		// (get) Token: 0x06004778 RID: 18296 RVA: 0x001D2993 File Offset: 0x001D0B93
		public Vector3d accelerationVector_normal
		{
			get
			{
				return this.accelerationVector_mps2.normalized;
			}
		}

		// Token: 0x17000E21 RID: 3617
		// (get) Token: 0x06004779 RID: 18297 RVA: 0x001D29A0 File Offset: 0x001D0BA0
		public Vector3d decelerationVector_normal
		{
			get
			{
				return this.decelerationVector_mps2.normalized;
			}
		}

		// Token: 0x17000E22 RID: 3618
		// (get) Token: 0x0600477A RID: 18298 RVA: 0x001D29AD File Offset: 0x001D0BAD
		// (set) Token: 0x0600477B RID: 18299 RVA: 0x001D29BC File Offset: 0x001D0BBC
		public override double boostDV_mps
		{
			get
			{
				return base.fleetCruiseAcceleration_mps2 * this.boostDuration_s;
			}
			protected set
			{
				this.boostDuration_s = value / base.fleetCruiseAcceleration_mps2;
			}
		}

		// Token: 0x17000E23 RID: 3619
		// (get) Token: 0x0600477C RID: 18300 RVA: 0x001D29CC File Offset: 0x001D0BCC
		// (set) Token: 0x0600477D RID: 18301 RVA: 0x001D29DB File Offset: 0x001D0BDB
		public override double decelDV_mps
		{
			get
			{
				return base.fleetCruiseAcceleration_mps2 * this.decelDuration_s;
			}
			protected set
			{
				this.decelDuration_s = value / base.fleetCruiseAcceleration_mps2;
			}
		}

		// Token: 0x0600477E RID: 18302 RVA: 0x001D29EB File Offset: 0x001D0BEB
		[return: TupleElementNames(new string[] { "start", "domain" })]
		public override List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> GetTrajectoryDomainsOverTime()
		{
			return new List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>
			{
				new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(base.launchTime, Trajectory.TrajectoryDomain.Torch)
			};
		}

		// Token: 0x0600477F RID: 18303 RVA: 0x001D2A04 File Offset: 0x001D0C04
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
			double num;
			double num2;
			switch (base.GetTrajectoryPhase(base.assignedTime, base.launchTime, time, false, out num, out num2))
			{
			case TrajectoryPhase.Loiter:
				return false;
			case TrajectoryPhase.Deceleration:
				return true;
			case TrajectoryPhase.Capture:
			case TrajectoryPhase.Arrive:
				return false;
			}
			return false;
		}

		// Token: 0x17000E24 RID: 3620
		// (get) Token: 0x06004780 RID: 18304 RVA: 0x001D2A83 File Offset: 0x001D0C83
		public override TrajectoryModel GetTrajectoryModel
		{
			get
			{
				return TrajectoryModel.Torch;
			}
		}

		// Token: 0x06004781 RID: 18305 RVA: 0x001D2A86 File Offset: 0x001D0C86
		public override string GetDisplayName()
		{
			return Loc.T("UI.Operations.Torch");
		}

		// Token: 0x06004782 RID: 18306 RVA: 0x001D2A94 File Offset: 0x001D0C94
		public override void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TrajectorySolver solver, double fleetCruiseAcceleration_mps2)
		{
			TorchTransfer torchTransfer = solver as TorchTransfer;
			base.fleetCruiseAcceleration_mps2 = fleetCruiseAcceleration_mps2;
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, torchTransfer.launchTime, torchTransfer.transitDuration_s, false);
			this.boostDV_mps = torchTransfer.boost_DV_mps;
			this.decelDV_mps = torchTransfer.decel_DV_mps;
			this.boostDuration_s = torchTransfer.accelDuration_s;
			this.decelDuration_s = torchTransfer.decelDuration_s;
			this.coastDuration_s = torchTransfer.arrivalTime.DifferenceInSeconds(torchTransfer.launchTime) - this.boostDuration_s - this.decelDuration_s;
			if (this.coastDuration_s < 0.0)
			{
				string[] array = new string[10];
				array[0] = "Torch trajectory coast duration is negative.  Launch = ";
				int num = 1;
				TIDateTime launchTime = torchTransfer.launchTime;
				array[num] = ((launchTime != null) ? launchTime.ToString() : null);
				array[2] = ", Arrive = ";
				int num2 = 3;
				TIDateTime arrivalTime = torchTransfer.arrivalTime;
				array[num2] = ((arrivalTime != null) ? arrivalTime.ToString() : null);
				array[4] = ", accel duration = ";
				array[5] = torchTransfer.accelDuration_s.ToString();
				array[6] = ", coast duration = ";
				array[7] = this.coastDuration_s.ToString();
				array[8] = ", decel duration =";
				array[9] = torchTransfer.decelDuration_s.ToString();
				Debug.LogError(string.Concat(array));
			}
			this.accelerationVector_mps2 = torchTransfer.accelerationVector_mps2;
			this.decelerationVector_mps2 = torchTransfer.decelerationVector_mps2;
			this.initialVelocityVector_mps = torchTransfer.initialVelocityVector_mps;
			this.arrivalVelocityVector_mps = torchTransfer.arrivalVelocityVector_mps;
			this.coastVelocityVector_mps = this.initialVelocityVector_mps + this.accelerationVector_mps2 * this.boostDuration_s;
			base.duration = base.BuildSingleTrajectory_SetDuration(base.duration_s);
		}

		// Token: 0x06004783 RID: 18307 RVA: 0x001D2C2C File Offset: 0x001D0E2C
		public override bool isPlausible()
		{
			if ((double)base.fleet.cruiseAcceleration_mps2 * 2.0 < this.accelerationVector_mps2.magnitude)
			{
				Log.Error(string.Concat(new string[]
				{
					"Torch trajectory implausible: require boost acceleration of ",
					this.accelerationVector_mps2.magnitude.ToString(),
					"m/s2 when the fleet can only achieve ",
					base.fleet.cruiseAcceleration_mps2.ToString(),
					"m/s2."
				}), Array.Empty<object>());
				return false;
			}
			if ((double)base.fleet.cruiseAcceleration_mps2 * 2.0 < this.decelerationVector_mps2.magnitude)
			{
				Log.Error(string.Concat(new string[]
				{
					"Torch trajectory implausible: requires decel acceleration of ",
					this.decelerationVector_mps2.magnitude.ToString(),
					"m/s2 when the fleet can only achieve ",
					base.fleet.cruiseAcceleration_mps2.ToString(),
					"m/s2."
				}), Array.Empty<object>());
				return false;
			}
			return true;
		}

		// Token: 0x06004784 RID: 18308 RVA: 0x001D2D38 File Offset: 0x001D0F38
		public override CartesianState ToGlobalCartesianStateAtTime(TIDateTime timeToCheck)
		{
			double num;
			double num2;
			Vector3d vector3d;
			switch (base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, false, out num, out num2))
			{
			case TrajectoryPhase.Loiter:
				vector3d = this.initialVelocityVector_mps;
				break;
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
			{
				double num3 = num2;
				vector3d = this.initialVelocityVector_mps + this.accelerationVector_mps2 * num3;
				break;
			}
			case TrajectoryPhase.Coast:
				vector3d = this.coastVelocityVector_mps;
				break;
			case TrajectoryPhase.Deceleration:
			case TrajectoryPhase.Capture:
			{
				double num4 = num2 - this.coastDuration_s - this.boostDuration_s;
				vector3d = this.coastVelocityVector_mps + this.decelerationVector_mps2 * num4;
				break;
			}
			case TrajectoryPhase.Arrive:
				return base.DestinationCartesianStateAtTime(timeToCheck);
			default:
				vector3d = default(Vector3d);
				break;
			}
			bool flag;
			return new CartesianState(this.PositionAtTime(timeToCheck, false, out flag), vector3d);
		}

		// Token: 0x06004785 RID: 18309 RVA: 0x001D2E00 File Offset: 0x001D1000
		public override OrbitalElementsState GetOrbitalElementsAtTime(TIDateTime time, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			if ((time < base.launchTime && base.fleet.ref_orbit != null) || time > base.arrivalTime)
			{
				return base.GetOrbitalElementsAtTime(time, precision);
			}
			TINaturalSpaceObjectState barycenterAtTime = this.GetBarycenterAtTime(time);
			CartesianState xzy = (this.ToGlobalCartesianStateAtTime(time) - barycenterAtTime.ToGlobalCartesianStateAtTime(time)).xzy;
			return (Quaterniond.Inverse(barycenterAtTime.SpatialRotation) * xzy).ToOrbitalElementsState(barycenterAtTime.mu, new DateTime?(time.ExportTime()));
		}

		// Token: 0x06004786 RID: 18310 RVA: 0x001D2E94 File Offset: 0x001D1094
		public override double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
		{
			if (timeToCheck < base.launchTime || timeToCheck > base.arrivalTime)
			{
				return base.getDistFromBarycenterAtTime_m(timeToCheck, out barycenter);
			}
			barycenter = this.GetBarycenterAtTime(timeToCheck);
			return (this.ToGlobalCartesianStateAtTime(timeToCheck) - barycenter.ToGlobalCartesianStateAtTime(timeToCheck)).position.magnitude;
		}

		// Token: 0x06004787 RID: 18311 RVA: 0x001D2EF0 File Offset: 0x001D10F0
		public override Vector3d PositionAtTime(TIDateTime timeToCheck, bool setPosition, out bool arrived)
		{
			double num = 0.0;
			double num2;
			double num3;
			TrajectoryPhase trajectoryPhase = base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, setPosition, out num2, out num3);
			Vector3d vector3d = Vector3d.zero;
			switch (trajectoryPhase)
			{
			case TrajectoryPhase.Loiter:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
				}
				arrived = false;
				return base.fleet.GetGlobalPositionAtTime(timeToCheck);
			case TrajectoryPhase.Preposition:
			case TrajectoryPhase.Boost:
			{
				if (setPosition)
				{
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					base.fleet.SetAccelerationPhaseStatus(true, false, false);
				}
				double num4 = num3;
				num = num4 * base.fleetCruiseAcceleration_mps2;
				vector3d = this.initialVelocityVector_mps * num3 + 0.5 * this.accelerationVector_mps2 * num4 * num4;
				break;
			}
			case TrajectoryPhase.Coast:
			{
				double num5 = num3 - this.boostDuration_s;
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
				}
				num = this.boostDuration_s * base.fleetCruiseAcceleration_mps2;
				vector3d = this.initialVelocityVector_mps * num3 + 0.5 * this.accelerationVector_mps2 * this.boostDuration_s * this.boostDuration_s + this.coastVelocityVector_mps * num5;
				break;
			}
			case TrajectoryPhase.Deceleration:
			case TrajectoryPhase.Capture:
			{
				double num6 = num3 - this.coastDuration_s - this.boostDuration_s;
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(true, false, false);
				}
				num = this.boostDuration_s * base.fleetCruiseAcceleration_mps2 + num6 * base.fleetCruiseAcceleration_mps2;
				vector3d = this.initialVelocityVector_mps * num3 + 0.5 * this.accelerationVector_mps2 * this.boostDuration_s * this.boostDuration_s + this.coastVelocityVector_mps * this.coastDuration_s + 0.5 * this.decelerationVector_mps2 * num6 * num6;
				break;
			}
			case TrajectoryPhase.Arrive:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					num = this.DV_mps;
				}
				vector3d = this.initialVelocityVector_mps * num3 + 0.5 * this.accelerationVector_mps2 * this.boostDuration_s * this.boostDuration_s + this.coastVelocityVector_mps * this.coastDuration_s + 0.5 * this.decelerationVector_mps2 * this.decelDuration_s * this.decelDuration_s;
				break;
			}
			if (setPosition && trajectoryPhase > TrajectoryPhase.Loiter)
			{
				double num7 = base.fleet.fleetTrajectoryData.initialDeltaV_mps - (double)base.fleet.currentDeltaV_mps;
				double DVToConsume_mps = num - num7;
				if (DVToConsume_mps > 0.0)
				{
					base.fleet.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.ConsumeDeltaV((float)(DVToConsume_mps / 1000.0), false);
					});
				}
			}
			double num8 = Mathd.Clamp01(num2 / base.duration_s);
			arrived = setPosition && num8 >= 1.0;
			return base.launchPosition + vector3d;
		}

		// Token: 0x06004788 RID: 18312 RVA: 0x001D3278 File Offset: 0x001D1478
		public override string deepDump()
		{
			string text = "   Trajectory_Torch:\n";
			base.appendCommonDeepDump(ref text);
			text = string.Concat(new string[]
			{
				text,
				"    initialVelocityVector_mps = ",
				this.initialVelocityVector_mps.ToString(),
				"m/s\n    arrivalVelocityVector_mps = ",
				this.arrivalVelocityVector_mps.ToString(),
				"m/s\n    coastVelocityVector_mps   = ",
				this.coastVelocityVector_mps.ToString(),
				"m/s\n    accelerationVector_mps2   = ",
				this.accelerationVector_mps2.ToString(),
				"m/s2\n    decelerationVector_mps2   = ",
				this.decelerationVector_mps2.ToString(),
				"m/s2\n    boostDuration_s           = ",
				this.boostDuration_s.ToString(),
				"s\n    decelDuration_s           = ",
				this.decelDuration_s.ToString(),
				"s\n"
			});
			base.appendCommonDeepDumpPostscript(ref text);
			return text;
		}

		// Token: 0x04002974 RID: 10612
		public Vector3d initialVelocityVector_mps;

		// Token: 0x04002975 RID: 10613
		public Vector3d arrivalVelocityVector_mps;

		// Token: 0x04002976 RID: 10614
		public Vector3d coastVelocityVector_mps;

		// Token: 0x04002977 RID: 10615
		public Vector3d accelerationVector_mps2;

		// Token: 0x04002978 RID: 10616
		public Vector3d decelerationVector_mps2;
	}
}
