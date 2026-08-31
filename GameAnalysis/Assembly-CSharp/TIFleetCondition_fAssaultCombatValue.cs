using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000126 RID: 294
public class TIFleetCondition_fAssaultCombatValue : TIFleetCondition_Numeric
{
	// Token: 0x06000486 RID: 1158 RVA: 0x00015447 File Offset: 0x00013647
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_fleet != null && TICondition.PassesComparison(this.sign, state.ref_fleet.AssaultCombatValue(true), TIUtilities.GetFloatValue(this.strValue));
	}
}
