using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000233 RID: 563
public class TIMissionModifier_MeanNationXenoforming_AlienOnly : TIMissionModifier_HideInCodex
{
	// Token: 0x0600076F RID: 1903 RVA: 0x0002353E File Offset: 0x0002173E
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x00023548 File Offset: 0x00021748
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		float num = 0f;
		if (ref_region != null && faction.ideology.alien)
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
