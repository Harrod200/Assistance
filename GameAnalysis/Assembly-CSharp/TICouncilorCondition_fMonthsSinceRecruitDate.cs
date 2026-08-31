using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000F2 RID: 242
public class TICouncilorCondition_fMonthsSinceRecruitDate : TIFactionCondition
{
	// Token: 0x17000087 RID: 135
	// (get) Token: 0x06000410 RID: 1040 RVA: 0x00014462 File Offset: 0x00012662
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00014477 File Offset: 0x00012677
	public override bool PassesCondition(TIGameState state)
	{
		return TICondition.PassesComparison(this.sign, state.ref_councilor.MonthsSinceRecruitDate(), TIUtilities.GetFloatValue(this.strValue));
	}
}
