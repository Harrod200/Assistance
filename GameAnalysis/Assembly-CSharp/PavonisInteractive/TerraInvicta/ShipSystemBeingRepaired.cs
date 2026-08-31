using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006CE RID: 1742
	public class ShipSystemBeingRepaired : GameEvent
	{
		// Token: 0x060028FA RID: 10490 RVA: 0x000DAE2A File Offset: 0x000D902A
		public ShipSystemBeingRepaired(TISpaceShipState ship, ShipSystem system)
		{
			this.ship = ship;
			this.system = system;
		}

		// Token: 0x04001F4D RID: 8013
		public TISpaceShipState ship;

		// Token: 0x04001F4E RID: 8014
		public ShipSystem system;
	}
}
