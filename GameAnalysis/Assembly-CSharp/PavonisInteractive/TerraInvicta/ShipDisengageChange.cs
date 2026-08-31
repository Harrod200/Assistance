using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D8 RID: 1752
	public class ShipDisengageChange : GameEvent
	{
		// Token: 0x06002904 RID: 10500 RVA: 0x000DAEF9 File Offset: 0x000D90F9
		public ShipDisengageChange(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F5F RID: 8031
		public TISpaceShipState ship;
	}
}
