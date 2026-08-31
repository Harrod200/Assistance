using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000209 RID: 521
public class TIMissionModifier_AttackerPopulationIdeology : TIMissionModifier
{
	// Token: 0x06000711 RID: 1809 RVA: 0x000220A0 File Offset: 0x000202A0
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			num = (10f + tinationState.democracy) * tinationState.GetPublicOpinionOfFaction(attackingCouncilor.faction.ideology);
		}
		return num;
	}
}
