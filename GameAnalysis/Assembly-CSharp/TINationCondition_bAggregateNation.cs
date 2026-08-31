using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200005C RID: 92
public class TINationCondition_bAggregateNation : TINationCondition
{
	// Token: 0x0600026A RID: 618 RVA: 0x00010C09 File Offset: 0x0000EE09
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00010C11 File Offset: 0x0000EE11
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.aggregateNation, TIUtilities.GetBoolValue(this.strValue));
	}
}
