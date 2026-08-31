using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006C6 RID: 1734
	public class ShipWeaponOutOfAmmo : GameEvent
	{
		// Token: 0x060028F2 RID: 10482 RVA: 0x000DAD7A File Offset: 0x000D8F7A
		public ShipWeaponOutOfAmmo(TISpaceShipState shipState, ModuleDataEntry weaponData)
		{
			this.shipState = shipState;
			this.weaponData = weaponData;
		}

		// Token: 0x04001F3D RID: 7997
		public TISpaceShipState shipState;

		// Token: 0x04001F3E RID: 7998
		public ModuleDataEntry weaponData;
	}
}
