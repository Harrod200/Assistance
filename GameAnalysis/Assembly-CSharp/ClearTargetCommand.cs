using System;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020003A9 RID: 937
public class ClearTargetCommand : TIShipCommandTemplate
{
	// Token: 0x06001150 RID: 4432 RVA: 0x000560C5 File Offset: 0x000542C5
	public override int IconPosition()
	{
		return 1;
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x000560C8 File Offset: 0x000542C8
	public override bool ActorCanPerformCommand(TISpaceShipState ship)
	{
		return base.ActorCanPerformCommand(ship) && ship.combatPrimaryTarget != null;
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x000560DE File Offset: 0x000542DE
	public override void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null)
	{
		ship.faction.playerControl.StartAction(new ClearPrimaryTargetAction(ship));
		base.OnExecuteCommand(ship);
	}
}
