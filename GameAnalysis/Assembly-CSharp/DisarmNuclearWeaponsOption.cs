using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002BD RID: 701
public class DisarmNuclearWeaponsOption : TIPolicyOption
{
	// Token: 0x060009F0 RID: 2544 RVA: 0x00030E5A File Offset: 0x0002F05A
	public override PolicyType GetPolicyType()
	{
		return PolicyType.DisarmNuclearWeaponsOption;
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00030E5E File Offset: 0x0002F05E
	public override bool WeakensNation()
	{
		return true;
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00030E61 File Offset: 0x0002F061
	public override bool Allowed(TINationState nationState)
	{
		return nationState.numNuclearWeapons > 0 && nationState.ExecutivePowerConsolidated;
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00030E74 File Offset: 0x0002F074
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return new List<TIGameState> { policyTarget };
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00030E82 File Offset: 0x0002F082
	public override bool RequiresTargets()
	{
		return false;
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00030E85 File Offset: 0x0002F085
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.ChangeNumNuclearWeapons(-5);
		if (enactingNation.numNuclearWeapons == 0)
		{
			enactingNation.SetAccumulatedInvestmentPoints(PriorityType.Military_BuildNuclearWeapons, 0f, true);
		}
	}
}
