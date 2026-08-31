using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000902 RID: 2306
	public class OptionsMenuController : MenuController
	{
		// Token: 0x0600583F RID: 22591 RVA: 0x00287908 File Offset: 0x00285B08
		private void OnEnable()
		{
			this.isInitializing = true;
			this.LoadLocalizedText();
			OptionsMenuController.optionsMenuOpen = true;
			this.waypointAngleSnapDropdown.value = TIPlayerProfileManager.GetIntByKey("WaypointSnapAngleIndex", this.waypointAngleSnapDropdown.value);
			this.maxShipsInCombatSlider.maxValue = (float)TemplateManager.global.maxShipsAllowedInCombat;
			this.maxShipsInCombatSlider.value = (float)TIPlayerProfileManager.GetIntByKey("MaxShipsInCombat", (int)this.maxShipsInCombatSlider.value);
			this.tooltipDelayPrimarySlider.value = TIPlayerProfileManager.GetFloatByKey("TooltipDelayPrimary", this.tooltipDelayPrimarySlider.value) * 20f;
			this.tooltipDelaySupplementalSlider.value = TIPlayerProfileManager.GetFloatByKey("TooltipDelaySupplemental", this.tooltipDelaySupplementalSlider.value) * 20f;
			this.languageSelection.options.Clear();
			this.saveChanges = false;
			this.customCursorToggle.isOn = TIPlayerProfileManager.usingWindowsCursor;
			this.missionPhaseSummaryStartsOpen.isOn = TIPlayerProfileManager.missionPhaseReportStartOpen;
			this.unpauseAfterMissionAssignment.isOn = TIPlayerProfileManager.unpauseAfterMissionAssignment;
			this.alertSpaceTimerToggle.isOn = TIPlayerProfileManager.alertSpaceTimerNotifications;
			this.monthlyIncomeToggle.isOn = TIPlayerProfileManager.showMonthlyIncomes;
			this.compressSavesToggle.isOn = TIPlayerProfileManager.compressSaves;
			this.displaySystemClockToggle.isOn = TIPlayerProfileManager.displaySystemClock;
			this.assignmentPhaseCouncilorCameraFocusToggle.isOn = TIPlayerProfileManager.assignmentPhaseCouncilorCameraFocus;
			this.cycleNextCouncilorWhenAssigningMissionsToggle.isOn = TIPlayerProfileManager.cycleNextCouncilorWhenAssigningMissions;
			this.showHighSpeedOrbitTrailsToggle.isOn = TIPlayerProfileManager.showHighSpeedOrbitTrails;
			this.showEarthLightsToggle.isOn = TIPlayerProfileManager.showEarthLights;
			this.saveChanges = true;
			if (GameControl.initialized && GameControl.control != null)
			{
				if (TIGlobalValuesState.isTutorialActive)
				{
					this.resetTutorialButton.SetActive(true);
				}
				else
				{
					this.resetTutorialButton.SetActive(false);
				}
			}
			this.SetLanguageDropdownOptions();
			this.isInitializing = false;
		}

		// Token: 0x06005840 RID: 22592 RVA: 0x00287ADC File Offset: 0x00285CDC
		public void OnDisable()
		{
			OptionsMenuController.optionsMenuOpen = false;
		}

		// Token: 0x06005841 RID: 22593 RVA: 0x00287AE4 File Offset: 0x00285CE4
		private void Start()
		{
			if (this.inGame)
			{
				base.GetComponent<CanvasGroup>().blocksRaycasts = true;
				base.GetComponent<CanvasGroup>().interactable = true;
			}
		}

		// Token: 0x06005842 RID: 22594 RVA: 0x00287B08 File Offset: 0x00285D08
		public void LoadLocalizedText()
		{
			if (this.optionsHeader != null)
			{
				this.optionsHeader.SetText(Loc.T("UI.Options.Settings"));
			}
			if (this.graphicsHeader != null)
			{
				this.graphicsHeader.SetText(Loc.T("UI.Options.Graphics"));
			}
			if (this.controlsHeader != null)
			{
				this.controlsHeader.SetText(Loc.T("UI.Options.Controls"));
			}
			if (this.audioHeader != null)
			{
				this.audioHeader.SetText(Loc.T("UI.Options.Audio"));
			}
			if (this.gameplayHeader != null)
			{
				this.gameplayHeader.SetText(Loc.T("UI.Options.Gameplay"));
			}
			if (this.languageTitle != null)
			{
				this.languageTitle.SetText(Loc.T("UI.Options.Language"));
			}
			if (this.waypointSnapAngleTitle != null)
			{
				this.waypointSnapAngleTitle.SetText(Loc.T("UI.Options.WaypointAngleSnap"));
			}
			if (this.cursorToggleTitle != null)
			{
				this.cursorToggleTitle.SetText(Loc.T("UI.Options.DefaultCursor"));
			}
			if (this.missionPhaseReportToggleTitle != null)
			{
				this.missionPhaseReportToggleTitle.SetText(Loc.T("UI.Options.MissionPhaseSummary"));
			}
			if (this.unpauseAfterMissionAssignmentTitle != null)
			{
				this.unpauseAfterMissionAssignmentTitle.SetText(Loc.T("UI.Options.UnpauseAfterMissionAssignments"));
			}
			if (this.maxShipsInCombatTitle != null)
			{
				this.maxShipsInCombatTitle.SetText(Loc.T("UI.Options.MaxShipsInCombat"));
			}
			if (this.tooltipDelayPrimaryTitle != null)
			{
				this.tooltipDelayPrimaryTitle.SetText(Loc.T("UI.Options.TooltipDelayPrimary"));
			}
			if (this.tooltipDelaySupplementalTitle != null)
			{
				this.tooltipDelaySupplementalTitle.SetText(Loc.T("UI.Options.TooltipDelaySupplemental"));
			}
			if (this.alertSpaceTimerTitle != null)
			{
				this.alertSpaceTimerTitle.SetText(Loc.T("UI.Options.AlertSpaceTimer"));
			}
			if (this.toggleMonthlyIncomeTimerTitle != null)
			{
				this.toggleMonthlyIncomeTimerTitle.SetText(Loc.T("UI.Options.ToggleMonthlyIncomes"));
			}
			if (this.toggleCompressSavesTitle != null)
			{
				this.toggleCompressSavesTitle.SetText(Loc.T("UI.Options.CompressSaves"));
			}
			if (this.toggleDisplaySystemClockTitle != null)
			{
				this.toggleDisplaySystemClockTitle.SetText(Loc.T("UI.Options.SystemClock"));
			}
			if (this.assignmentPhaseCouncilorCameraFocusToggleTitle != null)
			{
				this.assignmentPhaseCouncilorCameraFocusToggleTitle.SetText(Loc.T("UI.Options.AssignmentPhaseCameraFocus"));
			}
			if (this.cycleNextCouncilorWhenAssigningMissionsToggleTitle != null)
			{
				this.cycleNextCouncilorWhenAssigningMissionsToggleTitle.SetText(Loc.T("UI.Options.AssignmentPhaseCouncilorCycle"));
			}
			if (this.showHighSpeedOrbitTrailsToggleTitle != null)
			{
				this.showHighSpeedOrbitTrailsToggleTitle.SetText(Loc.T("UI.Options.ShowHighSpeedOrbitTrails"));
			}
			if (this.showEarthLightsToggleTitle != null)
			{
				this.showEarthLightsToggleTitle.SetText(Loc.T("UI.Options.ShowEarthLights"));
			}
			if (this.maxShipsInCombatValue != null)
			{
				TMP_Text tmp_Text = this.maxShipsInCombatValue;
				Slider slider = this.maxShipsInCombatSlider;
				tmp_Text.text = ((slider != null) ? slider.value.ToString() : null);
			}
		}

		// Token: 0x06005843 RID: 22595 RVA: 0x00287E28 File Offset: 0x00286028
		public void HandleLanguageSelectionInput(int index)
		{
			for (int i = 0; i < this._templates.Count; i++)
			{
				if (i == index)
				{
					Loc.SetLanguage(this._templates[i].dataName);
					TIPlayerProfileManager.SavePlayerConfig();
					this.SetLanguageDropdownOptions();
					return;
				}
			}
		}

		// Token: 0x06005844 RID: 22596 RVA: 0x00287E74 File Offset: 0x00286074
		private void SetLanguageDropdownOptions()
		{
			this.languageSelection.options.Clear();
			TILocalizationTemplate[] allTemplates = TemplateManager.GetAllTemplates<TILocalizationTemplate>(true);
			for (int i = 0; i < allTemplates.Length; i++)
			{
				if (allTemplates[i].active)
				{
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
					{
						text = allTemplates[i].displayNameCurrentForStartScreen()
					};
					this.languageSelection.options.Add(optionData);
					this._templates.Add(allTemplates[i]);
					if (allTemplates[i].dataName == Loc.CurrentLanguage)
					{
						this.languageSelection.SetValueWithoutNotify(i);
						this.languageSelection.captionText.text = allTemplates[i].displayNameCurrentForStartScreen();
					}
				}
			}
		}

		// Token: 0x06005845 RID: 22597 RVA: 0x00287F1C File Offset: 0x0028611C
		public void ChangedWaypointAngleSnap(bool dirty = false)
		{
			TIPlayerProfileManager.waypointAngleSnap = int.Parse(this.waypointAngleSnapDropdown.options[this.waypointAngleSnapDropdown.value].text);
			TIPlayerProfileManager.waypointAngleSnapIndex = this.waypointAngleSnapDropdown.value;
			if (this.initSnapAngle)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
			this.initSnapAngle = true;
		}

		// Token: 0x06005846 RID: 22598 RVA: 0x00287F78 File Offset: 0x00286178
		public void ChangedCursorSetting()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.usingWindowsCursor = this.customCursorToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
			if (this.customCursorToggle.isOn)
			{
				TIInputManager.SetCursor(null, false);
				return;
			}
			TIInputManager.SetDefaultCursor(true);
		}

		// Token: 0x06005847 RID: 22599 RVA: 0x00287FD1 File Offset: 0x002861D1
		public void ChangedMissionPhaseReportSetting()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.missionPhaseReportStartOpen = this.missionPhaseSummaryStartsOpen.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005848 RID: 22600 RVA: 0x00288004 File Offset: 0x00286204
		public void ChangedUnpauseAfterMissionAssignmentSetting()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.unpauseAfterMissionAssignment = this.unpauseAfterMissionAssignment.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005849 RID: 22601 RVA: 0x00288038 File Offset: 0x00286238
		public void ChangedAlertSpaceTimerSetting()
		{
			TIPlayerProfileManager.alertSpaceTimerNotifications = this.alertSpaceTimerToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
			if (GameControl.control.activePlayer != null)
			{
				GameControl.control.activePlayer.alertSpaceTimerNotifications = this.alertSpaceTimerToggle.isOn;
			}
		}

		// Token: 0x0600584A RID: 22602 RVA: 0x00288090 File Offset: 0x00286290
		public void ChangedMonthlyIncomeSetting()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.showMonthlyIncomes = this.monthlyIncomeToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
			if (GameControl.control.activePlayer != null)
			{
				GameControl.control.activePlayer.showMonthlyIncomesInTopBarAndIntel = this.monthlyIncomeToggle.isOn;
				GameControl.eventManager.TriggerEvent(new FactionResourcesUpdated(GameControl.control.activePlayer), null, new object[] { GameControl.control.activePlayer });
			}
		}

		// Token: 0x0600584B RID: 22603 RVA: 0x00288128 File Offset: 0x00286328
		public void ChangedCompressSavesSetting()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.compressSaves = this.compressSavesToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
			if (this.loadMenuController != null)
			{
				this.loadMenuController.saveList.PopulateList();
			}
			if (this.saveMenuController != null)
			{
				this.saveMenuController.saveList.PopulateList();
			}
			if (this.startMenuController != null)
			{
				this.startMenuController.RefreshContinueButton();
			}
		}

		// Token: 0x0600584C RID: 22604 RVA: 0x002881BC File Offset: 0x002863BC
		public void ChangedMaxShipsInCombat()
		{
			this.maxShipsInCombatValue.text = this.maxShipsInCombatSlider.value.ToString();
			TIPlayerProfileManager.maxShipsInCombat = (int)this.maxShipsInCombatSlider.value;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x0600584D RID: 22605 RVA: 0x00288208 File Offset: 0x00286408
		public void ChangedTooltipDelayPrimary()
		{
			this.tooltipDelayPrimaryValue.text = (this.tooltipDelayPrimarySlider.value * 0.05f).ToString();
			TIPlayerProfileManager.tooltipDelayPrimary = this.tooltipDelayPrimarySlider.value * 0.05f;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x0600584E RID: 22606 RVA: 0x0028825C File Offset: 0x0028645C
		public void ChangedTooltipDelaySupplemental()
		{
			this.tooltipDelaySupplementalValue.text = (this.tooltipDelaySupplementalSlider.value * 0.05f).ToString();
			TIPlayerProfileManager.tooltipDelaySupplemental = this.tooltipDelaySupplementalSlider.value * 0.05f;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x0600584F RID: 22607 RVA: 0x002882B0 File Offset: 0x002864B0
		public void ChangedDisplaySystemClock()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.displaySystemClock = this.displaySystemClockToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005850 RID: 22608 RVA: 0x002882E3 File Offset: 0x002864E3
		public void ChangedAssignmentPhaseCameraFocus()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.assignmentPhaseCouncilorCameraFocus = this.assignmentPhaseCouncilorCameraFocusToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005851 RID: 22609 RVA: 0x00288316 File Offset: 0x00286516
		public void ChangedAssignmentPhaseCouncilorCycle()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.cycleNextCouncilorWhenAssigningMissions = this.cycleNextCouncilorWhenAssigningMissionsToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005852 RID: 22610 RVA: 0x00288349 File Offset: 0x00286549
		public void ChangedShowHighSpeedOrbitTrails()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.showHighSpeedOrbitTrails = this.showHighSpeedOrbitTrailsToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005853 RID: 22611 RVA: 0x0028837C File Offset: 0x0028657C
		public void ChangedShowEarthLights()
		{
			if (!this.isInitializing)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			}
			TIPlayerProfileManager.showEarthLights = this.showEarthLightsToggle.isOn;
			if (this.saveChanges)
			{
				TIPlayerProfileManager.SavePlayerConfig();
			}
		}

		// Token: 0x06005854 RID: 22612 RVA: 0x002883AF File Offset: 0x002865AF
		public void PlayOptionsToggleAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x06005855 RID: 22613 RVA: 0x002883BD File Offset: 0x002865BD
		public void PlayOptionsTabAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_SwitchTabInTabbedPane", false, true);
		}

		// Token: 0x04003FDB RID: 16347
		public LoadMenuController loadMenuController;

		// Token: 0x04003FDC RID: 16348
		public SaveMenuController saveMenuController;

		// Token: 0x04003FDD RID: 16349
		public StartMenuController startMenuController;

		// Token: 0x04003FDE RID: 16350
		public GameObject resetTutorialButton;

		// Token: 0x04003FDF RID: 16351
		public Slider maxShipsInCombatSlider;

		// Token: 0x04003FE0 RID: 16352
		public Slider tooltipDelayPrimarySlider;

		// Token: 0x04003FE1 RID: 16353
		public Slider tooltipDelaySupplementalSlider;

		// Token: 0x04003FE2 RID: 16354
		public TMP_Dropdown languageSelection;

		// Token: 0x04003FE3 RID: 16355
		public TMP_Dropdown waypointAngleSnapDropdown;

		// Token: 0x04003FE4 RID: 16356
		[Header("Toggles")]
		public Toggle missionPhaseSummaryStartsOpen;

		// Token: 0x04003FE5 RID: 16357
		public Toggle customCursorToggle;

		// Token: 0x04003FE6 RID: 16358
		public Toggle unpauseAfterMissionAssignment;

		// Token: 0x04003FE7 RID: 16359
		public Toggle alertSpaceTimerToggle;

		// Token: 0x04003FE8 RID: 16360
		public Toggle monthlyIncomeToggle;

		// Token: 0x04003FE9 RID: 16361
		public Toggle compressSavesToggle;

		// Token: 0x04003FEA RID: 16362
		public Toggle displaySystemClockToggle;

		// Token: 0x04003FEB RID: 16363
		public Toggle assignmentPhaseCouncilorCameraFocusToggle;

		// Token: 0x04003FEC RID: 16364
		public Toggle cycleNextCouncilorWhenAssigningMissionsToggle;

		// Token: 0x04003FED RID: 16365
		public Toggle showHighSpeedOrbitTrailsToggle;

		// Token: 0x04003FEE RID: 16366
		public Toggle showEarthLightsToggle;

		// Token: 0x04003FEF RID: 16367
		[Header("Text Labels")]
		public TMP_Text languageTitle;

		// Token: 0x04003FF0 RID: 16368
		public TMP_Text waypointSnapAngleTitle;

		// Token: 0x04003FF1 RID: 16369
		public TMP_Text cursorToggleTitle;

		// Token: 0x04003FF2 RID: 16370
		public TMP_Text missionPhaseReportToggleTitle;

		// Token: 0x04003FF3 RID: 16371
		public TMP_Text unpauseAfterMissionAssignmentTitle;

		// Token: 0x04003FF4 RID: 16372
		public TMP_Text maxShipsInCombatTitle;

		// Token: 0x04003FF5 RID: 16373
		public TMP_Text tooltipDelayPrimaryTitle;

		// Token: 0x04003FF6 RID: 16374
		public TMP_Text tooltipDelaySupplementalTitle;

		// Token: 0x04003FF7 RID: 16375
		public TMP_Text alertSpaceTimerTitle;

		// Token: 0x04003FF8 RID: 16376
		public TMP_Text toggleMonthlyIncomeTimerTitle;

		// Token: 0x04003FF9 RID: 16377
		public TMP_Text toggleCompressSavesTitle;

		// Token: 0x04003FFA RID: 16378
		public TMP_Text toggleDisplaySystemClockTitle;

		// Token: 0x04003FFB RID: 16379
		public TMP_Text assignmentPhaseCouncilorCameraFocusToggleTitle;

		// Token: 0x04003FFC RID: 16380
		public TMP_Text cycleNextCouncilorWhenAssigningMissionsToggleTitle;

		// Token: 0x04003FFD RID: 16381
		public TMP_Text showHighSpeedOrbitTrailsToggleTitle;

		// Token: 0x04003FFE RID: 16382
		public TMP_Text showEarthLightsToggleTitle;

		// Token: 0x04003FFF RID: 16383
		public TMP_Text optionsHeader;

		// Token: 0x04004000 RID: 16384
		public TMP_Text graphicsHeader;

		// Token: 0x04004001 RID: 16385
		public TMP_Text controlsHeader;

		// Token: 0x04004002 RID: 16386
		public TMP_Text audioHeader;

		// Token: 0x04004003 RID: 16387
		public TMP_Text gameplayHeader;

		// Token: 0x04004004 RID: 16388
		public TMP_Text difficultyValue;

		// Token: 0x04004005 RID: 16389
		public TMP_Text maxShipsInCombatValue;

		// Token: 0x04004006 RID: 16390
		public TMP_Text tooltipDelayPrimaryValue;

		// Token: 0x04004007 RID: 16391
		public TMP_Text tooltipDelaySupplementalValue;

		// Token: 0x04004008 RID: 16392
		public bool inGame;

		// Token: 0x04004009 RID: 16393
		private bool initSnapAngle;

		// Token: 0x0400400A RID: 16394
		public bool saveChanges = true;

		// Token: 0x0400400B RID: 16395
		public static bool optionsMenuOpen;

		// Token: 0x0400400C RID: 16396
		private List<TILocalizationTemplate> _templates = new List<TILocalizationTemplate>();

		// Token: 0x0400400D RID: 16397
		private bool isInitializing = true;
	}
}
