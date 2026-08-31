using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000225 RID: 549
public class TIMissionModifier_NationPopulation : TIMissionModifier
{
	// Token: 0x0600074F RID: 1871 RVA: 0x00022F90 File Offset: 0x00021190
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			return Mathf.Pow(tinationState.population_Millions, 0.4f);
		}
		return 0f;
	}
}
