using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200006C RID: 108
public class TINationCondition_bPolicyClosedBorders : TINationCondition
{
	// Token: 0x06000299 RID: 665 RVA: 0x000111AE File Offset: 0x0000F3AE
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x0600029A RID: 666 RVA: 0x000111B6 File Offset: 0x0000F3B6
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.policy_closedBorders, TIUtilities.GetBoolValue(this.strValue));
	}
}
