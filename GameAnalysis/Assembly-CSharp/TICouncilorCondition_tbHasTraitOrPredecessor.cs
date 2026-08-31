using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000E1 RID: 225
public class TICouncilorCondition_tbHasTraitOrPredecessor : TICouncilorCondition
{
	// Token: 0x060003DE RID: 990 RVA: 0x00013CD1 File Offset: 0x00011ED1
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithSign();
	}

	// Token: 0x1700007A RID: 122
	// (get) Token: 0x060003DF RID: 991 RVA: 0x00013CD9 File Offset: 0x00011ED9
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(2)
			{
				TIUtilities.GetTemplateValue<TITraitTemplate>(TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx).upgradesFrom).displayName,
				TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx).displayName
			};
		}
	}

	// Token: 0x060003E0 RID: 992 RVA: 0x00013D18 File Offset: 0x00011F18
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_councilor != null && (TICondition.PassesComparison<TITraitTemplate>(this.sign, TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx), state.ref_councilor.traits) || TICondition.PassesComparison<TITraitTemplate>(this.sign, TIUtilities.GetTemplateValue<TITraitTemplate>(TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx).upgradesFrom), state.ref_councilor.traits));
	}
}
