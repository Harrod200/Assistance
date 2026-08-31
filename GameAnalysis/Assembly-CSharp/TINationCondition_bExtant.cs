using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000066 RID: 102
public class TINationCondition_bExtant : TINationCondition
{
	// Token: 0x06000286 RID: 646 RVA: 0x00010FAB File Offset: 0x0000F1AB
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x06000287 RID: 647 RVA: 0x00010FB3 File Offset: 0x0000F1B3
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.extant, TIUtilities.GetBoolValue(this.strValue));
	}
}
