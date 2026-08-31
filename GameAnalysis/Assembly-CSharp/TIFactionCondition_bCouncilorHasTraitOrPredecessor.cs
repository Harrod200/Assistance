using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A8 RID: 168
public class TIFactionCondition_bCouncilorHasTraitOrPredecessor : TIFactionCondition
{
	// Token: 0x06000344 RID: 836 RVA: 0x00012570 File Offset: 0x00010770
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000065 RID: 101
	// (get) Token: 0x06000345 RID: 837 RVA: 0x00012578 File Offset: 0x00010778
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

	// Token: 0x06000346 RID: 838 RVA: 0x000125B8 File Offset: 0x000107B8
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.CouncilHasTrait(TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx), false, null) || state.ref_faction.CouncilHasTrait(TIUtilities.GetTemplateValue<TITraitTemplate>(TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx).upgradesFrom), false, null), TIUtilities.GetBoolValue(this.strValue));
	}
}
