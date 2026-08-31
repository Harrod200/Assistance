using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200038F RID: 911
public class FleetBalancedCommand : TIFleetCommandTemplate
{
	// Token: 0x06001094 RID: 4244 RVA: 0x0005531F File Offset: 0x0005351F
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && this.GetEligibleShips(playerShips).Count > 0;
	}

	// Token: 0x06001095 RID: 4245 RVA: 0x0005533B File Offset: 0x0005353B
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return (from x in base.GetEligibleShips(playerShips)
			where x.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate y) => y.attackMode)
			select x).ToList<TISpaceShipState>();
	}

	// Token: 0x06001096 RID: 4246 RVA: 0x0005536D File Offset: 0x0005356D
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(BalancedCommand)).GetTemplate();
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x0005539D File Offset: 0x0005359D
	public override int IconPosition()
	{
		return 9;
	}
}
