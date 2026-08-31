using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003C9 RID: 969
public class DefensiveManeuversCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x06001205 RID: 4613 RVA: 0x0005760A File Offset: 0x0005580A
	public override int IconPosition()
	{
		return 13;
	}

	// Token: 0x06001206 RID: 4614 RVA: 0x0005760E File Offset: 0x0005580E
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.DefensiveManuevers;
	}

	// Token: 0x06001207 RID: 4615 RVA: 0x00057612 File Offset: 0x00055812
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.CanRotateAndRoll() && !ship.activeCombatManeuvers.Contains(this.Maneuver());
	}

	// Token: 0x06001208 RID: 4616 RVA: 0x00057648 File Offset: 0x00055848
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.activeCombatManeuvers.Clear();
		base.OnCommandExecute(ship, target);
	}
}
