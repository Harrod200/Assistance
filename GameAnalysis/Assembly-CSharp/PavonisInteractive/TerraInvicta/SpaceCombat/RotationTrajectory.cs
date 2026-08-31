using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F5 RID: 2549
	public sealed class RotationTrajectory : HoldTrajectory
	{
		// Token: 0x060060A1 RID: 24737 RVA: 0x002D6F40 File Offset: 0x002D5140
		public RotationTrajectory(IPreviousTrajectory start, IWaypoint end, float angularAcceleration, float maxAngularVelocity)
			: this(start, end, (float)(end.Timing - start.Timing).TotalSeconds, angularAcceleration, maxAngularVelocity)
		{
			this._mainCamera = Camera.main;
		}

		// Token: 0x060060A2 RID: 24738 RVA: 0x002D6F80 File Offset: 0x002D5180
		public RotationTrajectory(IPreviousTrajectory start, IWaypoint end, float availableTime, float angularAcceleration, float maxAngularVelocity)
			: base(start)
		{
			this._pathLineColor = new Color(0.65f, 0.598f, 0.0104f, 1f);
			this._angularAcceleration = angularAcceleration;
			this._maxAngularAcceleration = maxAngularVelocity;
			float num = RotationTrajectory.TopAngularVelocityForHeadingRotationLimitedByTime(start.Rotation, end.Rotation, angularAcceleration, maxAngularVelocity, availableTime);
			float num2 = RotationTrajectory.TimeRequiredForHeadingRotationLimitedByTime(start.Rotation, end.Rotation, angularAcceleration, maxAngularVelocity, availableTime);
			this._accelerationTime = num / angularAcceleration * 2f;
			this._idleTime = Mathf.Max(num2 - this._accelerationTime, 0f);
			this._totalDuration_s = this._accelerationTime + this._idleTime;
			this._totalDuration_s = Mathf.Min(availableTime, this._totalDuration_s);
			this._targetRotation = end.Rotation;
			this.SetData(start);
			base.Position = PhysicsHelpers.PositionFromVelocityAndTime(start.Position, start.Velocity, this._totalDuration_s);
			base.Rotation = this.RotationAt(this._totalDuration_s);
			base.Timing.AddSeconds((double)this._totalDuration_s);
			end.Rotation = base.Rotation;
			Quaternion quaternion = Quaternion.Inverse(this._previousWaypoint.Rotation) * base.Rotation;
			this._rotationAxis = new Vector3(quaternion.x, quaternion.y, quaternion.z).normalized;
			base.AlphaBlendValue = Mathf.Lerp(start.AlphaBlendValue, end.AlphaBlendValue, this._totalDuration_s / availableTime);
			this._alphaRange.x = start.AlphaBlendValue;
			this._alphaRange.y = base.AlphaBlendValue;
			this._mainCamera = Camera.main;
			base.InitializePathList();
		}

		// Token: 0x060060A3 RID: 24739 RVA: 0x002D7138 File Offset: 0x002D5338
		public static float TimeRequiredForHeadingRotation(Quaternion currentRotation, Quaternion requiredRotation, float angularAcceleration, float maxAngularVelocity)
		{
			if (maxAngularVelocity <= 1E-45f || angularAcceleration <= 1E-45f)
			{
				return float.MaxValue;
			}
			float num = Quaternion.Angle(currentRotation, requiredRotation) * 0.017453292f;
			if (num == 0f)
			{
				return 0f;
			}
			float num2 = PhysicsHelpers.TimeFromDisplacementAndAcceleration(num * 0.5f, angularAcceleration);
			float num3 = Mathf.Min(maxAngularVelocity, PhysicsHelpers.VelocityFromAccelerationAndTime(angularAcceleration, num2));
			return num / num3 + num3 / angularAcceleration;
		}

		// Token: 0x060060A4 RID: 24740 RVA: 0x002D719C File Offset: 0x002D539C
		public static float TimeRequiredForHeadingRotationLimitedByTime(Quaternion currentRotation, Quaternion requiredRotation, float angularAcceleration, float maxAngularVelocity, float availableTime)
		{
			if (maxAngularVelocity <= 1E-45f || angularAcceleration <= 1E-45f)
			{
				return float.MaxValue;
			}
			float num = Quaternion.Angle(currentRotation, requiredRotation) * 0.017453292f;
			if (num == 0f)
			{
				return 0f;
			}
			float num2 = Mathf.Min(PhysicsHelpers.TimeFromDisplacementAndAcceleration(num * 0.5f, angularAcceleration), availableTime / 2f);
			float num3 = Mathf.Min(maxAngularVelocity, PhysicsHelpers.VelocityFromAccelerationAndTime(angularAcceleration, num2));
			return Mathf.Min(num / num3 + num3 / angularAcceleration, availableTime);
		}

		// Token: 0x060060A5 RID: 24741 RVA: 0x002D7214 File Offset: 0x002D5414
		public static float TopAngularVelocityForHeadingRotationLimitedByTime(Quaternion currentRotation, Quaternion requiredRotation, float angularAcceleration, float maxAngularVelocity, float availableTime)
		{
			float num = Quaternion.Angle(currentRotation, requiredRotation) * 0.017453292f;
			if (num == 0f)
			{
				return 0f;
			}
			float num2 = Mathf.Min(PhysicsHelpers.TimeFromDisplacementAndAcceleration(num * 0.5f, angularAcceleration), availableTime / 2f);
			return Mathf.Min(maxAngularVelocity, PhysicsHelpers.VelocityFromAccelerationAndTime(angularAcceleration, num2));
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x002D7268 File Offset: 0x002D5468
		private bool IsUsingRightThruster(TIDateTime time)
		{
			float num = (float)(time - this._previousWaypoint.Timing).TotalSeconds;
			float y = this._rotationAxis.y;
			return (num < this._accelerationTime * 0.5f && y < -0.17f) || (num > this._accelerationTime * 0.5f + this._idleTime && y > 0.17f);
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x002D72D4 File Offset: 0x002D54D4
		private bool IsUsingLeftThruster(TIDateTime time)
		{
			float num = (float)(time - this._previousWaypoint.Timing).TotalSeconds;
			float y = this._rotationAxis.y;
			return (num > this._accelerationTime * 0.5f + this._idleTime && y < -0.17f) || (num < this._accelerationTime * 0.5f && y > 0.17f);
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x002D7340 File Offset: 0x002D5540
		private bool IsUsingUpThruster(TIDateTime time)
		{
			float num = (float)(time - this._previousWaypoint.Timing).TotalSeconds;
			float x = this._rotationAxis.x;
			return (num < this._accelerationTime * 0.5f && x < -0.17f) || (num > this._accelerationTime * 0.5f + this._idleTime && x > 0.17f);
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x002D73AC File Offset: 0x002D55AC
		private bool IsUsingDownThruster(TIDateTime time)
		{
			float num = (float)(time - this._previousWaypoint.Timing).TotalSeconds;
			float x = this._rotationAxis.x;
			return (num > this._accelerationTime * 0.5f + this._idleTime && x < -0.17f) || (num < this._accelerationTime * 0.5f && x > 0.17f);
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x002D7418 File Offset: 0x002D5618
		private bool IsUsingRollRightThruster(TIDateTime time)
		{
			float num = (float)(time - this._previousWaypoint.Timing).TotalSeconds;
			float z = this._rotationAxis.z;
			return (num < this._accelerationTime * 0.5f && z > 0.17f) || (num > this._accelerationTime * 0.5f + this._idleTime && z < -0.17f);
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x002D7484 File Offset: 0x002D5684
		private bool IsUsingRollLeftThruster(TIDateTime time)
		{
			float num = (float)(time - this._previousWaypoint.Timing).TotalSeconds;
			float z = this._rotationAxis.z;
			return (num > this._accelerationTime * 0.5f + this._idleTime && z > 0.17f) || (num < this._accelerationTime * 0.5f && z < -0.17f);
		}

		// Token: 0x060060AC RID: 24748 RVA: 0x002D74F0 File Offset: 0x002D56F0
		public override bool IsAcceleratingRight(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsAcceleratingRight(time);
			}
			return this.IsUsingRightThruster(time);
		}

		// Token: 0x060060AD RID: 24749 RVA: 0x002D751A File Offset: 0x002D571A
		public override bool IsAcceleratingLeft(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsAcceleratingLeft(time);
			}
			return this.IsUsingLeftThruster(time);
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x002D7544 File Offset: 0x002D5744
		public override bool IsAcceleratingUp(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsAcceleratingUp(time);
			}
			return this.IsUsingUpThruster(time);
		}

		// Token: 0x060060AF RID: 24751 RVA: 0x002D756E File Offset: 0x002D576E
		public override bool IsAcceleratingDown(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsAcceleratingDown(time);
			}
			return this.IsUsingDownThruster(time);
		}

		// Token: 0x060060B0 RID: 24752 RVA: 0x002D7598 File Offset: 0x002D5798
		public override bool IsAcceleratingRollRight(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsAcceleratingRollRight(time);
			}
			return this.IsUsingRollRightThruster(time);
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x002D75C2 File Offset: 0x002D57C2
		public override bool IsAcceleratingRollLeft(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsAcceleratingRollLeft(time);
			}
			return this.IsUsingRollLeftThruster(time);
		}

		// Token: 0x060060B2 RID: 24754 RVA: 0x002D75EC File Offset: 0x002D57EC
		protected override Quaternion RotationAt(float elapsedTime)
		{
			float num = this.TotalAngularDisplacementInDegreesAtTime(elapsedTime);
			return Quaternion.RotateTowards(this._previousWaypoint.Rotation, this._targetRotation, num);
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x002D7618 File Offset: 0x002D5818
		protected override Vector3 HeadingAt(float elapsedTime)
		{
			return this.RotationAt(elapsedTime) * Vector3.forward;
		}

		// Token: 0x060060B4 RID: 24756 RVA: 0x002D762C File Offset: 0x002D582C
		protected override float AngularVelocityAt(float elapsedTime)
		{
			Quaternion rotation = this._previousWaypoint.Rotation;
			Quaternion rotation2 = base.Rotation;
			float num = Quaternion.Angle(rotation, rotation2) * 0.017453292f;
			if (num == 0f)
			{
				return 0f;
			}
			if (this.TotalAngularDisplacementInDegreesAtTime(elapsedTime) * 0.017453292f == 0f)
			{
				return 0f;
			}
			float num2 = PhysicsHelpers.TimeFromDisplacementAndAcceleration(num * 0.5f, this._angularAcceleration);
			float num3 = elapsedTime;
			if (elapsedTime > num2)
			{
				num3 = num2 * 2f - elapsedTime;
			}
			return Mathf.Min(this._maxAngularAcceleration, PhysicsHelpers.VelocityFromAccelerationAndTime(this._angularAcceleration, num3));
		}

		// Token: 0x060060B5 RID: 24757 RVA: 0x002D76BC File Offset: 0x002D58BC
		private float TotalAngularDisplacementInDegreesAtTime(float elapsedTime)
		{
			float num = Mathf.Clamp(elapsedTime, 0f, this._accelerationTime * 0.5f);
			float num2 = Mathf.Clamp(elapsedTime - num * 0.5f, 0f, this._idleTime);
			float num3 = Mathf.Clamp(elapsedTime - num * 0.5f - this._idleTime, 0f, this._accelerationTime * 0.5f);
			float num4 = PhysicsHelpers.VelocityFromAccelerationAndTime(this._angularAcceleration, num);
			float num5 = PhysicsHelpers.DisplacementFromVelocityAccelerationAndTime(0f, this._angularAcceleration, num);
			float num6 = PhysicsHelpers.DisplacementFromVelocityAccelerationAndTime(num4, 0f, num2);
			float num7 = PhysicsHelpers.DisplacementFromVelocityAccelerationAndTime(num4, -this._angularAcceleration, num3);
			return (num5 + num6 + num7) * 57.29578f;
		}

		// Token: 0x060060B6 RID: 24758 RVA: 0x002D776C File Offset: 0x002D596C
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public override List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			if (this._nextTrajectory == null)
			{
				return new List<ValueTuple<TIDateTime, bool>>
				{
					new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, false)
				};
			}
			List<ValueTuple<TIDateTime, bool>> burnTimings = this._nextTrajectory.GetBurnTimings();
			burnTimings.Insert(0, new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, false));
			return burnTimings;
		}

		// Token: 0x04004428 RID: 17448
		private const float MIN_ANGLE_FOR_THRUSTER_ACTIVATION = 0.17f;

		// Token: 0x04004429 RID: 17449
		private readonly float _angularAcceleration;

		// Token: 0x0400442A RID: 17450
		private readonly float _maxAngularAcceleration;

		// Token: 0x0400442B RID: 17451
		private readonly float _idleTime;

		// Token: 0x0400442C RID: 17452
		private readonly float _accelerationTime;

		// Token: 0x0400442D RID: 17453
		private readonly Vector3 _rotationAxis;

		// Token: 0x0400442E RID: 17454
		private readonly Quaternion _targetRotation;
	}
}
