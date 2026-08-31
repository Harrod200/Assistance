using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000045 RID: 69
public class TINationCondition_iControlPoints : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000237 RID: 567 RVA: 0x0001046E File Offset: 0x0000E66E
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.numControlPoints, TIUtilities.GetIntValue(this.strValue));
	}
}
