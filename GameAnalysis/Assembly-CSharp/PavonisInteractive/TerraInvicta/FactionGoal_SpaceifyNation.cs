using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000733 RID: 1843
	public class FactionGoal_SpaceifyNation : FactionGoal_Nation
	{
		// Token: 0x06002E2C RID: 11820 RVA: 0x000FBFA5 File Offset: 0x000FA1A5
		public FactionGoal_SpaceifyNation()
		{
		}

		// Token: 0x06002E2D RID: 11821 RVA: 0x000FBFAD File Offset: 0x000FA1AD
		public FactionGoal_SpaceifyNation(TIFactionState faction, int importance, TINationState nation)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
		}

		// Token: 0x06002E2E RID: 11822 RVA: 0x000FBFCA File Offset: 0x000FA1CA
		public static FactionGoal_SpaceifyNation CreateGoal(FactionGoal_SpaceifyNation p)
		{
			FactionGoal_SpaceifyNation factionGoal_SpaceifyNation = GameStateManager.CreateNewGameState<FactionGoal_SpaceifyNation>();
			factionGoal_SpaceifyNation.nation = p.nation;
			return factionGoal_SpaceifyNation;
		}

		// Token: 0x06002E2F RID: 11823 RVA: 0x000FBFDD File Offset: 0x000FA1DD
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_SpaceifyNation>(base.ID, false);
		}

		// Token: 0x06002E30 RID: 11824 RVA: 0x000FBFEC File Offset: 0x000FA1EC
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002E31 RID: 11825 RVA: 0x000FBFEF File Offset: 0x000FA1EF
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002E32 RID: 11826 RVA: 0x000FBFF2 File Offset: 0x000FA1F2
		public override GoalType GetGoalType()
		{
			return GoalType.SpaceifyNation;
		}

		// Token: 0x06002E33 RID: 11827 RVA: 0x000FBFF6 File Offset: 0x000FA1F6
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002E34 RID: 11828 RVA: 0x000FC027 File Offset: 0x000FA227
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002E35 RID: 11829 RVA: 0x000FC02F File Offset: 0x000FA22F
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002E36 RID: 11830 RVA: 0x000FC03C File Offset: 0x000FA23C
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002E37 RID: 11831 RVA: 0x000FC044 File Offset: 0x000FA244
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002E38 RID: 11832 RVA: 0x000FC059 File Offset: 0x000FA259
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002E39 RID: 11833 RVA: 0x000FC05C File Offset: 0x000FA25C
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
		}

		// Token: 0x06002E3A RID: 11834 RVA: 0x000FC0A8 File Offset: 0x000FA2A8
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x170006C9 RID: 1737
		// (get) Token: 0x06002E3B RID: 11835 RVA: 0x000FC0AB File Offset: 0x000FA2AB
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006CA RID: 1738
		// (get) Token: 0x06002E3C RID: 11836 RVA: 0x000FC0AE File Offset: 0x000FA2AE
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_SpaceifyNation.missionModifiers;
			}
		}

		// Token: 0x170006CB RID: 1739
		// (get) Token: 0x06002E3D RID: 11837 RVA: 0x000FC0B5 File Offset: 0x000FA2B5
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

		// Token: 0x170006CC RID: 1740
		// (get) Token: 0x06002E3E RID: 11838 RVA: 0x000FC0DC File Offset: 0x000FA2DC
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006CD RID: 1741
		// (get) Token: 0x06002E3F RID: 11839 RVA: 0x000FC0E3 File Offset: 0x000FA2E3
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006CE RID: 1742
		// (get) Token: 0x06002E40 RID: 11840 RVA: 0x000FC0EA File Offset: 0x000FA2EA
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x170006CF RID: 1743
		// (get) Token: 0x06002E41 RID: 11841 RVA: 0x000FC0F1 File Offset: 0x000FA2F1
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_SpaceifyNation.prioritySettings;
			}
		}

		// Token: 0x170006D0 RID: 1744
		// (get) Token: 0x06002E42 RID: 11842 RVA: 0x000FC0F8 File Offset: 0x000FA2F8
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_SpaceifyNation.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002E43 RID: 11843 RVA: 0x000FC0FF File Offset: 0x000FA2FF
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x0400220A RID: 8714
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Advise", 5f },
			{ "Crackdown", 10f },
			{ "DefendInterests", 50f },
			{ "GainInfluence", 10f },
			{ "Propaganda", 5f },
			{ "Purge", 10f },
			{ "Unrest", 0f },
			{ "SabotageFacilities", 0f },
			{ "Protect", 2f },
			{ "Stabilize", 10f }
		};

		// Token: 0x0400220B RID: 8715
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Economy,
				2
			},
			{
				PriorityType.Government,
				1
			},
			{
				PriorityType.Civilian_InitiateSpaceflightProgram,
				3
			},
			{
				PriorityType.LaunchFacilities,
				3
			},
			{
				PriorityType.MissionControl,
				3
			},
			{
				PriorityType.Military_BuildSpaceDefenses,
				1
			},
			{
				PriorityType.Military_BuildSTOSquadron,
				1
			}
		};

		// Token: 0x0400220C RID: 8716
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.ExpandNation,
			GoalType.MilitarizeNation,
			GoalType.DevelopNation
		};
	}
}
