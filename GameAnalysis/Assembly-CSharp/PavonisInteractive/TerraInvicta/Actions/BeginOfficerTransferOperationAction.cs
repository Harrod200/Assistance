using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A51 RID: 2641
	internal class BeginOfficerTransferOperationAction : PlayerAction
	{
		// Token: 0x060064EF RID: 25839 RVA: 0x002FA89F File Offset: 0x002F8A9F
		public BeginOfficerTransferOperationAction(TISpaceFleetState fleet, Dictionary<TIOfficerState, OfficerCarrierState> plan)
		{
			this.fleetID = fleet.ID;
			this.plan = plan;
		}

		// Token: 0x060064F0 RID: 25840 RVA: 0x002FA8BC File Offset: 0x002F8ABC
		public override void Execute()
		{
			TISpaceFleetState state = this.fleetID.GetState<TISpaceFleetState>(false);
			state.SetOfficerTransferPlan(this.plan);
			state.faction.playerControl.StartAction(new ConfirmOperationAction(state, state, new TransferOfficersOperation(), TransferOfficersOperation.ResourceCostOptions(this.plan).FirstOrDefault<TIResourcesCost>(), null));
		}

		// Token: 0x0400470C RID: 18188
		private GameStateID fleetID;

		// Token: 0x0400470D RID: 18189
		private Dictionary<TIOfficerState, OfficerCarrierState> plan;
	}
}
