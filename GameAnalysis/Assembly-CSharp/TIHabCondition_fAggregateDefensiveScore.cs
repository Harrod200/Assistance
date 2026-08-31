using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000103 RID: 259
public class TIHabCondition_fAggregateDefensiveScore : TIHabCondition_Numeric
{
	// Token: 0x06000437 RID: 1079 RVA: 0x00014913 File Offset: 0x00012B13
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.AggregateDefensiveScore_Station(), TIUtilities.GetFloatValue(this.strValue));
	}
}
