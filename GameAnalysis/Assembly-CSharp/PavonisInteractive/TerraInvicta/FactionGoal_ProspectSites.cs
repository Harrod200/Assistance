using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200073C RID: 1852
	public class FactionGoal_ProspectSites : FactionGoal_Space
	{
		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x06002EF5 RID: 12021 RVA: 0x001020F5 File Offset: 0x001002F5
		// (set) Token: 0x06002EF6 RID: 12022 RVA: 0x001020FD File Offset: 0x001002FD
		public bool requireFleet { get; protected set; }

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06002EF7 RID: 12023 RVA: 0x00102106 File Offset: 0x00100306
		// (set) Token: 0x06002EF8 RID: 12024 RVA: 0x0010210E File Offset: 0x0010030E
		public TISpaceBodyState targetSpaceBody { get; private set; }

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06002EF9 RID: 12025 RVA: 0x00102117 File Offset: 0x00100317
		// (set) Token: 0x06002EFA RID: 12026 RVA: 0x0010211F File Offset: 0x0010031F
		public GoalType buildBaseGoal { get; private set; }

		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06002EFB RID: 12027 RVA: 0x00102128 File Offset: 0x00100328
		// (set) Token: 0x06002EFC RID: 12028 RVA: 0x00102130 File Offset: 0x00100330
		public GoalType buildStationGoal { get; private set; }

		// Token: 0x06002EFD RID: 12029 RVA: 0x00102139 File Offset: 0x00100339
		public FactionGoal_ProspectSites()
		{
		}

		// Token: 0x06002EFE RID: 12030 RVA: 0x00102144 File Offset: 0x00100344
		public FactionGoal_ProspectSites(TIFactionState faction, int importance, TISpaceBodyState targetSpaceBody, bool requireFleet, GoalType foundBaseGoal, GoalType buildBaseGoal, GoalType buildStationGoal)
		{
			this.faction = faction;
			base.SetImportance(importance);
			this.targetSpaceBody = targetSpaceBody;
			this.subsequentGoals = new List<GoalType> { foundBaseGoal };
			this.buildBaseGoal = buildBaseGoal;
			this.buildStationGoal = buildStationGoal;
			this.requireFleet = requireFleet;
		}

		// Token: 0x06002EFF RID: 12031 RVA: 0x00102197 File Offset: 0x00100397
		public static FactionGoal_ProspectSites CreateGoal(FactionGoal_ProspectSites p)
		{
			FactionGoal_ProspectSites factionGoal_ProspectSites = GameStateManager.CreateNewGameState<FactionGoal_ProspectSites>();
			factionGoal_ProspectSites.targetSpaceBody = p.targetSpaceBody;
			factionGoal_ProspectSites.buildBaseGoal = p.buildBaseGoal;
			factionGoal_ProspectSites.buildStationGoal = p.buildStationGoal;
			factionGoal_ProspectSites.requireFleet = p.requireFleet;
			return factionGoal_ProspectSites;
		}

		// Token: 0x06002F00 RID: 12032 RVA: 0x001021CE File Offset: 0x001003CE
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_ProspectSites>(base.ID, false);
		}

		// Token: 0x06002F01 RID: 12033 RVA: 0x001021DD File Offset: 0x001003DD
		public override GoalType GetGoalType()
		{
			return GoalType.ProspectSites;
		}

		// Token: 0x06002F02 RID: 12034 RVA: 0x001021E0 File Offset: 0x001003E0
		public override TIGameState actor()
		{
			if (!(base.assignedFleet != null))
			{
				return this.faction.ref_gameState;
			}
			return base.assignedFleet.ref_gameState;
		}

		// Token: 0x06002F03 RID: 12035 RVA: 0x00102207 File Offset: 0x00100407
		public override TIGameState target()
		{
			return this.targetSpaceBody;
		}

		// Token: 0x06002F04 RID: 12036 RVA: 0x0010220F File Offset: 0x0010040F
		public override TIGameState location()
		{
			return this.targetSpaceBody;
		}

		// Token: 0x06002F05 RID: 12037 RVA: 0x00102217 File Offset: 0x00100417
		public override TIGameState goalProduct()
		{
			return this.targetSpaceBody;
		}

		// Token: 0x06002F06 RID: 12038 RVA: 0x0010221F File Offset: 0x0010041F
		public override bool RequiresFleet()
		{
			return this.requireFleet;
		}

		// Token: 0x06002F07 RID: 12039 RVA: 0x00102227 File Offset: 0x00100427
		public override bool ValidNewGoal()
		{
			return this.faction.CandidateForProspecting(this.targetSpaceBody);
		}

		// Token: 0x06002F08 RID: 12040 RVA: 0x0010223A File Offset: 0x0010043A
		public override bool InProgress()
		{
			return base.assignedFleet != null || this.faction.ProspectingSpaceBody(this.targetSpaceBody);
		}

		// Token: 0x06002F09 RID: 12041 RVA: 0x0010225D File Offset: 0x0010045D
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || this.targetSpaceBody == null || !this.faction.CandidateForProspecting(this.targetSpaceBody);
		}

		// Token: 0x06002F0A RID: 12042 RVA: 0x0010228C File Offset: 0x0010048C
		public override bool GoalFulfilled()
		{
			return !this.faction.CandidateForProspecting(this.targetSpaceBody);
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06002F0B RID: 12043 RVA: 0x001022A2 File Offset: 0x001004A2
		public override List<Type> fleetOperations
		{
			get
			{
				return FactionGoal_ProspectSites.fleetOps;
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06002F0C RID: 12044 RVA: 0x001022A9 File Offset: 0x001004A9
		public override List<Type> spaceOperations
		{
			get
			{
				return FactionGoal_ProspectSites.spaceOps;
			}
		}

		// Token: 0x17000704 RID: 1796
		// (get) Token: 0x06002F0D RID: 12045 RVA: 0x001022B0 File Offset: 0x001004B0
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06002F0E RID: 12046 RVA: 0x001022B3 File Offset: 0x001004B3
		public override void ChangeTarget(TIGameState newTarget)
		{
			if (newTarget != null && newTarget.isSpaceBodyState)
			{
				this.targetSpaceBody = newTarget.ref_spaceBody;
			}
			this.targetSpaceBody = null;
		}

		// Token: 0x06002F0F RID: 12047 RVA: 0x001022D4 File Offset: 0x001004D4
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			using (List<GoalType>.Enumerator enumerator = this.subsequentGoals.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == GoalType.FoundBase)
					{
						list.Add(new FactionGoal_FoundBase(this.faction, base.importance, this.targetSpaceBody.habSites.MaxBy<TIHabSiteState, float>((TIHabSiteState x) => AIEvaluators.EvaluateHabSite(this.faction, x, false, false, true)), this.buildBaseGoal, null, this.buildStationGoal, false, null));
					}
				}
			}
			return list;
		}

		// Token: 0x06002F10 RID: 12048 RVA: 0x0010236C File Offset: 0x0010056C
		public override ShipRole GetPrimaryShipRole()
		{
			return ShipRole.Explorer;
		}

		// Token: 0x06002F11 RID: 12049 RVA: 0x0010236F File Offset: 0x0010056F
		public override Dictionary<ShipRole, float> GetSecondaryShipRoles()
		{
			return FactionGoal_ProspectSites.preferredShipRoles;
		}

		// Token: 0x06002F12 RID: 12050 RVA: 0x00102376 File Offset: 0x00100576
		public override float GetDesiredAssaultCombatValue()
		{
			return 0f;
		}

		// Token: 0x04002231 RID: 8753
		private static readonly List<Type> fleetOps = new List<Type>(FactionGoal_Fleet.coreFleetOpsList) { typeof(SurveyPlanetFromFleetOperation) };

		// Token: 0x04002232 RID: 8754
		private static readonly List<Type> spaceOps = new List<Type> { typeof(LaunchProbeOperation) };

		// Token: 0x04002233 RID: 8755
		private static readonly Dictionary<ShipRole, float> preferredShipRoles = new Dictionary<ShipRole, float>
		{
			{
				ShipRole.LM_Interdictor,
				1f
			},
			{
				ShipRole.LL_Intruder,
				1f
			}
		};
	}
}
