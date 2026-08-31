using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000635 RID: 1589
	public class FleetSymbolVisibilityChange : GameEvent
	{
		// Token: 0x0600285A RID: 10330 RVA: 0x000DA26F File Offset: 0x000D846F
		public FleetSymbolVisibilityChange(TISpaceFleetState fleet, bool active)
		{
			this.fleet = fleet;
			this.active = active;
		}

		// Token: 0x04001E77 RID: 7799
		public TISpaceFleetState fleet;

		// Token: 0x04001E78 RID: 7800
		public bool active;
	}
}
