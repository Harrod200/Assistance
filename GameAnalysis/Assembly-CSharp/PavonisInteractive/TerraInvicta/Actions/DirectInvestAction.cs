using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A72 RID: 2674
	public class DirectInvestAction : PlayerAction
	{
		// Token: 0x06006532 RID: 25906 RVA: 0x002FB8D9 File Offset: 0x002F9AD9
		public DirectInvestAction(TIFactionState faction, TINationState nation, Dictionary<PriorityType, float> plannedDirectInvestments)
		{
			this.plannedDirectInvestments = plannedDirectInvestments;
			this.factionID = faction.ID;
			this.nationID = nation.ID;
		}

		// Token: 0x06006533 RID: 25907 RVA: 0x002FB900 File Offset: 0x002F9B00
		public DirectInvestAction(TIFactionState faction, TINationState nation, PriorityType priority, float IPs)
		{
			this.factionID = faction.ID;
			this.nationID = nation.ID;
			this.plannedDirectInvestments = new Dictionary<PriorityType, float>();
			this.plannedDirectInvestments.Add(priority, IPs);
		}

		// Token: 0x06006534 RID: 25908 RVA: 0x002FB93C File Offset: 0x002F9B3C
		public override void Execute()
		{
			TINationState state = this.nationID.GetState<TINationState>(false);
			TIFactionState state2 = this.factionID.GetState<TIFactionState>(false);
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (PriorityType priorityType in this.plannedDirectInvestments.Keys)
			{
				state.DirectInvestment(priorityType, this.plannedDirectInvestments[priorityType]);
				TIResourcesCost tiresourcesCost2 = state.InvestmentPointDirectPurchasePrice(priorityType, state2);
				tiresourcesCost2 = tiresourcesCost2.MultiplyCost(this.plannedDirectInvestments[priorityType]);
				tiresourcesCost.SumCosts_NoDuration(tiresourcesCost2);
			}
			tiresourcesCost.PayCost(state2, "Direct Invest");
			state.ProcessPrioritySpending();
			state.SetDataDirty();
		}

		// Token: 0x0400475F RID: 18271
		private Dictionary<PriorityType, float> plannedDirectInvestments;

		// Token: 0x04004760 RID: 18272
		private GameStateID factionID;

		// Token: 0x04004761 RID: 18273
		private GameStateID nationID;
	}
}
