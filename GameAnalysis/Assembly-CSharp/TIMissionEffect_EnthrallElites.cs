using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001E9 RID: 489
public class TIMissionEffect_EnthrallElites : TIMissionEffect
{
	// Token: 0x060006C3 RID: 1731 RVA: 0x000209C8 File Offset: 0x0001EBC8
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TICouncilorState councilor = mission.councilor;
		TIFactionState tifactionState = GameStateManager.AlienProxy();
		TINationState ref_nation = target.ref_nation;
		TIControlPoint ticontrolPoint = ref_nation.FirstNativeControlPoint();
		if (target.isControlPointState)
		{
			TIControlPoint ref_controlPoint = target.ref_controlPoint;
			TIFactionState faction = ref_controlPoint.faction;
			if (base.MissionSuccess(outcome))
			{
				List<TIGameState> controlPointOwnersByPoint = ref_nation.controlPointOwnersByPoint;
				ref_nation.ChangeControlPointOwner(ref_controlPoint.positionInNation, ControlPointChangeCause.Enthrall, tifactionState);
				List<TIGameState> controlPointOwnersByPoint2 = ref_nation.controlPointOwnersByPoint;
				float num = 0f;
				if (outcome == TIMissionOutcome.CriticalSuccess)
				{
					num = ref_nation.PropagandaOnPop(councilor.faction.ideology, (float)(councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) / 2), false);
				}
				TINotificationQueueState.LogLoyaltySwitch(tifactionState, faction, ref_controlPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint, mission.missionTemplate);
				TINotificationQueueState.LogMyControlPointPurged(faction, tifactionState, ref_controlPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint);
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
				return num.ToPercent("P0");
			}
		}
		else if (target.isNationState && base.MissionSuccess(outcome) && ticontrolPoint != null)
		{
			if (ticontrolPoint != ref_nation.executiveControlPoint || ref_nation.numControlPoints == 1 || ref_nation.CountFactionControlPoints(ref_nation.numberTwoControlPoint.faction, true, false, true) >= 2)
			{
				int num2 = ref_nation.numControlPoints - ref_nation.StartOfTurnNativeControlPoints;
				if (ref_nation.GetControlPoint(num2).owned)
				{
					for (int j = ref_nation.FirstNativeControlPoint().positionInNation; j > num2; j--)
					{
						ref_nation.ChangeControlPointOwner(j, ControlPointChangeCause.Politics, ref_nation.GetControlPoint(j - 1).faction);
					}
				}
				ref_nation.ChangeControlPointOwner(num2, ControlPointChangeCause.Politics, tifactionState);
			}
			else
			{
				ref_nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Politics, tifactionState);
			}
			if (outcome == TIMissionOutcome.CriticalSuccess)
			{
				return ref_nation.PropagandaOnPop(councilor.faction.ideology, (float)(councilor.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false) / 2), false).ToPercent("P0");
			}
		}
		return string.Empty;
	}
}
