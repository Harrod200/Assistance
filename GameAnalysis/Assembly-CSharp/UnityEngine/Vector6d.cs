using System;

namespace UnityEngine
{
	// Token: 0x020004F6 RID: 1270
	public struct Vector6d
	{
		// Token: 0x17000467 RID: 1127
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

		// Token: 0x17000468 RID: 1128
		// (get) Token: 0x06001F44 RID: 8004 RVA: 0x000A23EA File Offset: 0x000A05EA
		public Vector6d normalized
		{
			get
			{
				return Vector6d.Normalize(this);
			}
		}

		// Token: 0x17000469 RID: 1129
		// (get) Token: 0x06001F45 RID: 8005 RVA: 0x000A23F7 File Offset: 0x000A05F7
		public double magnitude
		{
			get
			{
				return Math.Sqrt(this.sqrMagnitude);
			}
		}

		// Token: 0x1700046A RID: 1130
		// (get) Token: 0x06001F46 RID: 8006 RVA: 0x000A2404 File Offset: 0x000A0604
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

		// Token: 0x1700046B RID: 1131
		// (get) Token: 0x06001F47 RID: 8007 RVA: 0x000A2443 File Offset: 0x000A0643
		public static Vector6d zero
		{
			get
			{
				return new Vector6d(0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
			}
		}

		// Token: 0x1700046C RID: 1132
		// (get) Token: 0x06001F48 RID: 8008 RVA: 0x000A2480 File Offset: 0x000A0680
		public static Vector6d one
		{
			get
			{
				return new Vector6d(1.0, 1.0, 1.0, 1.0, 1.0, 1.0);
			}
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x000A24C0 File Offset: 0x000A06C0
		public Vector6d(double x, double y, double z, double i, double j, double k)
		{
			this.v = new double[6];
			this.v[0] = x;
			this.v[1] = y;
			this.v[2] = z;
			this.v[3] = i;
			this.v[4] = j;
			this.v[5] = k;
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x000A2514 File Offset: 0x000A0714
		public Vector6d(float x, float y, float z, float i, float j, float k)
		{
			this.v = new double[6];
			this.v[0] = (double)x;
			this.v[1] = (double)y;
			this.v[2] = (double)z;
			this.v[3] = (double)i;
			this.v[4] = (double)j;
			this.v[5] = (double)k;
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x000A256C File Offset: 0x000A076C
		public Vector6d(Vector3 v3, Vector3 i3)
		{
			this.v = new double[6];
			this.v[0] = (double)v3.x;
			this.v[1] = (double)v3.y;
			this.v[2] = (double)v3.z;
			this.v[3] = (double)i3.x;
			this.v[4] = (double)i3.y;
			this.v[5] = (double)i3.z;
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x000A25E0 File Offset: 0x000A07E0
		public Vector6d(Vector3d v3, Vector3d i3)
		{
			this.v = new double[6];
			this.v[0] = v3.x;
			this.v[1] = v3.y;
			this.v[2] = v3.z;
			this.v[3] = i3.x;
			this.v[4] = i3.y;
			this.v[5] = i3.z;
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x000A2650 File Offset: 0x000A0850
		public static Vector6d operator +(Vector6d a, Vector6d b)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] + b.v[i];
			}
			return zero;
		}

		// Token: 0x06001F4E RID: 8014 RVA: 0x000A2690 File Offset: 0x000A0890
		public static Vector6d operator -(Vector6d a, Vector6d b)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] - b.v[i];
			}
			return zero;
		}

		// Token: 0x06001F4F RID: 8015 RVA: 0x000A26D0 File Offset: 0x000A08D0
		public static Vector6d operator -(Vector6d a)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = -a.v[i];
			}
			return zero;
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x000A2708 File Offset: 0x000A0908
		public static Vector6d operator *(Vector6d a, double d)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] * d;
			}
			return zero;
		}

		// Token: 0x06001F51 RID: 8017 RVA: 0x000A2744 File Offset: 0x000A0944
		public static Vector6d operator *(double d, Vector6d a)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] * d;
			}
			return zero;
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x000A2780 File Offset: 0x000A0980
		public static Vector6d operator /(Vector6d a, double d)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] / d;
			}
			return zero;
		}

		// Token: 0x06001F53 RID: 8019 RVA: 0x000A27B9 File Offset: 0x000A09B9
		public static bool operator ==(Vector6d lhs, Vector6d rhs)
		{
			return Vector6d.SqrMagnitude(lhs - rhs) < 0.0;
		}

		// Token: 0x06001F54 RID: 8020 RVA: 0x000A27D3 File Offset: 0x000A09D3
		public static bool operator !=(Vector6d lhs, Vector6d rhs)
		{
			return Vector6d.SqrMagnitude(lhs - rhs) >= 0.0;
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x000A27F0 File Offset: 0x000A09F0
		public static Vector6d Lerp(Vector6d from, Vector6d to, double t)
		{
			t = Mathd.Clamp01(t);
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < from.v.Length; i++)
			{
				zero.v[i] = from.v[i] + (to.v[i] - from.v[i]) * t;
			}
			return zero;
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x000A2844 File Offset: 0x000A0A44
		public static Vector6d Slerp(Vector6d from, Vector6d to, double t)
		{
			Vector3 vector = Vector3.Slerp((Vector3)new Vector3d(from.v[0], from.v[1], from.v[2]), (Vector3)new Vector3d(to.v[0], to.v[1], to.v[2]), (float)t);
			Vector3 vector2 = Vector3.Slerp((Vector3)new Vector3d(from.v[3], from.v[4], from.v[5]), (Vector3)new Vector3d(to.v[3], to.v[4], to.v[5]), (float)t);
			return new Vector6d(vector, vector2);
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x000A28EE File Offset: 0x000A0AEE
		public void Set(double new_x, double new_y, double new_z, double new_i, double new_j, double new_k)
		{
			this.v[0] = new_x;
			this.v[1] = new_y;
			this.v[2] = new_z;
			this.v[3] = new_i;
			this.v[4] = new_j;
			this.v[5] = new_k;
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x000A292C File Offset: 0x000A0B2C
		public static Vector6d Scale(Vector6d a, Vector6d b)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < a.v.Length; i++)
			{
				zero.v[i] = a.v[i] * b.v[i];
			}
			return zero;
		}

		// Token: 0x06001F59 RID: 8025 RVA: 0x000A296C File Offset: 0x000A0B6C
		public void Scale(Vector6d scale)
		{
			this.v[0] *= scale.v[0];
			this.v[1] *= scale.v[1];
			this.v[2] *= scale.v[2];
			this.v[3] *= scale.v[3];
			this.v[4] *= scale.v[4];
			this.v[5] *= scale.v[5];
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x000A2A0C File Offset: 0x000A0C0C
		public override int GetHashCode()
		{
			return this.v[0].GetHashCode() ^ (this.v[1].GetHashCode() << 2) ^ (this.v[2].GetHashCode() >> 2) ^ (this.v[3].GetHashCode() >> 4) ^ (this.v[4].GetHashCode() << 4) ^ (this.v[5].GetHashCode() >> 6);
		}

		// Token: 0x06001F5B RID: 8027 RVA: 0x000A2A90 File Offset: 0x000A0C90
		public override bool Equals(object other)
		{
			if (!(other is Vector6d))
			{
				return false;
			}
			Vector6d vector6d = (Vector6d)other;
			for (int i = 0; i < this.v.Length; i++)
			{
				if (!this.v[i].Equals(vector6d.v[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001F5C RID: 8028 RVA: 0x000A2AE0 File Offset: 0x000A0CE0
		public static Vector6d Normalize(Vector6d value)
		{
			double num = Vector6d.Magnitude(value);
			if (num > 9.99999974737875E-06)
			{
				return value / num;
			}
			return Vector6d.zero;
		}

		// Token: 0x06001F5D RID: 8029 RVA: 0x000A2B10 File Offset: 0x000A0D10
		public void Normalize()
		{
			double num = Vector6d.Magnitude(this);
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

		// Token: 0x06001F5E RID: 8030 RVA: 0x000A2B80 File Offset: 0x000A0D80
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
				")"
			});
		}

		// Token: 0x06001F5F RID: 8031 RVA: 0x000A2C50 File Offset: 0x000A0E50
		public static double Dot(Vector6d lhs, Vector6d rhs)
		{
			double num = 0.0;
			for (int i = 0; i < lhs.v.Length; i++)
			{
				num += lhs.v[i] + rhs.v[i];
			}
			return num;
		}

		// Token: 0x06001F60 RID: 8032 RVA: 0x000A2C90 File Offset: 0x000A0E90
		public static Vector6d Project(Vector6d vector, Vector6d onNormal)
		{
			double num = Vector6d.Dot(onNormal, onNormal);
			if (num < 1.40129846432482E-45)
			{
				return Vector6d.zero;
			}
			return onNormal * Vector6d.Dot(vector, onNormal) / num;
		}

		// Token: 0x06001F61 RID: 8033 RVA: 0x000A2CCA File Offset: 0x000A0ECA
		public static Vector6d Exclude(Vector6d excludeThis, Vector6d fromThat)
		{
			return fromThat - Vector6d.Project(fromThat, excludeThis);
		}

		// Token: 0x06001F62 RID: 8034 RVA: 0x000A2CD9 File Offset: 0x000A0ED9
		public static double Angle(Vector6d from, Vector6d to)
		{
			return Mathd.Acos(Mathd.Clamp(Vector6d.Dot(from.normalized, to.normalized), -1.0, 1.0)) * 57.29578;
		}

		// Token: 0x06001F63 RID: 8035 RVA: 0x000A2D14 File Offset: 0x000A0F14
		public static double Distance(Vector6d a, Vector6d b)
		{
			return (a - b).magnitude;
		}

		// Token: 0x06001F64 RID: 8036 RVA: 0x000A2D30 File Offset: 0x000A0F30
		public static Vector6d ClampMagnitude(Vector6d vector, double maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}

		// Token: 0x06001F65 RID: 8037 RVA: 0x000A2D4D File Offset: 0x000A0F4D
		public static double Magnitude(Vector6d a)
		{
			return Math.Sqrt(a.sqrMagnitude);
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x000A2D5C File Offset: 0x000A0F5C
		public static double SqrMagnitude(Vector6d a)
		{
			double num = 0.0;
			for (int i = 0; i < a.v.Length; i++)
			{
				num += a.v[i] + a.v[i];
			}
			return num;
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x000A2D9C File Offset: 0x000A0F9C
		public static Vector6d Min(Vector6d lhs, Vector6d rhs)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < lhs.v.Length; i++)
			{
				zero.v[i] += Mathd.Min(lhs.v[i], rhs.v[i]);
			}
			return zero;
		}

		// Token: 0x06001F68 RID: 8040 RVA: 0x000A2DE8 File Offset: 0x000A0FE8
		public static Vector6d Max(Vector6d lhs, Vector6d rhs)
		{
			Vector6d zero = Vector6d.zero;
			for (int i = 0; i < lhs.v.Length; i++)
			{
				zero.v[i] += Mathd.Max(lhs.v[i], rhs.v[i]);
			}
			return zero;
		}

		// Token: 0x06001F69 RID: 8041 RVA: 0x000A2E34 File Offset: 0x000A1034
		[Obsolete("Use Vector6d.Angle instead. AngleBetween uses radians instead of degrees and was deprecated for this reason")]
		public static double AngleBetween(Vector6d from, Vector6d to)
		{
			return Mathd.Acos(Mathd.Clamp(Vector6d.Dot(from.normalized, to.normalized), -1.0, 1.0));
		}

		// Token: 0x0400181F RID: 6175
		public double[] v;

		// Token: 0x04001820 RID: 6176
		public const float kEpsilon = 1E-05f;
	}
}
