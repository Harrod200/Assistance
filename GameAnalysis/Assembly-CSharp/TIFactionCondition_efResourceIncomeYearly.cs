using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000099 RID: 153
public class TIFactionCondition_efResourceIncomeYearly : TIFactionCondition
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x06000316 RID: 790 RVA: 0x00011FFA File Offset: 0x000101FA
	public override string symbolResource
	{
		get
		{
			return TIUtilities.InlineResourceStr(this.strIdx.ToEnum(FactionResource.None));
		}
	}

	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000317 RID: 791 RVA: 0x0001200D File Offset: 0x0001020D
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				this.symbolResource,
				base.GetNumericComparisonString(false)
			};
		}
	}

	// Token: 0x06000318 RID: 792 RVA: 0x00012030 File Offset: 0x00010230
	public override bool PassesCondition(TIGameState state)
	{
		FactionResource factionResource = this.strIdx.ToEnum(FactionResource.None);
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.GetYearlyIncome(factionResource, false, false, false), TIUtilities.GetFloatValue(this.strValue));
	}
}
