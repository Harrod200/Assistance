using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006B9 RID: 1721
	public class ShipPrimaryTargetDestroyed : GameEvent
	{
		// Token: 0x060028E5 RID: 10469 RVA: 0x000DAC77 File Offset: 0x000D8E77
		public ShipPrimaryTargetDestroyed(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F27 RID: 7975
		public TISpaceShipState ship;
	}
}
