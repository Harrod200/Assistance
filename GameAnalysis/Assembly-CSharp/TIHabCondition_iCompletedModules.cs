using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200010E RID: 270
public class TIHabCondition_iCompletedModules : TIHabCondition_Numeric
{
	// Token: 0x0600044D RID: 1101 RVA: 0x00014BF4 File Offset: 0x00012DF4
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.CompletedModules().Count<TIHabModuleState>(), TIUtilities.GetIntValue(this.strValue));
	}
}
