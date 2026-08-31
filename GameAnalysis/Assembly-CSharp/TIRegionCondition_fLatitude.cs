using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000076 RID: 118
public class TIRegionCondition_fLatitude : TIRegionCondition_Numeric_NoSymbol
{
	// Token: 0x060002B5 RID: 693 RVA: 0x00011434 File Offset: 0x0000F634
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.latitude, TIUtilities.GetFloatValue(this.strValue));
	}
}
