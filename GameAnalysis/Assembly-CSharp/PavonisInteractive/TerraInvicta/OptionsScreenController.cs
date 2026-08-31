using System;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008BB RID: 2235
	public class OptionsScreenController : CanvasControllerBase, ICanvas
	{
		// Token: 0x0600555A RID: 21850 RVA: 0x0026D044 File Offset: 0x0026B244
		public override void Initialize()
		{
			base.Initialize();
			base.enabled = true;
			this.menu = base.GetComponent<MenuManager>();
			this.optionsHeaderText.SetText(Loc.T("UI.Options.Settings"));
			this.saveGameText.SetText(Loc.T("UI.Options.SaveGame"));
			this.loadGameText.SetText(Loc.T("UI.Options.LoadGame"));
			this.settingsText.SetText(Loc.T("UI.Options.Settings"));
			this.exitToMainMenuText.SetText(Loc.T("UI.Options.ExitToMainMenu"));
			this.exitGameText.SetText(Loc.T("UI.Options.ExitGame"));
			this.backtoGameText.SetText(Loc.T("UI.Options.BackToGame"));
			TMP_Text tmp_Text = this.codexButtonText;
			if (tmp_Text != null)
			{
				tmp_Text.SetText(Loc.T("UI.Codex.Title"));
			}
			this.mainMenuObject.SetActive(true);
			this.loadMenuObject.SetActive(true);
			this.saveMenuObject.SetActive(true);
			this.settingsMenuObject.SetActive(true);
			this.exitWithoutSaveWarningObject.SetActive(false);
			this.optionsSettingsHeader.SetText(Loc.T("UI.Options.Settings"));
			this.optionsVideoHeader.SetText(Loc.T("UI.Options.Video"));
			this.optionsGraphicsHeader.SetText(Loc.T("UI.Options.Graphics"));
			this.optionsAudioHeader.SetText(Loc.T("UI.Options.Audio"));
			this.optionsGameplayHeader.SetText(Loc.T("UI.Options.Gameplay"));
			this.optionsControlsHeader.SetText(Loc.T("UI.Options.Controls"));
			this.optionsNotificationsHeader.SetText(Loc.T("UI.Options.Notifications"));
			this.crashHeaderText.SetText(Loc.T("UI.Options.CrashHeader"));
			this.crashMainText.SetText(Loc.T("UI.Options.CrashDesc"));
			this.crashCloseButtonText.SetText(Loc.T("UI.Options.CrashCloseGame"));
			this.crashLogFolderButtonText.SetText(Loc.T("UI.Options.CrashLogFolder"));
			this.crashSaveFolderButtonText.SetText(Loc.T("UI.Options.CrashSaveFolder"));
			this.discordLinkText.SetText(Loc.T("UI.Options.DiscordCrashReports"));
			this.emailLinkText.SetText(Loc.T("UI.Options.SupportEmail"));
			this.emailLabelText.SetText(TIUtilities.CombineStrings(new string[]
			{
				Loc.T("UI.Options.EmailLabel"),
				" ",
				"support@pavonisinteractive.com"
			}));
			this.moddingText.SetText(TIUtilities.RedLine(Loc.T("UI.Options.CrashModding")));
			this.versionText.SetText(Application.version);
			int num = TemplateManager.global.quotes;
			this.quotes = new string[num];
			for (int i = 0; i < num; i++)
			{
				this.quotes[i] = Loc.T(new StringBuilder("UI.Options.Quote").Append(i.ToString()).ToString());
			}
			SaveMenuController.Singleton = this.optionsMenuController.saveMenuController;
		}

		// Token: 0x0600555B RID: 21851 RVA: 0x0026D334 File Offset: 0x0026B534
		public override void Show()
		{
			base.Show();
			if (!base.gameTime.Paused)
			{
				this.bankedPause = true;
				base.gameTime.Pause();
			}
			this.saveGameButton.interactable = !SaveMenuController.SavingIsBlocked() && !GameControl.control.skirmishMode;
			if (TIGlobalValuesState.isSpaceCombatEnabled || GameControl.control.skirmishMode)
			{
				this.mainSaveGameButton.interactable = false;
			}
			else
			{
				this.mainSaveGameButton.interactable = true;
			}
			this.quoteText.SetText(this.quotes.SelectRandomItem<string>());
			this.menu.ShowMenu(this.menu.startMenu);
			this.difficultyText.SetText(new StringBuilder(Loc.T("UI.Options.DifficultyLabel")).Append(" ").Append(Loc.T("UI.Options.Difficulty" + GameStateManager.GlobalValues().difficulty.ToString())).Append(GameStateManager.GlobalValues().scenarioCustomizations.customDifficulty ? Loc.T("UI.Options.DifficultyCustom") : ""));
		}

		// Token: 0x0600555C RID: 21852 RVA: 0x0026D452 File Offset: 0x0026B652
		public override void Hide()
		{
			base.Hide();
			this.menu.HideMenu();
			if (this.bankedPause)
			{
				base.gameTime.Play();
				this.bankedPause = false;
			}
			CoroutineDummy.Singleton.UnpauseAll();
		}

		// Token: 0x0600555D RID: 21853 RVA: 0x0026D489 File Offset: 0x0026B689
		public void OnReturnPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_UnPause", false, false);
			this.Hide();
			TIInputManager.acceptingInput = true;
		}

		// Token: 0x0600555E RID: 21854 RVA: 0x0026D4A4 File Offset: 0x0026B6A4
		public void ExitToMainMenu()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_UnPause", false, false);
			base.gameTime.Pause();
			this.fullExiting = false;
			if (GameControl.control.skirmishMode)
			{
				this.Hide();
				base.enabled = false;
				GameControl.control.viewMgr.GotoView(ViewType.MainMenu);
				return;
			}
			if (!SaveMenuController.SavingIsBlocked())
			{
				this.ShowExitConfirmation(false);
				return;
			}
			this.ShowExitWarning();
		}

		// Token: 0x0600555F RID: 21855 RVA: 0x0026D50E File Offset: 0x0026B70E
		public void PlayMenuButtonAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		}

		// Token: 0x06005560 RID: 21856 RVA: 0x0026D51C File Offset: 0x0026B71C
		public void PlayMenuCloseAudio()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x06005561 RID: 21857 RVA: 0x0026D52C File Offset: 0x0026B72C
		public void ShowExceptionDialog(string message, string exception)
		{
			this.Show();
			this.moddingText.gameObject.SetActive(TIPlayerProfileManager.useMods || (TIGlobalValuesState.GlobalValues != null && TIGlobalValuesState.GlobalValues.moddingUsedAnytime));
			this.crashExceptionText.SetText(TIUtilities.CombineStrings(new string[] { message, "\n", exception }));
			this.crashPanel.SetActive(true);
		}

		// Token: 0x06005562 RID: 21858 RVA: 0x0026D5A5 File Offset: 0x0026B7A5
		public void ShowSaveFolder()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			TIUtilities.OpenFileSystemURL(CreateSaveFileScrollList.GetSaveFolderPath());
		}

		// Token: 0x06005563 RID: 21859 RVA: 0x0026D5BD File Offset: 0x0026B7BD
		public void ShowLogFolder()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			TIUtilities.OpenFileSystemURL(Application.persistentDataPath);
		}

		// Token: 0x06005564 RID: 21860 RVA: 0x0026D5D5 File Offset: 0x0026B7D5
		public void OnClickDiscordLink()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			TIUtilities.OpenWebURL("https://discord.gg/QnnSnj32bS");
		}

		// Token: 0x06005565 RID: 21861 RVA: 0x0026D5ED File Offset: 0x0026B7ED
		public void OnClickEmailLink()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			GUIUtility.systemCopyBuffer = "support@pavonisinteractive.com";
			TIUtilities.OpenWebURL("mailto:support@pavonisinteractive.com");
		}

		// Token: 0x06005566 RID: 21862 RVA: 0x0026D60F File Offset: 0x0026B80F
		public void ExitGameWithException()
		{
			Debug.Log("Closing Terra Invicta due to exception");
			Application.Quit();
		}

		// Token: 0x06005567 RID: 21863 RVA: 0x0026D620 File Offset: 0x0026B820
		public void ExitGame()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_UnPause", false, false);
			base.gameTime.Pause();
			this.fullExiting = true;
			if (GameControl.control.skirmishMode)
			{
				Debug.Log("Closing Terra Invicta");
				Application.Quit();
				return;
			}
			if (!SaveMenuController.SavingIsBlocked())
			{
				this.ShowExitConfirmation(true);
				return;
			}
			this.ShowExitWarning();
		}

		// Token: 0x06005568 RID: 21864 RVA: 0x0026D67C File Offset: 0x0026B87C
		private void ShowExitConfirmation(bool fullExit)
		{
			if (fullExit)
			{
				this.exitWithoutSaveWarningText.SetText(Loc.T("UI.Options.QuitGameWarning"));
				this.exitWithoutSaveConfirm.SetText(Loc.T("UI.Options.QuitGame"));
			}
			else
			{
				this.exitWithoutSaveWarningText.SetText(Loc.T("UI.Options.QuitToMainMenuWarning"));
				this.exitWithoutSaveConfirm.SetText(Loc.T("UI.Options.QuitToMainMenu"));
			}
			this.exitWithoutSaveCancel.SetText(Loc.T("UI.Options.Cancel"));
			this.exitWithoutSaveWarningObject.SetActive(true);
			this.exitWarningMask.SetActive(true);
		}

		// Token: 0x06005569 RID: 21865 RVA: 0x0026D710 File Offset: 0x0026B910
		private void ShowExitWarning()
		{
			this.exitWithoutSaveWarningText.SetText(Loc.T("UI.Options.SaveWarnCantSaveOnExit"));
			this.exitWithoutSaveConfirm.SetText(Loc.T("UI.Options.QuitAnyway"));
			this.exitWithoutSaveCancel.SetText(Loc.T("UI.Options.Cancel"));
			this.exitWithoutSaveWarningObject.SetActive(true);
			this.exitWarningMask.SetActive(true);
		}

		// Token: 0x0600556A RID: 21866 RVA: 0x0026D774 File Offset: 0x0026B974
		public void OnCancelExit()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.exitWithoutSaveWarningObject.SetActive(false);
			this.exitWarningMask.SetActive(false);
		}

		// Token: 0x0600556B RID: 21867 RVA: 0x0026D79C File Offset: 0x0026B99C
		public void OnConfirmExit()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			if (this.fullExiting)
			{
				if (!SaveMenuController.SavingIsBlocked() && !TIGlobalValuesState.isSpaceCombatEnabled)
				{
					GameStateManager.SaveAllGameStates(StartMenuController.exitSaveFilePath, true);
				}
				Debug.Log("Closing Terra Invicta");
				Application.Quit();
				return;
			}
			if (!SaveMenuController.SavingIsBlocked() && !TIGlobalValuesState.isSpaceCombatEnabled)
			{
				GameStateManager.SaveAllGameStates(StartMenuController.exitSaveFilePath, true);
			}
			AudioManager.StopAllEvents();
			this.Hide();
			base.enabled = false;
			GameControl.control.viewMgr.GotoView(ViewType.MainMenu);
		}

		// Token: 0x0600556C RID: 21868 RVA: 0x0026D823 File Offset: 0x0026BA23
		public void OnCodexOpen()
		{
			CodexController.ShowCodexPanel("codex_welcome");
		}

		// Token: 0x0600556D RID: 21869 RVA: 0x0026D82F File Offset: 0x0026BA2F
		public override void OnDestroy()
		{
			base.OnDestroy();
		}

		// Token: 0x0600556E RID: 21870 RVA: 0x0026D837 File Offset: 0x0026BA37
		private void OnSkyboxChanged()
		{
		}

		// Token: 0x04003B92 RID: 15250
		private MenuManager menu;

		// Token: 0x04003B93 RID: 15251
		public OptionsMenuController optionsMenuController;

		// Token: 0x04003B94 RID: 15252
		public TMP_Text optionsHeaderText;

		// Token: 0x04003B95 RID: 15253
		public TMP_Text saveGameText;

		// Token: 0x04003B96 RID: 15254
		public TMP_Text loadGameText;

		// Token: 0x04003B97 RID: 15255
		public TMP_Text settingsText;

		// Token: 0x04003B98 RID: 15256
		public TMP_Text exitToMainMenuText;

		// Token: 0x04003B99 RID: 15257
		public TMP_Text exitGameText;

		// Token: 0x04003B9A RID: 15258
		public TMP_Text backtoGameText;

		// Token: 0x04003B9B RID: 15259
		public TMP_Text quoteText;

		// Token: 0x04003B9C RID: 15260
		public TMP_Text codexButtonText;

		// Token: 0x04003B9D RID: 15261
		public TMP_Text difficultyText;

		// Token: 0x04003B9E RID: 15262
		public TMP_Text versionText;

		// Token: 0x04003B9F RID: 15263
		public GameObject mainMenuObject;

		// Token: 0x04003BA0 RID: 15264
		public GameObject loadMenuObject;

		// Token: 0x04003BA1 RID: 15265
		public GameObject saveMenuObject;

		// Token: 0x04003BA2 RID: 15266
		public GameObject settingsMenuObject;

		// Token: 0x04003BA3 RID: 15267
		public Button exitToMainMenuButton;

		// Token: 0x04003BA4 RID: 15268
		public Button loadGameButton;

		// Token: 0x04003BA5 RID: 15269
		public Button saveGameButton;

		// Token: 0x04003BA6 RID: 15270
		public Button mainSaveGameButton;

		// Token: 0x04003BA7 RID: 15271
		public Button mainLoadGameButton;

		// Token: 0x04003BA8 RID: 15272
		public GameObject exitWithoutSaveWarningObject;

		// Token: 0x04003BA9 RID: 15273
		public GameObject exitWarningMask;

		// Token: 0x04003BAA RID: 15274
		public TMP_Text exitWithoutSaveWarningText;

		// Token: 0x04003BAB RID: 15275
		public TMP_Text exitWithoutSaveConfirm;

		// Token: 0x04003BAC RID: 15276
		public TMP_Text exitWithoutSaveCancel;

		// Token: 0x04003BAD RID: 15277
		public TMP_Text optionsSettingsHeader;

		// Token: 0x04003BAE RID: 15278
		public TMP_Text optionsVideoHeader;

		// Token: 0x04003BAF RID: 15279
		public TMP_Text optionsGraphicsHeader;

		// Token: 0x04003BB0 RID: 15280
		public TMP_Text optionsAudioHeader;

		// Token: 0x04003BB1 RID: 15281
		public TMP_Text optionsGameplayHeader;

		// Token: 0x04003BB2 RID: 15282
		public TMP_Text optionsControlsHeader;

		// Token: 0x04003BB3 RID: 15283
		public TMP_Text optionsNotificationsHeader;

		// Token: 0x04003BB4 RID: 15284
		public GameObject crashPanel;

		// Token: 0x04003BB5 RID: 15285
		public TMP_Text crashHeaderText;

		// Token: 0x04003BB6 RID: 15286
		public TMP_Text crashMainText;

		// Token: 0x04003BB7 RID: 15287
		public TMP_Text crashExceptionText;

		// Token: 0x04003BB8 RID: 15288
		public TMP_Text crashCloseButtonText;

		// Token: 0x04003BB9 RID: 15289
		public TMP_Text crashSaveFolderButtonText;

		// Token: 0x04003BBA RID: 15290
		public TMP_Text crashLogFolderButtonText;

		// Token: 0x04003BBB RID: 15291
		public TMP_Text discordLinkText;

		// Token: 0x04003BBC RID: 15292
		public TMP_Text emailLinkText;

		// Token: 0x04003BBD RID: 15293
		public TMP_Text emailLabelText;

		// Token: 0x04003BBE RID: 15294
		public TMP_Text moddingText;

		// Token: 0x04003BBF RID: 15295
		public string[] quotes;

		// Token: 0x04003BC0 RID: 15296
		private bool bankedPause;

		// Token: 0x04003BC1 RID: 15297
		private bool fullExiting;

		// Token: 0x04003BC2 RID: 15298
		public TMP_Text graphics_SkyboxOption;
	}
}
