using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200064C RID: 1612
	public class HabCreated : GameEvent
	{
		// Token: 0x06002871 RID: 10353 RVA: 0x000DA48C File Offset: 0x000D868C
		public HabCreated(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x04001EA6 RID: 7846
		public TIHabState hab;
	}
}
