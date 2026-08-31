using System;

// Token: 0x020002CD RID: 717
public enum EffectSecondaryStateType
{
	// Token: 0x040008E8 RID: 2280
	none,
	// Token: 0x040008E9 RID: 2281
	InputState,
	// Token: 0x040008EA RID: 2282
	Nation_FederationMember,
	// Token: 0x040008EB RID: 2283
	Nation_FederationLeader,
	// Token: 0x040008EC RID: 2284
	Nation_Ally,
	// Token: 0x040008ED RID: 2285
	Nation_NonAlly,
	// Token: 0x040008EE RID: 2286
	Nation_LargerAlly,
	// Token: 0x040008EF RID: 2287
	Nation_Ally_HigherMiltech,
	// Token: 0x040008F0 RID: 2288
	Nation_AllyOfCouncilorHomeNation,
	// Token: 0x040008F1 RID: 2289
	Nation_NormalRelations,
	// Token: 0x040008F2 RID: 2290
	Nation_AllyOrRival,
	// Token: 0x040008F3 RID: 2291
	Nation_Rival,
	// Token: 0x040008F4 RID: 2292
	Nation_Rival_NMF,
	// Token: 0x040008F5 RID: 2293
	Nation_NonRival,
	// Token: 0x040008F6 RID: 2294
	Nation_SmallerRival_NMF,
	// Token: 0x040008F7 RID: 2295
	Nation_EqualRival_NMF,
	// Token: 0x040008F8 RID: 2296
	Nation_LargerRival_NMF,
	// Token: 0x040008F9 RID: 2297
	Nation_Rival_Neighbor,
	// Token: 0x040008FA RID: 2298
	Nation_Rival_Neighbor_NMF,
	// Token: 0x040008FB RID: 2299
	Nation_Rival_Accessible,
	// Token: 0x040008FC RID: 2300
	Nation_RivalOfCouncilorHomeNation,
	// Token: 0x040008FD RID: 2301
	Nation_WarEnemy,
	// Token: 0x040008FE RID: 2302
	Nation_WarEnemy_Accessible,
	// Token: 0x040008FF RID: 2303
	Nation_OffensiveWarEnemy_Accessible,
	// Token: 0x04000900 RID: 2304
	Nation_OffensiveWarEnemy_AtrocityVictim,
	// Token: 0x04000901 RID: 2305
	Nation_Neighbor,
	// Token: 0x04000902 RID: 2306
	Nation_OpenControlPoint,
	// Token: 0x04000903 RID: 2307
	Nation_OpenControlPoint_NonExec,
	// Token: 0x04000904 RID: 2308
	Nation_NMF,
	// Token: 0x04000905 RID: 2309
	Nation_MyFaction,
	// Token: 0x04000906 RID: 2310
	Nation_AlienNation,
	// Token: 0x04000907 RID: 2311
	Region_CouncilorHomeRegion,
	// Token: 0x04000908 RID: 2312
	Region_Neighbor,
	// Token: 0x04000909 RID: 2313
	Region_Neighbor_Rival,
	// Token: 0x0400090A RID: 2314
	Region_Neighbor_WarEnemy,
	// Token: 0x0400090B RID: 2315
	Region_Claimed,
	// Token: 0x0400090C RID: 2316
	Region_Claimed_Rival,
	// Token: 0x0400090D RID: 2317
	Region_Claimed_Neighbor,
	// Token: 0x0400090E RID: 2318
	Region_ArmyHome,
	// Token: 0x0400090F RID: 2319
	Faction_AlienFaction,
	// Token: 0x04000910 RID: 2320
	Faction_AlienProxy,
	// Token: 0x04000911 RID: 2321
	Faction_AlienAppeaser,
	// Token: 0x04000912 RID: 2322
	Faction_NonExecFaction,
	// Token: 0x04000913 RID: 2323
	Faction_AnyHuman,
	// Token: 0x04000914 RID: 2324
	Faction_MostPopularInNation,
	// Token: 0x04000915 RID: 2325
	Faction_MostPopularOnEarth,
	// Token: 0x04000916 RID: 2326
	Faction_NMF_CloseIdeology,
	// Token: 0x04000917 RID: 2327
	Councilor_MyFaction,
	// Token: 0x04000918 RID: 2328
	Councilor_MyFaction_OpposedWealth,
	// Token: 0x04000919 RID: 2329
	Councilor_MyFaction_OpposedScience,
	// Token: 0x0400091A RID: 2330
	Councilor_MyFaction_OpposedGovStatus,
	// Token: 0x0400091B RID: 2331
	Councilor_MyFaction_OpposedPersonality,
	// Token: 0x0400091C RID: 2332
	Councilor_MyFaction_OpposedLearner,
	// Token: 0x0400091D RID: 2333
	Councilor_SharedHomeRegion,
	// Token: 0x0400091E RID: 2334
	Councilor_MyFactionOrCloseIdeology,
	// Token: 0x0400091F RID: 2335
	Councilor_NMF_CloseIdeology,
	// Token: 0x04000920 RID: 2336
	Councilor_MyFaction_AllyHomeNation,
	// Token: 0x04000921 RID: 2337
	Councilor_MyFaction_RivalHomeNation,
	// Token: 0x04000922 RID: 2338
	Orbit_ThisSpaceObject,
	// Token: 0x04000923 RID: 2339
	Hab_Any,
	// Token: 0x04000924 RID: 2340
	Hab_AnyHuman,
	// Token: 0x04000925 RID: 2341
	Hab_InOrbit,
	// Token: 0x04000926 RID: 2342
	Hab_CouncilorOnBoard,
	// Token: 0x04000927 RID: 2343
	Hab_NMF_CloseIdeology,
	// Token: 0x04000928 RID: 2344
	SpaceBody_HabSiteParent,
	// Token: 0x04000929 RID: 2345
	SpaceBody_FleetInOrbit,
	// Token: 0x0400092A RID: 2346
	Ship_MyFaction_AnotherFleet,
	// Token: 0x0400092B RID: 2347
	PriorEvent_Actor,
	// Token: 0x0400092C RID: 2348
	PriorEvent_Target,
	// Token: 0x0400092D RID: 2349
	PriorEvent_SecondaryTarget
}
