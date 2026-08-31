using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class UndockFromStationOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D66 RID: 3430 RVA: 0x00042E10 File Offset: 0x00041010
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000D67 RID: 3431 RVA: 0x00042E13 File Offset: 0x00041013
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00042E16 File Offset: 0x00041016
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet == actorState && actorState.ref_fleet.dockedAtStation;
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00042E33 File Offset: 0x00041033
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_FleetDefaultTargetOnly);
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x00042E3F File Offset: 0x0004103F
	public override int SortOrder()
	{
		return 12;
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x00042E44 File Offset: 0x00041044
	private bool CanUndock(TIGameState actorState)
	{
		return actorState.ref_fleet == actorState && actorState.ref_fleet.dockedAtStation && actorState.ref_fleet.allShipsHaveDeltaV && !actorState.ref_fleet.transferAssigned && !actorState.ref_fleet.inCombatOrWaitingForCombat && actorState.ref_fleet.allShipsCanManeuver;
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x00042EA0 File Offset: 0x000410A0
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.CanUndock(actorState) && base.ActorCanPerformOperation_PassInterruptCheck(actorState);
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x00042EB4 File Offset: 0x000410B4
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0.04f;
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x00042EBB File Offset: 0x000410BB
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState.ref_fleet.dockedLocation.ref_orbit };
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00042ED8 File Offset: 0x000410D8
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		TISpaceFleetState ref_fleet = actor.ref_fleet;
		foreach (TISpaceShipState tispaceShipState in ref_fleet.ships)
		{
			Vector3d vector3d = ref_fleet.dockedLocation.ref_hab.SpatialRotation * Vector3d.forward * 600.0;
			tispaceShipState.InitiateManeuverSequence(tispaceShipState.currentFleetOffset, tispaceShipState.fleetFormationOffset + vector3d * 0.5, tispaceShipState.fleetFormationOffset, ref_fleet.RotationNow);
		}
		return base.OperationConfirmed(actor, target, opCompleteDate);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00042F90 File Offset: 0x00041190
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (this.CanUndock(actorState))
		{
			TIHabState ref_hab = actorState.ref_fleet.ref_hab;
			actorState.ref_fleet.DepartFromDockingLocation();
			TINotificationQueueState.LogFleetUndocked(actorState.ref_fleet, ref_hab);
		}
	}

	// Token: 0x04000EB2 RID: 3762
	public const float UndockFromStationDuration_days = 0.04f;
}
