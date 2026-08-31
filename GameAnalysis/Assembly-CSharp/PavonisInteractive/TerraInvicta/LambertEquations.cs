using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000791 RID: 1937
	public struct LambertEquations
	{
		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06003DC0 RID: 15808 RVA: 0x001849B0 File Offset: 0x00182BB0
		// (set) Token: 0x06003DC1 RID: 15809 RVA: 0x001849B8 File Offset: 0x00182BB8
		public Vector3d initialVelocity { readonly get; private set; }

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x06003DC2 RID: 15810 RVA: 0x001849C1 File Offset: 0x00182BC1
		// (set) Token: 0x06003DC3 RID: 15811 RVA: 0x001849C9 File Offset: 0x00182BC9
		public Vector3d finalVelocity { readonly get; private set; }

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06003DC4 RID: 15812 RVA: 0x001849D2 File Offset: 0x00182BD2
		// (set) Token: 0x06003DC5 RID: 15813 RVA: 0x001849DA File Offset: 0x00182BDA
		public Vector3d burn0 { readonly get; private set; }

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06003DC6 RID: 15814 RVA: 0x001849E3 File Offset: 0x00182BE3
		// (set) Token: 0x06003DC7 RID: 15815 RVA: 0x001849EB File Offset: 0x00182BEB
		public Vector3d burn1 { readonly get; private set; }

		// Token: 0x06003DC8 RID: 15816 RVA: 0x001849F4 File Offset: 0x00182BF4
		public double SolveLambert(double TransitTimeSeconds, CartesianState InitialState, CartesianState EndState, double barycenterMu, bool bRetrograde = false, bool bFastPass = false)
		{
			Vector3d position = InitialState.position;
			Vector3d position2 = EndState.position;
			double magnitude = position.magnitude;
			double magnitude2 = position2.magnitude;
			double num = Mathd.Sqrt(magnitude * magnitude + magnitude2 * magnitude2 - 2.0 * Vector3d.Dot(in position, in position2));
			double num2 = (num + magnitude + magnitude2) / 2.0;
			Vector3d normalized = position.normalized;
			Vector3d normalized2 = position2.normalized;
			Vector3d vector3d = Vector3d.Cross(normalized, normalized2).normalized;
			if (vector3d.sqrMagnitude < 0.5)
			{
				Vector3d normalized3 = Vector3d.Cross(InitialState.position, InitialState.velocity).normalized;
				Vector3d normalized4 = Vector3d.Cross(EndState.position, EndState.velocity).normalized;
				vector3d = (normalized3 + normalized4).normalized;
			}
			this.lambda2 = Mathd.Max(1.0 - num / num2, 0.0);
			this.lambda = Mathd.Sqrt(this.lambda2);
			Vector3d vector3d2;
			Vector3d vector3d3;
			if (vector3d.z < 0.0)
			{
				this.lambda = -this.lambda;
				vector3d2 = Vector3d.Cross(normalized, vector3d);
				vector3d3 = Vector3d.Cross(normalized2, vector3d);
			}
			else
			{
				vector3d2 = Vector3d.Cross(vector3d, normalized);
				vector3d3 = Vector3d.Cross(vector3d, normalized2);
			}
			if (bRetrograde)
			{
				this.lambda = -this.lambda;
				vector3d2 = -vector3d2;
				vector3d3 = -vector3d3;
			}
			this.lambda3 = this.lambda * this.lambda2;
			double num3 = Mathd.Sqrt(2.0 * barycenterMu / Mathd.Pow(num2, 3.0)) * TransitTimeSeconds;
			double num4 = Mathd.Acos(this.lambda) + this.lambda * Mathd.Sqrt(1.0 - this.lambda2);
			double num5 = 0.6666666666666666 * (1.0 - this.lambda3);
			double num6;
			if (num3 >= num4)
			{
				num6 = -(num3 - num4) / (num3 - num4 + 4.0);
			}
			else if (num3 <= num5)
			{
				num6 = num5 * (num5 - num3) / (0.4 * (1.0 - this.lambda2 * this.lambda3) * num3) + 1.0;
			}
			else
			{
				num6 = Mathd.Pow(num3 / num4, 0.6931471805599453 / Mathd.Log(num5 / num4)) - 1.0;
			}
			double num7 = this.householder(num3, num6, 1E-11, 15);
			if (double.IsNaN(num7) || double.IsInfinity(num7))
			{
				Log.Warn("Lambert solver householder failed: trajectory is nearly parabolic.", Array.Empty<object>());
				num7 = num6;
			}
			double num8 = Mathd.Sqrt(barycenterMu * num2 / 2.0);
			double num9 = (magnitude - magnitude2) / num;
			double num10 = Mathd.Sqrt(1.0 - num9 * num9);
			double num11 = Mathd.Sqrt(1.0 - this.lambda2 + this.lambda2 * num7 * num7);
			double num12 = num8 * (this.lambda * num11 - num7 - num9 * (this.lambda * num11 + num7)) / magnitude;
			double num13 = -num8 * (this.lambda * num11 - num7 + num9 * (this.lambda * num11 + num7)) / magnitude2;
			double num14 = num8 * num10 * (num11 + this.lambda * num7);
			double num15 = num14 / magnitude;
			double num16 = num14 / magnitude2;
			this.initialVelocity = num12 * normalized + num15 * vector3d2;
			this.finalVelocity = num13 * normalized2 + num16 * vector3d3;
			this.burn0 = this.initialVelocity - InitialState.velocity;
			this.burn1 = EndState.velocity - this.finalVelocity;
			return this.burn0.magnitude + this.burn1.magnitude;
		}

		// Token: 0x06003DC9 RID: 15817 RVA: 0x00184DFC File Offset: 0x00182FFC
		private double householder(double T, double x0, double allowedError, int maxIterations)
		{
			int num = 0;
			double num2 = 1.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			while (num2 > allowedError && num < maxIterations)
			{
				double num6 = this.xToTimeOfTransit(x0);
				this.dTdx(ref num3, ref num4, ref num5, x0, num6);
				double num7 = num6 - T;
				double num8 = num3 * num3;
				double num9 = x0 - num7 * (num8 - num7 * num4 / 2.0) / (num3 * (num8 - num7 * num4) + num5 * num7 * num7 / 6.0);
				num2 = Mathd.Abs(x0 - num9);
				x0 = num9;
				num++;
			}
			return x0;
		}

		// Token: 0x06003DCA RID: 15818 RVA: 0x00184EAC File Offset: 0x001830AC
		private double xToTimeOfTransit(double x)
		{
			double num = 1.0 / (1.0 - Mathd.Pow(x, 2.0));
			if (num > 0.0)
			{
				double num2 = 2.0 * Mathd.Acos(x);
				double num3 = 2.0 * Mathd.Asin(Mathd.Sqrt(this.lambda2 / num));
				if (this.lambda < 0.0)
				{
					num3 = -num3;
				}
				return num * Mathd.Sqrt(num) * (num2 - Mathd.Sin(num2) - (num3 - Mathd.Sin(num3))) / 2.0;
			}
			double num4 = 2.0 * Mathd.ACosh(x);
			double num5 = 2.0 * Mathd.ASinh(Mathd.Sqrt(-this.lambda2 / num));
			if (this.lambda < 0.0)
			{
				num5 = -num5;
			}
			return -num * Mathd.Sqrt(-num) * (num5 - Mathd.Sinh(num5) - (num4 - Mathd.Sinh(num4))) / 2.0;
		}

		// Token: 0x06003DCB RID: 15819 RVA: 0x00184FC0 File Offset: 0x001831C0
		private double xToTimeOfTransit_Lagrange(double x)
		{
			double num = 1.0 / (1.0 - Mathd.Pow(x, 2.0));
			if (num > 0.0)
			{
				double num2 = 2.0 * Mathd.Acos(x);
				double num3 = 2.0 * Mathd.Asin(Mathd.Sqrt(this.lambda2 / num));
				if (this.lambda < 0.0)
				{
					num3 = -num3;
				}
				return num * Mathd.Sqrt(num) * (num2 - Mathd.Sin(num2) - (num3 - Mathd.Sin(num3))) / 2.0;
			}
			double num4 = 2.0 * Mathd.ACosh(x);
			double num5 = 2.0 * Mathd.ASinh(Mathd.Sqrt(-this.lambda2 / num));
			if (this.lambda < 0.0)
			{
				num5 = -num5;
			}
			return -num * Mathd.Sqrt(-num) * (num5 - Mathd.Sinh(num5) - (num4 - Mathd.Sinh(num4))) / 2.0;
		}

		// Token: 0x06003DCC RID: 15820 RVA: 0x001850D4 File Offset: 0x001832D4
		private double xToTimeOfTransit_Lancaster(double x)
		{
			double num = Mathd.Pow(x, 2.0) - 1.0;
			double num2 = Mathd.Abs(num);
			double num3 = Mathd.Sqrt(1.0 + this.lambda2 * num);
			double num4 = Mathd.Sqrt(num2);
			double num5 = x * num3 - this.lambda;
			double num6;
			if (num < 0.0)
			{
				num6 = Mathd.Acos(num5);
			}
			else
			{
				num6 = Mathd.Log(num4 * (num3 - this.lambda * x) + num5);
			}
			return (x - this.lambda * num3 - num6 / num4) / num;
		}

		// Token: 0x06003DCD RID: 15821 RVA: 0x00185168 File Offset: 0x00183368
		private double xToTimeOfTransit_Battin(double x)
		{
			double num = Mathd.Pow(x, 2.0) - 1.0;
			double num2 = Mathd.Sqrt(1.0 + this.lambda2 * num) - this.lambda * x;
			double num3 = (1.0 - this.lambda - x * num2) / 2.0;
			double num4 = this.hypergeometricF(num3, 1E-11);
			num4 = 1.3333333333333333 * num4;
			return (Mathd.Pow(num2, 3.0) * num4 + 4.0 * this.lambda * num2) / 2.0;
		}

		// Token: 0x06003DCE RID: 15822 RVA: 0x0018521C File Offset: 0x0018341C
		private double hypergeometricF(double z, double tol)
		{
			double num = 1.0;
			double num2 = 1.0;
			double num3 = 1.0;
			int num4 = 0;
			while (num3 > tol)
			{
				double num5 = num2 * (3.0 + (double)num4) * (1.0 + (double)num4) / (2.5 + (double)num4) * z / ((double)num4 + 1.0);
				double num6 = num + num5;
				num3 = Mathd.Abs(num5);
				num = num6;
				num2 = num5;
				num4++;
			}
			return num;
		}

		// Token: 0x06003DCF RID: 15823 RVA: 0x001852AC File Offset: 0x001834AC
		private void dTdx(ref double DT, ref double DDT, ref double DDDT, double x, double T)
		{
			double num = 1.0 - x * x;
			double num2 = 1.0 - this.lambda2 * num;
			double num3 = Mathd.Sqrt(num2);
			double num4 = num2 * num3;
			DT = 1.0 / num * (3.0 * T * x - 2.0 + 2.0 * this.lambda3 * x / num3);
			DDT = 1.0 / num * (3.0 * T + 5.0 * x * DT + 2.0 * (1.0 - this.lambda2) * this.lambda3 / num4);
			DDDT = 1.0 / num * (7.0 * x * DDT + 8.0 * DT - 6.0 * (1.0 - this.lambda2) * this.lambda2 * this.lambda3 * x / num4 / num2);
		}

		// Token: 0x040026A9 RID: 9897
		private double lambda;

		// Token: 0x040026AA RID: 9898
		private double lambda2;

		// Token: 0x040026AB RID: 9899
		private double lambda3;
	}
}
