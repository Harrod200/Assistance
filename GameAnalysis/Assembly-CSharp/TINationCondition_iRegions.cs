using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200004E RID: 78
public class TINationCondition_iRegions : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000249 RID: 585 RVA: 0x00010709 File Offset: 0x0000E909
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.regions.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
