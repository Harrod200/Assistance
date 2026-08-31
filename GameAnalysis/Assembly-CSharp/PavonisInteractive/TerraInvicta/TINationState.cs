using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200076B RID: 1899
	public class TINationState : TIPolityState
	{
		// Token: 0x1700090E RID: 2318
		// (get) Token: 0x060036DE RID: 14046 RVA: 0x0013E386 File Offset: 0x0013C586
		// (set) Token: 0x060036DF RID: 14047 RVA: 0x0013E38E File Offset: 0x0013C58E
		public TIRegionState capital { get; private set; }

		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x060036E0 RID: 14048 RVA: 0x0013E397 File Offset: 0x0013C597
		// (set) Token: 0x060036E1 RID: 14049 RVA: 0x0013E39F File Offset: 0x0013C59F
		public TIRegionState originalCapital { get; private set; }

		// Token: 0x17000910 RID: 2320
		// (get) Token: 0x060036E2 RID: 14050 RVA: 0x0013E3A8 File Offset: 0x0013C5A8
		// (set) Token: 0x060036E3 RID: 14051 RVA: 0x0013E3B0 File Offset: 0x0013C5B0
		public List<TIRegionState> regions { get; private set; }

		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x060036E4 RID: 14052 RVA: 0x0013E3B9 File Offset: 0x0013C5B9
		// (set) Token: 0x060036E5 RID: 14053 RVA: 0x0013E3C1 File Offset: 0x0013C5C1
		public List<TINationState> allies { get; private set; }

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x060036E6 RID: 14054 RVA: 0x0013E3CA File Offset: 0x0013C5CA
		// (set) Token: 0x060036E7 RID: 14055 RVA: 0x0013E3D2 File Offset: 0x0013C5D2
		public List<TINationState> rivals { get; private set; }

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x060036E8 RID: 14056 RVA: 0x0013E3DB File Offset: 0x0013C5DB
		// (set) Token: 0x060036E9 RID: 14057 RVA: 0x0013E3E3 File Offset: 0x0013C5E3
		public List<TIRegionState> claims { get; private set; }

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x060036EA RID: 14058 RVA: 0x0013E3EC File Offset: 0x0013C5EC
		// (set) Token: 0x060036EB RID: 14059 RVA: 0x0013E3F4 File Offset: 0x0013C5F4
		public List<TIRegionState> hostileClaims { get; private set; }

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x060036EC RID: 14060 RVA: 0x0013E3FD File Offset: 0x0013C5FD
		// (set) Token: 0x060036ED RID: 14061 RVA: 0x0013E405 File Offset: 0x0013C605
		public List<TINationState> wars { get; private set; }

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x060036EE RID: 14062 RVA: 0x0013E40E File Offset: 0x0013C60E
		// (set) Token: 0x060036EF RID: 14063 RVA: 0x0013E416 File Offset: 0x0013C616
		public List<TIArmyState> armies { get; private set; }

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x060036F0 RID: 14064 RVA: 0x0013E41F File Offset: 0x0013C61F
		// (set) Token: 0x060036F1 RID: 14065 RVA: 0x0013E427 File Offset: 0x0013C627
		public Dictionary<FactionIdeology, float> publicOpinion { get; private set; }

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x060036F2 RID: 14066 RVA: 0x0013E430 File Offset: 0x0013C630
		// (set) Token: 0x060036F3 RID: 14067 RVA: 0x0013E438 File Offset: 0x0013C638
		public int StartOfTurnNativeControlPoints { get; private set; }

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x060036F4 RID: 14068 RVA: 0x0013E441 File Offset: 0x0013C641
		// (set) Token: 0x060036F5 RID: 14069 RVA: 0x0013E449 File Offset: 0x0013C649
		public List<TICouncilorState> advisingCouncilors { get; private set; }

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x060036F6 RID: 14070 RVA: 0x0013E452 File Offset: 0x0013C652
		// (set) Token: 0x060036F7 RID: 14071 RVA: 0x0013E45A File Offset: 0x0013C65A
		public int numControlPoints { get; private set; }

		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x060036F8 RID: 14072 RVA: 0x0013E463 File Offset: 0x0013C663
		// (set) Token: 0x060036F9 RID: 14073 RVA: 0x0013E46B File Offset: 0x0013C66B
		public int numControlPoints_unclamped { get; private set; }

		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x060036FA RID: 14074 RVA: 0x0013E474 File Offset: 0x0013C674
		// (set) Token: 0x060036FB RID: 14075 RVA: 0x0013E47C File Offset: 0x0013C67C
		public float economyScore { get; private set; }

		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x060036FC RID: 14076 RVA: 0x0013E485 File Offset: 0x0013C685
		// (set) Token: 0x060036FD RID: 14077 RVA: 0x0013E48D File Offset: 0x0013C68D
		public float missionDifficultyEconomyScore { get; private set; }

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x060036FE RID: 14078 RVA: 0x0013E496 File Offset: 0x0013C696
		// (set) Token: 0x060036FF RID: 14079 RVA: 0x0013E49E File Offset: 0x0013C69E
		public float accumulatedLegitimizeClaimTriggers { get; private set; }

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06003700 RID: 14080 RVA: 0x0013E4A7 File Offset: 0x0013C6A7
		// (set) Token: 0x06003701 RID: 14081 RVA: 0x0013E4AF File Offset: 0x0013C6AF
		public bool canAccumulateCoreEconomyTriggers { get; private set; }

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06003702 RID: 14082 RVA: 0x0013E4B8 File Offset: 0x0013C6B8
		// (set) Token: 0x06003703 RID: 14083 RVA: 0x0013E4C0 File Offset: 0x0013C6C0
		public bool canAccumulateCoreOilTriggers { get; private set; }

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x06003704 RID: 14084 RVA: 0x0013E4C9 File Offset: 0x0013C6C9
		// (set) Token: 0x06003705 RID: 14085 RVA: 0x0013E4D1 File Offset: 0x0013C6D1
		public bool canAccumulateCoreMiningTriggers { get; private set; }

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06003706 RID: 14086 RVA: 0x0013E4DA File Offset: 0x0013C6DA
		// (set) Token: 0x06003707 RID: 14087 RVA: 0x0013E4E2 File Offset: 0x0013C6E2
		public bool canAccumulateDecolonizeTriggers { get; private set; }

		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06003708 RID: 14088 RVA: 0x0013E4EB File Offset: 0x0013C6EB
		// (set) Token: 0x06003709 RID: 14089 RVA: 0x0013E4F3 File Offset: 0x0013C6F3
		public bool canAccumulateDecontaminateTriggers { get; private set; }

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x0600370A RID: 14090 RVA: 0x0013E4FC File Offset: 0x0013C6FC
		// (set) Token: 0x0600370B RID: 14091 RVA: 0x0013E504 File Offset: 0x0013C704
		public bool canAccumulateLegitimizeClaimTriggers { get; private set; }

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x0600370C RID: 14092 RVA: 0x0013E50D File Offset: 0x0013C70D
		// (set) Token: 0x0600370D RID: 14093 RVA: 0x0013E515 File Offset: 0x0013C715
		public bool spaceFlightProgram { get; private set; }

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x0600370E RID: 14094 RVA: 0x0013E51E File Offset: 0x0013C71E
		// (set) Token: 0x0600370F RID: 14095 RVA: 0x0013E526 File Offset: 0x0013C726
		public bool military { get; private set; }

		// Token: 0x17000927 RID: 2343
		// (get) Token: 0x06003710 RID: 14096 RVA: 0x0013E52F File Offset: 0x0013C72F
		// (set) Token: 0x06003711 RID: 14097 RVA: 0x0013E537 File Offset: 0x0013C737
		public bool nuclearProgram { get; private set; }

		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x06003712 RID: 14098 RVA: 0x0013E540 File Offset: 0x0013C740
		// (set) Token: 0x06003713 RID: 14099 RVA: 0x0013E548 File Offset: 0x0013C748
		public bool canBuildSpaceDefenses { get; private set; }

		// Token: 0x17000929 RID: 2345
		// (get) Token: 0x06003714 RID: 14100 RVA: 0x0013E551 File Offset: 0x0013C751
		// (set) Token: 0x06003715 RID: 14101 RVA: 0x0013E559 File Offset: 0x0013C759
		public bool canBuildSTOSquadrons { get; private set; }

		// Token: 0x1700092A RID: 2346
		// (get) Token: 0x06003716 RID: 14102 RVA: 0x0013E562 File Offset: 0x0013C762
		// (set) Token: 0x06003717 RID: 14103 RVA: 0x0013E56A File Offset: 0x0013C76A
		public bool canDecontaminate { get; private set; }

		// Token: 0x1700092B RID: 2347
		// (get) Token: 0x06003718 RID: 14104 RVA: 0x0013E573 File Offset: 0x0013C773
		// (set) Token: 0x06003719 RID: 14105 RVA: 0x0013E57B File Offset: 0x0013C77B
		public int numNuclearWeapons { get; private set; }

		// Token: 0x1700092C RID: 2348
		// (get) Token: 0x0600371A RID: 14106 RVA: 0x0013E584 File Offset: 0x0013C784
		// (set) Token: 0x0600371B RID: 14107 RVA: 0x0013E58C File Offset: 0x0013C78C
		public float maxMilitaryTechLevel { get; private set; }

		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x0600371C RID: 14108 RVA: 0x0013E595 File Offset: 0x0013C795
		// (set) Token: 0x0600371D RID: 14109 RVA: 0x0013E59D File Offset: 0x0013C79D
		public float sustainability { get; private set; }

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x0600371E RID: 14110 RVA: 0x0013E5A6 File Offset: 0x0013C7A6
		// (set) Token: 0x0600371F RID: 14111 RVA: 0x0013E5AE File Offset: 0x0013C7AE
		public bool policy_closedBorders { get; private set; }

		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x06003720 RID: 14112 RVA: 0x0013E5B7 File Offset: 0x0013C7B7
		// (set) Token: 0x06003721 RID: 14113 RVA: 0x0013E5BF File Offset: 0x0013C7BF
		public bool policy_noOilDevelopment { get; private set; }

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06003722 RID: 14114 RVA: 0x0013E5C8 File Offset: 0x0013C7C8
		// (set) Token: 0x06003723 RID: 14115 RVA: 0x0013E5D0 File Offset: 0x0013C7D0
		public bool policy_noMineralDevelopment { get; private set; }

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06003724 RID: 14116 RVA: 0x0013E5D9 File Offset: 0x0013C7D9
		// (set) Token: 0x06003725 RID: 14117 RVA: 0x0013E5E1 File Offset: 0x0013C7E1
		public bool policy_noNukes { get; private set; }

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06003726 RID: 14118 RVA: 0x0013E5EA File Offset: 0x0013C7EA
		public override bool isNationState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06003727 RID: 14119 RVA: 0x0013E5ED File Offset: 0x0013C7ED
		public override Searchable searchable
		{
			get
			{
				if (!this.extant)
				{
					return Searchable.never;
				}
				return Searchable.always;
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06003728 RID: 14120 RVA: 0x0013E5FA File Offset: 0x0013C7FA
		public override TINationState ref_nation
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x06003729 RID: 14121 RVA: 0x0013E5FD File Offset: 0x0013C7FD
		public override TIRegionState ref_region
		{
			get
			{
				return this.capital;
			}
		}

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x0600372A RID: 14122 RVA: 0x0013E605 File Offset: 0x0013C805
		public override List<TIFactionState> ref_factions
		{
			get
			{
				return this.FactionsWithControlPoint;
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x0600372B RID: 14123 RVA: 0x0013E60D File Offset: 0x0013C80D
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return this.ref_region.spaceBody;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x0600372C RID: 14124 RVA: 0x0013E61A File Offset: 0x0013C81A
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return this.ref_spaceBody;
			}
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x0600372D RID: 14125 RVA: 0x0013E622 File Offset: 0x0013C822
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return this.ref_spaceBody;
			}
		}

		// Token: 0x1700093A RID: 2362
		// (get) Token: 0x0600372E RID: 14126 RVA: 0x0013E62A File Offset: 0x0013C82A
		public override TIFactionState ref_faction
		{
			get
			{
				if (!this.alienNation)
				{
					return this.executiveFaction;
				}
				return GameStateManager.AlienFaction();
			}
		}

		// Token: 0x1700093B RID: 2363
		// (get) Token: 0x0600372F RID: 14127 RVA: 0x0013E640 File Offset: 0x0013C840
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700093C RID: 2364
		// (get) Token: 0x06003730 RID: 14128 RVA: 0x0013E643 File Offset: 0x0013C843
		public override bool hasEarthMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700093D RID: 2365
		// (get) Token: 0x06003731 RID: 14129 RVA: 0x0013E646 File Offset: 0x0013C846
		public TINationTemplate template
		{
			get
			{
				return this.GetMyTemplate<TINationTemplate>();
			}
		}

		// Token: 0x1700093E RID: 2366
		// (get) Token: 0x06003732 RID: 14130 RVA: 0x0013E64E File Offset: 0x0013C84E
		public bool extant
		{
			get
			{
				return this.regions.Count > 0;
			}
		}

		// Token: 0x1700093F RID: 2367
		// (get) Token: 0x06003733 RID: 14131 RVA: 0x0013E65E File Offset: 0x0013C85E
		public bool inFederation
		{
			get
			{
				return this.federation != null;
			}
		}

		// Token: 0x17000940 RID: 2368
		// (get) Token: 0x06003734 RID: 14132 RVA: 0x0013E66C File Offset: 0x0013C86C
		public bool inAlienFederation
		{
			get
			{
				if (GameStateManager.AlienNation().extant)
				{
					TIFederationState tifederationState = this.federation;
					return tifederationState != null && tifederationState.members.Contains(GameStateManager.AlienNation());
				}
				return false;
			}
		}

		// Token: 0x17000941 RID: 2369
		// (get) Token: 0x06003735 RID: 14133 RVA: 0x0013E697 File Offset: 0x0013C897
		public bool alienAlly
		{
			get
			{
				return GameStateManager.AlienNation().extant && this.allies.Contains(GameStateManager.AlienNation());
			}
		}

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06003736 RID: 14134 RVA: 0x0013E6B8 File Offset: 0x0013C8B8
		public bool isUnion
		{
			get
			{
				return this.template.unionTrigger > 0 && this.regions.Count - ((this.template.unionTrigger == 2) ? 0 : this.colonyRegions) >= this.template.unionTrigger;
			}
		}

		// Token: 0x06003737 RID: 14135 RVA: 0x0013E708 File Offset: 0x0013C908
		public bool WillbeUnion(int newNonColonyRegions)
		{
			return this.template.unionTrigger > 0 && this.regions.Count + newNonColonyRegions - this.colonyRegions >= this.template.unionTrigger;
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06003738 RID: 14136 RVA: 0x0013E73E File Offset: 0x0013C93E
		public string displayNameWithArticle
		{
			get
			{
				if (!this.isUnion)
				{
					return this.template.displayNameWithArticle;
				}
				return this.template.unionDisplayNameWithArticle;
			}
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06003739 RID: 14137 RVA: 0x0013E75F File Offset: 0x0013C95F
		public string displayNameWithArticleCapitalized
		{
			get
			{
				if (!this.isUnion)
				{
					return Utilities.Capitalize(this.template.displayNameWithArticle);
				}
				return Utilities.Capitalize(this.template.unionDisplayNameWithArticle);
			}
		}

		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x0600373A RID: 14138 RVA: 0x0013E78A File Offset: 0x0013C98A
		public string nationalAdjective
		{
			get
			{
				if (!this.isUnion)
				{
					return this.template.nationAdjective;
				}
				return this.template.unionAdjective;
			}
		}

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x0600373B RID: 14139 RVA: 0x0013E7AB File Offset: 0x0013C9AB
		public string displayNameWithArticleAndPlacePrep
		{
			get
			{
				if (!this.isUnion)
				{
					return this.template.displayNameWithArticleAndPlacePrep;
				}
				return this.template.unionDisplayNameWithArticleAndPlacePrep;
			}
		}

		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x0600373C RID: 14140 RVA: 0x0013E7CC File Offset: 0x0013C9CC
		public string flagResource
		{
			get
			{
				if (!this.isUnion)
				{
					return this.template.flagResource;
				}
				return this.template.GetUnionFlagResource();
			}
		}

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x0600373D RID: 14141 RVA: 0x0013E7ED File Offset: 0x0013C9ED
		public int abductions
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState region) => region.abductions);
			}
		}

		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x0600373E RID: 14142 RVA: 0x0013E819 File Offset: 0x0013CA19
		public int maxControlPointIndex
		{
			get
			{
				return this.numControlPoints - 1;
			}
		}

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x0600373F RID: 14143 RVA: 0x0013E823 File Offset: 0x0013CA23
		public bool atWar
		{
			get
			{
				return this.wars.Count > 0;
			}
		}

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06003740 RID: 14144 RVA: 0x0013E833 File Offset: 0x0013CA33
		public bool belligerentInActiveWar
		{
			get
			{
				if (this.wars.Count > 0)
				{
					return this.currentWarStates.NotAll<TIWarState>((TIWarState x) => x.stalemate);
				}
				return false;
			}
		}

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06003741 RID: 14145 RVA: 0x0013E86F File Offset: 0x0013CA6F
		public bool hasAlienFacility
		{
			get
			{
				return this.regions.Any<TIRegionState>((TIRegionState x) => x.hasAlienFacility);
			}
		}

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06003742 RID: 14146 RVA: 0x0013E89B File Offset: 0x0013CA9B
		public int unionTrigger
		{
			get
			{
				return this.template.unionTrigger;
			}
		}

		// Token: 0x1700094E RID: 2382
		// (get) Token: 0x06003743 RID: 14147 RVA: 0x0013E8A8 File Offset: 0x0013CAA8
		public static int numExtantNations
		{
			get
			{
				return GameStateManager.AllExtantNations().Count<TINationState>();
			}
		}

		// Token: 0x1700094F RID: 2383
		// (get) Token: 0x06003744 RID: 14148 RVA: 0x0013E8B4 File Offset: 0x0013CAB4
		public bool breakaway
		{
			get
			{
				return this.breakawayParent != null;
			}
		}

		// Token: 0x17000950 RID: 2384
		// (get) Token: 0x06003745 RID: 14149 RVA: 0x0013E8C2 File Offset: 0x0013CAC2
		public Sprite flag
		{
			get
			{
				return this._flag;
			}
		}

		// Token: 0x06003746 RID: 14150 RVA: 0x0013E8CC File Offset: 0x0013CACC
		public override void InitWithTemplate(TIDataTemplate template)
		{
			base.InitWithTemplate(template);
			TINationTemplate tinationTemplate = template as TINationTemplate;
			if (tinationTemplate == null)
			{
				return;
			}
			this.templateName = tinationTemplate.dataName;
			this.alienNation = this.templateName == TemplateManager.global.alienNationDataName;
			if (!this.gameStateSubjectCreated)
			{
				this.GDP = (tinationTemplate.initialGDP * (double)GameStateManager.Time().template.globalStartingGDPScaling) ?? 100.0;
				this.spaceFunding_year = tinationTemplate.spaceFunding_year.GetValueOrDefault();
				this.inequality = tinationTemplate.inequality.GetValueOrDefault();
				this.education = tinationTemplate.education.GetValueOrDefault();
				this.democracy = tinationTemplate.democracy.GetValueOrDefault();
				this.cohesion = tinationTemplate.cohesion.GetValueOrDefault();
				this.unrest = tinationTemplate.unrest.GetValueOrDefault();
				this.cohesionRestState_dailyCache = this.cohesion;
				this.unrestRestState_dailyCache = this.unrest;
				this.militaryTechLevel = tinationTemplate.miltech.GetValueOrDefault();
				this.maxMilitaryTechLevel = (this.alienNation ? 8f : 5f);
				this.canBuildSpaceDefenses = this.alienNation;
				this.canBuildSTOSquadrons = this.alienNation;
				this.numNuclearWeapons = (int)tinationTemplate.nuclearWeapons.GetValueOrDefault();
				this.aggregateNation = tinationTemplate.aggregateNation;
				this.sustainability = tinationTemplate.greenEconomy;
				this.regions = new List<TIRegionState>();
				this.claims = new List<TIRegionState>();
				this.armies = new List<TIArmyState>();
				this.allies = new List<TINationState>();
				this.rivals = new List<TINationState>();
				this.wars = new List<TINationState>();
				this.adjacentNations = new Dictionary<TINationState, TerrestrialAdjacencyType>();
				this.controlPoints = new List<TIControlPoint>();
				this.publicOpinion = new Dictionary<FactionIdeology, float>();
				this.numControlPoints = this.getNumControlPoints;
				this.numControlPoints_unclamped = this.getNumControlPoints_unclamped;
				for (int i = 0; i <= this.maxControlPointIndex; i++)
				{
					TIControlPoint ticontrolPoint = GameStateManager.CreateNewGameState<TIControlPoint>();
					ticontrolPoint.InitWithNationState(this, i);
					this.controlPoints.Add(ticontrolPoint);
				}
				this.advisingCouncilors = new List<TICouncilorState>();
				this.improveRelationsCooldowns = new Dictionary<TINationState, TIDateTime>();
				this.rivalryCooldowns = new Dictionary<TINationState, TIDateTime>();
			}
			if (this.breakaways == null)
			{
				this.breakaways = new List<TINationState>();
			}
		}

		// Token: 0x06003747 RID: 14151 RVA: 0x0013EB48 File Offset: 0x0013CD48
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			foreach (TISpaceBodyState tispaceBodyState in GameStateManager.AllSpaceBodies())
			{
				if (tispaceBodyState.templateName == this.solarBody)
				{
					tispaceBodyState.nations.Add(this);
					break;
				}
			}
			foreach (PriorityType priorityType in Enums.PriorityTypes)
			{
				this.SetAccumulatedInvestmentPoints(priorityType, this.GetInitialInvestmentPoints(priorityType), false);
			}
			if (this.ReachedInvestmentThreshhold(PriorityType.Military_InitiateNuclearProgram))
			{
				this.nuclearProgram = true;
				this.SetAccumulatedInvestmentPoints(PriorityType.Military_InitiateNuclearProgram, 0f, false);
			}
			if (this.ReachedInvestmentThreshhold(PriorityType.Civilian_InitiateSpaceflightProgram))
			{
				this.spaceFlightProgram = true;
				this.SetAccumulatedInvestmentPoints(PriorityType.Civilian_InitiateSpaceflightProgram, 0f, false);
			}
			if (this.ReachedInvestmentThreshhold(PriorityType.Military_FoundMilitary))
			{
				this.military = true;
				this.SetAccumulatedInvestmentPoints(PriorityType.Military_FoundMilitary, 0f, false);
			}
			try
			{
				this.SetDisplayNameAndFlag();
			}
			catch
			{
				this.SetDisplayNameAndFlag();
			}
		}

		// Token: 0x06003748 RID: 14152 RVA: 0x0013EC38 File Offset: 0x0013CE38
		public static void SetAllBilaterals()
		{
			using (IEnumerator<TIBilateralTemplate> enumerator = TemplateManager.IterateByClass<TIBilateralTemplate>(true).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIBilateralTemplate bilateral = enumerator.Current;
					if (!string.IsNullOrEmpty(bilateral.nation1) && bilateral.BilateralIsInScenario())
					{
						if (bilateral.BilateralIsActive())
						{
							TINationState tinationState = GameStateManager.NationLookup()[bilateral.nation1];
							TINationState tinationState2 = tinationState;
							if (tinationState2.hostileClaims == null)
							{
								tinationState2.hostileClaims = new List<TIRegionState>();
							}
							TINationState tinationState3;
							switch (bilateral.relationType)
							{
							case BilateralRelationType.Federation:
								GameStateManager.IterateByClass<TIFederationState>(false).First<TIFederationState>((TIFederationState x) => x.federationName == bilateral.federation).AddNation(null, tinationState, true);
								continue;
							case BilateralRelationType.Alliance:
								tinationState3 = GameStateManager.NationLookup()[bilateral.nation2];
								tinationState.AddAlly(null, tinationState3, false, true);
								tinationState3.AddAlly(null, tinationState, false, true);
								continue;
							case BilateralRelationType.Rivalry:
								tinationState3 = GameStateManager.NationLookup()[bilateral.nation2];
								tinationState.AddRival(null, tinationState3, false, true, true);
								tinationState3.AddRival(null, tinationState, false, true, true);
								continue;
							case BilateralRelationType.War:
							{
								tinationState3 = GameStateManager.NationLookup()[bilateral.nation2];
								tinationState.AddWar(tinationState3);
								tinationState3.AddWar(tinationState);
								using (IEnumerator<TIWarState> enumerator2 = GameStateManager.IterateByClass<TIWarState>(false).GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										TIWarState tiwarState = enumerator2.Current;
										if (tiwarState.attacker == null)
										{
											GameStateManager.GlobalValues().InitiateWarFromStart(tiwarState, tinationState, tinationState3, new List<TINationState> { tinationState }, new List<TINationState> { tinationState3 });
											break;
										}
									}
									continue;
								}
								break;
							}
							case BilateralRelationType.PhysicalAdjacency:
								continue;
							case BilateralRelationType.Claim:
							{
								TIRegionState regionState = bilateral.regionState1;
								tinationState.claims.Add(regionState);
								regionState.AddClaim(tinationState);
								if (bilateral.initialOwner)
								{
									tinationState.AddRegion(regionState);
									regionState.nation = tinationState;
									regionState.colonyRegion = bilateral.initialColony;
									if (bilateral.capitalClaim)
									{
										tinationState.SetCapital(regionState);
									}
								}
								if (bilateral.capitalClaim)
								{
									tinationState.originalCapital = regionState;
									continue;
								}
								if (bilateral.hostileClaim && !tinationState.alienNation)
								{
									tinationState.hostileClaims.Add(regionState);
									continue;
								}
								continue;
							}
							case BilateralRelationType.Breakaway:
								break;
							default:
								continue;
							}
							tinationState3 = GameStateManager.NationLookup()[bilateral.nation2];
							tinationState.SetAsBreakaway(null, tinationState3);
						}
						else if (bilateral.relationType == BilateralRelationType.Claim && bilateral.capitalClaim)
						{
							TINationState tinationState4 = GameStateManager.NationLookup()[bilateral.nation1];
							TIRegionState regionState2 = bilateral.regionState1;
							tinationState4.originalCapital = regionState2;
						}
					}
				}
			}
		}

		// Token: 0x06003749 RID: 14153 RVA: 0x0013EF70 File Offset: 0x0013D170
		public override void PostGlobalGameStateCreateInit_2()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			if (this.rivalryCooldowns == null)
			{
				this.rivalryCooldowns = new Dictionary<TINationState, TIDateTime>();
			}
			if (this.hostileClaims == null)
			{
				this.hostileClaims = new List<TIRegionState>();
			}
			if (this.improveRelationsDeclinedUnderCurrentExecutivePair == null)
			{
				this.improveRelationsDeclinedUnderCurrentExecutivePair = new List<TINationState>();
			}
			if (!this.military)
			{
				float? foundMilitaryIPs = this.template.foundMilitaryIPs;
				float num = (float)1;
				if (((foundMilitaryIPs.GetValueOrDefault() >= num) & (foundMilitaryIPs != null)) || this.armies.Count > 0)
				{
					this.military = true;
				}
			}
			foreach (PriorityType priorityType in Enums.PriorityTypes)
			{
				if (!this._accumulatedInvestmentPoints.ContainsKey(priorityType))
				{
					this._accumulatedInvestmentPoints.Add(priorityType, 0f);
				}
			}
			if (!this.gameStateSubjectCreated)
			{
				this.CacheRegionValues();
			}
			else
			{
				foreach (TIArmyState tiarmyState in new List<TIArmyState>(this.armies))
				{
					if (tiarmyState.strength <= 0f)
					{
						Log.Warn("This save had a bad or destroyed armyState assigned to a nation. Please notify devs how it was destroyed. ArmyStateID: " + base.ID.ToString() + " Nation:" + this.displayName, Array.Empty<object>());
						this.RemoveArmy(tiarmyState);
					}
				}
				if (this.alienNation)
				{
					foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
					{
						if (!this.claims.Contains(tiregionState))
						{
							this.claims.Add(tiregionState);
							Log.Warn("Adding alien nation missing claim on " + tiregionState.templateName, Array.Empty<object>());
						}
					}
				}
				foreach (TIRegionState tiregionState2 in this.claims)
				{
					tiregionState2.AddClaim(this);
				}
			}
			if (this.capital == null && this.regions.Count > 0)
			{
				this.SetNonAssignedCapital();
			}
			if (this.originalCapital == null && !this.alienNation)
			{
				foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
				{
					if (tibilateralTemplate.nationState1 == this && tibilateralTemplate.capitalClaim)
					{
						this.originalCapital = tibilateralTemplate.regionState1;
					}
				}
			}
			if (this.originalCapital == null && !this.alienNation)
			{
				IEnumerable<TIRegionState> enumerable;
				if (this.regions.Count > 0)
				{
					enumerable = this.regions;
				}
				else
				{
					enumerable = from x in TemplateManager.IterateByClass<TIBilateralTemplate>(true)
						where x.relationType == BilateralRelationType.Claim
						where x.nationState1 == this
						select x.regionState1;
				}
				this.originalCapital = enumerable.MaxBy<TIRegionState, double>((TIRegionState x) => x.nationalGDPShareValue);
				string text = "Repairing missing originalCapital in ";
				string text2 = ((this != null) ? this.ToString() : null);
				string text3 = " : ";
				TIRegionState originalCapital = this.originalCapital;
				Log.Debug(text + text2 + text3 + (((originalCapital != null) ? originalCapital.ToString() : null) ?? "null"), Array.Empty<object>());
			}
			this.InitializeAllTrackers();
			foreach (PriorityType priorityType2 in this._accumulatedInvestmentPoints.Keys.ToList<PriorityType>())
			{
				if (float.IsNaN(this._accumulatedInvestmentPoints[priorityType2]) || float.IsInfinity(this._accumulatedInvestmentPoints[priorityType2]))
				{
					Log.Debug("Accumulated investment points for " + priorityType2.ToString() + " was NaN or Infinity. Repairing...", Array.Empty<object>());
					this.SetAccumulatedInvestmentPoints(priorityType2, 0f, false);
				}
			}
			this.education = Mathf.Clamp(this.education, 0f, 255f);
			this.SetDisplayNameAndFlag();
			this.regions = this.regions.Distinct<TIRegionState>().ToList<TIRegionState>();
			if (this.historyNumRegions == null)
			{
				this.historyNumRegions = new List<int>();
				this.historyNumRegions.AddRange(Enumerable.Repeat<int>(this.regions.Count, 32));
			}
			this.AddToMaxMilitaryTechLevel(0f);
		}

		// Token: 0x0600374A RID: 14154 RVA: 0x0013F444 File Offset: 0x0013D644
		public override void PostCanvasManagerCreateInit_3()
		{
			if (!this.gameStateSubjectCreated)
			{
				for (int i = 0; i <= this.maxControlPointIndex; i++)
				{
					this.ApplyInvestmentTemplateToControlPoint(i, this.template.initialPriorityPreset[i]);
				}
				this.UpdateControlPointTypes();
			}
			if (this.improveRelationsCooldowns == null)
			{
				this.improveRelationsCooldowns = new Dictionary<TINationState, TIDateTime>();
			}
			this.GenerateAdjacentNationsDictionary();
		}

		// Token: 0x0600374B RID: 14155 RVA: 0x0013F4A0 File Offset: 0x0013D6A0
		public override void PostInitializationInit_4()
		{
			if (this.inFederation)
			{
				if (this.federation.deleted)
				{
					Log.Debug(this.displayName + " is in deleted federation. Repairing...", Array.Empty<object>());
					this.federation = null;
				}
				else if (!this.federation.members.Contains(this))
				{
					Log.Debug(this.displayName + " is in " + this.federation.displayName + ", but not registered with that federation. Repairing...", Array.Empty<object>());
					this.federation.AddNation(null, this, false);
				}
			}
			foreach (TIRegionState tiregionState in this.regions.ToList<TIRegionState>())
			{
				if (tiregionState.nation != this)
				{
					this.RemoveRegion(tiregionState);
					Log.Error(string.Concat(new string[]
					{
						this.displayName,
						" had ",
						tiregionState.displayName,
						" but ",
						tiregionState.nation.displayName,
						" actually owned it."
					}), Array.Empty<object>());
				}
			}
			foreach (TIArmyState tiarmyState in this.armies.ToList<TIArmyState>())
			{
				if (tiarmyState.homeNation != this)
				{
					this.armies.Remove(tiarmyState);
				}
			}
			IEnumerable<TINationState> enumerable = this.currentWarStates.SelectMany<TIWarState, TINationState>((TIWarState x) => x.EnemyAlliance(this));
			if (enumerable.Count<TINationState>() != this.wars.Count)
			{
				Log.Error("Old Error when " + this.displayName + " has out of sync wars", Array.Empty<object>());
				foreach (TINationState tinationState in enumerable)
				{
					this.SyncWarCount(tinationState);
				}
				foreach (TINationState tinationState2 in this.wars.ToList<TINationState>())
				{
					this.SyncWarCount(tinationState2);
				}
			}
			if (!this.extant)
			{
				this.ClearAdvisingCouncilors();
				this.ClearAllies();
				GameStateManager.AllExtantNations().ToList<TINationState>().ForEach(delegate(TINationState x)
				{
					x.allies.Remove(this);
				});
			}
			this.ModifyGDP(0.0, TINationState.GDPChangeReason.GDPReason_EventEffect);
		}

		// Token: 0x0600374C RID: 14156 RVA: 0x0013F74C File Offset: 0x0013D94C
		public override void PostAllStartUpInit_5()
		{
			this.SetBaseInvestmentPoints_month();
			if (!this.gameStateSubjectCreated)
			{
				this.SetInitialPublicOpinion();
				if (TIGlobalValuesState.Customizations.usingCustomizations && TIGlobalValuesState.Customizations.customFactionStartingNationGroup.Count > 0)
				{
					foreach (KeyValuePair<string, int> keyValuePair in TIGlobalValuesState.Customizations.customFactionStartingNationGroup)
					{
						if (this.template.group == keyValuePair.Value)
						{
							for (int i = 0; i < 5; i++)
							{
								this.PropagandaOnPop(GameStateManager.FindByTemplate<TIFactionState>(keyValuePair.Key, false).ideology, 25f, false);
							}
						}
					}
				}
				if (this.alienNation && this.extant)
				{
					if (this.controlPoints.Any<TIControlPoint>((TIControlPoint x) => x.faction != GameStateManager.AlienFaction()))
					{
						this.controlPoints.ForEach(delegate(TIControlPoint x)
						{
							this.ChangeControlPointOwner(x.positionInNation, ControlPointChangeCause.None, GameStateManager.AlienFaction());
						});
					}
					GameStateManager.AlienFaction().AddGoal(new FactionGoal_InvadeEarth(GameStateManager.AlienFaction(), 20), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
				this.FillOutPastTrackerDataForNewNation();
			}
			else
			{
				this.BringTrackerDataUpToDate();
			}
			this.gameStateSubjectCreated = true;
			this.SetPriorityEffectPopScaling();
			if (this.extant && this.alienNation)
			{
				foreach (TIFactionState tifactionState in from x in GameStateManager.AllHumanFactions()
					where x.proAlien
					select x)
				{
					tifactionState.CompleteMilestone(CampaignMilestone.AlienInfrastructureExists);
				}
			}
		}

		// Token: 0x0600374D RID: 14157 RVA: 0x0013F908 File Offset: 0x0013DB08
		public void SetDataDirty()
		{
			GameControl.eventManager.TriggerEvent(new NationDataUpdated(this), null, new object[] { this });
		}

		// Token: 0x0600374E RID: 14158 RVA: 0x0013F928 File Offset: 0x0013DB28
		public void SetDisplayNameAndFlag()
		{
			if (this.isUnion)
			{
				this.displayName = this.template.unionDisplayName;
				this._flag = GameControl.assetLoader.LoadAsset<Sprite>(this.template.GetUnionFlagResource());
				return;
			}
			this.displayName = this.template.displayName;
			try
			{
				this._flag = GameControl.assetLoader.LoadAsset<Sprite>(this.template.flagResource);
			}
			catch
			{
				this._flag = GameControl.assetLoader.LoadAsset<Sprite>(this.template.flagResource);
			}
		}

		// Token: 0x0600374F RID: 14159 RVA: 0x0013F9C8 File Offset: 0x0013DBC8
		public float GetPublicOpinionOfFaction(TIFactionState faction)
		{
			if (faction.ideology.alien)
			{
				faction = GameStateManager.AlienProxy();
			}
			float num;
			this.publicOpinion.TryGetValue(faction.ideology.ideology, out num);
			return num;
		}

		// Token: 0x06003750 RID: 14160 RVA: 0x0013FA04 File Offset: 0x0013DC04
		public float GetPublicOpinionOfFaction(FactionIdeology ideology)
		{
			if (ideology == FactionIdeology.Alien)
			{
				ideology = GameStateManager.AlienProxy().ideology.ideology;
			}
			float num;
			this.publicOpinion.TryGetValue(ideology, out num);
			return num;
		}

		// Token: 0x06003751 RID: 14161 RVA: 0x0013FA38 File Offset: 0x0013DC38
		public float GetPublicOpinionOfFaction(TIFactionIdeologyTemplate factionIdeology)
		{
			if (factionIdeology.alien)
			{
				factionIdeology = GameStateManager.AlienProxy().ideology;
			}
			float num;
			this.publicOpinion.TryGetValue(factionIdeology.ideology, out num);
			return num;
		}

		// Token: 0x06003752 RID: 14162 RVA: 0x0013FA70 File Offset: 0x0013DC70
		public float GetMostPopularFactionValue(bool returnUndecided)
		{
			float num = 0f;
			foreach (KeyValuePair<FactionIdeology, float> keyValuePair in this.publicOpinion)
			{
				if (keyValuePair.Value > num && (keyValuePair.Key != FactionIdeology.Undecided || returnUndecided))
				{
					num = keyValuePair.Value;
				}
			}
			if (num <= 0f)
			{
				return 1f;
			}
			return num;
		}

		// Token: 0x06003753 RID: 14163 RVA: 0x0013FAF4 File Offset: 0x0013DCF4
		public TIFactionIdeologyTemplate GetMostPopularIdeology(bool returnUndecided)
		{
			float num = 0f;
			FactionIdeology factionIdeology = FactionIdeology.None;
			foreach (KeyValuePair<FactionIdeology, float> keyValuePair in this.publicOpinion)
			{
				if (keyValuePair.Value > num && (keyValuePair.Key != FactionIdeology.Undecided || returnUndecided))
				{
					num = keyValuePair.Value;
					factionIdeology = keyValuePair.Key;
				}
			}
			if (num <= 0f)
			{
				return GameStateManager.UndecidedIdeology();
			}
			return TIFactionIdeologyTemplate.GetIdeologyTemplate(factionIdeology);
		}

		// Token: 0x06003754 RID: 14164 RVA: 0x0013FB88 File Offset: 0x0013DD88
		public static bool proAlienPublic(TINationState nation)
		{
			return TIFactionIdeologyTemplate.GetIdeologyTemplate(nation.GetMeanPublicOpinion()).ideologyCoordinates.x > 0f;
		}

		// Token: 0x06003755 RID: 14165 RVA: 0x0013FBA6 File Offset: 0x0013DDA6
		public static bool fanaticProAlienPublic(TINationState nation)
		{
			return TIFactionIdeologyTemplate.GetIdeologyTemplate(nation.GetMeanPublicOpinion()).ideologyCoordinates.x > 1f;
		}

		// Token: 0x06003756 RID: 14166 RVA: 0x0013FBC4 File Offset: 0x0013DDC4
		public static bool antiAlienPublic(TINationState nation)
		{
			return TIFactionIdeologyTemplate.GetIdeologyTemplate(nation.GetMeanPublicOpinion()).ideologyCoordinates.x < 0f;
		}

		// Token: 0x06003757 RID: 14167 RVA: 0x0013FBE2 File Offset: 0x0013DDE2
		public static bool fanaticAntiAlienPublic(TINationState nation)
		{
			return TIFactionIdeologyTemplate.GetIdeologyTemplate(nation.GetMeanPublicOpinion()).ideologyCoordinates.x < 1f;
		}

		// Token: 0x06003758 RID: 14168 RVA: 0x0013FC00 File Offset: 0x0013DE00
		public FactionIdeology GetMeanPublicOpinion()
		{
			return TINationState.GetNearestIdeology(this.GetMeanPublicOpinionVector(), false, FactionIdeology.None);
		}

		// Token: 0x06003759 RID: 14169 RVA: 0x0013FC10 File Offset: 0x0013DE10
		public Vector3 GetMeanPublicOpinionVector()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			foreach (KeyValuePair<FactionIdeology, float> keyValuePair in this.publicOpinion)
			{
				Vector3 ideologyCoordinates = TIFactionIdeologyTemplate.GetIdeologyTemplate(keyValuePair.Key).ideologyCoordinates;
				num += ideologyCoordinates.x * keyValuePair.Value;
				num2 += ideologyCoordinates.y * keyValuePair.Value;
				num3 += ideologyCoordinates.z * keyValuePair.Value;
			}
			return new Vector3(num, num2, num3);
		}

		// Token: 0x0600375A RID: 14170 RVA: 0x0013FCC0 File Offset: 0x0013DEC0
		public float GetPublicOpinionStdDv()
		{
			Vector3 meanPublicOpinionVector = this.GetMeanPublicOpinionVector();
			float num = 0f;
			foreach (KeyValuePair<FactionIdeology, float> keyValuePair in this.publicOpinion)
			{
				num += (TIFactionIdeologyTemplate.GetIdeologyTemplate(keyValuePair.Key).ideologyCoordinates - meanPublicOpinionVector).sqrMagnitude * keyValuePair.Value;
			}
			return Mathf.Sqrt(num);
		}

		// Token: 0x0600375B RID: 14171 RVA: 0x0013FD4C File Offset: 0x0013DF4C
		public float PublicOpinionToMaxIdeologicalAntipathyRatio()
		{
			return Mathf.Clamp01(this.GetPublicOpinionStdDv() / TIGlobalValuesState.GlobalValues.worstCasePublicOpinionDispersal);
		}

		// Token: 0x0600375C RID: 14172 RVA: 0x0013FD64 File Offset: 0x0013DF64
		public FactionIdeology GetMeanEliteIdeology()
		{
			return TINationState.GetNearestIdeology(this.GetMeanEliteVector(), true, FactionIdeology.None);
		}

		// Token: 0x0600375D RID: 14173 RVA: 0x0013FD74 File Offset: 0x0013DF74
		public Vector3 GetMeanEliteVector()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				if (ticontrolPoint != null && ticontrolPoint.owned)
				{
					Vector3 ideologyCoordinates = ticontrolPoint.faction.ideology.ideologyCoordinates;
					num += ideologyCoordinates.x;
					num2 += ideologyCoordinates.y;
					num3 += ideologyCoordinates.z;
				}
			}
			return new Vector3(num / (float)this.numControlPoints, num2 / (float)this.numControlPoints, num3 / (float)this.numControlPoints);
		}

		// Token: 0x0600375E RID: 14174 RVA: 0x0013FE34 File Offset: 0x0013E034
		public static FactionIdeology GetNearestIdeology(Vector3 ideaPoint, bool allowAlien = false, FactionIdeology disallowIdeology = FactionIdeology.None)
		{
			float num = 999f;
			FactionIdeology factionIdeology = FactionIdeology.None;
			List<TIFactionIdeologyTemplate> list = (allowAlien ? GameStateManager.ActiveIdeologies().ToList<TIFactionIdeologyTemplate>() : GameStateManager.ActiveHumanIdeologies().ToList<TIFactionIdeologyTemplate>());
			if (disallowIdeology != FactionIdeology.None)
			{
				list.Remove(TIFactionIdeologyTemplate.GetIdeologyTemplate(disallowIdeology));
			}
			foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate in list)
			{
				float num2 = Vector3.Distance(tifactionIdeologyTemplate.ideologyCoordinates, ideaPoint);
				if (num2 < num)
				{
					factionIdeology = tifactionIdeologyTemplate.ideology;
					num = num2;
				}
			}
			if (factionIdeology == FactionIdeology.None)
			{
				factionIdeology = FactionIdeology.Undecided;
			}
			return factionIdeology;
		}

		// Token: 0x0600375F RID: 14175 RVA: 0x0013FED4 File Offset: 0x0013E0D4
		public static float GetIdeologicalDistance(Vector3 ideaPoint1, Vector3 ideaPoint2)
		{
			return Vector3.Distance(ideaPoint1, ideaPoint2);
		}

		// Token: 0x06003760 RID: 14176 RVA: 0x0013FEDD File Offset: 0x0013E0DD
		public static float GetIdeologicalDistance(TIFactionIdeologyTemplate ideology1, Vector3 ideaPoint)
		{
			return Vector3.Distance(ideology1.ideologyCoordinates, ideaPoint);
		}

		// Token: 0x06003761 RID: 14177 RVA: 0x0013FEEB File Offset: 0x0013E0EB
		public static float GetIdeologicalDistance(TIFactionState faction1, TIFactionState faction2)
		{
			return GameStateManager.GlobalValues().ideologyDistanceGrid[(faction1 != null) ? faction1.ideology.ideology : FactionIdeology.Undecided][(faction2 != null) ? faction2.ideology.ideology : FactionIdeology.Undecided];
		}

		// Token: 0x06003762 RID: 14178 RVA: 0x0013FF23 File Offset: 0x0013E123
		public static float GetIdeologicalDistance(TIFactionIdeologyTemplate ideology1, TIFactionIdeologyTemplate ideology2)
		{
			return GameStateManager.GlobalValues().ideologyDistanceGrid[ideology1.ideology][ideology2.ideology];
		}

		// Token: 0x06003763 RID: 14179 RVA: 0x0013FF45 File Offset: 0x0013E145
		public static float GetIdeologicalDistance(FactionIdeology ideology1, FactionIdeology ideology2)
		{
			return GameStateManager.GlobalValues().ideologyDistanceGrid[ideology1][ideology2];
		}

		// Token: 0x06003764 RID: 14180 RVA: 0x0013FF5D File Offset: 0x0013E15D
		public static float GetIdeologicalDistance(TIFactionIdeologyTemplate ideology1, FactionIdeology ideology2)
		{
			return GameStateManager.GlobalValues().ideologyDistanceGrid[ideology1.ideology][ideology2];
		}

		// Token: 0x06003765 RID: 14181 RVA: 0x0013FF7C File Offset: 0x0013E17C
		public float GetPublicOpinionProportion(FactionIdeology ideology)
		{
			float num;
			this.publicOpinion.TryGetValue(ideology, out num);
			return num;
		}

		// Token: 0x06003766 RID: 14182 RVA: 0x0013FF9C File Offset: 0x0013E19C
		public void SetInitialPublicOpinion()
		{
			float cohesionRestState = this.cohesionRestState;
			float num = ((10f - this.democracy) * 3f + cohesionRestState * (float)(6 - this.numControlPoints + 1)) / 100f;
			float num2 = this.education * 6.5f + (float)TIUtilities.RandomRange(0, 60) - 3.5f * (10f - this.democracy);
			Dictionary<FactionIdeology, float> dictionary = new Dictionary<FactionIdeology, float>();
			foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate in GameStateManager.ActiveHumanIdeologies())
			{
				dictionary.Add(tifactionIdeologyTemplate.ideology, (float)((tifactionIdeologyTemplate.ideology == FactionIdeology.Undecided) ? 1 : 0));
			}
			List<TIFactionIdeologyTemplate> list = (from x in GameStateManager.ActiveHumanIdeologies()
				where x.initialReactionGroup == 0
				select x).ToList<TIFactionIdeologyTemplate>();
			List<TIFactionIdeologyTemplate> list2 = (from x in GameStateManager.ActiveHumanIdeologies()
				where x.initialReactionGroup == 1
				select x).ToList<TIFactionIdeologyTemplate>();
			List<TIFactionIdeologyTemplate> list3 = (from x in GameStateManager.ActiveHumanIdeologies()
				where x.initialReactionGroup == 2
				select x).ToList<TIFactionIdeologyTemplate>();
			if (num2 <= 25f && list.Count > 0)
			{
				TIFactionIdeologyTemplate tifactionIdeologyTemplate2 = list.SelectRandomItem<TIFactionIdeologyTemplate>();
				dictionary[tifactionIdeologyTemplate2.ideology] = num;
				if (this.democracy > 3.5f || this.cohesion < 8f)
				{
					list.Remove(tifactionIdeologyTemplate2);
				}
			}
			else if (num2 < 65f || this.democracy <= 2f)
			{
				dictionary[FactionIdeology.Undecided] = num;
			}
			else if (list2.Count > 0)
			{
				TIFactionIdeologyTemplate tifactionIdeologyTemplate3 = list.SelectRandomItem<TIFactionIdeologyTemplate>();
				dictionary[tifactionIdeologyTemplate3.ideology] = num;
				if (this.democracy > 3.5f || this.cohesion < 8f)
				{
					list2.Remove(tifactionIdeologyTemplate3);
				}
			}
			float num3 = 1f - num;
			float num4 = this.education * 2f + this.democracy + (float)TIUtilities.RandomRange(0, 80);
			float num5 = Mathf.Min(num * 0.75f, TIUtilities.RandomRange(0.15f, 0.3f)) * num3;
			if (num4 <= 30f && list.Count > 0)
			{
				Dictionary<FactionIdeology, float> dictionary2 = dictionary;
				FactionIdeology factionIdeology = list.SelectRandomItem<TIFactionIdeologyTemplate>().ideology;
				dictionary2[factionIdeology] += num5;
			}
			else if (num4 < 60f && list3.Count > 0)
			{
				Dictionary<FactionIdeology, float> dictionary2 = dictionary;
				FactionIdeology factionIdeology = list3.SelectRandomItem<TIFactionIdeologyTemplate>().ideology;
				dictionary2[factionIdeology] += num5;
			}
			else if (list2.Count > 0)
			{
				Dictionary<FactionIdeology, float> dictionary2 = dictionary;
				FactionIdeology factionIdeology = list2.SelectRandomItem<TIFactionIdeologyTemplate>().ideology;
				dictionary2[factionIdeology] += num5;
			}
			num3 -= num5;
			foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate4 in GameStateManager.ActiveHumanIdeologies().ToList<TIFactionIdeologyTemplate>().Shuffle<TIFactionIdeologyTemplate>())
			{
				if (tifactionIdeologyTemplate4.ideology != FactionIdeology.Undecided)
				{
					if (list.Contains(tifactionIdeologyTemplate4))
					{
						Dictionary<FactionIdeology, float> dictionary2 = dictionary;
						FactionIdeology factionIdeology = tifactionIdeologyTemplate4.ideology;
						dictionary2[factionIdeology] += Mathf.Clamp((20f - this.education - (float)TIUtilities.RandomRange(0, 10)) / 100f, 0f, num3 * ((float)list.Count / 30f));
					}
					else if (list3.Contains(tifactionIdeologyTemplate4))
					{
						Dictionary<FactionIdeology, float> dictionary2 = dictionary;
						FactionIdeology factionIdeology = tifactionIdeologyTemplate4.ideology;
						dictionary2[factionIdeology] += Mathf.Clamp(this.education + (float)TIUtilities.RandomRange(0, 10) / 500f, 0f, num3 * ((float)list3.Count / 60f));
					}
					else if (list2.Contains(tifactionIdeologyTemplate4))
					{
						Dictionary<FactionIdeology, float> dictionary2 = dictionary;
						FactionIdeology factionIdeology = tifactionIdeologyTemplate4.ideology;
						dictionary2[factionIdeology] += Mathf.Clamp(2f * this.education + (float)TIUtilities.RandomRange(0, 10) / 100f, 0f, num3 * ((float)list2.Count / 6.5f));
					}
				}
			}
			float num6;
			for (num6 = dictionary.Sum<KeyValuePair<FactionIdeology, float>>((KeyValuePair<FactionIdeology, float> x) => x.Value) - dictionary[FactionIdeology.Undecided]; num6 > 1f; num6 = dictionary.Sum<KeyValuePair<FactionIdeology, float>>((KeyValuePair<FactionIdeology, float> x) => x.Value) - dictionary[FactionIdeology.Undecided])
			{
				bool flag = false;
				TIFactionIdeologyTemplate tifactionIdeologyTemplate5 = GameStateManager.ActiveHumanIdeologies().Except<TIFactionIdeologyTemplate>(new List<TIFactionIdeologyTemplate> { GameStateManager.UndecidedIdeology() }).SelectRandomItem<TIFactionIdeologyTemplate>();
				dictionary[tifactionIdeologyTemplate5.ideology] = Mathf.Clamp(dictionary[tifactionIdeologyTemplate5.ideology] - 0.025f, 0f, 1f);
				if (!flag)
				{
					Log.Info("Initial Ideology seed hit total of " + num6.ToString(), Array.Empty<object>());
				}
			}
			dictionary[FactionIdeology.Undecided] = 1f - num6;
			foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate6 in GameStateManager.ActiveHumanIdeologies())
			{
				this.publicOpinion.Add(tifactionIdeologyTemplate6.ideology, dictionary[tifactionIdeologyTemplate6.ideology]);
			}
			GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Influence);
			});
		}

		// Token: 0x06003767 RID: 14183 RVA: 0x001405B4 File Offset: 0x0013E7B4
		public bool PublicOpinionMonthlyChange(TIFactionState faction, float minDelta)
		{
			if (minDelta < 0f)
			{
				return this.GetPublicOpinionOfFaction(faction) - this.historyPublicOpinion[31][faction.ideology.ideology] <= minDelta;
			}
			return minDelta <= 0f || this.GetPublicOpinionOfFaction(faction) - this.historyPublicOpinion[31][faction.ideology.ideology] >= minDelta;
		}

		// Token: 0x06003768 RID: 14184 RVA: 0x0014062C File Offset: 0x0013E82C
		public void InitializeAllTrackers()
		{
			if (this.tracker_GDPChangeReason_CurrentTrackingPeriod == null)
			{
				this.tracker_GDPChangeReason_CurrentTrackingPeriod = ((TINationState.GDPChangeReason[])Enum.GetValues(typeof(TINationState.GDPChangeReason))).ToDictionary<TINationState.GDPChangeReason, TINationState.GDPChangeReason, float>((TINationState.GDPChangeReason x) => x, (TINationState.GDPChangeReason x) => 0f);
			}
			if (this.tracker_GDPChangeReason_PriorTrackingPeriod == null)
			{
				this.tracker_GDPChangeReason_PriorTrackingPeriod = ((TINationState.GDPChangeReason[])Enum.GetValues(typeof(TINationState.GDPChangeReason))).ToDictionary<TINationState.GDPChangeReason, TINationState.GDPChangeReason, float>((TINationState.GDPChangeReason x) => x, (TINationState.GDPChangeReason x) => 0f);
			}
			if (this.tracker_GDPChangeReason_AllTime == null)
			{
				this.tracker_GDPChangeReason_AllTime = ((TINationState.GDPChangeReason[])Enum.GetValues(typeof(TINationState.GDPChangeReason))).ToDictionary<TINationState.GDPChangeReason, TINationState.GDPChangeReason, float>((TINationState.GDPChangeReason x) => x, (TINationState.GDPChangeReason x) => 0f);
			}
			if (this.tracker_InequalityChangeReason_CurrentTrackingPeriod == null)
			{
				this.tracker_InequalityChangeReason_CurrentTrackingPeriod = ((TINationState.InequalityChangeReason[])Enum.GetValues(typeof(TINationState.InequalityChangeReason))).ToDictionary<TINationState.InequalityChangeReason, TINationState.InequalityChangeReason, float>((TINationState.InequalityChangeReason x) => x, (TINationState.InequalityChangeReason x) => 0f);
			}
			if (this.tracker_InequalityChangeReason_PriorTrackingPeriod == null)
			{
				this.tracker_InequalityChangeReason_PriorTrackingPeriod = ((TINationState.InequalityChangeReason[])Enum.GetValues(typeof(TINationState.InequalityChangeReason))).ToDictionary<TINationState.InequalityChangeReason, TINationState.InequalityChangeReason, float>((TINationState.InequalityChangeReason x) => x, (TINationState.InequalityChangeReason x) => 0f);
			}
			if (this.tracker_InequalityChangeReason_AllTime == null)
			{
				this.tracker_InequalityChangeReason_AllTime = ((TINationState.InequalityChangeReason[])Enum.GetValues(typeof(TINationState.InequalityChangeReason))).ToDictionary<TINationState.InequalityChangeReason, TINationState.InequalityChangeReason, float>((TINationState.InequalityChangeReason x) => x, (TINationState.InequalityChangeReason x) => 0f);
			}
			if (this.tracker_CohesionChangeReason_CurrentTrackingPeriod == null)
			{
				this.tracker_CohesionChangeReason_CurrentTrackingPeriod = ((TINationState.CohesionChangeReason[])Enum.GetValues(typeof(TINationState.CohesionChangeReason))).ToDictionary<TINationState.CohesionChangeReason, TINationState.CohesionChangeReason, float>((TINationState.CohesionChangeReason x) => x, (TINationState.CohesionChangeReason x) => 0f);
			}
			if (this.tracker_CohesionChangeReason_PriorTrackingPeriod == null)
			{
				this.tracker_CohesionChangeReason_PriorTrackingPeriod = ((TINationState.CohesionChangeReason[])Enum.GetValues(typeof(TINationState.CohesionChangeReason))).ToDictionary<TINationState.CohesionChangeReason, TINationState.CohesionChangeReason, float>((TINationState.CohesionChangeReason x) => x, (TINationState.CohesionChangeReason x) => 0f);
			}
			if (this.tracker_CohesionChangeReason_AllTime == null)
			{
				this.tracker_CohesionChangeReason_AllTime = ((TINationState.CohesionChangeReason[])Enum.GetValues(typeof(TINationState.CohesionChangeReason))).ToDictionary<TINationState.CohesionChangeReason, TINationState.CohesionChangeReason, float>((TINationState.CohesionChangeReason x) => x, (TINationState.CohesionChangeReason x) => 0f);
			}
			if (this.tracker_UnrestChangeReason_CurrentTrackingPeriod == null)
			{
				this.tracker_UnrestChangeReason_CurrentTrackingPeriod = ((TINationState.UnrestChangeReason[])Enum.GetValues(typeof(TINationState.UnrestChangeReason))).ToDictionary<TINationState.UnrestChangeReason, TINationState.UnrestChangeReason, float>((TINationState.UnrestChangeReason x) => x, (TINationState.UnrestChangeReason x) => 0f);
			}
			if (this.tracker_UnrestChangeReason_PriorTrackingPeriod == null)
			{
				this.tracker_UnrestChangeReason_PriorTrackingPeriod = ((TINationState.UnrestChangeReason[])Enum.GetValues(typeof(TINationState.UnrestChangeReason))).ToDictionary<TINationState.UnrestChangeReason, TINationState.UnrestChangeReason, float>((TINationState.UnrestChangeReason x) => x, (TINationState.UnrestChangeReason x) => 0f);
			}
			if (this.tracker_UnrestChangeReason_AllTime == null)
			{
				this.tracker_UnrestChangeReason_AllTime = ((TINationState.UnrestChangeReason[])Enum.GetValues(typeof(TINationState.UnrestChangeReason))).ToDictionary<TINationState.UnrestChangeReason, TINationState.UnrestChangeReason, float>((TINationState.UnrestChangeReason x) => x, (TINationState.UnrestChangeReason x) => 0f);
			}
			if (this.tracker_EducationChangeReason_CurrentTrackingPeriod == null)
			{
				this.tracker_EducationChangeReason_CurrentTrackingPeriod = ((TINationState.EducationChangeReason[])Enum.GetValues(typeof(TINationState.EducationChangeReason))).ToDictionary<TINationState.EducationChangeReason, TINationState.EducationChangeReason, float>((TINationState.EducationChangeReason x) => x, (TINationState.EducationChangeReason x) => 0f);
			}
			if (this.tracker_EducationChangeReason_PriorTrackingPeriod == null)
			{
				this.tracker_EducationChangeReason_PriorTrackingPeriod = ((TINationState.EducationChangeReason[])Enum.GetValues(typeof(TINationState.EducationChangeReason))).ToDictionary<TINationState.EducationChangeReason, TINationState.EducationChangeReason, float>((TINationState.EducationChangeReason x) => x, (TINationState.EducationChangeReason x) => 0f);
			}
			if (this.tracker_EducationChangeReason_AllTime == null)
			{
				this.tracker_EducationChangeReason_AllTime = ((TINationState.EducationChangeReason[])Enum.GetValues(typeof(TINationState.EducationChangeReason))).ToDictionary<TINationState.EducationChangeReason, TINationState.EducationChangeReason, float>((TINationState.EducationChangeReason x) => x, (TINationState.EducationChangeReason x) => 0f);
			}
			if (this.tracker_DemocracyChangeReason_CurrentTrackingPeriod == null)
			{
				this.tracker_DemocracyChangeReason_CurrentTrackingPeriod = ((TINationState.DemocracyChangeReason[])Enum.GetValues(typeof(TINationState.DemocracyChangeReason))).ToDictionary<TINationState.DemocracyChangeReason, TINationState.DemocracyChangeReason, float>((TINationState.DemocracyChangeReason x) => x, (TINationState.DemocracyChangeReason x) => 0f);
			}
			if (this.tracker_DemocracyChangeReason_PriorTrackingPeriod == null)
			{
				this.tracker_DemocracyChangeReason_PriorTrackingPeriod = ((TINationState.DemocracyChangeReason[])Enum.GetValues(typeof(TINationState.DemocracyChangeReason))).ToDictionary<TINationState.DemocracyChangeReason, TINationState.DemocracyChangeReason, float>((TINationState.DemocracyChangeReason x) => x, (TINationState.DemocracyChangeReason x) => 0f);
			}
			if (this.tracker_DemocracyChangeReason_AllTime == null)
			{
				this.tracker_DemocracyChangeReason_AllTime = ((TINationState.DemocracyChangeReason[])Enum.GetValues(typeof(TINationState.DemocracyChangeReason))).ToDictionary<TINationState.DemocracyChangeReason, TINationState.DemocracyChangeReason, float>((TINationState.DemocracyChangeReason x) => x, (TINationState.DemocracyChangeReason x) => 0f);
			}
			if (this.tracker_PCGDP_ByQuarter == null)
			{
				this.tracker_PCGDP_ByQuarter = new Dictionary<int, float>();
				if (this.extant)
				{
					for (int i = 0; i <= TITimeState.CurrentQuarter(); i++)
					{
						this.tracker_PCGDP_ByQuarter.Add(i, this.perCapitaGDP);
					}
					if (this.template.yearofHighestGDP != null)
					{
						Dictionary<int, float> dictionary = this.tracker_PCGDP_ByQuarter;
						TINationTemplate template = this.template;
						dictionary.Add(((((template != null) ? template.yearofHighestGDP : null) ?? 2000) - TITimeState.Now().year) * 4, this.template.highestPerCapitaGDP * this.perCapitaGDP);
					}
					if (this.tracker_PCGDP_ByQuarter.Keys.Any<int>((int x) => x < 0))
					{
						int num = this.tracker_PCGDP_ByQuarter.Keys.Min();
						float num2 = (this.tracker_PCGDP_ByQuarter[num] - this.tracker_PCGDP_ByQuarter[0]) / (float)num;
						for (int j = num + 1; j < 0; j++)
						{
							this.tracker_PCGDP_ByQuarter.Add(j, this.tracker_PCGDP_ByQuarter[j - 1] + num2);
						}
					}
				}
			}
			if (TIGlobalConfig.globalConfig.fullQuarterlyTracking)
			{
				if (this.tracker_GDP_ByQuarter == null)
				{
					this.tracker_GDP_ByQuarter = new Dictionary<int, float> { 
					{
						0,
						(float)this.GDP
					} };
				}
				if (this.tracker_Inequality_ByQuarter == null)
				{
					this.tracker_Inequality_ByQuarter = new Dictionary<int, float> { { 0, this.inequality } };
				}
				if (this.tracker_Cohesion_ByQuarter == null)
				{
					this.tracker_Cohesion_ByQuarter = new Dictionary<int, float> { { 0, this.cohesion } };
				}
				if (this.tracker_Unrest_ByQuarter == null)
				{
					this.tracker_Unrest_ByQuarter = new Dictionary<int, float> { { 0, this.unrest } };
				}
				if (this.tracker_Education_ByQuarter == null)
				{
					this.tracker_Education_ByQuarter = new Dictionary<int, float> { { 0, this.education } };
				}
				if (this.tracker_Democracy_ByQuarter == null)
				{
					this.tracker_Democracy_ByQuarter = new Dictionary<int, float> { { 0, this.democracy } };
				}
			}
			else
			{
				Dictionary<int, float> dictionary2 = this.tracker_GDP_ByQuarter;
				if (dictionary2 != null)
				{
					dictionary2.Clear();
				}
				Dictionary<int, float> dictionary3 = this.tracker_Inequality_ByQuarter;
				if (dictionary3 != null)
				{
					dictionary3.Clear();
				}
				Dictionary<int, float> dictionary4 = this.tracker_Cohesion_ByQuarter;
				if (dictionary4 != null)
				{
					dictionary4.Clear();
				}
				Dictionary<int, float> dictionary5 = this.tracker_Unrest_ByQuarter;
				if (dictionary5 != null)
				{
					dictionary5.Clear();
				}
				Dictionary<int, float> dictionary6 = this.tracker_Education_ByQuarter;
				if (dictionary6 != null)
				{
					dictionary6.Clear();
				}
				Dictionary<int, float> dictionary7 = this.tracker_Democracy_ByQuarter;
				if (dictionary7 != null)
				{
					dictionary7.Clear();
				}
			}
			if (this.historyCohesion == null)
			{
				this.historyCohesion = new List<float>(32);
			}
			if (this.historyCohesionRestState == null)
			{
				this.historyCohesionRestState = new List<float>(32);
			}
			if (this.historyDemocracy == null)
			{
				this.historyDemocracy = new List<float>(32);
			}
			if (this.historyUnrest == null)
			{
				this.historyUnrest = new List<float>(32);
			}
			if (this.historyUnrestRestState == null)
			{
				this.historyUnrestRestState = new List<float>(32);
			}
			if (this.historyInequality == null)
			{
				this.historyInequality = new List<float>(32);
			}
			if (this.historyGDP == null)
			{
				this.historyGDP = new List<double>(32);
			}
			if (this.historyEducation == null)
			{
				this.historyEducation = new List<float>(32);
			}
			if (this.historyBoost == null)
			{
				this.historyBoost = new List<float>(32);
			}
			if (this.historyPopulation == null)
			{
				this.historyPopulation = new List<float>(32);
			}
			if (this.historySustainability == null)
			{
				this.historySustainability = new List<float>(32);
			}
			if (this.historyMissionControl == null)
			{
				this.historyMissionControl = new List<int>(32);
			}
			if (this.historySpaceFunding == null)
			{
				this.historySpaceFunding = new List<float>(32);
			}
			if (this.historyResearch == null)
			{
				this.historyResearch = new List<float>(32);
			}
			if (this.historyMiltech == null)
			{
				this.historyMiltech = new List<float>(32);
			}
			if (this.historyNukes == null)
			{
				this.historyNukes = new List<int>(32);
			}
			if (this.historyInvestmentPoints == null)
			{
				this.historyInvestmentPoints = new List<float>(32);
			}
			if (this.historyNumRegions == null)
			{
				this.historyNumRegions = new List<int>(32);
			}
			if (this.historyPublicOpinion == null)
			{
				this.historyPublicOpinion = new List<Dictionary<FactionIdeology, float>>();
			}
			if (this.historyWarStatus == null)
			{
				this.historyWarStatus = new List<float>(32);
			}
		}

		// Token: 0x06003769 RID: 14185 RVA: 0x00141174 File Offset: 0x0013F374
		protected void FillOutPastTrackerDataForNewNation()
		{
			for (int i = 0; i < 32; i++)
			{
				this.historyInvestmentPoints.Add(this.BaseInvestmentPoints_month());
				this.historyCohesion.Add(this.cohesion);
				this.historyCohesionRestState.Add(this.cohesionRestState);
				this.historyDemocracy.Add(this.democracy);
				this.historyUnrest.Add(this.unrest);
				this.historyUnrestRestState.Add(this.unrestRestState);
				this.historyInequality.Add(this.inequality);
				this.historyGDP.Add(this.GDP);
				this.historyEducation.Add(this.education);
				this.historySpaceFunding.Add(this.spaceFunding_month);
				this.historyMissionControl.Add(this.currentMissionControl);
				this.historyBoost.Add(this.currentBoost_month);
				this.historyResearch.Add(this.research_month);
				this.historyMiltech.Add(this.militaryTechLevel);
				this.historyNukes.Add(this.numNuclearWeapons);
				this.historyWarStatus.Add(0f);
				this.historyNumRegions.Add(this.regions.Count);
				this.historyPopulation.Add(this.population_Millions);
				this.historySustainability.Add(this.sustainability);
				this.historyPublicOpinion.Add(this.publicOpinion.ToDictionary<KeyValuePair<FactionIdeology, float>, FactionIdeology, float>((KeyValuePair<FactionIdeology, float> x) => x.Key, (KeyValuePair<FactionIdeology, float> x) => x.Value));
			}
		}

		// Token: 0x0600376A RID: 14186 RVA: 0x00141330 File Offset: 0x0013F530
		protected void BringTrackerDataUpToDate()
		{
			this.historyInvestmentPoints.AddSizeItemsToDefault(32, 0f);
			this.historyCohesion.AddSizeItemsToDefault(32, 0f);
			this.historyCohesionRestState.AddSizeItemsToDefault(32, this.cohesionRestState);
			this.historyDemocracy.AddSizeItemsToDefault(32, 0f);
			this.historyUnrest.AddSizeItemsToDefault(32, 0f);
			this.historyUnrestRestState.AddSizeItemsToDefault(32, this.unrestRestState);
			this.historyInequality.AddSizeItemsToDefault(32, 0f);
			this.historyGDP.AddSizeItemsToDefault(32, 0.0);
			this.historyEducation.AddSizeItemsToDefault(32, 0f);
			this.historyBoost.AddSizeItemsToDefault(32, 0f);
			this.historySpaceFunding.AddSizeItemsToDefault(32, 0f);
			this.historyMissionControl.AddSizeItemsToDefault(32, 0);
			this.historyResearch.AddSizeItemsToDefault(32, 0f);
			this.historyMiltech.AddSizeItemsToDefault(32, 0f);
			this.historyNukes.AddSizeItemsToDefault(32, 0);
			this.historyWarStatus.AddSizeItemsToDefault(32, 0f);
			this.historyPopulation.AddSizeItemsToDefault(32, 0f);
			this.historySustainability.AddSizeItemsToDefault(32, 0f);
			this.historyNumRegions.AddSizeItemsToDefault(32, 0);
			if (this.historyPublicOpinion.Count < 32)
			{
				for (int i = this.historyPublicOpinion.Count - 1; i < 32; i++)
				{
					this.historyPublicOpinion.Add(this.historyPublicOpinion[i - 1].ToDictionary<KeyValuePair<FactionIdeology, float>, FactionIdeology, float>((KeyValuePair<FactionIdeology, float> x) => x.Key, (KeyValuePair<FactionIdeology, float> x) => x.Value));
				}
			}
			this.tracker_CohesionChangeReason_AllTime.CorrectEnumKeyedDictionary(0f);
			this.tracker_CohesionChangeReason_CurrentTrackingPeriod.CorrectEnumKeyedDictionary(0f);
			this.tracker_CohesionChangeReason_PriorTrackingPeriod.CorrectEnumKeyedDictionary(0f);
			this.tracker_InequalityChangeReason_CurrentTrackingPeriod.CorrectEnumKeyedDictionary(0f);
			this.tracker_InequalityChangeReason_PriorTrackingPeriod.CorrectEnumKeyedDictionary(0f);
			this.tracker_InequalityChangeReason_AllTime.CorrectEnumKeyedDictionary(0f);
		}

		// Token: 0x0600376B RID: 14187 RVA: 0x0014158C File Offset: 0x0013F78C
		public void ResetPeriodicTrackers()
		{
			this.tracker_GDPChangeReason_PriorTrackingPeriod = new Dictionary<TINationState.GDPChangeReason, float>(this.tracker_GDPChangeReason_CurrentTrackingPeriod);
			this.tracker_GDPChangeReason_CurrentTrackingPeriod = ((TINationState.GDPChangeReason[])Enum.GetValues(typeof(TINationState.GDPChangeReason))).ToDictionary<TINationState.GDPChangeReason, TINationState.GDPChangeReason, float>((TINationState.GDPChangeReason x) => x, (TINationState.GDPChangeReason x) => 0f);
			this.tracker_InequalityChangeReason_PriorTrackingPeriod = new Dictionary<TINationState.InequalityChangeReason, float>(this.tracker_InequalityChangeReason_CurrentTrackingPeriod);
			this.tracker_InequalityChangeReason_CurrentTrackingPeriod = ((TINationState.InequalityChangeReason[])Enum.GetValues(typeof(TINationState.InequalityChangeReason))).ToDictionary<TINationState.InequalityChangeReason, TINationState.InequalityChangeReason, float>((TINationState.InequalityChangeReason x) => x, (TINationState.InequalityChangeReason x) => 0f);
			this.tracker_CohesionChangeReason_PriorTrackingPeriod = new Dictionary<TINationState.CohesionChangeReason, float>(this.tracker_CohesionChangeReason_CurrentTrackingPeriod);
			this.tracker_CohesionChangeReason_CurrentTrackingPeriod = ((TINationState.CohesionChangeReason[])Enum.GetValues(typeof(TINationState.CohesionChangeReason))).ToDictionary<TINationState.CohesionChangeReason, TINationState.CohesionChangeReason, float>((TINationState.CohesionChangeReason x) => x, (TINationState.CohesionChangeReason x) => 0f);
			this.tracker_UnrestChangeReason_PriorTrackingPeriod = new Dictionary<TINationState.UnrestChangeReason, float>(this.tracker_UnrestChangeReason_CurrentTrackingPeriod);
			this.tracker_UnrestChangeReason_CurrentTrackingPeriod = ((TINationState.UnrestChangeReason[])Enum.GetValues(typeof(TINationState.UnrestChangeReason))).ToDictionary<TINationState.UnrestChangeReason, TINationState.UnrestChangeReason, float>((TINationState.UnrestChangeReason x) => x, (TINationState.UnrestChangeReason x) => 0f);
			this.tracker_EducationChangeReason_PriorTrackingPeriod = new Dictionary<TINationState.EducationChangeReason, float>(this.tracker_EducationChangeReason_CurrentTrackingPeriod);
			this.tracker_EducationChangeReason_CurrentTrackingPeriod = ((TINationState.EducationChangeReason[])Enum.GetValues(typeof(TINationState.EducationChangeReason))).ToDictionary<TINationState.EducationChangeReason, TINationState.EducationChangeReason, float>((TINationState.EducationChangeReason x) => x, (TINationState.EducationChangeReason x) => 0f);
			this.tracker_DemocracyChangeReason_PriorTrackingPeriod = new Dictionary<TINationState.DemocracyChangeReason, float>(this.tracker_DemocracyChangeReason_CurrentTrackingPeriod);
			this.tracker_DemocracyChangeReason_CurrentTrackingPeriod = ((TINationState.DemocracyChangeReason[])Enum.GetValues(typeof(TINationState.DemocracyChangeReason))).ToDictionary<TINationState.DemocracyChangeReason, TINationState.DemocracyChangeReason, float>((TINationState.DemocracyChangeReason x) => x, (TINationState.DemocracyChangeReason x) => 0f);
		}

		// Token: 0x0600376C RID: 14188 RVA: 0x00141830 File Offset: 0x0013FA30
		public void UpdateDailyTrackers()
		{
			this.historyCohesion.Insert(0, this.cohesion);
			this.historyCohesion.RemoveRange(32, this.historyCohesion.Count - 32);
			this.historyCohesionRestState.Insert(0, this.cohesionRestState);
			this.historyCohesionRestState.RemoveRange(32, this.historyCohesionRestState.Count - 32);
			this.historyDemocracy.Insert(0, this.democracy);
			this.historyDemocracy.RemoveRange(32, this.historyDemocracy.Count - 32);
			this.historyUnrest.Insert(0, this.unrest);
			this.historyUnrest.RemoveRange(32, this.historyUnrest.Count - 32);
			this.historyUnrestRestState.Insert(0, this.unrestRestState);
			this.historyUnrestRestState.RemoveRange(32, this.historyUnrestRestState.Count - 32);
			this.historyInequality.Insert(0, this.inequality);
			this.historyInequality.RemoveRange(32, this.historyInequality.Count - 32);
			this.historyEducation.Insert(0, this.education);
			this.historyEducation.RemoveRange(32, this.historyEducation.Count - 32);
			this.historyGDP.Insert(0, this.GDP);
			this.historyGDP.RemoveRange(32, this.historyGDP.Count - 32);
			this.historyPopulation.Insert(0, this.population_Millions);
			this.historyPopulation.RemoveRange(32, this.historyPopulation.Count - 32);
			this.historySustainability.Insert(0, this.sustainability);
			this.historySustainability.RemoveRange(32, this.historySustainability.Count - 32);
			this.historyEducation.Insert(0, this.education);
			this.historyEducation.RemoveRange(32, this.historyEducation.Count - 32);
			this.historyBoost.Insert(0, this.currentBoost_month);
			this.historyBoost.RemoveRange(32, this.historyBoost.Count - 32);
			this.historySpaceFunding.Insert(0, this.spaceFunding_month);
			this.historySpaceFunding.RemoveRange(32, this.historySpaceFunding.Count - 32);
			this.historyMiltech.Insert(0, this.militaryTechLevel);
			this.historyMiltech.RemoveRange(32, this.historyMiltech.Count - 32);
			this.historyResearch.Insert(0, this.research_month);
			this.historyResearch.RemoveRange(32, this.historyResearch.Count - 32);
			this.historyMissionControl.Insert(0, this.currentMissionControl);
			this.historyMissionControl.RemoveRange(32, this.historyMissionControl.Count - 32);
			this.historyInvestmentPoints.Insert(0, this.BaseInvestmentPoints_month());
			this.historyInvestmentPoints.RemoveRange(32, this.historyInvestmentPoints.Count - 32);
			this.historyNukes.Insert(0, this.numNuclearWeapons);
			this.historyNukes.RemoveRange(32, this.historyNukes.Count - 32);
			this.historyNumRegions.Insert(0, this.regions.Count);
			this.historyNumRegions.RemoveRange(32, this.historyNumRegions.Count - 32);
			this.historyPublicOpinion.Insert(0, this.publicOpinion.ToDictionary<KeyValuePair<FactionIdeology, float>, FactionIdeology, float>((KeyValuePair<FactionIdeology, float> x) => x.Key, (KeyValuePair<FactionIdeology, float> x) => x.Value));
			this.historyPublicOpinion.RemoveRange(32, this.historyPublicOpinion.Count - 32);
		}

		// Token: 0x0600376D RID: 14189 RVA: 0x00141C0C File Offset: 0x0013FE0C
		public void UpdateQuarterlyTrackers()
		{
			this.tracker_PCGDP_ByQuarter[TITimeState.CurrentQuarter()] = this.perCapitaGDP;
			if (TIGlobalConfig.globalConfig.fullQuarterlyTracking)
			{
				this.tracker_GDP_ByQuarter[TITimeState.CurrentQuarter()] = (float)this.GDP;
				this.tracker_Inequality_ByQuarter[TITimeState.CurrentQuarter()] = this.inequality;
				this.tracker_Cohesion_ByQuarter[TITimeState.CurrentQuarter()] = this.cohesion;
				this.tracker_Unrest_ByQuarter[TITimeState.CurrentQuarter()] = this.unrest;
				this.tracker_Education_ByQuarter[TITimeState.CurrentQuarter()] = this.education;
				this.tracker_Democracy_ByQuarter[TITimeState.CurrentQuarter()] = this.democracy;
			}
		}

		// Token: 0x17000951 RID: 2385
		// (get) Token: 0x0600376E RID: 14190 RVA: 0x00141CC3 File Offset: 0x0013FEC3
		// (set) Token: 0x0600376F RID: 14191 RVA: 0x00141CCB File Offset: 0x0013FECB
		public double GDP { get; private set; }

		// Token: 0x06003770 RID: 14192 RVA: 0x00141CD4 File Offset: 0x0013FED4
		public void ModifyGDP(double value, TINationState.GDPChangeReason reason)
		{
			bool flag = value > 0.0 && (this.missionControl >= this.maxMissionControl || this.spaceFundingIncome_year >= this.maxFunding_year);
			this.GDP += value;
			if (this.GDP < (double)(this.population * 100f))
			{
				this.GDP = (double)(this.population * 100f);
			}
			this.economyScore = (float)Mathd.Pow(this.GDP / 1000000000.0, (double)TIGlobalConfig.globalConfig.controlPointIPScaling) * TIGlobalConfig.globalConfig.controlPointIPFactor;
			this.missionDifficultyEconomyScore = (float)Mathd.Pow(this.GDP / (double)TIGlobalValuesState.PCGDPToRaiseMissionBaseDifficultyBy1, (double)TIGlobalConfig.globalConfig.TIMissionModifier_NationEconomyPower) * GameStateManager.Time().template.GDPDefenseModifier;
			this.SetDataDirty();
			this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Research);
			});
			Dictionary<TINationState.GDPChangeReason, float> dictionary = this.tracker_GDPChangeReason_CurrentTrackingPeriod;
			dictionary[reason] += (float)value;
			dictionary = this.tracker_GDPChangeReason_AllTime;
			dictionary[reason] += (float)value;
			if (flag)
			{
				this.PossiblePriorityValidationChange(false);
			}
		}

		// Token: 0x06003771 RID: 14193 RVA: 0x00141E1C File Offset: 0x0014001C
		public void GDPPctChange(float frac, TINationState.GDPChangeReason reason)
		{
			double num = this.GDP * (double)(1f + frac);
			this.ModifyGDP(num - this.GDP, reason);
		}

		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x06003772 RID: 14194 RVA: 0x00141E48 File Offset: 0x00140048
		public string GDPstring
		{
			get
			{
				return Loc.T("UI.Nation.AbbrGDP", new object[] { (this.GDP / 1000000000.0).ToString("N0") });
			}
		}

		// Token: 0x06003773 RID: 14195 RVA: 0x00141E88 File Offset: 0x00140088
		public string HistoryGDPStr(int days)
		{
			return Loc.T("UI.Nation.AbbrGDP", new object[] { ((this.GDP - this.historyGDP[days]) / 1000000000.0).ToString("N0") });
		}

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x06003774 RID: 14196 RVA: 0x00141ED2 File Offset: 0x001400D2
		private int getNumControlPoints
		{
			get
			{
				return Mathf.Clamp(this.getNumControlPoints_unclamped, 1, 6);
			}
		}

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x06003775 RID: 14197 RVA: 0x00141EE1 File Offset: 0x001400E1
		private int getNumControlPoints_unclamped
		{
			get
			{
				return Mathf.Max(Mathf.RoundToInt((float)Mathd.Pow(this.GDP / 1000000000.0, (double)TemplateManager.global.controlPointCountScaling) / TemplateManager.global.controlPointScalingDivisor), 1);
			}
		}

		// Token: 0x17000955 RID: 2389
		// (get) Token: 0x06003776 RID: 14198 RVA: 0x00141F1C File Offset: 0x0014011C
		public float perCapitaGDP
		{
			get
			{
				float population_Millions = this.population_Millions;
				if (population_Millions == 0f)
				{
					return 0f;
				}
				return (float)(this.GDP / ((double)population_Millions * 1000000.0));
			}
		}

		// Token: 0x17000956 RID: 2390
		// (get) Token: 0x06003777 RID: 14199 RVA: 0x00141F54 File Offset: 0x00140154
		public string perCapitaGDPstr
		{
			get
			{
				return Loc.T("UI.Global.DollarValue", new object[] { this.perCapitaGDP.ToString("N0") });
			}
		}

		// Token: 0x06003778 RID: 14200 RVA: 0x00141F87 File Offset: 0x00140187
		public float HistoryPerCapitaGDP(int days)
		{
			return (float)(this.historyGDP[days] / ((double)this.historyPopulation[days] * 1000000.0));
		}

		// Token: 0x06003779 RID: 14201 RVA: 0x00141FB0 File Offset: 0x001401B0
		public float PerCapitaGDPFractionOfHighest(int includedQuarters = 40)
		{
			float num = Mathf.Max(this.tracker_PCGDP_ByQuarter.Where<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Key >= TITimeState.CurrentQuarter() - includedQuarters).Max<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Value), 100f);
			return this.perCapitaGDP / num;
		}

		// Token: 0x0600377A RID: 14202 RVA: 0x00142018 File Offset: 0x00140218
		public float PerCapitaGDPFractionOfLowest(int includedQuarters = 40)
		{
			float num = Mathf.Max(this.tracker_PCGDP_ByQuarter.Where<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Key >= TITimeState.CurrentQuarter() - includedQuarters).Min<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Value), 100f);
			return this.perCapitaGDP / num;
		}

		// Token: 0x17000957 RID: 2391
		// (get) Token: 0x0600377B RID: 14203 RVA: 0x00142080 File Offset: 0x00140280
		// (set) Token: 0x0600377C RID: 14204 RVA: 0x00142088 File Offset: 0x00140288
		public float inequality { get; private set; }

		// Token: 0x17000958 RID: 2392
		// (get) Token: 0x0600377D RID: 14205 RVA: 0x00142091 File Offset: 0x00140291
		public bool inequalityWarning
		{
			get
			{
				return this.inequality - this.historyInequality[31] > 0f || this.inequality > TemplateManager.global.badInequality;
			}
		}

		// Token: 0x17000959 RID: 2393
		// (get) Token: 0x0600377E RID: 14206 RVA: 0x001420C2 File Offset: 0x001402C2
		public bool severeInequalityWarning
		{
			get
			{
				return this.inequality > TemplateManager.global.severeInequality;
			}
		}

		// Token: 0x0600377F RID: 14207 RVA: 0x001420D8 File Offset: 0x001402D8
		public void AddToInequality(float value, TINationState.InequalityChangeReason reason)
		{
			float num = this.inequality + value - 9f;
			this.inequality = Mathf.Clamp(this.inequality + value, 1f, 9f);
			if (num > 0f)
			{
				this.AddToCohesion(-num, TINationState.CohesionChangeReason.CohesionReason_InequalityAboveMax);
				this.AddToUnrest(num, TINationState.UnrestChangeReason.UnrestReason_InequalityAboveMax, 10f);
			}
			Dictionary<TINationState.InequalityChangeReason, float> dictionary = this.tracker_InequalityChangeReason_CurrentTrackingPeriod;
			dictionary[reason] += value;
			dictionary = this.tracker_InequalityChangeReason_AllTime;
			dictionary[reason] += value;
			this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Research);
			});
			this.SetDataDirty();
		}

		// Token: 0x1700095A RID: 2394
		// (get) Token: 0x06003780 RID: 14208 RVA: 0x00142194 File Offset: 0x00140394
		public string InequalityDescriptiveString
		{
			get
			{
				if (this.inequality < 2f)
				{
					return Loc.T("UI.Nation.Inequality2");
				}
				if (this.inequality < 3f)
				{
					return Loc.T("UI.Nation.Inequality3");
				}
				if (this.inequality < 4f)
				{
					return Loc.T("UI.Nation.Inequality4");
				}
				if (this.inequality < 5f)
				{
					return Loc.T("UI.Nation.Inequality5");
				}
				if (this.inequality < 6f)
				{
					return Loc.T("UI.Nation.Inequality6");
				}
				return Loc.T("UI.Nation.Inequality7");
			}
		}

		// Token: 0x06003781 RID: 14209 RVA: 0x00142224 File Offset: 0x00140424
		public string GetInequalityDescriptiveStringAndValue(int decimalPlaces = 1)
		{
			return Loc.T("UI.Nation.NationalStatStringValue", new object[]
			{
				this.InequalityDescriptiveString,
				TIUtilities.FormatSmallNumber(this.inequality, decimalPlaces, decimalPlaces, true, false)
			});
		}

		// Token: 0x06003782 RID: 14210 RVA: 0x0014225C File Offset: 0x0014045C
		public static float MeanAnnualGDPDamage(float tempAnomaly_C, float inequality)
		{
			float num = 0f;
			if (tempAnomaly_C > 0.25f)
			{
				float num2 = tempAnomaly_C - 0.25f;
				num = 0.14577f * num2 * num2 + 0.31839f * num2;
				num *= Mathf.Pow(1.14f, inequality);
				if (num2 >= 5f)
				{
					float num3 = Mathf.Clamp((num2 + inequality) / 10f, 1f, 1.5f);
					num *= num3;
				}
				num /= 100f;
				num *= -1f;
			}
			else if (tempAnomaly_C < 0f)
			{
				float num4 = Mathf.Abs(tempAnomaly_C);
				num = num4 * -0.04032f;
				if (tempAnomaly_C < -7f)
				{
					num += (num4 - 7f) * -0.04032f;
					if ((double)tempAnomaly_C < -10.5)
					{
						num += (num4 - 10.5f) * -0.04032f * 10f;
					}
				}
			}
			return Mathf.Clamp(num, -0.99f, 0f);
		}

		// Token: 0x06003783 RID: 14211 RVA: 0x0014233C File Offset: 0x0014053C
		public void MonthlyTemperatureEconomicImpact(float tempAnomaly_C, float CO2_ppm)
		{
			float num = 0f;
			foreach (TIRegionState tiregionState in this.regions)
			{
				num += (float)(tiregionState.template.environment - EnvironmentType.Beneficiary) * tiregionState.populationInMillions;
			}
			num /= this.population_Millions;
			num = Mathf.Clamp(num, 0f, 2f);
			float num2 = -1f * Mathf.Max(num * TINationState.MeanAnnualGDPDamage(tempAnomaly_C, this.inequality), -0.99f);
			if (num2 > 0f)
			{
				float num3 = Mathf.Clamp(1f - Mathf.Pow(1f - num2, 0.083333336f), 0f, 0.99f);
				this.GDPPctChange(-num3, TINationState.GDPChangeReason.GDPReason_ClimateChange);
				this.AddToInequality(num3 / 5f, TINationState.InequalityChangeReason.InqReason_ClimateChange);
			}
			if (CO2_ppm > 945f && this.education > 9.45f)
			{
				this.AddToEducation(-Mathf.Min((CO2_ppm - 945f) * 0.005f, 0.1f), TINationState.EducationChangeReason.EducationReason_CO2Poisoning);
			}
		}

		// Token: 0x06003784 RID: 14212 RVA: 0x0014245C File Offset: 0x0014065C
		public Tuple<double, double, double> GHGsFromEconomy_tons(bool monthly, float proposedSustainabilityChange = 0f)
		{
			float num = Mathf.Min(this.perCapitaGDP / 15000f, 1f);
			float num2 = Mathf.Min(this.perCapitaGDP / 7500f, 1f);
			double num3 = this.GDP / 1000000000.0;
			double num4 = num3 * 275000.0;
			double num5 = num3 * 275000.0;
			num5 += (double)this.population * 2.41 * (double)num * (double)num * (double)num2 * (double)num2;
			num5 += Mathd.Min((double)this.oilRegions * 250000000.0, num4 / 10.0);
			num5 *= (double)(this.sustainability + proposedSustainabilityChange);
			if (monthly)
			{
				num5 /= 12.0;
			}
			double num6 = num5 * 0.8230000138282776 * 0.4000000059604645;
			double num7 = num5 * 0.11500000208616257 * 1.0 / 21.0;
			double num8 = num5 * 0.06199999898672104 * 1.0 / 289.0;
			return new Tuple<double, double, double>(num6, num7, num8);
		}

		// Token: 0x06003785 RID: 14213 RVA: 0x0014257D File Offset: 0x0014077D
		public static double CO2toPPM(double input_tons)
		{
			return input_tons / 7820557000.0;
		}

		// Token: 0x06003786 RID: 14214 RVA: 0x0014258A File Offset: 0x0014078A
		public static double CH4toPPM(double input_tons)
		{
			return input_tons / 2850308000.0;
		}

		// Token: 0x06003787 RID: 14215 RVA: 0x00142597 File Offset: 0x00140797
		public static double N2OtoPPM(double input_tons)
		{
			return input_tons / 7821110000.0;
		}

		// Token: 0x06003788 RID: 14216 RVA: 0x001425A4 File Offset: 0x001407A4
		public void ProcessMonthlyGHGsFromEconomy()
		{
			Tuple<double, double, double> tuple = this.GHGsFromEconomy_tons(true, 0f);
			TIGlobalValuesState.GlobalValues.AddCO2_ppm((float)TINationState.CO2toPPM(tuple.Item1), GHGSources.Nations);
			TIGlobalValuesState.GlobalValues.AddCH4_ppm((float)TINationState.CH4toPPM(tuple.Item2), GHGSources.Nations);
			TIGlobalValuesState.GlobalValues.AddN2O_ppm((float)TINationState.N2OtoPPM(tuple.Item3), GHGSources.Nations);
		}

		// Token: 0x06003789 RID: 14217 RVA: 0x00142604 File Offset: 0x00140804
		public static string SustainabilityValueForDisplay(float sustainability)
		{
			if (sustainability <= 0f)
			{
				return 10.ToString();
			}
			if ((double)(1f / sustainability) > 9.99)
			{
				return "9.99+";
			}
			if ((double)(1f / sustainability) > 9.9)
			{
				return "9.9+";
			}
			return TIUtilities.FormatSmallNumber(Mathf.Min(9.9f, 1f / sustainability), 3, 3, true, false);
		}

		// Token: 0x0600378A RID: 14218 RVA: 0x00142670 File Offset: 0x00140870
		public string SustainabilityChangeForDisplay(float proposedChange)
		{
			float num = this.sustainability + proposedChange;
			if (this.sustainability > 0f)
			{
				if (this.sustainability <= 0.1f)
				{
					return Loc.T("UI.Nation.ASmallAmount");
				}
				return TIUtilities.FormatSmallNumber(1f / num - 1f / this.sustainability, 7, 0, true, false);
			}
			else
			{
				if (proposedChange != 0f)
				{
					return TIUtilities.FormatSmallNumber(-num, 7, 0, true, false);
				}
				return 0.ToString("N0");
			}
		}

		// Token: 0x0600378B RID: 14219 RVA: 0x001426EC File Offset: 0x001408EC
		public string SustainabilityIcon()
		{
			float sustainability = this.sustainability;
			if (sustainability <= 0f)
			{
				return "icons_2d/ICO_GHG_emission_5";
			}
			if (sustainability < 0.33333334f)
			{
				return "icons_2d/ICO_GHG_emission_4";
			}
			if (sustainability < 0.6666667f)
			{
				return "icons_2d/ICO_GHG_emission_3";
			}
			if (sustainability <= 1f)
			{
				return "icons_2d/ICO_GHG_emission_2";
			}
			return "icons_2d/ICO_GHG_emission_1";
		}

		// Token: 0x0600378C RID: 14220 RVA: 0x00142740 File Offset: 0x00140940
		public string SustainabilityIconInlinePath()
		{
			float sustainability = this.sustainability;
			if (sustainability <= 0f)
			{
				return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Green;
			}
			if (sustainability < 0.33333334f)
			{
				return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Blue;
			}
			if (sustainability < 0.6666667f)
			{
				return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Yellow;
			}
			if (sustainability <= 1f)
			{
				return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Orange;
			}
			return TIGlobalConfig.globalConfig.sustainabilityInlineSpritePath_Red;
		}

		// Token: 0x0600378D RID: 14221 RVA: 0x001427AA File Offset: 0x001409AA
		public void AddToSustainability(float value)
		{
			this.sustainability = Mathf.Clamp(this.sustainability + value, TINationState.BestCurrentSustainabilityValue(false), 10f);
		}

		// Token: 0x0600378E RID: 14222 RVA: 0x001427CA File Offset: 0x001409CA
		public void SetSustainability(float value, bool clamp = true)
		{
			if (clamp)
			{
				this.sustainability = Mathf.Clamp(value, TINationState.BestCurrentSustainabilityValue(false), 10f);
				return;
			}
			this.sustainability = value;
		}

		// Token: 0x0600378F RID: 14223 RVA: 0x001427F0 File Offset: 0x001409F0
		public static float BestCurrentSustainabilityValue(bool forceUpdate)
		{
			if (Time.frameCount != TINationState._bestCurrentSustainabilityFrame || forceUpdate)
			{
				TIGlobalValuesState globalValues = TIGlobalValuesState.GlobalValues;
				float num = ((globalValues != null) ? globalValues.initialSustainabilityMin : 0f);
				TIGlobalValuesState globalValues2 = TIGlobalValuesState.GlobalValues;
				float num2 = ((globalValues2 != null) ? globalValues2.sustainabilityHelperModifier : 1f) * TIEffectsState.SumEffectsModifiers(Context.Environment_BestSustainabilityValue, GameStateManager.AllHumanFactions()[0], num, null);
				TINationState._cachedBestCurrentSustainabilityValue = Mathf.Max(0f, num + num2);
				TINationState._bestCurrentSustainabilityFrame = TIFrameCounter.FrameCount;
			}
			return TINationState._cachedBestCurrentSustainabilityValue;
		}

		// Token: 0x06003790 RID: 14224 RVA: 0x00142870 File Offset: 0x00140A70
		public string BestCurrentSustainabilityValueForDisplay()
		{
			float num = TINationState.BestCurrentSustainabilityValue(false);
			if (num <= 0f)
			{
				return 10.ToString();
			}
			if ((double)(1f / num) > 9.9)
			{
				return "9.9+";
			}
			return TIUtilities.FormatSmallNumber(Mathf.Min(9.9f, 1f / num), 7, 1, true, false);
		}

		// Token: 0x1700095B RID: 2395
		// (get) Token: 0x06003791 RID: 14225 RVA: 0x001428CA File Offset: 0x00140ACA
		// (set) Token: 0x06003792 RID: 14226 RVA: 0x001428D2 File Offset: 0x00140AD2
		public float education { get; private set; }

		// Token: 0x06003793 RID: 14227 RVA: 0x001428DC File Offset: 0x00140ADC
		public void AddToEducation(float value, TINationState.EducationChangeReason reason)
		{
			bool flag = value > 0f && this.missionControl >= this.maxMissionControl;
			float education = this.education;
			this.education += value;
			this.education = Mathf.Clamp(this.education, 1f, 255f);
			if (education != this.education)
			{
				this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.Research);
				});
				this.SetDataDirty();
			}
			Dictionary<TINationState.EducationChangeReason, float> dictionary = this.tracker_EducationChangeReason_CurrentTrackingPeriod;
			dictionary[reason] += value;
			dictionary = this.tracker_EducationChangeReason_AllTime;
			dictionary[reason] += value;
			TIGlobalValuesState.GlobalValues.TrySetMaximumEducation(this, this.education);
			if (flag)
			{
				this.PossiblePriorityValidationChange(false);
			}
		}

		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06003794 RID: 14228 RVA: 0x001429B8 File Offset: 0x00140BB8
		public string EducationDescriptiveString
		{
			get
			{
				if (this.education < 5f)
				{
					return Loc.T("UI.Nation.Education5");
				}
				if (this.education < 8f)
				{
					return Loc.T("UI.Nation.Education8");
				}
				return Loc.T("UI.Nation.Education9");
			}
		}

		// Token: 0x06003795 RID: 14229 RVA: 0x001429F4 File Offset: 0x00140BF4
		public string GetEducationDescriptiveStringAndValue(int decimalPlaces = 1)
		{
			return Loc.T("UI.Nation.NationalStatStringValue", new object[]
			{
				this.EducationDescriptiveString,
				TIUtilities.FormatSmallNumber(this.education, decimalPlaces, decimalPlaces, true, false)
			});
		}

		// Token: 0x1700095D RID: 2397
		// (get) Token: 0x06003796 RID: 14230 RVA: 0x00142A2C File Offset: 0x00140C2C
		// (set) Token: 0x06003797 RID: 14231 RVA: 0x00142A34 File Offset: 0x00140C34
		public float democracy { get; private set; }

		// Token: 0x06003798 RID: 14232 RVA: 0x00142A40 File Offset: 0x00140C40
		public void AddToDemocracy(float value, TINationState.DemocracyChangeReason reason)
		{
			float democracy = this.democracy;
			this.democracy += value;
			this.democracy = Mathf.Clamp(this.democracy, 0f, 10f);
			if (this.democracy != democracy)
			{
				this.SetDataDirty();
				this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.Research);
				});
			}
			Dictionary<TINationState.DemocracyChangeReason, float> dictionary = this.tracker_DemocracyChangeReason_CurrentTrackingPeriod;
			dictionary[reason] += value;
			dictionary = this.tracker_DemocracyChangeReason_AllTime;
			dictionary[reason] += value;
		}

		// Token: 0x1700095E RID: 2398
		// (get) Token: 0x06003799 RID: 14233 RVA: 0x00142AE8 File Offset: 0x00140CE8
		public string DemocracyDescriptiveString
		{
			get
			{
				if (this.democracy < 2f)
				{
					return Loc.T("UI.Nation.Democracy2");
				}
				if (this.democracy < 4f)
				{
					return Loc.T("UI.Nation.Democracy4");
				}
				if (this.democracy < 6f)
				{
					return Loc.T("UI.Nation.Democracy6");
				}
				if (this.democracy < 8f)
				{
					return Loc.T("UI.Nation.Democracy8");
				}
				return Loc.T("UI.Nation.Democracy10");
			}
		}

		// Token: 0x0600379A RID: 14234 RVA: 0x00142B60 File Offset: 0x00140D60
		public string GetDemocracyDescriptiveStringAndValue(int decimalPlaces = 1)
		{
			return Loc.T("UI.Nation.NationalStatStringValue", new object[]
			{
				this.DemocracyDescriptiveString,
				TIUtilities.FormatSmallNumber(this.democracy, decimalPlaces, decimalPlaces, true, false)
			});
		}

		// Token: 0x1700095F RID: 2399
		// (get) Token: 0x0600379B RID: 14235 RVA: 0x00142B98 File Offset: 0x00140D98
		// (set) Token: 0x0600379C RID: 14236 RVA: 0x00142BA0 File Offset: 0x00140DA0
		public float cohesion { get; private set; }

		// Token: 0x0600379D RID: 14237 RVA: 0x00142BAC File Offset: 0x00140DAC
		public float AddToCohesion(float value, TINationState.CohesionChangeReason reason)
		{
			float cohesion = this.cohesion;
			this.cohesion += value;
			float num = 0f;
			float num2 = 0f;
			if (this.cohesion < 0f)
			{
				num2 = -this.cohesion * 0.5f;
				if (this.democracy > 5f)
				{
					num = Mathf.Min(-this.cohesion, this.democracy - 5f) * 0.5f;
					num2 -= num;
				}
			}
			this.cohesion = Mathf.Clamp(this.cohesion, 0f, 10f);
			if (num > 0f)
			{
				this.AddToDemocracy(-num * this.priorityEffectPopScaling, TINationState.DemocracyChangeReason.DemReason_ZeroCohesion);
			}
			if (num2 > 0f)
			{
				this.AddToUnrest(num2 * this.priorityEffectPopScaling, TINationState.UnrestChangeReason.UnrestReason_ZeroCohesion, 10f);
			}
			if (this.cohesion != cohesion)
			{
				this.SetDataDirty();
				this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.Research);
				});
			}
			Dictionary<TINationState.CohesionChangeReason, float> dictionary = this.tracker_CohesionChangeReason_CurrentTrackingPeriod;
			dictionary[reason] += value;
			dictionary = this.tracker_CohesionChangeReason_AllTime;
			dictionary[reason] += value;
			return value;
		}

		// Token: 0x17000960 RID: 2400
		// (get) Token: 0x0600379E RID: 14238 RVA: 0x00142CE8 File Offset: 0x00140EE8
		public float inequalityImpactOnCohesion
		{
			get
			{
				return Mathf.Min(1f, 0.5f + this.education / 20f) * (-(this.inequality * TemplateManager.global.inequalityCohesionMultiplier) - ((this.inequality > TemplateManager.global.severeInequality) ? (this.inequality - TemplateManager.global.severeInequality) : 0f));
			}
		}

		// Token: 0x17000961 RID: 2401
		// (get) Token: 0x0600379F RID: 14239 RVA: 0x00142D4F File Offset: 0x00140F4F
		public float populationImpactOnCohesion
		{
			get
			{
				return -Mathf.Pow(this.population_Millions, TemplateManager.global.populationCohesionImpactPower + ((this.regions.Count == 1) ? 0.1f : 0f));
			}
		}

		// Token: 0x17000962 RID: 2402
		// (get) Token: 0x060037A0 RID: 14240 RVA: 0x00142D82 File Offset: 0x00140F82
		public float regionsImpactOnCohesion
		{
			get
			{
				return Math.Max(TemplateManager.global.maxDistanceImpactOnCohesion, (float)Math.Truncate((double)(100f * (-(double)this.distanceFromCapitalToPopCenter_km * TemplateManager.global.cohesionImpactPerKMtoPopCenter))) / 100f);
			}
		}

		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x060037A1 RID: 14241 RVA: 0x00142DB8 File Offset: 0x00140FB8
		public float perCapitaGDPImpactOnCohesion
		{
			get
			{
				float num = Mathf.Max(this.tracker_PCGDP_ByQuarter.Where<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Key >= TITimeState.CurrentQuarter() - 40).Max<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Value), 100f);
				float num2 = this.perCapitaGDP / num;
				if (num2 < 1f)
				{
					return (1f - num2) * -this.inequality;
				}
				return 0f;
			}
		}

		// Token: 0x17000964 RID: 2404
		// (get) Token: 0x060037A2 RID: 14242 RVA: 0x00142E44 File Offset: 0x00141044
		public float rivalsImpactOnCohesion
		{
			get
			{
				return Mathf.Min(Mathf.Max(0f, 3f - this.warsImpactOnCohesion), 0.5f * (float)((this.democracy >= 6f) ? this.rivals.Count<TINationState>((TINationState x) => x.democracy < 6f && x.numControlPoints >= this.numControlPoints - 1) : this.rivals.Count<TINationState>((TINationState x) => x.numControlPoints >= this.numControlPoints - 1)));
			}
		}

		// Token: 0x17000965 RID: 2405
		// (get) Token: 0x060037A3 RID: 14243 RVA: 0x00142EB0 File Offset: 0x001410B0
		public float warsImpactOnCohesion
		{
			get
			{
				float num = 3f;
				float num2;
				if (this.democracy < 6f)
				{
					num2 = (float)this.wars.Distinct<TINationState>().Count<TINationState>((TINationState x) => x.extant);
				}
				else
				{
					num2 = (float)this.wars.Distinct<TINationState>().Count<TINationState>((TINationState x) => x.extant && x.democracy < 6f);
				}
				return Mathf.Min(num, num2);
			}
		}

		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x060037A4 RID: 14244 RVA: 0x00142F35 File Offset: 0x00141135
		public float publicEliteDivideImpactOnCohesion
		{
			get
			{
				return Vector3.Distance(this.GetMeanEliteVector(), this.GetMeanPublicOpinionVector()) * -TemplateManager.global.publicEliteIdeologicalDistanceCohesionMultiplier;
			}
		}

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x060037A5 RID: 14245 RVA: 0x00142F54 File Offset: 0x00141154
		public float publicOpinionImpactOnCohesion
		{
			get
			{
				return -0.5f + (this.PublicOpinionToMaxIdeologicalAntipathyRatio() - 0.5f) * -TemplateManager.global.publicOpinionDispersionCohesionMultiplier;
			}
		}

		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x060037A6 RID: 14246 RVA: 0x00142F74 File Offset: 0x00141174
		public float distanceFromCapitalToPopCenter_km_old
		{
			get
			{
				float num = 0f;
				float num2 = 0f;
				foreach (TIRegionState tiregionState in this.regions)
				{
					if (this.capital != tiregionState && tiregionState.NationsWithClaim(false, true, false, true).Count > 0)
					{
						float cohesionImpactMultiplierIfSeparatistMovement = TemplateManager.global.cohesionImpactMultiplierIfSeparatistMovement;
					}
					num += tiregionState.longitude * tiregionState.populationInMillions;
					num2 += tiregionState.latitude * tiregionState.populationInMillions;
				}
				num /= this.population_Millions;
				num2 /= this.population_Millions;
				return TIRegionState.DistanceBetweenTwoCoordinates_km(this.capital.latitude, this.capital.longitude, num2, num, this.ref_spaceBody.meanRadius_km);
			}
		}

		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x060037A7 RID: 14247 RVA: 0x00143054 File Offset: 0x00141254
		public float distanceFromCapitalToPopCenter_km
		{
			get
			{
				List<Vector2d> list = new List<Vector2d>();
				foreach (TIRegionState tiregionState in this.regions)
				{
					float num = TIRegionState.DistanceBetweenTwoCoordinates_km(this.capital.latitude, this.capital.longitude, tiregionState.latitude, tiregionState.longitude, this.ref_spaceBody.meanRadius_km);
					list.Add(new Vector2d((double)num, (double)tiregionState.populationInMillions));
				}
				return (float)Mathd.WeightedMean(list.Select<Vector2d, double>((Vector2d v) => v.x).ToArray<double>(), list.Select<Vector2d, double>((Vector2d v) => v.y).ToArray<double>());
			}
		}

		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x060037A8 RID: 14248 RVA: 0x00143148 File Offset: 0x00141348
		public float autocracyImpactOnCohesion
		{
			get
			{
				if (this.democracy <= 3.5f)
				{
					return (Mathf.Pow(3.5f, 1.285f) - Mathf.Pow(this.democracy, 1.285f)) * ((10f - this.unrest) / 10f);
				}
				return 0f;
			}
		}

		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x060037A9 RID: 14249 RVA: 0x0014319B File Offset: 0x0014139B
		public float anocracyImpactOnCohesion
		{
			get
			{
				if (this.democracy > 3.5f && this.democracy <= 6.5f)
				{
					return 2f * Mathf.Abs(5f - this.democracy) - 3f;
				}
				return 0f;
			}
		}

		// Token: 0x060037AA RID: 14250 RVA: 0x001431DC File Offset: 0x001413DC
		public float DemocracyImpactOnCohesion(float originalValue)
		{
			if (this.democracy > 6.5f)
			{
				float num;
				if (originalValue > 5f)
				{
					num = Mathf.Max(5f, originalValue - Mathf.Abs((6.5f - this.democracy) / 2f));
				}
				else
				{
					num = Mathf.Min(5f, originalValue + Mathf.Abs((6.5f - this.democracy) / 2f));
				}
				return num - originalValue;
			}
			return 0f;
		}

		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x060037AB RID: 14251 RVA: 0x00143253 File Offset: 0x00141453
		public float hostileClaimsImpactOnCohesion
		{
			get
			{
				return this.TotalImpactFromHostileClaims() * (this.democracy / 10f) * -1f;
			}
		}

		// Token: 0x1700096D RID: 2413
		// (get) Token: 0x060037AC RID: 14252 RVA: 0x00143270 File Offset: 0x00141470
		public float cohesionRestState
		{
			get
			{
				if (this.extant)
				{
					float num = 16f + this.inequalityImpactOnCohesion + this.perCapitaGDPImpactOnCohesion + this.populationImpactOnCohesion + this.regionsImpactOnCohesion + this.hostileClaimsImpactOnCohesion + this.rivalsImpactOnCohesion + this.warsImpactOnCohesion + this.publicEliteDivideImpactOnCohesion + this.publicOpinionImpactOnCohesion + this.autocracyImpactOnCohesion + this.anocracyImpactOnCohesion;
					num += this.DemocracyImpactOnCohesion(num);
					return Mathf.Clamp(num, 0f, 10f);
				}
				return this.cohesion;
			}
		}

		// Token: 0x060037AD RID: 14253 RVA: 0x001432FB File Offset: 0x001414FB
		private static string ColorCohesionRestStateValue(string formattedValue, float value)
		{
			if (value < 0f)
			{
				return TIUtilities.RedLine(formattedValue);
			}
			return TIUtilities.GreenLine(formattedValue);
		}

		// Token: 0x1700096E RID: 2414
		// (get) Token: 0x060037AE RID: 14254 RVA: 0x00143314 File Offset: 0x00141514
		public string CohesionRestStateDetail
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Loc.T("UI.Nation.CohesionReststateBreakdown"));
				stringBuilder.AppendLine(Loc.T("UI.Nation.BaseValue", new object[] { 16f.ToString("N2") }));
				if (this.inequalityImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromInequality", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.inequalityImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.inequalityImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.inequalityImpactOnCohesion) }));
				}
				if (this.perCapitaGDPImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromLowPCGDP", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.perCapitaGDPImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.perCapitaGDPImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.perCapitaGDPImpactOnCohesion) }));
				}
				if (this.populationImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromPopulation", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.populationImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.populationImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.populationImpactOnCohesion) }));
				}
				if (this.regionsImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromRegions", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.regionsImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.regionsImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.regionsImpactOnCohesion) }));
				}
				if (this.hostileClaimsImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromHostileClaims_Cohesion", new object[]
					{
						TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.hostileClaimsImpactOnCohesion, false, false, ""), this.hostileClaimsImpactOnCohesion),
						TIUtilities.FormatBigOrSmallNumber(this.hostileClaimsImpactOnCohesion, 1, 7, 0, false, false)
					}));
				}
				if (this.rivalsImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromRivals", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.rivalsImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.rivalsImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.rivalsImpactOnCohesion) }));
				}
				if (this.warsImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromWars", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.warsImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.warsImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.warsImpactOnCohesion) }));
				}
				if (this.publicEliteDivideImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromIdeology", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.publicEliteDivideImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.publicEliteDivideImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.publicEliteDivideImpactOnCohesion) }));
				}
				if (this.publicOpinionImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromInternalDifferences", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.publicOpinionImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.publicOpinionImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.publicOpinionImpactOnCohesion) }));
				}
				if (this.autocracyImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromAutocracy", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.autocracyImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.autocracyImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.autocracyImpactOnCohesion) }));
				}
				if (this.anocracyImpactOnCohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromAnocracy", new object[] { TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(this.anocracyImpactOnCohesion, TIUtilities.FormatBigOrSmallNumber(this.anocracyImpactOnCohesion, 1, 7, 0, false, false), false, false, NationInfoController.WhatIsGood.upIsGood), this.anocracyImpactOnCohesion) }));
				}
				float num = 16f + this.inequalityImpactOnCohesion + this.perCapitaGDPImpactOnCohesion + this.populationImpactOnCohesion + this.regionsImpactOnCohesion + this.hostileClaimsImpactOnCohesion + this.rivalsImpactOnCohesion + this.warsImpactOnCohesion + this.publicEliteDivideImpactOnCohesion + this.publicOpinionImpactOnCohesion + this.autocracyImpactOnCohesion + this.anocracyImpactOnCohesion;
				float num2 = this.DemocracyImpactOnCohesion(num);
				if (num2 != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromDemocracy", new object[]
					{
						TINationState.ColorCohesionRestStateValue(TIUtilities.ForceValueSign(num2, false, false, ""), num2),
						TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false)
					}));
				}
				stringBuilder.AppendLine(Loc.T("UI.Nation.CohesionLimits", new object[] { (num + num2).ToString("N2") }));
				return stringBuilder.ToString();
			}
		}

		// Token: 0x060037AF RID: 14255 RVA: 0x001437AC File Offset: 0x001419AC
		public string CohesionRestStateInlineSpritePath()
		{
			float cohesionRestState = this.cohesionRestState;
			if (this.cohesion == cohesionRestState)
			{
				return "<color=#FFFFFFFF><sprite name=\"cohesion_steady\"></color>";
			}
			if (this.cohesion < cohesionRestState)
			{
				return "<color=#FFFFFFFF><sprite name=\"cohesion_increasing_good\"></color>";
			}
			if (cohesionRestState > 5f && this.cohesion > cohesionRestState)
			{
				return "<color=#FFFFFFFF><sprite name=\"cohesion_decreasing_good\"></color>";
			}
			return "<color=#FFFFFFFF><sprite name=\"cohesion_decreasing_bad\"></color>";
		}

		// Token: 0x060037B0 RID: 14256 RVA: 0x001437FC File Offset: 0x001419FC
		public float GetMonthlyCohesionMovement()
		{
			float cohesionRestState = this.cohesionRestState;
			float num = Mathf.Clamp(Mathf.Max(0f, this.inequality - 3f) * Mathf.Max(0f, this.inequality - 3f) / 10f, TemplateManager.global.maxMonthlyCohesionDecrease_normal, TemplateManager.global.maxMonthlyCohesionDecrease_cap);
			if (this.cohesion < cohesionRestState)
			{
				return Mathf.Min(TemplateManager.global.maxMonthlyCohesionIncrease_normal, cohesionRestState - this.cohesion);
			}
			if (this.cohesion > cohesionRestState)
			{
				return -Mathf.Min(num, this.cohesion - cohesionRestState);
			}
			return 0f;
		}

		// Token: 0x1700096F RID: 2415
		// (get) Token: 0x060037B1 RID: 14257 RVA: 0x0014389C File Offset: 0x00141A9C
		public string CohesionDescriptiveString
		{
			get
			{
				return Loc.T(new StringBuilder("UI.Nation.Cohesion").Append(Math.Truncate((double)this.cohesion)).ToString());
			}
		}

		// Token: 0x060037B2 RID: 14258 RVA: 0x001438C4 File Offset: 0x00141AC4
		public string GetCohesionDescriptiveStringAndValue(int decimalPlaces = 1)
		{
			return Loc.T("UI.Nation.NationalStatStringValue", new object[]
			{
				this.CohesionDescriptiveString,
				TIUtilities.FormatSmallNumber(this.cohesion, decimalPlaces, decimalPlaces, true, false)
			});
		}

		// Token: 0x17000970 RID: 2416
		// (get) Token: 0x060037B3 RID: 14259 RVA: 0x001438FC File Offset: 0x00141AFC
		public bool cohesionWarning
		{
			get
			{
				return this.cohesion <= 2.5f;
			}
		}

		// Token: 0x17000971 RID: 2417
		// (get) Token: 0x060037B4 RID: 14260 RVA: 0x0014390E File Offset: 0x00141B0E
		public bool futureMajorCohesionWarning
		{
			get
			{
				return this.cohesionRestState_dailyCache <= 1f;
			}
		}

		// Token: 0x17000972 RID: 2418
		// (get) Token: 0x060037B5 RID: 14261 RVA: 0x00143920 File Offset: 0x00141B20
		public bool majorCohesionWarning
		{
			get
			{
				return this.cohesion <= 1f;
			}
		}

		// Token: 0x17000973 RID: 2419
		// (get) Token: 0x060037B6 RID: 14262 RVA: 0x00143934 File Offset: 0x00141B34
		public float corruption
		{
			get
			{
				if (this.alienNation)
				{
					return 0f;
				}
				float num = (75f + -3.982725f * this.democracy + -0.86013f * this.cohesion + -0.000412312f * this.perCapitaGDP) / 150f;
				num += TIEffectsState.SumEffectsModifiers(Context.Corruption, this.executiveFaction, num, null);
				return Mathf.Clamp(num, 0.05f, 0.95f);
			}
		}

		// Token: 0x17000974 RID: 2420
		// (get) Token: 0x060037B7 RID: 14263 RVA: 0x001439A6 File Offset: 0x00141BA6
		public bool elitesHappy
		{
			get
			{
				return this.percentWeighttoPriority(PriorityType.Spoils) >= this.corruption;
			}
		}

		// Token: 0x17000975 RID: 2421
		// (get) Token: 0x060037B8 RID: 14264 RVA: 0x001439BA File Offset: 0x00141BBA
		// (set) Token: 0x060037B9 RID: 14265 RVA: 0x001439C2 File Offset: 0x00141BC2
		public float unrest { get; private set; }

		// Token: 0x17000976 RID: 2422
		// (get) Token: 0x060037BA RID: 14266 RVA: 0x001439CB File Offset: 0x00141BCB
		public bool civilWar
		{
			get
			{
				return (double)this.unrest >= 9.0;
			}
		}

		// Token: 0x17000977 RID: 2423
		// (get) Token: 0x060037BB RID: 14267 RVA: 0x001439E2 File Offset: 0x00141BE2
		public bool futureUnrestMajorWarning
		{
			get
			{
				return this.unrestRestState_dailyCache >= 5f;
			}
		}

		// Token: 0x17000978 RID: 2424
		// (get) Token: 0x060037BC RID: 14268 RVA: 0x001439F4 File Offset: 0x00141BF4
		public bool unrestWarning
		{
			get
			{
				return this.unrest - this.historyUnrest[31] > 0f;
			}
		}

		// Token: 0x17000979 RID: 2425
		// (get) Token: 0x060037BD RID: 14269 RVA: 0x00143A11 File Offset: 0x00141C11
		public bool unrestMajorWarning
		{
			get
			{
				return this.unrest >= 5f || this.unrest - this.historyUnrest[31] > 0.5f;
			}
		}

		// Token: 0x060037BE RID: 14270 RVA: 0x00143A40 File Offset: 0x00141C40
		public void AddToUnrest(float value, TINationState.UnrestChangeReason reason, float cap = 10f)
		{
			float unrest = this.unrest;
			this.unrest += value;
			cap = Mathf.Max(unrest, cap);
			this.unrest = Mathf.Clamp(this.unrest, 0f, Mathf.Min(cap, 10f));
			if (this.unrest != unrest)
			{
				this.SetDataDirty();
				this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.Research);
				});
			}
			if (this.unrest == 0f && !this.ValidPriority(PriorityType.Oppression))
			{
				this.PossiblePriorityValidationChange(true);
			}
			Dictionary<TINationState.UnrestChangeReason, float> dictionary = this.tracker_UnrestChangeReason_CurrentTrackingPeriod;
			dictionary[reason] += value;
			dictionary = this.tracker_UnrestChangeReason_AllTime;
			dictionary[reason] += value;
		}

		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x060037BF RID: 14271 RVA: 0x00143B14 File Offset: 0x00141D14
		public float perCapitaGDPEffectOnUnrest
		{
			get
			{
				return -this.perCapitaGDP / TIGlobalValuesState.PCGDPToReduceUnrestBy1;
			}
		}

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x060037C0 RID: 14272 RVA: 0x00143B24 File Offset: 0x00141D24
		public float armyImpactOnUnrest
		{
			get
			{
				float num = 0f;
				List<TIArmyState> list = new List<TIArmyState>(this.standardArmies);
				list.AddRange(this.allies.SelectMany<TINationState, TIArmyState>((TINationState x) => x.standardArmies));
				float num2 = Mathf.Max(Mathf.Pow((float)this.regions.Count, 1f - TemplateManager.global.controlPointIPScaling), 1f);
				foreach (TIArmyState tiarmyState in list)
				{
					if (this.regions.Contains(tiarmyState.currentRegion) && (tiarmyState.homeNation.BaseInvestmentPoints_month() > 0f || tiarmyState.AlienRegularArmy))
					{
						num -= tiarmyState.strength * 0.5f * (10f - this.democracy) / num2;
						num += TIEffectsState.SumEffectsModifiers(Context.ArmyUnrestReductionImpact, tiarmyState.faction, num, null);
					}
				}
				return num;
			}
		}

		// Token: 0x060037C1 RID: 14273 RVA: 0x00143C38 File Offset: 0x00141E38
		public float IndividualArmyImpactOnUnrest(TIFactionState faction)
		{
			float num = Mathf.Max(Mathf.Pow((float)this.regions.Count, 1f - TemplateManager.global.controlPointIPScaling), 1f);
			if (this.BaseInvestmentPoints_month() > 0f)
			{
				float num2 = 0.5f * (10f - this.democracy) / num;
				return num2 + TIEffectsState.SumEffectsModifiers(Context.ArmyUnrestReductionImpact, faction, num2, null);
			}
			return 0f;
		}

		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x060037C2 RID: 14274 RVA: 0x00143CA8 File Offset: 0x00141EA8
		public float xenoformingImpactOnUnrest
		{
			get
			{
				if (this.alienNation && this.regions.Count > 0)
				{
					return -this.regions.Average<TIRegionState>((TIRegionState x) => x.xenoforming.xenoformingLevel / 20f);
				}
				return 0f;
			}
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x060037C3 RID: 14275 RVA: 0x00143CFC File Offset: 0x00141EFC
		public float unrestRestState
		{
			get
			{
				if (this.extant)
				{
					return Mathf.Clamp(10.5f - this.cohesion - this.perCapitaGDP / TIGlobalValuesState.PCGDPToReduceUnrestBy1 + this.armyImpactOnUnrest + this.xenoformingImpactOnUnrest + this.hostileClaimsImpactOnUnrest, 0f, 10f);
				}
				return 0f;
			}
		}

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x060037C4 RID: 14276 RVA: 0x00143D54 File Offset: 0x00141F54
		public float unrestRestState_unclamped
		{
			get
			{
				if (this.extant)
				{
					return 10.5f - this.cohesion - this.perCapitaGDP / TIGlobalValuesState.PCGDPToReduceUnrestBy1 + this.armyImpactOnUnrest + this.xenoformingImpactOnUnrest + this.hostileClaimsImpactOnUnrest;
				}
				return 0f;
			}
		}

		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x060037C5 RID: 14277 RVA: 0x00143D92 File Offset: 0x00141F92
		public float hostileClaimsImpactOnUnrest
		{
			get
			{
				return this.TotalImpactFromHostileClaims() * (1f - this.democracy / 10f);
			}
		}

		// Token: 0x060037C6 RID: 14278 RVA: 0x00143DAD File Offset: 0x00141FAD
		private static string ColorUnrestRestStateValue(string formattedValue, float value)
		{
			if (value > 0f)
			{
				return TIUtilities.RedLine(formattedValue);
			}
			return TIUtilities.GreenLine(formattedValue);
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x060037C7 RID: 14279 RVA: 0x00143DC4 File Offset: 0x00141FC4
		public string unrestRestStateDetail
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Loc.T("UI.Nation.UnrestRestStateBreakdown"));
				stringBuilder.AppendLine(Loc.T("UI.Nation.BaseValue", new object[] { 10.5f.ToString("N1") }));
				if (this.cohesion != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromCohesion", new object[]
					{
						TINationState.ColorUnrestRestStateValue(TIUtilities.ForceValueSign(-this.cohesion, false, false, ""), -this.cohesion),
						(-this.cohesion).ToString("N1")
					}));
				}
				if (this.perCapitaGDPEffectOnUnrest != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromPCGDP", new object[]
					{
						TINationState.ColorUnrestRestStateValue(TIUtilities.ForceValueSign(this.perCapitaGDPEffectOnUnrest, false, false, ""), this.perCapitaGDPEffectOnUnrest),
						this.perCapitaGDPEffectOnUnrest.ToString("N1")
					}));
				}
				if (this.armyImpactOnUnrest != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromArmies", new object[]
					{
						TINationState.ColorUnrestRestStateValue(TIUtilities.ForceValueSign(this.armyImpactOnUnrest, false, false, ""), this.armyImpactOnUnrest),
						TIUtilities.FormatBigOrSmallNumber(this.armyImpactOnUnrest, 1, 7, 0, false, false)
					}));
				}
				if (this.xenoformingImpactOnUnrest != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromXenoforming", new object[]
					{
						TINationState.ColorUnrestRestStateValue(TIUtilities.ForceValueSign(this.xenoformingImpactOnUnrest, false, false, ""), this.xenoformingImpactOnUnrest),
						TIUtilities.FormatBigOrSmallNumber(this.xenoformingImpactOnUnrest, 1, 7, 0, false, false)
					}));
				}
				if (this.hostileClaimsImpactOnUnrest != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.FromHostileClaims_Unrest", new object[]
					{
						TINationState.ColorUnrestRestStateValue(TIUtilities.ForceValueSign(this.hostileClaimsImpactOnUnrest, false, false, ""), this.hostileClaimsImpactOnUnrest),
						TIUtilities.FormatBigOrSmallNumber(this.hostileClaimsImpactOnUnrest, 1, 7, 0, false, false)
					}));
				}
				stringBuilder.AppendLine(Loc.T("UI.Nation.UnrestLimits", new object[] { this.unrestRestState_unclamped.ToString("N2") }));
				return stringBuilder.ToString();
			}
		}

		// Token: 0x060037C8 RID: 14280 RVA: 0x00144004 File Offset: 0x00142204
		public float GetMonthlyUnrestMovement()
		{
			float num = 0f;
			float unrestRestState = this.unrestRestState;
			if (this.cohesion == 0f && unrestRestState > this.unrest)
			{
				num = Mathf.Min(TemplateManager.global.maxMonthlyUnrestMovement_rapidIncrease, unrestRestState - this.unrest);
			}
			else if (this.unrest < unrestRestState)
			{
				num = Mathf.Min(TemplateManager.global.maxMonthlyUnrestMovement_normal, unrestRestState - this.unrest);
			}
			else if (this.unrest > unrestRestState)
			{
				num = -Mathf.Min(TemplateManager.global.maxMonthlyUnrestMovement_normal, this.unrest - unrestRestState);
			}
			return num;
		}

		// Token: 0x060037C9 RID: 14281 RVA: 0x00144094 File Offset: 0x00142294
		public string UnrestRestStateInlineSpritePath()
		{
			float unrestRestState = this.unrestRestState;
			if (this.unrest == unrestRestState)
			{
				return "<color=#FFFFFFFF><sprite name=\"unrest_steady\"></color>";
			}
			if (this.unrest > unrestRestState)
			{
				return "<color=#FFFFFFFF><sprite name=\"unrest_decreasing_good\"></color>";
			}
			return "<color=#FFFFFFFF><sprite name=\"unrest_increasing_bad\"></color>";
		}

		// Token: 0x060037CA RID: 14282 RVA: 0x001440CC File Offset: 0x001422CC
		public string GetUnrestDescriptiveStringAndValue(int decimalPlaces = 1)
		{
			float unrest = this.unrest;
			string text;
			if (unrest < 1.5f)
			{
				text = Loc.T("UI.Nation.Unrest15");
			}
			else if (unrest < 3f)
			{
				text = Loc.T("UI.Nation.Unrest3");
			}
			else if (unrest < 4.5f)
			{
				text = Loc.T("UI.Nation.Unrest45");
			}
			else if (unrest < 6f)
			{
				text = Loc.T("UI.Nation.Unrest6");
			}
			else if (unrest < 7.5f)
			{
				text = Loc.T("UI.Nation.Unrest75");
			}
			else if (unrest < 9f)
			{
				text = Loc.T("UI.Nation.Unrest9");
			}
			else
			{
				text = Loc.T("UI.Nation.Unrest10");
			}
			string text2 = text;
			return Loc.T("UI.Nation.NationalStatStringValue", new object[]
			{
				text2,
				TIUtilities.FormatSmallNumber(this.unrest, decimalPlaces, decimalPlaces, true, false)
			});
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x060037CB RID: 14283 RVA: 0x00144194 File Offset: 0x00142394
		public float research_month
		{
			get
			{
				float population_Millions = this.population_Millions;
				float perCapitaGDP = this.perCapitaGDP;
				return (population_Millions * ((perCapitaGDP <= 30000f) ? Mathf.Pow(this.perCapitaGDP / 15000f, 0.6f) : (1.5157166f + 0.90942997f * (Mathf.Log(this.perCapitaGDP / 15000f) - 0.6931472f))) * (this.education * Mathf.Min(this.education, 12f)) * Mathf.Pow(Mathf.Max(this.democracy, 1f), 0.16666667f) * 0.0075f + Mathf.Min(this.population / 5000f, (float)this.numControlPoints + this.education + this.democracy / 2f)) * (1.25f - Mathf.Abs(this.cohesion - 5f) / 10f) * (1f - this.unrest * this.unrest * 0.01f) * (1f + this.adviserScienceBonus);
			}
		}

		// Token: 0x17000982 RID: 2434
		// (get) Token: 0x060037CC RID: 14284 RVA: 0x0014429D File Offset: 0x0014249D
		// (set) Token: 0x060037CD RID: 14285 RVA: 0x001442A5 File Offset: 0x001424A5
		public float militaryTechLevel { get; private set; }

		// Token: 0x060037CE RID: 14286 RVA: 0x001442B0 File Offset: 0x001424B0
		public void AddToMilitaryTechLevel(float value)
		{
			this.militaryTechLevel += value;
			this.militaryTechLevel = Mathf.Clamp(this.militaryTechLevel, TemplateManager.global.minMilitaryTechLevel, this.maxMilitaryTechLevel);
			TIGlobalValuesState.GlobalValues.TrySetMaximumMiltech(this, this.militaryTechLevel);
			foreach (TIArmyState tiarmyState in this.armies)
			{
				tiarmyState.SetArmyDataDirty();
			}
			if (!this.ValidPriority(PriorityType.Military))
			{
				this.PossiblePriorityValidationChange(true);
			}
		}

		// Token: 0x060037CF RID: 14287 RVA: 0x00144354 File Offset: 0x00142554
		public void AddToMaxMilitaryTechLevel(float value)
		{
			this.maxMilitaryTechLevel += value;
			this.maxMilitaryTechLevel = Mathf.Max(this.maxMilitaryTechLevel, 5f);
		}

		// Token: 0x17000983 RID: 2435
		// (get) Token: 0x060037D0 RID: 14288 RVA: 0x0014437C File Offset: 0x0014257C
		public string MilitaryTechDescriptiveString
		{
			get
			{
				if (!this.military)
				{
					return Loc.T("UI.Nation.NoMilitary");
				}
				float militaryTechLevel = this.militaryTechLevel;
				if (militaryTechLevel < 1f)
				{
					return Loc.T("UI.Nation.Miltech0");
				}
				if (militaryTechLevel < 2f)
				{
					return Loc.T("UI.Nation.Miltech1");
				}
				if (militaryTechLevel < 3f)
				{
					return Loc.T("UI.Nation.Miltech2");
				}
				if (militaryTechLevel < 4f)
				{
					return Loc.T("UI.Nation.Miltech3");
				}
				if (militaryTechLevel < 5f)
				{
					return Loc.T("UI.Nation.Miltech4");
				}
				if (militaryTechLevel < 6f)
				{
					return Loc.T("UI.Nation.Miltech5");
				}
				if (militaryTechLevel < 7f)
				{
					return Loc.T("UI.Nation.Miltech6");
				}
				return Loc.T("UI.Nation.Miltech7");
			}
		}

		// Token: 0x060037D1 RID: 14289 RVA: 0x00144434 File Offset: 0x00142634
		public string GetMilitaryDescriptiveStringAndValue(int decimalPlaces = 1)
		{
			if (!this.military)
			{
				return this.MilitaryTechDescriptiveString;
			}
			return Loc.T("UI.Nation.NationalStatStringValue", new object[]
			{
				this.MilitaryTechDescriptiveString,
				TIUtilities.FormatSmallNumber(this.militaryTechLevel, decimalPlaces, decimalPlaces, true, false)
			});
		}

		// Token: 0x060037D2 RID: 14290 RVA: 0x0014447B File Offset: 0x0014267B
		public void ChangeNumNuclearWeapons(int value)
		{
			this.numNuclearWeapons = Mathf.Max(this.numNuclearWeapons + value, 0);
			GameControl.eventManager.TriggerEvent(new NationDataUpdated(this), null, new object[] { this });
		}

		// Token: 0x17000984 RID: 2436
		// (get) Token: 0x060037D3 RID: 14291 RVA: 0x001444AC File Offset: 0x001426AC
		public List<TIArmyState> standardArmies
		{
			get
			{
				if (!this.alienNation)
				{
					return this.armies;
				}
				return this.armies.Where<TIArmyState>((TIArmyState x) => x.HumanArmy || x.AlienRegularArmy).ToList<TIArmyState>();
			}
		}

		// Token: 0x17000985 RID: 2437
		// (get) Token: 0x060037D4 RID: 14292 RVA: 0x001444EC File Offset: 0x001426EC
		public int numStandardArmies
		{
			get
			{
				if (!this.alienNation)
				{
					return this.armies.Count;
				}
				return this.armies.Count<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy);
			}
		}

		// Token: 0x17000986 RID: 2438
		// (get) Token: 0x060037D5 RID: 14293 RVA: 0x0014452C File Offset: 0x0014272C
		public int numNavies
		{
			get
			{
				return this.armies.Count<TIArmyState>((TIArmyState x) => x.deploymentType == DeploymentType.Naval);
			}
		}

		// Token: 0x17000987 RID: 2439
		// (get) Token: 0x060037D6 RID: 14294 RVA: 0x00144558 File Offset: 0x00142758
		public int numSTOFighters
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState x) => x.numSTOFighters);
			}
		}

		// Token: 0x17000988 RID: 2440
		// (get) Token: 0x060037D7 RID: 14295 RVA: 0x00144584 File Offset: 0x00142784
		public int availableSTOFighters
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState x) => x.availableSTOFighters);
			}
		}

		// Token: 0x17000989 RID: 2441
		// (get) Token: 0x060037D8 RID: 14296 RVA: 0x001445B0 File Offset: 0x001427B0
		public float nationNavalScore
		{
			get
			{
				return this.armies.Where<TIArmyState>((TIArmyState x) => x.deploymentType == DeploymentType.Naval).Sum<TIArmyState>((TIArmyState x) => x.techLevel);
			}
		}

		// Token: 0x1700098A RID: 2442
		// (get) Token: 0x060037D9 RID: 14297 RVA: 0x0014460C File Offset: 0x0014280C
		public bool navalFreedom
		{
			get
			{
				if (this.navalFreedomCachedFrame != TIFrameCounter.FrameCount)
				{
					this.cachedNavalFreedom = this.wars.Count == 0 || this.currentWarStates.All<TIWarState>((TIWarState x) => x.WarNationsWithNavalFreedom().Contains(this));
					this.navalFreedomCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedNavalFreedom;
			}
		}

		// Token: 0x1700098B RID: 2443
		// (get) Token: 0x060037DA RID: 14298 RVA: 0x00144664 File Offset: 0x00142864
		public string navalFreedomString
		{
			get
			{
				if (this.nationNavalScore <= 0f)
				{
					return Loc.T("UI.Nation.NoNavy");
				}
				if (this.navalFreedom)
				{
					return TIUtilities.GreenLine(Loc.T("UI.Nation.NavalFreedom"));
				}
				return TIUtilities.RedLine(Loc.T("UI.Nation.Blockaded"));
			}
		}

		// Token: 0x060037DB RID: 14299 RVA: 0x001446B0 File Offset: 0x001428B0
		public string NavalFreedomStringValue(bool includeCounts)
		{
			if (!includeCounts)
			{
				return Loc.T("UI.Nation.NavalStringWithValue", new object[]
				{
					this.navalFreedomString,
					TIUtilities.FormatSmallNumber(this.nationNavalScore, 1, 0, true, false)
				});
			}
			return Loc.T("UI.Nation.NavalStringExpanded", new object[]
			{
				this.numNavies,
				this.maxNaviesCanBuild,
				this.maxNavies,
				this.navalFreedomString,
				TIUtilities.FormatSmallNumber(this.nationNavalScore, 1, 0, true, false)
			});
		}

		// Token: 0x1700098C RID: 2444
		// (get) Token: 0x060037DC RID: 14300 RVA: 0x00144744 File Offset: 0x00142944
		public int allowedArmies
		{
			get
			{
				if (this.military && this.population_Millions >= TemplateManager.global.minPopulationForFirstArmy_millions)
				{
					return Mathf.Min(this.regions.Count<TIRegionState>((TIRegionState x) => !x.colonyRegion && !x.IsFullyOccupied()), 1 + (int)(this.population_Millions / TemplateManager.global.minPopulationForAdditionalArmiesPer_millions));
				}
				return 0;
			}
		}

		// Token: 0x1700098D RID: 2445
		// (get) Token: 0x060037DD RID: 14301 RVA: 0x001447B0 File Offset: 0x001429B0
		public bool canBuildArmy
		{
			get
			{
				return this.military && this.allowedArmies > this.numStandardArmies;
			}
		}

		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x060037DE RID: 14302 RVA: 0x001447CC File Offset: 0x001429CC
		public int maxNavies
		{
			get
			{
				if (!this.military || this.coastalRegions == 0)
				{
					return 0;
				}
				if (this.numControlPoints == TemplateManager.global.minControlPointsForNavyException && this.perCapitaGDP >= TemplateManager.global.PCGDPForNavyException)
				{
					return 1;
				}
				if (this.numControlPoints >= TemplateManager.global.minControlPointsForNavy)
				{
					return this.allowedArmies;
				}
				return 0;
			}
		}

		// Token: 0x1700098F RID: 2447
		// (get) Token: 0x060037DF RID: 14303 RVA: 0x0014482C File Offset: 0x00142A2C
		public int maxNaviesCanBuild
		{
			get
			{
				if (!this.military || this.coastalRegions == 0)
				{
					return 0;
				}
				if (this.numControlPoints == TemplateManager.global.minControlPointsForNavyException && this.perCapitaGDP >= TemplateManager.global.PCGDPForNavyException)
				{
					return Mathf.Clamp(this.numStandardArmies - this.numNavies, 0, 1);
				}
				if (this.numControlPoints >= TemplateManager.global.minControlPointsForNavy)
				{
					return this.numStandardArmies - this.numNavies;
				}
				return 0;
			}
		}

		// Token: 0x17000990 RID: 2448
		// (get) Token: 0x060037E0 RID: 14304 RVA: 0x001448A8 File Offset: 0x00142AA8
		public bool canBuildNavy
		{
			get
			{
				return this.military && this.numStandardArmies > this.numNavies && this.coastalRegions > 0 && (this.numControlPoints >= TemplateManager.global.minControlPointsForNavy || (this.numControlPoints == TemplateManager.global.minControlPointsForNavyException && this.perCapitaGDP >= TemplateManager.global.PCGDPForNavyException && this.numNavies == 0));
			}
		}

		// Token: 0x17000991 RID: 2449
		// (get) Token: 0x060037E1 RID: 14305 RVA: 0x00144919 File Offset: 0x00142B19
		public float area_km2
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState x) => x.area_km2);
			}
		}

		// Token: 0x17000992 RID: 2450
		// (get) Token: 0x060037E2 RID: 14306 RVA: 0x00144945 File Offset: 0x00142B45
		public int coastalRegions
		{
			get
			{
				return this.regions.Count<TIRegionState>((TIRegionState region) => region.isCoastal);
			}
		}

		// Token: 0x17000993 RID: 2451
		// (get) Token: 0x060037E3 RID: 14307 RVA: 0x00144971 File Offset: 0x00142B71
		public int resourceRegions
		{
			get
			{
				return this.regions.Count<TIRegionState>((TIRegionState region) => region.coreResourceRegion);
			}
		}

		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x060037E4 RID: 14308 RVA: 0x0014499D File Offset: 0x00142B9D
		public int miningRegions
		{
			get
			{
				return this.regions.Count<TIRegionState>((TIRegionState region) => region.resourceRegion);
			}
		}

		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x060037E5 RID: 14309 RVA: 0x001449C9 File Offset: 0x00142BC9
		public int oilRegions
		{
			get
			{
				return this.regions.Count<TIRegionState>((TIRegionState region) => region.oilRegion);
			}
		}

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x060037E6 RID: 14310 RVA: 0x001449F5 File Offset: 0x00142BF5
		public int colonyRegions
		{
			get
			{
				return this.regions.Count<TIRegionState>((TIRegionState region) => region.colonyRegion);
			}
		}

		// Token: 0x17000997 RID: 2455
		// (get) Token: 0x060037E7 RID: 14311 RVA: 0x00144A21 File Offset: 0x00142C21
		public int nonColonyRegions
		{
			get
			{
				return this.regions.Count<TIRegionState>((TIRegionState region) => !region.colonyRegion);
			}
		}

		// Token: 0x17000998 RID: 2456
		// (get) Token: 0x060037E8 RID: 14312 RVA: 0x00144A4D File Offset: 0x00142C4D
		public int currentResourceRegions
		{
			get
			{
				return this.numMiningRegions_dailyCache + this.numOilRegions_dailyCache;
			}
		}

		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x060037E9 RID: 14313 RVA: 0x00144A5C File Offset: 0x00142C5C
		public bool landlocked
		{
			get
			{
				return this.coastalRegions == 0;
			}
		}

		// Token: 0x060037EA RID: 14314 RVA: 0x00144A68 File Offset: 0x00142C68
		public void CacheRegionValues()
		{
			this.numMiningRegions_dailyCache = this.regions.Count<TIRegionState>((TIRegionState x) => x.resourceRegion && !x.IsFullyOccupied());
			this.numOilRegions_dailyCache = this.regions.Count<TIRegionState>((TIRegionState x) => x.oilRegion && !x.IsFullyOccupied());
			this.numCoreEconomicRegions_dailyCache = this.regions.Count<TIRegionState>((TIRegionState x) => x.coreEconomicRegion && !x.IsFullyOccupied());
			TIFederationState tifederationState = this.federation;
			this.restofFederationECOBonus_dailyCache = ((tifederationState != null) ? tifederationState.ECOBonus(this) : 0f);
			this.canAccumulateCoreEconomyTriggers = this.CandidateCoreEconomicRegions().Count > 0;
			this.canAccumulateCoreMiningTriggers = this.CandidateCoreMiningRegions().Count > 0 && !this.policy_noMineralDevelopment;
			this.canAccumulateCoreOilTriggers = this.CandidateCoreOilRegions().Count<TIRegionState>() > 0 && !this.policy_noOilDevelopment;
			this.canAccumulateDecolonizeTriggers = this.CandidateDecolonizeRegions().Count > 0;
			this.canAccumulateDecontaminateTriggers = this.canDecontaminate && this.CandidateDecontaminateRegions().Count > 0;
			this.canAccumulateLegitimizeClaimTriggers = this.regions.Any<TIRegionState>((TIRegionState x) => x.hostileRegion);
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x060037EB RID: 14315 RVA: 0x00144BDC File Offset: 0x00142DDC
		public float spaceDefenseCoverage
		{
			get
			{
				float num = 0f;
				foreach (TIRegionState tiregionState in this.regions)
				{
					num += (tiregionState.antiSpaceDefenses ? tiregionState.populationInMillions : 0f);
				}
				return num / Mathf.Max(this.population_Millions, 1f);
			}
		}

		// Token: 0x060037EC RID: 14316 RVA: 0x00144C58 File Offset: 0x00142E58
		public TIRegionState RandomRegionWeightedByPopulation()
		{
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			foreach (TIRegionState tiregionState in this.regions)
			{
				dictionary.Add(tiregionState, tiregionState.populationInMillions);
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> k) => k.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x060037ED RID: 14317 RVA: 0x00144CF0 File Offset: 0x00142EF0
		public void AddRegion(TIRegionState region)
		{
			this.regions.Add(region);
			this.SetArmyAccessibilityDirty();
			if (GameControl.loadcycle100)
			{
				this.PossiblePriorityValidationChange(false);
			}
		}

		// Token: 0x060037EE RID: 14318 RVA: 0x00144D12 File Offset: 0x00142F12
		public void RemoveRegion(TIRegionState region)
		{
			this.regions.Remove(region);
			this.SetArmyAccessibilityDirty();
			if (GameControl.loadcycle100)
			{
				this.PossiblePriorityValidationChange(false);
			}
		}

		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x060037EF RID: 14319 RVA: 0x00144D35 File Offset: 0x00142F35
		public float population_Millions
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState region) => region.populationInMillions);
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x060037F0 RID: 14320 RVA: 0x00144D61 File Offset: 0x00142F61
		public float population
		{
			get
			{
				return this.population_Millions * 1000000f;
			}
		}

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x060037F1 RID: 14321 RVA: 0x00144D6F File Offset: 0x00142F6F
		public float annualNationalPopulationChange
		{
			get
			{
				return (float)this.regions.Sum<TIRegionState>((TIRegionState x) => x.annualPopulationGrowth * (double)x.populationInMillions) / this.population_Millions;
			}
		}

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x060037F2 RID: 14322 RVA: 0x00144DA4 File Offset: 0x00142FA4
		public float populationDesnity_pop_km2
		{
			get
			{
				if (this.regions.Count <= 0)
				{
					return 0f;
				}
				return this.population / this.regions.Sum<TIRegionState>((TIRegionState x) => x.area_km2);
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x060037F3 RID: 14323 RVA: 0x00144DF6 File Offset: 0x00142FF6
		public string solarBody
		{
			get
			{
				return this.template.solarBody ?? "Earth";
			}
		}

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x060037F4 RID: 14324 RVA: 0x00144E0C File Offset: 0x0014300C
		public float spaceFunding_month
		{
			get
			{
				return this.spaceFunding_year / 12f;
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x060037F5 RID: 14325 RVA: 0x00144E1C File Offset: 0x0014301C
		public List<TIRegionSpaceFacilityState> spaceProgramSites
		{
			get
			{
				return (from x in this.regions
					where x.boostPerYear_dekatons > 0f
					select x.boostFacility.ref_regionSpaceFacility).Union<TIRegionSpaceFacilityState>(from x in this.regions
					where x.missionControl > 0
					select x.missionControlFacility.ref_regionSpaceFacility).ToList<TIRegionSpaceFacilityState>();
			}
		}

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x060037F6 RID: 14326 RVA: 0x00144ECF File Offset: 0x001430CF
		public float rawBoostPerYear_dekatons
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState region) => region.boostPerYear_dekatons);
			}
		}

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x060037F7 RID: 14327 RVA: 0x00144EFB File Offset: 0x001430FB
		public float rawBoostPerMonth_dekatons
		{
			get
			{
				return this.rawBoostPerYear_dekatons / 12f;
			}
		}

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x060037F8 RID: 14328 RVA: 0x00144F09 File Offset: 0x00143109
		public int missionControl
		{
			get
			{
				return this.regions.Sum<TIRegionState>((TIRegionState region) => region.missionControl);
			}
		}

		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x060037F9 RID: 14329 RVA: 0x00144F38 File Offset: 0x00143138
		public float currentBoost_year
		{
			get
			{
				return this.regions.Where<TIRegionState>((TIRegionState region) => !region.IsFullyOccupied()).Sum<TIRegionState>((TIRegionState region) => region.boostPerYear_dekatons);
			}
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x060037FA RID: 14330 RVA: 0x00144F93 File Offset: 0x00143193
		public float currentBoost_month
		{
			get
			{
				return this.currentBoost_year / 12f;
			}
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x060037FB RID: 14331 RVA: 0x00144FA1 File Offset: 0x001431A1
		public float boostIncome_year_dekatons
		{
			get
			{
				if (!this.inFederation)
				{
					return this.currentBoost_year;
				}
				return this.federation.MemberPooledResource_Year(this, FactionResource.Boost);
			}
		}

		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x060037FC RID: 14332 RVA: 0x00144FBF File Offset: 0x001431BF
		public float boostIncome_month_dekatons
		{
			get
			{
				return this.boostIncome_year_dekatons / 12f;
			}
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x060037FD RID: 14333 RVA: 0x00144FCD File Offset: 0x001431CD
		public float spaceFundingIncome_year
		{
			get
			{
				if (!this.inFederation)
				{
					return this.spaceFunding_year;
				}
				return this.federation.MemberPooledResource_Year(this, FactionResource.Money);
			}
		}

		// Token: 0x170009AA RID: 2474
		// (get) Token: 0x060037FE RID: 14334 RVA: 0x00144FEB File Offset: 0x001431EB
		public float spaceFundingIncome_month
		{
			get
			{
				return this.spaceFundingIncome_year / 12f;
			}
		}

		// Token: 0x170009AB RID: 2475
		// (get) Token: 0x060037FF RID: 14335 RVA: 0x00144FFC File Offset: 0x001431FC
		public int currentMissionControl
		{
			get
			{
				return this.regions.Where<TIRegionState>((TIRegionState region) => !region.IsFullyOccupied()).Sum<TIRegionState>((TIRegionState region) => region.missionControl);
			}
		}

		// Token: 0x170009AC RID: 2476
		// (get) Token: 0x06003800 RID: 14336 RVA: 0x00145058 File Offset: 0x00143258
		public int maxMissionControl
		{
			get
			{
				return this.regions.Where<TIRegionState>((TIRegionState region) => !region.IsFullyOccupied()).Sum<TIRegionState>((TIRegionState x) => x.maxMissionControl);
			}
		}

		// Token: 0x06003801 RID: 14337 RVA: 0x001450B3 File Offset: 0x001432B3
		private void ChangeControlPointOwner(TIControlPoint controlPoint, ControlPointChangeCause cause, TIFactionState faction)
		{
			this.ChangeControlPointOwner(controlPoint.positionInNation, cause, faction);
		}

		// Token: 0x06003802 RID: 14338 RVA: 0x001450C4 File Offset: 0x001432C4
		public void ChangeControlPointOwner(int index, ControlPointChangeCause cause, TIFactionState faction = null)
		{
			TIControlPoint controlPoint = this.GetControlPoint(index);
			if (controlPoint != null)
			{
				if (this.alienNation)
				{
					faction = GameStateManager.AlienFaction();
				}
				TIFactionState faction2 = controlPoint.faction;
				if (faction != faction2)
				{
					controlPoint.EndControlPointDefense();
					controlPoint.SetFaction(faction, false);
					controlPoint.EnableBenefits();
					GameControl.eventManager.TriggerEvent(new NationFlashEvent(this), null, new object[] { this });
					if (faction != null)
					{
						this.ApplyInvestmentTemplateToControlPoint(controlPoint.positionInNation, faction.defaultPriorityPreset);
						faction.CompleteMilestone(CampaignMilestone.TutorialGainControlPoint);
						if (this.controlPoints.Count >= 4)
						{
							faction.CompleteMilestone(CampaignMilestone.TutorialGainLargeNationControlPoint);
						}
						if (controlPoint.executive)
						{
							this.improveRelationsDeclinedUnderCurrentExecutivePair.Clear();
							TINationState[] array = GameStateManager.AllNations();
							for (int i = 0; i < array.Length; i++)
							{
								array[i].improveRelationsDeclinedUnderCurrentExecutivePair.Remove(this);
							}
							if (faction.isActivePlayer)
							{
								if (this.controlPoints.Count >= 4)
								{
									faction.CompleteMilestone(CampaignMilestone.TutorialGainLargeNationExecutiveControlPoint);
								}
								faction.UnlockAchievement("controlNation");
								if (this.boostIncome_year_dekatons > 0f)
								{
									faction.UnlockAchievement("controlLaunchFacility");
								}
								List<TINationState> executiveNations = faction.executiveNations;
								if (executiveNations != null && executiveNations.Count >= 20)
								{
									faction.UnlockAchievement("controlManyNations");
								}
							}
							TINotificationQueueState.LogFirstExecutiveControlPoint(controlPoint);
						}
					}
					else
					{
						this.ApplyInvestmentTemplateToControlPoint(controlPoint.positionInNation, this.template.initialPriorityPreset[controlPoint.positionInNation]);
					}
					controlPoint.SetControlPointType();
					controlPoint.SetDisplayName();
					if (faction2 != null)
					{
						faction2.SetResourceIncomeDataDirty(TINationState.NationalResources);
					}
					if (faction != null)
					{
						faction.SetResourceIncomeDataDirty(TINationState.NationalResources);
					}
					if (this.executiveControlPoint == controlPoint)
					{
						this.lastExecutiveChange = new LastExecutiveChange(faction, TITimeState.Now(), cause);
					}
				}
			}
			if (cause == ControlPointChangeCause.Enthrall || cause == ControlPointChangeCause.Terrorize)
			{
				faction.RegisterControlPointRecievedFromAliens(controlPoint);
			}
		}

		// Token: 0x06003803 RID: 14339 RVA: 0x00145291 File Offset: 0x00143491
		public TIControlPoint GetControlPoint(int index)
		{
			if (index < 0 || index >= this.controlPoints.Count || this.controlPoints[index] == null)
			{
				return null;
			}
			return this.controlPoints[index];
		}

		// Token: 0x06003804 RID: 14340 RVA: 0x001452C7 File Offset: 0x001434C7
		public void RemoveControlPointFromNation(TIControlPoint controlpoint)
		{
			if (this.controlPoints.Remove(controlpoint))
			{
				this.numControlPoints--;
				this.numControlPoints_unclamped = this.getNumControlPoints_unclamped;
			}
		}

		// Token: 0x170009AD RID: 2477
		// (get) Token: 0x06003805 RID: 14341 RVA: 0x001452F1 File Offset: 0x001434F1
		public TIControlPoint executiveControlPoint
		{
			get
			{
				return this.controlPoints[this.maxControlPointIndex];
			}
		}

		// Token: 0x170009AE RID: 2478
		// (get) Token: 0x06003806 RID: 14342 RVA: 0x00145304 File Offset: 0x00143504
		public TIControlPoint numberTwoControlPoint
		{
			get
			{
				if (this.numControlPoints != 1)
				{
					return this.controlPoints[this.maxControlPointIndex - 1];
				}
				return null;
			}
		}

		// Token: 0x170009AF RID: 2479
		// (get) Token: 0x06003807 RID: 14343 RVA: 0x00145324 File Offset: 0x00143524
		public TIFactionState executiveFaction
		{
			get
			{
				return this.executiveControlPoint.faction;
			}
		}

		// Token: 0x06003808 RID: 14344 RVA: 0x00145331 File Offset: 0x00143531
		public int CountFactionControlPointsByIdeology(FactionIdeology ideology, bool includeDisabled, bool includeDefended)
		{
			return this.CountFactionControlPoints(TIFactionIdeologyTemplate.GetFactionByIdeology(ideology), includeDisabled, false, includeDefended);
		}

		// Token: 0x06003809 RID: 14345 RVA: 0x00145342 File Offset: 0x00143542
		public int CountFactionControlPoints(TIFactionState council, bool includeDisabled, bool includePermanentAllies, bool includeDefended)
		{
			return this.FactionControlPoints(council, includeDisabled, includePermanentAllies, includeDefended).Count;
		}

		// Token: 0x0600380A RID: 14346 RVA: 0x00145354 File Offset: 0x00143554
		public bool FactionHasControlPoint(TIFactionState faction)
		{
			return this.controlPoints.Any<TIControlPoint>((TIControlPoint x) => x.faction == faction);
		}

		// Token: 0x170009B0 RID: 2480
		// (get) Token: 0x0600380B RID: 14347 RVA: 0x00145388 File Offset: 0x00143588
		public float ControlPointMaintenanceCost
		{
			get
			{
				if (!this.alienNation)
				{
					return (float)(Mathd.Pow(this.GDP / (double)TIGlobalValuesState.PCGDPToRaiseBaseCPMaintenanceCostBy1, (double)TIGlobalConfig.globalConfig.controlPointCostScaling) / (double)(TemplateManager.global.controlPointMaintenanceDivisor * (float)this.numControlPoints)) * GameStateManager.Time().template.CPMaintenanceModifier;
				}
				return 0f;
			}
		}

		// Token: 0x170009B1 RID: 2481
		// (get) Token: 0x0600380C RID: 14348 RVA: 0x001453E8 File Offset: 0x001435E8
		public bool MajorGlobalPower
		{
			get
			{
				return this.numControlPoints >= 5 || (this.numControlPoints == 4 && this.numStandardArmies > 0 && (this.rawBoostPerMonth_dekatons >= 1f || this.boostIncome_month_dekatons >= 1f) && this.regions.Count > 1 && this.claims.Count >= 5);
			}
		}

		// Token: 0x170009B2 RID: 2482
		// (get) Token: 0x0600380D RID: 14349 RVA: 0x00145450 File Offset: 0x00143650
		public bool SignificantPower
		{
			get
			{
				return this.numControlPoints >= 5 || (this.numControlPoints == 4 && (this.numStandardArmies > 0 || this.spaceFlightProgram || this.perCapitaGDP >= 35000f || this.claims.Count >= 5));
			}
		}

		// Token: 0x0600380E RID: 14350 RVA: 0x001454A4 File Offset: 0x001436A4
		public TIControlPoint GetControlPointOfType(ControlPointType cpType)
		{
			return this.controlPoints.FirstOrDefault<TIControlPoint>((TIControlPoint x) => x.controlPointType == cpType);
		}

		// Token: 0x0600380F RID: 14351 RVA: 0x001454D5 File Offset: 0x001436D5
		public TIFactionState GetControlPointOfTypeFaction(ControlPointType cpType)
		{
			TIControlPoint controlPointOfType = this.GetControlPointOfType(cpType);
			if (controlPointOfType == null)
			{
				return null;
			}
			return controlPointOfType.faction;
		}

		// Token: 0x06003810 RID: 14352 RVA: 0x001454EC File Offset: 0x001436EC
		public void UpdateControlPointTypes()
		{
			for (int i = 0; i < this.numControlPoints; i++)
			{
				this.controlPoints[i].SetControlPointType();
				this.controlPoints[i].SetDisplayName();
			}
		}

		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x06003811 RID: 14353 RVA: 0x0014552C File Offset: 0x0014372C
		public List<TIGameState> controlPointOwnersByPoint
		{
			get
			{
				List<TIControlPoint> list = this.controlPoints.OrderBy<TIControlPoint, int>((TIControlPoint x) => x.positionInNation).ToList<TIControlPoint>();
				List<TIGameState> list2 = new List<TIGameState>();
				foreach (TIControlPoint ticontrolPoint in list)
				{
					if (ticontrolPoint.faction != null)
					{
						list2.Add(ticontrolPoint.faction);
					}
					else
					{
						list2.Add(this);
					}
				}
				return list2;
			}
		}

		// Token: 0x06003812 RID: 14354 RVA: 0x001455CC File Offset: 0x001437CC
		public List<TIControlPoint> FactionControlPoints(TIFactionState faction, bool includeDisabled, bool includePermanentAllies, bool includeDefended)
		{
			List<TIControlPoint> list = new List<TIControlPoint>();
			int i = 0;
			while (i <= this.maxControlPointIndex)
			{
				if (this.controlPoints[i].faction == faction)
				{
					goto IL_0046;
				}
				if (includePermanentAllies)
				{
					TIFactionState faction2 = this.controlPoints[i].faction;
					if (faction2 != null && faction2.permanentAlly(faction))
					{
						goto IL_0046;
					}
				}
				IL_0085:
				i++;
				continue;
				IL_0046:
				if ((includeDisabled || !this.controlPoints[i].benefitsDisabled) && (includeDefended || !this.controlPoints[i].defended))
				{
					list.Add(this.controlPoints[i]);
					goto IL_0085;
				}
				goto IL_0085;
			}
			return list;
		}

		// Token: 0x06003813 RID: 14355 RVA: 0x0014566F File Offset: 0x0014386F
		public float CouncilControlPointFraction(TIFactionState council, bool includeDisabled, bool includePermanentAllies)
		{
			return (float)this.CountFactionControlPoints(council, includeDisabled, includePermanentAllies, true) / (float)this.numControlPoints;
		}

		// Token: 0x06003814 RID: 14356 RVA: 0x00145684 File Offset: 0x00143884
		public float CouncilControlPointFraction_DiscountNeutral(TIFactionState council, bool includeDisabled, bool includePermanentAllies)
		{
			int numOwnedControlPoints = this.NumOwnedControlPoints;
			if (numOwnedControlPoints > 0)
			{
				return (float)this.CountFactionControlPoints(council, includeDisabled, includePermanentAllies, true) / (float)numOwnedControlPoints;
			}
			return 0f;
		}

		// Token: 0x06003815 RID: 14357 RVA: 0x001456B0 File Offset: 0x001438B0
		public IEnumerable<TIControlPoint> EnemyControlPoints(TIFactionState faction)
		{
			return this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.EnemyFactionControlPoint(faction));
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x06003816 RID: 14358 RVA: 0x001456E1 File Offset: 0x001438E1
		public IEnumerable<TIControlPoint> NativeControlPoints
		{
			get
			{
				return this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => !x.owned);
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06003817 RID: 14359 RVA: 0x0014570D File Offset: 0x0014390D
		public int NumNativeControlPoints
		{
			get
			{
				return this.NativeControlPoints.Count<TIControlPoint>();
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06003818 RID: 14360 RVA: 0x0014571A File Offset: 0x0014391A
		public int NumOwnedControlPoints
		{
			get
			{
				return this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.owned).Count<TIControlPoint>();
			}
		}

		// Token: 0x06003819 RID: 14361 RVA: 0x0014574C File Offset: 0x0014394C
		public TIControlPoint FirstNativeControlPoint()
		{
			for (int i = 0; i <= this.maxControlPointIndex; i++)
			{
				TIControlPoint controlPoint = this.GetControlPoint(i);
				if (!controlPoint.owned)
				{
					return controlPoint;
				}
			}
			return null;
		}

		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x0600381A RID: 14362 RVA: 0x00145780 File Offset: 0x00143980
		public TIFactionState TotalOwningFaction
		{
			get
			{
				TIControlPoint controlPoint = this.GetControlPoint(0);
				if (!controlPoint.owned)
				{
					return null;
				}
				TIFactionState faction = controlPoint.faction;
				for (int i = 1; i <= this.maxControlPointIndex; i++)
				{
					TIControlPoint controlPoint2 = this.GetControlPoint(i);
					if (!controlPoint2.owned)
					{
						return null;
					}
					if (controlPoint2.faction != faction)
					{
						return null;
					}
				}
				return faction;
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x0600381B RID: 14363 RVA: 0x001457DA File Offset: 0x001439DA
		public TIFactionState MajorityControlFaction
		{
			get
			{
				if (this.executiveFaction != null && this.CouncilControlPointFraction(this.executiveFaction, true, false) > 0.5f)
				{
					return this.executiveFaction;
				}
				return null;
			}
		}

		// Token: 0x0600381C RID: 14364 RVA: 0x00145808 File Offset: 0x00143A08
		public TIControlPoint HighestFactionControlPoint(TIFactionState council, bool includeDefended)
		{
			for (int i = this.maxControlPointIndex; i >= 0; i--)
			{
				TIControlPoint controlPoint = this.GetControlPoint(i);
				if (controlPoint.owned && controlPoint.faction == council && (includeDefended || !controlPoint.defended))
				{
					return controlPoint;
				}
			}
			return null;
		}

		// Token: 0x0600381D RID: 14365 RVA: 0x00145854 File Offset: 0x00143A54
		public TIControlPoint HighestOtherFactionControlPoint(TIFactionState council, bool includeDefended, bool requireAttackable)
		{
			for (int i = this.maxControlPointIndex; i >= 0; i--)
			{
				TIControlPoint controlPoint = this.GetControlPoint(i);
				if (controlPoint.owned && controlPoint.faction != council && (includeDefended || !controlPoint.defended) && (!requireAttackable || controlPoint.CanBeAttacked(council)))
				{
					return controlPoint;
				}
			}
			return null;
		}

		// Token: 0x0600381E RID: 14366 RVA: 0x001458AC File Offset: 0x00143AAC
		public TIControlPoint RandomOtherFactionControlPoint(TIFactionState council, bool includeDefended, bool requireAttackable)
		{
			List<TIControlPoint> list = new List<TIControlPoint>();
			for (int i = this.maxControlPointIndex; i >= 0; i--)
			{
				TIControlPoint controlPoint = this.GetControlPoint(i);
				if (controlPoint.owned && controlPoint.faction != council && (includeDefended || !controlPoint.defended) && (!requireAttackable || controlPoint.CanBeAttacked(council)))
				{
					list.Add(controlPoint);
				}
			}
			return list.SelectRandomItem<TIControlPoint>();
		}

		// Token: 0x0600381F RID: 14367 RVA: 0x00145914 File Offset: 0x00143B14
		public TIControlPoint LowestOtherFactionControlPoint(TIFactionState filterFaction)
		{
			return this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction != null && x.faction != filterFaction).MinBy<TIControlPoint, int>((TIControlPoint x) => x.positionInNation);
		}

		// Token: 0x170009B9 RID: 2489
		// (get) Token: 0x06003820 RID: 14368 RVA: 0x0014596C File Offset: 0x00143B6C
		public List<TIFactionState> FactionsWithControlPoint
		{
			get
			{
				return (from x in this.controlPoints
					where x.owned
					select x.faction).Distinct<TIFactionState>().ToList<TIFactionState>();
			}
		}

		// Token: 0x06003821 RID: 14369 RVA: 0x001459D4 File Offset: 0x00143BD4
		public TIFactionState WeightedRandomFactionByControlPoints()
		{
			List<TIFactionState> list = new List<TIFactionState>();
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				if (ticontrolPoint.owned)
				{
					list.Add(ticontrolPoint.faction);
				}
			}
			if (list.Count > 0)
			{
				return list.SelectRandomItem<TIFactionState>();
			}
			return null;
		}

		// Token: 0x06003822 RID: 14370 RVA: 0x00145A4C File Offset: 0x00143C4C
		public TIFactionState GetControlPointTypeOwner(ControlPointType controlPointType)
		{
			TIControlPoint ticontrolPoint = (this.extant ? this.controlPoints.FirstOrDefault<TIControlPoint>((TIControlPoint x) => x.controlPointType == controlPointType) : null);
			return ((ticontrolPoint != null) ? ticontrolPoint.faction : null) ?? null;
		}

		// Token: 0x06003823 RID: 14371 RVA: 0x00145A99 File Offset: 0x00143C99
		public bool CanDisableControlPoints(TIFactionState faction)
		{
			return this.CountFactionControlPoints(faction, false, false, true) > 0;
		}

		// Token: 0x06003824 RID: 14372 RVA: 0x00145AA8 File Offset: 0x00143CA8
		public void SelfDisableControlPoints(TIFactionState faction)
		{
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				if (ticontrolPoint.faction == faction)
				{
					ticontrolPoint.ResolveCrackdownEffect(TemplateManager.global.selfDisableControlPointDuration_months, faction, true, false, 0f);
				}
			}
		}

		// Token: 0x170009BA RID: 2490
		// (get) Token: 0x06003825 RID: 14373 RVA: 0x00145B1C File Offset: 0x00143D1C
		public float base_consolidateExecControl_days
		{
			get
			{
				return (TemplateManager.global.consolidateExecControl_d + (float)this.numControlPoints * TemplateManager.global.consolidateExecControl_perCP) / (TIGlobalValuesState.Customizations.usingCustomizations ? TIGlobalValuesState.Customizations.nationalIPMultiplier : 1f);
			}
		}

		// Token: 0x170009BB RID: 2491
		// (get) Token: 0x06003826 RID: 14374 RVA: 0x00145B59 File Offset: 0x00143D59
		public float modifiedConsolidatedExecControl_days
		{
			get
			{
				return this.base_consolidateExecControl_days + TIEffectsState.SumEffectsModifiers(Context.ConsolidatePowerDurationMultiplier, this.executiveFaction, this.base_consolidateExecControl_days, null);
			}
		}

		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06003827 RID: 14375 RVA: 0x00145B78 File Offset: 0x00143D78
		public float daysUntilExecutivePowerConsolidated
		{
			get
			{
				if (TemplateManager.global.consolidationRequiredExecChange.Contains(this.lastExecutiveChange.cause))
				{
					return Mathf.Max(-1f, this.modifiedConsolidatedExecControl_days - (float)TITimeState.Now().DifferenceInDays(this.lastExecutiveChange.date));
				}
				return -1f;
			}
		}

		// Token: 0x06003828 RID: 14376 RVA: 0x00145BD0 File Offset: 0x00143DD0
		public TIDateTime ExecutivePowerConsolidationDate()
		{
			float daysUntilExecutivePowerConsolidated = this.daysUntilExecutivePowerConsolidated;
			if (daysUntilExecutivePowerConsolidated > 0f)
			{
				TIDateTime tidateTime = TITimeState.Now();
				tidateTime.AddDays(daysUntilExecutivePowerConsolidated);
				return tidateTime;
			}
			return null;
		}

		// Token: 0x170009BD RID: 2493
		// (get) Token: 0x06003829 RID: 14377 RVA: 0x00145BFA File Offset: 0x00143DFA
		public bool ExecutivePowerConsolidated
		{
			get
			{
				return this.executiveFaction == null || this.executiveFaction.IsAlienFaction || this.daysUntilExecutivePowerConsolidated <= 0f;
			}
		}

		// Token: 0x0600382A RID: 14378 RVA: 0x00145C2C File Offset: 0x00143E2C
		public void PossiblePriorityValidationChange(bool alertReset = true)
		{
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				ticontrolPoint.RecordAndFixControlPointValues(alertReset);
			}
		}

		// Token: 0x0600382B RID: 14379 RVA: 0x00145C80 File Offset: 0x00143E80
		public float GetInvestmentFromControlPoint()
		{
			return this.BaseInvestmentPoints_month() / (float)this.numControlPoints;
		}

		// Token: 0x0600382C RID: 14380 RVA: 0x00145C90 File Offset: 0x00143E90
		public float GetMonthlyMoneyIncomeFromControlPoint(TIFactionState faction)
		{
			return this.spaceFundingIncome_month * ((faction != null && this.GetControlPointTypeOwner(ControlPointType.FinancialSector) == faction) ? TemplateManager.global.financialSectorFundingBonus : 1f) / (float)this.numControlPoints;
		}

		// Token: 0x0600382D RID: 14381 RVA: 0x00145CCB File Offset: 0x00143ECB
		public float GetMonthlyBoostIncomeFromControlPoint()
		{
			return this.boostIncome_month_dekatons / (float)this.numControlPoints;
		}

		// Token: 0x0600382E RID: 14382 RVA: 0x00145CDC File Offset: 0x00143EDC
		public int GetMissionControlFromControlPoint(int controlPointIndex)
		{
			int num = this.currentMissionControl / this.numControlPoints;
			if (controlPointIndex >= this.numControlPoints - this.currentMissionControl % this.numControlPoints)
			{
				num++;
			}
			return num;
		}

		// Token: 0x0600382F RID: 14383 RVA: 0x00145D14 File Offset: 0x00143F14
		public float GetMonthlyResearchFromControlPoint(TIFactionState faction)
		{
			float num = this.research_month * ((faction != null && this.GetControlPointTypeOwner(ControlPointType.KnowledgeSector) == faction) ? TemplateManager.global.knowledgeSectorResearchBonus : 1f);
			num += TIEffectsState.SumEffectsModifiers(Context.ControlPointResearch, faction, num, null);
			return num / (float)this.numControlPoints;
		}

		// Token: 0x06003830 RID: 14384 RVA: 0x00145D69 File Offset: 0x00143F69
		public float GetCouncilInvestmentPointShare(TIFactionState council)
		{
			return this.GetInvestmentFromControlPoint() * (float)this.CountFactionControlPoints(council, false, false, true);
		}

		// Token: 0x06003831 RID: 14385 RVA: 0x00145D80 File Offset: 0x00143F80
		public float GetFactionMissionControlFromNation(TIFactionState council, bool includeDisabled)
		{
			int num = 0;
			if (council.IsActiveHumanFaction)
			{
				foreach (TIControlPoint ticontrolPoint in this.controlPoints)
				{
					if (ticontrolPoint.faction == council && (!ticontrolPoint.benefitsDisabled || includeDisabled))
					{
						num += this.GetMissionControlFromControlPoint(ticontrolPoint.positionInNation);
					}
				}
				if (this.alienNation && council.IsAlienProxy)
				{
					num += this.currentMissionControl;
				}
			}
			return (float)num;
		}

		// Token: 0x06003832 RID: 14386 RVA: 0x00145E1C File Offset: 0x0014401C
		public float GetMonthlyCouncilResourceShare(TIFactionState faction, FactionResource resourceType, bool includeInactives = false)
		{
			switch (resourceType)
			{
			case FactionResource.Money:
				return this.GetMonthlyMoneyIncomeFromControlPoint(faction) * (float)this.CountFactionControlPoints(faction, includeInactives, false, true);
			case FactionResource.Influence:
				if (!faction.defeated)
				{
					float num = this.population_Millions * this.GetPublicOpinionOfFaction(faction) * 0.5f * (1f + TIEffectsState.SumEffectsModifiers(Context.PublicOpinionInfluence, faction, 1f, null));
					if (faction.IsAlienFaction)
					{
						num *= (float)Mathf.Min(this.abductions, 50) / 100f;
					}
					return num / 12f;
				}
				return 0f;
			case FactionResource.Research:
				return this.GetMonthlyResearchFromControlPoint(faction) * (float)this.CountFactionControlPoints(faction, includeInactives, false, true);
			case FactionResource.Boost:
				return this.GetMonthlyBoostIncomeFromControlPoint() * (float)this.CountFactionControlPoints(faction, includeInactives, false, true);
			case FactionResource.MissionControl:
				return this.GetFactionMissionControlFromNation(faction, includeInactives);
			}
			return 0f;
		}

		// Token: 0x06003833 RID: 14387 RVA: 0x00145EFC File Offset: 0x001440FC
		public List<TIArmyState> GetArmiesByControlPoint(int checkControlPoint)
		{
			return this.armies.Where<TIArmyState>((TIArmyState army) => army.controlPointIdx == checkControlPoint).ToList<TIArmyState>();
		}

		// Token: 0x06003834 RID: 14388 RVA: 0x00145F32 File Offset: 0x00144132
		public int GetNumArmiesAtControlPoint(int checkControlPoint)
		{
			return this.GetArmiesByControlPoint(checkControlPoint).Count;
		}

		// Token: 0x06003835 RID: 14389 RVA: 0x00145F40 File Offset: 0x00144140
		public int GetNumArmiesForFaction(TIFactionState councilState)
		{
			return this.controlPoints.Where<TIControlPoint>((TIControlPoint cp) => cp.faction == councilState).Sum<TIControlPoint>((TIControlPoint cp) => this.GetNumArmiesAtControlPoint(cp.positionInNation));
		}

		// Token: 0x06003836 RID: 14390 RVA: 0x00145F8C File Offset: 0x0014418C
		public void AddArmy(TIArmyState army)
		{
			if (!this.armies.Contains(army))
			{
				if (this.armies.Count != 0)
				{
					if (army.deploymentType != DeploymentType.Naval)
					{
						goto IL_0056;
					}
					if (this.armies.Count<TIArmyState>((TIArmyState x) => x.deploymentType == DeploymentType.Naval) != 0)
					{
						goto IL_0056;
					}
				}
				this.SetArmyAccessibilityDirty();
				IL_0056:
				this.armies.Add(army);
			}
		}

		// Token: 0x06003837 RID: 14391 RVA: 0x00145FFC File Offset: 0x001441FC
		public bool RemoveArmy(TIArmyState army)
		{
			if (!this.armies.Contains(army))
			{
				return false;
			}
			if (this.armies.Count != 1)
			{
				if (army.deploymentType != DeploymentType.Naval)
				{
					goto IL_005A;
				}
				if (this.armies.Count<TIArmyState>((TIArmyState x) => x.deploymentType == DeploymentType.Naval) != 1)
				{
					goto IL_005A;
				}
			}
			this.SetArmyAccessibilityDirty();
			IL_005A:
			return this.armies.Remove(army);
		}

		// Token: 0x06003838 RID: 14392 RVA: 0x0014606F File Offset: 0x0014426F
		public void ClearArmies()
		{
			this.armies.Clear();
			this.SetArmyAccessibilityDirty();
		}

		// Token: 0x06003839 RID: 14393 RVA: 0x00146084 File Offset: 0x00144284
		public TIPriorityPresetTemplate PlayerSettingsMatchTemplate(int controlPointIndex, bool validate = true)
		{
			TIControlPoint controlPoint = this.GetControlPoint(controlPointIndex);
			if (controlPoint != null)
			{
				foreach (TIPriorityPresetTemplate tipriorityPresetTemplate in TemplateManager.IterateByClass<TIPriorityPresetTemplate>(true))
				{
					if (!tipriorityPresetTemplate.deleted && (!validate || tipriorityPresetTemplate.ValidPreset(this, controlPoint.faction)) && tipriorityPresetTemplate.MatchesPreset(controlPoint.controlPointPriorities, controlPoint.nation.InvalidPriorities))
					{
						return tipriorityPresetTemplate;
					}
				}
			}
			return null;
		}

		// Token: 0x0600383A RID: 14394 RVA: 0x00146118 File Offset: 0x00144318
		public void ApplyInvestmentTemplateToControlPoint(int controlPointIndex, string investmentTemplateName)
		{
			TIPriorityPresetTemplate tipriorityPresetTemplate = TemplateManager.Find<TIPriorityPresetTemplate>(investmentTemplateName, false);
			this.ApplyInvestmentTemplateToControlPoint(controlPointIndex, tipriorityPresetTemplate);
		}

		// Token: 0x0600383B RID: 14395 RVA: 0x00146138 File Offset: 0x00144338
		public void ApplyInvestmentTemplateToControlPoint(int controlPointIndex, TIPriorityPresetTemplate investmentTemplate)
		{
			TIControlPoint controlPoint = this.GetControlPoint(controlPointIndex);
			if (controlPoint != null)
			{
				if (investmentTemplate != null)
				{
					foreach (PriorityType priorityType in Enums.PriorityTypes)
					{
						controlPoint.SetControlPointPriority(priorityType, investmentTemplate.GetPreset(priorityType), true, true, false);
					}
					controlPoint.SetControlPointPriority(PriorityType.Economy, investmentTemplate.GetPreset(PriorityType.Economy), false, false, false);
					return;
				}
				controlPoint.SetControlPointPriority(PriorityType.Economy, 1, false, false, false);
			}
		}

		// Token: 0x170009BE RID: 2494
		// (get) Token: 0x0600383C RID: 14396 RVA: 0x001461A2 File Offset: 0x001443A2
		public int armiesAtHome
		{
			get
			{
				return this.armies.Where<TIArmyState>((TIArmyState army) => army.useHomeInvestmentFactor).Count<TIArmyState>();
			}
		}

		// Token: 0x170009BF RID: 2495
		// (get) Token: 0x0600383D RID: 14397 RVA: 0x001461D3 File Offset: 0x001443D3
		public int deployedArmies
		{
			get
			{
				return this.armies.Where<TIArmyState>((TIArmyState army) => !army.useHomeInvestmentFactor && !army.AlienMegafaunaArmy).Count<TIArmyState>();
			}
		}

		// Token: 0x170009C0 RID: 2496
		// (get) Token: 0x0600383E RID: 14398 RVA: 0x00146204 File Offset: 0x00144404
		public float investmentPoints_unrestPenalty_frac
		{
			get
			{
				return Mathf.Max(this.unrest - 2f, 0f) / 10f;
			}
		}

		// Token: 0x170009C1 RID: 2497
		// (get) Token: 0x0600383F RID: 14399 RVA: 0x00146222 File Offset: 0x00144422
		public float investmentPoints_occupationPenalty_frac
		{
			get
			{
				return this.regions.Sum<TIRegionState>(delegate(TIRegionState x)
				{
					TINationState tinationState;
					List<TINationState> list;
					return x.NationalGDPProportion() * x.GetHighestWarAllianceOccupationValue(out tinationState, out list);
				});
			}
		}

		// Token: 0x06003840 RID: 14400 RVA: 0x00146250 File Offset: 0x00144450
		protected void SetBaseInvestmentPoints_month()
		{
			this.baseInvestmentPoints_month = this.economyScore;
			this.baseInvestmentPoints_month *= 1f + this.adviserAdministrationBonus;
			this.baseInvestmentPoints_month *= 1f - this.investmentPoints_occupationPenalty_frac;
			this.baseInvestmentPoints_month *= 1f - this.investmentPoints_unrestPenalty_frac;
			this.baseInvestmentPoints_month -= this.armies.Sum<TIArmyState>((TIArmyState x) => x.investmentArmyFactor);
			this.baseInvestmentPoints_month -= this.armies.Where<TIArmyState>((TIArmyState x) => x.deploymentType == DeploymentType.Naval).Sum<TIArmyState>((TIArmyState x) => x.investmentNavyFactor);
			this.baseInvestmentPoints_month = Mathf.Max(this.baseInvestmentPoints_month, 0f);
		}

		// Token: 0x06003841 RID: 14401 RVA: 0x0014635C File Offset: 0x0014455C
		public float BaseInvestmentPoints_month()
		{
			return this.baseInvestmentPoints_month;
		}

		// Token: 0x06003842 RID: 14402 RVA: 0x00146364 File Offset: 0x00144564
		public float BaseInvestmentPoints_month(TIFactionState faction)
		{
			return (float)this.CountFactionControlPoints(faction, true, false, true) * this.BaseInvestmentPoints_month() / (float)this.numControlPoints;
		}

		// Token: 0x06003843 RID: 14403 RVA: 0x00146380 File Offset: 0x00144580
		public float GetAccumulatedInvestmentPoints(PriorityType priority)
		{
			float num;
			this._accumulatedInvestmentPoints.TryGetValue(priority, out num);
			return num;
		}

		// Token: 0x06003844 RID: 14404 RVA: 0x0014639D File Offset: 0x0014459D
		public void SetAccumulatedInvestmentPoints(PriorityType priority, float value, bool triggerUpdate)
		{
			this._accumulatedInvestmentPoints[priority] = value;
			if (triggerUpdate)
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x06003845 RID: 14405 RVA: 0x001463B8 File Offset: 0x001445B8
		public float GetInitialInvestmentPoints(PriorityType priority)
		{
			switch (priority)
			{
			case PriorityType.Civilian_InitiateSpaceflightProgram:
				return (this.GetRequiredInvestmentPointsForPriority(priority) * this.template.initSpaceIPs).GetValueOrDefault();
			case PriorityType.Military_FoundMilitary:
				return (this.GetRequiredInvestmentPointsForPriority(priority) * this.template.foundMilitaryIPs).GetValueOrDefault();
			case PriorityType.Military_BuildArmy:
				return (this.GetRequiredInvestmentPointsForPriority(priority) * this.template.buildArmyIPs).GetValueOrDefault();
			case PriorityType.Military_BuildNavy:
				return (this.GetRequiredInvestmentPointsForPriority(priority) * this.template.buildNavyIPs).GetValueOrDefault();
			case PriorityType.Military_InitiateNuclearProgram:
				return (this.GetRequiredInvestmentPointsForPriority(priority) * this.template.nuclearProgramIPs).GetValueOrDefault();
			case PriorityType.Military_BuildNuclearWeapons:
				return (this.GetRequiredInvestmentPointsForPriority(priority) * this.template.buildNukeIPs).GetValueOrDefault();
			}
			return 0f;
		}

		// Token: 0x06003846 RID: 14406 RVA: 0x00146578 File Offset: 0x00144778
		public float GetRequiredInvestmentPointsForPriority(PriorityType priority)
		{
			if (priority == PriorityType.Military_BuildSpaceDefenses)
			{
				float requiredInvestmentPoints = TemplateManager.global.GetRequiredInvestmentPoints(priority);
				TIRegionState nextSpaceDefensesRegion = this.GetNextSpaceDefensesRegion();
				return requiredInvestmentPoints * Mathf.Clamp(((nextSpaceDefensesRegion != null) ? nextSpaceDefensesRegion.area_km2 : TIGlobalValuesState.GlobalValues.medianRegionArea_km2) / TIGlobalValuesState.GlobalValues.medianRegionArea_km2, 0.5f, 2f);
			}
			return TemplateManager.global.GetRequiredInvestmentPoints(priority);
		}

		// Token: 0x06003847 RID: 14407 RVA: 0x001465D7 File Offset: 0x001447D7
		public bool ReachedInvestmentThreshhold(PriorityType priority)
		{
			return this.GetAccumulatedInvestmentPoints(priority) >= this.GetRequiredInvestmentPointsForPriority(priority);
		}

		// Token: 0x06003848 RID: 14408 RVA: 0x001465EC File Offset: 0x001447EC
		public float DeltaToInvestmentThreshhold(PriorityType priority)
		{
			return this.GetRequiredInvestmentPointsForPriority(priority) - this.GetAccumulatedInvestmentPoints(priority);
		}

		// Token: 0x06003849 RID: 14409 RVA: 0x00146600 File Offset: 0x00144800
		public bool ValidPriority(PriorityType priority)
		{
			switch (priority)
			{
			case PriorityType.Economy:
			case PriorityType.Welfare:
			case PriorityType.Knowledge:
			case PriorityType.Unity:
			case PriorityType.Spoils:
				return true;
			case PriorityType.Environment:
				return this.sustainability <= 0f || this.sustainability > TINationState.BestCurrentSustainabilityValue(false) || this.canAccumulateDecontaminateTriggers;
			case PriorityType.Government:
				return this.democracy < 10f || this.canAccumulateLegitimizeClaimTriggers;
			case PriorityType.Oppression:
				return this.military;
			case PriorityType.Funding:
				return this.spaceFunding_year < this.maxFunding_year;
			case PriorityType.Civilian_InitiateSpaceflightProgram:
				return !this.spaceFlightProgram;
			case PriorityType.LaunchFacilities:
				if (!this.spaceFlightProgram)
				{
					TIFederationState tifederationState = this.federation;
					return tifederationState != null && tifederationState.spaceProgram;
				}
				return true;
			case PriorityType.MissionControl:
				if (!this.spaceFlightProgram)
				{
					TIFederationState tifederationState2 = this.federation;
					if (tifederationState2 == null || !tifederationState2.spaceProgram)
					{
						return false;
					}
				}
				return this.regions.Any<TIRegionState>((TIRegionState x) => x.missionControl < x.maxMissionControl);
			case PriorityType.Military_FoundMilitary:
				return !this.military;
			case PriorityType.Military:
				return this.military && this.militaryTechLevel < this.maxMilitaryTechLevel;
			case PriorityType.Military_BuildArmy:
				return this.canBuildArmy;
			case PriorityType.Military_BuildNavy:
				return this.canBuildNavy;
			case PriorityType.Military_InitiateNuclearProgram:
				return this.military && !this.nuclearProgram && !this.policy_noNukes;
			case PriorityType.Military_BuildNuclearWeapons:
				return this.nuclearProgram && !this.policy_noNukes;
			case PriorityType.Military_BuildSpaceDefenses:
				return this.military && this.canBuildSpaceDefenses && !this.completeAntiSpaceDefenses;
			case PriorityType.Military_BuildSTOSquadron:
				if (this.military && this.canBuildSTOSquadrons && this.rawBoostPerYear_dekatons > 0f)
				{
					return this.regions.Any<TIRegionState>((TIRegionState x) => x.numSTOFighters < x.maxSTOFighters);
				}
				return false;
			default:
				return false;
			}
		}

		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x0600384A RID: 14410 RVA: 0x001467F1 File Offset: 0x001449F1
		public List<PriorityType> ValidPriorities
		{
			get
			{
				return Enums.PriorityTypes.Where<PriorityType>((PriorityType x) => this.ValidPriority(x)).ToList<PriorityType>();
			}
		}

		// Token: 0x170009C3 RID: 2499
		// (get) Token: 0x0600384B RID: 14411 RVA: 0x0014680E File Offset: 0x00144A0E
		public List<PriorityType> InvalidPriorities
		{
			get
			{
				return Enums.PriorityTypes.Where<PriorityType>((PriorityType x) => !this.ValidPriority(x)).ToList<PriorityType>();
			}
		}

		// Token: 0x0600384C RID: 14412 RVA: 0x0014682C File Offset: 0x00144A2C
		public float percentWeighttoPriority(PriorityType priority)
		{
			float num = 0f;
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				float num2 = (float)ticontrolPoint.GetControlPointPriority(priority, true);
				num2 /= (float)ticontrolPoint.totalWeightsForControlPoint;
				num += num2 / (float)this.numControlPoints;
			}
			return num;
		}

		// Token: 0x0600384D RID: 14413 RVA: 0x001468A0 File Offset: 0x00144AA0
		public float ControlPointPriorityBonuses(TIControlPoint controlPoint, PriorityType priority, bool checkDisabled, bool ignoreDiversityBonus = false)
		{
			TIFactionState faction = controlPoint.faction;
			float num = ((faction != null) ? faction.cachedPriorityBonuses[priority] : 0f);
			float num2 = (ignoreDiversityBonus ? 0f : controlPoint.diversityBonus[priority]);
			float num3 = this.NationalPriorityBonuses(priority);
			float num4 = num + num2 + num3;
			if (num4 >= 0f && checkDisabled && controlPoint.benefitsDisabled)
			{
				return 0f;
			}
			return num4;
		}

		// Token: 0x0600384E RID: 14414 RVA: 0x0014690C File Offset: 0x00144B0C
		public float ControlPointPriorityBonuses_Uncached(TIControlPoint controlPoint, PriorityType priority, bool checkDisabled)
		{
			float num;
			if (!checkDisabled || !controlPoint.benefitsDisabled)
			{
				TIFactionState faction = controlPoint.faction;
				num = ((faction != null) ? faction.SumPriorityBonuses(priority, false) : 0f);
			}
			else
			{
				num = 0f;
			}
			return num + controlPoint.diversityBonus[priority] + this.NationalPriorityBonuses(priority);
		}

		// Token: 0x0600384F RID: 14415 RVA: 0x00146958 File Offset: 0x00144B58
		public float ControlPointWeightsTotalToPriorityIP(PriorityType priority)
		{
			float investmentFromControlPoint = this.GetInvestmentFromControlPoint();
			float num = 0f;
			for (int i = 0; i < this.controlPoints.Count; i++)
			{
				TIControlPoint ticontrolPoint = this.controlPoints[i];
				float num2 = investmentFromControlPoint * (float)ticontrolPoint.GetControlPointPriority(priority, true) / (float)ticontrolPoint.totalWeightsForControlPoint;
				if (num2 > 0f)
				{
					num2 *= 1f + this.ControlPointPriorityBonuses(ticontrolPoint, priority, true, false);
				}
				num += num2;
			}
			return num * 12f / 365.2422f;
		}

		// Token: 0x06003850 RID: 14416 RVA: 0x001469D9 File Offset: 0x00144BD9
		public float NationalPriorityBonuses(PriorityType priority)
		{
			if (priority == PriorityType.Economy)
			{
				return this.restofFederationECOBonus_dailyCache * TemplateManager.global.federationGDPEconomyBonus;
			}
			if (priority - PriorityType.Military_BuildArmy > 1)
			{
				return 0f;
			}
			return (float)this.numMiningRegions_dailyCache * TemplateManager.global.coreMineralBuildMilitaryModifier;
		}

		// Token: 0x170009C4 RID: 2500
		// (get) Token: 0x06003851 RID: 14417 RVA: 0x00146A14 File Offset: 0x00144C14
		public int MaxAnnualDirectInvestIPs
		{
			get
			{
				return Mathd.RoundToInt((double)(TIGlobalConfig.globalConfig.nationalDirectInvestmentCapGlobalMultiplier * 12f) * Mathd.Pow((double)this.population_Millions * 48.75, 0.175) * Mathd.Pow(this.GDP / 1000000000.0, 0.17499999701976776));
			}
		}

		// Token: 0x06003852 RID: 14418 RVA: 0x00146A76 File Offset: 0x00144C76
		public static bool EverAllowedForDirectInvest(PriorityType priority)
		{
			return priority != PriorityType.Spoils;
		}

		// Token: 0x06003853 RID: 14419 RVA: 0x00146A80 File Offset: 0x00144C80
		public bool CanDirectInvest(TIFactionState faction, PriorityType priority, out int maxAllowed)
		{
			maxAllowed = this.MaxDirectInvestIPsRemainingThisYear();
			if (maxAllowed > 0 && this.ValidPriority(priority))
			{
				switch (priority)
				{
				case PriorityType.Welfare:
				case PriorityType.Environment:
				case PriorityType.Knowledge:
				case PriorityType.Unity:
					return this.FactionHasControlPoint(faction) || (this.NumNativeControlPoints > 0 && !this.policy_closedBorders);
				case PriorityType.Government:
				case PriorityType.Oppression:
				case PriorityType.Civilian_InitiateSpaceflightProgram:
				case PriorityType.Military_InitiateNuclearProgram:
					return this.FactionHasControlPoint(faction);
				case PriorityType.Funding:
					maxAllowed = Mathf.Min(maxAllowed, (int)Math.Truncate((double)((this.maxFunding_year - this.spaceFunding_year) / this.spaceFundingPriorityIncomeChange)));
					return maxAllowed > 0 && (!this.policy_closedBorders || this.FactionHasControlPoint(faction));
				case PriorityType.Spoils:
					return false;
				case PriorityType.Military:
					maxAllowed = Mathf.Min(maxAllowed, (int)Math.Truncate((double)((this.maxMilitaryTechLevel - this.militaryTechLevel) / this.militaryPriorityTechLevelChange)));
					return maxAllowed > 0 && (!this.policy_closedBorders || this.FactionHasControlPoint(faction));
				}
				return !this.policy_closedBorders || this.FactionHasControlPoint(faction);
			}
			return false;
		}

		// Token: 0x06003854 RID: 14420 RVA: 0x00146BAD File Offset: 0x00144DAD
		public int MaxDirectInvestIPsRemainingThisYear()
		{
			return Mathf.RoundToInt(Mathf.Max(0f, (float)this.MaxAnnualDirectInvestIPs - this.directInvestmentedIPsThisYear));
		}

		// Token: 0x06003855 RID: 14421 RVA: 0x00146BCC File Offset: 0x00144DCC
		public void DirectInvestment(PriorityType priority, float IPs)
		{
			this.ModifyAccumulatedInvestment(priority, IPs, false, false);
			this.directInvestmentedIPsThisYear += IPs;
		}

		// Token: 0x06003856 RID: 14422 RVA: 0x00146BE8 File Offset: 0x00144DE8
		public bool SkipDirectInvestInfluenceCost(TIFactionState faction)
		{
			return this.lastExecutiveChange.newExecutive == faction && (this.lastExecutiveChange.cause == ControlPointChangeCause.Liberation || this.lastExecutiveChange.cause == ControlPointChangeCause.RegimeChange) && TITimeState.Now().DifferenceInDays(this.lastExecutiveChange.date) <= (double)TemplateManager.global.daysOfFreeDirectInvestAfterRegimeChange;
		}

		// Token: 0x06003857 RID: 14423 RVA: 0x00146C4C File Offset: 0x00144E4C
		public TIResourcesCost InvestmentPointDirectPurchasePrice(PriorityType priority, TIFactionState faction)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float corruption = this.corruption;
			float requiredInvestmentPointsForPriority = this.GetRequiredInvestmentPointsForPriority(priority);
			switch (priority)
			{
			case PriorityType.Economy:
				num = 1f * this.economyPriorityPerCapitaIncomeChange * this.population_Millions / requiredInvestmentPointsForPriority;
				num2 = 25f;
				break;
			case PriorityType.Welfare:
				num = 1800000f * -this.welfarePriorityInequalityChange * (this.population_Millions / 335f) / requiredInvestmentPointsForPriority;
				num2 = 100f;
				break;
			case PriorityType.Environment:
				num = 150000f * Mathf.Abs(this.environmentPrioritySustainabilityChange) / (requiredInvestmentPointsForPriority * this.priorityEffectPopScaling);
				num2 = 100f;
				break;
			case PriorityType.Knowledge:
				num = 250000f * this.priorityEffectPopScaling * TemplateManager.global.knowledgePriorityEducationIncrease * (this.population_Millions / 82f) / requiredInvestmentPointsForPriority;
				num2 = 100f;
				break;
			case PriorityType.Government:
				num = 150000f * this.governmentPriorityDemocracyChange / (this.priorityEffectPopScaling * requiredInvestmentPointsForPriority);
				num2 = 300f;
				break;
			case PriorityType.Unity:
				num = 400f / (this.priorityEffectPopScaling * requiredInvestmentPointsForPriority);
				num2 = 300f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Oppression:
				num2 = 50f * Mathf.Max(1f, this.democracy) * this.priorityEffectPopScaling * requiredInvestmentPointsForPriority;
				num3 = 30f / (1f + Math.Min(2f, this.militaryTechLevel / 10f));
				break;
			case PriorityType.Funding:
				num2 = 5f * this.spaceFundingPriorityIncomeChange / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Spoils:
				num2 = this.spoilsPriorityMoney * 2f * (1f - this.corruption) / (4f * this.priorityEffectPopScaling);
				break;
			case PriorityType.Civilian_InitiateSpaceflightProgram:
				num = 2800f / requiredInvestmentPointsForPriority;
				num2 = 2500f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.LaunchFacilities:
				num = 500f / (requiredInvestmentPointsForPriority * TemplateManager.global.boostPriorityIncreaseAtEquator * TemplateManager.global.spaceResourceToTons);
				num2 = 100f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.MissionControl:
				num = 2500f / requiredInvestmentPointsForPriority;
				num2 = 800f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_FoundMilitary:
				num = 2000f / requiredInvestmentPointsForPriority;
				num2 = 2000f / requiredInvestmentPointsForPriority;
				num3 = 1500f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military:
				num = 250f * this.militaryTechLevel / requiredInvestmentPointsForPriority;
				num2 = 250f / requiredInvestmentPointsForPriority;
				num3 = 30f * this.militaryTechLevel / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_BuildArmy:
				num = 10000f * this.militaryTechLevel / requiredInvestmentPointsForPriority;
				num2 = 3000f / requiredInvestmentPointsForPriority;
				num3 = 2000f * this.militaryTechLevel / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_BuildNavy:
				num = 1200f * this.militaryTechLevel / requiredInvestmentPointsForPriority;
				num2 = 3000f / requiredInvestmentPointsForPriority;
				num3 = 1500f * this.militaryTechLevel / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_InitiateNuclearProgram:
				num = 30000f / requiredInvestmentPointsForPriority;
				num2 = 3000f / requiredInvestmentPointsForPriority;
				num3 = 300f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_BuildNuclearWeapons:
				num = 5000f / requiredInvestmentPointsForPriority;
				num2 = 1500f / requiredInvestmentPointsForPriority;
				num3 = 150f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_BuildSpaceDefenses:
				num = 2500f / requiredInvestmentPointsForPriority;
				num2 = 800f / requiredInvestmentPointsForPriority;
				num3 = 600f / requiredInvestmentPointsForPriority;
				break;
			case PriorityType.Military_BuildSTOSquadron:
				num = 1000f / requiredInvestmentPointsForPriority;
				num2 = 200f / requiredInvestmentPointsForPriority;
				num3 = 150f / requiredInvestmentPointsForPriority;
				break;
			}
			if (num > 0f)
			{
				num *= 1.2f;
				num *= 1f + corruption * 0.75f;
				num *= 1f - TIEffectsState.SumEffectsModifiers(Context.DirectInvestGlobalDiscount_Money_PCT, faction, num, null);
				num /= (TIGlobalValuesState.Customizations.usingCustomizations ? TIGlobalValuesState.Customizations.nationalIPMultiplier : 1f);
				num = (float)Mathf.RoundToInt(num);
				if (num > 0f)
				{
					tiresourcesCost.AddCost(FactionResource.Money, num, true);
				}
			}
			if (num2 > 0f)
			{
				if (this.SkipDirectInvestInfluenceCost(faction) && num > 0f)
				{
					num2 *= 0.5f;
				}
				num2 *= 1f - TemplateManager.global.maxInvestmentPointDiscountfromControlPoints * this.CouncilControlPointFraction(faction, false, false);
				if (priority != PriorityType.Spoils)
				{
					num2 *= 1f + corruption * 0.75f;
					if (priority != PriorityType.Funding && this.ControlPointMaintenanceCost > 0f)
					{
						num2 *= this.ControlPointMaintenanceCost * 0.1f;
					}
				}
				num2 *= 1f - TIEffectsState.SumEffectsModifiers(Context.DirectInvestGlobalDiscount_Influence_PCT, faction, num2, null);
				num2 /= (TIGlobalValuesState.Customizations.usingCustomizations ? TIGlobalValuesState.Customizations.nationalIPMultiplier : 1f);
				num2 = (float)Mathf.RoundToInt(num2);
				if (num2 > 0f)
				{
					tiresourcesCost.AddCost(FactionResource.Influence, num2, true);
				}
			}
			if (num3 > 0f)
			{
				num3 *= 1f - TIEffectsState.SumEffectsModifiers(Context.DirectInvestGlobalDiscount_Ops_PCT, faction, num3, null);
				num3 /= (TIGlobalValuesState.Customizations.usingCustomizations ? TIGlobalValuesState.Customizations.nationalIPMultiplier : 1f);
				num3 = (float)Mathf.RoundToInt(num3);
				if (num3 > 0f)
				{
					tiresourcesCost.AddCost(FactionResource.Operations, num3, true);
				}
			}
			tiresourcesCost = tiresourcesCost.MultiplyCost(1f / (1f + faction.cachedPriorityBonuses[priority]));
			return tiresourcesCost.MultiplyCost(1f / (1f + this.NationalPriorityBonuses(priority)));
		}

		// Token: 0x06003858 RID: 14424 RVA: 0x0014717D File Offset: 0x0014537D
		public TIResourcesCost SingleDirectInvestmentPrice(PriorityType priority, int IPs, TIFactionState faction)
		{
			return new TIResourcesCost(this.InvestmentPointDirectPurchasePrice(priority, faction)).MultiplyCost((float)IPs);
		}

		// Token: 0x06003859 RID: 14425 RVA: 0x00147194 File Offset: 0x00145394
		public PriorityType GetRandomPriorityToDamage()
		{
			List<PriorityType> list = new List<PriorityType>();
			foreach (object obj in Enum.GetValues(typeof(PriorityType)))
			{
				PriorityType priorityType = (PriorityType)obj;
				if (priorityType != PriorityType.Unity && priorityType != PriorityType.Spoils && this.GetAccumulatedInvestmentPoints(priorityType) > 0f)
				{
					list.Add(priorityType);
				}
			}
			return list.SelectRandomItem<PriorityType>();
		}

		// Token: 0x0600385A RID: 14426 RVA: 0x00147218 File Offset: 0x00145418
		public void ModifyAccumulatedInvestment(PriorityType priority, float by, bool multiply, bool triggerUpdate)
		{
			if (multiply)
			{
				this.SetAccumulatedInvestmentPoints(priority, this._accumulatedInvestmentPoints[priority] * by, triggerUpdate);
			}
			else
			{
				this.SetAccumulatedInvestmentPoints(priority, this._accumulatedInvestmentPoints[priority] + by, triggerUpdate);
			}
			if (!this.ValidPriority(priority))
			{
				this.SetAccumulatedInvestmentPoints(priority, Mathf.Clamp(this._accumulatedInvestmentPoints[priority], 0f, this.GetRequiredInvestmentPointsForPriority(priority) - 1f), triggerUpdate);
				return;
			}
			this.SetAccumulatedInvestmentPoints(priority, Mathf.Max(0f, this._accumulatedInvestmentPoints[priority]), triggerUpdate);
		}

		// Token: 0x0600385B RID: 14427 RVA: 0x001472AD File Offset: 0x001454AD
		public void ModifyAccumulatedInvestmentFractional(PriorityType priority, float fraction, bool triggerUpdate)
		{
			this.ModifyAccumulatedInvestment(priority, fraction * this.GetRequiredInvestmentPointsForPriority(priority), false, triggerUpdate);
		}

		// Token: 0x0600385C RID: 14428 RVA: 0x001472C4 File Offset: 0x001454C4
		public void ProcessPrioritySpending()
		{
			bool flag = false;
			for (int i = 0; i < Enums.PriorityTypes.Length; i++)
			{
				PriorityType priorityType = Enums.PriorityTypes[i];
				float num = this.GetRequiredInvestmentPointsForPriority(priorityType);
				while (this._accumulatedInvestmentPoints[priorityType] >= num && this.ValidPriority(priorityType))
				{
					switch (priorityType)
					{
					case PriorityType.Economy:
						this.OnEconomyPriorityComplete();
						break;
					case PriorityType.Welfare:
						this.OnWelfarePriorityComplete();
						break;
					case PriorityType.Environment:
						this.OnEnvironmentPriorityComplete();
						break;
					case PriorityType.Knowledge:
						this.OnKnowledgePriorityComplete();
						break;
					case PriorityType.Government:
						this.OnGovernmentPriorityComplete();
						break;
					case PriorityType.Unity:
						this.OnUnityPriorityComplete();
						break;
					case PriorityType.Oppression:
						this.OnOppressionPriorityComplete();
						break;
					case PriorityType.Funding:
						this.OnFundingPriorityComplete();
						break;
					case PriorityType.Spoils:
						this.OnSpoilsPriorityComplete();
						break;
					case PriorityType.Civilian_InitiateSpaceflightProgram:
						this.OnSpaceFlightProgramPriorityComplete();
						break;
					case PriorityType.LaunchFacilities:
						this.OnBoostPriorityComplete();
						break;
					case PriorityType.MissionControl:
						this.OnMissionControlPriorityComplete();
						break;
					case PriorityType.Military_FoundMilitary:
						this.OnFoundMilitaryPriorityComplete();
						break;
					case PriorityType.Military:
						this.OnMilitaryPriorityComplete();
						break;
					case PriorityType.Military_BuildArmy:
						this.OnBuildArmyPriorityComplete();
						break;
					case PriorityType.Military_BuildNavy:
						this.OnBuildSealiftPriorityComplete();
						break;
					case PriorityType.Military_InitiateNuclearProgram:
						this.OnInitiateNuclearProgramComplete();
						break;
					case PriorityType.Military_BuildNuclearWeapons:
						this.OnBuildNuclearWeaponsPriorityComplete();
						break;
					case PriorityType.Military_BuildSpaceDefenses:
						this.OnBuildSpaceDefensesPriorityComplete();
						break;
					case PriorityType.Military_BuildSTOSquadron:
						this.OnBuildSTOSquadronPriorityComplete();
						break;
					}
					this.ModifyAccumulatedInvestment(priorityType, -num, false, false);
					num = this.GetRequiredInvestmentPointsForPriority(priorityType);
					flag = flag || !this.ValidPriority(priorityType);
					this.SetDataDirty();
				}
			}
			if (flag)
			{
				this.PossiblePriorityValidationChange(true);
			}
		}

		// Token: 0x170009C5 RID: 2501
		// (get) Token: 0x0600385D RID: 14429 RVA: 0x00147452 File Offset: 0x00145652
		// (set) Token: 0x0600385E RID: 14430 RVA: 0x0014745A File Offset: 0x0014565A
		public float priorityEffectPopScaling { get; private set; }

		// Token: 0x0600385F RID: 14431 RVA: 0x00147464 File Offset: 0x00145664
		public void SetPriorityEffectPopScaling()
		{
			if (this.extant && this.population > 0f)
			{
				this.priorityEffectPopScaling = Mathf.Pow(this.population / 50000000f, TIGlobalConfig.globalConfig.populationBasedIPEffectScaling);
				return;
			}
			this.priorityEffectPopScaling = 0f;
		}

		// Token: 0x170009C6 RID: 2502
		// (get) Token: 0x06003860 RID: 14432 RVA: 0x001474B4 File Offset: 0x001456B4
		public float economyPriorityPerCapitaIncomeChange
		{
			get
			{
				return (TemplateManager.global.economyPriorityPerCapitaIncomeChange_base + TIEffectsState.SumEffectsModifiers(Context.Economy_BasePCGDPIncrease, this, TemplateManager.global.economyPriorityPerCapitaIncomeChange_base, null) + (float)this.currentResourceRegions * (TemplateManager.global.economyPriorityPerCapitaIncomeChange_perResourceRegion + TIEffectsState.SumEffectsModifiers(Context.Economy_ResourcePCGDPMultiplier, this, TemplateManager.global.economyPriorityPerCapitaIncomeChange_perResourceRegion, null)) + (float)this.numCoreEconomicRegions_dailyCache * (TemplateManager.global.economyPriorityPerCapitaIncomeChange_perCoreEcoRegion + TIEffectsState.SumEffectsModifiers(Context.Economy_CoreEcoPCGDPMultiplier, this, TemplateManager.global.economyPriorityPerCapitaIncomeChange_perCoreEcoRegion, null)) + this.democracy * 0.5f + this.education) * this.priorityEffectPopScaling;
			}
		}

		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06003861 RID: 14433 RVA: 0x00147548 File Offset: 0x00145748
		public float economyPriorityInequalityChange
		{
			get
			{
				return (TemplateManager.global.economyPriorityInequalityIncrease + TemplateManager.global.economyPriorityInequalityIncrease_perResourceRegion * (float)this.currentResourceRegions) * this.priorityEffectPopScaling;
			}
		}

		// Token: 0x06003862 RID: 14434 RVA: 0x00147570 File Offset: 0x00145770
		public void OnEconomyPriorityComplete()
		{
			this.ModifyGDP((double)(this.economyPriorityPerCapitaIncomeChange * this.population_Millions * 1000000f), TINationState.GDPChangeReason.GDPReason_EconomyPriority);
			this.AddToInequality(this.economyPriorityInequalityChange + TIEffectsState.SumEffectsModifiers(Context.Economy_InequalityMultiplier, this, this.economyPriorityInequalityChange, null), TINationState.InequalityChangeReason.InqReason_EconomyPriority);
			TIGlobalValuesState.GlobalValues.ModifyMarketValuesForEconomyPriority();
			if (this.canAccumulateCoreOilTriggers)
			{
				TIRegionState nextCoreOilRegion = this.GetNextCoreOilRegion();
				if (nextCoreOilRegion != null)
				{
					nextCoreOilRegion.accumulatedCoreOilRegionTriggers++;
					if (nextCoreOilRegion.accumulatedCoreOilRegionTriggers >= TIGlobalConfig.globalConfig.numEcosForCoreOilRegion)
					{
						this.OnCoreOilRegionPriorityComplete(nextCoreOilRegion);
						return;
					}
				}
			}
			else if (this.canAccumulateCoreMiningTriggers)
			{
				TIRegionState nextCoreMiningRegion = this.GetNextCoreMiningRegion();
				if (nextCoreMiningRegion != null)
				{
					nextCoreMiningRegion.accumulatedCoreMiningRegionTriggers++;
					if (nextCoreMiningRegion.accumulatedCoreMiningRegionTriggers >= TIGlobalConfig.globalConfig.numEcosForCoreMiningRegion)
					{
						this.OnCoreMiningRegionComplete(nextCoreMiningRegion);
						return;
					}
				}
			}
			else if (this.canAccumulateCoreEconomyTriggers)
			{
				TIRegionState nextCoreEcoRegion = this.GetNextCoreEcoRegion();
				if (nextCoreEcoRegion != null)
				{
					nextCoreEcoRegion.accumulatedCoreEconomyRegionTriggers++;
					if (nextCoreEcoRegion.accumulatedCoreEconomyRegionTriggers >= TIGlobalConfig.globalConfig.numEcosForCoreEcoRegion)
					{
						this.OnCoreEconomicRegionPriorityComplete(nextCoreEcoRegion);
					}
				}
			}
		}

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x06003863 RID: 14435 RVA: 0x00147686 File Offset: 0x00145886
		public float welfarePriorityInequalityChange
		{
			get
			{
				return (TemplateManager.global.welfarePriorityInequalityChange + TIEffectsState.SumEffectsModifiers(Context.WelfareInequalityReductionBonus, this, TemplateManager.global.welfarePriorityInequalityChange, null)) * this.priorityEffectPopScaling;
			}
		}

		// Token: 0x06003864 RID: 14436 RVA: 0x001476B0 File Offset: 0x001458B0
		public void OnWelfarePriorityComplete()
		{
			this.AddToInequality(this.welfarePriorityInequalityChange, TINationState.InequalityChangeReason.InqReason_WelfarePriority);
			if (this.canAccumulateDecolonizeTriggers)
			{
				TIRegionState nextDecolonizeRegion = this.GetNextDecolonizeRegion();
				if (nextDecolonizeRegion != null)
				{
					nextDecolonizeRegion.accumulatedDecolonizeTriggers++;
					if (nextDecolonizeRegion.accumulatedDecolonizeTriggers >= 1000)
					{
						this.OnDecolonizeRegionPriorityComplete(nextDecolonizeRegion);
					}
				}
			}
		}

		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x06003865 RID: 14437 RVA: 0x00147708 File Offset: 0x00145908
		public float environmentPrioritySustainabilityChange
		{
			get
			{
				float num = this.priorityEffectPopScaling * (TemplateManager.global.environmentPrioritySustainabilityChange + TIEffectsState.SumEffectsModifiers(Context.Environment_SustainabilityChange, this, TemplateManager.global.environmentPrioritySustainabilityChange, null));
				float sustainability = this.sustainability;
				float num2;
				if (sustainability > 2f)
				{
					num2 = this.sustainability / 2f;
				}
				else if (sustainability < 0.5f)
				{
					num2 = Mathf.Max(0.25f, this.sustainability / 0.5f);
				}
				else
				{
					num2 = 1f;
				}
				int num3 = this.regions.Sum<TIRegionState>((TIRegionState x) => x.nuclearDetonations);
				if (num3 > 0)
				{
					num /= (float)num3;
				}
				return num * num2;
			}
		}

		// Token: 0x06003866 RID: 14438 RVA: 0x001477B8 File Offset: 0x001459B8
		public float OneStepGHGReduction(int which = 0)
		{
			double num;
			double num2;
			switch (which)
			{
			default:
				num = this.GHGsFromEconomy_tons(false, 0f).Item1;
				num2 = this.GHGsFromEconomy_tons(false, this.environmentPrioritySustainabilityChange).Item1;
				break;
			case 1:
				num = this.GHGsFromEconomy_tons(false, 0f).Item2;
				num2 = this.GHGsFromEconomy_tons(false, this.environmentPrioritySustainabilityChange).Item2;
				break;
			case 2:
				num = this.GHGsFromEconomy_tons(false, 0f).Item3;
				num2 = this.GHGsFromEconomy_tons(false, this.environmentPrioritySustainabilityChange).Item3;
				break;
			}
			return (float)((num2 - num) / num);
		}

		// Token: 0x06003867 RID: 14439 RVA: 0x00147850 File Offset: 0x00145A50
		public float EnvPriorityCO2Removed()
		{
			return (TemplateManager.global.WelCO2_ppm + TIEffectsState.SumEffectsModifiers(Context.Welfare_CO2_ppm, this, TemplateManager.global.WelCO2_ppm, null)) / this.priorityEffectPopScaling;
		}

		// Token: 0x06003868 RID: 14440 RVA: 0x00147877 File Offset: 0x00145A77
		public float EnvPriorityCH4Removed()
		{
			return (TemplateManager.global.WelCH4_ppm + TIEffectsState.SumEffectsModifiers(Context.Welfare_CH4_ppm, this, TemplateManager.global.WelCH4_ppm, null)) / this.priorityEffectPopScaling;
		}

		// Token: 0x06003869 RID: 14441 RVA: 0x0014789E File Offset: 0x00145A9E
		public float EnvPriorityN2ORemoved()
		{
			return (TemplateManager.global.WelN2O_ppm + TIEffectsState.SumEffectsModifiers(Context.Welfare_N2O_ppm, this, TemplateManager.global.WelN2O_ppm, null)) / this.priorityEffectPopScaling;
		}

		// Token: 0x0600386A RID: 14442 RVA: 0x001478C8 File Offset: 0x00145AC8
		public void OnEnvironmentPriorityComplete()
		{
			if (this.sustainability <= 0f)
			{
				TIGlobalValuesState.GlobalValues.AddEnvironmentPriorityEnvEffect(this);
			}
			else
			{
				this.AddToSustainability(this.environmentPrioritySustainabilityChange);
			}
			if (this.canAccumulateDecontaminateTriggers)
			{
				TIRegionState nextDecontaminateRegion = this.GetNextDecontaminateRegion();
				if (nextDecontaminateRegion != null)
				{
					nextDecontaminateRegion.accumulatedDecontaminateTriggers++;
					if (nextDecontaminateRegion.accumulatedDecontaminateTriggers > 100)
					{
						this.OnDecontaminateRegionPriorityComplete(nextDecontaminateRegion);
					}
				}
			}
		}

		// Token: 0x170009CA RID: 2506
		// (get) Token: 0x0600386B RID: 14443 RVA: 0x00147933 File Offset: 0x00145B33
		public float knowledgePriorityCohesionChange
		{
			get
			{
				return this.priorityEffectPopScaling * ((this.cohesion < 5f) ? 0.01f : ((this.cohesion > 5f) ? (-0.01f) : 0f));
			}
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x0600386C RID: 14444 RVA: 0x0014796C File Offset: 0x00145B6C
		public float knowledgePriorityEducationChange
		{
			get
			{
				float education = this.education;
				if (education < 8.5f)
				{
					return this.priorityEffectPopScaling * (8.5f / Mathf.Max(1f, this.education)) * TemplateManager.global.knowledgePriorityEducationIncrease;
				}
				if (education >= 12f)
				{
					return this.priorityEffectPopScaling * (12f / Mathf.Max(1f, this.education)) * TemplateManager.global.knowledgePriorityEducationIncrease;
				}
				return this.priorityEffectPopScaling * TemplateManager.global.knowledgePriorityEducationIncrease;
			}
		}

		// Token: 0x0600386D RID: 14445 RVA: 0x001479F3 File Offset: 0x00145BF3
		public void OnKnowledgePriorityComplete()
		{
			this.AddToEducation(this.knowledgePriorityEducationChange, TINationState.EducationChangeReason.EducationReason_KnowledgePriority);
			this.AddToCohesion(this.knowledgePriorityCohesionChange, TINationState.CohesionChangeReason.CohesionReason_KnowledgePriority);
		}

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x0600386E RID: 14446 RVA: 0x00147A10 File Offset: 0x00145C10
		public float governmentPriorityDemocracyChange
		{
			get
			{
				return this.priorityEffectPopScaling * TemplateManager.global.governmentPriorityDemocracyIncrease * (this.education / 10f);
			}
		}

		// Token: 0x0600386F RID: 14447 RVA: 0x00147A30 File Offset: 0x00145C30
		public void OnGovernmentPriorityComplete()
		{
			if (this.democracy >= 10f)
			{
				this.OnKnowledgePriorityComplete();
			}
			else
			{
				this.AddToDemocracy(this.governmentPriorityDemocracyChange, TINationState.DemocracyChangeReason.DemReason_GovernmentPriority);
			}
			if (this.canAccumulateLegitimizeClaimTriggers)
			{
				this.accumulatedLegitimizeClaimTriggers += 1f;
				if (this.accumulatedLegitimizeClaimTriggers >= (float)TIGlobalConfig.globalConfig.numPrioritiesForLegitimize && this.CandidateLegitimizeClaimRegions().Count > 0)
				{
					this.OnLegitimizeClaimPriorityComplete();
					this.accumulatedLegitimizeClaimTriggers = 0f;
				}
			}
		}

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06003870 RID: 14448 RVA: 0x00147AAC File Offset: 0x00145CAC
		public float unityPriorityCohesionChange
		{
			get
			{
				return this.priorityEffectPopScaling * Mathf.Clamp(TemplateManager.global.unityBaseCohesionChange - TemplateManager.global.unityBaseCohesionChange * (0.05f * (this.education + this.democracy)), TemplateManager.global.unityMinCohesionChange, TemplateManager.global.unityBaseCohesionChange);
			}
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x06003871 RID: 14449 RVA: 0x00147B02 File Offset: 0x00145D02
		public float unityPriorityEducationChange
		{
			get
			{
				return this.priorityEffectPopScaling * TemplateManager.global.unityPriorityEducationChange;
			}
		}

		// Token: 0x06003872 RID: 14450 RVA: 0x00147B18 File Offset: 0x00145D18
		public void OnUnityPriorityComplete()
		{
			TIFactionState controlPointOfTypeFaction = this.GetControlPointOfTypeFaction(ControlPointType.Religion);
			foreach (TIFactionState tifactionState in this.FactionsWithControlPoint)
			{
				this.PropagandaOnPop_PerOwnedCP(tifactionState.ideology, TemplateManager.global.unityPublicOpinionBaseStrength * this.priorityEffectPopScaling, (controlPointOfTypeFaction == tifactionState) ? TemplateManager.global.religionUnityPublicOpinionBonusStrength : 0, false);
			}
			this.AddToCohesion(this.unityPriorityCohesionChange, TINationState.CohesionChangeReason.CohesionReason_UnityPriority);
			this.AddToEducation(this.unityPriorityEducationChange, TINationState.EducationChangeReason.EducationReason_UnityPriority);
			if (this.canAccumulateLegitimizeClaimTriggers)
			{
				this.accumulatedLegitimizeClaimTriggers += 1f;
				if (this.accumulatedLegitimizeClaimTriggers >= (float)TIGlobalConfig.globalConfig.numPrioritiesForLegitimize && this.CandidateLegitimizeClaimRegions().Count > 0)
				{
					this.OnLegitimizeClaimPriorityComplete();
					this.accumulatedLegitimizeClaimTriggers = 0f;
				}
			}
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x06003873 RID: 14451 RVA: 0x00147C0C File Offset: 0x00145E0C
		public float militaryPriorityTechLevelChange
		{
			get
			{
				return TemplateManager.global.militaryPriorityMiltechIncrease * (TIGlobalValuesState.GlobalValues.bestGlobalHumanMiltech / Mathf.Max(1f, this.militaryTechLevel));
			}
		}

		// Token: 0x06003874 RID: 14452 RVA: 0x00147C34 File Offset: 0x00145E34
		public void OnMilitaryPriorityComplete()
		{
			this.AddToMilitaryTechLevel(this.militaryPriorityTechLevelChange);
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x06003875 RID: 14453 RVA: 0x00147C42 File Offset: 0x00145E42
		public float OppressionPriorityUnrestChange
		{
			get
			{
				return this.priorityEffectPopScaling * -Mathf.Min(this.unrest, 0.1f - this.democracy / 100f) * TemplateManager.global.oppressionPriorityUnrestMultiplier;
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x06003876 RID: 14454 RVA: 0x00147C74 File Offset: 0x00145E74
		public float OppressionPriorityDemocracyChange
		{
			get
			{
				return this.priorityEffectPopScaling * TemplateManager.global.oppressionPriorityDemocracyDecrease;
			}
		}

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06003877 RID: 14455 RVA: 0x00147C87 File Offset: 0x00145E87
		public float OppressionPriorityCohesionChange
		{
			get
			{
				if (this.democracy > 5f)
				{
					return this.priorityEffectPopScaling * (this.democracy - 5f) * TemplateManager.global.conditionalOppressionPriorityCohesionDecrease;
				}
				return 0f;
			}
		}

		// Token: 0x06003878 RID: 14456 RVA: 0x00147CBA File Offset: 0x00145EBA
		public void OnOppressionPriorityComplete()
		{
			this.AddToUnrest(this.OppressionPriorityUnrestChange, TINationState.UnrestChangeReason.UnrestReason_OppressionPriority, 10f);
			this.AddToDemocracy(this.OppressionPriorityDemocracyChange, TINationState.DemocracyChangeReason.DemReason_OppressionPriority);
			this.AddToCohesion(this.OppressionPriorityCohesionChange, TINationState.CohesionChangeReason.CohesionReason_OppressionPriority);
		}

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06003879 RID: 14457 RVA: 0x00147CE9 File Offset: 0x00145EE9
		public float spoilsPriorityMoney
		{
			get
			{
				return TemplateManager.global.spoilsPriorityMoneyPerInvestmentPoint * this.BaseInvestmentPoints_month() + TemplateManager.global.spoilsPriorityMoneyPerResourceRegion * (float)this.currentResourceRegions + TemplateManager.global.spoilsDemocracyMoneyModifier * (10f - this.democracy);
			}
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x0600387A RID: 14458 RVA: 0x00147D27 File Offset: 0x00145F27
		public float spoilsPriorityMoneyPerControlPoint
		{
			get
			{
				return this.spoilsPriorityMoney / (float)this.numControlPoints;
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x0600387B RID: 14459 RVA: 0x00147D37 File Offset: 0x00145F37
		public float spoilsPriorityInequalityChange
		{
			get
			{
				return this.priorityEffectPopScaling * (TemplateManager.global.spoilsPriorityBaseInequalityChange + TemplateManager.global.spoilsPriorityInequalityChange_perResourceRegion * (float)this.currentResourceRegions);
			}
		}

		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x0600387C RID: 14460 RVA: 0x00147D5D File Offset: 0x00145F5D
		public float spoilsPriorityDemocracyChange
		{
			get
			{
				return this.priorityEffectPopScaling * TemplateManager.global.spoilsPriorityDemocracyChange;
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x0600387D RID: 14461 RVA: 0x00147D70 File Offset: 0x00145F70
		public float spoilsSustainabilityChange
		{
			get
			{
				return this.priorityEffectPopScaling * (TemplateManager.global.spoilsPrioritySustainabilityChange + TemplateManager.global.spoilsPrioritySustainabilityChange_perResourceRegion * (float)this.currentResourceRegions);
			}
		}

		// Token: 0x0600387E RID: 14462 RVA: 0x00147D98 File Offset: 0x00145F98
		private void OnSpoilsPriorityComplete()
		{
			this.AddToInequality(this.spoilsPriorityInequalityChange, TINationState.InequalityChangeReason.InqReason_SpoilsPriority);
			this.AddToDemocracy(this.spoilsPriorityDemocracyChange, TINationState.DemocracyChangeReason.DemReason_SpoilsPriority);
			TIFactionState controlPointTypeOwner = this.GetControlPointTypeOwner(ControlPointType.Aristocracy);
			TIFactionState controlPointTypeOwner2 = this.GetControlPointTypeOwner(ControlPointType.ExtractiveSector);
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				float num = this.spoilsPriorityMoneyPerControlPoint * ((controlPointTypeOwner == ticontrolPoint.faction) ? TemplateManager.global.aristoracySpoilsMult : 1f) + ((controlPointTypeOwner2 == ticontrolPoint.faction) ? (TemplateManager.global.extractiveSpoilsBonusPerResourceRegion * (float)this.currentResourceRegions) : 0f);
				num += TIEffectsState.SumEffectsModifiers(Context.SpoilsOutput, ticontrolPoint.faction, num, null);
				if (ticontrolPoint.faction != null && !ticontrolPoint.benefitsDisabled)
				{
					ticontrolPoint.faction.AddToCurrentResource(num, FactionResource.Money, false, "Spoils");
					ticontrolPoint.faction.thisWeeksCumulativeSpoils += num;
					ticontrolPoint.faction.thisMonthsCumulativeSpoils += num;
				}
			}
			float num2 = (this.education + this.democracy) * TIGlobalConfig.globalConfig.spoilsPriorityPublicOpinionScaling;
			foreach (TIFactionState tifactionState in this.FactionsWithControlPoint)
			{
				this.PropagandaOnPop_PerOwnedCPFraction(tifactionState.ideology, num2);
			}
			this.AddToSustainability(this.spoilsSustainabilityChange);
			TIGlobalValuesState.GlobalValues.AddSpoilsPriorityEnvEffect(this, this.priorityEffectPopScaling * this.sustainability);
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x0600387F RID: 14463 RVA: 0x00147F60 File Offset: 0x00146160
		public float maxFunding_year
		{
			get
			{
				return (float)(0.004999999888241291 * (this.GDP / 1000000.0));
			}
		}

		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06003880 RID: 14464 RVA: 0x00147F7D File Offset: 0x0014617D
		public float spaceFundingPriorityIncomeChange
		{
			get
			{
				return TemplateManager.global.fundingPriorityBaseIncomeIncrease + (float)this.numControlPoints_unclamped;
			}
		}

		// Token: 0x06003881 RID: 14465 RVA: 0x00147F91 File Offset: 0x00146191
		public void OnFundingPriorityComplete()
		{
			this.ChangeAnnualSpaceFundingValue(this.spaceFundingPriorityIncomeChange);
		}

		// Token: 0x06003882 RID: 14466 RVA: 0x00147FA0 File Offset: 0x001461A0
		public void ChangeAnnualSpaceFundingValue(float change)
		{
			this.spaceFunding_year += change;
			this.spaceFunding_year = Mathf.Clamp(this.spaceFunding_year, 0f, this.maxFunding_year);
			if (!this.inFederation)
			{
				this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.Money);
				});
				return;
			}
			TIFederationState tifederationState = this.federation;
			if (tifederationState == null)
			{
				return;
			}
			tifederationState.ref_factions.ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Money);
			});
		}

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06003883 RID: 14467 RVA: 0x0014803E File Offset: 0x0014623E
		public float spaceflightInitialBoost
		{
			get
			{
				return 1f * TemplateManager.global.spaceResourceToTons;
			}
		}

		// Token: 0x06003884 RID: 14468 RVA: 0x00148050 File Offset: 0x00146250
		public float BoostIncrease(float boostLatitude)
		{
			return (TemplateManager.global.boostPriorityIncreaseAtEquator - Mathf.Abs(boostLatitude / TemplateManager.global.boostLatitudeDivisor)) * TemplateManager.global.spaceResourceToTons;
		}

		// Token: 0x06003885 RID: 14469 RVA: 0x00148079 File Offset: 0x00146279
		public float BoostGainLow()
		{
			return this.BoostIncrease(this.regions.Max<TIRegionState>((TIRegionState x) => Mathf.Abs(x.boostLatitude)));
		}

		// Token: 0x06003886 RID: 14470 RVA: 0x001480AB File Offset: 0x001462AB
		public float BoostGainHigh()
		{
			return this.BoostIncrease(this.regions.Min<TIRegionState>((TIRegionState x) => Mathf.Abs(x.boostLatitude)));
		}

		// Token: 0x170009DB RID: 2523
		// (get) Token: 0x06003887 RID: 14471 RVA: 0x001480DD File Offset: 0x001462DD
		public float BestBoostLatitude
		{
			get
			{
				return this.regions.MinBy<TIRegionState, float>((TIRegionState x) => Mathf.Abs(x.boostLatitude)).boostLatitude;
			}
		}

		// Token: 0x06003888 RID: 14472 RVA: 0x00148110 File Offset: 0x00146310
		public void OnBoostPriorityComplete()
		{
			TIRegionState tiregionState;
			if (!this.regions.Any<TIRegionState>((TIRegionState x) => x.boostPerYear_dekatons > 0f))
			{
				List<TIRegionState> list = new List<TIRegionState>(this.regions);
				if (list.Any<TIRegionState>((TIRegionState x) => x.NoOccupationUnderwayOrComplete()))
				{
					list = list.Where<TIRegionState>((TIRegionState x) => x.NoOccupationUnderwayOrComplete()).ToList<TIRegionState>();
				}
				float min = this.regions.Min<TIRegionState>((TIRegionState x) => Mathf.Abs(x.boostLatitude));
				list = this.regions.Where<TIRegionState>((TIRegionState x) => Mathf.Abs(x.boostLatitude) == min).ToList<TIRegionState>();
				if (list.Any<TIRegionState>((TIRegionState x) => x.isCoastal))
				{
					list = list.Where<TIRegionState>((TIRegionState x) => x.isCoastal).ToList<TIRegionState>();
				}
				if (list.Any<TIRegionState>((TIRegionState x) => !x.colonyRegion))
				{
					list = list.Where<TIRegionState>((TIRegionState x) => !x.colonyRegion).ToList<TIRegionState>();
				}
				tiregionState = list.MaxBy<TIRegionState, float>((TIRegionState x) => x.longitude);
			}
			else
			{
				Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
				List<TIRegionState> list2 = new List<TIRegionState>(this.regions);
				if (list2.Any<TIRegionState>((TIRegionState x) => x.NoOccupationUnderwayOrComplete()))
				{
					list2 = list2.Where<TIRegionState>((TIRegionState x) => x.NoOccupationUnderwayOrComplete()).ToList<TIRegionState>();
				}
				foreach (TIRegionState tiregionState2 in this.regions)
				{
					float num = tiregionState2.boostPerMonth_dekatons * 500000f;
					num += Mathf.Sqrt(90.1f - Mathf.Abs(tiregionState2.boostLatitude));
					dictionary[tiregionState2] = num;
				}
				tiregionState = dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> j) => j.Value, -1f, 1E-37f).Key;
			}
			tiregionState.ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, this.BoostIncrease(tiregionState.boostLatitude), false, false);
		}

		// Token: 0x06003889 RID: 14473 RVA: 0x001483FC File Offset: 0x001465FC
		private void OnMissionControlPriorityComplete()
		{
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			foreach (TIRegionState tiregionState in this.regions.Where<TIRegionState>((TIRegionState x) => x.missionControl < x.maxMissionControl))
			{
				if (!tiregionState.IsFullyOccupied())
				{
					float num = 1f;
					if (!tiregionState.colonyRegion)
					{
						num += 1f;
					}
					if (this.capital == tiregionState)
					{
						num += 1f;
					}
					if (tiregionState.coreEconomicRegion)
					{
						num += 1f;
					}
					num += (float)this.missionControl * (float)this.missionControl * (float)this.missionControl;
					if (num > 0f)
					{
						dictionary.Add(tiregionState, num);
					}
				}
			}
			if (dictionary.Count == 0)
			{
				this.controlPoints.ForEach(delegate(TIControlPoint x)
				{
					x.SetControlPointPriority(PriorityType.MissionControl, 0, false, false, false);
				});
				Log.Error("Mission Control finished but no place to put it", Array.Empty<object>());
				return;
			}
			dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> j) => j.Value, -1f, 1E-37f).Key.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, 1f, false, false);
		}

		// Token: 0x0600388A RID: 14474 RVA: 0x00148564 File Offset: 0x00146764
		private void OnSpaceFlightProgramPriorityComplete()
		{
			this.GrantSpaceFlightProgram();
		}

		// Token: 0x0600388B RID: 14475 RVA: 0x0014856C File Offset: 0x0014676C
		public void GrantSpaceFlightProgram()
		{
			if (this.spaceFlightProgram)
			{
				return;
			}
			new Dictionary<TIRegionState, float>();
			List<TIRegionState> list = new List<TIRegionState>(this.regions);
			if (list.Any<TIRegionState>((TIRegionState x) => x.NoOccupationUnderwayOrComplete()))
			{
				list = list.Where<TIRegionState>((TIRegionState x) => x.NoOccupationUnderwayOrComplete()).ToList<TIRegionState>();
			}
			float min = this.regions.Min<TIRegionState>((TIRegionState x) => Mathf.Abs(x.boostLatitude));
			list = this.regions.Where<TIRegionState>((TIRegionState x) => Mathf.Abs(x.boostLatitude) == min).ToList<TIRegionState>();
			if (list.Any<TIRegionState>((TIRegionState x) => x.isCoastal))
			{
				list = list.Where<TIRegionState>((TIRegionState x) => x.isCoastal).ToList<TIRegionState>();
			}
			if (list.Any<TIRegionState>((TIRegionState x) => !x.colonyRegion))
			{
				list = list.Where<TIRegionState>((TIRegionState x) => !x.colonyRegion).ToList<TIRegionState>();
			}
			list.MaxBy<TIRegionState, float>((TIRegionState x) => x.longitude).ChangeSpaceFacilityValue(SpaceFacilityType.launchFacility, this.spaceflightInitialBoost, false, false);
			this.spaceFlightProgram = true;
			if (this.inFederation)
			{
				this.federation.SetSpaceProgramValue();
			}
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				int controlPointPriority = ticontrolPoint.GetControlPointPriority(PriorityType.Civilian_InitiateSpaceflightProgram, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.Civilian_InitiateSpaceflightProgram, 0, true, true, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.LaunchFacilities, controlPointPriority, true, true, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.MissionControl, controlPointPriority, false, false, false);
			}
			TINotificationQueueState.LogNationGainsSpaceProgram(this);
		}

		// Token: 0x0600388C RID: 14476 RVA: 0x001487A0 File Offset: 0x001469A0
		public int GetNextArmyControlPointIdx()
		{
			this.controlPoints = this.controlPoints.OrderBy<TIControlPoint, int>((TIControlPoint x) => x.positionInNation).ToList<TIControlPoint>();
			Dictionary<TIControlPoint, int> dictionary = new Dictionary<TIControlPoint, int>();
			int num = 0;
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				int numArmies = ticontrolPoint.numArmies;
				dictionary.Add(ticontrolPoint, numArmies);
				if (numArmies > num)
				{
					num = numArmies;
				}
			}
			int num2 = this.maxControlPointIndex;
			int num3 = num2;
			for (int i = this.maxControlPointIndex; i >= 0; i--)
			{
				int numArmies2 = this.controlPoints[i].numArmies;
				if (numArmies2 < num && numArmies2 < num3)
				{
					num2 = i;
					num3 = numArmies2;
				}
			}
			return Mathf.Clamp(num2, 0, this.maxControlPointIndex);
		}

		// Token: 0x0600388D RID: 14477 RVA: 0x00148898 File Offset: 0x00146A98
		public TIRegionState GetNextArmyRegion()
		{
			List<TIRegionState> list = new List<TIRegionState>();
			foreach (TIArmyState tiarmyState in this.armies)
			{
				list.Add(tiarmyState.homeRegion);
			}
			List<TIRegionState> list2 = new List<TIRegionState>();
			bool flag = false;
			foreach (TIRegionState tiregionState in this.regions)
			{
				if (!tiregionState.IsFullyOccupied() && !list.Contains(tiregionState) && !tiregionState.colonyRegion)
				{
					list2.Add(tiregionState);
					if (tiregionState.coreEconomicRegion)
					{
						flag = true;
					}
				}
			}
			List<TIRegionState> list3 = new List<TIRegionState>(list2);
			if (flag)
			{
				foreach (TIRegionState tiregionState2 in list2)
				{
					if (!tiregionState2.coreEconomicRegion)
					{
						list3.Remove(tiregionState2);
					}
				}
			}
			if (list3.Count > 0)
			{
				return list3.MaxBy<TIRegionState, float>((TIRegionState r) => r.populationInMillions);
			}
			return null;
		}

		// Token: 0x0600388E RID: 14478 RVA: 0x001489F4 File Offset: 0x00146BF4
		private bool OnBuildArmyPriorityComplete()
		{
			TIRegionState nextArmyRegion = this.GetNextArmyRegion();
			if (nextArmyRegion == null)
			{
				foreach (TIControlPoint ticontrolPoint in this.controlPoints)
				{
					Log.Error("Can't find region to locate new " + this.displayName + " army", Array.Empty<object>());
					ticontrolPoint.SetControlPointPriority(PriorityType.Military_BuildArmy, 0, false, false, false);
				}
				return false;
			}
			TIArmyState tiarmyState = GameStateManager.CreateNewGameState<TIArmyState>();
			tiarmyState.createdFromTemplate = false;
			tiarmyState.deploymentType = DeploymentType.Standard;
			tiarmyState.controlPointIdx = this.GetNextArmyControlPointIdx();
			tiarmyState.homeRegion = nextArmyRegion;
			tiarmyState.NewArmy(ArmyType.Human, 0, 1f);
			tiarmyState.MoveArmyToRegion(nextArmyRegion, true);
			this.AddArmy(tiarmyState);
			TINotificationQueueState.LogNewArmyBuilt(tiarmyState);
			this.SetDataDirty();
			TIGlobalValuesState.GlobalValues.ModifyMarketValuesForArmyPriority();
			tiarmyState.SetGameStateCreated();
			return true;
		}

		// Token: 0x0600388F RID: 14479 RVA: 0x00148ADC File Offset: 0x00146CDC
		public TIArmyState GetNextNavy()
		{
			List<TIArmyState> list = new List<TIArmyState>();
			int[] array = new int[this.numControlPoints];
			int[] array2 = new int[this.numControlPoints];
			if (!this.ValidPriority(PriorityType.Military_BuildNavy))
			{
				return null;
			}
			foreach (TIArmyState tiarmyState in this.armies)
			{
				if (tiarmyState.armyType == ArmyType.Human)
				{
					array[tiarmyState.controlPointIdx]++;
					switch (tiarmyState.deploymentType)
					{
					case DeploymentType.Standard:
						list.Add(tiarmyState);
						break;
					case DeploymentType.Naval:
						array2[tiarmyState.controlPointIdx]++;
						break;
					}
				}
			}
			int num = this.armies.Max<TIArmyState>((TIArmyState x) => x.controlPointIdx);
			array2.Max();
			for (int i = this.maxControlPointIndex; i >= 0; i--)
			{
				if (array[i] > array2[i])
				{
					num = i;
					break;
				}
			}
			foreach (TIArmyState tiarmyState2 in list)
			{
				if (tiarmyState2.controlPointIdx == num && tiarmyState2.deploymentType == DeploymentType.Standard)
				{
					return tiarmyState2;
				}
			}
			return null;
		}

		// Token: 0x06003890 RID: 14480 RVA: 0x00148C58 File Offset: 0x00146E58
		private void OnBuildSealiftPriorityComplete()
		{
			TIArmyState nextNavy = this.GetNextNavy();
			if (nextNavy != null)
			{
				nextNavy.AddNavy();
				TINotificationQueueState.LogNewNavyBuilt(nextNavy);
				TIGlobalValuesState.GlobalValues.ModifyMarketValuesForArmyPriority();
			}
		}

		// Token: 0x06003891 RID: 14481 RVA: 0x00148C8C File Offset: 0x00146E8C
		private void OnInitiateNuclearProgramComplete()
		{
			this.nuclearProgram = true;
			TIGlobalValuesState.GlobalValues.TriggerNuclearDetonationEffect(false, null, null, null);
			this.ChangeNumNuclearWeapons(1);
			foreach (TIFactionState tifactionState in this.FactionsWithControlPoint)
			{
				tifactionState.CommitAtrocity(1, TIFactionState.AtrocityCause.NuclearTesting, false, 0.333f);
			}
			TINotificationQueueState.LogNationGainsNukes(this);
			if (this.executiveFaction != null && this.executiveFaction.isActivePlayer)
			{
				this.executiveFaction.UnlockAchievement("completeNukeProgram");
			}
			TIGlobalValuesState.GlobalValues.ModifyMarketValuesForNuclearWeaponsPriority();
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				int controlPointPriority = ticontrolPoint.GetControlPointPriority(PriorityType.Military_InitiateNuclearProgram, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.Military_InitiateNuclearProgram, 0, true, true, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.Military_BuildNuclearWeapons, controlPointPriority, false, false, false);
			}
		}

		// Token: 0x06003892 RID: 14482 RVA: 0x00148D98 File Offset: 0x00146F98
		public void OnBuildNuclearWeaponsPriorityComplete()
		{
			this.ChangeNumNuclearWeapons(1);
			TINotificationQueueState.LogNationCompletesNuke(this);
			TIGlobalValuesState.GlobalValues.ModifyMarketValuesForNuclearWeaponsPriority();
		}

		// Token: 0x06003893 RID: 14483 RVA: 0x00148DB4 File Offset: 0x00146FB4
		public void ActivateBuildSpaceDefenses()
		{
			if (!this.canBuildSpaceDefenses)
			{
				this.canBuildSpaceDefenses = true;
				foreach (TIControlPoint ticontrolPoint in this.controlPoints)
				{
					ticontrolPoint.SetControlPointPriority(PriorityType.Military_BuildSpaceDefenses, ticontrolPoint.GetControlPointPriority(PriorityType.Military_BuildSpaceDefenses, true), false, false, false);
				}
			}
		}

		// Token: 0x06003894 RID: 14484 RVA: 0x00148E24 File Offset: 0x00147024
		public TIRegionState GetNextSpaceDefensesRegion()
		{
			if (!this.capital.antiSpaceDefenses)
			{
				return this.capital;
			}
			IEnumerable<TIRegionState> enumerable = this.regions.Where<TIRegionState>((TIRegionState x) => !x.antiSpaceDefenses);
			if (enumerable.Count<TIRegionState>() > 0)
			{
				if (enumerable.Any<TIRegionState>((TIRegionState x) => x.coreEconomicRegion))
				{
					enumerable = enumerable.Where<TIRegionState>((TIRegionState x) => x.coreEconomicRegion);
				}
				else if (enumerable.Any<TIRegionState>((TIRegionState x) => x.coreResourceRegion))
				{
					enumerable = enumerable.Where<TIRegionState>((TIRegionState x) => x.coreResourceRegion);
				}
				if (enumerable.OnlySome<TIRegionState>((TIRegionState x) => x.colonyRegion))
				{
					enumerable = enumerable.Where<TIRegionState>((TIRegionState x) => !x.colonyRegion);
				}
				return enumerable.MaxBy<TIRegionState, float>((TIRegionState x) => x.populationInMillions);
			}
			return null;
		}

		// Token: 0x170009DC RID: 2524
		// (get) Token: 0x06003895 RID: 14485 RVA: 0x00148F8C File Offset: 0x0014718C
		public int hasAntiSpaceDefenses
		{
			get
			{
				int num = 0;
				for (int i = 0; i < this.regions.Count; i++)
				{
					if (this.regions[i].antiSpaceDefenses)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x170009DD RID: 2525
		// (get) Token: 0x06003896 RID: 14486 RVA: 0x00148FCC File Offset: 0x001471CC
		public bool completeAntiSpaceDefenses
		{
			get
			{
				for (int i = 0; i < this.regions.Count; i++)
				{
					if (!this.regions[i].antiSpaceDefenses)
					{
						return false;
					}
				}
				return true;
			}
		}

		// Token: 0x06003897 RID: 14487 RVA: 0x00149005 File Offset: 0x00147205
		public void OnBuildSpaceDefensesPriorityComplete()
		{
			if (!this.completeAntiSpaceDefenses)
			{
				TIRegionState nextSpaceDefensesRegion = this.GetNextSpaceDefensesRegion();
				nextSpaceDefensesRegion.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, true, false);
				TINotificationQueueState.LogSpaceDefensesComplete(nextSpaceDefensesRegion.spaceDefenseFacility);
			}
		}

		// Token: 0x06003898 RID: 14488 RVA: 0x00149030 File Offset: 0x00147230
		public float MilitaryTechLevelOnFounding()
		{
			float[] array = new float[4];
			array[0] = 2f;
			int num = 1;
			float num2;
			if (!this.inFederation)
			{
				num2 = 0f;
			}
			else
			{
				num2 = this.federation.members.Min<TINationState>((TINationState x) => x.militaryTechLevel) - 0.3f;
			}
			array[num] = num2;
			int num3 = 2;
			float num4;
			if (this.allies.Count <= 0)
			{
				num4 = 0f;
			}
			else
			{
				num4 = this.allies.Min<TINationState>((TINationState x) => x.militaryTechLevel) - 0.3f;
			}
			array[num3] = num4;
			array[3] = GameStateManager.AllExtantHumanNations().Min<TINationState>((TINationState x) => x.militaryTechLevel);
			return Mathf.Max(array);
		}

		// Token: 0x06003899 RID: 14489 RVA: 0x0014910C File Offset: 0x0014730C
		public void OnFoundMilitaryPriorityComplete()
		{
			if (!this.military)
			{
				this.military = true;
				this.militaryTechLevel = this.MilitaryTechLevelOnFounding();
				TINotificationQueueState.LogMilitaryFounded(this);
			}
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				int controlPointPriority = ticontrolPoint.GetControlPointPriority(PriorityType.Military_FoundMilitary, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.Military_FoundMilitary, 0, true, true, false);
				ticontrolPoint.SetControlPointPriority(PriorityType.Military, controlPointPriority, false, false, false);
			}
		}

		// Token: 0x0600389A RID: 14490 RVA: 0x0014919C File Offset: 0x0014739C
		public void ActivateBuildSTOSquadron()
		{
			this.canBuildSTOSquadrons = true;
		}

		// Token: 0x0600389B RID: 14491 RVA: 0x001491A8 File Offset: 0x001473A8
		public List<TILaunchFacilityState> CandidateSTOSquadronRegions()
		{
			return (from x in this.regions
				where x.boostPerMonth_dekatons > 0f && x.numSTOFighters < x.maxSTOFighters
				select x.boostFacility).ToList<TILaunchFacilityState>();
		}

		// Token: 0x0600389C RID: 14492 RVA: 0x00149208 File Offset: 0x00147408
		public TILaunchFacilityState GetNextSTOSquadronLocation()
		{
			List<TILaunchFacilityState> list = this.CandidateSTOSquadronRegions();
			if (list.Count <= 0)
			{
				return null;
			}
			if (list.All<TILaunchFacilityState>((TILaunchFacilityState x) => x.region.numSTOFighters == 0))
			{
				return list.MaxBy<TILaunchFacilityState, float>((TILaunchFacilityState x) => x.region.boostPerMonth_dekatons);
			}
			List<TILaunchFacilityState> list2 = list.Where<TILaunchFacilityState>((TILaunchFacilityState x) => x.region.numSTOFighters == 0).ToList<TILaunchFacilityState>();
			if (list2.Count > 0)
			{
				return list2.MaxBy<TILaunchFacilityState, float>((TILaunchFacilityState x) => x.region.boostPerMonth_dekatons);
			}
			return list.MaxBy<TILaunchFacilityState, float>((TILaunchFacilityState x) => x.region.boostPerMonth_dekatons / (float)x.region.numSTOFighters);
		}

		// Token: 0x0600389D RID: 14493 RVA: 0x001492F8 File Offset: 0x001474F8
		public void OnBuildSTOSquadronPriorityComplete()
		{
			TILaunchFacilityState nextSTOSquadronLocation = this.GetNextSTOSquadronLocation();
			if (nextSTOSquadronLocation != null)
			{
				nextSTOSquadronLocation.region.numSTOFighters++;
				TINotificationQueueState.LogSTOFighterComplete(nextSTOSquadronLocation.region.nation, nextSTOSquadronLocation);
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(nextSTOSquadronLocation.region), null, new object[] { nextSTOSquadronLocation.region });
			}
		}

		// Token: 0x0600389E RID: 14494 RVA: 0x0014935E File Offset: 0x0014755E
		public List<TIRegionState> CandidateCoreEconomicRegions()
		{
			return this.regions.Where<TIRegionState>((TIRegionState x) => !x.coreEconomicRegion && !x.colonyRegion && x.nuclearDetonations == 0 && x.nationalGDPShareValue_bn > 500.0).ToList<TIRegionState>();
		}

		// Token: 0x0600389F RID: 14495 RVA: 0x00149390 File Offset: 0x00147590
		public TIRegionState GetNextCoreEcoRegion()
		{
			List<TIRegionState> list = this.CandidateCoreEconomicRegions();
			if (list.Count > 0)
			{
				return (from x in list
					orderby x.accumulatedCoreEconomyRegionTriggers descending, x.nationalGDPShareValue_bn descending, x.populationInMillions descending
					select x).First<TIRegionState>();
			}
			return null;
		}

		// Token: 0x060038A0 RID: 14496 RVA: 0x00149424 File Offset: 0x00147624
		public bool OnCoreEconomicRegionPriorityComplete(TIRegionState region)
		{
			if (region != null)
			{
				region.coreEconomicRegion = true;
				region.resourceRegion = false;
				region.oilRegion = false;
				region.accumulatedCoreEconomyRegionTriggers = 0;
				TINotificationQueueState.LogNationGainsCoreEcoRegion(region.nation, region);
				GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(region), null, new object[] { this, region });
				return true;
			}
			return false;
		}

		// Token: 0x060038A1 RID: 14497 RVA: 0x00149484 File Offset: 0x00147684
		public List<TIRegionState> CandidateCoreMiningRegions()
		{
			return this.regions.Where<TIRegionState>((TIRegionState x) => !x.coreEconomicRegion && !x.coreResourceRegion && x.nuclearDetonations == 0 && x.template.mineCapable).ToList<TIRegionState>();
		}

		// Token: 0x060038A2 RID: 14498 RVA: 0x001494B8 File Offset: 0x001476B8
		public TIRegionState GetNextCoreMiningRegion()
		{
			List<TIRegionState> list = this.CandidateCoreMiningRegions();
			if (list.Count > 0)
			{
				return (from x in list
					orderby x.accumulatedCoreMiningRegionTriggers descending, x.nationalGDPShareValue_bn descending
					select x).First<TIRegionState>();
			}
			return null;
		}

		// Token: 0x060038A3 RID: 14499 RVA: 0x00149528 File Offset: 0x00147728
		public void OnCoreMiningRegionComplete(TIRegionState region)
		{
			if (region != null)
			{
				region.resourceRegion = true;
				region.accumulatedCoreMiningRegionTriggers = 0;
				TINotificationQueueState.LogNationGainsCoreMineralRegion(region.nation, region);
				GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(region), null, new object[] { this, region });
			}
		}

		// Token: 0x060038A4 RID: 14500 RVA: 0x00149578 File Offset: 0x00147778
		public List<TIRegionState> CandidateCoreOilRegions()
		{
			if (!TIGlobalValuesState.GlobalValues.endOfOil)
			{
				return this.regions.Where<TIRegionState>((TIRegionState x) => !x.coreEconomicRegion && !x.coreResourceRegion && x.nuclearDetonations == 0 && x.template.oilCapable).ToList<TIRegionState>();
			}
			return new List<TIRegionState>();
		}

		// Token: 0x060038A5 RID: 14501 RVA: 0x001495C8 File Offset: 0x001477C8
		public TIRegionState GetNextCoreOilRegion()
		{
			List<TIRegionState> list = this.CandidateCoreOilRegions();
			if (list.Count > 0)
			{
				return (from x in list
					orderby x.accumulatedCoreOilRegionTriggers descending, x.nationalGDPShareValue_bn descending
					select x).First<TIRegionState>();
			}
			return null;
		}

		// Token: 0x060038A6 RID: 14502 RVA: 0x00149638 File Offset: 0x00147838
		public bool OnCoreOilRegionPriorityComplete(TIRegionState region)
		{
			if (region != null)
			{
				region.oilRegion = true;
				region.accumulatedCoreOilRegionTriggers = 0;
				TINotificationQueueState.LogNationGainsCoreOilRegion(region.nation, region);
				GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(region), null, new object[] { this, region });
				return true;
			}
			return false;
		}

		// Token: 0x060038A7 RID: 14503 RVA: 0x0014968A File Offset: 0x0014788A
		public List<TIRegionState> CandidateDecolonizeRegions()
		{
			return this.regions.Where<TIRegionState>((TIRegionState x) => x.colonyRegion).ToList<TIRegionState>();
		}

		// Token: 0x060038A8 RID: 14504 RVA: 0x001496BC File Offset: 0x001478BC
		public TIRegionState GetNextDecolonizeRegion()
		{
			List<TIRegionState> list = this.CandidateDecolonizeRegions();
			if (list.Count > 0)
			{
				return (from x in list
					orderby x.accumulatedDecolonizeTriggers descending, x.nationalGDPShareValue_bn descending, x.populationInMillions descending
					select x).First<TIRegionState>();
			}
			return null;
		}

		// Token: 0x060038A9 RID: 14505 RVA: 0x00149750 File Offset: 0x00147950
		public bool OnDecolonizeRegionPriorityComplete(TIRegionState region)
		{
			if (region != null)
			{
				region.colonyRegion = false;
				region.accumulatedDecolonizeTriggers = 0;
				region.permanentlyDecolonized = true;
				this.canAccumulateDecolonizeTriggers = this.CandidateDecolonizeRegions().Count > 0;
				TINotificationQueueState.LogDecolonizeComplete(region.nation, region);
				GameControl.eventManager.TriggerEvent(new MajorRegionStatusChange(region), null, new object[] { this, region });
				return true;
			}
			return false;
		}

		// Token: 0x060038AA RID: 14506 RVA: 0x001497BD File Offset: 0x001479BD
		public void ActivateCanDecontaminateRegion()
		{
			this.canDecontaminate = true;
			if (this.CandidateDecontaminateRegions().Count > 0)
			{
				this.canAccumulateDecontaminateTriggers = true;
				this.PossiblePriorityValidationChange(false);
			}
		}

		// Token: 0x060038AB RID: 14507 RVA: 0x001497E2 File Offset: 0x001479E2
		public List<TIRegionState> CandidateDecontaminateRegions()
		{
			if (!this.canDecontaminate)
			{
				return new List<TIRegionState>();
			}
			return this.regions.Where<TIRegionState>((TIRegionState x) => x.nuclearDetonations > 0).ToList<TIRegionState>();
		}

		// Token: 0x060038AC RID: 14508 RVA: 0x00149824 File Offset: 0x00147A24
		public TIRegionState GetNextDecontaminateRegion()
		{
			List<TIRegionState> list = this.CandidateDecontaminateRegions();
			if (list.Count > 0)
			{
				return (from x in list
					orderby x.accumulatedDecontaminateTriggers descending, x.nationalGDPShareValue_bn descending, x.nuclearDetonations descending
					select x).First<TIRegionState>();
			}
			return null;
		}

		// Token: 0x060038AD RID: 14509 RVA: 0x001498B8 File Offset: 0x00147AB8
		public bool OnDecontaminateRegionPriorityComplete(TIRegionState region)
		{
			if (region != null)
			{
				region.nuclearDetonations--;
				this.canAccumulateDecontaminateTriggers = this.canDecontaminate && this.CandidateDecontaminateRegions().Count > 0;
				region.accumulatedDecontaminateTriggers = 0;
				TINotificationQueueState.LogDecontaminateComplete(region.nation, region);
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(region), null, new object[] { this, region });
				return true;
			}
			return false;
		}

		// Token: 0x060038AE RID: 14510 RVA: 0x00149930 File Offset: 0x00147B30
		public List<TIRegionState> CandidateLegitimizeClaimRegions()
		{
			return this.regions.Where<TIRegionState>((TIRegionState x) => x.hostileRegion).ToList<TIRegionState>();
		}

		// Token: 0x060038AF RID: 14511 RVA: 0x00149964 File Offset: 0x00147B64
		public TIRegionState GetNextLegitimizeClaimRegion()
		{
			List<TIRegionState> list = this.CandidateLegitimizeClaimRegions();
			if (list.Count > 0)
			{
				return (from x in list
					orderby x.AdjacentRegions(false).Any<TIRegionState>((TIRegionState n) => n.nation == x.nation && !n.hostileRegion) descending, x.populationInMillions descending
					select x).First<TIRegionState>();
			}
			return null;
		}

		// Token: 0x060038B0 RID: 14512 RVA: 0x001499D4 File Offset: 0x00147BD4
		public bool OnLegitimizeClaimPriorityComplete()
		{
			TIRegionState nextLegitimizeClaimRegion = this.GetNextLegitimizeClaimRegion();
			if (nextLegitimizeClaimRegion != null)
			{
				this.RemoveHostileClaim(nextLegitimizeClaimRegion);
				TINotificationQueueState.LogLegitimizeClaimComplete(nextLegitimizeClaimRegion);
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(nextLegitimizeClaimRegion), null, new object[] { this, nextLegitimizeClaimRegion });
				return true;
			}
			return false;
		}

		// Token: 0x060038B1 RID: 14513 RVA: 0x00149A20 File Offset: 0x00147C20
		public void SyncAllPriorites(int sourceIdx)
		{
			TIControlPoint ticontrolPoint = this.controlPoints[sourceIdx];
			foreach (TIControlPoint ticontrolPoint2 in this.FactionControlPoints(ticontrolPoint.faction, true, false, true))
			{
				if (ticontrolPoint2 != ticontrolPoint)
				{
					ticontrolPoint2.SyncAllPriorities(ticontrolPoint);
				}
			}
		}

		// Token: 0x060038B2 RID: 14514 RVA: 0x00149A94 File Offset: 0x00147C94
		public static string GetInlinePriorityIcon(PriorityType priority)
		{
			switch (priority)
			{
			case PriorityType.Economy:
				return TemplateManager.global.ECO_InlineSpritePath;
			case PriorityType.Welfare:
				return TemplateManager.global.WEL_InlineSpritePath;
			case PriorityType.Environment:
				return TemplateManager.global.ENV_InlineSpritePath;
			case PriorityType.Knowledge:
				return TemplateManager.global.KNO_InlineSpritePath;
			case PriorityType.Government:
				return TemplateManager.global.GOV_InlineSpritePath;
			case PriorityType.Unity:
				return TemplateManager.global.UNI_InlineSpritePath;
			case PriorityType.Oppression:
				return TemplateManager.global.OPP_InlineSpritePath;
			case PriorityType.Funding:
				return TemplateManager.global.DEV_InlineSpritePath;
			case PriorityType.Spoils:
				return TemplateManager.global.SPO_InlineSpritePath;
			case PriorityType.Civilian_InitiateSpaceflightProgram:
				return TemplateManager.global.FLI_InlineSpritePath;
			case PriorityType.LaunchFacilities:
				return TemplateManager.global.BOO_InlineSpritePath;
			case PriorityType.MissionControl:
				return TemplateManager.global.MC_InlineSpritePath;
			case PriorityType.Military_FoundMilitary:
				return TemplateManager.global.FMI_InlineSpritePath;
			case PriorityType.Military:
				return TemplateManager.global.MIL_InlineSpritePath;
			case PriorityType.Military_BuildArmy:
				return TemplateManager.global.ARM_InlineSpritePath;
			case PriorityType.Military_BuildNavy:
				return TemplateManager.global.NAV_InlineSpritePath;
			case PriorityType.Military_InitiateNuclearProgram:
				return TemplateManager.global.NUC_InlineSpritePath;
			case PriorityType.Military_BuildNuclearWeapons:
				return TemplateManager.global.NUK_InlineSpritePath;
			case PriorityType.Military_BuildSpaceDefenses:
				return TemplateManager.global.DEF_InlineSpritePath;
			case PriorityType.Military_BuildSTOSquadron:
				return TemplateManager.global.STO_InlineSpritePath;
			default:
				return string.Empty;
			}
		}

		// Token: 0x060038B3 RID: 14515 RVA: 0x00149BE0 File Offset: 0x00147DE0
		public bool IsAdjacentToRegion(TIRegionState testRegion, bool IAmAnInvadingArmy)
		{
			return this.extant && this.regions.Any<TIRegionState>((TIRegionState r) => r.IsAdjacent(testRegion, IAmAnInvadingArmy));
		}

		// Token: 0x060038B4 RID: 14516 RVA: 0x00149C22 File Offset: 0x00147E22
		public bool IsAdjacentToNation(TINationState nation, bool IAmAnInvadingArmy)
		{
			return this.adjacentNations.ContainsKey(nation) && (!IAmAnInvadingArmy || this.adjacentNations[nation] == TerrestrialAdjacencyType.FullAdjacency);
		}

		// Token: 0x060038B5 RID: 14517 RVA: 0x00149C48 File Offset: 0x00147E48
		public TerrestrialAdjacencyType NationAdjacency(TINationState nation)
		{
			if (this.adjacentNations.ContainsKey(nation))
			{
				return this.adjacentNations[nation];
			}
			return TerrestrialAdjacencyType.None;
		}

		// Token: 0x060038B6 RID: 14518 RVA: 0x00149C66 File Offset: 0x00147E66
		public List<TINationState> AdjacentNations(bool IAmAnInvadingArmy)
		{
			if (!IAmAnInvadingArmy)
			{
				return this.adjacentNations.Keys.ToList<TINationState>();
			}
			return this.adjacentNations.Keys.Where<TINationState>((TINationState nation) => this.adjacentNations[nation] == TerrestrialAdjacencyType.FullAdjacency).ToList<TINationState>();
		}

		// Token: 0x060038B7 RID: 14519 RVA: 0x00149CA0 File Offset: 0x00147EA0
		public void GenerateAdjacentNationsDictionary()
		{
			this.adjacentNations.Clear();
			if (this.extant)
			{
				foreach (TINationState tinationState in GameStateManager.AllExtantNations().Except<TINationState>(new List<TINationState> { this }))
				{
					TerrestrialAdjacencyType terrestrialAdjacencyType = TerrestrialAdjacencyType.None;
					foreach (TIRegionState tiregionState in tinationState.regions)
					{
						if (this.IsAdjacentToRegion(tiregionState, true))
						{
							terrestrialAdjacencyType = TerrestrialAdjacencyType.FullAdjacency;
							break;
						}
						if (this.IsAdjacentToRegion(tiregionState, false))
						{
							terrestrialAdjacencyType = TerrestrialAdjacencyType.FriendlyCrossingOnly;
						}
					}
					if (terrestrialAdjacencyType != TerrestrialAdjacencyType.None)
					{
						this.adjacentNations.Add(tinationState, terrestrialAdjacencyType);
					}
				}
			}
		}

		// Token: 0x060038B8 RID: 14520 RVA: 0x00149D78 File Offset: 0x00147F78
		public bool CanAlly(TINationState nation, bool ignoreAccess = false)
		{
			if (nation != this && !this.wars.Contains(nation) && !this.rivals.Contains(nation) && !this.allies.Contains(nation) && this.CanImproveRelationsYet(nation) && !this.breakaways.Contains(nation) && nation.allies.Intersect<TINationState>(this.breakaways).Count<TINationState>() == 0 && this.allies.Intersect<TINationState>(nation.breakaways).Count<TINationState>() == 0 && nation != this.breakawayParent && !nation.allies.Contains(this.breakawayParent) && !this.allies.Contains(nation.breakawayParent))
			{
				if (this.numControlPoints > 2 || nation.numControlPoints > 2 || this.HasClaimOnOtherNation(nation, true) || nation.HasClaimOnOtherNation(this, true))
				{
					ignoreAccess = true;
				}
				return ignoreAccess || this.IsAdjacentToNation(nation, false) || this.numNavies != 0 || nation.numNavies != 0;
			}
			return false;
		}

		// Token: 0x060038B9 RID: 14521 RVA: 0x00149E94 File Offset: 0x00148094
		public string GetFeedbackLine(string text, bool condition)
		{
			return new StringBuilder(text).Append(Loc.T("UI.Nation.RelationsFeedback", new object[] { condition ? TIUtilities.GreenLine(Loc.T("UI.Nation.Pass")) : TIUtilities.RedLine(Loc.T("UI.Nation.Fail")) })).ToString();
		}

		// Token: 0x060038BA RID: 14522 RVA: 0x00149EE8 File Offset: 0x001480E8
		public string CanAllyFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly1", new object[] { this.displayNameWithArticle }), this.CanAlly(nation, false)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly2"), !this.wars.Contains(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly3"), !this.rivals.Contains(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(this.CanImproveRelationsYet(nation) ? Loc.T("UI.Nation.CanAlly4") : Loc.T("UI.Nation.CanAlly5", new object[] { this.improveRelationsCooldowns[nation].ToCustomDateString() }), this.CanImproveRelationsYet(nation)));
			if (this.breakaways.Count > 0)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly6", new object[] { this.displayNameWithArticle }), !this.breakaways.Contains(nation)));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly7", new object[] { nation.displayName }), nation.allies.Intersect<TINationState>(this.breakaways).Count<TINationState>() == 0));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly8", new object[] { this.displayName, nation.displayName }), this.allies.Intersect<TINationState>(nation.breakaways).Count<TINationState>() == 0));
			}
			if (this.breakaway)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly9", new object[] { this.displayName, nation.displayName }), nation != this.breakawayParent));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly10", new object[]
				{
					nation.displayName,
					this.breakawayParent.displayName
				}), !nation.allies.Contains(this.breakawayParent)));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly11", new object[]
				{
					this.displayName,
					this.breakawayParent.displayName
				}), !this.allies.Contains(nation.breakawayParent)));
			}
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAlly12", new object[] { this.displayName, nation.displayName }), this.numControlPoints >= 3 || nation.numControlPoints >= 3 || this.IsAdjacentToNation(nation, true) || this.numNavies > 0 || nation.numNavies > 0 || nation.HasClaimOnOtherNation(this, true) || this.HasClaimOnOtherNation(nation, true)));
			return stringBuilder.ToString();
		}

		// Token: 0x170009DE RID: 2526
		// (get) Token: 0x060038BB RID: 14523 RVA: 0x0014A1DD File Offset: 0x001483DD
		public List<TINationState> eligibleAlliances
		{
			get
			{
				return (from x in GameStateManager.AllExtantNations()
					where this.CanAlly(x, false)
					select x).ToList<TINationState>();
			}
		}

		// Token: 0x060038BC RID: 14524 RVA: 0x0014A1FC File Offset: 0x001483FC
		public bool CanRival(TINationState nation)
		{
			return nation != this && !this.wars.Contains(nation) && !this.rivals.Contains(nation) && !this.allies.Contains(nation) && (this.numControlPoints > 2 || nation.numControlPoints > 2 || this.IsAdjacentToNation(nation, true) || this.numNavies != 0 || this.HasClaimOnOtherNation(nation, true));
		}

		// Token: 0x060038BD RID: 14525 RVA: 0x0014A270 File Offset: 0x00148470
		public string CanRivalFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanRival1", new object[] { this.displayNameWithArticle }), this.CanRival(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanRival2", new object[] { this.displayNameWithArticle, nation.displayNameWithArticle }), !this.allies.Contains(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanRival3", new object[] { this.displayName, nation.displayName }), this.numControlPoints >= 3 || nation.numControlPoints >= 3 || this.IsAdjacentToNation(nation, true) || this.numNavies > 0 || this.HasClaimOnOtherNation(nation, true)));
			return stringBuilder.ToString();
		}

		// Token: 0x170009DF RID: 2527
		// (get) Token: 0x060038BE RID: 14526 RVA: 0x0014A353 File Offset: 0x00148553
		public List<TINationState> eligibleRivals
		{
			get
			{
				return (from x in GameStateManager.AllExtantNations()
					where this.CanRival(x)
					select x).ToList<TINationState>();
			}
		}

		// Token: 0x060038BF RID: 14527 RVA: 0x0014A370 File Offset: 0x00148570
		public bool IsEnemy(TINationState nation)
		{
			return this.wars.Contains(nation) || this.rivals.Contains(nation);
		}

		// Token: 0x060038C0 RID: 14528 RVA: 0x0014A38E File Offset: 0x0014858E
		public bool CanEndAlliance(TINationState nation)
		{
			return this.allies.Contains(nation) && (!this.inFederation || this.federation != nation.federation);
		}

		// Token: 0x060038C1 RID: 14529 RVA: 0x0014A3BC File Offset: 0x001485BC
		public string CanEndAllianceFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndAlliance1", new object[] { this.displayNameWithArticle }), this.CanEndAlliance(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndAlliance2"), !this.inSameFederation(nation)));
			return stringBuilder.ToString();
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x060038C2 RID: 14530 RVA: 0x0014A423 File Offset: 0x00148623
		public List<TINationState> eligibleEndAlliances
		{
			get
			{
				return this.allies.Where<TINationState>((TINationState x) => this.CanEndAlliance(x)).ToList<TINationState>();
			}
		}

		// Token: 0x060038C3 RID: 14531 RVA: 0x0014A444 File Offset: 0x00148644
		public bool CanEndRivalry(TINationState nation)
		{
			if (this.rivals.Contains(nation) && !this.wars.Contains(nation) && this.CanImproveRelationsYet(nation) && !this.breakaways.Contains(nation) && this.breakawayParent != nation)
			{
				TINationState tinationState = this.breakawayParent;
				if (tinationState == null || !tinationState.allies.Contains(nation))
				{
					IEnumerable<TINationState> enumerable = this.breakaways.SelectMany<TINationState, TINationState>((TINationState x) => x.allies);
					if ((enumerable == null || !enumerable.Contains(nation)) && this.allies.None<TINationState>((TINationState x) => nation.breakaways.Contains(x)))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060038C4 RID: 14532 RVA: 0x0014A544 File Offset: 0x00148744
		public string CanEndRivalryFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndRivalry1", new object[] { this.displayNameWithArticle }), this.CanEndRivalry(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndRivalry2"), !this.wars.Contains(nation)));
			if (this.breakaway)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndRivalry3", new object[] { this.displayNameWithArticle }), nation != this.breakawayParent));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndRivalry4", new object[] { this.displayNameWithArticle }), !this.breakawayParent.allies.Contains(nation)));
			}
			if (this.breakaways.Count > 0)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndRivalry5", new object[] { this.displayNameWithArticle }), !this.breakaways.Contains(nation)));
				StringBuilder stringBuilder2 = stringBuilder;
				string text = Loc.T("UI.Nation.CanEndRivalry6", new object[] { this.displayNameWithArticle, nation.displayNameWithArticleCapitalized });
				IEnumerable<TINationState> enumerable = this.breakaways.SelectMany<TINationState, TINationState>((TINationState x) => x.allies);
				stringBuilder2.AppendLine(this.GetFeedbackLine(text, enumerable == null || !enumerable.Contains(nation)));
			}
			if (nation.breakaways.Count > 0)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanEndRivalry9", new object[] { this.displayNameWithArticleCapitalized, nation.displayNameWithArticle }), this.allies.None<TINationState>((TINationState x) => nation.breakaways.Contains(x))));
			}
			stringBuilder.AppendLine(this.GetFeedbackLine(this.CanImproveRelationsYet(nation) ? Loc.T("UI.Nation.CanEndRivalry7") : Loc.T("UI.Nation.CanEndRivalry8", new object[] { this.improveRelationsCooldowns[nation].ToCustomDateString() }), this.CanImproveRelationsYet(nation)));
			return stringBuilder.ToString();
		}

		// Token: 0x060038C5 RID: 14533 RVA: 0x0014A7BD File Offset: 0x001489BD
		public bool CanNormalize(TINationState nation)
		{
			return this.CanEndRivalry(nation) || this.CanEndAlliance(nation);
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x060038C6 RID: 14534 RVA: 0x0014A7D1 File Offset: 0x001489D1
		public List<TINationState> eligibleEndRivalries
		{
			get
			{
				return this.rivals.Where<TINationState>((TINationState x) => this.CanEndRivalry(x)).ToList<TINationState>();
			}
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x060038C7 RID: 14535 RVA: 0x0014A7F0 File Offset: 0x001489F0
		public List<TINationState> candidateUnifications
		{
			get
			{
				List<TINationState> list = new List<TINationState>();
				if (this.inFederation)
				{
					list.AddRange(this.federation.members.Where<TINationState>((TINationState x) => x != this && this.MyClaimOnOtherCapital(x, TemplateManager.global.prohibitCapitalShenanigans, false)));
				}
				list.AddRangeUnique<TINationState>(this.breakaways.Where<TINationState>((TINationState x) => !this.wars.Contains(x)).ToList<TINationState>());
				if (this.breakawayParent != null)
				{
					list.AddUnique(this.breakawayParent);
				}
				return list;
			}
		}

		// Token: 0x170009E3 RID: 2531
		// (get) Token: 0x060038C8 RID: 14536 RVA: 0x0014A86C File Offset: 0x00148A6C
		public List<TINationState> eligibleUnifications
		{
			get
			{
				List<TINationState> list = new List<TINationState>();
				if (this.inFederation)
				{
					list.AddRange(this.federation.members.Where<TINationState>((TINationState x) => x != this && this.MyClaimOnOtherCapital(x, TemplateManager.global.prohibitCapitalShenanigans, false) && this.executiveFaction == x.TotalOwningFaction && this.CanImproveRelationsYet(x) && this.ExecutivePowerConsolidated && x.ExecutivePowerConsolidated));
				}
				list.AddRange(this.breakaways.Where<TINationState>((TINationState x) => !this.wars.Contains(x) && this.executiveFaction == x.TotalOwningFaction));
				if (this.breakawayParent != null && !this.wars.Contains(this.breakawayParent) && this.breakawayParent.executiveFaction == this.TotalOwningFaction)
				{
					list.Add(this.breakawayParent);
				}
				return list;
			}
		}

		// Token: 0x060038C9 RID: 14537 RVA: 0x0014A90C File Offset: 0x00148B0C
		public string CanUnifyFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback1", new object[] { this.displayNameWithArticle }), this.eligibleUnifications.Contains(nation)));
			if (this.breakawayParent != null)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback2", new object[] { this.breakawayParent.displayName }), !this.wars.Contains(this.breakawayParent)));
				if (this.breakawayParent.executiveFaction != null)
				{
					stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback3", new object[]
					{
						this.breakawayParent.executiveFaction.displayName,
						this.displayName
					}), this.breakawayParent.executiveFaction == this.TotalOwningFaction));
				}
				else
				{
					stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback5", new object[]
					{
						this.breakawayParent.displayName,
						this.displayName
					}), this.breakawayParent.executiveFaction == this.TotalOwningFaction));
				}
			}
			if (this.breakaways.Contains(nation))
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback4", new object[] { nation.displayName }), !this.wars.Contains(nation)));
				if (this.executiveFaction != null)
				{
					stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback3", new object[]
					{
						this.executiveFaction.displayName,
						nation.displayName
					}), this.executiveFaction == nation.TotalOwningFaction));
				}
				else
				{
					stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback5", new object[] { this.displayName, nation.displayName }), this.executiveFaction == nation.TotalOwningFaction));
				}
			}
			else
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback6"), this.inFederation && this.federation == nation.federation));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback7", new object[]
				{
					this.displayName,
					nation.displayName,
					(TemplateManager.global.prohibitCapitalShenanigans && nation.originalCapital != null) ? nation.originalCapital.displayName : nation.capital.displayName
				}), this.MyClaimOnOtherCapital(nation, TemplateManager.global.prohibitCapitalShenanigans, false)));
				if (!this.MyClaimOnOtherCapital(nation, TemplateManager.global.prohibitCapitalShenanigans, false) && !this.hostileClaims.Contains(nation.capital) && this.MyClaimOnOtherCapital(nation, TemplateManager.global.prohibitCapitalShenanigans, true))
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.CanUnifyFeedback7a", new object[]
					{
						this.displayName,
						nation.displayName,
						TemplateManager.global.democracyDecreaseToMakeHostileClaim.ToString("N1")
					}));
				}
				else if (this.hostileClaims.Contains(nation.capital))
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.CanUnifyFeedback7b"));
				}
				if (this.executiveFaction != null)
				{
					stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback3", new object[]
					{
						this.executiveFaction.displayName,
						nation.displayName
					}), this.executiveFaction == nation.TotalOwningFaction));
				}
				else
				{
					stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback5", new object[] { this.displayName, nation.displayName }), this.executiveFaction == nation.TotalOwningFaction));
				}
				stringBuilder.AppendLine(this.GetFeedbackLine(this.CanImproveRelationsYet(nation) ? Loc.T("UI.Nation.CanUnifyFeedback8") : Loc.T("UI.Nation.CanUnifyFeedback9", new object[] { this.improveRelationsCooldowns[nation].ToCustomDateString() }), this.CanImproveRelationsYet(nation)));
			}
			bool flag = this.ExecutivePowerConsolidated && nation.ExecutivePowerConsolidated;
			if (flag)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback10", new object[] { nation.modifiedConsolidatedExecControl_days }), flag));
			}
			else
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback10a", new object[] { this.displayName, nation.modifiedConsolidatedExecControl_days }), this.ExecutivePowerConsolidated));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanUnifyFeedback10a", new object[] { nation.displayName, nation.modifiedConsolidatedExecControl_days }), nation.ExecutivePowerConsolidated));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060038CA RID: 14538 RVA: 0x0014AE1C File Offset: 0x0014901C
		public List<TIRegionState> MyClaimsOnOtherNation(TINationState targetNation, bool includeHostile)
		{
			if (!includeHostile)
			{
				return (from x in targetNation.regions.Intersect<TIRegionState>(this.claims)
					where !this.ClaimWillBeHostile(x, false)
					select x).ToList<TIRegionState>();
			}
			return targetNation.regions.Intersect<TIRegionState>(this.claims).ToList<TIRegionState>();
		}

		// Token: 0x060038CB RID: 14539 RVA: 0x0014AE6A File Offset: 0x0014906A
		public bool HasClaimOnOtherNation(TINationState targetNation, bool includeHostile = true)
		{
			return this.MyClaimsOnOtherNation(targetNation, includeHostile).Count > 0;
		}

		// Token: 0x060038CC RID: 14540 RVA: 0x0014AE7C File Offset: 0x0014907C
		public List<TIRegionState> MyNonCapitalClaimsOnOtherNation(TINationState targetNation)
		{
			return targetNation.regions.Except<TIRegionState>(new TIRegionState[] { targetNation.capital }).Intersect<TIRegionState>(this.claims).ToList<TIRegionState>();
		}

		// Token: 0x060038CD RID: 14541 RVA: 0x0014AEA8 File Offset: 0x001490A8
		public bool HasNonCapitalClaimOnOtherNation(TINationState targetNation)
		{
			return this.MyNonCapitalClaimsOnOtherNation(targetNation).Any<TIRegionState>();
		}

		// Token: 0x060038CE RID: 14542 RVA: 0x0014AEB6 File Offset: 0x001490B6
		public List<TIRegionState> MyNonCapitalAdjacentClaimsOnOtherNation(TINationState targetNation)
		{
			return (from x in targetNation.regions.Except<TIRegionState>(new TIRegionState[] { targetNation.capital }).Intersect<TIRegionState>(this.claims)
				where x.AdjacentNations(false, true).Contains(this)
				select x).ToList<TIRegionState>();
		}

		// Token: 0x060038CF RID: 14543 RVA: 0x0014AEF3 File Offset: 0x001490F3
		public bool NonCapitalAdjacentClaimsOnOtherNation(TINationState targetNation)
		{
			return this.MyNonCapitalAdjacentClaimsOnOtherNation(targetNation).Count > 0;
		}

		// Token: 0x060038D0 RID: 14544 RVA: 0x0014AF04 File Offset: 0x00149104
		public List<TIRegionState> ExternalClaims()
		{
			return this.claims.Where<TIRegionState>((TIRegionState x) => x.nation != this).ToList<TIRegionState>();
		}

		// Token: 0x060038D1 RID: 14545 RVA: 0x0014AF22 File Offset: 0x00149122
		public bool HasExternalClaims()
		{
			return this.claims.Any<TIRegionState>((TIRegionState x) => x.nation != this);
		}

		// Token: 0x060038D2 RID: 14546 RVA: 0x0014AF3C File Offset: 0x0014913C
		public bool MyClaimOnOtherCapital(TINationState targetNation, bool originalCapital, bool includeHostile)
		{
			if (originalCapital && targetNation.originalCapital == null)
			{
				return false;
			}
			TIRegionState tiregionState = targetNation.capital;
			if (originalCapital)
			{
				tiregionState = targetNation.originalCapital;
			}
			bool flag = this.claims.Contains(tiregionState);
			bool flag2 = !this.ClaimWillBeHostile(tiregionState, true) && !this.HostileClaimDueToDemocracy(targetNation);
			return targetNation.extant && flag && (includeHostile || flag2);
		}

		// Token: 0x170009E4 RID: 2532
		// (get) Token: 0x060038D3 RID: 14547 RVA: 0x0014AFA1 File Offset: 0x001491A1
		public bool WarCapable
		{
			get
			{
				return !this.breakaway && (this.numStandardArmies > 0 || this.numNuclearWeapons > 0);
			}
		}

		// Token: 0x170009E5 RID: 2533
		// (get) Token: 0x060038D4 RID: 14548 RVA: 0x0014AFC1 File Offset: 0x001491C1
		public List<TINationState> WarCapableAllies
		{
			get
			{
				return this.allies.Where<TINationState>((TINationState x) => x.WarCapable).ToList<TINationState>();
			}
		}

		// Token: 0x060038D5 RID: 14549 RVA: 0x0014AFF2 File Offset: 0x001491F2
		public bool IsAtWarWith(TINationState nation)
		{
			return this.wars.Contains(nation);
		}

		// Token: 0x060038D6 RID: 14550 RVA: 0x0014B000 File Offset: 0x00149200
		public bool IsRivalWith(TINationState nation)
		{
			return this.rivals.Contains(nation);
		}

		// Token: 0x060038D7 RID: 14551 RVA: 0x0014B00E File Offset: 0x0014920E
		public bool IsAlliedWith(TINationState nation, bool includeSelf = false)
		{
			return this.allies.Contains(nation) || (includeSelf && nation == this);
		}

		// Token: 0x170009E6 RID: 2534
		// (get) Token: 0x060038D8 RID: 14552 RVA: 0x0014B02C File Offset: 0x0014922C
		public List<TINationState> enemies
		{
			get
			{
				return new List<TINationState>(this.wars).Union<TINationState>(this.rivals).Distinct<TINationState>().ToList<TINationState>();
			}
		}

		// Token: 0x170009E7 RID: 2535
		// (get) Token: 0x060038D9 RID: 14553 RVA: 0x0014B04E File Offset: 0x0014924E
		public List<TIWarState> currentWarStates
		{
			get
			{
				return GameStateManager.GlobalValues().interstateWars.Where<TIWarState>((TIWarState x) => x.attackingAlliance.Contains(this) || x.defendingAlliance.Contains(this)).ToList<TIWarState>();
			}
		}

		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x060038DA RID: 14554 RVA: 0x0014B070 File Offset: 0x00149270
		public List<TIWarState> offensiveWarStates
		{
			get
			{
				return GameStateManager.GlobalValues().interstateWars.Where<TIWarState>((TIWarState x) => x.attackingAlliance.Contains(this)).ToList<TIWarState>();
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x060038DB RID: 14555 RVA: 0x0014B092 File Offset: 0x00149292
		public List<TIWarState> defensiveWarStates
		{
			get
			{
				return GameStateManager.GlobalValues().interstateWars.Where<TIWarState>((TIWarState x) => x.defendingAlliance.Contains(this)).ToList<TIWarState>();
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x060038DC RID: 14556 RVA: 0x0014B0B4 File Offset: 0x001492B4
		public List<TIWarState> warsImLeading
		{
			get
			{
				return GameStateManager.GlobalValues().interstateWars.Where<TIWarState>((TIWarState x) => x.attackingAllianceLeader == this || x.defendingAllianceLeader == this).ToList<TIWarState>();
			}
		}

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x060038DD RID: 14557 RVA: 0x0014B0D6 File Offset: 0x001492D6
		public List<TIWarState> offensiveWarsImLeading
		{
			get
			{
				return GameStateManager.GlobalValues().interstateWars.Where<TIWarState>((TIWarState x) => x.attackingAllianceLeader == this).ToList<TIWarState>();
			}
		}

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x060038DE RID: 14558 RVA: 0x0014B0F8 File Offset: 0x001492F8
		public List<TIWarState> defensiveWarsImLeading
		{
			get
			{
				return GameStateManager.GlobalValues().interstateWars.Where<TIWarState>((TIWarState x) => x.defendingAllianceLeader == this).ToList<TIWarState>();
			}
		}

		// Token: 0x060038DF RID: 14559 RVA: 0x0014B11C File Offset: 0x0014931C
		public List<TIWarState> findWarsWith(TINationState nation)
		{
			return this.currentWarStates.Where<TIWarState>((TIWarState x) => x.EnemyAlliance(this).Contains(nation)).ToList<TIWarState>();
		}

		// Token: 0x060038E0 RID: 14560 RVA: 0x0014B159 File Offset: 0x00149359
		private void AddWar(TINationState enemy)
		{
			this.wars.Add(enemy);
			this.SetDataDirty();
			this.SetArmyAccessibilityDirty();
			this.SortWarsList();
		}

		// Token: 0x060038E1 RID: 14561 RVA: 0x0014B179 File Offset: 0x00149379
		private void RemoveWar(TINationState enemy)
		{
			if (this.wars.Contains(enemy))
			{
				this.wars.Remove(enemy);
				this.SetDataDirty();
				this.SetArmyAccessibilityDirty();
				this.SortWarsList();
			}
		}

		// Token: 0x060038E2 RID: 14562 RVA: 0x0014B1A8 File Offset: 0x001493A8
		private void InitiateWarWithSingleEnemy(TIFactionState actingFaction, TINationState enemyNation)
		{
			this.EndAlliance(actingFaction, enemyNation);
			this.EndRivalry(actingFaction, enemyNation);
			this.AddWar(enemyNation);
			enemyNation.AddWar(this);
		}

		// Token: 0x060038E3 RID: 14563 RVA: 0x0014B1C8 File Offset: 0x001493C8
		private void EndWarWithSingleEnemy(TIFactionState actingFaction, TINationState otherNation, bool maintainRivalry, bool teleportArmiesNow)
		{
			if (this.wars.Contains(otherNation))
			{
				this.RemoveWar(otherNation);
				otherNation.RemoveWar(this);
				if (maintainRivalry && this.CanRival(otherNation))
				{
					this.InitiateRivalry(actingFaction, otherNation, false, false);
				}
			}
			foreach (TIRegionState tiregionState in this.regions.Union<TIRegionState>(otherNation.regions))
			{
				tiregionState.ValidateAndCleanOccupations();
			}
			foreach (TIArmyState tiarmyState in this.armies.Union<TIArmyState>(otherNation.armies))
			{
				if (otherNation.regions.Contains(tiarmyState.AI_targetEnemyRegion))
				{
					tiarmyState.AI_targetEnemyRegion = null;
				}
				tiarmyState.SetArmyDataDirty();
				if (teleportArmiesNow)
				{
					tiarmyState.CheckAndPromptIfInIllegalRegion(false, true);
				}
			}
			TIGlobalValuesState.AgglomerateAllWars();
		}

		// Token: 0x060038E4 RID: 14564 RVA: 0x0014B2C0 File Offset: 0x001494C0
		public void SyncWarCount(TINationState enemy)
		{
			int num = this.wars.Count<TINationState>((TINationState x) => x == enemy);
			foreach (TIWarState tiwarState in TIGlobalValuesState.GlobalValues.interstateWars)
			{
				if (tiwarState.attackingAlliance.Contains(this) && tiwarState.defendingAlliance.Contains(enemy))
				{
					num--;
				}
				else if (tiwarState.defendingAlliance.Contains(this) && tiwarState.attackingAlliance.Contains(enemy))
				{
					num--;
				}
			}
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					this.wars.Remove(enemy);
				}
				return;
			}
			if (num < 0)
			{
				for (int j = 0; j < Mathf.Abs(num); j++)
				{
					this.wars.Add(enemy);
				}
			}
		}

		// Token: 0x060038E5 RID: 14565 RVA: 0x0014B3D4 File Offset: 0x001495D4
		public void DeclareLimitedWar(TIFactionState actingFaction, TINationState defendingNation)
		{
			GameStateManager.GlobalValues().InitiateWar(this, defendingNation, new List<TINationState> { this }, new List<TINationState> { defendingNation });
			this.InitiateWarWithSingleEnemy(actingFaction, defendingNation);
			this.SetDataDirty();
		}

		// Token: 0x060038E6 RID: 14566 RVA: 0x0014B40C File Offset: 0x0014960C
		public float CohesionLossFromDeclaringWar(TINationState defendingNation)
		{
			float num = 0f;
			if (!this.alienNation && !defendingNation.alienNation && this.rivals.Contains(defendingNation) && this.rivalryCooldowns.ContainsKey(defendingNation) && this.rivalryCooldowns[defendingNation] >= TITimeState.Now())
			{
				num = Mathf.Min(TemplateManager.global.maxCohesionLossWhenDeclaringWarOnRival, TemplateManager.global.baseCohesionLossWhenDeclaringWarOnNewRival * (float)(this.rivalryCooldowns[defendingNation].DifferenceInDays(TITimeState.Now()) / (double)TemplateManager.global.newRivalryCohesionPenaltyWindow_d));
				num *= this.democracy / 10f;
			}
			return num;
		}

		// Token: 0x060038E7 RID: 14567 RVA: 0x0014B4B8 File Offset: 0x001496B8
		public void DeclareFullWar(TIFactionState actingFaction, TINationState defendingNation)
		{
			List<TINationState> list = new List<TINationState> { defendingNation };
			list.AddRange(defendingNation.WarCapableAllies.Where<TINationState>((TINationState x) => x.NoFederationConflictOfInterest(this)));
			TIWarState tiwarState = GameStateManager.GlobalValues().InitiateWar(this, defendingNation, new List<TINationState> { this }, list);
			float num = this.CohesionLossFromDeclaringWar(defendingNation);
			if (num > 0f)
			{
				float num2 = this.AddToCohesion(-num, TINationState.CohesionChangeReason.CohesionReason_DeclaringWarOnNewRival);
				Dictionary<TINationState, float> dictionary = tiwarState.cohesionGainByNation;
				dictionary[this] += num2;
			}
			else if (this.wars.Count == 0 && this.unrest < 8f && this.cohesion > 2f && (this.democracy < 7f || defendingNation.democracy < 7f))
			{
				float num3 = this.AddToCohesion(TemplateManager.global.cohesionGainFromDeclaringWarOnOldRival, TINationState.CohesionChangeReason.CohesionReason_DeclaringWarOnOldRival);
				Dictionary<TINationState, float> dictionary = tiwarState.cohesionGainByNation;
				dictionary[this] += num3;
			}
			if (defendingNation.unrest < 8f && defendingNation.cohesion > 2f)
			{
				float num4 = defendingNation.AddToCohesion(TemplateManager.global.cohesionGainFromBeingTargetOfWar, TINationState.CohesionChangeReason.CohesionReason_WarDeclaredOnUs);
				Dictionary<TINationState, float> dictionary = tiwarState.cohesionGainByNation;
				dictionary[defendingNation] += num4;
			}
			foreach (TINationState tinationState in list)
			{
				tinationState.InitiateWarWithSingleEnemy(actingFaction, this);
				if (tinationState != defendingNation && tinationState.unrest < 8f && tinationState.cohesion > 2f)
				{
					float num5 = tinationState.AddToCohesion(TemplateManager.global.cohesionGainFromAnsweringAllyCallToDefensiveWar, TINationState.CohesionChangeReason.CohesionReason_AnsweredAllyCallToWar);
					Dictionary<TINationState, float> dictionary = tiwarState.cohesionGainByNation;
					TINationState tinationState2 = tinationState;
					dictionary[tinationState2] += num5;
				}
			}
			foreach (TINationState tinationState3 in this.allies)
			{
				if (tinationState3.CanJoinNewWarAsAttacker(tiwarState))
				{
					GameStateManager.PromptQueue().AddPrompt(tinationState3, this, tiwarState, "PromptRespondToAllyOffensiveWarCall", 0);
				}
			}
			if (this.executiveFaction != null && defendingNation.executiveFaction != null && !this.executiveFaction.permanentAlly(defendingNation.executiveFaction))
			{
				defendingNation.executiveFaction.GainFactionHate(this.executiveFaction, (float)defendingNation.numControlPoints * TemplateManager.global.factionHateForDeclaringWarCPMultiplier, false, "Terrestrial War Declared", true);
			}
			this.SetDataDirty();
		}

		// Token: 0x060038E8 RID: 14568 RVA: 0x0014B764 File Offset: 0x00149964
		public void JoinWar(TIFactionState actingFaction, TINationState ally, TIWarState war)
		{
			if (war.attackingAlliance.Contains(ally))
			{
				war.JoinAttackers(this);
				if (this.wars.Count == 0)
				{
					this.AddToCohesion(TemplateManager.global.cohesionGainFromAnsweringAllyCallToOffensiveWar, TINationState.CohesionChangeReason.CohesionReason_AnsweredAllyCallToWar);
					Dictionary<TINationState, float> cohesionGainByNation = war.cohesionGainByNation;
					cohesionGainByNation[this] += TemplateManager.global.cohesionGainFromAnsweringAllyCallToOffensiveWar;
				}
				using (IEnumerator<TINationState> enumerator = war.defendingAlliance.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TINationState tinationState = enumerator.Current;
						this.InitiateWarWithSingleEnemy(actingFaction, tinationState);
					}
					goto IL_00D0;
				}
			}
			if (war.defendingAlliance.Contains(ally))
			{
				war.JoinDefenders(this);
				foreach (TINationState tinationState2 in war.attackingAlliance)
				{
					this.InitiateWarWithSingleEnemy(actingFaction, tinationState2);
				}
			}
			IL_00D0:
			TINotificationQueueState.LogNationJoinsWar(war, this);
			this.SetDataDirty();
		}

		// Token: 0x060038E9 RID: 14569 RVA: 0x0014B86C File Offset: 0x00149A6C
		public bool InThisWar(TIWarState war)
		{
			return war.attackingAlliance.Contains(this) || war.defendingAlliance.Contains(this);
		}

		// Token: 0x060038EA RID: 14570 RVA: 0x0014B88A File Offset: 0x00149A8A
		public bool NoFederationConflictOfInterest(TINationState otherNation)
		{
			return !this.inFederation || otherNation.federation != this.federation;
		}

		// Token: 0x060038EB RID: 14571 RVA: 0x0014B8A7 File Offset: 0x00149AA7
		public bool NoFederationConflictOfInterest(List<TINationState> otherNations)
		{
			return !this.inFederation || otherNations.None<TINationState>((TINationState x) => x.federation == this.federation);
		}

		// Token: 0x060038EC RID: 14572 RVA: 0x0014B8C5 File Offset: 0x00149AC5
		public bool AllowedWarTarget_NoRivalryCheck(TINationState targetNation, List<TINationState> warCapableAllies)
		{
			return targetNation.WarCapableAllies.Intersect<TINationState>(warCapableAllies).Count<TINationState>() <= 0 && this.AccessibleWarEnemy(targetNation, false);
		}

		// Token: 0x060038ED RID: 14573 RVA: 0x0014B8EC File Offset: 0x00149AEC
		public List<TINationState> ValidNewWarTargets()
		{
			if (!this.WarCapable)
			{
				return new List<TINationState>();
			}
			List<TINationState> list = new List<TINationState>(this.rivals);
			List<TINationState> warCapableAllies = this.WarCapableAllies;
			foreach (TINationState tinationState in this.rivals)
			{
				if (!this.AllowedWarTarget_NoRivalryCheck(tinationState, warCapableAllies))
				{
					list.Remove(tinationState);
				}
			}
			return list.Distinct<TINationState>().ToList<TINationState>();
		}

		// Token: 0x060038EE RID: 14574 RVA: 0x0014B978 File Offset: 0x00149B78
		public bool ValidNewWarTarget(TINationState nation, bool skipRivalry = false)
		{
			return this.WarCapable && (this.rivals.Contains(nation) || skipRivalry) && nation.WarCapableAllies.Intersect<TINationState>(this.WarCapableAllies).Count<TINationState>() <= 0 && this.AccessibleWarEnemy(nation, false);
		}

		// Token: 0x060038EF RID: 14575 RVA: 0x0014B9C8 File Offset: 0x00149BC8
		public string CanAttackFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAttack1", new object[] { this.displayNameWithArticle, nation.displayNameWithArticle }), this.ValidNewWarTarget(nation, false)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAttack2"), this.rivals.Contains(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAttack3"), this.armies.Count > 0));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAttack4", new object[] { this.displayNameWithArticle }), !this.breakaway));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAttack5", new object[] { nation.displayNameWithArticle }), nation.WarCapableAllies.Intersect<TINationState>(this.WarCapableAllies).Count<TINationState>() == 0));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanAttack6", new object[] { nation.displayNameWithArticle }), this.AccessibleWarEnemy(nation, false)));
			float num = this.CohesionLossFromDeclaringWar(nation);
			if (num > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.CohesionLossFromWarDeclaration", new object[]
				{
					this.displayNameWithArticleCapitalized,
					num.ToString("N2"),
					nation.displayNameWithArticle,
					this.rivalryCooldowns[nation].ToCustomDateString()
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060038F0 RID: 14576 RVA: 0x0014BB58 File Offset: 0x00149D58
		public bool CanJoinNewWarAsAttacker(TIWarState war)
		{
			return !this.InThisWar(war) && this.allies.Contains(war.attacker) && this.allies.Intersect<TINationState>(war.defendingAlliance).Count<TINationState>() == 0 && this.WarCapable && this.NoFederationConflictOfInterest(war.defendingAlliance.ToList<TINationState>());
		}

		// Token: 0x060038F1 RID: 14577 RVA: 0x0014BBB4 File Offset: 0x00149DB4
		public bool CanJoinExistingWarAsAttacker(TIWarState war)
		{
			return !this.InThisWar(war) && this.allies.Intersect<TINationState>(war.attackingAlliance).Count<TINationState>() > 0 && this.allies.Intersect<TINationState>(war.defendingAlliance).Count<TINationState>() == 0 && this.WarCapable && this.NoFederationConflictOfInterest(war.defendingAlliance.ToList<TINationState>());
		}

		// Token: 0x060038F2 RID: 14578 RVA: 0x0014BC18 File Offset: 0x00149E18
		public bool CanJoinExistingWarAsDefender(TIWarState war)
		{
			return !this.InThisWar(war) && this.allies.Intersect<TINationState>(war.attackingAlliance).Count<TINationState>() == 0 && this.allies.Intersect<TINationState>(war.defendingAlliance).Count<TINationState>() > 0 && this.WarCapable && this.NoFederationConflictOfInterest(war.attackingAlliance.ToList<TINationState>());
		}

		// Token: 0x060038F3 RID: 14579 RVA: 0x0014BC7C File Offset: 0x00149E7C
		public float CohesionLossFromWhitePeace(TIWarState war)
		{
			if (!this.alienNation && war.cohesionGainByNation.ContainsKey(this))
			{
				int num = war.annexedRegions.Distinct<TIRegionState>().Count<TIRegionState>((TIRegionState x) => x.nation == this);
				float num2 = war.cohesionGainByNation[this] * ((this.democracy - (float)num) / 10f) * (war.attackingAlliance.Contains(this) ? 2f : 0.5f);
				if (num2 > 0f)
				{
					return -num2;
				}
			}
			return 0f;
		}

		// Token: 0x060038F4 RID: 14580 RVA: 0x0014BD04 File Offset: 0x00149F04
		public void WhitePeace(TIFactionState actingFaction, TIWarState war, bool processCohesionChange)
		{
			if (war.attackingAllianceLeader == this || war.defendingAllianceLeader == this)
			{
				TINationState.EndFullWar(actingFaction, war, true, processCohesionChange);
				return;
			}
			if (war.attackingAlliance.Contains(this))
			{
				using (IEnumerator<TINationState> enumerator = war.defendingAlliance.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TINationState tinationState = enumerator.Current;
						if (processCohesionChange)
						{
							float num = tinationState.CohesionLossFromWhitePeace(war);
							if (num < 0f)
							{
								this.AddToCohesion(num, TINationState.CohesionChangeReason.CohesionReason_WarEnded);
							}
						}
						this.EndWarWithSingleEnemy(actingFaction, tinationState, true, true);
					}
					goto IL_00E5;
				}
			}
			if (war.defendingAlliance.Contains(this))
			{
				foreach (TINationState tinationState2 in war.attackingAlliance)
				{
					if (processCohesionChange)
					{
						float num2 = tinationState2.CohesionLossFromWhitePeace(war);
						if (num2 < 0f)
						{
							this.AddToCohesion(num2, TINationState.CohesionChangeReason.CohesionReason_WarEnded);
						}
					}
					this.EndWarWithSingleEnemy(actingFaction, tinationState2, true, true);
				}
			}
			IL_00E5:
			if (war.LeaveWar(this))
			{
				TINationState.EndFullWar(actingFaction, war, true, false);
			}
		}

		// Token: 0x060038F5 RID: 14581 RVA: 0x0014BE24 File Offset: 0x0014A024
		public static void EndFullWar(TIFactionState actingFaction, TIWarState war, bool forceArmyReturnCheck, bool processCohesionChange)
		{
			if (processCohesionChange)
			{
				foreach (TINationState tinationState in war.defendingAlliance)
				{
					float num = tinationState.CohesionLossFromWhitePeace(war);
					if (num < 0f)
					{
						tinationState.AddToCohesion(num, TINationState.CohesionChangeReason.CohesionReason_WarEnded);
					}
				}
			}
			foreach (TINationState tinationState2 in war.attackingAlliance)
			{
				if (processCohesionChange)
				{
					float num2 = tinationState2.CohesionLossFromWhitePeace(war);
					if (num2 < 0f)
					{
						tinationState2.AddToCohesion(num2, TINationState.CohesionChangeReason.CohesionReason_WarEnded);
					}
				}
				foreach (TINationState tinationState3 in war.defendingAlliance)
				{
					tinationState2.EndWarWithSingleEnemy(actingFaction, tinationState3, true, forceArmyReturnCheck);
				}
			}
			GameStateManager.GlobalValues().DeleteWar(war);
		}

		// Token: 0x060038F6 RID: 14582 RVA: 0x0014BF30 File Offset: 0x0014A130
		public void DeclineOffensiveWar(TINationState nation, TIWarState war)
		{
			this.DeclineImproveRelations(nation);
		}

		// Token: 0x060038F7 RID: 14583 RVA: 0x0014BF39 File Offset: 0x0014A139
		public void SortWarsList()
		{
			this.wars = this.wars.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).ToList<TINationState>();
		}

		// Token: 0x060038F8 RID: 14584 RVA: 0x0014BF70 File Offset: 0x0014A170
		public void SortAllianceList()
		{
			this.allies = this.allies.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).ToList<TINationState>();
		}

		// Token: 0x060038F9 RID: 14585 RVA: 0x0014BFA7 File Offset: 0x0014A1A7
		public void SortRivalsList()
		{
			this.rivals = this.rivals.OrderByDescending<TINationState, float>((TINationState x) => x.militaryStrength).ToList<TINationState>();
		}

		// Token: 0x060038FA RID: 14586 RVA: 0x0014BFE0 File Offset: 0x0014A1E0
		public void SetImproveRelationsCooldown(TIFactionState actingFaction, TINationState nation, int days)
		{
			if (nation == this)
			{
				return;
			}
			TIDateTime tidateTime = new TIDateTime(TITimeState.Now());
			if (actingFaction != null)
			{
				days += (int)TIEffectsState.SumEffectsModifiers(Context.BilateralRelationsCooldownMultiplier, actingFaction, (float)days, null);
			}
			tidateTime.AddDays((float)days);
			if (this.improveRelationsCooldowns.ContainsKey(nation))
			{
				if (tidateTime > this.improveRelationsCooldowns[nation])
				{
					this.improveRelationsCooldowns[nation] = tidateTime;
				}
			}
			else
			{
				this.improveRelationsCooldowns[nation] = tidateTime;
			}
			if (nation.improveRelationsCooldowns.ContainsKey(this))
			{
				if (tidateTime > nation.improveRelationsCooldowns[this])
				{
					nation.improveRelationsCooldowns[this] = tidateTime;
				}
			}
			else
			{
				nation.improveRelationsCooldowns[this] = tidateTime;
			}
			this.SetDataDirty();
			nation.SetDataDirty();
		}

		// Token: 0x060038FB RID: 14587 RVA: 0x0014C0AB File Offset: 0x0014A2AB
		public bool CanImproveRelationsYet(TINationState nation)
		{
			return !this.improveRelationsCooldowns.ContainsKey(nation) || TITimeState.Now() > this.improveRelationsCooldowns[nation];
		}

		// Token: 0x060038FC RID: 14588 RVA: 0x0014C0D3 File Offset: 0x0014A2D3
		public void DeclineImproveRelations(TINationState nation)
		{
			nation.improveRelationsDeclinedUnderCurrentExecutivePair.AddUnique(this);
			this.SetImproveRelationsCooldown(this.executiveFaction, nation, TemplateManager.global.improveRelationsCooldown_ImprovementDeclined_d);
		}

		// Token: 0x060038FD RID: 14589 RVA: 0x0014C0FC File Offset: 0x0014A2FC
		public void InitiateAlliance(TIFactionState actingFaction, TINationState newAlly)
		{
			this.EndRivalry(actingFaction, newAlly);
			if (newAlly != this)
			{
				this.AddAlly(actingFaction, newAlly, false, false);
				newAlly.AddAlly(actingFaction, this, false, false);
			}
			if (this.breakaway)
			{
				if (newAlly.allies.Contains(this.breakawayParent))
				{
					newAlly.EndAlliance(actingFaction, this.breakawayParent);
				}
				if (newAlly.CanRival(this.breakawayParent))
				{
					newAlly.InitiateRivalry(actingFaction, this.breakawayParent, false, false);
				}
			}
			if (newAlly.breakaway)
			{
				if (this.allies.Contains(newAlly.breakawayParent))
				{
					this.EndAlliance(actingFaction, newAlly.breakawayParent);
				}
				if (this.CanRival(newAlly.breakawayParent))
				{
					this.InitiateRivalry(actingFaction, newAlly.breakawayParent, false, false);
				}
			}
		}

		// Token: 0x060038FE RID: 14590 RVA: 0x0014C1BC File Offset: 0x0014A3BC
		public void EndAlliance(TIFactionState actingFaction, TINationState nation)
		{
			bool flag = this.RemoveAlly(actingFaction, nation);
			bool flag2 = nation.RemoveAlly(actingFaction, this);
			if (flag || flag2)
			{
				foreach (TIArmyState tiarmyState in this.armies)
				{
					tiarmyState.CheckAndPromptIfInIllegalRegion(true, false);
				}
				foreach (TIArmyState tiarmyState2 in nation.armies)
				{
					tiarmyState2.CheckAndPromptIfInIllegalRegion(true, false);
				}
			}
		}

		// Token: 0x060038FF RID: 14591 RVA: 0x0014C264 File Offset: 0x0014A464
		private bool AddAlly(TIFactionState actingFaction, TINationState nation, bool skipCooldown = false, bool suppressEventTrigger = false)
		{
			if (!this.allies.Contains(nation) && nation != this)
			{
				this.allies.Add(nation);
				this.SortAllianceList();
				if (!skipCooldown)
				{
					this.SetImproveRelationsCooldown(actingFaction, nation, TemplateManager.global.improveRelationsCooldown_FormAlliance_d);
				}
				if (suppressEventTrigger)
				{
					this.SetDataDirty();
					this.SetArmyAccessibilityDirty();
					GameControl.eventManager.TriggerEvent(new NationRelationsChange(this), null, new object[] { this, nation }.ToArray<object>());
				}
				return true;
			}
			return false;
		}

		// Token: 0x06003900 RID: 14592 RVA: 0x0014C2E8 File Offset: 0x0014A4E8
		private bool RemoveAlly(TIFactionState actingFaction, TINationState nation)
		{
			if (this.allies.Contains(nation))
			{
				this.SetImproveRelationsCooldown(actingFaction, nation, TemplateManager.global.improveRelationsCooldown_EndAlliance_d);
				this.allies.Remove(nation);
				this.SortAllianceList();
				this.SetDataDirty();
				this.SetArmyAccessibilityDirty();
				GameControl.eventManager.TriggerEvent(new NationRelationsChange(this), null, new object[] { this, nation }.ToArray<object>());
				return true;
			}
			return false;
		}

		// Token: 0x06003901 RID: 14593 RVA: 0x0014C35B File Offset: 0x0014A55B
		private void ClearAllies()
		{
			this.allies.Clear();
			this.SetArmyAccessibilityDirty();
		}

		// Token: 0x06003902 RID: 14594 RVA: 0x0014C36E File Offset: 0x0014A56E
		public void InitiateRivalry(TIFactionState actingFaction, TINationState nation, bool skipCooldown = false, bool skipRivalryClearDuration = false)
		{
			this.EndAlliance(actingFaction, nation);
			if (nation != this)
			{
				this.AddRival(actingFaction, nation, skipCooldown, skipRivalryClearDuration, false);
				nation.AddRival(actingFaction, this, skipCooldown, skipRivalryClearDuration, false);
			}
		}

		// Token: 0x06003903 RID: 14595 RVA: 0x0014C399 File Offset: 0x0014A599
		public void EndRivalry(TIFactionState actingFaction, TINationState nation)
		{
			this.RemoveRival(actingFaction, nation);
			nation.RemoveRival(actingFaction, this);
		}

		// Token: 0x06003904 RID: 14596 RVA: 0x0014C3AC File Offset: 0x0014A5AC
		private void AddRival(TIFactionState actingFaction, TINationState nation, bool skipCooldown = false, bool skipRivalryClearDuration = false, bool suppressEventTriggers = false)
		{
			if (!this.rivals.Contains(nation) && nation != this)
			{
				this.rivals.Add(nation);
				if (!skipCooldown)
				{
					this.SetImproveRelationsCooldown(actingFaction, nation, TemplateManager.global.improveRelationsCooldown_FormRivalry_d);
				}
				if (!this.rivalryCooldowns.ContainsKey(nation))
				{
					this.rivalryCooldowns.Add(nation, null);
				}
				TIDateTime tidateTime = TITimeState.Now();
				if (skipRivalryClearDuration)
				{
					tidateTime.AddDays(-1f);
				}
				else
				{
					tidateTime.AddDays((float)TemplateManager.global.newRivalryCohesionPenaltyWindow_d);
				}
				this.rivalryCooldowns[nation] = tidateTime;
				this.SortRivalsList();
				if (suppressEventTriggers)
				{
					GameControl.eventManager.TriggerEvent(new NationRelationsChange(this), null, new object[] { this, nation }.ToArray<object>());
					this.SetDataDirty();
				}
			}
		}

		// Token: 0x06003905 RID: 14597 RVA: 0x0014C47C File Offset: 0x0014A67C
		private void RemoveRival(TIFactionState actingFaction, TINationState nation)
		{
			if (this.rivals.Contains(nation))
			{
				this.rivals.Remove(nation);
				this.SetImproveRelationsCooldown(actingFaction, nation, TemplateManager.global.improveRelationsCooldown_EndRivalry_d);
				this.SortRivalsList();
				GameControl.eventManager.TriggerEvent(new NationRelationsChange(this), null, new object[] { this, nation }.ToArray<object>());
				this.SetDataDirty();
			}
		}

		// Token: 0x06003906 RID: 14598 RVA: 0x0014C4E6 File Offset: 0x0014A6E6
		public void UpgradeRelations(TIFactionState faction, TINationState nation)
		{
			if (this.CanAlly(nation, false))
			{
				this.InitiateAlliance(faction, nation);
				return;
			}
			if (this.CanEndRivalry(nation))
			{
				this.EndRivalry(faction, nation);
			}
		}

		// Token: 0x06003907 RID: 14599 RVA: 0x0014C50C File Offset: 0x0014A70C
		public void DowngradeRelations(TIFactionState faction, TINationState nation)
		{
			if (this.inFederation && nation.inFederation && this.federation == nation.federation && this.federation.leadNation != this)
			{
				this.federation.RemoveNation(faction, this, true);
				return;
			}
			if (this.CanEndAlliance(nation))
			{
				this.EndAlliance(faction, nation);
				return;
			}
			if (this.CanRival(nation))
			{
				this.InitiateRivalry(faction, nation, false, false);
				return;
			}
			if (this.IsRivalWith(nation))
			{
				this.DeclareLimitedWar(faction, nation);
			}
		}

		// Token: 0x06003908 RID: 14600 RVA: 0x0014C595 File Offset: 0x0014A795
		private void ClearRivals()
		{
			this.rivals.Clear();
		}

		// Token: 0x06003909 RID: 14601 RVA: 0x0014C5A4 File Offset: 0x0014A7A4
		public void SetClaim(TIRegionState region, bool fromSeizure, bool forceFromSeizure)
		{
			if (this.claims.AddUnique(region))
			{
				if (!this.alienNation && (fromSeizure || forceFromSeizure))
				{
					this.hostileClaims.AddUnique(region);
				}
				region.AddClaim(this);
				this.SetDataDirty();
				return;
			}
			if (!fromSeizure)
			{
				this.RemoveHostileClaim(region);
				return;
			}
			if (!this.alienNation && forceFromSeizure && this.hostileClaims.AddUnique(region))
			{
				this.SetDataDirty();
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x0600390A RID: 14602 RVA: 0x0014C613 File Offset: 0x0014A813
		public List<TIRegionState> nonHostileClaims
		{
			get
			{
				return this.claims.Where<TIRegionState>((TIRegionState x) => !this.ClaimWillBeHostile(x, false)).ToList<TIRegionState>();
			}
		}

		// Token: 0x0600390B RID: 14603 RVA: 0x0014C634 File Offset: 0x0014A834
		public float TotalImpactFromHostileClaims()
		{
			float num = 0f;
			for (int i = 0; i < this.regions.Count; i++)
			{
				if (this.regions[i].hostileRegion)
				{
					num += this.regions[i].populationInMillions;
				}
			}
			num /= this.population_Millions;
			return num * TemplateManager.global.maxCombinedImpactFromHostileClaims;
		}

		// Token: 0x0600390C RID: 14604 RVA: 0x0014C699 File Offset: 0x0014A899
		public bool HostileClaimDueToDemocracy(TINationState testNation)
		{
			return testNation.democracy > this.democracy + TIGlobalConfig.globalConfig.democracyDecreaseToMakeHostileClaim;
		}

		// Token: 0x0600390D RID: 14605 RVA: 0x0014C6B4 File Offset: 0x0014A8B4
		public bool ClaimWillBeHostile(TIRegionState region, bool ignoreCurrentNation = false)
		{
			return !this.alienNation && ((!ignoreCurrentNation && this.extant && this.HostileClaimDueToDemocracy(region.nation)) || this.hostileClaims.Contains(region) || !this.claims.Contains(region));
		}

		// Token: 0x0600390E RID: 14606 RVA: 0x0014C708 File Offset: 0x0014A908
		public string WillBeHostileExplanation(TIRegionState region)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.alienNation)
			{
				if (!this.claims.Contains(region))
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.HostileClaim_Reason1"));
				}
				if (this.hostileClaims.Contains(region))
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.HostileClaim_Reason2"));
				}
				if (this.extant && this.HostileClaimDueToDemocracy(region.nation))
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.HostileClaim_Reason3", new object[] { TemplateManager.global.democracyDecreaseToMakeHostileClaim.ToString("N1") }));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600390F RID: 14607 RVA: 0x0014C7B0 File Offset: 0x0014A9B0
		public void RemoveClaim(TIRegionState region)
		{
			this.claims.Remove(region);
			this.hostileClaims.Remove(region);
			region.RemoveClaim(this);
			this.canAccumulateLegitimizeClaimTriggers = this.regions.Any<TIRegionState>((TIRegionState x) => x.hostileRegion);
			this.SetDataDirty();
		}

		// Token: 0x06003910 RID: 14608 RVA: 0x0014C814 File Offset: 0x0014AA14
		public void RemoveHostileClaim(TIRegionState region)
		{
			if (this.hostileClaims.Remove(region))
			{
				this.SetDataDirty();
				this.claims.AddUnique(region);
				region.AddClaim(this);
				this.canAccumulateLegitimizeClaimTriggers = this.regions.Any<TIRegionState>((TIRegionState x) => x.hostileRegion);
			}
		}

		// Token: 0x06003911 RID: 14609 RVA: 0x0014C879 File Offset: 0x0014AA79
		public void FormFederation(TINationState nation)
		{
			TIFederationState tifederationState = GameStateManager.CreateNewGameState<TIFederationState>();
			tifederationState.Initialize();
			tifederationState.FoundFederation(this.executiveFaction, new List<TINationState> { this, nation });
			this.SetDataDirty();
		}

		// Token: 0x06003912 RID: 14610 RVA: 0x0014C8AC File Offset: 0x0014AAAC
		public bool CanFormFederation(TINationState nation)
		{
			return nation != this && !this.inFederation && this.allies.Contains(nation) && (this.HasClaimOnOtherNation(nation, true) || nation.HasClaimOnOtherNation(this, true)) && this.CanImproveRelationsYet(nation) && this.ExecutivePowerConsolidated && nation.ExecutivePowerConsolidated && !this.breakaway && !nation.breakaway;
		}

		// Token: 0x06003913 RID: 14611 RVA: 0x0014C918 File Offset: 0x0014AB18
		public string CanFormFederationFeedback(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback1", new object[] { this.displayName }), this.CanFormFederation(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback2"), !this.inFederation && !nation.inFederation));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback3"), this.allies.Contains(nation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback4"), this.HasClaimOnOtherNation(nation, true) || nation.HasClaimOnOtherNation(this, true)));
			stringBuilder.AppendLine(this.GetFeedbackLine(this.CanImproveRelationsYet(nation) ? Loc.T("UI.Nation.CanFederateFeedback5") : Loc.T("UI.Nation.CanFederateFeedback6", new object[] { this.improveRelationsCooldowns[nation].ToCustomDateString() }), this.CanImproveRelationsYet(nation)));
			bool flag = this.ExecutivePowerConsolidated && nation.ExecutivePowerConsolidated;
			if (flag)
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback7", new object[] { nation.modifiedConsolidatedExecControl_days.ToString("N0") }), flag));
			}
			else
			{
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback7a", new object[]
				{
					this.displayName,
					nation.modifiedConsolidatedExecControl_days.ToString("N0")
				}), this.ExecutivePowerConsolidated));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback7a", new object[]
				{
					nation.displayName,
					nation.modifiedConsolidatedExecControl_days.ToString("N0")
				}), nation.ExecutivePowerConsolidated));
			}
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanFederateFeedback8"), !this.breakaway && !nation.breakaway));
			return stringBuilder.ToString();
		}

		// Token: 0x06003914 RID: 14612 RVA: 0x0014CB24 File Offset: 0x0014AD24
		public static string FailingNationsPreventingFederation(TIFederationState federation, TINationState prospectiveNation)
		{
			List<TINationState> list = new List<TINationState>(federation.members);
			list.RemoveAll((TINationState x) => x.allies.Contains(prospectiveNation) || x.CanAlly(prospectiveNation, true));
			return TIUtilities.ConstructTextList(list.Select<TINationState, string>((TINationState x) => x.displayName).ToList<string>(), true, false);
		}

		// Token: 0x06003915 RID: 14613 RVA: 0x0014CB8C File Offset: 0x0014AD8C
		public string CanJoinFederationFeedback(TIFederationState federation, TINationState prospectiveNation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation1", new object[] { federation.displayName }), federation.CanAddNation(prospectiveNation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation3", new object[] { prospectiveNation.displayNameWithArticle }), !federation.memberEnemies.Contains(prospectiveNation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation2", new object[] { prospectiveNation.displayNameWithArticle }), federation.memberAllies.Contains(prospectiveNation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation4", new object[] { prospectiveNation.displayNameWithArticle }), prospectiveNation.CanImproveRelationsYet(federation.leadNation)));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation5", new object[] { prospectiveNation.displayNameWithArticle }), federation.MemberClaims(true).Any<TIRegionState>((TIRegionState x) => x.nation == prospectiveNation) || federation.members.Any<TINationState>((TINationState x) => prospectiveNation.HasClaimOnOtherNation(x, true))));
			bool flag = federation.members.All<TINationState>((TINationState x) => x.allies.Contains(prospectiveNation) || x.CanAlly(prospectiveNation, true));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation6", new object[] { prospectiveNation.displayNameWithArticle }), flag));
			if (!flag)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.CanJoinFederation6_Fail", new object[]
				{
					TIGlobalConfig.globalConfig.warningInlineSpritePath,
					TINationState.FailingNationsPreventingFederation(federation, prospectiveNation)
				}));
			}
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation7", new object[]
			{
				prospectiveNation.displayNameWithArticle,
				prospectiveNation.modifiedConsolidatedExecControl_days.ToString("N0"),
				Mathf.Max(0f, prospectiveNation.daysUntilExecutivePowerConsolidated)
			}), prospectiveNation.ExecutivePowerConsolidated));
			stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanJoinFederation8", new object[] { prospectiveNation.displayNameWithArticle }), !prospectiveNation.breakaway));
			return stringBuilder.ToString();
		}

		// Token: 0x06003916 RID: 14614 RVA: 0x0014CE1B File Offset: 0x0014B01B
		public bool CanLeaveFederation()
		{
			return this.inFederation && this.federation.leadNation != this && this.ExecutivePowerConsolidated;
		}

		// Token: 0x06003917 RID: 14615 RVA: 0x0014CE40 File Offset: 0x0014B040
		public string CanLeaveFederationFeedback()
		{
			if (this.inFederation)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanLeaveFederation1"), this.CanLeaveFederation()));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanLeaveFederation2", new object[] { this.displayNameWithArticleCapitalized }), this.federation.leadNation != this));
				stringBuilder.AppendLine(this.GetFeedbackLine(Loc.T("UI.Nation.CanLeaveFederation3", new object[]
				{
					this.modifiedConsolidatedExecControl_days.ToString("N0"),
					Mathf.Max(0f, this.daysUntilExecutivePowerConsolidated)
				}), this.ExecutivePowerConsolidated));
				if (this.federation.hegemonicFederation && this.federation.leadNation != this)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.CanLeaveFederation4", new object[] { this.federation.leadNation.displayName }));
				}
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		// Token: 0x06003918 RID: 14616 RVA: 0x0014CF5C File Offset: 0x0014B15C
		public void SetFederation(TIFactionState actingFaction, TIFederationState federationToJoin, bool starter = false, bool skipCooldown = false)
		{
			this.federation = federationToJoin;
			if (!skipCooldown)
			{
				this.federation.members.ForEach(delegate(TINationState x)
				{
					x.SetImproveRelationsCooldown(actingFaction, this, TemplateManager.global.improveRelationsCooldown_JoinFederation_d);
				});
				this.federation.members.ForEach(delegate(TINationState x)
				{
					this.SetImproveRelationsCooldown(actingFaction, x, TemplateManager.global.improveRelationsCooldown_JoinFederation_d);
				});
			}
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				ticontrolPoint.SetControlPointPriority(PriorityType.MissionControl, ticontrolPoint.GetControlPointPriority(PriorityType.MissionControl, true), false, false, false);
			}
		}

		// Token: 0x06003919 RID: 14617 RVA: 0x0014D018 File Offset: 0x0014B218
		public void LeaveFederation(TIFactionState actingFaction, bool process)
		{
			if (process)
			{
				this.federation.members.ForEach(delegate(TINationState x)
				{
					x.SetImproveRelationsCooldown(actingFaction, this, TemplateManager.global.improveRelationsCooldown_LeaveFederation_d);
				});
				this.federation.members.ForEach(delegate(TINationState x)
				{
					this.SetImproveRelationsCooldown(actingFaction, x, TemplateManager.global.improveRelationsCooldown_LeaveFederation_d);
				});
			}
			this.federation = null;
			this.PossiblePriorityValidationChange(true);
		}

		// Token: 0x0600391A RID: 14618 RVA: 0x0014D082 File Offset: 0x0014B282
		public bool CanDoFactionLevelRelationshipChange(TINationState targetNation, RelationChange change)
		{
			switch (change)
			{
			case RelationChange.NormalToAlly:
				return this.CanAlly(targetNation, false);
			case RelationChange.AllyToNormal:
				return this.CanEndAlliance(targetNation);
			case RelationChange.RivalToNormal:
				return this.CanEndRivalry(targetNation);
			case RelationChange.NormalToRival:
				return this.CanRival(targetNation);
			default:
				return false;
			}
		}

		// Token: 0x0600391B RID: 14619 RVA: 0x0014D0C0 File Offset: 0x0014B2C0
		public void HandleFactionLevelRelationshipChanges(TINationState targetNation, RelationChange change)
		{
			if (this.executiveFaction != null)
			{
				TINationState.FactionLevelRelationShipChangeCost.PayCost(this.executiveFaction, "Relationship Change");
				switch (change)
				{
				case RelationChange.NormalToAlly:
					this.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(this, this.executiveFaction, targetNation, null, new ProposeAllianceOption()));
					return;
				case RelationChange.AllyToNormal:
					this.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(this, this.executiveFaction, targetNation, null, new EndAllianceOption()));
					return;
				case RelationChange.RivalToNormal:
					this.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(this, this.executiveFaction, targetNation, null, new EndRivalryOption()));
					break;
				case RelationChange.NormalToRival:
					this.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(this, this.executiveFaction, targetNation, null, new InitiateRivalryOption()));
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x0600391C RID: 14620 RVA: 0x0014D19C File Offset: 0x0014B39C
		public void HandlePromptArmyOrderedToDepartDecision(TINationState challengingNation, ArmyOrderedToDepartOptions option, Prompt prompt)
		{
			switch (option)
			{
			case ArmyOrderedToDepartOptions.Depart:
			{
				IEnumerable<TIArmyState> armies = this.armies;
				Func<TIArmyState, bool> <>9__0;
				Func<TIArmyState, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (TIArmyState x) => x.currentNation == challengingNation);
				}
				using (IEnumerator<TIArmyState> enumerator = armies.Where<TIArmyState>(func).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIArmyState tiarmyState = enumerator.Current;
						tiarmyState.TeleportArmyFromIllegalRegion();
					}
					goto IL_0145;
				}
				break;
			}
			case ArmyOrderedToDepartOptions.OfferAlliance:
				break;
			case ArmyOrderedToDepartOptions.DeclareWar:
				goto IL_0108;
			default:
				goto IL_0145;
			}
			TINationState.FactionLevelRelationShipChangeCost.PayCost(this.executiveFaction, "Offer Alliance");
			this.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(this, this.executiveFaction, challengingNation, null, new ProposeAllianceOption()));
			using (List<TIRegionState>.Enumerator enumerator2 = challengingNation.regions.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIRegionState tiregionState = enumerator2.Current;
					GameControl.eventManager.TriggerEvent(new ForceAllArmyUpdateInRegion(tiregionState), null, new object[] { tiregionState });
				}
				goto IL_0145;
			}
			IL_0108:
			TINationState.FactionLevelRelationShipChangeCost.PayCost(this.executiveFaction, "Declare War");
			this.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(this, this.executiveFaction, challengingNation, null, new WarOption()));
			IL_0145:
			TIPromptQueueState.RemovePromptStatic(prompt);
		}

		// Token: 0x0600391D RID: 14621 RVA: 0x0014D310 File Offset: 0x0014B510
		public bool CanAllyForRemoveArmyPrompt(TINationState nationAskingForArmiesToLeave)
		{
			TIResourcesCost factionLevelRelationShipChangeCost = TINationState.FactionLevelRelationShipChangeCost;
			bool flag = this.CanAlly(nationAskingForArmiesToLeave, true);
			float num = (flag ? new ProposeAllianceOption().AIAgreeChance_Prospective(this, nationAskingForArmiesToLeave) : 0f);
			return factionLevelRelationShipChangeCost.CanAfford(this.executiveFaction, 1f, null, float.PositiveInfinity) && flag && num > 0f;
		}

		// Token: 0x0600391E RID: 14622 RVA: 0x0014D368 File Offset: 0x0014B568
		public bool CanDeclareWarForRemoveArmyPrompt(TINationState nationAskingForArmiesToLeave, bool justMadePeace)
		{
			TIResourcesCost factionLevelRelationShipChangeCost = TINationState.FactionLevelRelationShipChangeCost;
			return !justMadePeace && factionLevelRelationShipChangeCost.CanAfford(this.executiveFaction, 1f, null, float.PositiveInfinity) && !this.breakaway && this.WarCapableAllies.Intersect<TINationState>(nationAskingForArmiesToLeave.WarCapableAllies).Count<TINationState>() == 0;
		}

		// Token: 0x0600391F RID: 14623 RVA: 0x0014D3BC File Offset: 0x0014B5BC
		public List<TICouncilorState> GetCouncilorsInNation()
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TICouncilorState ticouncilorState in GameStateManager.AllFactions().SelectMany<TIFactionState, TICouncilorState>((TIFactionState x) => x.councilors))
			{
				if (ticouncilorState.currentNation == this && ticouncilorState.status == CouncilorStatus.Active)
				{
					list.Add(ticouncilorState);
				}
			}
			return list;
		}

		// Token: 0x06003920 RID: 14624 RVA: 0x0014D44C File Offset: 0x0014B64C
		public List<TICouncilorState> GetVisibleCouncilorsInNation(TIFactionState lookingFaction)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			IEnumerable<TICouncilorState> councilorsInNation = this.GetCouncilorsInNation();
			Func<TICouncilorState, bool> <>9__0;
			Func<TICouncilorState, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TICouncilorState x) => lookingFaction.HasIntelOnCouncilorLocation(x));
			}
			foreach (TICouncilorState ticouncilorState in councilorsInNation.Where<TICouncilorState>(func))
			{
				list.Add(ticouncilorState);
			}
			return list;
		}

		// Token: 0x06003921 RID: 14625 RVA: 0x0014D4D4 File Offset: 0x0014B6D4
		public void AddAdvisingCouncilor(TICouncilorState councilor)
		{
			this.advisingCouncilors.Add(councilor);
			this.SetBaseInvestmentPoints_month();
			this.SetDataDirty();
			this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Research);
			});
			this.armies.ForEach(delegate(TIArmyState x)
			{
				x.SetArmyDataDirty();
			});
		}

		// Token: 0x06003922 RID: 14626 RVA: 0x0014D550 File Offset: 0x0014B750
		public void RemoveAdvisingCouncilor(TICouncilorState councilor)
		{
			this.advisingCouncilors.Remove(councilor);
			this.SetBaseInvestmentPoints_month();
			this.SetDataDirty();
			this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Research);
			});
			this.armies.ForEach(delegate(TIArmyState x)
			{
				x.SetArmyDataDirty();
			});
		}

		// Token: 0x06003923 RID: 14627 RVA: 0x0014D5CC File Offset: 0x0014B7CC
		public void ClearAdvisingCouncilors()
		{
			if (this.advisingCouncilors.Count > 0)
			{
				this.advisingCouncilors.Clear();
				this.SetBaseInvestmentPoints_month();
				this.SetDataDirty();
				this.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(FactionResource.Research);
				});
				this.armies.ForEach(delegate(TIArmyState x)
				{
					x.SetArmyDataDirty();
				});
			}
		}

		// Token: 0x06003924 RID: 14628 RVA: 0x0014D654 File Offset: 0x0014B854
		public float GetAdvisingScore(CouncilorAttribute attribute)
		{
			float num = 0f;
			if (this.advisingCouncilors.Count > 0)
			{
				TICouncilorState[] array = (from x in this.advisingCouncilors
					where x.active
					orderby x.GetAttribute(attribute, true, true, true, false, false, false) descending
					select x).ToArray<TICouncilorState>();
				for (int i = 0; i < array.Length; i++)
				{
					num += array[i].AdvisingBonus(attribute) / (float)(i + 1);
				}
				return num;
			}
			return 0f;
		}

		// Token: 0x170009EE RID: 2542
		// (get) Token: 0x06003925 RID: 14629 RVA: 0x0014D6EE File Offset: 0x0014B8EE
		public float adviserCommandBonus
		{
			get
			{
				return this.GetAdvisingScore(CouncilorAttribute.Command);
			}
		}

		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06003926 RID: 14630 RVA: 0x0014D6F7 File Offset: 0x0014B8F7
		public float adviserScienceBonus
		{
			get
			{
				return this.GetAdvisingScore(CouncilorAttribute.Science);
			}
		}

		// Token: 0x170009F0 RID: 2544
		// (get) Token: 0x06003927 RID: 14631 RVA: 0x0014D700 File Offset: 0x0014B900
		public float adviserAdministrationBonus
		{
			get
			{
				return this.GetAdvisingScore(CouncilorAttribute.Administration);
			}
		}

		// Token: 0x06003928 RID: 14632 RVA: 0x0014D70C File Offset: 0x0014B90C
		public static void GlobalPropaganda(TIFactionIdeologyTemplate factionIdeology, float strength)
		{
			foreach (TINationState tinationState in GameStateManager.AllExtantNations())
			{
				tinationState.PropagandaOnPop(factionIdeology, strength, true);
			}
		}

		// Token: 0x06003929 RID: 14633 RVA: 0x0014D75C File Offset: 0x0014B95C
		public static void AllFactionNationsPropaganda_PerOwnedCP(TIFactionState faction, float strength)
		{
			foreach (TINationState tinationState in faction.nationsWithMyControlPoints)
			{
				tinationState.PropagandaOnPop_PerOwnedCP(faction.ideology, strength, 0, true);
			}
		}

		// Token: 0x0600392A RID: 14634 RVA: 0x0014D7B8 File Offset: 0x0014B9B8
		public void PropagandaOnPop_PerOwnedCP(TIFactionIdeologyTemplate targetIdeology, float strength, int bonusCPs = 0, bool bulk = false)
		{
			int num = this.CountFactionControlPoints(TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(targetIdeology), false, true, true) + bonusCPs;
			if (num > 0 && strength != 0f)
			{
				this.PropagandaOnPop(targetIdeology, strength * (float)num, bulk);
			}
		}

		// Token: 0x0600392B RID: 14635 RVA: 0x0014D7F4 File Offset: 0x0014B9F4
		public void PropagandaOnPop_PerOwnedCPFraction(TIFactionIdeologyTemplate targetIdeology, float strength)
		{
			int num = this.CountFactionControlPoints(TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(targetIdeology), false, true, true) / this.numControlPoints;
			if (num > 0 && strength != 0f)
			{
				this.PropagandaOnPop(targetIdeology, strength * (float)num, false);
			}
		}

		// Token: 0x0600392C RID: 14636 RVA: 0x0014D834 File Offset: 0x0014BA34
		public float PropagandaOnPop(TIFactionIdeologyTemplate targetIdeologyTemplate, float strength, bool bulkProcessing = false)
		{
			FactionIdeology factionIdeology = targetIdeologyTemplate.ideology;
			float publicOpinionOfFaction = this.GetPublicOpinionOfFaction(factionIdeology);
			if (!this.extant || this.population_Millions < 0.005f)
			{
				return 0f;
			}
			this.PropagandaOnPop(targetIdeologyTemplate.ideologyCoordinates, strength, false);
			if (targetIdeologyTemplate.alien)
			{
				factionIdeology = GameStateManager.AlienProxy().ideology.ideology;
			}
			return this.publicOpinion[factionIdeology] - publicOpinionOfFaction;
		}

		// Token: 0x170009F1 RID: 2545
		// (get) Token: 0x0600392D RID: 14637 RVA: 0x0014D89F File Offset: 0x0014BA9F
		public float singleIdeaCap
		{
			get
			{
				return (100f - this.democracy - (10f - this.cohesion)) / 100f;
			}
		}

		// Token: 0x0600392E RID: 14638 RVA: 0x0014D8C0 File Offset: 0x0014BAC0
		private void PropagandaOnPop(Vector3 targetIdeaPoint, float strength, bool bulkProcessing = false)
		{
			int num = (int)Math.Max(Mathf.Sqrt(this.population / 100f), 1f);
			Dictionary<FactionIdeology, int> dictionary = new Dictionary<FactionIdeology, int>();
			foreach (FactionIdeology factionIdeology in from x in GameStateManager.ActiveHumanIdeologies()
				select x.ideology)
			{
				dictionary.Add(factionIdeology, (int)(this.GetPublicOpinionProportion(factionIdeology) * (float)num));
			}
			Dictionary<FactionIdeology, int> dictionary2 = GameStateManager.ActiveHumanIdeologies().ToDictionary<TIFactionIdeologyTemplate, FactionIdeology, int>((TIFactionIdeologyTemplate x) => x.ideology, (TIFactionIdeologyTemplate x) => 0);
			if (strength < 0f && targetIdeaPoint == GameStateManager.AlienFaction().ideologyCoordinates)
			{
				targetIdeaPoint = TIFactionIdeologyTemplate.GetIdeologyTemplate(TINationState.GetNearestIdeology(targetIdeaPoint, false, FactionIdeology.None)).ideologyCoordinates;
			}
			FactionIdeology factionIdeology2 = FactionIdeology.None;
			if (this.publicOpinion.Values.Max() > this.singleIdeaCap)
			{
				factionIdeology2 = this.publicOpinion.Keys.MaxBy<FactionIdeology, float>((FactionIdeology x) => this.publicOpinion[x]);
			}
			foreach (FactionIdeology factionIdeology3 in from x in GameStateManager.ActiveHumanIdeologies()
				select x.ideology)
			{
				int num2;
				dictionary.TryGetValue(factionIdeology3, out num2);
				Vector3 ideologyCoordinates = TIFactionIdeologyTemplate.GetIdeologyTemplate(factionIdeology3).ideologyCoordinates;
				float ideologicalDistance = TINationState.GetIdeologicalDistance(ideologyCoordinates, targetIdeaPoint);
				for (int i = 1; i <= num2; i++)
				{
					if (strength < 0f && ideologicalDistance > 0f)
					{
						Dictionary<FactionIdeology, int> dictionary3 = dictionary2;
						FactionIdeology factionIdeology4 = factionIdeology3;
						dictionary3[factionIdeology4]++;
					}
					else
					{
						float num3 = Mathf.Abs(strength);
						float num4 = TIUtilities.RandomFloatValue() * 100f;
						float num5 = ideologicalDistance;
						if ((ideologicalDistance != 0f || strength < 0f) && num4 <= num3)
						{
							float num6 = num3 - num4;
							float num7 = 1f;
							if (ideologicalDistance == 0f)
							{
								ideologyCoordinates = new Vector3(ideologyCoordinates.x - 1f + TIUtilities.RandomRange(0f, 2f), ideologyCoordinates.y - 1f + TIUtilities.RandomRange(0f, 2f), ideologyCoordinates.z - 1f + TIUtilities.RandomRange(0f, 2f));
								num5 = Mathf.Sqrt(Mathf.Pow(targetIdeaPoint.x - ideologyCoordinates.x, 2f) + Mathf.Pow(targetIdeaPoint.y - ideologyCoordinates.y, 2f) + Mathf.Pow(targetIdeaPoint.z - ideologyCoordinates.z, 2f));
							}
							if (num6 > 10f)
							{
								num7 = Mathf.Min(2f, num5);
							}
							num7 = ((strength >= 0f) ? num7 : (-num7)) / num5;
							FactionIdeology nearestIdeology = TINationState.GetNearestIdeology(Vector3.LerpUnclamped(ideologyCoordinates, targetIdeaPoint, num7), false, factionIdeology2);
							Dictionary<FactionIdeology, int> dictionary3 = dictionary2;
							FactionIdeology factionIdeology4 = nearestIdeology;
							dictionary3[factionIdeology4]++;
						}
						else
						{
							Dictionary<FactionIdeology, int> dictionary3 = dictionary2;
							FactionIdeology factionIdeology4 = factionIdeology3;
							dictionary3[factionIdeology4]++;
						}
					}
				}
			}
			float num8 = 0f;
			foreach (FactionIdeology factionIdeology5 in from x in GameStateManager.ActiveHumanIdeologies()
				select x.ideology)
			{
				int num9 = 0;
				dictionary2.TryGetValue(factionIdeology5, out num9);
				float num10 = Mathf.Min((float)num9 / (float)num, 1f);
				num8 += num10;
				this.publicOpinion[factionIdeology5] = num10;
			}
			if (num8 < 1f)
			{
				Dictionary<FactionIdeology, float> publicOpinion = this.publicOpinion;
				publicOpinion[FactionIdeology.Undecided] = publicOpinion[FactionIdeology.Undecided] + (1f - num8);
			}
			else if (num8 > 1f)
			{
				this.publicOpinion[FactionIdeology.Undecided] = num8 - 1f;
			}
			GameStateManager.AllFactions().ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
			{
				x.SetResourceIncomeDataDirty(FactionResource.Influence);
			});
			this.SetDataDirty();
		}

		// Token: 0x0600392F RID: 14639 RVA: 0x0014DD90 File Offset: 0x0014BF90
		public float IncreaseUnrest(TIFactionState faction, float strength, bool capIncrease, TINationState.UnrestChangeReason reason)
		{
			float unrest = this.unrest;
			this.AddToUnrest(strength, reason, capIncrease ? 10f : 9.9f);
			if (this.factionUnrestAttempts.ContainsKey(faction))
			{
				Dictionary<TIFactionState, int> dictionary = this.factionUnrestAttempts;
				dictionary[faction]++;
			}
			else
			{
				this.factionUnrestAttempts.Add(faction, 1);
			}
			return this.unrest - unrest;
		}

		// Token: 0x06003930 RID: 14640 RVA: 0x0014DDFC File Offset: 0x0014BFFC
		public void StabilizeNation(TIFactionState faction, float strength, TINationState.UnrestChangeReason reason)
		{
			this.AddToUnrest(-strength, reason, 10f);
			if (this.factionUnrestAttempts.ContainsKey(faction) && this.factionUnrestAttempts[faction] > 0)
			{
				Dictionary<TIFactionState, int> dictionary = this.factionUnrestAttempts;
				dictionary[faction]--;
			}
		}

		// Token: 0x06003931 RID: 14641 RVA: 0x0014DE4D File Offset: 0x0014C04D
		public int GetCouncilUnrestAttempts(TIFactionState council)
		{
			if (this.factionUnrestAttempts.ContainsKey(council))
			{
				return this.factionUnrestAttempts[council];
			}
			return 0;
		}

		// Token: 0x06003932 RID: 14642 RVA: 0x0014DE6B File Offset: 0x0014C06B
		public void RemoveCouncilUnrestAttempts(TIFactionState faction)
		{
			if (this.factionUnrestAttempts.ContainsKey(faction))
			{
				this.factionUnrestAttempts[faction] = 0;
			}
		}

		// Token: 0x06003933 RID: 14643 RVA: 0x0014DE88 File Offset: 0x0014C088
		public void CreditCouncilUnrestAttempts(TIFactionState council)
		{
			if (this.factionUnrestAttempts[council] <= 1)
			{
				this.factionUnrestAttempts[council] = 0;
				return;
			}
			Dictionary<TIFactionState, int> dictionary = this.factionUnrestAttempts;
			dictionary[council] /= 2;
		}

		// Token: 0x06003934 RID: 14644 RVA: 0x0014DECC File Offset: 0x0014C0CC
		public TIFactionState HighestUnrestContributor()
		{
			if (GameStateManager.IterateByClass<TIFactionState>(false).Any<TIFactionState>((TIFactionState council) => this.factionUnrestAttempts.ContainsKey(council) && this.factionUnrestAttempts[council] > 0))
			{
				return this.factionUnrestAttempts.Aggregate<KeyValuePair<TIFactionState, int>>(delegate(KeyValuePair<TIFactionState, int> l, KeyValuePair<TIFactionState, int> r)
				{
					if (l.Value <= r.Value)
					{
						return r;
					}
					return l;
				}).Key;
			}
			return null;
		}

		// Token: 0x06003935 RID: 14645 RVA: 0x0014DF28 File Offset: 0x0014C128
		public List<TIPolicyOption> availableSetPolicyOptions(bool includeCancel)
		{
			List<TIPolicyOption> list = new List<TIPolicyOption>();
			if (!this.executiveControlPoint.benefitsDisabled)
			{
				IEnumerable<IPolicyOption> values = PolicyManager.policies.Values;
				Func<IPolicyOption, bool> <>9__0;
				Func<IPolicyOption, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (IPolicyOption x) => x.Allowed(this) && !x.HandledAtFactionLevel() && (includeCancel || !(x is CancelOption)));
				}
				foreach (IPolicyOption policyOption in values.Where<IPolicyOption>(func))
				{
					TIPolicyOption tipolicyOption = (TIPolicyOption)policyOption;
					list.Add(tipolicyOption);
				}
			}
			return list;
		}

		// Token: 0x06003936 RID: 14646 RVA: 0x0014DFCC File Offset: 0x0014C1CC
		public List<PolicyOptionWithTarget> AvailableSetPolicyOptionsWithTargets(bool includeCancel = false)
		{
			List<PolicyOptionWithTarget> list = new List<PolicyOptionWithTarget>();
			foreach (TIPolicyOption tipolicyOption in this.availableSetPolicyOptions(includeCancel))
			{
				if (tipolicyOption.RequiresTargets())
				{
					using (IEnumerator<TIGameState> enumerator2 = tipolicyOption.GetPossibleTargets(this).GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TIGameState tigameState = enumerator2.Current;
							list.Add(new PolicyOptionWithTarget(this, tipolicyOption, tigameState));
						}
						continue;
					}
				}
				list.Add(new PolicyOptionWithTarget(this, tipolicyOption, null));
			}
			return list;
		}

		// Token: 0x06003937 RID: 14647 RVA: 0x0014E07C File Offset: 0x0014C27C
		public void ClearRelationsCooldowns()
		{
			foreach (TINationState tinationState in new List<TINationState>(this.improveRelationsCooldowns.Keys))
			{
				tinationState.improveRelationsCooldowns.Remove(this);
				this.improveRelationsCooldowns.Remove(tinationState);
			}
		}

		// Token: 0x06003938 RID: 14648 RVA: 0x0014E0EC File Offset: 0x0014C2EC
		public List<int> NewGovernment(ControlPointChangeCause cause, TIFactionState retainingFaction = null)
		{
			TIFederationState tifederationState = this.federation;
			if (tifederationState != null)
			{
				tifederationState.RemoveNation(retainingFaction, this, true);
			}
			this.ClearRelationsCooldowns();
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				if (retainingFaction == null || ticontrolPoint.faction != retainingFaction)
				{
					this.ChangeControlPointOwner(ticontrolPoint.positionInNation, cause, null);
				}
			}
			List<int> list = new List<int>();
			foreach (TIControlPoint ticontrolPoint2 in this.controlPoints)
			{
				if (ticontrolPoint2.faction == null)
				{
					for (int i = ticontrolPoint2.positionInNation + 1; i < this.numControlPoints; i++)
					{
						if (this.GetControlPoint(i).faction != null)
						{
							list.Add(i);
							break;
						}
					}
				}
			}
			this.dateOfNewGovernment = TITimeState.Now();
			return list;
		}

		// Token: 0x06003939 RID: 14649 RVA: 0x0014E210 File Offset: 0x0014C410
		public void CheckOfferWarPostNewGovernment(bool leftFederation, TINationState federationLeader)
		{
			if (leftFederation)
			{
				TIFederationState tifederationState = this.federation;
				if (tifederationState != null && tifederationState.hegemonicFederation)
				{
					TIPromptQueueState.AddPromptStatic(federationLeader, this, null, "PromptNationLeavesDarkFederation_Violent", 0);
				}
			}
		}

		// Token: 0x0600393A RID: 14650 RVA: 0x0014E238 File Offset: 0x0014C438
		public void GrantControlPointsToUnrestingFactions(int maxToGrant, ControlPointChangeCause cause)
		{
			maxToGrant = Math.Min(maxToGrant, this.numControlPoints);
			for (int i = 0; i < maxToGrant; i++)
			{
				TIFactionState tifactionState = this.HighestUnrestContributor();
				TIFactionState tifactionState2 = tifactionState;
				if (tifactionState != null && tifactionState.IsAlienFaction)
				{
					tifactionState2 = GameStateManager.AlienProxy();
				}
				if (!(tifactionState2 != null))
				{
					break;
				}
				this.ChangeControlPointOwner(i, cause, tifactionState2);
				this.CreditCouncilUnrestAttempts(tifactionState);
			}
			this.UpdateNativeControlPointsCount();
		}

		// Token: 0x0600393B RID: 14651 RVA: 0x0014E2A0 File Offset: 0x0014C4A0
		public void GrantControlPointOfTypeByPopularity(ControlPointType CPtype, TIFactionState interestedFaction, float rigged)
		{
			List<TIGameState> controlPointOwnersByPoint = this.controlPointOwnersByPoint;
			TIControlPoint ticontrolPoint = this.GetControlPointOfType(CPtype) ?? this.controlPoints[0];
			TIFactionState faction = ticontrolPoint.faction;
			TIFactionState tifactionState = null;
			if (this.GetMostPopularFactionValue(true) == 1f)
			{
				tifactionState = TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(this.GetMostPopularIdeology(false));
				this.ChangeControlPointOwner(ticontrolPoint, ControlPointChangeCause.Politics, tifactionState);
			}
			else
			{
				Dictionary<TIFactionState, float> dictionary = new Dictionary<TIFactionState, float>();
				float num = 0f;
				foreach (KeyValuePair<FactionIdeology, float> keyValuePair in this.publicOpinion)
				{
					if (keyValuePair.Key != FactionIdeology.Undecided)
					{
						num += keyValuePair.Value;
						dictionary.Add(TIFactionIdeologyTemplate.GetFactionByIdeology(keyValuePair.Key), num);
					}
				}
				float num2 = TIUtilities.RandomRange(0f, num);
				foreach (KeyValuePair<TIFactionState, float> keyValuePair2 in dictionary)
				{
					if (num2 <= keyValuePair2.Value || (rigged > 0f && num2 <= keyValuePair2.Value + rigged && keyValuePair2.Key == interestedFaction))
					{
						tifactionState = keyValuePair2.Key;
						this.ChangeControlPointOwner(ticontrolPoint, ControlPointChangeCause.Politics, tifactionState);
						break;
					}
				}
			}
			List<TIGameState> controlPointOwnersByPoint2 = this.controlPointOwnersByPoint;
			if (faction != null)
			{
				TINotificationQueueState.LogMyControlPointPurged(faction, tifactionState, ticontrolPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint);
			}
			TINotificationQueueState.LogLoyaltySwitch(tifactionState, faction, ticontrolPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint, null);
		}

		// Token: 0x0600393C RID: 14652 RVA: 0x0014E42C File Offset: 0x0014C62C
		public void DistributeControlPointsByPopularity_Individual(TIFactionState interestedFaction, float rigged)
		{
			if (this.GetMostPopularFactionValue(true) == 1f)
			{
				TIFactionState factionByIdeologyTemplate = TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(this.GetMostPopularIdeology(false));
				for (int i = 0; i < this.numControlPoints; i++)
				{
					this.ChangeControlPointOwner(i, ControlPointChangeCause.Politics, factionByIdeologyTemplate);
				}
				return;
			}
			Dictionary<TIFactionState, float> dictionary = new Dictionary<TIFactionState, float>();
			float num = 0f;
			foreach (KeyValuePair<FactionIdeology, float> keyValuePair in this.publicOpinion)
			{
				if (keyValuePair.Key != FactionIdeology.Undecided)
				{
					num += keyValuePair.Value;
					dictionary.Add(TIFactionIdeologyTemplate.GetFactionByIdeology(keyValuePair.Key), num);
				}
			}
			for (int j = 0; j < this.numControlPoints; j++)
			{
				float num2 = TIUtilities.RandomRange(0f, num);
				foreach (KeyValuePair<TIFactionState, float> keyValuePair2 in dictionary)
				{
					if (num2 <= keyValuePair2.Value || (rigged > 0f && num2 <= keyValuePair2.Value + rigged && keyValuePair2.Key == interestedFaction))
					{
						TIFactionState key = keyValuePair2.Key;
						this.ChangeControlPointOwner(j, ControlPointChangeCause.Politics, key);
						break;
					}
				}
			}
			this.UpdateNativeControlPointsCount();
		}

		// Token: 0x0600393D RID: 14653 RVA: 0x0014E590 File Offset: 0x0014C790
		public void Revolution()
		{
			if (!this.alienNation)
			{
				TIFactionState executiveFaction = this.executiveFaction;
				List<TIGameState> controlPointOwnersByPoint = this.controlPointOwnersByPoint;
				TIFactionState tifactionState = this.HighestUnrestContributor();
				bool inFederation = this.inFederation;
				TIFederationState tifederationState = this.federation;
				float num = 0f;
				bool flag = true;
				foreach (TIControlPoint ticontrolPoint in this.controlPoints)
				{
					if (ticontrolPoint.faction != null)
					{
						num -= this.GetPublicOpinionOfFaction(ticontrolPoint.faction) * (float)(ticontrolPoint.executive ? 2 : 1) * 0.5f;
						flag = false;
					}
				}
				TIDateTime tidateTime = this.dateOfNewGovernment;
				this.NewGovernment(ControlPointChangeCause.Revolution, null);
				foreach (TIRegionState tiregionState in this.regions)
				{
					float num2 = TIUtilities.RandomRange(0f, 0.25f) * (float)((tiregionState == this.capital) ? 2 : 1);
					tiregionState.ApplyDamageToRegion(num2, null, null, true, false, false, false);
				}
				this.GrantControlPointsToUnrestingFactions(this.numControlPoints, ControlPointChangeCause.Revolution);
				foreach (TIControlPoint ticontrolPoint2 in this.controlPoints)
				{
					if (ticontrolPoint2.faction != null)
					{
						num += this.GetPublicOpinionOfFaction(ticontrolPoint2.faction) * (float)(ticontrolPoint2.executive ? 2 : 1);
						flag = false;
					}
				}
				num += TIUtilities.RandomFloatValue() * (flag ? 0.5f : 0.25f);
				this.AddToUnrest(Mathf.Clamp(-num * (0.8f + TIUtilities.RandomRange(0f, 0.4f)), -6f, -3f), TINationState.UnrestChangeReason.UnrestReason_Revolution, 10f);
				this.AddToCohesion(Mathf.Clamp(num * (0.8f + TIUtilities.RandomRange(0f, 0.4f)), -3f, 3f), TINationState.CohesionChangeReason.CohesionReason_Revolution);
				bool flag2 = tidateTime != null && TITimeState.Now().DifferenceInDays(tidateTime) < 365.2421875;
				if (this.inequality > 2.5f && !flag2)
				{
					this.AddToInequality(Mathf.Clamp(-num * ((this.inequality - 2.5f) / 2.5f) * (0.8f + TIUtilities.RandomRange(0f, 0.4f)), -3f, 0f), TINationState.InequalityChangeReason.InqReason_Revolution);
				}
				float num3 = TIUtilities.RandomRange(-3f, 1f) + TIUtilities.RandomRange(0.75f, 1.25f) * this.controlPoints.Average<TIControlPoint>(delegate(TIControlPoint x)
				{
					TIFactionState faction = x.faction;
					if (faction == null)
					{
						return 0f;
					}
					return faction.ideologyCoordinates.y;
				});
				if (flag2)
				{
					num3 = Mathf.Min(num3, 0f);
				}
				this.AddToDemocracy(num3, TINationState.DemocracyChangeReason.DemReason_Revolution);
				bool flag3 = false;
				if (TIUtilities.RandomFloatValue() < TemplateManager.global.looseNukeFromRevolutionChancePerNuke * (float)this.numNuclearWeapons)
				{
					GameStateManager.GlobalValues().ChangeLooseNukesValue(1);
					flag3 = true;
				}
				this.factionUnrestAttempts.Clear();
				if (this.executiveFaction != executiveFaction || (executiveFaction == null && this.executiveFaction == null))
				{
					foreach (TINationState tinationState in new List<TINationState>(this.allies))
					{
						if (this.CanEndAlliance(tinationState))
						{
							this.EndAlliance(tifactionState, tinationState);
							this.improveRelationsCooldowns.Remove(tinationState);
							tinationState.improveRelationsCooldowns.Remove(this);
						}
					}
				}
				TINotificationQueueState.LogRevolution(this, controlPointOwnersByPoint, flag3);
				this.CheckOfferWarPostNewGovernment(inFederation && !this.inFederation, (tifederationState != null) ? tifederationState.leadNation : null);
				return;
			}
			float num4 = this.GetPublicOpinionOfFaction(GameStateManager.AlienProxy()) * 2f + this.GetPublicOpinionOfFaction(GameStateManager.AlienAppeaser());
			this.AddToUnrest(Mathf.Clamp(-num4 * (0.8f + TIUtilities.RandomRange(0f, 0.4f)) - 2f, -6f, -3f), TINationState.UnrestChangeReason.UnrestReason_Revolution, 10f);
			this.AddToCohesion(Mathf.Clamp(num4 * (0.8f + TIUtilities.RandomRange(0f, 0.4f)), -3f, 3f), TINationState.CohesionChangeReason.CohesionReason_Revolution);
			if (this.inequality > 2.5f)
			{
				this.AddToInequality(Mathf.Clamp(-num4 * ((this.inequality - 2.5f) / 2.5f) * (0.8f + TIUtilities.RandomRange(0f, 0.4f)), -3f, 0f), TINationState.InequalityChangeReason.InqReason_Revolution);
			}
			this.AlienNationOverthrown(null, null);
		}

		// Token: 0x0600393E RID: 14654 RVA: 0x0014EA88 File Offset: 0x0014CC88
		public void ReInitializeNewNation()
		{
			this.ClearAdvisingCouncilors();
			float num = this.BaseInvestmentPoints_month();
			this.historyInvestmentPoints.Clear();
			this.historyInvestmentPoints.AddRange(Enumerable.Repeat<float>(num, 32));
			this.historyCohesion.Clear();
			this.historyCohesion.AddRange(Enumerable.Repeat<float>(this.cohesion, 32));
			this.historyCohesionRestState.Clear();
			float num2 = this.cohesionRestState;
			this.historyCohesionRestState.AddRange(Enumerable.Repeat<float>(num2, 32));
			this.historyDemocracy.Clear();
			this.historyDemocracy.AddRange(Enumerable.Repeat<float>(this.democracy, 32));
			this.historyUnrest.Clear();
			this.historyUnrest.AddRange(Enumerable.Repeat<float>(this.unrest, 32));
			this.historyUnrestRestState.Clear();
			num2 = this.unrestRestState;
			this.historyUnrestRestState.AddRange(Enumerable.Repeat<float>(num2, 32));
			this.historyInequality.Clear();
			this.historyInequality.AddRange(Enumerable.Repeat<float>(this.inequality, 32));
			this.historyGDP.Clear();
			double gdp = this.GDP;
			this.historyGDP.AddRange(Enumerable.Repeat<double>(gdp, 32));
			this.historyEducation.Clear();
			this.historyEducation.AddRange(Enumerable.Repeat<float>(this.education, 32));
			this.historySpaceFunding.Clear();
			this.historySpaceFunding.AddRange(Enumerable.Repeat<float>(this.spaceFunding_month, 32));
			this.historyMissionControl.Clear();
			this.historyMissionControl.AddRange(Enumerable.Repeat<int>(this.missionControl, 32));
			this.historyBoost.Clear();
			this.historyBoost.AddRange(Enumerable.Repeat<float>(this.currentBoost_month, 32));
			this.historyResearch.Clear();
			float research_month = this.research_month;
			this.historyResearch.AddRange(Enumerable.Repeat<float>(research_month, 32));
			this.historyMiltech.Clear();
			this.historyMiltech.AddRange(Enumerable.Repeat<float>(this.militaryTechLevel, 32));
			this.historyNukes.Clear();
			this.historyNukes.AddRange(Enumerable.Repeat<int>(this.numNuclearWeapons, 32));
			float num3 = this.AssessOverallWarStatus();
			this.historyWarStatus.Clear();
			this.historyWarStatus.AddRange(Enumerable.Repeat<float>(num3, 32));
			this.historyPopulation.Clear();
			this.historyPopulation.AddRange(Enumerable.Repeat<float>(this.population_Millions, 32));
			this.historySustainability.Clear();
			this.historySustainability.AddRange(Enumerable.Repeat<float>(this.sustainability, 32));
			this.historyNumRegions.Clear();
			this.historyNumRegions.Add(this.regions.Count);
			this.historyNumRegions.AddRange(Enumerable.Repeat<int>(0, 31));
			for (int i = 0; i < Enums.PriorityTypes.Length; i++)
			{
				PriorityType priorityType = Enums.PriorityTypes[i];
				this.SetAccumulatedInvestmentPoints(priorityType, 0f, false);
			}
		}

		// Token: 0x0600393F RID: 14655 RVA: 0x0014ED84 File Offset: 0x0014CF84
		public void AlienNationOverthrown(List<TINationState> conqueringAlliance, TIArmyState conqueringArmy)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			Dictionary<TINationState, List<TIRegionState>> dictionary = new Dictionary<TINationState, List<TIRegionState>>();
			Dictionary<TIFactionState, int> dictionary2 = new Dictionary<TIFactionState, int>(this.factionUnrestAttempts);
			TIRegionState capital = this.capital;
			int num = 0;
			using (IEnumerator<TIRegionState> enumerator = this.regions.OrderByDescending<TIRegionState, int>((TIRegionState x) => x.AdjacentRegions(false).Count<TIRegionState>()).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIRegionState region = enumerator.Current;
					if (conqueringArmy != null)
					{
						float num2 = TIUtilities.RandomRange(0f, 0.25f) * (float)((region == this.capital) ? 2 : 1);
						region.ApplyDamageToRegion(num2, null, null, true, false, false, false);
					}
					List<TINationState> list = region.NationsWithClaim(false, true, false, false);
					if (list.Count == 0)
					{
						list = region.NationsWithClaim(false, false, false, false);
						if (list.Count == 0)
						{
							list = (from x in region.AdjacentNations(false, false)
								where !x.alienNation
								select x).ToList<TINationState>();
							if (list.Count == 0)
							{
								list = GameStateManager.AllHumanNations().ToList<TINationState>();
							}
						}
					}
					if (conqueringAlliance != null && list.Intersect<TINationState>(conqueringAlliance).Count<TINationState>() > 0)
					{
						list = list.Intersect<TINationState>(conqueringAlliance).ToList<TINationState>();
					}
					IEnumerable<TINationState> enumerable = list.Where<TINationState>((TINationState x) => x.capital == region && x.claims.Count < GameStateManager.AllRegions().Length);
					TINationState tinationState = ((enumerable != null) ? enumerable.SelectRandomItem<TINationState>() : null);
					if (tinationState != null)
					{
						if (!dictionary.ContainsKey(tinationState))
						{
							dictionary[tinationState] = new List<TIRegionState>();
						}
						dictionary[tinationState].Add(region);
						num++;
					}
					else if (region.IsFullyOccupied() && list.Contains(region.leadOccupier))
					{
						if (!dictionary.ContainsKey(region.leadOccupier))
						{
							dictionary[region.leadOccupier] = new List<TIRegionState>();
						}
						dictionary[region.leadOccupier].Add(region);
						num++;
					}
					else
					{
						List<TINationState> adjacentNations = region.AdjacentNations(false, false);
						List<TINationState> list2 = list.Where<TINationState>(delegate(TINationState x)
						{
							if (x.extant && adjacentNations.Contains(x))
							{
								TIFactionState executiveFaction = x.executiveFaction;
								return executiveFaction == null || !executiveFaction.IsAlienProxy;
							}
							return false;
						}).ToList<TINationState>();
						if (list2.Count > 0)
						{
							list = list2;
						}
						TINationState tinationState2 = list.SelectRandomWeightedItem<TINationState>((TINationState x) => base.<AlienNationOverthrown>g__ScoreCandidateNation|12(x), -1f, 1E-37f);
						if (!dictionary.ContainsKey(tinationState2))
						{
							dictionary[tinationState2] = new List<TIRegionState>();
						}
						dictionary[tinationState2].Add(region);
						num++;
					}
				}
			}
			using (Dictionary<TINationState, List<TIRegionState>>.KeyCollection.Enumerator enumerator2 = dictionary.Keys.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TINationState gainingNation = enumerator2.Current;
					TIRegionState capital2 = gainingNation.capital;
					List<TINationState> list3 = ((capital2 != null) ? capital2.GetOccupyingAlliance(true) : null);
					if (dictionary[gainingNation].Count > 0)
					{
						bool flag = !gainingNation.extant;
						if (flag)
						{
							if (list3 != null && list3.Count > 0)
							{
								list3.ForEach(delegate(TINationState x)
								{
									gainingNation.InitiateAlliance(x.executiveFaction, x);
								});
							}
							else if (conqueringAlliance != null)
							{
								conqueringAlliance.ForEach(delegate(TINationState x)
								{
									gainingNation.InitiateAlliance(x.executiveFaction, x);
								});
							}
						}
						this.TransferRegionsControlTo(dictionary[gainingNation], gainingNation, true, false, false, false, false);
						if (flag)
						{
							if ((list3 == null || list3.Count == 0) && conqueringAlliance == null)
							{
								gainingNation.factionUnrestAttempts = new Dictionary<TIFactionState, int>(dictionary2);
								gainingNation.GrantControlPointsToUnrestingFactions(gainingNation.numControlPoints, ControlPointChangeCause.Revolution);
								gainingNation.factionUnrestAttempts.Clear();
								gainingNation.AddToUnrest(-5f, TINationState.UnrestChangeReason.UnrestReason_Independence, 10f);
							}
							else
							{
								foreach (TIControlPoint ticontrolPoint in gainingNation.controlPoints)
								{
									if (ticontrolPoint.executive)
									{
										if (list3 != null && list3.Count > 0)
										{
											gainingNation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Independence, list3[0].executiveFaction);
										}
										else
										{
											gainingNation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Independence, conqueringAlliance[0].executiveFaction);
										}
									}
									else if (list3 != null && list3.Count > 0)
									{
										gainingNation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Independence, list3.SelectRandomItem<TINationState>().executiveFaction);
									}
									else
									{
										gainingNation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Independence, conqueringAlliance[0].executiveFaction);
									}
								}
							}
						}
					}
				}
			}
			Dictionary<TIFactionState, float> winningFactions = new Dictionary<TIFactionState, float>();
			if (conqueringAlliance != null)
			{
				foreach (TINationState tinationState3 in conqueringAlliance)
				{
					foreach (TIControlPoint ticontrolPoint2 in tinationState3.controlPoints)
					{
						if (ticontrolPoint2.faction != null)
						{
							if (!winningFactions.ContainsKey(ticontrolPoint2.faction))
							{
								winningFactions.Add(ticontrolPoint2.faction, 0f);
							}
							Dictionary<TIFactionState, float> winningFactions2 = winningFactions;
							TIFactionState faction = ticontrolPoint2.faction;
							winningFactions2[faction] += (float)ticontrolPoint2.numArmies;
						}
					}
				}
			}
			if (conqueringAlliance == null)
			{
				winningFactions = dictionary2.ToDictionary<KeyValuePair<TIFactionState, int>, TIFactionState, float>((KeyValuePair<TIFactionState, int> x) => x.Key, (KeyValuePair<TIFactionState, int> y) => (float)y.Value);
			}
			if (winningFactions.Keys.Count > 0)
			{
				if (TIUtilities.RandomFloatValue() < 0.65f)
				{
					winningFactions.SelectRandomWeightedItem<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value, -1f, 1E-37f).Key.CompleteMilestone(CampaignMilestone.AccessAlienTech);
				}
				if (TIUtilities.RandomFloatValue() < 0.5f)
				{
					winningFactions.SelectRandomWeightedItem<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value, -1f, 1E-37f).Key.CompleteMilestone(CampaignMilestone.AccessHydraCorpus);
				}
				if (TIUtilities.RandomFloatValue() < 0.25f)
				{
					winningFactions.SelectRandomWeightedItem<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value, -1f, 1E-37f).Key.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
				}
				else if (TIUtilities.RandomFloatValue() < 0.5f)
				{
					winningFactions.SelectRandomWeightedItem<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value, -1f, 1E-37f).Key.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
				}
				if (TIUtilities.RandomFloatValue() < 0.4f)
				{
					winningFactions.SelectRandomWeightedItem<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value, -1f, 1E-37f).Key.CompleteMilestone(CampaignMilestone.AccessWarDogCorpus);
				}
				winningFactions.Keys.ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
				{
					GameStateManager.AlienFaction().GainFactionHate(x, TemplateManager.global.divisibleHateForDestroyingAlienNation / (float)winningFactions.Keys.Count, false, "Alien Nation Destroyed", true);
				});
				foreach (TIFactionState tifactionState2 in winningFactions.Keys)
				{
					tifactionState2.CompleteMilestone(CampaignMilestone.OverthrewAlienNation);
				}
			}
			if (this.regions.Count > 0 && !this.capital.nation.alienNation)
			{
				Dictionary<TIRegionState, int> capitalScores = new Dictionary<TIRegionState, int>();
				foreach (TIRegionState tiregionState in this.regions)
				{
					capitalScores.Add(tiregionState, 1);
					Dictionary<TIRegionState, int> dictionary3;
					TIRegionState tiregionState2;
					if (tiregionState.hasAlienFacility)
					{
						dictionary3 = capitalScores;
						tiregionState2 = tiregionState;
						dictionary3[tiregionState2]++;
					}
					dictionary3 = capitalScores;
					tiregionState2 = tiregionState;
					dictionary3[tiregionState2] += tiregionState.NumArmiesPresent(true, false, false, false);
					dictionary3 = capitalScores;
					tiregionState2 = tiregionState;
					dictionary3[tiregionState2] -= tiregionState.NumArmiesPresent(false, false, true, false) * 10000;
					List<TINationState> list4 = tiregionState.AdjacentNations(false, true);
					dictionary3 = capitalScores;
					tiregionState2 = tiregionState;
					dictionary3[tiregionState2] -= this.wars.Intersect<TINationState>(list4).Count<TINationState>() * 100;
					dictionary3 = capitalScores;
					tiregionState2 = tiregionState;
					dictionary3[tiregionState2] -= this.rivals.Intersect<TINationState>(list4).Count<TINationState>() * 10;
					dictionary3 = capitalScores;
					tiregionState2 = tiregionState;
					dictionary3[tiregionState2] -= list4.Count;
				}
				this.SetCapital(capitalScores.Keys.MaxBy<TIRegionState, int>((TIRegionState x) => capitalScores[x]));
				this.AddToUnrest((float)Mathf.Min(-num, -5), TINationState.UnrestChangeReason.UnrestReason_RegionBrokeAway, 10f);
			}
			if (!this.extant)
			{
				int numNuclearWeapons = this.numNuclearWeapons;
				this.ChangeNumNuclearWeapons(-numNuclearWeapons);
				List<TINationState> list5 = dictionary.Keys.Where<TINationState>((TINationState x) => x.nuclearProgram).ToList<TINationState>();
				for (int i = 0; i < numNuclearWeapons; i++)
				{
					if (TIUtilities.RandomFloatValue() < 0.8f && list5.Count > 0)
					{
						list5.SelectRandomItem<TINationState>().ChangeNumNuclearWeapons(1);
					}
					else if ((double)TIUtilities.RandomFloatValue() < 0.025)
					{
						TIGlobalValuesState.GlobalValues.ChangeLooseNukesValue(1);
					}
				}
				foreach (TINationState tinationState4 in this.allies.ToList<TINationState>())
				{
					this.EndAlliance(null, tinationState4);
				}
			}
			if (TIGlobalValuesState.IsQuietAlienCampaign() && tifactionState.GoalsOfType(GoalType.InvadeEarth, false, true).Count <= 1 && tifactionState.armiesLost[ArmyType.AlienInvader] == 0)
			{
				tifactionState.AddGoal(new FactionGoal_InvadeEarth(tifactionState, 19), HandleDuplicateGoalRule.ResetImportance, null);
			}
			if (TIGameState.Valid(conqueringArmy))
			{
				TINotificationQueueState.LogAlienNationConquered(this, conqueringArmy);
				return;
			}
			TINotificationQueueState.LogAlienNationOverthrown(this, capital);
		}

		// Token: 0x06003940 RID: 14656 RVA: 0x0014F9E0 File Offset: 0x0014DBE0
		public void Coup(TICouncilorState councilor = null, int strength = 0)
		{
			List<TIGameState> controlPointOwnersByPoint = this.controlPointOwnersByPoint;
			TIFactionState executiveFaction = this.executiveFaction;
			bool inFederation = this.inFederation;
			TIFederationState tifederationState = this.federation;
			TIFactionState tifactionState = null;
			if (councilor != null)
			{
				tifactionState = (councilor.faction.IsAlienFaction ? GameStateManager.AlienProxy() : councilor.faction);
			}
			List<int> list = this.NewGovernment(ControlPointChangeCause.Coup, tifactionState);
			this.AddToDemocracy(TIUtilities.RandomRange(-2f, 1f), TINationState.DemocracyChangeReason.DemReason_Coup);
			this.AddToUnrest(TIUtilities.RandomRange(-3f, 0f), TINationState.UnrestChangeReason.UnrestReason_Coup, 10f);
			this.AddToCohesion(TIUtilities.RandomRange(-1f, 1f), TINationState.CohesionChangeReason.CohesionReason_Coup);
			this.GDPPctChange(TIUtilities.RandomRange(0f, -0.1f), TINationState.GDPChangeReason.GDPReason_Coup);
			int num = ((list.Count > 0) ? Mathf.Clamp(list.Max() + 1, 2, this.numControlPoints) : Mathf.Min(2, this.numControlPoints));
			this.GrantControlPointsToUnrestingFactions(num, ControlPointChangeCause.Coup);
			if (councilor != null)
			{
				strength = Math.Min(strength, this.numControlPoints);
				int num2 = 0;
				TIControlPoint ticontrolPoint = this.FirstNativeControlPoint();
				if (ticontrolPoint != null)
				{
					if (this.NumNativeControlPoints < strength)
					{
						num2 = this.numControlPoints - strength;
					}
					else
					{
						num2 = ticontrolPoint.positionInNation;
					}
				}
				for (int i = 0; i < strength; i++)
				{
					if (this.controlPoints[num2 + i].faction != tifactionState)
					{
						this.ChangeControlPointOwner(num2 + i, ControlPointChangeCause.Coup, tifactionState);
					}
				}
			}
			TIControlPoint ticontrolPoint2 = this.FirstNativeControlPoint();
			if (ticontrolPoint2 != null)
			{
				foreach (TIControlPoint ticontrolPoint3 in this.controlPoints)
				{
					if (ticontrolPoint3.positionInNation > ticontrolPoint2.positionInNation && ticontrolPoint3.faction != null)
					{
						this.ChangeControlPointOwner(ticontrolPoint3.positionInNation, ControlPointChangeCause.Coup, null);
					}
				}
			}
			if (councilor != null)
			{
				TINotificationQueueState.LogCoup(this, controlPointOwnersByPoint, tifactionState);
			}
			else
			{
				TINotificationQueueState.LogCoup(this, controlPointOwnersByPoint, null);
			}
			if (this.executiveFaction != executiveFaction || (executiveFaction == null && this.executiveFaction == null))
			{
				foreach (TINationState tinationState in new List<TINationState>(this.allies))
				{
					if (this.CanEndAlliance(tinationState))
					{
						this.EndAlliance(null, tinationState);
						this.improveRelationsCooldowns.Remove(tinationState);
						tinationState.improveRelationsCooldowns.Remove(this);
					}
				}
			}
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int j = 0; j < array.Length; j++)
			{
				foreach (TICouncilorState ticouncilorState in array[j].activeCouncilors)
				{
					if (councilor != ticouncilorState && ticouncilorState.HasMission && ticouncilorState.activeMission.target == this && ticouncilorState.activeMission.missionTemplate == TIFactionState.coupMission)
					{
						ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.NationAlreadyCouped, "");
					}
				}
			}
			this.factionUnrestAttempts.Clear();
			this.CheckOfferWarPostNewGovernment(inFederation && !this.inFederation, (tifederationState != null) ? tifederationState.leadNation : null);
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x0014FD68 File Offset: 0x0014DF68
		public void RegimeChange(TINationState conqueringNation, List<TINationState> conqueringAlliance, TIArmyState conqueringArmy)
		{
			if (this.alienNation)
			{
				this.AlienNationOverthrown(conqueringAlliance, conqueringArmy);
				return;
			}
			this.RegimeChange(conqueringNation, conqueringAlliance, conqueringNation.executiveFaction, conqueringNation.executiveFaction != null && conqueringNation.executiveFaction == this.TotalOwningFaction);
		}

		// Token: 0x06003942 RID: 14658 RVA: 0x0014FDB8 File Offset: 0x0014DFB8
		public void RegimeChange(TINationState conqueringNation, List<TINationState> conqueringAlliance, TIFactionState conqueringFaction, bool suppressReporting = false)
		{
			FactionIdeology factionIdeology;
			if (conqueringFaction != null)
			{
				factionIdeology = conqueringFaction.ideology.ideology;
			}
			else
			{
				factionIdeology = conqueringNation.GetMostPopularIdeology(false).ideology;
			}
			bool inFederation = this.inFederation;
			TIFederationState tifederationState = this.federation;
			if (!this.alienNation)
			{
				TIFactionState executiveFaction = this.executiveFaction;
				List<TIGameState> controlPointOwnersByPoint = this.controlPointOwnersByPoint;
				if (conqueringFaction == null)
				{
					List<TIArmyState> enemyArmiesOnMySoil = new List<TIArmyState>();
					List<TIFactionState> list = new List<TIFactionState>();
					foreach (TIRegionState tiregionState in this.regions)
					{
						List<TIArmyState> list2 = (from x in tiregionState.FilteredArmiesPresent(false, false, true, false, false)
							where x.faction != null
							where !x.faction.IsAlienFaction
							select x).ToList<TIArmyState>();
						enemyArmiesOnMySoil.AddRange(list2);
					}
					list = enemyArmiesOnMySoil.Select<TIArmyState, TIFactionState>((TIArmyState x) => x.faction).Distinct<TIFactionState>().ToList<TIFactionState>();
					if (list.Count > 0)
					{
						conqueringFaction = list.ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, (TIFactionState x) => enemyArmiesOnMySoil.Where<TIArmyState>((TIArmyState y) => y.faction == x).Sum<TIArmyState>((TIArmyState z) => z.strength * z.adjustedTechLevel)).MaxBy<KeyValuePair<TIFactionState, float>, float>((KeyValuePair<TIFactionState, float> x) => x.Value).Key;
					}
				}
				List<TIWarState> list3 = conqueringNation.currentWarStates.Where<TIWarState>((TIWarState x) => x.EnemyAlliance(conqueringNation).Contains(this)).ToList<TIWarState>();
				foreach (TIWarState tiwarState in this.currentWarStates.Except<TIWarState>(list3))
				{
					using (List<TIWarState>.Enumerator enumerator3 = list3.ToList<TIWarState>().GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.EnemyAlliance(this).Intersect<TINationState>(tiwarState.EnemyAlliance(this)).Count<TINationState>() == tiwarState.EnemyAlliance(this).Count)
							{
								list3.Add(tiwarState);
								break;
							}
						}
					}
				}
				float num = Mathf.Max(1f, (float)this.regions.Where<TIRegionState>((TIRegionState x) => x.IsFullyOccupied()).Count<TIRegionState>()) / (float)this.regions.Count;
				List<TIWarState> list4 = new List<TIWarState>();
				foreach (TIWarState tiwarState2 in this.currentWarStates)
				{
					List<TINationState> list5 = new List<TINationState>();
					list5.AddRangeUnique<TINationState>(tiwarState2.EnemyAlliance(this).ToList<TINationState>());
					if (list5.Contains(conqueringNation))
					{
						conqueringAlliance.AddRange(list5);
					}
					foreach (TINationState tinationState in list5)
					{
						this.EndWarWithSingleEnemy(conqueringFaction, tinationState, false, false);
					}
					if (tiwarState2.LeaveWar(this))
					{
						list4.Add(tiwarState2);
					}
				}
				foreach (TIWarState tiwarState3 in list4)
				{
					TINationState.EndFullWar(conqueringFaction, tiwarState3, false, false);
				}
				conqueringAlliance = conqueringAlliance.Distinct<TINationState>().ToList<TINationState>();
				IEnumerable<TINationState> enumerable = conqueringAlliance.Where<TINationState>((TINationState x) => x.executiveFaction != null);
				TIFactionState tifactionState;
				if (enumerable == null)
				{
					tifactionState = null;
				}
				else
				{
					TINationState tinationState2 = enumerable.MaxBy<TINationState, float>((TINationState x) => x.militaryStrength);
					tifactionState = ((tinationState2 != null) ? tinationState2.executiveFaction : null);
				}
				TIFactionState tifactionState2 = tifactionState;
				this.NewGovernment(ControlPointChangeCause.RegimeChange, conqueringFaction);
				foreach (TINationState tinationState3 in new List<TINationState>(this.allies))
				{
					if (!conqueringAlliance.Contains(tinationState3))
					{
						this.EndAlliance(conqueringFaction, tinationState3);
					}
				}
				foreach (TINationState tinationState4 in new List<TINationState>(this.rivals))
				{
					if (tinationState4 == conqueringNation || conqueringAlliance.Contains(tinationState4) || conqueringNation.allies.Contains(tinationState4))
					{
						this.EndRivalry(conqueringFaction, tinationState4);
					}
				}
				foreach (TINationState tinationState5 in conqueringAlliance)
				{
					this.InitiateAlliance(tinationState5.executiveFaction, tinationState5);
					if (tinationState5.breakawayParent == this)
					{
						this.ReleaseBreakaway(tinationState5.executiveFaction, tinationState5, true);
					}
				}
				float num2 = 4f + (float)this.armies.Count - 6f * num;
				num2 += TIEffectsState.SumEffectsModifiers(Context.PostRegimeChangeUnrestReduction, conqueringFaction, num2, null);
				num2 += TIEffectsState.SumEffectsModifiers(Context.PostRegimeLossUnrestIncrease, executiveFaction, num2, null);
				if (num2 > 0f)
				{
					num2 *= 1f - this.publicOpinion[factionIdeology];
				}
				if (conqueringFaction != null)
				{
					num2 -= 0.25f * (float)this.GetCouncilUnrestAttempts(conqueringFaction);
				}
				this.AddToUnrest(TIUtilities.RandomRange(num2 - 1f, num2 + 1f), TINationState.UnrestChangeReason.UnrestReason_RegimeChange, 10f);
				this.AddToCohesion(TIUtilities.RandomRange(-3f, 0f), TINationState.CohesionChangeReason.CohesionReason_RegimeChange);
				this.AddToDemocracy(TIUtilities.RandomRange(-0.5f, 0f), TINationState.DemocracyChangeReason.DemReason_RegimeChange);
				if (this.democracy >= 5f && conqueringNation.democracy < this.democracy)
				{
					float num3 = this.democracy - conqueringNation.democracy;
					this.AddToDemocracy(TIUtilities.RandomRange(-num3 * 0.75f, -num3 * 0.25f), TINationState.DemocracyChangeReason.DemReason_RegimeChange);
				}
				this.factionUnrestAttempts.Clear();
				for (int i = 0; i <= this.maxControlPointIndex; i++)
				{
					TIFactionState tifactionState3 = conqueringFaction;
					if (this.numControlPoints > 3 && tifactionState2 != null && i == 0)
					{
						tifactionState3 = tifactionState2;
					}
					if (this.controlPoints[i].faction != tifactionState3)
					{
						this.ChangeControlPointOwner(i, ControlPointChangeCause.RegimeChange, tifactionState3);
					}
				}
				Dictionary<TINationState, List<TIRegionState>> dictionary = new Dictionary<TINationState, List<TIRegionState>>();
				using (List<TIRegionState>.Enumerator enumerator = this.regions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIRegionState region2 = enumerator.Current;
						if (!conqueringNation.regions.Contains(region2) && conqueringNation.claims.Contains(region2))
						{
							if (!dictionary.ContainsKey(conqueringNation))
							{
								dictionary.Add(conqueringNation, new List<TIRegionState>());
							}
							dictionary[conqueringNation].Add(region2);
						}
						else
						{
							IEnumerable<TINationState> enumerable2 = conqueringAlliance.Where<TINationState>((TINationState nation) => !nation.regions.Contains(region2) && nation.claims.Contains(region2));
							if (enumerable2.Any<TINationState>())
							{
								TINationState tinationState6 = enumerable2.OrderByDescending<TINationState, float>((TINationState nation) => nation.militaryStrength).First<TINationState>();
								if (!dictionary.ContainsKey(tinationState6))
								{
									dictionary.Add(tinationState6, new List<TIRegionState>());
								}
								dictionary[tinationState6].Add(region2);
							}
						}
					}
				}
				if (!dictionary.Values.Any<List<TIRegionState>>((List<TIRegionState> x) => x.Contains(this.capital)))
				{
					if (!this.regions.All<TIRegionState>((TIRegionState x) => x.colonyRegion))
					{
						goto IL_08F1;
					}
				}
				using (List<TIRegionState>.Enumerator enumerator = this.regions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TIRegionState region = enumerator.Current;
						if (region.colonyRegion && dictionary.None<KeyValuePair<TINationState, List<TIRegionState>>>((KeyValuePair<TINationState, List<TIRegionState>> x) => x.Value.Contains(region)))
						{
							List<TINationState> list6 = region.NationsWithClaim(false, true, false, true);
							if (list6.Count > 0)
							{
								TINationState tinationState7 = list6.SelectRandomItem<TINationState>();
								if (!dictionary.ContainsKey(tinationState7))
								{
									dictionary.Add(tinationState7, new List<TIRegionState>());
								}
								dictionary[tinationState7].Add(region);
							}
						}
					}
				}
				IL_08F1:
				if (dictionary.Values.Sum<List<TIRegionState>>((List<TIRegionState> x) => x.Count) < this.regions.Count)
				{
					if (!suppressReporting)
					{
						TINotificationQueueState.LogRegimeChange(this, conqueringNation, controlPointOwnersByPoint);
					}
					if (conqueringFaction != null && conqueringFaction.isActivePlayer)
					{
						conqueringFaction.UnlockAchievement("regimeChange");
					}
				}
				foreach (TINationState tinationState8 in dictionary.Keys.ToList<TINationState>())
				{
					this.TransferRegionsControlTo(dictionary[tinationState8], tinationState8, false, false, false, false, false);
				}
				foreach (TIRegionState tiregionState2 in this.regions)
				{
					foreach (TIArmyState tiarmyState in new List<TIArmyState>(tiregionState2.armies))
					{
						tiarmyState.CheckAndPromptIfInIllegalRegion(false, false);
					}
					GameControl.eventManager.TriggerEvent(new ForceAllArmyUpdateInRegion(tiregionState2), null, new object[] { tiregionState2 });
				}
				this.UpdateNativeControlPointsCount();
			}
			if (this.extant)
			{
				this.CheckOfferWarPostNewGovernment(inFederation && !this.inFederation, (tifederationState != null) ? tifederationState.leadNation : null);
			}
		}

		// Token: 0x06003943 RID: 14659 RVA: 0x00150978 File Offset: 0x0014EB78
		public float PeriodicOrganicCoupChance()
		{
			if (this.democracy >= 8f && this.unrest < 8f)
			{
				return 0f;
			}
			float num = (this.unrest * 2f - this.cohesion - this.democracy) / 10f;
			TIFactionState totalOwningFaction = this.TotalOwningFaction;
			if (!this.elitesHappy && (totalOwningFaction == null || this.GetPublicOpinionOfFaction(totalOwningFaction) < 0.65f))
			{
				num += this.corruption - this.percentWeighttoPriority(PriorityType.Spoils);
			}
			return num * 4f / 6000f;
		}

		// Token: 0x06003944 RID: 14660 RVA: 0x00150A0A File Offset: 0x0014EC0A
		public float PeriodicRevolutionChance()
		{
			if (this.unrest > TINationState.minUnrestForRevolution)
			{
				return 1f;
			}
			return 0f;
		}

		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x06003945 RID: 14661 RVA: 0x00150A24 File Offset: 0x0014EC24
		private bool CanExist
		{
			get
			{
				return this.alienNation || this.capital != null;
			}
		}

		// Token: 0x06003946 RID: 14662 RVA: 0x00150A3C File Offset: 0x0014EC3C
		public float SecessionChance(float unrestMultiplier, bool organic)
		{
			float num = this.capital.nation.unrest * unrestMultiplier - this.capital.nation.cohesion * (unrestMultiplier / 2f) - this.capital.nation.democracy;
			if (this.capital.nation.hostileClaims.Contains(this.capital))
			{
				num *= 3f;
			}
			if (this.capital.armies.Count > 0 && this.capital.nation.democracy < 8f)
			{
				num /= (float)(20 * this.capital.armies.Count);
			}
			return num * ((float)(organic ? 4 : 1) / 30000f);
		}

		// Token: 0x06003947 RID: 14663 RVA: 0x00150B00 File Offset: 0x0014ED00
		public void DailySecessionCheck()
		{
			if (!this.alienNation && this.CanExist)
			{
				TINationState parentNation = this.capital.nation;
				if (parentNation.cohesion <= TINationState.maxCohesionForSecession && parentNation.unrest >= TINationState.minUnrestForSecession && parentNation.capital != this.capital && TIUtilities.RandomFloatValue() < this.SecessionChance(0.5f, true))
				{
					List<TIRegionState> list = new List<TIRegionState> { this.capital };
					IEnumerable<TIRegionState> regions = parentNation.regions;
					Func<TIRegionState, bool> <>9__0;
					Func<TIRegionState, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (TIRegionState x) => x != parentNation.capital && x != this.capital && this.claims.Contains(x) && !this.ClaimWillBeHostile(x, false) && x.armies.Count == 0);
					}
					foreach (TIRegionState tiregionState in regions.Where<TIRegionState>(func))
					{
						if (TIUtilities.RandomFloatValue() * 100f < 3f * (parentNation.unrest * 2f - parentNation.cohesion - parentNation.democracy))
						{
							list.Add(tiregionState);
						}
					}
					parentNation.Secession(parentNation.HighestUnrestContributor(), this, list, null);
				}
			}
		}

		// Token: 0x06003948 RID: 14664 RVA: 0x00150C70 File Offset: 0x0014EE70
		public bool PostUnrestSecessionCheck(TIFactionState faction, float strength, bool forceAlien = false)
		{
			if (!this.alienNation && this.CanExist)
			{
				TINationState parentNation = this.capital.nation;
				if (parentNation.capital != this.capital)
				{
					if (faction != null && faction.completedProjects.Any<TIProjectTemplate>((TIProjectTemplate x) => x.associatedClaims.Any<TIBilateralTemplate>((TIBilateralTemplate x) => x.regionState1.templateName == this.capital.templateName)))
					{
						strength *= 3f;
					}
					if (TIUtilities.RandomFloatValue() < this.SecessionChance((float)((int)strength), false))
					{
						List<TIRegionState> list = new List<TIRegionState> { this.capital };
						IEnumerable<TIRegionState> regions = parentNation.regions;
						Func<TIRegionState, bool> func;
						Func<TIRegionState, bool> <>9__2;
						if ((func = <>9__2) == null)
						{
							func = (<>9__2 = (TIRegionState x) => x != parentNation.capital && x != this.capital && this.claims.Contains(x) && !this.ClaimWillBeHostile(x, false) && x.armies.Count == 0);
						}
						foreach (TIRegionState tiregionState in regions.Where<TIRegionState>(func))
						{
							if (TIUtilities.RandomFloatValue() * 100f < strength * (parentNation.unrest * 2f - parentNation.cohesion - parentNation.democracy) || (!this.hostileClaims.Contains(tiregionState) && parentNation.hostileClaims.Contains(tiregionState)))
							{
								list.Add(tiregionState);
							}
						}
						parentNation.Secession(faction, this, list, null);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06003949 RID: 14665 RVA: 0x00150DF0 File Offset: 0x0014EFF0
		public void PeriodicInvoluntaryRegionTransferAwayCheck()
		{
			if (this.cohesion <= TINationState.maxCohesionForSecession && this.unrest >= TINationState.minUnrestForSecession)
			{
				float num = this.SecessionChance(1f, true);
				foreach (TIRegionState tiregionState in this.regions.Where<TIRegionState>((TIRegionState x) => x != this.capital))
				{
					foreach (TINationState tinationState in from x in tiregionState.NationsWithClaim(true, true, false, false).Intersect<TINationState>(tiregionState.AdjacentNations(false, false))
						orderby x.democracy descending
						select x)
					{
						if ((!tinationState.extant || !tinationState.allies.Contains(this)) && !tinationState.ClaimWillBeHostile(tiregionState, false))
						{
							if (tinationState.alienNation)
							{
								if (tinationState.GetPublicOpinionOfFaction(GameStateManager.AlienProxy()) < 0.5f)
								{
									continue;
								}
								if (tiregionState.xenoforming.xenoformingLevel > 50f && tinationState.IsAdjacentToRegion(tiregionState, true))
								{
									num += tiregionState.xenoforming.xenoformingLevel / 10000f;
								}
							}
							if (TIUtilities.RandomFloatValue() < num)
							{
								this.Secession(this.HighestUnrestContributor(), tinationState, new List<TIRegionState> { tiregionState }, null);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x0600394A RID: 14666 RVA: 0x00150F98 File Offset: 0x0014F198
		public void Secession(TIFactionState actingFaction, TINationState newNation, List<TIRegionState> transferringRegions, TINationState liberator = null)
		{
			if (newNation.extant)
			{
				this.TransferRegionsControlTo(transferringRegions, newNation, false, false, false, true, false);
				newNation.AddToCohesion(TIUtilities.RandomFloatValue(), TINationState.CohesionChangeReason.CohesionReason_Secession);
			}
			else
			{
				transferringRegions.ForEach(delegate(TIRegionState x)
				{
					x.nation.SetClaim(x, true, true);
				});
				newNation.Independence(actingFaction, this, this, transferringRegions, false, false, liberator != null, ControlPointChangeCause.Independence);
				newNation.AddToCohesion(TIUtilities.RandomFloatValue(), TINationState.CohesionChangeReason.CohesionReason_Secession);
				newNation.unrest = TIUtilities.RandomFloatValue() * 4f;
				newNation.AddToDemocracy(-2f + TIUtilities.RandomFloatValue() * 2f + newNation.education / 2.25f, TINationState.DemocracyChangeReason.DemReason_Secession);
				newNation.AddToMilitaryTechLevel(-TIUtilities.RandomFloatValue() / 2f);
				newNation.AddToInequality(-1f + TIUtilities.RandomFloatValue() * 2f, TINationState.InequalityChangeReason.InqReason_Secession);
				newNation.GrantControlPointsToUnrestingFactions(newNation.numControlPoints, ControlPointChangeCause.Independence);
				foreach (TINationState tinationState in (from x in newNation.ExternalClaims()
					select x.nation).Distinct<TINationState>())
				{
					if (tinationState.democracy < 7f && tinationState != liberator && tinationState.CanRival(newNation))
					{
						tinationState.InitiateRivalry(actingFaction, newNation, false, false);
					}
				}
			}
			float num = transferringRegions.Sum<TIRegionState>((TIRegionState x) => x.population) / (transferringRegions.Sum<TIRegionState>((TIRegionState x) => x.population) + this.population);
			this.AddToUnrest(-(TIUtilities.RandomFloatValue() * 6f * num), TINationState.UnrestChangeReason.UnrestReason_RegionBrokeAway, 10f);
			this.AddToCohesion((1f + (TIUtilities.RandomFloatValue() + TIUtilities.RandomFloatValue())) * num, TINationState.CohesionChangeReason.CohesionReason_RegionBrokeAway);
			if (!this.rivals.Contains(newNation) && (this.democracy < 7f || liberator != null) && this.CanRival(newNation))
			{
				this.InitiateRivalry(actingFaction, newNation, false, false);
			}
		}

		// Token: 0x0600394B RID: 14667 RVA: 0x001511D4 File Offset: 0x0014F3D4
		public void ReleaseNation(TIFactionState actingFaction, TINationState newNation, bool capitalOnly)
		{
			newNation.AddToDemocracy(-2f + TIUtilities.RandomFloatValue() * 2f + newNation.education / 2.25f, TINationState.DemocracyChangeReason.DemReason_AmicableRelease);
			List<TIRegionState> list = new List<TIRegionState>();
			foreach (TIRegionState tiregionState in this.regions)
			{
				if (newNation.claims.Contains(tiregionState) && tiregionState != this.capital && (newNation.originalCapital == tiregionState || !capitalOnly))
				{
					list.Add(tiregionState);
				}
			}
			float num = list.Sum<TIRegionState>((TIRegionState x) => x.populationInMillions) / this.population_Millions;
			newNation.Independence(actingFaction, this, this, list, true, true, false, ControlPointChangeCause.Independence);
			if (this.cohesion < 5f)
			{
				this.AddToUnrest(-(TIUtilities.RandomFloatValue() * 3f * num), TINationState.UnrestChangeReason.UnrestReason_NationReleased, 10f);
				newNation.AddToUnrest(-(TIUtilities.RandomFloatValue() * 3f), TINationState.UnrestChangeReason.UnrestReason_Independence, 10f);
			}
			newNation.AddToCohesion(1f + (TIUtilities.RandomFloatValue() + TIUtilities.RandomFloatValue()), TINationState.CohesionChangeReason.CohesionReason_Independence);
			this.AddToCohesion(TIUtilities.RandomFloatValue() * 2f * num, TINationState.CohesionChangeReason.CohesionReason_NationReleased);
			foreach (TINationState tinationState in this.rivals)
			{
				if (newNation.CanRival(tinationState))
				{
					newNation.InitiateRivalry(actingFaction, tinationState, false, false);
				}
			}
			this.InitiateAlliance(actingFaction, newNation);
			IEnumerable<TINationState> allies = this.allies;
			Func<TINationState, bool> <>9__1;
			Func<TINationState, bool> func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = (TINationState x) => x != newNation);
			}
			foreach (TINationState tinationState2 in allies.Where<TINationState>(func))
			{
				if (tinationState2.CanEndRivalry(newNation))
				{
					tinationState2.EndRivalry(actingFaction, newNation);
				}
				if (tinationState2.CanAlly(newNation, false))
				{
					tinationState2.InitiateAlliance(actingFaction, newNation);
				}
			}
		}

		// Token: 0x0600394C RID: 14668 RVA: 0x00151464 File Offset: 0x0014F664
		public void SetAsBreakaway(TIFactionState actingFaction, TINationState parent)
		{
			if (parent.breakawayParent == null)
			{
				this.breakawayParent = parent;
				this.breakawayParent.breakaways.Add(this);
				if (parent.CanRival(this))
				{
					parent.InitiateRivalry(actingFaction, this, false, true);
				}
			}
			this.SetDataDirty();
		}

		// Token: 0x0600394D RID: 14669 RVA: 0x001514B0 File Offset: 0x0014F6B0
		public void ReleaseBreakaway(TIFactionState actingFaction, TINationState breakaway, bool amicable)
		{
			breakaway.breakawayParent = null;
			this.breakaways.Remove(breakaway);
			breakaway.regions.ForEach(delegate(TIRegionState x)
			{
				this.SetClaim(x, true, !amicable);
			});
			if (amicable)
			{
				if (this.CanEndRivalry(breakaway))
				{
					this.EndRivalry(actingFaction, breakaway);
					return;
				}
			}
			else if (this.ValidNewWarTarget(breakaway, false))
			{
				this.DeclareFullWar(actingFaction, breakaway);
			}
		}

		// Token: 0x0600394E RID: 14670 RVA: 0x00151528 File Offset: 0x0014F728
		public void SetCapital(TIRegionState region)
		{
			TIRegionState capital = this.capital;
			this.capital = region;
			if (region != null)
			{
				this.hostileClaims.Remove(region);
				region.colonyRegion = false;
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(region), null, new object[] { region });
			}
			if (capital != null)
			{
				GameControl.eventManager.TriggerEvent(new RegionDataUpdated(capital), null, new object[] { capital });
			}
		}

		// Token: 0x0600394F RID: 14671 RVA: 0x001515A0 File Offset: 0x0014F7A0
		private void SetNonAssignedCapital()
		{
			IOrderedEnumerable<TIRegionState> orderedEnumerable = this.regions.OrderByDescending<TIRegionState, float>((TIRegionState x) => x.populationInMillions);
			this.SetCapital(orderedEnumerable.FirstOrDefault<TIRegionState>((TIRegionState x) => !x.OccupiedOrOccupationUnderway()));
			if (this.capital == null)
			{
				this.SetCapital(orderedEnumerable.FirstOrDefault<TIRegionState>((TIRegionState x) => !x.IsFullyOccupied()));
				if (this.capital == null)
				{
					this.SetCapital(this.regions.First<TIRegionState>());
				}
			}
		}

		// Token: 0x06003950 RID: 14672 RVA: 0x0015165C File Offset: 0x0014F85C
		private void Independence(TIFactionState actingFaction, TINationState sourceNationForRegions, TINationState parentNationForStats, List<TIRegionState> regions, bool amicable, bool suppressReporting, bool actingFactionForCPs, ControlPointChangeCause cause)
		{
			if (cause == ControlPointChangeCause.Independence && !amicable)
			{
				List<TINationState> list = new List<TINationState>();
				foreach (TIRegionState tiregionState in regions)
				{
					if (tiregionState.IsFullyOccupied() && tiregionState.GetOccupyingAlliance(false).Contains(sourceNationForRegions))
					{
						list.AddRangeUnique<TINationState>(tiregionState.GetOccupyingAlliance(false));
					}
				}
				foreach (TINationState tinationState in list)
				{
					if (this.CanAlly(tinationState, false))
					{
						this.AddAlly(actingFaction, tinationState, false, false);
					}
				}
			}
			this.GDPPctChange(-(((float)parentNationForStats.numControlPoints_unclamped + 0.5f) / 100f), TINationState.GDPChangeReason.GDPReason_Independence);
			sourceNationForRegions.TransferRegionsControlTo(regions, this, false, false, true, true, true);
			this.publicOpinion = new Dictionary<FactionIdeology, float>(parentNationForStats.publicOpinion);
			this.historyPublicOpinion = new List<Dictionary<FactionIdeology, float>>(parentNationForStats.historyPublicOpinion);
			this.military = parentNationForStats.military;
			this.spaceFlightProgram = parentNationForStats.spaceFlightProgram;
			this.democracy = parentNationForStats.democracy;
			this.education = parentNationForStats.education;
			this.inequality = parentNationForStats.inequality;
			this.cohesion = parentNationForStats.cohesion;
			this.unrest = parentNationForStats.unrest;
			this.militaryTechLevel = parentNationForStats.militaryTechLevel;
			float num;
			if (this.alienNation || !parentNationForStats.alienNation)
			{
				num = parentNationForStats.maxMilitaryTechLevel;
			}
			else
			{
				num = GameStateManager.AllExtantHumanNations().Average<TINationState>((TINationState x) => x.maxMilitaryTechLevel);
			}
			this.maxMilitaryTechLevel = num;
			this.SetSustainability(parentNationForStats.sustainability, true);
			if (!amicable)
			{
				foreach (TIArmyState tiarmyState in this.armies.ToList<TIArmyState>())
				{
					if (tiarmyState.currentNation != this && tiarmyState.armyType == ArmyType.Human)
					{
						tiarmyState.Disband();
					}
				}
			}
			if (this.capital.nation != this)
			{
				this.SetNonAssignedCapital();
			}
			for (int i = 0; i < this.numControlPoints; i++)
			{
				if (!amicable)
				{
					TIFactionState tifactionState;
					if (this.alienNation)
					{
						tifactionState = GameStateManager.AlienFaction();
					}
					else
					{
						if (actingFactionForCPs)
						{
							tifactionState = actingFaction;
						}
						else
						{
							tifactionState = parentNationForStats.HighestUnrestContributor();
						}
						if (tifactionState != null && tifactionState.IsAlienFaction)
						{
							tifactionState = GameStateManager.AlienProxy();
						}
					}
					this.ChangeControlPointOwner(i, cause, tifactionState);
				}
				else if (this.alienNation)
				{
					this.ChangeControlPointOwner(i, cause, GameStateManager.AlienFaction());
				}
				else if (actingFactionForCPs)
				{
					this.ChangeControlPointOwner(i, cause, actingFaction);
				}
				else
				{
					this.ChangeControlPointOwner(i, cause, parentNationForStats.executiveFaction);
				}
			}
			int num2 = 0;
			while ((float)num2 < 10f - this.cohesion)
			{
				this.PropagandaOnPop(new Vector3((float)TIUtilities.RandomRange(-3, 3), (float)TIUtilities.RandomRange(-2, 2), 0f), TIUtilities.RandomRange(1f, 20f - this.cohesion), false);
				num2++;
			}
			if (!amicable)
			{
				if (sourceNationForRegions.claims.Contains(this.capital))
				{
					this.SetAsBreakaway(actingFaction, sourceNationForRegions);
				}
				this.SetImproveRelationsCooldown(actingFaction, sourceNationForRegions, TemplateManager.global.improveRelationsCooldown_Independence_d_nonAmicable);
				sourceNationForRegions.SetImproveRelationsCooldown(actingFaction, this, TemplateManager.global.improveRelationsCooldown_Independence_d_nonAmicable);
			}
			else
			{
				this.SetImproveRelationsCooldown(actingFaction, sourceNationForRegions, TemplateManager.global.improveRelationsCooldown_Independence_d_amicable);
				sourceNationForRegions.SetImproveRelationsCooldown(actingFaction, this, TemplateManager.global.improveRelationsCooldown_Independence_d_amicable);
			}
			foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
			{
				if (tifactionState2.IsActiveHumanFaction)
				{
					foreach (TIProjectTemplate tiprojectTemplate in TIGlobalResearchState.GetAllProjects())
					{
						if (tiprojectTemplate.requiredNationState == this && !tifactionState2.missedProjects.Contains(tiprojectTemplate.dataName))
						{
							tifactionState2.RollToAddProjectTrigger(tiprojectTemplate, null);
						}
					}
				}
				tifactionState2.ValidateAllOrgs(false);
			}
			if (!suppressReporting)
			{
				TINotificationQueueState.LogIndependence(this, parentNationForStats);
			}
		}

		// Token: 0x06003951 RID: 14673 RVA: 0x00151AB0 File Offset: 0x0014FCB0
		public void AbsorbNation(TIFactionState actingFaction, TINationState joiningNationState)
		{
			List<TIRegionState> list = new List<TIRegionState>(joiningNationState.regions);
			List<TIControlPoint> list2 = new List<TIControlPoint>();
			if (joiningNationState.extant)
			{
				list2 = joiningNationState.controlPoints.ToList<TIControlPoint>();
				joiningNationState.TransferRegionsControlTo(list, this, false, true, false, false, false);
				IEnumerable<TIControlPoint> enumerable = this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction != null);
				TIControlPoint ticontrolPoint;
				if (enumerable == null)
				{
					ticontrolPoint = null;
				}
				else
				{
					ticontrolPoint = enumerable.MaxBy<TIControlPoint, int>((TIControlPoint y) => y.positionInNation);
				}
				TIControlPoint ticontrolPoint2 = ticontrolPoint;
				if (ticontrolPoint2 != null)
				{
					foreach (TIControlPoint ticontrolPoint3 in this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction == null))
					{
						if (ticontrolPoint3.positionInNation < ticontrolPoint2.positionInNation)
						{
							this.ChangeControlPointOwner(ticontrolPoint3.positionInNation, ControlPointChangeCause.Annexation, ticontrolPoint2.faction);
						}
					}
				}
			}
			if (this.nuclearProgram)
			{
				this.ChangeNumNuclearWeapons(joiningNationState.numNuclearWeapons);
			}
			this.spaceFlightProgram = this.spaceFlightProgram || joiningNationState.spaceFlightProgram;
			if (this.inFederation && this.spaceFlightProgram)
			{
				this.federation.SetSpaceProgramValue();
			}
			this.military = this.military || joiningNationState.military;
			foreach (PriorityType priorityType in Enums.PriorityTypes)
			{
				if (this.ValidPriority(priorityType))
				{
					float num2;
					if (priorityType == PriorityType.Civilian_InitiateSpaceflightProgram || priorityType == PriorityType.Military_FoundMilitary || priorityType == PriorityType.Military_InitiateNuclearProgram)
					{
						float num = this.GetRequiredInvestmentPointsForPriority(priorityType) - this.GetAccumulatedInvestmentPoints(priorityType);
						num2 = Mathf.Min(joiningNationState.GetAccumulatedInvestmentPoints(priorityType) * 0.5f, num - 1f);
					}
					else
					{
						num2 = joiningNationState.GetAccumulatedInvestmentPoints(priorityType) * 0.5f;
					}
					if (num2 > 0f)
					{
						this.ModifyAccumulatedInvestment(priorityType, num2, false, false);
					}
				}
			}
			this.ChangeAnnualSpaceFundingValue(joiningNationState.spaceFunding_year);
			joiningNationState.ChangeAnnualSpaceFundingValue(-joiningNationState.spaceFundingIncome_year);
			this.regions.ForEach(delegate(TIRegionState x)
			{
				x.ChangeSpaceFacilityValue(SpaceFacilityType.missionControlFacility, 0f, false, false);
			});
			if (joiningNationState.democracy - 1f > this.democracy)
			{
				float num3 = (joiningNationState.democracy - this.democracy) / 2f;
				if (this.alienNation)
				{
					num3 *= 1f - this.publicOpinion[FactionIdeology.Submit];
				}
				this.AddToUnrest(num3, TINationState.UnrestChangeReason.UnrestReason_DemocracyLostInRegionTransfer, 9.8f);
			}
			joiningNationState.ChangeNumNuclearWeapons(-joiningNationState.numNuclearWeapons);
			list2.ForEach(delegate(TIControlPoint x)
			{
				joiningNationState.ChangeControlPointOwner(x, ControlPointChangeCause.Annexation, null);
			});
			joiningNationState.ClearArmies();
			joiningNationState.ClearAllies();
			joiningNationState.ClearRivals();
			foreach (TINationState tinationState in new List<TINationState>(joiningNationState.breakaways))
			{
				this.ReleaseBreakaway(actingFaction, tinationState, true);
			}
			TINationState tinationState2 = joiningNationState.breakawayParent;
			if (tinationState2 != null)
			{
				tinationState2.ReleaseBreakaway(actingFaction, joiningNationState, true);
			}
			joiningNationState.breakawayParent = null;
			foreach (TINationState tinationState3 in GameStateManager.AllExtantNations())
			{
				tinationState3.EndAlliance(actingFaction, joiningNationState);
				tinationState3.EndRivalry(actingFaction, joiningNationState);
			}
			this.SetDataDirty();
			TINationState[] array = GameStateManager.AllNations();
			for (int i = 0; i < array.Length; i++)
			{
				TINationState nation = array[i];
				IEnumerable<TINationState> enumerable2 = nation.currentWarStates.SelectMany<TIWarState, TINationState>((TIWarState x) => x.EnemyAlliance(nation));
				if (enumerable2.Count<TINationState>() != nation.wars.Count)
				{
					foreach (TINationState tinationState4 in enumerable2)
					{
						nation.SyncWarCount(tinationState4);
					}
					foreach (TINationState tinationState5 in nation.wars.ToList<TINationState>())
					{
						nation.SyncWarCount(tinationState5);
					}
				}
			}
		}

		// Token: 0x06003952 RID: 14674 RVA: 0x00151FE4 File Offset: 0x001501E4
		public void Unification(TIFactionState actingFaction, TINationState joiningNationState)
		{
			this.AbsorbNation(actingFaction, joiningNationState);
		}

		// Token: 0x06003953 RID: 14675 RVA: 0x00151FF0 File Offset: 0x001501F0
		public void SunderNation(TIFactionState actingFaction, TINationState sunderedNation, List<TIRegionState> candidateRegions, float breakawayChance, ControlPointChangeCause cause)
		{
			foreach (TIRegionState tiregionState in candidateRegions.Distinct<TIRegionState>().ToList<TIRegionState>())
			{
				List<TINationState> list = tiregionState.NationsWithClaim(false, true, false, true);
				list.Remove(GameStateManager.AlienNation());
				if (list.Count > 0 && TIUtilities.RandomFloatValue() < breakawayChance)
				{
					TINationState tinationState = list.SelectRandomItem<TINationState>();
					List<TIRegionState> list2 = new List<TIRegionState> { tiregionState };
					foreach (TIRegionState tiregionState2 in tinationState.claims)
					{
						if (this.regions.Contains(tiregionState2) && candidateRegions.Contains(tiregionState2) && !list2.Contains(tiregionState2) && tiregionState2.NationsWithClaim(false, true, false, true).Count == 0)
						{
							list2.Add(tiregionState2);
							candidateRegions.Remove(tiregionState2);
						}
					}
					tinationState.Independence(actingFaction, this, sunderedNation, new List<TIRegionState>(list2), this.democracy >= 6f, false, false, cause);
				}
			}
		}

		// Token: 0x06003954 RID: 14676 RVA: 0x00152134 File Offset: 0x00150334
		public void AnnexNation(TIFactionState actingFaction, TINationState joiningNationState, bool alienNationFounded = false)
		{
			this.AbsorbNation(actingFaction, joiningNationState);
			if (this.alienNation && alienNationFounded)
			{
				this.SetCapital((from x in this.regions
					where x.hasAlienFacility
					orderby x.population descending
					select x).First<TIRegionState>());
				foreach (TIControlPoint ticontrolPoint in this.controlPoints)
				{
					if (ticontrolPoint.faction != GameStateManager.AlienFaction())
					{
						this.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Annexation, GameStateManager.AlienFaction());
					}
				}
				foreach (TIFactionState tifactionState in from x in GameStateManager.AllHumanFactions()
					where x.proAlien
					select x)
				{
					tifactionState.CompleteMilestone(CampaignMilestone.AlienInfrastructureExists);
				}
				AIEvaluators.OnAlienNationCreated(true);
			}
		}

		// Token: 0x06003955 RID: 14677 RVA: 0x00152278 File Offset: 0x00150478
		public void TransferRegionsControlTo(List<TIRegionState> regions, TINationState newNation, bool destroyArmies, bool suppressReporting, bool forceDecolonize, bool autoTeleportArmies, bool skipOrgValidation = false)
		{
			bool flag = newNation.regions.Count == 0;
			if (flag && newNation.militaryTechLevel <= 0f)
			{
				newNation.militaryTechLevel = regions[0].nation.militaryTechLevel;
			}
			if (regions.Contains(this.capital))
			{
				regions = regions.OrderBy<TIRegionState, bool>((TIRegionState x) => x == this.capital).ToList<TIRegionState>();
			}
			Dictionary<TIRegionState, double> dictionary = regions.ToDictionary<TIRegionState, TIRegionState, double>((TIRegionState x) => x, (TIRegionState x) => x.regionalPerCapitaGDP);
			float perCapitaGDP = newNation.perCapitaGDP;
			List<TIRegionState> list = regions.Where<TIRegionState>((TIRegionState x) => newNation.ClaimWillBeHostile(x, false)).ToList<TIRegionState>();
			foreach (TIRegionState tiregionState in regions)
			{
				this.TransferRegionControlTo(tiregionState, newNation, destroyArmies, suppressReporting);
				if (list.Contains(tiregionState))
				{
					newNation.hostileClaims.AddUnique(tiregionState);
				}
				if (forceDecolonize)
				{
					tiregionState.colonyRegion = false;
				}
				else if (!flag && tiregionState.colonyRegion)
				{
					float num = perCapitaGDP * 0.6667f;
					if (dictionary[tiregionState] >= (double)num)
					{
						tiregionState.colonyRegion = false;
					}
				}
				else if (!tiregionState.colonyRegion && tiregionState.originalColony == newNation && !tiregionState.permanentlyDecolonized)
				{
					float num2 = perCapitaGDP * 0.6667f * (tiregionState.coreResourceRegion ? TIGlobalConfig.globalConfig.coreResourceRegionGDPModifier : 1f);
					if (dictionary[tiregionState] < (double)num2)
					{
						tiregionState.colonyRegion = true;
					}
				}
			}
			if (flag)
			{
				if (newNation.capital == null || !newNation.regions.Contains(newNation.capital))
				{
					newNation.SetCapital(newNation.regions.MaxBy<TIRegionState, float>((TIRegionState x) => x.populationInMillions));
				}
				newNation.ReInitializeNewNation();
			}
			this.CacheRegionValues();
			newNation.CacheRegionValues();
			this.SetDataDirty();
			newNation.SetDataDirty();
			this.GenerateAdjacentNationsDictionary();
			newNation.GenerateAdjacentNationsDictionary();
			List<TINationState> list2 = this.adjacentNations.Keys.ToList<TINationState>();
			list2.AddRange(newNation.adjacentNations.Keys);
			list2.Distinct<TINationState>().ToList<TINationState>().ForEach(delegate(TINationState x)
			{
				x.GenerateAdjacentNationsDictionary();
			});
			this.SetDisplayNameAndFlag();
			newNation.SetDisplayNameAndFlag();
			List<TIArmyState> list3 = new List<TIArmyState>(this.armies);
			if (flag)
			{
				list3.AddRange(newNation.armies);
			}
			list3.AddRange(regions.SelectMany<TIRegionState, TIArmyState>((TIRegionState x) => x.armies));
			list3 = list3.Distinct<TIArmyState>().ToList<TIArmyState>();
			foreach (TIArmyState tiarmyState in list3)
			{
				tiarmyState.CheckAndPromptIfInIllegalRegion(autoTeleportArmies, false);
				tiarmyState.SetArmyDataDirty();
			}
			if (!this.extant)
			{
				foreach (TIWarState tiwarState in new List<TIWarState>(this.currentWarStates).ToList<TIWarState>())
				{
					foreach (TIRegionState tiregionState2 in new List<TIRegionState>(tiwarState.allBelligerents.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions)))
					{
						if (tiregionState2.occupations.ContainsKey(this))
						{
							float num3 = tiregionState2.occupations[this];
							tiregionState2.occupations.Remove(this);
							if (tiwarState.allBelligerents.Contains(newNation) && newNation.wars.Contains(tiregionState2.nation))
							{
								tiregionState2.IncreaseOccupationValue(newNation, num3, null);
							}
						}
					}
				}
				List<TIWarState> list4 = new List<TIWarState>(this.warsImLeading);
				foreach (TINationState tinationState in this.wars.ToList<TINationState>())
				{
					TIWarState tiwarState2 = this.findWarsWith(tinationState).FirstOrDefault<TIWarState>();
					if (tiwarState2 != null)
					{
						if (tiwarState2.Alliance(this).Count == 1)
						{
							TINationState.EndFullWar(null, tiwarState2, true, false);
						}
						else
						{
							this.EndWarWithSingleEnemy(null, tinationState, false, false);
						}
						tiwarState2.LeaveWar(this);
						list4.Remove(tiwarState2);
					}
				}
				this.breakaways.ToList<TINationState>().ForEach(delegate(TINationState x)
				{
					this.ReleaseBreakaway(null, x, true);
				});
				foreach (TINationState tinationState2 in new List<TINationState>(this.allies))
				{
					this.EndAlliance(null, tinationState2);
				}
				foreach (TINationState tinationState3 in new List<TINationState>(this.rivals))
				{
					this.EndRivalry(null, tinationState3);
				}
				TIFederationState tifederationState = this.federation;
				if (tifederationState != null)
				{
					tifederationState.RemoveNation(null, this, false);
				}
				this.factionUnrestAttempts.Clear();
				this.ClearAdvisingCouncilors();
				this.ClearRelationsCooldowns();
			}
			if (!skipOrgValidation)
			{
				TIFactionState[] array = GameStateManager.AllFactions();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ValidateAllOrgs(false);
				}
			}
		}

		// Token: 0x06003956 RID: 14678 RVA: 0x00152900 File Offset: 0x00150B00
		private void UpdatePCGDPHistoryWithTerritoryLoss(TIRegionState movingRegion)
		{
			List<TIRegionState> list = this.regions.ToList<TIRegionState>();
			list.Remove(movingRegion);
			double num = (double)this.perCapitaGDP / Mathd.WeightedMean(list.Select<TIRegionState, double>((TIRegionState x) => x.regionalPerCapitaGDP).ToArray<double>(), list.Select<TIRegionState, double>((TIRegionState x) => (double)x.populationInMillions).ToArray<double>());
			foreach (int num2 in this.tracker_PCGDP_ByQuarter.Keys.ToList<int>())
			{
				this.tracker_PCGDP_ByQuarter[num2] = this.tracker_PCGDP_ByQuarter[num2] / (float)num;
			}
		}

		// Token: 0x06003957 RID: 14679 RVA: 0x001529E8 File Offset: 0x00150BE8
		public void UpdatePCGDPHistoryWithTerritoryGain(double movingRegionPCGDP, float movingRegionPopulationInMillions)
		{
			List<double> list = this.regions.Select<TIRegionState, double>((TIRegionState x) => x.regionalPerCapitaGDP).ToList<double>();
			list.Add(movingRegionPCGDP);
			List<double> list2 = this.regions.Select<TIRegionState, double>((TIRegionState x) => (double)x.populationInMillions).ToList<double>();
			list2.Add((double)movingRegionPopulationInMillions);
			double num = (double)this.perCapitaGDP / Mathd.WeightedMean(list.ToArray(), list2.ToArray());
			foreach (int num2 in this.tracker_PCGDP_ByQuarter.Keys.ToList<int>())
			{
				this.tracker_PCGDP_ByQuarter[num2] = this.tracker_PCGDP_ByQuarter[num2] / (float)num;
			}
		}

		// Token: 0x06003958 RID: 14680 RVA: 0x00152AE8 File Offset: 0x00150CE8
		private void TransferRegionControlTo(TIRegionState region, TINationState newNation, bool destroyArmies = true, bool suppressReporting = false)
		{
			if (newNation.regions.Contains(region))
			{
				Log.Error(newNation.displayName + " already owns " + region.displayName + ". This is a defect. Please upload savegame from which this can be reproduced to Github.", Array.Empty<object>());
				return;
			}
			List<TIArmyState> list = new List<TIArmyState>();
			int num = 0;
			foreach (TIArmyState tiarmyState in this.armies)
			{
				if (tiarmyState.armyType == ArmyType.Human && tiarmyState.homeRegion == region)
				{
					list.Add(tiarmyState);
					if (tiarmyState.deploymentType == DeploymentType.Naval)
					{
						num++;
					}
				}
			}
			if (destroyArmies)
			{
				foreach (TIArmyState tiarmyState2 in list.ToList<TIArmyState>())
				{
					if (tiarmyState2.deploymentType == DeploymentType.Naval)
					{
						num--;
					}
					tiarmyState2.Disband();
					list.Remove(tiarmyState2);
				}
				region.DestroyAllSTOFighters(false);
			}
			if (this.alienNation)
			{
				region.DestroyAllSTOFighters(false);
			}
			double nationalGDPShareValue = region.nationalGDPShareValue;
			double gdp = newNation.GDP;
			double regionalPerCapitaGDP = region.regionalPerCapitaGDP;
			if (this.regions.Count > 1)
			{
				this.UpdatePCGDPHistoryWithTerritoryLoss(region);
			}
			this.ModifyGDP(-nationalGDPShareValue, TINationState.GDPChangeReason.GDPReason_TerritoryChange);
			this.RemoveRegion(region);
			this.SetClaim(region, this.hostileClaims.Contains(region), this.hostileClaims.Contains(region));
			newNation.SetClaim(region, true, newNation.ClaimWillBeHostile(region, false));
			bool flag = false;
			if (this.regions.Count > 0 && region == this.capital)
			{
				IOrderedEnumerable<TIRegionState> orderedEnumerable = this.regions.OrderByDescending<TIRegionState, float>((TIRegionState x) => x.populationInMillions);
				this.SetCapital(orderedEnumerable.Where<TIRegionState>((TIRegionState x) => !x.IsFullyOccupied()).FirstOrDefault<TIRegionState>());
				if (this.capital == null)
				{
					this.SetCapital(this.regions.First<TIRegionState>());
					flag = true;
				}
			}
			float num2 = 0f;
			IEnumerable<TIArmyState> enumerable = newNation.armies.Where<TIArmyState>((TIArmyState x) => x.armyType == ArmyType.Human);
			int num3 = enumerable.Count<TIArmyState>();
			if (num3 > 0)
			{
				num2 = enumerable.Sum<TIArmyState>((TIArmyState x) => x.techLevel);
				num2 += (float)newNation.numNavies * newNation.militaryTechLevel;
				num3 += newNation.numNavies;
			}
			num2 += (float)newNation.regions.Count * newNation.militaryTechLevel * 2f;
			num3 += newNation.regions.Count * 2;
			int num4 = 1 + list.Count + num;
			float num5 = this.militaryTechLevel * (float)num4;
			float num6 = (num2 + num5) / (float)(num3 + num4) - newNation.militaryTechLevel;
			newNation.AddToMilitaryTechLevel(num6);
			if (newNation.extant)
			{
				newNation.UpdatePCGDPHistoryWithTerritoryGain(regionalPerCapitaGDP, region.populationInMillions);
			}
			else
			{
				newNation.tracker_PCGDP_ByQuarter[TITimeState.CurrentQuarter()] = newNation.perCapitaGDP;
			}
			newNation.AddRegion(region);
			this.SetPriorityEffectPopScaling();
			newNation.SetPriorityEffectPopScaling();
			if (region.colonyRegion || region.resourceRegion)
			{
				this.AddToInequality(-TemplateManager.global.inequalityHitFromResourceOrColonyAnnexation * this.priorityEffectPopScaling, TINationState.InequalityChangeReason.InqReason_Annexation);
				newNation.AddToInequality(TemplateManager.global.inequalityHitFromResourceOrColonyAnnexation * newNation.priorityEffectPopScaling, TINationState.InequalityChangeReason.InqReason_Annexation);
			}
			region.nation = newNation;
			if (newNation.originalCapital == region && TemplateManager.global.prohibitCapitalShenanigans)
			{
				newNation.SetCapital(region);
			}
			newNation.ModifyGDP(nationalGDPShareValue, TINationState.GDPChangeReason.GDPReason_TerritoryChange);
			float num7 = (float)(nationalGDPShareValue / newNation.GDP);
			newNation.SetSustainability((1f - num7) * newNation.sustainability + num7 * this.sustainability, true);
			float num8 = region.populationInMillions / newNation.population_Millions;
			float num9 = (1f - num8) * newNation.education + num8 * this.education;
			newNation.AddToEducation(num9 - newNation.education, TINationState.EducationChangeReason.EducationReason_RegionTransfer);
			float num10 = (1f - num8) * newNation.unrest + num8 * this.unrest;
			newNation.AddToUnrest(num10 - newNation.unrest, TINationState.UnrestChangeReason.UnrestReason_RegionTransfer, 10f);
			float num11 = (1f - num8) * newNation.cohesion + num8 * this.cohesion;
			newNation.AddToCohesion(num11 - newNation.cohesion + TemplateManager.global.cohesionHitFromRegionAnnexation, TINationState.CohesionChangeReason.CohesionReason_Annexation);
			float num12 = (1f - num8) * newNation.inequality + num8 * this.inequality;
			newNation.AddToInequality(num12 - newNation.inequality, TINationState.InequalityChangeReason.InqReason_Annexation);
			foreach (FactionIdeology factionIdeology in from x in GameStateManager.ActiveHumanIdeologies()
				select x.ideology)
			{
				newNation.publicOpinion[factionIdeology] = (1f - num8) * newNation.publicOpinion[factionIdeology] + num8 * this.publicOpinion[factionIdeology];
			}
			List<TIGameState> list2 = new List<TIGameState>(newNation.controlPointOwnersByPoint);
			this.UpdateControlPoints(null, suppressReporting);
			newNation.UpdateControlPoints(null, suppressReporting);
			GameControl.eventManager.TriggerEvent(new RegionControlChanged(region, this, newNation), null, new object[] { region });
			List<TIArmyState> list3 = new List<TIArmyState>(this.armies);
			list3.AddRange(region.armies);
			list3.AddRange(newNation.armies);
			foreach (TIArmyState tiarmyState3 in list3.Distinct<TIArmyState>().ToList<TIArmyState>())
			{
				if (tiarmyState3.homeRegion == region && tiarmyState3.armyType == ArmyType.Human)
				{
					int nextArmyControlPointIdx = newNation.GetNextArmyControlPointIdx();
					newNation.AddArmy(tiarmyState3);
					this.RemoveArmy(tiarmyState3);
					tiarmyState3.controlPointIdx = nextArmyControlPointIdx;
					TIControlPoint controlPoint = newNation.GetControlPoint(tiarmyState3.controlPointIdx);
					tiarmyState3.AssignToFaction(controlPoint.faction, false);
				}
			}
			region.ValidateAndCleanOccupations();
			GameControl.eventManager.TriggerEvent(new RegionDataUpdated(region), null, new object[] { region });
			if (!suppressReporting)
			{
				TINotificationQueueState.LogRegionChangesHands(region, this, list2);
			}
			if (flag)
			{
				List<TIRegionState> list4 = newNation.regions.Where<TIRegionState>((TIRegionState x) => this.hostileClaims.Contains(x) && newNation.ClaimWillBeHostile(x, false) && !x.IsFullyOccupied()).ToList<TIRegionState>();
				if (list4.Count > 0)
				{
					float num13 = Mathf.Max(0f, this.unrest * 0.025f);
					if (num13 > 0f)
					{
						this.SunderNation(this.capital.leadOccupier.executiveFaction, this.capital.nation, list4, num13, ControlPointChangeCause.Liberation);
					}
				}
				this.capital.nation.RegimeChange(this.capital.leadOccupier, this.capital.GetOccupyingAlliance(false), this.capital.leadOccupier.executiveFaction, this.capital.leadOccupier.executiveFaction == this.executiveFaction);
			}
			if (region.antiSpaceDefenses && !newNation.canBuildSpaceDefenses)
			{
				region.ChangeSpaceFacilityValue(SpaceFacilityType.spaceDefenseFacility, 0f, false, false);
			}
		}

		// Token: 0x06003959 RID: 14681 RVA: 0x00153374 File Offset: 0x00151574
		public List<TIRegionState> NuclearWeaponsTargets(bool targetArmiesOnly = false)
		{
			List<TIRegionState> list = new List<TIRegionState>();
			if (!targetArmiesOnly)
			{
				list = this.wars.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions).ToList<TIRegionState>();
				if (!this.spaceFlightProgram)
				{
					if (this.navalFreedom)
					{
						list = list.Where<TIRegionState>((TIRegionState x) => this.IsAdjacentToRegion(x, true) || x.onTheWater).ToList<TIRegionState>();
					}
					else
					{
						list = list.Where<TIRegionState>((TIRegionState x) => this.IsAdjacentToRegion(x, true)).ToList<TIRegionState>();
					}
				}
			}
			list.AddRange(this.wars.SelectMany<TINationState, TIRegionState>((TINationState x) => from enemyArmy in x.armies
				where enemyArmy.currentNation == this || this.allies.Contains(enemyArmy.currentNation)
				select enemyArmy into y
				select y.currentRegion));
			list = list.Where<TIRegionState>((TIRegionState x) => (this.wars.Contains(x.nation) && !x.antiSpaceDefenses) || this == x.nation || this.allies.Contains(x.nation)).ToList<TIRegionState>();
			return list.Distinct<TIRegionState>().ToList<TIRegionState>();
		}

		// Token: 0x0600395A RID: 14682 RVA: 0x00153440 File Offset: 0x00151640
		public bool AccessibleWarEnemy(TINationState potentialWar, bool skipWarEnemies)
		{
			TINationState.<>c__DisplayClass1105_0 CS$<>8__locals1 = new TINationState.<>c__DisplayClass1105_0();
			CS$<>8__locals1.potentialWar = potentialWar;
			CS$<>8__locals1.skipWarEnemies = skipWarEnemies;
			CS$<>8__locals1.<>4__this = this;
			if (this.AccessibleWarEnemyCachedNavalFreedom != this.navalFreedom)
			{
				this.SetArmyAccessibilityDirty();
			}
			bool flag;
			if (CS$<>8__locals1.skipWarEnemies && this.accessibleWarEnemy_skipWarEnemiesTRUE.TryGetValue(CS$<>8__locals1.potentialWar, out flag))
			{
				return flag;
			}
			if (!CS$<>8__locals1.skipWarEnemies && this.accessibleWarEnemy_skipWarEnemiesFALSE.TryGetValue(CS$<>8__locals1.potentialWar, out flag))
			{
				return flag;
			}
			this.AccessibleWarEnemyCachedNavalFreedom = this.navalFreedom;
			using (List<TIRegionState>.Enumerator enumerator = CS$<>8__locals1.potentialWar.regions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIRegionState region = enumerator.Current;
					if (this.armies.Any<TIArmyState>((TIArmyState army) => army.CanGetTo(region, CS$<>8__locals1.<AccessibleWarEnemy>g__GetIsRegionAllowed|0(army), null, null)))
					{
						if (CS$<>8__locals1.skipWarEnemies)
						{
							this.accessibleWarEnemy_skipWarEnemiesTRUE[CS$<>8__locals1.potentialWar] = true;
						}
						else
						{
							this.accessibleWarEnemy_skipWarEnemiesFALSE[CS$<>8__locals1.potentialWar] = true;
						}
						return true;
					}
				}
			}
			if (CS$<>8__locals1.skipWarEnemies)
			{
				this.accessibleWarEnemy_skipWarEnemiesTRUE[CS$<>8__locals1.potentialWar] = false;
			}
			else
			{
				this.accessibleWarEnemy_skipWarEnemiesFALSE[CS$<>8__locals1.potentialWar] = false;
			}
			return false;
		}

		// Token: 0x0600395B RID: 14683 RVA: 0x001535AC File Offset: 0x001517AC
		public void SetArmyAccessibilityDirty()
		{
			this.accessibleWarEnemy_skipWarEnemiesFALSE.Clear();
			this.accessibleWarEnemy_skipWarEnemiesTRUE.Clear();
		}

		// Token: 0x170009F3 RID: 2547
		// (get) Token: 0x0600395C RID: 14684 RVA: 0x001535C4 File Offset: 0x001517C4
		public float militaryStrength
		{
			get
			{
				if (TIFrameCounter.FrameCount != this.militaryStrengthCachedFrame)
				{
					this.cachedMilitaryStrength = (float)this.regions.Where<TIRegionState>((TIRegionState region) => !region.IsFullyOccupied()).Count<TIRegionState>() * this.militaryTechLevel * 0.7f;
					IEnumerable<TIArmyState> enumerable = this.armies.Where<TIArmyState>((TIArmyState x) => TIGameState.Valid(x) && !x.AlienMegafaunaArmy);
					float num = this.cachedMilitaryStrength;
					float num2 = 3f * enumerable.Sum<TIArmyState>((TIArmyState x) => x.techLevel);
					float num3;
					if (enumerable.Count<TIArmyState>() <= 0)
					{
						num3 = 0f;
					}
					else
					{
						num3 = enumerable.Average<TIArmyState>((TIArmyState x) => x.strength);
					}
					this.cachedMilitaryStrength = num + num2 * num3;
					this.cachedMilitaryStrength += (float)this.numNavies;
					this.cachedMilitaryStrength -= this.unrest;
					this.cachedMilitaryStrength += (float)((this.numNuclearWeapons > 0) ? (100 + this.numNuclearWeapons * 5) : 0);
					this.militaryStrengthCachedFrame = TIFrameCounter.FrameCount;
				}
				return this.cachedMilitaryStrength;
			}
		}

		// Token: 0x0600395D RID: 14685 RVA: 0x0015371C File Offset: 0x0015191C
		public void ArmiesDailyUpdate()
		{
			using (List<TIArmyState>.Enumerator enumerator = this.armies.ToList<TIArmyState>().Shuffle<TIArmyState>().ToList<TIArmyState>()
				.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIArmyState army = enumerator.Current;
					if (TIGameState.Valid(army))
					{
						List<TIArmyState> enemyArmiesInRegion = army.GetEnemyArmiesInRegion();
						if (enemyArmiesInRegion.Count > 0)
						{
							TIArmyState tiarmyState;
							if (enemyArmiesInRegion.Any<TIArmyState>((TIArmyState x) => x.faction != army.faction))
							{
								tiarmyState = enemyArmiesInRegion.Where<TIArmyState>((TIArmyState x) => x.faction != army.faction).SelectRandomItem<TIArmyState>();
							}
							else
							{
								tiarmyState = enemyArmiesInRegion.SelectRandomItem<TIArmyState>();
							}
							army.FireAtEnemyArmy(tiarmyState);
							if (army.AlienMegafaunaArmy)
							{
								if (!army.InFriendlyRegion)
								{
									army.EngageLocalForcesAndOccupy(true);
								}
							}
							else if (army.homeNation.wars.Contains(army.currentNation) || (army.currentRegion.IsFullyOccupied() && army.homeNation.wars.Contains(army.currentRegion.leadOccupier)))
							{
								army.EngageLocalForcesAndOccupy(true);
							}
							if (TIGameState.Valid(army) && army.HumanArmy)
							{
								TIRegionState currentRegion = army.currentRegion;
								if (((currentRegion != null) ? currentRegion.annexingArmy : null) == army)
								{
									army.currentRegion.CheckAndEndAnnexation(false);
								}
							}
						}
						else if (army.OccupyingRegion(true))
						{
							army.EngageLocalForcesAndOccupy(false);
						}
						else if (army.AlienMegafaunaArmy)
						{
							if (!army.InFriendlyRegion)
							{
								if (TIEffectsState.CheckForAnyEffectInContext(Context.MegafaunaRepellent, army.currentRegion.nation.executiveFaction))
								{
									continue;
								}
								army.EngageLocalForcesAndOccupy(false);
							}
							if (TIGameState.Valid(army) && army.CanHeal())
							{
								army.HealDamage();
							}
						}
						else
						{
							if (army.CanHeal())
							{
								army.HealDamage();
							}
							if (army.AlienRegularArmy && army.ref_alienArmyState.spawning && (army.strength >= 1f || !army.currentRegion.alienLanding.Extant()))
							{
								army.ref_alienArmyState.spawning = false;
							}
							else if (army.currentRegion.annexingArmy == army)
							{
								army.currentRegion.AnnexationDay();
							}
							else if (army.huntingXenofauna && army.CurrentOperations().Count == 0)
							{
								TIRegionState tiregionState = null;
								if (army.CanTakeOffensiveAction)
								{
									tiregionState = TIArmyState.GetArmyDestination(army, AIArmyDestination.NearestMegafaunaArmyThreat, 0);
									if (tiregionState == null)
									{
										tiregionState = TIArmyState.GetArmyDestination(army, AIArmyDestination.NearestAlienXenoformingThreat, 0);
									}
								}
								if (tiregionState == null)
								{
									tiregionState = army.homeRegion;
								}
								if (tiregionState == army.currentRegion)
								{
									if (army.currentRegion.xenoforming.VisibleToFaction(army.faction))
									{
										AssaultAlienAssetOperation assaultAlienAssetOperation = new AssaultAlienAssetOperation();
										if (assaultAlienAssetOperation.ActorCanPerformOperation(army, army.currentRegion.xenoforming))
										{
											army.ref_faction.playerControl.StartAction(new ConfirmOperationAction(army, army.currentRegion.xenoforming, assaultAlienAssetOperation, null, null));
										}
									}
								}
								else
								{
									DeployArmyOperation_OpenTarget deployArmyOperation_OpenTarget = new DeployArmyOperation_OpenTarget(false);
									if (deployArmyOperation_OpenTarget.ActorCanPerformOperation(army, tiregionState))
									{
										army.ref_faction.playerControl.StartAction(new ConfirmOperationAction(army, tiregionState, deployArmyOperation_OpenTarget, null, null));
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600395E RID: 14686 RVA: 0x00153B6C File Offset: 0x00151D6C
		public IEnumerable<TIArmyState> MegaFaunaArmiesOnSoil()
		{
			List<TIArmyState> list = new List<TIArmyState>();
			foreach (TIRegionState tiregionState in this.regions)
			{
				list.AddRange(tiregionState.MegafaunaArmiesPresent());
			}
			return list;
		}

		// Token: 0x0600395F RID: 14687 RVA: 0x00153BCC File Offset: 0x00151DCC
		public IEnumerable<TIArmyState> MegaFaunaArmiesWeShouldAttack()
		{
			List<TIArmyState> list = new List<TIArmyState>(this.MegaFaunaArmiesOnSoil());
			foreach (TINationState tinationState in this.allies)
			{
				list.AddRange(tinationState.MegaFaunaArmiesOnSoil());
			}
			return list;
		}

		// Token: 0x06003960 RID: 14688 RVA: 0x00153C34 File Offset: 0x00151E34
		public bool inSameFederation(TINationState nation)
		{
			return this.inFederation && nation.inFederation && nation.federation == this.federation;
		}

		// Token: 0x06003961 RID: 14689 RVA: 0x00153C5C File Offset: 0x00151E5C
		public int NumArmiesDefendingMe()
		{
			return this.numStandardArmies + this.allies.Where<TINationState>((TINationState x) => this.inSameFederation(x) || x.numNavies > 0 || this.AdjacentNations(false).Contains(x)).Sum<TINationState>((TINationState x) => x.numStandardArmies);
		}

		// Token: 0x06003962 RID: 14690 RVA: 0x00153CAC File Offset: 0x00151EAC
		public int NumArmiesDefendingMe(TIFactionState exceptFaction)
		{
			Func<TIArmyState, bool> <>9__3;
			return this.armies.Count<TIArmyState>(delegate(TIArmyState x)
			{
				TIFactionState faction = x.faction;
				return faction == null || !faction.permanentAlly(exceptFaction);
			}) + this.allies.Where<TINationState>((TINationState x) => this.inSameFederation(x) || x.numNavies > 0 || this.AdjacentNations(false).Contains(x)).Sum<TINationState>(delegate(TINationState x)
			{
				IEnumerable<TIArmyState> armies = x.armies;
				Func<TIArmyState, bool> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = delegate(TIArmyState x)
					{
						TIFactionState faction2 = x.faction;
						return faction2 == null || !faction2.permanentAlly(exceptFaction);
					});
				}
				return armies.Count<TIArmyState>(func);
			});
		}

		// Token: 0x06003963 RID: 14691 RVA: 0x00153D10 File Offset: 0x00151F10
		public int NumNuclearWeaponsDefendingMe()
		{
			IEnumerable<TINationState> enumerable = this.allies.Where<TINationState>((TINationState ally) => ally.armies.Count<TIArmyState>() > 0);
			return this.numNuclearWeapons + enumerable.Sum<TINationState>((TINationState x) => Mathf.Max(0, x.numNuclearWeapons - 1));
		}

		// Token: 0x06003964 RID: 14692 RVA: 0x00153D74 File Offset: 0x00151F74
		public int NumNuclearWeaponsDefendingMeAgainst(TINationState target)
		{
			IEnumerable<TIWarState> enumerable = this.currentWarStates.Where<TIWarState>((TIWarState x) => x.EnemyAlliance(this).Contains(target));
			if (enumerable.Count<TIWarState>() == 0)
			{
				return this.NumNuclearWeaponsDefendingMe();
			}
			return enumerable.SelectMany<TIWarState, TINationState>((TIWarState x) => x.Alliance(this)).Distinct<TINationState>().Sum<TINationState>((TINationState x) => x.numNuclearWeapons - ((x == this || x.numNuclearWeapons == 0) ? 0 : 1));
		}

		// Token: 0x06003965 RID: 14693 RVA: 0x00153DE4 File Offset: 0x00151FE4
		public int NumNuclearWeaponsDefendingMeInWar(TIWarState war)
		{
			return war.Alliance(this).Sum<TINationState>((TINationState x) => x.numNuclearWeapons);
		}

		// Token: 0x06003966 RID: 14694 RVA: 0x00153E11 File Offset: 0x00152011
		public int NumNuclearWeaponsThreateningMeInWars()
		{
			return this.wars.Distinct<TINationState>().Sum<TINationState>((TINationState x) => x.numNuclearWeapons);
		}

		// Token: 0x06003967 RID: 14695 RVA: 0x00153E42 File Offset: 0x00152042
		public float DefensiveAllianceMilitaryStrength()
		{
			return this.militaryStrength + this.WarCapableAllies.Sum<TINationState>((TINationState x) => x.militaryStrength);
		}

		// Token: 0x06003968 RID: 14696 RVA: 0x00153E78 File Offset: 0x00152078
		public List<TINationState> ProspectiveOffensiveAlliance(TINationState enemy, bool includeSelf = false)
		{
			List<TINationState> list = new List<TINationState>();
			if (includeSelf)
			{
				list.Add(this);
			}
			list.AddRange(this.allies.Where<TINationState>((TINationState ally) => AIEvaluators.AIWillingToJoinOffensiveAllysWar(ally, this, enemy)));
			return list;
		}

		// Token: 0x06003969 RID: 14697 RVA: 0x00153EC8 File Offset: 0x001520C8
		public float OffensiveAllianceProspectiveMilitaryStrength(TINationState enemy, bool includeSelf = false)
		{
			float num = this.militaryStrength;
			foreach (TINationState tinationState in this.ProspectiveOffensiveAlliance(enemy, includeSelf))
			{
				num += tinationState.militaryStrength;
			}
			return num;
		}

		// Token: 0x0600396A RID: 14698 RVA: 0x00153F28 File Offset: 0x00152128
		public TINationState DefensiveAllianceProspectiveWarLeader()
		{
			return this.WarCapableAllies.Append(this).MaxBy<TINationState, float>((TINationState x) => x.militaryStrength);
		}

		// Token: 0x0600396B RID: 14699 RVA: 0x00153F5A File Offset: 0x0015215A
		public List<TIArmyState> CurrentWarAllianceArmies(TINationState enemy)
		{
			return this.CurrentWarAllies(enemy, true).SelectMany<TINationState, TIArmyState>(delegate(TINationState x)
			{
				if (!this.alienNation)
				{
					return this.armies;
				}
				return this.armies.Where<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy);
			}).ToList<TIArmyState>();
		}

		// Token: 0x0600396C RID: 14700 RVA: 0x00153F7A File Offset: 0x0015217A
		public int iCurrentWarAllianceArmies(TINationState enemy)
		{
			return this.CurrentWarAllies(enemy, true).Sum<TINationState>((TINationState x) => x.numStandardArmies);
		}

		// Token: 0x0600396D RID: 14701 RVA: 0x00153FA8 File Offset: 0x001521A8
		public List<TINationState> CurrentWarAllies(TINationState enemy, bool includeSelf)
		{
			List<TINationState> list = new List<TINationState>();
			if (this.wars.Contains(enemy))
			{
				foreach (TIWarState tiwarState in this.currentWarStates)
				{
					list.AddRangeUnique<TINationState>(tiwarState.Alliance(this).ToList<TINationState>());
				}
				if (!includeSelf)
				{
					list.Remove(this);
				}
			}
			return list;
		}

		// Token: 0x0600396E RID: 14702 RVA: 0x00154028 File Offset: 0x00152228
		public List<TINationState> CurrentWarAllies_AllWars()
		{
			return this.currentWarStates.SelectMany<TIWarState, TINationState>((TIWarState x) => x.Alliance(this)).Distinct<TINationState>().ToList<TINationState>();
		}

		// Token: 0x0600396F RID: 14703 RVA: 0x0015404C File Offset: 0x0015224C
		public static float CurrentWarAllianceMilitaryStrength(TINationState baseNation, TINationState enemy)
		{
			float num = baseNation.militaryStrength;
			IEnumerable<TINationState> allies = baseNation.allies;
			Func<TINationState, bool> <>9__0;
			Func<TINationState, bool> func;
			if ((func = <>9__0) == null)
			{
				func = (<>9__0 = (TINationState ally) => ally.wars.Contains(enemy));
			}
			foreach (TINationState tinationState in allies.Where<TINationState>(func))
			{
				num += tinationState.militaryStrength;
			}
			return num;
		}

		// Token: 0x06003970 RID: 14704 RVA: 0x001540D8 File Offset: 0x001522D8
		public bool WinningWarAgainst(TINationState enemy)
		{
			TIRegionState capital = this.capital;
			if (capital != null && capital.OccupiedOrOccupationUnderway() && this.regions.Contains(this.capital))
			{
				return false;
			}
			TIRegionState capital2 = enemy.capital;
			return (capital2 != null && capital2.OccupiedOrOccupationUnderway()) || this.WinningWarBy(enemy) > 0f;
		}

		// Token: 0x06003971 RID: 14705 RVA: 0x00154134 File Offset: 0x00152334
		public int EnemyArmiesOnMyTerritory_NoMegafauna()
		{
			List<TIArmyState> list = this.wars.SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies).ToList<TIArmyState>();
			int num = 0;
			foreach (TIArmyState tiarmyState in list)
			{
				if (tiarmyState.currentNation == this)
				{
					num++;
				}
				else if (this.allies.Contains(tiarmyState.currentNation))
				{
					num++;
				}
				else if (tiarmyState.CurrentOperations().Count > 0 && this.regions.Contains(tiarmyState.CurrentOperations()[0].target))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06003972 RID: 14706 RVA: 0x0015420C File Offset: 0x0015240C
		public int ArmiesThreateningCapital(bool includeThoseinBattleWithArmies, bool capitalOnly = false)
		{
			TINationState.<>c__DisplayClass1132_0 CS$<>8__locals1 = new TINationState.<>c__DisplayClass1132_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.includeThoseinBattleWithArmies = includeThoseinBattleWithArmies;
			int num = 0;
			if (this.capital != null)
			{
				List<TIArmyState> list = this.wars.SelectMany<TINationState, TIArmyState>((TINationState x) => x.armies).ToList<TIArmyState>();
				num += list.Count<TIArmyState>((TIArmyState x) => x.currentRegion == CS$<>8__locals1.<>4__this.capital && (CS$<>8__locals1.includeThoseinBattleWithArmies || !x.InBattleWithArmies()));
				if (!capitalOnly)
				{
					TINationState.<>c__DisplayClass1132_1 CS$<>8__locals2 = new TINationState.<>c__DisplayClass1132_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					TINationState.<>c__DisplayClass1132_1 CS$<>8__locals3 = CS$<>8__locals2;
					TIRegionState capital = this.capital;
					CS$<>8__locals3.adjacentCapitalRegions = ((capital != null) ? capital.AdjacentRegions(true) : null);
					if (CS$<>8__locals2.adjacentCapitalRegions != null)
					{
						num += list.Count<TIArmyState>(delegate(TIArmyState x)
						{
							if (CS$<>8__locals2.adjacentCapitalRegions.Contains(x.currentRegion) && (CS$<>8__locals2.CS$<>8__locals1.includeThoseinBattleWithArmies || !x.InBattleWithArmies()))
							{
								IEnumerable<OperationData> enumerable = x.CurrentOperations();
								Func<OperationData, bool> func;
								if ((func = CS$<>8__locals2.CS$<>8__locals1.<>9__4) == null)
								{
									func = (CS$<>8__locals2.CS$<>8__locals1.<>9__4 = (OperationData x) => x.target == CS$<>8__locals2.CS$<>8__locals1.<>4__this.capital);
								}
								return enumerable.None<OperationData>(func);
							}
							return false;
						});
					}
					num += list.Count<TIArmyState>((TIArmyState x) => x.CurrentOperations().Count > 0 && x.CurrentOperations()[0].target == CS$<>8__locals2.CS$<>8__locals1.<>4__this.capital);
				}
			}
			return num;
		}

		// Token: 0x06003973 RID: 14707 RVA: 0x001542E8 File Offset: 0x001524E8
		public float WinningWarBy(TINationState enemy)
		{
			float num = TINationState.CurrentWarAllianceMilitaryStrength(this, enemy);
			float num2 = TINationState.CurrentWarAllianceMilitaryStrength(enemy, this);
			return num - num2;
		}

		// Token: 0x06003974 RID: 14708 RVA: 0x00154306 File Offset: 0x00152506
		public float AssessOverallWarStatus()
		{
			return this.currentWarStates.Sum<TIWarState>((TIWarState x) => this.WinningWarBy(x.EnemyWarLeader(this, false)));
		}

		// Token: 0x06003975 RID: 14709 RVA: 0x00154320 File Offset: 0x00152520
		internal void DailyNationUpdate()
		{
			if (this.extant)
			{
				this.SetBaseInvestmentPoints_month();
				this.UpdateDailyTrackers();
				this.CacheRegionValues();
				for (int i = 0; i < Enums.PriorityTypes.Length; i++)
				{
					PriorityType priorityType = Enums.PriorityTypes[i];
					this.ModifyAccumulatedInvestment(priorityType, this.ControlPointWeightsTotalToPriorityIP(priorityType), false, false);
				}
				this.ProcessPrioritySpending();
				this.ArmiesDailyUpdate();
				if ((int)base.ID % 4 == TITimeState.CampaignDuration_days() % 4)
				{
					if (TIUtilities.RandomFloatValue() < this.PeriodicRevolutionChance())
					{
						this.Revolution();
					}
					else if (TIUtilities.RandomFloatValue() < this.PeriodicOrganicCoupChance() && !this.alienNation)
					{
						this.Coup(null, 0);
					}
					else
					{
						this.PeriodicInvoluntaryRegionTransferAwayCheck();
					}
				}
				foreach (TIRegionState tiregionState in this.regions)
				{
					tiregionState.xenoforming.DailyXenoformingGrowth();
					tiregionState.CheckSTOFighterCooldowns();
				}
				TIFactionState executiveFaction = this.executiveFaction;
				if (executiveFaction != null && executiveFaction.IsActiveHumanFaction)
				{
					float daysUntilExecutivePowerConsolidated = this.daysUntilExecutivePowerConsolidated;
					if (daysUntilExecutivePowerConsolidated <= 0f && daysUntilExecutivePowerConsolidated > -1f)
					{
						TINotificationQueueState.LogControlConsolidated(this.executiveFaction, this);
					}
				}
				this.SetDataDirty();
				return;
			}
			this.DailySecessionCheck();
			if (this.alienNation)
			{
				this.ArmiesDailyUpdate();
			}
		}

		// Token: 0x06003976 RID: 14710 RVA: 0x00154470 File Offset: 0x00152670
		private void DebugAllies()
		{
			if (this.rivals.Intersect<TINationState>(this.allies).Any<TINationState>())
			{
				string text = string.Empty;
				List<TINationState> list = new List<TINationState>();
				foreach (TINationState tinationState in this.rivals.Intersect<TINationState>(this.allies))
				{
					text = text + tinationState.displayName + " ";
					list.Add(tinationState);
				}
				Log.Error(this.displayName + " has same nations in rivals and allies: " + text, Array.Empty<object>());
				foreach (TINationState tinationState2 in list)
				{
					this.EndRivalry(null, tinationState2);
				}
			}
		}

		// Token: 0x06003977 RID: 14711 RVA: 0x00154560 File Offset: 0x00152760
		public void UpdateControlPointStatus()
		{
			foreach (TIControlPoint ticontrolPoint in this.controlPoints)
			{
				if (ticontrolPoint.defended && this.gameTime.currentTime >= ticontrolPoint.defendExpiration)
				{
					ticontrolPoint.ExpireDefense();
				}
				if (ticontrolPoint.benefitsDisabled && this.gameTime.currentTime >= ticontrolPoint.crackdownExpiration)
				{
					ticontrolPoint.ReenableBenefits();
				}
			}
		}

		// Token: 0x06003978 RID: 14712 RVA: 0x001545F8 File Offset: 0x001527F8
		private void UpdateControlPoints(TIFactionState grantToCouncil = null, bool suppressReporting = false)
		{
			if (this.extant)
			{
				this.UpdateControlPointStatus();
				this.numControlPoints_unclamped = this.getNumControlPoints_unclamped;
				int num = Mathf.Clamp(this.numControlPoints_unclamped, 1, 6);
				if (this.numControlPoints > num && this.numControlPoints > 0)
				{
					int num2 = this.numControlPoints - num;
					List<TIArmyState> list = new List<TIArmyState>();
					for (int i = 0; i < num2; i++)
					{
						List<TIGameState> controlPointOwnersByPoint = this.controlPointOwnersByPoint;
						TIControlPoint controlPoint = this.GetControlPoint(0);
						TIFactionState faction = controlPoint.faction;
						list.AddRange(controlPoint.RemoveControlPointFromNation());
						GameControl.eventManager.TriggerEvent(new NationShedsControlPoint(this), null, new object[] { this });
						if (faction != null && !suppressReporting)
						{
							TINotificationQueueState.LogControlPointReduction(this, faction, controlPointOwnersByPoint);
						}
					}
					foreach (TIControlPoint ticontrolPoint in this.controlPoints)
					{
						List<TIArmyState> list2 = new List<TIArmyState>(ticontrolPoint.armies);
						ticontrolPoint.positionInNation -= num2;
						foreach (TIArmyState tiarmyState in list2)
						{
							tiarmyState.controlPointIdx -= num2;
							tiarmyState.SetArmyDataDirty();
						}
					}
					foreach (TIArmyState tiarmyState2 in list)
					{
						tiarmyState2.controlPointIdx = this.GetNextArmyControlPointIdx();
						tiarmyState2.AssignToFaction(this.controlPoints[tiarmyState2.controlPointIdx].faction, false);
					}
					this.UpdateNativeControlPointsCount();
					this.SetDataDirty();
					return;
				}
				if (this.numControlPoints < num)
				{
					List<TIGameState> controlPointOwnersByPoint2 = this.controlPointOwnersByPoint;
					int num3 = num - this.numControlPoints;
					foreach (TIControlPoint ticontrolPoint2 in this.controlPoints.Reverse<TIControlPoint>())
					{
						List<TIArmyState> list3 = new List<TIArmyState>(ticontrolPoint2.armies);
						ticontrolPoint2.positionInNation += num3;
						foreach (TIArmyState tiarmyState3 in list3)
						{
							tiarmyState3.controlPointIdx += num3;
							tiarmyState3.SetArmyDataDirty();
						}
					}
					for (int j = 0; j < num3; j++)
					{
						this.numControlPoints++;
						TIControlPoint ticontrolPoint3 = GameStateManager.CreateNewGameState<TIControlPoint>();
						ticontrolPoint3.InitWithNationState(this, j);
						this.controlPoints.Insert(j, ticontrolPoint3);
					}
					for (int k = 0; k < num3; k++)
					{
						TIControlPoint ticontrolPoint4 = this.controlPoints[k];
						if (grantToCouncil == null)
						{
							if (this.controlPoints.Any<TIControlPoint>((TIControlPoint x) => x.owned))
							{
								grantToCouncil = this.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.owned).MaxBy<TIControlPoint, int>((TIControlPoint y) => y.positionInNation).faction;
							}
						}
						if (grantToCouncil != null)
						{
							this.ChangeControlPointOwner(k, ControlPointChangeCause.Growth, grantToCouncil);
						}
						GameControl.eventManager.TriggerEvent(new NationGrowsNewControlPoint(ticontrolPoint4), null, Array.Empty<object>());
						this.ApplyInvestmentTemplateToControlPoint(k, (grantToCouncil == null || grantToCouncil.defaultPriorityPreset == null) ? this.template.initialPriorityPreset[k] : grantToCouncil.defaultPriorityPresetTemplateName);
						if (grantToCouncil != null && k == num3 - 1)
						{
							TINotificationQueueState.LogControlPointAdded(this, grantToCouncil, ticontrolPoint4, controlPointOwnersByPoint2);
						}
					}
					this.UpdateNativeControlPointsCount();
					this.SetDataDirty();
					return;
				}
			}
			else
			{
				List<TIControlPoint> list4 = new List<TIControlPoint>();
				foreach (TIControlPoint ticontrolPoint5 in this.controlPoints)
				{
					if (ticontrolPoint5.positionInNation > 0)
					{
						list4.Add(ticontrolPoint5);
					}
					else
					{
						ticontrolPoint5.SetFaction(null, false);
					}
				}
				List<TIArmyState> list5 = new List<TIArmyState>();
				foreach (TIControlPoint ticontrolPoint6 in list4)
				{
					list5.AddRange(ticontrolPoint6.RemoveControlPointFromNation());
				}
				this.numControlPoints = 1;
			}
		}

		// Token: 0x06003979 RID: 14713 RVA: 0x00154AC0 File Offset: 0x00152CC0
		public void UpdateNativeControlPointsCount()
		{
			this.StartOfTurnNativeControlPoints = this.NumNativeControlPoints;
		}

		// Token: 0x0600397A RID: 14714 RVA: 0x00154AD0 File Offset: 0x00152CD0
		public void UpdateArmiesControllingFactions()
		{
			foreach (TIArmyState tiarmyState in this.armies)
			{
				if (tiarmyState.armyType == ArmyType.Human && tiarmyState.strength > 0f && tiarmyState.homeRegion != null)
				{
					TIControlPoint controlPoint = this.GetControlPoint(tiarmyState.controlPointIdx);
					if (controlPoint == null)
					{
						Log.Error("Army " + tiarmyState.displayName + " does not have a control point assigned. Idx is " + tiarmyState.controlPointIdx.ToString(), Array.Empty<object>());
					}
					else if (controlPoint.faction != tiarmyState.faction)
					{
						tiarmyState.AssignToFaction(controlPoint.faction, false);
					}
				}
			}
		}

		// Token: 0x0600397B RID: 14715 RVA: 0x00154BA8 File Offset: 0x00152DA8
		internal void MonthlyNationUpdate(float eyes)
		{
			this.UpdateControlPoints(null, false);
			this.ResetPeriodicTrackers();
			if (this.extant)
			{
				if (this.gameTime.currentTime.month == 1)
				{
					this.directInvestmentedIPsThisYear = 0f;
				}
				if (!this.alienNation)
				{
					if (this.wars.Count > 0)
					{
						this.AddToDemocracy(-0.01f * this.priorityEffectPopScaling, TINationState.DemocracyChangeReason.DemReason_AtWar);
					}
					else
					{
						bool flag = true;
						bool flag2 = false;
						foreach (TINationState tinationState in GameStateManager.AllExtantHumanNations())
						{
							if (this.IsAdjacentToNation(tinationState, true))
							{
								flag2 = true;
								if (tinationState.wars.Count > 0 || tinationState.democracy <= this.democracy)
								{
									flag = false;
								}
							}
						}
						if (flag && flag2)
						{
							this.AddToDemocracy(TemplateManager.global.basePassiveDemocracyIncreaseFromNeighbor * this.priorityEffectPopScaling, TINationState.DemocracyChangeReason.DemReason_NearbyDemocracies);
						}
					}
					float num = (4f - this.cohesion) / 4f;
					if (TIUtilities.RandomFloatValue() < num)
					{
						this.AddToDemocracy(-0.01f * this.priorityEffectPopScaling, TINationState.DemocracyChangeReason.DemReason_LowCohesion);
					}
					if (eyes <= 0f)
					{
						goto IL_022C;
					}
					using (List<TIRegionState>.Enumerator enumerator2 = this.regions.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TIRegionState tiregionState = enumerator2.Current;
							if (this.alienNation || TIUtilities.RandomFloatValue() < eyes * TIGlobalConfig.globalConfig.monthlyChanceAbductionPerSurveillanceHabEye)
							{
								tiregionState.ConductAbductions(GameStateManager.AlienFaction(), 1);
							}
						}
						goto IL_022C;
					}
				}
				foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
				{
					if (tifactionState.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.Abductions))
					{
						this.capital.alienActivity.ActivitySightedByFaction(tifactionState, TIFactionState.abductionsMission, null, null, null);
					}
					else if (tifactionState.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTarget == ObjectiveMissionTargetType.EnthrallMission))
					{
						this.capital.alienActivity.ActivitySightedByFaction(tifactionState, TIFactionState.enthrallPublicMission, null, null, null);
					}
				}
				IL_022C:
				this.AddToCohesion(this.GetMonthlyCohesionMovement(), TINationState.CohesionChangeReason.CohesionReason_MovementToRestValue);
				this.AddToUnrest(this.GetMonthlyUnrestMovement(), TINationState.UnrestChangeReason.UnrestReason_MovementToRestValue, 10f);
				foreach (TIRegionState tiregionState2 in this.regions)
				{
					tiregionState2.GrowPopulationByMonth();
				}
				this.historyWarStatus.Insert(0, this.AssessOverallWarStatus());
				this.historyWarStatus.RemoveRange(30, this.historyWarStatus.Count - 30);
				this.UpdateControlPointTypes();
			}
		}

		// Token: 0x0600397C RID: 14716 RVA: 0x00154E90 File Offset: 0x00153090
		internal void QuarterlyNationUpdate()
		{
			this.UpdateQuarterlyTrackers();
		}

		// Token: 0x04002479 RID: 9337
		public List<TIControlPoint> controlPoints;

		// Token: 0x0400247D RID: 9341
		public TIFederationState federation;

		// Token: 0x04002484 RID: 9348
		public TINationState breakawayParent;

		// Token: 0x04002485 RID: 9349
		public List<TINationState> breakaways;

		// Token: 0x04002486 RID: 9350
		[SerializeField]
		private Dictionary<TINationState, TerrestrialAdjacencyType> adjacentNations;

		// Token: 0x04002487 RID: 9351
		[SerializeField]
		private Dictionary<TIFactionState, int> factionUnrestAttempts = new Dictionary<TIFactionState, int>();

		// Token: 0x0400248B RID: 9355
		public List<float> historyCohesion;

		// Token: 0x0400248C RID: 9356
		public List<float> historyCohesionRestState;

		// Token: 0x0400248D RID: 9357
		public List<float> historyDemocracy;

		// Token: 0x0400248E RID: 9358
		public List<float> historyUnrest;

		// Token: 0x0400248F RID: 9359
		public List<float> historyUnrestRestState;

		// Token: 0x04002490 RID: 9360
		public List<float> historyInequality;

		// Token: 0x04002491 RID: 9361
		public List<double> historyGDP;

		// Token: 0x04002492 RID: 9362
		public List<float> historySpaceFunding;

		// Token: 0x04002493 RID: 9363
		public List<float> historyEducation;

		// Token: 0x04002494 RID: 9364
		public List<float> historyPopulation;

		// Token: 0x04002495 RID: 9365
		public List<float> historySustainability = new List<float>(32);

		// Token: 0x04002496 RID: 9366
		public List<float> historyBoost;

		// Token: 0x04002497 RID: 9367
		public List<int> historyMissionControl;

		// Token: 0x04002498 RID: 9368
		public List<float> historyMiltech;

		// Token: 0x04002499 RID: 9369
		public List<int> historyNukes;

		// Token: 0x0400249A RID: 9370
		public List<float> historyResearch;

		// Token: 0x0400249B RID: 9371
		public List<float> historyInvestmentPoints;

		// Token: 0x0400249C RID: 9372
		public List<Dictionary<FactionIdeology, float>> historyPublicOpinion;

		// Token: 0x0400249D RID: 9373
		public List<float> historyWarStatus;

		// Token: 0x0400249E RID: 9374
		public List<int> historyNumRegions;

		// Token: 0x040024A1 RID: 9377
		[SerializeField]
		private float baseInvestmentPoints_month;

		// Token: 0x040024A4 RID: 9380
		public float directInvestmentedIPsThisYear;

		// Token: 0x040024B9 RID: 9401
		public bool alienNation;

		// Token: 0x040024BA RID: 9402
		public bool aggregateNation;

		// Token: 0x040024BB RID: 9403
		public Dictionary<TINationState, TIDateTime> improveRelationsCooldowns;

		// Token: 0x040024BC RID: 9404
		public Dictionary<TINationState, TIDateTime> rivalryCooldowns;

		// Token: 0x040024BD RID: 9405
		public List<TINationState> improveRelationsDeclinedUnderCurrentExecutivePair;

		// Token: 0x040024BE RID: 9406
		public TIDateTime dateOfNewGovernment;

		// Token: 0x040024BF RID: 9407
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040024C0 RID: 9408
		private GameTimeManager gameTime;

		// Token: 0x040024C1 RID: 9409
		private Sprite _flag;

		// Token: 0x040024C2 RID: 9410
		public const int daysOfHistoryTracking = 32;

		// Token: 0x040024C3 RID: 9411
		public const int lastIdxOfHistoryTracking = 31;

		// Token: 0x040024C4 RID: 9412
		public LastExecutiveChange lastExecutiveChange;

		// Token: 0x040024C5 RID: 9413
		public int numOilRegions_dailyCache;

		// Token: 0x040024C6 RID: 9414
		public int numMiningRegions_dailyCache;

		// Token: 0x040024C7 RID: 9415
		public int numCoreEconomicRegions_dailyCache;

		// Token: 0x040024C8 RID: 9416
		public float restofFederationECOBonus_dailyCache;

		// Token: 0x040024C9 RID: 9417
		public float cohesionRestState_dailyCache;

		// Token: 0x040024CA RID: 9418
		public float unrestRestState_dailyCache;

		// Token: 0x040024CB RID: 9419
		public const int maxControlPoints = 6;

		// Token: 0x040024CC RID: 9420
		public const float initialHumanMaxMilitaryTechLevel = 5f;

		// Token: 0x040024CD RID: 9421
		public const float maxAlienNationMilitaryTechLevel = 8f;

		// Token: 0x040024CE RID: 9422
		public Dictionary<TINationState.GDPChangeReason, float> tracker_GDPChangeReason_CurrentTrackingPeriod;

		// Token: 0x040024CF RID: 9423
		public Dictionary<TINationState.GDPChangeReason, float> tracker_GDPChangeReason_PriorTrackingPeriod;

		// Token: 0x040024D0 RID: 9424
		public Dictionary<TINationState.GDPChangeReason, float> tracker_GDPChangeReason_AllTime;

		// Token: 0x040024D1 RID: 9425
		public Dictionary<int, float> tracker_GDP_ByQuarter;

		// Token: 0x040024D2 RID: 9426
		public Dictionary<TINationState.InequalityChangeReason, float> tracker_InequalityChangeReason_CurrentTrackingPeriod;

		// Token: 0x040024D3 RID: 9427
		public Dictionary<TINationState.InequalityChangeReason, float> tracker_InequalityChangeReason_PriorTrackingPeriod;

		// Token: 0x040024D4 RID: 9428
		public Dictionary<TINationState.InequalityChangeReason, float> tracker_InequalityChangeReason_AllTime;

		// Token: 0x040024D5 RID: 9429
		public Dictionary<int, float> tracker_Inequality_ByQuarter;

		// Token: 0x040024D6 RID: 9430
		public Dictionary<TINationState.CohesionChangeReason, float> tracker_CohesionChangeReason_CurrentTrackingPeriod;

		// Token: 0x040024D7 RID: 9431
		public Dictionary<TINationState.CohesionChangeReason, float> tracker_CohesionChangeReason_PriorTrackingPeriod;

		// Token: 0x040024D8 RID: 9432
		public Dictionary<TINationState.CohesionChangeReason, float> tracker_CohesionChangeReason_AllTime;

		// Token: 0x040024D9 RID: 9433
		public Dictionary<int, float> tracker_Cohesion_ByQuarter;

		// Token: 0x040024DA RID: 9434
		public Dictionary<TINationState.UnrestChangeReason, float> tracker_UnrestChangeReason_CurrentTrackingPeriod;

		// Token: 0x040024DB RID: 9435
		public Dictionary<TINationState.UnrestChangeReason, float> tracker_UnrestChangeReason_PriorTrackingPeriod;

		// Token: 0x040024DC RID: 9436
		public Dictionary<TINationState.UnrestChangeReason, float> tracker_UnrestChangeReason_AllTime;

		// Token: 0x040024DD RID: 9437
		public Dictionary<int, float> tracker_Unrest_ByQuarter;

		// Token: 0x040024DE RID: 9438
		public Dictionary<TINationState.EducationChangeReason, float> tracker_EducationChangeReason_CurrentTrackingPeriod;

		// Token: 0x040024DF RID: 9439
		public Dictionary<TINationState.EducationChangeReason, float> tracker_EducationChangeReason_PriorTrackingPeriod;

		// Token: 0x040024E0 RID: 9440
		public Dictionary<TINationState.EducationChangeReason, float> tracker_EducationChangeReason_AllTime;

		// Token: 0x040024E1 RID: 9441
		public Dictionary<int, float> tracker_Education_ByQuarter;

		// Token: 0x040024E2 RID: 9442
		public Dictionary<TINationState.DemocracyChangeReason, float> tracker_DemocracyChangeReason_CurrentTrackingPeriod;

		// Token: 0x040024E3 RID: 9443
		public Dictionary<TINationState.DemocracyChangeReason, float> tracker_DemocracyChangeReason_PriorTrackingPeriod;

		// Token: 0x040024E4 RID: 9444
		public Dictionary<TINationState.DemocracyChangeReason, float> tracker_DemocracyChangeReason_AllTime;

		// Token: 0x040024E5 RID: 9445
		public Dictionary<int, float> tracker_Democracy_ByQuarter;

		// Token: 0x040024E6 RID: 9446
		public Dictionary<int, float> tracker_PCGDP_ByQuarter;

		// Token: 0x040024E8 RID: 9448
		private const int minControlPoints = 1;

		// Token: 0x040024EA RID: 9450
		private const int maxInequality = 9;

		// Token: 0x040024EB RID: 9451
		public const float CO2_ppm_CognitionLoss = 945f;

		// Token: 0x040024EC RID: 9452
		public const float CO2_CognitionLossRate_month = 0.005f;

		// Token: 0x040024ED RID: 9453
		public const float CO2_CognitionLossRate_max = 0.1f;

		// Token: 0x040024EE RID: 9454
		public const double GDPtoGHG = 275000.0;

		// Token: 0x040024EF RID: 9455
		public const double PoptoGHG = 2.41;

		// Token: 0x040024F0 RID: 9456
		public const double OilRegionToGHG = 250000000.0;

		// Token: 0x040024F1 RID: 9457
		public const float preIndustrialPCGDP = 15000f;

		// Token: 0x040024F2 RID: 9458
		public const float subsistencePCGDP = 7500f;

		// Token: 0x040024F3 RID: 9459
		public const float GHGPortion_CO2 = 0.823f;

		// Token: 0x040024F4 RID: 9460
		public const float GHGPortion_CH4 = 0.115f;

		// Token: 0x040024F5 RID: 9461
		public const float GHGPortion_N2O = 0.062f;

		// Token: 0x040024F6 RID: 9462
		public const float CO2AfterUptake = 0.4f;

		// Token: 0x040024F7 RID: 9463
		public const float CH4AfterUptake = 1f;

		// Token: 0x040024F8 RID: 9464
		public const float N2OAfterUptake = 1f;

		// Token: 0x040024F9 RID: 9465
		private static float _cachedBestCurrentSustainabilityValue;

		// Token: 0x040024FA RID: 9466
		private static int _bestCurrentSustainabilityFrame = -1;

		// Token: 0x040024FE RID: 9470
		public const float baseCohesionValue = 16f;

		// Token: 0x040024FF RID: 9471
		private const int worseOffQuarters = 40;

		// Token: 0x04002500 RID: 9472
		private const float CohesionPerPeerRival = 0.5f;

		// Token: 0x04002501 RID: 9473
		private const float MaxCohesionFromRivals = 3f;

		// Token: 0x04002502 RID: 9474
		private const float MaxCohesionFromWars = 3f;

		// Token: 0x04002503 RID: 9475
		private const float totalitarian = 2f;

		// Token: 0x04002504 RID: 9476
		private const float authoritarian = 3.5f;

		// Token: 0x04002505 RID: 9477
		private const float democratic = 6.5f;

		// Token: 0x04002506 RID: 9478
		private const float autocracyScaling = 1.285f;

		// Token: 0x04002508 RID: 9480
		public static readonly List<TINationState.UnrestChangeReason> alienNationOnlyUnrestChangeReason = new List<TINationState.UnrestChangeReason>
		{
			TINationState.UnrestChangeReason.UnrestReason_AlienNationDominance,
			TINationState.UnrestChangeReason.UnrestReason_AliensLostCapitalChaos
		};

		// Token: 0x04002509 RID: 9481
		public const float baseUnrestValue = 10.5f;

		// Token: 0x0400250A RID: 9482
		private const float DemocracyExponent = 0.16666667f;

		// Token: 0x0400250B RID: 9483
		private const float MasterResearchMultiplier = 0.0075f;

		// Token: 0x0400250C RID: 9484
		private const float PCGDPExponent = 0.6f;

		// Token: 0x0400250D RID: 9485
		private const float TwoToPCGDPExponent = 1.5157166f;

		// Token: 0x0400250F RID: 9487
		private bool cachedNavalFreedom;

		// Token: 0x04002510 RID: 9488
		private int navalFreedomCachedFrame = -1;

		// Token: 0x04002511 RID: 9489
		public float spaceFunding_year;

		// Token: 0x04002512 RID: 9490
		public static readonly FactionResource[] NationalResources = new FactionResource[]
		{
			FactionResource.Boost,
			FactionResource.Influence,
			FactionResource.MissionControl,
			FactionResource.Money,
			FactionResource.Research
		};

		// Token: 0x04002513 RID: 9491
		private const float publicOpinionToInfluenceScaler = 0.5f;

		// Token: 0x04002514 RID: 9492
		[SerializeField]
		private Dictionary<PriorityType, float> _accumulatedInvestmentPoints = new Dictionary<PriorityType, float>();

		// Token: 0x04002515 RID: 9493
		private const float directInvest_CPMaintenanceCostFractionInfluence = 0.1f;

		// Token: 0x04002516 RID: 9494
		private const float directInvest_MoneyInefficiencyMultiplier = 1.2f;

		// Token: 0x04002517 RID: 9495
		private const float directInvest_ViceroyInfluenceModifier = 0.5f;

		// Token: 0x04002518 RID: 9496
		private const float directInvest_CorruptionIncreaseModifier = 0.75f;

		// Token: 0x04002519 RID: 9497
		private const float direct_ECO_Base_Money = 1f;

		// Token: 0x0400251A RID: 9498
		private const float direct_ECO_Influence = 25f;

		// Token: 0x0400251B RID: 9499
		private const float direct_WEL_Base_Money = 1800000f;

		// Token: 0x0400251C RID: 9500
		private const float direct_WEL_Influence = 100f;

		// Token: 0x0400251D RID: 9501
		private const float direct_ENV_Base_Money = 150000f;

		// Token: 0x0400251E RID: 9502
		private const float direct_ENV_Influence = 100f;

		// Token: 0x0400251F RID: 9503
		private const float direct_KNO_Base_Money = 250000f;

		// Token: 0x04002520 RID: 9504
		private const float direct_KNO_Influence = 100f;

		// Token: 0x04002521 RID: 9505
		private const float direct_GOV_Base_Money = 150000f;

		// Token: 0x04002522 RID: 9506
		private const float direct_GOV_Influence = 300f;

		// Token: 0x04002523 RID: 9507
		private const float direct_UNI_Base_Money = 400f;

		// Token: 0x04002524 RID: 9508
		private const float direct_UNI_Influence = 300f;

		// Token: 0x04002525 RID: 9509
		private const float direct_MIL_Money = 250f;

		// Token: 0x04002526 RID: 9510
		private const float direct_MIL_Influence = 250f;

		// Token: 0x04002527 RID: 9511
		private const float direct_MIL_Ops = 30f;

		// Token: 0x04002528 RID: 9512
		private const float direct_OPP_Base_Influence = 50f;

		// Token: 0x04002529 RID: 9513
		private const float direct_OPP_Ops = 30f;

		// Token: 0x0400252A RID: 9514
		private const float direct_DEV_Influence = 5f;

		// Token: 0x0400252B RID: 9515
		private const float direct_FLI_Money = 2800f;

		// Token: 0x0400252C RID: 9516
		private const float direct_FLI_Influence = 2500f;

		// Token: 0x0400252D RID: 9517
		private const float direct_BOO_Money = 500f;

		// Token: 0x0400252E RID: 9518
		private const float direct_BOO_Influence = 100f;

		// Token: 0x0400252F RID: 9519
		private const float direct_MC_Money = 2500f;

		// Token: 0x04002530 RID: 9520
		private const float direct_MC_Influence = 800f;

		// Token: 0x04002531 RID: 9521
		private const float direct_ARM_Money = 10000f;

		// Token: 0x04002532 RID: 9522
		private const float direct_ARM_Influence = 3000f;

		// Token: 0x04002533 RID: 9523
		private const float direct_ARM_Ops = 2000f;

		// Token: 0x04002534 RID: 9524
		private const float direct_NAV_Money = 1200f;

		// Token: 0x04002535 RID: 9525
		private const float direct_NAV_Influence = 3000f;

		// Token: 0x04002536 RID: 9526
		private const float direct_NAV_Ops = 1500f;

		// Token: 0x04002537 RID: 9527
		private const float direct_FMI_Money = 2000f;

		// Token: 0x04002538 RID: 9528
		private const float direct_FMI_Influence = 2000f;

		// Token: 0x04002539 RID: 9529
		private const float direct_FMI_Ops = 1500f;

		// Token: 0x0400253A RID: 9530
		private const float direct_NUC_Money = 30000f;

		// Token: 0x0400253B RID: 9531
		private const float direct_NUC_Influence = 3000f;

		// Token: 0x0400253C RID: 9532
		private const float direct_NUC_Ops = 300f;

		// Token: 0x0400253D RID: 9533
		private const float direct_NUK_Money = 5000f;

		// Token: 0x0400253E RID: 9534
		private const float direct_NUK_Influence = 1500f;

		// Token: 0x0400253F RID: 9535
		private const float direct_NUK_Ops = 150f;

		// Token: 0x04002540 RID: 9536
		private const float direct_DEF_Money = 2500f;

		// Token: 0x04002541 RID: 9537
		private const float direct_DEF_Influence = 800f;

		// Token: 0x04002542 RID: 9538
		private const float direct_DEF_Ops = 600f;

		// Token: 0x04002543 RID: 9539
		private const float direct_STO_Money = 1000f;

		// Token: 0x04002544 RID: 9540
		private const float direct_STO_Influence = 200f;

		// Token: 0x04002545 RID: 9541
		private const float direct_STO_Ops = 150f;

		// Token: 0x04002546 RID: 9542
		public const int populationBaseLineForScaling = 50000000;

		// Token: 0x04002548 RID: 9544
		public const int numWelfaresForDecolonizeTriggers = 1000;

		// Token: 0x04002549 RID: 9545
		private const float BadSustainabilityCutPoint = 2f;

		// Token: 0x0400254A RID: 9546
		private const float GoodSustainabililityCutPoint = 0.5f;

		// Token: 0x0400254B RID: 9547
		public const int numEnvironmentsToTriggerDecontaminate = 100;

		// Token: 0x0400254C RID: 9548
		public const float PeakEducationEffectiveness = 12f;

		// Token: 0x0400254D RID: 9549
		public const float bonusEducationEffectiveness = 8.5f;

		// Token: 0x0400254E RID: 9550
		public const float OppressionDemocracyMinimumForLosingCohesion = 5f;

		// Token: 0x0400254F RID: 9551
		private const float maxGDPToFunding = 0.005f;

		// Token: 0x04002550 RID: 9552
		public const float minGDPForCoreEconomicRegion_bn = 500f;

		// Token: 0x04002551 RID: 9553
		private const float whitePeaceCohesionPenaltyModifier_attacker = 2f;

		// Token: 0x04002552 RID: 9554
		private const float whitePeaceCohesionPenaltyModifier_defender = 0.5f;

		// Token: 0x04002553 RID: 9555
		public static readonly TIResourcesCost FactionLevelRelationShipChangeCost = new TIResourcesCost(TIFactionState.setPolicyMission.cost.resourceType, TIFactionState.setPolicyMission.cost.value);

		// Token: 0x04002554 RID: 9556
		private const float maxRegionDamageDuringRevolution = 0.25f;

		// Token: 0x04002555 RID: 9557
		private const float lowUnrestChangeDuringRevolution = -6f;

		// Token: 0x04002556 RID: 9558
		private const float highUnrestChangeDuringRevolution = -3f;

		// Token: 0x04002557 RID: 9559
		private const float minCohesionChangeDuringRevolution = -3f;

		// Token: 0x04002558 RID: 9560
		private const float maxCohesionChangeDuringRevolution = 3f;

		// Token: 0x04002559 RID: 9561
		private const float minInequalityChangeDuringRevolution = -3f;

		// Token: 0x0400255A RID: 9562
		private const float maxInequalityChangeDuringRevolution = 0f;

		// Token: 0x0400255B RID: 9563
		public const float maxDemocracyForOrganicCoup = 8f;

		// Token: 0x0400255C RID: 9564
		public const float minUnrestForOrganicCoup = 8f;

		// Token: 0x0400255D RID: 9565
		public static readonly float minUnrestForRevolution = 9.9f;

		// Token: 0x0400255E RID: 9566
		public static readonly float maxCohesionForSecession = 3f;

		// Token: 0x0400255F RID: 9567
		public static readonly float minUnrestForSecession = 6f;

		// Token: 0x04002560 RID: 9568
		private Dictionary<TINationState, bool> accessibleWarEnemy_skipWarEnemiesTRUE = new Dictionary<TINationState, bool>();

		// Token: 0x04002561 RID: 9569
		private Dictionary<TINationState, bool> accessibleWarEnemy_skipWarEnemiesFALSE = new Dictionary<TINationState, bool>();

		// Token: 0x04002562 RID: 9570
		private bool AccessibleWarEnemyCachedNavalFreedom;

		// Token: 0x04002563 RID: 9571
		private float cachedMilitaryStrength;

		// Token: 0x04002564 RID: 9572
		private int militaryStrengthCachedFrame = -1;

		// Token: 0x04002565 RID: 9573
		public int oldRivalsCount;

		// Token: 0x04002566 RID: 9574
		private const int rareEventsProcessingFrequency_Days = 4;

		// Token: 0x04002567 RID: 9575
		public static readonly IEnumerable<PriorityType> Priorities = (PriorityType[])Enum.GetValues(typeof(PriorityType));

		// Token: 0x02000E32 RID: 3634
		public enum GDPChangeReason
		{
			// Token: 0x040056D9 RID: 22233
			GDPReason_EconomyPriority,
			// Token: 0x040056DA RID: 22234
			GDPReason_PopulationChange,
			// Token: 0x040056DB RID: 22235
			GDPReason_ClimateChange,
			// Token: 0x040056DC RID: 22236
			GDPReason_RegionDamage,
			// Token: 0x040056DD RID: 22237
			GDPReason_TerritoryChange,
			// Token: 0x040056DE RID: 22238
			GDPReason_Coup,
			// Token: 0x040056DF RID: 22239
			GDPReason_Independence,
			// Token: 0x040056E0 RID: 22240
			GDPReason_GlobalCoreEconomicRegionDestroyed,
			// Token: 0x040056E1 RID: 22241
			GDPReason_GlobalCoreResourceRegionDestroyed,
			// Token: 0x040056E2 RID: 22242
			GDPReason_EventEffect
		}

		// Token: 0x02000E33 RID: 3635
		public enum InequalityChangeReason
		{
			// Token: 0x040056E4 RID: 22244
			InqReason_EconomyPriority,
			// Token: 0x040056E5 RID: 22245
			InqReason_WelfarePriority,
			// Token: 0x040056E6 RID: 22246
			InqReason_SpoilsPriority,
			// Token: 0x040056E7 RID: 22247
			InqReason_ClimateChange,
			// Token: 0x040056E8 RID: 22248
			InqReason_Secession,
			// Token: 0x040056E9 RID: 22249
			InqReason_Annexation,
			// Token: 0x040056EA RID: 22250
			InqReason_EventEffects,
			// Token: 0x040056EB RID: 22251
			InqReason_Revolution
		}

		// Token: 0x02000E34 RID: 3636
		public enum EducationChangeReason
		{
			// Token: 0x040056ED RID: 22253
			EducationReason_KnowledgePriority,
			// Token: 0x040056EE RID: 22254
			EducationReason_UnityPriority,
			// Token: 0x040056EF RID: 22255
			EducationReason_RegionTransfer,
			// Token: 0x040056F0 RID: 22256
			EducationReason_PopulationLoss,
			// Token: 0x040056F1 RID: 22257
			EducationReason_CO2Poisoning,
			// Token: 0x040056F2 RID: 22258
			EducationReason_EventEffect
		}

		// Token: 0x02000E35 RID: 3637
		public enum DemocracyChangeReason
		{
			// Token: 0x040056F4 RID: 22260
			DemReason_GovernmentPriority,
			// Token: 0x040056F5 RID: 22261
			DemReason_OppressionPriority,
			// Token: 0x040056F6 RID: 22262
			DemReason_SpoilsPriority,
			// Token: 0x040056F7 RID: 22263
			DemReason_LowCohesion,
			// Token: 0x040056F8 RID: 22264
			DemReason_ZeroCohesion,
			// Token: 0x040056F9 RID: 22265
			DemReason_Revolution,
			// Token: 0x040056FA RID: 22266
			DemReason_Coup,
			// Token: 0x040056FB RID: 22267
			DemReason_RegimeChange,
			// Token: 0x040056FC RID: 22268
			DemReason_AmicableRelease,
			// Token: 0x040056FD RID: 22269
			DemReason_Secession,
			// Token: 0x040056FE RID: 22270
			DemReason_AtWar,
			// Token: 0x040056FF RID: 22271
			DemReason_NearbyDemocracies,
			// Token: 0x04005700 RID: 22272
			DemReason_EventEffect
		}

		// Token: 0x02000E36 RID: 3638
		public enum CohesionChangeReason
		{
			// Token: 0x04005702 RID: 22274
			CohesionReason_UnityPriority,
			// Token: 0x04005703 RID: 22275
			CohesionReason_KnowledgePriority,
			// Token: 0x04005704 RID: 22276
			CohesionReason_OppressionPriority,
			// Token: 0x04005705 RID: 22277
			CohesionReason_MovementToRestValue,
			// Token: 0x04005706 RID: 22278
			CohesionReason_InequalityAboveMax,
			// Token: 0x04005707 RID: 22279
			CohesionReason_ArmyLost,
			// Token: 0x04005708 RID: 22280
			CohesionReason_RegimeChange,
			// Token: 0x04005709 RID: 22281
			CohesionReason_DeclaringWarOnNewRival,
			// Token: 0x0400570A RID: 22282
			CohesionReason_DeclaringWarOnOldRival,
			// Token: 0x0400570B RID: 22283
			CohesionReason_WarDeclaredOnUs,
			// Token: 0x0400570C RID: 22284
			CohesionReason_AnsweredAllyCallToWar,
			// Token: 0x0400570D RID: 22285
			CohesionReason_Revolution,
			// Token: 0x0400570E RID: 22286
			CohesionReason_Secession,
			// Token: 0x0400570F RID: 22287
			CohesionReason_Coup,
			// Token: 0x04005710 RID: 22288
			CohesionReason_Independence,
			// Token: 0x04005711 RID: 22289
			CohesionReason_NationReleased,
			// Token: 0x04005712 RID: 22290
			CohesionReason_Annexation,
			// Token: 0x04005713 RID: 22291
			CohesionReason_RegionBrokeAway,
			// Token: 0x04005714 RID: 22292
			CohesionReason_WarEnded,
			// Token: 0x04005715 RID: 22293
			CohesionReason_Effect
		}

		// Token: 0x02000E37 RID: 3639
		public enum UnrestChangeReason
		{
			// Token: 0x04005717 RID: 22295
			UnrestReason_UnrestMission,
			// Token: 0x04005718 RID: 22296
			UnrestReason_StabilizeMission,
			// Token: 0x04005719 RID: 22297
			UnrestReason_StabilizeMissionFailure,
			// Token: 0x0400571A RID: 22298
			UnrestReason_OppressionPriority,
			// Token: 0x0400571B RID: 22299
			UnrestReason_MovementToRestValue,
			// Token: 0x0400571C RID: 22300
			UnrestReason_ZeroCohesion,
			// Token: 0x0400571D RID: 22301
			UnrestReason_Revolution,
			// Token: 0x0400571E RID: 22302
			UnrestReason_Independence,
			// Token: 0x0400571F RID: 22303
			UnrestReason_Coup,
			// Token: 0x04005720 RID: 22304
			UnrestReason_RegimeChange,
			// Token: 0x04005721 RID: 22305
			UnrestReason_RegionBrokeAway,
			// Token: 0x04005722 RID: 22306
			UnrestReason_RegionTransfer,
			// Token: 0x04005723 RID: 22307
			UnrestReason_NationReleased,
			// Token: 0x04005724 RID: 22308
			UnrestReason_DemocracyLostInRegionTransfer,
			// Token: 0x04005725 RID: 22309
			UnrestReason_InequalityAboveMax,
			// Token: 0x04005726 RID: 22310
			UnrestReason_EventEffect,
			// Token: 0x04005727 RID: 22311
			UnrestReason_AlienNationDominance,
			// Token: 0x04005728 RID: 22312
			UnrestReason_AliensLostCapitalChaos
		}
	}
}
