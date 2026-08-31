using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B3 RID: 947
public class FocusFireCommand : TIShipCommandTemplate
{
	// Token: 0x06001190 RID: 4496 RVA: 0x00056975 File Offset: 0x00054B75
	public override int IconPosition()
	{
		return 3;
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x00056978 File Offset: 0x00054B78
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		if (base.CommandVisibleToActor(ship) && !ship.disengageFromCombat)
		{
			return ship.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode);
		}
		return false;
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x000569B7 File Offset: 0x00054BB7
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && !ship.disengageFromCombat;
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x000569D0 File Offset: 0x00054BD0
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Focus))
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Focus));
			}
			else
			{
				ship.faction.playerControl.StartAction(new SetWeaponModeAction(ship, weapon as Weapon, FireMode.Defense));
			}
		}
		base.OnExecuteCommand(ship);
	}
}
