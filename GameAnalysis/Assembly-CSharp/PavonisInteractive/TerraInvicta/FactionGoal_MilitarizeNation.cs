using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000731 RID: 1841
	public class FactionGoal_MilitarizeNation : FactionGoal_Nation
	{
		// Token: 0x06002DFB RID: 11771 RVA: 0x000FBA9B File Offset: 0x000F9C9B
		public FactionGoal_MilitarizeNation()
		{
		}

		// Token: 0x06002DFC RID: 11772 RVA: 0x000FBAA3 File Offset: 0x000F9CA3
		public FactionGoal_MilitarizeNation(TIFactionState faction, int importance, TINationState nation)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
		}

		// Token: 0x06002DFD RID: 11773 RVA: 0x000FBAC0 File Offset: 0x000F9CC0
		public static FactionGoal_MilitarizeNation CreateGoal(FactionGoal_MilitarizeNation p)
		{
			FactionGoal_MilitarizeNation factionGoal_MilitarizeNation = GameStateManager.CreateNewGameState<FactionGoal_MilitarizeNation>();
			factionGoal_MilitarizeNation.nation = p.nation;
			return factionGoal_MilitarizeNation;
		}

		// Token: 0x06002DFE RID: 11774 RVA: 0x000FBAD3 File Offset: 0x000F9CD3
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_MilitarizeNation>(base.ID, false);
		}

		// Token: 0x06002DFF RID: 11775 RVA: 0x000FBAE2 File Offset: 0x000F9CE2
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E00 RID: 11776 RVA: 0x000FBAE5 File Offset: 0x000F9CE5
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002E01 RID: 11777 RVA: 0x000FBAE8 File Offset: 0x000F9CE8
		public override GoalType GetGoalType()
		{
			return GoalType.MilitarizeNation;
		}

		// Token: 0x06002E02 RID: 11778 RVA: 0x000FBAEC File Offset: 0x000F9CEC
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002E03 RID: 11779 RVA: 0x000FBB1D File Offset: 0x000F9D1D
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002E04 RID: 11780 RVA: 0x000FBB25 File Offset: 0x000F9D25
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002E05 RID: 11781 RVA: 0x000FBB32 File Offset: 0x000F9D32
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002E06 RID: 11782 RVA: 0x000FBB3A File Offset: 0x000F9D3A
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal();
		}

		// Token: 0x06002E07 RID: 11783 RVA: 0x000FBB45 File Offset: 0x000F9D45
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002E08 RID: 11784 RVA: 0x000FBB48 File Offset: 0x000F9D48
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
		}

		// Token: 0x06002E09 RID: 11785 RVA: 0x000FBB94 File Offset: 0x000F9D94
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06002E0A RID: 11786 RVA: 0x000FBB97 File Offset: 0x000F9D97
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06002E0B RID: 11787 RVA: 0x000FBB9A File Offset: 0x000F9D9A
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_MilitarizeNation.missionModifiers;
			}
		}

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06002E0C RID: 11788 RVA: 0x000FBBA1 File Offset: 0x000F9DA1
		public override List<PolicyType> policiesAsNation
		{
			get
			{
				return PolicyManager.RegularPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x06002E0D RID: 11789 RVA: 0x000FBBA8 File Offset: 0x000F9DA8
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x06002E0E RID: 11790 RVA: 0x000FBBAF File Offset: 0x000F9DAF
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006C0 RID: 1728
		// (get) Token: 0x06002E0F RID: 11791 RVA: 0x000FBBB6 File Offset: 0x000F9DB6
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006C1 RID: 1729
		// (get) Token: 0x06002E10 RID: 11792 RVA: 0x000FBBBD File Offset: 0x000F9DBD
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_MilitarizeNation.prioritySettings;
			}
		}

		// Token: 0x170006C2 RID: 1730
		// (get) Token: 0x06002E11 RID: 11793 RVA: 0x000FBBC4 File Offset: 0x000F9DC4
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_MilitarizeNation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002E12 RID: 11794 RVA: 0x000FBBCB File Offset: 0x000F9DCB
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x04002205 RID: 8709
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Advise", 10f },
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

		// Token: 0x04002206 RID: 8710
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Military_BuildArmy,
				3
			},
			{
				PriorityType.Military_BuildNuclearWeapons,
				3
			},
			{
				PriorityType.Military_BuildSpaceDefenses,
				3
			},
			{
				PriorityType.Military_InitiateNuclearProgram,
				3
			},
			{
				PriorityType.Military_BuildNavy,
				3
			},
			{
				PriorityType.Military_BuildSTOSquadron,
				3
			}
		};

		// Token: 0x04002207 RID: 8711
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.ExpandNation,
			GoalType.DevelopNation,
			GoalType.SpaceifyNation
		};
	}
}
