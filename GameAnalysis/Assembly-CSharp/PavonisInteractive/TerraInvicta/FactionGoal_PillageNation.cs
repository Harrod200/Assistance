using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000734 RID: 1844
	public class FactionGoal_PillageNation : FactionGoal_Nation
	{
		// Token: 0x06002E45 RID: 11845 RVA: 0x000FC244 File Offset: 0x000FA444
		public FactionGoal_PillageNation()
		{
		}

		// Token: 0x06002E46 RID: 11846 RVA: 0x000FC24C File Offset: 0x000FA44C
		public FactionGoal_PillageNation(TIFactionState faction, int importance, TINationState nation)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
		}

		// Token: 0x06002E47 RID: 11847 RVA: 0x000FC269 File Offset: 0x000FA469
		public static FactionGoal_PillageNation CreateGoal(FactionGoal_PillageNation p)
		{
			FactionGoal_PillageNation factionGoal_PillageNation = GameStateManager.CreateNewGameState<FactionGoal_PillageNation>();
			factionGoal_PillageNation.nation = p.nation;
			return factionGoal_PillageNation;
		}

		// Token: 0x06002E48 RID: 11848 RVA: 0x000FC27C File Offset: 0x000FA47C
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_PillageNation>(base.ID, false);
		}

		// Token: 0x06002E49 RID: 11849 RVA: 0x000FC28B File Offset: 0x000FA48B
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E4A RID: 11850 RVA: 0x000FC28E File Offset: 0x000FA48E
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002E4B RID: 11851 RVA: 0x000FC291 File Offset: 0x000FA491
		public override GoalType GetGoalType()
		{
			return GoalType.PillageNation;
		}

		// Token: 0x06002E4C RID: 11852 RVA: 0x000FC295 File Offset: 0x000FA495
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002E4D RID: 11853 RVA: 0x000FC2C6 File Offset: 0x000FA4C6
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002E4E RID: 11854 RVA: 0x000FC2CE File Offset: 0x000FA4CE
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002E4F RID: 11855 RVA: 0x000FC2DB File Offset: 0x000FA4DB
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002E50 RID: 11856 RVA: 0x000FC2E3 File Offset: 0x000FA4E3
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002E51 RID: 11857 RVA: 0x000FC2F8 File Offset: 0x000FA4F8
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002E52 RID: 11858 RVA: 0x000FC2FC File Offset: 0x000FA4FC
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
		}

		// Token: 0x06002E53 RID: 11859 RVA: 0x000FC348 File Offset: 0x000FA548
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x170006D1 RID: 1745
		// (get) Token: 0x06002E54 RID: 11860 RVA: 0x000FC34B File Offset: 0x000FA54B
		public override List<PolicyType> policiesAsNation
		{
			get
			{
				return FactionGoal_PillageNation.executivepolicies_SetPolicy;
			}
		}

		// Token: 0x170006D2 RID: 1746
		// (get) Token: 0x06002E55 RID: 11861 RVA: 0x000FC352 File Offset: 0x000FA552
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006D3 RID: 1747
		// (get) Token: 0x06002E56 RID: 11862 RVA: 0x000FC359 File Offset: 0x000FA559
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return new List<PolicyType>();
			}
		}

		// Token: 0x170006D4 RID: 1748
		// (get) Token: 0x06002E57 RID: 11863 RVA: 0x000FC360 File Offset: 0x000FA560
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.DegradeRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006D5 RID: 1749
		// (get) Token: 0x06002E58 RID: 11864 RVA: 0x000FC367 File Offset: 0x000FA567
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006D6 RID: 1750
		// (get) Token: 0x06002E59 RID: 11865 RVA: 0x000FC36A File Offset: 0x000FA56A
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_PillageNation.missionModifiers;
			}
		}

		// Token: 0x170006D7 RID: 1751
		// (get) Token: 0x06002E5A RID: 11866 RVA: 0x000FC371 File Offset: 0x000FA571
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_PillageNation.prioritySettings;
			}
		}

		// Token: 0x170006D8 RID: 1752
		// (get) Token: 0x06002E5B RID: 11867 RVA: 0x000FC378 File Offset: 0x000FA578
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_PillageNation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002E5C RID: 11868 RVA: 0x000FC37F File Offset: 0x000FA57F
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x0400220D RID: 8717
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Advise", 0f },
			{ "DefendInterests", 0f },
			{ "Stabilize", 0f }
		};

		// Token: 0x0400220E RID: 8718
		private static readonly List<PolicyType> executivepolicies_SetPolicy = new List<PolicyType>
		{
			PolicyType.PeacefulBreakupOption,
			PolicyType.TransferRegionsOption
		};

		// Token: 0x0400220F RID: 8719
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Economy,
				0
			},
			{
				PriorityType.Welfare,
				0
			},
			{
				PriorityType.Military_BuildArmy,
				0
			},
			{
				PriorityType.Military_BuildNuclearWeapons,
				0
			},
			{
				PriorityType.Military_BuildSpaceDefenses,
				0
			},
			{
				PriorityType.Military_InitiateNuclearProgram,
				0
			},
			{
				PriorityType.Knowledge,
				0
			},
			{
				PriorityType.LaunchFacilities,
				0
			},
			{
				PriorityType.Military,
				0
			},
			{
				PriorityType.MissionControl,
				0
			},
			{
				PriorityType.Funding,
				0
			},
			{
				PriorityType.Civilian_InitiateSpaceflightProgram,
				0
			},
			{
				PriorityType.Spoils,
				3
			},
			{
				PriorityType.Unity,
				0
			},
			{
				PriorityType.Military_BuildNavy,
				0
			}
		};

		// Token: 0x04002210 RID: 8720
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.NeutralizeNation,
			GoalType.DevelopNation,
			GoalType.ExpandNation,
			GoalType.MilitarizeNation,
			GoalType.SpaceifyNation
		};
	}
}
