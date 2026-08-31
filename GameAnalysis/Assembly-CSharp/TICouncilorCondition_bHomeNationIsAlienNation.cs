using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000DF RID: 223
public class TICouncilorCondition_bHomeNationIsAlienNation : TICouncilorCondition
{
	// Token: 0x060003D8 RID: 984 RVA: 0x00013C30 File Offset: 0x00011E30
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.homeNation.alienNation, TIUtilities.GetBoolValue(this.strValue));
	}
}
