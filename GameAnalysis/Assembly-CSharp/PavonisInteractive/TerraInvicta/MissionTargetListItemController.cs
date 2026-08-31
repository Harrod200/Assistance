using System;
using System.Collections.Generic;
using ModelShark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008A4 RID: 2212
	public class MissionTargetListItemController : MonoBehaviour
	{
		// Token: 0x0600537E RID: 21374 RVA: 0x002563F0 File Offset: 0x002545F0
		public void Init(NotificationScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x0600537F RID: 21375 RVA: 0x002563FC File Offset: 0x002545FC
		public void SetListItem(TIOrgState org)
		{
			this.targetIcon.sprite = org.icon;
			this.targetText.SetText(org.displayName);
			if (org.requiresNationInterest)
			{
				this.orgFlag.sprite = org.requiredNationInterest.flag;
				this.orgFlag.enabled = true;
			}
			else
			{
				this.orgFlag.enabled = false;
			}
			this.orgTier.text = org.tierStarsInline;
			this.targetTooltip.SetText("BodyText", org.description(true, GameControl.control.activePlayer, false, false));
			this.org = org;
			this.orgListItem = true;
			if (org.requiresNationInterest)
			{
				this.orgNationFlag.sprite = org.requiredNationInterest.flag;
				this.orgNationFlag.enabled = true;
			}
			else
			{
				this.orgNationFlag.enabled = false;
			}
			this.summaryDescription.SetText(org.QuickDescription(false));
			if (org.hasCouncilor)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(org.assignedCouncilor.iconResource, this.assignedCouncilorIcon);
			}
			else
			{
				this.assignedCouncilorIcon.sprite = org.factionOrbit.factionIcon64;
			}
			int num = 0;
			foreach (TIMissionTemplate timissionTemplate in org.missionsGranted)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(timissionTemplate.missionIconImagePath_Off, this.missionImages[num++]);
			}
			for (int i = 0; i < 8; i++)
			{
				this.missionImages[i].enabled = i < num;
			}
			this.orgDetailObject.SetActive(true);
		}

		// Token: 0x06005380 RID: 21376 RVA: 0x002565B8 File Offset: 0x002547B8
		public void SetListItem(TIProjectTemplate project)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(project.IconResource, this.targetIcon);
			this.targetText.SetText(project.displayName);
			this.targetTooltip.SetText("BodyText", project.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.Prospective, null, false));
			this.orgFlag.enabled = false;
			this.orgTier.text = "";
			this.project = project;
			this.orgListItem = false;
			this.orgDetailObject.SetActive(false);
		}

		// Token: 0x06005381 RID: 21377 RVA: 0x00256648 File Offset: 0x00254848
		public void SetListItem(TIFactionState faction, ProjectProgress projectProgress)
		{
			GameControl.assetLoader.LoadAssetForImageAssignment(projectProgress.projectTemplate.IconResource, this.targetIcon);
			this.targetTooltip.SetText("BodyText", projectProgress.projectTemplate.GetFullDescription(GameControl.control.activePlayer, TechBenefitsContext.Prospective, null, false));
			this.project = projectProgress.projectTemplate;
			float accumulatedResearch = projectProgress.accumulatedResearch;
			float researchCost = this.project.GetResearchCost(faction);
			this.targetText.SetText(Loc.T("UI.Science.SelectProjectListText", new object[]
			{
				this.project.displayName,
				accumulatedResearch.ToString("N0"),
				researchCost.ToString("N0"),
				TemplateManager.global.researchInlineSpritePath
			}));
			this.orgFlag.enabled = false;
			this.orgTier.text = "";
			this.orgListItem = false;
			this.orgDetailObject.SetActive(false);
		}

		// Token: 0x06005382 RID: 21378 RVA: 0x0025673B File Offset: 0x0025493B
		public void OnClicked()
		{
			if (this.orgListItem)
			{
				this.controller.MissionTargetSelected(this.org);
				return;
			}
			this.controller.MissionTargetSelected(this.project);
		}

		// Token: 0x04003949 RID: 14665
		public Image targetIcon;

		// Token: 0x0400394A RID: 14666
		public Image orgFlag;

		// Token: 0x0400394B RID: 14667
		public TMP_Text orgTier;

		// Token: 0x0400394C RID: 14668
		public TooltipTrigger targetTooltip;

		// Token: 0x0400394D RID: 14669
		public TMP_Text targetText;

		// Token: 0x0400394E RID: 14670
		private NotificationScreenController controller;

		// Token: 0x0400394F RID: 14671
		private TIProjectTemplate project;

		// Token: 0x04003950 RID: 14672
		private TIOrgState org;

		// Token: 0x04003951 RID: 14673
		private bool orgListItem;

		// Token: 0x04003952 RID: 14674
		public GameObject orgDetailObject;

		// Token: 0x04003953 RID: 14675
		public Image orgNationFlag;

		// Token: 0x04003954 RID: 14676
		public Image assignedCouncilorIcon;

		// Token: 0x04003955 RID: 14677
		public TMP_Text summaryDescription;

		// Token: 0x04003956 RID: 14678
		public List<Image> missionImages;
	}
}
