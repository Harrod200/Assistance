using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200031C RID: 796
public abstract class FoundOutpostFromFleetOperation : FoundHabFromFleetOperation
{
	// Token: 0x06000CEA RID: 3306 RVA: 0x000419C7 File Offset: 0x0003FBC7
	public override Type GetTargetingMethod()
	{
		if (!(this.fleet.location is TIHabSiteState))
		{
			return typeof(TIOperationTargeting_HabSite);
		}
		return typeof(TIOperationTargeting_FleetDefaultTargetOnly);
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x000419F0 File Offset: 0x0003FBF0
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		if (base.ActorCanPerformOperation_PassInterruptCheck(actorState))
		{
			this.fleet = actorState.ref_fleet;
			TIHabSiteState tihabSiteState = this.fleet.location as TIHabSiteState;
			if (tihabSiteState != null)
			{
				return !tihabSiteState.hasPlannedOrOperatingBase && !this.fleet.inCombatOrWaitingForCombat && this.fleet.faction.AvailableMissionControl >= -this.CoreModule(this.fleet.faction.IsAlienFaction).missionControl && this.fleet.faction.EligibleForFoundingBase(tihabSiteState.ref_spaceBody);
			}
			TIOrbitState tiorbitState = this.fleet.location as TIOrbitState;
			if (tiorbitState != null)
			{
				TISpaceBodyState tispaceBodyState = tiorbitState.barycenter as TISpaceBodyState;
				if (tispaceBodyState != null)
				{
					return tiorbitState.interfaceOrbit && !this.fleet.inCombatOrWaitingForCombat && this.fleet.faction.AvailableMissionControl >= -this.CoreModule(this.fleet.faction.IsAlienFaction).missionControl && this.fleet.faction.EligibleForFoundingBase(tispaceBodyState);
				}
			}
		}
		return false;
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00041B12 File Offset: 0x0003FD12
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		if (base.OnOperationConfirm(actorState, target, resourcesCost, trajectory))
		{
			target.ref_habSite.MarkPendingHab();
			return true;
		}
		return false;
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00041B30 File Offset: 0x0003FD30
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TIFactionState faction = ref_fleet.faction;
		List<TIGameState> list = new List<TIGameState>();
		if (ref_fleet.location.isHabSiteState)
		{
			TIHabSiteState ref_habSite = ref_fleet.location.ref_habSite;
			if (!ref_habSite.hasPlannedOrOperatingBase && faction.EligibleForFoundingBase(ref_habSite.parentBody))
			{
				list.Add(ref_habSite);
			}
		}
		else if (ref_fleet.location.isOrbitState)
		{
			TIOrbitState ref_orbit = ref_fleet.location.ref_orbit;
			if (ref_orbit.interfaceOrbit)
			{
				TISpaceBodyState ref_spaceBody = ref_orbit.barycenter.ref_spaceBody;
				if (ref_spaceBody != null && faction.EligibleForFoundingBase(ref_spaceBody))
				{
					list.AddRange(ref_spaceBody.vacantHabSites);
				}
			}
		}
		return list;
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00041BDC File Offset: 0x0003FDDC
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TIHabSiteState ref_habSite = target.ref_habSite;
		ref_habSite.FoundHab();
		TIHabState tihabState = GameStateManager.CreateNewGameState<TIHabState>();
		tihabState.InitializeNewHab(ref_fleet.faction, ref_habSite, ref_fleet, this.GetTier(), 0f, this.AdditionalModules(ref_fleet.faction.IsAlienFaction));
		TINotificationQueueState.LogHabFounded(ref_fleet.faction, tihabState, ref_habSite);
		ref_fleet.ExpendSpecialModuleCapability(this.RequiredCapability(), false, false);
		FactionGoal_FoundHab factionGoal_FoundHab = ref_fleet.AssignedGoal() as FactionGoal_FoundHab;
		if (factionGoal_FoundHab == null)
		{
			return;
		}
		factionGoal_FoundHab.SetHab(tihabState);
	}

	// Token: 0x04000EB0 RID: 3760
	private TISpaceFleetState fleet;
}
