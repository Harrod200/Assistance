using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A91 RID: 2705
	public class SetCouncilorRepeatMission : PlayerAction
	{
		// Token: 0x06006577 RID: 25975 RVA: 0x002FD020 File Offset: 0x002FB220
		public SetCouncilorRepeatMission(TICouncilorState councilor, bool repeatOrder)
		{
			this.councilorID = councilor.ID;
			this.repeatOrder = repeatOrder;
		}

		// Token: 0x06006578 RID: 25976 RVA: 0x002FD03B File Offset: 0x002FB23B
		public override void Execute()
		{
			this.councilorID.GetState<TICouncilorState>(false).SetRepeatOrder(this.repeatOrder);
		}

		// Token: 0x040047BA RID: 18362
		private GameStateID councilorID;

		// Token: 0x040047BB RID: 18363
		private bool repeatOrder;
	}
}
