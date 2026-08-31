using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002B9 RID: 697
public class PeacefulBreakupOption : TIPolicyOption
{
	// Token: 0x060009D1 RID: 2513 RVA: 0x00030B96 File Offset: 0x0002ED96
	public override PolicyType GetPolicyType()
	{
		return PolicyType.PeacefulBreakupOption;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00030B9A File Offset: 0x0002ED9A
	public override bool WeakensNation()
	{
		return true;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00030B9D File Offset: 0x0002ED9D
	public override bool Allowed(TINationState nationState)
	{
		return nationState.ExecutivePowerConsolidated && this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00030BB8 File Offset: 0x0002EDB8
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		List<TIGameState> list = new List<TIGameState>();
		foreach (TINationState tinationState in GameStateManager.AllHumanNations())
		{
			if (tinationState.extant && tinationState.breakawayParent == policyTarget)
			{
				list.Add(tinationState);
			}
			else if (policyTarget.regions.Count > 1 && !tinationState.extant && tinationState.originalCapital != policyTarget.capital && policyTarget.regions.Contains(tinationState.originalCapital) && tinationState.claims.Contains(tinationState.originalCapital))
			{
				list.Add(tinationState);
			}
		}
		return list;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00030C7C File Offset: 0x0002EE7C
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		if (policyTarget.ref_nation.breakawayParent == enactingNation)
		{
			enactingNation.ReleaseBreakaway(enactingNation.executiveFaction, policyTarget.ref_nation, true);
			return;
		}
		enactingNation.ReleaseNation(enactingNation.executiveFaction, policyTarget.ref_nation, true);
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00030CB8 File Offset: 0x0002EEB8
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 2;
	}
}
