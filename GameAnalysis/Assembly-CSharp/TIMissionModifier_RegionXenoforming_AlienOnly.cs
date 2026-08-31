using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000232 RID: 562
public class TIMissionModifier_RegionXenoforming_AlienOnly : TIMissionModifier_HideInCodex
{
	// Token: 0x0600076C RID: 1900 RVA: 0x000234E8 File Offset: 0x000216E8
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming);
	}

	// Token: 0x0600076D RID: 1901 RVA: 0x000234F4 File Offset: 0x000216F4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null && faction.ideology.alien)
		{
			return ref_region.xenoforming.AlienAttributeBonus();
		}
		return 0f;
	}
}
