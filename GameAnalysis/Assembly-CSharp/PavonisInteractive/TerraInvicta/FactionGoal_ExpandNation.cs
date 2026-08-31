using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200072F RID: 1839
	public class FactionGoal_ExpandNation : FactionGoal_Nation
	{
		// Token: 0x06002DC9 RID: 11721 RVA: 0x000FB48E File Offset: 0x000F968E
		public FactionGoal_ExpandNation()
		{
		}

		// Token: 0x06002DCA RID: 11722 RVA: 0x000FB496 File Offset: 0x000F9696
		public FactionGoal_ExpandNation(TIFactionState faction, int importance, TINationState nation)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
		}

		// Token: 0x06002DCB RID: 11723 RVA: 0x000FB4B3 File Offset: 0x000F96B3
		public static FactionGoal_ExpandNation CreateGoal(FactionGoal_ExpandNation prospectiveGoal)
		{
			FactionGoal_ExpandNation factionGoal_ExpandNation = GameStateManager.CreateNewGameState<FactionGoal_ExpandNation>();
			factionGoal_ExpandNation.nation = prospectiveGoal.nation;
			return factionGoal_ExpandNation;
		}

		// Token: 0x06002DCC RID: 11724 RVA: 0x000FB4C6 File Offset: 0x000F96C6
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_ExpandNation>(base.ID, false);
		}

		// Token: 0x06002DCD RID: 11725 RVA: 0x000FB4D5 File Offset: 0x000F96D5
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002DCE RID: 11726 RVA: 0x000FB4D8 File Offset: 0x000F96D8
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002DCF RID: 11727 RVA: 0x000FB4DB File Offset: 0x000F96DB
		public override GoalType GetGoalType()
		{
			return GoalType.ExpandNation;
		}

		// Token: 0x06002DD0 RID: 11728 RVA: 0x000FB4DF File Offset: 0x000F96DF
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002DD1 RID: 11729 RVA: 0x000FB510 File Offset: 0x000F9710
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002DD2 RID: 11730 RVA: 0x000FB518 File Offset: 0x000F9718
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002DD3 RID: 11731 RVA: 0x000FB525 File Offset: 0x000F9725
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002DD4 RID: 11732 RVA: 0x000FB52D File Offset: 0x000F972D
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002DD5 RID: 11733 RVA: 0x000FB542 File Offset: 0x000F9742
		public override bool InProgress()
		{
			return true;
		}

		// Token: 0x06002DD6 RID: 11734 RVA: 0x000FB548 File Offset: 0x000F9748
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant || !base.nation.FactionsWithControlPoint.Contains(this.faction);
		}

		// Token: 0x06002DD7 RID: 11735 RVA: 0x000FB594 File Offset: 0x000F9794
		public override bool GoalFulfilled()
		{
			return false;
		}

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x06002DD8 RID: 11736 RVA: 0x000FB597 File Offset: 0x000F9797
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_ExpandNation.missionModifiers;
			}
		}

		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06002DD9 RID: 11737 RVA: 0x000FB59E File Offset: 0x000F979E
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06002DDA RID: 11738 RVA: 0x000FB5A1 File Offset: 0x000F97A1
		public override List<PolicyType> policiesAsNation
		{
			get
			{
				return PolicyManager.RegularPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06002DDB RID: 11739 RVA: 0x000FB5A8 File Offset: 0x000F97A8
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.AllPolicyNames_Faction;
			}
		}

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06002DDC RID: 11740 RVA: 0x000FB5AF File Offset: 0x000F97AF
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.RegularPolicyNames_SetPolicy;
			}
		}

		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x06002DDD RID: 11741 RVA: 0x000FB5B6 File Offset: 0x000F97B6
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.AllPolicyNames_Faction;
			}
		}

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x06002DDE RID: 11742 RVA: 0x000FB5C0 File Offset: 0x000F97C0
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				if (base.nation.wars.Count <= 0)
				{
					if (!(from x in base.nation.ExternalClaims()
						select x.nation).Intersect<TINationState>(base.nation.rivals).Any<TINationState>())
					{
						return FactionGoal_ExpandNation.prioritySettings_peaceful;
					}
				}
				return FactionGoal_ExpandNation.prioritySettings_war;
			}
		}

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x06002DDF RID: 11743 RVA: 0x000FB631 File Offset: 0x000F9831
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_ExpandNation.incompatibleGoalsForNation;
			}
		}

		// Token: 0x06002DE0 RID: 11744 RVA: 0x000FB638 File Offset: 0x000F9838
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			return null;
		}

		// Token: 0x040021FE RID: 8702
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Advise", 5f },
			{ "Crackdown", 10f },
			{ "DefendInterests", 100f },
			{ "GainInfluence", 10f },
			{ "EnthrallElites", 10f },
			{ "EnthrallPublic", 10f },
			{ "EnthrallUnalignedElites", 10f },
			{ "Propaganda", 5f },
			{ "Purge", 10f },
			{ "Unrest", 0f },
			{ "SabotageFacilities", 0f },
			{ "TerrorizeRegion", 0f },
			{ "Stabilize", 5f }
		};

		// Token: 0x040021FF RID: 8703
		private static readonly Dictionary<PriorityType, int> prioritySettings_war = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Military_FoundMilitary,
				3
			},
			{
				PriorityType.Military_BuildArmy,
				3
			},
			{
				PriorityType.Military_BuildSpaceDefenses,
				2
			},
			{
				PriorityType.Military_BuildNavy,
				3
			},
			{
				PriorityType.Military,
				3
			}
		};

		// Token: 0x04002200 RID: 8704
		private static readonly Dictionary<PriorityType, int> prioritySettings_peaceful = new Dictionary<PriorityType, int>
		{
			{
				PriorityType.Economy,
				2
			},
			{
				PriorityType.Welfare,
				2
			},
			{
				PriorityType.Knowledge,
				2
			},
			{
				PriorityType.Military_FoundMilitary,
				1
			},
			{
				PriorityType.Military,
				1
			},
			{
				PriorityType.Military_BuildSpaceDefenses,
				1
			}
		};

		// Token: 0x04002201 RID: 8705
		private static readonly List<GoalType> incompatibleGoalsForNation = new List<GoalType>
		{
			GoalType.NeutralizeNation,
			GoalType.PillageNation,
			GoalType.DevelopNation,
			GoalType.MilitarizeNation,
			GoalType.SpaceifyNation
		};
	}
}
