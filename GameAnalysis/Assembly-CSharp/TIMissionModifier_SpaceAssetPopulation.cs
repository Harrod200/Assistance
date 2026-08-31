using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000252 RID: 594
public class TIMissionModifier_SpaceAssetPopulation : TIMissionModifier
{
	// Token: 0x060007B4 RID: 1972 RVA: 0x000247B8 File Offset: 0x000229B8
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		if (target.isHabState)
		{
			if (target.ref_hab.decommissioning)
			{
				return 0f;
			}
			return Mathf.Pow((float)target.ref_hab.crew, 0.33f);
		}
		else
		{
			if (target.isSpaceShipState)
			{
				return Mathf.Pow((float)target.ref_ship.template.crewBillets, 0.5f);
			}
			return 0f;
		}
	}
}
