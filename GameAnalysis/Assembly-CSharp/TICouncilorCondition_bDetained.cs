using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000EA RID: 234
public class TICouncilorCondition_bDetained : TICouncilorCondition
{
	// Token: 0x060003F8 RID: 1016 RVA: 0x000140BC File Offset: 0x000122BC
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.detained, TIUtilities.GetBoolValue(this.strValue));
	}
}
