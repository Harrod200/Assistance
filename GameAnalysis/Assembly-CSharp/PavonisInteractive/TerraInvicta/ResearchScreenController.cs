using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020008C6 RID: 2246
	public class ResearchScreenController : CanvasControllerBase, IInfoScreen, ICanvas
	{
		// Token: 0x060055BD RID: 21949 RVA: 0x0027044C File Offset: 0x0026E64C
		public override void Initialize()
		{
			base.Initialize();
			GameControl.eventManager.AddListener<ForceProjectSelectionUI>(new EventManager.EventDelegate<ForceProjectSelectionUI>(this.PushProjectSelection), "ForceProjectSelectionUI", null, false, false);
			GameControl.eventManager.AddListener<ForceTechSelectionUI>(new EventManager.EventDelegate<ForceTechSelectionUI>(this.PushTechSelection), "ForceTechSelectionUI", null, false, false);
			GameControl.eventManager.AddListener<ResearchUpdated>(new EventManager.EventDelegate<ResearchUpdated>(this.UpdateResearchValues), null, base.activePlayer, true, false);
			GameControl.eventManager.AddListener<ProjectSelectedFromRemoteUI>(new EventManager.EventDelegate<ProjectSelectedFromRemoteUI>(this.OnProjectSelectedRemotely), null, base.activePlayer, false, false);
			this.archiveOverlayTitle.SetText(Loc.T("UI.Science.ArchivesOverlayTitle"));
			this.selectTechOverlayTitle.SetText(Loc.T("UI.Science.SelectTechTitle"));
			this.selectTechButtonText.SetText(Loc.T("UI.Science.SelectTechButton"));
			this.selectProjectOverlayTitle.SetText(Loc.T("UI.Science.SelectProjectTitle"));
			this.selectProjectButtonText.SetText(Loc.T("UI.Science.SelectProjectButton"));
			this.researchTabText.SetText(Loc.T("UI.Science.Research"));
			this.archiveTabText.SetText(Loc.T("UI.Science.ArchivesButton"));
			this.techTreeTabText.SetText(Loc.T("UI.Science.Panel.TechTreeButton"));
			this.researchPanelHeader.SetText(Loc.T("UI.Science.ResearchPanelHeader"));
			this.globalResearchHeader.SetText(Loc.T("UI.Science.GlobalPanelHeader"));
			this.councilEngineeringHeader.SetText(Loc.T("UI.Science.EngineeringPanelHeader", new object[] { base.activePlayer.adjective }));
			this.orgProjectRequiredExplainer.SetText(Loc.T("UI.Science.OrgProjectRequiredExplainer"));
			this.habProjectRequiredExplainer.SetText(Loc.T("UI.Science.HabProjectRequiredExplainer"));
			this.techTreeTitle.SetText(Loc.T("UI.Science.TechTreeHeader"));
			this.techTreeHeader.SetText(Loc.T("UI.Science.TechTreeHeader"));
			this.techTreeHeaderNP.SetText(Loc.T("UI.Science.TechTreeHeader"));
			this.treeSwapButtonText.SetText(Loc.T("UI.Science.TechTreeSwapButton"));
			this.treeSimpleButtonText.SetText(Loc.T("UI.Science.TechTreeSimpleButton"));
			this.closeSelectiveTreeButtonText.SetText(Loc.T("UI.Notifications.Back"));
			this.techTreeZoomText.SetText(Loc.T("UI.Habs.Zoom"));
			this.searchPanelHeaderTechs.SetText(Loc.T("UI.Science.Search"));
			this.searchPanelHeaderProjects.SetText(Loc.T("UI.Science.Search"));
			this.searchPanelHeaderFullTechs.SetText(Loc.T("UI.Science.SearchFull"));
			this.searchPanelHeaderFullProjects.SetText(Loc.T("UI.Science.SearchFull"));
			this.orText.SetText(Loc.T("UI.Science.Or"));
			this.projectSortByText.SetText(Loc.T("UI.Science.ArrangeBy"));
			this.projectSortAscendText.SetText(Loc.T("UI.Science.SortAscend"));
			this.projectSortObsoleteText.SetText(Loc.T("UI.Science.SortObsolete"));
			this.sortProjectDropdown.options[0].text = Loc.T("UI.Science.Name");
			this.sortProjectDropdown.options[1].text = Loc.T("UI.Science.Category");
			this.sortProjectDropdown.options[2].text = Loc.T("UI.Science.Cost");
			this.techSortByText.SetText(Loc.T("UI.Science.ArrangeBy"));
			this.techSortAscendText.SetText(Loc.T("UI.Science.SortAscend"));
			this.sortTechDropdown.options[0].text = Loc.T("UI.Science.Name");
			this.sortTechDropdown.options[1].text = Loc.T("UI.Science.Category");
			this.sortTechDropdown.options[2].text = Loc.T("UI.Science.Cost");
			this.archiveSearchTitle.SetText(Loc.T("UI.GeneralControls.GlobalSearch"));
			this.effectsSearchTitle.SetText(Loc.T("UI.GeneralControls.GlobalSearch"));
			this.selectedTechPanelLongTermButtonTooltip.SetDelegate("BodyText", () => Loc.T("UI.Science.LongTermTechTooltip"));
			this.primaryResearchPanel.gameObject.SetActive(true);
			this.selectProjectOverlay.gameObject.SetActive(true);
			this.selectTechOverlay.gameObject.SetActive(true);
			this.archivesOverlayPanel.gameObject.SetActive(true);
			this.rightButtonOverlayPanel.gameObject.SetActive(true);
			this.techTreeMasterObject.SetActive(true);
			ResearchScreenController.fullTechTreeOn = this.usingFullTechTree;
			this.globalResearchState = GameStateManager.FindGameState<TIGlobalResearchState>();
			int num = 0;
			foreach (ResearchPanelController researchPanelController in this.researchPanelGrid)
			{
				researchPanelController.Init(this, num++);
				if (num < 4)
				{
					researchPanelController.gameObject.SetActive(true);
				}
			}
			this.effectsBreakdownCanvas.gameObject.SetActive(true);
			this.InitializeEffectsBreakdownScreen();
		}

		// Token: 0x060055BE RID: 21950 RVA: 0x00270941 File Offset: 0x0026EB41
		public override void Show()
		{
			base.Show();
			this.ResetAllPanels();
			GameControl.eventManager.AddListener<ProjectUIOptionsChanged>(new EventManager.EventDelegate<ProjectUIOptionsChanged>(this.ProjectUIOptionsUpdated), null, base.activePlayer, true, false);
		}

		// Token: 0x060055BF RID: 21951 RVA: 0x00270970 File Offset: 0x0026EB70
		public override void Hide()
		{
			this.primaryResearchPanel.enabled = false;
			this.archivesOverlayPanel.enabled = false;
			this.selectProjectOverlay.enabled = false;
			this.selectTechOverlay.enabled = false;
			this.rightButtonOverlayPanel.enabled = false;
			this.fullTechTreeCanvas.enabled = false;
			this.fullTechTreeCanvasNP.enabled = false;
			this.selectiveTechTreeCanvas.enabled = false;
			this.HideTutorials();
			base.Hide();
			GameControl.eventManager.RemoveListener<ProjectUIOptionsChanged>(new EventManager.EventDelegate<ProjectUIOptionsChanged>(this.ProjectUIOptionsUpdated), null);
		}

		// Token: 0x060055C0 RID: 21952 RVA: 0x00270A00 File Offset: 0x0026EC00
		public override void HideNoCache()
		{
			this.primaryResearchPanel.enabled = false;
			this.archivesOverlayPanel.enabled = false;
			this.selectProjectOverlay.enabled = false;
			this.selectTechOverlay.enabled = false;
			this.rightButtonOverlayPanel.enabled = false;
			this.fullTechTreeCanvas.enabled = false;
			this.fullTechTreeCanvasNP.enabled = false;
			this.selectiveTechTreeCanvas.enabled = false;
			this.HideTutorials();
			base.HideNoCache();
		}

		// Token: 0x060055C1 RID: 21953 RVA: 0x00270A7C File Offset: 0x0026EC7C
		public override void Refresh()
		{
			if (this.techTreeContentToScale != null)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					this.ToggleTechTreeMoveScrolling(false);
					float num = Input.mouseScrollDelta.y * 0.05f;
					if (UIMagnifier.IsMagnifierActive)
					{
						num = 0f;
					}
					this.techTreeContentToScale.localScale = new Vector3(Mathf.Clamp(this.techTreeContentToScale.localScale.x + num, 0.3f, 1f), Mathf.Clamp(this.techTreeContentToScale.localScale.y + num, 0.3f, 1f), 1f);
					this.UpdateTechTreeZoomSlider();
					return;
				}
				this.ToggleTechTreeMoveScrolling(true);
			}
		}

		// Token: 0x060055C2 RID: 21954 RVA: 0x00270B2E File Offset: 0x0026ED2E
		public override bool Visible()
		{
			return base.Visible() && base.canvasManager.IsShowingInfoScreen<ResearchScreenController>();
		}

		// Token: 0x060055C3 RID: 21955 RVA: 0x00270B48 File Offset: 0x0026ED48
		public void CloseInfoScreen(bool toggle = false)
		{
			if (this.openingSelectiveTree)
			{
				return;
			}
			this.primaryResearchPanel.enabled = false;
			this.archivesOverlayPanel.enabled = false;
			this.selectProjectOverlay.enabled = false;
			this.selectTechOverlay.enabled = false;
			base.canvasManager.HideInfoScreen<ResearchScreenController>(toggle);
		}

		// Token: 0x060055C4 RID: 21956 RVA: 0x00270B9A File Offset: 0x0026ED9A
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.primaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x060055C5 RID: 21957 RVA: 0x00270BCB File Offset: 0x0026EDCB
		public void ShowResearchTutorial()
		{
			this.HideTutorials();
			if (!GameControl.loadcycle100 || GameControl.control.skirmishMode || TIGlobalValuesState.isSpaceCombatEnabled)
			{
				return;
			}
			this.researchUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_ResearchScreenCanvas_Primary, false, true);
		}

		// Token: 0x060055C6 RID: 21958 RVA: 0x00270C00 File Offset: 0x0026EE00
		public void ShowArchivesTutorial()
		{
			this.HideTutorials();
			this.archivesUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_ResearchScreenCanvas_Archives, false, true);
		}

		// Token: 0x060055C7 RID: 21959 RVA: 0x00270C1A File Offset: 0x0026EE1A
		public void ShowTechTreeTutorial()
		{
			this.HideTutorials();
			if (base.Canvas.enabled)
			{
				this.techTreeUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_ResearchScreenCanvas_TechTree, false, true);
			}
		}

		// Token: 0x060055C8 RID: 21960 RVA: 0x00270C41 File Offset: 0x0026EE41
		public void ShowModifiersTutorial()
		{
			this.HideTutorials();
			this.modifiersUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_ResearchScreenCanvas_Modifiers, false, true);
		}

		// Token: 0x060055C9 RID: 21961 RVA: 0x00270C5B File Offset: 0x0026EE5B
		public void HideTutorials()
		{
			this.researchUITutorialController.HideTutorial();
			this.archivesUITutorialController.HideTutorial();
			this.techTreeUITutorialController.HideTutorial();
			this.modifiersUITutorialController.HideTutorial();
		}

		// Token: 0x060055CA RID: 21962 RVA: 0x00270C89 File Offset: 0x0026EE89
		public void OnArchiveButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.primaryResearchPanel.enabled = false;
			this.HideTutorials();
			this.archivesOverlayPanel.enabled = true;
		}

		// Token: 0x060055CB RID: 21963 RVA: 0x00270CB8 File Offset: 0x0026EEB8
		public void OnSelectProjectButtonSelected()
		{
			if (!string.IsNullOrEmpty(this.selectedProjectEntry))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(this.selectedProjectEntry, false);
				base.activePlayer.playerControl.StartAction(new SelectProjectForDevelopmentAction(base.activePlayer, this.changingProjectSlot, tiprojectTemplate));
				this.selectedProjectEntry = string.Empty;
				this.HighlightSelectedProjectEntry(null);
				this.ResetAllPanels();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x060055CC RID: 21964 RVA: 0x00270D34 File Offset: 0x0026EF34
		public void OnSelectTechButtonSelected()
		{
			if (!string.IsNullOrEmpty(this.selectedTechEntry))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(this.selectedTechEntry, false);
				base.activePlayer.playerControl.StartAction(new SelectTechAction(base.activePlayer, this.selectTechSlot, titechTemplate));
				this.selectedTechEntry = string.Empty;
				this.HighlightSelectedTechEntry(null);
				this.ResetAllPanels();
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x060055CD RID: 21965 RVA: 0x00270DAE File Offset: 0x0026EFAE
		public void ExitResearchScreen()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.HideTutorials();
			this.CloseInfoScreen(false);
		}

		// Token: 0x060055CE RID: 21966 RVA: 0x00270DC9 File Offset: 0x0026EFC9
		public void OnSelectCloseandPlay()
		{
			this.ExitResearchScreen();
			base.gameTime.Play();
		}

		// Token: 0x060055CF RID: 21967 RVA: 0x00270DDC File Offset: 0x0026EFDC
		public void ExitRightPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.rightButtonOverlayPanel.enabled = false;
			this.primaryResearchPanel.enabled = true;
			this.ShowResearchTutorial();
			this.mainGridObject.SetActive(true);
		}

		// Token: 0x060055D0 RID: 21968 RVA: 0x00270E14 File Offset: 0x0026F014
		public void SetSelectedArchiveEntry(string dataName)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.selectedArchiveEntry = dataName;
			this.FillOutArchiveData();
			this.HighlightSelectedArchiveEntry(this.selectedArchiveEntry);
		}

		// Token: 0x060055D1 RID: 21969 RVA: 0x00270E3C File Offset: 0x0026F03C
		public void HighlightSelectedArchiveEntry(string entryDataName = "")
		{
			bool flag = string.IsNullOrEmpty(entryDataName);
			using (IEnumerator<object> enumerator = this.archivedTechsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__117.<>p__0 == null)
					{
						ResearchScreenController.<>o__117.<>p__0 = CallSite<Func<CallSite, object, CombinedResearchListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombinedResearchListItemController), typeof(ResearchScreenController)));
					}
					CombinedResearchListItemController combinedResearchListItemController = ResearchScreenController.<>o__117.<>p__0.Target(ResearchScreenController.<>o__117.<>p__0, enumerator.Current);
					combinedResearchListItemController.SetSelected(!flag && entryDataName == combinedResearchListItemController.heldDataName);
				}
			}
		}

		// Token: 0x060055D2 RID: 21970 RVA: 0x00270EE4 File Offset: 0x0026F0E4
		public void ExitArchivePanel()
		{
			this.archivesOverlayPanel.enabled = false;
			this.primaryResearchPanel.enabled = true;
			this.ShowResearchTutorial();
			this.mainGridObject.SetActive(true);
			this.ResetAllPanels();
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x060055D3 RID: 21971 RVA: 0x00270F24 File Offset: 0x0026F124
		public void ExitSelectProjectPanel()
		{
			this.selectProjectOverlay.enabled = false;
			this.ShowResearchTutorial();
			this.mainGridObject.SetActive(true);
			this.ResetAllPanels();
			this.selectedProjectEntry = string.Empty;
			this.HighlightSelectedProjectEntry(null);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x060055D4 RID: 21972 RVA: 0x00270F74 File Offset: 0x0026F174
		public void ExitSelectTechPanel()
		{
			this.selectTechOverlay.enabled = false;
			this.ShowResearchTutorial();
			this.mainGridObject.SetActive(true);
			this.ResetAllPanels();
			this.selectedTechEntry = string.Empty;
			this.HighlightSelectedTechEntry(null);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x060055D5 RID: 21973 RVA: 0x00270FC4 File Offset: 0x0026F1C4
		private int GetFirstRequiredSlot(TIFactionState faction)
		{
			for (int i = 3; i <= 5; i++)
			{
				if (faction.NewProjectRequired(i))
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x060055D6 RID: 21974 RVA: 0x00270FE9 File Offset: 0x0026F1E9
		private int GetLowestTechFactionWon(TIFactionState faction)
		{
			return this.globalResearchState.GetSlotForFactionCompletedTechs(faction);
		}

		// Token: 0x060055D7 RID: 21975 RVA: 0x00270FF8 File Offset: 0x0026F1F8
		private void OnProjectSelectedRemotely(ProjectSelectedFromRemoteUI e)
		{
			if (this.Visible())
			{
				if (this.selectProjectOverlay.enabled)
				{
					this.selectProjectOverlay.enabled = false;
					this.primaryResearchPanel.enabled = true;
				}
				else
				{
					bool enabled = this.archivesOverlayPanel.enabled;
				}
				if (this.primaryResearchPanel.enabled)
				{
					this.UpdateResearchLists(base.activePlayer);
				}
			}
		}

		// Token: 0x060055D8 RID: 21976 RVA: 0x0027105C File Offset: 0x0026F25C
		private void PushProjectSelection(ForceProjectSelectionUI e)
		{
			if (e.councilState == base.activePlayer)
			{
				base.canvasManager.ShowInfoScreen<ResearchScreenController>();
				int firstRequiredSlot = this.GetFirstRequiredSlot(base.activePlayer);
				if (firstRequiredSlot != -1)
				{
					this.SetChangingProjectSlot(firstRequiredSlot);
					this.UpdateSelectProjectPanel(base.activePlayer, this.changingProjectSlot);
					this.FillOutProjectData();
					this.archivesOverlayPanel.enabled = false;
					this.selectTechOverlay.enabled = false;
					this.HideTutorials();
					this.selectProjectOverlay.enabled = true;
				}
			}
		}

		// Token: 0x060055D9 RID: 21977 RVA: 0x002710E4 File Offset: 0x0026F2E4
		private void PushTechSelection(ForceTechSelectionUI e)
		{
			if (e.councilState == base.activePlayer)
			{
				base.canvasManager.ShowInfoScreen<ResearchScreenController>();
				int lowestTechFactionWon = this.GetLowestTechFactionWon(base.activePlayer);
				if (lowestTechFactionWon != -1)
				{
					this.SetChangingTechSlot(lowestTechFactionWon);
					this.UpdateSelectTechPanel(base.activePlayer);
					this.selectedTechEntry = string.Empty;
					this.FillOutTechData();
					this.HideTutorials();
					this.archivesOverlayPanel.enabled = false;
					this.selectProjectOverlay.enabled = false;
					this.selectTechOverlay.enabled = true;
				}
			}
		}

		// Token: 0x060055DA RID: 21978 RVA: 0x0027116F File Offset: 0x0026F36F
		private void ProjectUIOptionsUpdated(ProjectUIOptionsChanged e)
		{
			this.UpdateSelectProjectPanel(base.activePlayer, this.changingProjectSlot);
		}

		// Token: 0x060055DB RID: 21979 RVA: 0x00271184 File Offset: 0x0026F384
		private void ResetAllPanels()
		{
			this.mainGridObject.SetActive(true);
			this.primaryResearchPanel.enabled = true;
			this.ShowResearchTutorial();
			this.tabbedPaneManager.Toggle(this.researchTab);
			this.archivesOverlayPanel.enabled = false;
			this.rightButtonOverlayPanel.enabled = false;
			this.selectProjectOverlay.enabled = false;
			this.selectTechOverlay.enabled = false;
			this.UpdateResearchLists(base.activePlayer);
			this.UpdateSelectTechPanel(base.activePlayer);
			this.UpdateEffectsBreakdownScreen();
			this.fullTechTreeCanvas.enabled = false;
			this.fullTechTreeCanvasNP.enabled = true;
			this.selectiveTechTreeCanvas.enabled = false;
			this.ClearSelectiveTechTreeData();
			this.currentTechTreeViewed = ResearchScreenController.techTreeType.techsOnly;
		}

		// Token: 0x060055DC RID: 21980 RVA: 0x0027123F File Offset: 0x0026F43F
		private void UpdateResearchValues(ResearchUpdated e)
		{
			if (this.Visible())
			{
				this.UpdateResearchValues();
			}
		}

		// Token: 0x060055DD RID: 21981 RVA: 0x0027124F File Offset: 0x0026F44F
		private void UpdateResearchValues()
		{
			this.UpdateResearchLists(base.activePlayer);
		}

		// Token: 0x060055DE RID: 21982 RVA: 0x00271260 File Offset: 0x0026F460
		public void UpdateResearchLists(TIFactionState faction)
		{
			int num = 0;
			foreach (ResearchPanelController researchPanelController in this.researchPanelGrid)
			{
				switch (researchPanelController.slot)
				{
				case 0:
				case 1:
				case 2:
				case 3:
					researchPanelController.UpdatePanel(faction);
					break;
				case 4:
					if (faction.OrgProjectAllowed())
					{
						this.orgProjectBackground.gameObject.SetActive(false);
						researchPanelController.UpdatePanel(faction);
						researchPanelController.gameObject.SetActive(true);
					}
					else
					{
						this.orgProjectBackground.gameObject.SetActive(true);
						researchPanelController.gameObject.SetActive(false);
					}
					break;
				case 5:
					if (faction.HabProjectAllowed())
					{
						this.habProjectBackground.gameObject.SetActive(false);
						researchPanelController.UpdatePanel(faction);
						researchPanelController.gameObject.SetActive(true);
					}
					else
					{
						this.habProjectBackground.gameObject.SetActive(true);
						researchPanelController.gameObject.SetActive(false);
					}
					break;
				}
				num++;
			}
		}

		// Token: 0x060055DF RID: 21983 RVA: 0x00271362 File Offset: 0x0026F562
		public void UpdateArchivesPanel()
		{
			this.UpdateArchivesPanel(base.activePlayer);
		}

		// Token: 0x060055E0 RID: 21984 RVA: 0x00271370 File Offset: 0x0026F570
		public void UpdateArchivesPanel(TIFactionState faction)
		{
			List<TITechTemplate> list = TIGlobalResearchState.FinishedTechs().AsEnumerable<TITechTemplate>().Reverse<TITechTemplate>()
				.ToList<TITechTemplate>();
			List<TIProjectTemplate> completedProjectsDistinct = faction.completedProjectsDistinct;
			completedProjectsDistinct.Reverse();
			this.archivedTechsList.SetListSize<CombinedResearchListItemController>(list.Count + completedProjectsDistinct.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.archivedTechsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__132.<>p__0 == null)
					{
						ResearchScreenController.<>o__132.<>p__0 = CallSite<Func<CallSite, object, CombinedResearchListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombinedResearchListItemController), typeof(ResearchScreenController)));
					}
					CombinedResearchListItemController combinedResearchListItemController = ResearchScreenController.<>o__132.<>p__0.Target(ResearchScreenController.<>o__132.<>p__0, enumerator.Current);
					combinedResearchListItemController.Init(this);
					if (num < list.Count)
					{
						combinedResearchListItemController.UpdateTechListItem(list[num]);
					}
					else
					{
						combinedResearchListItemController.UpdateProjectListItem(completedProjectsDistinct[num - list.Count]);
					}
					num++;
				}
			}
			this.FillOutArchiveData();
			this.FillOutProjectData();
			this.FillOutTechData();
		}

		// Token: 0x060055E1 RID: 21985 RVA: 0x00271488 File Offset: 0x0026F688
		public void UpdateArchiveSearch()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			string text = this.archiveSearchInput.text;
			text = TIUtilities.StripDiacriticsFromString(text);
			using (IEnumerator<object> enumerator = this.archivedTechsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__133.<>p__0 == null)
					{
						ResearchScreenController.<>o__133.<>p__0 = CallSite<Func<CallSite, object, CombinedResearchListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CombinedResearchListItemController), typeof(ResearchScreenController)));
					}
					CombinedResearchListItemController combinedResearchListItemController = ResearchScreenController.<>o__133.<>p__0.Target(ResearchScreenController.<>o__133.<>p__0, enumerator.Current);
					if (text.Length < 2 || TIUtilities.StripDiacriticsFromString(combinedResearchListItemController.selectTechButtonText.text.ToLower()).Contains(text.ToLower()))
					{
						combinedResearchListItemController.gameObject.SetActive(true);
					}
					else
					{
						combinedResearchListItemController.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060055E2 RID: 21986 RVA: 0x00271578 File Offset: 0x0026F778
		public void UpdateEffectsSearch()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			string text = this.effectsSearchInput.text;
			text = TIUtilities.StripDiacriticsFromString(text);
			using (IEnumerator<object> enumerator = this.effectsContextList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__134.<>p__0 == null)
					{
						ResearchScreenController.<>o__134.<>p__0 = CallSite<Func<CallSite, object, EffectContextListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EffectContextListItemController), typeof(ResearchScreenController)));
					}
					EffectContextListItemController effectContextListItemController = ResearchScreenController.<>o__134.<>p__0.Target(ResearchScreenController.<>o__134.<>p__0, enumerator.Current);
					if (text.Length < 2 || TIUtilities.StripDiacriticsFromString(effectContextListItemController.selectContextButtonText.text.ToLower()).Contains(text.ToLower()))
					{
						effectContextListItemController.gameObject.SetActive(true);
					}
					else
					{
						effectContextListItemController.gameObject.SetActive(false);
					}
				}
			}
		}

		// Token: 0x060055E3 RID: 21987 RVA: 0x00271668 File Offset: 0x0026F868
		private string CategoryDescriptionAndBonus(TIFactionState faction, TIGenericTechTemplate template)
		{
			StringBuilder stringBuilder = new StringBuilder(template.categoryDescription);
			float num = faction.SumCategoryModifiers(template.techCategory);
			if (num > 0f)
			{
				stringBuilder.Append(Loc.T("UI.Science.CategoryBonus", new object[] { num.ToPercent("P0") }));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060055E4 RID: 21988 RVA: 0x002716C4 File Offset: 0x0026F8C4
		public void FillOutArchiveData()
		{
			if (!string.IsNullOrEmpty(this.selectedArchiveEntry))
			{
				TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(this.selectedArchiveEntry, false);
				if (titechTemplate != null)
				{
					this.archiveHeadline.SetText(titechTemplate.displayName);
					this.archiveTechDetailScrollRect.verticalNormalizedPosition = 1f;
					this.archiveBody.SetText(titechTemplate.GetFullDescription(base.activePlayer, TechBenefitsContext.Archive, null, false));
					this.archiveSummary.SetText(titechTemplate.summary);
					this.archiveCategory.SetText(titechTemplate.categoryString);
					GameControl.assetLoader.LoadAssetForImageAssignment(titechTemplate.GetCategoryIconPath(), this.archiveCategoryIcon);
					GameControl.assetLoader.LoadAssetForImageAssignment(titechTemplate.IconResource, this.archiveTechIcon);
					this.archiveCategoryDescription.SetText(this.CategoryDescriptionAndBonus(base.activePlayer, titechTemplate));
					this.archiveTechIcon.enabled = true;
					this.archiveCategoryIcon.enabled = true;
					return;
				}
				TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(this.selectedArchiveEntry, false);
				if (tiprojectTemplate != null)
				{
					this.archiveHeadline.SetText(tiprojectTemplate.displayName);
					this.archiveCategory.SetText(tiprojectTemplate.categoryString);
					this.archiveSummary.SetText(tiprojectTemplate.summary);
					this.archiveTechDetailScrollRect.verticalNormalizedPosition = 1f;
					this.archiveBody.SetText(tiprojectTemplate.GetFullDescription(base.activePlayer, TechBenefitsContext.Archive, null, false));
					this.archiveCategoryDescription.SetText(this.CategoryDescriptionAndBonus(base.activePlayer, tiprojectTemplate));
					GameControl.assetLoader.LoadAssetForImageAssignment(tiprojectTemplate.IconResource, this.archiveTechIcon);
					GameControl.assetLoader.LoadAssetForImageAssignment(tiprojectTemplate.GetCategoryIconPath(), this.archiveCategoryIcon);
					this.archiveTechIcon.enabled = true;
					this.archiveCategoryIcon.enabled = true;
					return;
				}
			}
			this.archiveHeadline.text = string.Empty;
			this.archiveBody.text = string.Empty;
			this.archiveSummary.SetText(string.Empty);
			this.archiveCategoryDescription.SetText(string.Empty);
			this.archiveCategory.SetText(string.Empty);
			this.archiveTechIcon.enabled = false;
			this.archiveCategoryIcon.enabled = false;
		}

		// Token: 0x060055E5 RID: 21989 RVA: 0x002718E4 File Offset: 0x0026FAE4
		public void UpdateSelectProjectPanel(TIFactionState councilState, int currentSlot)
		{
			List<TIProjectTemplate> list = councilState.SelectableProjects(currentSlot);
			this.availableProjectsList.SetListSize<ProjectsButtonListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.availableProjectsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__137.<>p__0 == null)
					{
						ResearchScreenController.<>o__137.<>p__0 = CallSite<Func<CallSite, object, ProjectsButtonListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ProjectsButtonListItemController), typeof(ResearchScreenController)));
					}
					ProjectsButtonListItemController projectsButtonListItemController = ResearchScreenController.<>o__137.<>p__0.Target(ResearchScreenController.<>o__137.<>p__0, enumerator.Current);
					projectsButtonListItemController.Init(this);
					projectsButtonListItemController.UpdateListItem(list[num], councilState);
					num++;
				}
			}
			num = 0;
			using (IEnumerator<object> enumerator = this.availableProjectsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__137.<>p__1 == null)
					{
						ResearchScreenController.<>o__137.<>p__1 = CallSite<Func<CallSite, object, ProjectsButtonListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ProjectsButtonListItemController), typeof(ResearchScreenController)));
					}
					ResearchScreenController.<>o__137.<>p__1.Target(ResearchScreenController.<>o__137.<>p__1, enumerator.Current).UpdateToggles(list[num++], councilState);
				}
			}
			this.ChangeProjectSelectionSort();
		}

		// Token: 0x060055E6 RID: 21990 RVA: 0x00271A2C File Offset: 0x0026FC2C
		public void SetSelectedProjectEntry(string dataName)
		{
			this.selectedProjectEntry = dataName;
			this.selectedTechPanelTechName.SetText("");
			this.FillOutProjectData();
			this.HighlightSelectedProjectEntry(this.selectedProjectEntry);
		}

		// Token: 0x060055E7 RID: 21991 RVA: 0x00271A58 File Offset: 0x0026FC58
		public void HighlightSelectedProjectEntry(string entryDataName = "")
		{
			bool flag = string.IsNullOrEmpty(entryDataName);
			using (IEnumerator<object> enumerator = this.availableProjectsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__139.<>p__0 == null)
					{
						ResearchScreenController.<>o__139.<>p__0 = CallSite<Func<CallSite, object, ProjectsButtonListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ProjectsButtonListItemController), typeof(ResearchScreenController)));
					}
					ProjectsButtonListItemController projectsButtonListItemController = ResearchScreenController.<>o__139.<>p__0.Target(ResearchScreenController.<>o__139.<>p__0, enumerator.Current);
					projectsButtonListItemController.SetSelected(!flag && entryDataName == projectsButtonListItemController.heldDataName);
				}
			}
		}

		// Token: 0x060055E8 RID: 21992 RVA: 0x00271B00 File Offset: 0x0026FD00
		public void OnClickSetNewLongTermTech()
		{
			if (this.cachedSelectedTech.dataName == base.activePlayer.longtermTechTarget)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			}
			else
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			}
			base.activePlayer.playerControl.StartAction(new SetLongTermTechTargetAction(base.activePlayer, (this.cachedSelectedTech.dataName == base.activePlayer.longtermTechTarget) ? "" : this.cachedSelectedTech.dataName));
			this.UpdateLongTermTechTargetButtonText();
			this.RefreshTechTreeStatuses();
		}

		// Token: 0x060055E9 RID: 21993 RVA: 0x00271B9A File Offset: 0x0026FD9A
		public void OnChangeProjectAscendSort()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.projectSortAscend = this.projectSortAscendToggle.isOn;
			TIGlobalValuesState.GlobalValues.projectSortAscend = this.projectSortAscend;
			this.ChangeProjectSelectionSort();
		}

		// Token: 0x060055EA RID: 21994 RVA: 0x00271BCF File Offset: 0x0026FDCF
		public void OnChangeProjectObsoleteSort()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.projectSortShowObsolete = this.projectSortObsoleteToggle.isOn;
			TIGlobalValuesState.GlobalValues.projectSortShowObsolete = this.projectSortShowObsolete;
			this.ChangeProjectSelectionSort();
		}

		// Token: 0x060055EB RID: 21995 RVA: 0x00271C04 File Offset: 0x0026FE04
		public void OnProjectObsoleteToggle(bool isOn, string projectDataName)
		{
			base.activePlayer.playerControl.StartAction(new HideProjectAction(base.activePlayer, projectDataName, isOn));
			if (!this.projectSortShowObsolete)
			{
				this.ChangeProjectSelectionSort();
			}
		}

		// Token: 0x060055EC RID: 21996 RVA: 0x00271C31 File Offset: 0x0026FE31
		public void OnProjectFavoriteToggle(bool isOn, string projectDataName)
		{
			base.activePlayer.playerControl.StartAction(new FavorProjectAction(base.activePlayer, projectDataName, isOn));
			this.ChangeProjectSelectionSort();
		}

		// Token: 0x060055ED RID: 21997 RVA: 0x00271C58 File Offset: 0x0026FE58
		public void ChangeProjectSelectionSort()
		{
			if (!this.initProjectSortSettings)
			{
				this.projectSortObsoleteToggle.SetIsOnWithoutNotify(TIGlobalValuesState.GlobalValues.projectSortShowObsolete);
				this.projectSortShowObsolete = TIGlobalValuesState.GlobalValues.projectSortShowObsolete;
				this.sortProjectDropdown.SetValueWithoutNotify(TIGlobalValuesState.GlobalValues.currentProjectSort);
				this.projectSortAscendToggle.SetIsOnWithoutNotify(TIGlobalValuesState.GlobalValues.projectSortAscend);
				this.projectSortAscend = TIGlobalValuesState.GlobalValues.projectSortAscend;
				this.initProjectSortSettings = true;
			}
			int value = this.sortProjectDropdown.value;
			TIGlobalValuesState.GlobalValues.currentProjectSort = value;
			this.currentProjectSort = (ResearchScreenController.SortProjectDataBy)value;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.availableProjectsList.transform.childCount; i++)
			{
				list.Add(this.availableProjectsList.transform.GetChild(i).gameObject);
			}
			switch (this.currentProjectSort)
			{
			case ResearchScreenController.SortProjectDataBy.Name:
				if (!this.projectSortAscend)
				{
					list = list.OrderByDescending<GameObject, string>((GameObject o) => o.GetComponent<ProjectsButtonListItemController>().projectTemplate.displayName).ToList<GameObject>();
				}
				if (this.projectSortAscend)
				{
					list = list.OrderBy<GameObject, string>((GameObject o) => o.GetComponent<ProjectsButtonListItemController>().projectTemplate.displayName).ToList<GameObject>();
				}
				break;
			case ResearchScreenController.SortProjectDataBy.Category:
				if (!this.projectSortAscend)
				{
					list = (from o in list
						orderby base.activePlayer.GetProjectProgressByTemplate(o.GetComponent<ProjectsButtonListItemController>().projectTemplate).accumulatedResearch descending, o.GetComponent<ProjectsButtonListItemController>().categorySortWeight descending
						select o).ToList<GameObject>();
				}
				if (this.projectSortAscend)
				{
					list = (from o in list
						orderby base.activePlayer.GetProjectProgressByTemplate(o.GetComponent<ProjectsButtonListItemController>().projectTemplate).accumulatedResearch descending, o.GetComponent<ProjectsButtonListItemController>().categorySortWeight
						select o).ToList<GameObject>();
				}
				break;
			case ResearchScreenController.SortProjectDataBy.Cost:
				if (!this.projectSortAscend)
				{
					list = (from o in list
						orderby base.activePlayer.GetProjectProgressByTemplate(o.GetComponent<ProjectsButtonListItemController>().projectTemplate).accumulatedResearch descending, o.GetComponent<ProjectsButtonListItemController>().projectTemplate.GetResearchCost(base.activePlayer) descending
						select o).ToList<GameObject>();
				}
				if (this.projectSortAscend)
				{
					list = (from o in list
						orderby base.activePlayer.GetProjectProgressByTemplate(o.GetComponent<ProjectsButtonListItemController>().projectTemplate).accumulatedResearch descending, o.GetComponent<ProjectsButtonListItemController>().projectTemplate.GetResearchCost(base.activePlayer)
						select o).ToList<GameObject>();
				}
				break;
			}
			list = list.OrderByDescending<GameObject, bool>((GameObject o) => o.GetComponent<ProjectsButtonListItemController>().favoriteToggle.isOn).ToList<GameObject>();
			this.UpdateSortedProjectList(list);
			this.lastProjectSort = value;
		}

		// Token: 0x060055EE RID: 21998 RVA: 0x00271EE4 File Offset: 0x002700E4
		public void UpdateSortedProjectList(List<GameObject> projectList)
		{
			for (int i = 0; i < projectList.Count; i++)
			{
				if (!this.projectSortShowObsolete)
				{
					if (base.activePlayer.hiddenProjects.Contains(projectList[i].GetComponent<ProjectsButtonListItemController>().projectTemplate.dataName))
					{
						projectList[i].SetActive(false);
					}
					else
					{
						projectList[i].SetActive(true);
					}
				}
				else
				{
					projectList[i].SetActive(true);
				}
				int num = i;
				projectList[i].transform.SetSiblingIndex(num);
			}
			for (int j = 0; j < projectList.Count; j++)
			{
				if (projectList[j].GetComponent<ProjectsButtonListItemController>().projectTemplate.FulfillsObjective(base.activePlayer, true) != null)
				{
					projectList[j].transform.SetSiblingIndex(0);
				}
			}
		}

		// Token: 0x060055EF RID: 21999 RVA: 0x00271FB3 File Offset: 0x002701B3
		public void SetChangingProjectSlot(int slot)
		{
			this.changingProjectSlot = slot;
		}

		// Token: 0x060055F0 RID: 22000 RVA: 0x00271FBC File Offset: 0x002701BC
		public void FillOutProjectData()
		{
			if (this.selectedProjectEntry == string.Empty || this.selectedProjectEntry == null)
			{
				this.availableProjectHeadline.SetText(string.Empty);
				this.selectProjectTextDetailScrollRect.verticalNormalizedPosition = 1f;
				this.availableProjectBody.SetText(string.Empty);
				this.availableProjectImage.enabled = false;
				this.availableProjectTechCategoryIcon.enabled = false;
				this.availableProjectTechCategoryText.SetText(string.Empty);
				this.availableProjectTechCategoryName.SetText(string.Empty);
				this.availableProjectSummary.SetText(string.Empty);
				this.selectProjectTechTreeButton.interactable = false;
				return;
			}
			TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(this.selectedProjectEntry, false);
			this.availableProjectHeadline.SetText(tiprojectTemplate.displayName);
			this.selectProjectTextDetailScrollRect.verticalNormalizedPosition = 1f;
			this.availableProjectBody.SetText(tiprojectTemplate.GetFullDescription(base.activePlayer, TechBenefitsContext.Prospective, null, false));
			this.availableProjectSummary.SetText(tiprojectTemplate.summary);
			GameControl.assetLoader.LoadAssetForImageAssignment(tiprojectTemplate.IconResource, this.availableProjectImage);
			this.availableProjectImage.enabled = true;
			this.availableProjectTechCategoryIcon.enabled = true;
			GameControl.assetLoader.LoadAssetForImageAssignment(tiprojectTemplate.GetCategoryIconPath(), this.availableProjectTechCategoryIcon);
			this.availableProjectTechCategoryName.SetText(tiprojectTemplate.categoryString);
			this.availableProjectTechCategoryText.SetText(this.CategoryDescriptionAndBonus(base.activePlayer, tiprojectTemplate));
			this.selectProjectTechTreeButton.interactable = true;
		}

		// Token: 0x060055F1 RID: 22001 RVA: 0x00272140 File Offset: 0x00270340
		public void UpdateSelectTechPanel(TIFactionState councilState)
		{
			List<TITechTemplate> list = TIGlobalResearchState.AvailableTechs();
			this.availableTechsList.SetListSize<TechsButtonListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.availableTechsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__149.<>p__0 == null)
					{
						ResearchScreenController.<>o__149.<>p__0 = CallSite<Func<CallSite, object, TechsButtonListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TechsButtonListItemController), typeof(ResearchScreenController)));
					}
					TechsButtonListItemController techsButtonListItemController = ResearchScreenController.<>o__149.<>p__0.Target(ResearchScreenController.<>o__149.<>p__0, enumerator.Current);
					techsButtonListItemController.Init(this);
					techsButtonListItemController.UpdateListItem(list[num++]);
				}
			}
			base.StartCoroutine(this.ChangeTechSelectionSort());
		}

		// Token: 0x060055F2 RID: 22002 RVA: 0x00272204 File Offset: 0x00270404
		public void SetSelectedTechEntry(string dataName)
		{
			this.selectedTechEntry = dataName;
			this.FillOutTechData();
			this.HighlightSelectedTechEntry(this.selectedTechEntry);
		}

		// Token: 0x060055F3 RID: 22003 RVA: 0x00272220 File Offset: 0x00270420
		public void HighlightSelectedTechEntry(string entryDataName = "")
		{
			bool flag = string.IsNullOrEmpty(entryDataName);
			using (IEnumerator<object> enumerator = this.availableTechsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__151.<>p__0 == null)
					{
						ResearchScreenController.<>o__151.<>p__0 = CallSite<Func<CallSite, object, TechsButtonListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TechsButtonListItemController), typeof(ResearchScreenController)));
					}
					TechsButtonListItemController techsButtonListItemController = ResearchScreenController.<>o__151.<>p__0.Target(ResearchScreenController.<>o__151.<>p__0, enumerator.Current);
					techsButtonListItemController.SetSelected(!flag && entryDataName == techsButtonListItemController.heldDataName);
				}
			}
		}

		// Token: 0x060055F4 RID: 22004 RVA: 0x002722C8 File Offset: 0x002704C8
		public void OnChangeTechAscendSort()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.techSortAscend = this.techSortAscendToggle.isOn;
			TIGlobalValuesState.GlobalValues.techSortAscend = this.techSortAscend;
			base.StartCoroutine(this.ChangeTechSelectionSort());
		}

		// Token: 0x060055F5 RID: 22005 RVA: 0x00272304 File Offset: 0x00270504
		public void OnClickChangeTechSelectionSort()
		{
			base.StartCoroutine(this.ChangeTechSelectionSort());
		}

		// Token: 0x060055F6 RID: 22006 RVA: 0x00272313 File Offset: 0x00270513
		public IEnumerator ChangeTechSelectionSort()
		{
			yield return null;
			if (!this.initTechSortSettings)
			{
				this.sortTechDropdown.SetValueWithoutNotify(TIGlobalValuesState.GlobalValues.currentTechSort);
				this.techSortAscendToggle.SetIsOnWithoutNotify(TIGlobalValuesState.GlobalValues.techSortAscend);
				this.techSortAscend = TIGlobalValuesState.GlobalValues.techSortAscend;
				this.initTechSortSettings = true;
			}
			int value = this.sortTechDropdown.value;
			TIGlobalValuesState.GlobalValues.currentTechSort = value;
			this.currentTechSort = (ResearchScreenController.SortTechDataBy)value;
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.availableTechsList.transform.childCount; i++)
			{
				list.Add(this.availableTechsList.transform.GetChild(i).gameObject);
			}
			switch (this.currentTechSort)
			{
			case ResearchScreenController.SortTechDataBy.Name:
				if (!this.techSortAscend)
				{
					list = list.OrderByDescending<GameObject, string>((GameObject o) => o.GetComponent<TechsButtonListItemController>().techTemplate.displayName).ToList<GameObject>();
				}
				if (this.techSortAscend)
				{
					list = list.OrderBy<GameObject, string>((GameObject o) => o.GetComponent<TechsButtonListItemController>().techTemplate.displayName).ToList<GameObject>();
				}
				break;
			case ResearchScreenController.SortTechDataBy.Category:
				if (!this.techSortAscend)
				{
					list = list.OrderByDescending<GameObject, float>((GameObject o) => o.GetComponent<TechsButtonListItemController>().categorySortWeight).ToList<GameObject>();
				}
				if (this.techSortAscend)
				{
					list = list.OrderBy<GameObject, float>((GameObject o) => o.GetComponent<TechsButtonListItemController>().categorySortWeight).ToList<GameObject>();
				}
				break;
			case ResearchScreenController.SortTechDataBy.Cost:
				if (!this.techSortAscend)
				{
					list = list.OrderByDescending<GameObject, float>((GameObject o) => o.GetComponent<TechsButtonListItemController>().techTemplate.GetResearchCost(base.activePlayer)).ToList<GameObject>();
				}
				if (this.techSortAscend)
				{
					list = list.OrderBy<GameObject, float>((GameObject o) => o.GetComponent<TechsButtonListItemController>().techTemplate.GetResearchCost(base.activePlayer)).ToList<GameObject>();
				}
				break;
			}
			this.UpdateSortedTechList(list);
			this.lastTechSort = value;
			yield break;
		}

		// Token: 0x060055F7 RID: 22007 RVA: 0x00272324 File Offset: 0x00270524
		public void UpdateSortedTechList(List<GameObject> techList)
		{
			for (int i = 0; i < techList.Count; i++)
			{
				techList[i].transform.SetSiblingIndex(i);
			}
		}

		// Token: 0x060055F8 RID: 22008 RVA: 0x00272354 File Offset: 0x00270554
		public void SetChangingTechSlot(int slot)
		{
			this.selectTechSlot = slot;
		}

		// Token: 0x060055F9 RID: 22009 RVA: 0x00272360 File Offset: 0x00270560
		public void FillOutTechData()
		{
			if (string.IsNullOrEmpty(this.selectedTechEntry))
			{
				this.availableTechHeadline.SetText(string.Empty);
				this.selectTechTextDetailScrollRect.verticalNormalizedPosition = 1f;
				this.availableTechBody.SetText(string.Empty);
				this.availableTechTechCategoryName.SetText(string.Empty);
				this.availableTechTechCategoryText.SetText(string.Empty);
				this.availableTechSummary.SetText(string.Empty);
				this.availableTechTechCategoryIcon.enabled = false;
				this.availableTechImage.enabled = false;
				this.selectTechTechTreeButton.interactable = false;
				return;
			}
			TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(this.selectedTechEntry, false);
			this.availableTechHeadline.SetText(titechTemplate.displayName);
			this.selectTechTextDetailScrollRect.verticalNormalizedPosition = 1f;
			this.availableTechBody.SetText(titechTemplate.GetFullDescription(base.activePlayer, TechBenefitsContext.Prospective, null, false));
			this.availableTechSummary.SetText(titechTemplate.summary);
			GameControl.assetLoader.LoadAssetForImageAssignment(titechTemplate.IconResource, this.availableTechImage);
			this.availableTechTechCategoryName.SetText(titechTemplate.categoryString);
			GameControl.assetLoader.LoadAssetForImageAssignment(titechTemplate.GetCategoryIconPath(), this.availableTechTechCategoryIcon);
			this.availableTechTechCategoryText.SetText(this.CategoryDescriptionAndBonus(base.activePlayer, titechTemplate));
			this.availableTechImage.enabled = true;
			this.availableTechTechCategoryIcon.enabled = true;
			this.selectTechTechTreeButton.interactable = true;
		}

		// Token: 0x060055FA RID: 22010 RVA: 0x002724D4 File Offset: 0x002706D4
		private static string TechName(TIGenericTechTemplate genericTechTemplate)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(genericTechTemplate.displayName);
			return stringBuilder.ToString();
		}

		// Token: 0x060055FB RID: 22011 RVA: 0x002724F0 File Offset: 0x002706F0
		private static string TechStatusString(TIGenericTechTemplate genericTechTemplate, TIFactionState faction)
		{
			TIProjectTemplate tiprojectTemplate = genericTechTemplate as TIProjectTemplate;
			if (tiprojectTemplate != null)
			{
				if (faction.completedProjects.Contains(tiprojectTemplate))
				{
					if (tiprojectTemplate.repeatable)
					{
						return Loc.T("UI.Science.CompletedRepeat");
					}
					return Loc.T("UI.Science.Completed");
				}
				else
				{
					float projectProgressValueByTemplate = faction.GetProjectProgressValueByTemplate(tiprojectTemplate);
					if (projectProgressValueByTemplate > 0f)
					{
						return Loc.T("UI.Science.InProgress", new object[]
						{
							projectProgressValueByTemplate.ToString("N0"),
							tiprojectTemplate.GetResearchCost(faction).ToString()
						});
					}
					return Loc.T("UI.Science.NotStarted", new object[] { tiprojectTemplate.GetResearchCost(faction).ToString() });
				}
			}
			else
			{
				TITechTemplate titechTemplate = genericTechTemplate as TITechTemplate;
				if (TIGlobalResearchState.TechFinished(titechTemplate))
				{
					return Loc.T("UI.Science.Completed");
				}
				float accumulatedResearchByTech = TIGlobalResearchState.GetAccumulatedResearchByTech(titechTemplate);
				if (accumulatedResearchByTech > 0f)
				{
					return Loc.T("UI.Science.InProgress", new object[]
					{
						accumulatedResearchByTech.ToString("N0"),
						titechTemplate.GetResearchCost(faction).ToString()
					});
				}
				return Loc.T("UI.Science.NotStarted", new object[] { titechTemplate.GetResearchCost(faction).ToString() });
			}
		}

		// Token: 0x060055FC RID: 22012 RVA: 0x0027261C File Offset: 0x0027081C
		private static Color TechStatusColor(TIGenericTechTemplate genericTechTemplate, TIFactionState faction)
		{
			TIProjectTemplate tiprojectTemplate = genericTechTemplate as TIProjectTemplate;
			if (tiprojectTemplate != null)
			{
				if (faction.completedProjects.Contains(tiprojectTemplate))
				{
					if (tiprojectTemplate.repeatable)
					{
						return ResearchScreenController.techStatusColor[0];
					}
					return ResearchScreenController.techStatusColor[8];
				}
				else
				{
					if (faction.GetProjectProgressValueByTemplate(tiprojectTemplate) > 0f)
					{
						return ResearchScreenController.techStatusColor[1];
					}
					if (faction.availableProjects.Contains(tiprojectTemplate))
					{
						return ResearchScreenController.techStatusColor[2];
					}
					if (TIGlobalValuesState.GlobalValues.scenarioCustomizations.showTriggeredProjects)
					{
						if (faction.TriggeredProjects.Contains(tiprojectTemplate))
						{
							return ResearchScreenController.techStatusColor[9];
						}
						if (faction.missedProjects.Contains(tiprojectTemplate.dataName))
						{
							return ResearchScreenController.techStatusColor[3];
						}
					}
					if (faction.GetProjectUnlockChance(tiprojectTemplate, faction.TechContributionBonus(tiprojectTemplate)) >= 100f)
					{
						return ResearchScreenController.techStatusColor[10];
					}
					return ResearchScreenController.techStatusColor[11];
				}
			}
			else
			{
				TITechTemplate titechTemplate = genericTechTemplate as TITechTemplate;
				if (TIGlobalResearchState.TechFinished(titechTemplate))
				{
					return ResearchScreenController.techStatusColor[4];
				}
				if (TIGlobalResearchState.CurrentResearchingTechs.Contains(titechTemplate))
				{
					return ResearchScreenController.techStatusColor[5];
				}
				if (TIGlobalResearchState.AvailableTechs().Contains(titechTemplate))
				{
					return ResearchScreenController.techStatusColor[6];
				}
				return ResearchScreenController.techStatusColor[7];
			}
		}

		// Token: 0x060055FD RID: 22013 RVA: 0x002727A8 File Offset: 0x002709A8
		public static int GetTechStatusAppearanceIndex(TIGenericTechTemplate genericTechTemplate, TIFactionState faction)
		{
			TIProjectTemplate tiprojectTemplate = genericTechTemplate as TIProjectTemplate;
			if (tiprojectTemplate != null)
			{
				if (faction.completedProjects.Contains(tiprojectTemplate))
				{
					if (tiprojectTemplate.repeatable)
					{
						return 0;
					}
					return 8;
				}
				else
				{
					if (faction.GetProjectProgressValueByTemplate(tiprojectTemplate) > 0f)
					{
						return 1;
					}
					if (faction.availableProjects.Contains(tiprojectTemplate))
					{
						return 2;
					}
					if (TIGlobalValuesState.GlobalValues.scenarioCustomizations.showTriggeredProjects)
					{
						if (faction.TriggeredProjects.Contains(tiprojectTemplate))
						{
							return 9;
						}
						if (faction.missedProjects.Contains(tiprojectTemplate.dataName))
						{
							return 3;
						}
					}
					if (faction.GetProjectUnlockChance(tiprojectTemplate, faction.TechContributionBonus(tiprojectTemplate)) >= 100f)
					{
						return 10;
					}
					return 11;
				}
			}
			else
			{
				TITechTemplate titechTemplate = genericTechTemplate as TITechTemplate;
				if (TIGlobalResearchState.TechFinished(titechTemplate))
				{
					return 4;
				}
				if (TIGlobalResearchState.CurrentResearchingTechs.Contains(titechTemplate))
				{
					return 5;
				}
				if (TIGlobalResearchState.AvailableTechs().Contains(titechTemplate))
				{
					return 6;
				}
				return 7;
			}
		}

		// Token: 0x060055FE RID: 22014 RVA: 0x00272880 File Offset: 0x00270A80
		public void OnTechinTreeClicked(int value)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			switch (value)
			{
			case 0:
				this.DisplayTechTree(this.selectedTech.TechPrereqs[0]);
				return;
			case 1:
				this.DisplayTechTree(this.selectedTech.AltTechPrereq0);
				return;
			case 2:
				this.DisplayTechTree(this.selectedTech.TechPrereqs[1]);
				return;
			case 3:
				this.DisplayTechTree(this.selectedTech.AltTechPrereq1);
				return;
			case 4:
				this.DisplayTechTree(this.selectedTech.TechPrereqs[2]);
				return;
			case 5:
				this.DisplayTechTree(this.selectedTech.TechPrereqs[3]);
				return;
			default:
				return;
			}
		}

		// Token: 0x060055FF RID: 22015 RVA: 0x0027293B File Offset: 0x00270B3B
		public void OnTechSearchToggle(bool projects)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.UpdateTechSearch(projects);
		}

		// Token: 0x06005600 RID: 22016 RVA: 0x00272950 File Offset: 0x00270B50
		public void UpdateTechSearch(bool projects = false)
		{
			List<ChildTechGridItemController> list = new List<ChildTechGridItemController>();
			if (!projects)
			{
				string text = this.searchFieldTechs.text.ToLower();
				if (this.searchFieldTechs.text.Length > 1)
				{
					for (int i = 0; i < this.fullTechTreeGridManagerNP.transform.childCount; i++)
					{
						ChildTechGridItemController component = this.fullTechTreeGridManagerNP.transform.GetChild(i).GetComponent<ChildTechGridItemController>();
						if (component != null && component.gameObject.activeSelf && (component.tech.displayName.ToLower().Contains(text) || (this.fullSearchTechs.isOn && component.toolTipString.ToLower().Contains(text))))
						{
							list.Add(component);
						}
					}
					this.searchResultsTechs.SetListSize<TechSearchListItemController>(list.Count, false, false);
					this.searchPanelTechs.sizeDelta = new Vector2(this.searchPanelTechs.sizeDelta.x, Mathf.Min(70f + (float)list.Count * 25f, 200f));
					int num = 0;
					using (IEnumerator<object> enumerator = this.searchResultsTechs.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (ResearchScreenController.<>o__304.<>p__0 == null)
							{
								ResearchScreenController.<>o__304.<>p__0 = CallSite<Func<CallSite, object, TechSearchListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TechSearchListItemController), typeof(ResearchScreenController)));
							}
							ResearchScreenController.<>o__304.<>p__0.Target(ResearchScreenController.<>o__304.<>p__0, enumerator.Current).UpdateItem(this, list[num++]);
						}
						goto IL_01C6;
					}
				}
				this.searchResultsTechs.SetListSize<TechSearchListItemController>(1, true, false);
				this.searchPanelTechs.sizeDelta = new Vector2(this.searchPanelTechs.sizeDelta.x, 70f);
			}
			IL_01C6:
			if (projects)
			{
				string text2 = this.searchFieldProjects.text.ToLower();
				if (this.searchFieldProjects.text.Length > 1)
				{
					using (IEnumerator<object> enumerator = this.FullTechTreeGridManager.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (ResearchScreenController.<>o__304.<>p__1 == null)
							{
								ResearchScreenController.<>o__304.<>p__1 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
							}
							ChildTechGridItemController childTechGridItemController = ResearchScreenController.<>o__304.<>p__1.Target(ResearchScreenController.<>o__304.<>p__1, enumerator.Current);
							if (childTechGridItemController.gameObject.activeSelf && (childTechGridItemController.tech.displayName.ToLower().Contains(text2) || (this.fullSearchProjects.isOn && childTechGridItemController.toolTipString.ToLower().Contains(text2))))
							{
								list.Add(childTechGridItemController);
							}
						}
					}
					this.searchResultsProjects.SetListSize<TechSearchListItemController>(list.Count, false, false);
					this.searchPanelProjects.sizeDelta = new Vector2(this.searchPanelProjects.sizeDelta.x, Mathf.Min(70f + (float)list.Count * 25f, 200f));
					int num2 = 0;
					using (IEnumerator<object> enumerator = this.searchResultsProjects.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (ResearchScreenController.<>o__304.<>p__2 == null)
							{
								ResearchScreenController.<>o__304.<>p__2 = CallSite<Func<CallSite, object, TechSearchListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(TechSearchListItemController), typeof(ResearchScreenController)));
							}
							ResearchScreenController.<>o__304.<>p__2.Target(ResearchScreenController.<>o__304.<>p__2, enumerator.Current).UpdateItem(this, list[num2++]);
						}
						return;
					}
				}
				this.searchResultsProjects.SetListSize<TechSearchListItemController>(1, true, false);
				this.searchPanelProjects.sizeDelta = new Vector2(this.searchPanelProjects.sizeDelta.x, 70f);
			}
		}

		// Token: 0x06005601 RID: 22017 RVA: 0x00272D48 File Offset: 0x00270F48
		public void GotoSearchItem(ChildTechGridItemController techItem, ResearchScreenController.techTreeType treeType)
		{
			techItem.SelectFullTechItem(false);
			base.StartCoroutine(this.LerpTechTreeToItem(techItem, treeType));
		}

		// Token: 0x06005602 RID: 22018 RVA: 0x00272D60 File Offset: 0x00270F60
		public IEnumerator GotoSearchItemNextFrame(ChildTechGridItemController techItem, ResearchScreenController.techTreeType treeType)
		{
			yield return null;
			if (techItem != null)
			{
				this.GotoSearchItem(techItem, treeType);
			}
			yield break;
		}

		// Token: 0x06005603 RID: 22019 RVA: 0x00272D7D File Offset: 0x00270F7D
		public IEnumerator LerpTechTreeToItem(ChildTechGridItemController techItem, ResearchScreenController.techTreeType treeType)
		{
			this.moving = true;
			Vector3 targetPos = Vector3.zero;
			RectTransform targetContent = null;
			if (treeType == ResearchScreenController.techTreeType.techsOnly)
			{
				targetPos = new Vector3((techItem.transform.localPosition.x * this.fullTechTreeContentNP.localScale.x - this.primaryResearchPanel.transform.GetComponent<RectTransform>().rect.width / 2f) * -1f, (techItem.transform.localPosition.y * this.fullTechTreeContentNP.localScale.y + this.primaryResearchPanel.transform.GetComponent<RectTransform>().rect.height / 2f) * -1f, 0f);
				targetContent = this.fullTechTreeContentNP;
			}
			else if (treeType == ResearchScreenController.techTreeType.fullTree)
			{
				targetPos = new Vector3((techItem.transform.localPosition.x * this.fullTechTreeContent.localScale.x - this.primaryResearchPanel.transform.GetComponent<RectTransform>().rect.width / 2f) * -1f, (techItem.transform.localPosition.y * this.fullTechTreeContent.localScale.y + this.primaryResearchPanel.transform.GetComponent<RectTransform>().rect.height / 2f) * -1f, 0f);
				targetContent = this.fullTechTreeContent;
			}
			else if (treeType == ResearchScreenController.techTreeType.selectiveTree)
			{
				targetPos = new Vector3((techItem.transform.localPosition.x * this.selectiveTechTreeContent.localScale.x - this.primaryResearchPanel.transform.GetComponent<RectTransform>().rect.width / 2f) * -1f, (techItem.transform.localPosition.y * this.selectiveTechTreeContent.localScale.y + this.primaryResearchPanel.transform.GetComponent<RectTransform>().rect.height / 2f) * -1f, 0f);
				targetContent = this.selectiveTechTreeContent;
			}
			Vector3 startPos = targetContent.localPosition;
			this.lerpFrames = Mathf.Clamp((int)Vector3.Distance(startPos, targetPos) / 75, 5, 30);
			int num2;
			for (int i = 1; i < this.lerpFrames + 1; i = num2 + 1)
			{
				float num = (float)i / (float)this.lerpFrames;
				targetContent.localPosition = Vector3.Lerp(startPos, targetPos, num);
				yield return new WaitForSeconds(0.016f);
				num2 = i;
			}
			this.moving = false;
			yield break;
		}

		// Token: 0x06005604 RID: 22020 RVA: 0x00272D9C File Offset: 0x00270F9C
		public static string TechTreeTooltip(TIFactionState faction, TIGenericTechTemplate tech, bool simple)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(tech.displayName);
			if (tech.isProject())
			{
				TIProjectTemplate ref_project = tech.ref_project;
				if (faction.completedProjects.Contains(ref_project))
				{
					stringBuilder.AppendLine(Loc.T("UI.Science.ProjectStatus1"));
				}
				else if (faction.GetProjectProgressValueByTemplate(ref_project) > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Science.ProjectStatus2"));
				}
				else if (faction.availableProjects.Contains(ref_project))
				{
					stringBuilder.AppendLine(Loc.T("UI.Science.ProjectStatus3"));
				}
				else
				{
					bool flag = false;
					if (TIGlobalValuesState.GlobalValues.scenarioCustomizations.showTriggeredProjects)
					{
						if (faction.TriggeredProjects.Contains(ref_project))
						{
							stringBuilder.AppendLine(Loc.T("UI.Science.ProjectStatus4"));
							flag = true;
						}
						else if (faction.missedProjects.Contains(ref_project.dataName))
						{
							stringBuilder.AppendLine(Loc.T("UI.Science.ProjectStatus5"));
							flag = true;
						}
					}
					if (!flag)
					{
						string unlockChanceString = TIGenericTechTemplate.GetUnlockChanceString(ref_project, faction);
						if (!string.IsNullOrEmpty(unlockChanceString))
						{
							stringBuilder.AppendLine(Loc.T("UI.Science.ProjectStatus6", new object[] { unlockChanceString }));
						}
					}
				}
			}
			if (simple || TIGlobalResearchState.UseHarshTechTree)
			{
				stringBuilder.AppendLine(tech.BenefitsDescription(faction, TechBenefitsContext.Prospective, null));
			}
			else
			{
				stringBuilder.AppendLine(tech.GetFullDescription(faction, TechBenefitsContext.Prospective, null, true));
			}
			return stringBuilder.ToString().TrimStart(new char[] { '\r', '\n' }).TrimEnd(new char[] { '\r', '\n' });
		}

		// Token: 0x06005605 RID: 22021 RVA: 0x00272F24 File Offset: 0x00271124
		public static void ShowTech(TIFactionState faction, TIGenericTechTemplate tech, GameObject panel, TMP_Text techName, TMP_Text techStatus, Image icon)
		{
			ChildTechGridItemController component = panel.GetComponent<ChildTechGridItemController>();
			if (ResearchScreenController.fullTechTreeOn)
			{
				if (!component.hidden && panel.activeSelf)
				{
					panel.SetActive(true);
				}
				else if (component.hidden && !panel.activeSelf)
				{
					panel.SetActive(false);
				}
				if (!component.imageLoaded)
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(tech.IconResource, icon);
					component.imageLoaded = true;
				}
			}
			techName.SetText(ResearchScreenController.TechName(tech));
			techStatus.SetText(ResearchScreenController.TechStatusString(tech, faction));
			string ttString = faction.GetCachedTechTooltipString(tech);
			component.toolTipString = ttString;
			component.techTooltip.SetDelegate("BodyText", () => ttString);
			if (!ResearchScreenController.fullTechTreeOn)
			{
				foreach (Image image in panel.GetComponentsInChildren<Image>())
				{
					if (image != icon)
					{
						image.color = ResearchScreenController.techStatusColor[ResearchScreenController.GetTechStatusAppearanceIndex(tech, faction)];
					}
				}
				return;
			}
			panel.GetComponent<Image>().color = ResearchScreenController.techStatusColor[ResearchScreenController.GetTechStatusAppearanceIndex(tech, faction)];
		}

		// Token: 0x06005606 RID: 22022 RVA: 0x00273050 File Offset: 0x00271250
		public void DisplayTechTree(TIGenericTechTemplate genericTech)
		{
			if (!this.isFullTechTreeInit && this.usingFullTechTree)
			{
				Log.Time("<color=#00cc00>LoadTime:</color> Initialize Full Tech Tree", delegate
				{
					this.InitializeFullTechTree(false, "", false, true);
				}, true, true);
			}
			if (this.usingFullTechTree)
			{
				Log.Time("<color=#00cc00>LoadTime:</color> Show no project Tech Tree", delegate
				{
					this.ShowNoProjectTechTree();
				}, true, true);
			}
			this.tabbedPaneManager.Toggle(this.techTreeTab);
			if (!this.usingFullTechTree)
			{
				this.selectedTech = genericTech;
				ResearchScreenController.ShowTech(base.activePlayer, genericTech, this.selectedTechPanel, this.selectedTechName, this.selectedTechStatus, this.selectedTechIcon);
				this.selectedTechDetail.SetText(genericTech.summary);
				List<TIGenericTechTemplate> techPrereqs = genericTech.TechPrereqs;
				this.prereqArrow.enabled = false;
				this.childArrow.enabled = false;
				if (techPrereqs.Count > 0 && techPrereqs[0] != null)
				{
					this.prereqArrow.enabled = true;
					ResearchScreenController.ShowTech(base.activePlayer, techPrereqs[0], this.prereq1MasterPanel, this.prereq1TechName, this.prereq1Status, this.prereq1Icon);
					if (genericTech.AltTechPrereq0 != null)
					{
						this.orPanel.SetActive(true);
						ResearchScreenController.ShowTech(base.activePlayer, genericTech.AltTechPrereq0, this.altPrereq1Panel, this.altPrereq1TechName, this.altPrereq1Status, this.altPrereq1Icon);
					}
					else
					{
						this.orPanel.SetActive(false);
						this.altPrereq1Panel.SetActive(false);
					}
					if (techPrereqs.Count > 1 && techPrereqs[1] != null)
					{
						ResearchScreenController.ShowTech(base.activePlayer, techPrereqs[1], this.prereq2MasterPanel, this.prereq2TechName, this.prereq2Status, this.prereq2Icon);
						if (techPrereqs.Count > 2 && techPrereqs[2] != null)
						{
							ResearchScreenController.ShowTech(base.activePlayer, techPrereqs[2], this.prereq3MasterPanel, this.prereq3TechName, this.prereq3Status, this.prereq3Icon);
							if (techPrereqs.Count > 3 && techPrereqs[3] != null)
							{
								ResearchScreenController.ShowTech(base.activePlayer, techPrereqs[3], this.prereq4MasterPanel, this.prereq4TechName, this.prereq4Status, this.prereq4Icon);
							}
							else
							{
								this.prereq4MasterPanel.SetActive(false);
							}
						}
						else
						{
							this.prereq3MasterPanel.SetActive(false);
							this.prereq4MasterPanel.SetActive(false);
						}
					}
					else
					{
						this.prereq2MasterPanel.SetActive(false);
						this.prereq3MasterPanel.SetActive(false);
						this.prereq4MasterPanel.SetActive(false);
					}
				}
				else
				{
					this.prereq1MasterPanel.SetActive(false);
					this.prereq2MasterPanel.SetActive(false);
					this.prereq3MasterPanel.SetActive(false);
					this.prereq4MasterPanel.SetActive(false);
					this.orPanel.SetActive(false);
					this.altPrereq1Panel.SetActive(false);
				}
				TIProjectTemplate tiprojectTemplate = genericTech as TIProjectTemplate;
				if (tiprojectTemplate != null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					TINationState requiredNationState = tiprojectTemplate.requiredNationState;
					if (requiredNationState != null)
					{
						stringBuilder.AppendLine(Loc.T("UI.Science.RequiredNation", new object[] { requiredNationState.displayNameWithArticle }));
					}
					if (!string.IsNullOrEmpty(stringBuilder.ToString()))
					{
						this.prereqArrow.enabled = true;
						this.otherRequirementsMasterPanel.SetActive(true);
						this.otherRequirementsList.SetText(stringBuilder.ToString());
					}
					else
					{
						this.otherRequirementsMasterPanel.SetActive(false);
					}
				}
				else
				{
					this.otherRequirementsMasterPanel.SetActive(false);
				}
				List<TIGenericTechTemplate> list = genericTech.AllPrereqFor(base.activePlayer, true);
				this.childArrow.enabled = list.Count > 0;
				this.childTechsGrid.SetListSize<ChildTechGridItemController>(list.Count, false, false);
				int num = 0;
				using (IEnumerator<object> enumerator = this.childTechsGrid.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__312.<>p__0 == null)
						{
							ResearchScreenController.<>o__312.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ChildTechGridItemController childTechGridItemController = ResearchScreenController.<>o__312.<>p__0.Target(ResearchScreenController.<>o__312.<>p__0, enumerator.Current);
						childTechGridItemController.Init(this, list[num++]);
						childTechGridItemController.UpdateGridItem();
					}
				}
			}
		}

		// Token: 0x06005607 RID: 22023 RVA: 0x00273484 File Offset: 0x00271684
		public void InitializeFullTechTree(bool selectiveTree = false, string selectiveTech = "", bool noProjectTree = false, bool selectiveFromFull = true)
		{
			if (!this.usingFullTechTree)
			{
				return;
			}
			this.CloseSelectedTechPanel(true);
			if (selectiveFromFull)
			{
				this.fullTechTreeObject.SetActive(true);
			}
			else
			{
				this.fullTechTreeObjectNP.SetActive(true);
			}
			if (!selectiveTree)
			{
				this.currentListManager = this.FullTechTreeGridManager;
				this.currentNodeContainer = this.nodeContainer;
				this.currentPrereqLineContainer = this.prereqLineContainer;
				this.currentTechContent = this.fullTechTreeContent;
			}
			else
			{
				this.currentListManager = this.selectiveTechTreeGridManager;
				this.currentNodeContainer = this.selectiveNodeContainer;
				this.currentPrereqLineContainer = this.selectiveTechTreePrereqLineContainer;
				this.currentTechContent = this.selectiveTechTreeContent;
			}
			if (noProjectTree)
			{
				this.currentListManager = this.fullTechTreeGridManagerNP;
				this.currentNodeContainer = this.nodeContainerNP;
				this.currentPrereqLineContainer = this.prereqLineContainerNP;
				this.currentTechContent = this.fullTechTreeContentNP;
			}
			this.treeNodes.Clear();
			for (int i = 0; i < this.currentNodeContainer.transform.childCount; i++)
			{
				this.treeNodes.Add(this.currentNodeContainer.transform.GetChild(i).gameObject);
			}
			if (!this.isFullTechTreeInit || selectiveTree)
			{
				List<TITechTemplate> allTechs = TIGlobalResearchState.GetAllTechs();
				List<TIProjectTemplate> allProjects = TIGlobalResearchState.GetAllProjects();
				List<TIProjectTemplate> list = new List<TIProjectTemplate>();
				List<TITechTemplate> list2 = new List<TITechTemplate>();
				if (selectiveTree)
				{
					foreach (ChildTechGridItemController childTechGridItemController in this.selectiveTechList)
					{
						for (int j = 0; j < allProjects.Count; j++)
						{
							if (allProjects[j].dataName == childTechGridItemController.tech.dataName)
							{
								list.Add(allProjects[j]);
							}
						}
						for (int k = 0; k < allTechs.Count; k++)
						{
							if (allTechs[k].dataName == childTechGridItemController.GetComponent<ChildTechGridItemController>().tech.dataName)
							{
								list2.Add(allTechs[k]);
							}
						}
					}
				}
				if (!selectiveTree)
				{
					for (int l = 0; l < allProjects.Count; l++)
					{
						list.Add(allProjects[l]);
					}
				}
				this.sortedTechList.Clear();
				this.techVisited.Clear();
				if (selectiveTree)
				{
					this.currentListManager.SetListSize<ChildTechGridItemController>(list2.Count + list.Count, false, false);
				}
				else if (this.addProjects)
				{
					this.currentListManager.SetListSize<ChildTechGridItemController>(allTechs.Count + list.Count, false, false);
				}
				else
				{
					this.currentListManager.SetListSize<ChildTechGridItemController>(allTechs.Count, false, false);
				}
				int num = 0;
				int num2 = 0;
				if (!selectiveTree)
				{
					using (IEnumerator<object> enumerator2 = this.currentListManager.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (ResearchScreenController.<>o__313.<>p__0 == null)
							{
								ResearchScreenController.<>o__313.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
							}
							ChildTechGridItemController childTechGridItemController2 = ResearchScreenController.<>o__313.<>p__0.Target(ResearchScreenController.<>o__313.<>p__0, enumerator2.Current);
							if (this.addProjects && num >= allTechs.Count)
							{
								childTechGridItemController2.Init(this, list[num2++]);
							}
							if (num < allTechs.Count)
							{
								childTechGridItemController2.Init(this, allTechs[num++]);
							}
							this.sortedTechList.Add(childTechGridItemController2);
							this.techVisited.Add(false);
						}
					}
				}
				if (selectiveTree)
				{
					using (IEnumerator<object> enumerator2 = this.currentListManager.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (ResearchScreenController.<>o__313.<>p__1 == null)
							{
								ResearchScreenController.<>o__313.<>p__1 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
							}
							ChildTechGridItemController childTechGridItemController3 = ResearchScreenController.<>o__313.<>p__1.Target(ResearchScreenController.<>o__313.<>p__1, enumerator2.Current);
							if (this.addProjects && num >= list2.Count)
							{
								childTechGridItemController3.Init(this, list[num2++]);
							}
							if (num < list2.Count)
							{
								childTechGridItemController3.Init(this, list2[num++]);
							}
							this.sortedTechList.Add(childTechGridItemController3);
							this.techVisited.Add(false);
						}
					}
				}
				if (!selectiveTree)
				{
					this.selectiveMode = false;
					this.BuildTree(this.sortedTechList, false);
					this.PlaceTechsBehindPrereqs();
					this.PlaceTechsBehindSameTierPrereqs();
					this.HandleEndGameTechs();
					base.StartCoroutine(this.SetGridParent());
					Debug.Log("Start TT CR");
					foreach (ChildTechGridItemController childTechGridItemController4 in this.sortedTechList)
					{
						this.mainTechObjectList.Add(childTechGridItemController4);
					}
				}
			}
			if (noProjectTree)
			{
				List<TITechTemplate> allTechs2 = TIGlobalResearchState.GetAllTechs();
				List<TIProjectTemplate> list3 = new List<TIProjectTemplate>();
				this.sortedTechList.Clear();
				this.techVisited.Clear();
				this.currentListManager.SetListSize<ChildTechGridItemController>(allTechs2.Count, false, false);
				int num3 = 0;
				int num4 = 0;
				if (!selectiveTree)
				{
					using (IEnumerator<object> enumerator2 = this.currentListManager.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (ResearchScreenController.<>o__313.<>p__2 == null)
							{
								ResearchScreenController.<>o__313.<>p__2 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
							}
							ChildTechGridItemController childTechGridItemController5 = ResearchScreenController.<>o__313.<>p__2.Target(ResearchScreenController.<>o__313.<>p__2, enumerator2.Current);
							if (this.addProjects && num3 >= allTechs2.Count)
							{
								childTechGridItemController5.Init(this, list3[num4++]);
							}
							if (num3 < allTechs2.Count)
							{
								childTechGridItemController5.Init(this, allTechs2[num3++]);
							}
							this.sortedTechList.Add(childTechGridItemController5);
							this.techVisited.Add(false);
						}
					}
				}
				if (noProjectTree)
				{
					this.BuildTree(this.sortedTechList, false);
					this.PlaceTechsBehindPrereqs();
					this.PlaceTechsBehindLowerCosts();
					this.PlaceTechsBehindSameTierPrereqs();
					this.HandleEndGameTechs();
					base.StartCoroutine(this.SetGridParent());
					this.isFullTechTreeInitNP = true;
					this.ShowNoProjectTechTree();
				}
			}
			if (selectiveTree)
			{
				this.selectiveMode = true;
				this.selectiveTechTreeContent.transform.localScale = Vector3.one;
				this.BuildTree(this.sortedTechList, true);
				this.PlaceTechsBehindPrereqs();
				this.PlaceTechsBehindSameTierPrereqs();
				this.HandleEndGameTechs();
				base.StartCoroutine(this.SetGridParent());
				this.ShowSelectiveTechTree();
				base.StartCoroutine(this.GotoSearchItemNextFrame(this.controllerForSelectiveTree, ResearchScreenController.techTreeType.selectiveTree));
			}
		}

		// Token: 0x06005608 RID: 22024 RVA: 0x00273B68 File Offset: 0x00271D68
		private void BuildTree(List<ChildTechGridItemController> techList, bool selectiveTree = false)
		{
			Func<ChildTechGridItemController, float> <>9__1;
			Log.Time("<color=#00cc00>LoadTime:</color> TechTree, BuildTree", delegate
			{
				IEnumerable<ChildTechGridItemController> techList2 = techList;
				Func<ChildTechGridItemController, float> func;
				if ((func = <>9__1) == null)
				{
					func = (<>9__1 = (ChildTechGridItemController o) => o.tech.GetResearchCost(this.activePlayer));
				}
				techList = techList2.OrderBy<ChildTechGridItemController, float>(func).ToList<ChildTechGridItemController>();
				int num = 0;
				int num2 = 0;
				foreach (ChildTechGridItemController childTechGridItemController in techList)
				{
					if (selectiveTree)
					{
						childTechGridItemController.visited = false;
					}
					TITechTemplate titechTemplate = childTechGridItemController.tech as TITechTemplate;
					if (GameStateManager.Time().template.techTreeUIStarters.Contains(childTechGridItemController.tech.dataName))
					{
						childTechGridItemController.transform.SetParent(this.treeNodes[0].transform);
						childTechGridItemController.node = 0;
						childTechGridItemController.visited = true;
						this.totalVisited++;
						foreach (ChildTechGridItemController childTechGridItemController2 in techList)
						{
							foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController2.tech.TechPrereqs)
							{
								if (!childTechGridItemController2.visited && tigenericTechTemplate.dataName == childTechGridItemController.tech.dataName)
								{
									childTechGridItemController2.node = childTechGridItemController.node + 1;
									if (childTechGridItemController2.node > this.lastNode)
									{
										this.lastNode = childTechGridItemController2.node + 1;
									}
									childTechGridItemController2.transform.SetParent(this.treeNodes[childTechGridItemController2.node].transform);
									childTechGridItemController2.visited = true;
									this.totalVisited++;
								}
							}
						}
						num2++;
					}
					else if (titechTemplate != null)
					{
						if ((!titechTemplate.endGameTech && childTechGridItemController.tech.TechPrereqs.Count == 0) || childTechGridItemController.tech.dataName == "Skywatch")
						{
							childTechGridItemController.transform.SetParent(this.treeNodes[1].transform);
							childTechGridItemController.node = 1;
							childTechGridItemController.visited = true;
							this.totalVisited++;
						}
						else if (titechTemplate.endGameTech)
						{
							this.endGameTechs++;
						}
					}
					else if ((childTechGridItemController.tech.GetResearchCost(this.activePlayer) < 100000f && childTechGridItemController.tech.TechPrereqs.Count == 0) || childTechGridItemController.tech.dataName == "Skywatch")
					{
						childTechGridItemController.transform.SetParent(this.treeNodes[1].transform);
						childTechGridItemController.node = 1;
						childTechGridItemController.visited = true;
						this.totalVisited++;
					}
					else
					{
						TITechTemplate ref_tech = childTechGridItemController.tech.ref_tech;
						if (ref_tech != null && ref_tech.endGameTech)
						{
							this.endGameTechs++;
						}
					}
					num++;
				}
			}, true, true);
		}

		// Token: 0x06005609 RID: 22025 RVA: 0x00273BA8 File Offset: 0x00271DA8
		public void InitializeSelectiveTechTree(string dataName, string displayName, bool fullTree)
		{
			Debug.Log(dataName);
			this.selectiveTechTreeHeader.SetText(Loc.T("UI.Science.SelectiveTreeHeader", new object[]
			{
				displayName,
				Loc.T("UI.Science.TechTreeHeader")
			}));
			this.selectiveTechList.Clear();
			if (fullTree)
			{
				using (IEnumerator<object> enumerator = this.FullTechTreeGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__315.<>p__0 == null)
						{
							ResearchScreenController.<>o__315.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ChildTechGridItemController childTechGridItemController = ResearchScreenController.<>o__315.<>p__0.Target(ResearchScreenController.<>o__315.<>p__0, enumerator.Current);
						if (childTechGridItemController.techName.color != this.techNameColorDeSelected)
						{
							this.selectiveTechList.Add(childTechGridItemController);
						}
					}
					goto IL_016A;
				}
			}
			using (IEnumerator<object> enumerator = this.fullTechTreeGridManagerNP.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__315.<>p__1 == null)
					{
						ResearchScreenController.<>o__315.<>p__1 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
					}
					ChildTechGridItemController childTechGridItemController2 = ResearchScreenController.<>o__315.<>p__1.Target(ResearchScreenController.<>o__315.<>p__1, enumerator.Current);
					if (childTechGridItemController2.techName.color != this.techNameColorDeSelected)
					{
						this.selectiveTechList.Add(childTechGridItemController2);
					}
				}
			}
			IL_016A:
			this.InitializeFullTechTree(true, dataName, false, fullTree);
		}

		// Token: 0x0600560A RID: 22026 RVA: 0x00273D48 File Offset: 0x00271F48
		public void InitializeNoProjectTechTree()
		{
			this.fullTechTreeObjectNP.SetActive(true);
			this.selectiveTechTreeHeader.SetText(Loc.T("UI.Science.TechTreeHeader"));
			if (this.noProjectTechList.Count == 0)
			{
				using (IEnumerator<object> enumerator = this.FullTechTreeGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__316.<>p__0 == null)
						{
							ResearchScreenController.<>o__316.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ChildTechGridItemController childTechGridItemController = ResearchScreenController.<>o__316.<>p__0.Target(ResearchScreenController.<>o__316.<>p__0, enumerator.Current);
						if (!childTechGridItemController.tech.isProject())
						{
							this.noProjectTechList.Add(childTechGridItemController);
						}
					}
				}
			}
			this.InitializeFullTechTree(false, "", true, true);
		}

		// Token: 0x0600560B RID: 22027 RVA: 0x00273E28 File Offset: 0x00272028
		private void PlaceTechsBehindPrereqs()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> TechTree, PlaceTechsBehindPrereqs", delegate
			{
				int num = 0;
				for (;;)
				{
					num++;
					foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
					{
						if (!childTechGridItemController.visited)
						{
							if (childTechGridItemController.tech.TechPrereqs.Count > 0)
							{
								foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
								{
									if (childTechGridItemController2.tech.dataName == childTechGridItemController.tech.TechPrereqs[0].dataName)
									{
										childTechGridItemController.node = childTechGridItemController2.node + 1;
										if (childTechGridItemController.node > this.lastNode)
										{
											this.lastNode = childTechGridItemController.node + 1;
										}
										childTechGridItemController.transform.SetParent(this.treeNodes[childTechGridItemController.node].transform);
										childTechGridItemController.visited = true;
										this.totalVisited++;
									}
								}
							}
							if (this.selectiveMode && (childTechGridItemController.tech.AltTechPrereq0 != null || childTechGridItemController.tech.AltTechPrereq1 != null))
							{
								string dataName = childTechGridItemController.tech.dataName;
								TIGenericTechTemplate altTechPrereq = childTechGridItemController.tech.AltTechPrereq0;
								if (dataName == ((altTechPrereq != null) ? altTechPrereq.dataName : null))
								{
									Debug.Log("Error - " + childTechGridItemController.tech.dataName + " is an alt prereq of itself");
								}
								string dataName2 = childTechGridItemController.tech.dataName;
								TIGenericTechTemplate altTechPrereq2 = childTechGridItemController.tech.AltTechPrereq1;
								if (dataName2 == ((altTechPrereq2 != null) ? altTechPrereq2.dataName : null))
								{
									Debug.Log("Error - " + childTechGridItemController.tech.dataName + " is an alt prereq of itself");
								}
								foreach (ChildTechGridItemController childTechGridItemController3 in this.sortedTechList)
								{
									string dataName3 = childTechGridItemController3.tech.dataName;
									TIGenericTechTemplate altTechPrereq3 = childTechGridItemController.tech.AltTechPrereq0;
									if (!(dataName3 == ((altTechPrereq3 != null) ? altTechPrereq3.dataName : null)))
									{
										string dataName4 = childTechGridItemController3.tech.dataName;
										TIGenericTechTemplate altTechPrereq4 = childTechGridItemController.tech.AltTechPrereq1;
										if (!(dataName4 == ((altTechPrereq4 != null) ? altTechPrereq4.dataName : null)))
										{
											continue;
										}
									}
									childTechGridItemController.node = childTechGridItemController3.node + 1;
									if (childTechGridItemController.node > this.lastNode)
									{
										this.lastNode = childTechGridItemController.node + 1;
									}
									childTechGridItemController.transform.SetParent(this.treeNodes[childTechGridItemController.node].transform);
									childTechGridItemController.visited = true;
									this.totalVisited++;
								}
							}
						}
					}
					if (num > 20)
					{
						break;
					}
					if (this.totalVisited >= TIGlobalResearchState.GetAllTechs().Count - 1 - this.endGameTechs)
					{
						return;
					}
				}
				Debug.LogError("HIT TechTreeLimit Break, something is wrong");
				Debug.Log(this.totalVisited.ToString() + "/" + TIGlobalResearchState.GetAllTechs().Count.ToString());
			}, true, true);
		}

		// Token: 0x0600560C RID: 22028 RVA: 0x00273E44 File Offset: 0x00272044
		private void PlaceTechsBehindLowerCosts()
		{
			bool flag = false;
			int num = 0;
			for (;;)
			{
				int[] array = new int[this.nodeCountLimit];
				for (int i = 0; i < this.nodeCountLimit; i++)
				{
					float num2 = 0f;
					int num3 = 0;
					foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
					{
						if (childTechGridItemController.tech.researchCost >= 100f && childTechGridItemController.node == i)
						{
							num3++;
							num2 += childTechGridItemController.tech.researchCost;
						}
					}
					num2 /= (float)num3;
					array[i] = (int)num2;
				}
				flag = false;
				num++;
				foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
				{
					if (childTechGridItemController2.node > 0 && (double)childTechGridItemController2.tech.researchCost > (double)array[childTechGridItemController2.node] * 1.2)
					{
						childTechGridItemController2.node++;
						if (childTechGridItemController2.node == this.nodeCountLimit)
						{
							childTechGridItemController2.node = this.nodeCountLimit - 1;
						}
						if (childTechGridItemController2.node > this.lastNode)
						{
							this.lastNode = childTechGridItemController2.node + 1;
						}
						childTechGridItemController2.transform.SetParent(this.treeNodes[childTechGridItemController2.node].transform);
						childTechGridItemController2.visited = true;
						flag = true;
					}
				}
				if (num > 20)
				{
					break;
				}
				if (!flag)
				{
					return;
				}
			}
			Debug.LogWarning("HIT TechTreeLimit Break in PlaceTechsBehindLowerCosts, something is wrong");
			Debug.Log(this.totalVisited.ToString() + "/" + TIGlobalResearchState.GetAllTechs().Count.ToString());
		}

		// Token: 0x0600560D RID: 22029 RVA: 0x0027403C File Offset: 0x0027223C
		public void HandleEndGameTechs()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> TechTree, HandleEndGameTechs", delegate
			{
				foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
				{
					if (!childTechGridItemController.visited)
					{
						TITechTemplate ref_tech = childTechGridItemController.tech.ref_tech;
						if (ref_tech != null && ref_tech.endGameTech)
						{
							childTechGridItemController.transform.SetParent(this.currentNodeContainer.transform.GetChild(this.lastNode).transform);
							childTechGridItemController.node = this.lastNode + 1;
							if (childTechGridItemController.node > this.nodeCountLimit)
							{
								childTechGridItemController.node = this.nodeCountLimit - 1;
							}
							childTechGridItemController.visited = true;
							this.totalVisited++;
						}
					}
				}
			}, true, true);
		}

		// Token: 0x0600560E RID: 22030 RVA: 0x00274056 File Offset: 0x00272256
		private IEnumerator ToggleTechVisibility(bool fullTree = false)
		{
			yield return null;
			Log.Time("<color=#00cc00>LoadTime:</color> TechTree, ToggleTechVisibility", delegate
			{
				foreach (ChildTechGridItemController childTechGridItemController in (fullTree ? this.mainTechObjectList : this.sortedTechList))
				{
					if (childTechGridItemController.tech.ShouldHide(this.activePlayer) && childTechGridItemController.gameObject.activeSelf)
					{
						childTechGridItemController.gameObject.SetActive(false);
						childTechGridItemController.showLines = false;
						childTechGridItemController.hidden = true;
					}
					else if (!childTechGridItemController.tech.ShouldHide(this.activePlayer) && !childTechGridItemController.gameObject.activeSelf)
					{
						childTechGridItemController.gameObject.SetActive(true);
						childTechGridItemController.showLines = true;
						childTechGridItemController.hidden = false;
					}
				}
				this.ToggleTechLineVisibility();
			}, true, true);
			this.openingSelectiveTree = false;
			yield break;
		}

		// Token: 0x0600560F RID: 22031 RVA: 0x0027406C File Offset: 0x0027226C
		private void ToggleTechLineVisibility()
		{
			for (int i = 0; i < this.currentPrereqLineContainer.transform.childCount; i++)
			{
				GameObject gameObject = this.currentPrereqLineContainer.transform.GetChild(i).gameObject;
				TechTreeConnection component = gameObject.GetComponent<TechTreeConnection>();
				if (!component.preTech.activeSelf || !component.endTech.activeSelf)
				{
					if (gameObject.activeSelf)
					{
						gameObject.SetActive(false);
					}
				}
				else if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
				}
			}
			this.isFullTechTreeInit = true;
			Debug.Log("FinishedTechTreeInit");
			if (!this.isFullTechTreeInitNP)
			{
				this.InitializeNoProjectTechTree();
			}
		}

		// Token: 0x06005610 RID: 22032 RVA: 0x0027410B File Offset: 0x0027230B
		public void PlaceTechsBehindSameTierPrereqs()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> TechTree, PlaceTechsBehindSameTierPrereqs", delegate
			{
				bool flag = true;
				int num = 0;
				int num2 = 0;
				for (;;)
				{
					flag = true;
					num = 0;
					foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
					{
						if (childTechGridItemController.tech.TechPrereqs.Count > 0)
						{
							foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController.tech.TechPrereqs)
							{
								if (childTechGridItemController.tech.dataName == tigenericTechTemplate.dataName)
								{
									Debug.Log("Error - " + childTechGridItemController.tech.dataName + " is a prereq of itself");
								}
								foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
								{
									if (childTechGridItemController.node <= childTechGridItemController2.node && childTechGridItemController2.tech.dataName == tigenericTechTemplate.dataName)
									{
										childTechGridItemController.node = childTechGridItemController2.node + 1;
										if (childTechGridItemController.node == this.nodeCountLimit)
										{
											childTechGridItemController.node = this.nodeCountLimit - 1;
										}
										if (childTechGridItemController.node > this.lastNode)
										{
											this.lastNode = childTechGridItemController.node + 1;
										}
										childTechGridItemController.transform.SetParent(this.treeNodes[childTechGridItemController.node].transform);
										childTechGridItemController.visited = true;
										flag = false;
										num++;
									}
								}
							}
							if (childTechGridItemController.tech.AltTechPrereq0 != null || childTechGridItemController.tech.AltTechPrereq1 != null)
							{
								string dataName = childTechGridItemController.tech.dataName;
								TIGenericTechTemplate altTechPrereq = childTechGridItemController.tech.AltTechPrereq0;
								if (dataName == ((altTechPrereq != null) ? altTechPrereq.dataName : null))
								{
									Debug.Log("Error - " + childTechGridItemController.tech.dataName + " is an alt prereq of itself");
								}
								string dataName2 = childTechGridItemController.tech.dataName;
								TIGenericTechTemplate altTechPrereq2 = childTechGridItemController.tech.AltTechPrereq1;
								if (dataName2 == ((altTechPrereq2 != null) ? altTechPrereq2.dataName : null))
								{
									Debug.Log("Error - " + childTechGridItemController.tech.dataName + " is an alt prereq of itself");
								}
								foreach (ChildTechGridItemController childTechGridItemController3 in this.sortedTechList)
								{
									if (childTechGridItemController.node <= childTechGridItemController3.node)
									{
										string dataName3 = childTechGridItemController3.tech.dataName;
										TIGenericTechTemplate altTechPrereq3 = childTechGridItemController.tech.AltTechPrereq0;
										if (!(dataName3 == ((altTechPrereq3 != null) ? altTechPrereq3.dataName : null)))
										{
											string dataName4 = childTechGridItemController3.tech.dataName;
											TIGenericTechTemplate altTechPrereq4 = childTechGridItemController.tech.AltTechPrereq0;
											if (!(dataName4 == ((altTechPrereq4 != null) ? altTechPrereq4.dataName : null)))
											{
												continue;
											}
										}
										childTechGridItemController.node = childTechGridItemController3.node + 1;
										if (childTechGridItemController.node == this.nodeCountLimit)
										{
											childTechGridItemController.node = this.nodeCountLimit - 1;
										}
										childTechGridItemController.transform.SetParent(this.treeNodes[childTechGridItemController.node].transform);
										childTechGridItemController.visited = true;
										flag = false;
										num++;
									}
								}
							}
						}
					}
					num2++;
					if (num2 > 40)
					{
						break;
					}
					if (flag)
					{
						return;
					}
				}
				Debug.Log("Place Behind Iteration Limit");
			}, true, true);
		}

		// Token: 0x06005611 RID: 22033 RVA: 0x00274125 File Offset: 0x00272325
		public IEnumerator SetGridParent()
		{
			yield return null;
			this.sortedTechList.Clear();
			foreach (object obj in this.currentNodeContainer.transform)
			{
				using (IEnumerator enumerator2 = ((Transform)obj).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ChildTechGridItemController childTechGridItemController;
						if (((Transform)enumerator2.Current).TryGetComponent<ChildTechGridItemController>(out childTechGridItemController))
						{
							this.sortedTechList.Add(childTechGridItemController);
						}
					}
				}
			}
			base.StartCoroutine(this.SetupSpacing());
			foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
			{
				childTechGridItemController2.GetComponent<LayoutElement>().enabled = false;
				childTechGridItemController2.transform.SetParent(this.currentListManager.gameObject.transform, true);
			}
			base.StartCoroutine(this.SetupConnections());
			yield break;
		}

		// Token: 0x06005612 RID: 22034 RVA: 0x00274134 File Offset: 0x00272334
		public IEnumerator SetupSpacing()
		{
			yield return null;
			Log.Time("<color=#00cc00>LoadTime:</color> TechTree, SetupSpacing", delegate
			{
				this.SetContentHeight();
				this.SpaceTechBranches();
				this.AlignTechsToPrereqs();
				this.AlignProjectsToUnlocks();
				this.AlignTechsToPrereqs();
				this.AlignTechsWithNoPrereqToUnlocks();
				this.AlignTechsToUnlocks();
				this.AlignTechsToPrereqs();
				this.PushTechsAway();
				this.SetContentHeight();
			}, true, true);
			yield break;
		}

		// Token: 0x06005613 RID: 22035 RVA: 0x00274144 File Offset: 0x00272344
		private void AlignTechsToPrereqs()
		{
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				float num = 0f;
				if (childTechGridItemController.tech.TechPrereqs.Count > 0)
				{
					foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController.tech.TechPrereqs)
					{
						foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
						{
							if (childTechGridItemController2.tech.dataName == tigenericTechTemplate.dataName)
							{
								num += childTechGridItemController2.transform.localPosition.y;
							}
						}
					}
					num /= (float)childTechGridItemController.tech.TechPrereqs.Count;
					childTechGridItemController.prereqY = num;
					childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, num, childTechGridItemController.transform.localPosition.z);
				}
			}
		}

		// Token: 0x06005614 RID: 22036 RVA: 0x002742D0 File Offset: 0x002724D0
		private void AlignTechsToUnlocks()
		{
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				float num = 0f;
				int num2 = 0;
				num = 0f;
				foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
				{
					foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController2.tech.TechPrereqs)
					{
						if (childTechGridItemController.tech.dataName == tigenericTechTemplate.dataName)
						{
							num += childTechGridItemController2.transform.localPosition.y;
							num2++;
						}
					}
				}
				if (num2 > 0)
				{
					num /= (float)num2;
					childTechGridItemController.prereqY = num;
					childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, num, childTechGridItemController.transform.localPosition.z);
				}
			}
		}

		// Token: 0x06005615 RID: 22037 RVA: 0x00274448 File Offset: 0x00272648
		private void AlignTechsWithNoPrereqToUnlocks()
		{
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				float num = 0f;
				if (childTechGridItemController.tech.TechPrereqs.Count == 0 && childTechGridItemController.tech.GetResearchCost(base.activePlayer) > 500f && childTechGridItemController.tech.GetResearchCost(base.activePlayer) < 100000f)
				{
					int num2 = 0;
					num = 0f;
					foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
					{
						foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController2.tech.TechPrereqs)
						{
							if (childTechGridItemController.tech.dataName == tigenericTechTemplate.dataName)
							{
								num += childTechGridItemController2.transform.localPosition.y;
								num2++;
							}
						}
					}
					if (num2 > 0)
					{
						num /= (float)num2;
						childTechGridItemController.prereqY = num;
						childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, num, childTechGridItemController.transform.localPosition.z);
					}
				}
			}
		}

		// Token: 0x06005616 RID: 22038 RVA: 0x0027460C File Offset: 0x0027280C
		private void AlignProjectsToUnlocks()
		{
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				float num = 0f;
				int num2 = 0;
				foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
				{
					if (childTechGridItemController.node != 0)
					{
						foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController2.tech.TechPrereqs)
						{
							if (childTechGridItemController.tech.dataName == tigenericTechTemplate.dataName)
							{
								num += childTechGridItemController2.transform.localPosition.y;
								num2++;
							}
						}
					}
				}
				if (num2 > 0)
				{
					num /= (float)num2;
					childTechGridItemController.prereqY = num;
					childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, num, childTechGridItemController.transform.localPosition.z);
				}
			}
		}

		// Token: 0x06005617 RID: 22039 RVA: 0x00274788 File Offset: 0x00272988
		private void SetContentHeight()
		{
			this.contentHeight = 0f;
			this.contentWidth = 0f;
			this.nodeCounts = new List<int>(this.nodeCountLimit);
			for (int i = 0; i < this.nodeCountLimit; i++)
			{
				this.nodeCounts.Add(0);
			}
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				List<int> list = this.nodeCounts;
				int node = childTechGridItemController.node;
				int num = list[node];
				list[node] = num + 1;
				if (childTechGridItemController.transform.localPosition.y < this.contentHeight)
				{
					this.contentHeight = childTechGridItemController.transform.localPosition.y;
				}
				if (childTechGridItemController.transform.localPosition.x > this.contentWidth - 1000f)
				{
					this.contentWidth = childTechGridItemController.transform.localPosition.x + 1000f;
				}
			}
			this.currentTechContent.GetComponent<RectTransform>().sizeDelta = new Vector2(this.contentWidth, -this.contentHeight + 500f);
		}

		// Token: 0x06005618 RID: 22040 RVA: 0x002748D0 File Offset: 0x00272AD0
		private void PushTechsAway()
		{
			bool flag = true;
			float num = 52f;
			int num2 = 0;
			while (num2 < 20 && flag)
			{
				flag = false;
				foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
				{
					foreach (ChildTechGridItemController childTechGridItemController2 in this.sortedTechList)
					{
						if (childTechGridItemController != childTechGridItemController2 && childTechGridItemController.node == childTechGridItemController2.node && Mathf.Abs(childTechGridItemController.transform.localPosition.y - childTechGridItemController2.transform.localPosition.y) < num)
						{
							flag = true;
							if (childTechGridItemController2.tech.TechPrereqs.Count > 0)
							{
								childTechGridItemController2.transform.localPosition += new Vector3(0f, Mathf.Abs(childTechGridItemController.transform.localPosition.y - childTechGridItemController2.transform.localPosition.y) - num, 0f);
							}
							if (childTechGridItemController2.tech.TechPrereqs.Count == 0)
							{
								childTechGridItemController2.transform.localPosition += new Vector3(0f, Mathf.Abs(childTechGridItemController.transform.localPosition.y - childTechGridItemController2.transform.localPosition.y) + num, 0f);
							}
						}
					}
				}
				num2++;
			}
		}

		// Token: 0x06005619 RID: 22041 RVA: 0x00274ABC File Offset: 0x00272CBC
		private void SpaceTechBranches()
		{
			float num = 0.6f;
			float num2 = 0f;
			if (this.addProjects)
			{
				num = 1.2f;
			}
			if (this.selectiveMode)
			{
				num2 = 0f;
				num = 0.6f;
			}
			List<int> list = new List<int>();
			for (int i = 0; i < this.nodeCounts.Count; i++)
			{
				list.Add(0);
			}
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				if (childTechGridItemController.node == 0 || (this.selectiveMode && this.nodeCounts[0] == 0 && childTechGridItemController.node == 1))
				{
					childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, this.contentHeight / (float)this.nodeCounts[childTechGridItemController.node] * (((float)list[childTechGridItemController.node] + num) / 2f) + num2, childTechGridItemController.transform.localPosition.z);
					if (childTechGridItemController.prereqY != 0f)
					{
						childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, childTechGridItemController.prereqY, childTechGridItemController.transform.localPosition.z);
					}
					List<int> list2 = list;
					int node = childTechGridItemController.node;
					int num3 = list2[node];
					list2[node] = num3 + 1;
				}
				else if (childTechGridItemController.tech.TechPrereqs.Count == 0 && childTechGridItemController.node == 1)
				{
					childTechGridItemController.transform.localPosition = new Vector3(childTechGridItemController.transform.localPosition.x, (childTechGridItemController.prereqY == 0f) ? (childTechGridItemController.transform.localPosition.y - 40f) : childTechGridItemController.prereqY, childTechGridItemController.transform.localPosition.z);
				}
			}
		}

		// Token: 0x0600561A RID: 22042 RVA: 0x00274CEC File Offset: 0x00272EEC
		public IEnumerator SetupConnections()
		{
			yield return null;
			using (IEnumerator<object> enumerator = this.currentListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__332.<>p__0 == null)
					{
						ResearchScreenController.<>o__332.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
					}
					ChildTechGridItemController childTechGridItemController = ResearchScreenController.<>o__332.<>p__0.Target(ResearchScreenController.<>o__332.<>p__0, enumerator.Current);
					foreach (TIGenericTechTemplate tigenericTechTemplate in childTechGridItemController.tech.TechPrereqs)
					{
						this.DrawConnections(childTechGridItemController, tigenericTechTemplate, false, false);
					}
					if (childTechGridItemController.tech.AltTechPrereq0 != null)
					{
						this.DrawConnections(childTechGridItemController, childTechGridItemController.tech.AltTechPrereq0, true, false);
					}
					if (childTechGridItemController.tech.AltTechPrereq1 != null)
					{
						this.DrawConnections(childTechGridItemController, childTechGridItemController.tech.AltTechPrereq1, false, true);
					}
				}
			}
			base.StartCoroutine(this.ToggleTechVisibility(false));
			yield break;
		}

		// Token: 0x0600561B RID: 22043 RVA: 0x00274CFC File Offset: 0x00272EFC
		public void DrawConnections(ChildTechGridItemController controller, TIGenericTechTemplate tech, bool altReq0 = false, bool altReq1 = false)
		{
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				if (tech.dataName == childTechGridItemController.GetComponent<ChildTechGridItemController>().tech.dataName)
				{
					Vector2 vector = childTechGridItemController.transform.localPosition - controller.transform.localPosition;
					float num = childTechGridItemController.GetComponent<RectTransform>().sizeDelta.x / 2f;
					vector += new Vector2(num, 0f);
					RectTransform component = base.GetComponent<RectTransform>();
					vector *= new Vector2(component.localScale.x, component.localScale.y);
					controller.connectionList.Add(childTechGridItemController.techNameString);
					childTechGridItemController.enablesList.Add(controller.gameObject);
					if (altReq0)
					{
						controller.altPrereq0 = childTechGridItemController.gameObject;
					}
					if (altReq1)
					{
						controller.altPrereq1 = childTechGridItemController.gameObject;
					}
					controller.prereqList.Add(childTechGridItemController.gameObject);
					controller.SetConnection(vector, childTechGridItemController.gameObject);
					break;
				}
			}
		}

		// Token: 0x0600561C RID: 22044 RVA: 0x00274E54 File Offset: 0x00273054
		public void ResetAllConnectionColors()
		{
			using (IEnumerator<object> enumerator = this.FullTechTreeGridManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__334.<>p__0 == null)
					{
						ResearchScreenController.<>o__334.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
					}
					ResearchScreenController.<>o__334.<>p__0.Target(ResearchScreenController.<>o__334.<>p__0, enumerator.Current).ResetLineColors();
				}
			}
			if (this.selectiveTechTreeCanvas.enabled)
			{
				using (IEnumerator<object> enumerator = this.selectiveTechTreeGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__334.<>p__1 == null)
						{
							ResearchScreenController.<>o__334.<>p__1 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ResearchScreenController.<>o__334.<>p__1.Target(ResearchScreenController.<>o__334.<>p__1, enumerator.Current).ResetLineColors();
					}
				}
			}
			if (this.fullTechTreeCanvasNP.enabled)
			{
				using (IEnumerator<object> enumerator = this.fullTechTreeGridManagerNP.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__334.<>p__2 == null)
						{
							ResearchScreenController.<>o__334.<>p__2 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ResearchScreenController.<>o__334.<>p__2.Target(ResearchScreenController.<>o__334.<>p__2, enumerator.Current).ResetLineColors();
					}
				}
			}
		}

		// Token: 0x0600561D RID: 22045 RVA: 0x00274FEC File Offset: 0x002731EC
		public void OnClickShowFullTechTree()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.ShowFullTechTree();
		}

		// Token: 0x0600561E RID: 22046 RVA: 0x00275000 File Offset: 0x00273200
		public void ShowFullTechTree()
		{
			this.tabbedPaneManager.Toggle(this.techTreeTab);
			this.CloseSelectedTechPanel(false);
			this.currentTechTreeViewed = ResearchScreenController.techTreeType.fullTree;
			this.currentPrereqLineContainer = this.prereqLineContainer;
			this.fullTechTreeCanvas.enabled = true;
			this.fullTechTreeCanvasNP.enabled = false;
			this.techTreeContentToScale = this.fullTechTreeContent;
			this.UpdateTechTreeZoomSlider();
			this.UpdateResearchTargetText();
			this.techTreeZoomObject.transform.SetParent(this.fullTechTreeObject.transform, true);
			this.techTreeZoomObject.transform.SetAsLastSibling();
			this.selectedTechPanelObject.transform.SetParent(this.fullTechTreeObject.transform, true);
			this.selectedTechPanelObject.transform.SetAsLastSibling();
			this.researchTargetObject.transform.SetParent(this.fullTechTreeObject.transform, true);
			this.researchTargetObject.transform.SetAsLastSibling();
			foreach (ChildTechGridItemController childTechGridItemController in this.mainTechObjectList)
			{
				childTechGridItemController.UpdateGridItem();
			}
			base.StartCoroutine(this.ToggleTechVisibility(true));
			if (!string.IsNullOrEmpty(this.selectedProjectEntry))
			{
				ChildTechGridItemController childTechGridItemController2 = null;
				foreach (ChildTechGridItemController childTechGridItemController3 in this.mainTechObjectList)
				{
					if (childTechGridItemController3.tech.dataName == this.selectedProjectEntry)
					{
						childTechGridItemController2 = childTechGridItemController3;
						break;
					}
				}
				if (childTechGridItemController2 != null)
				{
					base.StartCoroutine(this.GotoSearchItemNextFrame(childTechGridItemController2, ResearchScreenController.techTreeType.fullTree));
					return;
				}
			}
			else if (this.selectedTechPanelTechName.text != null)
			{
				ChildTechGridItemController childTechGridItemController4 = null;
				foreach (ChildTechGridItemController childTechGridItemController5 in this.mainTechObjectList)
				{
					if (childTechGridItemController5.tech.displayName == this.selectedTechPanelTechName.text)
					{
						childTechGridItemController4 = childTechGridItemController5;
						break;
					}
				}
				if (childTechGridItemController4 != null)
				{
					base.StartCoroutine(this.GotoSearchItemNextFrame(childTechGridItemController4, ResearchScreenController.techTreeType.fullTree));
				}
			}
		}

		// Token: 0x0600561F RID: 22047 RVA: 0x00275248 File Offset: 0x00273448
		public void CloseFullTechTree()
		{
			this.fullTechTreeCanvas.enabled = false;
			this.techTreeContentToScale = null;
			this.CloseSelectedTechPanel(true);
			if (string.IsNullOrEmpty(this.selectedProjectEntry))
			{
				this.currentTechTreeViewed = ResearchScreenController.techTreeType.techsOnly;
				this.fullTechTreeCanvasNP.enabled = true;
				this.techTreeContentToScale = this.fullTechTreeContentNP;
				this.UpdateTechTreeZoomSlider();
				this.UpdateResearchTargetText();
				this.techTreeZoomObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
				this.techTreeZoomObject.transform.SetAsLastSibling();
				this.selectedTechPanelObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
				this.selectedTechPanelObject.transform.SetAsLastSibling();
				this.researchTargetObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
				this.researchTargetObject.transform.SetAsLastSibling();
			}
			else
			{
				this.fullTechTreeCanvasNP.enabled = true;
				this.tabbedPaneManager.Toggle(this.researchTab);
			}
			this.rightButtonOverlayPanel.enabled = false;
			this.mainGridObject.SetActive(true);
		}

		// Token: 0x06005620 RID: 22048 RVA: 0x00275365 File Offset: 0x00273565
		public void OnClickShowNoProjectTechTree()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.ShowNoProjectTechTree();
		}

		// Token: 0x06005621 RID: 22049 RVA: 0x0027537C File Offset: 0x0027357C
		public void ShowNoProjectTechTree()
		{
			this.tabbedPaneManager.Toggle(this.techTreeTab);
			if (!this.isFullTechTreeInitNP)
			{
				this.InitializeNoProjectTechTree();
			}
			this.CloseSelectedTechPanel(true);
			this.currentTechTreeViewed = ResearchScreenController.techTreeType.techsOnly;
			this.fullTechTreeCanvasNP.enabled = true;
			this.fullTechTreeCanvas.enabled = false;
			this.techTreeContentToScale = this.fullTechTreeContentNP;
			this.UpdateTechTreeZoomSlider();
			this.UpdateResearchTargetText();
			this.techTreeZoomObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
			this.techTreeZoomObject.transform.SetAsLastSibling();
			this.selectedTechPanelObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
			this.selectedTechPanelObject.transform.SetAsLastSibling();
			this.researchTargetObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
			this.researchTargetObject.transform.SetAsLastSibling();
			this.sortedTechList.Clear();
			using (IEnumerator<object> enumerator = this.fullTechTreeGridManagerNP.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__339.<>p__0 == null)
					{
						ResearchScreenController.<>o__339.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
					}
					ChildTechGridItemController childTechGridItemController = ResearchScreenController.<>o__339.<>p__0.Target(ResearchScreenController.<>o__339.<>p__0, enumerator.Current);
					childTechGridItemController.UpdateGridItem();
					this.sortedTechList.Add(childTechGridItemController);
				}
			}
			if (!string.IsNullOrEmpty(this.selectedTechEntry))
			{
				ChildTechGridItemController childTechGridItemController2 = null;
				foreach (ChildTechGridItemController childTechGridItemController3 in this.sortedTechList)
				{
					if (childTechGridItemController3.tech.dataName == this.selectedTechEntry)
					{
						childTechGridItemController2 = childTechGridItemController3;
						break;
					}
				}
				if (childTechGridItemController2 != null)
				{
					base.StartCoroutine(this.GotoSearchItemNextFrame(childTechGridItemController2, ResearchScreenController.techTreeType.techsOnly));
				}
			}
		}

		// Token: 0x06005622 RID: 22050 RVA: 0x00275584 File Offset: 0x00273784
		public void CloseNoProjectTechTree()
		{
			this.fullTechTreeCanvas.enabled = false;
			this.fullTechTreeCanvasNP.enabled = false;
			this.rightButtonOverlayPanel.enabled = false;
			this.mainGridObject.SetActive(true);
			this.techTreeContentToScale = null;
		}

		// Token: 0x06005623 RID: 22051 RVA: 0x002755C0 File Offset: 0x002737C0
		public void ShowSelectiveTechTree()
		{
			this.selectiveTechTreeCanvas.enabled = true;
			this.currentTechTreeViewed = ResearchScreenController.techTreeType.selectiveTree;
			this.CloseSelectedTechPanel(false);
			this.techTreeContentToScale = this.selectiveTechTreeContent;
			this.UpdateTechTreeZoomSlider();
			this.UpdateResearchTargetText();
			this.techTreeZoomObject.transform.SetParent(this.selectiveTechTreeObject.transform, true);
			this.techTreeZoomObject.transform.SetAsLastSibling();
			this.selectedTechPanelObject.transform.SetParent(this.selectiveTechTreeObject.transform, true);
			this.selectedTechPanelObject.transform.SetAsLastSibling();
			this.researchTargetObject.transform.SetParent(this.selectiveTechTreeObject.transform, true);
			this.researchTargetObject.transform.SetAsLastSibling();
			foreach (ChildTechGridItemController childTechGridItemController in this.sortedTechList)
			{
				childTechGridItemController.UpdateGridItem();
			}
		}

		// Token: 0x06005624 RID: 22052 RVA: 0x002756C8 File Offset: 0x002738C8
		public void OnClickCloseSelectiveTechTree()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseSelectiveTechTree();
		}

		// Token: 0x06005625 RID: 22053 RVA: 0x002756DC File Offset: 0x002738DC
		public void CloseSelectiveTechTree()
		{
			if (this.openingSelectiveTree)
			{
				return;
			}
			this.selectiveTechTreeCanvas.enabled = false;
			this.controllerForSelectiveTree = null;
			this.CloseSelectedTechPanel(true);
			if (this.fullTechTreeCanvas.enabled)
			{
				this.currentTechTreeViewed = ResearchScreenController.techTreeType.fullTree;
				this.techTreeContentToScale = this.fullTechTreeContent;
				this.UpdateTechTreeZoomSlider();
				this.UpdateResearchTargetText();
				this.techTreeZoomObject.transform.SetParent(this.fullTechTreeObject.transform, true);
				this.techTreeZoomObject.transform.SetAsLastSibling();
				this.selectedTechPanelObject.transform.SetParent(this.fullTechTreeObject.transform, true);
				this.selectedTechPanelObject.transform.SetAsLastSibling();
				this.researchTargetObject.transform.SetParent(this.fullTechTreeObject.transform, true);
				this.researchTargetObject.transform.SetAsLastSibling();
			}
			else if (this.fullTechTreeCanvasNP.enabled)
			{
				this.currentTechTreeViewed = ResearchScreenController.techTreeType.techsOnly;
				this.techTreeContentToScale = this.fullTechTreeContentNP;
				this.UpdateTechTreeZoomSlider();
				this.UpdateResearchTargetText();
				this.techTreeZoomObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
				this.techTreeZoomObject.transform.SetAsLastSibling();
				this.selectedTechPanelObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
				this.selectedTechPanelObject.transform.SetAsLastSibling();
				this.researchTargetObject.transform.SetParent(this.fullTechTreeObjectNP.transform, true);
				this.researchTargetObject.transform.SetAsLastSibling();
			}
			this.ClearSelectiveTechTreeData();
		}

		// Token: 0x06005626 RID: 22054 RVA: 0x00275880 File Offset: 0x00273A80
		private void ClearSelectiveTechTreeData()
		{
			TooltipManager.Instance.HideAll();
			for (int i = 0; i < this.selectiveTechTreePrereqLineContainer.transform.childCount; i++)
			{
				global::UnityEngine.Object.Destroy(this.selectiveTechTreePrereqLineContainer.transform.GetChild(i).gameObject);
			}
			for (int j = 0; j < this.selectiveTechTreeGridManager.gameObject.transform.childCount; j++)
			{
				if (j == 0)
				{
					ChildTechGridItemController component = this.selectiveTechTreeGridManager.gameObject.transform.GetChild(j).gameObject.GetComponent<ChildTechGridItemController>();
					component.connectionLines.Clear();
					component.enablesList.Clear();
					component.connectionsTarget.Clear();
					component.connectionList.Clear();
					component.prereqList.Clear();
					component.connectionInit = false;
					component.connectionTarget = new Vector2(0f, 0f);
					component.prereqY = 0f;
					component.imageLoaded = false;
				}
				if (j > 0)
				{
					global::UnityEngine.Object.Destroy(this.selectiveTechTreeGridManager.gameObject.transform.GetChild(j).gameObject);
				}
			}
		}

		// Token: 0x06005627 RID: 22055 RVA: 0x002759A4 File Offset: 0x00273BA4
		public void RefreshTechTreeStatuses()
		{
			if (this.currentTechTreeViewed == ResearchScreenController.techTreeType.techsOnly)
			{
				using (IEnumerator<object> enumerator = this.fullTechTreeGridManagerNP.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__345.<>p__0 == null)
						{
							ResearchScreenController.<>o__345.<>p__0 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ResearchScreenController.<>o__345.<>p__0.Target(ResearchScreenController.<>o__345.<>p__0, enumerator.Current).UpdateGridItem();
					}
				}
			}
			if (this.currentTechTreeViewed == ResearchScreenController.techTreeType.fullTree)
			{
				foreach (ChildTechGridItemController childTechGridItemController in this.mainTechObjectList)
				{
					childTechGridItemController.UpdateGridItem();
				}
			}
			if (this.currentTechTreeViewed == ResearchScreenController.techTreeType.selectiveTree || this.selectiveTechTreeCanvas.enabled)
			{
				using (IEnumerator<object> enumerator = this.selectiveTechTreeGridManager.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__345.<>p__1 == null)
						{
							ResearchScreenController.<>o__345.<>p__1 = CallSite<Func<CallSite, object, ChildTechGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ChildTechGridItemController), typeof(ResearchScreenController)));
						}
						ResearchScreenController.<>o__345.<>p__1.Target(ResearchScreenController.<>o__345.<>p__1, enumerator.Current).UpdateGridItem();
					}
				}
			}
			this.UpdateSelectedTechPanel(null, null);
			this.UpdateResearchTargetText();
		}

		// Token: 0x06005628 RID: 22056 RVA: 0x00275B1C File Offset: 0x00273D1C
		public void UpdateSelectedTechPanel(TIGenericTechTemplate tech, ChildTechGridItemController itemController)
		{
			if (tech != null && itemController != null)
			{
				this.cachedSelectedTech = tech;
				this.cachedItemController = itemController;
			}
			else if (this.cachedSelectedTech == null || this.cachedItemController == null)
			{
				return;
			}
			int techStatusAppearanceIndex = ResearchScreenController.GetTechStatusAppearanceIndex(this.cachedSelectedTech, base.activePlayer);
			this.selectedLongTermTechButton.gameObject.SetActive(this.cachedSelectedTech.prereqs.Count > 0);
			this.UpdateLongTermTechTargetButtonText();
			this.selectedTechPanelObject.SetActive(true);
			this.selectedTechPanelTechName.SetText(this.cachedSelectedTech.displayName);
			GameControl.assetLoader.LoadAssetForImageAssignment(this.cachedSelectedTech.GetCategoryIconPath(), this.selectedTechPanelCategoryIcon);
			this.selectedTechPanelTopGradient.sprite = this.techStatusGradient[techStatusAppearanceIndex];
			this.selectedTechPanelStatusGradient.sprite = this.techStatusGradient[techStatusAppearanceIndex];
			this.selectedTechPanelTechCostLabel.SetText(Loc.T("UI.Science.Cost"));
			this.selectedTechPanelRequiresHeaderText.SetText(Loc.T("UI.Science.Requires"));
			this.selectedTechPanelUnlocksHeaderText.SetText(Loc.T("UI.Science.Unlocks"));
			this.selectedTechPanelTechCost.SetText(new StringBuilder(TemplateManager.global.researchInlineSpritePath).Append(" ").Append(this.cachedSelectedTech.GetResearchCost(base.activePlayer)).ToString());
			string text = this.cachedSelectedTech.BenefitsDescription(base.activePlayer, TechBenefitsContext.Prospective, null);
			text = text.TrimStart(new char[] { '\r', '\n' }).TrimEnd(new char[] { '\r', '\n' });
			this.selectedTechPanelTechSummary.SetText(text);
			this.selectedTechPanelTechCategory.SetText(this.cachedSelectedTech.categoryString);
			if (techStatusAppearanceIndex == 10 || techStatusAppearanceIndex == 11)
			{
				if (this.cachedSelectedTech.ref_project != null)
				{
					float num = base.activePlayer.GetProjectUnlockChance(this.cachedSelectedTech.ref_project, base.activePlayer.TechContributionBonus(this.cachedSelectedTech.ref_project)) / 100f;
					this.selectedTechPanelTechStatus.SetText(Loc.T("UI.Science.UnlockChanceFull", new object[] { num.ToPercent("P0") }));
				}
			}
			else
			{
				this.selectedTechPanelTechStatus.SetText(ResearchScreenController.TechStatusString(this.cachedSelectedTech, base.activePlayer));
			}
			int num2;
			if (this.currentTechTreeViewed == ResearchScreenController.techTreeType.selectiveTree)
			{
				List<TIGenericTechTemplate> list = new List<TIGenericTechTemplate>();
				list = this.cachedItemController.tech.TechPrereqs.ToList<TIGenericTechTemplate>();
				if (this.cachedItemController.tech.AltTechPrereq0 != null)
				{
					list.Add(this.cachedItemController.tech.AltTechPrereq0);
				}
				if (this.cachedItemController.tech.AltTechPrereq1 != null)
				{
					list.Add(this.cachedItemController.tech.AltTechPrereq1);
				}
				list = list.Where<TIGenericTechTemplate>((TIGenericTechTemplate x) => !x.ShouldHide(base.activePlayer)).ToList<TIGenericTechTemplate>();
				this.selectedTechPanelRequirementList.SetListSize<SelectedTechListItemController>(list.Count, false, false);
				num2 = 0;
				using (IEnumerator<object> enumerator = this.selectedTechPanelRequirementList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ResearchScreenController.<>o__348.<>p__0 == null)
						{
							ResearchScreenController.<>o__348.<>p__0 = CallSite<Func<CallSite, object, SelectedTechListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SelectedTechListItemController), typeof(ResearchScreenController)));
						}
						SelectedTechListItemController selectedTechListItemController = ResearchScreenController.<>o__348.<>p__0.Target(ResearchScreenController.<>o__348.<>p__0, enumerator.Current);
						ChildTechGridItemController childTechGridItemController = null;
						foreach (GameObject gameObject in this.cachedItemController.prereqList)
						{
							ChildTechGridItemController component = gameObject.GetComponent<ChildTechGridItemController>();
							if (component.tech == list[num2])
							{
								childTechGridItemController = component;
								break;
							}
						}
						if (list[num2].IsAnAltPrereqOf(this.cachedItemController.tech))
						{
							int num3 = -1;
							if (this.cachedSelectedTech.prereqs.Count >= 1 && list[num2].dataName == this.cachedSelectedTech.prereqs[0])
							{
								num3 = 0;
							}
							else if (this.cachedSelectedTech.prereqs.Count >= 2 && list[num2].dataName == this.cachedSelectedTech.prereqs[1])
							{
								num3 = 1;
							}
							else if (list[num2] == this.cachedSelectedTech.AltTechPrereq0)
							{
								num3 = 0;
							}
							else if (list[num2] == this.cachedSelectedTech.AltTechPrereq1)
							{
								num3 = 1;
							}
							selectedTechListItemController.UpdateData(list[num2], this, childTechGridItemController, true, new TIGenericTechTemplate[]
							{
								list[num2],
								this.cachedSelectedTech.AltTechPrereq0
							}, new TIGenericTechTemplate[]
							{
								list[num2],
								this.cachedSelectedTech.AltTechPrereq1
							}, num3);
						}
						else
						{
							selectedTechListItemController.UpdateData(list[num2], this, childTechGridItemController, true, null, null, -1);
						}
						num2++;
					}
					goto IL_06DB;
				}
			}
			this.selectedTechPanelRequirementList.SetListSize<SelectedTechListItemController>(this.cachedItemController.prereqList.Count, false, false);
			num2 = 0;
			using (IEnumerator<object> enumerator = this.selectedTechPanelRequirementList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__348.<>p__1 == null)
					{
						ResearchScreenController.<>o__348.<>p__1 = CallSite<Func<CallSite, object, SelectedTechListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SelectedTechListItemController), typeof(ResearchScreenController)));
					}
					SelectedTechListItemController selectedTechListItemController2 = ResearchScreenController.<>o__348.<>p__1.Target(ResearchScreenController.<>o__348.<>p__1, enumerator.Current);
					ChildTechGridItemController component2 = this.cachedItemController.prereqList[num2].GetComponent<ChildTechGridItemController>();
					if (component2.tech.IsAnAltPrereqOf(this.cachedSelectedTech))
					{
						int num4 = -1;
						if (this.cachedSelectedTech.prereqs.Count >= 1 && component2.tech.dataName == this.cachedSelectedTech.prereqs[0])
						{
							num4 = 0;
						}
						else if (this.cachedSelectedTech.prereqs.Count >= 2 && component2.tech.dataName == this.cachedSelectedTech.prereqs[1])
						{
							num4 = 1;
						}
						else if (component2.tech == this.cachedSelectedTech.AltTechPrereq0)
						{
							num4 = 0;
						}
						else if (component2.tech == this.cachedSelectedTech.AltTechPrereq1)
						{
							num4 = 1;
						}
						selectedTechListItemController2.UpdateData(component2.tech, this, component2, true, new TIGenericTechTemplate[]
						{
							component2.tech,
							this.cachedSelectedTech.AltTechPrereq0
						}, new TIGenericTechTemplate[]
						{
							component2.tech,
							this.cachedSelectedTech.AltTechPrereq1
						}, num4);
						num2++;
					}
					else
					{
						selectedTechListItemController2.UpdateData(component2.tech, this, component2, true, null, null, -1);
						num2++;
					}
				}
			}
			IL_06DB:
			this.selectedTechPanelUnlocksList.SetListSize<SelectedTechListItemController>(this.cachedItemController.enablesList.Count, false, false);
			num2 = 0;
			using (IEnumerator<object> enumerator = this.selectedTechPanelUnlocksList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__348.<>p__2 == null)
					{
						ResearchScreenController.<>o__348.<>p__2 = CallSite<Func<CallSite, object, SelectedTechListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(SelectedTechListItemController), typeof(ResearchScreenController)));
					}
					SelectedTechListItemController selectedTechListItemController3 = ResearchScreenController.<>o__348.<>p__2.Target(ResearchScreenController.<>o__348.<>p__2, enumerator.Current);
					ChildTechGridItemController component3 = this.cachedItemController.enablesList[num2].GetComponent<ChildTechGridItemController>();
					selectedTechListItemController3.UpdateData(component3.tech, this, component3, false, null, null, -1);
					num2++;
				}
			}
		}

		// Token: 0x06005629 RID: 22057 RVA: 0x00276324 File Offset: 0x00274524
		public void UpdateLongTermTechTargetButtonText()
		{
			this.selectedTechPanelLongTermButtonText.SetText((this.cachedSelectedTech.dataName == base.activePlayer.longtermTechTarget) ? Loc.T("UI.Science.ClearLongTermTech") : Loc.T("UI.Science.SelectLongTermTech"));
			this.UpdateResearchTargetText();
		}

		// Token: 0x0600562A RID: 22058 RVA: 0x00276375 File Offset: 0x00274575
		public void OnClickCloseSelectedTechPanel(bool clearName)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseSelectedTechPanel(clearName);
		}

		// Token: 0x0600562B RID: 22059 RVA: 0x0027638A File Offset: 0x0027458A
		public void CloseSelectedTechPanel(bool clearName = true)
		{
			this.selectedTechPanelObject.SetActive(false);
			if (clearName)
			{
				this.selectedTechPanelTechName.SetText("");
			}
		}

		// Token: 0x0600562C RID: 22060 RVA: 0x002763AB File Offset: 0x002745AB
		public void UpdateTechTreeZoomSlider()
		{
			this.techTreeZoomSlider.SetValueWithoutNotify(this.techTreeContentToScale.localScale.x / 0.05f - 6f);
		}

		// Token: 0x0600562D RID: 22061 RVA: 0x002763D4 File Offset: 0x002745D4
		public void UpdateResearchTargetText()
		{
			if (string.IsNullOrEmpty(base.activePlayer.longtermTechTarget))
			{
				this.researchTargetObject.SetActive(false);
				return;
			}
			TIGenericTechTemplate tigenericTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(base.activePlayer.longtermTechTarget, true);
			if (tigenericTechTemplate != null)
			{
				this.researchTargetText.SetText(Loc.T("UI.Science.ResearchTarget", new object[] { tigenericTechTemplate.displayName }));
				this.researchTargetObject.SetActive(true);
				return;
			}
			this.researchTargetObject.SetActive(false);
		}

		// Token: 0x0600562E RID: 22062 RVA: 0x00276454 File Offset: 0x00274654
		public void UpdateTechTreeZoom()
		{
			if (this.techTreeContentToScale != null)
			{
				this.techTreeContentToScale.localScale = new Vector3(0.3f + this.techTreeZoomSlider.value * 0.05f, 0.3f + this.techTreeZoomSlider.value * 0.05f, 1f);
			}
		}

		// Token: 0x0600562F RID: 22063 RVA: 0x002764B4 File Offset: 0x002746B4
		public void IncreaseTechTreeZoom()
		{
			this.techTreeZoomSlider.value = Mathf.Clamp(this.techTreeZoomSlider.value + 1f, this.techTreeZoomSlider.minValue, this.techTreeZoomSlider.maxValue);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
		}

		// Token: 0x06005630 RID: 22064 RVA: 0x00276504 File Offset: 0x00274704
		public void DecreaseTechTreeZoom()
		{
			this.techTreeZoomSlider.value = Mathf.Clamp(this.techTreeZoomSlider.value - 1f, this.techTreeZoomSlider.minValue, this.techTreeZoomSlider.maxValue);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_HoverNonButton", false, false);
		}

		// Token: 0x06005631 RID: 22065 RVA: 0x00276554 File Offset: 0x00274754
		private void ToggleTechTreeMoveScrolling(bool allow)
		{
			int num = (allow ? 60 : 0);
			this.FullTechTreeScrollRect.scrollSensitivity = (float)num;
			this.FullTechTreeScrollRectNP.scrollSensitivity = (float)num;
			this.SelectiveTechTreeScrollRect.scrollSensitivity = (float)num;
		}

		// Token: 0x06005632 RID: 22066 RVA: 0x00276591 File Offset: 0x00274791
		public static string EffectContextToString(Context context)
		{
			return Loc.T(new StringBuilder("Context.displayName.").Append(context.ToString()).ToString());
		}

		// Token: 0x06005633 RID: 22067 RVA: 0x002765BC File Offset: 0x002747BC
		private void InitializeEffectsBreakdownScreen()
		{
			this.effectsTabText.SetText(Loc.T("UI.Science.EffectsTabText"));
			this.effectsHeaderText.SetText(Loc.T("UI.Science.EffectsPanelHeader"));
			this.effectsGeneralExplainerText.SetText(Loc.T("UI.Science.EffectsPanelExplainer"));
		}

		// Token: 0x06005634 RID: 22068 RVA: 0x00276608 File Offset: 0x00274808
		private void UpdateEffectsBreakdownScreen()
		{
			List<Context> list = new List<Context>();
			foreach (object obj in Enum.GetValues(typeof(Context)))
			{
				Context context = (Context)obj;
				if (TIEffectsState.GetFactionEffectsForContext(context, base.activePlayer).Any<TIEffectTemplate>((TIEffectTemplate x) => x.description(base.activePlayer, null) != string.Empty) && ResearchScreenController.EffectContextToString(context) != string.Empty)
				{
					list.Add(context);
				}
			}
			this.effectsContextList.SetListSize<EffectContextListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator2 = this.effectsContextList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (ResearchScreenController.<>o__360.<>p__0 == null)
					{
						ResearchScreenController.<>o__360.<>p__0 = CallSite<Func<CallSite, object, EffectContextListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EffectContextListItemController), typeof(ResearchScreenController)));
					}
					ResearchScreenController.<>o__360.<>p__0.Target(ResearchScreenController.<>o__360.<>p__0, enumerator2.Current).SetListItem(list[num++], this);
				}
			}
			if (this.selectedContext == Context.None)
			{
				this.selectedContextNameText.SetText(string.Empty);
				this.primarySelectedEffectListingText.SetText(string.Empty);
				this.totalEffectListingText.SetText(string.Empty);
			}
		}

		// Token: 0x06005635 RID: 22069 RVA: 0x0027677C File Offset: 0x0027497C
		public void OnEffectContextButtonPressed(Context context)
		{
			this.selectedContext = context;
			this.selectedContextNameText.SetText(ResearchScreenController.EffectContextToString(this.selectedContext));
			StringBuilder stringBuilder = new StringBuilder();
			List<TIEffectTemplate> effectList = (from x in TIEffectsState.GetFactionEffectsForContext(this.selectedContext, base.activePlayer)
				orderby x.value descending
				select x).ToList<TIEffectTemplate>();
			if (effectList.Count > 0)
			{
				bool flag = effectList.Count > 1 && effectList.All<TIEffectTemplate>((TIEffectTemplate x) => x.operation == effectList[0].operation);
				if (flag && effectList[0].operation == StatModSetOperation.DecreaseToValue)
				{
					effectList.Reverse();
				}
				bool flag2 = false;
				bool flag3 = false;
				foreach (TIEffectTemplate tieffectTemplate in effectList)
				{
					string text = tieffectTemplate.description(base.activePlayer, null);
					bool flag4 = text.Any<char>(new Func<char, bool>(char.IsDigit));
					flag2 = flag2 || flag4;
					flag3 = text.Contains("%");
					if (flag && flag4)
					{
						StatModSetOperation operation = effectList[0].operation;
						if (operation != StatModSetOperation.IncreaseToValue)
						{
							if (operation == StatModSetOperation.DecreaseToValue)
							{
								if (tieffectTemplate.value > effectList.Min<TIEffectTemplate>((TIEffectTemplate x) => x.value))
								{
									text = TIUtilities.RedLine(text);
								}
								else
								{
									text = TIUtilities.GreenLine(text);
								}
							}
						}
						else if (tieffectTemplate.value < effectList.Max<TIEffectTemplate>((TIEffectTemplate x) => x.value))
						{
							text = TIUtilities.RedLine(text);
						}
						else
						{
							text = TIUtilities.GreenLine(text);
						}
					}
					if (text != string.Empty)
					{
						if (tieffectTemplate.effectDuration == EffectDuration.temporary)
						{
							stringBuilder.Append(Loc.T("UI.Science.EffectTemporary"));
						}
						stringBuilder.AppendLine(text);
					}
				}
				if (flag && flag2)
				{
					switch (effectList[0].operation)
					{
					case StatModSetOperation.SetToFixedValue:
					case StatModSetOperation.IncreaseToValue:
					case StatModSetOperation.DecreaseToValue:
					case StatModSetOperation.SetToAnotherAttribute:
						this.totalEffectListingText.SetText(string.Empty);
						break;
					case StatModSetOperation.Additive:
					case StatModSetOperation.AdditivePer:
					case StatModSetOperation.SubtractivePer:
					case StatModSetOperation.Multiplicative:
					{
						float num = (float)((effectList[0].operation == StatModSetOperation.Multiplicative) ? 1 : 0);
						float num2 = TIEffectsState.SumEffectsModifiers(context, base.activePlayer, num, null);
						if (num2 == num)
						{
							this.totalEffectListingText.SetText(string.Empty);
						}
						else
						{
							switch (effectList[0].showTotal)
							{
							case TotalEffectDisplayBehavior.Invert:
								num2 *= -1f;
								break;
							case TotalEffectDisplayBehavior.Positive:
								num2 = Mathf.Abs(num2);
								break;
							case TotalEffectDisplayBehavior.Negative:
								if (num2 > 0f)
								{
									num2 = -num2;
								}
								break;
							}
							string text2 = (flag3 ? num2.ToPercent(TIUtilities.DecimalPlaces_P((double)num2, 2, 0)) : TIUtilities.FormatBigOrSmallNumber(num2, 1, 7, 0, false, false));
							this.totalEffectListingText.SetText(Loc.T("UI.Science.UnlocksCodexMission", new object[]
							{
								Loc.T("UI.Council.Ledger.FactionTotals"),
								text2
							}));
						}
						break;
					}
					}
				}
				else
				{
					this.totalEffectListingText.SetText(string.Empty);
				}
			}
			this.primarySelectedEffectListingText.SetText(stringBuilder.ToString());
			this.HighlightSelectedEffectsEntry(this.selectedContext);
		}

		// Token: 0x06005636 RID: 22070 RVA: 0x00276B40 File Offset: 0x00274D40
		public void HighlightSelectedEffectsEntry(Context entryContext = Context.None)
		{
			bool flag = entryContext == Context.None;
			using (IEnumerator<object> enumerator = this.effectsContextList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (ResearchScreenController.<>o__362.<>p__0 == null)
					{
						ResearchScreenController.<>o__362.<>p__0 = CallSite<Func<CallSite, object, EffectContextListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(EffectContextListItemController), typeof(ResearchScreenController)));
					}
					EffectContextListItemController effectContextListItemController = ResearchScreenController.<>o__362.<>p__0.Target(ResearchScreenController.<>o__362.<>p__0, enumerator.Current);
					effectContextListItemController.SetSelected(!flag && entryContext == effectContextListItemController.effectContext);
				}
			}
		}

		// Token: 0x04003C52 RID: 15442
		public Canvas primaryResearchPanel;

		// Token: 0x04003C53 RID: 15443
		public RectTransform primaryPanelTransform;

		// Token: 0x04003C54 RID: 15444
		public UITutorialController researchUITutorialController;

		// Token: 0x04003C55 RID: 15445
		public UITutorialController archivesUITutorialController;

		// Token: 0x04003C56 RID: 15446
		public UITutorialController techTreeUITutorialController;

		// Token: 0x04003C57 RID: 15447
		public UITutorialController modifiersUITutorialController;

		// Token: 0x04003C58 RID: 15448
		public ResearchPanelController[] researchPanelGrid;

		// Token: 0x04003C59 RID: 15449
		public TabbedPaneManager tabbedPaneManager;

		// Token: 0x04003C5A RID: 15450
		public TabbedPaneController researchTab;

		// Token: 0x04003C5B RID: 15451
		public TabbedPaneController techTreeTab;

		// Token: 0x04003C5C RID: 15452
		public TMP_Text researchTabText;

		// Token: 0x04003C5D RID: 15453
		public TMP_Text archiveTabText;

		// Token: 0x04003C5E RID: 15454
		public TMP_Text techTreeTabText;

		// Token: 0x04003C5F RID: 15455
		public TMP_Text researchPanelHeader;

		// Token: 0x04003C60 RID: 15456
		public TMP_Text globalResearchHeader;

		// Token: 0x04003C61 RID: 15457
		public TMP_Text councilEngineeringHeader;

		// Token: 0x04003C62 RID: 15458
		public GameObject mainGridObject;

		// Token: 0x04003C63 RID: 15459
		public Image orgProjectBackground;

		// Token: 0x04003C64 RID: 15460
		public TMP_Text orgProjectRequiredExplainer;

		// Token: 0x04003C65 RID: 15461
		public Image habProjectBackground;

		// Token: 0x04003C66 RID: 15462
		public TMP_Text habProjectRequiredExplainer;

		// Token: 0x04003C67 RID: 15463
		public Canvas rightButtonOverlayPanel;

		// Token: 0x04003C68 RID: 15464
		[Header("Archives")]
		public Canvas archivesOverlayPanel;

		// Token: 0x04003C69 RID: 15465
		public ListManagerBase archivedTechsList;

		// Token: 0x04003C6A RID: 15466
		public ScrollRect archiveTechDetailScrollRect;

		// Token: 0x04003C6B RID: 15467
		public TMP_Text archiveHeadline;

		// Token: 0x04003C6C RID: 15468
		public TMP_Text archiveCategory;

		// Token: 0x04003C6D RID: 15469
		public TMP_Text archiveSummary;

		// Token: 0x04003C6E RID: 15470
		public TMP_Text archiveBody;

		// Token: 0x04003C6F RID: 15471
		public Image archiveCategoryIcon;

		// Token: 0x04003C70 RID: 15472
		public TMP_Text archiveCategoryDescription;

		// Token: 0x04003C71 RID: 15473
		public Image archiveTechIcon;

		// Token: 0x04003C72 RID: 15474
		private string selectedArchiveEntry;

		// Token: 0x04003C73 RID: 15475
		public TMP_Text archiveOverlayTitle;

		// Token: 0x04003C74 RID: 15476
		public TMP_Text archiveSearchTitle;

		// Token: 0x04003C75 RID: 15477
		public TMP_InputField archiveSearchInput;

		// Token: 0x04003C76 RID: 15478
		[Header("Select Project")]
		public Canvas selectProjectOverlay;

		// Token: 0x04003C77 RID: 15479
		public TMP_Text selectProjectOverlayTitle;

		// Token: 0x04003C78 RID: 15480
		private int changingProjectSlot;

		// Token: 0x04003C79 RID: 15481
		public ListManagerBase availableProjectsList;

		// Token: 0x04003C7A RID: 15482
		public ScrollRect selectProjectTextDetailScrollRect;

		// Token: 0x04003C7B RID: 15483
		public TMP_Text availableProjectHeadline;

		// Token: 0x04003C7C RID: 15484
		public TMP_Text availableProjectSummary;

		// Token: 0x04003C7D RID: 15485
		public TMP_Text availableProjectBody;

		// Token: 0x04003C7E RID: 15486
		public TMP_Text availableProjectTechCategoryName;

		// Token: 0x04003C7F RID: 15487
		public Image availableProjectTechCategoryIcon;

		// Token: 0x04003C80 RID: 15488
		public TMP_Text availableProjectTechCategoryText;

		// Token: 0x04003C81 RID: 15489
		public Image availableProjectImage;

		// Token: 0x04003C82 RID: 15490
		public TMP_Text selectProjectButtonText;

		// Token: 0x04003C83 RID: 15491
		public Button selectProjectTechTreeButton;

		// Token: 0x04003C84 RID: 15492
		private string selectedProjectEntry;

		// Token: 0x04003C85 RID: 15493
		public TMP_Dropdown sortProjectDropdown;

		// Token: 0x04003C86 RID: 15494
		public Toggle projectSortAscendToggle;

		// Token: 0x04003C87 RID: 15495
		public Toggle projectSortObsoleteToggle;

		// Token: 0x04003C88 RID: 15496
		public TMP_Text projectSortByText;

		// Token: 0x04003C89 RID: 15497
		public TMP_Text projectSortAscendText;

		// Token: 0x04003C8A RID: 15498
		public TMP_Text projectSortObsoleteText;

		// Token: 0x04003C8B RID: 15499
		private ResearchScreenController.SortProjectDataBy currentProjectSort;

		// Token: 0x04003C8C RID: 15500
		private int lastProjectSort;

		// Token: 0x04003C8D RID: 15501
		private bool projectSortAscend = true;

		// Token: 0x04003C8E RID: 15502
		private bool projectSortShowObsolete = true;

		// Token: 0x04003C8F RID: 15503
		[Header("Select Tech")]
		public Canvas selectTechOverlay;

		// Token: 0x04003C90 RID: 15504
		private int selectTechSlot;

		// Token: 0x04003C91 RID: 15505
		public ListManagerBase availableTechsList;

		// Token: 0x04003C92 RID: 15506
		public ScrollRect selectTechTextDetailScrollRect;

		// Token: 0x04003C93 RID: 15507
		public TMP_Text availableTechHeadline;

		// Token: 0x04003C94 RID: 15508
		public TMP_Text availableTechSummary;

		// Token: 0x04003C95 RID: 15509
		public TMP_Text availableTechBody;

		// Token: 0x04003C96 RID: 15510
		public Image availableTechImage;

		// Token: 0x04003C97 RID: 15511
		public TMP_Text selectTechOverlayTitle;

		// Token: 0x04003C98 RID: 15512
		public TMP_Text selectTechButtonText;

		// Token: 0x04003C99 RID: 15513
		public TMP_Text availableTechTechCategoryName;

		// Token: 0x04003C9A RID: 15514
		public Image availableTechTechCategoryIcon;

		// Token: 0x04003C9B RID: 15515
		public TMP_Text availableTechTechCategoryText;

		// Token: 0x04003C9C RID: 15516
		public Button selectTechTechTreeButton;

		// Token: 0x04003C9D RID: 15517
		private string selectedTechEntry;

		// Token: 0x04003C9E RID: 15518
		public TMP_Dropdown sortTechDropdown;

		// Token: 0x04003C9F RID: 15519
		public Toggle techSortAscendToggle;

		// Token: 0x04003CA0 RID: 15520
		public TMP_Text techSortByText;

		// Token: 0x04003CA1 RID: 15521
		public TMP_Text techSortAscendText;

		// Token: 0x04003CA2 RID: 15522
		private ResearchScreenController.SortTechDataBy currentTechSort;

		// Token: 0x04003CA3 RID: 15523
		private int lastTechSort;

		// Token: 0x04003CA4 RID: 15524
		private bool techSortAscend = true;

		// Token: 0x04003CA5 RID: 15525
		[Header("Effects Breakdown")]
		public TMP_Text effectsTabText;

		// Token: 0x04003CA6 RID: 15526
		public Canvas effectsBreakdownCanvas;

		// Token: 0x04003CA7 RID: 15527
		public ListManagerBase effectsContextList;

		// Token: 0x04003CA8 RID: 15528
		public TMP_InputField effectsSearchInput;

		// Token: 0x04003CA9 RID: 15529
		public TMP_Text effectsSearchTitle;

		// Token: 0x04003CAA RID: 15530
		public TMP_Text effectsHeaderText;

		// Token: 0x04003CAB RID: 15531
		public TMP_Text effectsGeneralExplainerText;

		// Token: 0x04003CAC RID: 15532
		public TMP_Text selectedContextNameText;

		// Token: 0x04003CAD RID: 15533
		public TMP_Text primarySelectedEffectListingText;

		// Token: 0x04003CAE RID: 15534
		public TMP_Text totalEffectListingText;

		// Token: 0x04003CAF RID: 15535
		private Context selectedContext;

		// Token: 0x04003CB0 RID: 15536
		private bool initProjectSortSettings;

		// Token: 0x04003CB1 RID: 15537
		private bool initTechSortSettings;

		// Token: 0x04003CB2 RID: 15538
		private TIGlobalResearchState globalResearchState;

		// Token: 0x04003CB3 RID: 15539
		[Header("Tech Tree")]
		public GameObject techTreeMasterObject;

		// Token: 0x04003CB4 RID: 15540
		public TMP_Text techTreeTitle;

		// Token: 0x04003CB5 RID: 15541
		public GameObject selectedTechPanel;

		// Token: 0x04003CB6 RID: 15542
		private TIGenericTechTemplate selectedTech;

		// Token: 0x04003CB7 RID: 15543
		public TMP_Text selectedTechName;

		// Token: 0x04003CB8 RID: 15544
		public TMP_Text selectedTechStatus;

		// Token: 0x04003CB9 RID: 15545
		public TMP_Text selectedTechDetail;

		// Token: 0x04003CBA RID: 15546
		public Image selectedTechIcon;

		// Token: 0x04003CBB RID: 15547
		public GameObject prereq1MasterPanel;

		// Token: 0x04003CBC RID: 15548
		public TMP_Text prereq1TechName;

		// Token: 0x04003CBD RID: 15549
		public TMP_Text prereq1Status;

		// Token: 0x04003CBE RID: 15550
		public Image prereq1Icon;

		// Token: 0x04003CBF RID: 15551
		public TMP_Text orText;

		// Token: 0x04003CC0 RID: 15552
		public GameObject orPanel;

		// Token: 0x04003CC1 RID: 15553
		public GameObject altPrereq1Panel;

		// Token: 0x04003CC2 RID: 15554
		public TMP_Text altPrereq1TechName;

		// Token: 0x04003CC3 RID: 15555
		public TMP_Text altPrereq1Status;

		// Token: 0x04003CC4 RID: 15556
		public Image altPrereq1Icon;

		// Token: 0x04003CC5 RID: 15557
		public GameObject prereq2MasterPanel;

		// Token: 0x04003CC6 RID: 15558
		public TMP_Text prereq2TechName;

		// Token: 0x04003CC7 RID: 15559
		public TMP_Text prereq2Status;

		// Token: 0x04003CC8 RID: 15560
		public Image prereq2Icon;

		// Token: 0x04003CC9 RID: 15561
		public GameObject prereq3MasterPanel;

		// Token: 0x04003CCA RID: 15562
		public TMP_Text prereq3TechName;

		// Token: 0x04003CCB RID: 15563
		public TMP_Text prereq3Status;

		// Token: 0x04003CCC RID: 15564
		public Image prereq3Icon;

		// Token: 0x04003CCD RID: 15565
		public GameObject prereq4MasterPanel;

		// Token: 0x04003CCE RID: 15566
		public TMP_Text prereq4TechName;

		// Token: 0x04003CCF RID: 15567
		public TMP_Text prereq4Status;

		// Token: 0x04003CD0 RID: 15568
		public Image prereq4Icon;

		// Token: 0x04003CD1 RID: 15569
		public GameObject otherRequirementsMasterPanel;

		// Token: 0x04003CD2 RID: 15570
		public TMP_Text otherRequirementsList;

		// Token: 0x04003CD3 RID: 15571
		public ListManagerBase childTechsGrid;

		// Token: 0x04003CD4 RID: 15572
		public Image prereqArrow;

		// Token: 0x04003CD5 RID: 15573
		public Image childArrow;

		// Token: 0x04003CD6 RID: 15574
		private bool usingFullTechTree = true;

		// Token: 0x04003CD7 RID: 15575
		private bool addProjects = true;

		// Token: 0x04003CD8 RID: 15576
		private bool isFullTechTreeInit;

		// Token: 0x04003CD9 RID: 15577
		private bool isFullTechTreeInitNP;

		// Token: 0x04003CDA RID: 15578
		[Header("TechTreeSelectedPanel")]
		public GameObject selectedTechPanelObject;

		// Token: 0x04003CDB RID: 15579
		public Image selectedTechPanelTopGradient;

		// Token: 0x04003CDC RID: 15580
		public Image selectedTechPanelStatusGradient;

		// Token: 0x04003CDD RID: 15581
		public Image selectedTechPanelCategoryIcon;

		// Token: 0x04003CDE RID: 15582
		public TMP_Text selectedTechPanelTechName;

		// Token: 0x04003CDF RID: 15583
		public TMP_Text selectedTechPanelTechCategory;

		// Token: 0x04003CE0 RID: 15584
		public TMP_Text selectedTechPanelTechStatus;

		// Token: 0x04003CE1 RID: 15585
		public TMP_Text selectedTechPanelTechCostLabel;

		// Token: 0x04003CE2 RID: 15586
		public TMP_Text selectedTechPanelTechPathCostLabel;

		// Token: 0x04003CE3 RID: 15587
		public TMP_Text selectedTechPanelTechCost;

		// Token: 0x04003CE4 RID: 15588
		public TMP_Text selectedTechPanelTechPathCost;

		// Token: 0x04003CE5 RID: 15589
		public TMP_Text selectedTechPanelTechSummary;

		// Token: 0x04003CE6 RID: 15590
		public TMP_Text selectedTechPanelRequiresHeaderText;

		// Token: 0x04003CE7 RID: 15591
		public TMP_Text selectedTechPanelUnlocksHeaderText;

		// Token: 0x04003CE8 RID: 15592
		public ListManagerBase selectedTechPanelRequirementList;

		// Token: 0x04003CE9 RID: 15593
		public ListManagerBase selectedTechPanelUnlocksList;

		// Token: 0x04003CEA RID: 15594
		public Button selectedLongTermTechButton;

		// Token: 0x04003CEB RID: 15595
		public TMP_Text selectedTechPanelLongTermButtonText;

		// Token: 0x04003CEC RID: 15596
		public TooltipTrigger selectedTechPanelLongTermButtonTooltip;

		// Token: 0x04003CED RID: 15597
		public static bool fullTechTreeOn;

		// Token: 0x04003CEE RID: 15598
		[Header("FullTechTree")]
		public TMP_Text techTreeHeader;

		// Token: 0x04003CEF RID: 15599
		public TMP_Text treeSwapButtonText;

		// Token: 0x04003CF0 RID: 15600
		public TMP_Text treeSimpleButtonText;

		// Token: 0x04003CF1 RID: 15601
		public TMP_Text closeSelectiveTreeButtonText;

		// Token: 0x04003CF2 RID: 15602
		public GameObject fullTechTreeObject;

		// Token: 0x04003CF3 RID: 15603
		public RectTransform fullTechTreeContent;

		// Token: 0x04003CF4 RID: 15604
		public ListManagerBase FullTechTreeGridManager;

		// Token: 0x04003CF5 RID: 15605
		public GameObject techTreeItemPrefab;

		// Token: 0x04003CF6 RID: 15606
		public GameObject prereqLineContainer;

		// Token: 0x04003CF7 RID: 15607
		public GameObject selectedFullTech;

		// Token: 0x04003CF8 RID: 15608
		public GameObject nodeContainer;

		// Token: 0x04003CF9 RID: 15609
		public Canvas fullTechTreeCanvas;

		// Token: 0x04003CFA RID: 15610
		public ScrollRect FullTechTreeScrollRect;

		// Token: 0x04003CFB RID: 15611
		[Header("FullTechTreeNoProjects")]
		public TMP_Text techTreeHeaderNP;

		// Token: 0x04003CFC RID: 15612
		public GameObject fullTechTreeObjectNP;

		// Token: 0x04003CFD RID: 15613
		public RectTransform fullTechTreeContentNP;

		// Token: 0x04003CFE RID: 15614
		public ListManagerBase fullTechTreeGridManagerNP;

		// Token: 0x04003CFF RID: 15615
		public GameObject prereqLineContainerNP;

		// Token: 0x04003D00 RID: 15616
		public GameObject nodeContainerNP;

		// Token: 0x04003D01 RID: 15617
		public Canvas fullTechTreeCanvasNP;

		// Token: 0x04003D02 RID: 15618
		public ScrollRect FullTechTreeScrollRectNP;

		// Token: 0x04003D03 RID: 15619
		[Header("SelectiveTechTree")]
		public TMP_Text selectiveTechTreeHeader;

		// Token: 0x04003D04 RID: 15620
		public GameObject selectiveTechTreeObject;

		// Token: 0x04003D05 RID: 15621
		public RectTransform selectiveTechTreeContent;

		// Token: 0x04003D06 RID: 15622
		public ListManagerBase selectiveTechTreeGridManager;

		// Token: 0x04003D07 RID: 15623
		public GameObject selectiveTechTreePrereqLineContainer;

		// Token: 0x04003D08 RID: 15624
		public GameObject selectiveNodeContainer;

		// Token: 0x04003D09 RID: 15625
		public Canvas selectiveTechTreeCanvas;

		// Token: 0x04003D0A RID: 15626
		public ScrollRect SelectiveTechTreeScrollRect;

		// Token: 0x04003D0B RID: 15627
		public ChildTechGridItemController controllerForSelectiveTree;

		// Token: 0x04003D0C RID: 15628
		public List<GameObject> treeNodes = new List<GameObject>();

		// Token: 0x04003D0D RID: 15629
		public List<ChildTechGridItemController> selectiveTechList = new List<ChildTechGridItemController>();

		// Token: 0x04003D0E RID: 15630
		public List<ChildTechGridItemController> mainTechObjectList = new List<ChildTechGridItemController>();

		// Token: 0x04003D0F RID: 15631
		public List<ChildTechGridItemController> noProjectTechList = new List<ChildTechGridItemController>();

		// Token: 0x04003D10 RID: 15632
		private RectTransform currentTechContent;

		// Token: 0x04003D11 RID: 15633
		private ListManagerBase currentListManager;

		// Token: 0x04003D12 RID: 15634
		public GameObject currentPrereqLineContainer;

		// Token: 0x04003D13 RID: 15635
		private GameObject currentNodeContainer;

		// Token: 0x04003D14 RID: 15636
		private Transform techTreeContentToScale;

		// Token: 0x04003D15 RID: 15637
		public ResearchScreenController.techTreeType currentTechTreeViewed;

		// Token: 0x04003D16 RID: 15638
		[Header("Tech Tree Zoom")]
		public GameObject techTreeZoomObject;

		// Token: 0x04003D17 RID: 15639
		public Slider techTreeZoomSlider;

		// Token: 0x04003D18 RID: 15640
		public TMP_Text techTreeZoomText;

		// Token: 0x04003D19 RID: 15641
		[Header("Tech Tree Search")]
		public TMP_InputField searchFieldTechs;

		// Token: 0x04003D1A RID: 15642
		public ListManagerBase searchResultsTechs;

		// Token: 0x04003D1B RID: 15643
		public RectTransform searchPanelTechs;

		// Token: 0x04003D1C RID: 15644
		public TMP_InputField searchFieldProjects;

		// Token: 0x04003D1D RID: 15645
		public ListManagerBase searchResultsProjects;

		// Token: 0x04003D1E RID: 15646
		public RectTransform searchPanelProjects;

		// Token: 0x04003D1F RID: 15647
		public TMP_Text searchPanelHeaderTechs;

		// Token: 0x04003D20 RID: 15648
		public TMP_Text searchPanelHeaderProjects;

		// Token: 0x04003D21 RID: 15649
		public TMP_Text searchPanelHeaderFullTechs;

		// Token: 0x04003D22 RID: 15650
		public TMP_Text searchPanelHeaderFullProjects;

		// Token: 0x04003D23 RID: 15651
		public Toggle fullSearchTechs;

		// Token: 0x04003D24 RID: 15652
		public Toggle fullSearchProjects;

		// Token: 0x04003D25 RID: 15653
		[Header("Tech Tree Misc")]
		public TMP_Text researchTargetText;

		// Token: 0x04003D26 RID: 15654
		public GameObject researchTargetObject;

		// Token: 0x04003D27 RID: 15655
		private bool selectiveMode;

		// Token: 0x04003D28 RID: 15656
		public List<ChildTechGridItemController> sortedTechList = new List<ChildTechGridItemController>();

		// Token: 0x04003D29 RID: 15657
		private List<bool> techVisited = new List<bool>();

		// Token: 0x04003D2A RID: 15658
		private int totalVisited;

		// Token: 0x04003D2B RID: 15659
		private int lastNode;

		// Token: 0x04003D2C RID: 15660
		private int endGameTechs;

		// Token: 0x04003D2D RID: 15661
		private readonly int nodeCountLimit = 29;

		// Token: 0x04003D2E RID: 15662
		private List<int> nodeCounts = new List<int>(13);

		// Token: 0x04003D2F RID: 15663
		private float contentHeight;

		// Token: 0x04003D30 RID: 15664
		private float contentWidth;

		// Token: 0x04003D31 RID: 15665
		public bool openingSelectiveTree;

		// Token: 0x04003D32 RID: 15666
		public float normalConnectionLineWidth = 1f;

		// Token: 0x04003D33 RID: 15667
		public float highlightedConnectionLineWidth = 2.6f;

		// Token: 0x04003D34 RID: 15668
		public Color32 techNameColorSelected = new Color32(250, 250, 50, byte.MaxValue);

		// Token: 0x04003D35 RID: 15669
		public Color32 techNameColorDeSelected = new Color32(207, 231, 232, byte.MaxValue);

		// Token: 0x04003D36 RID: 15670
		public Color32 connectionColorNeutral = new Color32(60, 88, 100, byte.MaxValue);

		// Token: 0x04003D37 RID: 15671
		public Color32 connectionColorDeSelected = new Color32(30, 44, 50, byte.MaxValue);

		// Token: 0x04003D38 RID: 15672
		public Color32 connectionColorDownstream = new Color32(byte.MaxValue, 100, 100, byte.MaxValue);

		// Token: 0x04003D39 RID: 15673
		public Color32 connectionColorUpstream = new Color32(212, 135, 32, byte.MaxValue);

		// Token: 0x04003D3A RID: 15674
		public Color32 connectionColorResearched = new Color32(100, 250, 100, byte.MaxValue);

		// Token: 0x04003D3B RID: 15675
		public Color32 connectionColorOrPrereq = new Color32(204, 201, 126, byte.MaxValue);

		// Token: 0x04003D3C RID: 15676
		public List<Sprite> techStatusGradient = new List<Sprite>();

		// Token: 0x04003D3D RID: 15677
		public List<Color32> techStatusIconColors = new List<Color32>();

		// Token: 0x04003D3E RID: 15678
		private static readonly Color32[] techStatusColor = new Color32[]
		{
			new Color32(87, 150, 87, byte.MaxValue),
			new Color32(30, 117, 198, byte.MaxValue),
			new Color32(70, 110, 140, byte.MaxValue),
			new Color32(140, 40, 40, byte.MaxValue),
			new Color32(87, 150, 87, byte.MaxValue),
			new Color32(30, 117, 198, byte.MaxValue),
			new Color32(70, 110, 140, byte.MaxValue),
			new Color32(140, 40, 40, byte.MaxValue),
			new Color32(87, 150, 87, byte.MaxValue),
			new Color32(70, 110, 140, byte.MaxValue),
			new Color32(23, 185, 185, byte.MaxValue),
			new Color32(12, 90, 90, byte.MaxValue)
		};

		// Token: 0x04003D3F RID: 15679
		private int lerpFrames = 20;

		// Token: 0x04003D40 RID: 15680
		private bool moving;

		// Token: 0x04003D41 RID: 15681
		private TIGenericTechTemplate cachedSelectedTech;

		// Token: 0x04003D42 RID: 15682
		private ChildTechGridItemController cachedItemController;

		// Token: 0x0200119B RID: 4507
		public enum techTreeType
		{
			// Token: 0x040067C9 RID: 26569
			techsOnly,
			// Token: 0x040067CA RID: 26570
			fullTree,
			// Token: 0x040067CB RID: 26571
			selectiveTree
		}

		// Token: 0x0200119C RID: 4508
		public enum SortProjectDataBy
		{
			// Token: 0x040067CD RID: 26573
			Name,
			// Token: 0x040067CE RID: 26574
			Category,
			// Token: 0x040067CF RID: 26575
			Cost
		}

		// Token: 0x0200119D RID: 4509
		public enum SortTechDataBy
		{
			// Token: 0x040067D1 RID: 26577
			Name,
			// Token: 0x040067D2 RID: 26578
			Category,
			// Token: 0x040067D3 RID: 26579
			Cost
		}
	}
}
