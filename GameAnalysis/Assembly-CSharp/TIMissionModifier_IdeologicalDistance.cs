using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000238 RID: 568
public class TIMissionModifier_IdeologicalDistance : TIMissionModifier
{
	// Token: 0x0600077E RID: 1918 RVA: 0x000237F4 File Offset: 0x000219F4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		TIFactionIdeologyTemplate ideology = attackingCouncilor.faction.ideology;
		TIFactionIdeologyTemplate tifactionIdeologyTemplate = GameStateManager.UndecidedIdeology();
		if (target.ref_faction != null)
		{
			tifactionIdeologyTemplate = target.ref_faction.ideology;
		}
		return TINationState.GetIdeologicalDistance(ideology, tifactionIdeologyTemplate) * 2f;
	}
}
