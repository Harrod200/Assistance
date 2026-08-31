using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000777 RID: 1911
	public class TIRegionState : TIGameState
	{
		// Token: 0x17000A19 RID: 2585
		// (get) Token: 0x06003A36 RID: 14902 RVA: 0x00156FE1 File Offset: 0x001551E1
		public TIRegionTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIRegionTemplate>();
			}
		}

		// Token: 0x17000A1A RID: 2586
		// (get) Token: 0x06003A37 RID: 14903 RVA: 0x00156FE9 File Offset: 0x001551E9
		public string mapRegionTemplateName
		{
			get
			{
				return this.template.mapRegionName;
			}
		}

		// Token: 0x17000A1B RID: 2587
		// (get) Token: 0x06003A38 RID: 14904 RVA: 0x00156FF6 File Offset: 0x001551F6
		// (set) Token: 0x06003A39 RID: 14905 RVA: 0x00156FFE File Offset: 0x001551FE
		public TINationState nation { get; set; }

		// Token: 0x17000A1C RID: 2588
		// (get) Token: 0x06003A3A RID: 14906 RVA: 0x00157007 File Offset: 0x00155207
		// (set) Token: 0x06003A3B RID: 14907 RVA: 0x0015700F File Offset: 0x0015520F
		public TINationState leadOccupier { get; private set; }

		// Token: 0x17000A1D RID: 2589
		// (get) Token: 0x06003A3C RID: 14908 RVA: 0x00157018 File Offset: 0x00155218
		// (set) Token: 0x06003A3D RID: 14909 RVA: 0x00157020 File Offset: 0x00155220
		public Dictionary<TINationState, float> occupations { get; private set; }

		// Token: 0x17000A1E RID: 2590
		// (get) Token: 0x06003A3E RID: 14910 RVA: 0x00157029 File Offset: 0x00155229
		// (set) Token: 0x06003A3F RID: 14911 RVA: 0x00157031 File Offset: 0x00155231
		public float populationInMillions { get; private set; }

		// Token: 0x17000A1F RID: 2591
		// (get) Token: 0x06003A40 RID: 14912 RVA: 0x0015703A File Offset: 0x0015523A
		public bool coreResourceRegion
		{
			get
			{
				return this.resourceRegion || this.oilRegion;
			}
		}

		// Token: 0x17000A20 RID: 2592
		// (get) Token: 0x06003A41 RID: 14913 RVA: 0x0015704C File Offset: 0x0015524C
		// (set) Token: 0x06003A42 RID: 14914 RVA: 0x00157054 File Offset: 0x00155254
		public bool antiSpaceDefenses { get; private set; }

		// Token: 0x17000A21 RID: 2593
		// (get) Token: 0x06003A43 RID: 14915 RVA: 0x0015705D File Offset: 0x0015525D
		// (set) Token: 0x06003A44 RID: 14916 RVA: 0x00157065 File Offset: 0x00155265
		public bool underBombardment { get; private set; }

		// Token: 0x17000A22 RID: 2594
		// (get) Token: 0x06003A45 RID: 14917 RVA: 0x0015706E File Offset: 0x0015526E
		// (set) Token: 0x06003A46 RID: 14918 RVA: 0x00157076 File Offset: 0x00155276
		public bool isCounterfiring { get; private set; }

		// Token: 0x17000A23 RID: 2595
		// (get) Token: 0x06003A47 RID: 14919 RVA: 0x0015707F File Offset: 0x0015527F
		// (set) Token: 0x06003A48 RID: 14920 RVA: 0x00157087 File Offset: 0x00155287
		public TIRegionAlienFacilityState alienFacility { get; private set; }

		// Token: 0x17000A24 RID: 2596
		// (get) Token: 0x06003A49 RID: 14921 RVA: 0x00157090 File Offset: 0x00155290
		// (set) Token: 0x06003A4A RID: 14922 RVA: 0x00157098 File Offset: 0x00155298
		public TIRegionAlienActivityState alienActivity { get; private set; }

		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x06003A4B RID: 14923 RVA: 0x001570A1 File Offset: 0x001552A1
		// (set) Token: 0x06003A4C RID: 14924 RVA: 0x001570A9 File Offset: 0x001552A9
		public TIRegionUFOLandingState alienLanding { get; private set; }

		// Token: 0x17000A26 RID: 2598
		// (get) Token: 0x06003A4D RID: 14925 RVA: 0x001570B2 File Offset: 0x001552B2
		// (set) Token: 0x06003A4E RID: 14926 RVA: 0x001570BA File Offset: 0x001552BA
		public TIRegionUFOCrashdownState alienCrashdown { get; private set; }

		// Token: 0x17000A27 RID: 2599
		// (get) Token: 0x06003A4F RID: 14927 RVA: 0x001570C3 File Offset: 0x001552C3
		// (set) Token: 0x06003A50 RID: 14928 RVA: 0x001570CB File Offset: 0x001552CB
		public TIRegionXenoformingState xenoforming { get; private set; }

		// Token: 0x17000A28 RID: 2600
		// (get) Token: 0x06003A51 RID: 14929 RVA: 0x001570D4 File Offset: 0x001552D4
		// (set) Token: 0x06003A52 RID: 14930 RVA: 0x001570DC File Offset: 0x001552DC
		public float annualPopGrowthModifier { get; set; }

		// Token: 0x17000A29 RID: 2601
		// (get) Token: 0x06003A53 RID: 14931 RVA: 0x001570E5 File Offset: 0x001552E5
		public override bool isRegionState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A2A RID: 2602
		// (get) Token: 0x06003A54 RID: 14932 RVA: 0x001570E8 File Offset: 0x001552E8
		public override Searchable searchable
		{
			get
			{
				return Searchable.always;
			}
		}

		// Token: 0x17000A2B RID: 2603
		// (get) Token: 0x06003A55 RID: 14933 RVA: 0x001570EB File Offset: 0x001552EB
		public override TIRegionState ref_region
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000A2C RID: 2604
		// (get) Token: 0x06003A56 RID: 14934 RVA: 0x001570EE File Offset: 0x001552EE
		public override TINationState ref_nation
		{
			get
			{
				return this.nation;
			}
		}

		// Token: 0x17000A2D RID: 2605
		// (get) Token: 0x06003A57 RID: 14935 RVA: 0x001570F6 File Offset: 0x001552F6
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.spaceBody;
			}
		}

		// Token: 0x17000A2E RID: 2606
		// (get) Token: 0x06003A58 RID: 14936 RVA: 0x001570FE File Offset: 0x001552FE
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.spaceBody;
			}
		}

		// Token: 0x17000A2F RID: 2607
		// (get) Token: 0x06003A59 RID: 14937 RVA: 0x00157106 File Offset: 0x00155306
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this.spaceBody;
			}
		}

		// Token: 0x17000A30 RID: 2608
		// (get) Token: 0x06003A5A RID: 14938 RVA: 0x0015710E File Offset: 0x0015530E
		public override TIFactionState ref_faction
		{
			get
			{
				return this.nation.ref_faction;
			}
		}

		// Token: 0x17000A31 RID: 2609
		// (get) Token: 0x06003A5B RID: 14939 RVA: 0x0015711B File Offset: 0x0015531B
		public override List<TIFactionState> ref_factions
		{
			get
			{
				return this.nation.ref_factions;
			}
		}

		// Token: 0x17000A32 RID: 2610
		// (get) Token: 0x06003A5C RID: 14940 RVA: 0x00157128 File Offset: 0x00155328
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06003A5D RID: 14941 RVA: 0x0015712B File Offset: 0x0015532B
		public override bool hasEarthMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06003A5E RID: 14942 RVA: 0x0015712E File Offset: 0x0015532E
		public override TIRegionUFOLandingState ref_UFOLanding
		{
			get
			{
				return this.alienLanding;
			}
		}

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06003A5F RID: 14943 RVA: 0x00157136 File Offset: 0x00155336
		public override TIRegionUFOCrashdownState ref_UFOCrashdown
		{
			get
			{
				return this.alienCrashdown;
			}
		}

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06003A60 RID: 14944 RVA: 0x0015713E File Offset: 0x0015533E
		public override TIRegionAlienActivityState ref_regionAlienActivity
		{
			get
			{
				return this.alienActivity;
			}
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06003A61 RID: 14945 RVA: 0x00157146 File Offset: 0x00155346
		public override TIRegionAlienFacilityState ref_alienFacility
		{
			get
			{
				return this.alienFacility;
			}
		}

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06003A62 RID: 14946 RVA: 0x0015714E File Offset: 0x0015534E
		public override TIRegionXenoformingState ref_xenoforming
		{
			get
			{
				return this.xenoforming;
			}
		}

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06003A63 RID: 14947 RVA: 0x00157156 File Offset: 0x00155356
		public string solarBodyName
		{
			get
			{
				return this.mapRegionTemplate.solarBody ?? "Earth";
			}
		}

		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06003A64 RID: 14948 RVA: 0x0015716C File Offset: 0x0015536C
		public float boostLatitude
		{
			get
			{
				return this.mapRegionTemplate.boostLatitude;
			}
		}

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06003A65 RID: 14949 RVA: 0x00157179 File Offset: 0x00155379
		public float latitude
		{
			get
			{
				return this.mapRegionTemplate.latitude;
			}
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06003A66 RID: 14950 RVA: 0x00157186 File Offset: 0x00155386
		public float longitude
		{
			get
			{
				return this.mapRegionTemplate.longitude;
			}
		}

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06003A67 RID: 14951 RVA: 0x00157193 File Offset: 0x00155393
		public TerrainType terrain
		{
			get
			{
				return this.mapRegionTemplate.terrain;
			}
		}

		// Token: 0x06003A68 RID: 14952 RVA: 0x001571A0 File Offset: 0x001553A0
		public Vector3d GetLocalPosition(TIDateTime time)
		{
			Quaterniond spatialRotation = this.spaceBody.SpatialRotation;
			Quaternion quaternion = Quaternion.AngleAxis((float)this.spaceBody.GetSurfaceRotation_Rad(time) * 57.29578f, Vector3.up);
			Vector3 vector = (Quaternion)spatialRotation * quaternion * this.localized_coordinates_offset;
			return new Vector3d(vector.x, vector.z, vector.y);
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06003A69 RID: 14953 RVA: 0x00157204 File Offset: 0x00155404
		public Vector3d StandardLocalPosition
		{
			get
			{
				Vector3d vector3d = default(Vector3d);
				if ((in this.standardLocalPosition) == (in vector3d))
				{
					vector3d = (this.standardLocalPosition = this.GetLocalPosition(new TIDateTime(DateTime.MinValue)));
					return vector3d;
				}
				return this.standardLocalPosition;
			}
		}

		// Token: 0x06003A6A RID: 14954 RVA: 0x00157248 File Offset: 0x00155448
		public float GetDistanceEstimate_km(TIRegionState other)
		{
			Vector3d vector3d = this.StandardLocalPosition;
			Vector3d vector3d2 = other.StandardLocalPosition;
			return (float)Vector3d.Distance(in vector3d, in vector3d2) / 1000f;
		}

		// Token: 0x06003A6B RID: 14955 RVA: 0x00157273 File Offset: 0x00155473
		public Vector3d GetGlobalPosition(TIDateTime time)
		{
			return this.spaceBody.GetGlobalPositionAtTime(time) + this.GetLocalPosition(time);
		}

		// Token: 0x06003A6C RID: 14956 RVA: 0x00157290 File Offset: 0x00155490
		public override void InitWithTemplate(TIDataTemplate template)
		{
			if (!this.gameStateSubjectCreated)
			{
				base.InitWithTemplate(template);
				TIRegionTemplate tiregionTemplate = template as TIRegionTemplate;
				if (tiregionTemplate == null)
				{
					return;
				}
				this.templateName = tiregionTemplate.dataName;
				this.displayName = tiregionTemplate.displayName;
				this.boostPerYear_dekatons = tiregionTemplate.baseBoostPerYear_dekatons;
				this.missionControl = tiregionTemplate.missionControl.GetValueOrDefault();
				this.annualPopGrowthModifier = tiregionTemplate.annualPopGrowthModifier;
				this.populationInMillions = Mathf.Max(tiregionTemplate.population_Millions, 0.001f);
				this.adjacencies = new Dictionary<TIRegionState, TerrestrialAdjacencyType>();
				this.occupations = new Dictionary<TINationState, float>();
				this.coreEconomicRegion = tiregionTemplate.coreEco;
				this.resourceRegion = tiregionTemplate.mining;
				this.oilRegion = tiregionTemplate.oilResource;
				this.armies = new List<TIArmyState>();
				this.boostFacility = GameStateManager.CreateNewGameState<TILaunchFacilityState>();
				this.boostFacility.InitWithRegionState(SpaceFacilityType.launchFacility, this);
				this.missionControlFacility = GameStateManager.CreateNewGameState<TIMissionControlFacilityState>();
				this.missionControlFacility.InitWithRegionState(SpaceFacilityType.missionControlFacility, this);
				this.spaceDefenseFacility = GameStateManager.CreateNewGameState<TISpaceDefensesFacilityState>();
				this.spaceDefenseFacility.InitWithRegionState(SpaceFacilityType.spaceDefenseFacility, this);
				if (TemplateManager.global.debug_advancedFactionStart && (template.dataName == "Wuhan" || template.dataName == "Texas" || template.dataName == "Crimea"))
				{
					this.antiSpaceDefenses = true;
				}
				this.spaceFacilities = new List<TIRegionSpaceFacilityState> { this.boostFacility, this.missionControlFacility, this.spaceDefenseFacility };
				TIRegionAlienFacilityState tiregionAlienFacilityState = GameStateManager.CreateNewGameState<TIRegionAlienFacilityState>();
				tiregionAlienFacilityState.InitWithRegionState(this);
				this.alienFacility = tiregionAlienFacilityState;
				TIRegionAlienActivityState tiregionAlienActivityState = GameStateManager.CreateNewGameState<TIRegionAlienActivityState>();
				tiregionAlienActivityState.InitWithRegionState(this);
				this.alienActivity = tiregionAlienActivityState;
				TIRegionUFOLandingState tiregionUFOLandingState = GameStateManager.CreateNewGameState<TIRegionUFOLandingState>();
				tiregionUFOLandingState.InitWithRegionState(this);
				this.alienLanding = tiregionUFOLandingState;
				TIRegionUFOCrashdownState tiregionUFOCrashdownState = GameStateManager.CreateNewGameState<TIRegionUFOCrashdownState>();
				tiregionUFOCrashdownState.InitWithRegionState(this);
				this.alienCrashdown = tiregionUFOCrashdownState;
				TIRegionXenoformingState tiregionXenoformingState = GameStateManager.CreateNewGameState<TIRegionXenoformingState>();
				tiregionXenoformingState.InitWithRegionState(this);
				this.xenoforming = tiregionXenoformingState;
				this.oceanType = tiregionTemplate.worldOcean;
				this.nuclearDetonations = tiregionTemplate.nuclearDetonations.GetValueOrDefault();
			}
		}

		// Token: 0x06003A6D RID: 14957 RVA: 0x001574A0 File Offset: 0x001556A0
		public void InitializePostCampaignCreation()
		{
			TIRegionState tiregionState = GameStateManager.MapRegionLookup(this.mapRegionTemplate.parent);
			tiregionState.SetDisplayName(tiregionState.template.displayName);
			this.nation = tiregionState.nation;
			tiregionState.nation.regions.Add(this);
			foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
			{
				if (tibilateralTemplate.BilateralIsActive() && tibilateralTemplate.regionState1 == this && tibilateralTemplate.relationType == BilateralRelationType.Claim)
				{
					tibilateralTemplate.nationState1.SetClaim(this, tibilateralTemplate.hostileClaim, false);
					if (tibilateralTemplate.initialOwner)
					{
						this.colonyRegion = tibilateralTemplate.initialColony;
					}
				}
			}
			float num = tiregionState.template.population_Millions + this.template.population_Millions;
			float num2 = tiregionState.populationInMillions / num;
			float num3 = this.template.population_Millions * num2;
			tiregionState.ChangePopulation_Millions(-num3, false);
			if (tiregionState.isCoastal && tiregionState.mapRegionTemplate.coast == CoastRegion.none)
			{
				tiregionState.ChangeOceanType(WorldOceanType.No);
			}
			this.ChangePopulation_Millions(num3 - this.populationInMillions, false);
			this.adjacencies.Clear();
			this.SetAdjacencies();
			foreach (TIRegionState tiregionState2 in this.adjacencies.Keys)
			{
				tiregionState2.adjacencies.Clear();
				tiregionState2.SetAdjacencies();
			}
		}

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06003A6E RID: 14958 RVA: 0x0015763C File Offset: 0x0015583C
		public TIMapRegionTemplate mapRegionTemplate
		{
			get
			{
				if (this._mapRegionTemplate == null)
				{
					this._mapRegionTemplate = TemplateManager.Find<TIMapRegionTemplate>(this.mapRegionTemplateName, false);
					if (this._mapRegionTemplate == null)
					{
						Log.Error("Bad " + this.mapRegionTemplateName + " for " + this.templateName, Array.Empty<object>());
					}
				}
				return this._mapRegionTemplate;
			}
		}

		// Token: 0x06003A6F RID: 14959 RVA: 0x00157698 File Offset: 0x00155898
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (!this.gameStateSubjectCreated)
			{
				this.SetAdjacencies();
			}
			else if (this.isCoastal && this.mapRegionTemplate.coast == CoastRegion.none)
			{
				this.ChangeOceanType(WorldOceanType.No);
			}
			if (this.STOFighterCooldownExpiry == null)
			{
				this.STOFighterCooldownExpiry = new List<TIDateTime>();
			}
			if (this.oceanType == WorldOceanType.None)
			{
				this.oceanType = this.template.worldOcean;
			}
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.NationLaunchedNuclearAttackArrival), this.NuclearDetonationEventName, null, true, false);
			this.localized_coordinates_offset = Quaternion.AngleAxis(this.longitude, -Vector3.up) * Quaternion.AngleAxis(this.latitude, -Vector3.right) * Vector3.forward * (float)this.spaceBody.meanRadius_m;
			bool isCounterfiring = this.isCounterfiring;
			foreach (TIArmyState tiarmyState in this.armies.ToList<TIArmyState>())
			{
				if (tiarmyState.deleted)
				{
					this.armies.Remove(tiarmyState);
					Log.Error("Removing deleted army " + tiarmyState.ID.ToString() + " from " + this.displayName, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06003A70 RID: 14960 RVA: 0x00157810 File Offset: 0x00155A10
		public override void PostCanvasManagerCreateInit_3()
		{
			if (!this.gameStateSubjectCreated)
			{
				if (this.template.occupyingNation != string.Empty && this.template.occupationValue > 0f)
				{
					TINationState tinationState = GameStateManager.FindByTemplate<TINationState>(this.template.occupyingNation, false);
					if (this.nation.wars.Contains(tinationState))
					{
						this.SetOccupationValue(tinationState, this.template.occupationValue, null);
					}
				}
				if (this.colonyRegion && this.originalColony == null)
				{
					foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
					{
						if (tibilateralTemplate.regionState1 == this && tibilateralTemplate.initialColony)
						{
							this.originalColony = tibilateralTemplate.nationState1;
							break;
						}
					}
				}
			}
		}

		// Token: 0x06003A71 RID: 14961 RVA: 0x001578FC File Offset: 0x00155AFC
		public override void PostAllStartUpInit_5()
		{
			if (this.gameStateSubjectCreated)
			{
				IEnumerable<TIRegionState> enumerable = this.Neighbors;
			}
			this.regionSizeFactor = Mathf.Pow(this.mapRegionTemplate.area_km2, 0.5f) / Mathf.Pow(TIGlobalValuesState.GlobalValues.medianRegionArea_km2, 0.5f);
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06003A72 RID: 14962 RVA: 0x0015794F File Offset: 0x00155B4F
		public TISpaceBodyState spaceBody
		{
			get
			{
				if (this._spaceBody == null)
				{
					this._spaceBody = GameStateManager.FindByTemplate<TISpaceBodyState>(this.solarBodyName, false);
				}
				return this._spaceBody;
			}
		}

		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06003A73 RID: 14963 RVA: 0x00157977 File Offset: 0x00155B77
		public float boostPerMonth_dekatons
		{
			get
			{
				return this.boostPerYear_dekatons / 12f;
			}
		}

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06003A74 RID: 14964 RVA: 0x00157985 File Offset: 0x00155B85
		public bool hasAnySpaceFacility
		{
			get
			{
				return this.boostPerYear_dekatons > 0f || this.missionControl > 0 || this.antiSpaceDefenses;
			}
		}

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06003A75 RID: 14965 RVA: 0x001579A5 File Offset: 0x00155BA5
		public bool hasAlienFacility
		{
			get
			{
				return this.alienFacility.built;
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06003A76 RID: 14966 RVA: 0x001579B2 File Offset: 0x00155BB2
		public float area_km2
		{
			get
			{
				return this.mapRegionTemplate.area_km2;
			}
		}

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06003A77 RID: 14967 RVA: 0x001579C0 File Offset: 0x00155BC0
		public bool coastCurrentlyFrozen
		{
			get
			{
				return this.oceanType == WorldOceanType.Seasonal && ((this.latitude >= 0f && (this.gameTime.currentTime.month <= 3 || this.gameTime.currentTime.month > 9)) || (this.latitude < 0f && (this.gameTime.currentTime.month > 3 || this.gameTime.currentTime.month <= 9)));
			}
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06003A78 RID: 14968 RVA: 0x00157A49 File Offset: 0x00155C49
		public bool isCoastal
		{
			get
			{
				return this.oceanType == WorldOceanType.Yes || this.oceanType == WorldOceanType.Seasonal;
			}
		}

		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06003A79 RID: 14969 RVA: 0x00157A60 File Offset: 0x00155C60
		public bool onTheWater
		{
			get
			{
				return this.oceanType == WorldOceanType.Yes || (this.oceanType == WorldOceanType.Seasonal && ((this.latitude >= 0f && this.gameTime.currentTime.month > 3 && this.gameTime.currentTime.month <= 9) || (this.latitude < 0f && this.gameTime.currentTime.month <= 3) || this.gameTime.currentTime.month > 9));
			}
		}

		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06003A7A RID: 14970 RVA: 0x00157AED File Offset: 0x00155CED
		public bool isIsland
		{
			get
			{
				return this.isCoastal && this.AdjacentRegions(true).Count<TIRegionState>() == 0;
			}
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06003A7B RID: 14971 RVA: 0x00157B08 File Offset: 0x00155D08
		public TIRegionAlienAssetState[] alienAssets
		{
			get
			{
				return new TIRegionAlienAssetState[] { this.alienFacility, this.alienLanding, this.xenoforming };
			}
		}

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06003A7C RID: 14972 RVA: 0x00157B2B File Offset: 0x00155D2B
		public TIRegionAlienEntityState[] alienActivities
		{
			get
			{
				return new TIRegionAlienEntityState[] { this.alienActivity, this.alienCrashdown };
			}
		}

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06003A7D RID: 14973 RVA: 0x00157B45 File Offset: 0x00155D45
		public List<string> illustrationPaths
		{
			get
			{
				return this.template.illustrationPaths;
			}
		}

		// Token: 0x06003A7E RID: 14974 RVA: 0x00157B54 File Offset: 0x00155D54
		public float NationalGDPProportion()
		{
			float num = 0f;
			float num2 = 0f;
			foreach (TIRegionState tiregionState in this.nation.regions)
			{
				float num3 = tiregionState.populationInMillions;
				if (tiregionState.coreEconomicRegion)
				{
					num3 *= TemplateManager.global.coreEcoRegionGDPModifier;
				}
				if (tiregionState.coreResourceRegion)
				{
					num3 *= TemplateManager.global.coreResourceRegionGDPModifier;
				}
				if (tiregionState.colonyRegion)
				{
					num3 *= TemplateManager.global.colonyRegionGDPModifier;
				}
				if (tiregionState == this)
				{
					num2 = num3;
				}
				num += num3;
			}
			return num2 / num;
		}

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x06003A7F RID: 14975 RVA: 0x00157C08 File Offset: 0x00155E08
		public double nationalGDPShareValue
		{
			get
			{
				return this.nation.GDP * (double)this.NationalGDPProportion();
			}
		}

		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06003A80 RID: 14976 RVA: 0x00157C1D File Offset: 0x00155E1D
		public double nationalGDPShareValue_bn
		{
			get
			{
				return this.nation.GDP * (double)this.NationalGDPProportion() / 1000000000.0;
			}
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06003A81 RID: 14977 RVA: 0x00157C3C File Offset: 0x00155E3C
		public double regionalPerCapitaGDP
		{
			get
			{
				return this.nationalGDPShareValue / (double)this.population;
			}
		}

		// Token: 0x06003A82 RID: 14978 RVA: 0x00157C4C File Offset: 0x00155E4C
		public float GlobalGDPProportion()
		{
			return (float)this.nationalGDPShareValue / (float)TIGlobalValuesState.globalGDP;
		}

		// Token: 0x17000A4F RID: 2639
		// (get) Token: 0x06003A83 RID: 14979 RVA: 0x00157C5C File Offset: 0x00155E5C
		public string perCapitaGDPstr
		{
			get
			{
				return Loc.T("UI.Global.DollarValue", new object[] { this.regionalPerCapitaGDP.ToString("N0") });
			}
		}

		// Token: 0x17000A50 RID: 2640
		// (get) Token: 0x06003A84 RID: 14980 RVA: 0x00157C90 File Offset: 0x00155E90
		public string GDPstring
		{
			get
			{
				return Loc.T("UI.Global.DollarValue", new object[] { Loc.T("UI.Nation.AbbrBn", new object[] { this.nationalGDPShareValue_bn.ToString("N0") }) });
			}
		}

		// Token: 0x06003A85 RID: 14981 RVA: 0x00157CD8 File Offset: 0x00155ED8
		public static Dictionary<TIRegionState, float> GlobalGDPProportions()
		{
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			double globalGDP = TIGlobalValuesState.globalGDP;
			foreach (TIRegionState tiregionState in from x in GameStateManager.AllRegions()
				where x.nation != null
				select x)
			{
				dictionary.Add(tiregionState, (float)(tiregionState.nationalGDPShareValue / globalGDP));
			}
			return dictionary;
		}

		// Token: 0x17000A51 RID: 2641
		// (get) Token: 0x06003A86 RID: 14982 RVA: 0x00157D60 File Offset: 0x00155F60
		public string displayNameSentIn
		{
			get
			{
				return this.template.displayNameSentIn;
			}
		}

		// Token: 0x17000A52 RID: 2642
		// (get) Token: 0x06003A87 RID: 14983 RVA: 0x00157D6D File Offset: 0x00155F6D
		public string displayNameSentOf
		{
			get
			{
				return this.template.displayNameSentOf;
			}
		}

		// Token: 0x06003A88 RID: 14984 RVA: 0x00157D7C File Offset: 0x00155F7C
		public string IconString(TIFactionState faction)
		{
			StringBuilder stringBuilder = new StringBuilder(8);
			if (this.nation.capital == this)
			{
				stringBuilder.Append(TemplateManager.global.capitalRegionInlineSpritePath);
			}
			if (this.coreEconomicRegion)
			{
				stringBuilder.Append(TemplateManager.global.coreEconomicRegionInlineSpritePath);
			}
			if (this.resourceRegion)
			{
				stringBuilder.Append(TemplateManager.global.miningRegionInlineSpritePath);
			}
			if (this.oilRegion)
			{
				stringBuilder.Append(TemplateManager.global.coreOilRegionInlineSpritePath);
			}
			if (this.colonyRegion)
			{
				stringBuilder.Append(TemplateManager.global.colonyRegionInlineSpritePath);
			}
			if (this.template.environment == EnvironmentType.Vulnerable)
			{
				stringBuilder.Append(TemplateManager.global.ecologicallyVulnerableRegionInlineSpritePath);
			}
			if (this.template.environment == EnvironmentType.Beneficiary)
			{
				stringBuilder.Append(TemplateManager.global.ecologicallySafeRegionInlineSpritePath);
			}
			if (this.mapRegionTemplate.terrain == TerrainType.Rugged)
			{
				stringBuilder.Append(TemplateManager.global.ruggedRegionInlineSpritePath);
			}
			if (this.nuclearDetonations > 0)
			{
				stringBuilder.Append(TemplateManager.global.nukedRegionInlineSpritePath);
			}
			if (this.antiSpaceDefenses)
			{
				stringBuilder.Append(TemplateManager.global.antiSpaceDefensesInlineSpritePath);
			}
			if (this.hostileRegion)
			{
				stringBuilder.Append(TemplateManager.global.unrestInlineSpritePath);
			}
			if (faction.KnownAlienEntities.Any<TIRegionAlienEntityState>((TIRegionAlienEntityState x) => x.region == this))
			{
				stringBuilder.Append(TemplateManager.global.alienEntityInlineSpritePath);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x17000A53 RID: 2643
		// (get) Token: 0x06003A89 RID: 14985 RVA: 0x00157EF1 File Offset: 0x001560F1
		public bool isCapital
		{
			get
			{
				return this.nation.capital == this;
			}
		}

		// Token: 0x17000A54 RID: 2644
		// (get) Token: 0x06003A8A RID: 14986 RVA: 0x00157F04 File Offset: 0x00156104
		public bool hostileRegion
		{
			get
			{
				return this.nation.hostileClaims.Contains(this);
			}
		}

		// Token: 0x17000A55 RID: 2645
		// (get) Token: 0x06003A8B RID: 14987 RVA: 0x00157F17 File Offset: 0x00156117
		public RegionController Controller
		{
			get
			{
				return this.ref_spaceObject.controller.mapController.GetRegionController(this);
			}
		}

		// Token: 0x06003A8C RID: 14988 RVA: 0x00157F30 File Offset: 0x00156130
		public float DistanceToRegion_km(TIRegionState region)
		{
			if (region == this)
			{
				return 0f;
			}
			if (!this._distanceToRegion.ContainsKey(region))
			{
				if (region._distanceToRegion.ContainsKey(this))
				{
					return region._distanceToRegion[this];
				}
				float num = 0.017453292f * (region.longitude - this.longitude);
				float num2 = Mathf.Pow(Mathf.Sin(0.017453292f * (region.latitude - this.latitude) / 2f), 2f) + Mathf.Cos(0.017453292f * this.latitude) * Mathf.Cos(0.017453292f * region.latitude) * Mathf.Pow(Mathf.Sin(num / 2f), 2f);
				float num3 = 2f * Mathf.Atan2(Mathf.Sqrt(num2), Mathf.Sqrt(1f - num2));
				float num4 = (float)this.spaceBody.meanRadius_km * num3;
				this._distanceToRegion.Add(region, num4);
			}
			return this._distanceToRegion[region];
		}

		// Token: 0x06003A8D RID: 14989 RVA: 0x00158038 File Offset: 0x00156238
		public static float DistanceBetweenTwoCoordinates_km(float lat1, float long1, float lat2, float long2, double planetRadius_km)
		{
			float num = 0.017453292f * (long2 - long1);
			float num2 = Mathf.Pow(Mathf.Sin(0.017453292f * (lat2 - lat1) / 2f), 2f) + Mathf.Cos(0.017453292f * lat1) * Mathf.Cos(0.017453292f * lat2) * Mathf.Pow(Mathf.Sin(num / 2f), 2f);
			float num3 = 2f * Mathf.Atan2(Mathf.Sqrt(num2), Mathf.Sqrt(1f - num2));
			return (float)planetRadius_km * num3;
		}

		// Token: 0x06003A8E RID: 14990 RVA: 0x001580C4 File Offset: 0x001562C4
		public IEnumerable<WaterBody> GetBorderingWaterBodies()
		{
			IEnumerable<WaterBody> enumerable = Enumerable.Empty<WaterBody>();
			if (this.onTheWater)
			{
				CoastRegion coast = this.mapRegionTemplate.coast;
				if (coast != CoastRegion.none)
				{
					if (coast != CoastRegion.BlackSea)
					{
						if (coast != CoastRegion.BlackMed)
						{
							enumerable = enumerable.Append(WaterBody.Ocean);
						}
						else
						{
							enumerable = enumerable.Append(WaterBody.BlackSea).Append(WaterBody.Ocean);
						}
					}
					else
					{
						enumerable = enumerable.Append(WaterBody.BlackSea);
					}
				}
			}
			return enumerable;
		}

		// Token: 0x06003A8F RID: 14991 RVA: 0x0015811C File Offset: 0x0015631C
		public IEnumerable<WaterBody> GetAccessibleWaterBodies(TINationState askingNation)
		{
			IEnumerable<WaterBody> enumerable = this.GetBorderingWaterBodies();
			bool flag = TIRegionState.TurkishStraitAccess(askingNation);
			if (enumerable.Contains(WaterBody.BlackSea) && flag)
			{
				enumerable = enumerable.Append(WaterBody.Ocean);
			}
			if (enumerable.Contains(WaterBody.Ocean) && flag)
			{
				enumerable = enumerable.Append(WaterBody.BlackSea);
			}
			return enumerable.Distinct<WaterBody>();
		}

		// Token: 0x06003A90 RID: 14992 RVA: 0x00158164 File Offset: 0x00156364
		public bool CanalRegion()
		{
			return this.mapRegionTemplateName == TIGlobalConfig.globalConfig.SuezCanalRegion || this.mapRegionTemplateName == TIGlobalConfig.globalConfig.PanamaCanalRegion || this.mapRegionTemplateName == TIGlobalConfig.globalConfig.TurkishStraitsRegion;
		}

		// Token: 0x06003A91 RID: 14993 RVA: 0x001581B6 File Offset: 0x001563B6
		public static bool SuezAccess(TINationState askingNation)
		{
			List<TINationState> wars = askingNation.wars;
			TIRegionState suezRegion = TIGlobalValuesState.GlobalValues.SuezRegion;
			if (wars.Contains((suezRegion != null) ? suezRegion.nation : null))
			{
				TIRegionState suezRegion2 = TIGlobalValuesState.GlobalValues.SuezRegion;
				return suezRegion2 == null || suezRegion2.IsFullyOccupied();
			}
			return true;
		}

		// Token: 0x06003A92 RID: 14994 RVA: 0x001581F2 File Offset: 0x001563F2
		public static bool PanamaAccess(TINationState askingNation)
		{
			List<TINationState> wars = askingNation.wars;
			TIRegionState panamaRegion = TIGlobalValuesState.GlobalValues.PanamaRegion;
			if (wars.Contains((panamaRegion != null) ? panamaRegion.nation : null))
			{
				TIRegionState panamaRegion2 = TIGlobalValuesState.GlobalValues.PanamaRegion;
				return panamaRegion2 == null || panamaRegion2.IsFullyOccupied();
			}
			return true;
		}

		// Token: 0x06003A93 RID: 14995 RVA: 0x0015822E File Offset: 0x0015642E
		public static bool TurkishStraitAccess(TINationState askingNation)
		{
			List<TINationState> wars = askingNation.wars;
			TIRegionState turkishStraitRegion = TIGlobalValuesState.GlobalValues.TurkishStraitRegion;
			if (wars.Contains((turkishStraitRegion != null) ? turkishStraitRegion.nation : null))
			{
				TIRegionState turkishStraitRegion2 = TIGlobalValuesState.GlobalValues.TurkishStraitRegion;
				return turkishStraitRegion2 == null || turkishStraitRegion2.IsFullyOccupied();
			}
			return true;
		}

		// Token: 0x06003A94 RID: 14996 RVA: 0x0015826C File Offset: 0x0015646C
		public static float SeaTravelMultiplier(TINationState movingNation, TIRegionState region1, TIRegionState region2)
		{
			if (region1.IsAdjacent(region2, false))
			{
				return 1f;
			}
			bool flag = movingNation == null || TIRegionState.SuezAccess(movingNation);
			bool flag2 = movingNation == null || TIRegionState.PanamaAccess(movingNation);
			bool flag3 = false;
			bool flag4 = false;
			CoastRegion coastRegion = CoastRegion.none;
			CoastRegion coastRegion2 = CoastRegion.none;
			CoastRegion coastRegion3 = CoastRegion.none;
			CoastRegion coastRegion4 = CoastRegion.none;
			switch (region1.mapRegionTemplate.coast)
			{
			case CoastRegion.IndianMed:
				coastRegion = CoastRegion.Indian;
				coastRegion2 = CoastRegion.Mediterranean;
				flag3 = true;
				break;
			case CoastRegion.PacificCarib:
				coastRegion = CoastRegion.NortheastPacific;
				coastRegion2 = CoastRegion.Caribbean;
				flag3 = true;
				break;
			case CoastRegion.MedNorthAtlantic:
				coastRegion = CoastRegion.Mediterranean;
				coastRegion2 = CoastRegion.NortheastAtlantic;
				flag3 = true;
				break;
			case CoastRegion.BlackMed:
				coastRegion = CoastRegion.BlackSea;
				coastRegion2 = CoastRegion.Mediterranean;
				flag3 = true;
				break;
			}
			switch (region2.mapRegionTemplate.coast)
			{
			case CoastRegion.IndianMed:
				coastRegion3 = CoastRegion.Indian;
				coastRegion4 = CoastRegion.Mediterranean;
				flag4 = true;
				break;
			case CoastRegion.PacificCarib:
				coastRegion3 = CoastRegion.NortheastPacific;
				coastRegion4 = CoastRegion.Caribbean;
				flag4 = true;
				break;
			case CoastRegion.MedNorthAtlantic:
				coastRegion3 = CoastRegion.Mediterranean;
				coastRegion4 = CoastRegion.NortheastAtlantic;
				flag4 = true;
				break;
			case CoastRegion.BlackMed:
				coastRegion3 = CoastRegion.BlackSea;
				coastRegion4 = CoastRegion.Mediterranean;
				flag4 = true;
				break;
			}
			if (flag3)
			{
				if (flag4)
				{
					float seaTravelMultiplier = TIMapRegionTemplate.GetSeaTravelMultiplier(coastRegion, coastRegion3, flag, flag2, false);
					float seaTravelMultiplier2 = TIMapRegionTemplate.GetSeaTravelMultiplier(coastRegion2, coastRegion3, flag, flag2, false);
					float seaTravelMultiplier3 = TIMapRegionTemplate.GetSeaTravelMultiplier(coastRegion, coastRegion4, flag, flag2, false);
					float seaTravelMultiplier4 = TIMapRegionTemplate.GetSeaTravelMultiplier(coastRegion2, coastRegion4, flag, flag2, false);
					return Mathf.Min(new float[] { seaTravelMultiplier, seaTravelMultiplier2, seaTravelMultiplier3, seaTravelMultiplier4 });
				}
				float seaTravelMultiplier5 = TIMapRegionTemplate.GetSeaTravelMultiplier(coastRegion, region2.mapRegionTemplate.coast, flag, flag2, false);
				float seaTravelMultiplier6 = TIMapRegionTemplate.GetSeaTravelMultiplier(coastRegion2, region2.mapRegionTemplate.coast, flag, flag2, false);
				return Mathf.Min(seaTravelMultiplier5, seaTravelMultiplier6);
			}
			else
			{
				if (flag4)
				{
					float seaTravelMultiplier7 = TIMapRegionTemplate.GetSeaTravelMultiplier(region1.mapRegionTemplate.coast, coastRegion3, flag, flag2, false);
					float seaTravelMultiplier8 = TIMapRegionTemplate.GetSeaTravelMultiplier(region1.mapRegionTemplate.coast, coastRegion4, flag, flag2, false);
					return Mathf.Min(seaTravelMultiplier7, seaTravelMultiplier8);
				}
				return TIMapRegionTemplate.GetSeaTravelMultiplier(region1.mapRegionTemplate.coast, region2.mapRegionTemplate.coast, flag, flag2, false);
			}
		}

		// Token: 0x06003A95 RID: 14997 RVA: 0x00158448 File Offset: 0x00156648
		public bool Battle()
		{
			using (List<TIArmyState>.Enumerator enumerator = this.armies.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.InBattleWithArmies())
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003A96 RID: 14998 RVA: 0x001584A4 File Offset: 0x001566A4
		public bool BorderWithAnotherNation(bool enemiesOnly)
		{
			bool flag = this.nation.wars.Count > 0;
			foreach (TIRegionState tiregionState in from r in this.AdjacentRegions(enemiesOnly)
				where r.nation != this.nation
				select r)
			{
				if (!enemiesOnly)
				{
					return true;
				}
				if (flag && this.nation.wars.Contains(tiregionState.nation))
				{
					return true;
				}
				if (!flag && this.nation.rivals.Contains(tiregionState.nation))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003A97 RID: 14999 RVA: 0x00158558 File Offset: 0x00156758
		public void DestroySpaceAssets(bool attack)
		{
			this.DestroySpaceFacility(SpaceFacilityType.launchFacility, attack);
			this.DestroySpaceFacility(SpaceFacilityType.missionControlFacility, attack);
			this.DestroySpaceFacility(SpaceFacilityType.spaceDefenseFacility, attack);
		}

		// Token: 0x06003A98 RID: 15000 RVA: 0x00158574 File Offset: 0x00156774
		public void DestroySpaceFacility(SpaceFacilityType facilityType, bool attack)
		{
			switch (facilityType)
			{
			case SpaceFacilityType.launchFacility:
				this.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -this.boostPerYear_dekatons, false, attack);
				return;
			case SpaceFacilityType.missionControlFacility:
				this.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, (float)(-(float)this.missionControl), false, attack);
				return;
			case SpaceFacilityType.spaceDefenseFacility:
				this.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, attack);
				return;
			default:
				return;
			}
		}

		// Token: 0x17000A56 RID: 2646
		// (get) Token: 0x06003A99 RID: 15001 RVA: 0x001585C8 File Offset: 0x001567C8
		private string NuclearDetonationEventName
		{
			get
			{
				return new StringBuilder("Nuclear Detonation").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x17000A57 RID: 2647
		// (get) Token: 0x06003A9A RID: 15002 RVA: 0x00158600 File Offset: 0x00156800
		public string ArmyEmbarkEventName
		{
			get
			{
				return new StringBuilder("EmbarkArmy").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x17000A58 RID: 2648
		// (get) Token: 0x06003A9B RID: 15003 RVA: 0x00158638 File Offset: 0x00156838
		public string ArmySeaTransitEventName
		{
			get
			{
				return new StringBuilder("TransitArmy").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06003A9C RID: 15004 RVA: 0x0015866D File Offset: 0x0015686D
		public void ChangeNuclearDetonations(int value)
		{
			this.nuclearDetonations = Mathf.Max(this.nuclearDetonations + value, 0);
		}

		// Token: 0x06003A9D RID: 15005 RVA: 0x00158684 File Offset: 0x00156884
		public void NuclearAttackOnRegion(TIFactionState launchingFaction, TINationState launchingNation = null)
		{
			if (launchingNation != null)
			{
				TIRegionState tiregionState = launchingNation.RandomRegionWeightedByPopulation();
				GameControl.eventManager.TriggerEvent(new NuclearLaunch(tiregionState), null, new object[] { tiregionState });
				TIDateTime tidateTime = new TIDateTime(this.gameTime.currentTime);
				tidateTime.AddSeconds(1800.0);
				string nuclearDetonationEventName = this.NuclearDetonationEventName;
				TITimeEvent.CreateNewTimeEvent(tidateTime, this, launchingNation, null, nuclearDetonationEventName, true, true, TITimeQueueRepeatType.None, 1, true, false);
				List<TINationState> list = new List<TINationState>();
				bool flag = launchingNation == this.nation || launchingNation.allies.Contains(this.nation);
				if (flag)
				{
					List<TIArmyState> list2 = (from x in this.FilteredArmiesPresent(false, false, true, false, false)
						where launchingNation.wars.Contains(x.homeNation)
						select x).ToList<TIArmyState>();
					if (list2.Count > 0)
					{
						list = list2.Select<TIArmyState, TINationState>((TIArmyState x) => x.homeNation).Distinct<TINationState>().ToList<TINationState>();
					}
					using (List<TIWarState>.Enumerator enumerator = launchingNation.findWarsWith(this.nation).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIWarState tiwarState = enumerator.Current;
							tiwarState.TallyDefensiveNuke();
						}
						goto IL_01A4;
					}
				}
				list.Add(this.nation);
				foreach (TIWarState tiwarState2 in launchingNation.findWarsWith(this.nation))
				{
					tiwarState2.AddNukedRegion(this);
				}
				IL_01A4:
				TIGlobalValuesState.GlobalValues.NuclearBarrageLaunched(launchingNation, this, flag ? this.nation : list[0]);
				using (List<TINationState>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TINationState tinationState = enumerator2.Current;
						TIFactionState executiveFaction = tinationState.executiveFaction;
						if (executiveFaction == null || executiveFaction.player.isAI)
						{
							AIDailyFactionPlanner.ConsiderNuclearAttack(tinationState, launchingNation, flag);
						}
					}
					goto IL_0223;
				}
			}
			this.OnNuclearAttackArrives(launchingFaction, null);
			IL_0223:
			this.nuclearDetonations++;
		}

		// Token: 0x06003A9E RID: 15006 RVA: 0x001588EC File Offset: 0x00156AEC
		public void NationLaunchedNuclearAttackArrival(TimeEventStart e)
		{
			if (e.eventObject == this)
			{
				this.OnNuclearAttackArrives(e.eventObject2.ref_nation.executiveFaction, e.eventObject2.ref_nation);
			}
		}

		// Token: 0x06003A9F RID: 15007 RVA: 0x0015891D File Offset: 0x00156B1D
		public void OnNuclearAttackArrives(TIFactionState applyingFaction, TINationState applyingNation = null)
		{
			Mood.TriggerEvent(Mood.Event.SDKL_MushroomCloud);
			GameControl.eventManager.TriggerEvent(new NuclearStrike(applyingNation, this), null, new object[] { this });
			this.ApplyDamageToRegion(1f, applyingFaction, applyingNation, true, true, true, true);
		}

		// Token: 0x06003AA0 RID: 15008 RVA: 0x00158954 File Offset: 0x00156B54
		public void ApplyDamageToRegion(float strength, TIFactionState applyingFaction = null, TINationState applyingNation = null, bool includeArmies = true, bool includeCouncilors = false, bool forceAttackSpaceAssets = false, bool nuclear = false)
		{
			if (strength > 0f)
			{
				bool flag = nuclear && (applyingNation == null || applyingNation.enemies.Contains(this.nation));
				double num;
				float num2;
				if (nuclear)
				{
					num = -1.0 * this.nationalGDPShareValue * (double)strength * (double)(0.75f + TIUtilities.RandomRange(0f, 0.5f)) * (flag ? 0.7 : 0.20000000298023224);
					num += (double)TIEffectsState.SumEffectsModifiers(Context.NuclearStrikeDamageReduction, this, (float)num, null);
					num2 = -1f * this.populationInMillions * strength * ((0.75f + TIUtilities.RandomRange(0f, 0.5f)) * (flag ? 0.25f : 0.025f));
					num2 += TIEffectsState.SumEffectsModifiers(Context.NuclearStrikeDamageReduction, this, num2, null);
					this.nation.AddToSustainability(this.NationalGDPProportion() * strength * (0.075f + TIUtilities.RandomRange(0f, 0.05f)) * (flag ? 1f : 0.05f));
				}
				else
				{
					num = -1.0 * this.nationalGDPShareValue * (double)strength * (double)(0.75f + TIUtilities.RandomRange(0f, 0.5f)) * 0.10000000149011612;
					num2 = -1f * this.populationInMillions * strength * ((0.75f + TIUtilities.RandomRange(0f, 0.5f)) * 0.001f);
					this.nation.AddToSustainability(this.NationalGDPProportion() * strength * (7.5E-06f + TIUtilities.RandomRange(0f, 5E-06f)));
				}
				this.nation.ModifyGDP(num, TINationState.GDPChangeReason.GDPReason_RegionDamage);
				this.ChangePopulation_Millions(num2, true);
				if ((nuclear || -num2 > 0.1f) && applyingFaction != null)
				{
					int num3 = ((flag && applyingNation != null && !this.nation.alienNation && applyingNation.defensiveWarStates.None<TIWarState>((TIWarState x) => x.attackingAlliance.Contains(this.nation))) ? 10 : 1);
					applyingFaction.CommitAtrocity((int)Mathf.Clamp(-num2 * 10f * (float)num3, 1f, 20f), TIFactionState.AtrocityCause.MassCasualtiesfromRegionDamage, false, 0.333f);
				}
				if (strength >= 0.9f)
				{
					if (nuclear)
					{
						float num4 = this.GlobalGDPProportion() * (flag ? 1f : 0.2f) * 0.25f;
						foreach (TINationState tinationState in GameStateManager.AllExtantHumanNations())
						{
							tinationState.GDPPctChange(-1f * (num4 + (TIUtilities.RandomFloatValue() + TIUtilities.RandomFloatValue()) / 100f), TINationState.GDPChangeReason.GDPReason_RegionDamage);
						}
						foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
						{
							foreach (TICouncilorState ticouncilorState in tifactionState.councilors)
							{
								if (ticouncilorState.homeRegion == this)
								{
									TITraitTemplate.ProcessLoyaltyChangeFromTraits(ticouncilorState, SpecialTraitRule.LoyaltyLossOnHomeRegionNuked, (applyingFaction == tifactionState) ? 2 : 1);
								}
							}
						}
					}
					if (flag)
					{
						if (this.coreEconomicRegion)
						{
							this.coreEconomicRegion = false;
							GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(this), null, new object[] { this });
							using (IEnumerator<TINationState> enumerator = GameStateManager.AllExtantHumanNations().GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									TINationState tinationState2 = enumerator.Current;
									tinationState2.GDPPctChange(-1f * (0.025f + (TIUtilities.RandomFloatValue() + TIUtilities.RandomFloatValue()) / 100f), TINationState.GDPChangeReason.GDPReason_GlobalCoreEconomicRegionDestroyed);
								}
								goto IL_0457;
							}
						}
						if (this.coreResourceRegion)
						{
							this.resourceRegion = false;
							this.oilRegion = false;
							GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(this), null, new object[] { this });
							foreach (TINationState tinationState3 in GameStateManager.AllExtantHumanNations())
							{
								tinationState3.GDPPctChange(-1f * (0.015f + (TIUtilities.RandomFloatValue() + TIUtilities.RandomFloatValue()) / 100f), TINationState.GDPChangeReason.GDPReason_GlobalCoreResourceRegionDestroyed);
							}
						}
						IL_0457:
						this.accumulatedCoreEconomyRegionTriggers = 0;
						this.accumulatedCoreMiningRegionTriggers = 0;
						this.accumulatedCoreOilRegionTriggers = 0;
						this.accumulatedDecolonizeTriggers = 0;
						this.accumulatedDecontaminateTriggers = 0;
					}
					foreach (PriorityType priorityType in Enums.PriorityTypes)
					{
						if (priorityType - PriorityType.Unity > 1 && priorityType != PriorityType.Spoils)
						{
							this.nation.ModifyAccumulatedInvestment(priorityType, 1f - strength, true, false);
						}
					}
					this.nation.SetDataDirty();
				}
				else if (TIUtilities.RandomFloatValue() < strength * 5f)
				{
					this.nation.ModifyAccumulatedInvestment(this.nation.GetRandomPriorityToDamage(), this.colonyRegion ? (1f - strength * 0.5f) : (1f - strength), true, true);
				}
				if (applyingNation != this.nation)
				{
					this.nation.ChangeAnnualSpaceFundingValue(-1f * (this.NationalGDPProportion() * this.nation.spaceFunding_year * strength * (nuclear ? 0.5f : 0.1f)));
					if (strength >= 0.75f)
					{
						this.DestroySpaceAssets(true);
					}
					else
					{
						if (this.boostPerMonth_dekatons > 0f && (TIUtilities.RandomFloatValue() < strength || forceAttackSpaceAssets))
						{
							this.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, -(this.boostPerYear_dekatons * strength), false, true);
						}
						if (this.missionControl > 0 && (TIUtilities.RandomFloatValue() < strength || forceAttackSpaceAssets))
						{
							this.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, -1f, false, true);
						}
						if (this.antiSpaceDefenses && (TIUtilities.RandomFloatValue() < strength || forceAttackSpaceAssets))
						{
							this.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, true);
						}
					}
				}
				if (includeArmies)
				{
					List<TIArmyState> list = this.armies.Where<TIArmyState>(delegate(TIArmyState army)
					{
						if (army.homeNation != applyingNation && !army.atSea)
						{
							TINationState applyingNation2 = applyingNation;
							if (applyingNation2 == null || !applyingNation2.allies.Contains(army.homeNation))
							{
								return army.faction != applyingFaction || applyingFaction == null;
							}
						}
						return false;
					}).ToList<TIArmyState>();
					TIFactionState applyingFaction2 = applyingFaction;
					if (applyingFaction2 == null || !applyingFaction2.IsAlienFaction)
					{
						list.AddRange(this.MegafaunaArmiesPresent());
					}
					list = list.OrderByDescending<TIArmyState, float>((TIArmyState x) => x.strength * x.techLevel).ToList<TIArmyState>();
					for (int j = list.Count - 1; j >= 0; j--)
					{
						if (nuclear && j > 0)
						{
							float num5 = strength;
							if (list[j].AlienRegularArmy || (Mathd.d100() < 50 && list[j].techLevel >= 3.8f))
							{
								float num6 = Mathf.Max(list[j].techLevel - 3.79f, 0f) * TIUtilities.RandomRange(1f, 5f);
								num5 -= num6 / 100f;
							}
							num5 = Mathf.Max(num5, 0f);
							num5 += TIEffectsState.SumEffectsModifiers(Context.ArmyNuclearHardening, list[j].faction, num5, null);
							list[j].TakeDamage(num5, applyingFaction, applyingNation, false);
						}
						else
						{
							list[j].TakeDamage(strength, applyingFaction, applyingNation, !nuclear);
						}
					}
					if (nuclear)
					{
						TIArmyState[] array2 = this.armies.Where<TIArmyState>((TIArmyState x) => !x.atSea).Except<TIArmyState>(list).ToArray<TIArmyState>();
						for (int k = array2.Length - 1; k >= 0; k--)
						{
							array2[k].TakeDamage(strength / (48f + TIUtilities.RandomRange(0f, 4f)), applyingFaction, applyingNation, false);
						}
					}
				}
				if (includeCouncilors)
				{
					foreach (TICouncilorState ticouncilorState2 in this.GetCouncilorsInRegion())
					{
						if (ticouncilorState2.traits.None<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.Survivor) && TIUtilities.RandomRange(0f, 2f) < strength)
						{
							TINotificationQueueState.LogCouncilorKilledInAttack(ticouncilorState2, ticouncilorState2.location);
							ticouncilorState2.KillCouncilor(true, applyingFaction);
						}
					}
				}
				if (nuclear)
				{
					this.xenoforming.SetXenoformingLevel(0f);
					TIGlobalValuesState.GlobalValues.TriggerNuclearDetonationEffect(true, applyingNation, this, this.nation);
					GameControl.eventManager.TriggerEvent(new RegionNuked(this), null, new object[] { this });
				}
				else if (applyingFaction == null || (!applyingFaction.IsAlienFaction && !applyingFaction.IsAlienProxy))
				{
					this.xenoforming.ChangeXenoformingLevel(-(this.xenoforming.xenoformingLevel * strength));
				}
				GameControl.eventManager.TriggerEvent(new RegionDamaged(this), null, new object[] { this });
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x06003AA1 RID: 15009 RVA: 0x001592D4 File Offset: 0x001574D4
		private void CompleteOccupationofRegion(TIArmyState army)
		{
			TINationState tinationState = this.leadOccupier ?? army.homeNation;
			List<TINationState> occupyingAlliance = this.GetOccupyingAlliance(true);
			List<TINationState> list = new List<TINationState>();
			TINationState nation = this.nation;
			if (!(this == nation.capital) || (nation.alienNation && nation.regions.Count != 1))
			{
				if (!nation.regions.All<TIRegionState>((TIRegionState x) => x.IsFullyOccupied()))
				{
					TINationState tinationState2 = GameStateManager.AlienNation();
					if (army != null && army.homeNation == tinationState2 && army.armyType != ArmyType.AlienMegafauna)
					{
						bool extant = tinationState2.extant;
						nation.TransferRegionsControlTo(new List<TIRegionState> { this }, tinationState2, false, extant, true, false, false);
						if (!extant)
						{
							tinationState2.controlPoints.ForEach(delegate(TIControlPoint x)
							{
								x.SetFaction(GameStateManager.AlienFaction(), false);
							});
							AIEvaluators.OnAlienNationCreated(false);
							TINotificationQueueState.LogAlienNationFounded(tinationState2, nation, false);
							this.occupations.Clear();
							goto IL_08C2;
						}
						goto IL_08C2;
					}
					else if (nation.alienNation)
					{
						TIFactionState tifactionState = GameStateManager.AlienFaction();
						List<TINationState> list2 = new List<TINationState>(this._claimsOnRegion);
						list2.Remove(nation);
						TINationState tinationState3 = null;
						bool flag = nation.capital == this;
						if (list2.Contains(tinationState))
						{
							tinationState3 = tinationState;
						}
						else
						{
							foreach (TINationState tinationState4 in occupyingAlliance)
							{
								if (tinationState4.IsAtWarWith(nation) && tinationState4.claims.Contains(this))
								{
									tinationState3 = tinationState4;
									break;
								}
							}
							if (tinationState3 == null)
							{
								foreach (TINationState tinationState5 in tinationState.allies.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).ToList<TINationState>())
								{
									if (tinationState5.claims.Contains(this))
									{
										tinationState3 = tinationState5;
										break;
									}
								}
							}
						}
						if (tinationState3 == null && list2.Count > 0)
						{
							Dictionary<TINationState, int> claimScores = list2.ToDictionary<TINationState, TINationState, int>((TINationState x) => x, (TINationState x) => 1);
							foreach (TINationState tinationState6 in list2)
							{
								if (tinationState6.capital == this)
								{
									Dictionary<TINationState, int> dictionary = claimScores;
									TINationState tinationState7 = tinationState6;
									dictionary[tinationState7] += 4;
								}
								else if (tinationState6.IsAdjacentToRegion(this, false))
								{
									Dictionary<TINationState, int> dictionary = claimScores;
									TINationState tinationState7 = tinationState6;
									dictionary[tinationState7] += 2;
								}
								if (tinationState.executiveFaction != null && tinationState6.executiveFaction == tinationState.executiveFaction)
								{
									Dictionary<TINationState, int> dictionary = claimScores;
									TINationState tinationState7 = tinationState6;
									dictionary[tinationState7]++;
								}
							}
							int highScore = claimScores.Values.Max();
							tinationState3 = claimScores.Keys.Where<TINationState>((TINationState x) => claimScores[x] == highScore).SelectRandomItem<TINationState>();
						}
						if (tinationState3 == null)
						{
							tinationState3 = tinationState;
						}
						bool extant2 = tinationState3.extant;
						if (!extant2 && tinationState != tinationState3)
						{
							tinationState3.InitiateAlliance(tinationState.executiveFaction, tinationState);
							foreach (TINationState tinationState8 in tinationState.allies)
							{
								if (tinationState8.wars.Contains(nation) && tinationState8 != tinationState3)
								{
									tinationState8.InitiateAlliance(tinationState8.executiveFaction, tinationState3);
								}
							}
						}
						if (!extant2)
						{
							nation.TransferRegionsControlTo(new List<TIRegionState> { this }, tinationState3, false, false, false, true, false);
							tinationState3.RegimeChange(tinationState, occupyingAlliance, army);
							nation.DeclareFullWar(nation.executiveFaction, tinationState3);
						}
						else
						{
							nation.TransferRegionsControlTo(new List<TIRegionState> { this }, tinationState3, false, false, false, false, false);
						}
						if (!flag)
						{
							goto IL_08C2;
						}
						nation.AddToUnrest(1f, TINationState.UnrestChangeReason.UnrestReason_AliensLostCapitalChaos, 10f);
						TINotificationQueueState.LogAlienNationCapitalConquered(nation, this, nation.capital);
						List<TIRegionState> list3 = new List<TIRegionState>(nation.regions);
						list3.Remove(nation.capital);
						TIFactionState faction = army.faction;
						foreach (TIRegionState tiregionState in list3)
						{
							foreach (TINationState tinationState9 in tiregionState.SecessionCandidates())
							{
								float num = 10000f * nation.unrest + (float)(this.AdjacentRegions(false).Contains(this) ? 100000 : 0);
								if (faction != null)
								{
									num += TIEffectsState.SumEffectsModifiers(Context.BreakawayChance, faction, num, null);
								}
								if (tinationState9.PostUnrestSecessionCheck(faction, num, false))
								{
									break;
								}
							}
						}
						if (!(faction != null))
						{
							goto IL_08C2;
						}
						if (Mathd.d100() < 10)
						{
							faction.CompleteMilestone(CampaignMilestone.AccessAlienTech);
						}
						if (Mathd.d100() < 2)
						{
							faction.CompleteMilestone(CampaignMilestone.AccessHydraCorpus);
						}
						if (Mathd.d100() < 2)
						{
							faction.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
						}
						else if (Mathd.d100() < 5)
						{
							faction.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
						}
						if (Mathd.d100() < 10)
						{
							faction.CompleteMilestone(CampaignMilestone.AccessWarDogCorpus);
							goto IL_08C2;
						}
						goto IL_08C2;
					}
					else
					{
						if (army != null)
						{
							TINotificationQueueState.LogArmyCompletesOccupationOfRegion(army);
							goto IL_08C2;
						}
						goto IL_08C2;
					}
				}
			}
			List<TIRegionState> list4 = new List<TIRegionState>();
			List<TINationState> list5 = new List<TINationState>();
			foreach (TIRegionState tiregionState2 in nation.regions)
			{
				if (tinationState.claims.Contains(tiregionState2))
				{
					list4.Add(tiregionState2);
				}
			}
			if (list4.Count > 0)
			{
				list5.Add(tinationState);
				nation.TransferRegionsControlTo(list4, tinationState, true, false, false, false, false);
				if (list4.Count == tinationState.regions.Count)
				{
					list.Add(tinationState);
				}
			}
			foreach (TINationState tinationState10 in occupyingAlliance.ToList<TINationState>())
			{
				List<TIRegionState> list6 = new List<TIRegionState>();
				foreach (TIRegionState tiregionState3 in nation.regions)
				{
					if (tinationState10.IsAtWarWith(nation) && tinationState10.claims.Contains(tiregionState3))
					{
						list6.Add(tiregionState3);
					}
				}
				if (list6.Count > 0)
				{
					nation.TransferRegionsControlTo(list6, tinationState10, true, false, false, false, false);
					if (list6.Count == tinationState10.regions.Count)
					{
						list.Add(tinationState10);
					}
					if (!list5.Contains(tinationState10))
					{
						list5.Add(tinationState10);
					}
				}
			}
			if (nation.extant && !tinationState.alienNation)
			{
				nation.RegimeChange(tinationState, occupyingAlliance, army);
			}
			else
			{
				if (this.nation != null && this.nation != nation)
				{
					this.nation.AbsorbNation(this.nation.executiveFaction, nation);
				}
				else
				{
					TINationState tinationState11 = list5.FirstOrDefault<TINationState>((TINationState x) => x.regions.Contains(this));
					if (tinationState11 != null)
					{
						tinationState11.AbsorbNation(tinationState.executiveFaction, nation);
					}
				}
				foreach (TINationState tinationState12 in list)
				{
					if (tinationState12 == GameStateManager.AlienNation())
					{
						foreach (TIControlPoint ticontrolPoint in tinationState12.controlPoints)
						{
							if (ticontrolPoint.faction != GameStateManager.AlienFaction())
							{
								tinationState12.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Annexation, GameStateManager.AlienFaction());
							}
						}
						AIEvaluators.OnAlienNationCreated(false);
						TINotificationQueueState.LogAlienNationFounded(tinationState12, nation, false);
					}
					else
					{
						TINotificationQueueState.LogIndependence(tinationState12, nation);
					}
				}
				if (nation.alienNation)
				{
					nation.AlienNationOverthrown(occupyingAlliance, army);
				}
				else if (army != null && list5 != null && list5.Count > 0)
				{
					TINotificationQueueState.LogArmyConquersNation(army, nation, this, list5);
				}
			}
			IL_08C2:
			foreach (TIArmyState tiarmyState in this.armies)
			{
				tiarmyState.SetArmyDataDirty();
			}
		}

		// Token: 0x06003AA2 RID: 15010 RVA: 0x00159C6C File Offset: 0x00157E6C
		public void LiberateMyRegion()
		{
			this.occupations.Clear();
			this.SetLeadOccupier();
		}

		// Token: 0x06003AA3 RID: 15011 RVA: 0x00159C7F File Offset: 0x00157E7F
		public void SetLeadOccupier()
		{
			this.leadOccupier = this.GetLeadOccupierInFullOccupation();
		}

		// Token: 0x06003AA4 RID: 15012 RVA: 0x00159C8D File Offset: 0x00157E8D
		public bool IsFullyOccupied()
		{
			return this.leadOccupier != null;
		}

		// Token: 0x06003AA5 RID: 15013 RVA: 0x00159C9C File Offset: 0x00157E9C
		public bool OccupiedOrOccupationUnderway()
		{
			if (this.occupations.Count > 0)
			{
				return this.occupations.Values.Any<float>((float x) => x > 0f);
			}
			return false;
		}

		// Token: 0x06003AA6 RID: 15014 RVA: 0x00159CE8 File Offset: 0x00157EE8
		public bool OccupationUnderwayButNotComplete()
		{
			if (this.occupations.Count > 0 && this.leadOccupier == null)
			{
				return this.occupations.Values.Any<float>((float x) => x > 0f && x < 1f);
			}
			return false;
		}

		// Token: 0x06003AA7 RID: 15015 RVA: 0x00159D42 File Offset: 0x00157F42
		public bool NoOccupationUnderwayOrComplete()
		{
			if (this.occupations.Count != 0)
			{
				return this.occupations.Values.All<float>((float x) => x == 0f);
			}
			return true;
		}

		// Token: 0x06003AA8 RID: 15016 RVA: 0x00159D84 File Offset: 0x00157F84
		public void ValidateAndCleanOccupations()
		{
			List<TINationState> list = new List<TINationState>();
			foreach (TINationState tinationState in this.occupations.Keys)
			{
				if (!this.nation.wars.Contains(tinationState))
				{
					list.Add(tinationState);
				}
			}
			foreach (TINationState tinationState2 in list)
			{
				this.occupations.Remove(tinationState2);
			}
			this.CheckAndEndAnnexation(false);
			TINationState leadOccupier = this.leadOccupier;
			this.SetLeadOccupier();
			if (list.Count > 0 || leadOccupier != this.leadOccupier)
			{
				GameControl.eventManager.TriggerEvent(new OccupationStatusChange(this), null, new object[] { this }.Where<object>((object x) => x != null).ToArray<object>());
			}
		}

		// Token: 0x06003AA9 RID: 15017 RVA: 0x00159EAC File Offset: 0x001580AC
		public TINationState GetLeadOccupierInFullOccupation()
		{
			TINationState tinationState;
			List<TINationState> list;
			if (this.GetHighestWarAllianceOccupationValue(out tinationState, out list) >= 1f)
			{
				return tinationState;
			}
			return null;
		}

		// Token: 0x06003AAA RID: 15018 RVA: 0x00159ED0 File Offset: 0x001580D0
		public List<TINationState> GetOccupyingAlliance(bool ordered = false)
		{
			TINationState tinationState;
			List<TINationState> list;
			if (this.GetHighestWarAllianceOccupationValue(out tinationState, out list) >= 1f)
			{
				if (ordered)
				{
					list = (from x in list
						orderby this.GetIndividualOccupationValue(x) descending, x.militaryStrength descending
						select x).ToList<TINationState>();
				}
				return list;
			}
			return new List<TINationState>();
		}

		// Token: 0x06003AAB RID: 15019 RVA: 0x00159F34 File Offset: 0x00158134
		public bool PartofOccupyingAlliance(TINationState nation)
		{
			return this.GetOccupyingAlliance(false).Contains(nation);
		}

		// Token: 0x06003AAC RID: 15020 RVA: 0x00159F43 File Offset: 0x00158143
		public float GetIndividualOccupationValue(TINationState occupyingNation)
		{
			if (this.occupations.ContainsKey(occupyingNation))
			{
				return this.occupations[occupyingNation];
			}
			return 0f;
		}

		// Token: 0x06003AAD RID: 15021 RVA: 0x00159F68 File Offset: 0x00158168
		public float GetHighestWarAllianceOccupationValueByNation(TINationState occupyingNation, out TINationState allianceLeader)
		{
			Dictionary<TIWarState, float> allianceOccupationsbyWar = new Dictionary<TIWarState, float>();
			foreach (TIWarState tiwarState in this.nation.currentWarStates)
			{
				IReadOnlyList<TINationState> readOnlyList = tiwarState.EnemyAlliance(this.nation);
				if (readOnlyList.Contains(occupyingNation))
				{
					allianceOccupationsbyWar.Add(tiwarState, 0f);
					foreach (TINationState tinationState in readOnlyList)
					{
						Dictionary<TIWarState, float> allianceOccupationsbyWar2 = allianceOccupationsbyWar;
						TIWarState tiwarState2 = tiwarState;
						allianceOccupationsbyWar2[tiwarState2] += this.GetIndividualOccupationValue(tinationState);
					}
				}
			}
			if (allianceOccupationsbyWar.Count > 0)
			{
				TIWarState tiwarState3 = allianceOccupationsbyWar.Keys.MaxBy<TIWarState, float>((TIWarState x) => allianceOccupationsbyWar[x]);
				allianceLeader = tiwarState3.Alliance(occupyingNation).MaxBy<TINationState, float>((TINationState x) => this.GetIndividualOccupationValue(x));
				return Mathf.Min(1f, allianceOccupationsbyWar[tiwarState3]);
			}
			allianceLeader = null;
			return 0f;
		}

		// Token: 0x06003AAE RID: 15022 RVA: 0x0015A0C0 File Offset: 0x001582C0
		public float GetHighestWarAllianceOccupationValue(out TINationState leaderOfLeadingAlliance, out List<TINationState> occupyingAlliance)
		{
			Dictionary<TIWarState, float> allianceOccupationsbyWar = new Dictionary<TIWarState, float>();
			Dictionary<TIWarState, TINationState> dictionary = new Dictionary<TIWarState, TINationState>();
			foreach (TIWarState tiwarState in this.nation.currentWarStates)
			{
				allianceOccupationsbyWar.Add(tiwarState, 0f);
				dictionary.Add(tiwarState, null);
				float num = -1f;
				TINationState tinationState = null;
				foreach (TINationState tinationState2 in tiwarState.EnemyAlliance(this.nation))
				{
					float individualOccupationValue = this.GetIndividualOccupationValue(tinationState2);
					Dictionary<TIWarState, float> allianceOccupationsbyWar2 = allianceOccupationsbyWar;
					TIWarState tiwarState2 = tiwarState;
					allianceOccupationsbyWar2[tiwarState2] += individualOccupationValue;
					if (individualOccupationValue > num)
					{
						num = individualOccupationValue;
						tinationState = tinationState2;
					}
				}
				dictionary[tiwarState] = tinationState;
			}
			if (allianceOccupationsbyWar.Count > 0)
			{
				TIWarState tiwarState3 = allianceOccupationsbyWar.Keys.MaxBy<TIWarState, float>((TIWarState x) => allianceOccupationsbyWar[x]);
				leaderOfLeadingAlliance = dictionary[tiwarState3];
				occupyingAlliance = tiwarState3.EnemyAlliance(this.nation).ToList<TINationState>();
				return Mathf.Min(1f, allianceOccupationsbyWar[tiwarState3]);
			}
			leaderOfLeadingAlliance = null;
			occupyingAlliance = new List<TINationState>();
			return 0f;
		}

		// Token: 0x06003AAF RID: 15023 RVA: 0x0015A244 File Offset: 0x00158444
		public void CheckAndTriggerOccupation(TIArmyState army)
		{
			if ((army == null || !army.CanReduceOccupation()) && (!this.IsFullyOccupied() || (army != null && !army.InBattleWithArmies() && (!this.GetOccupyingAlliance(false).Contains(army.homeNation) || army.AlienRegularArmy))))
			{
				TINationState leadOccupier = this.leadOccupier;
				TINationState tinationState;
				if (army != null && army.AlienRegularArmy && leadOccupier != GameStateManager.AlienNation() && this.GetHighestWarAllianceOccupationValueByNation(GameStateManager.AlienNation(), out tinationState) >= 1f)
				{
					this.leadOccupier = GameStateManager.AlienNation();
				}
				else
				{
					this.SetLeadOccupier();
				}
				if (this.leadOccupier != null && this.leadOccupier != leadOccupier)
				{
					this.CompleteOccupationofRegion(army);
					GameControl.eventManager.TriggerEvent(new OccupationStatusChange(this), null, new object[] { this, this.leadOccupier, army }.Where<object>((object x) => x != null).ToArray<object>());
				}
			}
		}

		// Token: 0x06003AB0 RID: 15024 RVA: 0x0015A35C File Offset: 0x0015855C
		public void IncreaseOccupationValue(TINationState occupyingNation, float value, TIArmyState army = null)
		{
			if (value != 0f)
			{
				bool flag = this.IsFullyOccupied();
				if (this.occupations.ContainsKey(occupyingNation))
				{
					Dictionary<TINationState, float> occupations = this.occupations;
					TINationState occupyingNation2 = occupyingNation;
					occupations[occupyingNation2] += value;
					this.occupations[occupyingNation] = Mathf.Clamp(this.occupations[occupyingNation], 0f, 1f);
				}
				else
				{
					this.occupations.Add(occupyingNation, Mathf.Clamp(value, 0f, 1f));
				}
				GameControl.eventManager.TriggerEvent(new RegionOccupationValueChange(this), null, new object[]
				{
					this,
					(army != null) ? army.homeNation : null,
					army
				}.Where<object>((object x) => x != null).ToArray<object>());
				if (value > 0f)
				{
					this.CheckAndTriggerOccupation(army);
				}
				else if (value < 0f && flag)
				{
					this.SetLeadOccupier();
					GameControl.eventManager.TriggerEvent(new OccupationStatusChange(this), null, new object[] { this }.Where<object>((object x) => x != null).ToArray<object>());
				}
				IEnumerable<TIWarState> currentWarStates = this.nation.currentWarStates;
				Func<TIWarState, bool> <>9__2;
				Func<TIWarState, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (TIWarState x) => x.allBelligerents.Contains(occupyingNation));
				}
				foreach (TIWarState tiwarState in currentWarStates.Where<TIWarState>(func))
				{
					tiwarState.FightingOccurs();
				}
			}
		}

		// Token: 0x06003AB1 RID: 15025 RVA: 0x0015A538 File Offset: 0x00158738
		public void SetOccupationValue(TINationState occupyingNation, float value, TIArmyState army = null)
		{
			if (this.occupations.ContainsKey(occupyingNation))
			{
				this.occupations[occupyingNation] = Mathf.Clamp(value, 0f, 1f);
			}
			else if (value > 0f)
			{
				this.occupations.Add(occupyingNation, Mathf.Clamp(value, 0f, 1f));
			}
			GameControl.eventManager.TriggerEvent(new RegionOccupationValueChange(this), null, new object[]
			{
				this,
				(army != null) ? army.homeNation : null,
				army
			}.Where<object>((object x) => x != null).ToArray<object>());
			this.CheckAndTriggerOccupation(army);
		}

		// Token: 0x06003AB2 RID: 15026 RVA: 0x0015A5F4 File Offset: 0x001587F4
		public float RegionArmyActionMultiplier(bool invert = true)
		{
			float num = (Mathf.Pow(this.populationInMillions, 0.75f) / Mathf.Pow(TIGlobalValuesState.GlobalValues.averageRegionPopulation, 0.75f) + this.regionSizeFactor) / 2f;
			if (invert)
			{
				num = 1f / num;
			}
			return num;
		}

		// Token: 0x17000A59 RID: 2649
		// (get) Token: 0x06003AB3 RID: 15027 RVA: 0x0015A640 File Offset: 0x00158840
		// (set) Token: 0x06003AB4 RID: 15028 RVA: 0x0015A648 File Offset: 0x00158848
		public bool isBeingAnnexed { get; private set; }

		// Token: 0x17000A5A RID: 2650
		// (get) Token: 0x06003AB5 RID: 15029 RVA: 0x0015A651 File Offset: 0x00158851
		// (set) Token: 0x06003AB6 RID: 15030 RVA: 0x0015A659 File Offset: 0x00158859
		public TIArmyState annexingArmy { get; private set; }

		// Token: 0x17000A5B RID: 2651
		// (get) Token: 0x06003AB7 RID: 15031 RVA: 0x0015A662 File Offset: 0x00158862
		// (set) Token: 0x06003AB8 RID: 15032 RVA: 0x0015A66A File Offset: 0x0015886A
		public TIDateTime annexationBeginDate { get; private set; }

		// Token: 0x17000A5C RID: 2652
		// (get) Token: 0x06003AB9 RID: 15033 RVA: 0x0015A673 File Offset: 0x00158873
		// (set) Token: 0x06003ABA RID: 15034 RVA: 0x0015A67B File Offset: 0x0015887B
		public float annexationDaysLeft { get; private set; }

		// Token: 0x17000A5D RID: 2653
		// (get) Token: 0x06003ABB RID: 15035 RVA: 0x0015A684 File Offset: 0x00158884
		public TIDateTime annexationEndDate
		{
			get
			{
				TIDateTime tidateTime = new TIDateTime(TITimeState.Now());
				tidateTime.AddDays(this.annexationDaysLeft);
				return tidateTime;
			}
		}

		// Token: 0x06003ABC RID: 15036 RVA: 0x0015A69C File Offset: 0x0015889C
		public bool ValidRegionToAnnexOrLiberate(TIArmyState army)
		{
			return !this.isBeingAnnexed && army.homeNation.wars.Contains(army.currentNation) && (army.homeNation.claims.Contains(army.currentRegion) || TIRegionState.LiberationTarget(army) != null) && army.currentNation.capital != this && army.currentRegion.IsFullyOccupied() && army.currentRegion.GetOccupyingAlliance(false).Contains(army.homeNation);
		}

		// Token: 0x06003ABD RID: 15037 RVA: 0x0015A728 File Offset: 0x00158928
		public void BeginAnnexation(TIArmyState annexingArmy, float days)
		{
			this.isBeingAnnexed = true;
			this.annexationBeginDate = TITimeState.Now();
			if (days <= 1f)
			{
				days = 1f;
			}
			this.annexationDaysLeft = days;
			this.annexingArmy = annexingArmy;
			GameControl.eventManager.TriggerEvent(new RegionAnnexationValueChange(this, annexingArmy), null, new object[] { this, annexingArmy }.Where<object>((object x) => x != null).ToArray<object>());
		}

		// Token: 0x06003ABE RID: 15038 RVA: 0x0015A7B0 File Offset: 0x001589B0
		public void EndAnnexation()
		{
			if (this.isBeingAnnexed)
			{
				if (TIGameState.Valid(this.annexingArmy))
				{
					OperationData operationData = null;
					foreach (OperationData operationData2 in this.annexingArmy.CurrentOperations())
					{
						if (operationData2.operation is AnnexRegionOperation && operationData2.target == this)
						{
							operationData = operationData2;
						}
					}
					if (operationData != null)
					{
						GameTimeManager.Singleton.CancelTimeEvent(this.annexingArmy.armyOperationCompleteEventName, this.annexingArmy, this, operationData.operation as TIOperationTemplate, operationData.completionDate);
						this.annexingArmy.RemoveOperation(operationData);
					}
				}
				this.isBeingAnnexed = false;
				GameControl.eventManager.TriggerEvent(new RegionAnnexationValueChange(this, this.annexingArmy), null, new object[] { this, this.annexingArmy }.Where<object>((object x) => x != null).ToArray<object>());
			}
			this.annexingArmy = null;
			this.annexationBeginDate = null;
		}

		// Token: 0x06003ABF RID: 15039 RVA: 0x0015A8E0 File Offset: 0x00158AE0
		public float PercentAnnexed()
		{
			if (this.isBeingAnnexed)
			{
				return (float)(TITimeState.Now().DifferenceInDays(this.annexationBeginDate) / this.annexationEndDate.DifferenceInDays(this.annexationBeginDate));
			}
			return 0f;
		}

		// Token: 0x06003AC0 RID: 15040 RVA: 0x0015A913 File Offset: 0x00158B13
		public static TINationState LiberationTarget(TIArmyState liberatingArmy)
		{
			return liberatingArmy.currentRegion.SecessionCandidates().FirstOrDefault<TINationState>();
		}

		// Token: 0x06003AC1 RID: 15041 RVA: 0x0015A928 File Offset: 0x00158B28
		public bool CheckAndEndAnnexation(bool force)
		{
			bool flag = false;
			if (force)
			{
				flag = true;
			}
			else if (this.isBeingAnnexed)
			{
				if (TIGameState.Valid(this.annexingArmy) && this.annexingArmy.strength > TIGlobalConfig.globalConfig.armyStrengthToLiberate && !(this.annexingArmy.currentRegion != this) && !this.annexingArmy.atSea && this.annexingArmy.CurrentOperations().Count != 0)
				{
					if (!this.annexingArmy.CurrentOperations().NotAll<OperationData>((OperationData x) => x.operation is AnnexRegionOperation))
					{
						if (!this.annexingArmy.homeNation.wars.Contains(this.annexingArmy.currentNation))
						{
							flag = true;
							goto IL_0154;
						}
						if (!this.annexingArmy.homeNation.claims.Contains(this.annexingArmy.currentRegion) && TIRegionState.LiberationTarget(this.annexingArmy) == null)
						{
							flag = true;
							goto IL_0154;
						}
						if (this.annexingArmy.currentNation.capital == this)
						{
							flag = true;
							goto IL_0154;
						}
						if (!this.annexingArmy.currentRegion.GetOccupyingAlliance(false).Contains(this.annexingArmy.homeNation))
						{
							flag = true;
							goto IL_0154;
						}
						if (!this.IsFullyOccupied())
						{
							flag = true;
							goto IL_0154;
						}
						goto IL_0154;
					}
				}
				flag = true;
			}
			IL_0154:
			if (flag)
			{
				this.EndAnnexation();
			}
			return flag;
		}

		// Token: 0x06003AC2 RID: 15042 RVA: 0x0015AA94 File Offset: 0x00158C94
		public void AnnexationDay()
		{
			if (this.CheckAndEndAnnexation(false))
			{
				return;
			}
			this.annexationDaysLeft -= 1f;
			List<TIWarState> list = this.annexingArmy.homeNation.findWarsWith(this.nation);
			list.ForEach(delegate(TIWarState x)
			{
				x.FightingOccurs();
			});
			if (this.annexationDaysLeft < 1f)
			{
				TINationState nation = this.nation;
				if (this.annexingArmy.homeNation.claims.Contains(this.annexingArmy.currentRegion))
				{
					this.nation.TransferRegionsControlTo(new List<TIRegionState> { this }, this.annexingArmy.homeNation, true, false, false, false, false);
				}
				else
				{
					TINationState tinationState = TIRegionState.LiberationTarget(this.annexingArmy);
					if (tinationState != null)
					{
						this.annexingArmy.homeNation.InitiateAlliance(this.annexingArmy.faction, tinationState);
						this.annexingArmy.currentNation.Secession(this.annexingArmy.faction, tinationState, new List<TIRegionState> { this.annexingArmy.currentRegion }, this.annexingArmy.homeNation);
						if (nation.ValidNewWarTarget(tinationState, false))
						{
							nation.DeclareFullWar(null, tinationState);
						}
					}
					list.ForEach(delegate(TIWarState x)
					{
						x.annexedRegions.Add(this);
					});
				}
				this.EndAnnexation();
				GameControl.eventManager.TriggerEvent(new NationDataUpdated(nation), null, new object[] { this, nation }.Where<object>((object x) => x != null).ToArray<object>());
				GameControl.eventManager.TriggerEvent(new NationDataUpdated(this.nation), null, new object[] { this, this.nation }.Where<object>((object x) => x != null).ToArray<object>());
				return;
			}
			GameControl.eventManager.TriggerEvent(new RegionAnnexationValueChange(this, this.annexingArmy), null, new object[] { this, this.annexingArmy }.Where<object>((object x) => x != null).ToArray<object>());
		}

		// Token: 0x06003AC3 RID: 15043 RVA: 0x0015ACE4 File Offset: 0x00158EE4
		public List<TIArmyState> FilteredArmiesPresent(bool includeNations, bool includeAllAllies, bool includeNationsEnemies, bool includeAtSea, bool includeOnlyWarActiveAllies)
		{
			List<TIArmyState> list = new List<TIArmyState>();
			foreach (TIArmyState tiarmyState in this.armies)
			{
				ArmySeaTransitStage armySeaTransitStage = tiarmyState.SeaTransitStage();
				if (armySeaTransitStage != ArmySeaTransitStage.Sea_DestinationRegion && (includeAtSea || armySeaTransitStage != ArmySeaTransitStage.Sea_HomeRegion))
				{
					if (tiarmyState.AlienMegafaunaArmy)
					{
						if (includeNations && this.nation.alienNation && tiarmyState.faction.IsAlienFaction)
						{
							list.Add(tiarmyState);
						}
						else if (includeNationsEnemies && (!this.nation.alienNation || !tiarmyState.faction.IsAlienFaction))
						{
							list.Add(tiarmyState);
						}
					}
					else if (includeNations && tiarmyState.homeNation == this.nation)
					{
						list.Add(tiarmyState);
					}
					else if (includeAllAllies && tiarmyState.homeNation.IsAlliedWith(this.nation, false))
					{
						list.Add(tiarmyState);
					}
					else if (includeOnlyWarActiveAllies && tiarmyState.homeNation.IsAlliedWith(this.nation, false) && tiarmyState.homeNation.CurrentWarAllies_AllWars().Contains(tiarmyState.currentNation))
					{
						list.Add(tiarmyState);
					}
					else if (includeNationsEnemies && tiarmyState.homeNation.IsAtWarWith(this.nation))
					{
						list.Add(tiarmyState);
					}
				}
			}
			return list;
		}

		// Token: 0x06003AC4 RID: 15044 RVA: 0x0015AE58 File Offset: 0x00159058
		public int NumArmiesPresent(bool includeNations, bool includeAllies, bool includeEnemies, bool includeOnlyWarActiveAllies)
		{
			return this.FilteredArmiesPresent(includeNations, includeAllies, includeEnemies, false, includeOnlyWarActiveAllies).Count;
		}

		// Token: 0x06003AC5 RID: 15045 RVA: 0x0015AE6B File Offset: 0x0015906B
		public List<TIArmyState> MegafaunaArmiesPresent()
		{
			return this.armies.Where<TIArmyState>((TIArmyState x) => x.AlienMegafaunaArmy).ToList<TIArmyState>();
		}

		// Token: 0x06003AC6 RID: 15046 RVA: 0x0015AE9C File Offset: 0x0015909C
		public List<TIArmyState> FactionArmiesPresent(TIFactionState faction, bool includeNations, bool includeAllies, bool includeEnemies, bool includeMegafauna)
		{
			List<TIArmyState> list = new List<TIArmyState>();
			foreach (TIArmyState tiarmyState in this.armies)
			{
				TINationState homeNation = tiarmyState.homeNation;
				if (tiarmyState.AlienMegafaunaArmy)
				{
					if (includeMegafauna && tiarmyState.faction == faction)
					{
						list.Add(tiarmyState);
					}
				}
				else if (tiarmyState.currentRegion == this && tiarmyState.faction == faction)
				{
					if (includeNations && homeNation == this.nation)
					{
						list.Add(tiarmyState);
					}
					if (includeAllies && homeNation.IsAlliedWith(this.nation, false))
					{
						list.Add(tiarmyState);
					}
					if (includeEnemies && homeNation.IsAtWarWith(this.nation))
					{
						list.Add(tiarmyState);
					}
				}
			}
			return list;
		}

		// Token: 0x06003AC7 RID: 15047 RVA: 0x0015AF88 File Offset: 0x00159188
		public int NumFactionArmiesPresent(TIFactionState faction, bool includeNations, bool includeAllies, bool includeEnemies, bool includeMegafauna)
		{
			int num = 0;
			foreach (TIArmyState tiarmyState in this.armies)
			{
				TINationState homeNation = tiarmyState.homeNation;
				if (tiarmyState.AlienMegafaunaArmy)
				{
					if (includeMegafauna && faction.IsAlienFaction)
					{
						num++;
					}
				}
				else if (tiarmyState.currentRegion == this && tiarmyState.faction == faction)
				{
					if (includeNations && homeNation == this.nation)
					{
						num++;
					}
					if (includeAllies && homeNation.IsAlliedWith(this.nation, false))
					{
						num++;
					}
					if (includeEnemies && homeNation.IsAtWarWith(this.nation))
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x17000A5E RID: 2654
		// (get) Token: 0x06003AC8 RID: 15048 RVA: 0x0015B05C File Offset: 0x0015925C
		public TINationState GetOccupierNation
		{
			get
			{
				TINationState leadOccupier = this.leadOccupier;
				if (leadOccupier == null)
				{
					List<TINationState> list;
					this.GetHighestWarAllianceOccupationValue(out leadOccupier, out list);
				}
				return leadOccupier;
			}
		}

		// Token: 0x06003AC9 RID: 15049 RVA: 0x0015B088 File Offset: 0x00159288
		public float GenericLocalForcesDefenseLevel(bool modifyForCohesion)
		{
			if (this.IsFullyOccupied())
			{
				TINationState getOccupierNation = this.GetOccupierNation;
				return getOccupierNation.militaryTechLevel + getOccupierNation.adviserCommandBonus;
			}
			if (this.nation.military)
			{
				float num = this.nation.militaryTechLevel + this.nation.adviserCommandBonus + TemplateManager.global.baseRegionDefenseBonus;
				if (this.terrain == TerrainType.Rugged)
				{
					num += TemplateManager.global.ruggedTerrainDefenseBonus;
					num += TIEffectsState.SumEffectsModifiers(Context.ArmyRuggedWarfare, this.nation.executiveFaction, num, null);
				}
				if (this.coreEconomicRegion)
				{
					num += TemplateManager.global.coreEconomicRegionDefenseBonus;
					num += TIEffectsState.SumEffectsModifiers(Context.ArmyUrbanWarfare, this.nation.executiveFaction, num, null);
				}
				if (modifyForCohesion)
				{
					num += this.nation.cohesion * TemplateManager.global.defenseCohesionMultiplier;
					num += this.nation.unrest * TemplateManager.global.defenseUnrestMultiplier;
				}
				num += TIArmyState.LocalForcesAdjacentRegionsBonus(this);
				return num * (this.occupations.Values.Any<float>((float x) => x > 0f) ? (1f - this.occupations.Values.Max() * TemplateManager.global.localDefensesDamageEffectivenessFactor) : 1f);
			}
			return 0f;
		}

		// Token: 0x17000A5F RID: 2655
		// (get) Token: 0x06003ACA RID: 15050 RVA: 0x0015B1DF File Offset: 0x001593DF
		public float population
		{
			get
			{
				return this.populationInMillions * 1000000f;
			}
		}

		// Token: 0x17000A60 RID: 2656
		// (get) Token: 0x06003ACB RID: 15051 RVA: 0x0015B1ED File Offset: 0x001593ED
		public float populationDensity
		{
			get
			{
				return this.population / this.mapRegionTemplate.area_km2;
			}
		}

		// Token: 0x17000A61 RID: 2657
		// (get) Token: 0x06003ACC RID: 15052 RVA: 0x0015B204 File Offset: 0x00159404
		public double annualPopulationGrowth
		{
			get
			{
				float populationRegressionPeriod_years = GameStateManager.Time().template.populationRegressionPeriod_years;
				double num = 4.49788037409348;
				return Mathd.Max(Mathd.Clamp(num + Mathd.Max(-num, -0.418190741 * (double)this.nation.education) + -0.0624798523403752 * (double)this.nation.cohesion + 9.80843732089162E-06 * (double)Mathf.Min(180000f, this.nation.perCapitaGDP) + -0.115739931206548 * (double)Mathf.Sqrt(Mathf.Abs(this.latitude)) + (double)(this.annualPopGrowthModifier * Mathf.Max(0f, (populationRegressionPeriod_years - TITimeState.CampaignDuration_years_Exact()) / populationRegressionPeriod_years)) + (double)this.nation.template.popGrowthModifier - (double)(this.xenoforming.xenoformingLevel / 200f) - (double)(this.nuclearDetonations * 4), -10.0, 10.0) - (double)(Math.Max(0f, Mathf.Abs(GameStateManager.GlobalValues().temperatureAnomaly_C) - 8f) * ((this.template.environment == EnvironmentType.Beneficiary) ? 0.5f : ((this.template.environment == EnvironmentType.Vulnerable) ? 2f : 1f))), -100.0) * 0.01;
			}
		}

		// Token: 0x06003ACD RID: 15053 RVA: 0x0015B368 File Offset: 0x00159568
		public void GrowPopulationByMonth()
		{
			double num = Mathd.Pow(1.0 + this.annualPopulationGrowth, 0.0833333358168602) - 1.0;
			num += (double)TIUtilities.RandomRange(-0.000412f, 0.000412f);
			float populationInMillions = this.populationInMillions;
			this.populationInMillions *= 1f + (float)num;
			this.populationInMillions = Mathf.Max(this.populationInMillions, 0.001f);
			float num2 = this.populationInMillions - populationInMillions;
			GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(new FactionResource[]
				{
					FactionResource.Influence,
					FactionResource.Research
				});
			});
			if (num2 < 0f)
			{
				this.nation.ModifyGDP(this.regionalPerCapitaGDP * (double)num2 * 1000000.0, TINationState.GDPChangeReason.GDPReason_PopulationChange);
				this.nation.AddToEducation(Mathf.Clamp(num2 / 100f, -0.005f, 0f), TINationState.EducationChangeReason.EducationReason_PopulationLoss);
			}
			else
			{
				this.nation.ModifyGDP(this.regionalPerCapitaGDP * (double)num2 * 1000000.0, TINationState.GDPChangeReason.GDPReason_PopulationChange);
			}
			this.nation.SetPriorityEffectPopScaling();
		}

		// Token: 0x06003ACE RID: 15054 RVA: 0x0015B490 File Offset: 0x00159690
		public void ChangePopulation_Millions(float value, bool modifyGDPForChange = true)
		{
			this.populationInMillions += value;
			this.populationInMillions = Mathf.Max(this.populationInMillions, 0.001f);
			GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(new FactionResource[]
				{
					FactionResource.Influence,
					FactionResource.Research
				});
			});
			if (modifyGDPForChange && value < 0f)
			{
				this.nation.ModifyGDP(this.regionalPerCapitaGDP * (double)value * 1000000.0, TINationState.GDPChangeReason.GDPReason_PopulationChange);
			}
			this.nation.SetPriorityEffectPopScaling();
		}

		// Token: 0x06003ACF RID: 15055 RVA: 0x0015B524 File Offset: 0x00159724
		public float PropagandaOnPop(TIFactionIdeologyTemplate targetIdeologyTemplate, float strength)
		{
			strength *= this.populationInMillions / this.nation.population_Millions;
			return this.nation.PropagandaOnPop(targetIdeologyTemplate, strength, false);
		}

		// Token: 0x06003AD0 RID: 15056 RVA: 0x0015B54A File Offset: 0x0015974A
		public void ChangeAnnualPopulationGrowthModifier(float value)
		{
			this.annualPopGrowthModifier += value;
		}

		// Token: 0x06003AD1 RID: 15057 RVA: 0x0015B55C File Offset: 0x0015975C
		public bool ClaimedBy(TINationState nationState, bool requireExtantNation = false, bool requireProjectGatePassed = true, bool includeCurrentOwner = true)
		{
			if (!this._claimsOnRegion.Contains(nationState))
			{
				return false;
			}
			if (requireExtantNation && !nationState.extant)
			{
				return false;
			}
			if (!includeCurrentOwner && nationState.regions.Contains(this))
			{
				return false;
			}
			if (requireProjectGatePassed)
			{
				TIBilateralTemplate tibilateralTemplate = TemplateManager.Find<TIBilateralTemplate>(new StringBuilder("Claim").Append(nationState.templateName).Append(this.templateName).ToString(), false);
				if (tibilateralTemplate != null && !tibilateralTemplate.BilateralIsActive())
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06003AD2 RID: 15058 RVA: 0x0015B5DC File Offset: 0x001597DC
		public void AddClaim(TINationState nation)
		{
			this._claimsOnRegion.AddUnique(nation);
		}

		// Token: 0x06003AD3 RID: 15059 RVA: 0x0015B5EB File Offset: 0x001597EB
		public void RemoveClaim(TINationState nation)
		{
			this._claimsOnRegion.Remove(nation);
		}

		// Token: 0x06003AD4 RID: 15060 RVA: 0x0015B5FC File Offset: 0x001597FC
		public List<TINationState> SecessionCandidates()
		{
			List<TINationState> list = new List<TINationState>();
			foreach (TINationState tinationState in this._claimsOnRegion)
			{
				if (!tinationState.extant && tinationState.originalCapital == this && this.nation.capital != this)
				{
					TIBilateralTemplate tibilateralTemplate = TemplateManager.Find<TIBilateralTemplate>(new StringBuilder("Claim").Append(tinationState.templateName).Append(this.templateName).ToString(), false);
					if (tibilateralTemplate != null && tibilateralTemplate.BilateralIsActive())
					{
						list.Add(tinationState);
					}
				}
			}
			return list;
		}

		// Token: 0x06003AD5 RID: 15061 RVA: 0x0015B6B8 File Offset: 0x001598B8
		public List<TINationState> NationsWithClaim(bool requireExtantNation = false, bool requireExtantClaim = true, bool includeCurrentOwner = true, bool capitalsOnly = false)
		{
			List<TINationState> list = new List<TINationState>();
			foreach (TINationState tinationState in this._claimsOnRegion)
			{
				if (((!requireExtantNation && !(tinationState == GameStateManager.AlienNation())) || tinationState.extant) && (!capitalsOnly || ((!tinationState.extant || !(this != tinationState.capital)) && (tinationState.extant || !(this != tinationState.originalCapital)))) && (includeCurrentOwner || !tinationState.regions.Contains(this)))
				{
					TIBilateralTemplate tibilateralTemplate = TemplateManager.Find<TIBilateralTemplate>(new StringBuilder("Claim").Append(tinationState.templateName).Append(this.templateName).ToString(), false);
					if (tibilateralTemplate != null && (tibilateralTemplate == null || !requireExtantClaim || tibilateralTemplate.BilateralIsActive()))
					{
						list.Add(tinationState);
					}
				}
			}
			return list;
		}

		// Token: 0x06003AD6 RID: 15062 RVA: 0x0015B7B4 File Offset: 0x001599B4
		public List<TIRegionState> ThisAndAdjacentRegions(bool IAmAnInvadingArmy)
		{
			List<TIRegionState> list = new List<TIRegionState>();
			list.Add(this);
			list.AddRange(this.AdjacentRegions(IAmAnInvadingArmy));
			return list;
		}

		// Token: 0x06003AD7 RID: 15063 RVA: 0x0015B7D0 File Offset: 0x001599D0
		public List<TINationState> AdjacentNations(bool includingOwner, bool IAmAnInvadingArmy)
		{
			List<TINationState> list = new List<TINationState>();
			foreach (TIRegionState tiregionState in this.AdjacentRegions(IAmAnInvadingArmy))
			{
				if (!list.Contains(tiregionState.nation) && (includingOwner || tiregionState.nation != this.nation))
				{
					list.Add(tiregionState.nation);
				}
			}
			return list;
		}

		// Token: 0x06003AD8 RID: 15064 RVA: 0x0015B854 File Offset: 0x00159A54
		public List<TIRegionState> AdjacentRegions(bool IAmAnInvadingArmy)
		{
			if (!IAmAnInvadingArmy)
			{
				return this.adjacencies.Keys.ToList<TIRegionState>();
			}
			return this.adjacencies.Keys.Where<TIRegionState>((TIRegionState region) => this.adjacencies[region] == TerrestrialAdjacencyType.FullAdjacency).ToList<TIRegionState>();
		}

		// Token: 0x06003AD9 RID: 15065 RVA: 0x0015B88C File Offset: 0x00159A8C
		public TerrestrialAdjacencyType GetAdjacencyType(TIRegionState regionState)
		{
			TerrestrialAdjacencyType terrestrialAdjacencyType;
			if (this.adjacencies.TryGetValue(regionState, out terrestrialAdjacencyType))
			{
				return terrestrialAdjacencyType;
			}
			return TerrestrialAdjacencyType.None;
		}

		// Token: 0x06003ADA RID: 15066 RVA: 0x0015B8AC File Offset: 0x00159AAC
		public bool IsAdjacent(TIRegionState region, bool IAmAnInvadingArmy)
		{
			switch (this.GetAdjacencyType(region))
			{
			case TerrestrialAdjacencyType.None:
				return false;
			case TerrestrialAdjacencyType.FriendlyCrossingOnly:
				return !IAmAnInvadingArmy;
			case TerrestrialAdjacencyType.FullAdjacency:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06003ADB RID: 15067 RVA: 0x0015B8E0 File Offset: 0x00159AE0
		public void ChangeAdjacency(TIRegionState region, TerrestrialAdjacencyType newAdjacencyType)
		{
			this.adjacencies[region] = newAdjacencyType;
			region.adjacencies[this] = newAdjacencyType;
			this.nation.GenerateAdjacentNationsDictionary();
			if (this.nation != region.nation)
			{
				region.nation.GenerateAdjacentNationsDictionary();
			}
			Func<TINationState, bool> <>9__1;
			Func<TINationState, bool> <>9__2;
			Func<TINationState, bool> <>9__3;
			Func<TINationState, bool> <>9__4;
			foreach (TINationState tinationState in GameStateManager.AllNations().Where<TINationState>(delegate(TINationState x)
			{
				if (!x.regions.Contains(this) && !x.regions.Contains(region))
				{
					IEnumerable<TINationState> allies = x.allies;
					Func<TINationState, bool> func;
					if ((func = <>9__1) == null)
					{
						func = (<>9__1 = (TINationState y) => y.regions.Contains(this));
					}
					if (!allies.Any<TINationState>(func))
					{
						IEnumerable<TINationState> allies2 = x.allies;
						Func<TINationState, bool> func2;
						if ((func2 = <>9__2) == null)
						{
							func2 = (<>9__2 = (TINationState y) => y.regions.Contains(region));
						}
						if (!allies2.Any<TINationState>(func2))
						{
							IEnumerable<TINationState> wars = x.wars;
							Func<TINationState, bool> func3;
							if ((func3 = <>9__3) == null)
							{
								func3 = (<>9__3 = (TINationState y) => y.regions.Contains(this));
							}
							if (!wars.Any<TINationState>(func3))
							{
								IEnumerable<TINationState> wars2 = x.wars;
								Func<TINationState, bool> func4;
								if ((func4 = <>9__4) == null)
								{
									func4 = (<>9__4 = (TINationState y) => y.regions.Contains(region));
								}
								return wars2.Any<TINationState>(func4);
							}
						}
					}
				}
				return true;
			}))
			{
				tinationState.SetArmyAccessibilityDirty();
			}
		}

		// Token: 0x06003ADC RID: 15068 RVA: 0x0015B9A8 File Offset: 0x00159BA8
		private void SetAdjacencies()
		{
			foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
			{
				if (tibilateralTemplate.BilateralIsActive())
				{
					TIRegionState regionState = tibilateralTemplate.regionState1;
					if (((regionState != null) ? regionState.templateName : null) == this.template.dataName)
					{
						if (tibilateralTemplate.relationType == BilateralRelationType.PhysicalAdjacency)
						{
							if (tibilateralTemplate.friendlyOnly)
							{
								this.adjacencies.Add(tibilateralTemplate.regionState2, TerrestrialAdjacencyType.FriendlyCrossingOnly);
							}
							else
							{
								this.adjacencies.Add(tibilateralTemplate.regionState2, TerrestrialAdjacencyType.FullAdjacency);
							}
						}
					}
					else
					{
						TIRegionState regionState2 = tibilateralTemplate.regionState2;
						if (((regionState2 != null) ? regionState2.templateName : null) == this.template.dataName && tibilateralTemplate.relationType == BilateralRelationType.PhysicalAdjacency)
						{
							if (tibilateralTemplate.friendlyOnly)
							{
								this.adjacencies.Add(tibilateralTemplate.regionState1, TerrestrialAdjacencyType.FriendlyCrossingOnly);
							}
							else
							{
								this.adjacencies.Add(tibilateralTemplate.regionState1, TerrestrialAdjacencyType.FullAdjacency);
							}
						}
					}
				}
			}
		}

		// Token: 0x06003ADD RID: 15069 RVA: 0x0015BABC File Offset: 0x00159CBC
		public void ChangeOceanType(WorldOceanType newOceanType)
		{
			this.oceanType = newOceanType;
		}

		// Token: 0x17000A62 RID: 2658
		// (get) Token: 0x06003ADE RID: 15070 RVA: 0x0015BAC5 File Offset: 0x00159CC5
		public static IEnumerable<TIRegionState> Regions
		{
			get
			{
				return GameStateManager.AllRegions();
			}
		}

		// Token: 0x17000A63 RID: 2659
		// (get) Token: 0x06003ADF RID: 15071 RVA: 0x0015BACC File Offset: 0x00159CCC
		public static IEnumerable<TIRegionState> CoastalRegions
		{
			get
			{
				return TIRegionState.Regions.Where<TIRegionState>((TIRegionState x) => x.isCoastal);
			}
		}

		// Token: 0x17000A64 RID: 2660
		// (get) Token: 0x06003AE0 RID: 15072 RVA: 0x0015BAF8 File Offset: 0x00159CF8
		public IEnumerable<TIRegionState> Neighbors
		{
			get
			{
				if (this.neighbors == null)
				{
					this.neighbors = new List<TIRegionState>();
					foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
					{
						if (tibilateralTemplate.relationType == BilateralRelationType.PhysicalAdjacency)
						{
							TIRegionState regionState = tibilateralTemplate.regionState1;
							bool flag = ((regionState != null) ? regionState.templateName : null) == this.template.dataName;
							bool flag2;
							if (!flag)
							{
								TIRegionState regionState2 = tibilateralTemplate.regionState2;
								flag2 = ((regionState2 != null) ? regionState2.templateName : null) == this.template.dataName;
							}
							else
							{
								flag2 = false;
							}
							bool flag3 = flag2;
							if ((flag || flag3) && tibilateralTemplate.BilateralCanBeActive)
							{
								TIRegionState tiregionState = (flag ? tibilateralTemplate.regionState2 : tibilateralTemplate.regionState1);
								this.neighbors.Add(tiregionState);
							}
						}
					}
				}
				return this.neighbors;
			}
		}

		// Token: 0x17000A65 RID: 2661
		// (get) Token: 0x06003AE1 RID: 15073 RVA: 0x0015BBE4 File Offset: 0x00159DE4
		public IEnumerable<TIRegionState> ConnectedRegions
		{
			get
			{
				IEnumerable<TIRegionState> enumerable = this.Neighbors;
				if (this.onTheWater)
				{
					enumerable = enumerable.Union<TIRegionState>(TIRegionState.CoastalRegions);
				}
				return enumerable;
			}
		}

		// Token: 0x06003AE2 RID: 15074 RVA: 0x0015BC10 File Offset: 0x00159E10
		public List<TICouncilorState> GetCouncilorsInRegion()
		{
			return (from x in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.activeCouncilors)
				where x.location == this
				select x).ToList<TICouncilorState>();
		}

		// Token: 0x06003AE3 RID: 15075 RVA: 0x0015BC5C File Offset: 0x00159E5C
		public List<TICouncilorState> GetVisibleCouncilorsInRegion(TIFactionState faction)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			if (faction != null)
			{
				foreach (TICouncilorState ticouncilorState in this.GetCouncilorsInRegion())
				{
					if (faction.HasIntelOnCouncilorLocation(ticouncilorState))
					{
						list.Add(ticouncilorState);
					}
				}
			}
			return list;
		}

		// Token: 0x06003AE4 RID: 15076 RVA: 0x0015BCC8 File Offset: 0x00159EC8
		public void ConductAbductions(TIFactionState faction, int number)
		{
			this.abductions = Mathf.Max(this.abductions + number, 0);
			faction.abductions = Mathf.Max(faction.abductions + number, 0);
			if (number > 0)
			{
				faction.AddToCurrentResource((float)number * TemplateManager.global.influenceGainFromAbductions, FactionResource.Influence, false, "Abductions");
			}
			this.nation.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Influence);
			});
		}

		// Token: 0x06003AE5 RID: 15077 RVA: 0x0015BD4B File Offset: 0x00159F4B
		public float GetAbductionsMissionBonusFromRegion()
		{
			return Mathf.Min((float)this.abductions, TemplateManager.global.maxAbductionMissionImpact) * TemplateManager.global.GetAbductionMissionBonusDifficultyScaling();
		}

		// Token: 0x06003AE6 RID: 15078 RVA: 0x0015BD70 File Offset: 0x00159F70
		public TIRegionState NearestInSupraRegion(bool includeIslands)
		{
			TIRegionState tiregionState = null;
			float num = float.PositiveInfinity;
			foreach (TIRegionState tiregionState2 in GameStateManager.AllRegions())
			{
				if (tiregionState2.mapRegionTemplate.supraRegion == this.mapRegionTemplate.supraRegion && (includeIslands || !tiregionState2.mapRegionTemplate.island) && tiregionState2 != this)
				{
					float num2 = this.DistanceToRegion_km(tiregionState2);
					if (num2 < num)
					{
						tiregionState = tiregionState2;
						num = num2;
					}
				}
			}
			return tiregionState;
		}

		// Token: 0x06003AE7 RID: 15079 RVA: 0x0015BDE8 File Offset: 0x00159FE8
		public bool AllowedDestinationForAlienCouncilor(TICouncilorState councilor)
		{
			if (!councilor.location.isRegionState)
			{
				return false;
			}
			TIRegionState ref_region = councilor.location.ref_region;
			if (ref_region == this)
			{
				return true;
			}
			if (this.IsAdjacent(ref_region, false))
			{
				return true;
			}
			if (this.nation.alienNation)
			{
				return true;
			}
			foreach (SpecialRegionAdjacencies specialRegionAdjacencies in councilor.faction.specialRegionAdjacencies)
			{
				if (specialRegionAdjacencies.region1 == ref_region && specialRegionAdjacencies.region2 == this)
				{
					return true;
				}
				if (specialRegionAdjacencies.region2 == ref_region && specialRegionAdjacencies.region1 == this)
				{
					return true;
				}
			}
			if ((ref_region.mapRegionTemplate.island || ref_region.AdjacentRegions(true).Count == 0) && ref_region == councilor.priorLocation && this.mapRegionTemplate.supraRegion == ref_region.mapRegionTemplate.supraRegion && ref_region.NearestInSupraRegion(false) == this)
			{
				return true;
			}
			bool flag = ref_region.hasAlienFacility || ref_region.ref_UFOLanding.Extant() || ref_region.nation.alienNation || ref_region.nation.inAlienFederation || ref_region.nation.alienAlly || TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, ref_region.nation.executiveFaction);
			bool flag2 = this.hasAlienFacility || this.nation.alienNation || this.nation.inAlienFederation || this.nation.alienAlly || this.ref_UFOLanding.Extant() || TIEffectsState.CheckForAnyEffectInContext(Context.AlienRelationsEstablished, this.nation.executiveFaction);
			return flag && flag2;
		}

		// Token: 0x06003AE8 RID: 15080 RVA: 0x0015BFC0 File Offset: 0x0015A1C0
		public List<TICouncilorState> GetProtectors()
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors))
			{
				if (ticouncilorState.active && ticouncilorState.location == this && ticouncilorState.protectingTarget == this)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x06003AE9 RID: 15081 RVA: 0x0015C05C File Offset: 0x0015A25C
		public float GetProtectionBonus(CouncilorAttribute attribute)
		{
			float num = 0f;
			foreach (TICouncilorState ticouncilorState in this.GetProtectors())
			{
				num += (float)ticouncilorState.GetAttribute(attribute, true, true, true, false, false, false);
			}
			return num;
		}

		// Token: 0x17000A66 RID: 2662
		// (get) Token: 0x06003AEA RID: 15082 RVA: 0x0015C0C0 File Offset: 0x0015A2C0
		public int maxMissionControl
		{
			get
			{
				return Mathf.Max(this.missionControl, 1 + (int)(this.nationalGDPShareValue_bn / (double)Mathf.Max(200f, 300f - 6f * this.nation.education)));
			}
		}

		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x06003AEB RID: 15083 RVA: 0x0015C0F9 File Offset: 0x0015A2F9
		public bool canLaunch
		{
			get
			{
				return this.boostPerYear_dekatons > 0f;
			}
		}

		// Token: 0x06003AEC RID: 15084 RVA: 0x0015C108 File Offset: 0x0015A308
		public TIRegionSpaceFacilityState GetRegionSpaceFacility(SpaceFacilityType facilityType)
		{
			return this.spaceFacilities.Single<TIRegionSpaceFacilityState>((TIRegionSpaceFacilityState x) => x.spaceFacilityType == facilityType);
		}

		// Token: 0x06003AED RID: 15085 RVA: 0x0015C13C File Offset: 0x0015A33C
		public int ChangeSpaceFacilityValue(SpaceFacilityType facilityType, float fValue = 0f, bool bValue = false, bool attack = false)
		{
			int num = 0;
			switch (facilityType)
			{
			case SpaceFacilityType.launchFacility:
				this.boostPerYear_dekatons += fValue;
				this.boostPerYear_dekatons = Mathf.Max(0f, this.boostPerYear_dekatons);
				if (this.nation.inFederation)
				{
					TIFederationState federation = this.nation.federation;
					if (federation != null)
					{
						federation.ref_factions.ForEach(delegate(TIFactionState x)
						{
							x.SetResourceIncomeDataDirty(FactionResource.Boost);
						});
					}
				}
				else
				{
					this.nation.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
					{
						x.SetResourceIncomeDataDirty(FactionResource.Boost);
					});
				}
				if (this.numSTOFighters > this.maxSTOFighters)
				{
					int num2 = this.numSTOFighters - this.maxSTOFighters;
					for (int i = 0; i < num2; i++)
					{
						this.DestroyRandomSTOFighter();
					}
					num = num2;
				}
				if (TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron) && !this.nation.ValidPriority(PriorityType.Military_BuildSTOSquadron))
				{
					this.nation.PossiblePriorityValidationChange(true);
				}
				break;
			case SpaceFacilityType.missionControlFacility:
			{
				int num3 = this.missionControl;
				this.missionControl += (int)fValue;
				if (fValue < 0f)
				{
					this.missionControl = Mathf.Clamp(this.missionControl, 0, this.maxMissionControl);
				}
				else if (fValue > 0f)
				{
					if (this.missionControl > this.maxMissionControl)
					{
						this.missionControl = Mathf.Max(0, num3);
					}
				}
				else
				{
					this.missionControl = Mathf.Max(0, this.missionControl);
				}
				this.nation.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.MissionControl);
				});
				if (!this.nation.ValidPriority(PriorityType.MissionControl))
				{
					this.nation.PossiblePriorityValidationChange(true);
				}
				break;
			}
			case SpaceFacilityType.spaceDefenseFacility:
				this.antiSpaceDefenses = bValue;
				if (this.antiSpaceDefenses)
				{
					this.spaceDefenseFacility.SetLaserDefenseWeaponTemplate();
				}
				if (!this.nation.ValidPriority(PriorityType.Military_BuildSpaceDefenses))
				{
					this.nation.PossiblePriorityValidationChange(true);
				}
				break;
			}
			if (attack)
			{
				TIRegionSpaceFacilityState regionSpaceFacility = this.GetRegionSpaceFacility(facilityType);
				GameControl.eventManager.TriggerEvent(new SpaceFacilityTakesDamage(regionSpaceFacility), null, new object[] { regionSpaceFacility, this });
			}
			GameControl.eventManager.TriggerEvent(new RegionDataUpdated(this), null, new object[] { this });
			return num;
		}

		// Token: 0x06003AEE RID: 15086 RVA: 0x0015C397 File Offset: 0x0015A597
		public void UnderBombardment()
		{
			if (this.antiSpaceDefenses && !this.isCounterfiring)
			{
				this.spaceDefenseFacility.SetLaserDefenseWeaponTemplate();
			}
			this.underBombardment = true;
		}

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x06003AEF RID: 15087 RVA: 0x0015C3BC File Offset: 0x0015A5BC
		public string baseSTOFireStr
		{
			get
			{
				return new StringBuilder("STOFireMission").Append(base.ID.ToString()).ToString();
			}
		}

		// Token: 0x06003AF0 RID: 15088 RVA: 0x0015C3F4 File Offset: 0x0015A5F4
		public void EndBombardment(TISpaceFleetState endingFleet)
		{
			this.underBombardment = this.spaceBody.fleetsInOrbit.Any<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				if (x != endingFleet)
				{
					TIGameState bombardmentTarget = x.bombardmentTarget;
					return ((bombardmentTarget != null) ? bombardmentTarget.ref_region : null) == this;
				}
				return false;
			});
			if (!this.underBombardment)
			{
				this.isCounterfiring = false;
			}
		}

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06003AF1 RID: 15089 RVA: 0x0015C446 File Offset: 0x0015A646
		public int numSTOFightersOnCooldown
		{
			get
			{
				return this.STOFighterCooldownExpiry.Count;
			}
		}

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06003AF2 RID: 15090 RVA: 0x0015C453 File Offset: 0x0015A653
		public int maxSTOFighters
		{
			get
			{
				if (this.boostPerMonth_dekatons >= 1f)
				{
					return Mathf.Clamp(Mathf.CeilToInt(this.boostPerMonth_dekatons / 4f), 1, 64);
				}
				return 0;
			}
		}

		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x06003AF3 RID: 15091 RVA: 0x0015C47D File Offset: 0x0015A67D
		public int availableSTOFighters
		{
			get
			{
				if (!this.IsFullyOccupied())
				{
					return this.numSTOFighters - this.numSTOFightersOnCooldown;
				}
				return 0;
			}
		}

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x06003AF4 RID: 15092 RVA: 0x0015C496 File Offset: 0x0015A696
		public string fighterSquadronName
		{
			get
			{
				return this.template.fighterSquadronName;
			}
		}

		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x06003AF5 RID: 15093 RVA: 0x0015C4A3 File Offset: 0x0015A6A3
		public bool canAddSTOFighter
		{
			get
			{
				return this.maxSTOFighters > 0 && !this.IsFullyOccupied();
			}
		}

		// Token: 0x06003AF6 RID: 15094 RVA: 0x0015C4BC File Offset: 0x0015A6BC
		public void SetSTOFighterOnCooldown(int duration_days)
		{
			TIDateTime tidateTime = TITimeState.Now();
			tidateTime.AddDays((float)duration_days);
			this.STOFighterCooldownExpiry.Add(tidateTime);
		}

		// Token: 0x06003AF7 RID: 15095 RVA: 0x0015C4E4 File Offset: 0x0015A6E4
		public void CheckSTOFighterCooldowns()
		{
			bool flag = false;
			foreach (TIDateTime tidateTime in this.STOFighterCooldownExpiry.ToList<TIDateTime>())
			{
				if (TITimeState.Now() > tidateTime)
				{
					this.STOFighterCooldownExpiry.Remove(tidateTime);
					flag = true;
				}
			}
			if (flag)
			{
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x06003AF8 RID: 15096 RVA: 0x0015C574 File Offset: 0x0015A774
		public void DestroyRandomSTOFighter()
		{
			int num = TIUtilities.RandomRange(0, this.numSTOFighters);
			if (num + 1 > this.availableSTOFighters && this.STOFighterCooldownExpiry.Count > num - this.availableSTOFighters)
			{
				this.STOFighterCooldownExpiry.RemoveAt(num - this.availableSTOFighters);
			}
			this.numSTOFighters--;
		}

		// Token: 0x06003AF9 RID: 15097 RVA: 0x0015C5CF File Offset: 0x0015A7CF
		public void DestroyAllSTOFighters(bool forceUpdate = false)
		{
			bool flag = forceUpdate && this.numSTOFighters > 0;
			this.numSTOFighters = 0;
			this.STOFighterCooldownExpiry.Clear();
			if (flag)
			{
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(this), null, new object[] { this });
			}
		}

		// Token: 0x0400257D RID: 9597
		[SerializeField]
		private Dictionary<TIRegionState, TerrestrialAdjacencyType> adjacencies;

		// Token: 0x0400257E RID: 9598
		public int missionControl;

		// Token: 0x0400257F RID: 9599
		public float boostPerYear_dekatons;

		// Token: 0x04002581 RID: 9601
		public bool coreEconomicRegion;

		// Token: 0x04002582 RID: 9602
		public bool resourceRegion;

		// Token: 0x04002583 RID: 9603
		public bool oilRegion;

		// Token: 0x04002584 RID: 9604
		public bool colonyRegion;

		// Token: 0x04002585 RID: 9605
		public bool permanentlyDecolonized;

		// Token: 0x04002586 RID: 9606
		public int nuclearDetonations;

		// Token: 0x04002587 RID: 9607
		public WorldOceanType oceanType;

		// Token: 0x04002588 RID: 9608
		private TISpaceBodyState _spaceBody;

		// Token: 0x0400258C RID: 9612
		public int numSTOFighters;

		// Token: 0x0400258D RID: 9613
		public List<TIDateTime> STOFighterCooldownExpiry;

		// Token: 0x0400258E RID: 9614
		public List<TIRegionSpaceFacilityState> spaceFacilities;

		// Token: 0x0400258F RID: 9615
		public TILaunchFacilityState boostFacility;

		// Token: 0x04002590 RID: 9616
		public TIMissionControlFacilityState missionControlFacility;

		// Token: 0x04002591 RID: 9617
		public TISpaceDefensesFacilityState spaceDefenseFacility;

		// Token: 0x04002592 RID: 9618
		public List<TIArmyState> armies;

		// Token: 0x04002598 RID: 9624
		public int abductions;

		// Token: 0x04002599 RID: 9625
		private GameTimeManager gameTime;

		// Token: 0x0400259A RID: 9626
		private TIMapRegionTemplate _mapRegionTemplate;

		// Token: 0x0400259B RID: 9627
		private Vector3 localized_coordinates_offset;

		// Token: 0x0400259C RID: 9628
		private List<TINationState> _claimsOnRegion = new List<TINationState>();

		// Token: 0x0400259D RID: 9629
		public TINationState originalColony;

		// Token: 0x0400259F RID: 9631
		public int accumulatedCoreEconomyRegionTriggers;

		// Token: 0x040025A0 RID: 9632
		public int accumulatedCoreOilRegionTriggers;

		// Token: 0x040025A1 RID: 9633
		public int accumulatedCoreMiningRegionTriggers;

		// Token: 0x040025A2 RID: 9634
		public int accumulatedDecolonizeTriggers;

		// Token: 0x040025A3 RID: 9635
		public int accumulatedDecontaminateTriggers;

		// Token: 0x040025A4 RID: 9636
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040025A5 RID: 9637
		private float regionSizeFactor;

		// Token: 0x040025A6 RID: 9638
		private Vector3d standardLocalPosition;

		// Token: 0x040025A7 RID: 9639
		private Dictionary<TIRegionState, float> _distanceToRegion = new Dictionary<TIRegionState, float>();

		// Token: 0x040025AC RID: 9644
		private List<TIRegionState> neighbors;

		// Token: 0x040025AD RID: 9645
		public const int cooldownForHealthyFighter_days = 7;

		// Token: 0x040025AE RID: 9646
		public const int cooldownForDamagedFighter_days = 14;

		// Token: 0x040025AF RID: 9647
		public const int maxMaxSTOFighters_Region = 64;

		// Token: 0x040025B0 RID: 9648
		public const float boostDivisorForMaxSTOFighters = 4f;
	}
}
