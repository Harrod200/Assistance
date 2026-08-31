using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000333 RID: 819
public class CancelContinuousBombardXenoformingOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000DB0 RID: 3504 RVA: 0x00043EE2 File Offset: 0x000420E2
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000DB1 RID: 3505 RVA: 0x00043EE5 File Offset: 0x000420E5
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000DB2 RID: 3506 RVA: 0x00043EF1 File Offset: 0x000420F1
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_fleet.huntingXenofauna;
	}

	// Token: 0x06000DB3 RID: 3507 RVA: 0x00043EFE File Offset: 0x000420FE
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.huntingXenofauna;
	}

	// Token: 0x06000DB4 RID: 3508 RVA: 0x00043F0B File Offset: 0x0004210B
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000DB5 RID: 3509 RVA: 0x00043F19 File Offset: 0x00042119
	public override int SortOrder()
	{
		return 15;
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x00043F1D File Offset: 0x0004211D
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x00043F24 File Offset: 0x00042124
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_fleet.SetHuntingXenofauna(false, false);
	}
}
