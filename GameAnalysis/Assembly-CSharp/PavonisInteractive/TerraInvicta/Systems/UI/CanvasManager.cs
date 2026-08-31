using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Systems.UI
{
	// Token: 0x020009BE RID: 2494
	public class CanvasManager : ManagerSystem
	{
		// Token: 0x1700102A RID: 4138
		// (get) Token: 0x06005DE7 RID: 24039 RVA: 0x002CAD3D File Offset: 0x002C8F3D
		// (set) Token: 0x06005DE8 RID: 24040 RVA: 0x002CAD45 File Offset: 0x002C8F45
		public bool initted { get; private set; }

		// Token: 0x1700102B RID: 4139
		// (get) Token: 0x06005DE9 RID: 24041 RVA: 0x002CAD4E File Offset: 0x002C8F4E
		public IInfoScreen ActiveInfoScreen
		{
			get
			{
				return this.activeInfoScreen;
			}
		}

		// Token: 0x06005DEA RID: 24042 RVA: 0x002CAD58 File Offset: 0x002C8F58
		public void Initialize()
		{
			this.canvasGOs.Clear();
			this.canvases.Clear();
			this.canvasesByType.Clear();
			this.infoScreens.Clear();
			this.disableInfoPanelOrders.Clear();
			this.disableAssetPanelOrders.Clear();
			this.activeAssetPanel = AssetPanel.None;
			this.activeInfoPanel = InfoPanel.None;
			Log.Time("<color=#00cc00>LoadTime:</color> LoadAllCanvases", delegate
			{
				foreach (Camera camera in Camera.allCameras)
				{
					if (camera.gameObject.name == "UICamera")
					{
						this.UICamera = camera;
						break;
					}
				}
				string[] array = (GameControl.control.skirmishMode ? TemplateManager.global.skirmishCanvasesToLoad : TemplateManager.global.canvasesToLoad);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					GameObject gameObject = GameControl.assetLoader.InstantiatePrefab("ui/" + text);
					if (gameObject == null)
					{
						throw new Exception("No Canvas " + text);
					}
					ICanvas canvas = gameObject.GetComponent<ICanvas>();
					if (canvas == null)
					{
						Log.Warn("No ICanvas found on " + text, Array.Empty<object>());
					}
					else
					{
						Log.Time("<color=#00cc00>LoadTime:</color> Init Canvas " + text, delegate
						{
							canvas.Initialize();
						}, true, true);
						if (canvas.Canvas == null)
						{
							Log.Debug("No Canvas component on " + text, Array.Empty<object>());
						}
						else
						{
							canvas.Canvas.gameObject.SetActive(false);
							canvas.Canvas.gameObject.SetActive(true);
							if (canvas is ResearchScreenController)
							{
								canvas.HideNoCache();
							}
							else
							{
								canvas.Hide();
							}
							canvas.Canvas.worldCamera = this.UICamera;
							gameObject.name = text;
							this.canvasGOs[gameObject.name] = gameObject;
							this.canvases[gameObject.name] = canvas;
							this.canvasesByType[canvas.GetType()] = canvas;
							if (canvas is OptionsScreenController)
							{
								this.OptionsScreen = canvas;
							}
							else if (canvas is CodexController)
							{
								this.Codex = canvas;
							}
							else if (canvas is IInfoScreen)
							{
								this.infoScreens.Add(canvas.GetType(), (IInfoScreen)canvas);
							}
							else if (canvas is GeneralControlsController)
							{
								this.StrategyHud = (IHud)canvas;
							}
							else if (canvas is SpaceCombatCanvasController)
							{
								this.CombatHud = (IHud)canvas;
							}
							else if (canvas is NationInfoController)
							{
								this.NationInfo = canvas;
							}
							else if (canvas is NotificationScreenController)
							{
								this.Notifications = canvas;
							}
							else if (canvas is CouncilorMissionCanvasController)
							{
								this.CouncilorMissionController = canvas;
							}
							else if (canvas is OperationCanvasController)
							{
								this.OperationCanvasController = canvas;
							}
							else if (canvas is SpaceObjectDetailController)
							{
								this.SpaceObjectDetail = canvas;
							}
							else if (canvas is ArmyDetailController)
							{
								this.ArmyDetail = canvas;
							}
							else if (canvas is PrecombatController)
							{
								this.PrecombatControllerCanvas = canvas;
							}
						}
					}
				}
				GameControl.eventManager.AddListener<InfoPanelOpened>(new EventManager.EventDelegate<InfoPanelOpened>(this.OnInfoPanelOpened), null, null, true, false);
				GameControl.eventManager.AddListener<MyAssetPanelOpened>(new EventManager.EventDelegate<MyAssetPanelOpened>(this.OnAssetPanelOpened), null, null, true, false);
				IHud strategyHud = this.StrategyHud;
				if (strategyHud != null)
				{
					strategyHud.Show();
				}
				ICanvas notifications = this.Notifications;
				if (notifications == null)
				{
					return;
				}
				notifications.Show();
			}, true, true);
		}

		// Token: 0x06005DEB RID: 24043 RVA: 0x002CADD0 File Offset: 0x002C8FD0
		protected override void OnUpdate()
		{
			foreach (ICanvas canvas in this.canvases.Values)
			{
				if (canvas.Canvas.enabled)
				{
					canvas.Refresh();
				}
			}
		}

		// Token: 0x06005DEC RID: 24044 RVA: 0x002CAE34 File Offset: 0x002C9034
		public void RefreshUIScaling()
		{
			foreach (ICanvas canvas in this.canvases.Values)
			{
				canvas.RefreshScaling();
			}
		}

		// Token: 0x06005DED RID: 24045 RVA: 0x002CAE8C File Offset: 0x002C908C
		public void RefreshUltraWideScaling()
		{
			foreach (ICanvas canvas in this.canvases.Values)
			{
				canvas.SetUltraWideScaling();
			}
		}

		// Token: 0x06005DEE RID: 24046 RVA: 0x002CAEE4 File Offset: 0x002C90E4
		public T Canvas<T>() where T : ICanvas
		{
			return (T)((object)this.canvasesByType[typeof(T)]);
		}

		// Token: 0x06005DEF RID: 24047 RVA: 0x002CAF00 File Offset: 0x002C9100
		public void HideStrategyLayerUIs()
		{
			this.CloseActiveInfoScreen();
			this.SetActiveAssetPanel(AssetPanel.None, 0f);
			this.SetActiveInfoPanel(InfoPanel.None, 0f);
			IHud strategyHud = this.StrategyHud;
			if (strategyHud != null)
			{
				strategyHud.Hide();
			}
			ICanvas spaceObjectDetail = this.SpaceObjectDetail;
			if (spaceObjectDetail != null)
			{
				spaceObjectDetail.Hide();
			}
			ICanvas armyDetail = this.ArmyDetail;
			if (armyDetail != null)
			{
				armyDetail.Hide();
			}
			ICanvas councilorMissionController = this.CouncilorMissionController;
			if (councilorMissionController != null)
			{
				councilorMissionController.Hide();
			}
			ICanvas operationCanvasController = this.OperationCanvasController;
			if (operationCanvasController != null)
			{
				operationCanvasController.Hide();
			}
			ICanvas notifications = this.Notifications;
			if (notifications != null)
			{
				notifications.Hide();
			}
			ICanvas nationInfo = this.NationInfo;
			if (nationInfo != null)
			{
				nationInfo.Hide();
			}
			GeneralControlsController.SetUISelectedAssetState(null);
			GeneralControlsController.SetUIOtherSelectedState(null);
		}

		// Token: 0x06005DF0 RID: 24048 RVA: 0x002CAFB0 File Offset: 0x002C91B0
		public void RestoreStrategyLayerUIs()
		{
			IHud strategyHud = this.StrategyHud;
			if (strategyHud != null)
			{
				strategyHud.Show();
			}
			ICanvas notifications = this.Notifications;
			if (notifications != null)
			{
				notifications.Show();
			}
			if (TIMissionPhaseState.InMissionPhase())
			{
				ICanvas councilorMissionController = this.CouncilorMissionController;
				if (councilorMissionController != null)
				{
					councilorMissionController.Show();
				}
				GameControl.eventManager.TriggerEvent(new MissionPhaseRestart(), null, Array.Empty<object>());
			}
			ICanvas operationCanvasController = this.OperationCanvasController;
			if (operationCanvasController == null)
			{
				return;
			}
			operationCanvasController.Show();
		}

		// Token: 0x06005DF1 RID: 24049 RVA: 0x002CB01C File Offset: 0x002C921C
		public void ResetActivePlayerDuringRunTime()
		{
			foreach (ICanvas canvas in this.canvases.Values)
			{
				(canvas as CanvasControllerBase).SetActivePlayer(false);
			}
		}

		// Token: 0x06005DF2 RID: 24050 RVA: 0x002CB078 File Offset: 0x002C9278
		public bool IsShowingInfoScreen()
		{
			return this.activeInfoScreen != null;
		}

		// Token: 0x06005DF3 RID: 24051 RVA: 0x002CB083 File Offset: 0x002C9283
		public bool IsShowingInfoScreen<T>() where T : IInfoScreen
		{
			return this.activeInfoScreen != null && this.activeInfoScreen.GetType() == typeof(T);
		}

		// Token: 0x06005DF4 RID: 24052 RVA: 0x002CB0AC File Offset: 0x002C92AC
		public void ToggleInfoScreen<T>() where T : IInfoScreen
		{
			bool flag = !this.IsShowingInfoScreen<T>();
			if (this.activeInfoScreen != null)
			{
				this.activeInfoScreen.CloseInfoScreen(flag);
				this.activeInfoScreen = null;
			}
			if (flag)
			{
				this.ShowInfoScreen<T>();
			}
		}

		// Token: 0x06005DF5 RID: 24053 RVA: 0x002CB0E8 File Offset: 0x002C92E8
		public IInfoScreen ShowInfoScreen<T>() where T : IInfoScreen
		{
			if (this.IsShowingInfoScreen<T>())
			{
				return this.infoScreens[typeof(T)];
			}
			IInfoScreen infoScreen = this.activeInfoScreen;
			if (infoScreen != null)
			{
				infoScreen.CloseInfoScreen(false);
			}
			this.activeInfoScreen = this.infoScreens[typeof(T)];
			this.activeInfoScreen.Show();
			GameControl.eventManager.TriggerEvent(new InfoScreenOpened(), null, Array.Empty<object>());
			return this.activeInfoScreen;
		}

		// Token: 0x06005DF6 RID: 24054 RVA: 0x002CB166 File Offset: 0x002C9366
		public T GetInfoScreen<T>() where T : class, IInfoScreen
		{
			return this.infoScreens[typeof(T)] as T;
		}

		// Token: 0x06005DF7 RID: 24055 RVA: 0x002CB188 File Offset: 0x002C9388
		public void HideInfoScreen<T>(bool toggle = false) where T : IInfoScreen
		{
			if (this.activeInfoScreen == null)
			{
				if (this.infoScreens.ContainsKey(typeof(T)))
				{
					this.infoScreens[typeof(T)].Hide();
				}
				return;
			}
			if (this.activeInfoScreen is T)
			{
				this.activeInfoScreen.Hide();
				this.activeInfoScreen = null;
				if (!toggle)
				{
					GameControl.eventManager.TriggerEvent(new InfoScreenClosed(), null, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06005DF8 RID: 24056 RVA: 0x002CB206 File Offset: 0x002C9406
		public void OnInfoPanelOpened(InfoPanelOpened e)
		{
			if (this.activeInfoScreen != null)
			{
				this.activeInfoScreen.CloseInfoScreen(false);
			}
		}

		// Token: 0x06005DF9 RID: 24057 RVA: 0x002CB21C File Offset: 0x002C941C
		public void OnAssetPanelOpened(MyAssetPanelOpened e)
		{
			if (this.activeInfoScreen != null)
			{
				this.activeInfoScreen.CloseInfoScreen(false);
			}
		}

		// Token: 0x06005DFA RID: 24058 RVA: 0x002CB232 File Offset: 0x002C9432
		public void CloseActiveInfoScreen()
		{
			if (this.activeInfoScreen != null)
			{
				this.activeInfoScreen.CloseInfoScreen(false);
			}
		}

		// Token: 0x06005DFB RID: 24059 RVA: 0x002CB248 File Offset: 0x002C9448
		public void ClearCanvas(ICanvas canvas)
		{
			GameObject gameObject = canvas.GameObject;
			this.canvasGOs.Remove(gameObject.name);
			this.canvases.Remove(gameObject.name);
			this.canvasesByType.Remove(canvas.GetType());
			if (canvas is OptionsScreenController)
			{
				this.OptionsScreen = null;
				return;
			}
			if (canvas is IInfoScreen)
			{
				this.infoScreens.Remove(canvas.GetType());
				return;
			}
			if (canvas is GeneralControlsController)
			{
				this.StrategyHud = null;
				return;
			}
			if (canvas is SpaceCombatCanvasController)
			{
				this.CombatHud = null;
				return;
			}
			if (canvas is NationInfoController)
			{
				this.NationInfo = null;
				return;
			}
			if (canvas is NotificationScreenController)
			{
				this.Notifications = null;
				return;
			}
			if (canvas is CouncilorMissionCanvasController)
			{
				this.CouncilorMissionController = null;
				return;
			}
			if (canvas is OperationCanvasController)
			{
				this.OperationCanvasController = null;
				return;
			}
			if (canvas is SpaceObjectDetailController)
			{
				this.SpaceObjectDetail = null;
				return;
			}
			if (canvas is ArmyDetailController)
			{
				this.ArmyDetail = null;
				return;
			}
			if (canvas is PrecombatController)
			{
				this.PrecombatControllerCanvas = null;
			}
		}

		// Token: 0x06005DFC RID: 24060 RVA: 0x002CB34C File Offset: 0x002C954C
		public void RegisterInfoPanelDisableOrder(InfoPanel infoPanel, Action disableOrder)
		{
			if (!this.disableInfoPanelOrders.ContainsKey(infoPanel))
			{
				this.disableInfoPanelOrders.Add(infoPanel, new List<Action>());
			}
			this.disableInfoPanelOrders[infoPanel].Add(disableOrder);
		}

		// Token: 0x06005DFD RID: 24061 RVA: 0x002CB37F File Offset: 0x002C957F
		public void RegisterAssetPanelDisableOrder(AssetPanel assetPanel, Action disableOrder)
		{
			if (!this.disableAssetPanelOrders.ContainsKey(assetPanel))
			{
				this.disableAssetPanelOrders.Add(assetPanel, new List<Action>());
			}
			this.disableAssetPanelOrders[assetPanel].Add(disableOrder);
		}

		// Token: 0x06005DFE RID: 24062 RVA: 0x002CB3B4 File Offset: 0x002C95B4
		public void SetActiveInfoPanel(InfoPanel infoPanel, float panelHeight = 0f)
		{
			InfoPanel infoPanel2 = this.activeInfoPanel;
			this.activeInfoPanel = infoPanel;
			if (this.activeInfoPanel != infoPanel2)
			{
				if (this.disableInfoPanelOrders.ContainsKey(infoPanel2))
				{
					foreach (Action action in this.disableInfoPanelOrders[infoPanel2])
					{
						action();
					}
				}
				if (this.activeInfoPanel == InfoPanel.None)
				{
					GeneralControlsController.SetUIOtherSelectedState(null);
					GameControl.eventManager.TriggerEvent(new InfoWindowEntirelyClosed(), null, Array.Empty<object>());
				}
				else
				{
					GameControl.eventManager.TriggerEvent(new InfoPanelOpened(infoPanel, panelHeight), null, Array.Empty<object>());
				}
				if (infoPanel2 != InfoPanel.None)
				{
					GameControl.eventManager.TriggerEvent(new InfoPanelClosed(infoPanel2), null, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06005DFF RID: 24063 RVA: 0x002CB488 File Offset: 0x002C9688
		public void ActiveInfoPanelResized(float floatPanelHeight)
		{
			GameControl.eventManager.TriggerEvent(new MyActiveInfoPanelResized(floatPanelHeight), null, Array.Empty<object>());
		}

		// Token: 0x06005E00 RID: 24064 RVA: 0x002CB4A0 File Offset: 0x002C96A0
		public InfoPanel GetActiveInfoPanel()
		{
			return this.activeInfoPanel;
		}

		// Token: 0x06005E01 RID: 24065 RVA: 0x002CB4A8 File Offset: 0x002C96A8
		public void SetActiveAssetPanel(AssetPanel assetPanel, float panelHeight)
		{
			AssetPanel assetPanel2 = this.activeAssetPanel;
			this.activeAssetPanel = assetPanel;
			if (this.activeAssetPanel != assetPanel2)
			{
				if (this.disableAssetPanelOrders.ContainsKey(assetPanel2))
				{
					foreach (Action action in this.disableAssetPanelOrders[assetPanel2])
					{
						action();
					}
				}
				if (this.activeAssetPanel == AssetPanel.None)
				{
					GeneralControlsController.SetUISelectedAssetState(null);
					GameControl.eventManager.TriggerEvent(new MyAssetPanelEntirelyClosed(), null, Array.Empty<object>());
				}
				else
				{
					GameControl.eventManager.TriggerEvent(new MyAssetPanelOpened(assetPanel, panelHeight), null, Array.Empty<object>());
					this.ActiveAssetPanelResized(panelHeight);
				}
				if (assetPanel2 != AssetPanel.None)
				{
					GameControl.eventManager.TriggerEvent(new MyAssetPanelClosed(assetPanel2), null, Array.Empty<object>());
				}
			}
		}

		// Token: 0x06005E02 RID: 24066 RVA: 0x002CB584 File Offset: 0x002C9784
		public void ActiveAssetPanelResized(float floatPanelHeight)
		{
			GameControl.eventManager.TriggerEvent(new MyActiveAssetPanelResized(floatPanelHeight), null, Array.Empty<object>());
		}

		// Token: 0x06005E03 RID: 24067 RVA: 0x002CB59C File Offset: 0x002C979C
		public AssetPanel GetActiveAssetPanel()
		{
			return this.activeAssetPanel;
		}

		// Token: 0x06005E04 RID: 24068 RVA: 0x002CB5A4 File Offset: 0x002C97A4
		public void HideAll()
		{
			foreach (ICanvas canvas in this.canvases.Values)
			{
				if (canvas.Visible())
				{
					canvas.Hide();
				}
			}
		}

		// Token: 0x06005E05 RID: 24069 RVA: 0x002CB604 File Offset: 0x002C9804
		public void Show(GameObject canvasObject)
		{
			this.canvases[canvasObject.name].Canvas.enabled = true;
		}

		// Token: 0x06005E06 RID: 24070 RVA: 0x002CB622 File Offset: 0x002C9822
		public void Hide(string name)
		{
			this.canvases[name].Canvas.enabled = false;
		}

		// Token: 0x06005E07 RID: 24071 RVA: 0x002CB63B File Offset: 0x002C983B
		public bool IsVisible(string name)
		{
			return this.canvases[name].Canvas.enabled;
		}

		// Token: 0x04004323 RID: 17187
		private Camera UICamera;

		// Token: 0x04004325 RID: 17189
		public ICanvas OptionsScreen;

		// Token: 0x04004326 RID: 17190
		public ICanvas NationInfo;

		// Token: 0x04004327 RID: 17191
		public ICanvas Notifications;

		// Token: 0x04004328 RID: 17192
		public ICanvas Codex;

		// Token: 0x04004329 RID: 17193
		public ICanvas SpaceObjectDetail;

		// Token: 0x0400432A RID: 17194
		public IHud StrategyHud;

		// Token: 0x0400432B RID: 17195
		public IHud CombatHud;

		// Token: 0x0400432C RID: 17196
		public ICanvas CouncilorMissionController;

		// Token: 0x0400432D RID: 17197
		public ICanvas OperationCanvasController;

		// Token: 0x0400432E RID: 17198
		public ICanvas ArmyDetail;

		// Token: 0x0400432F RID: 17199
		public ICanvas PrecombatControllerCanvas;

		// Token: 0x04004330 RID: 17200
		private readonly Dictionary<string, GameObject> canvasGOs = new Dictionary<string, GameObject>();

		// Token: 0x04004331 RID: 17201
		private readonly Dictionary<string, ICanvas> canvases = new Dictionary<string, ICanvas>();

		// Token: 0x04004332 RID: 17202
		private readonly Dictionary<Type, ICanvas> canvasesByType = new Dictionary<Type, ICanvas>();

		// Token: 0x04004333 RID: 17203
		private readonly Dictionary<Type, IInfoScreen> infoScreens = new Dictionary<Type, IInfoScreen>();

		// Token: 0x04004334 RID: 17204
		private IInfoScreen activeInfoScreen;

		// Token: 0x04004335 RID: 17205
		private AssetPanel activeAssetPanel;

		// Token: 0x04004336 RID: 17206
		private InfoPanel activeInfoPanel;

		// Token: 0x04004337 RID: 17207
		public Dictionary<InfoPanel, List<Action>> disableInfoPanelOrders = new Dictionary<InfoPanel, List<Action>>();

		// Token: 0x04004338 RID: 17208
		public Dictionary<AssetPanel, List<Action>> disableAssetPanelOrders = new Dictionary<AssetPanel, List<Action>>();
	}
}
