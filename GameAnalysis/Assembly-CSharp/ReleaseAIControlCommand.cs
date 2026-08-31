using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Ship;

// Token: 0x020003B0 RID: 944
public class ReleaseAIControlCommand : TIShipCommandTemplate
{
	// Token: 0x0600117D RID: 4477 RVA: 0x00056624 File Offset: 0x00054824
	public override int IconPosition()
	{
		return 11;
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x00056628 File Offset: 0x00054828
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return ship.combatAIControl && !ship.faction.player.isAI;
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x00056647 File Offset: 0x00054847
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return ship.combatAIControl && !ship.faction.player.isAI;
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x00056668 File Offset: 0x00054868
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetAIControlAction(ship, false));
		foreach (IWeapon weapon in GameControl.spaceCombat.combatantLookup[ship].ref_shipController.hull.IterateByClass<IWeapon>())
		{
			foreach (IFireMode fireMode in weapon.fireModes)
			{
				if (fireMode.mode == FireMode.Guardian)
				{
					(fireMode as GuardianFireMode).SetAIManagement(false);
				}
			}
		}
		base.OnExecuteCommand(ship);
	}
}
