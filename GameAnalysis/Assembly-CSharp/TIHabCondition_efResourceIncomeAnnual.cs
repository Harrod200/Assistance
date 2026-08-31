using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000114 RID: 276
public class TIHabCondition_efResourceIncomeAnnual : TIHabCondition_Numeric
{
	// Token: 0x1700008C RID: 140
	// (get) Token: 0x0600045A RID: 1114 RVA: 0x00014E79 File Offset: 0x00013079
	public override string symbolResource
	{
		get
		{
			return TIUtilities.InlineResourceStr(this.strIdx.ToEnum(FactionResource.None));
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x0600045B RID: 1115 RVA: 0x00014E8C File Offset: 0x0001308C
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

	// Token: 0x0600045C RID: 1116 RVA: 0x00014EB0 File Offset: 0x000130B0
	public override bool PassesCondition(TIGameState state)
	{
		FactionResource factionResource = this.strIdx.ToEnum(FactionResource.None);
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.GetAnnualNetResourceIncome(state.ref_faction, factionResource), TIUtilities.GetFloatValue(this.strValue));
	}
}
