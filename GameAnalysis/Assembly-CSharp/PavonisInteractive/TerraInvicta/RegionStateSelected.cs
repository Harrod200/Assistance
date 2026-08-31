using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000663 RID: 1635
	public class RegionStateSelected : GameEvent
	{
		// Token: 0x0600288A RID: 10378 RVA: 0x000DA61B File Offset: 0x000D881B
		public RegionStateSelected(TIRegionState region)
		{
			this.region = region;
		}

		// Token: 0x04001EC1 RID: 7873
		public TIRegionState region;
	}
}
