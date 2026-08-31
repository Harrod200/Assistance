using System;
using Unity.Burst;

namespace UnityEngine
{
	// Token: 0x020004F3 RID: 1267
	[BurstCompile]
	[Serializable]
	public struct Quaterniond
	{
		// Token: 0x1700045C RID: 1116
		// (get) Token: 0x06001EB6 RID: 7862 RVA: 0x000A06D0 File Offset: 0x0009E8D0
		public static Quaterniond identity
		{
			get
			{
				return new Quaterniond(0.0, 0.0, 0.0, 1.0);
			}
		}

		// Token: 0x06001EB7 RID: 7863 RVA: 0x000A06FB File Offset: 0x0009E8FB
		[BurstDiscard]
		public static explicit operator Quaternion(Quaterniond q)
		{
			return new Quaternion((float)q.x, (float)q.y, (float)q.z, (float)q.w);
		}

		// Token: 0x06001EB8 RID: 7864 RVA: 0x000A071E File Offset: 0x0009E91E
		[BurstDiscard]
		public static implicit operator Quaterniond(Quaternion q)
		{
			return new Quaterniond(q.x, q.y, q.z, q.w);
		}

		// Token: 0x1700045D RID: 1117
		public double this[int index]
		{
			readonly get
			{
				double num;
				switch (index)
				{
				case 0:
					num = this.x;
					break;
				case 1:
					num = this.y;
					break;
				case 2:
					num = this.z;
					break;
				case 3:
					num = this.w;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
				return num;
			}
			set
			{
				switch (index)
				{
				case 0:
					this.x = value;
					return;
				case 1:
					this.y = value;
					return;
				case 2:
					this.z = value;
					return;
				case 3:
					this.w = value;
					return;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}

		// Token: 0x06001EBB RID: 7867 RVA: 0x000A07CF File Offset: 0x0009E9CF
		public Quaterniond(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		// Token: 0x06001EBC RID: 7868 RVA: 0x000A07EE File Offset: 0x0009E9EE
		public Quaterniond(float x, float y, float z, float w)
		{
			this.x = (double)x;
			this.y = (double)y;
			this.z = (double)z;
			this.w = (double)w;
		}

		// Token: 0x06001EBD RID: 7869 RVA: 0x000A0811 File Offset: 0x0009EA11
		public Quaterniond(Quaterniond q)
		{
			this.x = q.x;
			this.y = q.y;
			this.z = q.z;
			this.w = q.w;
		}

		// Token: 0x06001EBE RID: 7870 RVA: 0x000A0843 File Offset: 0x0009EA43
		public static Quaterniond operator +(Quaterniond a, Quaterniond b)
		{
			return new Quaterniond(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		// Token: 0x06001EBF RID: 7871 RVA: 0x000A087E File Offset: 0x0009EA7E
		public static Quaterniond operator -(Quaterniond a, Quaterniond b)
		{
			return new Quaterniond(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		// Token: 0x06001EC0 RID: 7872 RVA: 0x000A08B9 File Offset: 0x0009EAB9
		public static Quaterniond operator -(Quaterniond a)
		{
			return new Quaterniond(-a.x, -a.y, -a.z, -a.w);
		}

		// Token: 0x06001EC1 RID: 7873 RVA: 0x000A08DC File Offset: 0x0009EADC
		public static Quaterniond operator *(double d, Quaterniond a)
		{
			return new Quaterniond(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		// Token: 0x06001EC2 RID: 7874 RVA: 0x000A0903 File Offset: 0x0009EB03
		public static Quaterniond operator *(Quaterniond a, double d)
		{
			return new Quaterniond(a.x * d, a.y * d, a.z * d, a.w * d);
		}

		// Token: 0x06001EC3 RID: 7875 RVA: 0x000A092A File Offset: 0x0009EB2A
		public static Quaterniond operator /(Quaterniond a, double d)
		{
			return new Quaterniond(a.x / d, a.y / d, a.z / d, a.w / d);
		}

		// Token: 0x06001EC4 RID: 7876 RVA: 0x000A0954 File Offset: 0x0009EB54
		public static Quaterniond operator *(Quaterniond a, Quaterniond b)
		{
			return new Quaterniond(a.w * b.x + a.x * b.w + a.y * b.z + a.z * b.y, a.w * b.y - a.x * b.z + a.y * b.w + a.z * b.x, a.w * b.z + a.x * b.y - a.y * b.x + a.z * b.w, a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z);
		}

		// Token: 0x06001EC5 RID: 7877 RVA: 0x000A0A44 File Offset: 0x0009EC44
		public static Vector3d operator *(Quaterniond a, Vector3d b)
		{
			double num = a.x * 2.0;
			double num2 = a.y * 2.0;
			double num3 = a.z * 2.0;
			double num4 = a.x * num;
			double num5 = a.y * num2;
			double num6 = a.z * num3;
			double num7 = a.x * num2;
			double num8 = a.x * num3;
			double num9 = a.y * num3;
			double num10 = a.w * num;
			double num11 = a.w * num2;
			double num12 = a.w * num3;
			return new Vector3d((1.0 - (num5 + num6)) * b.x + (num7 - num12) * b.y + (num8 + num11) * b.z, (num7 + num12) * b.x + (1.0 - (num4 + num6)) * b.y + (num9 - num10) * b.z, (num8 - num11) * b.x + (num9 + num10) * b.y + (1.0 - (num4 + num5)) * b.z);
		}

		// Token: 0x06001EC6 RID: 7878 RVA: 0x000A0B70 File Offset: 0x0009ED70
		[BurstDiscard]
		public static Vector3d operator *(Quaterniond a, Vector3 b)
		{
			return a * b;
		}

		// Token: 0x06001EC7 RID: 7879 RVA: 0x000A0B80 File Offset: 0x0009ED80
		[BurstCompile]
		public static bool operator ==(in Quaterniond lhs, in Quaterniond rhs)
		{
			return Math.Abs(lhs.x - rhs.x) < 1E-05 && Math.Abs(lhs.y - rhs.y) < 1E-05 && Math.Abs(lhs.z - rhs.z) < 1E-05 && Math.Abs(lhs.w - rhs.w) < 1E-05;
		}

		// Token: 0x06001EC8 RID: 7880 RVA: 0x000A0C03 File Offset: 0x0009EE03
		[BurstCompile]
		public static bool operator !=(in Quaterniond lhs, in Quaterniond rhs)
		{
			return !((in lhs) == (in rhs));
		}

		// Token: 0x06001EC9 RID: 7881 RVA: 0x000A0C10 File Offset: 0x0009EE10
		public override readonly bool Equals(object other)
		{
			if (other is Quaterniond)
			{
				Quaterniond quaterniond = (Quaterniond)other;
				return (in this) == (in quaterniond);
			}
			return false;
		}

		// Token: 0x06001ECA RID: 7882 RVA: 0x000A0C38 File Offset: 0x0009EE38
		public override readonly int GetHashCode()
		{
			double num = this.x;
			int hashCode = num.GetHashCode();
			num = this.y;
			int num2 = hashCode ^ (num.GetHashCode() << 2);
			num = this.z;
			int num3 = num2 ^ (num.GetHashCode() >> 2);
			num = this.w;
			return num3 ^ (num.GetHashCode() << 1);
		}

		// Token: 0x06001ECB RID: 7883 RVA: 0x000A0C86 File Offset: 0x0009EE86
		public readonly double Angle(Quaterniond a, Quaterniond b)
		{
			return Mathd.Acos(Mathd.Clamp(Quaterniond.Dot(in a, in b), -1.0, 1.0)) * 57.29577951308232 * 2.0;
		}

		// Token: 0x06001ECC RID: 7884 RVA: 0x000A0CC4 File Offset: 0x0009EEC4
		public static Quaterniond AngleAxis(double angle, Vector3d axis)
		{
			double num = Mathd.Sin(angle * 0.017453292519943295 * 0.5);
			Vector3d normalized = axis.normalized;
			return new Quaterniond(num * normalized.x, num * normalized.y, num * normalized.z, Mathd.Cos(angle * 0.017453292519943295 * 0.5)).Normalized();
		}

		// Token: 0x06001ECD RID: 7885 RVA: 0x000A0D34 File Offset: 0x0009EF34
		public static Quaterniond AngleAxis(float angle, Vector3d axis)
		{
			double num = Mathd.Sin((double)angle * 0.017453292519943295 * 0.5);
			Vector3d normalized = axis.normalized;
			return new Quaterniond(num * normalized.x, num * normalized.y, num * normalized.z, Mathd.Cos((double)angle * 0.017453292519943295 * 0.5)).Normalized();
		}

		// Token: 0x06001ECE RID: 7886 RVA: 0x000A0DA5 File Offset: 0x0009EFA5
		[BurstCompile]
		public static double Dot(in Quaterniond lhs, in Quaterniond rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z + lhs.w * rhs.w;
		}

		// Token: 0x06001ECF RID: 7887 RVA: 0x000A0DDE File Offset: 0x0009EFDE
		public static Quaterniond Inverse(Quaterniond q)
		{
			return new Quaterniond(-q.x, -q.y, -q.z, q.w);
		}

		// Token: 0x06001ED0 RID: 7888 RVA: 0x000A0E00 File Offset: 0x0009F000
		public static Quaterniond Lerp(Quaterniond from, Quaterniond to, double t)
		{
			t = Mathd.Clamp01(t);
			return new Quaterniond(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t, from.w + (to.w - from.w) * t);
		}

		// Token: 0x06001ED1 RID: 7889 RVA: 0x000A0E72 File Offset: 0x0009F072
		public void Set(double new_w, double new_x, double new_y, double new_z)
		{
			this.x = new_x;
			this.y = new_y;
			this.z = new_z;
			this.w = new_w;
		}

		// Token: 0x06001ED2 RID: 7890 RVA: 0x000A0E94 File Offset: 0x0009F094
		public override readonly string ToString()
		{
			return string.Format("({0}, {1}, {2}, {3})", new object[] { this.x, this.y, this.z, this.w });
		}

		// Token: 0x06001ED3 RID: 7891 RVA: 0x000A0EEC File Offset: 0x0009F0EC
		internal readonly Quaterniond Normalized()
		{
			double num = Quaterniond.Magnitude(in this);
			if (num <= 1E-05)
			{
				return Quaterniond.identity;
			}
			return this / num;
		}

		// Token: 0x06001ED4 RID: 7892 RVA: 0x000A0F1E File Offset: 0x0009F11E
		[BurstCompile]
		internal static double Magnitude(in Quaterniond a)
		{
			return Mathd.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w);
		}

		// Token: 0x06001ED5 RID: 7893 RVA: 0x000A0F5C File Offset: 0x0009F15C
		[BurstCompile]
		internal static double SqrMagnitude(in Quaterniond a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z + a.w * a.w;
		}

		// Token: 0x06001ED6 RID: 7894 RVA: 0x000A0F98 File Offset: 0x0009F198
		[BurstDiscard]
		public static Quaterniond Euler(double x, double y, double z)
		{
			Quaternion quaternion = Quaternion.Euler((float)x, (float)y, (float)z);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001ED7 RID: 7895 RVA: 0x000A0FD0 File Offset: 0x0009F1D0
		[BurstDiscard]
		public static Quaterniond Euler(float x, float y, float z)
		{
			Quaternion quaternion = Quaternion.Euler(x, y, z);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001ED8 RID: 7896 RVA: 0x000A1003 File Offset: 0x0009F203
		[BurstDiscard]
		public static Quaterniond Euler(Vector3d v)
		{
			return Quaterniond.Euler(v.x, v.y, v.z);
		}

		// Token: 0x06001ED9 RID: 7897 RVA: 0x000A101C File Offset: 0x0009F21C
		[BurstDiscard]
		public static Quaterniond Euler(Vector3 v)
		{
			return Quaterniond.Euler(v.x, v.y, v.z);
		}

		// Token: 0x06001EDA RID: 7898 RVA: 0x000A1038 File Offset: 0x0009F238
		[BurstDiscard]
		public static Quaterniond FromToRotation(Vector3d from, Vector3d to)
		{
			Quaternion quaternion = Quaternion.FromToRotation((Vector3)from, (Vector3)to);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EDB RID: 7899 RVA: 0x000A1074 File Offset: 0x0009F274
		[BurstDiscard]
		public static Quaterniond FromToRotation(Vector3 from, Vector3 to)
		{
			Quaternion quaternion = Quaternion.FromToRotation(from, to);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EDC RID: 7900 RVA: 0x000A10A8 File Offset: 0x0009F2A8
		[BurstDiscard]
		public static Quaterniond LookRotation(Vector3d forward, Vector3d upwards)
		{
			Quaternion quaternion = Quaternion.LookRotation((Vector3)forward, (Vector3)upwards);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EDD RID: 7901 RVA: 0x000A10E4 File Offset: 0x0009F2E4
		[BurstDiscard]
		public static Quaterniond LookRotation(Vector3d forward)
		{
			Quaternion quaternion = Quaternion.LookRotation((Vector3)forward, Vector3.up);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EDE RID: 7902 RVA: 0x000A111F File Offset: 0x0009F31F
		[BurstDiscard]
		public static Quaterniond RotateTowards(Quaterniond from, Quaterniond to, double maxDegreesDelta)
		{
			return new Quaterniond(Quaternion.RotateTowards((Quaternion)from, (Quaternion)to, (float)maxDegreesDelta));
		}

		// Token: 0x06001EDF RID: 7903 RVA: 0x000A113E File Offset: 0x0009F33E
		[BurstDiscard]
		public static Quaterniond Slerp(Quaterniond from, Quaterniond to, double t)
		{
			return new Quaterniond(Quaternion.Slerp((Quaternion)from, (Quaternion)to, (float)t));
		}

		// Token: 0x06001EE0 RID: 7904 RVA: 0x000A1160 File Offset: 0x0009F360
		[BurstDiscard]
		public static Quaterniond SetFromToRotation(Vector3d fromDirection, Vector3d toDirection)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.SetFromToRotation((Vector3)fromDirection, (Vector3)toDirection);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EE1 RID: 7905 RVA: 0x000A11A8 File Offset: 0x0009F3A8
		[BurstDiscard]
		public static Quaterniond SetLookRotation(Vector3d view)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.SetLookRotation((Vector3)view, Vector3.up);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EE2 RID: 7906 RVA: 0x000A11EC File Offset: 0x0009F3EC
		[BurstDiscard]
		public static Quaterniond SetLookRotation(Vector3d view, Vector3d up)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.SetLookRotation((Vector3)view, (Vector3)up);
			return new Quaterniond(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
		}

		// Token: 0x06001EE3 RID: 7907 RVA: 0x000A1234 File Offset: 0x0009F434
		[BurstDiscard]
		public static void ToAngleAxis(Quaterniond qd, out double angle, out Vector3d axis)
		{
			float num;
			Vector3 vector;
			((Quaternion)qd).ToAngleAxis(out num, out vector);
			angle = (double)num;
			axis = vector;
		}

		// Token: 0x04001805 RID: 6149
		public const double kEpsilon = 1E-05;

		// Token: 0x04001806 RID: 6150
		public double w;

		// Token: 0x04001807 RID: 6151
		public double x;

		// Token: 0x04001808 RID: 6152
		public double y;

		// Token: 0x04001809 RID: 6153
		public double z;
	}
}
