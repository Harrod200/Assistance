using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D1 RID: 209
public class TIGlobalCondition_iFactionsCompletedMilestone : TIGlobalCondition
{
	// Token: 0x060003B1 RID: 945 RVA: 0x000133D8 File Offset: 0x000115D8
	public override bool PassesCondition(TIGameState state)
	{
		CampaignMilestone campaignMilestone = this.strIdx.ToEnum(CampaignMilestone.None);
		int num = 0;
		TIFactionState[] array = GameStateManager.AllHumanFactions();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].milestones.Contains(campaignMilestone))
			{
				num++;
			}
		}
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, num, TIUtilities.GetIntValue(this.strValue));
	}
}
