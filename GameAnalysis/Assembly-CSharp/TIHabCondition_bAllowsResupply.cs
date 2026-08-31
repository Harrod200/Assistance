using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200010A RID: 266
public class TIHabCondition_bAllowsResupply : TIHabCondition
{
	// Token: 0x06000445 RID: 1093 RVA: 0x00014AFB File Offset: 0x00012CFB
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.AllowsResupply(state.ref_faction, true, true), TIUtilities.GetBoolValue(this.strValue));
	}
}
