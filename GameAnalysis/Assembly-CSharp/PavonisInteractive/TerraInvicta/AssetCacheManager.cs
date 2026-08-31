using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005B7 RID: 1463
	public static class AssetCacheManager
	{
		// Token: 0x04001D77 RID: 7543
		public static readonly Sprite armyIconBackground = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmyIconBackground);

		// Token: 0x04001D78 RID: 7544
		public static readonly Sprite navalArmyIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathNavalArmyIcon);

		// Token: 0x04001D79 RID: 7545
		public static readonly Sprite councilorIconBackground = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathCouncilorIconBackground);

		// Token: 0x04001D7A RID: 7546
		public static readonly Sprite controlPointCircle = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathEmptyControlPoint);

		// Token: 0x04001D7B RID: 7547
		public static readonly Sprite coreEconomicRegionIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathCoreEconomicRegion);

		// Token: 0x04001D7C RID: 7548
		public static readonly Sprite coreResourceRegionOilIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathCoreResourceRegion_Oil);

		// Token: 0x04001D7D RID: 7549
		public static readonly Sprite coreResourceRegionMiningIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathCoreResourceRegion_Mining);

		// Token: 0x04001D7E RID: 7550
		public static readonly Sprite launchFacilitySmallIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeLaunchSite1);

		// Token: 0x04001D7F RID: 7551
		public static readonly Sprite launchFacilityMediumIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeLaunchSite2);

		// Token: 0x04001D80 RID: 7552
		public static readonly Sprite launchFacilityLargeIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeLaunchSite3);

		// Token: 0x04001D81 RID: 7553
		public static readonly Sprite missionControlFacilitySmallIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeMissionControl1);

		// Token: 0x04001D82 RID: 7554
		public static readonly Sprite missionControlFacilityMediumIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeMissionControl2);

		// Token: 0x04001D83 RID: 7555
		public static readonly Sprite missionControlFacilityLargeIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeMissionControl3);

		// Token: 0x04001D84 RID: 7556
		public static readonly Sprite spaceDefensesIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeSpaceDefenses);

		// Token: 0x04001D85 RID: 7557
		public static readonly Sprite STOFighterIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathSTOFighter);

		// Token: 0x04001D86 RID: 7558
		public static readonly Sprite airliner1 = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeAirliner1);

		// Token: 0x04001D87 RID: 7559
		public static readonly Sprite airliner2 = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeAirliner2);

		// Token: 0x04001D88 RID: 7560
		public static readonly Sprite privateJet1 = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapePrivateJet1);

		// Token: 0x04001D89 RID: 7561
		public static readonly Sprite privateJet2 = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapePrivateJet2);

		// Token: 0x04001D8A RID: 7562
		public static readonly Sprite armyCombatIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmyCombatIcon);

		// Token: 0x04001D8B RID: 7563
		public static readonly Sprite humanArmy0_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy0_attacking);

		// Token: 0x04001D8C RID: 7564
		public static readonly Sprite humanArmy1_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy1_attacking);

		// Token: 0x04001D8D RID: 7565
		public static readonly Sprite humanArmy2_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy2_attacking);

		// Token: 0x04001D8E RID: 7566
		public static readonly Sprite humanArmy3_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy3_attacking);

		// Token: 0x04001D8F RID: 7567
		public static readonly Sprite humanArmy4_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy4_attacking);

		// Token: 0x04001D90 RID: 7568
		public static readonly Sprite humanArmy5_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy5_attacking);

		// Token: 0x04001D91 RID: 7569
		public static readonly Sprite humanArmy6_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy6_attacking);

		// Token: 0x04001D92 RID: 7570
		public static readonly Sprite humanArmy7_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy7_attacking);

		// Token: 0x04001D93 RID: 7571
		public static readonly Sprite humanArmy0_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy0_defending);

		// Token: 0x04001D94 RID: 7572
		public static readonly Sprite humanArmy1_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy1_defending);

		// Token: 0x04001D95 RID: 7573
		public static readonly Sprite humanArmy2_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy2_defending);

		// Token: 0x04001D96 RID: 7574
		public static readonly Sprite humanArmy3_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy3_defending);

		// Token: 0x04001D97 RID: 7575
		public static readonly Sprite humanArmy4_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy4_defending);

		// Token: 0x04001D98 RID: 7576
		public static readonly Sprite humanArmy5_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy5_defending);

		// Token: 0x04001D99 RID: 7577
		public static readonly Sprite humanArmy6_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy6_defending);

		// Token: 0x04001D9A RID: 7578
		public static readonly Sprite humanArmy7_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy7_defending);

		// Token: 0x04001D9B RID: 7579
		public static readonly Sprite alienMegafaunaArmy = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathAlienMegafaunaArmy);

		// Token: 0x04001D9C RID: 7580
		public static readonly Sprite alienArmy_def = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathAlienArmy_defending);

		// Token: 0x04001D9D RID: 7581
		public static readonly Sprite alienArmy_att = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathAlienArmy_defending);

		// Token: 0x04001D9E RID: 7582
		public static readonly Sprite navyTransportIcon_0 = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy0_sea);

		// Token: 0x04001D9F RID: 7583
		public static readonly Sprite navyTransportIcon_2 = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathArmy2_sea);

		// Token: 0x04001DA0 RID: 7584
		public static readonly Sprite alienNavyTransportIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathAlienArmy_defending);

		// Token: 0x04001DA1 RID: 7585
		public static readonly Sprite warningIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathWarningIcon);

		// Token: 0x04001DA2 RID: 7586
		public static readonly Sprite rammingSpeedIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.rammingSpeedIcon);

		// Token: 0x04001DA3 RID: 7587
		public static readonly Sprite disengageIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.disengageIcon);

		// Token: 0x04001DA4 RID: 7588
		public static readonly Sprite unidentifiedCouncilor = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathGeoscapeUnidentifiedCouncilor);

		// Token: 0x04001DA5 RID: 7589
		public static readonly Sprite defendInterestsIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.defendInterestsMissionIconPath);

		// Token: 0x04001DA6 RID: 7590
		public static readonly Sprite crackdownIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.crackdownMissionIconPath);

		// Token: 0x04001DA7 RID: 7591
		public static readonly Sprite smallDefendInterestsIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.smallDefendInterestsMissionIconPath);

		// Token: 0x04001DA8 RID: 7592
		public static readonly Sprite smallCrackdownIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.smallCrackdownMissionIconPath);

		// Token: 0x04001DA9 RID: 7593
		public static readonly Sprite spaceCombatIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathFleetCombatIcon);

		// Token: 0x04001DAA RID: 7594
		public static readonly Sprite orbitIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathOrbitIcon);

		// Token: 0x04001DAB RID: 7595
		public static readonly Sprite prospectedHabSiteIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathProspectedHabSite);

		// Token: 0x04001DAC RID: 7596
		public static readonly Sprite notProspectedHabSiteIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathNotProspectedHabSite);

		// Token: 0x04001DAD RID: 7597
		public static readonly Sprite beyondRangeHabSiteIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathBeyondRangeHabSite);

		// Token: 0x04001DAE RID: 7598
		public static readonly Sprite prospectedIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathProbeComplete);

		// Token: 0x04001DAF RID: 7599
		public static readonly Sprite prospectingUnderway = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathProbeEnRoute);

		// Token: 0x04001DB0 RID: 7600
		public static readonly Sprite plusButtonIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathPlusButtonIconPath);

		// Token: 0x04001DB1 RID: 7601
		public static readonly Sprite plusButtonHoverIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathPlusHoverButtonIconPath);

		// Token: 0x04001DB2 RID: 7602
		public static readonly Sprite minusButtonIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathMinusButtonIconPath);

		// Token: 0x04001DB3 RID: 7603
		public static readonly Sprite minusButtonHoverIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathMinusHoverButtonIconPath);

		// Token: 0x04001DB4 RID: 7604
		public static readonly Sprite notificationPlusButtonIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathNotificationPlusButtonIconPath);

		// Token: 0x04001DB5 RID: 7605
		public static readonly Sprite notificationPlusButtonHoverIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathNotificationPlusHoverButtonIconPath);

		// Token: 0x04001DB6 RID: 7606
		public static readonly Sprite notificationMinusButtonIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathNotificationMinusButtonIconPath);

		// Token: 0x04001DB7 RID: 7607
		public static readonly Sprite notificationMinusButtonHoverIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathNotificationMinusHoverButtonIconPath);

		// Token: 0x04001DB8 RID: 7608
		public static readonly Sprite MaximizeButtonIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathMaximizeButtonIconPath);

		// Token: 0x04001DB9 RID: 7609
		public static readonly Sprite MaximizeButtonHoverIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathMaximizeHoverButtonIconPath);

		// Token: 0x04001DBA RID: 7610
		public static readonly Sprite MinimizeButtonIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathMinimizeButtonIconPath);

		// Token: 0x04001DBB RID: 7611
		public static readonly Sprite MinimizeButtonHoverIcon = GameControl.assetLoader.LoadAsset<Sprite>(TemplateManager.global.pathMinimizeHoverButtonIconPath);

		// Token: 0x04001DBC RID: 7612
		public static readonly GameObject stationPrefab = Resources.Load<GameObject>("Prefabs/StationPrefab");

		// Token: 0x04001DBD RID: 7613
		public static readonly GameObject basePrefab = Resources.Load<GameObject>("Prefabs/BasePrefab");

		// Token: 0x04001DBE RID: 7614
		public static readonly GameObject surfaceBasePrefab = Resources.Load<GameObject>("Prefabs/SurfaceBasePrefab");

		// Token: 0x04001DBF RID: 7615
		public static readonly GameObject basicAlienConnectorPrefab = GameControl.assetLoader.LoadAsset<GameObject>("habmodules/Alien_Connector_Module");

		// Token: 0x04001DC0 RID: 7616
		public static readonly GameObject stationShuttleSoyuz = GameControl.assetLoader.LoadAsset<GameObject>("ships/Soyuz");

		// Token: 0x04001DC1 RID: 7617
		public static readonly GameObject stationShuttleA = GameControl.assetLoader.LoadAsset<GameObject>("ships/Shuttle_A");

		// Token: 0x04001DC2 RID: 7618
		public static readonly GameObject stationShuttleB = GameControl.assetLoader.LoadAsset<GameObject>("ships/Shuttle_B");

		// Token: 0x04001DC3 RID: 7619
		public static readonly Dictionary<string, GameObject> stationModulePrefabs = (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
			where !string.IsNullOrEmpty(x.stationModelResource)
			group x by x.stationModelResource into x
			select x.FirstOrDefault<TIHabModuleTemplate>()).ToDictionary<TIHabModuleTemplate, string, GameObject>((TIHabModuleTemplate x) => x.stationModelResource, (TIHabModuleTemplate x) => GameControl.assetLoader.LoadAsset<GameObject>(x.stationModelResource));

		// Token: 0x04001DC4 RID: 7620
		public static readonly Dictionary<string, Sprite> habStationModuleIcons = (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
			where !string.IsNullOrEmpty(x.iconResource(HabType.Station))
			group x by x.iconResource(HabType.Station) into x
			select x.FirstOrDefault<TIHabModuleTemplate>()).ToDictionary<TIHabModuleTemplate, string, Sprite>((TIHabModuleTemplate x) => x.iconResource(HabType.Station), (TIHabModuleTemplate x) => GameControl.assetLoader.LoadAsset<Sprite>(x.iconResource(HabType.Station)));

		// Token: 0x04001DC5 RID: 7621
		public static readonly Dictionary<string, Sprite> habBaseModuleIcons = (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
			where !string.IsNullOrEmpty(x.iconResource(HabType.Base))
			group x by x.iconResource(HabType.Base) into x
			select x.FirstOrDefault<TIHabModuleTemplate>()).ToDictionary<TIHabModuleTemplate, string, Sprite>((TIHabModuleTemplate x) => x.iconResource(HabType.Base), (TIHabModuleTemplate x) => GameControl.assetLoader.LoadAsset<Sprite>(x.iconResource(HabType.Base)));

		// Token: 0x04001DC6 RID: 7622
		public static readonly Dictionary<string, GameObject> constructionModulePrefabs = new Dictionary<string, GameObject>
		{
			{
				TemplateManager.global.station_human_underconstruction_t1_module,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_human_underconstruction_t1_module)
			},
			{
				TemplateManager.global.station_human_underconstruction_t2_module,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_human_underconstruction_t2_module)
			},
			{
				TemplateManager.global.station_human_underconstruction_t3_module,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_human_underconstruction_t3_module)
			},
			{
				TemplateManager.global.station_alien_underconstruction_t1_module,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_alien_underconstruction_t1_module)
			},
			{
				TemplateManager.global.station_alien_underconstruction_t2_module,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_alien_underconstruction_t2_module)
			},
			{
				TemplateManager.global.station_alien_underconstruction_t3_module,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_alien_underconstruction_t3_module)
			},
			{
				TemplateManager.global.station_human_underconstruction_t1_module_destruction,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_human_underconstruction_t1_module_destruction)
			},
			{
				TemplateManager.global.station_human_underconstruction_t2_module_destruction,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_human_underconstruction_t2_module_destruction)
			},
			{
				TemplateManager.global.station_human_underconstruction_t3_module_destruction,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_human_underconstruction_t3_module_destruction)
			},
			{
				TemplateManager.global.station_alien_underconstruction_t1_module_destruction,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_alien_underconstruction_t1_module_destruction)
			},
			{
				TemplateManager.global.station_alien_underconstruction_t2_module_destruction,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_alien_underconstruction_t2_module_destruction)
			},
			{
				TemplateManager.global.station_alien_underconstruction_t3_module_destruction,
				GameControl.assetLoader.LoadAsset<GameObject>(TemplateManager.global.station_alien_underconstruction_t3_module_destruction)
			}
		};

		// Token: 0x04001DC7 RID: 7623
		public static readonly Dictionary<string, GameObject> destructionSequencePrefabs = new Dictionary<string, GameObject>
		{
			{
				"habmodules/stationdestruction_T1_generic",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T1_generic")
			},
			{
				"habmodules/stationdestruction_T1_large",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T1_large")
			},
			{
				"habmodules/stationdestruction_T1_solar",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T1_solar")
			},
			{
				"habmodules/stationdestruction_T2_generic",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T2_generic")
			},
			{
				"habmodules/stationdestruction_T2_large",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T2_large")
			},
			{
				"habmodules/stationdestruction_T3_generic",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T3_generic")
			},
			{
				"habmodules/stationdestruction_T3_large",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_T3_large")
			},
			{
				"habmodules/stationdestruction_alien_T1_generic",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_alien_T1_generic")
			},
			{
				"habmodules/stationdestruction_alien_T2_generic",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_alien_T2_generic")
			},
			{
				"habmodules/stationdestruction_alien_T3_generic",
				GameControl.assetLoader.LoadAsset<GameObject>("habmodules/stationdestruction_alien_T3_generic")
			}
		};

		// Token: 0x04001DC8 RID: 7624
		public static readonly GameObject stationCouncilorMarker = GameControl.assetLoader.LoadAsset<GameObject>("ui/StationCouncilorMarker");

		// Token: 0x04001DC9 RID: 7625
		public static readonly Dictionary<string, GameObject> vectorThrusterFXPrefabs = new Dictionary<string, GameObject>
		{
			{
				"ships/AlienThrusterVector",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/AlienThrusterVector")
			},
			{
				"ships/HumanThrusterVectorAdvanced",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThrusterVectorAdvanced")
			},
			{
				"ships/HumanThrusterVectorBasic",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThrusterVectorBasic")
			}
		};

		// Token: 0x04001DCA RID: 7626
		public static readonly Dictionary<string, GameObject> thrusterFXPrefabs = new Dictionary<string, GameObject>
		{
			{
				"ships/HumanThrusterBasic",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThrusterBasic")
			},
			{
				"ships/HumanThruster_Chemical",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_Chemical")
			},
			{
				"ships/HumanThruster_NuclearSalt",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_NuclearSalt")
			},
			{
				"ships/HumanThruster_Hydrogen",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_Hydrogen")
			},
			{
				"ships/HumanThruster_MassDriver",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_MassDriver")
			},
			{
				"ships/HumanThruster_FissionFrag",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_FissionFrag")
			},
			{
				"ships/HumanThruster_Noble",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_Noble")
			},
			{
				"ships/HumanThruster_ReactionProducts",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_ReactionProducts")
			},
			{
				"ships/HumanThruster_Fission",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_Fission")
			},
			{
				"ships/HumanThruster_PCT",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/HumanThruster_PCT")
			},
			{
				"ships/AlienThruster",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/AlienThruster")
			},
			{
				"ships/NuclearThruster",
				GameControl.assetLoader.LoadAsset<GameObject>("ships/NuclearThruster")
			}
		};
	}
}
