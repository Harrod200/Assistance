using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200011D RID: 285
public class TIHabSiteCondition_tbMiningSiteProfile : TIHabSiteCondition
{
	// Token: 0x06000472 RID: 1138 RVA: 0x000151D2 File Offset: 0x000133D2
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000473 RID: 1139 RVA: 0x000151DA File Offset: 0x000133DA
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIMiningProfileTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x06000474 RID: 1140 RVA: 0x000151F8 File Offset: 0x000133F8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_habSite.miningProfile == TIUtilities.GetTemplateValue<TIMiningProfileTemplate>(this.strIdx), TIUtilities.GetBoolValue(this.strValue));
	}
}
