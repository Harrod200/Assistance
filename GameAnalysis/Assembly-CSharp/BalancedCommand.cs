using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B5 RID: 949
public class BalancedCommand : TIShipCommandTemplate
{
	// Token: 0x06001199 RID: 4505 RVA: 0x00056BF4 File Offset: 0x00054DF4
	public override int IconPosition()
	{
		return 5;
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x00056BF7 File Offset: 0x00054DF7
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		if (base.CommandVisibleToActor(ship) && !ship.disengageFromCombat)
		{
			return ship.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode);
		}
		return false;
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x00056C38 File Offset: 0x00054E38
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Guardian))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Guardian));
			}
			else if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Offense))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Offense));
			}
			else if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Defense))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Defense));
			}
		}
		base.OnExecuteCommand(ship);
	}
}
