using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B4 RID: 180
public class TIFactionCondition_iAggregateStat : TIFactionCondition
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x0600036C RID: 876 RVA: 0x00012A9B File Offset: 0x00010C9B
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

	// Token: 0x0600036D RID: 877 RVA: 0x00012AC8 File Offset: 0x00010CC8
	public override bool PassesCondition(TIGameState state)
	{
		CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
		return state.ref_faction != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, state.ref_faction.GetAggregateStat(councilorAttribute, false, null), (float)TIUtilities.GetIntValue(this.strValue));
	}
}
