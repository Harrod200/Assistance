using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F2 RID: 2546
	public sealed class BurnTrajectory : HoldTrajectory
	{
		// Token: 0x0600606C RID: 24684 RVA: 0x002D5CA8 File Offset: 0x002D3EA8
		public BurnTrajectory(IPreviousTrajectory start, IProposedWaypoint end, float requiredDisplacement, float linearAcceleration)
			: base(start)
		{
			this._pathLineColor = new Color(0.65f, 0.1529f, 0.0509f, 1f);
			this.IsValidBurnTrajectory = true;
			this._linearAcceleration = linearAcceleration;
			float num = (float)(end.Timing - start.Timing).TotalSeconds;
			float num2 = num - Mathf.Sqrt(num * num - PhysicsHelpers.TimeSquaredFromDisplacementAndAcceleration(requiredDisplacement, linearAcceleration));
			bool flag = false;
			if (float.IsNaN(num2))
			{
				num2 = num;
				flag = true;
			}
			Vector3 vector = PhysicsHelpers.DisplacementFromVelocityAndTime(start.Velocity, num2);
			Vector3 vector2 = PhysicsHelpers.DisplacementFromAccelerationAndTime(start.Heading, linearAcceleration, num2);
			Vector3 vector3 = PhysicsHelpers.VelocityFromAccelerationAndTime(start.Heading, linearAcceleration, num2);
			this.SetData(start);
			base.Timing.AddSeconds((double)num2);
			base.Position += vector + vector2;
			base.Velocity += vector3;
			base.Rotation = this._previousWaypoint.Rotation;
			if (flag)
			{
				end.Position = base.Position;
				if (end.IsPositionLocked)
				{
					this.IsValidBurnTrajectory = false;
				}
			}
			base.AlphaBlendValue = Mathf.Lerp(start.AlphaBlendValue, end.AlphaBlendValue, num2 / num);
			this._alphaRange.x = start.AlphaBlendValue;
			this._alphaRange.y = base.AlphaBlendValue;
			this._totalDuration_s = (float)(base.Timing - start.Timing).TotalSeconds;
			this._mainCamera = Camera.main;
			base.InitializePathList();
		}

		// Token: 0x0600606D RID: 24685 RVA: 0x002D5E37 File Offset: 0x002D4037
		public override bool IsInBurn(TIDateTime time)
		{
			if (!(time < base.Timing))
			{
				ITrajectory nextTrajectory = this._nextTrajectory;
				return nextTrajectory != null && nextTrajectory.IsInBurn(time);
			}
			return true;
		}

		// Token: 0x0600606E RID: 24686 RVA: 0x002D5E5B File Offset: 0x002D405B
		public static float TimeRequiredForDisplacement(Vector3 desiredDisplacementVector, float linearAcceleration)
		{
			return PhysicsHelpers.TimeFromDisplacementAndAcceleration(desiredDisplacementVector.magnitude, linearAcceleration);
		}

		// Token: 0x0600606F RID: 24687 RVA: 0x002D5E6C File Offset: 0x002D406C
		protected override Vector3 PositionAt(float elapsedTime)
		{
			Vector3 vector = PhysicsHelpers.DisplacementFromVelocityAndTime(this._previousWaypoint.Velocity, elapsedTime);
			Vector3 vector2 = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._previousWaypoint.Heading, this._linearAcceleration, elapsedTime);
			return this._previousWaypoint.Position + vector + vector2;
		}

		// Token: 0x06006070 RID: 24688 RVA: 0x002D5EBA File Offset: 0x002D40BA
		protected override Vector3 VelocityAt(float elapsedTime)
		{
			return this._previousWaypoint.Velocity + PhysicsHelpers.VelocityFromAccelerationAndTime(this._previousWaypoint.Heading, this._linearAcceleration, elapsedTime);
		}

		// Token: 0x06006071 RID: 24689 RVA: 0x002D5EE3 File Offset: 0x002D40E3
		protected override Vector3 AccelerationAt(float elapsedTime)
		{
			return this._previousWaypoint.Heading * this._linearAcceleration;
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x002D5EFC File Offset: 0x002D40FC
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		public override List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			if (this._previousWaypoint.Timing == base.Timing)
			{
				if (this._nextTrajectory == null)
				{
					return new List<ValueTuple<TIDateTime, bool>>();
				}
				return this._nextTrajectory.GetBurnTimings();
			}
			else
			{
				if (this._nextTrajectory == null)
				{
					return new List<ValueTuple<TIDateTime, bool>>
					{
						new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, true),
						new ValueTuple<TIDateTime, bool>(base.Timing, false)
					};
				}
				List<ValueTuple<TIDateTime, bool>> burnTimings = this._nextTrajectory.GetBurnTimings();
				burnTimings.Insert(0, new ValueTuple<TIDateTime, bool>(this._previousWaypoint.Timing, true));
				return burnTimings;
			}
		}

		// Token: 0x0400441A RID: 17434
		private float _linearAcceleration;

		// Token: 0x0400441B RID: 17435
		public bool IsValidBurnTrajectory;
	}
}
