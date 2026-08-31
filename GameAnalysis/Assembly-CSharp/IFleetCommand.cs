using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000383 RID: 899
public interface IFleetCommand
{
	// Token: 0x06001039 RID: 4153
	string GetDisplayName(bool isGroupCommand = false);

	// Token: 0x0600103A RID: 4154
	string GetDescription(bool isGroupCommand = false);

	// Token: 0x0600103B RID: 4155
	string GetTooltipText(bool isGroupCommand = false);

	// Token: 0x0600103C RID: 4156
	int IconPosition();

	// Token: 0x0600103D RID: 4157
	string GetCommandIconImagePath_On();

	// Token: 0x0600103E RID: 4158
	string GetCommandIconImagePath_Off();

	// Token: 0x0600103F RID: 4159
	bool CommandVisibleToPlayer(List<TISpaceShipState> playerShips);

	// Token: 0x06001040 RID: 4160
	bool PlayerCanIssueCommand(List<TISpaceShipState> playerShips);

	// Token: 0x06001041 RID: 4161
	List<TISpaceShipState> GetEligibleShips(List<TISpaceShipState> playerShips);

	// Token: 0x06001042 RID: 4162
	void OnExecuteFleetCommand(List<TISpaceShipState> playerShips, CombatTargetableState target = null);

	// Token: 0x06001043 RID: 4163
	bool RequiresTarget();

	// Token: 0x06001044 RID: 4164
	TIFleetCommandTemplate GetTemplate();
}
