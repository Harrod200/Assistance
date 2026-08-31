using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000389 RID: 905
public class FleetExtendRadiatorsCommand : TIFleetCommandTemplate
{
	// Token: 0x06001072 RID: 4210 RVA: 0x00054F4A File Offset: 0x0005314A
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.canIssueExtendRadiatorsCommand && !x.PartDestroyed(x.radiatorModule));
		}
		return false;
	}

	// Token: 0x06001073 RID: 4211 RVA: 0x00054F7C File Offset: 0x0005317C
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return (from x in base.GetEligibleShips(playerShips)
			where x.canIssueExtendRadiatorsCommand
			select x).ToList<TISpaceShipState>();
	}

	// Token: 0x06001074 RID: 4212 RVA: 0x00054FAE File Offset: 0x000531AE
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(ExtendRadiatorsCommand)).GetTemplate();
	}

	// Token: 0x06001075 RID: 4213 RVA: 0x00054FDE File Offset: 0x000531DE
	public override int IconPosition()
	{
		return 3;
	}
}
