using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A9F RID: 2719
	public class SyncPrioritiesAction : PlayerAction
	{
		// Token: 0x06006595 RID: 26005 RVA: 0x002FD5FF File Offset: 0x002FB7FF
		public SyncPrioritiesAction(TIControlPoint sourceControlPoint)
		{
			this.controlPointID = sourceControlPoint.ID;
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x002FD614 File Offset: 0x002FB814
		public override void Execute()
		{
			TIControlPoint state = this.controlPointID.GetState<TIControlPoint>(false);
			state.nation.SyncAllPriorites(state.positionInNation);
		}

		// Token: 0x040047E6 RID: 18406
		private GameStateID controlPointID;
	}
}
