using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003A8 RID: 936
public class SelectTargetCommand : TIShipCommandTemplate, IShipCommandWithTarget
{
	// Token: 0x06001146 RID: 4422 RVA: 0x00055FFF File Offset: 0x000541FF
	public override int IconPosition()
	{
		return 0;
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x00056002 File Offset: 0x00054202
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && !ship.disengageFromCombat;
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x00056018 File Offset: 0x00054218
	public override bool RequiresTarget()
	{
		return true;
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x0005601B File Offset: 0x0005421B
	public bool IncludeFriendlyTargets()
	{
		return false;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0005601E File Offset: 0x0005421E
	public bool OnlyFriendlyTargets()
	{
		return false;
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x00056024 File Offset: 0x00054224
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target)
	{
		if (TIGameState.Valid(ship) && target != null && TIGameState.Valid(target.GetTargetableState()))
		{
			ship.faction.playerControl.StartAction(new SetCombatPrimaryTargetAction(ship, target));
			base.OnExecuteCommand(ship);
			if (!ship.combatAIControl)
			{
				this.EndTargeting(ship.faction);
			}
		}
	}

	// Token: 0x0600114C RID: 4428 RVA: 0x0005607B File Offset: 0x0005427B
	public Type GetTargetingMethod()
	{
		return typeof(TICommandTargetableTargeting);
	}

	// Token: 0x0600114D RID: 4429 RVA: 0x00056088 File Offset: 0x00054288
	public void InitiateTargeting(TISpaceShipState ship)
	{
		TICommandTargeting ticommandTargeting = Activator.CreateInstance(this.GetTargetingMethod()) as TICommandTargeting;
		ticommandTargeting.Initialize(ship, this);
		GeneralControlsController.SetUIGlobalTargetingMode(ship, ticommandTargeting);
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x000560B5 File Offset: 0x000542B5
	public void EndTargeting(TIFactionState faction)
	{
		GeneralControlsController.ShutdownUIGlobalTargetingMode(faction);
	}
}
