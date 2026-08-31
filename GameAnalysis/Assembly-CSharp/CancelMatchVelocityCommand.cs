using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003C8 RID: 968
public class CancelMatchVelocityCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011FE RID: 4606 RVA: 0x0005757D File Offset: 0x0005577D
	public override int IconPosition()
	{
		return 20;
	}

	// Token: 0x060011FF RID: 4607 RVA: 0x00057581 File Offset: 0x00055781
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelMatchVelocity;
	}

	// Token: 0x06001200 RID: 4608 RVA: 0x00057585 File Offset: 0x00055785
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.MatchVelocity;
	}

	// Token: 0x06001201 RID: 4609 RVA: 0x00057589 File Offset: 0x00055789
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x06001202 RID: 4610 RVA: 0x0005759C File Offset: 0x0005579C
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x06001203 RID: 4611 RVA: 0x000575C7 File Offset: 0x000557C7
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new CancelCombatManeuverAction(ship, this.CancelManeuver()));
		ship.faction.playerControl.StartAction(new ClearManeuverTargetAction(ship));
		base.OnExecuteCommand(ship);
	}
}
