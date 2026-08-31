using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000087 RID: 135
public class TIRegionCondition_bIsBeingOccupied : TIRegionCondition
{
	// Token: 0x060002E3 RID: 739 RVA: 0x0001191E File Offset: 0x0000FB1E
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.OccupiedOrOccupationUnderway(), TIUtilities.GetBoolValue(this.strValue));
	}
}
