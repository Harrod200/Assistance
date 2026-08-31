using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000732 RID: 1842
	public class FactionGoal_SupportNation : FactionGoal_Nation
	{
		// Token: 0x06002E14 RID: 11796 RVA: 0x000FBD39 File Offset: 0x000F9F39
		public FactionGoal_SupportNation()
		{
		}

		// Token: 0x06002E15 RID: 11797 RVA: 0x000FBD41 File Offset: 0x000F9F41
		public FactionGoal_SupportNation(TIFactionState faction, int importance, TINationState nation)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
		}

		// Token: 0x06002E16 RID: 11798 RVA: 0x000FBD5E File Offset: 0x000F9F5E
		public static FactionGoal_SupportNation CreateGoal(FactionGoal_SupportNation p)
		{
			FactionGoal_SupportNation factionGoal_SupportNation = GameStateManager.CreateNewGameState<FactionGoal_SupportNation>();
			factionGoal_SupportNation.nation = p.nation;
			return factionGoal_SupportNation;
		}

		// Token: 0x06002E17 RID: 11799 RVA: 0x000FBD71 File Offset: 0x000F9F71
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_SupportNation>(base.ID, false);
		}

		// Token: 0x06002E18 RID: 11800 RVA: 0x000FBD80 File Offset: 0x000F9F80
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E19 RID: 11801 RVA: 0x000FBD83 File Offset: 0x000F9F83
		public override bool PoliciesAsNationGoal()
		{
			return false;
		}

		// Token: 0x06002E1A RID: 11802 RVA: 0x000FBD86 File Offset: 0x000F9F86
		public override bool NationPrioritiesGoal()
		{
			return false;
		}

		// Token: 0x06002E1B RID: 11803 RVA: 0x000FBD89 File Offset: 0x000F9F89
		public override GoalType GetGoalType()
		{
			return GoalType.SupportNation;
		}

		// Token: 0x06002E1C RID: 11804 RVA: 0x000FBD8D File Offset: 0x000F9F8D
		public override TIGameState actor()
		{
			return this.faction.ref_gameState;
		}

		// Token: 0x06002E1D RID: 11805 RVA: 0x000FBD9A File Offset: 0x000F9F9A
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002E1E RID: 11806 RVA: 0x000FBDA2 File Offset: 0x000F9FA2
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002E1F RID: 11807 RVA: 0x000FBDAF File Offset: 0x000F9FAF
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002E20 RID: 11808 RVA: 0x000FBDB7 File Offset: 0x000F9FB7
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002E21 RID: 11809 RVA: 0x000FBDCC File Offset: 0x000F9FCC
		public override bool InProgress()
		{
			return base.nation.extant;
		}

		// Token: 0x06002E22 RID: 11810 RVA: 0x000FBDD9 File Offset: 0x000F9FD9
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant || base.nation.executiveFaction == this.faction;
		}

		// Token: 0x06002E23 RID: 11811 RVA: 0x000FBE17 File Offset: 0x000FA017
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x170006C3 RID: 1731
		// (get) Token: 0x06002E24 RID: 11812 RVA: 0x000FBE1A File Offset: 0x000FA01A
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006C4 RID: 1732
		// (get) Token: 0x06002E25 RID: 11813 RVA: 0x000FBE1D File Offset: 0x000FA01D
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_SupportNation.missionModifiers;
			}
		}

		// Token: 0x170006C5 RID: 1733
		// (get) Token: 0x06002E26 RID: 11814 RVA: 0x000FBE24 File Offset: 0x000FA024
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006C6 RID: 1734
		// (get) Token: 0x06002E27 RID: 11815 RVA: 0x000FBE2B File Offset: 0x000FA02B
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006C7 RID: 1735
		// (get) Token: 0x06002E28 RID: 11816 RVA: 0x000FBE32 File Offset: 0x000FA032
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return new Dictionary<PriorityType, int>();
			}
		}

		// Token: 0x170006C8 RID: 1736
		// (get) Token: 0x06002E29 RID: 11817 RVA: 0x000FBE39 File Offset: 0x000FA039
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_SupportNation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002E2A RID: 11818 RVA: 0x000FBE40 File Offset: 0x000FA040
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x04002208 RID: 8712
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Advise", 5f },
			{ "Crackdown", 0f },
			{ "DefendInterests", 0f },
			{ "GainInfluence", 0f },
			{ "EnthrallElites", 0f },
			{ "EnthrallPublic", 0f },
			{ "EnthrallUnalignedElites", 0f },
			{ "Propaganda", 0f },
			{ "Purge", 0f },
			{ "Unrest", 0f },
			{ "SabotageFacilities", 0f },
			{ "TerrorizeRegion", 0f },
			{ "Stabilize", 10f },
			{ "BuildFacility", 10f },
			{ "AssumeControl", 10f }
		};

		// Token: 0x04002209 RID: 8713
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.ExpandNation,
			GoalType.DevelopNation,
			GoalType.SpaceifyNation,
			GoalType.CaptureNationClean,
			GoalType.CaptureNationDirty,
			GoalType.MilitarizeNation
		};
	}
}
