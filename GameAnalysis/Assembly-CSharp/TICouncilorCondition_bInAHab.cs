using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E9 RID: 233
public class TICouncilorCondition_bInAHab : TICouncilorCondition
{
	// Token: 0x060003F6 RID: 1014 RVA: 0x0001407B File Offset: 0x0001227B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.ref_hab != null, TIUtilities.GetBoolValue(this.strValue));
	}
}
