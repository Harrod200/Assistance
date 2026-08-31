using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000098 RID: 152
public class TIFactionCondition_efResourceIncomeMonthly : TIFactionCondition
{
	// Token: 0x17000057 RID: 87
	// (get) Token: 0x06000312 RID: 786 RVA: 0x00011F6E File Offset: 0x0001016E
	public override string symbolResource
	{
		get
		{
			return TIUtilities.InlineResourceStr(this.strIdx.ToEnum(FactionResource.None));
		}
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x06000313 RID: 787 RVA: 0x00011F81 File Offset: 0x00010181
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

	// Token: 0x06000314 RID: 788 RVA: 0x00011FA4 File Offset: 0x000101A4
	public override bool PassesCondition(TIGameState state)
	{
		FactionResource factionResource = this.strIdx.ToEnum(FactionResource.None);
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.GetMonthlyIncome(factionResource, false, false), TIUtilities.GetFloatValue(this.strValue));
	}
}
