using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000112 RID: 274
public class TIHabCondition_fConstructionTimeModifier : TIHabCondition_Numeric
{
	// Token: 0x06000456 RID: 1110 RVA: 0x00014DF8 File Offset: 0x00012FF8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.GetModuleConstructionTimeModifier(true, null), TIUtilities.GetFloatValue(this.strValue));
	}
}
