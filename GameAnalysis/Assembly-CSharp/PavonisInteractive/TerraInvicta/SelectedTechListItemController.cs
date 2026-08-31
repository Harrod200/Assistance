using System;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C7 RID: 2247
	public class SelectedTechListItemController : MonoBehaviour
	{
		// Token: 0x06005649 RID: 22089 RVA: 0x00277858 File Offset: 0x00275A58
		public void UpdateData(TIGenericTechTemplate tech, ResearchScreenController controller, ChildTechGridItemController techTreeObjectLink, bool prereqList, TIGenericTechTemplate[] altPrereq0 = null, TIGenericTechTemplate[] altPrereq1 = null, int altPrereqIndex = -1)
		{
			TIFactionState activePlayer = GameControl.control.activePlayer;
			this.controller = controller;
			this.techTreeLink = techTreeObjectLink;
			if (tech.ShouldHide(activePlayer))
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(true);
			this.techNameText.SetText(tech.displayName);
			this.techTooltip.SetDelegate("BodyText", () => activePlayer.GetCachedTechTooltipString(tech));
			float num = 1f;
			if (tech.ref_project != null)
			{
				num = activePlayer.GetProjectUnlockChance(tech.ref_project, activePlayer.TechContributionBonus(tech.ref_project)) / 100f;
				this.projectIcon.SetActive(true);
			}
			else
			{
				this.projectIcon.SetActive(false);
			}
			this.techUnlockChanceText.SetText(num.ToPercent("P0"));
			int techStatusAppearanceIndex = ResearchScreenController.GetTechStatusAppearanceIndex(tech, activePlayer);
			this.lockIcon.color = controller.techStatusIconColors[techStatusAppearanceIndex];
			this.xIcon.color = controller.techStatusIconColors[techStatusAppearanceIndex];
			this.checkIcon.color = controller.techStatusIconColors[techStatusAppearanceIndex];
			this.leftCheckIcon.color = controller.techStatusIconColors[0];
			this.leftXIcon.color = controller.techStatusIconColors[7];
			this.lockIcon.gameObject.SetActive(false);
			this.techUnlockChanceText.gameObject.SetActive(false);
			bool flag = false;
			if (prereqList)
			{
				if (tech == ((altPrereq0 != null) ? altPrereq0[1] : null) || tech == ((altPrereq1 != null) ? altPrereq1[1] : null))
				{
					this.leftCompletionObject.SetActive(false);
					if (tech == altPrereq0[1])
					{
						base.transform.SetSiblingIndex(1);
					}
					if (tech == altPrereq1[1] && altPrereqIndex > 0)
					{
						base.transform.SetSiblingIndex(3);
					}
				}
				else if ((tech == ((altPrereq0 != null) ? altPrereq0[0] : null) && ((altPrereq0 != null) ? altPrereq0[1] : null) != null) || (tech == ((altPrereq1 != null) ? altPrereq1[0] : null) && ((altPrereq1 != null) ? altPrereq1[1] : null) != null))
				{
					this.leftCompletionObject.SetActive(true);
					this.leftCompletionRT.sizeDelta = new Vector2(this.leftCompletionRT.sizeDelta.x, 76f);
					TIProjectTemplate tiprojectTemplate = altPrereq0[0] as TIProjectTemplate;
					if (tiprojectTemplate == null || !activePlayer.completedProjects.Contains(tiprojectTemplate))
					{
						TITechTemplate titechTemplate = altPrereq0[0] as TITechTemplate;
						if (titechTemplate == null || !TIGlobalResearchState.FinishedTechs().Contains(titechTemplate))
						{
							TIProjectTemplate tiprojectTemplate2 = altPrereq0[1] as TIProjectTemplate;
							if (tiprojectTemplate2 == null || !activePlayer.completedProjects.Contains(tiprojectTemplate2))
							{
								TITechTemplate titechTemplate2 = altPrereq0[1] as TITechTemplate;
								if (titechTemplate2 == null || !TIGlobalResearchState.FinishedTechs().Contains(titechTemplate2))
								{
									goto IL_0333;
								}
							}
						}
					}
					flag = true;
					IL_0333:
					if (altPrereq1.None<TIGenericTechTemplate>((TIGenericTechTemplate x) => x == null))
					{
						TIProjectTemplate tiprojectTemplate3 = altPrereq1[0] as TIProjectTemplate;
						if (tiprojectTemplate3 != null && activePlayer.completedProjects.Contains(tiprojectTemplate3))
						{
							goto IL_03DB;
						}
					}
					TITechTemplate titechTemplate3 = altPrereq1[0] as TITechTemplate;
					if (titechTemplate3 == null || !TIGlobalResearchState.FinishedTechs().Contains(titechTemplate3))
					{
						TIProjectTemplate tiprojectTemplate4 = altPrereq1[1] as TIProjectTemplate;
						if (tiprojectTemplate4 == null || !activePlayer.completedProjects.Contains(tiprojectTemplate4))
						{
							TITechTemplate titechTemplate4 = altPrereq1[1] as TITechTemplate;
							if (titechTemplate4 == null || !TIGlobalResearchState.FinishedTechs().Contains(titechTemplate4))
							{
								goto IL_041E;
							}
						}
					}
					IL_03DB:
					flag = true;
				}
				else
				{
					this.leftCompletionObject.SetActive(true);
					this.leftCompletionRT.sizeDelta = new Vector2(this.leftCompletionRT.sizeDelta.x, 38f);
				}
			}
			else
			{
				this.leftCompletionObject.SetActive(false);
			}
			IL_041E:
			if (techStatusAppearanceIndex == 1)
			{
				this.techUnlockChanceText.gameObject.SetActive(true);
				TIProjectTemplate tiprojectTemplate5 = tech as TIProjectTemplate;
				if (tiprojectTemplate5 != null)
				{
					float projectProgressValueByTemplateFraction = activePlayer.GetProjectProgressValueByTemplateFraction(tiprojectTemplate5);
					this.techUnlockChanceText.SetText(projectProgressValueByTemplateFraction.ToPercent("P0"));
				}
			}
			if (techStatusAppearanceIndex == 5)
			{
				this.techUnlockChanceText.gameObject.SetActive(true);
				float accumulatedResearchByTech = TIGlobalResearchState.GetAccumulatedResearchByTech(tech as TITechTemplate);
				this.techUnlockChanceText.SetText((accumulatedResearchByTech / tech.GetResearchCost(activePlayer)).ToPercent("P0"));
			}
			if (techStatusAppearanceIndex == 10 || techStatusAppearanceIndex == 11)
			{
				this.lockIcon.gameObject.SetActive(true);
				this.techUnlockChanceText.gameObject.SetActive(true);
				this.techUnlockChanceText.SetText(num.ToPercent("P0"));
			}
			if (techStatusAppearanceIndex == 2 || techStatusAppearanceIndex == 6 || techStatusAppearanceIndex == 9 || techStatusAppearanceIndex == 7)
			{
				this.lockIcon.gameObject.SetActive(true);
			}
			if (techStatusAppearanceIndex == 0 || techStatusAppearanceIndex == 4 || techStatusAppearanceIndex == 8 || flag)
			{
				this.checkIcon.gameObject.SetActive(true);
				this.leftCheckIcon.gameObject.SetActive(true);
				this.leftBackgroundGradient.sprite = this.leftCompleteSprite;
				this.leftXIcon.gameObject.SetActive(false);
				this.techUnlockChanceText.gameObject.SetActive(true);
			}
			else
			{
				this.checkIcon.gameObject.SetActive(false);
				this.leftCheckIcon.gameObject.SetActive(false);
				this.leftBackgroundGradient.sprite = this.leftIncompleteSprite;
				this.leftXIcon.gameObject.SetActive(true);
			}
			this.backgroundGradient.sprite = controller.techStatusGradient[techStatusAppearanceIndex];
			this.xIcon.gameObject.SetActive(techStatusAppearanceIndex == 3 || techStatusAppearanceIndex == 7);
		}

		// Token: 0x0600564A RID: 22090 RVA: 0x00277E62 File Offset: 0x00276062
		public void OnClickItem()
		{
			if (this.techTreeLink != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
				this.controller.GotoSearchItem(this.techTreeLink, this.controller.currentTechTreeViewed);
			}
		}

		// Token: 0x04003D43 RID: 15683
		public TMP_Text techNameText;

		// Token: 0x04003D44 RID: 15684
		public TMP_Text techUnlockChanceText;

		// Token: 0x04003D45 RID: 15685
		public Image lockIcon;

		// Token: 0x04003D46 RID: 15686
		public Image xIcon;

		// Token: 0x04003D47 RID: 15687
		public Image checkIcon;

		// Token: 0x04003D48 RID: 15688
		public Image leftXIcon;

		// Token: 0x04003D49 RID: 15689
		public Image leftCheckIcon;

		// Token: 0x04003D4A RID: 15690
		public GameObject leftCompletionObject;

		// Token: 0x04003D4B RID: 15691
		public RectTransform leftCompletionRT;

		// Token: 0x04003D4C RID: 15692
		public GameObject projectIcon;

		// Token: 0x04003D4D RID: 15693
		public Image backgroundGradient;

		// Token: 0x04003D4E RID: 15694
		public Image leftBackgroundGradient;

		// Token: 0x04003D4F RID: 15695
		public TooltipTrigger techTooltip;

		// Token: 0x04003D50 RID: 15696
		public Sprite leftCompleteSprite;

		// Token: 0x04003D51 RID: 15697
		public Sprite leftIncompleteSprite;

		// Token: 0x04003D52 RID: 15698
		private ResearchScreenController controller;

		// Token: 0x04003D53 RID: 15699
		private ChildTechGridItemController techTreeLink;
	}
}
