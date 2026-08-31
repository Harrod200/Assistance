using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000A2 RID: 162
public class TIFactionCondition_tbCompletedCampaignObjective : TIFactionCondition
{
	// Token: 0x06000334 RID: 820 RVA: 0x000123AC File Offset: 0x000105AC
	public override bool PassesCondition(TIGameState state)
	{
		return state.ref_faction != null && TICondition.PassesComparison<TIObjectiveTemplate>(this.sign, TIUtilities.GetTemplateValue<TIObjectiveTemplate>(this.strValue), state.ref_faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Completed));
	}
}
