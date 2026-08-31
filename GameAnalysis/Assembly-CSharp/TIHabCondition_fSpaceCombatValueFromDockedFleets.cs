using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000102 RID: 258
public class TIHabCondition_fSpaceCombatValueFromDockedFleets : TIHabCondition_Numeric
{
	// Token: 0x06000435 RID: 1077 RVA: 0x000148D8 File Offset: 0x00012AD8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.SpaceCombatValueFromDockedFleets(), TIUtilities.GetFloatValue(this.strValue));
	}
}
