using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Components
{
	// Token: 0x020009D2 RID: 2514
	[Obsolete("Use Vector3d instead")]
	[Serializable]
	public struct TIDouble3 : IEquatable<TIDouble3>
	{
		// Token: 0x06005E44 RID: 24132 RVA: 0x002CD30A File Offset: 0x002CB50A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TIDouble3(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		// Token: 0x17001034 RID: 4148
		public double this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return this.x;
				case 1:
					return this.y;
				case 2:
					return this.z;
				default:
					throw new ArgumentOutOfRangeException("index", index, "Range 0..2");
				}
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
					throw new ArgumentOutOfRangeException("index", index, "Range 0..2");
				}
			}
		}

		// Token: 0x17001035 RID: 4149
		// (get) Token: 0x06005E47 RID: 24135 RVA: 0x002CD3B2 File Offset: 0x002CB5B2
		public TIDouble3 xzy
		{
			get
			{
				return new TIDouble3(this.x, this.z, this.y);
			}
		}

		// Token: 0x17001036 RID: 4150
		// (get) Token: 0x06005E48 RID: 24136 RVA: 0x002CD3CC File Offset: 0x002CB5CC
		public double Magnitude
		{
			get
			{
				Vector3d vector3d = this;
				return Vector3d.Magnitude(in vector3d);
			}
		}

		// Token: 0x17001037 RID: 4151
		// (get) Token: 0x06005E49 RID: 24137 RVA: 0x002CD3EC File Offset: 0x002CB5EC
		public TIDouble3 Direction
		{
			get
			{
				return Vector3d.Normalize(this);
			}
		}

		// Token: 0x06005E4A RID: 24138 RVA: 0x002CD403 File Offset: 0x002CB603
		public override int GetHashCode()
		{
			return (((this.x.GetHashCode() * 397) ^ this.y.GetHashCode()) * 397) ^ this.z.GetHashCode();
		}

		// Token: 0x06005E4B RID: 24139 RVA: 0x002CD434 File Offset: 0x002CB634
		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((TIDouble3)obj)));
		}

		// Token: 0x06005E4C RID: 24140 RVA: 0x002CD484 File Offset: 0x002CB684
		public override string ToString()
		{
			return string.Format("double3({0:#,#0.###}, {1:#,#0.###}, {2:#,#0.###}) mag: {3:#,#0.###}", new object[] { this.x, this.y, this.z, this.Magnitude });
		}

		// Token: 0x06005E4D RID: 24141 RVA: 0x002CD4DC File Offset: 0x002CB6DC
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return string.Concat(new string[]
			{
				"double3(",
				this.x.ToString(format, formatProvider),
				"f, ",
				this.y.ToString(format, formatProvider),
				"f, ",
				this.z.ToString(format, formatProvider),
				"f)"
			});
		}

		// Token: 0x06005E4E RID: 24142 RVA: 0x002CD544 File Offset: 0x002CB744
		public bool Equals(TIDouble3 other)
		{
			return this.x == other.x && this.y == other.y && this.z == other.z;
		}

		// Token: 0x06005E4F RID: 24143 RVA: 0x002CD572 File Offset: 0x002CB772
		public static bool operator ==(TIDouble3 left, TIDouble3 right)
		{
			return left.Equals(right);
		}

		// Token: 0x06005E50 RID: 24144 RVA: 0x002CD57C File Offset: 0x002CB77C
		public static bool operator !=(TIDouble3 left, TIDouble3 right)
		{
			return !left.Equals(right);
		}

		// Token: 0x06005E51 RID: 24145 RVA: 0x002CD589 File Offset: 0x002CB789
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator *(TIDouble3 lhs, TIDouble3 rhs)
		{
			return new TIDouble3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
		}

		// Token: 0x06005E52 RID: 24146 RVA: 0x002CD5B7 File Offset: 0x002CB7B7
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator *(TIDouble3 lhs, double rhs)
		{
			return new TIDouble3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x002CD5D6 File Offset: 0x002CB7D6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator +(TIDouble3 lhs, TIDouble3 rhs)
		{
			return new TIDouble3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
		}

		// Token: 0x06005E54 RID: 24148 RVA: 0x002CD604 File Offset: 0x002CB804
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator +(TIDouble3 lhs, double rhs)
		{
			return new TIDouble3(lhs.x + rhs, lhs.y + rhs, lhs.z + rhs);
		}

		// Token: 0x06005E55 RID: 24149 RVA: 0x002CD623 File Offset: 0x002CB823
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator -(TIDouble3 lhs, TIDouble3 rhs)
		{
			return new TIDouble3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
		}

		// Token: 0x06005E56 RID: 24150 RVA: 0x002CD651 File Offset: 0x002CB851
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator -(TIDouble3 lhs, double rhs)
		{
			return new TIDouble3(lhs.x - rhs, lhs.y - rhs, lhs.z - rhs);
		}

		// Token: 0x06005E57 RID: 24151 RVA: 0x002CD670 File Offset: 0x002CB870
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator /(TIDouble3 lhs, TIDouble3 rhs)
		{
			return new TIDouble3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
		}

		// Token: 0x06005E58 RID: 24152 RVA: 0x002CD69E File Offset: 0x002CB89E
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator /(TIDouble3 lhs, double rhs)
		{
			return new TIDouble3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
		}

		// Token: 0x06005E59 RID: 24153 RVA: 0x002CD6BD File Offset: 0x002CB8BD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator -(TIDouble3 val)
		{
			return new TIDouble3(-val.x, -val.y, -val.z);
		}

		// Token: 0x06005E5A RID: 24154 RVA: 0x002CD6D9 File Offset: 0x002CB8D9
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TIDouble3 operator +(TIDouble3 val)
		{
			return new TIDouble3(val.x, val.y, val.z);
		}

		// Token: 0x06005E5B RID: 24155 RVA: 0x002CD6F2 File Offset: 0x002CB8F2
		public static implicit operator Vector3d(TIDouble3 d)
		{
			return new Vector3d(d.x, d.y, d.z);
		}

		// Token: 0x06005E5C RID: 24156 RVA: 0x002CD70B File Offset: 0x002CB90B
		public static implicit operator TIDouble3(Vector3d d)
		{
			return new TIDouble3(d.x, d.y, d.z);
		}

		// Token: 0x06005E5D RID: 24157 RVA: 0x002CD724 File Offset: 0x002CB924
		public static implicit operator TIDouble3(Vector3 d)
		{
			return new TIDouble3((double)d.x, (double)d.y, (double)d.z);
		}

		// Token: 0x06005E5E RID: 24158 RVA: 0x002CD740 File Offset: 0x002CB940
		public static implicit operator TIDouble3(float3 d)
		{
			return new TIDouble3((double)d.x, (double)d.y, (double)d.z);
		}

		// Token: 0x06005E5F RID: 24159 RVA: 0x002CD75C File Offset: 0x002CB95C
		public static explicit operator Vector3(TIDouble3 d)
		{
			return new Vector3((float)d.x, (float)d.y, (float)d.z);
		}

		// Token: 0x06005E60 RID: 24160 RVA: 0x002CD778 File Offset: 0x002CB978
		public static explicit operator float3(TIDouble3 d)
		{
			return new Vector3((float)d.x, (float)d.y, (float)d.z);
		}

		// Token: 0x04004374 RID: 17268
		public double x;

		// Token: 0x04004375 RID: 17269
		public double y;

		// Token: 0x04004376 RID: 17270
		public double z;
	}
}
