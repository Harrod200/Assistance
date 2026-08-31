using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000326 RID: 806
public class SplitFleetOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000D1C RID: 3356 RVA: 0x000422DC File Offset: 0x000404DC
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000D1D RID: 3357 RVA: 0x000422DF File Offset: 0x000404DF
	public override int SortOrder()
	{
		return 7;
	}

	// Token: 0x06000D1E RID: 3358 RVA: 0x000422E2 File Offset: 0x000404E2
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000D1F RID: 3359 RVA: 0x000422E5 File Offset: 0x000404E5
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000D20 RID: 3360 RVA: 0x000422E8 File Offset: 0x000404E8
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.ships.Count > 1;
	}

	// Token: 0x06000D21 RID: 3361 RVA: 0x000422FD File Offset: 0x000404FD
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000D22 RID: 3362 RVA: 0x00042304 File Offset: 0x00040504
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_fleet.ships.Count > 1 && base.ActorCanPerformOperation_PassInterruptCheck(actorState) && actorState.ref_fleet.mayLegallyStartATransfer && !actorState.ref_fleet.inCombatOrWaitingForCombat && SplitFleetOperation.EligibleShips(actorState.ref_fleet).Count > 0 && (!actorState.ref_fleet.landed || !actorState.ref_fleet.underBombardment);
	}

	// Token: 0x06000D23 RID: 3363 RVA: 0x00042379 File Offset: 0x00040579
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Ships);
	}

	// Token: 0x06000D24 RID: 3364 RVA: 0x00042385 File Offset: 0x00040585
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000D25 RID: 3365 RVA: 0x00042393 File Offset: 0x00040593
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
	}

	// Token: 0x06000D26 RID: 3366 RVA: 0x00042395 File Offset: 0x00040595
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost = null, Trajectory trajectory = null)
	{
		return target != null && target.ref_fleet.ships.Count > 0;
	}

	// Token: 0x06000D27 RID: 3367 RVA: 0x000423B8 File Offset: 0x000405B8
	public static List<TISpaceShipState> EligibleShips(TISpaceFleetState fleet)
	{
		IEnumerable<OperationData> enumerable = fleet.CurrentOperations();
		List<TISpaceShipState> list = new List<TISpaceShipState>(fleet.ships);
		if (enumerable.Any<OperationData>((OperationData x) => x.operation.GetType() == typeof(ResupplyOperation) || x.operation.GetType() == typeof(RepairFleetOperation) || x.operation.GetType() == typeof(ResupplyAndRepairOperation)))
		{
			list.RemoveAll((TISpaceShipState x) => x.plannedResupplyAndRepair.active);
		}
		return list;
	}

	// Token: 0x06000D28 RID: 3368 RVA: 0x00042424 File Offset: 0x00040624
	public static TISpaceFleetState BuildFleetFromSelectedTargets(TISpaceFleetState originFleet, List<TISpaceShipState> shipsToTransfer, FactionGoal_Fleet goal = null)
	{
		TISpaceFleetState tispaceFleetState = TISpaceFleetState.CreateAtRunTime(originFleet.faction, shipsToTransfer, originFleet, originFleet, goal, false, false, null);
		originFleet.AddFleetLog("Split");
		return tispaceFleetState;
	}
}
