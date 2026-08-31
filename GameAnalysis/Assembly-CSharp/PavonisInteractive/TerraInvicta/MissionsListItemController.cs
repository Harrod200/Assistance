using System;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200083A RID: 2106
	public class MissionsListItemController : MonoBehaviour
	{
		// Token: 0x06004C44 RID: 19524 RVA: 0x00203110 File Offset: 0x00201310
		public void SetListItem(TIMissionTemplate missionTemplate, TICouncilorState councilor, int totalCouncilorsWithMission = -1, bool automateMode = false, bool disableLineSeparator = false)
		{
			this.councilorState = councilor;
			this.missionTemplate = missionTemplate;
			StringBuilder stringBuilder = new StringBuilder(missionTemplate.displayName);
			if (missionTemplate.primaryResource != FactionResource.None)
			{
				stringBuilder.Append(TIUtilities.InlineResourceStr(missionTemplate.primaryResource));
			}
			if (totalCouncilorsWithMission >= 0)
			{
				this.councilorsWithMissionCount.text = totalCouncilorsWithMission.ToString();
				this.councilorsWithMissionCount.gameObject.SetActive(true);
			}
			this.missionName.text = stringBuilder.ToString();
			GameControl.assetLoader.LoadAssetForImageAssignment(missionTemplate.missionIconImagePath_Off, this.missionIcon);
			this.missionDescription.SetText("BodyText", missionTemplate.descriptionWithTiming);
			if (councilor != null)
			{
				TIOrgState tiorgState = councilor.OrgGrantingMission(missionTemplate, true);
				if (tiorgState != null)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(tiorgState.orgIconPath, this.sourceIcon);
					this.sourceIcon.enabled = true;
				}
				else
				{
					this.sourceIcon.enabled = false;
				}
			}
			else
			{
				this.sourceIcon.enabled = false;
			}
			if (automateMode)
			{
				Toggle toggle = this.automateSettingsToggle;
				if (toggle != null)
				{
					toggle.gameObject.SetActive(missionTemplate.allowedForAutoDefense);
				}
				Toggle toggle2 = this.automateSettingsToggle;
				if (toggle2 != null)
				{
					toggle2.SetIsOnWithoutNotify(!councilor.missionsExcludedFromDefenseMode.Contains(missionTemplate.dataName));
				}
			}
			else
			{
				Toggle toggle3 = this.automateSettingsToggle;
				if (toggle3 != null)
				{
					toggle3.gameObject.SetActive(false);
				}
			}
			this.lineSeparator.enabled = !disableLineSeparator;
			this.headerBackgroundObject.enabled = false;
			this.councilorsWithMissionIcon.gameObject.SetActive(false);
			this.missionDescription.enabled = true;
			this.councilorsWithMissionToolTip.enabled = false;
		}

		// Token: 0x06004C45 RID: 19525 RVA: 0x002032B4 File Offset: 0x002014B4
		public void SetListItem(CouncilorAttribute attribute, TICouncilorState councilor, TIMissionTemplate missionTemplate = null, bool isHeader = false, bool isFirstHeader = false)
		{
			Toggle toggle = this.automateSettingsToggle;
			if (toggle != null)
			{
				toggle.gameObject.SetActive(false);
			}
			string text = GameControl.control.activePlayer.template.genericCouncilorIcon;
			string text2 = GameControl.control.activePlayer.factionIcon64path;
			if (councilor.faction != null)
			{
				text = councilor.faction.template.genericCouncilorIcon;
				text2 = councilor.faction.factionIcon64path;
			}
			this.councilorsWithMissionIcon.gameObject.SetActive(isFirstHeader);
			this.councilorsWithMissionToolTip.enabled = isFirstHeader;
			if (isFirstHeader)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(text, this.councilorsWithMissionIcon);
				this.councilorsWithMissionToolTip.SetText("BodyText", Loc.T("UI.Councilor.TotalCouncilorsWithMission"));
			}
			if (attribute == CouncilorAttribute.None)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (missionTemplate != null && missionTemplate.baseMission)
				{
					stringBuilder.Append(Loc.T("UI.Global.Common"));
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.Councilor.Uncontested"));
				}
				this.missionName.text = stringBuilder.ToString();
				GameControl.assetLoader.LoadAssetForImageAssignment(text2, this.missionIcon);
				this.missionDescription.enabled = false;
			}
			else
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(Loc.T(new StringBuilder("UI.Global.").Append(attribute.ToString()).ToString()));
				this.missionName.text = stringBuilder2.ToString();
				GameControl.assetLoader.LoadAssetForImageAssignment(TIUtilities.PathAttributeIcon(attribute), this.missionIcon);
				this.missionDescription.SetText("BodyText", Loc.T(new StringBuilder("UI.Councilor.").Append(attribute.ToString()).Append("Tip").ToString()));
				this.missionDescription.enabled = true;
			}
			this.lineSeparator.enabled = !isFirstHeader && !isHeader;
			this.headerBackgroundObject.enabled = isFirstHeader || isHeader;
			this.councilorsWithMissionCount.gameObject.SetActive(false);
			this.sourceIcon.enabled = false;
		}

		// Token: 0x06004C46 RID: 19526 RVA: 0x002034C8 File Offset: 0x002016C8
		public void SetSimpleListItem(TIMissionTemplate missionTemplate, TICouncilorState councilor)
		{
			StringBuilder stringBuilder = new StringBuilder(missionTemplate.displayName);
			if (missionTemplate.primaryAttackerStat != CouncilorAttribute.None)
			{
				stringBuilder.Append(TIUtilities.InlineAttributeStr(missionTemplate.primaryAttackerStat));
			}
			if (missionTemplate.primaryResource != FactionResource.None)
			{
				stringBuilder.Append(TIUtilities.InlineResourceStr(missionTemplate.primaryResource));
			}
			this.missionName.text = stringBuilder.ToString();
			GameControl.assetLoader.LoadAssetForImageAssignment(missionTemplate.missionIconImagePath_Off, this.missionIcon);
			this.missionDescription.SetText("BodyText", missionTemplate.descriptionWithTiming);
			if (!(councilor != null))
			{
				this.sourceIcon.enabled = false;
				return;
			}
			TIOrgState tiorgState = councilor.OrgGrantingMission(missionTemplate, true);
			if (tiorgState != null)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(tiorgState.orgIconPath, this.sourceIcon);
				this.sourceIcon.enabled = true;
				return;
			}
			this.sourceIcon.enabled = false;
		}

		// Token: 0x06004C47 RID: 19527 RVA: 0x002035A9 File Offset: 0x002017A9
		public void OnToggleAutomateMode()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.councilorState.ToggleDefenseModeMission(this.missionTemplate, this.automateSettingsToggle.isOn);
		}

		// Token: 0x04002E15 RID: 11797
		public Image missionIcon;

		// Token: 0x04002E16 RID: 11798
		public TMP_Text missionName;

		// Token: 0x04002E17 RID: 11799
		public TooltipTrigger missionDescription;

		// Token: 0x04002E18 RID: 11800
		public Image sourceIcon;

		// Token: 0x04002E19 RID: 11801
		public TMP_Text councilorsWithMissionCount;

		// Token: 0x04002E1A RID: 11802
		public Image councilorsWithMissionIcon;

		// Token: 0x04002E1B RID: 11803
		public Image lineSeparator;

		// Token: 0x04002E1C RID: 11804
		public Image headerBackgroundObject;

		// Token: 0x04002E1D RID: 11805
		public Toggle automateSettingsToggle;

		// Token: 0x04002E1E RID: 11806
		public TooltipTrigger councilorsWithMissionToolTip;

		// Token: 0x04002E1F RID: 11807
		private TICouncilorState councilorState;

		// Token: 0x04002E20 RID: 11808
		private TIMissionTemplate missionTemplate;

		// Token: 0x04002E21 RID: 11809
		public bool inAutomateMode;
	}
}
