using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000636 RID: 1590
	public class StationSymbolVisibilityChange : GameEvent
	{
		// Token: 0x0600285B RID: 10331 RVA: 0x000DA285 File Offset: 0x000D8485
		public StationSymbolVisibilityChange(TIHabState hab, bool active)
		{
			this.hab = hab;
			this.active = active;
		}

		// Token: 0x04001E79 RID: 7801
		public TIHabState hab;

		// Token: 0x04001E7A RID: 7802
		public bool active;
	}
}
