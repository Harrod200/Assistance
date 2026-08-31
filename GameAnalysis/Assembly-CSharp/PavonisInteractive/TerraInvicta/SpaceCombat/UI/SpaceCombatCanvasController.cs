using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A09 RID: 2569
	public class SpaceCombatCanvasController : CanvasControllerBase, IHud, ICanvas
	{
		// Token: 0x17001109 RID: 4361
		// (get) Token: 0x06006335 RID: 25397 RVA: 0x002EC317 File Offset: 0x002EA517
		// (set) Token: 0x06006336 RID: 25398 RVA: 0x002EC31F File Offset: 0x002EA51F
		[HideInInspector]
		public IDictionary<CombatantController, FriendlyShipListItemController> leftHandCombatants { get; private set; }

		// Token: 0x1700110A RID: 4362
		// (get) Token: 0x06006337 RID: 25399 RVA: 0x002EC328 File Offset: 0x002EA528
		// (set) Token: 0x06006338 RID: 25400 RVA: 0x002EC330 File Offset: 0x002EA530
		[HideInInspector]
		public IDictionary<CombatantController, EnemyShipListItemController> rightHandCombatants { get; private set; }

		// Token: 0x1700110B RID: 4363
		// (get) Token: 0x06006339 RID: 25401 RVA: 0x002EC339 File Offset: 0x002EA539
		public AccelerationConstraints GroupConstraints
		{
			get
			{
				return this._groupConstraints;
			}
		}

		// Token: 0x1700110C RID: 4364
		// (get) Token: 0x0600633A RID: 25402 RVA: 0x002EC341 File Offset: 0x002EA541
		public SpaceCombatManager combatMgr
		{
			get
			{
				return GameControl.spaceCombat;
			}
		}

		// Token: 0x1700110D RID: 4365
		// (get) Token: 0x0600633B RID: 25403 RVA: 0x002EC348 File Offset: 0x002EA548
		public TISpaceCombatState combatState
		{
			get
			{
				return this.combatMgr.combatState;
			}
		}

		// Token: 0x1700110E RID: 4366
		// (get) Token: 0x0600633C RID: 25404 RVA: 0x002EC355 File Offset: 0x002EA555
		// (set) Token: 0x0600633D RID: 25405 RVA: 0x002EC35D File Offset: 0x002EA55D
		public bool debugHideUI { get; private set; }

		// Token: 0x0600633E RID: 25406 RVA: 0x002EC368 File Offset: 0x002EA568
		public override void Initialize()
		{
			base.Initialize();
			GeneralControlsController.ShutdownUIGlobalTargetingMode(GameControl.control.activePlayer);
			this.groupSelectedFriendlyShips = new List<CombatShipController>();
			this._groupConstraints = new AccelerationConstraints(-1f, -1f, -1f, -1f);
			this.leftHandCombatants = new Dictionary<CombatantController, FriendlyShipListItemController>();
			this.rightHandCombatants = new Dictionary<CombatantController, EnemyShipListItemController>();
			this.friendlyShipList.SetListSize<FriendlyShipListItemController>(0, false, false);
			this.enemyShipList.SetListSize<EnemyShipListItemController>(0, false, false);
			this.reinforcementReorderList.SetListSize<ReinforcementReorderListItemController>(0, false, false);
			this.battleLogCanvas.enabled = false;
			this.ShowReinforcementTimer(false, true);
			this.ShowReinforcementTimer(false, false);
			this.reinforcementReorderPanelHeaderText.SetText(Loc.T("UI.SpaceCombat.ReinforcementReorderPanelHeaderText"));
			this.weaponUIControllers = new Dictionary<ModuleDataEntry, ShipWeaponUIController>();
			this.selectedShipPanel.gameObject.SetActive(true);
			this.selectedShipPanel.enabled = false;
			this.batteryYBottom = this.selectedShipCurrentBatteryCharge.transform.localPosition.y;
			float y = this.selectedShipCurrentBatteryCapacity.transform.localPosition.y;
			this.batteryYRange = y - this.batteryYBottom;
			this.heatYBottom = this.selectedShipCurrentHeat.transform.localPosition.y;
			float y2 = this.selectedShipCurrentHeatCapacity.transform.localPosition.y;
			this.HeatYRange = y2 - this.heatYBottom;
			this.currentDeltaVCoverMaxWidth = this.currentDeltaVCoverImage.sizeDelta.x;
			this.masterDamageGridControllers = new Dictionary<Vector2Int, SpaceCombatDamageGridItemController>();
			int num = 0;
			int num2 = 0;
			foreach (object obj in this.masterDamageGridGroup.transform)
			{
				SpaceCombatDamageGridItemController component = ((Transform)obj).GetComponent<SpaceCombatDamageGridItemController>();
				Vector2Int vector2Int = new Vector2Int(num, num2);
				component.PreInitialize(vector2Int);
				this.masterDamageGridControllers.Add(vector2Int, component);
				num2++;
				if (num2 == 8)
				{
					num++;
					num2 = 0;
				}
			}
			this.fleetManeuverPanel.SetActive(false);
			this.shipManeuverPanel.SetActive(false);
			foreach (Button button in this.shipManeuverPanel.GetComponentsInChildren<Button>(true))
			{
				this.commandButtons.Add(button);
				this.commandTooltips.Add(button.gameObject.GetComponent<TooltipTrigger>());
			}
			foreach (Button button2 in this.fleetManeuverPanel.GetComponentsInChildren<Button>(true))
			{
				this.fleetCommandButtons.Add(button2);
				this.fleetCommandTooltips.Add(button2.gameObject.GetComponent<TooltipTrigger>());
			}
			this.shipManeuverButtonTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.SpecialManeuverPanel"));
			this.fleetManuverButtonTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.SpecialManeuverPanel"));
			this.fleetCommandTooltips[20].SetText("BodyText", Loc.T("UI.SpaceCombat.SpecialManeuverPanel"));
			this.deltaVTooltip.SetDelegate("BodyText", () => this.DeltaVTooltip());
			this.endBattleTooltip.SetDelegate("BodyText", () => this.EndBattleTooltip());
			this.autoResolveTooltip.SetDelegate("BodyText", () => this.AutoResolveTooltip());
			this.fleetCommandsTargetText.SetText(Loc.T("UI.SpaceCombat.CommandTarget.Fleet"));
			this.confirmFormationButtonText.SetText(Loc.T("UI.Notifications.Confirm"));
			this.commandIconCache = new Dictionary<string, SpaceCombatCanvasController.CommandIconCacheItem>();
			foreach (IShipCommand shipCommand in ShipCommandsManager.shipCommands)
			{
				Sprite sprite = GameControl.assetLoader.LoadAsset<Sprite>(shipCommand.GetCommandIconImagePath_Off());
				Sprite sprite2 = GameControl.assetLoader.LoadAsset<Sprite>(shipCommand.GetCommandIconImagePath_On());
				this.commandIconCache.Add(shipCommand.GetTemplate().dataName, new SpaceCombatCanvasController.CommandIconCacheItem(shipCommand is TIShipManeuverCommandTemplate, sprite2, sprite));
			}
			foreach (IFleetCommand fleetCommand in ShipCommandsManager.fleetCommands)
			{
				Sprite sprite3 = GameControl.assetLoader.LoadAsset<Sprite>(fleetCommand.GetCommandIconImagePath_Off());
				Sprite sprite4 = GameControl.assetLoader.LoadAsset<Sprite>(fleetCommand.GetCommandIconImagePath_On());
				this.commandIconCache.Add(fleetCommand.GetTemplate().dataName, new SpaceCombatCanvasController.CommandIconCacheItem(false, sprite4, sprite3));
			}
			this.groupMembershipList.enabled = false;
			this.shipCommandsDataDirty = false;
			this.fleetCommandsDataDirty = false;
		}

		// Token: 0x0600633F RID: 25407 RVA: 0x002EC830 File Offset: 0x002EAA30
		public override void Show()
		{
			base.Show();
			this.SetupShipLists(null);
			bool flag = true;
			CombatFleetController combatFleetController = this.leftHandFleetController;
			this.UpdateReinforcementPanel(flag, (combatFleetController != null) ? combatFleetController.reinforcements : null, GameControl.spaceCombat.GetAvailableReinforcementsCount(this.leftHandFaction));
			bool flag2 = false;
			CombatFleetController combatFleetController2 = this.rightHandFleetController;
			this.UpdateReinforcementPanel(flag2, (combatFleetController2 != null) ? combatFleetController2.reinforcements : null, GameControl.spaceCombat.GetAvailableReinforcementsCount(this.rightHandFaction));
			GameControl.eventManager.AddListener<CombatTargetedableStateSelected>(new EventManager.EventDelegate<CombatTargetedableStateSelected>(this.OnCombatTargetableStateSelected), null, null, false, false);
			GameControl.eventManager.AddListener<CombatTargetedableStateSwap>(new EventManager.EventDelegate<CombatTargetedableStateSwap>(this.OnCombatTargetableStateSwapped), null, null, true, false);
			GameControl.eventManager.AddListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipRemoved), null, null, false, false);
			GameControl.eventManager.AddListener<HabModuleDestroyedInCombat>(new EventManager.EventDelegate<HabModuleDestroyedInCombat>(this.OnHabModuleDestroyed), null, null, true, false);
			GameControl.eventManager.AddListener<FleetCommandExecuted>(new EventManager.EventDelegate<FleetCommandExecuted>(this.OnFleetCommandExecuted), null, null, true, false);
			GameControl.eventManager.AddListener<CompleteExtendRadiatorsEvent>(new EventManager.EventDelegate<CompleteExtendRadiatorsEvent>(this.OnRadiatorsExtended), null, null, true, false);
			GameControl.eventManager.AddListener<CompleteRetractRadiatorsEvent>(new EventManager.EventDelegate<CompleteRetractRadiatorsEvent>(this.OnRadiatorsRetracted), null, null, true, false);
			GameControl.eventManager.AddListener<EndCombatStanceChanged>(new EventManager.EventDelegate<EndCombatStanceChanged>(this.SetEndBattleToggleSprite), null, null, false, false);
			GameControl.eventManager.AddListener<CombatEndTriggered>(new EventManager.EventDelegate<CombatEndTriggered>(this.OnCombatEndTriggered), null, null, false, false);
			GameControl.eventManager.AddListener<ShipSelectedDuringFormationSetting>(new EventManager.EventDelegate<ShipSelectedDuringFormationSetting>(this.OnShipSelectedDuringFormationSetting), null, null, false, false);
			GameControl.eventManager.AddListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null, null, true, false);
			this.playerReinforcementEntryText.text = "";
			this.aiReinforcementEntryText.text = "";
			this.SetEndBattleToggleSprite();
			this.UpdateFleetCommandPanel();
			this.UpdateCommandPanel(this.groupSelectedFriendlyShips.Count > 1);
			this.clockController.UpdateClockDisplay();
			this.battleLogController.Init();
			this.autoResolveButton.gameObject.SetActive(!GameControl.control.skirmishMode);
			TIInputManager.SetDefaultCursor(GameControl.control.skirmishMode);
			this.autoResolveConfirmationPanel.SetActive(false);
			this.ConfigureAutoResolvePanel();
			if (GameControl.spaceCombat.IsInFormationSelectionMode)
			{
				StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.SpaceCombat.ShipSwapping"));
				CombatFleetController combatFleetController3 = this.leftHandFleetController;
				if (combatFleetController3 != null && combatFleetController3.reinforcements.Count > 0)
				{
					stringBuilder.Append(Loc.T("UI.SpaceCombat.ReinforcementSwapping"));
				}
				this.shipSwapInstructionsText.SetText(stringBuilder.ToString());
				CombatFleetController combatFleetController4 = this.leftHandFleetController;
				bool flag3;
				if (combatFleetController4 == null)
				{
					flag3 = false;
				}
				else
				{
					TISpaceFleetState fleetState = combatFleetController4.fleetState;
					int? num;
					if (fleetState == null)
					{
						num = null;
					}
					else
					{
						List<TISpaceShipState> ships = fleetState.ships;
						num = ((ships != null) ? new int?(ships.Count) : null);
					}
					int? num2 = num;
					int num3 = 0;
					flag3 = (num2.GetValueOrDefault() > num3) & (num2 != null);
				}
				if (flag3)
				{
					this.OpenFormationUI();
					this.formationReinforcementSwapPanel.SetActive(true);
				}
				else
				{
					this.ConfirmFormation();
					this.formationReinforcementSwapPanel.SetActive(false);
				}
			}
			else
			{
				this.formationReinforcementSwapPanel.SetActive(false);
			}
			this.SetGroupSelectionButtons(false);
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x002ECB3C File Offset: 0x002EAD3C
		public override void Hide()
		{
			base.Hide();
			this.friendlyShipList.SetListSize<FriendlyShipListItemController>(0, false, false);
			this.enemyShipList.SetListSize<EnemyShipListItemController>(0, false, false);
			this.leftHandCombatants.Clear();
			this.rightHandCombatants.Clear();
			this.SetGroupSelectionButtons(true);
			this.OnCloseFriendlyShipView(true);
			GameControl.eventManager.RemoveListener<CombatTargetedableStateSelected>(new EventManager.EventDelegate<CombatTargetedableStateSelected>(this.OnCombatTargetableStateSelected), null);
			GameControl.eventManager.RemoveListener<CombatTargetedableStateSwap>(new EventManager.EventDelegate<CombatTargetedableStateSwap>(this.OnCombatTargetableStateSwapped), null);
			GameControl.eventManager.RemoveListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipRemoved), null);
			GameControl.eventManager.RemoveListener<FleetCommandExecuted>(new EventManager.EventDelegate<FleetCommandExecuted>(this.OnFleetCommandExecuted), null);
			GameControl.eventManager.RemoveListener<CompleteExtendRadiatorsEvent>(new EventManager.EventDelegate<CompleteExtendRadiatorsEvent>(this.OnRadiatorsExtended), null);
			GameControl.eventManager.RemoveListener<CompleteRetractRadiatorsEvent>(new EventManager.EventDelegate<CompleteRetractRadiatorsEvent>(this.OnRadiatorsRetracted), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyedInCombat>(new EventManager.EventDelegate<HabModuleDestroyedInCombat>(this.OnHabModuleDestroyed), null);
			GameControl.eventManager.RemoveListener<EndCombatStanceChanged>(new EventManager.EventDelegate<EndCombatStanceChanged>(this.SetEndBattleToggleSprite), null);
			GameControl.eventManager.RemoveListener<CombatEndTriggered>(new EventManager.EventDelegate<CombatEndTriggered>(this.OnCombatEndTriggered), null);
			GameControl.eventManager.RemoveListener<UIScaleSettingChange>(new EventManager.EventDelegate<UIScaleSettingChange>(this.OnUIScaleChanged), null);
		}

		// Token: 0x06006341 RID: 25409 RVA: 0x002ECC78 File Offset: 0x002EAE78
		public override void Refresh()
		{
			if (TIGlobalValuesState.isSpaceCombatEnabled && !GameControl.handlingException)
			{
				bool flag = this.groupSelectedFriendlyShips.Count > 1;
				if (this.shipCommandsDataDirty)
				{
					this.UpdateCommandPanel(flag);
					this.shipCommandsDataDirty = false;
				}
				if (this.fleetCommandsDataDirty)
				{
					if (flag)
					{
						this.UpdateCommandPanel(flag);
					}
					this.UpdateFleetCommandPanel();
					this.fleetCommandsDataDirty = false;
				}
				if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
				{
					this.control = true;
				}
				if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
				{
					this.control = false;
				}
				if (this.control && Input.GetKeyUp(KeyCode.M))
				{
					BusManager.SetVolume(BusManager.Music, 0f);
				}
				if (Input.GetKeyUp(KeyCode.Escape))
				{
					if (this.groupSelectedFriendlyShips.Count > 0)
					{
						this.ClearGroupSelect();
						this.OnCloseFriendlyShipButtonClicked();
					}
					else if (!TIGlobalValuesState.GlobalValues.tutorialMode || !TutorialTip.TipVisible)
					{
						this.ToggleMainMenu();
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.toggleFPSWidget, TIInputManager.KeyPressMode.Up))
				{
					this.ToggleFPSWidget();
				}
				this.UpdateReinforcementText();
			}
		}

		// Token: 0x06006342 RID: 25410 RVA: 0x002ECD94 File Offset: 0x002EAF94
		public void PostCombatCleanup()
		{
			this.leftHandFleetController = null;
			this.rightHandFleetController = null;
			foreach (IShipCommand shipCommand in ShipCommandsManager.shipCommands)
			{
				this.commandButtons[shipCommand.IconPosition()].onClick.RemoveAllListeners();
			}
			foreach (IFleetCommand fleetCommand in ShipCommandsManager.fleetCommands)
			{
				this.fleetCommandButtons[fleetCommand.IconPosition()].onClick.RemoveAllListeners();
			}
			GeneralControlsController.ShutdownUIGlobalTargetingMode(base.activePlayer);
			this.ClearGroupSelect();
			this.battleLogController.PostCombatCleanup();
		}

		// Token: 0x06006343 RID: 25411 RVA: 0x002ECE7C File Offset: 0x002EB07C
		public void ToggleMainMenu()
		{
			GameControl.eventManager.TriggerEvent(new CombatPauseMenuOpened(), null, Array.Empty<object>());
			if (base.canvasManager.OptionsScreen.Visible())
			{
				base.canvasManager.OptionsScreen.Hide();
				TIInputManager.acceptingInput = true;
				return;
			}
			base.canvasManager.OptionsScreen.Show();
			CodexController.HideCodexPanel();
			TIInputManager.acceptingInput = false;
		}

		// Token: 0x06006344 RID: 25412 RVA: 0x002ECEE4 File Offset: 0x002EB0E4
		public void ToggleFPSWidget()
		{
			TMP_FrameRateCounter component = base.gameObject.GetComponent<TMP_FrameRateCounter>();
			if (component != null)
			{
				if (component.enabled)
				{
					component.Clear();
				}
				component.enabled = !component.enabled;
			}
		}

		// Token: 0x06006345 RID: 25413 RVA: 0x002ECF23 File Offset: 0x002EB123
		public void ToggleDebugHideUI()
		{
			this.debugHideUI = !this.debugHideUI;
		}

		// Token: 0x06006346 RID: 25414 RVA: 0x002ECF34 File Offset: 0x002EB134
		private void SetupShipLists(List<CombatShipController> shipsToHighlight)
		{
			if (this.combatState.factions.Contains(base.activePlayer))
			{
				this.leftHandFaction = base.activePlayer;
			}
			else
			{
				this.leftHandFaction = this.combatState.factions[0];
			}
			this.rightHandFaction = this.combatState.factions.First<TIFactionState>((TIFactionState x) => x != this.leftHandFaction);
			List<CombatFleetController> list = new List<CombatFleetController>(this.combatMgr.fleetControllers);
			int num = 0;
			List<CombatantController> list2 = new List<CombatantController>();
			if (list.Any<CombatFleetController>((CombatFleetController x) => x.fleetState.faction == this.leftHandFaction))
			{
				this.leftHandFleetController = this.combatMgr.fleetControllers.Single<CombatFleetController>((CombatFleetController x) => x.fleetState.faction == this.leftHandFaction);
				this.leftHandFleetController.activeShipControllers = (from x in this.leftHandFleetController.activeShipControllers
					orderby x.ShipState.hull.length_m descending, x.ShipState.dryMass_kg descending
					select x).ToList<CombatShipController>();
				list2.AddRange(this.leftHandFleetController.activeShipControllers);
				list.Remove(this.leftHandFleetController);
				num += this.leftHandFleetController.activeShipControllers.Count;
			}
			int num2 = 0;
			List<CombatantController> list3 = new List<CombatantController>();
			if (list.Any<CombatFleetController>((CombatFleetController x) => x.fleetState.faction == this.rightHandFaction))
			{
				this.rightHandFleetController = this.combatMgr.fleetControllers.Single<CombatFleetController>((CombatFleetController x) => x.fleetState.faction == this.rightHandFaction);
				this.rightHandFleetController.activeShipControllers = (from x in this.rightHandFleetController.activeShipControllers
					orderby x.ShipState.hull.length_m descending, x.ShipState.dryMass_kg descending
					select x).ToList<CombatShipController>();
				list3.AddRange(this.rightHandFleetController.activeShipControllers);
				list.Remove(this.rightHandFleetController);
				num2 += this.rightHandFleetController.activeShipControllers.Count;
			}
			if (this.combatState.hab != null)
			{
				this.combatState.hab.ActiveCombatModules();
				if (this.combatState.hab.faction == this.leftHandFaction)
				{
					num += this.combatMgr.combatHabModuleControllers.Count;
					list2.AddRange(this.combatMgr.combatHabModuleControllers);
				}
				else
				{
					num2 += this.combatMgr.combatHabModuleControllers.Count;
					list3.AddRange(this.combatMgr.combatHabModuleControllers);
				}
			}
			List<CombatantController> list4 = new List<CombatantController>();
			for (int i = 0; i < list2.Count; i++)
			{
				if (!list2[i].isDestroyed)
				{
					list4.Add(list2[i]);
				}
				else
				{
					num--;
				}
			}
			int num3 = 0;
			this.friendlyShipList.SetListSize<FriendlyShipListItemController>(num, false, false);
			using (IEnumerator<object> enumerator = this.friendlyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__168.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__168.<>p__0 = CallSite<Func<CallSite, object, FriendlyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FriendlyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					FriendlyShipListItemController friendlyShipListItemController = SpaceCombatCanvasController.<>o__168.<>p__0.Target(SpaceCombatCanvasController.<>o__168.<>p__0, enumerator.Current);
					friendlyShipListItemController.Init(this, list4[num3], num3);
					this.leftHandCombatants.Add(list4[num3], friendlyShipListItemController);
					list4[num3].UIController().InitializeForCombat(list4[num3], friendlyShipListItemController);
					if (!list4[num3].isDestroyed)
					{
						friendlyShipListItemController.gameObject.SetActive(true);
						if (shipsToHighlight != null && shipsToHighlight.Contains(list4[num3].ref_shipController))
						{
							friendlyShipListItemController.highlightObject.SetActive(true);
						}
					}
					num3++;
				}
			}
			this.friendlyShipListTransform.sizeDelta = new Vector2(this.friendlyShipListTransform.sizeDelta.x, Mathf.Min(base.Canvas.GetComponent<RectTransform>().sizeDelta.y, 106f * (float)num3 + 2f));
			list4 = new List<CombatantController>();
			for (int j = 0; j < list3.Count; j++)
			{
				if (!list3[j].isDestroyed)
				{
					list4.Add(list3[j]);
				}
				else
				{
					num2--;
				}
			}
			num3 = 0;
			this.enemyShipList.SetListSize<EnemyShipListItemController>(num2, false, false);
			using (IEnumerator<object> enumerator = this.enemyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__168.<>p__1 == null)
					{
						SpaceCombatCanvasController.<>o__168.<>p__1 = CallSite<Func<CallSite, object, EnemyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EnemyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					EnemyShipListItemController enemyShipListItemController = SpaceCombatCanvasController.<>o__168.<>p__1.Target(SpaceCombatCanvasController.<>o__168.<>p__1, enumerator.Current);
					enemyShipListItemController.Init(this, list4[num3], num3);
					this.rightHandCombatants.Add(list4[num3], enemyShipListItemController);
					list4[num3].UIController().InitializeForCombat(list4[num3], enemyShipListItemController);
					if (!list4[num3].isDestroyed)
					{
						enemyShipListItemController.gameObject.SetActive(true);
						if (shipsToHighlight != null && shipsToHighlight.Contains(list4[num3].ref_shipController))
						{
							enemyShipListItemController.highlightObject.SetActive(true);
						}
					}
					num3++;
				}
			}
			this.enemyShipListTransform.sizeDelta = new Vector2(this.enemyShipListTransform.sizeDelta.x, Mathf.Min(base.Canvas.GetComponent<RectTransform>().sizeDelta.y, 108f * (float)num3 + 2f));
		}

		// Token: 0x06006347 RID: 25415 RVA: 0x002ED544 File Offset: 0x002EB744
		private void OnHabModuleDestroyed(HabModuleDestroyedInCombat e)
		{
			this.OnCombatantRemoved(e.habModule);
		}

		// Token: 0x06006348 RID: 25416 RVA: 0x002ED554 File Offset: 0x002EB754
		private void OnShipRemoved(ShipDestroyed e)
		{
			if (e.ship == this.selectedFriendlyShipState)
			{
				this.OnCloseFriendlyShipView(false);
			}
			CombatShipController combatShipController = this.groupSelectedFriendlyShips.Where<CombatShipController>((CombatShipController o) => o.ShipState == e.ship).FirstOrDefault<CombatShipController>();
			if (combatShipController != null)
			{
				this.groupSelectedFriendlyShips.Remove(combatShipController);
			}
			this.shipCommandsDataDirty = true;
			this.fleetCommandsDataDirty = true;
			this.OnCombatantRemoved(e.ship);
			bool flag = true;
			CombatFleetController combatFleetController = this.leftHandFleetController;
			this.UpdateReinforcementPanel(flag, (combatFleetController != null) ? combatFleetController.reinforcements : null, GameControl.spaceCombat.GetAvailableReinforcementsCount(this.leftHandFaction));
			bool flag2 = false;
			CombatFleetController combatFleetController2 = this.rightHandFleetController;
			this.UpdateReinforcementPanel(flag2, (combatFleetController2 != null) ? combatFleetController2.reinforcements : null, GameControl.spaceCombat.GetAvailableReinforcementsCount(this.rightHandFaction));
		}

		// Token: 0x06006349 RID: 25417 RVA: 0x002ED634 File Offset: 0x002EB834
		private void OnCombatantRemoved(CombatTargetableState combatantState)
		{
			CombatantController combatantController = this.combatMgr.combatantLookup[combatantState];
			if (this.leftHandCombatants.ContainsKey(combatantController))
			{
				this.leftHandCombatants[combatantController].gameObject.SetActive(false);
				int num = 0;
				foreach (FriendlyShipListItemController friendlyShipListItemController in this.leftHandCombatants.Values)
				{
					if (friendlyShipListItemController != null && friendlyShipListItemController.gameObject != null && friendlyShipListItemController.gameObject.activeSelf)
					{
						num++;
					}
				}
				this.friendlyShipListTransform.sizeDelta = new Vector2(this.friendlyShipListTransform.sizeDelta.x, Mathf.Min(base.Canvas.GetComponent<RectTransform>().sizeDelta.y, 106f * (float)num + 2f));
			}
			else if (this.rightHandCombatants.ContainsKey(combatantController))
			{
				this.rightHandCombatants[combatantController].gameObject.SetActive(false);
				int num2 = 0;
				foreach (EnemyShipListItemController enemyShipListItemController in this.rightHandCombatants.Values)
				{
					if (enemyShipListItemController != null && enemyShipListItemController.gameObject != null && enemyShipListItemController.gameObject.activeSelf)
					{
						num2++;
					}
				}
				this.enemyShipListTransform.sizeDelta = new Vector2(this.enemyShipListTransform.sizeDelta.x, Mathf.Min(base.Canvas.GetComponent<RectTransform>().sizeDelta.y, 108f * (float)num2 + 2f));
			}
			if (this.groupSelectedFriendlyShips.Count > 0 && this.groupSelectedFriendlyShips.Contains(combatantController))
			{
				this.RemoveShipFromGroupSelect(combatantController);
			}
		}

		// Token: 0x0600634A RID: 25418 RVA: 0x002ED834 File Offset: 0x002EBA34
		private void OnFleetCommandExecuted(FleetCommandExecuted e)
		{
			this.fleetCommandsDataDirty = true;
		}

		// Token: 0x0600634B RID: 25419 RVA: 0x002ED83D File Offset: 0x002EBA3D
		private void OnUIScaleChanged(UIScaleSettingChange e)
		{
			base.StartCoroutine(this.RefreshUIScale());
		}

		// Token: 0x0600634C RID: 25420 RVA: 0x002ED84C File Offset: 0x002EBA4C
		private IEnumerator RefreshUIScale()
		{
			yield return null;
			this.friendlyShipListTransform.sizeDelta = new Vector2(this.friendlyShipListTransform.sizeDelta.x, Mathf.Min(base.Canvas.GetComponent<RectTransform>().sizeDelta.y, 106f * (float)this.friendlyShipList.size + 2f));
			this.enemyShipListTransform.sizeDelta = new Vector2(this.enemyShipListTransform.sizeDelta.x, Mathf.Min(base.Canvas.GetComponent<RectTransform>().sizeDelta.y, 108f * (float)this.enemyShipList.size + 2f));
			yield break;
		}

		// Token: 0x0600634D RID: 25421 RVA: 0x002ED85B File Offset: 0x002EBA5B
		private IEnumerator ShowMainTutorialDelayed()
		{
			yield return null;
			this.ShowMainTutorial();
			yield break;
		}

		// Token: 0x0600634E RID: 25422 RVA: 0x002ED86A File Offset: 0x002EBA6A
		private void ShowMainTutorial()
		{
			this.waypointTutorialController.ShowTutorialTips(CampaignMilestone.UITutorial_SpaceCombatCanvas_Waypoints, false, true);
		}

		// Token: 0x0600634F RID: 25423 RVA: 0x002ED87E File Offset: 0x002EBA7E
		private void ShowShipSelectedTutorial()
		{
			if (TIGlobalValuesState.GlobalValues.tutorialMode && !TutorialTip.TipVisible)
			{
				this.spaceCombat_ShipSelectedUITutorialController.ShowTutorialTips(CampaignMilestone.UITutorial_SpaceCombatCanvas_FriendlyShipDetail, false, true);
			}
		}

		// Token: 0x06006350 RID: 25424 RVA: 0x002ED8A5 File Offset: 0x002EBAA5
		private void ShowFormationTutorial()
		{
			this.formationTutorialController.ShowTutorialTips(CampaignMilestone.UITutorial_SpaceCombatCanvas_Formations, false, true);
		}

		// Token: 0x06006351 RID: 25425 RVA: 0x002ED8BC File Offset: 0x002EBABC
		private void UpdateReinforcementText()
		{
			if (this.playerReinforcementEntryTextTimer >= 0f)
			{
				this.playerReinforcementEntryTextTimer -= Time.deltaTime;
				if (this.playerReinforcementEntryTextTimer <= 0f)
				{
					this.playerReinforcementEntryText.SetText("");
				}
			}
			if (this.aiReinforcementEntryTextTimer >= 0f)
			{
				this.aiReinforcementEntryTextTimer -= Time.deltaTime;
				if (this.aiReinforcementEntryTextTimer <= 0f)
				{
					this.aiReinforcementEntryText.SetText("");
				}
			}
		}

		// Token: 0x06006352 RID: 25426 RVA: 0x002ED944 File Offset: 0x002EBB44
		public void UpdateReinforcmentTimerText(float value, bool isPlayer)
		{
			if (isPlayer)
			{
				if (!this.playerReinforcementTimerText.gameObject.activeInHierarchy)
				{
					this.playerReinforcementTimerText.gameObject.SetActive(true);
				}
				this.playerReinforcementTimerText.SetText(Loc.T("UI.SpaceCombat.ReinforcementsTimer", new object[] { value.ToString() }));
				return;
			}
			if (!this.aiReinforcementTimerText.gameObject.activeInHierarchy)
			{
				this.aiReinforcementTimerText.gameObject.SetActive(true);
			}
			this.aiReinforcementTimerText.SetText(Loc.T("UI.SpaceCombat.ReinforcementsTimer", new object[] { value.ToString() }));
		}

		// Token: 0x06006353 RID: 25427 RVA: 0x002ED9E5 File Offset: 0x002EBBE5
		public void ShowReinforcementTimer(bool value, bool isPlayer)
		{
			if (isPlayer)
			{
				this.playerReinforcementTimerText.gameObject.SetActive(value);
				return;
			}
			this.aiReinforcementTimerText.gameObject.SetActive(value);
		}

		// Token: 0x06006354 RID: 25428 RVA: 0x002EDA10 File Offset: 0x002EBC10
		public void UpdateReinforcementUI(TIFactionState faction, CombatFleetController controller, List<CombatShipController> ships)
		{
			if (faction == base.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_IncomingComms", false, false);
				this.playerReinforcementEntryText.SetText(new StringBuilder(ships[0].ShipState.hull.displayName).Append(" ").Append(ships[0].ShipState.GetDisplayName(faction)).Append(Loc.T("UI.SpaceCombat.ReinforceEntry")));
				this.playerReinforcementEntryTextTimer = this.ReinforcementEntryTextTime;
			}
			else
			{
				this.aiReinforcementEntryText.SetText(new StringBuilder(ships[0].ShipState.hull.displayName).Append(" ").Append(ships[0].ShipState.GetDisplayName(faction)).Append(Loc.T("UI.SpaceCombat.ReinforceEntry")));
				this.aiReinforcementEntryTextTimer = this.ReinforcementEntryTextTime;
			}
			this.UpdateReinforcementPanel(faction == base.activePlayer, controller.reinforcements, this.combatMgr.GetAvailableReinforcementsCount(faction));
			this.UpdateCombatShipList(ships);
			if (faction == base.activePlayer)
			{
				this.SetReinforcementReorderList(false);
			}
		}

		// Token: 0x06006355 RID: 25429 RVA: 0x002EDB40 File Offset: 0x002EBD40
		private string UpdateReinforcementTooltip(IList<TISpaceShipState> reinforcingShips = null, int availableReinforcementCount = -1)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.Reinforcements")).AppendLine("");
			int num = 0;
			for (;;)
			{
				int num2 = num;
				int? num3 = ((reinforcingShips != null) ? new int?(reinforcingShips.Count) : null);
				if (!((num2 < num3.GetValueOrDefault()) & (num3 != null)))
				{
					break;
				}
				if (num < availableReinforcementCount)
				{
					stringBuilder.Append(Loc.T("UI.SpaceCombat.ReinforcementReady"));
				}
				stringBuilder.AppendLine(new StringBuilder(reinforcingShips[num].hull.displayName).Append(" ").Append(reinforcingShips[num].GetDisplayName(base.activePlayer)).ToString());
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06006356 RID: 25430 RVA: 0x002EDC08 File Offset: 0x002EBE08
		private void UpdateReinforcementPanel(bool isPlayer, IList<TISpaceShipState> reinforcingShips = null, int availableReinforcementCount = -1)
		{
			if (isPlayer)
			{
				if (reinforcingShips == null || reinforcingShips.Count == 0)
				{
					this.playerReinforcmentButton.gameObject.SetActive(false);
					this.playerReinforcmentButton.interactable = false;
					this.CloseReinforcementReorderPanel();
					return;
				}
				if (availableReinforcementCount > 0)
				{
					this.playerReinforcmentButton.interactable = true;
				}
				else
				{
					this.playerReinforcmentButton.interactable = false;
				}
				this.playerReinforcmentButton.gameObject.SetActive(true);
				this.playerReinforcementTooltip.SetDelegate("BodyText", () => this.UpdateReinforcementTooltip(reinforcingShips, availableReinforcementCount));
				this.playerReinforcementTotal.SetText(reinforcingShips.Count.ToString());
				this.playerReinforcementReadyCount.SetText(availableReinforcementCount.ToString());
				return;
			}
			else
			{
				if (reinforcingShips == null || reinforcingShips.Count == 0)
				{
					this.enemyReinforcmentButtonGO.SetActive(false);
					return;
				}
				this.enemyReinforcmentButtonGO.SetActive(true);
				this.enemyReinforcementTooltip.SetDelegate("BodyText", () => this.UpdateReinforcementTooltip(reinforcingShips, availableReinforcementCount));
				this.enemyReinforcementQty.SetText(reinforcingShips.Count.ToString());
				return;
			}
		}

		// Token: 0x06006357 RID: 25431 RVA: 0x002EDD5C File Offset: 0x002EBF5C
		private void UpdateCombatShipList(List<CombatShipController> shipsToHighlight)
		{
			this.friendlyShipList.SetListSize<FriendlyShipListItemController>(0, false, false);
			this.enemyShipList.SetListSize<EnemyShipListItemController>(0, false, false);
			this.leftHandCombatants.Clear();
			this.rightHandCombatants.Clear();
			this.SetupShipLists(shipsToHighlight);
			if (this.selectedFriendlyShip != null)
			{
				using (IEnumerator<object> enumerator = this.enemyShipList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (SpaceCombatCanvasController.<>o__185.<>p__0 == null)
						{
							SpaceCombatCanvasController.<>o__185.<>p__0 = CallSite<Func<CallSite, object, EnemyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EnemyShipListItemController), typeof(SpaceCombatCanvasController)));
						}
						SpaceCombatCanvasController.<>o__185.<>p__0.Target(SpaceCombatCanvasController.<>o__185.<>p__0, enumerator.Current).OnPlayerShipSelected();
					}
				}
			}
		}

		// Token: 0x06006358 RID: 25432 RVA: 0x002EDE30 File Offset: 0x002EC030
		public void OnReinforcementButtonPressed()
		{
			this.combatMgr.SendInPlayerReinforcements();
			this.playerReinforcmentButton.interactable = false;
		}

		// Token: 0x06006359 RID: 25433 RVA: 0x002EDE49 File Offset: 0x002EC049
		public void OnPressOpenReorderReinforcementButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.OpenReinforcementReorderPanel();
		}

		// Token: 0x0600635A RID: 25434 RVA: 0x002EDE5D File Offset: 0x002EC05D
		private void OpenReinforcementReorderPanel()
		{
			this.openReinforcementsReorderPanelButtonObject.SetActive(false);
			this.closeReinforcementsReorderPanelButtonObject.SetActive(true);
			this.reinforcementReorderPanel.SetActive(this.leftHandFleetController.reinforcements.Count > 0);
		}

		// Token: 0x0600635B RID: 25435 RVA: 0x002EDE95 File Offset: 0x002EC095
		public void OnPressCloseReorderReinforcementButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseReinforcementReorderPanel();
		}

		// Token: 0x0600635C RID: 25436 RVA: 0x002EDEA9 File Offset: 0x002EC0A9
		private void CloseReinforcementReorderPanel()
		{
			GameObject gameObject = this.openReinforcementsReorderPanelButtonObject;
			CombatFleetController combatFleetController = this.leftHandFleetController;
			gameObject.SetActive(combatFleetController != null && combatFleetController.reinforcements.Count > 0);
			this.closeReinforcementsReorderPanelButtonObject.SetActive(false);
			this.reinforcementReorderPanel.SetActive(false);
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x002EDEE8 File Offset: 0x002EC0E8
		public void SetReinforcementReorderList(bool startup)
		{
			this.reinforcementListItems = new Dictionary<TISpaceShipState, ReinforcementReorderListItemController>();
			if (this.leftHandFleetController != null)
			{
				this.reinforcementReorderList.SetListSize<ReinforcementReorderListItemController>(this.leftHandFleetController.reinforcements.Count, false, false);
			}
			else
			{
				this.reinforcementReorderList.SetListSize<ReinforcementReorderListItemController>(0, false, false);
			}
			int num = 0;
			using (IEnumerator<object> enumerator = this.reinforcementReorderList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__192.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__192.<>p__0 = CallSite<Func<CallSite, object, ReinforcementReorderListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ReinforcementReorderListItemController), typeof(SpaceCombatCanvasController)));
					}
					ReinforcementReorderListItemController reinforcementReorderListItemController = SpaceCombatCanvasController.<>o__192.<>p__0.Target(SpaceCombatCanvasController.<>o__192.<>p__0, enumerator.Current);
					reinforcementReorderListItemController.SetListItem(this.leftHandFleetController.reinforcements[num], this);
					this.reinforcementListItems.Add(this.leftHandFleetController.reinforcements[num], reinforcementReorderListItemController);
					num++;
				}
			}
			if (startup)
			{
				this.CloseReinforcementReorderPanel();
			}
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x002EDFF8 File Offset: 0x002EC1F8
		public void RepositionShipInReinforcements(TISpaceShipState ship, int order)
		{
			if (order >= 9999)
			{
				this.leftHandFleetController.reinforcements.Remove(ship);
				this.leftHandFleetController.reinforcements.Add(ship);
				this.reinforcementListItems[ship].transform.SetAsLastSibling();
			}
			else if (order <= -9999)
			{
				this.leftHandFleetController.reinforcements.Remove(ship);
				this.leftHandFleetController.reinforcements.Insert(0, ship);
				this.reinforcementListItems[ship].transform.SetAsFirstSibling();
			}
			else
			{
				int num = Mathf.Clamp(Mathf.Max(0, this.leftHandFleetController.reinforcements.IndexOf(ship)) + order, 0, this.leftHandFleetController.reinforcements.Count);
				this.leftHandFleetController.reinforcements.Remove(ship);
				this.leftHandFleetController.reinforcements.Insert(num, ship);
				this.reinforcementListItems[ship].transform.SetSiblingIndex(num);
			}
			using (IEnumerator<object> enumerator = this.reinforcementReorderList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__193.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__193.<>p__0 = CallSite<Func<CallSite, object, ReinforcementReorderListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ReinforcementReorderListItemController), typeof(SpaceCombatCanvasController)));
					}
					SpaceCombatCanvasController.<>o__193.<>p__0.Target(SpaceCombatCanvasController.<>o__193.<>p__0, enumerator.Current).SetReorderButtonsInteractable();
				}
			}
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x002EE178 File Offset: 0x002EC378
		private void OnCombatTargetableStateSelected(CombatTargetedableStateSelected e)
		{
			if (e.target != null && TIInputManager.acceptingInput)
			{
				if (GeneralControlsController.UIPlayerInTargetingMode)
				{
					return;
				}
				TISpaceShipState tispaceShipState = e.target.GetTargetableState() as TISpaceShipState;
				if (((tispaceShipState != null) ? tispaceShipState.faction : null) == base.activePlayer)
				{
					if (TIInputManager.IsControlKeyDown)
					{
						this.DeselectShip(this.combatMgr.combatantLookup[tispaceShipState]);
						return;
					}
					if (TIInputManager.IsAltKeyDown)
					{
						this.SelectAllShipsOfClass(tispaceShipState, !e.boxSelected);
						return;
					}
					this.NewFriendlyShipSelected(this.combatMgr.combatantLookup[tispaceShipState], true, e.boxSelected, e.isGroupSelectPrimarySelection);
				}
			}
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x002EE228 File Offset: 0x002EC428
		private void OnCombatTargetableStateSwapped(CombatTargetedableStateSwap e)
		{
			if (e.target != null && TIInputManager.acceptingInput)
			{
				TISpaceShipState tispaceShipState = e.target.GetTargetableState() as TISpaceShipState;
				if (((tispaceShipState != null) ? tispaceShipState.faction : null) == base.activePlayer)
				{
					this.NewFriendlyShipSelected(this.combatMgr.combatantLookup[tispaceShipState], false, false, false);
				}
			}
		}

		// Token: 0x06006361 RID: 25441 RVA: 0x002EE288 File Offset: 0x002EC488
		public void DeselectShip(CombatantController combatantController)
		{
			CombatShipController ref_shipController = combatantController.ref_shipController;
			if (!this.combatMgr.waypointsVisible && this.selectedFriendlyShip != null && combatantController == this.selectedFriendlyShip)
			{
				this.selectedFriendlyShip.SetWaypointVisualization(false);
			}
			if (this.groupSelectedFriendlyShips.Contains(ref_shipController))
			{
				this.RemoveShipFromGroupSelect(ref_shipController);
				if (this.selectedFriendlyShip == combatantController && this.groupSelectedFriendlyShips.Count > 0)
				{
					this.ClearShipUI(ref_shipController);
					this.SelectPrimaryShip(this.groupSelectedFriendlyShips[0]);
					return;
				}
				if (this.groupSelectedFriendlyShips.Count == 0)
				{
					this.OnCloseFriendlyShipButtonClicked();
					this.ClearGroupSelect();
					return;
				}
			}
			else if (this.selectedFriendlyShip == combatantController)
			{
				this.OnCloseFriendlyShipButtonClicked();
			}
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x002EE34C File Offset: 0x002EC54C
		private void NewFriendlyShipSelected(CombatantController combatantController, bool openPanel = true, bool isAddingFromBoxSelect = false, bool isGroupSelectPrimarySelection = false)
		{
			this.isFleetCommandCardPanelOpen = false;
			CombatShipController ref_shipController = combatantController.ref_shipController;
			CombatShipController combatShipController = this.selectedFriendlyShip;
			if (!isAddingFromBoxSelect || (isAddingFromBoxSelect && isGroupSelectPrimarySelection))
			{
				if (this.selectedFriendlyShip != null)
				{
					this.ClearShipUI(this.selectedFriendlyShip);
				}
				if (ref_shipController != null && !ref_shipController.isDestroyed)
				{
					this.SelectPrimaryShip(ref_shipController);
				}
			}
			this.HandleGroupSelect(combatantController, combatShipController, isAddingFromBoxSelect);
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x002EE3B4 File Offset: 0x002EC5B4
		private void SelectAllShipsOfClass(TISpaceShipState selectedShipState, bool clearPrevGroup = true)
		{
			if (clearPrevGroup)
			{
				this.ClearGroupSelect();
			}
			List<CombatShipController> list = new List<CombatShipController>();
			foreach (CombatShipController combatShipController in this.leftHandFleetController.activeShipControllers)
			{
				if (selectedShipState.template.fullClassName.Equals(combatShipController.ShipState.template.fullClassName))
				{
					list.Add(combatShipController);
					if (combatShipController.ShipState == selectedShipState)
					{
						this.NewFriendlyShipSelected(combatShipController, true, true, true);
					}
					else
					{
						this.NewFriendlyShipSelected(combatShipController, true, true, false);
					}
				}
			}
		}

		// Token: 0x06006364 RID: 25444 RVA: 0x002EE45C File Offset: 0x002EC65C
		public void SelectPrimaryShip(CombatShipController shipController)
		{
			this.selectedFriendlyShip = shipController;
			this.selectedFriendlyShipState = shipController.ShipState;
			this.selectedFriendlyShip.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
			this.selectedFriendlyShip.UIController().maintainAnimation = true;
			this.selectedFriendlyShip.ModelController.StartSelectionAnimation();
			using (IEnumerator<object> enumerator = this.friendlyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__199.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__199.<>p__0 = CallSite<Func<CallSite, object, FriendlyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FriendlyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					FriendlyShipListItemController friendlyShipListItemController = SpaceCombatCanvasController.<>o__199.<>p__0.Target(SpaceCombatCanvasController.<>o__199.<>p__0, enumerator.Current);
					if (friendlyShipListItemController.combatantController.GetCombatantType() == IDamageableType.Ship && friendlyShipListItemController.combatantController.ref_shipController.ShipState.ID == this.selectedFriendlyShip.ShipState.ID)
					{
						friendlyShipListItemController.SetPrimarySelected(true);
						break;
					}
				}
			}
			using (IEnumerator<object> enumerator = this.enemyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__199.<>p__1 == null)
					{
						SpaceCombatCanvasController.<>o__199.<>p__1 = CallSite<Func<CallSite, object, EnemyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EnemyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					SpaceCombatCanvasController.<>o__199.<>p__1.Target(SpaceCombatCanvasController.<>o__199.<>p__1, enumerator.Current).OnPlayerShipSelected();
				}
			}
			if (this.selectedFriendlyShip.primaryTarget != null && this.selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.Ship)
			{
				this.selectedFriendlyShip.primaryTarget.ref_shipController.ModelController.StartSelectionAnimation();
				this.selectedFriendlyShip.primaryTarget.UIController().maintainAnimation = true;
				(this.selectedFriendlyShip.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).OnPrimaryTargetSelected();
			}
			if (this.selectedFriendlyShip.primaryTarget != null && this.selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.StationModule)
			{
				(this.selectedFriendlyShip.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).OnPrimaryTargetSelected();
			}
			if (this.selectedFriendlyShip.maneuverTarget != null)
			{
				this.selectedFriendlyShip.maneuverTarget.UIController().combatantListItemController.OnManeuverTargetSelected();
			}
			this.SetSelectedShipPanel();
			if (!this.selectedShipPanel.enabled)
			{
				this.selectedShipPanel.enabled = true;
			}
			this.selectedFriendlyShip.SetWaypointVisualization(true);
		}

		// Token: 0x06006365 RID: 25445 RVA: 0x002EE6FC File Offset: 0x002EC8FC
		public void OnCloseFriendlyShipButtonClicked()
		{
			this.isFleetCommandCardPanelOpen = true;
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.OnCloseFriendlyShipView(false);
		}

		// Token: 0x06006366 RID: 25446 RVA: 0x002EE718 File Offset: 0x002EC918
		public void ClearShipUI(CombatShipController shipController)
		{
			if (shipController == null)
			{
				return;
			}
			shipController.ModelController.StopSelectionAnimation();
			shipController.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
			shipController.UIController().maintainAnimation = false;
			if (shipController.visualizationController.UIController != null)
			{
				shipController.visualizationController.UIController.DisableWeaponRangeVisualizations();
			}
			using (IEnumerator<object> enumerator = this.friendlyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__201.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__201.<>p__0 = CallSite<Func<CallSite, object, FriendlyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FriendlyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					FriendlyShipListItemController friendlyShipListItemController = SpaceCombatCanvasController.<>o__201.<>p__0.Target(SpaceCombatCanvasController.<>o__201.<>p__0, enumerator.Current);
					if (friendlyShipListItemController.combatantController.GetCombatantType() == IDamageableType.Ship && friendlyShipListItemController.combatantController.ref_shipController.ShipState.ID == shipController.ShipState.ID)
					{
						friendlyShipListItemController.SetPrimarySelected(false);
					}
				}
			}
			using (IEnumerator<object> enumerator = this.enemyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__201.<>p__1 == null)
					{
						SpaceCombatCanvasController.<>o__201.<>p__1 = CallSite<Func<CallSite, object, EnemyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EnemyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					SpaceCombatCanvasController.<>o__201.<>p__1.Target(SpaceCombatCanvasController.<>o__201.<>p__1, enumerator.Current).OnShipSelectionCleared();
				}
			}
			if (shipController.primaryTarget != null)
			{
				if (shipController.primaryTarget.GetCombatantType() == IDamageableType.Ship)
				{
					shipController.primaryTarget.ref_shipController.ModelController.StopSelectionAnimation();
					shipController.primaryTarget.UIController().maintainAnimation = false;
					(shipController.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).ClearPrimaryTarget();
				}
				else if (shipController.primaryTarget.GetCombatantType() == IDamageableType.StationModule)
				{
					(shipController.primaryTarget.UIController().combatantListItemController as EnemyShipListItemController).ClearPrimaryTarget();
				}
			}
			if (shipController.maneuverTarget != null)
			{
				shipController.maneuverTarget.UIController().combatantListItemController.ClearManeuverTarget();
			}
			if (!this.combatMgr.waypointsVisible)
			{
				shipController.SetWaypointVisualization(this.groupSelectedFriendlyShips.Count > 0 && this.groupSelectedFriendlyShips.Contains(shipController));
			}
		}

		// Token: 0x06006367 RID: 25447 RVA: 0x002EE984 File Offset: 0x002ECB84
		public void OnCloseFriendlyShipView(bool skipTutorial = false)
		{
			if (this.selectedFriendlyShip != null)
			{
				this.ClearShipUI(this.selectedFriendlyShip);
				this.selectedFriendlyShip = null;
				this.selectedFriendlyShipState = null;
			}
			this.RemoveFriendlyShipListeners();
			this.selectedShipPanel.enabled = false;
			if (!skipTutorial)
			{
				this.ShowMainTutorial();
			}
		}

		// Token: 0x06006368 RID: 25448 RVA: 0x002EE9D4 File Offset: 0x002ECBD4
		private void HandleGroupSelect(CombatantController combatantController, CombatantController previousSelectedShip, bool isAddingFromBoxSelect)
		{
			if (isAddingFromBoxSelect)
			{
				this.groupSelectedFriendlyShips.AddUnique(combatantController.ref_shipController);
				this.UpdateGroupSelectUI();
			}
			else if (TIInputManager.IsShiftKeyDown && !this.groupSelectedFriendlyShips.Contains(combatantController.ref_shipController))
			{
				if (previousSelectedShip != null)
				{
					this.groupSelectedFriendlyShips.AddUnique(previousSelectedShip.ref_shipController);
				}
				this.groupSelectedFriendlyShips.AddUnique(combatantController.ref_shipController);
				this.UpdateGroupSelectUI();
			}
			else if (!TIInputManager.IsShiftKeyDown && this.groupSelectedFriendlyShips.Count > 0)
			{
				this.ClearGroupSelect();
			}
			this.UpdateCommandPanel(this.groupSelectedFriendlyShips.Count > 1);
		}

		// Token: 0x06006369 RID: 25449 RVA: 0x002EEA7C File Offset: 0x002ECC7C
		public void UpdateGroupSelectUI()
		{
			foreach (CombatShipController combatShipController in this.groupSelectedFriendlyShips)
			{
				combatShipController.ModelController.StartGroupSelectionAnimation();
				using (IEnumerator<object> enumerator2 = this.friendlyShipList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (SpaceCombatCanvasController.<>o__204.<>p__0 == null)
						{
							SpaceCombatCanvasController.<>o__204.<>p__0 = CallSite<Func<CallSite, object, FriendlyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FriendlyShipListItemController), typeof(SpaceCombatCanvasController)));
						}
						FriendlyShipListItemController friendlyShipListItemController = SpaceCombatCanvasController.<>o__204.<>p__0.Target(SpaceCombatCanvasController.<>o__204.<>p__0, enumerator2.Current);
						if (friendlyShipListItemController.combatantController.GetCombatantType() == IDamageableType.Ship && friendlyShipListItemController.combatantController.ref_shipController.ShipState.ID == combatShipController.ShipState.ID)
						{
							friendlyShipListItemController.SetGroupSelected(true);
						}
					}
				}
				combatShipController.SetWaypointVisualization(true);
			}
			this.UpdateFleetCommandPanel();
			this.UpdateCommandPanel(this.groupSelectedFriendlyShips.Count > 1);
			this.UpdateGroupConstraints();
		}

		// Token: 0x0600636A RID: 25450 RVA: 0x002EEBB8 File Offset: 0x002ECDB8
		public void ClearGroupSelect()
		{
			foreach (CombatShipController combatShipController in this.groupSelectedFriendlyShips)
			{
				combatShipController.ModelController.StopGroupSelectionAnimation();
				combatShipController._waypointNavigationController.ClearWaypointGizmos();
				if (!this.combatMgr.waypointsVisible && this.selectedFriendlyShip != combatShipController)
				{
					combatShipController.SetWaypointVisualization(false);
				}
				using (IEnumerator<object> enumerator2 = this.friendlyShipList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (SpaceCombatCanvasController.<>o__205.<>p__0 == null)
						{
							SpaceCombatCanvasController.<>o__205.<>p__0 = CallSite<Func<CallSite, object, FriendlyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FriendlyShipListItemController), typeof(SpaceCombatCanvasController)));
						}
						FriendlyShipListItemController friendlyShipListItemController = SpaceCombatCanvasController.<>o__205.<>p__0.Target(SpaceCombatCanvasController.<>o__205.<>p__0, enumerator2.Current);
						if (friendlyShipListItemController.combatantController.GetCombatantType() == IDamageableType.Ship && friendlyShipListItemController.combatantController.ref_shipController.ShipState.ID == combatShipController.ShipState.ID)
						{
							friendlyShipListItemController.SetGroupSelected(false);
						}
					}
				}
			}
			this.groupSelectedFriendlyShips.Clear();
			this.UpdateFleetCommandPanel();
			this.UpdateCommandPanel(false);
			this.UpdateGroupConstraints();
		}

		// Token: 0x0600636B RID: 25451 RVA: 0x002EED18 File Offset: 0x002ECF18
		private void RemoveShipFromGroupSelect(CombatantController combatantController)
		{
			combatantController.ref_shipController.ModelController.StopGroupSelectionAnimation();
			if (!this.combatMgr.waypointsVisible)
			{
				combatantController.ref_shipController.SetWaypointVisualization(false);
			}
			using (IEnumerator<object> enumerator = this.friendlyShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__206.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__206.<>p__0 = CallSite<Func<CallSite, object, FriendlyShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FriendlyShipListItemController), typeof(SpaceCombatCanvasController)));
					}
					FriendlyShipListItemController friendlyShipListItemController = SpaceCombatCanvasController.<>o__206.<>p__0.Target(SpaceCombatCanvasController.<>o__206.<>p__0, enumerator.Current);
					if (friendlyShipListItemController.combatantController.GetCombatantType() == IDamageableType.Ship && friendlyShipListItemController.combatantController.ref_shipController.ShipState.ID == combatantController.ref_shipController.ShipState.ID)
					{
						friendlyShipListItemController.SetGroupSelected(false);
						break;
					}
				}
			}
			this.groupSelectedFriendlyShips.Remove(combatantController.ref_shipController);
			this.UpdateFleetCommandPanel();
			this.UpdateCommandPanel(this.groupSelectedFriendlyShips.Count > 1);
			this.UpdateGroupConstraints();
		}

		// Token: 0x0600636C RID: 25452 RVA: 0x002EEE44 File Offset: 0x002ED044
		private void UpdateGroupConstraints()
		{
			this._groupConstraints = TIUtilities.GetAccelerationConstraintsForGroup(this.groupSelectedFriendlyShips, false);
		}

		// Token: 0x0600636D RID: 25453 RVA: 0x002EEE58 File Offset: 0x002ED058
		public List<TISpaceShipState> GetBatchofShips(TISpaceShipState ship, SpaceCombatCanvasController.ChangeCommandScopeMode inputType)
		{
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			switch (inputType)
			{
			default:
				list.Add(ship);
				return list;
			case SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsInGroup:
			{
				using (List<CombatShipController>.Enumerator enumerator = this.groupSelectedFriendlyShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CombatShipController combatShipController = enumerator.Current;
						list.Add(combatShipController.ShipState);
					}
					return list;
				}
				break;
			}
			case SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsOfClass:
				break;
			case SpaceCombatCanvasController.ChangeCommandScopeMode.AllShipsInFleet:
				goto IL_00C5;
			}
			using (IEnumerator<CombatShipController> enumerator2 = this.leftHandFleetController.activeShipControllers.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					CombatShipController combatShipController2 = enumerator2.Current;
					if (ship.template.fullClassName.Equals(combatShipController2.ShipState.template.fullClassName))
					{
						list.Add(combatShipController2.ShipState);
					}
				}
				return list;
			}
			IL_00C5:
			list.AddRange(this.leftHandFleetController.activeShipControllers.Select<CombatShipController, TISpaceShipState>((CombatShipController x) => x.ShipState));
			return list;
		}

		// Token: 0x0600636E RID: 25454 RVA: 0x002EEF7C File Offset: 0x002ED17C
		private void AddFriendlyShipListeners()
		{
			GameControl.eventManager.AddListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.UpdateForDamageChange), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.UpdateForDamageChange), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipPowerSystemsChargeChange>(new EventManager.EventDelegate<ShipPowerSystemsChargeChange>(this.SetPowerStoragePosition), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipHeatChange>(new EventManager.EventDelegate<ShipHeatChange>(this.SetHeatPosition), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipWeaponModeChanged>(new EventManager.EventDelegate<ShipWeaponModeChanged>(this.OnWeaponModeChanged), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnWeaponFired), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipAIControlChange>(new EventManager.EventDelegate<ShipAIControlChange>(this.OnAIControlChange), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipCommandExecuted>(new EventManager.EventDelegate<ShipCommandExecuted>(this.OnCommandExecuted), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipPrimaryTargetSelected>(new EventManager.EventDelegate<ShipPrimaryTargetSelected>(this.OnPrimaryTargetSelected), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.OnDeltaVChange), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<CombatShipPropulsionValuesUpdated>(new EventManager.EventDelegate<CombatShipPropulsionValuesUpdated>(this.OnPropulsionValuesUpdated), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<CombatManeuverComplete>(new EventManager.EventDelegate<CombatManeuverComplete>(this.OnCombatManeuverComplete), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<CombatCollisionAvoidanceStatusChange>(new EventManager.EventDelegate<CombatCollisionAvoidanceStatusChange>(this.OnCollisionStatusUpdate), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipPartBeingRepaired>(new EventManager.EventDelegate<ShipPartBeingRepaired>(this.OnPartBeingRepaired), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipPartNoLongerBeingRepaired>(new EventManager.EventDelegate<ShipPartNoLongerBeingRepaired>(this.OnPartNoLongerBeingRepaired), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipSystemBeingRepaired>(new EventManager.EventDelegate<ShipSystemBeingRepaired>(this.OnSystemBeingRepaired), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipSystemNoLongerBeingRepaired>(new EventManager.EventDelegate<ShipSystemNoLongerBeingRepaired>(this.OnSystemNoLongerBeingRepaired), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.OnShipOfficerKilled), null, this.selectedFriendlyShip, false, false);
			GameControl.eventManager.AddListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null, this.selectedFriendlyShipState, false, false);
			GameControl.eventManager.AddListener<CombatShipGroupChange>(new EventManager.EventDelegate<CombatShipGroupChange>(this.OnShipGroupChanged), null, null, false, false);
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x002EF1F0 File Offset: 0x002ED3F0
		private void RemoveFriendlyShipListeners()
		{
			GameControl.eventManager.RemoveListener<ShipPowerSystemsChargeChange>(new EventManager.EventDelegate<ShipPowerSystemsChargeChange>(this.SetPowerStoragePosition), null);
			GameControl.eventManager.RemoveListener<ShipHeatChange>(new EventManager.EventDelegate<ShipHeatChange>(this.SetHeatPosition), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.UpdateForDamageChange), null);
			GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.UpdateForDamageChange), null);
			GameControl.eventManager.RemoveListener<ShipWeaponModeChanged>(new EventManager.EventDelegate<ShipWeaponModeChanged>(this.OnWeaponModeChanged), null);
			GameControl.eventManager.RemoveListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnWeaponFired), null);
			GameControl.eventManager.RemoveListener<ShipAIControlChange>(new EventManager.EventDelegate<ShipAIControlChange>(this.OnAIControlChange), null);
			GameControl.eventManager.RemoveListener<ShipCommandExecuted>(new EventManager.EventDelegate<ShipCommandExecuted>(this.OnCommandExecuted), null);
			GameControl.eventManager.RemoveListener<ShipPrimaryTargetSelected>(new EventManager.EventDelegate<ShipPrimaryTargetSelected>(this.OnPrimaryTargetSelected), null);
			GameControl.eventManager.RemoveListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.OnDeltaVChange), null);
			GameControl.eventManager.RemoveListener<CombatShipPropulsionValuesUpdated>(new EventManager.EventDelegate<CombatShipPropulsionValuesUpdated>(this.OnPropulsionValuesUpdated), null);
			GameControl.eventManager.RemoveListener<CombatManeuverComplete>(new EventManager.EventDelegate<CombatManeuverComplete>(this.OnCombatManeuverComplete), null);
			GameControl.eventManager.RemoveListener<CombatCollisionAvoidanceStatusChange>(new EventManager.EventDelegate<CombatCollisionAvoidanceStatusChange>(this.OnCollisionStatusUpdate), null);
			GameControl.eventManager.RemoveListener<ShipPartBeingRepaired>(new EventManager.EventDelegate<ShipPartBeingRepaired>(this.OnPartBeingRepaired), null);
			GameControl.eventManager.RemoveListener<ShipPartNoLongerBeingRepaired>(new EventManager.EventDelegate<ShipPartNoLongerBeingRepaired>(this.OnPartNoLongerBeingRepaired), null);
			GameControl.eventManager.RemoveListener<ShipSystemBeingRepaired>(new EventManager.EventDelegate<ShipSystemBeingRepaired>(this.OnSystemBeingRepaired), null);
			GameControl.eventManager.RemoveListener<ShipSystemNoLongerBeingRepaired>(new EventManager.EventDelegate<ShipSystemNoLongerBeingRepaired>(this.OnSystemNoLongerBeingRepaired), null);
			GameControl.eventManager.RemoveListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.OnShipOfficerKilled), null);
			GameControl.eventManager.RemoveListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null);
			GameControl.eventManager.RemoveListener<CombatShipGroupChange>(new EventManager.EventDelegate<CombatShipGroupChange>(this.OnShipGroupChanged), null);
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x002EF3CC File Offset: 0x002ED5CC
		public void SetBatteryCapacityPosition()
		{
			this.selectedShipCurrentBatteryCapacity_y = this.batteryYBottom + this.selectedFriendlyShipState.availablePowerStorageFraction * this.batteryYRange;
			this.selectedShipCurrentBatteryCapacity.transform.localPosition = new Vector3(this.selectedShipCurrentBatteryCapacity.transform.localPosition.x, this.selectedShipCurrentBatteryCapacity_y, this.selectedShipCurrentBatteryCapacity.transform.localPosition.z);
			this.selectedShipCurrentBatteryCapacity.color = ((this.selectedFriendlyShipState.CurrentBatteryCapacity_GJ() <= 0f) ? Color.red : Color.white);
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x002EF466 File Offset: 0x002ED666
		public void SetPowerStoragePosition(ShipPowerSystemsChargeChange e)
		{
			this.SetBatteryChargePosition();
		}

		// Token: 0x06006372 RID: 25458 RVA: 0x002EF470 File Offset: 0x002ED670
		public void SetBatteryChargePosition()
		{
			float availablePowerFraction = this.selectedFriendlyShipState.availablePowerFraction;
			this.selectedShipCurrentBatteryCharge_y = this.batteryYBottom + availablePowerFraction * this.batteryYRange;
			this.selectedShipCurrentBatteryCharge.transform.localPosition = new Vector3(this.selectedShipCurrentBatteryCharge.transform.localPosition.x, this.selectedShipCurrentBatteryCharge_y, this.selectedShipCurrentBatteryCharge.transform.localPosition.z);
			this.selectedShipCurrentBatteryCharge.color = ((availablePowerFraction <= 0.1f) ? Color.red : ((availablePowerFraction <= 0.3f) ? Color.yellow : Color.white));
			this.selectedShipBatteryAlert.enabled = availablePowerFraction <= 0.3f;
			if (availablePowerFraction <= 0.1f)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui/ICO_critical", this.selectedShipBatteryAlert);
				return;
			}
			if (availablePowerFraction <= 0.3f)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui/ICO_warning", this.selectedShipBatteryAlert);
			}
		}

		// Token: 0x06006373 RID: 25459 RVA: 0x002EF562 File Offset: 0x002ED762
		public void SetHeatPosition(ShipHeatChange e)
		{
			if (TIGameState.Valid(e.ship))
			{
				this.SetHeatPosition();
			}
		}

		// Token: 0x06006374 RID: 25460 RVA: 0x002EF578 File Offset: 0x002ED778
		public void SetHeatPosition()
		{
			if (TIGameState.Valid(this.selectedFriendlyShipState))
			{
				float num = Mathf.Min(this.selectedFriendlyShipState.heatFraction, 1f);
				this.selectedShipCurrentHeat_y = this.heatYBottom + num * this.HeatYRange;
				this.selectedShipCurrentHeat.transform.localPosition = new Vector3(this.selectedShipCurrentHeat.transform.localPosition.x, this.selectedShipCurrentHeat_y, this.selectedShipCurrentHeat.transform.localPosition.z);
				float num2 = this.selectedFriendlyShipState.heatCapFraction - num;
				this.selectedShipCurrentHeat.color = ((num2 <= 0.15f) ? Color.red : ((num2 <= 0.45f) ? Color.yellow : Color.cyan));
				this.selectedShipHeatAlert.enabled = num2 <= 0.15f;
				if (num2 <= 0f)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment("ui/ICO_critical", this.selectedShipHeatAlert);
				}
				else if (num2 <= 0.15f)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment("ui/ICO_warning", this.selectedShipHeatAlert);
				}
				SpaceCombatCanvasController.SetHeatIcon(this.selectedFriendlyShipState, this.selectedShipHeatIcon, this.selectedShipCoolingIcon);
			}
		}

		// Token: 0x06006375 RID: 25461 RVA: 0x002EF6AC File Offset: 0x002ED8AC
		public void SetHeatCapacityPosition()
		{
			if (TIGameState.Valid(this.selectedFriendlyShipState))
			{
				this.selectedShipCurrentHeatCapacity_y = this.heatYBottom + this.selectedFriendlyShipState.heatCapFraction * this.HeatYRange;
				this.selectedShipCurrentHeatCapacity.transform.localPosition = new Vector3(this.selectedShipCurrentHeatCapacity.transform.localPosition.x, this.selectedShipCurrentHeatCapacity_y, this.selectedShipCurrentHeatCapacity.transform.localPosition.z);
				this.selectedShipCurrentHeatCapacity.color = ((this.selectedFriendlyShipState.radiatorsExtended && !this.selectedFriendlyShipState.PartDestroyed(this.selectedFriendlyShipState.radiatorModule)) ? Color.white : Color.red);
				SpaceCombatCanvasController.SetHeatIcon(this.selectedFriendlyShipState, this.selectedShipHeatIcon, this.selectedShipCoolingIcon);
			}
		}

		// Token: 0x06006376 RID: 25462 RVA: 0x002EF780 File Offset: 0x002ED980
		public void UpdateVelocityValue()
		{
			float magnitude = this.selectedFriendlyShip.velocityVector_kps.magnitude;
			if (magnitude < 1f)
			{
				this.velocityText.SetText(Loc.T("UI.SpaceCombat.velocity_mps", new object[] { (magnitude * 1000f).ToString("N0") }));
				return;
			}
			this.velocityText.SetText(Loc.T("UI.SpaceCombat.velocity_kps", new object[] { magnitude.ToString("N2") }));
		}

		// Token: 0x06006377 RID: 25463 RVA: 0x002EF806 File Offset: 0x002EDA06
		public void UpdateAccelerationValue()
		{
			this.accelerationText.SetText(FleetsScreenController.accelerationStr((double)this.selectedFriendlyShip.ShipState.combatAcceleration_gs, true, false, true));
		}

		// Token: 0x06006378 RID: 25464 RVA: 0x002EF82C File Offset: 0x002EDA2C
		private string DeltaVTooltip()
		{
			return Loc.T("UI.SpaceCombat.DeltaVTooltip", new object[] { TIUtilities.FormatBigOrSmallNumber(this.selectedFriendlyShip.ShipState.AvailableDeltaVForCombat_kps(), 1, 7, 0, false, false) });
		}

		// Token: 0x06006379 RID: 25465 RVA: 0x002EF866 File Offset: 0x002EDA66
		public void OnDeltaVChange(ShipDeltaVChange e)
		{
			this.OnDeltaVChange();
		}

		// Token: 0x0600637A RID: 25466 RVA: 0x002EF870 File Offset: 0x002EDA70
		public void OnDeltaVChange()
		{
			float num = this.selectedFriendlyShipState.AvailableDeltaVForCombat_kps();
			this.currentDeltaVCoverImage.sizeDelta = new Vector2(this.currentDeltaVCoverMaxWidth * (1f - num / this.combatState.maxDeltaVAvailableForCombat_kps[this.selectedFriendlyShipState]), this.currentDeltaVCoverImage.sizeDelta.y);
			this.UpdateVelocityValue();
			this.currentDeltaVText.SetText(Loc.T("UI.SpaceCombat.DeltaV", new object[] { TIUtilities.FormatBigOrSmallNumber(num, 1, 1, 0, false, false) }));
			this.selectedShipDeltaVAlert.enabled = num <= 0f;
			if (num <= 0f)
			{
				this.fleetCommandsDataDirty = true;
				this.shipCommandsDataDirty = true;
			}
		}

		// Token: 0x0600637B RID: 25467 RVA: 0x002EF929 File Offset: 0x002EDB29
		public void OnShipDamageControlRotationStatusChanged(ShipDamageControlRotationStatusChanged e)
		{
			this.SetDamageControlIcons();
		}

		// Token: 0x0600637C RID: 25468 RVA: 0x002EF934 File Offset: 0x002EDB34
		public void OnPrimaryTargetSelected(ShipPrimaryTargetSelected e)
		{
			if (this.selectedFriendlyShip.oldPrimaryTarget != null)
			{
				CombatShipController ref_shipController = this.selectedFriendlyShip.oldPrimaryTarget.ref_shipController;
				if (ref_shipController != null)
				{
					ref_shipController.ModelController.StopSelectionAnimation();
				}
				this.selectedFriendlyShip.oldPrimaryTarget.UIController().maintainAnimation = false;
			}
			this.SetPrimaryTargetText();
			this.fleetCommandsDataDirty = true;
			if (this.selectedFriendlyShip.primaryTarget != null)
			{
				CombatShipController ref_shipController2 = this.selectedFriendlyShip.primaryTarget.ref_shipController;
				if (ref_shipController2 != null)
				{
					ref_shipController2.ModelController.StartSelectionAnimation();
				}
				this.selectedFriendlyShip.primaryTarget.UIController().maintainAnimation = true;
			}
		}

		// Token: 0x0600637D RID: 25469 RVA: 0x002EF9EC File Offset: 0x002EDBEC
		public void SetPrimaryTargetText()
		{
			if (this.selectedFriendlyShipState.combatPrimaryTarget == null)
			{
				this.selectedShipPrimaryTargetText.SetText(Loc.T("UI.SpaceCombat.NoPrimaryTarget"));
				return;
			}
			this.selectedShipPrimaryTargetText.SetText(Loc.T("UI.SpaceCombat.PrimaryTarget", new object[] { this.selectedFriendlyShipState.combatPrimaryTarget.GetTargetableState().displayName }));
		}

		// Token: 0x0600637E RID: 25470 RVA: 0x002EFA50 File Offset: 0x002EDC50
		public static void SetHeatIcon(TISpaceShipState ship, Image icon, Image coolingIcon)
		{
			float num = ship.heatCapFraction - ship.heatFraction;
			if (ship.overheated)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_thermo_D", icon);
			}
			else if (num <= 0.15f)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_thermo_C", icon);
			}
			else if (num <= 0.45f)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_thermo_B", icon);
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_thermo_A", icon);
			}
			coolingIcon.enabled = ship.cooling;
		}

		// Token: 0x0600637F RID: 25471 RVA: 0x002EFAD8 File Offset: 0x002EDCD8
		public void UpdateForDamageChange(ShipSystemDamageChange e)
		{
			this.systemDamageGrid[e.system].TookDamage(e.systemRepaired);
			this.systemDamageGrid[e.system].UpdateListItem();
			ShipSystem system = e.system;
			if (system != ShipSystem.PowerCoupling)
			{
				if (system == ShipSystem.DriveCoupling || system - ShipSystem.PowerPlant <= 1)
				{
					this.UpdateAccelerationValue();
				}
			}
			else
			{
				this.SetBatteryCapacityPosition();
			}
			this.fleetCommandsDataDirty = true;
		}

		// Token: 0x06006380 RID: 25472 RVA: 0x002EFB44 File Offset: 0x002EDD44
		public void UpdateForDamageChange(ShipPartDamageChange e)
		{
			this.moduleDamageGrid[e.partData].TookDamage(e.partRepaired);
			this.moduleDamageGrid[e.partData].UpdateListItem();
			if (e.partData.moduleTemplate is TIHeatSinkTemplate || e.partData.moduleTemplate is TIRadiatorTemplate)
			{
				this.SetHeatPosition();
				this.SetHeatCapacityPosition();
				SpaceCombatCanvasController.SetHeatIcon(this.selectedFriendlyShipState, this.selectedShipHeatIcon, this.selectedShipCoolingIcon);
			}
			else if (e.partData.moduleTemplate is TIBatteryTemplate)
			{
				this.SetBatteryChargePosition();
				this.SetBatteryCapacityPosition();
			}
			this.fleetCommandsDataDirty = true;
			if (this.selectedFriendlyShip != null && e.ship == this.selectedFriendlyShip.ShipState)
			{
				this.shipCommandsDataDirty = true;
			}
		}

		// Token: 0x06006381 RID: 25473 RVA: 0x002EFC1E File Offset: 0x002EDE1E
		public void OnArmorHit(ShipArmorFacingStruckInCombat e)
		{
		}

		// Token: 0x06006382 RID: 25474 RVA: 0x002EFC20 File Offset: 0x002EDE20
		public void SetDamageControlIcons()
		{
			foreach (DamagedShipPartData damagedShipPartData in this.selectedFriendlyShip.ShipState.prevPartsBeingRepaired)
			{
				this.moduleDamageGrid[damagedShipPartData.module].SetRepairStatus(true, this.selectedFriendlyShip.ShipState.isDamageControlSuspended);
			}
			foreach (ShipSystem shipSystem in this.selectedFriendlyShip.ShipState.prevSystemsBeingRepaired)
			{
				this.systemDamageGrid[shipSystem].SetRepairStatus(true, this.selectedFriendlyShip.ShipState.isDamageControlSuspended);
			}
		}

		// Token: 0x06006383 RID: 25475 RVA: 0x002EFD04 File Offset: 0x002EDF04
		public void OnPartBeingRepaired(ShipPartBeingRepaired e)
		{
			this.moduleDamageGrid[e.partData].SetRepairStatus(true, this.selectedFriendlyShip.ShipState.isDamageControlSuspended);
		}

		// Token: 0x06006384 RID: 25476 RVA: 0x002EFD2D File Offset: 0x002EDF2D
		public void OnPartNoLongerBeingRepaired(ShipPartNoLongerBeingRepaired e)
		{
			this.moduleDamageGrid[e.partData].SetRepairStatus(false, this.selectedFriendlyShip.ShipState.isDamageControlSuspended);
		}

		// Token: 0x06006385 RID: 25477 RVA: 0x002EFD56 File Offset: 0x002EDF56
		public void OnSystemBeingRepaired(ShipSystemBeingRepaired e)
		{
			this.systemDamageGrid[e.system].SetRepairStatus(true, this.selectedFriendlyShip.ShipState.isDamageControlSuspended);
		}

		// Token: 0x06006386 RID: 25478 RVA: 0x002EFD7F File Offset: 0x002EDF7F
		public void OnSystemNoLongerBeingRepaired(ShipSystemNoLongerBeingRepaired e)
		{
			this.systemDamageGrid[e.system].SetRepairStatus(false, this.selectedFriendlyShip.ShipState.isDamageControlSuspended);
		}

		// Token: 0x06006387 RID: 25479 RVA: 0x002EFDA8 File Offset: 0x002EDFA8
		public void OnAIControlChange(ShipAIControlChange e)
		{
			foreach (ShipWeaponUIController shipWeaponUIController in this.weaponUIControllers.Values)
			{
				shipWeaponUIController.UpdateStatus();
			}
			this.fleetCommandsDataDirty = true;
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x002EFE04 File Offset: 0x002EE004
		public void OnWeaponModeChanged(ShipWeaponModeChanged e)
		{
			this.weaponUIControllers[e.weaponData].UpdateFireMode();
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x002EFE1C File Offset: 0x002EE01C
		public void OnWeaponFired(ShipWeaponFired e)
		{
			this.weaponUIControllers[e.weaponData].UpdateGridItem();
		}

		// Token: 0x0600638A RID: 25482 RVA: 0x002EFE34 File Offset: 0x002EE034
		public void OnCommandExecuted(ShipCommandExecuted e)
		{
			this.shipCommandsDataDirty = true;
			this.fleetCommandsDataDirty = true;
			if (e.command.TriggersManeuver)
			{
				SpaceCombatCanvasController.UpdateManeuverList(this.selectedFriendlyShipState, this.selectedFriendlyShip, this.maneuverList);
			}
		}

		// Token: 0x0600638B RID: 25483 RVA: 0x002EFE68 File Offset: 0x002EE068
		public void OnCombatManeuverComplete(CombatManeuverComplete e)
		{
			SpaceCombatCanvasController.UpdateManeuverList(this.selectedFriendlyShipState, this.selectedFriendlyShip, this.maneuverList);
			this.shipCommandsDataDirty = true;
			this.fleetCommandsDataDirty = true;
		}

		// Token: 0x0600638C RID: 25484 RVA: 0x002EFE8F File Offset: 0x002EE08F
		public void OnCollisionStatusUpdate(CombatCollisionAvoidanceStatusChange e)
		{
			SpaceCombatCanvasController.UpdateManeuverList(this.selectedFriendlyShipState, this.selectedFriendlyShip, this.maneuverList);
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x002EFEA8 File Offset: 0x002EE0A8
		public void OnRadiatorsExtended(CompleteExtendRadiatorsEvent e)
		{
			if (((e.ship != null) & (e.ship.fleet != null)) && base.activePlayer == e.ship.faction)
			{
				this.fleetCommandsDataDirty = true;
				if (this.selectedFriendlyShip != null && e.ship == this.selectedFriendlyShip.ShipState)
				{
					this.SetSelectedShipRadiators();
					this.shipCommandsDataDirty = true;
				}
			}
		}

		// Token: 0x0600638E RID: 25486 RVA: 0x002EFF28 File Offset: 0x002EE128
		public void OnRadiatorsRetracted(CompleteRetractRadiatorsEvent e)
		{
			if (base.activePlayer == e.ship.faction)
			{
				this.fleetCommandsDataDirty = true;
				if (this.selectedFriendlyShip != null && e.ship == this.selectedFriendlyShip.ShipState)
				{
					this.SetSelectedShipRadiators();
					this.shipCommandsDataDirty = true;
				}
			}
		}

		// Token: 0x0600638F RID: 25487 RVA: 0x002EFF88 File Offset: 0x002EE188
		public void SetSelectedShipRadiators()
		{
			if (!this.selectedFriendlyShipState.hull.simpleHull)
			{
				if (this.selectedFriendlyShipState.radiatorsExtended)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(this.selectedFriendlyShipState.radiators.largecombatUI_On(this.selectedFriendlyShipState.hull, this.selectedFriendlyShipState.template.GetHullAppearanceIndex), this.selectedShipRadiators);
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(this.selectedFriendlyShipState.radiators.largecombatUI_Off(this.selectedFriendlyShipState.hull, this.selectedFriendlyShipState.template.GetHullAppearanceIndex), this.selectedShipRadiators);
				}
				this.selectedShipRadiators.gameObject.SetActive(true);
				return;
			}
			this.selectedShipRadiators.gameObject.SetActive(false);
		}

		// Token: 0x06006390 RID: 25488 RVA: 0x002F0052 File Offset: 0x002EE252
		public void OnPropulsionValuesUpdated(CombatShipPropulsionValuesUpdated e)
		{
			this.UpdateAccelerationValue();
			this.OnDeltaVChange();
			if (this.selectedFriendlyShip.ShipState.ManeuverEffectivenessRatio == 0f)
			{
				this.shipCommandsDataDirty = true;
				this.fleetCommandsDataDirty = true;
			}
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x002F0088 File Offset: 0x002EE288
		public static Vector2Int GetModuleDamageControllerPosition(TISpaceShipState ship, ModuleDataEntry module)
		{
			return new Vector2Int(ship.hull.shipModuleSlots[module.slotIndex].slotPosition.x * 2, ship.hull.shipModuleSlots[module.slotIndex].slotPosition.y);
		}

		// Token: 0x06006392 RID: 25490 RVA: 0x002F00E8 File Offset: 0x002EE2E8
		public static Vector2Int GetSystemDamageControllerPosition(TISpaceShipState ship, ShipSystem shipSystem)
		{
			switch (shipSystem)
			{
			case ShipSystem.NoseStructure:
				return new Vector2Int(ship.hull.shipModuleSlots.Max<TIShipHullTemplate.ShipModuleSlot>((TIShipHullTemplate.ShipModuleSlot x) => x.x) * 2, 7);
			case ShipSystem.CentralStructure:
				return new Vector2Int(9, 7);
			case ShipSystem.TailStructure:
				return new Vector2Int(ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive).x * 2, 7);
			case ShipSystem.Bridge:
				return new Vector2Int(9, 3);
			case ShipSystem.FireControl:
				return new Vector2Int(9, 4);
			case ShipSystem.SystemsReactor:
				return new Vector2Int(11, 3);
			case ShipSystem.PowerCoupling:
			{
				Vector2Int uniqueSlotCoordinates = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.PowerPlant);
				return new Vector2Int(uniqueSlotCoordinates.x * 2 + 1, uniqueSlotCoordinates.y);
			}
			case ShipSystem.DriveCoupling:
			{
				Vector2Int uniqueSlotCoordinates2 = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive);
				return new Vector2Int(uniqueSlotCoordinates2.x * 2 + 1, uniqueSlotCoordinates2.y);
			}
			case ShipSystem.VectorThrusters:
			{
				Vector2Int uniqueSlotCoordinates3 = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive);
				return new Vector2Int(uniqueSlotCoordinates3.x * 2 + 1, uniqueSlotCoordinates3.y + 1);
			}
			case ShipSystem.LifeSupportMain:
				return new Vector2Int(7, 4);
			case ShipSystem.LifeSupportBackup:
				return new Vector2Int(7, 2);
			case ShipSystem.DamageControl:
				return new Vector2Int(9, 2);
			case ShipSystem.Propellant:
			{
				Vector2Int uniqueSlotCoordinates4 = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive);
				return new Vector2Int(uniqueSlotCoordinates4.x * 2 + 1, uniqueSlotCoordinates4.y - 1);
			}
			case ShipSystem.Sensors:
				return new Vector2Int(7, 3);
			case ShipSystem.Radiators:
			{
				Vector2Int uniqueSlotCoordinates5 = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.Radiator);
				return new Vector2Int(uniqueSlotCoordinates5.x * 2, uniqueSlotCoordinates5.y);
			}
			case ShipSystem.PowerPlant:
			{
				Vector2Int uniqueSlotCoordinates6 = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.PowerPlant);
				return new Vector2Int(uniqueSlotCoordinates6.x * 2, uniqueSlotCoordinates6.y);
			}
			case ShipSystem.Drive:
			{
				Vector2Int uniqueSlotCoordinates7 = ship.hull.GetUniqueSlotCoordinates(ShipModuleSlotType.Drive);
				return new Vector2Int(uniqueSlotCoordinates7.x * 2, uniqueSlotCoordinates7.y);
			}
			}
			return default(Vector2Int);
		}

		// Token: 0x06006393 RID: 25491 RVA: 0x002F02FC File Offset: 0x002EE4FC
		public void UpdateCommandPanel(bool groupSelected)
		{
			this.commandButtons.ForEach(delegate(Button x)
			{
				x.enabled = false;
			});
			this.commandButtons.ForEach(delegate(Button x)
			{
				x.image.enabled = false;
			});
			this.commandTooltips.ForEach(delegate(TooltipTrigger x)
			{
				x.enabled = false;
			});
			if (groupSelected)
			{
				this.UpdateCommandPanelForGroup();
				return;
			}
			this.UpdateCommandPanelForSingleShip();
		}

		// Token: 0x06006394 RID: 25492 RVA: 0x002F0398 File Offset: 0x002EE598
		private void UpdateCommandPanelForSingleShip()
		{
			using (List<IShipCommand>.Enumerator enumerator = ShipCommandsManager.shipCommands.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IShipCommand command = enumerator.Current;
					if (this.selectedFriendlyShipState != null && command.CommandVisibleToActor(this.selectedFriendlyShipState))
					{
						Button button = this.commandButtons[command.IconPosition()];
						if (button.transition == Selectable.Transition.SpriteSwap)
						{
							SpriteState spriteState = new SpriteState
							{
								highlightedSprite = this.commandIconCache[command.GetTemplate().dataName].commandSprite_on,
								pressedSprite = this.commandIconCache[command.GetTemplate().dataName].commandSprite_on
							};
							button.spriteState = spriteState;
						}
						button.image.sprite = this.commandIconCache[command.GetTemplate().dataName].commandSprite_off;
						button.enabled = true;
						button.image.enabled = true;
						button.onClick.RemoveAllListeners();
						this.commandTooltips[command.IconPosition()].enabled = true;
						this.commandTooltips[command.IconPosition()].SetDelegate("BodyText", () => command.GetTooltipText(this.selectedFriendlyShipState));
						if (command.ActorCanPerformCommand(this.selectedFriendlyShipState))
						{
							button.onClick.AddListener(delegate
							{
								AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteShipCommand", false, false);
							});
							button.interactable = true;
							if (command.RequiresTarget())
							{
								button.onClick.AddListener(delegate
								{
									(command as IShipCommandWithTarget).InitiateTargeting(this.selectedFriendlyShipState);
								});
							}
							else
							{
								button.onClick.AddListener(delegate
								{
									command.OnCommandExecute(this.selectedFriendlyShipState, null);
								});
							}
						}
						else
						{
							button.interactable = false;
						}
						button.image.color = (button.interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.2f));
					}
				}
			}
			this.isDisplayingGroupCommands = false;
			this.commandsTargetText.SetText(Loc.T("UI.SpaceCombat.CommandTarget.Single"));
			if (this.groupSelectedFriendlyShips.Count <= 1)
			{
				this.commandSpinnerLeft.gameObject.SetActive(false);
				this.commandSpinnerRight.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006395 RID: 25493 RVA: 0x002F065C File Offset: 0x002EE85C
		private void UpdateCommandPanelForGroup()
		{
			List<TISpaceShipState> shipsToRecieveGroupCommands = new List<TISpaceShipState>();
			if (this.groupSelectedFriendlyShips.Count > 1)
			{
				shipsToRecieveGroupCommands = (from x in this.groupSelectedFriendlyShips
					select x.GetCombatantState() as TISpaceShipState into y
					where y != null && !y.ShipDestroyed() && !y.hasDisengaged
					select y).ToList<TISpaceShipState>();
			}
			else
			{
				Debug.LogError("No Group Selected, Should not be attempting to update command panel for group");
			}
			using (List<IFleetCommand>.Enumerator enumerator = ShipCommandsManager.fleetCommands.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IFleetCommand command = enumerator.Current;
					if (this.selectedFriendlyShipState != null && command.CommandVisibleToPlayer(shipsToRecieveGroupCommands))
					{
						Button button = this.commandButtons[command.IconPosition()];
						if (button.transition == Selectable.Transition.SpriteSwap)
						{
							SpriteState spriteState = new SpriteState
							{
								highlightedSprite = this.commandIconCache[command.GetTemplate().dataName].commandSprite_on,
								pressedSprite = this.commandIconCache[command.GetTemplate().dataName].commandSprite_on
							};
							button.spriteState = spriteState;
						}
						button.image.sprite = this.commandIconCache[command.GetTemplate().dataName].commandSprite_off;
						button.enabled = true;
						button.image.enabled = true;
						button.image.color = Color.red;
						button.onClick.RemoveAllListeners();
						this.commandTooltips[command.IconPosition()].enabled = true;
						this.commandTooltips[command.IconPosition()].SetDelegate("BodyText", () => command.GetTooltipText(true));
						if (command.PlayerCanIssueCommand(shipsToRecieveGroupCommands))
						{
							button.onClick.AddListener(delegate
							{
								AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteFleetCommand", false, false);
							});
							button.interactable = true;
							if (command.RequiresTarget())
							{
								button.onClick.AddListener(delegate
								{
									(command as IFleetCommandWithTarget).InitiateTargeting(shipsToRecieveGroupCommands);
								});
							}
							else
							{
								button.onClick.AddListener(delegate
								{
									command.OnExecuteFleetCommand(shipsToRecieveGroupCommands, null);
								});
							}
						}
						else
						{
							button.interactable = false;
						}
						button.image.color = (button.interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.2f));
					}
				}
			}
			this.isDisplayingGroupCommands = true;
			this.commandsTargetText.SetText(Loc.T("UI.SpaceCombat.CommandTarget.Group"));
			this.commandSpinnerLeft.gameObject.SetActive(true);
			this.commandSpinnerRight.gameObject.SetActive(true);
		}

		// Token: 0x06006396 RID: 25494 RVA: 0x002F09B4 File Offset: 0x002EEBB4
		public void ToggleTargetForCommandButtonPanel()
		{
			if (this.isDisplayingGroupCommands)
			{
				this.UpdateCommandPanelForSingleShip();
				return;
			}
			this.UpdateCommandPanelForGroup();
		}

		// Token: 0x06006397 RID: 25495 RVA: 0x002F09CC File Offset: 0x002EEBCC
		public void ToggleShipManeuverPanel()
		{
			if (!this.shipManeuverPanel.activeInHierarchy)
			{
				this.shipCommandsDataDirty = true;
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.shipManeuverPanel.SetActive(true);
				this.isManeuverPanelOpen = true;
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.shipManeuverPanel.SetActive(false);
			this.isManeuverPanelOpen = false;
		}

		// Token: 0x06006398 RID: 25496 RVA: 0x002F0A2C File Offset: 0x002EEC2C
		public void ToggleFleetManeuverPanel()
		{
			if (!this.fleetManeuverPanel.activeInHierarchy)
			{
				this.fleetCommandsDataDirty = true;
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.fleetManeuverPanel.SetActive(true);
				this.isFleetManeuverPanelOpen = true;
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.fleetManeuverPanel.SetActive(false);
			this.isFleetManeuverPanelOpen = false;
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x002F0A8C File Offset: 0x002EEC8C
		public void ToggleFleetCommandCard()
		{
			if (this.shipCommandPanelObject.activeSelf)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.shipCommandPanelObject.SetActive(false);
				this.isFleetCommandCardPanelOpen = true;
				if (this.isManeuverPanelOpen)
				{
					this.shipManeuverPanel.SetActive(false);
					return;
				}
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.shipCommandPanelObject.SetActive(true);
				this.isFleetCommandCardPanelOpen = false;
				if (this.isManeuverPanelOpen)
				{
					this.shipManeuverPanel.SetActive(true);
				}
			}
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x002F0B10 File Offset: 0x002EED10
		public static void UpdateManeuverList(TISpaceShipState shipState, CombatShipController shipController, ListManagerBase maneuverList)
		{
			List<Sprite> list = new List<Sprite>();
			int num = 0;
			foreach (CombatManeuver combatManeuver in shipState.activeCombatManeuvers)
			{
				list.Add(ShipCommandsManager.maneuverIcons[shipState.activeCombatManeuvers[num++]]);
			}
			if (shipState.canSuicide)
			{
				list.Add(AssetCacheManager.rammingSpeedIcon);
			}
			if (shipController.InCollisionAvoidanceManeuver)
			{
				list.Add(AssetCacheManager.warningIcon);
			}
			if (shipState.disengageFromCombat)
			{
				list.Add(AssetCacheManager.disengageIcon);
			}
			num = 0;
			maneuverList.SetListSize<CombatManeuverListItemController>(list.Count, false, false);
			using (IEnumerator<object> enumerator2 = maneuverList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__254.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__254.<>p__0 = CallSite<Func<CallSite, object, CombatManeuverListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombatManeuverListItemController), typeof(SpaceCombatCanvasController)));
					}
					SpaceCombatCanvasController.<>o__254.<>p__0.Target(SpaceCombatCanvasController.<>o__254.<>p__0, enumerator2.Current).SetListItem(list[num++]);
				}
			}
		}

		// Token: 0x0600639B RID: 25499 RVA: 0x002F0C4C File Offset: 0x002EEE4C
		public void OnShipOfficerKilled(ShipOfficerKilled e)
		{
			this.UpdatePersonnelList();
		}

		// Token: 0x0600639C RID: 25500 RVA: 0x002F0C54 File Offset: 0x002EEE54
		public void UpdatePersonnelList()
		{
			TIFactionState activePlayer = GameControl.control.activePlayer;
			List<TIOfficerState> list = new List<TIOfficerState>(this.selectedFriendlyShip.ShipState.officers);
			List<TIOfficerState> list2 = new List<TIOfficerState>();
			if (GameControl.spaceCombat.combatState.deadOfficers.ContainsKey(this.selectedFriendlyShip.ShipState))
			{
				list2 = GameControl.spaceCombat.combatState.deadOfficers[this.selectedFriendlyShip.ShipState].ToList<TIOfficerState>();
				list.AddRange(list2);
			}
			list = list.OrderBy<TIOfficerState, int>((TIOfficerState x) => x.template.sortOrder).ToList<TIOfficerState>();
			List<TICouncilorState> list3 = this.selectedFriendlyShip.ShipState.CouncilorStatesPresentAndKnownToFaction(activePlayer);
			List<CouncilorView> list4 = new List<CouncilorView>();
			foreach (TICouncilorState ticouncilorState in list3)
			{
				list4.Add(activePlayer.GetViewofCouncilor(ticouncilorState));
			}
			int num = 0;
			int num2 = list4.Count + list.Count;
			this.selectedShipPersonnelList.SetListSize<ShipPersonnelGridItemController>(num2, false, false);
			using (IEnumerator<object> enumerator2 = this.selectedShipPersonnelList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__256.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__256.<>p__0 = CallSite<Func<CallSite, object, ShipPersonnelGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipPersonnelGridItemController), typeof(SpaceCombatCanvasController)));
					}
					ShipPersonnelGridItemController shipPersonnelGridItemController = SpaceCombatCanvasController.<>o__256.<>p__0.Target(SpaceCombatCanvasController.<>o__256.<>p__0, enumerator2.Current);
					if (num < list4.Count)
					{
						shipPersonnelGridItemController.UpdateGridItem(list4[num]);
					}
					else
					{
						int num3 = num - list4.Count;
						shipPersonnelGridItemController.UpdateGridItem(list[num3], list2.Contains(list[num3]));
					}
					num++;
				}
			}
			GridLayoutGroup component = this.selectedShipPersonnelList.GetComponent<GridLayoutGroup>();
			if (num2 > 6)
			{
				float num4 = component.cellSize.x - component.cellSize.x * (float)num2 / 6f;
				component.spacing = new Vector2(num4, 0f);
				return;
			}
			this.selectedShipPersonnelList.GetComponent<GridLayoutGroup>().spacing = new Vector2(0f, 0f);
		}

		// Token: 0x0600639D RID: 25501 RVA: 0x002F0EB8 File Offset: 0x002EF0B8
		public void SetSelectedShipPanel()
		{
			this.RemoveFriendlyShipListeners();
			this.weaponUIControllers.Clear();
			this.selectedShipName.SetText(new StringBuilder(this.selectedFriendlyShipState.displayName).Append(", ").Append(this.selectedFriendlyShipState.template.fullClassName));
			GameControl.assetLoader.LoadAssetForImageAssignment(this.selectedFriendlyShipState.hull.largeCombatUIPath(this.selectedFriendlyShipState.template.GetHullAppearanceIndex), this.selectedShipHull);
			this.SetSelectedShipRadiators();
			if (!this.selectedFriendlyShipState.hull.simpleHull)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.selectedFriendlyShipState.drive.largeCombatUIPath(this.selectedFriendlyShipState.hull, this.selectedFriendlyShipState.template.GetHullAppearanceIndex), this.selectedShipDrive);
				this.selectedShipDrive.gameObject.SetActive(true);
			}
			else
			{
				this.selectedShipDrive.gameObject.SetActive(false);
			}
			ListManagerBase listManagerBase = this.selectedShipNoseWeapons;
			List<ModuleDataEntry> noseWeapons = this.selectedFriendlyShipState.noseWeapons;
			listManagerBase.SetListSize<ShipWeaponUIController>((noseWeapons != null) ? noseWeapons.Count : 0, false, false);
			IWeapon[] array = (from x in this.selectedFriendlyShip.hull.IterateByClass<IWeapon>()
				where (x as Weapon).weaponTemplate.noseWeapon
				select x).ToArray<IWeapon>();
			int num = 0;
			using (IEnumerator<object> enumerator = this.selectedShipNoseWeapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__257.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__257.<>p__0 = CallSite<Func<CallSite, object, ShipWeaponUIController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipWeaponUIController), typeof(SpaceCombatCanvasController)));
					}
					ShipWeaponUIController shipWeaponUIController = SpaceCombatCanvasController.<>o__257.<>p__0.Target(SpaceCombatCanvasController.<>o__257.<>p__0, enumerator.Current);
					Weapon weapon = array[num++] as Weapon;
					shipWeaponUIController.Initialize(weapon, this);
					this.weaponUIControllers.Add(weapon.weaponData, shipWeaponUIController);
				}
			}
			ListManagerBase listManagerBase2 = this.selectedShipHullWeapons;
			List<ModuleDataEntry> hullWeapons = this.selectedFriendlyShipState.hullWeapons;
			listManagerBase2.SetListSize<ShipWeaponUIController>((hullWeapons != null) ? hullWeapons.Count : 0, false, false);
			IWeapon[] array2 = (from x in this.selectedFriendlyShip.hull.IterateByClass<IWeapon>()
				where (x as Weapon).weaponTemplate.hullWeapon
				select x).ToArray<IWeapon>();
			num = 0;
			using (IEnumerator<object> enumerator = this.selectedShipHullWeapons.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__257.<>p__1 == null)
					{
						SpaceCombatCanvasController.<>o__257.<>p__1 = CallSite<Func<CallSite, object, ShipWeaponUIController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipWeaponUIController), typeof(SpaceCombatCanvasController)));
					}
					ShipWeaponUIController shipWeaponUIController2 = SpaceCombatCanvasController.<>o__257.<>p__1.Target(SpaceCombatCanvasController.<>o__257.<>p__1, enumerator.Current);
					Weapon weapon2 = array2[num++] as Weapon;
					shipWeaponUIController2.Initialize(weapon2, this);
					this.weaponUIControllers.Add(weapon2.weaponData, shipWeaponUIController2);
				}
			}
			this.SetBatteryCapacityPosition();
			this.SetBatteryChargePosition();
			this.SetHeatCapacityPosition();
			this.SetHeatPosition();
			SpaceCombatCanvasController.SetHeatIcon(this.selectedFriendlyShipState, this.selectedShipHeatIcon, this.selectedShipCoolingIcon);
			this.UpdateAccelerationValue();
			this.OnDeltaVChange();
			this.moduleDamageGrid = new Dictionary<ModuleDataEntry, SpaceCombatDamageGridItemController>();
			this.systemDamageGrid = new Dictionary<ShipSystem, SpaceCombatDamageGridItemController>();
			foreach (SpaceCombatDamageGridItemController spaceCombatDamageGridItemController in this.masterDamageGridControllers.Values)
			{
				spaceCombatDamageGridItemController.Clear();
			}
			foreach (ModuleDataEntry moduleDataEntry in this.selectedFriendlyShipState.AllWeaponModuleData())
			{
				SpaceCombatDamageGridItemController spaceCombatDamageGridItemController2 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetModuleDamageControllerPosition(this.selectedFriendlyShipState, moduleDataEntry)];
				spaceCombatDamageGridItemController2.Initialize(this.selectedFriendlyShipState, moduleDataEntry);
				this.moduleDamageGrid.Add(moduleDataEntry, spaceCombatDamageGridItemController2);
			}
			foreach (ModuleDataEntry moduleDataEntry2 in this.selectedFriendlyShipState.utilityModules)
			{
				SpaceCombatDamageGridItemController spaceCombatDamageGridItemController3 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetModuleDamageControllerPosition(this.selectedFriendlyShipState, moduleDataEntry2)];
				spaceCombatDamageGridItemController3.Initialize(this.selectedFriendlyShipState, moduleDataEntry2);
				this.moduleDamageGrid.Add(moduleDataEntry2, spaceCombatDamageGridItemController3);
			}
			SpaceCombatDamageGridItemController spaceCombatDamageGridItemController4 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(this.selectedFriendlyShipState, ShipSystem.Drive)];
			ModuleDataEntry driveModule = this.selectedFriendlyShipState.driveModule;
			spaceCombatDamageGridItemController4.Initialize(this.selectedFriendlyShipState, driveModule);
			this.moduleDamageGrid.Add(driveModule, spaceCombatDamageGridItemController4);
			SpaceCombatDamageGridItemController spaceCombatDamageGridItemController5 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(this.selectedFriendlyShipState, ShipSystem.Radiators)];
			ModuleDataEntry radiatorModule = this.selectedFriendlyShipState.radiatorModule;
			spaceCombatDamageGridItemController5.Initialize(this.selectedFriendlyShipState, radiatorModule);
			this.moduleDamageGrid.Add(radiatorModule, spaceCombatDamageGridItemController5);
			SpaceCombatDamageGridItemController spaceCombatDamageGridItemController6 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(this.selectedFriendlyShipState, ShipSystem.PowerPlant)];
			ModuleDataEntry powerPlantModule = this.selectedFriendlyShipState.powerPlantModule;
			spaceCombatDamageGridItemController6.Initialize(this.selectedFriendlyShipState, powerPlantModule);
			this.moduleDamageGrid.Add(powerPlantModule, spaceCombatDamageGridItemController6);
			foreach (ShipSystem shipSystem in Enums.DamageableShipSystems)
			{
				SpaceCombatDamageGridItemController spaceCombatDamageGridItemController7 = this.masterDamageGridControllers[SpaceCombatCanvasController.GetSystemDamageControllerPosition(this.selectedFriendlyShipState, shipSystem)];
				spaceCombatDamageGridItemController7.Initialize(this.selectedFriendlyShipState, shipSystem);
				this.systemDamageGrid.Add(shipSystem, spaceCombatDamageGridItemController7);
			}
			this.shipCommandsDataDirty = true;
			this.SetPrimaryTargetText();
			SpaceCombatCanvasController.UpdateManeuverList(this.selectedFriendlyShipState, this.selectedFriendlyShip, this.maneuverList);
			this.UpdatePersonnelList();
			this.UpdateCommandPanel(this.groupSelectedFriendlyShips.Count > 1);
			this.AddFriendlyShipListeners();
			this.ShowShipSelectedTutorial();
			this.SetDamageControlIcons();
			this.SetShipGroupMembershipString();
			this.heatTooltip.SetDelegate("BodyText", () => this.BuildHeatTooltip(this.selectedFriendlyShipState));
			this.batteryTooltip.SetDelegate("BodyText", () => this.BuildBatteryTooltip(this.selectedFriendlyShipState));
		}

		// Token: 0x0600639E RID: 25502 RVA: 0x002F1508 File Offset: 0x002EF708
		private void SetShipGroupMembershipString()
		{
			if (this.selectedFriendlyShip.controlGroups.Count > 0)
			{
				this.groupMembershipList.SetText(this.selectedFriendlyShip.GetGroupMembershipString());
				this.groupMembershipList.enabled = true;
				return;
			}
			this.groupMembershipList.enabled = false;
		}

		// Token: 0x0600639F RID: 25503 RVA: 0x002F1557 File Offset: 0x002EF757
		private void OnShipGroupChanged(CombatShipGroupChange e)
		{
			if (this.selectedFriendlyShipState == e.ship)
			{
				this.SetShipGroupMembershipString();
			}
			this.SetGroupSelectionButtons(false);
		}

		// Token: 0x060063A0 RID: 25504 RVA: 0x002F157C File Offset: 0x002EF77C
		public void UpdateFleetCommandPanel()
		{
			this.fleetCommandButtons.ForEach(delegate(Button x)
			{
				x.enabled = false;
			});
			this.fleetCommandButtons.ForEach(delegate(Button x)
			{
				x.image.enabled = false;
			});
			this.fleetCommandTooltips.ForEach(delegate(TooltipTrigger x)
			{
				x.enabled = false;
			});
			List<TISpaceShipState> shipsToRecieveFleetCommands = new List<TISpaceShipState>();
			shipsToRecieveFleetCommands = (from x in this.leftHandCombatants.Keys
				select x.GetCombatantState() as TISpaceShipState into y
				where y != null && !y.ShipDestroyed() && !y.hasDisengaged
				select y).ToList<TISpaceShipState>();
			if (shipsToRecieveFleetCommands.Count == 0 && this.fleetManeuverPanel.activeInHierarchy)
			{
				this.ToggleFleetManeuverPanel();
			}
			using (List<IFleetCommand>.Enumerator enumerator = ShipCommandsManager.fleetCommands.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IFleetCommand fleetCommand = enumerator.Current;
					if (fleetCommand.CommandVisibleToPlayer(shipsToRecieveFleetCommands))
					{
						Button button = this.fleetCommandButtons[fleetCommand.IconPosition()];
						TooltipTrigger tooltipTrigger = this.fleetCommandTooltips[fleetCommand.IconPosition()];
						if (button.transition == Selectable.Transition.SpriteSwap)
						{
							SpriteState spriteState = new SpriteState
							{
								highlightedSprite = this.commandIconCache[fleetCommand.GetTemplate().dataName].commandSprite_on,
								pressedSprite = this.commandIconCache[fleetCommand.GetTemplate().dataName].commandSprite_on
							};
							button.spriteState = spriteState;
						}
						button.image.sprite = this.commandIconCache[fleetCommand.GetTemplate().dataName].commandSprite_off;
						button.enabled = true;
						button.image.enabled = true;
						button.onClick.RemoveAllListeners();
						tooltipTrigger.enabled = true;
						tooltipTrigger.SetDelegate("BodyText", () => fleetCommand.GetTooltipText(false));
						if (fleetCommand.PlayerCanIssueCommand(shipsToRecieveFleetCommands))
						{
							button.interactable = true;
							if (fleetCommand.RequiresTarget())
							{
								button.onClick.AddListener(delegate
								{
									(fleetCommand as IFleetCommandWithTarget).InitiateTargeting(shipsToRecieveFleetCommands);
								});
							}
							else
							{
								button.onClick.AddListener(delegate
								{
									fleetCommand.OnExecuteFleetCommand(shipsToRecieveFleetCommands, null);
								});
							}
						}
						else
						{
							button.interactable = false;
						}
						button.image.color = (button.interactable ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.2f));
					}
				}
			}
		}

		// Token: 0x060063A1 RID: 25505 RVA: 0x002F18C0 File Offset: 0x002EFAC0
		public void PlayFleetCommandButtonAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteFleetCommand", false, false);
		}

		// Token: 0x060063A2 RID: 25506 RVA: 0x002F18CE File Offset: 0x002EFACE
		public void PlayFleetManuverCommandButtonAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteShipCommand", false, false);
		}

		// Token: 0x060063A3 RID: 25507 RVA: 0x002F18DC File Offset: 0x002EFADC
		public void SetEndBattleToggleSprite(EndCombatStanceChanged e)
		{
			this.SetEndBattleToggleSprite();
		}

		// Token: 0x060063A4 RID: 25508 RVA: 0x002F18E4 File Offset: 0x002EFAE4
		public void SetEndBattleToggleSprite()
		{
			if (!TIGameState.Valid(this.combatState))
			{
				return;
			}
			bool flag = this.combatState.votedEndCombat[this.leftHandFaction];
			bool flag2 = this.combatState.votedEndCombat[this.rightHandFaction];
			string text;
			string text2;
			if (flag)
			{
				if (flag2)
				{
					text = "ui_spacecombat/BUT_end_battle_RED_RED_off";
					text2 = "ui_spacecombat/BUT_end_battle_RED_RED_on";
				}
				else
				{
					text = "ui_spacecombat/BUT_end_battle_RED_GREEN_off";
					text2 = "ui_spacecombat/BUT_end_battle_RED_GREEN_on";
				}
			}
			else if (flag2)
			{
				text = "ui_spacecombat/BUT_end_battle_GREEN_RED_off";
				text2 = "ui_spacecombat/BUT_end_battle_GREEN_RED_on";
			}
			else
			{
				text = "ui_spacecombat/BUT_end_battle_GREEN_GREEN_off";
				text2 = "ui_spacecombat/BUT_end_battle_GREEN_GREEN_on";
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, this.endBattleButtonOffImage);
			Sprite sprite = GameControl.assetLoader.LoadAsset<Sprite>(text2);
			this.endBattleButton.spriteState = new SpriteState
			{
				highlightedSprite = sprite,
				pressedSprite = sprite
			};
		}

		// Token: 0x060063A5 RID: 25509 RVA: 0x002F19AC File Offset: 0x002EFBAC
		public void OnEndBattleToggleSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SetEndCombatVoteAction(base.activePlayer, !this.combatState.votedEndCombat[base.activePlayer]));
		}

		// Token: 0x060063A6 RID: 25510 RVA: 0x002F19FC File Offset: 0x002EFBFC
		public string EndBattleTooltip()
		{
			TIFactionState tifactionState = this.combatState.votedEndCombat.Keys.Single<TIFactionState>((TIFactionState x) => x != base.activePlayer);
			return new StringBuilder(Loc.T("UI.SpaceCombat.EndBattleButtonText")).AppendLine().Append(this.combatState.votedEndCombat[tifactionState] ? Loc.T("UI.SpaceCombat.EndBattleButtonTooltipYes") : Loc.T("UI.SpaceCombat.EndBattleButtonTooltipNo")).ToString();
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x002F1A72 File Offset: 0x002EFC72
		public void OnCombatEndTriggered(CombatEndTriggered e)
		{
			this.autoResolveButton.interactable = false;
			this.autoResolveButton.image.color = new Color(1f, 1f, 1f, 0.2f);
		}

		// Token: 0x060063A8 RID: 25512 RVA: 0x002F1AA9 File Offset: 0x002EFCA9
		public void OnAutoResolveBattleSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OptionSelect", false, false);
			TIInputManager.acceptingInput = false;
			this.autoResolveConfirmationPanel.SetActive(true);
			this._wasGamePlayingWhenAutoresolveOpened = !this.clockController.IsPaused;
			this.clockController.Pause();
		}

		// Token: 0x060063A9 RID: 25513 RVA: 0x002F1AE8 File Offset: 0x002EFCE8
		public void OnAutoResolveConfirmed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIInputManager.acceptingInput = true;
			this.combatMgr.EndCombatWithAutoresolve();
		}

		// Token: 0x060063AA RID: 25514 RVA: 0x002F1B07 File Offset: 0x002EFD07
		public void OnAutoResolveCanceled()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			TIInputManager.acceptingInput = true;
			this.autoResolveConfirmationPanel.SetActive(false);
			if (this._wasGamePlayingWhenAutoresolveOpened)
			{
				this.clockController.Play();
			}
		}

		// Token: 0x060063AB RID: 25515 RVA: 0x002F1B3A File Offset: 0x002EFD3A
		public string AutoResolveTooltip()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.AutoResolve"));
			stringBuilder.AppendLine(Loc.T("UI.SpaceCombat.AutoResolve.Description"));
			return stringBuilder.ToString();
		}

		// Token: 0x060063AC RID: 25516 RVA: 0x002F1B68 File Offset: 0x002EFD68
		public void ConfigureAutoResolvePanel()
		{
			this.autoResolveButton.interactable = true;
			this.autoResolveButton.image.color = new Color(1f, 1f, 1f, 1f);
			this.autoResolveQuery.text = Loc.T("UI.SpaceCombat.AutoResolve.Query");
			this.autoResolveConfirm.text = Loc.T("UI.SpaceCombat.AutoResolve");
			this.autoResolveCancel.text = Loc.T("UI.Notifications.Cancel");
		}

		// Token: 0x060063AD RID: 25517 RVA: 0x002F1BEC File Offset: 0x002EFDEC
		public string BuildHeatTooltip(TISpaceShipState state)
		{
			if (!TIGameState.Valid(state))
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Objectives.SpaceCombatCanvas.ShipDetailTemperature.Desc"));
			stringBuilder.AppendLine().AppendLine();
			stringBuilder.Append(Loc.T("UI.Objectives.FleetScreenCanvas.DesignDataShipHeatSink.Name"));
			stringBuilder.Append(Loc.T("UI.Fleets.FracGJ", new object[]
			{
				TIUtilities.FormatSmallNumber(state.accumulatedHeat_GJ, 1, 0, true, false),
				TIUtilities.FormatSmallNumber(state.currentHeatSinkCapacity_GJ, 1, 0, true, false)
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x002F1C7C File Offset: 0x002EFE7C
		public string BuildBatteryTooltip(TISpaceShipState state)
		{
			if (!TIGameState.Valid(state))
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Objectives.SpaceCombatCanvas.ShipDetailBattery.Desc"));
			stringBuilder.AppendLine().AppendLine();
			stringBuilder.Append(Loc.T("UI.Fleets.GJ", new object[] { Loc.T("UI.Fleets.TwoValues", new object[]
			{
				TIUtilities.FormatSmallNumber(state.availablePower_GJ + state.currentBatteryCharge_GJ, 1, 0, true, false),
				TIUtilities.FormatSmallNumber(state.AuxPowerRequriedStorage_GJ + state.CurrentBatteryCapacity_GJ(), 1, 0, true, false)
			}) }));
			return stringBuilder.ToString();
		}

		// Token: 0x060063AF RID: 25519 RVA: 0x002F1D1C File Offset: 0x002EFF1C
		public void OnMouseEnterShipList()
		{
			TIInputManager.blockCombatZoom = true;
		}

		// Token: 0x060063B0 RID: 25520 RVA: 0x002F1D24 File Offset: 0x002EFF24
		public void OnMouseExitShipList()
		{
			TIInputManager.blockCombatZoom = false;
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x002F1D2C File Offset: 0x002EFF2C
		public void OnMouseEnterDropDown()
		{
			TIInputManager.blockCombatZoom = true;
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x002F1D34 File Offset: 0x002EFF34
		public void SetGroupSelectionButtons(bool forceAllOff = false)
		{
			Dictionary<int, List<TISpaceShipState>> controlGroups = this.combatMgr.GetControlGroups();
			int i = 0;
			while (i <= 9)
			{
				if (forceAllOff || !controlGroups.ContainsKey(i))
				{
					goto IL_0048;
				}
				List<TISpaceShipState> list = controlGroups[i];
				if (list == null || list.Count <= 0)
				{
					goto IL_0048;
				}
				this.groupSelectionButtonObjects[i].SetActive(true);
				IL_005A:
				i++;
				continue;
				IL_0048:
				this.groupSelectionButtonObjects[i].SetActive(false);
				goto IL_005A;
			}
		}

		// Token: 0x060063B3 RID: 25523 RVA: 0x002F1DA4 File Offset: 0x002EFFA4
		public void OnControlGroupButtonPressed(int buttonIdx)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatFriendlyShipSelect", false, false);
			this.combatMgr.SelectControlGroup(buttonIdx);
		}

		// Token: 0x060063B4 RID: 25524 RVA: 0x002F1DC0 File Offset: 0x002EFFC0
		private void OpenFormationUI()
		{
			this.fleetCommandPanelObject.SetActive(false);
			this.fleetCommandTargetPanel.SetActive(false);
			this.friendlyShipListTransform.gameObject.SetActive(false);
			this.enemyShipListTransform.gameObject.SetActive(false);
			this.playerReinforcmentPanel.SetActive(false);
			this.enemyReinforcmentPanel.SetActive(false);
			this.openBattleLogButton.SetActive(false);
			this.battleLogCanvas.enabled = false;
			this.formationPatternDictionary = new Dictionary<string, int>();
			this.formationPatternReverseDictionary = new Dictionary<int, string>();
			this.formationConcentrationDropdown.ClearOptions();
			this.formationFocusDropdown.ClearOptions();
			this.formationPatternDropdown.ClearOptions();
			this.formationSpreadDropdown.ClearOptions();
			this.formationPatterns = TemplateManager.IterateByClass<TIFormationTemplate>(true).ToList<TIFormationTemplate>();
			for (int i = 0; i < this.formationPatterns.Count; i++)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(this.formationPatterns[i].displayName);
				this.formationPatternDropdown.options.Add(optionData);
				this.formationPatternDictionary.Add(this.formationPatterns[i].dataName, i);
				this.formationPatternReverseDictionary.Add(i, this.formationPatterns[i].dataName);
			}
			foreach (object obj in Enum.GetValues(typeof(FormationFocus)))
			{
				TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData(Formation.focusName((FormationFocus)obj));
				this.formationFocusDropdown.options.Add(optionData2);
			}
			foreach (object obj2 in Enum.GetValues(typeof(FormationConcentration)))
			{
				TMP_Dropdown.OptionData optionData3 = new TMP_Dropdown.OptionData(Formation.concentrationName((FormationConcentration)obj2));
				this.formationConcentrationDropdown.options.Add(optionData3);
			}
			foreach (object obj3 in Enum.GetValues(typeof(FormationSpacing)))
			{
				TMP_Dropdown.OptionData optionData4 = new TMP_Dropdown.OptionData(Formation.spacingName((FormationSpacing)obj3));
				this.formationSpreadDropdown.options.Add(optionData4);
			}
			TISpaceFleetState fleetState = this.leftHandFleetController.fleetState;
			if (fleetState != null && fleetState.savedFormation.patternDataName != null)
			{
				this.formationFocusDropdown.SetValueWithoutNotify((int)fleetState.savedFormation.focus);
				this.formationConcentrationDropdown.SetValueWithoutNotify((int)fleetState.savedFormation.concentration);
				this.formationSpreadDropdown.SetValueWithoutNotify((int)fleetState.savedFormation.spacing);
				this.formationPatternDropdown.value = this.formationPatternDictionary[fleetState.savedFormation.patternDataName];
			}
			else
			{
				this.formationPatternDropdown.value = this.formationPatternDictionary["Line"];
				this.formationFocusDropdown.value = (int)fleetState.formation.focus;
				this.formationConcentrationDropdown.value = (int)fleetState.formation.concentration;
				this.formationSpreadDropdown.value = (int)fleetState.formation.spacing;
			}
			this.formationPatternDropdown.RefreshShownValue();
			this.formationSpreadDropdown.RefreshShownValue();
			this.formationFocusDropdown.RefreshShownValue();
			this.formationConcentrationDropdown.RefreshShownValue();
			this.formationPatternDropdown.gameObject.SetActive(true);
			this.formationSpreadDropdown.gameObject.SetActive(true);
			this.formationFocusDropdown.gameObject.SetActive(true);
			this.formationConcentrationDropdown.gameObject.SetActive(true);
			this.formationDescription.gameObject.SetActive(true);
			this.formationTitle.SetText(Loc.T("UI.SpaceCombat.FormationTitle"));
			this.initialVelocityText.SetText(Loc.T("UI.SpaceCombat.InitialVelocity"));
			this.formationHabPlacementText.SetText(Loc.T("UI.SpaceCombat.FleetHabPlacement"));
			float magnitude = this.leftHandFleetController.activeShipControllers[0].velocityVector_kps.magnitude;
			this.formationInitialVelocitySlider.value = magnitude * 1000f;
			if (magnitude < 1f)
			{
				this.formationInitialVelocityValue.SetText(Loc.T("UI.SpaceCombat.velocity_mps", new object[] { (magnitude * 1000f).ToString("N0") }));
			}
			else
			{
				this.formationInitialVelocityValue.SetText(Loc.T("UI.SpaceCombat.velocity_kps", new object[] { magnitude.ToString("N2") }));
			}
			if (this.combatState.hab != null && this.combatState.hab.faction == this.leftHandFleetController.fleetState.ref_faction && this.combatState.hab.ActiveCombatModules().Count > 0)
			{
				this.formationHabPlacementPanel.SetActive(true);
			}
			else
			{
				this.formationHabPlacementPanel.SetActive(false);
			}
			this.SetFormation();
			this.formationPanelCanvas.enabled = true;
			this.InitializeFormationSelectionReinforcementList();
			this.ShowFormationTutorial();
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x002F2314 File Offset: 0x002F0514
		public void ConfirmFormation()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.UpdateCombatShipList(null);
			this.SetReinforcementReorderList(true);
			this.fleetCommandPanelObject.SetActive(true);
			this.fleetCommandTargetPanel.SetActive(true);
			this.friendlyShipListTransform.gameObject.SetActive(true);
			this.enemyShipListTransform.gameObject.SetActive(true);
			this.playerReinforcmentPanel.SetActive(true);
			this.enemyReinforcmentPanel.SetActive(true);
			this.openBattleLogButton.SetActive(true);
			TIInputManager.blockCombatZoom = false;
			GameControl.spaceCombat.TurnOffFormationSelectionMode();
			base.StartCoroutine(this.ShowMainTutorialDelayed());
			this.formationPanelCanvas.enabled = false;
			this.formationReinforcementSwapPanel.SetActive(false);
			MusicController.Instance.PlayFanfare(this.rightHandFaction.IsAlienFaction ? "event:/Music/Fanfares/trig_Combat_Aliens_Start" : "event:/Music/Fanfares/trig_Combat_Humans_Start");
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x002F23F4 File Offset: 0x002F05F4
		private void SetFormation()
		{
			Formation formation = new Formation(this.formationPatternReverseDictionary[this.formationPatternDropdown.value], (FormationFocus)this.formationFocusDropdown.value, (FormationSpacing)this.formationSpreadDropdown.value, (FormationConcentration)this.formationConcentrationDropdown.value);
			IList<CombatShipController> activeShipControllers = this.leftHandFleetController.activeShipControllers;
			this.leftHandFleetController.fleetState.faction.playerControl.StartAction(new SelectFleetFormationAction(this.leftHandFleetController.fleetState, formation, this.combatState, this.combatMgr.activeShips.Where<CombatShipController>((CombatShipController x) => x.faction == this.leftHandFleetController.fleetState.faction).Count<CombatShipController>(), activeShipControllers, true));
			this.formationDescription.SetText(this.leftHandFleetController.fleetState.formation.description);
			this.combatMgr.ArrangePlayerFleetInFormation(this.leftHandFleetController, this.formationHabPlacementToggle.isOn);
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x002F24E0 File Offset: 0x002F06E0
		private void SetFormationVelocity(float value_mps)
		{
			foreach (CombatShipController combatShipController in this.leftHandFleetController.activeShipControllers)
			{
				if (Mathf.Abs(value_mps) < 1E-45f)
				{
					value_mps = 0.3f;
				}
				float num = 0.001f * value_mps * 0.05f;
				switch (GameControl.spaceCombat.setup)
				{
				case CombatSetup.Confrontation:
					num = ((this.leftHandFleetController.FleetIndex == 0) ? num : (-num));
					break;
				case CombatSetup.Fleet0ChaseFleet1:
					num = ((this.leftHandFleetController.FleetIndex == 0) ? num : num);
					break;
				case CombatSetup.Fleet1ChaseFleet0:
					num = ((this.leftHandFleetController.FleetIndex == 0) ? (-num) : (-num));
					break;
				}
				combatShipController.SetInitialVelocityVector(new Vector3(0f, 0f, num));
			}
		}

		// Token: 0x060063B8 RID: 25528 RVA: 0x002F25C8 File Offset: 0x002F07C8
		public void OnFormationTemplateDropdownChanged()
		{
			this.SetFormation();
		}

		// Token: 0x060063B9 RID: 25529 RVA: 0x002F25D0 File Offset: 0x002F07D0
		public void OnFormationSpacingDropdownChanged()
		{
			this.SetFormation();
		}

		// Token: 0x060063BA RID: 25530 RVA: 0x002F25D8 File Offset: 0x002F07D8
		public void OnFormationConcentrationDropdownChanged()
		{
			this.SetFormation();
		}

		// Token: 0x060063BB RID: 25531 RVA: 0x002F25E0 File Offset: 0x002F07E0
		public void OnFormationFocusDropdownChanged()
		{
			this.SetFormation();
		}

		// Token: 0x060063BC RID: 25532 RVA: 0x002F25E8 File Offset: 0x002F07E8
		public void OnVelocitySliderValueChanged()
		{
			if (this.formationInitialVelocitySlider.value < 1000f)
			{
				this.formationInitialVelocityValue.SetText(Loc.T("UI.SpaceCombat.velocity_mps", new object[] { this.formationInitialVelocitySlider.value.ToString("N0") }));
				return;
			}
			this.formationInitialVelocityValue.SetText(Loc.T("UI.SpaceCombat.velocity_kps", new object[] { ((double)this.formationInitialVelocitySlider.value * 0.001).ToString("N2") }));
		}

		// Token: 0x060063BD RID: 25533 RVA: 0x002F267F File Offset: 0x002F087F
		public void OnFormationVelocityChanged()
		{
			this.SetFormationVelocity(this.formationInitialVelocitySlider.value);
		}

		// Token: 0x060063BE RID: 25534 RVA: 0x002F2692 File Offset: 0x002F0892
		public void ToggleFormationHabPlacement()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.SetFormation();
		}

		// Token: 0x060063BF RID: 25535 RVA: 0x002F26A8 File Offset: 0x002F08A8
		private void InitializeFormationSelectionReinforcementList()
		{
			this.formationSelectedShip1 = null;
			this.reinforcementSelectedShip = null;
			this.formationReinforcementShipList.SetListSize<FormationSelectionReinforcementSwapController>(this.leftHandFleetController.fleetState.ships.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.formationReinforcementShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__292.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__292.<>p__0 = CallSite<Func<CallSite, object, FormationSelectionReinforcementSwapController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FormationSelectionReinforcementSwapController), typeof(SpaceCombatCanvasController)));
					}
					FormationSelectionReinforcementSwapController formationSelectionReinforcementSwapController = SpaceCombatCanvasController.<>o__292.<>p__0.Target(SpaceCombatCanvasController.<>o__292.<>p__0, enumerator.Current);
					TISpaceShipState tispaceShipState = this.leftHandFleetController.fleetState.ships[num];
					formationSelectionReinforcementSwapController.SetListItem(tispaceShipState, this);
					num++;
				}
			}
			this.UpdateInteractableFormationReinforcementButtons();
		}

		// Token: 0x060063C0 RID: 25536 RVA: 0x002F2788 File Offset: 0x002F0988
		private void UpdateInteractableFormationReinforcementButtons()
		{
			using (IEnumerator<object> enumerator = this.formationReinforcementShipList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (SpaceCombatCanvasController.<>o__293.<>p__0 == null)
					{
						SpaceCombatCanvasController.<>o__293.<>p__0 = CallSite<Func<CallSite, object, FormationSelectionReinforcementSwapController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FormationSelectionReinforcementSwapController), typeof(SpaceCombatCanvasController)));
					}
					FormationSelectionReinforcementSwapController formationSelectionReinforcementSwapController = SpaceCombatCanvasController.<>o__293.<>p__0.Target(SpaceCombatCanvasController.<>o__293.<>p__0, enumerator.Current);
					formationSelectionReinforcementSwapController.HighlightButtonAfterSelection(this.reinforcementSelectedShip == formationSelectionReinforcementSwapController.ship);
					formationSelectionReinforcementSwapController.SetButtonInteractable(!this.combatMgr.combatantLookup.ContainsKey(formationSelectionReinforcementSwapController.ship));
				}
			}
		}

		// Token: 0x060063C1 RID: 25537 RVA: 0x002F2848 File Offset: 0x002F0A48
		private void OnShipSelectedDuringFormationSetting(ShipSelectedDuringFormationSetting e)
		{
			if (this.formationSelectedShip1 == null)
			{
				this.formationSelectedShip1 = e.ship;
				CombatShipController combatShipController = this.combatMgr.combatantLookup[this.formationSelectedShip1] as CombatShipController;
				if (this.reinforcementSelectedShip != null)
				{
					this.FormationSetting_SwapShipToReinforcements();
				}
				else
				{
					combatShipController.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.GreenSquare);
					combatShipController.UIController().maintainAnimation = true;
					combatShipController.ModelController.StartSelectionAnimation();
				}
			}
			else
			{
				this.combatMgr.Precombat_SwapShipPositions(this.combatMgr.combatantLookup[this.formationSelectedShip1] as CombatShipController, this.combatMgr.combatantLookup[e.ship] as CombatShipController);
				CombatShipController combatShipController2 = this.combatMgr.combatantLookup[this.formationSelectedShip1] as CombatShipController;
				combatShipController2.ModelController.StopSelectionAnimation();
				combatShipController2.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
				combatShipController2.UIController().maintainAnimation = false;
				this.formationSelectedShip1 = null;
			}
			this.UpdateInteractableFormationReinforcementButtons();
		}

		// Token: 0x060063C2 RID: 25538 RVA: 0x002F2958 File Offset: 0x002F0B58
		public void FormationSetting_SwapShipToReinforcements()
		{
			CombatShipController combatShipController = this.combatMgr.combatantLookup[this.formationSelectedShip1] as CombatShipController;
			this.combatMgr.Precombat_SwapShipToReinforcements(combatShipController, this.reinforcementSelectedShip);
			this.UpdateCombatShipList(null);
			bool flag = true;
			CombatFleetController combatFleetController = this.leftHandFleetController;
			this.UpdateReinforcementPanel(flag, (combatFleetController != null) ? combatFleetController.reinforcements : null, GameControl.spaceCombat.GetAvailableReinforcementsCount(this.leftHandFaction));
			combatShipController.ModelController.StopSelectionAnimation();
			combatShipController.ModelController.AssignAnimationToSelectionSprite(MarkerController.MarkerAnimations.CyanSquare);
			combatShipController.UIController().maintainAnimation = false;
			this.formationSelectedShip1 = null;
			this.reinforcementSelectedShip = null;
		}

		// Token: 0x060063C3 RID: 25539 RVA: 0x002F29F4 File Offset: 0x002F0BF4
		public void OnFormationSettingReinforcementShipSelected(TISpaceShipState ship)
		{
			this.reinforcementSelectedShip = ship;
			if (this.formationSelectedShip1 != null)
			{
				this.FormationSetting_SwapShipToReinforcements();
			}
			this.UpdateInteractableFormationReinforcementButtons();
		}

		// Token: 0x060063C4 RID: 25540 RVA: 0x002F2A17 File Offset: 0x002F0C17
		public void OpenBattleLog()
		{
			if (this.battleLogCanvas.enabled)
			{
				this.CloseBattleLog();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenLarge", false, false);
			this.battleLogCanvas.enabled = true;
			TIInputManager.blockCombatZoom = true;
		}

		// Token: 0x060063C5 RID: 25541 RVA: 0x002F2A4B File Offset: 0x002F0C4B
		public void CloseBattleLog()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.battleLogCanvas.enabled = false;
			TIInputManager.blockCombatZoom = false;
		}

		// Token: 0x04004613 RID: 17939
		public UITutorialController spaceCombatUITutorialController;

		// Token: 0x04004614 RID: 17940
		public UITutorialController spaceCombat_ShipSelectedUITutorialController;

		// Token: 0x04004615 RID: 17941
		public UITutorialController waypointTutorialController;

		// Token: 0x04004616 RID: 17942
		public UITutorialController formationTutorialController;

		// Token: 0x04004617 RID: 17943
		public Button endBattleButton;

		// Token: 0x04004618 RID: 17944
		public Image endBattleButtonOffImage;

		// Token: 0x04004619 RID: 17945
		public TooltipTrigger endBattleTooltip;

		// Token: 0x0400461A RID: 17946
		public GameObject fleetCommandPanelObject;

		// Token: 0x0400461B RID: 17947
		public GameObject fleetManeuverPanel;

		// Token: 0x0400461C RID: 17948
		public TooltipTrigger fleetManuverButtonTooltip;

		// Token: 0x0400461D RID: 17949
		public GameObject fleetCommandTargetPanel;

		// Token: 0x0400461E RID: 17950
		public TMP_Text fleetCommandsTargetText;

		// Token: 0x0400461F RID: 17951
		[Header("Autoresolve UI")]
		public Button autoResolveButton;

		// Token: 0x04004620 RID: 17952
		public TooltipTrigger autoResolveTooltip;

		// Token: 0x04004621 RID: 17953
		public GameObject autoResolveConfirmationPanel;

		// Token: 0x04004622 RID: 17954
		public TMP_Text autoResolveQuery;

		// Token: 0x04004623 RID: 17955
		public TMP_Text autoResolveConfirm;

		// Token: 0x04004624 RID: 17956
		public TMP_Text autoResolveCancel;

		// Token: 0x04004625 RID: 17957
		private bool _wasGamePlayingWhenAutoresolveOpened;

		// Token: 0x04004626 RID: 17958
		[Header("Top Bar")]
		public List<Button> fleetCommandButtons;

		// Token: 0x04004627 RID: 17959
		public List<TooltipTrigger> fleetCommandTooltips;

		// Token: 0x04004628 RID: 17960
		public List<GameObject> groupSelectionButtonObjects;

		// Token: 0x04004629 RID: 17961
		[Header("Ship")]
		public Canvas selectedShipPanel;

		// Token: 0x0400462A RID: 17962
		public Image selectedShipHull;

		// Token: 0x0400462B RID: 17963
		public Image selectedShipRadiators;

		// Token: 0x0400462C RID: 17964
		public Image selectedShipDrive;

		// Token: 0x0400462D RID: 17965
		public TMP_Text selectedShipName;

		// Token: 0x0400462E RID: 17966
		public ListManagerBase selectedShipNoseWeapons;

		// Token: 0x0400462F RID: 17967
		public ListManagerBase selectedShipHullWeapons;

		// Token: 0x04004630 RID: 17968
		private Dictionary<ModuleDataEntry, ShipWeaponUIController> weaponUIControllers;

		// Token: 0x04004631 RID: 17969
		public Image selectedShipCurrentBatteryCharge;

		// Token: 0x04004632 RID: 17970
		public Image selectedShipCurrentBatteryCapacity;

		// Token: 0x04004633 RID: 17971
		public Image selectedShipCurrentHeat;

		// Token: 0x04004634 RID: 17972
		public Image selectedShipCurrentHeatCapacity;

		// Token: 0x04004635 RID: 17973
		public Image selectedShipHeatIcon;

		// Token: 0x04004636 RID: 17974
		public Image selectedShipCoolingIcon;

		// Token: 0x04004637 RID: 17975
		public TMP_Text selectedShipPrimaryTargetText;

		// Token: 0x04004638 RID: 17976
		public List<Button> commandButtons;

		// Token: 0x04004639 RID: 17977
		public List<TooltipTrigger> commandTooltips;

		// Token: 0x0400463A RID: 17978
		public GameObject shipCommandPanelObject;

		// Token: 0x0400463B RID: 17979
		public GameObject shipManeuverPanel;

		// Token: 0x0400463C RID: 17980
		public TooltipTrigger shipManeuverButtonTooltip;

		// Token: 0x0400463D RID: 17981
		public TMP_Text commandsTargetText;

		// Token: 0x0400463E RID: 17982
		public Button commandSpinnerLeft;

		// Token: 0x0400463F RID: 17983
		public Button commandSpinnerRight;

		// Token: 0x04004640 RID: 17984
		public ListManagerBase maneuverList;

		// Token: 0x04004641 RID: 17985
		public ListManagerBase selectedShipPersonnelList;

		// Token: 0x04004642 RID: 17986
		public Image selectedShipHeatAlert;

		// Token: 0x04004643 RID: 17987
		public Image selectedShipBatteryAlert;

		// Token: 0x04004644 RID: 17988
		public Image selectedShipDeltaVAlert;

		// Token: 0x04004645 RID: 17989
		public TMP_Text currentDeltaVText;

		// Token: 0x04004646 RID: 17990
		public RectTransform currentDeltaVCoverImage;

		// Token: 0x04004647 RID: 17991
		private float currentDeltaVCoverMaxWidth;

		// Token: 0x04004648 RID: 17992
		public TooltipTrigger heatTooltip;

		// Token: 0x04004649 RID: 17993
		public TooltipTrigger batteryTooltip;

		// Token: 0x0400464A RID: 17994
		public TMP_Text groupMembershipList;

		// Token: 0x0400464B RID: 17995
		private bool shipCommandsDataDirty;

		// Token: 0x0400464C RID: 17996
		private bool fleetCommandsDataDirty;

		// Token: 0x0400464D RID: 17997
		public TMP_Text accelerationText;

		// Token: 0x0400464E RID: 17998
		public TMP_Text velocityText;

		// Token: 0x0400464F RID: 17999
		private float batteryYBottom;

		// Token: 0x04004650 RID: 18000
		private float selectedShipCurrentBatteryCharge_y;

		// Token: 0x04004651 RID: 18001
		private float selectedShipCurrentBatteryCapacity_y;

		// Token: 0x04004652 RID: 18002
		private float batteryYRange;

		// Token: 0x04004653 RID: 18003
		private float heatYBottom;

		// Token: 0x04004654 RID: 18004
		private float selectedShipCurrentHeat_y;

		// Token: 0x04004655 RID: 18005
		private float selectedShipCurrentHeatCapacity_y;

		// Token: 0x04004656 RID: 18006
		private float HeatYRange;

		// Token: 0x04004657 RID: 18007
		public TooltipTrigger deltaVTooltip;

		// Token: 0x04004658 RID: 18008
		public GridLayoutGroup masterDamageGridGroup;

		// Token: 0x04004659 RID: 18009
		private Dictionary<Vector2Int, SpaceCombatDamageGridItemController> masterDamageGridControllers;

		// Token: 0x0400465A RID: 18010
		private Dictionary<ModuleDataEntry, SpaceCombatDamageGridItemController> moduleDamageGrid;

		// Token: 0x0400465B RID: 18011
		private Dictionary<ShipSystem, SpaceCombatDamageGridItemController> systemDamageGrid;

		// Token: 0x0400465C RID: 18012
		[HideInInspector]
		public CombatShipController selectedFriendlyShip;

		// Token: 0x0400465D RID: 18013
		[HideInInspector]
		public List<CombatShipController> groupSelectedFriendlyShips;

		// Token: 0x0400465E RID: 18014
		private AccelerationConstraints _groupConstraints;

		// Token: 0x0400465F RID: 18015
		public ListManagerBase friendlyShipList;

		// Token: 0x04004660 RID: 18016
		public ListManagerBase enemyShipList;

		// Token: 0x04004661 RID: 18017
		public RectTransform friendlyShipListTransform;

		// Token: 0x04004662 RID: 18018
		public RectTransform enemyShipListTransform;

		// Token: 0x04004663 RID: 18019
		public SpaceCombatSpeedController clockController;

		// Token: 0x04004664 RID: 18020
		public TISpaceShipState selectedFriendlyShipState;

		// Token: 0x04004665 RID: 18021
		[Header("Reinforcements")]
		public GameObject playerReinforcmentPanel;

		// Token: 0x04004666 RID: 18022
		public GameObject enemyReinforcmentPanel;

		// Token: 0x04004667 RID: 18023
		public Button playerReinforcmentButton;

		// Token: 0x04004668 RID: 18024
		public GameObject enemyReinforcmentButtonGO;

		// Token: 0x04004669 RID: 18025
		public TMP_Text playerReinforcementTotal;

		// Token: 0x0400466A RID: 18026
		public TMP_Text playerReinforcementReadyCount;

		// Token: 0x0400466B RID: 18027
		public TMP_Text enemyReinforcementQty;

		// Token: 0x0400466C RID: 18028
		public TooltipTrigger playerReinforcementTooltip;

		// Token: 0x0400466D RID: 18029
		public TooltipTrigger enemyReinforcementTooltip;

		// Token: 0x0400466E RID: 18030
		public TMP_Text playerReinforcementEntryText;

		// Token: 0x0400466F RID: 18031
		public TMP_Text aiReinforcementEntryText;

		// Token: 0x04004670 RID: 18032
		public TMP_Text playerReinforcementTimerText;

		// Token: 0x04004671 RID: 18033
		public TMP_Text aiReinforcementTimerText;

		// Token: 0x04004672 RID: 18034
		private float ReinforcementEntryTextTime = 10f;

		// Token: 0x04004673 RID: 18035
		private float playerReinforcementEntryTextTimer = 10f;

		// Token: 0x04004674 RID: 18036
		private float aiReinforcementEntryTextTimer = 10f;

		// Token: 0x04004675 RID: 18037
		public GameObject reinforcementReorderPanel;

		// Token: 0x04004676 RID: 18038
		public ListManagerBase reinforcementReorderList;

		// Token: 0x04004677 RID: 18039
		public TMP_Text reinforcementReorderPanelHeaderText;

		// Token: 0x04004678 RID: 18040
		public GameObject openReinforcementsReorderPanelButtonObject;

		// Token: 0x04004679 RID: 18041
		public GameObject closeReinforcementsReorderPanelButtonObject;

		// Token: 0x0400467A RID: 18042
		private Dictionary<string, SpaceCombatCanvasController.CommandIconCacheItem> commandIconCache;

		// Token: 0x0400467B RID: 18043
		public CombatFleetController leftHandFleetController;

		// Token: 0x0400467C RID: 18044
		public CombatFleetController rightHandFleetController;

		// Token: 0x0400467D RID: 18045
		private TIFactionState leftHandFaction;

		// Token: 0x0400467E RID: 18046
		private TIFactionState rightHandFaction;

		// Token: 0x0400467F RID: 18047
		private bool isDisplayingGroupCommands;

		// Token: 0x04004680 RID: 18048
		private bool isManeuverPanelOpen;

		// Token: 0x04004681 RID: 18049
		private bool isFleetManeuverPanelOpen;

		// Token: 0x04004682 RID: 18050
		private bool isFleetCommandCardPanelOpen = true;

		// Token: 0x04004683 RID: 18051
		private bool control;

		// Token: 0x04004684 RID: 18052
		[Header("Formation Selection")]
		public GameObject formationPanel;

		// Token: 0x04004685 RID: 18053
		public Canvas formationPanelCanvas;

		// Token: 0x04004686 RID: 18054
		public TMP_Text formationTitle;

		// Token: 0x04004687 RID: 18055
		public TMP_Dropdown formationPatternDropdown;

		// Token: 0x04004688 RID: 18056
		public TMP_Dropdown formationFocusDropdown;

		// Token: 0x04004689 RID: 18057
		public TMP_Dropdown formationConcentrationDropdown;

		// Token: 0x0400468A RID: 18058
		public TMP_Dropdown formationSpreadDropdown;

		// Token: 0x0400468B RID: 18059
		public TMP_Text formationDescription;

		// Token: 0x0400468C RID: 18060
		public TMP_Text initialVelocityText;

		// Token: 0x0400468D RID: 18061
		public Slider formationInitialVelocitySlider;

		// Token: 0x0400468E RID: 18062
		public TMP_Text formationInitialVelocityValue;

		// Token: 0x0400468F RID: 18063
		public GameObject formationHabPlacementPanel;

		// Token: 0x04004690 RID: 18064
		public TMP_Text formationHabPlacementText;

		// Token: 0x04004691 RID: 18065
		public Toggle formationHabPlacementToggle;

		// Token: 0x04004692 RID: 18066
		public Button confirmFormationButton;

		// Token: 0x04004693 RID: 18067
		public TMP_Text confirmFormationButtonText;

		// Token: 0x04004694 RID: 18068
		public TMP_Text shipSwapInstructionsText;

		// Token: 0x04004695 RID: 18069
		public GameObject formationReinforcementSwapPanel;

		// Token: 0x04004696 RID: 18070
		public ListManagerBase formationReinforcementShipList;

		// Token: 0x04004697 RID: 18071
		public TISpaceShipState formationSelectedShip1;

		// Token: 0x04004698 RID: 18072
		public TISpaceShipState reinforcementSelectedShip;

		// Token: 0x04004699 RID: 18073
		private List<TIFormationTemplate> formationPatterns = new List<TIFormationTemplate>();

		// Token: 0x0400469A RID: 18074
		private Dictionary<string, int> formationPatternDictionary;

		// Token: 0x0400469B RID: 18075
		private Dictionary<int, string> formationPatternReverseDictionary;

		// Token: 0x0400469C RID: 18076
		[Header("Battle Log")]
		public BattleLogController battleLogController;

		// Token: 0x0400469D RID: 18077
		public GameObject openBattleLogButton;

		// Token: 0x0400469E RID: 18078
		public Canvas battleLogCanvas;

		// Token: 0x040046A0 RID: 18080
		private const int ManeuverPanelIndex = 11;

		// Token: 0x040046A1 RID: 18081
		private const int SpecialManeuverPanelIndex = 21;

		// Token: 0x040046A2 RID: 18082
		public Dictionary<TISpaceShipState, ReinforcementReorderListItemController> reinforcementListItems;

		// Token: 0x020013A7 RID: 5031
		public enum ChangeCommandScopeMode
		{
			// Token: 0x0400726F RID: 29295
			JustThisShip,
			// Token: 0x04007270 RID: 29296
			AllShipsInGroup,
			// Token: 0x04007271 RID: 29297
			AllShipsOfClass,
			// Token: 0x04007272 RID: 29298
			AllShipsInFleet
		}

		// Token: 0x020013A8 RID: 5032
		private struct CommandIconCacheItem
		{
			// Token: 0x0600920C RID: 37388 RVA: 0x00348476 File Offset: 0x00346676
			public CommandIconCacheItem(bool colorChange, Sprite commandSprite_on, Sprite commandSprite_off)
			{
				this.colorChange = colorChange;
				this.commandSprite_on = commandSprite_on;
				this.commandSprite_off = commandSprite_off;
			}

			// Token: 0x04007273 RID: 29299
			public bool colorChange;

			// Token: 0x04007274 RID: 29300
			public Sprite commandSprite_on;

			// Token: 0x04007275 RID: 29301
			public Sprite commandSprite_off;
		}
	}
}
