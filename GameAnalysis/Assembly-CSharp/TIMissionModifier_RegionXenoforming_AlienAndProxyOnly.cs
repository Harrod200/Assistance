using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000234 RID: 564
public class TIMissionModifier_RegionXenoforming_AlienAndProxyOnly : TIMissionModifier_HideInCodex
{
	// Token: 0x06000772 RID: 1906 RVA: 0x000235F0 File Offset: 0x000217F0
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming);
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x000235FC File Offset: 0x000217FC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null && (faction.IsAlienProxy || faction.IsAlienFaction))
		{
			return ref_region.xenoforming.AlienAttributeBonus();
		}
		return 0f;
	}
}
