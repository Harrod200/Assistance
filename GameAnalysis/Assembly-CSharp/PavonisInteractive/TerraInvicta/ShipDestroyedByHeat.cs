using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D4 RID: 1748
	public class ShipDestroyedByHeat : GameEvent
	{
		// Token: 0x06002900 RID: 10496 RVA: 0x000DAEBD File Offset: 0x000D90BD
		public ShipDestroyedByHeat(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F5B RID: 8027
		public TISpaceShipState ship;
	}
}
