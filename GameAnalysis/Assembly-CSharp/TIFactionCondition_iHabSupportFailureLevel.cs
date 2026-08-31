using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000B7 RID: 183
public class TIFactionCondition_iHabSupportFailureLevel : TIFactionCondition
{
	// Token: 0x1700006E RID: 110
	// (get) Token: 0x06000374 RID: 884 RVA: 0x00012C1C File Offset: 0x00010E1C
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00012C34 File Offset: 0x00010E34
	public override bool PassesCondition(TIGameState state)
	{
		int num = 0;
		if (state.ref_faction != null)
		{
			if (state.ref_faction.InsufficientBoostToSupportHabs())
			{
				num++;
			}
			if (state.ref_faction.Insolvent)
			{
				num++;
			}
			if (state.ref_faction.MissionControlShortage > 0)
			{
				num++;
			}
		}
		return TICondition.PassesComparison(this.sign, num, TIUtilities.GetIntValue(this.strValue));
	}
}
