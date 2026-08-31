using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C4 RID: 1732
	public class ShipWeaponModeChanged : GameEvent
	{
		// Token: 0x060028F0 RID: 10480 RVA: 0x000DAD4E File Offset: 0x000D8F4E
		public ShipWeaponModeChanged(TISpaceShipState ship, ModuleDataEntry weaponData)
		{
			this.ship = ship;
			this.weaponData = weaponData;
		}

		// Token: 0x04001F39 RID: 7993
		public TISpaceShipState ship;

		// Token: 0x04001F3A RID: 7994
		public ModuleDataEntry weaponData;
	}
}
