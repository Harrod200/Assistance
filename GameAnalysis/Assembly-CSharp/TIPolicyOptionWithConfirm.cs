using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002BF RID: 703
public abstract class TIPolicyOptionWithConfirm : TIPolicyOption
{
	// Token: 0x1700014D RID: 333
	// (get) Token: 0x060009FE RID: 2558
	public abstract string PromptName { get; }

	// Token: 0x060009FF RID: 2559 RVA: 0x00030FB1 File Offset: 0x0002F1B1
	public override bool RequiresTargetConfirm()
	{
		return true;
	}

	// Token: 0x06000A00 RID: 2560
	public abstract float AIAgreeChance(TINationState proposingNation, TIGameState respondingState);

	// Token: 0x06000A01 RID: 2561 RVA: 0x00030FB4 File Offset: 0x0002F1B4
	public virtual float AIAgreeChance_Prospective(TINationState proposingNation, TIGameState respondingState)
	{
		return this.AIAgreeChance(proposingNation, respondingState);
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x00030FBE File Offset: 0x0002F1BE
	public override void OnConfirm(TINationState enactingNation, TIGameState policyTarget)
	{
		this.PromptPolicyResponse(enactingNation, policyTarget);
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x00030FC8 File Offset: 0x0002F1C8
	public virtual void PromptPolicyResponse(TINationState enactingNation, TIGameState policyTarget)
	{
		if (enactingNation.executiveFaction != null && policyTarget.ref_faction == enactingNation.executiveFaction)
		{
			this.EnactPolicy(enactingNation, policyTarget);
			return;
		}
		TIPromptQueueState.AddPromptStatic(policyTarget as TIPolityState, enactingNation, null, this.PromptName, 0);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x00031008 File Offset: 0x0002F208
	public override bool Allowed(TINationState nationState)
	{
		return this.GetPossibleTargets(nationState).Count > 0;
	}
}
