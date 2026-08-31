using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000332 RID: 818
public class SetContinuousBombardXenoformingOperation : TISpaceFleetOperationTemplate
{
	// Token: 0x06000DA7 RID: 3495 RVA: 0x00043E61 File Offset: 0x00042061
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000DA8 RID: 3496 RVA: 0x00043E64 File Offset: 0x00042064
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000DA9 RID: 3497 RVA: 0x00043E70 File Offset: 0x00042070
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return base.ActorCanPerformOperation(actorState, target) && this.OpVisibleToActor(actorState, null);
	}

	// Token: 0x06000DAA RID: 3498 RVA: 0x00043E86 File Offset: 0x00042086
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return !actorState.ref_fleet.dockedOrLanded && actorState.ref_fleet.CanHuntXenofauna() && !actorState.ref_fleet.huntingXenofauna;
	}

	// Token: 0x06000DAB RID: 3499 RVA: 0x00043EB2 File Offset: 0x000420B2
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000DAC RID: 3500 RVA: 0x00043EC0 File Offset: 0x000420C0
	public override int SortOrder()
	{
		return 15;
	}

	// Token: 0x06000DAD RID: 3501 RVA: 0x00043EC4 File Offset: 0x000420C4
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000DAE RID: 3502 RVA: 0x00043ECB File Offset: 0x000420CB
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_fleet.SetHuntingXenofauna(true, false);
	}
}
