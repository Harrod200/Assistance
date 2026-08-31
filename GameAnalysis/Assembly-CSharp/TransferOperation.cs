using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200030C RID: 780
public class TransferOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000C84 RID: 3204 RVA: 0x00040D1C File Offset: 0x0003EF1C
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x00040D1F File Offset: 0x0003EF1F
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x00040D22 File Offset: 0x0003EF22
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return true;
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00040D25 File Offset: 0x0003EF25
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00040D28 File Offset: 0x0003EF28
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState targetState = null)
	{
		if (base.ActorCanPerformOperation_PassInterruptCheck(actorState) && (actorState.ref_faction.player.isAI || base.ActorCanPerformOperation(actorState, targetState)))
		{
			TISpaceFleetState ref_fleet = actorState.ref_fleet;
			return ref_fleet.mayLegallyStartATransfer && ref_fleet.isCapableOfTransfering && !ref_fleet.landed && !ref_fleet.inCombatOrWaitingForCombat;
		}
		return false;
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00040D86 File Offset: 0x0003EF86
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_FleetDestination);
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x00040D92 File Offset: 0x0003EF92
	public override bool RequiresThrustProfile()
	{
		return true;
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00040D95 File Offset: 0x0003EF95
	public override bool UseAbsoluteCompletionDateFromTrajectory()
	{
		return true;
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00040D98 File Offset: 0x0003EF98
	public bool ValidTransferDestinationForFleet(TISpaceFleetState fleet, TIGameState dest)
	{
		if (dest.isOrbitState)
		{
			return fleet.faction.CanTargetOrbit(dest.ref_orbit) && (fleet.orbitState != dest || fleet.dockedAtStation || fleet.mayLegallyStartATransfer);
		}
		if (dest.isSpaceFleetState && fleet != dest && (!dest.ref_fleet.dockedAtStation || !fleet.dockedAtStation || dest.ref_fleet.dockedLocation != fleet.dockedLocation))
		{
			return fleet.faction.CanTargetFleet(dest.ref_fleet);
		}
		return dest.isHabState && (!fleet.dockedAtStation || fleet.dockedLocation != dest) && fleet.faction.CanTargetStation(dest.ref_hab);
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x00040E64 File Offset: 0x0003F064
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		TIFactionState faction = ref_fleet.faction;
		list.AddRange(faction.TargetableOrbitsForNavigation);
		if (!ref_fleet.dockedOrLanded && !ref_fleet.mayLegallyStartATransfer)
		{
			list.Remove(ref_fleet.orbitState);
		}
		list.AddRange(faction.TargetableFleets.Where<TISpaceFleetState>((TISpaceFleetState x) => !x.landed));
		list.Remove(ref_fleet);
		list.AddRange(faction.TargetableStations);
		if (ref_fleet.dockedOrLanded)
		{
			list.Remove(ref_fleet.dockedLocation);
			if (ref_fleet.dockedLocation.isHabState && ref_fleet.dockedLocation.ref_hab.IsStation)
			{
				list.Remove(ref_fleet.dockedLocation.ref_hab.ref_orbit);
				foreach (TISpaceFleetState tispaceFleetState in ref_fleet.dockedLocation.ref_hab.dockedFleets)
				{
					list.Remove(tispaceFleetState);
				}
			}
		}
		return list;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x00040F94 File Offset: 0x0003F194
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory selectedTrajectory)
	{
		return (float)selectedTrajectory.launchTime.DifferenceInDays(selectedTrajectory.assignedTime);
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x00040FA8 File Offset: 0x0003F1A8
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost, Trajectory trajectory)
	{
		actorState.ref_fleet.AssignTrajectory(trajectory);
		bool flag = base.OnOperationConfirm(actorState, target, resourcesCost, trajectory);
		if (flag)
		{
			actorState.ref_faction.LogTransfer(trajectory);
		}
		return flag;
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x00040FD4 File Offset: 0x0003F1D4
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.transferAssigned || !ref_fleet.trajectory.launched)
		{
			base.OnOperationCancel(actorState, target, opCompleteDate);
			ref_fleet.AbortTransfer(-1, null, false);
		}
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00041010 File Offset: 0x0003F210
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (ref_fleet.transferAssigned)
		{
			ref_fleet.LaunchFleet(true);
			Trajectory trajectory = ref_fleet.trajectory;
			if (trajectory != null && trajectory.launched)
			{
				ref_fleet.GlobalCheckNotifyFleetLaunch();
			}
			ref_fleet.AddFleetLog("TransferLaunch");
		}
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00041058 File Offset: 0x0003F258
	public override bool CanCancel()
	{
		return true;
	}
}
