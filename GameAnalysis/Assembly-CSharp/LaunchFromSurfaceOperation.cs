using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using Unity.Entities;

// Token: 0x0200032C RID: 812
public class LaunchFromSurfaceOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D72 RID: 3442 RVA: 0x00042FD1 File Offset: 0x000411D1
	public override int SortOrder()
	{
		return 12;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00042FD5 File Offset: 0x000411D5
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00042FD8 File Offset: 0x000411D8
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x00042FDB File Offset: 0x000411DB
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet == actorState && actorState.ref_fleet.landed;
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00042FF8 File Offset: 0x000411F8
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.isSpaceFleetState && actorState.ref_fleet.landed && (double)actorState.ref_fleet.maxAcceleration_gs >= actorState.ref_fleet.dockedLocation.ref_habSite.parentBody.surfaceGravity_g && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && actorState.ref_spaceBody.interfaceOrbits.Any<TIOrbitState>((TIOrbitState x) => (double)actorState.ref_fleet.currentDeltaV_kps >= x.DeltaVToReachFromSurface_kps(actorState.ref_fleet.dockedLocation.ref_habSite.latitude, (double)actorState.ref_fleet.maxAcceleration_mps2));
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00043094 File Offset: 0x00041294
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return (float)(target.ref_orbit.altitude_km / target.ref_orbit.averageOrbitalVelocity_kps) / 86400f;
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x000430B4 File Offset: 0x000412B4
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return actorState.ref_fleet.dockedLocation.ref_habSite.parentBody.interfaceOrbits.Where<TIOrbitState>((TIOrbitState x) => (double)actorState.ref_fleet.currentDeltaV_kps >= x.DeltaVToReachFromSurface_kps(actorState.ref_fleet.dockedLocation.ref_habSite.latitude, (double)actorState.ref_fleet.maxAcceleration_mps2)).ToList<TIOrbitState>().ConvertAll<TIGameState>((TIOrbitState x) => x);
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00043127 File Offset: 0x00041327
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_OrbitFew);
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00043134 File Offset: 0x00041334
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState fleet = actorState.ref_fleet;
		if (fleet.landed && (double)fleet.maxAcceleration_gs >= fleet.dockedLocation.ref_habSite.parentBody.surfaceGravity_g && fleet.ref_spaceBody.interfaceOrbits.Any<TIOrbitState>((TIOrbitState x) => (double)fleet.currentDeltaV_kps >= x.DeltaVToReachFromSurface_kps(fleet.dockedLocation.ref_habSite.latitude, (double)fleet.maxAcceleration_mps2)))
		{
			TIOrbitState orbit = target.ref_orbit;
			TIHabSiteState launchSite = actorState.ref_fleet.dockedLocation.ref_habSite;
			fleet.ships.ForEach(delegate(TISpaceShipState x)
			{
				x.ConsumeDeltaV((float)orbit.DeltaVToReachFromSurface_kps(launchSite.latitude, (double)actorState.ref_fleet.maxAcceleration_mps2), true);
			});
			fleet.DepartFromDockingLocation();
			fleet.SetRandomizedOrbitFromState(orbit, true);
			fleet.ships.ForEach(delegate(TISpaceShipState x)
			{
				x.SetVisualizationDataDirty();
			});
			World.Active.GetExistingManager<SpaceObjectPositioning>().TriggerForceUpdate();
			GameControl.eventManager.TriggerEvent(new FleetArrivesAtDestination(fleet, orbit, true), null, new object[] { fleet, orbit, orbit.ref_spaceBody });
			TINotificationQueueState.LogFleetArrival(fleet, launchSite, orbit, false, false, new Dictionary<TIFactionState, string>());
		}
	}
}
