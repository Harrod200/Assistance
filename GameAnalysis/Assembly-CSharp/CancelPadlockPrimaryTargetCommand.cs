using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003C4 RID: 964
public class CancelPadlockPrimaryTargetCommand : TIShipManeuverCommandTemplate
{
	// Token: 0x060011E2 RID: 4578 RVA: 0x000573AE File Offset: 0x000555AE
	public override int IconPosition()
	{
		return 18;
	}

	// Token: 0x060011E3 RID: 4579 RVA: 0x000573B2 File Offset: 0x000555B2
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.CancelPadlock;
	}

	// Token: 0x060011E4 RID: 4580 RVA: 0x000573B5 File Offset: 0x000555B5
	public CombatManeuver CancelManeuver()
	{
		return CombatManeuver.Padlock;
	}

	// Token: 0x060011E5 RID: 4581 RVA: 0x000573B8 File Offset: 0x000555B8
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x060011E6 RID: 4582 RVA: 0x000573CB File Offset: 0x000555CB
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.activeCombatManeuvers.Contains(this.CancelManeuver());
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x000573E9 File Offset: 0x000555E9
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new CancelCombatManeuverAction(ship, this.CancelManeuver()));
		base.OnExecuteCommand(ship);
	}
}
