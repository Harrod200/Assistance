using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000071 RID: 113
public class TIRegionCondition_bOnTheWater : TIRegionCondition
{
	// Token: 0x060002A5 RID: 677 RVA: 0x00011285 File Offset: 0x0000F485
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x0001128D File Offset: 0x0000F48D
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.onTheWater, TIUtilities.GetBoolValue(this.strValue));
	}
}
