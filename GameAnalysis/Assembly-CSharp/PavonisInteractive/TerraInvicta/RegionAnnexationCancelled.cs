using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200061A RID: 1562
	public class RegionAnnexationCancelled : GameEvent
	{
		// Token: 0x0600283F RID: 10303 RVA: 0x000DA071 File Offset: 0x000D8271
		public RegionAnnexationCancelled(TIRegionState region, TIArmyState annexingArmy)
		{
			this.region = region;
			this.army = annexingArmy;
		}

		// Token: 0x04001E4D RID: 7757
		public TIRegionState region;

		// Token: 0x04001E4E RID: 7758
		public TIArmyState army;
	}
}
