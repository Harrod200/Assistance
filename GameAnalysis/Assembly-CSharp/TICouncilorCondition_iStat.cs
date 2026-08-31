using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000EB RID: 235
public class TICouncilorCondition_iStat : TICouncilorCondition
{
	// Token: 0x1700007F RID: 127
	// (get) Token: 0x060003FA RID: 1018 RVA: 0x000140F7 File Offset: 0x000122F7
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TIUtilities.GetAttributeString(this.strIdx.ToEnum(CouncilorAttribute.None)),
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00014124 File Offset: 0x00012324
	public override bool PassesCondition(TIGameState state)
	{
		CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
		TICouncilorState ref_councilor = state.ref_councilor;
		return ref_councilor != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, ref_councilor.GetAttribute(councilorAttribute, true, true, true, false, false, false), TIUtilities.GetIntValue(this.strValue));
	}
}
