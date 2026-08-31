using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000104 RID: 260
public class TIHabCondition_fAssaultCombatValue : TIHabCondition_Numeric
{
	// Token: 0x06000439 RID: 1081 RVA: 0x0001494E File Offset: 0x00012B4E
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.AssaultCombatValue(true), TIUtilities.GetFloatValue(this.strValue));
	}
}
