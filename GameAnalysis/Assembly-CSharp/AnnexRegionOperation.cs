using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000304 RID: 772
public class AnnexRegionOperation : TIArmyOperationTemplate
{
	// Token: 0x06000C1E RID: 3102 RVA: 0x0003FE24 File Offset: 0x0003E024
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000C1F RID: 3103 RVA: 0x0003FE27 File Offset: 0x0003E027
	public override int SortOrder()
	{
		return 1;
	}

	// Token: 0x06000C20 RID: 3104 RVA: 0x0003FE2A File Offset: 0x0003E02A
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Region);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0003FE36 File Offset: 0x0003E036
	public override bool IsCombatOperation()
	{
		return true;
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0003FE3C File Offset: 0x0003E03C
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		TIArmyState ref_army = actorState.ref_army;
		TIRegionState ref_region = target.ref_region;
		return Mathf.Clamp(90f * ref_region.RegionArmyActionMultiplier(false) * ((ref_region.terrain == TerrainType.Rugged) ? 1.25f : 1f), 45f, 270f) * (ref_region.nation.militaryTechLevel / ref_army.techLevel);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0003FE9C File Offset: 0x0003E09C
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[] { TIGlobalConfig.globalConfig.armyStrengthToLiberate.ToPercent("P0") }));
		if (actorState != null)
		{
			TIArmyState ref_army = actorState.ref_army;
			if (this.ArmyCanAnnex(ref_army) && !ref_army.homeNation.claims.Contains(ref_army.currentRegion))
			{
				TINationState tinationState = TIRegionState.LiberationTarget(ref_army);
				if (tinationState != null)
				{
					stringBuilder.Append(Loc.T("AnnexRegionOperation.bonus", new object[] { tinationState.displayNameWithArticle }));
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0003FF57 File Offset: 0x0003E157
	private bool ArmyCanAnnex(TIArmyState army)
	{
		return army.armyType == ArmyType.Human && !army.InBattleWithArmies() && army.strength >= TIGlobalConfig.globalConfig.armyStrengthToLiberate && army.currentRegion.ValidRegionToAnnexOrLiberate(army);
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0003FF89 File Offset: 0x0003E189
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		return actorState.ref_army.armyType == ArmyType.Human;
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0003FF99 File Offset: 0x0003E199
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.ArmyCanAnnex(actorState.ref_army);
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0003FFA8 File Offset: 0x0003E1A8
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		if (actorState.isArmyState)
		{
			TIArmyState ref_army = actorState.ref_army;
			if (this.ArmyCanAnnex(ref_army))
			{
				return new List<TIGameState> { ref_army.currentRegion };
			}
		}
		return new List<TIGameState>();
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0003FFE4 File Offset: 0x0003E1E4
	public override bool HasResourceCost()
	{
		return true;
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x0003FFE8 File Offset: 0x0003E1E8
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost(FactionResource.Influence, 100f);
		tiresourcesCost.SetCompletionTime_Days(this.GetDuration_days(actor, target, null));
		return new List<TIResourcesCost> { tiresourcesCost };
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0004001C File Offset: 0x0003E21C
	public override void OnOperationCancel(TIGameState actorState, TIGameState target, TIDateTime opCompleteDate)
	{
		target.ref_region.EndAnnexation();
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x0004002C File Offset: 0x0003E22C
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		if (base.OperationConfirmed(actor, target, opCompleteDate))
		{
			TIArmyState ref_army = actor.ref_army;
			ref_army.currentRegion.BeginAnnexation(ref_army, this.GetDuration_days(actor, target, null));
			return true;
		}
		return false;
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x00040063 File Offset: 0x0003E263
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
	}

	// Token: 0x04000EAE RID: 3758
	public const float baselineAnnexationDuration_days = 90f;

	// Token: 0x04000EAF RID: 3759
	public const int influenceCost = 100;
}
