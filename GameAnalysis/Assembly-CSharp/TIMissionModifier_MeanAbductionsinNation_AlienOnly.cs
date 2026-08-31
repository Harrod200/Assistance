using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000237 RID: 567
public class TIMissionModifier_MeanAbductionsinNation_AlienOnly : TIMissionModifier_HideInCodex
{
	// Token: 0x0600077B RID: 1915 RVA: 0x0002374B File Offset: 0x0002194B
	public override bool ShowCondition(TIFactionState faction)
	{
		return faction.CanDetectAbductions;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00023754 File Offset: 0x00021954
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		if (faction.ideology.alien && ref_region != null)
		{
			foreach (TIRegionState tiregionState in ref_region.nation.regions)
			{
				num += tiregionState.GetAbductionsMissionBonusFromRegion();
			}
			return num / (float)ref_region.nation.regions.Count;
		}
		return num;
	}
}
