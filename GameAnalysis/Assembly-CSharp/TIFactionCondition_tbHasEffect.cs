using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A0 RID: 160
public class TIFactionCondition_tbHasEffect : TIFactionCondition
{
	// Token: 0x0600032C RID: 812 RVA: 0x000122BC File Offset: 0x000104BC
	public override string GetDescriptionPath()
	{
		return base.GetDescriptionPathWithValue();
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x0600032D RID: 813 RVA: 0x000122C4 File Offset: 0x000104C4
	public override List<string> descriptionParams
	{
		get
		{
			return new List<string>(1) { TIUtilities.GetTemplateValue<TIEffectTemplate>(this.strIdx).displayName };
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x000122E2 File Offset: 0x000104E2
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, TIEffectsState.CheckForEffectInAnyContext(state.ref_faction, TIUtilities.GetTemplateValue<TIEffectTemplate>(this.strIdx)), TIUtilities.GetBoolValue(this.strValue));
	}
}
