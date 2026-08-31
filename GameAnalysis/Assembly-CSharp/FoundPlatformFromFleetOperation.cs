using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000310 RID: 784
public abstract class FoundPlatformFromFleetOperation : FoundHabFromFleetOperation
{
	// Token: 0x06000CB2 RID: 3250 RVA: 0x00041336 File Offset: 0x0003F536
	public virtual bool DestroyShipOnExecute()
	{
		return false;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x00041339 File Offset: 0x0003F539
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_FleetDefaultTargetOnly);
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x00041348 File Offset: 0x0003F548
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.dockedOrLanded && !ref_fleet.inTransfer && !ref_fleet.inCombatOrWaitingForCombat && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && ref_fleet.faction.AvailableMissionControl >= -this.CoreModule(ref_fleet.faction.IsAlienFaction).missionControl)
		{
			TIOrbitState ref_orbit = ref_fleet.location.ref_orbit;
			return ref_orbit != null && ref_orbit.NewStationAllowed(this.GetTier(), null);
		}
		return false;
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x000413C0 File Offset: 0x0003F5C0
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		List<TIGameState> list = new List<TIGameState>();
		if (!ref_fleet.landed && !ref_fleet.inTransfer)
		{
			TIOrbitState ref_orbit = ref_fleet.location.ref_orbit;
			if (ref_orbit != null && ref_orbit.NewStationAllowed(this.GetTier(), null))
			{
				list.Add(ref_fleet.location.ref_orbit);
			}
		}
		return list;
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0004141C File Offset: 0x0003F61C
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		if (base.OnOperationConfirm(actorState, target, resourcesCost, trajectory))
		{
			target.ref_orbit.MarkPendingHab();
			return true;
		}
		return false;
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0004143C File Offset: 0x0003F63C
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TIOrbitState ref_orbit = target.ref_orbit;
		TIHabState tihabState = GameStateManager.CreateNewGameState<TIHabState>();
		ref_orbit.FoundHab();
		tihabState.InitializeNewHab(ref_fleet.faction, ref_orbit, ref_fleet, this.GetTier(), 0f, this.AdditionalModules(ref_fleet.faction.IsAlienFaction));
		TINotificationQueueState.LogHabFounded(ref_fleet.faction, tihabState, ref_orbit);
		ref_fleet.ExpendSpecialModuleCapability(this.RequiredCapability(), false, this.DestroyShipOnExecute());
		FactionGoal_FoundHab factionGoal_FoundHab = ref_fleet.AssignedGoal() as FactionGoal_FoundHab;
		if (factionGoal_FoundHab == null)
		{
			return;
		}
		factionGoal_FoundHab.SetHab(tihabState);
	}
}
