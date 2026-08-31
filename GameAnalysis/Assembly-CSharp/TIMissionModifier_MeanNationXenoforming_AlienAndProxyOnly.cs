using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000235 RID: 565
public class TIMissionModifier_MeanNationXenoforming_AlienAndProxyOnly : TIMissionModifier_HideInCodex
{
	// Token: 0x06000775 RID: 1909 RVA: 0x00023649 File Offset: 0x00021849
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming);
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00023654 File Offset: 0x00021854
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		float num = 0f;
		if (ref_region != null && (faction.IsAlienProxy || faction.IsAlienFaction))
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
