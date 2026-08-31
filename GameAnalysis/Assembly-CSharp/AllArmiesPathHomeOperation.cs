using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000303 RID: 771
public class AllArmiesPathHomeOperation : TIArmyOperationTemplate
{
	// Token: 0x06000C12 RID: 3090 RVA: 0x0003FC50 File Offset: 0x0003DE50
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000C13 RID: 3091 RVA: 0x0003FC53 File Offset: 0x0003DE53
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x06000C14 RID: 3092 RVA: 0x0003FC56 File Offset: 0x0003DE56
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000C15 RID: 3093 RVA: 0x0003FC62 File Offset: 0x0003DE62
	public override bool IsCombatOperation()
	{
		return false;
	}

	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000C16 RID: 3094 RVA: 0x0003FC65 File Offset: 0x0003DE65
	public override bool isConvenienceOperation
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000C17 RID: 3095 RVA: 0x0003FC68 File Offset: 0x0003DE68
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 0f;
	}

	// Token: 0x06000C18 RID: 3096 RVA: 0x0003FC70 File Offset: 0x0003DE70
	private List<TIArmyState> EligibleArmies(TIArmyState army)
	{
		List<TIArmyState> list = new List<TIArmyState>();
		DeployArmyOperation_TargetHome deployArmyOperation_TargetHome = new DeployArmyOperation_TargetHome();
		deployArmyOperation_TargetHome.SetJourneyMode(true);
		foreach (TIArmyState tiarmyState in army.currentRegion.armies)
		{
			if (tiarmyState.faction == army.faction && tiarmyState.currentOperations.Count == 0 && deployArmyOperation_TargetHome.ActorCanPerformOperation(tiarmyState, tiarmyState.homeRegion))
			{
				list.Add(tiarmyState);
			}
		}
		return list;
	}

	// Token: 0x06000C19 RID: 3097 RVA: 0x0003FD0C File Offset: 0x0003DF0C
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		List<TIArmyState> list = this.EligibleArmies(actorState.ref_army);
		return list.Count > 1 && list.Contains(actorState.ref_army);
	}

	// Token: 0x06000C1A RID: 3098 RVA: 0x0003FD3D File Offset: 0x0003DF3D
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.OpVisibleToActor(actorState, target);
	}

	// Token: 0x06000C1B RID: 3099 RVA: 0x0003FD48 File Offset: 0x0003DF48
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		if (actorState.isArmyState && actorState.ref_army.homeRegion != null)
		{
			TIArmyState ref_army = actorState.ref_army;
			return new List<TIGameState> { ref_army.homeRegion.ref_gameState };
		}
		return new List<TIGameState>();
	}

	// Token: 0x06000C1C RID: 3100 RVA: 0x0003FD94 File Offset: 0x0003DF94
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (actorState.isArmyState)
		{
			DeployArmyOperation_OpenTarget deployArmyOperation_OpenTarget = new DeployArmyOperation_OpenTarget(false);
			deployArmyOperation_OpenTarget.SetJourneyMode(true);
			foreach (TIArmyState tiarmyState in this.EligibleArmies(actorState.ref_army))
			{
				if (deployArmyOperation_OpenTarget.ActorCanPerformOperation(tiarmyState, tiarmyState.homeRegion))
				{
					deployArmyOperation_OpenTarget.OnOperationConfirm(tiarmyState, tiarmyState.homeRegion, null, null);
				}
			}
		}
	}
}
