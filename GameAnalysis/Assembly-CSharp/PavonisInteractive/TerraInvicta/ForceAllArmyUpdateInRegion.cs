using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000622 RID: 1570
	public class ForceAllArmyUpdateInRegion : GameEvent
	{
		// Token: 0x06002847 RID: 10311 RVA: 0x000DA10C File Offset: 0x000D830C
		public ForceAllArmyUpdateInRegion(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E5A RID: 7770
		public TIRegionState region;
	}
}
