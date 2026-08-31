using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000308 RID: 776
public class AssaultSpaceFacilityOperation : TIArmyOperationTemplate
{
	// Token: 0x06000C55 RID: 3157 RVA: 0x00040670 File Offset: 0x0003E870
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return base.ActorCanPerformOperation(actorState, target) && !actorState.ref_army.InBattleWithArmies() && !actorState.ref_army.atSea;
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x00040699 File Offset: 0x0003E899
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x0004069C File Offset: 0x0003E89C
	public override int SortOrder()
	{
		return 3;
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x0004069F File Offset: 0x0003E89F
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_SpaceFacility);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x000406AB File Offset: 0x0003E8AB
	public override string GetSuccessHeadline(TIArmyState army, TIGameState target)
	{
		return Loc.T("AssaultSpaceFacilityOperation.success.hed", new object[] { army.displayName, target.displayName });
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x000406CF File Offset: 0x0003E8CF
	public override string GetSuccessSummary(TIArmyState army, TIGameState target)
	{
		return Loc.T("AssaultSpaceFacilityOperation.success.summary", new object[]
		{
			army.displayName,
			target.displayName,
			target.ref_region.displayName
		});
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x00040701 File Offset: 0x0003E901
	public override string GetSuccessDetail(TIArmyState army, TIGameState target)
	{
		return Loc.T("AssaultSpaceFacilityOperation.success.detail", new object[]
		{
			army.displayName,
			target.displayName,
			target.ref_region.displayName,
			target.ref_nation.displayName
		});
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x00040744 File Offset: 0x0003E944
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		if (actorState.isArmyState)
		{
			TIArmyState ref_army = actorState.ref_army;
			if (ref_army.CanTakeOffensiveAction && (ref_army.currentRegion.PartofOccupyingAlliance(ref_army.homeNation) || (ref_army.homeNation.civilWar && ref_army.currentNation == ref_army.homeNation)))
			{
				foreach (TIRegionSpaceFacilityState tiregionSpaceFacilityState in ref_army.currentRegion.spaceFacilities)
				{
					if (tiregionSpaceFacilityState.Extant() && !tiregionSpaceFacilityState.UnderArmyAssault())
					{
						list.Add(tiregionSpaceFacilityState);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x00040800 File Offset: 0x0003EA00
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 7f;
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x00040807 File Offset: 0x0003EA07
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		target.ref_region.DestroySpaceFacility(target.ref_regionSpaceFacility.spaceFacilityType, true);
		TINotificationQueueState.LogArmyCompletesOperation(actorState.ref_army, this, target, TIMissionOutcome.Success, "");
	}
}
