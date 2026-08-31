using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000230 RID: 560
public class TIMissionModifier_RegionXenoforming : TIMissionModifier_HideInCodex
{
	// Token: 0x06000766 RID: 1894 RVA: 0x00023408 File Offset: 0x00021608
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming);
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00023414 File Offset: 0x00021614
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null)
		{
			return ref_region.xenoforming.AlienAttributeBonus();
		}
		return 0f;
	}
}
