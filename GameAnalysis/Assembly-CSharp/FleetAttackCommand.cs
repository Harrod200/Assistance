using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200038E RID: 910
public class FleetAttackCommand : TIFleetCommandTemplate
{
	// Token: 0x0600108F RID: 4239 RVA: 0x00055296 File Offset: 0x00053496
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && this.GetEligibleShips(playerShips).Count > 0;
	}

	// Token: 0x06001090 RID: 4240 RVA: 0x000552B2 File Offset: 0x000534B2
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return (from x in base.GetEligibleShips(playerShips)
			where x.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate y) => y.attackMode)
			select x).ToList<TISpaceShipState>();
	}

	// Token: 0x06001091 RID: 4241 RVA: 0x000552E4 File Offset: 0x000534E4
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(AttackCommand)).GetTemplate();
	}

	// Token: 0x06001092 RID: 4242 RVA: 0x00055314 File Offset: 0x00053514
	public override int IconPosition()
	{
		return 8;
	}
}
