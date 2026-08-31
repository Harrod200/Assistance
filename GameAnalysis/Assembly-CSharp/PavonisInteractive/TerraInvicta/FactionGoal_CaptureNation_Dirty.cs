using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200072D RID: 1837
	public class FactionGoal_CaptureNation_Dirty : FactionGoal_CaptureNation
	{
		// Token: 0x06002DA1 RID: 11681 RVA: 0x000FA6AF File Offset: 0x000F88AF
		public FactionGoal_CaptureNation_Dirty()
		{
		}

		// Token: 0x06002DA2 RID: 11682 RVA: 0x000FA6B7 File Offset: 0x000F88B7
		public FactionGoal_CaptureNation_Dirty(TIFactionState faction, int importance, TINationState nation, GoalType manageNationGoal, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.SetImportance(importance);
			base.nation = nation;
			this.objective = objective;
			this.subsequentGoals = new List<GoalType> { manageNationGoal };
		}

		// Token: 0x06002DA3 RID: 11683 RVA: 0x000FA6EF File Offset: 0x000F88EF
		public static FactionGoal_CaptureNation_Dirty CreateGoal(FactionGoal_CaptureNation_Dirty prospectiveGoal)
		{
			FactionGoal_CaptureNation_Dirty factionGoal_CaptureNation_Dirty = GameStateManager.CreateNewGameState<FactionGoal_CaptureNation_Dirty>();
			factionGoal_CaptureNation_Dirty.nation = prospectiveGoal.nation;
			factionGoal_CaptureNation_Dirty.objective = prospectiveGoal.objective;
			return factionGoal_CaptureNation_Dirty;
		}

		// Token: 0x06002DA4 RID: 11684 RVA: 0x000FA70E File Offset: 0x000F890E
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_CaptureNation_Dirty>(base.ID, false);
		}

		// Token: 0x06002DA5 RID: 11685 RVA: 0x000FA71D File Offset: 0x000F891D
		public override GoalType GetGoalType()
		{
			return GoalType.CaptureNationDirty;
		}

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06002DA6 RID: 11686 RVA: 0x000FA721 File Offset: 0x000F8921
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.RegularPolicyNames_SetPolicy;
			}
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06002DA7 RID: 11687 RVA: 0x000FA728 File Offset: 0x000F8928
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.AllPolicyNames_Faction;
			}
		}

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06002DA8 RID: 11688 RVA: 0x000FA72F File Offset: 0x000F892F
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_CaptureNation_Dirty.prioritySettings;
			}
		}

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06002DA9 RID: 11689 RVA: 0x000FA736 File Offset: 0x000F8936
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_CaptureNation_Dirty.missionModifiers;
			}
		}

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x06002DAA RID: 11690 RVA: 0x000FA73D File Offset: 0x000F893D
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_CaptureNation_Dirty.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002DAB RID: 11691 RVA: 0x000FA744 File Offset: 0x000F8944
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			foreach (GoalType goalType in this.subsequentGoals)
			{
				if (goalType != GoalType.ExpandNation)
				{
					if (goalType != GoalType.DevelopNation)
					{
						if (goalType == GoalType.PillageNation)
						{
							list.Add(new FactionGoal_PillageNation(this.faction, base.importance, base.nation));
						}
					}
					else
					{
						list.Add(new FactionGoal_DevelopNation(this.faction, base.importance, base.nation));
					}
				}
				else
				{
					list.Add(new FactionGoal_ExpandNation(this.faction, base.importance, base.nation));
				}
			}
			return list;
		}

		// Token: 0x040021F7 RID: 8695
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 10f },
			{ "Crackdown", 10f },
			{ "DefendInterests", 10f },
			{ "GainInfluence", 10f },
			{ "EnthrallElites", 10f },
			{ "EnthrallPublic", 5f },
			{ "EnthrallUnalignedElites", 10f },
			{ "Propaganda", 5f },
			{ "Purge", 10f },
			{ "SabotageFacilities", 0f },
			{ "Unrest", 10f },
			{ "TerrorizeRegion", 10f },
			{ "Stabilize", 0f },
			{ "Advise", 0f },
			{ "BuildFacility", 2f },
			{ "Abductions", 2f },
			{ "Xenoform", 2f }
		};

		// Token: 0x040021F8 RID: 8696
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int> { 
		{
			PriorityType.Unity,
			3
		} };

		// Token: 0x040021F9 RID: 8697
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.CaptureNationClean,
			GoalType.SupportNation,
			GoalType.NeutralizeNation
		};
	}
}
