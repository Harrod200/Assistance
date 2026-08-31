using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B6 RID: 950
public class DefensiveCommand : TIShipCommandTemplate
{
	// Token: 0x0600119D RID: 4509 RVA: 0x00056D90 File Offset: 0x00054F90
	public override int IconPosition()
	{
		return 6;
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x00056D93 File Offset: 0x00054F93
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		if (base.CommandVisibleToActor(ship) && !ship.disengageFromCombat)
		{
			return ship.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.defenseMode);
		}
		return false;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x00056DD4 File Offset: 0x00054FD4
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Defense))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Defense));
			}
			else if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Offense))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Offense));
			}
		}
	}
}
