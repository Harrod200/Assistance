using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001E3 RID: 483
public class TIMissionEffect_HostileTakeover : TIMissionEffect
{
	// Token: 0x060006B7 RID: 1719 RVA: 0x000201BC File Offset: 0x0001E3BC
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		if (base.MissionSuccess(outcome))
		{
			TIOrgState tiorgState = target as TIOrgState;
			TICouncilorState assignedCouncilor = tiorgState.assignedCouncilor;
			TIFactionState factionOrbit = tiorgState.factionOrbit;
			List<TIOrgState> list;
			mission.councilor.StealOrg(tiorgState, out list);
			if (tiorgState.assignedCouncilor != null)
			{
				factionOrbit.AddSuspicionForMajorReversal(5f, assignedCouncilor);
			}
			TINotificationQueueState.LogMyOrgStolen(mission.councilor.faction, assignedCouncilor, factionOrbit, tiorgState, list);
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TICouncilorState ticouncilorState in array[i].activeCouncilors)
				{
					if (ticouncilorState != mission.councilor && ticouncilorState.HasMission && ticouncilorState.activeMission.target == target && (ticouncilorState.activeMission.missionTemplate == TIFactionState.hostileTakeoverMission || ticouncilorState.activeMission.missionTemplate == TIFactionState.enthrallOrgMission))
					{
						ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.OrgAlreadyTaken, "");
					}
				}
			}
			if (outcome == TIMissionOutcome.CriticalSuccess)
			{
				float num = factionOrbit.TransferResourceToFaction(50f, FactionResource.Money, mission.councilor.faction);
				return new StringBuilder(TemplateManager.global.moneyInlineSpritePath).Append(num.ToString("N0")).ToString();
			}
		}
		return string.Empty;
	}
}
