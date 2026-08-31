using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA3 RID: 2723
	public class TransferOrgToFactionPoolAction : PlayerAction
	{
		// Token: 0x0600659D RID: 26013 RVA: 0x002FD760 File Offset: 0x002FB960
		public TransferOrgToFactionPoolAction(TIOrgState org, TICouncilorState councilor)
		{
			this.orgID = org.ID;
			this.councilorID = councilor.ID;
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x002FD780 File Offset: 0x002FB980
		public override void Execute()
		{
			TICouncilorState state = this.councilorID.GetState<TICouncilorState>(false);
			TIOrgState state2 = this.orgID.GetState<TIOrgState>(false);
			state.faction.AddOrgToFactionPool(state2, state, false);
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x002FD7B5 File Offset: 0x002FB9B5
		public TIOrgState GetOrg()
		{
			return this.orgID.GetState<TIOrgState>(false);
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x002FD7C3 File Offset: 0x002FB9C3
		public TICouncilorState GetCouncilorAssignment()
		{
			return this.councilorID.GetState<TICouncilorState>(false);
		}

		// Token: 0x040047EF RID: 18415
		private GameStateID councilorID;

		// Token: 0x040047F0 RID: 18416
		private GameStateID orgID;
	}
}
