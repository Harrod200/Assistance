using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000347 RID: 839
public abstract class FoundHabOperation : TISpaceBodyOperationTemplate
{
	// Token: 0x06000E87 RID: 3719 RVA: 0x00048CB3 File Offset: 0x00046EB3
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.InstantExecution;
	}

	// Token: 0x06000E88 RID: 3720
	public abstract int GetTier();

	// Token: 0x06000E89 RID: 3721
	public abstract Context GetRequiredConstructionTechEffectContext();

	// Token: 0x06000E8A RID: 3722
	public abstract string CoreModuleDataName(bool alien);

	// Token: 0x06000E8B RID: 3723 RVA: 0x00048CB6 File Offset: 0x00046EB6
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x00048CBD File Offset: 0x00046EBD
	public TIHabModuleTemplate CoreModule(bool alien)
	{
		return TemplateManager.Find<TIHabModuleTemplate>(this.CoreModuleDataName(alien), false);
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x00048CCC File Offset: 0x00046ECC
	public void FoundHab(TIFactionState faction, TIGameState location, int tier)
	{
		TIHabState tihabState = GameStateManager.CreateNewGameState<TIHabState>();
		tihabState.InitializeNewHab(faction, location, faction, tier, this.deliveryDuration_days, null);
		FactionGoal_FoundHab factionGoal_FoundHab = (from x in faction.AllFoundHabGoals(true)
			where x.location() == location
			select x).FirstOrDefault<TIFactionGoalState>() as FactionGoal_FoundHab;
		if (factionGoal_FoundHab != null)
		{
			factionGoal_FoundHab.SetHab(tihabState);
		}
		TINotificationQueueState.LogHabFounded(faction, tihabState, location);
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x00048D40 File Offset: 0x00046F40
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return actorState.ref_faction.AvailableMissionControl >= -this.CoreModule(actorState.ref_faction.IsAlienFaction).missionControl && target.ref_naturalSpaceObject.maxHabTier >= Mathf.Abs(this.GetTier()) && this.GetPossibleTargets(actorState, target).Any<TIGameState>((TIGameState x) => this.ResourceCostOptions(actorState.ref_faction, x, actorState, true).Count > 0);
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x00048DCC File Offset: 0x00046FCC
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		List<TIGameState> possibleTargets = this.GetPossibleTargets(actorState, target);
		StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString(), new object[]
		{
			-this.CoreModule(ref_faction.IsAlienFaction).missionControl,
			TemplateManager.global.missionControlInlineSpritePath
		}));
		List<TIResourcesCost> list = new List<TIResourcesCost>();
		new List<TIResourcesCost>();
		if (ref_faction.IsActiveHumanFaction || GameStateManager.AlienNation().extant)
		{
			TIHabModuleTemplate tihabModuleTemplate = this.CoreModule(ref_faction.IsAlienFaction);
			foreach (TIGameState tigameState in possibleTargets)
			{
				if (tihabModuleTemplate.CostFromEarth(ref_faction, tigameState, false).completionTime_days <= TemplateManager.global.maxHabBoostFromEarthDuration_days)
				{
					list.Add(tihabModuleTemplate.CostFromEarth(ref_faction, tigameState, false));
				}
			}
			list = list.OrderBy<TIResourcesCost, float>((TIResourcesCost x) => x.GetSingleCostValue(FactionResource.Boost)).ToList<TIResourcesCost>();
			if (list.Count > 0)
			{
				stringBuilder.Append(Loc.T("UI.Operations.MinimumCostEarth", new object[] { list[0].ToString("Relevant", false, false, actorState.ref_faction, false, FactionResource.None) }));
			}
			else
			{
				stringBuilder.Append(Loc.T("UI.Operations.CantReachFromEarth", new object[] { TemplateManager.global.maxHabBoostFromEarthDuration_days }));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x00048F74 File Offset: 0x00047174
	public override bool HasResourceCost()
	{
		return true;
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x00048F78 File Offset: 0x00047178
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		List<TIResourcesCost> list = new List<TIResourcesCost>();
		TIHabModuleTemplate tihabModuleTemplate = this.CoreModule(faction.IsAlienFaction);
		if (faction.IsActiveHumanFaction || GameStateManager.AlienNation().extant)
		{
			TIResourcesCost tiresourcesCost = tihabModuleTemplate.CostFromEarth(faction, target, false);
			if ((!checkCanAfford || tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity)) && tiresourcesCost.completionTime_days <= TemplateManager.global.maxHabBoostFromEarthDuration_days)
			{
				list.Add(tiresourcesCost);
			}
		}
		if (faction.MaxTierCanFoundAtLocation(target, false, false) >= Mathf.Abs(this.GetTier()))
		{
			TIResourcesCost tiresourcesCost2 = tihabModuleTemplate.CostFromSpace(faction, target, false, true, 0, false);
			if (!checkCanAfford || tiresourcesCost2.CanAfford(faction, 1f, null, float.PositiveInfinity))
			{
				list.Add(tiresourcesCost2);
			}
		}
		if (list.Count > 1)
		{
			list = list.OrderBy<TIResourcesCost, float>((TIResourcesCost x) => x.completionTime_days).ToList<TIResourcesCost>();
		}
		return list;
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00049060 File Offset: 0x00047260
	public static TIResourcesCost GetCostFromSpace(TIGameState location, TIFactionState faction, bool substituteBoost = true)
	{
		FoundHabOperation foundHabOperation;
		if (location.isHabSiteState)
		{
			foundHabOperation = new FoundOutpostOperation();
		}
		else
		{
			foundHabOperation = new FoundPlatformOperation();
		}
		return foundHabOperation.CoreModule(faction.IsAlienFaction).CostFromSpace(faction, location, false, substituteBoost, 0, false);
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0004909C File Offset: 0x0004729C
	public static TIResourcesCost GetCostFromEarth(TIGameState location, TIFactionState faction, bool substituteBoost = true)
	{
		FoundHabOperation foundHabOperation;
		if (location.isHabSiteState)
		{
			foundHabOperation = new FoundOutpostOperation();
		}
		else
		{
			foundHabOperation = new FoundPlatformOperation();
		}
		return foundHabOperation.CoreModule(faction.IsAlienFaction).CostFromEarth(faction, location, false);
	}

	// Token: 0x04000EBC RID: 3772
	protected float deliveryDuration_days;
}
