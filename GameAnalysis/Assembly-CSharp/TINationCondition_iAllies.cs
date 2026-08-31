using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200004A RID: 74
public class TINationCondition_iAllies : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000241 RID: 577 RVA: 0x000105E9 File Offset: 0x0000E7E9
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.allies.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
