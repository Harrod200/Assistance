using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using Unity.Burst;

namespace UnityEngine
{
	// Token: 0x020004F0 RID: 1264
	[BurstCompile]
	public struct Mathd
	{
		// Token: 0x1700044C RID: 1100
		// (get) Token: 0x06001E3F RID: 7743 RVA: 0x0009EAFD File Offset: 0x0009CCFD
		public static double G
		{
			get
			{
				return 6.67384E-11;
			}
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x0009EB08 File Offset: 0x0009CD08
		[BurstCompile]
		public static double Sinh(double d)
		{
			return Math.Sinh(d);
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x0009EB10 File Offset: 0x0009CD10
		[BurstCompile]
		public static double Cosh(double d)
		{
			return Math.Cosh(d);
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x0009EB18 File Offset: 0x0009CD18
		[BurstCompile]
		public static double Tanh(double x)
		{
			return Math.Sinh(x) / Math.Cosh(x);
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x0009EB27 File Offset: 0x0009CD27
		[BurstCompile]
		public static double ACosh(double x)
		{
			return Math.Log(x + Math.Sqrt(x * x - 1.0));
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x0009EB42 File Offset: 0x0009CD42
		[BurstCompile]
		public static double ASinh(double x)
		{
			return Math.Log(x + Math.Sqrt(x * x + 1.0));
		}

		// Token: 0x06001E45 RID: 7749 RVA: 0x0009EB5D File Offset: 0x0009CD5D
		[BurstCompile]
		public static double Sin(double d)
		{
			return Math.Sin(d);
		}

		// Token: 0x06001E46 RID: 7750 RVA: 0x0009EB65 File Offset: 0x0009CD65
		[BurstCompile]
		public static double Cos(double d)
		{
			return Math.Cos(d);
		}

		// Token: 0x06001E47 RID: 7751 RVA: 0x0009EB6D File Offset: 0x0009CD6D
		[BurstCompile]
		public static double Tan(double d)
		{
			return Math.Tan(d);
		}

		// Token: 0x06001E48 RID: 7752 RVA: 0x0009EB75 File Offset: 0x0009CD75
		[BurstCompile]
		public static double Asin(double d)
		{
			return Math.Asin(d);
		}

		// Token: 0x06001E49 RID: 7753 RVA: 0x0009EB7D File Offset: 0x0009CD7D
		[BurstCompile]
		public static double Acos(double d)
		{
			return Math.Acos(d);
		}

		// Token: 0x06001E4A RID: 7754 RVA: 0x0009EB85 File Offset: 0x0009CD85
		[BurstCompile]
		public static double Atan(double d)
		{
			return Math.Atan(d);
		}

		// Token: 0x06001E4B RID: 7755 RVA: 0x0009EB8D File Offset: 0x0009CD8D
		[BurstCompile]
		public static double Atan2(double y, double x)
		{
			return Math.Atan2(y, x);
		}

		// Token: 0x06001E4C RID: 7756 RVA: 0x0009EB96 File Offset: 0x0009CD96
		[BurstCompile]
		public static double Atanh(double d)
		{
			return Math.Log((1.0 + d) / (1.0 - d)) / 2.0;
		}

		// Token: 0x06001E4D RID: 7757 RVA: 0x0009EBBE File Offset: 0x0009CDBE
		[BurstCompile]
		public static double Sqrt(double d)
		{
			return Math.Sqrt(d);
		}

		// Token: 0x06001E4E RID: 7758 RVA: 0x0009EBC6 File Offset: 0x0009CDC6
		public static double Abs(double d)
		{
			return Math.Abs(d);
		}

		// Token: 0x06001E4F RID: 7759 RVA: 0x0009EBCE File Offset: 0x0009CDCE
		public static int Abs(int value)
		{
			return Math.Abs(value);
		}

		// Token: 0x06001E50 RID: 7760 RVA: 0x0009EBD6 File Offset: 0x0009CDD6
		[BurstCompile]
		public static double Normalize_Rad(double angle)
		{
			return angle - 6.283185307179586 * Mathd.Floor((angle + 3.141592653589793) / 6.283185307179586);
		}

		// Token: 0x06001E51 RID: 7761 RVA: 0x0009EBFE File Offset: 0x0009CDFE
		public static double Min(double a, double b)
		{
			if (a < b)
			{
				return a;
			}
			return b;
		}

		// Token: 0x06001E52 RID: 7762 RVA: 0x0009EC08 File Offset: 0x0009CE08
		public static double Min(params double[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0.0;
			}
			double num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] < num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		// Token: 0x06001E53 RID: 7763 RVA: 0x0009EC41 File Offset: 0x0009CE41
		public static int Min(int a, int b)
		{
			if (a < b)
			{
				return a;
			}
			return b;
		}

		// Token: 0x06001E54 RID: 7764 RVA: 0x0009EC4C File Offset: 0x0009CE4C
		public static int Min(params int[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0;
			}
			int num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] < num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		// Token: 0x06001E55 RID: 7765 RVA: 0x0009EC7D File Offset: 0x0009CE7D
		public static double Max(double a, double b)
		{
			if (a > b)
			{
				return a;
			}
			return b;
		}

		// Token: 0x06001E56 RID: 7766 RVA: 0x0009EC88 File Offset: 0x0009CE88
		public static double Max(params double[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0.0;
			}
			double num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] > num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		// Token: 0x06001E57 RID: 7767 RVA: 0x0009ECC3 File Offset: 0x0009CEC3
		public static int Max(int a, int b)
		{
			if (a > b)
			{
				return a;
			}
			return b;
		}

		// Token: 0x06001E58 RID: 7768 RVA: 0x0009ECCC File Offset: 0x0009CECC
		public static int Max(params int[] values)
		{
			int num = values.Length;
			if (num == 0)
			{
				return 0;
			}
			int num2 = values[0];
			for (int i = 1; i < num; i++)
			{
				if (values[i] > num2)
				{
					num2 = values[i];
				}
			}
			return num2;
		}

		// Token: 0x06001E59 RID: 7769 RVA: 0x0009ECFD File Offset: 0x0009CEFD
		[BurstCompile]
		public static double Pow(double d, double p)
		{
			return Math.Pow(d, p);
		}

		// Token: 0x06001E5A RID: 7770 RVA: 0x0009ED06 File Offset: 0x0009CF06
		[BurstCompile]
		public static double Exp(double power)
		{
			return Math.Exp(power);
		}

		// Token: 0x06001E5B RID: 7771 RVA: 0x0009ED0E File Offset: 0x0009CF0E
		[BurstCompile]
		public static double Log(double d, double p)
		{
			return Math.Log(d, p);
		}

		// Token: 0x06001E5C RID: 7772 RVA: 0x0009ED17 File Offset: 0x0009CF17
		[BurstCompile]
		public static double Log(double d)
		{
			return Math.Log(d);
		}

		// Token: 0x06001E5D RID: 7773 RVA: 0x0009ED1F File Offset: 0x0009CF1F
		[BurstCompile]
		public static double Log10(double d)
		{
			return Math.Log10(d);
		}

		// Token: 0x06001E5E RID: 7774 RVA: 0x0009ED27 File Offset: 0x0009CF27
		[BurstCompile]
		public static double Ceil(double d)
		{
			return Math.Ceiling(d);
		}

		// Token: 0x06001E5F RID: 7775 RVA: 0x0009ED2F File Offset: 0x0009CF2F
		[BurstCompile]
		public static double Floor(double d)
		{
			return Math.Floor(d);
		}

		// Token: 0x06001E60 RID: 7776 RVA: 0x0009ED37 File Offset: 0x0009CF37
		[BurstCompile]
		public static double Round(double d)
		{
			return Math.Round(d);
		}

		// Token: 0x06001E61 RID: 7777 RVA: 0x0009ED3F File Offset: 0x0009CF3F
		[BurstCompile]
		public static int CeilToInt(double d)
		{
			return (int)Math.Ceiling(d);
		}

		// Token: 0x06001E62 RID: 7778 RVA: 0x0009ED48 File Offset: 0x0009CF48
		[BurstCompile]
		public static int FloorToInt(double d)
		{
			return (int)Math.Floor(d);
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x0009ED51 File Offset: 0x0009CF51
		[BurstCompile]
		public static int RoundToInt(double d)
		{
			return (int)Math.Round(d);
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x0009ED5A File Offset: 0x0009CF5A
		[BurstCompile]
		public static double Sign(double d)
		{
			if (d < 0.0)
			{
				return -1.0;
			}
			return 1.0;
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x0009ED7B File Offset: 0x0009CF7B
		public static double Clamp(double value, double min, double max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
			return value;
		}

		// Token: 0x06001E66 RID: 7782 RVA: 0x0009ED8E File Offset: 0x0009CF8E
		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
			return value;
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x0009EDA1 File Offset: 0x0009CFA1
		[BurstCompile]
		public static double Clamp01(double value)
		{
			if (value < 0.0)
			{
				return 0.0;
			}
			if (value > 1.0)
			{
				return 1.0;
			}
			return value;
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x0009EDD0 File Offset: 0x0009CFD0
		[BurstCompile]
		public static double ClampRadiansTwoPI(double angle)
		{
			angle %= 6.283185307179586;
			if (angle < 0.0)
			{
				angle += 6.283185307179586;
			}
			return angle;
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x0009EDF9 File Offset: 0x0009CFF9
		[BurstCompile]
		public static double ClampRadiansPI(double angle)
		{
			angle = Mathd.ClampRadiansTwoPI(angle);
			if (angle > 3.141592653589793)
			{
				angle -= 6.283185307179586;
			}
			return angle;
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x0009EE1D File Offset: 0x0009D01D
		[BurstCompile]
		public static double Lerp(double from, double to, double t)
		{
			return from + (to - from) * Mathd.Clamp01(t);
		}

		// Token: 0x06001E6B RID: 7787 RVA: 0x0009EE2C File Offset: 0x0009D02C
		[BurstCompile]
		public static double LerpAngle(double a, double b, double t)
		{
			double num = Mathd.Repeat(b - a, 360.0);
			if (num > 180.0)
			{
				num -= 360.0;
			}
			return a + num * Mathd.Clamp01(t);
		}

		// Token: 0x06001E6C RID: 7788 RVA: 0x0009EE70 File Offset: 0x0009D070
		[BurstCompile]
		public static double LerpRadians(double a, double b, double t)
		{
			double num = Mathd.Repeat(b - a, 6.283185307179586);
			if (num > 3.141592653589793)
			{
				num -= 6.283185307179586;
			}
			return a + num * Mathd.Clamp01(t);
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x0009EEB4 File Offset: 0x0009D0B4
		[BurstCompile]
		public static double Berp(double a, double b, double t)
		{
			return a * ((2.0 * t - 3.0) * t * t + 1.0) + b * (3.0 - 2.0 * t) * t * t;
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x0009EF04 File Offset: 0x0009D104
		[BurstCompile]
		public static double BerpRadians(double a, double b, double t)
		{
			double num = Mathd.Repeat(b - a, 6.283185307179586);
			if (num > 3.141592653589793)
			{
				num -= 6.283185307179586;
			}
			return Mathd.Berp(a, a + num, t);
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x0009EF45 File Offset: 0x0009D145
		[BurstCompile]
		public static double MoveTowards(double current, double target, double maxDelta)
		{
			if (Mathd.Abs(target - current) <= maxDelta)
			{
				return target;
			}
			return current + Mathd.Sign(target - current) * maxDelta;
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0009EF60 File Offset: 0x0009D160
		[BurstCompile]
		public static double MoveTowardsAngle(double current, double target, double maxDelta)
		{
			target = current + Mathd.DeltaAngle(current, target);
			return Mathd.MoveTowards(current, target, maxDelta);
		}

		// Token: 0x06001E71 RID: 7793 RVA: 0x0009EF75 File Offset: 0x0009D175
		[BurstCompile]
		public static double SmoothStep(double from, double to, double t)
		{
			t = Mathd.Clamp01(t);
			t = -2.0 * t * t * t + 3.0 * t * t;
			return to * t + from * (1.0 - t);
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x0009EFB0 File Offset: 0x0009D1B0
		[BurstCompile]
		public static double Gamma(double value, double absmax, double gamma)
		{
			bool flag = false;
			if (value < 0.0)
			{
				flag = true;
			}
			double num = Mathd.Abs(value);
			if (num > absmax)
			{
				if (flag)
				{
					return -num;
				}
				return num;
			}
			else
			{
				double num2 = Mathd.Pow(num / absmax, gamma) * absmax;
				if (flag)
				{
					return -num2;
				}
				return num2;
			}
		}

		// Token: 0x06001E73 RID: 7795 RVA: 0x0009EFF3 File Offset: 0x0009D1F3
		[BurstCompile]
		public static bool Approximately(double a, double b)
		{
			return Mathd.Abs(b - a) < Mathd.Max(1E-06 * Mathd.Max(Mathd.Abs(a), Mathd.Abs(b)), 1.121039E-44);
		}

		// Token: 0x06001E74 RID: 7796 RVA: 0x0009F028 File Offset: 0x0009D228
		public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed)
		{
			double num = (double)Time.deltaTime;
			return Mathd.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, num);
		}

		// Token: 0x06001E75 RID: 7797 RVA: 0x0009F048 File Offset: 0x0009D248
		public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime)
		{
			double num = (double)Time.deltaTime;
			double positiveInfinity = double.PositiveInfinity;
			return Mathd.SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, num);
		}

		// Token: 0x06001E76 RID: 7798 RVA: 0x0009F074 File Offset: 0x0009D274
		[BurstCompile]
		public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
		{
			smoothTime = Mathd.Max(0.0001, smoothTime);
			double num = 2.0 / smoothTime;
			double num2 = num * deltaTime;
			double num3 = 1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2);
			double num4 = current - target;
			double num5 = target;
			double num6 = maxSpeed * smoothTime;
			double num7 = Mathd.Clamp(num4, -num6, num6);
			target = current - num7;
			double num8 = (currentVelocity + num * num7) * deltaTime;
			currentVelocity = (currentVelocity - num * num8) * num3;
			double num9 = target + (num7 + num8) * num3;
			if (num5 - current > 0.0 == num9 > num5)
			{
				num9 = num5;
				currentVelocity = (num9 - num5) / deltaTime;
			}
			return num9;
		}

		// Token: 0x06001E77 RID: 7799 RVA: 0x0009F13C File Offset: 0x0009D33C
		public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed)
		{
			double num = (double)Time.deltaTime;
			return Mathd.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, num);
		}

		// Token: 0x06001E78 RID: 7800 RVA: 0x0009F15C File Offset: 0x0009D35C
		public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime)
		{
			double num = (double)Time.deltaTime;
			double positiveInfinity = double.PositiveInfinity;
			return Mathd.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, positiveInfinity, num);
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x0009F185 File Offset: 0x0009D385
		[BurstCompile]
		public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
		{
			target = current + Mathd.DeltaAngle(current, target);
			return Mathd.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
		}

		// Token: 0x06001E7A RID: 7802 RVA: 0x0009F19F File Offset: 0x0009D39F
		[BurstCompile]
		public static double Repeat(double t, double length)
		{
			return t - Mathd.Floor(t / length) * length;
		}

		// Token: 0x06001E7B RID: 7803 RVA: 0x0009F1AD File Offset: 0x0009D3AD
		[BurstCompile]
		public static double PingPong(double t, double length)
		{
			t = Mathd.Repeat(t, length * 2.0);
			return length - Mathd.Abs(t - length);
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x0009F1CC File Offset: 0x0009D3CC
		[BurstCompile]
		public static double InverseLerp(double from, double to, double value)
		{
			if (from < to)
			{
				if (value < from)
				{
					return 0.0;
				}
				if (value > to)
				{
					return 1.0;
				}
				value -= from;
				value /= to - from;
				return value;
			}
			else
			{
				if (from <= to)
				{
					return 0.0;
				}
				if (value < to)
				{
					return 1.0;
				}
				if (value > from)
				{
					return 0.0;
				}
				return 1.0 - (value - to) / (from - to);
			}
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x0009F244 File Offset: 0x0009D444
		[BurstCompile]
		public static double DeltaAngle(double current, double target)
		{
			double num = Mathd.Repeat(target - current, 360.0);
			if (num > 180.0)
			{
				num -= 360.0;
			}
			return num;
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x0009F27C File Offset: 0x0009D47C
		[BurstCompile]
		internal static bool LineIntersection(in Vector2d p1, in Vector2d p2, in Vector2d p3, in Vector2d p4, ref Vector2d result)
		{
			double num = p2.x - p1.x;
			double num2 = p2.y - p1.y;
			double num3 = p4.x - p3.x;
			double num4 = p4.y - p3.y;
			double num5 = num * num4 - num2 * num3;
			if (num5 == 0.0)
			{
				return false;
			}
			double num6 = p3.x - p1.x;
			double num7 = p3.y - p1.y;
			double num8 = (num6 * num4 - num7 * num3) / num5;
			result = new Vector2d(p1.x + num8 * num, p1.y + num8 * num2);
			return true;
		}

		// Token: 0x06001E7F RID: 7807 RVA: 0x0009F324 File Offset: 0x0009D524
		[BurstCompile]
		internal static bool LineSegmentIntersection(in Vector2d p1, in Vector2d p2, in Vector2d p3, in Vector2d p4, ref Vector2d result)
		{
			double num = p2.x - p1.x;
			double num2 = p2.y - p1.y;
			double num3 = p4.x - p3.x;
			double num4 = p4.y - p3.y;
			double num5 = num * num4 - num2 * num3;
			if (num5 == 0.0)
			{
				return false;
			}
			double num6 = p3.x - p1.x;
			double num7 = p3.y - p1.y;
			double num8 = (num6 * num4 - num7 * num3) / num5;
			if (num8 < 0.0 || num8 > 1.0)
			{
				return false;
			}
			double num9 = (num6 * num2 - num7 * num) / num5;
			if (num9 < 0.0 || num9 > 1.0)
			{
				return false;
			}
			result = new Vector2d(p1.x + num8 * num, p1.y + num8 * num2);
			return true;
		}

		// Token: 0x06001E80 RID: 7808 RVA: 0x0009F416 File Offset: 0x0009D616
		public static int d100()
		{
			return TIUtilities.RandomRange(1, 101);
		}

		// Token: 0x06001E81 RID: 7809 RVA: 0x0009F420 File Offset: 0x0009D620
		[BurstCompile]
		public static double AngularRadiusOfSphere_Rad(double radius, double distance)
		{
			return Mathd.Asin(radius / distance);
		}

		// Token: 0x06001E82 RID: 7810 RVA: 0x0009F42A File Offset: 0x0009D62A
		[BurstCompile]
		public static double AngularDiameterOfSphere(double radius, double distance)
		{
			return 2.0 * Mathd.Asin(radius / distance) * 57.29577951308232;
		}

		// Token: 0x06001E83 RID: 7811 RVA: 0x0009F448 File Offset: 0x0009D648
		[BurstCompile]
		public static double AngularDiameterOfPlane(double radius, double distance)
		{
			return 2.0 * Mathd.Atan(radius / distance) * 57.29577951308232;
		}

		// Token: 0x06001E84 RID: 7812 RVA: 0x0009F468 File Offset: 0x0009D668
		public static double SumProduct(double[] list1, double[] list2)
		{
			double num = 0.0;
			for (int i = 0; i < list1.Length; i++)
			{
				num += list1[i] * list2[i];
			}
			return num;
		}

		// Token: 0x06001E85 RID: 7813 RVA: 0x0009F498 File Offset: 0x0009D698
		public static double WeightedMean(double[] list1, double[] weights)
		{
			return Mathd.SumProduct(list1, weights) / weights.Sum();
		}

		// Token: 0x040017F5 RID: 6133
		public const double HALFPI = 1.5707963267948966;

		// Token: 0x040017F6 RID: 6134
		public const double PI = 3.141592653589793;

		// Token: 0x040017F7 RID: 6135
		public const double TWOPI = 6.283185307179586;

		// Token: 0x040017F8 RID: 6136
		public const double Infinity = double.PositiveInfinity;

		// Token: 0x040017F9 RID: 6137
		public const double NegativeInfinity = double.NegativeInfinity;

		// Token: 0x040017FA RID: 6138
		public const double Deg2Rad = 0.017453292519943295;

		// Token: 0x040017FB RID: 6139
		public const double Rad2Deg = 57.29577951308232;

		// Token: 0x040017FC RID: 6140
		public const double Epsilon = 1.401298E-45;

		// Token: 0x040017FD RID: 6141
		public const float TWOPIf = 6.2831855f;

		// Token: 0x040017FE RID: 6142
		public const float Log10Two = 0.30103f;

		// Token: 0x040017FF RID: 6143
		public const float LnTwo = 0.6931472f;
	}
}
