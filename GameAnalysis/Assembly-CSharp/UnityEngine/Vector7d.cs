using System;

namespace UnityEngine
{
	// Token: 0x020004F7 RID: 1271
	public class Vector7d
	{
		// Token: 0x1700046D RID: 1133
		public double this[int index]
		{
			get
			{
				return this.v[index];
			}
			set
			{
				this.v[index] = value;
			}
		}

		// Token: 0x1700046E RID: 1134
		// (get) Token: 0x06001F6C RID: 8044 RVA: 0x000A2E7A File Offset: 0x000A107A
		public Vector7d normalized
		{
			get
			{
				return Vector7d.Normalize(this);
			}
		}

		// Token: 0x1700046F RID: 1135
		// (get) Token: 0x06001F6D RID: 8045 RVA: 0x000A2E82 File Offset: 0x000A1082
		public double magnitude
		{
			get
			{
				return Math.Sqrt(this.sqrMagnitude);
			}
		}

		// Token: 0x17000470 RID: 1136
		// (get) Token: 0x06001F6E RID: 8046 RVA: 0x000A2E90 File Offset: 0x000A1090
		public double sqrMagnitude
		{
			get
			{
				double num = 0.0;
				for (int i = 0; i < this.v.Length; i++)
				{
					num += this.v[i] * this.v[i];
				}
				return num;
			}
		}

		// Token: 0x17000471 RID: 1137
		// (get) Token: 0x06001F6F RID: 8047 RVA: 0x000A2ED0 File Offset: 0x000A10D0
		public static Vector7d zero
		{
			get
			{
				return new Vector7d(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
			}
		}

		// Token: 0x17000472 RID: 1138
		// (get) Token: 0x06001F70 RID: 8048 RVA: 0x000A2F24 File Offset: 0x000A1124
		public static Vector7d one
		{
			get
			{
				return new Vector7d(1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0);
			}
		}

		// Token: 0x06001F71 RID: 8049 RVA: 0x000A2F78 File Offset: 0x000A1178
		public Vector7d(double x, double y, double z, double i, double j, double k, double w)
		{
			this.v[0] = x;
			this.v[1] = y;
			this.v[2] = z;
			this.v[3] = i;
			this.v[4] = j;
			this.v[5] = k;
			this.v[6] = w;
		}

		// Token: 0x06001F72 RID: 8050 RVA: 0x000A2FDC File Offset: 0x000A11DC
		public Vector7d(float x, float y, float z, float i, float j, float k, float w)
		{
			this.v[0] = (double)x;
			this.v[1] = (double)y;
			this.v[2] = (double)z;
			this.v[3] = (double)i;
			this.v[4] = (double)j;
			this.v[5] = (double)k;
			this.v[6] = (double)w;
		}

		// Token: 0x06001F73 RID: 8051 RVA: 0x000A3048 File Offset: 0x000A1248
		public Vector7d(Vector3 v3, Vector3 i3, float w)
		{
			this.v[0] = (double)v3.x;
			this.v[1] = (double)v3.y;
			this.v[2] = (double)v3.z;
			this.v[3] = (double)i3.x;
			this.v[4] = (double)i3.y;
			this.v[5] = (double)i3.z;
			this.v[6] = (double)w;
		}

		// Token: 0x06001F74 RID: 8052 RVA: 0x000A30CC File Offset: 0x000A12CC
		public Vector7d(Vector3d v3, Vector3d i3, double w)
		{
			this.v[0] = v3.x;
			this.v[1] = v3.y;
			this.v[2] = v3.z;
			this.v[3] = i3.x;
			this.v[4] = i3.y;
			this.v[5] = i3.z;
			this.v[6] = w;
		}

		// Token: 0x06001F75 RID: 8053 RVA: 0x000A3148 File Offset: 0x000A1348
		public Vector7d(Vector7d v7)
		{
			this.v[0] = v7.v[0];
			this.v[1] = v7.v[1];
			this.v[2] = v7.v[2];
			this.v[3] = v7.v[3];
			this.v[4] = v7.v[4];
			this.v[5] = v7.v[5];
			this.v[6] = v7.v[6];
		}

		// Token: 0x06001F76 RID: 8054 RVA: 0x000A31D8 File Offset: 0x000A13D8
		public static Vector7d operator +(Vector7d a, Vector7d b)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] + b.v[i];
			}
			return zero;
		}

		// Token: 0x06001F77 RID: 8055 RVA: 0x000A3218 File Offset: 0x000A1418
		public static Vector7d operator -(Vector7d a, Vector7d b)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] - b.v[i];
			}
			return zero;
		}

		// Token: 0x06001F78 RID: 8056 RVA: 0x000A3258 File Offset: 0x000A1458
		public static Vector7d operator -(Vector7d a)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = -a.v[i];
			}
			return zero;
		}

		// Token: 0x06001F79 RID: 8057 RVA: 0x000A3290 File Offset: 0x000A1490
		public static Vector7d operator *(Vector7d a, double d)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] * d;
			}
			return zero;
		}

		// Token: 0x06001F7A RID: 8058 RVA: 0x000A32CC File Offset: 0x000A14CC
		public static Vector7d operator *(double d, Vector7d a)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] * d;
			}
			return zero;
		}

		// Token: 0x06001F7B RID: 8059 RVA: 0x000A3308 File Offset: 0x000A1508
		public static Vector7d operator /(Vector7d a, double d)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] / d;
			}
			return zero;
		}

		// Token: 0x06001F7C RID: 8060 RVA: 0x000A3341 File Offset: 0x000A1541
		public static bool operator ==(Vector7d lhs, Vector7d rhs)
		{
			return Vector7d.SqrMagnitude(lhs - rhs) < 0.0;
		}

		// Token: 0x06001F7D RID: 8061 RVA: 0x000A335B File Offset: 0x000A155B
		public static bool operator !=(Vector7d lhs, Vector7d rhs)
		{
			return Vector7d.SqrMagnitude(lhs - rhs) >= 0.0;
		}

		// Token: 0x06001F7E RID: 8062 RVA: 0x000A3378 File Offset: 0x000A1578
		public static Vector7d Lerp(Vector7d from, Vector7d to, double t)
		{
			t = Mathd.Clamp01(t);
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < from.v.Length; i++)
			{
				zero.v[i] = from.v[i] + (to.v[i] - from.v[i]) * t;
			}
			return zero;
		}

		// Token: 0x06001F7F RID: 8063 RVA: 0x000A33CC File Offset: 0x000A15CC
		public void Set(double new_x, double new_y, double new_z, double new_i, double new_j, double new_k, double new_w)
		{
			this.v[0] = new_x;
			this.v[1] = new_y;
			this.v[2] = new_z;
			this.v[3] = new_i;
			this.v[4] = new_j;
			this.v[5] = new_k;
			this.v[6] = new_w;
		}

		// Token: 0x06001F80 RID: 8064 RVA: 0x000A341C File Offset: 0x000A161C
		public static Vector7d Scale(Vector7d a, Vector7d b)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] * b.v[i];
			}
			return zero;
		}

		// Token: 0x06001F81 RID: 8065 RVA: 0x000A345C File Offset: 0x000A165C
		public void Scale(Vector7d scale)
		{
			this.v[0] *= scale.v[0];
			this.v[1] *= scale.v[1];
			this.v[2] *= scale.v[2];
			this.v[3] *= scale.v[3];
			this.v[4] *= scale.v[4];
			this.v[5] *= scale.v[5];
			this.v[6] *= scale.v[6];
		}

		// Token: 0x06001F82 RID: 8066 RVA: 0x000A3514 File Offset: 0x000A1714
		public override int GetHashCode()
		{
			return this.v[0].GetHashCode() ^ (this.v[1].GetHashCode() << 2) ^ (this.v[2].GetHashCode() >> 2) ^ (this.v[3].GetHashCode() >> 4) ^ (this.v[4].GetHashCode() << 4) ^ (this.v[5].GetHashCode() >> 6) ^ (this.v[6].GetHashCode() << 6);
		}

		// Token: 0x06001F83 RID: 8067 RVA: 0x000A35AC File Offset: 0x000A17AC
		public override bool Equals(object other)
		{
			if (!(other is Vector7d))
			{
				return false;
			}
			Vector7d vector7d = (Vector7d)other;
			for (int i = 0; i < this.v.Length; i++)
			{
				if (!this.v[i].Equals(vector7d.v[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001F84 RID: 8068 RVA: 0x000A35FC File Offset: 0x000A17FC
		public static Vector7d Normalize(Vector7d value)
		{
			double num = Vector7d.Magnitude(value);
			if (num > 9.99999974737875E-06)
			{
				return value / num;
			}
			return Vector7d.zero;
		}

		// Token: 0x06001F85 RID: 8069 RVA: 0x000A362C File Offset: 0x000A182C
		public void Normalize()
		{
			double num = Vector7d.Magnitude(this);
			if (num > 9.99999974737875E-06)
			{
				for (int i = 0; i < this.v.Length; i++)
				{
					this.v[i] = this.v[i] / num;
				}
				return;
			}
			for (int j = 0; j < this.v.Length; j++)
			{
				this.v[j] = 0.0;
			}
		}

		// Token: 0x06001F86 RID: 8070 RVA: 0x000A3698 File Offset: 0x000A1898
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"(",
				this.v[0].ToString(),
				", ",
				this.v[1].ToString(),
				", ",
				this.v[2].ToString(),
				", ",
				this.v[3].ToString(),
				", ",
				this.v[4].ToString(),
				", ",
				this.v[5].ToString(),
				", ",
				this.v[6].ToString(),
				")"
			});
		}

		// Token: 0x06001F87 RID: 8071 RVA: 0x000A3784 File Offset: 0x000A1984
		public static double Dot(Vector7d lhs, Vector7d rhs)
		{
			double num = 0.0;
			for (int i = 0; i < lhs.v.Length; i++)
			{
				num += lhs.v[i] + rhs.v[i];
			}
			return num;
		}

		// Token: 0x06001F88 RID: 8072 RVA: 0x000A37C4 File Offset: 0x000A19C4
		public static Vector7d Project(Vector7d vector, Vector7d onNormal)
		{
			double num = Vector7d.Dot(onNormal, onNormal);
			if (num < 1.40129846432482E-45)
			{
				return Vector7d.zero;
			}
			return onNormal * Vector7d.Dot(vector, onNormal) / num;
		}

		// Token: 0x06001F89 RID: 8073 RVA: 0x000A37FE File Offset: 0x000A19FE
		public static Vector7d Exclude(Vector7d excludeThis, Vector7d fromThat)
		{
			return fromThat - Vector7d.Project(fromThat, excludeThis);
		}

		// Token: 0x06001F8A RID: 8074 RVA: 0x000A380D File Offset: 0x000A1A0D
		public static double Angle(Vector7d from, Vector7d to)
		{
			return Mathd.Acos(Mathd.Clamp(Vector7d.Dot(from.normalized, to.normalized), -1.0, 1.0)) * 57.29578;
		}

		// Token: 0x06001F8B RID: 8075 RVA: 0x000A3846 File Offset: 0x000A1A46
		public static double Distance(Vector7d a, Vector7d b)
		{
			return (a - b).magnitude;
		}

		// Token: 0x06001F8C RID: 8076 RVA: 0x000A3854 File Offset: 0x000A1A54
		public static Vector7d ClampMagnitude(Vector7d vector, double maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}

		// Token: 0x06001F8D RID: 8077 RVA: 0x000A386F File Offset: 0x000A1A6F
		public static double Magnitude(Vector7d a)
		{
			return Math.Sqrt(a.sqrMagnitude);
		}

		// Token: 0x06001F8E RID: 8078 RVA: 0x000A387C File Offset: 0x000A1A7C
		public static double SqrMagnitude(Vector7d a)
		{
			double num = 0.0;
			for (int i = 0; i < a.v.Length; i++)
			{
				num += a.v[i] + a.v[i];
			}
			return num;
		}

		// Token: 0x06001F8F RID: 8079 RVA: 0x000A38BC File Offset: 0x000A1ABC
		public static Vector7d Min(Vector7d lhs, Vector7d rhs)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < lhs.v.Length; i++)
			{
				zero.v[i] += Mathd.Min(lhs.v[i], rhs.v[i]);
			}
			return zero;
		}

		// Token: 0x06001F90 RID: 8080 RVA: 0x000A3908 File Offset: 0x000A1B08
		public static Vector7d Max(Vector7d lhs, Vector7d rhs)
		{
			Vector7d zero = Vector7d.zero;
			for (int i = 0; i < lhs.v.Length; i++)
			{
				zero.v[i] += Mathd.Max(lhs.v[i], rhs.v[i]);
			}
			return zero;
		}

		// Token: 0x06001F91 RID: 8081 RVA: 0x000A3954 File Offset: 0x000A1B54
		[Obsolete("Use Vector7d.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
		public static double AngleBetween(Vector7d from, Vector7d to)
		{
			return Mathd.Acos(Mathd.Clamp(Vector7d.Dot(from.normalized, to.normalized), -1.0, 1.0));
		}

		// Token: 0x04001821 RID: 6177
		public double[] v = new double[7];

		// Token: 0x04001822 RID: 6178
		public const float kEpsilon = 1E-05f;
	}
}
