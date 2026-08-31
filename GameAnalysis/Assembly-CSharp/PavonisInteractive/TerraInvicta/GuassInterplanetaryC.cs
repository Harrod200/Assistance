using System;
using System.Collections.Generic;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200078D RID: 1933
	internal class GuassInterplanetaryC
	{
		// Token: 0x06003D8D RID: 15757 RVA: 0x00183358 File Offset: 0x00181558
		private GuassInterplanetaryC.FlightPlanC MakeFlightPlan(double a_m, double e, double i_rad, double o_rad, double l_rad, double w_rad, double launchDate_js, double duration_s, double departure_deltaV_mps, double arrival_deltaV_mps)
		{
			GuassInterplanetaryC.FlightPlanC flightPlanC;
			flightPlanC.orbit = new GuassInterplanetaryC.OrbitalBody
			{
				a_m = a_m,
				e = e,
				i_rad = i_rad,
				o_rad = o_rad,
				l_rad = l_rad,
				w_rad = w_rad,
				epoch_seconds = launchDate_js,
				radius_m = 0.0,
				mu = 0.0
			};
			flightPlanC.launchDate_js = launchDate_js;
			flightPlanC.duration_s = duration_s;
			flightPlanC.departure_deltaV_mps = departure_deltaV_mps;
			flightPlanC.arrival_deltaV_mps = arrival_deltaV_mps;
			return flightPlanC;
		}

		// Token: 0x06003D8E RID: 15758 RVA: 0x001833F8 File Offset: 0x001815F8
		private GuassInterplanetaryC.PosVel makePosVel(double px, double py, double pz, double vx, double vy, double vz)
		{
			GuassInterplanetaryC.PosVel posVel = default(GuassInterplanetaryC.PosVel);
			posVel.pos.x = px;
			posVel.pos.y = py;
			posVel.pos.z = pz;
			posVel.vel.x = vx;
			posVel.vel.y = vy;
			posVel.vel.z = vz;
			return posVel;
		}

		// Token: 0x06003D8F RID: 15759 RVA: 0x0018345F File Offset: 0x0018165F
		private double OrbitalPeriod(double semiMajorAxis_m, double barycenterMass_kg)
		{
			return 6.2831854820251465 * Mathd.Sqrt(semiMajorAxis_m * semiMajorAxis_m * semiMajorAxis_m / (6.674200081491222E-11 * barycenterMass_kg));
		}

		// Token: 0x06003D90 RID: 15760 RVA: 0x00183481 File Offset: 0x00181681
		private double Normalize_Rad(double angle)
		{
			return angle - 6.2831854820251465 * Math.Floor((angle + 3.1415927410125732) / 6.2831854820251465);
		}

		// Token: 0x06003D91 RID: 15761 RVA: 0x001834AC File Offset: 0x001816AC
		private GuassInterplanetaryC.PosVel AnglesToCartesianState(double omega, double u, double i)
		{
			double num = Mathd.Sin(u);
			double num2 = Mathd.Cos(u);
			double num3 = Mathd.Sin(i);
			double num4 = Mathd.Cos(i);
			double num5 = Mathd.Sin(omega);
			double num6 = Mathd.Cos(omega);
			double num7 = num * num4;
			double num8 = num6 * num2 - num5 * num7;
			double num9 = num5 * num2 + num6 * num7;
			double num10 = num * num3;
			double num11 = num2 * num4;
			double num12 = num6 * num + num5 * num11;
			double num13 = num5 * num - num6 * num11;
			double num14 = num2 * num3;
			return this.makePosVel(num8, num9, num10, num12, num13, num14);
		}

		// Token: 0x06003D92 RID: 15762 RVA: 0x0018353C File Offset: 0x0018173C
		private GuassInterplanetaryC.PosVel getOrbitPointAtJD(double jd, double a_m, double e, double inc_rad, double o_rad, double l_rad, double w_rad, double j2000Epoch, double barycenterMass_kg)
		{
			double num = l_rad - w_rad - o_rad;
			double num2 = jd - j2000Epoch;
			Vector3d vector3d;
			Vector3d vector3d2;
			if (e < 1.0)
			{
				num += 6.2831854820251465 * (num2 / this.OrbitalPeriod(a_m, barycenterMass_kg));
				num = this.Normalize_Rad(num);
				double num3 = num;
				for (double num4 = 0.0; num4 < 1000.0; num4 += 1.0)
				{
					double num5 = num3;
					num3 = num5 - (num5 - e * Mathd.Sin(num5) - num) / (1.0 - e * Mathd.Cos(num5));
					if (Mathd.Abs(num3 - num5) < 9.999999974752427E-07)
					{
						break;
					}
				}
				double num6 = 2.0 * Mathd.Atan2(Mathd.Sqrt(1.0 + e) * Mathd.Sin(num3 / 2.0), Mathd.Sqrt(1.0 - e) * Mathd.Cos(num3 / 2.0));
				num6 = this.Normalize_Rad(num6);
				double num7 = a_m * (1.0 - Mathd.Pow(e, 2.0)) / (1.0 + e * Mathd.Cos(num6));
				double num8 = num6 + w_rad;
				num8 = this.Normalize_Rad(num8);
				GuassInterplanetaryC.PosVel posVel = this.AnglesToCartesianState(o_rad, num8, inc_rad);
				vector3d = posVel.pos;
				vector3d2 = posVel.vel;
				vector3d *= num7;
				if (barycenterMass_kg != 0.0)
				{
					double num9 = a_m * (1.0 - e * e);
					double num10 = Mathd.Sqrt(6.674200081491222E-11 * barycenterMass_kg * num9);
					Vector3d vector3d3 = new Vector3d(-1f, -1f, 1f);
					Vector3d vector3d4 = num10 / num7 * vector3d3;
					vector3d2 = new Vector3d(vector3d2.x * vector3d4.x, vector3d2.y * vector3d4.y, vector3d2.z * vector3d4.z);
				}
			}
			else
			{
				double num11 = Mathd.Sqrt(6.674200081491222E-11 * barycenterMass_kg / Mathd.Pow(-a_m, 3.0));
				num += num11 * num2;
				double num3 = ((num > 10.0) ? Mathd.Log(num / e) : num);
				for (double num12 = 0.0; num12 < 1000.0; num12 += 1.0)
				{
					double num13 = num3;
					num3 = num13 - (e * Math.Sinh(num13) - num13 - num) / (e * Math.Cosh(num13) - 1.0);
					if (Mathd.Abs(num3 - num13) >= 9.999999974752427E-07)
					{
						break;
					}
				}
				double num6 = Mathd.Acos((Math.Cosh(num3) - e) / (1.0 - e * Math.Cosh(num3)));
				if (num3 < 0.0)
				{
					num6 = 6.2831854820251465 - num6;
				}
				else if (num3 == 0.0)
				{
					num6 = 0.0;
				}
				double num7 = a_m * (1.0 - e * Math.Cosh(num3));
				double num8 = num6 + w_rad;
				num8 = this.Normalize_Rad(num8);
				GuassInterplanetaryC.PosVel posVel2 = this.AnglesToCartesianState(o_rad, num8, inc_rad);
				vector3d = posVel2.pos;
				vector3d2 = posVel2.vel;
				vector3d *= num7;
				if (barycenterMass_kg != 0.0)
				{
					double num14 = a_m * (1.0 - e * e);
					double num15 = Mathd.Sqrt(6.674200081491222E-11 * barycenterMass_kg * num14);
					Vector3d vector3d5 = new Vector3d(-1f, 1f, 1f);
					Vector3d vector3d6 = num15 / num7 * vector3d5;
					vector3d2 = new Vector3d(vector3d2.x * vector3d6.x, vector3d2.y * vector3d6.y, vector3d2.z * vector3d6.z);
				}
			}
			GuassInterplanetaryC.PosVel posVel3;
			posVel3.pos = vector3d;
			posVel3.vel = vector3d2;
			return posVel3;
		}

		// Token: 0x06003D93 RID: 15763 RVA: 0x00183950 File Offset: 0x00181B50
		private GuassInterplanetaryC.PosVel getOrbitalBodyAtTime(GuassInterplanetaryC.OrbitalBody ob, double t)
		{
			return this.getOrbitPointAtJD(t, ob.a_m, ob.e, ob.i_rad, ob.o_rad, ob.l_rad, ob.w_rad, ob.epoch_seconds, this.commonBarycenterMu / 6.674200081491222E-11);
		}

		// Token: 0x06003D94 RID: 15764 RVA: 0x001839A0 File Offset: 0x00181BA0
		private double gaussCalcT(double r1d, double r2d, double angleTo, double k, double l, double m, double trialP, out double a, out double f, out double g)
		{
			a = m * k * trialP / ((2.0 * m - l * l) * trialP * trialP + 2.0 * k * l * trialP - k * k);
			f = 1.0 - r2d / trialP * (1.0 - Mathd.Cos(angleTo));
			g = r1d * r2d * Mathd.Sin(angleTo) / Mathd.Sqrt(this.commonBarycenterMu_AU * trialP);
			if (a > 0.0)
			{
				double num = Mathd.Acos(Mathd.Clamp(1.0 - r1d / a * (1.0 - f), -1.0, 1.0));
				return (g + Mathd.Sqrt(a * a * a / this.commonBarycenterMu_AU) * (num - Mathd.Sin(num))) / 86400.0;
			}
			double num2 = Mathd.ACosh(Mathd.Max(1.0 - r1d / a * (1.0 - f), 1.0));
			return (g + Mathd.Sqrt(-a * -a * -a / this.commonBarycenterMu_AU) * (Mathd.Sinh(num2) - num2)) / 86400.0;
		}

		// Token: 0x06003D95 RID: 15765 RVA: 0x00183B04 File Offset: 0x00181D04
		private double gaussGuessP(ref double t, double r1d, double r2d, double angleTo, out double a, out double f, out double g)
		{
			r1d /= 149597870700.0;
			r2d /= 149597870700.0;
			t /= 86400.0;
			double num = r1d * r2d * (1.0 - Mathd.Cos(angleTo));
			double num2 = r1d + r2d;
			double num3 = r1d * r2d * (1.0 + Mathd.Cos(angleTo));
			double num4 = num / (num2 + Mathd.Sqrt(2.0 * num3));
			double num5 = num / (num2 - Mathd.Sqrt(2.0 * num3));
			double num6;
			if (angleTo < 3.1415927410125732)
			{
				num6 = num4;
			}
			else
			{
				num6 = 0.0;
				Log.Info("Long way round: " + t.ToString(), Array.Empty<object>());
			}
			double num7 = 0.3333333333333333 * (num4 + num4 + num5);
			double num8 = 0.3333333333333333 * (num4 + num5 + num5);
			double num9 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num7, out a, out f, out g);
			double num10 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num8, out a, out f, out g);
			double num11 = num9 - t;
			double num12 = num10 - t;
			int num13 = 0;
			double num15;
			double num18;
			for (;;)
			{
				for (int i = 0; i < 60; i++)
				{
					double num14 = -((num7 - num8) * num12 / (num11 - num12));
					num15 = num8 + num14 / (1.0 + num14 * num14);
					double num16 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num15, out a, out f, out g) - t;
					if (Mathd.Abs(num16) <= 9.999999960041972E-12 || (i == 59 && Mathd.Abs(num16) <= 9.999999747378752E-05))
					{
						goto IL_019D;
					}
					if (double.IsNaN(num16))
					{
						break;
					}
					num7 = num8;
					num8 = num15;
					num11 = num12;
					num12 = num16;
				}
				if (num13 == 0)
				{
					num7 = num6;
					num8 = num6 * 1.0499999523162842;
					num9 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num7, out a, out f, out g);
					num10 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num8, out a, out f, out g);
					int num17 = 0;
					if (angleTo < 3.1415927410125732)
					{
						while ((num9 - t) * (num10 - t) >= 0.0 && num17 < 14)
						{
							num8 += num5;
							num10 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num8, out a, out f, out g);
							num17++;
						}
					}
					if (num17 < 14)
					{
						for (int j = 0; j < 50; j++)
						{
							num18 = (num7 + num8) / 2.0;
							double num16 = this.gaussCalcT(r1d, r2d, angleTo, num, num2, num3, num18, out a, out f, out g) - t;
							if (Mathd.Abs(num16) < 9.999999960041972E-12 || (j >= 49 && Mathd.Abs(num16) < 9.999999747378752E-05))
							{
								goto IL_02F3;
							}
							if (num16 < 0.0)
							{
								num8 = num18;
							}
							else
							{
								num7 = num18;
							}
						}
					}
				}
				num13++;
				if (num13 == 1)
				{
					goto Block_12;
				}
				if (num13 >= 2)
				{
					goto IL_0372;
				}
			}
			IL_019D:
			a *= 149597870700.0;
			num15 *= 149597870700.0;
			t *= 86400.0;
			return num15;
			IL_02F3:
			a *= 149597870700.0;
			num18 *= 149597870700.0;
			t *= 86400.0;
			return num18;
			Block_12:
			t -= this.durationStep_s / 172800.0;
			IL_0372:
			t *= 86400.0;
			return double.PositiveInfinity;
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x00183E9C File Offset: 0x0018209C
		public void ComputeFlightPlans()
		{
			this.commonBarycenterMu_AU = this.commonBarycenterMu / 3.3479289758107494E+33;
			this.Results = new List<GuassInterplanetaryC.FlightPlanC>();
			for (int i = 0; i < 1; i++)
			{
				double num;
				if (i == -1)
				{
					num = this.nextHohmannLaunchDate;
				}
				else
				{
					num = this.baseLaunch_js + (double)i * this.launchDateStep_s;
				}
				GuassInterplanetaryC.PosVel orbitalBodyAtTime = this.getOrbitalBodyAtTime(this.Source, num);
				Vector3d pos = orbitalBodyAtTime.pos;
				double num2 = Vector3d.Magnitude(in pos);
				for (int j = 0; j < 64; j++)
				{
					double num3 = this.baseDuration_s + (double)j * this.durationStep_s;
					double num4 = num + num3;
					GuassInterplanetaryC.PosVel orbitalBodyAtTime2 = this.getOrbitalBodyAtTime(this.Destination, num4);
					Vector3d pos2 = orbitalBodyAtTime2.pos;
					double num5 = Vector3d.Magnitude(in pos2);
					double num6 = Mathd.Acos((pos.x * pos2.x + pos.y * pos2.y + pos.z * pos2.z) / (num2 * num5));
					double num8;
					double num9;
					double num10;
					double num7 = this.gaussGuessP(ref num3, num2, num5, num6, out num8, out num9, out num10);
					if (num7 != double.PositiveInfinity && !double.IsNaN(num7))
					{
						double num11 = Mathd.Sqrt(this.commonBarycenterMu / num7) * Mathd.Tan(num6 / 2.0) * ((1.0 - Mathd.Cos(num6)) / num7 - 1.0 / num2 - 1.0 / num5);
						double num12 = 1.0 - num2 / num7 * (1.0 - Mathd.Cos(num6));
						Vector3d vector3d = (pos2 - pos * num9) / num10;
						Vector3d vector3d2 = pos * num11 + vector3d * num12;
						double num13 = Vector3d.Magnitude(in vector3d);
						Vector3d vector3d3 = Vector3d.Cross(pos, vector3d);
						Vector3d vector3d4 = new Vector3d(-vector3d3.y, vector3d3.x, 0.0);
						double num14 = Vector3d.Magnitude(in vector3d3);
						double num15 = Vector3d.Magnitude(in vector3d4);
						Vector3d vector3d5 = ((num13 * num13 - this.commonBarycenterMu / num2) * pos - Vector3d.Dot(in pos, in vector3d) * vector3d) / this.commonBarycenterMu;
						double num16 = Vector3d.Magnitude(in vector3d5);
						double num17 = Mathd.Acos(vector3d3.z / num14);
						double num18 = Mathd.Acos(vector3d4.x / num15);
						double num19 = Mathd.Acos(Vector3d.Dot(in vector3d5, in vector3d4) / (num15 * num16));
						double num20 = Mathd.Acos(Vector3d.Dot(in pos, in vector3d5) / (num16 * num2));
						double num21 = num18 + num19 + num20;
						Vector3d vector3d6 = vector3d - orbitalBodyAtTime.vel;
						double num22 = Vector3d.Magnitude(in vector3d6);
						double num23 = Mathd.Sqrt(num22 * num22 + 2.0 * this.Source.mu / this.Source.radius_m) - Mathd.Sqrt(this.Source.mu / this.Source.radius_m);
						vector3d6 = vector3d2 - orbitalBodyAtTime2.vel;
						double num24 = Vector3d.Magnitude(in vector3d6);
						double num25 = Mathd.Sqrt(num24 * num24 + 2.0 * this.Destination.mu / this.Destination.radius_m) - Mathd.Sqrt(this.Destination.mu / this.Destination.radius_m);
						this.Results.Add(this.MakeFlightPlan(num8, num16, num17, num18, num21, num19, num, num3, num23, num25));
					}
				}
			}
		}

		// Token: 0x04002695 RID: 9877
		private const double DAY_s = 86400.0;

		// Token: 0x04002696 RID: 9878
		private const double G = 6.674200081491222E-11;

		// Token: 0x04002697 RID: 9879
		private const double PI = 3.1415927410125732;

		// Token: 0x04002698 RID: 9880
		private const double TWOPI = 6.2831854820251465;

		// Token: 0x04002699 RID: 9881
		public double commonBarycenterMu;

		// Token: 0x0400269A RID: 9882
		public double baseLaunch_js;

		// Token: 0x0400269B RID: 9883
		public double baseDuration_s;

		// Token: 0x0400269C RID: 9884
		public double durationStep_s;

		// Token: 0x0400269D RID: 9885
		public double launchDateStep_s;

		// Token: 0x0400269E RID: 9886
		public GuassInterplanetaryC.OrbitalBody Source;

		// Token: 0x0400269F RID: 9887
		public GuassInterplanetaryC.OrbitalBody Destination;

		// Token: 0x040026A0 RID: 9888
		public double nextHohmannLaunchDate;

		// Token: 0x040026A1 RID: 9889
		public double commonBarycenterMu_AU;

		// Token: 0x040026A2 RID: 9890
		public double synodicPeriod_days;

		// Token: 0x040026A3 RID: 9891
		public List<GuassInterplanetaryC.FlightPlanC> Results;

		// Token: 0x02000EC2 RID: 3778
		public struct OrbitalBody
		{
			// Token: 0x060079CB RID: 31179 RVA: 0x0031A910 File Offset: 0x00318B10
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					"a: ",
					this.a_m.ToString("N0"),
					" m e: ",
					this.e.ToString("N3"),
					" i: ",
					this.i_rad.ToString("N2"),
					" rad o: ",
					this.o_rad.ToString("N2"),
					" rad l: ",
					this.l_rad.ToString("N2"),
					" rad w: ",
					this.w_rad.ToString("N2"),
					" rad epoch_seconds: ",
					this.epoch_seconds.ToString("N0"),
					" launch radius: ",
					this.radius_m.ToString("N0"),
					" m mu: ",
					this.mu.ToString("N2")
				});
			}

			// Token: 0x04005A30 RID: 23088
			public double a_m;

			// Token: 0x04005A31 RID: 23089
			public double e;

			// Token: 0x04005A32 RID: 23090
			public double i_rad;

			// Token: 0x04005A33 RID: 23091
			public double o_rad;

			// Token: 0x04005A34 RID: 23092
			public double l_rad;

			// Token: 0x04005A35 RID: 23093
			public double w_rad;

			// Token: 0x04005A36 RID: 23094
			public double epoch_seconds;

			// Token: 0x04005A37 RID: 23095
			public double radius_m;

			// Token: 0x04005A38 RID: 23096
			public double mu;

			// Token: 0x04005A39 RID: 23097
			public double period_d;
		}

		// Token: 0x02000EC3 RID: 3779
		public struct FlightPlanC
		{
			// Token: 0x04005A3A RID: 23098
			public GuassInterplanetaryC.OrbitalBody orbit;

			// Token: 0x04005A3B RID: 23099
			public double launchDate_js;

			// Token: 0x04005A3C RID: 23100
			public double duration_s;

			// Token: 0x04005A3D RID: 23101
			public double departure_deltaV_mps;

			// Token: 0x04005A3E RID: 23102
			public double arrival_deltaV_mps;
		}

		// Token: 0x02000EC4 RID: 3780
		public struct PosVel
		{
			// Token: 0x04005A3F RID: 23103
			public Vector3d pos;

			// Token: 0x04005A40 RID: 23104
			public Vector3d vel;
		}
	}
}
