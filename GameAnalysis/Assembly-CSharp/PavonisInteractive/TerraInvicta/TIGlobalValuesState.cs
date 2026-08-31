using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000780 RID: 1920
	public class TIGlobalValuesState : TIGameState
	{
		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x06003BCA RID: 15306 RVA: 0x00168F46 File Offset: 0x00167146
		// (set) Token: 0x06003BCB RID: 15307 RVA: 0x00168F4E File Offset: 0x0016714E
		public float earthAtmosphericCO2_ppm { get; private set; }

		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x06003BCC RID: 15308 RVA: 0x00168F57 File Offset: 0x00167157
		// (set) Token: 0x06003BCD RID: 15309 RVA: 0x00168F5F File Offset: 0x0016715F
		public float earthAtmosphericCH4_ppm { get; private set; }

		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x06003BCE RID: 15310 RVA: 0x00168F68 File Offset: 0x00167168
		// (set) Token: 0x06003BCF RID: 15311 RVA: 0x00168F70 File Offset: 0x00167170
		public float earthAtmosphericN2O_ppm { get; private set; }

		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x06003BD0 RID: 15312 RVA: 0x00168F79 File Offset: 0x00167179
		// (set) Token: 0x06003BD1 RID: 15313 RVA: 0x00168F81 File Offset: 0x00167181
		public float stratosphericAerosols_ppm { get; private set; }

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x06003BD2 RID: 15314 RVA: 0x00168F8A File Offset: 0x0016718A
		// (set) Token: 0x06003BD3 RID: 15315 RVA: 0x00168F92 File Offset: 0x00167192
		public float globalSeaLevelAnomaly_cm { get; private set; }

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x06003BD4 RID: 15316 RVA: 0x00168F9B File Offset: 0x0016719B
		// (set) Token: 0x06003BD5 RID: 15317 RVA: 0x00168FA3 File Offset: 0x001671A3
		public float initialSustainabilityMin { get; private set; }

		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x06003BD6 RID: 15318 RVA: 0x00168FAC File Offset: 0x001671AC
		// (set) Token: 0x06003BD7 RID: 15319 RVA: 0x00168FB4 File Offset: 0x001671B4
		[fsIgnore]
		public float sustainabilityHelperModifier { get; private set; }

		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x06003BD8 RID: 15320 RVA: 0x00168FBD File Offset: 0x001671BD
		// (set) Token: 0x06003BD9 RID: 15321 RVA: 0x00168FC5 File Offset: 0x001671C5
		public int nuclearStrikes { get; private set; }

		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x06003BDA RID: 15322 RVA: 0x00168FCE File Offset: 0x001671CE
		// (set) Token: 0x06003BDB RID: 15323 RVA: 0x00168FD6 File Offset: 0x001671D6
		public int looseNukes { get; private set; }

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x06003BDC RID: 15324 RVA: 0x00168FDF File Offset: 0x001671DF
		// (set) Token: 0x06003BDD RID: 15325 RVA: 0x00168FE7 File Offset: 0x001671E7
		public int difficulty { get; private set; }

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x06003BDE RID: 15326 RVA: 0x00168FF0 File Offset: 0x001671F0
		// (set) Token: 0x06003BDF RID: 15327 RVA: 0x00168FF8 File Offset: 0x001671F8
		public float bestGlobalHumanMiltech { get; private set; }

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x06003BE0 RID: 15328 RVA: 0x00169001 File Offset: 0x00167201
		// (set) Token: 0x06003BE1 RID: 15329 RVA: 0x00169009 File Offset: 0x00167209
		public float bestGlobalHumanEducation { get; private set; }

		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x06003BE2 RID: 15330 RVA: 0x00169012 File Offset: 0x00167212
		// (set) Token: 0x06003BE3 RID: 15331 RVA: 0x0016901A File Offset: 0x0016721A
		public int controlPointMaintenanceFreebies { get; private set; }

		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x06003BE4 RID: 15332 RVA: 0x00169023 File Offset: 0x00167223
		public static bool usingCustomizations
		{
			get
			{
				return TIGlobalValuesState.GlobalValues.scenarioCustomizations.usingCustomizations;
			}
		}

		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x06003BE5 RID: 15333 RVA: 0x00169034 File Offset: 0x00167234
		public static ScenarioCustomizations Customizations
		{
			get
			{
				TIGlobalValuesState globalValues = TIGlobalValuesState.GlobalValues;
				return ((globalValues != null) ? globalValues.scenarioCustomizations : null) ?? GameControl.control.scenarioCustomizationsStartup;
			}
		}

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x06003BE6 RID: 15334 RVA: 0x00169055 File Offset: 0x00167255
		// (set) Token: 0x06003BE7 RID: 15335 RVA: 0x0016905D File Offset: 0x0016725D
		public string campaignStartVersion { get; private set; }

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x06003BE8 RID: 15336 RVA: 0x00169066 File Offset: 0x00167266
		// (set) Token: 0x06003BE9 RID: 15337 RVA: 0x0016906E File Offset: 0x0016726E
		public string latestSaveVersion { get; private set; }

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x06003BEA RID: 15338 RVA: 0x00169077 File Offset: 0x00167277
		// (set) Token: 0x06003BEB RID: 15339 RVA: 0x0016907F File Offset: 0x0016727F
		public TIDateTime realWorldCampaignStart { get; private set; }

		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x06003BEC RID: 15340 RVA: 0x00169088 File Offset: 0x00167288
		// (set) Token: 0x06003BED RID: 15341 RVA: 0x001690A7 File Offset: 0x001672A7
		public static float BaselineUnnormalizedSpaceCombatValue
		{
			get
			{
				if (TIGlobalValuesState.GlobalValues != null)
				{
					return TIGlobalValuesState.GlobalValues.baselineUnnormalizedSpaceCombatValue;
				}
				return TIGlobalValuesState.baselineUnnormalizedSpaceCombatValue_Static;
			}
			set
			{
				if (TIGlobalValuesState.GlobalValues != null)
				{
					TIGlobalValuesState.GlobalValues.baselineUnnormalizedSpaceCombatValue = value;
					return;
				}
				TIGlobalValuesState.baselineUnnormalizedSpaceCombatValue_Static = value;
			}
		}

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x06003BEE RID: 15342 RVA: 0x001690C8 File Offset: 0x001672C8
		// (set) Token: 0x06003BEF RID: 15343 RVA: 0x001690D0 File Offset: 0x001672D0
		[fsIgnore]
		public float averageRegionPopulation { get; private set; }

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x06003BF0 RID: 15344 RVA: 0x001690D9 File Offset: 0x001672D9
		// (set) Token: 0x06003BF1 RID: 15345 RVA: 0x001690E1 File Offset: 0x001672E1
		[fsIgnore]
		public float medianRegionArea_km2 { get; private set; }

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x06003BF2 RID: 15346 RVA: 0x001690EA File Offset: 0x001672EA
		// (set) Token: 0x06003BF3 RID: 15347 RVA: 0x001690F2 File Offset: 0x001672F2
		public Dictionary<FactionResource, float> maxGlobalExpectedHabSiteProduction_day { get; private set; }

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06003BF4 RID: 15348 RVA: 0x001690FB File Offset: 0x001672FB
		// (set) Token: 0x06003BF5 RID: 15349 RVA: 0x00169103 File Offset: 0x00167303
		[fsIgnore]
		public float maxSolar { get; private set; }

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06003BF6 RID: 15350 RVA: 0x0016910C File Offset: 0x0016730C
		public static double globalGDP
		{
			get
			{
				if (TIGlobalValuesState.cachedGlobalGDP <= 0.0)
				{
					TIGlobalValuesState.cachedGlobalGDP = GameStateManager.AllExtantNations().Sum<TINationState>((TINationState nationState) => nationState.GDP);
				}
				return TIGlobalValuesState.cachedGlobalGDP;
			}
		}

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x06003BF7 RID: 15351 RVA: 0x0016915C File Offset: 0x0016735C
		public static double globalGDP_CampaignStart
		{
			get
			{
				if (TIGlobalValuesState.cachedGlobalGDP_CampaignStart <= 0.0)
				{
					Dictionary<TIRegionTemplate, TINationTemplate> dictionary = (from x in TemplateManager.GetAllTemplates<TIBilateralTemplate>(true)
						where x.relationType == BilateralRelationType.Claim
						where x.initialOwner
						where x.BilateralIsActive()
						select x).ToDictionary<TIBilateralTemplate, TIRegionTemplate, TINationTemplate>((TIBilateralTemplate x) => x.regionState1.template, (TIBilateralTemplate x) => x.nationState1.template);
					float gdpScaling = GameStateManager.Time().template.globalStartingGDPScaling;
					Dictionary<TINationTemplate, double> perCapitaGDPs = (from x in dictionary
						group x by x.Value).ToDictionary<IGrouping<TINationTemplate, KeyValuePair<TIRegionTemplate, TINationTemplate>>, TINationTemplate, double>((IGrouping<TINationTemplate, KeyValuePair<TIRegionTemplate, TINationTemplate>> x) => x.Key, (IGrouping<TINationTemplate, KeyValuePair<TIRegionTemplate, TINationTemplate>> x) => (double)gdpScaling * x.Key.initialGDP.GetValueOrDefault() / (double)x.Sum<KeyValuePair<TIRegionTemplate, TINationTemplate>>((KeyValuePair<TIRegionTemplate, TINationTemplate> y) => y.Key.population_Millions));
					TIGlobalValuesState.cachedGlobalGDP_CampaignStart = dictionary.Sum<KeyValuePair<TIRegionTemplate, TINationTemplate>>((KeyValuePair<TIRegionTemplate, TINationTemplate> x) => (double)x.Key.population_Millions * perCapitaGDPs[x.Value]);
				}
				return TIGlobalValuesState.cachedGlobalGDP_CampaignStart;
			}
		}

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x06003BF8 RID: 15352 RVA: 0x001692C4 File Offset: 0x001674C4
		public static float globalResearch
		{
			get
			{
				if (TIGlobalValuesState.cachedGlobalResearch <= 0f)
				{
					TIGlobalValuesState.cachedGlobalResearch = GameStateManager.AllExtantNations().Sum<TINationState>((TINationState x) => x.research_month);
				}
				return TIGlobalValuesState.cachedGlobalResearch;
			}
		}

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x06003BF9 RID: 15353 RVA: 0x00169310 File Offset: 0x00167510
		public static float globalGDPFractionOfBaseline
		{
			get
			{
				return (float)(TIGlobalValuesState.globalGDP / 159600000000000.0);
			}
		}

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x06003BFA RID: 15354 RVA: 0x00169322 File Offset: 0x00167522
		public static float globalResearchFractionOfBaseline
		{
			get
			{
				return TIGlobalValuesState.globalResearch / 6300f;
			}
		}

		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x06003BFB RID: 15355 RVA: 0x00169330 File Offset: 0x00167530
		public float pcgdpToReduceUnrestBy1
		{
			get
			{
				if (this.fixedPCGDPToReduceUnrestBy1 <= 0f)
				{
					this.fixedPCGDPToReduceUnrestBy1 = (float)(TIGlobalValuesState.globalGDP_CampaignStart * 6.26E-11);
				}
				return this.fixedPCGDPToReduceUnrestBy1;
			}
		}

		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x06003BFC RID: 15356 RVA: 0x0016935C File Offset: 0x0016755C
		public float pcgdpToRaiseMissionBaseDifficultyBy1
		{
			get
			{
				if (!GameStateManager.Time().template.scaleEconomyDefenseWithStartingGDP)
				{
					return 1E+09f;
				}
				if (this.fixedPCGDPToRaiseMissionBaseDifficultyBy1 <= 0f)
				{
					this.fixedPCGDPToRaiseMissionBaseDifficultyBy1 = (float)(TIGlobalValuesState.globalGDP_CampaignStart * 6.26E-06);
				}
				return this.fixedPCGDPToRaiseMissionBaseDifficultyBy1;
			}
		}

		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x06003BFD RID: 15357 RVA: 0x001693AC File Offset: 0x001675AC
		public float pcgdpToRaiseBaseCPMaintenanceCostBy1
		{
			get
			{
				if (!GameStateManager.Time().template.scaleCPMaintenanceWithStartingGDP)
				{
					return 1E+09f;
				}
				if (this.fixedPCGDPToRaiseBaseCPMaintenanceCostBy1 <= 0f)
				{
					this.fixedPCGDPToRaiseBaseCPMaintenanceCostBy1 = (float)(TIGlobalValuesState.globalGDP_CampaignStart * 6.26E-06);
				}
				return this.fixedPCGDPToRaiseBaseCPMaintenanceCostBy1;
			}
		}

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x06003BFE RID: 15358 RVA: 0x001693F9 File Offset: 0x001675F9
		public static float PCGDPToReduceUnrestBy1
		{
			get
			{
				return TIGlobalValuesState.GlobalValues.pcgdpToReduceUnrestBy1;
			}
		}

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x06003BFF RID: 15359 RVA: 0x00169405 File Offset: 0x00167605
		public static float PCGDPToRaiseMissionBaseDifficultyBy1
		{
			get
			{
				return TIGlobalValuesState.GlobalValues.pcgdpToRaiseMissionBaseDifficultyBy1;
			}
		}

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x06003C00 RID: 15360 RVA: 0x00169411 File Offset: 0x00167611
		public static float PCGDPToRaiseBaseCPMaintenanceCostBy1
		{
			get
			{
				return TIGlobalValuesState.GlobalValues.pcgdpToRaiseBaseCPMaintenanceCostBy1;
			}
		}

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x06003C01 RID: 15361 RVA: 0x0016941D File Offset: 0x0016761D
		public static TIGlobalValuesState GlobalValues
		{
			get
			{
				return GameStateManager.GlobalValues();
			}
		}

		// Token: 0x06003C02 RID: 15362 RVA: 0x00169424 File Offset: 0x00167624
		public override bool Initialize()
		{
			if (this.interstateWars == null)
			{
				this.interstateWars = new List<TIWarState>();
			}
			if (this.scenarioCustomizations == null)
			{
				this.scenarioCustomizations = GameControl.control.scenarioCustomizationsStartup.Clone();
			}
			return base.Initialize();
		}

		// Token: 0x06003C03 RID: 15363 RVA: 0x0016945C File Offset: 0x0016765C
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			this.SaveStartSettings();
		}

		// Token: 0x06003C04 RID: 15364 RVA: 0x00169464 File Offset: 0x00167664
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.timeState = GameStateManager.Time();
			if (this.scenarioCustomizations == null)
			{
				this.scenarioCustomizations = new ScenarioCustomizations
				{
					usingCustomizations = false
				};
			}
			if (this.removedNarrativeEvents == null)
			{
				this.removedNarrativeEvents = new List<string>();
			}
			if (!this.gameStateSubjectCreated)
			{
				this.campaignStartVersion = Application.version;
				this.realWorldCampaignStart = new TIDateTime(DateTime.Now);
				this.moddingActive = TIPlayerProfileManager.useMods;
				this.moddingUsedAnytime = TIPlayerProfileManager.useMods;
				TINationState.SetAllBilaterals();
				this.resourceMarketValues = new Dictionary<FactionResource, float>
				{
					{
						FactionResource.Water,
						TemplateManager.global.initialWaterValue
					},
					{
						FactionResource.Volatiles,
						TemplateManager.global.initialVolatilesValue
					},
					{
						FactionResource.Metals,
						TemplateManager.global.initialMetalsValue
					},
					{
						FactionResource.NobleMetals,
						TemplateManager.global.initialNobleMetalsValue
					},
					{
						FactionResource.Fissiles,
						TemplateManager.global.initialFissilesValue
					},
					{
						FactionResource.Antimatter,
						TemplateManager.global.initialAntimatterValue
					},
					{
						FactionResource.Exotics,
						TemplateManager.global.initialExoticsValue
					}
				};
				this.inactiveNarrativeEvents = new List<string>();
				this.narrativeEvents = new Dictionary<string, float>();
				this.narrativeEventsOnCooldown_months = new Dictionary<string, float>();
				this.triggeredOncePerTargetEvents = new Dictionary<string, List<TIFactionState>>();
				this.priorNarrativeEventData = new Dictionary<string, List<PriorNarrativeEventData>>();
				foreach (TINarrativeEventTemplate tinarrativeEventTemplate in TemplateManager.IterateByClass<TINarrativeEventTemplate>(true))
				{
					if (!tinarrativeEventTemplate.reqEventUnlock)
					{
						this.inactiveNarrativeEvents.Add(tinarrativeEventTemplate.dataName);
					}
					if (tinarrativeEventTemplate.repeatable == RepeatableStatus.OncePerFaction)
					{
						this.triggeredOncePerTargetEvents.Add(tinarrativeEventTemplate.dataName, new List<TIFactionState>());
					}
				}
				TIStartTimeTemplate tistartTimeTemplate = this.timeState.GetMyTemplate() as TIStartTimeTemplate;
				this.earthAtmosphericCO2_ppm = tistartTimeTemplate.initialAtmosphericCO2_ppm;
				this.earthAtmosphericCH4_ppm = tistartTimeTemplate.initialAtmosphericCH4_ppm;
				this.earthAtmosphericN2O_ppm = tistartTimeTemplate.initialAtmosphericN2O_ppm;
				this.stratosphericAerosols_ppm = tistartTimeTemplate.initialStratosphericAerosols_ppm;
				this.globalSeaLevelAnomaly_cm = tistartTimeTemplate.initialGlobalSeaLevelAnomaly_cm;
				float num = GameStateManager.AllHumanNations().Min<TINationState>((TINationState x) => x.template.greenEconomy);
				this.initialSustainabilityMin = Mathf.Max(num * 0.95f, 1f / (1f / num + 0.05f));
				if (this.globalSeaLevelAnomaly_cm >= 61f)
				{
					this.globalSeaLevelRise1Triggered = true;
					if (this.globalSeaLevelAnomaly_cm >= 305f)
					{
						this.globalSeaLevelRise2Triggered = true;
					}
				}
				this.looseNukes = tistartTimeTemplate.initialLooseNukes.GetValueOrDefault();
				if (this.scenarioCustomizations.usingCustomizations)
				{
					this.controlPointMaintenanceFreebies = this.scenarioCustomizations.controlPointMaintenanceFreebieBonus;
					if (this.scenarioCustomizations.customFactionStartingNationGroup.Count > 0)
					{
						IEnumerable<TINationTemplate> enumerable = from x in GameStateManager.AllNations()
							select x.template;
						int num2 = 0;
						int i = 1;
						for (;;)
						{
							if (i >= enumerable.Max<TINationTemplate>((TINationTemplate x) => x.group) + 1)
							{
								break;
							}
							int num3 = (int)(from o in GameStateManager.AllExtantNations()
								where o.template.@group == i
								select o).Sum<TINationState>((TINationState o) => o.ControlPointMaintenanceCost * (float)o.numControlPoints);
							if (num3 > num2)
							{
								num2 = num3;
							}
							int j = i;
							i = j + 1;
						}
						this.controlPointMaintenanceFreebies += num2;
					}
				}
				else
				{
					this.controlPointMaintenanceFreebies = TemplateManager.global.controlPointMaintenanceFreebies;
					if (GameStateManager.AllHumanFactions().Length < 7)
					{
						this.controlPointMaintenanceFreebies += (7 - GameStateManager.AllHumanFactions().Length) * TemplateManager.global.controlPointBonusMaintenanceFreebiesPerRemovedFaction;
					}
				}
				this.bestGlobalHumanMiltech = GameStateManager.AllHumanNations().Max<TINationState>((TINationState x) => x.militaryTechLevel);
				this.bestGlobalHumanEducation = GameStateManager.AllHumanNations().Max<TINationState>((TINationState x) => x.education);
				this.repairCPMaintenanceScaling = false;
			}
			else
			{
				base.DeArchiveState();
				this.moddingActive = TIPlayerProfileManager.useMods;
				foreach (TINarrativeEventTemplate tinarrativeEventTemplate2 in TemplateManager.IterateByClass<TINarrativeEventTemplate>(true))
				{
					if (tinarrativeEventTemplate2.repeatable == RepeatableStatus.OncePerFaction && !this.triggeredOncePerTargetEvents.ContainsKey(tinarrativeEventTemplate2.dataName))
					{
						this.triggeredOncePerTargetEvents.Add(tinarrativeEventTemplate2.dataName, new List<TIFactionState>());
					}
				}
				foreach (string text in this.narrativeEvents.Keys.ToList<string>())
				{
					if (this.NarrativeEventTemplate(text) == null)
					{
						this.narrativeEvents.Remove(text);
					}
				}
				foreach (string text2 in this.narrativeEventsOnCooldown_months.Keys.ToList<string>())
				{
					if (this.NarrativeEventTemplate(text2) == null)
					{
						this.narrativeEventsOnCooldown_months.Remove(text2);
					}
				}
				foreach (string text3 in this.narrativeEventsTargetSpecificCooldowns.Keys.ToList<string>())
				{
					if (this.NarrativeEventTemplate(text3) == null)
					{
						this.narrativeEventsTargetSpecificCooldowns.Remove(text3);
					}
				}
				foreach (string text4 in this.triggeredOncePerTargetEvents.Keys.ToList<string>())
				{
					if (this.NarrativeEventTemplate(text4) == null)
					{
						this.triggeredOncePerTargetEvents.Remove(text4);
					}
				}
				foreach (string text5 in this.priorNarrativeEventData.Keys.ToList<string>())
				{
					if (this.NarrativeEventTemplate(text5) == null)
					{
						this.priorNarrativeEventData.Remove(text5);
					}
				}
				if (this.latestSaveVersion == "0.4.1" || this.latestSaveVersion == "0.4.2" || this.latestSaveVersion == "0.4.3" || this.latestSaveVersion == "0.4.4")
				{
					foreach (string text6 in GameStateManager.GlobalResearch().FinishedTechDataNames)
					{
						foreach (TIEffectTemplate tieffectTemplate in TemplateManager.Find<TITechTemplate>(text6, false).Effects)
						{
							if (tieffectTemplate.contexts.Contains(Context.MCFreeSpaceMineNetwork))
							{
								TIEffectsState.AddEffect(tieffectTemplate, GameControl.control.activePlayer, null, null, "");
								break;
							}
						}
					}
				}
				foreach (TIProjectTemplate tiprojectTemplate in TemplateManager.IterateByClass<TIProjectTemplate>(true))
				{
					List<TIBilateralTemplate> associatedBilaterals = tiprojectTemplate.associatedBilaterals;
					List<TIBilateralTemplate> associatedClaims = tiprojectTemplate.associatedClaims;
				}
				if (this.latestSaveVersion == "0.4.90a")
				{
					TIGlobalValuesState.Customizations.cinematicCombatRealismDV = true;
					TIGlobalValuesState.Customizations.cinematicCombatRealismScale = true;
				}
				if (this.repairCPMaintenanceScaling && GameControl.control.scenarioTemplate.dataName == "ModernScenario")
				{
					this.fixedPCGDPToRaiseBaseCPMaintenanceCostBy1 = 1E+09f;
					this.repairCPMaintenanceScaling = false;
				}
			}
			if (this.altWeightConditionTriggered == null)
			{
				this.altWeightConditionTriggered = new List<string>();
			}
			if (this.pendingNarrativeEvents == null)
			{
				this.pendingNarrativeEvents = new List<PendingNarrativeEvent>();
			}
			if (this.triggeredOncePerCampaignEvents == null)
			{
				this.triggeredOncePerCampaignEvents = new List<string>();
			}
			if (this.currentNuclearExchanges == null)
			{
				this.currentNuclearExchanges = new List<NuclearExchange>();
			}
			if (this.narrativeEventsTargetSpecificCooldowns == null)
			{
				this.narrativeEventsTargetSpecificCooldowns = new Dictionary<string, List<EventStateCooldownData>>();
			}
			if (this.globalMilestones == null)
			{
				this.globalMilestones = new Dictionary<GlobalMilestone, TIFactionState>();
			}
			float num4 = -1f * TemplateManager.IterateByClass<TITechTemplate>(false).SelectMany<TITechTemplate, TIEffectTemplate>((TITechTemplate x) => x.Effects.Where<TIEffectTemplate>((TIEffectTemplate x) => x.contexts.Contains(Context.Environment_BestSustainabilityValue))).Sum<TIEffectTemplate>((TIEffectTemplate x) => x.value);
			float num5 = GameStateManager.AllHumanNations().Min<TINationState>((TINationState x) => x.template.greenEconomy);
			if (num4 > num5)
			{
				this.sustainabilityHelperModifier = num5 / num4;
			}
			else
			{
				this.sustainabilityHelperModifier = 1f;
			}
			if (this.CO2SourcesRecord_ppm == null)
			{
				GHGSources[] array = (GHGSources[])Enum.GetValues(typeof(GHGSources));
				this.CO2SourcesRecord_ppm = array.ToDictionary<GHGSources, GHGSources, double>((GHGSources x) => x, (GHGSources x) => 0.0);
				this.CH4SourcesRecord_ppm = array.ToDictionary<GHGSources, GHGSources, double>((GHGSources x) => x, (GHGSources x) => 0.0);
				this.N2OSourcesRecord_ppm = array.ToDictionary<GHGSources, GHGSources, double>((GHGSources x) => x, (GHGSources x) => 0.0);
			}
			this.ideologyTemplateLookup = GameStateManager.ActiveIdeologies().ToDictionary<TIFactionIdeologyTemplate, FactionIdeology, TIFactionIdeologyTemplate>((TIFactionIdeologyTemplate x) => x.ideology, (TIFactionIdeologyTemplate x) => x);
			this.latestSaveVersion = Application.version;
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.promptQueue = GameStateManager.FindGameState<TIPromptQueueState>();
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.TriggerNarrativeEvent), "NarrativeEvent", null, true, false);
			this.SetIdeologyDistanceGrid();
			this.medianRegionArea_km2 = Utilities.Median(from x in GameStateManager.AllRegions()
				select x.area_km2, false);
			this.averageRegionPopulation = GameStateManager.AllRegions().Average<TIRegionState>((TIRegionState x) => x.template.population_Millions);
			if (this.maxGlobalExpectedHabSiteProduction_day == null)
			{
				this.maxGlobalExpectedHabSiteProduction_day = new Dictionary<FactionResource, float>();
				using (HashSet<FactionResource>.Enumerator enumerator5 = TIResourcesCost.basicSpaceResources.GetEnumerator())
				{
					while (enumerator5.MoveNext())
					{
						FactionResource resource = enumerator5.Current;
						this.maxGlobalExpectedHabSiteProduction_day.Add(resource, GameStateManager.IterateByClass<TIHabSiteState>(false).Max<TIHabSiteState>((TIHabSiteState x) => x.GetHabSiteExpectedProductivity_day(resource)));
					}
				}
			}
			this.councilorAppearanceTemplatesInUse = (from x in GameStateManager.IterateByClass<TICouncilorState>(false)
				where !x.archived && x.appearanceTemplate != null
				select x.appearanceTemplateName).ToList<string>();
			this.SuezRegion = GameStateManager.AllRegions().FirstOrDefault<TIRegionState>((TIRegionState x) => x.mapRegionTemplateName == TemplateManager.global.SuezCanalRegion);
			this.PanamaRegion = GameStateManager.AllRegions().FirstOrDefault<TIRegionState>((TIRegionState x) => x.mapRegionTemplateName == TemplateManager.global.PanamaCanalRegion);
			this.TurkishStraitRegion = GameStateManager.AllRegions().FirstOrDefault<TIRegionState>((TIRegionState x) => x.mapRegionTemplateName == TemplateManager.global.TurkishStraitsRegion);
			Log.Info("Campaign Start Version: " + this.campaignStartVersion + " Current Version: " + Application.version, Array.Empty<object>());
		}

		// Token: 0x06003C05 RID: 15365 RVA: 0x0016A144 File Offset: 0x00168344
		public override void PostCanvasManagerCreateInit_3()
		{
			float num = GameStateManager.IterateByClass<TIOrbitState>(false).ToList<TIOrbitState>().Max<TIOrbitState>((TIOrbitState x) => x.solarMultiplier);
			float num2 = GameStateManager.IterateByClass<TISpaceBodyState>(false).ToList<TISpaceBodyState>().Max<TISpaceBodyState>((TISpaceBodyState x) => x.solarMultiplier);
			float num3 = GameStateManager.IterateByClass<TIHabSiteState>(false).ToList<TIHabSiteState>().Max<TIHabSiteState>((TIHabSiteState x) => x.solarMultiplier);
			this.maxSolar = Mathf.Max(new float[] { num, num2, num3 });
		}

		// Token: 0x06003C06 RID: 15366 RVA: 0x0016A1FE File Offset: 0x001683FE
		public override void PostVisualizerCreationInit_7()
		{
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x06003C07 RID: 15367 RVA: 0x0016A208 File Offset: 0x00168408
		public void SetIdeologyDistanceGrid()
		{
			foreach (FactionIdeology factionIdeology in (from x in GameStateManager.ActiveIdeologies()
				select x.ideology).ToList<FactionIdeology>())
			{
				this.ideologyDistanceGrid.Add(factionIdeology, new Dictionary<FactionIdeology, float>());
				foreach (FactionIdeology factionIdeology2 in (from x in GameStateManager.ActiveIdeologies()
					select x.ideology).ToList<FactionIdeology>())
				{
					this.ideologyDistanceGrid[factionIdeology].Add(factionIdeology2, TINationState.GetIdeologicalDistance(TIFactionIdeologyTemplate.GetIdeologyTemplate(factionIdeology).ideologyCoordinates, TIFactionIdeologyTemplate.GetIdeologyTemplate(factionIdeology2).ideologyCoordinates));
				}
			}
			float num = -1f;
			TIFactionIdeologyTemplate tifactionIdeologyTemplate = GameStateManager.AllHumanFactions().MinBy<TIFactionState, float>((TIFactionState x) => x.ideology.ideologyCoordinates.x).ideology;
			TIFactionIdeologyTemplate tifactionIdeologyTemplate2 = GameStateManager.AllHumanFactions().MaxBy<TIFactionState, float>((TIFactionState x) => x.ideology.ideologyCoordinates.x).ideology;
			foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate3 in GameStateManager.ActiveHumanIdeologies())
			{
				foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate4 in GameStateManager.ActiveHumanIdeologies())
				{
					float num2 = this.ideologyDistanceGrid[tifactionIdeologyTemplate3.ideology][tifactionIdeologyTemplate4.ideology];
					if (num2 > num)
					{
						tifactionIdeologyTemplate = tifactionIdeologyTemplate3;
						tifactionIdeologyTemplate2 = tifactionIdeologyTemplate4;
						num = num2;
					}
				}
			}
			Vector3 vector = new Vector3
			{
				x = (tifactionIdeologyTemplate.ideologyCoordinates.x + tifactionIdeologyTemplate2.ideologyCoordinates.x) / 2f,
				y = (tifactionIdeologyTemplate.ideologyCoordinates.y + tifactionIdeologyTemplate2.ideologyCoordinates.y) / 2f,
				z = (tifactionIdeologyTemplate.ideologyCoordinates.z + tifactionIdeologyTemplate2.ideologyCoordinates.z) / 2f
			};
			float num3 = 0f;
			num3 += (tifactionIdeologyTemplate.ideologyCoordinates - vector).sqrMagnitude * 0.5f;
			num3 += (tifactionIdeologyTemplate2.ideologyCoordinates - vector).sqrMagnitude * 0.5f;
			this.worstCasePublicOpinionDispersal = Mathf.Sqrt(num3);
		}

		// Token: 0x06003C08 RID: 15368 RVA: 0x0016A504 File Offset: 0x00168704
		public void ModifyMarketValuesForResourceSale(Dictionary<FactionResource, int> resourcesSold)
		{
			foreach (FactionResource factionResource in resourcesSold.Keys)
			{
				this.resourceMarketValues[factionResource] = Mathf.Max(0.001f, this.resourceMarketValues[factionResource] * (1f - TIUtilities.RandomRange(1E-06f, 2E-06f) * (float)resourcesSold[factionResource]));
			}
		}

		// Token: 0x06003C09 RID: 15369 RVA: 0x0016A594 File Offset: 0x00168794
		public void ModifyMarketValuesForEconomyPriority()
		{
			Dictionary<FactionResource, float> dictionary = this.resourceMarketValues;
			dictionary[FactionResource.Metals] = dictionary[FactionResource.Metals] * (1f + TIUtilities.RandomRange(1E-05f, 2E-05f));
			dictionary = this.resourceMarketValues;
			dictionary[FactionResource.NobleMetals] = dictionary[FactionResource.NobleMetals] * (1f + TIUtilities.RandomRange(5E-06f, 1E-05f));
		}

		// Token: 0x06003C0A RID: 15370 RVA: 0x0016A5FC File Offset: 0x001687FC
		public void ModifyMarketValuesForArmyPriority()
		{
			Dictionary<FactionResource, float> dictionary = this.resourceMarketValues;
			dictionary[FactionResource.Metals] = dictionary[FactionResource.Metals] * (1f + TIUtilities.RandomRange(0.0001f, 0.0002f));
			dictionary = this.resourceMarketValues;
			dictionary[FactionResource.NobleMetals] = dictionary[FactionResource.NobleMetals] * (1f + TIUtilities.RandomRange(5E-05f, 0.0001f));
		}

		// Token: 0x06003C0B RID: 15371 RVA: 0x0016A664 File Offset: 0x00168864
		public void ModifyMarketValuesForMilitaryPriority()
		{
			Dictionary<FactionResource, float> dictionary = this.resourceMarketValues;
			dictionary[FactionResource.Metals] = dictionary[FactionResource.Metals] * (1f + TIUtilities.RandomRange(1E-05f, 2E-05f));
			dictionary = this.resourceMarketValues;
			dictionary[FactionResource.NobleMetals] = dictionary[FactionResource.NobleMetals] * (1f + TIUtilities.RandomRange(5E-06f, 1E-05f));
		}

		// Token: 0x06003C0C RID: 15372 RVA: 0x0016A6CC File Offset: 0x001688CC
		public void ModifyMarketValuesForNuclearWeaponsPriority()
		{
			Dictionary<FactionResource, float> dictionary = this.resourceMarketValues;
			dictionary[FactionResource.Fissiles] = dictionary[FactionResource.Fissiles] * (1f + TIUtilities.RandomRange(1E-05f, 6E-05f));
		}

		// Token: 0x06003C0D RID: 15373 RVA: 0x0016A708 File Offset: 0x00168908
		public void ModifyMarketValuesForRecession(int power)
		{
			Dictionary<FactionResource, float> dictionary = this.resourceMarketValues;
			dictionary[FactionResource.Metals] = dictionary[FactionResource.Metals] * (1f - TIUtilities.RandomRange(0.0001f, 0.0002f) * (float)power * (float)power);
			dictionary = this.resourceMarketValues;
			dictionary[FactionResource.NobleMetals] = dictionary[FactionResource.NobleMetals] * (1f - TIUtilities.RandomRange(5E-05f, 0.0001f) * (float)power * (float)power);
		}

		// Token: 0x06003C0E RID: 15374 RVA: 0x0016A77B File Offset: 0x0016897B
		public float GetPurchaseResourceMarketValue(FactionResource resource)
		{
			if (this.resourceMarketValues.ContainsKey(resource))
			{
				return this.resourceMarketValues[resource];
			}
			return 0f;
		}

		// Token: 0x06003C0F RID: 15375 RVA: 0x0016A7A0 File Offset: 0x001689A0
		public float GetModifiedResourceMarketValueForSelling(TIFactionState faction, FactionResource resource)
		{
			if (this.resourceMarketValues.ContainsKey(resource))
			{
				return this.resourceMarketValues[resource] * Mathf.Min(0.6666667f, TemplateManager.global.baseEarthSaleInefficiency * (1f + TIEffectsState.SumEffectsModifiers(Context.ResourceMarketSales, faction, 0f, null)));
			}
			return 0f;
		}

		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x06003C10 RID: 15376 RVA: 0x0016A7FA File Offset: 0x001689FA
		public float temperatureAnomalyCO2_C
		{
			get
			{
				return Mathf.Max(0f, (this.earthAtmosphericCO2_ppm - 325.68f) / 94.5f);
			}
		}

		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x06003C11 RID: 15377 RVA: 0x0016A818 File Offset: 0x00168A18
		public float temperatureAnomalyCH4_C
		{
			get
			{
				return Mathf.Max(0f, (this.earthAtmosphericCH4_ppm - 1.3f) * 21f / 94.5f);
			}
		}

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06003C12 RID: 15378 RVA: 0x0016A83C File Offset: 0x00168A3C
		public float temperatureAnomalyN2O_C
		{
			get
			{
				return Mathf.Max(0f, (this.earthAtmosphericN2O_ppm - 0.29f) * 289f / 94.5f);
			}
		}

		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06003C13 RID: 15379 RVA: 0x0016A860 File Offset: 0x00168A60
		public float temperatureAnomalyStratosphericAerosols_C
		{
			get
			{
				return Mathf.Max(-40f, -1f * (this.stratosphericAerosols_ppm / 0.03885f));
			}
		}

		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x06003C14 RID: 15380 RVA: 0x0016A87E File Offset: 0x00168A7E
		public float temperatureAnomaly_C
		{
			get
			{
				return this.temperatureAnomalyCO2_C + this.temperatureAnomalyCH4_C + this.temperatureAnomalyN2O_C + this.temperatureAnomalyStratosphericAerosols_C;
			}
		}

		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x06003C15 RID: 15381 RVA: 0x0016A89B File Offset: 0x00168A9B
		public float temperatureAnomaly_F
		{
			get
			{
				return this.temperatureAnomaly_C * 1.8f;
			}
		}

		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x06003C16 RID: 15382 RVA: 0x0016A8AC File Offset: 0x00168AAC
		public float temperatureAnomaly_C_startTime
		{
			get
			{
				TIStartTimeTemplate template = GameStateManager.Time().template;
				float num = Mathf.Max(0f, (template.initialAtmosphericCO2_ppm - 325.68f) / 94.5f);
				float num2 = Mathf.Max(0f, (template.initialAtmosphericCH4_ppm - 1.3f) * 21f / 94.5f);
				float num3 = Mathf.Max(0f, (template.initialAtmosphericN2O_ppm - 0.29f) * 289f / 94.5f);
				float num4 = Mathf.Max(-40f, -1f * (template.initialStratosphericAerosols_ppm / 0.03885f));
				return num + num2 + num3 + num4;
			}
		}

		// Token: 0x06003C17 RID: 15383 RVA: 0x0016A94C File Offset: 0x00168B4C
		public void AddCO2_ppm(float amount, GHGSources source)
		{
			this.earthAtmosphericCO2_ppm += amount;
			this.earthAtmosphericCO2_ppm = Mathf.Max(this.earthAtmosphericCO2_ppm, 280f);
			Dictionary<GHGSources, double> co2SourcesRecord_ppm = this.CO2SourcesRecord_ppm;
			co2SourcesRecord_ppm[source] += (double)amount;
		}

		// Token: 0x06003C18 RID: 15384 RVA: 0x0016A998 File Offset: 0x00168B98
		public void AddCH4_ppm(float amount, GHGSources source)
		{
			this.earthAtmosphericCH4_ppm += amount;
			this.earthAtmosphericCH4_ppm = Mathf.Max(this.earthAtmosphericCH4_ppm, 0.7f);
			Dictionary<GHGSources, double> ch4SourcesRecord_ppm = this.CH4SourcesRecord_ppm;
			ch4SourcesRecord_ppm[source] += (double)amount;
		}

		// Token: 0x06003C19 RID: 15385 RVA: 0x0016A9E4 File Offset: 0x00168BE4
		public void AddN2O_ppm(float amount, GHGSources source)
		{
			this.earthAtmosphericN2O_ppm += amount;
			this.earthAtmosphericN2O_ppm = Mathf.Max(this.earthAtmosphericN2O_ppm, 0.27f);
			Dictionary<GHGSources, double> n2OSourcesRecord_ppm = this.N2OSourcesRecord_ppm;
			n2OSourcesRecord_ppm[source] += (double)amount;
		}

		// Token: 0x06003C1A RID: 15386 RVA: 0x0016AA30 File Offset: 0x00168C30
		public void AddStratosphericAerosols_ppm(float amount, bool causedByNuke)
		{
			float stratosphericAerosols_ppm = this.stratosphericAerosols_ppm;
			this.stratosphericAerosols_ppm += amount;
			this.stratosphericAerosols_ppm = Mathf.Max(this.stratosphericAerosols_ppm, 0f);
			if (stratosphericAerosols_ppm < 0.01f && this.stratosphericAerosols_ppm >= 0.01f)
			{
				GameControl.eventManager.TriggerEvent(new EarthParticulateThresholdChanges(1), null, Array.Empty<object>());
				if (causedByNuke)
				{
					TIFactionState activePlayer = GameControl.control.activePlayer;
					if (activePlayer == null)
					{
						return;
					}
					activePlayer.UnlockAchievement("nuclearWinter");
					return;
				}
			}
			else if (stratosphericAerosols_ppm >= 0.01f && this.stratosphericAerosols_ppm < 0.01f)
			{
				GameControl.eventManager.TriggerEvent(new EarthParticulateThresholdChanges(0), null, Array.Empty<object>());
			}
		}

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x06003C1B RID: 15387 RVA: 0x0016AADB File Offset: 0x00168CDB
		public float globalSeaLevelAnomaly_m
		{
			get
			{
				return this.globalSeaLevelAnomaly_cm / 1000f;
			}
		}

		// Token: 0x06003C1C RID: 15388 RVA: 0x0016AAEC File Offset: 0x00168CEC
		public void AddToSeaLevel_cm(float amount)
		{
			this.globalSeaLevelAnomaly_cm += amount;
			if (this.globalSeaLevelAnomaly_cm >= 61f && !this.globalSeaLevelRise1Triggered)
			{
				this.globalSeaLevelRise1Triggered = true;
				GameStateManager.Earth().SetModelResource();
			}
			if (this.globalSeaLevelAnomaly_cm >= 305f && !this.globalSeaLevelRise2Triggered)
			{
				this.globalSeaLevelRise2Triggered = true;
				GameStateManager.Earth().SetModelResource();
				TIFactionState activePlayer = GameControl.control.activePlayer;
				if (activePlayer == null)
				{
					return;
				}
				activePlayer.UnlockAchievement("seaLevelRise");
			}
		}

		// Token: 0x06003C1D RID: 15389 RVA: 0x0016AB6C File Offset: 0x00168D6C
		public void AddSpoilsPriorityEnvEffect(TINationState nation, float scaling)
		{
			float num = nation.economyScore / 100f;
			this.AddCO2_ppm(scaling * num * (TemplateManager.global.SpoCO2_ppm + TemplateManager.global.SpoResCO2_ppm * (float)nation.resourceRegions), GHGSources.SpoilsPriority);
			this.AddCH4_ppm(scaling * num * (TemplateManager.global.SpoCH4_ppm + TemplateManager.global.SpoResCH4_ppm * (float)nation.resourceRegions), GHGSources.SpoilsPriority);
			this.AddN2O_ppm(scaling * num * (TemplateManager.global.SpoN2O_ppm + TemplateManager.global.SpoResN2O_ppm * (float)nation.resourceRegions), GHGSources.SpoilsPriority);
		}

		// Token: 0x06003C1E RID: 15390 RVA: 0x0016ABFE File Offset: 0x00168DFE
		public void AddEnvironmentPriorityEnvEffect(TINationState nation)
		{
			this.AddCO2_ppm(nation.EnvPriorityCO2Removed(), GHGSources.EnvironmentPriority);
			this.AddCH4_ppm(nation.EnvPriorityCH4Removed(), GHGSources.EnvironmentPriority);
			this.AddN2O_ppm(nation.EnvPriorityN2ORemoved(), GHGSources.EnvironmentPriority);
		}

		// Token: 0x06003C1F RID: 15391 RVA: 0x0016AC27 File Offset: 0x00168E27
		public void NuclearBarrageLaunched(TINationState attacker, TIRegionState target, TINationState enemy)
		{
			this.currentNuclearExchanges.Add(new NuclearExchange(attacker, target, enemy));
		}

		// Token: 0x06003C20 RID: 15392 RVA: 0x0016AC3C File Offset: 0x00168E3C
		public void TriggerNuclearDetonationEffect(bool barrage, TINationState attacker, TIRegionState region, TINationState enemy)
		{
			if (barrage)
			{
				this.nuclearStrikes++;
				foreach (NuclearExchange nuclearExchange in this.currentNuclearExchanges)
				{
					if (nuclearExchange.attacker == attacker && nuclearExchange.enemyTargeted == enemy && nuclearExchange.target == region)
					{
						this.currentNuclearExchanges.Remove(nuclearExchange);
						break;
					}
				}
				this.AddStratosphericAerosols_ppm(0.00777f * ((region.mapRegionTemplate.island && region.area_km2 < 10000f) ? (region.area_km2 / 10000f) : 1f), true);
				return;
			}
			this.AddStratosphericAerosols_ppm(7.77E-05f, true);
		}

		// Token: 0x06003C21 RID: 15393 RVA: 0x0016AD20 File Offset: 0x00168F20
		public void MonthlyGlobalEnvironmentalChanges()
		{
			float num = this.earthAtmosphericCO2_ppm * 1f / 90000f;
			this.AddCO2_ppm(-num, GHGSources.NaturalRemoval);
			float num2 = this.earthAtmosphericCH4_ppm * 1f / 144f;
			this.AddCH4_ppm(-num2, GHGSources.NaturalRemoval);
			float num3 = this.earthAtmosphericN2O_ppm * 1f / 14641f;
			this.AddN2O_ppm(-num3, GHGSources.NaturalRemoval);
			float num4 = GameStateManager.AllRegions().Average<TIRegionState>((TIRegionState x) => x.xenoforming.xenoformingLevel) / 100f;
			this.AddCO2_ppm(-num4 * 3.45f / 12f, GHGSources.Xenoforming);
			this.stratosphericAerosols_ppm = Mathf.Max(this.stratosphericAerosols_ppm * 0.935f - 0.001f, 0f);
			this.pastEarthAtmosphericCO2_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericCO2_ppm;
			this.pastEarthAtmosphericCH4_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericCH4_ppm;
			this.pastEarthAtmosphericN2O_ppm[this.gameTime.currentTime.month - 1] = this.earthAtmosphericN2O_ppm;
			float anomaly_C = this.temperatureAnomaly_C;
			if (anomaly_C > 0f)
			{
				this.AddToSeaLevel_cm(0.017f * anomaly_C);
			}
			else if (this.stratosphericAerosols_ppm == 0f)
			{
				GameControl.control.activePlayer.UnlockAchievement("temperatureAnomaly");
			}
			GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate(TINationState x)
			{
				x.ProcessMonthlyGHGsFromEconomy();
			});
			GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate(TINationState x)
			{
				x.MonthlyTemperatureEconomicImpact(anomaly_C, this.earthAtmosphericCO2_ppm);
			});
		}

		// Token: 0x06003C22 RID: 15394 RVA: 0x0016AEEB File Offset: 0x001690EB
		public void ChangeLooseNukesValue(int delta)
		{
			this.looseNukes += delta;
			if (this.looseNukes < 0)
			{
				this.looseNukes = 0;
			}
		}

		// Token: 0x06003C23 RID: 15395 RVA: 0x0016AF0B File Offset: 0x0016910B
		public void TrySetMaximumMiltech(TINationState nation, float newValue)
		{
			if (!nation.alienNation && newValue > this.bestGlobalHumanMiltech)
			{
				this.bestGlobalHumanMiltech = newValue;
			}
		}

		// Token: 0x06003C24 RID: 15396 RVA: 0x0016AF25 File Offset: 0x00169125
		public void TrySetMaximumEducation(TINationState nation, float newValue)
		{
			if (!nation.alienNation && newValue > this.bestGlobalHumanEducation)
			{
				this.bestGlobalHumanEducation = newValue;
			}
		}

		// Token: 0x06003C25 RID: 15397 RVA: 0x0016AF40 File Offset: 0x00169140
		public static bool CanAnyHumanNationUsePriority(PriorityType priority)
		{
			if (priority == PriorityType.Military_BuildSpaceDefenses)
			{
				return GameStateManager.AllHumanNations().Any<TINationState>((TINationState x) => x.canBuildSpaceDefenses);
			}
			if (priority == PriorityType.Military_BuildSTOSquadron)
			{
				return GameStateManager.AllHumanNations().Any<TINationState>((TINationState x) => x.canBuildSTOSquadrons);
			}
			return true;
		}

		// Token: 0x06003C26 RID: 15398 RVA: 0x0016AFAC File Offset: 0x001691AC
		public Dictionary<FactionIdeology, float> GetGlobalPublicOpinionProportions()
		{
			Dictionary<FactionIdeology, float> dictionary = new Dictionary<FactionIdeology, float>();
			float num = 0f;
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				num += tinationState.population_Millions;
				foreach (FactionIdeology factionIdeology in from x in GameStateManager.ActiveHumanIdeologies()
					select x.ideology)
				{
					if (!dictionary.ContainsKey(factionIdeology))
					{
						dictionary.Add(factionIdeology, 0f);
					}
					Dictionary<FactionIdeology, float> dictionary2 = dictionary;
					FactionIdeology factionIdeology2 = factionIdeology;
					dictionary2[factionIdeology2] += tinationState.population_Millions * tinationState.GetPublicOpinionProportion(factionIdeology);
				}
			}
			Dictionary<FactionIdeology, float> dictionary3 = new Dictionary<FactionIdeology, float>();
			foreach (FactionIdeology factionIdeology3 in from x in GameStateManager.ActiveHumanIdeologies()
				select x.ideology)
			{
				Dictionary<FactionIdeology, float> dictionary4 = dictionary3;
				FactionIdeology factionIdeology4 = factionIdeology3;
				Dictionary<FactionIdeology, float> dictionary2 = dictionary;
				FactionIdeology factionIdeology2 = factionIdeology3;
				dictionary4.Add(factionIdeology4, dictionary2[factionIdeology2] /= num);
			}
			return dictionary3;
		}

		// Token: 0x06003C27 RID: 15399 RVA: 0x0016B13C File Offset: 0x0016933C
		private TINarrativeEventTemplate NarrativeEventTemplate(string dataName)
		{
			return TemplateManager.Find<TINarrativeEventTemplate>(dataName, false);
		}

		// Token: 0x06003C28 RID: 15400 RVA: 0x0016B148 File Offset: 0x00169348
		private bool EventConditionsMet(TINarrativeEventTemplate narrativeEvent)
		{
			if (narrativeEvent.eventOptions.Any<NarrativeEventOption>((NarrativeEventOption x) => x.outcomes.Any<NarrativeEventOutcome>((NarrativeEventOutcome y) => y.effectTemplates.Concat<TIEffectTemplate>(y.delayedEffectTemplates).Any<TIEffectTemplate>((TIEffectTemplate z) => z.instantEffect == InstantEffect.XenoformingChange && z.value > 0f))))
			{
				if (!GameStateManager.AllRegions().Any<TIRegionState>((TIRegionState x) => x.xenoforming.Extant()))
				{
					return false;
				}
			}
			if (narrativeEvent.year != null)
			{
				int year = this.gameTime.currentTime.year;
				int? num = narrativeEvent.year;
				if ((year < num.GetValueOrDefault()) & (num != null))
				{
					return false;
				}
			}
			if (narrativeEvent.endYear != null)
			{
				int year2 = this.gameTime.currentTime.year;
				int? num = narrativeEvent.endYear;
				if ((year2 > num.GetValueOrDefault()) & (num != null))
				{
					return false;
				}
			}
			if (narrativeEvent.earliestMonth != null && narrativeEvent.latestMonth != null)
			{
				int? num = narrativeEvent.earliestMonth;
				int? num2 = narrativeEvent.latestMonth;
				if ((num.GetValueOrDefault() > num2.GetValueOrDefault()) & ((num != null) & (num2 != null)))
				{
					int month = this.gameTime.currentTime.month;
					num2 = narrativeEvent.earliestMonth;
					if ((month < num2.GetValueOrDefault()) & (num2 != null))
					{
						int month2 = this.gameTime.currentTime.month;
						num2 = narrativeEvent.latestMonth;
						if ((month2 > num2.GetValueOrDefault()) & (num2 != null))
						{
							return false;
						}
					}
				}
				else
				{
					num2 = narrativeEvent.earliestMonth;
					num = narrativeEvent.latestMonth;
					if ((num2.GetValueOrDefault() < num.GetValueOrDefault()) & ((num2 != null) & (num != null)))
					{
						int month3 = this.gameTime.currentTime.month;
						num = narrativeEvent.earliestMonth;
						if (!((month3 < num.GetValueOrDefault()) & (num != null)))
						{
							int month4 = this.gameTime.currentTime.month;
							num = narrativeEvent.latestMonth;
							if (!((month4 > num.GetValueOrDefault()) & (num != null)))
							{
								goto IL_0272;
							}
						}
						return false;
					}
				}
			}
			else if (narrativeEvent.earliestMonth != null)
			{
				int month5 = this.gameTime.currentTime.month;
				int? num = narrativeEvent.earliestMonth;
				if ((month5 < num.GetValueOrDefault()) & (num != null))
				{
					return false;
				}
			}
			else if (narrativeEvent.latestMonth != null)
			{
				int month6 = this.gameTime.currentTime.month;
				int? num = narrativeEvent.latestMonth;
				if ((month6 > num.GetValueOrDefault()) & (num != null))
				{
					return false;
				}
			}
			IL_0272:
			if (!string.IsNullOrEmpty(narrativeEvent.reqTechDataName))
			{
				TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(narrativeEvent.reqTechDataName, false);
				if (!GameStateManager.GlobalResearch().IsTechFinished(titechTemplate))
				{
					return false;
				}
			}
			return (!narrativeEvent.requiresAliens || !(GameStateManager.AlienFaction() == null)) && (narrativeEvent.repeatable != RepeatableStatus.OncePerCampaign || !this.triggeredOncePerCampaignEvents.Contains(narrativeEvent.dataName));
		}

		// Token: 0x06003C29 RID: 15401 RVA: 0x0016B428 File Offset: 0x00169628
		private bool ValidateSingleTarget(TINarrativeEventTemplate narrativeEvent, TIGameState possibleTarget)
		{
			if (narrativeEvent.repeatable == RepeatableStatus.OncePerFaction)
			{
				List<TIFactionState> list = this.triggeredOncePerTargetEvents[narrativeEvent.dataName];
				TIGameState possibleTarget2 = possibleTarget;
				if (list.Contains((possibleTarget2 != null) ? possibleTarget2.ref_faction : null))
				{
					return false;
				}
			}
			if (narrativeEvent.target_cooldown_months > 0 && this.narrativeEventsTargetSpecificCooldowns.ContainsKey(narrativeEvent.dataName) && this.narrativeEventsTargetSpecificCooldowns[narrativeEvent.dataName].Any<EventStateCooldownData>((EventStateCooldownData x) => x.coolingState == possibleTarget))
			{
				return false;
			}
			List<TICondition> targetConditions = narrativeEvent.targetConditions;
			return targetConditions == null || targetConditions.All<TICondition>((TICondition condition) => condition.PassesCondition(possibleTarget));
		}

		// Token: 0x06003C2A RID: 15402 RVA: 0x0016B4DC File Offset: 0x001696DC
		private Dictionary<TIGameState, float> GetTargets(TINarrativeEventTemplate narrativeEvent, bool ignoreConditions = false)
		{
			Dictionary<TIGameState, float> dictionary = new Dictionary<TIGameState, float>();
			List<TIGameState> list = new List<TIGameState>();
			bool flag = false;
			foreach (string text in narrativeEvent.possibleTargetDataNames)
			{
				if (string.IsNullOrEmpty(text))
				{
					break;
				}
				flag = true;
				switch (narrativeEvent.targetType)
				{
				case NarrativeEventTargetType.faction:
				{
					TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(text, false);
					if (tifactionState != null)
					{
						list.Add(tifactionState);
					}
					break;
				}
				case NarrativeEventTargetType.nation:
				{
					TINationState tinationState = GameStateManager.FindByTemplate<TINationState>(text, false);
					if (tinationState != null)
					{
						list.Add(tinationState);
					}
					break;
				}
				case NarrativeEventTargetType.region:
				{
					TIRegionState tiregionState = GameStateManager.FindByTemplate<TIRegionState>(text, false);
					if (tiregionState != null)
					{
						list.Add(tiregionState);
					}
					break;
				}
				case NarrativeEventTargetType.mapRegion:
				{
					TIRegionState tiregionState2 = GameStateManager.MapRegionLookup(text);
					if (tiregionState2 != null)
					{
						list.Add(tiregionState2);
					}
					break;
				}
				case NarrativeEventTargetType.spaceBody:
				{
					TISpaceBodyState tispaceBodyState = GameStateManager.FindByTemplate<TISpaceBodyState>(text, false);
					if (tispaceBodyState != null)
					{
						list.Add(tispaceBodyState);
					}
					break;
				}
				case NarrativeEventTargetType.habSite:
				{
					TIHabSiteState tihabSiteState = GameStateManager.FindByTemplate<TIHabSiteState>(text, false);
					if (tihabSiteState != null)
					{
						list.Add(tihabSiteState);
					}
					else
					{
						TISpaceBodyState tispaceBodyState2 = GameStateManager.FindByTemplate<TISpaceBodyState>(text, false);
						if (tispaceBodyState2 != null)
						{
							list.AddRange(tispaceBodyState2.habSites);
						}
					}
					break;
				}
				case NarrativeEventTargetType.hab:
				{
					TIHabState tihabState = GameStateManager.FindByTemplate<TIHabState>(text, false);
					if (tihabState != null)
					{
						list.Add(tihabState);
					}
					else
					{
						TIHabSiteState tihabSiteState2 = GameStateManager.FindByTemplate<TIHabSiteState>(text, false);
						if (tihabSiteState2 != null && tihabSiteState2.hab != null)
						{
							list.Add(tihabSiteState2.hab);
						}
						else
						{
							TINaturalSpaceObjectState tinaturalSpaceObjectState = GameStateManager.FindByTemplate<TINaturalSpaceObjectState>(text, false);
							if (tinaturalSpaceObjectState != null)
							{
								list.AddRange(tinaturalSpaceObjectState.habs);
							}
						}
					}
					break;
				}
				}
			}
			if (!flag)
			{
				switch (narrativeEvent.targetType)
				{
				case NarrativeEventTargetType.none:
					goto IL_05E4;
				case NarrativeEventTargetType.global:
					list.Add(GameControl.control.activePlayer);
					goto IL_0603;
				case NarrativeEventTargetType.faction:
					list.AddRange(GameStateManager.AllHumanFactions());
					goto IL_0603;
				case NarrativeEventTargetType.nation:
					list.AddRange(GameStateManager.AllExtantNations());
					goto IL_0603;
				case NarrativeEventTargetType.region:
				case NarrativeEventTargetType.mapRegion:
					list.AddRange(GameStateManager.AllRegions());
					goto IL_0603;
				case NarrativeEventTargetType.councilor:
					list.AddRange(GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors));
					goto IL_0603;
				case NarrativeEventTargetType.spaceBody:
					list.AddRange(GameStateManager.AllSpaceBodies());
					goto IL_0603;
				case NarrativeEventTargetType.habSite:
					list.AddRange(GameStateManager.IterateByClass<TIHabSiteState>(false));
					goto IL_0603;
				case NarrativeEventTargetType.hab:
					list.AddRange(GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TIHabState>((TIFactionState x) => x.habs));
					goto IL_0603;
				case NarrativeEventTargetType.fleet:
					list.AddRange(GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets));
					goto IL_0603;
				case NarrativeEventTargetType.ship:
					list.AddRange(GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TISpaceShipState>((TIFactionState x) => x.ships));
					goto IL_0603;
				case NarrativeEventTargetType.officer:
					list.AddRange(GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TISpaceShipState>((TIFactionState x) => x.ships).SelectMany<TISpaceShipState, TIOfficerState>((TISpaceShipState y) => y.officers));
					list.AddRange(GameStateManager.AllHumanFactions().SelectMany<TIFactionState, TIHabState>((TIFactionState x) => x.habs).SelectMany<TIHabState, TIOfficerState>((TIHabState y) => y.officersOnBoard));
					goto IL_0603;
				case NarrativeEventTargetType.army:
					list.AddRange(from x in GameStateManager.IterateByClass<TIArmyState>(false)
						where x.armyType == ArmyType.Human
						select x);
					goto IL_0603;
				case NarrativeEventTargetType.war:
					goto IL_0603;
				case NarrativeEventTargetType.priorActor:
				{
					if (!this.priorNarrativeEventData.ContainsKey(narrativeEvent.dataName))
					{
						goto IL_0603;
					}
					using (List<PriorNarrativeEventData>.Enumerator enumerator2 = this.priorNarrativeEventData[narrativeEvent.dataName].GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							PriorNarrativeEventData priorNarrativeEventData = enumerator2.Current;
							if (priorNarrativeEventData.actorState != null && priorNarrativeEventData.actorState.exists)
							{
								list.Add(priorNarrativeEventData.actorState);
							}
						}
						goto IL_0603;
					}
					break;
				}
				case NarrativeEventTargetType.priorTarget:
					break;
				case NarrativeEventTargetType.priorSecondary:
					goto IL_0568;
				default:
					goto IL_0603;
				}
				if (!this.priorNarrativeEventData.ContainsKey(narrativeEvent.dataName))
				{
					goto IL_0603;
				}
				using (List<PriorNarrativeEventData>.Enumerator enumerator2 = this.priorNarrativeEventData[narrativeEvent.dataName].GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						PriorNarrativeEventData priorNarrativeEventData2 = enumerator2.Current;
						if (priorNarrativeEventData2.selectedTarget != null && priorNarrativeEventData2.selectedTarget.exists)
						{
							list.Add(priorNarrativeEventData2.selectedTarget);
						}
					}
					goto IL_0603;
				}
				IL_0568:
				if (!this.priorNarrativeEventData.ContainsKey(narrativeEvent.dataName))
				{
					goto IL_0603;
				}
				using (List<PriorNarrativeEventData>.Enumerator enumerator2 = this.priorNarrativeEventData[narrativeEvent.dataName].GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						PriorNarrativeEventData priorNarrativeEventData3 = enumerator2.Current;
						if (priorNarrativeEventData3.secondaryTarget != null && priorNarrativeEventData3.secondaryTarget.exists)
						{
							list.Add(priorNarrativeEventData3.secondaryTarget);
						}
					}
					goto IL_0603;
				}
				IL_05E4:
				Log.Warn("narrativeEvent " + narrativeEvent.dataName + " has no targetType", Array.Empty<object>());
			}
			IL_0603:
			foreach (TIGameState tigameState in list)
			{
				if (!dictionary.ContainsKey(tigameState) && this.ValidateSingleTarget(narrativeEvent, tigameState))
				{
					float num = 1f;
					foreach (NarrativeEventWeightModifier narrativeEventWeightModifier in narrativeEvent.targetWeightModifiers)
					{
						TICondition condition = narrativeEventWeightModifier.condition;
						if (condition != null && condition.PassesCondition(tigameState))
						{
							num *= narrativeEventWeightModifier.value;
						}
					}
					dictionary.Add(tigameState, num);
				}
			}
			if (ignoreConditions && dictionary.Count == 0 && list.Count > 0)
			{
				dictionary.Add(list.SelectRandomItem<TIGameState>(), 1f);
				Log.Info("DEBUG: Triggering Narrative Event " + narrativeEvent.dataName + " with no validated targets, picking random core target", Array.Empty<object>());
			}
			return dictionary;
		}

		// Token: 0x06003C2B RID: 15403 RVA: 0x0016BC74 File Offset: 0x00169E74
		public TIGameState GetSecondaryTarget(TINarrativeEventTemplate narrativeEvent, TIGameState primaryState, bool ignoreConditions = false)
		{
			List<TIGameState> list = new List<TIGameState>();
			if (narrativeEvent.secondaryStateType == EffectSecondaryStateType.InputState)
			{
				using (List<string>.Enumerator enumerator = narrativeEvent.possibleSecondaryStateDataNames.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						if (string.IsNullOrEmpty(text))
						{
							break;
						}
						TIGameState tigameState = GameStateManager.FindByTemplate<TIGameState>(text, true);
						if (tigameState != null)
						{
							list.Add(tigameState);
						}
					}
					goto IL_0072;
				}
			}
			list = TIEffectsState.GetEffectSecondaryStateCandidates(primaryState, narrativeEvent.secondaryStateType, null, narrativeEvent);
			IL_0072:
			Dictionary<TIGameState, float> dictionary = new Dictionary<TIGameState, float>();
			using (List<TIGameState>.Enumerator enumerator2 = list.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIGameState candidate = enumerator2.Current;
					if (!dictionary.ContainsKey(candidate))
					{
						List<TICondition> secondaryStateConditions = narrativeEvent.secondaryStateConditions;
						if (secondaryStateConditions == null || secondaryStateConditions.All<TICondition>((TICondition condition) => condition.PassesCondition(candidate)))
						{
							float num = 1f;
							if (narrativeEvent.secondaryWeightModifiers != null)
							{
								foreach (NarrativeEventWeightModifier narrativeEventWeightModifier in narrativeEvent.secondaryWeightModifiers)
								{
									TICondition condition2 = narrativeEventWeightModifier.condition;
									if (condition2 != null && condition2.PassesCondition(candidate))
									{
										num *= narrativeEventWeightModifier.value;
									}
								}
							}
							if (num > 0f)
							{
								dictionary.Add(candidate, num);
							}
						}
					}
				}
			}
			if (ignoreConditions && dictionary.Count == 0)
			{
				dictionary = list.ToDictionary<TIGameState, TIGameState, float>((TIGameState x) => x, (TIGameState x) => 1f);
			}
			if (dictionary.Count > 0)
			{
				return dictionary.SelectRandomWeightedItem<KeyValuePair<TIGameState, float>>((KeyValuePair<TIGameState, float> x) => x.Value, -1f, 1E-37f).Key;
			}
			return null;
		}

		// Token: 0x06003C2C RID: 15404 RVA: 0x0016BEA8 File Offset: 0x0016A0A8
		public void NarrativeEventsMonthlyUpdate()
		{
			new List<string>();
			foreach (string text in this.narrativeEvents.Keys.ToList<string>())
			{
				TINarrativeEventTemplate tinarrativeEventTemplate = this.NarrativeEventTemplate(text);
				if (!this.altWeightConditionTriggered.Contains(text))
				{
					Dictionary<string, float> dictionary = this.narrativeEvents;
					string text2 = text;
					dictionary[text2] += tinarrativeEventTemplate.monthlyWeightDelta;
					TIGlobalCondition tiglobalCondition = tinarrativeEventTemplate.altBaseWeight.condition as TIGlobalCondition;
					if (tiglobalCondition != null)
					{
						if (tiglobalCondition.PassesCondition(null))
						{
							this.altWeightConditionTriggered.Add(tinarrativeEventTemplate.dataName);
							float num = this.narrativeEvents[text] - tinarrativeEventTemplate.baseWeight;
							this.narrativeEvents[text] = tinarrativeEventTemplate.altBaseWeight.value + num;
						}
					}
					else if (!tinarrativeEventTemplate.forceEvent && this.narrativeEvents[text] <= 0f && tinarrativeEventTemplate.monthlyWeightDelta <= 0f)
					{
						this.narrativeEvents.Remove(text);
						this.inactiveNarrativeEvents.Add(text);
					}
				}
				else
				{
					Dictionary<string, float> dictionary = this.narrativeEvents;
					string text2 = text;
					dictionary[text2] += tinarrativeEventTemplate.altMonthlyWeightDelta;
					TIGlobalCondition tiglobalCondition2 = tinarrativeEventTemplate.altBaseWeight.condition as TIGlobalCondition;
					if (tiglobalCondition2 != null && !tiglobalCondition2.PassesCondition(null))
					{
						this.altWeightConditionTriggered.Remove(text);
						float num2 = this.narrativeEvents[text] - tinarrativeEventTemplate.altBaseWeight.value;
						this.narrativeEvents[text] = tinarrativeEventTemplate.baseWeight + num2;
					}
					if (!tinarrativeEventTemplate.forceEvent && this.narrativeEvents[text] <= 0f && tinarrativeEventTemplate.altMonthlyWeightDelta <= 0f)
					{
						this.narrativeEvents.Remove(text);
						this.inactiveNarrativeEvents.Add(text);
					}
				}
			}
			List<string> list = new List<string>();
			foreach (string text3 in this.narrativeEventsOnCooldown_months.Keys.ToList<string>())
			{
				Dictionary<string, float> dictionary = this.narrativeEventsOnCooldown_months;
				string text2 = text3;
				dictionary[text2] -= 1f;
				if (this.narrativeEventsOnCooldown_months[text3] <= 0f)
				{
					list.Add(text3);
				}
			}
			foreach (string text4 in this.narrativeEventsTargetSpecificCooldowns.Keys.ToList<string>())
			{
				List<EventStateCooldownData> list2 = new List<EventStateCooldownData>(this.narrativeEventsTargetSpecificCooldowns[text4]);
				List<int> list3 = new List<int>();
				for (int i = 0; i < list2.Count; i++)
				{
					this.narrativeEventsTargetSpecificCooldowns[text4][i] = new EventStateCooldownData(list2[i].coolingState, list2[i].cooldown_months - 1f);
					if (this.narrativeEventsTargetSpecificCooldowns[text4][i].cooldown_months == 0f)
					{
						list3.Add(i);
					}
				}
				for (int j = list2.Count - 1; j >= 0; j--)
				{
					if (list3.Contains(j))
					{
						this.narrativeEventsTargetSpecificCooldowns[text4].RemoveAt(j);
					}
				}
			}
			foreach (string text5 in list)
			{
				TINarrativeEventTemplate tinarrativeEventTemplate2 = this.NarrativeEventTemplate(text5);
				if (!this.narrativeEvents.ContainsKey(text5))
				{
					this.narrativeEventsOnCooldown_months.Remove(text5);
					float num3 = tinarrativeEventTemplate2.baseWeight + tinarrativeEventTemplate2.weightDeltaWhenTriggered;
					if (!this.altWeightConditionTriggered.Contains(text5))
					{
						TIGlobalCondition tiglobalCondition3 = tinarrativeEventTemplate2.altBaseWeight.condition as TIGlobalCondition;
						if (tiglobalCondition3 != null)
						{
							if (tiglobalCondition3.PassesCondition(null))
							{
								this.altWeightConditionTriggered.Add(tinarrativeEventTemplate2.dataName);
								num3 = tinarrativeEventTemplate2.altBaseWeight.value + tinarrativeEventTemplate2.altMonthlyWeightDelta;
							}
							else
							{
								this.altWeightConditionTriggered.Remove(text5);
							}
						}
					}
					else
					{
						num3 = tinarrativeEventTemplate2.altBaseWeight.value + tinarrativeEventTemplate2.altMonthlyWeightDelta;
					}
					this.narrativeEvents.Add(text5, num3);
				}
				else
				{
					Log.Warn("System tried to reactivate duplicate " + text5, Array.Empty<object>());
				}
			}
			List<string> list4 = new List<string>();
			foreach (string text6 in this.inactiveNarrativeEvents.ToList<string>())
			{
				TINarrativeEventTemplate tinarrativeEventTemplate3 = this.NarrativeEventTemplate(text6);
				if (tinarrativeEventTemplate3 == null)
				{
					Log.Error("Narrative event " + text6 + " not found while trying to check event's conditions", Array.Empty<object>());
				}
				else if (this.EventConditionsMet(tinarrativeEventTemplate3))
				{
					if (!this.narrativeEvents.ContainsKey(text6))
					{
						if (!this.altWeightConditionTriggered.Contains(text6))
						{
							this.narrativeEvents.Add(tinarrativeEventTemplate3.dataName, tinarrativeEventTemplate3.baseWeight);
						}
						else
						{
							this.narrativeEvents.Add(tinarrativeEventTemplate3.dataName, tinarrativeEventTemplate3.altBaseWeight.value);
						}
						list4.Add(tinarrativeEventTemplate3.dataName);
						this.inactiveNarrativeEvents.Remove(text6);
					}
					else
					{
						Log.Warn("System tried to add duplicate " + text6, Array.Empty<object>());
					}
				}
			}
			foreach (KeyValuePair<string, float> keyValuePair in this.narrativeEvents.ToDictionary<KeyValuePair<string, float>, string, float>((KeyValuePair<string, float> x) => x.Key, (KeyValuePair<string, float> x) => x.Value))
			{
				if (!list4.Contains(keyValuePair.Key))
				{
					TINarrativeEventTemplate tinarrativeEventTemplate4 = this.NarrativeEventTemplate(keyValuePair.Key);
					if (!this.EventConditionsMet(tinarrativeEventTemplate4))
					{
						this.inactiveNarrativeEvents.Add(tinarrativeEventTemplate4.dataName);
						this.narrativeEvents.Remove(keyValuePair.Key);
					}
				}
			}
			List<string> list5 = new List<string>();
			foreach (string text7 in this.narrativeEvents.Keys)
			{
				TINarrativeEventTemplate tinarrativeEventTemplate5 = this.NarrativeEventTemplate(text7);
				if (tinarrativeEventTemplate5.forceEvent && this.GetTargets(tinarrativeEventTemplate5, false).Any<KeyValuePair<TIGameState, float>>())
				{
					list5.Add(tinarrativeEventTemplate5.dataName);
				}
			}
			int num4;
			if (TIGlobalValuesState.Customizations.usingCustomizations && TIGlobalValuesState.Customizations.averageMonthlyEvents <= 0)
			{
				num4 = 0;
			}
			else
			{
				num4 = (int)Mathf.Clamp((float)TIUtilities.RandomRange(this.minEventsPerMonth, this.maxEventsPerMonth) * ((float)GameStateManager.AllHumanFactions().Length / 7f), (float)this.minEventsPerMonth, (float)this.maxEventsPerMonth);
				float num5 = 0f;
				foreach (string text8 in this.narrativeEvents.Keys.ToList<string>())
				{
					TINarrativeEventTemplate tinarrativeEventTemplate6 = this.NarrativeEventTemplate(text8);
					if (this.GetTargets(tinarrativeEventTemplate6, false).Count == 0)
					{
						this.narrativeEvents.Remove(text8);
						this.inactiveNarrativeEvents.Add(text8);
					}
					else
					{
						num5 += this.narrativeEvents[text8];
					}
				}
				num4 = Mathf.Clamp(num4, 1, (int)(num5 / 30f));
			}
			int num6 = list5.Count;
			if (num4 + num6 > this.minEventsPerMonth + this.maxEventsPerMonth)
			{
				num4 = this.maxEventsPerMonth - num6;
			}
			int num7 = 0;
			int num8 = 0;
			while (num8 < num4 && num7 < 200)
			{
				string key = this.narrativeEvents.SelectRandomWeightedItem<KeyValuePair<string, float>>((KeyValuePair<string, float> o) => o.Value, -1f, 1E-37f).Key;
				TINarrativeEventTemplate tinarrativeEventTemplate7 = this.NarrativeEventTemplate(key);
				if (!list5.Contains(key))
				{
					list5.Add(key);
					num8++;
					if (tinarrativeEventTemplate7.forceEvent)
					{
						num6++;
					}
				}
				num7++;
			}
			List<int> list6 = new List<int>(Enumerable.Range(2, DateTime.DaysInMonth(this.gameTime.Now.Year, this.gameTime.Now.Month) - 2));
			foreach (string text9 in list5)
			{
				TINarrativeEventTemplate tinarrativeEventTemplate8 = this.NarrativeEventTemplate(text9);
				int num9 = list6.SelectRandomItem<int>();
				if (tinarrativeEventTemplate8.forceEvent)
				{
					int num10 = 0;
					while (num9 > 8 + ((num6 >= 6) ? num6 : 0) && num10 < 1000)
					{
						num9 = list6.SelectRandomItem<int>();
						num10++;
					}
				}
				list6.Remove(num9);
				int num11 = TIUtilities.RandomRange(0, 23);
				if (num11 <= 11)
				{
					list6.Remove(num9 - 1);
				}
				else
				{
					list6.Remove(num9 + 1);
				}
				TIDateTime tidateTime = TITimeState.Now();
				float num12 = (float)(24 * (num9 - 1) + num11);
				tidateTime.AddHours((double)num12);
				TITimeEvent.CreateNewTimeEvent(tidateTime, this, null, tinarrativeEventTemplate8, "NarrativeEvent", true, false, TITimeQueueRepeatType.None, 1, true, false);
				if (list6.Count == 0)
				{
					break;
				}
			}
		}

		// Token: 0x06003C2D RID: 15405 RVA: 0x0016C94C File Offset: 0x0016AB4C
		private string FindNarrativeEvent(string eventDataName)
		{
			if (this.narrativeEvents.ContainsKey(eventDataName))
			{
				return "narrativeEvents";
			}
			if (this.inactiveNarrativeEvents.Contains(eventDataName))
			{
				return "inactiveNarrativeEvents";
			}
			if (this.removedNarrativeEvents.Contains(eventDataName))
			{
				return "removedNarrativeEvents";
			}
			if (this.narrativeEventsOnCooldown_months.ContainsKey(eventDataName))
			{
				return "narrativeEventsOnCooldown";
			}
			if (!(from x in TemplateManager.IterateByClass<TINarrativeEventTemplate>(true)
				select x.dataName).ToList<string>().Contains(eventDataName))
			{
				return "Can't Find DataName!";
			}
			return "Only found template.";
		}

		// Token: 0x06003C2E RID: 15406 RVA: 0x0016C9EC File Offset: 0x0016ABEC
		public static PendingNarrativeEvent GetCurrentNarrativeEvent(Prompt prompt)
		{
			return TIGlobalValuesState.GlobalValues.pendingNarrativeEvents.FirstOrDefault<PendingNarrativeEvent>((PendingNarrativeEvent x) => x.prompt == prompt);
		}

		// Token: 0x06003C2F RID: 15407 RVA: 0x0016CA24 File Offset: 0x0016AC24
		public static void ClearNarrativeEvent(Prompt prompt)
		{
			if (TIGlobalValuesState.GlobalValues.pendingNarrativeEvents.RemoveAll((PendingNarrativeEvent x) => x.prompt == prompt) != 1)
			{
				Log.Error("Error in clearing narrative events", Array.Empty<object>());
			}
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x0016CA6C File Offset: 0x0016AC6C
		public static Prompt FindPromptForNarrativeEvent(TIGameState actor, TIGameState target, TIGameState secondaryTarget, string narrativeEventName)
		{
			Prompt prompt;
			if (actor.isFactionState)
			{
				prompt = new Prompt(actor.ref_faction, target, secondaryTarget, "PromptAddressNarrativeEvent", 0);
			}
			else
			{
				prompt = new Prompt(actor.ref_nation, target, secondaryTarget, "PromptAddressNarrativeEvent", 0);
			}
			return TIGlobalValuesState.GlobalValues.pendingNarrativeEvents.FirstOrDefault<PendingNarrativeEvent>((PendingNarrativeEvent x) => x.prompt == prompt && narrativeEventName == x.dataName).prompt;
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x0016CAE2 File Offset: 0x0016ACE2
		public void TriggerNarrativeEvent(TimeEventStart e)
		{
			this.TriggerNarrativeEvent(e.eventDataTemplate as TINarrativeEventTemplate, null, false);
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x0016CAF8 File Offset: 0x0016ACF8
		public void TriggerNarrativeEvent(TINarrativeEventTemplate narrativeEvent, TIFactionState forceFaction = null, bool forceEvent = false)
		{
			if (this.promptQueue.HasAnyPromptofType("PromptAddressNarrativeEvent", false, false))
			{
				Log.Debug("NarrativeEvent fired when another pending", Array.Empty<object>());
			}
			Dictionary<TIGameState, float> targets = this.GetTargets(narrativeEvent, forceEvent);
			if (targets.Count > 0)
			{
				List<TIGameState> list = new List<TIGameState>();
				if (narrativeEvent.hitAllQualifyingTargets)
				{
					list.AddRange(targets.Keys);
				}
				else
				{
					if (forceFaction != null)
					{
						IEnumerable<TIGameState> enumerable = list.Where<TIGameState>((TIGameState x) => x.ref_faction == forceFaction);
						if (enumerable.Count<TIGameState>() > 0)
						{
							list = enumerable.ToList<TIGameState>();
						}
						else if (!forceEvent)
						{
							list = new List<TIGameState>();
						}
					}
					list.Add(targets.SelectRandomWeightedItem<KeyValuePair<TIGameState, float>>((KeyValuePair<TIGameState, float> o) => o.Value, -1f, 1E-37f).Key);
				}
				int count = list.Count;
				TIGameState tigameState = null;
				int num = 0;
				Dictionary<TIGameState, TIGameState> dictionary = new Dictionary<TIGameState, TIGameState>();
				foreach (TIGameState tigameState2 in list)
				{
					if (narrativeEvent.secondaryStateType != EffectSecondaryStateType.none)
					{
						if (count > 1 && narrativeEvent.sameSecondaryForAllTargets && tigameState != null)
						{
							if (!TIEffectsState.GetEffectSecondaryStateCandidates(tigameState2, narrativeEvent.secondaryStateType, null, narrativeEvent).Contains(tigameState))
							{
								continue;
							}
						}
						else
						{
							tigameState = this.GetSecondaryTarget(narrativeEvent, tigameState2, forceEvent);
							if (tigameState == null)
							{
								continue;
							}
						}
					}
					dictionary.Add(tigameState2, tigameState);
				}
				using (Dictionary<TIGameState, TIGameState>.KeyCollection.Enumerator enumerator2 = dictionary.Keys.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIGameState target = enumerator2.Current;
						TIFactionState tifactionState = ((forceEvent && forceFaction != null) ? forceFaction : target.ref_faction);
						num++;
						if (num <= 1 || !(tifactionState != null) || !this.triggeredOncePerTargetEvents.ContainsKey(narrativeEvent.dataName) || !this.triggeredOncePerTargetEvents[narrativeEvent.dataName].Contains(tifactionState))
						{
							if (num == 1 || !narrativeEvent.firstTargetNotificationOnly)
							{
								if (tifactionState != null)
								{
									TINotificationQueueState.AlertNarrativeEvent(tifactionState, narrativeEvent, target, tigameState, dictionary);
									Prompt prompt = new Prompt(tifactionState, target, tigameState, "PromptAddressNarrativeEvent", 0);
									this.promptQueue.AddPrompt(prompt);
									this.pendingNarrativeEvents.Add(new PendingNarrativeEvent
									{
										dataName = narrativeEvent.dataName,
										prompt = prompt,
										allTargetsandSeconds = dictionary
									});
								}
								else
								{
									TINationState ref_nation = target.ref_nation;
									if (!(ref_nation != null))
									{
										Log.Error("Narrative event " + narrativeEvent.dataName + " could not find faction or nation to target", Array.Empty<object>());
										return;
									}
									Prompt prompt2 = new Prompt(ref_nation, target, tigameState, "PromptAddressNarrativeEvent", 0);
									this.promptQueue.AddPrompt(prompt2);
									this.pendingNarrativeEvents.Add(new PendingNarrativeEvent
									{
										dataName = narrativeEvent.dataName,
										prompt = prompt2,
										allTargetsandSeconds = dictionary
									});
								}
							}
							if (narrativeEvent.repeatable == RepeatableStatus.OncePerFaction && tifactionState != null && !this.triggeredOncePerTargetEvents[narrativeEvent.dataName].Contains(tifactionState))
							{
								this.triggeredOncePerTargetEvents[narrativeEvent.dataName].Add(tifactionState);
								if (GameStateManager.AllHumanFactions().Length >= GameStateManager.AllHumanFactions().Intersect<TIFactionState>(this.triggeredOncePerTargetEvents[narrativeEvent.dataName]).Count<TIFactionState>())
								{
									this.removedNarrativeEvents.Add(narrativeEvent.dataName);
									this.inactiveNarrativeEvents.Remove(narrativeEvent.dataName);
									this.narrativeEvents.Remove(narrativeEvent.dataName);
									this.narrativeEventsOnCooldown_months.Remove(narrativeEvent.dataName);
									this.narrativeEventsTargetSpecificCooldowns.Remove(narrativeEvent.dataName);
								}
							}
							if (narrativeEvent.target_cooldown_months > 0)
							{
								if (!this.narrativeEventsTargetSpecificCooldowns.ContainsKey(narrativeEvent.dataName))
								{
									this.narrativeEventsTargetSpecificCooldowns.Add(narrativeEvent.dataName, new List<EventStateCooldownData>());
								}
								this.narrativeEventsTargetSpecificCooldowns[narrativeEvent.dataName].Add(new EventStateCooldownData(target, (float)narrativeEvent.target_cooldown_months));
							}
							if (narrativeEvent.reqEventUnlock)
							{
								List<PriorNarrativeEventData> list2 = new List<PriorNarrativeEventData>();
								switch (narrativeEvent.targetType)
								{
								case NarrativeEventTargetType.priorActor:
									list2 = this.priorNarrativeEventData[narrativeEvent.dataName].Where<PriorNarrativeEventData>((PriorNarrativeEventData x) => x.actorState == target && !x.actorState.deleted).ToList<PriorNarrativeEventData>();
									break;
								case NarrativeEventTargetType.priorTarget:
									list2 = this.priorNarrativeEventData[narrativeEvent.dataName].Where<PriorNarrativeEventData>((PriorNarrativeEventData x) => x.selectedTarget == target && !x.selectedTarget.deleted).ToList<PriorNarrativeEventData>();
									break;
								case NarrativeEventTargetType.priorSecondary:
									list2 = this.priorNarrativeEventData[narrativeEvent.dataName].Where<PriorNarrativeEventData>((PriorNarrativeEventData x) => x.secondaryTarget == target && !x.secondaryTarget.deleted).ToList<PriorNarrativeEventData>();
									break;
								}
								List<PriorNarrativeEventData> list3 = new List<PriorNarrativeEventData>();
								switch (narrativeEvent.secondaryStateType)
								{
								case EffectSecondaryStateType.PriorEvent_Actor:
									list3 = this.priorNarrativeEventData[narrativeEvent.dataName].Where<PriorNarrativeEventData>((PriorNarrativeEventData x) => x.actorState == target && !x.actorState.deleted).ToList<PriorNarrativeEventData>();
									break;
								case EffectSecondaryStateType.PriorEvent_Target:
									list3 = this.priorNarrativeEventData[narrativeEvent.dataName].Where<PriorNarrativeEventData>((PriorNarrativeEventData x) => x.selectedTarget == target && !x.selectedTarget.deleted).ToList<PriorNarrativeEventData>();
									break;
								case EffectSecondaryStateType.PriorEvent_SecondaryTarget:
									list3 = this.priorNarrativeEventData[narrativeEvent.dataName].Where<PriorNarrativeEventData>((PriorNarrativeEventData x) => x.secondaryTarget == target && !x.secondaryTarget.deleted).ToList<PriorNarrativeEventData>();
									break;
								}
								if (list2.Count > 0 && list3.Count > 0)
								{
									list2 = list2.Intersect<PriorNarrativeEventData>(list3).ToList<PriorNarrativeEventData>();
								}
								else if (list2.Count == 0 && list3.Count > 0)
								{
									list2 = list3;
								}
								if (list2.Count > 0)
								{
									this.priorNarrativeEventData[narrativeEvent.dataName].Remove(list2.First<PriorNarrativeEventData>());
								}
							}
						}
					}
				}
			}
			if (narrativeEvent.repeatable == RepeatableStatus.OncePerCampaign)
			{
				this.narrativeEvents.Remove(narrativeEvent.dataName);
				this.removedNarrativeEvents.Add(narrativeEvent.dataName);
				this.triggeredOncePerCampaignEvents.Add(narrativeEvent.dataName);
			}
			else if (narrativeEvent.global_cooldown_months > 0 && !forceEvent)
			{
				this.narrativeEvents.Remove(narrativeEvent.dataName);
				this.narrativeEventsOnCooldown_months[narrativeEvent.dataName] = (float)narrativeEvent.global_cooldown_months;
			}
			GameControl.eventManager.TriggerEvent(new TimeEventComplete(this, narrativeEvent), "NarrativeEvent", new object[] { this });
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x0016D20C File Offset: 0x0016B40C
		public void ExecuteNarrativeEventOption(TINarrativeEventTemplate eventTemplate, TIFactionState faction, TIGameState targetGameState, TIGameState secondaryGameState, int optionSelectedValue, Dictionary<TIGameState, TIGameState> allTargetsandSeconds, Prompt prompt)
		{
			NarrativeEventOption narrativeEventOption = eventTemplate.eventOptions[optionSelectedValue];
			NarrativeEventOutcome narrativeEventOutcome = narrativeEventOption.outcomes.SelectRandomWeightedItem<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.GetModifiedWeight(faction, targetGameState, secondaryGameState), -1f, 1E-37f);
			int num = narrativeEventOption.outcomes.IndexOf(narrativeEventOutcome);
			TIResourcesCost costs = narrativeEventOutcome.GetCosts(targetGameState);
			if (costs != null)
			{
				List<ResourceValue> resourceCosts = costs.resourceCosts;
				int? num2 = ((resourceCosts != null) ? new int?(resourceCosts.Count) : null);
				int num3 = 0;
				if ((num2.GetValueOrDefault() > num3) & (num2 != null))
				{
					costs.PayCost(faction, "Narrative Event");
				}
			}
			TIProjectTemplate projectGranted = narrativeEventOutcome.projectGranted;
			if (projectGranted != null)
			{
				faction.OnProjectComplete(projectGranted, -1, false, false);
			}
			TIOrgTemplate orgGranted = narrativeEventOutcome.orgGranted;
			if (orgGranted != null)
			{
				faction.CreateOrTransferOrgToFactionPool(orgGranted, true);
			}
			foreach (TINarrativeEventTemplate tinarrativeEventTemplate in narrativeEventOutcome.eventsToAdd)
			{
				if (this.EventConditionsMet(tinarrativeEventTemplate) && !this.narrativeEvents.ContainsKey(tinarrativeEventTemplate.dataName) && !this.inactiveNarrativeEvents.Contains(tinarrativeEventTemplate.dataName))
				{
					this.inactiveNarrativeEvents.Add(tinarrativeEventTemplate.dataName);
					if (tinarrativeEventTemplate.ShouldCacheEventData)
					{
						if (!this.priorNarrativeEventData.ContainsKey(tinarrativeEventTemplate.dataName))
						{
							this.priorNarrativeEventData.Add(tinarrativeEventTemplate.dataName, new List<PriorNarrativeEventData>());
						}
						this.priorNarrativeEventData[tinarrativeEventTemplate.dataName].Add(new PriorNarrativeEventData(eventTemplate, faction, targetGameState, secondaryGameState, allTargetsandSeconds));
					}
				}
			}
			foreach (TINarrativeEventTemplate tinarrativeEventTemplate2 in narrativeEventOutcome.eventsToRemove)
			{
				if (this.narrativeEvents.ContainsKey(tinarrativeEventTemplate2.dataName))
				{
					this.narrativeEvents.Remove(tinarrativeEventTemplate2.dataName);
					this.removedNarrativeEvents.Add(tinarrativeEventTemplate2.dataName);
				}
				this.inactiveNarrativeEvents.Remove(tinarrativeEventTemplate2.dataName);
				if (this.priorNarrativeEventData.ContainsKey(eventTemplate.dataName))
				{
					this.priorNarrativeEventData.Remove(eventTemplate.dataName);
				}
			}
			foreach (TIEffectTemplate tieffectTemplate in narrativeEventOutcome.effectTemplates)
			{
				if (eventTemplate.firstTargetNotificationOnly)
				{
					using (Dictionary<TIGameState, TIGameState>.KeyCollection.Enumerator enumerator3 = allTargetsandSeconds.Keys.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							TIGameState tigameState = enumerator3.Current;
							TIEffectsState.AddEffect(tieffectTemplate, faction, tigameState, allTargetsandSeconds[tigameState], "");
						}
						continue;
					}
				}
				TIEffectsState.AddEffect(tieffectTemplate, faction, targetGameState, secondaryGameState, "");
			}
			TINotificationQueueState.LogNarrativeEventResolution(faction, targetGameState, secondaryGameState, eventTemplate, optionSelectedValue, num, eventTemplate.ReportOutcome(narrativeEventOption, narrativeEventOutcome, faction, targetGameState, secondaryGameState));
			foreach (TIEffectTemplate tieffectTemplate2 in narrativeEventOutcome.delayedEffectTemplates)
			{
				if (eventTemplate.firstTargetNotificationOnly)
				{
					using (Dictionary<TIGameState, TIGameState>.KeyCollection.Enumerator enumerator3 = allTargetsandSeconds.Keys.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							TIGameState tigameState2 = enumerator3.Current;
							TIEffectsState.AddEffect(tieffectTemplate2, faction, tigameState2, allTargetsandSeconds[tigameState2], "");
						}
						continue;
					}
				}
				TIEffectsState.AddEffect(tieffectTemplate2, faction, targetGameState, secondaryGameState, "");
			}
			TIGlobalValuesState.ClearNarrativeEvent(prompt);
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x0016D674 File Offset: 0x0016B874
		public void ExecuteNarrativeEventOption(TINarrativeEventTemplate eventTemplate, TINationState nation, TIGameState targetGameState, TIGameState secondaryGameState, int optionSelectedValue, Dictionary<TIGameState, TIGameState> allTargetsandSeconds, Prompt prompt)
		{
			NarrativeEventOption narrativeEventOption = eventTemplate.eventOptions[optionSelectedValue];
			NarrativeEventOutcome narrativeEventOutcome = narrativeEventOption.outcomes.SelectRandomWeightedItem<NarrativeEventOutcome>((NarrativeEventOutcome x) => x.GetModifiedWeight(null, targetGameState, secondaryGameState), -1f, 1E-37f);
			int num = narrativeEventOption.outcomes.IndexOf(narrativeEventOutcome);
			foreach (TINarrativeEventTemplate tinarrativeEventTemplate in narrativeEventOutcome.eventsToAdd)
			{
				if (this.EventConditionsMet(tinarrativeEventTemplate) && !this.narrativeEvents.ContainsKey(tinarrativeEventTemplate.dataName) && !this.inactiveNarrativeEvents.Contains(tinarrativeEventTemplate.dataName))
				{
					this.inactiveNarrativeEvents.Add(tinarrativeEventTemplate.dataName);
					if (tinarrativeEventTemplate.ShouldCacheEventData)
					{
						if (!this.priorNarrativeEventData.ContainsKey(tinarrativeEventTemplate.dataName))
						{
							this.priorNarrativeEventData.Add(tinarrativeEventTemplate.dataName, new List<PriorNarrativeEventData>());
						}
						this.priorNarrativeEventData[tinarrativeEventTemplate.dataName].Add(new PriorNarrativeEventData(eventTemplate, nation, targetGameState, secondaryGameState, allTargetsandSeconds));
					}
				}
			}
			foreach (TINarrativeEventTemplate tinarrativeEventTemplate2 in narrativeEventOutcome.eventsToRemove)
			{
				if (this.narrativeEvents.ContainsKey(tinarrativeEventTemplate2.dataName))
				{
					this.narrativeEvents.Remove(tinarrativeEventTemplate2.dataName);
					this.removedNarrativeEvents.Add(tinarrativeEventTemplate2.dataName);
				}
				this.inactiveNarrativeEvents.Remove(tinarrativeEventTemplate2.dataName);
				if (this.priorNarrativeEventData.ContainsKey(eventTemplate.dataName))
				{
					this.priorNarrativeEventData.Remove(eventTemplate.dataName);
				}
			}
			foreach (TIEffectTemplate tieffectTemplate in narrativeEventOutcome.effectTemplates)
			{
				TIEffectsState.AddEffect(tieffectTemplate, null, targetGameState, secondaryGameState, "");
			}
			TINotificationQueueState.LogNarrativeEventResolution(nation, targetGameState, secondaryGameState, eventTemplate, optionSelectedValue, num, eventTemplate.ReportOutcome(narrativeEventOption, narrativeEventOutcome, null, targetGameState, secondaryGameState));
			foreach (TIEffectTemplate tieffectTemplate2 in narrativeEventOutcome.delayedEffectTemplates)
			{
				TIEffectsState.AddEffect(tieffectTemplate2, null, targetGameState, secondaryGameState, "");
			}
			TIGlobalValuesState.ClearNarrativeEvent(prompt);
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x0016D950 File Offset: 0x0016BB50
		public void CleanUpOrgs()
		{
			List<TIOrgState> list = (from x in GameStateManager.IterateByClass<TIOrgState>(false)
				where x.factionOrbit == null && x.template.randomized
				select x).ToList<TIOrgState>();
			int num = list.Count<TIOrgState>() - 50;
			List<TIOrgState> list2 = new List<TIOrgState>();
			for (int i = 0; i < num; i++)
			{
				TIOrgState tiorgState = list.SelectRandomWeightedItem<TIOrgState>((TIOrgState x) => (float)(4 - x.tier), -1f, 1E-37f);
				list2.Add(tiorgState);
				list.Remove(tiorgState);
			}
			foreach (TIOrgState tiorgState2 in list2)
			{
				tiorgState2.ArchiveState(true);
				GameStateManager.RemoveGameState<TIOrgState>(tiorgState2.ID, false);
			}
		}

		// Token: 0x06003C36 RID: 15414 RVA: 0x0016DA38 File Offset: 0x0016BC38
		public TIWarState InitiateWarFromStart(TIWarState war, TINationState attacker, TINationState defender, List<TINationState> attackingAlliance, List<TINationState> defendingAlliance)
		{
			war.SetWarData(attacker, defender, attackingAlliance, defendingAlliance, TITimeState.Now());
			Dictionary<TINationState, float> dictionary = war.cohesionGainByNation;
			TINationState tinationState = attacker;
			dictionary[tinationState] += TemplateManager.global.cohesionGainFromDeclaringWarOnOldRival;
			attackingAlliance.ForEach(delegate(TINationState x)
			{
				Dictionary<TINationState, float> cohesionGainByNation = war.cohesionGainByNation;
				cohesionGainByNation[x] += ((x != attacker) ? TemplateManager.global.cohesionGainFromAnsweringAllyCallToOffensiveWar : 0f);
			});
			dictionary = war.cohesionGainByNation;
			tinationState = defender;
			dictionary[tinationState] += TemplateManager.global.cohesionGainFromBeingTargetOfWar;
			defendingAlliance.ForEach(delegate(TINationState x)
			{
				Dictionary<TINationState, float> cohesionGainByNation2 = war.cohesionGainByNation;
				cohesionGainByNation2[x] += ((x != defender) ? TemplateManager.global.cohesionGainFromAnsweringAllyCallToDefensiveWar : 0f);
			});
			this.interstateWars.Add(war);
			return war;
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x0016DB18 File Offset: 0x0016BD18
		public TIWarState InitiateWar(TINationState attacker, TINationState defender, List<TINationState> attackingAlliance, List<TINationState> defendingAlliance)
		{
			TIWarState tiwarState = GameStateManager.CreateNewGameState<TIWarState>();
			tiwarState.SetWarData(attacker, defender, attackingAlliance, defendingAlliance, TITimeState.Now());
			this.interstateWars.Add(tiwarState);
			return tiwarState;
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x0016DB48 File Offset: 0x0016BD48
		public TIWarState FindWarByInitiators(TINationState attacker, TINationState defender)
		{
			return this.interstateWars.FirstOrDefault<TIWarState>((TIWarState x) => x.attacker == attacker && x.defender == defender);
		}

		// Token: 0x06003C39 RID: 15417 RVA: 0x0016DB80 File Offset: 0x0016BD80
		public void DeleteWar(TIWarState war)
		{
			this.interstateWars.Remove(war);
			foreach (TINationState tinationState in war.attackingAlliance)
			{
				foreach (TINationState tinationState2 in war.defendingAlliance)
				{
					tinationState.SyncWarCount(tinationState2);
					tinationState2.SyncWarCount(tinationState);
				}
			}
			war.ArchiveState(true);
			GameStateManager.RemoveGameState<TIWarState>(war.ID, false);
		}

		// Token: 0x06003C3A RID: 15418 RVA: 0x0016DC2C File Offset: 0x0016BE2C
		public static void AgglomerateAllWars()
		{
			foreach (TIWarState tiwarState in TIGlobalValuesState.GlobalValues.interstateWars.OrderBy<TIWarState, TIDateTime>((TIWarState x) => x.startDate).ToList<TIWarState>())
			{
				if (TIGameState.Valid(tiwarState) && !tiwarState.archived)
				{
					tiwarState.AgglomerateDuplicateWars();
				}
			}
		}

		// Token: 0x06003C3B RID: 15419 RVA: 0x0016DCBC File Offset: 0x0016BEBC
		public void CheckGlobalMilestone(GlobalMilestone milestoneToCheck, TIFactionState faction, TIGameState locationOfAchievement)
		{
			if ((faction == null || faction.IsActiveHumanFaction) && !this.globalMilestones.ContainsKey(milestoneToCheck) && milestoneToCheck != GlobalMilestone.none)
			{
				this.globalMilestones.Add(milestoneToCheck, faction);
				if (faction != null)
				{
					List<ResourceValue> list = new List<ResourceValue>();
					if (this.globalMilestoneRewards.ContainsKey(milestoneToCheck))
					{
						list = this.globalMilestoneRewards[milestoneToCheck];
					}
					TINotificationQueueState.LogGlobalMilestoneComplete(faction, milestoneToCheck, locationOfAchievement, list);
					foreach (ResourceValue resourceValue in list)
					{
						faction.AddToCurrentResource(resourceValue.value, resourceValue.resource, true, "Global Milestone");
					}
					GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(faction), null, new object[] { faction });
				}
			}
		}

		// Token: 0x06003C3C RID: 15420 RVA: 0x0016DDA4 File Offset: 0x0016BFA4
		public bool HasGlobalMilestoneBeenAchieved(GlobalMilestone milestone)
		{
			return this.globalMilestones.ContainsKey(milestone);
		}

		// Token: 0x06003C3D RID: 15421 RVA: 0x0016DDB4 File Offset: 0x0016BFB4
		public void CheckGlobalMilestoneOnHabFounding(TIHabState hab, bool createdFromTemplate)
		{
			if (hab.IsBase && !hab.faction.IsAlienFaction && !hab.coreModule.moduleTemplate.automated)
			{
				TIFactionState tifactionState = (createdFromTemplate ? null : hab.faction);
				GlobalMilestone globalMilestone = GlobalMilestone.none;
				if (hab.habSite.parentBody == GameStateManager.Luna())
				{
					globalMilestone = GlobalMilestone.FirstBaseOnLuna;
				}
				else if (hab.habSite.parentBody == GameStateManager.Mars())
				{
					globalMilestone = GlobalMilestone.FirstBaseOnMars;
				}
				else if (hab.habSite.parentBody.objectType == SpaceObjectType.Asteroid || hab.habSite.parentBody == GameStateManager.Ceres())
				{
					globalMilestone = GlobalMilestone.FirstAsteroidBase;
				}
				else
				{
					TISpaceObjectState getSunOrbitingRelatedObject = hab.GetSunOrbitingRelatedObject;
					if (getSunOrbitingRelatedObject == GameStateManager.Mercury())
					{
						globalMilestone = GlobalMilestone.FirstMercuryBase;
					}
					else if (getSunOrbitingRelatedObject == GameStateManager.Jupiter())
					{
						globalMilestone = GlobalMilestone.FirstJupiterSystemBase;
					}
					else if (getSunOrbitingRelatedObject == GameStateManager.Saturn())
					{
						globalMilestone = GlobalMilestone.FirstSaturnSystemBase;
					}
					else if (getSunOrbitingRelatedObject == GameStateManager.Uranus())
					{
						globalMilestone = GlobalMilestone.FirstUranusSystemBase;
					}
					else if (getSunOrbitingRelatedObject == GameStateManager.Neptune())
					{
						globalMilestone = GlobalMilestone.FirstNeptuneSystemBase;
					}
					else if (GameStateManager.KuiperBeltObjects(true).Contains(getSunOrbitingRelatedObject))
					{
						globalMilestone = GlobalMilestone.FirstKuiperBeltObjectBase;
					}
				}
				this.CheckGlobalMilestone(globalMilestone, tifactionState, hab);
			}
		}

		// Token: 0x06003C3E RID: 15422 RVA: 0x0016DEE0 File Offset: 0x0016C0E0
		public void SaveStartSettings()
		{
			if (!this.savedInit)
			{
				this.tutorialMode = GameControl.control.startupTutorialActive;
				this.startDifficulty = GameControl.control.startupDifficulty;
				this.savedInit = true;
			}
			TIGlobalValuesState.isTutorialActive = this.tutorialMode;
			this.difficulty = this.startDifficulty;
		}

		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x06003C3F RID: 15423 RVA: 0x0016DF33 File Offset: 0x0016C133
		public static bool CanDisableFactions
		{
			get
			{
				return TIGlobalValuesState.Customizations.canDisableFactions;
			}
		}

		// Token: 0x06003C40 RID: 15424 RVA: 0x0016DF3F File Offset: 0x0016C13F
		public static float GetResearchSpeedModifier()
		{
			return TIGlobalValuesState.Customizations.researchSpeedMultiplier;
		}

		// Token: 0x06003C41 RID: 15425 RVA: 0x0016DF4B File Offset: 0x0016C14B
		public static float GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact()
		{
			return TITimeState.CampaignDuration_years_Exact() * TIGlobalValuesState.Customizations.alienProgressionSpeed * GameStateManager.Time().template.alienProgressionModifier;
		}

		// Token: 0x06003C42 RID: 15426 RVA: 0x0016DF70 File Offset: 0x0016C170
		public static float GetAlienProgressionModifiedDuration_years_exact()
		{
			float num = GameStateManager.Time().template.alienStartingProgression_years + TIGlobalValuesState.GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact();
			return Mathf.Max(0f, num);
		}

		// Token: 0x06003C43 RID: 15427 RVA: 0x0016DF9E File Offset: 0x0016C19E
		public static float GetGlobalMineProductivityModifier()
		{
			return TIGlobalValuesState.Customizations.miningProductivityMultiplier;
		}

		// Token: 0x06003C44 RID: 15428 RVA: 0x0016DFAC File Offset: 0x0016C1AC
		public static float GetHabModuleConstructionTimeSettingsModifier(TIFactionState faction)
		{
			if (GameControl.control.activePlayer == faction)
			{
				return 1f / GameStateManager.GlobalValues().scenarioCustomizations.habConstructionSpeedPlayer;
			}
			if (faction.IsAlienFaction)
			{
				return 1f / GameStateManager.GlobalValues().scenarioCustomizations.habConstructionSpeedAlien;
			}
			return 1f / GameStateManager.GlobalValues().scenarioCustomizations.habConstructionSpeedHumanAI;
		}

		// Token: 0x06003C45 RID: 15429 RVA: 0x0016E014 File Offset: 0x0016C214
		public static float GetShipConstructionTimeSettingsModifier(TIFactionState faction)
		{
			if (GameControl.control.activePlayer == faction)
			{
				return 1f / GameStateManager.GlobalValues().scenarioCustomizations.shipConstructionSpeedPlayer;
			}
			if (faction.IsAlienFaction)
			{
				return 1f / GameStateManager.GlobalValues().scenarioCustomizations.shipConstructionSpeedAlien;
			}
			return 1f / GameStateManager.GlobalValues().scenarioCustomizations.shipConstructionSpeedHumanAI;
		}

		// Token: 0x06003C46 RID: 15430 RVA: 0x0016E07C File Offset: 0x0016C27C
		public static float GetMiningRateSettingsModifier(TIFactionState faction)
		{
			if (GameControl.control.activePlayer == faction)
			{
				return GameStateManager.GlobalValues().scenarioCustomizations.miningRatePlayer;
			}
			if (faction.IsAlienFaction)
			{
				return GameStateManager.GlobalValues().scenarioCustomizations.miningRateAlien;
			}
			return GameStateManager.GlobalValues().scenarioCustomizations.miningRateHumanAI;
		}

		// Token: 0x06003C47 RID: 15431 RVA: 0x0016E0D2 File Offset: 0x0016C2D2
		public static bool IsQuietAlienCampaign()
		{
			return GameStateManager.Time().template.alienQuietDuration_years > 0f;
		}

		// Token: 0x06003C48 RID: 15432 RVA: 0x0016E0EA File Offset: 0x0016C2EA
		public static bool IsInvasionFocusedAlienCampaign()
		{
			return GameStateManager.Time().template.invasionFocusedAliens;
		}

		// Token: 0x06003C49 RID: 15433 RVA: 0x0016E0FB File Offset: 0x0016C2FB
		public static void ClearStaticData()
		{
			TIGlobalValuesState.cachedGlobalGDP = -1.0;
			TIGlobalValuesState.cachedGlobalResearch = -1f;
			TIGlobalValuesState.cachedGlobalGDP_CampaignStart = -1.0;
		}

		// Token: 0x040025E6 RID: 9702
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040025E7 RID: 9703
		public Dictionary<FactionResource, float> resourceMarketValues;

		// Token: 0x040025EB RID: 9707
		public float[] pastEarthAtmosphericCO2_ppm = new float[12];

		// Token: 0x040025EC RID: 9708
		public float[] pastEarthAtmosphericCH4_ppm = new float[12];

		// Token: 0x040025ED RID: 9709
		public float[] pastEarthAtmosphericN2O_ppm = new float[12];

		// Token: 0x040025F2 RID: 9714
		public bool globalSeaLevelRise1Triggered;

		// Token: 0x040025F3 RID: 9715
		public bool globalSeaLevelRise2Triggered;

		// Token: 0x040025F4 RID: 9716
		public bool endOfOil;

		// Token: 0x040025F5 RID: 9717
		public Dictionary<GHGSources, double> CO2SourcesRecord_ppm;

		// Token: 0x040025F6 RID: 9718
		public Dictionary<GHGSources, double> CH4SourcesRecord_ppm;

		// Token: 0x040025F7 RID: 9719
		public Dictionary<GHGSources, double> N2OSourcesRecord_ppm;

		// Token: 0x040025FE RID: 9726
		[fsIgnore]
		public TIRegionState SuezRegion;

		// Token: 0x040025FF RID: 9727
		[fsIgnore]
		public TIRegionState PanamaRegion;

		// Token: 0x04002600 RID: 9728
		[fsIgnore]
		public TIRegionState TurkishStraitRegion;

		// Token: 0x04002601 RID: 9729
		public ScenarioCustomizations scenarioCustomizations;

		// Token: 0x04002602 RID: 9730
		public List<NuclearExchange> currentNuclearExchanges;

		// Token: 0x04002603 RID: 9731
		public static bool isTutorialActive;

		// Token: 0x04002604 RID: 9732
		private GameTimeManager gameTime;

		// Token: 0x04002605 RID: 9733
		public List<string> inactiveNarrativeEvents;

		// Token: 0x04002606 RID: 9734
		public Dictionary<string, float> narrativeEvents;

		// Token: 0x04002607 RID: 9735
		public Dictionary<string, float> narrativeEventsOnCooldown_months;

		// Token: 0x04002608 RID: 9736
		public Dictionary<string, List<EventStateCooldownData>> narrativeEventsTargetSpecificCooldowns;

		// Token: 0x04002609 RID: 9737
		public List<string> triggeredOncePerCampaignEvents;

		// Token: 0x0400260A RID: 9738
		public Dictionary<string, List<TIFactionState>> triggeredOncePerTargetEvents;

		// Token: 0x0400260B RID: 9739
		public Dictionary<string, List<PriorNarrativeEventData>> priorNarrativeEventData;

		// Token: 0x0400260C RID: 9740
		public List<string> altWeightConditionTriggered;

		// Token: 0x0400260D RID: 9741
		public List<string> removedNarrativeEvents;

		// Token: 0x0400260E RID: 9742
		public List<PendingNarrativeEvent> pendingNarrativeEvents;

		// Token: 0x0400260F RID: 9743
		public List<TIWarState> interstateWars;

		// Token: 0x04002613 RID: 9747
		public bool moddingActive;

		// Token: 0x04002614 RID: 9748
		public bool moddingUsedAnytime;

		// Token: 0x04002615 RID: 9749
		public int currentTechSort;

		// Token: 0x04002616 RID: 9750
		public bool techSortAscend = true;

		// Token: 0x04002617 RID: 9751
		public int currentProjectSort;

		// Token: 0x04002618 RID: 9752
		public bool projectSortAscend = true;

		// Token: 0x04002619 RID: 9753
		public bool projectSortShowObsolete = true;

		// Token: 0x0400261A RID: 9754
		public bool fleetScreenClassShowObsolete = true;

		// Token: 0x0400261B RID: 9755
		public bool habQuickBuildToggle;

		// Token: 0x0400261C RID: 9756
		public bool habQuickBuildWithBoostToggle;

		// Token: 0x0400261D RID: 9757
		public bool showFinderCouncilors = true;

		// Token: 0x0400261E RID: 9758
		public bool showFinderArmies = true;

		// Token: 0x0400261F RID: 9759
		public bool showFinderHabs = true;

		// Token: 0x04002620 RID: 9760
		public bool showFinderFleets = true;

		// Token: 0x04002621 RID: 9761
		public Dictionary<GlobalMilestone, TIFactionState> globalMilestones;

		// Token: 0x04002622 RID: 9762
		public int alienInvaderArmies;

		// Token: 0x04002623 RID: 9763
		private TIPromptQueueState promptQueue;

		// Token: 0x04002624 RID: 9764
		private TITimeState timeState;

		// Token: 0x04002625 RID: 9765
		[SerializeField]
		private float baselineUnnormalizedSpaceCombatValue = -1f;

		// Token: 0x04002626 RID: 9766
		private static float baselineUnnormalizedSpaceCombatValue_Static = -1f;

		// Token: 0x04002629 RID: 9769
		[fsIgnore]
		public List<string> councilorAppearanceTemplatesInUse = new List<string>();

		// Token: 0x0400262C RID: 9772
		[fsIgnore]
		public Dictionary<FactionIdeology, TIFactionIdeologyTemplate> ideologyTemplateLookup = new Dictionary<FactionIdeology, TIFactionIdeologyTemplate>();

		// Token: 0x0400262D RID: 9773
		private static double cachedGlobalGDP = -1.0;

		// Token: 0x0400262E RID: 9774
		private static double cachedGlobalGDP_CampaignStart = -1.0;

		// Token: 0x0400262F RID: 9775
		public static float cachedGlobalResearch = -1f;

		// Token: 0x04002630 RID: 9776
		[SerializeField]
		private float fixedPCGDPToReduceUnrestBy1 = -1f;

		// Token: 0x04002631 RID: 9777
		[SerializeField]
		private float fixedPCGDPToRaiseMissionBaseDifficultyBy1 = -1f;

		// Token: 0x04002632 RID: 9778
		[SerializeField]
		private bool repairCPMaintenanceScaling = true;

		// Token: 0x04002633 RID: 9779
		[SerializeField]
		private float fixedPCGDPToRaiseBaseCPMaintenanceCostBy1 = -1f;

		// Token: 0x04002634 RID: 9780
		public static bool isSpaceCombatEnabled = false;

		// Token: 0x04002635 RID: 9781
		[fsIgnore]
		public Dictionary<FactionIdeology, Dictionary<FactionIdeology, float>> ideologyDistanceGrid = new Dictionary<FactionIdeology, Dictionary<FactionIdeology, float>>();

		// Token: 0x04002636 RID: 9782
		[fsIgnore]
		public float worstCasePublicOpinionDispersal;

		// Token: 0x04002637 RID: 9783
		public const float preindustrialCO2_ppm = 280f;

		// Token: 0x04002638 RID: 9784
		public const float preindustrialCH4_ppm = 0.7f;

		// Token: 0x04002639 RID: 9785
		public const float preindustrialN2O_ppm = 0.27f;

		// Token: 0x0400263A RID: 9786
		public const float safeAtmosphericCO2_ppm = 325.68f;

		// Token: 0x0400263B RID: 9787
		public const float safeAtmosphericCH4_ppm = 1.3f;

		// Token: 0x0400263C RID: 9788
		public const float safeAtmosphericN2O_ppm = 0.29f;

		// Token: 0x0400263D RID: 9789
		public const float safeAtmosphericGICs_ppm = 0f;

		// Token: 0x0400263E RID: 9790
		public const float CH4_relativeImpact = 21f;

		// Token: 0x0400263F RID: 9791
		public const float N2O_relativeImpact = 289f;

		// Token: 0x04002640 RID: 9792
		private const float anomalyFactor = 94.5f;

		// Token: 0x04002641 RID: 9793
		public const float aerosolsFromNukeBarrage_ppm = 0.00777f;

		// Token: 0x04002642 RID: 9794
		public const float aerosolsFromSingleNuke_ppm = 7.77E-05f;

		// Token: 0x04002643 RID: 9795
		public const float aerosolsForCloudCover_ppm = 0.01f;

		// Token: 0x04002644 RID: 9796
		public const float xenoformingFullCoverageCO2AnnualConsumption_ppm = 3.45f;

		// Token: 0x04002645 RID: 9797
		public const float globalSeaLevelRise1_cm = 61f;

		// Token: 0x04002646 RID: 9798
		public const float globalSeaLevelRise2_cm = 305f;

		// Token: 0x04002647 RID: 9799
		public const float MeltPerCAnomaly = 0.017f;

		// Token: 0x04002648 RID: 9800
		private int minEventsPerMonth = (TIGlobalValuesState.Customizations.usingCustomizations ? Mathf.Max(0, TIGlobalValuesState.Customizations.averageMonthlyEvents - TemplateManager.global.randomEventsPerMonthVariability) : 3);

		// Token: 0x04002649 RID: 9801
		private int maxEventsPerMonth = (TIGlobalValuesState.Customizations.usingCustomizations ? (TIGlobalValuesState.Customizations.averageMonthlyEvents + TemplateManager.global.randomEventsPerMonthVariability) : 7);

		// Token: 0x0400264A RID: 9802
		private Dictionary<GlobalMilestone, List<ResourceValue>> globalMilestoneRewards = new Dictionary<GlobalMilestone, List<ResourceValue>>
		{
			{
				GlobalMilestone.FirstMercuryBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 500f),
					new ResourceValue(FactionResource.Influence, 100f)
				}
			},
			{
				GlobalMilestone.FirstBaseOnLuna,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 200f),
					new ResourceValue(FactionResource.Influence, 100f)
				}
			},
			{
				GlobalMilestone.FirstBaseOnMars,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 500f),
					new ResourceValue(FactionResource.Influence, 200f)
				}
			},
			{
				GlobalMilestone.FirstAsteroidBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 500f),
					new ResourceValue(FactionResource.Influence, 100f)
				}
			},
			{
				GlobalMilestone.FirstJupiterSystemBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 1000f),
					new ResourceValue(FactionResource.Influence, 200f)
				}
			},
			{
				GlobalMilestone.FirstSaturnSystemBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 1000f),
					new ResourceValue(FactionResource.Influence, 100f)
				}
			},
			{
				GlobalMilestone.FirstUranusSystemBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 500f),
					new ResourceValue(FactionResource.Influence, 100f)
				}
			},
			{
				GlobalMilestone.FirstNeptuneSystemBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 500f),
					new ResourceValue(FactionResource.Influence, 100f)
				}
			},
			{
				GlobalMilestone.FirstKuiperBeltObjectBase,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Research, 1000f),
					new ResourceValue(FactionResource.Influence, 200f)
				}
			},
			{
				GlobalMilestone.FirstWarship,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Influence, 200f)
				}
			},
			{
				GlobalMilestone.FirstSpaceCombatVictoryAgainstAliens,
				new List<ResourceValue>
				{
					new ResourceValue(FactionResource.Influence, 500f)
				}
			}
		};

		// Token: 0x0400264B RID: 9803
		public bool savedInit;

		// Token: 0x0400264C RID: 9804
		public bool tutorialMode;

		// Token: 0x0400264D RID: 9805
		public int startDifficulty;
	}
}
