using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000FA RID: 250
public class TIHabCondition_tbHasModuleFunctioning : TIHabCondition
{
	// Token: 0x06000421 RID: 1057 RVA: 0x000145F7 File Offset: 0x000127F7
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000422 RID: 1058 RVA: 0x000145FF File Offset: 0x000127FF
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIHabModuleTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x0001461D File Offset: 0x0001281D
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.ModuleFunctioning(TIUtilities.GetTemplateValue<TIHabModuleTemplate>(this.strIdx), false), TIUtilities.GetBoolValue(this.strValue));
	}
}
