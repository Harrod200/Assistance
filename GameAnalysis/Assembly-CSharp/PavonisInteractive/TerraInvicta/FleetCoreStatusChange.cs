using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000630 RID: 1584
	public class FleetCoreStatusChange : GameEvent
	{
		// Token: 0x06002855 RID: 10325 RVA: 0x000DA20F File Offset: 0x000D840F
		public FleetCoreStatusChange(TISpaceFleetState fleet)
		{
			this.fleet = fleet;
		}

		// Token: 0x04001E6F RID: 7791
		public TISpaceFleetState fleet;
	}
}
