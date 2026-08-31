using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A5 RID: 933
public interface IShipCommand
{
	// Token: 0x06001124 RID: 4388
	string GetDisplayName();

	// Token: 0x06001125 RID: 4389
	string GetDescription(TISpaceShipState ship = null);

	// Token: 0x06001126 RID: 4390
	string GetTooltipText(TISpaceShipState ship = null);

	// Token: 0x06001127 RID: 4391
	int IconPosition();

	// Token: 0x06001128 RID: 4392
	string GetCommandIconImagePath_On();

	// Token: 0x06001129 RID: 4393
	string GetCommandIconImagePath_Off();

	// Token: 0x0600112A RID: 4394
	bool CommandVisibleToActor(TISpaceShipState ship);

	// Token: 0x0600112B RID: 4395
	bool ActorCanPerformCommand(TISpaceShipState ship);

	// Token: 0x0600112C RID: 4396
	void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target = null);

	// Token: 0x0600112D RID: 4397
	bool RequiresTarget();

	// Token: 0x0600112E RID: 4398
	TIResourcesCost GetResourcesCost(TISpaceShipState ship);

	// Token: 0x0600112F RID: 4399
	TIShipCommandTemplate GetTemplate();
}
