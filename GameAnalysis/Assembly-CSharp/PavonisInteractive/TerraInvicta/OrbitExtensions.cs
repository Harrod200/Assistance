using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Components;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;
using UnityEngine.Rendering;
using Vectrosity;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F9 RID: 2041
	public static class OrbitExtensions
	{
		// Token: 0x06004A10 RID: 18960 RVA: 0x001F14A0 File Offset: 0x001EF6A0
		public static Orbit Fill(this Orbit orbit, bool noElements = false)
		{
			if (noElements)
			{
				return orbit;
			}
			SpaceObject value = orbit.Barycenter.GetComponent<SpaceObjectComponent>().Value;
			double num = Mathd.Sin(orbit.Inclination_Rad);
			orbit.Normal = Vector3d.Normalize(new Vector3d(num * Mathd.Sin(orbit.LongitudeAscendingNode_Rad), -num * Mathd.Cos(orbit.LongitudeAscendingNode_Rad), Mathd.Cos(orbit.Inclination_Rad)));
			if (orbit.IsElliptical)
			{
				orbit.Period = 6.283185307179586 * Mathd.Sqrt(Mathd.Pow(orbit.SemimajorAxis_m, 3.0) / (value.Mass * 6.67384E-11));
				orbit.MeanMotion = 6.283185307179586 / orbit.Period;
				orbit.Apoapsis = orbit.ApoapsisPosition();
			}
			else
			{
				orbit.MeanMotion = Mathd.Sqrt(value.Mass * 6.67384E-11 / Mathd.Abs(orbit.SemimajorAxis_m * orbit.SemimajorAxis_m * orbit.SemimajorAxis_m));
			}
			orbit.Periapsis = orbit.PeriapsisPosition();
			return orbit;
		}

		// Token: 0x06004A11 RID: 18961 RVA: 0x001F15B4 File Offset: 0x001EF7B4
		public static Orbit FillOrbitTrail(this Orbit orbit, TISpaceObjectState spaceObject, out GameObject orbitTrailObject)
		{
			if (OrbitExtensions.s_defaultOrbitMaterial == null)
			{
				OrbitExtensions.s_defaultOrbitMaterial = Resources.Load("OrbitLine") as Material;
				if (OrbitExtensions.s_defaultOrbitMaterial == null)
				{
					Debug.LogError("No OrbitLine material found in Resources.");
				}
			}
			int num = (spaceObject.barycenter.isSun ? 61 : 31);
			orbit.WorldPoints = new Vector3d[num];
			orbit.TimeAtPoint_s = new double[num];
			OrbitalElementsState orbitalElementsState = new OrbitalElementsState(orbit);
			double num2 = 6.283185307179586 / (double)(num - 1);
			double num3 = Mathd.Sqrt(1.0 - orbit.Eccentricity * orbit.Eccentricity);
			double num4 = orbit.Period / 6.283185307179586;
			double num5 = orbit.SemimajorAxis_m * orbit.SemimajorAxis_m * orbit.SemimajorAxis_m / (6.67384E-11 * num4 * num4);
			orbit.TimeAtPoint_s[0] = 0.0;
			orbit.WorldPoints[0] = orbitalElementsState.ToCartesianStateAtMeanAnomaly(0.0, num5).position;
			for (int i = 1; i < num - 1; i++)
			{
				double num6 = (double)i * num2 % 6.283185307179586;
				double num7 = Mathd.Atan(Mathd.Tan(num6) * num3);
				if (num6 > 1.5707963267948966 && num6 <= 4.71238898038469)
				{
					num7 += 3.141592653589793;
				}
				num7 = Mathd.ClampRadiansTwoPI(num7);
				double meanAnomalyFromEccentricAnomaly = orbitalElementsState.GetMeanAnomalyFromEccentricAnomaly(num7);
				double num8 = meanAnomalyFromEccentricAnomaly * orbit.Period / 6.283185307179586;
				orbit.TimeAtPoint_s[i] = num8;
				orbit.WorldPoints[i] = orbitalElementsState.ToCartesianStateAtMeanAnomaly(meanAnomalyFromEccentricAnomaly, num5).position;
			}
			orbit.TimeAtPoint_s[orbit.TimeAtPoint_s.Length - 1] = orbit.Period;
			orbit.WorldPoints[orbit.WorldPoints.Length - 1] = orbit.WorldPoints[0];
			orbit.ScaledPoints = new Vector3[orbit.WorldPoints.Length + 1];
			string text = string.Format("FixedOrbitTrail_{0}", spaceObject.ID);
			orbit.OrbitTrail = new VectorLine(text, new List<Vector3>(orbit.WorldPoints.Length * 3 + 1), 3f, LineType.Continuous)
			{
				layer = LayerMask.NameToLayer("Default"),
				color = (spaceObject.isSpaceAssetState ? (spaceObject.ref_faction.template.color * spaceObject.ref_faction.template.colorIntensity) : OrbitExtensions.s_orbitColor[spaceObject.objectType])
			};
			orbit.OrbitTrail.SetMaterial(new Material(OrbitExtensions.s_defaultOrbitMaterial), true);
			orbitTrailObject = orbit.OrbitTrail.rectTransform.gameObject;
			MeshRenderer component = orbitTrailObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.shadowCastingMode = ShadowCastingMode.Off;
				component.receiveShadows = false;
				component.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
				component.lightProbeUsage = LightProbeUsage.Off;
				component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			}
			GameControl.solarSystem.AddOrbitTrailToContainer(orbitTrailObject);
			return orbit;
		}

		// Token: 0x06004A12 RID: 18962 RVA: 0x001F18C8 File Offset: 0x001EFAC8
		public static Orbit FillTransferOrbit(this Orbit orbit, IMobileAsset fleet, Trajectory trajectory, out GameObject orbitTrailObject)
		{
			if (OrbitExtensions.s_defaultOrbitMaterial == null)
			{
				OrbitExtensions.s_defaultOrbitMaterial = Resources.Load("OrbitLine") as Material;
				if (OrbitExtensions.s_defaultOrbitMaterial == null)
				{
					Debug.LogError("No OrbitLine material found in Resources.");
				}
			}
			int num = (int)Mathd.Clamp(trajectory.duration_s / 1036800.0, 30.0, 100.0);
			double num2 = trajectory.duration_s / (double)num;
			TIDateTime tidateTime;
			if (fleet.transferAssigned)
			{
				tidateTime = trajectory.launchTime;
				num = Mathd.Clamp(Mathd.FloorToInt(trajectory.flightDuration_s / num2), 30, 100) + 1;
				num2 = Mathd.Max(trajectory.flightDuration_s / (double)num, 0.0);
			}
			else
			{
				tidateTime = TITimeState.Now();
			}
			orbit.TimeAtPoint_s = new double[num];
			TIDateTime tidateTime2 = new TIDateTime(tidateTime);
			orbit.WorldPoints = new Vector3d[num];
			bool flag;
			orbit.WorldPoints[0] = trajectory.PositionAtTime(tidateTime, false, out flag);
			for (int i = 1; i < orbit.WorldPoints.Length - 1; i++)
			{
				tidateTime2.AddSeconds(num2);
				orbit.TimeAtPoint_s[i] = tidateTime2.DifferenceInSeconds(tidateTime);
				orbit.WorldPoints[i] = trajectory.PositionAtTime(tidateTime2, false, out flag);
			}
			orbit.TimeAtPoint_s[orbit.WorldPoints.Length - 1] = trajectory.arrivalTime.DifferenceInSeconds(tidateTime);
			orbit.WorldPoints[orbit.WorldPoints.Length - 1] = trajectory.PositionAtTime(trajectory.arrivalTime, false, out flag);
			orbit.ScaledPoints = new Vector3[orbit.WorldPoints.Length];
			TISpaceFleetState tispaceFleetState = fleet as TISpaceFleetState;
			string text = ((tispaceFleetState != null) ? string.Format("TransferOrbitTrail_{0}", tispaceFleetState.ID) : "TransferOrbitTrail_virtual");
			orbit.OrbitTrail = new VectorLine(text, new List<Vector3>(orbit.WorldPoints.Length * 3), 4f, LineType.Continuous)
			{
				layer = LayerMask.NameToLayer("Default"),
				color = fleet.faction.template.color * fleet.faction.template.colorIntensity
			};
			orbit.OrbitTrail.SetMaterial(new Material(OrbitExtensions.s_defaultOrbitMaterial), true);
			orbitTrailObject = orbit.OrbitTrail.rectTransform.gameObject;
			MeshRenderer component = orbitTrailObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.shadowCastingMode = ShadowCastingMode.Off;
				component.receiveShadows = false;
				component.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
				component.lightProbeUsage = LightProbeUsage.Off;
				component.reflectionProbeUsage = ReflectionProbeUsage.Off;
			}
			GameControl.solarSystem.AddOrbitTrailToContainer(orbitTrailObject);
			return orbit;
		}

		// Token: 0x06004A13 RID: 18963 RVA: 0x001F1B54 File Offset: 0x001EFD54
		public static double SynodicPeriod(this Orbit a, Orbit b)
		{
			int num = ((Vector3d.Dot(in a.Normal, in b.Normal) > 0.0) ? 1 : (-1));
			return Mathd.Abs(1.0 / (1.0 / a.Period - (double)num / b.Period));
		}

		// Token: 0x06004A14 RID: 18964 RVA: 0x001F1BB0 File Offset: 0x001EFDB0
		public static Orbit PerturbedOrbit(this Orbit orbit, DateTime time, Vector3d deltaV, out OrbitalElementsState newOrbit)
		{
			CartesianState cartesianState = orbit.LocalCartesianState(time);
			Vector3d vector3d = cartesianState.velocity + deltaV;
			double mass = orbit.Barycenter.GetComponent<SpaceObjectComponent>().Value.Mass;
			newOrbit = new CartesianState(cartesianState.position, vector3d).ToOrbitalElementsState(mass * 6.67384E-11, null);
			return new Orbit
			{
				Eccentricity = newOrbit.eccentricity,
				SemimajorAxis_m = newOrbit.semiMajorAxis_m,
				Inclination_Rad = newOrbit.inclination_Rad,
				LongitudeAscendingNode_Rad = newOrbit.longAscendingNode_Rad,
				ArgumentPeriapsis_Rad = newOrbit.argPeriapsis_Rad,
				MeanAnomalyAtEpoch_Rad = newOrbit.meanAnomalyAtEpoch_Rad,
				Epoch = time,
				Barycenter = orbit.Barycenter
			}.Fill(false);
		}

		// Token: 0x06004A15 RID: 18965 RVA: 0x001F1C8A File Offset: 0x001EFE8A
		public static Vector3d Position(this Orbit orbit, DateTime time)
		{
			return orbit.CartesianState(time).position;
		}

		// Token: 0x06004A16 RID: 18966 RVA: 0x001F1C98 File Offset: 0x001EFE98
		public static Vector3d LocalPosition(this Orbit orbit, DateTime time)
		{
			return orbit.LocalCartesianState(time).position;
		}

		// Token: 0x06004A17 RID: 18967 RVA: 0x001F1CA8 File Offset: 0x001EFEA8
		public static CartesianState LocalCartesianState(this Orbit orbit, DateTime time)
		{
			double mass = orbit.Barycenter.GetComponent<SpaceObjectComponent>().Value.Mass;
			return new OrbitalElementsState(orbit).ToCartesianStateAtTime(time, mass);
		}

		// Token: 0x06004A18 RID: 18968 RVA: 0x001F1CDC File Offset: 0x001EFEDC
		public static CartesianState CartesianState(this Orbit orbit, DateTime time)
		{
			CartesianState cartesianState = orbit.LocalCartesianState(time);
			if (orbit.Barycenter.HasComponent<OrbitComponent>())
			{
				SpaceObject value = orbit.Barycenter.GetComponent<SpaceObjectComponent>().Value;
				Vector3d vector3d = cartesianState.positionDisplay;
				Vector3d vector3d2 = cartesianState.velocityDisplay;
				vector3d = value.SpatialRotation * vector3d;
				vector3d2 = value.SpatialRotation * vector3d2;
				vector3d = vector3d.xzy;
				vector3d2 = vector3d2.xzy;
				cartesianState = orbit.Barycenter.GetComponent<OrbitComponent>().Value.CartesianState(time) + new CartesianState(vector3d, vector3d2);
			}
			return cartesianState;
		}

		// Token: 0x06004A19 RID: 18969 RVA: 0x001F1D6C File Offset: 0x001EFF6C
		public static DateTime TimeOfTrueAnomaly(this Orbit orbit, double trueAnomaly, DateTime time)
		{
			double num = orbit.TrueToEccentric(trueAnomaly);
			double num2 = orbit.EccentricToMean(num);
			return orbit.TimeAtMeanAnomaly(num2, time);
		}

		// Token: 0x06004A1A RID: 18970 RVA: 0x001F1D94 File Offset: 0x001EFF94
		public static double TrueAnomalyFromVector(this Orbit orbit, Vector3d v)
		{
			Vector3d vector3d = Vector3d.Exclude(orbit.Normal, v);
			double num = 0.017453292519943295 * Vector3d.Angle(in orbit.Periapsis, in vector3d);
			double num2 = 0.017453292519943295;
			Vector3d vector3d2 = Vector3d.Cross(orbit.Normal, orbit.Periapsis);
			if (Mathd.Abs(num2 * Vector3d.Angle(in vector3d, in vector3d2)) < 1.5707963267948966)
			{
				return num;
			}
			return 6.283185307179586 - num;
		}

		// Token: 0x06004A1B RID: 18971 RVA: 0x001F1E0C File Offset: 0x001F000C
		public static double RadiusAtTrueAnomaly(this Orbit orbit, double t)
		{
			double semimajorAxis_m = orbit.SemimajorAxis_m;
			double eccentricity = orbit.Eccentricity;
			return semimajorAxis_m * ((1.0 - eccentricity * eccentricity) / (1.0 + eccentricity * Mathd.Cos(t)));
		}

		// Token: 0x06004A1C RID: 18972 RVA: 0x001F1E47 File Offset: 0x001F0047
		public static DateTime NextPeriapsisTime(this Orbit orbit, DateTime time)
		{
			if (orbit.IsElliptical)
			{
				return orbit.TimeAtMeanAnomaly(0.0, time);
			}
			return time - TimeSpan.FromSeconds(orbit.MeanAnomalyAtTime(time) / orbit.MeanMotion);
		}

		// Token: 0x06004A1D RID: 18973 RVA: 0x001F1E7C File Offset: 0x001F007C
		public static DateTime PrevPeriapsisTime(this Orbit orbit, DateTime time)
		{
			if (orbit.IsHyperbolic)
			{
				return orbit.NextPeriapsisTime(time);
			}
			return orbit.NextPeriapsisTime(time).AddSeconds(-orbit.Period);
		}

		// Token: 0x06004A1E RID: 18974 RVA: 0x001F1EB0 File Offset: 0x001F00B0
		public static DateTime NextApoapsisTime(this Orbit orbit, DateTime time)
		{
			if (orbit.IsElliptical)
			{
				return orbit.TimeAtMeanAnomaly(3.141592653589793, time);
			}
			throw new ArgumentException("OrbitExtensions.NextApoapsisTime cannot be called on hyperbolic orbits");
		}

		// Token: 0x06004A1F RID: 18975 RVA: 0x001F1ED8 File Offset: 0x001F00D8
		public static Vector3d ApoapsisPosition(this Orbit orbit)
		{
			if (orbit.IsElliptical)
			{
				OrbitalElementsState orbitalElementsState = new OrbitalElementsState(orbit);
				Vector3d periapsisVector = orbitalElementsState.periapsisVector;
				return periapsisVector - periapsisVector.normalized * 2.0 * orbit.SemimajorAxis_m;
			}
			throw new ArgumentException("OrbitExtensions.ApoapsisPosition cannot be called on hyperbolic orbits");
		}

		// Token: 0x06004A20 RID: 18976 RVA: 0x001F1F30 File Offset: 0x001F0130
		public static Vector3d PeriapsisPosition(this Orbit orbit)
		{
			return new OrbitalElementsState(orbit).periapsisVector;
		}

		// Token: 0x06004A21 RID: 18977 RVA: 0x001F1F4C File Offset: 0x001F014C
		private static DateTime TimeAtMeanAnomaly(this Orbit orbit, double meanAnomaly, DateTime time)
		{
			double num = orbit.MeanAnomalyAtTime(time);
			double num2 = meanAnomaly - num;
			if (orbit.IsElliptical)
			{
				num2 = Mathd.ClampRadiansTwoPI(num2);
			}
			double num3 = num2 / orbit.MeanMotion;
			return time.AddSeconds(num3);
		}

		// Token: 0x06004A22 RID: 18978 RVA: 0x001F1F88 File Offset: 0x001F0188
		private static double MeanAnomalyAtTime(this Orbit orbit, DateTime time)
		{
			double num = (time - orbit.Epoch).TotalSeconds * orbit.MeanMotion;
			double num2 = orbit.MeanAnomalyAtEpoch_Rad + num;
			if (orbit.IsElliptical)
			{
				num2 = Mathd.ClampRadiansTwoPI(num2);
			}
			return num2;
		}

		// Token: 0x06004A23 RID: 18979 RVA: 0x001F1FCC File Offset: 0x001F01CC
		private static double EccentricToMean(this Orbit orbit, double E)
		{
			double eccentricity = orbit.Eccentricity;
			if (orbit.IsElliptical)
			{
				return Mathd.ClampRadiansTwoPI(E - eccentricity * Mathd.Sin(E));
			}
			return eccentricity * Mathd.Sinh(E) - E;
		}

		// Token: 0x06004A24 RID: 18980 RVA: 0x001F2004 File Offset: 0x001F0204
		private static double TrueToEccentric(this Orbit orbit, double trueAnomaly)
		{
			double eccentricity = orbit.Eccentricity;
			double num = Mathd.Cos(trueAnomaly);
			if (orbit.IsElliptical)
			{
				double num2 = (eccentricity + num) / (1.0 + eccentricity * num);
				double num3 = Mathd.Sqrt(1.0 - num2 * num2);
				if (trueAnomaly > 3.141592653589793)
				{
					num3 *= -1.0;
				}
				return Mathd.ClampRadiansTwoPI(Mathd.Atan2(num3, num2));
			}
			double num4 = (eccentricity + num) / (1.0 + eccentricity * num);
			if (num4 < 1.0)
			{
				throw new ArgumentException("OrbitExtensions.GetEccentricAnomalyAtTrueAnomaly: True anomaly of " + trueAnomaly.ToString() + " radians is not attained by orbit with eccentricity " + orbit.Eccentricity.ToString());
			}
			double num5 = Mathd.ACosh(num4);
			if (trueAnomaly > 3.141592653589793)
			{
				num5 *= -1.0;
			}
			return num5;
		}

		// Token: 0x06004A25 RID: 18981 RVA: 0x001F20DA File Offset: 0x001F02DA
		private static double CircularOrbitSpeed(SpaceObject body, double radius)
		{
			return Mathd.Sqrt(body.Mass * 6.67384E-11 / radius);
		}

		// Token: 0x06004A26 RID: 18982 RVA: 0x001F20F4 File Offset: 0x001F02F4
		public static Vector3d DeltaVToCircularize(this Orbit orbit, DateTime time)
		{
			Vector3d vector3d = OrbitExtensions.CircularOrbitSpeed(orbit.Barycenter.GetComponent<SpaceObjectComponent>().Value, orbit.Radius(time)) * orbit.Horizontal(time);
			Vector3d velocity = orbit.LocalCartesianState(time).velocity;
			return vector3d - velocity;
		}

		// Token: 0x06004A27 RID: 18983 RVA: 0x001F213C File Offset: 0x001F033C
		public static Vector3d HeadingToAN(this Orbit orbit)
		{
			CartesianState cartesianState = orbit.LocalCartesianState(orbit.Epoch);
			Vector3d vector3d = Vector3d.Cross(cartesianState.position, cartesianState.velocity);
			Vector3d vector3d2 = new Vector3d(-vector3d.y, vector3d.x, 0.0);
			return vector3d2.normalized;
		}

		// Token: 0x06004A28 RID: 18984 RVA: 0x001F218C File Offset: 0x001F038C
		public static Vector3d Prograde(this Orbit orbit, DateTime time)
		{
			return orbit.LocalCartesianState(time).velocity.normalized;
		}

		// Token: 0x06004A29 RID: 18985 RVA: 0x001F21B0 File Offset: 0x001F03B0
		public static Vector3d Horizontal(this Orbit orbit, DateTime time)
		{
			return Vector3d.Exclude(orbit.Radial(time), orbit.Prograde(time)).normalized;
		}

		// Token: 0x06004A2A RID: 18986 RVA: 0x001F21D8 File Offset: 0x001F03D8
		public static Vector3d Radial(this Orbit orbit, DateTime time)
		{
			return orbit.LocalPosition(time).normalized;
		}

		// Token: 0x06004A2B RID: 18987 RVA: 0x001F21F4 File Offset: 0x001F03F4
		public static double Radius(this Orbit orbit, DateTime time)
		{
			return orbit.LocalPosition(time).magnitude;
		}

		// Token: 0x06004A2C RID: 18988 RVA: 0x001F2210 File Offset: 0x001F0410
		public static double Separation(this Orbit a, Orbit b, DateTime time)
		{
			Vector3d vector3d = a.Position(time) - b.Position(time);
			return Vector3d.Magnitude(in vector3d);
		}

		// Token: 0x04002B12 RID: 11026
		private static float s_orbitIntensity = 1.5f;

		// Token: 0x04002B13 RID: 11027
		private static Material s_defaultOrbitMaterial = null;

		// Token: 0x04002B14 RID: 11028
		private static readonly Dictionary<SpaceObjectType, Color> s_orbitColor = new Dictionary<SpaceObjectType, Color>
		{
			{
				SpaceObjectType.Planet,
				OrbitExtensions.s_orbitIntensity * new Color32(23, 93, byte.MaxValue, byte.MaxValue)
			},
			{
				SpaceObjectType.DwarfPlanet,
				OrbitExtensions.s_orbitIntensity * new Color32(23, 93, byte.MaxValue, byte.MaxValue)
			},
			{
				SpaceObjectType.Asteroid,
				OrbitExtensions.s_orbitIntensity * new Color32(byte.MaxValue, 120, 0, byte.MaxValue)
			},
			{
				SpaceObjectType.AsteroidalMoon,
				OrbitExtensions.s_orbitIntensity * new Color32(byte.MaxValue, 120, 0, byte.MaxValue)
			},
			{
				SpaceObjectType.Comet,
				OrbitExtensions.s_orbitIntensity * new Color32(23, 93, byte.MaxValue, byte.MaxValue)
			},
			{
				SpaceObjectType.LagrangePoint,
				OrbitExtensions.s_orbitIntensity * new Color32(byte.MaxValue, 120, 0, byte.MaxValue)
			},
			{
				SpaceObjectType.PlanetaryMoon,
				OrbitExtensions.s_orbitIntensity * new Color32(23, 93, byte.MaxValue, byte.MaxValue)
			},
			{
				SpaceObjectType.Star,
				OrbitExtensions.s_orbitIntensity * new Color32(23, 93, byte.MaxValue, byte.MaxValue)
			}
		};
	}
}
