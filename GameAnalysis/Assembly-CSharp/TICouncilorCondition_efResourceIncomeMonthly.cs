using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000EE RID: 238
public class TICouncilorCondition_efResourceIncomeMonthly : TICouncilorCondition
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06000403 RID: 1027 RVA: 0x0001425F File Offset: 0x0001245F
	public override string symbolResource
	{
		get
		{
			return TIUtilities.InlineResourceStr(this.strIdx.ToEnum(FactionResource.None));
		}
	}

	// Token: 0x17000083 RID: 131
	// (get) Token: 0x06000404 RID: 1028 RVA: 0x00014272 File Offset: 0x00012472
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

	// Token: 0x06000405 RID: 1029 RVA: 0x00014294 File Offset: 0x00012494
	public override bool PassesCondition(TIGameState state)
	{
		FactionResource factionResource = this.strIdx.ToEnum(FactionResource.None);
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.GetMonthlyIncome(factionResource), TIUtilities.GetFloatValue(this.strValue));
	}
}
