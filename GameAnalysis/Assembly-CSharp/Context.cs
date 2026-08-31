using System;

// Token: 0x0200016C RID: 364
public enum Context
{
	// Token: 0x04000312 RID: 786
	None,
	// Token: 0x04000313 RID: 787
	CanContactAliens,
	// Token: 0x04000314 RID: 788
	AlienRelationsEstablished,
	// Token: 0x04000315 RID: 789
	CanTransferTerritoryToAliens,
	// Token: 0x04000316 RID: 790
	DetectAliensOnEarth,
	// Token: 0x04000317 RID: 791
	DetectAlienActivity,
	// Token: 0x04000318 RID: 792
	CanCaptureHydra,
	// Token: 0x04000319 RID: 793
	DetectAlienSpaceAssetsRange,
	// Token: 0x0400031A RID: 794
	PherocyteResistance,
	// Token: 0x0400031B RID: 795
	ManyAliensOnEarth,
	// Token: 0x0400031C RID: 796
	CouncilSize,
	// Token: 0x0400031D RID: 797
	NewCouncilorRecruitXP,
	// Token: 0x0400031E RID: 798
	MaxAvailableOrgs,
	// Token: 0x0400031F RID: 799
	OrgPurchaseCost,
	// Token: 0x04000320 RID: 800
	HumanLifespan,
	// Token: 0x04000321 RID: 801
	AdvancedAircraft,
	// Token: 0x04000322 RID: 802
	AllRecruitStats,
	// Token: 0x04000323 RID: 803
	AllRecruitTraits,
	// Token: 0x04000324 RID: 804
	TraitSpawnChance,
	// Token: 0x04000325 RID: 805
	AugmentationMoneyCost,
	// Token: 0x04000326 RID: 806
	ControlPointMaintenance,
	// Token: 0x04000327 RID: 807
	ConsolidatePowerDurationMultiplier,
	// Token: 0x04000328 RID: 808
	BilateralRelationsCooldownMultiplier,
	// Token: 0x04000329 RID: 809
	ProjectUnlockChance,
	// Token: 0x0400032A RID: 810
	MonthlyProjectTriggerChance,
	// Token: 0x0400032B RID: 811
	EconomyPriority,
	// Token: 0x0400032C RID: 812
	Economy_BasePCGDPIncrease,
	// Token: 0x0400032D RID: 813
	Economy_CoreEcoPCGDPMultiplier,
	// Token: 0x0400032E RID: 814
	Economy_ResourcePCGDPMultiplier,
	// Token: 0x0400032F RID: 815
	Economy_InequalityMultiplier,
	// Token: 0x04000330 RID: 816
	WelfarePriority,
	// Token: 0x04000331 RID: 817
	WelfareInequalityReductionBonus,
	// Token: 0x04000332 RID: 818
	EnvironmentPriority,
	// Token: 0x04000333 RID: 819
	Environment_SustainabilityChange,
	// Token: 0x04000334 RID: 820
	Welfare_CO2_ppm,
	// Token: 0x04000335 RID: 821
	Welfare_CH4_ppm,
	// Token: 0x04000336 RID: 822
	Welfare_N2O_ppm,
	// Token: 0x04000337 RID: 823
	Environment_BestSustainabilityValue,
	// Token: 0x04000338 RID: 824
	KnowledgePriority,
	// Token: 0x04000339 RID: 825
	GovernmentPriority,
	// Token: 0x0400033A RID: 826
	UnityPriority,
	// Token: 0x0400033B RID: 827
	OppressionPriority,
	// Token: 0x0400033C RID: 828
	SpoilsPriority,
	// Token: 0x0400033D RID: 829
	SpoilsOutput,
	// Token: 0x0400033E RID: 830
	SpaceDevPriority,
	// Token: 0x0400033F RID: 831
	SpaceflightPriority,
	// Token: 0x04000340 RID: 832
	LaunchFacilitiesPriority,
	// Token: 0x04000341 RID: 833
	MissionControlPriority,
	// Token: 0x04000342 RID: 834
	MilitaryPriority,
	// Token: 0x04000343 RID: 835
	BuildArmyPriority,
	// Token: 0x04000344 RID: 836
	UpgradeArmyPriority,
	// Token: 0x04000345 RID: 837
	BuildNuclearWeaponsPriority,
	// Token: 0x04000346 RID: 838
	BuildSpaceDefensesPriority,
	// Token: 0x04000347 RID: 839
	BuildSTOSquadronPriority,
	// Token: 0x04000348 RID: 840
	DirectInvestGlobalDiscount_Money_PCT,
	// Token: 0x04000349 RID: 841
	DirectInvestGlobalDiscount_Influence_PCT,
	// Token: 0x0400034A RID: 842
	DirectInvestGlobalDiscount_Ops_PCT,
	// Token: 0x0400034B RID: 843
	ControlPointResearch,
	// Token: 0x0400034C RID: 844
	PublicOpinionInfluence,
	// Token: 0x0400034D RID: 845
	DetectHumanCouncilorsOnEarth,
	// Token: 0x0400034E RID: 846
	MaxMissionSliderSteps,
	// Token: 0x0400034F RID: 847
	Mission_GainControlPoint,
	// Token: 0x04000350 RID: 848
	Mission_GainInfluence_Att,
	// Token: 0x04000351 RID: 849
	Mission_Propaganda_Att,
	// Token: 0x04000352 RID: 850
	Mission_Terrorize_Def,
	// Token: 0x04000353 RID: 851
	Mission_Abductions_Att,
	// Token: 0x04000354 RID: 852
	Mission_Abductions_Def,
	// Token: 0x04000355 RID: 853
	Mission_EnthrallElites_Def,
	// Token: 0x04000356 RID: 854
	Mission_EnthrallPublic_Def,
	// Token: 0x04000357 RID: 855
	Mission_AssaultAlienAsset_Att,
	// Token: 0x04000358 RID: 856
	Mission_Xenoform_Def,
	// Token: 0x04000359 RID: 857
	Mission_HostileTakeover_Att,
	// Token: 0x0400035A RID: 858
	Mission_HostileTakeover_Def,
	// Token: 0x0400035B RID: 859
	Mission_SabotageProject_Att,
	// Token: 0x0400035C RID: 860
	Mission_SabotageProject_Def,
	// Token: 0x0400035D RID: 861
	Mission_StealProject_Att,
	// Token: 0x0400035E RID: 862
	Mission_StealProject_Def,
	// Token: 0x0400035F RID: 863
	Mission_InvestigateCouncilor_Att,
	// Token: 0x04000360 RID: 864
	Mission_Inspire_Att,
	// Token: 0x04000361 RID: 865
	Mission_Detain_Att,
	// Token: 0x04000362 RID: 866
	Mission_Detain_Def,
	// Token: 0x04000363 RID: 867
	Mission_Assassination_Att,
	// Token: 0x04000364 RID: 868
	Mission_Assassinate_Def,
	// Token: 0x04000365 RID: 869
	Mission_Coup_Att,
	// Token: 0x04000366 RID: 870
	Mission_Unrest_Att,
	// Token: 0x04000367 RID: 871
	Mission_Stabilize_Att,
	// Token: 0x04000368 RID: 872
	Mission_Turn_Att,
	// Token: 0x04000369 RID: 873
	Mission_Crackdown_Att,
	// Token: 0x0400036A RID: 874
	Mission_Crackdown_Def,
	// Token: 0x0400036B RID: 875
	Mission_Purge_Att,
	// Token: 0x0400036C RID: 876
	Mission_Purge_Def,
	// Token: 0x0400036D RID: 877
	Mission_SabotageFacilities_Att,
	// Token: 0x0400036E RID: 878
	Mission_SabotageFacilities_Def,
	// Token: 0x0400036F RID: 879
	Mission_SabotageHabModule_Att,
	// Token: 0x04000370 RID: 880
	Mission_SabotageHabModule_Def,
	// Token: 0x04000371 RID: 881
	Mission_ControlAsset_Def,
	// Token: 0x04000372 RID: 882
	Mission_SeizeSpaceAsset_Att,
	// Token: 0x04000373 RID: 883
	Mission_SeizeSpaceAsset_Def,
	// Token: 0x04000374 RID: 884
	PublicCampaignStrength,
	// Token: 0x04000375 RID: 885
	DefendInterestEarthDuration,
	// Token: 0x04000376 RID: 886
	DefendInterestsValue,
	// Token: 0x04000377 RID: 887
	InterrogationBonus,
	// Token: 0x04000378 RID: 888
	Corruption,
	// Token: 0x04000379 RID: 889
	BreakawayChance,
	// Token: 0x0400037A RID: 890
	AtrocityMitigation,
	// Token: 0x0400037B RID: 891
	PostRegimeChangeUnrestReduction,
	// Token: 0x0400037C RID: 892
	PostRegimeLossUnrestIncrease,
	// Token: 0x0400037D RID: 893
	ArmyUnrestReductionImpact,
	// Token: 0x0400037E RID: 894
	ArmyHealRate,
	// Token: 0x0400037F RID: 895
	ArmyUrbanWarfare,
	// Token: 0x04000380 RID: 896
	ArmyRuggedWarfare,
	// Token: 0x04000381 RID: 897
	XenoformingDestructionStrength,
	// Token: 0x04000382 RID: 898
	ArmyDamageBonustoMegafauna,
	// Token: 0x04000383 RID: 899
	ArmyDamageBonustoInvaderArmy,
	// Token: 0x04000384 RID: 900
	ArmyDamageBonustoHumanArmy,
	// Token: 0x04000385 RID: 901
	ArmyDamageBonustoAllArmies,
	// Token: 0x04000386 RID: 902
	MegafaunaDamageMitigation,
	// Token: 0x04000387 RID: 903
	MegafaunaRepellent,
	// Token: 0x04000388 RID: 904
	MegafaunaMastery,
	// Token: 0x04000389 RID: 905
	ArmyNuclearHardening,
	// Token: 0x0400038A RID: 906
	NuclearStrikeDamageReduction,
	// Token: 0x0400038B RID: 907
	BombardmentArmyDefenseBonus,
	// Token: 0x0400038C RID: 908
	LaserDefenseType,
	// Token: 0x0400038D RID: 909
	LaserDefenseFreq,
	// Token: 0x0400038E RID: 910
	MCFreeSpaceMineNetwork,
	// Token: 0x0400038F RID: 911
	HabMissionControlReduction,
	// Token: 0x04000390 RID: 912
	ShipMissionControlReduction,
	// Token: 0x04000391 RID: 913
	AlienHateFromMCUsage,
	// Token: 0x04000392 RID: 914
	MissionControlDisruption_PCT,
	// Token: 0x04000393 RID: 915
	CanFoundTier1Station,
	// Token: 0x04000394 RID: 916
	CanFoundTier2Station,
	// Token: 0x04000395 RID: 917
	CanFoundTier3Station,
	// Token: 0x04000396 RID: 918
	CanFoundTier1Base,
	// Token: 0x04000397 RID: 919
	CanFoundTier2Base,
	// Token: 0x04000398 RID: 920
	CanFoundTier3Base,
	// Token: 0x04000399 RID: 921
	CanFoundAutomatedT1Station,
	// Token: 0x0400039A RID: 922
	CanFoundAutomatedT1Base,
	// Token: 0x0400039B RID: 923
	GenericModuleTransferTime,
	// Token: 0x0400039C RID: 924
	GenericTransferEV_kps,
	// Token: 0x0400039D RID: 925
	GenericTransfer_OffDate_PCT,
	// Token: 0x0400039E RID: 926
	ProbeTransferTime,
	// Token: 0x0400039F RID: 927
	InnerExplorationRange_AU,
	// Token: 0x040003A0 RID: 928
	ExploreLuna,
	// Token: 0x040003A1 RID: 929
	ExploreEarthLagrangePoints,
	// Token: 0x040003A2 RID: 930
	OuterExplorationRange_AU,
	// Token: 0x040003A3 RID: 931
	SemiMajorAxisExplorationRange_AU,
	// Token: 0x040003A4 RID: 932
	SpaceCameraResolution,
	// Token: 0x040003A5 RID: 933
	HabResearchProduction,
	// Token: 0x040003A6 RID: 934
	HabNuclearFreighters,
	// Token: 0x040003A7 RID: 935
	BombardmentHabDefenseBonus,
	// Token: 0x040003A8 RID: 936
	CanAmassAntimatter,
	// Token: 0x040003A9 RID: 937
	CanAmassExotics,
	// Token: 0x040003AA RID: 938
	SpaceMiningBonus,
	// Token: 0x040003AB RID: 939
	MiningWaterBonus,
	// Token: 0x040003AC RID: 940
	MiningVolatilesBonus,
	// Token: 0x040003AD RID: 941
	MiningMetalsBonus,
	// Token: 0x040003AE RID: 942
	MiningNoblesBonus,
	// Token: 0x040003AF RID: 943
	MiningFissilesBonus,
	// Token: 0x040003B0 RID: 944
	ResourceMarketSales,
	// Token: 0x040003B1 RID: 945
	ShipConstructionTime,
	// Token: 0x040003B2 RID: 946
	ShipOfficerPromotion,
	// Token: 0x040003B3 RID: 947
	Ship_MaxSurvivableCombatAcceleration_Bonus,
	// Token: 0x040003B4 RID: 948
	Ship_MaxSurvivableCruiseAcceleration_Bonus,
	// Token: 0x040003B5 RID: 949
	DockyardRepairSpeed,
	// Token: 0x040003B6 RID: 950
	ShipLaserDamage,
	// Token: 0x040003B7 RID: 951
	ParticleLaserDamage,
	// Token: 0x040003B8 RID: 952
	ShipMagDamage,
	// Token: 0x040003B9 RID: 953
	ShipConvMissileDamage,
	// Token: 0x040003BA RID: 954
	BonusDamagetoAlienShips,
	// Token: 0x040003BB RID: 955
	GlobalECMBonus,
	// Token: 0x040003BC RID: 956
	STOFighterECM,
	// Token: 0x040003BD RID: 957
	HumanECMAgainstAliens,
	// Token: 0x040003BE RID: 958
	TargetingComputerBonus,
	// Token: 0x040003BF RID: 959
	GlobalTargetingBonus,
	// Token: 0x040003C0 RID: 960
	MissileECM,
	// Token: 0x040003C1 RID: 961
	DamageReductionAgainstAllShips,
	// Token: 0x040003C2 RID: 962
	DamageReductionAgainstAlienShips,
	// Token: 0x040003C3 RID: 963
	ShipLaserCooldownTime,
	// Token: 0x040003C4 RID: 964
	ShipLaserEfficiencyBonus,
	// Token: 0x040003C5 RID: 965
	ShipMagWeaponEfficiencyBonus,
	// Token: 0x040003C6 RID: 966
	ShipMagRechargeTime,
	// Token: 0x040003C7 RID: 967
	ShipWeaponTargetingRange,
	// Token: 0x040003C8 RID: 968
	Combat_ShipRepairSpeed,
	// Token: 0x040003C9 RID: 969
	SpaceAssaultBonus,
	// Token: 0x040003CA RID: 970
	BombardmentDamageBonus,
	// Token: 0x040003CB RID: 971
	HabConstructionSpeed,
	// Token: 0x040003CC RID: 972
	MineSizeModifier,
	// Token: 0x040003CD RID: 973
	MaterialScience,
	// Token: 0x040003CE RID: 974
	SpaceScience,
	// Token: 0x040003CF RID: 975
	EnergyScience,
	// Token: 0x040003D0 RID: 976
	LifeScience,
	// Token: 0x040003D1 RID: 977
	MilitaryScience,
	// Token: 0x040003D2 RID: 978
	InformationScience,
	// Token: 0x040003D3 RID: 979
	SocialScience,
	// Token: 0x040003D4 RID: 980
	Xenology,
	// Token: 0x040003D5 RID: 981
	MiningOutputPerformanceRecord,
	// Token: 0x040003D6 RID: 982
	BuddyMovie,
	// Token: 0x040003D7 RID: 983
	FakedDeath,
	// Token: 0x040003D8 RID: 984
	GlobalFissionTechLevel,
	// Token: 0x040003D9 RID: 985
	GlobalFusionTechLevel,
	// Token: 0x040003DA RID: 986
	MarkedChain,
	// Token: 0x040003DB RID: 987
	MarkedCombatChain,
	// Token: 0x040003DC RID: 988
	InfiltratedCombatChain,
	// Token: 0x040003DD RID: 989
	InfiltratedMoneyChain,
	// Token: 0x040003DE RID: 990
	InfiltratedInfluenceChain,
	// Token: 0x040003DF RID: 991
	InfiltratedChain,
	// Token: 0x040003E0 RID: 992
	InfiltratedCount,
	// Token: 0x040003E1 RID: 993
	VaccineChips,
	// Token: 0x040003E2 RID: 994
	InfluenceLies,
	// Token: 0x040003E3 RID: 995
	PublicizedAlienThreat,
	// Token: 0x040003E4 RID: 996
	DemocracyBackslides,
	// Token: 0x040003E5 RID: 997
	OldWorldArtifacts
}
