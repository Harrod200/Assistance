using System;
using System.Numerics;

namespace PavonisInteractive.TerraInvicta.Jobs
{
	// Token: 0x02000987 RID: 2439
	public struct ProjectileJobData
	{
		// Token: 0x0400420D RID: 16909
		public ProjectileJobData.MovementType Movement;

		// Token: 0x0400420E RID: 16910
		public float MaxAcceleration;

		// Token: 0x0400420F RID: 16911
		public float TurnRate;

		// Token: 0x04004210 RID: 16912
		public float ManeuverAngle_rad;

		// Token: 0x04004211 RID: 16913
		public float ManeuverParameter;

		// Token: 0x04004212 RID: 16914
		public float CurrentDv;

		// Token: 0x04004213 RID: 16915
		public float ElapseTime;

		// Token: 0x04004214 RID: 16916
		public float TerminalVelocity;

		// Token: 0x04004215 RID: 16917
		public float CurrentAcceleration;

		// Token: 0x04004216 RID: 16918
		public float Cumulative_line_of_sight_error;

		// Token: 0x04004217 RID: 16919
		public float thrustFraction;

		// Token: 0x04004218 RID: 16920
		public float MaxTurnRate_deg;

		// Token: 0x04004219 RID: 16921
		public float ThrustRamp_s;

		// Token: 0x0400421A RID: 16922
		public float TurnRamp_s;

		// Token: 0x0400421B RID: 16923
		public Vector3 AccelerationVector;

		// Token: 0x0400421C RID: 16924
		public Vector3 VelocityVector;

		// Token: 0x0400421D RID: 16925
		public Vector3 LaunchVelocity;

		// Token: 0x0400421E RID: 16926
		public Vector3 OriginPosition;

		// Token: 0x0400421F RID: 16927
		public Vector3 TargetPosition;

		// Token: 0x04004220 RID: 16928
		public Vector3 TargetVelocity;

		// Token: 0x0200133B RID: 4923
		public enum MovementType
		{
			// Token: 0x04006F78 RID: 28536
			Ballistic,
			// Token: 0x04006F79 RID: 28537
			Missile
		}
	}
}
