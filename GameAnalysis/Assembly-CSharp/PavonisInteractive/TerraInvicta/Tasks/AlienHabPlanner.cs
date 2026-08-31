using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000937 RID: 2359
	public class AlienHabPlanner : LegacyHabPlanner
	{
		// Token: 0x06005A65 RID: 23141 RVA: 0x002B0D58 File Offset: 0x002AEF58
		public static Func<FactionResource, float> GetEstimatedFutureIncomeFunctionForPurposeOfHabSiteSelection(TIFactionState faction)
		{
			Dictionary<FactionResource, float> estimatedFutureIncomes = TIResourcesCost.habResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, delegate(FactionResource resource)
			{
				if (TIResourcesCost.basicSpaceResources.Contains(resource))
				{
					return AIEvaluators.EstimateFutureIncomePerMonth(faction, resource, true, true, true);
				}
				return 0.001f;
			});
			return (FactionResource resource) => estimatedFutureIncomes[resource];
		}

		// Token: 0x06005A66 RID: 23142 RVA: 0x002B0DBC File Offset: 0x002AEFBC
		public override void ManageHabGoals(TIFactionState faction)
		{
			AlienHabPlanner.<>c__DisplayClass1_0 CS$<>8__locals1 = new AlienHabPlanner.<>c__DisplayClass1_0();
			CS$<>8__locals1.faction = faction;
			float alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact();
			foreach (KeyValuePair<TISpaceBodyState, List<TIFactionGoalState>> keyValuePair in (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.FoundBase, false, true)
				orderby x.importance descending
				group x by x.target().ref_system).ToDictionary<IGrouping<TISpaceBodyState, TIFactionGoalState>, TISpaceBodyState, List<TIFactionGoalState>>((IGrouping<TISpaceBodyState, TIFactionGoalState> x) => x.Key, (IGrouping<TISpaceBodyState, TIFactionGoalState> x) => x.ToList<TIFactionGoalState>()))
			{
				TIGameState key = keyValuePair.Key;
				TIHabState primaryHab = CS$<>8__locals1.faction.primaryHab;
				if (!(key == ((primaryHab != null) ? primaryHab.ref_system : null)))
				{
					List<TIFactionGoalState> value = keyValuePair.Value;
					for (int i = 0; i < value.Count; i++)
					{
						if (i > 0)
						{
							value[i].SetImportance(0);
						}
					}
				}
			}
			CS$<>8__locals1.foundBaseGoals = (from x in CS$<>8__locals1.faction.AllFoundHabGoals(true).Where<TIFactionGoalState>(delegate(TIFactionGoalState x)
				{
					TIGameState tigameState = x.target();
					return ((tigameState != null) ? tigameState.ref_habSite : null) != null;
				})
				where x is FactionGoal_FoundBase
				select x as FactionGoal_FoundBase).ToList<FactionGoal_FoundBase>();
			Func<FactionResource, float> estimatedFutureIncomeFunctionForPurposeOfHabSiteSelection = PavonisInteractive.TerraInvicta.Tasks.AlienHabPlanner.GetEstimatedFutureIncomeFunctionForPurposeOfHabSiteSelection(CS$<>8__locals1.faction);
			bool flag = alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact < TemplateManager.global.GetDifficultyBasedYearsToDelayAlienMiddleColonization();
			if (!flag)
			{
				AlienHabPlanner.<>c__DisplayClass1_1 CS$<>8__locals2 = new AlienHabPlanner.<>c__DisplayClass1_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.jupiter = GameStateManager.Jupiter();
				if (!CS$<>8__locals2.jupiter.habsInSystem.Where<TIHabState>((TIHabState x) => x.IsBase).Any<TIHabState>((TIHabState x) => x.faction == CS$<>8__locals2.CS$<>8__locals1.faction) && !CS$<>8__locals2.CS$<>8__locals1.foundBaseGoals.Any<FactionGoal_FoundBase>((FactionGoal_FoundBase x) => x.target().ref_system == CS$<>8__locals2.jupiter))
				{
					TIHabSiteState tihabSiteState = LegacyHabPlanner.SelectHabSiteForDevelopment(CS$<>8__locals2.CS$<>8__locals1.faction, CS$<>8__locals2.jupiter.habSitesInSystem, false, true, estimatedFutureIncomeFunctionForPurposeOfHabSiteSelection);
					if (tihabSiteState != null)
					{
						CS$<>8__locals2.CS$<>8__locals1.<ManageHabGoals>g__CreateFoundBaseGoal|8(tihabSiteState, 19);
					}
				}
				CS$<>8__locals2.beltSites = new HashSet<TIHabSiteState>(GameStateManager.FullAsteroidBelt(false).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem));
				int num = 0;
				while (CS$<>8__locals2.<ManageHabGoals>g__GetAsteroidBeltCoverage|24() < 3 && num++ < 3)
				{
					TIHabSiteState tihabSiteState2 = LegacyHabPlanner.SelectHabSiteForDevelopment(CS$<>8__locals2.CS$<>8__locals1.faction, CS$<>8__locals2.beltSites.Except<TIHabSiteState>(CS$<>8__locals2.CS$<>8__locals1.foundBaseGoals.Select<FactionGoal_FoundBase, TIHabSiteState>((FactionGoal_FoundBase x) => x.target().ref_habSite)), false, true, estimatedFutureIncomeFunctionForPurposeOfHabSiteSelection);
					if (tihabSiteState2 != null)
					{
						CS$<>8__locals2.CS$<>8__locals1.<ManageHabGoals>g__CreateFoundBaseGoal|8(tihabSiteState2, 18);
					}
				}
			}
			int num2 = CS$<>8__locals1.faction.fleets.Count<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Any<TISpaceShipState>((TISpaceShipState y) => y.role == ShipRole.InnerSystemColonyShip || y.role == ShipRole.OuterSystemColonyShip));
			int num3 = Mathf.Max(3, num2 - 2);
			int num4 = TemplateManager.global.GetMaxAlienBases(alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact).Round();
			if (flag)
			{
				num4 -= 4;
			}
			if (CS$<>8__locals1.<ManageHabGoals>g__GetFutureBaseCount|7() < num4 && CS$<>8__locals1.foundBaseGoals.Count < num3)
			{
				IEnumerable<TIHabSiteState> enumerable = CS$<>8__locals1.foundBaseGoals.Where<FactionGoal_FoundBase>(delegate(FactionGoal_FoundBase x)
				{
					TIGameState tigameState2;
					if (x == null)
					{
						tigameState2 = null;
					}
					else
					{
						TIGameState tigameState3 = x.target();
						tigameState2 = ((tigameState3 != null) ? tigameState3.ref_system : null);
					}
					return tigameState2 != null;
				}).SelectMany<FactionGoal_FoundBase, TIHabSiteState>((FactionGoal_FoundBase x) => x.target().ref_system.habSitesInSystem).ToList<TIHabSiteState>();
				List<TIHabSiteState> list = (from x in GameStateManager.GetAllGameStates<TIHabSiteState>(true)
					where x.maxTier < 3
					select x).ToList<TIHabSiteState>();
				IEnumerable<TIHabSiteState> enumerable2 = Enumerable.Empty<TIHabSiteState>();
				if (flag)
				{
					IEnumerable<TIHabSiteState> habSitesInSystem = GameStateManager.Jupiter().habSitesInSystem;
					List<TIHabSiteState> list2 = GameStateManager.FullAsteroidBelt(false).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem).ToList<TIHabSiteState>();
					enumerable2 = habSitesInSystem.Concat<TIHabSiteState>(list2);
				}
				IEnumerable<TIHabSiteState> enumerable3 = Enumerable.Empty<TIHabSiteState>();
				List<TISpaceBodyState> list3 = GameStateManager.KuiperBeltObjects(false);
				List<TISpaceBodyState> list4 = list3.Where<TISpaceBodyState>(delegate(TISpaceBodyState x)
				{
					IEnumerable<TIHabState> habsInSystem = x.habsInSystem;
					Func<TIHabState, bool> func;
					if ((func = CS$<>8__locals1.<>9__40) == null)
					{
						func = (CS$<>8__locals1.<>9__40 = (TIHabState x) => x.faction == CS$<>8__locals1.faction);
					}
					return habsInSystem.Any<TIHabState>(func);
				}).ToList<TISpaceBodyState>();
				IEnumerable<TISpaceBodyState> enumerable4 = list3.Except<TISpaceBodyState>(list4);
				int difficultyBasedAlienNonPlanetaryOuterSystemColonizationLimit = TemplateManager.global.GetDifficultyBasedAlienNonPlanetaryOuterSystemColonizationLimit();
				if (list4.Count >= difficultyBasedAlienNonPlanetaryOuterSystemColonizationLimit + 1)
				{
					enumerable3 = enumerable4.SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem).ToList<TIHabSiteState>();
				}
				else
				{
					TIHabState primaryHab2 = CS$<>8__locals1.faction.primaryHab;
					if (((primaryHab2 != null) ? primaryHab2.ref_system : null) != null)
					{
						AlienHabPlanner.<>c__DisplayClass1_3 CS$<>8__locals4 = new AlienHabPlanner.<>c__DisplayClass1_3();
						AlienHabPlanner.<>c__DisplayClass1_3 CS$<>8__locals5 = CS$<>8__locals4;
						TIHabState primaryHab3 = CS$<>8__locals1.faction.primaryHab;
						CS$<>8__locals5.primarySystem = ((primaryHab3 != null) ? primaryHab3.ref_system : null);
						Dictionary<TISpaceBodyState, double> dictionary = list3.Where<TISpaceBodyState>((TISpaceBodyState x) => x != CS$<>8__locals4.primarySystem).ToDictionary<TISpaceBodyState, TISpaceBodyState, double>((TISpaceBodyState x) => x, (TISpaceBodyState x) => TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(CS$<>8__locals4.primarySystem, x) / 149597870700.0);
						CS$<>8__locals4.neighborhood_AU = dictionary.OrderBy<KeyValuePair<TISpaceBodyState, double>, double>((KeyValuePair<TISpaceBodyState, double> x) => x.Value).Take<KeyValuePair<TISpaceBodyState, double>>(3).Average<KeyValuePair<TISpaceBodyState, double>>((KeyValuePair<TISpaceBodyState, double> x) => x.Value) * 1.5;
						List<TISpaceBodyState> list5 = (from x in dictionary
							where x.Value <= CS$<>8__locals4.neighborhood_AU
							select x.Key).ToList<TISpaceBodyState>();
						List<TISpaceBodyState> list6 = list5.Where<TISpaceBodyState>(new Func<TISpaceBodyState, bool>(CS$<>8__locals4.<ManageHabGoals>g__IsKBOPreferred|48)).ToList<TISpaceBodyState>();
						if (list6.Count == 0)
						{
							list6 = list5;
						}
						enumerable3 = enumerable4.Except<TISpaceBodyState>(list6).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem).ToList<TIHabSiteState>();
					}
				}
				double neptune_AU = GameStateManager.Neptune().semiMajorAxis_AU;
				List<TIHabSiteState> list7 = (from x in (from x in GameStateManager.AllSpaceBodies()
						select x.ref_system into x
						where x != null
						select x).Distinct<TISpaceBodyState>()
					where x.semiMajorAxis_AU > neptune_AU
					select x).Except<TISpaceBodyState>(list3).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem).ToList<TIHabSiteState>();
				List<TIHabSiteState> list8 = enumerable.Concat<TIHabSiteState>(list).Concat<TIHabSiteState>(enumerable2).Concat<TIHabSiteState>(enumerable3)
					.Concat<TIHabSiteState>(list7)
					.Distinct<TIHabSiteState>()
					.ToList<TIHabSiteState>();
				TIHabSiteState tihabSiteState3 = LegacyHabPlanner.SelectHabSiteForDevelopment(CS$<>8__locals1.faction, 2.5f, 50f, list8, false, false, false, null, 1, true, estimatedFutureIncomeFunctionForPurposeOfHabSiteSelection);
				if (tihabSiteState3 != null)
				{
					CS$<>8__locals1.<ManageHabGoals>g__CreateFoundBaseGoal|8(tihabSiteState3, 18);
				}
			}
			foreach (TISpaceBodyState tispaceBodyState in (from x in CS$<>8__locals1.faction.bases.Select<TIHabState, TISpaceBodyState>((TIHabState x) => x.ref_spaceBody).Distinct<TISpaceBodyState>().Where<TISpaceBodyState>(delegate(TISpaceBodyState x)
				{
					IEnumerable<TIHabState> stationsInOrbit = x.stationsInOrbit;
					Func<TIHabState, bool> func2;
					if ((func2 = CS$<>8__locals1.<>9__50) == null)
					{
						func2 = (CS$<>8__locals1.<>9__50 = (TIHabState y) => y.faction == CS$<>8__locals1.faction);
					}
					return stationsInOrbit.None<TIHabState>(func2);
				})
				where CS$<>8__locals1.faction.AllFoundHabGoals(true).None<TIFactionGoalState>(delegate(TIFactionGoalState y)
				{
					if (y is FactionGoal_FoundStation)
					{
						TIGameState tigameState4 = y.target();
						return ((tigameState4 != null) ? tigameState4.ref_spaceBody : null) == x;
					}
					return false;
				})
				where x.orbits.Any<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null))
				select x).ToList<TISpaceBodyState>())
			{
				TIOrbitState tiorbitState = tispaceBodyState.orbits.OrderBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_m).First<TIOrbitState>((TIOrbitState x) => x.NewStationAllowed(0, null));
				CS$<>8__locals1.faction.AddGoal(new FactionGoal_FoundPlatform(CS$<>8__locals1.faction, 18, tiorbitState, GoalType.BuildFullStation, null, GoalType.None), HandleDuplicateGoalRule.Ignore, null);
			}
			(from x in CS$<>8__locals1.faction.bases
				select x.mine into x
				where x.empty || x.destroyed
				select x).ToList<TIHabModuleState>();
			int num5 = TemplateManager.global.GetMaxAlienBases(alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact).Round();
			int num6 = ((float)num5 * 0.5f).RoundUp();
			int num7 = Mathf.Max(CS$<>8__locals1.<ManageHabGoals>g__GetFutureBaseCount|7() - num5, 0);
			bool flag2 = CS$<>8__locals1.faction.HasUpkeepInsecurityInTheFuture();
			bool flag3 = CS$<>8__locals1.<ManageHabGoals>g__GetFutureBaseCount|7() < num5;
			bool flag4 = flag2 && CS$<>8__locals1.<ManageHabGoals>g__GetFutureBaseCount|7() < num5 + num6;
			bool flag5 = !flag2 && CS$<>8__locals1.<ManageHabGoals>g__GetFutureBaseCount|7() >= num5 && num7 < Mathf.Max(1, ((float)num6 * 0.15f).Round());
			if ((flag3 || flag4 || flag5) && !flag)
			{
				AlienHabPlanner.<>c__DisplayClass1_5 CS$<>8__locals6 = new AlienHabPlanner.<>c__DisplayClass1_5();
				CS$<>8__locals6.CS$<>8__locals2 = CS$<>8__locals1;
				CS$<>8__locals6.insecureResources = CS$<>8__locals6.CS$<>8__locals2.faction.ResourcesExperiencingUpkeepInsecurityInTheFuture().Intersect<FactionResource>(TIResourcesCost.basicSpaceResources).ToList<FactionResource>();
				if (CS$<>8__locals6.insecureResources.Count == 0)
				{
					CS$<>8__locals6.insecureResources.Add(FactionResource.Water);
				}
				if ((from x in CS$<>8__locals6.CS$<>8__locals2.faction.AllFoundHabGoals(true)
					where x.target().isHabSiteState
					where base.<ManageHabGoals>g__IsHabSiteUsefulForFuel|55(x.target().ref_habSite)
					select x).ToList<TIFactionGoalState>().Count == 0)
				{
					List<TIHabSiteState> list9 = (from x in CS$<>8__locals6.CS$<>8__locals2.faction.habs
						group x by x.ref_system).SelectMany<IGrouping<TISpaceBodyState, TIHabState>, TIHabSiteState>((IGrouping<TISpaceBodyState, TIHabState> x) => x.Key.habSitesInSystem).Where<TIHabSiteState>(new Func<TIHabSiteState, bool>(CS$<>8__locals6.<ManageHabGoals>g__IsHabSiteUsefulForFuel|55)).ToList<TIHabSiteState>();
					if (list9.Count == 0)
					{
						list9 = (from x in GameStateManager.AllSpaceBodies()
							where x.semiMajorAxis_AU >= GameStateManager.Jupiter().semiMajorAxis_AU
							select x).SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSitesInSystem).Where<TIHabSiteState>(new Func<TIHabSiteState, bool>(CS$<>8__locals6.<ManageHabGoals>g__IsHabSiteUsefulForFuel|55)).ToList<TIHabSiteState>();
					}
					if (list9.Count > 0)
					{
						TIHabSiteState tihabSiteState4 = list9.MaxBy<TIHabSiteState, float>((TIHabSiteState x) => CS$<>8__locals6.insecureResources.Sum<FactionResource>((FactionResource y) => x.GetDailyProduction(y)));
						FactionGoal_FoundBase factionGoal_FoundBase = CS$<>8__locals6.CS$<>8__locals2.faction.AddGoal(new FactionGoal_FoundBase(CS$<>8__locals6.CS$<>8__locals2.faction, 18, tihabSiteState4, GoalType.BuildFullBase, null, GoalType.None, false, null), HandleDuplicateGoalRule.Ignore, null) as FactionGoal_FoundBase;
						if (factionGoal_FoundBase != null)
						{
							CS$<>8__locals6.CS$<>8__locals2.foundBaseGoals.Add(factionGoal_FoundBase);
						}
					}
				}
			}
			int num8 = 18;
			foreach (FactionGoal_FoundBase factionGoal_FoundBase2 in CS$<>8__locals1.foundBaseGoals)
			{
				if (factionGoal_FoundBase2.ShouldPauseGoal() && factionGoal_FoundBase2.importance > num8)
				{
					factionGoal_FoundBase2.SetImportance(num8);
				}
			}
			FactionGoal_FoundBase factionGoal_FoundBase3 = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.FoundBase, false, true)
				select x as FactionGoal_FoundBase into x
				where x.assignedFleet == null || !x.assignedFleet.inTransfer
				where !x.ShouldPauseGoal()
				orderby x.importance descending
				select x).FirstOrDefault<FactionGoal_FoundBase>();
			if (factionGoal_FoundBase3 != null)
			{
				factionGoal_FoundBase3.SetImportance(19);
			}
		}
	}
}
