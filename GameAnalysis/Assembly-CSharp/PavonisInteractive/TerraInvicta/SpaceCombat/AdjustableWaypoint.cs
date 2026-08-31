using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E5 RID: 2533
	public class AdjustableWaypoint : BasicWaypoint, IPreviousWaypoint, IWaypoint, INextWaypoint, IPathDetail, IMovableWaypoint
	{
		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06005FBB RID: 24507 RVA: 0x002D4BB4 File Offset: 0x002D2DB4
		// (remove) Token: 0x06005FBC RID: 24508 RVA: 0x002D4BEC File Offset: 0x002D2DEC
		public event Action OnPositionRotationChange;

		// Token: 0x17001080 RID: 4224
		// (get) Token: 0x06005FBD RID: 24509 RVA: 0x002D4C21 File Offset: 0x002D2E21
		private float _intendedAcceleration
		{
			get
			{
				return this._activeTrajectorySequence.intendedLinearAcceleration;
			}
		}

		// Token: 0x17001081 RID: 4225
		// (get) Token: 0x06005FBE RID: 24510 RVA: 0x002D4C2E File Offset: 0x002D2E2E
		public bool IsCoastOnly
		{
			get
			{
				WaypointTrajectorySequence activeTrajectorySequence = this._activeTrajectorySequence;
				return activeTrajectorySequence == null || activeTrajectorySequence.IsCoasting;
			}
		}

		// Token: 0x17001082 RID: 4226
		// (get) Token: 0x06005FBF RID: 24511 RVA: 0x002D4C44 File Offset: 0x002D2E44
		public ITrajectory ValidTrajectorySequence
		{
			get
			{
				if (!this._activeTrajectorySequence.IsTrajectoryValid)
				{
					return this._nextWaypoint.ValidTrajectorySequence;
				}
				return this._activeTrajectorySequence;
			}
		}

		// Token: 0x17001083 RID: 4227
		// (get) Token: 0x06005FC0 RID: 24512 RVA: 0x002D4C72 File Offset: 0x002D2E72
		public IPreviousWaypoint PreviousWaypoint
		{
			get
			{
				return this._previousWaypoint;
			}
		}

		// Token: 0x17001084 RID: 4228
		// (get) Token: 0x06005FC1 RID: 24513 RVA: 0x002D4C7A File Offset: 0x002D2E7A
		public int UID
		{
			get
			{
				return this._uid;
			}
		}

		// Token: 0x17001085 RID: 4229
		// (get) Token: 0x06005FC2 RID: 24514 RVA: 0x002D4C82 File Offset: 0x002D2E82
		// (set) Token: 0x06005FC3 RID: 24515 RVA: 0x002D4C8A File Offset: 0x002D2E8A
		public bool IsInputLocked { get; set; }

		// Token: 0x17001086 RID: 4230
		// (get) Token: 0x06005FC4 RID: 24516 RVA: 0x002D4C93 File Offset: 0x002D2E93
		public bool IsRecursivelyLocked
		{
			get
			{
				return this.IsInputLocked || this._nextWaypoint.IsRecursivelyLocked;
			}
		}

		// Token: 0x17001087 RID: 4231
		// (get) Token: 0x06005FC5 RID: 24517 RVA: 0x002D4CAA File Offset: 0x002D2EAA
		// (set) Token: 0x06005FC6 RID: 24518 RVA: 0x002D4CB2 File Offset: 0x002D2EB2
		public bool IsCoreWaypoint { get; private set; }

		// Token: 0x17001088 RID: 4232
		// (get) Token: 0x06005FC7 RID: 24519 RVA: 0x002D4CBB File Offset: 0x002D2EBB
		// (set) Token: 0x06005FC8 RID: 24520 RVA: 0x002D4CC3 File Offset: 0x002D2EC3
		public bool RenderTrajectoryLines { get; set; } = true;

		// Token: 0x17001089 RID: 4233
		// (get) Token: 0x06005FC9 RID: 24521 RVA: 0x002D4CCC File Offset: 0x002D2ECC
		// (set) Token: 0x06005FCA RID: 24522 RVA: 0x002D4CD4 File Offset: 0x002D2ED4
		public bool PadlockEnabled { get; set; }

		// Token: 0x1700108A RID: 4234
		// (get) Token: 0x06005FCB RID: 24523 RVA: 0x002D4CDD File Offset: 0x002D2EDD
		// (set) Token: 0x06005FCC RID: 24524 RVA: 0x002D4CE5 File Offset: 0x002D2EE5
		public bool AllStopEnabled { get; set; }

		// Token: 0x1700108B RID: 4235
		// (get) Token: 0x06005FCD RID: 24525 RVA: 0x002D4CEE File Offset: 0x002D2EEE
		// (set) Token: 0x06005FCE RID: 24526 RVA: 0x002D4CF6 File Offset: 0x002D2EF6
		public bool MatchVelocityEnabled { get; set; }

		// Token: 0x1700108C RID: 4236
		// (get) Token: 0x06005FCF RID: 24527 RVA: 0x002D4CFF File Offset: 0x002D2EFF
		// (set) Token: 0x06005FD0 RID: 24528 RVA: 0x002D4D07 File Offset: 0x002D2F07
		public bool DefensiveManueversEnabled { get; set; }

		// Token: 0x1700108D RID: 4237
		// (get) Token: 0x06005FD1 RID: 24529 RVA: 0x002D4D10 File Offset: 0x002D2F10
		// (set) Token: 0x06005FD2 RID: 24530 RVA: 0x002D4D18 File Offset: 0x002D2F18
		public bool CollisionWarningNeeded { get; set; }

		// Token: 0x06005FD3 RID: 24531 RVA: 0x002D4D21 File Offset: 0x002D2F21
		public bool IsInBurn(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsInBurn(time);
		}

		// Token: 0x06005FD4 RID: 24532 RVA: 0x002D4D2F File Offset: 0x002D2F2F
		public bool IsAcceleratingRight(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsAcceleratingRight(time);
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x002D4D3D File Offset: 0x002D2F3D
		public bool IsAcceleratingLeft(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsAcceleratingLeft(time);
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x002D4D4B File Offset: 0x002D2F4B
		public bool IsAcceleratingUp(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsAcceleratingUp(time);
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x002D4D59 File Offset: 0x002D2F59
		public bool IsAcceleratingDown(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsAcceleratingDown(time);
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x002D4D67 File Offset: 0x002D2F67
		public bool IsAcceleratingRollRight(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsAcceleratingRollRight(time);
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x002D4D75 File Offset: 0x002D2F75
		public bool IsAcceleratingRollLeft(TIDateTime time)
		{
			return this.ValidTrajectorySequence.IsAcceleratingRollLeft(time);
		}

		// Token: 0x06005FDA RID: 24538 RVA: 0x002D4D83 File Offset: 0x002D2F83
		public Vector3 PositionAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.ValidTrajectorySequence.PositionAt(time);
			}
			return this._nextWaypoint.PositionAt(time);
		}

		// Token: 0x06005FDB RID: 24539 RVA: 0x002D4DAC File Offset: 0x002D2FAC
		public Vector3 VelocityAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.ValidTrajectorySequence.VelocityAt(time);
			}
			return this._nextWaypoint.VelocityAt(time);
		}

		// Token: 0x06005FDC RID: 24540 RVA: 0x002D4DD5 File Offset: 0x002D2FD5
		public Vector3 AccelerationAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.ValidTrajectorySequence.AccelerationAt(time);
			}
			return this._nextWaypoint.AccelerationAt(time);
		}

		// Token: 0x06005FDD RID: 24541 RVA: 0x002D4DFE File Offset: 0x002D2FFE
		public Vector3 HeadingAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.ValidTrajectorySequence.HeadingAt(time);
			}
			return this._nextWaypoint.HeadingAt(time);
		}

		// Token: 0x06005FDE RID: 24542 RVA: 0x002D4E27 File Offset: 0x002D3027
		public Quaternion RotationAt(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.ValidTrajectorySequence.RotationAt(time);
			}
			return this._nextWaypoint.RotationAt(time);
		}

		// Token: 0x06005FDF RID: 24543 RVA: 0x002D4E50 File Offset: 0x002D3050
		public float AngularVelocityAt_Rad(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.ValidTrajectorySequence.AngularVelocityAt_Rad(time);
			}
			return this._nextWaypoint.AngularVelocityAt_Rad(time);
		}

		// Token: 0x06005FE0 RID: 24544 RVA: 0x002D4E79 File Offset: 0x002D3079
		public HoldTrajectory TrajectoryAt(TIDateTime time)
		{
			return this.ValidTrajectorySequence.TrajectoryAt(time) as HoldTrajectory;
		}

		// Token: 0x06005FE1 RID: 24545 RVA: 0x002D4E8C File Offset: 0x002D308C
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			return this.ValidTrajectorySequence.GetBurnTimings();
		}

		// Token: 0x06005FE2 RID: 24546 RVA: 0x002D4E99 File Offset: 0x002D3099
		public void UpdatePathRender(TIDateTime timingStart, Camera cam, Vector3 shipPosition)
		{
			this._nextWaypoint.UpdatePathRender(timingStart, cam, shipPosition);
			if (this.RenderTrajectoryLines)
			{
				this._activeTrajectorySequence.UpdatePathNodes(timingStart, cam, shipPosition);
			}
		}

		// Token: 0x06005FE3 RID: 24547 RVA: 0x002D4EBF File Offset: 0x002D30BF
		public void UpdateAccelerationConstraints(AccelerationConstraints constraints)
		{
			this._constraints = constraints;
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x002D4EC8 File Offset: 0x002D30C8
		public float LinearAcceleration()
		{
			return Mathf.Min(this._constraints.LinearAcceleration, this._intendedAcceleration);
		}

		// Token: 0x06005FE5 RID: 24549 RVA: 0x002D4EE0 File Offset: 0x002D30E0
		public AdjustableWaypoint(AccelerationConstraints constraints)
		{
			this._uid = AdjustableWaypoint._UIDGenerator++;
			this.IsCoreWaypoint = false;
			this._constraints = constraints;
			this._changeProposal = new ProposedWaypoint();
			this._proposedTrajectorySequence = WaypointTrajectorySequence.InvalidTrajectorySequence;
			this._activeTrajectorySequence = WaypointTrajectorySequence.InvalidTrajectorySequence;
		}

		// Token: 0x06005FE6 RID: 24550 RVA: 0x002D4F3C File Offset: 0x002D313C
		public AdjustableWaypoint(IPreviousWaypoint previousWaypoint, AccelerationConstraints constraints, float alphaValue)
		{
			this._uid = AdjustableWaypoint._UIDGenerator++;
			this.IsCoreWaypoint = true;
			this._constraints = constraints;
			this._changeProposal = new ProposedWaypoint();
			this.SetData(previousWaypoint);
			base.AlphaBlendValue = alphaValue;
			this.EstablishDesiredPreviousPoint(previousWaypoint);
		}

		// Token: 0x06005FE7 RID: 24551 RVA: 0x002D4F97 File Offset: 0x002D3197
		public void SetPreviousWaypoint(IPreviousWaypoint previousWaypoint)
		{
			this._previousWaypoint = previousWaypoint;
		}

		// Token: 0x06005FE8 RID: 24552 RVA: 0x002D4FA0 File Offset: 0x002D31A0
		public void EstablishDesiredPreviousPoint(IPreviousWaypoint previousWaypoint)
		{
			IPreviousWaypoint previousWaypoint2 = this._previousWaypoint;
			if (previousWaypoint2 != null)
			{
				previousWaypoint2.SetNextWaypoint(null);
			}
			this._previousWaypoint = previousWaypoint;
			this.SetNextWaypoint(null);
			this._proposedTrajectorySequence = WaypointTrajectorySequence.InvalidTrajectorySequence;
			this._activeTrajectorySequence = WaypointTrajectorySequence.InvalidTrajectorySequence;
		}

		// Token: 0x06005FE9 RID: 24553 RVA: 0x002D4FD8 File Offset: 0x002D31D8
		public void ResetCurrentWaypointSequence()
		{
			this._previousWaypoint.ResetNextWaypointSequence();
		}

		// Token: 0x06005FEA RID: 24554 RVA: 0x002D4FE5 File Offset: 0x002D31E5
		public void ResetNextWaypointSequence()
		{
			this._nextWaypoint.ResetLocksRecursive();
			this._proposedTrajectorySequence = this._activeTrajectorySequence;
			this._changeProposal.SetData(this);
			this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FEB RID: 24555 RVA: 0x002D5011 File Offset: 0x002D3211
		public void ResetLocksRecursive()
		{
			this.IsInputLocked = false;
			this._nextWaypoint.ResetLocksRecursive();
		}

		// Token: 0x06005FEC RID: 24556 RVA: 0x002D5028 File Offset: 0x002D3228
		public void RecalculateTrajectoryPathRecursive()
		{
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint();
			proposedWaypoint.SetData(this);
			WaypointTrajectorySequence waypointTrajectorySequence = WaypointTrajectorySequence.CreateConstrainedTrajectory(this._previousWaypoint, proposedWaypoint, this._constraints, false, false, 0f, -1f);
			if (waypointTrajectorySequence.IsTrajectoryValid)
			{
				this._changeProposal = proposedWaypoint;
				this._proposedTrajectorySequence = waypointTrajectorySequence;
				this.AdoptProposedChanges();
			}
			else
			{
				this.ResetNextWaypointSequence();
			}
			this._nextWaypoint.RecalculateTrajectoryPathRecursive();
		}

		// Token: 0x06005FED RID: 24557 RVA: 0x002D5090 File Offset: 0x002D3290
		public void ResumePreviousTargetPosition(Vector3 targetDisplacement)
		{
			this.ProposePlacement(targetDisplacement, null, false, -1f);
		}

		// Token: 0x06005FEE RID: 24558 RVA: 0x002D50A4 File Offset: 0x002D32A4
		public void CacheWaypointOrientation()
		{
			this._previousOrientation = new BasicWaypoint.WaypointOrientation
			{
				Position = base.Position,
				Velocity = base.Velocity,
				Rotation = base.Rotation,
				Timing = base.Timing
			};
		}

		// Token: 0x06005FEF RID: 24559 RVA: 0x002D50F4 File Offset: 0x002D32F4
		public void CacheWaypointOrientationRecursively()
		{
			this.CacheWaypointOrientation();
			this._nextWaypoint.CacheWaypointOrientationRecursively();
		}

		// Token: 0x06005FF0 RID: 24560 RVA: 0x002D5108 File Offset: 0x002D3308
		public bool ProposeTrajectory(WaypointTrajectorySequence sequence)
		{
			this._changeProposal.Position = sequence.End.Position;
			this._changeProposal.Velocity = sequence.End.Velocity;
			this._changeProposal.Timing = base.Timing;
			this._changeProposal.Rotation = sequence.End.Rotation;
			this._changeProposal.IsPositionLocked = false;
			this._changeProposal.RotationAllowed = true;
			this._proposedTrajectorySequence = sequence;
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FF1 RID: 24561 RVA: 0x002D5190 File Offset: 0x002D3390
		public void AdjustPlacement(ProposedWaypoint start, ProposedWaypoint end, Vector3 targetDisplacement)
		{
			WaypointTrajectorySequence waypointTrajectorySequence = WaypointTrajectorySequence.CreateConstrainedTrajectory(start, end, this._constraints, false, false, 0f, -1f);
			this._changeProposal.Position = waypointTrajectorySequence.PositionAt(base.Timing);
			this._changeProposal.Velocity = waypointTrajectorySequence.VelocityAt(base.Timing);
			this._changeProposal.Timing = base.Timing;
			this._changeProposal.Rotation = waypointTrajectorySequence.RotationAt(base.Timing);
			this._changeProposal.IsPositionLocked = false;
			this._changeProposal.RotationAllowed = false;
			this.GenerateConstrainedTrajectoryProposal(start, null, false, -1f);
			this.AdoptProposedChanges();
			this._nextWaypoint.AllignToTrajectoryPathRecursively(waypointTrajectorySequence, end.Timing, targetDisplacement);
		}

		// Token: 0x06005FF2 RID: 24562 RVA: 0x002D524C File Offset: 0x002D344C
		public void AllignToTrajectoryPathRecursively(WaypointTrajectorySequence sequence, TIDateTime endTime, Vector3 targetDisplacement)
		{
			if (base.Timing < endTime)
			{
				this._changeProposal.Position = sequence.PositionAt(base.Timing);
				this._changeProposal.Velocity = sequence.VelocityAt(base.Timing);
				this._changeProposal.Timing = base.Timing;
				this._changeProposal.Rotation = sequence.RotationAt(base.Timing);
				this._changeProposal.IsPositionLocked = false;
				this._changeProposal.RotationAllowed = false;
				this.GenerateConstrainedTrajectoryProposal(this._previousWaypoint, null, false, -1f);
				this.AdoptProposedChanges();
				this._nextWaypoint.AllignToTrajectoryPathRecursively(sequence, endTime, targetDisplacement);
				return;
			}
			this.HoldRecursively();
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x002D5308 File Offset: 0x002D3508
		public void HoldRecursively()
		{
			this.GenerateHoldTrajectoryProposal(this._previousWaypoint);
			if (this._proposedTrajectorySequence.IsTrajectoryValid)
			{
				this._changeProposal.SetData(this._proposedTrajectorySequence.End);
				this._changeProposal.IsPositionLocked = false;
			}
			this.AdoptProposedChanges();
			this._nextWaypoint.HoldRecursively();
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x002D5364 File Offset: 0x002D3564
		public bool ProposeWaypoint(ProposedWaypoint proposedWaypoint, AccelerationConstraints overrideConstraints = null)
		{
			if (!this._activeTrajectorySequence.IsTrajectoryValid)
			{
				return false;
			}
			AccelerationConstraints accelerationConstraints = overrideConstraints ?? this._constraints ?? null;
			this._changeProposal.SetData(proposedWaypoint);
			this.GenerateConstrainedTrajectoryProposal(this._previousWaypoint, accelerationConstraints, false, -1f);
			this._changeProposal.Rotation = proposedWaypoint.Rotation;
			this._proposedTrajectorySequence.AdjustEndHeading(this._changeProposal, accelerationConstraints, false);
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FF5 RID: 24565 RVA: 0x002D53DC File Offset: 0x002D35DC
		public bool ProposePlacement(Vector3 position, AccelerationConstraints overrideConstraints = null, bool preserveRoll = false, float forceAcceleration = -1f)
		{
			this._isProposalSource = true;
			TIDateTime tidateTime = new TIDateTime(base.Timing);
			return this.ProposePlacement(position, tidateTime, overrideConstraints, preserveRoll, forceAcceleration);
		}

		// Token: 0x06005FF6 RID: 24566 RVA: 0x002D5408 File Offset: 0x002D3608
		public bool ProposePlacement(Vector3 position, TIDateTime timing, AccelerationConstraints overrideConstraints = null, bool preserveRoll = false, float forceAcceleration = -1f)
		{
			this._changeProposal.SetData(this);
			this._changeProposal.Position = position;
			this._changeProposal.Timing = timing;
			this._changeProposal.IsPositionLocked = false;
			this._changeProposal.RotationAllowed = true;
			this.GenerateConstrainedTrajectoryProposal(this._previousWaypoint, overrideConstraints, preserveRoll, forceAcceleration);
			if (this._proposedTrajectorySequence.IsTrajectoryValid)
			{
				this._changeProposal.Velocity = this._proposedTrajectorySequence.End.Velocity;
			}
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FF7 RID: 24567 RVA: 0x002D5490 File Offset: 0x002D3690
		public bool ProposeHeading(Vector3 heading)
		{
			this._changeProposal.SetData(this);
			this._changeProposal.Heading = heading;
			this._changeProposal.RotationAllowed = true;
			this.GenerateConstrainedTrajectoryProposal(this._previousWaypoint, null, false, -1f);
			if (this._proposedTrajectorySequence.IsTrajectoryValid)
			{
				this._changeProposal.Velocity = this._proposedTrajectorySequence.End.Velocity;
			}
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FF8 RID: 24568 RVA: 0x002D5504 File Offset: 0x002D3704
		public bool ProposeRotation(Quaternion rotation, AccelerationConstraints overrideConstraints = null)
		{
			this._changeProposal.SetData(this);
			this._changeProposal.Rotation = rotation;
			this._changeProposal.RotationAllowed = true;
			this.GenerateConstrainedTrajectoryProposal(this._previousWaypoint, overrideConstraints, false, -1f);
			if (this._proposedTrajectorySequence.IsTrajectoryValid)
			{
				this._changeProposal.Velocity = this._proposedTrajectorySequence.End.Velocity;
			}
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FF9 RID: 24569 RVA: 0x002D5578 File Offset: 0x002D3778
		public bool AdjustRotation(Quaternion rotation, AccelerationConstraints overrideConstraints = null)
		{
			if (!this._activeTrajectorySequence.IsTrajectoryValid)
			{
				return false;
			}
			this._changeProposal.SetData(this);
			this._proposedTrajectorySequence = this._activeTrajectorySequence;
			this._changeProposal.Rotation = rotation;
			if (overrideConstraints == null)
			{
				this._proposedTrajectorySequence.AdjustEndHeading(this._changeProposal, this._constraints, false);
			}
			else
			{
				this._proposedTrajectorySequence.AdjustEndHeading(this._changeProposal, overrideConstraints, false);
			}
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FFA RID: 24570 RVA: 0x002D55F0 File Offset: 0x002D37F0
		public bool IsRecursiveStartChangeViable(IWaypoint startProposal)
		{
			if (this.IsRecursivelyLocked)
			{
				this._changeProposal.SetData(this);
				this._changeProposal.IsPositionLocked = true;
				this._changeProposal.RotationAllowed = true;
				this.GenerateConstrainedTrajectoryProposal(startProposal, null, false, -1f);
				if (this._proposedTrajectorySequence.IsTrajectoryValid)
				{
					this._changeProposal.Velocity = this._proposedTrajectorySequence.End.Velocity;
				}
			}
			else
			{
				this.GenerateHoldTrajectoryProposal(startProposal);
				if (this._proposedTrajectorySequence.IsTrajectoryValid)
				{
					this._changeProposal.SetData(this._proposedTrajectorySequence.End);
					this._changeProposal.IsPositionLocked = false;
				}
			}
			return this.IsChangeProposalValidForNextWaypoints();
		}

		// Token: 0x06005FFB RID: 24571 RVA: 0x002D56A0 File Offset: 0x002D38A0
		private bool RecursiveRotationHandler(ProposedWaypoint startProposal, INextWaypoint nextWaypoint)
		{
			Quaternion quaternion = base.Rotation;
			quaternion = Quaternion.Euler(base.Rotation.eulerAngles.x, base.Rotation.eulerAngles.y + 90f, base.Rotation.eulerAngles.z);
			if (this.AdjustRotation(quaternion, null))
			{
				this._changeProposal.SetData(this);
				this._changeProposal.IsPositionLocked = true;
				this._changeProposal.RotationAllowed = true;
				this.GenerateConstrainedTrajectoryProposal(startProposal, null, false, -1f);
				if (this._proposedTrajectorySequence.IsTrajectoryValid)
				{
					this._changeProposal.Velocity = this._proposedTrajectorySequence.End.Velocity;
				}
				return this.IsChangeProposalValidForNextWaypoints(new Func<ProposedWaypoint, INextWaypoint, bool>(this.RecursiveRotationHandler));
			}
			Debug.LogError("Rotation Proposal Rejected.");
			return false;
		}

		// Token: 0x06005FFC RID: 24572 RVA: 0x002D577D File Offset: 0x002D397D
		private bool IsChangeProposalValidForNextWaypoints()
		{
			if (this._proposedTrajectorySequence.IsTrajectoryValid && this._nextWaypoint.IsRecursiveStartChangeViable(this._changeProposal))
			{
				this.AdoptProposedChanges();
				return true;
			}
			return false;
		}

		// Token: 0x06005FFD RID: 24573 RVA: 0x002D57A8 File Offset: 0x002D39A8
		private bool IsChangeProposalValidForNextWaypoints(Func<ProposedWaypoint, INextWaypoint, bool> evaluator)
		{
			if (this._proposedTrajectorySequence.IsTrajectoryValid && evaluator(this._changeProposal, this._nextWaypoint))
			{
				this.AdoptProposedChanges();
				return true;
			}
			return false;
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x002D57D4 File Offset: 0x002D39D4
		private void GenerateConstrainedTrajectoryProposal(IWaypoint start, AccelerationConstraints overrideConstraints = null, bool preserveRoll = false, float forceAcceleration = -1f)
		{
			if (overrideConstraints == null)
			{
				this._proposedTrajectorySequence = WaypointTrajectorySequence.CreateConstrainedTrajectory(start, this._changeProposal, this._constraints, preserveRoll, false, 0f, forceAcceleration);
				return;
			}
			this._proposedTrajectorySequence = WaypointTrajectorySequence.CreateConstrainedTrajectory(start, this._changeProposal, overrideConstraints, preserveRoll, false, 0f, forceAcceleration);
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x002D5824 File Offset: 0x002D3A24
		private void GenerateHoldTrajectoryProposal(IWaypoint start)
		{
			float num = (float)(base.Timing - start.Timing).TotalSeconds;
			this._proposedTrajectorySequence = WaypointTrajectorySequence.CreateHoldTrajectory(start, num, base.AlphaBlendValue);
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x002D5860 File Offset: 0x002D3A60
		private void AdoptProposedChanges()
		{
			this._isProposalSource = false;
			this.SetData(this._changeProposal);
			this._previousWaypoint.SetNextWaypoint(this);
			this._activeTrajectorySequence = this._proposedTrajectorySequence;
			this._proposedTrajectorySequence = WaypointTrajectorySequence.InvalidTrajectorySequence;
			base.Rotation = this._activeTrajectorySequence.End.Rotation;
			Action onPositionRotationChange = this.OnPositionRotationChange;
			if (onPositionRotationChange == null)
			{
				return;
			}
			onPositionRotationChange();
		}

		// Token: 0x06006001 RID: 24577 RVA: 0x002D58C9 File Offset: 0x002D3AC9
		public void SetNextWaypoint(INextWaypoint nextWaypoint)
		{
			this._nextWaypoint = nextWaypoint ?? new AdjustableWaypoint.EndlessTrajectory(this);
		}

		// Token: 0x06006002 RID: 24578 RVA: 0x002D58DC File Offset: 0x002D3ADC
		public void InsertBefore(AdjustableWaypoint waypoint)
		{
			this._previousWaypoint.SetNextWaypoint(waypoint);
			waypoint.EstablishDesiredPreviousPoint(this._previousWaypoint);
			waypoint.SetNextWaypoint(this);
			this.SetPreviousWaypoint(waypoint);
			waypoint.ProposePlacement(waypoint.Position, null, false, -1f);
		}

		// Token: 0x06006003 RID: 24579 RVA: 0x002D5918 File Offset: 0x002D3B18
		public bool RequestRemoval()
		{
			if (!this.IsCoreWaypoint)
			{
				this.RemoveWaypoint();
				return true;
			}
			return false;
		}

		// Token: 0x06006004 RID: 24580 RVA: 0x002D592B File Offset: 0x002D3B2B
		private void RemoveWaypoint()
		{
			this._previousWaypoint.SetNextWaypoint(this._nextWaypoint);
			this._nextWaypoint.SetPreviousWaypoint(this._previousWaypoint);
			this._previousWaypoint.ResetNextWaypointSequence();
		}

		// Token: 0x040043FA RID: 17402
		private static int _UIDGenerator;

		// Token: 0x040043FC RID: 17404
		private int _uid;

		// Token: 0x040043FD RID: 17405
		private AccelerationConstraints _constraints;

		// Token: 0x040043FE RID: 17406
		private ProposedWaypoint _changeProposal;

		// Token: 0x040043FF RID: 17407
		private WaypointTrajectorySequence _proposedTrajectorySequence;

		// Token: 0x04004400 RID: 17408
		private WaypointTrajectorySequence _activeTrajectorySequence;

		// Token: 0x04004401 RID: 17409
		private IPreviousWaypoint _previousWaypoint;

		// Token: 0x04004402 RID: 17410
		private INextWaypoint _nextWaypoint;

		// Token: 0x04004403 RID: 17411
		private bool _isProposalSource;

		// Token: 0x02001385 RID: 4997
		private class EndlessTrajectory : HoldTrajectory, INextWaypoint, IPathDetail
		{
			// Token: 0x06009165 RID: 37221 RVA: 0x0034761A File Offset: 0x0034581A
			public override Vector3 PositionAt(TIDateTime time)
			{
				return this.PositionAt(base.ElapsedTimeInSeconds(time));
			}

			// Token: 0x06009166 RID: 37222 RVA: 0x00347629 File Offset: 0x00345829
			public override Vector3 VelocityAt(TIDateTime time)
			{
				return this.VelocityAt(base.ElapsedTimeInSeconds(time));
			}

			// Token: 0x06009167 RID: 37223 RVA: 0x00347638 File Offset: 0x00345838
			public override float AngularVelocityAt_Rad(TIDateTime time)
			{
				return this.AngularVelocityAt(base.ElapsedTimeInSeconds(time));
			}

			// Token: 0x06009168 RID: 37224 RVA: 0x00347647 File Offset: 0x00345847
			public override Vector3 HeadingAt(TIDateTime time)
			{
				return this.HeadingAt(base.ElapsedTimeInSeconds(time));
			}

			// Token: 0x06009169 RID: 37225 RVA: 0x00347656 File Offset: 0x00345856
			public override Quaternion RotationAt(TIDateTime time)
			{
				return this.RotationAt(base.ElapsedTimeInSeconds(time));
			}

			// Token: 0x0600916A RID: 37226 RVA: 0x00347665 File Offset: 0x00345865
			public void SetPreviousWaypoint(IPreviousWaypoint previousWaypoint)
			{
			}

			// Token: 0x0600916B RID: 37227 RVA: 0x00347667 File Offset: 0x00345867
			public void RecalculateTrajectoryPathRecursive()
			{
			}

			// Token: 0x0600916C RID: 37228 RVA: 0x00347669 File Offset: 0x00345869
			public void ResumePreviousTargetPosition(Vector3 targetDisplacement)
			{
			}

			// Token: 0x0600916D RID: 37229 RVA: 0x0034766B File Offset: 0x0034586B
			public void CacheWaypointOrientationRecursively()
			{
			}

			// Token: 0x0600916E RID: 37230 RVA: 0x0034766D File Offset: 0x0034586D
			public void AllignToTrajectoryPathRecursively(WaypointTrajectorySequence sequence, TIDateTime endTime, Vector3 targetDisplacement)
			{
			}

			// Token: 0x0600916F RID: 37231 RVA: 0x0034766F File Offset: 0x0034586F
			public void HoldRecursively()
			{
			}

			// Token: 0x06009170 RID: 37232 RVA: 0x00347671 File Offset: 0x00345871
			public void UpdatePathRender(TIDateTime timingStart, Camera cam, Vector3 shipPosition)
			{
			}

			// Token: 0x06009171 RID: 37233 RVA: 0x00347673 File Offset: 0x00345873
			public EndlessTrajectory(IWaypoint previousWaypoint)
				: base(previousWaypoint)
			{
			}

			// Token: 0x170012DB RID: 4827
			// (get) Token: 0x06009172 RID: 37234 RVA: 0x0034767C File Offset: 0x0034587C
			public bool IsRecursivelyLocked
			{
				get
				{
					return false;
				}
			}

			// Token: 0x06009173 RID: 37235 RVA: 0x0034767F File Offset: 0x0034587F
			public bool IsRecursiveStartChangeViable(IWaypoint changeProposal)
			{
				return true;
			}

			// Token: 0x06009174 RID: 37236 RVA: 0x00347682 File Offset: 0x00345882
			public void ResetLocksRecursive()
			{
			}

			// Token: 0x170012DC RID: 4828
			// (get) Token: 0x06009175 RID: 37237 RVA: 0x00347684 File Offset: 0x00345884
			public ITrajectory ValidTrajectorySequence
			{
				get
				{
					return this;
				}
			}
		}
	}
}
