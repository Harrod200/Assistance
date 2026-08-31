using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x0200017E RID: 382
public static class TradeAI
{
	// Token: 0x06000578 RID: 1400 RVA: 0x0001829C File Offset: 0x0001649C
	public static TradeOffer.TradeAgreement CreateTradeAgreement(TIFactionState agreementCreator, TIFactionState agreementRecipient)
	{
		List<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>> list = ((TradeAI.CategoryType[])Enum.GetValues(typeof(TradeAI.CategoryType))).Where<TradeAI.CategoryType>((TradeAI.CategoryType x) => x != TradeAI.CategoryType.None && x != TradeAI.CategoryType.Diplomatic).Select<TradeAI.CategoryType, ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>(delegate(TradeAI.CategoryType category)
		{
			TradeOffer tradeOffer = TradeAI.MakeDemands(category, agreementCreator, agreementRecipient);
			if (category != TradeAI.CategoryType.Resources)
			{
				TradeOffer tradeOffer2 = TradeAI.MakeDemands(TradeAI.CategoryType.Resources, agreementCreator, agreementRecipient);
				tradeOffer = tradeOffer.MergeWith(tradeOffer2);
			}
			if (category != TradeAI.CategoryType.Diplomatic)
			{
				TradeOffer tradeOffer3 = TradeAI.MakeDemands(TradeAI.CategoryType.Diplomatic, agreementCreator, agreementRecipient);
				tradeOffer = tradeOffer.MergeWith(tradeOffer3);
			}
			TradeOffer counterOffer = TradeAI.GetCounterOffer(tradeOffer, agreementCreator, agreementRecipient);
			if (tradeOffer.treatyType != counterOffer.treatyType)
			{
				tradeOffer.treatyType = (counterOffer.treatyType = TradeOffer.TreatyType.None);
			}
			if (tradeOffer.intelExchange != counterOffer.intelExchange)
			{
				tradeOffer.intelExchange = (counterOffer.intelExchange = false);
			}
			TradeOffer.TradeAgreement tradeAgreement = new ValueTuple<TradeOffer, TradeOffer>(tradeOffer, counterOffer);
			float num = TradeAI.ScoreAgreement(tradeAgreement, agreementCreator);
			float num2 = TradeAI.ScoreAgreement(tradeAgreement, agreementRecipient);
			bool flag = TradeAI.IsAgreementAcceptable(tradeAgreement, agreementCreator, agreementRecipient);
			bool flag2 = TradeAI.IsAgreementAcceptable(tradeAgreement, agreementRecipient, agreementCreator);
			return new ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>(tradeAgreement, num, num2, flag, flag2);
		}).ToList<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>();
		IEnumerable<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>> enumerable = list.Where<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>(([TupleElementNames(new string[] { "Agreement", "CreatorScore", "RecipientScore", "IsAcceptableToCreator", "IsAcceptableToRecipient" })] ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool> x) => x.Item4);
		if (enumerable.Any<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>())
		{
			list = enumerable.ToList<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>();
		}
		else
		{
			IEnumerable<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>> enumerable2 = list.Where<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>(([TupleElementNames(new string[] { "Agreement", "CreatorScore", "RecipientScore", "IsAcceptableToCreator", "IsAcceptableToRecipient" })] ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool> x) => x.Item2 > 0f);
			if (enumerable2.Any<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>())
			{
				list = enumerable2.ToList<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>();
			}
		}
		IEnumerable<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>> enumerable3 = list.Where<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>(([TupleElementNames(new string[] { "Agreement", "CreatorScore", "RecipientScore", "IsAcceptableToCreator", "IsAcceptableToRecipient" })] ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool> x) => x.Item5);
		if (enumerable3.Any<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>())
		{
			list = enumerable3.ToList<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>();
		}
		else
		{
			IEnumerable<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>> enumerable4 = list.Where<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>(([TupleElementNames(new string[] { "Agreement", "CreatorScore", "RecipientScore", "IsAcceptableToCreator", "IsAcceptableToRecipient" })] ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool> x) => x.Item3 > 0f);
			if (enumerable4.Any<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>())
			{
				list = enumerable4.ToList<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>();
			}
		}
		return list.SelectRandomWeightedItem<ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool>>(delegate([TupleElementNames(new string[] { "Agreement", "CreatorScore", "RecipientScore", "IsAcceptableToCreator", "IsAcceptableToRecipient" })] ValueTuple<TradeOffer.TradeAgreement, float, float, bool, bool> x)
		{
			float num3 = 2f * x.Item2 + 1f * x.Item3;
			if (x.Item3 <= 0f)
			{
				num3 *= 0.7f;
			}
			return Mathf.Pow(num3, 1.5f);
		}, -1f, 1E-37f).Item1;
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x00018420 File Offset: 0x00016620
	public static void PrepareCachesForTrading(TIFactionState faction, TIFactionState otherFaction)
	{
		faction.RecalculateIncomes();
		otherFaction.RecalculateIncomes();
		foreach (TIHabState tihabState in faction.habs.Concat<TIHabState>(otherFaction.habs))
		{
			tihabState.UpdateCurrentAnnualNetResourceIncomes(true);
		}
		faction.TriggerFactionResourceUpdateEvent();
		otherFaction.TriggerFactionResourceUpdateEvent();
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00018490 File Offset: 0x00016690
	private static TradeOffer MakeDemands(TradeAI.CategoryType category, TIFactionState demandMaker, TIFactionState demandRecipient)
	{
		TradeOffer tradeOffer = new TradeOffer(demandRecipient);
		switch (category)
		{
		case TradeAI.CategoryType.Orgs:
			tradeOffer.orgs = TradeAI.<MakeDemands>g__SelectTradeItems|3_0<TIOrgState>(demandRecipient.GetAllOrgs(), (TIOrgState x) => demandRecipient.CanTradeOrg(x, demandMaker), (TIOrgState x) => (float)x.tier, (TIOrgState x) => TradeAI.GetTradeEfficiency(x, demandRecipient, demandMaker), 2, 1f, 1.5f).ToList<TIOrgState>();
			break;
		case TradeAI.CategoryType.Resources:
		{
			ValueTuple<FactionResource, float, float> valueTuple = TradeAI.FilterTradeCandidatesForEfficiency<ValueTuple<FactionResource, float, float>>((from x in TIResourcesCost.tradeableResources
				where demandRecipient.CanTradeAwayResource(x, demandMaker)
				where demandRecipient.GetCurrentResourceAmount(x) > 0f
				select x).Select<FactionResource, ValueTuple<FactionResource, float, float>>(delegate(FactionResource resource)
			{
				float num = demandRecipient.GetCurrentResourceAmount(resource) * 0.2f;
				float maximumTradeQuantity = TradeAI.GetMaximumTradeQuantity(demandRecipient, resource);
				num = Mathf.Min(num, maximumTradeQuantity);
				float tradeEfficiency = TradeAI.GetTradeEfficiency(resource, num, demandRecipient, demandMaker);
				return new ValueTuple<FactionResource, float, float>(resource, num, tradeEfficiency);
			}).ToList<ValueTuple<FactionResource, float, float>>(), ([TupleElementNames(new string[] { "Resource", "Quantity", "Efficiency" })] ValueTuple<FactionResource, float, float> x) => x.Item3, 5).ToList<ValueTuple<FactionResource, float, float>>().SelectRandomWeightedItem<ValueTuple<FactionResource, float, float>>(([TupleElementNames(new string[] { "Resource", "Quantity", "Efficiency" })] ValueTuple<FactionResource, float, float> x) => Mathf.Pow(x.Item2, 0.8f) * Mathf.Pow(x.Item3, 1.5f), -1f, 1E-37f);
			tradeOffer.resourceValues.Add(new ResourceValue(valueTuple.Item1, valueTuple.Item2));
			break;
		}
		case TradeAI.CategoryType.Projects:
			tradeOffer.projects = TradeAI.<MakeDemands>g__SelectTradeItems|3_0<TIProjectTemplate>(demandRecipient.completedProjects, (TIProjectTemplate x) => demandRecipient.CanTradeProject(x, demandMaker), (TIProjectTemplate x) => x.researchCost, (TIProjectTemplate x) => TradeAI.GetTradeEfficiency(x, demandRecipient, demandMaker), 2, 1f, 1.5f).ToList<TIProjectTemplate>();
			break;
		case TradeAI.CategoryType.Habs:
			tradeOffer.habs = TradeAI.<MakeDemands>g__SelectTradeItems|3_0<TIHabState>(demandRecipient.habs, (TIHabState x) => TradeAI.IsValidTradeItem(x, demandRecipient, demandMaker), (TIHabState x) => (float)(x.AllSlots().Count + (x.IsBase ? 5 : 0)) * Mathf.Pow((float)x.tier, 0.7f), (TIHabState x) => TradeAI.GetTradeEfficiency(x, demandRecipient, demandMaker), 1, 1f, 1.5f).ToList<TIHabState>();
			break;
		case TradeAI.CategoryType.Diplomatic:
			foreach (TradeOffer.TreatyType treatyType in new List<TradeOffer.TreatyType>
			{
				TradeOffer.TreatyType.Truce,
				TradeOffer.TreatyType.NAP,
				TradeOffer.TreatyType.Intel
			})
			{
				if (demandMaker.CanTradeTreaty(demandRecipient, treatyType) && TradeAI.GetTradeValue(treatyType, demandMaker, demandRecipient) > 0f)
				{
					tradeOffer.treatyType = treatyType;
					break;
				}
			}
			break;
		}
		return tradeOffer;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x00018748 File Offset: 0x00016948
	private static IEnumerable<T> FilterTradeCandidatesForEfficiency<T>(IEnumerable<T> tradeCandidates, Func<T, float> GetEfficiency, int minimumCount = 1)
	{
		tradeCandidates = tradeCandidates.Where<T>((T x) => GetEfficiency(x) > 0f);
		IEnumerable<T> enumerable = tradeCandidates.Where<T>((T x) => GetEfficiency(x) > 1.5f);
		if (enumerable.Count<T>() >= minimumCount)
		{
			tradeCandidates = enumerable;
		}
		enumerable = tradeCandidates.Where<T>((T x) => GetEfficiency(x) > 1f);
		if (enumerable.Count<T>() >= minimumCount)
		{
			tradeCandidates = enumerable;
		}
		enumerable = tradeCandidates.Where<T>((T x) => GetEfficiency(x) > 0.5f);
		if (enumerable.Count<T>() >= minimumCount)
		{
			tradeCandidates = enumerable;
		}
		return tradeCandidates;
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x000187D4 File Offset: 0x000169D4
	private static float GetTradeEfficiency(float valueToGiver, float valueToRecipient)
	{
		float num;
		if (valueToGiver == valueToRecipient)
		{
			num = 1f;
		}
		else if (valueToGiver > 0f && valueToRecipient < 0f)
		{
			num = -1f;
		}
		else if (valueToGiver < 0f && valueToRecipient > 0f)
		{
			num = 10f * (valueToRecipient - valueToGiver) / 5000f;
		}
		else if (valueToGiver < 0f && valueToRecipient < 0f)
		{
			num = -1f;
		}
		else
		{
			num = valueToRecipient / valueToGiver;
		}
		if (num > 10f)
		{
			num = Mathf.Pow(num - 9f, 0.5f) + 9f;
		}
		return num;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00018864 File Offset: 0x00016A64
	private static TradeAI.ResourceTradeData GetResourceTradeData(TIFactionState faction, FactionResource resource)
	{
		if (TIResourcesCost.unAccumulatableResources.Contains(resource))
		{
			return default(TradeAI.ResourceTradeData);
		}
		if (TradeAI.tradeDataCachedFrame != TIFrameCounter.FrameCount)
		{
			TradeAI.cachedResourceTradeData.Clear();
			TradeAI.tradeDataCachedFrame = TIFrameCounter.FrameCount;
		}
		Dictionary<FactionResource, TradeAI.ResourceTradeData> dictionary;
		if (!TradeAI.cachedResourceTradeData.TryGetValue(faction, out dictionary))
		{
			dictionary = (TradeAI.cachedResourceTradeData[faction] = new Dictionary<FactionResource, TradeAI.ResourceTradeData>());
		}
		TradeAI.ResourceTradeData resourceTradeData;
		if (!dictionary.TryGetValue(resource, out resourceTradeData))
		{
			float num = 547.8633f;
			IEnumerable<TIFactionState.Transaction> filteredTransactions = faction.GetFilteredTransactions(ref num, null, resource, (string label) => label != "Daily Income");
			float num2 = -filteredTransactions.Where<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount < 0f).Sum<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount) / num;
			float num3 = filteredTransactions.Where<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount > 0f).Sum<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount) / num;
			float dailyRevenue = faction.GetDailyRevenue(resource, false);
			float dailyIncome = faction.GetDailyIncome(resource, false, false);
			if (dailyIncome > dailyRevenue && dailyIncome - dailyRevenue > 0.01f)
			{
				Log.Warn("Daily income should never exceed daily revenue. Recommend investigating faction resource caches: https://github.com/pavonisinteractive/terra-invicta-issues/issues/6241", Array.Empty<object>());
			}
			float num4 = dailyRevenue + num3;
			float num5 = num2 + (num4 - dailyIncome);
			float currentResourceAmount = faction.GetCurrentResourceAmount(resource);
			float num6 = 0f;
			float num7 = 0f;
			switch (resource)
			{
			case FactionResource.Money:
				num6 += 3000f;
				num7 += 10f;
				break;
			case FactionResource.Influence:
				num6 += 120f;
				num7 += 1.2f;
				break;
			case FactionResource.Operations:
				num6 += 60f;
				num7 += 0.5f;
				break;
			case FactionResource.Boost:
				num6 += 30f;
				num7 += 0.8f;
				break;
			case FactionResource.Water:
			case FactionResource.Volatiles:
				num6 += 500f;
				num7 += 3f;
				break;
			case FactionResource.Metals:
				num6 += 300f;
				num7 += 2.3f;
				break;
			case FactionResource.NobleMetals:
				num6 += 150f;
				num7 += 1.2f;
				break;
			case FactionResource.Fissiles:
				num6 += 50f;
				num7 += 0.6f;
				break;
			case FactionResource.Antimatter:
				num6 += 1f;
				num7 += 1E-06f;
				break;
			case FactionResource.Exotics:
				num6 += 1f;
				num7 += 1E-06f;
				break;
			}
			float num8 = (currentResourceAmount + num6) / (num5 + num7);
			resourceTradeData.RevenuePerDay = num4;
			resourceTradeData.CostPerDay = num5;
			resourceTradeData.StorageDays = num8;
			dictionary[resource] = resourceTradeData;
		}
		return resourceTradeData;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00018B5C File Offset: 0x00016D5C
	private static float GetMiscResourceModifier(TIFactionState faction, FactionResource resource)
	{
		float num = 1f;
		switch (resource)
		{
		case FactionResource.Money:
			num *= 0.7f * faction.aiValues.gatherMoney;
			break;
		case FactionResource.Influence:
			num *= faction.aiValues.gatherInfluence;
			break;
		case FactionResource.Operations:
			num *= faction.aiValues.gatherOps;
			if (faction.currentlySearchingForHydraCouncilor)
			{
				num *= 2f;
			}
			else if (!faction.veryProAlien && !faction.MilestoneCompleted(CampaignMilestone.AccessLiveHydra))
			{
				num *= 1.5f;
			}
			else if (faction.veryProAlien && !faction.MilestoneCompleted(CampaignMilestone.AlienDiplomacy))
			{
				num *= 1.5f;
			}
			break;
		case FactionResource.Research:
		case FactionResource.Projects:
			num *= faction.aiValues.gatherScience;
			if (faction.IsAlienFaction)
			{
				num *= 0f;
			}
			break;
		case FactionResource.Boost:
			num *= 3f * faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability;
			if (TIResourcesCost.basicSpaceResources.All<FactionResource>((FactionResource x) => faction.GetDailyIncome(x, true, false) >= 0f))
			{
				num *= 2f;
			}
			break;
		case FactionResource.MissionControl:
		case FactionResource.Water:
		case FactionResource.Volatiles:
		case FactionResource.Metals:
		case FactionResource.NobleMetals:
		case FactionResource.Fissiles:
		case FactionResource.Antimatter:
		case FactionResource.Exotics:
			num *= faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability;
			break;
		}
		return num;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x00018D18 File Offset: 0x00016F18
	public static float GetResourceStorageModifier(TIFactionState faction, FactionResource resource, float hypotheticalAdditionalStoredQuantity)
	{
		if (TIResourcesCost.unAccumulatableResources.Contains(resource))
		{
			return 1f;
		}
		TradeAI.ResourceTradeData resourceTradeData = TradeAI.GetResourceTradeData(faction, resource);
		float currentResourceAmount = faction.GetCurrentResourceAmount(resource);
		resourceTradeData.StorageDays *= (currentResourceAmount + hypotheticalAdditionalStoredQuantity) / currentResourceAmount;
		IEnumerable<FactionResource> enumerable = Enumerable.Empty<FactionResource>().Append(FactionResource.Boost).Append(FactionResource.Antimatter)
			.Append(FactionResource.Exotics);
		List<TradeAI.ResourceTradeData> list = (from x in TIResourcesCost.tradeableResources.Except<FactionResource>(enumerable)
			select TradeAI.GetResourceTradeData(faction, x) into x
			where x.StorageDays >= 0f
			select x).ToList<TradeAI.ResourceTradeData>();
		if (!list.Any<TradeAI.ResourceTradeData>())
		{
			Log.Error("No related resource trade data available, investigation required: https://github.com/pavonisinteractive/terra-invicta-issues/issues/6241", Array.Empty<object>());
			return 0f;
		}
		float num = list.Average<TradeAI.ResourceTradeData>((TradeAI.ResourceTradeData x) => x.StorageDays);
		float num2 = Mathf.Lerp(list.Median<TradeAI.ResourceTradeData>((TradeAI.ResourceTradeData x) => x.StorageDays), num, 0.35f);
		float num3 = 1f;
		float num4 = 1f;
		if (resourceTradeData.StorageDays > 0f)
		{
			num3 = Mathf.Pow(180f / resourceTradeData.StorageDays, 0.4f);
			num4 = Mathf.Pow(num2 / resourceTradeData.StorageDays, 0.7f);
		}
		return Mathf.Pow(num3 * num4, 0.8f);
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x00018EA4 File Offset: 0x000170A4
	private static float GetTradeValue_Segment(TIFactionState faction, FactionResource resource, float quantity, float hypotheticalAdditionalStoredQuantity)
	{
		if (quantity == 0f)
		{
			return 0f;
		}
		float num = AIEvaluators.GetAIRelativeValuation(resource) * quantity;
		float miscResourceModifier = TradeAI.GetMiscResourceModifier(faction, resource);
		float resourceStorageModifier = TradeAI.GetResourceStorageModifier(faction, resource, hypotheticalAdditionalStoredQuantity);
		TradeAI.ResourceTradeData resourceTradeData = TradeAI.GetResourceTradeData(faction, resource);
		float num2 = 1f;
		if (resourceTradeData.RevenuePerDay < resourceTradeData.CostPerDay)
		{
			num2 *= 1.5f;
		}
		return 0.6f * num * miscResourceModifier * resourceStorageModifier;
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x00018F0C File Offset: 0x0001710C
	public static float GetTradeValue(TIFactionState faction, FactionResource resource, float quantity, float hypotheticalAdditionalStoredQuantity = 0f)
	{
		if (quantity == 0f)
		{
			return 0f;
		}
		float currentResourceAmount = faction.GetCurrentResourceAmount(resource);
		float num;
		float num2;
		int num3;
		if (quantity > 0f)
		{
			num = Mathf.Min(Mathf.Max(quantity / 150f, currentResourceAmount / 5f), quantity / 5f);
			num2 = 1.3f;
			num3 = Mathf.Clamp((quantity / (currentResourceAmount + 1f)).RoundUp(), 0, 18) + 2;
		}
		else
		{
			float num4 = currentResourceAmount * 0.5f + 1f;
			num3 = Mathf.Clamp((num4 / Mathf.Max(num4 + quantity, 0.001f)).RoundUp(), 0, 20) + 1;
			num2 = 0.7f;
			num = quantity * (1f - num2) / (1f - Mathf.Pow(num2, (float)num3));
		}
		float num5 = 0f;
		float num6 = 0f;
		for (int i = 0; i < num3; i++)
		{
			num5 += TradeAI.GetTradeValue_Segment(faction, resource, num, hypotheticalAdditionalStoredQuantity + num6);
			num6 += num;
			num *= num2;
			if (Mathf.Abs(num + num6) > Mathf.Abs(quantity))
			{
				break;
			}
		}
		float num7 = quantity - num6;
		if ((double)Mathf.Abs(num7) > 0.001)
		{
			num5 += TradeAI.GetTradeValue_Segment(faction, resource, num, hypotheticalAdditionalStoredQuantity + num6);
			num6 += num7;
		}
		return num5;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x00019040 File Offset: 0x00017240
	public static float GetTradeEfficiency(FactionResource resource, float quantity, TIFactionState giver, TIFactionState recipient)
	{
		float tradeValue = TradeAI.GetTradeValue(giver, resource, quantity, 0f);
		float tradeValue2 = TradeAI.GetTradeValue(recipient, resource, quantity, 0f);
		return TradeAI.GetTradeEfficiency(tradeValue, tradeValue2);
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0001906E File Offset: 0x0001726E
	public static float GetTradeValue(TIOrgState org, TIFactionState faction)
	{
		return AIEvaluators.EvaluateOrgForTrade(org, faction);
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00019078 File Offset: 0x00017278
	public static float GetTradeEfficiency(TIOrgState org, TIFactionState giver, TIFactionState recipient)
	{
		float tradeValue = TradeAI.GetTradeValue(org, giver);
		float tradeValue2 = TradeAI.GetTradeValue(org, recipient);
		return TradeAI.GetTradeEfficiency(tradeValue, tradeValue2);
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x0001909A File Offset: 0x0001729A
	public static float GetTradeValue(TIProjectTemplate project, TIFactionState faction)
	{
		return AIEvaluators.EvaluateTechForTrade(faction, project);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x000190A4 File Offset: 0x000172A4
	public static float GetTradeEfficiency(TIProjectTemplate project, TIFactionState giver, TIFactionState recipient)
	{
		float tradeValue = TradeAI.GetTradeValue(project, giver);
		float tradeValue2 = TradeAI.GetTradeValue(project, recipient);
		return TradeAI.GetTradeEfficiency(tradeValue, tradeValue2);
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x000190C6 File Offset: 0x000172C6
	private static bool IsValidTradeItem(TIHabState hab, TIFactionState givingFaction, TIFactionState receivingFaction)
	{
		if (hab.faction != givingFaction)
		{
			throw new ArgumentException();
		}
		return hab.faction.MayTradeAwayHab(hab, receivingFaction) && !hab.faction.AI_ShouldNotTradeAwayHab(hab) && !receivingFaction.AI_ShouldNotAcquireHabInTrade(hab);
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00019105 File Offset: 0x00017305
	public static float GetTradeValue(TIHabState hab, TIFactionState faction)
	{
		return AIEvaluators.EvaluateHabForTrade(faction, hab);
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00019110 File Offset: 0x00017310
	public static float GetMissionControlChangeTradeValue(TIFactionState faction, int netMC)
	{
		float num = TradeAI.GetTradeValue(faction, FactionResource.MissionControl, (float)netMC, 0f);
		if (netMC < 0 && faction.MissionControlBalance + netMC < 0)
		{
			num *= 4f;
		}
		return num;
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00019144 File Offset: 0x00017344
	public static float GetTradeEfficiency(TIHabState hab, TIFactionState giver, TIFactionState recipient)
	{
		float tradeValue = TradeAI.GetTradeValue(hab, giver);
		float tradeValue2 = TradeAI.GetTradeValue(hab, recipient);
		return TradeAI.GetTradeEfficiency(tradeValue, tradeValue2);
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x00019168 File Offset: 0x00017368
	public static float GetTradeValue(TradeOffer.TreatyType treatyType, TIFactionState faction, TIFactionState otherFaction)
	{
		if (treatyType == TradeOffer.TreatyType.None)
		{
			return 0f;
		}
		if (faction.isActivePlayer)
		{
			return 500f;
		}
		if (AIEvaluators.GetWillingnessToTradeTreaty(faction, otherFaction, treatyType) <= 0f)
		{
			return -1f;
		}
		float num = 0f;
		switch (treatyType)
		{
		case TradeOffer.TreatyType.Truce:
			num = 470f;
			break;
		case TradeOffer.TreatyType.NAP:
			num = 400f;
			break;
		case TradeOffer.TreatyType.Intel:
			num = 250f;
			break;
		}
		TIFactionState strongestHumanFaction = AIEvaluators.GetStrongestHumanFaction(null);
		int num2 = (from x in GameStateManager.AllFactions()
			where x != strongestHumanFaction
			select x).Count<TIFactionState>((TIFactionState x) => x.HasNAP(faction, true));
		int num3 = 2;
		if (Mathf.Abs(faction.ideologyCoordinates.x) <= 0.5f)
		{
			num3++;
		}
		if (faction.ideologyCoordinates.x == 0f)
		{
			num3++;
		}
		num += (float)(Mathf.Max(new int[] { num3 - 2 - num2 }) * 240);
		return num / 2f;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00019284 File Offset: 0x00017484
	public static float GetAgreementFavorability(TradeOffer.TradeAgreement agreement, TIFactionState assessingFaction, TIFactionState otherFaction)
	{
		float num = TradeAI.ScoreAgreement(agreement, assessingFaction);
		if (num <= 0f)
		{
			return 0f;
		}
		float num2 = TradeAI.ScoreAgreement(agreement, otherFaction);
		if (num2 <= 0f)
		{
			return 1f;
		}
		float num3 = num + num2;
		return num / num3;
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x000192C4 File Offset: 0x000174C4
	public static float GetMinimumAgreementFavorability(TIFactionState faction, TIFactionState otherFaction)
	{
		float distrust = TradeAI.GetDistrust(faction, otherFaction);
		return Mathf.Lerp(0.3f, 0.49f, distrust - 1.2f);
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x000192F0 File Offset: 0x000174F0
	public static bool IsAgreementAcceptable(TradeOffer.TradeAgreement agreement, TIFactionState assessingFaction, TIFactionState otherFaction, out float favorability)
	{
		TradeOffer offer = agreement.GetOffer(assessingFaction);
		TradeOffer offer2 = agreement.GetOffer(otherFaction);
		favorability = 0f;
		if (offer2.habs.Count == 0)
		{
			if (offer.habs.Count > 1)
			{
				return false;
			}
		}
		else if (offer.habs.Count > offer2.habs.Count)
		{
			return false;
		}
		if (offer.orgs.Count > offer2.orgs.Count + 2)
		{
			return false;
		}
		using (IEnumerator<FactionResource> enumerator = TIResourcesCost.tradeableResources.Append(FactionResource.MissionControl).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				FactionResource resource = enumerator.Current;
				float num = offer2.orgs.Sum<TIOrgState>((TIOrgState x) => x.GetMonthlyIncome(resource)) - offer.orgs.Sum<TIOrgState>((TIOrgState x) => x.GetMonthlyIncome(resource));
				float num2 = offer2.habs.Sum<TIHabState>((TIHabState x) => x.GetNetCurrentMonthlyIncome(x.faction, resource, false, true));
				num2 -= offer.habs.Sum<TIHabState>((TIHabState x) => x.GetNetCurrentMonthlyIncome(x.faction, resource, false, true));
				float num3 = num + num2;
				if (num3 < 0f)
				{
					if (resource == FactionResource.MissionControl)
					{
						if ((float)assessingFaction.MissionControlBalance + num3 < 0f)
						{
							return false;
						}
					}
					else
					{
						float monthlyIncome = assessingFaction.GetMonthlyIncome(resource, true, false);
						if (monthlyIncome <= 0f)
						{
							return false;
						}
						float num4 = monthlyIncome + num3;
						if (monthlyIncome + num3 <= 0f)
						{
							return false;
						}
						if (num4 / monthlyIncome < 0.6f)
						{
							return false;
						}
					}
				}
			}
		}
		favorability = TradeAI.GetAgreementFavorability(agreement, assessingFaction, otherFaction);
		float minimumAgreementFavorability = TradeAI.GetMinimumAgreementFavorability(assessingFaction, otherFaction);
		return favorability >= minimumAgreementFavorability;
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x000194C4 File Offset: 0x000176C4
	public static bool IsAgreementAcceptable(TradeOffer.TradeAgreement agreement, TIFactionState assessingFaction, TIFactionState otherFaction)
	{
		float num;
		return TradeAI.IsAgreementAcceptable(agreement, assessingFaction, otherFaction, out num);
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x000194DC File Offset: 0x000176DC
	public static TradeOffer GetCounterOffer(TradeOffer demands, TIFactionState demandMaker, TIFactionState demandCounterer)
	{
		TradeAI.<>c__DisplayClass28_0 CS$<>8__locals1 = new TradeAI.<>c__DisplayClass28_0();
		CS$<>8__locals1.demandMaker = demandMaker;
		CS$<>8__locals1.demandCounterer = demandCounterer;
		CS$<>8__locals1.demands = demands;
		CS$<>8__locals1.counterOffer = new TradeOffer(CS$<>8__locals1.demandMaker);
		if (CS$<>8__locals1.demands.treatyType != TradeOffer.TreatyType.None)
		{
			if (CS$<>8__locals1.demandCounterer.CanTradeTreaty(CS$<>8__locals1.demandMaker, CS$<>8__locals1.demands.treatyType))
			{
				if (TradeAI.GetTradeValue(CS$<>8__locals1.demands.treatyType, CS$<>8__locals1.demandCounterer, CS$<>8__locals1.demandMaker) > 0f || CS$<>8__locals1.demandCounterer.isActivePlayer)
				{
					CS$<>8__locals1.counterOffer.treatyType = CS$<>8__locals1.demands.treatyType;
				}
			}
			else
			{
				CS$<>8__locals1.demands.treatyType = TradeOffer.TreatyType.None;
			}
		}
		TradeAI.TryToBalanceCategory<TIOrgState>(TradeAI.CategoryType.Orgs, CS$<>8__locals1.demands, CS$<>8__locals1.counterOffer, from x in CS$<>8__locals1.demandMaker.GetAllOrgs()
			where CS$<>8__locals1.demandMaker.CanTradeOrg(x, CS$<>8__locals1.demandCounterer)
			select x, (TIOrgState x) => new ValueTuple<TIOrgState, float, float>(x, TradeAI.GetTradeValue(x, CS$<>8__locals1.demandCounterer), TradeAI.GetTradeEfficiency(x, CS$<>8__locals1.demandMaker, CS$<>8__locals1.demandCounterer)), 1.4f, 1.8f, 0);
		TradeAI.TryToBalanceCategory<TIHabState>(TradeAI.CategoryType.Habs, CS$<>8__locals1.demands, CS$<>8__locals1.counterOffer, CS$<>8__locals1.demandMaker.habs.Where<TIHabState>((TIHabState x) => TradeAI.IsValidTradeItem(x, CS$<>8__locals1.demandMaker, CS$<>8__locals1.demandCounterer)), (TIHabState x) => new ValueTuple<TIHabState, float, float>(x, TradeAI.GetTradeValue(x, CS$<>8__locals1.demandCounterer), TradeAI.GetTradeEfficiency(x, CS$<>8__locals1.demandMaker, CS$<>8__locals1.demandCounterer)), 1.6f, 1.8f, 0);
		TradeAI.TryToBalanceCategory<TIProjectTemplate>(TradeAI.CategoryType.Projects, CS$<>8__locals1.demands, CS$<>8__locals1.counterOffer, CS$<>8__locals1.demandMaker.completedProjects.Where<TIProjectTemplate>((TIProjectTemplate x) => CS$<>8__locals1.demandMaker.CanTradeProject(x, CS$<>8__locals1.demandCounterer)), (TIProjectTemplate x) => new ValueTuple<TIProjectTemplate, float, float>(x, TradeAI.GetTradeValue(x, CS$<>8__locals1.demandCounterer), TradeAI.GetTradeEfficiency(x, CS$<>8__locals1.demandMaker, CS$<>8__locals1.demandCounterer)), 1.4f, 1.8f, 1);
		CS$<>8__locals1.marginalFraction = 0.05f;
		CS$<>8__locals1.<GetCounterOffer>g__AddResourcesToCounterOffer|6(40, () => TradeAI.IsAgreementAcceptable(new ValueTuple<TradeOffer, TradeOffer>(CS$<>8__locals1.demands, CS$<>8__locals1.counterOffer), CS$<>8__locals1.demandCounterer, CS$<>8__locals1.demandMaker));
		return CS$<>8__locals1.counterOffer;
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x0001968C File Offset: 0x0001788C
	private static bool TryToBalanceCategory<T>(TradeAI.CategoryType category, TradeOffer fixedOffer, TradeOffer dynamicOffer, IEnumerable<T> candidates, [TupleElementNames(new string[] { "Candidate", "Value", "Efficiency" })] Func<T, ValueTuple<T, float, float>> GetCandidateMetrics, float sizeFactor, float efficiencyFactor, int bonusItemCount = 0) where T : class
	{
		TradeAI.<>c__DisplayClass29_0<T> CS$<>8__locals1 = new TradeAI.<>c__DisplayClass29_0<T>();
		CS$<>8__locals1.fixedOffer = fixedOffer;
		CS$<>8__locals1.category = category;
		CS$<>8__locals1.dynamicOffer = dynamicOffer;
		CS$<>8__locals1.bonusItemCount = bonusItemCount;
		CS$<>8__locals1.sizeFactor = sizeFactor;
		CS$<>8__locals1.efficiencyFactor = efficiencyFactor;
		CS$<>8__locals1.giveToFaction = CS$<>8__locals1.fixedOffer.offeringFaction;
		CS$<>8__locals1.takeFromFaction = CS$<>8__locals1.dynamicOffer.offeringFaction;
		switch (CS$<>8__locals1.category)
		{
		case TradeAI.CategoryType.Orgs:
		{
			candidates = candidates.Where<T>(delegate(T x)
			{
				TIOrgState tiorgState = x as TIOrgState;
				return tiorgState != null && !CS$<>8__locals1.dynamicOffer.orgs.Contains(tiorgState);
			});
			Action<TradeOffer, T> action = delegate(TradeOffer offer, T x)
			{
				offer.orgs.Add(x as TIOrgState);
			};
			goto IL_0123;
		}
		case TradeAI.CategoryType.Projects:
		{
			candidates = candidates.Where<T>(delegate(T x)
			{
				TIProjectTemplate tiprojectTemplate = x as TIProjectTemplate;
				return tiprojectTemplate != null && !CS$<>8__locals1.dynamicOffer.projects.Contains(tiprojectTemplate);
			});
			Action<TradeOffer, T> action = delegate(TradeOffer offer, T x)
			{
				offer.projects.Add(x as TIProjectTemplate);
			};
			goto IL_0123;
		}
		case TradeAI.CategoryType.Habs:
		{
			candidates = candidates.Where<T>(delegate(T x)
			{
				TIHabState tihabState = x as TIHabState;
				return tihabState != null && !CS$<>8__locals1.dynamicOffer.habs.Contains(tihabState);
			});
			Action<TradeOffer, T> action = delegate(TradeOffer offer, T x)
			{
				offer.habs.Add(x as TIHabState);
			};
			goto IL_0123;
		}
		}
		throw new NotSupportedException();
		IL_0123:
		List<ValueTuple<T, float, float>> list = candidates.Distinct<T>().Select<T, ValueTuple<T, float, float>>(GetCandidateMetrics).ToList<ValueTuple<T, float, float>>();
		CS$<>8__locals1.itemsAddedCount = 0;
		int num = 0;
		int num2 = 3;
		while (CS$<>8__locals1.<TryToBalanceCategory>g__KeepAddingItems|7(CS$<>8__locals1.dynamicOffer))
		{
			TradeAI.<>c__DisplayClass29_1<T> CS$<>8__locals2 = new TradeAI.<>c__DisplayClass29_1<T>();
			List<ValueTuple<T, float, float>> list2 = TradeAI.FilterTradeCandidatesForEfficiency<ValueTuple<T, float, float>>(list, ([TupleElementNames(new string[] { "Candidate", "Value", "Efficiency" })] ValueTuple<T, float, float> x) => x.Item3, 1).ToList<ValueTuple<T, float, float>>();
			if (list2.Count == 0)
			{
				break;
			}
			TradeAI.<>c__DisplayClass29_1<T> CS$<>8__locals3 = CS$<>8__locals2;
			IEnumerable<ValueTuple<T, float, float>> enumerable = list2;
			Func<ValueTuple<T, float, float>, float> func;
			if ((func = CS$<>8__locals1.<>9__9) == null)
			{
				func = (CS$<>8__locals1.<>9__9 = ([TupleElementNames(new string[] { "Candidate", "Value", "Efficiency" })] ValueTuple<T, float, float> x) => Mathf.Pow(x.Item2, CS$<>8__locals1.sizeFactor) * Mathf.Pow(x.Item3, CS$<>8__locals1.efficiencyFactor));
			}
			CS$<>8__locals3.selectedCandidate = enumerable.SelectRandomWeightedItem<ValueTuple<T, float, float>>(func, -1f, 1E-37f).Item1;
			list.RemoveAll(([TupleElementNames(new string[] { "Candidate", "Value", "Efficiency" })] ValueTuple<T, float, float> x) => x.Item1 == CS$<>8__locals2.selectedCandidate);
			TradeOffer tradeOffer = CS$<>8__locals1.dynamicOffer.Copy();
			Action<TradeOffer, T> action;
			action(tradeOffer, CS$<>8__locals2.selectedCandidate);
			bool flag = TradeAI.IsAgreementAcceptable(new ValueTuple<TradeOffer, TradeOffer>(CS$<>8__locals1.fixedOffer, tradeOffer), CS$<>8__locals1.takeFromFaction, CS$<>8__locals1.giveToFaction);
			float categoryScore = TradeAI.GetCategoryScore(new ValueTuple<TradeOffer, TradeOffer>(CS$<>8__locals1.fixedOffer, tradeOffer), CS$<>8__locals1.category, CS$<>8__locals1.takeFromFaction, CS$<>8__locals1.giveToFaction, TradeAI.CategoryScoreType.Net);
			if (flag && categoryScore > 0f)
			{
				CS$<>8__locals1.dynamicOffer.BecomeCopyOf(tradeOffer);
				int itemsAddedCount = CS$<>8__locals1.itemsAddedCount;
				CS$<>8__locals1.itemsAddedCount = itemsAddedCount + 1;
			}
			else if (++num >= num2)
			{
				break;
			}
		}
		return CS$<>8__locals1.<TryToBalanceCategory>g__IsBalanced|0(CS$<>8__locals1.dynamicOffer);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00019930 File Offset: 0x00017B30
	public static float GetMaximumTradeQuantity(TIFactionState faction, FactionResource resource)
	{
		if (faction.isActivePlayer)
		{
			return faction.GetCurrentResourceAmount(resource);
		}
		float num = 0.4f;
		float num2 = float.PositiveInfinity;
		if (resource != FactionResource.Boost)
		{
			if (resource != FactionResource.Antimatter)
			{
				if (resource == FactionResource.Exotics)
				{
					num = 0.15f;
				}
			}
			else
			{
				num = 0.25f;
			}
		}
		else
		{
			num2 = 0.3f;
		}
		float num3 = faction.GetCurrentResourceAmount(resource) * num;
		float num4 = float.PositiveInfinity;
		if (num2 != float.PositiveInfinity)
		{
			float num5 = 180f;
			float num6 = faction.GetFilteredTransactions(ref num5, "Trade Credit", resource, null).Sum<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount);
			float num7 = -faction.GetFilteredTransactions(ref num5, "Trade Debit", resource, null).Sum<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount);
			float num8 = num6 - num7;
			num4 = (from x in faction.GetFilteredTransactions(ref num5, null, resource, null)
				where x.Amount > 0f
				select x).Sum<TIFactionState.Transaction>((TIFactionState.Transaction x) => x.Amount) * num2;
			num4 = Mathf.Max(num4 - num8, 0f);
		}
		return Mathf.Min(num3, num4);
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00019A78 File Offset: 0x00017C78
	private static float GetDistrust(TIFactionState judge, TIFactionState otherFaction)
	{
		if (judge.permanentAlly(otherFaction))
		{
			return 1.025f;
		}
		float num = 1f + TINationState.GetIdeologicalDistance(judge, otherFaction) * 0.1f;
		float num2;
		if (judge.isActivePlayer)
		{
			num2 = AIEvaluators.FactionsGoToWarProgress(otherFaction, judge);
		}
		else
		{
			num2 = AIEvaluators.FactionsGoToWarProgress(judge, otherFaction);
		}
		num *= 1f + num2 / 2f;
		TIFactionState strongestHumanFaction = AIEvaluators.GetStrongestHumanFaction(null);
		if (judge == strongestHumanFaction)
		{
			num = (num - 1f) * 0.5f + 1f;
		}
		else if (otherFaction == strongestHumanFaction)
		{
			num *= 1.2f;
		}
		return num;
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00019B0C File Offset: 0x00017D0C
	private static float GetCategoryScore(TradeOffer.TradeAgreement agreement, TradeAI.CategoryType category, TIFactionState agreementScorer, TIFactionState otherFaction, TradeAI.CategoryScoreType categoryScoreType = TradeAI.CategoryScoreType.Net)
	{
		TradeOffer offer = agreement.GetOffer(agreementScorer);
		TradeOffer offer2 = agreement.GetOffer(otherFaction);
		float distrust = TradeAI.GetDistrust(agreementScorer, otherFaction);
		float num = 0f;
		float num2 = 0f;
		switch (category)
		{
		case TradeAI.CategoryType.Orgs:
			break;
		case TradeAI.CategoryType.Resources:
		{
			using (IEnumerator<FactionResource> enumerator = agreement.ResourcesTraded.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					FactionResource factionResource = enumerator.Current;
					float resourceQuantityReceived = agreement.GetResourceQuantityReceived(agreementScorer, factionResource);
					float tradeValue = TradeAI.GetTradeValue(agreementScorer, factionResource, resourceQuantityReceived, 0f);
					if (tradeValue < 0f)
					{
						num += tradeValue * distrust;
					}
					else
					{
						num2 += tradeValue;
					}
				}
				goto IL_01A3;
			}
			break;
		}
		case TradeAI.CategoryType.Projects:
			num2 = offer2.projects.Sum<TIProjectTemplate>((TIProjectTemplate x) => TradeAI.GetTradeValue(x, agreementScorer));
			num = offer.projects.Sum<TIProjectTemplate>((TIProjectTemplate x) => TradeAI.GetTradeValue(x, agreementScorer)) * -(distrust - 0.5f);
			goto IL_01A3;
		case TradeAI.CategoryType.Habs:
			num2 = offer2.habs.Sum<TIHabState>((TIHabState x) => TradeAI.GetTradeValue(x, agreementScorer));
			num = offer.habs.Sum<TIHabState>((TIHabState x) => TradeAI.GetTradeValue(x, agreementScorer)) * -(distrust * 1.09f);
			goto IL_01A3;
		case TradeAI.CategoryType.Diplomatic:
			num2 = TradeAI.GetTradeValue(offer.treatyType, agreementScorer, otherFaction);
			goto IL_01A3;
		default:
			goto IL_01A3;
		}
		num2 = offer2.orgs.Sum<TIOrgState>((TIOrgState x) => TradeAI.GetTradeValue(x, agreementScorer));
		num = offer.orgs.Sum<TIOrgState>((TIOrgState x) => TradeAI.GetTradeValue(x, agreementScorer)) * -(distrust * 1.07f);
		IL_01A3:
		switch (categoryScoreType)
		{
		case TradeAI.CategoryScoreType.Given:
			return num;
		case TradeAI.CategoryScoreType.Received:
			return num2;
		}
		return num2 + num;
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x00019CEC File Offset: 0x00017EEC
	private static float GetCategoryScore(TradeOffer offer, TradeAI.CategoryType category, TIFactionState offerScorer, TIFactionState otherFaction)
	{
		return TradeAI.GetCategoryScore(new ValueTuple<TradeOffer, TradeOffer>(offer, null), category, offerScorer, otherFaction, TradeAI.CategoryScoreType.Net);
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x00019D04 File Offset: 0x00017F04
	public static float ScoreAgreement(TradeOffer.TradeAgreement agreement, TIFactionState scorer)
	{
		TIFactionState otherFaction = agreement.Factions.FirstOrDefault<TIFactionState>((TIFactionState x) => x != scorer);
		List<ValueTuple<TradeAI.CategoryType, float>> list = ((TradeAI.CategoryType[])Enum.GetValues(typeof(TradeAI.CategoryType))).Select<TradeAI.CategoryType, ValueTuple<TradeAI.CategoryType, float>>((TradeAI.CategoryType x) => new ValueTuple<TradeAI.CategoryType, float>(x, TradeAI.GetCategoryScore(agreement, x, scorer, otherFaction, TradeAI.CategoryScoreType.Net))).ToList<ValueTuple<TradeAI.CategoryType, float>>();
		float num = list.Where<ValueTuple<TradeAI.CategoryType, float>>(([TupleElementNames(new string[] { "Category", "Score" })] ValueTuple<TradeAI.CategoryType, float> x) => x.Item2 > 0f).Sum<ValueTuple<TradeAI.CategoryType, float>>(([TupleElementNames(new string[] { "Category", "Score" })] ValueTuple<TradeAI.CategoryType, float> x) => x.Item2);
		float num2 = list.Where<ValueTuple<TradeAI.CategoryType, float>>(([TupleElementNames(new string[] { "Category", "Score" })] ValueTuple<TradeAI.CategoryType, float> x) => x.Item2 < 0f).Sum<ValueTuple<TradeAI.CategoryType, float>>(([TupleElementNames(new string[] { "Category", "Score" })] ValueTuple<TradeAI.CategoryType, float> x) => x.Item2);
		return 1f * num + 3f * num2;
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00019E28 File Offset: 0x00018028
	[CompilerGenerated]
	internal static IEnumerable<T> <MakeDemands>g__SelectTradeItems|3_0<T>(IEnumerable<T> options, Func<T, bool> Predicate, Func<T, float> GetScore, Func<T, float> GetTradeEfficiency, int count, float sizeWeight, float efficiencyWeight)
	{
		return from x in TradeAI.FilterTradeCandidatesForEfficiency<ValueTuple<T, float, float>>((from x in options.Where<T>(Predicate)
				select new ValueTuple<T, float, float>(x, GetScore(x), GetTradeEfficiency(x))).ToList<ValueTuple<T, float, float>>(), ([TupleElementNames(new string[] { "Item", "Score", "Efficiency" })] ValueTuple<T, float, float> x) => x.Item3, 1).ToList<ValueTuple<T, float, float>>().SelectRandomWeightedItems<ValueTuple<T, float, float>>(([TupleElementNames(new string[] { "Item", "Score", "Efficiency" })] ValueTuple<T, float, float> x) => Mathf.Pow(x.Item2, sizeWeight) * Mathf.Pow(x.Item3, efficiencyWeight), count, true)
				.ToList<ValueTuple<T, float, float>>()
			select x.Item1;
	}

	// Token: 0x0400055A RID: 1370
	private static Dictionary<TIFactionState, Dictionary<FactionResource, TradeAI.ResourceTradeData>> cachedResourceTradeData = new Dictionary<TIFactionState, Dictionary<FactionResource, TradeAI.ResourceTradeData>>();

	// Token: 0x0400055B RID: 1371
	private static int tradeDataCachedFrame = -1;

	// Token: 0x02000AF3 RID: 2803
	private enum CategoryType
	{
		// Token: 0x04004908 RID: 18696
		None,
		// Token: 0x04004909 RID: 18697
		Orgs,
		// Token: 0x0400490A RID: 18698
		Resources,
		// Token: 0x0400490B RID: 18699
		Projects,
		// Token: 0x0400490C RID: 18700
		Habs,
		// Token: 0x0400490D RID: 18701
		Diplomatic
	}

	// Token: 0x02000AF4 RID: 2804
	private struct ResourceTradeData
	{
		// Token: 0x0400490E RID: 18702
		public float RevenuePerDay;

		// Token: 0x0400490F RID: 18703
		public float CostPerDay;

		// Token: 0x04004910 RID: 18704
		public float StorageDays;
	}

	// Token: 0x02000AF5 RID: 2805
	private enum CategoryScoreType
	{
		// Token: 0x04004912 RID: 18706
		Net,
		// Token: 0x04004913 RID: 18707
		Given,
		// Token: 0x04004914 RID: 18708
		Received
	}
}
