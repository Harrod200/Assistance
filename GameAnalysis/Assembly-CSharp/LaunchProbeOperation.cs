using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000342 RID: 834
public class LaunchProbeOperation : TISpaceBodyOperationTemplate
{
	// Token: 0x06000E61 RID: 3681 RVA: 0x00047FCB File Offset: 0x000461CB
	public override OperationTiming GetOperationTiming()
	{
		return OperationTiming.DelayedExecutionOfInstantEffect;
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00047FCE File Offset: 0x000461CE
	public override int SortOrder()
	{
		return 0;
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00047FD1 File Offset: 0x000461D1
	public override Type GetTargetingMethod()
	{
		return typeof(TIOperationTargeting_Presupplied);
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00047FDD File Offset: 0x000461DD
	public override bool UseResourceCostDuration()
	{
		return true;
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x00047FE0 File Offset: 0x000461E0
	public override string GetDescription(TIGameState actorState = null, TIGameState target = null)
	{
		StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder(base.GetType().Name).Append(".description").ToString()));
		TIResourcesCost tiresourcesCost = this.EarthCost(actorState.ref_faction, target);
		TIResourcesCost tiresourcesCost2 = this.SpaceCost(actorState.ref_faction, target);
		if (tiresourcesCost2.anyDebit)
		{
			stringBuilder.Append(Loc.T("UI.Operations.CostBoth", new object[]
			{
				tiresourcesCost.GetString("Relevant", false, false, false, 7, false, false, actorState.ref_faction, false, FactionResource.None),
				tiresourcesCost2.GetString("Relevant", false, false, false, 7, false, false, actorState.ref_faction, false, FactionResource.None)
			}));
		}
		else
		{
			stringBuilder.Append(Loc.T("UI.Operations.CostEarth", new object[] { tiresourcesCost.GetString("Relevant", false, false, false, 7, false, false, actorState.ref_faction, false, FactionResource.None) }));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x000480C5 File Offset: 0x000462C5
	private float probeBasePayloadMass_tons(TISpaceBodyState body)
	{
		return (TemplateManager.global.probePayloadBaseline_tons + TemplateManager.global.probePayloadPerHabSite_tons * (float)body.habSites.Length) * (1f + 0.5f * (body.irradiatedMultiplier - 1f));
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00048100 File Offset: 0x00046300
	private float ScanDuration_days(TIFactionState faction, TISpaceBodyState target)
	{
		string effect = target.template.effectToExplore;
		Func<TIEffectTemplate, bool> <>9__1;
		TITechTemplate titechTemplate = TemplateManager.IterateByClass<TITechTemplate>(true).FirstOrDefault<TITechTemplate>(delegate(TITechTemplate x)
		{
			IEnumerable<TIEffectTemplate> effects = x.Effects;
			Func<TIEffectTemplate, bool> func;
			if ((func = <>9__1) == null)
			{
				func = (<>9__1 = (TIEffectTemplate x) => x.dataName == effect);
			}
			return effects.Any<TIEffectTemplate>(func);
		});
		float num = 1f;
		if (titechTemplate != null && faction.techContributionHistory.ContainsKey(titechTemplate))
		{
			num = 1f - faction.techContributionHistory[titechTemplate];
		}
		return Mathf.Max(1f, (float)(1 + target.habSites.Length - target.occupiedHabSites.Count) * num);
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x0004818C File Offset: 0x0004638C
	public TIResourcesCost SpaceCost(TIFactionState faction, TIGameState target)
	{
		TIHabState tihabState;
		float num = (float)TISpaceObjectState.GenericTransferTimeFromNearestHab_d(faction, target.ref_spaceBody.interfaceOrbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km), TISpaceObjectState.HabClassification.Shipyard, out tihabState);
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		if (tihabState != null)
		{
			double num2 = TISpaceObjectState.GenericTransferDeltaV_mps(tihabState, target.ref_spaceBody.interfaceOrbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km), false);
			float num3 = this.probeBasePayloadMass_tons(target.ref_spaceBody);
			double num4 = (double)(TISpaceObjectState.ModifiedGenericTransferEV_kps(faction) * 1000f);
			float num5 = (float)((double)num3 * Mathd.Exp(num2 / num4)) - num3;
			tiresourcesCost.ConstructCost(new ResourceValue[]
			{
				new ResourceValue
				{
					resource = FactionResource.Metals,
					value = num3 * TemplateManager.global.spaceResourceToTons * TemplateManager.global.probeMetalsPayloadMassFraction
				},
				new ResourceValue
				{
					resource = FactionResource.Volatiles,
					value = num3 * TemplateManager.global.spaceResourceToTons * TemplateManager.global.probeVolatilesPayloadMassFraction + num5 * TemplateManager.global.spaceResourceToTons * TemplateManager.global.probeVolatilesPropellantMassFraction
				},
				new ResourceValue
				{
					resource = FactionResource.Water,
					value = num5 * TemplateManager.global.spaceResourceToTons * TemplateManager.global.probeWaterPropellantMassFraction
				},
				new ResourceValue
				{
					resource = FactionResource.NobleMetals,
					value = num3 * TemplateManager.global.spaceResourceToTons * TemplateManager.global.probeNoblesPayloadMassFraction
				},
				new ResourceValue
				{
					resource = FactionResource.Fissiles,
					value = num3 * TemplateManager.global.spaceResourceToTons * TemplateManager.global.probeFissilesPayloadMassFraction
				}
			});
			float num6 = this.ScanDuration_days(faction, target.ref_spaceBody);
			tiresourcesCost.SetCompletionTime_Days(TemplateManager.global.probeConstructionTime_d + num + TIEffectsState.SumEffectsModifiers(Context.ProbeTransferTime, faction, num, null) + num6);
		}
		return tiresourcesCost;
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x000483C4 File Offset: 0x000465C4
	public TIResourcesCost EarthCost(TIFactionState faction, TIGameState target)
	{
		float num = this.probeBasePayloadMass_tons(target.ref_spaceBody);
		float num2 = (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, target.ref_spaceBody.interfaceOrbits.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km), num);
		float num3 = num * TemplateManager.global.spaceResourceToTons;
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.ConstructCost(new ResourceValue[]
		{
			new ResourceValue
			{
				resource = FactionResource.Boost,
				value = num2
			},
			new ResourceValue
			{
				resource = FactionResource.Money,
				value = num3 * TemplateManager.global.probeMetalsPayloadMassFraction * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Metals) + num3 * TemplateManager.global.probeVolatilesPayloadMassFraction * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Volatiles) + num3 * TemplateManager.global.probeFissilesPayloadMassFraction * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Fissiles) + num3 * TemplateManager.global.probeNoblesPayloadMassFraction * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.NobleMetals)
			}
		});
		float num4 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, target);
		num4 += TIEffectsState.SumEffectsModifiers(Context.ProbeTransferTime, faction, num4, null);
		float num5 = this.ScanDuration_days(faction, target.ref_spaceBody);
		tiresourcesCost.SetCompletionTime_Days(TemplateManager.global.probeConstructionTime_d + num4 + num5);
		return tiresourcesCost;
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x00048520 File Offset: 0x00046720
	public override bool OpVisibleToActor(TIGameState actorState, TIGameState targetState = null)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		return !ref_faction.IsAlienFaction && targetState.isSpaceBodyState && ref_faction.CanProspectWithProbe(targetState.ref_spaceBody, false);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00048553 File Offset: 0x00046753
	public override bool ActorCanPerformOperation(TIGameState actorState, TIGameState target)
	{
		return this.ResourceCostOptions(actorState.ref_faction, target, actorState, true).Count > 0;
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x0004856C File Offset: 0x0004676C
	public override bool HasResourceCost()
	{
		return true;
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00048570 File Offset: 0x00046770
	public override List<TIResourcesCost> ResourceCostOptions(TIFactionState faction, TIGameState target, TIGameState actor, bool checkCanAfford = true)
	{
		List<TIResourcesCost> list = new List<TIResourcesCost>();
		if (faction.ShipConstructionHabs(false, false).Count > 0)
		{
			TIResourcesCost tiresourcesCost = this.SpaceCost(faction, target);
			if (!checkCanAfford || tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity))
			{
				list.Add(tiresourcesCost);
			}
		}
		TIResourcesCost tiresourcesCost2 = this.EarthCost(faction, target);
		if (!checkCanAfford || tiresourcesCost2.CanAfford(faction, 1f, null, float.PositiveInfinity))
		{
			list.Add(tiresourcesCost2);
		}
		if (list.Count > 1)
		{
			list = (from x in list
				orderby x.completionTime_days, x.GetSingleCostValue(FactionResource.Boost)
				select x).ToList<TIResourcesCost>();
		}
		return list;
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x0004863C File Offset: 0x0004683C
	public override float GetDuration_days(TIGameState actorState, TIGameState target, Trajectory trajectory = null)
	{
		return -1f;
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00048643 File Offset: 0x00046843
	public override List<TIGameState> GetPossibleTargets(TIGameState actorState, TIGameState defaultTarget = null)
	{
		return new List<TIGameState> { defaultTarget };
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x00048654 File Offset: 0x00046854
	public override bool OnOperationConfirm(TIGameState actorState, TIGameState target, TIResourcesCost resourcesCost, Trajectory trajectory)
	{
		if (base.OnOperationConfirm(actorState, target, resourcesCost, trajectory))
		{
			TIFactionState ref_faction = actorState.ref_faction;
			TISpaceBodyState ref_spaceBody = target.ref_spaceBody;
			ref_faction.LaunchProspector(ref_spaceBody);
			GameControl.eventManager.TriggerEvent(new ProspectingBody(actorState.ref_faction, target.ref_spaceBody), null, new object[] { actorState.ref_faction, target.ref_spaceBody });
			TINotificationQueueState.LogProbeLaunched(ref_faction, ref_spaceBody);
			TINotificationQueueState.LogEnemyProbeLaunched(ref_faction, ref_spaceBody);
			return true;
		}
		return false;
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x000486C8 File Offset: 0x000468C8
	public override void ExecuteOperation(TIGameState actorState, TIGameState target)
	{
		TIFactionState ref_faction = actorState.ref_faction;
		TISpaceBodyState ref_spaceBody = target.ref_spaceBody;
		ref_faction.ProspectSpaceBody(ref_spaceBody);
		TINotificationQueueState.LogProbeArrived(ref_faction, ref_spaceBody);
		TINotificationQueueState.LogEnemyProbeArrived(ref_faction, ref_spaceBody);
	}
}
