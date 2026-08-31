using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000385 RID: 901
public interface IFleetCommandWithTarget
{
	// Token: 0x06001054 RID: 4180
	void InitiateTargeting(List<TISpaceShipState> ships);

	// Token: 0x06001055 RID: 4181
	void EndTargeting(TIFactionState faction);

	// Token: 0x06001056 RID: 4182
	Type GetTargetingMethod();

	// Token: 0x06001057 RID: 4183
	bool IncludeFriendlyTargets();

	// Token: 0x06001058 RID: 4184
	bool OnlyFriendlyTargets();

	// Token: 0x06001059 RID: 4185
	void OnExecuteFleetCommand(List<TISpaceShipState> playerShips, CombatTargetableState target = null);
}
