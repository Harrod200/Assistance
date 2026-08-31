using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000665 RID: 1637
	public class HabSelectedEvent : GameEvent
	{
		// Token: 0x0600288C RID: 10380 RVA: 0x000DA639 File Offset: 0x000D8839
		public HabSelectedEvent(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x04001EC3 RID: 7875
		public TIHabState hab;
	}
}
