using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001F1 RID: 497
public class TIMissionEffect_SeizeSpaceAsset : TIMissionEffect
{
	// Token: 0x060006D5 RID: 1749 RVA: 0x00021357 File Offset: 0x0001F557
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x0002135C File Offset: 0x0001F55C
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TIFactionState faction = mission.councilor.faction;
		TIFactionState ref_faction = target.ref_faction;
		List<TIOfficerState> list = new List<TIOfficerState>();
		List<TIOfficerState> list2 = new List<TIOfficerState>();
		Dictionary<TIFactionState, string> dictionary = new Dictionary<TIFactionState, string>();
		if (mission.councilor.ref_fleet != null && mission.councilor.ref_fleet.faction == faction)
		{
			List<TIOfficerState> list3;
			list.AddRange(mission.councilor.ref_fleet.PostAssaultPromotionsAndDeaths(outcome, true, out list3));
			list2.AddRange(list3);
		}
		if (mission.councilor.ref_hab != null)
		{
			foreach (TISpaceFleetState tispaceFleetState in mission.councilor.ref_hab.dockedFleets)
			{
				if (faction == tispaceFleetState.faction && tispaceFleetState != mission.councilor.ref_fleet)
				{
					List<TIOfficerState> list4;
					list.AddRange(tispaceFleetState.PostAssaultPromotionsAndDeaths(outcome, true, out list4));
					list2.AddRange(list4);
				}
			}
		}
		if (list.Count > 0)
		{
			dictionary.Add(faction, TIOfficerTemplate.BuildOfficerPromotionReport(list, faction));
		}
		if (list2.Count > 0)
		{
			dictionary.Add(faction, TIOfficerTemplate.BuildOfficerDeathsReport(list2, faction));
			foreach (TIOfficerState tiofficerState in list2.ToList<TIOfficerState>())
			{
				tiofficerState.DeleteOfficer(true);
			}
		}
		if (target.isHabState)
		{
			TIHabState ref_hab = target.ref_hab;
			ref_hab.TakeDamageFromParticipatingInAssault_Defense(outcome, faction);
			List<TIOfficerState> list5 = new List<TIOfficerState>();
			List<TIOfficerState> list6 = new List<TIOfficerState>();
			foreach (TISpaceFleetState tispaceFleetState2 in ref_hab.dockedFleets)
			{
				if (tispaceFleetState2.faction == ref_faction)
				{
					tispaceFleetState2.PostAssaultDamage(outcome, false);
					list5.AddRange(tispaceFleetState2.PostAssaultPromotionsAndDeaths(outcome, false, out list6));
				}
			}
			if (list5.Count > 0)
			{
				dictionary.Add(ref_faction, TIOfficerTemplate.BuildOfficerPromotionReport(list5, ref_faction));
			}
			if (list6.Count > 0)
			{
				dictionary.Add(ref_faction, TIOfficerTemplate.BuildOfficerDeathsReport(list6, ref_faction));
				foreach (TIOfficerState tiofficerState2 in list6.ToList<TIOfficerState>())
				{
					tiofficerState2.DeleteOfficer(true);
				}
			}
			switch (outcome)
			{
			case TIMissionOutcome.CriticalFailure:
				TINotificationQueueState.LogHabAssaultFailed(mission.councilor, ref_hab, dictionary, outcome);
				if (mission.councilor.GetProtectors().Count<TICouncilorState>() > 0 && !ref_hab.IsAlien())
				{
					mission.councilor.DetainCouncilor(ref_faction, 3f, 2f, true);
				}
				break;
			case TIMissionOutcome.Failure:
				TINotificationQueueState.LogHabAssaultFailed(mission.councilor, ref_hab, dictionary, outcome);
				if (mission.councilor.GetProtectors().Count<TICouncilorState>() == 0 && !ref_hab.IsAlien())
				{
					mission.councilor.DetainCouncilor(ref_faction, 3f, 2f, true);
				}
				break;
			case TIMissionOutcome.Success:
				return ref_hab.CaptureHab(faction, 1, false, false, dictionary, null);
			case TIMissionOutcome.CriticalSuccess:
				return ref_hab.CaptureHab(faction, 3, false, false, dictionary, null);
			}
		}
		if (mission.councilor.ref_fleet != null && mission.councilor.ref_fleet.faction == faction)
		{
			mission.councilor.ref_fleet.PostAssaultDamage(outcome, true);
		}
		if (mission.councilor.ref_hab != null)
		{
			if (mission.councilor.ref_hab.faction == faction && faction.MissionControlShortage <= 0 && faction.DailyHabBoostShortage() <= 0f && !faction.Insolvent)
			{
				mission.councilor.ref_hab.TakeDamageFromParticipatingInAssault_Offense(outcome, ref_faction);
			}
			foreach (TISpaceFleetState tispaceFleetState3 in mission.councilor.ref_hab.dockedFleets)
			{
				if (faction == tispaceFleetState3.faction)
				{
					tispaceFleetState3.PostAssaultDamage(outcome, true);
				}
			}
		}
		return string.Empty;
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x000217B8 File Offset: 0x0001F9B8
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		if (outcome == TIMissionOutcome.CriticalFailure && mission.councilor.GetProtectors().Count == 0)
		{
			mission.councilor.KillCouncilorOnMission(mission);
		}
	}
}
