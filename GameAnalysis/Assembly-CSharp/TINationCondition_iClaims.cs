using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200004F RID: 79
public class TINationCondition_iClaims : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600024B RID: 587 RVA: 0x00010749 File Offset: 0x0000E949
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.claims.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
