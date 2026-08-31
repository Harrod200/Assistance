using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D4 RID: 1492
	public class HabDetailRequested : GameEvent
	{
		// Token: 0x060027F9 RID: 10233 RVA: 0x000D9BA4 File Offset: 0x000D7DA4
		public HabDetailRequested(TIHabState hab, bool straightToManagement)
		{
			this.hab = hab;
			this.manage = straightToManagement;
		}

		// Token: 0x04001DF3 RID: 7667
		public TIHabState hab;

		// Token: 0x04001DF4 RID: 7668
		public bool manage;
	}
}
