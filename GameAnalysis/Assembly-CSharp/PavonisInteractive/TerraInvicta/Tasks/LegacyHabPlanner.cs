using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000947 RID: 2375
	public abstract class LegacyHabPlanner : HabPlanner
	{
		// Token: 0x06005AC6 RID: 23238 RVA: 0x002B6D24 File Offset: 0x002B4F24
		public override void FoundHabs(TIFactionState faction)
		{
			List<FactionGoal_FoundHab> list = (from x in faction.GoalsOfType(TIFactionGoalState.FoundHabGoals, false, true)
				where !x.InProgress()
				select x as FactionGoal_FoundHab).ToList<FactionGoal_FoundHab>();
			Dictionary<FactionGoal_FoundHab, float> goalBoostCosts = list.ToDictionary<FactionGoal_FoundHab, FactionGoal_FoundHab, float>((FactionGoal_FoundHab x) => x, delegate(FactionGoal_FoundHab foundHabGoal)
			{
				IOperation operation2 = new FoundOutpostOperation();
				if (foundHabGoal is FactionGoal_FoundStation)
				{
					operation2 = new FoundPlatformOperation();
				}
				List<TIResourcesCost> list3 = operation2.ResourceCostOptions(faction, foundHabGoal.target(), faction, false);
				if (!list3.Any<TIResourcesCost>())
				{
					return float.PositiveInfinity;
				}
				return list3.Min<TIResourcesCost>((TIResourcesCost x) => x.GetSingleCostValue(FactionResource.Boost));
			});
			using (List<FactionGoal_FoundHab>.Enumerator enumerator = (from x in list
				orderby x is FactionGoal_FoundBase && x.location().ref_naturalSpaceObject.isLuna descending, x.importance, goalBoostCosts[x]
				select x).ToList<FactionGoal_FoundHab>().GetEnumerator())
			{
				Func<TIHabSiteState, float> <>9__11;
				while (enumerator.MoveNext())
				{
					FactionGoal_FoundHab foundHabGoal = enumerator.Current;
					if (foundHabGoal.target().IsSafeForColonization(faction, HabType.Any) && !AIEvaluators.IsSystemContested(faction, foundHabGoal.target()))
					{
						Func<FoundHabOperation, bool> func = delegate(FoundHabOperation operation)
						{
							TIHabModuleTemplate tihabModuleTemplate = operation.CoreModule(faction.IsAlienFaction);
							return !AIEvaluators.ShouldNotBuildHabModuleRightNow(tihabModuleTemplate, faction, foundHabGoal.target()) && AIEvaluators.ShouldPayTodaysBoostCost(tihabModuleTemplate, faction, foundHabGoal.target(), false, 180);
						};
						List<Type> list2 = (from x in faction.AvailableOperationList(foundHabGoal.target().ref_naturalSpaceObject)
							select x.GetType()).Intersect<Type>(foundHabGoal.spaceOperations).ToList<Type>();
						if (list2.Count > 0)
						{
							int availableMissionControl = faction.AvailableMissionControl;
							int count = faction.nShipyardQueues.Values.Count;
							FoundHabOperation foundHabOperation = null;
							if (!(foundHabGoal is FactionGoal_FoundPlatform))
							{
								if (!(foundHabGoal is FactionGoal_FoundMaxStation))
								{
									if (foundHabGoal is FactionGoal_FoundBase)
									{
										if (list2.Contains(typeof(FoundColonyOperation)))
										{
											foundHabOperation = new FoundColonyOperation();
											if (func(foundHabOperation))
											{
												goto IL_036F;
											}
											foundHabOperation = null;
										}
										if (list2.Contains(typeof(FoundSettlementOperation)))
										{
											foundHabOperation = new FoundSettlementOperation();
											if (func(foundHabOperation))
											{
												goto IL_036F;
											}
											foundHabOperation = null;
										}
										if (list2.Contains(typeof(FoundOutpostOperation)))
										{
											foundHabOperation = new FoundOutpostOperation();
											if (!func(foundHabOperation))
											{
												foundHabOperation = null;
											}
										}
									}
								}
								else
								{
									if (list2.Contains(typeof(FoundRingOperation)))
									{
										foundHabOperation = new FoundRingOperation();
										if (func(foundHabOperation))
										{
											goto IL_036F;
										}
										foundHabOperation = null;
									}
									if (list2.Contains(typeof(FoundOrbitalOperation)))
									{
										foundHabOperation = new FoundOrbitalOperation();
										if (func(foundHabOperation))
										{
											goto IL_036F;
										}
										foundHabOperation = null;
									}
									if (list2.Contains(typeof(FoundPlatformOperation)))
									{
										foundHabOperation = new FoundPlatformOperation();
										if (!func(foundHabOperation))
										{
											foundHabOperation = null;
										}
									}
								}
							}
							else
							{
								if (list2.Contains(typeof(FoundPlatformOperation)))
								{
									foundHabOperation = new FoundPlatformOperation();
								}
								if (foundHabOperation != null && !func(foundHabOperation))
								{
									foundHabOperation = null;
								}
							}
							IL_036F:
							if (foundHabOperation != null)
							{
								TIGameState tigameState = foundHabGoal.target();
								if (tigameState.isNaturalSpaceObjectState)
								{
									if (foundHabOperation is FoundBaseOperation)
									{
										IEnumerable<TIHabSiteState> enumerable = tigameState.ref_spaceBody.habSites.Where<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase);
										Func<TIHabSiteState, float> func2;
										if ((func2 = <>9__11) == null)
										{
											func2 = (<>9__11 = (TIHabSiteState x) => AIEvaluators.EvaluateHabSite(faction, x, false, false, true));
										}
										tigameState = enumerable.MaxBy<TIHabSiteState, float>(func2);
									}
									else if (foundHabOperation is FoundStationOperation)
									{
										tigameState = tigameState.ref_naturalSpaceObject.orbits.SelectRandomItem<TIOrbitState>();
									}
								}
								TIResourcesCost tiresourcesCost = (from x in foundHabOperation.ResourceCostOptions(faction, tigameState, faction, true)
									orderby x.GetSingleCostValue(FactionResource.Boost)
									select x).FirstOrDefault<TIResourcesCost>();
								if (tiresourcesCost != null && tiresourcesCost.completionTime_days <= 720f)
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
									if (tiresourcesCost.GetSingleCostValue(FactionResource.Boost) > 0f && AIEvaluators.ShouldRateLimitBoostExpenditure(foundHabOperation.CoreModule(faction.IsAlienFaction), faction, tigameState))
									{
										TIFactionState.BoostAccountName boostAccountName = ((foundHabOperation is FoundBaseOperation) ? TIFactionState.BoostAccountName.Base : TIFactionState.BoostAccountName.Station);
										faction.boostAccounts[boostAccountName] = TITimeState.Now();
									}
									faction.ref_faction.playerControl.StartAction(new ConfirmOperationAction(faction, tigameState, foundHabOperation, tiresourcesCost, null));
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005AC7 RID: 23239 RVA: 0x002B732C File Offset: 0x002B552C
		public override void ManageHabs(TIFactionState faction)
		{
			this.BuildHabModules(faction);
		}

		// Token: 0x06005AC8 RID: 23240 RVA: 0x002B7338 File Offset: 0x002B5538
		public void BuildHabModules(TIFactionState faction)
		{
			bool focusOnMines = TIGlobalValuesState.IsQuietAlienCampaign();
			List<FactionGoal_BuildHab> list = (from x in faction.GoalsOfType(TIFactionGoalState.BuildHabGoals, false, true).ConvertAll<FactionGoal_BuildHab>((TIFactionGoalState x) => x as FactionGoal_BuildHab)
				where !x.skipGoal
				orderby x.importance descending, focusOnMines && x is FactionGoal_BuildBase descending, x.assignedDate
				select x).ToList<FactionGoal_BuildHab>();
			List<TIHabState> list2 = faction.ShipConstructionHabs(true, true);
			List<TIHabState> habs = faction.habs;
			List<HabModuleSpecialRule> list3 = new List<HabModuleSpecialRule>();
			using (IEnumerator enumerator = Enum.GetValues(typeof(HabModuleSpecialRule)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HabModuleSpecialRule rule = (HabModuleSpecialRule)enumerator.Current;
					switch (rule)
					{
					case HabModuleSpecialRule.LEOBonusArmyCombatValue:
						if (faction.LEOStations.Sum<TIHabState>((TIHabState x) => x.GetLEOLabBonus(rule, true)) >= TemplateManager.global.maxArmyCombatBonusFromLEOHabs)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusPropagandaStrength:
						if (faction.LEOStations.Sum<TIHabState>((TIHabState x) => x.GetLEOLabBonus(rule, true)) >= TemplateManager.global.maxLEOHabPropagandaStrengthBonus)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusAlienDetection:
						if (faction.LEOStations.Sum<TIHabState>((TIHabState x) => x.GetLEOLabBonus(rule, true)) >= TemplateManager.global.alienDetectionBonusCapFromLEOHabs)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusMiltech:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Military, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusWelfare:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Welfare, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusLaunchFacilities:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.LaunchFacilities, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusKnowledge:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Knowledge, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusMissionControl:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.MissionControl, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusEconomy:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Economy, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusUnity:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Unity, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusOppression:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Oppression, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusEnvironment:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Environment, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusGovernment:
						if (faction.SumLEOHabPriorityBonuses(PriorityType.Welfare, true, 0f) >= TemplateManager.global.LEOHabModulePriorityBonusCap)
						{
							list3.Add(rule);
						}
						break;
					case HabModuleSpecialRule.LEOBonusHumanDetection:
						if (faction.LEOStations.Sum<TIHabState>((TIHabState x) => x.GetLEOLabBonus(rule, true)) >= TemplateManager.global.humanDetectionBonusCapFromLEOHabs)
						{
							list3.Add(rule);
						}
						break;
					}
				}
			}
			List<TechCategory> list4 = new List<TechCategory>();
			foreach (TechCategory techCategory in Enums.TechCategories)
			{
				if (faction.HabsMultiplier(techCategory) >= 0.5f)
				{
					list4.Add(techCategory);
				}
			}
			bool flag = false;
			foreach (FactionGoal_BuildHab factionGoal_BuildHab in list)
			{
				habs.Remove(factionGoal_BuildHab.target().ref_hab);
				if (!factionGoal_BuildHab.target().ref_hab.ShouldPauseHabConstruction())
				{
					TIHabState ref_hab = factionGoal_BuildHab.target().ref_hab;
					TIHabModuleTemplate tihabModuleTemplate = this.SelectHabModuleForBuilding(faction, ref_hab, factionGoal_BuildHab, list2, list3, list4);
					Dictionary<TIHabState, string> dictionary;
					string text;
					if (((tihabModuleTemplate == null && ref_hab.IsBase) & focusOnMines) && this.habModuleSelections.TryGetValue(faction, out dictionary) && dictionary.TryGetValue(ref_hab, out text))
					{
						TIHabModuleTemplate tihabModuleTemplate2 = TemplateManager.Find<TIHabModuleTemplate>(text, false);
						if (tihabModuleTemplate2 != null && tihabModuleTemplate2.mine)
						{
							flag = true;
							break;
						}
					}
					if (tihabModuleTemplate != null && factionGoal_BuildHab.importance >= 15 && this.SelectHabModuleForBuilding(faction, factionGoal_BuildHab.target().ref_hab, factionGoal_BuildHab, list2, list3, list4) != null && factionGoal_BuildHab.importance == 20)
					{
						this.SelectHabModuleForBuilding(faction, factionGoal_BuildHab.target().ref_hab, factionGoal_BuildHab, list2, list3, list4);
					}
				}
			}
			if (!flag)
			{
				foreach (TIHabState tihabState in habs)
				{
					if (!tihabState.ShouldPauseHabConstruction())
					{
						this.SelectHabModuleForBuilding(faction, tihabState, null, list2, list3, list4);
					}
				}
			}
		}

		// Token: 0x06005AC9 RID: 23241 RVA: 0x002B7970 File Offset: 0x002B5B70
		private TIHabModuleTemplate GetTargetedModuleForHab(TIFactionState faction, TIHabState hab, TIFactionGoalState goal, IEnumerable<TIHabModuleTemplate> allowedModules, IEnumerable<TIHabModuleState> shipyardsAtHab)
		{
			LegacyHabPlanner.<>c__DisplayClass3_0 CS$<>8__locals1 = new LegacyHabPlanner.<>c__DisplayClass3_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.hab = hab;
			if (CS$<>8__locals1.faction.AISavingTarget.active && CS$<>8__locals1.faction.AISavingTarget.relatedGoal == goal && allowedModules.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.dataName == CS$<>8__locals1.faction.AISavingTarget.desiredPurchase.dataName))
			{
				return CS$<>8__locals1.faction.AISavingTarget.desiredPurchase as TIHabModuleTemplate;
			}
			CS$<>8__locals1.returnValue = null;
			int num = shipyardsAtHab.Count<TIHabModuleState>();
			if (!CS$<>8__locals1.faction.IsAlienFaction && CS$<>8__locals1.hab.IsStation && CS$<>8__locals1.hab.ref_orbit.isEarthLEO && num == 0)
			{
				LegacyHabPlanner.<>c__DisplayClass3_0 CS$<>8__locals2 = CS$<>8__locals1;
				IEnumerable<TIHabModuleTemplate> enumerable = allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsShipConstruction);
				TIHabModuleTemplate tihabModuleTemplate;
				if (enumerable == null)
				{
					tihabModuleTemplate = null;
				}
				else
				{
					tihabModuleTemplate = enumerable.MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.power);
				}
				TIHabModuleTemplate tihabModuleTemplate2;
				if ((tihabModuleTemplate2 = tihabModuleTemplate) == null)
				{
					tihabModuleTemplate2 = (from x in TemplateManager.IterateByClass<TIHabModuleTemplate>(true)
						where x.allowsShipConstruction
						select x).MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.power);
				}
				CS$<>8__locals2.returnValue = tihabModuleTemplate2;
			}
			else if (goal != null)
			{
				IEnumerable<TIHabModuleTemplate> enumerable2 = from x in CS$<>8__locals1.hab.AllModules()
					select x.moduleTemplate;
				List<TIHabModuleTemplate> list = (goal as FactionGoal_BuildHab).RequiredModules().Except<TIHabModuleTemplate>(enumerable2).ToList<TIHabModuleTemplate>();
				list = list.Intersect<TIHabModuleTemplate>(allowedModules).ToList<TIHabModuleTemplate>();
				CS$<>8__locals1.returnValue = list.MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => Mathf.Abs(x.power));
			}
			if (CS$<>8__locals1.returnValue == null)
			{
				if (CS$<>8__locals1.faction.IsAlienFaction)
				{
					int num2 = CS$<>8__locals1.hab.OkayModules().Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.spaceCombatModule);
					int num3 = CS$<>8__locals1.hab.OkayModules().Count<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.DropTroops));
					if ((CS$<>8__locals1.hab == CS$<>8__locals1.faction.primaryHab && num < 2 && CS$<>8__locals1.hab.tier > 1) || (CS$<>8__locals1.hab.IsStation && num < CS$<>8__locals1.hab.sectors.Count && (CS$<>8__locals1.hab.tier == 1 || (num2 > 0 && num3 > 0))))
					{
						return allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsShipConstruction && x.tier == CS$<>8__locals1.hab.tier).FirstOrDefault<TIHabModuleTemplate>();
					}
					if (num2 < CS$<>8__locals1.hab.tier)
					{
						return allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.spaceCombatModule).MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
					}
					if (num3 < Mathf.Min(CS$<>8__locals1.hab.tier, 2))
					{
						return allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.specialRules.Contains(HabModuleSpecialRule.DropTroops)).MaxBy<TIHabModuleTemplate, int>((TIHabModuleTemplate x) => x.tier);
					}
				}
				return null;
			}
			if (!CS$<>8__locals1.hab.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate == CS$<>8__locals1.returnValue))
			{
				return CS$<>8__locals1.returnValue;
			}
			return null;
		}

		// Token: 0x06005ACA RID: 23242 RVA: 0x002B7D48 File Offset: 0x002B5F48
		private TIHabModuleTemplate SelectHabModuleForBuilding(TIFactionState faction, TIHabState hab, FactionGoal_BuildHab goal, List<TIHabState> shipyardHabs, List<HabModuleSpecialRule> maxxedOutSpecialRules, List<TechCategory> maxxedOutTechCategories)
		{
			LegacyHabPlanner.<>c__DisplayClass6_0 CS$<>8__locals1 = new LegacyHabPlanner.<>c__DisplayClass6_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.hab = hab;
			CS$<>8__locals1.maxxedOutSpecialRules = maxxedOutSpecialRules;
			CS$<>8__locals1.maxxedOutTechCategories = maxxedOutTechCategories;
			CS$<>8__locals1.emptySlots = new List<LegacyHabPlanner.SectorSlot>();
			List<TIHabModuleState> list = new List<TIHabModuleState>();
			List<LegacyHabPlanner.SectorSlot> list2 = new List<LegacyHabPlanner.SectorSlot>();
			List<TIHabModuleTemplate> list3 = new List<TIHabModuleTemplate>();
			CS$<>8__locals1.moduleUpgradeOptions = new List<TIHabModuleTemplate>();
			IEnumerable<TISectorState> sectors = CS$<>8__locals1.hab.sectors;
			Func<TISectorState, bool> func;
			if ((func = CS$<>8__locals1.<>9__15) == null)
			{
				func = (CS$<>8__locals1.<>9__15 = (TISectorState s) => s.faction == CS$<>8__locals1.faction);
			}
			foreach (TISectorState tisectorState in sectors.Where<TISectorState>(func).ToList<TISectorState>())
			{
				foreach (TIHabModuleState tihabModuleState in tisectorState.habModules)
				{
					if (tihabModuleState.empty || tihabModuleState.destroyed)
					{
						CS$<>8__locals1.emptySlots.Add(new LegacyHabPlanner.SectorSlot
						{
							sector = tisectorState.sectorNum,
							slot = tihabModuleState.slot
						});
					}
					else if (tihabModuleState.CanUpgrade(CS$<>8__locals1.faction))
					{
						list2.Add(new LegacyHabPlanner.SectorSlot
						{
							sector = tisectorState.sectorNum,
							slot = tihabModuleState.slot
						});
						list.Add(tihabModuleState);
						list3.Add(tihabModuleState.moduleTemplate);
						CS$<>8__locals1.moduleUpgradeOptions.Add(tihabModuleState.moduleTemplate.UpgradesTo);
					}
				}
			}
			CS$<>8__locals1.allowedModules = ((goal != null) ? goal.allowedModules() : null) ?? CS$<>8__locals1.hab.AllowedModules(CS$<>8__locals1.faction);
			if (!CS$<>8__locals1.allowedModules.Any<TIHabModuleTemplate>())
			{
				return null;
			}
			CS$<>8__locals1.bestPowerModuleTemplate = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource).MaxBy<TIHabModuleTemplate, float>((TIHabModuleTemplate x) => (float)base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x) + (x.IsSolarPower ? 0.01f : 0f));
			if (CS$<>8__locals1.bestPowerModuleTemplate == null)
			{
				return null;
			}
			CS$<>8__locals1.availablePower = CS$<>8__locals1.hab.NetPower(true, true);
			CS$<>8__locals1.immediatelyAvailablePower = CS$<>8__locals1.hab.NetPower(false, true);
			CS$<>8__locals1.powerModulesAreUnderConstruction = CS$<>8__locals1.hab.UnderConstructionModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.powerSource);
			IEnumerable<TIHabModuleTemplate> enumerable = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource || base.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x));
			if (enumerable.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.powerSource))
			{
				CS$<>8__locals1.allowedModules = enumerable;
			}
			CS$<>8__locals1.habShipyards = from x in CS$<>8__locals1.hab.AllModules()
				where x.moduleTemplate.allowsShipConstruction
				select x;
			CS$<>8__locals1.targetedModule = this.GetTargetedModuleForHab(CS$<>8__locals1.faction, CS$<>8__locals1.hab, goal, CS$<>8__locals1.allowedModules, CS$<>8__locals1.habShipyards);
			CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>(delegate(TIHabModuleTemplate x)
			{
				if (x.mine && CS$<>8__locals1.hab.HasMine && CS$<>8__locals1.hab.mine.powered)
				{
					int num7 = -base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x);
					int num8 = CS$<>8__locals1.immediatelyAvailablePower + CS$<>8__locals1.hab.mine.PowerConsumed();
					return num7 <= num8;
				}
				return true;
			});
			CS$<>8__locals1.wantConstructionModule = !CS$<>8__locals1.faction.IsAlienFaction && !CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(CS$<>8__locals1.hab, true, true);
			if (!CS$<>8__locals1.allowedModules.Contains(CS$<>8__locals1.targetedModule) & CS$<>8__locals1.wantConstructionModule)
			{
				if (CS$<>8__locals1.allowedModules.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding))
				{
					CS$<>8__locals1.targetedModule = CS$<>8__locals1.allowedModules.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding && x.tier == 1);
				}
			}
			CS$<>8__locals1.excessPower = CS$<>8__locals1.availablePower;
			if (CS$<>8__locals1.targetedModule != null)
			{
				CS$<>8__locals1.excessPower += CS$<>8__locals1.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(CS$<>8__locals1.targetedModule);
				if (CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(CS$<>8__locals1.targetedModule))
				{
					CS$<>8__locals1.excessPower -= CS$<>8__locals1.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(CS$<>8__locals1.targetedModule.UpgradesFrom);
				}
			}
			if (CS$<>8__locals1.excessPower < 0)
			{
				CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource || x.coreModule || x.EnablesLocalFounding).ToList<TIHabModuleTemplate>();
			}
			TIHabModuleState tihabModuleState2 = null;
			bool flag = false;
			TIHabModuleState tihabModuleState3 = (from x in CS$<>8__locals1.hab.OkayModules()
				where x.moduleTemplate.powerSource
				select x).MinBy<TIHabModuleState, int>((TIHabModuleState x) => base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x.moduleTemplate));
			if (tihabModuleState3 != null)
			{
				int num = CS$<>8__locals1.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(tihabModuleState3.moduleTemplate);
				int num2 = CS$<>8__locals1.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(CS$<>8__locals1.bestPowerModuleTemplate);
				flag = CS$<>8__locals1.immediatelyAvailablePower >= num && CS$<>8__locals1.excessPower >= num && CS$<>8__locals1.emptySlots.Count == 0;
				if (flag || (CS$<>8__locals1.emptySlots.Count <= 1 && !CS$<>8__locals1.powerModulesAreUnderConstruction && num < num2))
				{
					tihabModuleState2 = tihabModuleState3;
					CS$<>8__locals1.availablePower -= num;
					CS$<>8__locals1.excessPower -= num;
				}
			}
			if (tihabModuleState2 == null)
			{
				TIHabModuleTemplate constructionModuleTemplate = CS$<>8__locals1.allowedModules.FirstOrDefault<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding && x.tier == 1);
				if (CS$<>8__locals1.wantConstructionModule && constructionModuleTemplate != null && (CS$<>8__locals1.emptySlots.Count == 0 || CS$<>8__locals1.availablePower < -CS$<>8__locals1.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(constructionModuleTemplate)) && constructionModuleTemplate.MinimumBoostCostToday(CS$<>8__locals1.faction, CS$<>8__locals1.hab, false).GetSingleCostValue(FactionResource.Boost) == 0f)
				{
					tihabModuleState2 = (from x in CS$<>8__locals1.hab.FunctionalModules()
						where !x.moduleTemplate.mine && !x.moduleTemplate.coreModule && !x.moduleTemplate.powerSource && x.moduleTemplate.power <= constructionModuleTemplate.power
						select x).FirstOrDefault<TIHabModuleState>();
				}
			}
			if (tihabModuleState2 != null)
			{
				CS$<>8__locals1.emptySlots.Add(new LegacyHabPlanner.SectorSlot
				{
					sector = tihabModuleState2.sector.sectorNum,
					slot = tihabModuleState2.slot
				});
			}
			if (CS$<>8__locals1.faction.AISavingTarget.active)
			{
				TIHabModuleTemplate tihabModuleTemplate = CS$<>8__locals1.faction.AISavingTarget.desiredPurchase as TIHabModuleTemplate;
				if (tihabModuleTemplate != null && CS$<>8__locals1.faction.AISavingTarget.location.ref_hab == CS$<>8__locals1.hab && (CS$<>8__locals1.emptySlots.Count == 0 || (CS$<>8__locals1.emptySlots.Count == 1 && -tihabModuleTemplate.power > CS$<>8__locals1.availablePower)))
				{
					CS$<>8__locals1.faction.AIClearSavingTarget("Can't build module. No room at " + CS$<>8__locals1.hab.displayName);
				}
			}
			if (CS$<>8__locals1.emptySlots.Count <= 0 && list2.Count <= 0)
			{
				return null;
			}
			TIHabModuleTemplate tihabModuleTemplate2 = null;
			if (!this.habModuleSelections.ContainsKey(CS$<>8__locals1.faction))
			{
				this.habModuleSelections[CS$<>8__locals1.faction] = new Dictionary<TIHabState, string>();
			}
			if (!this.habModuleSelections[CS$<>8__locals1.faction].ContainsKey(CS$<>8__locals1.hab))
			{
				this.habModuleSelections[CS$<>8__locals1.faction][CS$<>8__locals1.hab] = null;
			}
			string text = this.habModuleSelections[CS$<>8__locals1.faction][CS$<>8__locals1.hab];
			if (text != null)
			{
				tihabModuleTemplate2 = TemplateManager.Find<TIHabModuleTemplate>(text, false);
			}
			if (tihabModuleTemplate2 == null)
			{
				if (CS$<>8__locals1.emptySlots.Count == 0)
				{
					CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>(new Func<TIHabModuleTemplate, bool>(CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4));
				}
				else if (CS$<>8__locals1.hab.IsBase && CS$<>8__locals1.emptySlots.Count == 1 && CS$<>8__locals1.emptySlots[0].sector == 0 && CS$<>8__locals1.emptySlots[0].slot == 1)
				{
					CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine || x.coreModule || base.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x));
				}
				bool habCanResupply = CS$<>8__locals1.hab.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.allowsResupply);
				IEnumerable<TIHabModuleTemplate> allowedPowerModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.powerSource);
				if (allowedPowerModules.Any<TIHabModuleTemplate>())
				{
					if (CS$<>8__locals1.targetedModule != null)
					{
						if (CS$<>8__locals1.excessPower + (CS$<>8__locals1.emptySlots.Count - ((!CS$<>8__locals1.hab.HasMine) ? 1 : 0)) * CS$<>8__locals1.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(CS$<>8__locals1.bestPowerModuleTemplate) + (from x in list
							where x.moduleTemplate.powerSource
							where allowedPowerModules.Contains(x.moduleTemplate.UpgradesTo)
							select x).Sum<TIHabModuleState>((TIHabModuleState x) => base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x.moduleTemplate.UpgradesTo) - base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x.moduleTemplate)) < 0)
						{
							allowedPowerModules = Enumerable.Empty<TIHabModuleTemplate>();
						}
					}
					allowedPowerModules = allowedPowerModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x == CS$<>8__locals1.bestPowerModuleTemplate);
				}
				allowedPowerModules = allowedPowerModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => CS$<>8__locals1.emptySlots.Count > 1 || base.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x) || (CS$<>8__locals1.availablePower < 0 && base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x) >= -CS$<>8__locals1.availablePower) || (CS$<>8__locals1.targetedModule != null && CS$<>8__locals1.targetedModule.mine && base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x) >= -CS$<>8__locals1.excessPower));
				CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.powerSource || allowedPowerModules.Contains(x));
				Dictionary<TIHabModuleTemplate, float> moduleUtilityScores = new Dictionary<TIHabModuleTemplate, float>();
				Action action = delegate
				{
					if (moduleUtilityScores.Any<KeyValuePair<TIHabModuleTemplate, float>>())
					{
						IEnumerable<TIHabModuleTemplate> keys = moduleUtilityScores.Keys;
						Func<TIHabModuleTemplate, bool> func2;
						if ((func2 = CS$<>8__locals1.<>9__35) == null)
						{
							func2 = (CS$<>8__locals1.<>9__35 = (TIHabModuleTemplate x) => !CS$<>8__locals1.allowedModules.Contains(x));
						}
						foreach (TIHabModuleTemplate tihabModuleTemplate4 in keys.Where<TIHabModuleTemplate>(func2).ToList<TIHabModuleTemplate>())
						{
							moduleUtilityScores.Remove(tihabModuleTemplate4);
						}
						return;
					}
					TIHabModuleState tihabModuleState5 = (from x in CS$<>8__locals1.hab.OkayModules()
						where x.moduleTemplate.constructionModule
						select x).FirstOrDefault<TIHabModuleState>();
					float num9 = CS$<>8__locals1.hab.SpaceCombatValue() + CS$<>8__locals1.hab.UnderConstructionModules().Sum<TIHabModuleState>((TIHabModuleState x) => x.SpaceCombatValue());
					int num10 = CS$<>8__locals1.faction.habs.Select<TIHabState, IEnumerable<TIHabModuleState>>((TIHabState x) => from x in x.AllModules()
						where x.moduleTemplate.allowsShipConstruction
						select x).Count<IEnumerable<TIHabModuleState>>();
					int num11 = CS$<>8__locals1.habShipyards.Count<TIHabModuleState>();
					int num12 = (int)(from x in CS$<>8__locals1.hab.AllModules()
						where x.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.Farm)
						select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.specialRulesValue);
					foreach (TIHabModuleTemplate tihabModuleTemplate5 in CS$<>8__locals1.allowedModules)
					{
						float num13 = AIEvaluators.EvaluateHabModule(CS$<>8__locals1.faction, CS$<>8__locals1.hab, tihabModuleTemplate5, CS$<>8__locals1.wantConstructionModule, habCanResupply, num9, tihabModuleState5, num10, num11, CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(tihabModuleTemplate5), num12, CS$<>8__locals1.maxxedOutSpecialRules, CS$<>8__locals1.maxxedOutTechCategories);
						if (num13 > 0f)
						{
							moduleUtilityScores.Add(tihabModuleTemplate5, num13);
						}
					}
				};
				if (CS$<>8__locals1.allowedModules.Any<TIHabModuleTemplate>())
				{
					Func<TIHabModuleTemplate, int> ExcessPowerConsumed = delegate(TIHabModuleTemplate x)
					{
						if (x == null || x.powerSource || x == CS$<>8__locals1.targetedModule)
						{
							return 0;
						}
						return -base.<SelectHabModuleForBuilding>g__GetUpgradeAdjustedPower|5(x);
					};
					int num3 = CS$<>8__locals1.allowedModules.Max<TIHabModuleTemplate>(ExcessPowerConsumed);
					if (CS$<>8__locals1.emptySlots.Count == 1)
					{
						CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => -base.<SelectHabModuleForBuilding>g__GetAdjustedPower|0(x) <= CS$<>8__locals1.availablePower);
					}
					else if (CS$<>8__locals1.excessPower >= num3)
					{
						CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.powerSource || base.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x));
					}
					else if (CS$<>8__locals1.wantConstructionModule && CS$<>8__locals1.emptySlots.Count == 1 && !CS$<>8__locals1.faction.IsAlienFaction)
					{
						CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.EnablesLocalFounding);
					}
					else
					{
						TIHabModuleTemplate tihabModuleTemplate3 = CS$<>8__locals1.targetedModule;
						if (tihabModuleTemplate3 == null)
						{
							action();
							tihabModuleTemplate3 = moduleUtilityScores.Keys.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => !x.powerSource).SelectRandomWeightedItem<TIHabModuleTemplate>((TIHabModuleTemplate x) => moduleUtilityScores[x], -1f, 1E-37f);
						}
						if (CS$<>8__locals1.availablePower < 0 || ExcessPowerConsumed(tihabModuleTemplate3) > CS$<>8__locals1.excessPower || (tihabModuleState2 != null && tihabModuleState2.moduleTemplate.powerSource && !flag))
						{
							if (allowedPowerModules.Any<TIHabModuleTemplate>())
							{
								CS$<>8__locals1.allowedModules = allowedPowerModules;
							}
							else
							{
								CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.coreModule);
							}
						}
						else
						{
							if (CS$<>8__locals1.targetedModule != null && CS$<>8__locals1.emptySlots.Count<LegacyHabPlanner.SectorSlot>() <= 1 && !CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(CS$<>8__locals1.targetedModule))
							{
								CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x == CS$<>8__locals1.targetedModule || base.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x));
							}
							CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => (!x.powerSource || CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x)) && (ExcessPowerConsumed(x) <= CS$<>8__locals1.excessPower || x.power >= 0));
						}
					}
				}
				if (CS$<>8__locals1.allowedModules.Any<TIHabModuleTemplate>())
				{
					int availableMissionControl = CS$<>8__locals1.faction.AvailableMissionControl;
					CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>(delegate(TIHabModuleTemplate x)
					{
						if (x.missionControl >= 0)
						{
							return true;
						}
						int num14 = 0;
						if (CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x))
						{
							num14 = -x.UpgradesFrom.missionControl;
						}
						return availableMissionControl >= -x.missionControl - num14;
					});
				}
				if (CS$<>8__locals1.allowedModules.Any<TIHabModuleTemplate>() && !CS$<>8__locals1.faction.IsAlienFaction && CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Money) < 1000f && CS$<>8__locals1.faction.GetMonthlyIncome(FactionResource.Money, true, false) < 0f)
				{
					float monthlyMoneyIncome = CS$<>8__locals1.faction.GetMonthlyIncome(FactionResource.Money, true, false);
					CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.incomeMoney_month >= 0f || x.power >= 0 || x.mine || x.missionControl > 0 || monthlyMoneyIncome > Mathf.Abs(x.MonthlyResourceIncome(FactionResource.Money, CS$<>8__locals1.hab, CS$<>8__locals1.faction)));
				}
				if (CS$<>8__locals1.allowedModules.Any<TIHabModuleTemplate>())
				{
					IEnumerable<string> enumerable2 = CS$<>8__locals1.allowedModules.Select<TIHabModuleTemplate, string>((TIHabModuleTemplate x) => x.dataName);
					if (CS$<>8__locals1.targetedModule != null)
					{
						if (enumerable2.Contains(CS$<>8__locals1.targetedModule.dataName))
						{
							tihabModuleTemplate2 = CS$<>8__locals1.targetedModule;
						}
						else
						{
							IEnumerable<string> enumerable3 = enumerable2;
							TIHabModuleTemplate upgradesFrom = CS$<>8__locals1.targetedModule.UpgradesFrom;
							if (enumerable3.Contains((upgradesFrom != null) ? upgradesFrom.dataName : null))
							{
								tihabModuleTemplate2 = CS$<>8__locals1.targetedModule.UpgradesFrom;
							}
							else
							{
								IEnumerable<string> enumerable4 = enumerable2;
								TIHabModuleTemplate upgradesFrom2 = CS$<>8__locals1.targetedModule.UpgradesFrom;
								string text2;
								if (upgradesFrom2 == null)
								{
									text2 = null;
								}
								else
								{
									TIHabModuleTemplate upgradesFrom3 = upgradesFrom2.UpgradesFrom;
									text2 = ((upgradesFrom3 != null) ? upgradesFrom3.dataName : null);
								}
								if (enumerable4.Contains(text2))
								{
									TIHabModuleTemplate upgradesFrom4 = CS$<>8__locals1.targetedModule.UpgradesFrom;
									tihabModuleTemplate2 = ((upgradesFrom4 != null) ? upgradesFrom4.UpgradesFrom : null);
								}
								else if (CS$<>8__locals1.emptySlots.Count <= 1 && !CS$<>8__locals1.targetedModule.mine && !CS$<>8__locals1.targetedModule.coreModule)
								{
									CS$<>8__locals1.allowedModules = CS$<>8__locals1.allowedModules.Where<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.mine || x.coreModule || base.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(x));
								}
							}
						}
					}
					if (tihabModuleTemplate2 == null)
					{
						action();
						tihabModuleTemplate2 = moduleUtilityScores.SelectRandomWeightedItem<KeyValuePair<TIHabModuleTemplate, float>>((KeyValuePair<TIHabModuleTemplate, float> x) => x.Value, -1f, 1E-37f).Key;
					}
				}
				if (tihabModuleTemplate2 == null)
				{
					return null;
				}
				this.habModuleSelections[CS$<>8__locals1.faction][CS$<>8__locals1.hab] = tihabModuleTemplate2.dataName;
			}
			int num4 = -1;
			int num5 = -1;
			bool flag2 = CS$<>8__locals1.<SelectHabModuleForBuilding>g__CanBeUpgrade|4(tihabModuleTemplate2);
			if (tihabModuleTemplate2.coreModule)
			{
				num4 = 0;
				num5 = 0;
			}
			else if (tihabModuleTemplate2.mine)
			{
				num4 = 0;
				num5 = 1;
			}
			else if (num4 == -1 || num5 == -1)
			{
				if (flag2)
				{
					using (List<TIHabModuleState>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							TIHabModuleState tihabModuleState4 = enumerator2.Current;
							if (tihabModuleState4.moduleTemplate.UpgradesTo.dataName == tihabModuleTemplate2.dataName)
							{
								num4 = tihabModuleState4.sectorNum;
								num5 = tihabModuleState4.slot;
								break;
							}
						}
						goto IL_12B1;
					}
				}
				CS$<>8__locals1.emptySlots = (from x in CS$<>8__locals1.emptySlots
					orderby x.sector, TIUtilities.RandomRange(0, int.MaxValue)
					select x).ToList<LegacyHabPlanner.SectorSlot>();
				foreach (LegacyHabPlanner.SectorSlot sectorSlot in CS$<>8__locals1.emptySlots)
				{
					if (CS$<>8__locals1.hab.sectors[sectorSlot.sector].ValidModuleForSlot(tihabModuleTemplate2, sectorSlot.slot))
					{
						num4 = sectorSlot.sector;
						num5 = sectorSlot.slot;
						break;
					}
				}
			}
			IL_12B1:
			if (num4 == -1 || num5 == -1)
			{
				this.habModuleSelections[CS$<>8__locals1.faction][CS$<>8__locals1.hab] = null;
				return null;
			}
			if (CS$<>8__locals1.faction.IsAlienFaction && !AIEvaluators.ShouldAliensGoLoud() && (tihabModuleTemplate2.spaceCombatModule || tihabModuleTemplate2.SpecialRules.Contains(HabModuleSpecialRule.DropTroops)))
			{
				return null;
			}
			TIResourcesCost tiresourcesCost = tihabModuleTemplate2.MinimumBoostCostToday(CS$<>8__locals1.faction, CS$<>8__locals1.hab, flag2);
			if (!tiresourcesCost.CanAfford_AI(CS$<>8__locals1.faction, tihabModuleTemplate2, CS$<>8__locals1.hab, (goal != null) ? goal.importance : 1, false, false, 1f, null, float.PositiveInfinity) || AIEvaluators.ShouldNotBuildHabModuleRightNow(tihabModuleTemplate2, CS$<>8__locals1.faction, CS$<>8__locals1.hab) || !AIEvaluators.ShouldPayTodaysBoostCost(tihabModuleTemplate2, CS$<>8__locals1.faction, CS$<>8__locals1.hab, flag2, 180))
			{
				return null;
			}
			string[] array = new string[10];
			array[0] = CS$<>8__locals1.faction.displayName;
			array[1] = " buying ";
			array[2] = tihabModuleTemplate2.displayName;
			array[3] = " at ";
			array[4] = CS$<>8__locals1.hab.displayName;
			array[5] = ", ";
			int num6 = 6;
			TINaturalSpaceObjectState ref_naturalSpaceObject = CS$<>8__locals1.hab.ref_naturalSpaceObject;
			array[num6] = ((ref_naturalSpaceObject != null) ? ref_naturalSpaceObject.displayName : null) ?? "unknown";
			array[7] = ", spending ";
			array[8] = tiresourcesCost.GetSingleCostValue(FactionResource.Boost).ToString();
			array[9] = " boost.";
			TIFactionState.LogAI(string.Concat(array), false);
			if (tiresourcesCost.GetSingleCostValue(FactionResource.Boost) > 0f && AIEvaluators.ShouldRateLimitBoostExpenditure(tihabModuleTemplate2, CS$<>8__locals1.faction, CS$<>8__locals1.hab))
			{
				TIFactionState.BoostAccountName boostAccountName = (CS$<>8__locals1.hab.IsBase ? TIFactionState.BoostAccountName.Base : TIFactionState.BoostAccountName.Station);
				CS$<>8__locals1.faction.boostAccounts[boostAccountName] = TITimeState.Now();
			}
			CS$<>8__locals1.faction.playerControl.StartAction(new BuildHabModuleAction(tihabModuleTemplate2, CS$<>8__locals1.hab.sectors[num4], num5, tiresourcesCost, null));
			if (CS$<>8__locals1.faction.AISavingTarget.active && CS$<>8__locals1.faction.AISavingTarget.location.ref_hab == CS$<>8__locals1.hab && CS$<>8__locals1.faction.AISavingTarget.desiredPurchase.dataName == tihabModuleTemplate2.dataName)
			{
				CS$<>8__locals1.faction.AIClearSavingTarget("Building module");
			}
			this.habModuleSelections[CS$<>8__locals1.faction][CS$<>8__locals1.hab] = null;
			if (tihabModuleTemplate2.mine && CS$<>8__locals1.faction.boostAccounts[TIFactionState.BoostAccountName.Org] == null)
			{
				CS$<>8__locals1.faction.boostAccounts[TIFactionState.BoostAccountName.Org] = TITimeState.Now();
			}
			return tihabModuleTemplate2;
		}

		// Token: 0x06005ACB RID: 23243 RVA: 0x002B9310 File Offset: 0x002B7510
		public static TIHabSiteState SelectHabSiteForDevelopment(TIFactionState faction, float lowDist_AU, float highDist_AU, List<TIHabSiteState> sitesToSkip, bool forcePlanetarySystem = false, bool forceSunOrbitingAsteroid = false, bool forceBest = false, TISpaceBodyState skipThis = null, int requiredMaxTier = 1, bool usePercentChangeScoring = false, Func<FactionResource, float> GetCurrentMonthlyIncome = null)
		{
			List<TISpaceBodyState> list = (from x in AIEvaluators.SpaceBodiesBetween(lowDist_AU, highDist_AU)
				where x.IsSafeForColonization(faction, HabType.Any)
				select x).ToList<TISpaceBodyState>();
			if (skipThis != null)
			{
				list.Remove(skipThis);
			}
			if (forcePlanetarySystem)
			{
				list = list.Where<TISpaceBodyState>((TISpaceBodyState x) => x.objectType == SpaceObjectType.Planet || x.objectType == SpaceObjectType.PlanetaryMoon || x.objectType == SpaceObjectType.AsteroidalMoon || x.objectType == SpaceObjectType.DwarfPlanet).ToList<TISpaceBodyState>();
			}
			else if (forceSunOrbitingAsteroid)
			{
				list = list.Where<TISpaceBodyState>((TISpaceBodyState x) => x.barycenter.isSun && (x.objectType == SpaceObjectType.Asteroid || x.objectType == SpaceObjectType.DwarfPlanet)).ToList<TISpaceBodyState>();
			}
			list.RemoveAll((TISpaceBodyState x) => x.maxHabTier < requiredMaxTier);
			TIHabModuleTemplate tihabModuleTemplate = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining)
				orderby x.tier
				where !x.automated
				where x.alienModule == faction.IsAlienFaction
				select x).FirstOrDefault<TIHabModuleTemplate>();
			List<TIFactionGoalState> list2 = faction.GoalsOfType(GoalType.FoundBase, false, true);
			HashSet<TISpaceBodyState> hashSet = new HashSet<TISpaceBodyState>(from x in faction.habs
				where x.ref_system != null
				select x.ref_system);
			Dictionary<TIHabSiteState, float> dictionary = new Dictionary<TIHabSiteState, float>();
			using (List<TISpaceBodyState>.Enumerator enumerator = list.GetEnumerator())
			{
				Func<TIHabSiteState, bool> <>9__10;
				Func<TIHabState, bool> <>9__11;
				while (enumerator.MoveNext())
				{
					TISpaceBodyState body = enumerator.Current;
					IEnumerable<TIHabSiteState> habSites = body.habSites;
					Func<TIHabSiteState, bool> func;
					if ((func = <>9__10) == null)
					{
						func = (<>9__10 = (TIHabSiteState x) => !sitesToSkip.Contains(x) && !x.hasPlannedOrOperatingBase && faction.Prospected(x));
					}
					Func<TIFactionGoalState, bool> <>9__12;
					foreach (TIHabSiteState tihabSiteState in habSites.Where<TIHabSiteState>(func).ToList<TIHabSiteState>())
					{
						float num;
						if (tihabModuleTemplate != null && usePercentChangeScoring)
						{
							num = AIEvaluators.EvaluateHabModule_PercentChange(faction, tihabSiteState, tihabModuleTemplate, null, null, GetCurrentMonthlyIncome, false, false);
							float num2 = 1f;
							if (hashSet.Contains(body.ref_system))
							{
								num2 = 1.6f;
								IEnumerable<TIHabState> habsInSystem = body.ref_system.habsInSystem;
								Func<TIHabState, bool> func2;
								if ((func2 = <>9__11) == null)
								{
									func2 = (<>9__11 = (TIHabState x) => x.IsBase && x.faction == faction);
								}
								int num3 = habsInSystem.Count<TIHabState>(func2);
								IEnumerable<TIFactionGoalState> enumerable = list2;
								Func<TIFactionGoalState, bool> func3;
								if ((func3 = <>9__12) == null)
								{
									func3 = (<>9__12 = delegate(TIFactionGoalState x)
									{
										TIGameState tigameState = x.target();
										return ((tigameState != null) ? tigameState.ref_system : null) == body.ref_system;
									});
								}
								int num4 = num3 + enumerable.Count<TIFactionGoalState>(func3);
								num2 = 1f + (num2 - 1f) * 9f / (float)(num4 + 8);
							}
							float num5 = 1f;
							TIHabState primaryHab = faction.primaryHab;
							if (((primaryHab != null) ? primaryHab.ref_system : null) != null && body.ref_system != null)
							{
								double semiMajorAxis_AU = faction.primaryHab.ref_system.semiMajorAxis_AU;
								num5 = 1f + 0.15f * (float)((semiMajorAxis_AU - body.ref_system.semiMajorAxis_AU) / (semiMajorAxis_AU - GameStateManager.Jupiter().semiMajorAxis_AU));
							}
							num *= num2 * num5;
						}
						else
						{
							num = AIEvaluators.EvaluateHabSite(faction, tihabSiteState, true, true, true);
						}
						dictionary.Add(tihabSiteState, num);
					}
				}
			}
			if (dictionary.Count <= 0)
			{
				return null;
			}
			if (forceBest)
			{
				return dictionary.MaxBy<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Key;
			}
			return dictionary.OrderByDescending<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Take<KeyValuePair<TIHabSiteState, float>>(Mathf.Max(2, 14 - TIGlobalValuesState.GlobalValues.difficulty * 2)).ToList<KeyValuePair<TIHabSiteState, float>>()
				.SelectRandomWeightedItem<KeyValuePair<TIHabSiteState, float>>((KeyValuePair<TIHabSiteState, float> x) => x.Value, -1f, 1E-37f)
				.Key;
		}

		// Token: 0x06005ACC RID: 23244 RVA: 0x002B97C0 File Offset: 0x002B79C0
		public static TIHabSiteState SelectHabSiteForDevelopment(TIFactionState faction, TISpaceBodyState spaceBody, List<TIHabSiteState> sitesToSkip, bool system = false, bool forceBest = false, int requiredMaxTier = 1, bool usePercentChangeScoring = false, Func<FactionResource, float> GetCurrentMonthlyIncome = null)
		{
			Dictionary<TIHabSiteState, float> dictionary = new Dictionary<TIHabSiteState, float>();
			List<TISpaceBodyState> list = new List<TISpaceBodyState> { spaceBody };
			if (system)
			{
				if (spaceBody.isaMoon)
				{
					list.Add(spaceBody.barycenter.ref_spaceBody);
					list.AddRange(spaceBody.barycenter.ref_spaceBody.naturalSatellites);
				}
				else
				{
					list.AddRange(spaceBody.naturalSatellites);
				}
			}
			TIHabModuleTemplate tihabModuleTemplate = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining)
				orderby x.tier
				where !x.automated
				where x.alienModule == faction.IsAlienFaction
				select x).FirstOrDefault<TIHabModuleTemplate>();
			Func<TIHabSiteState, bool> <>9__6;
			foreach (TISpaceBodyState tispaceBodyState in list.Distinct<TISpaceBodyState>().ToList<TISpaceBodyState>())
			{
				IEnumerable<TIHabSiteState> habSites = tispaceBodyState.habSites;
				Func<TIHabSiteState, bool> func;
				if ((func = <>9__6) == null)
				{
					func = (<>9__6 = (TIHabSiteState x) => !sitesToSkip.Contains(x) && !x.hasPlannedOrOperatingBase && faction.Prospected(x) && x.maxTier >= requiredMaxTier);
				}
				foreach (TIHabSiteState tihabSiteState in habSites.Where<TIHabSiteState>(func).ToList<TIHabSiteState>())
				{
					float num;
					if (tihabModuleTemplate != null && usePercentChangeScoring)
					{
						num = AIEvaluators.EvaluateHabModule_PercentChange(faction, tihabSiteState, tihabModuleTemplate, null, null, GetCurrentMonthlyIncome, false, false);
					}
					else
					{
						num = AIEvaluators.EvaluateHabSite(faction, tihabSiteState, true, true, true);
					}
					dictionary.Add(tihabSiteState, num);
				}
			}
			if (dictionary.Count <= 0)
			{
				return null;
			}
			if (forceBest)
			{
				return dictionary.MaxBy<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Key;
			}
			return dictionary.OrderByDescending<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Take<KeyValuePair<TIHabSiteState, float>>(Mathf.Max(2, 6 - TIGlobalValuesState.GlobalValues.difficulty)).SelectRandomWeightedItem<KeyValuePair<TIHabSiteState, float>>((KeyValuePair<TIHabSiteState, float> x) => x.Value, -1f, 1E-37f)
				.Key;
		}

		// Token: 0x06005ACD RID: 23245 RVA: 0x002B9A44 File Offset: 0x002B7C44
		public static TIHabSiteState SelectHabSiteForDevelopment(TIFactionState faction, IEnumerable<TIHabSiteState> habSites, bool forceBest = false, bool usePercentChangeScoring = false, Func<FactionResource, float> GetCurrentMonthlyIncome = null)
		{
			habSites = habSites.Where<TIHabSiteState>((TIHabSiteState x) => !x.hasPlannedOrOperatingBase && faction.Prospected(x)).ToList<TIHabSiteState>();
			if (!habSites.Any<TIHabSiteState>())
			{
				return null;
			}
			TIHabModuleTemplate mineTemplate = (from x in ArchetypeDecision.GetTemplatesWithinArchetype(ArchetypeDecision.HabModuleArchetype.Mining)
				orderby x.tier
				where !x.automated
				where x.alienModule == faction.IsAlienFaction
				select x).FirstOrDefault<TIHabModuleTemplate>();
			List<TIFactionGoalState> foundBaseGoals = faction.GoalsOfType(GoalType.FoundBase, false, true);
			HashSet<TISpaceBodyState> colonizedSystems = new HashSet<TISpaceBodyState>(from x in faction.habs
				where x.ref_system != null
				select x.ref_system);
			Func<TIHabState, bool> <>9__9;
			Dictionary<TIHabSiteState, float> dictionary = habSites.ToDictionary<TIHabSiteState, TIHabSiteState, float>((TIHabSiteState x) => x, delegate(TIHabSiteState site)
			{
				TISpaceBodyState body = site.ref_spaceBody;
				float num;
				if ((mineTemplate != null) & usePercentChangeScoring)
				{
					num = AIEvaluators.EvaluateHabModule_PercentChange(faction, site, mineTemplate, null, null, GetCurrentMonthlyIncome, false, false);
					float num2 = 1f;
					if (colonizedSystems.Contains(body.ref_system))
					{
						num2 = 1.6f;
						IEnumerable<TIHabState> habsInSystem = body.ref_system.habsInSystem;
						Func<TIHabState, bool> func;
						if ((func = <>9__9) == null)
						{
							func = (<>9__9 = (TIHabState x) => x.IsBase && x.faction == faction);
						}
						int num3 = habsInSystem.Count<TIHabState>(func) + foundBaseGoals.Count<TIFactionGoalState>(delegate(TIFactionGoalState x)
						{
							TIGameState tigameState = x.target();
							return ((tigameState != null) ? tigameState.ref_system : null) == body.ref_system;
						});
						num2 = 1f + (num2 - 1f) * 9f / (float)(num3 + 8);
					}
					float num4 = 1f;
					TIHabState primaryHab = faction.primaryHab;
					if (((primaryHab != null) ? primaryHab.ref_system : null) != null && body.ref_system != null)
					{
						double semiMajorAxis_AU = faction.primaryHab.ref_system.semiMajorAxis_AU;
						num4 = 1f + 0.15f * (float)((semiMajorAxis_AU - body.ref_system.semiMajorAxis_AU) / (semiMajorAxis_AU - GameStateManager.Jupiter().semiMajorAxis_AU));
					}
					num *= num2 * num4;
				}
				else
				{
					num = AIEvaluators.EvaluateHabSite(faction, site, true, true, true);
				}
				return num;
			});
			if (forceBest)
			{
				return dictionary.MaxBy<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Key;
			}
			return dictionary.OrderByDescending<KeyValuePair<TIHabSiteState, float>, float>((KeyValuePair<TIHabSiteState, float> x) => x.Value).Take<KeyValuePair<TIHabSiteState, float>>(Mathf.Max(2, 14 - TIGlobalValuesState.GlobalValues.difficulty * 2)).ToList<KeyValuePair<TIHabSiteState, float>>()
				.SelectRandomWeightedItem<KeyValuePair<TIHabSiteState, float>>((KeyValuePair<TIHabSiteState, float> x) => x.Value, -1f, 1E-37f)
				.Key;
		}

		// Token: 0x0400415B RID: 16731
		[SerializeField]
		private Dictionary<TIFactionState, Dictionary<TIHabState, string>> habModuleSelections = new Dictionary<TIFactionState, Dictionary<TIHabState, string>>();

		// Token: 0x020012FC RID: 4860
		private struct SectorSlot
		{
			// Token: 0x04006E58 RID: 28248
			public int sector;

			// Token: 0x04006E59 RID: 28249
			public int slot;
		}
	}
}
