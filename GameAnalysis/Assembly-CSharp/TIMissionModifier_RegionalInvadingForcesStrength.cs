using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200022A RID: 554
public class TIMissionModifier_RegionalInvadingForcesStrength : TIMissionModifier
{
	// Token: 0x06000759 RID: 1881 RVA: 0x000231FC File Offset: 0x000213FC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null)
		{
			foreach (TIArmyState tiarmyState in ref_region.FilteredArmiesPresent(false, false, true, false, false))
			{
				num += tiarmyState.GetEffectiveCombatStrength();
			}
		}
		return Mathf.Max(num, 0f);
	}
}
