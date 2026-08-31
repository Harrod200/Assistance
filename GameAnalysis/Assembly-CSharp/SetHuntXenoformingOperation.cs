using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000305 RID: 773
public class SetHuntXenoformingOperation : TIArmyOperationTemplate
{
	// Token: 0x06000C2E RID: 3118 RVA: 0x0004006D File Offset: 0x0003E26D
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x00040070 File Offset: 0x0003E270
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Self);
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0004007C File Offset: 0x0003E27C
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return base.ActorCanPerformOperation(actorState, target) && this.OpVisibleToActor(actorState, null) && actorState.ref_army.CanTakeOffensiveAction;
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0004009F File Offset: 0x0003E29F
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_army.HumanArmy && (actorState.ref_faction.MilestoneCompleted(CampaignMilestone.DetectXenoforming) || actorState.ref_faction.MilestoneCompleted(CampaignMilestone.AlienMegafaunaSpawns)) && !actorState.ref_army.huntingXenofauna;
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x000400DC File Offset: 0x0003E2DC
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { actorState };
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x000400EA File Offset: 0x0003E2EA
	public override int SortOrder()
	{
		return 12;
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x000400EE File Offset: 0x0003E2EE
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x000400F5 File Offset: 0x0003E2F5
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		actorState.ref_army.SetHuntingXenofauna(true);
	}

	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06000C36 RID: 3126 RVA: 0x00040103 File Offset: 0x0003E303
	public override bool isConvenienceOperation
	{
		get
		{
			return true;
		}
	}
}
