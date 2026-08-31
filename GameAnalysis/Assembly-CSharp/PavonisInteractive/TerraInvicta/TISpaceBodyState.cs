using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007AF RID: 1967
	public class TISpaceBodyState : TINaturalSpaceObjectState
	{
		// Token: 0x17000C4F RID: 3151
		// (get) Token: 0x06004235 RID: 16949 RVA: 0x001ABD14 File Offset: 0x001A9F14
		// (set) Token: 0x06004236 RID: 16950 RVA: 0x001ABD1C File Offset: 0x001A9F1C
		[fsIgnore]
		public Quaterniond currentTilt { get; private set; }

		// Token: 0x17000C50 RID: 3152
		// (get) Token: 0x06004237 RID: 16951 RVA: 0x001ABD25 File Offset: 0x001A9F25
		public override bool isSpaceBodyState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000C51 RID: 3153
		// (get) Token: 0x06004238 RID: 16952 RVA: 0x001ABD28 File Offset: 0x001A9F28
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000C52 RID: 3154
		// (get) Token: 0x06004239 RID: 16953 RVA: 0x001ABD2C File Offset: 0x001A9F2C
		public override List<TIFactionState> ref_factions
		{
			get
			{
				if (!base.isEarth)
				{
					return this.habSites.SelectMany<TIHabSiteState, TIFactionState>((TIHabSiteState x) => x.ref_factions).Distinct<TIFactionState>().ToList<TIFactionState>();
				}
				return GameStateManager.AllFactions().ToList<TIFactionState>();
			}
		}

		// Token: 0x17000C53 RID: 3155
		// (get) Token: 0x0600423A RID: 16954 RVA: 0x001ABD80 File Offset: 0x001A9F80
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000C54 RID: 3156
		// (get) Token: 0x0600423B RID: 16955 RVA: 0x001ABD83 File Offset: 0x001A9F83
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000C55 RID: 3157
		// (get) Token: 0x0600423C RID: 16956 RVA: 0x001ABD86 File Offset: 0x001A9F86
		public override double semiMajorAxis_m
		{
			get
			{
				return this._semimajorAxis_m;
			}
		}

		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x0600423D RID: 16957 RVA: 0x001ABD8E File Offset: 0x001A9F8E
		public override double ecc
		{
			get
			{
				return this._ecc;
			}
		}

		// Token: 0x17000C57 RID: 3159
		// (get) Token: 0x0600423E RID: 16958 RVA: 0x001ABD96 File Offset: 0x001A9F96
		public override double inclination_Rad
		{
			get
			{
				return this._inclination_Rad;
			}
		}

		// Token: 0x17000C58 RID: 3160
		// (get) Token: 0x0600423F RID: 16959 RVA: 0x001ABD9E File Offset: 0x001A9F9E
		public override double longAscendingNode_Rad
		{
			get
			{
				return this._longAscendingNode_Rad;
			}
		}

		// Token: 0x17000C59 RID: 3161
		// (get) Token: 0x06004240 RID: 16960 RVA: 0x001ABDA6 File Offset: 0x001A9FA6
		public override double argPeriapsis_Rad
		{
			get
			{
				return this._argPeriapsis_Rad;
			}
		}

		// Token: 0x17000C5A RID: 3162
		// (get) Token: 0x06004241 RID: 16961 RVA: 0x001ABDAE File Offset: 0x001A9FAE
		public override double meanAnomalyAtEpoch_Rad
		{
			get
			{
				return this._meanAnomalyAtEpoch_Rad;
			}
		}

		// Token: 0x17000C5B RID: 3163
		// (get) Token: 0x06004242 RID: 16962 RVA: 0x001ABDB6 File Offset: 0x001A9FB6
		public virtual string mapResource
		{
			get
			{
				return this.template.MapResource;
			}
		}

		// Token: 0x17000C5C RID: 3164
		// (get) Token: 0x06004243 RID: 16963 RVA: 0x001ABDC3 File Offset: 0x001A9FC3
		public virtual double mapScale
		{
			get
			{
				return (double)this.template.MapScale;
			}
		}

		// Token: 0x17000C5D RID: 3165
		// (get) Token: 0x06004244 RID: 16964 RVA: 0x001ABDD1 File Offset: 0x001A9FD1
		public new TISpaceBodyTemplate template
		{
			get
			{
				return this.GetMyTemplate<TISpaceBodyTemplate>();
			}
		}

		// Token: 0x17000C5E RID: 3166
		// (get) Token: 0x06004245 RID: 16965 RVA: 0x001ABDD9 File Offset: 0x001A9FD9
		public bool irradiated
		{
			get
			{
				return this.template.irradiated;
			}
		}

		// Token: 0x17000C5F RID: 3167
		// (get) Token: 0x06004246 RID: 16966 RVA: 0x001ABDE6 File Offset: 0x001A9FE6
		public float irradiatedMultiplier
		{
			get
			{
				return this.template.irradiatedMultiplier;
			}
		}

		// Token: 0x17000C60 RID: 3168
		// (get) Token: 0x06004247 RID: 16967 RVA: 0x001ABDF3 File Offset: 0x001A9FF3
		public Atmosphere atmosphere
		{
			get
			{
				return this.template.atmosphere;
			}
		}

		// Token: 0x17000C61 RID: 3169
		// (get) Token: 0x06004248 RID: 16968 RVA: 0x001ABE00 File Offset: 0x001AA000
		public override bool supportsAerocapture
		{
			get
			{
				return this.atmosphere >= Atmosphere.Thin;
			}
		}

		// Token: 0x17000C62 RID: 3170
		// (get) Token: 0x06004249 RID: 16969 RVA: 0x001ABE0E File Offset: 0x001AA00E
		public bool restrictsOrbitalBombardment
		{
			get
			{
				return this.atmosphere >= Atmosphere.Standard;
			}
		}

		// Token: 0x17000C63 RID: 3171
		// (get) Token: 0x0600424A RID: 16970 RVA: 0x001ABE1C File Offset: 0x001AA01C
		public override ulong population
		{
			get
			{
				if (base.isEarth)
				{
					return (ulong)((double)GameStateManager.AllRegions().Sum<TIRegionState>((TIRegionState x) => x.populationInMillions) * 1000000.0);
				}
				TIHabSiteState[] array = this.habSites;
				int? num;
				if (array == null)
				{
					num = null;
				}
				else
				{
					num = new int?((from x in array
						select x.hab into x
						where x != null && !x.IsAlien()
						select x).Sum<TIHabState>((TIHabState y) => y.crew));
				}
				int? num2 = num;
				ulong? num3 = ((num2 != null) ? new ulong?((ulong)((long)num2.GetValueOrDefault())) : null);
				List<TIOrbitState> orbits = this.orbits;
				int? num4;
				if (orbits == null)
				{
					num4 = null;
				}
				else
				{
					num4 = new int?((from x in orbits.SelectMany<TIOrbitState, TIHabState>((TIOrbitState x) => x.stationsInOrbit)
						where !x.IsAlien()
						select x).Sum<TIHabState>((TIHabState y) => y.crew));
				}
				num2 = num4;
				ulong? num5 = num3 + ((num2 != null) ? new ulong?((ulong)((long)num2.GetValueOrDefault())) : null);
				if (num5 == null)
				{
					List<TIOrbitState> orbits2 = this.orbits;
					int? num6;
					if (orbits2 == null)
					{
						num6 = null;
					}
					else
					{
						num6 = new int?((from x in orbits2.SelectMany<TIOrbitState, TIHabState>((TIOrbitState x) => x.stationsInOrbit)
							where !x.IsAlien()
							select x).Sum<TIHabState>((TIHabState y) => y.crew));
					}
					num2 = num6;
					return ((num2 != null) ? new ulong?((ulong)((long)num2.GetValueOrDefault())) : null).GetValueOrDefault();
				}
				return num5.GetValueOrDefault();
			}
		}

		// Token: 0x0600424B RID: 16971 RVA: 0x001AC0B8 File Offset: 0x001AA2B8
		public bool innerSystemAsteroid(bool includeSatellites)
		{
			return this.barycenter != null && ((includeSatellites && this.barycenter.ref_spaceBody.innerSystemAsteroid(false)) || (this.barycenter.isSun && base.periapsis_AU <= GameStateManager.Mars().apoapsis_AU && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.Comet)));
		}

		// Token: 0x0600424C RID: 16972 RVA: 0x001AC134 File Offset: 0x001AA334
		public bool innerMainBeltAsteroid(bool includeSatellites)
		{
			return this.barycenter != null && ((includeSatellites && this.barycenter.ref_spaceBody.innerMainBeltAsteroid(false)) || (this.barycenter.isSun && base.semiMajorAxis_AU > GameStateManager.Mars().apoapsis_AU && base.semiMajorAxis_AU < (double)TemplateManager.global.innerMiddleBeltLine && !GameStateManager.InnerSystemAsteroids(true).Contains(this) && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.Comet)));
		}

		// Token: 0x0600424D RID: 16973 RVA: 0x001AC1D4 File Offset: 0x001AA3D4
		public bool midMainBeltAsteroid(bool includeSatellites)
		{
			return this.barycenter != null && ((includeSatellites && this.barycenter.ref_spaceBody.midMainBeltAsteroid(false)) || (this.barycenter.isSun && base.semiMajorAxis_AU >= (double)TemplateManager.global.innerMiddleBeltLine && base.semiMajorAxis_AU < (double)TemplateManager.global.middleOuterBeltLine && !GameStateManager.InnerSystemAsteroids(true).Contains(this) && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.Comet)));
		}

		// Token: 0x0600424E RID: 16974 RVA: 0x001AC278 File Offset: 0x001AA478
		public bool outerMainBeltAsteroid(bool includeSatellites)
		{
			return this.barycenter != null && ((includeSatellites && this.barycenter.ref_spaceBody.outerMainBeltAsteroid(false)) || (this.barycenter.isSun && base.semiMajorAxis_AU >= (double)TemplateManager.global.middleOuterBeltLine && base.semiMajorAxis_AU < GameStateManager.Jupiter().periapsis_AU && !GameStateManager.InnerSystemAsteroids(true).Contains(this) && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.Comet)));
		}

		// Token: 0x0600424F RID: 16975 RVA: 0x001AC318 File Offset: 0x001AA518
		public bool centaur(bool includeSatellites)
		{
			return this.barycenter != null && ((includeSatellites && this.barycenter.ref_spaceBody.centaur(false)) || (this.barycenter.isSun && base.semiMajorAxis_AU > GameStateManager.Jupiter().periapsis_AU && base.semiMajorAxis_AU <= GameStateManager.Neptune().apoapsis_AU && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.PlanetaryMoon || this.objectType == SpaceObjectType.Comet)));
		}

		// Token: 0x06004250 RID: 16976 RVA: 0x001AC3B0 File Offset: 0x001AA5B0
		public bool kuiperBeltObject(bool includeSatellites)
		{
			return this.barycenter != null && ((includeSatellites && this.barycenter.ref_spaceBody.kuiperBeltObject(false)) || (this.barycenter.isSun && base.semiMajorAxis_AU > GameStateManager.Neptune().apoapsis_AU && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.PlanetaryMoon || this.objectType == SpaceObjectType.Comet)));
		}

		// Token: 0x17000C64 RID: 3172
		// (get) Token: 0x06004251 RID: 16977 RVA: 0x001AC435 File Offset: 0x001AA635
		public double circumfrence_km
		{
			get
			{
				return 6.283185307179586 * this.meanRadius_km;
			}
		}

		// Token: 0x17000C65 RID: 3173
		// (get) Token: 0x06004252 RID: 16978 RVA: 0x001AC447 File Offset: 0x001AA647
		public override string modelResource
		{
			get
			{
				return this.currentModelResource;
			}
		}

		// Token: 0x17000C66 RID: 3174
		// (get) Token: 0x06004253 RID: 16979 RVA: 0x001AC44F File Offset: 0x001AA64F
		public double maxRadiusDimension_km
		{
			get
			{
				return Mathd.Max(new double[] { this.dimensionX_km, this.dimensionY_km, this.dimensionZ_km }) / 2.0;
			}
		}

		// Token: 0x17000C67 RID: 3175
		// (get) Token: 0x06004254 RID: 16980 RVA: 0x001AC481 File Offset: 0x001AA681
		public double maxRadiusDimension_m
		{
			get
			{
				return 1000.0 * this.maxRadiusDimension_km;
			}
		}

		// Token: 0x06004255 RID: 16981 RVA: 0x001AC493 File Offset: 0x001AA693
		public override bool Colonized()
		{
			return this.GetSunOrbitingRelatedObject.isEarth || base.Colonized();
		}

		// Token: 0x06004256 RID: 16982 RVA: 0x001AC4AA File Offset: 0x001AA6AA
		public override bool Populous()
		{
			return this.GetSunOrbitingRelatedObject.isEarth || base.Populous();
		}

		// Token: 0x06004257 RID: 16983 RVA: 0x001AC4C4 File Offset: 0x001AA6C4
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TISpaceBodyTemplate tispaceBodyTemplate = template as TISpaceBodyTemplate;
			if (tispaceBodyTemplate == null)
			{
				return;
			}
			this.templateName = tispaceBodyTemplate.dataName;
			this.currentTilt = Quaterniond.AngleAxis(this.tilt_Deg, new Vector3d(Mathd.Cos((double)this.tiltSkew_Deg * 0.017453292519943295), 0.0, Mathd.Sin((double)this.tiltSkew_Deg * 0.017453292519943295)));
			this.nations = new List<TINationState>();
			this.naturalSatellites = new List<TISpaceBodyState>();
			this.lagrangePoints = new List<TILagrangePointState>();
			this._polarRadius_m = this.GetPolarRadius_m();
			this._meanRadius_km = this.GetMeanRadius_km();
			this._meanRadius_m = this._meanRadius_km * 1000.0;
		}

		// Token: 0x06004258 RID: 16984 RVA: 0x001AC58C File Offset: 0x001AA78C
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			base.PostGameStateCreateInit_OnCreationOnly_1();
			base.CreateOrbitStates();
			if (this.orbits.Count == 0 && !base.isSun)
			{
				Log.Error(this.displayName + " has no orbits around it.", Array.Empty<object>());
			}
			List<TIHabSiteState> list = new List<TIHabSiteState>();
			bool flag = false;
			foreach (string text in this.template.habSites)
			{
				TIHabSiteTemplate tihabSiteTemplate = TemplateManager.Find<TIHabSiteTemplate>(text, false);
				if (tihabSiteTemplate != null)
				{
					TIHabSiteState tihabSiteState = GameStateManager.CreateNewGameState<TIHabSiteState>();
					tihabSiteState.InitWithTemplate(tihabSiteTemplate);
					if (tihabSiteTemplate.latitude == null)
					{
						flag = true;
					}
					list.Add(tihabSiteState);
				}
				else
				{
					Log.Error("Hab Site Template " + text + " not found while processing " + this.templateName, Array.Empty<object>());
				}
			}
			if (list.Count > 1 && flag)
			{
				foreach (TIHabSiteState tihabSiteState2 in list)
				{
					foreach (TIHabSiteState tihabSiteState3 in list)
					{
						if (tihabSiteState2 != tihabSiteState3 && TISpaceBodyState.<PostGameStateCreateInit_OnCreationOnly_1>g__SitesTooClose|81_0(tihabSiteState2, tihabSiteState3))
						{
							if (tihabSiteState2.latitude > tihabSiteState3.latitude)
							{
								tihabSiteState2.latitude += 10f;
							}
							else
							{
								tihabSiteState2.latitude -= 10f;
							}
						}
					}
				}
			}
			this.habSites = list.ToArray();
			if (this.habSites.Length == 0 && !base.isEarth && !base.isSun && this.atmosphere <= Atmosphere.Standard)
			{
				Log.Error(this.displayName + " has no habSites on it.", Array.Empty<object>());
			}
		}

		// Token: 0x06004259 RID: 16985 RVA: 0x001AC794 File Offset: 0x001AA994
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.currentTilt = Quaterniond.AngleAxis(this.tilt_Deg, new Vector3d(Mathd.Cos((double)this.tiltSkew_Deg * 0.017453292519943295), 0.0, Mathd.Sin((double)this.tiltSkew_Deg * 0.017453292519943295)));
			this._semimajorAxis_m = this.template.SemiMajorAxis_m;
			this._orbitalPeriod_s = ((this.barycenter != null) ? (6.283185307179586 * Mathd.Sqrt(this.semiMajorAxis_m * this.semiMajorAxis_m * this.semiMajorAxis_m / (6.67384E-11 * this.barycenter.mass_kg))) : 1.0);
			this._ecc = this.template.Eccentricity;
			this._inclination_Rad = this.template.Inclination_Rad;
			this._longAscendingNode_Rad = this.template.LongitudeAscendingNode_Rad;
			this._argPeriapsis_Rad = this.template.ArgumentPeriapsis_Rad;
			this._meanAnomalyAtEpoch_Rad = this.template.MeanAnomalyAtEpoch_Rad;
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this._rotationPeriod_Hours = this.GetRotationPeriod_Hours();
			this._polarRadius_m = this.GetPolarRadius_m();
			this._meanRadius_km = this.GetMeanRadius_km();
			this._meanRadius_m = this._meanRadius_km * 1000.0;
			if (!base.isSun)
			{
				base.sphereOfInfluence_m = this.semiMajorAxis_m * Mathd.Pow(this.mass_kg / this.barycenter.mass_kg, 0.4);
				base.localBarycenterGravity_kps2 = (float)(6.67384E-11 * this.barycenter.mass_kg / (this.semiMajorAxis_m * this.semiMajorAxis_m) / 1000.0);
				base.SetHillRadius_m();
			}
			else
			{
				base.sphereOfInfluence_m = double.PositiveInfinity;
				base.hillRadius_m = double.PositiveInfinity;
			}
			this._sunOrbitingRelatedObject = (TINaturalSpaceObjectState)TISpaceObjectState.GetSunOrbitingRelatedObject_static(this);
			this.naturalSatellites = (from spaceBody in GameStateManager.IterateByClass<TISpaceBodyState>(false)
				where spaceBody.barycenter == this
				select spaceBody).ToList<TISpaceBodyState>();
			this.lagrangePoints = (from point in GameStateManager.IterateByClass<TILagrangePointState>(false)
				where point.barycenter == this
				select point).ToList<TILagrangePointState>();
			this.north_pole_localized_coordinates_offset = Quaternion.AngleAxis(0f, -Vector3.up) * Quaternion.AngleAxis(90f, -Vector3.right) * Vector3.forward * (float)this.polarRadius_m;
			if (this.solarMirrorBonus == null)
			{
				this.solarMirrorBonus = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, int>((TIFactionState x) => x, (TIFactionState x) => 0);
			}
			this.solarMultiplier = TIHabModuleState.SetLocationSolarPowerMultiplier(this);
		}

		// Token: 0x0600425A RID: 16986 RVA: 0x001ACA84 File Offset: 0x001AAC84
		public override void PostInitializationInit_4()
		{
			if (!base.isSun)
			{
				this.alienTerritory = this.surfaceBases.Contains(GameStateManager.AlienFaction().primaryHab) || this.barycenter.ref_spaceBody.surfaceBases.Contains(GameStateManager.AlienFaction().primaryHab);
				if (this.habSites.Length != 0)
				{
					if (this.orbits.Count<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit) == 0)
					{
						Log.Error(this.displayName + "has sites but no interface orbit.", Array.Empty<object>());
					}
				}
			}
			else
			{
				this.alienTerritory = false;
			}
			this.SetModelResource();
		}

		// Token: 0x0600425B RID: 16987 RVA: 0x001ACB3C File Offset: 0x001AAD3C
		public Vector3d NorthPolePosition(TIDateTime time)
		{
			Quaterniond spatialRotation = this.SpatialRotation;
			Quaternion quaternion = Quaternion.AngleAxis((float)this.GetSurfaceRotation_Rad(time) * 57.29578f, Vector3.up);
			Vector3 vector = (Quaternion)spatialRotation * quaternion * this.north_pole_localized_coordinates_offset;
			return this.GetGlobalPositionAtTime(time) + new Vector3d(vector.x, vector.z, vector.y);
		}

		// Token: 0x0600425C RID: 16988 RVA: 0x001ACBA4 File Offset: 0x001AADA4
		public ValueTuple<Vector3, Vector3> GetForwardAndUp(TIDateTime time)
		{
			Quaterniond spatialRotation = this.SpatialRotation;
			Quaternion quaternion = Quaternion.AngleAxis((float)this.GetSurfaceRotation_Rad(time) * 57.29578f, Vector3.up);
			Quaternion quaternion2 = (Quaternion)spatialRotation * quaternion;
			return new ValueTuple<Vector3, Vector3>((quaternion2 * Vector3.forward).XZY(), (quaternion2 * Vector3.up).XZY());
		}

		// Token: 0x17000C68 RID: 3176
		// (get) Token: 0x0600425D RID: 16989 RVA: 0x001ACC01 File Offset: 0x001AAE01
		public override Quaterniond SpatialRotation
		{
			get
			{
				if (!(this.barycenter != null))
				{
					return this.currentTilt;
				}
				return this.currentTilt * this.barycenter.SpatialRotation;
			}
		}

		// Token: 0x0600425E RID: 16990 RVA: 0x001ACC2E File Offset: 0x001AAE2E
		private double GetRotationPeriod_Hours()
		{
			if (this.template.rotationPeriod_strHours == "Lock")
			{
				return base.orbitalPeriod_Hours;
			}
			return TIUtilities.GetDoubleValue(this.template.rotationPeriod_strHours);
		}

		// Token: 0x17000C69 RID: 3177
		// (get) Token: 0x0600425F RID: 16991 RVA: 0x001ACC5E File Offset: 0x001AAE5E
		public double rotationPeriod_Hours
		{
			get
			{
				return this._rotationPeriod_Hours;
			}
		}

		// Token: 0x17000C6A RID: 3178
		// (get) Token: 0x06004260 RID: 16992 RVA: 0x001ACC66 File Offset: 0x001AAE66
		public double rotationperiod_s
		{
			get
			{
				return this._rotationPeriod_Hours * 3600.0;
			}
		}

		// Token: 0x17000C6B RID: 3179
		// (get) Token: 0x06004261 RID: 16993 RVA: 0x001ACC78 File Offset: 0x001AAE78
		public double oblateness
		{
			get
			{
				if (this.template.oblateness == null)
				{
					return (this.dimensionX_km - this.dimensionZ_km) / this.dimensionX_km;
				}
				return (double)this.template.oblateness.Value;
			}
		}

		// Token: 0x17000C6C RID: 3180
		// (get) Token: 0x06004262 RID: 16994 RVA: 0x001ACCB2 File Offset: 0x001AAEB2
		public double polarRadius_m
		{
			get
			{
				return this._polarRadius_m;
			}
		}

		// Token: 0x17000C6D RID: 3181
		// (get) Token: 0x06004263 RID: 16995 RVA: 0x001ACCBA File Offset: 0x001AAEBA
		public double polarRadius_km
		{
			get
			{
				return this._polarRadius_m / 1000.0;
			}
		}

		// Token: 0x06004264 RID: 16996 RVA: 0x001ACCCC File Offset: 0x001AAECC
		private double GetPolarRadius_m()
		{
			if (this.template.equatorialRadius_km != null)
			{
				return 1000.0 * (this.template.equatorialRadius_km * (1.0 - this.oblateness)).Value;
			}
			if (this.template.dimensionZ_km != null)
			{
				return 1000.0 * this.template.dimensionZ_km.Value;
			}
			return 1000.0 * this.meanRadius_km;
		}

		// Token: 0x17000C6E RID: 3182
		// (get) Token: 0x06004265 RID: 16997 RVA: 0x001ACD7C File Offset: 0x001AAF7C
		public override double meanRadius_km
		{
			get
			{
				return this._meanRadius_km;
			}
		}

		// Token: 0x06004266 RID: 16998 RVA: 0x001ACD84 File Offset: 0x001AAF84
		private double GetMeanRadius_km()
		{
			if (this.template.meanRadius_km != null)
			{
				return this.template.meanRadius_km.Value;
			}
			if (this.template.equatorialRadius_km != null)
			{
				double? num = 2.0 * this.template.equatorialRadius_km;
				double polarRadius_km = this.polarRadius_km;
				return ((num != null) ? new double?((num.GetValueOrDefault() + polarRadius_km) / 3.0) : null).Value;
			}
			return (this.dimensionX_km + this.dimensionY_km + this.dimensionZ_km) / 6.0;
		}

		// Token: 0x17000C6F RID: 3183
		// (get) Token: 0x06004267 RID: 16999 RVA: 0x001ACE5C File Offset: 0x001AB05C
		public override double meanRadius_m
		{
			get
			{
				return this._meanRadius_m;
			}
		}

		// Token: 0x17000C70 RID: 3184
		// (get) Token: 0x06004268 RID: 17000 RVA: 0x001ACE64 File Offset: 0x001AB064
		public double escapeVelocity_mps
		{
			get
			{
				return Mathd.Sqrt(1.334768E-10 * this.mass_kg / this.meanRadius_m);
			}
		}

		// Token: 0x17000C71 RID: 3185
		// (get) Token: 0x06004269 RID: 17001 RVA: 0x001ACE82 File Offset: 0x001AB082
		public double escapeVelocity_kps
		{
			get
			{
				return this.escapeVelocity_mps / 1000.0;
			}
		}

		// Token: 0x0600426A RID: 17002 RVA: 0x001ACE94 File Offset: 0x001AB094
		public double relativeEnergyForMining(TIFactionState faction)
		{
			double num = this.escapeVelocityforMining_kps(faction);
			double num2 = num * num / 2.0;
			double num3 = 0.0;
			TISpaceObjectState getSunOrbitingRelatedObject = this.GetSunOrbitingRelatedObject;
			if (faction != null && faction.IsAlienFaction && getSunOrbitingRelatedObject != faction.primaryHab.ref_spaceBody.GetSunOrbitingRelatedObject)
			{
				num3 = 40.0 - this.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
			}
			else if (faction.IsActiveHumanFaction && !this.barycenter.isEarth)
			{
				num3 = this.GetSunOrbitingRelatedObject.semiMajorAxis_AU * 10.0;
			}
			return (num2 + num3) * 0.004999999888241291;
		}

		// Token: 0x0600426B RID: 17003 RVA: 0x001ACF38 File Offset: 0x001AB138
		public double escapeVelocityforMining_kps(TIFactionState faction)
		{
			if (this._escapeVelocityForMining_kps == -1.0)
			{
				double num = this.escapeVelocity_mps + this.DragVelocityPenaltyToReachOrbit_kps() * 1000.0;
				if (base.isaMoon)
				{
					if ((this.barycenter.isEarth && faction.IsActiveHumanFaction) || (faction.IsAlienFaction && this.barycenter == faction.primaryHab.ref_spaceBody))
					{
						double num2 = this.barycenter.localEscapeVelocity_mps(this.semiMajorAxis_m) / 2.0;
						this._escapeVelocityForMining_kps = Mathd.Sqrt(num * num + num2 * num2) / 1000.0;
					}
					else
					{
						double num3 = this.barycenter.localEscapeVelocity_mps(this.semiMajorAxis_m);
						double num4 = this.barycenter.barycenter.localEscapeVelocity_mps(this.barycenter.semiMajorAxis_m) / 2.0;
						this._escapeVelocityForMining_kps = Mathd.Sqrt(num * num + num3 * num3 + num4 * num4) / 1000.0;
					}
				}
				else
				{
					double num5 = this.barycenter.localEscapeVelocity_mps(this.semiMajorAxis_m) / 2.0;
					this._escapeVelocityForMining_kps = Mathd.Sqrt(num * num + num5 * num5) / 1000.0;
				}
			}
			return this._escapeVelocityForMining_kps;
		}

		// Token: 0x0600426C RID: 17004 RVA: 0x001AD08C File Offset: 0x001AB28C
		public string AtmosphereIconPath()
		{
			return new StringBuilder("icons_2d/ICO_Atmo_").Append(((int)this.atmosphere).ToString()).ToString();
		}

		// Token: 0x0600426D RID: 17005 RVA: 0x001AD0BC File Offset: 0x001AB2BC
		public string AtmosphereDescription()
		{
			if (this.objectType == SpaceObjectType.Comet)
			{
				return Loc.T("UI.Space.Atmo_Comet", new object[] { 0.5f.ToPercent("P0") });
			}
			if (this.atmosphere == Atmosphere.Thick && this.habSites.Length == 0)
			{
				return Loc.T("UI.Space.Atmo_Thick_NoSites");
			}
			if (this.atmosphere == Atmosphere.Standard && this.habSites.Length == 0)
			{
				return Loc.T("UI.Space.Atmo_Standard_NoSites", new object[] { this.LaserEffectivenessFactorThroughAtmo().ToPercent("P0") });
			}
			return Loc.T(new StringBuilder("UI.Space.Atmo_").Append(this.atmosphere.ToString()).ToString(), new object[]
			{
				this.LaserEffectivenessFactorThroughAtmo().ToPercent("P0"),
				this.DragVelocityPenaltyToReachOrbit_kps().ToString("N2"),
				this.DragDeltaVSavingsToLand_Frac(false).ToPercent("P0"),
				this.DragDeltaVSavingsToLand_Frac(true).ToPercent("P0"),
				TIHabModuleState.AtmosphereSolarModifier(this).ToPercent("P0")
			});
		}

		// Token: 0x0600426E RID: 17006 RVA: 0x001AD1E0 File Offset: 0x001AB3E0
		public double DragVelocityPenaltyToReachOrbit_kps()
		{
			switch (this.atmosphere)
			{
			case Atmosphere.Thin:
				return 0.05000000074505806;
			case Atmosphere.Standard:
				return 0.5;
			case Atmosphere.Thick:
				return 15.0;
			case Atmosphere.Massive:
				return 30.0;
			}
			return 0.0;
		}

		// Token: 0x0600426F RID: 17007 RVA: 0x001AD248 File Offset: 0x001AB448
		public double DragDeltaVSavingsToLand_Frac(bool aerodynamic)
		{
			switch (this.atmosphere)
			{
			case Atmosphere.Thin:
				return (double)(aerodynamic ? 0.325f : 0.5f);
			case Atmosphere.Standard:
				return (double)(aerodynamic ? 0.85f : 0.95f);
			case Atmosphere.Thick:
				return (double)(aerodynamic ? 0.9f : 0.95f);
			case Atmosphere.Massive:
				return (double)(aerodynamic ? 0.925f : 0.95f);
			}
			return 0.0;
		}

		// Token: 0x06004270 RID: 17008 RVA: 0x001AD2CC File Offset: 0x001AB4CC
		public float LaserEffectivenessFactorThroughAtmo()
		{
			switch (this.atmosphere)
			{
			case Atmosphere.Trace:
				return 0.99f;
			case Atmosphere.Thin:
				return 0.95f;
			case Atmosphere.Standard:
				return 0.8f;
			case Atmosphere.Thick:
				return 0.2f;
			case Atmosphere.Massive:
				return 0.01f;
			default:
				return 1f;
			}
		}

		// Token: 0x17000C72 RID: 3186
		// (get) Token: 0x06004271 RID: 17009 RVA: 0x001AD321 File Offset: 0x001AB521
		public double longestDimension_km
		{
			get
			{
				return Mathd.Max(new double[] { this.dimensionX_km, this.dimensionY_km, this.dimensionZ_km });
			}
		}

		// Token: 0x17000C73 RID: 3187
		// (get) Token: 0x06004272 RID: 17010 RVA: 0x001AD349 File Offset: 0x001AB549
		public double longestDimension_m
		{
			get
			{
				return this.longestDimension_km * 1000.0;
			}
		}

		// Token: 0x17000C74 RID: 3188
		// (get) Token: 0x06004273 RID: 17011 RVA: 0x001AD35C File Offset: 0x001AB55C
		public double dimensionX_km
		{
			get
			{
				if (this.template.dimensionX_km != null)
				{
					return this.template.dimensionX_km.Value;
				}
				if (this.template.equatorialRadius_km != null)
				{
					return this.template.equatorialRadius_km.Value * 2.0;
				}
				return this.meanRadius_km * 2.0;
			}
		}

		// Token: 0x17000C75 RID: 3189
		// (get) Token: 0x06004274 RID: 17012 RVA: 0x001AD3CC File Offset: 0x001AB5CC
		public double dimensionY_km
		{
			get
			{
				if (this.template.dimensionY_km != null)
				{
					return this.template.dimensionY_km.Value;
				}
				if (this.template.dimensionX_km != null)
				{
					return this.template.dimensionX_km.Value;
				}
				if (this.template.equatorialRadius_km != null)
				{
					return this.template.equatorialRadius_km.Value * 2.0;
				}
				return this.meanRadius_km * 2.0;
			}
		}

		// Token: 0x17000C76 RID: 3190
		// (get) Token: 0x06004275 RID: 17013 RVA: 0x001AD460 File Offset: 0x001AB660
		public double dimensionZ_km
		{
			get
			{
				if (this.template.dimensionZ_km != null)
				{
					return this.template.dimensionZ_km.Value;
				}
				if (this.template.dimensionY_km != null)
				{
					return this.template.dimensionY_km.Value;
				}
				if (this.template.dimensionX_km != null)
				{
					return this.template.dimensionX_km.Value;
				}
				if (this.template.equatorialRadius_km != null)
				{
					return this.template.equatorialRadius_km.Value * 2.0;
				}
				return this.meanRadius_km * 2.0;
			}
		}

		// Token: 0x17000C77 RID: 3191
		// (get) Token: 0x06004276 RID: 17014 RVA: 0x001AD518 File Offset: 0x001AB718
		public double density_gcm3
		{
			get
			{
				double? density_gcm = this.template.density_gcm3;
				if (density_gcm == null)
				{
					return 2.0;
				}
				return density_gcm.GetValueOrDefault();
			}
		}

		// Token: 0x17000C78 RID: 3192
		// (get) Token: 0x06004277 RID: 17015 RVA: 0x001AD54B File Offset: 0x001AB74B
		public double surfaceGravity_mps2
		{
			get
			{
				return base.mu / (this.meanRadius_m * this.meanRadius_m);
			}
		}

		// Token: 0x17000C79 RID: 3193
		// (get) Token: 0x06004278 RID: 17016 RVA: 0x001AD561 File Offset: 0x001AB761
		public double surfaceGravity_g
		{
			get
			{
				return this.surfaceGravity_mps2 / 9.80665;
			}
		}

		// Token: 0x17000C7A RID: 3194
		// (get) Token: 0x06004279 RID: 17017 RVA: 0x001AD574 File Offset: 0x001AB774
		public double stationaryOrbitRadius_m
		{
			get
			{
				double num = Mathd.Abs(Mathd.Pow(6.67384E-11 * this.mass_kg * Mathd.Pow(this.rotationperiod_s, 2.0) / (4.0 * Mathd.Pow(3.141592653589793, 2.0)), 0.33333333333));
				if (num > this.meanRadius_m * 1.5 && num < base.hillRadius_m)
				{
					return num;
				}
				return -1.0;
			}
		}

		// Token: 0x17000C7B RID: 3195
		// (get) Token: 0x0600427A RID: 17018 RVA: 0x001AD603 File Offset: 0x001AB803
		public float tilt_Deg
		{
			get
			{
				return this.template.tilt_Deg.GetValueOrDefault();
			}
		}

		// Token: 0x17000C7C RID: 3196
		// (get) Token: 0x0600427B RID: 17019 RVA: 0x001AD615 File Offset: 0x001AB815
		public float tiltSkew_Deg
		{
			get
			{
				return this.template.tiltSkew_Deg.GetValueOrDefault();
			}
		}

		// Token: 0x17000C7D RID: 3197
		// (get) Token: 0x0600427C RID: 17020 RVA: 0x001AD628 File Offset: 0x001AB828
		public float rotationOffset_Deg
		{
			get
			{
				if (this.template.rotationOffset_Deg != null)
				{
					return this.template.rotationOffset_Deg.Value;
				}
				if (this._rnd_rotationOffset_Deg != null)
				{
					return (float)this._rnd_rotationOffset_Deg.Value;
				}
				this._rnd_rotationOffset_Deg = new double?((double)TIUtilities.RandomRange(0f, 359.9999f));
				return (float)this._rnd_rotationOffset_Deg.Value;
			}
		}

		// Token: 0x0600427D RID: 17021 RVA: 0x001AD69C File Offset: 0x001AB89C
		public override double GetSurfaceRotation_Rad(TIDateTime time)
		{
			double num = (double)this.rotationOffset_Deg * 0.017453292519943295 / 6.283185307179586;
			return 6.283185307179586 - (num + (time - base.epoch_DateTime).TotalSeconds / this.rotationperiod_s) % 1.0 * 6.283185307179586;
		}

		// Token: 0x0600427E RID: 17022 RVA: 0x001AD700 File Offset: 0x001AB900
		public override double GetAngularDiameter(double distanceInMeters)
		{
			return Mathd.AngularDiameterOfSphere(this.meanRadius_m, distanceInMeters);
		}

		// Token: 0x0600427F RID: 17023 RVA: 0x001AD710 File Offset: 0x001AB910
		public void ChangeSolarMirrorBonus(int changeBy, TIFactionState faction)
		{
			if (changeBy != 0)
			{
				Dictionary<TIFactionState, int> dictionary = this.solarMirrorBonus;
				dictionary[faction] += changeBy;
				this.solarMirrorBonus[faction] = Mathf.Max(0, this.solarMirrorBonus[faction]);
				foreach (TIHabState tihabState in this.surfaceBases)
				{
					if (tihabState.faction == faction)
					{
						tihabState.UpdatePowerManagement(changeBy > 0, null, faction.player.isAI);
					}
				}
			}
		}

		// Token: 0x06004280 RID: 17024 RVA: 0x001AD7C0 File Offset: 0x001AB9C0
		public void ChangePlayerTag(PlayerTag newTag)
		{
			if (newTag != this.playerTag)
			{
				this.playerTag = newTag;
				GameControl.eventManager.TriggerEvent(new SpaceBodyTagChanged(this), null, new object[] { this });
			}
		}

		// Token: 0x06004281 RID: 17025 RVA: 0x001AD7F0 File Offset: 0x001AB9F0
		public string GetMiningPotentialString()
		{
			if (this.habSites.Length == 1)
			{
				return this.habSites[0].miningProfile.description;
			}
			if (base.isEarth)
			{
				return string.Empty;
			}
			if (this.habSites.Length == 0)
			{
				return Loc.T("UI.Space.NoHabs");
			}
			if (this.habSites.All<TIHabSiteState>((TIHabSiteState x) => x.miningProfile.dataName == this.habSites[0].miningProfile.dataName))
			{
				return this.habSites[0].miningProfile.description;
			}
			return this.template.descriptor2;
		}

		// Token: 0x17000C7E RID: 3198
		// (get) Token: 0x06004282 RID: 17026 RVA: 0x001AD874 File Offset: 0x001ABA74
		public float baseSortPosition
		{
			get
			{
				if (this.barycenter.objectType == SpaceObjectType.Star)
				{
					return (float)base.semiMajorAxis_AU;
				}
				return (float)this.barycenter.semiMajorAxis_AU + 0.001f;
			}
		}

		// Token: 0x17000C7F RID: 3199
		// (get) Token: 0x06004283 RID: 17027 RVA: 0x001AD8A0 File Offset: 0x001ABAA0
		public string getAsteroidResourceString
		{
			get
			{
				if (this.habSites.Length == 0)
				{
					Error.Log(this.template.displayName, Array.Empty<object>());
				}
				string text = this.habSites[0].miningProfile.modelValue.ToString("D2");
				if (text == "00")
				{
					Error.Log(this.template.displayName, Array.Empty<object>());
				}
				string text2;
				if (this.density_gcm3 <= 1.0)
				{
					text2 = "F";
				}
				else if (this.oblateness == 0.0)
				{
					text2 = "E";
				}
				else if (this.oblateness <= 0.1)
				{
					text2 = "A";
				}
				else if (this.oblateness <= 0.25)
				{
					text2 = "B";
				}
				else if (this.oblateness <= 0.4)
				{
					text2 = "D";
				}
				else
				{
					text2 = "C";
				}
				return string.Join("_", new string[] { "Asteroid", text2, text });
			}
		}

		// Token: 0x06004284 RID: 17028 RVA: 0x001AD9B0 File Offset: 0x001ABBB0
		public void SetModelResource()
		{
			string text = this.currentModelResource;
			if (string.IsNullOrEmpty(this.template.modelResource))
			{
				if (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.Comet)
				{
					this.currentModelResource = new StringBuilder("planets/").Append(this.getAsteroidResourceString).ToString();
				}
			}
			else
			{
				this.currentModelResource = this.template.ModelResource;
			}
			if (this.template.numAltModels > 0)
			{
				for (int i = 0; i < this.template.numAltModels; i++)
				{
					if (this.template.altModels[i].condition.PassesCondition(this))
					{
						this.currentModelResource = this.template.altModels[i].modelResource;
					}
				}
			}
			if (text != string.Empty && text != this.currentModelResource)
			{
				GameControl.eventManager.TriggerEvent(new ForceUpdateSpaceBodyModel(this), null, new object[] { this });
			}
		}

		// Token: 0x17000C80 RID: 3200
		// (get) Token: 0x06004285 RID: 17029 RVA: 0x001ADAB8 File Offset: 0x001ABCB8
		public override string iconResource
		{
			get
			{
				if (string.IsNullOrEmpty(this.template.symbolTexture) && (this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.AsteroidalMoon || this.objectType == SpaceObjectType.Comet))
				{
					return new StringBuilder("icons_2d/ICO_").Append(this.getAsteroidResourceString).ToString();
				}
				return base.iconResource;
			}
		}

		// Token: 0x06004286 RID: 17030 RVA: 0x001ADB14 File Offset: 0x001ABD14
		public SiteProfileRating GetSiteProfileRating(FactionResource resource, bool prospected)
		{
			SiteProfileRating siteProfileRating = SiteProfileRating.empty;
			if (prospected)
			{
				switch (resource)
				{
				case FactionResource.Water:
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.water_day), 0f, this.habSites.Min<TIHabSiteState>((TIHabSiteState x) => x.water_day), TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], true);
					break;
				case FactionResource.Volatiles:
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.volatiles_day), 0f, this.habSites.Min<TIHabSiteState>((TIHabSiteState x) => x.volatiles_day), TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], true);
					break;
				case FactionResource.Metals:
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.metals_day), 0f, this.habSites.Min<TIHabSiteState>((TIHabSiteState x) => x.metals_day), TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], true);
					break;
				case FactionResource.NobleMetals:
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.nobles_day), 0f, this.habSites.Min<TIHabSiteState>((TIHabSiteState x) => x.nobles_day), TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], true);
					break;
				case FactionResource.Fissiles:
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.fissiles_day), 0f, this.habSites.Min<TIHabSiteState>((TIHabSiteState x) => x.fissiles_day), TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], true);
					break;
				}
			}
			else
			{
				switch (resource)
				{
				case FactionResource.Water:
				{
					float num = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteExpectedProductivity_day(FactionResource.Water));
					float num2 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.miningProfile.water_width);
					float num3 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteMinProductivity_day(resource));
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(num, num2, num3, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], false);
					break;
				}
				case FactionResource.Volatiles:
				{
					float num4 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteExpectedProductivity_day(FactionResource.Volatiles));
					float num5 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.miningProfile.volatiles_width);
					float num6 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteMinProductivity_day(resource));
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(num4, num5, num6, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], false);
					break;
				}
				case FactionResource.Metals:
				{
					float num7 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteExpectedProductivity_day(FactionResource.Metals));
					float num8 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.miningProfile.metals_width);
					float num9 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteMinProductivity_day(resource));
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(num7, num8, num9, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], false);
					break;
				}
				case FactionResource.NobleMetals:
				{
					float num10 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteExpectedProductivity_day(FactionResource.NobleMetals));
					float num11 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.miningProfile.nobles_width);
					float num12 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteMinProductivity_day(resource));
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(num10, num11, num12, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], false);
					break;
				}
				case FactionResource.Fissiles:
				{
					float num13 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteExpectedProductivity_day(FactionResource.Fissiles));
					float num14 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.miningProfile.fissiles_width);
					float num15 = this.habSites.Average<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteMinProductivity_day(resource));
					siteProfileRating = TIMiningProfileTemplate.GetSiteProfileRating(num13, num14, num15, TIGlobalValuesState.GlobalValues.maxGlobalExpectedHabSiteProduction_day[resource], false);
					break;
				}
				}
			}
			return siteProfileRating;
		}

		// Token: 0x06004287 RID: 17031 RVA: 0x001AE0C8 File Offset: 0x001AC2C8
		public static string GetProfileRatingIconPath(SiteProfileRating rating, bool inline)
		{
			switch (rating)
			{
			case SiteProfileRating.empty:
				if (!inline)
				{
					return TemplateManager.global.pathResNoneIcon;
				}
				return TemplateManager.global.zeroResourcesInlineSpritePath;
			case SiteProfileRating.possible:
				if (!inline)
				{
					return TemplateManager.global.pathResPossibleIcon;
				}
				return TemplateManager.global.unknownResourcesInlineSpritePath;
			case SiteProfileRating.low:
				if (!inline)
				{
					return TemplateManager.global.pathResLowIcon;
				}
				return TemplateManager.global.level1ResourcesInlineSpritePath;
			case SiteProfileRating.medium:
				if (!inline)
				{
					return TemplateManager.global.pathResMedIcon;
				}
				return TemplateManager.global.level2ResourcesInlineSpritePath;
			case SiteProfileRating.high:
				if (!inline)
				{
					return TemplateManager.global.pathResHighIcon;
				}
				return TemplateManager.global.level3ResourcesInlineSpritePath;
			case SiteProfileRating.max:
				if (!inline)
				{
					return TemplateManager.global.pathResMaxIcon;
				}
				return TemplateManager.global.level4ResourcesInlineSpritePath;
			default:
				return string.Empty;
			}
		}

		// Token: 0x06004288 RID: 17032 RVA: 0x001AE193 File Offset: 0x001AC393
		public string GetProfileRatingIconPath(FactionResource resource, bool inline, bool prospected)
		{
			return TISpaceBodyState.GetProfileRatingIconPath(this.GetSiteProfileRating(resource, prospected), inline);
		}

		// Token: 0x06004289 RID: 17033 RVA: 0x001AE1A4 File Offset: 0x001AC3A4
		public string GetProfileRatingAllIconsString(bool prospected)
		{
			if (this.habSites.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(TemplateManager.global.waterInlineSpritePath).Append(this.GetProfileRatingIconPath(FactionResource.Water, true, prospected)).Append(TemplateManager.global.volatilesInlineSpritePath)
				.Append(this.GetProfileRatingIconPath(FactionResource.Volatiles, true, prospected))
				.Append(TemplateManager.global.metalsInlineSpritePath)
				.Append(this.GetProfileRatingIconPath(FactionResource.Metals, true, prospected))
				.Append(TemplateManager.global.noblesInlineSpritePath)
				.Append(this.GetProfileRatingIconPath(FactionResource.NobleMetals, true, prospected))
				.Append(TemplateManager.global.fissilesInlineSpritePath)
				.Append(this.GetProfileRatingIconPath(FactionResource.Fissiles, true, prospected));
			return stringBuilder.ToString();
		}

		// Token: 0x17000C81 RID: 3201
		// (get) Token: 0x0600428A RID: 17034 RVA: 0x001AE261 File Offset: 0x001AC461
		public List<TIHabSiteState> occupiedHabSites
		{
			get
			{
				return this.habSites.Where<TIHabSiteState>((TIHabSiteState x) => x.hasPlannedOrOperatingBase).ToList<TIHabSiteState>();
			}
		}

		// Token: 0x17000C82 RID: 3202
		// (get) Token: 0x0600428B RID: 17035 RVA: 0x001AE292 File Offset: 0x001AC492
		public List<TIHabSiteState> vacantHabSites
		{
			get
			{
				return this.habSites.Where<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase).ToList<TIHabSiteState>();
			}
		}

		// Token: 0x0600428C RID: 17036 RVA: 0x001AE2C4 File Offset: 0x001AC4C4
		public TIHabSiteState GetHabSiteAtLocation(Vector2 coordinates)
		{
			for (int i = 0; i < this.habSites.Length; i++)
			{
				TIHabSiteState tihabSiteState = this.habSites[i];
				if ((float)tihabSiteState.template.x == coordinates.x && (float)tihabSiteState.template.y == coordinates.y)
				{
					return tihabSiteState;
				}
			}
			return null;
		}

		// Token: 0x0600428D RID: 17037 RVA: 0x001AE318 File Offset: 0x001AC518
		public TIHabSiteState GetHabSiteByName(string habSiteName)
		{
			foreach (TIHabSiteState tihabSiteState in this.habSites)
			{
				if (tihabSiteState.templateName == habSiteName)
				{
					return tihabSiteState;
				}
			}
			return null;
		}

		// Token: 0x17000C83 RID: 3203
		// (get) Token: 0x0600428E RID: 17038 RVA: 0x001AE34F File Offset: 0x001AC54F
		public IEnumerable<TISpaceBodyState> AllNaturalSatellites
		{
			get
			{
				return this.naturalSatellites.SelectMany<TISpaceBodyState, TISpaceBodyState>((TISpaceBodyState x) => x.AllNaturalSatellites).Append(this);
			}
		}

		// Token: 0x17000C84 RID: 3204
		// (get) Token: 0x0600428F RID: 17039 RVA: 0x001AE381 File Offset: 0x001AC581
		public IEnumerable<TISpaceBodyState> SpaceBodiesInSystem
		{
			get
			{
				return base.ref_system.AllNaturalSatellites;
			}
		}

		// Token: 0x17000C85 RID: 3205
		// (get) Token: 0x06004290 RID: 17040 RVA: 0x001AE38E File Offset: 0x001AC58E
		public IEnumerable<TIOrbitState> OrbitsInSystem
		{
			get
			{
				return this.SpaceBodiesInSystem.SelectMany<TISpaceBodyState, TIOrbitState>((TISpaceBodyState x) => x.orbits);
			}
		}

		// Token: 0x17000C86 RID: 3206
		// (get) Token: 0x06004291 RID: 17041 RVA: 0x001AE3BA File Offset: 0x001AC5BA
		public bool orbitsStar
		{
			get
			{
				return this.barycenter.objectType == SpaceObjectType.Star;
			}
		}

		// Token: 0x17000C87 RID: 3207
		// (get) Token: 0x06004292 RID: 17042 RVA: 0x001AE3CA File Offset: 0x001AC5CA
		public bool canHaveMoons
		{
			get
			{
				return this.objectType == SpaceObjectType.Asteroid || this.objectType == SpaceObjectType.DwarfPlanet || this.objectType == SpaceObjectType.Planet;
			}
		}

		// Token: 0x17000C88 RID: 3208
		// (get) Token: 0x06004293 RID: 17043 RVA: 0x001AE3EC File Offset: 0x001AC5EC
		public List<TIHabState> surfaceBases
		{
			get
			{
				return (from x in this.habSites
					where x.hasPlannedOrOperatingBase
					select x.hab).ToList<TIHabState>();
			}
		}

		// Token: 0x17000C89 RID: 3209
		// (get) Token: 0x06004294 RID: 17044 RVA: 0x001AE44C File Offset: 0x001AC64C
		public bool hasAvailableHabSites
		{
			get
			{
				return this.habSites.Any<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase);
			}
		}

		// Token: 0x17000C8A RID: 3210
		// (get) Token: 0x06004295 RID: 17045 RVA: 0x001AE478 File Offset: 0x001AC678
		public List<TIOrbitState> interfaceOrbits
		{
			get
			{
				return this.orbits.Where<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit).ToList<TIOrbitState>();
			}
		}

		// Token: 0x17000C8B RID: 3211
		// (get) Token: 0x06004296 RID: 17046 RVA: 0x001AE4A9 File Offset: 0x001AC6A9
		public override List<TIHabState> habs
		{
			get
			{
				return base.stationsInOrbit.Union<TIHabState>(this.surfaceBases).ToList<TIHabState>();
			}
		}

		// Token: 0x17000C8C RID: 3212
		// (get) Token: 0x06004297 RID: 17047 RVA: 0x001AE4C1 File Offset: 0x001AC6C1
		public override List<TIHabState> habsInSystem
		{
			get
			{
				return this.habs.Union<TIHabState>(this.naturalSatellites.SelectMany<TISpaceBodyState, TIHabState>((TISpaceBodyState x) => x.habs)).ToList<TIHabState>();
			}
		}

		// Token: 0x17000C8D RID: 3213
		// (get) Token: 0x06004298 RID: 17048 RVA: 0x001AE4FD File Offset: 0x001AC6FD
		public List<TIHabSiteState> habSitesInSystem
		{
			get
			{
				return this.habSites.Union<TIHabSiteState>(this.naturalSatellites.SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites)).ToList<TIHabSiteState>();
			}
		}

		// Token: 0x17000C8E RID: 3214
		// (get) Token: 0x06004299 RID: 17049 RVA: 0x001AE539 File Offset: 0x001AC739
		public List<TISpaceFleetState> landedFleets
		{
			get
			{
				return this.habSites.SelectMany<TIHabSiteState, TISpaceFleetState>((TIHabSiteState x) => x.landedFleets).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000C8F RID: 3215
		// (get) Token: 0x0600429A RID: 17050 RVA: 0x001AE56A File Offset: 0x001AC76A
		public List<TISpaceFleetState> landedFleetsInSystem
		{
			get
			{
				return this.landedFleets.Union<TISpaceFleetState>(this.naturalSatellites.SelectMany<TISpaceBodyState, TISpaceFleetState>((TISpaceBodyState x) => x.landedFleets)).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000C90 RID: 3216
		// (get) Token: 0x0600429B RID: 17051 RVA: 0x001AE5A6 File Offset: 0x001AC7A6
		public List<TISpaceFleetState> fleetsInOrbitInSystem
		{
			get
			{
				return base.fleetsInOrbit.Union<TISpaceFleetState>(this.naturalSatellites.SelectMany<TISpaceBodyState, TISpaceFleetState>((TISpaceBodyState x) => x.fleetsInOrbit)).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000C91 RID: 3217
		// (get) Token: 0x0600429C RID: 17052 RVA: 0x001AE5E2 File Offset: 0x001AC7E2
		public override List<TISpaceFleetState> fleetsInSystem
		{
			get
			{
				return this.landedFleetsInSystem.Union<TISpaceFleetState>(this.fleetsInOrbitInSystem).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000C92 RID: 3218
		// (get) Token: 0x0600429D RID: 17053 RVA: 0x001AE5FC File Offset: 0x001AC7FC
		public List<TISpaceFleetState> fleetsInInterfaceOrbits
		{
			get
			{
				return (from x in this.interfaceOrbits.SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit)
					where !x.archived && !x.inTransfer && x.barycenter == this
					select x).ToList<TISpaceFleetState>();
			}
		}

		// Token: 0x17000C93 RID: 3219
		// (get) Token: 0x0600429E RID: 17054 RVA: 0x001AE64C File Offset: 0x001AC84C
		public List<TISpaceAssetState> assetsInInterfaceOrbits
		{
			get
			{
				return this.interfaceOrbits.SelectMany<TIOrbitState, TISpaceAssetState>((TIOrbitState x) => x.assetsInOrbit).Where<TISpaceAssetState>(delegate(TISpaceAssetState x)
				{
					if (!x.archived)
					{
						TISpaceFleetState ref_fleet = x.ref_fleet;
						if (ref_fleet == null || !ref_fleet.inTransfer)
						{
							return x.barycenter == this;
						}
					}
					return false;
				}).ToList<TISpaceAssetState>();
			}
		}

		// Token: 0x060042A0 RID: 17056 RVA: 0x001AE6B0 File Offset: 0x001AC8B0
		[CompilerGenerated]
		internal static bool <PostGameStateCreateInit_OnCreationOnly_1>g__SitesTooClose|81_0(TIHabSiteState site1, TIHabSiteState site2)
		{
			return Mathf.Abs(site1.latitude - site2.latitude) < 10f && Mathf.Abs(site1.longitude - site2.longitude) < 10f;
		}

		// Token: 0x040027C2 RID: 10178
		public List<TINationState> nations;

		// Token: 0x040027C4 RID: 10180
		public TIHabSiteState[] habSites;

		// Token: 0x040027C5 RID: 10181
		public string currentModelResource;

		// Token: 0x040027C6 RID: 10182
		public PlayerTag playerTag;

		// Token: 0x040027C7 RID: 10183
		[fsIgnore]
		public List<TISpaceBodyState> naturalSatellites;

		// Token: 0x040027C8 RID: 10184
		[fsIgnore]
		public List<TILagrangePointState> lagrangePoints;

		// Token: 0x040027C9 RID: 10185
		[fsIgnore]
		public bool alienTerritory;

		// Token: 0x040027CA RID: 10186
		private Vector3 north_pole_localized_coordinates_offset;

		// Token: 0x040027CB RID: 10187
		private double _semimajorAxis_m;

		// Token: 0x040027CC RID: 10188
		private double _ecc;

		// Token: 0x040027CD RID: 10189
		private double _inclination_Rad;

		// Token: 0x040027CE RID: 10190
		private double _longAscendingNode_Rad;

		// Token: 0x040027CF RID: 10191
		private double _argPeriapsis_Rad;

		// Token: 0x040027D0 RID: 10192
		private double _meanAnomalyAtEpoch_Rad;

		// Token: 0x040027D1 RID: 10193
		private double _rotationPeriod_Hours;

		// Token: 0x040027D2 RID: 10194
		private double _meanRadius_m;

		// Token: 0x040027D3 RID: 10195
		private double _meanRadius_km;

		// Token: 0x040027D4 RID: 10196
		private double _polarRadius_m;

		// Token: 0x040027D5 RID: 10197
		[fsIgnore]
		public float solarMultiplier;

		// Token: 0x040027D6 RID: 10198
		public Dictionary<TIFactionState, int> solarMirrorBonus;

		// Token: 0x040027D7 RID: 10199
		private double _escapeVelocityForMining_kps = -1.0;
	}
}
