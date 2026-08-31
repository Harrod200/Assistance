using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A67 RID: 2663
	public class ConfirmOperationAction : PlayerAction
	{
		// Token: 0x0600651C RID: 25884 RVA: 0x002FB36B File Offset: 0x002F956B
		public ConfirmOperationAction(TIGameState actorState, TIGameState target, IOperation operation, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
		{
			this.actorID = actorState.ID;
			this.targetID = target.ID;
			this.operation = operation;
			this.resourcesCost = resourcesCost;
			this.trajectory = trajectory;
		}

		// Token: 0x0600651D RID: 25885 RVA: 0x002FB3A4 File Offset: 0x002F95A4
		public override void Execute()
		{
			TIGameState state = this.actorID.GetState<TIGameState>(true);
			TIGameState state2 = this.targetID.GetState<TIGameState>(true);
			IOperation operation = this.operation;
			if (operation == null)
			{
				return;
			}
			operation.OnOperationConfirm(state, state2, this.resourcesCost, this.trajectory);
		}

		// Token: 0x0400473F RID: 18239
		private GameStateID actorID;

		// Token: 0x04004740 RID: 18240
		private GameStateID targetID;

		// Token: 0x04004741 RID: 18241
		private IOperation operation;

		// Token: 0x04004742 RID: 18242
		private TIResourcesCost resourcesCost;

		// Token: 0x04004743 RID: 18243
		private Trajectory trajectory;
	}
}
