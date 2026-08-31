using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000145 RID: 325
public class TIArmyCondition_fStrength : TIArmyCondition_Numeric
{
	// Token: 0x060004D9 RID: 1241 RVA: 0x00015CD0 File Offset: 0x00013ED0
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_army.strength, TIUtilities.GetFloatValue(this.strValue));
	}
}
