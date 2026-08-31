using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003CA RID: 970
public class CancelDefensiveManeuversCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x0600120A RID: 4618 RVA: 0x00057665 File Offset: 0x00055865
	public override int IconPosition()
	{
		return 13;
	}

	// Token: 0x0600120B RID: 4619 RVA: 0x00057669 File Offset: 0x00055869
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelDefensiveManeuvers;
	}

	// Token: 0x0600120C RID: 4620 RVA: 0x0005766D File Offset: 0x0005586D
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.DefensiveManuevers;
	}

	// Token: 0x0600120D RID: 4621 RVA: 0x00057671 File Offset: 0x00055871
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x00057684 File Offset: 0x00055884
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x000576AF File Offset: 0x000558AF
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new CancelCombatManeuverAction(ship, this.CancelManeuver()));
		base.OnExecuteCommand(ship);
	}
}
