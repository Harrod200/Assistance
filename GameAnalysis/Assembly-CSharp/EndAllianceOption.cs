using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002B6 RID: 694
public class EndAllianceOption : TIPolicyOption
{
	// Token: 0x060009B8 RID: 2488 RVA: 0x0003082A File Offset: 0x0002EA2A
	public override PolicyType GetPolicyType()
	{
		return PolicyType.EndAllianceOption;
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0003082D File Offset: 0x0002EA2D
	public override bool DegradesRelations()
	{
		return true;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00030830 File Offset: 0x0002EA30
	public override bool HandledAtFactionLevel()
	{
		return true;
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x060009BB RID: 2491 RVA: 0x00030833 File Offset: 0x0002EA33
	public override RelationChange relationChange
	{
		get
		{
			return RelationChange.AllyToNormal;
		}
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00030836 File Offset: 0x0002EA36
	public override bool Allowed(TINationState nationState)
	{
		return this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00030847 File Offset: 0x0002EA47
	public override IList<TIGameState> GetPossibleTargets(TINationState policyNation)
	{
		return policyNation.eligibleEndAlliances.ConvertAll<TIGameState>((TINationState x) => x.ref_gameState);
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00030873 File Offset: 0x0002EA73
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		enactingNation.EndAlliance(enactingNation.executiveFaction, policyTarget as TINationState);
	}
}
