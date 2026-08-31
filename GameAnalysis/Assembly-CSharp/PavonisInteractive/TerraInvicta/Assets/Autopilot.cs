using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.Assets
{
	// Token: 0x020009C2 RID: 2498
	public class Autopilot : MonoBehaviour
	{
		// Token: 0x06005E15 RID: 24085 RVA: 0x002CBA8E File Offset: 0x002C9C8E
		public static void Pause()
		{
			if (!(Autopilot.Singleton == null))
			{
				Autopilot.Singleton.Activated = false;
				return;
			}
			GameTimeManager existingManager = World.Active.GetExistingManager<GameTimeManager>();
			if (existingManager == null)
			{
				return;
			}
			existingManager.Pause();
		}

		// Token: 0x1700102E RID: 4142
		// (get) Token: 0x06005E16 RID: 24086 RVA: 0x002CBABD File Offset: 0x002C9CBD
		// (set) Token: 0x06005E17 RID: 24087 RVA: 0x002CBAC8 File Offset: 0x002C9CC8
		public bool Activated
		{
			get
			{
				return this.activated;
			}
			set
			{
				if (this.activated == value)
				{
					return;
				}
				this.activated = value;
				if (this.activated)
				{
					TIFactionState activePlayer = GameControl.control.activePlayer;
					for (int i = 0; i < 3; i++)
					{
						activePlayer.SetResearchPriority(i, 1);
						activePlayer.SetResearchPriority(i + 3, 0);
					}
					return;
				}
				GameTimeManager gameTimeManager = this.gameTimeManager;
				if (gameTimeManager == null)
				{
					return;
				}
				gameTimeManager.Pause();
			}
		}

		// Token: 0x06005E18 RID: 24088 RVA: 0x002CBB28 File Offset: 0x002C9D28
		private void Start()
		{
			Autopilot.Singleton = this;
			this.gameTimeManager = World.Active.GetExistingManager<GameTimeManager>();
			this.CanvasManager = World.Active.GetExistingManager<CanvasManager>();
			this.notificationScreenController = this.CanvasManager.Notifications.GameObject.GetComponent<NotificationScreenController>();
			this.councilorMissionCanvasController = GameControl.control._canvasStack.CouncilorMissionController.GameObject.GetComponent<CouncilorMissionCanvasController>();
			this.researchScreenController = this.CanvasManager.GetInfoScreen<ResearchScreenController>();
			this.precombatController = this.CanvasManager.PrecombatControllerCanvas as PrecombatController;
			GameControl.eventManager.AddListener<MissionPhaseStart>(new EventManager.EventDelegate<MissionPhaseStart>(this.OnStartMissionPhase), null, null, false, false);
			Application.logMessageReceived += this.LogCallback;
		}

		// Token: 0x06005E19 RID: 24089 RVA: 0x002CBBE8 File Offset: 0x002C9DE8
		private void Update()
		{
			if (!this.Activated || TIFrameCounter.FrameCount % 30 != 0 || this.CanvasManager.OptionsScreen.Visible())
			{
				return;
			}
			TIFactionState activePlayer = GameControl.control.activePlayer;
			if (this.notificationScreenController.singleAlertBox.activeInHierarchy)
			{
				while (this.IsOkayButtonClickable || this.IsCloseButtonClickable || this.IsGotoButtonClickable)
				{
					if (this.IsOkayButtonClickable)
					{
						this.notificationScreenController.OkayButtonPressed();
					}
					else if (this.IsCloseButtonClickable)
					{
						this.notificationScreenController.CloseButtonPressed();
					}
					else if (this.IsGotoButtonClickable)
					{
						this.notificationScreenController.GotoButtonPressed();
					}
				}
				Button button = this.notificationScreenController.optionButtons.FirstOrDefault<Button>(new Func<Button, bool>(this.IsButtonClickable));
				if (button != null)
				{
					int num = this.notificationScreenController.optionButtons.IndexOf(button);
					this.notificationScreenController.OnOptionButtonPressed(num);
				}
			}
			if (this.notificationScreenController.masterPolicyPanelObject.activeInHierarchy)
			{
				this.notificationScreenController.PolicySelected(new CancelOption());
				this.notificationScreenController.OnConfirmPolicy();
			}
			if (this.notificationScreenController.responsePanelObject.activeInHierarchy)
			{
				this.notificationScreenController.OnResponseDecline();
			}
			if (this.notificationScreenController.callAllyResponseObject.activeInHierarchy)
			{
				this.notificationScreenController.DeclineWarButton();
			}
			if (this.notificationScreenController.factionDiplomacyGreetingContinueButton.gameObject.activeInHierarchy)
			{
				this.notificationScreenController.OnDiplomacyGreetingContinueButton();
			}
			if (this.notificationScreenController.diplomacyController.gameObject.activeInHierarchy)
			{
				this.notificationScreenController.DiplomacyClose();
			}
			if (this.notificationScreenController.removeArmiesPromptObject.activeInHierarchy)
			{
				this.notificationScreenController.removeArmies_GoHomePressed();
			}
			if (this.IsButtonClickable(this.councilorMissionCanvasController.confirmAssignmentsButton))
			{
				this.councilorMissionCanvasController.PlayerConfirmsMissionAssignments();
			}
			for (int i = 0; i < 3; i++)
			{
				if (TIPromptQueueState.HasPromptStatic(activePlayer, GameStateManager.GlobalResearch(), null, "PromptSelectTech", i))
				{
					TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(TIGlobalResearchState.AvailableTechs().SelectRandomItem<TITechTemplate>().dataName, false);
					activePlayer.playerControl.StartAction(new SelectTechAction(activePlayer, i, titechTemplate));
				}
			}
			if (this.researchScreenController.Canvas.enabled)
			{
				this.researchScreenController.CloseInfoScreen(false);
			}
			if (this.precombatController.Canvas.enabled)
			{
				if (this.IsButtonClickable(this.precombatController.closeButton.GetComponentInChildren<Button>()))
				{
					this.precombatController.CloseResolveSelected();
				}
				else if (this.IsButtonClickable(this.precombatController.rejectAutoresolveButton.GetComponentInChildren<Button>()))
				{
					this.precombatController.OnAcceptAutoresolveSelected();
				}
				else if (this.IsButtonClickable(this.precombatController.postCombatCloseButton.GetComponentInChildren<Button>()))
				{
					this.precombatController.OnClosePostCombatButtonSelected();
				}
				else if (this.precombatController.combat != null)
				{
					Dictionary<TIFactionState, CombatStance> stances = this.precombatController.combat.stances;
					if (!stances.ContainsKey(activePlayer) || stances[activePlayer] == CombatStance.NotYetSet)
					{
						this.precombatController.StanceSubmit(2);
					}
					else if (this.IsButtonClickable(this.precombatController.autoResolveButton.GetComponentInChildren<Button>()))
					{
						this.precombatController.AutoresolveSelected();
					}
				}
			}
			if (this.IsButtonClickable(OperationCanvasController.Singleton.changeTrajectoryCancelButton))
			{
				OperationCanvasController.Singleton.OnCancelTrajectoryChange();
			}
			if (TIPromptQueueState.HasPromptStatic(activePlayer, activePlayer, null, "PromptDropOrgs", 0))
			{
				TIOrgState tiorgState = activePlayer.unassignedOrgs.First<TIOrgState>((TIOrgState x) => x.template.allowedOnMarket);
				activePlayer.playerControl.StartAction(new SellOrgAction(tiorgState, activePlayer, null));
			}
			if (!this.gameTimeManager.DontCapSpeed)
			{
				TIUtilities.GotoGameState(GameStateManager.Earth(), true, true, true, true, false, -1f);
			}
			if (!this.gameTimeManager.IsTimeFlowing)
			{
				this.gameTimeManager.IncreaseSpeed();
				this.gameTimeManager.DecreaseSpeed();
				this.gameTimeManager.TogglePause();
			}
			this.gameTimeManager.SetSpeed(6, false);
		}

		// Token: 0x06005E1A RID: 24090 RVA: 0x002CBFE3 File Offset: 0x002CA1E3
		private void LogCallback(string condition, string stackTrace, LogType type)
		{
			if (!this.Activated || type != LogType.Exception || this.IgnoreExeptions)
			{
				return;
			}
			Debug.Log("An Exception occurred. Stopping autopilot.");
			this.activated = false;
			this.gameTimeManager.Pause();
		}

		// Token: 0x06005E1B RID: 24091 RVA: 0x002CC016 File Offset: 0x002CA216
		private bool IsButtonClickable(Button button)
		{
			return button.gameObject.activeInHierarchy && button.enabled && button.interactable && button.GetComponentInParent<Canvas>().enabled;
		}

		// Token: 0x06005E1C RID: 24092 RVA: 0x002CC044 File Offset: 0x002CA244
		private void OnStartMissionPhase(MissionPhaseStart e)
		{
			if (!this.Activated)
			{
				return;
			}
			if (this.CycleIndex % this.SaveRate == 0)
			{
				string text = string.Concat(new string[]
				{
					"AP ",
					this.CycleIndex.ToString(),
					" - ",
					TITimeState.Now().ToCustomDateString(),
					" - v",
					Application.version
				});
				GameStateManager.SaveAllGameStates(TIUtilities.GetSaveFilePath(text), false);
				string text2 = "Created autopilot save named " + text;
				Log.Debug(text2, Array.Empty<object>());
				TIFactionState.LogAI(text2, false);
			}
			this.CycleIndex++;
		}

		// Token: 0x1700102F RID: 4143
		// (get) Token: 0x06005E1D RID: 24093 RVA: 0x002CC0E7 File Offset: 0x002CA2E7
		private bool IsOkayButtonClickable
		{
			get
			{
				return this.IsButtonClickable(this.notificationScreenController.okayButton);
			}
		}

		// Token: 0x17001030 RID: 4144
		// (get) Token: 0x06005E1E RID: 24094 RVA: 0x002CC0FA File Offset: 0x002CA2FA
		private bool IsCloseButtonClickable
		{
			get
			{
				return this.IsButtonClickable(this.notificationScreenController.closeButton);
			}
		}

		// Token: 0x17001031 RID: 4145
		// (get) Token: 0x06005E1F RID: 24095 RVA: 0x002CC10D File Offset: 0x002CA30D
		private bool IsGotoButtonClickable
		{
			get
			{
				return this.IsButtonClickable(this.notificationScreenController.gotoButton);
			}
		}

		// Token: 0x04004339 RID: 17209
		public static Autopilot Singleton;

		// Token: 0x0400433A RID: 17210
		private GameTimeManager gameTimeManager;

		// Token: 0x0400433B RID: 17211
		private CanvasManager CanvasManager;

		// Token: 0x0400433C RID: 17212
		private NotificationScreenController notificationScreenController;

		// Token: 0x0400433D RID: 17213
		private CouncilorMissionCanvasController councilorMissionCanvasController;

		// Token: 0x0400433E RID: 17214
		private ResearchScreenController researchScreenController;

		// Token: 0x0400433F RID: 17215
		private PrecombatController precombatController;

		// Token: 0x04004340 RID: 17216
		private bool activated;

		// Token: 0x04004341 RID: 17217
		public int CycleIndex;

		// Token: 0x04004342 RID: 17218
		public int SaveRate = 4;

		// Token: 0x04004343 RID: 17219
		public bool IgnoreExeptions;
	}
}
