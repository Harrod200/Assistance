using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003AB RID: 939
public class ExtendRadiatorsCommand : TIShipCommandTemplate
{
	// Token: 0x0600115E RID: 4446 RVA: 0x00056249 File Offset: 0x00054449
	public override int IconPosition()
	{
		return 8;
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0005624C File Offset: 0x0005444C
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return base.CommandVisibleToActor(ship) && ship.canIssueExtendRadiatorsCommand;
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0005625F File Offset: 0x0005445F
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.canIssueExtendRadiatorsCommand && !ship.PartDestroyed(ship.radiatorModule);
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x00056283 File Offset: 0x00054483
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new ExtendRadiatorsAction(ship));
		base.OnExecuteCommand(ship);
	}
}
