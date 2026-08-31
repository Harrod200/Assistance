using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A3 RID: 931
public class FleetCancelDefensiveManeuversCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x06001116 RID: 4374 RVA: 0x00055D88 File Offset: 0x00053F88
	public override int IconPosition()
	{
		return 13;
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x00055D8C File Offset: 0x00053F8C
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelDefensiveManeuvers;
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x00055D90 File Offset: 0x00053F90
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.DefensiveManuevers;
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x00055D94 File Offset: 0x00053F94
	public override bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips)
	{
		return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x00055DA8 File Offset: 0x00053FA8
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.activeCombatManeuvers.Contains(this.CancelManeuver()));
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x00055DC7 File Offset: 0x00053FC7
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(CancelDefensiveManeuversCommand)).GetTemplate();
	}
}
