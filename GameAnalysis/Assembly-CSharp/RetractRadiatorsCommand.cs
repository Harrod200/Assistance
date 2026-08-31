using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003AC RID: 940
public class RetractRadiatorsCommand : TIShipCommandTemplate
{
	// Token: 0x06001163 RID: 4451 RVA: 0x000562AA File Offset: 0x000544AA
	public override int IconPosition()
	{
		return 8;
	}

	// Token: 0x06001164 RID: 4452 RVA: 0x000562AD File Offset: 0x000544AD
	public override bool CommandVisibleToActor(TISpaceShipState ship)
	{
		return base.CommandVisibleToActor(ship) && ship.canIssueRetractRadiatorsCommand;
	}

	// Token: 0x06001165 RID: 4453 RVA: 0x000562C0 File Offset: 0x000544C0
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.canIssueRetractRadiatorsCommand && !ship.PartDestroyed(ship.radiatorModule);
	}

	// Token: 0x06001166 RID: 4454 RVA: 0x000562E4 File Offset: 0x000544E4
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new RetractRadiatorsAction(ship));
		base.OnExecuteCommand(ship);
	}
}
