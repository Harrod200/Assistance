using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;

// Token: 0x020001D3 RID: 467
public class TIMissionEffect_Assassinate : TIMissionEffect
{
	// Token: 0x0600068C RID: 1676 RVA: 0x0001E0A0 File Offset: 0x0001C2A0
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		if (base.MissionSuccess(outcome) && ref_councilor.isAlien)
		{
			return Loc.T("TIMissionTemplate.AssassinateAlien");
		}
		if (!ref_councilor.isAlien)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (outcome == TIMissionOutcome.Success || outcome == TIMissionOutcome.Failure || outcome == TIMissionOutcome.CriticalFailure)
			{
				if (ref_councilor.GetTraitWithSpecialTraitRule(SpecialTraitRule.AtrocityIfAssassinationAttemptUpon) != null)
				{
					mission.councilor.faction.CommitAtrocity(1, TIFactionState.AtrocityCause.AssassinatedBeloved, false, 0.333f);
				}
				if (ref_councilor.GrantsMarkedToAssassin() && !mission.councilor.traitTemplateNames.Contains("Marked"))
				{
					mission.councilor.AddTrait("Marked");
					stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special1").ToString()));
				}
			}
			if (outcome == TIMissionOutcome.CriticalFailure && !ref_councilor.detained && mission.councilor.GetProtectors().Count == 0 && !mission.councilor.isAlien)
			{
				if (ref_councilor.traits.None<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.HardTarget))
				{
					TIFactionState faction = ref_councilor.faction;
					if (faction != mission.councilor.faction)
					{
						mission.councilor.DetainCouncilor(faction, 2f, 1f, true);
						stringBuilder.AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".Special2").ToString(), new object[] { faction.displayNameCapitalized }));
					}
				}
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0001E244 File Offset: 0x0001C444
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0001E248 File Offset: 0x0001C448
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		if (base.MissionSuccess(outcome))
		{
			if (ref_councilor.isAlien)
			{
				mission.councilor.faction.CompleteMilestone(CampaignMilestone.AccessHydraCorpus);
				if (mission.councilor.faction.aliensRemoved > 0)
				{
					int num = ref_councilor.orgs.Count<TIOrgState>((TIOrgState x) => x.templateName == TIGlobalConfig.globalConfig.alienShockTroopOrgDataName);
					if (num > 0)
					{
						if (mission.councilor.faction.MilestoneCompleted(CampaignMilestone.AccessSalamanderCorpus))
						{
							if (TIUtilities.RandomFloatValue() < (float)num * 0.1f)
							{
								mission.councilor.faction.CompleteMilestone(CampaignMilestone.AccessLiveSalamander);
							}
						}
						else if (TIUtilities.RandomFloatValue() < (float)num * 0.25f)
						{
							mission.councilor.faction.CompleteMilestone(CampaignMilestone.AccessSalamanderCorpus);
						}
					}
				}
				mission.councilor.faction.aliensRemoved++;
				mission.councilor.faction.alienInvestigations += 2;
				if (mission.missionTemplate.hate[(int)outcome] == 0f)
				{
					(from x in GameStateManager.AllHumanFactions()
						where x != GameStateManager.AlienProxy() && x != GameStateManager.AlienAppeaser()
						select x).ToList<TIFactionState>().ForEach(delegate(TIFactionState x)
					{
						GameStateManager.AlienFaction().GainFactionHate(x, mission.missionTemplate.hate.Max() / (float)GameStateManager.AllHumanFactions().Length, false, "Alien Assassinated by unknown faction", true);
					});
				}
				if (mission.councilor.faction.isActivePlayer)
				{
					mission.councilor.faction.UnlockAchievement("killAlien");
				}
			}
			else
			{
				TIFactionState faction = mission.councilor.faction;
				if (faction != null && faction.isActivePlayer)
				{
					mission.councilor.faction.UnlockAchievement("killCouncilor");
					if (ref_councilor.inSpace)
					{
						mission.councilor.faction.UnlockAchievement("killCouncilorSpace");
					}
				}
			}
			if (mission.councilor.faction != ref_councilor.faction)
			{
				mission.councilor.faction.RegisterKill(ref_councilor, 1f);
				ref_councilor.faction.AddSuspicionForMajorReversal(25f, ref_councilor);
				TINotificationQueueState.LogMyCouncilorAssassinated(ref_councilor, mission.councilor, mission.missionTemplate.hate[(int)outcome]);
			}
			if (!mission.councilor.assassinations.ContainsKey(ref_councilor.faction))
			{
				mission.councilor.assassinations.Add(ref_councilor.faction, 0);
			}
			Dictionary<TIFactionState, int> dictionary = mission.councilor.assassinations;
			TIFactionState tifactionState = ref_councilor.faction;
			dictionary[tifactionState]++;
			if (!mission.councilor.faction.factionAssassinations.ContainsKey(ref_councilor.faction))
			{
				mission.councilor.faction.factionAssassinations.Add(ref_councilor.faction, 0);
			}
			dictionary = mission.councilor.faction.factionAssassinations;
			tifactionState = ref_councilor.faction;
			dictionary[tifactionState]++;
			ref_councilor.KillCouncilor(true, (outcome == TIMissionOutcome.Success) ? mission.councilor.faction : null);
			return;
		}
		if (outcome == TIMissionOutcome.CriticalFailure && !ref_councilor.detained && mission.councilor.GetProtectors().Count == 0)
		{
			if (ref_councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.HardTarget))
			{
				mission.councilor.KillCouncilorOnMission(mission);
				return;
			}
		}
		if (mission.councilor.faction == ref_councilor.faction)
		{
			IEnumerable<TIFactionState> enumerable = from x in GameStateManager.AllHumanFactions().Except<TIFactionState>(new List<TIFactionState> { ref_councilor.faction })
				where x.turnedCouncilors.Count < 2
				select x;
			if (enumerable.Count<TIFactionState>() > 0)
			{
				TIFactionState tifactionState2 = enumerable.SelectRandomItem<TIFactionState>();
				ref_councilor.TurnCouncilor(tifactionState2);
				mission.councilor.faction.DismissCouncilor(ref_councilor, tifactionState2);
				ref_councilor.AddTrait("Vengeful");
				return;
			}
			List<TransferOrgToFactionPoolAction> list = new List<TransferOrgToFactionPoolAction>();
			foreach (TIOrgState tiorgState in ref_councilor.orgs)
			{
				list.Add(new TransferOrgToFactionPoolAction(tiorgState, ref_councilor));
			}
			foreach (TransferOrgToFactionPoolAction transferOrgToFactionPoolAction in list)
			{
				mission.councilor.faction.playerControl.StartAction(transferOrgToFactionPoolAction);
			}
			mission.councilor.faction.DismissCouncilor(ref_councilor, mission.councilor.faction);
			mission.councilor.faction.availableCouncilors.Remove(ref_councilor);
		}
	}
}
