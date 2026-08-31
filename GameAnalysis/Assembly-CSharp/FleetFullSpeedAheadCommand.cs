using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200039E RID: 926
public class FleetFullSpeedAheadCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010F3 RID: 4339 RVA: 0x00055A71 File Offset: 0x00053C71
	public override int IconPosition()
	{
		return 12;
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x00055A75 File Offset: 0x00053C75
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.FullSpeedAhead;
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x00055A79 File Offset: 0x00053C79
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.ThrustEffectivenessRatio > 0f);
		}
		return false;
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00055AAB File Offset: 0x00053CAB
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(FullSpeedAheadCommand)).GetTemplate();
	}
}
