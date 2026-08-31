using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E8 RID: 232
public class TICouncilorCondition_bOnAShip : TICouncilorCondition
{
	// Token: 0x060003F4 RID: 1012 RVA: 0x0001403A File Offset: 0x0001223A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.ref_fleet != null, TIUtilities.GetBoolValue(this.strValue));
	}
}
