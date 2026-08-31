using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000309 RID: 777
public class RazeRegionOperation : TIArmyOperationTemplate
{
	// Token: 0x06000C60 RID: 3168 RVA: 0x0004083B File Offset: 0x0003EA3B
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return base.ActorCanPerformOperation(actorState, target) && !actorState.ref_army.InBattleWithArmies() && !actorState.ref_army.atSea;
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x00040864 File Offset: 0x0003EA64
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x00040867 File Offset: 0x0003EA67
	public override int SortOrder()
	{
		return 4;
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x0004086A File Offset: 0x0003EA6A
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x00040876 File Offset: 0x0003EA76
	public override string GetSuccessHeadline(TIArmyState army, TIGameState target)
	{
		return Loc.T("RazeRegionOperation.success.hed", new object[] { army.displayName, target.displayName });
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0004089A File Offset: 0x0003EA9A
	public override string GetSuccessSummary(TIArmyState army, TIGameState target)
	{
		return Loc.T("RazeRegionOperation.success.summary", new object[] { army.displayName, target.displayName });
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x000408BE File Offset: 0x0003EABE
	public override string GetSuccessDetail(TIArmyState army, TIGameState target)
	{
		return Loc.T("RazeRegionOperation.success.detail", new object[] { army.displayName, target.displayName });
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x000408E4 File Offset: 0x0003EAE4
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		if (actorState.isArmyState)
		{
			TIArmyState ref_army = actorState.ref_army;
			if (ref_army.CanTakeOffensiveAction && (ref_army.currentRegion.PartofOccupyingAlliance(ref_army.homeNation) || (ref_army.homeNation.civilWar && ref_army.currentNation == ref_army.homeNation)))
			{
				list.Add(ref_army.currentRegion);
			}
		}
		return list;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x0004094E File Offset: 0x0003EB4E
	public override bool WarnTarget(TIGameState target)
	{
		return true;
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x00040951 File Offset: 0x0003EB51
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 28f;
	}

	// Token: 0x06000C6A RID: 3178 RVA: 0x00040958 File Offset: 0x0003EB58
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TIArmyState ref_army = actorState.ref_army;
		TIRegionState ref_region = target.ref_region;
		if (!ref_region.nation.alienNation)
		{
			TIFactionState faction = ref_army.faction;
			if (faction != null)
			{
				faction.CommitAtrocity(5, TIFactionState.AtrocityCause.ArmyRazeHumanNationRegions, false, 0.333f);
			}
		}
		ref_region.ApplyDamageToRegion(ref_army.techLevel / 10f * ref_army.strength, ref_army.faction, ref_army.homeNation, true, false, true, false);
		TINotificationQueueState.LogArmyCompletesOperation(ref_army, this, target, TIMissionOutcome.Success, "");
	}

	// Token: 0x06000C6B RID: 3179 RVA: 0x000409CE File Offset: 0x0003EBCE
	public override bool Repeatable()
	{
		return true;
	}
}
