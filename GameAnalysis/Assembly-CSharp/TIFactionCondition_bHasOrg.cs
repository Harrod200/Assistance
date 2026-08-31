using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000AA RID: 170
public class TIFactionCondition_bHasOrg : TIFactionCondition
{
	// Token: 0x17000067 RID: 103
	// (get) Token: 0x0600034D RID: 845 RVA: 0x00012701 File Offset: 0x00010901
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIOrgTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0001271F File Offset: 0x0001091F
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.CouncilHasOrg(TIUtilities.GetTemplateValue<TIOrgTemplate>(this.strIdx), false), TIUtilities.GetBoolValue(this.strValue));
	}
}
