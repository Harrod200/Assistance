using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E6 RID: 230
public class TICouncilorCondition_bOnEarth : TICouncilorCondition
{
	// Token: 0x060003F0 RID: 1008 RVA: 0x00013F9A File Offset: 0x0001219A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.OnEarth, TIUtilities.GetBoolValue(this.strValue));
	}
}
