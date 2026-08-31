using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000880 RID: 2176
	public class IntelProjectsListItemController : MonoBehaviour
	{
		// Token: 0x06005142 RID: 20802 RVA: 0x00238760 File Offset: 0x00236960
		public void SetListItem(TIProjectTemplate project, bool inProgress, bool currentWork, int weight, float accumulatedResearch = 0f, float researchCost = 1f, List<TIProjectTemplate> stealableProjects = null, List<TIProjectTemplate> sabotageProjects = null)
		{
			string text = project.displayName;
			if (inProgress)
			{
				StringBuilder stringBuilder = new StringBuilder(text).Append(" ").Append(Loc.T("UI.Science.Panel.OverallProgress", new object[]
				{
					accumulatedResearch.ToString("N0"),
					researchCost.ToString("N0"),
					TemplateManager.global.researchInlineSpritePath
				}));
				if (currentWork)
				{
					stringBuilder.Append(TemplateManager.global.projectsInlineSpritePath);
					this.weightSprite.sprite = NationInfoController.weightSprite[weight];
					this.weightSprite.enabled = true;
				}
				else
				{
					this.weightSprite.enabled = false;
				}
				GameControl.assetLoader.LoadAssetForImageAssignment(TIFactionState.sabotageProjectMission.missionIconImagePath_Off, this.stealable);
				this.stealable.enabled = sabotageProjects != null && sabotageProjects.Contains(project);
				text = stringBuilder.ToString();
			}
			else
			{
				this.weightSprite.enabled = false;
				GameControl.assetLoader.LoadAssetForImageAssignment(TIFactionState.stealProjectMission.missionIconImagePath_Off, this.stealable);
				this.stealable.enabled = stealableProjects != null && stealableProjects.Contains(project);
			}
			this.projectName.SetText(text);
		}

		// Token: 0x04003558 RID: 13656
		public TMP_Text projectName;

		// Token: 0x04003559 RID: 13657
		public Image weightSprite;

		// Token: 0x0400355A RID: 13658
		public Image stealable;
	}
}
