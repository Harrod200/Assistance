using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200003E RID: 62
public class TINationCondition_bHasMilitary : TINationCondition
{
	// Token: 0x06000223 RID: 547 RVA: 0x0001020D File Offset: 0x0000E40D
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.military, TIUtilities.GetBoolValue(this.strValue));
	}
}
