using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000083 RID: 131
public class TIRegionCondition_bAlienNearby : TIRegionCondition
{
	// Token: 0x060002DA RID: 730 RVA: 0x000117F4 File Offset: 0x0000F9F4
	public override bool PassesCondition(TIGameState state)
	{
		if (state.ref_region != null)
		{
			return TICondition.PassesComparison(this.sign, state.ref_region.ThisAndAdjacentRegions(false).Any<TIRegionState>((TIRegionState x) => x.GetCouncilorsInRegion().Any<TICouncilorState>((TICouncilorState x) => x.isAlien)), TIUtilities.GetBoolValue(this.strValue));
		}
		return false;
	}
}
