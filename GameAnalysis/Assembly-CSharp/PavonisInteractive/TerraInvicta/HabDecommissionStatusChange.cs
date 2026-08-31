using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200064E RID: 1614
	public class HabDecommissionStatusChange : GameEvent
	{
		// Token: 0x06002875 RID: 10357 RVA: 0x000DA4C4 File Offset: 0x000D86C4
		public HabDecommissionStatusChange(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x04001EA8 RID: 7848
		public TIHabState hab;
	}
}
