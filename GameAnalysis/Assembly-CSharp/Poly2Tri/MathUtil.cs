using System;
using UnityEngine;

namespace Poly2Tri
{
	// Token: 0x020004E7 RID: 1255
	public class MathUtil
	{
		// Token: 0x06001D62 RID: 7522 RVA: 0x0009B75D File Offset: 0x0009995D
		public static bool AreValuesEqual(double val1, double val2)
		{
			return MathUtil.AreValuesEqual(val1, val2, MathUtil.EPSILON);
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x0009B76B File Offset: 0x0009996B
		public static bool AreValuesEqual(double val1, double val2, double tolerance)
		{
			return val1 >= val2 - tolerance && val1 <= val2 + tolerance;
		}

		// Token: 0x06001D64 RID: 7524 RVA: 0x0009B77C File Offset: 0x0009997C
		public static bool IsValueBetween(double val, double min, double max, double tolerance)
		{
			if (min > max)
			{
				double num = min;
				min = max;
				max = num;
			}
			return val + tolerance >= min && val - tolerance <= max;
		}

		// Token: 0x06001D65 RID: 7525 RVA: 0x0009B798 File Offset: 0x00099998
		public static double RoundWithPrecision(double f, double precision)
		{
			if (precision < 0.0)
			{
				return f;
			}
			double num = Math.Pow(10.0, precision);
			return Math.Floor(f * num) / num;
		}

		// Token: 0x06001D66 RID: 7526 RVA: 0x0009B7CD File Offset: 0x000999CD
		public static double Clamp(double a, double low, double high)
		{
			return Math.Max(low, Math.Min(a, high));
		}

		// Token: 0x06001D67 RID: 7527 RVA: 0x0009B7DC File Offset: 0x000999DC
		public static void Swap<T>(ref T a, ref T b)
		{
			T t = a;
			a = b;
			b = t;
		}

		// Token: 0x06001D68 RID: 7528 RVA: 0x0009B804 File Offset: 0x00099A04
		public static uint Jenkins32Hash(byte[] data, uint nInitialValue)
		{
			foreach (byte b in data)
			{
				nInitialValue += (uint)b;
				nInitialValue += nInitialValue << 10;
				nInitialValue += nInitialValue >> 6;
			}
			nInitialValue += nInitialValue << 3;
			nInitialValue ^= nInitialValue >> 11;
			nInitialValue += nInitialValue << 15;
			return nInitialValue;
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x0009B854 File Offset: 0x00099A54
		public static Vector3d GetSphericalPosition(double latitude, double longitude, Quaterniond parentRotation, float parentRadius, Vector3d parentPosition)
		{
			return parentRotation * Quaterniond.AngleAxis(longitude, -Vector3.up) * Quaterniond.AngleAxis(latitude, -Vector3.right) * Vector3.forward * (double)parentRadius + parentPosition;
		}

		// Token: 0x040017CB RID: 6091
		public static double EPSILON = 1E-12;
	}
}
