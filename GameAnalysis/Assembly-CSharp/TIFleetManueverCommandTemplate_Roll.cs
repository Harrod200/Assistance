using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000393 RID: 915
public abstract class TIFleetManueverCommandTemplate_Roll : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010AA RID: 4266 RVA: 0x0005555F File Offset: 0x0005375F
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.SystemDestroyed(ShipSystem.VectorThrusters) && !x.Rolling() && !x.PerformingCombatManeuver(TIFleetManeuverCommandTemplate.exclusiveManeuvers));
		}
		return false;
	}
}
