using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000306 RID: 774
public class CancelHuntXenoformingOperation : TIArmyOperationTemplate
{
	// Token: 0x06000C38 RID: 3128 RVA: 0x0004010E File Offset: 0x0003E30E
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x00040111 File Offset: 0x0003E311
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0004011D File Offset: 0x0003E31D
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_army.huntingXenofauna;
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0004012A File Offset: 0x0003E32A
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_army.huntingXenofauna;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x00040137 File Offset: 0x0003E337
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x00040145 File Offset: 0x0003E345
	public override int SortOrder()
	{
		return 12;
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00040149 File Offset: 0x0003E349
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000C3F RID: 3135 RVA: 0x00040150 File Offset: 0x0003E350
	public override bool isConvenienceOperation
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x00040153 File Offset: 0x0003E353
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_army.SetHuntingXenofauna(false);
	}
}
