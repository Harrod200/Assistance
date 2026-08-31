using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x020002C0 RID: 704
public class LeaveFederationOption : TIPolicyOptionWithConfirm
{
	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000A06 RID: 2566 RVA: 0x00031021 File Offset: 0x0002F221
	public override string PromptName
	{
		get
		{
			return "PromptNationLeavesDarkFederation_Policy";
		}
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00031028 File Offset: 0x0002F228
	public override PolicyType GetPolicyType()
	{
		return PolicyType.LeaveFederationOption;
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0003102B File Offset: 0x0002F22B
	public override bool DegradesRelations()
	{
		return true;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0003102E File Offset: 0x0002F22E
	public override bool RequiresTargets()
	{
		return true;
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000A0A RID: 2570 RVA: 0x00031031 File Offset: 0x0002F231
	public override bool TargetsMyFederation
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x00031034 File Offset: 0x0002F234
	public override bool Allowed(TINationState nationState)
	{
		return nationState.extant && nationState.CanLeaveFederation();
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00031046 File Offset: 0x0002F246
	public override float AIAgreeChance(TINationState proposingNation, TIGameState fedLeader)
	{
		if (proposingNation.federation.hegemonicFederation)
		{
			return StratPolicyResponseSelector.ChanceAllowDarkFederationDeparture(proposingNation);
		}
		return 1f;
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00031061 File Offset: 0x0002F261
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		if (policyTarget.extant)
		{
			return new List<TIGameState>(1) { policyTarget.federation };
		}
		return new List<TIGameState>();
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x00031084 File Offset: 0x0002F284
	public override string GetResponsePrompt(TINationState policyNation, TINationState respondingNation, TIGameState policyTarget)
	{
		return Loc.T(new StringBuilder(base.GetType().Name).Append(".responsePrompt").ToString(), new object[]
		{
			policyNation.displayNameWithArticleAndPlacePrep,
			policyTarget.ref_nation.federation.displayNameWithArticle,
			policyTarget.ref_nation.federation.leadNation.displayName
		});
	}

	// Token: 0x06000A0F RID: 2575 RVA: 0x000310EF File Offset: 0x0002F2EF
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.federation.RemoveNation(enactingNation.executiveFaction, enactingNation, true);
	}

	// Token: 0x06000A10 RID: 2576 RVA: 0x00031104 File Offset: 0x0002F304
	public override void PromptPolicyResponse(TINationState enactingNation, TIGameState policyTarget)
	{
		if (!enactingNation.federation.hegemonicFederation || (enactingNation.executiveFaction != null && policyTarget.ref_faction == enactingNation.executiveFaction))
		{
			this.EnactPolicy(enactingNation, policyTarget);
			return;
		}
		TIPromptQueueState.AddPromptStatic(policyTarget.ref_nation, enactingNation, policyTarget, this.PromptName, 0);
	}

	// Token: 0x06000A11 RID: 2577 RVA: 0x0003115C File Offset: 0x0002F35C
	public override void DeclinePolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINotificationQueueState.LogPolicyDeclined(this, enactingNation, policyTarget as TINationState);
		if (enactingNation.federation.hegemonicFederation)
		{
			enactingNation.federation.RecordAttemptToLeaveDarkFederation(enactingNation);
		}
	}

	// Token: 0x06000A12 RID: 2578 RVA: 0x00031184 File Offset: 0x0002F384
	public override void EnactPolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		TINotificationQueueState.LogPolicyAdopted(this, enactingNation, policyTarget, null, this.Importance(enactingNation, policyTarget), "", "");
		this.OnPassage(enactingNation, policyTarget);
	}
}
