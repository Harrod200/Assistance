using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Tasks;

// Token: 0x020002C4 RID: 708
public class ProposeAllianceOption : TIPolicyOptionWithConfirm
{
	// Token: 0x06000A36 RID: 2614 RVA: 0x000315B8 File Offset: 0x0002F7B8
	public override PolicyType GetPolicyType()
	{
		return PolicyType.ProposeAllianceOption;
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x000315BB File Offset: 0x0002F7BB
	public override bool ImprovesRelations()
	{
		return true;
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x000315BE File Offset: 0x0002F7BE
	public override bool HandledAtFactionLevel()
	{
		return true;
	}

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000A39 RID: 2617 RVA: 0x000315C1 File Offset: 0x0002F7C1
	public override RelationChange relationChange
	{
		get
		{
			return RelationChange.NormalToAlly;
		}
	}

	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000A3A RID: 2618 RVA: 0x000315C4 File Offset: 0x0002F7C4
	public override string PromptName
	{
		get
		{
			return "PromptRespondToFormAllianceCall";
		}
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x000315CB File Offset: 0x0002F7CB
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return policyTarget.eligibleAlliances.ConvertAll<TIGameState>((TINationState x) => x);
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x000315F7 File Offset: 0x0002F7F7
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.InitiateAlliance(enactingNation.executiveFaction, policyTarget as TINationState);
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x0003160C File Offset: 0x0002F80C
	public override void DeclinePolicy(TINationState enactingNation, TIGameState policyTarget)
	{
		base.DeclinePolicy(enactingNation, policyTarget);
		foreach (TIArmyState tiarmyState in enactingNation.armies)
		{
			if (policyTarget.ref_nation.regions.Contains(tiarmyState.currentRegion))
			{
				tiarmyState.TeleportArmyFromIllegalRegion();
			}
		}
	}

	// Token: 0x06000A3E RID: 2622 RVA: 0x00031680 File Offset: 0x0002F880
	public override float AIAgreeChance(TINationState proposingNation, TIGameState respondingNation)
	{
		return StratPolicyResponseSelector.ChanceFormAlliance(proposingNation, respondingNation.ref_nation);
	}
}
