using System;
using Unity.Burst;

namespace UnityEngine
{
	// Token: 0x020004F4 RID: 1268
	[BurstCompile]
	[Serializable]
	public struct Vector2d
	{
		// Token: 0x06001EE4 RID: 7908 RVA: 0x000A1263 File Offset: 0x0009F463
		[BurstDiscard]
		public static explicit operator Vector2(Vector2d v)
		{
			return new Vector2((float)v.x, (float)v.y);
		}

		// Token: 0x06001EE5 RID: 7909 RVA: 0x000A1278 File Offset: 0x0009F478
		[BurstDiscard]
		public static explicit operator Vector3(Vector2d v)
		{
			return new Vector3((float)v.x, (float)v.y, 0f);
		}

		// Token: 0x06001EE6 RID: 7910 RVA: 0x000A1292 File Offset: 0x0009F492
		[BurstDiscard]
		public static implicit operator Vector2d(Vector3d v)
		{
			return new Vector2d(v.x, v.y);
		}

		// Token: 0x06001EE7 RID: 7911 RVA: 0x000A12A5 File Offset: 0x0009F4A5
		[BurstDiscard]
		public static implicit operator Vector3d(Vector2d v)
		{
			return new Vector3d(v.x, v.y, 0.0);
		}

		// Token: 0x1700045E RID: 1118
		public double this[int index]
		{
			readonly get
			{
				double num;
				if (index != 0)
				{
					if (index != 1)
					{
						throw new IndexOutOfRangeException("Invalid Vector2d index!");
					}
					num = this.y;
				}
				else
				{
					num = this.x;
				}
				return num;
			}
			set
			{
				if (index == 0)
				{
					this.x = value;
					return;
				}
				if (index != 1)
				{
					throw new IndexOutOfRangeException("Invalid Vector2d index!");
				}
				this.y = value;
			}
		}

		// Token: 0x1700045F RID: 1119
		// (get) Token: 0x06001EEA RID: 7914 RVA: 0x000A131D File Offset: 0x0009F51D
		public readonly Vector2d normalized
		{
			get
			{
				return Vector2d.Normalize(this);
			}
		}

		// Token: 0x17000460 RID: 1120
		// (get) Token: 0x06001EEB RID: 7915 RVA: 0x000A132A File Offset: 0x0009F52A
		public readonly double magnitude
		{
			get
			{
				return Vector2d.Magnitude(in this);
			}
		}

		// Token: 0x17000461 RID: 1121
		// (get) Token: 0x06001EEC RID: 7916 RVA: 0x000A1332 File Offset: 0x0009F532
		public readonly double sqrMagnitude
		{
			get
			{
				return Vector2d.SqrMagnitude(in this);
			}
		}

		// Token: 0x06001EED RID: 7917 RVA: 0x000A133A File Offset: 0x0009F53A
		public Vector2d(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06001EEE RID: 7918 RVA: 0x000A134A File Offset: 0x0009F54A
		public static Vector2d operator +(Vector2d a, Vector2d b)
		{
			return new Vector2d(a.x + b.x, a.y + b.y);
		}

		// Token: 0x06001EEF RID: 7919 RVA: 0x000A136B File Offset: 0x0009F56B
		public static Vector2d operator -(Vector2d a, Vector2d b)
		{
			return new Vector2d(a.x - b.x, a.y - b.y);
		}

		// Token: 0x06001EF0 RID: 7920 RVA: 0x000A138C File Offset: 0x0009F58C
		public static Vector2d operator -(Vector2d a)
		{
			return new Vector2d(-a.x, -a.y);
		}

		// Token: 0x06001EF1 RID: 7921 RVA: 0x000A13A1 File Offset: 0x0009F5A1
		public static Vector2d operator *(Vector2d a, double d)
		{
			return new Vector2d(a.x * d, a.y * d);
		}

		// Token: 0x06001EF2 RID: 7922 RVA: 0x000A13B8 File Offset: 0x0009F5B8
		public static Vector2d operator *(double d, Vector2d a)
		{
			return new Vector2d(a.x * d, a.y * d);
		}

		// Token: 0x06001EF3 RID: 7923 RVA: 0x000A13CF File Offset: 0x0009F5CF
		public static Vector2d operator /(Vector2d a, double d)
		{
			return new Vector2d(a.x / d, a.y / d);
		}

		// Token: 0x06001EF4 RID: 7924 RVA: 0x000A13E6 File Offset: 0x0009F5E6
		[BurstCompile]
		public static bool operator ==(in Vector2d lhs, in Vector2d rhs)
		{
			return Mathd.Abs(lhs.x - rhs.x) < 1E-05 && Mathd.Abs(lhs.y - rhs.y) < 1E-05;
		}

		// Token: 0x06001EF5 RID: 7925 RVA: 0x000A1424 File Offset: 0x0009F624
		[BurstCompile]
		public static bool operator !=(in Vector2d lhs, in Vector2d rhs)
		{
			return !((in lhs) == (in rhs));
		}

		// Token: 0x06001EF6 RID: 7926 RVA: 0x000A1430 File Offset: 0x0009F630
		public override readonly bool Equals(object other)
		{
			if (other is Vector2d)
			{
				Vector2d vector2d = (Vector2d)other;
				return (in this) == (in vector2d);
			}
			return false;
		}

		// Token: 0x06001EF7 RID: 7927 RVA: 0x000A1458 File Offset: 0x0009F658
		public override readonly int GetHashCode()
		{
			double num = this.x;
			int hashCode = num.GetHashCode();
			num = this.y;
			return hashCode ^ (num.GetHashCode() << 2);
		}

		// Token: 0x06001EF8 RID: 7928 RVA: 0x000A1484 File Offset: 0x0009F684
		public void Set(double new_x, double new_y)
		{
			this.x = new_x;
			this.y = new_y;
		}

		// Token: 0x06001EF9 RID: 7929 RVA: 0x000A1494 File Offset: 0x0009F694
		public static Vector2d Lerp(Vector2d from, Vector2d to, double t)
		{
			t = Mathd.Clamp01(t);
			return new Vector2d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
		}

		// Token: 0x06001EFA RID: 7930 RVA: 0x000A14D0 File Offset: 0x0009F6D0
		public static Vector2d MoveTowards(Vector2d current, Vector2d target, double maxDistanceDelta)
		{
			Vector2d vector2d = target - current;
			double magnitude = vector2d.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == 0.0)
			{
				return target;
			}
			return current + vector2d / magnitude * maxDistanceDelta;
		}

		// Token: 0x06001EFB RID: 7931 RVA: 0x000A1512 File Offset: 0x0009F712
		public static Vector2d Scale(Vector2d a, Vector2d b)
		{
			return new Vector2d(a.x * b.x, a.y * b.y);
		}

		// Token: 0x06001EFC RID: 7932 RVA: 0x000A1533 File Offset: 0x0009F733
		public void Scale(Vector2d scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
		}

		// Token: 0x06001EFD RID: 7933 RVA: 0x000A155B File Offset: 0x0009F75B
		[BurstCompile]
		public static double Dot(in Vector2d lhs, in Vector2d rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y;
		}

		// Token: 0x06001EFE RID: 7934 RVA: 0x000A1578 File Offset: 0x0009F778
		public static Vector2d Normalize(Vector2d v)
		{
			double num = Vector2d.Magnitude(in v);
			if (num <= 1E-05)
			{
				return Vector2d.zero;
			}
			return v / num;
		}

		// Token: 0x06001EFF RID: 7935 RVA: 0x000A15A8 File Offset: 0x0009F7A8
		public void Normalize()
		{
			double magnitude = this.magnitude;
			this = ((magnitude > 1E-05) ? (this / magnitude) : Vector2d.zero);
		}

		// Token: 0x06001F00 RID: 7936 RVA: 0x000A15E4 File Offset: 0x0009F7E4
		[BurstCompile]
		public static double Angle(in Vector2d from, in Vector2d to)
		{
			Vector2d normalized = from.normalized;
			Vector2d normalized2 = to.normalized;
			return Mathd.Acos(Mathd.Clamp(Vector2d.Dot(in normalized, in normalized2), -1.0, 1.0)) * 57.29577951308232;
		}

		// Token: 0x06001F01 RID: 7937 RVA: 0x000A1630 File Offset: 0x0009F830
		[BurstCompile]
		public static double Distance(in Vector2d a, in Vector2d b)
		{
			return (a - b).magnitude;
		}

		// Token: 0x06001F02 RID: 7938 RVA: 0x000A1656 File Offset: 0x0009F856
		public static Vector2d ClampMagnitude(Vector2d vector, double maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}

		// Token: 0x06001F03 RID: 7939 RVA: 0x000A1673 File Offset: 0x0009F873
		[BurstCompile]
		public static double Magnitude(in Vector2d a)
		{
			return Mathd.Sqrt(a.x * a.x + a.y * a.y);
		}

		// Token: 0x06001F04 RID: 7940 RVA: 0x000A1695 File Offset: 0x0009F895
		[BurstCompile]
		public static double SqrMagnitude(in Vector2d a)
		{
			return a.x * a.x + a.y * a.y;
		}

		// Token: 0x06001F05 RID: 7941 RVA: 0x000A16B2 File Offset: 0x0009F8B2
		public static Vector2d Min(Vector2d lhs, Vector2d rhs)
		{
			return new Vector2d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y));
		}

		// Token: 0x06001F06 RID: 7942 RVA: 0x000A16DB File Offset: 0x0009F8DB
		public static Vector2d Max(Vector2d lhs, Vector2d rhs)
		{
			return new Vector2d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y));
		}

		// Token: 0x06001F07 RID: 7943 RVA: 0x000A1704 File Offset: 0x0009F904
		public override readonly string ToString()
		{
			return string.Format("x {0}, y {1}, m {2}", this.x, this.y, this.magnitude);
		}

		// Token: 0x0400180A RID: 6154
		public const double kEpsilon = 1E-05;

		// Token: 0x0400180B RID: 6155
		public double x;

		// Token: 0x0400180C RID: 6156
		public double y;

		// Token: 0x0400180D RID: 6157
		public static readonly Vector2d zero = new Vector2d(0.0, 0.0);

		// Token: 0x0400180E RID: 6158
		public static readonly Vector2d one = new Vector2d(1.0, 1.0);

		// Token: 0x0400180F RID: 6159
		public static readonly Vector2d up = new Vector2d(0.0, 1.0);

		// Token: 0x04001810 RID: 6160
		public static readonly Vector2d down = new Vector2d(0.0, -1.0);

		// Token: 0x04001811 RID: 6161
		public static readonly Vector2d right = new Vector2d(1.0, 0.0);

		// Token: 0x04001812 RID: 6162
		public static readonly Vector2d left = new Vector2d(-1.0, 0.0);
	}
}
