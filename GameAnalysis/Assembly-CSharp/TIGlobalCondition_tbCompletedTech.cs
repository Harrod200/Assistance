using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000C0 RID: 192
public class TIGlobalCondition_tbCompletedTech : TIGlobalCondition
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x0600038E RID: 910 RVA: 0x00013029 File Offset: 0x00011229
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TITechTemplate>(this.strValue).displayName };
		}
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00013047 File Offset: 0x00011247
	public override bool PassesCondition(TIGameState state = null)
	{
		return TICondition.PassesComparison<TITechTemplate>(this.sign, TIUtilities.GetTemplateValue<TITechTemplate>(this.strValue), TIGlobalResearchState.FinishedTechs());
	}
}
