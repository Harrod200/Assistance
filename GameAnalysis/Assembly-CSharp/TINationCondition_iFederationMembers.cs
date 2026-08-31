using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200004B RID: 75
public class TINationCondition_iFederationMembers : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000243 RID: 579 RVA: 0x0001062C File Offset: 0x0000E82C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && state.ref_nation.inFederation && TICondition.PassesComparison(this.sign, state.ref_nation.federation.members.Count, TIUtilities.GetIntValue(this.strValue));
	}
}
