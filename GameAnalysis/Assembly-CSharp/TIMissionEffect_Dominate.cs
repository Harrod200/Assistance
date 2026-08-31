using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020001D9 RID: 473
public class TIMissionEffect_Dominate : TIMissionEffect
{
	// Token: 0x0600069E RID: 1694 RVA: 0x0001EFCC File Offset: 0x0001D1CC
	public override string ApplyEffect(TIMissionState mission, TIGameState target, TIMissionOutcome outcome = TIMissionOutcome.Success)
	{
		TIFactionState gainingFaction = mission.councilor.faction;
		TIControlPoint ticontrolPoint = target.ref_nation.controlPoints.Where<TIControlPoint>((TIControlPoint x) => x.faction != gainingFaction).MinBy<TIControlPoint, int>((TIControlPoint x) => x.positionInNation);
		TIFactionState faction = ticontrolPoint.faction;
		List<TIGameState> controlPointOwnersByPoint = ticontrolPoint.nation.controlPointOwnersByPoint;
		ticontrolPoint.nation.ChangeControlPointOwner(ticontrolPoint.positionInNation, ControlPointChangeCause.Politics, gainingFaction);
		List<TIGameState> controlPointOwnersByPoint2 = ticontrolPoint.nation.controlPointOwnersByPoint;
		switch (outcome)
		{
		case TIMissionOutcome.CriticalFailure:
			ticontrolPoint.nation.capital.ApplyDamageToRegion(0.9f, gainingFaction, null, true, false, true, false);
			if (ticontrolPoint.executive)
			{
				ticontrolPoint.nation.AddToDemocracy(-3f, TINationState.DemocracyChangeReason.DemReason_RegimeChange);
			}
			break;
		case TIMissionOutcome.Failure:
			ticontrolPoint.nation.capital.ApplyDamageToRegion(0.4f, gainingFaction, null, true, false, true, false);
			if (ticontrolPoint.executive)
			{
				ticontrolPoint.nation.AddToDemocracy(-3f, TINationState.DemocracyChangeReason.DemReason_RegimeChange);
			}
			break;
		case TIMissionOutcome.Success:
			if (ticontrolPoint.executive)
			{
				ticontrolPoint.nation.AddToDemocracy(-3f, TINationState.DemocracyChangeReason.DemReason_RegimeChange);
			}
			break;
		case TIMissionOutcome.CriticalSuccess:
			ticontrolPoint.nation.PropagandaOnPop(gainingFaction.ideology, 15f, false);
			break;
		}
		if (faction != null)
		{
			TINotificationQueueState.LogCPDominated(ticontrolPoint, mission.councilor.faction, faction, outcome, controlPointOwnersByPoint2, controlPointOwnersByPoint);
		}
		return ((faction != null) ? faction.displayNameWithColor : null) ?? Loc.T("UI.Notifications.Neutral");
	}
}
