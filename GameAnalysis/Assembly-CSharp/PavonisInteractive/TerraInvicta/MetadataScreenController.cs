using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000806 RID: 2054
	public class MetadataScreenController : MonoBehaviour
	{
		// Token: 0x06004A69 RID: 19049 RVA: 0x001F3340 File Offset: 0x001F1540
		public void RefreshUIWithMetaData(TIMetadataState metaData, string saveName)
		{
			if (metaData == null)
			{
				this.ClearUI();
				this.saveNameText.SetText(saveName);
				return;
			}
			this.cachedMetaData = metaData;
			this.cachedSaveName = saveName;
			this.metaDataLayoutObject.SetActive(true);
			this.saveNameText.SetText(saveName);
			this.factionIcon.enabled = true;
			this.objectiveImage.enabled = true;
			this.factionGradient.enabled = true;
			if (string.IsNullOrEmpty(metaData.lastCompletedObjectiveArtPath))
			{
				metaData.lastCompletedObjectiveArtPath = metaData.playerFactionGradientPath;
			}
			TIUtilities.assetLoader.LoadAssetForImageAssignment(metaData.lastCompletedObjectiveArtPath, this.objectiveImage);
			TIUtilities.assetLoader.LoadAssetForImageAssignment(metaData.playerFactionIconPath, this.factionIcon);
			TIUtilities.assetLoader.LoadAssetForImageAssignment(metaData.playerFactionGradientPath, this.factionGradient);
			this.modsActiveObject.SetActive(metaData.playedWithMods);
			this.DLCActiveObject.SetActive(metaData.requiredDLC != null && metaData.requiredDLC.Count > 0);
			this.playerFactionNameText.SetText(metaData.playerFactionName);
			this.saveDateText.SetText(metaData.gameTimeString);
			this.lastObjectiveNameText.SetText(metaData.lastCompletedObjectiveName);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Options.DifficultyLabel")).Append(" ").Append(metaData.difficulty);
			if (metaData.customDifficulty)
			{
				stringBuilder.Append(Loc.T("UI.Options.DifficultyCustom"));
			}
			string text = stringBuilder.ToString();
			this.difficultyText.SetText(text);
			this.researchSpeedText.SetText(metaData.researchSpeedMultiplier);
			this.miningProductivityText.SetText(metaData.miningProductivityMultiplier);
			this.alienProgressionText.SetText(metaData.alienProgressionSpeed);
			this.nationalIPMultiplierText.SetText(metaData.nationalIPMultiplier);
			this.mcBonusText.SetText(metaData.missionControlBonus);
			this.mcBonusAIText.SetText(metaData.missionControlBonusAI);
			this.cpBonusText.SetText(metaData.controlPointMaintenanceFreebieBonus);
			this.cpBonusAIText.SetText(metaData.controlPointMaintenanceFreebieBonusAI);
			this.monthlyEventText.SetText(metaData.averageMonthlyEvents);
		}

		// Token: 0x06004A6A RID: 19050 RVA: 0x001F3568 File Offset: 0x001F1768
		public void ClearUI()
		{
			this.metaDataLayoutObject.SetActive(false);
			this.researchSpeedText.SetText("");
			this.miningProductivityText.SetText("");
			this.alienProgressionText.SetText("");
			this.nationalIPMultiplierText.SetText("");
			this.mcBonusText.SetText("");
			this.mcBonusAIText.SetText("");
			this.cpBonusText.SetText("");
			this.cpBonusAIText.SetText("");
			this.monthlyEventText.SetText("");
			this.modsActiveObject.SetActive(false);
			this.DLCActiveObject.SetActive(false);
			this.saveNameText.SetText("");
			this.playerFactionNameText.SetText("");
			this.saveDateText.SetText("");
			this.lastObjectiveNameText.SetText("");
			this.factionIcon.enabled = false;
			this.objectiveImage.enabled = false;
			this.factionGradient.enabled = false;
		}

		// Token: 0x06004A6B RID: 19051 RVA: 0x001F368D File Offset: 0x001F188D
		private void Start()
		{
			Loc.OnLanguageChangedEvent += this.OnLanguageChangedEvent;
			this.LoadLocalizedText();
			this.ClearUI();
		}

		// Token: 0x06004A6C RID: 19052 RVA: 0x001F36AC File Offset: 0x001F18AC
		private void LoadLocalizedText()
		{
			this.ModsActiveText.SetText(Loc.T("UI.StartScreen.Mods.ModsInSave"));
			this.researchSpeedTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.ResearchSpeed"));
			this.miningProductivityTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MiningProductivity"));
			this.alienProgressionTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.AlienProgressionRate"));
			this.nationalIPMultiplierTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.NationalIPModifier"));
			this.mcBonusTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MCFreebies"));
			this.mcBonusAITitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.MCFreebiesAI"));
			this.cpBonusTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CPFreebies"));
			this.cpBonusAITitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.CPFreebiesAI"));
			this.monthlyEventTitleText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.AverageMonthlyEvents"));
			this.campaignSettingsText.SetText(Loc.T("UI.StartScreen.CustomizeCampaign.Options"));
		}

		// Token: 0x06004A6D RID: 19053 RVA: 0x001F37A0 File Offset: 0x001F19A0
		private void OnLanguageChangedEvent()
		{
			this.LoadLocalizedText();
			Loc.SwapFonts(base.gameObject);
			this.RefreshUIWithMetaData(this.cachedMetaData, this.cachedSaveName);
		}

		// Token: 0x06004A6E RID: 19054 RVA: 0x001F37C5 File Offset: 0x001F19C5
		private void OnDestroy()
		{
			Loc.OnLanguageChangedEvent -= this.OnLanguageChangedEvent;
		}

		// Token: 0x04002B4B RID: 11083
		public GameObject metaDataLayoutObject;

		// Token: 0x04002B4C RID: 11084
		public Image objectiveImage;

		// Token: 0x04002B4D RID: 11085
		public Image factionIcon;

		// Token: 0x04002B4E RID: 11086
		public Image factionGradient;

		// Token: 0x04002B4F RID: 11087
		public GameObject modsActiveObject;

		// Token: 0x04002B50 RID: 11088
		public GameObject DLCActiveObject;

		// Token: 0x04002B51 RID: 11089
		public TMP_Text ModsActiveText;

		// Token: 0x04002B52 RID: 11090
		public TMP_Text campaignSettingsText;

		// Token: 0x04002B53 RID: 11091
		public TMP_Text saveNameText;

		// Token: 0x04002B54 RID: 11092
		public TMP_Text saveDateText;

		// Token: 0x04002B55 RID: 11093
		public TMP_Text playerFactionNameText;

		// Token: 0x04002B56 RID: 11094
		public TMP_Text lastObjectiveNameText;

		// Token: 0x04002B57 RID: 11095
		public TMP_Text difficultyText;

		// Token: 0x04002B58 RID: 11096
		public TMP_Text researchSpeedTitleText;

		// Token: 0x04002B59 RID: 11097
		public TMP_Text miningProductivityTitleText;

		// Token: 0x04002B5A RID: 11098
		public TMP_Text alienProgressionTitleText;

		// Token: 0x04002B5B RID: 11099
		public TMP_Text nationalIPMultiplierTitleText;

		// Token: 0x04002B5C RID: 11100
		public TMP_Text mcBonusTitleText;

		// Token: 0x04002B5D RID: 11101
		public TMP_Text mcBonusAITitleText;

		// Token: 0x04002B5E RID: 11102
		public TMP_Text cpBonusTitleText;

		// Token: 0x04002B5F RID: 11103
		public TMP_Text cpBonusAITitleText;

		// Token: 0x04002B60 RID: 11104
		public TMP_Text monthlyEventTitleText;

		// Token: 0x04002B61 RID: 11105
		public TMP_Text researchSpeedText;

		// Token: 0x04002B62 RID: 11106
		public TMP_Text miningProductivityText;

		// Token: 0x04002B63 RID: 11107
		public TMP_Text alienProgressionText;

		// Token: 0x04002B64 RID: 11108
		public TMP_Text nationalIPMultiplierText;

		// Token: 0x04002B65 RID: 11109
		public TMP_Text mcBonusText;

		// Token: 0x04002B66 RID: 11110
		public TMP_Text mcBonusAIText;

		// Token: 0x04002B67 RID: 11111
		public TMP_Text cpBonusText;

		// Token: 0x04002B68 RID: 11112
		public TMP_Text cpBonusAIText;

		// Token: 0x04002B69 RID: 11113
		public TMP_Text monthlyEventText;

		// Token: 0x04002B6A RID: 11114
		private TIMetadataState cachedMetaData;

		// Token: 0x04002B6B RID: 11115
		private string cachedSaveName;
	}
}
