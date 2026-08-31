using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D6 RID: 1750
	public class ShipHeatChange : GameEvent
	{
		// Token: 0x06002902 RID: 10498 RVA: 0x000DAEDB File Offset: 0x000D90DB
		public ShipHeatChange(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F5D RID: 8029
		public TISpaceShipState ship;
	}
}
