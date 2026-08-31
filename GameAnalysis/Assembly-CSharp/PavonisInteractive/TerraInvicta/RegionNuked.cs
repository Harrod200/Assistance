using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200060E RID: 1550
	public class RegionNuked : GameEvent
	{
		// Token: 0x06002833 RID: 10291 RVA: 0x000D9FAF File Offset: 0x000D81AF
		public RegionNuked(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E3F RID: 7743
		public TIRegionState region;
	}
}
