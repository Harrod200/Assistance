using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000605 RID: 1541
	public class RegionFlashEvent : GameEvent
	{
		// Token: 0x0600282A RID: 10282 RVA: 0x000D9EFC File Offset: 0x000D80FC
		public RegionFlashEvent(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E35 RID: 7733
		public TIRegionState region;
	}
}
