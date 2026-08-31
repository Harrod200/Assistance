using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000ED RID: 237
public class TICouncilorCondition_iSpareOrgCapacity : TICouncilorCondition
{
	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000400 RID: 1024 RVA: 0x00014206 File Offset: 0x00012406
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x0001421C File Offset: 0x0001241C
	public override bool PassesCondition(TIGameState state)
	{
		TICouncilorState ref_councilor = state.ref_councilor;
		return ref_councilor != null && TICondition.PassesComparison(this.sign, ref_councilor.SpareCapacityForOrgs(), TIUtilities.GetIntValue(this.strValue));
	}
}
