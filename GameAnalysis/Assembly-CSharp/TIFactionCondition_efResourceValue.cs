using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000096 RID: 150
public class TIFactionCondition_efResourceValue : TIFactionCondition
{
	// Token: 0x17000053 RID: 83
	// (get) Token: 0x0600030A RID: 778 RVA: 0x00011E5C File Offset: 0x0001005C
	public override string symbolResource
	{
		get
		{
			return TIUtilities.InlineResourceStr(this.strIdx.ToEnum(FactionResource.None));
		}
	}

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x0600030B RID: 779 RVA: 0x00011E6F File Offset: 0x0001006F
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

	// Token: 0x0600030C RID: 780 RVA: 0x00011E90 File Offset: 0x00010090
	public override bool PassesCondition(TIGameState state)
	{
		FactionResource factionResource = this.strIdx.ToEnum(FactionResource.None);
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.GetCurrentResourceAmount(factionResource), TIUtilities.GetFloatValue(this.strValue));
	}
}
