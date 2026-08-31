using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.TIVirtualFleetState;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Tasks
{
	// Token: 0x02000935 RID: 2357
	public class AIDailyFactionPlanner : MonoBehaviour
	{
		// Token: 0x17000F4C RID: 3916
		// (get) Token: 0x06005A19 RID: 23065 RVA: 0x0029DDC3 File Offset: 0x0029BFC3
		// (set) Token: 0x06005A1A RID: 23066 RVA: 0x0029DDCA File Offset: 0x0029BFCA
		public static AIDailyFactionPlanner singleton { get; private set; }

		// Token: 0x06005A1B RID: 23067 RVA: 0x0029DDD2 File Offset: 0x0029BFD2
		private void Awake()
		{
			AIDailyFactionPlanner.singleton = this;
		}

		// Token: 0x06005A1C RID: 23068 RVA: 0x0029DDDC File Offset: 0x0029BFDC
		public void Initialize()
		{
			AIDailyFactionPlanner.singleton = this;
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.InitializeAILogging();
			this.SetGlobalStaticData();
			this.AIFactions = (from x in GameStateManager.AllFactions()
				where x.player.isAI
				select x).ToArray<TIFactionState>();
			this.numAIFactions = this.AIFactions.Length;
			AIDailyFactionPlanner.factionAIData = new Dictionary<TIFactionState, StaticFactionAIData>();
			int num = 1;
			foreach (TIFactionState tifactionState in this.AIFactions)
			{
				AIDailyFactionPlanner.factionAIData.Add(tifactionState, new StaticFactionAIData(tifactionState, num++));
			}
		}

		// Token: 0x06005A1D RID: 23069 RVA: 0x0029DE88 File Offset: 0x0029C088
		private void InitializeAILogging()
		{
			File.WriteAllText(TIFactionState.dumpfile, string.Concat(new string[]
			{
				"ActivePlayer: ",
				GameControl.control.activePlayer.displayNameCapitalized,
				" Version: ",
				Application.version,
				" Start version: ",
				TIGlobalValuesState.GlobalValues.campaignStartVersion,
				"\nDifficulty: ",
				TIGlobalValuesState.GlobalValues.difficulty.ToString(),
				"\nLoc: ",
				Utilities.PlayerCountryCode(),
				"\n"
			}));
		}

		// Token: 0x06005A1E RID: 23070 RVA: 0x0029DF24 File Offset: 0x0029C124
		private void LogHardwareSpecs()
		{
			this.LogDumpString(new StringBuilder("FirstCouncilor: ").Append(Utilities.PlayerCountryCode()).ToString());
			this.LogDumpString(new StringBuilder(DateTime.UtcNow.ToString()).Append(" UTC").ToString());
			this.LogDumpString(new StringBuilder(SystemInfo.operatingSystem).ToString());
			this.LogDumpString(new StringBuilder(SystemInfo.graphicsDeviceName).ToString());
			this.LogDumpString(new StringBuilder(SystemInfo.graphicsDeviceVersion).ToString());
			this.LogDumpString(new StringBuilder(SystemInfo.graphicsMemorySize.ToString()).Append(" MB VRAM").ToString());
			this.LogDumpString(new StringBuilder(SystemInfo.deviceName).ToString());
			this.LogDumpString(new StringBuilder(SystemInfo.systemMemorySize.ToString()).Append(" MB RAM").ToString());
			this.LogDumpString(new StringBuilder(Screen.currentResolution.ToString()).ToString());
			this.LogDumpString(new StringBuilder(Application.consoleLogPath).ToString());
		}

		// Token: 0x06005A1F RID: 23071 RVA: 0x0029E051 File Offset: 0x0029C251
		private void LogDumpString(string logString)
		{
			File.AppendAllText("AIDump.txt", logString + "\n");
		}

		// Token: 0x06005A20 RID: 23072 RVA: 0x0029E068 File Offset: 0x0029C268
		private void SetGlobalStaticData()
		{
			this.LEOs = GameStateManager.LEOStates();
		}

		// Token: 0x06005A21 RID: 23073 RVA: 0x0029E075 File Offset: 0x0029C275
		public static void InitializeAIForNewCampaign()
		{
			AIDailyFactionPlanner.SetInitialFactionNationTargets();
		}

		// Token: 0x06005A22 RID: 23074 RVA: 0x0029E07C File Offset: 0x0029C27C
		public void IdleAIPlanning()
		{
			this.PerformAITaskGroup(this.AIFactions.SelectRandomItem<TIFactionState>(), this.gameTime.currentTime.hour < 12);
		}

		// Token: 0x06005A23 RID: 23075 RVA: 0x0029E0A4 File Offset: 0x0029C2A4
		private void PerformAITaskGroup(TIFactionState faction, bool early)
		{
			List<AITaskCategory> list = (early ? faction.factionEarlyToDoList : faction.factionLateToDoList);
			if (list.Count > 0)
			{
				AITaskCategory aitaskCategory = list.SelectRandomItem<AITaskCategory>();
				list.Remove(aitaskCategory);
				this.PerformAITaskGroup(faction, aitaskCategory);
			}
		}

		// Token: 0x06005A24 RID: 23076 RVA: 0x0029E0E4 File Offset: 0x0029C2E4
		private void PerformAITaskGroup(TIFactionState faction, AITaskCategory task)
		{
			if (faction.defeated)
			{
				return;
			}
			switch (task)
			{
			case AITaskCategory.ManageCouncilors:
				AIDailyFactionPlanner.ManageCouncilors(faction, this.gameTime);
				return;
			case AITaskCategory.SetResearchPriorities:
				this.SetResearchPriorities(faction);
				return;
			case AITaskCategory.ManageNations:
				this.ManageNations(faction);
				return;
			case AITaskCategory.BuildSpaceAssets:
				this.BuildSpaceAssets(faction);
				return;
			case AITaskCategory.ArmyOperations:
				this.ArmyOperations(faction);
				return;
			case AITaskCategory.FleetOperations:
				CoroutineDummy.Singleton.StartCoroutine(this.PeriodicFleetOperations(faction));
				return;
			default:
				return;
			}
		}

		// Token: 0x06005A25 RID: 23077 RVA: 0x0029E15C File Offset: 0x0029C35C
		public void FactionOperations0000()
		{
			this.AIFactions = this.AIFactions.OrderBy<TIFactionState, int>((TIFactionState a) => TIUtilities.RandomRange(0, int.MaxValue)).ToArray<TIFactionState>();
			float num = TITimeState.CampaignDuration_years_Exact();
			for (int i = 0; i < this.numAIFactions; i++)
			{
				TIFactionState tifactionState = this.AIFactions[i];
				tifactionState.factionEarlyToDoList = new List<AITaskCategory>
				{
					AITaskCategory.ManageCouncilors,
					AITaskCategory.SetResearchPriorities,
					AITaskCategory.ManageNations
				};
				tifactionState.factionLateToDoList = new List<AITaskCategory>
				{
					AITaskCategory.BuildSpaceAssets,
					AITaskCategory.ArmyOperations,
					AITaskCategory.FleetOperations
				};
			}
			Dictionary<FactionResource, Dictionary<TIFactionState, float>> dictionary = new Dictionary<FactionResource, Dictionary<TIFactionState, float>>(Enums.FactionResources.ToDictionary<FactionResource, FactionResource, Dictionary<TIFactionState, float>>((FactionResource x) => x, (FactionResource x) => new Dictionary<TIFactionState, float>(GameStateManager.AllHumanFactions().ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState y) => y, (TIFactionState y) => y.GetDailyIncome(x, false, false)))));
			foreach (TIFactionState tifactionState2 in GameStateManager.AllFactions())
			{
				if (!tifactionState2.defeated)
				{
					if (tifactionState2.IsActiveHumanFaction)
					{
						this.AnalyzeDeficiencies_Human(tifactionState2, dictionary, num, true);
					}
					else
					{
						this.AnalyzeDeficiences_Alien(tifactionState2, dictionary, num);
					}
				}
			}
			for (int k = 0; k < this.numAIFactions; k++)
			{
				TIFactionState tifactionState3 = this.AIFactions[k];
				if (!tifactionState3.defeated)
				{
					this.ReviewAndSetGoals(tifactionState3);
					if (tifactionState3.AISavingTarget.active)
					{
						tifactionState3.AISavingTarget.DailySavingUpdate();
					}
				}
			}
		}

		// Token: 0x06005A26 RID: 23078 RVA: 0x0029E2DD File Offset: 0x0029C4DD
		public void FactionOperations(bool early)
		{
			CoroutineDummy.Singleton.StartCoroutine(this.FactionOperationsLoop(early));
		}

		// Token: 0x06005A27 RID: 23079 RVA: 0x0029E2F1 File Offset: 0x0029C4F1
		public IEnumerator FactionOperationsLoop(bool early)
		{
			int num;
			for (int i = 0; i < this.numAIFactions; i = num + 1)
			{
				this.PerformAITaskGroup(this.AIFactions[i], early);
				yield return null;
				num = i;
			}
			yield break;
		}

		// Token: 0x06005A28 RID: 23080 RVA: 0x0029E308 File Offset: 0x0029C508
		public void FactionOperations2300()
		{
			for (int i = 0; i < this.numAIFactions; i++)
			{
				TIFactionState tifactionState = this.AIFactions[i];
				foreach (AITaskCategory aitaskCategory in tifactionState.factionEarlyToDoList.Union<AITaskCategory>(tifactionState.factionLateToDoList).ToList<AITaskCategory>())
				{
					this.PerformAITaskGroup(tifactionState, aitaskCategory);
				}
			}
		}

		// Token: 0x06005A29 RID: 23081 RVA: 0x0029E388 File Offset: 0x0029C588
		private void AnalyzeDeficiencies_Human(TIFactionState faction, Dictionary<FactionResource, Dictionary<TIFactionState, float>> factionIncomes, float campaignDuration_years, bool useAltMethod = true)
		{
			Dictionary<TIFactionState, float> dictionary = new Dictionary<TIFactionState, float>();
			if (useAltMethod)
			{
				dictionary = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, (TIFactionState x) => x.GetFactionStrengthEstimate());
				faction.mostPowerfulHumanEnemy = faction.GetMostThreateningEnemyHumanFaction();
			}
			else
			{
				dictionary = GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, float>((TIFactionState x) => x, (TIFactionState x) => AIDailyFactionPlanner.<AnalyzeDeficiencies_Human>g__FactionThreatScore_Old|27_0(faction, x));
				faction.mostPowerfulHumanEnemy = dictionary.Where<KeyValuePair<TIFactionState, float>>((KeyValuePair<TIFactionState, float> x) => x.Key.IsActiveHumanFaction && !x.Key.permanentAlly(faction)).MaxBy<KeyValuePair<TIFactionState, float>, float>((KeyValuePair<TIFactionState, float> x) => x.Value).Key;
			}
			Func<TIHabState, bool> <>9__15;
			if (GameStateManager.Earth().interfaceOrbits.All<TIOrbitState>(delegate(TIOrbitState x)
			{
				if (!x.NewStationAllowed(0, null))
				{
					IEnumerable<TIHabState> stationsInOrbit = x.stationsInOrbit;
					Func<TIHabState, bool> func;
					if ((func = <>9__15) == null)
					{
						func = (<>9__15 = (TIHabState x) => x.faction == faction);
					}
					return stationsInOrbit.None<TIHabState>(func);
				}
				return false;
			}))
			{
				faction.selfAssessement = FactionSelfAssessment.LosingBig;
			}
			else if (dictionary[faction] > 2f * dictionary[faction.mostPowerfulHumanEnemy])
			{
				faction.selfAssessement = FactionSelfAssessment.WayAhead;
			}
			else if ((double)dictionary[faction] > 1.25 * (double)dictionary[faction.mostPowerfulHumanEnemy])
			{
				faction.selfAssessement = FactionSelfAssessment.Ahead;
			}
			else if ((double)dictionary[faction] < 0.8 * (double)dictionary[faction.mostPowerfulHumanEnemy])
			{
				faction.selfAssessement = FactionSelfAssessment.Losing;
			}
			else if ((double)dictionary[faction] < 0.5 * (double)dictionary[faction.mostPowerfulHumanEnemy])
			{
				faction.selfAssessement = FactionSelfAssessment.LosingBig;
			}
			else
			{
				faction.selfAssessement = FactionSelfAssessment.Even;
			}
			IEnumerable<TIObjectiveTemplate> enumerable = faction.GetObjectivesByTypeAndStatus(ObjectiveType.Campaign, ObjectiveStatus.Unlocked).ToList<TIObjectiveTemplate>().Where<TIObjectiveTemplate>(delegate(TIObjectiveTemplate x)
			{
				TIMissionTemplate targetMissionTemplate = x.targetMissionTemplate;
				if (targetMissionTemplate == null)
				{
					return false;
				}
				FactionResource primaryResource = targetMissionTemplate.primaryResource;
				return true;
			});
			IEnumerable<FactionResource> currentObjectiveResources = enumerable.Select<TIObjectiveTemplate, FactionResource>((TIObjectiveTemplate x) => x.targetMissionTemplate.primaryResource);
			faction.resourceIncomeDeficiencies = factionIncomes.Keys.Where<FactionResource>((FactionResource x) => AIEvaluators.Deficient(faction, x, factionIncomes[x][faction], faction.GetCurrentResourceAmount(x), factionIncomes[x].Values.Average(), campaignDuration_years, factionIncomes) || currentObjectiveResources.Contains(x)).ToList<FactionResource>();
			faction.minorCPTrouble = faction.MinorCPTrouble();
			faction.majorCPTrouble = faction.MajorCPTrouble();
			faction.currentRiskAversion = faction.AI_ModifiedRiskAversion();
		}

		// Token: 0x06005A2A RID: 23082 RVA: 0x0029E688 File Offset: 0x0029C888
		public void AnalyzeDeficiences_Alien(TIFactionState faction, Dictionary<FactionResource, Dictionary<TIFactionState, float>> factionIncomes, float campaignDuration_years)
		{
			faction.resourceIncomeDeficiencies = TIResourcesCost.basicSpaceResources.Where<FactionResource>((FactionResource x) => AIEvaluators.Deficient(faction, x, faction.GetDailyIncome(x, false, false), faction.GetCurrentResourceAmount(x), factionIncomes[x].Values.Max(), campaignDuration_years, factionIncomes)).ToList<FactionResource>();
			faction.minorCPTrouble = GameStateManager.AlienProxy().MinorCPTrouble();
			faction.majorCPTrouble = GameStateManager.AlienProxy().MajorCPTrouble();
			faction.alienProxyNeedsHelp = GameStateManager.AlienProxy().totalControlNations.None<TINationState>((TINationState x) => x.MajorGlobalPower);
		}

		// Token: 0x06005A2B RID: 23083 RVA: 0x0029E73C File Offset: 0x0029C93C
		private void KillAnyAlienShipForObjective(TIFactionState faction, TIObjectiveTemplate objective, int maxRelatedAttackGoals)
		{
			AIDailyFactionPlanner.<>c__DisplayClass29_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass29_0();
			CS$<>8__locals1.objective = objective;
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.maxRelatedAttackGoals = maxRelatedAttackGoals;
			CS$<>8__locals1.relatedAttackGoals = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.AttackWithFleet, false, true)
				select x as FactionGoal_AttackWithFleet into x
				where x.objective == CS$<>8__locals1.objective
				select x).ToList<FactionGoal_AttackWithFleet>();
			if (TITimeState.Now().day % 14 == 0)
			{
				using (List<FactionGoal_AttackWithFleet>.Enumerator enumerator = CS$<>8__locals1.relatedAttackGoals.ToList<FactionGoal_AttackWithFleet>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FactionGoal_AttackWithFleet factionGoal_AttackWithFleet = enumerator.Current;
						if (!factionGoal_AttackWithFleet.InProgress())
						{
							factionGoal_AttackWithFleet.SetImportance(0);
							CS$<>8__locals1.relatedAttackGoals.Remove(factionGoal_AttackWithFleet);
						}
					}
					goto IL_00E2;
				}
			}
			if (CS$<>8__locals1.relatedAttackGoals.Count >= CS$<>8__locals1.maxRelatedAttackGoals)
			{
				return;
			}
			IL_00E2:
			IEnumerable<ValueTuple<TISpaceFleetState, TISpaceFleetState>> enumerable = AIEvaluators.GenerateQuickAttacks(CS$<>8__locals1.faction, GameStateManager.AlienFaction().fleets, CS$<>8__locals1.maxRelatedAttackGoals - CS$<>8__locals1.relatedAttackGoals.Count, null);
			if (CS$<>8__locals1.<KillAnyAlienShipForObjective>g__TryFillAttacks|3(enumerable))
			{
				return;
			}
			IEnumerable<TISpaceFleetState> enumerable2 = from x in GameStateManager.AlienFaction().fleets
				where !x.inTransfer
				where x.ref_system != null
				select x;
			IEnumerable<TISpaceFleetState> enumerable3 = enumerable2.Where<TISpaceFleetState>((TISpaceFleetState x) => x.AI_NeedsRepairBadly());
			if (enumerable3.Any<TISpaceFleetState>())
			{
				enumerable2 = enumerable3;
			}
			using (List<float>.Enumerator enumerator2 = new List<float> { 1f, 2f, 5f }.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					float size = enumerator2.Current;
					IEnumerable<TISpaceFleetState> enumerable4 = enumerable2.Where<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue() <= size * 100f);
					if (enumerable4.Count<TISpaceFleetState>() >= CS$<>8__locals1.maxRelatedAttackGoals)
					{
						enumerable2 = enumerable4;
						break;
					}
				}
			}
			enumerable2 = enumerable2.OrderBy<TISpaceFleetState, float>(delegate(TISpaceFleetState x)
			{
				float num = Mathf.Abs((float)x.ref_system.semiMajorAxis_AU - 1f);
				return Mathf.Max(0.001f, Mathf.Pow(num, 2f)) * x.SpaceCombatValue();
			}).ToList<TISpaceFleetState>();
			foreach (TISpaceFleetState tispaceFleetState in enumerable2)
			{
				if (CS$<>8__locals1.<KillAnyAlienShipForObjective>g__TryFillAttacks_Single|2(tispaceFleetState, null))
				{
					break;
				}
			}
		}

		// Token: 0x06005A2C RID: 23084 RVA: 0x0029E9F4 File Offset: 0x0029CBF4
		private void ManageObjectiveGoals(TIFactionState faction)
		{
			using (List<TIObjectiveTemplate>.Enumerator enumerator = faction.GetObjectivesByStatus(ObjectiveStatus.Unlocked).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIObjectiveTemplate objective = enumerator.Current;
					ObjectiveType objectiveType = objective.objectiveType;
					if (objectiveType != ObjectiveType.Campaign)
					{
						if (objectiveType == ObjectiveType.Victory && faction.GoalsOfType(GoalType.PursueVictory, false, true).Count == 0)
						{
							faction.AddGoal(new FactionGoal_Victory(faction, faction.victoryTemplate, objective), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						}
					}
					else
					{
						if (objective.targetMilestone == CampaignMilestone.AccessAlienTech || objective.targetMilestone == CampaignMilestone.AccessAlienShip)
						{
							this.KillAnyAlienShipForObjective(faction, objective, 2);
						}
						if (!string.IsNullOrEmpty(objective.targetHabModuleName))
						{
							TIHabModuleTemplate targetHabModuleTemplate = objective.targetHabModuleTemplate;
							List<TIHabState> list = faction.habs.Where<TIHabState>((TIHabState x) => AIEvaluators.DoesHabMatchObjectiveHabModuleRequirements(x, true)).ToList<TIHabState>();
							List<FactionGoal_BuildHab> list2 = (from x in faction.GoalsOfType(TIFactionGoalState.BuildHabGoals, false, true)
								select x as FactionGoal_BuildHab into x
								where x.objective == objective
								select x).ToList<FactionGoal_BuildHab>();
							List<FactionGoal_FoundHab> list3 = (from x in faction.GoalsOfType(TIFactionGoalState.FoundHabGoals, false, true)
								select x as FactionGoal_FoundHab into x
								where x.objective == objective
								select x).ToList<FactionGoal_FoundHab>();
							Func<TIHabModuleState, bool> <>9__7;
							int num = objective.targetCount - list.Sum<TIHabState>(delegate(TIHabState x)
							{
								IEnumerable<TIHabModuleState> enumerable2 = x.OkayModules();
								Func<TIHabModuleState, bool> func;
								if ((func = <>9__7) == null)
								{
									func = (<>9__7 = (TIHabModuleState y) => y.templateName == objective.targetHabModuleName);
								}
								return enumerable2.Count<TIHabModuleState>(func);
							});
							int num2 = 5;
							if (targetHabModuleTemplate.onePerHab)
							{
								num2 = 1;
							}
							int num3 = ((float)objective.targetCount / (float)num2).RoundUp() + 2;
							Func<TIHabModuleState, bool> <>9__8;
							if (num > 0 && list2.Count + list3.Count < num3 && list3.Count == 0 && list2.All<FactionGoal_BuildHab>(delegate(FactionGoal_BuildHab x)
							{
								if (x.hab.tier >= targetHabModuleTemplate.tier)
								{
									IEnumerable<TIHabModuleState> enumerable3 = x.hab.OkayModules();
									Func<TIHabModuleState, bool> func2;
									if ((func2 = <>9__8) == null)
									{
										func2 = (<>9__8 = (TIHabModuleState x) => x.templateName == targetHabModuleTemplate.dataName);
									}
									if (enumerable3.Any<TIHabModuleState>(func2))
									{
										return x.hab.AvailableSlots().Count <= 1;
									}
								}
								return false;
							}))
							{
								HabType habType = objective.targetHabModuleTemplate.habType;
								if (habType == HabType.Any)
								{
									TIGameState targetHabLocationState = objective.targetHabLocationState;
									if (targetHabLocationState != null && targetHabLocationState.isOrbitState)
									{
										habType = HabType.Station;
									}
									if (objective.targetHabLocationState.isHabSiteState || objective.targetHabLocationState.isSpaceBodyState)
									{
										habType = HabType.Base;
									}
								}
								int num4 = 19;
								List<TIHabModuleTemplate> list4 = Enumerable.Repeat<TIHabModuleTemplate>(objective.targetHabModuleTemplate, (objective.targetCount > 1) ? num2 : 1).ToList<TIHabModuleTemplate>();
								IEnumerable<TIHabState> enumerable = list.Except<TIHabState>(list2.Select<FactionGoal_BuildHab, TIHabState>((FactionGoal_BuildHab x) => x.hab));
								if (enumerable.Any<TIHabState>())
								{
									TIHabState tihabState = enumerable.First<TIHabState>();
									habType = tihabState.habType;
									if (habType == HabType.Base)
									{
										faction.AddGoal(new FactionGoal_BuildSpecialtyBase(faction, num4, tihabState, list4, false, objective), HandleDuplicateGoalRule.ResetImportance, null);
									}
									else
									{
										faction.AddGoal(new FactionGoal_BuildSpecialtyStation(faction, num4, tihabState, list4, false, objective), HandleDuplicateGoalRule.ResetImportance, null);
									}
								}
								else if (habType == HabType.Base)
								{
									faction.AddGoal(new FactionGoal_FoundBase(faction, num4, objective.targetHabLocationState.ref_habSite, GoalType.BuildSpecialtyBase, list4, GoalType.None, false, objective), HandleDuplicateGoalRule.ResetImportance, null);
								}
								else
								{
									faction.AddGoal(new FactionGoal_FoundMaxStation(faction, num4, objective.targetHabLocationState.ref_orbit, GoalType.BuildSpecialtyStation, list4, GoalType.None, false, objective), HandleDuplicateGoalRule.ResetImportance, null);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005A2D RID: 23085 RVA: 0x0029EDC4 File Offset: 0x0029CFC4
		public static float GetMonthlyAlienHateGain(TIFactionState humanFaction, bool isMostThreateningHumanEnemy, bool isStrongestHumanFaction, float miscModifier = 1f, float campaignDuration_years = -1f, int difficulty = -1)
		{
			if (humanFaction.veryProAlien)
			{
				return 0f;
			}
			if (campaignDuration_years < 0f)
			{
				campaignDuration_years = TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact();
			}
			float num = TemplateManager.global.GetAlienSteadyHateGainModifier(difficulty) * campaignDuration_years / 60f;
			float num2 = 0f;
			if (isMostThreateningHumanEnemy)
			{
				num2 += 1.3f / (float)(isStrongestHumanFaction ? 1 : 2);
			}
			if (humanFaction.unlockedVictoryObjective)
			{
				num2 += 0.5f;
			}
			if (humanFaction.antiAlien)
			{
				num2 += 0.07f;
				if (humanFaction.veryAntiAlien)
				{
					num2 += 0.1f;
				}
				if (humanFaction.extremist)
				{
					num2 += 0.17f;
				}
			}
			if (humanFaction.proAlien)
			{
				num2 *= 0.75f;
			}
			float num3 = (AIEvaluators.ShouldAliensGoLoud() ? 1f : 0.1f);
			return miscModifier * num2 * num * num3;
		}

		// Token: 0x06005A2E RID: 23086 RVA: 0x0029EE8C File Offset: 0x0029D08C
		public static float GetExpectedYearsUntilWarWithAliens(TIFactionTemplate humanFaction, int difficulty, float extraHateLossPerMonth = 0f, float extraHate = 0f)
		{
			TIFactionState tifactionState = new TIFactionState
			{
				ideology = TemplateManager.Find<TIFactionIdeologyTemplate>(humanFaction.ideologyName, false)
			};
			float num = 0.4f;
			if (tifactionState.antiAlien)
			{
				num *= 0.8f;
			}
			num += extraHateLossPerMonth;
			float num2 = 0f;
			int i = 0;
			bool flag = false;
			while (i < 1000)
			{
				num2 += AIDailyFactionPlanner.GetMonthlyAlienHateGain(tifactionState, true, true, (float)i / 12f, (float)difficulty, -1);
				num2 -= num;
				num2 = Mathf.Max(0f, num2);
				if (num2 > 0f && flag)
				{
					Log.Debug(humanFaction.dataName + " breakeven at " + ((float)i / 12f).ToString() + " years.", Array.Empty<object>());
					flag = false;
				}
				if (num2 >= 50f - extraHate)
				{
					return (float)(i + 1) / 12f;
				}
				i++;
			}
			return float.PositiveInfinity;
		}

		// Token: 0x06005A2F RID: 23087 RVA: 0x0029EF68 File Offset: 0x0029D168
		public static float JealousyAndDeescalation(TIFactionState faction, TIFactionState enemyFaction, bool generalDeescalation, bool processPeriodicChange)
		{
			if (faction.permanentAlly(enemyFaction))
			{
				return 0f;
			}
			TIFactionState mostThreateningEnemyHumanFaction = faction.GetMostThreateningEnemyHumanFaction();
			TIFactionState strongestHumanFaction = AIEvaluators.GetStrongestHumanFaction(null);
			TIFactionState mostThreateningEnemyHumanFaction2 = enemyFaction.GetMostThreateningEnemyHumanFaction();
			bool flag = enemyFaction.player.isAI || TINationState.GetIdeologicalDistance(faction.ideology, enemyFaction.ideology) >= TemplateManager.global.AI_GangUpOnLeaderBehavior_MinIdeologicalDistance_Difficulty();
			float num = 0f;
			if ((mostThreateningEnemyHumanFaction != enemyFaction || generalDeescalation) && (!enemyFaction.IsAlienFaction || !faction.veryAntiAlien))
			{
				if (processPeriodicChange)
				{
					float num2 = -1f;
					if (faction.IsAlienFaction)
					{
						num2 *= 0.8f;
						if (mostThreateningEnemyHumanFaction != enemyFaction && faction.enemyWarFactions.Count > 2)
						{
							num2 *= 1.5f;
						}
						if (enemyFaction.antiAlien)
						{
							num2 *= 0.8f;
						}
					}
					faction.GainFactionHate(enemyFaction, num2, true, "Periodic hate", true);
				}
				if (mostThreateningEnemyHumanFaction != null && mostThreateningEnemyHumanFaction == mostThreateningEnemyHumanFaction2)
				{
					num -= (faction.IsAlienFaction ? 0.33f : 1f);
					if ((faction.proAlien && mostThreateningEnemyHumanFaction.antiAlien) || (faction.antiAlien && mostThreateningEnemyHumanFaction.proAlien))
					{
						num -= 1f;
					}
				}
			}
			if (faction.IsAlienFaction && enemyFaction.isAlienAppeaser)
			{
				num -= 1f;
				if (enemyFaction.CanContactAlien)
				{
					num -= 3f;
				}
				if (enemyFaction.unlockedVictoryObjective)
				{
					num -= 6f;
				}
			}
			else if (faction.isAlienAppeaser && enemyFaction.IsAlienFaction)
			{
				num -= 10f;
				if (faction.unlockedVictoryObjective)
				{
					num -= 9999f;
				}
			}
			if (faction.IsAlienFaction)
			{
				num += AIDailyFactionPlanner.GetMonthlyAlienHateGain(enemyFaction, enemyFaction == mostThreateningEnemyHumanFaction, enemyFaction == strongestHumanFaction, 1f, -1f, -1);
			}
			else if (flag)
			{
				if (mostThreateningEnemyHumanFaction == enemyFaction)
				{
					switch (faction.selfAssessement)
					{
					case FactionSelfAssessment.LosingBig:
						num += 2.25f;
						break;
					case FactionSelfAssessment.Losing:
						num += 1.75f;
						break;
					case FactionSelfAssessment.Even:
						num += 1f;
						break;
					}
					if ((faction.veryProAlien && enemyFaction.veryAntiAlien) || (faction.veryAntiAlien && enemyFaction.veryProAlien))
					{
						num += 1.4f;
					}
					if ((faction.proAlien && enemyFaction.proAlien) || (faction.antiAlien && enemyFaction.antiAlien))
					{
						num -= ((TITimeState.CampaignDuration_years_Exact() < 7f) ? 3f : 0.75f);
					}
					num = Mathf.Max(0f, num);
				}
				if (!faction.permanentAlly(enemyFaction) && enemyFaction.unlockedVictoryObjective)
				{
					num += 1f;
				}
			}
			return num;
		}

		// Token: 0x06005A30 RID: 23088 RVA: 0x0029F22C File Offset: 0x0029D42C
		private void ManageWarsWithFaction(TIFactionState faction)
		{
			foreach (TIFactionState tifactionState in AIDailyFactionPlanner.factionAIData[faction].enemyFactions)
			{
				if (this.gameTime.currentTime.day == 1 && (!tifactionState.IsAlienFaction || !faction.antiAlien))
				{
					float num = AIDailyFactionPlanner.JealousyAndDeescalation(faction, tifactionState, this.gameTime.currentTime.month % 2 == 0, true);
					faction.GainFactionHate(tifactionState, num, true, "Jealousy and Deescelation", true);
				}
				float factionHate = faction.GetFactionHate(tifactionState);
				FactionGoal_WarOnFaction factionGoal_WarOnFaction = faction.FindGoals(GoalType.WarOnFaction, faction, tifactionState, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() as FactionGoal_WarOnFaction;
				int num2 = (15f + 3f * Mathf.Clamp01(factionHate / 200f)).Round() + ((factionGoal_WarOnFaction != null && factionGoal_WarOnFaction.IsTotalWar) ? 1 : 0);
				bool flag = AIEvaluators.FactionsGoToWar(faction, tifactionState);
				if (factionGoal_WarOnFaction != null && flag)
				{
					factionGoal_WarOnFaction.SetImportance(num2);
				}
				else if (factionGoal_WarOnFaction == null && flag)
				{
					FactionGoal_WarOnFaction factionGoal_WarOnFaction2 = faction.AddGoal(new FactionGoal_WarOnFaction(faction, num2, tifactionState, null), HandleDuplicateGoalRule.ResetImportance, null) as FactionGoal_WarOnFaction;
					if (factionGoal_WarOnFaction2 != null)
					{
						if (!faction.IsAlienFaction)
						{
							TINotificationQueueState.AddCouncilorMessage(faction, CouncilorChatType.WarDeclared, tifactionState);
						}
						else if (!factionGoal_WarOnFaction2.IsTotalWar)
						{
							TINotificationQueueState.AddCouncilorMessage(tifactionState, CouncilorChatType.AlienRetaliationDeclared, tifactionState);
						}
						else
						{
							TINotificationQueueState.AddCouncilorMessage(GameStateManager.AlienProxy(), CouncilorChatType.AlienFullWarDeclared, tifactionState);
						}
						if (factionHate < AIEvaluators.FactionGotoWarRequiredHate(faction, tifactionState))
						{
							faction.SetFactionHate(tifactionState, AIEvaluators.FactionGotoWarRequiredHate(faction, tifactionState), true, "New war");
						}
						tifactionState.FixAssessedAlienHateToActualValue();
					}
				}
			}
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x0029F3DC File Offset: 0x0029D5DC
		public static void SetInitialFactionNationTargets()
		{
			Dictionary<SupraRegion, int> dictionary = new Dictionary<SupraRegion, int>(AIDailyFactionPlanner.maxTargets);
			using (List<SupraRegion>.Enumerator enumerator = dictionary.Keys.ToList<SupraRegion>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SupraRegion supra = enumerator.Current;
					if (!GameStateManager.AllExtantHumanNations().ToList<TINationState>().Any<TINationState>((TINationState x) => x.capital.mapRegionTemplate.supraRegion == supra))
					{
						dictionary[supra] = 0;
					}
				}
			}
			using (List<TIFactionState>.Enumerator enumerator2 = GameStateManager.AllHumanFactions().ToList<TIFactionState>().Shuffle<TIFactionState>()
				.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIFactionState faction = enumerator2.Current;
					if (faction.player.isAI)
					{
						List<TINationState> list = GameStateManager.AllExtantHumanNations().ToList<TINationState>();
						List<TINationState> list2 = new List<TINationState>();
						SupraRegion sr1;
						if (dictionary.Values.Any<int>((int x) => x > 0))
						{
							sr1 = dictionary.SelectRandomWeightedItem<KeyValuePair<SupraRegion, int>>((KeyValuePair<SupraRegion, int> x) => (float)x.Value, -1f, 1E-37f).Key;
							Dictionary<SupraRegion, int> dictionary2 = dictionary;
							SupraRegion supraRegion = sr1;
							dictionary2[supraRegion]--;
						}
						else
						{
							sr1 = AIDailyFactionPlanner.maxTargets.SelectRandomWeightedItem<KeyValuePair<SupraRegion, int>>((KeyValuePair<SupraRegion, int> x) => (float)x.Value, -1f, 1E-37f).Key;
						}
						IEnumerable<TINationState> enumerable = list.Where<TINationState>((TINationState x) => x.capital.mapRegionTemplate.supraRegion == sr1);
						int maxCPsinSupraRegion = enumerable.Max<TINationState>((TINationState x) => x.numControlPoints_unclamped);
						TINationState sr1CoreNation = enumerable.Where<TINationState>((TINationState x) => x.numControlPoints_unclamped == maxCPsinSupraRegion).MaxBy<TINationState, float>((TINationState x) => x.GetPublicOpinionOfFaction(faction));
						TIFactionState.LogAI(string.Concat(new string[]
						{
							faction.displayName,
							" SupraRegion: ",
							sr1.ToString(),
							" Primary: ",
							sr1CoreNation.displayName
						}), false);
						list2.Add(sr1CoreNation);
						list.Remove(sr1CoreNation);
						List<TINationState> adjacents = (from x in sr1CoreNation.AdjacentNations(false)
							where !sr1CoreNation.rivals.Contains(sr1CoreNation)
							select x).ToList<TINationState>();
						if (sr1CoreNation.allies.Count > 0)
						{
							adjacents.Add(sr1CoreNation.allies.MaxBy<TINationState, float>((TINationState x) => x.GetPublicOpinionOfFaction(faction)));
						}
						list2.AddRange(adjacents);
						list.RemoveAll((TINationState x) => adjacents.Contains(x));
						SupraRegion sr2;
						if (dictionary.Values.Any<int>((int x) => x > 0))
						{
							sr2 = dictionary.Where<KeyValuePair<SupraRegion, int>>((KeyValuePair<SupraRegion, int> x) => x.Key != sr1).SelectRandomWeightedItem<KeyValuePair<SupraRegion, int>>((KeyValuePair<SupraRegion, int> x) => (float)x.Value, -1f, 1E-37f).Key;
							Dictionary<SupraRegion, int> dictionary2 = dictionary;
							SupraRegion supraRegion = sr2;
							dictionary2[supraRegion]--;
						}
						else
						{
							sr2 = AIDailyFactionPlanner.maxTargets.SelectRandomWeightedItem<KeyValuePair<SupraRegion, int>>((KeyValuePair<SupraRegion, int> x) => (float)x.Value, -1f, 1E-37f).Key;
						}
						int maxCPsinSupraRegion2 = list.Where<TINationState>((TINationState x) => x.capital.mapRegionTemplate.supraRegion == sr2).Max<TINationState>((TINationState x) => x.numControlPoints_unclamped);
						TINationState sr2CoreNation = list.Where<TINationState>((TINationState x) => x.capital.mapRegionTemplate.supraRegion == sr2 && x.numControlPoints_unclamped >= Mathf.Min(4, maxCPsinSupraRegion2)).MaxBy<TINationState, float>((TINationState x) => x.GetPublicOpinionOfFaction(faction));
						TIFactionState.LogAI(string.Concat(new string[]
						{
							faction.displayName,
							" SupraRegion: ",
							sr2.ToString(),
							" Primary: ",
							sr2CoreNation.displayName
						}), false);
						list2.Add(sr2CoreNation);
						list.Remove(sr2CoreNation);
						List<TINationState> adjacents2 = (from x in sr2CoreNation.AdjacentNations(false)
							where !sr2CoreNation.rivals.Contains(sr2CoreNation)
							select x).ToList<TINationState>();
						if (sr2CoreNation.allies.Count > 0)
						{
							adjacents2.Add(sr2CoreNation.allies.MaxBy<TINationState, float>((TINationState x) => x.GetPublicOpinionOfFaction(faction)));
						}
						list2.AddRange(adjacents2);
						list.RemoveAll((TINationState x) => adjacents2.Contains(x));
						if (faction.IsAlienProxy)
						{
							list2.AddRange((from x in GameStateManager.IterateByClass<TIRegionUFOCrashdownState>(false)
								where x.crashdownPresent
								select x.ref_nation).ToList<TINationState>());
						}
						list2.RemoveAll((TINationState x) => x == null);
						list2 = list2.Distinct<TINationState>().ToList<TINationState>();
						int num = 15 - list2.Count;
						Func<TINationState, float> <>9__27;
						Func<TINationState, float> <>9__29;
						for (int i = 0; i < Mathf.Max(3, num); i++)
						{
							TINationState tinationState;
							if (i == 0)
							{
								IEnumerable<TINationState> enumerable2 = list;
								Func<TINationState, float> func;
								if ((func = <>9__27) == null)
								{
									func = (<>9__27 = (TINationState x) => x.GetPublicOpinionOfFaction(faction));
								}
								tinationState = enumerable2.MaxBy<TINationState, float>(func);
							}
							else
							{
								IEnumerable<TINationState> enumerable3 = list.Where<TINationState>((TINationState x) => x.numControlPoints <= 4);
								Func<TINationState, float> func2;
								if ((func2 = <>9__29) == null)
								{
									func2 = (<>9__29 = (TINationState x) => x.GetPublicOpinionOfFaction(faction) * x.population_Millions);
								}
								tinationState = enumerable3.MaxBy<TINationState, float>(func2);
							}
							list2.Add(tinationState);
							list.Remove(tinationState);
						}
						list2.AddRange(from x in GameStateManager.AllExtantHumanNations()
							where x.numControlPoints <= 3 && x.spaceFlightProgram
							select x);
						list2.RemoveAll((TINationState x) => x == null);
						list2 = list2.Distinct<TINationState>().ToList<TINationState>();
						faction.initialAINationGoals = new List<TINationState>(list2);
					}
				}
			}
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x0029FB6C File Offset: 0x0029DD6C
		public static void ManagePriorityNationControlGoalsForFaction(TIFactionState faction)
		{
			TIDateTime tidateTime = TITimeState.Now();
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			if (faction.initialAINationGoals != null)
			{
				foreach (TINationState tinationState in faction.initialAINationGoals)
				{
					int num = (int)((float)tinationState.numControlPoints_unclamped * 2.5f);
					if (tinationState.spaceFlightProgram || tinationState.boostIncome_year_dekatons > 0f || tinationState.missionControl > 0)
					{
						num = (int)((float)num + (float)tinationState.numControlPoints_unclamped * 0.5f);
						if (tinationState.numControlPoints_unclamped <= 4)
						{
							num += (int)((float)num * (Mathf.Max(tinationState.rawBoostPerYear_dekatons, tinationState.boostIncome_year_dekatons) / 3f));
						}
					}
					faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, Mathf.Min(num, 19), tinationState, faction.AI_GetPreferredManagementGoalForNation(tinationState), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
				faction.initialAINationGoals = null;
			}
			foreach (TIFactionGoalState tifactionGoalState in faction.factionGoals[GoalType.CaptureNationClean])
			{
				if (tifactionGoalState.target().ref_nation.CountFactionControlPoints(faction, true, true, true) == 0 && tifactionGoalState.target().ref_nation.NumNativeControlPoints == 0 && (double)tifactionGoalState.target().ref_nation.GetPublicOpinionOfFaction(faction) < 0.3 && tifactionGoalState.assignedDate.DifferenceInDays(tidateTime) > (double)((float)(360 * tifactionGoalState.target().ref_nation.numControlPoints) * (1f / faction.aiValues.dirtyTricks)))
				{
					list.Add(tifactionGoalState);
				}
			}
			foreach (TIFactionGoalState tifactionGoalState2 in list)
			{
				faction.RemoveGoal(tifactionGoalState2);
				if (tifactionGoalState2.importance > 10)
				{
					faction.AddGoal(new FactionGoal_CaptureNation_Dirty(faction, tifactionGoalState2.importance, tifactionGoalState2.target().ref_nation, tifactionGoalState2.subsequentGoals[0], null), HandleDuplicateGoalRule.Ignore, null);
				}
			}
			list.Clear();
			foreach (TIFactionGoalState tifactionGoalState3 in faction.factionGoals[GoalType.CaptureNationDirty])
			{
				if (tifactionGoalState3.target().ref_nation.CountFactionControlPoints(faction, true, true, true) == 0 && tifactionGoalState3.target().ref_nation.NumNativeControlPoints == 0 && tifactionGoalState3.target().ref_nation.numControlPoints_unclamped <= 4 && tifactionGoalState3.importance <= 5 && tifactionGoalState3.assignedDate.DifferenceInDays(tidateTime) > (double)(720f * (1f / faction.aiValues.dirtyTricks)))
				{
					list.Add(tifactionGoalState3);
				}
			}
			foreach (TIFactionGoalState tifactionGoalState4 in list)
			{
				faction.RemoveGoal(tifactionGoalState4);
			}
			if (!faction.veryProAlien && GameStateManager.AlienNation().extant)
			{
				faction.AddGoal(new FactionGoal_NeutralizeNation(faction, faction.veryAntiAlien ? 20 : 15, GameStateManager.AlienNation(), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
			int num2 = faction.factionGoals[GoalType.CaptureNationClean].Count + faction.factionGoals[GoalType.CaptureNationDirty].Count;
			IEnumerable<TINationState> enumerable = faction.factionGoals[GoalType.CaptureNationClean].Select<TIFactionGoalState, TINationState>((TIFactionGoalState x) => x.target().ref_nation).Union<TINationState>(faction.factionGoals[GoalType.CaptureNationDirty].Select<TIFactionGoalState, TINationState>((TIFactionGoalState x) => x.target().ref_nation));
			List<TINationState> nationsWithMyControlPoints = faction.nationsWithMyControlPoints;
			nationsWithMyControlPoints.AddRangeUnique<TINationState>((from x in faction.lostControlPoints.Keys
				where TITimeState.Now().DifferenceInDays(faction.lostControlPoints[x]) < 15.0
				select x.nation into x
				where x.extant
				select x).ToList<TINationState>());
			using (List<TINationState>.Enumerator enumerator = nationsWithMyControlPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TINationState nation = enumerator.Current;
					if (nation.FactionHasControlPoint(faction))
					{
						faction.SetManagementGoalForNation(nation);
					}
					if (!enumerable.Contains(nation) && faction.FindGoals(new List<GoalType>
					{
						GoalType.NeutralizeNation,
						GoalType.PillageNation
					}, faction, nation, TIFactionState.GoalFilter.none, true).Count == 0)
					{
						if (nation.executiveFaction != faction)
						{
							faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 5 + 2 * nation.numControlPoints_unclamped, nation, faction.AI_GetPreferredManagementGoalForNation(nation), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
							enumerable.Append(nation);
							num2++;
						}
						else
						{
							foreach (TINationState tinationState2 in nation.allies)
							{
								if (!enumerable.Contains(tinationState2) && tinationState2.TotalOwningFaction != faction)
								{
									if (nationsWithMyControlPoints.Contains(tinationState2))
									{
										goto IL_06A7;
									}
									if (!tinationState2.controlPoints.All<TIControlPoint>((TIControlPoint x) => x.defended))
									{
										goto IL_06A7;
									}
									faction.AddGoal(new FactionGoal_CaptureNation_Dirty(faction, 5 + 2 * nation.numControlPoints_unclamped, tinationState2, faction.AI_GetPreferredManagementGoalForNation(tinationState2), null), HandleDuplicateGoalRule.Ignore, null);
									IL_06E0:
									enumerable.Append(nation);
									num2++;
									continue;
									IL_06A7:
									faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 5 + 2 * nation.numControlPoints_unclamped, tinationState2, faction.AI_GetPreferredManagementGoalForNation(tinationState2), null), HandleDuplicateGoalRule.Ignore, null);
									goto IL_06E0;
								}
							}
							IEnumerable<TINationState> enemies = nation.enemies;
							Func<TINationState, bool> func;
							Func<TINationState, bool> <>9__12;
							if ((func = <>9__12) == null)
							{
								func = (<>9__12 = (TINationState x) => nation.HasClaimOnOtherNation(x, true));
							}
							foreach (TINationState tinationState3 in enemies.Where<TINationState>(func).ToList<TINationState>())
							{
								if (!enumerable.Contains(tinationState3) && tinationState3.TotalOwningFaction != faction)
								{
									if (nation.NumNativeControlPoints <= 1)
									{
										faction.AddGoal(new FactionGoal_CaptureNation_Dirty(faction, 5 + 2 * nation.numControlPoints_unclamped, tinationState3, faction.AI_GetPreferredManagementGoalForNation(tinationState3), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
									}
									else
									{
										faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 5 + 2 * nation.numControlPoints_unclamped, tinationState3, faction.AI_GetPreferredManagementGoalForNation(tinationState3), null), HandleDuplicateGoalRule.Ignore, null);
									}
								}
							}
						}
					}
					if (nation.alienNation)
					{
						List<TINationState> list2 = nation.AdjacentNations(true);
						if (list2.Count > 0)
						{
							using (List<TINationState>.Enumerator enumerator3 = list2.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									TINationState tinationState4 = enumerator3.Current;
									TIFactionState executiveFaction = tinationState4.executiveFaction;
									if (executiveFaction != null && executiveFaction.permanentAlly(faction))
									{
										faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 16, tinationState4, GoalType.ExpandNation, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
									}
									else
									{
										faction.AddGoal(new FactionGoal_CaptureNation_Dirty(faction, 15 + nation.numStandardArmies + nation.numNuclearWeapons, tinationState4, GoalType.ExpandNation, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
									}
								}
								continue;
							}
						}
						if (nation.armies.Count > 0)
						{
							TIArmyState armyToCheckFrom = nation.armies.FirstOrDefault<TIArmyState>((TIArmyState x) => x.currentRegion == nation.capital) ?? nation.armies.FirstOrDefault<TIArmyState>((TIArmyState x) => x.currentNation == nation);
							if (armyToCheckFrom != null)
							{
								if (GameStateManager.AllNations().Any<TINationState>((TINationState x) => x.extant && !x.alienNation))
								{
									TIRegionState tiregionState = (from x in GameStateManager.AllRegions()
										where x.nation != nation
										select x).MinBy<TIRegionState, float>((TIRegionState x) => armyToCheckFrom.currentRegion.DistanceToRegion_km(x));
									TINationState tinationState5 = ((tiregionState != null) ? tiregionState.nation : null);
									if (tinationState5 != null)
									{
										TIFactionState executiveFaction2 = tinationState5.executiveFaction;
										if (executiveFaction2 != null && executiveFaction2.permanentAlly(faction))
										{
											faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 16, tinationState5, GoalType.ExpandNation, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
										}
										else
										{
											faction.AddGoal(new FactionGoal_CaptureNation_Dirty(faction, 10 + nation.numStandardArmies + nation.numNuclearWeapons, tinationState5, GoalType.ExpandNation, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
										}
									}
								}
							}
						}
					}
				}
			}
			List<TINationState> list3 = (from x in GameStateManager.AllExtantHumanNations()
				where x.MajorGlobalPower
				select x).ToList<TINationState>();
			List<TINationState> list4 = faction.executiveNations.Intersect<TINationState>(list3).Intersect<TINationState>(from x in faction.AllCaptureNationGoals(true)
				select x.target().ref_nation).ToList<TINationState>();
			if (faction.numActiveCouncilors >= 4 && list3.Count > 0 && list4.Count == 0)
			{
				list3 = (from x in list3
					orderby x.CountFactionControlPoints(faction, true, false, true) descending, x.controlPoints.Count<TIControlPoint>((TIControlPoint y) => y.benefitsDisabled) descending, x.controlPoints.Count<TIControlPoint>((TIControlPoint y) => y.defended), AIEvaluators.EvaluateNation(faction, x) descending
					select x).ToList<TINationState>();
				faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 17, list3[0], faction.AI_GetPreferredManagementGoalForNation(list3[0]), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				foreach (TINationState tinationState6 in list3[0].AdjacentNations(false))
				{
					faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, 15, tinationState6, faction.AI_GetPreferredManagementGoalForNation(tinationState6), null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					num2++;
				}
			}
			if (faction.IsActiveHumanFaction && faction.majorCPTrouble)
			{
				using (List<TIFactionGoalState>.Enumerator enumerator2 = faction.AllCaptureNationGoals(true).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIFactionGoalState tifactionGoalState5 = enumerator2.Current;
						TINationState ref_nation = tifactionGoalState5.target().ref_nation;
						if (!ref_nation.alienNation && !ref_nation.MajorGlobalPower && !ref_nation.FactionHasControlPoint(faction) && ref_nation.AdjacentNations(false).Intersect<TINationState>(list4).Count<TINationState>() <= 0)
						{
							tifactionGoalState5.SetImportance(ref_nation.numControlPoints - 2);
						}
					}
					return;
				}
			}
			int num3 = faction.councilors.Where<TICouncilorState>((TICouncilorState x) => x.OnOrAroundEarth).Count<TICouncilorState>();
			if (num2 < 2 * num3)
			{
				List<TINationState> list5 = GameStateManager.AllExtantNations().Except<TINationState>(enumerable).Except<TINationState>(faction.totalControlNations)
					.ToList<TINationState>();
				if (enumerable.Count<TINationState>((TINationState x) => x.numControlPoints >= 5) > 3)
				{
					list5.RemoveAll((TINationState x) => x.numControlPoints >= 5);
				}
				if (enumerable.Count<TINationState>((TINationState x) => x.numControlPoints == 4) > 7)
				{
					list5.RemoveAll((TINationState x) => x.numControlPoints == 4);
				}
				if (enumerable.Count<TINationState>() > 0)
				{
					if (enumerable.All<TINationState>((TINationState x) => x.numControlPoints <= 3))
					{
						list5.RemoveAll((TINationState x) => x.numControlPoints <= 3);
					}
				}
				if (list5.Count > 0)
				{
					list5 = list5.OrderByDescending<TINationState, float>((TINationState x) => AIEvaluators.EvaluateNation(faction, x) * (0.5f + x.GetPublicOpinionOfFaction(faction.ideology))).ToList<TINationState>();
					int num4 = num3 - num2;
					for (int i = 0; i < num4; i++)
					{
						AIDailyFactionPlanner.<>c__DisplayClass37_3 CS$<>8__locals4 = new AIDailyFactionPlanner.<>c__DisplayClass37_3();
						AIDailyFactionPlanner.<>c__DisplayClass37_3 CS$<>8__locals5 = CS$<>8__locals4;
						TINationState tinationState7;
						if (TIUtilities.RandomFloatValue() >= 0.75f)
						{
							tinationState7 = list5.SelectRandomWeightedItem<TINationState>((TINationState x) => (float)x.NumNativeControlPoints, -1f, 1E-37f);
						}
						else
						{
							tinationState7 = list5[0];
						}
						CS$<>8__locals5.targetNation = tinationState7;
						List<TINationState> list6 = new List<TINationState>();
						list6.Add(CS$<>8__locals4.targetNation);
						list6.AddRange(new List<TINationState>((from x in CS$<>8__locals4.targetNation.allies.Union<TINationState>(CS$<>8__locals4.targetNation.AdjacentNations(false))
							where x.numControlPoints < CS$<>8__locals4.targetNation.numControlPoints
							select x).ToList<TINationState>()));
						foreach (TINationState tinationState8 in list6)
						{
							int num5 = Mathf.Clamp(2 * tinationState8.numControlPoints_unclamped + tinationState8.numStandardArmies + num4 - i + ((CS$<>8__locals4.targetNation == tinationState8) ? 2 : 0), 1, 19);
							GoalType goalType = faction.AI_GetPreferredManagementGoalForNation(tinationState8);
							if (tinationState8.controlPoints.All<TIControlPoint>((TIControlPoint x) => x.defended) || (CS$<>8__locals4.targetNation.enemies.Contains(tinationState8) && num3 >= CS$<>8__locals4.targetNation.numControlPoints))
							{
								faction.AddGoal(new FactionGoal_CaptureNation_Dirty(faction, num5, tinationState8, goalType, null), HandleDuplicateGoalRule.ResetImportance, null);
							}
							else
							{
								faction.AddGoal(new FactionGoal_CaptureNation_Clean(faction, num5, tinationState8, goalType, null), HandleDuplicateGoalRule.ResetImportance, null);
							}
							list5.Remove(tinationState8);
						}
						if (list5.Count == 0)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x002A0CE0 File Offset: 0x0029EEE0
		public static void DisableOwnNations(TIFactionState faction, Dictionary<TIControlPoint, float> controlPointValues)
		{
			bool flag = faction.numActiveCouncilors > 0;
			float num = ((!flag) ? 0f : ((float)TemplateManager.global.AI_BaseAllowedOverageCPMaintenance * (1.65f - faction.aiValues.riskAversion)));
			if (faction.GetAnnualControlPointMaintenanceCost() > num)
			{
				Dictionary<TINationState, float> NationCPValues = new Dictionary<TINationState, float>();
				Func<TIArmyState, bool> <>9__2;
				Func<TIControlPoint, float> <>9__3;
				foreach (TINationState tinationState in faction.nationsWithMyControlPoints)
				{
					if (!tinationState.MajorGlobalPower || !flag)
					{
						if (!tinationState.FactionControlPoints(faction, true, false, true).All<TIControlPoint>((TIControlPoint x) => x.benefitsDisabled))
						{
							bool flag2;
							if (tinationState.atWar && tinationState.SignificantPower)
							{
								IEnumerable<TIArmyState> armies = tinationState.armies;
								Func<TIArmyState, bool> func;
								if ((func = <>9__2) == null)
								{
									func = (<>9__2 = (TIArmyState x) => x.faction == faction);
								}
								flag2 = armies.Count<TIArmyState>(func) > 0;
							}
							else
							{
								flag2 = false;
							}
							if ((!flag2 || !flag) && (faction.GetDailyIncome(FactionResource.Influence, false, false) <= 0f || (!tinationState.SignificantPower && (tinationState.numControlPoints < 3 || tinationState.perCapitaGDP <= 40000f) && (AIEvaluators.Abundant(faction, FactionResource.MissionControl, 0f, faction.AvailableMissionControlMinusFutureUsage > 0, 1f) || tinationState.GetFactionMissionControlFromNation(faction, false) <= 0f))))
							{
								IEnumerable<TIControlPoint> enumerable = tinationState.FactionControlPoints(faction, true, false, true);
								Func<TIControlPoint, float> func2;
								if ((func2 = <>9__3) == null)
								{
									func2 = (<>9__3 = delegate(TIControlPoint x)
									{
										if (!controlPointValues.ContainsKey(x))
										{
											return AIEvaluators.EvaluateControlPoint(faction, x);
										}
										return controlPointValues[x];
									});
								}
								float num2 = enumerable.Sum<TIControlPoint>(func2);
								if (tinationState.executiveFaction == faction)
								{
									num2 *= (float)tinationState.numControlPoints_unclamped;
								}
								else
								{
									num2 *= 1f + (float)tinationState.NumNativeControlPoints / 2f;
								}
								List<TIFactionGoalState> list = faction.FindGoals(TIFactionGoalState.BenevolentNationManagementGoals, faction, tinationState, TIFactionState.GoalFilter.none, true);
								if (list.Count > 0)
								{
									num2 /= (float)list[0].importance;
								}
								else
								{
									List<TIFactionGoalState> list2 = faction.FindGoals(TIFactionGoalState.CaptureNationGoals, faction, tinationState, TIFactionState.GoalFilter.none, true);
									if (list2.Count > 0)
									{
										num2 /= (float)list2[0].importance;
									}
								}
								NationCPValues.Add(tinationState, num2);
							}
						}
					}
				}
				foreach (TINationState tinationState2 in NationCPValues.Keys.OrderBy<TINationState, float>((TINationState x) => NationCPValues[x]))
				{
					faction.playerControl.StartAction(new SelfDisableControlPoints(faction, tinationState2));
					if (faction.GetAnnualControlPointMaintenanceCost() <= num && faction.GetDailyIncome(FactionResource.Influence, false, true) > 0f)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06005A34 RID: 23092 RVA: 0x002A105C File Offset: 0x0029F25C
		public static void ConsiderNuclearAttack(TINationState nation, TINationState nationNukingUs = null, bool nationNukingOurInvadingArmies = false)
		{
			if (nation.numNuclearWeapons > 0)
			{
				Dictionary<TINationState, int> dictionary = new Dictionary<TINationState, int>();
				TIGlobalValuesState globalValues = TIGlobalValuesState.GlobalValues;
				if (nationNukingUs != null)
				{
					int num = globalValues.currentNuclearExchanges.Where<NuclearExchange>((NuclearExchange x) => x.attacker == nationNukingUs && x.enemyTargeted == nation).Count<NuclearExchange>();
					int num2 = globalValues.currentNuclearExchanges.Where<NuclearExchange>((NuclearExchange x) => x.attacker == nation && x.enemyTargeted == nationNukingUs).Count<NuclearExchange>();
					if (num > num2)
					{
						dictionary.Add(nationNukingUs, nationNukingOurInvadingArmies ? 3 : 5);
					}
				}
				List<TIArmyState> list = nation.regions.SelectMany<TIRegionState, TIArmyState>((TIRegionState x) => x.FilteredArmiesPresent(false, false, true, false, false)).ToList<TIArmyState>();
				List<TIRegionState> list2 = new List<TIRegionState>();
				foreach (TIArmyState tiarmyState in list)
				{
					if ((!(nation.executiveFaction != null) || !nation.executiveFaction.veryProAlien || (!tiarmyState.AlienRegularArmy && !tiarmyState.AlienMegafaunaArmy)) && (!tiarmyState.AlienMegafaunaArmy || nation.NumArmiesDefendingMe() <= 0))
					{
						list2.Add(tiarmyState.currentRegion);
						TINationState tinationState;
						bool flag = (tiarmyState.OccupyingRegion(false) || tiarmyState.currentRegion.annexingArmy == tiarmyState) && (tiarmyState.currentRegion.GetHighestWarAllianceOccupationValueByNation(tiarmyState.homeNation, out tinationState) >= 0.99f - 0.1f * (float)tiarmyState.currentRegion.NumArmiesPresent(false, false, true, false) || tiarmyState.InBattleWithOtherArmiesAndWinningByALot());
						int num3 = 0;
						if (flag)
						{
							if (!(tiarmyState.currentRegion == nation.capital))
							{
								if (!nation.findWarsWith(tiarmyState.homeNation).Any<TIWarState>((TIWarState x) => x.defensiveNukes > 1 || x.nukedRegions.Count > 0))
								{
									if (tiarmyState.currentRegion.annexingArmy == tiarmyState)
									{
										num3 = (tiarmyState.currentRegion.colonyRegion ? 2 : 3);
										goto IL_024F;
									}
									if (!tiarmyState.homeNation.WinningWarAgainst(nation))
									{
										goto IL_024F;
									}
									num3 = 2;
									if (tiarmyState.AlienRegularArmy)
									{
										num3 = 3;
										goto IL_024F;
									}
									goto IL_024F;
								}
							}
							num3 = 4;
						}
						IL_024F:
						if (num3 <= 3 && tiarmyState.homeNation.numNuclearWeapons > 0 && !tiarmyState.homeNation.alienNation)
						{
							num3--;
							if (tiarmyState.homeNation.numNuclearWeapons > nation.numNuclearWeapons + 1)
							{
								num3--;
							}
						}
						if (num3 > 0)
						{
							if (!dictionary.ContainsKey(tiarmyState.homeNation))
							{
								dictionary.Add(tiarmyState.homeNation, num3);
							}
							else if (tiarmyState.currentRegion == nation.capital)
							{
								dictionary[tiarmyState.homeNation] = Mathf.Max(dictionary[tiarmyState.homeNation], num3);
							}
						}
					}
				}
				foreach (TIArmyState tiarmyState2 in nation.allies.Where<TINationState>((TINationState x) => x.numNuclearWeapons == 0).SelectMany<TINationState, TIRegionState>((TINationState x) => x.regions).SelectMany<TIRegionState, TIArmyState>((TIRegionState x) => x.FilteredArmiesPresent(false, false, true, false, false))
					.ToList<TIArmyState>())
				{
					if ((!(nation.executiveFaction != null) || !nation.executiveFaction.veryProAlien || (!tiarmyState2.AlienRegularArmy && !tiarmyState2.AlienMegafaunaArmy)) && (!tiarmyState2.AlienMegafaunaArmy || tiarmyState2.currentNation.NumArmiesDefendingMe() <= 0))
					{
						list2.Add(tiarmyState2.currentRegion);
						int num4 = 0;
						TINationState tinationState2;
						if (tiarmyState2.currentNation.numNuclearWeapons == 0 && ((tiarmyState2.OccupyingRegion(false) && tiarmyState2.currentRegion.GetHighestWarAllianceOccupationValueByNation(tiarmyState2.homeNation, out tinationState2) >= 0.65f) || tiarmyState2.InBattleWithOtherArmiesAndWinningByALot()) && tiarmyState2.currentRegion == tiarmyState2.currentNation.capital)
						{
							num4 = tiarmyState2.currentNation.numControlPoints / 2;
						}
						if (num4 > 0)
						{
							if (num4 <= 3 && tiarmyState2.homeNation.numNuclearWeapons > 2 && !tiarmyState2.AlienRegularArmy)
							{
								num4--;
								if (tiarmyState2.homeNation.numNuclearWeapons > nation.numNuclearWeapons)
								{
									num4--;
								}
							}
							if (!dictionary.ContainsKey(tiarmyState2.homeNation))
							{
								dictionary.Add(tiarmyState2.homeNation, num4);
							}
							else
							{
								dictionary[tiarmyState2.homeNation] = Mathf.Max(dictionary[tiarmyState2.homeNation], 2);
							}
						}
					}
				}
				TIFactionState executiveFaction = nation.executiveFaction;
				if (executiveFaction != null && executiveFaction.IsActiveHumanFaction && nation.executiveFaction.aiValues.protectHumanLife < 0.5f)
				{
					foreach (TINationState tinationState3 in nation.wars)
					{
						if (!nation.WinningWarAgainst(tinationState3) && nation.executiveFaction.GetFactionHate(tinationState3.executiveFaction) >= 100f && tinationState3.NumNuclearWeaponsDefendingMeAgainst(nation) == 0 && !dictionary.ContainsKey(tinationState3))
						{
							dictionary.Add(tinationState3, 1);
						}
					}
				}
				List<TINationState> list3 = new List<TINationState>();
				foreach (TINationState tinationState4 in dictionary.Keys)
				{
					TIFactionState executiveFaction2 = tinationState4.executiveFaction;
					if (executiveFaction2 != null && executiveFaction2.permanentAlly(nation.executiveFaction))
					{
						list3.Add(tinationState4);
					}
				}
				if (nation.alienNation)
				{
					foreach (TINationState tinationState5 in dictionary.Keys)
					{
						if (dictionary[tinationState5] <= 3)
						{
							list3.Add(tinationState5);
						}
					}
				}
				foreach (TINationState tinationState6 in list3)
				{
					dictionary.Remove(tinationState6);
				}
				if (dictionary.Count > 0)
				{
					bool flag2;
					if (!nation.alienNation && !nationNukingOurInvadingArmies)
					{
						TIFactionState executiveFaction3 = nation.executiveFaction;
						if (executiveFaction3 != null && executiveFaction3.extremist)
						{
							flag2 = nationNukingUs == null;
							goto IL_072C;
						}
					}
					flag2 = true;
					IL_072C:
					bool flag3 = flag2;
					list2 = list2.Distinct<TIRegionState>().ToList<TIRegionState>();
					TINationState targetNation = dictionary.MaxBy<KeyValuePair<TINationState, int>, int>((KeyValuePair<TINationState, int> x) => x.Value).Key;
					IEnumerable<TIRegionState> enumerable = nation.NuclearWeaponsTargets(flag3);
					List<TIRegionState> list4 = new List<TIRegionState>(list2);
					IEnumerable<TIRegionState> enumerable2 = from x in targetNation.armies
						where !x.AlienMegafaunaArmy && x.InFriendlyRegion
						select x.currentRegion;
					if (enumerable2.Count<TIRegionState>() > 0 && list2.Count == 0 && (nation.NumNuclearWeaponsThreateningMeInWars() == 0 || nationNukingUs != null))
					{
						list4.AddRange(enumerable2.Distinct<TIRegionState>());
					}
					if (!flag3)
					{
						list4 = list4.Union<TIRegionState>(targetNation.regions).ToList<TIRegionState>();
					}
					IEnumerable<TIRegionState> enumerable3 = enumerable.Intersect<TIRegionState>(list4).ToList<TIRegionState>().Except<TIRegionState>(globalValues.currentNuclearExchanges.Select<NuclearExchange, TIRegionState>((NuclearExchange x) => x.target))
						.ToList<TIRegionState>();
					List<TIWarState> list5 = nation.findWarsWith(targetNation);
					IEnumerable<TIRegionState> enumerable4;
					if (list5 == null)
					{
						enumerable4 = null;
					}
					else
					{
						IEnumerable<TIRegionState> enumerable5 = list5.SelectMany<TIWarState, TIRegionState>((TIWarState x) => x.nukedRegions);
						enumerable4 = ((enumerable5 != null) ? enumerable5.Except<TIRegionState>(list2) : null);
					}
					IEnumerable<TIRegionState> enumerable6 = enumerable4;
					Dictionary<TIRegionState, float> dictionary2 = enumerable3.Except<TIRegionState>(enumerable6).ToList<TIRegionState>().ToDictionary<TIRegionState, TIRegionState, float>((TIRegionState x) => x, (TIRegionState x) => AIEvaluators.ScoreNuclearTarget(nation, x, targetNation));
					if (dictionary2.Values.All<float>((float x) => x == 0f))
					{
						foreach (TIRegionState tiregionState in new List<TIRegionState>(dictionary2.Keys))
						{
							if (tiregionState.nation == targetNation)
							{
								dictionary2[tiregionState] = (tiregionState.colonyRegion ? 0.0001f : 1f);
							}
							else
							{
								dictionary2[tiregionState] = 0f;
							}
						}
					}
					if (dictionary2.Values.Any<float>((float x) => x > 0f))
					{
						TIRegionState tiregionState2 = null;
						if (AIDailyFactionPlanner.ExpectNukeLaunch(dictionary[targetNation], nation, targetNation))
						{
							tiregionState2 = dictionary2.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> x) => x.Value, -1f, 1E-37f).Key;
						}
						if (tiregionState2 != null)
						{
							if (nation.executiveFaction != null)
							{
								nation.executiveFaction.playerControl.StartAction(new ConfirmPolicyAction(nation, nation.executiveFaction, tiregionState2, null, new EmployNuclearWeaponsOption()));
								return;
							}
							new EmployNuclearWeaponsOption().OnConfirm(nation, tiregionState2);
						}
					}
				}
			}
		}

		// Token: 0x06005A35 RID: 23093 RVA: 0x002A1BE8 File Offset: 0x0029FDE8
		public static bool ExpectNukeLaunch(int situation, TINationState nationWithNukes, TINationState targetNation)
		{
			switch (situation)
			{
			case 1:
				return nationWithNukes.numNuclearWeapons > 10;
			case 2:
				return nationWithNukes.numNuclearWeapons > (targetNation.alienNation ? 5 : 10);
			case 3:
				return nationWithNukes.numNuclearWeapons >= (targetNation.alienNation ? 2 : 5);
			case 4:
				return true;
			case 5:
				return true;
			default:
				return false;
			}
		}

		// Token: 0x06005A36 RID: 23094 RVA: 0x002A1C54 File Offset: 0x0029FE54
		public static void ManageFleetGoals(TIFactionState faction)
		{
			IEnumerable<FactionGoal_DefendWithFleet> enumerable = from x in faction.GoalsOfType(GoalType.DefendWithFleet, false, true)
				select x as FactionGoal_DefendWithFleet;
			Dictionary<TISpaceObjectState, Dictionary<TIFactionState, float>> systemFleetStrengths = AIEvaluators.SystemFleetStrengths;
			IEnumerable<TINaturalSpaceObjectState> enumerable2 = faction.bases.Select<TIHabState, TINaturalSpaceObjectState>((TIHabState x) => x.ref_naturalSpaceObject).Distinct<TINaturalSpaceObjectState>();
			using (IEnumerator<TIGameState> enumerator = faction.stations.Union<TIGameState>(enumerable2).GetEnumerator())
			{
				Func<TIHabState, bool> <>9__3;
				while (enumerator.MoveNext())
				{
					TIGameState defenseTarget = enumerator.Current;
					FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = enumerable.FirstOrDefault<FactionGoal_DefendWithFleet>((FactionGoal_DefendWithFleet x) => x.target() == defenseTarget);
					TISpaceObjectState getSunOrbitingRelatedObject = defenseTarget.ref_spaceObject.GetSunOrbitingRelatedObject;
					float num = 0f;
					TIHabState tihabState;
					if (defenseTarget.isHabState)
					{
						tihabState = defenseTarget.ref_hab;
						num += tihabState.SpaceCombatValue();
					}
					else
					{
						if (!defenseTarget.isSpaceBodyState)
						{
							continue;
						}
						List<TIHabState> surfaceBases = defenseTarget.ref_spaceBody.surfaceBases;
						IEnumerable<TIHabState> enumerable3 = surfaceBases;
						Func<TIHabState, bool> func;
						if ((func = <>9__3) == null)
						{
							func = (<>9__3 = (TIHabState x) => x == faction.primaryHab);
						}
						tihabState = enumerable3.FirstOrDefault<TIHabState>(func);
						if (tihabState == null)
						{
							tihabState = surfaceBases.MaxBy<TIHabState, int>((TIHabState x) => x.tier);
						}
					}
					if (factionGoal_DefendWithFleet != null && factionGoal_DefendWithFleet.assignedFleet != null)
					{
						num += factionGoal_DefendWithFleet.assignedFleet.SpaceCombatValue();
					}
					bool flag = false;
					foreach (TIFactionState tifactionState in AIDailyFactionPlanner.factionAIData[faction].enemyFactions)
					{
						float num2 = 0f;
						if (systemFleetStrengths.ContainsKey(getSunOrbitingRelatedObject) && systemFleetStrengths[getSunOrbitingRelatedObject].ContainsKey(tifactionState))
						{
							num2 = systemFleetStrengths[getSunOrbitingRelatedObject][tifactionState] * faction.GetPerceivedEnemyFleetStrengthFactor(tifactionState);
						}
						if (AIEvaluators.IsDefenseFeasible(faction, defenseTarget, num2))
						{
							float requiredDefenseStrength = AIEvaluators.GetRequiredDefenseStrength(faction, tifactionState, num2, tihabState);
							if (num < requiredDefenseStrength)
							{
								flag = true;
							}
						}
					}
					bool flag2 = tihabState == faction.GetMainBaseInSystem(tihabState.ref_system) && tihabState.ref_system.objectType == SpaceObjectType.Planet;
					if (factionGoal_DefendWithFleet == null && ((faction.IsAlienFaction && (tihabState.tier > 1 || flag2)) || tihabState == faction.primaryHab || flag))
					{
						string text = "";
						if (faction.IsAlienFaction && tihabState == faction.primaryHab)
						{
							TIShipHullTemplate flagshipHull = faction.FlagshipHull;
							text = ((flagshipHull != null) ? flagshipHull.dataName : null) ?? "";
						}
						factionGoal_DefendWithFleet = new FactionGoal_DefendWithFleet(faction, 1, defenseTarget, text);
						factionGoal_DefendWithFleet = faction.AddGoal(factionGoal_DefendWithFleet, HandleDuplicateGoalRule.ResetImportance, null) as FactionGoal_DefendWithFleet;
						if (factionGoal_DefendWithFleet != null)
						{
							enumerable = enumerable.Append(factionGoal_DefendWithFleet);
						}
					}
					if (factionGoal_DefendWithFleet != null)
					{
						int num3 = 10 + tihabState.tier + 1;
						bool flag3 = flag2 && tihabState.ref_system == faction.GetInnermostColonizedPlanet();
						if (faction.IsAlienFaction && tihabState == faction.primaryHab && flag)
						{
							num3 = 20;
						}
						else if (faction.IsAlienFaction && flag3)
						{
							num3 = 17;
						}
						else if (flag)
						{
							num3 = 16;
						}
						else if (tihabState == faction.primaryHab)
						{
							num3 = 15;
							TIFactionState mostThreateningEnemyHumanFaction = faction.GetMostThreateningEnemyHumanFaction();
							if (faction.enemyTotalWarFactions.Count > 0 || faction.enemyWarFactions.Contains(mostThreateningEnemyHumanFaction))
							{
								num3++;
							}
							if (faction.IsAlienFaction)
							{
								if (GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x.habs.Any<TIHabState>((TIHabState y) => y.GetSunOrbitingRelatedObject.semiMajorAxis_AU >= GameStateManager.Jupiter().semiMajorAxis_AU)))
								{
									num3 += 3;
								}
							}
						}
						factionGoal_DefendWithFleet.SetImportance(num3);
					}
				}
			}
			if (faction.primaryHab != null)
			{
				FactionGoal_DefendWithFleet factionGoal_DefendWithFleet2 = enumerable.FirstOrDefault<FactionGoal_DefendWithFleet>((FactionGoal_DefendWithFleet defenseGoal) => defenseGoal.target() == faction.primaryHab || (faction.primaryHab.IsBase && defenseGoal.target() == faction.primaryHab.ref_naturalSpaceObject));
				if (factionGoal_DefendWithFleet2 != null && factionGoal_DefendWithFleet2.importance == 20)
				{
					foreach (FactionGoal_DefendWithFleet factionGoal_DefendWithFleet3 in enumerable)
					{
						if (factionGoal_DefendWithFleet3.importance == 20 && factionGoal_DefendWithFleet3 != factionGoal_DefendWithFleet2)
						{
							factionGoal_DefendWithFleet3.SetImportance(19);
						}
					}
				}
			}
			if (faction.IsAlienFaction)
			{
				IEnumerable<FactionGoal_TransportCouncilorsWithFleet> enumerable4 = from x in faction.GoalsOfType(GoalType.TransportCouncilorsViaFleet, false, true)
					select x as FactionGoal_TransportCouncilorsWithFleet;
				FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = enumerable4.FirstOrDefault<FactionGoal_TransportCouncilorsWithFleet>((FactionGoal_TransportCouncilorsWithFleet x) => x.IsFrontGoal);
				if (factionGoal_TransportCouncilorsWithFleet != null)
				{
					int num4 = enumerable4.Max<FactionGoal_TransportCouncilorsWithFleet>((FactionGoal_TransportCouncilorsWithFleet x) => x.importance);
					factionGoal_TransportCouncilorsWithFleet.SetImportance(num4);
				}
			}
		}

		// Token: 0x06005A37 RID: 23095 RVA: 0x002A228C File Offset: 0x002A048C
		private void ManageFleets(TIFactionState faction)
		{
			AIDailyFactionPlanner.<>c__DisplayClass42_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass42_0();
			CS$<>8__locals1.faction = faction;
			foreach (TISpaceFleetState tispaceFleetState in CS$<>8__locals1.faction.fleets)
			{
				FactionGoal_Fleet factionGoal_Fleet = tispaceFleetState.AssignedGoal();
				if (factionGoal_Fleet != null && factionGoal_Fleet.faction != tispaceFleetState.faction)
				{
					TIFactionState faction2 = factionGoal_Fleet.faction;
					if (faction2 != null && faction2.fleetGoalTracker.ContainsKey(tispaceFleetState))
					{
						tispaceFleetState.AssignedGoal().faction.fleetGoalTracker.Remove(tispaceFleetState);
					}
					tispaceFleetState.AssignedGoal().UnassignFleet();
				}
				if (!CS$<>8__locals1.faction.fleetGoalTracker.ContainsKey(tispaceFleetState))
				{
					if (factionGoal_Fleet == null || factionGoal_Fleet.ShouldDiscardGoal())
					{
						CS$<>8__locals1.faction.fleetGoalTracker.Add(tispaceFleetState, null);
					}
					else
					{
						CS$<>8__locals1.faction.fleetGoalTracker.Add(tispaceFleetState, factionGoal_Fleet);
					}
				}
			}
			if (AIEvaluators.IsPrimarySystemInPeril(CS$<>8__locals1.faction))
			{
				TISpaceBodyState primarySystem = CS$<>8__locals1.faction.primaryHab.ref_system;
				TIHabState primaryStation = CS$<>8__locals1.faction.primaryStation;
				TIOrbitState tiorbitState = ((primaryStation != null) ? primaryStation.ref_orbit : null) ?? primarySystem.orbits.FirstOrDefault<TIOrbitState>();
				bool flag = AIEvaluators.IsPrimarySystemCampedOrSoonToBe(CS$<>8__locals1.faction);
				TISpaceFleetState tispaceFleetState2 = null;
				if (flag)
				{
					FactionGoal_AttackWithFleet factionGoal_AttackWithFleet = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.AttackWithFleet, true, true)
						where x.target().isSpaceFleetState
						where x.target().ref_system == primarySystem
						select x).FirstOrDefault<TIFactionGoalState>() as FactionGoal_AttackWithFleet;
					if (factionGoal_AttackWithFleet == null)
					{
						float primarySystemCampedSoonCutoff_days = AIEvaluators.PrimarySystemCampedSoonCutoff_days;
						TISpaceFleetState tispaceFleetState3 = (from x in AIEvaluators.GetEnemyFleetsInSystemOrSoonToArrive(CS$<>8__locals1.faction, primarySystem, primarySystemCampedSoonCutoff_days)
							orderby !x.inTransfer descending, x.SpaceCombatValue() descending
							select x).FirstOrDefault<TISpaceFleetState>();
						if (tispaceFleetState3 != null)
						{
							FactionGoal_AttackWithFleet factionGoal_AttackWithFleet2 = new FactionGoal_AttackWithFleet(CS$<>8__locals1.faction, 20, tispaceFleetState3, false, null, false);
							factionGoal_AttackWithFleet2 = CS$<>8__locals1.faction.AddGoal(factionGoal_AttackWithFleet2, HandleDuplicateGoalRule.ResetImportance, null) as FactionGoal_AttackWithFleet;
							if (factionGoal_AttackWithFleet2 != null)
							{
								factionGoal_AttackWithFleet = factionGoal_AttackWithFleet2;
							}
						}
					}
					if (factionGoal_AttackWithFleet != null)
					{
						if (factionGoal_AttackWithFleet.assignedFleet == null)
						{
							TISpaceFleetState tispaceFleetState4 = (from x in CS$<>8__locals1.faction.fleets
								where x.ref_system != primarySystem
								where !x.inTransfer
								orderby x.dockedAtStation && x.ref_hab.faction == CS$<>8__locals1.faction descending
								select x).ThenByDescending<TISpaceFleetState, bool?>(delegate(TISpaceFleetState x)
							{
								TISpaceBodyState ref_system = x.ref_system;
								if (ref_system == null)
								{
									return null;
								}
								IEnumerable<TIHabState> habsInSystem = ref_system.habsInSystem;
								Func<TIHabState, bool> func4;
								if ((func4 = CS$<>8__locals1.<>9__39) == null)
								{
									func4 = (CS$<>8__locals1.<>9__39 = (TIHabState y) => y.IsStation && CS$<>8__locals1.faction == y.faction);
								}
								return new bool?(habsInSystem.Any<TIHabState>(func4));
							}).ThenByDescending<TISpaceFleetState, bool>((TISpaceFleetState x) => !x.AI_NeedsRepairBadly() && !x.AI_NeedsRefuelBadly()).ThenByDescending<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue())
								.FirstOrDefault<TISpaceFleetState>();
							if (tispaceFleetState4 != null)
							{
								factionGoal_AttackWithFleet.AssignFleet(tispaceFleetState4);
							}
							foreach (TIFactionGoalState tifactionGoalState in CS$<>8__locals1.faction.GoalsOfType(GoalType.AttackWithFleet, false, true))
							{
								if (tifactionGoalState.importance == 20)
								{
									tifactionGoalState.SetImportance(19);
								}
							}
						}
						if (factionGoal_AttackWithFleet.assignedFleet != null)
						{
							factionGoal_AttackWithFleet.SetImportance(20);
							tispaceFleetState2 = factionGoal_AttackWithFleet.assignedFleet;
						}
					}
				}
				List<TISpaceFleetState> list = (from x in (from x in CS$<>8__locals1.faction.fleets
						where x.ref_system != primarySystem
						where !x.inTransfer
						select x).Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
					{
						FactionGoal_SendFleet factionGoal_SendFleet = x.AssignedGoal() as FactionGoal_SendFleet;
						return factionGoal_SendFleet == null || !(factionGoal_SendFleet.target().ref_system == primarySystem);
					})
					orderby x.CombatFleet() descending, !(x.AssignedGoal() is FactionGoal_DefendWithFleet) descending, x.AssignedGoal() == null descending, x.AssignedGoal() is FactionGoal_JoinFleet || x.AssignedGoal() is FactionGoal_AssembleFleet descending, !x.AI_NeedsRepairBadly() descending, !x.AI_NeedsRearmBadly() descending, !x.AI_NeedsRefuelBadly() descending
					select x).ThenBy<TISpaceFleetState, int>(delegate(TISpaceFleetState x)
				{
					FactionGoal_Fleet factionGoal_Fleet4 = x.AssignedGoal();
					if (factionGoal_Fleet4 == null)
					{
						return -1;
					}
					return factionGoal_Fleet4.importance;
				}).ThenByDescending<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue()).ToList<TISpaceFleetState>();
				if (tispaceFleetState2 != null)
				{
					using (List<TISpaceFleetState>.Enumerator enumerator = list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TISpaceFleetState tispaceFleetState5 = enumerator.Current;
							CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, tispaceFleetState2), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState5);
						}
						goto IL_061F;
					}
				}
				if (!flag)
				{
					foreach (TISpaceFleetState tispaceFleetState6 in list)
					{
						CS$<>8__locals1.faction.AddGoal(new FactionGoal_SendFleet(CS$<>8__locals1.faction, tiorbitState), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState6);
						if (!AIEvaluators.IsPrimarySystemInPeril(CS$<>8__locals1.faction))
						{
							break;
						}
					}
				}
			}
			IL_061F:
			if ((TITimeState.Now().day + AIDailyFactionPlanner.factionAIData[CS$<>8__locals1.faction].every4DaysOffset) % 4 != 0)
			{
				return;
			}
			foreach (TISpaceFleetState tispaceFleetState7 in CS$<>8__locals1.faction.fleets)
			{
				FactionGoal_Fleet factionGoal_Fleet2 = tispaceFleetState7.AssignedGoal();
				if (factionGoal_Fleet2 != null && factionGoal_Fleet2.ShouldPauseGoal())
				{
					factionGoal_Fleet2.UnassignFleet();
				}
			}
			List<TISpaceFleetState> list2 = CS$<>8__locals1.faction.fleets.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
			{
				if (x.transferAssigned)
				{
					return false;
				}
				if (x.AssignedGoal() == null)
				{
					return true;
				}
				if (CS$<>8__locals1.faction.fleetGoalTracker[x].GetGoalType() == GoalType.AssembleFleet)
				{
					List<GoalType> subsequentGoals = CS$<>8__locals1.faction.fleetGoalTracker[x].subsequentGoals;
					return subsequentGoals != null && subsequentGoals.Count == 0;
				}
				return false;
			}).ToList<TISpaceFleetState>();
			if (CS$<>8__locals1.faction.IsAlienFaction)
			{
				CS$<>8__locals1.genericAssembleLocation = CS$<>8__locals1.faction.primaryStation;
				if (CS$<>8__locals1.genericAssembleLocation == null)
				{
					CS$<>8__locals1.genericAssembleLocation = CS$<>8__locals1.faction.primaryHab.ref_spaceBody.interfaceOrbits.First<TIOrbitState>();
				}
			}
			else
			{
				CS$<>8__locals1.genericAssembleLocation = GameStateManager.Earth().interfaceOrbits.First<TIOrbitState>();
			}
			FactionGoal_AssembleFleet factionGoal_AssembleFleet = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.AssembleFleet, false, true)
				select x as FactionGoal_AssembleFleet into x
				where x.assemblyLocation == CS$<>8__locals1.genericAssembleLocation
				select x).FirstOrDefault<FactionGoal_AssembleFleet>();
			if (CS$<>8__locals1.faction.FuelEfficiencyMode() || CS$<>8__locals1.faction.HasUpkeepInsecurityInTheFuture())
			{
				if (factionGoal_AssembleFleet != null)
				{
					factionGoal_AssembleFleet.SetImportance(0);
				}
			}
			else if (factionGoal_AssembleFleet == null)
			{
				factionGoal_AssembleFleet = new FactionGoal_AssembleFleet(CS$<>8__locals1.faction, 1, CS$<>8__locals1.genericAssembleLocation, float.PositiveInfinity, false);
				factionGoal_AssembleFleet = CS$<>8__locals1.faction.AddGoal(factionGoal_AssembleFleet, HandleDuplicateGoalRule.ResetImportance, null) as FactionGoal_AssembleFleet;
			}
			foreach (TISpaceFleetState tispaceFleetState8 in (from x in CS$<>8__locals1.faction.fleets.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
				{
					TIOrbitState ref_orbit = x.ref_orbit;
					return ref_orbit != null && ref_orbit.isAdHocOrbit;
				})
				where x.AssignedGoal() == null || x.AssignedGoal() is FactionGoal_FixUpFleet
				select x).ToList<TISpaceFleetState>())
			{
				if (!CS$<>8__locals1.faction.IsAlienFaction && TIUtilities.RandomFloatValue() < 0.025f)
				{
					double num = (double)tispaceFleetState8.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.ScuttleCost().GetSingleCostValue(FactionResource.Boost));
					float currentResourceAmount = CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Boost);
					if (num < 0.025 * (double)currentResourceAmount)
					{
						CS$<>8__locals1.faction.playerControl.StartAction(new ScuttleShipsOperationAction(tispaceFleetState8, tispaceFleetState8.ships));
						continue;
					}
				}
				if (tispaceFleetState8.AssignedGoal() == null || TIUtilities.RandomFloatValue() < 0.1f)
				{
					IEnumerable<TIHabState> enumerable = from x in CS$<>8__locals1.faction.ResupplyHabs(false, false)
						where x.IsStation
						select x;
					if (!enumerable.Any<TIHabState>())
					{
						enumerable = CS$<>8__locals1.faction.ResupplyHabs(false, false);
						if (!enumerable.Any<TIHabState>())
						{
							enumerable = CS$<>8__locals1.faction.habs;
						}
					}
					enumerable = enumerable.Take_Random<TIHabState>(10).ToList<TIHabState>();
					List<TIOrbitState> list3 = (from x in enumerable.Select<TIHabState, TIOrbitState>(delegate(TIHabState x)
						{
							if (!x.IsStation)
							{
								return x.ref_spaceBody.orbits.FirstOrDefault<TIOrbitState>();
							}
							return x.ref_orbit;
						})
						where x != null
						select x).ToList<TIOrbitState>();
					TIOrbitState tiorbitState2 = null;
					float num2 = float.PositiveInfinity;
					foreach (TIOrbitState tiorbitState3 in list3)
					{
						Trajectory trajectory = null;
						TransferResult transferResult;
						double num3 = AIDailyFactionPlanner.SelectTrajectoryAsync(tispaceFleetState8, tiorbitState3, 0f, out transferResult, delegate(Trajectory x)
						{
							trajectory = x;
						}, false, 0.20000000298023224);
						if (trajectory != null && num3 < (double)tispaceFleetState8.currentDeltaV_kps && trajectory.duration_d < (double)num2)
						{
							tiorbitState2 = tiorbitState3;
							num2 = (float)trajectory.duration_d;
						}
					}
					if (tiorbitState2 != null)
					{
						CS$<>8__locals1.faction.AddGoal(new FactionGoal_SendFleet(CS$<>8__locals1.faction, tiorbitState2), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState8);
						list2.Remove(tispaceFleetState8);
					}
				}
			}
			List<FactionGoal_DefendWithFleet> list4 = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.DefendWithFleet, false, true)
				select x as FactionGoal_DefendWithFleet).ToList<FactionGoal_DefendWithFleet>();
			List<TISpaceFleetState> list5 = (from x in list4
				select x.assignedFleet into x
				where TIGameState.Valid(x)
				select x).ToList<TISpaceFleetState>();
			IEnumerable<TISpaceBodyState> enumerable2 = (from x in CS$<>8__locals1.faction.fleets
				select x.ref_system into x
				where x != null && !x.isSun
				select x).Distinct<TISpaceBodyState>();
			CS$<>8__locals1.defenseModeSystems = new HashSet<TISpaceBodyState>(enumerable2.Where<TISpaceBodyState>((TISpaceBodyState x) => AIEvaluators.ShouldSystemBeInDefenseMode(CS$<>8__locals1.faction, x)));
			foreach (TISpaceFleetState tispaceFleetState9 in list2.Union<TISpaceFleetState>(list5).ToList<TISpaceFleetState>())
			{
				if (!TIGameState.Valid(tispaceFleetState9))
				{
					list2.Remove(tispaceFleetState9);
				}
				else if (!CS$<>8__locals1.defenseModeSystems.Contains(tispaceFleetState9.ref_system))
				{
					FactionGoal_Fleet factionGoal_Fleet3 = tispaceFleetState9.AssignedGoal();
					if (factionGoal_Fleet3 == null || !factionGoal_Fleet3.LeaveMyFleetAlone())
					{
						if (!(tispaceFleetState9.AssignedGoal() is FactionGoal_DefendWithFleet))
						{
							if (tispaceFleetState9.NeedsRepair())
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_RepairFleet(CS$<>8__locals1.faction, tispaceFleetState9, null), HandleDuplicateGoalRule.Ignore, tispaceFleetState9);
								list2.Remove(tispaceFleetState9);
								continue;
							}
							if (tispaceFleetState9.ships.Any<TISpaceShipState>((TISpaceShipState x) => x.propellant_tons < x.template.propellantMass_tons * 0.8f) || tispaceFleetState9.NeedsRearm())
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_ResupplyFleet(CS$<>8__locals1.faction, tispaceFleetState9, null), HandleDuplicateGoalRule.Ignore, tispaceFleetState9);
								list2.Remove(tispaceFleetState9);
								continue;
							}
						}
						if (TIUtilities.RandomFloatValue() < 0.3f && tispaceFleetState9.NeedsRefit())
						{
							TIResourcesCost tiresourcesCost = new TIResourcesCost();
							foreach (KeyValuePair<TISpaceShipState, TISpaceShipTemplate> keyValuePair in tispaceFleetState9.RefitsAvailable)
							{
								tiresourcesCost.SumCosts_NoDuration(keyValuePair.Value.RefitResourceCost(null, keyValuePair.Key.template, true, true, keyValuePair.Key));
							}
							int num4 = CS$<>8__locals1.faction.GoalsOfType(GoalType.RefitFleet, false, true).SelectMany<TIFactionGoalState, TISpaceShipState>((TIFactionGoalState x) => (x as FactionGoal_RefitFleet).assignedFleet.ships).Count<TISpaceShipState>();
							tiresourcesCost = tiresourcesCost.MultiplyCost(5f + (float)num4 / (float)tispaceFleetState9.ships.Count);
							if (tiresourcesCost.CanAfford(CS$<>8__locals1.faction, 1f, null, float.PositiveInfinity))
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_RefitFleet(CS$<>8__locals1.faction, tispaceFleetState9, null), HandleDuplicateGoalRule.Ignore, tispaceFleetState9);
							}
						}
					}
				}
			}
			float staticFleetFraction = CS$<>8__locals1.faction.GetStaticFleetFraction();
			float desiredStaticFleetFraction = CS$<>8__locals1.faction.GetDesiredStaticFleetFraction();
			if (staticFleetFraction < desiredStaticFleetFraction)
			{
				IEnumerable<FactionGoal_DefendWithFleet> enumerable3 = list4.Where<FactionGoal_DefendWithFleet>((FactionGoal_DefendWithFleet x) => x.MayIncreaseFleetSize());
				using (List<TISpaceFleetState>.Enumerator enumerator = list2.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ref_system != null).ToList<TISpaceFleetState>().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						TISpaceFleetState fleet2 = enumerator.Current;
						FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = (from x in enumerable3.Where<FactionGoal_DefendWithFleet>(delegate(FactionGoal_DefendWithFleet x)
							{
								TIGameState tigameState = x.target();
								return ((tigameState != null) ? tigameState.ref_system : null) == fleet2.ref_system;
							})
							where x.assignedFleet == null || x.assignedFleet.ref_system == fleet2.ref_system
							orderby x.EarmarkedFleetMC > 0 descending
							select x).FirstOrDefault<FactionGoal_DefendWithFleet>() ?? enumerable3.FirstOrDefault<FactionGoal_DefendWithFleet>();
						if (factionGoal_DefendWithFleet == null)
						{
							break;
						}
						if (fleet2.CanFulfillGoal(factionGoal_DefendWithFleet, false) && factionGoal_DefendWithFleet.learnedPerformanceRequirements.MeetsRequirements(fleet2, null))
						{
							if (TIGameState.Valid(factionGoal_DefendWithFleet.assignedFleet))
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, factionGoal_DefendWithFleet.assignedFleet), HandleDuplicateGoalRule.ResetImportance, fleet2);
							}
							else
							{
								factionGoal_DefendWithFleet.AssignFleet(fleet2);
							}
							list2.Remove(fleet2);
							if (CS$<>8__locals1.faction.GetStaticFleetFraction() >= desiredStaticFleetFraction)
							{
								break;
							}
						}
					}
				}
			}
			using (List<FactionGoal_Fleet>.Enumerator enumerator5 = (from x in CS$<>8__locals1.faction.AllFleetGoals(true).Where<FactionGoal_Fleet>(delegate(FactionGoal_Fleet goal)
				{
					if (goal.target() == null)
					{
						return false;
					}
					FactionGoal_AssembleFleet factionGoal_AssembleFleet2 = goal as FactionGoal_AssembleFleet;
					if (factionGoal_AssembleFleet2 != null && factionGoal_AssembleFleet2.constructionOnly)
					{
						return false;
					}
					FactionGoal_TransportCouncilorsWithFleet transportGoal = goal as FactionGoal_TransportCouncilorsWithFleet;
					if (transportGoal != null)
					{
						if (transportGoal.assignedCouncilors.Count == 0)
						{
							return false;
						}
						if (transportGoal.assignedFleet != null && transportGoal.assignedCouncilors.Any<TICouncilorState>((TICouncilorState x) => x.ref_fleet == transportGoal.assignedFleet))
						{
							return false;
						}
					}
					return (!TIGameState.Valid(goal.assignedFleet) || (!goal.assignedFleet.inTransfer && !goal.ReadyForTransferToTarget(goal.assignedFleet))) && !goal.ShouldPauseGoal() && !goal.LeaveMyFleetAlone();
				})
				orderby x.importance descending
				select x).ToList<FactionGoal_Fleet>().GetEnumerator())
			{
				while (enumerator5.MoveNext())
				{
					AIDailyFactionPlanner.<>c__DisplayClass42_5 CS$<>8__locals5 = new AIDailyFactionPlanner.<>c__DisplayClass42_5();
					CS$<>8__locals5.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals5.factionGoal = enumerator5.Current;
					bool flag2 = CS$<>8__locals5.factionGoal.target().isSpaceFleetState && CS$<>8__locals5.factionGoal.target().ref_fleet.inTransfer;
					List<ShipRole> allowedRolesForGoal = CS$<>8__locals5.factionGoal.allRoles;
					List<TISpaceFleetState> list6 = new List<TISpaceFleetState>();
					if (CS$<>8__locals5.factionGoal.importance == 20 && (!CS$<>8__locals5.factionGoal.buildFleetsSequentially || CS$<>8__locals5.factionGoal.IsFrontGoal))
					{
						list6 = CS$<>8__locals5.CS$<>8__locals1.faction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => !x.transferAssigned && x.CanFulfillGoal(CS$<>8__locals5.factionGoal, true)).ToList<TISpaceFleetState>();
					}
					else
					{
						list6 = list2.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
						{
							if (x.CanFulfillGoal(CS$<>8__locals5.factionGoal, false))
							{
								return allowedRolesForGoal.Intersect<ShipRole>(x.ships.Select<TISpaceShipState, ShipRole>((TISpaceShipState y) => y.role)).Count<ShipRole>() > 0;
							}
							return false;
						}).ToList<TISpaceFleetState>();
					}
					IEnumerable<TISpaceFleetState> enumerable4 = list6.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ref_system != null && !x.ref_system.isSun);
					Func<TISpaceFleetState, bool> func;
					if ((func = CS$<>8__locals5.CS$<>8__locals1.<>9__56) == null)
					{
						func = (CS$<>8__locals5.CS$<>8__locals1.<>9__56 = (TISpaceFleetState x) => !CS$<>8__locals5.CS$<>8__locals1.defenseModeSystems.Contains(x.ref_system));
					}
					list6 = enumerable4.Where<TISpaceFleetState>(func).ToList<TISpaceFleetState>();
					if (!CS$<>8__locals5.factionGoal.LookingForFleet())
					{
						list6 = list6.Where<TISpaceFleetState>((TISpaceFleetState x) => CS$<>8__locals5.factionGoal.ReadyForTransferToTarget(x)).ToList<TISpaceFleetState>();
					}
					if (CS$<>8__locals5.factionGoal.SpaceCombatGoal() && CS$<>8__locals5.factionGoal.importance < 20)
					{
						list6 = list6.Where<TISpaceFleetState>((TISpaceFleetState x) => !x.NonCombatFleet() && !x.InvasionFleet()).ToList<TISpaceFleetState>();
					}
					list6 = (from x in list6
						where x.AssignedGoal() == null || x.AssignedGoal().importance < CS$<>8__locals5.factionGoal.importance
						where x.ships.None<TISpaceShipState>((TISpaceShipState x) => x.badlyDamaged || x.AI_InvoluntaryNoncombatant())
						select x).ToList<TISpaceFleetState>();
					if ((list6 = list6.Where<TISpaceFleetState>((TISpaceFleetState x) => base.<ManageFleets>g__GetValidShips|61(x).Any<TISpaceShipState>()).ToList<TISpaceFleetState>()).Count != 0)
					{
						FactionGoal_AttackWithFleet attackGoal = CS$<>8__locals5.factionGoal as FactionGoal_AttackWithFleet;
						if (attackGoal != null && attackGoal.bombardmentGoal)
						{
							float desiredBombardmentValue = attackGoal.GetDesiredBombardmentValue();
							IEnumerable<TISpaceFleetState> enumerable5 = list6.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
							{
								TIGameState tigameState2 = attackGoal.target();
								return x.BombardmentValue((tigameState2 != null) ? tigameState2.ref_spaceBody : null) >= desiredBombardmentValue;
							});
							if (enumerable5.Any<TISpaceFleetState>())
							{
								list6 = enumerable5.ToList<TISpaceFleetState>();
							}
							IEnumerable<TISpaceFleetState> enumerable6 = list6.Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
							{
								TIGameState tigameState3 = attackGoal.target();
								return x.BombardmentValue((tigameState3 != null) ? tigameState3.ref_spaceBody : null) >= 2f * desiredBombardmentValue;
							});
							if (enumerable6.Any<TISpaceFleetState>())
							{
								list6 = enumerable6.ToList<TISpaceFleetState>();
							}
						}
						IEnumerable<TISpaceFleetState> enumerable7 = list6.Where<TISpaceFleetState>((TISpaceFleetState x) => base.<ManageFleets>g__GetValidShips|61(x).Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)) >= CS$<>8__locals5.factionGoal.desiredFleetCombatValue);
						if (enumerable7.Any<TISpaceFleetState>())
						{
							list6 = enumerable7.ToList<TISpaceFleetState>();
						}
						IEnumerable<TISpaceFleetState> enumerable8 = list6.Where<TISpaceFleetState>((TISpaceFleetState x) => base.<ManageFleets>g__GetValidShips|61(x).Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)) >= 1.5f * CS$<>8__locals5.factionGoal.desiredFleetCombatValue);
						if (!flag2 && enumerable8.Count<TISpaceFleetState>() >= 3)
						{
							list6 = enumerable8.ToList<TISpaceFleetState>();
						}
						if (TIGameState.Valid(CS$<>8__locals5.factionGoal.assignedFleet))
						{
							list6.Add(CS$<>8__locals5.factionGoal.assignedFleet);
						}
						float exampleDV_mps = list6.Max<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Max<TISpaceShipState>((TISpaceShipState y) => 1000f * y.currentMaxDeltaV_kps));
						float exampleAcceleration_mps2 = list6.Max<TISpaceFleetState>((TISpaceFleetState x) => x.ships.Max<TISpaceShipState>((TISpaceShipState y) => y.cruiseAcceleration_mps2));
						float failureTransferTime_days = 3652.4219f;
						Dictionary<TISpaceBodyState, float> exampleTransferTimes_days = (from x in list6
							group x by x.ref_system).ToDictionary<IGrouping<TISpaceBodyState, TISpaceFleetState>, TISpaceBodyState, float>((IGrouping<TISpaceBodyState, TISpaceFleetState> x) => x.Key, delegate(IGrouping<TISpaceBodyState, TISpaceFleetState> group)
						{
							TIOrbitState tiorbitState4 = group.Key.orbits.FirstOrDefault<TIOrbitState>();
							TIGameState tigameState4 = CS$<>8__locals5.factionGoal.target();
							FactionGoal_TransportCouncilorsWithFleet factionGoal_TransportCouncilorsWithFleet = CS$<>8__locals5.factionGoal as FactionGoal_TransportCouncilorsWithFleet;
							if (factionGoal_TransportCouncilorsWithFleet != null)
							{
								tigameState4 = factionGoal_TransportCouncilorsWithFleet.assignedCouncilors.FirstOrDefault<TICouncilorState>();
							}
							if (tigameState4.ref_orbit != null && (!tigameState4.isSpaceFleetState || !tigameState4.ref_fleet.landed))
							{
								tigameState4 = tigameState4.ref_orbit;
							}
							else if (tigameState4.ref_system != null && !tigameState4.ref_system.isSun)
							{
								tigameState4 = tigameState4.ref_system.orbits.FirstOrDefault<TIOrbitState>();
							}
							else
							{
								tigameState4 = null;
							}
							if (tiorbitState4 == null || tigameState4 == null)
							{
								return failureTransferTime_days;
							}
							if (tiorbitState4 == tigameState4)
							{
								return 0f;
							}
							return AIEvaluators.GetEstimatedTransferTime_days(CS$<>8__locals5.CS$<>8__locals1.faction, tiorbitState4, tigameState4, exampleAcceleration_mps2, exampleDV_mps, failureTransferTime_days);
						});
						TISpaceFleetState key = (from x in list6.ToDictionary<TISpaceFleetState, TISpaceFleetState, float>((TISpaceFleetState x) => x, delegate(TISpaceFleetState x)
							{
								float num14 = 1f / Mathf.Max(exampleTransferTimes_days[x.ref_system], 0.001f);
								if (CS$<>8__locals5.factionGoal.ReadyForTransferToTarget(x))
								{
									num14 *= 10f;
								}
								return num14;
							})
							orderby x.Value descending, x.Key == CS$<>8__locals5.factionGoal.assignedFleet descending
							select x).FirstOrDefault<KeyValuePair<TISpaceFleetState, float>>().Key;
						if (key != null && key != CS$<>8__locals5.factionGoal.assignedFleet)
						{
							CS$<>8__locals5.factionGoal.AssignFleet(key);
							list2.Remove(key);
						}
					}
				}
			}
			foreach (TISpaceFleetState tispaceFleetState10 in list2.ToList<TISpaceFleetState>())
			{
				if (!CS$<>8__locals1.defenseModeSystems.Contains(tispaceFleetState10.ref_system) && tispaceFleetState10.AssignedGoal() == null)
				{
					if (tispaceFleetState10.InvasionFleet())
					{
						List<FactionGoal_InvadeEarth> list7 = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.InvadeEarth, false, true)
							orderby x.assignedDate
							select x as FactionGoal_InvadeEarth).ToList<FactionGoal_InvadeEarth>();
						if (list7.Count != 0)
						{
							TISpaceObjectState getSunOrbitingRelatedObject = tispaceFleetState10.GetSunOrbitingRelatedObject;
							double? num5 = ((getSunOrbitingRelatedObject != null) ? new double?(getSunOrbitingRelatedObject.semiMajorAxis_AU) : null);
							double num6 = GameStateManager.Saturn().semiMajorAxis_AU;
							if (!((num5.GetValueOrDefault() <= num6) & (num5 != null)))
							{
								if (tispaceFleetState10.inTransfer)
								{
									Trajectory trajectory2 = tispaceFleetState10.trajectory;
									double? num7;
									if (trajectory2 == null)
									{
										num7 = null;
									}
									else
									{
										TIOrbitState destinationOrbit = trajectory2.destinationOrbit;
										if (destinationOrbit == null)
										{
											num7 = null;
										}
										else
										{
											TISpaceObjectState getSunOrbitingRelatedObject2 = destinationOrbit.ref_naturalSpaceObject.GetSunOrbitingRelatedObject;
											num7 = ((getSunOrbitingRelatedObject2 != null) ? new double?(getSunOrbitingRelatedObject2.semiMajorAxis_AU) : null);
										}
									}
									num5 = num7;
									num6 = GameStateManager.Saturn().semiMajorAxis_AU;
									if ((num5.GetValueOrDefault() <= num6) & (num5 != null))
									{
										goto IL_16A2;
									}
								}
								FactionGoal_InvadeEarth factionGoal_InvadeEarth = list7.FirstOrDefault<FactionGoal_InvadeEarth>((FactionGoal_InvadeEarth x) => TIGameState.Valid(x.assignedFleet));
								TISpaceFleetState tispaceFleetState11 = ((factionGoal_InvadeEarth != null) ? factionGoal_InvadeEarth.assignedFleet : null);
								if (tispaceFleetState11 != null)
								{
									CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, tispaceFleetState11), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState10);
									list2.Remove(tispaceFleetState10);
									continue;
								}
								continue;
							}
						}
						IL_16A2:
						CS$<>8__locals1.faction.AddGoal(new FactionGoal_InvadeEarth(CS$<>8__locals1.faction, 15), HandleDuplicateGoalRule.Ignore, tispaceFleetState10);
						list2.Remove(tispaceFleetState10);
					}
					else if (tispaceFleetState10.SurveillanceFleet())
					{
						List<FactionGoal_SurveilEarth> list8 = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.SurveilEarth, false, true)
							select x as FactionGoal_SurveilEarth).ToList<FactionGoal_SurveilEarth>();
						if (list8.Count < 2)
						{
							CS$<>8__locals1.faction.AddGoal(new FactionGoal_SurveilEarth(CS$<>8__locals1.faction, 15), HandleDuplicateGoalRule.Ignore, tispaceFleetState10);
							list2.Remove(tispaceFleetState10);
						}
						else
						{
							FactionGoal_SurveilEarth factionGoal_SurveilEarth = list8.FirstOrDefault<FactionGoal_SurveilEarth>((FactionGoal_SurveilEarth x) => TIGameState.Valid(x.assignedFleet));
							TISpaceFleetState tispaceFleetState12 = ((factionGoal_SurveilEarth != null) ? factionGoal_SurveilEarth.assignedFleet : null);
							if (tispaceFleetState12 != null)
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, tispaceFleetState12), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState10);
								list2.Remove(tispaceFleetState10);
							}
						}
					}
				}
			}
			Dictionary<TISpaceBodyState, List<TISpaceFleetState>> dictionary = (from x in CS$<>8__locals1.faction.fleets
				where x.ref_system != null && x.ref_system != GameStateManager.Sol()
				group x by x.ref_system).ToDictionary<IGrouping<TISpaceBodyState, TISpaceFleetState>, TISpaceBodyState, List<TISpaceFleetState>>((IGrouping<TISpaceBodyState, TISpaceFleetState> x) => x.Key, (IGrouping<TISpaceBodyState, TISpaceFleetState> x) => x.ToList<TISpaceFleetState>());
			using (Dictionary<TISpaceBodyState, List<TISpaceFleetState>>.KeyCollection.Enumerator enumerator6 = dictionary.Keys.GetEnumerator())
			{
				while (enumerator6.MoveNext())
				{
					TISpaceBodyState system2 = enumerator6.Current;
					List<TISpaceFleetState> list9 = (from x in CS$<>8__locals1.faction.fleets.Where<TISpaceFleetState>((TISpaceFleetState x) => x.ref_system == system2).ToList<TISpaceFleetState>()
						where !x.landed
						select x).ToList<TISpaceFleetState>();
					bool flag3 = list9.Where<TISpaceFleetState>((TISpaceFleetState x) => x.AssignedGoal() != null).Any<TISpaceFleetState>((TISpaceFleetState x) => x.AssignedGoal().GetGoalType() == GoalType.DefendWithFleet);
					bool flag4 = AIEvaluators.ShouldSystemBeInDefenseMode(CS$<>8__locals1.faction, system2);
					if (!flag3 && flag4)
					{
						FactionGoal_DefendWithFleet defenseGoal = (from x in CS$<>8__locals1.faction.GoalsOfType(GoalType.DefendWithFleet, false, true).Where<TIFactionGoalState>(delegate(TIFactionGoalState x)
							{
								TIGameState tigameState5 = x.target();
								return ((tigameState5 != null) ? tigameState5.ref_system : null) == system2;
							})
							orderby x.target().ref_hab != null descending
							select x).FirstOrDefault<TIFactionGoalState>() as FactionGoal_DefendWithFleet;
						if (defenseGoal != null)
						{
							TISpaceFleetState tispaceFleetState13 = (from x in list9
								where x.AssignedGoal() == null || x.AssignedGoal() is FactionGoal_JoinFleet
								where x.CanFulfillGoal(defenseGoal, false)
								select x).MaxBy<TISpaceFleetState, float>((TISpaceFleetState x) => x.SpaceCombatValue());
							if (tispaceFleetState13 != null)
							{
								defenseGoal.AssignFleet(tispaceFleetState13);
								flag3 = true;
							}
						}
					}
					bool flag5 = flag3 && flag4;
					List<TISpaceFleetState> list10 = (from x in (from x in CS$<>8__locals1.faction.fleets
							where x.AssignedGoal() != null
							where x.AssignedGoal().GetGoalType() == GoalType.DefendWithFleet || x.AssignedGoal().GetGoalType() == GoalType.AttackWithFleet
							where !x.transferAssigned && x.isCapableOfTransfering
							select x).Where<TISpaceFleetState>(delegate(TISpaceFleetState x)
						{
							FactionGoal_Fleet factionGoal_Fleet5 = x.AssignedGoal();
							return factionGoal_Fleet5 != null && !factionGoal_Fleet5.ShouldDiscardGoal();
						})
						where x.ref_system == system2
						where x.AssignedGoal().MayIncreaseFleetSize()
						select x).OrderByDescending<TISpaceFleetState, int>(new Func<TISpaceFleetState, int>(AIDailyFactionPlanner.<ManageFleets>g__DefenseSort|42_92)).ThenBy<TISpaceFleetState, bool>((TISpaceFleetState x) => x.AssignedGoal().HasEnoughSpaceCombatValue(x)).ThenByDescending<TISpaceFleetState, int>((TISpaceFleetState x) => x.AssignedGoal().importance)
						.ThenBy<TISpaceFleetState, TIDateTime>((TISpaceFleetState x) => x.AssignedGoal().assignedDate)
						.ToList<TISpaceFleetState>();
					TISpaceFleetState mergeTarget = list10.FirstOrDefault<TISpaceFleetState>();
					if (!(mergeTarget == null))
					{
						FactionGoal_Fleet mergeTargetGoal = mergeTarget.AssignedGoal();
						IEnumerable<TISpaceFleetState> enumerable9 = dictionary[system2].Where<TISpaceFleetState>(delegate(TISpaceFleetState candidateJoiner)
						{
							FactionGoal_Fleet factionGoal_Fleet6 = candidateJoiner.AssignedGoal();
							GoalType goalType = ((factionGoal_Fleet6 != null) ? factionGoal_Fleet6.GetGoalType() : GoalType.None);
							if (factionGoal_Fleet6 != null && factionGoal_Fleet6.LeaveMyFleetAlone())
							{
								return false;
							}
							if (goalType == GoalType.SecureEarthSpace)
							{
								return false;
							}
							if (!candidateJoiner.transferAssigned && candidateJoiner.isCapableOfTransfering)
							{
								if (!candidateJoiner.ships.All<TISpaceShipState>((TISpaceShipState x) => x.badlyDamaged || x.AI_InvoluntaryNoncombatant()))
								{
									if (factionGoal_Fleet6 != null)
									{
										FactionGoal_JoinFleet factionGoal_JoinFleet = factionGoal_Fleet6 as FactionGoal_JoinFleet;
										if (factionGoal_JoinFleet != null && factionGoal_JoinFleet.target() == mergeTarget)
										{
											return false;
										}
									}
									if (factionGoal_Fleet6 != null)
									{
										FactionGoal_AssembleFleet factionGoal_AssembleFleet3 = factionGoal_Fleet6 as FactionGoal_AssembleFleet;
										if (factionGoal_AssembleFleet3 != null && factionGoal_AssembleFleet3.constructionOnly)
										{
											return true;
										}
									}
									if (!(factionGoal_Fleet6 == null))
									{
										FactionGoal_FixUpFleet factionGoal_FixUpFleet = factionGoal_Fleet6 as FactionGoal_FixUpFleet;
										if (factionGoal_FixUpFleet != null)
										{
											return candidateJoiner.ref_system == factionGoal_FixUpFleet.destination.ref_system;
										}
									}
									return true;
								}
							}
							return false;
						});
						if (!flag5)
						{
							enumerable9 = from x in enumerable9
								where x.AssignedGoal() is FactionGoal_AttackWithFleet
								where !x.AssignedGoal().HasEnoughSpaceCombatValue(x) || !(x.AssignedGoal() as FactionGoal_AttackWithFleet).HasEnoughBombardmentValue(x)
								select x;
						}
						Func<TISpaceShipState, bool> <>9__120;
						foreach (TISpaceFleetState tispaceFleetState14 in enumerable9.ToList<TISpaceFleetState>())
						{
							if (!(mergeTarget == tispaceFleetState14))
							{
								IEnumerable<TISpaceShipState> enumerable10 = from x in tispaceFleetState14.ships
									where TIShipPartTemplate.PrimaryRoleModules.Intersect<SpecialModuleRule>(x.SpecialModuleRules(true)).Count<SpecialModuleRule>() == 0
									where x.combatant
									select x;
								Func<TISpaceShipState, bool> func2;
								if ((func2 = <>9__120) == null)
								{
									func2 = (<>9__120 = (TISpaceShipState x) => mergeTargetGoal == null || mergeTargetGoal.learnedPerformanceRequirements.MeetsRequirements(x, null));
								}
								List<TISpaceShipState> list11 = enumerable10.Where<TISpaceShipState>(func2).ToList<TISpaceShipState>();
								if (list11.Count != 0)
								{
									if (list11.Count != tispaceFleetState14.ships.Count)
									{
										tispaceFleetState14 = SplitFleetOperation.BuildFleetFromSelectedTargets(tispaceFleetState14, list11, null);
									}
									CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, mergeTarget), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState14);
								}
							}
						}
						IEnumerable<TISpaceFleetState> enumerable11 = from x in CS$<>8__locals1.faction.fleets
							where x.ref_system == system2
							where x.currentOperations.Count == 0
							select x;
						Func<TISpaceFleetState, bool> func3;
						if ((func3 = CS$<>8__locals1.<>9__107) == null)
						{
							func3 = (CS$<>8__locals1.<>9__107 = (TISpaceFleetState x) => !x.dockedAtHab || !x.ref_hab.AllowsResupply(CS$<>8__locals1.faction, false, false));
						}
						List<TISpaceFleetState> list12 = (from x in enumerable11.Where<TISpaceFleetState>(func3)
							where x.isCapableOfTransfering
							where x.currentDeltaV_kps > 10f
							where x.AssignedGoal() != null && x.AssignedGoal().GetGoalType() == GoalType.ResupplyFleet && !x.AssignedGoal().skipGoal
							where x.currentOperations.Count == 0
							select x).ToList<TISpaceFleetState>();
						TISpaceFleetState tispaceFleetState15 = list12.FirstOrDefault<TISpaceFleetState>();
						foreach (TISpaceFleetState tispaceFleetState16 in list12)
						{
							if (!(tispaceFleetState16 == tispaceFleetState15))
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, tispaceFleetState15), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState16);
							}
						}
					}
				}
			}
			if (CS$<>8__locals1.faction.IsAlienFaction)
			{
				int? num8;
				if (factionGoal_AssembleFleet == null)
				{
					num8 = null;
				}
				else
				{
					TISpaceFleetState assignedFleet = factionGoal_AssembleFleet.assignedFleet;
					num8 = ((assignedFleet != null) ? new int?(assignedFleet.ships.Count) : null);
				}
				int? num9 = num8;
				if (num9.GetValueOrDefault() >= 10)
				{
					List<FactionGoal_DefendWithFleet> list13 = AIEvaluators.GetBossDefenseGoals(CS$<>8__locals1.faction).ToList<FactionGoal_DefendWithFleet>();
					float typicalShipStrength = CS$<>8__locals1.faction.GetTypicalShipSpaceCombatValue();
					float typicalShipMC = CS$<>8__locals1.faction.GetTypicalShipMissionControlConsumption();
					Dictionary<FactionGoal_DefendWithFleet, int> dictionary2 = list13.ToDictionary<FactionGoal_DefendWithFleet, FactionGoal_DefendWithFleet, int>((FactionGoal_DefendWithFleet x) => x, delegate(FactionGoal_DefendWithFleet x)
					{
						IEnumerable<TISpaceShipTemplate> enumerable14 = x.pendingFleets.SelectMany<TISpaceFleetState, TISpaceShipTemplate>((TISpaceFleetState x) => x.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState y) => y.template)).Concat<TISpaceShipTemplate>(from x in x.PendingShips()
							where x.costPaid
							select x.shipDesign);
						if (x.assignedFleet != null)
						{
							enumerable14 = enumerable14.Concat<TISpaceShipTemplate>(x.assignedFleet.ships.Select<TISpaceShipState, TISpaceShipTemplate>((TISpaceShipState x) => x.template));
						}
						return (enumerable14.Sum<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false)) / typicalShipStrength / typicalShipMC).RoundUp();
					});
					FactionGoal_DefendWithFleet factionGoal_DefendWithFleet2 = (from x in dictionary2
						where x.Value < x.Key.EarmarkedFleetMC
						orderby x.Key.EarmarkedFleetMC - x.Value descending
						select x).FirstOrDefault<KeyValuePair<FactionGoal_DefendWithFleet, int>>().Key ?? AIEvaluators.GetNextBossDefenseGoalToFortify(CS$<>8__locals1.faction, list13);
					if (factionGoal_DefendWithFleet2 != null)
					{
						TISpaceFleetState tispaceFleetState17 = factionGoal_AssembleFleet.assignedFleet;
						int num10 = 4;
						if (tispaceFleetState17.ships.Count > num10)
						{
							tispaceFleetState17 = SplitFleetOperation.BuildFleetFromSelectedTargets(tispaceFleetState17, tispaceFleetState17.ships.Take_Random<TISpaceShipState>(num10).ToList<TISpaceShipState>(), null);
						}
						int num11 = dictionary2[factionGoal_DefendWithFleet2];
						int num12 = tispaceFleetState17.RawMissionControlConsumption();
						factionGoal_DefendWithFleet2.EarmarkedFleetMC = Mathf.Max(factionGoal_DefendWithFleet2.EarmarkedFleetMC, Mathf.Min(num11 + num12, factionGoal_DefendWithFleet2.EarmarkedFleetMC + num12));
						if (factionGoal_DefendWithFleet2.assignedFleet == null)
						{
							factionGoal_DefendWithFleet2.AssignFleet(tispaceFleetState17);
						}
						else
						{
							CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, factionGoal_DefendWithFleet2.assignedFleet), HandleDuplicateGoalRule.ResetImportance, tispaceFleetState17);
						}
					}
				}
			}
			using (List<TISpaceFleetState>.Enumerator enumerator = list2.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TISpaceFleetState fleet = enumerator.Current;
					if (fleet.ships.All<TISpaceShipState>((TISpaceShipState x) => x.isCapableOfTransfering))
					{
						TISpaceBodyState system = fleet.ref_system;
						IEnumerable<ShipRole> enumerable12 = fleet.ships.Select<TISpaceShipState, ShipRole>((TISpaceShipState x) => x.role).Distinct<ShipRole>();
						int num13 = enumerable12.Count<ShipRole>();
						IEnumerable<TISpaceFleetState> enumerable13 = CS$<>8__locals1.faction.fleets.Where<TISpaceFleetState>(delegate(TISpaceFleetState otherFleet)
						{
							if (otherFleet.transferAssigned || otherFleet.ref_system != system || otherFleet == fleet)
							{
								return false;
							}
							FactionGoal_Fleet factionGoal_Fleet7 = otherFleet.AssignedGoal();
							return factionGoal_Fleet7 == null || factionGoal_Fleet7.MayIncreaseFleetSize();
						});
						enumerable13 = enumerable13.OrderByDescending<TISpaceFleetState, bool>((TISpaceFleetState x) => x.AssignedGoal() != null).ThenBy<TISpaceFleetState, bool?>(delegate(TISpaceFleetState x)
						{
							FactionGoal_Fleet factionGoal_Fleet8 = x.AssignedGoal();
							if (factionGoal_Fleet8 == null)
							{
								return null;
							}
							return new bool?(factionGoal_Fleet8.HasEnoughSpaceCombatValue(x));
						}).ThenByDescending<TISpaceFleetState, int>(delegate(TISpaceFleetState x)
						{
							FactionGoal_Fleet factionGoal_Fleet9 = x.AssignedGoal();
							if (factionGoal_Fleet9 == null)
							{
								return 0;
							}
							return factionGoal_Fleet9.importance;
						})
							.ThenBy<TISpaceFleetState, TIDateTime>(delegate(TISpaceFleetState x)
							{
								FactionGoal_Fleet factionGoal_Fleet10 = x.AssignedGoal();
								return ((factionGoal_Fleet10 != null) ? factionGoal_Fleet10.assignedDate : null) ?? new TIDateTime();
							})
							.ToList<TISpaceFleetState>();
						if (fleet.CombatFleet())
						{
							enumerable13 = enumerable13.Where<TISpaceFleetState>((TISpaceFleetState x) => x.CombatFleet()).ToList<TISpaceFleetState>();
						}
						else
						{
							enumerable13 = enumerable13.Where<TISpaceFleetState>((TISpaceFleetState x) => x.NonCombatFleet()).ToList<TISpaceFleetState>();
						}
						bool flag6 = false;
						foreach (TISpaceFleetState tispaceFleetState18 in enumerable13)
						{
							if (!tispaceFleetState18.ships.Any<TISpaceShipState>((TISpaceShipState x) => !x.isCapableOfTransfering) && tispaceFleetState18.AssignedGoal() != null && tispaceFleetState18.AssignedGoal().GetGoalType() != GoalType.JoinFleet && CS$<>8__locals1.faction.fleetGoalTracker[tispaceFleetState18].allRoles.Intersect<ShipRole>(enumerable12).Count<ShipRole>() == num13 && tispaceFleetState18.AssignedGoal().learnedPerformanceRequirements.MeetsRequirements(fleet, null))
							{
								CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, tispaceFleetState18), HandleDuplicateGoalRule.ResetImportance, fleet);
								flag6 = true;
								break;
							}
						}
						if (!flag6)
						{
							foreach (TISpaceFleetState tispaceFleetState19 in enumerable13)
							{
								if (!tispaceFleetState19.ships.Any<TISpaceShipState>((TISpaceShipState x) => !x.isCapableOfTransfering) && tispaceFleetState19.AssignedGoal() == null && !tispaceFleetState19.AI_NeedsRefuelBadly())
								{
									CS$<>8__locals1.faction.AddGoal(new FactionGoal_JoinFleet(CS$<>8__locals1.faction, tispaceFleetState19), HandleDuplicateGoalRule.ResetImportance, fleet);
									flag6 = true;
									break;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005A38 RID: 23096 RVA: 0x002A48C4 File Offset: 0x002A2AC4
		public static void ResolveGoals(TIFactionState faction)
		{
			List<TIFactionGoalState> list = new List<TIFactionGoalState>();
			List<TIFactionGoalState> list2 = new List<TIFactionGoalState>();
			foreach (List<TIFactionGoalState> list3 in faction.factionGoals.Values)
			{
				foreach (TIFactionGoalState tifactionGoalState in list3)
				{
					if (tifactionGoalState.GoalFulfilled())
					{
						list2.Add(tifactionGoalState);
					}
					else if (tifactionGoalState.ShouldDiscardGoal())
					{
						list.Add(tifactionGoalState);
					}
				}
			}
			foreach (TIFactionGoalState tifactionGoalState2 in list)
			{
				tifactionGoalState2.OnGoalDiscarded();
				faction.RemoveGoal(tifactionGoalState2);
			}
			foreach (TIFactionGoalState tifactionGoalState3 in list2)
			{
				tifactionGoalState3.OnGoalComplete();
			}
		}

		// Token: 0x06005A39 RID: 23097 RVA: 0x002A49FC File Offset: 0x002A2BFC
		private void ReviewAndSetGoals(TIFactionState faction)
		{
			AIDailyFactionPlanner.ResolveGoals(faction);
			foreach (TIFactionGoalState tifactionGoalState in (from x in faction.factionGoals.Values.SelectMany<List<TIFactionGoalState>, TIFactionGoalState>((List<TIFactionGoalState> x) => x)
				orderby x.importance descending
				select x).ToList<TIFactionGoalState>())
			{
				tifactionGoalState.DailyGoalMaintenance();
			}
			this.ManageObjectiveGoals(faction);
			this.ManageWarsWithFaction(faction);
			if (!faction.IsAlienFaction)
			{
				if (this.gameTime.currentTime.day % 14 == AIDailyFactionPlanner.factionAIData[faction].every14DaysOffsetLate && this.gameTime.currentTime.day <= 28)
				{
					HabPlanner.HumanHabPlanner.ManageHabGoals(faction);
					HabPlanner.HumanHabPlanner.FoundHabs(faction);
				}
			}
			else if (this.gameTime.currentTime.day == 1)
			{
				this.AliensCheckGoals(faction);
			}
			AIDailyFactionPlanner.ManageFleetGoals(faction);
			this.ManageFleets(faction);
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x002A4B34 File Offset: 0x002A2D34
		private static void DismissCouncilors(TIFactionState faction, List<TIMissionTemplate> missingRequiredMissions, ref Dictionary<TICouncilorState, Dictionary<FactionResource, float>> councilorIncomes)
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			if (faction.IsActiveHumanFaction)
			{
				Dictionary<CouncilorView, float> dictionary = new Dictionary<CouncilorView, float>();
				foreach (TICouncilorState ticouncilorState in faction.councilors)
				{
					CouncilorView viewofCouncilor = faction.GetViewofCouncilor(ticouncilorState);
					dictionary.Add(viewofCouncilor, viewofCouncilor.EvaluateCouncilor());
					List<TICouncilorState> list2 = new List<TICouncilorState>(faction.councilors);
					list2.Remove(ticouncilorState);
					if (!faction.ShouldTryToRestoreCouncilorLoyalty(ticouncilorState) || dictionary[viewofCouncilor] < 0f)
					{
						if (viewofCouncilor.turned)
						{
							list.Add(ticouncilorState);
						}
						else if (list2.Count > 0 && faction.AI_SuspectTurned(ticouncilorState))
						{
							if (list2.None<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false).Contains(TIFactionState.inspireMission)))
							{
								list.Add(ticouncilorState);
							}
						}
					}
					else if (list2.Count > 0 && faction.GetViewofCouncilor(ticouncilorState).turned)
					{
						if (list2.None<TICouncilorState>((TICouncilorState x) => x.GetPossibleMissionList(false, false, false, null, false).Contains(TIFactionState.inspireMission)))
						{
							list.Add(ticouncilorState);
						}
					}
				}
				if (faction.councilors.Count == faction.maxCouncilSize && list.Count == 0 && missingRequiredMissions.Count > 0 && faction.availableCouncilors.Any<TICouncilorState>((TICouncilorState candidate) => candidate.typeTemplate.missions.Intersect<TIMissionTemplate>(missingRequiredMissions).Any<TIMissionTemplate>()))
				{
					list.Add(dictionary.Aggregate<KeyValuePair<CouncilorView, float>>(delegate(KeyValuePair<CouncilorView, float> l, KeyValuePair<CouncilorView, float> r)
					{
						if (l.Value >= r.Value)
						{
							return r;
						}
						return l;
					}).Key.councilor);
				}
			}
			else
			{
				int num = faction.availableCouncilors.Count<TICouncilorState>((TICouncilorState x) => x.location.ref_spaceBody.isEarth);
				if (num > 0)
				{
					foreach (TICouncilorState ticouncilorState2 in faction.councilors)
					{
						if (ticouncilorState2.location == faction.primaryHab)
						{
							list.Add(ticouncilorState2);
							if (list.Count == num)
							{
								break;
							}
						}
					}
				}
			}
			foreach (TICouncilorState ticouncilorState3 in list)
			{
				List<TransferOrgToFactionPoolAction> list3 = new List<TransferOrgToFactionPoolAction>();
				foreach (TIOrgState tiorgState in ticouncilorState3.orgs)
				{
					list3.Add(new TransferOrgToFactionPoolAction(tiorgState, ticouncilorState3));
				}
				foreach (TransferOrgToFactionPoolAction transferOrgToFactionPoolAction in list3)
				{
					faction.playerControl.StartAction(transferOrgToFactionPoolAction);
				}
				ticouncilorState3.RemoveFromGoals();
				faction.playerControl.StartAction(new DismissCouncilorAction(ticouncilorState3, faction, faction));
				councilorIncomes.Remove(ticouncilorState3);
			}
		}

		// Token: 0x06005A3B RID: 23099 RVA: 0x002A4EFC File Offset: 0x002A30FC
		public static bool AI_ControllingNeutralPowers(TIFactionState faction)
		{
			return faction.AllCaptureNationGoals(true).Any<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_nation.NumNativeControlPoints > 0 && x.target().ref_nation.SignificantPower);
		}

		// Token: 0x06005A3C RID: 23100 RVA: 0x002A4F2C File Offset: 0x002A312C
		public static void RecruitCouncilors(TIFactionState faction, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, ref Dictionary<TICouncilorState, Dictionary<FactionResource, float>> councilorIncomes, bool chasingHydra, int factionWars, bool controllingNeutralPowers)
		{
			int num = 6;
			if (faction.IsAlienFaction)
			{
				num = AIEvaluators.GetAliensPreferredCouncilorCount();
			}
			if (faction.councilors.Count >= num)
			{
				return;
			}
			if (faction.emptyCouncilorSlots > 0)
			{
				float maxWillingToSpend = (((float)(faction.emptyCouncilorSlots / faction.maxCouncilSize) <= 0.67f) ? 1f : 0.75f);
				Dictionary<CouncilorAttribute, float> currentKeyStatPresence = Enums.CouncilorAttributes.ToDictionary<CouncilorAttribute, CouncilorAttribute, float>((CouncilorAttribute x) => x, (CouncilorAttribute x) => 0f);
				currentKeyStatPresence.Add(CouncilorAttribute.None, 0f);
				foreach (TICouncilorState ticouncilorState in faction.councilors)
				{
					Dictionary<CouncilorAttribute, float> dictionary = currentKeyStatPresence;
					CouncilorAttribute councilorAttribute = ticouncilorState.typeTemplate.keyStat[0];
					dictionary[councilorAttribute] += 1f;
					dictionary = currentKeyStatPresence;
					councilorAttribute = ticouncilorState.typeTemplate.keyStat[1];
					dictionary[councilorAttribute] += 0.5f;
				}
				currentKeyStatPresence.Remove(CouncilorAttribute.None);
				currentKeyStatPresence.Remove(CouncilorAttribute.Loyalty);
				currentKeyStatPresence.Remove(CouncilorAttribute.ApparentLoyalty);
				currentKeyStatPresence.Remove(CouncilorAttribute.Security);
				CouncilorAttribute councilorAttribute2 = currentKeyStatPresence.Keys.MinBy<CouncilorAttribute, float>((CouncilorAttribute x) => currentKeyStatPresence[x]);
				Dictionary<TICouncilorState, float> candidates = AIEvaluators.EvaluateCandidateCouncilors(faction, requiredMissions, missingRequiredMissions, councilorAttribute2, chasingHydra, factionWars, controllingNeutralPowers);
				if (faction.IsAlienFaction)
				{
					if (candidates.Keys.Any<TICouncilorState>((TICouncilorState x) => x.OnOrAroundEarth))
					{
						foreach (TICouncilorState ticouncilorState2 in candidates.Keys.ToList<TICouncilorState>())
						{
							if (!ticouncilorState2.OnOrAroundEarth)
							{
								candidates.Remove(ticouncilorState2);
							}
						}
					}
				}
				int num2 = Mathf.Min(faction.emptyCouncilorSlots, num);
				Func<TICouncilorState, bool> <>9__4;
				Func<TICouncilorState, float> <>9__5;
				Func<TICouncilorState, float> <>9__6;
				for (int i = 0; i < num2; i++)
				{
					if (candidates.Count > 0)
					{
						AIDailyFactionPlanner.<>c__DisplayClass47_1 CS$<>8__locals2 = new AIDailyFactionPlanner.<>c__DisplayClass47_1();
						if (faction.councilors.Count <= 2)
						{
							AIDailyFactionPlanner.<>c__DisplayClass47_1 CS$<>8__locals3 = CS$<>8__locals2;
							IEnumerable<TICouncilorState> keys = candidates.Keys;
							Func<TICouncilorState, bool> func;
							if ((func = <>9__4) == null)
							{
								func = (<>9__4 = (TICouncilorState x) => x.HireRecruitCost(faction).CanAfford(faction, maxWillingToSpend, null, float.PositiveInfinity));
							}
							IEnumerable<TICouncilorState> enumerable = keys.Where<TICouncilorState>(func);
							Func<TICouncilorState, float> func2;
							if ((func2 = <>9__5) == null)
							{
								func2 = (<>9__5 = (TICouncilorState x) => candidates[x]);
							}
							CS$<>8__locals3.favorite = enumerable.MaxBy<TICouncilorState, float>(func2);
						}
						else
						{
							AIDailyFactionPlanner.<>c__DisplayClass47_1 CS$<>8__locals4 = CS$<>8__locals2;
							IEnumerable<TICouncilorState> keys2 = candidates.Keys;
							Func<TICouncilorState, float> func3;
							if ((func3 = <>9__6) == null)
							{
								func3 = (<>9__6 = (TICouncilorState x) => candidates[x]);
							}
							CS$<>8__locals4.favorite = keys2.MaxBy<TICouncilorState, float>(func3);
						}
						if (CS$<>8__locals2.favorite != null && CS$<>8__locals2.favorite.HireRecruitCost(faction).CanAfford(faction, maxWillingToSpend, new List<FactionResource> { FactionResource.Influence }, float.PositiveInfinity) && candidates[CS$<>8__locals2.favorite] > 0f)
						{
							faction.playerControl.StartAction(new RecruitCouncilorAction(CS$<>8__locals2.favorite, faction));
							councilorIncomes.Add(CS$<>8__locals2.favorite, TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource y) => CS$<>8__locals2.favorite.GetMonthlyIncome(y)));
							candidates.Remove(CS$<>8__locals2.favorite);
						}
					}
				}
			}
		}

		// Token: 0x06005A3D RID: 23101 RVA: 0x002A5384 File Offset: 0x002A3584
		private static bool TryAddOrgToCouncilor(AIDailyFactionPlanner.OrgCouncilorScore orgCandidate, List<TIMissionTemplate> possibleMissions, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, out bool removeOrg, out bool removeCouncilor, Dictionary<FactionResource, float> councilorIncomes, bool chasingHydra, int factionWars, bool chasingNeutralPowers)
		{
			TIFactionState faction = orgCandidate.councilor.faction;
			removeCouncilor = false;
			removeOrg = false;
			if (faction.CanPurchaseOrg(orgCandidate.org))
			{
				if (!orgCandidate.councilor.SufficientCapacityForOrg(orgCandidate.org) && orgCandidate.councilor.orgs.Count > 0)
				{
					Dictionary<TIOrgState, float> dictionary = new Dictionary<TIOrgState, float>();
					foreach (TIOrgState tiorgState in orgCandidate.councilor.RemoveableOrgs())
					{
						float num = AIEvaluators.EvaluateOrgForCouncilor(tiorgState, orgCandidate.councilor, possibleMissions, requiredMissions, missingRequiredMissions, false, councilorIncomes, chasingHydra, factionWars, chasingNeutralPowers, false);
						if (tiorgState.orgType != OrgType.Faction || num < 0f)
						{
							dictionary.Add(tiorgState, (num > 0f) ? (1.075f * num) : num);
						}
					}
					if (dictionary.Count > 0)
					{
						int num2 = orgCandidate.org.tier - orgCandidate.councilor.availableAdministration - orgCandidate.org.administration;
						int num3 = ((orgCandidate.councilor.orgs.Count == TemplateManager.global.councilorMaxOrgs) ? 1 : 0);
						int num4 = orgCandidate.councilor.orgsWeight + orgCandidate.org.tier;
						TIOrgState key = dictionary.MinBy<KeyValuePair<TIOrgState, float>, float>((KeyValuePair<TIOrgState, float> x) => x.Value).Key;
						List<TIOrgState> list = new List<TIOrgState>();
						float num5 = 0f;
						if (orgCandidate.score <= dictionary[key])
						{
							removeCouncilor = true;
							return false;
						}
						num5 += dictionary[key];
						list.Add(key);
						dictionary.Remove(key);
						num2 = num2 - key.tier + key.administration;
						int num6 = 0;
						num4 -= key.tier;
						if ((num2 > 0 || num4 > orgCandidate.councilor.GetClampedMaxStatValue(CouncilorAttribute.Administration)) && dictionary.Count > 0)
						{
							TIOrgState key2 = dictionary.MinBy<KeyValuePair<TIOrgState, float>, float>((KeyValuePair<TIOrgState, float> x) => x.Value).Key;
							if (orgCandidate.score <= dictionary[key2] + num5)
							{
								return false;
							}
							num5 += dictionary[key2];
							list.Add(key2);
							dictionary.Remove(key2);
							num2 = num2 - key2.tier + key2.administration;
							num4 -= key2.tier;
							if ((num2 > 0 || num4 > orgCandidate.councilor.GetClampedMaxStatValue(CouncilorAttribute.Administration)) && dictionary.Count > 0)
							{
								TIOrgState key3 = dictionary.MinBy<KeyValuePair<TIOrgState, float>, float>((KeyValuePair<TIOrgState, float> x) => x.Value).Key;
								if (orgCandidate.score <= dictionary[key3] + num5)
								{
									return false;
								}
								num5 += dictionary[key3];
								list.Add(key3);
								num2 = num2 - key3.tier + key3.administration;
								num4 -= key3.tier;
							}
						}
						if (num2 > 0 || num6 > 0 || num4 > orgCandidate.councilor.GetClampedMaxStatValue(CouncilorAttribute.Administration))
						{
							goto IL_0385;
						}
						using (List<TIOrgState>.Enumerator enumerator = list.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TIOrgState tiorgState2 = enumerator.Current;
								faction.playerControl.StartAction(new TransferOrgToFactionPoolAction(tiorgState2, orgCandidate.councilor));
							}
							goto IL_0385;
						}
					}
					return false;
				}
				IL_0385:
				if (orgCandidate.councilor.SufficientCapacityForOrg(orgCandidate.org))
				{
					faction.playerControl.StartAction(new PurchaseOrgAction(orgCandidate.org, faction, orgCandidate.councilor));
					return true;
				}
			}
			else
			{
				removeOrg = true;
			}
			return false;
		}

		// Token: 0x06005A3E RID: 23102 RVA: 0x002A576C File Offset: 0x002A396C
		private static void PurchaseOrgs(TIFactionState faction, Dictionary<TIOrgState, TICouncilorState> missionCriticalTransfers, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, ref Dictionary<TICouncilorState, Dictionary<FactionResource, float>> councilorIncomes, bool chasingHydra, int factionWars, bool controllingNeutralPowers, List<TICouncilorState> criticalAdminNeed)
		{
			float num;
			float num2;
			if (TIGlobalValuesState.IsQuietAlienCampaign())
			{
				float currentResourceAmount = faction.GetCurrentResourceAmount(FactionResource.Money);
				num = Mathf.Max(0f, currentResourceAmount - 1000f);
				num2 = faction.GetCurrentResourceAmount(FactionResource.Influence);
				if (faction.emptyCouncilorSlots > 0)
				{
					num2 = Mathf.Max(0f, num2 - 60f);
				}
				if (TIUtilities.RandomFloatValue() < 0.1f)
				{
					num *= 0.8f;
					num2 *= 0.8f;
				}
				else
				{
					num *= 0.2f;
					num2 *= 0.2f;
				}
			}
			else
			{
				float currentResourceAmount2 = faction.GetCurrentResourceAmount(FactionResource.Money);
				num = ((currentResourceAmount2 > (float)AIEvaluators.AbundantValue(FactionResource.Money)) ? (currentResourceAmount2 * 0.95f) : (currentResourceAmount2 * 0.85f));
				float currentResourceAmount3 = faction.GetCurrentResourceAmount(FactionResource.Influence);
				if (currentResourceAmount3 > (float)AIEvaluators.AbundantValue(FactionResource.Influence))
				{
					num2 = currentResourceAmount3 * 0.75f;
				}
				else if (faction.emptyCouncilorSlots > 0)
				{
					num2 = currentResourceAmount3 * 0.25f;
				}
				else if (controllingNeutralPowers)
				{
					num2 = currentResourceAmount3 * 0.4f;
				}
				else
				{
					num2 = currentResourceAmount3 * 0.5f;
				}
			}
			float num3 = faction.GetCurrentResourceAmount(FactionResource.Operations) * (faction.currentlyDetectingHydra ? 0.25f : 0.75f);
			float num4 = AIEvaluators.GetMaxBoostForRateLimitedBoostPurchase(faction, 0.1f, TIFactionState.BoostAccountName.Org);
			List<AIDailyFactionPlanner.OrgCouncilorScore> list = new List<AIDailyFactionPlanner.OrgCouncilorScore>();
			Dictionary<TICouncilorState, List<TIMissionTemplate>> dictionary = faction.councilors.ToDictionary<TICouncilorState, TICouncilorState, List<TIMissionTemplate>>((TICouncilorState x) => x, (TICouncilorState x) => x.GetPossibleMissionList(false, false, true, null, false));
			List<TIOrgState> list2 = new List<TIOrgState>(faction.availableOrgs);
			IEnumerable<TIOrgState> keys = missionCriticalTransfers.Keys;
			Func<TIOrgState, bool> <>9__4;
			Func<TIOrgState, bool> func;
			if ((func = <>9__4) == null)
			{
				func = (<>9__4 = (TIOrgState x) => faction.availableOrgs.Contains(x));
			}
			foreach (TIOrgState tiorgState in keys.Where<TIOrgState>(func))
			{
				list.Add(new AIDailyFactionPlanner.OrgCouncilorScore
				{
					org = tiorgState,
					councilor = missionCriticalTransfers[tiorgState],
					score = float.MaxValue
				});
				list2.Remove(tiorgState);
			}
			foreach (TIOrgState tiorgState2 in list2)
			{
				if (faction.CanPurchaseOrg(tiorgState2))
				{
					new List<AIDailyFactionPlanner.CouncilorOrgValue>();
					foreach (TICouncilorState ticouncilorState in faction.activeCouncilors)
					{
						if (tiorgState2.CouncilorCanAcquire(ticouncilorState))
						{
							float num5 = AIEvaluators.EvaluateOrgForCouncilor(tiorgState2, ticouncilorState, dictionary[ticouncilorState], requiredMissions, missingRequiredMissions, true, councilorIncomes[ticouncilorState], chasingHydra, factionWars, controllingNeutralPowers, criticalAdminNeed.Contains(ticouncilorState));
							if (num5 > 0f)
							{
								list.Add(new AIDailyFactionPlanner.OrgCouncilorScore
								{
									org = tiorgState2,
									councilor = ticouncilorState,
									score = num5
								});
							}
						}
					}
				}
			}
			list = list.OrderByDescending<AIDailyFactionPlanner.OrgCouncilorScore, float>((AIDailyFactionPlanner.OrgCouncilorScore o) => o.score).ThenBy<AIDailyFactionPlanner.OrgCouncilorScore, int>((AIDailyFactionPlanner.OrgCouncilorScore x) => x.councilor.orgs.Count).ToList<AIDailyFactionPlanner.OrgCouncilorScore>();
			List<TICouncilorState> list3 = new List<TICouncilorState>();
			List<TIOrgState> list4 = new List<TIOrgState>();
			using (List<AIDailyFactionPlanner.OrgCouncilorScore>.Enumerator enumerator4 = list.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					AIDailyFactionPlanner.OrgCouncilorScore orgCandidate = enumerator4.Current;
					if (orgCandidate.score <= 0f)
					{
						break;
					}
					if (list4.Contains(orgCandidate.org) || list3.Contains(orgCandidate.councilor))
					{
						break;
					}
					TIResourcesCost purchaseCost = orgCandidate.org.GetPurchaseCost(faction);
					bool flag = orgCandidate.score == float.MaxValue || (orgCandidate.org.administration - orgCandidate.org.tier > 0 && criticalAdminNeed.Contains(orgCandidate.councilor)) || missingRequiredMissions.Intersect<TIMissionTemplate>(orgCandidate.org.missionsGranted).Any<TIMissionTemplate>();
					if (purchaseCost.CanAfford_AI(faction, null, null, 1, false, false, 1f, null, float.PositiveInfinity) && (flag || (purchaseCost.GetSingleCostValue(FactionResource.Money) <= num && purchaseCost.GetSingleCostValue(FactionResource.Influence) <= num2 && purchaseCost.GetSingleCostValue(FactionResource.Operations) <= num3 && purchaseCost.GetSingleCostValue(FactionResource.Boost) <= num4)))
					{
						bool flag2;
						bool flag3;
						if (AIDailyFactionPlanner.TryAddOrgToCouncilor(orgCandidate, dictionary[orgCandidate.councilor], requiredMissions, missingRequiredMissions, out flag2, out flag3, councilorIncomes[orgCandidate.councilor], chasingHydra, factionWars, controllingNeutralPowers))
						{
							num -= purchaseCost.GetSingleCostValue(FactionResource.Money) + orgCandidate.org.adjustedIncomeMoney_month;
							num2 -= purchaseCost.GetSingleCostValue(FactionResource.Influence) + orgCandidate.org.adjustedIncomeInfluence_month;
							num3 -= purchaseCost.GetSingleCostValue(FactionResource.Operations) + orgCandidate.org.adjustedIncomeOps_month;
							missingRequiredMissions.RemoveAll((TIMissionTemplate x) => orgCandidate.org.missionsGranted.Contains(x));
							if (purchaseCost.GetSingleCostValue(FactionResource.Boost) > 0f)
							{
								num4 = 0f;
								faction.boostAccounts[TIFactionState.BoostAccountName.Org] = TITimeState.Now();
							}
							dictionary[orgCandidate.councilor] = orgCandidate.councilor.GetPossibleMissionList(false, false, true, null, false);
							councilorIncomes[orgCandidate.councilor] = TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource y) => orgCandidate.councilor.GetMonthlyIncome(y));
							flag2 = true;
						}
						if (flag3)
						{
							list3.Add(orgCandidate.councilor);
						}
						if (flag2)
						{
							list4.Add(orgCandidate.org);
						}
					}
					else
					{
						list4.Add(orgCandidate.org);
					}
				}
			}
		}

		// Token: 0x06005A3F RID: 23103 RVA: 0x002A5E84 File Offset: 0x002A4084
		private static void TransferOrgsToPool(TIFactionState faction)
		{
			foreach (TICouncilorState ticouncilorState in faction.councilors.ToList<TICouncilorState>())
			{
				if (ticouncilorState.detained && ticouncilorState.detainingFaction != faction)
				{
					foreach (TIOrgState tiorgState in ticouncilorState.orgs.ToList<TIOrgState>())
					{
						if (tiorgState.orgType == OrgType.Faction)
						{
							faction.playerControl.StartAction(new TransferOrgToFactionPoolAction(tiorgState, ticouncilorState));
						}
					}
				}
			}
		}

		// Token: 0x06005A40 RID: 23104 RVA: 0x002A5F48 File Offset: 0x002A4148
		public static void TransferOrgsFromPool(TIFactionState faction)
		{
			List<TIMissionTemplate> list = faction.RequiredMissions(true);
			List<TIMissionTemplate> list2 = faction.MissingRequiredMissions(list);
			bool currentlyDetectingHydra = faction.currentlyDetectingHydra;
			int count = faction.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
			Dictionary<TICouncilorState, Dictionary<FactionResource, float>> dictionary = faction.councilors.ToDictionary<TICouncilorState, TICouncilorState, Dictionary<FactionResource, float>>((TICouncilorState x) => x, (TICouncilorState y) => TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource z) => z, (FactionResource z) => y.GetMonthlyIncome(z)));
			bool flag = AIDailyFactionPlanner.AI_ControllingNeutralPowers(faction);
			AIDailyFactionPlanner.TransferOrgsFromPool(faction, new Dictionary<TIOrgState, TICouncilorState>(), list, list2, ref dictionary, currentlyDetectingHydra, count, flag);
		}

		// Token: 0x06005A41 RID: 23105 RVA: 0x002A5FE4 File Offset: 0x002A41E4
		public static void TransferOrgsFromPool(TIFactionState faction, Dictionary<TIOrgState, TICouncilorState> missionCriticalTransfers, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, ref Dictionary<TICouncilorState, Dictionary<FactionResource, float>> councilorIncomes, bool chasingHydra, int factionWars, bool controllingNeutralPowers)
		{
			List<TIOrgState> list = new List<TIOrgState>();
			List<AIDailyFactionPlanner.OrgCouncilorScore> list2 = new List<AIDailyFactionPlanner.OrgCouncilorScore>();
			List<AIDailyFactionPlanner.OrgCouncilorScore> list3 = new List<AIDailyFactionPlanner.OrgCouncilorScore>();
			Dictionary<TICouncilorState, List<TIMissionTemplate>> dictionary = faction.councilors.ToDictionary<TICouncilorState, TICouncilorState, List<TIMissionTemplate>>((TICouncilorState x) => x, (TICouncilorState x) => x.GetPossibleMissionList(false, false, true, null, false));
			IEnumerable<TIOrgState> keys = missionCriticalTransfers.Keys;
			Func<TIOrgState, bool> <>9__5;
			Func<TIOrgState, bool> func;
			if ((func = <>9__5) == null)
			{
				func = (<>9__5 = (TIOrgState x) => faction.unassignedOrgs.Contains(x));
			}
			foreach (TIOrgState tiorgState in keys.Where<TIOrgState>(func))
			{
				list.Add(tiorgState);
				list2.Add(new AIDailyFactionPlanner.OrgCouncilorScore
				{
					councilor = missionCriticalTransfers[tiorgState],
					org = tiorgState,
					score = float.MaxValue
				});
			}
			foreach (TIOrgState tiorgState2 in new List<TIOrgState>(faction.unassignedOrgs.Except<TIOrgState>(list)))
			{
				if (tiorgState2.orgType == OrgType.Faction)
				{
					list.Add(tiorgState2);
					using (List<TICouncilorState>.Enumerator enumerator3 = faction.activeCouncilors.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							TICouncilorState ticouncilorState = enumerator3.Current;
							if (tiorgState2.CouncilorCanAcquire(ticouncilorState))
							{
								list2.Add(new AIDailyFactionPlanner.OrgCouncilorScore
								{
									org = tiorgState2,
									councilor = ticouncilorState,
									score = 100f * AIEvaluators.EvaluateOrgForCouncilor(tiorgState2, ticouncilorState, dictionary[ticouncilorState], requiredMissions, missingRequiredMissions, true, councilorIncomes[ticouncilorState], chasingHydra, factionWars, controllingNeutralPowers, false)
								});
							}
						}
						continue;
					}
				}
				new List<AIDailyFactionPlanner.CouncilorOrgValue>();
				foreach (TICouncilorState ticouncilorState2 in faction.activeCouncilors)
				{
					if (tiorgState2.CouncilorCanAcquire(ticouncilorState2))
					{
						float num = AIEvaluators.EvaluateOrgForCouncilor(tiorgState2, ticouncilorState2, requiredMissions, dictionary[ticouncilorState2], missingRequiredMissions, true, councilorIncomes[ticouncilorState2], chasingHydra, factionWars, controllingNeutralPowers, false);
						if (num > 0f)
						{
							list3.Add(new AIDailyFactionPlanner.OrgCouncilorScore
							{
								org = tiorgState2,
								councilor = ticouncilorState2,
								score = num
							});
						}
					}
				}
			}
			if (list.Count > 0)
			{
				list2 = list2.OrderByDescending<AIDailyFactionPlanner.OrgCouncilorScore, float>((AIDailyFactionPlanner.OrgCouncilorScore o) => o.score).ToList<AIDailyFactionPlanner.OrgCouncilorScore>();
				foreach (AIDailyFactionPlanner.OrgCouncilorScore orgCouncilorScore in list2)
				{
					if (list.Contains(orgCouncilorScore.org) && orgCouncilorScore.org.GetTransferCost().CanAfford(faction, 1f, null, float.PositiveInfinity) && orgCouncilorScore.councilor.SufficientCapacityForOrg(orgCouncilorScore.org) && !orgCouncilorScore.councilor.detained)
					{
						faction.playerControl.StartAction(new PurchaseOrgAction(orgCouncilorScore.org, faction, orgCouncilorScore.councilor));
						dictionary[orgCouncilorScore.councilor] = orgCouncilorScore.councilor.GetPossibleMissionList(false, false, true, null, false);
						list.Remove(orgCouncilorScore.org);
					}
				}
				if (list.Count > 0)
				{
					using (List<AIDailyFactionPlanner.OrgCouncilorScore>.Enumerator enumerator4 = list2.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							AIDailyFactionPlanner.OrgCouncilorScore orgToForce = enumerator4.Current;
							bool flag;
							bool flag2;
							if (list.Contains(orgToForce.org) && !orgToForce.councilor.detained && AIDailyFactionPlanner.TryAddOrgToCouncilor(orgToForce, dictionary[orgToForce.councilor], requiredMissions, missingRequiredMissions, out flag, out flag2, councilorIncomes[orgToForce.councilor], chasingHydra, factionWars, controllingNeutralPowers))
							{
								list.Remove(orgToForce.org);
								dictionary[orgToForce.councilor] = orgToForce.councilor.GetPossibleMissionList(false, false, true, null, false);
								councilorIncomes[orgToForce.councilor] = TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource y) => orgToForce.councilor.GetMonthlyIncome(y));
							}
						}
					}
				}
			}
			List<TIOrgState> list4 = new List<TIOrgState>();
			list3 = list3.OrderByDescending<AIDailyFactionPlanner.OrgCouncilorScore, float>((AIDailyFactionPlanner.OrgCouncilorScore o) => o.score).ThenBy<AIDailyFactionPlanner.OrgCouncilorScore, int>((AIDailyFactionPlanner.OrgCouncilorScore x) => x.councilor.orgs.Count).ToList<AIDailyFactionPlanner.OrgCouncilorScore>();
			using (List<AIDailyFactionPlanner.OrgCouncilorScore>.Enumerator enumerator4 = list3.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					AIDailyFactionPlanner.OrgCouncilorScore org = enumerator4.Current;
					bool flag3;
					bool flag4;
					if (org.org.hasFactionbutNoCouncilor && !list4.Contains(org.org) && AIDailyFactionPlanner.TryAddOrgToCouncilor(org, dictionary[org.councilor], requiredMissions, missingRequiredMissions, out flag3, out flag4, councilorIncomes[org.councilor], chasingHydra, factionWars, controllingNeutralPowers))
					{
						list4.Add(org.org);
						dictionary[org.councilor] = org.councilor.GetPossibleMissionList(false, false, true, null, false);
						councilorIncomes[org.councilor] = TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource y) => org.councilor.GetMonthlyIncome(y));
					}
				}
			}
		}

		// Token: 0x06005A42 RID: 23106 RVA: 0x002A671C File Offset: 0x002A491C
		public static void SellOrgs(TIFactionState faction, List<TIMissionTemplate> requiredMissions, int requiredToSell = 0)
		{
			int num = 0;
			using (List<TIOrgState>.Enumerator enumerator = (from x in faction.unassignedOrgs
				orderby x.tier, x.administration
				select x).ToList<TIOrgState>().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIOrgState org = enumerator.Current;
					faction.GetDailyIncome(FactionResource.Money, false, false);
					if (org.AllowedOnFactionMarket(faction) && org.missionsGranted.Intersect<TIMissionTemplate>(requiredMissions).Any<TIMissionTemplate>())
					{
						if (faction.councilors.None<TICouncilorState>((TICouncilorState x) => org.IsEligibleForCouncilor(x) && org.tier < 3))
						{
							faction.playerControl.StartAction(new SellOrgAction(org, faction, null));
							num++;
						}
						else if ((org.adjustedIncomeMoney_month < 0f && faction.GetDailyIncome(FactionResource.Money, false, false) < 0f) || (org.adjustedIncomeInfluence_month < 0f && faction.GetDailyIncome(FactionResource.Influence, false, false) < 0f) || (org.adjustedIncomeOps_month < 0f && faction.GetDailyIncome(FactionResource.Operations, false, false) < 0f) || (org.adjustedIncomeBoost_month < 0f && faction.GetDailyIncome(FactionResource.Boost, false, false) < 0f) || (org.incomeMissionControl < 0f && faction.MissionControlShortage > 0))
						{
							faction.playerControl.StartAction(new SellOrgAction(org, faction, null));
							num++;
						}
					}
				}
			}
			requiredToSell -= num;
			if (requiredToSell > 0)
			{
				List<TIOrgState> list = new List<TIOrgState>(from x in faction.unassignedOrgs
					where x.AllowedOnFactionMarket(faction)
					orderby x.missionsGranted.Intersect<TIMissionTemplate>(requiredMissions).Any<TIMissionTemplate>(), x.tier
					select x).OrderByDescending<TIOrgState, float>((TIOrgState x) => x.GetSalePrice(false).GetSingleCostValue(FactionResource.Money)).ToList<TIOrgState>();
				int num2 = 0;
				foreach (TIOrgState tiorgState in list)
				{
					faction.playerControl.StartAction(new SellOrgAction(tiorgState, faction, null));
					num2++;
					if (num2 >= requiredToSell)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06005A43 RID: 23107 RVA: 0x002A6A5C File Offset: 0x002A4C5C
		public static void SpendXP(TIFactionState faction, List<TIMissionTemplate> requiredMissions, List<TIMissionTemplate> missingRequiredMissions, ref Dictionary<TICouncilorState, Dictionary<FactionResource, float>> councilorIncomes, bool chasingHydra, int factionWars, bool controllingNeutralPowers)
		{
			AIDailyFactionPlanner.<>c__DisplayClass56_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass56_0();
			CS$<>8__locals1.requiredMissions = requiredMissions;
			CS$<>8__locals1.missingRequiredMissions = missingRequiredMissions;
			CS$<>8__locals1.chasingHydra = chasingHydra;
			CS$<>8__locals1.factionWars = factionWars;
			CS$<>8__locals1.controllingNeutralPowers = controllingNeutralPowers;
			using (List<TICouncilorState>.Enumerator enumerator = faction.councilors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AIDailyFactionPlanner.<>c__DisplayClass56_1 CS$<>8__locals2 = new AIDailyFactionPlanner.<>c__DisplayClass56_1();
					CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
					CS$<>8__locals2.councilor = enumerator.Current;
					List<TIMissionTemplate> possibleMissions = CS$<>8__locals2.councilor.GetPossibleMissionList(false, false, true, null, false);
					Dictionary<FactionResource, float> incomes = councilorIncomes[CS$<>8__locals2.councilor];
					Dictionary<CouncilorAugmentationOption, float> dictionary = (from x in CS$<>8__locals2.councilor.GetCandidateAugmentations()
						where x.CouncilorCanAfford(CS$<>8__locals2.councilor)
						select x).ToDictionary<CouncilorAugmentationOption, CouncilorAugmentationOption, float>((CouncilorAugmentationOption x) => x, (CouncilorAugmentationOption x) => AIEvaluators.EvaluateAugmentationOption(CS$<>8__locals2.councilor, x, possibleMissions, CS$<>8__locals2.CS$<>8__locals1.requiredMissions, CS$<>8__locals2.CS$<>8__locals1.missingRequiredMissions, incomes, CS$<>8__locals2.CS$<>8__locals1.chasingHydra, CS$<>8__locals2.CS$<>8__locals1.factionWars, CS$<>8__locals2.CS$<>8__locals1.controllingNeutralPowers));
					if (dictionary.Keys.Count > 0)
					{
						if (dictionary.Any<KeyValuePair<CouncilorAugmentationOption, float>>((KeyValuePair<CouncilorAugmentationOption, float> x) => x.Value > 0f))
						{
							faction.playerControl.StartAction(new AugmentCouncilorAction(CS$<>8__locals2.councilor, dictionary.SelectRandomWeightedItem<KeyValuePair<CouncilorAugmentationOption, float>>((KeyValuePair<CouncilorAugmentationOption, float> o) => o.Value, -1f, 1E-37f).Key));
							councilorIncomes[CS$<>8__locals2.councilor] = TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource x) => x, (FactionResource y) => CS$<>8__locals2.councilor.GetMonthlyIncome(y));
						}
					}
				}
			}
		}

		// Token: 0x06005A44 RID: 23108 RVA: 0x002A6C70 File Offset: 0x002A4E70
		private static void ManageCouncilors(TIFactionState faction, GameTimeManager gameTime)
		{
			int day = gameTime.currentTime.day;
			if (day % 7 == AIDailyFactionPlanner.factionAIData[faction].every7DaysOffset && day <= 28)
			{
				List<TIMissionTemplate> list = faction.ObjectiveCriticalMissions();
				List<TIMissionTemplate> list2 = faction.RequiredMissions(true);
				List<TIMissionTemplate> list3 = faction.MissingRequiredMissions(list2);
				bool currentlyDetectingHydra = faction.currentlyDetectingHydra;
				int count = faction.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
				Dictionary<TICouncilorState, Dictionary<FactionResource, float>> dictionary = faction.councilors.ToDictionary<TICouncilorState, TICouncilorState, Dictionary<FactionResource, float>>((TICouncilorState x) => x, (TICouncilorState y) => TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource z) => z, (FactionResource z) => y.GetMonthlyIncome(z)));
				AIDailyFactionPlanner.DismissCouncilors(faction, list3, ref dictionary);
				Dictionary<TIOrgState, TICouncilorState> dictionary2 = new Dictionary<TIOrgState, TICouncilorState>();
				List<TICouncilorState> list4 = new List<TICouncilorState>();
				if (list.Count > 0)
				{
					List<TIMissionTemplate> list5;
					dictionary2 = faction.ProposeOptimizedCriticalOrgMissions(out list5, list);
					foreach (TIOrgState tiorgState in dictionary2.Keys)
					{
						if (tiorgState.hasCouncilor)
						{
							if (faction.CanTransferOrgFromCouncilorToCouncilor(tiorgState, dictionary2[tiorgState], true))
							{
								faction.playerControl.StartAction(new TransferOrgToCouncilorAction(tiorgState, faction, dictionary2[tiorgState], tiorgState.assignedCouncilor));
							}
							else
							{
								faction.playerControl.StartAction(new TransferOrgToFactionPoolAction(tiorgState, tiorgState.assignedCouncilor));
							}
						}
					}
					foreach (TIOrgState tiorgState2 in dictionary2.Keys.ToList<TIOrgState>())
					{
						if (dictionary2[tiorgState2] == tiorgState2.assignedCouncilor)
						{
							dictionary2.Remove(tiorgState2);
						}
					}
					foreach (TICouncilorState ticouncilorState in faction.councilors)
					{
						if (ticouncilorState.availableAdministration <= 1 && ticouncilorState.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false) < ticouncilorState.GetClampedMaxStatValue(CouncilorAttribute.Administration) - 1)
						{
							foreach (TIMissionTemplate timissionTemplate in from x in ticouncilorState.GetPossibleMissionList(false, false, false, null, false).Intersect<TIMissionTemplate>(list)
								where x.primaryAttackerStat > CouncilorAttribute.None
								select x)
							{
								if (ticouncilorState.GetAttribute(timissionTemplate.primaryAttackerStat, true, true, true, false, false, false) < 15 + TITimeState.CampaignDuration_CompleteYears())
								{
									list4.AddUnique(ticouncilorState);
								}
							}
						}
					}
				}
				bool flag = AIDailyFactionPlanner.AI_ControllingNeutralPowers(faction);
				AIDailyFactionPlanner.TransferOrgsToPool(faction);
				AIDailyFactionPlanner.RecruitCouncilors(faction, list2, list3, ref dictionary, currentlyDetectingHydra, count, flag);
				AIDailyFactionPlanner.SpendXP(faction, list2, list3, ref dictionary, currentlyDetectingHydra, count, flag);
				AIDailyFactionPlanner.TransferOrgsFromPool(faction, dictionary2, list2, list3, ref dictionary, currentlyDetectingHydra, count, flag);
				AIDailyFactionPlanner.PurchaseOrgs(faction, dictionary2, list2, list3, ref dictionary, currentlyDetectingHydra, count, flag, list4);
				AIDailyFactionPlanner.SellOrgs(faction, list2, 0);
				foreach (TICouncilorState ticouncilorState2 in faction.councilors)
				{
					if (faction.GetViewofCouncilor(ticouncilorState2).turned)
					{
						faction.GainFactionHate(ticouncilorState2.agentForFaction, TIFactionState.turnMission.hate[1], false, "My councilor turned", true);
					}
				}
				AIDailyFactionPlanner.CheckSellResourcesOnEarth(faction);
			}
		}

		// Token: 0x06005A45 RID: 23109 RVA: 0x002A7028 File Offset: 0x002A5228
		private void SetResearchPriorities(TIFactionState faction)
		{
			AIDailyFactionPlanner.<>c__DisplayClass58_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass58_0();
			CS$<>8__locals1.faction = faction;
			if (CS$<>8__locals1.faction.IsAlienFaction)
			{
				return;
			}
			CS$<>8__locals1.faction.GetYearlyIncome(FactionResource.Research, false, false, true);
			if (!CS$<>8__locals1.faction.AIReviewProjects)
			{
				for (int n = 3; n <= 5; n++)
				{
					if (CS$<>8__locals1.faction.ProjectAllowedInSlot(n) && CS$<>8__locals1.faction.GetProjectProgressInSlot(n) == null)
					{
						CS$<>8__locals1.faction.AIReviewProjects = true;
					}
				}
			}
			if (CS$<>8__locals1.faction.AIReviewProjects)
			{
				int num = CS$<>8__locals1.faction.MostReplaceableProjectSlot();
				TIProjectTemplate projectInSlot = CS$<>8__locals1.faction.GetProjectInSlot(num);
				if (projectInSlot != null)
				{
					ProjectProgress projectProgressInSlot = CS$<>8__locals1.faction.GetProjectProgressInSlot(num);
					float researchCost = projectInSlot.GetResearchCost(CS$<>8__locals1.faction);
					float num2 = researchCost - projectProgressInSlot.accumulatedResearch;
					float num3 = (float)CS$<>8__locals1.faction.researchWeights[num] / (float)CS$<>8__locals1.faction.researchWeights.Sum() * CS$<>8__locals1.faction.GetYearlyIncome(FactionResource.Research, true, false, false);
					bool flag = num2 / num3 > 1f;
					bool flag2 = num2 / num3 < 0.1f && projectProgressInSlot.accumulatedResearch / researchCost > 0.7f;
					float num4 = CS$<>8__locals1.faction.GetProjectProgressInSlot(num).accumulatedResearch / researchCost;
					if (projectInSlot.AI_projectRole == ProjectRole.Objective || projectInSlot.AI_criticalTech || CS$<>8__locals1.faction.forcedTechNames.Contains(projectInSlot.dataName))
					{
						if (!flag)
						{
							CS$<>8__locals1.faction.AIReviewProjects = false;
						}
					}
					else if (flag2)
					{
						CS$<>8__locals1.faction.AIReviewProjects = false;
					}
				}
				if (CS$<>8__locals1.faction.AIReviewProjects)
				{
					TIProjectTemplate tiprojectTemplate = AIEvaluators.SelectProject(CS$<>8__locals1.faction, num);
					CS$<>8__locals1.faction.playerControl.StartAction(new SelectProjectForDevelopmentAction(CS$<>8__locals1.faction, num, tiprojectTemplate));
				}
				CS$<>8__locals1.faction.AIReviewProjects = false;
				TIProjectTemplate tiprojectTemplate2 = CS$<>8__locals1.faction.CurrentlyActiveProjects().FirstOrDefault<TIProjectTemplate>((TIProjectTemplate x) => x.AI_projectRole == ProjectRole.Objective);
				TIProjectTemplate tiprojectTemplate3 = CS$<>8__locals1.faction.availableProjects.Where<TIProjectTemplate>((TIProjectTemplate x) => x.AI_projectRole == ProjectRole.Objective).MinBy<TIProjectTemplate, float>(delegate(TIProjectTemplate x)
				{
					ProjectProgress projectProgress = CS$<>8__locals1.faction.currentProjectProgress.FirstOrDefault<ProjectProgress>((ProjectProgress y) => y.projectTemplateName == x.dataName);
					return x.researchCost - ((projectProgress != null) ? projectProgress.accumulatedResearch : 0f);
				});
				if (tiprojectTemplate3 != null && tiprojectTemplate2 != tiprojectTemplate3)
				{
					int num5 = CS$<>8__locals1.faction.GetSlotForProject(tiprojectTemplate2);
					if (num5 < 3)
					{
						num5 = 3;
					}
					CS$<>8__locals1.faction.playerControl.StartAction(new SelectProjectForDevelopmentAction(CS$<>8__locals1.faction, num5, tiprojectTemplate3));
				}
			}
			bool flag4;
			bool flag3 = CS$<>8__locals1.faction.ShouldFocusOnObjectiveProject(out flag4);
			bool flag5 = CS$<>8__locals1.faction.ShouldFocusOnGlobalResearch();
			bool flag6 = false;
			CS$<>8__locals1.techRaceSlot = (CS$<>8__locals1.faction.IsInTechRace ? CS$<>8__locals1.faction.TechRaceSlot : AIEvaluators.SelectTechRaceSlot(CS$<>8__locals1.faction));
			CS$<>8__locals1.raceTechProgress = ((CS$<>8__locals1.techRaceSlot >= 0) ? GameStateManager.GlobalResearch().GetTechProgress(CS$<>8__locals1.techRaceSlot) : null);
			if (CS$<>8__locals1.faction.IsInTechRace)
			{
				bool flag7 = CS$<>8__locals1.raceTechProgress.CantWin(CS$<>8__locals1.faction);
				bool flag8 = CS$<>8__locals1.raceTechProgress.CantLose(CS$<>8__locals1.faction);
				if (flag8)
				{
					CS$<>8__locals1.faction.SetPassiveTechSlot(CS$<>8__locals1.techRaceSlot);
				}
				if (flag7 || flag8 || flag4)
				{
					CS$<>8__locals1.faction.EndTechRace();
				}
			}
			else if (CS$<>8__locals1.techRaceSlot >= 0)
			{
				if (CS$<>8__locals1.<SetResearchPriorities>g__DoNotRace|5(CS$<>8__locals1.raceTechProgress.techTemplate, CS$<>8__locals1.raceTechProgress.remainingResearch))
				{
					if ((from i in Enumerable.Range(0, 3)
						where i != CS$<>8__locals1.techRaceSlot
						select GameStateManager.GlobalResearch().GetTechProgress(i)).ToList<TechProgress>().All<TechProgress>((TechProgress x) => base.<SetResearchPriorities>g__DoNotRace|5(x.techTemplate, x.remainingResearch)))
					{
						CS$<>8__locals1.faction.GlobalResearchPurse = Mathf.Min(CS$<>8__locals1.faction.GlobalResearchPurse, 500f);
					}
				}
				else if (CS$<>8__locals1.faction.GlobalResearchPurse > 0f)
				{
					CS$<>8__locals1.faction.BeginTechRace(CS$<>8__locals1.techRaceSlot);
				}
			}
			if (CS$<>8__locals1.faction.IsInTechRace)
			{
				TIFactionState expectedWinner = CS$<>8__locals1.raceTechProgress.GetExpectedWinner(true);
				bool flag9 = false;
				for (int j = 0; j < 6; j++)
				{
					int num6 = ((j == CS$<>8__locals1.faction.TechRaceSlot) ? 1 : 0);
					if (num6 != CS$<>8__locals1.faction.GetResearchPriority(j))
					{
						CS$<>8__locals1.faction.playerControl.StartAction(new SetResearchPriorityAction(CS$<>8__locals1.faction, j, num6));
						flag9 = true;
					}
				}
				if (flag9)
				{
					if (!flag6)
					{
						this.FocusCompetitionTechs(CS$<>8__locals1.faction);
						flag6 = true;
					}
					TIFactionState expectedWinner2 = CS$<>8__locals1.raceTechProgress.GetExpectedWinner(true);
					if (expectedWinner != null && expectedWinner2 != null && expectedWinner2 != expectedWinner && expectedWinner2 == CS$<>8__locals1.faction && expectedWinner.GetIntel(expectedWinner2) >= TemplateManager.global.intelToSeeFactionBasicData)
					{
						TINotificationQueueState.LogTechWinnerWarning(expectedWinner, expectedWinner2, CS$<>8__locals1.techRaceSlot);
					}
				}
			}
			else
			{
				int num7 = 0;
				for (int k = 3; k < 6; k++)
				{
					if (CS$<>8__locals1.faction.ProjectAllowedInSlot(k))
					{
						TIProjectTemplate projectInSlot2 = CS$<>8__locals1.faction.GetProjectInSlot(k);
						int num8 = 0;
						if (!AIEvaluators.ShouldSkipProject(projectInSlot2, CS$<>8__locals1.faction))
						{
							if (projectInSlot2.AI_projectRole == ProjectRole.Objective)
							{
								num8 = 3;
							}
							else if (flag3)
							{
								if (!flag4 && projectInSlot2.AI_criticalTech)
								{
									num8 = 1;
								}
							}
							else if (projectInSlot2.AI_criticalTech)
							{
								num8 = 2;
							}
							else if (TIUtilities.RandomFloatValue() < 0.5f)
							{
								num8 = 1;
							}
						}
						CS$<>8__locals1.faction.playerControl.StartAction(new SetResearchPriorityAction(CS$<>8__locals1.faction, k, num8));
						num7 += num8;
					}
				}
				if (CS$<>8__locals1.faction.HasChosenPassiveTechSlot && (TITimeState.Now().day - 1) % 7 == 0)
				{
					if ((from x in Enumerable.Range(0, 3)
						select GameStateManager.GlobalResearch().GetTechProgress(x)).ToList<TechProgress>().Any<TechProgress>(delegate(TechProgress x)
					{
						TITechTemplate techTemplate2 = x.techTemplate;
						return techTemplate2.AI_techRole == TechRole.Blocker || techTemplate2.AI_techRole == TechRole.Competition;
					}))
					{
						CS$<>8__locals1.faction.ClearPassiveTechSlot();
					}
				}
				if (!CS$<>8__locals1.faction.HasChosenPassiveTechSlot)
				{
					CS$<>8__locals1.faction.SetPassiveTechSlot(AIEvaluators.SelectPassiveTechSlot(CS$<>8__locals1.faction));
				}
				int passiveTechSlot = CS$<>8__locals1.faction.PassiveTechSlot;
				TITechTemplate techTemplate = GameStateManager.GlobalResearch().GetTechProgress(passiveTechSlot).techTemplate;
				float num9 = TemplateManager.global.GetPassiveTechInvestmentDifficultyScaling();
				if (techTemplate.AI_criticalTech || CS$<>8__locals1.faction.forcedTechNames.Contains(techTemplate.dataName))
				{
					num9 = Mathf.Min(num9 * 1.5f, 1f);
				}
				if (flag5)
				{
					num9 = Mathf.Min(num9 * 2f, 1f);
				}
				while (num7 > 1 && 3f / (float)(num7 + 3) < num9)
				{
					for (int l = 3; l < 6; l++)
					{
						int num10 = CS$<>8__locals1.faction.researchWeights[l] - 1;
						if (num10 >= 0)
						{
							CS$<>8__locals1.faction.playerControl.StartAction(new SetResearchPriorityAction(CS$<>8__locals1.faction, l, num10));
							num7--;
							if (num7 <= 1)
							{
								break;
							}
						}
					}
				}
				float num11 = (float)num7 * num9 / (1f - num9);
				int num12 = (int)num11;
				if (TIUtilities.RandomFloatValue() < num11 - (float)num12)
				{
					num12++;
				}
				num12 = Mathf.Clamp(num12, 0, 3);
				if (num7 == 0)
				{
					num12 = 1;
				}
				for (int m = 0; m < 3; m++)
				{
					int num13 = ((passiveTechSlot == m) ? num12 : 0);
					if (flag4)
					{
						num13 = 0;
					}
					CS$<>8__locals1.faction.playerControl.StartAction(new SetResearchPriorityAction(CS$<>8__locals1.faction, m, num13));
				}
			}
			if (!flag6)
			{
				this.FocusCompetitionTechs(CS$<>8__locals1.faction);
			}
		}

		// Token: 0x06005A46 RID: 23110 RVA: 0x002A7818 File Offset: 0x002A5A18
		private void FocusCompetitionTechs(TIFactionState faction)
		{
			foreach (ValueTuple<int, TechProgress> valueTuple in from x in Enumerable.Range(0, 3)
				select new ValueTuple<int, TechProgress>(x, GameStateManager.GlobalResearch().GetTechProgress(x)) into x
				where x.Item2.techTemplate.AI_techRole == TechRole.Competition
				select x)
			{
				int researchPriority = faction.GetResearchPriority(valueTuple.Item1);
				faction.playerControl.StartAction(new SetResearchPriorityAction(faction, valueTuple.Item1, researchPriority + 1));
			}
		}

		// Token: 0x06005A47 RID: 23111 RVA: 0x002A78D0 File Offset: 0x002A5AD0
		private void AliensCheckGoals(TIFactionState aliens)
		{
			AIDailyFactionPlanner.<>c__DisplayClass60_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass60_0();
			CS$<>8__locals1.aliens = aliens;
			List<TIFactionGoalState> list = CS$<>8__locals1.aliens.factionGoals[GoalType.InvadeEarth];
			float alienProgressionModifiedDuration_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact();
			if (list.Count == 0 && alienProgressionModifiedDuration_years_exact > TemplateManager.global.GetYearsUntilFirstAlienInvasionDifficultyScaling())
			{
				CS$<>8__locals1.aliens.AddGoal(new FactionGoal_InvadeEarth(GameStateManager.AlienFaction(), 10), HandleDuplicateGoalRule.Ignore, null);
			}
			else if (this.gameTime.currentTime.month == 1 && alienProgressionModifiedDuration_years_exact > TemplateManager.global.GetYearsUntilFirstAlienInvasionDifficultyScaling())
			{
				list.ForEach(delegate(TIFactionGoalState x)
				{
					x.ChangeImportance(1, 1, 20);
				});
				if (list.Count <= ((CS$<>8__locals1.aliens.armiesLost[ArmyType.AlienInvader] - TemplateManager.global.AI_invaderArmiesLostBeforeBuildup > 0) ? 2 : 3))
				{
					CS$<>8__locals1.aliens.AddGoal(new FactionGoal_InvadeEarth(GameStateManager.AlienFaction(), 10), HandleDuplicateGoalRule.Ignore, null);
				}
			}
			if (this.gameTime.currentTime.month % 6 == 0)
			{
				AIDailyFactionPlanner.<>c__DisplayClass60_1 CS$<>8__locals2;
				CS$<>8__locals2.outerBaseBuildGoals = CS$<>8__locals1.aliens.factionGoals[GoalType.FoundBase].Where<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_spaceBody.GetSunOrbitingRelatedObject.semiMajorAxis_AU >= 5.0 && x.target().ref_spaceBody.GetSunOrbitingRelatedObject.semiMajorAxis_AU <= 35.0);
				if (!CS$<>8__locals1.<AliensCheckGoals>g__SpaceBodyCovered|24(GameStateManager.Neptune(), ref CS$<>8__locals2))
				{
					TIHabSiteState tihabSiteState = LegacyHabPlanner.SelectHabSiteForDevelopment(CS$<>8__locals1.aliens, GameStateManager.Neptune(), new List<TIHabSiteState>(), true, false, 3, false, null);
					if (tihabSiteState != null)
					{
						CS$<>8__locals1.aliens.AddGoal(new FactionGoal_FoundBase(CS$<>8__locals1.aliens, 17, tihabSiteState, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
					}
				}
				else if (!CS$<>8__locals1.<AliensCheckGoals>g__SpaceBodyCovered|24(GameStateManager.Uranus(), ref CS$<>8__locals2))
				{
					TIHabSiteState tihabSiteState2 = LegacyHabPlanner.SelectHabSiteForDevelopment(CS$<>8__locals1.aliens, GameStateManager.Uranus(), new List<TIHabSiteState>(), true, false, 3, false, null);
					if (tihabSiteState2 != null)
					{
						CS$<>8__locals1.aliens.AddGoal(new FactionGoal_FoundBase(CS$<>8__locals1.aliens, 17, tihabSiteState2, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
					}
				}
				else if (!CS$<>8__locals1.<AliensCheckGoals>g__SpaceBodyCovered|24(GameStateManager.Saturn(), ref CS$<>8__locals2))
				{
					TIHabSiteState tihabSiteState3 = LegacyHabPlanner.SelectHabSiteForDevelopment(CS$<>8__locals1.aliens, GameStateManager.Saturn(), new List<TIHabSiteState>(), true, false, 3, false, null);
					if (tihabSiteState3 != null)
					{
						CS$<>8__locals1.aliens.AddGoal(new FactionGoal_FoundBase(CS$<>8__locals1.aliens, 17, tihabSiteState3, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
					}
				}
			}
			List<FactionGoal_FoundSurveillanceStation> list2 = CS$<>8__locals1.aliens.factionGoals[GoalType.FoundSurveillanceStation].ConvertAll<FactionGoal_FoundSurveillanceStation>((TIFactionGoalState x) => x as FactionGoal_FoundSurveillanceStation);
			List<TIHabState> list3 = CS$<>8__locals1.aliens.habs.Where<TIHabState>(delegate(TIHabState x)
			{
				if (x.GetSunOrbitingRelatedObject.isEarth)
				{
					return x.AllModuleStates().Any<TIHabModuleState>(delegate(TIHabModuleState x)
					{
						TIHabModuleTemplate moduleTemplate = x.moduleTemplate;
						return moduleTemplate != null && moduleTemplate.specialRules.Contains(HabModuleSpecialRule.AlienSurveillance);
					});
				}
				return false;
			}).ToList<TIHabState>();
			if (list2.Count + list3.Count < 3)
			{
				if (alienProgressionModifiedDuration_years_exact > Mathf.Max(3f, TemplateManager.global.GetYearsUntilFirstAlienInvasionDifficultyScaling() + 1f))
				{
					if (list3.None<TIHabState>((TIHabState x) => x.tier == 3))
					{
						if (list2.None<FactionGoal_FoundSurveillanceStation>((FactionGoal_FoundSurveillanceStation x) => x.tier >= 3))
						{
							List<TIOrbitState> list4 = FactionGoal_FoundSurveillanceStation.candidateOrbits(3);
							if (list4.Count > 0)
							{
								CS$<>8__locals1.aliens.AddGoal(new FactionGoal_FoundSurveillanceStation(CS$<>8__locals1.aliens, 17, list4.MaxBy<TIOrbitState, double>(delegate(TIOrbitState x)
								{
									if (!x.barycenter.isEarth)
									{
										return x.barycenter.semiMajorAxis_km + x.semiMajorAxis_km;
									}
									return x.semiMajorAxis_km;
								}), GoalType.DefendWithFleet, 3), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
								goto IL_060A;
							}
							goto IL_060A;
						}
					}
				}
				if (alienProgressionModifiedDuration_years_exact > Mathf.Max(2f, TemplateManager.global.GetYearsUntilFirstAlienInvasionDifficultyScaling() - 1f))
				{
					if (list3.None<TIHabState>((TIHabState x) => x.tier >= 2))
					{
						if (list2.None<FactionGoal_FoundSurveillanceStation>((FactionGoal_FoundSurveillanceStation x) => x.tier >= 2))
						{
							List<TIOrbitState> list5 = FactionGoal_FoundSurveillanceStation.candidateOrbits(2);
							if (list5.Any<TIOrbitState>((TIOrbitState x) => !x.interfaceOrbit))
							{
								list5 = list5.Where<TIOrbitState>((TIOrbitState x) => !x.interfaceOrbit).ToList<TIOrbitState>();
							}
							if (list5.Count > 0)
							{
								CS$<>8__locals1.aliens.AddGoal(new FactionGoal_FoundSurveillanceStation(CS$<>8__locals1.aliens, 17, list5.SelectRandomWeightedItem<TIOrbitState>((TIOrbitState x) => (float)(1.0 - x.semiMajorAxis_AU), -1f, 1E-37f), GoalType.DefendWithFleet, 2), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
								goto IL_060A;
							}
							goto IL_060A;
						}
					}
				}
				if (alienProgressionModifiedDuration_years_exact > Mathf.Max(1f, TemplateManager.global.GetYearsUntilFirstAlienInvasionDifficultyScaling() - 3f) && list3.Count == 0 && list2.Count == 0)
				{
					List<TIOrbitState> list6 = FactionGoal_FoundSurveillanceStation.candidateOrbits(1);
					if (list6.Any<TIOrbitState>((TIOrbitState x) => !x.interfaceOrbit))
					{
						list6 = list6.Where<TIOrbitState>((TIOrbitState x) => !x.interfaceOrbit).ToList<TIOrbitState>();
					}
					if (list6.Count > 0)
					{
						CS$<>8__locals1.aliens.AddGoal(new FactionGoal_FoundSurveillanceStation(CS$<>8__locals1.aliens, 17, list6.MinBy<TIOrbitState, double>((TIOrbitState x) => x.semiMajorAxis_m), GoalType.DefendWithFleet, 1), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					}
				}
			}
			else if (this.gameTime.currentTime.month == 1)
			{
				foreach (FactionGoal_FoundSurveillanceStation factionGoal_FoundSurveillanceStation in list2)
				{
					if (factionGoal_FoundSurveillanceStation.importance < 18)
					{
						factionGoal_FoundSurveillanceStation.SetImportance(factionGoal_FoundSurveillanceStation.importance + 1);
					}
				}
			}
			IL_060A:
			if (alienProgressionModifiedDuration_years_exact > Mathf.Max(3f, TemplateManager.global.GetYearsUntilFirstAlienInvasionDifficultyScaling() - 3f))
			{
				CS$<>8__locals1.aliens.AddGoal(new FactionGoal_SecureEarthSpace(CS$<>8__locals1.aliens, 10), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
			HabPlanner.GetPlanner(CS$<>8__locals1.aliens).ManageHabGoals(CS$<>8__locals1.aliens);
			CS$<>8__locals1.impudentSpaceAssets = (from x in GameStateManager.AllHumanFactions()
				where !x.isAlienAppeaser && !x.IsAlienProxy && !CS$<>8__locals1.aliens.HasNAP(x, true)
				select x).SelectMany<TIFactionState, TISpaceAssetState>((TIFactionState x) => Enumerable.Empty<TISpaceAssetState>().Union<TISpaceAssetState>(x.habs).Union<TISpaceAssetState>(x.fleets)).Where<TISpaceAssetState>(delegate(TISpaceAssetState x)
			{
				if (CS$<>8__locals1.aliens.IsTrespassing(x))
				{
					return true;
				}
				TIFactionState faction = x.faction;
				if (faction != null && faction.UnlockedAntimatter && x.isHabState)
				{
					List<TIHabModuleState> list9 = (from y in x.ref_hab.ActiveModules()
						where y.moduleTemplate.incomeAntimatter_month > 0f
						where !y.moduleTemplate.specialRules.Contains(HabModuleSpecialRule.HarvestAntimatter)
						select y).ToList<TIHabModuleState>();
					if (list9.Any<TIHabModuleState>())
					{
						if (list9.Any<TIHabModuleState>((TIHabModuleState x) => !x.underConstruction) || TIUtilities.RandomFloatValue() < 0.05f)
						{
							return true;
						}
					}
				}
				return false;
			}).ToList<TISpaceAssetState>();
			CS$<>8__locals1.existingAttackGoals = (from x in CS$<>8__locals1.aliens.GoalsOfType(GoalType.AttackWithFleet, false, true)
				select x as FactionGoal_AttackWithFleet into x
				where CS$<>8__locals1.impudentSpaceAssets.Contains(x.target())
				select x).ToList<FactionGoal_AttackWithFleet>();
			List<FactionGoal_AttackWithFleet> list7 = CS$<>8__locals1.existingAttackGoals.Where<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet x) => x.assignedFleet != null && x.assignedFleet.inTransfer).ToList<FactionGoal_AttackWithFleet>();
			CS$<>8__locals1.impudentSpaceAssets = CS$<>8__locals1.impudentSpaceAssets.Except<TISpaceAssetState>(list7.Select<FactionGoal_AttackWithFleet, TISpaceAssetState>((FactionGoal_AttackWithFleet x) => x.target() as TISpaceAssetState)).ToList<TISpaceAssetState>();
			CS$<>8__locals1.impudentSystems = (from x in CS$<>8__locals1.impudentSpaceAssets
				where x.GetFutureSystem() != null
				group x by x.GetFutureSystem()).ToDictionary<IGrouping<TISpaceBodyState, TISpaceAssetState>, TISpaceBodyState, List<TISpaceAssetState>>((IGrouping<TISpaceBodyState, TISpaceAssetState> x) => x.Key, (IGrouping<TISpaceBodyState, TISpaceAssetState> x) => x.ToList<TISpaceAssetState>());
			List<ValueTuple<TISpaceAssetState, int>> list8 = (from x in CS$<>8__locals1.impudentSystems.Keys.SelectMany<TISpaceBodyState, ValueTuple<TISpaceAssetState, int>>((TISpaceBodyState system) => CS$<>8__locals1.impudentSystems[system].Select<TISpaceAssetState, ValueTuple<TISpaceAssetState, int>>(delegate(TISpaceAssetState spaceAsset)
				{
					int num = 15;
					if (system.objectType != SpaceObjectType.Planet)
					{
						num -= 2;
					}
					if (system.habSitesInSystem.Count < 4)
					{
						num--;
					}
					int num2 = (int)(2.0 * (system.semiMajorAxis_AU - GameStateManager.Jupiter().semiMajorAxis_AU) / (CS$<>8__locals1.aliens.primaryHab.ref_system.semiMajorAxis_AU - GameStateManager.Jupiter().semiMajorAxis_AU));
					num += Mathf.Clamp(num2, -1, 1) - 1;
					bool flag5 = false;
					if (AIEvaluators.ShouldLaunchEmergencyAttackAgainstAsset(CS$<>8__locals1.aliens, spaceAsset, false))
					{
						bool flag6 = false;
						if (spaceAsset.isHabState)
						{
							flag6 = CS$<>8__locals1.existingAttackGoals.Any<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet x) => x.importance == 20 && x.target().isHabState && x.target().ref_hab.IsStation == spaceAsset.ref_hab.IsStation && x.target().ref_system == system);
						}
						if (!flag6)
						{
							num = 20;
							flag5 = true;
						}
					}
					else if (spaceAsset.isHabState)
					{
						num += 4;
						if (spaceAsset.ref_hab.ShipsBeingBuiltAtHab(spaceAsset.ref_hab.faction).Count > 0)
						{
							num++;
						}
					}
					else if (spaceAsset.isSpaceFleetState)
					{
						float num3 = AIEvaluators.FactionsGoToWarProgress(CS$<>8__locals1.aliens, spaceAsset.ref_faction);
						num += 2 + (num3 * 1f).Round();
					}
					num = Mathf.Min(num, 20 - (flag5 ? 0 : 1));
					return new ValueTuple<TISpaceAssetState, int>(spaceAsset, num);
				}))
				orderby CS$<>8__locals1.existingAttackGoals.Any<FactionGoal_AttackWithFleet>((FactionGoal_AttackWithFleet y) => y.target() == x.Item1) descending, x.Item2 descending, AIDailyFactionPlanner.<AliensCheckGoals>g__GetMiscSortValue|60_18(x.Item1) descending
				select x).ToList<ValueTuple<TISpaceAssetState, int>>();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			int i = 0;
			while (i < list8.Count)
			{
				ValueTuple<TISpaceAssetState, int> valueTuple = list8[i];
				TISpaceAssetState item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				bool flag4 = item2 == 20;
				if (flag4 || !flag || !flag2 || !flag3)
				{
					if (item.isSpaceFleetState)
					{
						if (!flag || flag4)
						{
							flag = true;
							goto IL_091D;
						}
					}
					else if (item.isHabState && item.ref_hab.IsBase)
					{
						if (!flag2 || flag4)
						{
							flag2 = true;
							goto IL_091D;
						}
					}
					else
					{
						if (!item.isHabState || !item.ref_hab.IsStation)
						{
							goto IL_091D;
						}
						if (!flag3 || flag4)
						{
							flag3 = true;
							goto IL_091D;
						}
					}
					IL_093D:
					i++;
					continue;
					IL_091D:
					CS$<>8__locals1.aliens.AddGoal(new FactionGoal_AttackWithFleet(CS$<>8__locals1.aliens, item2, item, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					goto IL_093D;
				}
				break;
			}
			if (CS$<>8__locals1.impudentSpaceAssets.Count<TISpaceAssetState>() == 0)
			{
				this.CheckTriggerInnerSystemOffensives(CS$<>8__locals1.aliens);
			}
			HabPlanner.GetPlanner(CS$<>8__locals1.aliens).FoundHabs(CS$<>8__locals1.aliens);
		}

		// Token: 0x06005A48 RID: 23112 RVA: 0x002A8270 File Offset: 0x002A6470
		private static void CreateAlienBaseFoundingGoal(TISpaceBodyState system, int importance)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			TIHabSiteState tihabSiteState = LegacyHabPlanner.SelectHabSiteForDevelopment(tifactionState, GameStateManager.Jupiter(), new List<TIHabSiteState>(), true, false, 3, true, AlienHabPlanner.GetEstimatedFutureIncomeFunctionForPurposeOfHabSiteSelection(tifactionState));
			if (tihabSiteState != null)
			{
				tifactionState.AddGoal(new FactionGoal_FoundBase(tifactionState, importance, tihabSiteState, GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.Ignore, null);
			}
		}

		// Token: 0x06005A49 RID: 23113 RVA: 0x002A82C0 File Offset: 0x002A64C0
		private void CheckTriggerInnerSystemOffensives(TIFactionState aliens)
		{
			if ((float)TITimeState.CampaignDuration_CompleteYears() > TIGlobalConfig.globalConfig.GetCampaignDurationBeforeAlienInnerSystemOffensives())
			{
				IEnumerable<TIHabState> enumerable = from x in GameStateManager.IterateByClass<TIHabState>(false)
					where x.ref_spaceBody != null && (x.tier == 3 || x.ref_spaceBody.habSites.Length > 1) && !x.faction.veryProAlien && !aliens.HasNAP(x.faction, true) && x.GetSunOrbitingRelatedObject.semiMajorAxis_AU < GameStateManager.Jupiter().semiMajorAxis_AU
					select x;
				IEnumerable<TIHabState> enumerable2 = enumerable.Where<TIHabState>((TIHabState x) => GameStateManager.FullAsteroidBelt(true).Contains(x.ref_spaceBody));
				Func<TIHabSiteState, float> <>9__4;
				foreach (TIHabState tihabState in (from x in enumerable2
					orderby x.tier descending, x.GetSunOrbitingRelatedObject.semiMajorAxis_AU descending
					select x).Take<TIHabState>(3))
				{
					int num = 10 + 2 * tihabState.tier;
					aliens.AddGoal(new FactionGoal_AttackWithFleet(aliens, num, tihabState, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					if (tihabState.ref_spaceBody.habSites.Length > 1)
					{
						TIFactionState aliens2 = aliens;
						TIFactionState aliens3 = aliens;
						int num2 = num;
						IEnumerable<TIHabSiteState> habSites = tihabState.ref_spaceBody.habSites;
						Func<TIHabSiteState, float> func;
						if ((func = <>9__4) == null)
						{
							func = (<>9__4 = (TIHabSiteState x) => AIEvaluators.EvaluateHabSite(aliens, x, false, false, true));
						}
						aliens2.AddGoal(new FactionGoal_FoundBase(aliens3, num2, habSites.MaxBy<TIHabSiteState, float>(func), GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					}
				}
				if (enumerable2.Count<TIHabState>() == 0)
				{
					IEnumerable<TIHabState> enumerable3 = enumerable.Where<TIHabState>((TIHabState x) => x.ref_naturalSpaceObject == GameStateManager.Mars() || x.ref_spaceBody.innerSystemAsteroid(true));
					Func<TIHabSiteState, float> <>9__8;
					foreach (TIHabState tihabState2 in (from x in enumerable3
						orderby x.tier descending, x.GetSunOrbitingRelatedObject == GameStateManager.Mars()
						select x).Take<TIHabState>(3))
					{
						int num3 = 10 + 2 * tihabState2.tier;
						aliens.AddGoal(new FactionGoal_AttackWithFleet(aliens, num3, tihabState2, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						if (tihabState2.ref_spaceBody.habSites.Length > 1)
						{
							TIFactionState aliens4 = aliens;
							TIFactionState aliens5 = aliens;
							int num4 = num3;
							IEnumerable<TIHabSiteState> habSites2 = tihabState2.ref_spaceBody.habSites;
							Func<TIHabSiteState, float> func2;
							if ((func2 = <>9__8) == null)
							{
								func2 = (<>9__8 = (TIHabSiteState x) => AIEvaluators.EvaluateHabSite(aliens, x, false, false, true));
							}
							aliens4.AddGoal(new FactionGoal_FoundBase(aliens5, num4, habSites2.MaxBy<TIHabSiteState, float>(func2), GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
						}
					}
					if (enumerable3.Count<TIHabState>() == 0)
					{
						Func<TIHabSiteState, float> <>9__12;
						foreach (TIHabState tihabState3 in (from x in enumerable
							where x.ref_spaceBody.isLuna
							orderby x.tier descending, x.ref_spaceBody.isLuna
							select x).Take<TIHabState>(3))
						{
							int num5 = 10 + 2 * tihabState3.tier;
							aliens.AddGoal(new FactionGoal_AttackWithFleet(aliens, num5, tihabState3, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
							if (tihabState3.ref_spaceBody.habSites.Length > 1)
							{
								TIFactionState aliens6 = aliens;
								TIFactionState aliens7 = aliens;
								int num6 = num5;
								IEnumerable<TIHabSiteState> habSites3 = tihabState3.ref_spaceBody.habSites;
								Func<TIHabSiteState, float> func3;
								if ((func3 = <>9__12) == null)
								{
									func3 = (<>9__12 = (TIHabSiteState x) => AIEvaluators.EvaluateHabSite(aliens, x, false, false, true));
								}
								aliens6.AddGoal(new FactionGoal_FoundBase(aliens7, num6, habSites3.MaxBy<TIHabSiteState, float>(func3), GoalType.BuildFullBase, null, GoalType.BuildFullStation, false, null), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
							}
						}
						List<TIFactionGoalState> list = aliens.GoalsOfType(GoalType.SecureEarthSpace, false, true);
						if (list.Count > 0)
						{
							using (List<TIFactionGoalState>.Enumerator enumerator2 = list.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									TIFactionGoalState tifactionGoalState = enumerator2.Current;
									tifactionGoalState.SetImportance(Mathf.Min(tifactionGoalState.importance + 1, 19));
								}
								return;
							}
						}
						aliens.AddGoal(new FactionGoal_SecureEarthSpace(aliens, 15), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					}
				}
			}
		}

		// Token: 0x06005A4A RID: 23114 RVA: 0x002A8758 File Offset: 0x002A6958
		public static void LaunchDeployArmyOperation(TIArmyState army, TIGameState destination)
		{
			army.AI_targetEnemyRegion = null;
			AIDailyFactionPlanner.LaunchOperation(army, new DeployArmyOperation_OpenTarget(false), destination, null);
		}

		// Token: 0x06005A4B RID: 23115 RVA: 0x002A876F File Offset: 0x002A696F
		public static void LaunchOperation(TIGameState actorState, IOperation operation, TIGameState target, TIResourcesCost cost = null)
		{
			if (cost != null)
			{
				actorState.ref_faction.playerControl.StartAction(new ConfirmOperationAction(actorState, target, operation, cost, null));
				return;
			}
			actorState.ref_faction.playerControl.StartAction(new ConfirmOperationAction(actorState, target, operation, null, null));
		}

		// Token: 0x06005A4C RID: 23116 RVA: 0x002A87A9 File Offset: 0x002A69A9
		private void BuildSpaceAssets(TIFactionState faction)
		{
			CoroutineDummy.Singleton.StartCoroutine(this.BuildSpaceAssetsCo(faction));
		}

		// Token: 0x06005A4D RID: 23117 RVA: 0x002A87C0 File Offset: 0x002A69C0
		private static void ProspectSites(TIFactionState faction)
		{
			TIFactionGoalState tifactionGoalState = (from x in faction.GoalsOfType(GoalType.ProspectSites, false, true)
				where !x.InProgress()
				select x).OrderBy<TIFactionGoalState, int>(delegate(TIFactionGoalState x)
			{
				if (!x.location().ref_spaceBody.isLuna)
				{
					return 1;
				}
				return 0;
			}).FirstOrDefault<TIFactionGoalState>();
			if (tifactionGoalState != null)
			{
				LaunchProbeOperation launchProbeOperation = new LaunchProbeOperation();
				if (launchProbeOperation.OpVisibleToActor(faction, tifactionGoalState.target().ref_spaceBody) && launchProbeOperation.ActorCanPerformOperation(faction, tifactionGoalState.target().ref_spaceBody))
				{
					List<TIResourcesCost> list = (from x in launchProbeOperation.ResourceCostOptions(faction, tifactionGoalState.target(), faction, true)
						orderby x.completionTime_days
						select x).ToList<TIResourcesCost>();
					TIResourcesCost tiresourcesCost;
					if (list.Count == 1)
					{
						tiresourcesCost = list[0];
					}
					else if (Mathf.Abs(list[0].completionTime_days - list[0].completionTime_days) < 365f)
					{
						tiresourcesCost = list.MinBy<TIResourcesCost, float>((TIResourcesCost x) => x.GetSingleCostValue(FactionResource.Boost));
					}
					else
					{
						tiresourcesCost = list.MinBy<TIResourcesCost, float>((TIResourcesCost x) => x.completionTime_days);
					}
					float singleCostValue = tiresourcesCost.GetSingleCostValue(FactionResource.Boost);
					float num = float.PositiveInfinity;
					if (faction.boostAccounts[TIFactionState.BoostAccountName.Probe] != null)
					{
						num = (float)(TITimeState.Now() - faction.boostAccounts[TIFactionState.BoostAccountName.Probe]).TotalDays;
					}
					float num2 = 1f;
					if (singleCostValue > 0f)
					{
						float rateLimitedBoostSpendFraction_Probe = faction.GetRateLimitedBoostSpendFraction_Probe();
						num2 = AIEvaluators.GetDaysToWaitForRateLimitedBoostPurchase(faction, rateLimitedBoostSpendFraction_Probe, singleCostValue);
					}
					if (num >= num2)
					{
						TIFactionState.LogAI(faction.displayName + " launching probe to " + tifactionGoalState.target().displayName, false);
						AIDailyFactionPlanner.LaunchOperation(faction, launchProbeOperation, tifactionGoalState.target(), tiresourcesCost);
						if (singleCostValue > 0f)
						{
							faction.boostAccounts[TIFactionState.BoostAccountName.Probe] = TITimeState.Now();
						}
					}
				}
			}
		}

		// Token: 0x06005A4E RID: 23118 RVA: 0x002A89DC File Offset: 0x002A6BDC
		private IEnumerator BuildSpaceAssetsCo(TIFactionState faction)
		{
			AIDailyFactionPlanner.<>c__DisplayClass69_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass69_0();
			CS$<>8__locals1.faction = faction;
			int day = this.gameTime.currentTime.day;
			if (this.BuildSpaceAssetsBusy)
			{
				yield return null;
			}
			this.BuildSpaceAssetsBusy = true;
			if (day <= 28 && day % 7 == AIDailyFactionPlanner.factionAIData[CS$<>8__locals1.faction].every7DaysOffset)
			{
				bool flag = day % 2 == 0;
				bool flag2 = !flag;
				List<FactionGoal_Fleet> list = new List<FactionGoal_Fleet>();
				foreach (FactionGoal_Fleet factionGoal_Fleet in CS$<>8__locals1.faction.AllFleetGoals(true))
				{
					TISpaceFleetState assignedFleet = factionGoal_Fleet.assignedFleet;
					if (assignedFleet != null && assignedFleet.deleted)
					{
						factionGoal_Fleet.UnassignFleet();
						Log.Error("Unassigning illegal fleet from " + factionGoal_Fleet.ID.ToString() + ". Please provide upload last 3 autosaves to issue 1842 on Github.", Array.Empty<object>());
					}
					list.Add(factionGoal_Fleet);
				}
				List<FactionGoal_BuildHab> list2 = (from x in CS$<>8__locals1.faction.GoalsOfType(TIFactionGoalState.BuildHabGoals, false, true).ConvertAll<FactionGoal_BuildHab>((TIFactionGoalState x) => x as FactionGoal_BuildHab)
					where !x.skipGoal
					orderby x.importance descending, x.assignedDate
					select x).ToList<FactionGoal_BuildHab>();
				list = (from x in list
					orderby x.importance descending, x.assignedDate
					select x).ToList<FactionGoal_Fleet>();
				if (CS$<>8__locals1.faction.AISavingTarget.active)
				{
					if (!(CS$<>8__locals1.faction.AISavingTarget.location == null) && (!(CS$<>8__locals1.faction.AISavingTarget.location.ref_hab == null) || CS$<>8__locals1.faction.AISavingTarget.relatedGoal.FoundHabGoal()) && !(CS$<>8__locals1.faction.AISavingTarget.location.ref_faction != CS$<>8__locals1.faction))
					{
						TIHabState ref_hab = CS$<>8__locals1.faction.AISavingTarget.location.ref_hab;
						if (ref_hab == null || !ref_hab.archived)
						{
							TIHabState ref_hab2 = CS$<>8__locals1.faction.AISavingTarget.location.ref_hab;
							if (ref_hab2 == null || !ref_hab2.deleted)
							{
								if (!CS$<>8__locals1.faction.factionGoals[CS$<>8__locals1.faction.AISavingTarget.relatedGoal.GetGoalType()].Contains(CS$<>8__locals1.faction.AISavingTarget.relatedGoal) || CS$<>8__locals1.faction.AISavingTarget.relatedGoal.GoalFulfilled())
								{
									CS$<>8__locals1.faction.AIClearSavingTarget("Goal gone");
									goto IL_04B1;
								}
								TISpaceShipTemplate tispaceShipTemplate = CS$<>8__locals1.faction.AISavingTarget.desiredPurchase as TISpaceShipTemplate;
								if (tispaceShipTemplate == null)
								{
									goto IL_04B1;
								}
								if (CS$<>8__locals1.faction.AISavingTarget.location.ref_habModule == null || CS$<>8__locals1.faction.AISavingTarget.location.ref_habModule.archived || !CS$<>8__locals1.faction.AISavingTarget.location.ref_habModule.active)
								{
									CS$<>8__locals1.faction.AIClearSavingTarget("shipyard gone");
									goto IL_04B1;
								}
								if (CS$<>8__locals1.faction.obsoleteShipDesigns.Contains(tispaceShipTemplate.dataName))
								{
									CS$<>8__locals1.faction.AIClearSavingTarget("obsolete ship design");
									goto IL_04B1;
								}
								goto IL_04B1;
							}
						}
					}
					CS$<>8__locals1.faction.AIClearSavingTarget("Hab gone");
					IL_04B1:
					FactionGoal_Fleet factionGoal_Fleet2 = CS$<>8__locals1.faction.AISavingTarget.relatedGoal as FactionGoal_Fleet;
					if (factionGoal_Fleet2 != null && factionGoal_Fleet2.buildFleetsSequentially && !factionGoal_Fleet2.IsFrontGoal)
					{
						CS$<>8__locals1.faction.AIClearSavingTarget("Sequential fleet goal had non-front goal as saving target");
					}
				}
				if (!CS$<>8__locals1.faction.AISavingTarget.CanSaveFor)
				{
					CS$<>8__locals1.faction.AIClearSavingTarget("No income on one or more of the costs.");
				}
				if (!CS$<>8__locals1.faction.AISavingTarget.active || (day <= 28 && this.gameTime.currentTime.day % 14 == AIDailyFactionPlanner.factionAIData[CS$<>8__locals1.faction].every14DaysOffset))
				{
					CS$<>8__locals1.faction.SetResourceIncomeDataDirty();
					TIDataTemplate tidataTemplate = null;
					FactionGoal_BuildHab factionGoal_BuildHab = null;
					int num = (CS$<>8__locals1.faction.AISavingTarget.active ? CS$<>8__locals1.faction.AISavingTarget.importance : (-1));
					foreach (FactionGoal_BuildHab factionGoal_BuildHab2 in list2)
					{
						if (!factionGoal_BuildHab2.ShouldPauseGoal() && factionGoal_BuildHab2.importance > num)
						{
							bool flag3;
							TIHabModuleState tihabModuleState;
							TIDataTemplate tidataTemplate2 = factionGoal_BuildHab2.SavingForTemplate(CS$<>8__locals1.faction, out flag3, out tihabModuleState);
							if (tidataTemplate2 != null)
							{
								tidataTemplate = tidataTemplate2;
								factionGoal_BuildHab = factionGoal_BuildHab2;
								num = factionGoal_BuildHab2.importance;
								if (num >= 20)
								{
									break;
								}
							}
						}
					}
					TIDataTemplate tidataTemplate3 = null;
					FactionGoal_Fleet factionGoal_Fleet3 = null;
					TIHabModuleState tihabModuleState2 = null;
					if (CS$<>8__locals1.faction.nShipyardQueues.Count > 0)
					{
						foreach (FactionGoal_Fleet factionGoal_Fleet4 in list)
						{
							if ((!factionGoal_Fleet4.buildFleetsSequentially || factionGoal_Fleet4.IsFrontGoal) && factionGoal_Fleet4.importance >= num && factionGoal_Fleet4.importance > ((factionGoal_Fleet3 != null) ? factionGoal_Fleet3.importance : (-1)) && !factionGoal_Fleet4.ShouldPauseGoal())
							{
								bool flag4;
								TIHabModuleState tihabModuleState3;
								TISpaceShipTemplate tispaceShipTemplate2 = factionGoal_Fleet4.SavingForTemplate(CS$<>8__locals1.faction, out flag4, out tihabModuleState3) as TISpaceShipTemplate;
								if (tispaceShipTemplate2 != null)
								{
									bool flag5 = false;
									if (CS$<>8__locals1.faction.IsAlienFaction && tispaceShipTemplate2.combatant && !AIEvaluators.ShouldAliensGoLoud())
									{
										flag5 = true;
									}
									bool flag6 = false;
									bool flag7 = false;
									if (flag4)
									{
										flag6 = true;
									}
									else
									{
										TIFactionState.ShipyardAISearchResult shipyardAISearchResult;
										tihabModuleState3 = CS$<>8__locals1.faction.AI_GetBestShipyardForBuild(tispaceShipTemplate2, factionGoal_Fleet4.location(), factionGoal_Fleet4, out shipyardAISearchResult, out flag7, true, 1f, true, true);
										if (shipyardAISearchResult == TIFactionState.ShipyardAISearchResult.Success)
										{
											flag6 = true;
										}
									}
									if (flag6 && !flag5)
									{
										TIResourcesCost tiresourcesCost;
										if (flag7)
										{
											tiresourcesCost = TISpaceShipTemplate.MixedResourceConstructionCost(CS$<>8__locals1.faction, tihabModuleState3.ref_hab, tispaceShipTemplate2.spaceResourceConstructionCost(false, tihabModuleState3, true, false, false), null, false);
										}
										else
										{
											tiresourcesCost = tispaceShipTemplate2.spaceResourceConstructionCost(false, tihabModuleState3, true, false, false);
										}
										if (tiresourcesCost.CanAfford_AI(CS$<>8__locals1.faction, tispaceShipTemplate2, tihabModuleState3, factionGoal_Fleet4.importance, false, false, 1f, null, float.PositiveInfinity) || tiresourcesCost.CanPayInFuture(CS$<>8__locals1.faction, 360))
										{
											factionGoal_Fleet3 = factionGoal_Fleet4;
											num = factionGoal_Fleet3.importance;
											tihabModuleState2 = tihabModuleState3;
											tidataTemplate3 = tispaceShipTemplate2;
											if (num >= 20)
											{
												break;
											}
										}
									}
								}
							}
						}
					}
					int num2 = ((factionGoal_BuildHab != null) ? factionGoal_BuildHab.importance : (-1));
					int num3 = ((factionGoal_Fleet3 != null) ? factionGoal_Fleet3.importance : (-1));
					if (factionGoal_BuildHab != null && (factionGoal_Fleet3 == null || num2 > num3 || (num2 == num3 && !(factionGoal_Fleet3 is FactionGoal_FoundHab))))
					{
						if (CS$<>8__locals1.faction.AISavingTarget.active && num2 > CS$<>8__locals1.faction.AISavingTarget.importance)
						{
							CS$<>8__locals1.faction.AIClearSavingTarget("Existing importance exceeded: Hab");
						}
						if (!CS$<>8__locals1.faction.AISavingTarget.active)
						{
							CS$<>8__locals1.faction.AISetSavingTarget(tidataTemplate, factionGoal_BuildHab.hab, factionGoal_BuildHab);
						}
					}
					else if (factionGoal_Fleet3 != null)
					{
						if (CS$<>8__locals1.faction.AISavingTarget.active && num3 > CS$<>8__locals1.faction.AISavingTarget.importance)
						{
							CS$<>8__locals1.faction.AIClearSavingTarget("Existing importance exceeded: Fleet");
						}
						if (!CS$<>8__locals1.faction.AISavingTarget.active)
						{
							CS$<>8__locals1.faction.AISetSavingTarget(tidataTemplate3, tihabModuleState2, factionGoal_Fleet3);
						}
					}
				}
				AIDailyFactionPlanner.ProspectSites(CS$<>8__locals1.faction);
				CS$<>8__locals1.faction.SetResourceIncomeDataDirty();
				if (flag2 || flag)
				{
					HabPlanner.GetPlanner(CS$<>8__locals1.faction).FoundHabs(CS$<>8__locals1.faction);
				}
				if (flag)
				{
					HabPlanner.GetPlanner(CS$<>8__locals1.faction).ManageHabs(CS$<>8__locals1.faction);
				}
				if (flag2)
				{
					if (CS$<>8__locals1.faction.updateShipDesignsFlag)
					{
						AIDailyFactionPlanner.DesignShips(CS$<>8__locals1.faction, new Action(CS$<>8__locals1.<BuildSpaceAssetsCo>g__UpdateShipyardQueues|6));
					}
					else
					{
						CS$<>8__locals1.<BuildSpaceAssetsCo>g__UpdateShipyardQueues|6();
					}
				}
			}
			this.BuildSpaceAssetsBusy = false;
			yield break;
		}

		// Token: 0x06005A4F RID: 23119 RVA: 0x002A89F4 File Offset: 0x002A6BF4
		public static int AdjustShipyardQueue(TIFactionState faction, ShipConstructionQueueItem item)
		{
			AIDailyFactionPlanner.<>c__DisplayClass70_0 CS$<>8__locals1;
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.favorNoncombatants = !AIEvaluators.IsSystemContested(CS$<>8__locals1.faction, item.shipyard);
			List<ShipConstructionQueueItem> list = CS$<>8__locals1.faction.nShipyardQueues[item.shipyard];
			int num = list.IndexOf(item);
			int num2 = num;
			int num3 = 0;
			if (item.AIFactionGoal != null)
			{
				num3 = item.AIFactionGoal.importance;
			}
			bool flag = AIDailyFactionPlanner.<AdjustShipyardQueue>g__IsCriticalBuild|70_0(item, ref CS$<>8__locals1);
			bool flag2 = CS$<>8__locals1.faction.AISavingTarget.active && item.AIFactionGoal == CS$<>8__locals1.faction.AISavingTarget.relatedGoal;
			int num4 = num3;
			if (CS$<>8__locals1.favorNoncombatants && item.shipDesign.nonCombatant)
			{
				num4 += 20;
			}
			for (int i = 0; i < num; i++)
			{
				ShipConstructionQueueItem shipConstructionQueueItem = list[i];
				int num5 = 0;
				if (shipConstructionQueueItem.AIFactionGoal != null)
				{
					num5 = shipConstructionQueueItem.AIFactionGoal.importance;
				}
				if (CS$<>8__locals1.favorNoncombatants && shipConstructionQueueItem.shipDesign.nonCombatant)
				{
					num5 += 20;
				}
				bool flag3 = AIDailyFactionPlanner.<AdjustShipyardQueue>g__IsCriticalBuild|70_0(shipConstructionQueueItem, ref CS$<>8__locals1);
				if (shipConstructionQueueItem.costPaid != item.costPaid)
				{
					if (item.costPaid)
					{
						num2 = i;
						break;
					}
				}
				else if (flag)
				{
					if (!flag3 || flag2)
					{
						num2 = i;
						break;
					}
				}
				else if (item.isRefit)
				{
					if (!shipConstructionQueueItem.isRefit)
					{
						num2 = i;
						break;
					}
				}
				else
				{
					bool flag4 = item.AIFactionGoal != null && (shipConstructionQueueItem.AIFactionGoal == null || item.AIFactionGoal.assignedDate < shipConstructionQueueItem.AIFactionGoal.assignedDate);
					if (num4 > num5 || flag4)
					{
						num2 = i;
						break;
					}
				}
			}
			if (num2 != num)
			{
				CS$<>8__locals1.faction.playerControl.StartAction(new RepositionShipinConstructionQueueAction(item.shipyard, item, num2));
			}
			return num2;
		}

		// Token: 0x06005A50 RID: 23120 RVA: 0x002A8BE8 File Offset: 0x002A6DE8
		private static void CheckSellResourcesOnEarth(TIFactionState faction)
		{
			if (faction.GetMonthlyIncome(FactionResource.Money, false, false) < 0f && faction.GetCurrentResourceAmount(FactionResource.Money) < 0f && faction.CanSellSpaceResourcesOnEarth)
			{
				List<FactionResource> list = faction.SellableResourcesOnEarth();
				FactionResource factionResource = list.Where<FactionResource>((FactionResource x) => AIEvaluators.Abundant(faction, x, faction.GetCurrentResourceAmount(x), faction.GetDailyIncome(x, false, false) > 0f, 1f)).MaxBy<FactionResource, float>((FactionResource x) => faction.GetCurrentResourceAmount(x) * TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(faction, x));
				if (factionResource == FactionResource.None)
				{
					factionResource = list.Where<FactionResource>((FactionResource x) => AIEvaluators.Abundant(faction, x, faction.GetCurrentResourceAmount(x), faction.GetDailyIncome(x, false, false) > 0f, 0.5f)).MaxBy<FactionResource, float>((FactionResource x) => faction.GetCurrentResourceAmount(x) * TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(faction, x));
				}
				if (factionResource == FactionResource.None)
				{
					factionResource = list.Where<FactionResource>((FactionResource x) => AIEvaluators.Abundant(faction, x, faction.GetCurrentResourceAmount(x), faction.GetDailyIncome(x, false, false) > 0f, 0.1f)).MaxBy<FactionResource, float>((FactionResource x) => faction.GetCurrentResourceAmount(x) * TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(faction, x));
				}
				if (factionResource != FactionResource.None)
				{
					float modifiedResourceMarketValueForSelling = TIGlobalValuesState.GlobalValues.GetModifiedResourceMarketValueForSelling(faction, factionResource);
					float num = -1f * (faction.GetCurrentResourceAmount(FactionResource.Money) + faction.GetMonthlyIncome(FactionResource.Money, false, false) * 6f) / modifiedResourceMarketValueForSelling;
					int num2 = (int)Mathf.Min(faction.GetCurrentResourceAmount(factionResource) / 2f, num);
					if (num2 > 0)
					{
						faction.playerControl.StartAction(new SellSpaceResourcesToEarthAction(faction, new Dictionary<FactionResource, int> { { factionResource, num2 } }));
					}
				}
			}
		}

		// Token: 0x06005A51 RID: 23121 RVA: 0x002A8D4C File Offset: 0x002A6F4C
		public static void ManageAlliancesAndRivalries(TIFactionState faction)
		{
			int num = (int)(faction.GetCurrentResourceAmount(FactionResource.Influence) - faction.AISavingTarget.GetBankedQuantity(FactionResource.Influence));
			if (faction.councilors.Count < faction.maxCouncilSize)
			{
				num -= TemplateManager.global.baseCouncilorRecruitCost_influence * (faction.maxCouncilSize - faction.councilors.Count);
			}
			if (faction.IsAlienProxy && faction.knowsWinCondition)
			{
				num -= (int)TIFactionState.grantNationMission.cost.value;
			}
			int num2 = (int)((float)num * 0.25f / TIFactionState.setPolicyMission.cost.value);
			if (num2 > 0)
			{
				AIDailyFactionPlanner.<>c__DisplayClass74_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass74_0();
				List<TINationState> executiveNations = faction.executiveNations;
				CS$<>8__locals1.relationChanges = new Dictionary<AIDailyFactionPlanner.AIRelationshipChangeKey, AIDailyFactionPlanner.AIRelationshipChange>();
				List<TIFactionGoalState> list = faction.factionGoals.Values.SelectMany<List<TIFactionGoalState>, TIFactionGoalState>((List<TIFactionGoalState> x) => x.Where<TIFactionGoalState>((TIFactionGoalState y) => y.PoliciesAsNationGoal())).ToList<TIFactionGoalState>();
				List<TIFactionGoalState> list2 = faction.factionGoals.Values.SelectMany<List<TIFactionGoalState>, TIFactionGoalState>((List<TIFactionGoalState> x) => x.Where<TIFactionGoalState>((TIFactionGoalState y) => y.PoliciesAtTargetNationGoal())).ToList<TIFactionGoalState>();
				List<TINationState> list3 = new List<TINationState>();
				List<TINationState> list4 = new List<TINationState>();
				foreach (TIFactionGoalState tifactionGoalState in list)
				{
					list3.Clear();
					if (tifactionGoalState.actor() == faction && tifactionGoalState.PoliciesAsFactionActor)
					{
						list3 = executiveNations;
					}
					else if (tifactionGoalState.actor().isNationState && executiveNations.Contains(tifactionGoalState.actor().ref_nation))
					{
						list3.Add(tifactionGoalState.actor().ref_nation);
					}
					for (int i = 0; i < list3.Count; i++)
					{
						TINationState tinationState = list3[i];
						for (int j = 0; j < tifactionGoalState.factionLevelPoliciesAsNation.Count; j++)
						{
							PolicyType policyType = tifactionGoalState.factionLevelPoliciesAsNation[j];
							for (int k = 0; k < list2.Count; k++)
							{
								TIFactionGoalState tifactionGoalState2 = list2[k];
								if (tifactionGoalState2.factionLevelPoliciesAtTarget.Contains(policyType))
								{
									list4.Clear();
									if (tifactionGoalState2.target().isNationState)
									{
										list4.Add(tifactionGoalState2.target().ref_nation);
									}
									else if (tifactionGoalState2.target().isFactionState)
									{
										list4.AddRange(tifactionGoalState2.target().ref_faction.executiveNations);
									}
									list4.Remove(tinationState);
									TIPolicyOption tipolicyOption = PolicyManager.policies[policyType] as TIPolicyOption;
									foreach (TINationState tinationState2 in list4)
									{
										AIDailyFactionPlanner.AIRelationshipChangeKey airelationshipChangeKey = new AIDailyFactionPlanner.AIRelationshipChangeKey
										{
											nation = tinationState,
											targetNation = tinationState2,
											change = tipolicyOption.relationChange
										};
										if (!CS$<>8__locals1.relationChanges.ContainsKey(airelationshipChangeKey) && tinationState.CanDoFactionLevelRelationshipChange(tinationState2, tipolicyOption.relationChange))
										{
											float num3 = AIEvaluators.ScoreRelationsChange(tinationState, tinationState2, tipolicyOption.relationChange, tinationState2.executiveFaction == faction);
											if (tipolicyOption.RequiresTargetConfirm())
											{
												num3 *= (tipolicyOption as TIPolicyOptionWithConfirm).AIAgreeChance(tinationState, tinationState2);
											}
											if (num3 > 0f)
											{
												CS$<>8__locals1.<ManageAlliancesAndRivalries>g__TryAddRelationshipChange|0(new AIDailyFactionPlanner.AIRelationshipChange(tinationState, tinationState2, tipolicyOption.relationChange, num3 * 2f, tifactionGoalState.importance));
											}
										}
									}
								}
							}
						}
					}
				}
				using (List<TINationState>.Enumerator enumerator2 = executiveNations.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TINationState nation = enumerator2.Current;
						if (nation.HasExternalClaims() && nation.executiveFaction.FindGoals(GoalType.ExpandNation, nation, nation, TIFactionState.GoalFilter.none, true).FirstOrDefault<TIFactionGoalState>() != null)
						{
							foreach (TINationState tinationState3 in (from x in nation.ExternalClaims()
								select x.nation).Distinct<TINationState>().ToList<TINationState>())
							{
								float num4 = AIEvaluators.ScoreRelationsChange(nation, tinationState3, RelationChange.NormalToAlly, tinationState3.executiveFaction == faction);
								if (num4 > 0f)
								{
									CS$<>8__locals1.<ManageAlliancesAndRivalries>g__TryAddRelationshipChange|0(new AIDailyFactionPlanner.AIRelationshipChange(nation, tinationState3, RelationChange.NormalToAlly, num4, 0));
								}
							}
						}
						foreach (TINationState tinationState4 in new List<TINationState>(nation.allies.Where<TINationState>((TINationState x) => CS$<>8__locals1.relationChanges.Keys.None<AIDailyFactionPlanner.AIRelationshipChangeKey>((AIDailyFactionPlanner.AIRelationshipChangeKey y) => nation == y.nation && y.targetNation == x) && nation.CanEndAlliance(x))))
						{
							float num5 = AIEvaluators.ScoreRelationsChange(nation, tinationState4, RelationChange.AllyToNormal, tinationState4.executiveFaction == faction);
							if (num5 > 0f)
							{
								CS$<>8__locals1.<ManageAlliancesAndRivalries>g__TryAddRelationshipChange|0(new AIDailyFactionPlanner.AIRelationshipChange(nation, tinationState4, RelationChange.AllyToNormal, num5, 0));
							}
						}
						foreach (TINationState tinationState5 in new List<TINationState>(nation.rivals.Where<TINationState>((TINationState x) => CS$<>8__locals1.relationChanges.Keys.None<AIDailyFactionPlanner.AIRelationshipChangeKey>((AIDailyFactionPlanner.AIRelationshipChangeKey y) => nation == y.nation && y.targetNation == x) && nation.CanEndRivalry(x))))
						{
							float num6 = AIEvaluators.ScoreRelationsChange(nation, tinationState5, RelationChange.RivalToNormal, tinationState5.executiveFaction == faction);
							num6 *= StratPolicyResponseSelector.ChanceEndRivalry(nation, tinationState5);
							if (num6 > 0f)
							{
								CS$<>8__locals1.<ManageAlliancesAndRivalries>g__TryAddRelationshipChange|0(new AIDailyFactionPlanner.AIRelationshipChange(nation, tinationState5, RelationChange.RivalToNormal, num6, 0));
							}
						}
					}
				}
				List<AIDailyFactionPlanner.AIRelationshipChange> list5 = CS$<>8__locals1.relationChanges.Values.OrderByDescending<AIDailyFactionPlanner.AIRelationshipChange, float>((AIDailyFactionPlanner.AIRelationshipChange x) => x.score).ToList<AIDailyFactionPlanner.AIRelationshipChange>();
				int num7 = 0;
				int num8 = 0;
				int num9 = num2;
				if (num2 > 0 && num9 <= 0)
				{
					num9 = 1;
				}
				while (num8 < num9 && num7 < CS$<>8__locals1.relationChanges.Count)
				{
					AIDailyFactionPlanner.AIRelationshipChange airelationshipChange = list5[num7];
					float num10 = Mathf.Clamp((float)airelationshipChange.goalImportance / 20f, 0.33f, 1f);
					if (!TINationState.FactionLevelRelationShipChangeCost.CanAfford(faction, num10, null, float.PositiveInfinity))
					{
						break;
					}
					if (airelationshipChange.nation.CanDoFactionLevelRelationshipChange(airelationshipChange.targetNation, airelationshipChange.change))
					{
						airelationshipChange.nation.HandleFactionLevelRelationshipChanges(airelationshipChange.targetNation, airelationshipChange.change);
						num8++;
					}
					num7++;
				}
			}
		}

		// Token: 0x06005A52 RID: 23122 RVA: 0x002A94DC File Offset: 0x002A76DC
		private void ManageNations(TIFactionState faction)
		{
			int day = this.gameTime.currentTime.day;
			if (day <= 28 && day % 14 == AIDailyFactionPlanner.factionAIData[faction].every14DaysOffset)
			{
				bool flag = faction.NeedsSpaceBootstrap();
				bool flag2 = flag || faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost);
				bool flag3 = faction.resourceIncomeDeficiencies.Contains(FactionResource.MissionControl);
				bool flag4 = !flag2 && AIEvaluators.Abundant(faction, FactionResource.Boost, 1f);
				bool flag5 = !flag3 && AIEvaluators.Abundant(faction, FactionResource.MissionControl, 0f, faction.AI_GenericMissionControlAvailable > 0, 1f);
				bool flag6 = faction.resourceIncomeDeficiencies.Contains(FactionResource.Research);
				List<FactionGoal_Nation> list = faction.GoalsOfType(TIFactionGoalState.NationPriorityModifyingGoals, false, true).ConvertAll<FactionGoal_Nation>((TIFactionGoalState x) => (FactionGoal_Nation)x);
				bool flag7 = faction.GetCurrentResourceAmount(FactionResource.Money) < 10000f;
				bool flag8 = flag7 || faction.resourceIncomeDeficiencies.Contains(FactionResource.Money);
				Dictionary<FactionResource, ValueTuple<bool, bool, bool>> spaceResourceIncomesChecklist = AIEvaluators.GetSpaceResourceIncomesChecklist((FactionResource x) => AIEvaluators.EstimateFutureIncomePerMonth(faction, x, false, false, false));
				using (List<TINationState>.Enumerator enumerator = faction.nationsWithMyControlPoints.GetEnumerator())
				{
					Func<TINationState, bool> <>9__29;
					Func<TINationState, bool> <>9__32;
					while (enumerator.MoveNext())
					{
						AIDailyFactionPlanner.<>c__DisplayClass77_1 CS$<>8__locals2 = new AIDailyFactionPlanner.<>c__DisplayClass77_1();
						CS$<>8__locals2.nation = enumerator.Current;
						List<TIControlPoint> list2 = CS$<>8__locals2.nation.FactionControlPoints(faction, true, false, true);
						int count = list2.Count;
						List<FactionGoal_Nation> list3 = list.Where<FactionGoal_Nation>((FactionGoal_Nation x) => x.target() == CS$<>8__locals2.nation).ToList<FactionGoal_Nation>();
						int num = -1;
						int num2 = 0;
						bool flag9 = CS$<>8__locals2.nation.executiveFaction == faction;
						bool flag10 = !CS$<>8__locals2.nation.ValidPriority(PriorityType.LaunchFacilities) || (CS$<>8__locals2.nation.inFederation && CS$<>8__locals2.nation.federation.leadNation != CS$<>8__locals2.nation && CS$<>8__locals2.nation.executiveFaction == faction && faction.AI_AtWarWithFaction(CS$<>8__locals2.nation.federation.leadNation.executiveFaction));
						if (faction.IsAlienFaction)
						{
							using (List<TIControlPoint>.Enumerator enumerator2 = list2.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									TIControlPoint ticontrolPoint = enumerator2.Current;
									faction.playerControl.StartAction(new ApplyPriorityPresetToControlPoint(ticontrolPoint, faction, faction.defaultPriorityPresetTemplateName));
								}
								continue;
							}
						}
						AIDailyFactionPlanner.<>c__DisplayClass77_2 CS$<>8__locals3 = new AIDailyFactionPlanner.<>c__DisplayClass77_2();
						CS$<>8__locals3.CS$<>8__locals1 = CS$<>8__locals2;
						CS$<>8__locals3.desiredPrioritySettings = Enums.PriorityTypes.ToDictionary<PriorityType, PriorityType, int>((PriorityType x) => x, (PriorityType x) => 0);
						CS$<>8__locals3.validPriorities = CS$<>8__locals3.CS$<>8__locals1.nation.ValidPriorities;
						FactionGoal_Nation factionGoal_Nation = list3.FirstOrDefault<FactionGoal_Nation>((FactionGoal_Nation x) => x.GetGoalType() == GoalType.NeutralizeNation || x.GetGoalType() == GoalType.PillageNation);
						if (factionGoal_Nation != null)
						{
							using (List<PriorityType>.Enumerator enumerator3 = CS$<>8__locals3.desiredPrioritySettings.Keys.ToList<PriorityType>().GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									PriorityType priorityType = enumerator3.Current;
									if (factionGoal_Nation.prioritiesAsNation.ContainsKey(priorityType))
									{
										CS$<>8__locals3.<ManageNations>g__SetDesiredPriority|5(priorityType, (float)factionGoal_Nation.prioritiesAsNation[priorityType]);
									}
								}
								goto IL_2530;
							}
							goto IL_03C8;
						}
						goto IL_03C8;
						IL_2530:
						IEnumerable<PriorityType> priorityTypes = Enums.PriorityTypes;
						Func<PriorityType, bool> func;
						if ((func = CS$<>8__locals3.CS$<>8__locals1.<>9__36) == null)
						{
							func = (CS$<>8__locals3.CS$<>8__locals1.<>9__36 = (PriorityType x) => CS$<>8__locals3.CS$<>8__locals1.nation.ValidPriority(x));
						}
						foreach (PriorityType priorityType2 in from x in priorityTypes.Where<PriorityType>(func)
							orderby TIUtilities.RandomFloatValue()
							select x)
						{
							foreach (TIControlPoint ticontrolPoint2 in CS$<>8__locals3.CS$<>8__locals1.nation.FactionControlPoints(faction, true, false, true))
							{
								int num3 = CS$<>8__locals3.desiredPrioritySettings[priorityType2];
								if (num3 > 0 && num2 > 0)
								{
									CS$<>8__locals3.desiredPrioritySettings[priorityType2] = num3 - 1;
									num2--;
								}
							}
						}
						PriorityType priorityType3 = PriorityType.None;
						if (AIEvaluators.FactionIsWorkingOnHabModuleBasedObjectives(faction) && faction.AvailableMissionControlMinusFutureUsage < 5)
						{
							priorityType3 = PriorityType.MissionControl;
						}
						if (priorityType3 != PriorityType.None)
						{
							IEnumerable<PriorityType> priorityTypes2 = Enums.PriorityTypes;
							Func<PriorityType, bool> func2;
							if ((func2 = CS$<>8__locals3.CS$<>8__locals1.<>9__38) == null)
							{
								func2 = (CS$<>8__locals3.CS$<>8__locals1.<>9__38 = (PriorityType x) => CS$<>8__locals3.CS$<>8__locals1.nation.ValidPriority(x));
							}
							foreach (PriorityType priorityType4 in priorityTypes2.Where<PriorityType>(func2))
							{
								if (priorityType4 == priorityType3)
								{
									CS$<>8__locals3.desiredPrioritySettings[priorityType4] = 3;
								}
								else
								{
									int num4 = CS$<>8__locals3.desiredPrioritySettings[priorityType4];
									if (num4 > 0)
									{
										CS$<>8__locals3.desiredPrioritySettings[priorityType4] = num4 - 1;
									}
								}
							}
						}
						List<TIControlPoint> list4 = CS$<>8__locals3.CS$<>8__locals1.nation.FactionControlPoints(faction, true, false, true);
						CS$<>8__locals3.exampleControlPoint = list4.First<TIControlPoint>();
						CS$<>8__locals3.priorityEfficiencies = Enums.PriorityTypes.ToDictionary<PriorityType, PriorityType, float>((PriorityType x) => x, (PriorityType x) => 1f + CS$<>8__locals3.CS$<>8__locals1.nation.ControlPointPriorityBonuses(CS$<>8__locals3.exampleControlPoint, x, false, true));
						Dictionary<PriorityType, float> dictionary = CS$<>8__locals3.desiredPrioritySettings.ToDictionary<KeyValuePair<PriorityType, int>, PriorityType, float>((KeyValuePair<PriorityType, int> x) => x.Key, (KeyValuePair<PriorityType, int> x) => (float)x.Value * CS$<>8__locals3.priorityEfficiencies[x.Key]);
						float num5 = dictionary.Values.Max();
						float num6 = 1f;
						if (num5 > 3f)
						{
							num6 = 3f / num5;
						}
						foreach (KeyValuePair<PriorityType, int> keyValuePair in CS$<>8__locals3.desiredPrioritySettings.ToList<KeyValuePair<PriorityType, int>>())
						{
							PriorityType key = keyValuePair.Key;
							int value = keyValuePair.Value;
							float num7 = dictionary[key] * num6;
							int num8 = num7.RoundDown();
							if (global::UnityEngine.Random.value < num7 - (float)num8)
							{
								num8++;
							}
							num8 = Mathf.Clamp(num8, 0, 3);
							CS$<>8__locals3.desiredPrioritySettings[key] = num8;
						}
						IEnumerable<PriorityType> priorityTypes3 = Enums.PriorityTypes;
						Func<PriorityType, bool> func3;
						if ((func3 = CS$<>8__locals3.CS$<>8__locals1.<>9__39) == null)
						{
							func3 = (CS$<>8__locals3.CS$<>8__locals1.<>9__39 = (PriorityType x) => CS$<>8__locals3.CS$<>8__locals1.nation.ValidPriority(x));
						}
						foreach (PriorityType priorityType5 in from x in priorityTypes3.Where<PriorityType>(func3)
							orderby TIUtilities.RandomFloatValue()
							select x)
						{
							foreach (TIControlPoint ticontrolPoint3 in list4)
							{
								bool flag11 = false;
								if (priorityType5 == PriorityType.Spoils)
								{
									if (ticontrolPoint3.benefitsDisabled)
									{
										if (ticontrolPoint3.GetControlPointPriority(priorityType5, false) != 0)
										{
											faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint3, faction, priorityType5, 0, false, true));
											flag11 = true;
										}
									}
									else if (num > 0)
									{
										int num9 = Mathf.Clamp(num, 0, 3);
										if (num9 > 0 && priorityType3 != PriorityType.None && priorityType3 != PriorityType.Spoils)
										{
											num9--;
										}
										faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint3, faction, priorityType5, num9, false, true));
										num -= num9;
										flag11 = true;
									}
									else if (CS$<>8__locals3.desiredPrioritySettings[priorityType5] != ticontrolPoint3.GetControlPointPriority(priorityType5, false))
									{
										faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint3, faction, priorityType5, CS$<>8__locals3.desiredPrioritySettings[priorityType5], false, true));
										flag11 = true;
									}
								}
								else
								{
									int num10 = CS$<>8__locals3.desiredPrioritySettings[priorityType5];
									if (num10 != ticontrolPoint3.GetControlPointPriority(priorityType5, false))
									{
										faction.playerControl.StartAction(new SetPriorityAction(ticontrolPoint3, faction, priorityType5, num10, false, true));
										flag11 = true;
									}
								}
								if (flag11)
								{
									ticontrolPoint3.RecordAndFixControlPointValues(false);
									GameControl.eventManager.TriggerEvent(new ControlPointDataUpdated(ticontrolPoint3), null, new object[] { CS$<>8__locals3.CS$<>8__locals1.nation });
								}
							}
						}
						continue;
						IL_03C8:
						AIDailyFactionPlanner.<>c__DisplayClass77_3 CS$<>8__locals4 = new AIDailyFactionPlanner.<>c__DisplayClass77_3();
						CS$<>8__locals4.CS$<>8__locals2 = CS$<>8__locals3;
						CS$<>8__locals4.IPs = (float)count * (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.BaseInvestmentPoints_month() / (float)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.numControlPoints);
						Dictionary<PriorityType, bool> dictionary2 = Enums.PriorityTypes.ToDictionary<PriorityType, PriorityType, bool>((PriorityType x) => x, (PriorityType x) => CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.ValidPriority(x) && (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.DeltaToInvestmentThreshhold(x) < CS$<>8__locals4.IPs || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetAccumulatedInvestmentPoints(x) / CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetRequiredInvestmentPointsForPriority(x) > 0.9f));
						bool flag12 = faction.IsAlienFaction || CS$<>8__locals4.IPs <= 5f || (float)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.NumNuclearWeaponsDefendingMe() >= 10f * faction.aiValues.wantEarthWarCapability || !flag9;
						bool flag13;
						if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.wars.Count > 0)
						{
							flag13 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.currentWarStates.Where<TIWarState>((TIWarState war) => !war.stalemate).Any<TIWarState>(delegate(TIWarState war)
							{
								IEnumerable<TINationState> enumerable = war.EnemyAlliance(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation);
								Func<TINationState, bool> func7;
								if ((func7 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.<>9__23) == null)
								{
									func7 = (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.<>9__23 = delegate(TINationState enemy)
									{
										if (enemy.WinningWarAgainst(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation))
										{
											IEnumerable<TIArmyState> armies = enemy.armies;
											Func<TIArmyState, bool> func8;
											if ((func8 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.<>9__24) == null)
											{
												func8 = (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.<>9__24 = (TIArmyState enemyArmy) => enemyArmy.CanGetTo(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.capital, null, null, null));
											}
											return armies.Any<TIArmyState>(func8);
										}
										return false;
									});
								}
								return enumerable.Any<TINationState>(func7);
							});
						}
						else
						{
							flag13 = false;
						}
						bool flag14 = flag13;
						bool flag15 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.ArmiesThreateningCapital(true, false) > 0;
						CS$<>8__locals4.problems = 0;
						if (flag14)
						{
							CS$<>8__locals4.problems = 10;
							CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_FoundMilitary, 3f);
							CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military, 3f);
							if (!CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.civilWar && !flag12 && (dictionary2[PriorityType.Military_InitiateNuclearProgram] || dictionary2[PriorityType.Military_BuildNuclearWeapons]))
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_InitiateNuclearProgram, 3f);
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_BuildNuclearWeapons, 3f);
							}
							if (!flag15 || dictionary2[PriorityType.Military_BuildArmy])
							{
								goto IL_061B;
							}
							if (CS$<>8__locals4.IPs - (float)CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings.Sum<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value) > 0f)
							{
								goto IL_061B;
							}
							IL_0716:
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.civilWar)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Oppression, 3f);
								goto IL_22FF;
							}
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.unrestMajorWarning && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy < 6.5f)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Oppression, 1f);
								goto IL_22FF;
							}
							goto IL_22FF;
							IL_061B:
							CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildArmy, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextArmyControlPointIdx()].faction == faction || flag9) ? 3 : 0));
							if (flag15)
							{
								goto IL_0716;
							}
							CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildNavy, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextArmyControlPointIdx()].faction == faction || flag9) ? 3 : 0));
							if (!flag12 && !CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.civilWar)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_InitiateNuclearProgram, 3f);
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_BuildNuclearWeapons, 3f);
								goto IL_0716;
							}
							goto IL_0716;
						}
						else
						{
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.belligerentInActiveWar)
							{
								CS$<>8__locals4.problems = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.wars.Sum<TINationState>((TINationState x) => x.numStandardArmies);
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military, 3f);
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.civilWar)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Oppression, 3f);
								}
								else if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.unrestMajorWarning && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy < 6.5f)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Oppression, (float)(1 + (faction.cynical ? 1 : 0)));
								}
								else if (!flag12)
								{
									if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.wars.Any<TINationState>((TINationState x) => x.numNuclearWeapons > 0) && (CS$<>8__locals4.IPs > 12f || dictionary2[PriorityType.Military_InitiateNuclearProgram] || dictionary2[PriorityType.Military_BuildNuclearWeapons]))
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_InitiateNuclearProgram, (float)(dictionary2[PriorityType.Military_BuildArmy] ? 1 : 3));
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildNuclearWeapons, (float)(dictionary2[PriorityType.Military_BuildArmy] ? 1 : 3));
									}
								}
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.regions.Any<TIRegionState>((TIRegionState x) => x.underBombardment))
								{
									goto IL_09AB;
								}
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.wars.Any<TINationState>((TINationState x) => x.numNuclearWeapons > 1))
								{
									goto IL_09AB;
								}
								IL_09C6:
								if (dictionary2[PriorityType.Military_BuildArmy] || CS$<>8__locals4.IPs > 6f || flag9)
								{
									goto IL_0A2A;
								}
								if (CS$<>8__locals4.IPs - (float)CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings.Sum<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value) > 0f)
								{
									goto IL_0A2A;
								}
								IL_0B08:
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning)
								{
									if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.majorCohesionWarning)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Unity, 3f);
										CS$<>8__locals4.problems += 2;
									}
									else
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Unity, (float)((CS$<>8__locals4.IPs > 2f && (double)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy <= 6.5) ? 1 : 0));
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Knowledge, CS$<>8__locals4.IPs / 6f);
										CS$<>8__locals4.problems++;
									}
								}
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.severeInequalityWarning)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Welfare, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning ? 1 : 0) + ((faction.aiValues.protectHumanLife > 0.5f) ? 2 : 1) + (faction.cynical ? 1 : 2)));
								}
								else if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Welfare, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning ? 1 : 0) + ((faction.aiValues.protectHumanLife > 0.5f) ? 1 : 0) + (faction.cynical ? 0 : 1)));
								}
								if (flag && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.numControlPoints > 3)
								{
									if (flag10)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Civilian_InitiateSpaceflightProgram, 3f);
									}
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.LaunchFacilities, 3f);
									goto IL_22FF;
								}
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.MissionControl, (float)(flag3 ? 2 : (flag5 ? 0 : 1)));
								goto IL_22FF;
								IL_0A2A:
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildArmy, (float)((int)Mathf.Clamp(CS$<>8__locals4.IPs / 4f, 0f, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextArmyControlPointIdx()].faction == faction) ? 3 : 1))));
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildNavy, (float)((int)Mathf.Clamp(CS$<>8__locals4.IPs / 4f, 0f, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextArmyControlPointIdx()].faction == faction) ? 3 : 1))));
								goto IL_0B08;
								IL_09AB:
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildSpaceDefenses, CS$<>8__locals4.IPs / 4f);
								goto IL_09C6;
							}
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.unrestMajorWarning)
							{
								if (!flag9)
								{
									if (!list.None<FactionGoal_Nation>((FactionGoal_Nation x) => x.GetGoalType() == GoalType.CaptureNationDirty))
									{
										goto IL_0ED9;
									}
								}
								CS$<>8__locals4.problems = (int)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.unrest;
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Oppression, (float)(CS$<>8__locals4.problems / 3));
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Unity, (float)(((double)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy <= 6.5) ? 2 : 1));
								}
								if (CS$<>8__locals4.IPs > 2f && (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning))
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Welfare, (float)(((faction.aiValues.protectHumanLife > 0.5f) ? 2 : 1) + (faction.cynical ? 1 : 2)));
								}
								if (CS$<>8__locals4.IPs > 4f && dictionary2[PriorityType.Military_BuildArmy])
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_BuildArmy, (float)((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextArmyControlPointIdx()].faction == faction) ? 3 : 1));
									goto IL_22FF;
								}
								goto IL_22FF;
							}
							IL_0ED9:
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.severeInequalityWarning)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Welfare, (float)(((faction.aiValues.protectHumanLife > 0.5f) ? 2 : 1) + (faction.cynical ? 1 : 2)));
								CS$<>8__locals4.problems++;
							}
							else if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Welfare, (float)(1 + ((CS$<>8__locals4.IPs > 5f) ? 1 : 0) + ((!faction.cynical && (double)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy >= 4.5) ? 1 : 0)));
							}
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.unrestMajorWarning)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Oppression, (7f - CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy) / 2.333f);
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.canAccumulateLegitimizeClaimTriggers)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Unity, (float)((CS$<>8__locals4.IPs > 5f && (double)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy <= 6.5) ? 2 : 1));
									if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Welfare, (float)((CS$<>8__locals4.IPs > 5f && (double)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy >= 4.5) ? 2 : 1));
									}
								}
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Economy, Mathf.Clamp(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy / 3f, 0f, 3f));
								CS$<>8__locals4.problems += 2;
							}
							else if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.unrestWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.futureUnrestMajorWarning)
							{
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.perCapitaGDP < 6000f)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Economy, Mathf.Clamp(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy / 3f, 1f, 3f));
								}
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Welfare, (float)(((faction.aiValues.protectHumanLife > 0.5f) ? 2 : 1) + (faction.cynical ? 0 : 1)));
								}
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.majorCohesionWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.canAccumulateLegitimizeClaimTriggers)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Unity, 3f);
									CS$<>8__locals4.problems += 2;
								}
								else
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Unity, (float)((CS$<>8__locals4.IPs > 2f && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.education + CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy <= 15f) ? ((int)Mathf.Clamp(CS$<>8__locals4.IPs / 6f, 1f, 3f)) : ((CS$<>8__locals4.IPs > 6f) ? 1 : 0)));
								}
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Knowledge, (float)((CS$<>8__locals4.IPs > 2f) ? ((int)Mathf.Clamp(CS$<>8__locals4.IPs / 2f, 1f, 3f)) : 0));
								CS$<>8__locals4.problems++;
							}
							if (CS$<>8__locals4.problems < 5 && (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.canAccumulateCoreEconomyTriggers || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.canAccumulateCoreMiningTriggers || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.canAccumulateCoreOilTriggers))
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Economy, (float)Mathf.Clamp(5 - CS$<>8__locals4.problems / 2, 1, 3));
							}
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.corruption > 0.5f && !CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.severeInequalityWarning && !CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.majorCohesionWarning)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Spoils, 1f);
								CS$<>8__locals4.problems++;
							}
							if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy > 3.5f + (float)CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings[PriorityType.Oppression] && (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy < 7f || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.canAccumulateLegitimizeClaimTriggers) && CS$<>8__locals4.problems < 5)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Government, (float)Mathf.Min(3, 5 - CS$<>8__locals4.problems));
							}
							if (faction == CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.executiveFaction && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.executiveFaction.LEOStations.Count > 0)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildSTOSquadron, (float)Mathf.Min(3, 5 - CS$<>8__locals4.problems));
							}
							if (factionGoal_Nation == null && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.CouncilControlPointFraction_DiscountNeutral(faction, false, false) > 0.5f)
							{
								if (flag14 || !CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.MajorGlobalPower)
								{
									TIGameState nation = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation;
									TIFactionGoalState focusGoal = faction.focusGoal;
									if (!(nation == ((focusGoal != null) ? focusGoal.target() : null)))
									{
										goto IL_1689;
									}
								}
								if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetPublicOpinionOfFaction(faction) < 0.25f)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Unity, (float)((faction.cynical || faction.extremist) ? 3 : 2));
								}
								else if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetPublicOpinionOfFaction(faction) < 0.5f)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Unity, (float)((faction.cynical || faction.extremist) ? 2 : 1));
								}
								else if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetPublicOpinionOfFaction(faction) < CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.singleIdeaCap - 0.05f)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Unity, (float)((faction.cynical || faction.extremist) ? 1 : 0));
								}
							}
							IL_1689:
							TIFactionGoalState tifactionGoalState = faction.GetManagementGoalForNation(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation, false) ?? faction.SetManagementGoalForNation(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation);
							if (tifactionGoalState != null && tifactionGoalState.GetGoalType() == GoalType.SpaceifyNation)
							{
								if (flag10)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Civilian_InitiateSpaceflightProgram, 3f);
								}
								int num11;
								if (flag2)
								{
									num11 = 3;
								}
								else if (flag4 && flag3)
								{
									num11 = 1;
								}
								else
								{
									num11 = 2;
								}
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.LaunchFacilities, (float)num11);
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.MissionControl, (float)((flag2 && !flag3) ? 1 : 3));
							}
							int num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
							bool flag16 = flag9 || (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.CouncilControlPointFraction(faction, true, true) >= 0.5f && faction.FindGoals(TIFactionGoalState.CaptureNationGoals, faction, CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation, TIFactionState.GoalFilter.none, false).Count > 0 && !faction.enemyWarFactions.Contains(CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.executiveFaction));
							flag16 = flag16 && tifactionGoalState != null && this.buildUpMilitaryGoals.Contains(tifactionGoalState.GetGoalType());
							bool flag17 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.numStandardArmies < CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.numControlPoints_unclamped && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextArmyControlPointIdx()].faction == faction;
							bool flag18 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextNavy() != null && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.GetNextNavy().faction == faction;
							if (flag16 || flag17 || flag18 || GameStateManager.AlienNation().extant)
							{
								IEnumerable<TINationState> rivals = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.rivals;
								Func<TINationState, bool> func4;
								if ((func4 = <>9__29) == null)
								{
									func4 = (<>9__29 = delegate(TINationState x)
									{
										TIFactionState executiveFaction = x.executiveFaction;
										return executiveFaction == null || !executiveFaction.permanentAlly(faction);
									});
								}
								int num13 = rivals.Where<TINationState>(func4).Sum<TINationState>((TINationState x) => x.numStandardArmies) - CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.numStandardArmies - CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.allies.Sum<TINationState>((TINationState x) => x.numStandardArmies);
								if (GameStateManager.AlienNation().extant)
								{
									num13++;
								}
								if (num13 > 0)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(PriorityType.Military_FoundMilitary, 3f);
									if (flag16)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military, (float)((int)Mathf.Clamp((float)num13 * 0.5f, 1f, 3f)));
									}
									if (num12 >= (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.HasExternalClaims() ? 8 : 12))
									{
										if (flag16 || flag17)
										{
											CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildArmy, (float)Mathf.Min(num13, num12 / 8));
										}
										if ((flag16 || flag18) && num12 >= (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.HasExternalClaims() ? 10 : 20))
										{
											CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildNavy, (float)Mathf.Min(num13, num12 / 8));
										}
									}
									num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
									if (num12 > 0 && (flag16 || GameStateManager.AlienNation().extant))
									{
										IEnumerable<TINationState> rivals2 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.rivals;
										Func<TINationState, bool> func5;
										if ((func5 = <>9__32) == null)
										{
											func5 = (<>9__32 = (TINationState x) => x.executiveFaction != faction);
										}
										if (rivals2.Where<TINationState>(func5).Any<TINationState>((TINationState x) => x.numNuclearWeapons > 1))
										{
											CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildSpaceDefenses, (float)Mathf.Clamp(num13, 1, 3));
										}
										if (!flag12 && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.NumNuclearWeaponsDefendingMe() == 0)
										{
											CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_InitiateNuclearProgram, (float)Mathf.Clamp(num12, 1, Mathf.Min(3, num13)));
											CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Military_BuildNuclearWeapons, (float)Mathf.Clamp(num12, 1, Mathf.Min(3, num13)));
										}
									}
									CS$<>8__locals4.problems++;
									num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
								}
							}
							if (num12 > 0 && (flag8 || flag7))
							{
								if ((flag7 || (!CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.severeInequalityWarning && !CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.majorCohesionWarning && !CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.futureMajorCohesionWarning && (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy < 7.5f || faction.cynical))) && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.FactionControlPoints(faction, false, false, true).Count > 0)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Spoils, (float)(1 + (faction.cynical ? 1 : (-1)) + ((CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.elitesHappy || (double)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.corruption < 0.15) ? (-2) : 1) + (faction.believers ? (-1) : 1) + CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.currentResourceRegions + (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.cohesionWarning ? (-2) : 0) + (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning ? (-1) : 0)));
								}
								if (num12 - CS$<>8__locals4.problems > 8)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Funding, (float)Mathf.Clamp(num12, 1, 3));
								}
								num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
							}
							int num14 = 0;
							if (flag2 && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.IsUsefulForBoost())
							{
								if (spaceResourceIncomesChecklist.None<KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>>>(([TupleElementNames(new string[] { "MeetsMinimum", "MeetsRecommended", "MeetsGood" })] KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>> x) => x.Value.Item1) && faction.GetDailyIncome(FactionResource.Boost, false, false) < 0f)
								{
									num14 = 3;
								}
								else if (spaceResourceIncomesChecklist.NotAll<KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>>>(([TupleElementNames(new string[] { "MeetsMinimum", "MeetsRecommended", "MeetsGood" })] KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>> x) => x.Value.Item1) || faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
								{
									num14 = 2;
								}
								else if (spaceResourceIncomesChecklist.None<KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>>>(([TupleElementNames(new string[] { "MeetsMinimum", "MeetsRecommended", "MeetsGood" })] KeyValuePair<FactionResource, ValueTuple<bool, bool, bool>> x) => x.Value.Item2))
								{
									num14 = 1;
								}
								if (num14 > 0)
								{
									CS$<>8__locals4.problems++;
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.LaunchFacilities, (float)Mathf.Min(num12, num14));
									if (flag10)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Civilian_InitiateSpaceflightProgram, (float)num12);
									}
									num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
								}
							}
							if (flag3 || !flag5)
							{
								CS$<>8__locals4.problems += ((num14 > 0) ? 0 : 1);
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.MissionControl, (float)Mathf.Max(num12, (faction.AI_GenericMissionControlAvailable < 20) ? 2 : 0));
								if (flag10)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Civilian_InitiateSpaceflightProgram, (float)Mathf.Max(num12, (faction.AI_GenericMissionControlAvailable < 20) ? 1 : 0));
								}
								if ((!CS$<>8__locals4.CS$<>8__locals2.validPriorities.Contains(PriorityType.MissionControl) && !CS$<>8__locals4.CS$<>8__locals2.validPriorities.Contains(PriorityType.Civilian_InitiateSpaceflightProgram)) || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.missionControl >= CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.maxMissionControl)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Economy, (float)Mathf.Clamp(num12, 1, 3));
								}
								num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
							}
							if (CS$<>8__locals4.problems <= 2 || CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings.Values.Sum() <= 5 || (CS$<>8__locals4.problems < 5 && flag6))
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Economy, (float)(num12 / 2 - CS$<>8__locals4.problems));
								int num15 = 0;
								if (flag6)
								{
									num15 += 1 - CS$<>8__locals4.problems;
									if (faction.aiValues.gatherScience >= 0.25f || (flag6 && num12 > 1))
									{
										num15++;
										if (faction.aiValues.gatherScience > 1f && num12 > 2)
										{
											num15++;
										}
									}
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Knowledge, (float)num15);
								}
								if (faction.aiValues.protectHumanLife >= 1f)
								{
									if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy < 8f + faction.aiValues.protectHumanLife)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Government, (float)Mathf.Clamp(num12, 1, 3));
									}
									if (CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings.Values.Sum() <= 6)
									{
										CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Environment, (float)Mathf.Clamp(num12, 1, 3));
									}
								}
								else if (CS$<>8__locals4.problems == 0 && CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.democracy < 8f + faction.aiValues.protectHumanLife)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Government, (float)Mathf.Clamp(num12 / 2, 1, 3));
									num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Funding, (float)(num12 / 3));
									num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(PriorityType.Environment, (float)(num12 / 3));
								}
								num12 = CS$<>8__locals4.<ManageNations>g__GetIPsLeftToGive|25();
							}
							if (num12 - 6 > 0 || CS$<>8__locals4.problems == 0)
							{
								using (List<FactionGoal_Nation>.Enumerator enumerator6 = list3.GetEnumerator())
								{
									while (enumerator6.MoveNext())
									{
										FactionGoal_Nation goal = enumerator6.Current;
										if (num12 > 0)
										{
											IEnumerable<PriorityType> keys = goal.prioritiesAsNation.Keys;
											Func<PriorityType, int> func6;
											Func<PriorityType, int> <>9__34;
											if ((func6 = <>9__34) == null)
											{
												func6 = (<>9__34 = (PriorityType x) => goal.prioritiesAsNation[x]);
											}
											foreach (PriorityType priorityType6 in keys.OrderByDescending<PriorityType, int>(func6))
											{
												if (priorityType6 != PriorityType.Civilian_InitiateSpaceflightProgram && priorityType6 != PriorityType.LaunchFacilities && priorityType6 != PriorityType.MissionControl && (!flag12 || (priorityType6 != PriorityType.Military_BuildNuclearWeapons && priorityType6 != PriorityType.Military_InitiateNuclearProgram)))
												{
													int num16 = CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings[priorityType6];
													int num17 = Mathf.Min(goal.prioritiesAsNation[priorityType6], num12);
													if (CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.ValidPriority(priorityType6))
													{
														CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(priorityType6, (float)num17);
													}
													num12 -= (CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings[priorityType6] - num16) * 2;
													if (num12 <= 0)
													{
														break;
													}
												}
											}
										}
									}
								}
							}
							if (num12 > 0 && CS$<>8__locals4.problems == 0)
							{
								foreach (PriorityType priorityType7 in this.orderedPrioritiesForSpares)
								{
									CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriorityIfGreater|6(priorityType7, (float)faction.defaultPriorityPreset.GetPreset(priorityType7));
									num12 -= CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings[priorityType7] * 3;
									if (num12 <= 0)
									{
										break;
									}
								}
							}
						}
						IL_22FF:
						if (CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings.Values.Sum() == 0)
						{
							foreach (TIControlPoint ticontrolPoint4 in list2)
							{
								faction.playerControl.StartAction(new ApplyPriorityPresetToControlPoint(ticontrolPoint4, faction, faction.defaultPriorityPresetTemplateName));
							}
							foreach (PriorityType priorityType8 in Enums.PriorityTypes)
							{
								CS$<>8__locals4.CS$<>8__locals2.<ManageNations>g__SetDesiredPriority|5(priorityType8, (float)CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.controlPoints[0].GetControlPointPriority(priorityType8, true));
							}
						}
						if (CS$<>8__locals4.problems >= 10 || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.elitesHappy || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.severeInequalityWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.majorCohesionWarning || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.futureMajorCohesionWarning)
						{
							goto IL_2530;
						}
						int num18 = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.CountFactionControlPoints(faction, false, false, true);
						float corruption = CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.corruption;
						float num19 = (float)CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings.Sum<KeyValuePair<PriorityType, int>>((KeyValuePair<PriorityType, int> x) => x.Value);
						float num20 = (float)CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings[PriorityType.Spoils] / Mathf.Max(1f, num19);
						int num21 = Mathf.RoundToInt(num19 * (corruption - num20));
						if (num21 > 0 && ((!flag8 && !flag7) || CS$<>8__locals4.CS$<>8__locals2.CS$<>8__locals1.nation.inequalityWarning))
						{
							num21 /= 2;
						}
						num = CS$<>8__locals4.CS$<>8__locals2.desiredPrioritySettings[PriorityType.Spoils] * num18 + num21;
						if (num > 3 * num18)
						{
							num2 = 3 * num18 - num;
							goto IL_2530;
						}
						goto IL_2530;
					}
				}
			}
			if (day <= 28 && this.gameTime.currentTime.day % 14 == AIDailyFactionPlanner.factionAIData[faction].every14DaysOffsetLate)
			{
				Dictionary<FactionResource, float> dictionary3 = new Dictionary<FactionResource, float>
				{
					{
						FactionResource.Money,
						faction.GetCurrentResourceAmount(FactionResource.Money) * 0.5f
					},
					{
						FactionResource.Influence,
						(faction.GetCurrentResourceAmount(FactionResource.Influence) * (float)faction.councilors.Count == (float)faction.maxCouncilSize) ? 0.2f : 0.1f
					},
					{
						FactionResource.Operations,
						faction.GetCurrentResourceAmount(FactionResource.Operations) * (faction.currentlySearchingForHydraCouncilor ? 0f : 0.25f)
					}
				};
				List<TINationState> majorityControlNations = faction.majorityControlNations;
				foreach (TIFactionGoalState tifactionGoalState2 in (from x in faction.GoalsOfType(TIFactionGoalState.BenevolentNationManagementGoals, false, true)
					orderby (float)x.importance / (float)x.target().ref_nation.numControlPoints descending
					select x).ToList<TIFactionGoalState>())
				{
					TINationState ref_nation = tifactionGoalState2.target().ref_nation;
					if (majorityControlNations.Contains(ref_nation))
					{
						if (ref_nation.belligerentInActiveWar)
						{
							this.AttemptFundPriority(faction, ref_nation, PriorityType.Military, ref dictionary3);
						}
						else if ((ref_nation.unrestWarning && ref_nation.democracy < 7f) || ref_nation.civilWar)
						{
							this.AttemptFundPriority(faction, ref_nation, PriorityType.Oppression, ref dictionary3);
						}
						else if (!ref_nation.alienNation)
						{
							if (ref_nation.numControlPoints >= 3 && AIEvaluators.Abundant(faction, FactionResource.Influence, dictionary3[FactionResource.Influence], faction.GetDailyIncome(FactionResource.Influence, false, false) > 1f, 1f))
							{
								this.AttemptFundPriority(faction, ref_nation, PriorityType.Funding, ref dictionary3);
							}
							bool flag19 = faction.AI_GenericMissionControlAvailable >= 10;
							if (ref_nation.spaceFlightProgram)
							{
								if (!flag19)
								{
									this.AttemptFundPriority(faction, ref_nation, PriorityType.MissionControl, ref dictionary3);
								}
								else if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost))
								{
									this.AttemptFundPriority(faction, ref_nation, PriorityType.LaunchFacilities, ref dictionary3);
								}
							}
							else if (faction.resourceIncomeDeficiencies.Contains(FactionResource.Boost) || !flag19)
							{
								this.AttemptFundPriority(faction, ref_nation, PriorityType.Civilian_InitiateSpaceflightProgram, ref dictionary3);
							}
							else if (ref_nation.numControlPoints >= 3 && ref_nation.cohesionWarning)
							{
								if (faction.extremist)
								{
									this.AttemptFundPriority(faction, ref_nation, PriorityType.Unity, ref dictionary3);
								}
								else
								{
									this.AttemptFundPriority(faction, ref_nation, PriorityType.Knowledge, ref dictionary3);
								}
							}
							else if (ref_nation.majorCohesionWarning)
							{
								this.AttemptFundPriority(faction, ref_nation, PriorityType.Unity, ref dictionary3);
							}
						}
					}
				}
			}
		}

		// Token: 0x06005A53 RID: 23123 RVA: 0x002AC3D0 File Offset: 0x002AA5D0
		public void AttemptFundPriority(TIFactionState faction, TINationState nation, PriorityType priority, ref Dictionary<FactionResource, float> resourcesAvailable)
		{
			int num;
			if (TINationState.EverAllowedForDirectInvest(priority) && nation.CanDirectInvest(faction, priority, out num))
			{
				TIResourcesCost tiresourcesCost = nation.InvestmentPointDirectPurchasePrice(priority, faction);
				int num2 = Mathf.Min(num, tiresourcesCost.CanAfford_Count(faction, resourcesAvailable));
				if ((float)num2 > nation.DeltaToInvestmentThreshhold(priority))
				{
					faction.playerControl.StartAction(new DirectInvestAction(faction, nation, priority, (float)num2));
					foreach (ResourceValue resourceValue in tiresourcesCost.resourceCosts)
					{
						Dictionary<FactionResource, float> dictionary = resourcesAvailable;
						FactionResource resource = resourceValue.resource;
						dictionary[resource] -= resourceValue.value * (float)num2;
					}
				}
			}
		}

		// Token: 0x06005A54 RID: 23124 RVA: 0x002AC49C File Offset: 0x002AA69C
		private void ArmyOperations(TIFactionState faction)
		{
			if (this.gameTime.currentTime.day % 3 == AIDailyFactionPlanner.factionAIData[faction].every3DaysOffset)
			{
				faction.executiveNations.ForEach(delegate(TINationState x)
				{
					AIDailyFactionPlanner.ConsiderNuclearAttack(x, null, false);
				});
			}
			using (List<TIArmyState>.Enumerator enumerator = faction.armies.GetEnumerator())
			{
				Func<TINationState, bool> <>9__7;
				Func<TIArmyState, bool> <>9__6;
				while (enumerator.MoveNext())
				{
					AIDailyFactionPlanner.<>c__DisplayClass82_1 CS$<>8__locals2 = new AIDailyFactionPlanner.<>c__DisplayClass82_1();
					CS$<>8__locals2.army = enumerator.Current;
					if (!CS$<>8__locals2.army.AlienMegafaunaArmy || (int)CS$<>8__locals2.army.ID % 10 == this.gameTime.currentTime.day % 10)
					{
						bool flag = CS$<>8__locals2.army.CurrentOperations().Any<OperationData>();
						if (flag && CS$<>8__locals2.army.IsMoving && !CS$<>8__locals2.army.LegalRegion(CS$<>8__locals2.army.CurrentOperations()[0].target.ref_region))
						{
							AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new CancelArmyOperation(), CS$<>8__locals2.army, null);
						}
						else
						{
							bool flag2 = CS$<>8__locals2.army.InBattleWithArmies();
							TINationState homeNation = CS$<>8__locals2.army.homeNation;
							bool flag3 = homeNation.wars.Count > 0;
							bool flag4 = CS$<>8__locals2.army.strength < 0.65f * faction.aiValues.riskAversion;
							bool flag5 = CS$<>8__locals2.army.CanHeal();
							if (flag3)
							{
								if (CS$<>8__locals2.army.homeNation.wars.Contains(CS$<>8__locals2.army.currentNation))
								{
									TIFactionState executiveFaction = CS$<>8__locals2.army.currentNation.executiveFaction;
									if (executiveFaction != null && executiveFaction.permanentAlly(faction))
									{
										if (flag)
										{
											if (!CS$<>8__locals2.army.CurrentOperations().None<OperationData>((OperationData x) => x.operation is DeployArmyOperation))
											{
												TIRegionState finalDestination = CS$<>8__locals2.army.finalDestination;
												if (!(((finalDestination != null) ? finalDestination.nation : null) == CS$<>8__locals2.army.currentNation))
												{
													goto IL_02CF;
												}
											}
											AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new CancelArmyOperation(), CS$<>8__locals2.army, null);
											continue;
										}
										IL_02CF:
										TIRegionState armyDestination = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestSafeRegion, 4);
										if (armyDestination != null && armyDestination != CS$<>8__locals2.army.finalDestination)
										{
											AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination);
											continue;
										}
									}
								}
								CS$<>8__locals2.army.SubstantiveAvailableOperationList();
								bool flag6 = homeNation.unrest < 7f && homeNation.wars.All<TINationState>((TINationState x) => homeNation.WinningWarAgainst(x));
								bool isMoving = CS$<>8__locals2.army.IsMoving;
								TIRegionState capital = homeNation.capital;
								bool flag7 = capital != null && capital.OccupiedOrOccupationUnderway();
								bool flag8 = CS$<>8__locals2.army.InEnemyCapital();
								bool flag9 = CS$<>8__locals2.army.currentRegion == homeNation.capital;
								if (flag7 && !flag8 && !flag9)
								{
									TIRegionState armyDestination2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.MyCapital, 4);
									if (armyDestination2 != null)
									{
										if (armyDestination2 != CS$<>8__locals2.army.currentRegion && (!flag || CS$<>8__locals2.army.currentOperations[0].target != armyDestination2))
										{
											if (flag)
											{
												AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new CancelArmyOperation(), CS$<>8__locals2.army, null);
											}
											AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination2);
											continue;
										}
										continue;
									}
								}
								bool flag10 = isMoving && TIArmyState.RegionMeetsDestinationCriteria(CS$<>8__locals2.army, CS$<>8__locals2.army.currentOperations[0].target.ref_region, AIArmyDestination.NearestSafeRegion);
								bool flag11 = CS$<>8__locals2.army.OccupyingRegion(true);
								flag2 = flag2 || flag11;
								TINationState tinationState;
								float highestWarAllianceOccupationValueByNation = CS$<>8__locals2.army.currentRegion.GetHighestWarAllianceOccupationValueByNation(CS$<>8__locals2.army.homeNation, out tinationState);
								bool flag12 = CS$<>8__locals2.army.strength < 0.5f * faction.aiValues.riskAversion;
								bool flag13 = CS$<>8__locals2.army.homeNation.IsAtWarWith(CS$<>8__locals2.army.currentNation) && CS$<>8__locals2.army.faction != null && CS$<>8__locals2.army.faction == CS$<>8__locals2.army.currentNation.executiveFaction;
								if (!flag10 && !flag5 && !flag9 && (flag13 || (flag4 && !flag2) || (flag12 && (!flag11 || CS$<>8__locals2.army.strength < 1f - highestWarAllianceOccupationValueByNation))))
								{
									TIRegionState armyDestination3 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestSafeRegion, 4);
									if (armyDestination3 != null)
									{
										if (armyDestination3 != CS$<>8__locals2.army.currentRegion && (!flag || CS$<>8__locals2.army.currentOperations[0].target != armyDestination3))
										{
											if (flag)
											{
												AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new CancelArmyOperation(), CS$<>8__locals2.army, null);
											}
											AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination3);
										}
										CS$<>8__locals2.army.AI_targetEnemyRegion = null;
										continue;
									}
								}
								if ((homeNation.unrest >= 7f || (homeNation.unrest > 5f && homeNation.unrestRestState_dailyCache >= 7f)) && !CS$<>8__locals2.army.AlienMegafaunaArmy)
								{
									if (CS$<>8__locals2.army.currentNation != homeNation)
									{
										TIRegionState armyDestination4 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestHomeNationRegion, 4);
										if (armyDestination4 != null)
										{
											if (flag)
											{
												TIRegionState finalDestination2 = CS$<>8__locals2.army.finalDestination;
												if (!(((finalDestination2 != null) ? finalDestination2.nation : null) != homeNation))
												{
													continue;
												}
											}
											if (armyDestination4 != CS$<>8__locals2.army.currentRegion)
											{
												AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination4);
											}
										}
									}
								}
								else if (!flag)
								{
									if (flag5 && flag12 && CS$<>8__locals2.army.currentRegion.nation != homeNation && CS$<>8__locals2.army.currentRegion.AdjacentRegions(true).None<TIRegionState>((TIRegionState x) => x.nation.IsAtWarWith(homeNation)))
									{
										TIRegionState armyDestination5 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestSafeHomeNationRegion, 4);
										if (armyDestination5 != null && homeNation != null && armyDestination5.armies.None<TIArmyState>(delegate(TIArmyState x)
										{
											TINationState homeNation2 = x.homeNation;
											return homeNation2 != null && homeNation2.wars.Contains(CS$<>8__locals2.army.homeNation);
										}) && armyDestination5 != CS$<>8__locals2.army.currentRegion && (!flag || armyDestination5 != CS$<>8__locals2.army.finalDestination))
										{
											AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination5);
											continue;
										}
									}
									if (!flag2 && !flag5)
									{
										if (homeNation.ArmiesThreateningCapital(false, false) > homeNation.capital.FilteredArmiesPresent(true, false, false, false, true).Count<TIArmyState>((TIArmyState x) => x.CurrentOperations().Count == 0 && x.AI_targetEnemyRegion == null))
										{
											TIRegionState armyDestination6 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.MyCapital, 4);
											if (armyDestination6 != null)
											{
												if (armyDestination6 != CS$<>8__locals2.army.currentRegion && armyDestination6 != CS$<>8__locals2.army.finalDestination)
												{
													AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination6);
												}
												CS$<>8__locals2.army.AI_targetEnemyRegion = null;
												continue;
											}
										}
										IEnumerable<TIArmyState> armies = homeNation.armies;
										Func<TIArmyState, bool> func;
										if ((func = <>9__6) == null)
										{
											func = (<>9__6 = delegate(TIArmyState x)
											{
												if (!x.AlienMegafaunaArmy)
												{
													IEnumerable<TINationState> wars = x.homeNation.wars;
													Func<TINationState, bool> func2;
													if ((func2 = <>9__7) == null)
													{
														func2 = (<>9__7 = delegate(TINationState y)
														{
															TIFactionState executiveFaction2 = y.executiveFaction;
															return executiveFaction2 != null && executiveFaction2.permanentAlly(faction);
														});
													}
													return wars.All<TINationState>(func2);
												}
												return false;
											});
										}
										List<TIArmyState> list = armies.Where<TIArmyState>(func).ToList<TIArmyState>();
										bool flag14 = !flag7 && (flag6 || CS$<>8__locals2.army.homeNation.EnemyArmiesOnMyTerritory_NoMegafauna() == 0 || CS$<>8__locals2.army.armyType == ArmyType.AlienMegafauna);
										bool flag15 = CS$<>8__locals2.army.homeNation.wars.Contains(CS$<>8__locals2.army.currentNation);
										bool flag16 = false;
										if (list.Contains(CS$<>8__locals2.army))
										{
											flag16 = true;
										}
										else
										{
											List<TIArmyState> list2 = homeNation.armies.Where<TIArmyState>((TIArmyState x) => !x.AlienMegafaunaArmy).Except<TIArmyState>(list).ToList<TIArmyState>();
											int num = list2.Count - list.Count - 1;
											Func<TIArmyState, bool> <>9__11;
											int num2 = homeNation.wars.Sum<TINationState>(delegate(TINationState enemy)
											{
												IEnumerable<TIArmyState> armies2 = enemy.armies;
												Func<TIArmyState, bool> func3;
												if ((func3 = <>9__11) == null)
												{
													func3 = (<>9__11 = (TIArmyState army) => army.CanGetTo(homeNation.capital, null, null, null));
												}
												return armies2.Count<TIArmyState>(func3);
											}) + (homeNation.rivals.Any<TINationState>((TINationState x) => x.numStandardArmies > 0) ? 1 : 0);
											int num3;
											if (num2 == 0)
											{
												num3 = 0;
											}
											else if (num >= num2 * 2)
											{
												num3 = num / 3;
											}
											else
											{
												num3 = num / 2;
											}
											if (CS$<>8__locals2.army.homeNation.unrestRestState_dailyCache > 5.5f)
											{
												float num4 = CS$<>8__locals2.army.homeNation.IndividualArmyImpactOnUnrest(CS$<>8__locals2.army.faction);
												if (num4 >= 0.2f)
												{
													num3 = Mathf.Max(num3, CS$<>8__locals2.army.homeNation.armies.Count<TIArmyState>((TIArmyState x) => x.currentNation == x.homeNation) + Mathf.RoundToInt((CS$<>8__locals2.army.homeNation.unrestRestState_dailyCache - 5.5f) / num4));
												}
											}
											if (num3 > 0)
											{
												List<TIArmyState> list3 = (from x in new List<TIArmyState>(list2)
													where !x.AlienMegafaunaArmy
													orderby x.deploymentType, x.controlPointIdx
													select x).ToList<TIArmyState>();
												List<TIArmyState> list4 = new List<TIArmyState>();
												foreach (TIArmyState tiarmyState in list3)
												{
													list4.Add(tiarmyState);
													if (list4.Count >= num3)
													{
														break;
													}
													if (CS$<>8__locals2.army == tiarmyState)
													{
														break;
													}
												}
												flag16 = list4.Contains(CS$<>8__locals2.army);
												if (flag16)
												{
													CS$<>8__locals2.army.AI_targetEnemyRegion = null;
												}
											}
										}
										if (flag14 && !flag16)
										{
											if (flag15 && (CS$<>8__locals2.army.currentNation.executiveFaction == null || !CS$<>8__locals2.army.currentNation.executiveFaction.permanentAlly(faction)))
											{
												if (CS$<>8__locals2.army.currentRegion.antiSpaceDefenses)
												{
													AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new AssaultSpaceFacilityOperation(), CS$<>8__locals2.army.currentRegion.spaceDefenseFacility, null);
													continue;
												}
												if (faction.GoalsWithTarget(CS$<>8__locals2.army.currentNation, GoalType.NeutralizeNation, true).Count > 0)
												{
													if (CS$<>8__locals2.army.currentRegion.missionControlFacility.Extant())
													{
														AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new AssaultSpaceFacilityOperation(), CS$<>8__locals2.army.currentRegion.missionControlFacility, null);
														continue;
													}
													if (CS$<>8__locals2.army.currentRegion.boostFacility.Extant())
													{
														AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new AssaultSpaceFacilityOperation(), CS$<>8__locals2.army.currentRegion.boostFacility, null);
														continue;
													}
													if (faction.aiValues.protectHumanLife <= 0.5f && (CS$<>8__locals2.army.currentRegion.coreEconomicRegion || CS$<>8__locals2.army.currentRegion.coreResourceRegion) && CS$<>8__locals2.army.currentNation.numStandardArmies == 0)
													{
														AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new RazeRegionOperation(), CS$<>8__locals2.army.currentRegion, null);
														continue;
													}
												}
												else if (CS$<>8__locals2.army.homeNation.wars.Contains(CS$<>8__locals2.army.currentNation) && CS$<>8__locals2.army.homeNation.claims.Contains(CS$<>8__locals2.army.currentRegion) && CS$<>8__locals2.army.currentRegion != CS$<>8__locals2.army.currentNation.capital && CS$<>8__locals2.army.currentNation.numNuclearWeapons <= 1 && CS$<>8__locals2.army.currentNation.numStandardArmies <= 1 && CS$<>8__locals2.army.homeNation.numStandardArmies <= 3 + CS$<>8__locals2.army.currentNation.numStandardArmies && faction.GoalsWithTarget(CS$<>8__locals2.army.homeNation, GoalType.PillageNation, true).Count == 0)
												{
													AnnexRegionOperation annexRegionOperation = new AnnexRegionOperation();
													if (annexRegionOperation.ActorCanPerformOperation(CS$<>8__locals2.army, CS$<>8__locals2.army.currentRegion))
													{
														AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, annexRegionOperation, CS$<>8__locals2.army.currentRegion, null);
														continue;
													}
													if (CS$<>8__locals2.army.currentNation.numStandardArmies == 0 && CS$<>8__locals2.army.faction.permanentAlly(homeNation.executiveFaction) && CS$<>8__locals2.army.strength < 1f)
													{
														if (CS$<>8__locals2.army.currentRegion.armies.None<TIArmyState>((TIArmyState x) => x.IsMoving))
														{
															TIRegionState armyDestination7 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestSafeRegion, 4);
															if (armyDestination7 != null && armyDestination7 != CS$<>8__locals2.army.currentRegion)
															{
																AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination7);
																continue;
															}
														}
													}
												}
											}
											TIRegionState destination2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestOffensiveBattle, 4) ?? TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestEnemyRegion, 4);
											if (destination2 != null && destination2 != CS$<>8__locals2.army.currentRegion && CS$<>8__locals2.army.homeNation.wars.Contains(destination2.nation))
											{
												AIDailyFactionPlanner.<>c__DisplayClass82_4 CS$<>8__locals5;
												CS$<>8__locals5.enemyNation = destination2.nation;
												if (!CS$<>8__locals2.<ArmyOperations>g__advantageDestination|17(destination2, ref CS$<>8__locals5))
												{
													List<TIRegionState> list5 = (from x in CS$<>8__locals2.army.homeNation.CurrentWarAllianceArmies(CS$<>8__locals5.enemyNation)
														select x.AI_targetEnemyRegion into x
														where x != null && CS$<>8__locals2.army.CanGetTo(x, null, null, null) && x != destination2 && x != CS$<>8__locals2.army.currentRegion
														select x).ToList<TIRegionState>();
													if (list5.Count <= 0)
													{
														CS$<>8__locals2.army.AI_targetEnemyRegion = destination2;
														continue;
													}
													TIRegionState tiregionState = (from x in list5
														group x by x into grp
														orderby grp.Count<TIRegionState>() descending
														select grp.Key).FirstOrDefault<TIRegionState>();
													if (tiregionState != null)
													{
														CS$<>8__locals2.army.AI_targetEnemyRegion = tiregionState;
														if (CS$<>8__locals2.<ArmyOperations>g__advantageDestination|17(tiregionState, ref CS$<>8__locals5))
														{
															destination2 = tiregionState;
														}
													}
												}
											}
											if (destination2 != null && destination2 != CS$<>8__locals2.army.currentRegion)
											{
												AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, destination2);
											}
										}
										else
										{
											TIRegionState tiregionState2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestDefensiveBattle, 4);
											if (tiregionState2 != null)
											{
												if (tiregionState2 != CS$<>8__locals2.army.currentRegion)
												{
													AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, tiregionState2);
												}
											}
											else
											{
												tiregionState2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestOccupiedFriendlyRegion, 4);
												if (tiregionState2 != null)
												{
													if (tiregionState2 != CS$<>8__locals2.army.currentRegion)
													{
														AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, tiregionState2);
													}
												}
												else
												{
													tiregionState2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestAlliedBorderWithEnemyArmy, 4);
													if (tiregionState2 != null)
													{
														if (tiregionState2 != CS$<>8__locals2.army.currentRegion)
														{
															AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, tiregionState2);
														}
													}
													else
													{
														tiregionState2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestAlliedBorderWithEnemy, 4);
														if (tiregionState2 != null)
														{
															if (tiregionState2 != CS$<>8__locals2.army.currentRegion)
															{
																AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, tiregionState2);
															}
														}
														else
														{
															tiregionState2 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.MyHome, 4);
															if (tiregionState2 != null && tiregionState2 != CS$<>8__locals2.army.currentRegion)
															{
																AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, tiregionState2);
															}
														}
													}
												}
											}
										}
									}
								}
							}
							else if ((flag2 || (flag4 && !flag5) || (int)CS$<>8__locals2.army.ID % 4 == TITimeState.CampaignDuration_days() % 4) && !flag)
							{
								List<IOperation> list6 = CS$<>8__locals2.army.SubstantiveAvailableOperationList();
								if (CS$<>8__locals2.army.AlienMegafaunaArmy)
								{
									if (!CS$<>8__locals2.army.ref_megafaunaArmyState.AI_DesiredRegion(CS$<>8__locals2.army.currentRegion))
									{
										TIRegionState armyDestination8 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.MegafaunaDestination, 0);
										if (armyDestination8 != null && armyDestination8 != CS$<>8__locals2.army.currentRegion)
										{
											AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination8);
										}
									}
								}
								else
								{
									TIRegionState armyDestination9 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestDefensiveBattle, 0);
									if (armyDestination9 != null && (armyDestination9.nation == CS$<>8__locals2.army.homeNation || CS$<>8__locals2.army.homeNation.numStandardArmies > 1 || CS$<>8__locals2.army.ref_controlPoint != homeNation.executiveControlPoint))
									{
										if (armyDestination9 != CS$<>8__locals2.army.currentRegion)
										{
											AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination9);
										}
									}
									else
									{
										if (homeNation.BaseInvestmentPoints_month() >= 2f + TemplateManager.global.nationalInvestmentArmyFactorAway * (float)(1 + homeNation.deployedArmies))
										{
											if (homeNation.rivals.Count == 0 && homeNation.unrest >= TINationState.minUnrestForSecession - 2f)
											{
												TIRegionState armyDestination10 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestPotentialBreakaway, 0);
												if (armyDestination10 != null)
												{
													if (armyDestination10 != CS$<>8__locals2.army.currentRegion)
													{
														AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination10);
														continue;
													}
													continue;
												}
											}
											else
											{
												if (!CS$<>8__locals2.army.ref_faction.proAlien)
												{
													TIRegionState destination = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestAlienFacility, 0);
													if (destination != null && CS$<>8__locals2.army.ref_faction.activeCouncilors.Any<TICouncilorState>(delegate(TICouncilorState x)
													{
														TIMissionState activeMission = x.activeMission;
														return ((activeMission != null) ? activeMission.target : null) != destination.ref_alienFacility;
													}))
													{
														if (destination != CS$<>8__locals2.army.currentRegion)
														{
															AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, destination);
															continue;
														}
														if (list6.Contains(OperationsManager.operationsLookup[typeof(AssaultAlienAssetOperation)]))
														{
															AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new AssaultAlienAssetOperation(), CS$<>8__locals2.army.currentRegion.alienFacility, null);
															continue;
														}
														continue;
													}
												}
												if (!CS$<>8__locals2.army.ref_faction.permanentAlly(GameStateManager.AlienFaction()))
												{
													TIRegionState armyDestination11 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.NearestAlienXenoformingThreat, 0);
													if (armyDestination11 != null && (armyDestination11.nation == CS$<>8__locals2.army.homeNation || CS$<>8__locals2.army.homeNation.numStandardArmies == 1 || CS$<>8__locals2.army.ref_controlPoint != homeNation.executiveControlPoint))
													{
														if (armyDestination11 != CS$<>8__locals2.army.currentRegion)
														{
															AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination11);
															continue;
														}
														if (list6.Contains(OperationsManager.operationsLookup[typeof(AssaultAlienAssetOperation)]))
														{
															AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new AssaultAlienAssetOperation(), CS$<>8__locals2.army.currentRegion.xenoforming, null);
															continue;
														}
														continue;
													}
												}
											}
										}
										if (CS$<>8__locals2.army.currentRegion != CS$<>8__locals2.army.homeRegion && CS$<>8__locals2.army.homeRegion != null)
										{
											TIRegionState armyDestination12 = TIArmyState.GetArmyDestination(CS$<>8__locals2.army, AIArmyDestination.MyHome, 4);
											if (armyDestination12 == null && CS$<>8__locals2.army.homeRegion != null && list6.Contains(OperationsManager.operationsLookup[typeof(ArmyGoHomeOperation)]))
											{
												AIDailyFactionPlanner.LaunchOperation(CS$<>8__locals2.army, new ArmyGoHomeOperation(), CS$<>8__locals2.army.homeRegion, null);
											}
											else if (armyDestination12 != null)
											{
												AIDailyFactionPlanner.LaunchDeployArmyOperation(CS$<>8__locals2.army, armyDestination12);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06005A55 RID: 23125 RVA: 0x002ADFFC File Offset: 0x002AC1FC
		public static double SelectTrajectoryAsync(IMobileAsset fleet, TIGameState destination, float desiredReserveDVFraction, out TransferResult result, Action<Trajectory> callback, bool mayUseReserveDV = false, double sampleSizeMultiplier = 1.0)
		{
			double num2;
			try
			{
				double num;
				result = MasterTransferPlanner.RequestTrajectories(fleet, destination, 64, delegate(Trajectory[] trajectories)
				{
					if (trajectories.Length == 0)
					{
						return;
					}
					float fleetDVToSpend = fleet.currentDeltaV_mps * (1f - desiredReserveDVFraction);
					Trajectory trajectory = null;
					List<Trajectory> list = trajectories.Where<Trajectory>((Trajectory x) => x.DV_mps <= (double)fleetDVToSpend && x.launchTime != x.arrivalTime).ToList<Trajectory>();
					if (list.Count > 0)
					{
						trajectory = list.MinBy<Trajectory, TimeSpan>((Trajectory x) => x.duration);
					}
					else if (mayUseReserveDV)
					{
						trajectory = trajectories.MinBy<Trajectory, double>((Trajectory x) => x.DV_kps);
					}
					callback(trajectory);
				}, out num, false, false, sampleSizeMultiplier);
				num2 = num;
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message + "\n" + ex.StackTrace, Array.Empty<object>());
				result = new TransferResult(TransferResult.Outcome.Fail_CodePathNotImplemented, 0.0, 0.0);
				num2 = double.PositiveInfinity;
			}
			return num2;
		}

		// Token: 0x06005A56 RID: 23126 RVA: 0x002AE0AC File Offset: 0x002AC2AC
		public static bool SingleFleetOperation(TISpaceFleetState fleet, bool allowRecursiveCalls = true)
		{
			TIFactionState faction = fleet.faction;
			FactionGoal_Fleet fleetGoal = fleet.AssignedGoal();
			if (fleet.inTransfer && fleetGoal != null && !(fleetGoal is FactionGoal_FixUpFleet))
			{
				if (!fleet.trajectory.endsInCrash && !fleet.trajectory.exitsSolarSystem)
				{
					TIOrbitState destinationOrbit = fleet.trajectory.destinationOrbit;
					if (destinationOrbit == null || !destinationOrbit.isAdHocOrbit || (fleet.trajectory.arrivalTime - TITimeState.Now()).TotalDays <= 90.0)
					{
						goto IL_00D8;
					}
				}
				fleetGoal.UnassignFleet();
			}
			IL_00D8:
			if (fleet.inCombat || !fleet.mayLegallyStartATransfer || fleet.currentOperations.Count > 0)
			{
				return false;
			}
			FactionGoal_AttackWithFleet attackGoal = fleetGoal as FactionGoal_AttackWithFleet;
			FactionGoal_Fleet fleetGoal4 = fleetGoal;
			if (attackGoal != null && attackGoal.bombardmentGoal)
			{
				FactionGoal_AttackWithFleet attackGoal3 = attackGoal;
			}
			bool flag = !fleet.dockedAtStation || (fleet.faction.permanentAlly(fleet.ref_hab.faction) && !(fleetGoal is FactionGoal_FixUpFleet));
			if (flag)
			{
				List<TISpaceShipState> list = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.isCapableOfTransfering).ToList<TISpaceShipState>();
				if (list.Count == fleet.ships.Count && !fleet.dockedAtHab)
				{
					return false;
				}
				if (list.Count > 0 && new SplitFleetOperation().ActorCanPerformOperation(fleet, null))
				{
					fleet.faction.playerControl.StartAction(new SplitFleetOperationAction(fleet, list, null));
					return false;
				}
				List<TISpaceShipState> list2 = new List<TISpaceShipState>();
				FactionGoal_Fleet fleetGoal2 = fleetGoal;
				if (fleetGoal2 != null && fleetGoal2.skipGoal)
				{
					fleetGoal = null;
				}
				if ((fleetGoal == null || fleetGoal.SpaceCombatGoal()) && fleet.AI_NeedsRepairBadly() && !fleet.dockedAtHab)
				{
					List<TISpaceShipState> list3 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.badlyDamaged).ToList<TISpaceShipState>();
					List<TISpaceShipState> list4 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.AI_InvoluntaryNoncombatant()).ToList<TISpaceShipState>();
					if (fleetGoal == null || !TIGameState.Valid(fleetGoal.target()))
					{
						list2.AddRangeUnique<TISpaceShipState>(list3);
						list2.AddRangeUnique<TISpaceShipState>(list4);
					}
					else if (fleetGoal.GetGoalType() == GoalType.AttackWithFleet)
					{
						if (fleetGoal.target().isSpaceFleetState || (fleetGoal.target().isHabState && fleetGoal.target().ref_hab.IsStation))
						{
							list2.AddRangeUnique<TISpaceShipState>(list3);
							list2.AddRangeUnique<TISpaceShipState>(list4);
						}
						else
						{
							list2.AddRangeUnique<TISpaceShipState>(list3);
							list2.AddRangeUnique<TISpaceShipState>(list4);
						}
					}
					else if (fleetGoal.GetGoalType() == GoalType.CaptureHab && fleet.location != fleetGoal.target().ref_hab.location)
					{
						list2.AddRangeUnique<TISpaceShipState>(list3);
						list2.AddRangeUnique<TISpaceShipState>(list4.Where<TISpaceShipState>((TISpaceShipState x) => x.AssaultCombatValue(false) == 0f).ToList<TISpaceShipState>());
					}
					else if (fleetGoal.GetGoalType() == GoalType.DefendWithFleet)
					{
						list2.AddRangeUnique<TISpaceShipState>(list3);
						list2.AddRangeUnique<TISpaceShipState>(list4);
					}
				}
				if (list2.Count == fleet.ships.Count)
				{
					FactionGoal_Fleet fleetGoal3 = fleetGoal;
					if (fleetGoal3 != null)
					{
						fleetGoal3.UnassignFleet();
					}
					return false;
				}
				if (list2.Count > 0 && new SplitFleetOperation().ActorCanPerformOperation(fleet, null))
				{
					fleet.faction.playerControl.StartAction(new SplitFleetOperationAction(fleet, list2, null));
					return false;
				}
			}
			if (fleetGoal == null || fleetGoal.skipGoal)
			{
				return false;
			}
			if (flag)
			{
				if (fleetGoal is FactionGoal_FixUpFleet && fleet.CanRefitAtLocation())
				{
					List<TIHabModuleState> list5 = fleet.ref_hab.CompletedShipyards();
					Dictionary<TISpaceShipState, TISpaceShipTemplate> refitsAvailable = fleet.RefitsAvailable;
					if (list5.Count > 0 && refitsAvailable.Count > 0)
					{
						int num = 0;
						foreach (TISpaceShipState tispaceShipState in refitsAvailable.Keys)
						{
							if (tispaceShipState.NeedsRefit)
							{
								TIHabModuleState tihabModuleState = list5[num++ % list5.Count];
								TIResourcesCost tiresourcesCost = refitsAvailable[tispaceShipState].RefitResourceCost(tihabModuleState, tispaceShipState.template, true, true, tispaceShipState);
								if (!tiresourcesCost.CanAfford_AI(faction, null, tihabModuleState, fleetGoal.importance, false, false, AIEvaluators.SpaceResourcesForShipBuild(fleetGoal), null, float.PositiveInfinity))
								{
									tiresourcesCost = tiresourcesCost.GetBoostSubstitutedCost(faction, tihabModuleState, false, null);
									if (tiresourcesCost.CanAfford_AI(faction, null, tihabModuleState, fleetGoal.importance, false, false, AIEvaluators.SpaceResourcesForShipBuild(fleetGoal), null, float.PositiveInfinity) && AIEvaluators.ShouldSpendBoostAtShipyard(faction, tihabModuleState, tiresourcesCost.GetSingleCostValue(FactionResource.Boost), fleetGoal))
									{
										fleet.faction.playerControl.StartAction(new AddShipDesignToConstructionQueueAction(tihabModuleState, refitsAvailable[tispaceShipState], true, 1f, null, true, tispaceShipState.template, tispaceShipState));
									}
								}
								else
								{
									fleet.faction.playerControl.StartAction(new AddShipDesignToConstructionQueueAction(tihabModuleState, refitsAvailable[tispaceShipState], false, 1f, null, true, tispaceShipState.template, tispaceShipState));
								}
								List<ShipConstructionQueueItem> shipyardQueue = faction.GetShipyardQueue(tihabModuleState);
								if (shipyardQueue.Count > 0)
								{
									AIDailyFactionPlanner.AdjustShipyardQueue(faction, shipyardQueue.Last<ShipConstructionQueueItem>());
								}
							}
						}
						if (!fleet.ships.Any<TISpaceShipState>())
						{
							return false;
						}
					}
				}
				bool flag2 = AIDailyFactionPlanner.<SingleFleetOperation>g__MustStayPut|84_7(fleet);
				bool flag3;
				if (fleet.ref_naturalSpaceObject != null)
				{
					TIGameState ref_naturalSpaceObject = fleet.ref_naturalSpaceObject;
					TIGameState tigameState = fleetGoal.target();
					flag3 = ref_naturalSpaceObject == ((tigameState != null) ? tigameState.ref_naturalSpaceObject : null);
				}
				else
				{
					flag3 = false;
				}
				bool flag4 = flag3;
				bool flag5 = fleet.ref_naturalSpaceObject != null && (fleet.ref_naturalSpaceObject.stationsInOrbit.Any<TIHabState>((TIHabState x) => x.faction == faction) || fleet.ref_naturalSpaceObject.fleetsInOrbit.Any<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(AIDailyFactionPlanner.<SingleFleetOperation>g__MustStayPut|84_7)));
				if ((flag2 || (!flag4 && flag5)) && (fleet.ships.Count > 1 || fleetGoal.desiredFleetCombatValue == 0f || fleetGoal.GetGoalType() == GoalType.DefendWithFleet))
				{
					float desiredFleetCombatValue = fleetGoal.desiredFleetCombatValue;
					float num2 = fleet.SpaceCombatValue() / fleetGoal.desiredFleetCombatValue;
					float maximumFleetCombatValueRatio = fleetGoal.GetMaximumFleetCombatValueRatio();
					int num3 = int.MaxValue;
					float num4 = float.PositiveInfinity;
					if (fleetGoal.GetGoalType() == GoalType.DefendWithFleet)
					{
						TIGameState tigameState2 = fleetGoal.target();
						bool? flag6;
						if (tigameState2 == null)
						{
							flag6 = null;
						}
						else
						{
							TISpaceBodyState ref_system = tigameState2.ref_system;
							flag6 = ((ref_system != null) ? new bool?(ref_system.isEarth) : null);
						}
						bool? flag7 = flag6;
						if (!flag7.GetValueOrDefault())
						{
							float num5 = faction.GetStaticFleetFraction() - faction.GetDesiredStaticFleetFraction();
							num4 = faction.fleets.Sum<TISpaceFleetState>((TISpaceFleetState x) => x.SpaceCombatValue()) * num5;
							if (num5 <= 0f)
							{
								num3 = 0;
							}
							else
							{
								num3 = fleet.ships.Count - 1;
							}
						}
					}
					if (!(fleetGoal is FactionGoal_JoinFleet) && !(fleetGoal is FactionGoal_AssembleFleet) && !(fleetGoal is FactionGoal_FixUpFleet) && desiredFleetCombatValue > 0f && num2 > maximumFleetCombatValueRatio && num3 > 0)
					{
						List<TISpaceShipState> list6 = new List<TISpaceShipState>();
						List<TISpaceShipState> list7 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.SpecialModuleRules(true).Intersect<SpecialModuleRule>(TIShipPartTemplate.PrimaryRoleModules).Any<SpecialModuleRule>() || x.councilorPassengers.Count > 0 || faction.councilors.Any<TICouncilorState>(delegate(TICouncilorState y)
						{
							TIMissionState activeMission = y.activeMission;
							return ((activeMission != null) ? activeMission.target : null) == x;
						}) || x.hull.dataName == "AlienMothership").ToList<TISpaceShipState>();
						IEnumerable<TISpaceShipState> enumerable = Enumerable.Empty<TISpaceShipState>();
						if (attackGoal != null && attackGoal.bombardmentGoal)
						{
							enumerable = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => x.BombardmentValue(attackGoal.target().ref_spaceBody) <= 0f);
						}
						float num6 = fleet.SpaceCombatValue();
						while (fleet.ships.Count > 0 && list7.Count + list6.Count < fleet.ships.Count && num6 / desiredFleetCombatValue > maximumFleetCombatValueRatio && list6.Count <= num3)
						{
							IEnumerable<TISpaceShipState> enumerable2 = fleet.ships.Except<TISpaceShipState>(list7).Except<TISpaceShipState>(list6);
							if (enumerable2.Intersect<TISpaceShipState>(enumerable).Any<TISpaceShipState>())
							{
								enumerable2 = enumerable2.Intersect<TISpaceShipState>(enumerable);
							}
							TISpaceShipState tispaceShipState2 = enumerable2.SelectRandomItem<TISpaceShipState>();
							if (tispaceShipState2.SpaceCombatValue(false, 0f) + list6.Sum<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f)) > num4)
							{
								break;
							}
							list6.Add(tispaceShipState2);
							num6 -= tispaceShipState2.SpaceCombatValue(false, 0f);
						}
						if (list6.Count == fleet.ships.Count)
						{
							fleetGoal.UnassignFleet();
							return false;
						}
						if (list6.Count > 1)
						{
							list6.RemoveAt(list6.Count - 1);
							fleet.faction.playerControl.StartAction(new SplitFleetOperationAction(fleet, list6, null));
						}
					}
				}
			}
			List<IOperation> list8 = fleet.AvailableOperationList(null);
			if (attackGoal != null && attackGoal.bombardmentGoal && fleet.dockedAtStation)
			{
				if (faction.AI_AtWarWithFaction(fleet.ref_hab.faction))
				{
					if (list8.Any<IOperation>((IOperation x) => x is DestroyHabOperation))
					{
						goto IL_0C2C;
					}
				}
				TIGameState ref_spaceBody = fleet.ref_spaceBody;
				TIGameState tigameState3 = attackGoal.target();
				if (ref_spaceBody == ((tigameState3 != null) ? tigameState3.ref_spaceBody : null))
				{
					TIOrbitState orbitState = fleet.orbitState;
					if (orbitState != null && orbitState.interfaceOrbit)
					{
						if (!list8.Any<IOperation>((IOperation x) => x is RepairFleetOperation))
						{
							if (!list8.Any<IOperation>((IOperation x) => x is ResupplyOperation) && attackGoal.ReadyForTransferToTarget(fleet))
							{
								fleet.DepartFromDockingLocation();
								list8 = fleet.AvailableOperationList(null);
							}
						}
					}
				}
			}
			IL_0C2C:
			List<Type> list9 = new List<Type>
			{
				typeof(AlienCrashdownOperation),
				typeof(AlienLandArmyOperation),
				typeof(DestroyHabOperation)
			};
			List<Type> list10 = new List<Type>();
			foreach (Type type in fleetGoal.fleetOperations.Concat<Type>(list9).Distinct<Type>())
			{
				IOperation operation = OperationsManager.operationsLookup[type];
				if (list8.Contains(operation))
				{
					list10.Add(type);
				}
			}
			TIGameState ref_hab;
			Type bestOperation = fleetGoal.GetBestOperation(fleet, list10, out ref_hab);
			if (bestOperation != null)
			{
				IOperation operation2 = OperationsManager.operationsLookup[bestOperation];
				if (operation2.RequiresThrustProfile())
				{
					if (!ref_hab.isSpaceFleetState && !ref_hab.isHabState && ref_hab.ref_hab != null)
					{
						ref_hab = ref_hab.ref_hab;
					}
					if ((operation2 as TransferOperation).ValidTransferDestinationForFleet(fleet, ref_hab))
					{
						bool flag8 = ref_hab.ref_hab != null && ref_hab.ref_hab.faction.permanentAlly(faction) && ref_hab.ref_hab.AllowsResupply(fleet.faction, false, false);
						if (!flag8 && ref_hab.ref_spaceBody != null && ref_hab.ref_spaceBody.habs.Where<TIHabState>((TIHabState x) => x.AllowsResupply(faction, false, false) && (x.IsStation || fleet.AI_LegsToUseSiteForResupply(x.ref_habSite, fleet.currentDeltaV_kps - fleet.maxDeltaV_kps * 0.2f))).Any<TIHabState>())
						{
							flag8 = true;
						}
						float num7;
						if (!fleetGoal.SpaceCombatGoal())
						{
							if ((ref_hab.ref_hab != null && ref_hab.ref_hab.AllowsResupply(fleet.faction, false, false)) || fleetGoal.FoundHabGoal() || fleetGoal.GetGoalType() == GoalType.TransportCouncilorsViaFleet || (ref_hab.isOrbitState && ref_hab.ref_orbit.stationsInOrbit.Any<TIHabState>((TIHabState x) => x.AllowsResupply(faction, false, false) && x.IsSafeToVisit(fleet))) || (ref_hab.isOrbitState && ref_hab.ref_orbit.interfaceOrbit && fleetGoal.target().isHabState && fleetGoal.target().ref_hab.IsBase && ref_hab.ref_orbit.barycenter == fleetGoal.target().ref_habSite.parentBody && fleetGoal.target().ref_hab.AllowsResupply(faction, false, false)))
							{
								num7 = ((ref_hab.ref_orbit != null && ref_hab.ref_orbit == fleet.ref_orbit) ? 0.01f : 0.2f);
							}
							else
							{
								num7 = ((ref_hab.ref_orbit != null && ref_hab.ref_orbit == fleet.ref_orbit) ? 0.01f : 0.5f);
							}
						}
						else if ((ref_hab.ref_faction == null || ref_hab.ref_faction == fleet.faction) && fleet.faction.CanResupplyShipsAtLocation(ref_hab, false))
						{
							num7 = 0.2f;
						}
						else
						{
							num7 = 0.5f;
						}
						Trajectory trajectory = null;
						TransferResult transferResult;
						double num8 = AIDailyFactionPlanner.SelectTrajectoryAsync(fleet, ref_hab, num7, out transferResult, delegate(Trajectory trajectory_)
						{
							trajectory = trajectory_;
						}, flag8, 1.0);
						FactionGoal_AttackWithFleet attackGoal2 = attackGoal;
						TISpaceFleetState tispaceFleetState;
						if (attackGoal2 == null)
						{
							tispaceFleetState = null;
						}
						else
						{
							TIGameState tigameState4 = attackGoal2.target();
							tispaceFleetState = ((tigameState4 != null) ? tigameState4.ref_fleet : null);
						}
						TISpaceFleetState tispaceFleetState2 = tispaceFleetState;
						if (((ref_hab != null) ? ref_hab.ref_fleet : null) != null && !faction.permanentAlly(ref_hab.ref_fleet.ref_faction))
						{
							tispaceFleetState2 = ref_hab.ref_fleet;
						}
						if (tispaceFleetState2 != null)
						{
							if (!tispaceFleetState2.dockedAtHab && (!faction.IsAlienFaction || !(tispaceFleetState2.ref_system == faction.primarySystem)))
							{
								fleetGoal.learnedPerformanceRequirements.GiveChaseDVLowerBound(tispaceFleetState2.ships.Average<TISpaceShipState>((TISpaceShipState x) => x.currentMaxDeltaV_kps) * (faction.IsAlienFaction ? 1f : 0.8f));
								fleetGoal.learnedPerformanceRequirements.GiveChaseAccelerationLowerBound(tispaceFleetState2.ships.Average<TISpaceShipState>((TISpaceShipState x) => x.pursuitAcceleration_mps2));
							}
							else
							{
								fleetGoal.learnedPerformanceRequirements.ClearChaseRequirements();
							}
						}
						if (!double.IsInfinity(num8) && !double.IsNaN(num8) && num8 > 0.0)
						{
							fleetGoal.learnedPerformanceRequirements.RegisterDVRequirement(fleet.location, (float)(num8 / (double)(1f - num7)));
						}
						if (trajectory == null)
						{
							if (num8 == 0.0)
							{
								TIHabState tihabState = ref_hab as TIHabState;
								if (tihabState != null && tihabState.IsStation && !fleet.PrecludeDockingWithEnemyStation(tihabState))
								{
									fleet.faction.playerControl.StartAction(new ApproachDockAction(tihabState, fleet));
									if (fleet.dockedAtStation && !fleet.unavailableForOperations)
									{
										AIDailyFactionPlanner.AIReaction(AIReactionEvent.PostCombatFleetRecovery, fleet, tihabState);
									}
								}
							}
							else
							{
								if (!ref_hab.isSpaceFleetState)
								{
									fleet.unreachableLocations.Add(ref_hab);
								}
								if (fleetGoal.resupplyHab == ref_hab)
								{
									fleetGoal.resupplyHab = null;
								}
							}
						}
						if (trajectory != null && fleetGoal.learnedPerformanceRequirements.MeetsRequirements(fleet, null))
						{
							operation2.OnOperationConfirm(fleet, ref_hab, null, trajectory);
							return true;
						}
						if (!ref_hab.isSpaceFleetState || !ref_hab.ref_fleet.transferAssigned || !(ref_hab.ref_fleet.trajectory.destination != null) || !(ref_hab.ref_fleet.trajectory.destination.ref_naturalSpaceObject.GetSunOrbitingRelatedObject == fleet.ref_naturalSpaceObject.GetSunOrbitingRelatedObject))
						{
							if (num8 < 0.0 && transferResult.Result != TransferResult.Outcome.Fail_BurnLongerThanTransfer)
							{
								fleetGoal.UnassignFleet();
							}
							else
							{
								List<TISpaceShipState> list11 = fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => !fleetGoal.learnedPerformanceRequirements.MeetsRequirements(x, null)).ToList<TISpaceShipState>();
								TransferResult.Outcome result = transferResult.Result;
								if (result != TransferResult.Outcome.Fail_LaunchInPast)
								{
									if (result != TransferResult.Outcome.Fail_InsufficientAcceleration)
									{
										if (result == TransferResult.Outcome.Fail_BurnLongerThanTransfer)
										{
											list11.AddUnique(fleet.ships.MinBy<TISpaceShipState, float>((TISpaceShipState x) => x.cruiseAcceleration_mps2));
										}
									}
									else
									{
										list11.AddRangeUnique<TISpaceShipState>(fleet.ships.Where<TISpaceShipState>((TISpaceShipState x) => (double)x.cruiseAcceleration_mps2 <= transferResult.Value).ToList<TISpaceShipState>());
									}
								}
								else if (list11.Count == 0)
								{
									list11.AddUnique(fleet.ships.MinBy<TISpaceShipState, float>((TISpaceShipState x) => x.cruiseAcceleration_mps2));
								}
								if (list11.Count > 0)
								{
									if (list11.Count == fleet.ships.Count)
									{
										if (!(fleetGoal is FactionGoal_FixUpFleet))
										{
											fleetGoal.UnassignFleet();
										}
									}
									else if (list8.Contains(OperationsManager.operationsLookup[typeof(SplitFleetOperation)]))
									{
										fleet.faction.playerControl.StartAction(new SplitFleetOperationAction(fleet, list11, null));
										if (allowRecursiveCalls)
										{
											AIDailyFactionPlanner.SingleFleetOperation(fleet, false);
										}
									}
								}
							}
						}
					}
				}
				else if (operation2.GetPossibleTargets(fleet, null).Contains(ref_hab))
				{
					operation2.OnOperationConfirm(fleet, ref_hab, null, null);
				}
			}
			return false;
		}

		// Token: 0x06005A57 RID: 23127 RVA: 0x002AF5A4 File Offset: 0x002AD7A4
		private IEnumerator PeriodicFleetOperations(TIFactionState faction)
		{
			if (this.fleetOperationsLive)
			{
				yield return null;
			}
			this.fleetOperationsLive = true;
			Queue<TISpaceFleetState> fleetQueue = new Queue<TISpaceFleetState>(faction.fleets.OrderByDescending<TISpaceFleetState, bool>(delegate(TISpaceFleetState x)
			{
				FactionGoal_Fleet factionGoal_Fleet = x.AssignedGoal();
				return factionGoal_Fleet != null && factionGoal_Fleet.GetGoalType() == GoalType.JoinFleet;
			}));
			while (fleetQueue.Count > 0)
			{
				if (!TIUtilities.IsThereUnresolvedCombats)
				{
					TISpaceFleetState tispaceFleetState = fleetQueue.Dequeue();
					if (!tispaceFleetState.deleted)
					{
						AIDailyFactionPlanner.SingleFleetOperation(tispaceFleetState, true);
					}
				}
				yield return null;
			}
			List<FactionGoal_AttackWithFleet> list = (from x in faction.GoalsOfType(GoalType.AttackWithFleet, true, true)
				select x as FactionGoal_AttackWithFleet).Where<FactionGoal_AttackWithFleet>(new Func<FactionGoal_AttackWithFleet, bool>(AIDailyFactionPlanner.<PeriodicFleetOperations>g__IsPotentialSTOFighterAttackGoal|86_1)).ToList<FactionGoal_AttackWithFleet>();
			Func<TISpaceFleetState, bool> <>9__3;
			foreach (FactionGoal_AttackWithFleet factionGoal_AttackWithFleet in list)
			{
				if (AIDailyFactionPlanner.<PeriodicFleetOperations>g__IsPotentialSTOFighterAttackGoal|86_1(factionGoal_AttackWithFleet))
				{
					IEnumerable<TISpaceFleetState> defenders = TIFactionState.GetDefenders(factionGoal_AttackWithFleet.target() as TISpaceObjectState);
					TIGameState tigameState = factionGoal_AttackWithFleet.target();
					TIHabState tihabState = ((tigameState != null) ? tigameState.ref_hab : null);
					if (TIGameState.Valid(tihabState) && tihabState.faction != faction)
					{
						goto IL_020E;
					}
					IEnumerable<TISpaceFleetState> enumerable = defenders;
					Func<TISpaceFleetState, bool> func;
					if ((func = <>9__3) == null)
					{
						func = (<>9__3 = (TISpaceFleetState x) => TIGameState.Valid(x) && x.faction != faction);
					}
					if (enumerable.Any<TISpaceFleetState>(func))
					{
						goto IL_020E;
					}
					IL_0271:
					yield return null;
					continue;
					IL_020E:
					if (AIDailyFactionPlanner.DetermineSTOFighterPlan(faction, defenders, tihabState, false, true).Values.Sum<PlannedFighters>((PlannedFighters x) => x.count) > 0)
					{
						new LaunchSTOInterceptorsOperation().OnOperationConfirm(faction, factionGoal_AttackWithFleet.target(), null, null);
						break;
					}
					goto IL_0271;
				}
			}
			List<FactionGoal_AttackWithFleet>.Enumerator enumerator = default(List<FactionGoal_AttackWithFleet>.Enumerator);
			this.fleetOperationsLive = false;
			yield break;
			yield break;
		}

		// Token: 0x06005A58 RID: 23128 RVA: 0x002AF5BA File Offset: 0x002AD7BA
		public static void DesignShips(TIFactionState faction, Action Callback = null)
		{
			CoroutineDummy.Singleton.StartCoroutine(AIDailyFactionPlanner.DesignShipsCoroutine(faction, Callback));
		}

		// Token: 0x06005A59 RID: 23129 RVA: 0x002AF5CE File Offset: 0x002AD7CE
		public static IEnumerator DesignShipsCoroutine(TIFactionState faction, Action Callback)
		{
			AIDailyFactionPlanner.<>c__DisplayClass89_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass89_0();
			CS$<>8__locals1.faction = faction;
			if (AIDailyFactionPlanner.isDesigningShips.ContainsKey(CS$<>8__locals1.faction) && AIDailyFactionPlanner.isDesigningShips[CS$<>8__locals1.faction])
			{
				yield break;
			}
			AIDailyFactionPlanner.isDesigningShips[CS$<>8__locals1.faction] = true;
			List<TISpaceShipTemplate> designsToRemove = new List<TISpaceShipTemplate>();
			float desiredStrategicRange_AU = CS$<>8__locals1.faction.DesiredStrategicRange_AU();
			ShipRole[] array = Enums.ActiveShipRoles;
			for (int j = 0; j < array.Length; j++)
			{
				AIDailyFactionPlanner.<>c__DisplayClass89_1 CS$<>8__locals2 = new AIDailyFactionPlanner.<>c__DisplayClass89_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.role = array[j];
				AIDailyFactionPlanner.<>c__DisplayClass89_2 CS$<>8__locals3 = new AIDailyFactionPlanner.<>c__DisplayClass89_2();
				CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
				CS$<>8__locals3.colonyRole = CS$<>8__locals3.CS$<>8__locals2.role == ShipRole.InnerSystemColonyShip || CS$<>8__locals3.CS$<>8__locals2.role == ShipRole.OuterSystemColonyShip;
				List<List<SpecialModuleRule>> forcedModuleRules = new List<List<SpecialModuleRule>>();
				if ((CS$<>8__locals3.CS$<>8__locals2.role != ShipRole.ArmyCarrier && CS$<>8__locals3.CS$<>8__locals2.role != ShipRole.EarthSurveillance) || !CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.IsActiveHumanFaction)
				{
					int num;
					for (int i = 0; i < 8; i = num + 1)
					{
						AIDailyFactionPlanner.<>c__DisplayClass89_3 CS$<>8__locals4 = new AIDailyFactionPlanner.<>c__DisplayClass89_3();
						CS$<>8__locals4.CS$<>8__locals3 = CS$<>8__locals3;
						CS$<>8__locals4.candidateShipDesign = new TISpaceShipTemplate(new StringBuilder(CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.templateName).Append("ShipDesign").Append(CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.shipDesignCount).ToString());
						bool flag = false;
						TIShipHullTemplate tishipHullTemplate = null;
						if (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.IsAlienFaction)
						{
							switch (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.role)
							{
							case ShipRole.TroopCarrier:
								flag = true;
								break;
							case ShipRole.ArmyCarrier:
								if (i == 6)
								{
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienAssaultCarrier", false);
								}
								else
								{
									flag = true;
								}
								break;
							case ShipRole.Explorer:
								flag = true;
								break;
							case ShipRole.InnerSystemColonyShip:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienFrigate", false);
									goto IL_09C0;
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCruiser", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMothership", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.OuterSystemColonyShip:
								num = i;
								if (num == 0 || num == 2)
								{
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienFrigate", false);
								}
								else
								{
									flag = true;
								}
								break;
							case ShipRole.EarthSurveillance:
								switch (i)
								{
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCruiser", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.CouncilorTransport:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienGunship", false);
									break;
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									break;
								default:
									flag = true;
									break;
								}
								break;
							case ShipRole.LS_Penetrator:
								switch (i)
								{
								case 0:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienGunship", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCorvette", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattlecruiser", false);
									goto IL_09C0;
								case 4:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDreadnought", false);
									goto IL_09C0;
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienLancer", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.LM_Protector:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienEscort", false);
									goto IL_09C0;
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMonitor", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattleship", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.LM_Interdictor:
								switch (i)
								{
								case 0:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCorvette", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienFrigate", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCruiser", false);
									goto IL_09C0;
								case 4:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDreadnought", false);
									goto IL_09C0;
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienTitan", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.LL_Intruder:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienEscort", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMonitor", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCruiser", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattleship", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.LL_Bomber:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCorvette", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattlecruiser", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienLancer", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.MS_Strike:
								switch (i)
								{
								case 0:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienGunship", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCorvette", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattlecruiser", false);
									goto IL_09C0;
								case 4:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienLancer", false);
									goto IL_09C0;
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDreadnought", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.MM_SpaceSuperiority:
								switch (i)
								{
								case 0:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCorvette", false);
									goto IL_09C0;
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienFrigate", false);
									goto IL_09C0;
								case 4:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDreadnought", false);
									goto IL_09C0;
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienTitan", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.ML_Standoff:
								switch (i)
								{
								case 0:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienEscort", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMonitor", false);
									goto IL_09C0;
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienFrigate", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCruiser", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattleship", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.SS_Interceptor:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienGunship", false);
									goto IL_09C0;
								case 1:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCruiser", false);
									goto IL_09C0;
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattleCruiser", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDreadnought", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.SM_Patrol:
								switch (i)
								{
								case 0:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienCorvette", false);
									goto IL_09C0;
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDestroyer", false);
									goto IL_09C0;
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienFrigate", false);
									goto IL_09C0;
								case 4:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienDreadnought", false);
									goto IL_09C0;
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienTitan", false);
									goto IL_09C0;
								case 7:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMothership", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							case ShipRole.SL_Defender:
								switch (i)
								{
								case 0:
								case 2:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienEscort", false);
									goto IL_09C0;
								case 1:
								case 3:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMonitor", false);
									goto IL_09C0;
								case 4:
								case 6:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienBattleship", false);
									goto IL_09C0;
								case 7:
									tishipHullTemplate = TemplateManager.Find<TIShipHullTemplate>("AlienMothership", false);
									goto IL_09C0;
								}
								flag = true;
								break;
							}
						}
						IL_09C0:
						if (!flag)
						{
							CS$<>8__locals4.antimatterBuild = !CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.IsAlienFaction && (i == 1 || i == 3 || i == 5 || i == 7) && !CS$<>8__locals4.CS$<>8__locals3.colonyRole;
							if (!CS$<>8__locals4.antimatterBuild || CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.UnlockedAntimatter)
							{
								CS$<>8__locals4.exoticsBuild = (i == 2 || i == 3 || i == 6 || i == 7) && (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.IsAlienFaction || !CS$<>8__locals4.CS$<>8__locals3.colonyRole);
								if (!CS$<>8__locals4.exoticsBuild || CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.UnlockedExotics)
								{
									TIOrbitState tiorbitState = null;
									CS$<>8__locals4.exampleDestination = null;
									float num2 = 540f;
									float num3 = (float)(CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.IsAlienFaction ? 78892310.0 : 78892310.0) / 86400f;
									if (!CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.IsAlienFaction & CS$<>8__locals4.CS$<>8__locals3.colonyRole)
									{
										if (i == 0)
										{
											if (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.role == ShipRole.InnerSystemColonyShip)
											{
												forcedModuleRules.Add(new List<SpecialModuleRule>
												{
													SpecialModuleRule.FoundSolarOutpost,
													SpecialModuleRule.FoundFusionOutpost,
													SpecialModuleRule.FoundFissionOutpost
												});
											}
											else
											{
												forcedModuleRules.Add(new List<SpecialModuleRule>
												{
													SpecialModuleRule.FoundFusionOutpost,
													SpecialModuleRule.FoundFissionOutpost,
													SpecialModuleRule.FoundSolarOutpost
												});
											}
										}
										else
										{
											if (i != 1)
											{
												goto IL_0F4A;
											}
											if (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.role == ShipRole.InnerSystemColonyShip)
											{
												forcedModuleRules.Add(new List<SpecialModuleRule>
												{
													SpecialModuleRule.FoundSolarPlatform,
													SpecialModuleRule.FoundFusionPlatform,
													SpecialModuleRule.FoundFissionPlatform
												});
											}
											else
											{
												forcedModuleRules.Add(new List<SpecialModuleRule>
												{
													SpecialModuleRule.FoundFusionPlatform,
													SpecialModuleRule.FoundFissionPlatform,
													SpecialModuleRule.FoundSolarPlatform
												});
											}
										}
										AIDailyFactionPlanner.<>c__DisplayClass89_3 CS$<>8__locals5 = CS$<>8__locals4;
										IEnumerable<TISpaceBodyState> enumerable = from x in CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.GoalsOfType(TIFactionGoalState.FoundHabGoals, false, true)
											select x.target().ref_spaceBody ?? x.target().ref_system;
										Func<TISpaceBodyState, double> func;
										if ((func = CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.<>9__4) == null)
										{
											func = (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.<>9__4 = (TISpaceBodyState x) => x.semiMajorAxis_AU * (double)((CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.role == ShipRole.InnerSystemColonyShip) ? (-1) : 1));
										}
										TISpaceBodyState tispaceBodyState = enumerable.MaxBy<TISpaceBodyState, double>(func);
										CS$<>8__locals5.exampleDestination = ((tispaceBodyState != null) ? tispaceBodyState.interfaceOrbits.FirstOrDefault<TIOrbitState>() : null);
										if (CS$<>8__locals4.exampleDestination != null)
										{
											tiorbitState = CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.nShipyardQueues.Select<KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>>, TIOrbitState>((KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> x) => x.Key.ref_system.orbits.OrderBy<TIOrbitState, bool>((TIOrbitState x) => !x.interfaceOrbit).First<TIOrbitState>()).Distinct<TIOrbitState>().MinBy<TIOrbitState, float>((TIOrbitState x) => TISpaceObjectState.GenericTransferTime_d(CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction, x, CS$<>8__locals4.exampleDestination));
										}
									}
									if (CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.DesignShip(false, CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.role, out CS$<>8__locals4.candidateShipDesign, desiredStrategicRange_AU, CS$<>8__locals4.exoticsBuild, CS$<>8__locals4.antimatterBuild, tishipHullTemplate, forcedModuleRules, i >= 4, tiorbitState, CS$<>8__locals4.exampleDestination, num2, num3) == TIFactionState.ShipDesignerOutcome.Success)
									{
										bool flag2 = false;
										List<TISpaceShipTemplate> shipDesigns = CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.shipDesigns;
										List<TISpaceShipTemplate> list = ((shipDesigns != null) ? shipDesigns.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.role && x.size == CS$<>8__locals4.candidateShipDesign.size && CS$<>8__locals4.exoticsBuild == x.requiresExotics && CS$<>8__locals4.antimatterBuild == x.requiresAntimatter && (!CS$<>8__locals4.CS$<>8__locals3.colonyRole || CS$<>8__locals4.candidateShipDesign.HasFoundBaseCapability() == x.HasFoundBaseCapability()) && (!CS$<>8__locals4.CS$<>8__locals3.colonyRole || CS$<>8__locals4.candidateShipDesign.HasFoundStationCapability() == x.HasFoundStationCapability()) && !x.Obsolete(CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction)).ToList<TISpaceShipTemplate>() : null);
										if (list.Count == 0)
										{
											flag2 = true;
										}
										else if (CS$<>8__locals4.candidateShipDesign.combatant)
										{
											if (CS$<>8__locals4.candidateShipDesign.TemplateSpaceCombatValue(false, -1f, 1f, false) > list.Max<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.TemplateSpaceCombatValue(false, -1f, 1f, false)))
											{
												flag2 = true;
											}
										}
										else if (CS$<>8__locals4.candidateShipDesign.role == ShipRole.TroopCarrier)
										{
											float num4 = CS$<>8__locals4.candidateShipDesign.AssaultCombatValue(false);
											float num5 = list.Max<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.AssaultCombatValue(false));
											if (num4 <= num5)
											{
												if (num4 != num5)
												{
													goto IL_0EF0;
												}
												if (CS$<>8__locals4.candidateShipDesign.baseCruiseDeltaV_kps(false) <= list.Max<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false)))
												{
													goto IL_0EF0;
												}
											}
											flag2 = true;
										}
										else if (CS$<>8__locals4.candidateShipDesign.baseCruiseDeltaV_kps(false) > list.Max<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.baseCruiseDeltaV_kps(false)))
										{
											flag2 = true;
										}
										IL_0EF0:
										if (flag2)
										{
											CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.playerControl.StartAction(new SaveShipDesignAction(CS$<>8__locals4.CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction, CS$<>8__locals4.candidateShipDesign));
										}
									}
									yield return null;
								}
							}
						}
						IL_0F4A:
						num = i;
					}
					using (IEnumerator enumerator = Enum.GetValues(typeof(ShipSize)).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ShipSize size = (ShipSize)enumerator.Current;
							List<Func<TISpaceShipTemplate, bool>> list2 = new List<Func<TISpaceShipTemplate, bool>>();
							if (CS$<>8__locals3.colonyRole)
							{
								list2.Add((TISpaceShipTemplate x) => x.HasFoundBaseCapability() && !x.HasFoundStationCapability());
								list2.Add((TISpaceShipTemplate x) => !x.HasFoundBaseCapability() && x.HasFoundStationCapability());
								list2.Add((TISpaceShipTemplate x) => x.HasFoundBaseCapability() && x.HasFoundStationCapability());
							}
							else
							{
								list2.Add((TISpaceShipTemplate x) => !x.requiresExotics && !x.requiresAntimatter);
								list2.Add((TISpaceShipTemplate x) => !x.requiresExotics && x.requiresAntimatter);
								list2.Add((TISpaceShipTemplate x) => x.requiresExotics && !x.requiresAntimatter);
								list2.Add((TISpaceShipTemplate x) => x.requiresExotics && x.requiresAntimatter);
							}
							using (List<Func<TISpaceShipTemplate, bool>>.Enumerator enumerator2 = list2.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									Func<TISpaceShipTemplate, bool> category = enumerator2.Current;
									AIDailyFactionPlanner.<>c__DisplayClass89_6 CS$<>8__locals8 = new AIDailyFactionPlanner.<>c__DisplayClass89_6();
									List<TISpaceShipTemplate> shipDesigns2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction.shipDesigns;
									IEnumerable<TISpaceShipTemplate> enumerable2 = ((shipDesigns2 != null) ? shipDesigns2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.role == CS$<>8__locals3.CS$<>8__locals2.role && x.size == size && category(x) && !x.Obsolete(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.faction)) : null);
									AIDailyFactionPlanner.<>c__DisplayClass89_6 CS$<>8__locals9 = CS$<>8__locals8;
									IEnumerable<TISpaceShipTemplate> enumerable3 = enumerable2;
									Func<TISpaceShipTemplate, float> func2;
									if ((func2 = CS$<>8__locals3.CS$<>8__locals2.<>9__21) == null)
									{
										func2 = (CS$<>8__locals3.CS$<>8__locals2.<>9__21 = delegate(TISpaceShipTemplate x)
										{
											if (!CS$<>8__locals3.CS$<>8__locals2.role.IsCombatantRole())
											{
												return x.baseCruiseDeltaV_kps(false);
											}
											return x.TemplateSpaceCombatValue(false, -1f, 1f, false);
										});
									}
									CS$<>8__locals9.bestShip = enumerable3.MaxBy<TISpaceShipTemplate, float>(func2);
									enumerable2 = enumerable2.Where<TISpaceShipTemplate>((TISpaceShipTemplate x) => x != CS$<>8__locals8.bestShip);
									designsToRemove.AddRange(enumerable2);
								}
							}
						}
					}
					CS$<>8__locals3 = null;
					forcedModuleRules = null;
				}
			}
			array = null;
			designsToRemove.AddRange(CS$<>8__locals1.faction.obsoleteShipDesigns.Select<string, TISpaceShipTemplate>((string x) => TemplateManager.Find<TISpaceShipTemplate>(x, false)));
			designsToRemove.AddRange(CS$<>8__locals1.faction.shipRefitDesignNames.Select<string, TISpaceShipTemplate>((string x) => TemplateManager.Find<TISpaceShipTemplate>(x, false)));
			if (designsToRemove.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x == null))
			{
				Log.Error("designsToRemove contained a null. Please upload logs and last 3 autosaves to issue 2246 on Github.", Array.Empty<object>());
				designsToRemove.RemoveAll((TISpaceShipTemplate x) => x == null);
				Dictionary<string, TISpaceShipTemplate> dictionary = CS$<>8__locals1.faction.obsoleteShipDesigns.ToDictionary<string, string, TISpaceShipTemplate>((string x) => x, (string x) => TemplateManager.Find<TISpaceShipTemplate>(x, false));
				if (dictionary.Values.Any<TISpaceShipTemplate>((TISpaceShipTemplate x) => x == null))
				{
					using (Dictionary<string, TISpaceShipTemplate>.KeyCollection.Enumerator enumerator3 = dictionary.Keys.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							string obsoleteName = enumerator3.Current;
							if (dictionary[obsoleteName] == null)
							{
								TISpaceShipTemplate tispaceShipTemplate = CS$<>8__locals1.faction.shipDesigns.FirstOrDefault<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.dataName == obsoleteName);
								if (tispaceShipTemplate != null)
								{
									Log.Error("Saving obsolete ship template " + obsoleteName + " to TemplateManager...", Array.Empty<object>());
									TemplateManager.Add<TISpaceShipTemplate>(tispaceShipTemplate, false);
									designsToRemove.Add(tispaceShipTemplate);
								}
								else
								{
									Log.Error("Obsolete ship template " + obsoleteName + " was missing from both the template manager as well as TIFactionState.shipDesigns. Deleting from TIFactionState.obsoleteShipDesigns", Array.Empty<object>());
									CS$<>8__locals1.faction.obsoleteShipDesigns.Remove(obsoleteName);
								}
							}
						}
					}
				}
			}
			foreach (TISpaceShipTemplate tispaceShipTemplate2 in designsToRemove)
			{
				if (tispaceShipTemplate2.CanDeleteDesign)
				{
					CS$<>8__locals1.faction.playerControl.StartAction(new DeleteShipDesignAction(CS$<>8__locals1.faction, tispaceShipTemplate2));
				}
				else if (!CS$<>8__locals1.faction.obsoleteShipDesigns.Contains(tispaceShipTemplate2.dataName))
				{
					CS$<>8__locals1.faction.playerControl.StartAction(new ToggleObsoleteShipDesignAction(CS$<>8__locals1.faction, tispaceShipTemplate2.dataName));
					foreach (KeyValuePair<TIHabModuleState, List<ShipConstructionQueueItem>> keyValuePair in CS$<>8__locals1.faction.nShipyardQueues)
					{
						int num6 = 0;
						TIHabModuleState key = keyValuePair.Key;
						foreach (ShipConstructionQueueItem shipConstructionQueueItem in CS$<>8__locals1.faction.nShipyardQueues[key].ToList<ShipConstructionQueueItem>())
						{
							if (!shipConstructionQueueItem.costPaid && !shipConstructionQueueItem.isRefit && CS$<>8__locals1.faction.obsoleteShipDesigns.Contains(shipConstructionQueueItem.shipDesignTemplateName))
							{
								CS$<>8__locals1.faction.playerControl.StartAction(new RemoveShipFromShipyardQueueAction(key, shipConstructionQueueItem));
							}
							num6++;
						}
					}
				}
			}
			if (!CS$<>8__locals1.faction.IsAlienFaction)
			{
				foreach (TISpaceShipTemplate tispaceShipTemplate3 in CS$<>8__locals1.faction.shipDesigns.ToList<TISpaceShipTemplate>())
				{
					TISpaceShipTemplate tispaceShipTemplate4 = CS$<>8__locals1.faction.DesignRefit(tispaceShipTemplate3);
					if (tispaceShipTemplate4 != null)
					{
						CS$<>8__locals1.faction.playerControl.StartAction(new SaveShipDesignAction(CS$<>8__locals1.faction, tispaceShipTemplate4));
						CS$<>8__locals1.faction.shipRefitDesignNames.Add(tispaceShipTemplate4.dataName);
					}
				}
			}
			CS$<>8__locals1.faction.updateShipDesignsFlag = false;
			if (Callback != null)
			{
				Callback();
			}
			AIDailyFactionPlanner.isDesigningShips[CS$<>8__locals1.faction] = false;
			yield break;
		}

		// Token: 0x06005A5A RID: 23130 RVA: 0x002AF5E4 File Offset: 0x002AD7E4
		public static Dictionary<TINationState, PlannedFighters> DetermineSTOFighterPlan(TIFactionState faction, IEnumerable<TISpaceFleetState> fleets, TIHabState hab, bool isReinforcement, bool assessmentOnly)
		{
			AIDailyFactionPlanner.<>c__DisplayClass90_0 CS$<>8__locals1 = new AIDailyFactionPlanner.<>c__DisplayClass90_0();
			CS$<>8__locals1.faction = faction;
			CS$<>8__locals1.fighterPlan = new Dictionary<TINationState, PlannedFighters>();
			IEnumerable<TISpaceAssetState> enumerable = from x in Enumerable.Empty<TISpaceAssetState>().Concat<TISpaceAssetState>(fleets).Append(hab)
				where x != null
				select x;
			TISpaceFleetState tispaceFleetState = fleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x != null && x.faction != CS$<>8__locals1.faction);
			TISpaceFleetState tispaceFleetState2 = fleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => x != null && x.faction == CS$<>8__locals1.faction);
			TIHabState tihabState = ((hab != null && !CS$<>8__locals1.faction.permanentAlly(hab.faction)) ? hab : null);
			TIFactionState tifactionState = ((tispaceFleetState != null) ? tispaceFleetState.faction : null) ?? ((tihabState != null) ? tihabState.faction : null);
			CS$<>8__locals1.percievedStrengthFactor = ((tifactionState != null) ? CS$<>8__locals1.faction.GetPerceivedEnemyFleetStrengthFactor(tifactionState) : 1f);
			float? num;
			if (tispaceFleetState == null)
			{
				num = null;
			}
			else
			{
				List<TISpaceShipState> ships = tispaceFleetState.ships;
				if (ships == null)
				{
					num = null;
				}
				else
				{
					num = new float?(ships.Where<TISpaceShipState>((TISpaceShipState x) => !x.hull.simpleHull).Sum<TISpaceShipState>((TISpaceShipState x) => CS$<>8__locals1.percievedStrengthFactor * x.SpaceCombatValue(false, 0f)));
				}
			}
			float? num2 = num;
			float num3 = num2.GetValueOrDefault();
			num3 += CS$<>8__locals1.percievedStrengthFactor * ((tihabState != null) ? tihabState.SpaceCombatValue() : 0f);
			if (tifactionState != null && tifactionState.EarthSTOFightersAvailable > 0)
			{
				float currentResourceAmount = tifactionState.GetCurrentResourceAmount(FactionResource.Boost);
				int num4 = Mathf.Min(tifactionState.EarthSTOFightersAvailable, (currentResourceAmount / AIEvaluators.GetTypicalSTOFighterBoostCost()).RoundDown());
				num3 += 0.6f * (float)num4 * AIEvaluators.GetTypicalSTOFighterSpaceCombatValue();
			}
			float num5 = enumerable.Where<TISpaceAssetState>((TISpaceAssetState x) => x.faction == CS$<>8__locals1.faction).Sum<TISpaceAssetState>((TISpaceAssetState x) => x.SpaceCombatValue());
			TIHabState tihabState2 = ((hab != null && CS$<>8__locals1.faction.permanentAlly(hab.faction)) ? hab : null);
			int earthSTOFightersAvailable = CS$<>8__locals1.faction.EarthSTOFightersAvailable;
			CS$<>8__locals1.availableFightersByNation = CS$<>8__locals1.faction.executiveNations.Where<TINationState>((TINationState x) => x.availableSTOFighters > 0).ToDictionary<TINationState, TINationState, int>((TINationState x) => x, (TINationState x) => x.availableSTOFighters);
			List<TIShipWeaponTemplate> list = CS$<>8__locals1.faction.AllowedFighterHullWeapons();
			float currentResourceAmount2 = CS$<>8__locals1.faction.GetCurrentResourceAmount(FactionResource.Boost);
			bool flag = assessmentOnly || tihabState2 != null || (tispaceFleetState2 != null && tispaceFleetState2.ships.Count > 0);
			TIGameState tigameState;
			if (tihabState != null)
			{
				tigameState = tihabState;
			}
			else
			{
				tigameState = tispaceFleetState;
			}
			float num6 = FactionGoal_AttackWithFleet.ComputeDesiredFleetCombatValueForAttack(CS$<>8__locals1.faction, tigameState, true, isReinforcement);
			float num7 = FactionGoal_AttackWithFleet.ComputeKillValue(CS$<>8__locals1.faction, tigameState);
			float num8 = Mathf.Clamp01(2f * num7);
			float num9 = Mathf.Lerp(0f, Mathf.Max(currentResourceAmount2 - 100f, 0f), num8);
			float num10 = CS$<>8__locals1.faction.executiveNations.Sum<TINationState>((TINationState x) => x.ControlPointWeightsTotalToPriorityIP(PriorityType.Military_BuildSTOSquadron) / x.GetRequiredInvestmentPointsForPriority(PriorityType.Military_BuildSTOSquadron)) * 365.2422f;
			num10 = Mathf.Max(num10, (float)earthSTOFightersAvailable / 8f);
			float num11 = 0.65f;
			int num12 = (num10 * (20f * num7) / num11).Round();
			float num13 = 0f;
			IEnumerable<TINationState> keys = CS$<>8__locals1.availableFightersByNation.Keys;
			Func<TINationState, int> func;
			if ((func = CS$<>8__locals1.<>9__13) == null)
			{
				func = (CS$<>8__locals1.<>9__13 = (TINationState x) => CS$<>8__locals1.availableFightersByNation[x]);
			}
			foreach (TINationState tinationState in keys.OrderByDescending<TINationState, int>(func))
			{
				if (num9 <= 0f)
				{
					break;
				}
				if (CS$<>8__locals1.<DetermineSTOFighterPlan>g__GetPlannedFighterCount|11() >= num12)
				{
					break;
				}
				CS$<>8__locals1.fighterPlan.Add(tinationState, new PlannedFighters());
				CS$<>8__locals1.fighterPlan[tinationState].SetDesign(CS$<>8__locals1.faction.DesignSTOFighter(tinationState, list.MaxBy<TIShipWeaponTemplate, float>((TIShipWeaponTemplate x) => x.GenericScore())));
				int num14 = Mathf.Min(new int[]
				{
					(num9 / CS$<>8__locals1.fighterPlan[tinationState].singleFighterBoostCost).RoundDown(),
					CS$<>8__locals1.availableFightersByNation[tinationState],
					num12 - CS$<>8__locals1.<DetermineSTOFighterPlan>g__GetPlannedFighterCount|11()
				});
				CS$<>8__locals1.fighterPlan[tinationState].SetCount(num14);
				num13 += (float)num14 * CS$<>8__locals1.fighterPlan[tinationState].fighter.TemplateSpaceCombatValue(false, -1f, 1f, true);
				num9 -= CS$<>8__locals1.fighterPlan[tinationState].boostCost;
			}
			float num15 = 0.2f * (float)CS$<>8__locals1.<DetermineSTOFighterPlan>g__GetPlannedFighterCount|11() / num10 * num11 / num7 / 100f;
			if (((num5 + num13) / num3 / (num5 / num3) - 1f < num15 || num5 + num13 < num3 * 0.85f || (num3 > 0f && num5 > num3 * 3f && !AIEvaluators.Abundant(CS$<>8__locals1.faction, FactionResource.Boost, 1f)) || (!isReinforcement && num13 < num6)) && flag)
			{
				CS$<>8__locals1.fighterPlan.Clear();
			}
			return CS$<>8__locals1.fighterPlan;
		}

		// Token: 0x06005A5B RID: 23131 RVA: 0x002AFB9C File Offset: 0x002ADD9C
		public static void AIReaction(AIReactionEvent reactionEvent, TIGameState relevantState1 = null, TIGameState relevantState2 = null)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			switch (reactionEvent)
			{
			case AIReactionEvent.HostileFleetArrivesAroundNaturalSpaceObject:
			{
				TINaturalSpaceObjectState naturalSpaceObject = relevantState1.ref_naturalSpaceObject;
				TILagrangePointState ref_lagrangePoint = naturalSpaceObject.ref_lagrangePoint;
				TISpaceObjectState getSunOrbitingRelatedObject = naturalSpaceObject.GetSunOrbitingRelatedObject;
				TISpaceBodyState tispaceBodyState = ((getSunOrbitingRelatedObject != null) ? getSunOrbitingRelatedObject.ref_spaceBody : null);
				TISpaceFleetState arrivingFleet = relevantState2.ref_fleet;
				using (IEnumerator<TIFactionState> enumerator = (from x in GameStateManager.AllFactions()
					where !x.isActivePlayer && !x.permanentAlly(arrivingFleet.faction)
					select x).GetEnumerator())
				{
					Func<TIFactionGoalState, bool> <>9__4;
					while (enumerator.MoveNext())
					{
						AIDailyFactionPlanner.<>c__DisplayClass91_2 CS$<>8__locals3 = new AIDailyFactionPlanner.<>c__DisplayClass91_2();
						CS$<>8__locals3.faction = enumerator.Current;
						HashSet<TISpaceFleetState> hashSet = new HashSet<TISpaceFleetState>();
						IEnumerable<TIFactionGoalState> enumerable = CS$<>8__locals3.faction.GoalsOfType(GoalType.DefendWithFleet, false, true);
						Func<TIFactionGoalState, bool> func;
						if ((func = <>9__4) == null)
						{
							func = (<>9__4 = (TIFactionGoalState x) => x.target() == naturalSpaceObject);
						}
						FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = enumerable.Where<TIFactionGoalState>(func).FirstOrDefault<TIFactionGoalState>() as FactionGoal_DefendWithFleet;
						if (factionGoal_DefendWithFleet != null && factionGoal_DefendWithFleet.assignedFleet != null && CS$<>8__locals3.<AIReaction>g__IsValidReactor|3(factionGoal_DefendWithFleet.assignedFleet))
						{
							hashSet.Add(factionGoal_DefendWithFleet.assignedFleet);
						}
						if (arrivingFleet.orbitState != null)
						{
							hashSet.UnionWith(arrivingFleet.orbitState.fleetsInOrbit.Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(CS$<>8__locals3.<AIReaction>g__IsValidReactor|3)));
							if (naturalSpaceObject.isSpaceBodyState && arrivingFleet.orbitState.interfaceOrbit)
							{
								hashSet.UnionWith(naturalSpaceObject.ref_spaceBody.habSites.SelectMany<TIHabSiteState, TISpaceFleetState>((TIHabSiteState x) => x.landedFleets).Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(CS$<>8__locals3.<AIReaction>g__IsValidReactor|3)));
							}
						}
						if (naturalSpaceObject.isEarth || naturalSpaceObject.orbits.Count <= 2 || hashSet.Count == 0)
						{
							hashSet.UnionWith(naturalSpaceObject.orbits.SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit).Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(CS$<>8__locals3.<AIReaction>g__IsValidReactor|3)));
						}
						if (hashSet.Count == 0 && tispaceBodyState != null)
						{
							hashSet.UnionWith(tispaceBodyState.orbits.SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit).Where<TISpaceFleetState>(new Func<TISpaceFleetState, bool>(CS$<>8__locals3.<AIReaction>g__IsValidReactor|3)));
						}
						foreach (TISpaceFleetState tispaceFleetState in hashSet)
						{
							AIDailyFactionPlanner.SingleFleetOperation(tispaceFleetState, true);
						}
					}
					return;
				}
				break;
			}
			case AIReactionEvent.HostileFleetBeginsBombardmentofMyAsset:
				break;
			case AIReactionEvent.MyCouncilorKilled:
				goto IL_0473;
			case AIReactionEvent.NewArmyGained:
			{
				TIArmyState ref_army = relevantState1.ref_army;
				TIFactionState ref_faction = relevantState1.ref_faction;
				if (ref_faction == null || ref_faction.isActivePlayer)
				{
					return;
				}
				if (ref_army.CurrentOperations().Count > 0)
				{
					AIDailyFactionPlanner.LaunchOperation(ref_army, new CancelArmyOperation(), ref_army, null);
				}
				if (!ref_army.homeNation.wars.Contains(ref_army.currentNation))
				{
					return;
				}
				TIFactionState executiveFaction = ref_army.currentNation.executiveFaction;
				if (executiveFaction == null || !executiveFaction.permanentAlly(ref_faction))
				{
					return;
				}
				TIRegionState armyDestination = TIArmyState.GetArmyDestination(ref_army, AIArmyDestination.NearestSafeRegion, 4);
				if (armyDestination != null)
				{
					AIDailyFactionPlanner.LaunchDeployArmyOperation(ref_army, armyDestination);
					return;
				}
				return;
			}
			case AIReactionEvent.ColonizationTechCompleted:
			{
				TIFactionState ref_faction2 = relevantState1.ref_faction;
				if (ref_faction2 != null && !ref_faction2.isActivePlayer && TIGlobalValuesState.GlobalValues.difficulty > 1)
				{
					ref_faction2.updateHabPlanningFlag = true;
					HabPlanner.HumanHabPlanner.ManageHabGoals(ref_faction2);
					AIDailyFactionPlanner.ProspectSites(ref_faction2);
					return;
				}
				return;
			}
			case AIReactionEvent.SpaceBodyProspected:
			{
				TIFactionState ref_faction3 = relevantState1.ref_faction;
				TISpaceBodyState ref_spaceBody = relevantState2.ref_spaceBody;
				if (ref_faction3 != null && !ref_faction3.isActivePlayer && ref_faction3.IsActiveHumanFaction && (ref_spaceBody.habSites.Length > 1 || ref_spaceBody.GetSunOrbitingRelatedObject.semiMajorAxis_AU <= 2.0))
				{
					ref_faction3.updateHabPlanningFlag = true;
					HabPlanner.HumanHabPlanner.ManageHabGoals(ref_faction3);
					HabPlanner.GetPlanner(ref_faction3).FoundHabs(ref_faction3);
					return;
				}
				return;
			}
			case AIReactionEvent.ArmyEntersMyRegion:
			{
				TIRegionState ref_region = relevantState1.ref_region;
				TINationState ref_nation = relevantState1.ref_nation;
				TIFactionState executiveFaction2 = ref_nation.executiveFaction;
				TIArmyState ref_army2 = relevantState2.ref_army;
				if ((executiveFaction2 == null || !executiveFaction2.isActivePlayer) && ref_region.isCapital && ref_nation.wars.Contains(ref_army2.homeNation))
				{
					AIDailyFactionPlanner.ConsiderNuclearAttack(ref_nation, null, false);
					return;
				}
				return;
			}
			case AIReactionEvent.MyHabCaptured:
			{
				TIHabState ref_hab = relevantState1.ref_hab;
				TIFactionState ref_faction4 = relevantState2.ref_faction;
				if (ref_hab != null && ref_faction4 != null && !ref_faction4.isActivePlayer && !ref_faction4.permanentAlly(ref_hab.faction))
				{
					TIFactionGoalState tifactionGoalState = ref_faction4.GoalsWithTarget(ref_hab, TIFactionGoalState.BuildHabGoals, true).FirstOrDefault<TIFactionGoalState>();
					GoalType goalType = ((tifactionGoalState != null) ? tifactionGoalState.GetGoalType() : GoalType.None);
					int num;
					if (ref_faction4.habs.Count <= 0)
					{
						num = 3;
					}
					else
					{
						num = ref_faction4.habs.Max<TIHabState>((TIHabState x) => x.tier);
					}
					int num2 = num - ref_hab.tier;
					ref_faction4.AddGoal(new FactionGoal_CaptureHab(ref_faction4, 20 - 3 * num2 - 2, ref_hab, goalType), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					return;
				}
				return;
			}
			case AIReactionEvent.NewHabDetected:
			{
				TIHabState ref_hab2 = relevantState1.ref_hab;
				TINaturalSpaceObjectState ref_naturalSpaceObject = ref_hab2.GetSunOrbitingRelatedObject.ref_naturalSpaceObject;
				if (ref_hab2.IsAlien())
				{
					using (IEnumerator<TIFactionState> enumerator = (from x in GameStateManager.AllHumanFactions()
						where !x.isActivePlayer
						select x).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							TIFactionState faction = enumerator.Current;
							if ((faction.AI_AtWarWithFaction(ref_hab2.ref_faction) || faction.veryAntiAlien) && (ref_naturalSpaceObject.isEarth || ref_naturalSpaceObject.habsInSystem.Count<TIHabState>((TIHabState x) => x.faction == faction) > 0))
							{
								int num3 = 5 + 3 * ref_hab2.tier + 4;
								if (ref_naturalSpaceObject.fleetsInSystem.Any<TISpaceFleetState>((TISpaceFleetState x) => x.AssaultCombatValue(false) > 0f))
								{
									faction.AddGoal(new FactionGoal_CaptureHab(faction, num3, ref_hab2, GoalType.None), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
								}
								else
								{
									faction.AddGoal(new FactionGoal_AttackWithFleet(faction, num3, ref_hab2, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
								}
							}
						}
						return;
					}
				}
				if (AIEvaluators.ShouldLaunchEmergencyAttackAgainstAsset(tifactionState, ref_hab2, false))
				{
					tifactionState.AddGoal(new FactionGoal_AttackWithFleet(tifactionState, 20, ref_hab2, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
					return;
				}
				return;
			}
			case AIReactionEvent.FleetStartedTransfer:
			{
				TISpaceFleetState ref_fleet = relevantState1.ref_fleet;
				if (AIEvaluators.ShouldLaunchEmergencyAttackAgainstAsset(tifactionState, ref_fleet, false))
				{
					tifactionState.AddGoal(new FactionGoal_AttackWithFleet(tifactionState, 20, ref_fleet, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
				}
				TISpaceBodyState jupiter = GameStateManager.Jupiter();
				if (ref_fleet.faction.IsAlienFaction)
				{
					return;
				}
				TISpaceGameState destination = ref_fleet.trajectory.destination;
				if (((destination != null) ? destination.ref_system : null) == jupiter && TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact() < TemplateManager.global.GetDifficultyBasedYearsToDelayAlienMiddleColonization() && !tifactionState.habs.Any<TIHabState>((TIHabState x) => x.ref_system == jupiter) && !tifactionState.GoalsOfType(GoalType.FoundBase, false, true).Any<TIFactionGoalState>((TIFactionGoalState x) => x.target().ref_system == jupiter))
				{
					AIDailyFactionPlanner.CreateAlienBaseFoundingGoal(jupiter, 19);
					return;
				}
				return;
			}
			case AIReactionEvent.PostCombatFleetRecovery:
			{
				TISpaceFleetState ref_fleet2 = relevantState1.ref_fleet;
				if (!ref_fleet2.faction.isActivePlayer)
				{
					for (int i = 0; i < 3; i++)
					{
						if (TIGameState.Valid(ref_fleet2) && ref_fleet2.currentOperations.Count == 0 && !ref_fleet2.unavailableForOperations)
						{
							AIDailyFactionPlanner.SingleFleetOperation(ref_fleet2, true);
						}
					}
					return;
				}
				return;
			}
			case AIReactionEvent.AlienCarrierDestroyed:
			{
				TIFactionState tifactionState2 = ((relevantState1 != null) ? relevantState1.ref_faction : null);
				if (tifactionState2 == null || tifactionState2.isActivePlayer)
				{
					return;
				}
				TIFactionState tifactionState3 = ((relevantState2 != null) ? relevantState2.ref_faction : null);
				if (tifactionState3 != null)
				{
					AIDailyFactionPlanner.<AIReaction>g__Retaliate|91_0(tifactionState2, tifactionState3);
					return;
				}
				return;
			}
			case AIReactionEvent.NewMissionPhase:
				return;
			case AIReactionEvent.BombardmentTargetEntersDangerZone:
			{
				TISpaceFleetState ref_fleet3 = relevantState1.ref_fleet;
				if (ref_fleet3.faction.isActivePlayer)
				{
					return;
				}
				TIFactionGoalState tifactionGoalState2 = ref_fleet3.faction.GoalsWithTarget(relevantState2, GoalType.AttackWithFleet, true).FirstOrDefault<TIFactionGoalState>();
				if (tifactionGoalState2 == null || tifactionGoalState2.importance <= 20)
				{
					ref_fleet3.CancelOperation(ref_fleet3.CurrentOperations().FirstOrDefault<OperationData>((OperationData x) => x.operation is BombardOperation));
					return;
				}
				return;
			}
			case AIReactionEvent.CheckForCPTrouble:
			{
				if (!(relevantState2 != null))
				{
					return;
				}
				TIFactionState ref_faction5 = relevantState2.ref_faction;
				if (!ref_faction5.isActivePlayer && ref_faction5.MajorCPTrouble())
				{
					AIDailyFactionPlanner.DisableOwnNations(ref_faction5, new Dictionary<TIControlPoint, float>());
					return;
				}
				return;
			}
			default:
				return;
			}
			TISpaceFleetState ref_fleet4 = relevantState1.ref_fleet;
			List<TIFactionState> ref_factions = relevantState2.ref_factions;
			TINaturalSpaceObjectState ref_naturalSpaceObject2 = ref_fleet4.ref_orbit.ref_naturalSpaceObject;
			List<TISpaceFleetState> list = new List<TISpaceFleetState>();
			if (ref_naturalSpaceObject2.isEarth)
			{
				list.AddRange(ref_naturalSpaceObject2.orbits.Where<TIOrbitState>((TIOrbitState x) => x.interfaceOrbit).SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit));
			}
			else if (ref_naturalSpaceObject2.orbits.Count <= 2)
			{
				list.AddRange(ref_naturalSpaceObject2.orbits.SelectMany<TIOrbitState, TISpaceFleetState>((TIOrbitState x) => x.fleetsInOrbit));
			}
			else
			{
				list.AddRange(ref_fleet4.ref_orbit.fleetsInOrbit);
			}
			if (ref_naturalSpaceObject2.isSpaceBodyState)
			{
				list.AddRange(ref_naturalSpaceObject2.ref_spaceBody.habSites.SelectMany<TIHabSiteState, TISpaceFleetState>((TIHabSiteState x) => x.landedFleets));
			}
			using (List<TISpaceFleetState>.Enumerator enumerator3 = list.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					TISpaceFleetState tispaceFleetState2 = enumerator3.Current;
					if (ref_factions.Contains(tispaceFleetState2.faction))
					{
						AIDailyFactionPlanner.SingleFleetOperation(tispaceFleetState2, true);
					}
				}
				return;
			}
			IL_0473:
			TIFactionState tifactionState4 = ((relevantState1 != null) ? relevantState1.ref_faction : null);
			if (tifactionState4 != null && !tifactionState4.isActivePlayer)
			{
				TIFactionState tifactionState5 = ((relevantState2 != null) ? relevantState2.ref_faction : null);
				List<TIMissionTemplate> list2 = tifactionState4.RequiredMissions(true);
				List<TIMissionTemplate> list3 = tifactionState4.MissingRequiredMissions(list2);
				bool currentlyDetectingHydra = tifactionState4.currentlyDetectingHydra;
				int count = tifactionState4.GoalsOfType(GoalType.WarOnFaction, false, true).Count;
				Dictionary<TICouncilorState, Dictionary<FactionResource, float>> dictionary = tifactionState4.councilors.ToDictionary<TICouncilorState, TICouncilorState, Dictionary<FactionResource, float>>((TICouncilorState x) => x, (TICouncilorState y) => TIFactionState.councilorResources.ToDictionary<FactionResource, FactionResource, float>((FactionResource z) => z, (FactionResource z) => y.GetMonthlyIncome(z)));
				bool flag = AIDailyFactionPlanner.AI_ControllingNeutralPowers(tifactionState4);
				AIDailyFactionPlanner.RecruitCouncilors(relevantState1.ref_faction, list2, list3, ref dictionary, currentlyDetectingHydra, count, flag);
				AIDailyFactionPlanner.TransferOrgsFromPool(tifactionState4, new Dictionary<TIOrgState, TICouncilorState>(), list2, list3, ref dictionary, currentlyDetectingHydra, count, flag);
				AIDailyFactionPlanner.<AIReaction>g__Retaliate|91_0(tifactionState4, tifactionState5);
				return;
			}
		}

		// Token: 0x06005A5E RID: 23134 RVA: 0x002B0894 File Offset: 0x002AEA94
		[CompilerGenerated]
		internal static float <AnalyzeDeficiencies_Human>g__FactionThreatScore_Old|27_0(TIFactionState viewingFaction, TIFactionState targetFaction)
		{
			return (float)targetFaction.controlPoints.Sum<TIControlPoint>((TIControlPoint x) => x.nation.numControlPoints_unclamped) * 1f + targetFaction.armies.Sum<TIArmyState>((TIArmyState y) => y.techLevel) * 0.5f + (float)targetFaction.activeHabModules.Sum<TIHabModuleState>((TIHabModuleState y) => y.tier) * 0.3f + (float)targetFaction.ships.Sum<TISpaceShipState>((TISpaceShipState y) => y.hull.structuralIntegrity) * 0.3f + (float)(viewingFaction.GetViewofFaction(targetFaction).GetObjectives(ObjectiveType.Campaign, ObjectiveStatus.Completed).Count * 10);
		}

		// Token: 0x06005A5F RID: 23135 RVA: 0x002B0984 File Offset: 0x002AEB84
		[CompilerGenerated]
		internal static int <ManageFleets>g__DefenseSort|42_92(TISpaceFleetState fleetToJoin)
		{
			FactionGoal_Fleet factionGoal_Fleet = fleetToJoin.AssignedGoal();
			if (factionGoal_Fleet != null && !factionGoal_Fleet.MayIncreaseFleetSize())
			{
				return -1;
			}
			if (factionGoal_Fleet.GetGoalType() == GoalType.SecureEarthSpace)
			{
				return 4;
			}
			if (factionGoal_Fleet != null)
			{
				FactionGoal_DefendWithFleet factionGoal_DefendWithFleet = factionGoal_Fleet as FactionGoal_DefendWithFleet;
				if (factionGoal_DefendWithFleet != null)
				{
					if (factionGoal_DefendWithFleet.EarmarkedFleetMC > 0)
					{
						return 3;
					}
					if (!factionGoal_Fleet.target().isSpaceBodyState)
					{
						return 1;
					}
					return 2;
				}
			}
			return 0;
		}

		// Token: 0x06005A60 RID: 23136 RVA: 0x002B09E8 File Offset: 0x002AEBE8
		[CompilerGenerated]
		internal static float <AliensCheckGoals>g__GetMiscSortValue|60_18(TISpaceAssetState spaceAsset)
		{
			float num = 0f;
			if (spaceAsset.isSpaceFleetState)
			{
				num = spaceAsset.SpaceCombatValue();
			}
			else if (spaceAsset.isHabState)
			{
				num = (float)spaceAsset.ref_hab.mass_kg;
			}
			return Mathf.Pow(num, 1.5f) * TIUtilities.RandomFloatValue();
		}

		// Token: 0x06005A61 RID: 23137 RVA: 0x002B0A34 File Offset: 0x002AEC34
		[CompilerGenerated]
		internal static bool <AdjustShipyardQueue>g__IsCriticalBuild|70_0(ShipConstructionQueueItem item_, ref AIDailyFactionPlanner.<>c__DisplayClass70_0 A_1)
		{
			return (A_1.faction.AISavingTarget.active && item_.AIFactionGoal == A_1.faction.AISavingTarget.relatedGoal) || (item_.AIFactionGoal != null && item_.AIFactionGoal.importance == 20) || (A_1.favorNoncombatants && item_.shipDesign.nonCombatant);
		}

		// Token: 0x06005A62 RID: 23138 RVA: 0x002B0AAC File Offset: 0x002AECAC
		[CompilerGenerated]
		internal static bool <SingleFleetOperation>g__MustStayPut|84_7(TISpaceFleetState queryFleet)
		{
			FactionGoal_Fleet factionGoal_Fleet = queryFleet.AssignedGoal();
			if (factionGoal_Fleet == null)
			{
				return false;
			}
			if (!(queryFleet.ref_naturalSpaceObject == null))
			{
				TIGameState ref_naturalSpaceObject = queryFleet.ref_naturalSpaceObject;
				TIGameState tigameState = factionGoal_Fleet.target();
				if (!(ref_naturalSpaceObject != ((tigameState != null) ? tigameState.ref_naturalSpaceObject : null)))
				{
					return factionGoal_Fleet.GetGoalType() == GoalType.DefendWithFleet || factionGoal_Fleet.GetGoalType() == GoalType.SecureEarthSpace || factionGoal_Fleet.GetGoalType() == GoalType.SurveilEarth;
				}
			}
			return false;
		}

		// Token: 0x06005A63 RID: 23139 RVA: 0x002B0B1C File Offset: 0x002AED1C
		[CompilerGenerated]
		internal static bool <PeriodicFleetOperations>g__IsPotentialSTOFighterAttackGoal|86_1(FactionGoal_AttackWithFleet attackGoal)
		{
			if (TIGameState.Valid(attackGoal.target()) && attackGoal.target().ref_orbit != null && attackGoal.target().ref_orbit.isEarthLEO)
			{
				TISpaceFleetState assignedFleet = attackGoal.assignedFleet;
				if ((assignedFleet == null || !assignedFleet.inTransfer) && attackGoal.target().isSpaceObjectState)
				{
					return attackGoal.target().isHabState || attackGoal.target().isSpaceFleetState;
				}
			}
			return false;
		}

		// Token: 0x06005A64 RID: 23140 RVA: 0x002B0B98 File Offset: 0x002AED98
		[CompilerGenerated]
		internal static void <AIReaction>g__Retaliate|91_0(TIFactionState faction, TIFactionState killingFaction)
		{
			List<TIGameState> list = new List<TIGameState>();
			if (killingFaction != null && killingFaction != faction && !faction.AI_AtWarWithFaction(killingFaction))
			{
				int num = 0;
				int num2;
				if (faction.IsAlienFaction && killingFaction.factionAssassinations.TryGetValue(faction, out num2))
				{
					num += 1 + Mathf.Min(num2, 2);
				}
				IEnumerable<TIHabState> enumerable = killingFaction.habs.Where<TIHabState>((TIHabState x) => x.IsBase);
				AIEvaluators.HabCapturingLogic habCapturingLogic = AIEvaluators.HabCapturingLogic.LowEffortHighReward;
				TIHabState tihabState = AIEvaluators.SelectHabToCapture(faction, killingFaction, enumerable, habCapturingLogic, false);
				if (tihabState != null)
				{
					FactionGoal_CaptureHab factionGoal_CaptureHab = new FactionGoal_CaptureHab(faction, 19, tihabState, GoalType.None);
					factionGoal_CaptureHab = faction.AddGoal(factionGoal_CaptureHab, HandleDuplicateGoalRule.ResetImportanceIfHigher, null) as FactionGoal_CaptureHab;
					if (factionGoal_CaptureHab != null)
					{
						num = Mathf.Max(num - 1, 1);
					}
				}
				for (int i = 0; i < num; i++)
				{
					TIGameState tigameState = null;
					if (killingFaction.habs.Count == 0 || TIUtilities.RandomFloatValue() < 0.35f)
					{
						tigameState = AIEvaluators.SelectFleetToAttack(faction, killingFaction.fleets, -1f);
					}
					if (tigameState == null && (killingFaction.habs.Count == 0 || TIUtilities.RandomFloatValue() < 0.35f))
					{
						tigameState = AIEvaluators.SelectSpaceFacilityToAttack(faction, killingFaction);
					}
					if (tigameState == null)
					{
						tigameState = AIEvaluators.SelectHabToAttack(faction, killingFaction.habs);
					}
					if (tigameState != null)
					{
						list.Add(tigameState);
					}
				}
			}
			foreach (TIGameState tigameState2 in list)
			{
				faction.AddGoal(new FactionGoal_AttackWithFleet(faction, 18, tigameState2, false, null, false), HandleDuplicateGoalRule.ResetImportanceIfHigher, null);
			}
		}

		// Token: 0x04004120 RID: 16672
		private const int Every3DaysCap = 27;

		// Token: 0x04004121 RID: 16673
		private const int Every4DaysCap = 28;

		// Token: 0x04004122 RID: 16674
		private const int Every7DaysCap = 28;

		// Token: 0x04004123 RID: 16675
		private const int Every14DaysCap = 28;

		// Token: 0x04004124 RID: 16676
		private GameTimeManager gameTime;

		// Token: 0x04004125 RID: 16677
		private TIFactionState[] AIFactions;

		// Token: 0x04004126 RID: 16678
		private int numAIFactions;

		// Token: 0x04004127 RID: 16679
		private List<TIOrbitState> LEOs;

		// Token: 0x04004128 RID: 16680
		public static Dictionary<TIFactionState, StaticFactionAIData> factionAIData;

		// Token: 0x0400412A RID: 16682
		private static readonly Dictionary<SupraRegion, int> maxTargets = new Dictionary<SupraRegion, int>
		{
			{
				SupraRegion.Africa,
				1
			},
			{
				SupraRegion.CentralAsia,
				2
			},
			{
				SupraRegion.SouthAmerica,
				1
			},
			{
				SupraRegion.NorthAmerica,
				3
			},
			{
				SupraRegion.EastAsia,
				3
			},
			{
				SupraRegion.Europe,
				3
			},
			{
				SupraRegion.Oceania,
				1
			}
		};

		// Token: 0x0400412B RID: 16683
		public const int maxAcceptableTimeForSpaceBodyOperation_d = 720;

		// Token: 0x0400412C RID: 16684
		private bool BuildSpaceAssetsBusy;

		// Token: 0x0400412D RID: 16685
		private readonly List<PriorityType> orderedPrioritiesForSpares = new List<PriorityType>
		{
			PriorityType.Knowledge,
			PriorityType.Civilian_InitiateSpaceflightProgram,
			PriorityType.LaunchFacilities,
			PriorityType.MissionControl,
			PriorityType.Economy,
			PriorityType.Military,
			PriorityType.Funding,
			PriorityType.Military_BuildSpaceDefenses,
			PriorityType.Military_BuildArmy,
			PriorityType.Military_BuildSTOSquadron,
			PriorityType.Welfare,
			PriorityType.Military_BuildNavy,
			PriorityType.Environment,
			PriorityType.Unity,
			PriorityType.Spoils,
			PriorityType.Military_InitiateNuclearProgram,
			PriorityType.Military_BuildNuclearWeapons
		};

		// Token: 0x0400412E RID: 16686
		private readonly List<GoalType> buildUpMilitaryGoals = new List<GoalType>
		{
			GoalType.MilitarizeNation,
			GoalType.ExpandNation
		};

		// Token: 0x0400412F RID: 16687
		public const int NucConsiderationFrequency_days = 3;

		// Token: 0x04004130 RID: 16688
		public const int PeacetimeArmyConsiderationFrequence_days = 4;

		// Token: 0x04004131 RID: 16689
		public const int megaFaunaActionFrequency_days = 10;

		// Token: 0x04004132 RID: 16690
		private bool fleetOperationsLive;

		// Token: 0x04004133 RID: 16691
		private static Dictionary<TIFactionState, bool> isDesigningShips = new Dictionary<TIFactionState, bool>();

		// Token: 0x02001247 RID: 4679
		private struct CouncilorOrgValue
		{
			// Token: 0x040069EF RID: 27119
			public TICouncilorState councilor;

			// Token: 0x040069F0 RID: 27120
			public float score;
		}

		// Token: 0x02001248 RID: 4680
		private struct OrgCouncilorScore
		{
			// Token: 0x040069F1 RID: 27121
			public TIOrgState org;

			// Token: 0x040069F2 RID: 27122
			public TICouncilorState councilor;

			// Token: 0x040069F3 RID: 27123
			public float score;
		}

		// Token: 0x02001249 RID: 4681
		private struct AIRelationshipChangeKey
		{
			// Token: 0x040069F4 RID: 27124
			public TINationState nation;

			// Token: 0x040069F5 RID: 27125
			public TINationState targetNation;

			// Token: 0x040069F6 RID: 27126
			public RelationChange change;
		}

		// Token: 0x0200124A RID: 4682
		private struct AIRelationshipChange
		{
			// Token: 0x06008A81 RID: 35457 RVA: 0x00339005 File Offset: 0x00337205
			public AIRelationshipChange(TINationState nation, TINationState target, RelationChange change, float score, int goalImportance)
			{
				this.nation = nation;
				this.targetNation = target;
				this.change = change;
				this.score = score;
				this.goalImportance = goalImportance;
			}

			// Token: 0x06008A82 RID: 35458 RVA: 0x0033902C File Offset: 0x0033722C
			public AIDailyFactionPlanner.AIRelationshipChangeKey GetKey()
			{
				return new AIDailyFactionPlanner.AIRelationshipChangeKey
				{
					nation = this.nation,
					targetNation = this.targetNation,
					change = this.change
				};
			}

			// Token: 0x040069F7 RID: 27127
			public TINationState nation;

			// Token: 0x040069F8 RID: 27128
			public TINationState targetNation;

			// Token: 0x040069F9 RID: 27129
			public RelationChange change;

			// Token: 0x040069FA RID: 27130
			public float score;

			// Token: 0x040069FB RID: 27131
			public int goalImportance;
		}
	}
}
