using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000065 RID: 101
public class TINationCondition_iDefensiveWars : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000284 RID: 644 RVA: 0x00010F6B File Offset: 0x0000F16B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.defensiveWarsImLeading.Count<TIWarState>(), TIUtilities.GetIntValue(this.strValue));
	}
}
