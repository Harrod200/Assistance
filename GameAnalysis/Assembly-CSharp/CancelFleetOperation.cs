using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200030B RID: 779
public class CancelFleetOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000C7A RID: 3194 RVA: 0x00040C3C File Offset: 0x0003EE3C
	public override int SortOrder()
	{
		return 99;
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00040C40 File Offset: 0x0003EE40
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_fleet.CurrentOperations().Any<OperationData>((OperationData x) => (x.operation as TISpaceFleetOperationTemplate).CanCancel());
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x00040C71 File Offset: 0x0003EE71
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.OpVisibleToActor(actorState, target);
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x00040C7B File Offset: 0x0003EE7B
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		return true;
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x00040C80 File Offset: 0x0003EE80
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		foreach (OperationData operationData in actorState.ref_fleet.CurrentOperations())
		{
			if ((operationData.operation as TISpaceFleetOperationTemplate).CanCancel())
			{
				actorState.ref_fleet.CancelOperation(operationData);
			}
		}
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x00040CF0 File Offset: 0x0003EEF0
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00040CF3 File Offset: 0x0003EEF3
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00040CFF File Offset: 0x0003EEFF
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x00040D06 File Offset: 0x0003EF06
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}
}
