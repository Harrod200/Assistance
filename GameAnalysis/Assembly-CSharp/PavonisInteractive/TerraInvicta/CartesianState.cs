using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007EF RID: 2031
	public struct CartesianState
	{
		// Token: 0x17000E80 RID: 3712
		// (get) Token: 0x060048F0 RID: 18672 RVA: 0x001DFBA6 File Offset: 0x001DDDA6
		public Vector3d positionDisplay
		{
			get
			{
				return this.position.xzy;
			}
		}

		// Token: 0x17000E81 RID: 3713
		// (get) Token: 0x060048F1 RID: 18673 RVA: 0x001DFBB3 File Offset: 0x001DDDB3
		public Vector3d velocityDisplay
		{
			get
			{
				return this.velocity.xzy;
			}
		}

		// Token: 0x17000E82 RID: 3714
		// (get) Token: 0x060048F2 RID: 18674 RVA: 0x001DFBC0 File Offset: 0x001DDDC0
		public CartesianState xzy
		{
			get
			{
				return new CartesianState(this.position.xzy, this.velocity.xzy);
			}
		}

		// Token: 0x17000E83 RID: 3715
		// (get) Token: 0x060048F3 RID: 18675 RVA: 0x001DFBDD File Offset: 0x001DDDDD
		public static CartesianState zero
		{
			get
			{
				return new CartesianState(Vector3d.zero, Vector3d.zero);
			}
		}

		// Token: 0x060048F4 RID: 18676 RVA: 0x001DFBEE File Offset: 0x001DDDEE
		public CartesianState(Vector3d position, Vector3d velocity)
		{
			this.position = position;
			this.velocity = velocity;
		}

		// Token: 0x060048F5 RID: 18677 RVA: 0x001DFBFE File Offset: 0x001DDDFE
		public CartesianState(CartesianState a)
		{
			this.position = a.position;
			this.velocity = a.velocity;
		}

		// Token: 0x060048F6 RID: 18678 RVA: 0x001DFC18 File Offset: 0x001DDE18
		public static CartesianState operator +(CartesianState a, CartesianState b)
		{
			return new CartesianState(a.position + b.position, a.velocity + b.velocity);
		}

		// Token: 0x060048F7 RID: 18679 RVA: 0x001DFC41 File Offset: 0x001DDE41
		public static CartesianState operator -(CartesianState a, CartesianState b)
		{
			return new CartesianState(a.position - b.position, a.velocity - b.velocity);
		}

		// Token: 0x060048F8 RID: 18680 RVA: 0x001DFC6A File Offset: 0x001DDE6A
		public static CartesianState operator *(Quaterniond q, CartesianState a)
		{
			return new CartesianState(q * a.position, q * a.velocity);
		}

		// Token: 0x060048F9 RID: 18681 RVA: 0x001DFC89 File Offset: 0x001DDE89
		public static CartesianState operator *(CartesianState a, Quaterniond q)
		{
			return q * a;
		}

		// Token: 0x060048FA RID: 18682 RVA: 0x001DFC94 File Offset: 0x001DDE94
		public CartesianState ChangeReferenceFrame(TINaturalSpaceObjectState oldBarycenter, TISpaceObjectState newBarycenter, TIDateTime time)
		{
			if (oldBarycenter == newBarycenter)
			{
				return this;
			}
			return this.ToGlobal(oldBarycenter, time).ToLocal(newBarycenter, time);
		}

		// Token: 0x060048FB RID: 18683 RVA: 0x001DFCC4 File Offset: 0x001DDEC4
		public CartesianState ToGlobal(TISpaceObjectState oldBarycenter, TIDateTime time)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			return (oldBarycenter.SpatialRotation * this.xzy).xzy + oldBarycenter.ToGlobalCartesianStateAtTime(time);
		}

		// Token: 0x060048FC RID: 18684 RVA: 0x001DFD08 File Offset: 0x001DDF08
		public CartesianState ToLocal(TISpaceObjectState newBarycenter, TIDateTime time)
		{
			if (time == null)
			{
				time = TITimeState.Now();
			}
			CartesianState cartesianState = this - newBarycenter.ToGlobalCartesianStateAtTime(time);
			return (Quaterniond.Inverse(newBarycenter.SpatialRotation) * cartesianState.xzy).xzy;
		}

		// Token: 0x060048FD RID: 18685 RVA: 0x001DFD5C File Offset: 0x001DDF5C
		public OrbitalElementsState ToOrbitalElementsState(double mu_barycenter, DateTime? dateTime = null)
		{
			DateTime dateTime2 = TITimeState.SystemNow();
			if (dateTime != null)
			{
				dateTime2 = dateTime.Value;
			}
			Vector3d vector3d = Vector3d.Cross(this.position, this.velocity);
			Vector3d vector3d2 = Vector3d.Cross(this.velocity, vector3d) / mu_barycenter - this.position.normalized;
			double magnitude = vector3d2.magnitude;
			double num = 1.0 / (2.0 / this.position.magnitude - this.velocity.sqrMagnitude / mu_barycenter);
			double num2 = Mathd.Acos(vector3d.z / vector3d.magnitude);
			Vector3d vector3d3 = new Vector3d(-vector3d.y, vector3d.x, 0.0);
			double num3;
			double num4;
			if (vector3d3.sqrMagnitude == 0.0)
			{
				num3 = 0.0;
				num4 = Mathd.Atan2(vector3d2.y, vector3d2.x);
				if (vector3d.z < 0.0)
				{
					num4 = 6.283185307179586 - num4;
				}
			}
			else
			{
				num3 = Mathd.Acos(vector3d3.x / vector3d3.magnitude);
				if (vector3d3.y < 0.0)
				{
					num3 = 6.283185307179586 - num3;
				}
				if (magnitude == 0.0)
				{
					num4 = 0.0;
				}
				else
				{
					num4 = Mathd.Acos(Vector3d.Dot(in vector3d3, in vector3d2) / (vector3d3.magnitude * vector3d2.magnitude));
					if (vector3d2.z < 0.0)
					{
						num4 = 6.283185307179586 - num4;
					}
				}
			}
			double num7;
			if (Mathd.Approximately(magnitude, 1.0))
			{
				double num5 = num * (1.0 - magnitude * magnitude);
				double num6 = Mathd.Tan((6.283185307179586 - Mathd.Acos(Math.Round((num5 - this.position.magnitude) / (magnitude * this.position.magnitude), 5))) / 2.0);
				num7 = num6 + Mathd.Pow(num6, 3.0) / 3.0;
			}
			else if (magnitude == 0.0)
			{
				Vector3d vector3d4 = new Vector3d(Mathd.Cos(num3), Mathd.Sin(num3), 0.0);
				Vector3d vector3d5 = this.position.normalized;
				num7 = Mathd.Acos(Vector3d.Dot(in vector3d5, in vector3d4));
				vector3d5 = Vector3d.Cross(vector3d4, this.position);
				if (Vector3d.Dot(in vector3d5, in vector3d) < 0.0)
				{
					num7 = 6.283185307179586 - num7;
				}
			}
			else if (magnitude < 1.0)
			{
				double num8 = Mathd.Acos(Mathd.Clamp(Vector3d.Dot(in vector3d2, in this.position) / (magnitude * this.position.magnitude), -1.0, 1.0));
				if (Vector3d.Dot(in this.position, in this.velocity) < 0.0)
				{
					num8 = 6.283185307179586 - num8;
				}
				double num9 = 2.0 * Mathd.Atan2(Mathd.Tan(num8 / 2.0), Mathd.Sqrt((1.0 + magnitude) / (1.0 - magnitude)));
				num7 = num9 - magnitude * Mathd.Sin(num9);
			}
			else
			{
				double num10 = num * (1.0 - magnitude * magnitude);
				bool flag = Vector3d.Dot(in this.velocity, in this.position) > 0.0;
				double num11 = Mathd.Acos((num10 - this.position.magnitude) / (magnitude * this.position.magnitude));
				if (double.IsNaN(num11))
				{
					num11 = 0.0;
				}
				if (!flag)
				{
					num11 = 6.283185307179586 - num11;
				}
				double num12 = Mathd.Cos(num11);
				double num13 = Mathd.ACosh((magnitude + num12) / (1.0 + magnitude * num12));
				if (double.IsNaN(num13))
				{
					Log.Warn("Eccentric Anomaly is not a number in hyperbolic orbit", Array.Empty<object>());
				}
				num7 = magnitude * Mathd.Sinh(num13) - num13;
				if (flag ^ (num7 > 0.0))
				{
					num7 = -num7;
				}
			}
			return new OrbitalElementsState(num3, num4, num2, num, magnitude, num7, dateTime2);
		}

		// Token: 0x04002ACF RID: 10959
		public Vector3d position;

		// Token: 0x04002AD0 RID: 10960
		public Vector3d velocity;
	}
}
