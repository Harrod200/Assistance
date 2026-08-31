using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200008A RID: 138
public class TIRegionCondition_iSecessionCandidates : TIRegionCondition
{
	// Token: 0x060002E9 RID: 745 RVA: 0x000119D4 File Offset: 0x0000FBD4
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.SecessionCandidates().Count<TINationState>(), TIUtilities.GetIntValue(this.strValue));
	}
}
