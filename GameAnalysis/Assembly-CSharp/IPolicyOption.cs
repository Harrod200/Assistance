using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002B3 RID: 691
public interface IPolicyOption
{
	// Token: 0x06000985 RID: 2437
	string templateName();

	// Token: 0x06000986 RID: 2438
	string GetDisplayName();

	// Token: 0x06000987 RID: 2439
	string GetDescription();

	// Token: 0x06000988 RID: 2440
	string GetTargetSelectionHeaderText();

	// Token: 0x06000989 RID: 2441
	string GetResponsePrompt(TINationState policyNation, TINationState respondingNation, TIGameState policyTarget);

	// Token: 0x0600098A RID: 2442
	IList<TIGameState> GetPossibleTargets(TINationState policyTarget);

	// Token: 0x0600098B RID: 2443
	void OnPassage(TINationState enactingNation, TIGameState policyTarget);

	// Token: 0x0600098C RID: 2444
	bool Allowed(TINationState nation);

	// Token: 0x0600098D RID: 2445
	bool RequiresTargets();

	// Token: 0x0600098E RID: 2446
	bool RequiresTargetConfirm();

	// Token: 0x0600098F RID: 2447
	void OnConfirm(TINationState enactingNation, TIGameState policyTarget);

	// Token: 0x06000990 RID: 2448
	int Importance(TINationState enactingNation, TIGameState policyTarget);

	// Token: 0x06000991 RID: 2449
	bool HandledAtFactionLevel();

	// Token: 0x06000992 RID: 2450
	bool ImprovesRelations();

	// Token: 0x06000993 RID: 2451
	bool DegradesRelations();

	// Token: 0x06000994 RID: 2452
	bool WeakensNation();
}
