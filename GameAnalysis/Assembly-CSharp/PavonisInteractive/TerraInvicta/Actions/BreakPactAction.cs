using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A52 RID: 2642
	public class BreakPactAction : PlayerAction
	{
		// Token: 0x060064F1 RID: 25841 RVA: 0x002FA910 File Offset: 0x002F8B10
		public BreakPactAction(TIFactionState actingFaction, TIFactionState otherFaction, List<TradeOffer.TreatyType> pactsToBreak)
		{
			this.actingFactionID = actingFaction.ID;
			this.otherFactionID = otherFaction.ID;
			this.pactsToBreak = pactsToBreak;
			if (pactsToBreak.Contains(TradeOffer.TreatyType.NAP) && actingFaction.intelSharingFactions.Contains(otherFaction))
			{
				pactsToBreak.AddUnique(TradeOffer.TreatyType.Intel);
			}
		}

		// Token: 0x060064F2 RID: 25842 RVA: 0x002FA964 File Offset: 0x002F8B64
		public override void Execute()
		{
			TIFactionState state = this.actingFactionID.GetState<TIFactionState>(false);
			TIFactionState state2 = this.otherFactionID.GetState<TIFactionState>(false);
			bool flag = false;
			using (List<TradeOffer.TreatyType>.Enumerator enumerator = this.pactsToBreak.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case TradeOffer.TreatyType.Truce:
					{
						List<TIFactionGoalState> list = state.FindGoals(GoalType.TruceWithFaction, state, state2, TIFactionState.GoalFilter.none, true);
						list.ForEach(delegate(TIFactionGoalState x)
						{
							x.SetImportance(0);
						});
						List<TIFactionGoalState> list2 = state2.FindGoals(GoalType.TruceWithFaction, state2, state, TIFactionState.GoalFilter.none, true);
						list2.ForEach(delegate(TIFactionGoalState x)
						{
							x.SetImportance(0);
						});
						flag |= list.Any<TIFactionGoalState>() || list2.Any<TIFactionGoalState>();
						break;
					}
					case TradeOffer.TreatyType.NAP:
					{
						List<TIFactionGoalState> list3 = state.FindGoals(GoalType.NonAggressionPact, state, state2, TIFactionState.GoalFilter.none, true);
						list3.ForEach(delegate(TIFactionGoalState x)
						{
							x.SetImportance(0);
						});
						List<TIFactionGoalState> list4 = state2.FindGoals(GoalType.NonAggressionPact, state2, state, TIFactionState.GoalFilter.none, true);
						list4.ForEach(delegate(TIFactionGoalState x)
						{
							x.SetImportance(0);
						});
						flag |= list3.Any<TIFactionGoalState>() || list4.Any<TIFactionGoalState>();
						break;
					}
					case TradeOffer.TreatyType.Intel:
						flag |= state.intelSharingFactions.Contains(state2) || state2.intelSharingFactions.Contains(state);
						state.EndIntelSharingWith(state2);
						state2.EndIntelSharingWith(state);
						break;
					}
				}
			}
			if (flag)
			{
				TINotificationQueueState.LogPactEnds(state, state2, this.pactsToBreak);
			}
		}

		// Token: 0x0400470E RID: 18190
		private GameStateID actingFactionID;

		// Token: 0x0400470F RID: 18191
		private GameStateID otherFactionID;

		// Token: 0x04004710 RID: 18192
		private readonly List<TradeOffer.TreatyType> pactsToBreak;
	}
}
