using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000058 RID: 88
public class TINationCondition_iNativeControlPoints : TINationCondition
{
	// Token: 0x0600025F RID: 607 RVA: 0x00010A8F File Offset: 0x0000EC8F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_nation != null && TICondition.PassesComparison(this.sign, state.ref_nation.NumNativeControlPoints, TIUtilities.GetIntValue(this.strValue));
	}
}
