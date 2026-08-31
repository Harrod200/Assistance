using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000239 RID: 569
public class TIMissionModifier_AlienAssetDefense : TIMissionModifier
{
	// Token: 0x06000780 RID: 1920 RVA: 0x00023840 File Offset: 0x00021A40
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIRegionAlienAssetState ref_regionAlienAsset = target.ref_regionAlienAsset;
		TIRegionState region = ref_regionAlienAsset.region;
		float num2 = (float)region.NumFactionArmiesPresent(GameStateManager.AlienFaction(), true, true, true, true);
		float num3 = (float)region.NumFactionArmiesPresent(GameStateManager.AlienProxy(), true, true, true, false);
		bool alienNation = region.nation.alienNation;
		bool flag = GameStateManager.AlienProxy().NationWithFactionInterest(region.nation, false);
		if (ref_regionAlienAsset.isRegionLandedUFO)
		{
			num += 36f;
			num += num2;
			num += num3 / 2f;
			num += (float)(alienNation ? 6 : 0);
			num += (float)(flag ? 3 : 0);
		}
		else if (ref_regionAlienAsset.isRegionAlienFacility)
		{
			num += 18f;
			num += num2;
			num += (float)(alienNation ? 7 : 0);
			if (GameStateManager.AlienProxy().SufficientIntel(ref_regionAlienAsset.ref_alienFacility, 1f))
			{
				num += num3 / 2f;
				num += (float)(flag ? 1 : 0);
			}
		}
		else if (ref_regionAlienAsset.isRegionXenoformingState)
		{
			num += ref_regionAlienAsset.ref_xenoforming.xenoformingLevel / 20f;
			num += num2;
			num += (float)(alienNation ? 2 : 0);
			if (ref_regionAlienAsset.ref_xenoforming.VisibleToFaction(GameStateManager.AlienProxy()))
			{
				num += num3 / 2f;
				num += (float)(flag ? 1 : 0);
			}
		}
		return num;
	}
}
