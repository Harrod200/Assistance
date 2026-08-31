using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000105 RID: 261
public class TIHabCondition_fAssaultCombatValueFromDockedFleets : TIHabCondition_Numeric
{
	// Token: 0x0600043B RID: 1083 RVA: 0x0001498A File Offset: 0x00012B8A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.AssaultCombatValueFromDockedFleets(state.ref_faction, true), TIUtilities.GetFloatValue(this.strValue));
	}
}
