using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000338 RID: 824
public class InterfleetRefuelOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000DEA RID: 3562 RVA: 0x00046984 File Offset: 0x00044B84
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x00046987 File Offset: 0x00044B87
	public override bool UpdatePropulsionOnComplete()
	{
		return true;
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0004698A File Offset: 0x00044B8A
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0004698D File Offset: 0x00044B8D
	public override int SortOrder()
	{
		return 11;
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x00046991 File Offset: 0x00044B91
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_SelfRefuel);
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0004699D File Offset: 0x00044B9D
	public override bool HasResourceCost()
	{
		return true;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x000469A0 File Offset: 0x00044BA0
	public override bool UseResourceCostDuration()
	{
		return true;
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x000469A3 File Offset: 0x00044BA3
	public override bool CanCancel()
	{
		return true;
	}

	// Token: 0x06000DF2 RID: 3570 RVA: 0x000469A6 File Offset: 0x00044BA6
	public override bool CancelUponCombat()
	{
		return true;
	}

	// Token: 0x06000DF3 RID: 3571 RVA: 0x000469A9 File Offset: 0x00044BA9
	public override bool MustAcceptCombat()
	{
		return true;
	}

	// Token: 0x06000DF4 RID: 3572 RVA: 0x000469AC File Offset: 0x00044BAC
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		bool flag = actorState.ref_fleet.transferAssigned && !actorState.ref_fleet.trajectory.involuntary;
		return actorState.ref_fleet.CanSharePropellant() && actorState.ref_fleet.currentOperations.Count == 0 && !flag && !actorState.ref_fleet.inCombatOrWaitingForCombat && base.ActorCanPerformOperation_PassInterruptCheck(actorState);
	}

	// Token: 0x06000DF5 RID: 3573 RVA: 0x00046A15 File Offset: 0x00044C15
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.CanSharePropellant();
	}

	// Token: 0x06000DF6 RID: 3574 RVA: 0x00046A22 File Offset: 0x00044C22
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000DF7 RID: 3575 RVA: 0x00046A30 File Offset: 0x00044C30
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000DF8 RID: 3576 RVA: 0x00046A38 File Offset: 0x00044C38
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		base.OnOperationCancel(actorState, target, opCompleteDate);
		actorState.ref_fleet.ships.ForEach(delegate(TISpaceShipState x)
		{
			x.plannedResupplyAndRepair.CancelResupply(actorState.ref_faction);
		});
	}

	// Token: 0x06000DF9 RID: 3577 RVA: 0x00046A81 File Offset: 0x00044C81
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_fleet.ExecutePropellantSharingPlan();
		TINotificationQueueState.LogOurFleetRefueled(actorState.ref_fleet, true);
	}

	// Token: 0x06000DFA RID: 3578 RVA: 0x00046A9A File Offset: 0x00044C9A
	public static float GetRefuelDuration_days(List<PropellantSharingEvent> plan)
	{
		return plan.Sum<PropellantSharingEvent>((PropellantSharingEvent x) => x.amount_tons / 100f) * TemplateManager.global.daysToRefuelAPropellantTank;
	}
}
