using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200039C RID: 924
public class FleetPadlockPrimaryTargetCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010E2 RID: 4322 RVA: 0x00055921 File Offset: 0x00053B21
	public override int IconPosition()
	{
		return 18;
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00055925 File Offset: 0x00053B25
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.Padlock;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x00055928 File Offset: 0x00053B28
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.activeCombatManeuvers.Contains(this.Maneuver()));
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x0005593C File Offset: 0x00053B3C
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.CanRotateAndRoll() && !x.activeCombatManeuvers.Contains(this.Maneuver()));
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x0005595B File Offset: 0x00053B5B
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(PadlockPrimaryTargetCommand)).GetTemplate();
	}
}
