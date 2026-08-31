using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000620 RID: 1568
	public class ArmySeaTransitCancelled : GameEvent
	{
		// Token: 0x06002845 RID: 10309 RVA: 0x000DA0E7 File Offset: 0x000D82E7
		public ArmySeaTransitCancelled(TIArmyState army)
		{
			this.army = army;
		}

		// Token: 0x04001E57 RID: 7767
		public TIArmyState army;
	}
}
