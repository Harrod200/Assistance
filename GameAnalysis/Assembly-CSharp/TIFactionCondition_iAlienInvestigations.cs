using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B3 RID: 179
public class TIFactionCondition_iAlienInvestigations : TIFactionCondition
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x06000369 RID: 873 RVA: 0x00012A4B File Offset: 0x00010C4B
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600036A RID: 874 RVA: 0x00012A60 File Offset: 0x00010C60
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.alienInvestigations, TIUtilities.GetIntValue(this.strValue));
	}
}
