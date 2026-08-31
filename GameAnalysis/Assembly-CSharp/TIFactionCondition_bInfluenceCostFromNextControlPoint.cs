using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000AD RID: 173
public class TIFactionCondition_bInfluenceCostFromNextControlPoint : TIFactionCondition
{
	// Token: 0x1700006A RID: 106
	// (get) Token: 0x06000357 RID: 855 RVA: 0x000128A5 File Offset: 0x00010AA5
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000358 RID: 856 RVA: 0x000128BA File Offset: 0x00010ABA
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.GetAnnualControlPointMaintenanceCost() > 0f, TIUtilities.GetBoolValue(this.strValue));
	}
}
