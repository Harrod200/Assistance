using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B7 RID: 951
public class FortifyCommand : TIShipCommandTemplate
{
	// Token: 0x060011A1 RID: 4513 RVA: 0x00056ED8 File Offset: 0x000550D8
	public override int IconPosition()
	{
		return 7;
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x00056EDB File Offset: 0x000550DB
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return base.CommandVisibleToActor(ship) && !ship.disengageFromCombat && ship.allWeaponTemplates.Count > 0;
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x00056F00 File Offset: 0x00055100
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Defense))
			{
				if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Salvo))
				{
					ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Idle));
				}
				else
				{
					ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Defense));
				}
			}
			else
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Idle));
			}
		}
		base.OnExecuteCommand(ship);
	}
}
