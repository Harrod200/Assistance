using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A3 RID: 163
public class TIFactionCondition_tbUnlockedCampaignObjective : TIFactionCondition
{
	// Token: 0x06000336 RID: 822 RVA: 0x000123E9 File Offset: 0x000105E9
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison<TIObjectiveTemplate>(this.sign, TIUtilities.GetTemplateValue<TIObjectiveTemplate>(this.strValue), state.ref_faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked));
	}
}
