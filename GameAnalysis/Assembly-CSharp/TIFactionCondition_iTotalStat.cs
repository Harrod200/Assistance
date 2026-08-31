using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B5 RID: 181
public class TIFactionCondition_iTotalStat : TIFactionCondition
{
	// Token: 0x1700006D RID: 109
	// (get) Token: 0x0600036F RID: 879 RVA: 0x00012B22 File Offset: 0x00010D22
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(3)
			{
				TIUtilities.GetAttributeString(this.strIdx.ToEnum(CouncilorAttribute.None)),
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x00012B50 File Offset: 0x00010D50
	public override bool PassesCondition(TIGameState state)
	{
		CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
		return state.ref_faction != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, state.ref_faction.GetTotalStat(councilorAttribute, false, null), TIUtilities.GetIntValue(this.strValue));
	}
}
