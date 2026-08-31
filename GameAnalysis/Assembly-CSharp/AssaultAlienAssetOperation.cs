using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000307 RID: 775
public class AssaultAlienAssetOperation : TIArmyOperationTemplate, IContestedOperation
{
	// Token: 0x06000C42 RID: 3138 RVA: 0x00040169 File Offset: 0x0003E369
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x0004016C File Offset: 0x0003E36C
	public override int SortOrder()
	{
		return 2;
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x0004016F File Offset: 0x0003E36F
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_AlienAsset);
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x0004017B File Offset: 0x0003E37B
	public string targetStr(TIGameState target)
	{
		if (target.isRegionXenoformingState)
		{
			return "xenoforming";
		}
		if (target.isRegionAlienFacility)
		{
			return "facility";
		}
		if (target.isRegionLandedUFO)
		{
			return "landedUFO";
		}
		return string.Empty;
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x000401AC File Offset: 0x0003E3AC
	public override string GetSuccessHeadline(TIArmyState army, TIGameState target)
	{
		return Loc.T(new StringBuilder("AssaultAlienAssetOperation.").Append(this.targetStr(target)).Append(".success.hed").ToString(), new object[] { army.displayName });
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x000401E7 File Offset: 0x0003E3E7
	public override string GetFailureHeadline(TIArmyState army, TIGameState target)
	{
		return Loc.T(new StringBuilder("AssaultAlienAssetOperation.").Append(this.targetStr(target)).Append(".failure.hed").ToString(), new object[] { army.displayName });
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00040224 File Offset: 0x0003E424
	public override string GetSuccessSummary(TIArmyState army, TIGameState target)
	{
		return Loc.T(new StringBuilder("AssaultAlienAssetOperation.").Append(this.targetStr(target)).Append(".success.summary").ToString(), new object[]
		{
			army.displayNameWithArticleCapitalized,
			target.ref_region.displayName
		});
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00040278 File Offset: 0x0003E478
	public override string GetFailureSummary(TIArmyState army, TIGameState target)
	{
		return Loc.T(new StringBuilder("AssaultAlienAssetOperation.").Append(this.targetStr(target)).Append(".failure.summary").ToString(), new object[]
		{
			army.displayNameWithArticleCapitalized,
			target.ref_region.displayName
		});
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x000402CC File Offset: 0x0003E4CC
	public override string GetSuccessDetail(TIArmyState army, TIGameState target)
	{
		return Loc.T(new StringBuilder("AssaultAlienAssetOperation.").Append(this.targetStr(target)).Append(".success.detail").ToString(), new object[]
		{
			army.displayNameWithArticleCapitalized,
			target.ref_region.displayName
		});
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00040320 File Offset: 0x0003E520
	public override string GetFailureDetail(TIArmyState army, TIGameState target)
	{
		return Loc.T(new StringBuilder("AssaultAlienAssetOperation.").Append(this.targetStr(target)).Append(".failure.detail").ToString(), new object[]
		{
			army.displayNameWithArticleCapitalized,
			target.ref_region.displayName
		});
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00040374 File Offset: 0x0003E574
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		return (!(ref_faction != null) || (!ref_faction.IsAlienFaction && !ref_faction.IsAlienProxy)) && base.OpVisibleToActor(actorState, targetState);
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x000403AB File Offset: 0x0003E5AB
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return base.ActorCanPerformOperation(actorState, target) && !actorState.ref_army.InBattleWithArmies() && !actorState.ref_army.atSea;
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x000403D4 File Offset: 0x0003E5D4
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		List<TIGameState> list = new List<TIGameState>();
		TIArmyState ref_army = actorState.ref_army;
		if (ref_army != null && ref_army.CanTakeOffensiveAction)
		{
			foreach (TIRegionAlienAssetState tiregionAlienAssetState in ref_army.currentRegion.alienAssets)
			{
				if (tiregionAlienAssetState.Extant() && ref_army.faction != null && tiregionAlienAssetState.VisibleToFaction(ref_army.faction) && !tiregionAlienAssetState.UnderArmyAssault() && ((!tiregionAlienAssetState.isRegionLandedUFO && !tiregionAlienAssetState.isRegionAlienFacility) || !GameStateManager.AlienNation().extant || !actorState.ref_army.homeNation.allies.Contains(GameStateManager.AlienNation())))
				{
					list.Add(tiregionAlienAssetState);
				}
			}
		}
		return list;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00040496 File Offset: 0x0003E696
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return 7f;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x000404A0 File Offset: 0x0003E6A0
	public float GetSuccessChance(TIGameState actor, TIGameState defender)
	{
		float num = actor.ref_army.adjustedTechLevel * actor.ref_army.strength;
		float armyAssaultDefenseScore = defender.ref_regionAlienAsset.GetArmyAssaultDefenseScore();
		float num2 = num - armyAssaultDefenseScore;
		float num3 = 0.5f * Mathf.Pow(0.775f, Mathf.Abs(num2));
		if (num2 >= 0f)
		{
			num3 = 1f - num3;
		}
		return num3;
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x000404FC File Offset: 0x0003E6FC
	public override bool OperationConfirmed(TIGameState actor, TIGameState target, TIDateTime opCompleteDate)
	{
		if (!actor.ref_army.homeNation.IsAtWarWith(GameStateManager.AlienNation()) && (target.isRegionLandedUFO || (target.isRegionAlienFacility && GameStateManager.AlienNation().extant)) && !actor.ref_army.AlienMegafaunaArmy)
		{
			GameStateManager.AlienNation().DeclareFullWar(GameStateManager.AlienFaction(), actor.ref_army.homeNation);
			TINotificationQueueState.LogPolicyAdopted(PolicyManager.policies[PolicyType.WarOption] as TIPolicyOption, GameStateManager.AlienNation(), actor.ref_army.homeNation, null, 1, "", "");
		}
		return base.OperationConfirmed(actor, target, opCompleteDate);
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x000405A0 File Offset: 0x0003E7A0
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		if (target.ref_regionAlienEntity.Extant())
		{
			TIArmyState ref_army = actorState.ref_army;
			float successChance = this.GetSuccessChance(actorState, target);
			float num = TIUtilities.RandomFloatValue();
			TIMissionOutcome timissionOutcome;
			if (num <= successChance / 10f)
			{
				timissionOutcome = TIMissionOutcome.CriticalSuccess;
			}
			else if (num <= successChance)
			{
				timissionOutcome = TIMissionOutcome.Success;
			}
			else
			{
				float num2 = 1f - (1f - successChance) / 10f;
				if (num >= num2)
				{
					timissionOutcome = TIMissionOutcome.CriticalFailure;
				}
				else
				{
					timissionOutcome = TIMissionOutcome.Failure;
				}
			}
			if (target.isRegionXenoformingState)
			{
				timissionOutcome = ref_army.AssaultAlienAsset(target.ref_xenoforming, timissionOutcome);
			}
			else if (target.isRegionLandedUFO)
			{
				timissionOutcome = ref_army.AssaultAlienAsset(target.ref_UFOLanding, timissionOutcome);
			}
			else if (target.isRegionAlienFacility)
			{
				timissionOutcome = ref_army.AssaultAlienAsset(target.ref_alienFacility, timissionOutcome);
			}
			if (!ref_army.deleted)
			{
				TINotificationQueueState.LogArmyCompletesOperation(ref_army, this, target, timissionOutcome, "");
			}
		}
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x00040665 File Offset: 0x0003E865
	public override bool Repeatable()
	{
		return true;
	}
}
