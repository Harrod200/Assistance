using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000088 RID: 136
public class TIRegionCondition_bIsOccupied : TIRegionCondition
{
	// Token: 0x060002E5 RID: 741 RVA: 0x00011959 File Offset: 0x0000FB59
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.IsFullyOccupied(), TIUtilities.GetBoolValue(this.strValue));
	}
}
