using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003C6 RID: 966
public class InterceptCourseCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011EE RID: 4590 RVA: 0x00057462 File Offset: 0x00055662
	public override int IconPosition()
	{
		return 14;
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x00057466 File Offset: 0x00055666
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.InterceptCourse;
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0005746A File Offset: 0x0005566A
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.CanRotateAndRoll() && ship.combatPrimaryTarget != null;
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x00057495 File Offset: 0x00055695
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.activeCombatManeuvers.Clear();
		base.OnCommandExecute(ship, target);
	}
}
