using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000AB RID: 171
public class TIFactionCondition_iCouncilorStat : TIFactionCondition
{
	// Token: 0x17000068 RID: 104
	// (get) Token: 0x06000350 RID: 848 RVA: 0x00012766 File Offset: 0x00010966
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

	// Token: 0x06000351 RID: 849 RVA: 0x00012794 File Offset: 0x00010994
	public override bool PassesCondition(TIGameState state)
	{
		CouncilorAttribute councilorAttribute = this.strIdx.ToEnum(CouncilorAttribute.None);
		return state.ref_faction != null && councilorAttribute != CouncilorAttribute.None && TICondition.PassesComparison(this.sign, state.ref_faction.GetMaxCouncilorStat(councilorAttribute, false, null), (float)TIUtilities.GetIntValue(this.strValue));
	}
}
