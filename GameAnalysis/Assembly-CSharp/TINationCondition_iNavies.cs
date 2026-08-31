using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000049 RID: 73
public class TINationCondition_iNavies : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600023F RID: 575 RVA: 0x000105AE File Offset: 0x0000E7AE
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.numNavies, TIUtilities.GetIntValue(this.strValue));
	}
}
