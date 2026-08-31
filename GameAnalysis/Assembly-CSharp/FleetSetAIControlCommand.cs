using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200038B RID: 907
public class FleetSetAIControlCommand : TIFleetCommandTemplate
{
	// Token: 0x0600107C RID: 4220 RVA: 0x00055088 File Offset: 0x00053288
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return true;
	}

	// Token: 0x0600107D RID: 4221 RVA: 0x0005508B File Offset: 0x0005328B
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl);
		}
		return false;
	}

	// Token: 0x0600107E RID: 4222 RVA: 0x000550BD File Offset: 0x000532BD
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return playerShips.Where<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl).ToList<TISpaceShipState>();
	}

	// Token: 0x0600107F RID: 4223 RVA: 0x000550E9 File Offset: 0x000532E9
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(SetAIControlCommand)).GetTemplate();
	}

	// Token: 0x06001080 RID: 4224 RVA: 0x00055119 File Offset: 0x00053319
	public override int IconPosition()
	{
		return 5;
	}

	// Token: 0x06001081 RID: 4225 RVA: 0x0005511C File Offset: 0x0005331C
	public override void OnExecuteFleetCommand(List<TISpaceShipState> playerShips, CombatTargetableState target = null)
	{
		base.OnExecuteFleetCommand(playerShips, target);
		GameControl.eventManager.TriggerEvent(new FleetAIControlChanged(true, playerShips[0].fleet), null, Array.Empty<object>());
	}
}
