using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class TIMissionModifier_RegionPopulationDensity : TIMissionModifier
{
	// Token: 0x0600075D RID: 1885 RVA: 0x000232CC File Offset: 0x000214CC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIRegionState ref_region = target.ref_region;
		float num = 0f;
		if (ref_region != null)
		{
			num = Mathf.Pow(ref_region.populationInMillions, 0.33f);
			if (ref_region.terrain == TerrainType.Rugged)
			{
				num /= 2f;
			}
			if (ref_region.coreEconomicRegion)
			{
				num *= 1.5f;
			}
		}
		return num;
	}
}
