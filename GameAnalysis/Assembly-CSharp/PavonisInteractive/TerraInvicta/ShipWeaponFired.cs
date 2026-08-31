using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C5 RID: 1733
	public class ShipWeaponFired : GameEvent
	{
		// Token: 0x060028F1 RID: 10481 RVA: 0x000DAD64 File Offset: 0x000D8F64
		public ShipWeaponFired(TISpaceShipState ship, ModuleDataEntry weaponData)
		{
			this.ship = ship;
			this.weaponData = weaponData;
		}

		// Token: 0x04001F3B RID: 7995
		public TISpaceShipState ship;

		// Token: 0x04001F3C RID: 7996
		public ModuleDataEntry weaponData;
	}
}
