using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D5 RID: 1749
	public class ShipDeltaVChange : GameEvent
	{
		// Token: 0x06002901 RID: 10497 RVA: 0x000DAECC File Offset: 0x000D90CC
		public ShipDeltaVChange(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F5C RID: 8028
		public TISpaceShipState ship;
	}
}
