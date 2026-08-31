using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200060D RID: 1549
	public class RegionDamaged : GameEvent
	{
		// Token: 0x06002832 RID: 10290 RVA: 0x000D9FA0 File Offset: 0x000D81A0
		public RegionDamaged(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E3E RID: 7742
		public TIRegionState region;
	}
}
