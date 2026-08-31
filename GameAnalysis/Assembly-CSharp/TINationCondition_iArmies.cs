using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000047 RID: 71
public class TINationCondition_iArmies : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600023B RID: 571 RVA: 0x00010507 File Offset: 0x0000E707
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.numStandardArmies, TIUtilities.GetIntValue(this.strValue));
	}
}
