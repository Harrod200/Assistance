using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005EB RID: 1515
	public class RegionEntityUpdated : GameEvent
	{
		// Token: 0x06002810 RID: 10256 RVA: 0x000D9D1A File Offset: 0x000D7F1A
		public RegionEntityUpdated(TIRegionEntityState facility)
		{
			this.facility = facility;
		}

		// Token: 0x04001E0E RID: 7694
		public TIRegionEntityState facility;
	}
}
