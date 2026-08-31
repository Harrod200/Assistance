using System;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001D4 RID: 468
public class TIMissionEffect_DetainCouncilor : TIMissionEffect
{
	// Token: 0x06000690 RID: 1680 RVA: 0x0001E7C4 File Offset: 0x0001C9C4
	public override bool HasDelayedEffect()
	{
		return true;
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x0001E7C8 File Offset: 0x0001C9C8
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TICouncilorState ref_councilor = target.ref_councilor;
		if (ref_councilor == null)
		{
			return string.Empty;
		}
		if (!base.MissionSuccess(outcome))
		{
			return string.Empty;
		}
		ref_councilor.AddToParanoia(mission.councilor.faction);
		string text = ref_councilor.DetainCouncilor(councilor.faction, (float)((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1), (float)((outcome == TIMissionOutcome.CriticalSuccess) ? 2 : 1), false);
		ref_councilor.faction.AddSuspicionForMajorReversal(10f, ref_councilor);
		if (ref_councilor.isAlien)
		{
			councilor.faction.CompleteMilestone(CampaignMilestone.AccessLiveHydra);
			if (mission.councilor.faction.aliensRemoved > 1)
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
			councilor.faction.aliensRemoved++;
			councilor.faction.alienInvestigations += 3;
			councilor.faction.FixAssessedAlienHateToActualValue();
			ref_councilor.faction.GainFactionHate(councilor.faction, TIFactionState.assassinateMission.hate[(int)outcome] - TIFactionState.detainMission.hate[(int)outcome], false, "Alien Captured", true);
			return Loc.T("TIMissionTemplate.Detain.DetainAlien");
		}
		StringBuilder stringBuilder = new StringBuilder(Loc.T("TIMissionTemplate.DetentionEnd.Detain", new object[] { text }));
		if (ref_councilor.HasMission)
		{
			stringBuilder.AppendLine(Loc.T("TIMissionTemplate.BonusEffect.Detain", new object[] { ref_councilor.activeMission.displayName }));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x0001E9B8 File Offset: 0x0001CBB8
	public override void ApplyDelayedEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success, string dataName = "")
	{
		TICouncilorState ref_councilor = target.ref_councilor;
		if (base.MissionSuccess(outcome))
		{
			if (ref_councilor.isAlien)
			{
				if (mission.councilor != null && mission.councilor.faction != null && mission.councilor.faction.isActivePlayer)
				{
					TICouncilorState councilor = mission.councilor;
					if (councilor != null)
					{
						TIFactionState faction = councilor.faction;
						if (faction != null)
						{
							faction.UnlockAchievement("captureAlien");
						}
					}
				}
				ref_councilor.Retire();
				return;
			}
			if (mission.councilor != null && mission.councilor.faction != null && mission.councilor.faction.isActivePlayer)
			{
				mission.councilor.faction.UnlockAchievement("captureCouncilor");
				return;
			}
		}
		else if (outcome == TIMissionOutcome.CriticalFailure)
		{
			if (ref_councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.specialTraitRule == SpecialTraitRule.HardTarget) && mission.councilor.GetProtectors().Count == 0 && !mission.councilor.isAlien)
			{
				mission.councilor.KillCouncilorOnMission(mission);
			}
		}
	}
}
