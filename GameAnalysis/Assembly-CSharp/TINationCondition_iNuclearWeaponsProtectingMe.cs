using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000052 RID: 82
public class TINationCondition_iNuclearWeaponsProtectingMe : TINationCondition_Numeric_NoSymbol
{
	// Token: 0x06000251 RID: 593 RVA: 0x00010839 File Offset: 0x0000EA39
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.NumNuclearWeaponsDefendingMe(), TIUtilities.GetIntValue(this.strValue));
	}
}
