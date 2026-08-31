using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200012A RID: 298
public class TISpaceShipCondition_fAssaultCombatValue : TISpaceShipCondition_Numeric
{
	// Token: 0x0600048F RID: 1167 RVA: 0x0001550F File Offset: 0x0001370F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.AssaultCombatValue(true), TIUtilities.GetFloatValue(this.strValue));
	}
}
