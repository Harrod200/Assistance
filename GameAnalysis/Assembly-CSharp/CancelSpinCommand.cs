using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003BE RID: 958
public abstract class CancelSpinCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011C3 RID: 4547 RVA: 0x000571B5 File Offset: 0x000553B5
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return base.CommandVisibleToActor(ship) && ship.PerformingCombatManeuver(this.CancelManeuver());
	}

	// Token: 0x060011C4 RID: 4548 RVA: 0x000571CE File Offset: 0x000553CE
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.PerformingCombatManeuver(this.CancelManeuver()) && !ship.PerformingCombatManeuver(this.Maneuver()) && ship.CanRotateAndRoll();
	}

	// Token: 0x060011C5 RID: 4549
	public abstract CombatManeuver CancelManeuver();

	// Token: 0x060011C6 RID: 4550 RVA: 0x000571FD File Offset: 0x000553FD
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new CancelCombatManeuverAction(ship, this.CancelManeuver()));
		base.OnExecuteCommand(ship);
	}
}
