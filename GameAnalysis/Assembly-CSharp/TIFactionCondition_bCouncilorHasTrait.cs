using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A7 RID: 167
public class TIFactionCondition_bCouncilorHasTrait : TIFactionCondition
{
	// Token: 0x06000340 RID: 832 RVA: 0x00012502 File Offset: 0x00010702
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000064 RID: 100
	// (get) Token: 0x06000341 RID: 833 RVA: 0x0001250A File Offset: 0x0001070A
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00012528 File Offset: 0x00010728
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, state.ref_faction.CouncilHasTrait(TIUtilities.GetTemplateValue<TITraitTemplate>(this.strIdx), false, null), TIUtilities.GetBoolValue(this.strValue));
	}
}
