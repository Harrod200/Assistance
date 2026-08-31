using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000108 RID: 264
public class TIHabCondition_fMarineCombatValueCombined : TIHabCondition_Numeric
{
	// Token: 0x06000441 RID: 1089 RVA: 0x00014A64 File Offset: 0x00012C64
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.MarineModuleCombatValue() + state.ref_hab.AssaultCombatValueFromDockedFleets(state.ref_faction, true), TIUtilities.GetFloatValue(this.strValue));
	}
}
