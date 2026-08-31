using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000948 RID: 2376
	public class LegacyHumanHabPlanner : LegacyHabPlanner
	{
		// Token: 0x06005ACF RID: 23247 RVA: 0x002B9C60 File Offset: 0x002B7E60
		public LegacyHumanHabPlanner()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.NEOBaseScores = new Dictionary<TIOrbitState, float>();
			foreach (TIOrbitState tiorbitState in GameStateManager.AllOrbits())
			{
				if (tiorbitState.barycenter.isEarth || (tiorbitState.barycenter.barycenter != null && tiorbitState.barycenter.barycenter.isEarth))
				{
					float num = 1f;
					if (tiorbitState.irradiated)
					{
						num /= 5f;
					}
					if (tiorbitState.barycenter.isLagrangePointState && tiorbitState.barycenter.ref_lagrangePoint.lagrangeValue == LagrangeValue.L2)
					{
						num /= 3f;
					}
					this.NEOBaseScores.Add(tiorbitState, num);
				}
			}
		}

		// Token: 0x06005AD0 RID: 23248 RVA: 0x002B9D29 File Offset: 0x002B7F29
		public override void ManageHabGoals(TIFactionState faction)
		{
			this.ManagePriorityProspectFactionGoals_Human(faction);
			this.ManagePriorityHabsConstructionFactionGoals_Human(faction);
		}

		// Token: 0x06005AD1 RID: 23249 RVA: 0x002B9D3C File Offset: 0x002B7F3C
		private void EstablishEarthSpacePresence(TIFactionState faction, List<TIHabState> earthSystemStations)
		{
			List<TIHabState> leostations = faction.LEOStations;
			int count = leostations.Count;
			leostations.Intersect<TIHabState>(faction.MaxedOutHabsForFaction(HabType.Station)).Count<TIHabState>();
			List<TIOrbitState> list = GameStateManager.LEOStates();
			IEnumerable<TIOrbitState> enumerable = list.Where<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null));
			bool flag = enumerable.Any<TIOrbitState>();
			TIFactionState faction2 = faction;
			List<GoalType> foundStationGoals = TIFactionGoalState.FoundStationGoals;
			List<TIGameState> list2 = new List<TIGameState>(faction.fleets);
			list2.Add(faction);
			if (faction2.FindGoals(foundStationGoals, list2, (from x in GameStateManager.NEOStates()
				select x.ref_gameState).ToList<TIGameState>(), TIFactionState.GoalFilter.none, true).Count == 0)
			{
				switch (count)
				{
				case 0:
					if (flag)
					{
						faction.AddGoal(new FactionGoal_FoundMaxStation(faction, 15, enumerable.MinBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km), GoalType.BuildFullStation, null, GoalType.DefendWithFleet, false, null), HandleDuplicateGoalRule.Ignore, null);
					}
					break;
				case 1:
					if (flag && leostations[0].numCompletedModules > 3)
					{
						faction.AddGoal(new FactionGoal_FoundMaxStation(faction, 11, enumerable.MaxBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km), GoalType.BuildFullStation, null, GoalType.DefendWithFleet, false, null), HandleDuplicateGoalRule.Ignore, null);
					}
					break;
				case 2:
				{
					TIOrbitState key = this.NEOBaseScores.Where<KeyValuePair<TIOrbitState, float>>((KeyValuePair<TIOrbitState, float> x) => !x.Key.ref_orbit.isEarthLEO && faction.MyHabsAtLocation(x.Key).Count == 0 && x.Key.ref_orbit.NewStationAllowed(0, null)).SelectRandomWeightedItem<KeyValuePair<TIOrbitState, float>>((KeyValuePair<TIOrbitState, float> x) => x.Value, -1f, 1E-37f).Key;
					if (key != null)
					{
						faction.AddGoal(new FactionGoal_FoundPlatform(faction, 7, key, GoalType.BuildFullStation, null, GoalType.DefendWithFleet), HandleDuplicateGoalRule.Ignore, null);
					}
					break;
				}
				case 3:
					if (faction.fleets.Count > 0 && faction.GoalsOfType(GoalType.BuildRefuellingStation, false, true).Count == 0)
					{
						TIOrbitState key2 = (from x in this.NEOBaseScores
							where !x.Key.ref_orbit.isEarthLEO && faction.MyHabsAtLocation(x.Key).Count == 0 && x.Key.ref_orbit.NewStationAllowed(0, null)
							where x.Key.barycenter != GameStateManager.Earth()
							select x).SelectRandomWeightedItem<KeyValuePair<TIOrbitState, float>>((KeyValuePair<TIOrbitState, float> x) => x.Value, -1f, 1E-37f).Key;
						if (key2 != null)
						{
							faction.AddGoal(new FactionGoal_FoundPlatform(faction, 10, key2, GoalType.BuildRefuellingStation, null, GoalType.None), HandleDuplicateGoalRule.Ignore, null);
						}
					}
					break;
				case 4:
				case 5:
					if (faction.bases.Count > 0)
					{
						TIOrbitState key3 = this.NEOBaseScores.Where<KeyValuePair<TIOrbitState, float>>((KeyValuePair<TIOrbitState, float> x) => !x.Key.ref_orbit.isEarthLEO && faction.MyHabsAtLocation(x.Key).Count == 0 && x.Key.ref_orbit.NewStationAllowed(0, null)).SelectRandomWeightedItem<KeyValuePair<TIOrbitState, float>>((KeyValuePair<TIOrbitState, float> x) => x.Value, -1f, 1E-37f).Key;
						if (key3 != null)
						{
							faction.AddGoal(new FactionGoal_FoundPlatform(faction, 5, key3, GoalType.BuildFullStation, null, GoalType.DefendWithFleet), HandleDuplicateGoalRule.Ignore, null);
						}
					}
					break;
				}
			}
			if (count == 0 && !flag)
			{
				if (faction.GoalsOfType(GoalType.CaptureHab, false, true).None<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_hab.IsStation && x.target().ref_orbit.isEarthLEO))
				{
					Func<TIHabState, bool> <>9__13;
					TIHabState tihabState = list.Select<TIOrbitState, TIHabState>(delegate(TIOrbitState x)
					{
						IEnumerable<TIHabState> stationsInOrbit = x.stationsInOrbit;
						Func<TIHabState, bool> func;
						if ((func = <>9__13) == null)
						{
							func = (<>9__13 = (TIHabState y) => !y.coreFaction.permanentAlly(faction));
						}
						return stationsInOrbit.Where<TIHabState>(func).MinBy<TIHabState, float>((TIHabState z) => z.SpaceCombatValue() + z.AssaultCombatValue(true));
					}).FirstOrDefault<TIHabState>();
					if (tihabState != null)
					{
						faction.AddGoal(new FactionGoal_CaptureHab(faction, 15, tihabState, GoalType.BuildFullStation), HandleDuplicateGoalRule.ResetImportance, null);
					}
				}
			}
			if (faction.needsPrimaryHab)
			{
				TIObjectiveTemplate tiobjectiveTemplate = (from x in faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked)
					where x.targetHabModuleTemplate != null
					select x).FirstOrDefault<TIObjectiveTemplate>();
				TIHabModuleTemplate targetHabModuleTemplate = tiobjectiveTemplate.targetHabModuleTemplate;
				TIGameState targetHabLocationState = tiobjectiveTemplate.targetHabLocationState;
				if (targetHabLocationState.isHabSiteState)
				{
					if (!targetHabLocationState.ref_habSite.hasPlannedOrOperatingBase)
					{
						faction.AddGoal(new FactionGoal_FoundBase(faction, 20, targetHabLocationState.ref_habSite, GoalType.BuildSpecialtyBase, new List<TIHabModuleTemplate> { targetHabModuleTemplate }, GoalType.BuildFullStation, true, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						return;
					}
					if (targetHabLocationState.ref_hab.faction != faction)
					{
						faction.AddGoal(new FactionGoal_CaptureHab(faction, 20, targetHabLocationState.ref_hab, GoalType.BuildFullBase), HandleDuplicateGoalRule.ResetImportance, null);
						return;
					}
					List<TIFactionGoalState> list3 = faction.GoalsWithTarget(targetHabLocationState.ref_hab, new List<GoalType>
					{
						GoalType.BuildFullBase,
						GoalType.BuildMiningBase
					}, true);
					if (list3 != null)
					{
						list3.ForEach(delegate(TIFactionGoalState x)
						{
							faction.RemoveGoal(x);
						});
					}
					faction.AddGoal(new FactionGoal_BuildSpecialtyBase(faction, 20, targetHabLocationState.ref_hab, new List<TIHabModuleTemplate> { targetHabModuleTemplate }, true, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					return;
				}
				else if (targetHabLocationState.isOrbitState)
				{
					if (targetHabLocationState.ref_orbit.NewStationAllowed(0, null))
					{
						faction.AddGoal(new FactionGoal_FoundMaxStation(faction, 20, targetHabLocationState.ref_orbit, GoalType.BuildSpecialtyStation, new List<TIHabModuleTemplate> { targetHabModuleTemplate }, GoalType.DefendWithFleet, true, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						return;
					}
					IEnumerable<TIHabState> enumerable2 = targetHabLocationState.ref_orbit.stationsInOrbit.Where<TIHabState>((TIHabState x) => x.faction == faction);
					if (!enumerable2.Any<TIHabState>())
					{
						faction.AddGoal(new FactionGoal_CaptureHab(faction, 20, targetHabLocationState.ref_orbit.stationsInOrbit.MinBy<TIHabState, float>((TIHabState x) => x.AssaultCombatValue(true)), GoalType.BuildFullStation), HandleDuplicateGoalRule.ResetImportance, null);
						return;
					}
					TIHabState tihabState2 = enumerable2.MaxBy<TIHabState, int>((TIHabState x) => x.NetPower(true, false));
					List<TIFactionGoalState> list4 = faction.GoalsWithTarget(tihabState2, new List<GoalType>
					{
						GoalType.BuildFullStation,
						GoalType.BuildRefuellingStation
					}, true);
					if (list4 != null)
					{
						list4.ForEach(delegate(TIFactionGoalState x)
						{
							faction.RemoveGoal(x);
						});
					}
					faction.AddGoal(new FactionGoal_BuildSpecialtyBase(faction, 20, targetHabLocationState.ref_hab, new List<TIHabModuleTemplate> { targetHabModuleTemplate }, true, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
			}
		}

		// Token: 0x06005AD2 RID: 23250 RVA: 0x002BA404 File Offset: 0x002B8604
		private void ManagePriorityProspectFactionGoals_Human(TIFactionState faction)
		{
			List<TIFactionGoalState> list = faction.factionGoals[GoalType.ProspectSites].Where<TIFactionGoalState>((TIFactionGoalState x) => !x.skipGoal).ToList<TIFactionGoalState>();
			List<TIFactionGoalState> list2 = list.Where<TIFactionGoalState>((TIFactionGoalState x) => !x.InProgress()).ToList<TIFactionGoalState>();
			if (this.gameTime.currentTime.day <= 28 && this.gameTime.currentTime.day % 14 == AIDailyFactionPlanner.factionAIData[faction].every14DaysOffsetLate)
			{
				faction.updateHabPlanningFlag = true;
			}
			if (faction.updateHabPlanningFlag || list2.Count == 0)
			{
				if (faction.CandidateForProspecting(GameStateManager.Luna()))
				{
					if (list.None<TIFactionGoalState>((TIFactionGoalState x) => x.location().ref_spaceBody.isLuna))
					{
						FactionGoal_ProspectSites factionGoal_ProspectSites = new FactionGoal_ProspectSites(faction, 10, GameStateManager.Luna(), false, GoalType.None, GoalType.None, GoalType.None);
						faction.AddGoal(factionGoal_ProspectSites, HandleDuplicateGoalRule.ResetImportance, null);
						list.Add(factionGoal_ProspectSites);
					}
				}
				foreach (TIFactionGoalState tifactionGoalState in list2)
				{
					tifactionGoalState.SetImportance(0);
				}
				list = list.Except<TIFactionGoalState>(list2).ToList<TIFactionGoalState>();
				List<TISpaceBodyState> list3 = list.Select<TIFactionGoalState, TISpaceBodyState>((TIFactionGoalState x) => x.target().ref_spaceBody).ToList<TISpaceBodyState>();
				Dictionary<TISpaceBodyState, float> candidates = (from x in GameStateManager.AllSpaceBodies()
					where faction.CandidateForProspecting(x)
					select x).Except<TISpaceBodyState>(list3).ToDictionary<TISpaceBodyState, TISpaceBodyState, float>((TISpaceBodyState x) => x, (TISpaceBodyState x) => -1f);
				Dictionary<FactionResource, ValueTuple<bool, bool, bool>> incomeChecklist = AIEvaluators.GetSpaceResourceIncomesChecklist((FactionResource x) => AIEvaluators.EstimateFutureIncomePerMonth(faction, x, true, true, true));
				bool useStrategicEvaluation = !AIEvaluators.IsChecklistComplete(incomeChecklist);
				if (useStrategicEvaluation)
				{
					Func<TISpaceBodyState, FactionResource, float> GetSpaceBodyIncomes = delegate(TISpaceBodyState spaceBody, FactionResource resource)
					{
						IEnumerable<TIHabSiteState> enumerable = spaceBody.habSites.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase);
						if (!enumerable.Any<TIHabSiteState>())
						{
							return 0f;
						}
						return enumerable.Max<TIHabSiteState>((TIHabSiteState y) => TIHabSiteState.Statistics.ExpectedSpaceResourcesPerMonth[y][resource]);
					};
					candidates = candidates.Keys.ToDictionary<TISpaceBodyState, TISpaceBodyState, float>((TISpaceBodyState x) => x, (TISpaceBodyState x) => AIEvaluators.EvaluateSpaceResourceIncomes_Strategic((FactionResource resource) => GetSpaceBodyIncomes(x, resource), incomeChecklist));
					TISpaceBodyState bestCandidate = candidates.Keys.MaxBy<TISpaceBodyState, float>((TISpaceBodyState x) => candidates[x]);
					candidates = candidates.Where<KeyValuePair<TISpaceBodyState, float>>((KeyValuePair<TISpaceBodyState, float> x) => (int)x.Value == (int)candidates[bestCandidate]).ToDictionary<KeyValuePair<TISpaceBodyState, float>, TISpaceBodyState, float>((KeyValuePair<TISpaceBodyState, float> x) => x.Key, (KeyValuePair<TISpaceBodyState, float> x) => x.Value);
				}
				else
				{
					candidates = candidates.Keys.ToDictionary<TISpaceBodyState, TISpaceBodyState, float>((TISpaceBodyState x) => x, (TISpaceBodyState x) => AIEvaluators.EvaluateSpaceBody(faction, x, true, true, true));
				}
				int num = Mathf.Min(1, candidates.Count);
				Func<TISpaceBodyState, float> <>9__20;
				for (int i = 0; i < num; i++)
				{
					IEnumerable<TISpaceBodyState> keys = candidates.Keys;
					Func<TISpaceBodyState, float> func;
					if ((func = <>9__20) == null)
					{
						func = (<>9__20 = delegate(TISpaceBodyState x)
						{
							if (!useStrategicEvaluation)
							{
								return Mathf.Pow(candidates[x], 1.5f);
							}
							return 1f;
						});
					}
					TISpaceBodyState tispaceBodyState = keys.SelectRandomWeightedItem<TISpaceBodyState>(func, -1f, 1E-37f);
					faction.AddGoal(new FactionGoal_ProspectSites(faction, 10, tispaceBodyState, false, GoalType.None, GoalType.None, GoalType.None), HandleDuplicateGoalRule.ResetImportance, null);
					candidates.Remove(tispaceBodyState);
				}
				faction.updateHabPlanningFlag = false;
			}
		}

		// Token: 0x06005AD3 RID: 23251 RVA: 0x002BA868 File Offset: 0x002B8A68
		private void ManagePriorityHabsConstructionFactionGoals_Human(TIFactionState faction)
		{
			LegacyHumanHabPlanner.<>c__DisplayClass7_0 CS$<>8__locals1 = new LegacyHumanHabPlanner.<>c__DisplayClass7_0();
			CS$<>8__locals1.faction = faction;
			int count = (from x in CS$<>8__locals1.faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked)
				where x.targetHabModuleTemplate != null
				select x).ToList<TIObjectiveTemplate>().Count;
			List<TIFactionGoalState> list = (from x in CS$<>8__locals1.faction.GoalsOfType(new List<GoalType>(TIFactionGoalState.FoundHabGoals), false, true)
				where !x.InProgress() || x.ref_fleetGoal.assignedFleet != null
				select x).ToList<TIFactionGoalState>();
			CS$<>8__locals1.currentFoundBaseGoals = (from x in list
				select x as FactionGoal_FoundBase into x
				where x != null
				select x).ToList<FactionGoal_FoundBase>();
			CS$<>8__locals1.faction.GoalsOfType(TIFactionGoalState.BuildHabGoals, false, true).ToList<TIFactionGoalState>();
			List<FactionGoal_CaptureHab> list2 = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.CaptureHab, false, true)
				select x as FactionGoal_CaptureHab into x
				where x.captureTarget != null
				select x).ToList<FactionGoal_CaptureHab>();
			int num = CS$<>8__locals1.faction.AvailableMissionControlMinusFutureUsage - list.Count<TIFactionGoalState>();
			if (list2.Any<FactionGoal_CaptureHab>())
			{
				num -= list2.Max<FactionGoal_CaptureHab>((FactionGoal_CaptureHab x) => -x.captureTarget.MissionControlCost(false, null));
			}
			int num2 = 5;
			int num3 = num * 2 / 3;
			int num4 = num - num3;
			LegacyHumanHabPlanner.<>c__DisplayClass7_0 CS$<>8__locals2 = CS$<>8__locals1;
			bool flag;
			if (!CS$<>8__locals1.faction.bases.Any<TIHabState>((TIHabState x) => x.ref_spaceBody.isLuna))
			{
				flag = CS$<>8__locals1.currentFoundBaseGoals.Any<FactionGoal_FoundBase>((FactionGoal_FoundBase x) => x.target().ref_spaceBody.isLuna);
			}
			else
			{
				flag = true;
			}
			CS$<>8__locals2.hasMoonBaseOrGoal = flag;
			Func<TISpaceBodyState, bool> func = delegate(TISpaceBodyState spaceBody)
			{
				if (!spaceBody.hasAvailableHabSites)
				{
					return false;
				}
				if (!CS$<>8__locals1.faction.Prospected(spaceBody))
				{
					return false;
				}
				if (!CS$<>8__locals1.faction.IsAlienProxy)
				{
					if (spaceBody.surfaceBases.Any<TIHabState>((TIHabState x) => x.IsAlien()))
					{
						return false;
					}
				}
				return !CS$<>8__locals1.faction.Prospected(GameStateManager.Luna()) || !GameStateManager.Luna().hasAvailableHabSites || spaceBody.isLuna != CS$<>8__locals1.hasMoonBaseOrGoal;
			};
			List<TISpaceBodyState> list3 = GameStateManager.AllSpaceBodies().Where<TISpaceBodyState>(func).ToList<TISpaceBodyState>();
			List<TISpaceBodyState> list4 = (from x in list3.Where<TISpaceBodyState>((TISpaceBodyState x) => x.ref_spaceBody.habSitesInSystem.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase).Count<TIHabSiteState>() > 1).ToList<TISpaceBodyState>()
				where TIUtilities.RandomFloatValue() < Mathf.Pow(Mathf.Max(x.habSitesInSystem.Average<TIHabSiteState>(delegate(TIHabSiteState y)
				{
					if (!(y.hab == null))
					{
						return 0f;
					}
					return 1f;
				}) - 0.5f, 0f) * 2f, 2f)
				select x).ToList<TISpaceBodyState>();
			if (list4.Count<TISpaceBodyState>() > 0)
			{
				list3 = list4.ToList<TISpaceBodyState>();
			}
			List<TIHabSiteState> list5 = list3.SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites.Where<TIHabSiteState>((TIHabSiteState y) => !y.hasPlannedOrOperatingBase)).ToList<TIHabSiteState>();
			IEnumerable<TIHabSiteState> enumerable = list5.Union<TIHabSiteState>(CS$<>8__locals1.currentFoundBaseGoals.Select<FactionGoal_FoundBase, TIHabSiteState>((FactionGoal_FoundBase x) => x.site));
			CS$<>8__locals1.incomeChecklist = AIEvaluators.GetSpaceResourceIncomesChecklist((FactionResource x) => AIEvaluators.EstimateFutureIncomePerMonth(CS$<>8__locals1.faction, x, false, false, false));
			CS$<>8__locals1.resourceScoredHabsites = enumerable.ToDictionary<TIHabSiteState, TIHabSiteState, float>((TIHabSiteState x) => x, (TIHabSiteState x) => AIEvaluators.EvaluateSpaceResourceIncomes_Strategic((FactionResource y) => x.GetMonthlyProduction(y), CS$<>8__locals1.incomeChecklist));
			Func<TIHabSiteState, int> func2 = (TIHabSiteState habSite) => 10 + Mathf.Min(1 + (int)(CS$<>8__locals1.resourceScoredHabsites[habSite] + 0.99f), 5) + (int)(CS$<>8__locals1.faction.aiValues.wantSpaceFacilities * 2f) + (CS$<>8__locals1.faction.CanFoundHabFromHabAtLocation(habSite, false, false) ? 1 : 0);
			foreach (FactionGoal_FoundBase factionGoal_FoundBase in CS$<>8__locals1.currentFoundBaseGoals)
			{
				factionGoal_FoundBase.SetImportance(func2(factionGoal_FoundBase.site));
			}
			int num5 = -1;
			if (CS$<>8__locals1.currentFoundBaseGoals.Any<FactionGoal_FoundBase>())
			{
				num5 = CS$<>8__locals1.currentFoundBaseGoals.Max<FactionGoal_FoundBase>((FactionGoal_FoundBase x) => x.importance);
			}
			List<TIHabSiteState> list6 = list5.Where<TIHabSiteState>((TIHabSiteState x) => CS$<>8__locals1.currentFoundBaseGoals.None<FactionGoal_FoundBase>((FactionGoal_FoundBase y) => y.site == x)).ToList<TIHabSiteState>();
			if (list6.Any<TIHabSiteState>() && !AIEvaluators.IsChecklistComplete(CS$<>8__locals1.incomeChecklist))
			{
				float highestScore = list6.Max<TIHabSiteState>((TIHabSiteState x) => CS$<>8__locals1.resourceScoredHabsites[x]);
				list6 = list6.Where<TIHabSiteState>((TIHabSiteState x) => CS$<>8__locals1.resourceScoredHabsites[x] == highestScore).ToList<TIHabSiteState>();
			}
			if (num3 > 0 && list6.Any<TIHabSiteState>())
			{
				TIHabSiteState tihabSiteState = list6.MaxBy<TIHabSiteState, float>((TIHabSiteState x) => AIEvaluators.EvaluateHabSite(CS$<>8__locals1.faction, x, true, true, true));
				int num6 = func2(tihabSiteState);
				bool flag2 = CS$<>8__locals1.currentFoundBaseGoals.Count < num2;
				if (flag2 || num6 > num5)
				{
					if (!flag2)
					{
						CS$<>8__locals1.currentFoundBaseGoals.MinBy<FactionGoal_FoundBase, int>((FactionGoal_FoundBase x) => x.importance).SetImportance(0);
					}
					CS$<>8__locals1.faction.AddGoal(new FactionGoal_FoundBase(CS$<>8__locals1.faction, num6, tihabSiteState, GoalType.BuildFullBase, null, GoalType.None, false, null), HandleDuplicateGoalRule.ResetImportance, null);
					num3--;
				}
			}
			if (num4 > 0)
			{
				Enumerable.Empty<TISpaceBodyState>();
				using (IEnumerator<TISpaceBodyState> enumerator2 = (from x in (from x in CS$<>8__locals1.faction.habs
						where x.ref_spaceBody != null
						select x.ref_spaceBody).Union<TISpaceBodyState>(from x in list
						where x.target() != null && x.target().ref_spaceBody != null
						select x.target().ref_spaceBody)
					select x.GetSunOrbitingRelatedObject.ref_spaceBody).Distinct<TISpaceBodyState>().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TISpaceBodyState spaceBody = enumerator2.Current;
						IEnumerable<TIHabState> stationsInOrbit = spaceBody.stationsInOrbit;
						Func<TIHabState, bool> func3;
						if ((func3 = CS$<>8__locals1.<>9__35) == null)
						{
							func3 = (CS$<>8__locals1.<>9__35 = (TIHabState x) => x.faction == CS$<>8__locals1.faction);
						}
						if (stationsInOrbit.None<TIHabState>(func3) && list.None<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_spaceBody == spaceBody))
						{
							IEnumerable<TIOrbitState> enumerable2 = spaceBody.orbits.Where<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null));
							if (enumerable2.Count<TIOrbitState>() != 0)
							{
								TIOrbitState tiorbitState = enumerable2.MaxBy<TIOrbitState, double>((TIOrbitState x) => x.semiMajorAxis_km);
								int num7 = (int)(15f - (2f - CS$<>8__locals1.faction.aiValues.wantSpaceFacilities) * 2f);
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_FoundMaxStation(CS$<>8__locals1.faction, num7, tiorbitState, GoalType.BuildFullStation, null, GoalType.None, false, null), HandleDuplicateGoalRule.ResetImportance, null);
								num4--;
							}
						}
					}
				}
			}
			List<TIHabState> earthSystemStations = CS$<>8__locals1.faction.EarthSystemStations;
			if (num4 > 0)
			{
				IEnumerable<TIFactionGoalState> enumerable3 = from x in CS$<>8__locals1.faction.GoalsOfType(new List<GoalType>
					{
						GoalType.BuildFullStation,
						GoalType.BuildRefuellingStation,
						GoalType.BuildSpecialtyStation,
						GoalType.FoundMaxStation,
						GoalType.FoundPlatform,
						GoalType.CaptureHab
					}, false, true)
					where x.target().ref_spaceBody == GameStateManager.Earth() || x.target().ref_spaceBody == GameStateManager.Luna()
					select x;
				int num8 = earthSystemStations.Count + enumerable3.Count<TIFactionGoalState>();
				if (CS$<>8__locals1.faction.needsPrimaryHab || num8 <= 5)
				{
					this.EstablishEarthSpacePresence(CS$<>8__locals1.faction, earthSystemStations);
				}
			}
		}

		// Token: 0x0400415C RID: 16732
		private GameTimeManager gameTime;

		// Token: 0x0400415D RID: 16733
		private Dictionary<TIOrbitState, float> NEOBaseScores;

		// Token: 0x0400415E RID: 16734
		private const int missionControlReserve = 0;
	}
}
