using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x020002C3 RID: 707
public class EndRivalryOption : TIPolicyOptionWithConfirm
{
	// Token: 0x06000A2D RID: 2605 RVA: 0x0003154F File Offset: 0x0002F74F
	public override PolicyType GetPolicyType()
	{
		return PolicyType.EndRivalryOption;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00031552 File Offset: 0x0002F752
	public override bool ImprovesRelations()
	{
		return true;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00031555 File Offset: 0x0002F755
	public override bool HandledAtFactionLevel()
	{
		return true;
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00031558 File Offset: 0x0002F758
	public override RelationChange relationChange
	{
		get
		{
			return RelationChange.RivalToNormal;
		}
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000A31 RID: 2609 RVA: 0x0003155B File Offset: 0x0002F75B
	public override string PromptName
	{
		get
		{
			return "PromptRespondToEndRivalryCall";
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00031562 File Offset: 0x0002F762
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return policyTarget.eligibleEndRivalries.ConvertAll<TIGameState>((TINationState x) => x);
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0003158E File Offset: 0x0002F78E
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.EndRivalry(enactingNation.executiveFaction, policyTarget as TINationState);
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x000315A2 File Offset: 0x0002F7A2
	public override float AIAgreeChance(TINationState proposingNation, TIGameState respondingNation)
	{
		return StratPolicyResponseSelector.ChanceEndRivalry(proposingNation, respondingNation.ref_nation);
	}
}
