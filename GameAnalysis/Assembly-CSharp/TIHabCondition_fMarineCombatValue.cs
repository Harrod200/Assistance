using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000107 RID: 263
public class TIHabCondition_fMarineCombatValue : TIHabCondition_Numeric
{
	// Token: 0x0600043F RID: 1087 RVA: 0x00014A26 File Offset: 0x00012C26
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.MarineModuleCombatValue(), TIUtilities.GetFloatValue(this.strValue));
	}
}
