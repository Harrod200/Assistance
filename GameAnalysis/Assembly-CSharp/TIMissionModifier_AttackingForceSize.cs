using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200023E RID: 574
public class TIMissionModifier_AttackingForceSize : TIMissionModifier
{
	// Token: 0x0600078A RID: 1930 RVA: 0x00023ACC File Offset: 0x00021CCC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		TIFactionState ref_faction = attackingCouncilor.ref_faction;
		TISpaceFleetState ref_fleet = attackingCouncilor.ref_fleet;
		TISpaceFleetState tispaceFleetState = ((((ref_fleet != null) ? ref_fleet.faction : null) == ref_faction) ? attackingCouncilor.ref_fleet : null);
		TIHabState ref_hab = attackingCouncilor.ref_hab;
		TIHabState tihabState = ((((ref_hab != null) ? ref_hab.faction : null) == ref_faction) ? attackingCouncilor.ref_hab : null);
		if (tispaceFleetState != null)
		{
			num += tispaceFleetState.AssaultCombatValue(false);
		}
		if (tihabState != null)
		{
			if (ref_faction.GetAveragedMissionControlShortage() <= 0f && ref_faction.DailyHabBoostShortage() <= 0f && !ref_faction.Insolvent)
			{
				num += tihabState.AssaultCombatValue(false);
			}
			foreach (TISpaceFleetState tispaceFleetState2 in tihabState.dockedFleets)
			{
				if (tispaceFleetState2 != tispaceFleetState && tispaceFleetState2.faction == attackingCouncilor.faction)
				{
					num += tispaceFleetState2.AssaultCombatValue(false);
				}
			}
		}
		return num;
	}
}
