using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000FB RID: 251
public class TIHabCondition_tbHasModuleOrUpgradePrereqFunctioning : TIHabCondition
{
	// Token: 0x06000425 RID: 1061 RVA: 0x00014664 File Offset: 0x00012864
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000426 RID: 1062 RVA: 0x0001466C File Offset: 0x0001286C
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TIUtilities.GetTemplateValue<TIHabModuleTemplate>(TIUtilities.GetTemplateValue<TIHabModuleTemplate>(this.strIdx).upgradesFromName).displayName,
				TIUtilities.GetTemplateValue<TIHabModuleTemplate>(this.strIdx).displayName
			};
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x000146AA File Offset: 0x000128AA
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.ModuleFunctioning(TIUtilities.GetTemplateValue<TIHabModuleTemplate>(this.strIdx), true), TIUtilities.GetBoolValue(this.strValue));
	}
}
