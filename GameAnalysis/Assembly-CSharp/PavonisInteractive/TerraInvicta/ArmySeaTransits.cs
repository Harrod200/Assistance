using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200061F RID: 1567
	public class ArmySeaTransits : GameEvent
	{
		// Token: 0x06002844 RID: 10308 RVA: 0x000DA0D1 File Offset: 0x000D82D1
		public ArmySeaTransits(TIArmyState army, TIRegionState destinationRegion)
		{
			this.army = army;
			this.destinationRegion = destinationRegion;
		}

		// Token: 0x04001E55 RID: 7765
		public TIArmyState army;

		// Token: 0x04001E56 RID: 7766
		public TIRegionState destinationRegion;
	}
}
