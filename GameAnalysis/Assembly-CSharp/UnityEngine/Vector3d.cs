using System;
using Unity.Burst;

namespace UnityEngine
{
	// Token: 0x020004F5 RID: 1269
	[BurstCompile]
	[Serializable]
	public struct Vector3d
	{
		// Token: 0x06001F09 RID: 7945 RVA: 0x000A17E9 File Offset: 0x0009F9E9
		[BurstDiscard]
		public static explicit operator Vector3(Vector3d v)
		{
			return new Vector3((float)v.x, (float)v.y, (float)v.z);
		}

		// Token: 0x06001F0A RID: 7946 RVA: 0x000A1805 File Offset: 0x0009FA05
		[BurstDiscard]
		public static implicit operator Vector3d(Vector3 v)
		{
			return new Vector3d(v.x, v.y, v.z);
		}

		// Token: 0x17000462 RID: 1122
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
				default:
					throw new IndexOutOfRangeException("Invalid Vector3d index!");
				}
			}
		}

		// Token: 0x17000463 RID: 1123
		// (get) Token: 0x06001F0D RID: 7949 RVA: 0x000A189B File Offset: 0x0009FA9B
		public readonly Vector3d xzy
		{
			get
			{
				return Vector3d.SwapYZ(this);
			}
		}

		// Token: 0x17000464 RID: 1124
		// (get) Token: 0x06001F0E RID: 7950 RVA: 0x000A18A8 File Offset: 0x0009FAA8
		public readonly Vector3d normalized
		{
			get
			{
				return Vector3d.Normalize(this);
			}
		}

		// Token: 0x17000465 RID: 1125
		// (get) Token: 0x06001F0F RID: 7951 RVA: 0x000A18B5 File Offset: 0x0009FAB5
		public readonly double magnitude
		{
			get
			{
				return Vector3d.Magnitude(in this);
			}
		}

		// Token: 0x17000466 RID: 1126
		// (get) Token: 0x06001F10 RID: 7952 RVA: 0x000A18BD File Offset: 0x0009FABD
		public readonly double sqrMagnitude
		{
			get
			{
				return Vector3d.SqrMagnitude(in this);
			}
		}

		// Token: 0x06001F11 RID: 7953 RVA: 0x000A18C5 File Offset: 0x0009FAC5
		public Vector3d(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		// Token: 0x06001F12 RID: 7954 RVA: 0x000A18DC File Offset: 0x0009FADC
		public Vector3d(float x, float y, float z)
		{
			this.x = (double)x;
			this.y = (double)y;
			this.z = (double)z;
		}

		// Token: 0x06001F13 RID: 7955 RVA: 0x000A18F6 File Offset: 0x0009FAF6
		public Vector3d(Vector3d v3)
		{
			this.x = v3.x;
			this.y = v3.y;
			this.z = v3.z;
		}

		// Token: 0x06001F14 RID: 7956 RVA: 0x000A191C File Offset: 0x0009FB1C
		public Vector3d(double x, double y)
		{
			this.x = x;
			this.y = y;
			this.z = 0.0;
		}

		// Token: 0x06001F15 RID: 7957 RVA: 0x000A193B File Offset: 0x0009FB3B
		public static Vector3d operator +(Vector3d a, Vector3d b)
		{
			return new Vector3d(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		// Token: 0x06001F16 RID: 7958 RVA: 0x000A1969 File Offset: 0x0009FB69
		public static Vector3d operator -(Vector3d a, Vector3d b)
		{
			return new Vector3d(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		// Token: 0x06001F17 RID: 7959 RVA: 0x000A1997 File Offset: 0x0009FB97
		public static Vector3d operator -(Vector3d a)
		{
			return new Vector3d(-a.x, -a.y, -a.z);
		}

		// Token: 0x06001F18 RID: 7960 RVA: 0x000A19B3 File Offset: 0x0009FBB3
		public static Vector3d operator *(Vector3d a, double d)
		{
			return new Vector3d(a.x * d, a.y * d, a.z * d);
		}

		// Token: 0x06001F19 RID: 7961 RVA: 0x000A19D2 File Offset: 0x0009FBD2
		public static Vector3d operator *(double d, Vector3d a)
		{
			return new Vector3d(a.x * d, a.y * d, a.z * d);
		}

		// Token: 0x06001F1A RID: 7962 RVA: 0x000A19F1 File Offset: 0x0009FBF1
		public static Vector3d operator *(Quaternion q, Vector3d v)
		{
			return new Quaterniond(q) * v;
		}

		// Token: 0x06001F1B RID: 7963 RVA: 0x000A1A04 File Offset: 0x0009FC04
		public static Vector3d operator /(Vector3d a, double d)
		{
			return new Vector3d(a.x / d, a.y / d, a.z / d);
		}

		// Token: 0x06001F1C RID: 7964 RVA: 0x000A1A24 File Offset: 0x0009FC24
		[BurstCompile]
		public static bool operator ==(in Vector3d lhs, in Vector3d rhs)
		{
			return Mathd.Abs(lhs.x - rhs.x) < 9.999999747378752E-06 && Mathd.Abs(lhs.y - rhs.y) < 9.999999747378752E-06 && Mathd.Abs(lhs.z - rhs.z) < 9.999999747378752E-06;
		}

		// Token: 0x06001F1D RID: 7965 RVA: 0x000A1A8A File Offset: 0x0009FC8A
		[BurstCompile]
		public static bool operator !=(in Vector3d lhs, in Vector3d rhs)
		{
			return !((in lhs) == (in rhs));
		}

		// Token: 0x06001F1E RID: 7966 RVA: 0x000A1A98 File Offset: 0x0009FC98
		public override readonly bool Equals(object other)
		{
			if (other is Vector3d)
			{
				Vector3d vector3d = (Vector3d)other;
				return (in this) == (in vector3d);
			}
			return false;
		}

		// Token: 0x06001F1F RID: 7967 RVA: 0x000A1AC0 File Offset: 0x0009FCC0
		public override readonly int GetHashCode()
		{
			double num = this.x;
			int hashCode = num.GetHashCode();
			num = this.y;
			int num2 = hashCode ^ (num.GetHashCode() << 2);
			num = this.z;
			return num2 ^ (num.GetHashCode() >> 2);
		}

		// Token: 0x06001F20 RID: 7968 RVA: 0x000A1AFD File Offset: 0x0009FCFD
		public void Set(double new_x, double new_y, double new_z)
		{
			this.x = new_x;
			this.y = new_y;
			this.z = new_z;
		}

		// Token: 0x06001F21 RID: 7969 RVA: 0x000A1B14 File Offset: 0x0009FD14
		public static Vector3d Lerp(Vector3d from, Vector3d to, double t)
		{
			t = Mathd.Clamp01(t);
			return new Vector3d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
		}

		// Token: 0x06001F22 RID: 7970 RVA: 0x000A1B70 File Offset: 0x0009FD70
		public static Vector3d MoveTowards(Vector3d current, Vector3d target, double maxDistanceDelta)
		{
			Vector3d vector3d = target - current;
			double magnitude = vector3d.magnitude;
			if (magnitude <= maxDistanceDelta || magnitude == 0.0)
			{
				return target;
			}
			return current + vector3d / magnitude * maxDistanceDelta;
		}

		// Token: 0x06001F23 RID: 7971 RVA: 0x000A1BB2 File Offset: 0x0009FDB2
		public static Vector3d Scale(Vector3d a, Vector3d b)
		{
			return new Vector3d(a.x * b.x, a.y * b.y, a.z * b.z);
		}

		// Token: 0x06001F24 RID: 7972 RVA: 0x000A1BE0 File Offset: 0x0009FDE0
		public void Scale(Vector3d scale)
		{
			this.x *= scale.x;
			this.y *= scale.y;
			this.z *= scale.z;
		}

		// Token: 0x06001F25 RID: 7973 RVA: 0x000A1C1C File Offset: 0x0009FE1C
		public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed)
		{
			double num = (double)Time.deltaTime;
			return Vector3d.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, num);
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x000A1C3C File Offset: 0x0009FE3C
		public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime)
		{
			double num = (double)Time.deltaTime;
			double positiveInfinity = double.PositiveInfinity;
			return Vector3d.SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, num);
		}

		// Token: 0x06001F27 RID: 7975 RVA: 0x000A1C68 File Offset: 0x0009FE68
		public static Vector3d SmoothDamp(Vector3d current, Vector3d target, ref Vector3d currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
		{
			smoothTime = Mathd.Max(0.0001, smoothTime);
			double num = 2.0 / smoothTime;
			double num2 = num * deltaTime;
			double num3 = 1.0 / (1.0 + num2 + 0.479999989271164 * num2 * num2 + 0.234999999403954 * num2 * num2 * num2);
			Vector3d vector3d = current - target;
			Vector3d vector3d2 = target;
			double num4 = maxSpeed * smoothTime;
			Vector3d vector3d3 = Vector3d.ClampMagnitude(vector3d, num4);
			target = current - vector3d3;
			Vector3d vector3d4 = (currentVelocity + num * vector3d3) * deltaTime;
			currentVelocity = (currentVelocity - num * vector3d4) * num3;
			Vector3d vector3d5 = target + (vector3d3 + vector3d4) * num3;
			Vector3d vector3d6 = vector3d2 - current;
			Vector3d vector3d7 = vector3d5 - vector3d2;
			if (Vector3d.Dot(in vector3d6, in vector3d7) > 0.0)
			{
				vector3d5 = vector3d2;
				currentVelocity = (vector3d5 - vector3d2) / deltaTime;
			}
			return vector3d5;
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x000A1D80 File Offset: 0x0009FF80
		public static Vector3d Cross(Vector3d lhs, Vector3d rhs)
		{
			return new Vector3d(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
		}

		// Token: 0x06001F29 RID: 7977 RVA: 0x000A1DE3 File Offset: 0x0009FFE3
		[BurstCompile]
		public static double Dot(in Vector3d lhs, in Vector3d rhs)
		{
			return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
		}

		// Token: 0x06001F2A RID: 7978 RVA: 0x000A1E10 File Offset: 0x000A0010
		public static Vector3d Normalize(Vector3d v)
		{
			double num = Vector3d.Magnitude(in v);
			if (num <= 9.999999747378752E-06)
			{
				return Vector3d.zero;
			}
			return v / num;
		}

		// Token: 0x06001F2B RID: 7979 RVA: 0x000A1E3E File Offset: 0x000A003E
		public void Normalize()
		{
			this = Vector3d.Normalize(this);
		}

		// Token: 0x06001F2C RID: 7980 RVA: 0x000A1E51 File Offset: 0x000A0051
		public static Vector3d Reflect(Vector3d inDirection, Vector3d inNormal)
		{
			return -2.0 * Vector3d.Dot(in inNormal, in inDirection) * inNormal + inDirection;
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x000A1E74 File Offset: 0x000A0074
		public static Vector3d Project(Vector3d vector, Vector3d onNormal)
		{
			double num = Vector3d.Dot(in onNormal, in onNormal);
			if (num < 9.999999747378752E-06)
			{
				return Vector3d.zero;
			}
			return onNormal * Vector3d.Dot(in vector, in onNormal) / num;
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x000A1EB2 File Offset: 0x000A00B2
		public static Vector3d Exclude(Vector3d excludeThis, Vector3d fromThat)
		{
			return fromThat - Vector3d.Project(fromThat, excludeThis);
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x000A1EC4 File Offset: 0x000A00C4
		[BurstCompile]
		public static double Angle(in Vector3d from, in Vector3d to)
		{
			Vector3d normalized = from.normalized;
			Vector3d normalized2 = to.normalized;
			return Mathd.Acos(Mathd.Clamp(Vector3d.Dot(in normalized, in normalized2), -1.0, 1.0)) * 57.29577951308232;
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x000A1F10 File Offset: 0x000A0110
		[BurstCompile]
		public static double SignedAngle(in Vector3d from, in Vector3d to, in Vector3d axis)
		{
			double num = Vector3d.Angle(in from, in to);
			Vector3d vector3d = Vector3d.Cross(from.normalized, to.normalized);
			return num * Mathd.Sign(Vector3d.Dot(in axis, in vector3d));
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x000A1F44 File Offset: 0x000A0144
		[BurstCompile]
		public static double Distance(in Vector3d a, in Vector3d b)
		{
			Vector3d vector3d = a - b;
			return Vector3d.Magnitude(in vector3d);
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x000A1F6A File Offset: 0x000A016A
		public static Vector3d ClampMagnitude(Vector3d vector, double maxLength)
		{
			if (vector.sqrMagnitude > maxLength * maxLength)
			{
				return vector.normalized * maxLength;
			}
			return vector;
		}

		// Token: 0x06001F33 RID: 7987 RVA: 0x000A1F87 File Offset: 0x000A0187
		[BurstCompile]
		public static double Magnitude(in Vector3d a)
		{
			return Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x000A1FB7 File Offset: 0x000A01B7
		[BurstCompile]
		public static double SqrMagnitude(in Vector3d a)
		{
			return a.x * a.x + a.y * a.y + a.z * a.z;
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x000A1FE2 File Offset: 0x000A01E2
		public static Vector3d Min(Vector3d lhs, Vector3d rhs)
		{
			return new Vector3d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y), Mathd.Min(lhs.z, rhs.z));
		}

		// Token: 0x06001F36 RID: 7990 RVA: 0x000A201C File Offset: 0x000A021C
		public static Vector3d Max(Vector3d lhs, Vector3d rhs)
		{
			return new Vector3d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y), Mathd.Max(lhs.z, rhs.z));
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x000A2058 File Offset: 0x000A0258
		public static Vector3d RotateAround(Vector3d vector, Vector3d axis, double radians)
		{
			Vector3d vector3d = Vector3d.Normalize(axis);
			double num = Mathd.Sin(radians);
			double num2 = Mathd.Cos(radians);
			return vector * num2 + Vector3d.Cross(vector3d, vector) * num + axis * (Vector3d.Dot(in vector3d, in vector) * (1.0 - num2));
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x000A20B3 File Offset: 0x000A02B3
		public static Vector3d Flatten(Vector3d vector, Vector3d normalVector)
		{
			if (normalVector.sqrMagnitude != 1.0)
			{
				normalVector = normalVector.normalized;
			}
			return vector - Vector3d.Dot(in vector, in normalVector) * normalVector;
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x000A20E5 File Offset: 0x000A02E5
		public static Vector3d SwapYZ(Vector3d v)
		{
			return new Vector3d(v.x, v.z, v.y);
		}

		// Token: 0x06001F3A RID: 7994 RVA: 0x000A20FE File Offset: 0x000A02FE
		[BurstCompile]
		public static bool Approximately(in Vector3d a, in Vector3d b)
		{
			return Mathd.Approximately(a.x, b.x) && Mathd.Approximately(a.y, b.y) && Mathd.Approximately(a.z, b.z);
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x000A213C File Offset: 0x000A033C
		public override readonly string ToString()
		{
			return string.Format("x {0}, y {1}, z {2}, m {3}", new object[] { this.x, this.y, this.z, this.magnitude });
		}

		// Token: 0x06001F3C RID: 7996 RVA: 0x000A2191 File Offset: 0x000A0391
		[BurstDiscard]
		public static Vector3d FromVector3(Vector3 v)
		{
			return new Vector3d(v.x, v.y, v.z);
		}

		// Token: 0x06001F3D RID: 7997 RVA: 0x000A21AA File Offset: 0x000A03AA
		[BurstDiscard]
		public static Vector3d Slerp(Vector3d a, Vector3d b, double t)
		{
			return Vector3.Slerp((Vector3)a, (Vector3)b, (float)t);
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x000A21C4 File Offset: 0x000A03C4
		[BurstDiscard]
		public static Vector3d RotateTowards(Vector3d a, Vector3d b, double maxRadians, double maxMag)
		{
			return Vector3.RotateTowards((Vector3)a, (Vector3)b, (float)maxRadians, (float)maxMag);
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x000A21E0 File Offset: 0x000A03E0
		[BurstDiscard]
		public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent)
		{
			Vector3 vector = (Vector3)normal;
			Vector3 vector2 = (Vector3)tangent;
			Vector3.OrthoNormalize(ref vector, ref vector2);
			normal = new Vector3d(vector);
			tangent = new Vector3d(vector2);
		}

		// Token: 0x06001F40 RID: 8000 RVA: 0x000A2230 File Offset: 0x000A0430
		[BurstDiscard]
		public static void OrthoNormalize(ref Vector3d normal, ref Vector3d tangent, ref Vector3d binormal)
		{
			Vector3 vector = (Vector3)normal;
			Vector3 vector2 = (Vector3)tangent;
			Vector3 vector3 = (Vector3)binormal;
			Vector3.OrthoNormalize(ref vector, ref vector2, ref vector3);
			normal = new Vector3d(vector);
			tangent = new Vector3d(vector2);
			binormal = new Vector3d(vector3);
		}

		// Token: 0x04001813 RID: 6163
		public const double kEpsilon = 9.999999747378752E-06;

		// Token: 0x04001814 RID: 6164
		public double x;

		// Token: 0x04001815 RID: 6165
		public double y;

		// Token: 0x04001816 RID: 6166
		public double z;

		// Token: 0x04001817 RID: 6167
		public static readonly Vector3d zero = new Vector3d(0.0, 0.0, 0.0);

		// Token: 0x04001818 RID: 6168
		public static readonly Vector3d one = new Vector3d(1.0, 1.0, 1.0);

		// Token: 0x04001819 RID: 6169
		public static readonly Vector3d forward = new Vector3d(0.0, 0.0, 1.0);

		// Token: 0x0400181A RID: 6170
		public static readonly Vector3d back = new Vector3d(0.0, 0.0, -1.0);

		// Token: 0x0400181B RID: 6171
		public static readonly Vector3d up = new Vector3d(0.0, 1.0, 0.0);

		// Token: 0x0400181C RID: 6172
		public static readonly Vector3d down = new Vector3d(0.0, -1.0, 0.0);

		// Token: 0x0400181D RID: 6173
		public static readonly Vector3d left = new Vector3d(-1.0, 0.0, 0.0);

		// Token: 0x0400181E RID: 6174
		public static readonly Vector3d right = new Vector3d(1.0, 0.0, 0.0);
	}
}
