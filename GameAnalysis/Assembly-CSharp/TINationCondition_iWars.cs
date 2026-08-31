using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200004D RID: 77
public class TINationCondition_iWars : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000247 RID: 583 RVA: 0x000106C9 File Offset: 0x0000E8C9
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.wars.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
