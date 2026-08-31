using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A71 RID: 2673
	public class DiplomacyTradeAction : PlayerAction
	{
		// Token: 0x06006530 RID: 25904 RVA: 0x002FB7F6 File Offset: 0x002F99F6
		public DiplomacyTradeAction(TIFactionState sendingFaction, TIFactionState receivingFaction, TradeOffer sendingTrade, TradeOffer receivingTrade, float hateModifier)
		{
			this.sendingFactionID = sendingFaction.ID;
			this.receivingFactionID = receivingFaction.ID;
			this.sendingTrade = sendingTrade;
			this.receivingTrade = receivingTrade;
			this.tradeHateModifier = hateModifier;
		}

		// Token: 0x06006531 RID: 25905 RVA: 0x002FB830 File Offset: 0x002F9A30
		public override void Execute()
		{
			TIFactionState state = this.sendingFactionID.GetState<TIFactionState>(false);
			TIFactionState state2 = this.receivingFactionID.GetState<TIFactionState>(false);
			state.ProcessTrade(this.receivingTrade, this.tradeHateModifier, state2, true);
			state2.ProcessTrade(this.sendingTrade, this.tradeHateModifier, state, false);
			if (state.UnassignedPoolOverage() > 0 && !TIPromptQueueState.HasPromptStatic(state, state, null, "PromptDropOrgs", 0))
			{
				TINotificationQueueState.LogOrgPoolOverfull(state);
				TIPromptQueueState.AddPromptStatic(state, state, null, "PromptDropOrgs", 0);
			}
			if (state2.UnassignedPoolOverage() > 0 && !TIPromptQueueState.HasPromptStatic(state2, state2, null, "PromptDropOrgs", 0))
			{
				TINotificationQueueState.LogOrgPoolOverfull(state2);
				TIPromptQueueState.AddPromptStatic(state2, state2, null, "PromptDropOrgs", 0);
			}
		}

		// Token: 0x0400475A RID: 18266
		private GameStateID sendingFactionID;

		// Token: 0x0400475B RID: 18267
		private GameStateID receivingFactionID;

		// Token: 0x0400475C RID: 18268
		private TradeOffer sendingTrade;

		// Token: 0x0400475D RID: 18269
		private TradeOffer receivingTrade;

		// Token: 0x0400475E RID: 18270
		private float tradeHateModifier;
	}
}
