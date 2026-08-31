using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200033F RID: 831
public class RemoteRefuelFleetOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000E45 RID: 3653 RVA: 0x00047C43 File Offset: 0x00045E43
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x00047C46 File Offset: 0x00045E46
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000E47 RID: 3655 RVA: 0x00047C49 File Offset: 0x00045E49
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.NeedsRefuel() && actorState.ref_fleet.currentOperations.Count == 0;
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x00047C6D File Offset: 0x00045E6D
	public override int SortOrder()
	{
		return 6;
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x00047C70 File Offset: 0x00045E70
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		TISpaceFleetState ref_fleet = actorState.ref_fleet;
		if (!ref_fleet.transferAssigned && !ref_fleet.inCombatOrWaitingForCombat && ref_fleet.CurrentOperations().Count == 0 && ref_fleet.NeedsRefuel())
		{
			base.ActorCanPerformOperation_PassInterruptCheck(actorState);
		}
		return false;
	}

	// Token: 0x06000E4A RID: 3658 RVA: 0x00047CB2 File Offset: 0x00045EB2
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000E4B RID: 3659 RVA: 0x00047CBE File Offset: 0x00045EBE
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000E4C RID: 3660 RVA: 0x00047CCC File Offset: 0x00045ECC
	public override bool UseResourceCostDuration()
	{
		return true;
	}

	// Token: 0x06000E4D RID: 3661 RVA: 0x00047CCF File Offset: 0x00045ECF
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000E4E RID: 3662 RVA: 0x00047CD8 File Offset: 0x00045ED8
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		foreach (TISpaceShipState tispaceShipState in actorState.ref_fleet.ships)
		{
			tispaceShipState.plannedResupplyAndRepair.ProcessResupplyAndRepair(tispaceShipState);
		}
	}
}
