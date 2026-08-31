using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000617 RID: 1559
	public class RegionOccupationValueChange : GameEvent
	{
		// Token: 0x0600283C RID: 10300 RVA: 0x000DA03D File Offset: 0x000D823D
		public RegionOccupationValueChange(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001E49 RID: 7753
		public TIRegionState region;
	}
}
