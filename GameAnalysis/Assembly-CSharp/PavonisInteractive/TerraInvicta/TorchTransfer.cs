using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007C7 RID: 1991
	public class TorchTransfer : TrajectorySolver
	{
		// Token: 0x17000DEF RID: 3567
		// (get) Token: 0x060046F2 RID: 18162 RVA: 0x001CFD8B File Offset: 0x001CDF8B
		// (set) Token: 0x060046F3 RID: 18163 RVA: 0x001CFD93 File Offset: 0x001CDF93
		public double accelDuration_s { get; private set; }

		// Token: 0x17000DF0 RID: 3568
		// (get) Token: 0x060046F4 RID: 18164 RVA: 0x001CFD9C File Offset: 0x001CDF9C
		// (set) Token: 0x060046F5 RID: 18165 RVA: 0x001CFDA4 File Offset: 0x001CDFA4
		public double decelDuration_s { get; private set; }

		// Token: 0x17000DF1 RID: 3569
		// (get) Token: 0x060046F6 RID: 18166 RVA: 0x001CFDAD File Offset: 0x001CDFAD
		// (set) Token: 0x060046F7 RID: 18167 RVA: 0x001CFDB5 File Offset: 0x001CDFB5
		public Vector3d accelerationVector_mps2 { get; private set; }

		// Token: 0x17000DF2 RID: 3570
		// (get) Token: 0x060046F8 RID: 18168 RVA: 0x001CFDBE File Offset: 0x001CDFBE
		// (set) Token: 0x060046F9 RID: 18169 RVA: 0x001CFDC6 File Offset: 0x001CDFC6
		public Vector3d decelerationVector_mps2 { get; private set; }

		// Token: 0x17000DF3 RID: 3571
		// (get) Token: 0x060046FA RID: 18170 RVA: 0x001CFDCF File Offset: 0x001CDFCF
		public double coastDuration_s
		{
			get
			{
				return this.transitDuration_s - this.accelDuration_s - this.decelDuration_s;
			}
		}

		// Token: 0x17000DF4 RID: 3572
		// (get) Token: 0x060046FB RID: 18171 RVA: 0x001CFDE5 File Offset: 0x001CDFE5
		public Vector3d coastVelocity_mps
		{
			get
			{
				return this.initialVelocityVector_mps + this.accelDuration_s * this.accelerationVector_mps2;
			}
		}

		// Token: 0x17000DF5 RID: 3573
		// (get) Token: 0x060046FC RID: 18172 RVA: 0x001CFE03 File Offset: 0x001CE003
		// (set) Token: 0x060046FD RID: 18173 RVA: 0x001CFE0B File Offset: 0x001CE00B
		public Vector3d initialVelocityVector_mps { get; private set; }

		// Token: 0x17000DF6 RID: 3574
		// (get) Token: 0x060046FE RID: 18174 RVA: 0x001CFE14 File Offset: 0x001CE014
		// (set) Token: 0x060046FF RID: 18175 RVA: 0x001CFE1C File Offset: 0x001CE01C
		public Vector3d arrivalVelocityVector_mps { get; private set; }

		// Token: 0x17000DF7 RID: 3575
		// (get) Token: 0x06004700 RID: 18176 RVA: 0x001CFE25 File Offset: 0x001CE025
		// (set) Token: 0x06004701 RID: 18177 RVA: 0x001CFE2D File Offset: 0x001CE02D
		public CartesianState initialState { get; private set; }

		// Token: 0x17000DF8 RID: 3576
		// (get) Token: 0x06004702 RID: 18178 RVA: 0x001CFE36 File Offset: 0x001CE036
		// (set) Token: 0x06004703 RID: 18179 RVA: 0x001CFE3E File Offset: 0x001CE03E
		public CartesianState finalState { get; private set; }

		// Token: 0x17000DF9 RID: 3577
		// (get) Token: 0x06004704 RID: 18180 RVA: 0x001CFE47 File Offset: 0x001CE047
		// (set) Token: 0x06004705 RID: 18181 RVA: 0x001CFE4F File Offset: 0x001CE04F
		public TINaturalSpaceObjectState barycenter { get; private set; }

		// Token: 0x06004706 RID: 18182 RVA: 0x001CFE58 File Offset: 0x001CE058
		public double MinDistanceToTrajectory_m(Vector3d v, Vector3d s, Vector3d p)
		{
			double num = (v.x * p.x + v.y * p.y + v.z * p.z - v.x * s.x - v.y * s.y - v.z - s.z) / (v.x * v.x + v.y * v.y + v.z * v.z);
			return new Vector3d(s.x + num * v.x - p.x, s.y + num * v.y - p.y, s.z + num * v.z - p.z).magnitude;
		}

		// Token: 0x06004707 RID: 18183 RVA: 0x001CFF34 File Offset: 0x001CE134
		public TransferResult Solve(TIDateTime startTime, double transitDuration_s, double fleetInitialAcceleration_mps2, ITransferTarget iOrigin, ITransferTarget iDestination, TINaturalSpaceObjectState transferBarycenter, double fleetDV_mps, out bool possible)
		{
			CartesianState cartesianState = iOrigin.relevantGlobalCartesianState(transferBarycenter, startTime);
			CartesianState cartesianState2 = iDestination.relevantGlobalCartesianState(transferBarycenter, new TIDateTime(startTime, transitDuration_s));
			return this.Solve(startTime, transitDuration_s, fleetInitialAcceleration_mps2, cartesianState, cartesianState2, transferBarycenter, fleetDV_mps, out possible, false);
		}

		// Token: 0x06004708 RID: 18184 RVA: 0x001CFF70 File Offset: 0x001CE170
		public TransferResult Solve(TIDateTime startTime, double transitDuration_s, double fleetInitialAcceleration_mps2, CartesianState originGlobalState, CartesianState destinationGlobalState, TINaturalSpaceObjectState transferBarycenter, double fleetDV_mps, out bool possible, bool allowImpossibleTrajectories = false)
		{
			this.launchTime = new TIDateTime(startTime);
			this.arrivalTime = new TIDateTime(startTime);
			this.arrivalTime.AddSeconds(transitDuration_s);
			this.transitDuration_s = transitDuration_s;
			CartesianState cartesianState;
			CartesianState cartesianState2;
			if (transferBarycenter.isSun)
			{
				cartesianState = new CartesianState(originGlobalState);
				cartesianState2 = new CartesianState(destinationGlobalState);
			}
			else
			{
				cartesianState = originGlobalState - transferBarycenter.ToGlobalCartesianStateAtTime(this.launchTime);
				cartesianState = (transferBarycenter.SpatialRotation * cartesianState.xzy).xzy;
				cartesianState2 = destinationGlobalState - transferBarycenter.ToGlobalCartesianStateAtTime(this.arrivalTime);
				cartesianState2 = (transferBarycenter.SpatialRotation * cartesianState2.xzy).xzy;
			}
			this.initialVelocityVector_mps = cartesianState.velocity;
			this.arrivalVelocityVector_mps = cartesianState2.velocity;
			Vector3d vector3d = (cartesianState.velocity + cartesianState2.velocity) / 2.0;
			CartesianState cartesianState3 = new CartesianState(cartesianState.position, cartesianState.velocity - vector3d);
			CartesianState cartesianState4 = new CartesianState(cartesianState2.position - vector3d * transitDuration_s, cartesianState2.velocity - vector3d);
			if (!Mathd.Approximately(cartesianState3.velocity.x, -cartesianState4.velocity.x) || !Mathd.Approximately(cartesianState3.velocity.y, -cartesianState4.velocity.y) || !Mathd.Approximately(cartesianState3.velocity.z, -cartesianState4.velocity.z))
			{
				Debug.LogError("TorchTransfer.Solve(): the initial and final velocities must be equal and opposite in the moving reference frame.  Initial = " + cartesianState3.velocity.ToString() + ", Final = " + cartesianState4.velocity.ToString());
			}
			Vector3d vector3d2 = cartesianState4.position - cartesianState3.position;
			Vector3d normalized = vector3d2.normalized;
			float num = (float)vector3d2.magnitude;
			float num2 = (float)Vector3d.Dot(in cartesianState3.velocity, in normalized);
			Vector3d vector3d3 = normalized * (double)num2;
			Vector3d vector3d4 = cartesianState3.velocity - vector3d3;
			float num3 = (float)vector3d4.magnitude;
			Vector3d normalized2 = vector3d4.normalized;
			float num4 = (float)fleetInitialAcceleration_mps2;
			float num5 = 1f / num4;
			float num6 = (float)transitDuration_s;
			float num7 = -num3;
			float num8 = -num3;
			float num9 = num2 - 0.5f * num6 * num4;
			float num10;
			bool flag;
			if (num9 * num9 + num2 * num6 * num4 - 2f * num2 * num2 - num * num4 >= 0f)
			{
				num10 = 0.5f * num6 * num4 - Mathf.Sqrt(num9 * num9 + num2 * num6 * num4 - 2f * num2 * num2 - num * num4);
				flag = true;
			}
			else
			{
				float num11 = num4 * num6 + 2f * num2;
				float num12 = num11 * num11 - 4f * num4 * num2 * num6 - 8f * num2 * num2 + 4f * num4 * num;
				if (num12 >= 0f)
				{
					num10 = -0.5f * (num11 + Mathf.Sqrt(num12));
					flag = true;
				}
				else
				{
					num10 = -num2;
					flag = false;
				}
			}
			float num13 = -2f * num2 - num10;
			float num14 = num10;
			float num15 = num7;
			float num16 = num13;
			float num17 = num8;
			int num18 = 20;
			float num19 = 1E-11f;
			float num20 = float.PositiveInfinity;
			float num21 = float.PositiveInfinity;
			for (int i = 0; i < num18; i++)
			{
				float num22 = num10 * num10 + num7 * num7;
				float num23 = Mathf.Sqrt(num22);
				float num24 = Mathf.Sqrt(num13 * num13 + num8 * num8);
				float num25 = num7 + num8 + 2f * num3;
				float num26 = num10 + num13 + 2f * num2;
				float num27 = num3 * num6 + num7 * (num6 - 0.5f * num23 * num5) + 0.5f * num8 * num24 * num5;
				float num28 = num2 * num6 + num10 * (num6 - 0.5f * num23 * num5) + 0.5f * num13 * num24 * num5 - num;
				float num29 = num21;
				num21 = num25 * num25 + num26 * num26 + num27 * num27 + num28 * num28;
				if (i == 0)
				{
					num20 = num21;
				}
				if (num21 < num19 || num29 == num21 || float.IsNaN(num21))
				{
					break;
				}
				Matrix4x4 matrix4x = new Matrix4x4(new Vector4(0f, 1f, -num10 * num7 / (2f * num4 * num23), num6 - (2f * num10 * num10 + num7 * num7) / (2f * num4 * num23)), new Vector4(1f, 0f, num6 - num22 / (2f * num4 * num23), -num10 * num7 / (2f * num4 * num23)), new Vector4(0f, 1f, num13 * num8 / (2f * num4 * num24), (2f * num13 * num13 + num8 * num8) / (2f * num4 * num24)), new Vector4(1f, 0f, (num13 * num13 + 2f * num8 * num8) / (2f * num4 * num24), num13 * num8 / (2f * num4 * num24)));
				Vector4 vector = new Vector4(num10, num7, num13, num8) - matrix4x.inverse * new Vector4(num25, num26, num27, num28);
				num10 = vector.x;
				num7 = vector.y;
				num13 = vector.z;
				num8 = vector.w;
			}
			if (num21 > num20 || float.IsInfinity(num21) || float.IsNaN(num21))
			{
				if (!flag)
				{
					possible = false;
					return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				}
				num10 = num14;
				num7 = num15;
				num13 = num16;
				num8 = num17;
			}
			Vector3d vector3d5 = normalized * (double)num10 + normalized2 * (double)num7;
			Vector3d vector3d6 = normalized * (double)num13 + normalized2 * (double)num8;
			base.boost_DV_mps = vector3d5.magnitude;
			base.decel_DV_mps = vector3d6.magnitude;
			this.accelDuration_s = base.boost_DV_mps * (double)num5;
			this.decelDuration_s = base.decel_DV_mps * (double)num5;
			base.DV_mps = base.boost_DV_mps + base.decel_DV_mps;
			this.accelerationVector_mps2 = fleetInitialAcceleration_mps2 * vector3d5.normalized;
			this.decelerationVector_mps2 = fleetInitialAcceleration_mps2 * vector3d6.normalized;
			if (double.IsNaN(this.accelDuration_s) || double.IsNaN(this.decelDuration_s))
			{
				possible = false;
				Debug.LogError("TorchTransfer failed -- produced NaN burn durations.");
				return new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
			}
			if (this.accelDuration_s + this.decelDuration_s > (double)num6)
			{
				possible = false;
				return new TransferResult(TransferResult.Outcome.Fail_BurnLongerThanTransfer, this.accelDuration_s + this.decelDuration_s, (double)num6);
			}
			if (base.DV_mps > fleetDV_mps)
			{
				possible = false;
				return new TransferResult(TransferResult.Outcome.Fail_InsufficientDV, base.DV_mps, 0.0);
			}
			possible = true;
			return new TransferResult(TransferResult.Outcome.Success, 0.0, 0.0);
		}
	}
}
