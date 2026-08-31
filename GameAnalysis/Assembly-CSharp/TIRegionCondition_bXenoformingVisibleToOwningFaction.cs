using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000081 RID: 129
public class TIRegionCondition_bXenoformingVisibleToOwningFaction : TIRegionCondition
{
	// Token: 0x060002D6 RID: 726 RVA: 0x0001175C File Offset: 0x0000F95C
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.xenoforming.VisibleToFaction(state.ref_region.nation.executiveFaction), TIUtilities.GetBoolValue(this.strValue));
	}
}
