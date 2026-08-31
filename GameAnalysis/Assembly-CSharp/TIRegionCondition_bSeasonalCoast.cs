using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000072 RID: 114
public class TIRegionCondition_bSeasonalCoast : TIRegionCondition
{
	// Token: 0x060002A8 RID: 680 RVA: 0x000112C8 File Offset: 0x0000F4C8
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x000112D0 File Offset: 0x0000F4D0
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.oceanType == WorldOceanType.Seasonal, TIUtilities.GetBoolValue(this.strValue));
	}
}
