using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C5 RID: 2245
	public class ResearchPanelController : MonoBehaviour
	{
		// Token: 0x17000F14 RID: 3860
		// (get) Token: 0x060055AF RID: 21935 RVA: 0x0026F030 File Offset: 0x0026D230
		// (set) Token: 0x060055B0 RID: 21936 RVA: 0x0026F038 File Offset: 0x0026D238
		public int slot { get; private set; }

		// Token: 0x060055B1 RID: 21937 RVA: 0x0026F044 File Offset: 0x0026D244
		public void Init(ResearchScreenController controller, int idx)
		{
			this.controller = controller;
			this.globalResearchState = GameStateManager.GlobalResearch();
			this.slot = idx;
			if (this.slot <= 2)
			{
				this.forceSelectTechButtonText.SetText(Loc.T("UI.Science.Panel.SelectNewTech"));
				this.leftButtonText.SetText(Loc.T("UI.Science.Panel.TechDetailsButton"));
				this.rightButtonText.SetText(Loc.T("UI.Science.Panel.TechTreeButton"));
				this.researchTooltip.SetText("BodyText", Loc.T("UI.Science.LeaderTip"));
			}
			else
			{
				this.forceSelectTechButtonText.SetText(Loc.T("UI.Science.Panel.SelectNewProject"));
				this.leftButtonText.SetText(Loc.T("UI.Science.Panel.ProjectDetailsButton"));
				this.rightButtonText.SetText(Loc.T("UI.Science.Panel.ChangeProjectButton"));
			}
			this.mainInfoPanel.SetActive(true);
		}

		// Token: 0x060055B2 RID: 21938 RVA: 0x0026F11C File Offset: 0x0026D31C
		public static string TechCategoryTooltip(TIFactionState faction, TIGenericTechTemplate currentGenericTemplate)
		{
			float num = faction.SumCategoryModifiers(currentGenericTemplate.techCategory);
			float num2 = faction.DistributedCategoryModifierValue(currentGenericTemplate.techCategory);
			if (num2 == 0f)
			{
				return Loc.T((faction == GameControl.control.activePlayer) ? "UI.Science.Panel.TechCategoryTooltip_NoBonus" : "UI.Science.Panel.TechCategoryTooltip_NoBonus_Other", new object[] { currentGenericTemplate.categoryString });
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (num2 < 0f && faction == GameControl.control.activePlayer)
			{
				stringBuilder.Append(Loc.T("UI.Science.Panel.TechCategoryTooltip_Malus", new object[] { currentGenericTemplate.categoryString })).AppendLine().AppendLine();
			}
			string text = Loc.T("UI.Science.Panel.PositiveBonus", new object[] { num2.ToPercent("+0%;-0%;0%") });
			stringBuilder.Append(Loc.T((faction == GameControl.control.activePlayer) ? "UI.Science.Panel.TechCategoryTooltip_Bonus" : "UI.Science.Panel.TechCategoryTooltip_Bonus_Other", new object[] { text, currentGenericTemplate.categoryString })).AppendLine();
			float num3 = faction.HabsMultiplier(currentGenericTemplate.techCategory);
			float num4 = faction.OrgsMultiplier(currentGenericTemplate.techCategory);
			float num5 = faction.TraitsMultiplier(currentGenericTemplate.techCategory);
			float num6 = faction.FleetsModifier(currentGenericTemplate.techCategory);
			float num7 = faction.EffectsModifier(currentGenericTemplate.techCategory);
			float num8 = faction.InvestigationsModifier(currentGenericTemplate.techCategory);
			if (num5 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Councilors", new object[] { num5.ToPercent("P0") }));
			}
			if (num4 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Orgs", new object[] { num4.ToPercent("P0") }));
			}
			if (num3 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Habs", new object[] { num3.ToPercent("P0") }));
			}
			if (num6 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Fleets", new object[] { num6.ToPercent("P0") }));
			}
			if (num8 > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Investigations", new object[] { num8.ToPercent("P0") }));
			}
			if (num7 != 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.Panel.Effects", new object[] { num7.ToPercent("P0") }));
			}
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Science.Panel.DiminishingReturns"));
			if (num2 != num)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Science.Panel.BonusDistribution", new object[]
				{
					num.ToPercent("P0"),
					num2.ToPercent("P0")
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060055B3 RID: 21939 RVA: 0x0026F3F0 File Offset: 0x0026D5F0
		public void UpdatePanel(TIFactionState faction)
		{
			this.priority = faction.researchWeights[this.slot];
			TIGenericTechTemplate currentGenericTemplate = null;
			if ((this.slot <= 2 && TIPromptQueueState.HasPromptStatic(this.controller.activePlayer, this.globalResearchState, null, "PromptSelectTech", this.slot)) || TIPromptQueueState.HasPromptStatic(this.controller.activePlayer, this.controller.activePlayer, null, "PromptSelectProject", this.slot))
			{
				this.forceSelectTechOverlay.enabled = true;
				return;
			}
			this.forceSelectTechOverlay.enabled = false;
			if (this.slot <= 2)
			{
				TechProgress techProgress = this.globalResearchState.GetTechProgress(this.slot);
				currentGenericTemplate = techProgress.techTemplate;
				this.projectName.SetText(techProgress.techTemplate.displayName);
				this.summary.text = techProgress.techTemplate.summary;
				this.wedge.color = TemplateManager.global.techColor[(int)techProgress.techTemplate.techCategory];
				this.wedge.fillAmount = techProgress.accumulatedResearch / techProgress.techTemplate.GetResearchCost(faction);
				this.progressFraction.SetText(Loc.T("UI.Science.Panel.OverallProgress", new object[]
				{
					techProgress.accumulatedResearch.ToString("N0"),
					techProgress.techTemplate.GetResearchCost(faction).ToString("N0"),
					TemplateManager.global.researchInlineSpritePath,
					"TO DO"
				}));
				string text = this.globalResearchState.TechCompletionDate(this.slot);
				if (text != string.Empty)
				{
					this.completionDate.enabled = true;
					this.completionDate.SetText(Loc.T("UI.Science.Panel.EstimatedCompletion", new object[] { text }));
				}
				else
				{
					this.completionDate.enabled = false;
				}
				if (techProgress.factionContributions.ContainsKey(faction))
				{
					this.playerContribution.SetText(Loc.T("UI.Science.Panel.MyFactionContribution", new object[]
					{
						faction.displayNameCapitalized,
						techProgress.factionContributions[faction].ToString("N0"),
						TemplateManager.global.researchInlineSpritePath
					}));
				}
				else
				{
					this.playerContribution.text = string.Empty;
				}
				this.factionContribution.SetListSize<FactionContributionListItemController>(techProgress.factionContributions.Count, false, false);
				this.factionContributionBar.SetListSize<FactionContributionBarListItemController>(techProgress.factionContributions.Count, false, false);
				List<TIFactionState> list = new List<TIFactionState>();
				foreach (TIFactionState tifactionState in techProgress.factionContributions.Keys)
				{
					list.Add(tifactionState);
				}
				list = list.OrderBy<TIFactionState, string>((TIFactionState o) => o.displayName).ToList<TIFactionState>();
				list.OrderBy<TIFactionState, bool>((TIFactionState o) => o == GameControl.control.activePlayer).ToList<TIFactionState>();
				int num = 0;
				using (IEnumerator<object> enumerator2 = this.factionContribution.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (ResearchPanelController.<>o__41.<>p__0 == null)
						{
							ResearchPanelController.<>o__41.<>p__0 = CallSite<Func<CallSite, object, FactionContributionListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionContributionListItemController), typeof(ResearchPanelController)));
						}
						ResearchPanelController.<>o__41.<>p__0.Target(ResearchPanelController.<>o__41.<>p__0, enumerator2.Current).UpdateListItem(list[num++], techProgress);
					}
				}
				int num2 = 0;
				using (IEnumerator<object> enumerator2 = this.factionContributionBar.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (ResearchPanelController.<>o__41.<>p__1 == null)
						{
							ResearchPanelController.<>o__41.<>p__1 = CallSite<Func<CallSite, object, FactionContributionBarListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionContributionBarListItemController), typeof(ResearchPanelController)));
						}
						ResearchPanelController.<>o__41.<>p__1.Target(ResearchPanelController.<>o__41.<>p__1, enumerator2.Current).UpdateListItem(list[num2++], techProgress, techProgress.factionContributions.Count);
					}
				}
				this.leftPanelHeadline.text = techProgress.techTemplate.displayName;
				this.leftPanelMainText.text = techProgress.techTemplate.GetFullDescription(this.controller.activePlayer, TechBenefitsContext.Prospective, null, false);
				GameControl.assetLoader.LoadAssetForImageAssignment(techProgress.techTemplate.IconResource, this.techCategoryIcon);
				GameControl.assetLoader.LoadAssetForImageAssignment(techProgress.techTemplate.IconResource, this.techCategoryIconFaded);
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.gradientTechCategoryPath[techProgress.TechCategory], this.techCategoryGradient);
				TIFactionState tifactionState2 = this.globalResearchState.Leader(this.slot);
				if (tifactionState2 != null && techProgress.factionContributions[tifactionState2] > 0f && techProgress.accumulatedResearch > 0f)
				{
					this.researchTypeIcon.sprite = tifactionState2.factionIcon128UI;
					this.researchTypeBonus.SetText((techProgress.factionContributions[tifactionState2] / techProgress.accumulatedResearch).ToPercent("P0"));
					this.researchTypePanel.SetActive(true);
				}
				else
				{
					this.researchTypePanel.SetActive(false);
				}
			}
			else
			{
				ProjectProgress projectProgressInSlot = faction.GetProjectProgressInSlot(this.slot);
				currentGenericTemplate = projectProgressInSlot.projectTemplate;
				this.projectName.text = projectProgressInSlot.projectTemplate.displayName;
				this.summary.text = projectProgressInSlot.projectTemplate.summary;
				this.wedge.fillAmount = projectProgressInSlot.accumulatedResearch / projectProgressInSlot.projectTemplate.GetResearchCost(faction);
				this.wedge.color = TemplateManager.global.techColor[(int)projectProgressInSlot.projectTemplate.techCategory];
				this.progressFraction.SetText(Loc.T("UI.Science.Panel.OverallProgress", new object[]
				{
					projectProgressInSlot.accumulatedResearch.ToString("N0"),
					projectProgressInSlot.projectTemplate.GetResearchCost(faction).ToString("N0"),
					TemplateManager.global.researchInlineSpritePath
				}));
				string text2 = faction.ProjectCompletionDate(this.slot);
				if (text2 != string.Empty)
				{
					this.completionDate.enabled = true;
					this.completionDate.SetText(Loc.T("UI.Science.Panel.EstimatedCompletion", new object[] { text2 }));
				}
				else
				{
					this.completionDate.enabled = false;
				}
				this.playerContribution.enabled = false;
				this.playerContributionObject.SetActive(false);
				this.factionContribution.SetListSize<FactionContributionListItemController>(0, false, false);
				this.factionContribution.enabled = false;
				this.factionContributionBar.SetListSize<FactionContributionBarListItemController>(1, false, false);
				using (IEnumerator<object> enumerator2 = this.factionContributionBar.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (ResearchPanelController.<>o__41.<>p__2 == null)
						{
							ResearchPanelController.<>o__41.<>p__2 = CallSite<Func<CallSite, object, FactionContributionBarListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(FactionContributionBarListItemController), typeof(ResearchPanelController)));
						}
						ResearchPanelController.<>o__41.<>p__2.Target(ResearchPanelController.<>o__41.<>p__2, enumerator2.Current).UpdateListItem(faction, projectProgressInSlot, 0);
					}
				}
				this.leftPanelHeadline.text = projectProgressInSlot.projectTemplate.displayName;
				this.leftPanelMainText.text = projectProgressInSlot.projectTemplate.GetFullDescription(this.controller.activePlayer, TechBenefitsContext.Prospective, null, false);
				GameControl.assetLoader.LoadAssetForImageAssignment(projectProgressInSlot.projectTemplate.IconResource, this.techCategoryIcon);
				GameControl.assetLoader.LoadAssetForImageAssignment(projectProgressInSlot.projectTemplate.IconResource, this.techCategoryIconFaded);
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.gradientTechCategoryPath[projectProgressInSlot.projectCategory], this.techCategoryGradient);
				this.researchTypePanel.SetActive(true);
				int num3 = faction.TraitProjectCount();
				int orgProjectCount = faction.OrgProjectCount();
				int habProjectCount = faction.HabProjectCount();
				float num4 = faction.MultipleFacilitiesMultiplier(num3, orgProjectCount, habProjectCount);
				if (num4 > 0f)
				{
					string bonusStr = Loc.T("UI.Science.Panel.PositiveBonus", new object[] { num4.ToPercent("P0") });
					this.researchTypeBonus.SetText(bonusStr);
					switch (this.slot)
					{
					case 3:
						this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_HQ", new object[] { faction.adjective, bonusStr }));
						break;
					case 4:
						if (orgProjectCount > 1)
						{
							this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_OrgsBonus", new object[]
							{
								orgProjectCount.ToString("N0"),
								bonusStr
							}));
						}
						else
						{
							this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_NoOrgBonus2", new object[] { bonusStr }));
						}
						break;
					case 5:
						if (habProjectCount > 1)
						{
							this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_HabsBonus", new object[]
							{
								habProjectCount.ToString("N0"),
								bonusStr
							}));
						}
						else
						{
							this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_NoHabBonus2", new object[] { bonusStr }));
						}
						break;
					}
				}
				else
				{
					this.researchTypeBonus.SetText(string.Empty);
					switch (this.slot)
					{
					case 3:
						this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_HQNoBonus", new object[] { faction.adjective }));
						break;
					case 4:
						this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_NoOrgBonus"));
						break;
					case 5:
						this.researchTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.ResearchTypeTooltip_NoHabBonus"));
						break;
					}
				}
			}
			float num5 = faction.SumCategoryModifiers(currentGenericTemplate.techCategory);
			if (num5 > 0f)
			{
				float num6 = faction.DistributedCategoryModifierValue(currentGenericTemplate.techCategory);
				string text3 = Loc.T("UI.Science.Panel.PositiveBonus", new object[] { num6.ToPercent("P0") });
				this.techCategoryBonus.SetText(text3);
				this.techTooltip.SetDelegate("BodyText", () => ResearchPanelController.TechCategoryTooltip(faction, currentGenericTemplate));
			}
			else if (num5 < 0f)
			{
				this.techCategoryBonus.SetText(num5.ToPercent("P0"));
				this.techTooltip.SetDelegate("BodyText", () => ResearchPanelController.TechCategoryTooltip(faction, currentGenericTemplate));
			}
			else
			{
				this.techCategoryBonus.SetText(string.Empty);
				this.techTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.Panel.TechCategoryTooltip_NoBonus", new object[] { currentGenericTemplate.categoryString }));
			}
			switch (this.priority)
			{
			case 0:
				this.lowPriorityImage.color = ResearchPanelController.grayColor;
				this.mediumPriorityImage.color = ResearchPanelController.grayColor;
				this.highPriorityImage.color = ResearchPanelController.grayColor;
				break;
			case 1:
				this.lowPriorityImage.color = TIUtilities.UIColorIndicatorNegative;
				this.mediumPriorityImage.color = ResearchPanelController.grayColor;
				this.highPriorityImage.color = ResearchPanelController.grayColor;
				break;
			case 2:
				this.lowPriorityImage.color = TIUtilities.UIColorIndicatorNeutral;
				this.mediumPriorityImage.color = TIUtilities.UIColorIndicatorNeutral;
				this.highPriorityImage.color = ResearchPanelController.grayColor;
				break;
			case 3:
				this.lowPriorityImage.color = TIUtilities.UIColorIndicatorPositive;
				this.mediumPriorityImage.color = TIUtilities.UIColorIndicatorPositive;
				this.highPriorityImage.color = TIUtilities.UIColorIndicatorPositive;
				break;
			}
			this.ContributionWeightPercentageText.SetText(Loc.T("UI.Science.Panel.DailyContributionFraction", new object[]
			{
				faction.FractionWeightInSlot(this.slot, faction.OrgProjectAllowed(), faction.HabProjectAllowed()).ToPercent("P0"),
				faction.PointsToSlot(this.slot, faction.GetDailyIncome(FactionResource.Research, false, false), (float)faction.TotalResearchWeights(faction.OrgProjectAllowed(), faction.HabProjectAllowed())).ToString("N3"),
				TemplateManager.global.researchInlineSpritePath
			}));
		}

		// Token: 0x060055B4 RID: 21940 RVA: 0x0027018C File Offset: 0x0026E38C
		public void OnEnable()
		{
			this.leftButtonOverlayPanel.SetActive(false);
			this.mainInfoPanel.SetActive(true);
		}

		// Token: 0x060055B5 RID: 21941 RVA: 0x002701A6 File Offset: 0x0026E3A6
		public void ActivateLeftButtonPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.mainInfoPanel.SetActive(false);
			this.leftButtonOverlayPanel.SetActive(true);
		}

		// Token: 0x060055B6 RID: 21942 RVA: 0x002701CC File Offset: 0x0026E3CC
		public void ExitLeftPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.leftButtonOverlayPanel.SetActive(false);
			this.mainInfoPanel.SetActive(true);
		}

		// Token: 0x060055B7 RID: 21943 RVA: 0x002701F4 File Offset: 0x0026E3F4
		public void ActivateRightButtonPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.controller.HideTutorials();
			if (this.slot <= 2)
			{
				TIGenericTechTemplate techTemplate = this.globalResearchState.GetTechProgress(this.slot).techTemplate;
				this.controller.SetSelectedTechEntry(techTemplate.dataName);
				this.controller.DisplayTechTree(techTemplate);
				if (!ResearchScreenController.fullTechTreeOn)
				{
					this.controller.rightButtonOverlayPanel.enabled = true;
					return;
				}
			}
			else
			{
				this.controller.UpdateSelectProjectPanel(this.controller.activePlayer, this.slot);
				this.controller.FillOutProjectData();
				this.controller.selectProjectOverlay.enabled = true;
				this.controller.SetChangingProjectSlot(this.slot);
			}
		}

		// Token: 0x060055B8 RID: 21944 RVA: 0x002702B8 File Offset: 0x0026E4B8
		public void OnSelectTechButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.forceSelectTechOverlay.enabled = false;
			if (this.slot <= 2)
			{
				this.controller.UpdateSelectTechPanel(this.controller.activePlayer);
				this.controller.FillOutTechData();
				this.controller.selectTechOverlay.enabled = true;
				this.controller.SetChangingTechSlot(this.slot);
				return;
			}
			this.controller.UpdateSelectProjectPanel(this.controller.activePlayer, this.slot);
			this.controller.FillOutProjectData();
			this.controller.selectProjectOverlay.enabled = true;
			this.controller.SetChangingProjectSlot(this.slot);
		}

		// Token: 0x060055B9 RID: 21945 RVA: 0x00270374 File Offset: 0x0026E574
		public void OnPriorityButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.activePlayer.playerControl.StartAction(new CycleResearchPriorityAction(this.controller.activePlayer, this.slot, false));
			this.controller.UpdateResearchLists(this.controller.activePlayer);
		}

		// Token: 0x060055BA RID: 21946 RVA: 0x002703D0 File Offset: 0x0026E5D0
		public void OnRightPriorityButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.controller.activePlayer.playerControl.StartAction(new CycleResearchPriorityAction(this.controller.activePlayer, this.slot, true));
			this.controller.UpdateResearchLists(this.controller.activePlayer);
		}

		// Token: 0x04003C2E RID: 15406
		private ResearchScreenController controller;

		// Token: 0x04003C2F RID: 15407
		public GameObject mainInfoPanel;

		// Token: 0x04003C30 RID: 15408
		public TMP_Text projectName;

		// Token: 0x04003C31 RID: 15409
		public TMP_Text summary;

		// Token: 0x04003C32 RID: 15410
		public Image piechartCircle;

		// Token: 0x04003C33 RID: 15411
		public Image wedge;

		// Token: 0x04003C34 RID: 15412
		public Image techCategoryIcon;

		// Token: 0x04003C35 RID: 15413
		public Image techCategoryIconFaded;

		// Token: 0x04003C36 RID: 15414
		public Image techCategoryGradient;

		// Token: 0x04003C37 RID: 15415
		public TMP_Text techCategoryBonus;

		// Token: 0x04003C38 RID: 15416
		public TooltipTrigger techTooltip;

		// Token: 0x04003C39 RID: 15417
		public GameObject researchTypePanel;

		// Token: 0x04003C3A RID: 15418
		public Image researchTypeIcon;

		// Token: 0x04003C3B RID: 15419
		public TMP_Text researchTypeBonus;

		// Token: 0x04003C3C RID: 15420
		public TooltipTrigger researchTooltip;

		// Token: 0x04003C3D RID: 15421
		public TMP_Text progressFraction;

		// Token: 0x04003C3E RID: 15422
		public TMP_Text completionDate;

		// Token: 0x04003C3F RID: 15423
		public GameObject playerContributionObject;

		// Token: 0x04003C40 RID: 15424
		public TMP_Text playerContribution;

		// Token: 0x04003C41 RID: 15425
		public ListManagerBase factionContribution;

		// Token: 0x04003C42 RID: 15426
		public ListManagerBase factionContributionBar;

		// Token: 0x04003C43 RID: 15427
		public GameObject leftButtonOverlayPanel;

		// Token: 0x04003C44 RID: 15428
		public TMP_Text leftButtonText;

		// Token: 0x04003C45 RID: 15429
		public TMP_Text rightButtonText;

		// Token: 0x04003C46 RID: 15430
		public TMP_Text leftPanelHeadline;

		// Token: 0x04003C47 RID: 15431
		public TMP_Text leftPanelMainText;

		// Token: 0x04003C48 RID: 15432
		public TMP_Text ContributionWeightPercentageText;

		// Token: 0x04003C49 RID: 15433
		public Canvas forceSelectTechOverlay;

		// Token: 0x04003C4A RID: 15434
		public TMP_Text forceSelectTechButtonText;

		// Token: 0x04003C4B RID: 15435
		public Image lowPriorityImage;

		// Token: 0x04003C4C RID: 15436
		public Image mediumPriorityImage;

		// Token: 0x04003C4D RID: 15437
		public Image highPriorityImage;

		// Token: 0x04003C4E RID: 15438
		private int priority;

		// Token: 0x04003C50 RID: 15440
		private TIGlobalResearchState globalResearchState;

		// Token: 0x04003C51 RID: 15441
		private static readonly Color32 grayColor = new Color32(45, 45, 45, byte.MaxValue);
	}
}
