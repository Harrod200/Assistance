using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200005A RID: 90
public class TINationCondition_bIsAlienNation : TINationCondition
{
	// Token: 0x06000264 RID: 612 RVA: 0x00010B13 File Offset: 0x0000ED13
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000265 RID: 613 RVA: 0x00010B1B File Offset: 0x0000ED1B
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.alienNation, TIUtilities.GetBoolValue(this.strValue));
	}
}
