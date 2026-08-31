using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000082 RID: 130
public class TIRegionCondition_iAbductions : TIRegionCondition
{
	// Token: 0x060002D8 RID: 728 RVA: 0x000117B7 File Offset: 0x0000F9B7
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.abductions, TIUtilities.GetIntValue(this.strValue));
	}
}
