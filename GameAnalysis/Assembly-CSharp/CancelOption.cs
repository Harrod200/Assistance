using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002BB RID: 699
public class CancelOption : TIPolicyOption
{
	// Token: 0x060009E1 RID: 2529 RVA: 0x00030D37 File Offset: 0x0002EF37
	public override PolicyType GetPolicyType()
	{
		return PolicyType.CancelOption;
	}

	// Token: 0x060009E2 RID: 2530 RVA: 0x00030D3B File Offset: 0x0002EF3B
	public override bool Allowed(TINationState nationState)
	{
		return true;
	}

	// Token: 0x060009E3 RID: 2531 RVA: 0x00030D40 File Offset: 0x0002EF40
	public override string GetDescription()
	{
		return Loc.T(new StringBuilder(base.dataName).Append(".description").ToString(), new object[]
		{
			TIUtilities.InlineResourceStr(TIFactionState.setPolicyMission.cost.resourceType),
			TIFactionState.setPolicyMission.cost.value
		});
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x00030DA0 File Offset: 0x0002EFA0
	public override IList<TIGameState> GetPossibleTargets(TINationState policyTarget)
	{
		return null;
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x00030DA3 File Offset: 0x0002EFA3
	public override void OnPassage(TINationState enactingNation, TIGameState policyTarget)
	{
		TIFactionState executiveFaction = enactingNation.executiveFaction;
		if (executiveFaction == null)
		{
			return;
		}
		executiveFaction.AddToCurrentResource(TIFactionState.setPolicyMission.cost.value, TIFactionState.setPolicyMission.cost.resourceType, false, null);
	}

	// Token: 0x060009E6 RID: 2534 RVA: 0x00030DD6 File Offset: 0x0002EFD6
	public override bool RequiresTargets()
	{
		return false;
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00030DD9 File Offset: 0x0002EFD9
	public override int Importance(TINationState policyNation, TIGameState target)
	{
		return -1;
	}
}
