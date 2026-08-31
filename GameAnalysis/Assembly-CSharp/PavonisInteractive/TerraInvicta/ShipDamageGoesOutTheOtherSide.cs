using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006BC RID: 1724
	public class ShipDamageGoesOutTheOtherSide : GameEvent
	{
		// Token: 0x060028E8 RID: 10472 RVA: 0x000DACAB File Offset: 0x000D8EAB
		public ShipDamageGoesOutTheOtherSide(TISpaceShipState ship, ArmorFacing facing)
		{
			this.ship = ship;
			this.facing = facing;
		}

		// Token: 0x04001F2B RID: 7979
		public TISpaceShipState ship;

		// Token: 0x04001F2C RID: 7980
		public ArmorFacing facing;
	}
}
