using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200030E RID: 782
public class SurveyPlanetFromFleetOperation : TISpaceFleetOperationTemplate_Special
{
	// Token: 0x06000C96 RID: 3222 RVA: 0x0004106B File Offset: 0x0003F26B
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0004106E File Offset: 0x0003F26E
	public override List<SpecialModuleRule> RequiredCapability()
	{
		return new List<SpecialModuleRule> { SpecialModuleRule.Prospector };
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x0004107D File Offset: 0x0003F27D
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return (float)(28 / actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count);
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x00041099 File Offset: 0x0003F299
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_FleetDefaultTargetOnly);
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x000410A5 File Offset: 0x0003F2A5
	public override int SortOrder()
	{
		return 13;
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x000410A9 File Offset: 0x0003F2A9
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x000410AC File Offset: 0x0003F2AC
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x000410AF File Offset: 0x0003F2AF
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x000410B2 File Offset: 0x0003F2B2
	public override bool CanCancel()
	{
		return true;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x000410B5 File Offset: 0x0003F2B5
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ShipsWithSpecialModuleRule(this.RequiredCapability()).Count > 0;
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x000410D0 File Offset: 0x0003F2D0
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TIFactionState faction = ref_fleet.faction;
		return !ref_fleet.transferAssigned && ref_fleet.location.isOrbitState && ref_fleet.ref_orbit.interfaceOrbit && ref_fleet.ref_orbit.barycenter.isSpaceBodyState && !ref_fleet.inCombatOrWaitingForCombat && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && faction.CanProspectFromShip(ref_fleet.ref_orbit.barycenter.ref_spaceBody);
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x00041148 File Offset: 0x0003F348
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.transferAssigned && ref_fleet.location.isOrbitState && ref_fleet.ref_orbit.interfaceOrbit && ref_fleet.ref_orbit.barycenter.isSpaceBodyState)
		{
			TISpaceBodyState ref_spaceBody = ref_fleet.ref_orbit.ref_spaceBody;
			if (ref_fleet.faction.CanProspectFromShip(ref_spaceBody))
			{
				list.Add(ref_spaceBody);
			}
		}
		return list;
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x000411B8 File Offset: 0x0003F3B8
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost, Trajectory trajectory)
	{
		if (base.OnOperationConfirm(actorState, target, resourcesCost, trajectory))
		{
			GameControl.eventManager.TriggerEvent(new ProspectingBody(actorState.ref_faction, target.ref_spaceBody), null, new object[] { actorState.ref_faction, target.ref_spaceBody });
			TINotificationQueueState.LogScanningPlanet(actorState.ref_fleet, target.ref_spaceBody, this.GetDuration_days(actorState, target, trajectory));
			return true;
		}
		return false;
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x00041224 File Offset: 0x0003F424
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TISpaceBodyState ref_spaceBody = target.ref_spaceBody;
		ref_fleet.faction.ProspectSpaceBody(ref_spaceBody);
		TINotificationQueueState.LogScannedPlanet(ref_fleet, ref_spaceBody);
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x00041250 File Offset: 0x0003F450
	public override List<Type> BreakthroughOps()
	{
		return new List<Type>
		{
			typeof(CancelFleetOperation),
			typeof(MergeFleetOperation)
		};
	}
}
