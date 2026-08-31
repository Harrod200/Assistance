using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000137 RID: 311
public class TIOfficerCondition_iMaxRank : TIOfficerCondition_Numeric
{
	// Token: 0x060004B2 RID: 1202 RVA: 0x0001594F File Offset: 0x00013B4F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, state.ref_officer.maxRank, TIUtilities.GetIntValue(this.strValue));
	}
}
