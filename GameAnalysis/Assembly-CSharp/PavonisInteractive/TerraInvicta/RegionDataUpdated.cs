using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200060A RID: 1546
	public class RegionDataUpdated : GameEvent
	{
		// Token: 0x0600282F RID: 10287 RVA: 0x000D9F47 File Offset: 0x000D8147
		public RegionDataUpdated(TIRegionState region)
		{
			this.region = region;
			GameControl.eventManager.TriggerEvent(new NationDataUpdated(region.nation), null, new object[] { region.nation });
		}

		// Token: 0x04001E3A RID: 7738
		public TIRegionState region;
	}
}
