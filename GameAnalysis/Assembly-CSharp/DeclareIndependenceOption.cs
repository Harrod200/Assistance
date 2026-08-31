using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002BA RID: 698
public class DeclareIndependenceOption : TIPolicyOption
{
	// Token: 0x060009D8 RID: 2520 RVA: 0x00030CC3 File Offset: 0x0002EEC3
	public override bool DegradesRelations()
	{
		return true;
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x00030CC6 File Offset: 0x0002EEC6
	public override PolicyType GetPolicyType()
	{
		return PolicyType.DeclareIndependenceOption;
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x00030CCA File Offset: 0x0002EECA
	public override bool Allowed(TINationState nationState)
	{
		return nationState.breakaway;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x00030CD2 File Offset: 0x0002EED2
	public override bool RequiresTargets()
	{
		return true;
	}

	// Token: 0x060009DC RID: 2524 RVA: 0x00030CD5 File Offset: 0x0002EED5
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 2;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x00030CD8 File Offset: 0x0002EED8
	public override IList<TIGameState> GetPossibleTargets(TINationState policyNation)
	{
		return new List<TIGameState> { policyNation.breakawayParent };
	}

	// Token: 0x060009DE RID: 2526 RVA: 0x00030CEB File Offset: 0x0002EEEB
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.breakawayParent.ReleaseBreakaway(enactingNation.executiveFaction, enactingNation, false);
	}

	// Token: 0x060009DF RID: 2527 RVA: 0x00030D00 File Offset: 0x0002EF00
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, enactingNation.breakawayParent, policyTarget, this.Importance(enactingNation, policyTarget), "", "");
		this.OnPassage(enactingNation, enactingNation.breakawayParent);
	}
}
