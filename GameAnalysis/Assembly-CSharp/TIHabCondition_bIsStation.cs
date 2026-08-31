using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000FC RID: 252
public class TIHabCondition_bIsStation : TIHabCondition
{
	// Token: 0x06000429 RID: 1065 RVA: 0x000146F1 File Offset: 0x000128F1
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_hab != null && TICondition.PassesComparison(this.sign, state.ref_hab.IsStation, TIUtilities.GetBoolValue(this.strValue));
	}
}
