using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200039D RID: 925
public class FleetCancelPadlockPrimaryTargetCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010EA RID: 4330 RVA: 0x000559D6 File Offset: 0x00053BD6
	public override int IconPosition()
	{
		return 18;
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x000559DA File Offset: 0x00053BDA
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelPadlock;
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x000559DD File Offset: 0x00053BDD
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.Padlock;
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x000559E0 File Offset: 0x00053BE0
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x000559F4 File Offset: 0x00053BF4
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x00055A13 File Offset: 0x00053C13
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(CancelPadlockPrimaryTargetCommand)).GetTemplate();
	}
}
