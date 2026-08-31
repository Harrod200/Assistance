using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000132 RID: 306
public class TISpaceShipCondition_iHullMaxOfficers : TISpaceShipCondition_Numeric
{
	// Token: 0x060004A7 RID: 1191 RVA: 0x00015860 File Offset: 0x00013A60
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.hull.maxOfficers, TIUtilities.GetIntValue(this.strValue));
	}
}
