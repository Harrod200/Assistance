using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006CF RID: 1743
	public class ShipSystemNoLongerBeingRepaired : GameEvent
	{
		// Token: 0x060028FB RID: 10491 RVA: 0x000DAE40 File Offset: 0x000D9040
		public ShipSystemNoLongerBeingRepaired(TISpaceShipState ship, ShipSystem system)
		{
			this.ship = ship;
			this.system = system;
		}

		// Token: 0x04001F4F RID: 8015
		public TISpaceShipState ship;

		// Token: 0x04001F50 RID: 8016
		public ShipSystem system;
	}
}
