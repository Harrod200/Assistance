using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000106 RID: 262
public class TIHabCondition_fAssaultCombatValueCombined : TIHabCondition_Numeric
{
	// Token: 0x0600043D RID: 1085 RVA: 0x000149CC File Offset: 0x00012BCC
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.AssaultCombatValue(true) + state.ref_hab.AssaultCombatValueFromDockedFleets(state.ref_faction, true), TIUtilities.GetFloatValue(this.strValue));
	}
}
