using System;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200023F RID: 575
public class TIMissionModifier_DefendingForceSize : TIMissionModifier
{
	// Token: 0x0600078C RID: 1932 RVA: 0x00023BEC File Offset: 0x00021DEC
	public override float GetModifier(TICouncilorState attackingCouncilor, TIGameState target = null, float resourcesSpent = 0f, FactionResource resource = FactionResource.None)
	{
		float num = 0f;
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			TIFactionState ref_faction = target.ref_faction;
			num = ref_hab.AssaultCombatValue(true);
			foreach (TISpaceFleetState tispaceFleetState in ref_hab.dockedFleets)
			{
				if (tispaceFleetState.faction == ref_faction)
				{
					num += tispaceFleetState.AssaultCombatValue(true);
				}
			}
			if (attackingCouncilor.ref_habSite != null && ref_hab.IsBase)
			{
				num += (float)ref_hab.ActiveCombatModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.tier);
			}
		}
		else
		{
			num += target.ref_ship.AssaultCombatValue(true);
		}
		return num;
	}
}
