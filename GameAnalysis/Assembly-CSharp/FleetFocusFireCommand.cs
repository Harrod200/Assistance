using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200038D RID: 909
public class FleetFocusFireCommand : TIFleetCommandTemplate
{
	// Token: 0x0600108A RID: 4234 RVA: 0x0005520D File Offset: 0x0005340D
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && this.GetEligibleShips(playerShips).Count > 0;
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x00055229 File Offset: 0x00053429
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return (from x in base.GetEligibleShips(playerShips)
			where x.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate y) => y.attackMode)
			select x).ToList<TISpaceShipState>();
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x0005525B File Offset: 0x0005345B
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(FocusFireCommand)).GetTemplate();
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x0005528B File Offset: 0x0005348B
	public override int IconPosition()
	{
		return 7;
	}
}
