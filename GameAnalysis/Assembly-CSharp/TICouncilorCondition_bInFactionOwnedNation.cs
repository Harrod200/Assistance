using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D8 RID: 216
public class TICouncilorCondition_bInFactionOwnedNation : TICouncilorCondition
{
	// Token: 0x060003C3 RID: 963 RVA: 0x00013735 File Offset: 0x00011935
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060003C4 RID: 964 RVA: 0x00013740 File Offset: 0x00011940
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && state.ref_councilor.currentNation != null && TICondition.PassesComparison(this.sign, state.ref_councilor.currentNation.ref_faction == state.ref_councilor.ref_faction, TIUtilities.GetBoolValue(this.strValue));
	}
}
