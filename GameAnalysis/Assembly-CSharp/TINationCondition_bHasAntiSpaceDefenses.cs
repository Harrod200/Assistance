using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000068 RID: 104
public class TINationCondition_bHasAntiSpaceDefenses : TINationCondition
{
	// Token: 0x0600028D RID: 653 RVA: 0x00011071 File Offset: 0x0000F271
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00011079 File Offset: 0x0000F279
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.hasAntiSpaceDefenses > 0, TIUtilities.GetBoolValue(this.strValue));
	}
}
