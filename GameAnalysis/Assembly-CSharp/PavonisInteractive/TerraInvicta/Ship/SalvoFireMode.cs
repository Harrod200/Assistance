using System;
using PavonisInteractive.TerraInvicta.Actions;

namespace PavonisInteractive.TerraInvicta.Ship
{
	// Token: 0x0200097F RID: 2431
	public class SalvoFireMode : FocusFireMode
	{
		// Token: 0x17000FD8 RID: 4056
		// (get) Token: 0x06005C72 RID: 23666 RVA: 0x002C1114 File Offset: 0x002BF314
		public override string displayName
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Salvo");
			}
		}

		// Token: 0x17000FD9 RID: 4057
		// (get) Token: 0x06005C73 RID: 23667 RVA: 0x002C1120 File Offset: 0x002BF320
		public override string description
		{
			get
			{
				return Loc.T("UI.SpaceCombat.Salvo.description");
			}
		}

		// Token: 0x17000FDA RID: 4058
		// (get) Token: 0x06005C74 RID: 23668 RVA: 0x002C112C File Offset: 0x002BF32C
		public override string iconPath
		{
			get
			{
				return "ui_spacecombat/BUT_mode_salvo_fire";
			}
		}

		// Token: 0x17000FDB RID: 4059
		// (get) Token: 0x06005C75 RID: 23669 RVA: 0x002C1133 File Offset: 0x002BF333
		public override FireMode mode
		{
			get
			{
				return FireMode.Salvo;
			}
		}

		// Token: 0x06005C76 RID: 23670 RVA: 0x002C1138 File Offset: 0x002BF338
		public SalvoFireMode(IWeapon weapon)
			: base(weapon)
		{
			this._totalSalvo = this.weaponTemplate.ref_projectileWeapon.FullAmmoCount_Max(this.ship.ShipState.template) / 4;
			GameControl.eventManager.AddListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnWeaponFired), null, this.ship.ShipState, false, false);
			GameControl.eventManager.AddListener<ShipWeaponModeChanged>(new EventManager.EventDelegate<ShipWeaponModeChanged>(this.OnWeaponModeChanged), null, this.ship.ShipState, false, false);
		}

		// Token: 0x06005C77 RID: 23671 RVA: 0x002C11BC File Offset: 0x002BF3BC
		~SalvoFireMode()
		{
			GameControl.eventManager.RemoveListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnWeaponFired), null);
			GameControl.eventManager.RemoveListener<ShipWeaponModeChanged>(new EventManager.EventDelegate<ShipWeaponModeChanged>(this.OnWeaponModeChanged), null);
		}

		// Token: 0x06005C78 RID: 23672 RVA: 0x002C1210 File Offset: 0x002BF410
		private void OnWeaponModeChanged(ShipWeaponModeChanged e)
		{
			if (e.weaponData == this.weaponAsset.weaponData && base.weapon.currentFireMode.mode == this.mode)
			{
				this._shotsFired = 0;
			}
		}

		// Token: 0x06005C79 RID: 23673 RVA: 0x002C1244 File Offset: 0x002BF444
		private void OnWeaponFired(ShipWeaponFired e)
		{
			if (e.weaponData == this.weaponAsset.weaponData && base.weapon.currentFireMode.mode == this.mode)
			{
				this._shotsFired++;
				if (this._shotsFired == this._totalSalvo)
				{
					this.ship.faction.playerControl.StartAction(new SetWeaponModeAction(this.ship.ShipState, this.weaponAsset, FireMode.Idle));
					this._shotsFired = 0;
				}
			}
		}

		// Token: 0x040041E9 RID: 16873
		private int _totalSalvo;

		// Token: 0x040041EA RID: 16874
		private int _shotsFired;
	}
}
