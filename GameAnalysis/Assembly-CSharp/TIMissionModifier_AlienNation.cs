using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000251 RID: 593
public class TIMissionModifier_AlienNation : TIMissionModifier_HideInCodex
{
	// Token: 0x060007B1 RID: 1969 RVA: 0x00024755 File Offset: 0x00022955
	public override bool ShowCondition(TIFactionState faction)
	{
		return GameStateManager.AlienNation().extant;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00024764 File Offset: 0x00022964
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
		if (tinationState != null && tinationState.alienNation)
		{
			return 6f;
		}
		return 0f;
	}
}
