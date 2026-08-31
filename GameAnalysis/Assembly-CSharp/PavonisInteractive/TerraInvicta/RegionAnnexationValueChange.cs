using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000619 RID: 1561
	public class RegionAnnexationValueChange : GameEvent
	{
		// Token: 0x0600283E RID: 10302 RVA: 0x000DA05B File Offset: 0x000D825B
		public RegionAnnexationValueChange(TIRegionState region, TIArmyState annexingArmy)
		{
			this.region = region;
			this.army = annexingArmy;
		}

		// Token: 0x04001E4B RID: 7755
		public TIRegionState region;

		// Token: 0x04001E4C RID: 7756
		public TIArmyState army;
	}
}
