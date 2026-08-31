using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200009B RID: 155
public class TIFactionCondition_fAvailableMissionControl : TIFactionCondition
{
	// Token: 0x1700005B RID: 91
	// (get) Token: 0x0600031D RID: 797 RVA: 0x000120CA File Offset: 0x000102CA
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x0600031E RID: 798 RVA: 0x000120DF File Offset: 0x000102DF
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, (float)state.ref_faction.AvailableMissionControl, TIUtilities.GetFloatValue(this.strValue));
	}
}
