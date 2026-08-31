using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000229 RID: 553
public class TIMissionModifier_RegionalInvadingForcesStrength_MyFaction : TIMissionModifier
{
	// Token: 0x06000757 RID: 1879 RVA: 0x00023160 File Offset: 0x00021360
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIFactionState faction = attackingCouncilor.faction;
		TIRegionState ref_region = target.ref_region;
		if (ref_region != null)
		{
			foreach (TIArmyState tiarmyState in ref_region.FilteredArmiesPresent(false, false, true, false, false))
			{
				if (tiarmyState.faction == faction)
				{
					num += tiarmyState.GetEffectiveCombatStrength();
				}
			}
		}
		return Mathf.Max(num, 0f);
	}
}
