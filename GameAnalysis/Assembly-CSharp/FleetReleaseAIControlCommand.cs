using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200038C RID: 908
public class FleetReleaseAIControlCommand : TIFleetCommandTemplate
{
	// Token: 0x06001083 RID: 4227 RVA: 0x00055150 File Offset: 0x00053350
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return true;
	}

	// Token: 0x06001084 RID: 4228 RVA: 0x00055153 File Offset: 0x00053353
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.combatAIControl);
	}

	// Token: 0x06001085 RID: 4229 RVA: 0x0005517A File Offset: 0x0005337A
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return playerShips.Where<TISpaceShipState>((TISpaceShipState x) => x.combatAIControl).ToList<TISpaceShipState>();
	}

	// Token: 0x06001086 RID: 4230 RVA: 0x000551A6 File Offset: 0x000533A6
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(ReleaseAIControlCommand)).GetTemplate();
	}

	// Token: 0x06001087 RID: 4231 RVA: 0x000551D6 File Offset: 0x000533D6
	public override int IconPosition()
	{
		return 6;
	}

	// Token: 0x06001088 RID: 4232 RVA: 0x000551D9 File Offset: 0x000533D9
	public override void OnExecuteFleetCommand(List<TISpaceShipState> playerShips, CombatTargetableState target = null)
	{
		base.OnExecuteFleetCommand(playerShips, target);
		GameControl.eventManager.TriggerEvent(new FleetAIControlChanged(false, playerShips[0].fleet), null, Array.Empty<object>());
	}
}
