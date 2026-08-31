using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200008E RID: 142
public class TIRegionCondition_bAlienActivityExtant : TIRegionCondition
{
	// Token: 0x060002F1 RID: 753 RVA: 0x00011AD1 File Offset: 0x0000FCD1
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00011AD9 File Offset: 0x0000FCD9
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.alienActivity.Extant(), TIUtilities.GetBoolValue(this.strValue));
	}
}
