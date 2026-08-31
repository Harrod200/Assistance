using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Tasks;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020007F1 RID: 2033
	public static class AIEvaluators
	{
		// Token: 0x06004924 RID: 18724 RVA: 0x001E1680 File Offset: 0x001DF880
		public static float GetAIRelativeValuation(FactionResource resource)
		{
			if (resource != FactionResource.Boost)
			{
				return AIEvaluators._AIRelativeValuation[resource];
			}
			if (GameStateManager.Mars().surfaceBases.Count > 15)
			{
				return AIEvaluators._AIRelativeValuation[resource] / 6f;
			}
			return AIEvaluators._AIRelativeValuation[resource];
		}

		// Token: 0x06004925 RID: 18725 RVA: 0x001E16D0 File Offset: 0x001DF8D0
		public static void ClearStaticData()
		{
			AIEvaluators.cachedSystemFleetStrengths.Clear();
			AIEvaluators.systemFleetStrengthsCachedDate = null;
			AIEvaluators.factionStrengthEstimatesCachedDate = null;
			AIEvaluators.cachedThreatLevels_all.Clear();
			AIEvaluators.threatLevelCachedDate_all = null;
			AIEvaluators.cachedThreatLevels_warEnemiesOnly.Clear();
			AIEvaluators.threatLevelCachedDate_warEnemiesOnly = null;
			AIEvaluators.typicalSTOFighterCachedDate = null;
			AIEvaluators.obsoleteProjects.Clear();
			AIEvaluators.upkeepInsecurityCache = null;
			AIEvaluators.cachedCriticalResources.Clear();
			AIEvaluators.cachedTechTiers.Clear();
			AIEvaluators.techTiersCacheDate = null;
		}

		// Token: 0x06004926 RID: 18726 RVA: 0x001E1743 File Offset: 0x001DF943
		public static float FixedResourceValue(TIFactionState faction, FactionResource resource, float value, bool scale)
		{
			return AIEvaluators.EvaluateMonthlyResourceIncome(faction, resource, value) / (scale ? 12f : 1f);
		}

		// Token: 0x06004927 RID: 18727 RVA: 0x001E1760 File Offset: 0x001DF960
		public static float EvaluateMonthlyResourceIncome(TIFactionState faction, FactionResource resource, float value)
		{
			float num = 0f;
			if (value != 0f)
			{
				num = AIEvaluators.GetAIRelativeValuation(resource) * value * (faction.resourceIncomeDeficiencies.Contains(resource) ? 3f : 1f);
				switch (resource)
				{
				case FactionResource.Money:
					if (value > 0f)
					{
						num *= faction.aiValues.gatherMoney;
					}
					else if (value * -1f > faction.GetMonthlyIncome(FactionResource.Money, false, false))
					{
						num *= -num;
					}
					break;
				case FactionResource.Influence:
					num *= ((value > 0f) ? faction.aiValues.gatherInfluence : 1f);
					break;
				case FactionResource.Operations:
				{
					bool flag = false;
					if (faction.currentlySearchingForHydraCouncilor)
					{
						num *= 7f;
						flag = true;
					}
					else if (!faction.veryProAlien && !faction.MilestoneCompleted(CampaignMilestone.AccessLiveHydra))
					{
						num *= 3f;
						flag = true;
					}
					else if (faction.veryProAlien && !faction.MilestoneCompleted(CampaignMilestone.AlienDiplomacy))
					{
						num *= 3f;
						flag = true;
					}
					if (flag && faction.GetCurrentResourceAmount(FactionResource.Operations) < 100f)
					{
						num *= 3f;
					}
					num *= ((value > 0f) ? faction.aiValues.gatherOps : 1f);
					break;
				}
				case FactionResource.Research:
				case FactionResource.Projects:
					if (faction.IsAlienFaction)
					{
						num = 0f;
					}
					num *= faction.aiValues.gatherScience;
					break;
				case FactionResource.Boost:
					if (value > 0f)
					{
						num *= faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability;
						if (faction.GetMonthlyIncome(FactionResource.Boost, true, true) > 0f && TIResourcesCost.basicSpaceResources.All<FactionResource>((FactionResource x) => faction.GetDailyIncome(x, true, true) >= 1f / AIEvaluators.GetAIRelativeValuation(x)))
						{
							num /= 5f;
						}
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
			}
			return num;
		}

		// Token: 0x06004928 RID: 18728 RVA: 0x001E19C4 File Offset: 0x001DFBC4
		public static float EvaluateMonthlyResourceIncome_Trade(TIFactionState faction, FactionResource resource, float quantityPerMonth, float permanence = 1f)
		{
			float num = quantityPerMonth;
			if (!TIResourcesCost.unAccumulatableResources.Contains(resource))
			{
				num *= 6f * permanence;
			}
			float num2 = Mathf.Abs(num);
			return TradeAI.GetTradeValue(faction, resource, num, num2);
		}

		// Token: 0x06004929 RID: 18729 RVA: 0x001E19FC File Offset: 0x001DFBFC
		public static int AbundantValue(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Money:
				return 100000;
			case FactionResource.Influence:
				return 1000;
			case FactionResource.Operations:
				return 500;
			case FactionResource.Research:
				return 15;
			case FactionResource.Projects:
				return 1;
			case FactionResource.Boost:
				return 200;
			case FactionResource.MissionControl:
				return 100;
			case FactionResource.Water:
			case FactionResource.Volatiles:
			case FactionResource.Metals:
				return 50000;
			case FactionResource.NobleMetals:
				return 10000;
			case FactionResource.Fissiles:
			case FactionResource.Exotics:
				return 1000;
			case FactionResource.Antimatter:
				return 10;
			default:
				return 1000;
			}
		}

		// Token: 0x0600492A RID: 18730 RVA: 0x001E1A88 File Offset: 0x001DFC88
		public static bool Abundant(TIFactionState faction, FactionResource resource, float stockpile, bool positiveIncome, float multiplier = 1f)
		{
			if (!positiveIncome)
			{
				return false;
			}
			if (resource - FactionResource.Research <= 1 || resource == FactionResource.MissionControl)
			{
				return faction.GetNetDailyIncome(resource, false) > (float)AIEvaluators.AbundantValue(resource) * ((multiplier > 1f) ? multiplier : TITimeState.CampaignDuration_years_Exact());
			}
			return stockpile >= (float)AIEvaluators.AbundantValue(resource) * multiplier;
		}

		// Token: 0x0600492B RID: 18731 RVA: 0x001E1AD9 File Offset: 0x001DFCD9
		public static bool Abundant(TIFactionState faction, FactionResource resource, float multiplier = 1f)
		{
			return AIEvaluators.Abundant(faction, resource, faction.GetCurrentResourceAmount(resource), faction.GetDailyIncome(resource, true, false) > 0f, multiplier);
		}

		// Token: 0x0600492C RID: 18732 RVA: 0x001E1AFC File Offset: 0x001DFCFC
		public static bool Deficient(TIFactionState faction, FactionResource resource, float dailyIncome, float stockpile, float thresholdValue, float campaignDuration_Years, Dictionary<FactionResource, Dictionary<TIFactionState, float>> factionIncomes)
		{
			if (AIEvaluators.Abundant(faction, resource, stockpile, dailyIncome > 0f, 1f))
			{
				return false;
			}
			if (resource != FactionResource.Boost && resource != FactionResource.MissionControl && thresholdValue > 0f && dailyIncome < thresholdValue * 0.67f)
			{
				return true;
			}
			switch (resource)
			{
			case FactionResource.Money:
				if (stockpile < 0f)
				{
					return true;
				}
				dailyIncome += faction.mediumTermDailySpoilsIncome;
				return dailyIncome < 4f * campaignDuration_Years || stockpile < dailyIncome * 90f;
			case FactionResource.Influence:
				return dailyIncome < Mathf.Max(2f, campaignDuration_Years / 4f);
			case FactionResource.Operations:
				return dailyIncome < Mathf.Max(0.25f, campaignDuration_Years / 10f);
			case FactionResource.Research:
				return dailyIncome < Mathf.Min(75f, campaignDuration_Years * 2f);
			case FactionResource.Boost:
			{
				if (dailyIncome < 0.2f)
				{
					return true;
				}
				bool flag = faction.LaggingInSpaceEconomy();
				bool flag2 = faction.NeedsSpaceBootstrap();
				return (flag || flag2) && dailyIncome <= 0.4f;
			}
			case FactionResource.MissionControl:
				return faction.LackingBasicMissionControl() || faction.AI_GenericMissionControlAvailable <= 20;
			}
			return dailyIncome <= 0f;
		}

		// Token: 0x0600492D RID: 18733 RVA: 0x001E1C24 File Offset: 0x001DFE24
		public static bool LackingBasicMissionControl(this TIFactionState faction)
		{
			return (float)faction.MissionControlIncomeSansHabIncome < 50f * faction.aiValues.wantSpaceFacilities;
		}

		// Token: 0x0600492E RID: 18734 RVA: 0x001E1C40 File Offset: 0x001DFE40
		public static float EvaluateControlPoint(TIFactionState faction, TIControlPoint controlPoint)
		{
			float num = 0f;
			TINationState nation = controlPoint.nation;
			num += nation.economyScore * nation.economyScore * nation.economyScore;
			num += (nation.spaceFlightProgram ? (100f * faction.aiValues.wantSpaceFacilities * faction.aiValues.wantSpaceWarCapability) : 0f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Research, nation.GetMonthlyResearchFromControlPoint(faction));
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Money, (nation.spaceFunding_year + nation.spaceFundingIncome_year) / 2f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Boost, (nation.rawBoostPerMonth_dekatons + nation.boostIncome_month_dekatons) / 2f * TemplateManager.global.spaceResourceToTons);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, (float)nation.GetMissionControlFromControlPoint(controlPoint.positionInNation));
			num += (nation.nuclearProgram ? (50f * faction.aiValues.wantEarthWarCapability) : 0f);
			num += (float)nation.GetNumArmiesAtControlPoint(controlPoint.positionInNation) * 25f * faction.aiValues.wantEarthWarCapability;
			num += nation.militaryTechLevel * 1.5f * faction.aiValues.wantEarthWarCapability;
			num += nation.spaceDefenseCoverage * 1000f;
			num += (nation.unrest + nation.unrestRestState) / 2f * ((nation.unrest + nation.unrestRestState) / 2f) * -4f * faction.aiValues.riskAversion;
			num += (controlPoint.nation.CouncilControlPointFraction(faction, true, false) + 1f / (float)controlPoint.nation.numControlPoints) * 5f;
			num *= (controlPoint.executive ? 2f : 1f);
			num *= 1f + Mathf.Max(0f, (nation.GetPublicOpinionOfFaction(faction.ideology) - 0.2f) * 1.25f);
			if (faction.lostControlPoints.ContainsKey(controlPoint))
			{
				float num2 = (float)TITimeState.Now().DifferenceInDays(faction.lostControlPoints[controlPoint]) / 30.436874f;
				num *= Mathf.Clamp(6f - num2, 1f, 4f);
			}
			return num;
		}

		// Token: 0x0600492F RID: 18735 RVA: 0x001E1E6C File Offset: 0x001E006C
		public static float EvaluateNation(TIFactionState faction, TINationState nation)
		{
			float num = 0f;
			num += nation.economyScore * nation.economyScore * nation.economyScore;
			float num2 = 100f * ((faction != null) ? faction.aiValues.wantSpaceFacilities : 1f) * ((faction != null) ? faction.aiValues.wantSpaceWarCapability : 1f);
			num += (nation.spaceFlightProgram ? num2 : 0f);
			float num3 = (90f - nation.BestBoostLatitude) / 3f;
			num += num3;
			if (faction != null)
			{
				num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Money, (nation.spaceFunding_year + nation.spaceFundingIncome_year) / 2f);
				num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Boost, (nation.rawBoostPerMonth_dekatons + nation.boostIncome_month_dekatons) / 2f * TemplateManager.global.spaceResourceToTons);
				num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Research, nation.research_month);
				num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, (float)nation.missionControl);
			}
			num += (nation.nuclearProgram ? (50f * ((faction != null) ? faction.aiValues.wantEarthWarCapability : 1f)) : 0f);
			num += (float)nation.armies.Count * 25f * ((faction != null) ? faction.aiValues.wantEarthWarCapability : 1f);
			num += nation.militaryTechLevel * 1.5f * ((faction != null) ? faction.aiValues.wantEarthWarCapability : 1f);
			return num + nation.spaceDefenseCoverage * 1000f;
		}

		// Token: 0x06004930 RID: 18736 RVA: 0x001E1FF0 File Offset: 0x001E01F0
		public static TIRegionState SelectAlienCrashdownRegion(bool advance, bool makeAHole = false)
		{
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			IEnumerable<TIFactionIdeologyTemplate> enumerable = from x in GameStateManager.AllFactions()
				where x.ideologyCoordinates.x < 0f
				select x into y
				select y.ideology;
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			List<SupraRegion> list = new List<SupraRegion>();
			foreach (TIFactionGoalState tifactionGoalState in tifactionState.GoalsOfType(GoalType.TransportCouncilorsViaFleet, false, true))
			{
				if (tifactionGoalState.target().isRegionState)
				{
					list.Add(tifactionGoalState.target().ref_region.mapRegionTemplate.supraRegion);
				}
			}
			foreach (TICouncilorState ticouncilorState in tifactionState.activeCouncilors)
			{
				if (ticouncilorState.OnEarth)
				{
					list.Add(ticouncilorState.ref_region.mapRegionTemplate.supraRegion);
				}
			}
			foreach (TIRegionState tiregionState in GameStateManager.AllRegions())
			{
				TIFactionState executiveFaction = tiregionState.nation.executiveFaction;
				bool flag = executiveFaction == null || !enumerable.Contains(executiveFaction.ideology);
				if (makeAHole || !tiregionState.antiSpaceDefenses || !flag)
				{
					if (!advance)
					{
						if (tiregionState.nation.TotalOwningFaction != null)
						{
							if (tiregionState.nation.TotalOwningFaction.ideology.alien)
							{
								dictionary.Add(tiregionState, tiregionState.antiSpaceDefenses ? 10000f : 100f);
							}
							else if (tiregionState.nation.TotalOwningFaction.IsAlienProxy)
							{
								dictionary.Add(tiregionState, tiregionState.antiSpaceDefenses ? 100f : (tiregionState.coreEconomicRegion ? 0.1f : 1f));
							}
						}
					}
					else if (!tiregionState.alienCrashdown.crashdownPresent)
					{
						float num = (float)(tiregionState.AdjacentRegions(true).Count - 3);
						if (num > 0f)
						{
							num += (float)tiregionState.nation.AdjacentNations(true).Count;
						}
						if (tiregionState.coreEconomicRegion)
						{
							num *= 0.01f;
						}
						if (tiregionState.nation.capital == tiregionState)
						{
							if (tiregionState.nation.regions.Count > 1)
							{
								num *= 0.5f;
							}
							else
							{
								num *= 0.75f;
							}
						}
						if (tiregionState.colonyRegion)
						{
							num *= 2f;
						}
						if (list.Contains(tiregionState.mapRegionTemplate.supraRegion))
						{
							num *= 0.0001f;
						}
						if (tiregionState.mapRegionTemplate.supraRegion == SupraRegion.Oceania)
						{
							num *= 1E-20f;
						}
						AIEvaluators.ShouldAliensGoLoud();
						if (num > 0f)
						{
							dictionary.Add(tiregionState, num);
						}
					}
				}
			}
			if (dictionary.Count == 0)
			{
				return GameStateManager.AllRegions().SelectRandomItem<TIRegionState>();
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> j) => j.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06004931 RID: 18737 RVA: 0x001E2378 File Offset: 0x001E0578
		public static TIRegionState SelectAlienArmyLandingRegion(bool makeAHole = false)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			TIOperationTemplate landArmyOperation = OperationsManager.operationsLookup[typeof(AlienLandArmyOperation)].GetTemplate();
			Dictionary<TIRegionState, float> dictionary = new Dictionary<TIRegionState, float>();
			TIRegionState[] array = GameStateManager.AllRegions();
			for (int i = 0; i < array.Length; i++)
			{
				TIRegionState region2 = array[i];
				float num = 0f;
				Func<OperationData, bool> <>9__4;
				if (!region2.alienLanding.landingPresent && tifactionState.fleets.None<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					IEnumerable<OperationData> enumerable = x.CurrentOperations();
					Func<OperationData, bool> func;
					if ((func = <>9__4) == null)
					{
						func = (<>9__4 = (OperationData x) => x.target == region2 && x.operationDataName == landArmyOperation.dataName);
					}
					return enumerable.Any<OperationData>(func);
				}))
				{
					if (region2.nation.alienNation)
					{
						num = 1000000f;
					}
					else if (region2.nation.allies.Contains(GameStateManager.AlienNation()))
					{
						num = 10000f;
					}
					else if ((!region2.antiSpaceDefenses || makeAHole) && region2.armies.Count == 0)
					{
						if (GameStateManager.AlienNation().extant && region2.AdjacentNations(false, true).Contains(GameStateManager.AlienNation()))
						{
							num = 10000f;
						}
						else if (region2.AdjacentRegions(true).Any<TIRegionState>((TIRegionState x) => x.alienLanding.landingPresent))
						{
							num = 10000f;
						}
						if (region2.nation.NumArmiesDefendingMe() > 0)
						{
							num += 1000f / region2.nation.militaryStrength;
						}
						else
						{
							num += (float)(region2.AdjacentRegions(true).Count * region2.nation.AdjacentNations(true).Count - 3);
						}
						if (region2.isCoastal)
						{
							num *= 0.5f;
						}
						if (region2.xenoforming.xenoformingLevel >= TIRegionXenoformingState.stage3Xenoforming)
						{
							num *= 2f;
						}
						if (region2.hasAlienFacility)
						{
							num *= 2f;
						}
						TIFactionState executiveFaction = region2.nation.executiveFaction;
						if (executiveFaction != null && !executiveFaction.permanentAlly(tifactionState) && region2.nation.NumNuclearWeaponsDefendingMe() > 0)
						{
							num *= 0.1f;
						}
					}
				}
				if (num > 0f)
				{
					dictionary.Add(region2, num);
				}
			}
			if (dictionary.Count == 0)
			{
				return (from region in GameStateManager.AllRegions()
					where !region.alienLanding.landingPresent
					select region).SelectRandomItem<TIRegionState>();
			}
			return dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> j) => j.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06004932 RID: 18738 RVA: 0x001E2684 File Offset: 0x001E0884
		public static float ScoreNuclearTarget(TINationState targetingNation, TIRegionState targetRegion, TINationState targetNation)
		{
			float num = 1E-05f;
			List<TIArmyState> list = targetRegion.armies.Where<TIArmyState>((TIArmyState x) => x.homeNation == targetNation || x.homeNation.allies.Contains(targetNation)).ToList<TIArmyState>();
			float num2 = num;
			float num3;
			if (list == null)
			{
				num3 = 0f;
			}
			else
			{
				num3 = list.Sum<TIArmyState>((TIArmyState x) => x.techLevel * x.combatEffectiveness);
			}
			num = num2 + num3;
			if (targetRegion == targetNation.capital)
			{
				num += 1f;
			}
			if (targetingNation.regions.Contains(targetRegion) && list.Count > 0)
			{
				TINationState tinationState;
				if (!targetRegion.OccupiedOrOccupationUnderway() || (double)targetRegion.GetHighestWarAllianceOccupationValueByNation(targetNation, out tinationState) < 0.65)
				{
					if (!list.Any<TIArmyState>((TIArmyState x) => x.InBattleWithOtherArmiesAndWinningByALot()))
					{
						return num;
					}
				}
				if (targetRegion == targetNation.capital)
				{
					num += 80f;
				}
				else
				{
					num += 30f;
				}
			}
			return num;
		}

		// Token: 0x06004933 RID: 18739 RVA: 0x001E2794 File Offset: 0x001E0994
		public static bool IsUsefulForBoost(this TINationState nation)
		{
			if (nation.BestBoostLatitude <= 25f)
			{
				return true;
			}
			if (nation.BestBoostLatitude <= 50f && nation.spaceFlightProgram)
			{
				TIFactionState executiveFaction = nation.executiveFaction;
				return executiveFaction != null && executiveFaction.NeedsSpaceBootstrap();
			}
			return false;
		}

		// Token: 0x06004934 RID: 18740 RVA: 0x001E27CD File Offset: 0x001E09CD
		public static float CalculateRiskUtility(TIFactionState faction, float chanceOfSuccess)
		{
			return faction.aiValues.riskAversion * 1f / (1f + 1f * Mathf.Pow(2f, -10f * chanceOfSuccess + 7f));
		}

		// Token: 0x06004935 RID: 18741 RVA: 0x001E2804 File Offset: 0x001E0A04
		public static float EvaluateMissionTemplateUtility(TICouncilorState councilor, TIMissionTemplate mission, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions)
		{
			return mission.utilityScore * councilor.MissionQuality(mission) * (float)(missingRequiredMissions.Contains(mission) ? 50 : (requiredMissions.Contains(mission) ? 3 : 1));
		}

		// Token: 0x06004936 RID: 18742 RVA: 0x001E2830 File Offset: 0x001E0A30
		public static float EvaluateOrgForCouncilor(TIOrgState org, TICouncilorState councilor, List<TIMissionTemplate> possibleMissions, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, bool acquiring, Dictionary<FactionResource, float> councilorIncomes, bool chasingHydra, int factionWars, bool chasingNeutralNations, bool criticalAdminNeed = false)
		{
			if (org.template == councilor.faction.winningOrgTemplate)
			{
				if (councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => x.restrictedLocations > RestrictedLocations.None))
				{
					return -100f;
				}
			}
			if (acquiring && org.tier - org.administration > councilor.GetAttribute(CouncilorAttribute.Administration, true, true, false, false, false, false))
			{
				return 0f;
			}
			if (org.administration <= org.tier)
			{
				if (org.adjustedIncomeMoney_month < 0f && -org.adjustedIncomeMoney_month > councilor.faction.GetMonthlyIncome(FactionResource.Money, false, false))
				{
					return org.adjustedIncomeMoney_month;
				}
				if (org.adjustedIncomeInfluence_month < 0f && -org.adjustedIncomeInfluence_month > councilor.faction.GetMonthlyIncome(FactionResource.Influence, false, false))
				{
					return org.adjustedIncomeInfluence_month;
				}
				if (org.adjustedIncomeOps_month < 0f && -org.adjustedIncomeOps_month > councilor.faction.GetMonthlyIncome(FactionResource.Operations, false, false))
				{
					return org.adjustedIncomeOps_month;
				}
				if (org.adjustedIncomeBoost_month < 0f && -org.adjustedIncomeBoost_month > councilor.faction.GetMonthlyIncome(FactionResource.Boost, false, false))
				{
					return org.adjustedIncomeBoost_month;
				}
			}
			bool flag = councilor.orgs.Contains(org);
			float num = 0f;
			TIFactionState faction = councilor.faction;
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Money, org.adjustedIncomeMoney_month) * ((org.adjustedIncomeMoney_month > 0f) ? councilor.GetResourceMultiplierFromAttributes(FactionResource.Money) : 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Influence, org.adjustedIncomeInfluence_month) * ((org.adjustedIncomeInfluence_month > 0f) ? councilor.GetResourceMultiplierFromAttributes(FactionResource.Influence) : 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Boost, org.adjustedIncomeBoost_month);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.MissionControl, org.incomeMissionControl);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Operations, org.adjustedIncomeOps_month) * ((org.adjustedIncomeOps_month > 0f) ? councilor.GetResourceMultiplierFromAttributes(FactionResource.Operations) : 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Research, org.adjustedIncomeResearch_month) * councilor.GetResourceMultiplierFromAttributes(FactionResource.Research);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Projects, (float)org.projectCapacityGranted);
			num += 5f * faction.aiValues.gatherInfluence * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Persuasion, org.persuasion, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag);
			num += 5f * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Investigation, org.investigation, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag);
			num += 5f * faction.aiValues.gatherOps * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Command, org.command, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag);
			num += 5f * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Espionage, org.espionage, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag);
			num += (float)((criticalAdminNeed && org.administration > org.tier) ? 20 : 5) * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Administration, org.administration, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag);
			num += 5f * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Science, org.science, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag) * faction.aiValues.gatherScience;
			num += 5f * AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, CouncilorAttribute.Security, org.security, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, flag) * faction.aiValues.protectCouncilors;
			num += org.unityBonus * 100f * faction.aiValues.wantPopularity;
			num += org.oppressionBonus * 100f * faction.aiValues.wantPopularity;
			num += org.militaryBonus * 120f * faction.aiValues.wantEarthWarCapability;
			if (councilor.faction.IsActiveHumanFaction)
			{
				num += org.economyBonus * 100f;
				num += org.miningBonus * 30000f;
				num += org.welfareBonus * 100f;
				num += org.environmentBonus * 100f * faction.aiValues.protectHumanLife;
				num += org.knowledgeBonus * 100f * faction.aiValues.gatherScience;
				num += org.governmentBonus * 100f * faction.aiValues.gatherScience;
				num += org.spoilsBonus * 50f * faction.aiValues.gatherMoney * (float)(faction.cynical ? 2 : 1);
				num += org.spaceDevBonus * 100f * faction.aiValues.gatherMoney;
				num += org.spaceflightBonus * 300f * Mathf.Max(faction.aiValues.wantSpaceFacilities, faction.aiValues.wantSpaceWarCapability);
				num += org.MCBonus * 300f * Mathf.Max(faction.aiValues.wantSpaceFacilities, faction.aiValues.wantSpaceWarCapability);
				List<TIMissionTemplate> list = possibleMissions ?? councilor.GetPossibleMissionList(false, false, false, null, false);
				foreach (TIMissionTemplate timissionTemplate in org.missionsGranted)
				{
					if (!list.Contains(timissionTemplate))
					{
						num += AIEvaluators.EvaluateMissionTemplateUtility(councilor, timissionTemplate, requiredMissions, missingRequiredMissions);
					}
				}
				if (org.projectGranted != null && !faction.completedProjects.Contains(org.projectGranted))
				{
					num += org.projectGranted.GetResearchCost(faction) / 100f;
				}
				foreach (TechBonus techBonus in org.techBonuses)
				{
					num += techBonus.bonus * 100f;
				}
			}
			return num;
		}

		// Token: 0x06004937 RID: 18743 RVA: 0x001E2E10 File Offset: 0x001E1010
		public static float EvaluateOrgForTrade(TIOrgState org, TIFactionState faction)
		{
			bool currentlyDetectingHydra = faction.currentlyDetectingHydra;
			float num = 50f;
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.Money, org.adjustedIncomeMoney_month, 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.Influence, org.adjustedIncomeInfluence_month, 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.Boost, org.adjustedIncomeBoost_month, 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.MissionControl, org.incomeMissionControl, 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.Operations, org.adjustedIncomeOps_month, 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.Research, org.adjustedIncomeResearch_month, 1f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, FactionResource.Projects, (float)org.projectCapacityGranted, 1f);
			num += (float)(org.persuasion * 50) * faction.aiValues.gatherInfluence;
			num += (float)(org.investigation * 50 * (currentlyDetectingHydra ? 5 : 1));
			num += (float)(org.command * 50) * faction.aiValues.gatherOps;
			num += (float)(org.espionage * 50 * (currentlyDetectingHydra ? 4 : 1));
			num += (float)(org.administration * 50) * faction.aiValues.gatherMoney + (float)(org.administration * 400);
			num += (float)(org.science * 100) * faction.aiValues.gatherScience;
			num += (float)(org.security * 50) * faction.aiValues.protectCouncilors;
			foreach (TIMissionTemplate timissionTemplate in org.missionsGranted)
			{
				num += 100f;
			}
			if (org.projectGranted != null && !faction.completedProjects.Contains(org.projectGranted))
			{
				num += org.projectGranted.GetResearchCost(faction) / 10f;
			}
			foreach (TechBonus techBonus in org.techBonuses)
			{
				num += techBonus.bonus * 1000f;
			}
			num += org.economyBonus * 100f;
			num += org.welfareBonus * 100f;
			num += org.environmentBonus * 100f * faction.aiValues.protectHumanLife;
			num += org.knowledgeBonus * 100f * faction.aiValues.gatherScience;
			num += org.governmentBonus * 100f * faction.aiValues.gatherScience;
			num += org.unityBonus * 100f * faction.aiValues.wantPopularity;
			num += org.oppressionBonus * 100f * faction.aiValues.wantPopularity;
			num += org.militaryBonus * 100f * faction.aiValues.wantEarthWarCapability;
			num += org.spoilsBonus * 50f * faction.aiValues.gatherMoney * (float)(faction.cynical ? 2 : 1);
			num += org.spaceDevBonus * 100f * faction.aiValues.gatherMoney;
			num += org.spaceflightBonus * 100f * faction.aiValues.wantSpaceFacilities;
			num += org.MCBonus * 100f * faction.aiValues.wantSpaceWarCapability;
			num += org.miningBonus * 2000f;
			return 2f * num;
		}

		// Token: 0x06004938 RID: 18744 RVA: 0x001E3170 File Offset: 0x001E1370
		public static float EvaluateTrait(TICouncilorState councilor, TIFactionState faction, TITraitTemplate trait, List<TIMissionTemplate> possibleMissions, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, Dictionary<FactionResource, float> councilorIncomes, bool chasingHydra, int factionWars, bool chasingNeutralNations)
		{
			float num = 0f;
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Money, trait.incomeMoney);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Influence, trait.incomeInfluence);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Research, trait.incomeResearch);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Operations, trait.incomeOps);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Boost, trait.incomeBoost);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(councilor.faction, FactionResource.Projects, (float)trait.incomeProjects);
			switch (trait.specialTraitRule)
			{
			case SpecialTraitRule.Government:
				num += (float)(8 + councilor.homeNation.numControlPoints_unclamped);
				break;
			case SpecialTraitRule.Criminal:
				num += 8f;
				break;
			case SpecialTraitRule.LoyaltyMonitor:
				num += ((float)TemplateManager.global.maxCouncilorAttribute - councilor.faction.GetViewofCouncilor(councilor).GetAttribute(CouncilorAttribute.Loyalty)) / 12f;
				break;
			case SpecialTraitRule.Undercover:
				num += 15f;
				break;
			case SpecialTraitRule.Survivor:
				num += 2f;
				break;
			case SpecialTraitRule.HardTarget:
				num += 8f;
				break;
			}
			num += (float)trait.detectionInvBonus;
			num += (float)trait.detectionEspBonus;
			num += trait.MissionsGranted.Sum<TIMissionTemplate>((TIMissionTemplate x) => AIEvaluators.EvaluateMissionTemplateUtility(councilor, x, requiredMissions, missingRequiredMissions));
			num -= trait.RestrictedMissions.Sum<TIMissionTemplate>((TIMissionTemplate x) => AIEvaluators.EvaluateMissionTemplateUtility(councilor, x, requiredMissions, missingRequiredMissions));
			foreach (StatModifier statModifier in trait.statMods.Where<StatModifier>((StatModifier x) => x.stat != CouncilorAttribute.None && x.operation == StatModSetOperation.Additive))
			{
				num += AIEvaluators.EvaluateStatIncreaseUtility(councilor, faction, statModifier.stat, statModifier.modifierValue, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, false) * ((statModifier.condition != null) ? 0.5f : 1f);
			}
			num += trait.techBonuses.Sum<TechBonus>((TechBonus x) => x.bonus * 20f);
			num += trait.priorityBonuses.Sum<PriorityBonus>((PriorityBonus x) => x.bonus * 15f);
			num += ((trait.XPModifier != 0f) ? (trait.XPModifier * -100f) : 0f);
			num -= (float)((trait.restrictedLocations != RestrictedLocations.None) ? 10 : 0);
			return num;
		}

		// Token: 0x06004939 RID: 18745 RVA: 0x001E3458 File Offset: 0x001E1658
		public static float ScoreTraitConditionals(TICouncilorState councilor, TIFactionState faction, TITraitTemplate trait, List<TIMissionTemplate> possibleMissions, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, Dictionary<FactionResource, float> councilorIncomes, bool chasingHydra, int factionWars, bool chasingNeutralNations)
		{
			float num = 0f;
			num += (float)((trait.specialTraitRule != SpecialTraitRule.None) ? 10 : 0);
			if (trait.isGovernmentTrait)
			{
				num += (float)(councilor.homeNation.numControlPoints - 1);
			}
			num += (float)trait.detectionInvBonus;
			num += (float)trait.detectionEspBonus;
			num += trait.MissionsGranted.Sum<TIMissionTemplate>((TIMissionTemplate x) => AIEvaluators.EvaluateMissionTemplateUtility(councilor, x, requiredMissions, missingRequiredMissions));
			num -= trait.RestrictedMissions.Sum<TIMissionTemplate>((TIMissionTemplate x) => AIEvaluators.EvaluateMissionTemplateUtility(councilor, x, requiredMissions, missingRequiredMissions));
			foreach (StatModifier statModifier in trait.statMods.Where<StatModifier>((StatModifier x) => x.stat != CouncilorAttribute.None && x.condition != null))
			{
				num += AIEvaluators.EvaluateStatIncreaseUtility(councilor, faction, statModifier.stat, statModifier.modifierValue, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, false) * 0.5f;
			}
			num += trait.techBonuses.Sum<TechBonus>((TechBonus x) => x.bonus * 10f);
			num -= (float)((trait.restrictedLocations != RestrictedLocations.None) ? 10 : 0);
			return num;
		}

		// Token: 0x0600493A RID: 18746 RVA: 0x001E35D0 File Offset: 0x001E17D0
		public static Dictionary<TICouncilorState, float> EvaluateCandidateCouncilors(TIFactionState faction, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, CouncilorAttribute lackingAttribute, bool chasingHydra, int factionWars, bool chasingNeutralNations)
		{
			AIEvaluators.<>c__DisplayClass43_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass43_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.requiredMissions = requiredMissions;
			CS$<>8__locals1.missingRequiredMissions = missingRequiredMissions;
			CS$<>8__locals1.chasingHydra = chasingHydra;
			CS$<>8__locals1.factionWars = factionWars;
			CS$<>8__locals1.chasingNeutralNations = chasingNeutralNations;
			Dictionary<TICouncilorState, float> dictionary = new Dictionary<TICouncilorState, float>();
			bool flag = CS$<>8__locals1.factionWars == 0 && CS$<>8__locals1.faction.IsActiveHumanFaction && TITimeState.CampaignDuration_years_Exact() < 1f && CS$<>8__locals1.faction.GoalsOfType(GoalType.CaptureNationDirty, false, true).Count == 0 && CS$<>8__locals1.faction.GoalsOfType(GoalType.NeutralizeNation, false, true).Count == 0;
			using (List<TICouncilorState>.Enumerator enumerator = CS$<>8__locals1.faction.availableCouncilors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AIEvaluators.<>c__DisplayClass43_1 CS$<>8__locals2 = new AIEvaluators.<>c__DisplayClass43_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.candidate = enumerator.Current;
					if (!flag || CS$<>8__locals2.candidate.typeTemplate.missionNames.Contains(TIFactionState.controlNationMission.dataName) || CS$<>8__locals2.candidate.typeTemplate.missionNames.Contains(TIFactionState.publicCampaignMission.dataName) || CS$<>8__locals2.candidate.typeTemplate.missionNames.Contains(TIFactionState.adviseMission.dataName))
					{
						float num = 0f;
						if (CS$<>8__locals2.candidate.typeTemplate.keyStat[0] == lackingAttribute)
						{
							num += 200f;
						}
						else if (CS$<>8__locals2.candidate.typeTemplate.keyStat[1] == lackingAttribute)
						{
							num += 100f;
						}
						foreach (TIMissionTemplate timissionTemplate in CS$<>8__locals2.candidate.typeTemplate.missions)
						{
							num += 25f * AIEvaluators.EvaluateMissionTemplateUtility(CS$<>8__locals2.candidate, timissionTemplate, CS$<>8__locals2.CS$<>8__locals1.requiredMissions, CS$<>8__locals2.CS$<>8__locals1.missingRequiredMissions);
						}
						Dictionary<FactionResource, float> councilorIncomes = TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => CS$<>8__locals2.candidate.GetMonthlyIncome(x));
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.Money, councilorIncomes[FactionResource.Money]);
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.Influence, councilorIncomes[FactionResource.Influence]);
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.Operations, councilorIncomes[FactionResource.Operations]);
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.Research, councilorIncomes[FactionResource.Research]);
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.Boost, councilorIncomes[FactionResource.Boost]);
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.MissionControl, councilorIncomes[FactionResource.MissionControl]);
						num += AIEvaluators.EvaluateMonthlyResourceIncome(CS$<>8__locals2.CS$<>8__locals1.faction, FactionResource.Projects, councilorIncomes[FactionResource.Projects]);
						num += CS$<>8__locals2.candidate.traits.Sum<TITraitTemplate>((TITraitTemplate x) => AIEvaluators.ScoreTraitConditionals(CS$<>8__locals2.candidate, CS$<>8__locals2.CS$<>8__locals1.faction, x, CS$<>8__locals2.candidate.typeTemplate.missions, CS$<>8__locals2.CS$<>8__locals1.requiredMissions, CS$<>8__locals2.CS$<>8__locals1.missingRequiredMissions, councilorIncomes, CS$<>8__locals2.CS$<>8__locals1.chasingHydra, CS$<>8__locals2.CS$<>8__locals1.factionWars, CS$<>8__locals2.CS$<>8__locals1.chasingNeutralNations));
						TIResourcesCost tiresourcesCost = CS$<>8__locals2.candidate.HireRecruitCost(CS$<>8__locals2.CS$<>8__locals1.faction);
						float singleCostValue = tiresourcesCost.GetSingleCostValue(FactionResource.Influence);
						if (singleCostValue > 0f && CS$<>8__locals2.CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Influence) < (float)(2 * TemplateManager.global.baseCouncilorRecruitCost_influence))
						{
							num /= singleCostValue;
						}
						if (CS$<>8__locals2.CS$<>8__locals1.faction.emptyCouncilorSlots <= 2 || tiresourcesCost.CanAfford(CS$<>8__locals2.CS$<>8__locals1.faction, 1f, null, float.PositiveInfinity))
						{
							dictionary.Add(CS$<>8__locals2.candidate, num);
						}
					}
				}
			}
			return dictionary;
		}

		// Token: 0x0600493B RID: 18747 RVA: 0x001E3A8C File Offset: 0x001E1C8C
		public static float EvaluateAugmentationOption(TICouncilorState councilor, CouncilorAugmentationOption option, List<TIMissionTemplate> possibleMissions, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, Dictionary<FactionResource, float> councilorIncomes, bool chasingHydra, int factionWars, bool chasingNeutralNations)
		{
			float num = 0f;
			if (option.stat != CouncilorAttribute.None)
			{
				num += AIEvaluators.EvaluateStatIncreaseUtility(councilor, councilor.faction, option.stat, option.statValue, possibleMissions, requiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations, false);
			}
			if (option.traitToGain != null)
			{
				num += AIEvaluators.EvaluateTrait(councilor, councilor.faction, option.traitToGain, possibleMissions, requiredMissions, missingRequiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations);
			}
			if (option.traitToLose != null)
			{
				num += AIEvaluators.EvaluateTrait(councilor, councilor.faction, option.traitToLose, possibleMissions, requiredMissions, missingRequiredMissions, councilorIncomes, chasingHydra, factionWars, chasingNeutralNations);
			}
			float num2 = (float)option.XPCost / 10f + option.resourceCost.resourceCosts.Sum<ResourceValue>((ResourceValue x) => AIEvaluators.FixedResourceValue(councilor.faction, x.resource, x.value, false));
			return num / num2;
		}

		// Token: 0x0600493C RID: 18748 RVA: 0x001E3B84 File Offset: 0x001E1D84
		public static float EvaluateStatIncreaseUtility(TICouncilorState councilor, TIFactionState faction, CouncilorAttribute attribute, int bonus, List<TIMissionTemplate> possibleMissions, List<TIMissionTemplate> requiredMissions, Dictionary<FactionResource, float> relevantIncomes, bool chasingHydra, int factionWars, bool chasingNeutralNations, bool isOrgEquipped = false)
		{
			int num = councilor.GetAttribute(attribute, true, true, true, false, false, false);
			if (isOrgEquipped)
			{
				num -= bonus;
			}
			int clampedMaxStatValue = councilor.GetClampedMaxStatValue(attribute);
			float num2 = (float)Mathf.Min(bonus, clampedMaxStatValue - num);
			if (num2 <= 0f)
			{
				return 0f;
			}
			foreach (TIMissionTemplate timissionTemplate in possibleMissions)
			{
				if (timissionTemplate.primaryAttackerStat == attribute)
				{
					num2 += AIEvaluators.EvaluateMissionTemplateUtility(councilor, timissionTemplate, requiredMissions, new List<TIMissionTemplate>());
				}
			}
			switch (attribute)
			{
			case CouncilorAttribute.Persuasion:
				if (councilor.isAlien)
				{
					num2 *= 3f;
				}
				else if (chasingNeutralNations && possibleMissions.Contains(TIFactionState.controlNationMission))
				{
					num2 *= 10f * TemplateManager.global.GetAICriticalCouncilorStatChasingAggressivenessDifficultyScaling();
				}
				if (faction.majorCPTrouble)
				{
					num2 *= 2f;
				}
				if (relevantIncomes[FactionResource.Influence] > 0f)
				{
					num2 *= Mathf.Max(2.5f, relevantIncomes[FactionResource.Influence] / 5f);
				}
				break;
			case CouncilorAttribute.Investigation:
				num2 *= (float)(1 + factionWars);
				if (chasingHydra)
				{
					num2 *= 10f + num2;
				}
				break;
			case CouncilorAttribute.Espionage:
				num2 *= (float)(1 + factionWars);
				if (chasingHydra)
				{
					num2 *= 8f + num2;
				}
				break;
			case CouncilorAttribute.Command:
				if (faction.majorCPTrouble)
				{
					num2 *= 2f;
				}
				if (relevantIncomes[FactionResource.Operations] > 0f)
				{
					num2 *= Mathf.Max(2f, relevantIncomes[FactionResource.Operations] / 10f);
				}
				break;
			case CouncilorAttribute.Administration:
			{
				int availableAdministration = councilor.availableAdministration;
				num2 *= 20f * TemplateManager.global.GetAICriticalCouncilorStatChasingAggressivenessDifficultyScaling();
				if (availableAdministration <= 0)
				{
					num2 *= (float)Mathf.Max(1, 2 * TemplateManager.global.maxCouncilorAttribute - num);
				}
				else if (availableAdministration <= 3)
				{
					num2 *= (float)Mathf.Max(1, TemplateManager.global.maxCouncilorAttribute - num);
				}
				if (faction.majorCPTrouble)
				{
					num2 *= 2f;
				}
				break;
			}
			case CouncilorAttribute.Science:
				if (relevantIncomes[FactionResource.Research] > 0f)
				{
					num2 *= Mathf.Max(5f, relevantIncomes[FactionResource.Research] / 5f);
				}
				break;
			case CouncilorAttribute.Security:
				num2 *= 1f + ((factionWars > 0) ? ((float)(TemplateManager.global.maxCouncilorAttribute - num) / 2f) : 0f);
				break;
			case CouncilorAttribute.Loyalty:
				num2 *= 3f;
				break;
			case CouncilorAttribute.ApparentLoyalty:
				return 0f;
			}
			if (councilor.typeTemplate.keyStat[0] == attribute)
			{
				num2 *= 8f;
			}
			if (councilor.typeTemplate.keyStat[1] == attribute)
			{
				num2 *= 4f;
			}
			return num2;
		}

		// Token: 0x0600493D RID: 18749 RVA: 0x001E3E44 File Offset: 0x001E2044
		public static TIRegionState GetBestRegionsForFacilityAbductions(TIFactionState faction, TICouncilorState councilor)
		{
			if (!faction.CanCountAbductions)
			{
				Log.Debug(faction.templateName + " is asking for the best abduction region but they can't count abductions", Array.Empty<object>());
				return null;
			}
			List<TIRegionState> list = (from x in TIFactionState.abductionsMission.GetValidTargets(councilor)
				select x.ref_region into x
				where x != null
				where !x.nation.alienNation
				where x.abductions < TemplateManager.global.minAbductionsinRegionForFacility
				where x.nation.regions.Count > 1
				select x).ToList<TIRegionState>();
			if (list.Count == 0)
			{
				return null;
			}
			TIFactionState proxy = GameStateManager.AlienProxy();
			IEnumerable<TIRegionState> enumerable = list.Where<TIRegionState>((TIRegionState x) => x.ref_nation.TotalOwningFaction == proxy);
			IEnumerable<TIRegionState> enumerable2 = list.Where<TIRegionState>((TIRegionState x) => x.ref_nation.executiveFaction == proxy);
			Func<TIControlPoint, bool> <>9__15;
			IEnumerable<TIRegionState> enumerable3 = list.Where<TIRegionState>(delegate(TIRegionState x)
			{
				IEnumerable<TIControlPoint> controlPoints = x.ref_nation.controlPoints;
				Func<TIControlPoint, bool> func;
				if ((func = <>9__15) == null)
				{
					func = (<>9__15 = (TIControlPoint y) => y.faction == proxy);
				}
				return controlPoints.Any<TIControlPoint>(func);
			});
			if (enumerable.Any<TIRegionState>())
			{
				list = enumerable.ToList<TIRegionState>();
			}
			else if (enumerable2.Any<TIRegionState>())
			{
				list = enumerable2.ToList<TIRegionState>();
			}
			else
			{
				if (!enumerable3.Any<TIRegionState>())
				{
					return null;
				}
				list = enumerable3.ToList<TIRegionState>();
			}
			IEnumerable<TIRegionState> enumerable4 = list.Where<TIRegionState>((TIRegionState x) => x.nation.regions.Count > 3);
			if (enumerable4.Any<TIRegionState>())
			{
				list = enumerable4.ToList<TIRegionState>();
			}
			int highestAbductionCount = list.Max<TIRegionState>((TIRegionState x) => x.abductions);
			list = list.Where<TIRegionState>((TIRegionState x) => x.abductions == highestAbductionCount).ToList<TIRegionState>();
			return (from x in list.ToDictionary<TIRegionState, TIRegionState, float>((TIRegionState x) => x, (TIRegionState x) => x.nation.population)
				orderby x.Value descending
				select x).ToList<KeyValuePair<TIRegionState, float>>().SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> x) => Mathf.Pow(x.Value, 1.5f), -1f, 1E-37f).Key;
		}

		// Token: 0x0600493E RID: 18750 RVA: 0x001E40E8 File Offset: 0x001E22E8
		private static float ModifyProjectScoreForResources(TIFactionState faction, TIShipPartTemplate part)
		{
			if (faction.resourceIncomeDeficiencies.Contains(FactionResource.NobleMetals))
			{
				return 1f - part.weightedBuildMaterials.nobleMetals;
			}
			if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Fissiles))
			{
				return 1f - part.weightedBuildMaterials.fissiles;
			}
			if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Exotics))
			{
				return 1f - part.weightedBuildMaterials.exotics;
			}
			if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Antimatter))
			{
				return 1f - part.weightedBuildMaterials.antimatter;
			}
			return 1f;
		}

		// Token: 0x0600493F RID: 18751 RVA: 0x001E4180 File Offset: 0x001E2380
		public static TITechTemplate SelectTech(TIFactionState faction, List<TITechTemplate> candidates, bool randomize)
		{
			candidates = (from x in candidates
				group x by AIEvaluators.GetTechTier(x, faction)).ToDictionary<IGrouping<int, TITechTemplate>, int, IGrouping<int, TITechTemplate>>((IGrouping<int, TITechTemplate> x) => x.Key, (IGrouping<int, TITechTemplate> x) => x).MaxBy<KeyValuePair<int, IGrouping<int, TITechTemplate>>, int>((KeyValuePair<int, IGrouping<int, TITechTemplate>> x) => x.Key).Value.ToList<TITechTemplate>();
			Dictionary<TITechTemplate, float> dictionary = new Dictionary<TITechTemplate, float>();
			string cheapestForcedTechName = faction.cheapestForcedTechName;
			bool shipBuilding = faction.shipBuilding;
			IEnumerable<TIMissionTemplate> allPossibleMissions = faction.GetAllPossibleMissions();
			foreach (TITechTemplate titechTemplate in candidates)
			{
				dictionary.Add(titechTemplate, AIEvaluators.ScoreTech(faction, titechTemplate, true, cheapestForcedTechName == titechTemplate.dataName, shipBuilding, allPossibleMissions));
			}
			if (randomize)
			{
				return dictionary.SelectRandomWeightedItem<KeyValuePair<TITechTemplate, float>>((KeyValuePair<TITechTemplate, float> j) => j.Value, -1f, 1E-37f).Key;
			}
			return dictionary.MaxBy<KeyValuePair<TITechTemplate, float>, float>((KeyValuePair<TITechTemplate, float> x) => x.Value).Key;
		}

		// Token: 0x06004940 RID: 18752 RVA: 0x001E431C File Offset: 0x001E251C
		public static TIProjectTemplate SelectProject(TIFactionState faction, int slot = -1)
		{
			List<TIProjectTemplate> list = faction.SelectableProjects(slot);
			if ((from x in Enumerable.Range(3, 3)
				where x != slot
				where faction.ProjectAllowedInSlot(x)
				select faction.GetProjectInSlot(x)).Any<TIProjectTemplate>((TIProjectTemplate x) => x != null && x.AI_projectRole == ProjectRole.Objective))
			{
				list = list.Where<TIProjectTemplate>((TIProjectTemplate x) => x.AI_projectRole != ProjectRole.Objective).ToList<TIProjectTemplate>();
			}
			return AIEvaluators.SelectProject(faction, list, true, true);
		}

		// Token: 0x06004941 RID: 18753 RVA: 0x001E43E8 File Offset: 0x001E25E8
		public static TIProjectTemplate SelectProject(TIFactionState faction, List<TIProjectTemplate> candidates, bool considerDuration, bool randomize)
		{
			candidates = (from x in candidates
				group x by AIEvaluators.GetTechTier(x, faction)).ToDictionary<IGrouping<int, TIProjectTemplate>, int, IGrouping<int, TIProjectTemplate>>((IGrouping<int, TIProjectTemplate> x) => x.Key, (IGrouping<int, TIProjectTemplate> x) => x).MaxBy<KeyValuePair<int, IGrouping<int, TIProjectTemplate>>, int>((KeyValuePair<int, IGrouping<int, TIProjectTemplate>> x) => x.Key).Value.ToList<TIProjectTemplate>();
			Dictionary<TIProjectTemplate, float> dictionary = new Dictionary<TIProjectTemplate, float>();
			string cheapestForcedTechName = faction.cheapestForcedTechName;
			bool shipBuilding = faction.shipBuilding;
			IEnumerable<TIMissionTemplate> allPossibleMissions = faction.GetAllPossibleMissions();
			foreach (TIProjectTemplate tiprojectTemplate in candidates)
			{
				dictionary.Add(tiprojectTemplate, AIEvaluators.ScoreTech(faction, tiprojectTemplate, considerDuration, cheapestForcedTechName == tiprojectTemplate.dataName, shipBuilding, allPossibleMissions));
			}
			TIProjectTemplate tiprojectTemplate2;
			if (randomize)
			{
				tiprojectTemplate2 = dictionary.SelectRandomWeightedItem<KeyValuePair<TIProjectTemplate, float>>((KeyValuePair<TIProjectTemplate, float> j) => j.Value, -1f, 1E-37f).Key;
			}
			else
			{
				tiprojectTemplate2 = dictionary.MaxBy<KeyValuePair<TIProjectTemplate, float>, float>((KeyValuePair<TIProjectTemplate, float> x) => x.Value).Key;
			}
			return tiprojectTemplate2;
		}

		// Token: 0x06004942 RID: 18754 RVA: 0x001E458C File Offset: 0x001E278C
		public static int SelectTechRaceSlot(TIFactionState faction)
		{
			Dictionary<int, TechProgress> dictionary = Enumerable.Range(0, 3).ToDictionary<int, int, TechProgress>((int x) => x, (int x) => GameStateManager.GlobalResearch().GetTechProgress(x));
			dictionary = dictionary.Where<KeyValuePair<int, TechProgress>>((KeyValuePair<int, TechProgress> x) => x.Value.techTemplate.AI_techRole == TechRole.Competition || (!x.Value.CantWin(faction) && !x.Value.CantLose(faction))).ToDictionary<KeyValuePair<int, TechProgress>, int, TechProgress>((KeyValuePair<int, TechProgress> x) => x.Key, (KeyValuePair<int, TechProgress> x) => x.Value);
			if (!dictionary.Any<KeyValuePair<int, TechProgress>>())
			{
				return -1;
			}
			Dictionary<int, float> dictionary2 = dictionary.ToDictionary<KeyValuePair<int, TechProgress>, int, float>((KeyValuePair<int, TechProgress> x) => x.Key, delegate(KeyValuePair<int, TechProgress> pair)
			{
				TechProgress progress = pair.Value;
				bool isCompetition = progress.techTemplate.AI_techRole == TechRole.Competition;
				float researchPerDay = faction.GetEffectiveResearchPerDay(progress.TechCategory, false, true);
				return (from x in GameStateManager.AllHumanFactions()
					where x != faction && (isCompetition || !progress.CantWin(x))
					select x).ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, delegate(TIFactionState otherFaction)
				{
					float effectiveResearchPerDay = otherFaction.GetEffectiveResearchPerDay(progress.TechCategory, false, true);
					float num = 0.8f * progress.remainingResearch / (effectiveResearchPerDay + researchPerDay);
					float num2 = progress.factionContributions[faction] + num * researchPerDay;
					float num3 = progress.factionContributions[otherFaction] + num * effectiveResearchPerDay;
					float num4 = 1f;
					if (num > 120f)
					{
						num4 = 1f / Mathf.Pow(num, 0.4f);
					}
					float num5 = 1f;
					float num6 = 1f;
					if (num > 300f)
					{
						num5 = 1E-06f;
					}
					else if (isCompetition)
					{
						num4 = 1f;
						if (num < 120f)
						{
							num6 = 1000000f;
						}
						else if (num < 200f)
						{
							num6 = 1.15f;
						}
					}
					return num4 * num5 * num6 * num2 / num3;
				}).Min<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value);
			});
			float bestWorstScore = dictionary2.MaxBy<KeyValuePair<int, float>, float>((KeyValuePair<int, float> x) => x.Value).Value;
			return dictionary2.Where<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Value > bestWorstScore * 0.9f).SelectRandomWeightedItem<KeyValuePair<int, float>>((KeyValuePair<int, float> x) => x.Value, -1f, 1E-37f).Key;
		}

		// Token: 0x06004943 RID: 18755 RVA: 0x001E4700 File Offset: 0x001E2900
		public static int SelectPassiveTechSlot(TIFactionState faction)
		{
			List<TechProgress> list = (from x in Enumerable.Range(0, 3)
				select GameStateManager.GlobalResearch().GetTechProgress(x)).ToList<TechProgress>();
			List<TechProgress> list2 = list.Where<TechProgress>(delegate(TechProgress x)
			{
				TITechTemplate techTemplate2 = x.techTemplate;
				return techTemplate2.AI_techRole == TechRole.Blocker || techTemplate2.AI_techRole == TechRole.Competition;
			}).ToList<TechProgress>();
			TechProgress techProgress = list2.Where<TechProgress>((TechProgress x) => faction.forcedTechNames.Contains(x.techTemplateName)).FirstOrDefault<TechProgress>();
			if (techProgress != null)
			{
				float num = 0.3f * Mathf.Pow(faction.aiValues.gatherScience, 3f);
				if (techProgress.techTemplate.AI_techRole == TechRole.Competition)
				{
					num *= 1.5f;
				}
				if (list2.Count == 3 || TIUtilities.RandomFloatValue() < num)
				{
					return techProgress.slot;
				}
			}
			List<TechProgress> list3 = list.Where<TechProgress>((TechProgress x) => x.CantLose(faction)).ToList<TechProgress>();
			if (!faction.cynical && list3.Count > 0)
			{
				list = list3;
			}
			else
			{
				List<TechProgress> list4 = list.Where<TechProgress>((TechProgress x) => x.remainingResearch / faction.GetEffectiveResearchPerDay(x.TechCategory, false, true) < 90f).ToList<TechProgress>();
				if (faction.believers && !faction.IsAlienProxy && !faction.isAlienAppeaser && list4.Count > 0)
				{
					list = list4;
				}
				else
				{
					List<TechProgress> list5 = list.Where<TechProgress>((TechProgress x) => x.selector == faction).ToList<TechProgress>();
					if (list5.Count > 0)
					{
						list = list5;
					}
				}
			}
			string nextFactionTech = faction.cheapestForcedTechName;
			bool shipBuilding = faction.shipBuilding;
			IEnumerable<TIMissionTemplate> availableMissions = faction.GetAllPossibleMissions();
			TITechTemplate techTemplate = list.ToDictionary<TechProgress, TechProgress, float>((TechProgress x) => x, delegate(TechProgress x)
			{
				float num2 = Mathf.Pow(AIEvaluators.ScoreTech(faction, x.techTemplate, true, x.techTemplateName == nextFactionTech, shipBuilding, availableMissions), 0.5f);
				float num3 = x.factionContributions[faction] / x.techTemplate.researchCost;
				return num2 * (0.1f + num3);
			}).SelectRandomWeightedItem<KeyValuePair<TechProgress, float>>((KeyValuePair<TechProgress, float> x) => x.Value, -1f, 1E-37f).Key.techTemplate;
			return GameStateManager.GlobalResearch().GetSlotForTech(techTemplate);
		}

		// Token: 0x06004944 RID: 18756 RVA: 0x001E493C File Offset: 0x001E2B3C
		public static int GetHighestActiveProjectTier(TIFactionState faction)
		{
			return faction.CurrentlyActiveProjects().Max<TIProjectTemplate>((TIProjectTemplate x) => AIEvaluators.GetTechTier(x, faction));
		}

		// Token: 0x06004945 RID: 18757 RVA: 0x001E4972 File Offset: 0x001E2B72
		public static bool ShouldFocusOnGlobalResearch(this TIFactionState faction)
		{
			return AIEvaluators.GetHighestActiveProjectTier(faction) < 1;
		}

		// Token: 0x06004946 RID: 18758 RVA: 0x001E4980 File Offset: 0x001E2B80
		public static int GetTechTier(TIGenericTechTemplate tech, TIFactionState faction)
		{
			float num = -1f;
			if (AIEvaluators.techTiersCacheDate != null)
			{
				num = (float)(TITimeState.Now() - AIEvaluators.techTiersCacheDate).TotalDays;
			}
			if (num < 0f || num > 30f)
			{
				AIEvaluators.cachedTechTiers.Clear();
				AIEvaluators.techTiersCacheDate = TITimeState.Now();
			}
			Dictionary<TIGenericTechTemplate, int> dictionary;
			if (!AIEvaluators.cachedTechTiers.TryGetValue(faction, out dictionary))
			{
				dictionary = (AIEvaluators.cachedTechTiers[faction] = new Dictionary<TIGenericTechTemplate, int>());
			}
			int num2;
			if (!dictionary.TryGetValue(tech, out num2))
			{
				num2 = (dictionary[tech] = AIEvaluators.RecalculateTechTier(tech, faction));
			}
			return num2;
		}

		// Token: 0x06004947 RID: 18759 RVA: 0x001E4A1C File Offset: 0x001E2C1C
		private static int RecalculateTechTier(TIGenericTechTemplate tech, TIFactionState faction)
		{
			if (tech == null)
			{
				return -99;
			}
			if (tech.AI_criticalTech && faction.forcedTechNames.Contains(tech.dataName))
			{
				return 7;
			}
			TIProjectTemplate tiprojectTemplate = tech as TIProjectTemplate;
			if (tiprojectTemplate != null)
			{
				if (faction.forcedTechNames.Contains(tech.dataName))
				{
					return 6;
				}
				if (tiprojectTemplate.AI_projectRole == ProjectRole.Objective)
				{
					return 5;
				}
				if (!faction.HasObjectiveProjectAvailable() && tiprojectTemplate.LeadsToObjectiveProjects(faction))
				{
					return 4;
				}
				if (tiprojectTemplate.AI_projectRole == ProjectRole.Core)
				{
					return 3;
				}
				if (tiprojectTemplate.repeatable)
				{
					float num = tiprojectTemplate.ScoreProjectResourceRewardsRelativeToResearchCost(faction);
					if (num < 1f)
					{
						return -1;
					}
					if (num < 2f)
					{
						return 2;
					}
					return 3;
				}
				else
				{
					if (tech.AI_criticalTech)
					{
						return 2;
					}
					if (tiprojectTemplate.AI_projectRole == ProjectRole.ExpandNation && AIEvaluators.ScoreExpandNationProject(faction, tiprojectTemplate) >= 10f)
					{
						return 2;
					}
					if (!faction.IsAlienProxy && tiprojectTemplate.AI_projectRole == ProjectRole.AlienMissionDefense)
					{
						return 2;
					}
					if (AIEvaluators.ShouldSkipProject(tiprojectTemplate, faction))
					{
						return -2;
					}
					if (tiprojectTemplate.ShipPartUnlocks.Count > 0 || tiprojectTemplate.HabModuleUnlocks().Count > 0 || (tiprojectTemplate.AI_projectRole == ProjectRole.ControlPointCap && faction.AvailableCPCapSpace() <= faction.GetControlPointMaintenanceFreebieCap() * 0.15f) || (tiprojectTemplate.AI_projectRole == ProjectRole.ExpandNation && AIEvaluators.ScoreExpandNationProject(faction, tiprojectTemplate) >= 0f) || (tiprojectTemplate.AI_projectRole == ProjectRole.MissionControl && faction.AI_GenericMissionControlAvailable <= 0) || tiprojectTemplate.AllPrereqFor(faction, true).Any<TIGenericTechTemplate>((TIGenericTechTemplate x) => x.AI_criticalTech || x.ref_project.AI_projectRole == ProjectRole.Core || x.ref_project.AI_projectRole == ProjectRole.Objective || faction.forcedTechNames.Contains(x.dataName)))
					{
						return 1;
					}
				}
			}
			else
			{
				if (tech.AI_criticalTech || faction.forcedTechNames.Contains(tech.dataName))
				{
					return 5;
				}
				if (tech.AllPrereqFor(faction, true).Any<TIGenericTechTemplate>((TIGenericTechTemplate x) => x.AI_criticalTech || faction.forcedTechNames.Contains(x.dataName)))
				{
					return 2;
				}
				if (tech.AI_techRole == TechRole.SpaceExpansion && faction.MineNetworkSize >= faction.SafeMineNextworkSize)
				{
					return 1;
				}
			}
			return 0;
		}

		// Token: 0x06004948 RID: 18760 RVA: 0x001E4C3C File Offset: 0x001E2E3C
		public static float ScoreProjectResourceRewardsRelativeToResearchCost(this TIProjectTemplate project, TIFactionState faction)
		{
			Dictionary<FactionResource, float> dictionary = (from x in project.resourcesGranted
				where x.value > 0f
				where x.resource != FactionResource.Research
				select x).ToDictionary<ResourceValue, FactionResource, float>((ResourceValue x) => x.resource, delegate(ResourceValue x)
			{
				float num5 = Mathf.Max(0f, faction.GetCurrentResourceAmount(x.resource));
				float num6;
				switch (x.resource)
				{
				case FactionResource.Money:
					num6 = 2000f;
					goto IL_00AF;
				case FactionResource.Influence:
					num6 = 80f;
					goto IL_00AF;
				case FactionResource.Operations:
					num6 = 40f;
					goto IL_00AF;
				case FactionResource.Projects:
					num6 = 3f;
					goto IL_00AF;
				case FactionResource.Boost:
					num6 = 20f;
					goto IL_00AF;
				case FactionResource.MissionControl:
					num6 = 5f;
					goto IL_00AF;
				case FactionResource.Water:
				case FactionResource.Volatiles:
				case FactionResource.Metals:
				case FactionResource.NobleMetals:
					num6 = 200f;
					goto IL_00AF;
				case FactionResource.Fissiles:
					num6 = 50f;
					goto IL_00AF;
				case FactionResource.Antimatter:
					num6 = 1f;
					goto IL_00AF;
				}
				num6 = 1000f;
				IL_00AF:
				num5 = num6 + num5;
				return x.value / num5;
			});
			float num = project.Effects.Where<TIEffectTemplate>((TIEffectTemplate x) => x.GetContexts().Any<Context>((Context x) => x == Context.ControlPointMaintenance)).Sum<TIEffectTemplate>((TIEffectTemplate x) => x.value);
			float num2 = Mathf.Max(0f, -num) / 15f;
			float num3 = dictionary.Values.Sum() + num2;
			float dailyIncome = faction.GetDailyIncome(FactionResource.Research, false, false);
			float num4 = (project.GetResearchCost(faction) - faction.GetProjectProgressByTemplate(project).accumulatedResearch) / dailyIncome;
			return Mathf.Clamp(num3 / 0.3f / Mathf.Max(0.1f, num4 / 30f), 0.01f, 1000f);
		}

		// Token: 0x06004949 RID: 18761 RVA: 0x001E4D9B File Offset: 0x001E2F9B
		public static bool AreProjectResourceRewardsWorthResearchCost(this TIProjectTemplate project, TIFactionState faction)
		{
			return project.ScoreProjectResourceRewardsRelativeToResearchCost(faction) >= 1f;
		}

		// Token: 0x0600494A RID: 18762 RVA: 0x001E4DB0 File Offset: 0x001E2FB0
		public static bool ShouldSkipProject(TIProjectTemplate project, TIFactionState faction)
		{
			if (project.Effects.Any<TIEffectTemplate>((TIEffectTemplate x) => x.contexts.Contains(Context.BuildNuclearWeaponsPriority) && x.effectTarget == EffectTargetType.AllHumanFactions && x.value > 0f) && (!faction.extremist || !faction.antiAlien))
			{
				return true;
			}
			if (project.AI_criticalTech || project.AI_projectRole == ProjectRole.Objective || project.AI_projectRole == ProjectRole.Core || faction.forcedTechNames.Contains(project.dataName))
			{
				return false;
			}
			if (project.repeatable)
			{
				return !project.AreProjectResourceRewardsWorthResearchCost(faction);
			}
			if (project.oneTimeGlobally)
			{
				if (project.AI_projectRole == ProjectRole.ExpandNation && project.associatedClaims.Count > 0)
				{
					if (AIEvaluators.ScoreExpandNationProject(faction, project) > 0f)
					{
						return false;
					}
				}
				else if (project.AI_projectRole == ProjectRole.NeutralizeNation && project.associatedClaims.Count > 0 && AIEvaluators.ScoreNeutralizeProject(faction, project) > 0)
				{
					return false;
				}
				return true;
			}
			HashSet<TIProjectTemplate> hashSet;
			if (!AIEvaluators.obsoleteProjects.TryGetValue(faction, out hashSet))
			{
				hashSet = (AIEvaluators.obsoleteProjects[faction] = new HashSet<TIProjectTemplate>());
			}
			if (hashSet.Contains(project))
			{
				return true;
			}
			List<TIShipPartTemplate> shipPartUnlocks = project.ShipPartUnlocks;
			if (shipPartUnlocks.Count == 0)
			{
				return false;
			}
			float maxFissilesPerTank_dt = faction.GetDailyIncome(FactionResource.Fissiles, true, false) / 4f;
			if (shipPartUnlocks.Any<TIShipPartTemplate>((TIShipPartTemplate x) => x.isDrive && x.ref_drive.GetPerTankPropellantMaterials(faction).fissiles > maxFissilesPerTank_dt))
			{
				return true;
			}
			if (faction.GetDailyIncome(FactionResource.Antimatter, true, false) == 0f && shipPartUnlocks.Any<TIShipPartTemplate>((TIShipPartTemplate x) => (x.isDrive && x.ref_drive.GetPerTankPropellantMaterials(faction).antimatter > 0f) || (x.isProjectileWeapon && x.ref_projectileWeapon.ammoMaterials.antimatter > 0f) || x.weightedBuildMaterials.antimatter > 0f))
			{
				return true;
			}
			float exoticsAvailable = faction.GetCurrentResourceAmount(FactionResource.Exotics);
			if (shipPartUnlocks.Any<TIShipPartTemplate>(delegate(TIShipPartTemplate x)
			{
				float singleCostValue = x.buildCost(0f, 0f).GetSingleCostValue(FactionResource.Exotics);
				return singleCostValue != 0f && exoticsAvailable / singleCostValue < 20f;
			}))
			{
				return true;
			}
			Func<TIShipPartTemplate, List<float>> func = (TIShipPartTemplate part) => new List<float> { part.AIScoringValueForResearch() };
			List<TIShipPartTemplate> list = faction.allowedShipParts.Union<TIShipPartTemplate>(faction.allowedArmors).ToList<TIShipPartTemplate>();
			List<TIShipPartTemplate> list2 = list.Union<TIShipPartTemplate>(faction.availableProjects.SelectMany<TIProjectTemplate, TIShipPartTemplate>((TIProjectTemplate x) => x.ShipPartUnlocks)).Except<TIShipPartTemplate>(shipPartUnlocks).ToList<TIShipPartTemplate>();
			foreach (TIShipPartTemplate tishipPartTemplate in shipPartUnlocks)
			{
				List<float> list3 = null;
				List<bool> list4 = null;
				Func<TIShipPartTemplate, TIShipPartTemplate, bool> func2;
				if (!(tishipPartTemplate is TIPowerPlantTemplate))
				{
					TIDriveTemplate tidriveTemplate = tishipPartTemplate as TIDriveTemplate;
					if (tidriveTemplate == null)
					{
						if (!(tishipPartTemplate is TIShipWeaponTemplate))
						{
							if (!(tishipPartTemplate is TIShipArmorTemplate))
							{
								if (!(tishipPartTemplate is TIBatteryTemplate))
								{
									if (!(tishipPartTemplate is TIHeatSinkTemplate))
									{
										if (!(tishipPartTemplate is TIRadiatorTemplate))
										{
											return false;
										}
										func2 = (TIShipPartTemplate partA, TIShipPartTemplate partB) => partA.isRadiator && partB.isRadiator;
									}
									else
									{
										func2 = (TIShipPartTemplate partA, TIShipPartTemplate partB) => partA.isHeatSink && partB.isHeatSink;
									}
								}
								else
								{
									func2 = (TIShipPartTemplate partA, TIShipPartTemplate partB) => partA.isBattery && partB.isBattery;
								}
							}
							else
							{
								func2 = (TIShipPartTemplate partA, TIShipPartTemplate partB) => partA.isArmor && partB.isArmor;
							}
						}
						else
						{
							func2 = (TIShipPartTemplate partA, TIShipPartTemplate partB) => partA.isWeapon && partB.isWeapon && partA.ref_weapon.noseWeapon == partB.ref_weapon.noseWeapon && (partA.ref_weapon.defenseMode && !partA.ref_weapon.attackMode) == (partB.ref_weapon.defenseMode && !partB.ref_weapon.attackMode) && (partA.ref_weapon.weaponClass == partB.ref_weapon.weaponClass || (partA.isGunTypeWeapon && partB.isGunTypeWeapon));
							IEnumerable<float> ranges_km = Enumerable.Empty<float>().Append(200f).Append(500f)
								.Append(800f);
							Func<TIShipPartTemplate, float, float> ScorePartGivenRange = (TIShipPartTemplate part, float range_km) => part.ref_weapon.EstimateDPS(range_km, null, true);
							func = (TIShipPartTemplate part) => ranges_km.Select<float, float>((float range_km) => ScorePartGivenRange(part, range_km)).ToList<float>();
						}
					}
					else
					{
						if (tidriveTemplate.thrusters != 1)
						{
							continue;
						}
						func2 = delegate(TIShipPartTemplate partA, TIShipPartTemplate partB)
						{
							TIDriveTemplate tidriveTemplate2 = partA as TIDriveTemplate;
							if (tidriveTemplate2 != null)
							{
								TIDriveTemplate tidriveTemplate3 = partB as TIDriveTemplate;
								if (tidriveTemplate3 != null)
								{
									return tidriveTemplate2.driveClassification == tidriveTemplate3.driveClassification;
								}
							}
							return false;
						};
						func = (TIShipPartTemplate part) => new List<float>
						{
							part.ref_drive.thrust_N,
							part.ref_drive.EV_kps,
							part.ref_drive.efficiency,
							part.ref_drive.thrust_N * part.ref_drive.EV_kps * part.ref_drive.efficiency
						};
					}
				}
				else
				{
					func2 = (TIShipPartTemplate partA, TIShipPartTemplate partB) => partA.isPowerPlant && partB.isPowerPlant;
					IEnumerable<TIDriveTemplate> driveSelection = faction.allowedDrives;
					func = (TIShipPartTemplate part) => driveSelection.Select<TIDriveTemplate, float>(delegate(TIDriveTemplate drive)
					{
						if (!drive.IsCompatible(part.ref_powerPlant))
						{
							return 0f;
						}
						return part.AIScoringValueForResearch();
					}).ToList<float>();
				}
				using (List<TIShipPartTemplate>.Enumerator enumerator2 = list2.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIShipPartTemplate comparisonPart = enumerator2.Current;
						if (func2(tishipPartTemplate, comparisonPart))
						{
							if (list3 == null)
							{
								list3 = func(tishipPartTemplate);
								list4 = list3.Select<float, bool>((float x) => true).ToList<bool>();
							}
							List<float> list5 = func(comparisonPart);
							int i = 0;
							Func<TIProjectTemplate, bool> <>9__22;
							while (i < list3.Count)
							{
								float num = 1f;
								if (list.Contains(comparisonPart))
								{
									goto IL_0557;
								}
								IEnumerable<TIProjectTemplate> enumerable = faction.CurrentlyActiveProjects();
								Func<TIProjectTemplate, bool> func3;
								if ((func3 = <>9__22) == null)
								{
									func3 = (<>9__22 = (TIProjectTemplate x) => x != null && x.ShipPartUnlocks.Contains(comparisonPart));
								}
								if (enumerable.Any<TIProjectTemplate>(func3))
								{
									goto IL_0557;
								}
								IL_055E:
								if (list3[i] <= list5[i] * num)
								{
									list4[i] = false;
								}
								i++;
								continue;
								IL_0557:
								num = 1.3f;
								goto IL_055E;
							}
							if (list4.None<bool>((bool x) => x))
							{
								break;
							}
						}
					}
				}
				if (list4 != null)
				{
					if (!list4.Any<bool>((bool x) => x))
					{
						continue;
					}
				}
				return false;
			}
			AIEvaluators.obsoleteProjects[faction].Add(project);
			return true;
		}

		// Token: 0x0600494B RID: 18763 RVA: 0x001E5434 File Offset: 0x001E3634
		public static bool ShouldFocusOnObjectiveProject(this TIFactionState faction, out bool hyperFocus)
		{
			hyperFocus = false;
			TIProjectTemplate tiprojectTemplate = faction.CurrentlyActiveProjects().FirstOrDefault<TIProjectTemplate>((TIProjectTemplate x) => x.AI_projectRole == ProjectRole.Objective);
			if (tiprojectTemplate == null)
			{
				return false;
			}
			if (tiprojectTemplate.IsVictoryRelated(faction))
			{
				hyperFocus = true;
				return true;
			}
			if (faction.IsAlienProxy && TIGlobalValuesState.IsQuietAlienCampaign() && !TIEffectsState.CheckForAnyEffectInContext(Context.CanTransferTerritoryToAliens, faction))
			{
				return true;
			}
			float accumulatedResearch = faction.GetProjectProgressByTemplate(tiprojectTemplate).accumulatedResearch;
			float num = (float)(TITimeState.Now() - faction.LastObjectiveProjectCompletionDate).TotalDays;
			float num2 = 0.2f * faction.GetEffectiveResearchPerDay(tiprojectTemplate.techCategory, true, true) * num;
			return accumulatedResearch < num2;
		}

		// Token: 0x0600494C RID: 18764 RVA: 0x001E54DC File Offset: 0x001E36DC
		public static float ScoreExpandNationProject(TIFactionState faction, TIProjectTemplate project)
		{
			IEnumerable<TINationState> enumerable = project.associatedClaims.Select<TIBilateralTemplate, TINationState>((TIBilateralTemplate x) => x.nationState1);
			if (enumerable.Count<TINationState>() == 0)
			{
				return 0f;
			}
			IEnumerable<TINationState> claimsOfInterest = enumerable.Intersect<TINationState>(faction.majorityControlNations);
			if (claimsOfInterest.Count<TINationState>() > 0)
			{
				return (float)(from x in faction.GoalsOfType(TIFactionGoalState.UnificationAllowedManagementGoals, false, true)
					where claimsOfInterest.Contains(x.target().ref_nation)
					select x).Sum<TIFactionGoalState>((TIFactionGoalState x) => x.importance * ((x.GetGoalType() == GoalType.ExpandNation) ? 3 : 1));
			}
			return -1f;
		}

		// Token: 0x0600494D RID: 18765 RVA: 0x001E5594 File Offset: 0x001E3794
		public static int ScoreNeutralizeProject(TIFactionState faction, TIProjectTemplate project)
		{
			IEnumerable<TIBilateralTemplate> enumerable = project.associatedClaims.Where<TIBilateralTemplate>((TIBilateralTemplate x) => !x.nationState1.regions.Contains(x.regionState1));
			if (enumerable.Any<TIBilateralTemplate>())
			{
				List<TIFactionGoalState> list = faction.GoalsOfType(GoalType.NeutralizeNation, false, true);
				IEnumerable<TIGameState> neutralizeGoalTargets = list.Select<TIFactionGoalState, TIGameState>((TIFactionGoalState x) => x.target());
				IEnumerable<TIBilateralTemplate> relevantNeutralizeBilaterals = enumerable.Where<TIBilateralTemplate>((TIBilateralTemplate x) => neutralizeGoalTargets.Contains(x.nationState1));
				if (relevantNeutralizeBilaterals.Count<TIBilateralTemplate>() > 0)
				{
					IEnumerable<TIFactionGoalState> enumerable2 = list.Where<TIFactionGoalState>((TIFactionGoalState x) => relevantNeutralizeBilaterals.Select<TIBilateralTemplate, TINationState>((TIBilateralTemplate y) => y.nationState1).Intersect<TIGameState>(neutralizeGoalTargets).Any<TIGameState>());
					if (enumerable2.Count<TIFactionGoalState>() > 0)
					{
						return enumerable2.Sum<TIFactionGoalState>((TIFactionGoalState x) => x.importance);
					}
				}
				return 0;
			}
			return -1;
		}

		// Token: 0x0600494E RID: 18766 RVA: 0x001E5680 File Offset: 0x001E3880
		public static float ScoreTech(TIFactionState faction, TIGenericTechTemplate tech, bool considerDuration, bool forcedFactionTech, bool shipBuilding, IEnumerable<TIMissionTemplate> availableMissions)
		{
			float num = 1f;
			num *= faction.TechCategoryValuation(tech.techCategory);
			num *= faction.TechRoleValuation(tech.AI_techRole);
			float researchCost = tech.GetResearchCost(faction);
			if (considerDuration)
			{
				num /= researchCost;
			}
			else
			{
				num *= Mathf.Max(1f, researchCost / 1000f);
			}
			if (tech.isProject())
			{
				TIProjectTemplate ref_project = tech.ref_project;
				float projectProgressValueByTemplateFraction = faction.GetProjectProgressValueByTemplateFraction(tech.ref_project);
				float num2 = ref_project.ScoreProjectResourceRewardsRelativeToResearchCost(faction);
				num *= num2 + 1f;
				if (projectProgressValueByTemplateFraction > 0f && !ref_project.repeatable)
				{
					num *= Mathf.Max(750f * projectProgressValueByTemplateFraction, 2f);
				}
				if (ref_project.AI_techRole == TechRole.SpaceWar && !shipBuilding)
				{
					num *= 0.05f;
				}
				else
				{
					if (ref_project.AI_criticalTech)
					{
						num *= 1000f;
					}
					if (forcedFactionTech)
					{
						num *= 1000f;
					}
					if (ref_project.repeatable && num2 < 1f)
					{
						num *= 0.05f;
					}
				}
				switch (ref_project.AI_projectRole)
				{
				case ProjectRole.Core:
					num *= 10000000f;
					goto IL_1666;
				case ProjectRole.Objective:
					if (faction.CurrentlyActiveProjects().None<TIProjectTemplate>((TIProjectTemplate x) => x != null && x.AI_techRole == TechRole.FactionObjective))
					{
						num *= 50f;
						goto IL_1666;
					}
					num *= 0.001f;
					goto IL_1666;
				case ProjectRole.Money:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
						if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Money))
						{
							num *= 10f;
						}
					}
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Money))
					{
						num *= 1f + AIEvaluators.GetAIRelativeValuation(FactionResource.Money);
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.Influence:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
					}
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Influence) || faction.selfAssessement < FactionSelfAssessment.Even)
					{
						num *= AIEvaluators.GetAIRelativeValuation(FactionResource.Influence);
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.Ops:
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Operations))
					{
						num *= AIEvaluators.GetAIRelativeValuation(FactionResource.Operations);
					}
					if (faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTemplateName == TIFactionState.assaultAlienAssetMission.dataName || x.targetMissionTemplateName == TIFactionState.detainMission.dataName || x.targetMissionTemplateName == TIFactionState.assassinateMission.dataName))
					{
						num *= 3f;
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.Research:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 25000f;
					}
					if (ref_project.ShipPartUnlocks.Count > 0)
					{
						num *= 30f;
					}
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Research) || faction.selfAssessement < FactionSelfAssessment.Even)
					{
						num *= AIEvaluators.GetAIRelativeValuation(FactionResource.Research);
					}
					num *= 3f;
					goto IL_1666;
				case ProjectRole.Boost:
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
					{
						int num3 = faction.bases.Count<TIHabState>((TIHabState x) => x.HasActiveMine);
						num *= (float)(8 / (num3 + 1));
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.MissionControl:
					num *= Mathf.Min(5f, AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, 1f));
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
					}
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.MissionControl))
					{
						num *= AIEvaluators.GetAIRelativeValuation(FactionResource.MissionControl);
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.SpaceResources:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
					}
					if (faction.selfAssessement < FactionSelfAssessment.Even)
					{
						num *= 3f;
					}
					num *= 3f;
					goto IL_1666;
				case ProjectRole.Antimatter:
					if (faction.shipDesigns.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.requiresAntimatter) && (faction.GetMonthlyIncome(FactionResource.Antimatter, false, false) <= 0f || faction.resourceIncomeDeficiencies.Contains(FactionResource.Antimatter)))
					{
						num *= 10f;
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.Councilors:
					num *= 2f;
					goto IL_1666;
				case ProjectRole.PoliticalMissions:
					num *= 2f;
					goto IL_1666;
				case ProjectRole.CombatMissions:
					if (faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.targetMissionTemplateName == TIFactionState.assaultAlienAssetMission.dataName))
					{
						num *= 3f;
						goto IL_1666;
					}
					goto IL_1666;
				case ProjectRole.ExpandNation:
					break;
				case ProjectRole.NeutralizeNation:
				{
					int num4 = AIEvaluators.ScoreNeutralizeProject(faction, ref_project);
					if (num4 > 0)
					{
						num *= (float)num4;
						goto IL_1666;
					}
					if (num <= 0f)
					{
						num *= 3f;
						goto IL_1666;
					}
					num = 1E-37f;
					goto IL_1666;
				}
				case ProjectRole.DevelopNation:
				{
					int count = faction.GoalsOfType(GoalType.DevelopNation, false, true).Count;
					num *= Mathf.Clamp((float)count / 5f, 0f, 3f);
					goto IL_1666;
				}
				case ProjectRole.Exploration:
				{
					int num5 = faction.resourceIncomeDeficiencies.Count<FactionResource>((FactionResource x) => TIResourcesCost.basicSpaceResources.Contains(x));
					num *= (float)(1 + num5);
					goto IL_1666;
				}
				case ProjectRole.HabInfrastructure:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
						goto IL_1666;
					}
					num *= 3f;
					goto IL_1666;
				case ProjectRole.HabDefense:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
						goto IL_1666;
					}
					num *= 3f;
					goto IL_1666;
				case ProjectRole.Fleet:
				{
					if (!shipBuilding)
					{
						num *= 0.1f;
						goto IL_1666;
					}
					int num6 = faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Count + faction.GoalsOfType(GoalType.AttackWithFleet, false, true).Count;
					num *= Mathf.Clamp((float)num6 / 10f, 1f, 3f);
					goto IL_1666;
				}
				case ProjectRole.Propulsion:
				{
					List<TIShipPartTemplate> shipPartUnlocks = ref_project.ShipPartUnlocks;
					if (shipPartUnlocks.Count > 0)
					{
						List<TIDriveTemplate> list = faction.allowedDrives.Where<TIDriveTemplate>((TIDriveTemplate x) => x.thrusters == 1).ToList<TIDriveTemplate>();
						float num7 = 1f;
						float num8 = 1f;
						float num9 = 0.01f;
						float num10 = 1E-05f;
						if (list.Count > 0)
						{
							num7 = list.Max<TIDriveTemplate>((TIDriveTemplate x) => x.thrustRating);
							num8 = list.Max<TIDriveTemplate>((TIDriveTemplate x) => x.EVRating);
							num9 = list.Max<TIDriveTemplate>((TIDriveTemplate x) => x.efficiency);
							num10 = list.Max<TIDriveTemplate>((TIDriveTemplate x) => x.thrustPower_GW);
						}
						foreach (TIShipPartTemplate tishipPartTemplate in shipPartUnlocks)
						{
							TIDriveTemplate tidriveTemplate = tishipPartTemplate as TIDriveTemplate;
							if (tidriveTemplate != null)
							{
								if (tidriveTemplate.thrusters == 1 && list.Count > 0)
								{
									float num11 = tidriveTemplate.thrustRating / num7;
									float num12 = tidriveTemplate.EVRating / num8;
									float num13 = tidriveTemplate.efficiency / num9;
									float num14;
									if (num10 > 0f)
									{
										num14 = tidriveTemplate.thrustPower_GW / num10;
									}
									else
									{
										num14 = 1f;
									}
									float num15 = Mathf.Max(num11, 1f) * Mathf.Max(num12, 1f) * Mathf.Max(num13, 1f) * Mathf.Max(num14, 1f);
									if (num15 > 1f)
									{
										num *= 30f * num15;
									}
									if (tidriveTemplate.GetPerTankPropellantMaterials(faction).antimatter > 0f && faction.resourceIncomeDeficiencies.Contains(FactionResource.Antimatter))
									{
										num *= 1E-06f;
									}
									if (tidriveTemplate.GetPerTankPropellantMaterials(faction).fissiles > 0f && faction.resourceIncomeDeficiencies.Contains(FactionResource.Fissiles))
									{
										num *= 0.05f;
									}
									if (tidriveTemplate.thrust_N < 100000f || tidriveTemplate.EV_kps < 8f)
									{
										num *= 0.25f;
									}
								}
							}
							else
							{
								TIUtilityModuleTemplate tiutilityModuleTemplate = tishipPartTemplate as TIUtilityModuleTemplate;
								if (tiutilityModuleTemplate != null)
								{
									num *= tiutilityModuleTemplate.EVMultiplier * tiutilityModuleTemplate.thrustMultiplier * 30f;
									num *= AIEvaluators.ModifyProjectScoreForResources(faction, tiutilityModuleTemplate);
								}
							}
						}
					}
					num *= (float)Mathf.Max(1, ref_project.ChildProjectShipPartUnlocks(faction).Count);
					goto IL_1666;
				}
				case ProjectRole.Radiators:
				{
					List<TIShipPartTemplate> list2 = new List<TIShipPartTemplate>(ref_project.ShipPartUnlocks);
					TIRadiatorTemplate bestRadiatorRaw = faction.GetBestRadiatorRaw();
					if (bestRadiatorRaw != null)
					{
						if (list2.Count > 0)
						{
							bool flag = false;
							float num16 = bestRadiatorRaw.AIScoringValueForResearch();
							float num17 = 1f;
							foreach (TIShipPartTemplate tishipPartTemplate2 in list2)
							{
								TIRadiatorTemplate tiradiatorTemplate = tishipPartTemplate2 as TIRadiatorTemplate;
								if (tiradiatorTemplate != null)
								{
									float num18 = tiradiatorTemplate.AIScoringValueForResearch() / num16;
									if (num18 > num17)
									{
										num17 = num18;
										flag = true;
										num *= AIEvaluators.ModifyProjectScoreForResources(faction, tiradiatorTemplate);
									}
								}
							}
							num *= (flag ? (num17 * 30f) : 0.001f);
						}
					}
					else
					{
						num *= 30f;
					}
					num *= (float)Mathf.Max(1, ref_project.ChildProjectShipPartUnlocks(faction).Count);
					goto IL_1666;
				}
				case ProjectRole.HeatSinks:
				{
					List<TIShipPartTemplate> list3 = new List<TIShipPartTemplate>(ref_project.ShipPartUnlocks);
					TIHeatSinkTemplate bestHeatSink = faction.GetBestHeatSink(false);
					if (list3.Count > 0 && bestHeatSink != null)
					{
						bool flag2 = false;
						float num19 = 1f;
						foreach (TIShipPartTemplate tishipPartTemplate3 in list3)
						{
							TIHeatSinkTemplate tiheatSinkTemplate = tishipPartTemplate3 as TIHeatSinkTemplate;
							if (tiheatSinkTemplate != null)
							{
								float num20 = tiheatSinkTemplate.AIScoringValueForResearch() / bestHeatSink.AIScoringValueForResearch();
								if (num20 > num19)
								{
									num19 = num20;
									num *= AIEvaluators.ModifyProjectScoreForResources(faction, tiheatSinkTemplate);
									flag2 = true;
								}
							}
						}
						num *= (flag2 ? (30f * num19) : 0f);
					}
					num *= (float)Mathf.Max(1, ref_project.ChildProjectShipPartUnlocks(faction).Count);
					goto IL_1666;
				}
				case ProjectRole.PowerPlant:
				{
					List<TIShipPartTemplate> shipPartUnlocks2 = ref_project.ShipPartUnlocks;
					if (shipPartUnlocks2.Count > 0)
					{
						IEnumerable<TIPowerPlantTemplate> allowedPowerPlants = faction.allowedPowerPlants;
						float num21 = 0.01f;
						if (allowedPowerPlants.Any<TIPowerPlantTemplate>())
						{
							num21 = allowedPowerPlants.Max<TIPowerPlantTemplate>((TIPowerPlantTemplate x) => x.maxOutput_GW);
						}
						float num22 = 0f;
						foreach (TIShipPartTemplate tishipPartTemplate4 in shipPartUnlocks2)
						{
							TIPowerPlantTemplate PP = tishipPartTemplate4 as TIPowerPlantTemplate;
							if (PP != null)
							{
								if (PP.weightedBuildMaterials.antimatter > 0f && faction.resourceIncomeDeficiencies.Contains(FactionResource.Antimatter))
								{
									num *= 1E-05f;
								}
								else
								{
									if (allowedPowerPlants.Any<TIPowerPlantTemplate>())
									{
										float num23 = PP.maxOutput_GW / num21;
										if (num23 > num22)
										{
											num22 = num23;
										}
									}
									if (faction.shipDesigns.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.driveTemplate.requiredPowerPlant == PowerPlantRequirement.Any_General || x.driveTemplate.requiredPowerPlant == PP.powerPlantClass))
									{
										num *= 5f;
									}
									num *= AIEvaluators.ModifyProjectScoreForResources(faction, PP);
								}
							}
						}
						if (allowedPowerPlants.Any<TIPowerPlantTemplate>())
						{
							num *= num22;
							if (num22 > 1f)
							{
								num *= 30f;
							}
						}
						num *= (float)Mathf.Max(1, ref_project.ChildProjectShipPartUnlocks(faction).Count);
						goto IL_1666;
					}
					goto IL_1666;
				}
				case ProjectRole.Batteries:
				{
					List<TIShipPartTemplate> shipPartUnlocks3 = ref_project.ShipPartUnlocks;
					TIBatteryTemplate bestBattery = faction.GetBestBattery(null, false);
					if (shipPartUnlocks3.Count <= 0)
					{
						goto IL_1666;
					}
					if (bestBattery != null)
					{
						bool flag3 = false;
						float num24 = 1f;
						foreach (TIShipPartTemplate tishipPartTemplate5 in shipPartUnlocks3)
						{
							TIBatteryTemplate tibatteryTemplate = tishipPartTemplate5 as TIBatteryTemplate;
							if (tibatteryTemplate != null)
							{
								float num25 = tibatteryTemplate.AIScoringValueForResearch() / bestBattery.AIScoringValueForResearch();
								if (num25 > num24)
								{
									num24 = num25;
									flag3 = true;
									num *= AIEvaluators.ModifyProjectScoreForResources(faction, tibatteryTemplate);
								}
							}
						}
						num *= (flag3 ? (30f * num24) : 0.001f);
						goto IL_1666;
					}
					num *= 30f;
					goto IL_1666;
				}
				case ProjectRole.Armor:
				{
					List<TIShipPartTemplate> list4 = new List<TIShipPartTemplate>(ref_project.ShipPartUnlocks);
					TIShipArmorTemplate bestArmor = faction.GetBestArmor(false);
					if (list4.Count > 0)
					{
						bool flag4 = false;
						bool flag5 = false;
						float num26 = 1f;
						foreach (TIShipPartTemplate tishipPartTemplate6 in list4)
						{
							TIShipArmorTemplate tishipArmorTemplate = tishipPartTemplate6 as TIShipArmorTemplate;
							if (tishipArmorTemplate != null)
							{
								flag5 = true;
								float num27 = tishipArmorTemplate.AIScoringValueForResearch() / bestArmor.AIScoringValueForResearch();
								if (num27 > num26)
								{
									num26 = num27;
									flag4 = true;
									num *= AIEvaluators.ModifyProjectScoreForResources(faction, tishipArmorTemplate);
								}
							}
						}
						if (flag5)
						{
							num *= (flag4 ? (30f * num26) : 0.001f);
						}
						else
						{
							num *= 30f;
						}
					}
					num *= (float)Mathf.Max(1, ref_project.ChildProjectShipPartUnlocks(faction).Count);
					goto IL_1666;
				}
				case ProjectRole.EnergyWeapons:
				{
					int num28 = faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Count + faction.GoalsOfType(GoalType.AttackWithFleet, false, true).Count;
					num *= Mathf.Clamp((float)num28 / 10f, 1f, 3f);
					using (List<TIShipPartTemplate>.Enumerator enumerator = ref_project.ShipPartUnlocks.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIShipPartTemplate tishipPartTemplate7 = enumerator.Current;
							TIBeamWeaponTemplate beamWeapon = tishipPartTemplate7 as TIBeamWeaponTemplate;
							if (beamWeapon != null)
							{
								if (!beamWeapon.attackMode)
								{
									num *= 10f;
								}
								else if (beamWeapon.noseWeapon)
								{
									IEnumerable<TIShipWeaponTemplate> enumerable = faction.allowedNoseWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.internalSize == beamWeapon.internalSize && x.weaponClass == beamWeapon.weaponClass);
									if (enumerable.Any<TIShipWeaponTemplate>())
									{
										float num29 = enumerable.Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.AIScoringValueForResearch());
										float num30 = beamWeapon.AIScoringValueForResearch() / Mathf.Max(num29, 0.1f);
										num *= num30;
										num *= AIEvaluators.ModifyProjectScoreForResources(faction, beamWeapon);
										if (num30 > 1f)
										{
											num *= 30f;
										}
									}
									else
									{
										num *= 30f;
									}
								}
								else
								{
									IEnumerable<TIShipWeaponTemplate> enumerable2 = faction.allowedHullWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.internalSize == beamWeapon.internalSize && x.weaponClass == beamWeapon.weaponClass);
									if (enumerable2.Any<TIShipWeaponTemplate>())
									{
										float num31 = enumerable2.Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.AIScoringValueForResearch());
										if (beamWeapon.AIScoringValueForResearch() / Mathf.Max(num31, 0.1f) > 1f)
										{
											num *= 30f;
										}
										num *= AIEvaluators.ModifyProjectScoreForResources(faction, beamWeapon);
									}
									else
									{
										num *= 30f;
									}
								}
								if (beamWeapon.internalSize == 1)
								{
									num *= 2f;
								}
							}
						}
						goto IL_1666;
					}
					break;
				}
				case ProjectRole.Missiles:
				{
					int num32 = faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Count + faction.GoalsOfType(GoalType.AttackWithFleet, false, true).Count;
					num *= Mathf.Clamp((float)num32 / 10f, 1f, 3f);
					List<TIShipPartTemplate> shipPartUnlocks4 = ref_project.ShipPartUnlocks;
					IEnumerable<TIShipWeaponTemplate> enumerable3 = faction.allowedHullWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.isMissileWeapon);
					float num33;
					if (!enumerable3.Any<TIShipWeaponTemplate>())
					{
						num33 = 0f;
					}
					else
					{
						num33 = enumerable3.Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.AIScoringValueForResearch());
					}
					float num34 = num33;
					foreach (TIShipPartTemplate tishipPartTemplate8 in shipPartUnlocks4)
					{
						TIMissileTemplate timissileTemplate = tishipPartTemplate8 as TIMissileTemplate;
						if (timissileTemplate != null)
						{
							if (enumerable3.Any<TIShipWeaponTemplate>())
							{
								float num35 = timissileTemplate.AIScoringValueForResearch() / Mathf.Max(num34, 0.1f);
								num *= num35;
								if (num35 > 1f)
								{
									num *= 30f;
								}
							}
							else
							{
								num *= 30f;
							}
							num *= AIEvaluators.ModifyProjectScoreForResources(faction, timissileTemplate);
						}
						else
						{
							num *= Mathf.Clamp(num34, 3f, 10f);
						}
					}
					if (enumerable3.Count<TIShipWeaponTemplate>() <= 2)
					{
						num *= 20f;
						goto IL_1666;
					}
					goto IL_1666;
				}
				case ProjectRole.Kinetics:
				{
					int num36 = faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Count + faction.GoalsOfType(GoalType.AttackWithFleet, false, true).Count;
					num *= Mathf.Clamp((float)num36 / 10f, 1f, 3f);
					using (List<TIShipPartTemplate>.Enumerator enumerator = ref_project.ShipPartUnlocks.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIShipPartTemplate tishipPartTemplate9 = enumerator.Current;
							TIProjectileWeaponTemplate kineticWeapon = tishipPartTemplate9 as TIProjectileWeaponTemplate;
							if (kineticWeapon != null)
							{
								if (!kineticWeapon.attackMode)
								{
									num *= 2f;
								}
								else
								{
									if (kineticWeapon.noseWeapon)
									{
										IEnumerable<TIShipWeaponTemplate> enumerable4 = faction.allowedNoseWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.internalSize == kineticWeapon.internalSize && x.isGunTypeWeapon);
										if (enumerable4.Any<TIShipWeaponTemplate>())
										{
											float num37 = enumerable4.Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.AIScoringValueForResearch());
											float num38 = kineticWeapon.AIScoringValueForResearch() / Mathf.Max(num37, 0.1f);
											num *= num38;
											if (num38 > 1f)
											{
												num *= 30f;
											}
										}
										else
										{
											num *= 30f;
										}
									}
									else
									{
										IEnumerable<TIShipWeaponTemplate> enumerable5 = faction.allowedHullWeapons.Where<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.internalSize == kineticWeapon.internalSize && x.isGunTypeWeapon);
										if (enumerable5.Any<TIShipWeaponTemplate>())
										{
											float num39 = enumerable5.Max<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.AIScoringValueForResearch());
											float num40 = kineticWeapon.AIScoringValueForResearch() / Mathf.Max(num39, 0.1f);
											num *= num40;
											if (num40 > 1f)
											{
												num *= 30f;
											}
										}
										else
										{
											num *= 30f;
										}
									}
									num *= AIEvaluators.ModifyProjectScoreForResources(faction, kineticWeapon);
								}
								if (kineticWeapon.internalSize == 1)
								{
									num *= 2f;
								}
							}
							else
							{
								num *= 3f;
							}
						}
						goto IL_1666;
					}
					goto IL_0BF5;
				}
				case ProjectRole.Marines:
					goto IL_0BF5;
				case ProjectRole.Xenology:
				case ProjectRole.AlienMissionDefense:
					if (ref_project.HabModuleUnlocks().Count > 0)
					{
						num *= 250f;
						goto IL_1666;
					}
					if (!faction.IsAlienProxy)
					{
						if (ref_project.Effects.Any<TIEffectTemplate>((TIEffectTemplate x) => x.GetContexts().Contains(Context.Mission_EnthrallElites_Def) || x.GetContexts().Contains(Context.Mission_EnthrallPublic_Def) || x.GetContexts().Contains(Context.Mission_Abductions_Def)))
						{
							num *= 500f;
							goto IL_1666;
						}
					}
					num *= 10f;
					goto IL_1666;
				case ProjectRole.TerrestrialWarfare:
					num *= 1f + (float)faction.armies.Count / 10f;
					goto IL_1666;
				case ProjectRole.ControlPointCap:
				{
					float num41 = 1f;
					if (ref_project.Effects.Count > 0)
					{
						num41 = Mathf.Abs(ref_project.Effects[0].value) / (ref_project.GetResearchCost(faction) / 1000f);
					}
					else
					{
						List<TIHabModuleTemplate> list5 = ref_project.HabModuleUnlocks();
						if (list5.Count > 0)
						{
							num41 = (float)list5.ConvertAll<TIHabModuleTemplate>((TIHabModuleTemplate x) => x).Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.controlPointCapacity);
							num *= 250f;
						}
					}
					float num42 = faction.GetAnnualControlPointMaintenanceCost() / 86400f;
					num *= 2f * num41 * (num42 + 6f) / 3f;
					goto IL_1666;
				}
				default:
					goto IL_1666;
				}
				float num43 = AIEvaluators.ScoreExpandNationProject(faction, ref_project);
				if (num43 >= 10f)
				{
					num *= num43 * num43;
					goto IL_1666;
				}
				if (num43 > 0f)
				{
					num *= num43;
					goto IL_1666;
				}
				if (num43 == 0f)
				{
					num *= 3f;
					goto IL_1666;
				}
				num = 1E-37f;
				goto IL_1666;
				IL_0BF5:
				int num44 = faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Count + faction.GoalsOfType(GoalType.CaptureHab, false, true).Count;
				num *= Mathf.Clamp((float)num44 / 10f, 1f, 3f);
				if (ref_project.ShipPartUnlocks.Count > 0)
				{
					num *= 30f;
				}
			}
			else if (tech.AI_techRole == TechRole.SpaceWar && !shipBuilding)
			{
				num *= 0.05f;
			}
			else
			{
				if (forcedFactionTech)
				{
					num *= 50f;
				}
				if (tech.AI_criticalTech)
				{
					num *= 50f;
				}
				num *= (float)(1 + 5 * tech.AllPrereqFor(faction, true).Count<TIGenericTechTemplate>((TIGenericTechTemplate x) => x.AI_criticalTech || faction.forcedTechNames.Contains(x.dataName) || x.AI_techRole == TechRole.FactionObjective));
			}
			IL_1666:
			if (!tech.isGlobalTech())
			{
				TIProjectTemplate ref_project2 = tech.ref_project;
				if (ref_project2 == null || ref_project2.repeatable)
				{
					goto IL_172E;
				}
			}
			float num45 = Mathf.Abs(tech.Effects.Where<TIEffectTemplate>((TIEffectTemplate x) => x.GetContexts().Contains(Context.ControlPointMaintenance)).Sum<TIEffectTemplate>((TIEffectTemplate x) => x.value));
			if (num45 > 0f)
			{
				float num46 = 1f + num45 / 10f;
				num *= num46;
				if (availableMissions.Any<TIMissionTemplate>((TIMissionTemplate x) => x.targetEffects.Any<TIMissionEffect>((TIMissionEffect y) => y is TIMissionEffect_Dominate)))
				{
					num *= 2f * num46;
				}
			}
			IL_172E:
			if (!faction.HasObjectiveProjectAvailable() && tech.LeadsToObjectiveProjects(faction))
			{
				num *= 100f;
			}
			return num;
		}

		// Token: 0x0600494F RID: 18767 RVA: 0x001E6EBC File Offset: 0x001E50BC
		public static float EvaluateTechForTrade(TIFactionState faction, TIGenericTechTemplate tech)
		{
			float researchCost = tech.GetResearchCost(faction);
			float num = 1000f * Mathf.Clamp(Mathf.Log(researchCost) - 6f, 0.5f, 10f);
			num *= faction.TechCategoryValuation(tech.techCategory);
			num *= Mathf.Clamp(faction.TechRoleValuation(tech.AI_techRole), 0.25f, 3f);
			TIProjectTemplate tiprojectTemplate = tech as TIProjectTemplate;
			if (tiprojectTemplate != null)
			{
				if (tiprojectTemplate.AI_criticalTech)
				{
					num *= 2f;
				}
				else
				{
					float num2 = faction.availableProjects.Sum<TIProjectTemplate>((TIProjectTemplate x) => x.GetResearchCost(faction));
					float num3 = faction.completedProjects.Sum<TIProjectTemplate>((TIProjectTemplate x) => x.GetResearchCost(faction));
					float num4 = num3 / (num3 + num2);
					num *= num4;
				}
			}
			return 2f * num;
		}

		// Token: 0x06004950 RID: 18768 RVA: 0x001E6FA8 File Offset: 0x001E51A8
		public static float EvaluateHabModule_PercentChange(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, HabPreferences preferences = null, IEnumerable<TIHabModuleTemplate> existingModules = null, Func<FactionResource, float> GetCurrentMonthlyIncome = null, bool moduleComparison = true, bool newModuleSameEverythingElse = false)
		{
			AIEvaluators.<>c__DisplayClass78_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass78_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.location = location;
			CS$<>8__locals1.moduleTemplate = moduleTemplate;
			CS$<>8__locals1.preferences = preferences;
			if (CS$<>8__locals1.preferences == null)
			{
				CS$<>8__locals1.preferences = new HabPreferences();
			}
			if (GetCurrentMonthlyIncome == null)
			{
				GetCurrentMonthlyIncome = (FactionResource resource) => CS$<>8__locals1.faction.GetMonthlyIncome(resource, true, false);
			}
			if (existingModules == null)
			{
				existingModules = Enumerable.Empty<TIHabModuleTemplate>();
			}
			if (CS$<>8__locals1.moduleTemplate.incomeProjects > 0)
			{
				existingModules = existingModules.ToList<TIHabModuleTemplate>();
			}
			bool flag = CS$<>8__locals1.location.ref_habSite != null;
			CS$<>8__locals1.prospectiveModules = existingModules.ToList<TIHabModuleTemplate>();
			if (CS$<>8__locals1.location.isHabState)
			{
				foreach (TIHabModuleState tihabModuleState in CS$<>8__locals1.location.ref_hab.FunctionalModules())
				{
					CS$<>8__locals1.prospectiveModules.Remove(tihabModuleState.moduleTemplate);
				}
			}
			CS$<>8__locals1.incomePerMonthDiminishingReturnsCutoff = TIResourcesCost.habResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource x) => float.PositiveInfinity);
			CS$<>8__locals1.incomePerMonthDiminishingReturnsCutoff[FactionResource.Operations] = 30f;
			CS$<>8__locals1.incomePerMonthDiminishingReturnsCutoff[FactionResource.Influence] = 60f;
			float num = 0f;
			float num2 = 0f;
			using (HashSet<FactionResource>.Enumerator enumerator2 = TIResourcesCost.habResources.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					AIEvaluators.<>c__DisplayClass78_1 CS$<>8__locals2 = new AIEvaluators.<>c__DisplayClass78_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.resource = enumerator2.Current;
					if (CS$<>8__locals2.resource != FactionResource.Projects && CS$<>8__locals2.resource != FactionResource.MissionControl)
					{
						float num3 = CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.MonthlyResourceIncome(CS$<>8__locals2.resource, CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction);
						Mathf.Max(0f, num3);
						float num4 = CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.MonthlySupportCost(CS$<>8__locals2.resource, true, CS$<>8__locals2.CS$<>8__locals1.faction, null);
						float monthlyRecyclableConsumption = CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.GetMonthlyRecyclableConsumption(CS$<>8__locals2.resource, CS$<>8__locals2.CS$<>8__locals1.faction, null);
						if ((moduleComparison && !newModuleSameEverythingElse) || num3 != 0f || num4 != 0f || (CS$<>8__locals2.resource == FactionResource.Research && (CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.incomeProjects != 0 || CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.techBonuses.Length != 0)))
						{
							num3 -= num4;
							AIEvaluators.<>c__DisplayClass78_2 CS$<>8__locals3;
							CS$<>8__locals3.netMonthlyIncome = GetCurrentMonthlyIncome(CS$<>8__locals2.resource);
							float num5 = CS$<>8__locals2.CS$<>8__locals1.faction.GetMonthlyRevenue_AI(CS$<>8__locals2.resource);
							if (CS$<>8__locals2.CS$<>8__locals1.location.isHabState)
							{
								CS$<>8__locals3.netMonthlyIncome -= CS$<>8__locals2.CS$<>8__locals1.location.ref_hab.GetNetCurrentMonthlyIncome(CS$<>8__locals2.CS$<>8__locals1.faction, CS$<>8__locals2.resource, false, true);
								num5 -= CS$<>8__locals2.CS$<>8__locals1.location.ref_hab.GetMonthlyRevenue(CS$<>8__locals2.resource, true);
							}
							if (newModuleSameEverythingElse)
							{
								num5 += AIEvaluators.cachedPlannedRevenueFromHab[CS$<>8__locals2.resource];
								CS$<>8__locals3.netMonthlyIncome += AIEvaluators.cachedPlannedNetIncomeFromHab[CS$<>8__locals2.resource];
							}
							else
							{
								float num6 = Mathf.Min(existingModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.GetFarmResourceValue(CS$<>8__locals2.resource)), existingModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.MonthlySupportCost(CS$<>8__locals2.resource, true, null, null))) + existingModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.MonthlyResourceIncome(CS$<>8__locals2.resource, CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction)) - existingModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.MonthlySupportCost(CS$<>8__locals2.resource, true, CS$<>8__locals2.CS$<>8__locals1.faction, null));
								CS$<>8__locals3.netMonthlyIncome += num6;
								AIEvaluators.cachedPlannedNetIncomeFromHab[CS$<>8__locals2.resource] = num6;
								float num7 = existingModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.MonthlyResourceRevenue(CS$<>8__locals2.resource, CS$<>8__locals2.CS$<>8__locals1.location, CS$<>8__locals2.CS$<>8__locals1.faction));
								num5 += num7;
								AIEvaluators.cachedPlannedRevenueFromHab[CS$<>8__locals2.resource] = num7;
								AIEvaluators.cachedNonHabCategoryBonuses.Clear();
								AIEvaluators.cachedProspectiveHabCategoryBonuses.Clear();
							}
							if (TIResourcesCost.farmResources.Contains(CS$<>8__locals2.resource))
							{
								num4 -= monthlyRecyclableConsumption * 0.6f;
							}
							if (CS$<>8__locals2.resource == FactionResource.Research)
							{
								if (CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.incomeProjects == 0)
								{
									if (!CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.techBonuses.Any<TechBonus>((TechBonus x) => x.bonus != 0f))
									{
										goto IL_06C9;
									}
								}
								float netMonthlyIncome = CS$<>8__locals3.netMonthlyIncome;
								TechCategory[] techCategories = Enums.TechCategories;
								if (AIEvaluators.cachedNonHabCategoryBonuses.Count == 0 || AIEvaluators.cachedProspectiveHabCategoryBonuses.Count == 0)
								{
									IEnumerable<TechCategory> enumerable = techCategories;
									Func<TechCategory, TechCategory> func = (TechCategory x) => x;
									Func<TechCategory, float> func2;
									if ((func2 = CS$<>8__locals2.CS$<>8__locals1.<>9__12) == null)
									{
										func2 = (CS$<>8__locals2.CS$<>8__locals1.<>9__12 = (TechCategory x) => CS$<>8__locals2.CS$<>8__locals1.faction.OrgsMultiplier(x) + CS$<>8__locals2.CS$<>8__locals1.faction.TraitsMultiplier(x) + CS$<>8__locals2.CS$<>8__locals1.faction.InvestigationsModifier(x) + CS$<>8__locals2.CS$<>8__locals1.faction.FleetsModifier(x) + CS$<>8__locals2.CS$<>8__locals1.faction.EffectsModifier(x));
									}
									AIEvaluators.cachedNonHabCategoryBonuses = enumerable.ToDictionary<TechCategory, TechCategory, float>(func, func2);
									IEnumerable<TechCategory> enumerable2 = techCategories;
									Func<TechCategory, TechCategory> func3 = (TechCategory x) => x;
									Func<TechCategory, float> func4;
									if ((func4 = CS$<>8__locals2.CS$<>8__locals1.<>9__14) == null)
									{
										func4 = (CS$<>8__locals2.CS$<>8__locals1.<>9__14 = delegate(TechCategory techCategory)
										{
											Func<TechBonus, bool> <>9__19;
											return CS$<>8__locals2.CS$<>8__locals1.prospectiveModules.Sum<TIHabModuleTemplate>(delegate(TIHabModuleTemplate x)
											{
												IEnumerable<TechBonus> techBonuses = x.techBonuses;
												Func<TechBonus, bool> func5;
												if ((func5 = <>9__19) == null)
												{
													func5 = (<>9__19 = (TechBonus y) => y.category == techCategory);
												}
												return techBonuses.Where<TechBonus>(func5).Sum<TechBonus>((TechBonus x) => x.bonus);
											});
										});
									}
									AIEvaluators.cachedProspectiveHabCategoryBonuses = enumerable2.ToDictionary<TechCategory, TechCategory, float>(func3, func4);
								}
								int num8 = CS$<>8__locals2.CS$<>8__locals1.faction.TraitProjectCount();
								int num9 = CS$<>8__locals2.CS$<>8__locals1.faction.OrgProjectCount();
								int num10 = CS$<>8__locals2.CS$<>8__locals1.faction.HabProjectCount() + CS$<>8__locals2.CS$<>8__locals1.prospectiveModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.incomeProjects);
								float num11 = CS$<>8__locals2.CS$<>8__locals1.faction.MultipleFacilitiesMultiplier(num8, num9, num10);
								float num12 = techCategories.Sum<TechCategory>(new Func<TechCategory, float>(CS$<>8__locals2.CS$<>8__locals1.<EvaluateHabModule_PercentChange>g__GetPlannedCategoryBonus|16)) / (float)techCategories.Length + num11;
								int num13 = CS$<>8__locals2.CS$<>8__locals1.moduleTemplate.incomeProjects;
								if (num10 == 0)
								{
									num13++;
								}
								float num14 = CS$<>8__locals2.CS$<>8__locals1.faction.MultipleFacilitiesMultiplier(num8, num9, num10 + num13) + techCategories.Sum<TechCategory>(new Func<TechCategory, float>(CS$<>8__locals2.CS$<>8__locals1.<EvaluateHabModule_PercentChange>g__GetCategoryBonusWithModule|17)) / (float)techCategories.Length;
								float num15 = num14 - num12;
								num3 = num3 * (1f + num14) + netMonthlyIncome * num15;
								num5 = (CS$<>8__locals3.netMonthlyIncome = netMonthlyIncome * (1f + num12) + (CS$<>8__locals3.netMonthlyIncome - netMonthlyIncome));
							}
							IL_06C9:
							float num16 = CS$<>8__locals2.<EvaluateHabModule_PercentChange>g__GetPoints|5(AIEvaluators.<EvaluateHabModule_PercentChange>g__GetPercentChange|78_4(num3, CS$<>8__locals3.netMonthlyIncome), false, ref CS$<>8__locals3);
							float num17 = CS$<>8__locals2.<EvaluateHabModule_PercentChange>g__GetPoints|5(AIEvaluators.<EvaluateHabModule_PercentChange>g__GetPercentChange|78_4(num3, num5), true, ref CS$<>8__locals3);
							FactionResource resource2 = CS$<>8__locals2.resource;
							float num18;
							if (resource2 != FactionResource.Money)
							{
								if (resource2 - FactionResource.Water > 3)
								{
									if (resource2 != FactionResource.Fissiles)
									{
										num18 = 0.7f;
									}
									else
									{
										num18 = 0.9f;
									}
								}
								else
								{
									num18 = 0.6f;
								}
							}
							else
							{
								num18 = 0.95f;
							}
							float num19 = Mathf.Lerp(num16, num17, num18);
							if (num19 >= 0f)
							{
								num += num19;
							}
							else
							{
								num2 += num19;
							}
						}
					}
				}
			}
			if (CS$<>8__locals1.moduleTemplate.allowsShipConstruction)
			{
				if (flag && CS$<>8__locals1.location.ref_spaceBody.surfaceGravity_g > 0.05000000074505806)
				{
					num2 -= 1000f;
				}
				else
				{
					float num20 = CS$<>8__locals1.faction.habs.Sum<TIHabState>((TIHabState x) => (from y in x.OkayModules()
						where y.moduleTemplate.allowsShipConstruction
						select y).Sum<TIHabModuleState>((TIHabModuleState y) => (float)y.tier / y.moduleTemplate.constructionTimeModifier));
					float num21 = CS$<>8__locals1.prospectiveModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate y) => y.allowsShipConstruction).Sum<TIHabModuleTemplate>((TIHabModuleTemplate y) => (float)y.tier / y.constructionTimeModifier);
					float num22 = Mathf.Min(100f * ((float)CS$<>8__locals1.moduleTemplate.tier / CS$<>8__locals1.moduleTemplate.constructionTimeModifier) / (num20 + num21), 10f * (float)CS$<>8__locals1.moduleTemplate.tier);
					float num23 = (CS$<>8__locals1.location.ref_system.isEarth ? 1.5f : 1f);
					float num24 = CS$<>8__locals1.preferences[HabMetric.Shipbuilding] * CS$<>8__locals1.faction.template.AdjustedHabPreferences[HabMetric.Shipbuilding];
					num += num22 * num23 * num24;
				}
			}
			num += AIEvaluators.EvaluateHabModule_LEO(CS$<>8__locals1.faction, CS$<>8__locals1.location, CS$<>8__locals1.moduleTemplate, CS$<>8__locals1.preferences, CS$<>8__locals1.prospectiveModules);
			num += AIEvaluators.EvaluateHabModule_Strategy(CS$<>8__locals1.faction, CS$<>8__locals1.location, CS$<>8__locals1.moduleTemplate, CS$<>8__locals1.preferences, CS$<>8__locals1.prospectiveModules);
			float num25 = 1f;
			if (moduleComparison)
			{
				num25 = 1f / AIEvaluators.GetHabModuleSize(CS$<>8__locals1.faction, CS$<>8__locals1.location, CS$<>8__locals1.moduleTemplate, existingModules);
			}
			return (num + 0.5f * num2) * num25;
		}

		// Token: 0x06004951 RID: 18769 RVA: 0x001E792C File Offset: 0x001E5B2C
		public static float GetHabModuleSize(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, IEnumerable<TIHabModuleTemplate> existingModules)
		{
			return AIEvaluators.GetPowerModuleSize(faction, location, moduleTemplate) + AIEvaluators.GetFarmModuleSize(faction, location, moduleTemplate) + 1f;
		}

		// Token: 0x06004952 RID: 18770 RVA: 0x001E7948 File Offset: 0x001E5B48
		public static float GetPowerModuleSize(TIFactionState faction, TIGameState location, float powerRequired)
		{
			TIHabModuleTemplate bestPowerModuleTemplate = PowerDecision.GetBestPowerModuleTemplate(faction, location, null);
			if (bestPowerModuleTemplate == null)
			{
				return 0f;
			}
			int num = bestPowerModuleTemplate.ProspectivePower(location, faction);
			return powerRequired / (float)num;
		}

		// Token: 0x06004953 RID: 18771 RVA: 0x001E7974 File Offset: 0x001E5B74
		public static float GetPowerModuleSize(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate)
		{
			if (moduleTemplate.powerSource)
			{
				return 0f;
			}
			int num = -moduleTemplate.ProspectivePower(location, faction);
			return AIEvaluators.GetPowerModuleSize(faction, location, (float)num);
		}

		// Token: 0x06004954 RID: 18772 RVA: 0x001E79A4 File Offset: 0x001E5BA4
		public static float GetFarmModuleSize(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate)
		{
			if (moduleTemplate.powerSource)
			{
				return 0f;
			}
			TIHabModuleTemplate bestFarm = FarmDecision.GetBestFarm(faction, location, null);
			if (bestFarm == null)
			{
				return 0f;
			}
			return TIResourcesCost.farmResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource y) => y, (FactionResource resource) => bestFarm.GetFarmResourceValue(resource)).Max<KeyValuePair<FactionResource, float>>((KeyValuePair<FactionResource, float> x) => moduleTemplate.GetMonthlyRecyclableConsumption(x.Key, null, null) / x.Value) * 0.6f;
		}

		// Token: 0x06004955 RID: 18773 RVA: 0x001E7A3C File Offset: 0x001E5C3C
		public static float EvaluateHabModule_Strategy(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, HabPreferences preferences, IEnumerable<TIHabModuleTemplate> prospectiveModules)
		{
			float num = 0f;
			int num2 = prospectiveModules.Count<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsShipConstruction);
			TISpaceBodyState system = location.ref_system;
			if (moduleTemplate.allowsShipConstruction && system.isEarth)
			{
				if (faction.nShipyardQueues.Count<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Key.hab.ref_system.isEarth) + num2 < 3)
				{
					num += 26f;
				}
			}
			if (moduleTemplate.allowsShipConstruction && system == GameStateManager.Mars())
			{
				if (faction.nShipyardQueues.Count<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Key.hab.ref_system == GameStateManager.Mars()) + num2 < 3)
				{
					num += 23f;
				}
			}
			if (moduleTemplate.allowsShipConstruction && AIEvaluators.<EvaluateHabModule_Strategy>g__IsBeyondEarthAndNotMars|83_3(location) && num2 == 0)
			{
				if ((from x in faction.nShipyardQueues
					where AIEvaluators.<EvaluateHabModule_Strategy>g__IsBeyondEarthAndNotMars|83_3(x.Key.hab)
					group x by x.Key.hab.ref_system).Count<IGrouping<TISpaceBodyState, KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>>() < 3)
				{
					num += 20f;
				}
			}
			IEnumerable<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>> enumerable = faction.nShipyardQueues.Where<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Key.ref_system == system);
			if (moduleTemplate.allowsShipConstruction && enumerable.Any<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>())
			{
				float num3 = (float)enumerable.Count<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value.Count > 0) / (float)(enumerable.Count<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>() + num2);
				num += Mathf.Pow(num3, 2f) * 20f;
			}
			num *= (float)moduleTemplate.tier;
			if (moduleTemplate.incomeProjects > 0 && !system.isEarth)
			{
				if (prospectiveModules.None<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.incomeProjects > 0))
				{
					if (faction.habs.Where<TIHabState>((TIHabState x) => !x.ref_system.isEarth).None<TIHabState>((TIHabState x) => x.OkayModules().Any<TIHabModuleState>((TIHabModuleState y) => y.moduleTemplate.incomeProjects > 0)))
					{
						num += 30f;
					}
				}
			}
			return num * preferences.Weight;
		}

		// Token: 0x06004956 RID: 18774 RVA: 0x001E7CB4 File Offset: 0x001E5EB4
		public static float EvaluateHabModule_LEO(TIFactionState faction, TIGameState location, TIHabModuleTemplate moduleTemplate, HabPreferences preferences, IEnumerable<TIHabModuleTemplate> prospectiveModules)
		{
			if (location.ref_orbit == null || !location.ref_orbit.isEarthLEO)
			{
				return 0f;
			}
			float num = 0f;
			if (moduleTemplate.HasLEOBonus())
			{
				foreach (PriorityType priorityType in TINationState.Priorities)
				{
					float averageNationPriorityFraction = faction.GetAverageNationPriorityFraction(priorityType);
					HabModuleSpecialRule relevantSpecialModuleRule = HabModuleSpecialRule.none;
					switch (priorityType)
					{
					case PriorityType.Economy:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusEconomy;
						break;
					case PriorityType.Welfare:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusWelfare;
						break;
					case PriorityType.Environment:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusEnvironment;
						break;
					case PriorityType.Knowledge:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusKnowledge;
						break;
					case PriorityType.Government:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusGovernment;
						break;
					case PriorityType.Unity:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusUnity;
						break;
					case PriorityType.Oppression:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusOppression;
						break;
					case PriorityType.LaunchFacilities:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusLaunchFacilities;
						break;
					case PriorityType.MissionControl:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusMissionControl;
						break;
					case PriorityType.Military:
						relevantSpecialModuleRule = HabModuleSpecialRule.LEOBonusMiltech;
						break;
					}
					if (relevantSpecialModuleRule != HabModuleSpecialRule.none && moduleTemplate.specialRules.Contains(relevantSpecialModuleRule))
					{
						float num2 = faction.SumPriorityBonuses(priorityType, true);
						float num3 = prospectiveModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.specialRules.Contains(relevantSpecialModuleRule)).Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.specialRulesValue);
						float num4 = faction.SumLEOHabPriorityBonuses(priorityType, true, num3);
						float num5 = num2 + num4;
						float num6 = num2 + faction.SumLEOHabPriorityBonuses(priorityType, true, num3 + moduleTemplate.specialRulesValue) - num5;
						float num7 = Mathf.Clamp(100f * num6 / (1f + num5), 0f, 10f);
						float num8 = preferences[HabMetric.LEO] * faction.template.AdjustedHabPreferences[HabMetric.LEO];
						num += num7 * averageNationPriorityFraction * num8;
					}
				}
				if (moduleTemplate.controlPointCapacity > 0)
				{
					float num9 = faction.GetControlPointMaintenanceFreebieCap() + (float)prospectiveModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.controlPointCapacity);
					float num10 = Mathf.Clamp((float)(100 * moduleTemplate.controlPointCapacity) / num9, 0f, 10f);
					num += num10 * preferences[HabMetric.LEO] * faction.template.AdjustedHabPreferences[HabMetric.LEO];
				}
				if ((moduleTemplate.AlienDetectionBonus > 0f || moduleTemplate.HumanDetectionBonus > 0f) && faction.councilors.Count > 0)
				{
					float num11 = (float)(faction.councilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Investigation, true, true, true, false, false, false)) / faction.councilors.Count);
					float num12 = 1f;
					float num13 = 0f;
					float num14 = 0f;
					if (moduleTemplate.AlienDetectionBonus > 0f)
					{
						num13 = (float)faction.AlienDetectionBonus + prospectiveModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.AlienDetectionBonus);
						num14 = Mathf.Min(moduleTemplate.specialRulesValue + num13, TemplateManager.global.alienDetectionBonusCapFromLEOHabs);
						if (faction.currentlyDetectingHydra)
						{
							num12 = 2f;
						}
					}
					else if (moduleTemplate.HumanDetectionBonus > 0f)
					{
						num13 = (float)faction.HumanDetectionBonus + prospectiveModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.HumanDetectionBonus);
						num14 = Mathf.Min(moduleTemplate.specialRulesValue + num13, TemplateManager.global.humanDetectionBonusCapFromLEOHabs);
					}
					float num15 = Mathf.Clamp(100f * (num14 - num13) / (num11 + num13), 0f, 10f);
					num += num12 * num15 * preferences[HabMetric.LEO] * faction.template.AdjustedHabPreferences[HabMetric.LEO];
				}
				if (moduleTemplate.ArmyCombatValueBonus > 0f)
				{
					float num16 = faction.armies.Sum<TIArmyState>((TIArmyState x) => x.adjustedTechLevel) / (float)faction.armies.Count;
					float num17 = faction.ArmyCombatBonus + prospectiveModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.ArmyCombatValueBonus);
					float num18 = Mathf.Min(moduleTemplate.specialRulesValue + num17, TemplateManager.global.maxArmyCombatBonusFromLEOHabs);
					float num19 = Mathf.Clamp(100f * (num18 - num17) / (num16 + num17), 0f, 10f);
					num += num19 * preferences[HabMetric.LEO] * faction.template.AdjustedHabPreferences[HabMetric.LEO];
				}
				if (moduleTemplate.PropandaStrengthBonus > 0f)
				{
					float num20 = faction.PropagandaBonus + prospectiveModules.Sum<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.PropandaStrengthBonus);
					float num21 = Mathf.Min(moduleTemplate.specialRulesValue + num20, TemplateManager.global.maxLEOHabPropagandaStrengthBonus);
					float num22 = Mathf.Clamp(100f * (num21 - num20) / (TemplateManager.global.basePropagandaStrength + num20), 0f, 10f);
					num += num22 * preferences[HabMetric.LEO] * faction.template.AdjustedHabPreferences[HabMetric.LEO];
				}
			}
			return num;
		}

		// Token: 0x06004957 RID: 18775 RVA: 0x001E8240 File Offset: 0x001E6440
		public static float EvaluateHabModule(TIFactionState faction, TIHabState hab, TIHabModuleTemplate module, bool expansionPlanned, bool habAllowsResupply, float habSpaceCombatValue, TIHabModuleState currentConstructionModule, int iFactionShipyards, int iHabShipyards, bool upgrade, int iFarms, List<HabModuleSpecialRule> maxxedOutSpecialRules, List<TechCategory> maxxedOutTechCategories)
		{
			float num = 1f;
			if (module.specialRules.Contains(HabModuleSpecialRule.DropTroops))
			{
				int num2 = 0;
				if (faction.IsAlienFaction)
				{
					num2 = hab.tier;
				}
				else
				{
					TISpaceObjectState getSunOrbitingRelatedObject = hab.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
					if (getSunOrbitingRelatedObject.isEarth || getSunOrbitingRelatedObject.ref_spaceBody.habs.Any<TIHabState>((TIHabState x) => x.faction != faction))
					{
						num2++;
					}
				}
				int num3 = hab.AllModules().Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.DropTroops));
				if (num3 >= num2)
				{
					return 0f;
				}
				num = (float)(500 * (hab.tier - num3));
			}
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Money, module.MonthlyResourceIncome(FactionResource.Money, hab, faction));
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Water, module.MonthlyResourceIncome(FactionResource.Water, hab, faction));
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Volatiles, module.MonthlyResourceIncome(FactionResource.Volatiles, hab, faction));
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Operations, module.incomeOps_month);
			if (module.incomeInfluence_month > 0f)
			{
				num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Influence, module.incomeInfluence_month);
			}
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Research, module.incomeResearch_month);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Exotics, module.incomeExotics_month);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Antimatter, module.incomeAntimatter_month);
			num += ((module.missionControl > 0) ? AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.MissionControl, (float)module.missionControl) : 0f);
			num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Projects, (float)module.incomeProjects);
			num += (module.mine ? ((float)module.tier * AIEvaluators.EvaluateHabSite(faction, hab.habSite, false, true, false)) : 0f);
			foreach (TechBonus techBonus in module.techBonuses)
			{
				if (!maxxedOutTechCategories.Contains(techBonus.category))
				{
					num += techBonus.bonus * 10f * faction.TechCategoryValuation(techBonus.category);
				}
			}
			if (module.allowsShipConstruction)
			{
				if (!upgrade)
				{
					if (iHabShipyards == 0)
					{
						num += (float)(10 - iFactionShipyards);
					}
					else if (iHabShipyards < hab.tier)
					{
						num /= 5f * (float)iHabShipyards;
					}
					else
					{
						num = 0.001f;
					}
				}
				else
				{
					num += 10f;
				}
			}
			else if (module.constructionTimeModifier < 1f && (currentConstructionModule == null || module.UpgradesFrom == currentConstructionModule.moduleTemplate))
			{
				if (expansionPlanned)
				{
					num += 50f * (1f - module.constructionTimeModifier);
				}
				else
				{
					num = 0.001f;
				}
			}
			else if (module.allowsResupply)
			{
				if (habAllowsResupply)
				{
					num = 0.001f;
				}
				else
				{
					num += 10f;
				}
			}
			else if (hab.tier >= 2 && module.SpecialRules.Contains(HabModuleSpecialRule.Farm) && (upgrade || iFarms < hab.tier - 1))
			{
				if (iFarms == 0)
				{
					num += (float)(500 * module.tier * hab.tier);
				}
				else if ((float)(hab.crew - iFarms) > module.specialRulesValue)
				{
					num += (float)(500 * module.tier * hab.tier);
				}
			}
			if (module.spaceCombatModule)
			{
				num += Mathf.Max(40f * module.SpaceCombatValue(faction, hab, true) - habSpaceCombatValue, 0f);
				if ((hab.tier >= 2 || hab.HasMine) && habSpaceCombatValue == 0f)
				{
					if (faction.IsAlienFaction)
					{
						num *= 5f;
					}
					else
					{
						num *= (hab.inEarthSystem ? 3f : 1.5f);
					}
				}
				num *= 2f;
			}
			if (hab.IsStation && hab.ref_orbit.isEarthLEO)
			{
				float num4 = 0f;
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusLaunchFacilities) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusLaunchFacilities))
				{
					num4 += module.specialRulesValue * 100f;
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
					{
						num4 += module.specialRulesValue * 50f;
					}
					num4 *= faction.aiValues.wantSpaceFacilities;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusAlienDetection) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusAlienDetection))
				{
					num4 += module.AlienDetectionBonus * 100f;
					if (faction.currentlyDetectingHydra)
					{
						num4 += module.AlienDetectionBonus * 400f;
					}
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusHumanDetection) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusHumanDetection))
				{
					num4 += module.HumanDetectionBonus * 100f;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusArmyCombatValue) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusArmyCombatValue))
				{
					num4 += module.specialRulesValue * 125f * faction.aiValues.wantEarthWarCapability;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusEconomy) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusLaunchFacilities))
				{
					num4 += module.specialRulesValue * 300f;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusKnowledge) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusKnowledge))
				{
					num4 += module.specialRulesValue * 100f;
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Research))
					{
						num4 += module.specialRulesValue * 50f;
					}
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusMiltech) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusMiltech))
				{
					num4 += module.specialRulesValue * 125f * faction.aiValues.wantEarthWarCapability;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusMissionControl) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusMissionControl))
				{
					num4 += module.specialRulesValue * 100f;
					if (faction.resourceIncomeDeficiencies.Contains(FactionResource.MissionControl))
					{
						num4 += module.specialRulesValue * 50f;
					}
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusPropagandaStrength) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusPropagandaStrength))
				{
					num4 += module.specialRulesValue * 100f * faction.aiValues.wantPopularity;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusUnity) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusUnity))
				{
					num4 += module.specialRulesValue * 100f;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusWelfare) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusWelfare))
				{
					num4 += module.specialRulesValue * 125f * faction.aiValues.protectHumanLife;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusGovernment) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusGovernment))
				{
					num4 += module.specialRulesValue * 50f;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusOppression) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusOppression))
				{
					num4 += module.specialRulesValue * 50f;
				}
				if (module.SpecialRules.Contains(HabModuleSpecialRule.LEOBonusEnvironment) && !maxxedOutSpecialRules.Contains(HabModuleSpecialRule.LEOBonusEnvironment))
				{
					num4 += module.specialRulesValue * 25f * faction.aiValues.preserveLife;
				}
				num += num4;
			}
			if (num > 0f && !module.powerSource && !module.coreModule && !module.constructionModule && !faction.IsAlienFaction && module.incomeMoney_month <= 0f && faction.GetDailyIncome(FactionResource.Money, false, false) + faction.mediumTermDailySpoilsIncome <= 0f)
			{
				num += AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Money, -module.MonthlySupportCost(FactionResource.Money, true, faction, hab));
			}
			return num;
		}

		// Token: 0x06004958 RID: 18776 RVA: 0x001E8A32 File Offset: 0x001E6C32
		public static float EvaluateHabSector(TIFactionState faction, TISectorState sector)
		{
			return (float)sector.OkayModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.tier * 3);
		}

		// Token: 0x06004959 RID: 18777 RVA: 0x001E8A60 File Offset: 0x001E6C60
		public static float EvaluateHab(TIFactionState faction, TIHabState hab, bool mySectorsOnly, bool enemySectorsOnly)
		{
			float num = 0f;
			foreach (TISectorState tisectorState in hab.sectors)
			{
				if ((mySectorsOnly && tisectorState.faction == faction) || (enemySectorsOnly && tisectorState.faction != faction) || (!mySectorsOnly && !enemySectorsOnly))
				{
					num += AIEvaluators.EvaluateHabSector(faction, tisectorState);
				}
			}
			return num;
		}

		// Token: 0x0600495A RID: 18778 RVA: 0x001E8AE8 File Offset: 0x001E6CE8
		public static float EvaluateHabResourcesForTrade(TIFactionState faction, TIHabState hab)
		{
			float num = 0f;
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				if (factionResource != FactionResource.MissionControl)
				{
					float num2 = 0f;
					if (hab.IsBase)
					{
						num2 = 1.5f * hab.habSite.GetMonthlyProduction(factionResource) * TIGlobalValuesState.GetMiningRateSettingsModifier(faction);
					}
					float num3 = AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, factionResource, num2, 2f);
					float netCurrentMonthlyIncome = hab.GetNetCurrentMonthlyIncome(hab.faction, factionResource, false, true);
					float num4 = AIEvaluators.EvaluateMonthlyResourceIncome_Trade(faction, factionResource, netCurrentMonthlyIncome, 0.6f);
					if (factionResource == FactionResource.Money)
					{
						num4 *= 1.5f;
					}
					if (netCurrentMonthlyIncome < 0f)
					{
						if (factionResource == FactionResource.MissionControl)
						{
							num4 = 0f;
						}
						else
						{
							float monthlyIncome = faction.GetMonthlyIncome(factionResource, true, false);
							float num5 = 1f + 4f * Mathf.Clamp01(-netCurrentMonthlyIncome / monthlyIncome);
							num4 *= num5;
						}
					}
					num += num4 + num3;
				}
			}
			return num;
		}

		// Token: 0x0600495B RID: 18779 RVA: 0x001E8BD4 File Offset: 0x001E6DD4
		public static float EvaluateHabForTrade(TIFactionState faction, TIHabState hab)
		{
			float num = AIEvaluators.EvaluateHab(faction, hab, false, false);
			num = num * 100f + 500f;
			if (hab.IsBase)
			{
				num += 800f;
			}
			TISpaceBodyState ref_system = hab.ref_system;
			if (ref_system == null || !ref_system.isEarth)
			{
				int num2 = hab.ref_system.habSitesInSystem.Count<TIHabSiteState>();
				if (num2 > 4)
				{
					num *= 2.8f;
					num += 4000f;
				}
				else if (num2 > 2)
				{
					num *= 1.8f;
					num += 2000f;
				}
			}
			num *= 0.65f;
			num += AIEvaluators.EvaluateHabResourcesForTrade(faction, hab);
			return 0.85f * num;
		}

		// Token: 0x0600495C RID: 18780 RVA: 0x001E8C74 File Offset: 0x001E6E74
		public static bool WillReceivingHabCauseOrWorsenDeficit(TIFactionState faction, TIHabState hab)
		{
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				float netCurrentMonthlyIncome = hab.GetNetCurrentMonthlyIncome(hab.faction, factionResource, true, false);
				if (netCurrentMonthlyIncome < 0f && faction.GetMonthlyIncome(factionResource, true, false) + netCurrentMonthlyIncome <= 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600495D RID: 18781 RVA: 0x001E8CC8 File Offset: 0x001E6EC8
		public static bool WillLosingHabCauseOrWorsenDeficit(TIFactionState faction, TIHabState hab)
		{
			foreach (FactionResource factionResource in Enums.FactionResources)
			{
				float netCurrentMonthlyIncome = hab.GetNetCurrentMonthlyIncome(faction, factionResource, true, false);
				if (netCurrentMonthlyIncome > 0f && faction.GetMonthlyIncome(factionResource, true, false) + netCurrentMonthlyIncome <= 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600495E RID: 18782 RVA: 0x001E8D18 File Offset: 0x001E6F18
		public static List<TISpaceBodyState> SpaceBodiesBetween(float lowDist_AU, float highDist_AU)
		{
			return GameStateManager.AllSpaceBodies().Where<TISpaceBodyState>(delegate(TISpaceBodyState x)
			{
				TISpaceObjectState getSunOrbitingRelatedObject = x.GetSunOrbitingRelatedObject;
				if (getSunOrbitingRelatedObject != null && getSunOrbitingRelatedObject.semiMajorAxis_AU >= (double)lowDist_AU)
				{
					TISpaceObjectState getSunOrbitingRelatedObject2 = x.GetSunOrbitingRelatedObject;
					return getSunOrbitingRelatedObject2 != null && getSunOrbitingRelatedObject2.semiMajorAxis_AU <= (double)highDist_AU;
				}
				return false;
			}).ToList<TISpaceBodyState>();
		}

		// Token: 0x0600495F RID: 18783 RVA: 0x001E8D54 File Offset: 0x001E6F54
		public static List<TINaturalSpaceObjectState> SpaceDestinationsBetween(float lowDist_AU, float highDist_AU)
		{
			return GameStateManager.AllSpaceBodiesAndLPoints().Where<TINaturalSpaceObjectState>(delegate(TINaturalSpaceObjectState x)
			{
				TISpaceObjectState getSunOrbitingRelatedObject = x.GetSunOrbitingRelatedObject;
				if (getSunOrbitingRelatedObject != null && getSunOrbitingRelatedObject.semiMajorAxis_AU >= (double)lowDist_AU)
				{
					TISpaceObjectState getSunOrbitingRelatedObject2 = x.GetSunOrbitingRelatedObject;
					return getSunOrbitingRelatedObject2 != null && getSunOrbitingRelatedObject2.semiMajorAxis_AU <= (double)highDist_AU;
				}
				return false;
			}).ToList<TINaturalSpaceObjectState>();
		}

		// Token: 0x06004960 RID: 18784 RVA: 0x001E8D90 File Offset: 0x001E6F90
		public static float EvaluateSpaceBody(TIFactionState faction, TISpaceBodyState body, bool considerDistance = false, bool considerGravity = false, bool considerOccupied = false)
		{
			List<TIHabSiteState> list = (considerOccupied ? body.habSites.ToList<TIHabSiteState>() : body.vacantHabSites);
			if (list.Count > 0)
			{
				return list.Max<TIHabSiteState>((TIHabSiteState x) => AIEvaluators.EvaluateHabSite(faction, x, considerDistance, considerGravity, true)) * (float)((body.habSitesInSystem.Count > 1) ? 4 : 1);
			}
			return 0f;
		}

		// Token: 0x06004961 RID: 18785 RVA: 0x001E8E05 File Offset: 0x001E7005
		public static float GetSolarEnergyEfficiency(TINaturalSpaceObjectState spaceObject)
		{
			return 1f / (float)Math.Pow(spaceObject.ref_system.semiMajorAxis_AU / GameStateManager.Earth().semiMajorAxis_AU, 2.0);
		}

		// Token: 0x06004962 RID: 18786 RVA: 0x001E8E32 File Offset: 0x001E7032
		public static bool IsEnergyEfficient(TINaturalSpaceObjectState spaceObject)
		{
			return AIEvaluators.GetSolarEnergyEfficiency(spaceObject) > 1.33f;
		}

		// Token: 0x06004963 RID: 18787 RVA: 0x001E8E41 File Offset: 0x001E7041
		public static float SpaceResourcesForShipBuild(TIFactionGoalState goal)
		{
			if (goal == null)
			{
				return 0.05f;
			}
			return goal.FractionalImportance(0f);
		}

		// Token: 0x06004964 RID: 18788 RVA: 0x001E8E58 File Offset: 0x001E7058
		public static bool ValidShipyardToSpendBoost(TIFactionState faction, TIHabModuleState shipyard)
		{
			if (!faction.IsActiveHumanFaction)
			{
				return false;
			}
			if (!shipyard.ref_naturalSpaceObject.inEarthSystem)
			{
				return faction.nShipyardQueues.Keys.None<TIHabModuleState>((TIHabModuleState x) => x.ref_naturalSpaceObject.inEarthSystem);
			}
			return true;
		}

		// Token: 0x06004965 RID: 18789 RVA: 0x001E8EB0 File Offset: 0x001E70B0
		public static bool ShouldSpendBoostAtShipyard(TIFactionState faction, TIHabModuleState shipyard, float boost, TIFactionGoalState relatedGoal)
		{
			if (AIEvaluators.ValidShipyardToSpendBoost(faction, shipyard))
			{
				float currentResourceAmount = faction.GetCurrentResourceAmount(FactionResource.Boost);
				if (boost <= 12.5f || currentResourceAmount - boost >= 500f || boost <= 500f * ((relatedGoal != null) ? relatedGoal.FractionalImportance(0f) : 0.5f))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06004966 RID: 18790 RVA: 0x001E8F04 File Offset: 0x001E7104
		public static bool ShouldPayTodaysBoostCost(TISpaceShipTemplate ship, TIFactionState faction, TIHabModuleState shipyard, float spaceResourcesFraction, TIFactionGoalState relatedGoal)
		{
			TIResourcesCost tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(faction, shipyard.hab, ship.spaceResourceConstructionCost(false, shipyard, true, false, false), faction.AvailableSpaceResources(spaceResourcesFraction), true);
			if (tiresourcesCost.CanAfford_AI(faction, ship, shipyard.hab, (relatedGoal != null) ? relatedGoal.importance : 1, false, false, spaceResourcesFraction, null, float.PositiveInfinity))
			{
				float singleCostValue = tiresourcesCost.GetSingleCostValue(FactionResource.Boost);
				return AIEvaluators.ShouldSpendBoostAtShipyard(faction, shipyard, singleCostValue, relatedGoal);
			}
			return false;
		}

		// Token: 0x06004967 RID: 18791 RVA: 0x001E8F6C File Offset: 0x001E716C
		public static bool ShouldRateLimitBoostExpenditure(TIHabModuleTemplate module, TIFactionState faction, TIGameState location)
		{
			AIEvaluators.<>c__DisplayClass101_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass101_0();
			CS$<>8__locals1.faction = faction;
			bool flag = location.ref_orbit != null;
			bool flag2 = !flag;
			if ((!module.coreModule && !module.mine && !module.powerSource && !module.EnablesLocalFounding && !module.allowsShipConstruction) || module.tier > 1)
			{
				return true;
			}
			if (module.allowsShipConstruction)
			{
				int num;
				if (CS$<>8__locals1.faction.nShipyardQueues.Count < 3 && TIResourcesCost.basicSpaceResourcesSansFissiles.All<FactionResource>((FactionResource x) => CS$<>8__locals1.faction.GetDailyIncome(x, true, false) > 0f))
				{
					num = ((CS$<>8__locals1.faction.habs.SelectMany<TIHabState, TIHabModuleState>((TIHabState x) => x.OkayModules()).Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsShipConstruction) < 3) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				return num == 0;
			}
			CS$<>8__locals1.canFoundLocally = CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(location, false, false);
			TIHabState ref_hab = location.ref_hab;
			bool flag3;
			if (ref_hab == null)
			{
				flag3 = false;
			}
			else
			{
				flag3 = ref_hab.UnpoweredModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsShipConstruction);
			}
			bool flag4 = flag3;
			if (((flag && !AIEvaluators.IsEnergyEfficient(location.ref_system)) & CS$<>8__locals1.canFoundLocally) && !flag4)
			{
				return true;
			}
			IEnumerable<TIHabState> enumerable = location.ref_system.habsInSystem.Where<TIHabState>((TIHabState x) => x.faction == CS$<>8__locals1.faction);
			enumerable.Where<TIHabState>((TIHabState x) => x.IsBase);
			if (module.coreModule && enumerable.Any<TIHabState>() && (!flag2 || !location.ref_system.isEarth))
			{
				if (!flag2)
				{
					return true;
				}
				float singleCostValue = module.MinimumBoostCostToday(CS$<>8__locals1.faction, location, false).GetSingleCostValue(FactionResource.Boost);
				float monthlyIncome = CS$<>8__locals1.faction.GetMonthlyIncome(FactionResource.Boost, false, false);
				if (singleCostValue > monthlyIncome * 3f)
				{
					return true;
				}
			}
			if (location.ref_hab != null && module.powerSource)
			{
				return !CS$<>8__locals1.<ShouldRateLimitBoostExpenditure>g__HabNeedsPowerForCriticalModules|3(location.ref_hab);
			}
			if (CS$<>8__locals1.faction.habs.Any<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals1.<ShouldRateLimitBoostExpenditure>g__HabNeedsPowerForCriticalModules|3)))
			{
				return true;
			}
			bool flag5 = CS$<>8__locals1.faction.bases.Any<TIHabState>((TIHabState x) => x.tier == 1 && x.mine.empty && !x.mine.destroyed);
			if (!module.mine && flag5)
			{
				return true;
			}
			if (location.ref_hab != null && module.mine && !location.ref_hab.HasMine)
			{
				return false;
			}
			bool flag6 = CS$<>8__locals1.canFoundLocally || CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(location, true, true);
			return module.EnablesLocalFounding && flag6;
		}

		// Token: 0x06004968 RID: 18792 RVA: 0x001E923C File Offset: 0x001E743C
		public static float GetRateLimitedBoostSpendFraction_Probe(this TIFactionState faction)
		{
			float num = 0.15f;
			if (faction.ProspectedAndSoonToBeProspectedSpaceBodies().Count<TISpaceBodyState>() < 10)
			{
				num *= 2f;
			}
			return num;
		}

		// Token: 0x06004969 RID: 18793 RVA: 0x001E9268 File Offset: 0x001E7468
		public static float GetDaysToWaitForRateLimitedBoostPurchase(TIFactionState faction, float incomeFraction, float boostCost)
		{
			float dailyIncome = faction.GetDailyIncome(FactionResource.Boost, true, false);
			if (dailyIncome < 0f)
			{
				return float.PositiveInfinity;
			}
			float num = dailyIncome * incomeFraction / boostCost;
			return 1f / num;
		}

		// Token: 0x0600496A RID: 18794 RVA: 0x001E929C File Offset: 0x001E749C
		public static float GetMaxBoostForRateLimitedBoostPurchase(TIFactionState faction, float incomeFraction, TIFactionState.BoostAccountName boostAccountName)
		{
			TIDateTime tidateTime;
			if (!faction.boostAccounts.TryGetValue(boostAccountName, out tidateTime) || tidateTime == null)
			{
				tidateTime = (faction.boostAccounts[boostAccountName] = TITimeState.Now());
			}
			float dailyIncome = faction.GetDailyIncome(FactionResource.Boost, true, false);
			if (dailyIncome < 0f)
			{
				return 0f;
			}
			float num = (float)(TITimeState.Now() - tidateTime).TotalDays;
			return dailyIncome * num * incomeFraction;
		}

		// Token: 0x0600496B RID: 18795 RVA: 0x001E9307 File Offset: 0x001E7507
		public static bool ShouldPayRateLimitedBoostCost(TIHabModuleTemplate moduleTemplate, TIFactionState faction, TIGameState location, bool isUpgrade = false)
		{
			return AIEvaluators.ShouldPayRateLimitedBoostCost(moduleTemplate.MinimumBoostCostToday(faction, location, isUpgrade).GetSingleCostValue(FactionResource.Boost), faction, location, isUpgrade);
		}

		// Token: 0x0600496C RID: 18796 RVA: 0x001E9320 File Offset: 0x001E7520
		public static bool ShouldPayRateLimitedBoostCost(float boostCost, TIFactionState faction, TIGameState location, bool isUpgrade = false)
		{
			bool flag = location.ref_habSite != null;
			float num;
			if (flag)
			{
				if (location.ref_habSite.hasPlannedOrOperatingBase)
				{
					return false;
				}
				num = 0.2f;
				if (faction.bases.Any<TIHabState>((TIHabState x) => !x.HasMine))
				{
					num *= 0.5f;
				}
			}
			else
			{
				TISpaceObjectState ref_spaceObject = location.ref_spaceObject;
				bool? flag2;
				if (ref_spaceObject == null)
				{
					flag2 = null;
				}
				else
				{
					TISpaceObjectState getSunOrbitingRelatedObject = ref_spaceObject.GetSunOrbitingRelatedObject;
					flag2 = ((getSunOrbitingRelatedObject != null) ? new bool?(!getSunOrbitingRelatedObject.isEarth) : null);
				}
				if (flag2 ?? true)
				{
					return false;
				}
				num = 0.25f;
				if (location.ref_orbit == null || !location.ref_orbit.isEarthLEO)
				{
					num *= 0.5f;
				}
				List<TIHabState> leostations = faction.LEOStations;
				if (location.ref_orbit == null || !location.ref_orbit.isEarthLEO || (leostations.Any<TIHabState>() && (location.ref_hab == null || location.ref_hab != leostations.FirstOrDefault<TIHabState>())))
				{
					num *= 0.5f;
				}
				float dailyIncome = faction.GetDailyIncome(FactionResource.Water, true, false);
				float dailyIncome2 = faction.GetDailyIncome(FactionResource.Volatiles, true, false);
				if (dailyIncome <= 0.5f || dailyIncome2 <= 0.5f)
				{
					num *= 0.5f;
					if (dailyIncome <= 0f || dailyIncome2 <= 0f)
					{
						num *= 0.25f;
					}
				}
			}
			float num2 = 1f;
			if (boostCost > 0f)
			{
				num2 = AIEvaluators.GetDaysToWaitForRateLimitedBoostPurchase(faction, num, boostCost);
			}
			TIFactionState.BoostAccountName boostAccountName = (flag ? TIFactionState.BoostAccountName.Base : TIFactionState.BoostAccountName.Station);
			if (faction.boostAccounts[boostAccountName] == null)
			{
				faction.boostAccounts[boostAccountName] = TITimeState.Now();
			}
			return (float)(TITimeState.Now() - faction.boostAccounts[boostAccountName]).TotalDays >= num2;
		}

		// Token: 0x0600496D RID: 18797 RVA: 0x001E951C File Offset: 0x001E771C
		public static bool ShouldPayTodaysBoostCost(TIHabModuleTemplate habModuleTemplate, TIFactionState faction, TIGameState location, bool isUpgrade = false, int maxDaysToSave = 180)
		{
			TIResourcesCost tiresourcesCost = habModuleTemplate.MinimumBoostCostToday(faction, location, isUpgrade);
			float singleCostValue = tiresourcesCost.GetSingleCostValue(FactionResource.Boost);
			return singleCostValue == 0f || (!faction.IsAlienFaction && tiresourcesCost.CanAfford_AI(faction, habModuleTemplate, location, 1, false, false, 1f, null, float.PositiveInfinity) && habModuleTemplate.MinimumBoostCost(faction, location, isUpgrade, maxDaysToSave).GetSingleCostValue(FactionResource.Boost) >= tiresourcesCost.GetSingleCostValue(FactionResource.Boost) && (singleCostValue <= 0f || !AIEvaluators.ShouldRateLimitBoostExpenditure(habModuleTemplate, faction, location) || AIEvaluators.ShouldPayRateLimitedBoostCost(singleCostValue, faction, location, isUpgrade)));
		}

		// Token: 0x0600496E RID: 18798 RVA: 0x001E95A8 File Offset: 0x001E77A8
		public static bool CanSaveBoostByWaiting(TIHabModuleTemplate module, TIFactionState faction, TIGameState location, bool isUpgrade = false, int maxDaysToWait = 180)
		{
			TIResourcesCost tiresourcesCost = module.MinimumBoostCostToday(faction, location, isUpgrade);
			return module.MinimumBoostCost(faction, location, isUpgrade, maxDaysToWait).GetSingleCostValue(FactionResource.Boost) < tiresourcesCost.GetSingleCostValue(FactionResource.Boost);
		}

		// Token: 0x0600496F RID: 18799 RVA: 0x001E95DC File Offset: 0x001E77DC
		public static bool ShouldNotTakeOnElectiveExpenditureRightNow(TIFactionState faction, FactionResource resource, float costPerYear)
		{
			if (costPerYear <= 0f)
			{
				return false;
			}
			float num = 0.1f;
			float yearlyIncome = faction.GetYearlyIncome(resource, true, false, false);
			return (costPerYear > 0f && yearlyIncome <= 0f) || costPerYear / yearlyIncome > num;
		}

		// Token: 0x06004970 RID: 18800 RVA: 0x001E961C File Offset: 0x001E781C
		public static bool ShouldNotBuildHabModuleRightNow(TIHabModuleTemplate module, TIFactionState faction, TIGameState location)
		{
			if (faction.IsAlienFaction)
			{
				if (module.coreModule)
				{
					return false;
				}
				if (location.ref_habSite != null && (module.mine || module.powerSource))
				{
					return false;
				}
				bool flag = faction.nShipyardQueues.Any<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value.Any<ShipConstructionQueueItem>((ShipConstructionQueueItem y) => !y.costPaid));
				IEnumerable<TIFactionState.Transaction> filteredTransactions = faction.GetFilteredTransactions(30f, "Ship Construction", FactionResource.None, null);
				return flag && !filteredTransactions.Any<TIFactionState.Transaction>();
			}
			else
			{
				if ((module.coreModule && module.habType == HabType.Base) || module.mine || module.powerSource || module.IsFarm || !AIEvaluators.ShouldRateLimitBoostExpenditure(module, faction, location))
				{
					return false;
				}
				TIResourcesCost tiresourcesCost = module.supportMaterials_month.ToResourcesCost(12f);
				TIHabModuleTemplate bestPowerModuleTemplate = PowerDecision.GetBestPowerModuleTemplate(faction, location, null);
				if (bestPowerModuleTemplate != null)
				{
					float num = (float)(-(float)module.ProspectivePower(location, faction));
					TIHabModuleTemplate bestFarm = FarmDecision.GetBestFarm(faction, location, null);
					if (bestFarm != null)
					{
						num += (float)(-(float)bestFarm.ProspectivePower(location, faction));
						float farmModuleSize = AIEvaluators.GetFarmModuleSize(faction, location, module);
						TIResourcesCost tiresourcesCost2 = bestFarm.supportMaterials_month.ToResourcesCost(12f * farmModuleSize);
						tiresourcesCost.SumCosts_NoDuration(tiresourcesCost2);
					}
					float powerModuleSize = AIEvaluators.GetPowerModuleSize(faction, location, num);
					TIResourcesCost tiresourcesCost3 = bestPowerModuleTemplate.supportMaterials_month.ToResourcesCost(12f * powerModuleSize);
					tiresourcesCost.SumCosts_NoDuration(tiresourcesCost3);
				}
				foreach (FactionResource factionResource in TIResourcesCost.habResources)
				{
					if (!TIResourcesCost.unAccumulatableResources.Contains(factionResource))
					{
						float num2 = tiresourcesCost.GetSingleCostValue(factionResource);
						if (factionResource == FactionResource.Water)
						{
							num2 += 0.5f * (float)module.crew * TemplateManager.global.crewWaterConsumptionTons_year * TemplateManager.global.spaceResourceToTons;
						}
						else if (factionResource == FactionResource.Volatiles)
						{
							num2 += 0.5f * (float)module.crew * TemplateManager.global.crewVolatilesConsumptionTons_year * TemplateManager.global.spaceResourceToTons;
						}
						if (AIEvaluators.ShouldNotTakeOnElectiveExpenditureRightNow(faction, factionResource, num2))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x06004971 RID: 18801 RVA: 0x001E9840 File Offset: 0x001E7A40
		public static bool ShouldPauseHabConstruction(this TIHabState hab)
		{
			if (hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(hab.faction)))
			{
				return true;
			}
			if (hab.ref_system.isEarth)
			{
				return false;
			}
			if (!hab.faction.IsAlienFaction && hab.ref_system.objectType == SpaceObjectType.Planet)
			{
				return false;
			}
			if (hab.ActiveCombatModules().Count > 0)
			{
				return false;
			}
			if (hab.IsStation)
			{
				if (hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue() > 0f))
				{
					return false;
				}
			}
			return hab.ref_system.fleetsInSystem.Any<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(hab.IsThreateningFleet));
		}

		// Token: 0x06004972 RID: 18802 RVA: 0x001E9938 File Offset: 0x001E7B38
		public static AIEvaluators.MoneySitation GetMoneySituation(this TIFactionState faction, float spoilsValue = 0f)
		{
			float currentResourceAmount = faction.GetCurrentResourceAmount(FactionResource.Money);
			float num = faction.GetMonthlyIncome(FactionResource.Money, true, false);
			if (spoilsValue > 0f)
			{
				num += faction.mediumTermDailySpoilsIncome * 30.436874f * spoilsValue;
			}
			if (num < 0f || currentResourceAmount < 0f)
			{
				if (currentResourceAmount > 50000f)
				{
					return AIEvaluators.MoneySitation.Tight;
				}
				if (currentResourceAmount > 25000f)
				{
					return AIEvaluators.MoneySitation.Bad;
				}
				return AIEvaluators.MoneySitation.Terrible;
			}
			else if (num < 50f || currentResourceAmount < 4000f)
			{
				if (currentResourceAmount > 25000f)
				{
					return AIEvaluators.MoneySitation.Tight;
				}
				return AIEvaluators.MoneySitation.Bad;
			}
			else
			{
				if (num >= 150f && currentResourceAmount >= 8000f)
				{
					return AIEvaluators.MoneySitation.Ok;
				}
				if (currentResourceAmount > 25000f)
				{
					return AIEvaluators.MoneySitation.Ok;
				}
				return AIEvaluators.MoneySitation.Tight;
			}
		}

		// Token: 0x06004973 RID: 18803 RVA: 0x001E99D0 File Offset: 0x001E7BD0
		public static float EvaluateHabSite(TIFactionState faction, TIHabSiteState habSite, bool considerDistance = false, bool considerGravity = false, bool considerZeroes = true)
		{
			float num = 0f;
			float num2;
			float num3;
			float num4;
			float num5;
			float num6;
			if (faction.Prospected(habSite))
			{
				num2 = habSite.GetDailyProduction(FactionResource.Water);
				num3 = habSite.GetDailyProduction(FactionResource.Volatiles);
				num4 = habSite.GetDailyProduction(FactionResource.Metals);
				num5 = habSite.GetDailyProduction(FactionResource.NobleMetals);
				num6 = habSite.GetDailyProduction(FactionResource.Fissiles);
			}
			else
			{
				num2 = habSite.GetHabSiteExpectedProductivity_day(FactionResource.Water);
				num3 = habSite.GetHabSiteExpectedProductivity_day(FactionResource.Volatiles);
				num4 = habSite.GetHabSiteExpectedProductivity_day(FactionResource.Metals);
				num5 = habSite.GetHabSiteExpectedProductivity_day(FactionResource.NobleMetals);
				num6 = habSite.GetHabSiteExpectedProductivity_day(FactionResource.Fissiles);
			}
			float num7 = 2f;
			float num8 = 2f;
			float num9;
			if (faction.IsAlienFaction)
			{
				num7 = 4f;
				num8 = 4f;
				num9 = 4f;
			}
			else
			{
				num9 = 3f;
			}
			num += num7 * AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Water, num2);
			num += num7 * AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Volatiles, num3);
			num += num8 * AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Metals, num4);
			num += num8 * AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.NobleMetals, num5);
			num += num9 * AIEvaluators.EvaluateMonthlyResourceIncome(faction, FactionResource.Fissiles, num6);
			if (considerZeroes)
			{
				if (num2 == 0f)
				{
					num /= 4f;
				}
				if (num3 == 0f)
				{
					num /= 3f;
				}
				if (num4 == 0f)
				{
					num /= 2f;
				}
				if (num5 == 0f)
				{
					num /= 1.25f;
				}
			}
			if (faction.IsActiveHumanFaction)
			{
				float num10 = (float)habSite.ref_spaceBody.GetSunOrbitingRelatedObject.semiMajorAxis_AU;
				if (considerDistance)
				{
					num *= 12.5f - Mathf.Abs(num10 - 1f) / 4f;
				}
				if (num10 < 1f)
				{
					num /= num10;
					num /= num10;
					num /= num10;
				}
			}
			else if (considerDistance && habSite.ref_spaceBody.GetSunOrbitingRelatedObject.semiMajorAxis_AU > 35.0)
			{
				double num11 = TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(habSite.ref_spaceBody, faction.primaryHab.ref_spaceBody) / 149597870700.0;
				if (num11 > 0.0)
				{
					num /= (float)num11;
				}
			}
			if (considerGravity && faction.IsActiveHumanFaction)
			{
				num *= 1f - (float)habSite.surfaceGravity_g;
			}
			num *= 1f / Mathf.Max(1f, habSite.irradiatedValue);
			if (!faction.veryProAlien)
			{
				if (habSite.ref_spaceBody.surfaceBases.Any<TIHabState>((TIHabState x) => x.IsAlien()))
				{
					num *= 1E-05f;
				}
			}
			return num;
		}

		// Token: 0x06004974 RID: 18804 RVA: 0x001E9C34 File Offset: 0x001E7E34
		[return: TupleElementNames(new string[] { "MeetsMinimum", "MeetsRecommended", "MeetsGood" })]
		public static Dictionary<FactionResource, ValueTuple<bool, bool, bool>> GetSpaceResourceIncomesChecklist(Func<FactionResource, float> GetIncomePerMonth)
		{
			return TIResourcesCost.basicSpaceResources.ToDictionary<FactionResource, FactionResource, ValueTuple<bool, bool, bool>>((FactionResource x) => x, delegate(FactionResource resource)
			{
				float num = GetIncomePerMonth(resource);
				bool flag = num >= AIEvaluators.<GetSpaceResourceIncomesChecklist>g__GetMinimumIncomePerMonth|115_0(resource);
				bool flag2 = num >= AIEvaluators.<GetSpaceResourceIncomesChecklist>g__GetRecommendedIncomePerMonth|115_1(resource);
				bool flag3 = num >= AIEvaluators.<GetSpaceResourceIncomesChecklist>g__GetGoodIncomePerMonth|115_2(resource);
				return new ValueTuple<bool, bool, bool>(flag, flag2, flag3);
			});
		}

		// Token: 0x06004975 RID: 18805 RVA: 0x001E9C83 File Offset: 0x001E7E83
		public static bool IsChecklistComplete([TupleElementNames(new string[] { "MeetsMinimum", "MeetsRecommended", "MeetsGood" })] Dictionary<FactionResource, ValueTuple<bool, bool, bool>> incomeChecklist)
		{
			return incomeChecklist.Values.All<ValueTuple<bool, bool, bool>>(([TupleElementNames(new string[] { "MeetsMinimum", "MeetsRecommended", "MeetsGood" })] ValueTuple<bool, bool, bool> x) => x.Item1 && x.Item2 && x.Item3);
		}

		// Token: 0x06004976 RID: 18806 RVA: 0x001E9CB0 File Offset: 0x001E7EB0
		public static float EstimateFutureIncomePerMonth(TIFactionState faction, FactionResource resourceType, bool includeAvailableHabSites, bool includePendingHabSites, bool doNotDiscountLongtermIncomes = false)
		{
			AIEvaluators.<>c__DisplayClass117_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass117_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.resourceType = resourceType;
			if (CS$<>8__locals1.faction.IsAlienFaction)
			{
				TIHabModuleTemplate tihabModuleTemplate = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining)
					orderby x.tier descending
					where !x.automated
					where x.alienModule == CS$<>8__locals1.faction.IsAlienFaction
					select x).FirstOrDefault<TIHabModuleTemplate>();
				float hypotheticalMiningModifier = tihabModuleTemplate.miningModifier;
				float num = (from x in (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.FoundBase, false, true)
						select x as FactionGoal_FoundBase).ToList<FactionGoal_FoundBase>()
					select x.site).Concat<TIHabSiteState>(CS$<>8__locals1.faction.bases.Select<TIHabState, TIHabSiteState>((TIHabState x) => x.habSite)).Distinct<TIHabSiteState>().ToList<TIHabSiteState>()
					.Sum<TIHabSiteState>((TIHabSiteState x) => x.GetMonthlyProduction(CS$<>8__locals1.resourceType) * hypotheticalMiningModifier);
				return Mathf.Max(CS$<>8__locals1.faction.GetMonthlyIncome(CS$<>8__locals1.resourceType, false, false), num);
			}
			float monthlyIncomeWithoutDiplomacy = CS$<>8__locals1.faction.GetMonthlyIncomeWithoutDiplomacy(CS$<>8__locals1.resourceType);
			if (!TIResourcesCost.basicSpaceResources.Contains(CS$<>8__locals1.resourceType))
			{
				return monthlyIncomeWithoutDiplomacy;
			}
			float num2 = CS$<>8__locals1.<EstimateFutureIncomePerMonth>g__GetUnderConstructionMineIncome|0(CS$<>8__locals1.resourceType);
			float num3 = 0f;
			if (includeAvailableHabSites || includePendingHabSites)
			{
				AIEvaluators.<>c__DisplayClass117_3 CS$<>8__locals3 = new AIEvaluators.<>c__DisplayClass117_3();
				CS$<>8__locals3.vacuumIncomeChecklist = AIEvaluators.GetSpaceResourceIncomesChecklist((FactionResource x) => CS$<>8__locals1.faction.GetMonthlyIncomeWithoutDiplomacy(x) + base.<EstimateFutureIncomePerMonth>g__GetUnderConstructionMineIncome|0(x));
				bool flag = AIEvaluators.IsChecklistComplete(CS$<>8__locals3.vacuumIncomeChecklist);
				int ai_GenericMissionControlAvailable = CS$<>8__locals1.faction.AI_GenericMissionControlAvailable;
				float num4 = 0.5f;
				if (includeAvailableHabSites)
				{
					List<TIHabSiteState> list = CS$<>8__locals1.<EstimateFutureIncomePerMonth>g__GetHabSites|11(CS$<>8__locals1.faction.ProspectedSpaceBodies()).OrderByDescending<TIHabSiteState, float>(new Func<TIHabSiteState, float>(CS$<>8__locals3.<EstimateFutureIncomePerMonth>g__EvaluateHabSite|15)).ToList<TIHabSiteState>();
					int num5 = Mathf.Max(1, ai_GenericMissionControlAvailable / 3);
					if (flag)
					{
						list = list.SelectRandomItems<TIHabSiteState>(num5).ToList<TIHabSiteState>();
					}
					else
					{
						list = list.Take<TIHabSiteState>(num5).ToList<TIHabSiteState>();
					}
					num3 += list.Sum<TIHabSiteState>((TIHabSiteState x) => TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth[x][CS$<>8__locals1.resourceType]) * num4;
				}
				float num6 = num4 * 0.7f;
				if (includePendingHabSites)
				{
					List<TIHabSiteState> list2 = CS$<>8__locals1.<EstimateFutureIncomePerMonth>g__GetHabSites|11(from x in GameStateManager.AllSpaceBodies()
						where CS$<>8__locals1.faction.ProspectingSpaceBody(x)
						select x).ToList<TIHabSiteState>();
					if (!doNotDiscountLongtermIncomes)
					{
						int num7 = Mathf.Max(1, ai_GenericMissionControlAvailable / 4);
						if (flag)
						{
							list2 = list2.SelectRandomItems<TIHabSiteState>(num7).ToList<TIHabSiteState>();
						}
						else
						{
							list2 = list2.OrderByDescending<TIHabSiteState, float>(new Func<TIHabSiteState, float>(CS$<>8__locals3.<EstimateFutureIncomePerMonth>g__EvaluateHabSite|20)).Take<TIHabSiteState>(num7).ToList<TIHabSiteState>();
						}
					}
					num3 += list2.Sum<TIHabSiteState>((TIHabSiteState x) => TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth[x][CS$<>8__locals1.resourceType]) * (doNotDiscountLongtermIncomes ? 1f : num6);
				}
			}
			return monthlyIncomeWithoutDiplomacy + num2 + num3;
		}

		// Token: 0x06004977 RID: 18807 RVA: 0x001E9FF0 File Offset: 0x001E81F0
		public static float EvaluateSpaceResourceIncomes_Strategic(Func<FactionResource, float> GetIncomePerMonth, Dictionary<FactionResource, ValueTuple<bool, bool, bool>> incomeChecklist)
		{
			if (AIEvaluators.IsChecklistComplete(incomeChecklist))
			{
				return -1f;
			}
			return TIResourcesCost.basicSpaceResources.Sum<FactionResource>((FactionResource x) => base.<EvaluateSpaceResourceIncomes_Strategic>g__GetResourceScore|0(x));
		}

		// Token: 0x06004978 RID: 18808 RVA: 0x001EA03C File Offset: 0x001E823C
		public static bool PassesBudgetingRules(TIFactionState faction, TIDataTemplate item, FactionResource resource, float cost, bool isPlanned, bool useSavingTargetBank = false)
		{
			if (faction.IsAlienFaction)
			{
				TISpaceShipTemplate tispaceShipTemplate = item as TISpaceShipTemplate;
				if (tispaceShipTemplate != null && tispaceShipTemplate.role.IsCombatantRole() && cost > 0f && faction.IsResourceUpkeepInsecure(resource, isPlanned ? AIEvaluators.UpkeepInsecurityType.Present : AIEvaluators.UpkeepInsecurityType.PresentCautious))
				{
					return false;
				}
			}
			float num = faction.GetCurrentResourceAmount(resource) - cost;
			float num2 = 0f;
			if (!useSavingTargetBank && faction.AISavingTarget.active)
			{
				num2 = Mathf.Max(num2, faction.AISavingTarget.GetBankedQuantity(resource));
			}
			return num >= num2;
		}

		// Token: 0x06004979 RID: 18809 RVA: 0x001EA0BC File Offset: 0x001E82BC
		public static bool PassesBudgetingRules(TIFactionState faction, TIDataTemplate item, TIResourcesCost cost, bool isPlanned, bool useSavingTargetBank = false)
		{
			return cost.resourceCosts.All<ResourceValue>((ResourceValue x) => AIEvaluators.PassesBudgetingRules(faction, item, x.resource, x.value, isPlanned, useSavingTargetBank));
		}

		// Token: 0x0600497A RID: 18810 RVA: 0x001EA103 File Offset: 0x001E8303
		public static bool PassesBudgetingRulesExceptExotics(TIFactionState faction, TIDataTemplate item, TIResourcesCost cost, bool isPlanned, bool useSavingTargetBank = false)
		{
			return !AIEvaluators.PassesBudgetingRules(faction, item, FactionResource.Exotics, cost.GetSingleCostValue(FactionResource.Exotics), isPlanned, useSavingTargetBank) && AIEvaluators.PassesBudgetingRulesSansExotics(faction, item, cost, isPlanned, useSavingTargetBank);
		}

		// Token: 0x0600497B RID: 18811 RVA: 0x001EA128 File Offset: 0x001E8328
		public static bool PassesBudgetingRulesSansExotics(TIFactionState faction, TIDataTemplate item, TIResourcesCost cost, bool isPlanned, bool useSavingTargetBank = false)
		{
			return cost.resourceCosts.All<ResourceValue>((ResourceValue x) => x.resource == FactionResource.Exotics || AIEvaluators.PassesBudgetingRules(faction, item, x.resource, x.value, isPlanned, useSavingTargetBank));
		}

		// Token: 0x0600497C RID: 18812 RVA: 0x001EA170 File Offset: 0x001E8370
		public static void ClearResourceUpkeepInsecurityCache(TIFactionState faction)
		{
			Dictionary<FactionResource, Dictionary<AIEvaluators.UpkeepInsecurityType, ValueTuple<bool, TIDateTime>>> dictionary;
			if (AIEvaluators.upkeepInsecurityCache != null && AIEvaluators.upkeepInsecurityCache.TryGetValue(faction, out dictionary))
			{
				foreach (KeyValuePair<FactionResource, Dictionary<AIEvaluators.UpkeepInsecurityType, ValueTuple<bool, TIDateTime>>> keyValuePair in dictionary)
				{
					keyValuePair.Value.Clear();
				}
			}
		}

		// Token: 0x0600497D RID: 18813 RVA: 0x001EA1DC File Offset: 0x001E83DC
		public static bool IsResourceUpkeepInsecure(this TIFactionState faction, FactionResource resource, AIEvaluators.UpkeepInsecurityType upkeepInsecurityType)
		{
			if (!TIResourcesCost.spaceResources.Contains(resource))
			{
				return false;
			}
			if (AIEvaluators.upkeepInsecurityCache == null)
			{
				AIEvaluators.ClearUpkeepInsecurityCache();
			}
			ValueTuple<bool, TIDateTime> valueTuple;
			if (AIEvaluators.upkeepInsecurityCache[faction][resource].TryGetValue(upkeepInsecurityType, out valueTuple))
			{
				try
				{
					float num = (float)(TITimeState.Now() - valueTuple.Item2).TotalDays;
					if (num >= 0f && num <= 14f)
					{
						return valueTuple.Item1;
					}
				}
				catch (Exception ex)
				{
					Log.Error("Ran into exception IsResourceUpkeepInsecure() try/catch block. " + ex.ToString(), Array.Empty<object>());
					AIEvaluators.ClearUpkeepInsecurityCache();
				}
			}
			bool flag = false;
			float num2 = faction.EstimateExpenditurePerDay(TIFactionState.Expenditure.ShipMaintainence, resource);
			float num3 = faction.GetDailyIncome(resource, true, false);
			float currentResourceAmount = faction.GetCurrentResourceAmount(resource);
			float num4 = 730.4844f;
			if (upkeepInsecurityType != AIEvaluators.UpkeepInsecurityType.PresentCautious)
			{
				if (upkeepInsecurityType == AIEvaluators.UpkeepInsecurityType.Future)
				{
					num3 += faction.GetUnderConstructionMiningIncomePerDay(resource);
				}
			}
			else
			{
				if (faction.IsResourceUpkeepInsecure(resource, AIEvaluators.UpkeepInsecurityType.Present))
				{
					flag = true;
				}
				num2 = Mathf.Min(num2 * 10f, faction.PredictMaximumMaintainenceCostsPerDay(resource));
				num4 = 182.6211f;
			}
			float num5 = num3 - num2;
			float num6 = currentResourceAmount / Mathf.Max(-num5, 0f);
			if (!flag)
			{
				if (num6 < num4)
				{
					flag = true;
				}
				else if (upkeepInsecurityType == AIEvaluators.UpkeepInsecurityType.Future && num5 > 0f)
				{
					flag = false;
				}
				else
				{
					float num7 = num2 * 365.2422f * 0.33f;
					float num8 = faction.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Sum<TISpaceShipState>((TISpaceShipState x) => x.template.propellantTanksBuildCost(faction, resource)) * 0.15f;
					if (upkeepInsecurityType == AIEvaluators.UpkeepInsecurityType.PresentCautious)
					{
						num7 *= 2f;
						num8 *= 2.5f;
					}
					float num9 = Mathf.Max(num7, num8);
					flag = currentResourceAmount < num9;
				}
			}
			TIDateTime tidateTime = TITimeState.Now();
			AIEvaluators.upkeepInsecurityCache[faction][resource][upkeepInsecurityType] = new ValueTuple<bool, TIDateTime>(flag, tidateTime);
			return flag;
		}

		// Token: 0x0600497E RID: 18814 RVA: 0x001EA448 File Offset: 0x001E8648
		private static void ClearUpkeepInsecurityCache()
		{
			AIEvaluators.upkeepInsecurityCache = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, Dictionary<FactionResource, Dictionary<AIEvaluators.UpkeepInsecurityType, ValueTuple<bool, TIDateTime>>>>((TIFactionState x) => x, (TIFactionState x) => TIResourcesCost.spaceResources.ToDictionary<FactionResource, FactionResource, Dictionary<AIEvaluators.UpkeepInsecurityType, ValueTuple<bool, TIDateTime>>>((FactionResource y) => y, (FactionResource y) => new Dictionary<AIEvaluators.UpkeepInsecurityType, ValueTuple<bool, TIDateTime>>()));
		}

		// Token: 0x0600497F RID: 18815 RVA: 0x001EA4A4 File Offset: 0x001E86A4
		public static IEnumerable<FactionResource> ResourcesExperiencingUpkeepInsecurity(this TIFactionState faction)
		{
			return TIResourcesCost.basicSpaceResourcesSansFissiles.Where<FactionResource>((FactionResource x) => faction.IsResourceUpkeepInsecure(x, AIEvaluators.UpkeepInsecurityType.Present));
		}

		// Token: 0x06004980 RID: 18816 RVA: 0x001EA4D4 File Offset: 0x001E86D4
		public static bool HasUpkeepInsecurity(this TIFactionState faction)
		{
			return faction.ResourcesExperiencingUpkeepInsecurity().Any<FactionResource>();
		}

		// Token: 0x06004981 RID: 18817 RVA: 0x001EA4E4 File Offset: 0x001E86E4
		public static IEnumerable<FactionResource> ResourcesExperiencingUpkeepInsecurityInTheFuture(this TIFactionState faction)
		{
			return TIResourcesCost.basicSpaceResourcesSansFissiles.Where<FactionResource>((FactionResource x) => faction.IsResourceUpkeepInsecure(x, AIEvaluators.UpkeepInsecurityType.Future));
		}

		// Token: 0x06004982 RID: 18818 RVA: 0x001EA514 File Offset: 0x001E8714
		public static bool HasUpkeepInsecurityInTheFuture(this TIFactionState faction)
		{
			return faction.ResourcesExperiencingUpkeepInsecurityInTheFuture().Any<FactionResource>();
		}

		// Token: 0x06004983 RID: 18819 RVA: 0x001EA524 File Offset: 0x001E8724
		public static IEnumerable<FactionResource> ResourcesExperiencingUpkeepInsecurity_Cautious(this TIFactionState faction)
		{
			return TIResourcesCost.basicSpaceResourcesSansFissiles.Where<FactionResource>((FactionResource x) => faction.IsResourceUpkeepInsecure(x, AIEvaluators.UpkeepInsecurityType.PresentCautious));
		}

		// Token: 0x06004984 RID: 18820 RVA: 0x001EA554 File Offset: 0x001E8754
		public static bool FuelEfficiencyMode(this TIFactionState faction)
		{
			return faction.ResourcesExperiencingUpkeepInsecurity_Cautious().Any<FactionResource>();
		}

		// Token: 0x06004985 RID: 18821 RVA: 0x001EA564 File Offset: 0x001E8764
		public static float GetStaticFleetFraction(this TIFactionState faction)
		{
			return (float)faction.fleets.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				FactionGoal_Fleet factionGoal_Fleet = x.AssignedGoal();
				return factionGoal_Fleet != null && factionGoal_Fleet.GetGoalType() == GoalType.DefendWithFleet;
			}).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Count) / (float)faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Count);
		}

		// Token: 0x06004986 RID: 18822 RVA: 0x001EA5EC File Offset: 0x001E87EC
		public static float GetTargetDesiredStaticFleetFraction(TIFactionState faction)
		{
			if (faction.HasUpkeepInsecurity())
			{
				return 0.95f;
			}
			if (faction.FuelEfficiencyMode())
			{
				return 0.35f;
			}
			return 0f;
		}

		// Token: 0x06004987 RID: 18823 RVA: 0x001EA60F File Offset: 0x001E880F
		public static bool ShouldIncreaseStaticFleetFraction(this TIFactionState faction)
		{
			return faction.GetStaticFleetFraction() < faction.GetDesiredStaticFleetFraction();
		}

		// Token: 0x06004988 RID: 18824 RVA: 0x001EA620 File Offset: 0x001E8820
		public static IEnumerable<FactionGoal_DefendWithFleet> GetBossDefenseGoals(TIFactionState faction)
		{
			if (!faction.IsAlienFaction)
			{
				return Enumerable.Empty<FactionGoal_DefendWithFleet>();
			}
			IEnumerable<FactionGoal_DefendWithFleet> enumerable = from x in faction.GoalsOfType(GoalType.DefendWithFleet, false, true)
				select x as FactionGoal_DefendWithFleet;
			IEnumerable<FactionGoal_DefendWithFleet> enumerable2 = from x in (from x in faction.GoalsOfType(GoalType.DefendWithFleet, false, true)
					select x as FactionGoal_DefendWithFleet into x
					where x.target().isSpaceBodyState
					select x).Where<FactionGoal_DefendWithFleet>(delegate(FactionGoal_DefendWithFleet x)
				{
					TIGameState tigameState = x.target();
					return ((tigameState != null) ? tigameState.ref_system : null) != null;
				})
				group x by x.target().ref_system into x
				where x.Key.objectType == SpaceObjectType.Planet
				where x.Key.semiMajorAxis_AU >= GameStateManager.Jupiter().semiMajorAxis_AU
				select x.FirstOrDefault<FactionGoal_DefendWithFleet>() into x
				orderby x.target().ref_system.semiMajorAxis_AU
				where x != null
				select x;
			FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = enumerable.FirstOrDefault<FactionGoal_DefendWithFleet>((FactionGoal_DefendWithFleet x) => x.target() == faction.primaryHab.ref_spaceBody);
			if (factionGoal_DefendWithFleet != null)
			{
				enumerable2 = enumerable2.Append(factionGoal_DefendWithFleet);
			}
			return enumerable2;
		}

		// Token: 0x06004989 RID: 18825 RVA: 0x001EA7F8 File Offset: 0x001E89F8
		public static FactionGoal_DefendWithFleet GetNextBossDefenseGoalToFortify(TIFactionState faction, List<FactionGoal_DefendWithFleet> bosses = null)
		{
			if (bosses == null)
			{
				bosses = AIEvaluators.GetBossDefenseGoals(faction).ToList<FactionGoal_DefendWithFleet>();
			}
			return bosses.MinBy<FactionGoal_DefendWithFleet, float>(delegate(FactionGoal_DefendWithFleet boss)
			{
				float num = (float)bosses.IndexOf(boss) / (float)(bosses.Count - 1);
				return (float)boss.EarmarkedFleetMC * (1f - num * 0.5f);
			});
		}

		// Token: 0x0600498A RID: 18826 RVA: 0x001EA844 File Offset: 0x001E8A44
		public static FactionResource GetCriticalBasicSpaceResource(this TIFactionState faction)
		{
			if (!faction.fleets.SelectMany<TISpaceFleetState, TISpaceShipState>((TISpaceFleetState x) => x.ships).Any<TISpaceShipState>())
			{
				return FactionResource.Water;
			}
			ValueTuple<FactionResource, TIDateTime> valueTuple;
			if (AIEvaluators.cachedCriticalResources.TryGetValue(faction, out valueTuple) && (TITimeState.Now() - valueTuple.Item2).TotalDays < 30.0)
			{
				return valueTuple.Item1;
			}
			Dictionary<FactionResource, float> dictionary = faction.GetTypicalShipBuildCostSansRareMaterials().resourceCosts.ToDictionary<ResourceValue, FactionResource, float>((ResourceValue x) => x.resource, (ResourceValue x) => faction.GetCurrentResourceAmount(x.resource) / x.value);
			return (AIEvaluators.cachedCriticalResources[faction] = new ValueTuple<FactionResource, TIDateTime>(dictionary.MinBy<KeyValuePair<FactionResource, float>, float>((KeyValuePair<FactionResource, float> x) => x.Value).Key, TITimeState.Now())).Item1;
		}

		// Token: 0x0600498B RID: 18827 RVA: 0x001EA967 File Offset: 0x001E8B67
		public static bool IsSpaceBodyDangerous(TISpaceBodyState spaceBody, TIFactionState faction)
		{
			if (faction.permanentAlly(GameStateManager.AlienFaction()))
			{
				return false;
			}
			return spaceBody.habs.Any<TIHabState>((TIHabState x) => x.faction == GameStateManager.AlienFaction());
		}

		// Token: 0x0600498C RID: 18828 RVA: 0x001EA9A4 File Offset: 0x001E8BA4
		public static float GetYearsNeededToPayForCompleteFleet(TIFactionState faction)
		{
			int num = faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.MissionControlConsumption());
			float num2 = (float)Mathf.Max(10, faction.AI_GenericMissionControlAvailable + num);
			IEnumerable<TISpaceShipTemplate> enumerable = faction.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template);
			float num3;
			if (enumerable.Any<TISpaceShipTemplate>())
			{
				num3 = (float)enumerable.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.hullTemplate.missionControl) / (float)enumerable.Count<TISpaceShipTemplate>();
			}
			else
			{
				num3 = 1.3f;
			}
			float num4 = faction.GetTypicalShipBuildCostSansRareMaterials().resourceCosts.ToDictionary<ResourceValue, ResourceValue, float>((ResourceValue x) => x, (ResourceValue x) => x.value / Mathf.Max(0f, base.<GetYearsNeededToPayForCompleteFleet>g__GetEffectiveIncome|3(x.resource))).Max<KeyValuePair<ResourceValue, float>>((KeyValuePair<ResourceValue, float> x) => x.Value);
			return num2 / num3 * num4;
		}

		// Token: 0x0600498D RID: 18829 RVA: 0x001EAAE0 File Offset: 0x001E8CE0
		public static bool NeedsSpaceBootstrap(this TIFactionState faction)
		{
			return faction.bases.Count < 4 || TIResourcesCost.basicSpaceResourcesSansFissiles.Any<FactionResource>((FactionResource x) => faction.GetDailyIncome(x, true, false) <= 0f);
		}

		// Token: 0x0600498E RID: 18830 RVA: 0x001EAB25 File Offset: 0x001E8D25
		public static bool LaggingInSpaceEconomy(this TIFactionState faction)
		{
			return AIEvaluators.GetYearsNeededToPayForCompleteFleet(faction) > 5f;
		}

		// Token: 0x0600498F RID: 18831 RVA: 0x001EAB34 File Offset: 0x001E8D34
		public static IEnumerable<TIHabModuleState> GetOrderedShipyards(TIFactionState faction)
		{
			IEnumerable<TIHabModuleState> enumerable = from x in faction.nShipyardQueues
				where !x.Key.active || x.Value.Count == 0 || x.Value.First<ShipConstructionQueueItem>().costPaid
				select x.Key;
			List<TIHabModuleState> list = faction.nShipyardQueues.Select<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>, TIHabModuleState>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Key).Except<TIHabModuleState>(enumerable).ToList<TIHabModuleState>();
			List<TIHabModuleState> list2 = new List<TIHabModuleState>();
			Func<TIHabModuleState, float> <>9__3;
			while (list.Count > 0)
			{
				IEnumerable<TIHabModuleState> enumerable2 = list;
				Func<TIHabModuleState, float> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = delegate(TIHabModuleState x)
					{
						float num3 = 90f;
						float num4 = 1f;
						if (x.hab == faction.primaryHab)
						{
							num3 += 1000000f;
						}
						if (AIEvaluators.IsSystemContested(faction, x))
						{
							num3 += 1000000f;
						}
						ShipConstructionQueueItem shipConstructionQueueItem2 = faction.nShipyardQueues[x].FirstOrDefault<ShipConstructionQueueItem>();
						if (shipConstructionQueueItem2.AIFactionGoal != null)
						{
							TIDateTime assignedDate = shipConstructionQueueItem2.AIFactionGoal.assignedDate;
							num3 += (float)(TITimeState.Now() - assignedDate).TotalDays;
							num4 *= (float)Mathf.Max(1, shipConstructionQueueItem2.AIFactionGoal.importance - 10);
							if (faction.AISavingTarget.active && faction.AISavingTarget.relatedGoal == shipConstructionQueueItem2.AIFactionGoal)
							{
								num4 *= 10f;
							}
						}
						if (shipConstructionQueueItem2.isRefit)
						{
							num4 *= 8f;
						}
						return Mathf.Pow(num3, 1.5f) * num4;
					});
				}
				TIHabModuleState tihabModuleState = enumerable2.SelectRandomWeightedItem<TIHabModuleState>(func, -1f, 1E-37f);
				list2.Add(tihabModuleState);
				list.Remove(tihabModuleState);
			}
			if (faction.lastUnaffordableShipShipyard != null && list2.Contains(faction.lastUnaffordableShipShipyard))
			{
				list2.Remove(faction.lastUnaffordableShipShipyard);
				list2.Insert(0, faction.lastUnaffordableShipShipyard);
			}
			if (faction.IsAlienFaction)
			{
				List<TIHabModuleState> list3 = list2.Where<TIHabModuleState>((TIHabModuleState x) => faction.nShipyardQueues[x].First<ShipConstructionQueueItem>().AIFactionGoal is FactionGoal_FoundBase).ToList<TIHabModuleState>();
				if (list3.Count > 0)
				{
					TIHabModuleState tihabModuleState2 = list3.MaxBy<TIHabModuleState, int>((TIHabModuleState x) => faction.nShipyardQueues[x].First<ShipConstructionQueueItem>().AIFactionGoal.importance);
					TIHabModuleState tihabModuleState3 = list2.FirstOrDefault<TIHabModuleState>();
					if (tihabModuleState2 != null && !list3.Contains(tihabModuleState3))
					{
						float num = (float)faction.bases.Count<TIHabState>((TIHabState x) => x.UnderConstructionModules().Count > 0) / (float)faction.bases.Count;
						bool flag = faction.bases.Count < 5 || num <= 0.2f;
						if (flag || num < 0.35f)
						{
							int importance = faction.nShipyardQueues[tihabModuleState2].First<ShipConstructionQueueItem>().AIFactionGoal.importance;
							int importance2 = faction.nShipyardQueues[tihabModuleState3].First<ShipConstructionQueueItem>().AIFactionGoal.importance;
							flag = flag || importance > importance2 + 1;
							if (!flag)
							{
								ShipConstructionQueueItem shipConstructionQueueItem = faction.nShipyardQueues[tihabModuleState3].First<ShipConstructionQueueItem>();
								TIResourcesCost shortfall = shipConstructionQueueItem.resourcesCost.GetShortfall(faction, shipConstructionQueueItem.shipDesign, tihabModuleState3, shipConstructionQueueItem.AIFactionGoal.importance, false);
								float num2 = 0f;
								if (shortfall.resourceCosts.Count > 0)
								{
									num2 = shortfall.resourceCosts.Max<ResourceValue>((ResourceValue x) => x.value / faction.GetDailyIncome(x.resource, false, false));
								}
								flag = num2 > (float)((2 + importance2 - importance) * 6);
							}
							if (flag)
							{
								list2.Remove(tihabModuleState2);
								list2.Insert(0, tihabModuleState2);
							}
						}
					}
				}
			}
			if (faction.AISavingTarget.active)
			{
				TIHabModuleState tihabModuleState4 = faction.nShipyardQueues.Keys.Where<TIHabModuleState>((TIHabModuleState x) => faction.nShipyardQueues[x].Count > 0).FirstOrDefault<TIHabModuleState>((TIHabModuleState x) => faction.nShipyardQueues[x].First<ShipConstructionQueueItem>().AIFactionGoal == faction.AISavingTarget.relatedGoal);
				if (tihabModuleState4 != null)
				{
					list2.Remove(tihabModuleState4);
					list2.Insert(0, tihabModuleState4);
				}
			}
			return list2.Concat<TIHabModuleState>(enumerable);
		}

		// Token: 0x06004990 RID: 18832 RVA: 0x001EAEC4 File Offset: 0x001E90C4
		public static bool MyTurf(this TIFactionState faction, TISpaceBodyState system)
		{
			if (faction.IsAlienFaction)
			{
				if (system == null || system == GameStateManager.Sol())
				{
					return false;
				}
				if (system.semiMajorAxis_AU >= GameStateManager.Jupiter().semiMajorAxis_AU)
				{
					return true;
				}
				if (system.habsInSystem.Any<TIHabState>((TIHabState x) => x.IsBase && x.faction.IsAlienFaction))
				{
					return true;
				}
				if (!system.isEarth)
				{
					if (system.habsInSystem.Any<TIHabState>((TIHabState x) => x.IsStation && x.faction.IsAlienFaction))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06004991 RID: 18833 RVA: 0x001EAF6D File Offset: 0x001E916D
		public static bool IsTrespassing(this TIFactionState aggrievedFaction, TIGameState accused)
		{
			return !(accused.ref_faction == null) && (!aggrievedFaction.permanentAlly(accused.ref_faction) && !aggrievedFaction.HasNAP(accused.ref_faction, true)) && aggrievedFaction.MyTurf(accused.GetFutureSystem());
		}

		// Token: 0x06004992 RID: 18834 RVA: 0x001EAFAC File Offset: 0x001E91AC
		public static TIObjectiveTemplate GetPrimaryHabModuleObjective(TIFactionState faction)
		{
			return (from x in faction.GetObjectives()
				where x.GetObjectiveStatus(faction) == ObjectiveStatus.Unlocked
				select x).FirstOrDefault<TIObjectiveTemplate>((TIObjectiveTemplate x) => !string.IsNullOrEmpty(x.targetHabModuleName));
		}

		// Token: 0x06004993 RID: 18835 RVA: 0x001EB008 File Offset: 0x001E9208
		public static bool DoesHabMatchObjectiveHabModuleRequirements(TIObjectiveTemplate objective, TIHabState hab, bool ignoreTier = false)
		{
			TIHabModuleTemplate targetHabModuleTemplate = AIEvaluators.cachedObjectiveHabModuleTemplate;
			if (targetHabModuleTemplate == null || AIEvaluators.cachedObjectiveHabModuleTemplateObjective != objective)
			{
				targetHabModuleTemplate = objective.targetHabModuleTemplate;
			}
			return (ignoreTier || hab.tier >= targetHabModuleTemplate.tier) && (targetHabModuleTemplate.habType == HabType.Any || hab.habType == targetHabModuleTemplate.habType) && !(hab.location != objective.targetHabLocationState);
		}

		// Token: 0x06004994 RID: 18836 RVA: 0x001EB070 File Offset: 0x001E9270
		public static bool DoesHabMatchObjectiveHabModuleRequirements(TIHabState hab, bool ignoreTier = false)
		{
			TIObjectiveTemplate primaryHabModuleObjective = AIEvaluators.GetPrimaryHabModuleObjective(hab.faction);
			return primaryHabModuleObjective != null && AIEvaluators.DoesHabMatchObjectiveHabModuleRequirements(primaryHabModuleObjective, hab, ignoreTier);
		}

		// Token: 0x06004995 RID: 18837 RVA: 0x001EB098 File Offset: 0x001E9298
		public static bool FactionNeedsNewObjectiveHab(TIFactionState faction, TIObjectiveTemplate objective)
		{
			Func<TIHabModuleState, bool> <>9__3;
			if (faction.habs.Sum<TIHabState>(delegate(TIHabState x)
			{
				IEnumerable<TIHabModuleState> enumerable = x.OkayModules();
				Func<TIHabModuleState, bool> func;
				if ((func = <>9__3) == null)
				{
					func = (<>9__3 = (TIHabModuleState x) => x.templateName == objective.targetHabModuleName);
				}
				return enumerable.Count<TIHabModuleState>(func);
			}) >= objective.targetCount)
			{
				return false;
			}
			return (from x in faction.habs
				where x.AvailableSlots().Count > 1
				where AIEvaluators.DoesHabMatchObjectiveHabModuleRequirements(objective, x, false)
				select x).Any<TIHabState>();
		}

		// Token: 0x06004996 RID: 18838 RVA: 0x001EB118 File Offset: 0x001E9318
		public static bool FactionNeedsNewObjectiveHab(TIFactionState faction)
		{
			TIObjectiveTemplate primaryHabModuleObjective = AIEvaluators.GetPrimaryHabModuleObjective(faction);
			return primaryHabModuleObjective != null && AIEvaluators.FactionNeedsNewObjectiveHab(faction, primaryHabModuleObjective);
		}

		// Token: 0x06004997 RID: 18839 RVA: 0x001EB138 File Offset: 0x001E9338
		public static bool FactionIsWorkingOnHabModuleBasedObjectives(TIFactionState faction)
		{
			return AIEvaluators.GetPrimaryHabModuleObjective(faction) != null;
		}

		// Token: 0x06004998 RID: 18840 RVA: 0x001EB144 File Offset: 0x001E9344
		public static float GetEstimatedTransferTime_days(TIFactionState faction, TIOrbitState originOrbit, TIGameState destination, float acceleration_mps2, float deltaV_mps, float failureTransferTime_days = float.PositiveInfinity)
		{
			TIVirtualSpaceFleet tivirtualSpaceFleet = new TIVirtualSpaceFleet(originOrbit, acceleration_mps2, deltaV_mps, faction, null, 0.0);
			IEnumerable<Trajectory> trajectories = Enumerable.Empty<Trajectory>();
			try
			{
				double num;
				MasterTransferPlanner.RequestTrajectories(tivirtualSpaceFleet, destination, 64, delegate(Trajectory[] t)
				{
					trajectories = t.ToList<Trajectory>();
				}, out num, false, false, 0.2);
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message + "\n" + ex.StackTrace, Array.Empty<object>());
			}
			if (!trajectories.Any<Trajectory>())
			{
				return failureTransferTime_days;
			}
			return (float)trajectories.Min<Trajectory>((Trajectory y) => y.duration_d) * 0.8f;
		}

		// Token: 0x06004999 RID: 18841 RVA: 0x001EB210 File Offset: 0x001E9410
		public static float GetEstimatedTransferTime_days(TIFactionState faction, TISpaceBodyState origin, TISpaceBodyState destination, float acceleration_mps2, float deltaV_mps, float failureTransferTime_days = float.PositiveInfinity)
		{
			return AIEvaluators.GetEstimatedTransferTime_days(faction, origin.orbits.First<TIOrbitState>(), destination.orbits.First<TIOrbitState>(), acceleration_mps2, deltaV_mps, failureTransferTime_days);
		}

		// Token: 0x0600499A RID: 18842 RVA: 0x001EB234 File Offset: 0x001E9434
		public static float PrimarySystemDangerLevel(TIFactionState faction, out float threatStrength)
		{
			threatStrength = 0f;
			if (!faction.IsAlienFaction || !TIGameState.Valid(faction.primaryHab))
			{
				return 0f;
			}
			TISpaceBodyState ref_system = faction.primaryHab.ref_system;
			if (ref_system == null)
			{
				return 0f;
			}
			if (!AIEvaluators.ShouldSystemBeInDefenseMode(faction, ref_system))
			{
				return 0f;
			}
			threatStrength = AIEvaluators.GetThreatLevelAtLocation(faction, ref_system, false);
			float presentFleetStrengthInSystem = AIEvaluators.GetPresentFleetStrengthInSystem(faction, ref_system);
			return threatStrength / presentFleetStrengthInSystem;
		}

		// Token: 0x0600499B RID: 18843 RVA: 0x001EB2A4 File Offset: 0x001E94A4
		public static bool IsPrimarySystemInPeril(TIFactionState faction)
		{
			if (!faction.IsAlienFaction)
			{
				return false;
			}
			float num;
			if ((double)AIEvaluators.PrimarySystemDangerLevel(faction, out num) < 0.15)
			{
				return false;
			}
			TISpaceBodyState primarySystem = faction.primaryHab.ref_system;
			return faction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.inTransfer).Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				TISpaceGameState destination = x.trajectory.destination;
				return ((destination != null) ? destination.ref_system : null) == primarySystem;
			}).Concat<TISpaceFleetState>(faction.fleets.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				FactionGoal_SendFleet factionGoal_SendFleet = x.AssignedGoal() as FactionGoal_SendFleet;
				return factionGoal_SendFleet != null && factionGoal_SendFleet.target().ref_system == primarySystem;
			}))
				.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) / num < 2f;
		}

		// Token: 0x0600499C RID: 18844 RVA: 0x001EB370 File Offset: 0x001E9570
		public static IEnumerable<TISpaceFleetState> GetEnemyFleetsInSystemOrSoonToArrive(TIFactionState faction, TISpaceBodyState system, float soonCutoff_days)
		{
			IEnumerable<TISpaceFleetState> enumerable = (from x in GameStateManager.AllFactions()
				where !faction.permanentAlly(x)
				select x).SelectMany<TIFactionState, TISpaceFleetState>((TIFactionState x) => x.fleets);
			IEnumerable<TISpaceFleetState> enumerable2 = enumerable.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ref_system == system);
			TIDateTime now = TITimeState.Now();
			IEnumerable<TISpaceFleetState> enumerable3 = from x in enumerable.Where<TISpaceFleetState>((TISpaceFleetState x) => x.inTransfer).Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					TISpaceGameState destination = x.trajectory.destination;
					return ((destination != null) ? destination.ref_system : null) == system;
				})
				where (x.trajectory.arrivalTime - now).TotalDays < (double)soonCutoff_days
				select x;
			return enumerable2.Concat<TISpaceFleetState>(enumerable3);
		}

		// Token: 0x0600499D RID: 18845 RVA: 0x001EB43E File Offset: 0x001E963E
		public static bool AreEnemyFleetsInSystemOrSoonToArrive(TIFactionState faction, TISpaceBodyState system, float soonCutoff_days)
		{
			return AIEvaluators.GetEnemyFleetsInSystemOrSoonToArrive(faction, system, soonCutoff_days).Any<TISpaceFleetState>();
		}

		// Token: 0x0600499E RID: 18846 RVA: 0x001EB44D File Offset: 0x001E964D
		public static bool IsPrimarySystemCampedOrSoonToBe(TIFactionState faction)
		{
			return !(faction.primarySystem == null) && faction.IsAlienFaction && AIEvaluators.AreEnemyFleetsInSystemOrSoonToArrive(faction, faction.primarySystem, AIEvaluators.PrimarySystemCampedSoonCutoff_days);
		}

		// Token: 0x0600499F RID: 18847 RVA: 0x001EB478 File Offset: 0x001E9678
		public static bool ShouldRescuePrimarySystem(TIFactionState faction)
		{
			return AIEvaluators.IsPrimarySystemCampedOrSoonToBe(faction) && AIEvaluators.IsPrimarySystemInPeril(faction);
		}

		// Token: 0x060049A0 RID: 18848 RVA: 0x001EB48C File Offset: 0x001E968C
		public static float GetAdjustedFleetSuperiorityFactor(TIFactionState faction)
		{
			float num = 1f + (faction.AI_ModifiedRiskAversion() - 0.5f) / 4f;
			float num2 = 0.3f;
			if (faction.IsAlienFaction)
			{
				num2 = 0.6f;
			}
			return (0.39999998f * num2 + 1f) * num;
		}

		// Token: 0x17000E8A RID: 3722
		// (get) Token: 0x060049A1 RID: 18849 RVA: 0x001EB4D8 File Offset: 0x001E96D8
		public static Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>> SystemFleetStrengths
		{
			get
			{
				float num = -1f;
				if (AIEvaluators.systemFleetStrengthsCachedDate != null)
				{
					num = (float)(TITimeState.Now() - AIEvaluators.systemFleetStrengthsCachedDate).TotalDays;
				}
				if (num < 0f || num > 1f)
				{
					AIEvaluators.cachedSystemFleetStrengths.Clear();
					AIEvaluators.systemFleetStrengthsCachedDate = TITimeState.Now();
					foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
					{
						foreach (TISpaceFleetState tispaceFleetState in tifactionState.fleets)
						{
							AIEvaluators.<get_SystemFleetStrengths>g__AddStrength|169_0(tifactionState, tispaceFleetState, tispaceFleetState.SpaceCombatValue());
						}
						foreach (KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> keyValuePair in tifactionState.nShipyardQueues)
						{
							float num2 = keyValuePair.Value.Where<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => x.isRefit).Sum<ShipConstructionQueueItem>((ShipConstructionQueueItem x) => x.refit_originalShipDesign.TemplateSpaceCombatValue(false, -1f, 1f, false));
							AIEvaluators.<get_SystemFleetStrengths>g__AddStrength|169_0(tifactionState, keyValuePair.Key, num2);
						}
					}
				}
				return AIEvaluators.cachedSystemFleetStrengths;
			}
		}

		// Token: 0x060049A2 RID: 18850 RVA: 0x001EB650 File Offset: 0x001E9850
		public static float GetFleetStrengthInSystem(TIFactionState faction, TISpaceObjectState system)
		{
			Dictionary<TIFactionState, float> dictionary;
			float num;
			if (AIEvaluators.SystemFleetStrengths.TryGetValue(system, out dictionary) && dictionary.TryGetValue(faction, out num))
			{
				return num;
			}
			return 0f;
		}

		// Token: 0x060049A3 RID: 18851 RVA: 0x001EB680 File Offset: 0x001E9880
		public static float GetPresentFleetStrengthInSystem(TIFactionState faction, TISpaceObjectState system)
		{
			return faction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ref_system == system).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
		}

		// Token: 0x060049A4 RID: 18852 RVA: 0x001EB6D8 File Offset: 0x001E98D8
		public static float GetThreatLevelAtLocation(TIFactionState faction, TIGameState location, bool warEnemiesOnly)
		{
			AIEvaluators.<>c__DisplayClass176_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass176_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.warEnemiesOnly = warEnemiesOnly;
			if (location.isSpaceFleetState && location.ref_fleet.inTransfer && location.ref_fleet.trajectory.destination != null)
			{
				return AIEvaluators.GetThreatLevelAtLocation(CS$<>8__locals1.faction, location.ref_fleet.trajectory.destination.ref_naturalSpaceObject, CS$<>8__locals1.warEnemiesOnly);
			}
			Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>> dictionary = AIEvaluators.cachedThreatLevels_all;
			TIDateTime tidateTime = AIEvaluators.threatLevelCachedDate_all;
			if (CS$<>8__locals1.warEnemiesOnly)
			{
				dictionary = AIEvaluators.cachedThreatLevels_warEnemiesOnly;
				tidateTime = AIEvaluators.threatLevelCachedDate_warEnemiesOnly;
			}
			float num = -1f;
			if (tidateTime != null)
			{
				num = (float)(TITimeState.Now() - tidateTime).TotalDays;
			}
			if (num < 0f || num > 1f)
			{
				dictionary.Clear();
				if (CS$<>8__locals1.warEnemiesOnly)
				{
					AIEvaluators.threatLevelCachedDate_warEnemiesOnly = TITimeState.Now();
				}
				else
				{
					AIEvaluators.threatLevelCachedDate_all = TITimeState.Now();
				}
			}
			CS$<>8__locals1.system = location.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
			if (CS$<>8__locals1.system == null)
			{
				return 0f;
			}
			if (!dictionary.ContainsKey(CS$<>8__locals1.system))
			{
				dictionary[CS$<>8__locals1.system] = new Dictionary<TIFactionState, float>();
			}
			if (!dictionary[CS$<>8__locals1.system].ContainsKey(CS$<>8__locals1.faction))
			{
				IEnumerable<TIFactionState> attackableFactions = AIEvaluators.GetAttackableFactions(CS$<>8__locals1.faction);
				float num2 = 1f * attackableFactions.Max<TIFactionState>(new Func<TIFactionState, float>(CS$<>8__locals1.<GetThreatLevelAtLocation>g__GetAdjustedThreatLevelOfFaction|1));
				dictionary[CS$<>8__locals1.system][CS$<>8__locals1.faction] = num2;
			}
			return dictionary[CS$<>8__locals1.system][CS$<>8__locals1.faction];
		}

		// Token: 0x060049A5 RID: 18853 RVA: 0x001EB87C File Offset: 0x001E9A7C
		public static IEnumerable<TIFactionState> GetAttackableFactions(TIFactionState faction)
		{
			return from x in GameStateManager.AllFactions()
				where !faction.permanentAlly(x)
				select x;
		}

		// Token: 0x060049A6 RID: 18854 RVA: 0x001EB8AC File Offset: 0x001E9AAC
		public static bool IsSystemContested(TIFactionState faction, TIGameState location)
		{
			TISpaceBodyState tispaceBodyState = ((location != null) ? location.ref_system : null);
			if (tispaceBodyState == null)
			{
				return false;
			}
			float num = (from x in tispaceBodyState.fleetsInSystem
				where !x.landed
				where x.faction == faction
				select x).Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
			float num2 = 0.3f;
			TIHabState primaryHab = faction.primaryHab;
			if (((primaryHab != null) ? primaryHab.ref_system : null) == tispaceBodyState)
			{
				num2 = 0.15f;
			}
			return AIEvaluators.GetThreatLevelAtLocation(faction, location, true) > num2 * num;
		}

		// Token: 0x060049A7 RID: 18855 RVA: 0x001EB97B File Offset: 0x001E9B7B
		public static float GetRiskAdjustedThreatLevelAtLocation(TIFactionState faction, TIGameState location, bool warEnemiesOnly)
		{
			return AIEvaluators.GetThreatLevelAtLocation(faction, location, warEnemiesOnly);
		}

		// Token: 0x060049A8 RID: 18856 RVA: 0x001EB988 File Offset: 0x001E9B88
		public static float GetRequiredDefenseStrength(TIFactionState defender, TIFactionState attacker, float attackStrength, TIHabState defendingHab = null)
		{
			float num = defender.AI_ModifiedRiskAversion();
			float num2 = 1.2f;
			if (defendingHab != null)
			{
				num2 *= (float)(defendingHab.tier - 1) / 10f + 1f;
				if (defendingHab == defender.primaryHab)
				{
					num2 *= 2f;
				}
			}
			float num3;
			if (defender.enemyWarFactions.Contains(attacker))
			{
				num3 = num2;
			}
			else
			{
				num3 = AIEvaluators.FactionsGoToWarProgress(defender, attacker) * (num2 - num) + num;
			}
			return attackStrength * num3;
		}

		// Token: 0x060049A9 RID: 18857 RVA: 0x001EB9FC File Offset: 0x001E9BFC
		public static bool IsDefenseFeasible(TIFactionState defender, TIGameState gameState, float attackStrength)
		{
			if (defender.IsAlienFaction)
			{
				return true;
			}
			TIHabState tihabState = (from x in defender.stations
				where x.ref_system == gameState.ref_system
				orderby x.CompletedShipyards().Count > 0 descending, x.SpaceCombatValue() descending, x.CompletedShipyards().Sum<TIHabModuleState>((TIHabModuleState y) => Mathf.Pow((float)y.tier, 2f)) descending
				select x).FirstOrDefault<TIHabState>();
			return gameState.isHabState && gameState == tihabState;
		}

		// Token: 0x060049AA RID: 18858 RVA: 0x001EBAC8 File Offset: 0x001E9CC8
		public static bool IsSafeForColonization(this TIGameState gameState, TIFactionState faction, HabType habType = HabType.Any)
		{
			TISpaceBodyState tispaceBodyState = ((gameState != null) ? gameState.ref_system : null);
			if (tispaceBodyState == null)
			{
				return true;
			}
			float systemDeadliness = tispaceBodyState.GetSystemDeadliness(faction, habType);
			float num = 1f;
			if ((faction.IsAlienFaction || !tispaceBodyState.isEarth) && !faction.CanFoundHabFromHabAtLocation(tispaceBodyState, false, false) && (faction.IsAlienFaction || tispaceBodyState.habSitesInSystem.Count < 4))
			{
				num = 0.001f;
			}
			if (systemDeadliness >= num)
			{
				return false;
			}
			if (faction.IsAlienFaction)
			{
				if (tispaceBodyState.fleetsInSystem.Where<TISpaceFleetState>((TISpaceFleetState x) => x.CombatFleet()).Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(faction)))
				{
					return false;
				}
				Dictionary<TIFactionState, float> dictionary;
				if (AIEvaluators.SystemFleetStrengths.TryGetValue(tispaceBodyState, out dictionary))
				{
					if (dictionary.Where<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => !x.Key.permanentAlly(faction)).Any<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Value > 0f))
					{
						return false;
					}
				}
				return true;
			}
			else
			{
				if (faction.IsAlienProxy)
				{
					return true;
				}
				if (faction.isAlienAppeaser)
				{
					return true;
				}
				if (tispaceBodyState.habsInSystem.Any<TIHabState>((TIHabState y) => y.faction.IsAlienFaction))
				{
					return false;
				}
				double jupiterAU = GameStateManager.Jupiter().semiMajorAxis_AU;
				if (tispaceBodyState.ref_system.semiMajorAxis_AU < jupiterAU)
				{
					return true;
				}
				List<TIFactionState> list = tispaceBodyState.habsInSystem.Select<TIHabState, TIFactionState>((TIHabState x) => x.faction).Distinct<TIFactionState>().ToList<TIFactionState>();
				if (list.Any<TIFactionState>((TIFactionState x) => x == faction))
				{
					return true;
				}
				if (list.Any<TIFactionState>((TIFactionState x) => faction.enemyWarFactions.Contains(x)))
				{
					return false;
				}
				if (list.Any<TIFactionState>())
				{
					return true;
				}
				float num2 = GameStateManager.AlienFaction().fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
				if (GameStateManager.AllHumanFactions().Sum<TIFactionState>((TIFactionState x) => x.fleets.Sum<TISpaceFleetState>((TISpaceFleetState y) => y.SpaceCombatValue())) > num2 * 0.5f)
				{
					return true;
				}
				TIFactionState strongestHumanFaction = AIEvaluators.GetStrongestHumanFaction((TIFactionState x) => !x.veryProAlien);
				if (faction == strongestHumanFaction && !GameStateManager.Planets().Contains(tispaceBodyState))
				{
					return true;
				}
				return (from x in (from x in GameStateManager.AllSpaceBodies()
						where x.semiMajorAxis_AU >= jupiterAU
						select x.ref_system).Distinct<TISpaceBodyState>().SelectMany<TISpaceBodyState, TIFactionState>((TISpaceBodyState x) => x.habsInSystem.Select<TIHabState, TIFactionState>((TIHabState x) => x.faction)).Distinct<TIFactionState>()
					where !x.IsAlienFaction
					select x).Any<TIFactionState>((TIFactionState x) => !x.veryProAlien);
			}
		}

		// Token: 0x060049AB RID: 18859 RVA: 0x001EBE3A File Offset: 0x001EA03A
		public static bool IsSafeForColonization(this TIOrbitState orbit, TIFactionState faction)
		{
			return orbit.ref_system.IsSafeForColonization(faction, HabType.Station);
		}

		// Token: 0x060049AC RID: 18860 RVA: 0x001EBE49 File Offset: 0x001EA049
		public static bool IsSafeForColonization(this TIHabSiteState habSite, TIFactionState faction)
		{
			return habSite.ref_system.IsSafeForColonization(faction, HabType.Base);
		}

		// Token: 0x060049AD RID: 18861 RVA: 0x001EBE58 File Offset: 0x001EA058
		public static float GetSystemDeadliness(this TISpaceBodyState system, TIFactionState faction, HabType habType = HabType.Any)
		{
			float num = (from x in faction.GetHabDestructions(system, habType)
				select x.Date).Sum<TIDateTime>(delegate(TIDateTime date)
			{
				float num3 = (float)((TITimeState.Now() - date).TotalDays / 30.436874389648438);
				if (num3 > 60f)
				{
					return 0f;
				}
				return Mathf.Pow(0.5f, num3 / 12f);
			});
			int num2 = system.habsInSystem.Count<TIHabState>((TIHabState x) => x.faction == faction);
			return num / (float)(num2 + 1);
		}

		// Token: 0x060049AE RID: 18862 RVA: 0x001EBEE4 File Offset: 0x001EA0E4
		public static float GetSystemDeadlinessScoreModifier(this TISpaceBodyState system, TIFactionState faction)
		{
			if (system == null)
			{
				return 0f;
			}
			return 1f - Mathf.Clamp(system.GetSystemDeadliness(faction, HabType.Any), 0f, 1f);
		}

		// Token: 0x060049AF RID: 18863 RVA: 0x001EBF14 File Offset: 0x001EA114
		public static bool ShouldSystemBeInDefenseMode(TIFactionState faction, TISpaceBodyState system)
		{
			return !(system == null) && !system.isSun && !system.isEarth && !(faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Where<TIFactionGoalState>(delegate(TIFactionGoalState x)
			{
				TIGameState tigameState = x.target();
				return ((tigameState != null) ? tigameState.ref_system : null) == system;
			}).FirstOrDefault<TIFactionGoalState>() == null) && !system.habsInSystem.Where<TIHabState>((TIHabState x) => !faction.permanentAlly(x.faction)).Any<TIHabState>() && ((from x in Enumerable.Empty<TISpaceGameState>().Union<TISpaceGameState>(system.fleetsInSystem).Union<TISpaceGameState>(system.habsInSystem)
				where faction.enemyWarFactions.Contains(x.ref_faction)
				select x).Any<TISpaceGameState>() || AIEvaluators.IsSystemContested(faction, system));
		}

		// Token: 0x060049B0 RID: 18864 RVA: 0x001EC003 File Offset: 0x001EA203
		public static float GetMinimumSuperiorityForSpontaniousAttack(this TIFactionState faction)
		{
			return 1.125f + (faction.AI_ModifiedRiskAversion() - 0.5f) / 4f;
		}

		// Token: 0x060049B1 RID: 18865 RVA: 0x001EC01D File Offset: 0x001EA21D
		public static float GetDesiredSuperiorityForSpontaniousAttack(this TIFactionState faction)
		{
			return faction.GetMinimumSuperiorityForSpontaniousAttack() * 1.25f;
		}

		// Token: 0x060049B2 RID: 18866 RVA: 0x001EC02C File Offset: 0x001EA22C
		public static TIHabState SelectHabToAttack(TIFactionState attackingFaction, IEnumerable<TIHabState> enemyHabs)
		{
			TIHabState tihabState = AIEvaluators.SelectStationToAttack(attackingFaction, enemyHabs, -1f);
			TIHabState tihabState2 = AIEvaluators.SelectBaseToAttack(attackingFaction, enemyHabs);
			if (tihabState == null && tihabState2 == null)
			{
				return null;
			}
			return (from x in Enumerable.Empty<TIHabState>().Append(tihabState).Append(tihabState2)
				where x != null
				select x).SelectRandomWeightedItem<TIHabState>((TIHabState x) => (x.IsBase ? 1.3f : 1f) * (float)x.mass_kg, -1f, 1E-37f);
		}

		// Token: 0x060049B3 RID: 18867 RVA: 0x001EC0C8 File Offset: 0x001EA2C8
		private static IEnumerable<TIHabState> GetCriticalConstructionHabs(IEnumerable<TIHabState> habs, out IEnumerable<TIHabState> constructionHabs)
		{
			List<TIHabState> list = new List<TIHabState>();
			List<TIHabState> list2 = new List<TIHabState>();
			using (List<TIFactionState>.Enumerator enumerator = habs.Select<TIHabState, TIFactionState>((TIHabState x) => x.faction).Distinct<TIFactionState>().ToList<TIFactionState>()
				.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIFactionState faction = enumerator.Current;
					IEnumerable<TIHabState> enumerable = habs.Where<TIHabState>((TIHabState x) => x.faction == faction);
					Func<TIHabState, bool> <>9__9;
					Dictionary<TISpaceBodyState, int> constructionModulesPerSystem = (from x in enumerable
						select x.ref_system into x
						where x != null
						select x).Distinct<TISpaceBodyState>().ToDictionary<TISpaceBodyState, TISpaceBodyState, int>((TISpaceBodyState x) => x, delegate(TISpaceBodyState x)
					{
						IEnumerable<TIHabState> habsInSystem = x.habsInSystem;
						Func<TIHabState, bool> func;
						if ((func = <>9__9) == null)
						{
							func = (<>9__9 = (TIHabState y) => y.faction == faction);
						}
						return habsInSystem.Where<TIHabState>(func).SelectMany<TIHabState, TIHabModuleState>((TIHabState y) => y.CompletedModules()).Count<TIHabModuleState>((TIHabModuleState y) => y.moduleTemplate.EnablesLocalFounding);
					});
					IEnumerable<TIHabState> enumerable2 = enumerable.Where<TIHabState>((TIHabState x) => x.OkayModules().Any<TIHabModuleState>((TIHabModuleState y) => y.moduleTemplate.EnablesLocalFounding));
					list.AddRange(enumerable2);
					list2.AddRange(from x in enumerable2
						where x.ref_system != null
						where constructionModulesPerSystem[x.ref_system] <= 1
						select x);
				}
			}
			constructionHabs = list;
			return list2;
		}

		// Token: 0x060049B4 RID: 18868 RVA: 0x001EC27C File Offset: 0x001EA47C
		public static TIHabState SelectStationToAttack(TIFactionState attackingFaction, IEnumerable<TIHabState> enemyStations, float expectedAttackStrength = -1f)
		{
			enemyStations = enemyStations.Where<TIHabState>((TIHabState x) => x.IsStation);
			if (enemyStations == null || enemyStations.Count<TIHabState>() == 0)
			{
				return null;
			}
			float minimumSuperiority = attackingFaction.GetMinimumSuperiorityForSpontaniousAttack();
			float desiredSuperiority = attackingFaction.GetDesiredSuperiorityForSpontaniousAttack();
			Dictionary<TIHabState, float> habMasses = enemyStations.ToDictionary<TIHabState, TIHabState, float>((TIHabState x) => x, (TIHabState x) => x.OkayModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.baseMass_tons) + 1f);
			float highestMass = habMasses.Values.Max();
			if (expectedAttackStrength > 0f)
			{
				Dictionary<TIHabState, float> strengthRatios = enemyStations.ToDictionary<TIHabState, TIHabState, float>((TIHabState x) => x, (TIHabState x) => expectedAttackStrength / x.PerceivedAggregateDefensiveScore_Station(attackingFaction));
				enemyStations = enemyStations.Where<TIHabState>((TIHabState x) => strengthRatios[x] >= minimumSuperiority);
				List<TIHabState> list = (from x in enemyStations
					where strengthRatios[x] >= desiredSuperiority
					where habMasses[x] > highestMass / 2f
					select x).ToList<TIHabState>();
				if (list.Count > 0)
				{
					enemyStations = list;
				}
			}
			IEnumerable<TIHabState> constructionStations;
			IEnumerable<TIHabState> criticalConstructionHabs = AIEvaluators.GetCriticalConstructionHabs(enemyStations, out constructionStations);
			if (criticalConstructionHabs.Any<TIHabState>())
			{
				enemyStations = criticalConstructionHabs;
			}
			return enemyStations.SelectRandomWeightedItem<TIHabState>((TIHabState x) => Mathf.Pow(habMasses[x], 1.5f) * (float)(constructionStations.Contains(x) ? 2 : 1), -1f, 1E-37f);
		}

		// Token: 0x060049B5 RID: 18869 RVA: 0x001EC430 File Offset: 0x001EA630
		public static TIHabState SelectBaseToAttack(TIFactionState attackingFaction, IEnumerable<TIHabState> enemyBases)
		{
			enemyBases = enemyBases.Where<TIHabState>((TIHabState x) => x.IsBase);
			if (!enemyBases.Any<TIHabState>())
			{
				return null;
			}
			IEnumerable<TIHabState> constructionBases;
			IEnumerable<TIHabState> criticalConstructionHabs = AIEvaluators.GetCriticalConstructionHabs(enemyBases, out constructionBases);
			if (criticalConstructionHabs.Any<TIHabState>())
			{
				enemyBases = criticalConstructionHabs;
			}
			float averageMass = enemyBases.Average<TIHabState>((TIHabState x) => (float)x.mass_kg);
			IEnumerable<TIHabState> enumerable = enemyBases.Where<TIHabState>((TIHabState x) => x.mass_kg >= (double)(averageMass * 0.6f));
			IEnumerable<TIHabState> enumerable2 = enumerable.Where<TIHabState>((TIHabState x) => x.SpaceCombatValue() == 0f);
			IEnumerable<TIHabState> enumerable3 = from x in enumerable
				where x.mass_kg > (double)(averageMass * 1f)
				where x.SpaceCombatValue() == 0f
				select x;
			if (enumerable3.Any<TIHabState>())
			{
				enemyBases = enumerable3;
			}
			else if (enumerable2.Any<TIHabState>())
			{
				enemyBases = enumerable2;
			}
			else
			{
				enemyBases = enumerable;
			}
			return enemyBases.SelectRandomWeightedItem<TIHabState>((TIHabState x) => Mathf.Pow((float)x.mass_kg, 1.5f) * (float)(constructionBases.Contains(x) ? 2 : 1), -1f, 1E-37f);
		}

		// Token: 0x060049B6 RID: 18870 RVA: 0x001EC564 File Offset: 0x001EA764
		public static TIHabState SelectHabToCapture(TIFactionState capturingFaction, TIFactionState targetFaction, IEnumerable<TIHabState> candidates = null, AIEvaluators.HabCapturingLogic capturingLogic = AIEvaluators.HabCapturingLogic.All, bool ignoreMissionControl = false)
		{
			if (capturingFaction.IsAlienFaction)
			{
				return null;
			}
			if (!ignoreMissionControl && capturingFaction.AvailableMissionControl <= 5)
			{
				return null;
			}
			if (candidates == null)
			{
				candidates = targetFaction.habs;
			}
			candidates = candidates.Where<TIHabState>((TIHabState x) => !capturingFaction.permanentAlly(x.faction));
			if (capturingLogic == AIEvaluators.HabCapturingLogic.LowEffortHighReward)
			{
				Func<TIHabState, bool> <>9__9;
				candidates = candidates.Where<TIHabState>(delegate(TIHabState x)
				{
					TISpaceBodyState ref_system = x.ref_system;
					return ((ref_system != null) ? ref_system.habSites.Length : 0) >= 4;
				}).Where<TIHabState>(delegate(TIHabState x)
				{
					IEnumerable<TIHabState> habsInSystem = x.ref_system.habsInSystem;
					Func<TIHabState, bool> func;
					if ((func = <>9__9) == null)
					{
						func = (<>9__9 = (TIHabState x) => x.faction == capturingFaction);
					}
					return habsInSystem.Any<TIHabState>(func);
				}).ToList<TIHabState>();
				IEnumerable<TISpaceBodyState> shipyardSystems = (from x in capturingFaction.nShipyardQueues
					select x.Key.hab.ref_system into x
					where x != null
					select x).Distinct<TISpaceBodyState>();
				List<TIHabState> list = candidates.Where<TIHabState>((TIHabState x) => shipyardSystems.Contains(x.ref_system)).ToList<TIHabState>();
				if (list.Any<TIHabState>())
				{
					candidates = list;
				}
			}
			List<ValueTuple<TIHabState, float, float>> list2 = (from x in candidates
				select new ValueTuple<TIHabState, float, float>(x, x.AssaultCombatValue(true), TradeAI.GetTradeValue(x, capturingFaction)) into x
				where x.Item3 > 0f
				select x).ToList<ValueTuple<TIHabState, float, float>>();
			if (list2.Count == 0)
			{
				return null;
			}
			return list2.SelectRandomWeightedItem<ValueTuple<TIHabState, float, float>>(([TupleElementNames(new string[] { "Hab", "AssaultValue", "Score" })] ValueTuple<TIHabState, float, float> x) => (float)(x.Item1.IsBase ? 10 : 1) * Mathf.Pow(x.Item3, 2f) / Mathf.Pow(x.Item2 + 0.1f, 2.5f), -1f, 1E-37f).Item1;
		}

		// Token: 0x060049B7 RID: 18871 RVA: 0x001EC708 File Offset: 0x001EA908
		public static TIRegionSpaceFacilityState SelectSpaceFacilityToAttack(TIFactionState attackingFaction, TIFactionState targetFaction)
		{
			List<TIRegionSpaceFacilityState> list = targetFaction.nationsWithMyControlPoints.SelectMany<TINationState, TIRegionSpaceFacilityState>((TINationState x) => x.spaceProgramSites).ToList<TIRegionSpaceFacilityState>();
			Func<TIControlPoint, bool> <>9__4;
			list.RemoveAll(delegate(TIRegionSpaceFacilityState x)
			{
				IEnumerable<TIControlPoint> controlPoints = x.ref_nation.controlPoints;
				Func<TIControlPoint, bool> func;
				if ((func = <>9__4) == null)
				{
					func = (<>9__4 = delegate(TIControlPoint x)
					{
						TIFactionState faction = x.faction;
						return faction != null && faction.permanentAlly(attackingFaction);
					});
				}
				return controlPoints.Any<TIControlPoint>(func);
			});
			if (list.Count > 0)
			{
				return list.SelectRandomWeightedItem<TIRegionSpaceFacilityState>((TIRegionSpaceFacilityState x) => base.<SelectSpaceFacilityToAttack>g__ScoreTarget|2(x), -1f, 1E-37f);
			}
			return null;
		}

		// Token: 0x060049B8 RID: 18872 RVA: 0x001EC798 File Offset: 0x001EA998
		public static TISpaceFleetState SelectFleetToAttack(TIFactionState attackingFaction, IEnumerable<TISpaceFleetState> enemyFleets, float expectedAttackStrength = -1f)
		{
			AIEvaluators.<>c__DisplayClass197_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass197_0();
			CS$<>8__locals1.attackingFaction = attackingFaction;
			CS$<>8__locals1.expectedAttackStrength = expectedAttackStrength;
			if (!enemyFleets.Any<TISpaceFleetState>())
			{
				return null;
			}
			CS$<>8__locals1.minimumSuperiority = CS$<>8__locals1.attackingFaction.GetMinimumSuperiorityForSpontaniousAttack();
			CS$<>8__locals1.desiredSuperiority = CS$<>8__locals1.attackingFaction.GetDesiredSuperiorityForSpontaniousAttack();
			CS$<>8__locals1.minimumStrength = enemyFleets.Average<TISpaceFleetState>((TISpaceFleetState x) => CS$<>8__locals1.attackingFaction.GetPerceivedEnemyFleetStrength(x)) * 0.6f;
			List<TISpaceFleetState> list = enemyFleets.Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(CS$<>8__locals1.<SelectFleetToAttack>g__PassesMinimums|1)).ToList<TISpaceFleetState>();
			IEnumerable<TISpaceFleetState> enumerable = list.Where<TISpaceFleetState>((TISpaceFleetState x) => x.bombarding && x.bombardmentTarget.ref_faction == CS$<>8__locals1.attackingFaction);
			if (enumerable.Count<TISpaceFleetState>() > 0)
			{
				list = enumerable.ToList<TISpaceFleetState>();
			}
			bool flag = false;
			if (CS$<>8__locals1.expectedAttackStrength > 0f)
			{
				List<TISpaceFleetState> list2 = list.Where<TISpaceFleetState>((TISpaceFleetState x) => CS$<>8__locals1.expectedAttackStrength / CS$<>8__locals1.attackingFaction.GetPerceivedEnemyFleetStrength(x) >= CS$<>8__locals1.desiredSuperiority).ToList<TISpaceFleetState>();
				if (list2.Count > 0)
				{
					list = list2;
					flag = true;
				}
			}
			if (!flag && list.Count > 1)
			{
				list.Remove(list.MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => CS$<>8__locals1.attackingFaction.GetPerceivedEnemyFleetStrength(x)));
			}
			return list.SelectRandomWeightedItem<TISpaceFleetState>((TISpaceFleetState x) => Mathf.Pow(x.SpaceCombatValue(), 2f), -1f, 1E-37f);
		}

		// Token: 0x060049B9 RID: 18873 RVA: 0x001EC8CC File Offset: 0x001EAACC
		[return: TupleElementNames(new string[] { "Predator", "Prey" })]
		public static IEnumerable<ValueTuple<TISpaceFleetState, TISpaceFleetState>> GenerateQuickAttacks(TIFactionState faction, IEnumerable<TISpaceFleetState> enemyFleets, int attackCount, Func<TISpaceFleetState, bool> MayUseFleetForAttack = null)
		{
			AIEvaluators.<>c__DisplayClass198_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass198_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.attackCount = attackCount;
			CS$<>8__locals1.MayUseFleetForAttack = MayUseFleetForAttack;
			if (CS$<>8__locals1.MayUseFleetForAttack == null)
			{
				CS$<>8__locals1.MayUseFleetForAttack = (TISpaceFleetState fleet) => !(fleet.faction != CS$<>8__locals1.faction) && !fleet.inTransfer && (fleet.AssignedGoal() == null || fleet.AssignedGoal() is FactionGoal_AssembleFleet || fleet.AssignedGoal() is FactionGoal_AttackWithFleet || fleet.AssignedGoal() is FactionGoal_DefendWithFleet || fleet.AssignedGoal() is FactionGoal_RefitFleet);
			}
			CS$<>8__locals1.attacks = new HashSet<ValueTuple<TISpaceFleetState, TISpaceFleetState>>();
			List<ValueTuple<TISpaceFleetState, TISpaceFleetState>> list = (from x in (from x in CS$<>8__locals1.faction.fleets
					where x.ref_system != null
					group x by x.ref_system).SelectMany<IGrouping<TISpaceBodyState, TISpaceFleetState>, TISpaceFleetState>((IGrouping<TISpaceBodyState, TISpaceFleetState> x) => x.Key.fleetsInSystem)
				where x.faction.IsAlienFaction
				select x).Select<TISpaceFleetState, ValueTuple<TISpaceFleetState, TISpaceFleetState>>(delegate(TISpaceFleetState prey)
			{
				float desiredStrength = FactionGoal_AttackWithFleet.ComputeDesiredFleetCombatValueForAttack(CS$<>8__locals1.faction, prey, false, false);
				IEnumerable<TISpaceFleetState> enumerable = prey.ref_system.fleetsInSystem.Where<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue() >= desiredStrength);
				Func<TISpaceFleetState, bool> func;
				if ((func = CS$<>8__locals1.<>9__26) == null)
				{
					func = (CS$<>8__locals1.<>9__26 = (TISpaceFleetState x) => CS$<>8__locals1.MayUseFleetForAttack(x));
				}
				return new ValueTuple<TISpaceFleetState, TISpaceFleetState>(enumerable.Where<TISpaceFleetState>(func).ToList<TISpaceFleetState>().MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue()), prey);
			}).ToList<ValueTuple<TISpaceFleetState, TISpaceFleetState>>();
			if (CS$<>8__locals1.<GenerateQuickAttacks>g__TryFillAttacks|1(list))
			{
				return CS$<>8__locals1.attacks;
			}
			CS$<>8__locals1.transferPredators = (from x in (from x in CS$<>8__locals1.faction.fleets
					where x.SpaceCombatValue() >= 150f
					where CS$<>8__locals1.MayUseFleetForAttack(x)
					where x.ref_system != null
					group x by x.ref_system into x
					select new ValueTuple<TISpaceBodyState, TISpaceFleetState>(x.Key, x.MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue()))).Take_Random<ValueTuple<TISpaceBodyState, TISpaceFleetState>>(5)
				select x.Item2).ToList<TISpaceFleetState>();
			CS$<>8__locals1.maxTransferPredatorStrength = 0f;
			if (CS$<>8__locals1.transferPredators.Count > 0)
			{
				CS$<>8__locals1.maxTransferPredatorStrength = CS$<>8__locals1.transferPredators.Max<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
			}
			List<ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>>> list2 = (from x in (from x in GameStateManager.AlienFaction().fleets
					where !x.inTransfer
					where x.ref_system != null
					select x.ref_system into x
					select new ValueTuple<TISpaceBodyState, float>(x, AIEvaluators.GetFleetStrengthInSystem(CS$<>8__locals1.faction, x)) into x
					where x.Item2 < CS$<>8__locals1.maxTransferPredatorStrength / 2f
					orderby x.Item2 * (1f + 0.5f * TIUtilities.RandomFloatValue())
					select x).Take<ValueTuple<TISpaceBodyState, float>>(5).ToList<ValueTuple<TISpaceBodyState, float>>()
				select new ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>>(x.Item1, CS$<>8__locals1.transferPredators.Select<TISpaceFleetState, ValueTuple<TISpaceFleetState, float>>(delegate(TISpaceFleetState predator)
				{
					TISpaceBodyState preySystem = x.Item1;
					TISpaceShipState tispaceShipState = predator.ships.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => Mathf.Pow(x.currentMaxDeltaV_kps, 1.5f) * x.cruiseAcceleration_mps2);
					TISpaceShipState tispaceShipState2 = predator.ships.MaxBy<TISpaceShipState, float>((TISpaceShipState x) => Mathf.Pow(x.currentMaxDeltaV_kps, 2.5f) * x.cruiseAcceleration_mps2);
					float num2 = Enumerable.Empty<TISpaceShipState>().Append(tispaceShipState).Append(tispaceShipState2)
						.Distinct<TISpaceShipState>()
						.ToList<TISpaceShipState>()
						.Min<TISpaceShipState>((TISpaceShipState x) => AIEvaluators.GetEstimatedTransferTime_days(CS$<>8__locals1.faction, predator.ref_system, preySystem, x.cruiseAcceleration_mps2, x.currentMaxDeltaV_kps * 1000f / 2f, float.PositiveInfinity));
					return new ValueTuple<TISpaceFleetState, float>(predator, num2);
				}).ToList<ValueTuple<TISpaceFleetState, float>>())).ToList<ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>>>();
			float num = float.PositiveInfinity;
			if (list2.Any<ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>>>(([TupleElementNames(new string[] { "System", "Predators", "Predator", "Days" })] ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>> x) => x.Item2.Count > 0))
			{
				num = list2.Min<ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>>>(([TupleElementNames(new string[] { "System", "Predators", "Predator", "Days" })] ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>> x) => x.Item2.Min<ValueTuple<TISpaceFleetState, float>>(([TupleElementNames(new string[] { "Predator", "Days" })] ValueTuple<TISpaceFleetState, float> y) => y.Item2));
			}
			CS$<>8__locals1.maxAllowableDays = Mathf.Min(250f, num * 1.3f);
			List<ValueTuple<TISpaceFleetState, TISpaceFleetState>> list3 = list2.Select<ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>>, ValueTuple<TISpaceBodyState, IEnumerable<TISpaceFleetState>>>(delegate([TupleElementNames(new string[] { "System", "Predators", "Predator", "Days" })] ValueTuple<TISpaceBodyState, List<ValueTuple<TISpaceFleetState, float>>> x)
			{
				TISpaceBodyState item = x.Item1;
				IEnumerable<ValueTuple<TISpaceFleetState, float>> item2 = x.Item2;
				Func<ValueTuple<TISpaceFleetState, float>, bool> func2;
				if ((func2 = CS$<>8__locals1.<>9__34) == null)
				{
					func2 = (CS$<>8__locals1.<>9__34 = ([TupleElementNames(new string[] { "Predator", "Days" })] ValueTuple<TISpaceFleetState, float> x) => x.Item2 <= CS$<>8__locals1.maxAllowableDays);
				}
				return new ValueTuple<TISpaceBodyState, IEnumerable<TISpaceFleetState>>(item, from x in item2.Where<ValueTuple<TISpaceFleetState, float>>(func2)
					select x.Item1);
			}).SelectMany<ValueTuple<TISpaceBodyState, IEnumerable<TISpaceFleetState>>, ValueTuple<TISpaceFleetState, TISpaceFleetState>>(([TupleElementNames(new string[] { "System", null })] ValueTuple<TISpaceBodyState, IEnumerable<TISpaceFleetState>> x) => from x in x.Item2.Select<TISpaceFleetState, ValueTuple<TISpaceFleetState, TISpaceFleetState>>(delegate(TISpaceFleetState predator)
				{
					float maxPreyStrength = predator.SpaceCombatValue() / 2f;
					IEnumerable<TISpaceFleetState> enumerable2 = from y in x.Item1.fleetsInSystem
						where y.faction.IsAlienFaction
						where y.SpaceCombatValue() < maxPreyStrength
						select y;
					IEnumerable<TISpaceFleetState> enumerable3 = enumerable2.Where<TISpaceFleetState>((TISpaceFleetState x) => x.AI_NeedsRepairBadly());
					if (enumerable3.Any<TISpaceFleetState>())
					{
						enumerable2 = enumerable3;
					}
					return new ValueTuple<TISpaceFleetState, TISpaceFleetState>(predator, enumerable2.MinBy<TISpaceFleetState, float>((TISpaceFleetState y) => y.SpaceCombatValue()));
				})
				where x.Item2 != null
				select x).ToList<ValueTuple<TISpaceFleetState, TISpaceFleetState>>();
			CS$<>8__locals1.<GenerateQuickAttacks>g__TryFillAttacks|1(list3);
			return CS$<>8__locals1.attacks;
		}

		// Token: 0x060049BA RID: 18874 RVA: 0x001ECC9C File Offset: 0x001EAE9C
		public static TISpaceBodyState GetFutureSystem(this TIGameState state)
		{
			TISpaceBodyState tispaceBodyState = state.ref_system;
			if (state.ref_fleet != null && state.ref_fleet.inTransfer)
			{
				TISpaceFleetState ref_fleet = state.ref_fleet;
				if (ref_fleet.trajectory.destinationFleet != null)
				{
					TISpaceFleetState destinationFleet = ref_fleet.trajectory.destinationFleet;
					if (!destinationFleet.inTransfer || destinationFleet.trajectory.destinationFleet == null)
					{
						tispaceBodyState = destinationFleet.GetFutureSystem();
					}
				}
				else if (ref_fleet.trajectory.destinationOrbit != null)
				{
					tispaceBodyState = ref_fleet.trajectory.destination.ref_system;
				}
			}
			if (tispaceBodyState == GameStateManager.Sol())
			{
				return null;
			}
			return tispaceBodyState;
		}

		// Token: 0x060049BB RID: 18875 RVA: 0x001ECD4C File Offset: 0x001EAF4C
		public static bool ShouldLaunchEmergencyAttackAgainstAsset(TIFactionState actor, TIGameState enemyAsset, bool spontaneousAttack)
		{
			if (actor.permanentAlly(enemyAsset.ref_faction))
			{
				return false;
			}
			if (actor.IsAlienFaction && actor.IsTrespassing(enemyAsset))
			{
				TISpaceBodyState futureSystem = enemyAsset.GetFutureSystem();
				bool flag = futureSystem.objectType == SpaceObjectType.Planet;
				List<TIHabState> list = futureSystem.habsInSystem.Where<TIHabState>((TIHabState x) => x.faction == enemyAsset.ref_faction).ToList<TIHabState>();
				if (enemyAsset.isHabState)
				{
					if (flag)
					{
						if (list.Count == 1)
						{
							return true;
						}
						if (spontaneousAttack)
						{
							if (enemyAsset.ref_hab.SpaceCombatValue() == 0f)
							{
								return true;
							}
						}
						else if (list.Sum<TIHabState>((TIHabState x) => x.SpaceCombatValue()) == 0f)
						{
							return true;
						}
					}
				}
				else if (enemyAsset.isSpaceFleetState && flag && list.Count == 0 && enemyAsset.ref_fleet.inTransfer && enemyAsset.ref_fleet.HasFoundHabCapability())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060049BC RID: 18876 RVA: 0x001ECE70 File Offset: 0x001EB070
		public static void GetTypicalSTOFighterStats(out float spaceCombatValue, out float boostCost)
		{
			float num = -1f;
			if (AIEvaluators.typicalSTOFighterCachedDate != null)
			{
				num = (float)(TITimeState.Now() - AIEvaluators.typicalSTOFighterCachedDate).TotalDays;
			}
			if (num >= 0f && num <= AIEvaluators.typicalSTOFighter_CacheWaitTime_d)
			{
				spaceCombatValue = AIEvaluators.cachedTypicalSTOFigherSCV;
				boostCost = AIEvaluators.cachedTypicalSTOFigherBoostCost;
				return;
			}
			List<TINationState> list = (from x in GameStateManager.AllExtantNations()
				where x.executiveFaction != null
				where x.numSTOFighters > 0
				select x).ToList<TINationState>();
			if (list.Count == 0)
			{
				spaceCombatValue = 5f;
				boostCost = 20f;
				AIEvaluators.typicalSTOFighterCachedDate = TITimeState.Now();
				return;
			}
			List<TINationState> list2 = new List<TINationState>();
			while (list2.Count < 5 && list.Count > 0)
			{
				TINationState tinationState = list.SelectRandomWeightedItem<TINationState>((TINationState x) => (float)x.numSTOFighters, -1f, 1E-37f);
				list.Remove(tinationState);
				list2.Add(tinationState);
			}
			Dictionary<TINationState, TISpaceShipTemplate> dictionary = list2.ToDictionary<TINationState, TINationState, TISpaceShipTemplate>((TINationState x) => x, (TINationState x) => x.executiveFaction.DesignSTOFighter(x, null));
			spaceCombatValue = (AIEvaluators.cachedTypicalSTOFigherSCV = dictionary.Sum<KeyValuePair<TINationState, TISpaceShipTemplate>>((KeyValuePair<TINationState, TISpaceShipTemplate> x) => x.Value.TemplateSpaceCombatValue(false, -1f, 1f, false) * (float)x.Key.numSTOFighters) / (float)dictionary.Sum<KeyValuePair<TINationState, TISpaceShipTemplate>>((KeyValuePair<TINationState, TISpaceShipTemplate> x) => x.Key.numSTOFighters));
			boostCost = (AIEvaluators.cachedTypicalSTOFigherBoostCost = dictionary.Sum<KeyValuePair<TINationState, TISpaceShipTemplate>>((KeyValuePair<TINationState, TISpaceShipTemplate> x) => x.Value.wetMass_tons * TemplateManager.global.spaceResourceToTons * (float)x.Key.numSTOFighters) / (float)dictionary.Sum<KeyValuePair<TINationState, TISpaceShipTemplate>>((KeyValuePair<TINationState, TISpaceShipTemplate> x) => x.Key.numSTOFighters));
			AIEvaluators.typicalSTOFighterCachedDate = TITimeState.Now();
		}

		// Token: 0x060049BD RID: 18877 RVA: 0x001ED090 File Offset: 0x001EB290
		public static float GetTypicalSTOFighterSpaceCombatValue()
		{
			float num;
			float num2;
			AIEvaluators.GetTypicalSTOFighterStats(out num, out num2);
			return num;
		}

		// Token: 0x060049BE RID: 18878 RVA: 0x001ED0A8 File Offset: 0x001EB2A8
		public static float GetTypicalSTOFighterBoostCost()
		{
			float num;
			float num2;
			AIEvaluators.GetTypicalSTOFighterStats(out num, out num2);
			return num2;
		}

		// Token: 0x060049BF RID: 18879 RVA: 0x001ED0C0 File Offset: 0x001EB2C0
		public static float ScoreRelationsChange(TINationState seeker, TINationState target, RelationChange change, bool sameExecutive)
		{
			switch (change)
			{
			case RelationChange.NormalToAlly:
				return AIEvaluators.ScoreFormAlliance(seeker, target, sameExecutive, true);
			case RelationChange.AllyToNormal:
				return AIEvaluators.ScoreEndAlliance(seeker, target, sameExecutive, true);
			case RelationChange.RivalToNormal:
				return AIEvaluators.ScoreImprovedRelations(seeker, target, sameExecutive);
			case RelationChange.NormalToRival:
				return AIEvaluators.ScoreInitiateRivalry(seeker, target, sameExecutive);
			default:
				return -1f;
			}
		}

		// Token: 0x060049C0 RID: 18880 RVA: 0x001ED112 File Offset: 0x001EB312
		public static float ScoreLeaveFederation(TINationState nation, TIFederationState federation)
		{
			if (!(federation.leadNation == nation))
			{
				federation.leadNation.executiveFaction == nation.executiveFaction;
			}
			return -1f;
		}

		// Token: 0x060049C1 RID: 18881 RVA: 0x001ED140 File Offset: 0x001EB340
		public static float ScoreEndAlliance(TINationState seeker, TINationState ally, bool sameExecutive, bool checkFormAllianceTest)
		{
			if (sameExecutive || seeker.NumArmiesDefendingMe() <= ally.numStandardArmies || (ally.numNuclearWeapons > 0 && seeker.NumNuclearWeaponsDefendingMe() == ally.numNuclearWeapons))
			{
				return -1f;
			}
			TIFactionState executiveFaction = seeker.executiveFaction;
			if (executiveFaction != null && executiveFaction.permanentAlly(ally.executiveFaction))
			{
				return -1f;
			}
			TIFactionState executiveFaction2 = seeker.executiveFaction;
			if (executiveFaction2 != null && executiveFaction2.FindGoals(GoalType.SupportNation, seeker.executiveFaction, ally, TIFactionState.GoalFilter.none, true).Count > 0)
			{
				return -1f;
			}
			if (seeker.wars.Intersect<TINationState>(ally.wars).Count<TINationState>() > 0)
			{
				return -1f;
			}
			float num = 0f;
			if (seeker.executiveFaction != null && ally.executiveFaction != null)
			{
				num = TINationState.GetIdeologicalDistance(seeker.executiveFaction.ideology, ally.executiveFaction.ideology) + seeker.executiveFaction.GetFactionHate(ally.executiveFaction) - 10f;
			}
			num += (float)(seeker.numStandardArmies + seeker.numNuclearWeapons * 3 - ally.numStandardArmies - ally.numNuclearWeapons * 3);
			if (seeker.AccessibleWarEnemy(ally, true))
			{
				TIFactionState executiveFaction3 = seeker.executiveFaction;
				TIFactionGoalState tifactionGoalState;
				if (executiveFaction3 == null)
				{
					tifactionGoalState = null;
				}
				else
				{
					tifactionGoalState = executiveFaction3.FindGoals(new List<GoalType>
					{
						GoalType.CaptureNationDirty,
						GoalType.NeutralizeNation,
						GoalType.PillageNation
					}, seeker.executiveFaction, ally, TIFactionState.GoalFilter.none, true).MaxBy<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance);
				}
				TIFactionGoalState tifactionGoalState2 = tifactionGoalState;
				if (seeker.HasClaimOnOtherNation(ally, true))
				{
					TIFactionState executiveFaction4 = seeker.executiveFaction;
					TIFactionGoalState tifactionGoalState3 = ((executiveFaction4 != null) ? executiveFaction4.FindGoals(GoalType.ExpandNation, seeker, ally, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() : null);
					int? num2 = ((tifactionGoalState3 != null) ? new int?(tifactionGoalState3.importance) : null);
					int? num3 = ((tifactionGoalState2 != null) ? new int?(tifactionGoalState2.importance) : null);
					if ((num2.GetValueOrDefault() > num3.GetValueOrDefault()) & ((num2 != null) & (num3 != null)))
					{
						tifactionGoalState2 = tifactionGoalState3;
					}
				}
				if (tifactionGoalState2 != null)
				{
					num += (float)tifactionGoalState2.importance;
				}
			}
			if (checkFormAllianceTest && num > -1f && AIEvaluators.ScoreFormAlliance(seeker, ally, sameExecutive, false) > 0f)
			{
				return -1f;
			}
			return num;
		}

		// Token: 0x060049C2 RID: 18882 RVA: 0x001ED384 File Offset: 0x001EB584
		public static float ScoreInitiateRivalry(TINationState attacker, TINationState defender, bool sameExecutive)
		{
			if (sameExecutive || (attacker.numStandardArmies == 0 && attacker.numNuclearWeapons == 0))
			{
				return -1f;
			}
			TIFactionState executiveFaction = attacker.executiveFaction;
			if (executiveFaction != null && executiveFaction.permanentAlly(defender.executiveFaction))
			{
				return -1f;
			}
			TIFactionState executiveFaction2 = attacker.executiveFaction;
			if (executiveFaction2 != null && executiveFaction2.isAlienAppeaser && defender.alienNation)
			{
				return -1f;
			}
			TIFactionState executiveFaction3 = attacker.executiveFaction;
			if ((executiveFaction3 == null || executiveFaction3.NationWithFactionInterest(defender, true)) && defender.executiveFaction == null)
			{
				return -1f;
			}
			float num = 0f;
			if (attacker.AccessibleWarEnemy(defender, true))
			{
				num = attacker.DefensiveAllianceMilitaryStrength() - 1.5f * defender.DefensiveAllianceMilitaryStrength();
				TIFactionState executiveFaction4 = attacker.executiveFaction;
				TIFactionGoalState tifactionGoalState;
				if (executiveFaction4 == null)
				{
					tifactionGoalState = null;
				}
				else
				{
					tifactionGoalState = executiveFaction4.FindGoals(new List<GoalType>
					{
						GoalType.CaptureNationDirty,
						GoalType.NeutralizeNation,
						GoalType.PillageNation
					}, attacker.executiveFaction, defender, TIFactionState.GoalFilter.none, true).MaxBy<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance);
				}
				TIFactionGoalState tifactionGoalState2 = tifactionGoalState;
				if (attacker.HasClaimOnOtherNation(defender, true))
				{
					TIFactionState executiveFaction5 = attacker.executiveFaction;
					TIFactionGoalState tifactionGoalState3 = ((executiveFaction5 != null) ? executiveFaction5.FindGoals(GoalType.ExpandNation, attacker, defender, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() : null);
					int? num2 = ((tifactionGoalState3 != null) ? new int?(tifactionGoalState3.importance) : null);
					int? num3 = ((tifactionGoalState2 != null) ? new int?(tifactionGoalState2.importance) : null);
					if ((num2.GetValueOrDefault() > num3.GetValueOrDefault()) & ((num2 != null) & (num3 != null)))
					{
						tifactionGoalState2 = tifactionGoalState3;
					}
				}
				if (tifactionGoalState2 != null)
				{
					num += (float)tifactionGoalState2.importance;
				}
				if (attacker.executiveFaction != null)
				{
					num *= 2.5f - attacker.executiveFaction.aiValues.riskAversion + attacker.executiveFaction.aiValues.protectHumanLife;
				}
				if (defender.alienNation)
				{
					TIFactionState executiveFaction6 = attacker.executiveFaction;
					if (executiveFaction6 != null && executiveFaction6.veryAntiAlien)
					{
						num += 10000f;
					}
					else
					{
						TIFactionState executiveFaction7 = attacker.executiveFaction;
						if (executiveFaction7 == null || executiveFaction7.antiAlien)
						{
							num += 1000f;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x060049C3 RID: 18883 RVA: 0x001ED5AC File Offset: 0x001EB7AC
		public static float ScoreFormAlliance(TINationState seeker, TINationState potentialAlly, bool sameExecutive, bool checkEndAllianceTest)
		{
			bool flag;
			if (potentialAlly.executiveFaction != null)
			{
				if (!sameExecutive)
				{
					TIFactionState executiveFaction = seeker.executiveFaction;
					flag = executiveFaction != null && executiveFaction.permanentAlly(potentialAlly.executiveFaction);
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			bool flag3;
			if (!flag2)
			{
				if (TINationState.GetIdeologicalDistance(seeker.executiveFaction, potentialAlly.executiveFaction) <= 1.5f)
				{
					if (!(potentialAlly.executiveFaction == null))
					{
						TIFactionState executiveFaction2 = seeker.executiveFaction;
						flag3 = executiveFaction2 == null || !executiveFaction2.AI_AtWarWithFaction(potentialAlly.executiveFaction);
					}
					else
					{
						flag3 = true;
					}
				}
				else
				{
					flag3 = false;
				}
			}
			else
			{
				flag3 = true;
			}
			if (!flag3)
			{
				return -1f;
			}
			TIFactionState executiveFaction3 = potentialAlly.executiveFaction;
			if (executiveFaction3 != null && executiveFaction3.ignoreInterstateDiplomacy.Contains(seeker.executiveFaction))
			{
				return -1f;
			}
			bool flag4 = seeker.HasClaimOnOtherNation(potentialAlly, true) && seeker.executiveFaction != null && (seeker.executiveFaction.FindGoals(GoalType.ExpandNation, seeker, seeker, TIFactionState.GoalFilter.none, true).Any<TIFactionGoalState>() || seeker.executiveFaction.FindGoals(GoalType.CaptureNationClean, seeker.executiveFaction, potentialAlly, TIFactionState.GoalFilter.none, true).Any<TIFactionGoalState>());
			TIFactionState executiveFaction4 = seeker.executiveFaction;
			bool flag5 = executiveFaction4 != null && executiveFaction4.FindGoals(GoalType.NeutralizeNation, seeker.executiveFaction, potentialAlly, TIFactionState.GoalFilter.none, true).Count > 0;
			if (!flag4 && flag5)
			{
				return -1f;
			}
			bool flag6 = seeker.wars.Contains(GameStateManager.AlienNation());
			bool flag7 = potentialAlly.wars.Contains(GameStateManager.AlienNation());
			if (checkEndAllianceTest && AIEvaluators.ScoreEndAlliance(seeker, potentialAlly, sameExecutive, false) > 0f)
			{
				return -1f;
			}
			bool flag8 = seeker.IsAdjacentToNation(potentialAlly, false);
			bool flag9 = (flag8 && potentialAlly.numStandardArmies > 0) || potentialAlly.numNavies > 0 || potentialAlly.numNuclearWeapons > 0;
			bool flag10 = (flag8 && seeker.numStandardArmies > 0) || seeker.numNavies > 0 || seeker.numNuclearWeapons > 0;
			seeker.enemies.Where<TINationState>((TINationState x) => x.AccessibleWarEnemy(seeker, false)).Sum<TINationState>((TINationState x) => x.numStandardArmies);
			bool flag11;
			if (seeker.numNuclearWeapons == 0)
			{
				flag11 = seeker.enemies.Sum<TINationState>((TINationState x) => x.numNuclearWeapons) > 0;
			}
			else
			{
				flag11 = false;
			}
			bool flag12 = flag11;
			int num = potentialAlly.enemies.Where<TINationState>((TINationState x) => x.AccessibleWarEnemy(seeker, false)).Sum<TINationState>((TINationState x) => x.numStandardArmies);
			bool flag13;
			if (potentialAlly.numNuclearWeapons == 0)
			{
				flag13 = potentialAlly.enemies.Sum<TINationState>((TINationState x) => x.numNuclearWeapons) > 0;
			}
			else
			{
				flag13 = false;
			}
			bool flag14 = flag13;
			float num2 = -1f;
			if (flag9 || flag10 || flag4)
			{
				if (flag2)
				{
					num2 = potentialAlly.militaryTechLevel * 3f + (float)seeker.numStandardArmies + (float)potentialAlly.numStandardArmies;
					if (!flag9 && flag10)
					{
						num2 -= (float)(num - ((flag12 && flag14) ? 18 : 0));
					}
					if (seeker.executiveFaction != null)
					{
						TIFactionGoalState tifactionGoalState = seeker.executiveFaction.FindGoals(GoalType.ExpandNation, seeker, seeker, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>();
						if (flag4 && tifactionGoalState != null)
						{
							num2 += (float)tifactionGoalState.importance;
							TIFactionState totalOwningFaction = potentialAlly.TotalOwningFaction;
							if (totalOwningFaction != null && totalOwningFaction.permanentAlly(seeker.executiveFaction))
							{
								num2 += (float)tifactionGoalState.importance;
							}
						}
					}
				}
				else if ((flag4 || flag8) && !flag14)
				{
					num2 = potentialAlly.militaryTechLevel + (float)seeker.numStandardArmies + (float)potentialAlly.numStandardArmies;
				}
				TIFactionState executiveFaction5 = seeker.executiveFaction;
				if (executiveFaction5 == null || !executiveFaction5.veryProAlien)
				{
					TIFactionState executiveFaction6 = potentialAlly.executiveFaction;
					if (executiveFaction6 == null || executiveFaction6.veryProAlien)
					{
						if (flag10 && flag7)
						{
							num2 += seeker.militaryTechLevel * (float)seeker.numStandardArmies * 3f;
						}
						if (flag9 && flag6)
						{
							num2 += potentialAlly.militaryTechLevel * (float)potentialAlly.numStandardArmies * 3f;
						}
					}
				}
			}
			return num2;
		}

		// Token: 0x060049C4 RID: 18884 RVA: 0x001EDA64 File Offset: 0x001EBC64
		private static bool AI_NationShouldAlterBehaviorDueToAlienNationPresenceOnEarth(TINationState nation1, TINationState nation2)
		{
			if (GameStateManager.AlienNation().extant)
			{
				int num;
				if (!nation1.alienNation && !nation2.alienNation)
				{
					TIFactionState executiveFaction = nation1.executiveFaction;
					if (executiveFaction == null || !executiveFaction.IsAlienProxy)
					{
						TIFactionState executiveFaction2 = nation2.executiveFaction;
						if ((executiveFaction2 == null || !executiveFaction2.IsAlienProxy) && !nation1.allies.Contains(GameStateManager.AlienNation()))
						{
							num = (nation2.allies.Contains(GameStateManager.AlienNation()) ? 1 : 0);
							goto IL_0069;
						}
					}
				}
				num = 1;
				IL_0069:
				return num == 0;
			}
			return false;
		}

		// Token: 0x060049C5 RID: 18885 RVA: 0x001EDAE0 File Offset: 0x001EBCE0
		public static float ScoreIncreasingConflict(TINationState attacker, TINationState defender, bool sameExecutive, PolicyType policy)
		{
			float num = -1f;
			TIFactionState executiveFaction = attacker.executiveFaction;
			if (executiveFaction != null && executiveFaction.permanentAlly(defender.executiveFaction))
			{
				return -1f;
			}
			TIFactionState executiveFaction2 = attacker.executiveFaction;
			if (executiveFaction2 != null && executiveFaction2.isAlienAppeaser && defender.alienNation)
			{
				return -1f;
			}
			if (attacker.allies.Contains(defender))
			{
				if (attacker.executiveFaction != null && sameExecutive)
				{
					return -1f;
				}
				if (attacker.executiveFaction != null && defender.executiveFaction != null)
				{
					num = attacker.executiveFaction.GetFactionHate(defender.executiveFaction);
				}
			}
			else
			{
				TIFactionState ref_faction = attacker.ref_faction;
				if ((ref_faction != null && ref_faction.NationWithFactionInterest(defender, true) && defender.executiveFaction == null) || (attacker.executiveFaction != null && attacker.executiveFaction == defender.executiveFaction))
				{
					return -1f;
				}
				if (policy == PolicyType.WarOption)
				{
					if (attacker.armies.Count == 0)
					{
						return -1f;
					}
					if (attacker.wars.Count > 0 && defender.numStandardArmies > (attacker.alienNation ? 1 : 0))
					{
						return -1f;
					}
					if ((float)attacker.wars.Count > (float)attacker.armies.Count<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy) / 2f)
					{
						return -1f;
					}
					if (attacker.wars.Count > 8)
					{
						return -1f;
					}
					if (attacker.unrest >= 7f)
					{
						return -1f;
					}
					if (attacker.armies.Count == 1 && (defender.NumArmiesDefendingMe() > 0 || defender.militaryTechLevel + 1f > attacker.militaryTechLevel))
					{
						return -1f;
					}
					float num2;
					List<TINationState> list;
					if (AIEvaluators.AIAlliesCollectivelyWillingToJoinOffensiveWar(attacker, defender, out num2, out list))
					{
						num = Mathf.Min((float)((list.Any<TINationState>((TINationState x) => x.HasClaimOnOtherNation(defender, true)) || defender.alienNation) ? 1000 : 20), num2 / (2f * defender.DefensiveAllianceMilitaryStrength()));
					}
					else
					{
						num = Mathf.Min((float)((attacker.HasClaimOnOtherNation(defender, true) || defender.alienNation) ? 1000 : 20), attacker.militaryStrength / (2f * defender.DefensiveAllianceMilitaryStrength()));
					}
					if (num >= 1f)
					{
						foreach (TINationState tinationState in attacker.wars)
						{
							if (!attacker.WinningWarAgainst(tinationState))
							{
								return -1f;
							}
							int num3 = Mathf.Min(20, tinationState.numStandardArmies + tinationState.regions.Count / 2);
							if (num3 > 0)
							{
								num *= 0.1f;
								for (int i = 0; i < num3; i++)
								{
									num *= 0.1f;
								}
							}
							else
							{
								num *= 0.1f;
							}
							if (num < 0.5f)
							{
								return -1f;
							}
						}
						foreach (TINationState tinationState2 in defender.wars)
						{
							if (defender.WinningWarAgainst(tinationState2))
							{
								num *= 1f;
							}
							else
							{
								num *= 1.1f;
							}
						}
						int num4 = defender.NumNuclearWeaponsDefendingMe();
						if (num4 > 0)
						{
							if (attacker.numNuclearWeapons == 0)
							{
								return -1f;
							}
							if (attacker.numNuclearWeapons < num4)
							{
								num *= 0.01f;
							}
							else
							{
								num *= 0.1f;
							}
						}
						float num5 = attacker.CohesionLossFromDeclaringWar(defender);
						if (num5 > 0f)
						{
							if (num5 > attacker.cohesion)
							{
								return -1f;
							}
							num -= num5 * (12f - attacker.cohesion);
						}
						if (attacker.executiveFaction != null && !defender.alienNation && (double)num > 0.5)
						{
							num *= 2.5f - attacker.executiveFaction.aiValues.riskAversion - attacker.executiveFaction.aiValues.protectHumanLife;
						}
					}
					IL_0488:
					if ((double)num <= 0.5)
					{
						return -1f;
					}
				}
			}
			if (attacker.AccessibleWarEnemy(defender, true))
			{
				bool flag = AIEvaluators.AI_NationShouldAlterBehaviorDueToAlienNationPresenceOnEarth(attacker, defender);
				TIFactionState executiveFaction3 = attacker.executiveFaction;
				TIFactionGoalState tifactionGoalState;
				if (executiveFaction3 == null)
				{
					tifactionGoalState = null;
				}
				else
				{
					tifactionGoalState = executiveFaction3.FindGoals(new List<GoalType>
					{
						GoalType.CaptureNationDirty,
						GoalType.NeutralizeNation,
						GoalType.PillageNation
					}, attacker.executiveFaction, defender, TIFactionState.GoalFilter.none, true).MaxBy<TIFactionGoalState, int>((TIFactionGoalState x) => x.importance);
				}
				TIFactionGoalState tifactionGoalState2 = tifactionGoalState;
				if (attacker.HasClaimOnOtherNation(defender, true))
				{
					TIFactionState executiveFaction4 = attacker.executiveFaction;
					TIFactionGoalState tifactionGoalState3 = ((executiveFaction4 != null) ? executiveFaction4.FindGoals(GoalType.ExpandNation, attacker, attacker, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() : null);
					int? num6 = ((tifactionGoalState3 != null) ? new int?(tifactionGoalState3.importance) : null);
					int? num7 = ((tifactionGoalState2 != null) ? new int?(tifactionGoalState2.importance) : null);
					if ((num6.GetValueOrDefault() > num7.GetValueOrDefault()) & ((num6 != null) & (num7 != null)))
					{
						tifactionGoalState2 = tifactionGoalState3;
					}
					if (flag)
					{
						TIFactionState executiveFaction5 = attacker.executiveFaction;
						if (executiveFaction5 == null || !executiveFaction5.isAlienAppeaser)
						{
							TIFactionState executiveFaction6 = defender.executiveFaction;
							if (executiveFaction6 == null || !executiveFaction6.isAlienAppeaser)
							{
								return -1f;
							}
						}
						num *= 1f;
					}
					else
					{
						num *= 10f;
					}
				}
				else
				{
					if (flag)
					{
						return -1f;
					}
					if (attacker != null && attacker.executiveFaction.minorCPTrouble && policy == PolicyType.WarOption && !defender.alienNation)
					{
						num /= 3f;
						if (attacker.executiveFaction.majorCPTrouble)
						{
							num /= (float)(8 - defender.numControlPoints);
						}
					}
				}
				if (tifactionGoalState2 != null)
				{
					if (num > 0f)
					{
						num *= (float)tifactionGoalState2.importance;
						num /= (float)(7 - defender.numControlPoints);
					}
				}
				else
				{
					num /= (float)(12 - defender.numControlPoints);
				}
				if (defender.alienNation)
				{
					if (attacker.executiveFaction.veryAntiAlien)
					{
						num += 1f;
						num *= 100f;
					}
					else if (!attacker.executiveFaction.veryProAlien)
					{
						num *= Mathf.Max(1f, attacker.executiveFaction.ideologyCoordinates.x + 1f);
					}
				}
			}
			return num;
		}

		// Token: 0x060049C6 RID: 18886 RVA: 0x001EE1F8 File Offset: 0x001EC3F8
		public static float ScoreImprovedRelations(TINationState seeker, TINationState recipient, bool sameExecutive)
		{
			float num = -1f;
			bool flag = seeker.enemies.Contains(recipient) && AIEvaluators.AlwaysEndConflict(seeker, recipient);
			if (seeker.executiveFaction != null && recipient.executiveFaction != null && !sameExecutive && !flag && ((seeker.executiveFaction.proAlien && recipient.executiveFaction.antiAlien) || (recipient.executiveFaction.proAlien && seeker.executiveFaction.antiAlien) || seeker.executiveFaction.GetFactionHate(recipient.executiveFaction) > 0f || recipient.executiveFaction.GetFactionHate(seeker.executiveFaction) > 0f))
			{
				return -1f;
			}
			if (seeker.improveRelationsDeclinedUnderCurrentExecutivePair.Contains(recipient))
			{
				return -1f;
			}
			TIFactionState executiveFaction = recipient.executiveFaction;
			if (executiveFaction != null && executiveFaction.ignoreInterstateDiplomacy.Contains(seeker.executiveFaction))
			{
				return -1f;
			}
			if (seeker.rivals.Contains(recipient) && AIEvaluators.ScoreInitiateRivalry(seeker, recipient, sameExecutive) > 0f)
			{
				return -1f;
			}
			bool flag2 = seeker.IsAdjacentToNation(recipient, false);
			bool flag3 = seeker.AccessibleWarEnemy(recipient, true);
			bool flag4 = (flag2 && recipient.numStandardArmies > 0) || recipient.numNavies > 0 || recipient.numNuclearWeapons > 0;
			bool flag5 = seeker.HasClaimOnOtherNation(recipient, false);
			bool flag6;
			if (recipient.executiveFaction != null)
			{
				if (!sameExecutive)
				{
					TIFactionState executiveFaction2 = seeker.executiveFaction;
					flag6 = executiveFaction2 != null && executiveFaction2.permanentAlly(recipient.executiveFaction);
				}
				else
				{
					flag6 = true;
				}
			}
			else
			{
				flag6 = false;
			}
			bool flag7 = flag6;
			bool flag8 = AIEvaluators.AI_NationShouldAlterBehaviorDueToAlienNationPresenceOnEarth(seeker, recipient);
			if (flag7)
			{
				if (seeker.rivals.Contains(recipient))
				{
					num += 50f;
				}
				if (flag2)
				{
					num += 10f;
				}
				if (flag4)
				{
					num += recipient.militaryTechLevel * (float)(flag8 ? 5 : 3);
				}
				if (flag5)
				{
					num += (float)(recipient.regions.Count * 10);
					if (seeker.executiveFaction.minorCPTrouble)
					{
						num *= 5f;
						if (seeker.executiveFaction.majorCPTrouble)
						{
							num *= 20f;
						}
					}
				}
			}
			else
			{
				bool flag9 = recipient.HasClaimOnOtherNation(seeker, false);
				float num2 = AIEvaluators.GetAIRelativeValuation(FactionResource.Boost) * recipient.boostIncome_month_dekatons + AIEvaluators.GetAIRelativeValuation(FactionResource.Research) * recipient.research_month - AIEvaluators.GetAIRelativeValuation(FactionResource.Boost) * seeker.boostIncome_month_dekatons - AIEvaluators.GetAIRelativeValuation(FactionResource.Research) * seeker.research_month;
				if (flag4)
				{
					num += recipient.militaryTechLevel + (float)seeker.rivals.Count;
					if (!flag3)
					{
						num *= 2f;
						if (seeker.rivals.Contains(recipient))
						{
							num *= 2f;
						}
					}
				}
				if (flag2)
				{
					num += 1f;
				}
				if (flag5)
				{
					num += 1f;
					if (num2 > 0f)
					{
						num += 1f;
					}
				}
				else if (flag9 && num2 <= 0f)
				{
					num -= 1f;
				}
				if (recipient.executiveFaction == null)
				{
					num -= 2f;
				}
				else if (seeker != null)
				{
					num -= Mathf.Min(seeker.executiveFaction.GetFactionHate(recipient.executiveFaction), 20f);
				}
				num -= (float)recipient.allies.Intersect<TINationState>(seeker.rivals).Count<TINationState>();
				num -= (float)recipient.rivals.Except<TINationState>(seeker.rivals).Count<TINationState>();
				if (recipient.breakaway)
				{
					num -= recipient.breakawayParent.militaryStrength;
					if (seeker.allies.Contains(recipient.breakawayParent))
					{
						num -= 20f;
					}
				}
			}
			if (num > 0f || seeker.rivals.Contains(recipient) || (seeker.numStandardArmies <= recipient.numStandardArmies && recipient.numStandardArmies > 0))
			{
				num += (float)recipient.allies.Intersect<TINationState>(seeker.allies).Count<TINationState>() / 2f;
				num += (float)seeker.rivals.Intersect<TINationState>(recipient.rivals).Count<TINationState>() / 2f;
				TIFactionState executiveFaction3 = seeker.executiveFaction;
				TIFactionGoalState tifactionGoalState = ((executiveFaction3 != null) ? executiveFaction3.FindGoals(GoalType.CaptureNationClean, seeker.executiveFaction, recipient, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() : null);
				if (flag7 && tifactionGoalState == null)
				{
					TIFactionState executiveFaction4 = seeker.executiveFaction;
					tifactionGoalState = ((executiveFaction4 != null) ? executiveFaction4.FindGoals(GoalType.CaptureNationDirty, seeker.executiveFaction, recipient, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() : null);
				}
				if (tifactionGoalState != null)
				{
					num += (float)tifactionGoalState.importance * ((flag5 || recipient.FactionHasControlPoint(seeker.executiveFaction)) ? 1f : 0.25f);
				}
				num += (float)recipient.numControlPoints_unclamped;
				if (flag8)
				{
					num += 20f;
				}
			}
			if (flag)
			{
				num = Mathf.Max(num * 100f, 100000f);
			}
			return num + (float)recipient.numControlPoints_unclamped;
		}

		// Token: 0x060049C7 RID: 18887 RVA: 0x001EE6A8 File Offset: 0x001EC8A8
		public static bool AlwaysEndConflict(TINationState actingNation, TINationState warNation)
		{
			if (actingNation.executiveFaction != null && warNation.executiveFaction != null)
			{
				if (actingNation.executiveFaction.permanentAlly(warNation.executiveFaction))
				{
					return true;
				}
				if (actingNation.executiveFaction.isAlienAppeaser && (warNation.alienNation || warNation.allies.Contains(GameStateManager.AlienNation())))
				{
					return true;
				}
			}
			return (actingNation.inFederation && actingNation.federation == warNation.federation) || (actingNation.wars.Contains(warNation) && actingNation.iCurrentWarAllianceArmies(warNation) == 0 && warNation.iCurrentWarAllianceArmies(actingNation) == 0 && actingNation.numNuclearWeapons == 0 && warNation.numNuclearWeapons == 0);
		}

		// Token: 0x060049C8 RID: 18888 RVA: 0x001EE760 File Offset: 0x001EC960
		public static float GetAlienQuietness()
		{
			float num = TIGlobalConfig.globalConfig.AI_AlienBaseQuietness();
			if (num <= 0f)
			{
				return 0f;
			}
			if (TIGlobalValuesState.GlobalValues.HasGlobalMilestoneBeenAchieved(GlobalMilestone.FirstSpaceCombatVictoryAgainstAliens))
			{
				return 0f;
			}
			IEnumerable<TIFactionState> enumerable = from x in GameStateManager.AllHumanFactions()
				where !x.veryProAlien
				select x;
			if (enumerable.Any<TIFactionState>((TIFactionState x) => x.FullSystemVisibility))
			{
				num *= 0.85f;
			}
			bool flag = GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.AlienAwareness_Public));
			if (flag)
			{
				num *= 0.75f;
			}
			TIFactionState aliens = GameStateManager.AlienFaction();
			bool flag2 = GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x.factionAssassinations.ContainsKey(aliens));
			bool flag3 = GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.AccessLiveHydra));
			int num2 = GameStateManager.AllHumanFactions().Sum<TIFactionState>((TIFactionState x) => x.aliensRemoved);
			bool flag4 = !aliens.councilors.Any<TICouncilorState>((TICouncilorState x) => x.OnEarth);
			bool flag5 = enumerable.Any<TIFactionState>((TIFactionState x) => TIEffectsState.SumEffectsModifiers(Context.PublicizedAlienThreat, x, 0f, null) > 0f);
			enumerable.Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.AlienCouncilorSighted));
			enumerable.Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.AlienWarshipSighted));
			enumerable.Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.AlienHabSighted));
			enumerable.Any<TIFactionState>((TIFactionState x) => x.MilestoneCompleted(CampaignMilestone.AccessAlienTech));
			bool extant = GameStateManager.AlienNation().extant;
			if (flag)
			{
				num -= 0.05f;
			}
			if (flag2)
			{
				num -= 0.05f;
			}
			if (flag3)
			{
				num -= 0.05f;
			}
			if (flag5)
			{
				num -= 0.15f;
			}
			if (flag4)
			{
				num -= 0.1f;
			}
			if (extant)
			{
				num -= 0.1f;
			}
			num -= (float)num2 * 0.05f;
			return Mathf.Clamp01(num);
		}

		// Token: 0x060049C9 RID: 18889 RVA: 0x001EE9F7 File Offset: 0x001ECBF7
		public static bool ShouldAliensGoLoud()
		{
			return AIEvaluators.GetAlienQuietness() <= 0f;
		}

		// Token: 0x060049CA RID: 18890 RVA: 0x001EEA08 File Offset: 0x001ECC08
		public static int GetAliensPreferredCouncilorCount()
		{
			GameStateManager.AlienFaction();
			float alienQuietness = AIEvaluators.GetAlienQuietness();
			if (alienQuietness <= 0f)
			{
				return 6;
			}
			int num = 1;
			if (alienQuietness < 0.9f)
			{
				num++;
			}
			if (alienQuietness < 0.8f)
			{
				num++;
			}
			if (alienQuietness < 0.7f)
			{
				num++;
			}
			if (alienQuietness < 0.6f)
			{
				num += 2;
			}
			int num2 = GameStateManager.AllHumanFactions().Sum<TIFactionState>((TIFactionState x) => x.aliensRemoved);
			return num + num2;
		}

		// Token: 0x060049CB RID: 18891 RVA: 0x001EEA8C File Offset: 0x001ECC8C
		public static bool ShouldAliensXenoform()
		{
			return AIEvaluators.GetAlienQuietness() < 0.9f;
		}

		// Token: 0x060049CC RID: 18892 RVA: 0x001EEA9A File Offset: 0x001ECC9A
		public static bool BadRegion(TINationState nation, TIRegionState region)
		{
			return nation.hostileClaims.Contains(region) && (nation.unrestWarning || (nation.democracy > 5f && nation.cohesionWarning)) && !region.coreEconomicRegion && !region.coreResourceRegion;
		}

		// Token: 0x060049CD RID: 18893 RVA: 0x001EEADC File Offset: 0x001ECCDC
		public static bool AIWillingToJoinOffensiveAllysWar(TINationState nation, TINationState allyStartingWar, TINationState defender)
		{
			if (!nation.ValidNewWarTarget(defender, true))
			{
				return false;
			}
			if (nation.allies.Contains(defender))
			{
				return false;
			}
			if (nation.executiveFaction == allyStartingWar.executiveFaction)
			{
				return true;
			}
			if (AIEvaluators.AlwaysEndConflict(nation, defender))
			{
				return false;
			}
			if (defender.FactionHasControlPoint(nation.executiveFaction))
			{
				return false;
			}
			if (nation.wars.Count >= nation.numStandardArmies)
			{
				return false;
			}
			List<TIRegionState> list = new List<TIRegionState>(defender.regions);
			List<TINationState> allDefendingNations = new List<TINationState> { defender };
			List<TINationState> warCapableAllies = defender.WarCapableAllies;
			if (warCapableAllies.Count > 0)
			{
				if (warCapableAllies.Any<TINationState>((TINationState x) => x.executiveFaction == nation.executiveFaction))
				{
					return false;
				}
				Func<TIArmyState, bool> <>9__7;
				if (warCapableAllies.Any<TINationState>(delegate(TINationState x)
				{
					IEnumerable<TIArmyState> armies = x.armies;
					Func<TIArmyState, bool> func;
					if ((func = <>9__7) == null)
					{
						func = (<>9__7 = (TIArmyState x) => x.faction == nation.executiveFaction);
					}
					return armies.Any<TIArmyState>(func);
				}))
				{
					return false;
				}
				allDefendingNations.AddRange(warCapableAllies);
				list.AddRange(warCapableAllies.SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions));
			}
			if (nation.executiveFaction == null)
			{
				return true;
			}
			if (defender.executiveFaction != null && nation.executiveFaction.AI_AtWarWithFaction(defender.executiveFaction))
			{
				return true;
			}
			if ((from x in nation.executiveFaction.GoalsOfType(GoalType.CaptureNationDirty, false, true)
				select x.target()).Any<TIGameState>((TIGameState x) => allDefendingNations.Contains(x)))
			{
				return true;
			}
			if ((from x in nation.executiveFaction.GoalsOfType(GoalType.NeutralizeNation, false, true)
				select x.target()).Any<TIGameState>((TIGameState x) => allDefendingNations.Contains(x)))
			{
				return true;
			}
			TIFactionGoalState managementGoalForNation = nation.executiveFaction.GetManagementGoalForNation(nation, false);
			return (managementGoalForNation == null || managementGoalForNation.GetGoalType() == GoalType.ExpandNation) && nation.ExternalClaims().Intersect<TIRegionState>(list).Count<TIRegionState>() > 0;
		}

		// Token: 0x060049CE RID: 18894 RVA: 0x001EED2C File Offset: 0x001ECF2C
		public static bool AIAlliesCollectivelyWillingToJoinOffensiveWar(TINationState allyStartingWar, TINationState defender, out float offensiveAllianceStrength, out List<TINationState> prospectiveAlliance)
		{
			prospectiveAlliance = allyStartingWar.ProspectiveOffensiveAlliance(defender, true);
			offensiveAllianceStrength = prospectiveAlliance.Sum<TINationState>((TINationState x) => x.militaryStrength);
			float num = defender.DefensiveAllianceMilitaryStrength();
			float num2 = offensiveAllianceStrength / num;
			int num3 = prospectiveAlliance.SelectMany<TINationState, TINationState>((TINationState x) => x.wars).Distinct<TINationState>().Count<TINationState>();
			int num4 = defender.wars.Distinct<TINationState>().Count<TINationState>() + defender.WarCapableAllies.SelectMany<TINationState, TINationState>((TINationState x) => x.wars).Distinct<TINationState>().Count<TINationState>();
			int num5 = num3 - num4;
			if (num5 <= 0)
			{
				return num2 >= 1.5f;
			}
			return num2 >= 3f + (float)num5;
		}

		// Token: 0x060049CF RID: 18895 RVA: 0x001EEE10 File Offset: 0x001ED010
		public static bool NuclearDeterred(TIFactionState potentiallyDeterredFaction, TINationState potentiallyDeterredNation, TINationState deterringNation, int goalImportance, TIWarState war = null)
		{
			if (deterringNation == null)
			{
				return false;
			}
			if (((war != null) ? deterringNation.NumNuclearWeaponsDefendingMeInWar(war) : deterringNation.NumNuclearWeaponsDefendingMe()) == 0)
			{
				return false;
			}
			if (GameStateManager.AlienNation().extant)
			{
				if (potentiallyDeterredFaction != null && potentiallyDeterredFaction.extremist)
				{
					if (potentiallyDeterredFaction.proAlien)
					{
						if (deterringNation.controlPoints.None<TIControlPoint>(delegate(TIControlPoint x)
						{
							TIFactionState faction = x.faction;
							return faction != null && faction.proAlien;
						}) && TINationState.proAlienPublic(deterringNation))
						{
							return false;
						}
					}
					if (potentiallyDeterredFaction.antiAlien)
					{
						if (deterringNation.controlPoints.All<TIControlPoint>(delegate(TIControlPoint x)
						{
							TIFactionState faction2 = x.faction;
							return faction2 != null && faction2.proAlien;
						}) && TINationState.antiAlienPublic(deterringNation))
						{
							return false;
						}
					}
				}
				else if (potentiallyDeterredFaction != null && ((double)potentiallyDeterredFaction.aiValues.protectHumanLife < 0.85 || deterringNation.alienNation) && goalImportance > 15 && potentiallyDeterredFaction.selfAssessement == FactionSelfAssessment.LosingBig)
				{
					if (potentiallyDeterredFaction.proAlien)
					{
						if (deterringNation.controlPoints.None<TIControlPoint>(delegate(TIControlPoint x)
						{
							TIFactionState faction3 = x.faction;
							return faction3 != null && faction3.proAlien;
						}) && TINationState.proAlienPublic(deterringNation))
						{
							return false;
						}
					}
					if (potentiallyDeterredFaction.antiAlien)
					{
						if (deterringNation.controlPoints.All<TIControlPoint>(delegate(TIControlPoint x)
						{
							TIFactionState faction4 = x.faction;
							return faction4 != null && faction4.proAlien;
						}) && TINationState.antiAlienPublic(deterringNation))
						{
							return false;
						}
					}
					if (!potentiallyDeterredFaction.proAlien && !potentiallyDeterredFaction.antiAlien && goalImportance == 20)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x060049D0 RID: 18896 RVA: 0x001EEFC4 File Offset: 0x001ED1C4
		public static float FactionGotoWarRequiredHate(TIFactionState faction, TIFactionState targetFaction)
		{
			if (faction.IsAlienFaction)
			{
				return TemplateManager.global.alienFactionHateWarValue;
			}
			float num = Mathf.Clamp(TINationState.GetIdeologicalDistance(faction.ideology, targetFaction.ideology), 1f, 2.5f);
			if (faction.mostPowerfulHumanEnemy == targetFaction)
			{
				num -= (float)faction.selfAssessement;
			}
			num = Mathf.Max(num, 1f);
			return TemplateManager.global.factionHateWarDeterminantDivisor / num;
		}

		// Token: 0x060049D1 RID: 18897 RVA: 0x001EF038 File Offset: 0x001ED238
		public static float FactionsGoToWarProgress(TIFactionState faction, TIFactionState targetFaction)
		{
			if (faction.IsAlienFaction && targetFaction.isAlienAppeaser && targetFaction.unlockedVictoryObjective && !faction.permanentAlly(targetFaction) && faction.GetFactionHate(targetFaction) < TemplateManager.global.alienFactionHateWarValue)
			{
				return 0f;
			}
			if (targetFaction.IsActiveHumanFaction && targetFaction.primaryHab != null && targetFaction.unlockedVictoryObjective && ((faction.IsActiveHumanFaction && targetFaction.proAlien) || (faction.veryProAlien && !targetFaction.proAlien)))
			{
				return 1f;
			}
			if (faction.HasTruce(targetFaction, false))
			{
				return 0f;
			}
			float num = 0f;
			if (faction.IsAlienFaction)
			{
				num = faction.GetFactionHate(targetFaction) / AIEvaluators.FactionGotoWarRequiredHate(faction, targetFaction);
			}
			else if (faction.GetIntel(targetFaction) >= TemplateManager.global.intelToSeeFactionBasicData)
			{
				num = faction.GetFactionHate(targetFaction) / AIEvaluators.FactionGotoWarRequiredHate(faction, targetFaction);
			}
			return Mathf.Clamp(num, 0f, 1f);
		}

		// Token: 0x060049D2 RID: 18898 RVA: 0x001EF12B File Offset: 0x001ED32B
		public static bool FactionsGoToWar(TIFactionState factionState, TIFactionState targetFaction)
		{
			return AIEvaluators.FactionsGoToWarProgress(factionState, targetFaction) >= 1f;
		}

		// Token: 0x060049D3 RID: 18899 RVA: 0x001EF140 File Offset: 0x001ED340
		public static TISpaceAssetState GetNearbySpaceAssetTarget(this TISpaceFleetState fleet)
		{
			AIEvaluators.<>c__DisplayClass228_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass228_0();
			CS$<>8__locals1.fleet = fleet;
			CS$<>8__locals1.faction = CS$<>8__locals1.fleet.faction;
			TISpaceBodyState ref_system = CS$<>8__locals1.fleet.ref_system;
			if (ref_system == null || ref_system == GameStateManager.Sol())
			{
				return null;
			}
			CS$<>8__locals1.bombardmentValue = CS$<>8__locals1.fleet.BombardmentValue(ref_system);
			List<TIHabState> list = ref_system.habsInSystem.Where<TIHabState>((TIHabState x) => x.IsBase).Where<TIHabState>(delegate(TIHabState x)
			{
				float desiredBombardmentValue = FactionGoal_AttackWithFleet.GetDesiredBombardmentValue(CS$<>8__locals1.faction, x, 0);
				return CS$<>8__locals1.bombardmentValue >= 1.5f * desiredBombardmentValue;
			}).ToList<TIHabState>();
			IEnumerable<TISpaceAssetState> enumerable = (from x in ref_system.fleetsInOrbitInSystem.Select<TISpaceFleetState, TISpaceAssetState>((TISpaceFleetState x) => x).Union<TISpaceAssetState>(ref_system.habsInSystem.Where<TIHabState>((TIHabState x) => x.IsStation)).Union<TISpaceAssetState>(list)
				where !x.deleted && x.ref_faction != null
				where !CS$<>8__locals1.faction.permanentAlly(x.ref_faction)
				select x).ToList<TISpaceAssetState>();
			CS$<>8__locals1.warFactions = CS$<>8__locals1.faction.enemyWarFactions;
			CS$<>8__locals1.strengthOfTargetsAndTheirDefenders = enumerable.ToDictionary<TISpaceAssetState, TISpaceAssetState, float>((TISpaceAssetState x) => x, new Func<TISpaceAssetState, float>(CS$<>8__locals1.faction.GetPerceivedEnemySpaceAssetStrength_AndItsDefenders));
			CS$<>8__locals1.desiredSuperiority = CS$<>8__locals1.faction.GetDesiredSuperiorityForSpontaniousAttack();
			List<TISpaceAssetState> list2 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => AIEvaluators.ShouldLaunchEmergencyAttackAgainstAsset(CS$<>8__locals1.faction, x, true)).Where<TISpaceAssetState>(new Func<TISpaceAssetState, bool>(CS$<>8__locals1.<GetNearbySpaceAssetTarget>g__NotTooDangerous|7)).ToList<TISpaceAssetState>();
			if (list2.Count > 0)
			{
				enumerable = list2;
				IEnumerable<TISpaceAssetState> enumerable2 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => CS$<>8__locals1.strengthOfTargetsAndTheirDefenders[x] == 0f);
				if (enumerable2.Any<TISpaceAssetState>())
				{
					enumerable = enumerable2;
				}
				IEnumerable<TISpaceAssetState> enumerable3 = enumerable.Where<TISpaceAssetState>(delegate(TISpaceAssetState x)
				{
					TIHabState ref_hab = x.ref_hab;
					return ref_hab != null && ref_hab.IsBase;
				});
				if (enumerable3.Any<TISpaceAssetState>())
				{
					enumerable = enumerable3;
				}
				enumerable = enumerable.ToList<TISpaceAssetState>();
			}
			else
			{
				enumerable = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => CS$<>8__locals1.warFactions.Contains(x.ref_faction) || CS$<>8__locals1.faction.IsTrespassing(x)).Where<TISpaceAssetState>(new Func<TISpaceAssetState, bool>(CS$<>8__locals1.<GetNearbySpaceAssetTarget>g__NotTooDangerous|7));
				if (TIUtilities.RandomFloatValue() < 0.15f)
				{
					IEnumerable<TISpaceAssetState> enumerable4 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.isHabState);
					IEnumerable<TISpaceAssetState> enumerable5 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.isSpaceFleetState);
					if (enumerable4.Any<TISpaceAssetState>() && (!enumerable5.Any<TISpaceAssetState>() || TIUtilities.RandomFloatValue() < 0.33f))
					{
						enumerable = enumerable4;
					}
					else
					{
						enumerable = enumerable5;
					}
				}
				else
				{
					IEnumerable<TISpaceAssetState> enumerable6 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.isHabState);
					if (enumerable6.Any<TISpaceAssetState>())
					{
						enumerable = enumerable6;
						IEnumerable<TISpaceAssetState> enumerable7 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.ref_hab.OkayModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.EnablesLocalFounding));
						if (enumerable7.Any<TISpaceAssetState>())
						{
							enumerable = enumerable7;
						}
						float averageMass_kg = (float)enumerable.Average<TISpaceAssetState>((TISpaceAssetState x) => x.mass_kg);
						IEnumerable<TISpaceAssetState> enumerable8 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.ref_hab.anyCoreCompleted && x.SpaceCombatValue() == 0f && x.mass_kg > (double)(0.8f * averageMass_kg));
						if (enumerable8.Any<TISpaceAssetState>())
						{
							enumerable = enumerable8;
						}
					}
					else
					{
						enumerable = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.isSpaceFleetState);
						IEnumerable<TISpaceAssetState> enumerable9 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => !x.ref_fleet.dockedAtHab);
						if (enumerable9.Any<TISpaceAssetState>())
						{
							enumerable = enumerable9;
						}
					}
					enumerable = enumerable.ToList<TISpaceAssetState>();
					List<TIFactionState> factions = (from x in enumerable
						select x.ref_faction into x
						where x != null
						select x).Distinct<TIFactionState>().ToList<TIFactionState>();
					TIFactionState mostThreateningEnemy = AIEvaluators.GetStrongestHumanFaction((TIFactionState x) => factions.Contains(x));
					enumerable = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.ref_faction == mostThreateningEnemy).ToList<TISpaceAssetState>();
				}
			}
			return enumerable.SelectRandomWeightedItem<TISpaceAssetState>(delegate(TISpaceAssetState x)
			{
				float num = Mathf.Pow((float)x.mass_kg, 2f);
				if (x.isHabState && !x.ref_hab.anyCoreCompleted)
				{
					num /= 100f;
				}
				return num;
			}, -1f, 1E-37f);
		}

		// Token: 0x060049D4 RID: 18900 RVA: 0x001EF600 File Offset: 0x001ED800
		public static void OnAlienNationCreated(bool addInvasionGoal)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			TINationState tinationState = GameStateManager.AlienNation();
			GameStateManager.AlienFaction().AddGoal(new FactionGoal_ExpandNation(tifactionState, 20, tinationState), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			foreach (TINationState tinationState2 in GameStateManager.AlienNation().AdjacentNations(true))
			{
				TIFactionState executiveFaction = tinationState2.executiveFaction;
				if (executiveFaction != null && executiveFaction.permanentAlly(tifactionState))
				{
					tifactionState.AddGoal(new FactionGoal_CaptureNation_Clean(tifactionState, 16, tinationState2, GoalType.ExpandNation, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
				else
				{
					tifactionState.AddGoal(new FactionGoal_CaptureNation_Dirty(tifactionState, 15 + tinationState2.numStandardArmies + tinationState2.numNuclearWeapons, tinationState2, GoalType.ExpandNation, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
			}
			if (TIGlobalValuesState.IsQuietAlienCampaign() || TIGlobalValuesState.IsInvasionFocusedAlienCampaign())
			{
				int num = 1;
				if (TIGlobalValuesState.IsInvasionFocusedAlienCampaign())
				{
					num++;
				}
				if (tifactionState.GoalsOfType(GoalType.InvadeEarth, false, true).Count < num)
				{
					tifactionState.AddGoal(new FactionGoal_InvadeEarth(tifactionState, 19), HandleDuplicateGoalRule.ResetImportance, null);
				}
			}
			else
			{
				if (!addInvasionGoal)
				{
					if (!tifactionState.armies.None<TIArmyState>((TIArmyState x) => x.AlienRegularArmy))
					{
						goto IL_0179;
					}
				}
				TIFactionGoalState tifactionGoalState = (from x in tifactionState.GoalsOfType(GoalType.InvadeEarth, false, true)
					orderby x.assignedDate
					select x).FirstOrDefault<TIFactionGoalState>();
				if (tifactionGoalState != null)
				{
					tifactionGoalState.SetImportance(20);
				}
				else
				{
					tifactionState.AddGoal(new FactionGoal_InvadeEarth(tifactionState, 20), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
			}
			IL_0179:
			foreach (TIFactionState tifactionState2 in GameStateManager.AllHumanFactions())
			{
				if (!tifactionState2.veryProAlien)
				{
					tifactionState2.GainFactionHate(tifactionState, 40f, false, "Alien Nation Created", true);
					if (tifactionState2.antiAlien)
					{
						tifactionState2.GainFactionHate(tifactionState, 30f, false, "Alien Nation Created", true);
					}
					if (tifactionState2.veryAntiAlien)
					{
						tifactionState2.GainFactionHate(tifactionState, 30f, false, "Alien Nation Created", true);
						tifactionState2.AddGoal(new FactionGoal_WarOnFaction(tifactionState2, 15, tifactionState, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					}
					tifactionState2.GainFactionHate(GameStateManager.AlienProxy(), 100f, false, "Alien Nation Created", true);
					tifactionState2.AddGoal(new FactionGoal_WarOnFaction(tifactionState2, tifactionState2.veryAntiAlien ? 19 : 14, GameStateManager.AlienProxy(), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					tifactionState2.AddGoal(new FactionGoal_NeutralizeNation(tifactionState2, tifactionState2.veryAntiAlien ? 20 : 15, tinationState, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					foreach (TIFactionState tifactionState3 in GameStateManager.AllHumanFactions())
					{
						if (!tifactionState3.veryProAlien)
						{
							TIFactionGoalState tifactionGoalState2 = tifactionState2.FindGoals(GoalType.WarOnFaction, tifactionState2, tifactionState3, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>();
							if (tifactionGoalState2 != null)
							{
								tifactionState2.SetFactionHate(tifactionState3, 9f, true, "Alien Nation Created -- Peacemaking");
								tifactionGoalState2.SetImportance(0);
							}
							else
							{
								tifactionState2.SetFactionHate(tifactionState3, Mathf.Min(tifactionState2.GetFactionHate(tifactionState3) - 10f, 8f), true, "Alien Nation Created -- Detente");
							}
						}
					}
				}
				else if (tifactionState2.IsAlienProxy)
				{
					tifactionState2.AddGoal(new FactionGoal_SupportNation(tifactionState2, 20, tinationState), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
			}
			tifactionState.AddGoal(new FactionGoal_SecureEarthSpace(tifactionState, 19), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			TIFactionState.CompleteMilestoneForAllHumanFactions(CampaignMilestone.AlienNationWasFounded);
		}

		// Token: 0x060049D5 RID: 18901 RVA: 0x001EF958 File Offset: 0x001EDB58
		public static bool AI_ShouldAbortBadMission(TIMissionState mission)
		{
			TICouncilorState councilor = mission.councilor;
			TIMissionTemplate missionTemplate = mission.missionTemplate;
			if (!TIGameState.Valid(mission.target) || !missionTemplate.target.ValidTarget(missionTemplate.target.ValidateSingleTarget(missionTemplate, councilor, mission.target)))
			{
				return true;
			}
			if (councilor.activeMission.GetSuccessChance() < 0.15f * councilor.faction.aiValues.riskAversion && (councilor.activeMission.missionTemplate.primaryResource == FactionResource.Influence || councilor.activeMission.missionTemplate.primaryResource == FactionResource.Operations) && councilor.activeMission.resources >= Mathf.Max(32f, councilor.faction.GetCurrentResourceAmount(councilor.activeMission.missionTemplate.primaryResource) * 0.5f))
			{
				return true;
			}
			using (List<TIFactionGoalState>.Enumerator enumerator = councilor.faction.factionGoals.Values.SelectMany<List<TIFactionGoalState>, TIFactionGoalState>((List<TIFactionGoalState> x) => x.Where<TIFactionGoalState>((TIFactionGoalState y) => y is FactionGoal_FriendlyRelations)).ToList<TIFactionGoalState>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (((FactionGoal_FriendlyRelations)enumerator.Current).CheckAbortMissionForViolationOfPact(councilor))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060049D6 RID: 18902 RVA: 0x001EFAAC File Offset: 0x001EDCAC
		public static bool AI_ShouldAvoidDoublingUpMissionTarget(TICouncilorState setCouncilor, TIMissionTemplate setMission, TIGameState setTarget, float setSuccessChance, TICouncilorState councilor, TIMissionTemplate missionTemplate, TIGameState target)
		{
			bool flag = false;
			if (setTarget == target)
			{
				if (councilor.isAlien && setMission == TIFactionState.adviseMission)
				{
					flag = true;
				}
				else if (setMission == missionTemplate)
				{
					if (missionTemplate == TIFactionState.enthrallNonAlignedElitesMission || missionTemplate == TIFactionState.controlNationMission)
					{
						flag = target.ref_nation.wars.Any<TINationState>((TINationState x) => x.numNuclearWeapons > 0 && x.executiveFaction != setCouncilor.faction) || (target.ref_nation.NumNativeControlPoints == 1 && setSuccessChance >= 0.5f);
					}
					if (!missionTemplate.AIDoubleUpAllowed)
					{
						flag = true;
					}
					else if (missionTemplate == TIFactionState.stabilizeMission && (double)target.ref_nation.unrest <= 1.5)
					{
						flag = true;
					}
				}
				else if (missionTemplate == TIFactionState.assassinateMission && (setMission == TIFactionState.turnMission || setMission == TIFactionState.detainMission))
				{
					flag = true;
				}
				else if (missionTemplate == TIFactionState.turnMission && (setMission == TIFactionState.assassinateMission || setMission == TIFactionState.detainMission))
				{
					flag = true;
				}
				else if (missionTemplate == TIFactionState.detainMission && (setMission == TIFactionState.assassinateMission || setMission == TIFactionState.turnMission))
				{
					flag = true;
				}
			}
			else if (missionTemplate == TIFactionState.protectMission && setMission == TIFactionState.protectMission && setTarget == councilor && target == setCouncilor)
			{
				flag = true;
			}
			else if (setMission == TIFactionState.contactMission && setTarget.ref_faction == target.ref_faction)
			{
				flag = true;
			}
			else if (setTarget.ref_nation == target.ref_nation)
			{
				if (missionTemplate == TIFactionState.coupMission)
				{
					if (setMission == TIFactionState.stabilizeMission || setMission == TIFactionState.adviseMission || setMission == TIFactionState.unrestMission || setMission == TIFactionState.terrorizeMission)
					{
						flag = true;
					}
				}
				else if (missionTemplate == TIFactionState.enthrallElitesMission || missionTemplate == TIFactionState.purgeMission || missionTemplate == TIFactionState.enthrallNonAlignedElitesMission || missionTemplate == TIFactionState.controlNationMission)
				{
					if (setMission == TIFactionState.unrestMission || setMission == TIFactionState.terrorizeMission)
					{
						flag = true;
					}
				}
				else if (missionTemplate == TIFactionState.terrorizeMission)
				{
					if (setMission == TIFactionState.enthrallElitesMission || setMission == TIFactionState.enthrallNonAlignedElitesMission || setMission == TIFactionState.unrestMission || setMission == TIFactionState.coupMission)
					{
						flag = true;
					}
				}
				else if (missionTemplate == TIFactionState.stabilizeMission)
				{
					if (setMission == TIFactionState.coupMission || setMission == TIFactionState.unrestMission)
					{
						flag = true;
					}
				}
				else if (missionTemplate == TIFactionState.unrestMission && (setMission == TIFactionState.stabilizeMission || setMission == TIFactionState.coupMission || setMission == TIFactionState.purgeMission || setMission == TIFactionState.enthrallElitesMission || setMission == TIFactionState.enthrallNonAlignedElitesMission || setMission == TIFactionState.controlNationMission))
				{
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x060049D7 RID: 18903 RVA: 0x001EFD50 File Offset: 0x001EDF50
		public static void ComputeFactionStengthEstimates()
		{
			float num = -1f;
			if (AIEvaluators.factionStrengthEstimatesCachedDate != null)
			{
				num = (float)(TITimeState.Now() - AIEvaluators.factionStrengthEstimatesCachedDate).TotalDays;
			}
			if (num >= 0f && num <= (float)AIEvaluators.factionStrengthEstimates_CacheWaitTime_d)
			{
				return;
			}
			AIEvaluators.cachedFactionStrengthEstimates = new Dictionary<TIFactionState, float>();
			AIEvaluators.cachedFactionStrengthEstimates_SpaceOnly = new Dictionary<TIFactionState, float>();
			float num2 = Mathf.Max(1f, GameStateManager.AllHumanFactions().Sum<TIFactionState>((TIFactionState x) => x.fleets.Sum<TISpaceFleetState>((TISpaceFleetState y) => y.SpaceCombatValue())));
			float num3 = (float)TIGlobalValuesState.globalGDP;
			float num4 = Mathf.Max(1f, GameStateManager.AllHumanFactions().Sum<TIFactionState>((TIFactionState x) => x.GetDailyIncome(FactionResource.Research, false, false)));
			float num5 = (float)Mathf.Max(1, -(from x in GameStateManager.IterateByClass<TIHabState>(false)
				where !x.IsAlien()
				select x).Sum<TIHabState>((TIHabState x) => x.coreModule.moduleTemplate.missionControl));
			TIFactionState[] array = GameStateManager.AllHumanFactions();
			for (int i = 0; i < array.Length; i++)
			{
				TIFactionState faction = array[i];
				float num6 = faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) / num2;
				Func<TIControlPoint, bool> <>9__9;
				float num7 = GameStateManager.AllExtantNations().Sum<TINationState>(delegate(TINationState x)
				{
					float num12 = (float)x.GDP;
					IEnumerable<TIControlPoint> controlPoints = x.controlPoints;
					Func<TIControlPoint, bool> func;
					if ((func = <>9__9) == null)
					{
						func = (<>9__9 = (TIControlPoint y) => faction.permanentAlly(y.faction));
					}
					return num12 * (float)controlPoints.Count<TIControlPoint>(func) / (float)x.controlPoints.Count;
				}) / num3;
				float num8 = (float)faction.habs.Sum<TIHabState>((TIHabState x) => -x.coreModule.moduleTemplate.missionControl) / num5;
				float num9 = faction.GetDailyIncome(FactionResource.Research, false, false) / num4;
				if (faction.IsAlienProxy)
				{
					float num10 = AIEvaluators.SystemFleetStrengths.Where<KeyValuePair<TISpaceObjectState, Dictionary<TIFactionState, float>>>((KeyValuePair<TISpaceObjectState, Dictionary<TIFactionState, float>> x) => x.Key.isEarth).Sum<KeyValuePair<TISpaceObjectState, Dictionary<TIFactionState, float>>>((KeyValuePair<TISpaceObjectState, Dictionary<TIFactionState, float>> x) => x.Value.Where<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> y) => y.Key.IsAlienFaction).Sum<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> y) => y.Value));
					float num11 = GameStateManager.AlienFaction().fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue());
					num6 += 0.25f * (num10 + 0.1f * (num11 - num10)) / num2;
				}
				AIEvaluators.cachedFactionStrengthEstimates[faction] = num6 + num7 + num8 + num9;
				AIEvaluators.cachedFactionStrengthEstimates_SpaceOnly[faction] = num6 + num8;
				TIHistoricalData.Record(faction, "Strength", AIEvaluators.cachedFactionStrengthEstimates[faction], 0f, true);
			}
			TIHistoricalData.Record(GameStateManager.AlienFaction(), "Strength", GameStateManager.AlienFaction().fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) / num2, 0f, true);
			AIEvaluators.factionStrengthEstimatesCachedDate = TITimeState.Now();
			AIEvaluators.factionStrengthEstimates_CacheWaitTime_d = 12 + Enumerable.Range(0, 7).SelectRandomItem<int>();
		}

		// Token: 0x060049D8 RID: 18904 RVA: 0x001F00AC File Offset: 0x001EE2AC
		public static float GetFactionStrengthEstimate(this TIFactionState faction)
		{
			AIEvaluators.ComputeFactionStengthEstimates();
			float num;
			if (AIEvaluators.cachedFactionStrengthEstimates.TryGetValue(faction, out num))
			{
				return num;
			}
			return 0.001f;
		}

		// Token: 0x060049D9 RID: 18905 RVA: 0x001F00D4 File Offset: 0x001EE2D4
		public static float GetRelativeHumanStrengthEstimate(this TIFactionState faction, TIFactionState otherFaction)
		{
			AIEvaluators.ComputeFactionStengthEstimates();
			float num;
			float num2;
			if (AIEvaluators.cachedFactionStrengthEstimates.TryGetValue(faction, out num) && AIEvaluators.cachedFactionStrengthEstimates.TryGetValue(otherFaction, out num2))
			{
				if (num2 != 0f)
				{
					return num / num2;
				}
				if (num > 0f)
				{
					return 10f;
				}
				if (num < 0f)
				{
					return -10f;
				}
			}
			return 1f;
		}

		// Token: 0x060049DA RID: 18906 RVA: 0x001F0134 File Offset: 0x001EE334
		public static TIFactionState GetStrongestHumanFaction(Func<TIFactionState, bool> Predicate = null)
		{
			AIEvaluators.ComputeFactionStengthEstimates();
			if (Predicate == null)
			{
				Predicate = (TIFactionState x) => true;
			}
			return AIEvaluators.cachedFactionStrengthEstimates.Where<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => Predicate(x.Key)).MaxBy<KeyValuePair<TIFactionState, float>, float>((KeyValuePair<TIFactionState, float> x) => x.Value).Key;
		}

		// Token: 0x060049DB RID: 18907 RVA: 0x001F01C2 File Offset: 0x001EE3C2
		public static TIFactionState GetMostThreateningEnemyHumanFaction(this TIFactionState faction)
		{
			return AIEvaluators.GetStrongestHumanFaction((TIFactionState x) => !x.permanentAlly(faction) && (!faction.IsAlienFaction || !x.isAlienAppeaser));
		}

		// Token: 0x060049DC RID: 18908 RVA: 0x001F01E0 File Offset: 0x001EE3E0
		public static TIFactionState GetMostThreateningWarEnemyHumanFaction(this TIFactionState faction)
		{
			return AIEvaluators.GetStrongestHumanFaction((TIFactionState x) => faction.enemyWarFactions.Contains(x));
		}

		// Token: 0x060049DD RID: 18909 RVA: 0x001F0200 File Offset: 0x001EE400
		public static float GetFactionStrengthEstimate_SpaceOnly(this TIFactionState faction)
		{
			AIEvaluators.ComputeFactionStengthEstimates();
			float num;
			if (AIEvaluators.cachedFactionStrengthEstimates_SpaceOnly.TryGetValue(faction, out num))
			{
				return num;
			}
			return 0.001f;
		}

		// Token: 0x060049DE RID: 18910 RVA: 0x001F0228 File Offset: 0x001EE428
		public static bool HumanFactionTooBeatDownToContinue(TIFactionState humanAIFaction, TIFactionState enemyFaction = null)
		{
			if (humanAIFaction.veryAntiAlien || humanAIFaction.currentlyCapturingHydra || humanAIFaction.currentlyHuntingHydraToKill)
			{
				TIFactionState enemyFaction2 = enemyFaction;
				if (enemyFaction2 != null && enemyFaction2.IsAlienFaction)
				{
					return false;
				}
			}
			if (humanAIFaction.IsAlienProxy && GameStateManager.AlienNation().extant)
			{
				return false;
			}
			if (!humanAIFaction.unlockedVictoryObjective)
			{
				TIFactionState enemyFaction3 = enemyFaction;
				if (enemyFaction3 == null || !enemyFaction3.unlockedVictoryObjective)
				{
					if (humanAIFaction.selfAssessement <= FactionSelfAssessment.Losing)
					{
						if (enemyFaction != null)
						{
							if (enemyFaction.IsActiveHumanFaction && humanAIFaction.GetRelativeHumanStrengthEstimate(enemyFaction) < 0.5f)
							{
								return true;
							}
							if (humanAIFaction.habs.Count == 0 && humanAIFaction.fleets.Count == 0 && enemyFaction.habs.Count > 0 && enemyFaction.fleets.Count > 0 && humanAIFaction.HabDestructionLog.Count<TIFactionState.HabDestructionLogEntry>((TIFactionState.HabDestructionLogEntry x) => x.Destroyer == enemyFaction && TITimeState.Now().DifferenceInJulianYears(x.Date) < 2.0) > 3)
							{
								return true;
							}
						}
						else if (humanAIFaction.habs.Count == 0 && humanAIFaction.fleets.Count == 0)
						{
							if (humanAIFaction.HabDestructionLog.Count<TIFactionState.HabDestructionLogEntry>((TIFactionState.HabDestructionLogEntry x) => TITimeState.Now().DifferenceInJulianYears(x.Date) < 2.0) > 3)
							{
								if (GameStateManager.AllHumanFactions().Except<TIFactionState>(new List<TIFactionState> { humanAIFaction }).All<TIFactionState>((TIFactionState x) => x.habs.Count > 0 && x.fleets.Count > 0))
								{
									return true;
								}
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x060049DF RID: 18911 RVA: 0x001F03D4 File Offset: 0x001EE5D4
		public static int GetWillingnessToTradeTruce(TIFactionState faction, TIFactionState otherFaction, bool checkOtherFaction)
		{
			if (faction.CanTradeTruce(otherFaction))
			{
				if (faction.player.isAI && AIDailyFactionPlanner.JealousyAndDeescalation(faction, otherFaction, false, false) > 0f && !AIEvaluators.HumanFactionTooBeatDownToContinue(faction, otherFaction))
				{
					return -1;
				}
				if ((!faction.proAlien && otherFaction.IsAlienFaction) || (faction.IsAlienFaction && otherFaction.proAlien))
				{
					return -1;
				}
				if (otherFaction.IsAlienFaction && (faction.currentlyCapturingHydra || faction.currentlyHuntingHydraToKill))
				{
					return -1;
				}
				if (otherFaction.IsActiveHumanFaction && otherFaction.primaryHab != null && otherFaction.unlockedVictoryObjective && ((faction.IsActiveHumanFaction && otherFaction.proAlien) || (faction.veryProAlien && !otherFaction.proAlien)))
				{
					return -1;
				}
				int num = 0;
				int num2 = (checkOtherFaction ? 0 : 1);
				switch (faction.selfAssessement)
				{
				case FactionSelfAssessment.LosingBig:
					num = 2;
					break;
				case FactionSelfAssessment.Losing:
					num = 1;
					break;
				case FactionSelfAssessment.Even:
					if (faction.enemyWarFactions.Count > 2 && faction.mostPowerfulHumanEnemy == otherFaction)
					{
						num = 1;
					}
					break;
				}
				FactionSelfAssessment selfAssessement = otherFaction.selfAssessement;
				if (selfAssessement != FactionSelfAssessment.LosingBig)
				{
					if (selfAssessement == FactionSelfAssessment.Losing)
					{
						num2 = 1;
					}
				}
				else
				{
					num2 = 2;
				}
				if (num > 0 && num2 > 0)
				{
					return num + num2;
				}
			}
			return -1;
		}

		// Token: 0x060049E0 RID: 18912 RVA: 0x001F0504 File Offset: 0x001EE704
		public static int GetWillingnessToTradeNAP(TIFactionState faction, TIFactionState otherFaction, bool checkOtherFaction)
		{
			if (faction.CanTradeNAP(otherFaction))
			{
				if (Mathf.Abs(faction.ideologyCoordinates.x - otherFaction.ideologyCoordinates.x) > 2f && !faction.malleable)
				{
					return -1;
				}
				if ((!faction.antiAlien && otherFaction.IsAlienFaction) || (faction.IsAlienFaction && otherFaction.antiAlien))
				{
					return -1;
				}
				if (otherFaction.IsAlienFaction && (faction.currentlyCapturingHydra || faction.currentlyHuntingHydraToKill))
				{
					return -1;
				}
				if (faction.permanentAlly(otherFaction) || otherFaction.permanentAlly(faction))
				{
					return 2;
				}
				if (faction.IsAlienFaction)
				{
					if (otherFaction.currentlyCapturingHydra || otherFaction.currentlyHuntingHydraToKill || (otherFaction.factionAssassinations.ContainsKey(faction) && otherFaction.factionAssassinations[faction] > 0))
					{
						return -1;
					}
					if (otherFaction.isAlienAppeaser && (otherFaction.selfAssessement > GameStateManager.AlienProxy().selfAssessement || otherFaction.selfAssessement >= faction.selfAssessement || otherFaction.unlockedVictoryObjective))
					{
						return 2;
					}
				}
				else if (faction.isAlienAppeaser && otherFaction.IsAlienFaction)
				{
					return 2;
				}
				int num = 0;
				int num2 = (checkOtherFaction ? 0 : 1);
				FactionSelfAssessment factionSelfAssessment = faction.selfAssessement;
				if (factionSelfAssessment != FactionSelfAssessment.LosingBig)
				{
					if (factionSelfAssessment == FactionSelfAssessment.Losing)
					{
						num = 1;
					}
				}
				else
				{
					num = 2;
				}
				if (checkOtherFaction)
				{
					factionSelfAssessment = otherFaction.selfAssessement;
					if (factionSelfAssessment != FactionSelfAssessment.LosingBig)
					{
						if (factionSelfAssessment == FactionSelfAssessment.Losing)
						{
							num2 = 1;
						}
					}
					else
					{
						num2 = 2;
					}
				}
				if (num > 0 && num2 > 0)
				{
					return num + num2;
				}
			}
			return -1;
		}

		// Token: 0x060049E1 RID: 18913 RVA: 0x001F0660 File Offset: 0x001EE860
		public static int GetWillingnessToShareIntel(TIFactionState AIfaction, TIFactionState otherFaction, bool checkOtherFaction, bool ignoreExistingAgreement = false)
		{
			AIEvaluators.<>c__DisplayClass248_0 CS$<>8__locals1 = new AIEvaluators.<>c__DisplayClass248_0();
			CS$<>8__locals1.AIfaction = AIfaction;
			CS$<>8__locals1.otherFaction = otherFaction;
			if (CS$<>8__locals1.AIfaction.CanTradeIntelSharing(CS$<>8__locals1.otherFaction, ignoreExistingAgreement))
			{
				int num = (checkOtherFaction ? 0 : 1);
				bool flag = false;
				if (CS$<>8__locals1.AIfaction.malleable || (CS$<>8__locals1.AIfaction.proAlien && CS$<>8__locals1.otherFaction.proAlien) || (CS$<>8__locals1.AIfaction.antiAlien && CS$<>8__locals1.otherFaction.antiAlien))
				{
					if (CS$<>8__locals1.AIfaction.permanentAlly(CS$<>8__locals1.otherFaction))
					{
						num = 2;
					}
					else
					{
						if (CS$<>8__locals1.AIfaction.IsAlienProxy && !CS$<>8__locals1.otherFaction.IsAlienFaction && CS$<>8__locals1.AIfaction.intelSharingFactions.Contains(GameStateManager.AlienFaction()) && CS$<>8__locals1.otherFaction.councilors.Any<TICouncilorState>((TICouncilorState x) => CS$<>8__locals1.AIfaction.GetViewofCouncilor(x).GetAttribute(CouncilorAttribute.Loyalty) < 20f))
						{
							return -1;
						}
						if (CS$<>8__locals1.AIfaction.IsAlienFaction && !CS$<>8__locals1.otherFaction.IsAlienProxy && CS$<>8__locals1.otherFaction.councilors.Any<TICouncilorState>((TICouncilorState x) => CS$<>8__locals1.AIfaction.GetViewofCouncilor(x).GetAttribute(CouncilorAttribute.Loyalty) < 20f))
						{
							return -1;
						}
						if (CS$<>8__locals1.AIfaction.proAlien)
						{
							if (CS$<>8__locals1.otherFaction.intelSharingFactions.Any<TIFactionState>((TIFactionState x) => x.veryAntiAlien))
							{
								return -1;
							}
						}
						if (CS$<>8__locals1.AIfaction.antiAlien)
						{
							if (CS$<>8__locals1.otherFaction.intelSharingFactions.Any<TIFactionState>((TIFactionState x) => x.veryProAlien))
							{
								return -1;
							}
						}
						if (CS$<>8__locals1.otherFaction.proAlien)
						{
							if (CS$<>8__locals1.AIfaction.intelSharingFactions.Any<TIFactionState>((TIFactionState x) => x.veryAntiAlien))
							{
								return -1;
							}
						}
						if (CS$<>8__locals1.otherFaction.antiAlien)
						{
							if (CS$<>8__locals1.AIfaction.intelSharingFactions.Any<TIFactionState>((TIFactionState x) => x.veryProAlien))
							{
								return -1;
							}
						}
						if (TINationState.GetIdeologicalDistance(CS$<>8__locals1.AIfaction, CS$<>8__locals1.otherFaction) <= 1.12f)
						{
							flag = true;
						}
					}
					int num2 = CS$<>8__locals1.<GetWillingnessToShareIntel>g__AssessRelativeFactionNeeds|6(CS$<>8__locals1.AIfaction, CS$<>8__locals1.otherFaction, flag);
					if (checkOtherFaction)
					{
						num = CS$<>8__locals1.<GetWillingnessToShareIntel>g__AssessRelativeFactionNeeds|6(CS$<>8__locals1.otherFaction, CS$<>8__locals1.AIfaction, flag);
					}
					if (num2 > 0 && num > 0)
					{
						return num2 + num;
					}
				}
			}
			return -1;
		}

		// Token: 0x060049E2 RID: 18914 RVA: 0x001F08E4 File Offset: 0x001EEAE4
		public static float GetWillingnessToTradeTreaty(TIFactionState faction, TIFactionState otherFaction, TradeOffer.TreatyType treatyType)
		{
			switch (treatyType)
			{
			case TradeOffer.TreatyType.Truce:
				return (float)AIEvaluators.GetWillingnessToTradeTruce(faction, otherFaction, false);
			case TradeOffer.TreatyType.NAP:
				return (float)AIEvaluators.GetWillingnessToTradeNAP(faction, otherFaction, false);
			case TradeOffer.TreatyType.Intel:
				return (float)AIEvaluators.GetWillingnessToShareIntel(faction, otherFaction, false, false);
			default:
				return 0f;
			}
		}

		// Token: 0x060049E4 RID: 18916 RVA: 0x001F0A7C File Offset: 0x001EEC7C
		[CompilerGenerated]
		internal static float <EvaluateHabModule_PercentChange>g__GetPercentChange|78_4(float numerator, float denominator)
		{
			float num;
			if (numerator == 0f)
			{
				num = 0f;
			}
			else if (denominator == 0f || numerator + denominator < 0f != denominator < 0f)
			{
				num = float.PositiveInfinity;
			}
			else if (numerator >= 0f == denominator >= 0f)
			{
				num = 100f * numerator / denominator;
			}
			else
			{
				num = 100f * (denominator / (denominator + numerator) - 1f);
			}
			if (numerator < 0f)
			{
				num *= -1f;
			}
			return num;
		}

		// Token: 0x060049E5 RID: 18917 RVA: 0x001F0B04 File Offset: 0x001EED04
		[CompilerGenerated]
		internal static bool <EvaluateHabModule_Strategy>g__IsBeyondEarthAndNotMars|83_3(TIGameState x)
		{
			return x.ref_system.semiMajorAxis_AU > GameStateManager.Earth().semiMajorAxis_AU && x.ref_system != GameStateManager.Mars();
		}

		// Token: 0x060049E6 RID: 18918 RVA: 0x001F0B2F File Offset: 0x001EED2F
		[CompilerGenerated]
		internal static float <GetSpaceResourceIncomesChecklist>g__GetMinimumIncomePerMonth|115_0(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Water:
			case FactionResource.Volatiles:
				return 30f;
			case FactionResource.Metals:
				return 20f;
			case FactionResource.NobleMetals:
				return 8f;
			case FactionResource.Fissiles:
				return 2f;
			default:
				return 0f;
			}
		}

		// Token: 0x060049E7 RID: 18919 RVA: 0x001F0B6C File Offset: 0x001EED6C
		[CompilerGenerated]
		internal static float <GetSpaceResourceIncomesChecklist>g__GetRecommendedIncomePerMonth|115_1(FactionResource resource)
		{
			return AIEvaluators.<GetSpaceResourceIncomesChecklist>g__GetMinimumIncomePerMonth|115_0(resource) * 3f;
		}

		// Token: 0x060049E8 RID: 18920 RVA: 0x001F0B7A File Offset: 0x001EED7A
		[CompilerGenerated]
		internal static float <GetSpaceResourceIncomesChecklist>g__GetGoodIncomePerMonth|115_2(FactionResource resource)
		{
			return AIEvaluators.<GetSpaceResourceIncomesChecklist>g__GetRecommendedIncomePerMonth|115_1(resource) * 3f;
		}

		// Token: 0x060049E9 RID: 18921 RVA: 0x001F0B88 File Offset: 0x001EED88
		[CompilerGenerated]
		internal static void <get_SystemFleetStrengths>g__AddStrength|169_0(TIFactionState faction, TIGameState location, float strength)
		{
			TISpaceBodyState futureSystem = location.GetFutureSystem();
			if (futureSystem == null)
			{
				return;
			}
			if (!AIEvaluators.cachedSystemFleetStrengths.Keys.Contains(futureSystem))
			{
				AIEvaluators.cachedSystemFleetStrengths[futureSystem] = new Dictionary<TIFactionState, float>();
			}
			if (!AIEvaluators.cachedSystemFleetStrengths[futureSystem].ContainsKey(faction))
			{
				AIEvaluators.cachedSystemFleetStrengths[futureSystem][faction] = 0f;
			}
			Dictionary<TIFactionState, float> dictionary = AIEvaluators.cachedSystemFleetStrengths[futureSystem];
			dictionary[faction] += strength;
		}

		// Token: 0x04002AD8 RID: 10968
		public const float controlEmptyCPUtility = 3f;

		// Token: 0x04002AD9 RID: 10969
		public const float baseControlPointUtility = 25f;

		// Token: 0x04002ADA RID: 10970
		public const float investmentPointUtility = 10f;

		// Token: 0x04002ADB RID: 10971
		public const float spaceFlightProgramUtility = 100f;

		// Token: 0x04002ADC RID: 10972
		public const float unrestsquaredUtility = -4f;

		// Token: 0x04002ADD RID: 10973
		public const float nuclearWeaponsProgramUtility = 50f;

		// Token: 0x04002ADE RID: 10974
		public const float ideologicalDistanceUtility = 3f;

		// Token: 0x04002ADF RID: 10975
		public const float resultingControlUtility = 5f;

		// Token: 0x04002AE0 RID: 10976
		public const float executiveControlPointUtilityMultiplier = 2f;

		// Token: 0x04002AE1 RID: 10977
		public const float armyUtility = 25f;

		// Token: 0x04002AE2 RID: 10978
		public const float miltechUtility = 1.5f;

		// Token: 0x04002AE3 RID: 10979
		public const float orbitalDefensesUtility = 1000f;

		// Token: 0x04002AE4 RID: 10980
		public const float terrorizingForControlPointRelativeUtility = 0.6f;

		// Token: 0x04002AE5 RID: 10981
		public const float popularityRelativeUtility = 1E-05f;

		// Token: 0x04002AE6 RID: 10982
		public const float coupUtility = 0.35f;

		// Token: 0x04002AE7 RID: 10983
		public const float revolutionUtility = 0.05f;

		// Token: 0x04002AE8 RID: 10984
		public const float CaptureNationGoalWeight = 500f;

		// Token: 0x04002AE9 RID: 10985
		public const float armyBadlyDamaged = 0.65f;

		// Token: 0x04002AEA RID: 10986
		public const float armyCriticallyDamaged = 0.5f;

		// Token: 0x04002AEB RID: 10987
		private static readonly Dictionary<FactionResource, float> _AIRelativeValuation = new Dictionary<FactionResource, float>
		{
			{
				FactionResource.Money,
				5f
			},
			{
				FactionResource.Influence,
				10f
			},
			{
				FactionResource.Operations,
				10f
			},
			{
				FactionResource.Research,
				15f
			},
			{
				FactionResource.Boost,
				30f
			},
			{
				FactionResource.MissionControl,
				200f
			},
			{
				FactionResource.Projects,
				1000f
			},
			{
				FactionResource.Antimatter,
				500f
			},
			{
				FactionResource.Exotics,
				500f
			},
			{
				FactionResource.Water,
				5f
			},
			{
				FactionResource.Volatiles,
				5f
			},
			{
				FactionResource.Metals,
				5f
			},
			{
				FactionResource.NobleMetals,
				20f
			},
			{
				FactionResource.Fissiles,
				50f
			}
		};

		// Token: 0x04002AEC RID: 10988
		public const float DeficientAdjustment = 3f;

		// Token: 0x04002AED RID: 10989
		private const float notShipBuildingPenalty = 0.05f;

		// Token: 0x04002AEE RID: 10990
		private static Dictionary<TIFactionState, Dictionary<TIGenericTechTemplate, int>> cachedTechTiers = new Dictionary<TIFactionState, Dictionary<TIGenericTechTemplate, int>>();

		// Token: 0x04002AEF RID: 10991
		private static TIDateTime techTiersCacheDate = null;

		// Token: 0x04002AF0 RID: 10992
		private static Dictionary<TIFactionState, HashSet<TIProjectTemplate>> obsoleteProjects = new Dictionary<TIFactionState, HashSet<TIProjectTemplate>>();

		// Token: 0x04002AF1 RID: 10993
		private const float improvedShipModuleModifier = 30f;

		// Token: 0x04002AF2 RID: 10994
		private const float newHabModuleModifier = 250f;

		// Token: 0x04002AF3 RID: 10995
		private const float criticalModifier = 1000f;

		// Token: 0x04002AF4 RID: 10996
		private const float forcedModifier = 1000f;

		// Token: 0x04002AF5 RID: 10997
		private const float coreModifier = 10000000f;

		// Token: 0x04002AF6 RID: 10998
		private static Dictionary<FactionResource, float> cachedPlannedNetIncomeFromHab = new Dictionary<FactionResource, float>();

		// Token: 0x04002AF7 RID: 10999
		private static Dictionary<FactionResource, float> cachedPlannedRevenueFromHab = new Dictionary<FactionResource, float>();

		// Token: 0x04002AF8 RID: 11000
		private static Dictionary<TechCategory, float> cachedNonHabCategoryBonuses = new Dictionary<TechCategory, float>();

		// Token: 0x04002AF9 RID: 11001
		private static Dictionary<TechCategory, float> cachedProspectiveHabCategoryBonuses = new Dictionary<TechCategory, float>();

		// Token: 0x04002AFA RID: 11002
		[TupleElementNames(new string[] { "IsInsecure", "CachedDate" })]
		private static Dictionary<TIFactionState, Dictionary<FactionResource, Dictionary<AIEvaluators.UpkeepInsecurityType, ValueTuple<bool, TIDateTime>>>> upkeepInsecurityCache;

		// Token: 0x04002AFB RID: 11003
		[TupleElementNames(new string[] { "Value", "Date" })]
		private static Dictionary<TIFactionState, ValueTuple<FactionResource, TIDateTime>> cachedCriticalResources = new Dictionary<TIFactionState, ValueTuple<FactionResource, TIDateTime>>();

		// Token: 0x04002AFC RID: 11004
		private static TIHabModuleTemplate cachedObjectiveHabModuleTemplate;

		// Token: 0x04002AFD RID: 11005
		private static TIObjectiveTemplate cachedObjectiveHabModuleTemplateObjective;

		// Token: 0x04002AFE RID: 11006
		public static float PrimarySystemCampedSoonCutoff_days = 120f;

		// Token: 0x04002AFF RID: 11007
		private static Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>> cachedSystemFleetStrengths = new Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>>();

		// Token: 0x04002B00 RID: 11008
		private static TIDateTime systemFleetStrengthsCachedDate = null;

		// Token: 0x04002B01 RID: 11009
		private static TIDateTime threatLevelCachedDate_all = null;

		// Token: 0x04002B02 RID: 11010
		private static Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>> cachedThreatLevels_all = new Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>>();

		// Token: 0x04002B03 RID: 11011
		private static TIDateTime threatLevelCachedDate_warEnemiesOnly = null;

		// Token: 0x04002B04 RID: 11012
		private static Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>> cachedThreatLevels_warEnemiesOnly = new Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>>();

		// Token: 0x04002B05 RID: 11013
		private static float cachedTypicalSTOFigherSCV;

		// Token: 0x04002B06 RID: 11014
		private static float cachedTypicalSTOFigherBoostCost;

		// Token: 0x04002B07 RID: 11015
		private static TIDateTime typicalSTOFighterCachedDate;

		// Token: 0x04002B08 RID: 11016
		private static float typicalSTOFighter_CacheWaitTime_d = 720f;

		// Token: 0x04002B09 RID: 11017
		private static Dictionary<TIFactionState, float> cachedFactionStrengthEstimates;

		// Token: 0x04002B0A RID: 11018
		private static Dictionary<TIFactionState, float> cachedFactionStrengthEstimates_SpaceOnly;

		// Token: 0x04002B0B RID: 11019
		private static TIDateTime factionStrengthEstimatesCachedDate;

		// Token: 0x04002B0C RID: 11020
		private static int factionStrengthEstimates_CacheWaitTime_d = -1;

		// Token: 0x04002B0D RID: 11021
		public const float MAX_IDEOLOGICAL_X_DISTANCE_FOR_NAP = 2f;

		// Token: 0x04002B0E RID: 11022
		public const float MAX_IDEOLOGICAL_DISTANCE_FOR_INTEL = 1.12f;

		// Token: 0x02000F9C RID: 3996
		public enum MoneySitation
		{
			// Token: 0x04005F4A RID: 24394
			Terrible,
			// Token: 0x04005F4B RID: 24395
			Bad,
			// Token: 0x04005F4C RID: 24396
			Tight,
			// Token: 0x04005F4D RID: 24397
			Ok
		}

		// Token: 0x02000F9D RID: 3997
		public enum UpkeepInsecurityType
		{
			// Token: 0x04005F4F RID: 24399
			Present,
			// Token: 0x04005F50 RID: 24400
			PresentCautious,
			// Token: 0x04005F51 RID: 24401
			Future
		}

		// Token: 0x02000F9E RID: 3998
		public enum HabCapturingLogic
		{
			// Token: 0x04005F53 RID: 24403
			All,
			// Token: 0x04005F54 RID: 24404
			LowEffortHighReward
		}
	}
}
