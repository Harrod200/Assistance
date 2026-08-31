using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A0 RID: 928
public class FleetMatchVelocityCommand : TIFleetManeuverCommandTemplate, IFleetCommandWithTarget
{
	// Token: 0x060010FD RID: 4349 RVA: 0x00055B55 File Offset: 0x00053D55
	public override int IconPosition()
	{
		return 20;
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00055B59 File Offset: 0x00053D59
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.MatchVelocity;
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00055B5D File Offset: 0x00053D5D
	public override bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips)
	{
		if (base.PlayerCanIssueCommand(playerShips))
		{
			return playerShips.Any<TISpaceShipState>((TISpaceShipState x) => x.AvailableDeltaVForCombat_kps() > 0f && x.CanRotateAndRoll());
		}
		return false;
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x00055B8F File Offset: 0x00053D8F
	public override TIShipCommandTemplate GetShipCommandTemplate()
	{
		return ShipCommandsManager.shipCommands.Single<IShipCommand>((IShipCommand x) => x.GetType() == typeof(MatchVelocityCommand)).GetTemplate();
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x00055BBF File Offset: 0x00053DBF
	public override bool RequiresTarget()
	{
		return true;
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x00055BC2 File Offset: 0x00053DC2
	public bool IncludeFriendlyTargets()
	{
		return (this.GetShipCommandTemplate() as IShipCommandWithTarget).IncludeFriendlyTargets();
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x00055BD4 File Offset: 0x00053DD4
	public bool OnlyFriendlyTargets()
	{
		return (this.GetShipCommandTemplate() as IShipCommandWithTarget).OnlyFriendlyTargets();
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x00055BE6 File Offset: 0x00053DE6
	public Type GetTargetingMethod()
	{
		return typeof(TICommandTargetableTargeting);
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x00055BF4 File Offset: 0x00053DF4
	public void InitiateTargeting(List<TISpaceShipState> ships)
	{
		ships = ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.ShipDestroyed()).ToList<TISpaceShipState>();
		TICommandTargeting ticommandTargeting = Activator.CreateInstance(this.GetTargetingMethod()) as TICommandTargeting;
		ticommandTargeting.Initialize(ships, this);
		this.ships = ships;
		GeneralControlsController.SetUIGlobalTargetingMode(ships[0], ticommandTargeting);
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x00055C5A File Offset: 0x00053E5A
	public void EndTargeting(TIFactionState faction)
	{
		GeneralControlsController.ShutdownUIGlobalTargetingMode(faction);
	}

	// Token: 0x040010BA RID: 4282
	public List<TISpaceShipState> ships;
}
