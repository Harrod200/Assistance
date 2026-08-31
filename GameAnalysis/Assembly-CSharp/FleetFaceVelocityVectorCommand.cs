using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A4 RID: 932
public class FleetFaceVelocityVectorCommand : TIFleetManeuverCommandTemplate
{
	// Token: 0x0600111F RID: 4383 RVA: 0x00055E34 File Offset: 0x00054034
	public override int IconPosition()
	{
		return 19;
	}

	// Token: 0x06001120 RID: 4384 RVA: 0x00055E38 File Offset: 0x00054038
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.FaceVelocityVector;
	}

	// Token: 0x06001121 RID: 4385 RVA: 0x00055E3C File Offset: 0x0005403C
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.CanRotateAndRoll());
		}
		return false;
	}

	// Token: 0x06001122 RID: 4386 RVA: 0x00055E6E File Offset: 0x0005406E
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(FaceVelocityVectorCommand)).GetTemplate();
	}
}
