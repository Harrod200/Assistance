using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000609 RID: 1545
	public class HabSymbolAssigned : GameEvent
	{
		// Token: 0x0600282E RID: 10286 RVA: 0x000D9F38 File Offset: 0x000D8138
		public HabSymbolAssigned(TIHabState hab)
		{
			this.hab = hab;
		}

		// Token: 0x04001E39 RID: 7737
		public TIHabState hab;
	}
}
