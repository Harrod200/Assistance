using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E0 RID: 224
public class TICouncilorCondition_tbHasTrait : TICouncilorCondition
{
	// Token: 0x060003DA RID: 986 RVA: 0x00013C70 File Offset: 0x00011E70
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithSign();
	}

	// Token: 0x17000079 RID: 121
	// (get) Token: 0x060003DB RID: 987 RVA: 0x00013C78 File Offset: 0x00011E78
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x060003DC RID: 988 RVA: 0x00013C96 File Offset: 0x00011E96
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && TICondition.PassesComparison<TITraitTemplate>(this.sign, TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx), state.ref_councilor.traits);
	}
}
