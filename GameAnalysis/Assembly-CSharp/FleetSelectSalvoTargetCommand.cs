using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000388 RID: 904
public class FleetSelectSalvoTargetCommand : TIFleetCommandTemplate, IFleetCommandWithTarget
{
	// Token: 0x06001068 RID: 4200 RVA: 0x00054E39 File Offset: 0x00053039
	public override int IconPosition()
	{
		return 2;
	}

	// Token: 0x06001069 RID: 4201 RVA: 0x00054E3C File Offset: 0x0005303C
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => !x.disengageFromCombat && x.AnyOffensiveMissileWeaponCanFire());
		}
		return false;
	}

	// Token: 0x0600106A RID: 4202 RVA: 0x00054E6E File Offset: 0x0005306E
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(SelectSalvoTargetCommand)).GetTemplate();
	}

	// Token: 0x0600106B RID: 4203 RVA: 0x00054E9E File Offset: 0x0005309E
	public override bool RequiresTarget()
	{
		return true;
	}

	// Token: 0x0600106C RID: 4204 RVA: 0x00054EA1 File Offset: 0x000530A1
	public bool IncludeFriendlyTargets()
	{
		return (this.GetShipCommandTemplate() as IShipCommandWithTarget).IncludeFriendlyTargets();
	}

	// Token: 0x0600106D RID: 4205 RVA: 0x00054EB3 File Offset: 0x000530B3
	public bool OnlyFriendlyTargets()
	{
		return (this.GetShipCommandTemplate() as IShipCommandWithTarget).OnlyFriendlyTargets();
	}

	// Token: 0x0600106E RID: 4206 RVA: 0x00054EC5 File Offset: 0x000530C5
	public Type GetTargetingMethod()
	{
		return typeof(TICommandTargetableTargeting);
	}

	// Token: 0x0600106F RID: 4207 RVA: 0x00054ED4 File Offset: 0x000530D4
	public void InitiateTargeting(List<TISpaceShipState> ships)
	{
		ships = ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.ShipDestroyed()).ToList<TISpaceShipState>();
		TICommandTargeting ticommandTargeting = Activator.CreateInstance(this.GetTargetingMethod()) as TICommandTargeting;
		ticommandTargeting.Initialize(ships, this);
		this.ships = ships;
		GeneralControlsController.SetUIGlobalTargetingMode(ships[0], ticommandTargeting);
	}

	// Token: 0x06001070 RID: 4208 RVA: 0x00054F3A File Offset: 0x0005313A
	public void EndTargeting(TIFactionState faction)
	{
		GeneralControlsController.ShutdownUIGlobalTargetingMode(faction);
	}

	// Token: 0x040010B8 RID: 4280
	public List<TISpaceShipState> ships;
}
