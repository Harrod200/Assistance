using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003C7 RID: 967
public class MatchVelocityCommand : TIShipManeuverCommandWithTargetTemplate, IShipCommandWithTarget
{
	// Token: 0x060011F3 RID: 4595 RVA: 0x000574B2 File Offset: 0x000556B2
	public override int IconPosition()
	{
		return 20;
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x000574B6 File Offset: 0x000556B6
	public override CombatManeuver Maneuver()
	{
		return CombatManeuver.MatchVelocity;
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x000574BA File Offset: 0x000556BA
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.AvailableDeltaVForCombat_kps() > 0f && ship.CanRotateAndRoll() && !ship.activeCombatManeuvers.Contains(this.Maneuver());
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x000574F0 File Offset: 0x000556F0
	public override bool RequiresTarget()
	{
		return true;
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x000574F3 File Offset: 0x000556F3
	public bool IncludeFriendlyTargets()
	{
		return true;
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x000574F6 File Offset: 0x000556F6
	public bool OnlyFriendlyTargets()
	{
		return false;
	}

	// Token: 0x060011F9 RID: 4601 RVA: 0x000574F9 File Offset: 0x000556F9
	public Type GetTargetingMethod()
	{
		return typeof(TICommandTargetableTargeting);
	}

	// Token: 0x060011FA RID: 4602 RVA: 0x00057508 File Offset: 0x00055708
	public void InitiateTargeting(TISpaceShipState ship)
	{
		TICommandTargeting ticommandTargeting = Activator.CreateInstance(this.GetTargetingMethod()) as TICommandTargeting;
		ticommandTargeting.Initialize(ship, this);
		GeneralControlsController.SetUIGlobalTargetingMode(ship, ticommandTargeting);
	}

	// Token: 0x060011FB RID: 4603 RVA: 0x00057535 File Offset: 0x00055735
	public void EndTargeting(TIFactionState faction)
	{
		GeneralControlsController.ShutdownUIGlobalTargetingMode(faction);
	}

	// Token: 0x060011FC RID: 4604 RVA: 0x0005753D File Offset: 0x0005573D
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new SetManeuverPrimaryTargetAction(ship, target));
		ship.activeCombatManeuvers.Clear();
		base.OnCommandExecute(ship, target);
		this.EndTargeting(ship.faction);
	}
}
