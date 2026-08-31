using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200008B RID: 139
public class TIRegionCondition_iArmiesPresent : TIRegionCondition
{
	// Token: 0x060002EB RID: 747 RVA: 0x00011A14 File Offset: 0x0000FC14
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.NumArmiesPresent(true, true, true, false), TIUtilities.GetIntValue(this.strValue));
	}
}
