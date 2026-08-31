using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003A7 RID: 935
public interface IShipCommandWithTarget
{
	// Token: 0x06001140 RID: 4416
	void InitiateTargeting(TISpaceShipState ship);

	// Token: 0x06001141 RID: 4417
	void EndTargeting(TIFactionState faction);

	// Token: 0x06001142 RID: 4418
	Type GetTargetingMethod();

	// Token: 0x06001143 RID: 4419
	bool IncludeFriendlyTargets();

	// Token: 0x06001144 RID: 4420
	bool OnlyFriendlyTargets();

	// Token: 0x06001145 RID: 4421
	void OnCommandExecute(TISpaceShipState ship, CombatTargetableState target);
}
