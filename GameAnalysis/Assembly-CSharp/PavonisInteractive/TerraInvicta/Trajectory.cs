using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FullSerializer;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C8 RID: 1992
	public abstract class Trajectory
	{
		// Token: 0x17000DFA RID: 3578
		// (get) Token: 0x0600470A RID: 18186 RVA: 0x001D069F File Offset: 0x001CE89F
		// (set) Token: 0x0600470B RID: 18187 RVA: 0x001D06BB File Offset: 0x001CE8BB
		[fsIgnore]
		public IMobileAsset fleet
		{
			get
			{
				if (this._fleet == null)
				{
					this._fleet = this.fleetAsSpaceFleetState;
				}
				return this._fleet;
			}
			set
			{
				this._fleet = value;
			}
		}

		// Token: 0x17000DFB RID: 3579
		// (get) Token: 0x0600470C RID: 18188 RVA: 0x001D06C4 File Offset: 0x001CE8C4
		// (set) Token: 0x0600470D RID: 18189 RVA: 0x001D06CC File Offset: 0x001CE8CC
		public TINaturalSpaceObjectState commonBarycenter { get; protected set; }

		// Token: 0x17000DFC RID: 3580
		// (get) Token: 0x0600470E RID: 18190 RVA: 0x001D06D5 File Offset: 0x001CE8D5
		// (set) Token: 0x0600470F RID: 18191 RVA: 0x001D06DD File Offset: 0x001CE8DD
		public TIOrbitState originOrbit { get; protected set; }

		// Token: 0x17000DFD RID: 3581
		// (get) Token: 0x06004710 RID: 18192 RVA: 0x001D06E6 File Offset: 0x001CE8E6
		// (set) Token: 0x06004711 RID: 18193 RVA: 0x001D06EE File Offset: 0x001CE8EE
		public TIOrbitState destinationOrbit { get; protected set; }

		// Token: 0x17000DFE RID: 3582
		// (get) Token: 0x06004712 RID: 18194 RVA: 0x001D06F7 File Offset: 0x001CE8F7
		// (set) Token: 0x06004713 RID: 18195 RVA: 0x001D06FF File Offset: 0x001CE8FF
		public TISpaceFleetState destinationFleet { get; protected set; }

		// Token: 0x17000DFF RID: 3583
		// (get) Token: 0x06004714 RID: 18196 RVA: 0x001D0708 File Offset: 0x001CE908
		// (set) Token: 0x06004715 RID: 18197 RVA: 0x001D0710 File Offset: 0x001CE910
		public TISpaceFleetState prevDestinationFleet { get; protected set; }

		// Token: 0x17000E00 RID: 3584
		// (get) Token: 0x06004716 RID: 18198 RVA: 0x001D0719 File Offset: 0x001CE919
		// (set) Token: 0x06004717 RID: 18199 RVA: 0x001D0721 File Offset: 0x001CE921
		public TIHabState destinationStation { get; protected set; }

		// Token: 0x17000E01 RID: 3585
		// (get) Token: 0x06004718 RID: 18200 RVA: 0x001D072A File Offset: 0x001CE92A
		// (set) Token: 0x06004719 RID: 18201 RVA: 0x001D0732 File Offset: 0x001CE932
		public TISpaceGameState destination { get; protected set; }

		// Token: 0x17000E02 RID: 3586
		// (get) Token: 0x0600471A RID: 18202 RVA: 0x001D073B File Offset: 0x001CE93B
		// (set) Token: 0x0600471B RID: 18203 RVA: 0x001D0743 File Offset: 0x001CE943
		public Trajectory destinationFleetTrajectory { get; set; }

		// Token: 0x17000E03 RID: 3587
		// (get) Token: 0x0600471C RID: 18204 RVA: 0x001D074C File Offset: 0x001CE94C
		// (set) Token: 0x0600471D RID: 18205 RVA: 0x001D0754 File Offset: 0x001CE954
		public double fleetCruiseAcceleration_mps2 { get; protected set; }

		// Token: 0x17000E04 RID: 3588
		// (get) Token: 0x0600471E RID: 18206 RVA: 0x001D075D File Offset: 0x001CE95D
		// (set) Token: 0x0600471F RID: 18207 RVA: 0x001D0765 File Offset: 0x001CE965
		public virtual double boostDV_mps { get; protected set; }

		// Token: 0x17000E05 RID: 3589
		// (get) Token: 0x06004720 RID: 18208 RVA: 0x001D076E File Offset: 0x001CE96E
		// (set) Token: 0x06004721 RID: 18209 RVA: 0x001D0776 File Offset: 0x001CE976
		public virtual double decelDV_mps { get; protected set; }

		// Token: 0x17000E06 RID: 3590
		// (get) Token: 0x06004722 RID: 18210 RVA: 0x001D077F File Offset: 0x001CE97F
		// (set) Token: 0x06004723 RID: 18211 RVA: 0x001D0787 File Offset: 0x001CE987
		public TIDateTime assignedTime { get; protected set; }

		// Token: 0x17000E07 RID: 3591
		// (get) Token: 0x06004724 RID: 18212 RVA: 0x001D0790 File Offset: 0x001CE990
		// (set) Token: 0x06004725 RID: 18213 RVA: 0x001D0798 File Offset: 0x001CE998
		public TIDateTime launchTime { get; protected set; }

		// Token: 0x17000E08 RID: 3592
		// (get) Token: 0x06004726 RID: 18214 RVA: 0x001D07A1 File Offset: 0x001CE9A1
		// (set) Token: 0x06004727 RID: 18215 RVA: 0x001D07A9 File Offset: 0x001CE9A9
		public TIDateTime arrivalTime { get; protected set; }

		// Token: 0x17000E09 RID: 3593
		// (get) Token: 0x06004728 RID: 18216 RVA: 0x001D07B2 File Offset: 0x001CE9B2
		public TIDateTime finalArrivalTime
		{
			get
			{
				if (this.nextTrajectory != null)
				{
					return this.nextTrajectory.finalArrivalTime;
				}
				return this.arrivalTime;
			}
		}

		// Token: 0x17000E0A RID: 3594
		// (get) Token: 0x06004729 RID: 18217 RVA: 0x001D07CE File Offset: 0x001CE9CE
		// (set) Token: 0x0600472A RID: 18218 RVA: 0x001D07D6 File Offset: 0x001CE9D6
		public Vector3d launchPosition { get; protected set; }

		// Token: 0x17000E0B RID: 3595
		// (get) Token: 0x0600472B RID: 18219 RVA: 0x001D07DF File Offset: 0x001CE9DF
		// (set) Token: 0x0600472C RID: 18220 RVA: 0x001D07E7 File Offset: 0x001CE9E7
		public Vector3d destinationPosition { get; protected set; }

		// Token: 0x17000E0C RID: 3596
		// (get) Token: 0x0600472D RID: 18221 RVA: 0x001D07F0 File Offset: 0x001CE9F0
		public bool endsInCrash
		{
			get
			{
				return this.collisionTarget != null;
			}
		}

		// Token: 0x17000E0D RID: 3597
		// (get) Token: 0x0600472E RID: 18222 RVA: 0x001D07FE File Offset: 0x001CE9FE
		public bool destroyOnArrival
		{
			get
			{
				return this.endsInCrash || this.exitsSolarSystem;
			}
		}

		// Token: 0x17000E0E RID: 3598
		// (get) Token: 0x0600472F RID: 18223 RVA: 0x001D0810 File Offset: 0x001CEA10
		public double straightLineDistance_m
		{
			get
			{
				if (this._straightLineDistance_m <= 0.0)
				{
					Vector3d launchPosition = this.launchPosition;
					Vector3d destinationPosition = this.destinationPosition;
					this._straightLineDistance_m = Vector3d.Distance(in launchPosition, in destinationPosition);
				}
				return this._straightLineDistance_m;
			}
		}

		// Token: 0x17000E0F RID: 3599
		// (get) Token: 0x06004730 RID: 18224 RVA: 0x001D0851 File Offset: 0x001CEA51
		// (set) Token: 0x06004731 RID: 18225 RVA: 0x001D0859 File Offset: 0x001CEA59
		public bool aerocapture { get; protected set; }

		// Token: 0x17000E10 RID: 3600
		// (get) Token: 0x06004732 RID: 18226 RVA: 0x001D0862 File Offset: 0x001CEA62
		// (set) Token: 0x06004733 RID: 18227 RVA: 0x001D086A File Offset: 0x001CEA6A
		public bool flyby { get; protected set; }

		// Token: 0x17000E11 RID: 3601
		// (get) Token: 0x06004734 RID: 18228 RVA: 0x001D0873 File Offset: 0x001CEA73
		// (set) Token: 0x06004735 RID: 18229 RVA: 0x001D087B File Offset: 0x001CEA7B
		public bool resupplyOnArrival { get; protected set; }

		// Token: 0x17000E12 RID: 3602
		// (get) Token: 0x06004736 RID: 18230 RVA: 0x001D0884 File Offset: 0x001CEA84
		// (set) Token: 0x06004737 RID: 18231 RVA: 0x001D088C File Offset: 0x001CEA8C
		public bool interceptTrajectory { get; protected set; }

		// Token: 0x17000E13 RID: 3603
		// (get) Token: 0x06004738 RID: 18232 RVA: 0x001D0895 File Offset: 0x001CEA95
		// (set) Token: 0x06004739 RID: 18233 RVA: 0x001D089D File Offset: 0x001CEA9D
		public TimeSpan duration { get; protected set; }

		// Token: 0x0600473A RID: 18234 RVA: 0x001D08A6 File Offset: 0x001CEAA6
		public virtual bool HasOrbitalElements()
		{
			return false;
		}

		// Token: 0x17000E14 RID: 3604
		// (get) Token: 0x0600473B RID: 18235 RVA: 0x001D08A9 File Offset: 0x001CEAA9
		public double duration_h
		{
			get
			{
				return this.duration_s / 3600.0;
			}
		}

		// Token: 0x17000E15 RID: 3605
		// (get) Token: 0x0600473C RID: 18236 RVA: 0x001D08BB File Offset: 0x001CEABB
		public double duration_d
		{
			get
			{
				return this.duration_s / 86400.0;
			}
		}

		// Token: 0x17000E16 RID: 3606
		// (get) Token: 0x0600473D RID: 18237 RVA: 0x001D08CD File Offset: 0x001CEACD
		public double duration_w
		{
			get
			{
				return this.duration_d / 7.0;
			}
		}

		// Token: 0x17000E17 RID: 3607
		// (get) Token: 0x0600473E RID: 18238 RVA: 0x001D08E0 File Offset: 0x001CEAE0
		public double duration_s
		{
			get
			{
				TIDateTime arrivalTime = this.arrivalTime;
				if (arrivalTime == null)
				{
					return this.loiterDuration_s + this.prepositionDuration_s + this.boostDuration_s + this.coastDuration_s + this.decelDuration_s + this.captureDuration_s;
				}
				return arrivalTime.DifferenceInSeconds(this.assignedTime);
			}
		}

		// Token: 0x17000E18 RID: 3608
		// (get) Token: 0x0600473F RID: 18239 RVA: 0x001D092C File Offset: 0x001CEB2C
		public double durationFromLaunchToFinalArrival_s
		{
			get
			{
				return this.finalArrivalTime.DifferenceInSeconds(this.launchTime);
			}
		}

		// Token: 0x17000E19 RID: 3609
		// (get) Token: 0x06004740 RID: 18240 RVA: 0x001D093F File Offset: 0x001CEB3F
		public double flightDuration_s
		{
			get
			{
				TIDateTime arrivalTime = this.arrivalTime;
				if (arrivalTime == null)
				{
					return this.prepositionDuration_s + this.boostDuration_s + this.coastDuration_s + this.decelDuration_s + this.captureDuration_s;
				}
				return arrivalTime.DifferenceInSeconds(this.launchTime);
			}
		}

		// Token: 0x17000E1A RID: 3610
		// (get) Token: 0x06004741 RID: 18241 RVA: 0x001D0979 File Offset: 0x001CEB79
		public virtual double DV_mps
		{
			get
			{
				return this.boostDV_mps + this.decelDV_mps + this.DV_targetFleet_mps;
			}
		}

		// Token: 0x17000E1B RID: 3611
		// (get) Token: 0x06004742 RID: 18242 RVA: 0x001D098F File Offset: 0x001CEB8F
		public double DV_kps
		{
			get
			{
				return this.DV_mps / 1000.0;
			}
		}

		// Token: 0x17000E1C RID: 3612
		// (get) Token: 0x06004743 RID: 18243 RVA: 0x001D09A1 File Offset: 0x001CEBA1
		public bool targetingFleet
		{
			get
			{
				return TIGameState.Valid(this.destinationFleet);
			}
		}

		// Token: 0x17000E1D RID: 3613
		// (get) Token: 0x06004744 RID: 18244 RVA: 0x001D09AE File Offset: 0x001CEBAE
		public bool targetingStation
		{
			get
			{
				return TIGameState.Valid(this.destinationStation);
			}
		}

		// Token: 0x17000E1E RID: 3614
		// (get) Token: 0x06004745 RID: 18245 RVA: 0x001D09BB File Offset: 0x001CEBBB
		public bool targetingOrbit
		{
			get
			{
				return !this.targetingFleet && !this.targetingStation;
			}
		}

		// Token: 0x06004746 RID: 18246
		public abstract string GetDisplayName();

		// Token: 0x06004747 RID: 18247 RVA: 0x001D09D0 File Offset: 0x001CEBD0
		public Trajectory ShallowCopy(IMobileAsset newFleet = null)
		{
			Trajectory trajectory = (Trajectory)base.MemberwiseClone();
			if (newFleet != null)
			{
				trajectory.fleet = newFleet;
			}
			if (trajectory.nextTrajectory != null)
			{
				trajectory.nextTrajectory = trajectory.nextTrajectory.ShallowCopy(newFleet);
			}
			return trajectory;
		}

		// Token: 0x06004748 RID: 18248 RVA: 0x001D0A10 File Offset: 0x001CEC10
		public virtual double RemainingDVatTime_mps(TIDateTime time)
		{
			if (time < this.launchTime)
			{
				return (double)((float)this.DV_mps);
			}
			if (!(time > this.arrivalTime))
			{
				double num;
				double num2;
				double num3;
				switch (this.GetTrajectoryPhase(this.assignedTime, this.launchTime, time, false, out num, out num2))
				{
				case TrajectoryPhase.Loiter:
					num3 = this.DV_mps;
					goto IL_0102;
				case TrajectoryPhase.Boost:
					num3 = this.DV_mps - num2 * this.fleetCruiseAcceleration_mps2;
					goto IL_0102;
				case TrajectoryPhase.Coast:
					num3 = this.decelDV_mps;
					goto IL_0102;
				case TrajectoryPhase.Deceleration:
				case TrajectoryPhase.Capture:
				{
					double num4 = num2 - this.coastDuration_s - this.boostDuration_s;
					num3 = this.decelDV_mps - num4 * this.fleetCruiseAcceleration_mps2;
					goto IL_0102;
				}
				}
				num3 = 0.0;
				IL_0102:
				return num3 + this.PostTransferDVfromTargetFleet_mps();
			}
			if (this.targetingFleet && this.destinationFleet.transferAssigned && this.destinationFleet.trajectory.launchTime < this.arrivalTime)
			{
				return this.destinationFleet.trajectory.RemainingDVatTime_mps(time);
			}
			return 0.0;
		}

		// Token: 0x06004749 RID: 18249 RVA: 0x001D0B2C File Offset: 0x001CED2C
		protected double PostTransferDVfromTargetFleet_mps()
		{
			if (!this.targetingFleet)
			{
				return 0.0;
			}
			if (!this.destinationFleet.transferAssigned)
			{
				return 0.0;
			}
			if (this.destinationFleet.trajectory.launchTime > this.arrivalTime)
			{
				return 0.0;
			}
			if (this.destinationFleet.trajectory.destinationFleet == this.fleetAsSpaceFleetState)
			{
				return 0.0;
			}
			if (!MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(this.destinationFleet, this.fleet.faction))
			{
				return 0.0;
			}
			return this.destinationFleet.trajectory.RemainingDVatTime_mps(this.arrivalTime);
		}

		// Token: 0x0600474A RID: 18250
		public abstract bool isPlausible();

		// Token: 0x0600474B RID: 18251
		public abstract string deepDump();

		// Token: 0x0600474C RID: 18252 RVA: 0x001D0BE8 File Offset: 0x001CEDE8
		public void appendCommonDeepDump(ref string output)
		{
			TISpaceFleetState tispaceFleetState = this.fleet as TISpaceFleetState;
			if (tispaceFleetState != null)
			{
				output = output + "    fleet     = " + tispaceFleetState.displayName + "\n";
			}
			else
			{
				TIVirtualSpaceFleet tivirtualSpaceFleet = this.fleet as TIVirtualSpaceFleet;
				if (tivirtualSpaceFleet != null)
				{
					output = output + "    virtual fleet:      faction     = " + tivirtualSpaceFleet.faction.displayName + "\n";
				}
			}
			string[] array = new string[22];
			array[0] = output;
			array[1] = "     fleet DV    = ";
			array[2] = this.fleet.currentDeltaV_mps.ToString();
			array[3] = "m/s\n     fleet accel = ";
			array[4] = this.fleet.cruiseAcceleration_mps2.ToString();
			array[5] = "m/s2\n    DV               = ";
			array[6] = this.DV_mps.ToString();
			array[7] = "m/s\n    assignedTime     = ";
			int num = 8;
			TIDateTime assignedTime = this.assignedTime;
			array[num] = ((assignedTime != null) ? assignedTime.ToString() : null);
			array[9] = "\n    launchTime       = ";
			int num2 = 10;
			TIDateTime launchTime = this.launchTime;
			array[num2] = ((launchTime != null) ? launchTime.ToString() : null);
			array[11] = "\n    arrivalTime      = ";
			int num3 = 12;
			TIDateTime arrivalTime = this.arrivalTime;
			array[num3] = ((arrivalTime != null) ? arrivalTime.ToString() : null);
			array[13] = "\n    commonBarycenter = ";
			int num4 = 14;
			TINaturalSpaceObjectState commonBarycenter = this.commonBarycenter;
			array[num4] = ((commonBarycenter != null) ? commonBarycenter.displayName : null) ?? "null";
			array[15] = "\n    originOrbit      = ";
			int num5 = 16;
			TIOrbitState originOrbit = this.originOrbit;
			array[num5] = ((originOrbit != null) ? originOrbit.displayName : null) ?? "null";
			array[17] = "\n    destination      = ";
			int num6 = 18;
			TISpaceGameState destination = this.destination;
			array[num6] = ((destination != null) ? destination.displayName : null) ?? "null";
			array[19] = "\n    destinationOrbit = ";
			int num7 = 20;
			TIOrbitState destinationOrbit = this.destinationOrbit;
			array[num7] = ((destinationOrbit != null) ? destinationOrbit.displayName : null) ?? "null";
			array[21] = "\n";
			output = string.Concat(array);
		}

		// Token: 0x0600474D RID: 18253 RVA: 0x001D0DB8 File Offset: 0x001CEFB8
		public void appendCommonDeepDumpPostscript(ref string output)
		{
			if (this.nextTrajectory != null)
			{
				output = output + "    there is a nextTrajectory, as follows:\n" + this.nextTrajectory.deepDump();
				return;
			}
			TISpaceFleetState destinationFleet = this.destinationFleet;
			if (((destinationFleet != null) ? destinationFleet.trajectory : null) != null)
			{
				output = output + "    the destinationFleet has a trajectory, as follows:\n" + this.destinationFleet.trajectory.deepDump();
			}
		}

		// Token: 0x0600474E RID: 18254 RVA: 0x001D0E1C File Offset: 0x001CF01C
		[return: TupleElementNames(new string[] { "orbit", "time" })]
		public ValueTuple<TIOrbitState, TIDateTime> getFinalOrbitAndArrivalTime()
		{
			if (this.nextTrajectory != null)
			{
				return this.nextTrajectory.getFinalOrbitAndArrivalTime();
			}
			TISpaceFleetState destinationFleet = this.destinationFleet;
			TIDateTime tidateTime;
			if (destinationFleet == null)
			{
				tidateTime = null;
			}
			else
			{
				Trajectory trajectory = destinationFleet.trajectory;
				tidateTime = ((trajectory != null) ? trajectory.arrivalTime : null);
			}
			if (tidateTime > this.arrivalTime)
			{
				return this.destinationFleet.trajectory.getFinalOrbitAndArrivalTime();
			}
			if (this.destroyOnArrival)
			{
				return new ValueTuple<TIOrbitState, TIDateTime>(null, this.arrivalTime);
			}
			return new ValueTuple<TIOrbitState, TIDateTime>(this.destinationOrbit, this.arrivalTime);
		}

		// Token: 0x0600474F RID: 18255 RVA: 0x001D0EA0 File Offset: 0x001CF0A0
		public void DestinationDestroyed()
		{
			if (this.targetingFleet && this.destinationFleet.transferAssigned && this.destinationFleet.trajectory.arrivalTime > this.arrivalTime && this.destinationFleet.trajectory.launchTime < TITimeState.Now())
			{
				this.prevDestinationFleet = this.destinationFleet;
				this.nextTrajectory = this.destinationFleet.trajectory.ShallowCopy(this.fleet);
				this.destinationOrbit = this.destinationFleet.trajectory.destinationOrbit;
				this.destination = this.destinationOrbit;
				this.destinationFleet = null;
				this.destinationFleetTrajectory = null;
				this.SetAsIntercept(true);
				return;
			}
			this.destination = this.destinationOrbit;
			if (this.destination == null)
			{
				TISpaceFleetState tispaceFleetState = this.fleet as TISpaceFleetState;
				if (tispaceFleetState != null)
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						tispaceFleetState.displayName,
						"'s target fleet (",
						this.destinationFleet.displayName,
						") was destroyed.  ",
						tispaceFleetState.displayName,
						" doesn't have a destinationOrbit to default to.  Reconstructing destination orbit."
					}));
				}
				else
				{
					Debug.LogWarning(this.destinationFleet.displayName + " was destroyed and a 'fleet' was targeting it, but the 'fleet' doesn't have a destinationOrbit to default to.  That 'fleet' isn't a TISpaceFleetState, and thus is almost certainly a fictional fleet from the Transfer Plotter.  This state should be impossible since trajectories in the Transfer Plotter should not persist over time.  We're reconstructing the destination orbit to keep the trajectory valid, but there is a deeper bug here.");
				}
				this.ReconstructMissingDestinationOrbit();
			}
			this.destinationFleet = null;
			this.destinationStation = null;
			this.destinationFleetTrajectory = null;
			this.SetAsIntercept(true);
		}

		// Token: 0x06004750 RID: 18256 RVA: 0x001D1010 File Offset: 0x001CF210
		public void ChangeDestinationFleet(TISpaceFleetState newDestination)
		{
			if (newDestination == this.fleet as TISpaceFleetState)
			{
				Log.Error("Attempted to change the destination of a trajectory to the fleet performing the trajectory.  Setting the destination to the target orbit, or no destination, instead.", Array.Empty<object>());
				this.DestinationDestroyed();
				return;
			}
			TISpaceFleetState tispaceFleetState = this.fleet as TISpaceFleetState;
			if (tispaceFleetState != null)
			{
				if (tispaceFleetState.CheckForTransferTargetLoop().Contains(newDestination))
				{
					Log.Error("Attempted to change the destination of a trajectory to a fleet that would cause a transfer target loop.  Setting the destination to the targbet orbit, or no destination, instead.", Array.Empty<object>());
					this.DestinationDestroyed();
					return;
				}
				OperationData operationData = tispaceFleetState.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation is TransferOperation);
				if (operationData != null)
				{
					operationData.ChangeTarget(newDestination);
				}
			}
			this.destinationFleet = newDestination;
			this.destination = this.destinationFleet;
		}

		// Token: 0x06004751 RID: 18257 RVA: 0x001D10C4 File Offset: 0x001CF2C4
		public void DeTargetFleet()
		{
			if (!this.targetingFleet)
			{
				return;
			}
			if (this.destinationFleetTrajectory != null && this.destinationFleetTrajectory.launchTime < TITimeState.Now() && this.destinationFleetTrajectory.arrivalTime > TITimeState.Now())
			{
				this.nextTrajectory = this.destinationFleetTrajectory.ShallowCopy(this.fleet);
				this.destination = this.nextTrajectory.destination;
				this.destinationOrbit = this.nextTrajectory.destinationOrbit;
				this.destinationFleet = this.nextTrajectory.destinationFleet;
				this.destinationStation = this.nextTrajectory.destinationStation;
				this.destinationFleetTrajectory = null;
				this.destinationFleet = null;
				return;
			}
			if (this.destinationOrbit != null)
			{
				this.destinationFleetTrajectory = null;
				this.destination = this.destinationOrbit;
				this.destinationFleet = null;
				return;
			}
			this.destinationFleetTrajectory = null;
			TISpaceFleetState tispaceFleetState = this.fleet as TISpaceFleetState;
			if (tispaceFleetState != null)
			{
				Debug.LogWarning(tispaceFleetState.displayName + " has de-targeted " + this.destinationFleet.displayName + " but doesn't have a destinationOrbit to default to.  Reconstructing destination orbit.");
			}
			else
			{
				Debug.LogWarning("A 'fleet' has de-targeted " + this.destinationFleet.displayName + " but doesn't have a destinationOrbit to default to.  That 'fleet' isn't a TISpaceFleetState, and thus is almost certainly a fictional fleet from the Transfer Plotter.  This state should be impossible since trajectories in the Transfer Plotter should not persist over time.  We're reconstructing the destination orbit to keep the trajectory valid, but there is a deeper bug here.");
			}
			this.ReconstructMissingDestinationOrbit();
			this.destinationFleet = null;
		}

		// Token: 0x06004752 RID: 18258 RVA: 0x001D1214 File Offset: 0x001CF414
		public void EnsureConsistentDestinationOrbitOnLoad()
		{
			if (this.destroyOnArrival)
			{
				return;
			}
			TISpaceFleetState tispaceFleetState = this.destination as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.DoIKnowThisFleetIsTransfering(this.fleet.faction) && tispaceFleetState.trajectory.launchTime < this.arrivalTime)
			{
				if (tispaceFleetState.trajectory.destinationOrbit != this.destinationOrbit)
				{
					string[] array = new string[7];
					array[0] = "Save repair: trajectory's destination is ";
					array[1] = this.destination.GetDisplayName(this.fleet.faction);
					array[2] = ", which is a transfering fleet who's destinationOrbit is ";
					int num = 3;
					TIOrbitState destinationOrbit = tispaceFleetState.trajectory.destinationOrbit;
					array[num] = ((destinationOrbit != null) ? destinationOrbit.GetDisplayName(this.fleet.faction) : null) ?? "null";
					array[4] = ".  Our destinationOrbit should match, but is instead: ";
					int num2 = 5;
					TIOrbitState destinationOrbit2 = this.destinationOrbit;
					array[num2] = ((destinationOrbit2 != null) ? destinationOrbit2.GetDisplayName(this.fleet.faction) : null) ?? "null";
					array[6] = ".";
					Log.Warn(string.Concat(array), Array.Empty<object>());
					this.destinationOrbit = tispaceFleetState.trajectory.destinationOrbit;
					return;
				}
			}
			else if (this.destinationOrbit != this.destination.ref_orbit)
			{
				string[] array2 = new string[7];
				array2[0] = "Save repair: trajectory's destination is ";
				array2[1] = this.destination.GetDisplayName(this.fleet.faction);
				array2[2] = " but its destinationOrbit is ";
				int num3 = 3;
				TIOrbitState destinationOrbit3 = this.destinationOrbit;
				array2[num3] = ((destinationOrbit3 != null) ? destinationOrbit3.GetDisplayName(this.fleet.faction) : null) ?? "null";
				array2[4] = ".  This should match the destination's ref_orbit, which is: ";
				int num4 = 5;
				TIOrbitState ref_orbit = this.destination.ref_orbit;
				array2[num4] = ((ref_orbit != null) ? ref_orbit.GetDisplayName(this.fleet.faction) : null) ?? "null";
				array2[6] = ".";
				Log.Warn(string.Concat(array2), Array.Empty<object>());
				this.destinationOrbit = this.destination.ref_orbit;
			}
		}

		// Token: 0x06004753 RID: 18259 RVA: 0x001D1410 File Offset: 0x001CF610
		public virtual TINaturalSpaceObjectState GetExactBarycenterAtTime(TIDateTime time)
		{
			TIOrbitState originOrbit = this.originOrbit;
			TINaturalSpaceObjectState tinaturalSpaceObjectState = ((originOrbit != null) ? originOrbit.barycenter : null);
			if (tinaturalSpaceObjectState == null)
			{
				TINaturalSpaceObjectState tinaturalSpaceObjectState2 = this.commonBarycenter;
				Vector3d position = this.ToGlobalCartesianStateAtTime(time).position;
				double num = (this.commonBarycenter.ToGlobalCartesianStateAtTime(time).position - position).sqrMagnitude;
				foreach (TINaturalSpaceObjectState tinaturalSpaceObjectState3 in GameStateManager.IterateByClass<TINaturalSpaceObjectState>(false))
				{
					double sqrMagnitude = (tinaturalSpaceObjectState3.ToGlobalCartesianStateAtTime(time).position - position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						tinaturalSpaceObjectState2 = tinaturalSpaceObjectState3;
						num = sqrMagnitude;
					}
				}
				if (this.IsInsideBarycenterSOI(tinaturalSpaceObjectState2, position, time))
				{
					tinaturalSpaceObjectState = tinaturalSpaceObjectState2;
				}
				else if (this.IsInsideBarycenterSOI(tinaturalSpaceObjectState2.barycenter, position, time))
				{
					tinaturalSpaceObjectState = tinaturalSpaceObjectState2.barycenter;
				}
				else
				{
					tinaturalSpaceObjectState = tinaturalSpaceObjectState2.barycenter.barycenter;
				}
			}
			if (time <= this.launchTime)
			{
				return tinaturalSpaceObjectState;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState4;
			if (this.destinationFleetTrajectory != null)
			{
				tinaturalSpaceObjectState4 = this.destinationFleetTrajectory.GetExactBarycenterAtTime(this.arrivalTime);
			}
			else if (this.destination != null)
			{
				tinaturalSpaceObjectState4 = this.destination.barycenter;
			}
			else if (this.collisionTarget != null)
			{
				tinaturalSpaceObjectState4 = this.collisionTarget;
			}
			else
			{
				tinaturalSpaceObjectState4 = this.commonBarycenter;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState5 = tinaturalSpaceObjectState.FindCommonBarycenter(tinaturalSpaceObjectState4);
			TINaturalSpaceObjectState tinaturalSpaceObjectState6;
			if (tinaturalSpaceObjectState == tinaturalSpaceObjectState5)
			{
				tinaturalSpaceObjectState = null;
				tinaturalSpaceObjectState6 = null;
			}
			else if (tinaturalSpaceObjectState.barycenter != tinaturalSpaceObjectState5)
			{
				tinaturalSpaceObjectState6 = tinaturalSpaceObjectState.barycenter;
			}
			else
			{
				tinaturalSpaceObjectState6 = null;
			}
			TINaturalSpaceObjectState tinaturalSpaceObjectState7;
			if (tinaturalSpaceObjectState4 == tinaturalSpaceObjectState5)
			{
				tinaturalSpaceObjectState4 = null;
				tinaturalSpaceObjectState7 = null;
			}
			else if (tinaturalSpaceObjectState4.barycenter != tinaturalSpaceObjectState5)
			{
				tinaturalSpaceObjectState7 = tinaturalSpaceObjectState4.barycenter;
			}
			else
			{
				tinaturalSpaceObjectState7 = null;
			}
			Vector3d position2 = this.ToGlobalCartesianStateAtTime(time).position;
			if (this.IsInsideBarycenterSOI(tinaturalSpaceObjectState, position2, time))
			{
				return tinaturalSpaceObjectState;
			}
			if (this.IsInsideBarycenterSOI(tinaturalSpaceObjectState4, position2, time))
			{
				return tinaturalSpaceObjectState4;
			}
			if (this.IsInsideBarycenterSOI(tinaturalSpaceObjectState6, position2, time))
			{
				return tinaturalSpaceObjectState6;
			}
			if (this.IsInsideBarycenterSOI(tinaturalSpaceObjectState7, position2, time))
			{
				return tinaturalSpaceObjectState7;
			}
			return tinaturalSpaceObjectState5;
		}

		// Token: 0x06004754 RID: 18260 RVA: 0x001D1628 File Offset: 0x001CF828
		private bool IsInsideBarycenterSOI(TINaturalSpaceObjectState barycenter, Vector3d fleetGlobalPosition, TIDateTime time)
		{
			if (barycenter == null)
			{
				return false;
			}
			if (barycenter.isSun)
			{
				return true;
			}
			double magnitude = (barycenter.ToGlobalCartesianStateAtTime(time).position - fleetGlobalPosition).magnitude;
			return barycenter.sphereOfInfluence_m > magnitude;
		}

		// Token: 0x06004755 RID: 18261 RVA: 0x001D1670 File Offset: 0x001CF870
		public virtual TINaturalSpaceObjectState GetBarycenterAtTime(TIDateTime time)
		{
			if (!(time <= this.launchTime))
			{
				if (time >= this.arrivalTime)
				{
					if (this.nextTrajectory != null)
					{
						return this.nextTrajectory.GetBarycenterAtTime(time);
					}
					if (this.targetingFleet)
					{
						if (this.destinationFleet.transferAssigned)
						{
							return this.destinationFleet.trajectory.GetBarycenterAtTime(time);
						}
						return this.destinationFleet.barycenter;
					}
					else
					{
						if (this.destination != null)
						{
							return this.destination.barycenter;
						}
						if (this.endsInCrash)
						{
							return this.collisionTarget;
						}
						if (this.exitsSolarSystem)
						{
							return GameStateManager.Sol();
						}
					}
				}
				return this.commonBarycenter;
			}
			if (this.originOrbit != null && this.originOrbit.barycenter != null)
			{
				return this.originOrbit.barycenter;
			}
			if (this.fleet.barycenter() != null)
			{
				return this.fleet.barycenter();
			}
			return this.commonBarycenter;
		}

		// Token: 0x06004756 RID: 18262 RVA: 0x001D1771 File Offset: 0x001CF971
		public virtual bool isInMicrothrust(TIDateTime time = null)
		{
			return false;
		}

		// Token: 0x06004757 RID: 18263 RVA: 0x001D1774 File Offset: 0x001CF974
		public virtual bool CantManeuver(TIDateTime time = null)
		{
			return false;
		}

		// Token: 0x06004758 RID: 18264 RVA: 0x001D1777 File Offset: 0x001CF977
		public virtual bool isInImpulse(TIDateTime time = null)
		{
			return false;
		}

		// Token: 0x06004759 RID: 18265
		[return: TupleElementNames(new string[] { "start", "domain" })]
		public abstract List<ValueTuple<TIDateTime, Trajectory.TrajectoryDomain>> GetTrajectoryDomainsOverTime();

		// Token: 0x0600475A RID: 18266 RVA: 0x001D177C File Offset: 0x001CF97C
		public OrbitalElementsState GetOrbitalElementsAtTime(TIDateTime time)
		{
			TISpaceAssetState.MeanAnomalyPrecision meanAnomalyPrecision = (this.fleet.faction.isActivePlayer ? TISpaceAssetState.MeanAnomalyPrecision.Player : TISpaceAssetState.MeanAnomalyPrecision.AI);
			return this.GetOrbitalElementsAtTime(time, meanAnomalyPrecision);
		}

		// Token: 0x0600475B RID: 18267 RVA: 0x001D17A8 File Offset: 0x001CF9A8
		public virtual OrbitalElementsState GetOrbitalElementsAtTime(TIDateTime time, TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			TIDateTime tidateTime = new TIDateTime();
			tidateTime.SetTime(1900, 1, 1, 0, 0, 0, 0);
			TIDateTime tidateTime2 = new TIDateTime();
			tidateTime2.SetTime(2300, 1, 1, 0, 0, 0, 0);
			if (time < tidateTime || time > tidateTime2)
			{
				Debug.LogWarning("Trajectory.GetOrbitalElementsAtTime: the given time was outside of expected range: " + ((time != null) ? time.ToString() : null));
			}
			tidateTime.ToJulianEpoch();
			tidateTime2.ToJulianEpoch();
			if (time < this.launchTime)
			{
				Debug.LogWarning("Trajectory.GetOrbitalElementsAtTime: requested fleet position prior to launch.  Epoch and mean anomaly will be wrong.");
				return this.originOrbit.ToOrbitalElementsState(this.launchTime, 0.0);
			}
			if (time > this.arrivalTime)
			{
				if (this.nextTrajectory != null)
				{
					return this.nextTrajectory.GetOrbitalElementsAtTime(time, precision);
				}
				TISpaceFleetState tispaceFleetState = this.destination as TISpaceFleetState;
				if (tispaceFleetState != null)
				{
					if (tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime <= time)
					{
						return tispaceFleetState.trajectory.GetOrbitalElementsAtTime(time, precision);
					}
					TIDateTime epoch_DateTime = tispaceFleetState.epoch_DateTime;
					if (epoch_DateTime < tidateTime || epoch_DateTime > tidateTime2)
					{
						string text = "Trajectory.GetOrbitalElementsAtTime: targetFleet.epoch_DateTime was outside expected range after intercept: ";
						TIDateTime tidateTime3 = epoch_DateTime;
						Debug.LogWarning(text + ((tidateTime3 != null) ? tidateTime3.ToString() : null));
					}
					return new OrbitalElementsState(tispaceFleetState);
				}
				else
				{
					TIOrbitState tiorbitState = this.destination as TIOrbitState;
					if (tiorbitState != null)
					{
						TIDateTime arrivalTime = this.arrivalTime;
						OrbitalElementsState orbitalElementsState = new OrbitalElementsState(tiorbitState, 0.0, arrivalTime);
						Vector3d vector3d = this.ToGlobalCartesianStateAtTime(arrivalTime).position - tiorbitState.barycenter.ToGlobalCartesianStateAtTime(arrivalTime).position;
						vector3d = (Quaterniond.Inverse(tiorbitState.barycenter.SpatialRotation) * vector3d.xzy).xzy;
						double num = TISpaceAssetState.CalculateMeanAnomalyFromPosition(orbitalElementsState, tiorbitState.barycenter, vector3d, arrivalTime, precision);
						if (arrivalTime < tidateTime || arrivalTime > tidateTime2)
						{
							string text2 = "Trajectory.GetOrbitalElementsAtTime: arrivalTime was outside expected range, and would be an invalid epoch: ";
							TIDateTime tidateTime4 = arrivalTime;
							Debug.LogWarning(text2 + ((tidateTime4 != null) ? tidateTime4.ToString() : null));
						}
						return new OrbitalElementsState(tiorbitState, num, arrivalTime);
					}
					TISpaceObjectState tispaceObjectState = this.destination as TISpaceObjectState;
					if (tispaceObjectState != null)
					{
						TIDateTime epoch_DateTime2 = tispaceObjectState.epoch_DateTime;
						if (epoch_DateTime2 < tidateTime || epoch_DateTime2 > tidateTime2)
						{
							string text3 = "Trajectory.GetOrbitalElementsAtTime: destination as TISpaceObjectState had an epoch outside expected range: ";
							TIDateTime tidateTime5 = epoch_DateTime2;
							Debug.LogWarning(text3 + ((tidateTime5 != null) ? tidateTime5.ToString() : null));
						}
						return new OrbitalElementsState(tispaceObjectState);
					}
					if (this.destination == null)
					{
						if (this.arrivalTime < tidateTime || this.arrivalTime > tidateTime2)
						{
							string text4 = "Trajectory.GetOrbitalElementsAtTime: destination was null; arrivalTime was outside expected range, and would be an invalid epoch: ";
							TIDateTime arrivalTime2 = this.arrivalTime;
							Debug.LogWarning(text4 + ((arrivalTime2 != null) ? arrivalTime2.ToString() : null));
						}
						return new OrbitalElementsState(this.GetBarycenterAtTime(new TIDateTime(this.arrivalTime, -1.0)));
					}
					return new OrbitalElementsState(this.destination.barycenter);
				}
			}
			else
			{
				if (this.HasOrbitalElements())
				{
					return (this as Trajectory_WithOrbitalElements).transferOrbit;
				}
				Debug.LogError("Base Trajectory.GetOrbitalElementsAtTime() was not overwritten by trajectory without orbital elements: " + this.ToString());
				if (this.originOrbit != null)
				{
					return new OrbitalElementsState(this.originOrbit, 0.0, time);
				}
				return this.GetOrbitalElementsAtTime(this.launchTime);
			}
		}

		// Token: 0x0600475C RID: 18268 RVA: 0x001D1AE0 File Offset: 0x001CFCE0
		public virtual double getDistFromBarycenterAtTime_m(TIDateTime timeToCheck, out TINaturalSpaceObjectState barycenter)
		{
			if (timeToCheck < this.launchTime)
			{
				barycenter = this.GetBarycenterAtTime(timeToCheck);
				return 0.0;
			}
			if (!(timeToCheck >= this.arrivalTime))
			{
				Debug.LogWarning("Trajectory.getDistFromBarycenterAtTime() was not overwridden by " + ((this != null) ? this.ToString() : null));
				barycenter = this.commonBarycenter;
				return this.GetOrbitalElementsAtTime(timeToCheck).semiMajorAxis_m;
			}
			if (this.targetingFleet)
			{
				if (this.destinationFleet.transferAssigned && this.destinationFleet.trajectory.launchTime < timeToCheck)
				{
					TINaturalSpaceObjectState tinaturalSpaceObjectState;
					double distFromBarycenterAtTime_m = this.destinationFleet.trajectory.getDistFromBarycenterAtTime_m(timeToCheck, out tinaturalSpaceObjectState);
					barycenter = tinaturalSpaceObjectState;
					return distFromBarycenterAtTime_m;
				}
				barycenter = this.destinationFleet.barycenter;
				return this.destinationFleet.semiMajorAxis_m;
			}
			else
			{
				if (this.destination != null)
				{
					barycenter = this.destination.barycenter;
				}
				else
				{
					barycenter = this.GetBarycenterAtTime(new TIDateTime(this.arrivalTime, -1.0));
				}
				ITransferTarget transferTarget = this.destination as ITransferTarget;
				if (transferTarget != null)
				{
					return transferTarget.a_m();
				}
				return 0.0;
			}
		}

		// Token: 0x0600475D RID: 18269 RVA: 0x001D1C08 File Offset: 0x001CFE08
		public double DVConsumedOnTrajectory_mps(TIDateTime timeToCheck)
		{
			if (timeToCheck <= this.arrivalTime)
			{
				return this.DV_mps - this.RemainingDVatTime_mps(timeToCheck);
			}
			double num = ((this.nextTrajectory != null && timeToCheck > this.arrivalTime) ? this.nextTrajectory.DVConsumedOnTrajectory_mps(timeToCheck) : 0.0);
			return this.DV_mps + num;
		}

		// Token: 0x0600475E RID: 18270 RVA: 0x001D1C68 File Offset: 0x001CFE68
		public Vector3d DestinationPositionAtTime(TIDateTime time, TIFactionState ourFaction)
		{
			ITransferTarget transferTarget = null;
			if (TIGameState.Valid(this.destinationStation))
			{
				transferTarget = this.destinationStation;
			}
			else if (TIGameState.Valid(this.destinationFleet))
			{
				transferTarget = this.destinationFleet;
			}
			else if (this.destinationOrbit != null)
			{
				transferTarget = this.destinationOrbit;
			}
			if (transferTarget != null)
			{
				return Trajectory.GetDestinationCartesianAroundCommonBarycenterAtTime(transferTarget, time, this.commonBarycenter, ourFaction, this.assignedTime, 0.0).ToGlobal(this.commonBarycenter, time).position;
			}
			if (!this.destroyOnArrival)
			{
				Log.Error("Transfer destination wasn't an orbit, fleet, or station.", Array.Empty<object>());
			}
			if (this.destination == null)
			{
				return this.GetBarycenterAtTime(new TIDateTime(this.arrivalTime, -1.0)).GetGlobalPositionAtTime(time);
			}
			if (this.destination.isSpaceObjectState)
			{
				return this.destination.ref_spaceObject.GetGlobalPositionAtTime(this.arrivalTime);
			}
			return this.destinationOrbit.GetGlobalPositionAtTimeAndAnomaly(this.arrivalTime, 0.0);
		}

		// Token: 0x0600475F RID: 18271 RVA: 0x001D1D74 File Offset: 0x001CFF74
		public TrajectoryPhase GetTrajectoryPhase(TIDateTime assignedTime, TIDateTime trajectoryLaunchTime, TIDateTime timeToCheck, bool settingPosition, out double timeSinceStarted_s, out double timeSinceLaunch_s)
		{
			timeSinceStarted_s = (timeToCheck - assignedTime).TotalSeconds;
			timeSinceLaunch_s = (timeToCheck - trajectoryLaunchTime).TotalSeconds;
			if (this.loiterDuration_s > 0.0 && timeSinceStarted_s <= this.loiterDuration_s)
			{
				return TrajectoryPhase.Loiter;
			}
			if (settingPosition && !this.launched)
			{
				return TrajectoryPhase.Loiter;
			}
			if (this.prepositionDuration_s > 0.0 && timeSinceLaunch_s <= this.prepositionDuration_s)
			{
				return TrajectoryPhase.Preposition;
			}
			double num = this.loiterDuration_s + this.prepositionDuration_s + this.boostDuration_s;
			if (this.boostDuration_s > 0.0 && timeSinceStarted_s <= num)
			{
				return TrajectoryPhase.Boost;
			}
			num += this.coastDuration_s;
			if (this.coastDuration_s > 0.0 && timeSinceStarted_s <= num)
			{
				return TrajectoryPhase.Coast;
			}
			num += this.decelDuration_s;
			if (this.decelDuration_s > 0.0 && timeSinceStarted_s <= num)
			{
				return TrajectoryPhase.Deceleration;
			}
			num += this.captureDuration_s;
			if (this.captureDuration_s > 0.0 && timeSinceStarted_s <= num)
			{
				return TrajectoryPhase.Capture;
			}
			return TrajectoryPhase.Arrive;
		}

		// Token: 0x06004760 RID: 18272 RVA: 0x001D1E84 File Offset: 0x001D0084
		public void BuildSingleTrajectory_Common(IMobileAsset fleet, TISpaceGameState destination, TINaturalSpaceObjectState commonBarycenter, TIDateTime launchTime, double transitDuration_s, bool forceImmediateLaunch = false)
		{
			this.fleetAsSpaceFleetState = fleet as TISpaceFleetState;
			this.fleet = fleet;
			if (this.fleet == null)
			{
				Debug.LogError("Attempted to create a trajectory without a fleet (real or virtual).");
			}
			this.originOrbit = fleet.ref_orbit;
			TISpaceFleetState tispaceFleetState = fleet as TISpaceFleetState;
			if (tispaceFleetState != null && tispaceFleetState.transferAssigned && tispaceFleetState.trajectory.launchTime < launchTime)
			{
				this.originOrbit = tispaceFleetState.trajectory.originOrbit;
			}
			this.destination = destination;
			this.destinationFleet = ((destination != null) ? destination.ref_fleet : null);
			TISpaceFleetState destinationFleet = this.destinationFleet;
			TISpaceFleetState tispaceFleetState2 = fleet as TISpaceFleetState;
			if (MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(destinationFleet, (tispaceFleetState2 != null) ? tispaceFleetState2.faction : null))
			{
				this.destinationFleetTrajectory = this.destinationFleet.trajectory;
			}
			this.destinationStation = ((destination != null) ? destination.ref_hab : null);
			this.destinationOrbit = ((destination != null) ? destination.ref_orbit : null);
			this.commonBarycenter = commonBarycenter;
			this.assignedTime = TITimeState.Now();
			if (launchTime < this.assignedTime)
			{
				launchTime = this.assignedTime;
			}
			double num = (double)((this.assignedTime == launchTime && !forceImmediateLaunch) ? 1 : 0);
			this.launchTime = new TIDateTime(launchTime, num);
			this.loiterDuration_s = launchTime.DifferenceInSeconds(this.assignedTime);
			this.arrivalTime = new TIDateTime(launchTime, transitDuration_s);
			this.launchPosition = fleet.GetGlobalPositionAtTime(launchTime);
			this.destinationPosition = this.DestinationPositionAtTime(this.arrivalTime, fleet.faction);
			this.distanceToDestinationHillSphere_m = this.straightLineDistance_m - ((destination != null) ? destination.ref_naturalSpaceObject.hillRadius_m : 0.0);
		}

		// Token: 0x06004761 RID: 18273 RVA: 0x001D202C File Offset: 0x001D022C
		public TimeSpan BuildSingleTrajectory_SetDuration(double duration_s)
		{
			int num = (int)(duration_s % 1.0 * 100.0);
			int num2 = (int)(duration_s / 60.0 % 1.0 * 60.0);
			int num3 = (int)(duration_s / 3600.0 % 1.0 * 60.0);
			int num4 = (int)(duration_s / 86400.0 % 1.0 * 24.0);
			return new TimeSpan((int)(duration_s / 604800.0 * 7.0), num4, num3, num2, num);
		}

		// Token: 0x06004762 RID: 18274 RVA: 0x001D20D4 File Offset: 0x001D02D4
		public virtual Vector3d DesiredOrientationVector_Acceleration()
		{
			return (this.destinationPosition - this.launchPosition).normalized;
		}

		// Token: 0x06004763 RID: 18275 RVA: 0x001D20FC File Offset: 0x001D02FC
		public virtual Vector3d DesiredOrientationVector_Deceleration()
		{
			return (this.launchPosition - this.destinationPosition).normalized;
		}

		// Token: 0x17000E1F RID: 3615
		// (get) Token: 0x06004764 RID: 18276
		public abstract TrajectoryModel GetTrajectoryModel { get; }

		// Token: 0x06004765 RID: 18277
		public abstract void BuildSingleTrajectory(IMobileAsset fleet, TISpaceGameState destination, ITransferTarget originValue, ITransferTarget destinationValue, TINaturalSpaceObjectState commonBarycenter, TrajectorySolver solver, double fleetCruiseAcceleration_mps2);

		// Token: 0x06004766 RID: 18278
		public abstract Vector3d PositionAtTime(TIDateTime timeToCheck, bool setPosition, out bool arrived);

		// Token: 0x06004767 RID: 18279
		public abstract CartesianState ToGlobalCartesianStateAtTime(TIDateTime timeToCheck);

		// Token: 0x06004768 RID: 18280 RVA: 0x001D2124 File Offset: 0x001D0324
		public CartesianState DestinationCartesianStateAtTime(TIDateTime timeToCheck)
		{
			if (this.destination == null)
			{
				return this.ToGlobalCartesianStateAtTime(new TIDateTime(this.arrivalTime, -1.0));
			}
			if (this.destination.isSpaceObjectState)
			{
				return this.destination.ref_spaceObject.ToGlobalCartesianStateAtTime(timeToCheck);
			}
			return this.destinationOrbit.relevantCartesianState(this.destination.barycenter, timeToCheck, this.getDestinationMeanAnomalyAtArrival());
		}

		// Token: 0x06004769 RID: 18281 RVA: 0x001D2198 File Offset: 0x001D0398
		public double getDestinationMeanAnomalyAtArrival()
		{
			TISpaceAssetState.MeanAnomalyPrecision meanAnomalyPrecision = (this.fleet.faction.isActivePlayer ? TISpaceAssetState.MeanAnomalyPrecision.Player : TISpaceAssetState.MeanAnomalyPrecision.AI);
			return this.getDestinationMeanAnomalyAtArrival(meanAnomalyPrecision);
		}

		// Token: 0x0600476A RID: 18282 RVA: 0x001D21C4 File Offset: 0x001D03C4
		private double getDestinationMeanAnomalyAtArrival(TISpaceAssetState.MeanAnomalyPrecision precision)
		{
			if (this.destination == null)
			{
				return 0.0;
			}
			if (this.destination.isSpaceObjectState)
			{
				return this.destination.ref_spaceObject.meanAnomaly_Rad(this.arrivalTime);
			}
			bool flag;
			Vector3d vector3d = this.PositionAtTime(this.arrivalTime, false, out flag) - this.destinationOrbit.barycenter.GetGlobalPositionAtTime(this.arrivalTime);
			vector3d = (Quaterniond.Inverse(this.destinationOrbit.barycenter.SpatialRotation) * vector3d.xzy).xzy;
			return TISpaceAssetState.CalculateMeanAnomalyFromPosition(this.destinationOrbit, vector3d, this.arrivalTime, precision);
		}

		// Token: 0x0600476B RID: 18283 RVA: 0x001D2278 File Offset: 0x001D0478
		public virtual TIDateTime getOrbitEndTime()
		{
			TIDateTime tidateTime = TITimeState.Now();
			if (tidateTime < this.launchTime)
			{
				return this.launchTime;
			}
			if (tidateTime > this.arrivalTime)
			{
				return null;
			}
			return tidateTime;
		}

		// Token: 0x0600476C RID: 18284 RVA: 0x001D22B4 File Offset: 0x001D04B4
		public bool NeedsPincerToCatchTargetFleet()
		{
			if (this.destinationFleet == null)
			{
				return false;
			}
			TISpaceFleetState tispaceFleetState = this.fleet as TISpaceFleetState;
			if (tispaceFleetState == null)
			{
				return false;
			}
			TIDateTime tidateTime = TITimeState.Now();
			float ourDVspent_mps = (float)this.DV_mps;
			float theirDVspent_mps = 0f;
			float num = tispaceFleetState.currentDeltaV_mps - ourDVspent_mps;
			float num2 = this.destinationFleet.currentDeltaV_mps;
			float num3 = num2;
			if (this.destinationFleet.DoIKnowThisFleetIsTransfering(tispaceFleetState.faction))
			{
				theirDVspent_mps = (float)this.destinationFleet.trajectory.DVConsumedOnTrajectory_mps(this.arrivalTime);
				num3 -= (float)this.destinationFleet.trajectory.RemainingDVatTime_mps(tidateTime);
				if (theirDVspent_mps > num2)
				{
					Debug.LogWarning("Trajectory.NeedsPincerToCatchTargetFleet(): target fleet's DV goes negative.");
					return false;
				}
			}
			num2 -= theirDVspent_mps;
			float num4 = tispaceFleetState.ships.Aggregate(float.PositiveInfinity, (float lowestAccel_mps2, TISpaceShipState ship) => Mathf.Min(ship.CombatAccelerationGivenRemainingDV_mps2(ship.currentDeltaV_kps * 1000f - ourDVspent_mps), lowestAccel_mps2));
			float num5;
			if (theirDVspent_mps == 0f)
			{
				num5 = this.destinationFleet.maxAcceleration_mps2;
			}
			else
			{
				num5 = this.destinationFleet.ships.Aggregate(float.PositiveInfinity, (float lowestAccel_mps2, TISpaceShipState ship) => Mathf.Min(ship.CombatAccelerationGivenRemainingDV_mps2(ship.currentDeltaV_kps * 1000f - theirDVspent_mps), lowestAccel_mps2));
			}
			float pursuitDistance_m = TISpaceCombatState.GetPursuitDistance_m(tispaceFleetState, this.destinationFleet);
			return TISpaceCombatState.OnTieBidDoesTheFirstFleetWin(num5, num3, num4, num, pursuitDistance_m);
		}

		// Token: 0x0600476D RID: 18285 RVA: 0x001D240C File Offset: 0x001D060C
		public void ReconstructMissingDestinationOrbit()
		{
			ValueTuple<TIOrbitState, TIDateTime, double> valueTuple = this.EstimateMissingDestinationOrbit();
			TIOrbitState item = valueTuple.Item1;
			TIDateTime item2 = valueTuple.Item2;
			double item3 = valueTuple.Item3;
			this.destinationFleet = null;
			this.destinationStation = null;
			this.destinationOrbit = item;
			this.destination = item;
			this.destinationOrbitEpoch = item2;
			this.destinationOrbitMeanAnomalyAtEpoch = new double?(item3);
		}

		// Token: 0x0600476E RID: 18286 RVA: 0x001D2464 File Offset: 0x001D0664
		public void ReconstructMissingOriginOrbit()
		{
			TISpaceFleetState tispaceFleetState = this.fleet as TISpaceFleetState;
			if (tispaceFleetState == null)
			{
				return;
			}
			CartesianState cartesianState = this.ToGlobalCartesianStateAtTime(this.launchTime).ToLocal(this.commonBarycenter, this.launchTime);
			OrbitalElementsState orbitalElementsState = cartesianState.ToOrbitalElementsState(this.commonBarycenter.mu, new DateTime?(this.launchTime.ExportTime()));
			if (orbitalElementsState.eccentricity >= 1.0)
			{
				double num = this.commonBarycenter.localEscapeVelocity_mps(cartesianState.position.magnitude);
				cartesianState.velocity = cartesianState.velocity.normalized * (num * 0.9);
				orbitalElementsState = cartesianState.ToOrbitalElementsState(this.commonBarycenter.mu, new DateTime?(this.launchTime.ExportTime()));
			}
			TIAdHocOrbitState tiadHocOrbitState = TIAdHocOrbitState.CreateAdHocOrbitState(this.commonBarycenter, orbitalElementsState, tispaceFleetState);
			this.originOrbit = tiadHocOrbitState;
		}

		// Token: 0x0600476F RID: 18287 RVA: 0x001D2554 File Offset: 0x001D0754
		[return: TupleElementNames(new string[] { "orbit", "epoch", "meanAnomalyAtEpoch_Rad" })]
		public ValueTuple<TIOrbitState, TIDateTime, double> EstimateMissingDestinationOrbit()
		{
			CartesianState cartesianState = this.ToGlobalCartesianStateAtTime(this.arrivalTime).ToLocal(this.commonBarycenter, this.arrivalTime);
			return this.EstimateMissingDestinationOrbit(this.commonBarycenter, cartesianState);
		}

		// Token: 0x06004770 RID: 18288 RVA: 0x001D2590 File Offset: 0x001D0790
		[return: TupleElementNames(new string[] { "orbit", "epoch", "meanAnomalyAtEpoch_Rad" })]
		public ValueTuple<TIOrbitState, TIDateTime, double> EstimateMissingDestinationOrbit(TINaturalSpaceObjectState barycenter, CartesianState localCartesianAtArrival)
		{
			ValueTuple<TIOrbitState, double, double> valueTuple = new ValueTuple<TIOrbitState, double, double>(null, double.PositiveInfinity, 0.0);
			foreach (TIOrbitState tiorbitState in barycenter.orbits)
			{
				if (tiorbitState.eccentricity < 1.0 && tiorbitState.semiMajorAxis_m > 0.0)
				{
					double num = TISpaceAssetState.CalculateMeanAnomalyFromPosition(tiorbitState, localCartesianAtArrival.position, this.arrivalTime, true);
					Vector3d position = tiorbitState.relevantCartesianState(this.commonBarycenter, this.arrivalTime, num).ToLocal(barycenter, this.arrivalTime).position;
					double magnitude = (localCartesianAtArrival.position - position).magnitude;
					if (magnitude < valueTuple.Item2)
					{
						valueTuple = new ValueTuple<TIOrbitState, double, double>(tiorbitState, magnitude, num);
					}
				}
			}
			ValueTuple<TINaturalSpaceObjectState, double> valueTuple2 = new ValueTuple<TINaturalSpaceObjectState, double>(null, double.PositiveInfinity);
			TISpaceBodyState tispaceBodyState = barycenter as TISpaceBodyState;
			if (tispaceBodyState != null)
			{
				List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>();
				list.AddRange(tispaceBodyState.naturalSatellites);
				list.AddRange(tispaceBodyState.lagrangePoints);
				foreach (TINaturalSpaceObjectState tinaturalSpaceObjectState in list)
				{
					Vector3d position2 = tinaturalSpaceObjectState.ToLocalCartesianStateAtTime(this.arrivalTime).position;
					double magnitude2 = (localCartesianAtArrival.position - position2).magnitude;
					if (magnitude2 < valueTuple2.Item2)
					{
						valueTuple2 = new ValueTuple<TINaturalSpaceObjectState, double>(tinaturalSpaceObjectState, magnitude2);
					}
				}
			}
			if (valueTuple.Item2 < valueTuple2.Item2)
			{
				return new ValueTuple<TIOrbitState, TIDateTime, double>(valueTuple.Item1, this.arrivalTime, valueTuple.Item3);
			}
			CartesianState cartesianState = localCartesianAtArrival.ChangeReferenceFrame(barycenter, valueTuple2.Item1, this.arrivalTime);
			return this.EstimateMissingDestinationOrbit(valueTuple2.Item1, cartesianState);
		}

		// Token: 0x06004771 RID: 18289 RVA: 0x001D2790 File Offset: 0x001D0990
		public void SetResupplyPlan(bool resupply)
		{
			TISpaceGameState destination = this.destination;
			if (destination != null && destination.isHabState && this.destinationStation.ref_factions.Contains(this.fleet.faction) && this.destinationStation.AllowsResupply(this.fleet.faction, false, false))
			{
				this.resupplyOnArrival = resupply;
				return;
			}
			this.resupplyOnArrival = false;
		}

		// Token: 0x06004772 RID: 18290 RVA: 0x001D27F7 File Offset: 0x001D09F7
		public void SetAsIntercept(bool intercept)
		{
			this.interceptTrajectory = intercept;
		}

		// Token: 0x06004773 RID: 18291 RVA: 0x001D2800 File Offset: 0x001D0A00
		public static void GetDestinationOrbitalElementsAroundLocalBarycenterAtTime(out OrbitalElementsState orbitalElements, out TINaturalSpaceObjectState localBarycenter, ITransferTarget destination, TIDateTime time, TIFactionState ourFaction, TIDateTime now = null, double meanAnomalyOfDestination = 0.0)
		{
			if (now == null)
			{
				now = TITimeState.Now();
			}
			TISpaceFleetState tispaceFleetState = destination as TISpaceFleetState;
			if (tispaceFleetState != null && !MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, ourFaction))
			{
				bool flag;
				tispaceFleetState.getOrbitalElementsState(now, out orbitalElements, out localBarycenter, out flag);
				return;
			}
			bool flag2;
			destination.getOrbitalElementsState(now, out orbitalElements, out localBarycenter, out flag2);
			if (!flag2)
			{
				orbitalElements.meanAnomalyAtEpoch_Rad = meanAnomalyOfDestination;
				orbitalElements.epoch = time.ExportTime();
			}
		}

		// Token: 0x06004774 RID: 18292 RVA: 0x001D2864 File Offset: 0x001D0A64
		public static void GetDestinationCartesianAroundLocalBarycenterAtTime(out CartesianState localCartesianState, out TINaturalSpaceObjectState localBarycenter, ITransferTarget destination, TIDateTime time, TIFactionState ourFaction, TIDateTime now = null, double meanAnomalyOfDestination = 0.0)
		{
			if (now == null)
			{
				now = TITimeState.Now();
			}
			TISpaceFleetState tispaceFleetState = destination as TISpaceFleetState;
			if ((tispaceFleetState == null || MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, ourFaction)) && destination.tryToGetLocalCartesianState(time, out localCartesianState, out localBarycenter))
			{
				return;
			}
			OrbitalElementsState orbitalElementsState;
			Trajectory.GetDestinationOrbitalElementsAroundLocalBarycenterAtTime(out orbitalElementsState, out localBarycenter, destination, time, ourFaction, now, meanAnomalyOfDestination);
			localCartesianState = orbitalElementsState.ToCartesianStateAtTime(time.ExportTime(), localBarycenter.mass_kg);
		}

		// Token: 0x06004775 RID: 18293 RVA: 0x001D28CC File Offset: 0x001D0ACC
		public static CartesianState GetDestinationCartesianAroundCommonBarycenterAtTime(ITransferTarget destination, TIDateTime time, TINaturalSpaceObjectState commonBarycenter, TIFactionState ourFaction, TIDateTime now = null, double meanAnomalyOfDestination = 0.0)
		{
			if (now == null)
			{
				now = TITimeState.Now();
			}
			CartesianState cartesianState;
			TINaturalSpaceObjectState tinaturalSpaceObjectState;
			Trajectory.GetDestinationCartesianAroundLocalBarycenterAtTime(out cartesianState, out tinaturalSpaceObjectState, destination, time, ourFaction, now, meanAnomalyOfDestination);
			return cartesianState.ChangeReferenceFrame(tinaturalSpaceObjectState, commonBarycenter, time);
		}

		// Token: 0x06004776 RID: 18294 RVA: 0x001D2904 File Offset: 0x001D0B04
		[return: TupleElementNames(new string[] { "orbit", "barycenter", "meanAnomalyIsGood" })]
		public static ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool> GetDestinationLocalOrbitalElementsAtTime(ITransferTarget destination, TIFactionState factionThatIsAsking, TIDateTime time, TIDateTime now = null, double meanAnomalyOfDestination = 0.0)
		{
			if (now == null)
			{
				now = TITimeState.Now();
			}
			TISpaceFleetState tispaceFleetState = destination as TISpaceFleetState;
			if (tispaceFleetState != null && !MasterTransferPlanner.DoWeKnowThatFleetIsTransfering(tispaceFleetState, factionThatIsAsking))
			{
				OrbitalElementsState orbitalElementsState;
				TINaturalSpaceObjectState tinaturalSpaceObjectState;
				bool flag;
				tispaceFleetState.getOrbitalElementsState(now, out orbitalElementsState, out tinaturalSpaceObjectState, out flag);
				return new ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool>(orbitalElementsState, tinaturalSpaceObjectState, flag);
			}
			OrbitalElementsState orbitalElementsState2;
			TINaturalSpaceObjectState tinaturalSpaceObjectState2;
			bool flag2;
			destination.getOrbitalElementsState(time, out orbitalElementsState2, out tinaturalSpaceObjectState2, out flag2);
			if (!flag2)
			{
				orbitalElementsState2.meanAnomalyAtEpoch_Rad = meanAnomalyOfDestination;
				orbitalElementsState2.epoch = time.ExportTime();
			}
			return new ValueTuple<OrbitalElementsState, TINaturalSpaceObjectState, bool>(orbitalElementsState2, tinaturalSpaceObjectState2, flag2);
		}

		// Token: 0x04002940 RID: 10560
		public TISpaceFleetState fleetAsSpaceFleetState;

		// Token: 0x04002941 RID: 10561
		[fsIgnore]
		private IMobileAsset _fleet;

		// Token: 0x04002950 RID: 10576
		public bool launched;

		// Token: 0x04002953 RID: 10579
		public bool involuntary;

		// Token: 0x04002954 RID: 10580
		public TISpaceBodyState collisionTarget;

		// Token: 0x04002955 RID: 10581
		public bool exitsSolarSystem;

		// Token: 0x04002956 RID: 10582
		public const double SOLAR_SYSTEM_EXIT_m = 12000000000000.0;

		// Token: 0x04002957 RID: 10583
		public const double MAX_ABORT_SOLAR_ALTITUDE_m = 9000000000000.0;

		// Token: 0x04002958 RID: 10584
		public const double MIN_SOLAR_PERIAPSIS_m = 1400000000.0;

		// Token: 0x04002959 RID: 10585
		public TISpaceObjectState originalDestinationSunOrbiter;

		// Token: 0x0400295A RID: 10586
		public double? destinationOrbitMeanAnomalyAtEpoch;

		// Token: 0x0400295B RID: 10587
		public TIDateTime destinationOrbitEpoch;

		// Token: 0x0400295C RID: 10588
		public Trajectory nextTrajectory;

		// Token: 0x0400295D RID: 10589
		[SerializeField]
		protected double loiterDuration_s;

		// Token: 0x0400295E RID: 10590
		[SerializeField]
		protected double prepositionDuration_s;

		// Token: 0x0400295F RID: 10591
		[SerializeField]
		protected double boostDuration_s;

		// Token: 0x04002960 RID: 10592
		[SerializeField]
		protected double coastDuration_s;

		// Token: 0x04002961 RID: 10593
		[SerializeField]
		protected double decelDuration_s;

		// Token: 0x04002962 RID: 10594
		[SerializeField]
		protected double captureDuration_s;

		// Token: 0x04002963 RID: 10595
		private double _straightLineDistance_m = -1.0;

		// Token: 0x04002964 RID: 10596
		public double distanceToDestinationHillSphere_m;

		// Token: 0x0400296A RID: 10602
		public double DV_targetFleet_mps;

		// Token: 0x0400296B RID: 10603
		protected const double MAX_PLAUSIBLE_SPEED_mps = 1440000.0;

		// Token: 0x0400296C RID: 10604
		protected const double MAX_PLAUSIBLE_ACCELERATION_MULTIPLIER = 2.0;

		// Token: 0x0400296D RID: 10605
		protected const double MAX_PLAUSIBLE_VERTICAL_MICROTHRUST_RATIO = 1.0;

		// Token: 0x0400296E RID: 10606
		protected const bool CANT_MANEUVER_IF_IN_MICROTHRUST = true;

		// Token: 0x0400296F RID: 10607
		protected const bool CANT_MANEUVER_IF_IN_IMPULSE = false;

		// Token: 0x04002970 RID: 10608
		protected const bool CANT_MANEUVER_IF_IN_FINAL_BURN = true;

		// Token: 0x04002971 RID: 10609
		protected const bool CANT_MANEUVER_IF_IN_ANY_BURN = false;

		// Token: 0x04002972 RID: 10610
		protected const bool CANT_MANEUVER_IF_WITHIN_BURN_DURATION_OF_FINAL_BURN = false;

		// Token: 0x04002973 RID: 10611
		protected const bool CANT_MANEUVER_WHILE_ORBIT_PHASING = true;

		// Token: 0x02000F74 RID: 3956
		public enum TrajectoryDomain
		{
			// Token: 0x04005E56 RID: 24150
			Microthrust,
			// Token: 0x04005E57 RID: 24151
			Impulse,
			// Token: 0x04005E58 RID: 24152
			Torch,
			// Token: 0x04005E59 RID: 24153
			Orbit,
			// Token: 0x04005E5A RID: 24154
			OrbitPhasing
		}
	}
}
