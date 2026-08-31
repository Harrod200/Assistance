using System;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001A4 RID: 420
public class TIMissionCondition_EnemyHumanSpaceAsset : TIMissionCondition
{
	// Token: 0x06000622 RID: 1570 RVA: 0x0001C390 File Offset: 0x0001A590
	public override string CanTarget(TICouncilorState councilor, TIGameState possibleTarget)
	{
		if (possibleTarget.isSpaceShipState)
		{
			TIFactionState faction = possibleTarget.ref_ship.faction;
			if (faction != null && !faction.permanentAlly(councilor.faction))
			{
				TIFactionState faction2 = possibleTarget.ref_ship.faction;
				if (faction2 != null && !faction2.IsAlienFaction)
				{
					return "_Pass";
				}
			}
		}
		if (possibleTarget.isHabState && !possibleTarget.ref_hab.faction.permanentAlly(councilor.faction) && !possibleTarget.ref_hab.faction.IsAlienFaction && !possibleTarget.ref_hab.coreModule.moduleTemplate.automated && possibleTarget.ref_hab.crew > 0)
		{
			return "_Pass";
		}
		return base.GetType().Name;
	}
}
