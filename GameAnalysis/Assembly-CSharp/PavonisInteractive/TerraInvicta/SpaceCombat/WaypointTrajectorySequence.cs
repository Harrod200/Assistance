using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F6 RID: 2550
	public class WaypointTrajectorySequence : ITrajectory, IPathDetail
	{
		// Token: 0x170010A6 RID: 4262
		// (get) Token: 0x060060B7 RID: 24759 RVA: 0x002D77C1 File Offset: 0x002D59C1
		public static WaypointTrajectorySequence InvalidTrajectorySequence
		{
			get
			{
				if (WaypointTrajectorySequence.s_InvalidTrajectorySequence == null)
				{
					WaypointTrajectorySequence.s_InvalidTrajectorySequence = new WaypointTrajectorySequence();
					WaypointTrajectorySequence.s_InvalidTrajectorySequence.IsTrajectoryValid = false;
				}
				return WaypointTrajectorySequence.s_InvalidTrajectorySequence;
			}
		}

		// Token: 0x170010A7 RID: 4263
		// (get) Token: 0x060060B8 RID: 24760 RVA: 0x002D77E4 File Offset: 0x002D59E4
		public float intendedLinearAcceleration
		{
			get
			{
				return this._intendedLinearAcceleration;
			}
		}

		// Token: 0x170010A8 RID: 4264
		// (get) Token: 0x060060B9 RID: 24761 RVA: 0x002D77EC File Offset: 0x002D59EC
		private float linearAcceleration
		{
			get
			{
				return Mathf.Min(this._constraints.LinearAcceleration, this._intendedLinearAcceleration);
			}
		}

		// Token: 0x170010A9 RID: 4265
		// (get) Token: 0x060060BA RID: 24762 RVA: 0x002D7804 File Offset: 0x002D5A04
		public bool IsCoasting
		{
			get
			{
				return this._drift == null && this._preBurn == null && this._burn == null && this._postBurn == null;
			}
		}

		// Token: 0x170010AA RID: 4266
		// (get) Token: 0x060060BB RID: 24763 RVA: 0x002D7829 File Offset: 0x002D5A29
		// (set) Token: 0x060060BC RID: 24764 RVA: 0x002D7831 File Offset: 0x002D5A31
		public bool IsTrajectoryValid { get; private set; }

		// Token: 0x170010AB RID: 4267
		// (get) Token: 0x060060BD RID: 24765 RVA: 0x002D783A File Offset: 0x002D5A3A
		// (set) Token: 0x060060BE RID: 24766 RVA: 0x002D7842 File Offset: 0x002D5A42
		private IPreviousTrajectory Start { get; set; }

		// Token: 0x170010AC RID: 4268
		// (get) Token: 0x060060BF RID: 24767 RVA: 0x002D784B File Offset: 0x002D5A4B
		// (set) Token: 0x060060C0 RID: 24768 RVA: 0x002D7853 File Offset: 0x002D5A53
		public IPreviousTrajectory End { get; private set; }

		// Token: 0x060060C1 RID: 24769 RVA: 0x002D785C File Offset: 0x002D5A5C
		public bool IsInBurn(TIDateTime time)
		{
			return this.Start.IsInBurn(time);
		}

		// Token: 0x060060C2 RID: 24770 RVA: 0x002D786A File Offset: 0x002D5A6A
		public bool IsAcceleratingRight(TIDateTime time)
		{
			return this.Start.IsAcceleratingRight(time);
		}

		// Token: 0x060060C3 RID: 24771 RVA: 0x002D7878 File Offset: 0x002D5A78
		public bool IsAcceleratingLeft(TIDateTime time)
		{
			return this.Start.IsAcceleratingLeft(time);
		}

		// Token: 0x060060C4 RID: 24772 RVA: 0x002D7886 File Offset: 0x002D5A86
		public bool IsAcceleratingUp(TIDateTime time)
		{
			return this.Start.IsAcceleratingUp(time);
		}

		// Token: 0x060060C5 RID: 24773 RVA: 0x002D7894 File Offset: 0x002D5A94
		public bool IsAcceleratingDown(TIDateTime time)
		{
			return this.Start.IsAcceleratingDown(time);
		}

		// Token: 0x060060C6 RID: 24774 RVA: 0x002D78A2 File Offset: 0x002D5AA2
		public bool IsAcceleratingRollRight(TIDateTime time)
		{
			return this.Start.IsAcceleratingRollRight(time);
		}

		// Token: 0x060060C7 RID: 24775 RVA: 0x002D78B0 File Offset: 0x002D5AB0
		public bool IsAcceleratingRollLeft(TIDateTime time)
		{
			return this.Start.IsAcceleratingRollLeft(time);
		}

		// Token: 0x060060C8 RID: 24776 RVA: 0x002D78BE File Offset: 0x002D5ABE
		public Vector3 PositionAt(TIDateTime time)
		{
			return this.Start.PositionAt(time);
		}

		// Token: 0x060060C9 RID: 24777 RVA: 0x002D78CC File Offset: 0x002D5ACC
		public Vector3 VelocityAt(TIDateTime time)
		{
			return this.Start.VelocityAt(time);
		}

		// Token: 0x060060CA RID: 24778 RVA: 0x002D78DA File Offset: 0x002D5ADA
		public Vector3 AccelerationAt(TIDateTime time)
		{
			return this.Start.AccelerationAt(time);
		}

		// Token: 0x060060CB RID: 24779 RVA: 0x002D78E8 File Offset: 0x002D5AE8
		public float AngularVelocityAt_Rad(TIDateTime time)
		{
			return this.Start.AngularVelocityAt_Rad(time);
		}

		// Token: 0x060060CC RID: 24780 RVA: 0x002D78F6 File Offset: 0x002D5AF6
		public Vector3 HeadingAt(TIDateTime time)
		{
			return this.Start.HeadingAt(time);
		}

		// Token: 0x060060CD RID: 24781 RVA: 0x002D7904 File Offset: 0x002D5B04
		public Quaternion RotationAt(TIDateTime time)
		{
			return this.Start.RotationAt(time);
		}

		// Token: 0x060060CE RID: 24782 RVA: 0x002D7912 File Offset: 0x002D5B12
		public ITrajectory TrajectoryAt(TIDateTime time)
		{
			return this.Start.TrajectoryAt(time);
		}

		// Token: 0x060060CF RID: 24783 RVA: 0x002D7920 File Offset: 0x002D5B20
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			return this.Start.GetBurnTimings();
		}

		// Token: 0x060060D0 RID: 24784 RVA: 0x002D792D File Offset: 0x002D5B2D
		public void UpdatePathNodes(TIDateTime timingCutoff, Camera cam, Vector3 shipPosition)
		{
			IPreviousTrajectory start = this.Start;
			if (start == null)
			{
				return;
			}
			start.UpdatePathNodes(timingCutoff, cam, shipPosition);
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x002D7942 File Offset: 0x002D5B42
		private WaypointTrajectorySequence()
		{
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x002D794C File Offset: 0x002D5B4C
		private WaypointTrajectorySequence(IWaypoint start, IProposedWaypoint target, float desiredDisplacement, AccelerationConstraints constraints, float intendedAcceleration)
		{
			this.IsTrajectoryValid = true;
			this._constraints = constraints;
			this._intendedLinearAcceleration = intendedAcceleration;
			this.Start = new AnchorTrajectory(start);
			IPreviousTrajectory previousTrajectory = this.Start;
			if (target.RotationAllowed)
			{
				if (constraints.AngularAcceleration > 1E-45f)
				{
					previousTrajectory = this.HandlePreBurnRotation(previousTrajectory, target);
				}
				if (intendedAcceleration > 1E-45f)
				{
					previousTrajectory = this.HandleBurn(previousTrajectory, target, desiredDisplacement);
				}
				previousTrajectory = this.HandleHold(previousTrajectory, target);
			}
			else
			{
				previousTrajectory = this.HandlePreBurnDrift(previousTrajectory, target, this.linearAcceleration);
				previousTrajectory = this.HandleHold(previousTrajectory, target);
			}
			this.End = previousTrajectory;
		}

		// Token: 0x060060D3 RID: 24787 RVA: 0x002D79E8 File Offset: 0x002D5BE8
		private WaypointTrajectorySequence(IWaypoint start, IProposedWaypoint target, AccelerationConstraints constraints, float availableTime)
		{
			this.IsTrajectoryValid = true;
			this._constraints = constraints;
			this._intendedLinearAcceleration = constraints.LinearAcceleration;
			this.Start = new AnchorTrajectory(start);
			if (target.RotationAllowed)
			{
				this.End = this.HandlePreBurnRotation(this.Start, target, availableTime, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				return;
			}
			this.End = this.HandlePreBurnDrift(this.Start, target, availableTime, this.linearAcceleration);
		}

		// Token: 0x060060D4 RID: 24788 RVA: 0x002D7A70 File Offset: 0x002D5C70
		private WaypointTrajectorySequence(IWaypoint start, float timingInterval, float targetAlphaBlendValue)
		{
			this.IsTrajectoryValid = true;
			this.Start = new AnchorTrajectory(start);
			this.End = this.HandleHold(this.Start, timingInterval, targetAlphaBlendValue);
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x002D7A9F File Offset: 0x002D5C9F
		public static WaypointTrajectorySequence CreateHoldTrajectory(IWaypoint start, float timingInterval, float targetAlpha)
		{
			return new WaypointTrajectorySequence(start, timingInterval, targetAlpha);
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x002D7AAC File Offset: 0x002D5CAC
		public static WaypointTrajectorySequence CreateConstrainedTrajectory(IWaypoint start, IProposedWaypoint target, AccelerationConstraints constraints, bool preserveRoll = false, bool useMaxThrust = false, float timeRequestedForPostBurn = 0f, float forceAcceleration = -1f)
		{
			if (!WaypointTrajectorySequence.IsManeuveringPossible(constraints))
			{
				return WaypointTrajectorySequence.InvalidTrajectorySequence;
			}
			float num = (float)(target.Timing - start.Timing).TotalSeconds;
			Vector3 vector = PhysicsHelpers.PositionFromVelocityAndTime(start.Position, start.Velocity, num);
			Vector3 vector2 = target.Position - vector;
			float num2 = (target.RotationAllowed ? WaypointTrajectorySequence.TimeRequiredForHeadingRotation(start.Rotation, target.Rotation, constraints.AngularAcceleration, constraints.MaxAngularVelocity) : 0f);
			if (num2 <= num || num2 == 3.4028235E+38f)
			{
				if (num2 == 3.4028235E+38f)
				{
					target.Heading = start.Heading;
				}
				float magnitude = vector2.magnitude;
				float num3 = num - num2 - timeRequestedForPostBurn;
				float num4 = PhysicsHelpers.AccelerationFromDisplacementAndTime(magnitude, num3);
				float num5 = (useMaxThrust ? constraints.LinearAcceleration : ((forceAcceleration >= 0f) ? forceAcceleration : Mathf.Clamp(num4, constraints.CruiseLinearAcceleration, constraints.LinearAcceleration)));
				float num6 = PhysicsHelpers.DisplacementFromAccelerationAndTime(num5, num3);
				if (!WaypointTrajectorySequence.IsTargetReachable(magnitude, num6))
				{
					if (target.IsPositionLocked)
					{
						return WaypointTrajectorySequence.InvalidTrajectorySequence;
					}
					target.Position = vector + vector2.normalized * num6;
				}
				if (target.RotationAllowed && magnitude > 0.01f && constraints.AngularAcceleration > 1E-45f)
				{
					target.SetHeading(vector2.normalized, preserveRoll);
				}
				return new WaypointTrajectorySequence(start, target, magnitude, constraints, num5);
			}
			if (!target.IsPositionLocked)
			{
				target.Position = vector;
				WaypointTrajectorySequence waypointTrajectorySequence = new WaypointTrajectorySequence(start, target, constraints, num);
				waypointTrajectorySequence.AdjustEndHeading(target, constraints, false);
				return waypointTrajectorySequence;
			}
			return WaypointTrajectorySequence.InvalidTrajectorySequence;
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x002D7C3A File Offset: 0x002D5E3A
		private static float TimeRequiredForDrift(Vector3 desiredDisplacementVector, float linearAcceleration)
		{
			return DriftTrajectory.TimeRequiredForDisplacement(desiredDisplacementVector, linearAcceleration, false);
		}

		// Token: 0x060060D8 RID: 24792 RVA: 0x002D7C44 File Offset: 0x002D5E44
		public static float TimeRequiredForHeadingRotation(Quaternion currentRotation, Quaternion requiredRotation, float angularAcceleration, float maxAngularVelocity)
		{
			return RotationTrajectory.TimeRequiredForHeadingRotation(currentRotation, requiredRotation, angularAcceleration, maxAngularVelocity);
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x002D7C4F File Offset: 0x002D5E4F
		private static bool IsManeuveringPossible(AccelerationConstraints constraints)
		{
			return constraints.AngularAcceleration > 0f;
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x002D7C5E File Offset: 0x002D5E5E
		private static bool IsTargetReachable(float requiredDisplacement, float possibleDisplacement)
		{
			return requiredDisplacement <= possibleDisplacement;
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x002D7C67 File Offset: 0x002D5E67
		private IPreviousTrajectory HandlePreBurnDrift(IPreviousTrajectory start, IProposedWaypoint target, float linearAcceleration)
		{
			this._drift = new DriftTrajectory(start, target, linearAcceleration, false);
			return this._drift;
		}

		// Token: 0x060060DC RID: 24796 RVA: 0x002D7C7E File Offset: 0x002D5E7E
		private IPreviousTrajectory HandlePreBurnDrift(IPreviousTrajectory start, IProposedWaypoint target, float availableTime, float linearAcceleration)
		{
			this._drift = new DriftTrajectory(start, target, availableTime, linearAcceleration, false);
			return this._drift;
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x002D7C97 File Offset: 0x002D5E97
		private IPreviousTrajectory HandlePreBurnRotation(IPreviousTrajectory start, IProposedWaypoint target, float availableTime, float angularAcceleration, float maxAngularVelocity)
		{
			this._preBurn = new RotationTrajectory(start, target, availableTime, angularAcceleration, maxAngularVelocity);
			return this._preBurn;
		}

		// Token: 0x060060DE RID: 24798 RVA: 0x002D7CB4 File Offset: 0x002D5EB4
		private IPreviousTrajectory HandlePreBurnRotation(IPreviousTrajectory current, IProposedWaypoint target)
		{
			if (this.IsRotationRequired(current.Rotation, target.Rotation))
			{
				this._preBurn = new RotationTrajectory(current, target, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				return this._preBurn;
			}
			return current;
		}

		// Token: 0x060060DF RID: 24799 RVA: 0x002D7D00 File Offset: 0x002D5F00
		private bool IsRotationRequired(Quaternion currentRotation, Quaternion targetRotation)
		{
			return Math.Abs(PhysicsHelpers.RadianAngleBetweenQuaternions(currentRotation, targetRotation)) > 0f;
		}

		// Token: 0x060060E0 RID: 24800 RVA: 0x002D7D18 File Offset: 0x002D5F18
		private IPreviousTrajectory HandleBurn(IPreviousTrajectory current, IProposedWaypoint target, float desiredDisplacement)
		{
			if (!this.IsRotationRequired(current.Rotation, target.Rotation))
			{
				float num = (float)(target.Timing - current.Timing).TotalSeconds;
				float.IsNaN(num - Mathf.Sqrt(num * num - PhysicsHelpers.TimeSquaredFromDisplacementAndAcceleration(desiredDisplacement, this.linearAcceleration)));
				this._burn = new BurnTrajectory(current, target, desiredDisplacement, this.linearAcceleration);
				this.IsTrajectoryValid = this._burn.IsValidBurnTrajectory;
				return this._burn;
			}
			return current;
		}

		// Token: 0x060060E1 RID: 24801 RVA: 0x002D7DA0 File Offset: 0x002D5FA0
		private IPreviousTrajectory HandleHold(IPreviousTrajectory current, float timingInterval, float targetAlphaBlendValue)
		{
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				AlphaBlendValue = targetAlphaBlendValue,
				Position = current.Position + current.Velocity * timingInterval,
				Velocity = current.Velocity,
				Rotation = current.Rotation,
				Timing = new TIDateTime(current.Timing)
			};
			proposedWaypoint.Timing.AddSeconds((double)timingInterval);
			this._hold = new HoldTrajectory(current, proposedWaypoint);
			return this._hold;
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x002D7E20 File Offset: 0x002D6020
		private IPreviousTrajectory HandleHold(IPreviousTrajectory current, IProposedWaypoint target)
		{
			if (current.Timing != target.Timing)
			{
				this._hold = new HoldTrajectory(current, target);
				return this._hold;
			}
			return current;
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x002D7E4C File Offset: 0x002D604C
		public void AdjustEndHeading(IProposedWaypoint target, AccelerationConstraints constraints, bool useMaxThrust = false)
		{
			this._constraints = constraints;
			bool flag = false;
			IPreviousTrajectory previousTrajectory = this.Start;
			if (this._constraints.AngularAcceleration <= 1E-45f)
			{
				target.Heading = this.Start.Heading;
				this._hold = new HoldTrajectory(this.Start, target);
				this.End = this._hold;
				return;
			}
			if (this._preBurn != null && this._burn == null)
			{
				float num = WaypointTrajectorySequence.TimeRequiredForHeadingRotation(this.Start.Rotation, target.Rotation, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				float num2 = (float)(target.Timing - this.Start.Timing).TotalSeconds;
				if (num > num2 && num != 3.4028235E+38f)
				{
					this.End = this.HandlePreBurnRotation(this.Start, target, num2, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				}
				else
				{
					IPreviousTrajectory previousTrajectory2 = this.HandlePreBurnRotation(this.Start, target);
					previousTrajectory2 = this.HandleHold(previousTrajectory2, target);
					this.End = previousTrajectory2;
				}
			}
			else if (this._burn != null)
			{
				flag = this.IsRotationRequired(this._burn.Rotation, target.Rotation);
				previousTrajectory = this._burn;
				if (!flag && this._postBurn != null)
				{
					this._hold = new HoldTrajectory(this._burn, target);
					this.End = this._hold;
				}
			}
			else
			{
				flag = this.IsRotationRequired(this.End.Rotation, target.Rotation);
				previousTrajectory = this.Start;
			}
			if (flag)
			{
				float num3 = (float)(target.Timing - previousTrajectory.Timing).TotalSeconds;
				float num4 = WaypointTrajectorySequence.TimeRequiredForHeadingRotation(previousTrajectory.Rotation, target.Rotation, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				if (previousTrajectory == this._burn && this._burn != null)
				{
					RotationTrajectory preBurn = this._preBurn;
					TIDateTime tidateTime = ((preBurn != null) ? preBurn.Timing : null) ?? this.Start.Timing;
					float num5 = (float)(target.Timing - tidateTime).TotalSeconds;
					float num6 = num5 - num4;
					Vector3 vector = this._burn.PositionAt(tidateTime);
					Vector3 vector2 = this._burn.VelocityAt(tidateTime);
					Vector3 vector3 = this._burn.PositionAt(target.Timing);
					Vector3 vector4 = vector + vector2 * num5;
					float magnitude = (vector3 - vector4).magnitude;
					float num7 = PhysicsHelpers.AccelerationFromDisplacementTimeAndBurnDuration(magnitude, num5, num6);
					float num8 = (useMaxThrust ? constraints.LinearAcceleration : Mathf.Clamp(num7, constraints.CruiseLinearAcceleration, constraints.LinearAcceleration));
					if (num8 > 1E-45f)
					{
						this._intendedLinearAcceleration = num8;
						ProposedWaypoint proposedWaypoint = new ProposedWaypoint
						{
							Position = target.Position,
							Velocity = target.Velocity,
							Rotation = this._burn.RotationAt(tidateTime),
							Timing = target.Timing
						};
						IPreviousTrajectory preBurn2 = this._preBurn;
						previousTrajectory = this.HandleBurn(preBurn2 ?? this.Start, proposedWaypoint, magnitude);
					}
				}
				this._postBurn = new RotationTrajectory(previousTrajectory, target, num3, this._constraints.AngularAcceleration, this._constraints.MaxAngularVelocity);
				this._hold = new HoldTrajectory(this._postBurn, target);
				this.End = this._hold;
			}
		}

		// Token: 0x0400442F RID: 17455
		private const float TARGET_FADE_DURATION_SECONDS = 10f;

		// Token: 0x04004430 RID: 17456
		private static WaypointTrajectorySequence s_InvalidTrajectorySequence;

		// Token: 0x04004431 RID: 17457
		private AccelerationConstraints _constraints;

		// Token: 0x04004432 RID: 17458
		private float _intendedLinearAcceleration;

		// Token: 0x04004433 RID: 17459
		private DriftTrajectory _drift;

		// Token: 0x04004434 RID: 17460
		private RotationTrajectory _preBurn;

		// Token: 0x04004435 RID: 17461
		private BurnTrajectory _burn;

		// Token: 0x04004436 RID: 17462
		private RotationTrajectory _postBurn;

		// Token: 0x04004437 RID: 17463
		private HoldTrajectory _hold;
	}
}
