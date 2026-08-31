using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000080 RID: 128
public class TIRegionCondition_fXenoforming : TIRegionCondition
{
	// Token: 0x060002D4 RID: 724 RVA: 0x00011719 File Offset: 0x0000F919
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.xenoforming.xenoformingLevel, TIUtilities.GetFloatValue(this.strValue));
	}
}
