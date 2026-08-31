using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000DD RID: 221
public class TICouncilorCondition_bIsAlien : TICouncilorCondition
{
	// Token: 0x060003D3 RID: 979 RVA: 0x00013B7C File Offset: 0x00011D7C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.isAlien, TIUtilities.GetBoolValue(this.strValue));
	}
}
