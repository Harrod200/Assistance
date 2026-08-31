using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003C2 RID: 962
public class CancelAllStopCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011D5 RID: 4565 RVA: 0x000572AC File Offset: 0x000554AC
	public override int IconPosition()
	{
		return 16;
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x000572B0 File Offset: 0x000554B0
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelAllStop;
	}

	// Token: 0x060011D7 RID: 4567 RVA: 0x000572B4 File Offset: 0x000554B4
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.AllStop;
	}

	// Token: 0x060011D8 RID: 4568 RVA: 0x000572B8 File Offset: 0x000554B8
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x060011D9 RID: 4569 RVA: 0x000572CB File Offset: 0x000554CB
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x060011DA RID: 4570 RVA: 0x000572F6 File Offset: 0x000554F6
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new CancelCombatManeuverAction(ship, this.CancelManeuver()));
		base.OnExecuteCommand(ship);
	}
}
