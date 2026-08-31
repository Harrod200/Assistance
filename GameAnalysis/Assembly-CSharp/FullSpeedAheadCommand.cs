using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003C5 RID: 965
public class FullSpeedAheadCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011E9 RID: 4585 RVA: 0x00057416 File Offset: 0x00055616
	public override int IconPosition()
	{
		return 12;
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0005741A File Offset: 0x0005561A
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.FullSpeedAhead;
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0005741E File Offset: 0x0005561E
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.ThrustEffectivenessRatio > 0f;
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x00057445 File Offset: 0x00055645
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.activeCombatManeuvers.Clear();
		base.OnCommandExecute(ship, target);
	}
}
