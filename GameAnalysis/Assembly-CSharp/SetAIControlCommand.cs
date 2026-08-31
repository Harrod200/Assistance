using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003AF RID: 943
public class SetAIControlCommand : TIShipCommandTemplate
{
	// Token: 0x06001178 RID: 4472 RVA: 0x00056539 File Offset: 0x00054739
	public override int IconPosition()
	{
		return 11;
	}

	// Token: 0x06001179 RID: 4473 RVA: 0x0005653D File Offset: 0x0005473D
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return !ship.combatAIControl;
	}

	// Token: 0x0600117A RID: 4474 RVA: 0x00056548 File Offset: 0x00054748
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return !ship.combatAIControl;
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x00056554 File Offset: 0x00054754
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetAIControlAction(ship, true));
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			foreach (IFireMode fireMode in weapon.fireModes)
			{
				if (fireMode.mode == FireMode.Guardian)
				{
					(fireMode as GuardianFireMode).SetAIManagement(true);
				}
			}
		}
		base.OnExecuteCommand(ship);
	}
}
