using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PavonisInteractive.TerraInvicta.Actions;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000946 RID: 2374
	public class HumanHabPlanner : HabPlanner
	{
		// Token: 0x06005AB9 RID: 23225 RVA: 0x002B3D48 File Offset: 0x002B1F48
		private IEnumerable<TISpaceBodyState> GetHighStrategicValueSpaceBodies(TIFactionState faction)
		{
			List<TISpaceBodyState> list = new List<TISpaceBodyState>();
			list.Add(GameStateManager.Jupiter());
			list.Add(GameStateManager.Saturn());
			list.Add(GameStateManager.Uranus());
			return from x in list.SelectMany<TISpaceBodyState, TISpaceBodyState>((TISpaceBodyState x) => x.SpaceBodiesInSystem.Append(x))
				where x.IsSafeForColonization(faction, HabType.Base)
				select x;
		}

		// Token: 0x06005ABA RID: 23226 RVA: 0x002B3DC0 File Offset: 0x002B1FC0
		private void ManageProspectGoals(TIFactionState faction)
		{
			List<TIFactionGoalState> list = faction.GoalsOfType(GoalType.ProspectSites, false, true);
			if (list.Any<TIFactionGoalState>((TIFactionGoalState x) => !x.InProgress()))
			{
				if (TIUtilities.RandomFloatValue() < 0.1f)
				{
					using (List<TIFactionGoalState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIFactionGoalState tifactionGoalState = enumerator.Current;
							tifactionGoalState.SetImportance(0);
						}
						goto IL_0087;
					}
				}
				return;
			}
			IL_0087:
			Dictionary<FactionResource, float> estimatedFutureIncomes = TIResourcesCost.habResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, delegate(FactionResource resource)
			{
				if (TIResourcesCost.basicSpaceResources.Contains(resource))
				{
					return AIEvaluators.EstimateFutureIncomePerMonth(faction, resource, true, true, true);
				}
				return faction.GetMonthlyIncome(resource, true, false);
			});
			HabSchematic arbitraryHabSchematic = faction.HabSchematics.SelectRandomItem<HabSchematic>();
			List<TISpaceBodyState> list2 = GameStateManager.AllSpaceBodies().Where<TISpaceBodyState>(delegate(TISpaceBodyState x)
			{
				if (faction.CandidateForProspecting(x))
				{
					if (x.habSites.Any<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase))
					{
						return x.ref_system.IsSafeForColonization(faction, HabType.Base);
					}
				}
				return false;
			}).ToList<TISpaceBodyState>();
			Dictionary<TISpaceBodyState, float> dictionary = list2.ToDictionary<TISpaceBodyState, TISpaceBodyState, float>((TISpaceBodyState x) => x, delegate(TISpaceBodyState x)
			{
				List<TIResourcesCost> list6 = new LaunchProbeOperation().ResourceCostOptions(faction, x, faction, true);
				if (!list6.Any<TIResourcesCost>())
				{
					return float.PositiveInfinity;
				}
				float num = list6.Min<TIResourcesCost>((TIResourcesCost x) => x.GetSingleCostValue(FactionResource.Boost));
				return AIEvaluators.GetDaysToWaitForRateLimitedBoostPurchase(faction, 0.15f, num);
			});
			List<TISpaceBodyState> list3 = (from x in dictionary
				where x.Value < 182.6211f
				select x.Key).ToList<TISpaceBodyState>();
			if (list3.Count > 0)
			{
				list2 = list3;
			}
			else
			{
				List<TISpaceBodyState> list4 = (from x in dictionary
					where x.Value < 730.4844f
					select x.Key).ToList<TISpaceBodyState>();
				if (list4.Count > 0)
				{
					list2 = list4;
				}
			}
			if (list2.Count == 0)
			{
				return;
			}
			list2 = list2.Where<TISpaceBodyState>((TISpaceBodyState x) => x.habSites.Length > 1).Union<TISpaceBodyState>(list2.Take_Random<TISpaceBodyState>(5 + ((float)list2.Count * 0.1f).Round())).ToList<TISpaceBodyState>();
			List<ValueTuple<TISpaceBodyState, float>> list5 = list2.Select<TISpaceBodyState, ValueTuple<TISpaceBodyState, float>>((TISpaceBodyState x) => new ValueTuple<TISpaceBodyState, float>(x, base.<ManageProspectGoals>g__GetSpaceBodyScore|5(x))).ToList<ValueTuple<TISpaceBodyState, float>>();
			IEnumerable<ValueTuple<TISpaceBodyState, float>> enumerable = list5.Where<ValueTuple<TISpaceBodyState, float>>(([TupleElementNames(new string[] { "SpaceBody", "Score" })] ValueTuple<TISpaceBodyState, float> x) => x.Item2 > 0f);
			TISpaceBodyState tispaceBodyState;
			if (enumerable.Any<ValueTuple<TISpaceBodyState, float>>())
			{
				tispaceBodyState = enumerable.SelectRandomWeightedItem<ValueTuple<TISpaceBodyState, float>>(([TupleElementNames(new string[] { "SpaceBody", "Score" })] ValueTuple<TISpaceBodyState, float> x) => Mathf.Pow(x.Item2, 2f), -1f, 1E-37f).Item1;
			}
			else
			{
				tispaceBodyState = list5.MaxBy<ValueTuple<TISpaceBodyState, float>, float>(([TupleElementNames(new string[] { "SpaceBody", "Score" })] ValueTuple<TISpaceBodyState, float> x) => x.Item2).Item1;
			}
			faction.AddGoal(new FactionGoal_ProspectSites(faction, 15, tispaceBodyState, false, GoalType.None, GoalType.None, GoalType.None), HandleDuplicateGoalRule.ResetImportance, null);
		}

		// Token: 0x06005ABB RID: 23227 RVA: 0x002B40E4 File Offset: 0x002B22E4
		private bool ShouldExpand(TIFactionState faction, HumanHabPlanner.ExpansionType expansionType)
		{
			if (expansionType == PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Station && faction.stations.Count < 4)
			{
				return true;
			}
			bool flag = faction.habs.Any<TIHabState>((TIHabState x) => x.coreModule.CanUpgrade(faction));
			if (expansionType != PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Upgrade && flag)
			{
				return false;
			}
			float num = (float)faction.habs.Count<TIHabState>((TIHabState x) => x.AvailableSlots().Count > 1 || (expansionType != PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Upgrade && x.coreModule.CanUpgrade(faction))) / (float)faction.habs.Count;
			float num2 = 0.15f;
			if (expansionType == PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Upgrade)
			{
				num2 /= 2f;
			}
			if (num > num2)
			{
				return false;
			}
			HumanHabPlanner.ExpansionType expansionType2 = expansionType;
			if (expansionType2 != PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Station)
			{
				if (expansionType2 == PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Renovation)
				{
					if ((float)faction.habs.Count<TIHabState>((TIHabState x) => x.UnderConstructionModules().Any<TIHabModuleState>()) / (float)faction.habs.Count > 0.15f)
					{
						return false;
					}
				}
			}
			else
			{
				float num3 = faction.stations.Sum<TIHabState>((TIHabState x) => -x.GetNetCurrentMonthlyIncome(faction, FactionResource.MissionControl, true, false)) / (float)(faction.MissionControlIncome - faction.GetMissionControlContributionFromHabs());
				if (num3 < 0.07f)
				{
					return true;
				}
				if (faction.GetMoneySituation(0f) > AIEvaluators.MoneySitation.Tight)
				{
					return false;
				}
				if (num3 > 0.2f)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005ABC RID: 23228 RVA: 0x002B425D File Offset: 0x002B245D
		private bool ShouldPerformHabUpgrades(TIFactionState faction)
		{
			return this.ShouldExpand(faction, PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Upgrade);
		}

		// Token: 0x06005ABD RID: 23229 RVA: 0x002B4268 File Offset: 0x002B2468
		private void ManageFoundGoals(TIFactionState faction)
		{
			HumanHabPlanner.<>c__DisplayClass5_0 CS$<>8__locals1 = new HumanHabPlanner.<>c__DisplayClass5_0();
			CS$<>8__locals1.faction = faction;
			bool flag = AIEvaluators.FactionIsWorkingOnHabModuleBasedObjectives(CS$<>8__locals1.faction);
			List<FactionGoal_FoundHab> list = (from x in CS$<>8__locals1.faction.GoalsOfType(TIFactionGoalState.FoundHabGoals, false, true)
				select x as FactionGoal_FoundHab into x
				where !x.InProgress()
				where x.importance > 0
				select x).ToList<FactionGoal_FoundHab>();
			CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Boost);
			CS$<>8__locals1.faction.GetYearlyIncome(FactionResource.Boost, false, false, false);
			list.Where<FactionGoal_FoundHab>((FactionGoal_FoundHab foundGoal) => foundGoal.target() == null || foundGoal.assignedFleet == null || !foundGoal.assignedFleet.CanFulfillGoal(foundGoal, false) || (!foundGoal.assignedFleet.inTransfer && foundGoal.assignedFleet.ref_system != foundGoal.target().ref_system)).ToList<FactionGoal_FoundHab>();
			foreach (FactionGoal_FoundHab factionGoal_FoundHab in list.ToList<FactionGoal_FoundHab>())
			{
				if (TIUtilities.RandomFloatValue() < 0.05f)
				{
					factionGoal_FoundHab.SetImportance(0);
					list.Remove(factionGoal_FoundHab);
				}
			}
			CS$<>8__locals1.foundStationGoals = list.Where<FactionGoal_FoundHab>((FactionGoal_FoundHab x) => x is FactionGoal_FoundStation).ToList<FactionGoal_FoundHab>();
			CS$<>8__locals1.foundBaseGoals = list.Where<FactionGoal_FoundHab>((FactionGoal_FoundHab x) => x is FactionGoal_FoundBase).ToList<FactionGoal_FoundHab>();
			bool flag2 = CS$<>8__locals1.faction.bases.Any<TIHabState>((TIHabState x) => x.ref_spaceBody == GameStateManager.Luna());
			bool flag3;
			if (!flag2)
			{
				flag3 = CS$<>8__locals1.faction.bases.None<TIHabState>((TIHabState x) => x.HasMine);
			}
			else
			{
				flag3 = false;
			}
			bool flag4 = flag3;
			Func<IEnumerable<TIGameState>, Func<TIGameState, float>, TIGameState> func = delegate(IEnumerable<TIGameState> candidates, Func<TIGameState, float> GetScoreModifier)
			{
				Dictionary<float, List<TIGameState>> dictionary2 = (from x in candidates
					group x by Mathf.Max(AIEvaluators.GetSolarEnergyEfficiency(x.ref_naturalSpaceObject), 0.999f)).ToDictionary<IGrouping<float, TIGameState>, float, List<TIGameState>>((IGrouping<float, TIGameState> x) => x.Key, (IGrouping<float, TIGameState> x) => x.ToList<TIGameState>());
				if (dictionary2.Count == 0)
				{
					return null;
				}
				IEnumerable<KeyValuePair<float, List<TIGameState>>> enumerable13 = dictionary2;
				Func<KeyValuePair<float, List<TIGameState>>, HabSchematicOrder> func7;
				if ((func7 = CS$<>8__locals1.<>9__22) == null)
				{
					func7 = (CS$<>8__locals1.<>9__22 = (KeyValuePair<float, List<TIGameState>> x) => HabSchematic.GetOrderWithoutHabSchematic(CS$<>8__locals1.faction, x.Value.SelectRandomItem<TIGameState>(), null));
				}
				return enumerable13.ToDictionary<KeyValuePair<float, List<TIGameState>>, HabSchematicOrder, List<TIGameState>>(func7, (KeyValuePair<float, List<TIGameState>> x) => x.Value).SelectMany<KeyValuePair<HabSchematicOrder, List<TIGameState>>, ValueTuple<TIGameState, HabSchematicOrder>>((KeyValuePair<HabSchematicOrder, List<TIGameState>> x) => x.Value.Select<TIGameState, ValueTuple<TIGameState, HabSchematicOrder>>((TIGameState y) => new ValueTuple<TIGameState, HabSchematicOrder>(y, x.Key))).ToDictionary<ValueTuple<TIGameState, HabSchematicOrder>, TIGameState, float>(([TupleElementNames(new string[] { "Candidate", "Order" })] ValueTuple<TIGameState, HabSchematicOrder> x) => x.Item1, delegate([TupleElementNames(new string[] { "Candidate", "Order" })] ValueTuple<TIGameState, HabSchematicOrder> pair)
				{
					TIGameState item = pair.Item1;
					HabSchematicOrder item2 = pair.Item2;
					if (item.ref_habSite != null)
					{
						if (!item2.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule))
						{
							item2.Add(ArchetypeDecision.HumanOutpostCore);
						}
						if (!item2.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine))
						{
							item2.Add(ArchetypeDecision.HumanOutpostMine);
						}
					}
					float num6 = item2.Score(CS$<>8__locals1.faction, item, null, false, true);
					int maxHabTier = item.ref_naturalSpaceObject.maxHabTier;
					if (maxHabTier == 1)
					{
						num6 *= 0.1f;
					}
					else
					{
						num6 *= 1f + (float)(maxHabTier - 1) / 2f;
					}
					if (GetScoreModifier != null)
					{
						float num7 = GetScoreModifier(item);
						if (num6 < 0f)
						{
							num7 = 1f / num7;
						}
						num6 *= num7;
					}
					return num6;
				})
					.MaxBy<KeyValuePair<TIGameState, float>, float>((KeyValuePair<TIGameState, float> x) => x.Value)
					.Key;
			};
			CS$<>8__locals1.GetDefaultScoreModifier = delegate(TIGameState candidate)
			{
				float singleCostValue = FoundHabOperation.GetCostFromSpace(candidate, CS$<>8__locals1.faction, true).GetSingleCostValue(FactionResource.Boost);
				return 1f / (singleCostValue + 15f);
			};
			Action<TIGameState, int> action = delegate(TIGameState location, int importance)
			{
				if (location == null)
				{
					return;
				}
				FactionGoal_FoundHab factionGoal_FoundHab3;
				if (location.isHabSiteState)
				{
					factionGoal_FoundHab3 = new FactionGoal_FoundBase(CS$<>8__locals1.faction, importance, location.ref_habSite, GoalType.None, null, GoalType.None, false, null);
				}
				else
				{
					factionGoal_FoundHab3 = new FactionGoal_FoundPlatform(CS$<>8__locals1.faction, importance, location.ref_orbit, GoalType.None, null, GoalType.DefendWithFleet);
				}
				factionGoal_FoundHab3 = CS$<>8__locals1.faction.AddGoal(factionGoal_FoundHab3, HandleDuplicateGoalRule.ResetImportance, null) as FactionGoal_FoundHab;
				if (factionGoal_FoundHab3 != null)
				{
					if (location.isHabSiteState)
					{
						CS$<>8__locals1.foundBaseGoals.Add(factionGoal_FoundHab3);
						return;
					}
					CS$<>8__locals1.foundStationGoals.Add(factionGoal_FoundHab3);
				}
			};
			CS$<>8__locals1.favorBoostEfficiencyForBases = CS$<>8__locals1.faction.bases.Count < 4;
			FactionResource criticalBasicSpaceResource = CS$<>8__locals1.faction.GetCriticalBasicSpaceResource();
			CS$<>8__locals1.bonusResources = Enumerable.Empty<FactionResource>().Append(criticalBasicSpaceResource);
			if (CS$<>8__locals1.foundBaseGoals.Where<FactionGoal_FoundHab>((FactionGoal_FoundHab x) => CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(x.location(), false, false)).ToList<FactionGoal_FoundHab>().Count < 1)
			{
				IEnumerable<TIHabSiteState> enumerable = (from x in CS$<>8__locals1.faction.habs.Select<TIHabState, TISpaceBodyState>((TIHabState x) => x.ref_system).Distinct<TISpaceBodyState>()
					where !x.isEarth
					where CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(x, false, false)
					where x.ref_system.IsSafeForColonization(CS$<>8__locals1.faction, HabType.Base)
					select x).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase));
				action(func(enumerable, new Func<TIGameState, float>(CS$<>8__locals1.<ManageFoundGoals>g__GetHabSiteScoreModifier|11)), 17);
			}
			CS$<>8__locals1.specialSpaceBodies = this.GetHighStrategicValueSpaceBodies(CS$<>8__locals1.faction);
			IEnumerable<FactionGoal_FoundHab> enumerable2 = from x in CS$<>8__locals1.foundBaseGoals
				where CS$<>8__locals1.specialSpaceBodies.Contains(x.target().ref_spaceBody)
				where x.importance > 0
				select x;
			foreach (FactionGoal_FoundHab factionGoal_FoundHab2 in enumerable2.ToList<FactionGoal_FoundHab>())
			{
				if (factionGoal_FoundHab2.assignedFleet == null && factionGoal_FoundHab2.PendingShipDataNames().Count == 0)
				{
					factionGoal_FoundHab2.SetImportance(0);
				}
			}
			if (!enumerable2.Any<FactionGoal_FoundHab>())
			{
				using (IEnumerator<IGrouping<TISpaceBodyState, TISpaceBodyState>> enumerator2 = (from x in CS$<>8__locals1.specialSpaceBodies.Where<TISpaceBodyState>(delegate(TISpaceBodyState x)
					{
						if (CS$<>8__locals1.faction.Prospected(x) && x.IsSafeForColonization(CS$<>8__locals1.faction, HabType.Base))
						{
							IEnumerable<TIHabState> habsInSystem = x.ref_system.habsInSystem;
							Func<TIHabState, bool> func8;
							if ((func8 = CS$<>8__locals1.<>9__40) == null)
							{
								func8 = (CS$<>8__locals1.<>9__40 = (TIHabState y) => CS$<>8__locals1.faction.habs.Contains(y));
							}
							return habsInSystem.None<TIHabState>(func8);
						}
						return false;
					})
					group x by x.ref_system into x
					orderby x.Key.semiMajorAxis_AU
					select x).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						IEnumerable<TIHabSiteState> enumerable3 = enumerator2.Current.SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase).Take_Random<TIHabSiteState>(4));
						if (enumerable3.Any<TIHabSiteState>())
						{
							TIGameState tigameState = func(enumerable3, new Func<TIGameState, float>(CS$<>8__locals1.<ManageFoundGoals>g__GetHabSiteScoreModifier|11));
							action(tigameState, 19);
							break;
						}
					}
				}
			}
			int num = 5 + Mathf.Clamp((TITimeState.CampaignDuration_years_Exact() - 5f) / 5f, 0f, 4f).RoundDown();
			bool flag5 = CS$<>8__locals1.faction.bases.Count < num;
			float num2 = (float)CS$<>8__locals1.faction.bases.Count<TIHabState>((TIHabState x) => x.HasInactiveButPowerableMine) / (float)CS$<>8__locals1.faction.bases.Count;
			if (TIResourcesCost.basicSpaceResourcesSansFissiles.Any<FactionResource>((FactionResource x) => CS$<>8__locals1.faction.GetDailyIncome(x, true, false) <= 0f))
			{
				flag5 = true;
			}
			else if (!flag && !flag5 && (CS$<>8__locals1.faction.GetMoneySituation(0f) > AIEvaluators.MoneySitation.Bad || TIUtilities.RandomFloatValue() < 0.04f) && num2 < 0.2f)
			{
				flag5 = CS$<>8__locals1.faction.LaggingInSpaceEconomy();
			}
			List<FactionGoal_FoundHab> list2 = (from x in CS$<>8__locals1.foundBaseGoals.Where<FactionGoal_FoundHab>(delegate(FactionGoal_FoundHab x)
				{
					IEnumerable<TIHabState> habsInSystem2 = x.location().ref_system.habsInSystem;
					Func<TIHabState, bool> func9;
					if ((func9 = CS$<>8__locals1.<>9__43) == null)
					{
						func9 = (CS$<>8__locals1.<>9__43 = (TIHabState x) => x.faction == CS$<>8__locals1.faction);
					}
					return habsInSystem2.None<TIHabState>(func9);
				})
				where x.importance > 0
				select x).Except<FactionGoal_FoundHab>(enumerable2).ToList<FactionGoal_FoundHab>();
			if (flag5 && list2.Count < 1)
			{
				TIHabModuleTemplate coreModule = new FoundOutpostOperation().CoreModule(false);
				IEnumerable<TIHabSiteState> enumerable4 = (from x in CS$<>8__locals1.faction.ProspectedSpaceBodies()
					where CS$<>8__locals1.faction.CanExplore(x.ref_spaceBody)
					where !AIEvaluators.ShouldRateLimitBoostExpenditure(coreModule, CS$<>8__locals1.faction, x)
					where x.ref_system.IsSafeForColonization(CS$<>8__locals1.faction, HabType.Base)
					select x).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase)).ToList<TIHabSiteState>();
				if (flag4)
				{
					IEnumerable<TIHabSiteState> enumerable5 = enumerable4.Where<TIHabSiteState>((TIHabSiteState x) => x.ref_spaceBody == GameStateManager.Luna());
					if (enumerable5.Any<TIHabSiteState>())
					{
						enumerable4 = enumerable5;
					}
					else
					{
						flag4 = false;
					}
				}
				else if (flag2)
				{
					enumerable4 = enumerable4.Where<TIHabSiteState>((TIHabSiteState x) => x.ref_spaceBody != GameStateManager.Luna());
				}
				IEnumerable<TIHabSiteState> enumerable6 = enumerable4.Where<TIHabSiteState>((TIHabSiteState x) => x.fissiles_day > 0.1f);
				TIHabSiteState tihabSiteState = enumerable4.MinBy<TIHabSiteState, double>((TIHabSiteState x) => x.ref_system.semiMajorAxis_AU);
				if (tihabSiteState != null && !enumerable6.Contains(tihabSiteState))
				{
					enumerable6 = enumerable6.Append(tihabSiteState);
				}
				List<TIHabSiteState> list3 = enumerable4.Except<TIHabSiteState>(enumerable6).ToList<TIHabSiteState>();
				int num3 = 10;
				if (list3.Count <= num3)
				{
					enumerable6 = enumerable6.Union<TIHabSiteState>(list3);
				}
				else
				{
					TIHabModuleTemplate mineTemplate = TemplateManager.HabModuleTemplates.First<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine && x.tier == 1 && !x.automated && !x.alienModule);
					Dictionary<TIHabSiteState, float> dictionary = list3.ToDictionary<TIHabSiteState, TIHabSiteState, float>((TIHabSiteState x) => x, (TIHabSiteState site) => AIEvaluators.EvaluateHabModule_PercentChange(CS$<>8__locals1.faction, site, mineTemplate, null, null, null, true, false));
					float minSiteEstimate = dictionary.MinBy<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Value;
					float maxSiteEstimate = dictionary.MaxBy<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Value;
					int num4 = 0;
					Func<KeyValuePair<TIHabSiteState, float>, float> <>9__58;
					while (num4 < num3 && dictionary.Count > 0)
					{
						IEnumerable<KeyValuePair<TIHabSiteState, float>> enumerable7 = dictionary;
						Func<KeyValuePair<TIHabSiteState, float>, float> func2;
						if ((func2 = <>9__58) == null)
						{
							func2 = (<>9__58 = (KeyValuePair<TIHabSiteState, float> x) => x.Value + (maxSiteEstimate - minSiteEstimate) * 0.33f + ((minSiteEstimate <= 0f) ? (-minSiteEstimate + 0.001f) : 0f));
						}
						TIHabSiteState key = enumerable7.SelectRandomWeightedItem<KeyValuePair<TIHabSiteState, float>>(func2, -1f, 1E-37f).Key;
						dictionary.Remove(key);
						enumerable6 = enumerable6.Append(key);
						num4++;
					}
				}
				enumerable6 = enumerable6.Distinct<TIHabSiteState>().ToList<TIHabSiteState>();
				int num5 = 4;
				if (flag4)
				{
					num5 = 1;
				}
				else if (CS$<>8__locals1.faction.bases.Count < 3)
				{
					IEnumerable<FactionResource> crewSupportResources = Enumerable.Empty<FactionResource>().Append(FactionResource.Water).Append(FactionResource.Volatiles);
					Dictionary<TIHabSiteState, Dictionary<FactionResource, TIHabSiteState.Statistics.SpaceResourceGrade>> candidateResourceGrades = enumerable6.ToDictionary<TIHabSiteState, TIHabSiteState, Dictionary<FactionResource, TIHabSiteState.Statistics.SpaceResourceGrade>>((TIHabSiteState x) => x, (TIHabSiteState candidate) => crewSupportResources.ToDictionary<FactionResource, FactionResource, TIHabSiteState.Statistics.SpaceResourceGrade>((FactionResource x) => x, delegate(FactionResource resource)
					{
						float monthlyProduction = candidate.GetMonthlyProduction(resource);
						return TIHabSiteState.Statistics.GetResourceGrade(resource, monthlyProduction);
					}));
					TIHabSiteState.Statistics.SpaceResourceGrade standardMinimumResourceGrade = TIHabSiteState.Statistics.SpaceResourceGrade.BelowAverage;
					TIHabSiteState.Statistics.SpaceResourceGrade multisiteMinimumResourceGrade = standardMinimumResourceGrade;
					Func<TIHabSiteState.Statistics.SpaceResourceGrade, bool> <>9__66;
					if (candidateResourceGrades.None<KeyValuePair<TIHabSiteState, Dictionary<FactionResource, TIHabSiteState.Statistics.SpaceResourceGrade>>>(delegate(KeyValuePair<TIHabSiteState, Dictionary<FactionResource, TIHabSiteState.Statistics.SpaceResourceGrade>> x)
					{
						IEnumerable<TIHabSiteState.Statistics.SpaceResourceGrade> values = x.Value.Values;
						Func<TIHabSiteState.Statistics.SpaceResourceGrade, bool> func10;
						if ((func10 = <>9__66) == null)
						{
							func10 = (<>9__66 = (TIHabSiteState.Statistics.SpaceResourceGrade y) => y >= standardMinimumResourceGrade);
						}
						return values.Any<TIHabSiteState.Statistics.SpaceResourceGrade>(func10);
					}))
					{
						multisiteMinimumResourceGrade = TIHabSiteState.Statistics.SpaceResourceGrade.Poor;
					}
					enumerable6 = enumerable6.Where<TIHabSiteState>(delegate(TIHabSiteState x)
					{
						Dictionary<FactionResource, TIHabSiteState.Statistics.SpaceResourceGrade> dictionary3 = candidateResourceGrades[x];
						TIHabSiteState.Statistics.SpaceResourceGrade minimumResourceGrade = standardMinimumResourceGrade;
						if (x.ref_spaceBody.habSites.Count<TIHabSiteState>() > 1)
						{
							minimumResourceGrade = multisiteMinimumResourceGrade;
						}
						return dictionary3.Values.Any<TIHabSiteState.Statistics.SpaceResourceGrade>((TIHabSiteState.Statistics.SpaceResourceGrade y) => y >= minimumResourceGrade);
					}).ToList<TIHabSiteState>();
					TIHabSiteState.Statistics.SpaceResourceGrade betterResourceGrade = TIHabSiteState.Statistics.SpaceResourceGrade.AboveAverage;
					Func<TIHabSiteState.Statistics.SpaceResourceGrade, bool> <>9__68;
					List<TIHabSiteState> list4 = enumerable6.Where<TIHabSiteState>(delegate(TIHabSiteState x)
					{
						IEnumerable<TIHabSiteState.Statistics.SpaceResourceGrade> values2 = candidateResourceGrades[x].Values;
						Func<TIHabSiteState.Statistics.SpaceResourceGrade, bool> func11;
						if ((func11 = <>9__68) == null)
						{
							func11 = (<>9__68 = (TIHabSiteState.Statistics.SpaceResourceGrade y) => y >= betterResourceGrade);
						}
						return values2.Any<TIHabSiteState.Statistics.SpaceResourceGrade>(func11);
					}).ToList<TIHabSiteState>();
					if (list4.Count > 0)
					{
						enumerable6 = list4;
					}
					num5 = 1;
				}
				if (enumerable6.Count<TIHabSiteState>() >= num5)
				{
					TIGameState tigameState2 = func(enumerable6, new Func<TIGameState, float>(CS$<>8__locals1.<ManageFoundGoals>g__GetHabSiteScoreModifier|11));
					action(tigameState2, 18);
				}
			}
			if (!flag && this.ShouldExpand(CS$<>8__locals1.faction, PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ExpansionType.Station) && CS$<>8__locals1.foundStationGoals.Count < 2)
			{
				bool flag6 = !new FoundOutpostOperation().CoreModule(false).FactionCanBuild(CS$<>8__locals1.faction) || !ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining).Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.automated && x.FactionCanBuild(CS$<>8__locals1.faction));
				IEnumerable<Func<TIOrbitState, bool>> enumerable8 = Enumerable.Empty<Func<TIOrbitState, bool>>().Append((TIOrbitState x) => AIEvaluators.IsEnergyEfficient(x.ref_naturalSpaceObject)).Append((TIOrbitState x) => CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(x, false, false))
					.Append((TIOrbitState x) => !CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(x, false, false));
				if (CS$<>8__locals1.faction.stations.Count<TIHabState>((TIHabState x) => x.ref_system.isEarth) < 2)
				{
					enumerable8 = Enumerable.Empty<Func<TIOrbitState, bool>>().Append((TIOrbitState x) => x.ref_system.isEarth).Concat<Func<TIOrbitState, bool>>(enumerable8);
				}
				using (IEnumerator<Func<TIOrbitState, bool>> enumerator3 = enumerable8.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						HumanHabPlanner.<>c__DisplayClass5_8 CS$<>8__locals5 = new HumanHabPlanner.<>c__DisplayClass5_8();
						CS$<>8__locals5.CS$<>8__locals4 = CS$<>8__locals1;
						CS$<>8__locals5.DoesOrbitMatchPredicate = enumerator3.Current;
						if (!CS$<>8__locals5.CS$<>8__locals4.foundStationGoals.Any<FactionGoal_FoundHab>((FactionGoal_FoundHab x) => CS$<>8__locals5.DoesOrbitMatchPredicate(x.target().ref_orbit)))
						{
							IEnumerable<TISpaceBodyState> enumerable9 = GameStateManager.AllSpaceBodies();
							Func<TISpaceBodyState, bool> func3;
							if ((func3 = CS$<>8__locals5.CS$<>8__locals4.<>9__76) == null)
							{
								func3 = (CS$<>8__locals5.CS$<>8__locals4.<>9__76 = (TISpaceBodyState x) => CS$<>8__locals5.CS$<>8__locals4.faction.CanExplore(x));
							}
							IEnumerable<TISpaceBodyState> enumerable10 = from x in enumerable9.Where<TISpaceBodyState>(func3)
								orderby x.ref_system.semiMajorAxis_AU
								select x;
							Func<TISpaceBodyState, bool> func4;
							if ((func4 = CS$<>8__locals5.CS$<>8__locals4.<>9__78) == null)
							{
								func4 = (CS$<>8__locals5.CS$<>8__locals4.<>9__78 = (TISpaceBodyState x) => x.ref_system.IsSafeForColonization(CS$<>8__locals5.CS$<>8__locals4.faction, HabType.Station));
							}
							List<TIOrbitState> list5 = (from x in enumerable10.Where<TISpaceBodyState>(func4)
								select x.orbits.Where<TIOrbitState>((TIOrbitState y) => y.NewStationAllowed(0, null)).SelectRandomItem<TIOrbitState>() into x
								where x != null
								select x).ToList<TIOrbitState>();
							if (flag6)
							{
								list5 = list5.Where<TIOrbitState>(delegate(TIOrbitState x)
								{
									bool? flag7;
									if (x == null)
									{
										flag7 = null;
									}
									else
									{
										TISpaceBodyState ref_system = x.ref_system;
										flag7 = ((ref_system != null) ? new bool?(ref_system.isEarth) : null);
									}
									bool? flag8 = flag7;
									return flag8.GetValueOrDefault();
								}).ToList<TIOrbitState>();
							}
							TIOrbitState leoOrbit = GameStateManager.LEOStates().FirstOrDefault<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null));
							if (leoOrbit != null && GameStateManager.Earth().IsSafeForColonization(CS$<>8__locals5.CS$<>8__locals4.faction, HabType.Station))
							{
								list5.Add(leoOrbit);
							}
							list5.AddRange(GameStateManager.Earth().OrbitsInSystem.Where<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null)).Take_Random<TIOrbitState>(1));
							IEnumerable<TISpaceBodyState> enumerable11 = GameStateManager.AllSpaceBodies();
							Func<TISpaceBodyState, bool> func5;
							if ((func5 = CS$<>8__locals5.CS$<>8__locals4.<>9__84) == null)
							{
								func5 = (CS$<>8__locals5.CS$<>8__locals4.<>9__84 = (TISpaceBodyState x) => CS$<>8__locals5.CS$<>8__locals4.faction.CanExplore(x));
							}
							IEnumerable<TISpaceBodyState> enumerable12 = from x in enumerable11.Where<TISpaceBodyState>(func5)
								where AIEvaluators.IsEnergyEfficient(x)
								select x;
							Func<TISpaceBodyState, bool> func6;
							if ((func6 = CS$<>8__locals5.CS$<>8__locals4.<>9__86) == null)
							{
								func6 = (CS$<>8__locals5.CS$<>8__locals4.<>9__86 = (TISpaceBodyState x) => x.IsSafeForColonization(CS$<>8__locals5.CS$<>8__locals4.faction, HabType.Station));
							}
							List<TISpaceBodyState> list6 = enumerable12.Where<TISpaceBodyState>(func6).ToList<TISpaceBodyState>();
							list5.AddRange(list6.SelectMany<TISpaceBodyState, TIOrbitState>((TISpaceBodyState x) => x.orbits.Where<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null)).Take<TIOrbitState>(1)));
							TIGameState tigameState3 = func(list5.Distinct<TIOrbitState>().Where<TIOrbitState>(CS$<>8__locals5.DoesOrbitMatchPredicate).Take_Random<TIOrbitState>(20), delegate(TIGameState candidate)
							{
								float num8 = CS$<>8__locals5.CS$<>8__locals4.GetDefaultScoreModifier(candidate);
								float num9 = ((leoOrbit == candidate) ? 1.25f : 1f);
								float num10 = ((candidate.ref_system == GameStateManager.Earth()) ? 1.25f : 1f);
								float systemDeadlinessScoreModifier = candidate.ref_system.GetSystemDeadlinessScoreModifier(CS$<>8__locals5.CS$<>8__locals4.faction);
								return num8 * num9 * num10 * systemDeadlinessScoreModifier;
							});
							action(tigameState3, 16);
						}
					}
				}
			}
		}

		// Token: 0x06005ABE RID: 23230 RVA: 0x002B51C4 File Offset: 0x002B33C4
		public override void ManageHabGoals(TIFactionState faction)
		{
			faction.RecalculateIncomes();
			this.ManageProspectGoals(faction);
			this.ManageFoundGoals(faction);
		}

		// Token: 0x06005ABF RID: 23231 RVA: 0x002B51DC File Offset: 0x002B33DC
		public override void FoundHabs(TIFactionState faction)
		{
			faction.RecalculateIncomes();
			bool anyAvailableMissionControl = faction.AnyAvailableMissionControl;
			bool ai_AnyAvailabeGenericMissionControl = faction.AI_AnyAvailabeGenericMissionControl;
			AIEvaluators.FactionIsWorkingOnHabModuleBasedObjectives(faction);
			Func<TIHabState, bool> <>9__10;
			foreach (FactionGoal_FoundHab factionGoal_FoundHab in (from x in faction.GoalsOfType(TIFactionGoalState.FoundHabGoals, true, true)
				select x as FactionGoal_FoundHab into x
				orderby x.objectiveGoal descending, x is FactionGoal_FoundBase descending
				select x).ToList<FactionGoal_FoundHab>())
			{
				if (!factionGoal_FoundHab.InProgress() && anyAvailableMissionControl && (ai_AnyAvailabeGenericMissionControl || factionGoal_FoundHab.GrantMissionControlIndulgence))
				{
					TIGameState tigameState = factionGoal_FoundHab.target();
					bool foundingABase = tigameState.ref_habSite != null;
					bool flag = !faction.CanFoundHabFromHabAtLocation(factionGoal_FoundHab.target(), false, false) || !(factionGoal_FoundHab is FactionGoal_FoundMaxStation);
					IEnumerable<FoundHabOperation> enumerable = (from x in faction.AvailableOperationList(factionGoal_FoundHab.target().ref_naturalSpaceObject)
						select x as FoundHabOperation into x
						where x != null
						select x).Where<FoundHabOperation>(delegate(FoundHabOperation x)
					{
						if (!foundingABase)
						{
							return x is FoundStationOperation;
						}
						return x is FoundBaseOperation;
					}).ToList<FoundHabOperation>();
					IEnumerable<FoundHabOperation> enumerable2 = enumerable.Where<FoundHabOperation>((FoundHabOperation x) => !x.CoreModule(false).automated);
					if (!foundingABase || enumerable2.Any<FoundHabOperation>())
					{
						enumerable = enumerable2;
					}
					if (enumerable.Any<FoundHabOperation>())
					{
						FoundHabOperation foundHabOperation;
						if (flag)
						{
							foundHabOperation = enumerable.MinBy<FoundHabOperation, int>((FoundHabOperation x) => x.GetTier());
						}
						else
						{
							foundHabOperation = enumerable.MaxBy<FoundHabOperation, int>((FoundHabOperation x) => x.GetTier());
						}
						TIHabModuleTemplate tihabModuleTemplate = foundHabOperation.CoreModule(false);
						if (factionGoal_FoundHab.objectiveGoal || faction.AI_GenericMissionControlAvailable + tihabModuleTemplate.missionControl > 0 || foundingABase || AIEvaluators.IsEnergyEfficient(tigameState.ref_system))
						{
							TIResourcesCost tiresourcesCost = (from x in foundHabOperation.ResourceCostOptions(faction, tigameState, faction, true)
								orderby x.GetSingleCostValue(FactionResource.Boost)
								select x).FirstOrDefault<TIResourcesCost>();
							if (tiresourcesCost != null)
							{
								float singleCostValue = tiresourcesCost.GetSingleCostValue(FactionResource.Boost);
								bool flag2 = false;
								if (singleCostValue > 0f)
								{
									flag2 = !factionGoal_FoundHab.objectiveGoal && AIEvaluators.ShouldRateLimitBoostExpenditure(tihabModuleTemplate, faction, tigameState);
									if (flag2 && tigameState.ref_system != GameStateManager.Earth())
									{
										continue;
									}
									if (flag2)
									{
										IEnumerable<TIHabState> habsInSystem = tigameState.ref_system.habsInSystem;
										Func<TIHabState, bool> func;
										if ((func = <>9__10) == null)
										{
											func = (<>9__10 = (TIHabState x) => x.faction == faction);
										}
										if (habsInSystem.Where<TIHabState>(func).Any<TIHabState>((TIHabState x) => x.AvailableSlots().Count > 1))
										{
											continue;
										}
									}
								}
								if (factionGoal_FoundHab.objectiveGoal || (!AIEvaluators.ShouldNotBuildHabModuleRightNow(tihabModuleTemplate, faction, tigameState) && AIEvaluators.ShouldPayTodaysBoostCost(tihabModuleTemplate, faction, tigameState, false, 180)))
								{
									string[] array = new string[10];
									array[0] = faction.displayName;
									array[1] = " founding ";
									array[2] = foundHabOperation.CoreModule(faction.IsAlienFaction).displayName;
									array[3] = " at ";
									array[4] = tigameState.displayName;
									array[5] = ", ";
									int num = 6;
									TINaturalSpaceObjectState ref_naturalSpaceObject = tigameState.ref_naturalSpaceObject;
									array[num] = ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.displayName : null) ?? "unknown";
									array[7] = ", spending ";
									array[8] = tiresourcesCost.GetSingleCostValue(FactionResource.Boost).ToString();
									array[9] = " boost.";
									TIFactionState.LogAI(string.Concat(array), false);
									if (tiresourcesCost.GetSingleCostValue(FactionResource.Boost) > 0f && flag2)
									{
										TIFactionState.BoostAccountName boostAccountName = ((foundHabOperation is FoundBaseOperation) ? TIFactionState.BoostAccountName.Base : TIFactionState.BoostAccountName.Station);
										faction.boostAccounts[boostAccountName] = TITimeState.Now();
									}
									faction.playerControl.StartAction(new ConfirmOperationAction(faction, tigameState, foundHabOperation, tiresourcesCost, null));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005AC0 RID: 23232 RVA: 0x002B56E4 File Offset: 0x002B38E4
		public static void ManageMineNetwork(TIFactionState faction)
		{
			bool flag = faction.bases.Any<TIHabState>((TIHabState x) => x.HasInactiveButPowerableMine);
			int num = 1;
			bool flag2 = faction.MissionControlBalance < num && faction.GetMissionControlRequirementFromMineNetwork(-1) > 0;
			if (flag || flag2)
			{
				foreach (TIHabState tihabState in faction.bases)
				{
					if (tihabState.HasMine && tihabState.mine != null)
					{
						tihabState.mine.SetPowerStatus(true, false);
						if (!tihabState.mine.active)
						{
							tihabState.ResetPower();
						}
					}
				}
				faction.RecalculateIncomes();
				List<TIHabModuleState> list = (from x in faction.bases
					where x.HasMine
					select x.mine into x
					group x by x.ref_spaceBody into x
					where x.Count<TIHabModuleState>() > 1
					select x).SelectMany<IGrouping<TISpaceBodyState, TIHabModuleState>, TIHabModuleState>((IGrouping<TISpaceBodyState, TIHabModuleState> x) => x).ToList<TIHabModuleState>();
				List<TIHabModuleState> list2 = (from x in faction.bases
					where x.HasMine
					select x.mine into x
					where x.powered
					select x).Except<TIHabModuleState>(list).ToList<TIHabModuleState>();
				Func<TIHabModuleState, float> <>9__10;
				while (list2.Count > 0)
				{
					AIEvaluators.ClearResourceUpkeepInsecurityCache(faction);
					IEnumerable<TIHabModuleState> enumerable = list2;
					Func<TIHabModuleState, TIHabModuleState> func = (TIHabModuleState x) => x;
					Func<TIHabModuleState, float> func2;
					if ((func2 = <>9__10) == null)
					{
						func2 = (<>9__10 = delegate(TIHabModuleState mine)
						{
							TIHabModuleTemplate mineTemplate = mine.moduleTemplate;
							Func<FactionResource, float> func3 = (FactionResource resource) => faction.GetMonthlyIncome(resource, true, false) - mineTemplate.MonthlyResourceIncome(resource, mine.hab, faction) + mineTemplate.MonthlySupportCost(resource, true, faction, mine.hab);
							return AIEvaluators.EvaluateHabModule_PercentChange(faction, mine.hab, mineTemplate, null, null, func3, false, false);
						});
					}
					TIHabModuleState key = enumerable.ToDictionary<TIHabModuleState, TIHabModuleState, float>(func, func2).MinBy<KeyValuePair<TIHabModuleState, float>, float>((KeyValuePair<TIHabModuleState, float> x) => x.Value).Key;
					list2.Remove(key);
					if (faction.GetMissionControlGainedFromTurningOffMine(key) == 0)
					{
						break;
					}
					key.SetPowerStatus(false, false);
					faction.RecalculateIncomes();
					if (faction.MissionControlBalance >= num)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06005AC1 RID: 23233 RVA: 0x002B59FC File Offset: 0x002B3BFC
		public override void ManageHabs(TIFactionState faction)
		{
			PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.ManageMineNetwork(faction);
			this.BuildHabModules(faction);
			foreach (TIHabState tihabState in faction.habs)
			{
				if (TIUtilities.RandomFloatValue() < 0.01f)
				{
					tihabState.HabSchematic = null;
				}
			}
		}

		// Token: 0x06005AC2 RID: 23234 RVA: 0x002B5A68 File Offset: 0x002B3C68
		public void BuildHabModules(TIFactionState faction)
		{
			HumanHabPlanner.<>c__DisplayClass11_0 CS$<>8__locals1 = new HumanHabPlanner.<>c__DisplayClass11_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.buildHabGoals = (from x in CS$<>8__locals1.faction.GoalsOfType(TIFactionGoalState.BuildHabGoals, false, true)
				select x as FactionGoal_BuildHab into x
				where x.hab != null
				where x.objectiveGoal
				select x).ToDictionary<FactionGoal_BuildHab, TIHabState, FactionGoal_BuildHab>((FactionGoal_BuildHab x) => x.hab, (FactionGoal_BuildHab x) => x);
			CS$<>8__locals1.anyAvailableGenericMissionControl = CS$<>8__locals1.faction.AI_AnyAvailabeGenericMissionControl;
			CS$<>8__locals1.anyAvailableMissionControl = CS$<>8__locals1.faction.AnyAvailableMissionControl;
			foreach (TIHabState tihabState in CS$<>8__locals1.faction.habs.Where<TIHabState>((TIHabState x) => x.HabSchematic == null))
			{
				tihabState.HabSchematic = HabSchematic.SelectHabSchematic(CS$<>8__locals1.faction, tihabState, null);
			}
			List<TIHabState> list = (from x in CS$<>8__locals1.faction.habs
				where !x.ShouldPauseHabConstruction()
				where !x.decommissioning
				select x).ToList<TIHabState>();
			CS$<>8__locals1.completeHabs = list.Where<TIHabState>(delegate(TIHabState hab)
			{
				int count = hab2.AvailableSlots().Count;
				return count == 0 || (count == 1 && hab2.NetPower(true, true) >= 0);
			}).ToList<TIHabState>();
			CS$<>8__locals1.incompleteHabs = list.Except<TIHabState>(CS$<>8__locals1.completeHabs).ToList<TIHabState>();
			int num;
			PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.nextTargetOrderCount.TryGetValue(CS$<>8__locals1.faction, out num);
			num = Mathf.Max(num, 3);
			List<TIHabState> list2 = CS$<>8__locals1.incompleteHabs.Where<TIHabState>((TIHabState x) => x.IsBase && (!x.mine.present || (!x.mine.active && x.NetPower(true, true) < 0))).ToList<TIHabState>();
			int num2 = num - list2.Count<TIHabState>();
			if (num2 > 0)
			{
				list2.AddRange((from x in CS$<>8__locals1.incompleteHabs.Except<TIHabState>(list2)
					orderby x.ref_system.isEarth descending, TIUtilities.RandomFloatValue()
					select x).Take<TIHabState>(num2));
			}
			int num3 = 0;
			List<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>> list3 = (from x in list2.Select<TIHabState, ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>(delegate(TIHabState hab)
				{
					IEnumerable<TIHabModuleTemplate> enumerable = null;
					FactionGoal_BuildHab factionGoal_BuildHab;
					if (CS$<>8__locals1.buildHabGoals.TryGetValue(hab2, out factionGoal_BuildHab))
					{
						enumerable = factionGoal_BuildHab.RequiredModules();
					}
					HabSchematicOrder order2 = hab2.HabSchematic.GetOrder(CS$<>8__locals1.faction, hab2, false, true, enumerable);
					foreach (TIHabModuleState tihabModuleState3 in hab2.AllModules())
					{
						if (!tihabModuleState3.destroyed)
						{
							order2.Remove(tihabModuleState3.moduleTemplate);
						}
					}
					if (order2.Count == 0)
					{
						CS$<>8__locals1.completeHabs.Add(hab2);
					}
					TIResourcesCost tiresourcesCost10 = new TIResourcesCost();
					foreach (TIHabModuleTemplate tihabModuleTemplate8 in order2)
					{
						tiresourcesCost10.SumCosts_NoDuration(tihabModuleTemplate8.CostFromSpace(CS$<>8__locals1.faction, hab2, false, false, 0, true));
					}
					return new ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>(hab2, order2, tiresourcesCost10);
				})
				where x.Item2.Count > 0
				orderby CS$<>8__locals1.buildHabGoals.ContainsKey(x.Item1) descending, x.Item2.Score(CS$<>8__locals1.faction, x.Item1, null, true, true)
				select x).ToList<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>();
			TIHabModuleTemplate moduleTemplate;
			List<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>> list4 = (from order in list3
				where order.Item2.Any<TIHabModuleTemplate>((TIHabModuleTemplate moduleTemplate) => !AIEvaluators.ShouldRateLimitBoostExpenditure(moduleTemplate, CS$<>8__locals1.faction, order.Item1))
				select order into x
				orderby x.Item1.NetPower(true, true) < 0, TISpaceObjectState.GenericTransferBoostFromEarthSurface(CS$<>8__locals1.faction, x.Item1, 1f)
				select x).ToList<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>();
			foreach (ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost> valueTuple in list4)
			{
				bool flag = false;
				foreach (TIHabModuleTemplate tihabModuleTemplate in valueTuple.Item2)
				{
					if (!AIEvaluators.ShouldRateLimitBoostExpenditure(tihabModuleTemplate, CS$<>8__locals1.faction, valueTuple.Item1))
					{
						if (!AIEvaluators.ShouldPayTodaysBoostCost(tihabModuleTemplate, CS$<>8__locals1.faction, valueTuple.Item1, false, 180))
						{
							flag = true;
							break;
						}
						TIResourcesCost tiresourcesCost = tihabModuleTemplate.CostFromSpace(CS$<>8__locals1.faction, valueTuple.Item1, false, true, 0, true);
						TIHabModuleState slotForNewModule = valueTuple.Item1.GetSlotForNewModule(tihabModuleTemplate, false, null);
						if (slotForNewModule == null)
						{
							Log.Error("Ran out of slots for HabSchematicOrder during critical hab module construction.", Array.Empty<object>());
						}
						else
						{
							CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate, slotForNewModule.sector, slotForNewModule.slot, tiresourcesCost, null));
						}
					}
				}
				if (flag)
				{
					break;
				}
				num3++;
			}
			using (List<TIHabState>.Enumerator enumerator4 = (from x in CS$<>8__locals1.faction.bases
				where x.HasMine
				where x.mine.tier != x.tier
				where x.AvailableSlots().Count > 0
				orderby x.ref_system.semiMajorAxis_AU, x.tier
				select x).ToList<TIHabState>().ToList<TIHabState>().GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					TIHabState base_ = enumerator4.Current;
					HabSchematicOrder order3 = base_.HabSchematic.GetOrder(CS$<>8__locals1.faction, base_, false, false, null);
					TIHabModuleTemplate tihabModuleTemplate2 = order3.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine);
					TIHabModuleTemplate tihabModuleTemplate3 = order3.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource);
					if (tihabModuleTemplate2 != null && tihabModuleTemplate3 != null)
					{
						int num4 = -base_.mine.moduleTemplate.ProspectivePower(base_);
						float num5 = (float)(-(float)tihabModuleTemplate2.ProspectivePower(base_));
						int num6 = tihabModuleTemplate3.ProspectivePower(base_);
						int num7 = ((num5 - (float)num4) / (float)num6).RoundUp();
						if (num7 <= base_.AvailableSlots().Count)
						{
							TIResourcesCost tiresourcesCost2 = tihabModuleTemplate2.CostFromSpace(CS$<>8__locals1.faction, base_, base_.mine.moduleTemplate.UpgradesTo == tihabModuleTemplate2, false, 0, false);
							TIResourcesCost tiresourcesCost3 = tihabModuleTemplate3.CostFromSpace(CS$<>8__locals1.faction, base_, base_.mine.moduleTemplate.UpgradesTo == tihabModuleTemplate2, false, 0, false);
							if (!(tiresourcesCost2 + tiresourcesCost3.MultiplyCost((float)num7)).CanAfford_AI(CS$<>8__locals1.faction, null, null, 1, false, false, 1f, null, float.PositiveInfinity))
							{
								return;
							}
							CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate2, base_.mine.sector, base_.mine.slot, tiresourcesCost2, null));
							for (int i = 0; i < num7; i++)
							{
								TIHabModuleState slotForNewModule2 = base_.GetSlotForNewModule(tihabModuleTemplate3, false, null);
								CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate3, slotForNewModule2.sector, slotForNewModule2.slot, tiresourcesCost3, null));
							}
							if (list3.Any<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>(([TupleElementNames(new string[] { "Hab", "Order", "Cost" })] ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost> x) => x.Item1 == base_))
							{
								num3++;
							}
							list3.RemoveAll(([TupleElementNames(new string[] { "Hab", "Order", "Cost" })] ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost> x) => x.Item1 == base_);
						}
					}
				}
			}
			List<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>> list5 = (from x in list3.Except<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>(list4)
				where base.<BuildHabModules>g__CanAfford|16(x.Item3)
				select x).ToList<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>();
			using (List<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>.Enumerator enumerator2 = list5.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost> element2 = enumerator2.Current;
					if (!CS$<>8__locals1.<BuildHabModules>g__CanAfford|16(element2.Item3))
					{
						break;
					}
					if (element2.Item2.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => AIEvaluators.ShouldNotBuildHabModuleRightNow(x, CS$<>8__locals1.faction, element2.Item1)))
					{
						break;
					}
					foreach (TIHabModuleTemplate tihabModuleTemplate4 in element2.Item2)
					{
						TIResourcesCost tiresourcesCost4 = tihabModuleTemplate4.CostFromSpace(CS$<>8__locals1.faction, element2.Item1, false, false, 0, true);
						TIHabModuleState slotForNewModule3 = element2.Item1.GetSlotForNewModule(tihabModuleTemplate4, false, null);
						if (slotForNewModule3 == null)
						{
							Log.Error("Ran out of slots for HabSchematicOrder during space based hab module construction.", Array.Empty<object>());
						}
						else
						{
							CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate4, slotForNewModule3.sector, slotForNewModule3.slot, tiresourcesCost4, null));
						}
					}
					num3++;
				}
			}
			List<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>> list6 = (from x in list3.Except<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>(list4).Except<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>(list5)
				where x.Item1.ref_system == GameStateManager.Earth()
				select x).ToList<ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost>>();
			if (list6.Count > 0)
			{
				ValueTuple<TIHabState, HabSchematicOrder, TIResourcesCost> valueTuple2 = list6[0];
				TIHabState hab2 = valueTuple2.Item1;
				HabSchematicOrder item = valueTuple2.Item2;
				TIResourcesCost boostSubstitutedCost = valueTuple2.Item3.GetBoostSubstitutedCost(CS$<>8__locals1.faction, hab2, true, null);
				float singleCostValue = boostSubstitutedCost.GetSingleCostValue(FactionResource.Boost);
				if (CS$<>8__locals1.<BuildHabModules>g__CanAfford|16(boostSubstitutedCost) && AIEvaluators.ShouldPayRateLimitedBoostCost(singleCostValue, CS$<>8__locals1.faction, hab2, false) && item.None<TIHabModuleTemplate>((TIHabModuleTemplate x) => AIEvaluators.ShouldNotBuildHabModuleRightNow(x, CS$<>8__locals1.faction, hab2)))
				{
					foreach (TIHabModuleTemplate tihabModuleTemplate5 in item)
					{
						TIResourcesCost tiresourcesCost5 = tihabModuleTemplate5.CostFromSpace(CS$<>8__locals1.faction, hab2, false, true, 0, true);
						TIHabModuleState slotForNewModule4 = hab2.GetSlotForNewModule(tihabModuleTemplate5, false, null);
						if (slotForNewModule4 == null)
						{
							Log.Error("Ran out of slots for HabSchematicOrder during boost-substituted hab module construction.", Array.Empty<object>());
						}
						else
						{
							CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate5, slotForNewModule4.sector, slotForNewModule4.slot, tiresourcesCost5, null));
						}
					}
					TIFactionState.BoostAccountName boostAccountName = (hab2.IsBase ? TIFactionState.BoostAccountName.Base : TIFactionState.BoostAccountName.Station);
					CS$<>8__locals1.faction.boostAccounts[boostAccountName] = TITimeState.Now();
					num3++;
				}
			}
			if (num3 < list2.Count)
			{
				PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.nextTargetOrderCount[CS$<>8__locals1.faction] = ((float)list2.Count * 0.6f).RoundUp();
			}
			else
			{
				PavonisInteractive.TerraInvicta.Tasks.HumanHabPlanner.nextTargetOrderCount[CS$<>8__locals1.faction] = ((float)list2.Count * 1.3f).RoundUp();
			}
			CS$<>8__locals1.shouldPerformStandardHabUpgrades = this.ShouldPerformHabUpgrades(CS$<>8__locals1.faction);
			bool flag2 = CS$<>8__locals1.buildHabGoals.Any<KeyValuePair<TIHabState, FactionGoal_BuildHab>>((KeyValuePair<TIHabState, FactionGoal_BuildHab> x) => x.Value.GrantMissionControlIndulgence);
			if (CS$<>8__locals1.anyAvailableMissionControl && (CS$<>8__locals1.shouldPerformStandardHabUpgrades || flag2))
			{
				List<TIHabState> upgradeableHabs = (from x in list.Where<TIHabState>(new Func<TIHabState, bool>(CS$<>8__locals1.<BuildHabModules>g__HabMayBeUpgraded|38))
					where !x.coreModule.underConstruction && x.coreModule.CanUpgrade(CS$<>8__locals1.faction)
					where x.IsStation || (x.HasMine && x.mine.active)
					orderby x.IsBase descending, x.ref_system.semiMajorAxis_AU
					select x).ToList<TIHabState>();
				if (AIEvaluators.FactionNeedsNewObjectiveHab(CS$<>8__locals1.faction))
				{
					upgradeableHabs.Where<TIHabState>((TIHabState x) => AIEvaluators.DoesHabMatchObjectiveHabModuleRequirements(x, true)).ToList<TIHabState>();
					upgradeableHabs = upgradeableHabs.OrderByDescending<TIHabState, bool>((TIHabState x) => upgradeableHabs.Contains(x)).ToList<TIHabState>();
				}
				foreach (TIHabState tihabState2 in upgradeableHabs.Take<TIHabState>(3))
				{
					TIHabModuleTemplate upgradesTo = tihabState2.coreModule.moduleTemplate.UpgradesTo;
					TIResourcesCost tiresourcesCost6 = upgradesTo.CostFromSpace(CS$<>8__locals1.faction, tihabState2, true, false, 0, true);
					if (!CS$<>8__locals1.<BuildHabModules>g__CanAfford|16(tiresourcesCost6))
					{
						break;
					}
					CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(upgradesTo, tihabState2.CoreSlot.sector, tihabState2.CoreSlot.slot, tiresourcesCost6, null));
					tihabState2.HabSchematic = null;
					if (!this.ShouldPerformHabUpgrades(CS$<>8__locals1.faction))
					{
						break;
					}
					if (!(CS$<>8__locals1.anyAvailableMissionControl = CS$<>8__locals1.faction.AnyAvailableMissionControl))
					{
						break;
					}
				}
			}
			if (CS$<>8__locals1.<BuildHabModules>g__MayRenovate|30())
			{
				List<TIHabState> list7 = CS$<>8__locals1.completeHabs.Where<TIHabState>((TIHabState x) => !x.UnderConstructionModules().Any<TIHabModuleState>()).ToList<TIHabState>();
				list7 = list7.Take_Random<TIHabState>(6).Union<TIHabState>(list7.Where<TIHabState>((TIHabState x) => base.<BuildHabModules>g__NeedsObjectiveRenovation|29(x))).ToList<TIHabState>();
				foreach (ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float> valueTuple3 in (from x in list7.Select<TIHabState, ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float>>(delegate(TIHabState hab)
					{
						HumanHabPlanner.<>c__DisplayClass11_6 CS$<>8__locals8 = new HumanHabPlanner.<>c__DisplayClass11_6();
						CS$<>8__locals8.CS$<>8__locals4 = CS$<>8__locals1;
						CS$<>8__locals8.hab = hab;
						HabSchematicOrder habSchematicOrder = new HabSchematicOrder(CS$<>8__locals8.hab.HabSchematic.Preferences, from x in CS$<>8__locals8.hab.OkayModules()
							select x.moduleTemplate);
						float num8 = habSchematicOrder.Score(CS$<>8__locals1.faction, CS$<>8__locals8.hab.location, new Func<FactionResource, float>(CS$<>8__locals8.<BuildHabModules>g__GetMonthlyIncomeSansThisHab|53), false, true);
						if (num8 == 0f)
						{
							num8 = 0.001f;
						}
						IEnumerable<TIHabModuleTemplate> enumerable2 = null;
						FactionGoal_BuildHab factionGoal_BuildHab2;
						if (CS$<>8__locals1.buildHabGoals.TryGetValue(CS$<>8__locals8.hab, out factionGoal_BuildHab2))
						{
							enumerable2 = factionGoal_BuildHab2.RequiredModules();
						}
						HabSchematicOrder order4 = CS$<>8__locals8.hab.HabSchematic.GetOrder(CS$<>8__locals1.faction, CS$<>8__locals8.hab, false, false, enumerable2);
						float num9 = order4.Score(CS$<>8__locals1.faction, CS$<>8__locals8.hab.location, new Func<FactionResource, float>(CS$<>8__locals8.<BuildHabModules>g__GetMonthlyIncomeSansThisHab|53), false, true);
						float num10 = num9 / num8;
						if (num10 < 0f)
						{
							num10 = 1000f;
						}
						if (num9 < 0f)
						{
							num10 = 1f / num10;
						}
						num10 = Mathf.Clamp(num10, 0f, 1000f);
						float num11 = num9 - num8;
						return new ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float>(CS$<>8__locals8.hab, habSchematicOrder, order4, num10, num11);
					}).Where<ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float>>(delegate([TupleElementNames(new string[] { "Hab", "OriginalOrder", "Order", "ImprovementRatio", "AbsoluteImprovement" })] ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float> element)
					{
						FactionGoal_BuildHab goal;
						if (CS$<>8__locals1.buildHabGoals.TryGetValue(element.Item1, out goal) && goal.objectiveGoal && goal.objective.GetObjectiveStatus(CS$<>8__locals1.faction) == ObjectiveStatus.Unlocked)
						{
							return element.Item3.Count<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.dataName == goal.objective.targetHabModuleName) > element.Item1.OkayModules().Count<TIHabModuleState>((TIHabModuleState x) => x.templateName == goal.objective.targetHabModuleName);
						}
						return element.Item5 > 0f && element.Item4 > 1.5f;
					})
					orderby base.<BuildHabModules>g__NeedsObjectiveRenovation|29(x.Item1) descending
					select x).ThenByDescending<ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float>, float>(delegate([TupleElementNames(new string[] { "Hab", "OriginalOrder", "Order", "ImprovementRatio", "AbsoluteImprovement" })] ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float> element)
				{
					float num12 = CS$<>8__locals1.faction.nShipyardQueues.Where<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value.Count > 0 && x.Key.hab == element.Item1).Sum<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Value.First<ShipConstructionQueueItem>().progressFraction);
					return element.Item5 - 20f * num12;
				}).ToList<ValueTuple<TIHabState, HabSchematicOrder, HabSchematicOrder, float, float>>())
				{
					TIHabState item2 = valueTuple3.Item1;
					HabSchematicOrder item3 = valueTuple3.Item3;
					List<TIHabModuleState> list8 = item2.AllSlots();
					foreach (TIHabModuleState tihabModuleState in item2.OkayModules())
					{
						if (item3.Contains(tihabModuleState.moduleTemplate))
						{
							item3.Remove(tihabModuleState.moduleTemplate);
							list8.Remove(tihabModuleState);
						}
					}
					TIResourcesCost tiresourcesCost7 = new TIResourcesCost();
					foreach (TIHabModuleTemplate tihabModuleTemplate6 in item3)
					{
						tiresourcesCost7.SumCosts_NoDuration(tihabModuleTemplate6.CostFromSpace(CS$<>8__locals1.faction, item2, false, false, 0, true));
					}
					if (CS$<>8__locals1.<BuildHabModules>g__CanAfford|16(tiresourcesCost7))
					{
						foreach (TIHabModuleTemplate tihabModuleTemplate7 in item3.ToList<TIHabModuleTemplate>())
						{
							TIHabModuleState slotForNewModule5 = item2.GetSlotForNewModule(tihabModuleTemplate7, true, list8);
							if (!(slotForNewModule5 == null))
							{
								item3.Remove(tihabModuleTemplate7);
								list8.Remove(slotForNewModule5);
								moduleTemplate = slotForNewModule5.moduleTemplate;
								bool flag3 = ((moduleTemplate != null) ? moduleTemplate.UpgradesTo : null) == tihabModuleTemplate7;
								TIResourcesCost tiresourcesCost8 = tihabModuleTemplate7.CostFromSpace(CS$<>8__locals1.faction, item2, flag3, false, 0, true);
								CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate7, slotForNewModule5.sector, slotForNewModule5.slot, tiresourcesCost8, null));
							}
						}
						using (List<TIHabModuleTemplate>.Enumerator enumerator3 = item3.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								TIHabModuleTemplate moduleTemplate = enumerator3.Current;
								TIHabModuleState tihabModuleState2 = list8.OrderBy<TIHabModuleState, bool>((TIHabModuleState x) => x.moduleTemplate.powerSource).FirstOrDefault<TIHabModuleState>((TIHabModuleState x) => x.IsModuleValidForSlot(moduleTemplate));
								list8.Remove(tihabModuleState2);
								if (tihabModuleState2 == null)
								{
									Log.Error("Ran out of slots for HabSchematicOrder during renovation.", Array.Empty<object>());
								}
								else
								{
									TIResourcesCost tiresourcesCost9 = moduleTemplate.CostFromSpace(CS$<>8__locals1.faction, item2, false, false, 0, true);
									CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(moduleTemplate, tihabModuleState2.sector, tihabModuleState2.slot, tiresourcesCost9, null));
								}
							}
						}
						if (!CS$<>8__locals1.<BuildHabModules>g__MayRenovate|30())
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06005AC5 RID: 23237 RVA: 0x002B6CBC File Offset: 0x002B4EBC
		[CompilerGenerated]
		internal static float <ManageProspectGoals>g__GetSpaceBodyIncomes|1_4(TISpaceBodyState spaceBody, FactionResource resource)
		{
			IEnumerable<TIHabSiteState> enumerable = spaceBody.habSites.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase);
			if (!enumerable.Any<TIHabSiteState>())
			{
				return 0f;
			}
			return enumerable.Max<TIHabSiteState>((TIHabSiteState y) => TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth[y][resource]);
		}

		// Token: 0x0400415A RID: 16730
		public static Dictionary<TIFactionState, int> nextTargetOrderCount = new Dictionary<TIFactionState, int>();

		// Token: 0x020012DD RID: 4829
		private enum ExpansionType
		{
			// Token: 0x04006D8C RID: 28044
			Station,
			// Token: 0x04006D8D RID: 28045
			Upgrade,
			// Token: 0x04006D8E RID: 28046
			Renovation
		}
	}
}
