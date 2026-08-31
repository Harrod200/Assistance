using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000085 RID: 133
public class TIRegionCondition_bBorderRegion : TIRegionCondition
{
	// Token: 0x060002DF RID: 735 RVA: 0x000118A6 File Offset: 0x0000FAA6
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.BorderWithAnotherNation(false), TIUtilities.GetBoolValue(this.strValue));
	}
}
