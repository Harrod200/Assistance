using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200072C RID: 1836
	public class FactionGoal_CaptureNation_Clean : FactionGoal_CaptureNation
	{
		// Token: 0x06002D95 RID: 11669 RVA: 0x000FA3C0 File Offset: 0x000F85C0
		public FactionGoal_CaptureNation_Clean()
		{
		}

		// Token: 0x06002D96 RID: 11670 RVA: 0x000FA3C8 File Offset: 0x000F85C8
		public FactionGoal_CaptureNation_Clean(TIFactionState faction, int importance, TINationState nation, GoalType manageNationGoal, TIObjectiveTemplate objective = null)
		{
			this.faction = faction;
			base.nation = nation;
			base.SetImportance(importance);
			this.objective = objective;
			this.subsequentGoals = new List<GoalType> { manageNationGoal };
		}

		// Token: 0x06002D97 RID: 11671 RVA: 0x000FA400 File Offset: 0x000F8600
		public override void RemoveState()
		{
			GameStateManager.RemoveGameState<FactionGoal_CaptureNation_Clean>(base.ID, false);
		}

		// Token: 0x06002D98 RID: 11672 RVA: 0x000FA40F File Offset: 0x000F860F
		public static FactionGoal_CaptureNation_Clean CreateGoal(FactionGoal_CaptureNation_Clean prospectiveGoal)
		{
			FactionGoal_CaptureNation_Clean factionGoal_CaptureNation_Clean = GameStateManager.CreateNewGameState<FactionGoal_CaptureNation_Clean>();
			factionGoal_CaptureNation_Clean.nation = prospectiveGoal.nation;
			factionGoal_CaptureNation_Clean.objective = prospectiveGoal.objective;
			return factionGoal_CaptureNation_Clean;
		}

		// Token: 0x06002D99 RID: 11673 RVA: 0x000FA42E File Offset: 0x000F862E
		public override GoalType GetGoalType()
		{
			return GoalType.CaptureNationClean;
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06002D9A RID: 11674 RVA: 0x000FA432 File Offset: 0x000F8632
		public override List<PolicyType> policiesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_SetPolicy;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x06002D9B RID: 11675 RVA: 0x000FA439 File Offset: 0x000F8639
		public override List<PolicyType> factionLevelPoliciesAtTarget
		{
			get
			{
				return PolicyManager.ImproveRelationsPolicyNames_Faction;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06002D9C RID: 11676 RVA: 0x000FA440 File Offset: 0x000F8640
		public override Dictionary<string, float> missionPayoffMultipliersAgainstTarget
		{
			get
			{
				return FactionGoal_CaptureNation_Clean.missionModifiers;
			}
		}

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06002D9D RID: 11677 RVA: 0x000FA447 File Offset: 0x000F8647
		public override Dictionary<PriorityType, int> prioritiesAsNation
		{
			get
			{
				return FactionGoal_CaptureNation_Clean.prioritySettings;
			}
		}

		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06002D9E RID: 11678 RVA: 0x000FA44E File Offset: 0x000F864E
		public override List<GoalType> incompatibleGoals
		{
			get
			{
				return FactionGoal_CaptureNation_Clean.incompatibleGoalsForTarget;
			}
		}

		// Token: 0x06002D9F RID: 11679 RVA: 0x000FA458 File Offset: 0x000F8658
		public override List<TIFactionGoalState> BuildSubsequentGoals()
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			using (List<GoalType>.Enumerator enumerator = this.subsequentGoals.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case GoalType.ExpandNation:
						list.Add(new FactionGoal_ExpandNation(this.faction, base.importance, base.nation));
						break;
					case GoalType.DevelopNation:
						list.Add(new FactionGoal_DevelopNation(this.faction, base.importance, base.nation));
						break;
					case GoalType.MilitarizeNation:
						list.Add(new FactionGoal_MilitarizeNation(this.faction, base.importance, base.nation));
						break;
					case GoalType.PillageNation:
						list.Add(new FactionGoal_PillageNation(this.faction, base.importance, base.nation));
						break;
					case GoalType.SpaceifyNation:
						list.Add(new FactionGoal_SpaceifyNation(this.faction, base.importance, base.nation));
						break;
					}
				}
			}
			return list;
		}

		// Token: 0x040021F4 RID: 8692
		private static readonly Dictionary<string, float> missionModifiers = new Dictionary<string, float>
		{
			{ "Coup", 0f },
			{ "Crackdown", 10f },
			{ "GainInfluence", 10f },
			{ "DefendInterests", 10f },
			{ "EnthrallElites", 10f },
			{ "EnthrallPublic", 5f },
			{ "EnthrallUnalignedElites", 10f },
			{ "Propaganda", 5f },
			{ "Purge", 10f },
			{ "SabotageFacilities", 0f },
			{ "Unrest", 0f },
			{ "TerrorizeRegion", 0f },
			{ "BuildFacility", 3f },
			{ "Abductions", 2f },
			{ "Xenoform", 2f }
		};

		// Token: 0x040021F5 RID: 8693
		private static readonly Dictionary<PriorityType, int> prioritySettings = new Dictionary<PriorityType, int> { 
		{
			PriorityType.Unity,
			1
		} };

		// Token: 0x040021F6 RID: 8694
		private static readonly List<GoalType> incompatibleGoalsForTarget = new List<GoalType>
		{
			GoalType.CaptureNationDirty,
			GoalType.NeutralizeNation,
			GoalType.SupportNation
		};
	}
}
