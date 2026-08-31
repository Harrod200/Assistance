using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F3 RID: 2547
	public sealed class DriftTrajectory : HoldTrajectory
	{
		// Token: 0x06006073 RID: 24691 RVA: 0x002D5F98 File Offset: 0x002D4198
		public DriftTrajectory(IPreviousTrajectory start, IProposedWaypoint end, float linearAcceleration, bool includeCounterBurn = false)
			: base(start)
		{
			this._pathLineColor = new Color(0f, 0.5f, 0.5f, 1f);
			float num = (float)(end.Timing - start.Timing).TotalSeconds;
			this._totalDuration_s = num;
			this._timeToComplete = num;
			Vector3 vector = PhysicsHelpers.PositionFromVelocityAndTime(start.Position, start.Velocity, this._timeToComplete);
			this._desiredDisplacementVector = end.Position - vector;
			this._timeToComplete -= Mathf.Sqrt(this._timeToComplete * this._timeToComplete - PhysicsHelpers.TimeSquaredFromDisplacementAndAcceleration(this._desiredDisplacementVector.magnitude, this._linearAcceleration));
			this._linearAcceleration = linearAcceleration;
			this._mainCamera = Camera.main;
			this.InitializeDriftTrajectory(start, end, num, includeCounterBurn);
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x002D6078 File Offset: 0x002D4278
		public DriftTrajectory(IPreviousTrajectory start, IProposedWaypoint end, float availableTime, float linearAcceleration, bool includeCounterBurn = false)
			: base(start)
		{
			this._pathLineColor = new Color(0f, 0.5f, 0.5f, 1f);
			this._timeToComplete = availableTime;
			this._totalDuration_s = availableTime;
			Vector3 vector = PhysicsHelpers.PositionFromVelocityAndTime(start.Position, start.Velocity, availableTime);
			this._desiredDisplacementVector = end.Position - vector;
			this._timeToComplete -= Mathf.Sqrt(this._timeToComplete * this._timeToComplete - PhysicsHelpers.TimeSquaredFromDisplacementAndAcceleration(this._desiredDisplacementVector.magnitude, this._linearAcceleration));
			this._linearAcceleration = linearAcceleration;
			this._mainCamera = Camera.main;
			this.InitializeDriftTrajectory(start, end, availableTime, includeCounterBurn);
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x002D6138 File Offset: 0x002D4338
		private void InitializeDriftTrajectory(IPreviousTrajectory start, IProposedWaypoint end, float availableTime, bool includeCounterBurn)
		{
			this._includeCounterBurn = includeCounterBurn;
			bool flag = false;
			if (float.IsNaN(this._timeToComplete))
			{
				this._timeToComplete = availableTime;
				flag = true;
			}
			this.SetData(start);
			base.Rotation = start.Rotation;
			Vector3 vector = PhysicsHelpers.DisplacementFromVelocityAndTime(start.Velocity, this._timeToComplete);
			Vector3 vector2 = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._desiredDisplacementVector.normalized, this._linearAcceleration, this._timeToComplete);
			Vector3 vector3 = PhysicsHelpers.VelocityFromAccelerationAndTime(this._desiredDisplacementVector.normalized, this._linearAcceleration, this._timeToComplete);
			base.Timing.AddSeconds((double)this._timeToComplete);
			base.Position += vector + vector2;
			base.Velocity = start.Velocity + vector3;
			if (this._includeCounterBurn)
			{
				vector += PhysicsHelpers.DisplacementFromVelocityAndTime(vector3, this._timeToComplete);
				Vector3 vector4 = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._desiredDisplacementVector.normalized, -this._linearAcceleration, this._timeToComplete);
				Vector3 vector5 = PhysicsHelpers.VelocityFromAccelerationAndTime(this._desiredDisplacementVector.normalized, -this._linearAcceleration, this._timeToComplete);
				base.Timing.AddSeconds((double)this._timeToComplete);
				base.Position += vector + vector4;
				base.Velocity += vector5;
			}
			if (flag)
			{
				end.Position = base.Position;
			}
			end.Velocity = base.Velocity;
			end.Rotation = base.Rotation;
			base.InitializePathList();
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x002D62D7 File Offset: 0x002D44D7
		private bool IsUsingRightThruster(TIDateTime time)
		{
			return this.IsAcceleratingInRelativeDirection(time, new Vector3(1f, 0f, 0f));
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x002D62F4 File Offset: 0x002D44F4
		private bool IsUsingLeftThruster(TIDateTime time)
		{
			return this.IsAcceleratingInRelativeDirection(time, new Vector3(-1f, 0f, 0f));
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x002D6311 File Offset: 0x002D4511
		private bool IsUsingUpThruster(TIDateTime time)
		{
			return this.IsAcceleratingInRelativeDirection(time, new Vector3(0f, -1f, 0f));
		}

		// Token: 0x06006079 RID: 24697 RVA: 0x002D632E File Offset: 0x002D452E
		private bool IsUsingDownThruster(TIDateTime time)
		{
			return this.IsAcceleratingInRelativeDirection(time, new Vector3(0f, 1f, 0f));
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x002D634B File Offset: 0x002D454B
		private bool IsAcceleratingInRelativeDirection(TIDateTime time, Vector3 direction)
		{
			return (this.RotationAt(time) * direction).Dot(this.AccelerationAt(time)) > 0f;
		}

		// Token: 0x0600607B RID: 24699 RVA: 0x002D636D File Offset: 0x002D456D
		public override bool IsAcceleratingRight(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.IsUsingLeftThruster(time);
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.IsUsingLeftThruster(time);
			}
			return nextTrajectory.IsAcceleratingLeft(time);
		}

		// Token: 0x0600607C RID: 24700 RVA: 0x002D639D File Offset: 0x002D459D
		public override bool IsAcceleratingLeft(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.IsUsingRightThruster(time);
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.IsUsingRightThruster(time);
			}
			return nextTrajectory.IsAcceleratingRight(time);
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x002D63CD File Offset: 0x002D45CD
		public override bool IsAcceleratingUp(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.IsUsingDownThruster(time);
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.IsUsingDownThruster(time);
			}
			return nextTrajectory.IsAcceleratingDown(time);
		}

		// Token: 0x0600607E RID: 24702 RVA: 0x002D63FD File Offset: 0x002D45FD
		public override bool IsAcceleratingDown(TIDateTime time)
		{
			if (time < base.Timing)
			{
				return this.IsUsingUpThruster(time);
			}
			ITrajectory nextTrajectory = this._nextTrajectory;
			if (nextTrajectory == null)
			{
				return this.IsUsingUpThruster(time);
			}
			return nextTrajectory.IsAcceleratingUp(time);
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x002D642D File Offset: 0x002D462D
		public static float TimeRequiredForDisplacement(Vector3 desiredDisplacementVector, float linearAcceleration, bool includeCounterBurn = false)
		{
			if (includeCounterBurn)
			{
				return PhysicsHelpers.TimeFromDisplacementAndAcceleration(desiredDisplacementVector.magnitude * 0.5f, linearAcceleration) * 2f;
			}
			return PhysicsHelpers.TimeFromDisplacementAndAcceleration(desiredDisplacementVector.magnitude, linearAcceleration);
		}

		// Token: 0x06006080 RID: 24704 RVA: 0x002D645C File Offset: 0x002D465C
		protected override Vector3 PositionAt(float elapsedTime)
		{
			float num = Mathf.Clamp(elapsedTime, 0f, this._timeToComplete);
			Vector3 vector = PhysicsHelpers.DisplacementFromVelocityAndTime(this._previousWaypoint.Velocity, num);
			Vector3 vector2 = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._desiredDisplacementVector.normalized, this._linearAcceleration, num);
			if (this._includeCounterBurn)
			{
				float num2 = Mathf.Clamp(elapsedTime - this._timeToComplete, 0f, this._timeToComplete);
				float num3 = PhysicsHelpers.VelocityFromAccelerationAndTime(this._linearAcceleration, num);
				vector += PhysicsHelpers.DisplacementFromVelocityAndTime(vector2.normalized * num3, num2);
				vector2 -= PhysicsHelpers.DisplacementFromAccelerationAndTime(this._desiredDisplacementVector.normalized, this._linearAcceleration, num2);
			}
			return this._previousWaypoint.Position + vector + vector2;
		}

		// Token: 0x06006081 RID: 24705 RVA: 0x002D6530 File Offset: 0x002D4730
		protected override Vector3 VelocityAt(float elapsedTime)
		{
			float num = Mathf.Clamp(elapsedTime, 0f, this._timeToComplete);
			Vector3 vector = this._previousWaypoint.Velocity + PhysicsHelpers.VelocityFromAccelerationAndTime(this._desiredDisplacementVector.normalized, this._linearAcceleration, num);
			if (this._includeCounterBurn)
			{
				float num2 = Mathf.Clamp(elapsedTime - this._timeToComplete, 0f, this._timeToComplete);
				vector += PhysicsHelpers.VelocityFromAccelerationAndTime(this._desiredDisplacementVector.normalized, -this._linearAcceleration, num2);
			}
			return vector;
		}

		// Token: 0x06006082 RID: 24706 RVA: 0x002D65C0 File Offset: 0x002D47C0
		protected override Vector3 AccelerationAt(float elapsedTime)
		{
			if (this._includeCounterBurn && elapsedTime > this._timeToComplete)
			{
				return this._desiredDisplacementVector.normalized * -this._linearAcceleration;
			}
			return this._desiredDisplacementVector.normalized * this._linearAcceleration;
		}

		// Token: 0x06006083 RID: 24707 RVA: 0x002D6614 File Offset: 0x002D4814
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public override List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			if (this._nextTrajectory != null)
			{
				List<ValueTuple<TIDateTime, bool>> burnTimings = this._nextTrajectory.GetBurnTimings();
				if (this._includeCounterBurn)
				{
					burnTimings.Insert(0, new ValueTuple<TIDateTime, bool>(new TIDateTime(this._previousWaypoint.Timing, (double)this._timeToComplete), true));
				}
				burnTimings.Insert(0, new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, true));
				return burnTimings;
			}
			if (this._includeCounterBurn)
			{
				return new List<ValueTuple<TIDateTime, bool>>
				{
					new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, true),
					new ValueTuple<TIDateTime, bool>(new TIDateTime(this._previousWaypoint.Timing, (double)this._timeToComplete), true),
					new ValueTuple<TIDateTime, bool>(base.Timing, false)
				};
			}
			return new List<ValueTuple<TIDateTime, bool>>
			{
				new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, true),
				new ValueTuple<TIDateTime, bool>(base.Timing, false)
			};
		}

		// Token: 0x0400441C RID: 17436
		private readonly Vector3 _desiredDisplacementVector;

		// Token: 0x0400441D RID: 17437
		private readonly float _linearAcceleration;

		// Token: 0x0400441E RID: 17438
		private float _timeToComplete;

		// Token: 0x0400441F RID: 17439
		private bool _includeCounterBurn;
	}
}
