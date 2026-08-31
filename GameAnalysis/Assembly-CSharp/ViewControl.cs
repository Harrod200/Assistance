using System;
using System.Collections;
using System.Collections.Generic;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using PavonisInteractive.TerraInvicta.Systems.SolarSystem;
using PavonisInteractive.TerraInvicta.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;

// Token: 0x0200002F RID: 47
public class ViewControl : MonoBehaviour
{
	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060001CB RID: 459 RVA: 0x0000EC8B File Offset: 0x0000CE8B
	// (set) Token: 0x060001CC RID: 460 RVA: 0x0000EC93 File Offset: 0x0000CE93
	public GameObject earthObject { get; private set; }

	// Token: 0x060001CD RID: 461 RVA: 0x0000EC9C File Offset: 0x0000CE9C
	public void Awake()
	{
		this.spaceCombat.enabled = false;
		TIGlobalValuesState.isSpaceCombatEnabled = false;
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000ECB0 File Offset: 0x0000CEB0
	public void StartSession()
	{
		string name = global::UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		if (name != null)
		{
			if (name == "SolarSystemScene")
			{
				this.currentView = ViewType.SolarSystem;
				return;
			}
			if (!(name == "StartScreenScene"))
			{
				return;
			}
			this.currentView = ViewType.MainMenu;
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000ECF8 File Offset: 0x0000CEF8
	public void Initialize()
	{
		this.globalRegionEffectRenderer = Camera.main.GetComponent<RegionEffectRenderer>();
		this.eventManager.AddListener<MapActivationChangedEvent>(new EventManager.EventDelegate<MapActivationChangedEvent>(this.MapChanged), null, null, false, false);
		foreach (ScriptBehaviourManager scriptBehaviourManager in World.Active.BehaviourManagers)
		{
			StrategyLayerComponentSystem strategyLayerComponentSystem = scriptBehaviourManager as StrategyLayerComponentSystem;
			if (strategyLayerComponentSystem != null)
			{
				strategyLayerComponentSystem.Initialize();
			}
		}
		this.SetAllStrategyLayerECSComponents(false);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x0000ED84 File Offset: 0x0000CF84
	public void SetEarthObject(SpaceObjectController container)
	{
		this.earthObject = container.gameObject;
		this.globalRegionEffectRenderer.Initialize();
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x0000EDA0 File Offset: 0x0000CFA0
	public void SetAllStrategyLayerECSComponents(bool enable)
	{
		if (enable != this.strategyLayerComponentsActive)
		{
			foreach (ScriptBehaviourManager scriptBehaviourManager in World.Active.BehaviourManagers)
			{
				StrategyLayerComponentSystem strategyLayerComponentSystem = scriptBehaviourManager as StrategyLayerComponentSystem;
				if (strategyLayerComponentSystem != null)
				{
					strategyLayerComponentSystem.Enabled = enable;
				}
			}
			this.strategyLayerComponentsActive = enable;
		}
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x0000EE0C File Offset: 0x0000D00C
	private void SetNaturalSpaceSymbolTooltips(bool enable)
	{
		for (int i = 0; i < ViewControl.naturalSpaceSymbolTooltips.Count; i++)
		{
			ViewControl.naturalSpaceSymbolTooltips[i].enabled = enable;
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x0000EE40 File Offset: 0x0000D040
	public static void SetEnableAllStrategyShipModels(bool enable)
	{
		foreach (TISpaceFleetState tispaceFleetState in GameStateManager.IterateByClass<TISpaceFleetState>(false))
		{
			StrategyShipController[] componentsInChildren = tispaceFleetState.gameObjectLink.GetComponentsInChildren<StrategyShipController>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetActive(enable);
			}
		}
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x0000EEB0 File Offset: 0x0000D0B0
	private void MapChanged(MapActivationChangedEvent e)
	{
		if (e.active)
		{
			if (this.currentView != ViewType.PoliticalMap)
			{
				this.currentView = ViewType.PoliticalMap;
				this.SetNaturalSpaceSymbolTooltips(false);
				this.globalRegionEffectRenderer.enabled = true;
				return;
			}
		}
		else if (this.currentView == ViewType.PoliticalMap)
		{
			this.currentView = ViewType.SolarSystem;
			this.SetNaturalSpaceSymbolTooltips(true);
			this.globalRegionEffectRenderer.enabled = false;
		}
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x0000EF0C File Offset: 0x0000D10C
	public void GotoView(ViewType newView)
	{
		if (!this.assigned)
		{
			this.selection = World.Active.GetExistingManager<SpaceObjectSelection>();
			this.assigned = true;
		}
		switch (newView)
		{
		case ViewType.MainMenu:
			TooltipManager.Instance.HideAll();
			this.solarSystem.enabled = false;
			this.selection.SelectObject(null, true, false);
			this.SetAllStrategyLayerECSComponents(false);
			TIArmyState.FinishBakingJourneyHeuristic();
			this.ClearGameData(false);
			global::UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("StartScreenScene");
			if (this.globalRegionEffectRenderer != null)
			{
				this.globalRegionEffectRenderer.enabled = false;
			}
			break;
		case ViewType.SolarSystem:
			TooltipManager.Instance.HideAll();
			if (global::UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SolarSystemScene")
			{
				this.spaceCombat.enabled = false;
				TIGlobalValuesState.isSpaceCombatEnabled = false;
			}
			else
			{
				global::UnityEngine.SceneManagement.SceneManager.LoadScene("SolarSystemScene");
			}
			this.SetAllStrategyLayerECSComponents(true);
			this.currentView = ViewType.SolarSystem;
			this.solarSystem.enabled = true;
			this.SetNaturalSpaceSymbolTooltips(true);
			this.globalRegionEffectRenderer.enabled = false;
			break;
		case ViewType.PoliticalMap:
			TooltipManager.Instance.HideAll();
			this.SetAllStrategyLayerECSComponents(true);
			this.selection.SelectObject(this.earthObject, false, false);
			TIUtilities.GotoGameState(GameStateManager.Earth(), true, false, false, true, false, -1f);
			this.SetNaturalSpaceSymbolTooltips(false);
			this.currentView = ViewType.PoliticalMap;
			this.globalRegionEffectRenderer.enabled = true;
			break;
		case ViewType.SpaceCombat:
			this.selection.SelectObject(null, true, false);
			this.solarSystem.enabled = false;
			ViewControl.SetEnableAllStrategyShipModels(false);
			this.SetAllStrategyLayerECSComponents(false);
			TooltipManager.Instance.HideAll();
			this.spaceCombat.Initialize();
			this.spaceCombat.enabled = true;
			TIGlobalValuesState.isSpaceCombatEnabled = true;
			this.globalRegionEffectRenderer.enabled = false;
			GameControl.eventManager.TriggerEvent(new CombatStarts(this.spaceCombat.combatState), null, Array.Empty<object>());
			break;
		}
		this.currentView = newView;
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000F100 File Offset: 0x0000D300
	public void ClearGameData(bool loadGame = false)
	{
		Log.Info("Clearing GameData", Array.Empty<object>());
		GameControl.gameStartedUnloading = true;
		CoroutineDummy.Singleton.StopAll();
		if (!loadGame)
		{
			this.CleanupTextures();
			TemplateManager.ClearAllTemplates();
			SolarSystemInstaller.container.Resolve<TemplateManager>().Initialize(Application.streamingAssetsPath + "/Templates");
		}
		base.StartCoroutine(this.CleanupData());
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000F165 File Offset: 0x0000D365
	private IEnumerator CleanupData()
	{
		yield return null;
		ArchetypeDecision.ClearTemplates();
		World.Active.GetExistingManager<SpaceObjectPositioning>().ResetCounts();
		this.spaceCombat.enabled = false;
		TIGlobalValuesState.isSpaceCombatEnabled = false;
		this.solarSystem.DestroySolarSystem();
		this.solarSystem.enabled = false;
		if (TooltipManager.Instance != null)
		{
			TooltipManager.Instance.HideAll();
		}
		if (this.eventManager != null)
		{
			this.eventManager.ClearAllEvents();
		}
		GameStateManager.ClearAllGameStates();
		this.ClearAllEntities();
		List<TooltipTrigger> list = ViewControl.naturalSpaceSymbolTooltips;
		if (list != null)
		{
			list.Clear();
		}
		AudioManager.StopAllEvents();
		GeneralControlsController generalControlsController = global::UnityEngine.Object.FindObjectOfType<GeneralControlsController>();
		if (generalControlsController != null)
		{
			generalControlsController.Cleanup();
		}
		GameControl.ResetLoadingState();
		yield break;
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x0000F174 File Offset: 0x0000D374
	private void CleanupTextures()
	{
		if (!GameControl.control.skirmishMode)
		{
			NotificationScreenController notificationScreenController = global::UnityEngine.Object.FindObjectOfType<NotificationScreenController>();
			if (notificationScreenController != null)
			{
				notificationScreenController.CleanupTextures();
			}
			FleetsScreenController fleetsScreenController = global::UnityEngine.Object.FindObjectOfType<FleetsScreenController>();
			if (fleetsScreenController != null)
			{
				fleetsScreenController.CleanupTextures();
			}
			SpaceObjectDetailController spaceObjectDetailController = global::UnityEngine.Object.FindObjectOfType<SpaceObjectDetailController>();
			if (spaceObjectDetailController != null)
			{
				spaceObjectDetailController.CleanupTextures();
			}
			GeneralControlsController generalControlsController = global::UnityEngine.Object.FindObjectOfType<GeneralControlsController>();
			if (generalControlsController != null)
			{
				generalControlsController.Cleanup();
			}
		}
		foreach (Camera camera in global::UnityEngine.Object.FindObjectsOfType<Camera>())
		{
			if (camera.targetTexture != null)
			{
				RenderTexture targetTexture = camera.targetTexture;
				camera.targetTexture = null;
				targetTexture.Release();
			}
		}
		foreach (VideoPlayer videoPlayer in global::UnityEngine.Object.FindObjectsOfType<VideoPlayer>())
		{
			if (videoPlayer.targetTexture != null)
			{
				RenderTexture targetTexture2 = videoPlayer.targetTexture;
				videoPlayer.targetTexture = null;
				targetTexture2.Release();
			}
		}
		foreach (RawImage rawImage in global::UnityEngine.Object.FindObjectsOfType<RawImage>())
		{
			if (rawImage.texture != null)
			{
				RenderTexture renderTexture = (RenderTexture)rawImage.texture;
				rawImage.texture = null;
				renderTexture.Release();
			}
		}
		Resources.UnloadUnusedAssets();
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x0000F2C4 File Offset: 0x0000D4C4
	private void ClearAllEntities()
	{
		EntityManager existingManager = World.Active.GetExistingManager<EntityManager>();
		NativeArray<Entity> allEntities = existingManager.GetAllEntities(Allocator.Temp);
		foreach (Entity entity in allEntities)
		{
			existingManager.DestroyEntity(entity);
		}
		allEntities.Dispose();
	}

	// Token: 0x060001DA RID: 474 RVA: 0x0000F330 File Offset: 0x0000D530
	public void DisableSolarSystemForSkirmishMode(IScenario scenario)
	{
		this.solarSystem.DisableSolarSystemObjectsForSkirmishMode(scenario);
	}

	// Token: 0x040001EA RID: 490
	public ViewType currentView;

	// Token: 0x040001EC RID: 492
	private SpaceObjectSelection selection;

	// Token: 0x040001ED RID: 493
	[global::Zenject.Inject]
	private EventManager eventManager;

	// Token: 0x040001EE RID: 494
	[global::Zenject.Inject]
	private SolarSystemControl solarSystem;

	// Token: 0x040001EF RID: 495
	[global::Zenject.Inject]
	private SpaceCombatManager spaceCombat;

	// Token: 0x040001F0 RID: 496
	private RegionEffectRenderer globalRegionEffectRenderer;

	// Token: 0x040001F1 RID: 497
	public static List<TooltipTrigger> naturalSpaceSymbolTooltips = new List<TooltipTrigger>();

	// Token: 0x040001F2 RID: 498
	private bool strategyLayerComponentsActive;

	// Token: 0x040001F3 RID: 499
	private bool assigned;
}
