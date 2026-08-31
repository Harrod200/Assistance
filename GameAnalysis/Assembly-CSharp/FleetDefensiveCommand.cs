using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000390 RID: 912
public class FleetDefensiveCommand : TIFleetCommandTemplate
{
	// Token: 0x06001099 RID: 4249 RVA: 0x000553A9 File Offset: 0x000535A9
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		return base.PlayerCanIssueCommand(playerShips) && this.GetEligibleShips(playerShips).Count > 0;
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x000553C5 File Offset: 0x000535C5
	public override List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips)
	{
		return playerShips.Where<TISpaceShipState>(delegate(TISpaceShipState x)
		{
			if (!x.combatAIControl)
			{
				return x.allWeaponTemplates.Any<TIShipWeaponTemplate>((TIShipWeaponTemplate y) => y.defenseMode);
			}
			return false;
		}).ToList<TISpaceShipState>();
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x000553F1 File Offset: 0x000535F1
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(DefensiveCommand)).GetTemplate();
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x00055421 File Offset: 0x00053621
	public override int IconPosition()
	{
		return 10;
	}
}
