using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001EA RID: 490
public class TIMissionEffect_TerrorizeRegion : TIMissionEffect
{
	// Token: 0x060006C5 RID: 1733 RVA: 0x00020C5C File Offset: 0x0001EE5C
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		float num = 0f;
		TIFactionState faction = mission.councilor.faction;
		TIRegionState ref_region = target.ref_region;
		TINationState ref_nation = target.ref_nation;
		ref_region.ApplyDamageToRegion(TIUtilities.RandomRange(0.01f, 0.07f), faction, null, true, false, false, false);
		TIFactionState tifactionState = GameStateManager.AlienAppeaser();
		if (base.MissionSuccess(outcome))
		{
			TIControlPoint ticontrolPoint = ref_nation.controlPoints.First<TIControlPoint>((TIControlPoint x) => x.CanBeTerrorized());
			List<TIGameState> controlPointOwnersByPoint = ref_nation.controlPointOwnersByPoint;
			TIFactionState faction2 = ticontrolPoint.faction;
			TIFactionState tifactionState2 = tifactionState;
			ref_nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Terrorize, tifactionState2);
			List<TIGameState> controlPointOwnersByPoint2 = ref_nation.controlPointOwnersByPoint;
			if (faction2 != null)
			{
				TINotificationQueueState.LogMyControlPointPurged(faction2, tifactionState2, ticontrolPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint);
				faction2.CompleteMilestone(CampaignMilestone.TargetedByTerrorMission);
			}
			TINotificationQueueState.LogLoyaltySwitch(tifactionState2, faction2, ticontrolPoint, controlPointOwnersByPoint2, controlPointOwnersByPoint, mission.missionTemplate);
		}
		switch (outcome)
		{
		case TIMissionOutcome.CriticalFailure:
			num = ref_nation.PropagandaOnPop(tifactionState.ideology, -10f, false);
			break;
		case TIMissionOutcome.Success:
			num = ref_nation.PropagandaOnPop(tifactionState.ideology, 20f, false);
			break;
		case TIMissionOutcome.CriticalSuccess:
			num = ref_nation.PropagandaOnPop(tifactionState.ideology, 40f, false);
			break;
		}
		TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienAwareness_Public);
		TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienOvertAggression);
		return num.ToPercent("P0");
	}
}
