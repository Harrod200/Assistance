using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006BB RID: 1723
	public class ShipManuverTargetDestroyed : GameEvent
	{
		// Token: 0x060028E7 RID: 10471 RVA: 0x000DAC9C File Offset: 0x000D8E9C
		public ShipManuverTargetDestroyed(TISpaceShipState ship)
		{
			this.ship = ship;
		}

		// Token: 0x04001F2A RID: 7978
		public TISpaceShipState ship;
	}
}
