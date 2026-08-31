using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000101 RID: 257
public class TIHabCondition_fSpaceCombatValue : TIHabCondition_Numeric
{
	// Token: 0x06000433 RID: 1075 RVA: 0x0001489D File Offset: 0x00012A9D
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.SpaceCombatValue(), TIUtilities.GetFloatValue(this.strValue));
	}
}
