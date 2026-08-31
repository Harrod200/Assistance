using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000136 RID: 310
public class TIOfficerCondition_iRank : TIOfficerCondition_Numeric
{
	// Token: 0x060004B0 RID: 1200 RVA: 0x00015914 File Offset: 0x00013B14
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, state.ref_officer.rank, TIUtilities.GetIntValue(this.strValue));
	}
}
