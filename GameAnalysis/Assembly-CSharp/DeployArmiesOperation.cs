using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002FF RID: 767
public class DeployArmiesOperation : TIArmyOperationTemplate
{
	// Token: 0x1700017F RID: 383
	// (get) Token: 0x06000BE9 RID: 3049 RVA: 0x0003F68D File Offset: 0x0003D88D
	public override bool isConvenienceOperation
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000BEA RID: 3050 RVA: 0x0003F690 File Offset: 0x0003D890
	public DeployArmiesOperation(bool allowJournies_ = false)
	{
		this.allowJournies = allowJournies_;
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x0003F69F File Offset: 0x0003D89F
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x0003F6A2 File Offset: 0x0003D8A2
	public override int SortOrder()
	{
		return 0;
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x0003F6A5 File Offset: 0x0003D8A5
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000BEE RID: 3054 RVA: 0x0003F6B1 File Offset: 0x0003D8B1
	public override bool IsCombatOperation()
	{
		return false;
	}

	// Token: 0x06000BEF RID: 3055 RVA: 0x0003F6B4 File Offset: 0x0003D8B4
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return this.GetPossibleTargets(actorState, null).Count > 0 && DeployArmiesOperation.GetEligibleArmies(actorState.ref_army).Count > 1;
	}

	// Token: 0x06000BF0 RID: 3056 RVA: 0x0003F6DB File Offset: 0x0003D8DB
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return actorState.ref_army.GetDeploymentToAdjacentRegionDuration_Days(target.ref_region);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x0003F6EE File Offset: 0x0003D8EE
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return DeployArmyOperation_OpenTarget.GetPossibleTargets(actorState, this.allowJournies);
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x0003F6FC File Offset: 0x0003D8FC
	public static List<TIArmyState> GetEligibleArmies(TIArmyState army)
	{
		List<TIArmyState> enemyArmiesInRegion = army.GetEnemyArmiesInRegion();
		ArmySeaTransitStage seaTransitStage = army.SeaTransitStage();
		Func<TIArmyState, bool> OperationsAreEquivalent = delegate(TIArmyState otherArmy)
		{
			if (otherArmy.CurrentOperations().Count != army.CurrentOperations().Count)
			{
				return false;
			}
			if ((from x in otherArmy.CurrentOperations().Concat<OperationData>(army.CurrentOperations())
				select new ValueTuple<string, TIGameState, TIDateTime>(x.operationDataName, x.target, x.startDate)).Distinct<ValueTuple<string, TIGameState, TIDateTime>>().Count<ValueTuple<string, TIGameState, TIDateTime>>() != army.CurrentOperations().Count)
			{
				return false;
			}
			if (otherArmy.destinationQueue.Count != army.destinationQueue.Count)
			{
				return false;
			}
			for (int i = 0; i < army.destinationQueue.Count; i++)
			{
				if (army.destinationQueue[i] != otherArmy.destinationQueue[i])
				{
					return false;
				}
			}
			return true;
		};
		return (from x in army.ref_region.armies.Except<TIArmyState>(enemyArmiesInRegion)
			where x.faction == army.ref_faction && x.SeaTransitStage() == seaTransitStage && OperationsAreEquivalent(x)
			select x).ToList<TIArmyState>();
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x0003F774 File Offset: 0x0003D974
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		if (actor.isArmyState)
		{
			foreach (TIArmyState tiarmyState in (from x in DeployArmiesOperation.GetEligibleArmies(actor.ref_army)
				where this.allowJournies || TIArmyState.OneStepValidDestinationRegions(x, x.currentRegion, x.IsMoving).Contains(target)
				select x).ToList<TIArmyState>())
			{
				new DeployArmyOperation_OpenTarget(this.allowJournies).OnOperationConfirm(tiarmyState, target, null, null);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x0003F814 File Offset: 0x0003DA14
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
	}

	// Token: 0x04000EAD RID: 3757
	private readonly bool allowJournies;
}
