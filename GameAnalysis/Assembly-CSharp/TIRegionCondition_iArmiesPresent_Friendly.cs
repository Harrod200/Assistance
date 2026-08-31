using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200008C RID: 140
public class TIRegionCondition_iArmiesPresent_Friendly : TIRegionCondition
{
	// Token: 0x060002ED RID: 749 RVA: 0x00011A53 File Offset: 0x0000FC53
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.NumArmiesPresent(true, true, false, false), TIUtilities.GetIntValue(this.strValue));
	}
}
