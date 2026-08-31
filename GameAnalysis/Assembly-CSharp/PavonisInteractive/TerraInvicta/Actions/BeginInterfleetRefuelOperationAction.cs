using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A50 RID: 2640
	internal class BeginInterfleetRefuelOperationAction : PlayerAction
	{
		// Token: 0x060064ED RID: 25837 RVA: 0x002FA827 File Offset: 0x002F8A27
		public BeginInterfleetRefuelOperationAction(TISpaceFleetState fleet, List<PropellantSharingEvent> plan)
		{
			this.fleetID = fleet.ID;
			this.plan = plan;
		}

		// Token: 0x060064EE RID: 25838 RVA: 0x002FA844 File Offset: 0x002F8A44
		public override void Execute()
		{
			TISpaceFleetState state = this.fleetID.GetState<TISpaceFleetState>(false);
			state.SetPropellantSharingPlan(this.plan);
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			tiresourcesCost.SetCompletionTime_Days(InterfleetRefuelOperation.GetRefuelDuration_days(this.plan));
			state.faction.playerControl.StartAction(new ConfirmOperationAction(state, state, new InterfleetRefuelOperation(), tiresourcesCost, null));
		}

		// Token: 0x0400470A RID: 18186
		private GameStateID fleetID;

		// Token: 0x0400470B RID: 18187
		private List<PropellantSharingEvent> plan;
	}
}
