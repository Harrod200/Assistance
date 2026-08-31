using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA2 RID: 2722
	public class TransferOrgToCouncilorAction : PlayerAction
	{
		// Token: 0x0600659B RID: 26011 RVA: 0x002FD6E1 File Offset: 0x002FB8E1
		public TransferOrgToCouncilorAction(TIOrgState org, TIFactionState faction, TICouncilorState councilorReceiving, TICouncilorState councilorGiving)
		{
			this.councilorReceivingID = councilorReceiving.ID;
			this.councilorGivingID = councilorGiving.ID;
			this.orgID = org.ID;
			this.factionID = faction.ID;
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x002FD71C File Offset: 0x002FB91C
		public override void Execute()
		{
			TIOrgState state = this.orgID.GetState<TIOrgState>(false);
			this.factionID.GetState<TIFactionState>(false).TransferOrgToCouncilor(state, this.councilorReceivingID.GetState<TICouncilorState>(false), this.councilorGivingID.GetState<TICouncilorState>(false));
		}

		// Token: 0x040047EB RID: 18411
		private GameStateID factionID;

		// Token: 0x040047EC RID: 18412
		private GameStateID councilorReceivingID;

		// Token: 0x040047ED RID: 18413
		private GameStateID councilorGivingID;

		// Token: 0x040047EE RID: 18414
		private GameStateID orgID;
	}
}
