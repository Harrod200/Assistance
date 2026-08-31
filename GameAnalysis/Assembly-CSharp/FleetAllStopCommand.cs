using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200039A RID: 922
public class FleetAllStopCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010D3 RID: 4307 RVA: 0x000557E9 File Offset: 0x000539E9
	public override int IconPosition()
	{
		return 16;
	}

	// Token: 0x060010D4 RID: 4308 RVA: 0x000557ED File Offset: 0x000539ED
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.AllStop;
	}

	// Token: 0x060010D5 RID: 4309 RVA: 0x000557F1 File Offset: 0x000539F1
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.CanRotateAndRoll() && !x.activeCombatManeuvers.Contains(this.Maneuver()));
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x00055810 File Offset: 0x00053A10
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(AllStopCommand)).GetTemplate();
	}
}
