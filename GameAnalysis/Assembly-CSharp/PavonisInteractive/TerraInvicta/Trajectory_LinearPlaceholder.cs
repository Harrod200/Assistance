using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007CE RID: 1998
	public class Trajectory_LinearPlaceholder : Trajectory
	{
		// Token: 0x17000E27 RID: 3623
		// (get) Token: 0x060047B7 RID: 18359 RVA: 0x001D562D File Offset: 0x001D382D
		public override TrajectoryModel GetTrajectoryModel
		{
			get
			{
				return TrajectoryModel.LinearPlaceholder;
			}
		}

		// Token: 0x060047B8 RID: 18360 RVA: 0x001D5630 File Offset: 0x001D3830
		public override string GetDisplayName()
		{
			return Loc.T("UI.Operations.LinearPlaceholder");
		}

		// Token: 0x060047B9 RID: 18361 RVA: 0x001D563C File Offset: 0x001D383C
		[return: TupleElementNames(new string[] { "start", "domain" })]
		public override List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> GetTrajectoryDomainsOverTime()
		{
			return new List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>
			{
				new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(base.launchTime, Trajectory.TrajectoryDomain.Torch)
			};
		}

		// Token: 0x060047BA RID: 18362 RVA: 0x001D5658 File Offset: 0x001D3858
		public override void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TrajectorySolver solver, double fleetCruiseAcceleration_mps2)
		{
			double num = TISpaceObjectState.TransferDistance(fleet, destination, originValue, destinationValue, true);
			base.fleetCruiseAcceleration_mps2 = fleetCruiseAcceleration_mps2;
			this.loiterDuration_s = 0.0;
			double num2 = (double)fleet.currentDeltaV_mps / 2.0;
			double num3 = num / 2.0;
			double num4 = num2 / (double)fleet.cruiseAcceleration_mps2;
			double num5 = Mathd.Sqrt(2.0 * num3 / (double)fleet.cruiseAcceleration_mps2);
			this.boostDuration_s = Mathd.Min(num5, num4);
			this.decelDuration_s = this.boostDuration_s;
			this.boostDV_mps = (double)fleet.cruiseAcceleration_mps2 * this.boostDuration_s;
			this.decelDV_mps = (double)fleet.cruiseAcceleration_mps2 * this.decelDuration_s;
			double num6 = 0.5 * (double)fleet.cruiseAcceleration_mps2 * Mathd.Pow(this.boostDuration_s, 2.0);
			double num7 = num6;
			double num8 = Mathd.Max(num - num6 - num7, 0.0);
			this.coastDuration_s = Mathd.Max(num8 / ((this.boostDV_mps == 0.0) ? 1.0 : this.boostDV_mps), 0.0);
			double num9 = this.boostDuration_s + this.decelDuration_s + this.coastDuration_s;
			int num10 = (int)(num9 % 1.0 * 100.0);
			int num11 = (int)(num9 / 60.0 % 1.0 * 60.0);
			int num12 = (int)(num9 / 3600.0 % 1.0 * 60.0);
			int num13 = (int)(num9 / 86400.0 % 1.0 * 24.0);
			int num14 = (int)(num9 / 604800.0 * 7.0);
			base.duration = new TimeSpan(num14, num13, num12, num11, num10);
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, TITimeState.Now(), num9, false);
		}

		// Token: 0x060047BB RID: 18363 RVA: 0x001D585C File Offset: 0x001D3A5C
		public override bool isPlausible()
		{
			Log.Error("Attempted a Linear Placeholder Trajectory.  This should never be called.", Array.Empty<object>());
			return false;
		}

		// Token: 0x060047BC RID: 18364 RVA: 0x001D5870 File Offset: 0x001D3A70
		public override CartesianState ToGlobalCartesianStateAtTime(TIDateTime timeToCheck)
		{
			bool flag;
			return new CartesianState(this.PositionAtTime(timeToCheck, false, out flag), default(Vector3d));
		}

		// Token: 0x060047BD RID: 18365 RVA: 0x001D5898 File Offset: 0x001D3A98
		public override Vector3d PositionAtTime(TIDateTime timeToCheck, bool setPosition, out bool arrived)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3;
			double num4;
			TrajectoryPhase trajectoryPhase = base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, setPosition, out num3, out num4);
			switch (trajectoryPhase)
			{
			case TrajectoryPhase.Loiter:
			{
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
				}
				arrived = false;
				if (base.fleet.tryToGetGlobalCartesianState(timeToCheck) == null)
				{
					return default(Vector3d);
				}
				CartesianState? cartesianState;
				return cartesianState.GetValueOrDefault().position;
			}
			case TrajectoryPhase.Boost:
				num = 0.5 * (double)base.fleet.cruiseAcceleration_mps2 * num4 * num4;
				if (setPosition)
				{
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					base.fleet.SetAccelerationPhaseStatus(true, false, false);
					num2 = num4 * (double)base.fleet.cruiseAcceleration_mps2;
				}
				break;
			case TrajectoryPhase.Coast:
				num = 0.5 * (double)base.fleet.cruiseAcceleration_mps2 * this.boostDuration_s * this.boostDuration_s;
				num += this.boostDV_mps * (num3 - this.boostDuration_s);
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					num2 = this.boostDV_mps;
				}
				break;
			case TrajectoryPhase.Deceleration:
			{
				num = 0.5 * (double)base.fleet.cruiseAcceleration_mps2 * this.boostDuration_s * this.boostDuration_s;
				num += this.boostDV_mps * this.coastDuration_s;
				double num5 = num3 - this.coastDuration_s - this.boostDuration_s;
				num += this.boostDV_mps * num5 + 0.5 * (double)(-(double)base.fleet.cruiseAcceleration_mps2) * num5 * num5;
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(true, false, false);
					num2 = this.boostDV_mps + num5 * (double)base.fleet.cruiseAcceleration_mps2;
				}
				break;
			}
			case TrajectoryPhase.Arrive:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					num2 = this.boostDV_mps + this.decelDV_mps;
				}
				num = base.straightLineDistance_m;
				break;
			}
			double num6 = Mathd.Clamp01(num3 / base.duration.TotalSeconds);
			if (setPosition && trajectoryPhase > TrajectoryPhase.Loiter)
			{
				double num7 = base.fleet.fleetTrajectoryData.initialDeltaV_mps - (double)base.fleet.currentDeltaV_mps;
				float DVToConsume_kps = (float)(num2 - num7) / 1000f;
				if (DVToConsume_kps > 0f)
				{
					base.fleet.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.ConsumeDeltaV(DVToConsume_kps, false);
					});
				}
			}
			arrived = setPosition && num6 >= 1.0;
			double num8 = num / base.straightLineDistance_m;
			return Vector3d.Lerp(base.launchPosition, base.destinationPosition, Mathd.Min(num8, 1.0));
		}

		// Token: 0x060047BE RID: 18366 RVA: 0x001D5BA4 File Offset: 0x001D3DA4
		public override string deepDump()
		{
			return "Linear Placeholder Trajectory (should not be used).  No data included.";
		}
	}
}
