using System;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class TIMissionModifier_Censorship : TIMissionModifier
{
	// Token: 0x0600075F RID: 1887 RVA: 0x0002332C File Offset: 0x0002152C
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState;
		if (target.isCouncilorState)
		{
			tinationState = TIMissionPhaseState.CouncilorLastKnownLocation(attackingCouncilor.faction, target.ref_councilor).ref_nation;
		}
		else
		{
			tinationState = target.ref_nation;
		}
		if (tinationState.democracy > 5f || tinationState.FactionHasControlPoint(attackingCouncilor.faction))
		{
			return 0f;
		}
		if (tinationState.alienNation && attackingCouncilor.faction.permanentAlly(tinationState.executiveFaction))
		{
			return 0f;
		}
		return 5f / Mathf.Max(tinationState.democracy, 0.5f);
	}
}
