using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A2 RID: 930
public class FleetDefensiveManeuversCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x06001111 RID: 4369 RVA: 0x00055D16 File Offset: 0x00053F16
	public override int IconPosition()
	{
		return 13;
	}

	// Token: 0x06001112 RID: 4370 RVA: 0x00055D1A File Offset: 0x00053F1A
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.DefensiveManuevers;
	}

	// Token: 0x06001113 RID: 4371 RVA: 0x00055D1E File Offset: 0x00053F1E
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.CanRotateAndRoll());
		}
		return false;
	}

	// Token: 0x06001114 RID: 4372 RVA: 0x00055D50 File Offset: 0x00053F50
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(DefensiveManeuversCommand)).GetTemplate();
	}
}
