using System;

// Token: 0x0200016D RID: 365
public enum InstantEffect
{
	// Token: 0x040003E7 RID: 999
	None,
	// Token: 0x040003E8 RID: 1000
	DummyInstantEffect,
	// Token: 0x040003E9 RID: 1001
	Propaganda,
	// Token: 0x040003EA RID: 1002
	Propaganda_Faction,
	// Token: 0x040003EB RID: 1003
	Propaganda_PerOwnedCP,
	// Token: 0x040003EC RID: 1004
	Propaganda_AllFactionsWithCP,
	// Token: 0x040003ED RID: 1005
	Propaganda_Region,
	// Token: 0x040003EE RID: 1006
	SpaceScan,
	// Token: 0x040003EF RID: 1007
	DamageRegions,
	// Token: 0x040003F0 RID: 1008
	DamageRegionBoost,
	// Token: 0x040003F1 RID: 1009
	DamageRegions_Nuclear,
	// Token: 0x040003F2 RID: 1010
	NationProsperity,
	// Token: 0x040003F3 RID: 1011
	NationRecession,
	// Token: 0x040003F4 RID: 1012
	FreePriorityWelfare,
	// Token: 0x040003F5 RID: 1013
	FreePriorityEnvironment,
	// Token: 0x040003F6 RID: 1014
	FreePriorityKnowledge,
	// Token: 0x040003F7 RID: 1015
	FreePriorityMilitary,
	// Token: 0x040003F8 RID: 1016
	FreePriorityFunding,
	// Token: 0x040003F9 RID: 1017
	FreePriorityBoost,
	// Token: 0x040003FA RID: 1018
	RerollMissingProjects,
	// Token: 0x040003FB RID: 1019
	XenoformingChange,
	// Token: 0x040003FC RID: 1020
	Exposure,
	// Token: 0x040003FD RID: 1021
	Exposure_SingleCouncilor,
	// Token: 0x040003FE RID: 1022
	NationDemocracyChange,
	// Token: 0x040003FF RID: 1023
	NationDemocracyChange_PopScaled,
	// Token: 0x04000400 RID: 1024
	NationMiltechChange,
	// Token: 0x04000401 RID: 1025
	NationMiltechChange_ReduceExcess,
	// Token: 0x04000402 RID: 1026
	NationCohesionChange,
	// Token: 0x04000403 RID: 1027
	NationCohesionChange_ToExtreme,
	// Token: 0x04000404 RID: 1028
	NationCohesionChange_PopScaled,
	// Token: 0x04000405 RID: 1029
	NationCohesionChange_ToExtreme_PopScaled,
	// Token: 0x04000406 RID: 1030
	NationEducationChange,
	// Token: 0x04000407 RID: 1031
	NationEducationChange_PopScaled,
	// Token: 0x04000408 RID: 1032
	NationUnrestChange,
	// Token: 0x04000409 RID: 1033
	NationUnrestChange_FactionCredit,
	// Token: 0x0400040A RID: 1034
	NationUnrestChange_PopScaled,
	// Token: 0x0400040B RID: 1035
	NationUnrestChange_FactionCredit_PopScaled,
	// Token: 0x0400040C RID: 1036
	NationGDPPctChange,
	// Token: 0x0400040D RID: 1037
	RegionGDPPctChange,
	// Token: 0x0400040E RID: 1038
	RegionGDPPctChange_StrValue,
	// Token: 0x0400040F RID: 1039
	MapRegionGDPPctChange_StrValue,
	// Token: 0x04000410 RID: 1040
	CPVariableNationGDPPctChange,
	// Token: 0x04000411 RID: 1041
	AllFactionNationsGDPPctChange,
	// Token: 0x04000412 RID: 1042
	NationInequalityChange,
	// Token: 0x04000413 RID: 1043
	NationInequalityChange_PopScaled,
	// Token: 0x04000414 RID: 1044
	NationMaxMiltechChange,
	// Token: 0x04000415 RID: 1045
	NationSetCanBuildSpaceDefenses,
	// Token: 0x04000416 RID: 1046
	NationSetCanBuildSTOFighters,
	// Token: 0x04000417 RID: 1047
	NationSetCanDecontaminateRegion,
	// Token: 0x04000418 RID: 1048
	NationNukesChange,
	// Token: 0x04000419 RID: 1049
	NationAnnualSpaceFundingChange,
	// Token: 0x0400041A RID: 1050
	NationPopGrowthModifierChange,
	// Token: 0x0400041B RID: 1051
	RegionRevealAlienActivities,
	// Token: 0x0400041C RID: 1052
	GlobalCO2Change_ppm,
	// Token: 0x0400041D RID: 1053
	GlobalCH4Change_ppm,
	// Token: 0x0400041E RID: 1054
	GlobalN2OChange_ppm,
	// Token: 0x0400041F RID: 1055
	GlobalStratosphericAerosolsChange_ppm,
	// Token: 0x04000420 RID: 1056
	GlobalSeaLevelChange_cm,
	// Token: 0x04000421 RID: 1057
	GlobalLooseNukesChange,
	// Token: 0x04000422 RID: 1058
	GlobalSeasonalOceansToPermanent,
	// Token: 0x04000423 RID: 1059
	Atrocity,
	// Token: 0x04000424 RID: 1060
	GainOpenControlPoint,
	// Token: 0x04000425 RID: 1061
	GainAnyControlPoint,
	// Token: 0x04000426 RID: 1062
	GainAnyControlPoint_Plus,
	// Token: 0x04000427 RID: 1063
	ReassignControlPointOfTypeByPopularity,
	// Token: 0x04000428 RID: 1064
	RedistributeControlPointsByPopularity_Individual,
	// Token: 0x04000429 RID: 1065
	GainControlPointOfType,
	// Token: 0x0400042A RID: 1066
	LoseArmyControlPoint,
	// Token: 0x0400042B RID: 1067
	CrackdownArmyControlPoint,
	// Token: 0x0400042C RID: 1068
	DefendAllOwnedControlPoints,
	// Token: 0x0400042D RID: 1069
	GainMoneyIncome,
	// Token: 0x0400042E RID: 1070
	GainInfluenceIncome,
	// Token: 0x0400042F RID: 1071
	GainOpsIncome,
	// Token: 0x04000430 RID: 1072
	GainBoostIncome,
	// Token: 0x04000431 RID: 1073
	GainResearchIncome,
	// Token: 0x04000432 RID: 1074
	GainMissionControl,
	// Token: 0x04000433 RID: 1075
	GainWaterIncome,
	// Token: 0x04000434 RID: 1076
	GainVolatilesIncome,
	// Token: 0x04000435 RID: 1077
	GainMetalsIncome,
	// Token: 0x04000436 RID: 1078
	GainNoblesIncome,
	// Token: 0x04000437 RID: 1079
	GainFissilesIncome,
	// Token: 0x04000438 RID: 1080
	GainAntimatterIncome,
	// Token: 0x04000439 RID: 1081
	GainExoticsIncome,
	// Token: 0x0400043A RID: 1082
	UpgradeRelations,
	// Token: 0x0400043B RID: 1083
	DowngradeRelations,
	// Token: 0x0400043C RID: 1084
	DeclareLimitedWar,
	// Token: 0x0400043D RID: 1085
	DeclareFullWar,
	// Token: 0x0400043E RID: 1086
	JoinPrimaryFederation,
	// Token: 0x0400043F RID: 1087
	JoinSecondaryFederation,
	// Token: 0x04000440 RID: 1088
	LoseUndefendedControlPoint,
	// Token: 0x04000441 RID: 1089
	LoseUndefendedControlPoint_Plus,
	// Token: 0x04000442 RID: 1090
	DamageNationalSpaceAssets,
	// Token: 0x04000443 RID: 1091
	RegionPopulationPctChange,
	// Token: 0x04000444 RID: 1092
	RegionPopulationPctChange_WealthMitigation,
	// Token: 0x04000445 RID: 1093
	RegionTransferPopulationPctToSecondary,
	// Token: 0x04000446 RID: 1094
	RegionAbductionsChange,
	// Token: 0x04000447 RID: 1095
	NationAbductionsChange,
	// Token: 0x04000448 RID: 1096
	RegionNuclearDetonationsChange,
	// Token: 0x04000449 RID: 1097
	GainFactionSpaceOrg,
	// Token: 0x0400044A RID: 1098
	PrimaryOccupiesSecondary,
	// Token: 0x0400044B RID: 1099
	SecondaryOccupiesPrimary,
	// Token: 0x0400044C RID: 1100
	PrimaryRegimeChangesSecondary,
	// Token: 0x0400044D RID: 1101
	SecondaryRegimeChangesPrimary,
	// Token: 0x0400044E RID: 1102
	PrimaryAbsorbsSecondary,
	// Token: 0x0400044F RID: 1103
	SecondaryAbsorbsPrimary,
	// Token: 0x04000450 RID: 1104
	SecondaryAnnexesPrimary,
	// Token: 0x04000451 RID: 1105
	RandomSecession,
	// Token: 0x04000452 RID: 1106
	NationBreaksUp,
	// Token: 0x04000453 RID: 1107
	Coup,
	// Token: 0x04000454 RID: 1108
	GlobalIPProduction,
	// Token: 0x04000455 RID: 1109
	RandomFactionNationGainsRandomClaim,
	// Token: 0x04000456 RID: 1110
	NationLosesClaim,
	// Token: 0x04000457 RID: 1111
	NationMoveCapitalToSecondaryOwnedRegion,
	// Token: 0x04000458 RID: 1112
	DestroyRandomModules,
	// Token: 0x04000459 RID: 1113
	DestroyRandomModules_Marines,
	// Token: 0x0400045A RID: 1114
	DestroyRandomModules_Power,
	// Token: 0x0400045B RID: 1115
	DestroyRandomNumberOfModules,
	// Token: 0x0400045C RID: 1116
	DestroyHabSector,
	// Token: 0x0400045D RID: 1117
	DestroyHab,
	// Token: 0x0400045E RID: 1118
	HabDefectsToSecondary,
	// Token: 0x0400045F RID: 1119
	DestroyShip,
	// Token: 0x04000460 RID: 1120
	ShipDefectsToSecondary,
	// Token: 0x04000461 RID: 1121
	DamageShipParts_Marines,
	// Token: 0x04000462 RID: 1122
	DamageShipParts_SpecifiedUtilityModule,
	// Token: 0x04000463 RID: 1123
	ShipNuclearTorpedoMagazineChange,
	// Token: 0x04000464 RID: 1124
	ShipFreeOfficerCreation,
	// Token: 0x04000465 RID: 1125
	ShipFreeOfficerPromotion,
	// Token: 0x04000466 RID: 1126
	SpawnSpaceFleet_StrValue,
	// Token: 0x04000467 RID: 1127
	OfficerPromoted,
	// Token: 0x04000468 RID: 1128
	FreeDaysSpaceResourceIncome,
	// Token: 0x04000469 RID: 1129
	LoseDaysSpaceResourceIncome,
	// Token: 0x0400046A RID: 1130
	ModifyHabMiningResourceIncomes,
	// Token: 0x0400046B RID: 1131
	FreeMonthsHabMiningResourceIncome,
	// Token: 0x0400046C RID: 1132
	OrbitDestroyedAssetsChange,
	// Token: 0x0400046D RID: 1133
	LEODestroyedAssetsChange,
	// Token: 0x0400046E RID: 1134
	SpawnMegafaunaArmies,
	// Token: 0x0400046F RID: 1135
	SpawnMegafaunaArmyDamaged,
	// Token: 0x04000470 RID: 1136
	AlienSpaceResourceSharing,
	// Token: 0x04000471 RID: 1137
	CouncilorGainsXP,
	// Token: 0x04000472 RID: 1138
	CouncilorGainsTrait,
	// Token: 0x04000473 RID: 1139
	FactionAllCouncilorsModifyAttribute,
	// Token: 0x04000474 RID: 1140
	FactionAllCouncilorsGainTrait,
	// Token: 0x04000475 RID: 1141
	FactionAllEligibleCouncilorsGainXP,
	// Token: 0x04000476 RID: 1142
	GainExoticsFromSpaceIndustry,
	// Token: 0x04000477 RID: 1143
	FactionAllEligibleCouncilorsGainTrait,
	// Token: 0x04000478 RID: 1144
	CouncilorLosesTrait,
	// Token: 0x04000479 RID: 1145
	CouncilorGainsTraitGroup,
	// Token: 0x0400047A RID: 1146
	CouncilorLosesTraitGroup,
	// Token: 0x0400047B RID: 1147
	CouncilorDetained,
	// Token: 0x0400047C RID: 1148
	CouncilorKilled,
	// Token: 0x0400047D RID: 1149
	CouncilorKilled_NoProtection,
	// Token: 0x0400047E RID: 1150
	CouncilorKilled_NoProtection_Nonviolent,
	// Token: 0x0400047F RID: 1151
	CouncilorInHiding,
	// Token: 0x04000480 RID: 1152
	CouncilorModifyAttribute,
	// Token: 0x04000481 RID: 1153
	CouncilorHomeNationPropaganda,
	// Token: 0x04000482 RID: 1154
	CouncilorHomeNationsImproveRelations,
	// Token: 0x04000483 RID: 1155
	CouncilorLosesOrgs,
	// Token: 0x04000484 RID: 1156
	CouncilorInitializeUnique_StrValue,
	// Token: 0x04000485 RID: 1157
	FactionInvestigationsChange,
	// Token: 0x04000486 RID: 1158
	ArmyStrengthChange,
	// Token: 0x04000487 RID: 1159
	DecreaseRegionOccupations,
	// Token: 0x04000488 RID: 1160
	DamageAllRegionArmies,
	// Token: 0x04000489 RID: 1161
	DamageAllRegionArmies_Enemy,
	// Token: 0x0400048A RID: 1162
	DamageAllNationArmies,
	// Token: 0x0400048B RID: 1163
	EndofOil,
	// Token: 0x0400048C RID: 1164
	GlobalCrackdown,
	// Token: 0x0400048D RID: 1165
	RegionCreateCoreEco_InputState,
	// Token: 0x0400048E RID: 1166
	RegionCreateCoreEco_StrValue,
	// Token: 0x0400048F RID: 1167
	RegionCreateResource_InputState,
	// Token: 0x04000490 RID: 1168
	RegionCreateResource_StrValue,
	// Token: 0x04000491 RID: 1169
	RegionCreateOilResource_InputState,
	// Token: 0x04000492 RID: 1170
	RegionCreateOilResource_StrValue,
	// Token: 0x04000493 RID: 1171
	RegionAccumulateCoreEconomyTriggers,
	// Token: 0x04000494 RID: 1172
	RegionAccumulateCoreOilTriggers,
	// Token: 0x04000495 RID: 1173
	RegionAccumulateCoreMiningTriggers,
	// Token: 0x04000496 RID: 1174
	RegionAccumulateDecontaminateTriggers,
	// Token: 0x04000497 RID: 1175
	RegionAccumulateDecolonizeTriggers,
	// Token: 0x04000498 RID: 1176
	EnergyCrisis,
	// Token: 0x04000499 RID: 1177
	TriggerNarrativeEvent_StrValue,
	// Token: 0x0400049A RID: 1178
	UpdateAlienThreatMeter,
	// Token: 0x0400049B RID: 1179
	UpdateAlienThreatMeter_Accurate,
	// Token: 0x0400049C RID: 1180
	SetAlienHate,
	// Token: 0x0400049D RID: 1181
	GainAlienHate,
	// Token: 0x0400049E RID: 1182
	LoseAlienHate,
	// Token: 0x0400049F RID: 1183
	RemoveEffectFromFaction,
	// Token: 0x040004A0 RID: 1184
	CompleteCampaignMilestone,
	// Token: 0x040004A1 RID: 1185
	BSBE_1stPlaceInTheSpaceRace,
	// Token: 0x040004A2 RID: 1186
	BSBE_2ndPlaceInTheSpaceRace,
	// Token: 0x040004A3 RID: 1187
	BSBE_3rdPlaceInTheSpaceRace
}
