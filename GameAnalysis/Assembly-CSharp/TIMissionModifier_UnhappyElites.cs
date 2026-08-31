using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000226 RID: 550
public class TIMissionModifier_UnhappyElites : TIMissionModifier
{
	// Token: 0x06000751 RID: 1873 RVA: 0x00022FD4 File Offset: 0x000211D4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null && tinationState.percentWeighttoPriority(PriorityType.Spoils) < tinationState.corruption)
		{
			return (tinationState.corruption - tinationState.percentWeighttoPriority(PriorityType.Spoils)) * 15f;
		}
		return 0f;
	}
}
