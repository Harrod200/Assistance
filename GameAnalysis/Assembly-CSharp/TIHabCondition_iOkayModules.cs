using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200010D RID: 269
public class TIHabCondition_iOkayModules : TIHabCondition_Numeric
{
	// Token: 0x0600044B RID: 1099 RVA: 0x00014BB4 File Offset: 0x00012DB4
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.OkayModules().Count<TIHabModuleState>(), TIUtilities.GetIntValue(this.strValue));
	}
}
