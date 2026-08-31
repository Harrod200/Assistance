using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A1 RID: 929
public class FleetCancelMatchVelocityCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x06001108 RID: 4360 RVA: 0x00055C6A File Offset: 0x00053E6A
	public override int IconPosition()
	{
		return 20;
	}

	// Token: 0x06001109 RID: 4361 RVA: 0x00055C6E File Offset: 0x00053E6E
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelMatchVelocity;
	}

	// Token: 0x0600110A RID: 4362 RVA: 0x00055C72 File Offset: 0x00053E72
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.MatchVelocity;
	}

	// Token: 0x0600110B RID: 4363 RVA: 0x00055C76 File Offset: 0x00053E76
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x0600110C RID: 4364 RVA: 0x00055C8A File Offset: 0x00053E8A
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x0600110D RID: 4365 RVA: 0x00055CA9 File Offset: 0x00053EA9
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(CancelMatchVelocityCommand)).GetTemplate();
	}
}
