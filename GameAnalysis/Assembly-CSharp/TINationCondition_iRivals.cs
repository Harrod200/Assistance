using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200004C RID: 76
public class TINationCondition_iRivals : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000245 RID: 581 RVA: 0x00010689 File Offset: 0x0000E889
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.rivals.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
