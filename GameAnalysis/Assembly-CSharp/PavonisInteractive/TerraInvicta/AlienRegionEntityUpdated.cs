using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000627 RID: 1575
	public class AlienRegionEntityUpdated : GameEvent
	{
		// Token: 0x0600284C RID: 10316 RVA: 0x000DA157 File Offset: 0x000D8357
		public AlienRegionEntityUpdated(TIRegionAlienEntityState alienEntityState, TIRegionState region)
		{
			this.alienEntityState = alienEntityState;
			this.region = region;
		}

		// Token: 0x04001E5F RID: 7775
		public TIRegionAlienEntityState alienEntityState;

		// Token: 0x04001E60 RID: 7776
		public TIRegionState region;
	}
}
