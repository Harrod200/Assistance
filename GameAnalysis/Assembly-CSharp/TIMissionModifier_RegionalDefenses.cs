using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000228 RID: 552
public class TIMissionModifier_RegionalDefenses : TIMissionModifier
{
	// Token: 0x06000755 RID: 1877 RVA: 0x0002307C File Offset: 0x0002127C
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null)
		{
			if (ref_region.IsFullyOccupied())
			{
				num += (float)(ref_region.NumArmiesPresent(false, false, true, false) - ref_region.NumFactionArmiesPresent(faction, false, false, true, false));
			}
			else
			{
				num += ref_region.nation.militaryTechLevel * 3f;
				num += (float)(ref_region.NumArmiesPresent(true, false, false, false) - ref_region.NumFactionArmiesPresent(faction, true, false, false, true)) * ref_region.nation.militaryTechLevel;
				num += (float)(ref_region.NumArmiesPresent(false, true, false, false) - ref_region.NumFactionArmiesPresent(faction, false, true, false, true));
				if (ref_region == ref_region.nation.capital)
				{
					num += 2f;
				}
				if (ref_region.colonyRegion)
				{
					num -= 1f;
				}
			}
		}
		return Math.Max(num, 0f);
	}
}
