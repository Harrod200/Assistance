using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020001D8 RID: 472
public class TIMissionEffect_Purge : TIMissionEffect
{
	// Token: 0x0600069C RID: 1692 RVA: 0x0001EE08 File Offset: 0x0001D008
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TIControlPoint ref_controlPoint = target.ref_controlPoint;
		TINationState nation = ref_controlPoint.nation;
		TIFactionState tifactionState = (councilor.faction.IsAlienFaction ? GameStateManager.AlienProxy() : councilor.faction);
		TIFactionState faction = ref_controlPoint.faction;
		if (base.MissionSuccess(outcome))
		{
			List<TIGameState> controlPointOwnersByPoint = nation.controlPointOwnersByPoint;
			nation.ChangeControlPointOwner(ref_controlPoint.positionInNation, ControlPointChangeCause.Politics, tifactionState);
			List<TIGameState> controlPointOwnersByPoint2 = nation.controlPointOwnersByPoint;
			if (outcome == TIMissionOutcome.CriticalSuccess)
			{
				nation.PropagandaOnPop(tifactionState.ideology, TemplateManager.global.basePropagandaStrength, false);
			}
			TINotificationQueueState.LogMyControlPointPurged(faction, tifactionState, ref_controlPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint);
			if (councilor.faction.isActivePlayer && ref_controlPoint.executive)
			{
				councilor.faction.UnlockAchievement("purgeExecutive");
			}
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				foreach (TICouncilorState ticouncilorState in array[i].activeCouncilors)
				{
					if (ticouncilorState != mission.councilor && ticouncilorState.HasMission && ticouncilorState.activeMission.target == target && (ticouncilorState.activeMission.missionTemplate == TIFactionState.purgeMission || ticouncilorState.activeMission.missionTemplate == TIFactionState.enthrallElitesMission))
					{
						ticouncilorState.activeMission.ResolveMission(TIMissionState.AbortReason.ControlPointAlreadyPurged, "");
					}
				}
			}
		}
		else if (outcome == TIMissionOutcome.CriticalFailure)
		{
			nation.PropagandaOnPop(tifactionState.ideology, (float)Mathf.Min(-1, councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) - 11), false);
		}
		return faction.displayNameWithColor;
	}
}
