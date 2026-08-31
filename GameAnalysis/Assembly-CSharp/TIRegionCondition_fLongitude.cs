using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000078 RID: 120
public class TIRegionCondition_fLongitude : TIRegionCondition_Numeric_NoSymbol
{
	// Token: 0x060002B9 RID: 697 RVA: 0x000114AF File Offset: 0x0000F6AF
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.longitude, TIUtilities.GetFloatValue(this.strValue));
	}
}
