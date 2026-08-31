using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200005D RID: 93
public class TINationCondition_iExecFactionUndefendedCPs : TINationCondition
{
	// Token: 0x0600026D RID: 621 RVA: 0x00010C4C File Offset: 0x0000EE4C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.CountFactionControlPoints(state.ref_nation.executiveFaction, true, false, false), TIUtilities.GetIntValue(this.strValue));
	}
}
