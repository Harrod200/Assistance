using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005FA RID: 1530
	public class CouncilorDepartsRegion : GameEvent
	{
		// Token: 0x0600281F RID: 10271 RVA: 0x000D9E10 File Offset: 0x000D8010
		public CouncilorDepartsRegion(TICouncilorState councilor, TIRegionState region)
		{
			this.councilor = councilor;
			this.region = region;
		}

		// Token: 0x04001E20 RID: 7712
		public TICouncilorState councilor;

		// Token: 0x04001E21 RID: 7713
		public TIRegionState region;
	}
}
