using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000353 RID: 851
public class NuclearWeaponsStrike : TINationOperationTemplate
{
	// Token: 0x06000ECB RID: 3787 RVA: 0x00049485 File Offset: 0x00047685
	public override int SortOrder()
	{
		return 0;
	}

	// Token: 0x06000ECC RID: 3788 RVA: 0x00049488 File Offset: 0x00047688
	public override bool IsBlockingOperation()
	{
		return true;
	}

	// Token: 0x06000ECD RID: 3789 RVA: 0x0004948B File Offset: 0x0004768B
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000ECE RID: 3790 RVA: 0x0004948E File Offset: 0x0004768E
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_nation.numNuclearWeapons > 0;
	}

	// Token: 0x06000ECF RID: 3791 RVA: 0x0004949E File Offset: 0x0004769E
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_nation.numNuclearWeapons > 0;
	}

	// Token: 0x06000ED0 RID: 3792 RVA: 0x000494AE File Offset: 0x000476AE
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return actorState.ref_nation.NuclearWeaponsTargets(false).ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState);
	}

	// Token: 0x06000ED1 RID: 3793 RVA: 0x000494E0 File Offset: 0x000476E0
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000ED2 RID: 3794 RVA: 0x000494E7 File Offset: 0x000476E7
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000ED3 RID: 3795 RVA: 0x000494F3 File Offset: 0x000476F3
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
	}

	// Token: 0x06000ED4 RID: 3796 RVA: 0x000494F5 File Offset: 0x000476F5
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		return true;
	}
}
