using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200032D RID: 813
public class LandOnSurfaceOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D7C RID: 3452 RVA: 0x000432FC File Offset: 0x000414FC
	public override int SortOrder()
	{
		return 12;
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00043300 File Offset: 0x00041500
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00043303 File Offset: 0x00041503
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00043306 File Offset: 0x00041506
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x0004330C File Offset: 0x0004150C
	private bool CanLandOnSurface(TIGameState actorState)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		return !ref_fleet.transferAssigned && !ref_fleet.landed && !ref_fleet.inCombatOrWaitingForCombat && ref_fleet.orbitState.interfaceOrbit && !ref_fleet.orbitState.barycenter.isEarth && (double)ref_fleet.fullyLoadedAcceleration_gs >= actorState.ref_fleet.orbitState.barycenter.ref_spaceBody.surfaceGravity_g;
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x0004337F File Offset: 0x0004157F
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.isSpaceFleetState && this.CanLandOnSurface(actorState) && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && this.GetPossibleTargets(actorState, null).Count > 0;
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x000433AD File Offset: 0x000415AD
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet == actorState && !actorState.ref_fleet.transferAssigned && !actorState.ref_fleet.landed;
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x000433DA File Offset: 0x000415DA
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0.041666668f + (float)actorState.ref_fleet.orbitalPeriod_s * 0.5f / 86400f;
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x000433FC File Offset: 0x000415FC
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		TISpaceBodyState ref_spaceBody = actorState.ref_orbit.barycenter.ref_spaceBody;
		TIOrbitState tiorbitState = ref_spaceBody.interfaceOrbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.averageOrbitalVelocity_kps);
		List<TIGameState> list = new List<TIGameState>();
		TIHabSiteState[] habSites = ref_spaceBody.habSites;
		int i = 0;
		Func<TISpaceFleetState, bool> <>9__2;
		while (i < habSites.Length)
		{
			TIHabSiteState site = habSites[i];
			if (!(site.ref_hab != null) || !(site.ref_hab.faction != actorState.ref_faction))
			{
				goto IL_0101;
			}
			if (site.ref_hab.SpaceCombatValue() <= 0f)
			{
				IEnumerable<TISpaceFleetState> dockedFleets = site.ref_hab.dockedFleets;
				Func<TISpaceFleetState, bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (TISpaceFleetState x) => x.faction.permanentAlly(actorState.ref_faction));
				}
				if (dockedFleets.All<TISpaceFleetState>(func))
				{
					goto IL_0101;
				}
			}
			IL_0252:
			i++;
			continue;
			IL_0101:
			double DVToLand = site.DeltaVToLandFromInterface_kps(actorState.ref_fleet.orbitState, (double)actorState.ref_fleet.maxAcceleration_mps2, false, actorState.ref_fleet.ships.All<TISpaceShipState>((TISpaceShipState x) => x.SpecialModuleRules(false).Contains(SpecialModuleRule.ImmunetoAerobrakingDamage)));
			double DVToLaunch = tiorbitState.DeltaVToReachFromSurface_kps(site.latitude, (double)actorState.ref_fleet.maxAcceleration_mps2);
			bool flag = site.ref_hab != null && site.ref_hab.AllowsResupply(actorState.ref_faction, true, false);
			bool flag2 = actorState.ref_fleet.ships.All<TISpaceShipState>((TISpaceShipState x) => (double)x.currentDeltaV_kps >= DVToLand);
			bool flag3 = flag || actorState.ref_fleet.ships.All<TISpaceShipState>((TISpaceShipState x) => (double)x.currentDeltaV_kps >= DVToLand + DVToLaunch || x.CanRefuelFromHabSite(site));
			if (!flag2 || !flag3)
			{
				goto IL_0252;
			}
			if (site.ref_hab != null)
			{
				list.Add(site.ref_habSite);
				goto IL_0252;
			}
			list.Add(site);
			goto IL_0252;
		}
		return list.OrderByDescending<TIGameState, bool>((TIGameState x) => x.ref_hab != null).ToList<TIGameState>();
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00043695 File Offset: 0x00041895
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_BaseHabSite);
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x000436A4 File Offset: 0x000418A4
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (this.CanLandOnSurface(actorState))
		{
			if (actorState.ref_fleet.dockedAtHab)
			{
				actorState.ref_fleet.DepartFromDockingLocation();
			}
			TIHabState ref_hab = target.ref_hab;
			if (ref_hab != null && ref_hab.IsBase)
			{
				actorState.ref_fleet.Dock(target.ref_hab, false);
			}
			else
			{
				actorState.ref_fleet.Land(target.ref_habSite);
			}
			TINotificationQueueState.LogFleetLanded(actorState.ref_fleet);
		}
	}
}
