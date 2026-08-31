using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000DE RID: 222
public class TICouncilorCondition_tbHomeNation : TICouncilorCondition
{
	// Token: 0x17000078 RID: 120
	// (get) Token: 0x060003D5 RID: 981 RVA: 0x00013BB7 File Offset: 0x00011DB7
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TINationTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x00013BD8 File Offset: 0x00011DD8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison(this.sign, state.ref_councilor.homeNation.template == TIUtilities.GetTemplateValue<TINationTemplate>(this.strIdx), TIUtilities.GetBoolValue(this.strValue));
	}
}
