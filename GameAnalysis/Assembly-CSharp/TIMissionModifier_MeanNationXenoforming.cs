using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000231 RID: 561
public class TIMissionModifier_MeanNationXenoforming : TIMissionModifier_HideInCodex
{
	// Token: 0x06000769 RID: 1897 RVA: 0x0002344A File Offset: 0x0002164A
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming);
	}

	// Token: 0x0600076A RID: 1898 RVA: 0x00023454 File Offset: 0x00021654
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIRegionState ref_region = target.ref_region;
		float num = 0f;
		if (ref_region != null)
		{
			foreach (TIRegionState tiregionState in ref_region.nation.regions)
			{
				num += tiregionState.xenoforming.AlienAttributeBonus();
			}
			return num / (float)ref_region.nation.regions.Count;
		}
		return num;
	}
}
