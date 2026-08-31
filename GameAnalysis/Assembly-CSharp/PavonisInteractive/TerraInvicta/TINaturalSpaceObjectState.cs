using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007A9 RID: 1961
	public abstract class TINaturalSpaceObjectState : TISpaceObjectState
	{
		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x060040DE RID: 16606 RVA: 0x001A30DF File Offset: 0x001A12DF
		// (set) Token: 0x060040DF RID: 16607 RVA: 0x001A30E7 File Offset: 0x001A12E7
		public int maxHabTier { get; protected set; }

		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x060040E0 RID: 16608 RVA: 0x001A30F0 File Offset: 0x001A12F0
		// (set) Token: 0x060040E1 RID: 16609 RVA: 0x001A30F8 File Offset: 0x001A12F8
		[fsIgnore]
		public double sphereOfInfluence_m { get; protected set; }

		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x060040E2 RID: 16610 RVA: 0x001A3101 File Offset: 0x001A1301
		// (set) Token: 0x060040E3 RID: 16611 RVA: 0x001A3109 File Offset: 0x001A1309
		[fsIgnore]
		public float localBarycenterGravity_kps2 { get; protected set; }

		// Token: 0x17000BE3 RID: 3043
		// (get) Token: 0x060040E4 RID: 16612 RVA: 0x001A3112 File Offset: 0x001A1312
		// (set) Token: 0x060040E5 RID: 16613 RVA: 0x001A311A File Offset: 0x001A131A
		[fsIgnore]
		public double hillRadius_m { get; protected set; }

		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x060040E6 RID: 16614 RVA: 0x001A3123 File Offset: 0x001A1323
		public TINaturalSpaceObjectTemplate naturalObjectTemplate
		{
			get
			{
				return this.GetMyTemplate<TINaturalSpaceObjectTemplate>();
			}
		}

		// Token: 0x17000BE5 RID: 3045
		// (get) Token: 0x060040E7 RID: 16615 RVA: 0x001A312B File Offset: 0x001A132B
		public virtual bool supportsAerocapture
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x060040E8 RID: 16616 RVA: 0x001A312E File Offset: 0x001A132E
		public override bool isNaturalSpaceObjectState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x060040E9 RID: 16617 RVA: 0x001A3131 File Offset: 0x001A1331
		public override Searchable searchable
		{
			get
			{
				return Searchable.always;
			}
		}

		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x060040EA RID: 16618 RVA: 0x001A3134 File Offset: 0x001A1334
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x060040EB RID: 16619 RVA: 0x001A3137 File Offset: 0x001A1337
		public TIEffectTemplate GetStandardEffectToExplore()
		{
			return TemplateManager.Find<TIEffectTemplate>(this.naturalObjectTemplate.effectToExplore, false);
		}

		// Token: 0x060040EC RID: 16620 RVA: 0x001A314C File Offset: 0x001A134C
		public IEnumerable<TIEffectTemplate> GetExplorationEffectOptions()
		{
			IEnumerable<TIEffectTemplate> enumerable = Enumerable.Empty<TIEffectTemplate>().Append(TemplateManager.Find<TIEffectTemplate>(this.naturalObjectTemplate.effectToExplore, false));
			if (!string.IsNullOrEmpty(this.naturalObjectTemplate.alternativeEffectToExplore))
			{
				enumerable = enumerable.Append(TemplateManager.Find<TIEffectTemplate>(this.naturalObjectTemplate.alternativeEffectToExplore, false));
			}
			return enumerable.Where<TIEffectTemplate>((TIEffectTemplate x) => x != null);
		}

		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x060040ED RID: 16621 RVA: 0x001A31C4 File Offset: 0x001A13C4
		public virtual ulong population
		{
			get
			{
				List<TIOrbitState> list = this.orbits;
				int? num;
				if (list == null)
				{
					num = null;
				}
				else
				{
					num = new int?((from x in list.SelectMany<TIOrbitState, TIHabState>((TIOrbitState x) => x.stationsInOrbit)
						where !x.IsAlien()
						select x).Sum<TIHabState>((TIHabState y) => y.crew));
				}
				int? num2 = num;
				return ((num2 != null) ? new ulong?((ulong)((long)num2.GetValueOrDefault())) : null).GetValueOrDefault();
			}
		}

		// Token: 0x060040EE RID: 16622 RVA: 0x001A3281 File Offset: 0x001A1481
		public virtual bool Colonized()
		{
			return this.population >= TemplateManager.global.colonizedSpaceObjectValue;
		}

		// Token: 0x060040EF RID: 16623 RVA: 0x001A3298 File Offset: 0x001A1498
		public virtual bool Populous()
		{
			return this.population >= TemplateManager.global.populousSpaceObjectValue;
		}

		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x060040F0 RID: 16624 RVA: 0x001A32AF File Offset: 0x001A14AF
		public override double orbitalPeriod_s
		{
			get
			{
				return this._orbitalPeriod_s;
			}
		}

		// Token: 0x060040F1 RID: 16625 RVA: 0x001A32B7 File Offset: 0x001A14B7
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			this.orbits = new List<TIOrbitState>();
		}

		// Token: 0x060040F2 RID: 16626 RVA: 0x001A32CC File Offset: 0x001A14CC
		protected void CreateOrbitStates()
		{
			foreach (string text in this.naturalObjectTemplate.orbits)
			{
				TIOrbitTemplate tiorbitTemplate = TemplateManager.Find<TIOrbitTemplate>(text, false);
				if (tiorbitTemplate != null)
				{
					TIOrbitState tiorbitState = GameStateManager.CreateNewGameState<TIOrbitState>();
					tiorbitState.InitWithTemplate(tiorbitTemplate);
					tiorbitState.AssignToBarycenter();
				}
				else
				{
					Log.Error("Orbit Template " + text + " not found while processing " + this.templateName, Array.Empty<object>());
				}
			}
		}

		// Token: 0x060040F3 RID: 16627 RVA: 0x001A335C File Offset: 0x001A155C
		public override void PostCanvasManagerCreateInit_3()
		{
			this.maxHabTier = Mathf.Clamp((base.template as TINaturalSpaceObjectTemplate).maxHabSize, 1, 3);
		}

		// Token: 0x060040F4 RID: 16628 RVA: 0x001A337C File Offset: 0x001A157C
		public override void PostAllStartUpInit_5()
		{
			foreach (TIOrbitState tiorbitState in this.orbits.ToList<TIOrbitState>())
			{
				if (tiorbitState.deleted)
				{
					this.orbits.Remove(tiorbitState);
				}
			}
		}

		// Token: 0x060040F5 RID: 16629 RVA: 0x001A33E4 File Offset: 0x001A15E4
		public override void PostEverythingSaveRepair_8()
		{
			foreach (TISpaceFleetState tispaceFleetState in (from fleet in this.fleetsInOrbit
				where fleet.ships.Count == 0
				select fleet into x
				where !x.dummyFleet
				select x).ToList<TISpaceFleetState>())
			{
				if (tispaceFleetState.exists)
				{
					tispaceFleetState.Disband();
				}
				else
				{
					this.fleetsInOrbit.Remove(tispaceFleetState);
					tispaceFleetState.ArchiveState(true);
					GameStateManager.RemoveGameState<TISpaceFleetState>(tispaceFleetState.ID, false);
				}
			}
			using (List<TIOrbitState>.Enumerator enumerator2 = this.orbits.Where<TIOrbitState>((TIOrbitState orbit) => orbit2.semiMajorAxis_m <= 0.0 || orbit2.eccentricity >= 1.0).ToList<TIOrbitState>().GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIOrbitState orbit2 = enumerator2.Current;
					if (orbit2.fleetsInOrbit.Count > 0)
					{
						Log.Error(string.Concat(new string[]
						{
							"Hyperbolic orbit ",
							orbit2.displayName,
							" around ",
							orbit2.barycenter.displayName,
							" has ",
							orbit2.fleetsInOrbit.Count.ToString(),
							" fleets in orbit.  This should have been a trajectory and will produce unexpected behavior."
						}), Array.Empty<object>());
					}
					else
					{
						List<TISpaceFleetState> list = (from fleet in GameStateManager.IterateByClass<TISpaceFleetState>(false)
							where fleet.ref_orbit == orbit2 || (fleet.transferAssigned && (fleet.trajectory.destination == orbit2 || fleet.trajectory.originOrbit == orbit2))
							select fleet).ToList<TISpaceFleetState>();
						bool flag = true;
						foreach (TISpaceFleetState tispaceFleetState2 in list)
						{
							if (tispaceFleetState2.ref_orbit == orbit2)
							{
								flag = false;
								Log.Error(tispaceFleetState2.displayName + " has ref_orbit of " + orbit2.displayName + ", which is hyperbolic.  Hyperbolic orbits are illegal (should be elliptical or a trajectory).  Furthermore, the orbit does not list this fleet as being at it.", Array.Empty<object>());
							}
							if (tispaceFleetState2.transferAssigned)
							{
								if (tispaceFleetState2.trajectory.originOrbit == orbit2)
								{
									flag = false;
								}
								if (tispaceFleetState2.trajectory.destination == orbit2)
								{
									string text = string.Concat(new string[]
									{
										tispaceFleetState2.displayName,
										" is transfering to a hyperbolic destination: ",
										orbit2.displayName,
										" ecc = ",
										orbit2.eccentricity.ToString(),
										"semi major axis (m) = ",
										orbit2.semiMajorAxis_m.ToString(),
										"."
									});
									tispaceFleetState2.trajectory.ReconstructMissingDestinationOrbit();
									Log.Error(text + "\nNew destination is " + tispaceFleetState2.trajectory.destinationOrbit.displayName, Array.Empty<object>());
								}
							}
						}
						if (flag)
						{
							if (orbit2 is TIAdHocOrbitState)
							{
								this.orbits.Remove(orbit2);
								orbit2.ArchiveState(true);
								Debug.LogWarning("Save repair: Removed unused adhoc orbit: " + orbit2.displayName);
								GameStateManager.RemoveGameState<TIAdHocOrbitState>(orbit2.ID, false);
							}
							else
							{
								Log.Error(string.Concat(new string[]
								{
									orbit2.displayName,
									" is hyperbolic with ecc ",
									orbit2.eccentricity.ToString(),
									" and semi major axis ",
									orbit2.semiMajorAxis_m.ToString(),
									"m, but is not an ad-hoc orbit.  Natural orbits should not be hyperbolic."
								}), Array.Empty<object>());
							}
						}
					}
				}
			}
			foreach (TIAdHocOrbitState tiadHocOrbitState in (from orbit in GameStateManager.IterateByClass<TIAdHocOrbitState>(false)
				where orbit.barycenter == this && orbit.fleetsInOrbit.Count == 0 && GameStateManager.IterateByClass<TISpaceFleetState>(false).None<TISpaceFleetState>((TISpaceFleetState fleet) => fleet.ref_orbit == orbit || (fleet.transferAssigned && (fleet.trajectory.destination == orbit || fleet.trajectory.originOrbit == orbit)))
				select orbit).ToList<TIAdHocOrbitState>())
			{
				Debug.LogWarning("Save repair: Removed unused adhoc orbit: " + tiadHocOrbitState.displayName);
				this.orbits.Remove(tiadHocOrbitState);
				tiadHocOrbitState.ArchiveState(true);
				GameStateManager.RemoveGameState<TIAdHocOrbitState>(tiadHocOrbitState.ID, false);
			}
		}

		// Token: 0x060040F6 RID: 16630 RVA: 0x001A38B0 File Offset: 0x001A1AB0
		public TIOrbitState GetClosestMatchingLegalOrbitState(OrbitalElementsState orbitalElementsToMatch)
		{
			TIOrbitState tiorbitState = null;
			IEnumerable<TIOrbitState> enumerable = this.orbits.Where<TIOrbitState>((TIOrbitState x) => x.eccentricity < 1.0 && x.semiMajorAxis_m > 0.0 && !x.deleted && !x.archived);
			if (orbitalElementsToMatch.eccentricity >= 1.0 || orbitalElementsToMatch.semiMajorAxis_m <= 0.0)
			{
				CartesianState cartesianState = orbitalElementsToMatch.ToCartesianStateAtTime(TITimeState.Now().ExportTime(), this.mass_kg);
				double num = double.PositiveInfinity;
				using (IEnumerator<TIOrbitState> enumerator = enumerable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIOrbitState tiorbitState2 = enumerator.Current;
						double num2 = TISpaceAssetState.CalculateMeanAnomalyFromPosition(tiorbitState2, cartesianState.position, TITimeState.Now(), TISpaceAssetState.MeanAnomalyPrecision.Maximum);
						double magnitude = (tiorbitState2.ToOrbitalElementsState(TITimeState.Now(), 0.0).ToCartesianStateAtMeanAnomaly(num2, this.mass_kg).position - cartesianState.position).magnitude;
						if (magnitude < num)
						{
							num = magnitude;
							tiorbitState = tiorbitState2;
						}
					}
					goto IL_01B3;
				}
			}
			Vector3d eccentricVector = orbitalElementsToMatch.eccentricVector;
			double num3 = double.PositiveInfinity;
			foreach (TIOrbitState tiorbitState3 in enumerable)
			{
				OrbitalElementsState orbitalElementsState = tiorbitState3.ToOrbitalElementsState(TITimeState.Now(), 0.0);
				double num4 = (orbitalElementsState.eccentricVector - eccentricVector).magnitude + 1.0;
				double num5 = Mathd.Abs(orbitalElementsState.semiMajorAxis_m - orbitalElementsToMatch.semiMajorAxis_m) + orbitalElementsToMatch.semiMajorAxis_m * 0.1;
				double num6 = num4 * num5;
				if (num6 < num3)
				{
					num3 = num6;
					tiorbitState = tiorbitState3;
				}
			}
			IL_01B3:
			if (!(tiorbitState == null))
			{
				return tiorbitState;
			}
			if (base.isSun)
			{
				IEnumerable<TINaturalSpaceObjectState> enumerable2 = from x in GameStateManager.IterateByClass<TINaturalSpaceObjectState>(false)
					where x.barycenter == this
					select x;
				CartesianState cartesianState2 = orbitalElementsToMatch.ToCartesianStateAtTime(TITimeState.Now().ExportTime(), this.mass_kg);
				Vector3d position = cartesianState2.position;
				TINaturalSpaceObjectState tinaturalSpaceObjectState = null;
				double num7 = double.PositiveInfinity;
				foreach (TINaturalSpaceObjectState tinaturalSpaceObjectState2 in enumerable2)
				{
					double magnitude2 = (tinaturalSpaceObjectState2.ToGlobalCartesianStateAtTime(TITimeState.Now()).position - position).magnitude;
					if (magnitude2 < num7)
					{
						num7 = magnitude2;
						tinaturalSpaceObjectState = tinaturalSpaceObjectState2;
					}
				}
				OrbitalElementsState orbitalElementsState2 = (cartesianState2 - tinaturalSpaceObjectState.ToGlobalCartesianStateAtTime(TITimeState.Now())).ToOrbitalElementsState(tinaturalSpaceObjectState.mu, new DateTime?(TITimeState.Now().ExportTime()));
				return tinaturalSpaceObjectState.GetClosestMatchingLegalOrbitState(orbitalElementsState2);
			}
			Log.Error("Attempting to repair a broken save with an illegal orbit.  Said orbit is around a barycenter that has no orbits around it but is not the Sun.  No such barycenter should exist.", Array.Empty<object>());
			return GameStateManager.IterateByClass<TINaturalSpaceObjectState>(false).First<TINaturalSpaceObjectState>((TINaturalSpaceObjectState x) => x.isEarth).orbits[0];
		}

		// Token: 0x060040F7 RID: 16631 RVA: 0x001A3BD4 File Offset: 0x001A1DD4
		public List<TIOrbitState> NearbyOrbits()
		{
			List<TIOrbitState> list = new List<TIOrbitState>();
			TISpaceObjectState getSunOrbitingRelatedObject = this.GetSunOrbitingRelatedObject;
			if (getSunOrbitingRelatedObject.isNaturalSpaceObjectState)
			{
				list.AddRange(getSunOrbitingRelatedObject.ref_naturalSpaceObject.orbits);
				if (getSunOrbitingRelatedObject.isSpaceBodyState)
				{
					foreach (TISpaceBodyState tispaceBodyState in getSunOrbitingRelatedObject.ref_spaceBody.naturalSatellites)
					{
						list.AddRange(tispaceBodyState.orbits);
					}
					foreach (TILagrangePointState tilagrangePointState in getSunOrbitingRelatedObject.ref_spaceBody.lagrangePoints)
					{
						list.AddRange(tilagrangePointState.orbits);
					}
				}
			}
			return list;
		}

		// Token: 0x060040F8 RID: 16632 RVA: 0x001A3CB8 File Offset: 0x001A1EB8
		public void SetHillRadius_m()
		{
			this.hillRadius_m = this.semiMajorAxis_m * (1.0 - this.ecc) * Mathd.Pow(this.mass_kg / (3.0 * this.barycenter.mass_kg), 0.3333333333333333);
		}

		// Token: 0x060040F9 RID: 16633 RVA: 0x001A3D0D File Offset: 0x001A1F0D
		public double localAccelerationDueToGravity_ms2(double radius_m)
		{
			if (radius_m > this.sphereOfInfluence_m)
			{
				return 0.0;
			}
			return 6.67384E-11 * this.mass_kg / (radius_m * radius_m);
		}

		// Token: 0x060040FA RID: 16634 RVA: 0x001A3D36 File Offset: 0x001A1F36
		public double localEscapeVelocity_mps(double radius_m)
		{
			return Mathd.Sqrt(1.334768E-10 * this.mass_kg / radius_m);
		}

		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x060040FB RID: 16635 RVA: 0x001A3D4F File Offset: 0x001A1F4F
		public override TISpaceObjectState GetSunOrbitingRelatedObject
		{
			get
			{
				if (!base.isSun)
				{
					return this._sunOrbitingRelatedObject;
				}
				return this;
			}
		}

		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x060040FC RID: 16636 RVA: 0x001A3D61 File Offset: 0x001A1F61
		public virtual List<TIHabState> habsInSystem
		{
			get
			{
				return this.habs;
			}
		}

		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x060040FD RID: 16637 RVA: 0x001A3D69 File Offset: 0x001A1F69
		public virtual List<TIHabState> habs
		{
			get
			{
				return this.stationsInOrbit;
			}
		}

		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x060040FE RID: 16638 RVA: 0x001A3D71 File Offset: 0x001A1F71
		public List<TIHabState> stationsInOrbit
		{
			get
			{
				return this.orbits.SelectMany<TIOrbitState, TIHabState>((TIOrbitState x) => x.stationsInOrbit).ToList<TIHabState>();
			}
		}

		// Token: 0x17000BEF RID: 3055
		// (get) Token: 0x060040FF RID: 16639 RVA: 0x001A3DA4 File Offset: 0x001A1FA4
		public List<TISpaceFleetState> fleetsInOrbit
		{
			get
			{
				return (from x in this.orbits.SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit)
					where !x.archived && !x.inTransfer && x.barycenter == this
					select x).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000BF0 RID: 3056
		// (get) Token: 0x06004100 RID: 16640 RVA: 0x001A3DF1 File Offset: 0x001A1FF1
		public virtual List<TISpaceFleetState> fleetsInSystem
		{
			get
			{
				return this.fleetsInOrbit;
			}
		}

		// Token: 0x06004101 RID: 16641 RVA: 0x001A3DFC File Offset: 0x001A1FFC
		public string GetMaxTierIconPath()
		{
			switch (this.maxHabTier)
			{
			case 1:
				return TIGlobalConfig.globalConfig.pathMaxTier1Hab;
			case 2:
				return TIGlobalConfig.globalConfig.pathMaxTier2Hab;
			case 4:
				return TIGlobalConfig.globalConfig.pathMaxTier4Hab;
			}
			return TIGlobalConfig.globalConfig.pathMaxTier3Hab;
		}

		// Token: 0x06004102 RID: 16642 RVA: 0x001A3E58 File Offset: 0x001A2058
		public static TIDateTime GetNextHohmannLaunchWindowDate(TIFactionState faction, TINaturalSpaceObjectState origin, TINaturalSpaceObjectState destination, TIDateTime time, out double synodicPeriod_s)
		{
			TINaturalSpaceObjectState tinaturalSpaceObjectState = origin.FindCommonBarycenter(destination);
			if (origin.ref_naturalSpaceObject == destination.ref_naturalSpaceObject || origin.ref_naturalSpaceObject == tinaturalSpaceObjectState || destination.ref_naturalSpaceObject == tinaturalSpaceObjectState)
			{
				synodicPeriod_s = double.PositiveInfinity;
				return time;
			}
			if (origin.isLagrangePointState && origin.ref_lagrangePoint.secondaryObject == destination && (origin.ref_lagrangePoint.lagrangeValue == LagrangeValue.L3 || origin.ref_lagrangePoint.lagrangeValue == LagrangeValue.L4 || origin.ref_lagrangePoint.lagrangeValue == LagrangeValue.L5))
			{
				synodicPeriod_s = double.PositiveInfinity;
				return time;
			}
			if (destination.isLagrangePointState && destination.ref_lagrangePoint.secondaryObject == origin && (destination.ref_lagrangePoint.lagrangeValue == LagrangeValue.L3 || destination.ref_lagrangePoint.lagrangeValue == LagrangeValue.L4 || destination.ref_lagrangePoint.lagrangeValue == LagrangeValue.L5))
			{
				synodicPeriod_s = double.PositiveInfinity;
				return time;
			}
			bool flag;
			synodicPeriod_s = TISpaceObjectState.genericSynodicPeriod_s(origin, destination, out flag);
			double num = synodicPeriod_s / 86400.0;
			if (num > 219145.3125)
			{
				synodicPeriod_s = double.PositiveInfinity;
				return time;
			}
			if (origin.HohmannDates.ContainsKey(destination))
			{
				while (origin.HohmannDates[destination] < time)
				{
					origin.HohmannDates[destination].AddDays((float)num);
				}
				return origin.HohmannDates[destination];
			}
			GenericSpaceObject genericSpaceObject = new GenericSpaceObject();
			genericSpaceObject.AssignData(origin);
			GenericSpaceObject genericSpaceObject2 = new GenericSpaceObject();
			genericSpaceObject2.AssignData(destination);
			if (origin.ref_naturalSpaceObject.barycenter != tinaturalSpaceObjectState)
			{
				origin = origin.ref_naturalSpaceObject.barycenter;
				genericSpaceObject.AssignData(origin);
			}
			if (destination.ref_naturalSpaceObject.barycenter != tinaturalSpaceObjectState)
			{
				destination = destination.ref_naturalSpaceObject.barycenter;
				genericSpaceObject2.AssignData(destination);
			}
			double num2 = TISpaceObjectState.HohmannTransferTime_s(faction, genericSpaceObject, genericSpaceObject2);
			double num3 = 360.0 / (origin.orbitalPeriod_s / 86400.0);
			double num4 = 360.0 / destination.orbitalPeriod_s;
			double num5 = num4 * 86400.0 * (double)(flag ? (-1) : 1);
			double num6 = num4 * num2;
			double num7;
			for (num7 = 180.0 - num6; num7 < 0.0; num7 += 360.0)
			{
			}
			double num8 = TISpaceObjectState.MeanLongitudeBetweenTwoSpaceObjects_deg(origin, destination, time);
			double num9 = num5 - num3;
			double num10 = num7 - num8;
			if (num10 < 0.0 && num9 > 0.0)
			{
				num10 += 360.0;
			}
			else if (num10 > 0.0 && num9 < 0.0)
			{
				num10 -= 360.0;
			}
			double num11 = num10 / num9;
			return new TIDateTime(time, num11 * 86400.0);
		}

		// Token: 0x06004103 RID: 16643 RVA: 0x001A4134 File Offset: 0x001A2334
		public static List<TINaturalSpaceObjectState> GetFilteredSolarSystemGroupObjects(TISpaceBodyState Filter, bool includeSatellites)
		{
			List<TINaturalSpaceObjectState> list = new List<TINaturalSpaceObjectState>();
			TISpaceBodyState spaceBody = Filter.ref_spaceBody;
			list.Add(spaceBody);
			if (!spaceBody.isSun)
			{
				list.AddRange(spaceBody.naturalSatellites);
			}
			list.AddRange(spaceBody.lagrangePoints);
			switch (spaceBody.objectType)
			{
			case SpaceObjectType.Planet:
				list.AddRange(from x in GameStateManager.SunOrbitingLangragePoints()
					where x.secondaryObject == spaceBody
					select x);
				break;
			case SpaceObjectType.DwarfPlanet:
			case SpaceObjectType.Asteroid:
			case SpaceObjectType.Comet:
				if (spaceBody.innerSystemAsteroid(true))
				{
					list.AddRange(GameStateManager.InnerSystemAsteroids(includeSatellites));
					list.Remove(spaceBody);
				}
				else if (spaceBody.innerMainBeltAsteroid(true))
				{
					list.AddRange(GameStateManager.InnerAsteroidBelt(includeSatellites));
					list.Remove(spaceBody);
				}
				else if (spaceBody.midMainBeltAsteroid(true))
				{
					list.AddRange(GameStateManager.MidAsteroidBelt(includeSatellites));
					list.Remove(spaceBody);
				}
				else if (spaceBody.outerMainBeltAsteroid(true))
				{
					list.AddRange(GameStateManager.OuterAsteroidBelt(includeSatellites));
					list.Remove(spaceBody);
				}
				else if (spaceBody.centaur(true))
				{
					list.AddRange(GameStateManager.Centaurs(includeSatellites));
					list.Remove(spaceBody);
				}
				else if (spaceBody.kuiperBeltObject(true))
				{
					list.AddRange(GameStateManager.KuiperBeltObjects(includeSatellites));
					list.Remove(spaceBody);
				}
				break;
			case SpaceObjectType.PlanetaryMoon:
			case SpaceObjectType.AsteroidalMoon:
				list.Add(spaceBody.barycenter);
				list.AddRange(spaceBody.barycenter.ref_spaceBody.naturalSatellites);
				list.Remove(spaceBody);
				break;
			}
			return list;
		}

		// Token: 0x06004104 RID: 16644 RVA: 0x001A4324 File Offset: 0x001A2524
		public string SummaryTooltip(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.displayName);
			if (this.isSpaceBodyState)
			{
				stringBuilder.AppendLine(this.ref_spaceBody.template.descriptor1);
				string miningPotentialString = this.ref_spaceBody.GetMiningPotentialString();
				if (!string.IsNullOrEmpty(miningPotentialString))
				{
					stringBuilder.AppendLine(miningPotentialString);
					if (this.ref_spaceBody.habSites.Length != 0)
					{
						stringBuilder.AppendLine(this.ref_spaceBody.GetProfileRatingAllIconsString(faction.Prospected(this.ref_spaceBody)));
					}
				}
				if (faction.CanExplore(this.ref_spaceBody) && faction.AlienTerritoryToAvoid(this.ref_spaceBody))
				{
					stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Space.AlienTerritory")));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06004105 RID: 16645 RVA: 0x001A43E8 File Offset: 0x001A25E8
		public string SolarInsolationIconPath(bool inline = false)
		{
			float num;
			if (this.orbits.Any<TIOrbitState>())
			{
				num = TIHabModuleState.NaturalSolarPowerMultiplier(this.orbits.MaxBy<TIOrbitState, float>((TIOrbitState x) => x.solarMultiplier));
			}
			else
			{
				num = TIHabModuleState.NaturalSolarPowerMultiplier(this.orbits.MaxBy<TIOrbitState, float>((TIOrbitState x) => x.solarMultiplier));
			}
			if (num > 0.75f * TIGlobalValuesState.GlobalValues.maxSolar)
			{
				if (!inline)
				{
					return TemplateManager.global.pathResMaxIcon;
				}
				return TemplateManager.global.level4ResourcesInlineSpritePath;
			}
			else if (num > 0.25f * TIGlobalValuesState.GlobalValues.maxSolar)
			{
				if (!inline)
				{
					return TemplateManager.global.pathResHighIcon;
				}
				return TemplateManager.global.level3ResourcesInlineSpritePath;
			}
			else if ((double)num > 0.14 * (double)TIGlobalValuesState.GlobalValues.maxSolar)
			{
				if (!inline)
				{
					return TemplateManager.global.pathResMedIcon;
				}
				return TemplateManager.global.level2ResourcesInlineSpritePath;
			}
			else if ((double)num > 0.05 * (double)TIGlobalValuesState.GlobalValues.maxSolar)
			{
				if (!inline)
				{
					return TemplateManager.global.pathResLowIcon;
				}
				return TemplateManager.global.level1ResourcesInlineSpritePath;
			}
			else
			{
				if (!inline)
				{
					return TemplateManager.global.pathResNoneIcon;
				}
				return TemplateManager.global.zeroResourcesInlineSpritePath;
			}
		}

		// Token: 0x06004106 RID: 16646 RVA: 0x001A4538 File Offset: 0x001A2738
		public string SolarTip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.orbits.Count > 0)
			{
				float num = this.orbits.Min<TIOrbitState>((TIOrbitState x) => x.solarMultiplier);
				float num2 = this.orbits.Max<TIOrbitState>((TIOrbitState x) => x.solarMultiplier);
				IEnumerable<TIHabModuleTemplate> enumerable = TemplateManager.HabModuleTemplates.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.FactionCanBuild(GameControl.control.activePlayer) && x.SpecialRules.Contains(HabModuleSpecialRule.Solar_Power_Variable_Output));
				int? num3;
				if (enumerable == null)
				{
					num3 = null;
				}
				else
				{
					TIHabModuleTemplate tihabModuleTemplate = enumerable.MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.power);
					num3 = ((tihabModuleTemplate != null) ? new int?(tihabModuleTemplate.power) : null);
				}
				int? num4 = num3;
				int valueOrDefault = num4.GetValueOrDefault();
				if (num2 * (float)valueOrDefault < 1f)
				{
					stringBuilder.Append(Loc.T("UI.Space.NoSolar"));
				}
				else
				{
					if (num == num2)
					{
						stringBuilder.Append(Loc.T("UI.Space.SolarSummary_OrbitOne", new object[] { TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(num, 7, 1, true, false)) }));
					}
					else
					{
						stringBuilder.Append(Loc.T("UI.Space.SolarSummary_OrbitMany", new object[]
						{
							TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(num, 7, 1, true, false)),
							TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(num2, 7, 1, true, false))
						}));
					}
					stringBuilder.Append(Loc.T("UI.Space.SolarSummary_Orbit"));
					if (this.isSpaceBodyState && this.ref_spaceBody.habSites.Length != 0)
					{
						TISpaceBodyState ref_spaceBody = this.ref_spaceBody;
						float num5 = ref_spaceBody.habSites.Min<TIHabSiteState>((TIHabSiteState x) => x.solarMultiplier);
						float num6 = ref_spaceBody.habSites.Max<TIHabSiteState>((TIHabSiteState x) => x.solarMultiplier);
						stringBuilder.Append(Loc.T("UI.Space.SolarSummary_OrbitBody"));
						stringBuilder.AppendLine().AppendLine();
						if (num6 * (float)valueOrDefault < 1f)
						{
							stringBuilder.AppendLine(Loc.T("UI.Space.NoSolarSurface"));
						}
						else
						{
							if (num5 == num6)
							{
								stringBuilder.Append(Loc.T("UI.Space.SolarSummary_SitesOne", new object[] { TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(num5, 7, 1, true, false)) }));
							}
							else
							{
								stringBuilder.Append(Loc.T("UI.Space.SolarSummary_SitesMany", new object[]
								{
									TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(num5, 7, 1, true, false)),
									TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(num6, 7, 1, true, false))
								}));
							}
							stringBuilder.Append(Loc.T("UI.Space.SolarSummary_Sites"));
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04002792 RID: 10130
		public List<TIOrbitState> orbits;

		// Token: 0x04002793 RID: 10131
		public Dictionary<TINaturalSpaceObjectState, TIDateTime> HohmannDates = new Dictionary<TINaturalSpaceObjectState, TIDateTime>();

		// Token: 0x04002798 RID: 10136
		protected double _orbitalPeriod_s;

		// Token: 0x04002799 RID: 10137
		protected TINaturalSpaceObjectState _sunOrbitingRelatedObject;

		// Token: 0x0400279A RID: 10138
		public const int maxMaxHabTier = 3;
	}
}
