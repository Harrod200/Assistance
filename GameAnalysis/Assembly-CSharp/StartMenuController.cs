using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Modding;
using PavonisInteractive.TerraInvicta.Systems.Bootstrap;
using PavonisInteractive.TerraInvicta.Tasks;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// Token: 0x0200041C RID: 1052
public class StartMenuController : MonoBehaviour
{
	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001574 RID: 5492 RVA: 0x00069B09 File Offset: 0x00067D09
	private TIFactionTemplate selectedFaction
	{
		get
		{
			return TemplateManager.Find<TIFactionTemplate>(this.selectedFactionDataName, false);
		}
	}

	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001575 RID: 5493 RVA: 0x00069B17 File Offset: 0x00067D17
	public static string continueSaveFilepath
	{
		get
		{
			return TIUtilities.GetMostRecentSave();
		}
	}

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001576 RID: 5494 RVA: 0x00069B1E File Offset: 0x00067D1E
	public static string exitSaveFilePath
	{
		get
		{
			return TIUtilities.GetSaveFilePath("ExitSave");
		}
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001577 RID: 5495 RVA: 0x00069B2A File Offset: 0x00067D2A
	public static string autoSaveFilepath
	{
		get
		{
			return TIUtilities.GetSaveFilePath("Autosave");
		}
	}

	// Token: 0x1700032B RID: 811
	// (get) Token: 0x06001578 RID: 5496 RVA: 0x00069B36 File Offset: 0x00067D36
	public static string oldAutoSaveFilepath
	{
		get
		{
			return TIUtilities.GetSaveFilePath("Autosave2");
		}
	}

	// Token: 0x1700032C RID: 812
	// (get) Token: 0x06001579 RID: 5497 RVA: 0x00069B42 File Offset: 0x00067D42
	public static string oldestAutoSaveFilepath
	{
		get
		{
			return TIUtilities.GetSaveFilePath("Autosave3");
		}
	}

	// Token: 0x1700032D RID: 813
	// (get) Token: 0x0600157A RID: 5498 RVA: 0x00069B4E File Offset: 0x00067D4E
	public static string quickSaveFilepath
	{
		get
		{
			return TIUtilities.GetSaveFilePath("Quicksave");
		}
	}

	// Token: 0x1700032E RID: 814
	// (get) Token: 0x0600157B RID: 5499 RVA: 0x00069B5A File Offset: 0x00067D5A
	public static string combatAutoSaveFilepath
	{
		get
		{
			return TIUtilities.GetSaveFilePath("CombatAutosave");
		}
	}

	// Token: 0x0600157C RID: 5500 RVA: 0x00069B66 File Offset: 0x00067D66
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
	private static void RuntimeInit()
	{
		QualitySettings.vSyncCount = 1;
		Application.targetFrameRate = 75;
	}

	// Token: 0x0600157D RID: 5501 RVA: 0x00069B78 File Offset: 0x00067D78
	private void Start()
	{
		this.EntryPoint();
		TIUtilities.SetMainThread(Thread.CurrentThread);
		Application.targetFrameRate = 75;
		this.sceneManager = StartScreenInstaller.container.Resolve<SceneManager>();
		this.loadingScreen.SetActive(false);
		this.tutorial = false;
		this.tutorialToggle.isOn = this.tutorial;
		this.SetLanguage();
		this.Initialize();
		if (StartMenuController.forceCredits)
		{
			base.gameObject.GetComponent<MenuManager>().ShowMenu(this.creditsMenu);
			StartMenuController.forceCredits = false;
		}
		if (this.bankedModFailure)
		{
			this.ShowModFailureDialog(Loc.T(this.bankedModWarningHeaderLoc), Loc.T(this.bankedModWarningDescLoc, new object[] { this.bankedModWarningLocArg1, this.bankedModWarningLocArg2 }));
		}
		if (TIPlayerProfileManager.loadingFailureDueToMods)
		{
			this.ShowModFailureDialog(Loc.T("UI.StartScreen.TemplateLoadingFailureHeader"), Loc.T("UI.StartScreen.TemplateLoadingFailureDesc"));
			TIPlayerProfileManager.loadingFailureDueToMods = false;
			TIPlayerProfileManager.SavePlayerConfig();
		}
		Mood.SetState(Mood.State.TRIN_Menu);
		this.UpdateUIScaling();
		RenderSettings.skybox = TIUtilities.assetLoader.LoadAsset<Material>(TemplateManager.global.skyboxes[TIPlayerProfileManager.skyboxVariant]);
	}

	// Token: 0x0600157E RID: 5502 RVA: 0x00069C98 File Offset: 0x00067E98
	public void UpdateUIScaling()
	{
		base.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, (float)TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting]);
		this.newGamePrimaryPanelTransform.anchoredPosition = new Vector2((float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 940) ? 0 : (-20)), (float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 1000) ? (-180) : (-100)));
		this.moddingPrimaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 1000) ? (-180) : (-150)));
		this.creditsPrimaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 1000) ? (-180) : (-110)));
		this.skirmishPrimaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 1000) ? (-180) : (-145)));
	}

	// Token: 0x0600157F RID: 5503 RVA: 0x00069DC4 File Offset: 0x00067FC4
	public void AdjustCampaignSettingsMenuOffsetWithScaling(bool open)
	{
		if (open && TIUtilities.GetScreenRatio() < 1.7f && TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] < 1030)
		{
			this.newGamePrimaryPanelTransform.anchoredPosition = new Vector2(-100f, (float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 1000) ? (-180) : (-100)));
			return;
		}
		this.newGamePrimaryPanelTransform.anchoredPosition = new Vector2((float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 940) ? 0 : (-20)), (float)((TemplateManager.global.uiScaleValues[TIPlayerProfileManager.uiScaleSetting] >= 1000) ? (-180) : (-100)));
	}

	// Token: 0x06001580 RID: 5504 RVA: 0x00069E7C File Offset: 0x0006807C
	private void EntryPoint()
	{
	}

	// Token: 0x06001581 RID: 5505 RVA: 0x00069E7E File Offset: 0x0006807E
	private void OnDestroy()
	{
		Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
	}

	// Token: 0x06001582 RID: 5506 RVA: 0x00069E91 File Offset: 0x00068091
	public TIMetaTemplate GetSelectedScenarioMetaTemplate()
	{
		if (this.selectedMetaTemplateScenario != null)
		{
			return this.selectedMetaTemplateScenario;
		}
		IScenario scenario = this.selectedScenario;
		if (scenario == null)
		{
			return null;
		}
		return scenario.scenarioTemplate;
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x00069EB3 File Offset: 0x000680B3
	public void SetLanguage()
	{
		if (!Loc.currentLocalizationTemplate.requiresFontChange)
		{
			TILocalizationTemplate priorLocalizationTemplate = Loc.priorLocalizationTemplate;
			if (priorLocalizationTemplate == null || !priorLocalizationTemplate.requiresFontChange)
			{
				goto IL_0025;
			}
		}
		this.OnLanguageChangedEvent();
		IL_0025:
		Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x00069EEC File Offset: 0x000680EC
	private void Initialize()
	{
		this.continueButtonText.SetText(Loc.T("UI.StartScreen.Continue"));
		this.newGameText.SetText(Loc.T("UI.StartScreen.NewGame"));
		this.loadGameText.SetText(Loc.T("UI.StartScreen.LoadGame"));
		this.optionsText.SetText(Loc.T("UI.StartScreen.Options"));
		this.modsText.SetText(Loc.T("UI.StartScreen.Mods"));
		this.skirmishModeText.SetText(Loc.T("UI.StartScreen.SkirmishMode"));
		this.creditsText.SetText(Loc.T("UI.StartScreen.Credits"));
		this.exitText.SetText(Loc.T("UI.StartScreen.Exit"));
		this.RefreshContinueButton();
		this.skirmishModeBeginText.SetText(Loc.T("UI.StartScreen.Skirmish.Begin"));
		this.skirmishModeHeaderText.SetText(Loc.T("UI.StartScreen.Skirmish.Header"));
		this.skirmishModePlayer1HeaderText.SetText(Loc.T("UI.StartScreen.Skirmish.Player1Header"));
		this.skirmishModePlayer2HeaderText.SetText(Loc.T("UI.StartScreen.Skirmish.Player2Header"));
		this.skirmishModeLocationTitle.SetText(Loc.T("UI.StartScreen.Skirmish.LocationTitle"));
		this.skirmishModeHabTitle.SetText(Loc.T("UI.StartScreen.Skirmish.HabTitle"));
		this.skirmishModePlayer1AddShipsText.SetText(Loc.T("UI.StartScreen.Skirmish.Player1AddShips"));
		this.skirmishModePlayer2AddShipsText.SetText(Loc.T("UI.StartScreen.Skirmish.Player2AddShips"));
		this.skirmishModePlayer1CloseAddShipsText.SetText(Loc.T("UI.StartScreen.Skirmish.CloseAddShips"));
		this.skirmishModePlayer2CloseAddShipsText.SetText(Loc.T("UI.StartScreen.Skirmish.CloseAddShips"));
		this.newGamePanelHeader.SetText(Loc.T("UI.StartScreen.NewGameOptions"));
		this.newGameStartButtonText.SetText(Loc.T("UI.StartScreen.StartNewGame"));
		this.selectFactionDropdownHeader.SetText(Loc.T("UI.StartScreen.SelectFaction"));
		this.tutorialToggleText.SetText(Loc.T("UI.StartScreen.ToggleTutorial"));
		this.skirmishTutorialToggleText.SetText(Loc.T("UI.StartScreen.ToggleCombatTutorial"));
		this.selectDifficultyHeader.SetText(Loc.T("UI.StartScreen.SelectDifficulty"));
		this.BuildCreditsStrings();
		this.factionCustomizeButton.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.Customize"));
		this.factionCustomizeHeader.SetText(Loc.T("UI.StartScreen.FactionCustomize.Header"));
		this.factionCustomizeDisplayName.SetText(Loc.T("UI.StartScreen.FactionCustomize.DisplayName"));
		this.factionCustomizeAdjective.SetText(Loc.T("UI.StartScreen.FactionCustomize.Adjective"));
		this.factionCustomizeLeaderAddress.SetText(Loc.T("UI.StartScreen.FactionCustomize.LeaderAddress"));
		this.factionCustomizeFleet.SetText(Loc.T("UI.StartScreen.FactionCustomize.FleetName"));
		this.factionCustomizeDefaultButton.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CampaignPresetRapid"));
		this.factionCustomizationCancelButton.SetText(Loc.T("UI.StartScreen.Cancel"));
		this.campaignCustomizationRapidPresetButtonText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CampaignPresetRapid"));
		this.campaignOptionsDifficultyHeaderText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.DifficultyHeader"));
		this.campaignOptionsFactionHeaderText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.SelectFactionsHeader"));
		this.campaignOptionsFactionNamesHeaderText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.YourFactionNamesHeader"));
		this.newGameCustomizationMainHeaderText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.Options"));
		this.researchSpeedTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.ResearchSpeed"));
		this.controlPointFreebieTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CPFreebies"));
		this.controlPointAIFreebieTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CPFreebiesAI"));
		this.miningProductivityTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MiningProductivity"));
		this.alienProgressionRateTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.AlienProgressionRate"));
		this.missionControlFreebieTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MCFreebies"));
		this.missionControlAIFreebieTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MCFreebiesAI"));
		this.variableProjectUnlocksText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.VariableProjectUnlocks"));
		this.showtriggeredProjectsText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.ShowTriggeredProjects"));
		this.firstCouncilorHomeNationText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.HomeNationCouncilor"));
		this.nationalIPModifierTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.NationalIPModifier"));
		this.averageMonthlyEventsModifierTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.AverageMonthlyEvents"));
		this.startCustomCampaignButtonText.SetText(Loc.T("UI.StartScreen.StartCustomCampaign"));
		this.campaignCustomizationLongPresetButtonText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CampaignPresetLong"));
		this.StartAcceleratedCampaignButtonText.SetText(Loc.T("UI.StartScreen.StartAcceleratedCampaign"));
		this.campaignCustomizationPreviousCampaignText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.LastCampaignPreset"));
		this.realismCombatDVMovementText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatMovement"));
		this.realismCombatScaleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatScaling"));
		this.skirmishRealismCombatDVMovementText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatMovement"));
		this.skirmishRealismCombatScaleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatScaling"));
		this.AddAlienAssaultFleetText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.AddAlienAssaultFleet"));
		this.otherFactionStartingNationsText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.OtherFactionsReceivesGroup"));
		this.canDisableFactionsText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.AllowDisableAIFactions"));
		this.miningRatePlayerTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MiningRatePlayer"));
		this.miningRateHumanAITitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MiningRateHumanAI"));
		this.miningRateAlienTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MiningRateAlien"));
		this.habConstructionSpeedPlayerTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.HabConstructionSpeedPlayer"));
		this.habConstructionSpeedHumanAITitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.HabConstructionSpeedHumanAI"));
		this.habConstructionSpeedAlienTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.HabConstructionSpeedAlien"));
		this.shipConstructionSpeedPlayerTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.ShipConstructionSpeedPlayer"));
		this.shipConstructionSpeedHumanAITitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.ShipConstructionSpeedHumanAI"));
		this.shipConstructionSpeedAlienTitle.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.ShipConstructionSpeedAlien"));
		this.smallShipNameListIdxText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.SmallShipNameListIdxText"));
		this.mediumShipNameListIdxText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MediumShipNameListIdxText"));
		this.largeShipNameListIdxText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.LargeShipNameListIdxText"));
		this.habNameListIdxText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.HabNameListIdxText"));
		this.customStartingNationGroupText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CustomStartingNationGroupText"));
		this.CPFreebieTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CPFreebiesTooltip"));
		this.AICPFreebieTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CPFreebiesAITooltip"));
		this.MCFreebieTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.MCFreebiesTooltip"));
		this.AIMCFreebieTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.MCFreebiesAITooltip"));
		this.researchSpeedTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.ResearchSpeedTooltip"));
		this.miningProductivityTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.MiningProductivityTooltip"));
		this.alienProgressionTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.AlienProgressionRateTooltip"));
		this.variableProjectUnlocksTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.VariableProjectUnlocksTooltip"));
		this.showtriggeredProjectsTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.ShowTriggeredProjectsTooltip"));
		this.difficultyWarningTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CustomDifficultyTooltip"));
		this.difficultyWarningOptionsTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CustomDifficultyTooltip"));
		this.firstCouncilorHomeNationTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.HomeNationCouncilorTip"));
		this.nationalIPModifierTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.NationalIPModifierTooltip"));
		this.averageMonthlyEventsModifierTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.AverageMonthlyEventsTooltip"));
		this.realismCombatScaleTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatScalingTooltip"));
		this.realismCombatDVMovementTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatMovementTooltip"));
		this.skirmishRealismCombatScaleTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatScalingTooltip"));
		this.skirmishRealismCombatDVMovementTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CinematicCombatMovementTooltip"));
		this.addAlienAssaultFleetTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.AddAlienAssaultFleetTooltip"));
		this.miningRatePlayerTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.MiningRatePlayerTooltip"));
		this.miningRateHumanAITooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.MiningRateHumanAITooltip"));
		this.miningRateAlienTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.MiningRateAlienTooltip"));
		this.habConstructionSpeedPlayerTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.HabConstructionSpeedPlayerTooltip"));
		this.habConstructionSpeedHumanAITooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.HabConstructionSpeedHumanAITooltip"));
		this.habConstructionSpeedAlienTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.HabConstructionSpeedAlienTooltip"));
		this.shipConstructionSpeedPlayerTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.ShipConstructionSpeedPlayerTooltip"));
		this.shipConstructionSpeedHumanAITooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.ShipConstructionSpeedHumanAITooltip"));
		this.shipConstructionSpeedAlienTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.ShipConstructionSpeedAlienTooltip"));
		this.nationGroupTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.CustomStartingNationGroupTooltip"));
		this.otherFactionStartingNationGroupTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.OtherFactionsReceivesGroupTooltip"));
		this.canDisableFactionsTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.AllowDisableAIFactionsTooltip"));
		this.longCampaignTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.longCampaignTooltip"));
		this.acceleratedCampaignTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.acceleratedCampaignTooltip"));
		this.longCampaignSettingsTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.longCampaignSettingsTooltip"));
		this.acceleratedCampaignSettingsTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.acceleratedCampaignSettingsTooltip"));
		this.difficultyWarningOptionsText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CustomDifficultyOptionsText"));
		this.recommendTutorialButtonText.SetText(Loc.T("UI.StartScreen.Mods.Ok"));
		this.recommendTutorialDescText.SetText(Loc.T("UI.StartScreen.TutorialDesc"));
		this.hardwareWarningTitleText.SetText(Loc.T("UI.Councilor.AbortWarningHeader"));
		this.hardwareWarningDescriptionText.SetText(Loc.T("UI.Options.WarningSystem"));
		this.hardwareWarningConfirmText.SetText(Loc.T("UI.Councilor.Orgs.AcknowledgeButton"));
		this.randomizeMapText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.RandomMap"));
		this.randomizeMapSeedText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.Seed"));
		this.randomizeMapTooltip.SetDelegate("BodyText", () => Loc.T("UI.StartScreen.CustomizeCampaign.RandomMapTooltip"));
		GameControl.control.ValidateDLC();
		this.applicationVersionText.SetText(Application.version);
		this.discordLinkText.SetText(Loc.T("UI.StartScreen.Discord"));
		this.wikiLinkText.SetText(Loc.T("UI.StartScreen.Wiki"));
		this.patchNotesText.SetText(new StringBuilder(Loc.T("UI.StartScreen.PatchNotes")).Append("<sprite tint=1 name=\"external_link\">"));
		if (ModManager.dlcDirectories.Any<string>((string x) => x.Contains("DarkSkies")) && GameControl.DLCValidated)
		{
			this.dlcDateText.enabled = false;
		}
		this.dlcDateText.SetText((new TIDateTime(DateTime.UtcNow) > new TIDateTime(2026, 7, 27, 17, 0)) ? Loc.T("UI.StartScreen.CallToAction00") : new TIDateTime(2026, 7, 27).ToCustomDateString());
		this.DarkSkiesPromoObject.SetActive(true);
		this.DarkSkiesStoreLogoSteam.SetActive(false);
		this.DarkSkiesStoreLogoGog.SetActive(false);
		this.DarkSkiesStoreLogoEpic.SetActive(false);
		this.DarkSkiesStoreLogoMicrosoft.SetActive(false);
		this.DarkSkiesStoreLogoSteam.SetActive(true);
		if (!Application.isEditor)
		{
			this.firstGameTutorialObject.SetActive(TIPlayerProfileManager.firstGame);
		}
		this.previousCampaignSettingsButton.interactable = TIPlayerProfileManager.storedCampaignOptions.isValid;
		this.optionsController.LoadLocalizedText();
		this.graphicsController.LoadLocalizedText();
		this.audioController.LoadLocalizedText();
		this.gameplayController.LoadLocalizedText();
		this.controlsController.LoadLocalizedText();
		this.modMenuController.LoadLocalizedText();
		this.InitializeCampaignStartControls();
		this.InitializeSkirmishMenu();
		TIInputManager.SetDefaultCursor(false);
		this.modMenuButton.interactable = true;
		this.factionCustomizationObject.SetActive(false);
	}

	// Token: 0x06001585 RID: 5509 RVA: 0x0006AE2B File Offset: 0x0006902B
	public void OnLaunchLongCampaignClicked()
	{
		this.SetDefaultCampaignOptions();
		this.OnLaunchCampaignClicked();
	}

	// Token: 0x06001586 RID: 5510 RVA: 0x0006AE39 File Offset: 0x00069039
	public void OnLaunchAcceleratedCampaignClicked()
	{
		this.OnSelectCampaignPreset(2f);
		this.OnLaunchCampaignClicked();
	}

	// Token: 0x06001587 RID: 5511 RVA: 0x0006AE4C File Offset: 0x0006904C
	public void OnLaunchCustomCampaignClicked()
	{
		this.StoreCampaignOptions();
		this.OnLaunchCampaignClicked();
	}

	// Token: 0x06001588 RID: 5512 RVA: 0x0006AE5C File Offset: 0x0006905C
	public void OnLaunchCampaignClicked()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmMission", false, false);
		GameControl.control.startupTutorialActive = this.tutorial;
		GameControl.control.startupDifficulty = this.selectDifficultyDropdown.value + 1;
		this.SetCustomCampaignOptions(false);
		Dictionary<TIMetaTemplate, string> dictionary = new Dictionary<TIMetaTemplate, string>();
		foreach (string text in this.regularScenario.scenarioTemplate.templateNames)
		{
			TIMetaTemplate timetaTemplate = TemplateManager.Find<TIMetaTemplate>(text, false);
			dictionary.Add(timetaTemplate, text);
		}
		foreach (string text2 in this.currentStartOptions.Values)
		{
			TIMetaTemplate metaTemplate = TemplateManager.Find<TIMetaTemplate>(text2, false);
			string dataName = dictionary.Keys.Single<TIMetaTemplate>((TIMetaTemplate x) => x.newCampaignOptionCategory == metaTemplate.newCampaignOptionCategory).dataName;
			this.regularScenario.scenarioTemplate.templateNames[this.regularScenario.scenarioTemplate.templateNames.IndexOf(dataName)] = metaTemplate.dataName;
		}
		GameControl.control.skirmishMode = false;
		Debug.Log(DateTime.Now);
		this.loadingScreen.SetActive(true);
		global::UnityEngine.Object.FindObjectOfType<LoadScreenWidget>().InitLoadWidget();
		if (TIPlayerProfileManager.firstGame)
		{
			TIPlayerProfileManager.firstGame = false;
			TIPlayerProfileManager.SavePlayerConfig();
		}
		this.sceneManager.LoadScene("SolarSystemScene", delegate(DiContainer container)
		{
			container.BindInstance<IScenario>(this.selectedScenario).WhenInjectedInto<SolarSystemBootstrap>();
		});
	}

	// Token: 0x06001589 RID: 5513 RVA: 0x0006B010 File Offset: 0x00069210
	public void OnStartSkirmishModeClicked()
	{
		GameStateManager.ClearAllGameStates();
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		List<TISpaceFleetTemplate> list = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x);
		this.skirmishScenario.SetActivePlayerFaction(list[0].factionTemplate);
		this.SetCustomCampaignOptions(true);
		TISpaceFleetTemplate tispaceFleetTemplate = list[1];
		TIHabTemplate habTemplate = this.skirmishScenario.habTemplate;
		Formation formation = StratCombatInitStrategy.SelectSkirmishFormation(tispaceFleetTemplate, ((habTemplate != null) ? habTemplate.sectors[0].faction : null) != list[0].factionName);
		list[1].formationName = formation.patternDataName;
		list[1].formationSpacing = formation.spacing;
		list[1].formationFocus = formation.focus;
		list[1].formationConcentration = formation.concentration;
		this.selectedScenario = this.skirmishScenario;
		this.selectedMetaTemplateScenario = null;
		GameControl.control.startupTutorialActive = this.skirmishTutorialToggle.isOn;
		GameControl.control.skirmishMode = true;
		this.loadingScreen.SetActive(true);
		this.sceneManager.LoadScene("SolarSystemScene", delegate(DiContainer container)
		{
			container.BindInstance<IScenario>(this.selectedScenario).WhenInjectedInto<SolarSystemBootstrap>();
		});
		GameControl.spaceCombat.prevSkirmishSettings = new SkirmishModeSettings(list, this.skirmishScenario.habTemplate, StartMenuController.importedShipTemplates);
	}

	// Token: 0x0600158A RID: 5514 RVA: 0x0006B190 File Offset: 0x00069390
	public void OnToggleSkirmishModeRealismSettings(int which)
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		if (which == 0)
		{
			StartMenuController.CinematicScalingMode = this.skirmishRealismCombatScaleToggle.isOn;
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.shipDictionary.Values)
			{
				tispaceShipTemplate.CacheTemplateValues(true);
			}
		}
	}

	// Token: 0x0600158B RID: 5515 RVA: 0x0006B208 File Offset: 0x00069408
	private void StoreCampaignOptions()
	{
		TIPlayerProfileManager.storedCampaignOptions.isValid = true;
		TIPlayerProfileManager.storedCampaignOptions.customFactionStartingNationGroup = this.customStartingNationGroupDropdown.value;
		TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Clear();
		if (this.tutorial)
		{
			if (this.startingCouncilor1Profession.value > this.startingCouncilorProfessionIndex)
			{
				TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Add(this.startingCouncilor1Profession.value + 1);
			}
			else
			{
				TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Add(this.startingCouncilor1Profession.value);
			}
			if (this.startingCouncilor2Profession.value > this.startingCouncilorProfessionIndex)
			{
				TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Add(this.startingCouncilor2Profession.value + 1);
			}
			else
			{
				TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Add(this.startingCouncilor2Profession.value);
			}
		}
		else
		{
			TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Add(this.startingCouncilor1Profession.value);
			TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions.Add(this.startingCouncilor2Profession.value);
		}
		TIPlayerProfileManager.storedCampaignOptions.usePlayerCountryForStartingCouncilor = this.firstCouncilorHomeNationToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.variableProjectUnlocks = this.variableProjectUnlocksToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.showTriggeredProjects = this.showtriggeredProjectsToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.addAlienAssaultCarrierFleet = this.addAlienAssaultFleetToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.cinematicCombatRealismDV = this.realismCombatDVMovementToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.cinematicCombatRealismScale = this.realismCombatScaleToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.otherFactionStartingNations = this.otherFactionStartingNations.isOn;
		TIPlayerProfileManager.storedCampaignOptions.canDisableFactions = this.canDisableFactionsToggle.isOn;
		TIPlayerProfileManager.storedCampaignOptions.researchSpeedMultiplier = (int)this.researchSpeedMultiplierSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.controlPointMaintenanceFreebieBonus = (int)this.controlPointFreebieBonusSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.controlPointMaintenanceFreebieBonusAI = (int)this.controlPointAIFreebieBonusSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.missionControlBonus = (int)this.missionControlFreebieBonusSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.missionControlBonusAI = (int)this.missionControlAIFreebieBonusSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.alienProgressionSpeed = (int)this.alienProgressionMultiplierSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.miningProductivityMultiplier = (int)this.miningProductivityMultiplierSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.nationalIPMultiplier = (int)this.nationalIPModifierSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.averageMonthlyEvents = (int)this.averageMonthlyEventsModifierSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.miningRatePlayer = (int)this.miningRatePlayerSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.miningRateHumanAI = (int)this.miningRateHumanAISlider.value;
		TIPlayerProfileManager.storedCampaignOptions.miningRateAlien = (int)this.miningRateAlienSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.habConstructionSpeedPlayer = (int)this.habConstructionSpeedPlayerSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.habConstructionSpeedHumanAI = (int)this.habConstructionSpeedHumanAISlider.value;
		TIPlayerProfileManager.storedCampaignOptions.habConstructionSpeedAlien = (int)this.habConstructionSpeedAlienSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.shipConstructionSpeedPlayer = (int)this.shipConstructionSpeedPlayerSlider.value;
		TIPlayerProfileManager.storedCampaignOptions.shipConstructionSpeedHumanAI = (int)this.shipConstructionSpeedHumanAISlider.value;
		TIPlayerProfileManager.storedCampaignOptions.shipConstructionSpeedAlien = (int)this.shipConstructionSpeedAlienSlider.value;
		TIPlayerProfileManager.SavePlayerConfig();
	}

	// Token: 0x0600158C RID: 5516 RVA: 0x0006B554 File Offset: 0x00069754
	public void OnToggleTutorial()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		this.tutorial = this.tutorialToggle.isOn;
		this.customStartingNationGroupGO.SetActive(!this.tutorial);
		this.otherFactionStartingNationsGO.SetActive(!this.tutorial);
		this.customStartingNationGroupDropdown.SetValueWithoutNotify(0);
		this.SetCouncilorProfessionOptions(true);
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x0006B5B9 File Offset: 0x000697B9
	public void PlayOpenMenuAudio()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x0006B5C7 File Offset: 0x000697C7
	public void PlayCloseMenuAudio()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x0006B5D5 File Offset: 0x000697D5
	public void ToggleHardwareWarning(bool show)
	{
		this.hardwareWarningObject.SetActive(show);
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x0006B5E4 File Offset: 0x000697E4
	public void RefreshContinueButton()
	{
		try
		{
			string continueFilePath = StartMenuController.continueSaveFilepath;
			bool flag = File.Exists(continueFilePath);
			bool flag2 = false;
			if (flag)
			{
				TIMetadataState.LoadMetaData(continueFilePath, out flag2, true);
			}
			this.continueButton.interactable = flag && flag2;
			this.continueButtonTooltip.enabled = flag && flag2;
			if (flag)
			{
				this.continueButtonTooltip.SetDelegate("BodyText", () => Path.GetFileNameWithoutExtension(continueFilePath));
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message + ex.StackTrace);
		}
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x0006B688 File Offset: 0x00069888
	public void ContinueGame()
	{
		if (File.Exists(StartMenuController.continueSaveFilepath))
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			GameControl.control.skirmishMode = false;
			this.loadingScreen.SetActive(true);
			this.sceneManager.LoadScene("SolarSystemScene", delegate(DiContainer container)
			{
				container.BindInstance<string>(StartMenuController.continueSaveFilepath).WhenInjectedInto<SolarSystemBootstrap>();
			});
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		this.continueButton.interactable = false;
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x0006B70C File Offset: 0x0006990C
	public void ExitGame()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
		Debug.Log("Closing Terra Invicta");
		Application.Quit();
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x0006B729 File Offset: 0x00069929
	public void OnOpenModMenu()
	{
		this.PlayOpenMenuAudio();
		this.modMenuController.SetSteamWorkshopTabs();
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x0006B73C File Offset: 0x0006993C
	public void OnClickCustomizeFaction()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		this.factionCustomizationObject.SetActive(true);
		this.startLongCampaignButton.interactable = false;
		this.startAcceleratedCampaignButton.interactable = false;
		if (TIPlayerProfileManager.storedCampaignOptions.isValid)
		{
			this.SetPreviousCampaignOptions();
		}
		this.AdjustCampaignSettingsMenuOffsetWithScaling(true);
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x0006B794 File Offset: 0x00069994
	public void OnClickCancelCustomizeFaction()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
		this.factionCustomizationObject.SetActive(false);
		this.ResetAllCustomizations();
		this.LoadFactionTextFields();
		this.startLongCampaignButton.interactable = true;
		this.startAcceleratedCampaignButton.interactable = true;
		this.AdjustCampaignSettingsMenuOffsetWithScaling(false);
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x0006B7E4 File Offset: 0x000699E4
	private void LoadFactionTextFields()
	{
		this.customDisplayNameInput.SetTextWithoutNotify(this.selectedScenario.activePlayerFaction.displayNameCurrentForStartScreen());
		this.customAdjectiveInput.SetTextWithoutNotify(this.selectedScenario.activePlayerFaction.adjective);
		this.customLeaderAddressInput.SetTextWithoutNotify(this.selectedScenario.activePlayerFaction.leaderAddress);
		this.customFleetInput.SetTextWithoutNotify(this.selectedScenario.activePlayerFaction.fleetNameBase);
		this.smallShipNameListIdxDropdown.value = this.smallShipNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.smallShipNameListIdx));
		this.smallShipNameListIdxDropdown.RefreshShownValue();
		this.mediumShipNameListIdxDropdown.value = this.mediumShipNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.mediumShipNameListIdx));
		this.mediumShipNameListIdxDropdown.RefreshShownValue();
		this.largeShipNameListIdxDropdown.value = this.largeShipNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.largeShipNameListIdx));
		this.largeShipNameListIdxDropdown.RefreshShownValue();
		this.habNameListIdxDropdown.value = this.habNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.habNameListIdx));
		this.habNameListIdxDropdown.RefreshShownValue();
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x0006B928 File Offset: 0x00069B28
	private void SetCustomCampaignOptions(bool skirmish = false)
	{
		ScenarioCustomizations scenarioCustomizations = new ScenarioCustomizations();
		scenarioCustomizations.usingCustomizations = true;
		scenarioCustomizations.customDifficulty = this.customDifficulty;
		scenarioCustomizations.customFactionText = new Dictionary<string, ScenarioCustomizations.CustomFactionText>();
		string key = this.nameListsToAdd.Where<KeyValuePair<string, string>>((KeyValuePair<string, string> x) => x.Value == this.smallShipNameListIdxDropdown.options[this.smallShipNameListIdxDropdown.value].text).First<KeyValuePair<string, string>>().Key;
		string key2 = this.nameListsToAdd.Where<KeyValuePair<string, string>>((KeyValuePair<string, string> x) => x.Value == this.mediumShipNameListIdxDropdown.options[this.mediumShipNameListIdxDropdown.value].text).First<KeyValuePair<string, string>>().Key;
		string key3 = this.nameListsToAdd.Where<KeyValuePair<string, string>>((KeyValuePair<string, string> x) => x.Value == this.largeShipNameListIdxDropdown.options[this.largeShipNameListIdxDropdown.value].text).First<KeyValuePair<string, string>>().Key;
		string key4 = this.nameListsToAdd.Where<KeyValuePair<string, string>>((KeyValuePair<string, string> x) => x.Value == this.habNameListIdxDropdown.options[this.habNameListIdxDropdown.value].text).First<KeyValuePair<string, string>>().Key;
		scenarioCustomizations.customFactionText.Add(this.selectedScenario.activePlayerFaction.dataName, new ScenarioCustomizations.CustomFactionText(this.customDisplayNameInput.text, this.customAdjectiveInput.text, this.customLeaderAddressInput.text, this.customFleetInput.text, key, key2, key3, key4));
		bool flag = key != this.selectedScenario.activePlayerFaction.smallShipNameListIdx;
		key2 != this.selectedScenario.activePlayerFaction.mediumShipNameListIdx;
		key3 != this.selectedScenario.activePlayerFaction.largeShipNameListIdx;
		bool flag2 = key4 != this.selectedScenario.activePlayerFaction.habNameListIdx;
		List<ScenarioCustomizations.CustomFactionText> list = new List<ScenarioCustomizations.CustomFactionText>();
		List<string> list2 = new List<string>();
		foreach (TIFactionTemplate tifactionTemplate in this.factionsInScenario)
		{
			if (!(tifactionTemplate.dataName == this.selectedScenario.activePlayerFaction.dataName) && !tifactionTemplate.isAlien)
			{
				list2.Add(tifactionTemplate.dataName);
				list.Add(new ScenarioCustomizations.CustomFactionText(tifactionTemplate.displayName, tifactionTemplate.adjective, tifactionTemplate.leaderAddress, tifactionTemplate.fleetNameBase, (flag && tifactionTemplate.smallShipNameListIdx == key) ? this.selectedScenario.activePlayerFaction.smallShipNameListIdx : tifactionTemplate.smallShipNameListIdx, (flag && tifactionTemplate.mediumShipNameListIdx == key2) ? this.selectedScenario.activePlayerFaction.mediumShipNameListIdx : tifactionTemplate.mediumShipNameListIdx, (flag && tifactionTemplate.largeShipNameListIdx == key3) ? this.selectedScenario.activePlayerFaction.largeShipNameListIdx : tifactionTemplate.largeShipNameListIdx, (flag2 && tifactionTemplate.habNameListIdx == key4) ? this.selectedScenario.activePlayerFaction.habNameListIdx : tifactionTemplate.habNameListIdx));
			}
		}
		int num = 0;
		foreach (ScenarioCustomizations.CustomFactionText customFactionText in list)
		{
			scenarioCustomizations.customFactionText.Add(list2[num++], customFactionText);
		}
		scenarioCustomizations.controlPointMaintenanceFreebieBonus = this.GetAdditionalCPFreebiesBonus();
		scenarioCustomizations.controlPointMaintenanceFreebieBonusAI = this.GetAICPFreebiesBonus();
		scenarioCustomizations.missionControlBonus = (float)this.GetMCFreebiesBonus();
		scenarioCustomizations.missionControlBonusAI = (float)this.GetAIMCFreebiesBonus();
		scenarioCustomizations.researchSpeedMultiplier = this.GetResearchSpeedMultiplier();
		scenarioCustomizations.alienProgressionSpeed = this.GetAlienProgressionSpeedMultiplier();
		scenarioCustomizations.miningProductivityMultiplier = this.GetMiningProductivitySpeedMultiplier();
		scenarioCustomizations.miningRatePlayer = this.GetSliderMultiplier(this.miningRatePlayerSlider, 0.05f);
		scenarioCustomizations.miningRateHumanAI = this.GetSliderMultiplier(this.miningRateHumanAISlider, 0.05f);
		scenarioCustomizations.miningRateAlien = this.GetSliderMultiplier(this.miningRateAlienSlider, 0.05f);
		scenarioCustomizations.habConstructionSpeedPlayer = this.GetSliderMultiplier(this.habConstructionSpeedPlayerSlider, 0.05f);
		scenarioCustomizations.habConstructionSpeedHumanAI = this.GetSliderMultiplier(this.habConstructionSpeedHumanAISlider, 0.05f);
		scenarioCustomizations.habConstructionSpeedAlien = this.GetSliderMultiplier(this.habConstructionSpeedAlienSlider, 0.05f);
		scenarioCustomizations.shipConstructionSpeedPlayer = this.GetSliderMultiplier(this.shipConstructionSpeedPlayerSlider, 0.05f);
		scenarioCustomizations.shipConstructionSpeedHumanAI = this.GetSliderMultiplier(this.shipConstructionSpeedHumanAISlider, 0.05f);
		scenarioCustomizations.shipConstructionSpeedAlien = this.GetSliderMultiplier(this.shipConstructionSpeedAlienSlider, 0.05f);
		scenarioCustomizations.variableProjectUnlocks = this.variableProjectUnlocksToggle.isOn;
		scenarioCustomizations.showTriggeredProjects = this.showtriggeredProjectsToggle.isOn;
		scenarioCustomizations.usePlayerCountryForStartingCouncilor = this.firstCouncilorHomeNationToggle.isOn;
		scenarioCustomizations.averageMonthlyEvents = (int)this.averageMonthlyEventsModifierSlider.value;
		scenarioCustomizations.nationalIPMultiplier = this.GetNationalIPMultiplier();
		if (this.startingCouncilor1Profession.value > this.startingCouncilorProfessionIndex - 1)
		{
			scenarioCustomizations.startingCouncilorProfessions.Add(this.allProfessions[this.startingCouncilor1Profession.value - this.startingCouncilorProfessionIndex]);
		}
		if (this.startingCouncilor2Profession.value > this.startingCouncilorProfessionIndex - 1)
		{
			scenarioCustomizations.startingCouncilorProfessions.Add(this.allProfessions[this.startingCouncilor2Profession.value - this.startingCouncilorProfessionIndex]);
		}
		if (!this.tutorial)
		{
			scenarioCustomizations.skipStartingCouncilors[0] = this.startingCouncilor1Profession.value == 1;
			scenarioCustomizations.skipStartingCouncilors[1] = this.startingCouncilor2Profession.value == 1;
		}
		scenarioCustomizations.cinematicCombatRealismScale = (skirmish ? this.skirmishRealismCombatScaleToggle.isOn : this.realismCombatScaleToggle.isOn);
		scenarioCustomizations.cinematicCombatRealismDV = (skirmish ? this.skirmishRealismCombatDVMovementToggle.isOn : this.realismCombatDVMovementToggle.isOn);
		scenarioCustomizations.canDisableFactions = this.canDisableFactionsToggle.isOn;
		scenarioCustomizations.addAlienAssaultCarrierFleet = this.addAlienAssaultFleetToggle.isOn;
		if (!this.tutorial)
		{
			scenarioCustomizations.otherFactionStartingNations = this.otherFactionStartingNations.isOn;
		}
		using (IEnumerator<object> enumerator3 = this.factionToggleListManager.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				if (StartMenuController.<>o__323.<>p__0 == null)
				{
					StartMenuController.<>o__323.<>p__0 = CallSite<Func<CallSite, object, FactionToggleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionToggleListItemController), typeof(StartMenuController)));
				}
				FactionToggleListItemController factionToggleListItemController = StartMenuController.<>o__323.<>p__0.Target(StartMenuController.<>o__323.<>p__0, enumerator3.Current);
				if (factionToggleListItemController.factionToggle.isOn)
				{
					scenarioCustomizations.selectedFactionsForScenario.Add(factionToggleListItemController.faction.dataName);
				}
			}
		}
		if (this.customStartingNationGroupDropdown.value > 0 && !this.tutorial)
		{
			scenarioCustomizations.customFactionStartingNationGroup.Add(this.selectedScenario.activePlayerFaction.dataName, this.customStartingNationGroupDropdown.value);
			if (scenarioCustomizations.otherFactionStartingNations)
			{
				List<string> list3 = new List<string>();
				list3 = (from o in scenarioCustomizations.selectedFactionsForScenario
					where !TemplateManager.Find<TIFactionTemplate>(o, false).isAlien
					orderby TemplateManager.Find<TIFactionIdeologyTemplate>(TemplateManager.Find<TIFactionTemplate>(o, false).ideologyName, false).ideologyCoordinates.x descending
					select o).ToList<string>();
				List<string> list4 = new List<string>();
				int i = 0;
				int num2 = list3.Count - 1;
				while (i < num2)
				{
					list4.Add(list3[num2--]);
					list4.Add(list3[i++]);
				}
				if (list3.Count % 2 != 0)
				{
					list4.Add(list3[i]);
				}
				int num3 = 1;
				foreach (string text in list4)
				{
					if (!(text == this.selectedScenario.activePlayerFaction.dataName))
					{
						if (num3 == this.customStartingNationGroupDropdown.value)
						{
							num3++;
							if (num3 > this.nationsInScenario.Max<TINationTemplate>((TINationTemplate x) => x.group))
							{
								num3 = 1;
							}
						}
						scenarioCustomizations.customFactionStartingNationGroup.Add(text, num3++);
					}
				}
			}
		}
		scenarioCustomizations.randomizeMap = this.randomizeMapToggle.isOn;
		int num4;
		if (int.TryParse(this.randomizeMapSeedInputField.text, out num4))
		{
			scenarioCustomizations.randomizedMapSeed = num4;
		}
		GameControl.control.scenarioCustomizationsStartup = scenarioCustomizations.Clone();
	}

	// Token: 0x06001598 RID: 5528 RVA: 0x0006C1D8 File Offset: 0x0006A3D8
	private void InitializeCampaignStartControls()
	{
		this.regularScenario = new FullScenario();
		this.regularScenario.OnStartScene();
		this.selectedScenario = this.regularScenario;
		this.selectedMetaTemplateScenario = null;
		List<StartMenuController.CategoryWithPriority> list = new List<StartMenuController.CategoryWithPriority>();
		foreach (TIMetaTemplate timetaTemplate in from x in TemplateManager.IterateByClass<TIMetaTemplate>(true)
			where x.isNewCampaignOption
			select x)
		{
			StartMenuController.CategoryWithPriority categoryWithPriority = new StartMenuController.CategoryWithPriority
			{
				category = timetaTemplate.newCampaignOptionCategory,
				priority = (float)timetaTemplate.optionPriority
			};
			if (!list.Contains(categoryWithPriority))
			{
				list.Add(categoryWithPriority);
			}
		}
		list = list.OrderBy<StartMenuController.CategoryWithPriority, float>((StartMenuController.CategoryWithPriority x) => x.priority).ToList<StartMenuController.CategoryWithPriority>();
		this.MatchChildListToCategories(list.Count);
		int num = 0;
		foreach (StartMenuController.CategoryWithPriority categoryWithPriority2 in list)
		{
			if (categoryWithPriority2.priority != 999f || Application.isEditor)
			{
				NewGameOptionController component = this.newCampaignOptionList.GetChild(num++).GetComponent<NewGameOptionController>();
				component.controller = this;
				component.InitWithMetaTemplateCategory(categoryWithPriority2.category);
			}
		}
		this.selectDifficultyDropdown.ClearOptions();
		for (int i = 1; i < 5; i++)
		{
			this.selectDifficultyDropdown.options.Add(new TMP_Dropdown.OptionData(Loc.T("UI.Options.Difficulty" + i.ToString())));
		}
		this.selectDifficultyDropdown.value = 1;
		this.canDisableFactionsToggle.isOn = TemplateManager.global.defaultDisableFactionValue;
		this.UpdateFactionOptions();
		this.ResetAdditionalCampaignOptions();
	}

	// Token: 0x06001599 RID: 5529 RVA: 0x0006C3CC File Offset: 0x0006A5CC
	private void MatchChildListToCategories(int numCategories)
	{
		if (numCategories == 0)
		{
			this.MakeChildListSize(1);
			this.newCampaignOptionList.gameObject.SetActive(false);
			return;
		}
		this.newCampaignOptionList.gameObject.SetActive(true);
		this.MakeChildListSize(numCategories);
	}

	// Token: 0x0600159A RID: 5530 RVA: 0x0006C404 File Offset: 0x0006A604
	private void MakeChildListSize(int newSize)
	{
		int childCount = this.newCampaignOptionList.childCount;
		if (newSize > childCount)
		{
			for (int i = childCount; i < newSize; i++)
			{
				global::UnityEngine.Object.Instantiate<GameObject>(this.newCampaignOptionPanel, this.newCampaignOptionList);
			}
			return;
		}
		for (int j = childCount - 1; j >= newSize; j--)
		{
			global::UnityEngine.Object.Destroy(this.newCampaignOptionList.GetChild(j));
		}
	}

	// Token: 0x0600159B RID: 5531 RVA: 0x0006C460 File Offset: 0x0006A660
	public void UpdateStartOptions(string category, TIMetaTemplate template)
	{
		this.currentStartOptions[category] = template.dataName;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string text in this.currentStartOptions.Values)
		{
			stringBuilder.AppendLine(Loc.T(new StringBuilder("TIMetaTemplate.description.").Append(text).ToString()));
		}
		this.newGameSummaryText.SetText(stringBuilder.ToString());
		if (template.newCampaignOptionCategory == "Scenario")
		{
			this.selectedMetaTemplateScenario = template;
			this.nationsInScenario = TIMetaTemplate.GetTemplatesOfTypeFromMeta(template.dataName, typeof(TINationTemplate)).ConvertAll<TINationTemplate>((TIDataTemplate x) => (TINationTemplate)x).ToList<TINationTemplate>();
			this.defaultCompletedProjectsInScenario = (TIMetaTemplate.GetTemplatesOfTypeFromMeta(template.dataName, typeof(TIStartTimeTemplate)).First<TIDataTemplate>() as TIStartTimeTemplate).projectsCompleted;
			this.UpdateAllowedFactions();
			this.UpdateTutorialOptions();
			this.SetStarterNationOptions();
			this.UpdateMapOptions(template.dataName);
		}
		if (template.newCampaignOptionCategory == "FactionCouncils")
		{
			this.currentAllowedFactions = (from x in TIMetaTemplate.GetTemplatesOfTypeFromMeta(template.dataName, typeof(TIFactionTemplate)).ConvertAll<TIFactionTemplate>((TIDataTemplate x) => (TIFactionTemplate)x)
				where x.activePlayerAllowed
				select x).ToList<TIFactionTemplate>();
			this.factionsInScenario = TIMetaTemplate.GetTemplatesOfTypeFromMeta(template.dataName, typeof(TIFactionTemplate)).ConvertAll<TIFactionTemplate>((TIDataTemplate x) => (TIFactionTemplate)x).ToList<TIFactionTemplate>();
			this.defaultFactionsInScenario = new List<string>(this.factionsInScenario.Select<TIFactionTemplate, string>((TIFactionTemplate x) => x.dataName).ToList<string>());
			this.ResetFactionOptions();
			this.UpdateAllowedFactions();
			this.UpdateFactionOptions();
		}
		if (template.newCampaignOptionCategory == "SolarSystem")
		{
			int count = TIMetaTemplate.GetTemplatesOfTypeFromMeta(template.dataName, typeof(TISpaceBodyTemplate)).Count;
			if (count <= TemplateManager.global.defaultSpaceBodyCapForMiningProductivityBonus)
			{
				this.defaultMiningProductivity = (int)((float)TemplateManager.global.defaultMiningProductivity * ((float)TemplateManager.global.defaultSpaceBodyCapForMiningProductivityBonus / (float)count));
			}
			else
			{
				this.defaultMiningProductivity = TemplateManager.global.defaultMiningProductivity;
			}
			this.miningProductivityMultiplierSlider.SetValueWithoutNotify((float)this.defaultMiningProductivity);
			this.UpdateMiningProductivySpeedSlider(false);
		}
	}

	// Token: 0x0600159C RID: 5532 RVA: 0x0006C738 File Offset: 0x0006A938
	public void UpdateTutorialOptions()
	{
		TIMetaTemplate timetaTemplate = TemplateManager.Find<TIMetaTemplate>(this.currentStartOptions["Scenario"], false);
		TIFactionTemplate selectedFaction = this.selectedFaction;
		if (selectedFaction != null && selectedFaction.tutorialAllowed && timetaTemplate != null && timetaTemplate.tutorialAllowed)
		{
			this.tutorialToggle.gameObject.SetActive(true);
			this.tutorialToggle.isOn = this.tutorial;
			if (!Application.isEditor && TIPlayerProfileManager.firstGame)
			{
				this.tutorialToggle.isOn = true;
				return;
			}
		}
		else
		{
			this.tutorialToggle.gameObject.SetActive(false);
			this.tutorial = false;
			this.tutorialToggle.isOn = this.tutorial;
		}
	}

	// Token: 0x0600159D RID: 5533 RVA: 0x0006C7E4 File Offset: 0x0006A9E4
	public void UpdateFactionOptions()
	{
		this.candidateFactionDataNames = new List<string>();
		this.newCampaignChooseFactionDropdown.ClearOptions();
		foreach (TIFactionTemplate tifactionTemplate in this.currentAllowedFactions)
		{
			this.candidateFactionDataNames.Add(tifactionTemplate.dataName);
			this.newCampaignChooseFactionDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = tifactionTemplate.capitalizedFactionNameCurrent
			});
		}
		if (!this.currentAllowedFactions.Contains(this.selectedFaction) || this.selectedFaction == null)
		{
			this.newCampaignChooseFactionDropdown.value = 0;
		}
		else
		{
			this.newCampaignChooseFactionDropdown.value = this.candidateFactionDataNames.IndexOf(this.selectedFaction.dataName);
		}
		this.OnFactionOptionSelected();
		this.newCampaignChooseFactionDropdown.captionText.SetText(this.selectedFaction.capitalizedFactionNameCurrent);
		this.newCampaignChooseFactionDropdown.RefreshShownValue();
		this.UpdateTutorialOptions();
	}

	// Token: 0x0600159E RID: 5534 RVA: 0x0006C8F4 File Offset: 0x0006AAF4
	public void OnFactionOptionSelected()
	{
		this.selectedFactionDataName = this.currentAllowedFactions[this.newCampaignChooseFactionDropdown.value].dataName;
		this.newCampaignChooseFactionDropdown.RefreshShownValue();
		this.selectedFactionGradient.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(this.selectedFaction.gradientPath);
		this.selectedFactionIconBold.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(this.selectedFaction.councilIcon128);
		this.selectedFactionIconFaded.sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(this.selectedFaction.councilIcon128);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.selectedFaction.difficulty; i++)
		{
			stringBuilder.Append(TemplateManager.global.starInlineSpritePath);
		}
		this.selectedFactionDescription.SetText(Loc.T("UI.StartScreen.FactionDescription", new object[]
		{
			this.selectedFaction.goal,
			stringBuilder.ToString()
		}));
		this.UpdateTutorialOptions();
		this.regularScenario.SetActivePlayerFaction(this.selectedFaction);
		using (IEnumerator<object> enumerator = this.factionToggleListManager.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (StartMenuController.<>o__335.<>p__0 == null)
				{
					StartMenuController.<>o__335.<>p__0 = CallSite<Func<CallSite, object, FactionToggleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionToggleListItemController), typeof(StartMenuController)));
				}
				StartMenuController.<>o__335.<>p__0.Target(StartMenuController.<>o__335.<>p__0, enumerator.Current).UpdateItem(this.selectedFaction);
			}
		}
		if (this.lastSelectedDifficulty != this.selectDifficultyDropdown.value)
		{
			this.OnDifficultyChanged();
		}
		if (this.lastSelectedFaction != this.newCampaignChooseFactionDropdown.value)
		{
			this.OnFactionChanged();
		}
	}

	// Token: 0x0600159F RID: 5535 RVA: 0x0006CAB8 File Offset: 0x0006ACB8
	public void OnDifficultyChanged()
	{
		this.lastSelectedDifficulty = this.selectDifficultyDropdown.value;
		this.ResetCampaignDifficultyOptions();
	}

	// Token: 0x060015A0 RID: 5536 RVA: 0x0006CAD1 File Offset: 0x0006ACD1
	private void OnFactionChanged()
	{
		this.lastSelectedFaction = this.newCampaignChooseFactionDropdown.value;
		this.LoadFactionTextFields();
	}

	// Token: 0x060015A1 RID: 5537 RVA: 0x0006CAEC File Offset: 0x0006ACEC
	public void OnCouncilorProfessionDropDownChanged()
	{
		this.firstCouncilorHomeNationToggle.interactable = this.startingCouncilor1Profession.value != 1 || this.startingCouncilor2Profession.value != 1;
		if (!this.firstCouncilorHomeNationToggle.interactable)
		{
			this.firstCouncilorHomeNationToggle.SetIsOnWithoutNotify(false);
		}
	}

	// Token: 0x060015A2 RID: 5538 RVA: 0x0006CB3F File Offset: 0x0006AD3F
	public void ResetAllCustomizations()
	{
		this.ResetAdditionalCampaignOptions();
		this.ResetCampaignDifficultyOptions();
	}

	// Token: 0x060015A3 RID: 5539 RVA: 0x0006CB50 File Offset: 0x0006AD50
	private void SetCouncilorProfessionOptions(bool retainSelection = false)
	{
		int num = this.startingCouncilor1Profession.value;
		int num2 = this.startingCouncilor2Profession.value;
		this.startingCouncilor1ProfessionText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CouncilorProfession"));
		this.startingCouncilor2ProfessionText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CouncilorProfession"));
		this.startingCouncilor1Profession.ClearOptions();
		this.startingCouncilor2Profession.ClearOptions();
		this.allProfessions.Clear();
		this.allProfessions = (from x in TemplateManager.IterateByClass<TICouncilorTypeTemplate>(true)
			where x.weight > 0f
			orderby x.displayNameCurrentForStartScreen()
			select x).ToList<TICouncilorTypeTemplate>();
		this.startingCouncilor1Profession.options.Add(new TMP_Dropdown.OptionData
		{
			text = Loc.T("UI.StartScreen.CustomizeCampaign.RandomProfession")
		});
		this.startingCouncilor2Profession.options.Add(new TMP_Dropdown.OptionData
		{
			text = Loc.T("UI.StartScreen.CustomizeCampaign.RandomProfession")
		});
		if (!this.tutorial)
		{
			this.startingCouncilor1Profession.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.StartScreen.CustomizeCampaign.SkipCouncilor", new object[]
				{
					TemplateManager.global.skipCouncilorInfluenceBonus,
					TemplateManager.global.influenceInlineSpritePath
				})
			});
		}
		if (!this.tutorial)
		{
			this.startingCouncilor2Profession.options.Add(new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.StartScreen.CustomizeCampaign.SkipCouncilor", new object[]
				{
					TemplateManager.global.skipCouncilorInfluenceBonus,
					TemplateManager.global.influenceInlineSpritePath
				})
			});
		}
		foreach (TICouncilorTypeTemplate ticouncilorTypeTemplate in this.allProfessions)
		{
			this.startingCouncilor1Profession.options.Add(new TMP_Dropdown.OptionData
			{
				text = ticouncilorTypeTemplate.displayNameCurrentForStartScreen()
			});
			this.startingCouncilor2Profession.options.Add(new TMP_Dropdown.OptionData
			{
				text = ticouncilorTypeTemplate.displayNameCurrentForStartScreen()
			});
		}
		if (retainSelection)
		{
			if (this.tutorial)
			{
				if (num > 1)
				{
					num--;
				}
				if (num2 > 1)
				{
					num2--;
				}
			}
			else
			{
				if (num > 0)
				{
					num++;
				}
				if (num2 > 0)
				{
					num2++;
				}
			}
		}
		this.startingCouncilorProfessionIndex = (this.tutorial ? 1 : 2);
		this.startingCouncilor1Profession.value = num;
		this.startingCouncilor1Profession.RefreshShownValue();
		this.startingCouncilor2Profession.value = num2;
		this.startingCouncilor2Profession.RefreshShownValue();
	}

	// Token: 0x060015A4 RID: 5540 RVA: 0x0006CDFC File Offset: 0x0006AFFC
	private void SetStarterNationOptions()
	{
		this.customStartingNationGroupDropdown.ClearOptions();
		this.customStartingNationGroupDropdown.options.Add(new TMP_Dropdown.OptionData
		{
			text = Loc.T(new StringBuilder("UI.StartScreen.CustomizeCampaign.NationGroupNone").ToString())
		});
		int num = this.nationsInScenario.Max<TINationTemplate>((TINationTemplate x) => x.group);
		int i;
		int j;
		for (i = 1; i < num + 1; i = j + 1)
		{
			IEnumerable<TINationTemplate> enumerable = this.nationsInScenario.Where<TINationTemplate>((TINationTemplate o) => o.group == i);
			TINationTemplate tinationTemplate = enumerable.MaxBy<TINationTemplate, int>((TINationTemplate x) => x.StartingClaims(this.nationsInScenario, this.defaultCompletedProjectsInScenario, true));
			TINationTemplate tinationTemplate2 = enumerable.MaxBy<TINationTemplate, double>((TINationTemplate x) => x.initialGDP.GetValueOrDefault());
			string text = (tinationTemplate.IsStartingUnion(this.nationsInScenario, this.defaultCompletedProjectsInScenario) ? tinationTemplate.startUpUnionDisplayName() : tinationTemplate.startUpDisplayName());
			string text2 = (tinationTemplate2.IsStartingUnion(this.nationsInScenario, this.defaultCompletedProjectsInScenario) ? tinationTemplate2.startUpUnionDisplayName() : tinationTemplate2.startUpDisplayName());
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = 1;
			if (tinationTemplate != tinationTemplate2)
			{
				stringBuilder.Append(Loc.T("UI.Global.2IC", new object[] { text, text2 }));
				num2 = 2;
			}
			else
			{
				stringBuilder.Append(text);
			}
			if (enumerable.Count<TINationTemplate>() > num2)
			{
				stringBuilder.Append("+");
			}
			this.customStartingNationGroupDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = stringBuilder.ToString()
			});
			j = i;
		}
		this.customStartingNationGroupDropdown.value = 0;
		this.customStartingNationGroupDropdown.RefreshShownValue();
	}

	// Token: 0x060015A5 RID: 5541 RVA: 0x0006CFCD File Offset: 0x0006B1CD
	private void UpdateMapOptions(string templateName)
	{
		this.mapSeedInputGO.SetActive(false);
		this.mapRandomizeToggleGO.SetActive(false);
	}

	// Token: 0x060015A6 RID: 5542 RVA: 0x0006CFE8 File Offset: 0x0006B1E8
	public void ResetAdditionalCampaignOptions()
	{
		this.SetCouncilorProfessionOptions(false);
		this.firstCouncilorHomeNationToggle.SetIsOnWithoutNotify(true);
		this.SetStarterNationOptions();
		this.SetStarterNationOptions();
		this.smallShipNameListIdxDropdown.ClearOptions();
		this.mediumShipNameListIdxDropdown.ClearOptions();
		this.largeShipNameListIdxDropdown.ClearOptions();
		this.habNameListIdxDropdown.ClearOptions();
		this.nameListsToAdd.Clear();
		for (int i = 0; i < this.factionsInScenario.Count; i++)
		{
			if (!this.factionsInScenario[i].isAlien)
			{
				this.nameListsToAdd.Add(this.factionsInScenario[i].smallShipNameListIdx, TIUtilities.LocalizedNamelistIDX(this.factionsInScenario[i].smallShipNameListIdx));
				this.nameListsToAdd.Add(this.factionsInScenario[i].mediumShipNameListIdx, TIUtilities.LocalizedNamelistIDX(this.factionsInScenario[i].mediumShipNameListIdx));
				this.nameListsToAdd.Add(this.factionsInScenario[i].largeShipNameListIdx, TIUtilities.LocalizedNamelistIDX(this.factionsInScenario[i].largeShipNameListIdx));
				this.nameListsToAdd.Add(this.factionsInScenario[i].habNameListIdx, TIUtilities.LocalizedNamelistIDX(this.factionsInScenario[i].habNameListIdx));
			}
		}
		foreach (KeyValuePair<string, string> keyValuePair in this.nameListsToAdd)
		{
			this.smallShipNameListIdxDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = keyValuePair.Value
			});
			this.mediumShipNameListIdxDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = keyValuePair.Value
			});
			this.largeShipNameListIdxDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = keyValuePair.Value
			});
			this.habNameListIdxDropdown.options.Add(new TMP_Dropdown.OptionData
			{
				text = keyValuePair.Value
			});
		}
		this.smallShipNameListIdxDropdown.value = this.smallShipNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.smallShipNameListIdx));
		this.smallShipNameListIdxDropdown.RefreshShownValue();
		this.mediumShipNameListIdxDropdown.value = this.mediumShipNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.mediumShipNameListIdx));
		this.mediumShipNameListIdxDropdown.RefreshShownValue();
		this.largeShipNameListIdxDropdown.value = this.largeShipNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.largeShipNameListIdx));
		this.largeShipNameListIdxDropdown.RefreshShownValue();
		this.habNameListIdxDropdown.value = this.habNameListIdxDropdown.options.FindIndex((TMP_Dropdown.OptionData x) => x.text == TIUtilities.LocalizedNamelistIDX(this.selectedScenario.activePlayerFaction.habNameListIdx));
		this.habNameListIdxDropdown.RefreshShownValue();
		this.addAlienAssaultFleetToggle.SetIsOnWithoutNotify(false);
		this.otherFactionStartingNations.SetIsOnWithoutNotify(false);
		this.canDisableFactionsToggle.SetIsOnWithoutNotify(TemplateManager.global.defaultDisableFactionValue);
		this.ResetFactionOptions();
		this.randomizeMapSeedInputField.text = "0";
		this.randomizeMapToggle.SetIsOnWithoutNotify(false);
	}

	// Token: 0x060015A7 RID: 5543 RVA: 0x0006D32C File Offset: 0x0006B52C
	private void ResetFactionOptions()
	{
		this.factionToggleListManager.SetListSize<FactionToggleListItemController>(this.factionsInScenario.Count, false, false);
		int num = 0;
		using (IEnumerator<object> enumerator = this.factionToggleListManager.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (StartMenuController.<>o__345.<>p__0 == null)
				{
					StartMenuController.<>o__345.<>p__0 = CallSite<Func<CallSite, object, FactionToggleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionToggleListItemController), typeof(StartMenuController)));
				}
				StartMenuController.<>o__345.<>p__0.Target(StartMenuController.<>o__345.<>p__0, enumerator.Current).Init(this.factionsInScenario[num], this.selectedScenario.activePlayerFaction, this);
				num++;
			}
		}
		this.UpdateAllowedFactions();
		this.UpdateBaseCPForDefaultFactions();
	}

	// Token: 0x060015A8 RID: 5544 RVA: 0x0006D3F8 File Offset: 0x0006B5F8
	private void ResetCampaignDifficultyOptions()
	{
		this.averageMonthlyEventsModifierSlider.value = (float)TemplateManager.global.defaultRandomEventsPerMonth;
		this.averageMonthlyEventsModifierSlider.minValue = 0f;
		this.averageMonthlyEventsModifierSlider.maxValue = (float)TemplateManager.global.maxRandomEventsPerMonth;
		this.nationalIPModifierSlider.value = 4f;
		this.nationalIPModifierSlider.minValue = 2f;
		this.nationalIPModifierSlider.maxValue = (float)TemplateManager.global.IPMultiplierSliderMax;
		this.alienProgressionMultiplierSlider.maxValue = (float)TemplateManager.global.alienProgressionRateSliderMax;
		this.alienProgressionMultiplierSlider.value = 20f;
		this.miningProductivityMultiplierSlider.maxValue = (float)TemplateManager.global.miningProductivitySliderMax;
		this.miningProductivityMultiplierSlider.value = (float)this.defaultMiningProductivity;
		this.researchSpeedMultiplierSlider.maxValue = (float)TemplateManager.global.researchSpeedSliderMax;
		this.researchSpeedMultiplierSlider.value = 20f;
		this.controlPointFreebieBonusSlider.maxValue = (float)TemplateManager.global.controlPointFreebieSliderMax;
		this.UpdateBaseCPForDefaultFactions();
		this.controlPointAIFreebieBonusSlider.maxValue = (float)TemplateManager.global.controlPointAIFreebieSliderMax;
		this.controlPointAIFreebieBonusSlider.value = TemplateManager.global.AI_BonusFreeCPCap_Difficulty(this.lastSelectedDifficulty + 1) / (float)TemplateManager.global.pointsPerCPSliderTick;
		this.missionControlFreebieBonusSlider.maxValue = (float)TemplateManager.global.missionControlFreebieSliderMax;
		this.missionControlFreebieBonusSlider.value = 0f;
		this.missionControlAIFreebieBonusSlider.maxValue = (float)TemplateManager.global.missionControlAIFreebieSliderMax;
		this.missionControlAIFreebieBonusSlider.value = TemplateManager.global.AI_BonusFreeMissionControl_Difficulty(this.lastSelectedDifficulty + 1) / (float)TemplateManager.global.pointsPerMCSliderTick;
		this.miningRatePlayerSlider.maxValue = (float)TemplateManager.global.miningRateSliderMax;
		this.miningRatePlayerSlider.value = 20f;
		this.miningRateHumanAISlider.maxValue = (float)TemplateManager.global.miningRateSliderMax;
		this.miningRateHumanAISlider.value = 20f;
		this.miningRateAlienSlider.maxValue = (float)TemplateManager.global.miningRateSliderMax;
		this.miningRateAlienSlider.value = 20f;
		this.habConstructionSpeedPlayerSlider.maxValue = (float)TemplateManager.global.habConstructionSpeedSliderMax;
		this.habConstructionSpeedPlayerSlider.value = 20f;
		this.habConstructionSpeedHumanAISlider.maxValue = (float)TemplateManager.global.habConstructionSpeedSliderMax;
		this.habConstructionSpeedHumanAISlider.value = 20f;
		this.habConstructionSpeedAlienSlider.maxValue = (float)TemplateManager.global.habConstructionSpeedSliderMax;
		this.habConstructionSpeedAlienSlider.value = 20f;
		this.shipConstructionSpeedPlayerSlider.maxValue = (float)TemplateManager.global.shipConstructionSpeedSliderMax;
		this.shipConstructionSpeedPlayerSlider.value = 20f;
		this.shipConstructionSpeedHumanAISlider.maxValue = (float)TemplateManager.global.shipConstructionSpeedSliderMax;
		this.shipConstructionSpeedHumanAISlider.value = 20f;
		this.shipConstructionSpeedAlienSlider.maxValue = (float)TemplateManager.global.shipConstructionSpeedSliderMax;
		this.shipConstructionSpeedAlienSlider.value = 20f;
		this.variableProjectUnlocksToggle.SetIsOnWithoutNotify(true);
		this.showtriggeredProjectsToggle.SetIsOnWithoutNotify(false);
		this.realismCombatScaleToggle.SetIsOnWithoutNotify(this.lastSelectedDifficulty < 2);
		this.realismCombatDVMovementToggle.SetIsOnWithoutNotify(this.lastSelectedDifficulty < 2);
		this.customStartingNationGroupDropdown.SetValueWithoutNotify(0);
		this.otherFactionStartingNations.SetIsOnWithoutNotify(false);
		this.otherFactionStartingNations.interactable = false;
		this.DisableCustomDifficulty();
	}

	// Token: 0x060015A9 RID: 5545 RVA: 0x0006D76C File Offset: 0x0006B96C
	private void ValidateCustomDifficultySettings()
	{
		if (this.nationalIPModifierSlider.value == 4f && this.alienProgressionMultiplierSlider.value == 20f && this.miningProductivityMultiplierSlider.value == (float)this.defaultMiningProductivity && this.researchSpeedMultiplierSlider.value == 20f && this.controlPointFreebieBonusSlider.value == (float)(this.baseFreebiesCount() / TemplateManager.global.pointsPerCPSliderTick) && this.controlPointAIFreebieBonusSlider.value == TemplateManager.global.AI_BonusFreeCPCap_Difficulty(this.lastSelectedDifficulty + 1) / (float)TemplateManager.global.pointsPerCPSliderTick && this.missionControlFreebieBonusSlider.value == 0f && this.missionControlAIFreebieBonusSlider.value == TemplateManager.global.AI_BonusFreeMissionControl_Difficulty(this.lastSelectedDifficulty + 1) / (float)TemplateManager.global.pointsPerMCSliderTick && this.miningRatePlayerSlider.value <= 20f && this.habConstructionSpeedPlayerSlider.value <= 20f && this.shipConstructionSpeedPlayerSlider.value <= 20f && this.miningRateHumanAISlider.value == 20f && this.habConstructionSpeedHumanAISlider.value == 20f && this.shipConstructionSpeedHumanAISlider.value == 20f && this.miningRateAlienSlider.value == 20f && this.habConstructionSpeedAlienSlider.value == 20f && this.shipConstructionSpeedAlienSlider.value == 20f && this.customStartingNationGroupDropdown.value == 0 && this.variableProjectUnlocksToggle.isOn && !this.showtriggeredProjectsToggle.isOn)
		{
			this.DisableCustomDifficulty();
			return;
		}
		this.EnableCustomDifficulty();
	}

	// Token: 0x060015AA RID: 5546 RVA: 0x0006D94C File Offset: 0x0006BB4C
	public void OnSelectCampaignPreset(float speedFactor = 2f)
	{
		this.ResetCampaignDifficultyOptions();
		this.nationalIPModifierSlider.value = 4f * speedFactor;
		this.alienProgressionMultiplierSlider.value = 20f * speedFactor;
		this.miningProductivityMultiplierSlider.value = (float)this.defaultMiningProductivity * speedFactor;
		this.researchSpeedMultiplierSlider.value = 20f * speedFactor;
		this.DisableCustomDifficulty();
	}

	// Token: 0x060015AB RID: 5547 RVA: 0x0006D9B0 File Offset: 0x0006BBB0
	public void OnChangedNationGroup()
	{
		bool flag = !this.otherFactionStartingNations.interactable;
		this.otherFactionStartingNations.interactable = this.customStartingNationGroupDropdown.value != 0;
		if (!this.otherFactionStartingNations.interactable)
		{
			this.otherFactionStartingNations.SetIsOnWithoutNotify(false);
		}
		else if (flag)
		{
			this.otherFactionStartingNations.SetIsOnWithoutNotify(true);
		}
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015AC RID: 5548 RVA: 0x0006DA18 File Offset: 0x0006BC18
	public void UpdateAverageMonthlyEventsSlider()
	{
		this.averageMonthlyEventsModifierValue.SetText(this.averageMonthlyEventsModifierSlider.value.ToString());
	}

	// Token: 0x060015AD RID: 5549 RVA: 0x0006DA43 File Offset: 0x0006BC43
	public void UpdateNationalIPModifierSlider()
	{
		this.nationalIPModifierValue.SetText(this.GetNationalIPMultiplier().ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015AE RID: 5550 RVA: 0x0006DA66 File Offset: 0x0006BC66
	public void UpdateResearchSpeedSlider()
	{
		this.researchSpeedValue.SetText(this.GetResearchSpeedMultiplier().ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0006DA89 File Offset: 0x0006BC89
	public void UpdateAlienProgressionSpeedSlider()
	{
		this.alienProgressionRateValue.SetText(this.GetAlienProgressionSpeedMultiplier().ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x0006DAAC File Offset: 0x0006BCAC
	public void UpdateMiningProductivySpeedSlider(bool updateDifficulty = true)
	{
		this.miningProductivityValue.SetText(this.GetMiningProductivitySpeedMultiplier().ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x0006DAD0 File Offset: 0x0006BCD0
	public void UpdateMiningRateSlider()
	{
		this.miningRatePlayerValue.SetText(this.GetSliderMultiplier(this.miningRatePlayerSlider, 0.05f).ToPercent("P0"));
		this.miningRateHumanAIValue.SetText(this.GetSliderMultiplier(this.miningRateHumanAISlider, 0.05f).ToPercent("P0"));
		this.miningRateAlienValue.SetText(this.GetSliderMultiplier(this.miningRateAlienSlider, 0.05f).ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x0006DB58 File Offset: 0x0006BD58
	public void UpdateHabConstructionSpeedSlider()
	{
		this.habConstructionSpeedPlayerValue.SetText(this.GetSliderMultiplier(this.habConstructionSpeedPlayerSlider, 0.05f).ToPercent("P0"));
		this.habConstructionSpeedHumanAIValue.SetText(this.GetSliderMultiplier(this.habConstructionSpeedHumanAISlider, 0.05f).ToPercent("P0"));
		this.habConstructionSpeedAlienValue.SetText(this.GetSliderMultiplier(this.habConstructionSpeedAlienSlider, 0.05f).ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x0006DBE0 File Offset: 0x0006BDE0
	public void UpdateShipConstructionSpeedSlider()
	{
		this.shipConstructionSpeedPlayerValue.SetText(this.GetSliderMultiplier(this.shipConstructionSpeedPlayerSlider, 0.05f).ToPercent("P0"));
		this.shipConstructionSpeedHumanAIValue.SetText(this.GetSliderMultiplier(this.shipConstructionSpeedHumanAISlider, 0.05f).ToPercent("P0"));
		this.shipConstructionSpeedAlienValue.SetText(this.GetSliderMultiplier(this.shipConstructionSpeedAlienSlider, 0.05f).ToPercent("P0"));
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x0006DC68 File Offset: 0x0006BE68
	public void UpdateControlPointFreebieSlider(bool updateDifficulty = true)
	{
		this.controlPointFreebieValue.SetText(this.GetTotalCPFreebieBonus().ToString());
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x0006DC94 File Offset: 0x0006BE94
	public void UpdateControlPointAIFreebieSlider()
	{
		this.controlPointAIFreebieValue.SetText(this.GetAICPFreebiesBonus().ToString());
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x0006DCC0 File Offset: 0x0006BEC0
	public void UpdateMCFreebieSlider()
	{
		this.missionControlFreebieValue.SetText(this.GetMCFreebiesBonus().ToString());
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x0006DCEC File Offset: 0x0006BEEC
	public void UpdateMCAIFreebieSlider()
	{
		this.missionControlAIFreebieValue.SetText(this.GetAIMCFreebiesBonus().ToString());
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x0006DD18 File Offset: 0x0006BF18
	public void OnToggleVariableProjectUnlocks()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x0006DD2C File Offset: 0x0006BF2C
	public void OnToggleShowTriggeredProjects()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		this.ValidateCustomDifficultySettings();
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x0006DD40 File Offset: 0x0006BF40
	public void OnToggleRandomizeMapSetting()
	{
		this.mapSeedInputGO.SetActive(this.randomizeMapToggle.isOn);
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x0006DD58 File Offset: 0x0006BF58
	public void PlayNewGameToggleAudio()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x0006DD66 File Offset: 0x0006BF66
	public void OnEndEditCustomFactionName(string newValue)
	{
		if (!this.customDisplayNameInput.wasCanceled)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x0006DD8E File Offset: 0x0006BF8E
	public void OnEndEditCustomFactionAdjective(string newValue)
	{
		if (!this.customAdjectiveInput.wasCanceled)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x0006DDB6 File Offset: 0x0006BFB6
	public void OnEndEditCustomFactionLeaderAddress(string newValue)
	{
		if (!this.customLeaderAddressInput.wasCanceled)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x0006DDDE File Offset: 0x0006BFDE
	public void OnEndEditCustomFactionFleet(string newValue)
	{
		if (!this.customFleetInput.wasCanceled)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			return;
		}
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x0006DE08 File Offset: 0x0006C008
	private void UpdateAllowedFactions()
	{
		int num = 0;
		using (IEnumerator<object> enumerator = this.factionToggleListManager.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (StartMenuController.<>o__370.<>p__0 == null)
				{
					StartMenuController.<>o__370.<>p__0 = CallSite<Func<CallSite, object, FactionToggleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionToggleListItemController), typeof(StartMenuController)));
				}
				StartMenuController.<>o__370.<>p__0.Target(StartMenuController.<>o__370.<>p__0, enumerator.Current).UpdateForDefaultFactions(this.factionsInScenario);
				num++;
			}
		}
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x0006DEA0 File Offset: 0x0006C0A0
	public void ValidateFactionRequirements()
	{
		FactionToggleListItemController factionToggleListItemController = null;
		bool flag = true;
		using (IEnumerator<object> enumerator = this.factionToggleListManager.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (StartMenuController.<>o__371.<>p__0 == null)
				{
					StartMenuController.<>o__371.<>p__0 = CallSite<Func<CallSite, object, FactionToggleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionToggleListItemController), typeof(StartMenuController)));
				}
				FactionToggleListItemController factionToggleListItemController2 = StartMenuController.<>o__371.<>p__0.Target(StartMenuController.<>o__371.<>p__0, enumerator.Current);
				if (factionToggleListItemController2.factionToggle.isOn && factionToggleListItemController2.faction.allowedSoleAntiAlien)
				{
					flag = false;
				}
				if (factionToggleListItemController2.faction.defaultAntiAlien && factionToggleListItemController == null)
				{
					factionToggleListItemController = factionToggleListItemController2;
				}
			}
		}
		if (flag && factionToggleListItemController != null)
		{
			factionToggleListItemController.factionToggle.isOn = true;
		}
		int num = TemplateManager.global.controlPointMaintenanceFreebies;
		if (this.CurrentlySelectedHumanFactionsForCampaign() < 7)
		{
			num += (7 - this.CurrentlySelectedHumanFactionsForCampaign()) * TemplateManager.global.controlPointBonusMaintenanceFreebiesPerRemovedFaction;
		}
		this.controlPointFreebieBonusSlider.SetValueWithoutNotify((float)(num / TemplateManager.global.pointsPerCPSliderTick));
		this.UpdateControlPointFreebieSlider(false);
	}

	// Token: 0x060015C2 RID: 5570 RVA: 0x0006DFCC File Offset: 0x0006C1CC
	private void EnableCustomDifficulty()
	{
		this.customDifficulty = true;
		this.difficultyWarningObject.SetActive(true);
		this.difficultyWarningObjectOptions.SetActive(true);
		this.selectDifficultyDropdown.captionText.SetText(Loc.T(new StringBuilder("UI.Options.Difficulty").Append(this.selectDifficultyDropdown.value + 1).ToString()) + Loc.T("UI.Options.DifficultyCustom"));
	}

	// Token: 0x060015C3 RID: 5571 RVA: 0x0006E040 File Offset: 0x0006C240
	private void DisableCustomDifficulty()
	{
		this.customDifficulty = false;
		this.difficultyWarningObject.SetActive(false);
		this.difficultyWarningObjectOptions.SetActive(false);
		this.selectDifficultyDropdown.captionText.SetText(Loc.T(new StringBuilder("UI.Options.Difficulty").Append(this.selectDifficultyDropdown.value + 1).ToString()));
	}

	// Token: 0x060015C4 RID: 5572 RVA: 0x0006E0A2 File Offset: 0x0006C2A2
	public int GetTotalCPFreebieBonus()
	{
		return (int)((float)TemplateManager.global.pointsPerCPSliderTick * this.controlPointFreebieBonusSlider.value);
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x0006E0BC File Offset: 0x0006C2BC
	public int GetAdditionalCPFreebiesBonus()
	{
		return (int)((float)TemplateManager.global.pointsPerCPSliderTick * this.controlPointFreebieBonusSlider.value);
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x0006E0D6 File Offset: 0x0006C2D6
	public int GetAICPFreebiesBonus()
	{
		return (int)((float)TemplateManager.global.pointsPerCPSliderTick * this.controlPointAIFreebieBonusSlider.value);
	}

	// Token: 0x060015C7 RID: 5575 RVA: 0x0006E0F0 File Offset: 0x0006C2F0
	public int GetMCFreebiesBonus()
	{
		return (int)((float)TemplateManager.global.pointsPerMCSliderTick * this.missionControlFreebieBonusSlider.value);
	}

	// Token: 0x060015C8 RID: 5576 RVA: 0x0006E10A File Offset: 0x0006C30A
	public int GetAIMCFreebiesBonus()
	{
		return (int)((float)TemplateManager.global.pointsPerMCSliderTick * this.missionControlAIFreebieBonusSlider.value);
	}

	// Token: 0x060015C9 RID: 5577 RVA: 0x0006E124 File Offset: 0x0006C324
	public float GetResearchSpeedMultiplier()
	{
		return 0.05f * this.researchSpeedMultiplierSlider.value;
	}

	// Token: 0x060015CA RID: 5578 RVA: 0x0006E137 File Offset: 0x0006C337
	public float GetAlienProgressionSpeedMultiplier()
	{
		return 0.05f * this.alienProgressionMultiplierSlider.value;
	}

	// Token: 0x060015CB RID: 5579 RVA: 0x0006E14A File Offset: 0x0006C34A
	public float GetMiningProductivitySpeedMultiplier()
	{
		return 0.05f * this.miningProductivityMultiplierSlider.value;
	}

	// Token: 0x060015CC RID: 5580 RVA: 0x0006E15D File Offset: 0x0006C35D
	public float GetSliderMultiplier(Slider slider, float rate)
	{
		return rate * slider.value;
	}

	// Token: 0x060015CD RID: 5581 RVA: 0x0006E167 File Offset: 0x0006C367
	public float GetNationalIPMultiplier()
	{
		return 0.25f * this.nationalIPModifierSlider.value;
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x0006E17C File Offset: 0x0006C37C
	private int CurrentlySelectedHumanFactionsForCampaign()
	{
		int num = 0;
		using (IEnumerator<object> enumerator = this.factionToggleListManager.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (StartMenuController.<>o__384.<>p__0 == null)
				{
					StartMenuController.<>o__384.<>p__0 = CallSite<Func<CallSite, object, FactionToggleListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionToggleListItemController), typeof(StartMenuController)));
				}
				FactionToggleListItemController factionToggleListItemController = StartMenuController.<>o__384.<>p__0.Target(StartMenuController.<>o__384.<>p__0, enumerator.Current);
				if (factionToggleListItemController.factionToggle.isOn && !factionToggleListItemController.faction.isAlien)
				{
					num++;
				}
			}
		}
		return num;
	}

	// Token: 0x060015CF RID: 5583 RVA: 0x0006E224 File Offset: 0x0006C424
	private int baseFreebiesCount()
	{
		int num = TemplateManager.global.controlPointMaintenanceFreebies;
		if (this.factionsInScenario.Where<TIFactionTemplate>((TIFactionTemplate o) => !o.isAlien).Count<TIFactionTemplate>() < 7)
		{
			num += (7 - this.factionsInScenario.Where<TIFactionTemplate>((TIFactionTemplate o) => !o.isAlien).Count<TIFactionTemplate>()) * TemplateManager.global.controlPointBonusMaintenanceFreebiesPerRemovedFaction;
		}
		return num;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x0006E2AE File Offset: 0x0006C4AE
	private void UpdateBaseCPForDefaultFactions()
	{
		this.controlPointFreebieBonusSlider.SetValueWithoutNotify((float)(this.baseFreebiesCount() / TemplateManager.global.pointsPerCPSliderTick));
		this.UpdateControlPointFreebieSlider(false);
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x0006E2D4 File Offset: 0x0006C4D4
	public void OnClickSetDefaultCampaignOptions()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		this.SetDefaultCampaignOptions();
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x0006E2E8 File Offset: 0x0006C4E8
	public void OnClickSetAcceleratedCampaignOptions()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		this.OnSelectCampaignPreset(2f);
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x0006E301 File Offset: 0x0006C501
	public void OnClickSetPreviousCampaignOptions()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
		this.SetPreviousCampaignOptions();
	}

	// Token: 0x060015D4 RID: 5588 RVA: 0x0006E318 File Offset: 0x0006C518
	private void SetPreviousCampaignOptions()
	{
		this.customStartingNationGroupDropdown.value = TIPlayerProfileManager.storedCampaignOptions.customFactionStartingNationGroup;
		this.startingCouncilor1Profession.value = TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions[0] - (this.tutorial ? 1 : 0);
		this.startingCouncilor2Profession.value = TIPlayerProfileManager.storedCampaignOptions.startingCouncilorProfessions[1] - (this.tutorial ? 1 : 0);
		this.firstCouncilorHomeNationToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.usePlayerCountryForStartingCouncilor;
		this.variableProjectUnlocksToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.variableProjectUnlocks;
		this.showtriggeredProjectsToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.showTriggeredProjects;
		this.addAlienAssaultFleetToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.addAlienAssaultCarrierFleet;
		this.realismCombatDVMovementToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.cinematicCombatRealismDV;
		this.realismCombatScaleToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.cinematicCombatRealismScale;
		this.otherFactionStartingNations.isOn = TIPlayerProfileManager.storedCampaignOptions.otherFactionStartingNations;
		this.canDisableFactionsToggle.isOn = TIPlayerProfileManager.storedCampaignOptions.canDisableFactions;
		this.researchSpeedMultiplierSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.researchSpeedMultiplier;
		this.controlPointFreebieBonusSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.controlPointMaintenanceFreebieBonus;
		this.controlPointAIFreebieBonusSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.controlPointMaintenanceFreebieBonusAI;
		this.missionControlFreebieBonusSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.missionControlBonus;
		this.missionControlAIFreebieBonusSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.missionControlBonusAI;
		this.alienProgressionMultiplierSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.alienProgressionSpeed;
		this.miningProductivityMultiplierSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.miningProductivityMultiplier;
		this.nationalIPModifierSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.nationalIPMultiplier;
		this.averageMonthlyEventsModifierSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.averageMonthlyEvents;
		this.miningRateHumanAISlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.miningRateHumanAI;
		this.miningRatePlayerSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.miningRatePlayer;
		this.miningRateAlienSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.miningRateAlien;
		this.habConstructionSpeedPlayerSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.habConstructionSpeedPlayer;
		this.habConstructionSpeedHumanAISlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.habConstructionSpeedHumanAI;
		this.habConstructionSpeedAlienSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.habConstructionSpeedAlien;
		this.shipConstructionSpeedPlayerSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.shipConstructionSpeedPlayer;
		this.shipConstructionSpeedHumanAISlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.shipConstructionSpeedHumanAI;
		this.shipConstructionSpeedAlienSlider.value = (float)TIPlayerProfileManager.storedCampaignOptions.shipConstructionSpeedAlien;
	}

	// Token: 0x060015D5 RID: 5589 RVA: 0x0006E5BE File Offset: 0x0006C7BE
	private void SetDefaultCampaignOptions()
	{
		this.ResetAllCustomizations();
		this.LoadFactionTextFields();
	}

	// Token: 0x060015D6 RID: 5590 RVA: 0x0006E5CC File Offset: 0x0006C7CC
	public void OnClickCloseTutorialRecommendation()
	{
		this.firstGameTutorialObject.SetActive(false);
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x0006E5DA File Offset: 0x0006C7DA
	public void OnClickDiscordLink()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
		TIUtilities.OpenWebURL("https://discord.com/invite/eQaRRq3y3M");
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x0006E5F2 File Offset: 0x0006C7F2
	public void OnClickWikiLink()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
		TIUtilities.OpenWebURL("https://hoodedhorse.com/wiki/Terra_Invicta/The_Official_Terra_Invicta_Wiki");
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x0006E60A File Offset: 0x0006C80A
	public void OnClickPavonisLink()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
		TIUtilities.OpenWebURL("https://www.pavonisinteractive.com/phpBB3/viewforum.php?f=26");
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x0006E622 File Offset: 0x0006C822
	public void OnClickDarkSkiesLink()
	{
		AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
		if (Application.isEditor)
		{
			TIUtilities.OpenWebURL("https://store.steampowered.com/app/4713340/Terra_Invicta__Dark_Skies/");
			return;
		}
		SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/4713340/Terra_Invicta__Dark_Skies/", EActivateGameOverlayToWebPageMode.k_EActivateGameOverlayToWebPageMode_Default);
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x0006E64D File Offset: 0x0006C84D
	public static void ForceCredits()
	{
		StartMenuController.forceCredits = true;
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0006E658 File Offset: 0x0006C858
	public void BuildCreditsStrings()
	{
		this.TICreditsStrings.Clear();
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		for (int i = 0; i < TemplateManager.global.creditsEntries; i++)
		{
			string text = new StringBuilder("UI.StartScreen.TICredits_").Append(i.ToString()).ToString();
			string text2 = Loc.T(text);
			if (!(text2 == text))
			{
				stringBuilder.Append(text2);
				num++;
				if (num > 300)
				{
					this.TICreditsStrings.Add(stringBuilder.ToString());
					stringBuilder.Clear();
					num = 0;
				}
			}
		}
		this.TICreditsStrings.Add(stringBuilder.ToString());
		for (int j = 0; j < this.TICreditsStrings.Count; j++)
		{
			if (j > 0 && this.TICreditsList.Count - 1 < j)
			{
				TMP_Text component = global::UnityEngine.Object.Instantiate<GameObject>(this.TICreditsList[0].gameObject, this.TICreditsList[0].transform.parent).GetComponent<TMP_Text>();
				this.TICreditsList.Add(component);
			}
			this.TICreditsList[j].SetText(this.TICreditsStrings[j]);
		}
		this.TICreditsList[0].transform.parent.gameObject.SetActive(true);
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x0006E7AE File Offset: 0x0006C9AE
	public void BankModFailureWarning(string headerLoc, string descLoc, string locArg1, string locArg2)
	{
		this.bankedModFailure = true;
		this.bankedModWarningHeaderLoc = headerLoc;
		this.bankedModWarningDescLoc = descLoc;
		this.bankedModWarningLocArg1 = locArg1;
		this.bankedModWarningLocArg2 = locArg2;
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0006E7D4 File Offset: 0x0006C9D4
	public void ShowModFailureDialog(string warningHeader, string warningDesc)
	{
		if (this.fatalStartupError)
		{
			this.fatalErrorBG.SetActive(true);
		}
		this.modLoaderWarningDialog.SetActive(true);
		this.modLoaderWarningHeaderText.SetText(warningHeader);
		this.modLoaderWarningDescriptionText.SetText(warningDesc);
		this.modLoaderWarningConfirmText.SetText(Loc.T("UI.Councilor.Orgs.AcknowledgeButton"));
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x0006E82E File Offset: 0x0006CA2E
	public void OnClickCloseModLoaderWarningDialog()
	{
		if (this.fatalStartupError)
		{
			Debug.Log("Closing Terra Invicta");
			Application.Quit();
		}
		this.modLoaderWarningDialog.SetActive(false);
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x0006E854 File Offset: 0x0006CA54
	public void InitializeSkirmishMenu()
	{
		this.skirmishScenario = new SkirmishModeScenario();
		this.skirmishScenario.OnStartScene();
		this.ships = TemplateManager.IterateByClass<TISpaceShipTemplate>(true).ToList<TISpaceShipTemplate>();
		this.shipDictionary.Clear();
		int num = 0;
		foreach (TISpaceShipTemplate tispaceShipTemplate in this.ships)
		{
			tispaceShipTemplate.SetClassDisplayName(false);
			if (this.shipDictionary.ContainsKey(tispaceShipTemplate.fullClassName))
			{
				TISpaceShipTemplate tispaceShipTemplate2 = tispaceShipTemplate;
				string displayName = tispaceShipTemplate.displayName;
				string text = "_";
				int num2;
				num = (num2 = num + 1);
				tispaceShipTemplate2.SetDisplayName(displayName + text + num2.ToString());
			}
			this.shipDictionary.Add(tispaceShipTemplate.fullClassName, tispaceShipTemplate);
		}
		if (GameControl.spaceCombat.prevSkirmishSettings != null)
		{
			foreach (TISpaceShipTemplate tispaceShipTemplate3 in GameControl.spaceCombat.prevSkirmishSettings.importedShips)
			{
				tispaceShipTemplate3.SetClassDisplayName(false);
				if (this.shipDictionary.ContainsKey(tispaceShipTemplate3.fullClassName))
				{
					TISpaceShipTemplate tispaceShipTemplate4 = tispaceShipTemplate3;
					string displayName2 = tispaceShipTemplate3.displayName;
					string text2 = "_";
					int num2;
					num = (num2 = num + 1);
					tispaceShipTemplate4.SetDisplayName(displayName2 + text2 + num2.ToString());
				}
				if (!this.ships.Contains(tispaceShipTemplate3))
				{
					this.ships.Add(tispaceShipTemplate3);
				}
				this.shipDictionary.Add(tispaceShipTemplate3.fullClassName, tispaceShipTemplate3);
				TemplateManager.Add(tispaceShipTemplate3, typeof(TISpaceShipTemplate), false);
			}
			List<TISpaceFleetTemplate> list = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].factionName = GameControl.spaceCombat.prevSkirmishSettings.fleetTemplates[i].factionName;
				list[i].orbitTemplateName = GameControl.spaceCombat.prevSkirmishSettings.fleetTemplates[i].orbitTemplateName;
				list[i].shipsInFleet.Clear();
				for (int j = 0; j < GameControl.spaceCombat.prevSkirmishSettings.fleetTemplates[i].shipsInFleet.Count; j++)
				{
					list[i].shipsInFleet.Add(new TISpaceFleetTemplate.ShipFleetDefinition(GameControl.spaceCombat.prevSkirmishSettings.fleetTemplates[i].shipsInFleet[j].shipTemplateName));
				}
				this.factionFleetGradient[i].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[i].factionTemplate.gradientPath);
				this.factionFleetCenterGradient[i].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(TemplateManager.global.pathUndecidedGradient);
				this.factionFleetIcon[i].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[i].factionTemplate.councilIcon128);
				this.factionFleetBackgroundIcon[i].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[i].factionTemplate.councilIcon256);
			}
			this.skirmishScenario.habTemplate = GameControl.spaceCombat.prevSkirmishSettings.habTemplate;
		}
		this.UpdateImportedShips();
		Log.Time("<color=#00cc00>LoadTime:</color> PopulateSkirmishDropdowns", delegate
		{
			this.PopulateSkirmishDropdowns();
		}, true, true);
		this.habFactionText.SetText(string.Empty);
	}

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x060015E1 RID: 5601 RVA: 0x0006EC2C File Offset: 0x0006CE2C
	// (set) Token: 0x060015E2 RID: 5602 RVA: 0x0006EC34 File Offset: 0x0006CE34
	public List<TISpaceShipTemplate> ImportedShipTemplates
	{
		get
		{
			return StartMenuController.importedShipTemplates;
		}
		set
		{
			foreach (TISpaceShipTemplate tispaceShipTemplate in this.ships.Intersect<TISpaceShipTemplate>(StartMenuController.importedShipTemplates).ToList<TISpaceShipTemplate>())
			{
				this.ships.Remove(this.shipDictionary[tispaceShipTemplate.fullClassName]);
				this.shipDictionary.Remove(tispaceShipTemplate.fullClassName);
			}
			StartMenuController.importedShipTemplates = value;
			this.UpdateImportedShips();
		}
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x0006ECCC File Offset: 0x0006CECC
	public void UpdateImportedShips()
	{
		foreach (TISpaceShipTemplate tispaceShipTemplate in StartMenuController.importedShipTemplates.Except<TISpaceShipTemplate>(this.ships).ToList<TISpaceShipTemplate>())
		{
			if (this.shipDictionary.ContainsKey(tispaceShipTemplate.fullClassName))
			{
				tispaceShipTemplate.SetDisplayName("Imported " + tispaceShipTemplate.displayName);
			}
			this.ships.Add(tispaceShipTemplate);
			this.shipDictionary[tispaceShipTemplate.fullClassName] = tispaceShipTemplate;
			tispaceShipTemplate.RenameDataName(TemplateManager.GenerateDataName("importedShipTemplate"));
			TemplateManager.Add(tispaceShipTemplate, typeof(TISpaceShipTemplate), false);
		}
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x0006ED90 File Offset: 0x0006CF90
	public void PopulateSkirmishDropdowns()
	{
		this.skirmishLocationSettingDropdown.ClearOptions();
		this.skirmishHabDropdown.ClearOptions();
		this.locationDictionary.Clear();
		this.habDictionary.Clear();
		List<TISpaceFleetTemplate> list = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x);
		List<string> list2 = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TINaturalSpaceObjectTemplate)).ConvertAll<TINaturalSpaceObjectTemplate>((TIDataTemplate x) => (TINaturalSpaceObjectTemplate)x).SelectMany<TINaturalSpaceObjectTemplate, string>((TINaturalSpaceObjectTemplate x) => x.orbits)
			.ToList<string>();
		List<TIOrbitTemplate> list3 = new List<TIOrbitTemplate>();
		foreach (string text in list2)
		{
			TIOrbitTemplate orbit = TemplateManager.Find<TIOrbitTemplate>(text, false);
			if (orbit != null && list3.None<TIOrbitTemplate>((TIOrbitTemplate x) => x.barycenterName == orbit.barycenterName))
			{
				list3.Add(orbit);
			}
		}
		List<TIHabTemplate> list4 = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TIHabTemplate)).ConvertAll<TIHabTemplate>((TIDataTemplate x) => (TIHabTemplate)x);
		list4.Insert(0, null);
		string text2 = TemplateManager.Find<TIOrbitTemplate>(list[0].orbitTemplateName, false).barycenterTemplate.displayNameCurrentForStartScreen();
		TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
		foreach (TIOrbitTemplate tiorbitTemplate in list3)
		{
			TINaturalSpaceObjectTemplate barycenterTemplate = tiorbitTemplate.barycenterTemplate;
			if (!this.locationDictionary.ContainsKey(barycenterTemplate.displayNameCurrentForStartScreen()))
			{
				this.locationDictionary.Add(barycenterTemplate.displayNameCurrentForStartScreen(), tiorbitTemplate);
				TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData(barycenterTemplate.displayNameCurrentForStartScreen());
				this.skirmishLocationSettingDropdown.options.Add(optionData2);
				if (text2 == barycenterTemplate.displayNameCurrentForStartScreen())
				{
					optionData = optionData2;
				}
			}
		}
		this.skirmishLocationSettingDropdown.value = this.skirmishLocationSettingDropdown.options.IndexOf(optionData);
		this.skirmishLocationSettingDropdown.RefreshShownValue();
		TMP_Dropdown.OptionData optionData3 = new TMP_Dropdown.OptionData();
		foreach (TIHabTemplate tihabTemplate in list4)
		{
			TMP_Dropdown.OptionData optionData4 = null;
			if (tihabTemplate == null)
			{
				this.habDictionary.Add(Loc.T("UI.StartScreen.Skirmish.NoHab"), null);
				optionData4 = new TMP_Dropdown.OptionData(Loc.T("UI.StartScreen.Skirmish.NoHab"));
			}
			else if (tihabTemplate.habType == HabType.Station)
			{
				this.habDictionary.Add(tihabTemplate.displayNameCurrentForStartScreen(), tihabTemplate);
				optionData4 = new TMP_Dropdown.OptionData(tihabTemplate.displayNameCurrentForStartScreen());
			}
			if (optionData4 != null)
			{
				this.skirmishHabDropdown.options.Add(optionData4);
				if (this.skirmishScenario.habTemplate != tihabTemplate)
				{
					TIHabTemplate habTemplate = this.skirmishScenario.habTemplate;
					if (!(((habTemplate != null) ? habTemplate.dataName : null) == ((tihabTemplate != null) ? tihabTemplate.dataName : null)))
					{
						continue;
					}
				}
				optionData3 = optionData4;
			}
		}
		this.skirmishHabDropdown.value = this.skirmishHabDropdown.options.IndexOf(optionData3);
		this.skirmishHabDropdown.RefreshShownValue();
		List<TIFactionTemplate> list5 = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TIFactionTemplate)).ConvertAll<TIFactionTemplate>((TIDataTemplate x) => (TIFactionTemplate)x);
		this.factionDictionary.Clear();
		foreach (TIFactionTemplate tifactionTemplate in list5)
		{
			this.factionDictionary.Add(tifactionTemplate.capitalizedFactionNameCurrent, tifactionTemplate);
		}
		for (int i = 0; i <= 1; i++)
		{
			this.skirmishFactionDropdown[i].ClearOptions();
			TMP_Dropdown.OptionData optionData5 = new TMP_Dropdown.OptionData();
			foreach (TIFactionTemplate tifactionTemplate2 in list5)
			{
				TMP_Dropdown.OptionData optionData6 = new TMP_Dropdown.OptionData(tifactionTemplate2.capitalizedFactionNameCurrent);
				this.skirmishFactionDropdown[i].options.Add(optionData6);
				if (list[i].factionTemplate.capitalizedFactionNameCurrent == tifactionTemplate2.capitalizedFactionNameCurrent)
				{
					optionData5 = optionData6;
				}
			}
			this.skirmishFactionDropdown[i].value = this.skirmishFactionDropdown[i].options.IndexOf(optionData5);
			this.skirmishFactionDropdown[i].RefreshShownValue();
			list[i].shipsInFleet = list[i].filteredShipsInFleet;
			this.skirmishShipLists[i].SetListSize<SkirmishShipListItemController>(list[i].shipsInFleet.Count, false, false);
			int num = -1;
			this.skirmishShipListDropdowns[i].Initialize(this, list[i], num++, i);
			using (IEnumerator<object> enumerator5 = this.skirmishShipLists[i].GetEnumerator())
			{
				while (enumerator5.MoveNext())
				{
					if (StartMenuController.<>o__418.<>p__0 == null)
					{
						StartMenuController.<>o__418.<>p__0 = CallSite<Func<CallSite, object, SkirmishShipListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SkirmishShipListItemController), typeof(StartMenuController)));
					}
					StartMenuController.<>o__418.<>p__0.Target(StartMenuController.<>o__418.<>p__0, enumerator5.Current).Initialize(this, list[i], num++, i);
				}
			}
		}
		this.SetFleetScores(list[0], list[1]);
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x0006F3BC File Offset: 0x0006D5BC
	public void SetFleetScores(TISpaceFleetTemplate fleet1, TISpaceFleetTemplate fleet2)
	{
		float num = 0f;
		using (IEnumerator<string> enumerator = fleet1.filteredShipsInFleet.Select<TISpaceFleetTemplate.ShipFleetDefinition, string>((TISpaceFleetTemplate.ShipFleetDefinition x) => x.shipTemplateName).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string ship2 = enumerator.Current;
				num += this.ships.Single<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.dataName == ship2).TemplateSpaceCombatValue(false, -1f, 1f, false);
			}
		}
		float num2 = 0f;
		using (IEnumerator<string> enumerator = fleet2.filteredShipsInFleet.Select<TISpaceFleetTemplate.ShipFleetDefinition, string>((TISpaceFleetTemplate.ShipFleetDefinition x) => x.shipTemplateName).GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				string ship = enumerator.Current;
				num2 += this.ships.Single<TISpaceShipTemplate>((TISpaceShipTemplate x) => x.dataName == ship).TemplateSpaceCombatValue(false, -1f, 1f, false);
			}
		}
		if (this.skirmishScenario.habTemplate != null)
		{
			float num3 = this.skirmishScenario.habTemplate.sectors.Sum<SectorTemplate>((SectorTemplate x) => x.habModuleNames.Sum<string>(delegate(string y)
			{
				TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(y, false);
				if (tihabModuleTemplate == null)
				{
					return 0f;
				}
				return tihabModuleTemplate.SpaceCombatValue(null, null, false) * (float)20;
			}));
			if (this.skirmishScenario.habTemplate.sectors[0].faction == fleet1.factionName)
			{
				num += num3;
			}
			else
			{
				num2 += num3;
			}
		}
		this.skirmishModePlayer1FleetScore.SetText(num.ToString("N0"));
		this.skirmishModePlayer2FleetScore.SetText(num2.ToString("N0"));
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x0006F5A0 File Offset: 0x0006D7A0
	public void OnSkirmishLocationChanged()
	{
		string text = this.skirmishLocationSettingDropdown.options[this.skirmishLocationSettingDropdown.value].text;
		foreach (TISpaceFleetTemplate tispaceFleetTemplate in TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x))
		{
			tispaceFleetTemplate.orbitTemplateName = this.locationDictionary[text].dataName;
		}
		foreach (TIHabTemplate tihabTemplate in from x in TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TIHabTemplate)).ConvertAll<TIHabTemplate>((TIDataTemplate x) => (TIHabTemplate)x)
			where x.habType == HabType.Station
			select x)
		{
			tihabTemplate.orbitTemplateName = this.locationDictionary[text].dataName;
		}
	}

	// Token: 0x060015E7 RID: 5607 RVA: 0x0006F700 File Offset: 0x0006D900
	public void OnShipDropdownChanged()
	{
		List<TISpaceFleetTemplate> list = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x);
		this.SetFleetScores(list[0], list[1]);
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x0006F760 File Offset: 0x0006D960
	public void OnFactionDropdownChanged(int fleetNum)
	{
		if (this.skirmishFactionDropdown[0].options[this.skirmishFactionDropdown[0].value].text == this.skirmishFactionDropdown[1].options[this.skirmishFactionDropdown[1].value].text)
		{
			int num;
			if (this.skirmishFactionDropdown[fleetNum].value > 0)
			{
				TMP_Dropdown tmp_Dropdown = this.skirmishFactionDropdown[fleetNum];
				num = tmp_Dropdown.value;
				tmp_Dropdown.value = num - 1;
				return;
			}
			TMP_Dropdown tmp_Dropdown2 = this.skirmishFactionDropdown[fleetNum];
			num = tmp_Dropdown2.value;
			tmp_Dropdown2.value = num + 1;
			return;
		}
		else
		{
			string text = this.skirmishFactionDropdown[fleetNum].options[this.skirmishFactionDropdown[fleetNum].value].text;
			List<TISpaceFleetTemplate> list = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x);
			list[fleetNum].factionName = this.factionDictionary[text].dataName;
			this.factionFleetGradient[fleetNum].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[fleetNum].factionTemplate.gradientPath);
			this.factionFleetCenterGradient[fleetNum].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[fleetNum].factionTemplate.gradientPath);
			this.factionFleetIcon[fleetNum].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[fleetNum].factionTemplate.councilIcon128);
			this.factionFleetBackgroundIcon[fleetNum].sprite = TIUtilities.assetLoader.LoadAssetForSpriteAssignment(list[fleetNum].factionTemplate.councilIcon256);
			if (this.skirmishScenario.habTemplate != null)
			{
				this.habFactionText.SetText(list[(this.skirmishScenario.habTemplate.sectors[0].faction == list[0].factionName) ? 0 : 1].factionTemplate.capitalizedFactionNameCurrent);
				for (int i = 0; i < 5; i++)
				{
					if (!string.IsNullOrWhiteSpace(this.skirmishScenario.habTemplate.sectors[i].faction))
					{
						this.skirmishScenario.habTemplate.sectors[i].faction = ((this.skirmishScenario.habTemplate.sectors[0].faction == list[0].factionName) ? list[0].factionName : list[1].factionName);
					}
				}
				return;
			}
			this.habFactionText.SetText(string.Empty);
			return;
		}
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x0006FA34 File Offset: 0x0006DC34
	public void OnHabDropdownChanged()
	{
		string text = this.skirmishHabDropdown.options[this.skirmishHabDropdown.value].text;
		this.skirmishScenario.habTemplate = this.habDictionary[text];
		if (this.skirmishScenario.habTemplate != null)
		{
			List<TISpaceFleetTemplate> list = TIMetaTemplate.GetTemplatesOfTypeFromMeta(this.skirmishScenario.scenarioTemplateName, typeof(TISpaceFleetTemplate)).ConvertAll<TISpaceFleetTemplate>((TIDataTemplate x) => (TISpaceFleetTemplate)x);
			this.habFactionText.SetText(list[(this.skirmishScenario.habTemplate.sectors[0].faction == list[0].factionName) ? 0 : 1].factionTemplate.capitalizedFactionNameCurrent);
			for (int i = 0; i < 5; i++)
			{
				if (!string.IsNullOrWhiteSpace(this.skirmishScenario.habTemplate.sectors[i].faction))
				{
					this.skirmishScenario.habTemplate.sectors[i].faction = ((this.skirmishScenario.habTemplate.sectors[0].faction == list[0].factionName) ? list[0].factionName : list[1].factionName);
				}
			}
			return;
		}
		this.habFactionText.SetText(string.Empty);
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x0006FBBD File Offset: 0x0006DDBD
	private void OnLanguageChangedEvent()
	{
		this.Initialize();
		this.LoadFactionTextFields();
		this.SetStarterNationOptions();
		Loc.SwapFonts(base.gameObject);
		this.modMenuController.RefreshInstalledMods();
	}

	// Token: 0x040012CF RID: 4815
	public CanvasGroup canvasGroup;

	// Token: 0x040012D0 RID: 4816
	public CanvasGroup buttonsCanvasGroup;

	// Token: 0x040012D1 RID: 4817
	public RectTransform newGamePrimaryPanelTransform;

	// Token: 0x040012D2 RID: 4818
	public RectTransform loadGamePrimaryPanelTransform;

	// Token: 0x040012D3 RID: 4819
	public RectTransform settingsPrimaryPanelTransform;

	// Token: 0x040012D4 RID: 4820
	public RectTransform moddingPrimaryPanelTransform;

	// Token: 0x040012D5 RID: 4821
	public RectTransform skirmishPrimaryPanelTransform;

	// Token: 0x040012D6 RID: 4822
	public RectTransform creditsPrimaryPanelTransform;

	// Token: 0x040012D7 RID: 4823
	public TMP_Text continueButtonText;

	// Token: 0x040012D8 RID: 4824
	public TMP_Text newGameText;

	// Token: 0x040012D9 RID: 4825
	public TMP_Text loadGameText;

	// Token: 0x040012DA RID: 4826
	public TMP_Text optionsText;

	// Token: 0x040012DB RID: 4827
	public TMP_Text skirmishModeText;

	// Token: 0x040012DC RID: 4828
	public TMP_Text modsText;

	// Token: 0x040012DD RID: 4829
	public TMP_Text creditsText;

	// Token: 0x040012DE RID: 4830
	public TMP_Text exitText;

	// Token: 0x040012DF RID: 4831
	public TMP_Text TICredits;

	// Token: 0x040012E0 RID: 4832
	public List<TMP_Text> TICreditsList = new List<TMP_Text>();

	// Token: 0x040012E1 RID: 4833
	public List<string> TICreditsStrings = new List<string>();

	// Token: 0x040012E2 RID: 4834
	public Button modMenuButton;

	// Token: 0x040012E3 RID: 4835
	public GameObject loadingScreen;

	// Token: 0x040012E4 RID: 4836
	public TMP_Text loadingText;

	// Token: 0x040012E5 RID: 4837
	private SceneManager sceneManager;

	// Token: 0x040012E6 RID: 4838
	private Dictionary<string, string> currentStartOptions = new Dictionary<string, string>();

	// Token: 0x040012E7 RID: 4839
	public GameObject DarkSkiesPromoObject;

	// Token: 0x040012E8 RID: 4840
	public GameObject DarkSkiesStoreLogoSteam;

	// Token: 0x040012E9 RID: 4841
	public GameObject DarkSkiesStoreLogoGog;

	// Token: 0x040012EA RID: 4842
	public GameObject DarkSkiesStoreLogoEpic;

	// Token: 0x040012EB RID: 4843
	public GameObject DarkSkiesStoreLogoMicrosoft;

	// Token: 0x040012EC RID: 4844
	public TMP_Text applicationVersionText;

	// Token: 0x040012ED RID: 4845
	public TMP_Text patchNotesText;

	// Token: 0x040012EE RID: 4846
	public TMP_Text dlcDateText;

	// Token: 0x040012EF RID: 4847
	public Button continueButton;

	// Token: 0x040012F0 RID: 4848
	public TooltipTrigger continueButtonTooltip;

	// Token: 0x040012F1 RID: 4849
	[Header("Controllers")]
	public MenuManager menuManager;

	// Token: 0x040012F2 RID: 4850
	public OptionsMenuController optionsController;

	// Token: 0x040012F3 RID: 4851
	public GraphicsMenuController graphicsController;

	// Token: 0x040012F4 RID: 4852
	public AudioMenuController audioController;

	// Token: 0x040012F5 RID: 4853
	public OptionsMenuController gameplayController;

	// Token: 0x040012F6 RID: 4854
	public ControlsMenuController controlsController;

	// Token: 0x040012F7 RID: 4855
	public LoadMenuController loadMenuController;

	// Token: 0x040012F8 RID: 4856
	public SkirmishMenuController skirmishMenuController;

	// Token: 0x040012F9 RID: 4857
	public ModMenuController modMenuController;

	// Token: 0x040012FA RID: 4858
	[Header("New Campaign UI")]
	public Transform newCampaignOptionList;

	// Token: 0x040012FB RID: 4859
	public GameObject newCampaignOptionPanel;

	// Token: 0x040012FC RID: 4860
	public TMP_Text newGamePanelHeader;

	// Token: 0x040012FD RID: 4861
	public TMP_Text newGameStartButtonText;

	// Token: 0x040012FE RID: 4862
	public TMP_Text StartAcceleratedCampaignButtonText;

	// Token: 0x040012FF RID: 4863
	public TMP_Text newGameSummaryText;

	// Token: 0x04001300 RID: 4864
	private FullScenario regularScenario;

	// Token: 0x04001301 RID: 4865
	public Button startLongCampaignButton;

	// Token: 0x04001302 RID: 4866
	public Button startAcceleratedCampaignButton;

	// Token: 0x04001303 RID: 4867
	public Button previousCampaignSettingsButton;

	// Token: 0x04001304 RID: 4868
	private bool tutorial;

	// Token: 0x04001305 RID: 4869
	public TMP_Text selectFactionDropdownHeader;

	// Token: 0x04001306 RID: 4870
	public Toggle tutorialToggle;

	// Token: 0x04001307 RID: 4871
	public TMP_Text tutorialToggleText;

	// Token: 0x04001308 RID: 4872
	public Toggle skirmishTutorialToggle;

	// Token: 0x04001309 RID: 4873
	public TMP_Text skirmishTutorialToggleText;

	// Token: 0x0400130A RID: 4874
	private List<string> candidateFactionDataNames;

	// Token: 0x0400130B RID: 4875
	public TMP_Dropdown newCampaignChooseFactionDropdown;

	// Token: 0x0400130C RID: 4876
	public TMP_Text selectedFactionDescription;

	// Token: 0x0400130D RID: 4877
	public Image selectedFactionGradient;

	// Token: 0x0400130E RID: 4878
	public Image selectedFactionIconBold;

	// Token: 0x0400130F RID: 4879
	public Image selectedFactionIconFaded;

	// Token: 0x04001310 RID: 4880
	private List<TIFactionTemplate> currentAllowedFactions;

	// Token: 0x04001311 RID: 4881
	private List<TIFactionTemplate> factionsInScenario;

	// Token: 0x04001312 RID: 4882
	private List<TINationTemplate> nationsInScenario;

	// Token: 0x04001313 RID: 4883
	private List<TICouncilorTypeTemplate> allProfessions = new List<TICouncilorTypeTemplate>();

	// Token: 0x04001314 RID: 4884
	private string selectedFactionDataName;

	// Token: 0x04001315 RID: 4885
	public TMP_Text selectDifficultyHeader;

	// Token: 0x04001316 RID: 4886
	public TMP_Dropdown selectDifficultyDropdown;

	// Token: 0x04001317 RID: 4887
	public GameObject firstGameTutorialObject;

	// Token: 0x04001318 RID: 4888
	public TMP_Text recommendTutorialDescText;

	// Token: 0x04001319 RID: 4889
	public TMP_Text recommendTutorialButtonText;

	// Token: 0x0400131A RID: 4890
	[Header("Faction Customization")]
	public GameObject factionCustomizationObject;

	// Token: 0x0400131B RID: 4891
	public TMP_Text factionCustomizeButton;

	// Token: 0x0400131C RID: 4892
	public TMP_Text factionCustomizeDefaultButton;

	// Token: 0x0400131D RID: 4893
	public TMP_Text factionCustomizeHeader;

	// Token: 0x0400131E RID: 4894
	public TMP_Text factionCustomizeDisplayName;

	// Token: 0x0400131F RID: 4895
	public TMP_Text factionCustomizeAdjective;

	// Token: 0x04001320 RID: 4896
	public TMP_Text factionCustomizeLeaderAddress;

	// Token: 0x04001321 RID: 4897
	public TMP_Text factionCustomizeFleet;

	// Token: 0x04001322 RID: 4898
	public TMP_InputField customDisplayNameInput;

	// Token: 0x04001323 RID: 4899
	public TMP_InputField customAdjectiveInput;

	// Token: 0x04001324 RID: 4900
	public TMP_InputField customLeaderAddressInput;

	// Token: 0x04001325 RID: 4901
	public TMP_InputField customFleetInput;

	// Token: 0x04001326 RID: 4902
	[Header("New Game Customization")]
	public TMP_Text newGameCustomizationMainHeaderText;

	// Token: 0x04001327 RID: 4903
	public TMP_Text campaignOptionsDifficultyHeaderText;

	// Token: 0x04001328 RID: 4904
	public TMP_Text campaignOptionsFactionHeaderText;

	// Token: 0x04001329 RID: 4905
	public TMP_Text campaignOptionsFactionNamesHeaderText;

	// Token: 0x0400132A RID: 4906
	public Slider researchSpeedMultiplierSlider;

	// Token: 0x0400132B RID: 4907
	public Slider alienProgressionMultiplierSlider;

	// Token: 0x0400132C RID: 4908
	public Slider miningProductivityMultiplierSlider;

	// Token: 0x0400132D RID: 4909
	public Slider controlPointFreebieBonusSlider;

	// Token: 0x0400132E RID: 4910
	public Slider controlPointAIFreebieBonusSlider;

	// Token: 0x0400132F RID: 4911
	public Slider missionControlFreebieBonusSlider;

	// Token: 0x04001330 RID: 4912
	public Slider missionControlAIFreebieBonusSlider;

	// Token: 0x04001331 RID: 4913
	public Slider nationalIPModifierSlider;

	// Token: 0x04001332 RID: 4914
	public Slider averageMonthlyEventsModifierSlider;

	// Token: 0x04001333 RID: 4915
	public Slider miningRatePlayerSlider;

	// Token: 0x04001334 RID: 4916
	public Slider miningRateHumanAISlider;

	// Token: 0x04001335 RID: 4917
	public Slider miningRateAlienSlider;

	// Token: 0x04001336 RID: 4918
	public Slider habConstructionSpeedPlayerSlider;

	// Token: 0x04001337 RID: 4919
	public Slider habConstructionSpeedHumanAISlider;

	// Token: 0x04001338 RID: 4920
	public Slider habConstructionSpeedAlienSlider;

	// Token: 0x04001339 RID: 4921
	public Slider shipConstructionSpeedPlayerSlider;

	// Token: 0x0400133A RID: 4922
	public Slider shipConstructionSpeedHumanAISlider;

	// Token: 0x0400133B RID: 4923
	public Slider shipConstructionSpeedAlienSlider;

	// Token: 0x0400133C RID: 4924
	public TMP_Text researchSpeedTitle;

	// Token: 0x0400133D RID: 4925
	public TMP_Text researchSpeedValue;

	// Token: 0x0400133E RID: 4926
	public TMP_Text alienProgressionRateTitle;

	// Token: 0x0400133F RID: 4927
	public TMP_Text alienProgressionRateValue;

	// Token: 0x04001340 RID: 4928
	public TMP_Text miningProductivityTitle;

	// Token: 0x04001341 RID: 4929
	public TMP_Text miningProductivityValue;

	// Token: 0x04001342 RID: 4930
	public TMP_Text controlPointFreebieTitle;

	// Token: 0x04001343 RID: 4931
	public TMP_Text controlPointFreebieValue;

	// Token: 0x04001344 RID: 4932
	public TMP_Text controlPointAIFreebieTitle;

	// Token: 0x04001345 RID: 4933
	public TMP_Text controlPointAIFreebieValue;

	// Token: 0x04001346 RID: 4934
	public TMP_Text missionControlFreebieTitle;

	// Token: 0x04001347 RID: 4935
	public TMP_Text missionControlFreebieValue;

	// Token: 0x04001348 RID: 4936
	public TMP_Text missionControlAIFreebieTitle;

	// Token: 0x04001349 RID: 4937
	public TMP_Text missionControlAIFreebieValue;

	// Token: 0x0400134A RID: 4938
	public TMP_Text nationalIPModifierTitle;

	// Token: 0x0400134B RID: 4939
	public TMP_Text nationalIPModifierValue;

	// Token: 0x0400134C RID: 4940
	public TMP_Text averageMonthlyEventsModifierTitle;

	// Token: 0x0400134D RID: 4941
	public TMP_Text averageMonthlyEventsModifierValue;

	// Token: 0x0400134E RID: 4942
	public TMP_Text startingCouncilor1ProfessionText;

	// Token: 0x0400134F RID: 4943
	public TMP_Text startingCouncilor2ProfessionText;

	// Token: 0x04001350 RID: 4944
	public TMP_Text variableProjectUnlocksText;

	// Token: 0x04001351 RID: 4945
	public TMP_Text showtriggeredProjectsText;

	// Token: 0x04001352 RID: 4946
	public TMP_Text firstCouncilorHomeNationText;

	// Token: 0x04001353 RID: 4947
	public TMP_Text realismCombatScaleText;

	// Token: 0x04001354 RID: 4948
	public TMP_Text realismCombatDVMovementText;

	// Token: 0x04001355 RID: 4949
	public TMP_Text skirmishRealismCombatScaleText;

	// Token: 0x04001356 RID: 4950
	public TMP_Text skirmishRealismCombatDVMovementText;

	// Token: 0x04001357 RID: 4951
	public TMP_Text AddAlienAssaultFleetText;

	// Token: 0x04001358 RID: 4952
	public TMP_Text otherFactionStartingNationsText;

	// Token: 0x04001359 RID: 4953
	public TMP_Text canDisableFactionsText;

	// Token: 0x0400135A RID: 4954
	public TMP_Text randomizeMapText;

	// Token: 0x0400135B RID: 4955
	public TMP_Text randomizeMapSeedText;

	// Token: 0x0400135C RID: 4956
	public TMP_Text smallShipNameListIdxText;

	// Token: 0x0400135D RID: 4957
	public TMP_Text mediumShipNameListIdxText;

	// Token: 0x0400135E RID: 4958
	public TMP_Text largeShipNameListIdxText;

	// Token: 0x0400135F RID: 4959
	public TMP_Text habNameListIdxText;

	// Token: 0x04001360 RID: 4960
	public TMP_Text customStartingNationGroupText;

	// Token: 0x04001361 RID: 4961
	public TMP_Text miningRatePlayerTitle;

	// Token: 0x04001362 RID: 4962
	public TMP_Text miningRatePlayerValue;

	// Token: 0x04001363 RID: 4963
	public TMP_Text miningRateHumanAITitle;

	// Token: 0x04001364 RID: 4964
	public TMP_Text miningRateHumanAIValue;

	// Token: 0x04001365 RID: 4965
	public TMP_Text miningRateAlienTitle;

	// Token: 0x04001366 RID: 4966
	public TMP_Text miningRateAlienValue;

	// Token: 0x04001367 RID: 4967
	public TMP_Text habConstructionSpeedPlayerTitle;

	// Token: 0x04001368 RID: 4968
	public TMP_Text habConstructionSpeedPlayerValue;

	// Token: 0x04001369 RID: 4969
	public TMP_Text habConstructionSpeedHumanAITitle;

	// Token: 0x0400136A RID: 4970
	public TMP_Text habConstructionSpeedHumanAIValue;

	// Token: 0x0400136B RID: 4971
	public TMP_Text habConstructionSpeedAlienTitle;

	// Token: 0x0400136C RID: 4972
	public TMP_Text habConstructionSpeedAlienValue;

	// Token: 0x0400136D RID: 4973
	public TMP_Text shipConstructionSpeedPlayerTitle;

	// Token: 0x0400136E RID: 4974
	public TMP_Text shipConstructionSpeedPlayerValue;

	// Token: 0x0400136F RID: 4975
	public TMP_Text shipConstructionSpeedHumanAITitle;

	// Token: 0x04001370 RID: 4976
	public TMP_Text shipConstructionSpeedHumanAIValue;

	// Token: 0x04001371 RID: 4977
	public TMP_Text shipConstructionSpeedAlienTitle;

	// Token: 0x04001372 RID: 4978
	public TMP_Text shipConstructionSpeedAlienValue;

	// Token: 0x04001373 RID: 4979
	public TMP_Text factionCustomizationCancelButton;

	// Token: 0x04001374 RID: 4980
	public TMP_Text campaignCustomizationRapidPresetButtonText;

	// Token: 0x04001375 RID: 4981
	public TMP_Text campaignCustomizationLongPresetButtonText;

	// Token: 0x04001376 RID: 4982
	public TMP_Text campaignCustomizationPreviousCampaignText;

	// Token: 0x04001377 RID: 4983
	public TMP_Text startCustomCampaignButtonText;

	// Token: 0x04001378 RID: 4984
	public TMP_Dropdown startingCouncilor1Profession;

	// Token: 0x04001379 RID: 4985
	public TMP_Dropdown startingCouncilor2Profession;

	// Token: 0x0400137A RID: 4986
	public TMP_Dropdown smallShipNameListIdxDropdown;

	// Token: 0x0400137B RID: 4987
	public TMP_Dropdown mediumShipNameListIdxDropdown;

	// Token: 0x0400137C RID: 4988
	public TMP_Dropdown largeShipNameListIdxDropdown;

	// Token: 0x0400137D RID: 4989
	public TMP_Dropdown habNameListIdxDropdown;

	// Token: 0x0400137E RID: 4990
	public TMP_Dropdown customStartingNationGroupDropdown;

	// Token: 0x0400137F RID: 4991
	public Toggle variableProjectUnlocksToggle;

	// Token: 0x04001380 RID: 4992
	public Toggle showtriggeredProjectsToggle;

	// Token: 0x04001381 RID: 4993
	public Toggle firstCouncilorHomeNationToggle;

	// Token: 0x04001382 RID: 4994
	public Toggle realismCombatScaleToggle;

	// Token: 0x04001383 RID: 4995
	public Toggle realismCombatDVMovementToggle;

	// Token: 0x04001384 RID: 4996
	public Toggle skirmishRealismCombatScaleToggle;

	// Token: 0x04001385 RID: 4997
	public Toggle skirmishRealismCombatDVMovementToggle;

	// Token: 0x04001386 RID: 4998
	public Toggle addAlienAssaultFleetToggle;

	// Token: 0x04001387 RID: 4999
	public Toggle otherFactionStartingNations;

	// Token: 0x04001388 RID: 5000
	public Toggle canDisableFactionsToggle;

	// Token: 0x04001389 RID: 5001
	public Toggle randomizeMapToggle;

	// Token: 0x0400138A RID: 5002
	public TMP_InputField randomizeMapSeedInputField;

	// Token: 0x0400138B RID: 5003
	public ListManagerBase factionToggleListManager;

	// Token: 0x0400138C RID: 5004
	public GameObject difficultyWarningObject;

	// Token: 0x0400138D RID: 5005
	public TooltipTrigger difficultyWarningTooltip;

	// Token: 0x0400138E RID: 5006
	public GameObject difficultyWarningObjectOptions;

	// Token: 0x0400138F RID: 5007
	public TMP_Text difficultyWarningOptionsText;

	// Token: 0x04001390 RID: 5008
	[Header("New Game Customization Gameobjects")]
	public GameObject customStartingNationGroupGO;

	// Token: 0x04001391 RID: 5009
	public GameObject otherFactionStartingNationsGO;

	// Token: 0x04001392 RID: 5010
	public GameObject mapSeedInputGO;

	// Token: 0x04001393 RID: 5011
	public GameObject mapRandomizeToggleGO;

	// Token: 0x04001394 RID: 5012
	[Header("New Game Customization Tooltips")]
	public TooltipTrigger difficultyWarningOptionsTooltip;

	// Token: 0x04001395 RID: 5013
	public TooltipTrigger CPFreebieTooltip;

	// Token: 0x04001396 RID: 5014
	public TooltipTrigger AICPFreebieTooltip;

	// Token: 0x04001397 RID: 5015
	public TooltipTrigger MCFreebieTooltip;

	// Token: 0x04001398 RID: 5016
	public TooltipTrigger AIMCFreebieTooltip;

	// Token: 0x04001399 RID: 5017
	public TooltipTrigger researchSpeedTooltip;

	// Token: 0x0400139A RID: 5018
	public TooltipTrigger miningProductivityTooltip;

	// Token: 0x0400139B RID: 5019
	public TooltipTrigger alienProgressionTooltip;

	// Token: 0x0400139C RID: 5020
	public TooltipTrigger variableProjectUnlocksTooltip;

	// Token: 0x0400139D RID: 5021
	public TooltipTrigger showtriggeredProjectsTooltip;

	// Token: 0x0400139E RID: 5022
	public TooltipTrigger firstCouncilorHomeNationTooltip;

	// Token: 0x0400139F RID: 5023
	public TooltipTrigger nationalIPModifierTooltip;

	// Token: 0x040013A0 RID: 5024
	public TooltipTrigger averageMonthlyEventsModifierTooltip;

	// Token: 0x040013A1 RID: 5025
	public TooltipTrigger longCampaignTooltip;

	// Token: 0x040013A2 RID: 5026
	public TooltipTrigger acceleratedCampaignTooltip;

	// Token: 0x040013A3 RID: 5027
	public TooltipTrigger longCampaignSettingsTooltip;

	// Token: 0x040013A4 RID: 5028
	public TooltipTrigger acceleratedCampaignSettingsTooltip;

	// Token: 0x040013A5 RID: 5029
	public TooltipTrigger realismCombatScaleTooltip;

	// Token: 0x040013A6 RID: 5030
	public TooltipTrigger realismCombatDVMovementTooltip;

	// Token: 0x040013A7 RID: 5031
	public TooltipTrigger skirmishRealismCombatScaleTooltip;

	// Token: 0x040013A8 RID: 5032
	public TooltipTrigger skirmishRealismCombatDVMovementTooltip;

	// Token: 0x040013A9 RID: 5033
	public TooltipTrigger addAlienAssaultFleetTooltip;

	// Token: 0x040013AA RID: 5034
	public TooltipTrigger miningRatePlayerTooltip;

	// Token: 0x040013AB RID: 5035
	public TooltipTrigger miningRateHumanAITooltip;

	// Token: 0x040013AC RID: 5036
	public TooltipTrigger miningRateAlienTooltip;

	// Token: 0x040013AD RID: 5037
	public TooltipTrigger habConstructionSpeedPlayerTooltip;

	// Token: 0x040013AE RID: 5038
	public TooltipTrigger habConstructionSpeedHumanAITooltip;

	// Token: 0x040013AF RID: 5039
	public TooltipTrigger habConstructionSpeedAlienTooltip;

	// Token: 0x040013B0 RID: 5040
	public TooltipTrigger shipConstructionSpeedPlayerTooltip;

	// Token: 0x040013B1 RID: 5041
	public TooltipTrigger shipConstructionSpeedHumanAITooltip;

	// Token: 0x040013B2 RID: 5042
	public TooltipTrigger shipConstructionSpeedAlienTooltip;

	// Token: 0x040013B3 RID: 5043
	public TooltipTrigger nationGroupTooltip;

	// Token: 0x040013B4 RID: 5044
	public TooltipTrigger otherFactionStartingNationGroupTooltip;

	// Token: 0x040013B5 RID: 5045
	public TooltipTrigger canDisableFactionsTooltip;

	// Token: 0x040013B6 RID: 5046
	public TooltipTrigger randomizeMapTooltip;

	// Token: 0x040013B7 RID: 5047
	private bool customDifficulty;

	// Token: 0x040013B8 RID: 5048
	private SkirmishModeScenario skirmishScenario;

	// Token: 0x040013B9 RID: 5049
	[Header("Skirmish Mode UI")]
	public TMP_Text skirmishModeHeaderText;

	// Token: 0x040013BA RID: 5050
	public TMP_Text skirmishModePlayer1HeaderText;

	// Token: 0x040013BB RID: 5051
	public TMP_Text skirmishModePlayer2HeaderText;

	// Token: 0x040013BC RID: 5052
	public TMP_Text skirmishModeLocationTitle;

	// Token: 0x040013BD RID: 5053
	public TMP_Text skirmishModeHabTitle;

	// Token: 0x040013BE RID: 5054
	public TMP_Text skirmishModeBeginText;

	// Token: 0x040013BF RID: 5055
	public TMP_Text skirmishModePlayer1AddShipsText;

	// Token: 0x040013C0 RID: 5056
	public TMP_Text skirmishModePlayer2AddShipsText;

	// Token: 0x040013C1 RID: 5057
	public TMP_Text skirmishModePlayer1CloseAddShipsText;

	// Token: 0x040013C2 RID: 5058
	public TMP_Text skirmishModePlayer2CloseAddShipsText;

	// Token: 0x040013C3 RID: 5059
	public TMP_Text skirmishModePlayer1FleetScore;

	// Token: 0x040013C4 RID: 5060
	public TMP_Text skirmishModePlayer2FleetScore;

	// Token: 0x040013C5 RID: 5061
	public List<Image> factionFleetGradient = new List<Image>();

	// Token: 0x040013C6 RID: 5062
	public List<Image> factionFleetCenterGradient = new List<Image>();

	// Token: 0x040013C7 RID: 5063
	public List<Image> factionFleetIcon = new List<Image>();

	// Token: 0x040013C8 RID: 5064
	public List<Image> factionFleetBackgroundIcon = new List<Image>();

	// Token: 0x040013C9 RID: 5065
	public TMP_Text habFactionText;

	// Token: 0x040013CA RID: 5066
	private IScenario selectedScenario;

	// Token: 0x040013CB RID: 5067
	private TIMetaTemplate selectedMetaTemplateScenario;

	// Token: 0x040013CC RID: 5068
	public Menu creditsMenu;

	// Token: 0x040013CD RID: 5069
	[Header("Hardware Warning UI")]
	public GameObject hardwareWarningObject;

	// Token: 0x040013CE RID: 5070
	public TMP_Text hardwareWarningConfirmText;

	// Token: 0x040013CF RID: 5071
	public TMP_Text hardwareWarningTitleText;

	// Token: 0x040013D0 RID: 5072
	public TMP_Text hardwareWarningDescriptionText;

	// Token: 0x040013D1 RID: 5073
	[Header("Translation Warning UI")]
	public GameObject translationWarningObject;

	// Token: 0x040013D2 RID: 5074
	public TMP_Text translationWarningDescription;

	// Token: 0x040013D3 RID: 5075
	[Header("Microsoft Gamepass Warning UI")]
	public GameObject gamepassWarningObject;

	// Token: 0x040013D4 RID: 5076
	public TMP_Text gamepassWarningDescription;

	// Token: 0x040013D5 RID: 5077
	[Header("Mod Warning Dialog")]
	public GameObject modLoaderWarningDialog;

	// Token: 0x040013D6 RID: 5078
	public GameObject fatalErrorBG;

	// Token: 0x040013D7 RID: 5079
	public TMP_Text modLoaderWarningConfirmText;

	// Token: 0x040013D8 RID: 5080
	public TMP_Text modLoaderWarningHeaderText;

	// Token: 0x040013D9 RID: 5081
	public TMP_Text modLoaderWarningDescriptionText;

	// Token: 0x040013DA RID: 5082
	private bool bankedModFailure;

	// Token: 0x040013DB RID: 5083
	public bool fatalStartupError;

	// Token: 0x040013DC RID: 5084
	private string bankedModWarningHeaderLoc;

	// Token: 0x040013DD RID: 5085
	private string bankedModWarningDescLoc;

	// Token: 0x040013DE RID: 5086
	private string bankedModWarningLocArg1;

	// Token: 0x040013DF RID: 5087
	private string bankedModWarningLocArg2;

	// Token: 0x040013E0 RID: 5088
	[Header("Misc Links")]
	public TMP_Text discordLinkText;

	// Token: 0x040013E1 RID: 5089
	public TMP_Text wikiLinkText;

	// Token: 0x040013E2 RID: 5090
	private int startingCouncilorProfessionIndex = 2;

	// Token: 0x040013E3 RID: 5091
	private const float sliderIncrementSmall = 0.05f;

	// Token: 0x040013E4 RID: 5092
	private static bool forceCredits;

	// Token: 0x040013E5 RID: 5093
	public static bool CinematicScalingMode = false;

	// Token: 0x040013E6 RID: 5094
	private int defaultMiningProductivity;

	// Token: 0x040013E7 RID: 5095
	private List<string> defaultFactionsInScenario;

	// Token: 0x040013E8 RID: 5096
	private List<string> defaultCompletedProjectsInScenario;

	// Token: 0x040013E9 RID: 5097
	private int lastSelectedDifficulty;

	// Token: 0x040013EA RID: 5098
	private int lastSelectedFaction = -1;

	// Token: 0x040013EB RID: 5099
	private Dictionary<string, string> nameListsToAdd = new Dictionary<string, string>();

	// Token: 0x040013EC RID: 5100
	public TMP_Dropdown skirmishLocationSettingDropdown;

	// Token: 0x040013ED RID: 5101
	private Dictionary<string, TIOrbitTemplate> locationDictionary = new Dictionary<string, TIOrbitTemplate>();

	// Token: 0x040013EE RID: 5102
	public TMP_Dropdown[] skirmishFactionDropdown = new TMP_Dropdown[2];

	// Token: 0x040013EF RID: 5103
	private Dictionary<string, TIFactionTemplate> factionDictionary = new Dictionary<string, TIFactionTemplate>();

	// Token: 0x040013F0 RID: 5104
	public ListManagerBase[] skirmishShipLists;

	// Token: 0x040013F1 RID: 5105
	public List<SkirmishShipListItemController> skirmishShipListDropdowns;

	// Token: 0x040013F2 RID: 5106
	public List<TISpaceShipTemplate> ships = new List<TISpaceShipTemplate>();

	// Token: 0x040013F3 RID: 5107
	public Dictionary<string, TISpaceShipTemplate> shipDictionary = new Dictionary<string, TISpaceShipTemplate>();

	// Token: 0x040013F4 RID: 5108
	public TMP_Dropdown skirmishHabDropdown;

	// Token: 0x040013F5 RID: 5109
	private Dictionary<string, TIHabTemplate> habDictionary = new Dictionary<string, TIHabTemplate>();

	// Token: 0x040013F6 RID: 5110
	private static List<TISpaceShipTemplate> importedShipTemplates = new List<TISpaceShipTemplate>();

	// Token: 0x02000C15 RID: 3093
	public struct CategoryWithPriority
	{
		// Token: 0x04004D20 RID: 19744
		public string category;

		// Token: 0x04004D21 RID: 19745
		public float priority;
	}
}
