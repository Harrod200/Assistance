using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000097 RID: 151
public class TIFactionCondition_efResourceIncomeDaily : TIFactionCondition
{
	// Token: 0x17000055 RID: 85
	// (get) Token: 0x0600030E RID: 782 RVA: 0x00011EE4 File Offset: 0x000100E4
	public override string symbolResource
	{
		get
		{
			return TIUtilities.InlineResourceStr(this.strIdx.ToEnum(FactionResource.None));
		}
	}

	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600030F RID: 783 RVA: 0x00011EF7 File Offset: 0x000100F7
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

	// Token: 0x06000310 RID: 784 RVA: 0x00011F18 File Offset: 0x00010118
	public override bool PassesCondition(TIGameState state)
	{
		FactionResource factionResource = this.strIdx.ToEnum(FactionResource.None);
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.GetDailyIncome(factionResource, false, false), TIUtilities.GetFloatValue(this.strValue));
	}
}
