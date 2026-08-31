using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000769 RID: 1897
	public class TIFederationState : TIPolityState
	{
		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x0600369F RID: 13983 RVA: 0x0013D221 File Offset: 0x0013B421
		// (set) Token: 0x060036A0 RID: 13984 RVA: 0x0013D229 File Offset: 0x0013B429
		public List<TINationState> members { get; private set; }

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x060036A1 RID: 13985 RVA: 0x0013D232 File Offset: 0x0013B432
		// (set) Token: 0x060036A2 RID: 13986 RVA: 0x0013D23A File Offset: 0x0013B43A
		[fsIgnore]
		public bool spaceProgram { get; private set; }

		// Token: 0x170008F5 RID: 2293
		// (get) Token: 0x060036A3 RID: 13987 RVA: 0x0013D243 File Offset: 0x0013B443
		public List<TINationState> memberAllies
		{
			get
			{
				return this.members.SelectMany<TINationState, TINationState>((TINationState x) => x.allies).Distinct<TINationState>().ToList<TINationState>();
			}
		}

		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x060036A4 RID: 13988 RVA: 0x0013D279 File Offset: 0x0013B479
		public List<TINationState> memberEnemies
		{
			get
			{
				return this.members.SelectMany<TINationState, TINationState>((TINationState x) => x.enemies).Distinct<TINationState>().ToList<TINationState>();
			}
		}

		// Token: 0x060036A5 RID: 13989 RVA: 0x0013D2B0 File Offset: 0x0013B4B0
		public List<TIRegionState> MemberClaims(bool includeHostile)
		{
			if (!includeHostile)
			{
				return this.members.SelectMany<TINationState, TIRegionState>((TINationState x) => x.nonHostileClaims).Distinct<TIRegionState>().ToList<TIRegionState>();
			}
			return this.members.SelectMany<TINationState, TIRegionState>((TINationState x) => x.claims).Distinct<TIRegionState>().ToList<TIRegionState>();
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x060036A6 RID: 13990 RVA: 0x0013D329 File Offset: 0x0013B529
		public TINationState leadNation
		{
			get
			{
				if (this.members.Count <= 0)
				{
					return null;
				}
				return this.members[0];
			}
		}

		// Token: 0x060036A7 RID: 13991 RVA: 0x0013D348 File Offset: 0x0013B548
		public double GDP(TINationState except = null)
		{
			if (!(except != null) || !this.members.Contains(except))
			{
				return this.members.Sum<TINationState>((TINationState x) => x.GDP);
			}
			return this.members.Except<TINationState>(new List<TINationState> { except }).Sum<TINationState>((TINationState x) => x.GDP);
		}

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x060036A8 RID: 13992 RVA: 0x0013D3D2 File Offset: 0x0013B5D2
		public override TINaturalSpaceObjectState ref_naturalSpaceObject
		{
			get
			{
				return GameStateManager.Earth();
			}
		}

		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x060036A9 RID: 13993 RVA: 0x0013D3D9 File Offset: 0x0013B5D9
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				return GameStateManager.Earth();
			}
		}

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x060036AA RID: 13994 RVA: 0x0013D3E0 File Offset: 0x0013B5E0
		public override TISpaceObjectState ref_spaceObject
		{
			get
			{
				return GameStateManager.Earth();
			}
		}

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x060036AB RID: 13995 RVA: 0x0013D3E7 File Offset: 0x0013B5E7
		public override TINationState ref_nation
		{
			get
			{
				return this.leadNation;
			}
		}

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x060036AC RID: 13996 RVA: 0x0013D3EF File Offset: 0x0013B5EF
		public override List<TIFactionState> ref_factions
		{
			get
			{
				return this.members.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint).Distinct<TIFactionState>().ToList<TIFactionState>();
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x060036AD RID: 13997 RVA: 0x0013D425 File Offset: 0x0013B625
		public bool hegemonicFederation
		{
			get
			{
				return this.leadNation.democracy < TemplateManager.global.fedLeaderDemocracyScoreToLeaveFederationFreely;
			}
		}

		// Token: 0x060036AE RID: 13998 RVA: 0x0013D43E File Offset: 0x0013B63E
		public override bool Initialize()
		{
			this.members = new List<TINationState>();
			return true;
		}

		// Token: 0x060036AF RID: 13999 RVA: 0x0013D44C File Offset: 0x0013B64C
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.lastAttemptToLeaveDarkFederation == null)
			{
				this.lastAttemptToLeaveDarkFederation = new Dictionary<TINationState, TIDateTime>();
			}
			this.SortMembers();
		}

		// Token: 0x060036B0 RID: 14000 RVA: 0x0013D467 File Offset: 0x0013B667
		public override void PostInitializationInit_4()
		{
			this.SetSpaceProgramValue();
		}

		// Token: 0x060036B1 RID: 14001 RVA: 0x0013D470 File Offset: 0x0013B670
		public override void PostAllStartUpInit_5()
		{
			List<TINationState> list = this.members.ToList<TINationState>();
			string displayName = this.displayName;
			foreach (TINationState tinationState in list)
			{
				if (tinationState.federation != this)
				{
					Log.Debug(tinationState.displayName + " is not in " + displayName + ", but it is registered in that federation. Repairing...", Array.Empty<object>());
					this.RemoveNation(null, tinationState, false);
				}
			}
			this.SortMembers();
			this.SetDisplayData();
		}

		// Token: 0x060036B2 RID: 14002 RVA: 0x0013D50C File Offset: 0x0013B70C
		public void SortMembers()
		{
			this.members = (from x in this.members
				orderby x.claims.Count + x.hostileClaims.Count descending, x.GDP descending
				select x).ToList<TINationState>();
		}

		// Token: 0x060036B3 RID: 14003 RVA: 0x0013D572 File Offset: 0x0013B772
		public void SetSpaceProgramValue()
		{
			this.spaceProgram = this.members.Any<TINationState>((TINationState x) => x.spaceFlightProgram);
		}

		// Token: 0x060036B4 RID: 14004 RVA: 0x0013D5A4 File Offset: 0x0013B7A4
		public void FoundFederation(TIFactionState actingFaction, List<TINationState> foundingMembers)
		{
			this.members = new List<TINationState>(foundingMembers);
			this.SortMembers();
			this.SetSpaceProgramValue();
			this.members.ForEach(delegate(TINationState x)
			{
				x.SetFederation(actingFaction, this, false, false);
			});
			this.members.ForEach(delegate(TINationState x)
			{
				this.AllyWithFederation(actingFaction, x);
			});
			this.members.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint).Distinct<TIFactionState>().ToList<TIFactionState>()
				.ForEach(delegate(TIFactionState y)
				{
					y.SetResourceIncomeDataDirty(TIFederationState.federationPooledResources);
				});
			this.SetDisplayData();
		}

		// Token: 0x060036B5 RID: 14005 RVA: 0x0013D66C File Offset: 0x0013B86C
		public bool CanAddNation(TINationState prospectiveNation)
		{
			return prospectiveNation.federation == null && !this.members.Contains(prospectiveNation) && this.memberAllies.Contains(prospectiveNation) && !this.memberEnemies.Contains(prospectiveNation) && prospectiveNation.ExecutivePowerConsolidated && !prospectiveNation.breakaway && this.leadNation.CanImproveRelationsYet(prospectiveNation) && (this.MemberClaims(true).Any<TIRegionState>((TIRegionState x) => x.nation == prospectiveNation) || prospectiveNation.claims.Any<TIRegionState>((TIRegionState x) => this.members.Contains(x.nation))) && this.members.All<TINationState>((TINationState x) => x.allies.Contains(prospectiveNation) || x.CanAlly(prospectiveNation, true));
		}

		// Token: 0x060036B6 RID: 14006 RVA: 0x0013D760 File Offset: 0x0013B960
		public void AddNation(TIFactionState actingFaction, TINationState nation, bool startup = false)
		{
			if ((this.CanAddNation(nation) || startup) && !this.members.Contains(nation))
			{
				this.members.Add(nation);
				this.SortMembers();
				this.SetSpaceProgramValue();
				if (nation.federation == null)
				{
					nation.SetFederation(actingFaction, this, startup, false);
				}
				this.AllyWithFederation(actingFaction, nation);
				this.members.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint).Distinct<TIFactionState>().ToList<TIFactionState>()
					.ForEach(delegate(TIFactionState y)
					{
						y.SetResourceIncomeDataDirty(TIFederationState.federationPooledResources);
					});
				this.SetDisplayData();
			}
		}

		// Token: 0x060036B7 RID: 14007 RVA: 0x0013D824 File Offset: 0x0013BA24
		public void RemoveNation(TIFactionState actingFaction, TINationState nation, bool offerWar)
		{
			if (this.members.Contains(nation))
			{
				this.members.Remove(nation);
				if (nation.federation == this)
				{
					nation.LeaveFederation(actingFaction, nation.extant);
				}
				this.SetSpaceProgramValue();
				nation.FactionsWithControlPoint.ForEach(delegate(TIFactionState x)
				{
					x.SetResourceIncomeDataDirty(TIFederationState.federationPooledResources);
				});
				this.members.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint).Distinct<TIFactionState>().ToList<TIFactionState>()
					.ForEach(delegate(TIFactionState y)
					{
						y.SetResourceIncomeDataDirty(TIFederationState.federationPooledResources);
					});
				this.SortMembers();
				this.SetDisplayData();
			}
			if (this.members.Count == 1)
			{
				if (this.members[0].federation == this)
				{
					this.members[0].LeaveFederation(actingFaction, false);
				}
				this.members.SelectMany<TINationState, TIFactionState>((TINationState x) => x.FactionsWithControlPoint).Distinct<TIFactionState>().ToList<TIFactionState>()
					.ForEach(delegate(TIFactionState y)
					{
						y.SetResourceIncomeDataDirty(TIFederationState.federationPooledResources);
					});
				this.members.Remove(this.members[0]);
				base.ArchiveState(true);
				GameStateManager.RemoveGameState<TIFederationState>(base.ID, false);
			}
		}

		// Token: 0x060036B8 RID: 14008 RVA: 0x0013D9C0 File Offset: 0x0013BBC0
		public void AllyWithFederation(TIFactionState actingFaction, TINationState nation)
		{
			foreach (TINationState tinationState in this.members)
			{
				nation.InitiateAlliance(actingFaction, tinationState);
			}
		}

		// Token: 0x060036B9 RID: 14009 RVA: 0x0013DA14 File Offset: 0x0013BC14
		private void SetDisplayData()
		{
			if (this.members.Contains(GameStateManager.AlienNation()))
			{
				TINationState tinationState = GameStateManager.AlienNation();
				this.displayName = tinationState.template.unionDisplayName;
				this.flagResource = tinationState.template.GetUnionFlagResource();
				this.displayNameWithArticle = tinationState.template.unionDisplayNameWithArticle;
				this.displayNameWithArticleCapitalized = Utilities.Capitalize(this.displayNameWithArticle);
				this.adjective = tinationState.template.unionAdjective;
				return;
			}
			foreach (TINationState tinationState2 in this.members)
			{
				if (tinationState2.template.unionTrigger > 0)
				{
					this.displayName = tinationState2.template.unionDisplayName;
					this.flagResource = tinationState2.template.GetUnionFlagResource();
					this.displayNameWithArticle = tinationState2.template.unionDisplayNameWithArticle;
					this.displayNameWithArticleCapitalized = Utilities.Capitalize(this.displayNameWithArticle);
					this.adjective = tinationState2.template.unionAdjective;
					return;
				}
			}
			this.displayName = Loc.T("TINationTemplate.GenericFederationDisplayName", new object[] { this.members[0].nationalAdjective });
			this.displayNameWithArticle = Loc.T("TINationTemplate.GenericFederationDisplayNameWithArticle", new object[] { this.members[0].nationalAdjective });
			this.adjective = Loc.T("TINationTemplate.GenericFederationAdjective", new object[] { this.members[0].nationalAdjective });
			this.flagResource = this.members[0].template.GetUnionFlagResource();
			this.displayNameWithArticleCapitalized = Utilities.Capitalize(this.displayNameWithArticle);
		}

		// Token: 0x060036BA RID: 14010 RVA: 0x0013DBE4 File Offset: 0x0013BDE4
		public void RecordAttemptToLeaveDarkFederation(TINationState leavingNation)
		{
			if (this.lastAttemptToLeaveDarkFederation == null)
			{
				this.lastAttemptToLeaveDarkFederation = new Dictionary<TINationState, TIDateTime>();
			}
			if (!this.lastAttemptToLeaveDarkFederation.ContainsKey(leavingNation))
			{
				this.lastAttemptToLeaveDarkFederation.Add(leavingNation, TITimeState.Now());
				return;
			}
			this.lastAttemptToLeaveDarkFederation[leavingNation] = TITimeState.Now();
		}

		// Token: 0x060036BB RID: 14011 RVA: 0x0013DC38 File Offset: 0x0013BE38
		public bool AttemptedToLeaveDarkFederationSince(TINationState nation, float inTheLastXYears)
		{
			if (this.lastAttemptToLeaveDarkFederation == null)
			{
				this.lastAttemptToLeaveDarkFederation = new Dictionary<TINationState, TIDateTime>();
			}
			return this.lastAttemptToLeaveDarkFederation.ContainsKey(nation) && !(this.lastAttemptToLeaveDarkFederation[nation] == null) && !(this.lastAttemptToLeaveDarkFederation[nation] == null) && TITimeState.Now().DifferenceInJulianYears(this.lastAttemptToLeaveDarkFederation[nation]) <= (double)inTheLastXYears;
		}

		// Token: 0x060036BC RID: 14012 RVA: 0x0013DCB0 File Offset: 0x0013BEB0
		public float MemberPooledResource_Year(TINationState nation, FactionResource resource)
		{
			float num = 0f;
			if (resource != FactionResource.Money)
			{
				if (resource == FactionResource.Boost)
				{
					num = this.members.Sum<TINationState>((TINationState x) => x.currentBoost_year);
				}
			}
			else
			{
				num = this.members.Sum<TINationState>((TINationState x) => x.spaceFunding_year);
			}
			float num2 = (float)this.members.Sum<TINationState>((TINationState x) => x.numControlPoints * x.numControlPoints * x.numControlPoints);
			float num3 = (float)(nation.numControlPoints * nation.numControlPoints * nation.numControlPoints) / num2;
			return num * num3;
		}

		// Token: 0x060036BD RID: 14013 RVA: 0x0013DD6C File Offset: 0x0013BF6C
		public bool NetTaker(TINationState member, FactionResource resource)
		{
			if (resource != FactionResource.Money)
			{
				return resource == FactionResource.Boost && member.currentBoost_year < this.MemberPooledResource_Year(member, resource);
			}
			return member.spaceFundingIncome_year < this.MemberPooledResource_Year(member, resource);
		}

		// Token: 0x060036BE RID: 14014 RVA: 0x0013DD9C File Offset: 0x0013BF9C
		public float ECOBonus(TINationState member)
		{
			if (this.members.Contains(member))
			{
				return (float)Mathf.Max(Mathf.RoundToInt((float)Mathd.Pow(this.GDP(member) / 1000000000.0, (double)TemplateManager.global.controlPointCountScaling) / TemplateManager.global.controlPointScalingDivisor), 1);
			}
			return 0f;
		}

		// Token: 0x0400246D RID: 9325
		public string federationName;

		// Token: 0x0400246E RID: 9326
		public string flagResource;

		// Token: 0x0400246F RID: 9327
		public string displayNameWithArticle;

		// Token: 0x04002470 RID: 9328
		public string displayNameWithArticleCapitalized;

		// Token: 0x04002471 RID: 9329
		public string adjective;

		// Token: 0x04002472 RID: 9330
		public Dictionary<TINationState, TIDateTime> lastAttemptToLeaveDarkFederation;

		// Token: 0x04002474 RID: 9332
		public static readonly FactionResource[] federationPooledResources = new FactionResource[]
		{
			FactionResource.Money,
			FactionResource.Boost
		};
	}
}
