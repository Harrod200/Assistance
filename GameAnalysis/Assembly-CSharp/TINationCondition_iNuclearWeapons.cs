using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000051 RID: 81
public class TINationCondition_iNuclearWeapons : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x0600024F RID: 591 RVA: 0x000107FE File Offset: 0x0000E9FE
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.numNuclearWeapons, TIUtilities.GetIntValue(this.strValue));
	}
}
