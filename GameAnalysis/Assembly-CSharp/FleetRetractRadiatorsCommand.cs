using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200038A RID: 906
public class FleetRetractRadiatorsCommand : TIFleetCommandTemplate
{
	// Token: 0x06001077 RID: 4215 RVA: 0x00054FE9 File Offset: 0x000531E9
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.canIssueRetractRadiatorsCommand && !x.PartDestroyed(x.radiatorModule));
		}
		return false;
	}

	// Token: 0x06001078 RID: 4216 RVA: 0x0005501B File Offset: 0x0005321B
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return (from x in base.GetEligibleShips(playerShips)
			where x.canIssueRetractRadiatorsCommand
			select x).ToList<TISpaceShipState>();
	}

	// Token: 0x06001079 RID: 4217 RVA: 0x0005504D File Offset: 0x0005324D
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(RetractRadiatorsCommand)).GetTemplate();
	}

	// Token: 0x0600107A RID: 4218 RVA: 0x0005507D File Offset: 0x0005327D
	public override int IconPosition()
	{
		return 4;
	}
}
