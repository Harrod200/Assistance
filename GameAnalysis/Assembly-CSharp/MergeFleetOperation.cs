using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000324 RID: 804
public class MergeFleetOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D0C RID: 3340 RVA: 0x00041F8A File Offset: 0x0004018A
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000D0D RID: 3341 RVA: 0x00041F8D File Offset: 0x0004018D
	public override int SortOrder()
	{
		return 6;
	}

	// Token: 0x06000D0E RID: 3342 RVA: 0x00041F90 File Offset: 0x00040190
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000D0F RID: 3343 RVA: 0x00041F93 File Offset: 0x00040193
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D10 RID: 3344 RVA: 0x00041F96 File Offset: 0x00040196
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return true;
	}

	// Token: 0x06000D11 RID: 3345 RVA: 0x00041F9C File Offset: 0x0004019C
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (base.ActorCanPerformOperation_PassInterruptCheck(actorState) && !ref_fleet.inCombatOrWaitingForCombat)
		{
			foreach (TISpaceFleetState tispaceFleetState in ref_fleet.faction.fleets)
			{
				if (ref_fleet.CanMerge(tispaceFleetState))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06000D12 RID: 3346 RVA: 0x00042018 File Offset: 0x00040218
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000D13 RID: 3347 RVA: 0x00042020 File Offset: 0x00040220
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		List<TIGameState> list = new List<TIGameState>();
		foreach (TISpaceFleetState tispaceFleetState in ref_fleet.faction.fleets)
		{
			if (ref_fleet.CanMerge(tispaceFleetState))
			{
				list.Add(tispaceFleetState);
			}
		}
		return list;
	}

	// Token: 0x06000D14 RID: 3348 RVA: 0x00042090 File Offset: 0x00040290
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Fleet);
	}

	// Token: 0x06000D15 RID: 3349 RVA: 0x0004209C File Offset: 0x0004029C
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		bool flag = false;
		FactionGoal_Fleet factionGoal_Fleet = actorState.ref_fleet.AssignedGoal();
		if (factionGoal_Fleet != null && factionGoal_Fleet is FactionGoal_JoinFleet && factionGoal_Fleet.target().ref_fleet == target.ref_fleet)
		{
			flag = true;
		}
		TISpaceFleetState tispaceFleetState = (flag ? target.ref_fleet : actorState.ref_fleet);
		TISpaceFleetState tispaceFleetState2 = (flag ? actorState.ref_fleet : target.ref_fleet);
		List<TISpaceFleetState> fleetsToIgnore = tispaceFleetState2.CheckForTransferTargetLoop();
		foreach (Trajectory trajectory in from x in GameStateManager.IterateByClass<TISpaceFleetState>(false)
			where x.transferAssigned && !fleetsToIgnore.Contains(x)
			select x.trajectory)
		{
			int num = 100;
			while (trajectory != null && num > 0)
			{
				num--;
				if (trajectory.destinationFleet == tispaceFleetState2)
				{
					if ((TIGameState)trajectory.fleet != tispaceFleetState.ref_gameState)
					{
						trajectory.ChangeDestinationFleet(tispaceFleetState);
					}
					else
					{
						trajectory.DestinationDestroyed();
					}
				}
				trajectory = trajectory.nextTrajectory;
			}
		}
		string displayName = tispaceFleetState2.GetDisplayName(GameControl.control.activePlayer);
		List<TISpaceShipState> list = new List<TISpaceShipState>(tispaceFleetState2.ships);
		if (tispaceFleetState2.dockedAtHab)
		{
			tispaceFleetState2.DepartFromDockingLocation();
		}
		tispaceFleetState.AddShipsToFleet(list, tispaceFleetState2, false, false);
		TINotificationQueueState.LogFleetsMerged(tispaceFleetState, displayName);
		tispaceFleetState.AddFleetLog("MergedInto");
	}
}
