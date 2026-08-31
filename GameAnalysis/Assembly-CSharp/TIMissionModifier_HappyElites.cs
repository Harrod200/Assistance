using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000227 RID: 551
public class TIMissionModifier_HappyElites : TIMissionModifier
{
	// Token: 0x06000753 RID: 1875 RVA: 0x00023028 File Offset: 0x00021228
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null && tinationState.corruption < tinationState.percentWeighttoPriority(PriorityType.Spoils))
		{
			return (tinationState.percentWeighttoPriority(PriorityType.Spoils) - tinationState.corruption) * 10f;
		}
		return 0f;
	}
}
