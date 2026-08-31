using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D7 RID: 1751
	public class ShipPowerSystemsChargeChange : GameEvent
	{
		// Token: 0x06002903 RID: 10499 RVA: 0x000DAEEA File Offset: 0x000D90EA
		public ShipPowerSystemsChargeChange(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F5E RID: 8030
		public TISpaceShipState ship;
	}
}
