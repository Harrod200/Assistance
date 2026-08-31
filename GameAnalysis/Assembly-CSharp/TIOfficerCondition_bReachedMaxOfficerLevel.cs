using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000139 RID: 313
public class TIOfficerCondition_bReachedMaxOfficerLevel : TIOfficerCondition
{
	// Token: 0x060004B7 RID: 1207 RVA: 0x000159D3 File Offset: 0x00013BD3
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x060004B8 RID: 1208 RVA: 0x000159DB File Offset: 0x00013BDB
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_officer != null && TICondition.PassesComparison(this.sign, state.ref_officer.maxRank >= 3, TIUtilities.GetBoolValue(this.strValue));
	}
}
