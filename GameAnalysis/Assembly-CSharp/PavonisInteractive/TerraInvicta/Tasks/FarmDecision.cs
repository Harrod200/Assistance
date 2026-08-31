using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x0200093C RID: 2364
	internal class FarmDecision : ArchetypeDecision
	{
		// Token: 0x06005A80 RID: 23168 RVA: 0x002B226E File Offset: 0x002B046E
		public FarmDecision()
			: base(ArchetypeDecision.HabModuleArchetype.Farming, false)
		{
		}

		// Token: 0x06005A81 RID: 23169 RVA: 0x002B227C File Offset: 0x002B047C
		public override IEnumerable<TIHabModuleTemplate> Decide(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			if (FarmDecision.factionMonthlyIncomeCachedFrame != TIFrameCounter.FrameCount || FarmDecision.factionMonthlyIncomeCachedFaction != faction)
			{
				FarmDecision.cachedFactionMonthlyProduction = TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource resource) => faction.habs.Sum<TIHabState>((TIHabState x) => x.ActiveModules().Sum<TIHabModuleState>((TIHabModuleState y) => y.moduleTemplate.MonthlyResourceIncome(resource, x, faction))));
				FarmDecision.factionMonthlyIncomeCachedFrame = TIFrameCounter.FrameCount;
				FarmDecision.factionMonthlyIncomeCachedFaction = faction;
			}
			Dictionary<FactionResource, float> factionMonthlyConsumption = TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource resource) => FarmDecision.cachedFactionMonthlyProduction[resource] - faction.GetMonthlyIncome(resource, true, false));
			Dictionary<FactionResource, float> monthlyFarmProduction = TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource resource) => order.Sum<TIHabModuleTemplate>((TIHabModuleTemplate moduleTemplate) => moduleTemplate.GetFarmResourceValue(resource)));
			List<TIHabModuleTemplate> newModules = order.ToList<TIHabModuleTemplate>();
			if (location.ref_hab != null)
			{
				foreach (TIHabModuleState tihabModuleState in location.ref_hab.OkayModules())
				{
					newModules.Remove(tihabModuleState.moduleTemplate);
				}
			}
			Dictionary<FactionResource, float> newMonthlyConsumption = TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource resource) => newModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate moduleTemplate) => moduleTemplate.MonthlySupportCost(resource, true, faction, null)));
			Dictionary<FactionResource, float> newMonthlyFarmProduction = TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource resource) => newModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate moduleTemplate) => moduleTemplate.GetFarmResourceValue(resource)));
			if (TIResourcesCost.farmResources.All<FactionResource>(delegate(FactionResource resource)
			{
				float num = FarmDecision.cachedFactionMonthlyProduction[resource] * 0.9f + newMonthlyFarmProduction[resource];
				float num2 = (factionMonthlyConsumption[resource] + newMonthlyConsumption[resource]) / num;
				float num3 = 0.85f;
				if (faction.IsResourceUpkeepInsecure(resource, AIEvaluators.UpkeepInsecurityType.Future))
				{
					num3 *= 0.6f;
				}
				else if (faction.IsResourceUpkeepInsecure(resource, AIEvaluators.UpkeepInsecurityType.Present))
				{
					num3 *= 0.7f;
				}
				else if (faction.IsResourceUpkeepInsecure(resource, AIEvaluators.UpkeepInsecurityType.PresentCautious))
				{
					num3 *= 0.8f;
				}
				if (faction.GetCriticalBasicSpaceResource() == resource)
				{
					num3 *= 0.9f;
				}
				return num2 < num3;
			}))
			{
				return HabSchematicDecision.Nothing;
			}
			Dictionary<FactionResource, float> monthlyRecyclableConsumption = TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource resource) => order.Sum<TIHabModuleTemplate>((TIHabModuleTemplate moduleTemplate) => moduleTemplate.GetMonthlyRecyclableConsumption(resource, faction, null)));
			TIHabModuleTemplate bestFarm = this.GetBestFarm_Internal(faction, location, order);
			if (bestFarm == null)
			{
				return HabSchematicDecision.Nothing;
			}
			int additionalFarmCount = 0;
			Func<bool> func = delegate
			{
				foreach (FactionResource factionResource in TIResourcesCost.farmResources)
				{
					float farmResourceValue = bestFarm.GetFarmResourceValue(factionResource);
					float num4 = monthlyRecyclableConsumption[factionResource] - monthlyFarmProduction[factionResource] - farmResourceValue * (float)additionalFarmCount;
					if (num4 > 0f && num4 / farmResourceValue > 0.3f)
					{
						return true;
					}
				}
				return false;
			};
			if (!func())
			{
				return HabSchematicDecision.Nothing;
			}
			do
			{
				int additionalFarmCount2 = additionalFarmCount;
				additionalFarmCount = additionalFarmCount2 + 1;
			}
			while (func());
			return Enumerable.Repeat<TIHabModuleTemplate>(bestFarm, additionalFarmCount);
		}

		// Token: 0x06005A82 RID: 23170 RVA: 0x002B2538 File Offset: 0x002B0738
		private TIHabModuleTemplate GetBestFarm_Internal(TIFactionState faction, TIGameState location, HabSchematicOrder order)
		{
			if (FarmDecision.bestFarmsCachedFrame != TIFrameCounter.FrameCount)
			{
				FarmDecision.cachedBestFarms.Clear();
				FarmDecision.bestFarmsCachedFrame = TIFrameCounter.FrameCount;
			}
			TIHabModuleTemplate tihabModuleTemplate = order.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule);
			if (tihabModuleTemplate == null)
			{
				return null;
			}
			ValueTuple<TIFactionState, TISpaceBodyState, int> valueTuple = new ValueTuple<TIFactionState, TISpaceBodyState, int>(faction, location.ref_system, tihabModuleTemplate.tier);
			TIHabModuleTemplate tihabModuleTemplate2;
			if (!FarmDecision.cachedBestFarms.TryGetValue(valueTuple, out tihabModuleTemplate2))
			{
				tihabModuleTemplate2 = base.Decide(faction, location, order).MaxBy<TIHabModuleTemplate, float>((TIHabModuleTemplate x) => x.FarmValue);
				FarmDecision.cachedBestFarms[valueTuple] = tihabModuleTemplate2;
			}
			return tihabModuleTemplate2;
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x002B25F0 File Offset: 0x002B07F0
		public static TIHabModuleTemplate GetBestFarm(TIFactionState faction, TIGameState location, IEnumerable<TIHabModuleTemplate> existingModules = null)
		{
			if (existingModules == null && location.isHabState)
			{
				existingModules = Enumerable.Empty<TIHabModuleTemplate>().Append(location.ref_hab.coreModule.moduleTemplate);
			}
			HabSchematicOrder habSchematicOrder = new HabSchematicOrder(null, existingModules);
			return new FarmDecision().GetBestFarm_Internal(faction, location, habSchematicOrder);
		}

		// Token: 0x0400414A RID: 16714
		private static Dictionary<FactionResource, float> cachedFactionMonthlyProduction;

		// Token: 0x0400414B RID: 16715
		private static int factionMonthlyIncomeCachedFrame = -1;

		// Token: 0x0400414C RID: 16716
		private static TIFactionState factionMonthlyIncomeCachedFaction;

		// Token: 0x0400414D RID: 16717
		private static Dictionary<ValueTuple<TIFactionState, TISpaceBodyState, int>, TIHabModuleTemplate> cachedBestFarms = new Dictionary<ValueTuple<TIFactionState, TISpaceBodyState, int>, TIHabModuleTemplate>();

		// Token: 0x0400414E RID: 16718
		private static int bestFarmsCachedFrame = -1;
	}
}
