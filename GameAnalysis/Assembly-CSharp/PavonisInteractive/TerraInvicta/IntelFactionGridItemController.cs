using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200087B RID: 2171
	public class IntelFactionGridItemController : MonoBehaviour
	{
		// Token: 0x0600511F RID: 20767 RVA: 0x00237338 File Offset: 0x00235538
		public void Initialize(TIFactionState faction, IntelScreenController intelController)
		{
			this.faction = faction;
			this.intelController = intelController;
			this.factionColorBackgroundImage.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(faction.template.gradientPath);
			this.factionLeaderNameGradientImage.sprite = this.factionColorBackgroundImage.sprite;
			this.factionVictoryGradientImage.sprite = this.factionColorBackgroundImage.sprite;
			this.factionLeaderImage.sprite = GameControl.assetLoader.LoadAsset<Sprite>(faction.pathLeaderHeadPortrait);
			this.factionView = GameControl.control.activePlayer.GetViewofFaction(faction);
			this.factionName.SetText(faction.displayNameCapitalized);
			this.factionIcon.sprite = faction.factionIcon256UI;
			this.factionLeaderBackgroundIcon.sprite = this.factionIcon.sprite;
			this.councilorsTabTitle.SetText(Loc.T("UI.Intel.Councilors"));
			this.resourcesTabTitle.SetText(Loc.T("UI.Intel.Resources"));
			this.objectivesTabTitle.SetText(Loc.T("UI.Intel.Objectives"));
			this.relationsTabTitle.SetText(Loc.T("UI.Intel.Relations"));
			this.techTabTitle.SetText(Loc.T("UI.Intel.Tech"));
			this.councilorTab.gameObject.SetActive(true);
			this.resourcesTab.gameObject.SetActive(true);
			this.objectivesTab.gameObject.SetActive(true);
			this.relationsTab.gameObject.SetActive(true);
			this.projectsTab.gameObject.SetActive(true);
			this.councilorsTabButton.onClick.AddListener(new UnityAction(this.OnClickCouncilors));
			this.objectivesTabButton.onClick.AddListener(new UnityAction(this.OnClickObjectives));
			this.relationsTabButton.onClick.AddListener(new UnityAction(this.OnClickRelations));
			this.resourcesTabButton.onClick.AddListener(new UnityAction(this.OnClickResources));
			this.projectsTabButton.onClick.AddListener(new UnityAction(this.OnClickProjects));
			this.noKnownObjectivesText.SetText(Loc.T("UI.Intel.NoKnownObjectives"));
			this.cpSprite.color = faction.template.color;
			this.ignoringThisFactionTT.SetText("BodyText", Loc.T("UI.Intel.IgnoringFaction"));
			this.allowingCommsFromThisFactionTT.SetText("BodyText", Loc.T("UI.Intel.AllowingContact"));
			this.ignoringFactionDiplomacyTT.SetText("BodyText", Loc.T("UI.Intel.IgnoringDiplomacy"));
			this.allowingFactionDiplomacyTT.SetText("BodyText", Loc.T("UI.Intel.AllowingDiplomacy"));
			GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.OnCouncilCompositionChanged), null, null, true, false);
		}

		// Token: 0x06005120 RID: 20768 RVA: 0x00237600 File Offset: 0x00235800
		public void Refresh()
		{
			if (this.faction.defeated)
			{
				base.gameObject.SetActive(false);
				return;
			}
			this.factionLeader.SetText(Loc.T("UI.Intel.Leader", new object[] { this.factionView.fullLeader }));
			this.factionGoal.SetText(Loc.T("UI.Intel.Goal", new object[] { this.factionView.goal }));
			this.factionVictory.SetText(Loc.T("UI.Intel.Victory", new object[] { this.factionView.victory }));
			this.factionLeaderImage.enabled = this.factionView.showLeader;
			this.penetratedLabel.SetText(GameControl.control.activePlayer.intelSharingFactions.Contains(this.faction) ? Loc.T("UI.Intel.IntelSharing") : (GameControl.control.activePlayer.factionsCompromised.Contains(this.faction) ? Loc.T("UI.Intel.Penetrated") : string.Empty));
			if (GameControl.control.activePlayer.ignoreContacts.Contains(this.faction))
			{
				this.setIgnoreFactionButtonObject.SetActive(false);
				this.setAllowCommsFactionButtonObject.SetActive(true);
			}
			else
			{
				this.setIgnoreFactionButtonObject.SetActive(true);
				this.setAllowCommsFactionButtonObject.SetActive(false);
			}
			if (GameControl.control.activePlayer.ignoreInterstateDiplomacy.Contains(this.faction))
			{
				this.setIgnoreFactionDiplomacyButtonObject.SetActive(false);
				this.setAllowFactionDiplomacyButtonObject.SetActive(true);
			}
			else
			{
				this.setIgnoreFactionDiplomacyButtonObject.SetActive(true);
				this.setAllowFactionDiplomacyButtonObject.SetActive(false);
			}
			if (this.factionTabbedPaneManager.activeTab == this.councilorTab)
			{
				this.RefreshCouncilors();
			}
			else if (this.factionTabbedPaneManager.activeTab == this.objectivesTab)
			{
				this.RefreshObjectives();
			}
			else if (this.factionTabbedPaneManager.activeTab == this.relationsTab)
			{
				this.RefreshRelations();
			}
			else if (this.factionTabbedPaneManager.activeTab == this.resourcesTab)
			{
				this.RefreshResources();
			}
			else if (this.factionTabbedPaneManager.activeTab == this.projectsTab)
			{
				this.RefreshProjects();
			}
			base.gameObject.SetActive(true);
		}

		// Token: 0x06005121 RID: 20769 RVA: 0x00237860 File Offset: 0x00235A60
		private void RefreshResources()
		{
			this.money.SetText(this.factionView.GetResourceString(FactionResource.Money));
			this.influence.SetText(this.factionView.GetResourceString(FactionResource.Influence));
			this.ops.SetText(this.factionView.GetResourceString(FactionResource.Operations));
			this.boost.SetText(this.factionView.GetResourceString(FactionResource.Boost));
			this.missionControl.SetText(this.factionView.GetResourceString(FactionResource.MissionControl));
			this.research.SetText(this.factionView.GetResourceString(FactionResource.Research));
			this.projects.SetText(this.factionView.GetResourceString(FactionResource.Projects));
			this.controlPoints.SetText(GeneralControlsController.ControlPointMaintenanceString(this.faction));
			this.water.SetText(this.factionView.GetResourceString(FactionResource.Water));
			this.volatiles.SetText(this.factionView.GetResourceString(FactionResource.Volatiles));
			this.metals.SetText(this.factionView.GetResourceString(FactionResource.Metals));
			this.nobles.SetText(this.factionView.GetResourceString(FactionResource.NobleMetals));
			this.fertiles.SetText(this.factionView.GetResourceString(FactionResource.Fissiles));
			if (TIEffectsState.CheckForAnyEffectInContext(Context.CanAmassAntimatter, this.faction))
			{
				this.antimatterPanel.SetActive(true);
				this.antimatter.SetText(this.factionView.GetResourceString(FactionResource.Antimatter));
			}
			else
			{
				this.antimatterPanel.SetActive(false);
			}
			if (TIEffectsState.CheckForAnyEffectInContext(Context.CanAmassExotics, this.faction))
			{
				this.exoticsPanel.SetActive(true);
				this.exotics.SetText(this.factionView.GetResourceString(FactionResource.Exotics));
				return;
			}
			this.exoticsPanel.SetActive(false);
		}

		// Token: 0x06005122 RID: 20770 RVA: 0x00237A24 File Offset: 0x00235C24
		public void RefreshCouncilors()
		{
			this.councilorList.SetListSize<IntelCouncilorListItem>(this.faction.councilors.Count, this.faction.councilors.Count == 0, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.councilorList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelFactionGridItemController.<>o__66.<>p__0 == null)
					{
						IntelFactionGridItemController.<>o__66.<>p__0 = CallSite<Func<CallSite, object, IntelCouncilorListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelCouncilorListItem), typeof(IntelFactionGridItemController)));
					}
					IntelCouncilorListItem intelCouncilorListItem = IntelFactionGridItemController.<>o__66.<>p__0.Target(IntelFactionGridItemController.<>o__66.<>p__0, enumerator.Current);
					intelCouncilorListItem.Initialize(this.faction.councilors[num++], this.intelController);
					intelCouncilorListItem.UpdateListItem();
					intelCouncilorListItem.gameObject.SetActive(true);
				}
			}
		}

		// Token: 0x06005123 RID: 20771 RVA: 0x00237B0C File Offset: 0x00235D0C
		private void RefreshObjectives()
		{
			List<TIObjectiveTemplate> list = this.factionView.GetObjectives(ObjectiveType.Campaign, ObjectiveStatus.Unlocked);
			bool flag = false;
			if (list.Count == 0)
			{
				list = this.faction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked);
				if (list.Count > 0)
				{
					flag = true;
				}
			}
			this.objectivesList.SetListSize<IntelObjectiveListItemController>(list.Count, false, false);
			if (list.Count > 0)
			{
				this.noKnownObjectivesText.gameObject.SetActive(false);
				int num = 0;
				using (IEnumerator<object> enumerator = this.objectivesList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (IntelFactionGridItemController.<>o__67.<>p__0 == null)
						{
							IntelFactionGridItemController.<>o__67.<>p__0 = CallSite<Func<CallSite, object, IntelObjectiveListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelObjectiveListItemController), typeof(IntelFactionGridItemController)));
						}
						IntelFactionGridItemController.<>o__67.<>p__0.Target(IntelFactionGridItemController.<>o__67.<>p__0, enumerator.Current).SetListItem(list[num++], this.faction, flag);
					}
					return;
				}
			}
			this.noKnownObjectivesText.gameObject.SetActive(true);
		}

		// Token: 0x06005124 RID: 20772 RVA: 0x00237C1C File Offset: 0x00235E1C
		private void RefreshRelations()
		{
			List<TIFactionState> list = GameStateManager.AllFactions().ToList<TIFactionState>();
			list.Remove(this.faction);
			this.relationsGrid.SetListSize<IntelFactionRelationsGridItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.relationsGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelFactionGridItemController.<>o__68.<>p__0 == null)
					{
						IntelFactionGridItemController.<>o__68.<>p__0 = CallSite<Func<CallSite, object, IntelFactionRelationsGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionRelationsGridItemController), typeof(IntelFactionGridItemController)));
					}
					IntelFactionGridItemController.<>o__68.<>p__0.Target(IntelFactionGridItemController.<>o__68.<>p__0, enumerator.Current).SetListItem(this.faction, list[num++]);
				}
			}
		}

		// Token: 0x06005125 RID: 20773 RVA: 0x00237CE4 File Offset: 0x00235EE4
		private void RefreshProjects()
		{
			List<ProjectProgress> list = this.factionView.currentProjectProgress.Where<ProjectProgress>((ProjectProgress x) => x.accumulatedResearch > 0f).ToList<ProjectProgress>();
			List<TIProjectTemplate> list2 = this.factionView.completedProjectsDistinct.OrderByDescending<TIProjectTemplate, float>((TIProjectTemplate x) => x.GetResearchCost(this.faction)).ToList<TIProjectTemplate>();
			int num = list.Count + list2.Count;
			this.projectsList.SetListSize<IntelProjectsListItemController>(num, false, false);
			this.projectListItems = new IntelProjectsListItemController[num];
			int num2 = 0;
			List<TIProjectTemplate> list3 = this.faction.StealableProjects(GameControl.control.activePlayer);
			List<TIProjectTemplate> list4 = this.faction.ProjectsVulnerableToSabotage(GameControl.control.activePlayer);
			using (IEnumerator<object> enumerator = this.projectsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelFactionGridItemController.<>o__69.<>p__0 == null)
					{
						IntelFactionGridItemController.<>o__69.<>p__0 = CallSite<Func<CallSite, object, IntelProjectsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelProjectsListItemController), typeof(IntelFactionGridItemController)));
					}
					IntelProjectsListItemController intelProjectsListItemController = IntelFactionGridItemController.<>o__69.<>p__0.Target(IntelFactionGridItemController.<>o__69.<>p__0, enumerator.Current);
					this.projectListItems[num2] = intelProjectsListItemController;
					if (num2 < list.Count)
					{
						bool flag = !this.faction.ProjectPaused(list[num2].projectTemplate);
						intelProjectsListItemController.SetListItem(list[num2].projectTemplate, true, flag, flag ? this.faction.researchWeights[list[num2].slot] : 0, list[num2].accumulatedResearch, list[num2].projectTemplate.GetResearchCost(this.faction), null, list4);
					}
					else
					{
						intelProjectsListItemController.SetListItem(list2[num2 - list.Count], false, false, 0, 0f, 1f, list3, null);
					}
					num2++;
				}
			}
		}

		// Token: 0x06005126 RID: 20774 RVA: 0x00237EE8 File Offset: 0x002360E8
		private void OnCouncilCompositionChanged(CouncilCompositionChanged e)
		{
			if (this.intelController.Canvas.enabled && e.council == this.faction && this.factionTabbedPaneManager.activeTab == this.councilorTab)
			{
				this.RefreshCouncilors();
			}
		}

		// Token: 0x06005127 RID: 20775 RVA: 0x00237F38 File Offset: 0x00236138
		private void OnClickRelations()
		{
			if (TIInputManager.IsShiftKeyDown)
			{
				this.intelController.ShowAllRelationsTabs();
			}
			if (!TIInputManager.IsShiftKeyDown)
			{
				this.RefreshRelations();
			}
		}

		// Token: 0x06005128 RID: 20776 RVA: 0x00237F59 File Offset: 0x00236159
		private void OnClickObjectives()
		{
			if (TIInputManager.IsShiftKeyDown)
			{
				this.intelController.ShowAllObjectivesTabs();
			}
			if (!TIInputManager.IsShiftKeyDown)
			{
				this.RefreshObjectives();
			}
		}

		// Token: 0x06005129 RID: 20777 RVA: 0x00237F7A File Offset: 0x0023617A
		private void OnClickResources()
		{
			if (TIInputManager.IsShiftKeyDown)
			{
				this.intelController.ShowAllResourcesTabs();
			}
			if (!TIInputManager.IsShiftKeyDown)
			{
				this.RefreshResources();
			}
		}

		// Token: 0x0600512A RID: 20778 RVA: 0x00237F9B File Offset: 0x0023619B
		private void OnClickProjects()
		{
			if (TIInputManager.IsShiftKeyDown)
			{
				this.intelController.ShowAllProjectsTabs();
			}
			if (!TIInputManager.IsShiftKeyDown)
			{
				this.RefreshProjects();
			}
		}

		// Token: 0x0600512B RID: 20779 RVA: 0x00237FBC File Offset: 0x002361BC
		private void OnClickCouncilors()
		{
			if (TIInputManager.IsShiftKeyDown)
			{
				this.intelController.ShowAllCouncilorTabs();
			}
			if (!TIInputManager.IsShiftKeyDown)
			{
				this.RefreshCouncilors();
			}
		}

		// Token: 0x0600512C RID: 20780 RVA: 0x00237FDD File Offset: 0x002361DD
		public void OnLeaderImageClicked()
		{
			if (this.factionView.showLeader)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
				this.intelController.UpdateLeaderPopup(this.faction);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x0600512D RID: 20781 RVA: 0x00238018 File Offset: 0x00236218
		public void OnIgnoreFaction()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			GameControl.control.activePlayer.playerControl.StartAction(new SetIgnoreFactionContactAction(GameControl.control.activePlayer, this.faction, true));
			this.setIgnoreFactionButtonObject.SetActive(false);
			this.setAllowCommsFactionButtonObject.SetActive(true);
		}

		// Token: 0x0600512E RID: 20782 RVA: 0x00238074 File Offset: 0x00236274
		public void OnAllowFactionContacts()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			GameControl.control.activePlayer.playerControl.StartAction(new SetIgnoreFactionContactAction(GameControl.control.activePlayer, this.faction, false));
			this.setIgnoreFactionButtonObject.SetActive(true);
			this.setAllowCommsFactionButtonObject.SetActive(false);
		}

		// Token: 0x0600512F RID: 20783 RVA: 0x002380D0 File Offset: 0x002362D0
		public void OnIgnoreFactionNationDiplo()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			GameControl.control.activePlayer.playerControl.StartAction(new SetIgnoreFactionInterstateDiplomacyAction(GameControl.control.activePlayer, this.faction, true));
			this.setIgnoreFactionDiplomacyButtonObject.SetActive(false);
			this.setAllowFactionDiplomacyButtonObject.SetActive(true);
		}

		// Token: 0x06005130 RID: 20784 RVA: 0x0023812C File Offset: 0x0023632C
		public void OnAllowFactionNationDiplo()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			GameControl.control.activePlayer.playerControl.StartAction(new SetIgnoreFactionInterstateDiplomacyAction(GameControl.control.activePlayer, this.faction, false));
			this.setIgnoreFactionDiplomacyButtonObject.SetActive(true);
			this.setAllowFactionDiplomacyButtonObject.SetActive(false);
		}

		// Token: 0x040034F6 RID: 13558
		public TMP_Text factionName;

		// Token: 0x040034F7 RID: 13559
		public Image factionIcon;

		// Token: 0x040034F8 RID: 13560
		public Image factionLeaderBackgroundIcon;

		// Token: 0x040034F9 RID: 13561
		public TMP_Text factionLeader;

		// Token: 0x040034FA RID: 13562
		public TMP_Text factionGoal;

		// Token: 0x040034FB RID: 13563
		public TMP_Text factionVictory;

		// Token: 0x040034FC RID: 13564
		public Image factionColorBackgroundImage;

		// Token: 0x040034FD RID: 13565
		public Image factionLeaderNameGradientImage;

		// Token: 0x040034FE RID: 13566
		public Image factionVictoryGradientImage;

		// Token: 0x040034FF RID: 13567
		public Image factionLeaderImage;

		// Token: 0x04003500 RID: 13568
		public TMP_Text penetratedLabel;

		// Token: 0x04003501 RID: 13569
		public TMP_Text councilorsTabTitle;

		// Token: 0x04003502 RID: 13570
		public TMP_Text resourcesTabTitle;

		// Token: 0x04003503 RID: 13571
		public TMP_Text objectivesTabTitle;

		// Token: 0x04003504 RID: 13572
		public TMP_Text relationsTabTitle;

		// Token: 0x04003505 RID: 13573
		public TMP_Text techTabTitle;

		// Token: 0x04003506 RID: 13574
		public TabbedPaneController councilorTab;

		// Token: 0x04003507 RID: 13575
		public TabbedPaneController resourcesTab;

		// Token: 0x04003508 RID: 13576
		public TabbedPaneController objectivesTab;

		// Token: 0x04003509 RID: 13577
		public TabbedPaneController relationsTab;

		// Token: 0x0400350A RID: 13578
		public TabbedPaneController projectsTab;

		// Token: 0x0400350B RID: 13579
		public TabbedPaneManager factionTabbedPaneManager;

		// Token: 0x0400350C RID: 13580
		public Button councilorsTabButton;

		// Token: 0x0400350D RID: 13581
		public Button resourcesTabButton;

		// Token: 0x0400350E RID: 13582
		public Button objectivesTabButton;

		// Token: 0x0400350F RID: 13583
		public Button relationsTabButton;

		// Token: 0x04003510 RID: 13584
		public Button projectsTabButton;

		// Token: 0x04003511 RID: 13585
		public GameObject setIgnoreFactionButtonObject;

		// Token: 0x04003512 RID: 13586
		public GameObject setAllowCommsFactionButtonObject;

		// Token: 0x04003513 RID: 13587
		public TooltipTrigger ignoringThisFactionTT;

		// Token: 0x04003514 RID: 13588
		public TooltipTrigger allowingCommsFromThisFactionTT;

		// Token: 0x04003515 RID: 13589
		public GameObject setIgnoreFactionDiplomacyButtonObject;

		// Token: 0x04003516 RID: 13590
		public GameObject setAllowFactionDiplomacyButtonObject;

		// Token: 0x04003517 RID: 13591
		public TooltipTrigger ignoringFactionDiplomacyTT;

		// Token: 0x04003518 RID: 13592
		public TooltipTrigger allowingFactionDiplomacyTT;

		// Token: 0x04003519 RID: 13593
		[Header("Councilor Tab")]
		public ListManagerBase councilorList;

		// Token: 0x0400351A RID: 13594
		private IntelCouncilorListItem[] councilorListItems;

		// Token: 0x0400351B RID: 13595
		[Header("Resources Tab")]
		public TMP_Text money;

		// Token: 0x0400351C RID: 13596
		public TMP_Text influence;

		// Token: 0x0400351D RID: 13597
		public TMP_Text ops;

		// Token: 0x0400351E RID: 13598
		public TMP_Text boost;

		// Token: 0x0400351F RID: 13599
		public TMP_Text missionControl;

		// Token: 0x04003520 RID: 13600
		public TMP_Text research;

		// Token: 0x04003521 RID: 13601
		public TMP_Text projects;

		// Token: 0x04003522 RID: 13602
		public TMP_Text water;

		// Token: 0x04003523 RID: 13603
		public TMP_Text volatiles;

		// Token: 0x04003524 RID: 13604
		public TMP_Text metals;

		// Token: 0x04003525 RID: 13605
		public TMP_Text nobles;

		// Token: 0x04003526 RID: 13606
		public TMP_Text fertiles;

		// Token: 0x04003527 RID: 13607
		public TMP_Text antimatter;

		// Token: 0x04003528 RID: 13608
		public TMP_Text exotics;

		// Token: 0x04003529 RID: 13609
		public TMP_Text controlPoints;

		// Token: 0x0400352A RID: 13610
		public Image cpSprite;

		// Token: 0x0400352B RID: 13611
		public GameObject antimatterPanel;

		// Token: 0x0400352C RID: 13612
		public GameObject exoticsPanel;

		// Token: 0x0400352D RID: 13613
		[Header("Objectives Tab")]
		public ListManagerBase objectivesList;

		// Token: 0x0400352E RID: 13614
		public TMP_Text noKnownObjectivesText;

		// Token: 0x0400352F RID: 13615
		[Header("ProjectsTab")]
		public ListManagerBase projectsList;

		// Token: 0x04003530 RID: 13616
		private IntelProjectsListItemController[] projectListItems;

		// Token: 0x04003531 RID: 13617
		[Header("RelationsTab")]
		public ListManagerBase relationsGrid;

		// Token: 0x04003532 RID: 13618
		private TIFactionState faction;

		// Token: 0x04003533 RID: 13619
		private FactionView factionView;

		// Token: 0x04003534 RID: 13620
		private IntelScreenController intelController;
	}
}
