using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000618 RID: 1560
	public class OccupationStatusChange : GameEvent
	{
		// Token: 0x0600283D RID: 10301 RVA: 0x000DA04C File Offset: 0x000D824C
		public OccupationStatusChange(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E4A RID: 7754
		public TIRegionState region;
	}
}
