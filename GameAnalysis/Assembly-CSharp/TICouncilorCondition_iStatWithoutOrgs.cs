using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000EC RID: 236
public class TICouncilorCondition_iStatWithoutOrgs : TICouncilorCondition
{
	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060003FD RID: 1021 RVA: 0x0001417E File Offset: 0x0001237E
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

	// Token: 0x060003FE RID: 1022 RVA: 0x000141AC File Offset: 0x000123AC
	public override bool PassesCondition(TIGameState state)
	{
		CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
		TICouncilorState ref_councilor = state.ref_councilor;
		return ref_councilor != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, ref_councilor.GetAttribute(councilorAttribute, false, true, true, false, false, false), TIUtilities.GetIntValue(this.strValue));
	}
}
