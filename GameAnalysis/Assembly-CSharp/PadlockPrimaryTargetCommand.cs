using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003C3 RID: 963
public class PadlockPrimaryTargetCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011DC RID: 4572 RVA: 0x00057323 File Offset: 0x00055523
	public override int IconPosition()
	{
		return 18;
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x00057327 File Offset: 0x00055527
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.Padlock;
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x0005732A File Offset: 0x0005552A
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return !ship.activeCombatManeuvers.Contains(this.Maneuver());
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x00057340 File Offset: 0x00055540
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.CanRotateAndRoll() && !ship.activeCombatManeuvers.Contains(this.Maneuver());
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x00057376 File Offset: 0x00055576
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.activeCombatManeuvers.Clear();
		ship.faction.playerControl.StartAction(new AddCombatManeuverAction(ship, this.Maneuver()));
		base.OnExecuteCommand(ship);
	}
}
