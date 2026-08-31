using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200060F RID: 1551
	public class RegionGDPLightsRecalculation : GameEvent
	{
		// Token: 0x06002834 RID: 10292 RVA: 0x000D9FBE File Offset: 0x000D81BE
		public RegionGDPLightsRecalculation(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E40 RID: 7744
		public TIRegionState region;
	}
}
