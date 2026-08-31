using System;
using System.Linq;
using PavonisInteractive.TerraInvicta.Ship;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A66 RID: 2662
	public class SetWeaponModeAction : PlayerAction
	{
		// Token: 0x06006519 RID: 25881 RVA: 0x002FB2B7 File Offset: 0x002F94B7
		public SetWeaponModeAction(TISpaceShipState ship, Weapon weapon, FireMode mode)
		{
			this.shipID = ship.ID;
			this.weapon = weapon;
			this.mode = mode;
		}

		// Token: 0x0600651A RID: 25882 RVA: 0x002FB2DC File Offset: 0x002F94DC
		public override void Execute()
		{
			TISpaceShipState state = this.shipID.GetState<TISpaceShipState>(false);
			IFireMode currentFireMode = this.weapon.currentFireMode;
			this.weapon.currentFireMode = this.weapon.fireModes.First<IFireMode>((IFireMode x) => x.mode == this.mode);
			if (currentFireMode != this.weapon.currentFireMode)
			{
				GameControl.eventManager.TriggerEvent(new ShipWeaponModeChanged(state, this.weapon.weaponData), null, new object[] { state });
			}
		}

		// Token: 0x0400473C RID: 18236
		private GameStateID shipID;

		// Token: 0x0400473D RID: 18237
		private readonly Weapon weapon;

		// Token: 0x0400473E RID: 18238
		private readonly FireMode mode;
	}
}
