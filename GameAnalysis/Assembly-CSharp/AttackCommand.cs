using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B4 RID: 948
public class AttackCommand : TIShipCommandTemplate
{
	// Token: 0x06001195 RID: 4501 RVA: 0x00056AA8 File Offset: 0x00054CA8
	public override int IconPosition()
	{
		return 4;
	}

	// Token: 0x06001196 RID: 4502 RVA: 0x00056AAB File Offset: 0x00054CAB
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		if (base.CommandVisibleToActor(ship) && !ship.disengageFromCombat)
		{
			return ship.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.attackMode);
		}
		return false;
	}

	// Token: 0x06001197 RID: 4503 RVA: 0x00056AEC File Offset: 0x00054CEC
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			if (weapon.fireModes.Any<IFireMode>((IFireMode x) => x.mode == FireMode.Offense))
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
