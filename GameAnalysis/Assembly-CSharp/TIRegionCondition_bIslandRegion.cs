using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000073 RID: 115
public class TIRegionCondition_bIslandRegion : TIRegionCondition
{
	// Token: 0x060002AB RID: 683 RVA: 0x0001130E File Offset: 0x0000F50E
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060002AC RID: 684 RVA: 0x00011316 File Offset: 0x0000F516
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.isIsland, TIUtilities.GetBoolValue(this.strValue));
	}
}
