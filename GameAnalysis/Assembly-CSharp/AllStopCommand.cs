using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003C1 RID: 961
public class AllStopCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011D0 RID: 4560 RVA: 0x00057251 File Offset: 0x00055451
	public override int IconPosition()
	{
		return 16;
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x00057255 File Offset: 0x00055455
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.AllStop;
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x00057259 File Offset: 0x00055459
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.CanRotateAndRoll() && !ship.activeCombatManeuvers.Contains(this.Maneuver());
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0005728F File Offset: 0x0005548F
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.activeCombatManeuvers.Clear();
		base.OnCommandExecute(ship, target);
	}
}
