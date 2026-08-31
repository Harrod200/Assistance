using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200033A RID: 826
public class ClearHomeportOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000E07 RID: 3591 RVA: 0x00046CE5 File Offset: 0x00044EE5
	public override bool IsBlockingOperation()
	{
		return false;
	}

	// Token: 0x06000E08 RID: 3592 RVA: 0x00046CE8 File Offset: 0x00044EE8
	public override bool UpdatePropulsionOnComplete()
	{
		return false;
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x00046CEB File Offset: 0x00044EEB
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x00046CEE File Offset: 0x00044EEE
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.homeport != null;
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x00046D01 File Offset: 0x00044F01
	public override int SortOrder()
	{
		return 25;
	}

	// Token: 0x06000E0C RID: 3596 RVA: 0x00046D05 File Offset: 0x00044F05
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_fleet.homeport != null;
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x00046D18 File Offset: 0x00044F18
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x00046D1F File Offset: 0x00044F1F
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x00046D2D File Offset: 0x00044F2D
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x00046D39 File Offset: 0x00044F39
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_fleet.SetHomePort(null);
	}
}
