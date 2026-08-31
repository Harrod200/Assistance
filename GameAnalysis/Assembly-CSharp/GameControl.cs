using System;
using System.Collections;
using System.Linq;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Plugins;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using PavonisInteractive.TerraInvicta.Systems.UI;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Zenject;

// Token: 0x0200002B RID: 43
public class GameControl : MonoBehaviour
{
	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000111 RID: 273 RVA: 0x00008CD6 File Offset: 0x00006ED6
	// (set) Token: 0x06000112 RID: 274 RVA: 0x00008CDE File Offset: 0x00006EDE
	[global::Zenject.Inject]
	public ViewControl viewMgr { get; private set; }

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000113 RID: 275 RVA: 0x00008CE7 File Offset: 0x00006EE7
	public TIMetaTemplate scenarioTemplate
	{
		get
		{
			return this._scenarioMetaTemplate ?? global::UnityEngine.Object.FindObjectOfType<StartMenuController>().GetSelectedScenarioMetaTemplate();
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000114 RID: 276 RVA: 0x00008CFD File Offset: 0x00006EFD
	// (set) Token: 0x06000115 RID: 277 RVA: 0x00008D04 File Offset: 0x00006F04
	public static GameControl control { get; set; }

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000116 RID: 278 RVA: 0x00008D0C File Offset: 0x00006F0C
	public static CanvasManager canvasStack
	{
		get
		{
			return GameControl.control._canvasStack;
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000117 RID: 279 RVA: 0x00008D18 File Offset: 0x00006F18
	public static EventManager eventManager
	{
		get
		{
			return GameControl.control._eventManager;
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000118 RID: 280 RVA: 0x00008D24 File Offset: 0x00006F24
	public static AssetLoader assetLoader
	{
		get
		{
			return GameControl.control._assetLoader;
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000119 RID: 281 RVA: 0x00008D30 File Offset: 0x00006F30
	public static SolarSystemControl solarSystem
	{
		get
		{
			return GameControl.control._solarSystem;
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x0600011A RID: 282 RVA: 0x00008D3C File Offset: 0x00006F3C
	public static PlayerManager playerManager
	{
		get
		{
			return GameControl.control._playerManager;
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x0600011B RID: 283 RVA: 0x00008D48 File Offset: 0x00006F48
	public static SpaceCombatManager spaceCombat
	{
		get
		{
			return GameControl.control._spaceCombat;
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x0600011C RID: 284 RVA: 0x00008D54 File Offset: 0x00006F54
	public static NamelistManager namelists
	{
		get
		{
			return GameControl.control._namelists;
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600011D RID: 285 RVA: 0x00008D60 File Offset: 0x00006F60
	// (set) Token: 0x0600011E RID: 286 RVA: 0x00008D68 File Offset: 0x00006F68
	public TIFactionState activePlayer { get; private set; }

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600011F RID: 287 RVA: 0x00008D71 File Offset: 0x00006F71
	// (set) Token: 0x06000120 RID: 288 RVA: 0x00008D78 File Offset: 0x00006F78
	public static bool DLCValidated { get; private set; }

	// Token: 0x06000121 RID: 289 RVA: 0x00008D80 File Offset: 0x00006F80
	public static void LoadGlobalGameStates()
	{
		TIMissionPhaseState timissionPhaseState = GameStateManager.FindGameState<TIMissionPhaseState>();
		if (timissionPhaseState == null)
		{
			timissionPhaseState = GameStateManager.CreateNewGameState<TIMissionPhaseState>();
		}
		timissionPhaseState.PostGameStateCreateInit_OnCreationOnly_1();
		TINotificationQueueState tinotificationQueueState = GameStateManager.FindGameState<TINotificationQueueState>();
		if (tinotificationQueueState == null)
		{
			tinotificationQueueState = GameStateManager.CreateNewGameState<TINotificationQueueState>();
		}
		tinotificationQueueState.PostGameStateCreateInit_OnCreationOnly_1();
		TIEffectsState tieffectsState = GameStateManager.FindGameState<TIEffectsState>();
		if (tieffectsState == null)
		{
			tieffectsState = GameStateManager.CreateNewGameState<TIEffectsState>();
		}
		tieffectsState.PostGameStateCreateInit_OnCreationOnly_1();
		TIGlobalResearchState tiglobalResearchState = GameStateManager.FindGameState<TIGlobalResearchState>();
		if (tiglobalResearchState == null)
		{
			tiglobalResearchState = GameStateManager.CreateNewGameState<TIGlobalResearchState>();
		}
		tiglobalResearchState.PostGameStateCreateInit_OnCreationOnly_1();
		(GameStateManager.FindGameState<TIGlobalValuesState>() ?? GameStateManager.CreateNewGameState<TIGlobalValuesState>()).PostGameStateCreateInit_OnCreationOnly_1();
		TIPromptQueueState tipromptQueueState = GameStateManager.FindGameState<TIPromptQueueState>();
		if (tipromptQueueState == null)
		{
			tipromptQueueState = GameStateManager.CreateNewGameState<TIPromptQueueState>();
		}
		if (GameStateManager.FindGameState<TIHistoricalData>() == null)
		{
			GameStateManager.CreateNewGameState<TIHistoricalData>();
		}
		tipromptQueueState.PostGameStateCreateInit_OnCreationOnly_1();
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00008E38 File Offset: 0x00007038
	public static void SetActivePlayer(TIFactionState faction)
	{
		GameControl.control.activePlayer = faction;
		if (faction != null)
		{
			faction.InitializeAchievements();
		}
		Mood.UpdateActivePlayerState();
		foreach (TIPlayerState tiplayerState in GameStateManager.IterateByClass<TIPlayerState>(false))
		{
			tiplayerState.AssignAIStatus(GameControl.control.activePlayer != tiplayerState.faction);
		}
		foreach (TIFactionState tifactionState in GameStateManager.IterateByClass<TIFactionState>(false))
		{
			tifactionState.showAlerts = tifactionState == GameControl.control.activePlayer;
			tifactionState.showRegularNotifications = tifactionState == GameControl.control.activePlayer;
			tifactionState.showTimerNotifications = tifactionState == GameControl.control.activePlayer;
			tifactionState.showSummaryLogs = tifactionState == GameControl.control.activePlayer;
			tifactionState.checkNotificationOverrides = tifactionState == GameControl.control.activePlayer;
			tifactionState.defaultFleetArrivalAlert = ((tifactionState == GameControl.control.activePlayer) ? 1 : 0);
			tifactionState.defaultFleetArrivalAlert_Earth = 0;
			tifactionState.defaultFleetArrivalAlienModifier = ((tifactionState == GameControl.control.activePlayer) ? 1 : 0);
			tifactionState.defaultFleetArrivalAlienModifier_Earth = ((tifactionState == GameControl.control.activePlayer) ? 1 : 0);
		}
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00008FB4 File Offset: 0x000071B4
	public static void Stop()
	{
		if (Application.isEditor)
		{
			Debug.Break();
			return;
		}
		Application.Quit();
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00008FC8 File Offset: 0x000071C8
	public static void StartSimulationAction(SimulationAction action)
	{
		action.Execute();
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00008FD0 File Offset: 0x000071D0
	public void SetScenarioMetaTemplate(string metaTemplate)
	{
		this._scenarioMetaTemplate = TemplateManager.Find<TIMetaTemplate>(metaTemplate, false);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00008FE0 File Offset: 0x000071E0
	public void Initialize(bool loadingSave, IScenario scenario)
	{
		this.loadScreenWidget = global::UnityEngine.Object.FindObjectOfType<LoadScreenWidget>();
		this.loadScreenWidget.InitLoadWidget();
		Log.Info("Execute Game Control Initialize Cycle", Array.Empty<object>());
		World.Active.GetExistingManager<CameraManager>().SetSkybox(TIPlayerProfileManager.skyboxVariant);
		this.mainCamera = Camera.main;
		this.mainCameraTransform = this.mainCamera.transform;
		VisualizerLoader visualizerLoader = base.gameObject.AddComponent<VisualizerLoader>();
		visualizerLoader.enabled = true;
		Log.Time("<color=#00cc00>LoadTime:</color> Init All Visualizers and Warm Shaders (Bad data in public build due to coroutine) ", delegate
		{
			visualizerLoader.Initialize(loadingSave, scenario);
		}, true, true);
		AICouncilorMissionPlanner.singleton.Initialize();
		if (!GameControl.control.skirmishMode)
		{
			Log.Time("<color=#00cc00>LoadTime:</color> Initialize Full Tech Tree", delegate
			{
				global::UnityEngine.Object.FindObjectOfType<ResearchScreenController>(true).InitializeFullTechTree(false, "", false, true);
			}, true, true);
		}
		GameControl.control.UpdateLoading(50f);
		TIInputManager.acceptingInput = true;
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000090E1 File Offset: 0x000072E1
	public static void CreateAndDestroyStartupGameStates()
	{
		Log.Time("<color=#00cc00>LoadTime:</color> PostEverything Save Repair", delegate
		{
			foreach (TIGameState tigameState in GameStateManager.IterateByClass<TIGameState>(true).ToList<TIGameState>())
			{
				tigameState.PostEverythingSaveRepair_8();
			}
		}, true, true);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00009110 File Offset: 0x00007310
	public void CompleteInit(bool loadingSave, IScenario scenario)
	{
		Log.Info("Visualizer Loader Complete: Complete Init", Array.Empty<object>());
		if (!this.skirmishMode)
		{
			this.viewMgr.GotoView(ViewType.SolarSystem);
			if (!loadingSave)
			{
				global::UnityEngine.Object.FindObjectOfType<NotificationScreenController>().LaunchIntroCinematic();
			}
		}
		GameControl.initialized = true;
		GameControl.eventManager.TriggerEvent(new StartupComplete(scenario), null, Array.Empty<object>());
		if (!GameControl.control.skirmishMode && !loadingSave && GameStateManager.CampaignHasAlienFaction())
		{
			TIUtilities.GotoGameState(GameStateManager.IterateByClass<TIRegionUFOCrashdownState>(false).FirstOrDefault<TIRegionUFOCrashdownState>((TIRegionUFOCrashdownState x) => x.crashdownPresent), true, true, true, true, false, 3f);
		}
		global::UnityEngine.Object.Destroy(base.gameObject.GetComponent<VisualizerLoader>());
		PostProcessLayer component = Camera.main.GetComponent<PostProcessLayer>();
		if (TIPlayerProfileManager.antiAliasingMode != 0 && TIPlayerProfileManager.antiAliasingMode == 1)
		{
			component.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
		}
		GameControl.frameFinishedLoading = TIFrameCounter.FrameCount;
		GameControl.control.LogTotalLoadTime();
	}

	// Token: 0x06000129 RID: 297 RVA: 0x000091FD File Offset: 0x000073FD
	public static IEnumerator PassErrorToStartScreen(string header, string desc)
	{
		int frames = 300;
		yield return null;
		int num;
		for (int i = 0; i < frames; i = num + 1)
		{
			StartMenuController startMenuController = global::UnityEngine.Object.FindObjectOfType<StartMenuController>();
			if (startMenuController != null)
			{
				startMenuController.ShowModFailureDialog(header, desc);
				yield break;
			}
			yield return null;
			num = i;
		}
		yield break;
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00009213 File Offset: 0x00007413
	public static void ResetLoadingState()
	{
		GameControl.gameStartedUnloading = false;
		GameControl.bootstrapFinished = false;
		GameControl.initialized = false;
		GameControl.loadcycle100 = false;
		GameControl.resolutionChangeCount = 0;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00009233 File Offset: 0x00007433
	public IEnumerator InitCanvas()
	{
		yield return null;
		yield break;
	}

	// Token: 0x0600012C RID: 300 RVA: 0x0000923B File Offset: 0x0000743B
	public void CheckDLCLicense()
	{
	}

	// Token: 0x0600012D RID: 301 RVA: 0x0000923D File Offset: 0x0000743D
	public void ValidateDLC()
	{
		GameControl.DLCValidated = true;
	}

	// Token: 0x0600012E RID: 302 RVA: 0x00009248 File Offset: 0x00007448
	public void UpdateLoading(float value)
	{
		if (this.loadScreenWidget == null)
		{
			this.loadScreenWidget = global::UnityEngine.Object.FindObjectOfType<LoadScreenWidget>();
		}
		this.loadScreenWidget.SetBar(value / 100f, false);
		if (value == 100f)
		{
			Debug.Log(value.ToString() + "% Loaded");
			this.loadScreenWidget.SetBar(1f, true);
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x000092B0 File Offset: 0x000074B0
	public void LoadLoadingIllustration()
	{
		if (this.loadScreenWidget == null)
		{
			this.loadScreenWidget = global::UnityEngine.Object.FindObjectOfType<LoadScreenWidget>();
		}
		this.loadScreenWidget.LoadIllustration();
	}

	// Token: 0x06000130 RID: 304 RVA: 0x000092D8 File Offset: 0x000074D8
	public void LogTotalLoadTime()
	{
		Log.Info("<color=#00cc00>LoadTime:</color> Campaign total load time was " + (Time.realtimeSinceStartup - this.loadStartTimeStamp).ToString() + " seconds.", Array.Empty<object>());
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00009314 File Offset: 0x00007514
	private void Update()
	{
		if (GameControl.loadcycle100 || Application.isEditor)
		{
			return;
		}
		if (((float)Screen.currentResolution.width != TIPlayerProfileManager.storedResolution.x || (float)Screen.currentResolution.height != TIPlayerProfileManager.storedResolution.y) && Screen.fullScreen && GameControl.resolutionChangeCount < 10)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Resolution Change Detected: ",
				Screen.currentResolution.ToString(),
				", Restoring Original Resolution: ",
				TIPlayerProfileManager.storedResolution.x.ToString(),
				",",
				TIPlayerProfileManager.storedResolution.y.ToString()
			}));
			Screen.SetResolution((int)TIPlayerProfileManager.storedResolution.x, (int)TIPlayerProfileManager.storedResolution.y, Screen.fullScreen);
			GameControl.resolutionChangeCount++;
			if (GameControl.resolutionChangeCount > 10)
			{
				Debug.Log("Failing to set resolution on monitor?");
			}
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0000941E File Offset: 0x0000761E
	private void OnApplicationQuit()
	{
		Debug.Log("OnApplicationQuit");
		GameControl.loadcycle100 = false;
		if (iCueDllHooks.Instance != null)
		{
			iCueDllHooks.Instance.Dispose();
		}
	}

	// Token: 0x0400010D RID: 269
	internal CanvasManager _canvasStack;

	// Token: 0x0400010E RID: 270
	[global::Zenject.Inject]
	private EventManager _eventManager;

	// Token: 0x0400010F RID: 271
	internal AssetLoader _assetLoader;

	// Token: 0x04000110 RID: 272
	[global::Zenject.Inject]
	private SolarSystemControl _solarSystem;

	// Token: 0x04000111 RID: 273
	[global::Zenject.Inject]
	private PlayerManager _playerManager;

	// Token: 0x04000112 RID: 274
	[global::Zenject.Inject]
	private SpaceCombatManager _spaceCombat;

	// Token: 0x04000113 RID: 275
	[global::Zenject.Inject]
	private NamelistManager _namelists;

	// Token: 0x04000114 RID: 276
	public Camera mainCamera;

	// Token: 0x04000115 RID: 277
	public Transform mainCameraTransform;

	// Token: 0x04000118 RID: 280
	public bool skirmishMode;

	// Token: 0x04000119 RID: 281
	public bool startupTutorialActive;

	// Token: 0x0400011A RID: 282
	public int startupDifficulty = 2;

	// Token: 0x0400011B RID: 283
	[HideInInspector]
	public ScenarioCustomizations scenarioCustomizationsStartup = new ScenarioCustomizations();

	// Token: 0x0400011C RID: 284
	public static bool bootstrapFinished;

	// Token: 0x0400011D RID: 285
	public static bool initialized;

	// Token: 0x0400011E RID: 286
	public static bool loadcycle100;

	// Token: 0x0400011F RID: 287
	public static int frameFinishedLoading;

	// Token: 0x04000120 RID: 288
	public static bool gameStartedUnloading;

	// Token: 0x04000121 RID: 289
	public static bool handlingException;

	// Token: 0x04000122 RID: 290
	public static bool needToViewSpaceCombat;

	// Token: 0x04000124 RID: 292
	public LoadScreenWidget loadScreenWidget;

	// Token: 0x04000125 RID: 293
	public float loadStartTimeStamp;

	// Token: 0x04000126 RID: 294
	private TIMetaTemplate _scenarioMetaTemplate;

	// Token: 0x04000127 RID: 295
	private static int resolutionChangeCount;

	// Token: 0x02000ABF RID: 2751
	public enum Storefront
	{
		// Token: 0x04004857 RID: 18519
		STEAM,
		// Token: 0x04004858 RID: 18520
		GOG
	}
}
