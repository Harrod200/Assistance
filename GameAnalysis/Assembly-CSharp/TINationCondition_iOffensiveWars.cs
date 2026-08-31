using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000064 RID: 100
public class TINationCondition_iOffensiveWars : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000282 RID: 642 RVA: 0x00010F2B File Offset: 0x0000F12B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.offensiveWarsImLeading.Count<TIWarState>(), TIUtilities.GetIntValue(this.strValue));
	}
}
