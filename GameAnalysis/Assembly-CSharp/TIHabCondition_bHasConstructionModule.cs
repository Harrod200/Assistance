using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000113 RID: 275
public class TIHabCondition_bHasConstructionModule : TIHabCondition
{
	// Token: 0x06000458 RID: 1112 RVA: 0x00014E35 File Offset: 0x00013035
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.GetModuleConstructionTimeModifier(true, null) < 1f, TIUtilities.GetBoolValue(this.strValue));
	}
}
