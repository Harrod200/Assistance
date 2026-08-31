using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E5 RID: 229
public class TICouncilorCondition_iNumLoseableOrgs : TICouncilorCondition
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060003ED RID: 1005 RVA: 0x00013F45 File Offset: 0x00012145
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x060003EE RID: 1006 RVA: 0x00013F5A File Offset: 0x0001215A
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.GetLoseableOrgs().Count, TIUtilities.GetIntValue(this.strValue));
	}
}
