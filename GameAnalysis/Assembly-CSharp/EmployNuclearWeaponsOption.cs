using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002BC RID: 700
public class EmployNuclearWeaponsOption : TIPolicyOption
{
	// Token: 0x060009E9 RID: 2537 RVA: 0x00030DE4 File Offset: 0x0002EFE4
	public override PolicyType GetPolicyType()
	{
		return PolicyType.EmployNuclearWeaponsOption;
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00030DE8 File Offset: 0x0002EFE8
	public override bool Allowed(TINationState nationState)
	{
		return nationState.numNuclearWeapons > 0 && this.GetPossibleTargets(nationState).Count > 0;
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00030E04 File Offset: 0x0002F004
	public override bool HandledAtFactionLevel()
	{
		return true;
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00030E07 File Offset: 0x0002F007
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return policyTarget.NuclearWeaponsTargets(false).ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00030E34 File Offset: 0x0002F034
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		(policyTarget as TIRegionState).NuclearAttackOnRegion(enactingNation.executiveFaction, enactingNation);
		enactingNation.ChangeNumNuclearWeapons(-1);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00030E4F File Offset: 0x0002F04F
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return 2;
	}
}
