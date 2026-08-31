using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009E4 RID: 2532
	public static class PhysicsHelpers
	{
		// Token: 0x06005FAC RID: 24492 RVA: 0x002D4AC5 File Offset: 0x002D2CC5
		public static Vector3 DisplacementFromAccelerationAndTime(Vector3 heading, float a, float t)
		{
			return heading * PhysicsHelpers.DisplacementFromAccelerationAndTime(a, t);
		}

		// Token: 0x06005FAD RID: 24493 RVA: 0x002D4AD4 File Offset: 0x002D2CD4
		public static float DisplacementFromVelocityAccelerationAndTime(float v, float a, float t)
		{
			return PhysicsHelpers.DisplacementFromVelocityAndTime(v, t) + PhysicsHelpers.DisplacementFromAccelerationAndTime(a, t);
		}

		// Token: 0x06005FAE RID: 24494 RVA: 0x002D4AE5 File Offset: 0x002D2CE5
		public static float DisplacementFromAccelerationAndTime(float a, float t)
		{
			return 0.5f * a * t * t;
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x002D4AF2 File Offset: 0x002D2CF2
		public static float TimeFromDisplacementAndAcceleration(float d, float a)
		{
			return (float)Math.Sqrt((double)PhysicsHelpers.TimeSquaredFromDisplacementAndAcceleration(d, a));
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x002D4B02 File Offset: 0x002D2D02
		public static float TimeSquaredFromDisplacementAndAcceleration(float d, float a)
		{
			if (a == 0f)
			{
				return float.PositiveInfinity;
			}
			return 2f * d / a;
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x002D4B1B File Offset: 0x002D2D1B
		public static float AccelerationFromDisplacementAndTime(float d, float t)
		{
			if (t <= 0f)
			{
				return float.PositiveInfinity;
			}
			return 2f * d / (t * t);
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x002D4B36 File Offset: 0x002D2D36
		public static float AccelerationFromDisplacementTimeAndBurnDuration(float d, float t, float b)
		{
			if (b <= 0f)
			{
				return float.PositiveInfinity;
			}
			return -d / (b * (0.5f * b - t));
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x002D4B54 File Offset: 0x002D2D54
		public static float DisplacementFromVelocityAndTime(float v, float t)
		{
			return v * t;
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x002D4B59 File Offset: 0x002D2D59
		public static Vector3 DisplacementFromVelocityAndTime(Vector3 v, float t)
		{
			return v * t;
		}

		// Token: 0x06005FB5 RID: 24501 RVA: 0x002D4B62 File Offset: 0x002D2D62
		public static Vector3 PositionFromVelocityAndTime(Vector3 pos, Vector3 v, float t)
		{
			return pos + PhysicsHelpers.DisplacementFromVelocityAndTime(v, t);
		}

		// Token: 0x06005FB6 RID: 24502 RVA: 0x002D4B71 File Offset: 0x002D2D71
		public static Vector3 VelocityFromAccelerationAndTime(Vector3 heading, float a, float t)
		{
			return heading * PhysicsHelpers.VelocityFromAccelerationAndTime(a, t);
		}

		// Token: 0x06005FB7 RID: 24503 RVA: 0x002D4B80 File Offset: 0x002D2D80
		public static float VelocityFromAccelerationAndTime(float a, float t)
		{
			return a * t;
		}

		// Token: 0x06005FB8 RID: 24504 RVA: 0x002D4B85 File Offset: 0x002D2D85
		public static float RadianAngleBetweenQuaternions(Quaternion q1, Quaternion q2)
		{
			return Quaternion.Angle(q1, q2) * 0.017453292f;
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x002D4B94 File Offset: 0x002D2D94
		public static float RadianAngleBetweenVectors(Vector3 v1, Vector3 v2)
		{
			return Vector3.Angle(v1, v2) * 0.017453292f;
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x002D4BA3 File Offset: 0x002D2DA3
		public static Vector3 RotateVectorAroundAxis(Vector3 v, Vector3 a, float d)
		{
			return Quaternion.AngleAxis(d, a) * v;
		}
	}
}
