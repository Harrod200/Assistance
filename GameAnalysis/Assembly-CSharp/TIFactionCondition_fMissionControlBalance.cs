using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200009C RID: 156
public class TIFactionCondition_fMissionControlBalance : TIFactionCondition
{
	// Token: 0x1700005C RID: 92
	// (get) Token: 0x06000320 RID: 800 RVA: 0x0001211B File Offset: 0x0001031B
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { base.GetNumericComparisonString(false) };
		}
	}

	// Token: 0x06000321 RID: 801 RVA: 0x00012130 File Offset: 0x00010330
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, (float)state.ref_faction.MissionControlBalance, TIUtilities.GetFloatValue(this.strValue));
	}
}
