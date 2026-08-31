using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Systems.Bootstrap
{
	// Token: 0x020009BA RID: 2490
	public class SolarSystemBootstrap : IInitializable
	{
		// Token: 0x06005DD5 RID: 24021 RVA: 0x002CA67D File Offset: 0x002C887D
		public void Initialize()
		{
			Application.targetFrameRate = TemplateManager.global.targetFrameRate;
			GameControl.control.loadStartTimeStamp = Time.realtimeSinceStartup;
			Log.Time("<color=#00cc00>LoadTime:</color> Solar System Bootstrap:", delegate
			{
				GameControl.control.StartCoroutine(this.LoadGame());
			}, true, true);
		}

		// Token: 0x06005DD6 RID: 24022 RVA: 0x002CA6B5 File Offset: 0x002C88B5
		private IEnumerator LoadGame()
		{
			GameStateManager.ClearAllGameStates();
			SolarSystemBootstrap.ClearStaticData();
			bool loadingSave = !string.IsNullOrEmpty(this.savefile);
			DiContainer container = SolarSystemInstaller.container;
			if (!GameControl.control.skirmishMode)
			{
				TemplateManager.ClearAllTemplates();
				container.Resolve<TemplateManager>().Initialize(Application.streamingAssetsPath + "/Templates");
			}
			foreach (ScriptBehaviourManager scriptBehaviourManager in World.Active.BehaviourManagers)
			{
				container.Inject(scriptBehaviourManager);
				ComponentSystemBase componentSystemBase = scriptBehaviourManager as ComponentSystemBase;
				if (componentSystemBase != null)
				{
					componentSystemBase.Enabled = false;
				}
			}
			yield return null;
			GameControl.control.UpdateLoading(10f);
			GameControl.control._assetLoader = this.assetLoader;
			GameControl.assetLoader.Initialize();
			GameControl.control.LoadLoadingIllustration();
			if (!loadingSave)
			{
				TIUtilities.InitRandom(478154);
			}
			List<TIGameState> list = new List<TIGameState>();
			if (loadingSave)
			{
				bool success = false;
				Log.Info("Loading gamestates from " + this.savefile, Array.Empty<object>());
				Log.Time("<color=#00cc00>LoadTime:</color> Load Gamestates from save", delegate
				{
					success = GameStateManager.LoadAllGameStates(this.savefile);
				}, true, true);
				if (GameStateManager.FindGameState<TIMetadataState>() == null)
				{
					GameStateManager.CreateNewGameState<TIMetadataState>();
				}
				if (success && (string.IsNullOrEmpty(GameStateManager.Time().masterMetaTemplateName) || string.IsNullOrEmpty(GameStateManager.Time().scenarioMetaTemplateName)))
				{
					Log.Info("Save repair to add scenario ID", Array.Empty<object>());
					GameStateManager.Time().SetMasterMetaTemplate("FullScenario", "ModernScenario");
				}
				GameControl.control.SetScenarioMetaTemplate(GameStateManager.Time().scenarioMetaTemplateName);
				TemplateManager.ResolveScenarioTemplates(GameControl.control.scenarioTemplate);
				if (!success)
				{
					goto IL_04B1;
				}
				List<string> list2 = (from x in GameStateManager.IterateByClass<TIRegionState>(false)
					select x.templateName).ToList<string>();
				using (List<TIDataTemplate>.Enumerator enumerator2 = TIMetaTemplate.GetTemplatesOfTypeFromMeta(GameStateManager.Time().scenarioMetaTemplateName, typeof(TIRegionTemplate)).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIDataTemplate tidataTemplate = enumerator2.Current;
						if (!list2.Contains(tidataTemplate.dataName))
						{
							Log.Info("Adding Region: " + tidataTemplate.dataName, Array.Empty<object>());
							TIGameState tigameState = tidataTemplate.CreateGameState();
							if (!Error.IsNull<TIGameState>(tigameState, "Failed to create GameState for {0} {1}", new object[]
							{
								typeof(TIRegionTemplate),
								tidataTemplate.dataName
							}))
							{
								tigameState.exists = true;
								tigameState.InitWithTemplate(tidataTemplate);
								list.Add(tigameState);
							}
						}
					}
					goto IL_04B1;
				}
			}
			if (this.scenario == null)
			{
				if (Application.isEditor)
				{
					this.scenario = new TestScenario();
					Log.Info("Loading " + this.scenario.GetType().Name, Array.Empty<object>());
				}
				else
				{
					Log.Error("No Scenario", Array.Empty<object>());
				}
			}
			else
			{
				Log.Info("Loading " + this.scenario.GetType().Name, Array.Empty<object>());
			}
			if (this.scenario == null)
			{
				throw new Exception("Cannot load , no save or scenario provided.");
			}
			if (GameStateManager.FindGameState<TIMetadataState>() == null)
			{
				GameStateManager.CreateNewGameState<TIMetadataState>();
			}
			string text = string.Empty;
			foreach (string text2 in this.scenario.scenarioTemplate.templateNames)
			{
				TIMetaTemplate timetaTemplate = TemplateManager.Find<TIMetaTemplate>(text2, false);
				if (timetaTemplate != null && timetaTemplate.newCampaignOptionCategory == "Scenario")
				{
					text = text2;
					break;
				}
			}
			GameControl.control.SetScenarioMetaTemplate(text);
			this.scenario.Initialize();
			GameStateManager.Time().SetMasterMetaTemplate(this.scenario.scenarioTemplateName, text);
			foreach (TIBilateralTemplate tibilateralTemplate in from x in TemplateManager.IterateByClass<TIBilateralTemplate>(true)
				where x.BilateralIsInScenario()
				select x)
			{
				tibilateralTemplate.CheckToCreateGameState();
			}
			IL_04B1:
			if (!GameStateManager.IsValid())
			{
				yield break;
			}
			TemplateManager.InitializeStaticManagers();
			GameControl.LoadGlobalGameStates();
			World.Active.GetExistingManager<GameTimeManager>().Initialize();
			foreach (TINationState tinationState in GameStateManager.IterateByClass<TINationState>(false).ToList<TINationState>())
			{
				if (!TIGameState.Valid(tinationState) || (!tinationState.extant && tinationState.template == null))
				{
					GameStateManager.RemoveGameState<TINationState>(tinationState.ID, false);
				}
			}
			foreach (TISpaceBodyState tispaceBodyState in GameStateManager.IterateByClass<TISpaceBodyState>(false))
			{
				tispaceBodyState.nations.RemoveAll((TINationState x) => !TIGameState.Valid(x));
			}
			foreach (TIControlPoint ticontrolPoint in GameStateManager.IterateByClass<TIControlPoint>(false).ToList<TIControlPoint>())
			{
				if (!TIGameState.Valid(ticontrolPoint.nation))
				{
					GameStateManager.RemoveGameState<TIControlPoint>(ticontrolPoint.ID, false);
				}
			}
			Log.Time("<color=#00cc00>LoadTime:</color> PostGlobalInit2", delegate
			{
				foreach (TIGameState tigameState3 in from x in GameStateManager.IterateByClass<TIGameState>(true)
					orderby x is TIFactionState descending, x is TIGlobalValuesState descending
					select x)
				{
					tigameState3.PostGlobalGameStateCreateInit_2();
				}
			}, true, true);
			this.playerManager.Initialize();
			if (loadingSave)
			{
				TIFactionState tifactionState = null;
				foreach (TIPlayerState tiplayerState in GameStateManager.IterateByClass<TIPlayerState>(false))
				{
					if (!tiplayerState.isAI)
					{
						tifactionState = tiplayerState.faction;
						break;
					}
				}
				GameControl.SetActivePlayer(tifactionState);
				using (List<TIGameState>.Enumerator enumerator9 = list.GetEnumerator())
				{
					while (enumerator9.MoveNext())
					{
						TIGameState tigameState2 = enumerator9.Current;
						(tigameState2 as TIRegionState).InitializePostCampaignCreation();
					}
					goto IL_06B8;
				}
			}
			GameControl.SetActivePlayer(GameStateManager.FindByTemplate<TIFactionState>(this.scenario.activePlayerFaction.dataName, false));
			IL_06B8:
			Log.Time("<color=#00cc00>LoadTime:</color> CalculateHabStats", delegate
			{
				TIHabSiteState.Statistics.Recalculate();
			}, true, true);
			GameControl.spaceCombat.ResetCombatManager();
			GameControl.spaceCombat.SetupEventListener();
			CanvasManager canvasManager = World.Active.GetExistingManager<CanvasManager>();
			GameControl.control._canvasStack = canvasManager;
			yield return null;
			GameControl.control.UpdateLoading(20f);
			canvasManager.Initialize();
			if (!GameControl.control.skirmishMode)
			{
				GameControl.control.LoadLoadingIllustration();
			}
			yield return null;
			GameControl.control.UpdateLoading(30f);
			Log.Time("<color=#00cc00>LoadTime:</color> PostCanvasInit3", delegate
			{
				foreach (TIGameState tigameState4 in GameStateManager.IterateByClass<TIGameState>(true))
				{
					tigameState4.PostCanvasManagerCreateInit_3();
				}
			}, true, true);
			Log.Time("<color=#00cc00>LoadTime:</color> Initialize Nations", delegate
			{
				this.InitializeNations();
			}, true, true);
			Log.Time("<color=#00cc00>LoadTime:</color> Initialize Factions", delegate
			{
				this.InitializeFactions();
			}, true, true);
			List<TIGameState> gameStates = GameStateManager.IterateByClass<TIGameState>(true).ToList<TIGameState>();
			Log.Time("<color=#00cc00>LoadTime:</color> PostInit4", delegate
			{
				foreach (TIGameState tigameState5 in gameStates)
				{
					tigameState5.PostInitializationInit_4();
				}
			}, true, true);
			gameStates = GameStateManager.IterateByClass<TIGameState>(true).ToList<TIGameState>();
			Log.Time("<color=#00cc00>LoadTime:</color> PostStartUpInit5", delegate
			{
				foreach (TIGameState tigameState6 in gameStates)
				{
					tigameState6.PostAllStartUpInit_5();
				}
			}, true, true);
			Log.Time("<color=#00cc00>LoadTime:</color> ViewControlInit", delegate
			{
				this.viewControl.Initialize();
			}, true, true);
			yield return null;
			GameControl.control.UpdateLoading(40f);
			if (!loadingSave && !GameControl.control.skirmishMode)
			{
				Log.Time("<color=#00cc00>LoadTime:</color> InitNewCampaignAI", delegate
				{
					AIDailyFactionPlanner.InitializeAIForNewCampaign();
				}, true, true);
			}
			GameControl.bootstrapFinished = true;
			GameControl.control.Initialize(loadingSave, this.scenario);
			Log.Time("<color=#00cc00>LoadTime:</color> BakeJourneyHeuristic", delegate
			{
				TIArmyState.BakeJourneyHeuristic();
			}, true, true);
			yield break;
		}

		// Token: 0x06005DD7 RID: 24023 RVA: 0x002CA6C4 File Offset: 0x002C88C4
		public static void ClearStaticData()
		{
			AIEvaluators.ClearStaticData();
			TIGlobalValuesState.ClearStaticData();
			TIHistoricalData.ClearStaticData();
			HabPlanner.ClearStaticData();
			TISpaceFleetState.ClearStaticData();
			TISpaceShipTemplate.ClearStaticData();
			TIHabModuleTemplate.ClearStaticData();
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x002CA6EC File Offset: 0x002C88EC
		private void InitializeFactions()
		{
			EntityManager existingManager = World.Active.GetExistingManager<EntityManager>();
			foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
			{
				Entity entity = existingManager.CreateEntity(new ComponentType[] { typeof(Faction) });
				existingManager.SetComponentData<Faction>(entity, new Faction
				{
					ID = tifactionState.ID
				});
				tifactionState.NewCampaign();
			}
		}

		// Token: 0x06005DD9 RID: 24025 RVA: 0x002CA784 File Offset: 0x002C8984
		private void InitializeNations()
		{
			EntityManager existingManager = World.Active.GetExistingManager<EntityManager>();
			foreach (TINationState tinationState in GameStateManager.IterateByClass<TINationState>(false))
			{
				Entity entity = existingManager.CreateEntity(new ComponentType[] { typeof(Nation) });
				existingManager.SetComponentData<Nation>(entity, new Nation
				{
					ID = tinationState.ID
				});
			}
			List<TIWarState> list = new List<TIWarState>();
			using (IEnumerator<TIWarState> enumerator2 = GameStateManager.GlobalValues().interstateWars.OrderByDescending<TIWarState, float>((TIWarState x) => x.attacker.militaryStrength + x.defender.militaryStrength).GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					TIWarState war = enumerator2.Current;
					bool flag = false;
					Func<TINationState, bool> <>9__1;
					Func<TINationState, bool> <>9__2;
					Func<TINationState, bool> <>9__3;
					Func<TINationState, bool> <>9__4;
					foreach (TIWarState tiwarState in list)
					{
						if (tiwarState.defendingAlliance.Contains(war.attacker))
						{
							goto IL_018A;
						}
						IEnumerable<TINationState> defendingAlliance = tiwarState.defendingAlliance;
						Func<TINationState, bool> func;
						if ((func = <>9__1) == null)
						{
							func = (<>9__1 = (TINationState x) => war.attacker.wars.Contains(x));
						}
						if (!defendingAlliance.All<TINationState>(func) || tiwarState.attackingAlliance.Intersect<TINationState>(war.attacker.wars).Any<TINationState>())
						{
							goto IL_018A;
						}
						IEnumerable<TINationState> attackingAlliance = tiwarState.attackingAlliance;
						Func<TINationState, bool> func2;
						if ((func2 = <>9__2) == null)
						{
							func2 = (<>9__2 = (TINationState x) => war.attackingAlliance.All<TINationState>((TINationState y) => y.allies.Contains(x)));
						}
						bool flag2 = attackingAlliance.All<TINationState>(func2);
						IL_018B:
						if (tiwarState.attackingAlliance.Contains(war.defender))
						{
							goto IL_024F;
						}
						IEnumerable<TINationState> attackingAlliance2 = tiwarState.attackingAlliance;
						Func<TINationState, bool> func3;
						if ((func3 = <>9__3) == null)
						{
							func3 = (<>9__3 = (TINationState x) => war.defender.wars.Contains(x));
						}
						if (!attackingAlliance2.All<TINationState>(func3) || tiwarState.defendingAlliance.Intersect<TINationState>(war.defender.wars).Any<TINationState>())
						{
							goto IL_024F;
						}
						IEnumerable<TINationState> defendingAlliance2 = tiwarState.defendingAlliance;
						Func<TINationState, bool> func4;
						if ((func4 = <>9__4) == null)
						{
							func4 = (<>9__4 = (TINationState x) => war.defendingAlliance.All<TINationState>((TINationState y) => y.allies.Contains(x)));
						}
						if (!defendingAlliance2.All<TINationState>(func4))
						{
							goto IL_024F;
						}
						bool flag3 = tiwarState.defendingAlliance.Intersect<TINationState>(war.defendingAlliance).Any<TINationState>();
						IL_0250:
						bool flag4 = flag3;
						if (flag2 && flag4)
						{
							if (!tiwarState.attackingAlliance.Contains(war.attacker))
							{
								tiwarState.JoinAttackers(war.attacker);
							}
							if (!tiwarState.defendingAlliance.Contains(war.defender))
							{
								tiwarState.JoinDefenders(war.defender);
							}
							flag = true;
							continue;
						}
						continue;
						IL_024F:
						flag3 = false;
						goto IL_0250;
						IL_018A:
						flag2 = false;
						goto IL_018B;
					}
					if (flag)
					{
						TIGlobalValuesState.GlobalValues.DeleteWar(war);
					}
					else
					{
						list.Add(war);
					}
				}
			}
			TIGlobalValuesState.AgglomerateAllWars();
		}

		// Token: 0x0400431B RID: 17179
		[global::Zenject.Inject]
		private PlayerManager playerManager;

		// Token: 0x0400431C RID: 17180
		[global::Zenject.Inject]
		private AssetLoader assetLoader;

		// Token: 0x0400431D RID: 17181
		[InjectOptional]
		private string savefile;

		// Token: 0x0400431E RID: 17182
		[InjectOptional]
		private IScenario scenario;

		// Token: 0x0400431F RID: 17183
		[global::Zenject.Inject]
		private ViewControl viewControl;
	}
}
