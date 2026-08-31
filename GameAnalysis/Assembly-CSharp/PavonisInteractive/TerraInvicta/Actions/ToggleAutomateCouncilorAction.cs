using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA0 RID: 2720
	public class ToggleAutomateCouncilorAction : PlayerAction
	{
		// Token: 0x06006597 RID: 26007 RVA: 0x002FD63F File Offset: 0x002FB83F
		public ToggleAutomateCouncilorAction(TICouncilorState councilor, bool setting)
		{
			this.councilorID = councilor.ID;
			this.setting = setting;
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x002FD65A File Offset: 0x002FB85A
		public override void Execute()
		{
			this.councilorID.GetState<TICouncilorState>(false).SetPermanentDefenseMode(this.setting);
		}

		// Token: 0x040047E7 RID: 18407
		private GameStateID councilorID;

		// Token: 0x040047E8 RID: 18408
		private bool setting;
	}
}
