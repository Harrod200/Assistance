using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E7 RID: 231
public class TICouncilorCondition_bInSpace : TICouncilorCondition
{
	// Token: 0x060003F2 RID: 1010 RVA: 0x00013FD8 File Offset: 0x000121D8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.ref_hab != null || state.ref_councilor.ref_fleet != null, TIUtilities.GetBoolValue(this.strValue));
	}
}
