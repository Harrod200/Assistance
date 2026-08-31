using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000070 RID: 112
public class TIRegionCondition_bCoastal : TIRegionCondition
{
	// Token: 0x060002A2 RID: 674 RVA: 0x00011242 File Offset: 0x0000F442
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x0001124A File Offset: 0x0000F44A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.isCoastal, TIUtilities.GetBoolValue(this.strValue));
	}
}
