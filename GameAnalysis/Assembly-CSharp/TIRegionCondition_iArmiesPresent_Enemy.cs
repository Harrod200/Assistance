using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200008D RID: 141
public class TIRegionCondition_iArmiesPresent_Enemy : TIRegionCondition
{
	// Token: 0x060002EF RID: 751 RVA: 0x00011A92 File Offset: 0x0000FC92
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.NumArmiesPresent(false, false, true, false), TIUtilities.GetIntValue(this.strValue));
	}
}
