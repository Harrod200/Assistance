using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200039F RID: 927
public class FleetInterceptCourseCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x060010F8 RID: 4344 RVA: 0x00055AE3 File Offset: 0x00053CE3
	public override int IconPosition()
	{
		return 14;
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x00055AE7 File Offset: 0x00053CE7
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.InterceptCourse;
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x00055AEB File Offset: 0x00053CEB
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.CanRotateAndRoll() && x.combatPrimaryTarget != null);
		}
		return false;
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x00055B1D File Offset: 0x00053D1D
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(InterceptCourseCommand)).GetTemplate();
	}
}
