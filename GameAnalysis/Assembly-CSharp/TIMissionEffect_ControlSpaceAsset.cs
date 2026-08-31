using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F0 RID: 496
public class TIMissionEffect_ControlSpaceAsset : TIMissionEffect
{
	// Token: 0x060006D1 RID: 1745 RVA: 0x00021138 File Offset: 0x0001F338
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x060006D2 RID: 1746 RVA: 0x0002113C File Offset: 0x0001F33C
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			if (base.MissionSuccess(outcome))
			{
				if (outcome == TIMissionOutcome.CriticalSuccess)
				{
					ref_hab.CaptureHab(mission.councilor.faction, 4, false, false, null, null);
					TIDateTime coreDefendExpiration = ref_hab.coreDefendExpiration;
					return ((coreDefendExpiration != null) ? coreDefendExpiration.ToCustomDateString() : null) ?? string.Empty;
				}
				ref_hab.CaptureHab(mission.councilor.faction, 2, false, false, null, null);
			}
			else
			{
				bool flag = false;
				TIFactionState ref_faction = target.ref_faction;
				if (outcome == TIMissionOutcome.CriticalFailure || ref_hab.tier == 1)
				{
					mission.councilor.DetainCouncilor(ref_faction, 3f, 2f, true);
					flag = true;
				}
				ref_hab.ResolveDefendHabEffect(ref_hab.faction, 6);
				if (!flag)
				{
					return string.Empty;
				}
				return Loc.T("TIMissionTemplate.FailurePlus.ControlSpaceAsset", new object[] { ref_faction.displayNameWithColor });
			}
		}
		else if (target.isSpaceShipState)
		{
			TISpaceShipState ref_ship = target.ref_ship;
			if (base.MissionSuccess(outcome))
			{
				TIFactionState faction = ref_ship.faction;
				TISpaceFleetState.CreateAtRunTime(mission.councilor.faction, new List<TISpaceShipState> { ref_ship }, ref_ship.fleet, ref_ship.fleet, null, false, false, null);
				mission.councilor.faction.GainIntel(faction, 50f, null, false);
				if (outcome == TIMissionOutcome.CriticalSuccess)
				{
					mission.councilor.faction.GainIntel(faction, 30f, null, false);
				}
				TINotificationQueueState.LogOurShipChangedSides(ref_ship, faction, mission.councilor.faction);
			}
			else
			{
				TIFactionState ref_faction2 = target.ref_faction;
				if (ref_ship.fleet.dockedAtHab)
				{
					mission.councilor.SetLocation(ref_ship.fleet.dockedLocation);
					if (outcome == TIMissionOutcome.CriticalFailure)
					{
						mission.councilor.DetainCouncilor(ref_faction2, 3f, 2f, true);
					}
					else
					{
						mission.councilor.DetainCouncilor(ref_faction2, 2f, 1f, true);
					}
				}
			}
		}
		return string.Empty;
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x0002131E File Offset: 0x0001F51E
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		if (!base.MissionSuccess(outcome) && target.isSpaceShipState && !target.ref_ship.fleet.dockedAtHab)
		{
			mission.councilor.KillCouncilorOnMission(mission);
		}
	}
}
