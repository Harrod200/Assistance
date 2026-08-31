using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020000D2 RID: 210
public class TIGlobalCondition_bAllFactionsCompletedMilestone : TIGlobalCondition
{
	// Token: 0x060003B3 RID: 947 RVA: 0x0001344C File Offset: 0x0001164C
	public override bool PassesCondition(TIGameState state)
	{
		CampaignMilestone campaignMilestone = this.strIdx.ToEnum(CampaignMilestone.None);
		int num = 0;
		TIFactionState[] array = GameStateManager.AllHumanFactions();
		int num2 = 0;
		while (num2 < array.Length && array[num2].milestones.Contains(campaignMilestone))
		{
			num++;
			num2++;
		}
		return state.ref_faction != null && TICondition.PassesComparison(this.sign, num == GameStateManager.AllHumanFactions().Count<TIFactionState>(), TIUtilities.GetBoolValue(this.strValue));
	}
}
