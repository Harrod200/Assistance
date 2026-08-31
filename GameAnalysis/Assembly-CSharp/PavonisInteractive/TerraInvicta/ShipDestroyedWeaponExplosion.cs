using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006D1 RID: 1745
	public class ShipDestroyedWeaponExplosion : GameEvent
	{
		// Token: 0x060028FD RID: 10493 RVA: 0x000DAE6C File Offset: 0x000D906C
		public ShipDestroyedWeaponExplosion(TISpaceShipState ship, ModuleDataEntry partExploding)
		{
			this.ship = ship;
			this.partExploding = partExploding;
		}

		// Token: 0x04001F53 RID: 8019
		public TISpaceShipState ship;

		// Token: 0x04001F54 RID: 8020
		public ModuleDataEntry partExploding;
	}
}
