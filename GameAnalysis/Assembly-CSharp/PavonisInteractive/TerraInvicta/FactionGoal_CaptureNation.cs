using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200072B RID: 1835
	public abstract class FactionGoal_CaptureNation : FactionGoal_Nation
	{
		// Token: 0x06002D86 RID: 11654 RVA: 0x000FA1E7 File Offset: 0x000F83E7
		public override bool NationPrioritiesGoal()
		{
			return true;
		}

		// Token: 0x06002D87 RID: 11655 RVA: 0x000FA1EA File Offset: 0x000F83EA
		public override bool NationMissionModifyingGoal()
		{
			return true;
		}

		// Token: 0x06002D88 RID: 11656 RVA: 0x000FA1ED File Offset: 0x000F83ED
		public override TIGameState actor()
		{
			if (!(base.nation.executiveFaction == this.faction))
			{
				return this.faction.ref_gameState;
			}
			return base.nation.ref_gameState;
		}

		// Token: 0x06002D89 RID: 11657 RVA: 0x000FA21E File Offset: 0x000F841E
		public override TIGameState target()
		{
			return base.nation;
		}

		// Token: 0x06002D8A RID: 11658 RVA: 0x000FA226 File Offset: 0x000F8426
		public override TIGameState location()
		{
			return base.nation.capital;
		}

		// Token: 0x06002D8B RID: 11659 RVA: 0x000FA233 File Offset: 0x000F8433
		public override TIGameState goalProduct()
		{
			return base.nation;
		}

		// Token: 0x06002D8C RID: 11660 RVA: 0x000FA23B File Offset: 0x000F843B
		public override bool ValidNewGoal()
		{
			return !this.ShouldDiscardGoal() && !this.GoalFulfilled();
		}

		// Token: 0x06002D8D RID: 11661 RVA: 0x000FA250 File Offset: 0x000F8450
		public override bool InProgress()
		{
			return base.nation.CountFactionControlPoints(this.faction, true, true, true) > 0;
		}

		// Token: 0x06002D8E RID: 11662 RVA: 0x000FA269 File Offset: 0x000F8469
		public override bool ShouldDiscardGoal()
		{
			return base.importance <= 0 || base.nation == null || !base.nation.extant;
		}

		// Token: 0x06002D8F RID: 11663 RVA: 0x000FA292 File Offset: 0x000F8492
		public override bool GoalFulfilled()
		{
			return base.nation.CouncilControlPointFraction(this.faction, true, true) >= 1f;
		}

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06002D90 RID: 11664 RVA: 0x000FA2B1 File Offset: 0x000F84B1
		public override List<Type> armyOperations
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06002D91 RID: 11665 RVA: 0x000FA2B4 File Offset: 0x000F84B4
		public override List<PolicyType> policiesAsNation
		{
			get
			{
				return PolicyManager.RegularPolicyNames_SetPolicy;
			}
		}

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06002D92 RID: 11666 RVA: 0x000FA2BB File Offset: 0x000F84BB
		public override List<PolicyType> factionLevelPoliciesAsNation
		{
			get
			{
				return PolicyManager.AllPolicyNames_Faction;
			}
		}

		// Token: 0x06002D93 RID: 11667 RVA: 0x000FA2C4 File Offset: 0x000F84C4
		public override void OnGoalComplete()
		{
			base.OnGoalComplete();
			int num = 20 - this.faction.AllCaptureNationGoals(true).Count<TIFactionGoalState>();
			if (num > 0)
			{
				foreach (TINationState tinationState in from x in base.nation.AdjacentNations(false)
					orderby x.numControlPoints_unclamped descending
					select x)
				{
					if (tinationState.executiveFaction == null && this.faction.GoalsWithTarget(tinationState, TIFactionGoalState.CaptureNationGoals, true).Count == 0)
					{
						this.faction.AddGoal(new FactionGoal_CaptureNation_Clean(this.faction, tinationState.numControlPoints_unclamped * 2 - 1, tinationState, this.faction.AI_GetPreferredManagementGoalForNation(tinationState), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						num--;
						if (num <= 0)
						{
							break;
						}
					}
				}
			}
		}
	}
}
