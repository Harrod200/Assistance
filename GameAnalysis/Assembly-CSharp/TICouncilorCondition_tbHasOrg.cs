using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E4 RID: 228
public class TICouncilorCondition_tbHasOrg : TICouncilorCondition
{
	// Token: 0x1700007D RID: 125
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x00013EE1 File Offset: 0x000120E1
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIOrgTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x060003EB RID: 1003 RVA: 0x00013EFF File Offset: 0x000120FF
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.HasOrg(TIUtilities.GetTemplateValue<TIOrgTemplate>(this.strIdx)), TIUtilities.GetBoolValue(this.strValue));
	}
}
