using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002BE RID: 702
public class DisbandArmyOption : TIPolicyOption
{
	// Token: 0x060009F7 RID: 2551 RVA: 0x00030EAD File Offset: 0x0002F0AD
	public override PolicyType GetPolicyType()
	{
		return PolicyType.DisbandArmyOption;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00030EB1 File Offset: 0x0002F0B1
	public override bool WeakensNation()
	{
		return true;
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00030EB4 File Offset: 0x0002F0B4
	public override bool Allowed(TINationState nationState)
	{
		return this.GetPossibleTargets(nationState).Count > 0 && nationState.ExecutivePowerConsolidated;
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00030ED0 File Offset: 0x0002F0D0
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return (from candidateArmy in policyTarget.armies
			where candidateArmy.faction == null || candidateArmy.faction == policyTarget.executiveFaction
			where candidateArmy.InFriendlyRegion
			where !candidateArmy.AlienMegafaunaArmy
			select candidateArmy).ToList<TIArmyState>().ConvertAll<TIGameState>((TIArmyState x) => x);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x00030F77 File Offset: 0x0002F177
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, policyTarget, null, this.Importance(enactingNation, policyTarget), "", "");
		this.OnPassage(enactingNation, policyTarget);
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00030F9C File Offset: 0x0002F19C
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		(policyTarget as TIArmyState).Disband();
	}
}
