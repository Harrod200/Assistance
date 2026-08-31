using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200039B RID: 923
public class FleetCancelAllStopCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010D9 RID: 4313 RVA: 0x00055875 File Offset: 0x00053A75
	public override int IconPosition()
	{
		return 16;
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x00055879 File Offset: 0x00053A79
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelAllStop;
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x0005587D File Offset: 0x00053A7D
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.AllStop;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x00055881 File Offset: 0x00053A81
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x00055895 File Offset: 0x00053A95
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x000558B4 File Offset: 0x00053AB4
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(CancelAllStopCommand)).GetTemplate();
	}
}
