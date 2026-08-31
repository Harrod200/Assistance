using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x020002C2 RID: 706
public class UnificationOption : TIPolicyOptionWithConfirm
{
	// Token: 0x06000A23 RID: 2595 RVA: 0x00031467 File Offset: 0x0002F667
	public override PolicyType GetPolicyType()
	{
		return PolicyType.UnificationOption;
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x0003146A File Offset: 0x0002F66A
	public override bool ImprovesRelations()
	{
		return true;
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000A25 RID: 2597 RVA: 0x0003146D File Offset: 0x0002F66D
	public override string PromptName
	{
		get
		{
			return "PromptRespondToUnificationCall";
		}
	}

	// Token: 0x06000A26 RID: 2598 RVA: 0x00031474 File Offset: 0x0002F674
	public override bool Allowed(TINationState nationState)
	{
		return nationState.ExecutivePowerConsolidated && base.Allowed(nationState);
	}

	// Token: 0x06000A27 RID: 2599 RVA: 0x00031487 File Offset: 0x0002F687
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return policyTarget.eligibleUnifications.ConvertAll<TIGameState>((TINationState x) => x);
	}

	// Token: 0x06000A28 RID: 2600 RVA: 0x000314B4 File Offset: 0x0002F6B4
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		string displayNameWithArticle = policyTarget.ref_nation.displayNameWithArticle;
		string flagResource = policyTarget.ref_nation.flagResource;
		this.OnPassage(enactingNation, policyTarget);
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, policyTarget, null, this.Importance(enactingNation, policyTarget), displayNameWithArticle, flagResource);
	}

	// Token: 0x06000A29 RID: 2601 RVA: 0x000314F4 File Offset: 0x0002F6F4
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		TINationState ref_nation = policyTarget.ref_nation;
		if (enactingNation.breakawayParent == ref_nation)
		{
			ref_nation.Unification(enactingNation.executiveFaction, enactingNation);
			return;
		}
		enactingNation.Unification(enactingNation.executiveFaction, policyTarget.ref_nation);
	}

	// Token: 0x06000A2A RID: 2602 RVA: 0x00031536 File Offset: 0x0002F736
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 2;
	}

	// Token: 0x06000A2B RID: 2603 RVA: 0x00031539 File Offset: 0x0002F739
	public override float AIAgreeChance(TINationState proposingNation, TIGameState respondingNation)
	{
		return StratPolicyResponseSelector.ChanceUnification(proposingNation, respondingNation.ref_nation);
	}
}
