using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A90 RID: 2704
	public class SetCouncilorPermanentAssignment : PlayerAction
	{
		// Token: 0x06006575 RID: 25973 RVA: 0x002FCFEC File Offset: 0x002FB1EC
		public SetCouncilorPermanentAssignment(TICouncilorState councilor, bool setting)
		{
			this.councilorID = councilor.ID;
			this.setting = setting;
		}

		// Token: 0x06006576 RID: 25974 RVA: 0x002FD007 File Offset: 0x002FB207
		public override void Execute()
		{
			this.councilorID.GetState<TICouncilorState>(false).SetPermanentAssignment(this.setting);
		}

		// Token: 0x040047B8 RID: 18360
		private GameStateID councilorID;

		// Token: 0x040047B9 RID: 18361
		private bool setting;
	}
}
