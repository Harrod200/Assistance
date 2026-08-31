using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000086 RID: 134
public class TIRegionCondition_bBorderRegionWithEnemy : TIRegionCondition
{
	// Token: 0x060002E1 RID: 737 RVA: 0x000118E2 File Offset: 0x0000FAE2
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_region != null && TICondition.PassesComparison(this.sign, state.ref_region.BorderWithAnotherNation(true), TIUtilities.GetBoolValue(this.strValue));
	}
}
