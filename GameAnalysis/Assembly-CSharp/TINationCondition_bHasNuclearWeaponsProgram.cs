using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000053 RID: 83
public class TINationCondition_bHasNuclearWeaponsProgram : TINationCondition
{
	// Token: 0x06000253 RID: 595 RVA: 0x00010874 File Offset: 0x0000EA74
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.nuclearProgram, TIUtilities.GetBoolValue(this.strValue));
	}
}
