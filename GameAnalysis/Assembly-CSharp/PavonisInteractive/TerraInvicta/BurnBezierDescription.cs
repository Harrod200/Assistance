using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007CB RID: 1995
	public class BurnBezierDescription
	{
		// Token: 0x0600478C RID: 18316 RVA: 0x001D3387 File Offset: 0x001D1587
		public BurnBezierDescription()
		{
		}

		// Token: 0x0600478D RID: 18317 RVA: 0x001D3390 File Offset: 0x001D1590
		public BurnBezierDescription(CartesianState startState, CartesianState endState, double duration_s)
		{
			this.startPosition = startState.positionDisplay;
			this.endPosition = endState.positionDisplay;
			this.startVelocityControlPoint = startState.positionDisplay + startState.velocityDisplay * duration_s / 3.0;
			this.endVelocityControlPoint = endState.positionDisplay - endState.velocityDisplay * duration_s / 3.0;
		}

		// Token: 0x0600478E RID: 18318 RVA: 0x001D3418 File Offset: 0x001D1618
		public Vector3d LocationInBurn(double timeInBurn, double totalBurnDuration)
		{
			double num = timeInBurn / totalBurnDuration;
			double num2 = 1.0 - num;
			return 1.0 * num2 * num2 * num2 * this.startPosition + 3.0 * num * num2 * num2 * this.startVelocityControlPoint + 3.0 * num * num * num2 * this.endVelocityControlPoint + 1.0 * num * num * num * this.endPosition;
		}

		// Token: 0x0600478F RID: 18319 RVA: 0x001D34AC File Offset: 0x001D16AC
		public Vector3d VelocityInBurn(double timeInBurn, double totalBurnDuration)
		{
			double num = timeInBurn / totalBurnDuration;
			return (-3.0 * (1.0 - num) * (1.0 - num) * this.startPosition + 3.0 * (num - 1.0) * (3.0 * num - 1.0) * this.startVelocityControlPoint + 3.0 * (2.0 - 3.0 * num) * num * this.endVelocityControlPoint + 3.0 * num * num * this.endPosition) / totalBurnDuration;
		}

		// Token: 0x06004790 RID: 18320 RVA: 0x001D3578 File Offset: 0x001D1778
		public double MaxAccelerationDuringBurn_mps2(double burnDuration_s)
		{
			return Mathd.Max(this.InitialAcceleration(burnDuration_s), this.FinalAcceleration(burnDuration_s));
		}

		// Token: 0x06004791 RID: 18321 RVA: 0x001D3590 File Offset: 0x001D1790
		public double InitialAcceleration(double burnDuration_s)
		{
			return 6.0 * (this.startPosition - 2.0 * this.startVelocityControlPoint + this.endVelocityControlPoint).magnitude / (burnDuration_s * burnDuration_s);
		}

		// Token: 0x06004792 RID: 18322 RVA: 0x001D35E0 File Offset: 0x001D17E0
		public double FinalAcceleration(double burnDuration_s)
		{
			return 6.0 * (this.startVelocityControlPoint - 2.0 * this.endVelocityControlPoint + this.endPosition).magnitude / (burnDuration_s * burnDuration_s);
		}

		// Token: 0x06004793 RID: 18323 RVA: 0x001D3630 File Offset: 0x001D1830
		public string deepDump()
		{
			return string.Concat(new string[]
			{
				"    Burn Bezier Description:\n     startPosition             = ",
				this.startPosition.ToString(),
				"\n     startVelocityControlPoint = ",
				this.startVelocityControlPoint.ToString(),
				"\n     endVelocityControlPoint   = ",
				this.endVelocityControlPoint.ToString(),
				"\n     endPosition               = ",
				this.endPosition.ToString(),
				"\n"
			});
		}

		// Token: 0x0400297A RID: 10618
		public Vector3d startPosition;

		// Token: 0x0400297B RID: 10619
		public Vector3d endPosition;

		// Token: 0x0400297C RID: 10620
		public Vector3d startVelocityControlPoint;

		// Token: 0x0400297D RID: 10621
		public Vector3d endVelocityControlPoint;
	}
}
