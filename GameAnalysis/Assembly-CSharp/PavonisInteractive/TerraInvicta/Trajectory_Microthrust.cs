using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007CD RID: 1997
	public class Trajectory_Microthrust : Trajectory_WithOrbitalElements
	{
		// Token: 0x17000E26 RID: 3622
		// (get) Token: 0x060047A6 RID: 18342 RVA: 0x001D4847 File Offset: 0x001D2A47
		public override TrajectoryModel GetTrajectoryModel
		{
			get
			{
				return TrajectoryModel.Microthrust;
			}
		}

		// Token: 0x060047A7 RID: 18343 RVA: 0x001D484A File Offset: 0x001D2A4A
		public override string GetDisplayName()
		{
			return Loc.T("UI.Operations.Microthrust");
		}

		// Token: 0x060047A8 RID: 18344 RVA: 0x001D4856 File Offset: 0x001D2A56
		[return: TupleElementNames(new string[] { "start", "domain" })]
		public override List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> GetTrajectoryDomainsOverTime()
		{
			return new List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>>
			{
				new ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>(base.launchTime, Trajectory.TrajectoryDomain.Microthrust)
			};
		}

		// Token: 0x060047A9 RID: 18345 RVA: 0x001D486F File Offset: 0x001D2A6F
		public override bool CantManeuver(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			return !(time < base.launchTime) && !(time > base.arrivalTime);
		}

		// Token: 0x060047AA RID: 18346 RVA: 0x001D48A0 File Offset: 0x001D2AA0
		public override void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TrajectorySolver solver, double fleetCruiseAcceleration_mps2)
		{
			MicrothrustTransfer microthrustTransfer = solver as MicrothrustTransfer;
			this.boostDuration_s = microthrustTransfer.boostDuration_s;
			this.decelDuration_s = microthrustTransfer.decelDuration_s;
			base.BuildSingleTrajectory_Common(fleet, destination, commonBarycenter, solver.launchTime, this.boostDuration_s + this.decelDuration_s, false);
			base.duration = base.BuildSingleTrajectory_SetDuration(this.boostDuration_s + this.decelDuration_s);
			this.boostDV_mps = microthrustTransfer.boost_DV_mps;
			this.decelDV_mps = microthrustTransfer.decel_DV_mps;
			this.initialOrbit_m = microthrustTransfer.initialOrbit_m;
			this.destinationOrbit_m = microthrustTransfer.destinationOrbit_m;
			this.initialInclination_rad = microthrustTransfer.initialInclination_rad;
			this.destinationInclination_rad = microthrustTransfer.destinationInclination_rad;
			this.ascending = microthrustTransfer.ascending;
			base.fleetCruiseAcceleration_mps2 = fleetCruiseAcceleration_mps2 * (double)(this.ascending ? 1 : (-1));
			this.initialEcc = originValue.common_e(commonBarycenter);
			this.initialNode_rad = originValue.common_Ω_rad(commonBarycenter);
			this.initialArgP_rad = originValue.common_ω_rad(commonBarycenter);
			this.initialVelocity_mps = microthrustTransfer.initialVelocity_mps;
			if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destination as TISpaceFleetState, fleet.faction))
			{
				OrbitalElementsState orbitalElementsState;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				bool flag;
				(destination as TISpaceFleetState).getOrbitalElementsState(solver.arrivalTime, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
				if (tinaturalSpaceObjectState == commonBarycenter)
				{
					this.destinationEcc = orbitalElementsState.eccentricity;
					this.destinationNode_rad = orbitalElementsState.longAscendingNode_Rad;
					this.destinationArgP_rad = orbitalElementsState.argPeriapsis_Rad;
				}
				else if (tinaturalSpaceObjectState.barycenter == commonBarycenter)
				{
					this.destinationEcc = tinaturalSpaceObjectState.ecc;
					this.destinationNode_rad = tinaturalSpaceObjectState.longAscendingNode_Rad;
					this.destinationArgP_rad = tinaturalSpaceObjectState.argPeriapsis_Rad;
				}
				else
				{
					TINaturalSpaceObjectState barycenter = tinaturalSpaceObjectState.barycenter;
					this.destinationEcc = ((barycenter != null) ? barycenter.ecc : 0.0);
					TINaturalSpaceObjectState barycenter2 = tinaturalSpaceObjectState.barycenter;
					this.destinationNode_rad = ((barycenter2 != null) ? barycenter2.longAscendingNode_Rad : 0.0);
					TINaturalSpaceObjectState barycenter3 = tinaturalSpaceObjectState.barycenter;
					this.destinationArgP_rad = ((barycenter3 != null) ? barycenter3.argPeriapsis_Rad : 0.0);
					TINaturalSpaceObjectState barycenter4 = tinaturalSpaceObjectState.barycenter;
					if (((barycenter4 != null) ? barycenter4.barycenter : null) != commonBarycenter)
					{
						Log.Error("Common barycenter was not common to destination at arrival time.", Array.Empty<object>());
					}
				}
			}
			else
			{
				this.destinationEcc = destinationValue.common_e(commonBarycenter);
				this.destinationNode_rad = destinationValue.common_Ω_rad(commonBarycenter);
				this.destinationArgP_rad = destinationValue.common_ω_rad(commonBarycenter);
			}
			if (commonBarycenter == fleet.barycenter())
			{
				this.initialMeanAnomaly_rad = fleet.meanAnomaly_Rad(base.launchTime);
				this.initialEpoch = fleet.epoch_DateTime;
			}
			else
			{
				this.initialMeanAnomaly_rad = originValue.common_M_rad(commonBarycenter, base.launchTime);
				this.initialEpoch = new TIDateTime();
				this.initialEpoch.SetTime(originValue.common_t0_jy(commonBarycenter));
			}
			this.initialOrbitalPeriod_s = 6.283185307179586 * Mathd.Sqrt(this.initialOrbit_m * this.initialOrbit_m * this.initialOrbit_m / commonBarycenter.mu);
		}

		// Token: 0x060047AB RID: 18347 RVA: 0x001D4B8A File Offset: 0x001D2D8A
		public override bool isInMicrothrust(TIDateTime time = null)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			return time >= base.launchTime && time <= base.arrivalTime;
		}

		// Token: 0x060047AC RID: 18348 RVA: 0x001D4BB8 File Offset: 0x001D2DB8
		public override bool isPlausible()
		{
			if (this.initialEcc <= 0.0)
			{
				Log.Error("Microthrust transfer implausible: initial eccentricity is " + this.initialEcc.ToString() + " which would require a hyperbolic 'orbit'.", Array.Empty<object>());
				return false;
			}
			if (this.destinationEcc <= 0.0)
			{
				Log.Error("Microthrust transfer implausible: final eccentricity is " + this.destinationEcc.ToString() + " which would require a hyperbolic 'orbit'.", Array.Empty<object>());
				return false;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			double distFromBarycenterAtTime_m = this.getDistFromBarycenterAtTime_m(base.launchTime, out tinaturalSpaceObjectState);
			double num = Mathd.Abs(this.getDistFromBarycenterAtTime_m(new TIDateTime(base.launchTime, 1.0), out tinaturalSpaceObjectState) - distFromBarycenterAtTime_m);
			double num2 = Mathd.Sqrt(base.commonBarycenter.mu / distFromBarycenterAtTime_m);
			if (num / num2 > 1.0)
			{
				Log.Error(string.Concat(new string[]
				{
					"Microthrust transfer implausible: initial vertical motion is ",
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
			double distFromBarycenterAtTime_m2 = this.getDistFromBarycenterAtTime_m(base.arrivalTime, out tinaturalSpaceObjectState);
			double distFromBarycenterAtTime_m3 = this.getDistFromBarycenterAtTime_m(new TIDateTime(base.arrivalTime, -1.0), out tinaturalSpaceObjectState);
			double num3 = Mathd.Abs(distFromBarycenterAtTime_m2 - distFromBarycenterAtTime_m3);
			double num4 = Mathd.Sqrt(base.commonBarycenter.mu / distFromBarycenterAtTime_m2);
			if (num3 / num4 > 1.0)
			{
				Log.Error(string.Concat(new string[]
				{
					"Microthrust transfer implausible: final vertical motion is ",
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

		// Token: 0x060047AD RID: 18349 RVA: 0x001D4DD8 File Offset: 0x001D2FD8
		public override CartesianState ToGlobalCartesianStateAtTime(TIDateTime timeToCheck)
		{
			double num;
			double num2;
			if (base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, false, out num, out num2) == TrajectoryPhase.Arrive)
			{
				return base.DestinationCartesianStateAtTime(timeToCheck);
			}
			return base.GetOrbitalElementsAtTime(timeToCheck).ToCartesianStateAtTime(timeToCheck.ExportTime(), base.commonBarycenter.mass_kg).ToGlobal(base.commonBarycenter, timeToCheck);
		}

		// Token: 0x060047AE RID: 18350 RVA: 0x001D4E3C File Offset: 0x001D303C
		public override OrbitalElementsState GetOrbitalElementsAtTime(TIDateTime timeToCheck, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			if ((timeToCheck < base.launchTime && base.fleet.ref_orbit != null) || timeToCheck > base.arrivalTime)
			{
				return base.GetOrbitalElementsAtTime(timeToCheck, precision);
			}
			double mu = base.commonBarycenter.mu;
			double num = timeToCheck.DifferenceInSeconds(base.launchTime);
			double num2 = this.initialVelocity_mps - base.fleetCruiseAcceleration_mps2 * num;
			double num3 = mu / (num2 * num2);
			double num4 = this.initialMeanAnomaly_rad + (this.FourthPower(this.initialVelocity_mps) - this.FourthPower(num2)) / (4.0 * base.fleetCruiseAcceleration_mps2 * mu);
			double num5 = Mathd.Clamp01(num / (base.duration_s - this.loiterDuration_s));
			num4 = num4 + this.initialNode_rad + this.initialArgP_rad - this.transferOrbit.longAscendingNode_Rad - this.transferOrbit.argPeriapsis_Rad;
			num4 = Mathd.Normalize_Rad(num4);
			return new OrbitalElementsState(Mathd.LerpRadians(this.initialNode_rad, this.destinationNode_rad, num5), Mathd.LerpRadians(this.initialArgP_rad, this.destinationArgP_rad, num5), Mathd.LerpRadians(this.initialInclination_rad, this.destinationInclination_rad, num5), num3, Mathd.Lerp(this.initialEcc, this.destinationEcc, num5), num4, timeToCheck.ExportTime());
		}

		// Token: 0x060047AF RID: 18351 RVA: 0x001D4F84 File Offset: 0x001D3184
		public override double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
		{
			if (timeToCheck < base.launchTime || timeToCheck > base.arrivalTime)
			{
				return base.getDistFromBarycenterAtTime_m(timeToCheck, out barycenter);
			}
			double num = timeToCheck.DifferenceInSeconds(base.launchTime);
			barycenter = base.commonBarycenter;
			double num2 = this.initialVelocity_mps - base.fleetCruiseAcceleration_mps2 * num;
			return barycenter.mu / (num2 * num2);
		}

		// Token: 0x060047B0 RID: 18352 RVA: 0x001D4FE8 File Offset: 0x001D31E8
		public override double RemainingDVatTime_mps(TIDateTime time)
		{
			if (time < base.launchTime)
			{
				return this.DV_mps;
			}
			if (!(time > base.arrivalTime))
			{
				return base.arrivalTime.DifferenceInSeconds(time) * Mathd.Abs(base.fleetCruiseAcceleration_mps2) + base.PostTransferDVfromTargetFleet_mps();
			}
			if (base.targetingFleet && base.destinationFleet.transferAssigned && base.destinationFleet.trajectory.launchTime < base.arrivalTime)
			{
				return base.destinationFleet.trajectory.RemainingDVatTime_mps(time);
			}
			return 0.0;
		}

		// Token: 0x060047B1 RID: 18353 RVA: 0x001D5088 File Offset: 0x001D3288
		public override Vector3d PositionAtTime(TIDateTime timeToCheck, bool setPosition, out bool arrived)
		{
			double num = 0.0;
			double num2;
			double num3;
			TrajectoryPhase trajectoryPhase = base.GetTrajectoryPhase(base.assignedTime, base.launchTime, timeToCheck, setPosition, out num2, out num3);
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
				if (setPosition)
				{
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					base.fleet.SetAccelerationPhaseStatus(true, true, false);
					num = (timeToCheck - base.launchTime).TotalSeconds * Mathd.Abs(base.fleetCruiseAcceleration_mps2);
				}
				break;
			case TrajectoryPhase.Coast:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
				}
				num = this.boostDV_mps;
				break;
			case TrajectoryPhase.Deceleration:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(true, true, false);
					double num4 = num3 - this.coastDuration_s - this.boostDuration_s;
					num = this.boostDV_mps + num4 * Mathd.Abs(base.fleetCruiseAcceleration_mps2);
				}
				break;
			case TrajectoryPhase.Arrive:
				if (setPosition)
				{
					base.fleet.SetAccelerationPhaseStatus(false, false, false);
					base.fleet.SetDecelerationPhaseStatus(false, false, false);
					num = this.boostDV_mps + this.decelDV_mps;
				}
				break;
			}
			double mu = base.commonBarycenter.mu;
			double num5 = this.initialVelocity_mps - base.fleetCruiseAcceleration_mps2 * num3;
			double num6 = mu / (num5 * num5);
			double num7 = this.initialMeanAnomaly_rad + (this.FourthPower(this.initialVelocity_mps) - this.FourthPower(num5)) / (4.0 * base.fleetCruiseAcceleration_mps2 * mu);
			double num8 = num3 / (base.duration_s - this.loiterDuration_s);
			this.transferOrbit.semiMajorAxis_m = num6;
			this.transferOrbit.eccentricity = Mathd.Lerp(this.initialEcc, this.destinationEcc, num8);
			this.transferOrbit.inclination_Rad = Mathd.LerpRadians(this.initialInclination_rad, this.destinationInclination_rad, num8);
			this.transferOrbit.longAscendingNode_Rad = Mathd.LerpRadians(this.initialNode_rad, this.destinationNode_rad, num8);
			this.transferOrbit.argPeriapsis_Rad = Mathd.LerpRadians(this.initialArgP_rad, this.destinationArgP_rad, num8);
			num7 = num7 + this.initialNode_rad + this.initialArgP_rad - this.transferOrbit.longAscendingNode_Rad - this.transferOrbit.argPeriapsis_Rad;
			num7 = Mathd.Normalize_Rad(num7);
			this.transferOrbit.meanAnomalyAtEpoch_Rad = num7;
			this.transferOrbit.epoch = timeToCheck.ExportTime();
			CartesianState cartesianState2 = this.transferOrbit.ToCartesianStateAtTime(timeToCheck.ExportTime(), base.commonBarycenter.mass_kg);
			Vector3d xzy = (base.commonBarycenter.SpatialRotation * cartesianState2.positionDisplay).xzy;
			this.velocity = (base.commonBarycenter.SpatialRotation * cartesianState2.velocityDisplay).xzy;
			cartesianState2 = base.commonBarycenter.ToGlobalCartesianStateAtTime(timeToCheck) + new CartesianState(xzy, this.velocity);
			if (setPosition && trajectoryPhase > TrajectoryPhase.Loiter)
			{
				double num9 = base.fleet.fleetTrajectoryData.initialDeltaV_mps - (double)base.fleet.currentDeltaV_mps;
				float DVToConsume_kps = (float)(num - num9) / 1000f;
				if (DVToConsume_kps > 0f)
				{
					base.fleet.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.ConsumeDeltaV(DVToConsume_kps, false);
					});
				}
			}
			arrived = num2 >= base.duration_s;
			return cartesianState2.position;
		}

		// Token: 0x060047B2 RID: 18354 RVA: 0x001D545C File Offset: 0x001D365C
		private double FourthPower(double x)
		{
			return x * x * x * x;
		}

		// Token: 0x060047B3 RID: 18355 RVA: 0x001D5465 File Offset: 0x001D3665
		public override Vector3d DesiredOrientationVector_Acceleration()
		{
			return this.velocity.normalized;
		}

		// Token: 0x060047B4 RID: 18356 RVA: 0x001D5472 File Offset: 0x001D3672
		public override Vector3d DesiredOrientationVector_Deceleration()
		{
			return this.velocity.normalized;
		}

		// Token: 0x060047B5 RID: 18357 RVA: 0x001D5480 File Offset: 0x001D3680
		public override string deepDump()
		{
			string text = "   Trajectory_Microthrust:\n";
			base.appendCommonDeepDump(ref text);
			string[] array = new string[32];
			array[0] = text;
			array[1] = "    initialVelocity_mps    = ";
			array[2] = this.initialVelocity_mps.ToString();
			array[3] = "m/s\n    initialOrbitalPeriod_s = ";
			array[4] = this.initialOrbitalPeriod_s.ToString();
			array[5] = "s\n    initialEpoch           = ";
			int num = 6;
			TIDateTime tidateTime = this.initialEpoch;
			array[num] = ((tidateTime != null) ? tidateTime.ToString() : null);
			array[7] = "\n    initialOrbit_m         = ";
			array[8] = this.initialOrbit_m.ToString();
			array[9] = "m\n    initialEcc             = ";
			array[10] = this.initialEcc.ToString();
			array[11] = "\n    initialNode_rad        = ";
			array[12] = this.initialNode_rad.ToString();
			array[13] = "rad\n    initialInclination_rad = ";
			array[14] = this.initialInclination_rad.ToString();
			array[15] = "rad\n    initialArgP_rad        = ";
			array[16] = this.initialArgP_rad.ToString();
			array[17] = "rad\n    initialMeanAnomaly_rad = ";
			array[18] = this.initialMeanAnomaly_rad.ToString();
			array[19] = "rad\n    destinationOrbit_m         = ";
			array[20] = this.destinationOrbit_m.ToString();
			array[21] = "m\n    destinationEcc             = ";
			array[22] = this.destinationEcc.ToString();
			array[23] = "\n    destinationNode_rad        = ";
			array[24] = this.destinationNode_rad.ToString();
			array[25] = "rad\n    destinationInclination_rad = ";
			array[26] = this.destinationInclination_rad.ToString();
			array[27] = "rad\n    destinationArgP_rad        = ";
			array[28] = this.destinationArgP_rad.ToString();
			array[29] = "rad\n    destinationMeanAnomaly_rad = ";
			array[30] = this.destinationMeanAnomaly_rad.ToString();
			array[31] = "rad\n";
			text = string.Concat(array);
			base.appendCommonDeepDumpPostscript(ref text);
			return text;
		}

		// Token: 0x04002981 RID: 10625
		public double initialOrbit_m;

		// Token: 0x04002982 RID: 10626
		public double destinationOrbit_m;

		// Token: 0x04002983 RID: 10627
		public double initialEcc;

		// Token: 0x04002984 RID: 10628
		public double destinationEcc;

		// Token: 0x04002985 RID: 10629
		public double initialNode_rad;

		// Token: 0x04002986 RID: 10630
		public double destinationNode_rad;

		// Token: 0x04002987 RID: 10631
		public double initialInclination_rad;

		// Token: 0x04002988 RID: 10632
		public double destinationInclination_rad;

		// Token: 0x04002989 RID: 10633
		public double initialArgP_rad;

		// Token: 0x0400298A RID: 10634
		public double destinationArgP_rad;

		// Token: 0x0400298B RID: 10635
		public bool ascending;

		// Token: 0x0400298C RID: 10636
		public double initialVelocity_mps;

		// Token: 0x0400298D RID: 10637
		public double initialMeanAnomaly_rad;

		// Token: 0x0400298E RID: 10638
		public double destinationMeanAnomaly_rad;

		// Token: 0x0400298F RID: 10639
		public double initialOrbitalPeriod_s;

		// Token: 0x04002990 RID: 10640
		public Vector3d velocity;

		// Token: 0x04002991 RID: 10641
		public TIDateTime initialEpoch;
	}
}
