using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000089 RID: 137
public class TIRegionCondition_iOccupations : TIRegionCondition
{
	// Token: 0x060002E7 RID: 743 RVA: 0x00011994 File Offset: 0x0000FB94
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.occupations.Count<KeyValuePair<TINationState, float>>(), TIUtilities.GetIntValue(this.strValue));
	}
}
