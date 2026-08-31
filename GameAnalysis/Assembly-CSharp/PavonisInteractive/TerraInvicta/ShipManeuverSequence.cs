using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200070B RID: 1803
	public class ShipManeuverSequence
	{
		// Token: 0x170005DB RID: 1499
		// (get) Token: 0x06002AC7 RID: 10951 RVA: 0x000E83AB File Offset: 0x000E65AB
		// (set) Token: 0x06002AC8 RID: 10952 RVA: 0x000E83B3 File Offset: 0x000E65B3
		public bool ValidSequence { get; private set; }

		// Token: 0x170005DC RID: 1500
		// (get) Token: 0x06002AC9 RID: 10953 RVA: 0x000E83BC File Offset: 0x000E65BC
		// (set) Token: 0x06002ACA RID: 10954 RVA: 0x000E83C4 File Offset: 0x000E65C4
		public IPreviousTrajectory Start { get; private set; }

		// Token: 0x170005DD RID: 1501
		// (get) Token: 0x06002ACB RID: 10955 RVA: 0x000E83CD File Offset: 0x000E65CD
		// (set) Token: 0x06002ACC RID: 10956 RVA: 0x000E83D5 File Offset: 0x000E65D5
		public IPreviousTrajectory End { get; private set; }

		// Token: 0x06002ACD RID: 10957 RVA: 0x000E83DE File Offset: 0x000E65DE
		private ShipManeuverSequence()
		{
		}

		// Token: 0x06002ACE RID: 10958 RVA: 0x000E83E6 File Offset: 0x000E65E6
		public ShipManeuverSequence(float linearAcceleration, float cruiseAcceleration, float angularAcceleration, float maxAngualarAcceleration)
		{
			this._constraints = new AccelerationConstraints(linearAcceleration, cruiseAcceleration, angularAcceleration, maxAngualarAcceleration);
		}

		// Token: 0x06002ACF RID: 10959 RVA: 0x000E8400 File Offset: 0x000E6600
		public void CreateManeuverSequence(IProposedWaypoint start, Vector3 driftTarget, Vector3 burnTarget, IProposedWaypoint end)
		{
			this.Start = new AnchorTrajectory(start);
			IPreviousTrajectory previousTrajectory = this.Start;
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint();
			proposedWaypoint.SetData(previousTrajectory);
			proposedWaypoint.Position = driftTarget;
			proposedWaypoint.Rotation = start.Rotation;
			float num = ShipManeuverSequence.TimeRequiredForDrift((proposedWaypoint.Position - previousTrajectory.Position) * 0.5f, this._constraints.LinearAcceleration);
			proposedWaypoint.Timing.AddSeconds((double)num);
			previousTrajectory = this.HandlePreBurnDriftManeuver(previousTrajectory, proposedWaypoint, num, this._constraints.LinearAcceleration);
			ProposedWaypoint proposedWaypoint2 = new ProposedWaypoint();
			proposedWaypoint2.SetData(previousTrajectory);
			proposedWaypoint2.Rotation = Quaternion.LookRotation((burnTarget - previousTrajectory.Position).normalized);
			float num2 = ShipManeuverSequence.TimeRequiredForHeadingRotation(previousTrajectory.Rotation, proposedWaypoint2.Rotation, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
			proposedWaypoint2.Timing.AddSeconds((double)num2);
			previousTrajectory = this.HandlePreBurnRotationManeuver(previousTrajectory, proposedWaypoint2, num2, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
			float magnitude = (burnTarget - previousTrajectory.Position).magnitude;
			num2 = ShipManeuverSequence.TimeRequiredForHeadingRotation(previousTrajectory.Rotation, previousTrajectory.Rotation * previousTrajectory.Rotation, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
			float num3 = PhysicsHelpers.TimeFromDisplacementAndAcceleration(magnitude * 0.25f, this._constraints.LinearAcceleration);
			Vector3 vector = PhysicsHelpers.DisplacementFromAccelerationAndTime(previousTrajectory.Heading, this._constraints.LinearAcceleration, num3);
			ProposedWaypoint proposedWaypoint3 = new ProposedWaypoint();
			proposedWaypoint3.SetData(previousTrajectory);
			proposedWaypoint3.Position = previousTrajectory.Position + vector;
			proposedWaypoint3.Rotation = previousTrajectory.Rotation;
			proposedWaypoint3.Velocity = this._constraints.LinearAcceleration * this._constraints.LinearAcceleration * previousTrajectory.Heading;
			proposedWaypoint3.Timing.AddSeconds((double)num3);
			previousTrajectory = this.HandleBurn(previousTrajectory, proposedWaypoint3, vector.magnitude, this._constraints.LinearAcceleration);
			if (num2 > 0f)
			{
				ProposedWaypoint proposedWaypoint4 = new ProposedWaypoint();
				proposedWaypoint4.SetData(previousTrajectory);
				proposedWaypoint4.Rotation = Quaternion.LookRotation(-previousTrajectory.Heading);
				proposedWaypoint4.Timing.AddSeconds((double)num2);
				previousTrajectory = this.HandleMidBurnRotationManeuver(previousTrajectory, proposedWaypoint4, num2, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
			}
			float magnitude2 = (burnTarget - previousTrajectory.Position).magnitude;
			float num4 = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._constraints.LinearAcceleration, num3);
			float num5 = (magnitude2 - num4) / previousTrajectory.Velocity.magnitude;
			num5 = Mathf.Max(num5, 0f);
			ProposedWaypoint proposedWaypoint5 = new ProposedWaypoint();
			proposedWaypoint5.SetData(previousTrajectory);
			proposedWaypoint5.Timing.AddSeconds((double)num5);
			previousTrajectory = this.HandlePostBurnHold(previousTrajectory, proposedWaypoint5);
			ProposedWaypoint proposedWaypoint6 = new ProposedWaypoint();
			proposedWaypoint6.SetData(previousTrajectory);
			proposedWaypoint6.Position = burnTarget;
			proposedWaypoint6.Velocity = Vector3.zero;
			proposedWaypoint6.Timing.AddSeconds((double)num3);
			previousTrajectory = this.HandleCounterBurn(previousTrajectory, proposedWaypoint6, vector.magnitude, this._constraints.LinearAcceleration);
			ProposedWaypoint proposedWaypoint7 = new ProposedWaypoint();
			proposedWaypoint7.SetData(previousTrajectory);
			proposedWaypoint7.Position = burnTarget;
			proposedWaypoint7.Velocity = Vector3.zero;
			proposedWaypoint7.Rotation = end.Rotation;
			if (!Mathf.Approximately(Quaternion.Angle(previousTrajectory.Rotation, proposedWaypoint7.Rotation), 0f))
			{
				num2 = ShipManeuverSequence.TimeRequiredForHeadingRotation(previousTrajectory.Rotation, proposedWaypoint7.Rotation, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				proposedWaypoint7.Timing = new TIDateTime(previousTrajectory.Timing);
				proposedWaypoint7.Timing.AddSeconds((double)num2);
				previousTrajectory = this.HandlePostBurnRotationManeuver(previousTrajectory, proposedWaypoint7, num2, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
			}
			ProposedWaypoint proposedWaypoint8 = new ProposedWaypoint();
			proposedWaypoint8.SetData(previousTrajectory);
			proposedWaypoint8.Position = end.Position;
			proposedWaypoint8.Rotation = end.Rotation;
			num = ShipManeuverSequence.TimeRequiredForDrift((proposedWaypoint8.Position - previousTrajectory.Position) * 0.5f, this._constraints.LinearAcceleration);
			proposedWaypoint8.Timing.AddSeconds((double)num);
			previousTrajectory = this.HandlePostBurnDriftManeuver(previousTrajectory, proposedWaypoint8, this._constraints.LinearAcceleration);
			this.End = previousTrajectory;
			this.ValidSequence = true;
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x000E8879 File Offset: 0x000E6A79
		private IPreviousTrajectory HandlePreBurnDriftManeuver(IPreviousTrajectory start, IProposedWaypoint target, float availableTime, float linearAcceleration)
		{
			this._preBurnDrift = new DriftTrajectory(start, target, availableTime, linearAcceleration, true);
			return this._preBurnDrift;
		}

		// Token: 0x06002AD1 RID: 10961 RVA: 0x000E8892 File Offset: 0x000E6A92
		private IPreviousTrajectory HandlePostBurnDriftManeuver(IPreviousTrajectory start, IProposedWaypoint target, float linearAcceleration)
		{
			this._postBurnDrift = new DriftTrajectory(start, target, linearAcceleration, true);
			return this._postBurnDrift;
		}

		// Token: 0x06002AD2 RID: 10962 RVA: 0x000E88A9 File Offset: 0x000E6AA9
		private IPreviousTrajectory HandlePreBurnRotationManeuver(IPreviousTrajectory start, IProposedWaypoint target, float availableTime, float angularAcceleration, float maxAngularVelocity)
		{
			this._preBurnRotation = new RotationTrajectory(start, target, availableTime, angularAcceleration, maxAngularVelocity);
			return this._preBurnRotation;
		}

		// Token: 0x06002AD3 RID: 10963 RVA: 0x000E88C3 File Offset: 0x000E6AC3
		private IPreviousTrajectory HandleMidBurnRotationManeuver(IPreviousTrajectory start, IProposedWaypoint target, float availableTime, float angularAcceleration, float maxAngularVelocity)
		{
			this._midBurnRotation = new RotationTrajectory(start, target, availableTime, angularAcceleration, maxAngularVelocity);
			return this._midBurnRotation;
		}

		// Token: 0x06002AD4 RID: 10964 RVA: 0x000E88DD File Offset: 0x000E6ADD
		private IPreviousTrajectory HandlePostBurnRotationManeuver(IPreviousTrajectory start, IProposedWaypoint target, float availableTime, float angularAcceleration, float maxAngularVelocity)
		{
			this._postBurnRotation = new RotationTrajectory(start, target, availableTime, angularAcceleration, maxAngularVelocity);
			return this._postBurnRotation;
		}

		// Token: 0x06002AD5 RID: 10965 RVA: 0x000E88F7 File Offset: 0x000E6AF7
		private IPreviousTrajectory HandleBurn(IPreviousTrajectory current, IProposedWaypoint target, float desiredDisplacement, float linearAcceleration)
		{
			this._burn = new BurnTrajectory(current, target, desiredDisplacement, linearAcceleration);
			return this._burn;
		}

		// Token: 0x06002AD6 RID: 10966 RVA: 0x000E890F File Offset: 0x000E6B0F
		private IPreviousTrajectory HandleCounterBurn(IPreviousTrajectory current, IProposedWaypoint target, float desiredDisplacement, float linearAcceleration)
		{
			this._counterBurn = new BurnTrajectory(current, target, desiredDisplacement, linearAcceleration);
			return this._counterBurn;
		}

		// Token: 0x06002AD7 RID: 10967 RVA: 0x000E8927 File Offset: 0x000E6B27
		private IPreviousTrajectory HandlePostBurnHold(IPreviousTrajectory current, IProposedWaypoint target)
		{
			this._postBurnHold = new HoldTrajectory(current, target);
			return this._postBurnHold;
		}

		// Token: 0x06002AD8 RID: 10968 RVA: 0x000E893C File Offset: 0x000E6B3C
		private static float TimeRequiredForDrift(Vector3 desiredDisplacementVector, float linearAcceleration)
		{
			return DriftTrajectory.TimeRequiredForDisplacement(desiredDisplacementVector, linearAcceleration, false);
		}

		// Token: 0x06002AD9 RID: 10969 RVA: 0x000E8946 File Offset: 0x000E6B46
		private static float TimeRequiredForHeadingRotation(Quaternion currentRotation, Quaternion requiredRotation, float angularAcceleration, float maxAngularVelocity)
		{
			return RotationTrajectory.TimeRequiredForHeadingRotation(currentRotation, requiredRotation, angularAcceleration, maxAngularVelocity);
		}

		// Token: 0x06002ADA RID: 10970 RVA: 0x000E8951 File Offset: 0x000E6B51
		public ITrajectory TrajectoryAt(TIDateTime time)
		{
			return this.Start.TrajectoryAt(time);
		}

		// Token: 0x06002ADB RID: 10971 RVA: 0x000E895F File Offset: 0x000E6B5F
		public void PositionAt(TIDateTime time, out Vector3 position)
		{
			if (this.Start == null)
			{
				position = Vector3.zero;
				return;
			}
			position = this.Start.PositionAt(time);
		}

		// Token: 0x06002ADC RID: 10972 RVA: 0x000E8987 File Offset: 0x000E6B87
		public void RotationAt(TIDateTime time, out Quaternion rotation)
		{
			if (this.Start == null)
			{
				rotation = Quaternion.identity;
				return;
			}
			rotation = this.Start.RotationAt(time);
		}

		// Token: 0x06002ADD RID: 10973 RVA: 0x000E89B0 File Offset: 0x000E6BB0
		public void PositionAndRotationAt(TIDateTime time, out Vector3 position, out Quaternion rotation)
		{
			if (this.Start == null)
			{
				position = Vector3.zero;
				rotation = Quaternion.identity;
				return;
			}
			ITrajectory trajectory = this.Start.TrajectoryAt(time);
			position = trajectory.PositionAt(time);
			rotation = trajectory.RotationAt(time);
		}

		// Token: 0x06002ADE RID: 10974 RVA: 0x000E8A03 File Offset: 0x000E6C03
		public bool IsInBurn(TIDateTime time)
		{
			return this.Start.IsInBurn(time);
		}

		// Token: 0x06002ADF RID: 10975 RVA: 0x000E8A11 File Offset: 0x000E6C11
		public bool IsAcceleratingRight(TIDateTime time)
		{
			return this.Start.IsAcceleratingRight(time);
		}

		// Token: 0x06002AE0 RID: 10976 RVA: 0x000E8A1F File Offset: 0x000E6C1F
		public bool IsAcceleratingLeft(TIDateTime time)
		{
			return this.Start.IsAcceleratingLeft(time);
		}

		// Token: 0x06002AE1 RID: 10977 RVA: 0x000E8A2D File Offset: 0x000E6C2D
		public bool IsAcceleratingUp(TIDateTime time)
		{
			return this.Start.IsAcceleratingUp(time);
		}

		// Token: 0x06002AE2 RID: 10978 RVA: 0x000E8A3B File Offset: 0x000E6C3B
		public bool IsAcceleratingDown(TIDateTime time)
		{
			return this.Start.IsAcceleratingDown(time);
		}

		// Token: 0x040020C3 RID: 8387
		private AccelerationConstraints _constraints;

		// Token: 0x040020C4 RID: 8388
		private DriftTrajectory _preBurnDrift;

		// Token: 0x040020C5 RID: 8389
		private RotationTrajectory _preBurnRotation;

		// Token: 0x040020C6 RID: 8390
		private BurnTrajectory _burn;

		// Token: 0x040020C7 RID: 8391
		private RotationTrajectory _midBurnRotation;

		// Token: 0x040020C8 RID: 8392
		private HoldTrajectory _postBurnHold;

		// Token: 0x040020C9 RID: 8393
		private BurnTrajectory _counterBurn;

		// Token: 0x040020CA RID: 8394
		private RotationTrajectory _postBurnRotation;

		// Token: 0x040020CB RID: 8395
		private DriftTrajectory _postBurnDrift;
	}
}
