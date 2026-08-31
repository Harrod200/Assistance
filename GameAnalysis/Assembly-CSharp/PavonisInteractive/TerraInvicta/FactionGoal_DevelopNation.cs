using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000730 RID: 1840
	public class FactionGoal_DevelopNation : FactionGoal_Nation
	{
		// Token: 0x06002DE2 RID: 11746 RVA: 0x000FB7D9 File Offset: 0x000F99D9
		public FactionGoal_DevelopNation()
		{
		}

		// Token: 0x06002DE3 RID: 11747 RVA: 0x000FB7E1 File Offset: 0x000F99E1
		public FactionGoal_DevelopNation(TIFactionState faction, int importance, TINationState nation)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
		}

		// Token: 0x06002DE4 RID: 11748 RVA: 0x000FB7FE File Offset: 0x000F99FE
		public static FactionGoal_DevelopNation CreateGoal(FactionGoal_DevelopNation p)
		{
			FactionGoal_DevelopNation factionGoal_DevelopNation = GameStateManager.CreateNewGameState<FactionGoal_DevelopNation>();
			factionGoal_DevelopNation.nation = p.nation;
			return factionGoal_DevelopNation;
		}

		// Token: 0x06002DE5 RID: 11749 RVA: 0x000FB811 File Offset: 0x000F9A11
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_DevelopNation>(base.ID, false);
		}

		// Token: 0x06002DE6 RID: 11750 RVA: 0x000FB820 File Offset: 0x000F9A20
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002DE7 RID: 11751 RVA: 0x000FB823 File Offset: 0x000F9A23
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002DE8 RID: 11752 RVA: 0x000FB826 File Offset: 0x000F9A26
		public override GoalType GetGoalType()
		{
			return GoalType.DevelopNation;
		}

		// Token: 0x06002DE9 RID: 11753 RVA: 0x000FB82A File Offset: 0x000F9A2A
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002DEA RID: 11754 RVA: 0x000FB85B File Offset: 0x000F9A5B
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002DEB RID: 11755 RVA: 0x000FB863 File Offset: 0x000F9A63
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002DEC RID: 11756 RVA: 0x000FB870 File Offset: 0x000F9A70
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002DED RID: 11757 RVA: 0x000FB878 File Offset: 0x000F9A78
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002DEE RID: 11758 RVA: 0x000FB88D File Offset: 0x000F9A8D
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002DEF RID: 11759 RVA: 0x000FB890 File Offset: 0x000F9A90
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
		}

		// Token: 0x06002DF0 RID: 11760 RVA: 0x000FB8DC File Offset: 0x000F9ADC
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06002DF1 RID: 11761 RVA: 0x000FB8DF File Offset: 0x000F9ADF
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06002DF2 RID: 11762 RVA: 0x000FB8E2 File Offset: 0x000F9AE2
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_DevelopNation.missionModifiers;
			}
		}

		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06002DF3 RID: 11763 RVA: 0x000FB8E9 File Offset: 0x000F9AE9
		public override List<PolicyType> policiesAsNation
		{
			get
			{
				if (!base.nation.civilWar || !base.nation.cohesionWarning)
				{
					return PolicyManager.RegularPolicyNames_SetPolicy;
				}
				return PolicyManager.StabilizeNationPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06002DF4 RID: 11764 RVA: 0x000FB910 File Offset: 0x000F9B10
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x06002DF5 RID: 11765 RVA: 0x000FB917 File Offset: 0x000F9B17
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x06002DF6 RID: 11766 RVA: 0x000FB91E File Offset: 0x000F9B1E
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06002DF7 RID: 11767 RVA: 0x000FB925 File Offset: 0x000F9B25
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_DevelopNation.prioritySettings;
			}
		}

		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06002DF8 RID: 11768 RVA: 0x000FB92C File Offset: 0x000F9B2C
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_DevelopNation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002DF9 RID: 11769 RVA: 0x000FB933 File Offset: 0x000F9B33
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x04002202 RID: 8706
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Advise", 5f },
			{ "Crackdown", 10f },
			{ "DefendInterests", 50f },
			{ "GainInfluence", 10f },
			{ "EnthrallElites", 10f },
			{ "EnthrallPublic", 5f },
			{ "EnthrallUnalignedElites", 10f },
			{ "Propaganda", 5f },
			{ "Purge", 10f },
			{ "Unrest", 0f },
			{ "SabotageFacilities", 0f },
			{ "TerrorizeRegion", 0f },
			{ "Stabilize", 10f }
		};

		// Token: 0x04002203 RID: 8707
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Economy,
				3
			},
			{
				PriorityType.Welfare,
				2
			},
			{
				PriorityType.Knowledge,
				3
			},
			{
				PriorityType.Government,
				3
			},
			{
				PriorityType.Environment,
				2
			},
			{
				PriorityType.Spoils,
				0
			}
		};

		// Token: 0x04002204 RID: 8708
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.ExpandNation,
			GoalType.MilitarizeNation,
			GoalType.SpaceifyNation
		};
	}
}
