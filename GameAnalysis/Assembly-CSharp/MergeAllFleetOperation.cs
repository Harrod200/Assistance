using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000325 RID: 805
public class MergeAllFleetOperation : MergeFleetOperation
{
	// Token: 0x06000D17 RID: 3351 RVA: 0x00042240 File Offset: 0x00040440
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000D18 RID: 3352 RVA: 0x0004224C File Offset: 0x0004044C
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return this.ActorCanPerformOperation(actorState, targetState) && base.GetPossibleTargets(actorState, null).Count > 1;
	}

	// Token: 0x06000D19 RID: 3353 RVA: 0x0004226A File Offset: 0x0004046A
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000D1A RID: 3354 RVA: 0x00042278 File Offset: 0x00040478
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		List<TIGameState> possibleTargets = base.GetPossibleTargets(actorState, null);
		MergeFleetOperation mergeFleetOperation = new MergeFleetOperation();
		foreach (TIGameState tigameState in possibleTargets)
		{
			mergeFleetOperation.ExecuteOperation(actorState, tigameState);
		}
	}
}
