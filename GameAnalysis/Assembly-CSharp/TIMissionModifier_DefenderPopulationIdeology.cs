using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200020A RID: 522
public class TIMissionModifier_DefenderPopulationIdeology : TIMissionModifier
{
	// Token: 0x06000713 RID: 1811 RVA: 0x000220F4 File Offset: 0x000202F4
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TINationState tinationState = TIMissionModifier.ObjectToNation(attackingCouncilor.faction, target);
		if (tinationState != null)
		{
			TIFactionState ref_faction = target.ref_faction;
			FactionIdeology factionIdeology = ((ref_faction != null) ? ref_faction.ideology.ideology : FactionIdeology.Undecided);
			if (factionIdeology != FactionIdeology.Undecided)
			{
				num = (10f + tinationState.democracy) * tinationState.GetPublicOpinionOfFaction(factionIdeology);
			}
		}
		return num;
	}
}
