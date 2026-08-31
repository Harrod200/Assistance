using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000391 RID: 913
public class FleetFortifyCommand : TIFleetCommandTemplate
{
	// Token: 0x0600109E RID: 4254 RVA: 0x0005542D File Offset: 0x0005362D
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && this.GetEligibleShips(playerShips).Count > 0;
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x00055449 File Offset: 0x00053649
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return playerShips.Where<TISpaceShipState>((TISpaceShipState x) => !x.combatAIControl && x.allWeaponTemplates.Count > 0).ToList<TISpaceShipState>();
	}

	// Token: 0x060010A0 RID: 4256 RVA: 0x00055475 File Offset: 0x00053675
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(FortifyCommand)).GetTemplate();
	}

	// Token: 0x060010A1 RID: 4257 RVA: 0x000554A5 File Offset: 0x000536A5
	public override int IconPosition()
	{
		return 11;
	}
}
