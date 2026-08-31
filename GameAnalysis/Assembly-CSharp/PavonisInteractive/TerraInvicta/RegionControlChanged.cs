using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000604 RID: 1540
	public class RegionControlChanged : GameEvent
	{
		// Token: 0x06002829 RID: 10281 RVA: 0x000D9EDF File Offset: 0x000D80DF
		public RegionControlChanged(TIRegionState region, TINationState oldNation, TINationState newNation)
		{
			this.region = region;
			this.oldNation = oldNation;
			this.newNation = newNation;
		}

		// Token: 0x04001E32 RID: 7730
		public TIRegionState region;

		// Token: 0x04001E33 RID: 7731
		public TINationState oldNation;

		// Token: 0x04001E34 RID: 7732
		public TINationState newNation;
	}
}
