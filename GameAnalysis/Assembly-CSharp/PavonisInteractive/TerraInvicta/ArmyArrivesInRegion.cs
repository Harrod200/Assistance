using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000621 RID: 1569
	public class ArmyArrivesInRegion : GameEvent
	{
		// Token: 0x06002846 RID: 10310 RVA: 0x000DA0F6 File Offset: 0x000D82F6
		public ArmyArrivesInRegion(TIArmyState army, TIRegionState region)
		{
			this.army = army;
			this.region = region;
		}

		// Token: 0x04001E58 RID: 7768
		public TIArmyState army;

		// Token: 0x04001E59 RID: 7769
		public TIRegionState region;
	}
}
