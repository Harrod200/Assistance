using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000130 RID: 304
public class TISpaceShipCondition_fLaserBonusPower_MJ : TISpaceShipCondition_Numeric
{
	// Token: 0x060004A2 RID: 1186 RVA: 0x000157A5 File Offset: 0x000139A5
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_ship != null && TICondition.PassesComparison(this.sign, state.ref_ship.GetLaserBonusPower_MJ(), TIUtilities.GetFloatValue(this.strValue));
	}
}
