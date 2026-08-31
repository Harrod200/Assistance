using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002B8 RID: 696
public class InitiateRivalryOption : TIPolicyOption
{
	// Token: 0x060009C9 RID: 2505 RVA: 0x00030B21 File Offset: 0x0002ED21
	public override PolicyType GetPolicyType()
	{
		return PolicyType.InitiateRivalryOption;
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x00030B24 File Offset: 0x0002ED24
	public override bool DegradesRelations()
	{
		return true;
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00030B27 File Offset: 0x0002ED27
	public override bool HandledAtFactionLevel()
	{
		return true;
	}

	// Token: 0x1700014C RID: 332
	// (get) Token: 0x060009CC RID: 2508 RVA: 0x00030B2A File Offset: 0x0002ED2A
	public override RelationChange relationChange
	{
		get
		{
			return RelationChange.NormalToRival;
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x00030B2D File Offset: 0x0002ED2D
	public override bool Allowed(TINationState nationState)
	{
		return this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x00030B3E File Offset: 0x0002ED3E
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return policyTarget.eligibleRivals.ConvertAll<TIGameState>((TINationState x) => x);
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x00030B6A File Offset: 0x0002ED6A
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		if (enactingNation.CanRival(policyTarget.ref_nation))
		{
			enactingNation.InitiateRivalry(enactingNation.executiveFaction, policyTarget.ref_nation, false, false);
		}
	}
}
