using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A8D RID: 2701
	public class SellOrgAction : PlayerAction
	{
		// Token: 0x0600656E RID: 25966 RVA: 0x002FCE4C File Offset: 0x002FB04C
		public SellOrgAction(TIOrgState org, TIFactionState council, TICouncilorState councilor = null)
		{
			this.orgID = org.ID;
			this.councilID = council.ID;
			if (councilor != null)
			{
				this.councilorID = councilor.ID;
				return;
			}
			this.poolSell = true;
		}

		// Token: 0x0600656F RID: 25967 RVA: 0x002FCE8C File Offset: 0x002FB08C
		public override void Execute()
		{
			TIFactionState state = this.councilID.GetState<TIFactionState>(false);
			TIOrgState state2 = this.orgID.GetState<TIOrgState>(false);
			if (!this.poolSell)
			{
				state.SellOrg(state2, this.councilorID.GetState<TICouncilorState>(false));
				return;
			}
			state.SellOrg(state2, null);
		}

		// Token: 0x06006570 RID: 25968 RVA: 0x002FCED7 File Offset: 0x002FB0D7
		public TIOrgState GetOrg()
		{
			return this.orgID.GetState<TIOrgState>(false);
		}

		// Token: 0x040047B0 RID: 18352
		private GameStateID councilID;

		// Token: 0x040047B1 RID: 18353
		private GameStateID councilorID;

		// Token: 0x040047B2 RID: 18354
		private GameStateID orgID;

		// Token: 0x040047B3 RID: 18355
		private bool poolSell;
	}
}
