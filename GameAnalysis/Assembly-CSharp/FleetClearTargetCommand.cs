using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000387 RID: 903
public class FleetClearTargetCommand : TIFleetCommandTemplate
{
	// Token: 0x06001063 RID: 4195 RVA: 0x00054D9A File Offset: 0x00052F9A
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.combatPrimaryTarget != null);
		}
		return false;
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00054DCC File Offset: 0x00052FCC
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return (from x in base.GetEligibleShips(playerShips)
			where x.combatPrimaryTarget != null
			select x).ToList<TISpaceShipState>();
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00054DFE File Offset: 0x00052FFE
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(ClearTargetCommand)).GetTemplate();
	}

	// Token: 0x06001066 RID: 4198 RVA: 0x00054E2E File Offset: 0x0005302E
	public override int IconPosition()
	{
		return 1;
	}
}
