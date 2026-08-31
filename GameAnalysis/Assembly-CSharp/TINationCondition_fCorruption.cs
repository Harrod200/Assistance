using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000062 RID: 98
public class TINationCondition_fCorruption : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600027D RID: 637 RVA: 0x00010E85 File Offset: 0x0000F085
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.corruption, TIUtilities.GetFloatValue(this.strValue));
	}
}
