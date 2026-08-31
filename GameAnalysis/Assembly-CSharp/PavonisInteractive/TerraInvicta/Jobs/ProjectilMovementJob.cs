using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

namespace PavonisInteractive.TerraInvicta.Jobs
{
	// Token: 0x02000989 RID: 2441
	[BurstCompile]
	public struct ProjectilMovementJob : IJobParallelForTransform
	{
		// Token: 0x06005CC7 RID: 23751 RVA: 0x002C3570 File Offset: 0x002C1770
		public void Execute(int index, TransformAccess transform)
		{
			ProjectileJobData.MovementType movement = this._projectileData[index].Movement;
			if (movement == ProjectileJobData.MovementType.Ballistic)
			{
				this.BallisticMovement(index, transform);
				return;
			}
			if (movement != ProjectileJobData.MovementType.Missile)
			{
				return;
			}
			this.MissileMovement(index, transform);
		}

		// Token: 0x06005CC8 RID: 23752 RVA: 0x002C35A8 File Offset: 0x002C17A8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void BallisticMovement(int index, TransformAccess transform)
		{
			ProjectileJobData projectileJobData = this._projectileData[index];
			global::UnityEngine.Vector3 vector = new global::UnityEngine.Vector3(projectileJobData.VelocityVector.X, projectileJobData.VelocityVector.Y, projectileJobData.VelocityVector.Z) * this.ElapsedTimeSinceLastUpdate;
			transform.position += vector;
		}

		// Token: 0x06005CC9 RID: 23753 RVA: 0x002C3608 File Offset: 0x002C1808
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MissileMovement(int index, TransformAccess transform)
		{
			ProjectileJobData projectileJobData = this._projectileData[index];
			if (projectileJobData.CurrentAcceleration < projectileJobData.MaxAcceleration)
			{
				float num = this.ElapsedTimeSinceLastUpdate * projectileJobData.MaxAcceleration / projectileJobData.ThrustRamp_s;
				projectileJobData.CurrentAcceleration = ProjectilMovementJob.Min(projectileJobData.CurrentAcceleration + num, projectileJobData.MaxAcceleration);
			}
			if (projectileJobData.TurnRate < projectileJobData.MaxTurnRate_deg)
			{
				float num2 = this.ElapsedTimeSinceLastUpdate * projectileJobData.MaxTurnRate_deg / projectileJobData.TurnRamp_s;
				projectileJobData.TurnRate = ProjectilMovementJob.Min(projectileJobData.TurnRate + num2, projectileJobData.MaxTurnRate_deg);
			}
			global::System.Numerics.Vector3 vector = ProjectilMovementJob.zeroVector;
			global::System.Numerics.Vector3 vector2 = new global::System.Numerics.Vector3(transform.position.x, transform.position.y, transform.position.z);
			if (projectileJobData.CurrentDv > 0f)
			{
				global::System.Numerics.Vector3 vector3 = projectileJobData.TargetPosition - vector2;
				global::System.Numerics.Vector3 vector4 = projectileJobData.TargetVelocity - projectileJobData.VelocityVector;
				global::System.Numerics.Vector3 vector5 = global::System.Numerics.Vector3.Cross(vector3, vector4) / vector3.LengthSquared();
				global::System.Numerics.Vector3 vector6 = global::System.Numerics.Vector3.Cross(4f * vector4, vector5);
				vector6 = ProjectilMovementJob.ClampMagnitude(vector6, projectileJobData.CurrentAcceleration);
				float num3 = global::System.Numerics.Vector3.Dot(vector4, global::System.Numerics.Vector3.Normalize(vector3));
				float num4 = 1f;
				if (projectileJobData.CurrentAcceleration > 0f)
				{
					float num5 = vector6.Length() / projectileJobData.CurrentAcceleration;
					float num6 = 1f - num5 * num5;
					num4 = ((num6 > 1E-07f) ? ((float)Math.Sqrt((double)num6)) : 0f);
					if (num4 < 0f)
					{
						num4 = 0f;
					}
				}
				bool flag = ProjectilMovementJob.Distance(vector2, projectileJobData.TargetPosition) / ProjectilMovementJob.Abs(num3) < projectileJobData.CurrentDv * this.ScalingFactor / projectileJobData.MaxAcceleration;
				global::System.Numerics.Vector3 vector7;
				if (num3 > 0f || flag || ProjectilMovementJob.Abs(num3) <= projectileJobData.CurrentDv * this.ScalingFactor * projectileJobData.ManeuverParameter)
				{
					vector7 = global::System.Numerics.Vector3.Normalize(vector3) * projectileJobData.CurrentAcceleration * num4;
				}
				else
				{
					vector7 = ProjectilMovementJob.zeroVector;
				}
				projectileJobData.AccelerationVector = vector7 + vector6;
				projectileJobData.thrustFraction = projectileJobData.AccelerationVector.Length() / projectileJobData.MaxAcceleration;
				vector = projectileJobData.AccelerationVector * this.ElapsedTimeSinceLastUpdate;
				global::UnityEngine.Vector3 vector8 = new global::UnityEngine.Vector3(vector.X, vector.Y, vector.Z);
				if (vector8.sqrMagnitude > 1E-07f)
				{
					global::UnityEngine.Quaternion quaternion = global::UnityEngine.Quaternion.LookRotation(vector8, transform.rotation * global::UnityEngine.Vector3.up);
					transform.rotation = global::UnityEngine.Quaternion.RotateTowards(transform.rotation, quaternion, this.ElapsedTimeSinceLastUpdate * projectileJobData.TurnRate);
				}
				projectileJobData.VelocityVector += vector;
				projectileJobData.Cumulative_line_of_sight_error = 0f;
			}
			else
			{
				projectileJobData.CurrentAcceleration = 0f;
				projectileJobData.AccelerationVector = ProjectilMovementJob.zeroVector;
			}
			global::System.Numerics.Vector3 vector9 = projectileJobData.VelocityVector * this.ElapsedTimeSinceLastUpdate;
			transform.position += new global::UnityEngine.Vector3(vector9.X, vector9.Y, vector9.Z);
			projectileJobData.CurrentDv -= ProjectilMovementJob.Magnitude(vector) / this.ScalingFactor;
			this._projectileData[index] = projectileJobData;
		}

		// Token: 0x06005CCA RID: 23754 RVA: 0x002C3961 File Offset: 0x002C1B61
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float Min(float f1, float f2)
		{
			if (f1 >= f2)
			{
				return f2;
			}
			return f1;
		}

		// Token: 0x06005CCB RID: 23755 RVA: 0x002C396A File Offset: 0x002C1B6A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float Abs(float f)
		{
			if (f <= 0f)
			{
				return f * -1f;
			}
			return f;
		}

		// Token: 0x06005CCC RID: 23756 RVA: 0x002C3980 File Offset: 0x002C1B80
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static global::System.Numerics.Vector3 Project(global::System.Numerics.Vector3 vector, global::System.Numerics.Vector3 onNormal)
		{
			float num = ProjectilMovementJob.Dot(onNormal, onNormal);
			if (num < 1E-05f)
			{
				return ProjectilMovementJob.zeroVector;
			}
			float num2 = ProjectilMovementJob.Dot(vector, onNormal);
			return new global::System.Numerics.Vector3(onNormal.X * num2 / num, onNormal.Y * num2 / num, onNormal.Z * num2 / num);
		}

		// Token: 0x06005CCD RID: 23757 RVA: 0x002C39D0 File Offset: 0x002C1BD0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static global::System.Numerics.Vector3 ClampMagnitude(global::System.Numerics.Vector3 vector, float maxLength)
		{
			float num = ProjectilMovementJob.SqrMagnitude(vector);
			if (num > maxLength * maxLength)
			{
				float num2 = (float)Math.Sqrt((double)num);
				float num3 = vector.X / num2;
				float num4 = vector.Y / num2;
				float num5 = vector.Z / num2;
				return new global::System.Numerics.Vector3(num3 * maxLength, num4 * maxLength, num5 * maxLength);
			}
			return vector;
		}

		// Token: 0x06005CCE RID: 23758 RVA: 0x002C3A1C File Offset: 0x002C1C1C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Dot(global::System.Numerics.Vector3 lhs, global::System.Numerics.Vector3 rhs)
		{
			return lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
		}

		// Token: 0x06005CCF RID: 23759 RVA: 0x002C3A47 File Offset: 0x002C1C47
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float SqrMagnitude(global::System.Numerics.Vector3 vector)
		{
			return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
		}

		// Token: 0x06005CD0 RID: 23760 RVA: 0x002C3A72 File Offset: 0x002C1C72
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Magnitude(global::System.Numerics.Vector3 vector)
		{
			return (float)Math.Sqrt((double)(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z));
		}

		// Token: 0x06005CD1 RID: 23761 RVA: 0x002C3AA4 File Offset: 0x002C1CA4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float Distance(global::System.Numerics.Vector3 l, global::System.Numerics.Vector3 r)
		{
			return ProjectilMovementJob.Magnitude(l - r);
		}

		// Token: 0x06005CD2 RID: 23762 RVA: 0x002C3AB4 File Offset: 0x002C1CB4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static global::System.Numerics.Vector3 Normalize(global::System.Numerics.Vector3 value)
		{
			float num = ProjectilMovementJob.Magnitude(value);
			if (num > 1E-05f)
			{
				return value / num;
			}
			return ProjectilMovementJob.zeroVector;
		}

		// Token: 0x04004224 RID: 16932
		private const float EPSILON = 1E-07f;

		// Token: 0x04004225 RID: 16933
		private const float DEG_TO_RADS = 0.017453292f;

		// Token: 0x04004226 RID: 16934
		private static readonly global::System.Numerics.Vector3 zeroVector = new global::System.Numerics.Vector3(0f, 0f, 0f);

		// Token: 0x04004227 RID: 16935
		public float ElapsedTimeSinceLastUpdate;

		// Token: 0x04004228 RID: 16936
		[ReadOnly]
		public float ScalingFactor;

		// Token: 0x04004229 RID: 16937
		public NativeArray<ProjectileJobData> _projectileData;

		// Token: 0x0400422A RID: 16938
		private const float _navigation_Constant = 4f;
	}
}
