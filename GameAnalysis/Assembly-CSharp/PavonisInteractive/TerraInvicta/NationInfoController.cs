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
	// Token: 0x02000891 RID: 2193
	public class NationInfoController : CanvasControllerBase
	{
		// Token: 0x17000EE8 RID: 3816
		// (get) Token: 0x060051F9 RID: 20985 RVA: 0x002405DA File Offset: 0x0023E7DA
		// (set) Token: 0x060051FA RID: 20986 RVA: 0x002405E2 File Offset: 0x0023E7E2
		public int proportionColumnSetting { get; protected set; }

		// Token: 0x17000EE9 RID: 3817
		// (get) Token: 0x060051FB RID: 20987 RVA: 0x002405EB File Offset: 0x0023E7EB
		// (set) Token: 0x060051FC RID: 20988 RVA: 0x002405F3 File Offset: 0x0023E7F3
		public TINationState nation { get; private set; }

		// Token: 0x060051FD RID: 20989 RVA: 0x002405FC File Offset: 0x0023E7FC
		public override void Initialize()
		{
			base.Initialize();
			this.nationPanelCanvas.enabled = false;
			this.nationPanelCanvas.gameObject.SetActive(true);
			this.mapObjectDetailCanvas.enabled = false;
			this.mapObjectDetailCanvas.gameObject.SetActive(true);
			this.mapObjectButtonPanelObject.SetActive(false);
			this.confirmDisableControlPointPanel.SetActive(false);
			this.armyTabTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.ArmiesTooltip", new object[] { TemplateManager.global.navyInlineSpritePath }));
			this.policiesTabText.SetText(Loc.T("UI.Nation.PoliciesTab"));
			this.policiesTabTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.PoliciesDescription", new object[] { string.Empty }));
			this.prioritiesTabTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.PrioritiesTooltip"));
			this.manageRelationsButtonTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.ClickToManageRelations"));
			this.regionTabTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.RegionsTooltip"));
			this.councilorsTabText.SetText(Loc.T("UI.Nation.CouncilorsTab"));
			this.councilorTabTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.CouncilorsTooltip"));
			this.relationsTabText.SetText(Loc.T("UI.Nation.RelationsTab"));
			this.relationsTabTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.RelationsTooltip"));
			this.occupationHeaderTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.OccupationTooltip"));
			this.boostHeaderTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.BoostTooltip"));
			this.MCHeaderTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.MissionControlTooltip"));
			this.claimsHeaderTooltipTrigger.SetText("BodyText", Loc.T("UI.Nation.ClaimsTooltip"));
			this.populationHeaderText.SetText(Loc.T("UI.Nation.PopulationHeader"));
			this.claimsHeaderText.SetText(Loc.T("UI.Nation.ClaimsHeader"));
			this.overviewHeaderText.SetText(Loc.T("UI.Nation.Overview"));
			this.militaryHeaderText.SetText(Loc.T("UI.Nation.Military"));
			this.developmentHeaderText.SetText(Loc.T("UI.Nation.Development"));
			this.peopleHeaderText.SetText(Loc.T("UI.Nation.People"));
			this.publicOpinionText.SetText(Loc.T("UI.Nation.PublicOpinion"));
			this.priorityHeader1.SetText(Loc.T("UI.Nation.Priority"));
			this.priorityHeader2.SetText(Loc.T("UI.Nation.Progress"));
			this.directInvestButtonText.SetText(Loc.T("UI.Nation.DirectInvestButtonText"));
			this.alliesHeader.SetText(Loc.T("UI.Nation.Allies"));
			this.rivalsHeaders.SetText(Loc.T("UI.Nation.Rivals"));
			this.warsHeader.SetText(Loc.T("UI.Nation.Wars"));
			this.manageRelationsButtonText.SetText(Loc.T("UI.Nation.Manage"));
			this.prioritiesTabText.SetText(Loc.T("UI.Nation.PrioritiesTab"));
			this.disableControlPointsButtonText.SetText(Loc.T("UI.Nation.DisableControlPointsButton"));
			this.disbaleControlPointsTip.SetText("BodyText", Loc.T("UI.Nation.DisableControlPointTip"));
			this.confirmDisableControlPointHeaderText.SetText(Loc.T("UI.Nation.DisableControlPointsWindowHeader"));
			this.confirmDisableControlConfirmButtonText.SetText(Loc.T("UI.Nation.DisableControlPointConfirm"));
			this.confirmDisableControlCancelButtonText.SetText(Loc.T("UI.Nation.DisableControlPointCancel"));
			this.autoAbandonToggleText.SetText(Loc.T("UI.Nation.AutoAbandonToggleText"));
			this.directInvestPanel.SetActive(false);
			GameControl.eventManager.AddListener<RegionStateSelected>(new EventManager.EventDelegate<RegionStateSelected>(this.ShowNationPanel), null, null, false, false);
			GameControl.eventManager.AddListener<ControlPointTargetSelected>(new EventManager.EventDelegate<ControlPointTargetSelected>(this.ShowNationPanel), null, null, false, false);
			GameControl.eventManager.AddListener<SpaceFacilityMapObjectSelected>(new EventManager.EventDelegate<SpaceFacilityMapObjectSelected>(this.ShowRegionMapObjectPanel), null, null, false, false);
			GameControl.eventManager.AddListener<AlienRegionMapEntitySelected>(new EventManager.EventDelegate<AlienRegionMapEntitySelected>(this.ShowRegionMapObjectPanel), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetOpenControlPoint>(new EventManager.EventDelegate<DeTargetOpenControlPoint>(this.OnDeTargetOpenControlPoints), null, null, false, false);
			GameControl.eventManager.AddListener<TargetControlPoints>(new EventManager.EventDelegate<TargetControlPoints>(this.OnTargetControlPoints), null, null, false, false);
			GameControl.eventManager.AddListener<TargetOpenControlPoint>(new EventManager.EventDelegate<TargetOpenControlPoint>(this.OnTargetOpenControlPoint), null, null, false, false);
			GameControl.eventManager.AddListener<DeTargetControlPoints>(new EventManager.EventDelegate<DeTargetControlPoints>(this.OnDeTargetControlPoints), null, null, false, false);
			GameControl.eventManager.AddListener<NationIPManagerRequested>(new EventManager.EventDelegate<NationIPManagerRequested>(this.OnNationIPManagerRequested), null, null, true, false);
			this.InitializeDirectInvestPanel();
			NationInfoController.weightStr[0] = TIUtilities.BlackLine(Loc.T("UI.Nation.Weight0"));
			NationInfoController.weightStr[1] = TIUtilities.RedLine(Loc.T("UI.Nation.Weight1"));
			NationInfoController.weightStr[2] = TIUtilities.BlueLine(Loc.T("UI.Nation.Weight2"));
			NationInfoController.weightStr[3] = TIUtilities.GreenLine(Loc.T("UI.Nation.Weight3"));
			NationInfoController.weightSprite[0] = GameControl.assetLoader.LoadAssetForSpriteAssignment("icons_2d/ICO_Weight0_priority");
			NationInfoController.weightSprite[1] = GameControl.assetLoader.LoadAssetForSpriteAssignment("icons_2d/ICO_Weight1_priority");
			NationInfoController.weightSprite[2] = GameControl.assetLoader.LoadAssetForSpriteAssignment("icons_2d/ICO_Weight2_priority");
			NationInfoController.weightSprite[3] = GameControl.assetLoader.LoadAssetForSpriteAssignment("icons_2d/ICO_Weight3_priority");
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.NationDetail, new Action(this.CloseNationPanel));
			base.canvasManager.RegisterInfoPanelDisableOrder(InfoPanel.EarthMapObjectDetail, new Action(this.CloseMapObjectPanel));
			this.SetTooltips();
			this.nationIdeologyPortions[0].fillAmount = 1f;
			this.nationIdeologyPortions[0].color = Color.gray;
			this.nationIdeologyPortions[0].enabled = true;
			for (int i = 1; i < this.nationIdeologyPortions.Count; i++)
			{
				this.nationIdeologyPortions[i].enabled = false;
			}
			this.priorityList.SetListSize<PriorityListItemController>(Enums.PriorityTypes.Length, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.priorityList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__194.<>p__0 == null)
					{
						NationInfoController.<>o__194.<>p__0 = CallSite<Func<CallSite, object, PriorityListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PriorityListItemController), typeof(NationInfoController)));
					}
					NationInfoController.<>o__194.<>p__0.Target(NationInfoController.<>o__194.<>p__0, enumerator.Current).Init(this, Enums.PriorityTypes[num++]);
				}
			}
			this.InitializeRelationsPanel();
			this.InitNuclearOption();
			this.InitializeDesignPresetPanel();
		}

		// Token: 0x060051FE RID: 20990 RVA: 0x00240C98 File Offset: 0x0023EE98
		public override void Show()
		{
			base.Show();
			this.mapObjectDetailCanvas.gameObject.SetActive(true);
			this.Refresh();
		}

		// Token: 0x060051FF RID: 20991 RVA: 0x00240CB7 File Offset: 0x0023EEB7
		public override void Hide()
		{
			this.nationPanelCanvas.enabled = false;
			this.mapObjectDetailCanvas.gameObject.SetActive(false);
			this.HideTooltips();
			this.HideTutorials();
			base.Hide();
		}

		// Token: 0x06005200 RID: 20992 RVA: 0x00240CE8 File Offset: 0x0023EEE8
		public override void Refresh()
		{
			if (this.nationPanelCanvas.enabled)
			{
				TINationState nation = this.nation;
				if (nation == null || !nation.extant)
				{
					this.CloseAnySecondaryPanels(null, false);
					this.Hide();
					this.nationDataDirty = false;
					this.councilorListDataDirty = false;
					return;
				}
				if (this.nationDataDirty)
				{
					this.UpdateNationPanel();
				}
				else if (this.councilorListDataDirty)
				{
					this.UpdateCouncilorList();
				}
				if (!base.Paused)
				{
					if (this.timeToNextUpdate_s <= 0f)
					{
						this.UpdateNationPanel();
						this.timeToNextUpdate_s = 15f;
					}
					else
					{
						this.timeToNextUpdate_s -= Time.unscaledDeltaTime;
					}
				}
			}
			this.nationDataDirty = false;
			this.councilorListDataDirty = false;
		}

		// Token: 0x06005201 RID: 20993 RVA: 0x00240DA0 File Offset: 0x0023EFA0
		public bool CloseAnySecondaryPanels(GameObject exceptPanel, bool allowGeneric)
		{
			bool flag = false;
			if (this.nationRelationsManagerPanel != null && this.nationRelationsManagerPanel.activeSelf && exceptPanel != this.nationRelationsManagerPanel)
			{
				this.CloseRelationsPanel();
				flag = true;
			}
			if (this.directInvestPanel != null && this.directInvestPanel.activeSelf && exceptPanel != this.directInvestPanel)
			{
				this.CloseDirectInvestPanel();
				flag = true;
			}
			if (this.designPresetPanel != null && this.designPresetPanel.activeSelf && !allowGeneric && exceptPanel != this.designPresetPanel)
			{
				this.CloseDesignPresetPanel();
				flag = true;
			}
			if (this.confirmDisableControlPointPanel != null && this.confirmDisableControlPointPanel.activeSelf && exceptPanel != this.confirmDisableControlPointPanel)
			{
				this.CloseSelfDisablePanel();
				flag = true;
			}
			if (this.nuclearWeaponsPanel != null && this.nuclearWeaponsPanel.activeSelf && exceptPanel != this.nuclearWeaponsPanel && (!this.currentlyNuclearTargeting || this.nuclearTargeting.GetPossibleTargets.Count == 0))
			{
				this.CloseNuclearWeaponsPanel();
				flag = true;
			}
			return flag;
		}

		// Token: 0x06005202 RID: 20994 RVA: 0x00240EC2 File Offset: 0x0023F0C2
		public void ForceShowNationPanel(TIRegionState region)
		{
			this.ShowNationPanel(region);
		}

		// Token: 0x06005203 RID: 20995 RVA: 0x00240ECB File Offset: 0x0023F0CB
		private void ShowNationPanel(ControlPointTargetSelected e)
		{
			this.ShowNationPanel(e.controlPoint.ref_region);
		}

		// Token: 0x06005204 RID: 20996 RVA: 0x00240EDE File Offset: 0x0023F0DE
		private void ShowNationPanel(RegionStateSelected e)
		{
			this.ShowNationPanel(e.region);
		}

		// Token: 0x06005205 RID: 20997 RVA: 0x00240EEC File Offset: 0x0023F0EC
		private void ShowRegionMapObjectPanel(SpaceFacilityMapObjectSelected e)
		{
			this.ShowMapObjectPanel(e.regionSpaceFacility);
		}

		// Token: 0x06005206 RID: 20998 RVA: 0x00240EFA File Offset: 0x0023F0FA
		private void ShowRegionMapObjectPanel(AlienRegionMapEntitySelected e)
		{
			this.ShowMapObjectPanel(e.alienEntity);
		}

		// Token: 0x06005207 RID: 20999 RVA: 0x00240F08 File Offset: 0x0023F108
		private void UpdateNationPanel(NationDataUpdated e)
		{
			this.nationDataDirty = true;
		}

		// Token: 0x06005208 RID: 21000 RVA: 0x00240F14 File Offset: 0x0023F114
		private void UpdateNationPanel(ControlPointDataUpdated e)
		{
			this.nationDataDirty = true;
			if (!TIGameState.Valid(e.controlPoint) || e.controlPoint.nation == null || e.controlPoint.executive)
			{
				if (this.nationRelationsManagerPanel.activeInHierarchy)
				{
					this.CloseRelationsPanel();
				}
				if (this.nuclearWeaponsPanel.activeInHierarchy)
				{
					this.CloseNuclearWeaponsPanel();
				}
			}
		}

		// Token: 0x06005209 RID: 21001 RVA: 0x00240F7E File Offset: 0x0023F17E
		private void UpdateNationPanel(CustomPriorityPresetsChanged e)
		{
			this.nationDataDirty = true;
		}

		// Token: 0x0600520A RID: 21002 RVA: 0x00240F87 File Offset: 0x0023F187
		private void UpdateCouncilorList(CouncilCompositionChanged e)
		{
			if (this.nation != null)
			{
				TIGameState location = e.location;
				if (((location != null) ? location.ref_nation : null) == this.nation)
				{
					this.councilorListDataDirty = true;
				}
			}
		}

		// Token: 0x0600520B RID: 21003 RVA: 0x00240FBD File Offset: 0x0023F1BD
		private void UpdateCouncilorList(CouncilorMissionUpdated e)
		{
			if (e.councilor.OnEarth && e.councilor.ref_nation == this.nation && this.nation != null)
			{
				this.councilorListDataDirty = true;
			}
		}

		// Token: 0x0600520C RID: 21004 RVA: 0x00240FF9 File Offset: 0x0023F1F9
		private void UpdateCouncilorList(CouncilorPositionUpdated e)
		{
			if (e.location.ref_nation == this.nation && this.nation != null)
			{
				this.councilorListDataDirty = true;
			}
		}

		// Token: 0x0600520D RID: 21005 RVA: 0x00241028 File Offset: 0x0023F228
		private void UpdateCouncilorList(CouncilorDepartsRegion e)
		{
			if (e.region.ref_nation == this.nation && this.nation != null)
			{
				this.councilorListDataDirty = true;
			}
		}

		// Token: 0x0600520E RID: 21006 RVA: 0x00241057 File Offset: 0x0023F257
		private void UpdateMapObjectPanel(RegionDataUpdated e)
		{
			if (this.mapObjectDetailCanvas.enabled && e.region == this.displayedRegionLocationState.ref_region)
			{
				this.UpdateMapObjectPanel(this.displayedRegionLocationState);
			}
		}

		// Token: 0x0600520F RID: 21007 RVA: 0x0024108A File Offset: 0x0023F28A
		private void AutocloseNationPanel(InfoScreenOpened e)
		{
			if (this.Visible() && this.nationPanelCanvas.enabled)
			{
				this.CloseAnySecondaryPanels(null, false);
				this.Hide();
				GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreNationPanel), null, null, true, false);
			}
		}

		// Token: 0x06005210 RID: 21008 RVA: 0x002410CA File Offset: 0x0023F2CA
		private void AutocloseMapObjectPanel(InfoScreenOpened e)
		{
			if (this.Visible() && this.mapObjectDetailCanvas.enabled)
			{
				this.Hide();
				GameControl.eventManager.AddListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreMapObjectPanel), null, null, true, false);
			}
		}

		// Token: 0x06005211 RID: 21009 RVA: 0x00241104 File Offset: 0x0023F304
		private void RestoreNationPanel(InfoScreenClosed e)
		{
			this.Show();
			if (this.region != null && GameControl.control.viewMgr.currentView == ViewType.PoliticalMap)
			{
				this.ShowNationPanel(this.region);
			}
			this.StartNationPanelTutorial();
			if (this.nationTabManager.activeTab == this.prioritiesTabController)
			{
				this.StartPrioritiesTutorial();
			}
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreNationPanel), null);
		}

		// Token: 0x06005212 RID: 21010 RVA: 0x0024117E File Offset: 0x0023F37E
		private void RestoreMapObjectPanel(InfoScreenClosed e)
		{
			this.Show();
			GameControl.eventManager.RemoveListener<InfoScreenClosed>(new EventManager.EventDelegate<InfoScreenClosed>(this.RestoreMapObjectPanel), null);
		}

		// Token: 0x06005213 RID: 21011 RVA: 0x0024119D File Offset: 0x0023F39D
		private void OnTargetControlPoints(TargetControlPoints e)
		{
			this.targetingOwnedCPs = true;
			this.currentMission = e.missionTemplate;
			this.currentMissionCouncilor = e.councilor;
			if (this.Visible() && this.nationPanelCanvas.enabled)
			{
				this.AssignControlPoints();
			}
		}

		// Token: 0x06005214 RID: 21012 RVA: 0x002411D9 File Offset: 0x0023F3D9
		private void OnDeTargetControlPoints(DeTargetControlPoints e)
		{
			this.targetingOwnedCPs = false;
			this.currentMission = null;
			this.currentMissionCouncilor = null;
			if (this.Visible() && this.nationPanelCanvas.enabled)
			{
				this.AssignControlPoints();
			}
		}

		// Token: 0x06005215 RID: 21013 RVA: 0x0024120B File Offset: 0x0023F40B
		private void OnTargetOpenControlPoint(TargetOpenControlPoint e)
		{
			this.targetingNeutralCP = true;
			this.currentMission = e.missionTemplate;
			this.currentMissionCouncilor = e.councilor;
			if (this.Visible() && this.nationPanelCanvas.enabled)
			{
				this.AssignControlPoints();
			}
		}

		// Token: 0x06005216 RID: 21014 RVA: 0x00241247 File Offset: 0x0023F447
		private void OnDeTargetOpenControlPoints(DeTargetOpenControlPoint e)
		{
			this.targetingNeutralCP = false;
			this.currentMission = null;
			this.currentMissionCouncilor = null;
			if (this.Visible() && this.nationPanelCanvas.enabled)
			{
				this.AssignControlPoints();
			}
		}

		// Token: 0x06005217 RID: 21015 RVA: 0x00241279 File Offset: 0x0023F479
		private void OnNationIPManagerRequested(NationIPManagerRequested e)
		{
			this.ForceShowNationPanel(e.region);
			if (this.nationTabManager.activeTab != this.prioritiesTab)
			{
				this.nationTabManager.Toggle(this.prioritiesTab);
			}
		}

		// Token: 0x06005218 RID: 21016 RVA: 0x002412B0 File Offset: 0x0023F4B0
		public void FlagClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_RegionSelect", false, false);
			TIUtilities.GotoGameState(this.region, true, true, true, true, false, -1f);
		}

		// Token: 0x06005219 RID: 21017 RVA: 0x002412D3 File Offset: 0x0023F4D3
		public void EarthClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_NaturalSpaceObjectSelect", false, false);
			TIUtilities.GotoGameState(this.region.spaceBody, true, true, true, true, false, -1f);
		}

		// Token: 0x0600521A RID: 21018 RVA: 0x002412FB File Offset: 0x0023F4FB
		public void ExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x0600521B RID: 21019 RVA: 0x0024131C File Offset: 0x0023F51C
		private void CheckforMainCanvasClose()
		{
			if (this.nationPanelCanvas != null && !this.nationPanelCanvas.enabled && this.mapObjectDetailCanvas != null && !this.mapObjectDetailCanvas.enabled)
			{
				this.Hide();
				GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.AutocloseNationPanel), null);
			}
		}

		// Token: 0x0600521C RID: 21020 RVA: 0x0024137C File Offset: 0x0023F57C
		private void AddNationPanelListeners()
		{
			GameControl.eventManager.AddListener<NationDataUpdated>(new EventManager.EventDelegate<NationDataUpdated>(this.UpdateNationPanel), null, this.nation, true, false);
			GameControl.eventManager.AddListener<ControlPointDataUpdated>(new EventManager.EventDelegate<ControlPointDataUpdated>(this.UpdateNationPanel), null, this.nation, true, false);
			GameControl.eventManager.AddListener<CustomPriorityPresetsChanged>(new EventManager.EventDelegate<CustomPriorityPresetsChanged>(this.UpdateNationPanel), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCouncilorList), null, null, true, false);
			GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateCouncilorList), null, this.nation, true, false);
			GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateCouncilorList), null, this.nation, true, false);
			GameControl.eventManager.AddListener<CouncilorDepartsRegion>(new EventManager.EventDelegate<CouncilorDepartsRegion>(this.UpdateCouncilorList), null, this.nation, true, false);
			GameControl.eventManager.AddListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnArmyMajorStatusUpdate), null, this.nation, false, false);
		}

		// Token: 0x0600521D RID: 21021 RVA: 0x00241478 File Offset: 0x0023F678
		private void RemoveNationPanelListeners()
		{
			GameControl.eventManager.RemoveListener<NationDataUpdated>(new EventManager.EventDelegate<NationDataUpdated>(this.UpdateNationPanel), null);
			GameControl.eventManager.RemoveListener<ControlPointDataUpdated>(new EventManager.EventDelegate<ControlPointDataUpdated>(this.UpdateNationPanel), null);
			GameControl.eventManager.RemoveListener<CustomPriorityPresetsChanged>(new EventManager.EventDelegate<CustomPriorityPresetsChanged>(this.UpdateNationPanel), null);
			GameControl.eventManager.RemoveListener<CouncilCompositionChanged>(new EventManager.EventDelegate<CouncilCompositionChanged>(this.UpdateCouncilorList), null);
			GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateCouncilorList), null);
			GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateCouncilorList), null);
			GameControl.eventManager.RemoveListener<CouncilorDepartsRegion>(new EventManager.EventDelegate<CouncilorDepartsRegion>(this.UpdateCouncilorList), null);
			GameControl.eventManager.RemoveListener<ArmyMajorStatusUpdate>(new EventManager.EventDelegate<ArmyMajorStatusUpdate>(this.OnArmyMajorStatusUpdate), null);
		}

		// Token: 0x0600521E RID: 21022 RVA: 0x00241540 File Offset: 0x0023F740
		private void ShowNationPanel(TIRegionState region)
		{
			TIRegionState tiregionState = this.region;
			TINationState nation = this.nation;
			this.RemoveNationPanelListeners();
			this.region = region;
			this.nation = region.nation;
			this.AddNationPanelListeners();
			if (!this.Visible() || !this.nationPanelCanvas.enabled)
			{
				this.UpdateNationPanel();
				if (!this.Visible())
				{
					this.Show();
				}
				this.nationPanelCanvas.enabled = true;
				base.canvasManager.SetActiveInfoPanel(InfoPanel.NationDetail, 0f);
				GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.AutocloseNationPanel), null, null, true, false);
			}
			else if (tiregionState != region || nation != this.nation)
			{
				this.UpdateNationPanel();
			}
			this.StartNationPanelTutorial();
			if (this.nationTabManager.activeTab == this.prioritiesTabController)
			{
				this.StartPrioritiesTutorial();
			}
			if (nation != this.nation)
			{
				this.CloseAnySecondaryPanels(this.nuclearWeaponsPanel, true);
			}
		}

		// Token: 0x0600521F RID: 21023 RVA: 0x0024163C File Offset: 0x0023F83C
		private void CloseNationPanel()
		{
			this.RemoveNationPanelListeners();
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.nation);
			GeneralControlsController.ConditionalCancelSelectedOtherState(this.region);
			this.nation = null;
			this.region = null;
			if (this.nationPanelCanvas != null)
			{
				this.nationPanelCanvas.enabled = false;
			}
			this.HideTooltips();
			this.CloseAnySecondaryPanels(null, false);
			this.CheckforMainCanvasClose();
			this.HideTutorials();
		}

		// Token: 0x06005220 RID: 21024 RVA: 0x002416A8 File Offset: 0x0023F8A8
		private void UpdateNationPanel()
		{
			if (this.nation == null || this.region == null || !this.nation.extant)
			{
				base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
				return;
			}
			this.UpdatePrimaryDisplayElements();
			this.UpdateArmyList();
			this.UpdatePoliciesPanel();
			this.UpdateRegionList();
			this.ResetDirectInvestPanel(this.nation);
			this.UpdatePriorityList();
			NationInfoController.PopulateNationPriorityDropdown(this.priorityPresetDropdown, this.nation, base.activePlayer, ref this.priorityPresetDictionary);
			this.UpdateCouncilorList();
			this.UpdateAllyGrid();
			this.UpdateWarGrid();
			this.UpdateRivalryGrid();
			if (this.nationRelationsManagerPanel.activeInHierarchy)
			{
				this.UpdateRelationsList();
			}
			if (this.nationTabManager.activeTab != null)
			{
				this.nationTabManager.activeTab.UpdateSize();
			}
		}

		// Token: 0x06005221 RID: 21025 RVA: 0x00241788 File Offset: 0x0023F988
		private string warTip()
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.AtWar")).AppendLine();
			foreach (TIWarState tiwarState in this.nation.currentWarStates)
			{
				stringBuilder.AppendLine(tiwarState.displayName);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005222 RID: 21026 RVA: 0x00241804 File Offset: 0x0023FA04
		private void UpdatePrimaryDisplayElements()
		{
			this.nationNameText.SetText(this.nation.displayName);
			if (this.nation.executiveFaction != null)
			{
				FactionView viewofFaction = base.activePlayer.GetViewofFaction(this.nation.executiveFaction);
				if (this.nation.executiveFaction != base.activePlayer && viewofFaction.showLeader)
				{
					this.executiveLeaderBackground.color = this.nation.executiveFaction.template.color;
					this.executiveLeaderBackground.enabled = true;
					this.executiveLeaderImage.sprite = this.nation.executiveFaction.leaderIcon;
					this.executiveLeaderTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.ExecutiveLeaderTooltip(this.nation.executiveFaction, this.nation));
					this.executiveLeaderImageObject.SetActive(true);
				}
				else
				{
					this.executiveLeaderBackground.enabled = false;
					this.executiveLeaderImage.sprite = this.nation.executiveFaction.factionIcon256;
					this.executiveLeaderTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.LesserExecutiveLeaderTooltip(this.nation.executiveFaction, this.nation));
					this.executiveLeaderImageObject.SetActive(true);
				}
				if (this.nation.ExecutivePowerConsolidated)
				{
					this.executiveLeaderConsolidatedVisualization.enabled = true;
					this.executiveLeaderCountdown.enabled = false;
				}
				else
				{
					this.executiveLeaderConsolidatedVisualization.enabled = false;
					this.executiveLeaderCountdown.SetText(Loc.T("UI.Nation.ExecutiveConsolidationTimer", new object[] { this.nation.daysUntilExecutivePowerConsolidated.ToString("N0") }));
					this.executiveLeaderCountdown.enabled = true;
				}
			}
			else
			{
				this.executiveLeaderImageObject.SetActive(false);
				this.executiveLeaderConsolidatedVisualization.enabled = false;
				this.executiveLeaderCountdown.enabled = false;
			}
			this.executiveLeaderRelationsButton.interactable = this.nation.executiveFaction == base.activePlayer;
			this.nukeButtonPanel.SetActive(this.nation.numNuclearWeapons > 0);
			this.nukeButton.interactable = this.nation.numNuclearWeapons > 0 && this.nation.executiveFaction == base.activePlayer;
			this.nukeChevrons.SetActive(this.nukeButton.interactable && this.nation.wars.Count > 0);
			this.regionNameText.SetText(this.region.displayName);
			this.regionIconsText.SetText(this.region.IconString(base.activePlayer));
			if (this.nation.inFederation)
			{
				string text = this.nation.federation.displayName;
				if (this.nation.federation.leadNation == this.nation)
				{
					text = new StringBuilder(text).Append(" ").Append(TemplateManager.global.starInlineSpritePath).ToString();
				}
				this.specialRelationshipName.SetText(text);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.federation.flagResource, this.specialRelationshipImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.federation.flagResource, this.statsFederationImage);
				this.spaceFundingFederationValue.SetText(TIUtilities.FormatBigNumber((double)this.nation.spaceFundingIncome_month, 1, false));
				this.boostFederationValue.SetText(TIUtilities.FormatBigOrSmallNumber(this.nation.boostIncome_month_dekatons, 1, 2, 0, false, false));
				this.specialRelationshipPanelObject.SetActive(true);
				this.statsFederationImageObject.SetActive(true);
				this.federationValuesObject.SetActive(true);
			}
			else if (this.nation.breakaway)
			{
				this.specialRelationshipName.SetText(new StringBuilder(this.nation.breakawayParent.displayName).Append(" ").Append(TemplateManager.global.unrestInlineSpritePath));
				this.specialRelationshipImage.sprite = this.nation.breakawayParent.flag;
				this.specialRelationshipPanelObject.SetActive(true);
				this.statsFederationImageObject.SetActive(false);
				this.federationValuesObject.SetActive(false);
			}
			else
			{
				this.specialRelationshipPanelObject.SetActive(false);
				this.statsFederationImageObject.SetActive(false);
				this.federationValuesObject.SetActive(false);
			}
			if (this.nation.alienNation)
			{
				this.headerBackground.color = GameStateManager.AlienFaction().template.color;
			}
			else
			{
				this.headerBackground.color = (this.nation.breakaway ? this.breakawayColor : Color.white);
			}
			this.flagImage.sprite = this.nation.flag;
			this.statsFlagImage.sprite = this.nation.flag;
			this.statsCouncilImage.sprite = base.activePlayer.factionIcon64UI;
			if (this.nation.atWar)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathWarIcon, this.conflictStatusImage);
				this.conflictStatusTooltipTrigger.SetDelegate("BodyText", () => this.warTip());
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathPeaceIcon, this.conflictStatusImage);
				this.conflictStatusTooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Nation.AtPeace"));
			}
			this.milTechText.SetText(new StringBuilder(this.nation.GetMilitaryDescriptiveStringAndValue(1)).Append(NationInfoController.numberToArrow((double)(this.nation.militaryTechLevel - this.nation.historyMiltech[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.numNukesValue.SetText(new StringBuilder(TemplateManager.global.nukesInlineSpritePath).Append((this.nation.nuclearProgram || this.nation.numNuclearWeapons > 0) ? this.nation.numNuclearWeapons.ToString() : TemplateManager.global.noneIconInlineSpritePath).Append(NationInfoController.numberToArrow((double)(this.nation.numNuclearWeapons - this.nation.historyNukes[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.navalFreedom ? TIGlobalConfig.globalConfig.pathNavalArmyIcon : TIGlobalConfig.globalConfig.pathNoNavyMovementIcon, this.navalStatusIcon);
			this.navalScoreText.SetText(this.nation.NavalFreedomStringValue(true));
			this.numArmiesText.SetText(Loc.T("UI.NationPriorityAccumulation", new object[]
			{
				this.nation.numStandardArmies.ToString(),
				this.nation.allowedArmies.ToString()
			}));
			if (this.nation.canBuildSTOSquadrons)
			{
				this.numSTOsIconObject.SetActive(true);
				TMP_Text tmp_Text = this.numSTOsText;
				string text2 = "UI.Nation.STOFighters";
				object[] array = new object[3];
				array[0] = this.nation.availableSTOFighters.ToString();
				array[1] = this.nation.numSTOFighters.ToString();
				array[2] = this.nation.regions.Sum<TIRegionState>((TIRegionState x) => x.maxSTOFighters);
				tmp_Text.SetText(Loc.T(text2, array));
				this.numSTOsObject.SetActive(true);
				this.StartBuildExofighterTutorial();
			}
			else
			{
				this.numSTOsIconObject.SetActive(false);
				this.numSTOsObject.SetActive(false);
			}
			this.democracyText.SetText(new StringBuilder(this.nation.GetDemocracyDescriptiveStringAndValue(1)).Append(NationInfoController.numberToArrow((double)(this.nation.democracy - this.nation.historyDemocracy[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.stabilityText.SetText(new StringBuilder(this.nation.GetUnrestDescriptiveStringAndValue(1)).Append(NationInfoController.numberToArrow((double)(this.nation.unrest - this.nation.historyUnrest[31]), NationInfoController.WhatIsGood.downIsGood, 0f, 5f)).Append(this.nation.UnrestRestStateInlineSpritePath()));
			this.GDPValue.SetText(new StringBuilder(this.nation.GDPstring).Append(NationInfoController.numberToArrow(this.nation.GDP - this.nation.historyGDP[31], NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.GDPPerCapitaValue.SetText(new StringBuilder(this.nation.perCapitaGDPstr).Append(NationInfoController.numberToArrow((double)((int)this.nation.perCapitaGDP - (int)(this.nation.historyGDP[31] / ((double)this.nation.historyPopulation[31] * 1000000.0))), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.inequalityText.SetText(new StringBuilder(this.nation.GetInequalityDescriptiveStringAndValue(1)).Append(NationInfoController.numberToArrow((double)(this.nation.inequality - this.nation.historyInequality[31]), NationInfoController.WhatIsGood.downIsGood, 0f, 5f)));
			this.populationValueText.SetText(new StringBuilder(TemplateManager.global.populationInlineSpritePath).Append(Loc.T("UI.Nation.PopulationValue", new object[] { this.nation.population_Millions.ToString("N1") })).Append(NationInfoController.numberToArrow((double)(this.nation.population_Millions - this.nation.historyPopulation[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.cohesionText.SetText(new StringBuilder(this.nation.GetCohesionDescriptiveStringAndValue(1)).Append(this.nation.CohesionRestStateInlineSpritePath()));
			this.educationText.SetText(new StringBuilder(this.nation.GetEducationDescriptiveStringAndValue(1)).Append(NationInfoController.numberToArrow((double)(this.nation.education - this.nation.historyEducation[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			GameControl.assetLoader.LoadAssetForImageAssignment(this.nation.SustainabilityIcon(), this.sustainabilityIcon);
			this.sustainabilityText.SetText(new StringBuilder(TINationState.SustainabilityValueForDisplay(this.nation.sustainability)).Append(NationInfoController.numberToArrow((double)(-1f * (this.nation.sustainability - this.nation.historySustainability[31])), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			float num = this.nation.GetPublicOpinionProportion(FactionIdeology.Undecided);
			int num2 = 1;
			foreach (FactionIdeology factionIdeology in from x in this.nation.publicOpinion.Keys
				where x != FactionIdeology.Undecided
				select x into y
				orderby TIFactionIdeologyTemplate.GetIdeologyTemplate(y).sortOrder descending
				select y)
			{
				this.nationIdeologyPortions[num2].color = TIFactionIdeologyTemplate.GetFactionByIdeology(factionIdeology).template.color;
				this.nationIdeologyPortions[num2].fillAmount = 1f - num;
				this.nationIdeologyPortions[num2++].enabled = true;
				num += this.nation.publicOpinion[factionIdeology];
			}
			this.investmentNationValue.SetText(new StringBuilder(TIUtilities.FormatSmallNumber(this.nation.BaseInvestmentPoints_month(), 2, 0, true, false)).Append(NationInfoController.numberToArrow((double)(this.nation.BaseInvestmentPoints_month() - this.nation.historyInvestmentPoints[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.investmentCouncilValue.SetText(TIUtilities.FormatSmallNumber(this.nation.GetCouncilInvestmentPointShare(base.activePlayer), 2, 0, true, false));
			this.spaceFundingNationValue.SetText(new StringBuilder(TIUtilities.FormatBigNumber((double)this.nation.spaceFunding_month, 1, false)).Append(NationInfoController.numberToArrow((double)(this.nation.spaceFunding_month - this.nation.historySpaceFunding[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.spaceFundingCouncilValue.SetText(TIUtilities.FormatBigNumber((double)this.nation.GetMonthlyCouncilResourceShare(base.activePlayer, FactionResource.Money, false), 1, false));
			this.scienceNationValue.SetText(new StringBuilder(TIUtilities.FormatBigOrSmallNumber(this.nation.research_month, 1, 2, 0, false, false)).Append(NationInfoController.numberToArrow((double)(this.nation.research_month - this.nation.historyResearch[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.scienceCouncilValue.SetText(TIUtilities.FormatBigOrSmallNumber(this.nation.GetMonthlyCouncilResourceShare(base.activePlayer, FactionResource.Research, false), 1, 7, 0, false, false));
			this.boostNationValue.SetText(new StringBuilder(TIUtilities.FormatBigOrSmallNumber(this.nation.currentBoost_month, 1, 2, 0, false, false)).Append(NationInfoController.numberToArrow((double)(this.nation.currentBoost_month - this.nation.historyBoost[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.boostCouncilValue.SetText(TIUtilities.FormatBigOrSmallNumber(this.nation.GetMonthlyCouncilResourceShare(base.activePlayer, FactionResource.Boost, false), 1, 2, 0, false, false));
			this.missionControlNationValue.SetText(new StringBuilder(Loc.T("UI.Nation.MissionControlValue", new object[]
			{
				this.nation.currentMissionControl.ToString("N0"),
				this.nation.maxMissionControl.ToString("N0")
			})).Append(NationInfoController.numberToArrow((double)(this.nation.currentMissionControl - this.nation.historyMissionControl[31]), NationInfoController.WhatIsGood.upIsGood, 0f, 5f)));
			this.missionControlCouncilValue.SetText(this.nation.GetMonthlyCouncilResourceShare(base.activePlayer, FactionResource.MissionControl, false).ToString("N0"));
			int num3 = this.nation.armies.Count<TIArmyState>((TIArmyState x) => x.armyType != ArmyType.AlienMegafauna && !x.destroyed);
			this.armiesTabText.SetText((num3 == 1) ? Loc.T("UI.Nation.ArmiesTabOne") : Loc.T("UI.Nation.ArmiesTab", new object[] { num3.ToString() }));
			this.regionsTabText.SetText((this.nation.regions.Count == 1) ? Loc.T("UI.Nation.RegionsTabOne") : Loc.T("UI.Nation.RegionsTab", new object[] { this.nation.regions.Count.ToString() }));
			this.developmentSummaryTooltipTrigger.SetDelegate("BodyText", () => Loc.T("UI.Nation.DevelopmentSummary", new object[] { base.activePlayer.displayName }));
			this.AssignControlPoints();
			this.relationsButton1.interactable = this.nation.executiveFaction == base.activePlayer;
		}

		// Token: 0x06005223 RID: 21027 RVA: 0x002427A0 File Offset: 0x002409A0
		public string TinyControlPointTooltip(TIControlPoint controlPoint)
		{
			TIPriorityPresetTemplate tipriorityPresetTemplate = this.nation.PlayerSettingsMatchTemplate(controlPoint.positionInNation, true);
			StringBuilder stringBuilder = new StringBuilder();
			if (tipriorityPresetTemplate != null)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.PresetTooltip", new object[] { tipriorityPresetTemplate.displayName }));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.PresetTooltip", new object[] { Loc.T("UI.Nation.Custom") }));
			}
			if (controlPoint.faction == base.activePlayer)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.MyTinyControlPointButtonHelp"));
				float num = controlPoint.diversityBonus.Where<KeyValuePair<PriorityType, float>>((KeyValuePair<PriorityType, float> x) => this.nation.ValidPriority(x.Key)).Average<KeyValuePair<PriorityType, float>>((KeyValuePair<PriorityType, float> x) => x.Value);
				if (num > 0f)
				{
					StringBuilder stringBuilder2 = stringBuilder.AppendLine();
					string text = "UI.Nation.DiversityBonus2";
					object[] array = new object[2];
					array[0] = num.ToPercent("P0");
					array[1] = TIUtilities.ConstructTextList(TIControlPoint.priorityDiversityBonus.Select<KeyValuePair<PriorityType, float>, string>((KeyValuePair<PriorityType, float> x) => TIUtilities.GetPriorityString(x.Key, false)).ToList<string>(), false, false);
					stringBuilder2.AppendLine(Loc.T(text, array));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005224 RID: 21028 RVA: 0x002428E8 File Offset: 0x00240AE8
		public void UpdateTinyControlPoints()
		{
			for (int i = 0; i <= 5; i++)
			{
				if (i > this.nation.maxControlPointIndex)
				{
					this.tinyControlPointImage[i].enabled = false;
				}
				else
				{
					TIControlPoint controlPoint = this.nation.GetControlPoint(i);
					if (controlPoint == null)
					{
						this.tinyControlPointImage[i].enabled = false;
					}
					else
					{
						this.tinyControlPointImage[i].enabled = true;
						if (controlPoint.benefitsDisabled)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment("councilor_missions/ICO_crackdown_on", this.tinyControlPointImage[i]);
						}
						else
						{
							this.tinyControlPointImage[i].sprite = controlPoint.GetIcon(true, false);
						}
						if (!controlPoint.owned)
						{
							this.tinyControlPointImage[i].color = this.nation.template.UIColor;
						}
						else
						{
							this.tinyControlPointImage[i].color = new Color(1f, 1f, 1f, 1f);
						}
						this.tinyControlPointTooltip[i].SetDelegate("BodyText", () => this.TinyControlPointTooltip(controlPoint));
						this.tinyControlPointButton[i].interactable = controlPoint.faction == base.activePlayer;
					}
				}
			}
		}

		// Token: 0x06005225 RID: 21029 RVA: 0x00242A4C File Offset: 0x00240C4C
		private void AssignControlPoints()
		{
			this.targetingOwnedCPs = GeneralControlsController.CurrentlyTargetingStateType(typeof(TIControlPoint));
			this.targetingNeutralCP = GeneralControlsController.UITargetingMode is TIMissionTargeting_OpenControlPoint;
			this.controlPointGrid.SetListSize<ControlPointGridItemController>(this.nation.controlPoints.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.controlPointGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__234.<>p__0 == null)
					{
						NationInfoController.<>o__234.<>p__0 = CallSite<Func<CallSite, object, ControlPointGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ControlPointGridItemController), typeof(NationInfoController)));
					}
					NationInfoController.<>o__234.<>p__0.Target(NationInfoController.<>o__234.<>p__0, enumerator.Current).SetGridItem(this, this.nation, this.nation.controlPoints[num++], this.flagImage);
				}
			}
			this.UpdateTinyControlPoints();
			this.disableControlPointsButton.interactable = this.nation.CanDisableControlPoints(base.activePlayer);
			this.autoAbandonToggle.SetIsOnWithoutNotify(base.activePlayer.permaAbandonedNations.Contains(this.nation));
		}

		// Token: 0x06005226 RID: 21030 RVA: 0x00242B84 File Offset: 0x00240D84
		public void OnTinyControlPointClicked(int CPValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SyncPrioritiesAction(this.nation.controlPoints[CPValue]));
			this.UpdatePriorityList();
		}

		// Token: 0x06005227 RID: 21031 RVA: 0x00242BC0 File Offset: 0x00240DC0
		public static string numberToArrow(double delta, NationInfoController.WhatIsGood whatIsGood, float baseValue = 0f, float midValue = 5f)
		{
			switch (whatIsGood)
			{
			case NationInfoController.WhatIsGood.upIsGood:
				if (delta > 0.0)
				{
					return TemplateManager.global.upGreenArrowInlineSpritePath;
				}
				if (delta < 0.0)
				{
					return TemplateManager.global.downRedArrowInlineSpritePath;
				}
				break;
			case NationInfoController.WhatIsGood.downIsGood:
				if (delta > 0.0)
				{
					return TemplateManager.global.upRedArrowInlineSpritePath;
				}
				if (delta < 0.0)
				{
					return TemplateManager.global.downGreenArrowInlineSpritePath;
				}
				break;
			case NationInfoController.WhatIsGood.middleIsGood:
				if (delta > 0.0)
				{
					if (baseValue > midValue)
					{
						return TemplateManager.global.upRedArrowInlineSpritePath;
					}
					if (baseValue <= midValue)
					{
						return TemplateManager.global.upGreenArrowInlineSpritePath;
					}
				}
				else if (delta < 0.0)
				{
					if (baseValue < midValue)
					{
						return TemplateManager.global.downRedArrowInlineSpritePath;
					}
					if (baseValue >= midValue)
					{
						return TemplateManager.global.downGreenArrowInlineSpritePath;
					}
				}
				break;
			case NationInfoController.WhatIsGood.upOrMiddleIsGood:
				if (delta > 0.0)
				{
					return TemplateManager.global.upGreenArrowInlineSpritePath;
				}
				if (baseValue < midValue && delta < 0.0)
				{
					return TemplateManager.global.downRedArrowInlineSpritePath;
				}
				if (delta < 0.0 && baseValue >= midValue)
				{
					return TemplateManager.global.downGreenArrowInlineSpritePath;
				}
				break;
			}
			return string.Empty;
		}

		// Token: 0x06005228 RID: 21032 RVA: 0x00242CF4 File Offset: 0x00240EF4
		private static string numberToColor(double delta, NationInfoController.WhatIsGood whatIsGood, float baseValue = 0f, float midValue = 5f)
		{
			switch (whatIsGood)
			{
			case NationInfoController.WhatIsGood.upIsGood:
				if (delta > 0.0)
				{
					return "<color=#85B260>";
				}
				if (delta < 0.0)
				{
					return "<color=#B26A60>";
				}
				break;
			case NationInfoController.WhatIsGood.downIsGood:
				if (delta > 0.0)
				{
					return "<color=#B26A60>";
				}
				return "<color=#85B260>";
			case NationInfoController.WhatIsGood.middleIsGood:
				if (delta > 0.0)
				{
					if (baseValue > midValue)
					{
						return "<color=#B26A60>";
					}
					if (baseValue < midValue)
					{
						return "<color=#85B260>";
					}
				}
				else if (delta < 0.0)
				{
					if (baseValue > midValue)
					{
						return "<color=#85B260>";
					}
					if (baseValue < midValue)
					{
						return "<color=#B26A60>";
					}
				}
				break;
			case NationInfoController.WhatIsGood.upOrMiddleIsGood:
				if (delta > 0.0 || (baseValue >= midValue && delta < 0.0))
				{
					return "<color=#85B260>";
				}
				if (baseValue < midValue && delta < 0.0)
				{
					return "<color=#B26A60>";
				}
				break;
			}
			return "<color=#9BB9C7>";
		}

		// Token: 0x06005229 RID: 21033 RVA: 0x00242DD8 File Offset: 0x00240FD8
		public static string ExecutiveLeaderTooltip(TIFactionState faction, TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.ExecutiveTooltip", new object[]
			{
				faction.leaderName,
				faction.GetDisplayName(GameControl.control.activePlayer)
			})).AppendLine();
			if (nation.ExecutivePowerConsolidated)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.ExecutiveConsolidation", new object[] { faction.displayNameCapitalizedWithColor }));
			}
			else
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.ExecutiveControlNotConsolidated", new object[]
				{
					faction.displayNameCapitalizedWithColor,
					nation.daysUntilExecutivePowerConsolidated.ToString("N0")
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600522A RID: 21034 RVA: 0x00242E8C File Offset: 0x0024108C
		public static string LesserExecutiveLeaderTooltip(TIFactionState faction, TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.ExecutiveTooltipLessDetail", new object[] { faction.GetDisplayName(GameControl.control.activePlayer) })).AppendLine();
			if (nation.ExecutivePowerConsolidated)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.ExecutiveConsolidation", new object[] { faction.displayNameCapitalizedWithColor }));
			}
			else
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.ExecutiveControlNotConsolidated", new object[]
				{
					faction.displayNameCapitalizedWithColor,
					nation.daysUntilExecutivePowerConsolidated.ToString("N0")
				}));
			}
			if (faction == GameControl.control.activePlayer)
			{
				stringBuilder.AppendLine().Append(Loc.T("UI.Nation.ClickToManageRelations"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600522B RID: 21035 RVA: 0x00242F60 File Offset: 0x00241160
		public static string ControlPointTooltip(TINationState nationState, TIControlPoint controlPoint)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.ControlPointHelpLine1", new object[]
			{
				nationState.displayName,
				(nationState.numControlPoints - controlPoint.positionInNation).ToString()
			}));
			stringBuilder.AppendLine().AppendLine(controlPoint.controlPointTypeDisplayName);
			if (controlPoint.owned)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpLine2", new object[]
				{
					controlPoint.faction.template.inlineColorString,
					controlPoint.faction.displayName
				}));
			}
			else
			{
				stringBuilder.AppendLine();
			}
			if (controlPoint.ExecutiveImmunity)
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Nation.ControlPointHelpExecutive")));
				if (!controlPoint.owned)
				{
					stringBuilder.AppendLine();
				}
			}
			if (controlPoint.owned)
			{
				if (controlPoint.benefitsDisabled)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpCrackdown", new object[]
					{
						controlPoint.crackdownExpiration.ToCustomDateString(),
						TemplateManager.global.armyCrackdownMalus.ToString("N2")
					}));
				}
				if (controlPoint.defended)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpDefend", new object[] { controlPoint.defendExpiration.ToCustomDateString() }));
				}
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpLine3"));
			stringBuilder.AppendLine(Loc.T(new StringBuilder("UI.Nation.").Append(controlPoint.controlPointType.ToString()).ToString()));
			foreach (TIArmyState tiarmyState in nationState.GetArmiesByControlPoint(controlPoint.positionInNation))
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpArmy", new object[] { tiarmyState.displayName }));
			}
			float investmentFromControlPoint = nationState.GetInvestmentFromControlPoint();
			if (investmentFromControlPoint > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpInvestmentPoints", new object[] { TIUtilities.FormatSmallNumber(investmentFromControlPoint, 7, 0, true, false) }));
			}
			float monthlyMoneyIncomeFromControlPoint = nationState.GetMonthlyMoneyIncomeFromControlPoint(controlPoint.faction);
			if (monthlyMoneyIncomeFromControlPoint > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpSpaceFunding", new object[]
				{
					TemplateManager.global.moneyInlineSpritePath,
					TIUtilities.FormatBigOrSmallNumber(monthlyMoneyIncomeFromControlPoint, 1, 7, 0, false, false)
				}));
			}
			float monthlyResearchFromControlPoint = nationState.GetMonthlyResearchFromControlPoint(controlPoint.faction);
			if (monthlyResearchFromControlPoint > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpResearch", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(monthlyResearchFromControlPoint, 1, 7, 0, false, false),
					TemplateManager.global.researchInlineSpritePath
				}));
			}
			float monthlyBoostIncomeFromControlPoint = nationState.GetMonthlyBoostIncomeFromControlPoint();
			if (monthlyBoostIncomeFromControlPoint > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpBoost", new object[]
				{
					TIUtilities.FormatBigOrSmallNumber(monthlyBoostIncomeFromControlPoint, 1, 7, 1, false, false),
					TemplateManager.global.boostInlineSpritePath
				}));
			}
			int missionControlFromControlPoint = nationState.GetMissionControlFromControlPoint(controlPoint.positionInNation);
			if (missionControlFromControlPoint > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointHelpMissionControl", new object[]
				{
					missionControlFromControlPoint.ToString("N0"),
					TemplateManager.global.missionControlInlineSpritePath
				}));
			}
			if (nationState.ControlPointMaintenanceCost > 0f)
			{
				string text = Loc.T("UI.Nations.CPMaint7", new object[]
				{
					TIUtilities.HighlightLine(nationState.ControlPointMaintenanceCost.ToString("N2")),
					TemplateManager.global.controlPointInlineSpritePath_empty
				}).Trim();
				stringBuilder.AppendLine(text);
			}
			if (controlPoint.owned && controlPoint.executive)
			{
				if (nationState.ExecutivePowerConsolidated)
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.ExecutiveConsolidation", new object[] { nationState.executiveFaction.displayNameCapitalizedWithColor }));
				}
				else
				{
					stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.ExecutiveControlNotConsolidated", new object[]
					{
						nationState.executiveFaction.displayNameCapitalizedWithColor,
						nationState.daysUntilExecutivePowerConsolidated.ToString("N0")
					}));
				}
			}
			stringBuilder.AppendLine();
			if (nationState.ControlPointMaintenanceCost > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointCost")).AppendLine();
				if (controlPoint.faction != GameControl.control.activePlayer)
				{
					float annualInfluenceCostOfNextControlPoint = GameControl.control.activePlayer.GetAnnualInfluenceCostOfNextControlPoint(nationState);
					if (annualInfluenceCostOfNextControlPoint > 0f)
					{
						if (annualInfluenceCostOfNextControlPoint < 365.2422f)
						{
							stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.NextControlPointCost_m", new object[] { TIUtilities.FormatBigOrSmallNumber(annualInfluenceCostOfNextControlPoint / 12f, 1, 7, 0, false, false) }))).AppendLine();
						}
						else
						{
							stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Nation.NextControlPointCost", new object[] { TIUtilities.FormatBigOrSmallNumber(annualInfluenceCostOfNextControlPoint / 365.2422f, 1, 7, 0, false, false) }))).AppendLine();
						}
					}
				}
			}
			stringBuilder.AppendLine(Loc.T("UI.Nation.ControlPointDescription"));
			return stringBuilder.ToString();
		}

		// Token: 0x0600522C RID: 21036 RVA: 0x00243474 File Offset: 0x00241674
		private static string requiredIPSummaryText(TINationState nation, PriorityType priority)
		{
			return Loc.T("UI.Nation.RequiredInvestmentPoints", new object[] { TIUtilities.FormatSmallNumber(nation.GetRequiredInvestmentPointsForPriority(priority), 1, 0, true, false) });
		}

		// Token: 0x0600522D RID: 21037 RVA: 0x002434A4 File Offset: 0x002416A4
		public static string BuildPublicOpinionLine(TINationState nation, FactionIdeology ideology, bool region = false)
		{
			float publicOpinionOfFaction = nation.GetPublicOpinionOfFaction(ideology);
			float num;
			nation.historyPublicOpinion[31].TryGetValue(ideology, out num);
			if (ideology == FactionIdeology.Undecided)
			{
				return Loc.T(region ? "UI.Region.PublicOpinionLineNoFaction" : "UI.Nation.PublicOpinionLineNoFaction", new object[]
				{
					GameStateManager.UndecidedIdeology().ideologyStrPublicOpinion,
					nation.GetPublicOpinionOfFaction(FactionIdeology.Undecided).ToPercent("P0"),
					NationInfoController.numberToArrow((double)(publicOpinionOfFaction - num), NationInfoController.WhatIsGood.downIsGood, 0f, 5f)
				});
			}
			TIFactionState factionByIdeology = TIFactionIdeologyTemplate.GetFactionByIdeology(ideology);
			return Loc.T(region ? "UI.Region.PublicOpinionLineFaction" : "UI.Nation.PublicOpinionLineFaction", new object[]
			{
				factionByIdeology.ideology.ideologyStrPublicOpinion,
				factionByIdeology.template.inlineColorString,
				factionByIdeology.displayName,
				publicOpinionOfFaction.ToPercent("P0"),
				NationInfoController.numberToArrow((double)(publicOpinionOfFaction - num), (factionByIdeology == GameControl.control.activePlayer) ? NationInfoController.WhatIsGood.upIsGood : NationInfoController.WhatIsGood.downIsGood, 0f, 5f)
			});
		}

		// Token: 0x0600522E RID: 21038 RVA: 0x002435A8 File Offset: 0x002417A8
		public static string BuildPublicOpinionTooltip(TINationState nationState)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.PublicOpinionHeader")).AppendLine().AppendLine();
			foreach (FactionIdeology factionIdeology in from x in GameStateManager.ActiveHumanIdeologies()
				select x.ideology)
			{
				stringBuilder.AppendLine(NationInfoController.BuildPublicOpinionLine(nationState, factionIdeology, false));
			}
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.PublicOpinionHelp1")).AppendLine()
				.AppendLine(Loc.T("UI.Nation.PublicOpinionHelp2"));
			return stringBuilder.ToString();
		}

		// Token: 0x0600522F RID: 21039 RVA: 0x0024366C File Offset: 0x0024186C
		private static string ChangeString_Sustainability(TINationState nation, float changeValue, bool useColor)
		{
			if (useColor)
			{
				if (changeValue < 0f)
				{
					return Loc.T("UI.Nation.RecentChange", new object[]
					{
						Loc.T("UI.Nation.Sustainability"),
						TIUtilities.GreenLine(nation.SustainabilityChangeForDisplay(changeValue))
					});
				}
				if (changeValue > 0f)
				{
					return Loc.T("UI.Nation.RecentChange", new object[]
					{
						Loc.T("UI.Nation.Sustainability"),
						TIUtilities.RedLine(nation.SustainabilityChangeForDisplay(changeValue))
					});
				}
			}
			return Loc.T("UI.Nation.RecentChange", new object[]
			{
				Loc.T("UI.Nation.Sustainability"),
				useColor ? TIUtilities.HighlightLine(nation.SustainabilityChangeForDisplay(changeValue)) : nation.SustainabilityChangeForDisplay(changeValue)
			});
		}

		// Token: 0x06005230 RID: 21040 RVA: 0x00243720 File Offset: 0x00241920
		private static string ChangeString(string stat, float changeValue, bool useColor, NationInfoController.WhatIsGood whatIsGood, bool dollars = false, bool pop = false, float baseValue = 0f)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!useColor)
			{
				if (!dollars)
				{
					stringBuilder = stringBuilder.Append(Loc.T("UI.Nation.RecentChange", new object[]
					{
						stat,
						TIUtilities.FormatBigOrSmallNumber(changeValue, 1, 7, 0, false, false)
					}));
				}
				else if (pop)
				{
					stringBuilder = stringBuilder.Append(Loc.T("UI.Nation.RecentChangePop", new object[]
					{
						stat,
						TIUtilities.FormatBigOrSmallNumber(changeValue, 1, 7, 0, false, false)
					}));
				}
				else
				{
					stringBuilder = stringBuilder.Append(Loc.T("UI.Nation.RecentChangeMoney", new object[]
					{
						stat,
						TIUtilities.FormatBigOrSmallNumber(changeValue, 1, 7, 1, false, false)
					}));
				}
			}
			else if (pop)
			{
				string text = TIUtilities.FormatBigOrSmallNumber(changeValue * 1000000f, 0, 0, 0, false, false);
				string text2 = (Mathf.Approximately(changeValue, 0f) ? TIUtilities.HighlightLine(text) : new StringBuilder(NationInfoController.numberToColor((double)changeValue, whatIsGood, baseValue, 5f)).Append(text).Append("</color>").ToString());
				stringBuilder = stringBuilder.AppendLine(Loc.T("UI.Nation.RecentChangePop", new object[] { stat, text2 }));
			}
			else
			{
				string text3 = TIUtilities.ForceValueSign(changeValue, dollars, false, "");
				StringBuilder stringBuilder2 = (Mathf.Approximately(changeValue, 0f) ? new StringBuilder(TIUtilities.HighlightLine(text3)) : new StringBuilder(NationInfoController.numberToColor((double)changeValue, whatIsGood, baseValue, 5f)).Append(text3).Append("</color>"));
				if (dollars)
				{
					stringBuilder = stringBuilder.Append(Loc.T("UI.Nation.RecentChangeMoney", new object[] { stat, stringBuilder2 }));
				}
				else
				{
					stringBuilder = stringBuilder.Append(Loc.T("UI.Nation.RecentChange", new object[] { stat, stringBuilder2 }));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005231 RID: 21041 RVA: 0x002438E0 File Offset: 0x00241AE0
		private static string RestStateString(string stat, float currentValue, float restStateValue, float movementCap, NationInfoController.WhatIsGood whatIsGood = NationInfoController.WhatIsGood.upOrMiddleIsGood)
		{
			if (currentValue == restStateValue)
			{
				return Loc.T("UI.Nation.AtRestState", new object[] { stat });
			}
			float num = Mathf.Min(movementCap, Mathf.Abs(currentValue - restStateValue)) * ((restStateValue > currentValue) ? 1f : (-1f));
			string text;
			string text2;
			if (Mathf.Approximately(num, 0f))
			{
				text = TIUtilities.HighlightLine(TIUtilities.ForceValueSign(num, false, false, ""));
				text2 = TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(restStateValue, 7, 0, true, false));
			}
			else if (whatIsGood == NationInfoController.WhatIsGood.middleIsGood)
			{
				string text3 = TIUtilities.FormatSmallNumber(restStateValue, 7, 0, true, false);
				text = ((num > 0f) ? TIUtilities.GreenLine(TIUtilities.ForceValueSign(num, false, false, "")) : ((num < 0f) ? TIUtilities.RedLine(TIUtilities.ForceValueSign(num, false, false, "")) : TIUtilities.HighlightLine(TIUtilities.ForceValueSign(num, false, false, ""))));
				text2 = ((text3 == "5") ? TIUtilities.HighlightLine(text3) : ((restStateValue > 5f) ? TIUtilities.GreenLine(text3) : TIUtilities.RedLine(text3)));
			}
			else if (whatIsGood == NationInfoController.WhatIsGood.downIsGood || whatIsGood == NationInfoController.WhatIsGood.upIsGood)
			{
				string text4 = NationInfoController.numberToColor((double)num, whatIsGood, currentValue, 5f);
				text = new StringBuilder(text4).Append(TIUtilities.ForceValueSign(num, false, false, "")).Append("</color>").ToString();
				text2 = new StringBuilder(text4).Append(TIUtilities.FormatSmallNumber(restStateValue, 7, 0, true, false)).Append("</color>").ToString();
			}
			else
			{
				text = (((restStateValue >= 5f && num > 0f) || (restStateValue < 5f && num < 0f)) ? TIUtilities.GreenLine(TIUtilities.ForceValueSign(num, false, false, "")) : TIUtilities.RedLine(TIUtilities.ForceValueSign(num, false, false, "")));
				text2 = ((restStateValue >= 5f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(restStateValue, 7, 0, true, false)) : TIUtilities.RedLine(TIUtilities.FormatSmallNumber(restStateValue, 7, 0, true, false)));
			}
			return Loc.T("UI.Nation.MovingToRestState", new object[] { stat, text, text2 });
		}

		// Token: 0x06005232 RID: 21042 RVA: 0x00243ADC File Offset: 0x00241CDC
		public static string BuildSpecialRelationshipTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (nation.inFederation)
			{
				stringBuilder.Append(Loc.T((nation.federation.leadNation == nation) ? "UI.Nation.FederationTooltipLeader" : "UI.Nation.FederationTooltipMember", new object[]
				{
					nation.displayNameWithArticleCapitalized,
					nation.federation.displayNameWithArticle,
					nation.federation.leadNation.displayNameWithArticle
				}));
				string text = TIUtilities.ConstructTextList(nation.federation.members.Select<TINationState, TIGameState>((TINationState x) => x.ref_gameState).ToList<TIGameState>(), false, false);
				stringBuilder.Append(Loc.T("UI.Nation.FederationTooltipDetail"));
				if (nation.federation.hegemonicFederation)
				{
					stringBuilder.AppendLine().AppendLine().Append(Loc.T("UI.Nation.DarkFederationTooltip", new object[]
					{
						TemplateManager.global.democracyInlineSpritePath,
						TemplateManager.global.fedLeaderDemocracyScoreToLeaveFederationFreely
					}));
				}
				stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("UI.Nation.FederationMemberList", new object[] { text }));
			}
			else if (nation.breakaway)
			{
				stringBuilder.Append(Loc.T("UI.Nation.BreakawayTooltip", new object[]
				{
					nation.displayNameWithArticleCapitalized,
					nation.breakawayParent.displayNameWithArticle
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005233 RID: 21043 RVA: 0x00243C54 File Offset: 0x00241E54
		public static string BuildRegionDataTooltip(TIRegionState region, TIFactionState faction, TINationState viewingNation)
		{
			StringBuilder stringBuilder = new StringBuilder(region.displayName).AppendLine();
			if (region.isBeingAnnexed)
			{
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.IsBeingAnnexed", new object[]
				{
					region.annexingArmy.homeNation.displayNameWithArticleCapitalized,
					region.annexationEndDate.ToCustomDateString()
				}))).AppendLine();
			}
			else if (region.IsFullyOccupied())
			{
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.IsOccupied"))).AppendLine();
			}
			else if (region.OccupationUnderwayButNotComplete())
			{
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.IsBeingOccupied"))).AppendLine();
			}
			if (region.isCapital)
			{
				stringBuilder.Append(TemplateManager.global.capitalRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionCapital", new object[] { region.nation.displayNameWithArticle })).AppendLine()
					.AppendLine();
			}
			if (region.coreEconomicRegion)
			{
				stringBuilder.Append(TemplateManager.global.coreEconomicRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionCoreEco")).AppendLine()
					.AppendLine();
			}
			if (region.resourceRegion)
			{
				stringBuilder.Append(TemplateManager.global.miningRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionResource")).AppendLine()
					.AppendLine();
			}
			if (region.oilRegion)
			{
				stringBuilder.Append(TemplateManager.global.coreOilRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionOil")).AppendLine()
					.AppendLine();
			}
			if (region.colonyRegion)
			{
				stringBuilder.Append(TemplateManager.global.colonyRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionColony")).AppendLine()
					.AppendLine();
			}
			if (region.template.environment == EnvironmentType.Vulnerable)
			{
				stringBuilder.Append(TemplateManager.global.ecologicallyVulnerableRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionEcoVulnerable")).AppendLine()
					.AppendLine();
			}
			else if (region.template.environment == EnvironmentType.Beneficiary)
			{
				stringBuilder.Append(TemplateManager.global.ecologicallySafeRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionEcoBeneficiary")).AppendLine()
					.AppendLine();
			}
			if (region.terrain == TerrainType.Rugged)
			{
				stringBuilder.Append(TemplateManager.global.ruggedRegionInlineSpritePath).Append(Loc.T("UI.Nation.RegionRugged")).AppendLine()
					.AppendLine();
			}
			if (region.nuclearDetonations > 0)
			{
				stringBuilder.Append(TemplateManager.global.nukedRegionInlineSpritePath).Append(Loc.T("UI.Nation.NukedRegion", new object[] { region.nuclearDetonations.ToString() })).AppendLine()
					.AppendLine();
			}
			if (region.antiSpaceDefenses)
			{
				stringBuilder.Append(TemplateManager.global.antiSpaceDefensesInlineSpritePath).Append(Loc.T("UI.Nation.SpaceDefenses")).AppendLine()
					.AppendLine();
			}
			if (region.nation.hostileClaims.Contains(region))
			{
				stringBuilder.Append(TemplateManager.global.unrestInlineSpritePath).Append(Loc.T("UI.Nation.HostileClaim", new object[] { TemplateManager.global.democracyDecreaseToMakeHostileClaim.ToString("N1") })).AppendLine()
					.AppendLine();
			}
			if (viewingNation != region.nation && !region.nation.hostileClaims.Contains(region) && viewingNation.ClaimWillBeHostile(region, false))
			{
				stringBuilder.Append(viewingNation.WillBeHostileExplanation(region)).AppendLine();
			}
			if (faction.KnownAlienEntities.Any<TIRegionAlienEntityState>((TIRegionAlienEntityState x) => x.region == region))
			{
				stringBuilder.Append(TemplateManager.global.alienEntityInlineSpritePath).Append(Loc.T("UI.Nation.AlienEntityPresent")).AppendLine()
					.AppendLine();
			}
			List<TIRegionState> list = region.AdjacentRegions(true);
			List<TIRegionState> list2 = region.AdjacentRegions(false).Except<TIRegionState>(list).ToList<TIRegionState>();
			List<TIBilateralTemplate> list3 = (from x in TemplateManager.IterateByClass<TIBilateralTemplate>(true)
				where x.relationType == BilateralRelationType.PhysicalAdjacency && x.BilateralIsInScenario() && x.projectUnlock != null && (x.regionState1 == region || x.regionState2 == region) && !GameControl.control.activePlayer.completedProjects.Contains(x.projectUnlock)
				select x).ToList<TIBilateralTemplate>();
			if (list.Count > 0)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				string text = "UI.Nation.RegionAdjacencies";
				object[] array = new object[2];
				array[0] = region.displayName;
				array[1] = TIUtilities.ConstructTextList(list.ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState), false, false);
				stringBuilder2.AppendLine(Loc.T(text, array)).AppendLine();
			}
			if (list2.Count > 0)
			{
				StringBuilder stringBuilder3 = stringBuilder;
				string text2 = "UI.Nation.RegionAdjacenciesFriendlyOnly";
				object[] array2 = new object[2];
				array2[0] = region.displayName;
				array2[1] = TIUtilities.ConstructTextList(list2.ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState), false, false);
				stringBuilder3.AppendLine(Loc.T(text2, array2)).AppendLine();
			}
			if (list3.Count > 0)
			{
				List<TIRegionState> list4 = list3.Select<TIBilateralTemplate, TIRegionState>(delegate(TIBilateralTemplate x)
				{
					if (!(x.regionState1 == region))
					{
						return x.regionState1;
					}
					return x.regionState2;
				}).ToList<TIRegionState>();
				StringBuilder stringBuilder4 = stringBuilder;
				string text3 = "UI.Nation.RegionAdjanciesAddable";
				object[] array3 = new object[1];
				array3[0] = TIUtilities.ConstructTextList(list4.ConvertAll<TIGameState>((TIRegionState x) => x.ref_gameState), false, false);
				stringBuilder4.AppendLine(Loc.T(text3, array3)).AppendLine();
			}
			List<TIRegionState> list5 = region.nation.ExternalClaims();
			List<TINationState> list6 = (from x in region.NationsWithClaim(false, true, list5.Contains(region), false)
				orderby x.regions.Contains(region)
				select x).ToList<TINationState>();
			if (list6.Count > 0)
			{
				StringBuilder stringBuilder5 = stringBuilder;
				string text4 = "UI.Nation.ClaimsList";
				object[] array4 = new object[1];
				array4[0] = TIUtilities.ConstructTextList(list6.ConvertAll<TIGameState>((TINationState x) => x.ref_gameState), false, false);
				stringBuilder5.AppendLine(Loc.T(text4, array4)).AppendLine();
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}

		// Token: 0x06005234 RID: 21044 RVA: 0x002442BC File Offset: 0x002424BC
		public static string BuildNukesTooltip(TINationState nation)
		{
			if (!nation.nuclearProgram)
			{
				return Loc.T("UI.Nation.NuclearTooltip.None");
			}
			return Loc.T("UI.Nation.NuclearTooltip", new object[] { nation.numNuclearWeapons.ToString() });
		}

		// Token: 0x06005235 RID: 21045 RVA: 0x00244300 File Offset: 0x00242500
		public static string BuildnumArmiesTooltip(TINationState nation)
		{
			return Loc.T("UI.Nation.numArmiesTip", new object[]
			{
				nation.numStandardArmies,
				nation.displayNameWithArticleAndPlacePrep,
				nation.allowedArmies,
				TemplateManager.global.minPopulationForFirstArmy_millions,
				TemplateManager.global.minPopulationForAdditionalArmiesPer_millions
			});
		}

		// Token: 0x06005236 RID: 21046 RVA: 0x00244368 File Offset: 0x00242568
		public static string BuildSTOFightersTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.Nation.numSTOTip", new object[]
			{
				TIGlobalConfig.globalConfig.boostInlineSpritePath,
				7,
				14,
				4f
			}));
			if (nation.regions.Any<TIRegionState>((TIRegionState x) => x.STOFighterCooldownExpiry.Count > 0))
			{
				StringBuilder stringBuilder2 = stringBuilder.AppendLine();
				string text = "UI.Nation.FighterAvailDate";
				object[] array = new object[2];
				array[0] = nation.regions.SelectMany<TIRegionState, TIDateTime>((TIRegionState x) => x.STOFighterCooldownExpiry).Min<TIDateTime>().ToCustomDateString();
				array[1] = nation.regions.SelectMany<TIRegionState, TIDateTime>((TIRegionState x) => x.STOFighterCooldownExpiry).Max<TIDateTime>().ToCustomDateString();
				stringBuilder2.AppendLine(Loc.T(text, array));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005237 RID: 21047 RVA: 0x00244480 File Offset: 0x00242680
		public static string BuildEducationTooltip(TINationState nation)
		{
			return new StringBuilder(TIGlobalConfig.globalConfig.educationInlineSpritePath).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Education"),
				nation.GetEducationDescriptiveStringAndValue(3)
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Education"), nation.education - nation.historyEducation[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f))
				.AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.EducationHelp1") : string.Empty)
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.EducationHelp2", new object[]
				{
					(nation.knowledgePriorityEducationChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.knowledgePriorityEducationChange, 7, 1, true, false)) : ((nation.knowledgePriorityEducationChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.knowledgePriorityEducationChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.knowledgePriorityEducationChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Knowledge)
				}))
				.ToString();
		}

		// Token: 0x06005238 RID: 21048 RVA: 0x002445AC File Offset: 0x002427AC
		public static string BuildCohesionTooltip(TINationState nation)
		{
			return new StringBuilder(TIGlobalConfig.globalConfig.cohesionInlineSpritePath).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Cohesion"),
				nation.GetCohesionDescriptiveStringAndValue(3)
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Cohesion"), nation.cohesion - nation.historyCohesion[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f))
				.AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.CohesionRestState"), nation.cohesionRestState - nation.historyCohesionRestState[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f))
				.AppendLine(NationInfoController.RestStateString(Loc.T("UI.Nation.Cohesion"), nation.cohesion, nation.cohesionRestState, Mathf.Abs(nation.GetMonthlyCohesionMovement()), NationInfoController.WhatIsGood.middleIsGood))
				.AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.CohesionHelp1Short") : string.Empty)
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.CohesionHelp2", new object[]
				{
					(nation.unityPriorityCohesionChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false)) : ((nation.unityPriorityCohesionChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Unity),
					(nation.knowledgePriorityCohesionChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false)) : ((nation.knowledgePriorityCohesionChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Knowledge)
				}))
				.AppendLine()
				.AppendLine(nation.CohesionRestStateDetail)
				.ToString();
		}

		// Token: 0x06005239 RID: 21049 RVA: 0x002447AC File Offset: 0x002429AC
		public static string BuildInequalityTooltip(TINationState nation)
		{
			return new StringBuilder(TIGlobalConfig.globalConfig.inequalityInlineSpritePath).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Inequality"),
				nation.GetInequalityDescriptiveStringAndValue(3)
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Inequality"), nation.inequality - nation.historyInequality[31], true, NationInfoController.WhatIsGood.downIsGood, false, false, 0f))
				.AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.InequalityHelp1") : string.Empty)
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.InequalityHelp2", new object[]
				{
					(nation.welfarePriorityInequalityChange < 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.welfarePriorityInequalityChange, 7, 1, true, false)) : ((nation.welfarePriorityInequalityChange > 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.welfarePriorityInequalityChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.welfarePriorityInequalityChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Welfare)
				}))
				.ToString();
		}

		// Token: 0x0600523A RID: 21050 RVA: 0x002448D8 File Offset: 0x00242AD8
		public static string BuildUnrestTooltip(TINationState nation)
		{
			string text = TIUtilities.FormatSmallNumber(nation.OppressionPriorityUnrestChange, 7, 1, true, false);
			return new StringBuilder(TIGlobalConfig.globalConfig.unrestInlineSpritePath).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Unrest"),
				nation.GetUnrestDescriptiveStringAndValue(3)
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Unrest"), nation.unrest - nation.historyUnrest[31], true, NationInfoController.WhatIsGood.downIsGood, false, false, 0f))
				.AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.UnrestRestState"), nation.unrestRestState - nation.historyUnrestRestState[31], true, NationInfoController.WhatIsGood.downIsGood, false, false, nation.unrestRestState))
				.AppendLine(NationInfoController.RestStateString(Loc.T("UI.Nation.Unrest"), nation.unrest, nation.unrestRestState, Mathf.Abs(nation.GetMonthlyUnrestMovement()), NationInfoController.WhatIsGood.downIsGood))
				.AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.UnrestHelp1") : string.Empty)
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.UnrestHelp2", new object[]
				{
					(text == "0") ? TIUtilities.HighlightLine(text) : ((nation.OppressionPriorityUnrestChange < 0f) ? TIUtilities.GreenLine(text) : TIUtilities.RedLine(text)),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Military)
				}))
				.AppendLine()
				.AppendLine(nation.unrestRestStateDetail)
				.ToString();
		}

		// Token: 0x0600523B RID: 21051 RVA: 0x00244A58 File Offset: 0x00242C58
		public static string BuildDemocracyTooltip(TINationState nation)
		{
			return new StringBuilder(TIGlobalConfig.globalConfig.democracyInlineSpritePath).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Democracy"),
				nation.GetDemocracyDescriptiveStringAndValue(3)
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Democracy"), nation.democracy - nation.historyDemocracy[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f))
				.AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.DemocracyHelp1") : string.Empty)
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.DemocracyHelp2", new object[]
				{
					(nation.governmentPriorityDemocracyChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.governmentPriorityDemocracyChange, 7, 1, true, false)) : ((nation.governmentPriorityDemocracyChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.governmentPriorityDemocracyChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.governmentPriorityDemocracyChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Government),
					(nation.OppressionPriorityDemocracyChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.OppressionPriorityDemocracyChange, 7, 1, true, false)) : ((nation.OppressionPriorityDemocracyChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.OppressionPriorityDemocracyChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.OppressionPriorityDemocracyChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Oppression),
					(nation.spoilsPriorityDemocracyChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.spoilsPriorityDemocracyChange, 7, 0, true, false)) : ((nation.spoilsPriorityDemocracyChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.spoilsPriorityDemocracyChange, 7, 0, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.spoilsPriorityDemocracyChange, 7, 0, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Spoils)
				}))
				.ToString();
		}

		// Token: 0x0600523C RID: 21052 RVA: 0x00244C54 File Offset: 0x00242E54
		public static string BuildPopulationTooltip(TINationState nation)
		{
			float num = nation.population_Millions - nation.historyPopulation[31];
			double num2 = (double)(num / nation.historyPopulation[31]);
			double num3 = (double)nation.annualNationalPopulationChange;
			return new StringBuilder(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Population"),
				Loc.T("UI.Nation.PopulationValue", new object[] { nation.population_Millions.ToString("N1") })
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Population"), num, true, NationInfoController.WhatIsGood.upIsGood, false, true, 0f)).AppendLine(Loc.T("UI.Nation.PopulationGrowth", new object[]
			{
				(num2 > 0.0) ? TIUtilities.GreenLine(num2.ToPercent("P2")) : ((num2 < 0.0) ? TIUtilities.RedLine(num2.ToPercent("P2")) : TIUtilities.HighlightLine(num2.ToPercent("P2"))),
				(num3 > 0.0) ? TIUtilities.GreenLine(num3.ToPercent("P2")) : ((num3 < 0.0) ? TIUtilities.RedLine(num3.ToPercent("P2")) : TIUtilities.HighlightLine(num3.ToPercent("P2")))
			}))
				.ToString();
		}

		// Token: 0x0600523D RID: 21053 RVA: 0x00244DB8 File Offset: 0x00242FB8
		public static string BuildMiltechTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Miltech"),
				nation.GetMilitaryDescriptiveStringAndValue(3)
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Miltech"), nation.militaryTechLevel - nation.historyMiltech[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f)).AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.MiltechHelp1") : string.Empty)
				.AppendLine();
			if (nation.military)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.MiltechHelp2", new object[]
				{
					(nation.militaryPriorityTechLevelChange > 0f) ? TIUtilities.GreenLine(TIUtilities.FormatSmallNumber(nation.militaryPriorityTechLevelChange, 7, 1, true, false)) : ((nation.militaryPriorityTechLevelChange < 0f) ? TIUtilities.RedLine(TIUtilities.FormatSmallNumber(nation.militaryPriorityTechLevelChange, 7, 1, true, false)) : TIUtilities.HighlightLine(TIUtilities.FormatSmallNumber(nation.militaryPriorityTechLevelChange, 7, 1, true, false))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Military),
					nation.maxMilitaryTechLevel.ToString("N2")
				}));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.MiltechHelp3"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600523E RID: 21054 RVA: 0x00244F10 File Offset: 0x00243110
		public static string BuildPerCapitaGDPTooltip(TINationState nation)
		{
			return new StringBuilder(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.PerCapitaGDP"),
				nation.perCapitaGDPstr
			})).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.PerCapitaGDP"), nation.perCapitaGDP - nation.HistoryPerCapitaGDP(31), true, NationInfoController.WhatIsGood.upIsGood, true, false, 0f)).AppendLine()
				.AppendLine(TIGlobalConfig.globalConfig.verboseStatDescriptions ? Loc.T("UI.Nation.PerCapitaGDPHelp1") : string.Empty)
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.PerCapitaGDPHelp2", new object[]
				{
					nation.economyPriorityPerCapitaIncomeChange.ToString("N2"),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Economy)
				}).Replace("$" + nation.economyPriorityPerCapitaIncomeChange.ToString("N2"), (nation.economyPriorityPerCapitaIncomeChange >= 0f) ? TIUtilities.GreenLine("$" + nation.economyPriorityPerCapitaIncomeChange.ToString("N2")) : TIUtilities.RedLine("$" + nation.economyPriorityPerCapitaIncomeChange.ToString("N2"))))
				.ToString();
		}

		// Token: 0x0600523F RID: 21055 RVA: 0x00245054 File Offset: 0x00243254
		public static string BuildGDPTooltip(TINationState nation)
		{
			float num = (float)((nation.GDP - nation.historyGDP[31]) / 1000000000.0);
			string text = new StringBuilder(TIGlobalConfig.globalConfig.investmentInlineSpritePath).Append(NationInfoController.numberToColor((double)num, NationInfoController.WhatIsGood.upIsGood, 0f, 5f)).Append(Loc.T("UI.Nation.AbbrBn", new object[] { TIUtilities.ForceValueSign(num, true, false, "") })).Append("</color>")
				.ToString();
			return new StringBuilder(nation.GDPstring).AppendLine().AppendLine(Loc.T("UI.Nation.RecentChangeMoney", new object[]
			{
				Loc.T("UI.Nation.GDP"),
				text
			})).AppendLine()
				.AppendLine(Loc.T("UI.Nation.GDPHelp1"))
				.ToString();
		}

		// Token: 0x06005240 RID: 21056 RVA: 0x0024512C File Offset: 0x0024332C
		public static string BuildSustainabilityTooltip(TINationState nation)
		{
			Tuple<double, double, double> tuple = nation.GHGsFromEconomy_tons(false, 0f);
			return new StringBuilder(nation.SustainabilityIconInlinePath()).Append(Loc.T("UI.Nation.NationalStatTooltipHeader", new object[]
			{
				Loc.T("UI.Nation.Sustainability"),
				TINationState.SustainabilityValueForDisplay(nation.sustainability)
			})).AppendLine().AppendLine(NationInfoController.ChangeString_Sustainability(nation, nation.sustainability - nation.historySustainability[31], true))
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.SustainabilityHelp2", new object[]
				{
					(nation.environmentPrioritySustainabilityChange < 0f) ? TIUtilities.GreenLine(nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange)) : ((nation.environmentPrioritySustainabilityChange > 0f) ? TIUtilities.RedLine(nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange)) : TIUtilities.HighlightLine(nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Environment),
					nation.BestCurrentSustainabilityValueForDisplay(),
					(nation.spoilsSustainabilityChange < 0f) ? TIUtilities.GreenLine(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange)) : ((nation.spoilsSustainabilityChange > 0f) ? TIUtilities.RedLine(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange)) : TIUtilities.HighlightLine(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange))),
					NationInfoController.requiredIPSummaryText(nation, PriorityType.Spoils)
				}))
				.AppendLine()
				.AppendLine(Loc.T("UI.Nation.SustainabilityHelp3", new object[]
				{
					TIUtilities.FormatBigNumber(tuple.Item1, 2, false),
					TIUtilities.FormatSmallNumber(TINationState.CO2toPPM(tuple.Item1), 7, 0, true, false),
					TIUtilities.FormatBigNumber(tuple.Item2, 2, false),
					TIUtilities.FormatSmallNumber(TINationState.CH4toPPM(tuple.Item2), 7, 0, true, false),
					TIUtilities.FormatBigNumber(tuple.Item3, 2, false),
					TIUtilities.FormatSmallNumber(TINationState.N2OtoPPM(tuple.Item3), 7, 0, true, false)
				}))
				.ToString();
		}

		// Token: 0x06005241 RID: 21057 RVA: 0x0024531C File Offset: 0x0024351C
		public static string BuildInvestmentTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.InvestmentPoints")).AppendLine();
			float economyScore = nation.economyScore;
			stringBuilder.Append(Loc.T("UI.Nation.BaseIPs", new object[] { economyScore.ToString("N2") }));
			float num = nation.BaseInvestmentPoints_month();
			if (num != economyScore)
			{
				stringBuilder.Append(Loc.T("UI.Nation.CurrentIPs", new object[] { num.ToString("N2") }));
			}
			stringBuilder.AppendLine();
			float adviserAdministrationBonus = nation.adviserAdministrationBonus;
			if (adviserAdministrationBonus > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.AdviserBonus", new object[] { adviserAdministrationBonus.ToPercent("P0") }));
			}
			float investmentPoints_occupationPenalty_frac = nation.investmentPoints_occupationPenalty_frac;
			if (investmentPoints_occupationPenalty_frac > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.IPOccupationPenalty", new object[] { investmentPoints_occupationPenalty_frac.ToPercent("P0") }));
			}
			float investmentPoints_unrestPenalty_frac = nation.investmentPoints_unrestPenalty_frac;
			if (investmentPoints_unrestPenalty_frac > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.IPUnrestPenalty", new object[] { investmentPoints_unrestPenalty_frac.ToPercent("P0") }));
			}
			int num2 = nation.armies.Count<TIArmyState>((TIArmyState x) => x.investmentArmyFactor > 0f && x.useHomeInvestmentFactor);
			if (num2 > 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.HomeArmiesPenalty", new object[]
				{
					TIUtilities.FormatSmallNumber(TemplateManager.global.nationalInvestmentArmyFactorHome, 7, 0, true, false),
					TIUtilities.FormatSmallNumber((float)num2 * TemplateManager.global.nationalInvestmentArmyFactorHome, 7, 0, true, false)
				}));
			}
			int num3 = nation.armies.Count<TIArmyState>((TIArmyState x) => x.investmentArmyFactor > 0f && !x.useHomeInvestmentFactor);
			if (num3 > 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.AwayArmiesPenalty", new object[]
				{
					TIUtilities.FormatSmallNumber(TemplateManager.global.nationalInvestmentArmyFactorAway, 7, 0, true, false),
					TIUtilities.FormatSmallNumber((float)num3 * TemplateManager.global.nationalInvestmentArmyFactorAway, 7, 0, true, false)
				}));
			}
			int num4 = nation.armies.Count<TIArmyState>((TIArmyState x) => x.deploymentType == DeploymentType.Naval && x.investmentNavyFactor > 0f);
			if (num4 > 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.NaviesPenalty", new object[]
				{
					TIUtilities.FormatSmallNumber(TemplateManager.global.nationalInvestmentNavyFactor, 7, 0, true, false),
					TIUtilities.FormatSmallNumber((float)num4 * TemplateManager.global.nationalInvestmentNavyFactor, 7, 0, true, false)
				}));
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x06005242 RID: 21058 RVA: 0x002455D8 File Offset: 0x002437D8
		public static string BuildSpaceFundingTooltip(TINationState nation)
		{
			string text = new StringBuilder(TIGlobalConfig.globalConfig.moneyInlineSpritePath).Append((nation.maxFunding_year / 12f).ToString("N0")).ToString();
			return new StringBuilder(Loc.T("UI.Nation.SpaceFunding", new object[] { text })).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Priority_Funding"), nation.spaceFunding_month - nation.historySpaceFunding[31], true, NationInfoController.WhatIsGood.upIsGood, true, false, 0f)).ToString()
				.Trim();
		}

		// Token: 0x06005243 RID: 21059 RVA: 0x00245674 File Offset: 0x00243874
		public static string BuildResearchTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.ResearchDescription")).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Nation.Research"), nation.research_month - nation.historyResearch[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f));
			float adviserScienceBonus = nation.adviserScienceBonus;
			if (adviserScienceBonus > 0f)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.AdviserResearchBonus", new object[] { adviserScienceBonus.ToPercent("P0") }));
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x06005244 RID: 21060 RVA: 0x0024570C File Offset: 0x0024390C
		public static string BuildBoostTooltip(TINationState nation)
		{
			return new StringBuilder(Loc.T("UI.Nation.Boost")).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Global.Boost"), nation.currentBoost_month - nation.historyBoost[31], true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f)).ToString()
				.Trim();
		}

		// Token: 0x06005245 RID: 21061 RVA: 0x00245768 File Offset: 0x00243968
		public static string BuildMissionControlTooltip(TINationState nation)
		{
			return new StringBuilder(Loc.T("UI.Nation.MissionControl")).AppendLine().AppendLine(NationInfoController.ChangeString(Loc.T("UI.Global.MissionControl"), (float)(nation.currentMissionControl - nation.historyMissionControl[31]), true, NationInfoController.WhatIsGood.upIsGood, false, false, 0f)).ToString()
				.Trim();
		}

		// Token: 0x06005246 RID: 21062 RVA: 0x002457C8 File Offset: 0x002439C8
		public static string BuildPoliciesTooltip(TINationState nation, bool includeDescription = true)
		{
			List<TIPolicyOption> list = nation.availableSetPolicyOptions(false);
			StringBuilder stringBuilder;
			if (includeDescription)
			{
				stringBuilder = new StringBuilder(Loc.T("UI.Nation.PoliciesDescription", new object[] { (list.Count > 0) ? Loc.T("UI.Nation.SomePolicies") : Loc.T("UI.Nation.NoPolicies") })).AppendLine().AppendLine();
			}
			else
			{
				stringBuilder = new StringBuilder((list.Count > 0) ? Loc.T("UI.Nation.SomePolicies") : Loc.T("UI.Nation.NoPolicies")).AppendLine().AppendLine();
			}
			if (!nation.ExecutivePowerConsolidated)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ExecutiveControlNotConsolidated", new object[]
				{
					nation.executiveFaction.displayNameCapitalizedWithColor,
					nation.daysUntilExecutivePowerConsolidated.ToString("N0")
				})).AppendLine();
			}
			foreach (TIPolicyOption tipolicyOption in nation.availableSetPolicyOptions(false))
			{
				IList<TIGameState> possibleTargets = tipolicyOption.GetPossibleTargets(nation);
				if (!tipolicyOption.RequiresTargets() || possibleTargets.Count > 0)
				{
					stringBuilder.AppendLine(TIUtilities.HighlightLine(tipolicyOption.GetDisplayName()));
					if (tipolicyOption.RequiresTargets() && (possibleTargets.Count > 1 || (possibleTargets.Count == 1 && possibleTargets[0] != nation)))
					{
						stringBuilder.Append(Loc.T("UI.Nation.PolicyCandidates"));
						int num = 1;
						foreach (TIGameState tigameState in possibleTargets)
						{
							if (num < possibleTargets.Count)
							{
								stringBuilder.Append(Loc.T("UI.Nation.PolicyCandidatesListItem", new object[] { tigameState.displayName }));
								num++;
							}
							else
							{
								stringBuilder.Append(" ").Append(tigameState.displayName);
								stringBuilder.AppendLine();
							}
						}
					}
					stringBuilder.AppendLine();
				}
			}
			if (nation.improveRelationsCooldowns.Any<KeyValuePair<TINationState, TIDateTime>>((KeyValuePair<TINationState, TIDateTime> x) => x.Key.extant && x.Value >= TITimeState.Now()))
			{
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Nation.Cooldowns")));
				IOrderedEnumerable<TINationState> orderedEnumerable = nation.improveRelationsCooldowns.Keys.OrderBy<TINationState, string>((TINationState x) => x.displayName);
				Func<TINationState, TIDateTime> <>9__2;
				Func<TINationState, TIDateTime> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = (TINationState x) => nation.improveRelationsCooldowns[x]);
				}
				foreach (TINationState tinationState in orderedEnumerable.ThenByDescending<TINationState, TIDateTime>(func))
				{
					if (tinationState.extant && nation.improveRelationsCooldowns[tinationState] >= TITimeState.Now())
					{
						stringBuilder.AppendLine(Loc.T("UI.Nation.CooldownItem", new object[]
						{
							tinationState.displayName,
							nation.improveRelationsCooldowns[tinationState].ToCustomDateString()
						}));
					}
				}
			}
			return stringBuilder.ToString().Trim().Trim(new char[] { ',' });
		}

		// Token: 0x06005247 RID: 21063 RVA: 0x00245B9C File Offset: 0x00243D9C
		public static string BuildNavalTooltip(TINationState nation)
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T("UI.Nation.NavalTooltip", new object[]
			{
				nation.NavalFreedomStringValue(false),
				Loc.T("UI.Nation.NavalDetail")
			}));
			if (nation.atWar && nation.nationNavalScore > 0f && !nation.navalFreedom)
			{
				nation.currentWarStates.Where<TIWarState>((TIWarState x) => x.EnemyAlliance(nation).Sum<TINationState>((TINationState y) => y.nationNavalScore) > x.Alliance(nation).Sum<TINationState>((TINationState z) => z.nationNavalScore));
				stringBuilder.AppendLine().AppendLine().AppendLine(Loc.T("UI.Nation.WartimeEnemyNavalScore", new object[] { TIUtilities.ConstructTextList(nation.currentWarStates.Where<TIWarState>((TIWarState x) => x.EnemyAlliance(nation).Sum<TINationState>((TINationState y) => y.nationNavalScore) > x.Alliance(nation).Sum<TINationState>((TINationState z) => z.nationNavalScore)).Select<TIWarState, string>(delegate(TIWarState x)
				{
					string text = "UI.Nation.WartimeEnemyNavalScoreEntry";
					object[] array = new object[3];
					array[0] = x.displayNameWithArticle;
					array[1] = TIUtilities.RedLine(x.EnemyAlliance(nation).Sum<TINationState>((TINationState y) => y.nationNavalScore).ToString("N0"));
					array[2] = x.Alliance(nation).Sum<TINationState>((TINationState z) => z.nationNavalScore).ToString("N0");
					return Loc.T(text, array);
				}).ToList<string>(), false, false) }));
			}
			stringBuilder.AppendLine(Loc.T("UI.Nation.NavalLimits", new object[]
			{
				nation.numNavies,
				nation.maxNaviesCanBuild,
				nation.maxNavies,
				TIGlobalConfig.globalConfig.minControlPointsForNavy,
				TIGlobalConfig.globalConfig.minControlPointsForNavyException,
				TIGlobalConfig.globalConfig.PCGDPForNavyException.ToString("N0")
			}));
			return stringBuilder.ToString();
		}

		// Token: 0x06005248 RID: 21064 RVA: 0x00245D28 File Offset: 0x00243F28
		public static string GetTrackedValueAtCell(TINationState nation, NationInfoController.TrackedValue value, string cellName)
		{
			int num = int.Parse(string.Concat<char>(cellName.Reverse<char>().TakeWhile<char>(new Func<char, bool>(char.IsNumber)).Reverse<char>()));
			int num2 = num / 10;
			switch (num % 10)
			{
			case 0:
				switch (value)
				{
				case NationInfoController.TrackedValue.GDP:
				{
					TINationState.GDPChangeReason gdpchangeReason = (TINationState.GDPChangeReason)num2;
					return Loc.T(gdpchangeReason.ToString());
				}
				case NationInfoController.TrackedValue.Inequality:
				{
					TINationState.InequalityChangeReason inequalityChangeReason = (TINationState.InequalityChangeReason)num2;
					return Loc.T(inequalityChangeReason.ToString());
				}
				case NationInfoController.TrackedValue.Cohesion:
				{
					TINationState.CohesionChangeReason cohesionChangeReason = (TINationState.CohesionChangeReason)num2;
					return Loc.T(cohesionChangeReason.ToString());
				}
				case NationInfoController.TrackedValue.Unrest:
				{
					TINationState.UnrestChangeReason unrestChangeReason = (TINationState.UnrestChangeReason)num2;
					return Loc.T(unrestChangeReason.ToString());
				}
				case NationInfoController.TrackedValue.Education:
				{
					TINationState.EducationChangeReason educationChangeReason = (TINationState.EducationChangeReason)num2;
					return Loc.T(educationChangeReason.ToString());
				}
				case NationInfoController.TrackedValue.Government:
				{
					TINationState.DemocracyChangeReason democracyChangeReason = (TINationState.DemocracyChangeReason)num2;
					return Loc.T(democracyChangeReason.ToString());
				}
				case NationInfoController.TrackedValue.GHGs:
					if (num2 == 4 && !GameControl.control.activePlayer.MilestoneCompleted(CampaignMilestone.DetectXenoforming))
					{
						return Loc.T("GHGChangeReason_Xenoforming_Early");
					}
					return Loc.T(new StringBuilder("GHGChangeReason_").Append((GHGSources)num2).ToString());
				}
				break;
			case 1:
				switch (value)
				{
				case NationInfoController.TrackedValue.GDP:
					return TIUtilities.ForceValueSign(nation.tracker_GDPChangeReason_CurrentTrackingPeriod[(TINationState.GDPChangeReason)num2], TIUtilities.FormatBigNumber((double)nation.tracker_GDPChangeReason_CurrentTrackingPeriod[(TINationState.GDPChangeReason)num2], 2, true), true, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Inequality:
					return TIUtilities.ForceValueSign(nation.tracker_InequalityChangeReason_CurrentTrackingPeriod[(TINationState.InequalityChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_InequalityChangeReason_CurrentTrackingPeriod[(TINationState.InequalityChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				case NationInfoController.TrackedValue.Cohesion:
					return TIUtilities.ForceValueSign(nation.tracker_CohesionChangeReason_CurrentTrackingPeriod[(TINationState.CohesionChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_CohesionChangeReason_CurrentTrackingPeriod[(TINationState.CohesionChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Unrest:
					return TIUtilities.ForceValueSign(nation.tracker_UnrestChangeReason_CurrentTrackingPeriod[(TINationState.UnrestChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_UnrestChangeReason_CurrentTrackingPeriod[(TINationState.UnrestChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				case NationInfoController.TrackedValue.Education:
					return TIUtilities.ForceValueSign(nation.tracker_EducationChangeReason_CurrentTrackingPeriod[(TINationState.EducationChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_EducationChangeReason_CurrentTrackingPeriod[(TINationState.EducationChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Government:
					return TIUtilities.ForceValueSign(nation.tracker_DemocracyChangeReason_CurrentTrackingPeriod[(TINationState.DemocracyChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_DemocracyChangeReason_CurrentTrackingPeriod[(TINationState.DemocracyChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.GHGs:
					return TIUtilities.ForceValueSign((float)TIGlobalValuesState.GlobalValues.CO2SourcesRecord_ppm[(GHGSources)num2], TIUtilities.FormatBigOrSmallNumber(TIGlobalValuesState.GlobalValues.CO2SourcesRecord_ppm[(GHGSources)num2], 1, 7, 0, false, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				}
				break;
			case 2:
				switch (value)
				{
				case NationInfoController.TrackedValue.GDP:
					return TIUtilities.ForceValueSign(nation.tracker_GDPChangeReason_PriorTrackingPeriod[(TINationState.GDPChangeReason)num2], TIUtilities.FormatBigNumber((double)nation.tracker_GDPChangeReason_PriorTrackingPeriod[(TINationState.GDPChangeReason)num2], 2, true), true, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Inequality:
					return TIUtilities.ForceValueSign(nation.tracker_InequalityChangeReason_PriorTrackingPeriod[(TINationState.InequalityChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_InequalityChangeReason_PriorTrackingPeriod[(TINationState.InequalityChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				case NationInfoController.TrackedValue.Cohesion:
					return TIUtilities.ForceValueSign(nation.tracker_CohesionChangeReason_PriorTrackingPeriod[(TINationState.CohesionChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_CohesionChangeReason_PriorTrackingPeriod[(TINationState.CohesionChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Unrest:
					return TIUtilities.ForceValueSign(nation.tracker_UnrestChangeReason_PriorTrackingPeriod[(TINationState.UnrestChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_UnrestChangeReason_PriorTrackingPeriod[(TINationState.UnrestChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				case NationInfoController.TrackedValue.Education:
					return TIUtilities.ForceValueSign(nation.tracker_EducationChangeReason_PriorTrackingPeriod[(TINationState.EducationChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_EducationChangeReason_PriorTrackingPeriod[(TINationState.EducationChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Government:
					return TIUtilities.ForceValueSign(nation.tracker_DemocracyChangeReason_PriorTrackingPeriod[(TINationState.DemocracyChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_DemocracyChangeReason_PriorTrackingPeriod[(TINationState.DemocracyChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.GHGs:
					return TIUtilities.ForceValueSign((float)TIGlobalValuesState.GlobalValues.CH4SourcesRecord_ppm[(GHGSources)num2], TIUtilities.FormatBigOrSmallNumber(TIGlobalValuesState.GlobalValues.CH4SourcesRecord_ppm[(GHGSources)num2], 1, 7, 0, false, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				}
				break;
			case 3:
				switch (value)
				{
				case NationInfoController.TrackedValue.GDP:
					return TIUtilities.ForceValueSign(nation.tracker_GDPChangeReason_AllTime[(TINationState.GDPChangeReason)num2], TIUtilities.FormatBigNumber((double)nation.tracker_GDPChangeReason_AllTime[(TINationState.GDPChangeReason)num2], 2, true), true, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Inequality:
					return TIUtilities.ForceValueSign(nation.tracker_InequalityChangeReason_AllTime[(TINationState.InequalityChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_InequalityChangeReason_AllTime[(TINationState.InequalityChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				case NationInfoController.TrackedValue.Cohesion:
					return TIUtilities.ForceValueSign(nation.tracker_CohesionChangeReason_AllTime[(TINationState.CohesionChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_CohesionChangeReason_AllTime[(TINationState.CohesionChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Unrest:
					return TIUtilities.ForceValueSign(nation.tracker_UnrestChangeReason_AllTime[(TINationState.UnrestChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_UnrestChangeReason_AllTime[(TINationState.UnrestChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				case NationInfoController.TrackedValue.Education:
					return TIUtilities.ForceValueSign(nation.tracker_EducationChangeReason_AllTime[(TINationState.EducationChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_EducationChangeReason_AllTime[(TINationState.EducationChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.Government:
					return TIUtilities.ForceValueSign(nation.tracker_DemocracyChangeReason_AllTime[(TINationState.DemocracyChangeReason)num2], TIUtilities.FormatSmallNumber(nation.tracker_DemocracyChangeReason_AllTime[(TINationState.DemocracyChangeReason)num2], 7, 0, true, true), false, true, NationInfoController.WhatIsGood.upIsGood);
				case NationInfoController.TrackedValue.GHGs:
					return TIUtilities.ForceValueSign((float)TIGlobalValuesState.GlobalValues.N2OSourcesRecord_ppm[(GHGSources)num2], TIUtilities.FormatBigOrSmallNumber(TIGlobalValuesState.GlobalValues.N2OSourcesRecord_ppm[(GHGSources)num2], 1, 7, 0, false, true), false, true, NationInfoController.WhatIsGood.downIsGood);
				}
				break;
			}
			return "Missing Value";
		}

		// Token: 0x06005249 RID: 21065 RVA: 0x00246274 File Offset: 0x00244474
		public void SetTableTipDelegates(TooltipTrigger tip, NationInfoController.TrackedValue whatTracking)
		{
			if (whatTracking == NationInfoController.TrackedValue.GHGs)
			{
				NationInfoController.SetGHGTableTipDelegates(tip);
				return;
			}
			tip.SetDelegate("HeaderCol1", () => Loc.T("UI.Nation.Cause"));
			tip.SetDelegate("HeaderCol2", () => Loc.T("UI.Nation.MTD"));
			tip.SetDelegate("HeaderCol3", () => Loc.T("UI.Nation.LastMonth"));
			tip.SetDelegate("HeaderCol4", () => Loc.T("UI.Nation.AllTime"));
			int num = 0;
			switch (whatTracking)
			{
			case NationInfoController.TrackedValue.GDP:
				num = Enum.GetNames(typeof(TINationState.GDPChangeReason)).Length;
				break;
			case NationInfoController.TrackedValue.Inequality:
				num = Enum.GetNames(typeof(TINationState.InequalityChangeReason)).Length;
				break;
			case NationInfoController.TrackedValue.Cohesion:
				num = Enum.GetNames(typeof(TINationState.CohesionChangeReason)).Length;
				break;
			case NationInfoController.TrackedValue.Unrest:
				num = Enum.GetNames(typeof(TINationState.UnrestChangeReason)).Length - 2;
				break;
			case NationInfoController.TrackedValue.Education:
				num = Enum.GetNames(typeof(TINationState.EducationChangeReason)).Length;
				break;
			case NationInfoController.TrackedValue.Government:
				num = Enum.GetNames(typeof(TINationState.DemocracyChangeReason)).Length;
				break;
			}
			for (int i = 0; i < num; i++)
			{
				string text = "Row" + i.ToString();
				tip.TurnSectionOn(text);
				for (int j = 0; j <= 3; j++)
				{
					string cellName = "Cell" + i.ToString() + j.ToString();
					NationInfoController.TrackedValue tracking = whatTracking;
					tip.SetDelegate(cellName, () => NationInfoController.GetTrackedValueAtCell(this.nation, tracking, cellName));
				}
			}
			for (int k = num; k < 20; k++)
			{
				string text2 = "Row" + k.ToString();
				tip.TurnSectionOff(text2);
			}
		}

		// Token: 0x0600524A RID: 21066 RVA: 0x0024647C File Offset: 0x0024467C
		public static void SetGHGTableTipDelegates(TooltipTrigger tip)
		{
			tip.SetDelegate("HeaderCol1", () => Loc.T("UI.GHGCause.Cause"));
			tip.SetDelegate("HeaderCol2", () => Loc.T("UI.GHGCause.CarbonDioxide"));
			tip.SetDelegate("HeaderCol3", () => Loc.T("UI.GHGCause.Methane"));
			tip.SetDelegate("HeaderCol4", () => Loc.T("UI.GHGCause.NitrousOxide"));
			int num = Enum.GetNames(typeof(GHGSources)).Length;
			for (int i = 0; i < num; i++)
			{
				string text = "Row" + i.ToString();
				tip.TurnSectionOn(text);
				for (int j = 0; j <= 3; j++)
				{
					string cellName = "Cell" + i.ToString() + j.ToString();
					NationInfoController.TrackedValue tracking = NationInfoController.TrackedValue.GHGs;
					tip.SetDelegate(cellName, () => NationInfoController.GetTrackedValueAtCell(null, tracking, cellName));
				}
			}
			for (int k = num; k < 20; k++)
			{
				string text2 = "Row" + k.ToString();
				tip.TurnSectionOff(text2);
			}
		}

		// Token: 0x0600524B RID: 21067 RVA: 0x002465EC File Offset: 0x002447EC
		private void SetTooltips()
		{
			this.regionNameTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildRegionDataTooltip(this.region, base.activePlayer, this.region.nation));
			this.regionIconsTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildRegionDataTooltip(this.region, base.activePlayer, this.region.nation));
			this.educationTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildEducationTooltip(this.nation));
			this.SetTableTipDelegates(this.educationTooltipTrigger, NationInfoController.TrackedValue.Education);
			this.publicOpinionTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildPublicOpinionTooltip(this.nation));
			this.cohesionTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildCohesionTooltip(this.nation));
			this.SetTableTipDelegates(this.cohesionTooltipTrigger, NationInfoController.TrackedValue.Cohesion);
			this.stabilityTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildUnrestTooltip(this.nation));
			this.SetTableTipDelegates(this.stabilityTooltipTrigger, NationInfoController.TrackedValue.Unrest);
			this.inequalityTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildInequalityTooltip(this.nation));
			this.SetTableTipDelegates(this.inequalityTooltipTrigger, NationInfoController.TrackedValue.Inequality);
			this.democracyTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildDemocracyTooltip(this.nation));
			this.SetTableTipDelegates(this.democracyTooltipTrigger, NationInfoController.TrackedValue.Government);
			this.GDPPerCapitaTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildPerCapitaGDPTooltip(this.nation));
			this.milTechTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildMiltechTooltip(this.nation));
			this.navalScoreTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildNavalTooltip(this.nation));
			this.nukesTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildNukesTooltip(this.nation));
			this.numArmiesTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildnumArmiesTooltip(this.nation));
			this.numSTOFightersTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildSTOFightersTooltip(this.nation));
			this.GDPTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildGDPTooltip(this.nation));
			this.SetTableTipDelegates(this.GDPTooltipTrigger, NationInfoController.TrackedValue.GDP);
			this.sustainabilityTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildSustainabilityTooltip(this.nation));
			this.populationTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildPopulationTooltip(this.nation));
			this.investmentNationTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildInvestmentTooltip(this.nation));
			this.spaceFundingTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildSpaceFundingTooltip(this.nation));
			this.scienceNationTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildResearchTooltip(this.nation));
			this.boostNationTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildBoostTooltip(this.nation));
			this.missionControlNationTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildMissionControlTooltip(this.nation));
			this.policyTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildPoliciesTooltip(this.nation, true));
			this.specialRelationshipTooltipTrigger.SetDelegate("BodyText", () => NationInfoController.BuildSpecialRelationshipTooltip(this.nation));
		}

		// Token: 0x0600524C RID: 21068 RVA: 0x002468E8 File Offset: 0x00244AE8
		private void HideTooltips()
		{
			this.regionNameTooltipTrigger.ForceHideTooltip();
			this.regionIconsTooltipTrigger.ForceHideTooltip();
			this.educationTooltipTrigger.ForceHideTooltip();
			this.publicOpinionTooltipTrigger.ForceHideTooltip();
			this.cohesionTooltipTrigger.ForceHideTooltip();
			this.stabilityTooltipTrigger.ForceHideTooltip();
			this.inequalityTooltipTrigger.ForceHideTooltip();
			this.democracyTooltipTrigger.ForceHideTooltip();
			this.GDPPerCapitaTooltipTrigger.ForceHideTooltip();
			this.milTechTooltipTrigger.ForceHideTooltip();
			this.navalScoreTooltipTrigger.ForceHideTooltip();
			this.nukesTooltipTrigger.ForceHideTooltip();
			this.numArmiesTooltipTrigger.ForceHideTooltip();
			this.numSTOFightersTooltipTrigger.ForceHideTooltip();
			this.GDPTooltipTrigger.ForceHideTooltip();
			this.sustainabilityTooltipTrigger.ForceHideTooltip();
			this.populationTooltipTrigger.ForceHideTooltip();
			this.investmentNationTooltipTrigger.ForceHideTooltip();
			this.spaceFundingTooltipTrigger.ForceHideTooltip();
			this.scienceNationTooltipTrigger.ForceHideTooltip();
			this.boostNationTooltipTrigger.ForceHideTooltip();
			this.missionControlNationTooltipTrigger.ForceHideTooltip();
			this.policyTooltipTrigger.ForceHideTooltip();
			this.specialRelationshipTooltipTrigger.ForceHideTooltip();
		}

		// Token: 0x0600524D RID: 21069 RVA: 0x002469FD File Offset: 0x00244BFD
		public void OnArmyMajorStatusUpdate(ArmyMajorStatusUpdate e)
		{
			this.UpdateNationPanel();
		}

		// Token: 0x0600524E RID: 21070 RVA: 0x00246A08 File Offset: 0x00244C08
		protected void UpdateArmyList()
		{
			List<TIArmyState> list = this.nation.armies.Where<TIArmyState>((TIArmyState x) => !x.destroyed && x.armyType != ArmyType.AlienMegafauna).ToList<TIArmyState>();
			this.armyList.SetListSize<ArmyListItemController>(list.Count, false, false);
			if (this.nation.armies.Count > 0)
			{
				this.armyTabButtonObject.SetActive(true);
				int num = 0;
				float num2 = 0f;
				using (IEnumerator<object> enumerator = this.armyList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (NationInfoController.<>o__277.<>p__0 == null)
						{
							NationInfoController.<>o__277.<>p__0 = CallSite<Func<CallSite, object, ArmyListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ArmyListItemController), typeof(NationInfoController)));
						}
						ArmyListItemController armyListItemController = NationInfoController.<>o__277.<>p__0.Target(NationInfoController.<>o__277.<>p__0, enumerator.Current);
						armyListItemController.Initialize(list[num++], this);
						if (num2 == 0f)
						{
							num2 = armyListItemController.transform.GetComponent<RectTransform>().sizeDelta.y;
						}
					}
				}
				this.armiesTabController.SetSize(35f, 27f, 47f, list.Count);
				return;
			}
			this.armyTabButtonObject.SetActive(false);
			if (this.nationTabManager.activeTab == this.armiesTabController)
			{
				this.nationTabManager.Toggle(this.nationTabManager.activeTab);
				this.nationTabManager.ClearActiveTab();
			}
		}

		// Token: 0x0600524F RID: 21071 RVA: 0x00246B9C File Offset: 0x00244D9C
		protected void UpdatePoliciesPanel()
		{
			List<TIPolicyOption> list = this.nation.availableSetPolicyOptions(false);
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.nation.ExecutivePowerConsolidated)
			{
				stringBuilder.AppendLine(Loc.T("UI.Nation.ExecutiveControlNotConsolidated", new object[]
				{
					this.nation.executiveFaction.displayNameCapitalizedWithColor,
					this.nation.daysUntilExecutivePowerConsolidated.ToString("N0")
				})).AppendLine().AppendLine();
			}
			stringBuilder.Append((list.Count > 0) ? Loc.T("UI.Nation.SomePolicies") : Loc.T("UI.Nation.NoPolicies"));
			this.policyHeaderText.SetText(stringBuilder.ToString());
			bool flag = this.nation.improveRelationsCooldowns.Any<KeyValuePair<TINationState, TIDateTime>>((KeyValuePair<TINationState, TIDateTime> x) => x.Key.extant && x.Value >= TITimeState.Now());
			this.policyList.SetListSize<PolicyDisplayListItemController>(list.Count + (flag ? 1 : 0), false, false);
			this.policyList.gameObject.SetActive(list.Count != 0 || flag);
			int num = 0;
			using (IEnumerator<object> enumerator = this.policyList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__278.<>p__0 == null)
					{
						NationInfoController.<>o__278.<>p__0 = CallSite<Func<CallSite, object, PolicyDisplayListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PolicyDisplayListItemController), typeof(NationInfoController)));
					}
					PolicyDisplayListItemController policyDisplayListItemController = NationInfoController.<>o__278.<>p__0.Target(NationInfoController.<>o__278.<>p__0, enumerator.Current);
					if (flag && num == list.Count)
					{
						policyDisplayListItemController.SetListItemAsCooldowns(this.nation);
					}
					else
					{
						policyDisplayListItemController.SetListItem(list[num++], this.nation);
					}
				}
			}
			this.policiesTabController.SetSize(412f, 0f, 0f, list.Count);
		}

		// Token: 0x06005250 RID: 21072 RVA: 0x00246D90 File Offset: 0x00244F90
		public void UpdatePriorityList()
		{
			List<PriorityType> list = Enums.PriorityTypes.ToList<PriorityType>();
			int num = 0;
			using (IEnumerator<object> enumerator = this.priorityList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__279.<>p__0 == null)
					{
						NationInfoController.<>o__279.<>p__0 = CallSite<Func<CallSite, object, PriorityListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PriorityListItemController), typeof(NationInfoController)));
					}
					PriorityListItemController priorityListItemController = NationInfoController.<>o__279.<>p__0.Target(NationInfoController.<>o__279.<>p__0, enumerator.Current);
					if (this.nation.ValidPriority(list[num]))
					{
						priorityListItemController.SetListItem(this.nation, list[num], base.activePlayer);
						priorityListItemController.gameObject.SetActive(true);
					}
					else
					{
						priorityListItemController.gameObject.SetActive(false);
					}
					num++;
				}
			}
			bool flag = this.nation.FactionHasControlPoint(base.activePlayer);
			this.priorityPresetDropdown.gameObject.SetActive(flag);
			this.priorityHeader1.gameObject.SetActive(!flag);
			this.priorityHeader2.gameObject.SetActive(!flag);
			this.SetPriorityValueColumnButtonText();
		}

		// Token: 0x06005251 RID: 21073 RVA: 0x00246ECC File Offset: 0x002450CC
		public static string GenericPriorityTipStr(PriorityType priority)
		{
			StringBuilder stringBuilder;
			if (priority != PriorityType.Military_BuildArmy)
			{
				if (priority != PriorityType.Military_BuildNavy)
				{
					stringBuilder = new StringBuilder(Loc.T(new StringBuilder("UI.Nation.").Append(priority.ToString()).Append("PriorityGeneric").ToString()));
				}
				else
				{
					stringBuilder = new StringBuilder(Loc.T("UI.Nation.Military_SealiftPriorityGeneric", new object[]
					{
						TemplateManager.global.minControlPointsForNavy.ToString(),
						TemplateManager.global.minControlPointsForNavyException.ToString(),
						TemplateManager.global.PCGDPForNavyException.ToString("N0")
					}));
				}
			}
			else
			{
				stringBuilder = new StringBuilder(Loc.T("UI.Nation.Military_BuildArmyPriorityGeneric", new object[]
				{
					TemplateManager.global.minPopulationForFirstArmy_millions.ToString(),
					TemplateManager.global.minPopulationForAdditionalArmiesPer_millions.ToString()
				}));
			}
			if (!TIGlobalValuesState.CanAnyHumanNationUsePriority(priority))
			{
				stringBuilder.AppendLine().AppendLine().AppendLine(TIUtilities.RedLine(Loc.T("UI.Nation.GenericPriorityNotAvailable")));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005252 RID: 21074 RVA: 0x00246FE0 File Offset: 0x002451E0
		public static TIPriorityPresetTemplate GlobalPlayerPresetSetting(TINationState nation, out bool playerCPPresent)
		{
			List<TIPriorityPresetTemplate> list = new List<TIPriorityPresetTemplate>();
			playerCPPresent = false;
			bool flag = true;
			using (List<TIControlPoint>.Enumerator enumerator = nation.controlPoints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.faction == GameControl.control.activePlayer)
					{
						playerCPPresent = true;
						break;
					}
				}
			}
			foreach (TIControlPoint ticontrolPoint in nation.controlPoints)
			{
				if (!playerCPPresent || ticontrolPoint.faction == GameControl.control.activePlayer)
				{
					TIPriorityPresetTemplate tipriorityPresetTemplate = nation.PlayerSettingsMatchTemplate(ticontrolPoint.positionInNation, true);
					if (tipriorityPresetTemplate != null)
					{
						list.AddUnique(tipriorityPresetTemplate);
					}
					else
					{
						flag = false;
					}
				}
			}
			if (flag && list.Count == 1)
			{
				return list[0];
			}
			return null;
		}

		// Token: 0x06005253 RID: 21075 RVA: 0x002470DC File Offset: 0x002452DC
		public static void UpdatePriorityPresetFromChanges(TMP_Dropdown priorityPresetDropdown, TINationState nation, Dictionary<TIPriorityPresetTemplate, int> priorityPresetDictionary = null)
		{
			bool flag;
			TIPriorityPresetTemplate tipriorityPresetTemplate = NationInfoController.GlobalPlayerPresetSetting(nation, out flag);
			if (tipriorityPresetTemplate != null)
			{
				priorityPresetDropdown.captionText.text = tipriorityPresetTemplate.displayName;
				if (priorityPresetDictionary != null)
				{
					int num;
					if (priorityPresetDictionary.TryGetValue(tipriorityPresetTemplate, out num))
					{
						NationInfoController.playDropdownAudio = false;
						priorityPresetDropdown.value = num;
						NationInfoController.playDropdownAudio = true;
					}
				}
				else
				{
					for (int i = 0; i < priorityPresetDropdown.options.Count; i++)
					{
						if (priorityPresetDropdown.options[i].text == tipriorityPresetTemplate.displayName)
						{
							NationInfoController.playDropdownAudio = false;
							priorityPresetDropdown.value = i;
							NationInfoController.playDropdownAudio = true;
						}
					}
				}
			}
			else if (flag)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
				{
					text = Loc.T("UI.Nation.Custom")
				};
				priorityPresetDropdown.options.Add(optionData);
				priorityPresetDropdown.captionText.SetText(optionData.text);
				priorityPresetDropdown.SetValueWithoutNotify(priorityPresetDropdown.options.Count - 1);
			}
			else
			{
				priorityPresetDropdown.captionText.SetText(Loc.T("UI.Nation.NoControl"));
			}
			priorityPresetDropdown.interactable = flag;
			priorityPresetDropdown.gameObject.SetActive(flag);
		}

		// Token: 0x06005254 RID: 21076 RVA: 0x002471F4 File Offset: 0x002453F4
		public static void PopulateNationPriorityDropdown(TMP_Dropdown priorityPresetDropdown, TINationState nation, TIFactionState faction, ref Dictionary<TIPriorityPresetTemplate, int> priorityPresetDictionary)
		{
			priorityPresetDictionary = new Dictionary<TIPriorityPresetTemplate, int>();
			priorityPresetDropdown.options.Clear();
			new List<TIPriorityPresetTemplate>();
			IEnumerable<TIPriorityPresetTemplate> enumerable = from x in TemplateManager.IterateByClass<TIPriorityPresetTemplate>(true)
				orderby x.customDesign descending
				select x;
			int num = 0;
			foreach (TIPriorityPresetTemplate tipriorityPresetTemplate in enumerable)
			{
				if (tipriorityPresetTemplate.ValidPreset(nation, faction))
				{
					priorityPresetDictionary.Add(tipriorityPresetTemplate, num++);
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
					{
						text = tipriorityPresetTemplate.displayName
					};
					priorityPresetDropdown.options.Add(optionData);
				}
			}
			priorityPresetDropdown.RefreshShownValue();
			NationInfoController.UpdatePriorityPresetFromChanges(priorityPresetDropdown, nation, priorityPresetDictionary);
		}

		// Token: 0x06005255 RID: 21077 RVA: 0x002472BC File Offset: 0x002454BC
		public void OnPriorityTemplateChanged()
		{
			if (this.nation != null)
			{
				if (NationInfoController.playDropdownAudio)
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
				}
				using (IEnumerator<TIPriorityPresetTemplate> enumerator = TemplateManager.IterateByClass<TIPriorityPresetTemplate>(true).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.displayName == this.priorityPresetDropdown.options[this.priorityPresetDropdown.value].text)
						{
							TIPriorityPresetTemplate key = this.priorityPresetDictionary.FirstOrDefault<KeyValuePair<TIPriorityPresetTemplate, int>>((KeyValuePair<TIPriorityPresetTemplate, int> x) => x.Value == this.priorityPresetDropdown.value).Key;
							if (key != null)
							{
								using (List<TIControlPoint>.Enumerator enumerator2 = this.nation.controlPoints.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										TIControlPoint ticontrolPoint = enumerator2.Current;
										if (ticontrolPoint.faction == base.activePlayer && !key.MatchesPreset(ticontrolPoint.controlPointPriorities, ticontrolPoint.nation.InvalidPriorities))
										{
											PlayerAction playerAction = new ApplyPriorityPresetToControlPoint(ticontrolPoint, ticontrolPoint.faction, key.dataName);
											base.activePlayer.playerControl.StartAction(playerAction);
										}
									}
									break;
								}
							}
						}
					}
				}
				this.UpdatePriorityList();
				this.UpdateTinyControlPoints();
				NationInfoController.UpdatePriorityPresetFromChanges(this.priorityPresetDropdown, this.nation, this.priorityPresetDictionary);
				using (List<TIObjectiveTemplate>.Enumerator enumerator3 = base.activePlayer.GetObjectivesByTypeAndStatus(ObjectiveType.Tutorial, ObjectiveStatus.Unlocked).GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						if (enumerator3.Current.targetMilestone == CampaignMilestone.TutorialCheckNationalPriority)
						{
							base.activePlayer.CompleteMilestone(CampaignMilestone.TutorialCheckNationalPriority);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06005256 RID: 21078 RVA: 0x00247498 File Offset: 0x00245698
		public static string PrioritySummaryString(PriorityType priority, TINationState nation, bool includeIPSymbol = true)
		{
			TIGlobalConfig global = TemplateManager.global;
			StringBuilder stringBuilder = new StringBuilder();
			if (includeIPSymbol)
			{
				stringBuilder.Append(global.investmentInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.GetRequiredInvestmentPointsForPriority(priority), 7, 0, true, false)).Append(": ");
			}
			switch (priority)
			{
			case PriorityType.Economy:
				stringBuilder.Append(global.perCapitaGDPInlineSpritePath).Append(nation.economyPriorityPerCapitaIncomeChange.ToString("N2")).Append(" ")
					.Append(global.inequalityInlineSpritePath)
					.Append(TIUtilities.FormatSmallNumber(nation.economyPriorityInequalityChange, 7, 0, true, false));
				break;
			case PriorityType.Welfare:
				stringBuilder.Append(global.inequalityInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.welfarePriorityInequalityChange, 7, 1, true, false));
				break;
			case PriorityType.Environment:
				stringBuilder.Append(nation.SustainabilityIconInlinePath());
				if (nation.sustainability <= 0f)
				{
					stringBuilder.Append(Loc.T("UI.Nation.WelfareGHGReductionShort", new object[]
					{
						nation.EnvPriorityCO2Removed().ToString("0.000"),
						nation.EnvPriorityCH4Removed().ToString("0.000"),
						nation.EnvPriorityN2ORemoved().ToString("0.000")
					}));
				}
				else
				{
					stringBuilder.Append(nation.SustainabilityChangeForDisplay(nation.environmentPrioritySustainabilityChange));
				}
				break;
			case PriorityType.Knowledge:
				stringBuilder.Append(global.educationInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.knowledgePriorityEducationChange, 7, 1, true, false)).Append(" ")
					.Append(global.cohesionInlineSpritePath)
					.Append(TIUtilities.FormatSmallNumber(nation.knowledgePriorityCohesionChange, 7, 1, true, false));
				break;
			case PriorityType.Government:
				stringBuilder.Append(global.democracyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.governmentPriorityDemocracyChange, 7, 1, true, false));
				break;
			case PriorityType.Unity:
				stringBuilder.Append(global.cohesionInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.unityPriorityCohesionChange, 7, 1, true, false)).Append(" ")
					.Append(global.educationInlineSpritePath)
					.Append(TIUtilities.FormatSmallNumber(nation.unityPriorityEducationChange, 7, 1, true, false));
				break;
			case PriorityType.Oppression:
				stringBuilder.Append(global.unrestInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.OppressionPriorityUnrestChange, 7, 1, true, false)).Append(" ")
					.Append(global.democracyInlineSpritePath)
					.Append(TIUtilities.FormatSmallNumber(nation.OppressionPriorityDemocracyChange, 7, 1, true, false));
				if (nation.OppressionPriorityCohesionChange != 0f)
				{
					stringBuilder.Append(" ").Append(global.cohesionInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.OppressionPriorityCohesionChange, 7, 1, true, false));
				}
				break;
			case PriorityType.Funding:
				stringBuilder.Append(global.moneyInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.spaceFundingPriorityIncomeChange, 7, 0, true, false)).Append(Loc.T("UI.Nation.Yearly"));
				break;
			case PriorityType.Spoils:
				stringBuilder.Append(global.moneyInlineSpritePath).Append(nation.spoilsPriorityMoney.ToString("N1")).Append(" ")
					.Append(global.inequalityInlineSpritePath)
					.Append(TIUtilities.FormatSmallNumber(nation.spoilsPriorityInequalityChange, 7, 1, true, false))
					.Append(" ")
					.Append(global.democracyInlineSpritePath)
					.Append(TIUtilities.FormatSmallNumber(nation.spoilsPriorityDemocracyChange, 7, 1, true, false))
					.Append(" ")
					.Append(global.sustainabilityInlineSpritePath_Red)
					.Append(nation.SustainabilityChangeForDisplay(nation.spoilsSustainabilityChange));
				break;
			case PriorityType.Civilian_InitiateSpaceflightProgram:
				stringBuilder.Append(global.boostInlineSpritePath).Append(nation.spaceflightInitialBoost);
				break;
			case PriorityType.LaunchFacilities:
			{
				float num = nation.BoostGainLow();
				float num2 = nation.BoostGainHigh();
				if (num != num2)
				{
					stringBuilder.Append(global.boostInlineSpritePath).Append(Loc.T("UI.Nation.BoostRange", new object[]
					{
						TIUtilities.FormatSmallNumber(num, 7, 0, true, false),
						TIUtilities.FormatSmallNumber(num2, 7, 0, true, false)
					})).Append(Loc.T("UI.Nation.Yearly"));
				}
				else
				{
					stringBuilder.Append(global.boostInlineSpritePath).Append(TIUtilities.FormatSmallNumber(num, 7, 0, true, false)).Append(Loc.T("UI.Nation.Yearly"));
				}
				break;
			}
			case PriorityType.MissionControl:
				stringBuilder.Append(global.missionControlInlineSpritePath).Append("1");
				break;
			case PriorityType.Military_FoundMilitary:
				stringBuilder.Append(global.miltechInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.MilitaryTechLevelOnFounding(), 7, 1, true, false));
				break;
			case PriorityType.Military:
				stringBuilder.Append(global.miltechInlineSpritePath).Append(TIUtilities.FormatSmallNumber(nation.militaryPriorityTechLevelChange, 7, 1, true, false));
				break;
			case PriorityType.Military_BuildArmy:
			{
				StringBuilder stringBuilder2 = stringBuilder;
				string text = "UI.Nation.NationalStatTooltipHeader";
				object[] array = new object[2];
				array[0] = global.armyInlineSpritePath;
				int num3 = 1;
				TIRegionState nextArmyRegion = nation.GetNextArmyRegion();
				array[num3] = ((nextArmyRegion != null) ? nextArmyRegion.displayName : null) ?? "Error";
				stringBuilder2.Append(Loc.T(text, array));
				break;
			}
			case PriorityType.Military_BuildNavy:
			{
				StringBuilder stringBuilder3 = stringBuilder;
				string text2 = "UI.Nation.NationalStatTooltipHeader";
				object[] array2 = new object[2];
				array2[0] = global.navyInlineSpritePath;
				int num4 = 1;
				TIArmyState nextNavy = nation.GetNextNavy();
				array2[num4] = ((nextNavy != null) ? nextNavy.displayName : null) ?? "Error";
				stringBuilder3.Append(Loc.T(text2, array2));
				break;
			}
			case PriorityType.Military_InitiateNuclearProgram:
				stringBuilder.Append(global.nukesInlineSpritePath).Append("1");
				break;
			case PriorityType.Military_BuildNuclearWeapons:
				stringBuilder.Append(global.nukesInlineSpritePath).Append("1");
				break;
			case PriorityType.Military_BuildSpaceDefenses:
			{
				StringBuilder stringBuilder4 = stringBuilder;
				string text3 = "UI.Nation.NationalStatTooltipHeader";
				object[] array3 = new object[2];
				array3[0] = global.antiSpaceDefensesInlineSpritePath;
				int num5 = 1;
				TIRegionState nextSpaceDefensesRegion = nation.GetNextSpaceDefensesRegion();
				array3[num5] = ((nextSpaceDefensesRegion != null) ? nextSpaceDefensesRegion.displayName : null) ?? "Error";
				stringBuilder4.Append(Loc.T(text3, array3));
				break;
			}
			case PriorityType.Military_BuildSTOSquadron:
			{
				StringBuilder stringBuilder5 = stringBuilder;
				string text4 = "UI.Nation.NationalStatTooltipHeader";
				object[] array4 = new object[2];
				array4[0] = global.STO_InlineSpritePath;
				int num6 = 1;
				TILaunchFacilityState nextSTOSquadronLocation = nation.GetNextSTOSquadronLocation();
				array4[num6] = ((nextSTOSquadronLocation != null) ? nextSTOSquadronLocation.displayName : null) ?? "Error";
				stringBuilder5.Append(Loc.T(text4, array4));
				break;
			}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06005257 RID: 21079 RVA: 0x00247AC4 File Offset: 0x00245CC4
		private void SetPriorityValueColumnButtonText()
		{
			switch (this.proportionColumnSetting)
			{
			case 0:
				this.proportionColumnButtonText.SetText(Loc.T("UI.Nation.PriorityReadout1"));
				return;
			case 1:
				this.proportionColumnButtonText.SetText(Loc.T("UI.Nation.PriorityReadout2"));
				return;
			case 2:
				this.proportionColumnButtonText.SetText(Loc.T("UI.Nation.PriorityReadout3"));
				return;
			default:
				return;
			}
		}

		// Token: 0x06005258 RID: 21080 RVA: 0x00247B2C File Offset: 0x00245D2C
		public void CyclePriorityValueColumn()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.proportionColumnSetting++;
			if (this.proportionColumnSetting > 2)
			{
				this.proportionColumnSetting = 0;
			}
			this.SetPriorityValueColumnButtonText();
			using (IEnumerator<object> enumerator = this.priorityList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__288.<>p__0 == null)
					{
						NationInfoController.<>o__288.<>p__0 = CallSite<Func<CallSite, object, PriorityListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PriorityListItemController), typeof(NationInfoController)));
					}
					PriorityListItemController priorityListItemController = NationInfoController.<>o__288.<>p__0.Target(NationInfoController.<>o__288.<>p__0, enumerator.Current);
					if (priorityListItemController.gameObject.activeInHierarchy)
					{
						priorityListItemController.SetBonusColumnText(this.nation);
					}
				}
			}
		}

		// Token: 0x06005259 RID: 21081 RVA: 0x00247BFC File Offset: 0x00245DFC
		public void OpenDirectInvestPanel()
		{
			this.CloseAnySecondaryPanels(this.directInvestPanel, false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.directInvestPanel.SetActive(true);
			GameControl.eventManager.AddListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null, base.activePlayer, true, false);
			this.UpdateDirectInvestList(this.nation);
		}

		// Token: 0x0600525A RID: 21082 RVA: 0x00247C5A File Offset: 0x00245E5A
		public void CloseDirectInvestPanel()
		{
			if (this.directInvestPanel != null)
			{
				this.directInvestPanel.SetActive(false);
				GameControl.eventManager.RemoveListener<FactionResourcesUpdated>(new EventManager.EventDelegate<FactionResourcesUpdated>(this.OnFactionResourcesUpdated), null);
			}
		}

		// Token: 0x0600525B RID: 21083 RVA: 0x00247C90 File Offset: 0x00245E90
		private void OnFactionResourcesUpdated(FactionResourcesUpdated e)
		{
			if (this.directInvestPanel != null && this.directInvestPanel.activeInHierarchy && !this.TotalDirectInvestmentCosts().CanAfford(e.council, 1f, null, float.PositiveInfinity))
			{
				this.ResetDirectInvestPanel(this.nation);
			}
		}

		// Token: 0x0600525C RID: 21084 RVA: 0x00247CE4 File Offset: 0x00245EE4
		private void InitializeDirectInvestPanel()
		{
			this.DIHeader_PriorityName.SetText(Loc.T("UI.Nation.DIHeader_PriorityName"));
			this.DIHeader_PerIPCost.SetText(Loc.T("UI.Nation.DIHeader_PerIPCost"));
			this.DIHeader_CurrentSetting.SetText(Loc.T("UI.Nation.DIHeader_CurrentSetting"));
			this.DIHeader_PlannedCost.SetText(Loc.T("UI.Nation.DIHeader_PlannedCost"));
			this.DIHeader_CompletionOutcome.SetText(Loc.T("UI.Nation.DIHeader_CompletionOutcome"));
			this.directInvestConfirmButtonText.SetText(Loc.T("UI.Nation.DI.Confirm"));
			this.directInvestResetButtonText.SetText(Loc.T("UI.Nation.DI.Reset"));
			this.directInvestCancelButtonText.SetText(Loc.T("UI.Nation.DI.Cancel"));
			this.directInvestTotalSpendText.SetText(Loc.T("UI.Nation.DI.TotalSpend"));
			this.directInvestAnnualText.SetText(Loc.T("UI.Nation.DirectInvestAnnualIPs"));
			this.freeInfluenceHeadsUp.SetText(Loc.T("UI.Nation.DI.NoInfluence"));
			this.directInvestListManager.SetListSize<DirectInvestPriorityListItem>(Enums.PriorityTypes.Length, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.directInvestListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__310.<>p__0 == null)
					{
						NationInfoController.<>o__310.<>p__0 = CallSite<Func<CallSite, object, DirectInvestPriorityListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DirectInvestPriorityListItem), typeof(NationInfoController)));
					}
					NationInfoController.<>o__310.<>p__0.Target(NationInfoController.<>o__310.<>p__0, enumerator.Current).Init(this, Enums.PriorityTypes[num++]);
				}
			}
		}

		// Token: 0x0600525D RID: 21085 RVA: 0x00247E78 File Offset: 0x00246078
		private void UpdateDirectInvestList(TINationState nation)
		{
			this.directInvestPanelHeader.SetText(Loc.T("UI.Nation.DI.Header", new object[] { nation.displayName }));
			using (IEnumerator<object> enumerator = this.directInvestListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__311.<>p__0 == null)
					{
						NationInfoController.<>o__311.<>p__0 = CallSite<Func<CallSite, object, DirectInvestPriorityListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DirectInvestPriorityListItem), typeof(NationInfoController)));
					}
					DirectInvestPriorityListItem directInvestPriorityListItem = NationInfoController.<>o__311.<>p__0.Target(NationInfoController.<>o__311.<>p__0, enumerator.Current);
					if (TINationState.EverAllowedForDirectInvest(directInvestPriorityListItem.priority) && nation.ValidPriority(directInvestPriorityListItem.priority))
					{
						directInvestPriorityListItem.SetListItem(nation);
						directInvestPriorityListItem.gameObject.SetActive(true);
					}
					else
					{
						directInvestPriorityListItem.gameObject.SetActive(false);
					}
				}
			}
			this.UpdateDirectInvestSummaryData();
		}

		// Token: 0x0600525E RID: 21086 RVA: 0x00247F6C File Offset: 0x0024616C
		private void ResetDirectInvestPanel(TINationState nation)
		{
			this.plannedDirectInvestments = new Dictionary<PriorityType, float>();
			this.freeInfluenceHeadsUp.gameObject.SetActive(nation.SkipDirectInvestInfluenceCost(GameControl.control.activePlayer));
			foreach (PriorityType priorityType in Enums.PriorityTypes)
			{
				this.plannedDirectInvestments.Add(priorityType, 0f);
			}
			this.UpdateDirectInvestList(nation);
		}

		// Token: 0x0600525F RID: 21087 RVA: 0x00247FD4 File Offset: 0x002461D4
		public void IncreaseDirectInvestment(PriorityType priority, int amount = 1)
		{
			if (this.nation == null)
			{
				return;
			}
			if (!this.plannedDirectInvestments.ContainsKey(priority))
			{
				this.plannedDirectInvestments.Add(priority, 0f);
			}
			TIResourcesCost tiresourcesCost = this.TotalDirectInvestmentCosts();
			TIResourcesCost tiresourcesCost2 = this.nation.SingleDirectInvestmentPrice(priority, amount, base.activePlayer);
			TIResourcesCost tiresourcesCost3 = new TIResourcesCost(tiresourcesCost);
			tiresourcesCost3.SumCosts_NoDuration(tiresourcesCost2);
			if (!tiresourcesCost3.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
			{
				for (int i = amount - 1; i > 0; i--)
				{
					tiresourcesCost2 = this.nation.SingleDirectInvestmentPrice(priority, i, base.activePlayer);
					TIResourcesCost tiresourcesCost4 = new TIResourcesCost(tiresourcesCost);
					tiresourcesCost4.SumCosts_NoDuration(tiresourcesCost2);
					if (tiresourcesCost4.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity))
					{
						amount = i;
						break;
					}
				}
			}
			int num;
			this.nation.CanDirectInvest(base.activePlayer, priority, out num);
			float num2 = this.plannedDirectInvestments.Sum<KeyValuePair<PriorityType, float>>((KeyValuePair<PriorityType, float> x) => x.Value);
			if (num2 + (float)amount > (float)num)
			{
				amount = Mathf.Max(0, Mathd.RoundToInt((double)((float)num - num2)));
			}
			Dictionary<PriorityType, float> dictionary = this.plannedDirectInvestments;
			dictionary[priority] += (float)amount;
			this.UpdateDirectInvestSummaryData();
		}

		// Token: 0x06005260 RID: 21088 RVA: 0x00248121 File Offset: 0x00246321
		public void DecreaseDirectInvestment(PriorityType priority, float amount = 1f)
		{
			if (this.plannedDirectInvestments.ContainsKey(priority))
			{
				this.plannedDirectInvestments[priority] = Mathf.Max(this.plannedDirectInvestments[priority] - amount, 0f);
			}
			this.UpdateDirectInvestSummaryData();
		}

		// Token: 0x06005261 RID: 21089 RVA: 0x0024815C File Offset: 0x0024635C
		private void UpdateDirectInvestSummaryData()
		{
			this.directInvestTotalSpendValue.SetText(this.TotalDirectInvestmentCosts().ToString("Relevant", false, false, null, false, FactionResource.None));
			this.directInvestAnnualIPs.SetText(Loc.T("UI.Nation.DirectInvestAnnualIPsValues", new object[]
			{
				this.nation.directInvestmentedIPsThisYear + this.plannedDirectInvestments.Values.Sum(),
				this.nation.MaxAnnualDirectInvestIPs
			}));
			this.confirmButton.interactable = this.plannedDirectInvestments.Count > 0 && this.plannedDirectInvestments.Values.Max() > 0f && this.TotalDirectInvestmentCosts().CanAfford(GameControl.control.activePlayer, 1f, null, float.PositiveInfinity);
			using (IEnumerator<object> enumerator = this.directInvestListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__315.<>p__0 == null)
					{
						NationInfoController.<>o__315.<>p__0 = CallSite<Func<CallSite, object, DirectInvestPriorityListItem>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(DirectInvestPriorityListItem), typeof(NationInfoController)));
					}
					DirectInvestPriorityListItem directInvestPriorityListItem = NationInfoController.<>o__315.<>p__0.Target(NationInfoController.<>o__315.<>p__0, enumerator.Current);
					if (directInvestPriorityListItem.gameObject.activeInHierarchy)
					{
						directInvestPriorityListItem.UpdateDIButtonsInteractable();
					}
				}
			}
		}

		// Token: 0x06005262 RID: 21090 RVA: 0x002482BC File Offset: 0x002464BC
		public TIResourcesCost TotalDirectInvestmentCosts()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			foreach (PriorityType priorityType in this.plannedDirectInvestments.Keys)
			{
				tiresourcesCost.SumCosts_NoDuration(this.CurrentSingleDirectInvestmentCost(priorityType));
			}
			return tiresourcesCost;
		}

		// Token: 0x06005263 RID: 21091 RVA: 0x00248324 File Offset: 0x00246524
		public TIResourcesCost ProspectiveDirectInvestmentCosts(TIResourcesCost proposedCost)
		{
			TIResourcesCost tiresourcesCost = this.TotalDirectInvestmentCosts();
			tiresourcesCost.SumCosts_NoDuration(proposedCost);
			return tiresourcesCost;
		}

		// Token: 0x06005264 RID: 21092 RVA: 0x00248333 File Offset: 0x00246533
		public TIResourcesCost CurrentSingleDirectInvestmentCost(PriorityType priority)
		{
			if (this.nation == null)
			{
				return new TIResourcesCost();
			}
			return new TIResourcesCost(this.nation.InvestmentPointDirectPurchasePrice(priority, base.activePlayer)).MultiplyCost(this.plannedDirectInvestments[priority]);
		}

		// Token: 0x06005265 RID: 21093 RVA: 0x00248374 File Offset: 0x00246574
		public void ResetDirectInvestments()
		{
			foreach (PriorityType priorityType in this.plannedDirectInvestments.Keys.ToList<PriorityType>())
			{
				this.plannedDirectInvestments[priorityType] = 0f;
			}
			this.UpdateDirectInvestSummaryData();
		}

		// Token: 0x06005266 RID: 21094 RVA: 0x002483E4 File Offset: 0x002465E4
		public void OnClickDirectInvestConfirmButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new DirectInvestAction(base.activePlayer, this.nation, this.plannedDirectInvestments));
			this.ResetDirectInvestments();
			this.UpdateNationPanel();
		}

		// Token: 0x06005267 RID: 21095 RVA: 0x00248430 File Offset: 0x00246630
		public void OnClickDirectInvestResetButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			this.ResetDirectInvestments();
			this.UpdateNationPanel();
		}

		// Token: 0x06005268 RID: 21096 RVA: 0x0024844A File Offset: 0x0024664A
		public void OnClickDirectInvestExitButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseDirectInvestPanel();
		}

		// Token: 0x06005269 RID: 21097 RVA: 0x0024845E File Offset: 0x0024665E
		public void OnClickCancelDirectInvestButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseDirectInvestPanel();
		}

		// Token: 0x0600526A RID: 21098 RVA: 0x00248472 File Offset: 0x00246672
		public void OnDesignPresetButtonSelected()
		{
			this.CloseAnySecondaryPanels(this.designPresetPanel, true);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.UpdateDesignPresetPanel(true, true);
			this.designPresetPanel.SetActive(true);
			this.StartDesignPresetTutorial();
		}

		// Token: 0x0600526B RID: 21099 RVA: 0x002484A8 File Offset: 0x002466A8
		private void InitializeDesignPresetPanel()
		{
			this.CloseDesignPresetPanel();
			this.designPresetPanelButtonText.SetText(Loc.T("UI.Nations.DesignPresetButton"));
			this.presetBuilderHeaderText.SetText(Loc.T("UI.Nations.PresetBuilderHeader"));
			this.savePresetButtonText.SetText(Loc.T("UI.Nations.PresetSaveButtonText"));
			this.resetPresetButtonText.SetText(Loc.T("UI.Nations.PresetResetButtonText"));
			this.setAsDefaultPresetButtonText.SetText(Loc.T("UI.Nations.PresetSetAsDefaultButtonText"));
			this.setAsDefaultTip.SetText("BodyText", Loc.T("UI.Nations.PresetSetAsDefaultButtonHelp"));
			this.applyPresetGloballyButtonText.SetText(Loc.T("UI.Nations.PresetApplyToAll"));
			this.applyGloballyTip.SetText("BodyText", Loc.T("UI.Nations.PresetApplyToAllHelp"));
			this.inputPresetDefaultText.SetText(Loc.T("UI.Nations.EnterPresetName"));
			this.deleteTooltip.SetDelegate("BodyText", new ParameterizedTextField.BuildStringOnTooltipHover(this.GetDeleteTooltip));
			this.ResetCurrentPresets();
			this.designPriorityPresetListManager.SetListSize<PriorityPresetListItemController>(Enums.PriorityTypes.Length + 1, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.designPriorityPresetListManager.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (NationInfoController.<>o__350.<>p__0 == null)
					{
						NationInfoController.<>o__350.<>p__0 = CallSite<Func<CallSite, object, PriorityPresetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PriorityPresetListItemController), typeof(NationInfoController)));
					}
					NationInfoController.<>o__350.<>p__0.Target(NationInfoController.<>o__350.<>p__0, enumerator.Current).Init(this, Enums.PriorityTypes[num++]);
				}
			}
		}

		// Token: 0x0600526C RID: 21100 RVA: 0x00248648 File Offset: 0x00246848
		private void UpdateDesignPresetPanel(bool updateList = true, bool init = false)
		{
			IOrderedEnumerable<TIPriorityPresetTemplate> orderedEnumerable = from x in base.activePlayer.ValidPresetsForFaction()
				orderby x.customDesign descending
				select x;
			bool flag = false;
			string text = string.Empty;
			int num = 0;
			this.duplicatedPreset = null;
			foreach (TIPriorityPresetTemplate tipriorityPresetTemplate in orderedEnumerable)
			{
				if (this.proposedPriorityPreset.MatchesPreset(tipriorityPresetTemplate.GetAllSettings(), new List<PriorityType>()))
				{
					this.duplicatedPreset = tipriorityPresetTemplate;
					text = this.duplicatedPreset.dataName;
					flag = true;
					break;
				}
				num++;
			}
			flag = flag || orderedEnumerable.Any<TIPriorityPresetTemplate>((TIPriorityPresetTemplate x) => x.displayName == this.proposedPriorityPreset.displayName);
			bool flag2 = this.proposedPriorityPreset.ValidPreset_Global() && this.proposedPriorityPreset.displayName != Loc.T("UI.Nation.Custom");
			this.savePresetButton.interactable = flag2 && !flag;
			this.resetPresetButton.interactable = true;
			bool flag3 = orderedEnumerable.Contains(this.duplicatedPreset);
			this.setAsDefaultPresetButton.interactable = flag3 && this.duplicatedPreset != base.activePlayer.defaultPriorityPreset;
			this.applyGloballyButton.interactable = flag3 && base.activePlayer.controlPoints.Count > 0;
			this.deletePresetButton.interactable = flag3 && this.duplicatedPreset.customDesign && this.duplicatedPreset != base.activePlayer.defaultPriorityPreset;
			this.BuildDeleteButtonTooltip(flag3 && this.duplicatedPreset.customDesign && this.duplicatedPreset != base.activePlayer.defaultPriorityPreset);
			int num2 = Mathf.Max(this.proposedPriorityPreset.TotalWeights, 1);
			using (IEnumerator<object> enumerator2 = this.designPriorityPresetListManager.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (NationInfoController.<>o__351.<>p__0 == null)
					{
						NationInfoController.<>o__351.<>p__0 = CallSite<Func<CallSite, object, PriorityPresetListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PriorityPresetListItemController), typeof(NationInfoController)));
					}
					PriorityPresetListItemController priorityPresetListItemController = NationInfoController.<>o__351.<>p__0.Target(NationInfoController.<>o__351.<>p__0, enumerator2.Current);
					priorityPresetListItemController.UpdateListItem(this.proposedPriorityPreset.GetPreset(priorityPresetListItemController.priority), num2);
				}
			}
			if (updateList)
			{
				this.designPriorityPresetDictionary.Clear();
				this.designPriorityPresetDropdown.ClearOptions();
				int num3 = 0;
				foreach (TIPriorityPresetTemplate tipriorityPresetTemplate2 in orderedEnumerable)
				{
					this.designPriorityPresetDictionary.Add(num3++, tipriorityPresetTemplate2);
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
					StringBuilder stringBuilder = new StringBuilder(tipriorityPresetTemplate2.displayName);
					if (tipriorityPresetTemplate2 == base.activePlayer.defaultPriorityPreset)
					{
						stringBuilder.Append(" ").Append(Loc.T("UI.Nations.Default"));
					}
					optionData.text = stringBuilder.ToString();
					this.designPriorityPresetDropdown.options.Add(optionData);
				}
			}
			if (!string.IsNullOrEmpty(this.proposedPriorityPreset.displayName))
			{
				this.inputPresetName.SetTextWithoutNotify(this.proposedPriorityPreset.displayName);
				this.inputPresetDefaultText.SetText(string.Empty);
			}
			else
			{
				this.inputPresetName.text = string.Empty;
				this.inputPresetDefaultText.SetText(Loc.T("UI.Nations.EnterPresetName"));
			}
			if (flag && text != string.Empty)
			{
				StringBuilder stringBuilder2 = new StringBuilder(this.duplicatedPreset.displayName);
				if (flag && this.duplicatedPreset == base.activePlayer.defaultPriorityPreset)
				{
					stringBuilder2.Append(" ").Append(Loc.T("UI.Nations.Default"));
				}
				this.designPriorityPresetDropdown.SetValueWithoutNotify(num);
				this.designPriorityPresetDropdownLabel.SetText(stringBuilder2.ToString());
			}
			else
			{
				this.designPriorityPresetDropdown.SetValueWithoutNotify(this.designPriorityPresetDropdown.options.Count - 1);
				this.designPriorityPresetDropdownLabel.SetText(Loc.T("UI.Nations.Custom"));
			}
			if (init)
			{
				this.designPriorityPresetDropdown.SetValueWithoutNotify(0);
				this.OnPresetDropdownChanged(false);
			}
		}

		// Token: 0x0600526D RID: 21101 RVA: 0x00248ABC File Offset: 0x00246CBC
		private void ResetCurrentPresets()
		{
			this.proposedPriorityPreset = new TIPriorityPresetTemplate(TemplateManager.GenerateDataName("priorityPresetTemplate"));
			this.proposedPriorityPreset.SetDisplayName(string.Empty);
			this.inputPresetName.text = string.Empty;
			this.inputPresetDefaultText.SetText(Loc.T("UI.Nations.EnterPresetName"));
			this.proposedPriorityPreset.customDesign = true;
			this.proposedPriorityPreset.factionName = base.activePlayer.templateName;
			this.proposedPriorityPreset.nationalAIOption = false;
			TIPriorityPresetTemplate.ResetPreset(this.proposedPriorityPreset);
		}

		// Token: 0x0600526E RID: 21102 RVA: 0x00248B4C File Offset: 0x00246D4C
		private void DuplicateSelectedPreset(TIPriorityPresetTemplate presetToDuplicate)
		{
			this.proposedPriorityPreset = new TIPriorityPresetTemplate(TemplateManager.GenerateDataName("priorityPresetTemplate"));
			this.proposedPriorityPreset.SetDisplayName(string.Empty);
			this.inputPresetName.text = string.Empty;
			this.inputPresetDefaultText.SetText(Loc.T("UI.Nations.EnterPresetName"));
			this.proposedPriorityPreset.customDesign = true;
			TIPriorityPresetTemplate.DuplicatePreset(presetToDuplicate, ref this.proposedPriorityPreset);
			this.proposedPriorityPreset.factionName = base.activePlayer.templateName;
			this.proposedPriorityPreset.nationalAIOption = false;
		}

		// Token: 0x0600526F RID: 21103 RVA: 0x00248BE0 File Offset: 0x00246DE0
		private int SetChangeValue(int initialValue, int valueChange)
		{
			int num = initialValue + valueChange;
			if (num > 3)
			{
				num = 0;
			}
			else if (num < 0)
			{
				num = 3;
			}
			return num;
		}

		// Token: 0x06005270 RID: 21104 RVA: 0x00248C00 File Offset: 0x00246E00
		public void ChangePresetValue(PriorityType preset, int valueChange)
		{
			int num = this.SetChangeValue(this.proposedPriorityPreset.GetPreset(preset), valueChange);
			this.proposedPriorityPreset.SetPreset(preset, num);
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005271 RID: 21105 RVA: 0x00248C38 File Offset: 0x00246E38
		public void OnSavePresetButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", true, false);
			base.activePlayer.playerControl.StartAction(new SavePriorityPresetTemplateAction(base.activePlayer, this.proposedPriorityPreset));
			this.inputPresetDefaultText.SetText(Loc.T("UI.Nations.EnterPresetName"));
			this.inputPresetName.text = string.Empty;
			this.DuplicateSelectedPreset(this.proposedPriorityPreset);
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005272 RID: 21106 RVA: 0x00248CAB File Offset: 0x00246EAB
		public void OnResetPresetButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.ResetCurrentPresets();
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005273 RID: 21107 RVA: 0x00248CC7 File Offset: 0x00246EC7
		public void DeletePresetButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new DeletePriorityPresetTemplateAction(base.activePlayer, this.duplicatedPreset));
			this.ResetCurrentPresets();
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005274 RID: 21108 RVA: 0x00248D04 File Offset: 0x00246F04
		public void OnPresetDropdownChanged(bool playAudio = true)
		{
			if (playAudio)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			}
			this.DuplicateSelectedPreset(this.designPriorityPresetDictionary[this.designPriorityPresetDropdown.value]);
			this.UpdateDesignPresetPanel(false, false);
		}

		// Token: 0x06005275 RID: 21109 RVA: 0x00248D39 File Offset: 0x00246F39
		public void OnSetPresetAsDefaultButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SetDefaultPriorityPresetTemplate(base.activePlayer, this.duplicatedPreset.dataName));
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005276 RID: 21110 RVA: 0x00248D78 File Offset: 0x00246F78
		public void OnApplyPresetToAllButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			foreach (TIControlPoint ticontrolPoint in base.activePlayer.controlPoints)
			{
				base.activePlayer.playerControl.StartAction(new ApplyPriorityPresetToControlPoint(ticontrolPoint, base.activePlayer, this.duplicatedPreset.dataName));
			}
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005277 RID: 21111 RVA: 0x00248E04 File Offset: 0x00247004
		public void OnNewPresetNameEntered()
		{
			string proposedPresetName = string.Empty;
			proposedPresetName = this.inputPresetName.text.Trim();
			IOrderedEnumerable<TIPriorityPresetTemplate> orderedEnumerable = from x in base.activePlayer.ValidPresetsForFaction()
				orderby x.customDesign descending
				select x;
			if (proposedPresetName == string.Empty || proposedPresetName.Length == 0 || orderedEnumerable.Any<TIPriorityPresetTemplate>((TIPriorityPresetTemplate x) => x.displayName == proposedPresetName))
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				this.inputPresetName.SetTextWithoutNotify(string.Empty);
			}
			else
			{
				this.proposedPriorityPreset.SetDisplayName(proposedPresetName);
			}
			this.UpdateDesignPresetPanel(true, false);
		}

		// Token: 0x06005278 RID: 21112 RVA: 0x00248ED2 File Offset: 0x002470D2
		public void TextEntryMode_Enter()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06005279 RID: 21113 RVA: 0x00248ED9 File Offset: 0x002470D9
		public void TextEntryMode_End()
		{
			TIInputManager.RestoreKeybindings();
		}

		// Token: 0x0600527A RID: 21114 RVA: 0x00248EE0 File Offset: 0x002470E0
		private string GetDeleteTooltip()
		{
			return this.deleteTooltipText;
		}

		// Token: 0x0600527B RID: 21115 RVA: 0x00248EE8 File Offset: 0x002470E8
		private void BuildDeleteButtonTooltip(bool canDeleteTemplate)
		{
			if (canDeleteTemplate)
			{
				this.deleteTooltipText = Loc.T("UI.Nations.TemplateDelete");
				return;
			}
			this.deleteTooltipText = Loc.T("UI.Nations.CannotTemplateDelete");
		}

		// Token: 0x0600527C RID: 21116 RVA: 0x00248F0E File Offset: 0x0024710E
		public void OnClickCloseDesignPresetPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseDesignPresetPanel();
		}

		// Token: 0x0600527D RID: 21117 RVA: 0x00248F22 File Offset: 0x00247122
		public void CloseDesignPresetPanel()
		{
			if (this.designPresetPanel != null)
			{
				this.designPresetPanel.SetActive(false);
				this.DesignPresetUITutorialController.HideTutorial();
			}
		}

		// Token: 0x0600527E RID: 21118 RVA: 0x00248F4C File Offset: 0x0024714C
		private void UpdateRegionList()
		{
			if (base.activePlayer.CanCountAbductions && this.region.abductions > 0)
			{
				this.abductionsHeader.enabled = true;
			}
			else
			{
				this.abductionsHeader.enabled = false;
			}
			List<TIRegionState> list = (from x in new List<TIRegionState>(this.nation.regions)
				orderby x.isCapital descending, x.population descending
				select x).ToList<TIRegionState>();
			List<TIRegionState> list2 = this.nation.ExternalClaims();
			if (!this.nation.alienNation)
			{
				list.AddRange(list2.OrderByDescending<TIRegionState, float>((TIRegionState x) => x.population));
			}
			this.regionListItemModels.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].ref_nation.extant)
				{
					NationInfoRegionListItemModel nationInfoRegionListItemModel = new NationInfoRegionListItemModel();
					RegionListItem_Data regionListItem_Data = new RegionListItem_Data();
					regionListItem_Data.showInList = true;
					regionListItem_Data.SetRegionData(list[i], list2.Contains(list[i]), this.nation.hostileClaims.Contains(list[i]), !this.nation.hostileClaims.Contains(list[i]) && this.nation.ClaimWillBeHostile(list[i], false), this.nation);
					nationInfoRegionListItemModel.regionListItemData = regionListItem_Data;
					this.regionListItemModels.Add(nationInfoRegionListItemModel);
				}
			}
			this.nationInfoRegionListAdapter.SetItems(this.regionListItemModels);
			this.regionsTabController.SetSize(35f, 27f, 23f, list.Count);
		}

		// Token: 0x0600527F RID: 21119 RVA: 0x0024912C File Offset: 0x0024732C
		public void UpdateCouncilorList()
		{
			List<TICouncilorState> list = new List<TICouncilorState>();
			foreach (TIRegionState tiregionState in this.nation.regions)
			{
				List<TICouncilorState> visibleCouncilorsAtLocation = TIMissionPhaseState.GetVisibleCouncilorsAtLocation(base.activePlayer, tiregionState, TemplateManager.global.intelToSeeNeutralPawn, 1f, false);
				list.AddRange(visibleCouncilorsAtLocation);
			}
			this.councilorList.SetListSize<CouncilorsListItemController>(list.Count, false, false);
			if (list.Count > 0)
			{
				this.councilorsTabButtonObject.SetActive(true);
				int num = 0;
				float num2 = 0f;
				using (IEnumerator<object> enumerator2 = this.councilorList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (NationInfoController.<>o__370.<>p__0 == null)
						{
							NationInfoController.<>o__370.<>p__0 = CallSite<Func<CallSite, object, CouncilorsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(CouncilorsListItemController), typeof(NationInfoController)));
						}
						CouncilorsListItemController councilorsListItemController = NationInfoController.<>o__370.<>p__0.Target(NationInfoController.<>o__370.<>p__0, enumerator2.Current);
						councilorsListItemController.Init(list[num++]);
						councilorsListItemController.UpdateListItem();
						if (num2 == 0f)
						{
							num2 = councilorsListItemController.transform.GetComponent<RectTransform>().sizeDelta.y;
						}
					}
				}
				this.councilorsTabController.SetSize(39f, 0f, 23f, list.Count);
				return;
			}
			this.councilorsTabButtonObject.SetActive(false);
			if (this.nationTabManager.activeTab == this.councilorsTabController)
			{
				this.nationTabManager.Toggle(this.nationTabManager.activeTab);
				this.nationTabManager.ClearActiveTab();
			}
		}

		// Token: 0x06005280 RID: 21120 RVA: 0x00249300 File Offset: 0x00247500
		public void UpdateAllyGrid()
		{
			int childCount = this.allyGrid.transform.childCount;
			List<GameObject> list = new List<GameObject>();
			List<AlliesGridItemController> list2 = new List<AlliesGridItemController>();
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = this.allyGrid.transform.GetChild(i).gameObject;
				list.Add(gameObject);
				list2.Add(gameObject.GetComponent<AlliesGridItemController>());
			}
			int num = 0;
			foreach (TINationState tinationState in this.nation.allies)
			{
				if (tinationState.extant)
				{
					if (num < childCount && list2[num] != null)
					{
						list2[num].UpdateGridItem(this.nation, tinationState);
					}
					else
					{
						GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(this.allyGridItem);
						gameObject2.transform.SetParent(this.allyGrid.transform, false);
						gameObject2.name = tinationState.templateName;
						gameObject2.GetComponent<AlliesGridItemController>().UpdateGridItem(this.nation, tinationState);
					}
					num++;
				}
			}
			for (int j = num; j < childCount; j++)
			{
				global::UnityEngine.Object.Destroy(list[j]);
			}
		}

		// Token: 0x06005281 RID: 21121 RVA: 0x0024944C File Offset: 0x0024764C
		public void UpdateWarGrid()
		{
			int childCount = this.warGrid.transform.childCount;
			List<GameObject> list = new List<GameObject>();
			List<WarGridItemController> list2 = new List<WarGridItemController>();
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = this.warGrid.transform.GetChild(i).gameObject;
				list.Add(gameObject);
				list2.Add(gameObject.GetComponent<WarGridItemController>());
			}
			int num = 0;
			foreach (TINationState tinationState in this.nation.wars.Distinct<TINationState>())
			{
				if (!tinationState.extant)
				{
					if (!tinationState.alienNation)
					{
						continue;
					}
					if (tinationState.armies.Count<TIArmyState>((TIArmyState x) => x.AlienRegularArmy) <= 0)
					{
						if (!this.nation.regions.Any<TIRegionState>((TIRegionState x) => x.alienLanding.Extant()))
						{
							continue;
						}
					}
				}
				if (num < childCount && list2[num] != null)
				{
					list2[num].UpdateGridItem(this.nation, tinationState);
				}
				else
				{
					GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(this.warGridItem);
					gameObject2.transform.SetParent(this.warGrid.transform, false);
					gameObject2.name = tinationState.templateName;
					gameObject2.GetComponent<WarGridItemController>().UpdateGridItem(this.nation, tinationState);
				}
				num++;
			}
			for (int j = num; j < childCount; j++)
			{
				global::UnityEngine.Object.Destroy(list[j]);
			}
		}

		// Token: 0x06005282 RID: 21122 RVA: 0x00249604 File Offset: 0x00247804
		public void UpdateRivalryGrid()
		{
			int childCount = this.rivalGrid.transform.childCount;
			List<GameObject> list = new List<GameObject>();
			List<RivalsGridItemController> list2 = new List<RivalsGridItemController>();
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = this.rivalGrid.transform.GetChild(i).gameObject;
				list.Add(gameObject);
				list2.Add(gameObject.GetComponent<RivalsGridItemController>());
			}
			int num = 0;
			foreach (TINationState tinationState in this.nation.rivals)
			{
				if (tinationState.extant)
				{
					if (num < childCount && list2[num] != null)
					{
						list2[num].UpdateGridItem(this.nation, tinationState);
					}
					else
					{
						GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(this.rivalGridItem);
						gameObject2.transform.SetParent(this.rivalGrid.transform, false);
						gameObject2.name = tinationState.templateName;
						gameObject2.GetComponent<RivalsGridItemController>().UpdateGridItem(this.nation, tinationState);
					}
					num++;
				}
			}
			for (int j = num; j < childCount; j++)
			{
				global::UnityEngine.Object.Destroy(list[j]);
			}
		}

		// Token: 0x06005283 RID: 21123 RVA: 0x00249750 File Offset: 0x00247950
		public void OpenSelfDisablePanel()
		{
			this.CloseAnySecondaryPanels(this.confirmDisableControlPointPanel, false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.confirmDisableControlPointBodyText.SetText(Loc.T("UI.Nation.DisableControlPointsText", new object[] { TemplateManager.global.selfDisableControlPointDuration_months.ToString("N0") }));
			this.confirmDisableControlPointPanel.SetActive(true);
		}

		// Token: 0x06005284 RID: 21124 RVA: 0x002497B5 File Offset: 0x002479B5
		public void CloseSelfDisablePanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.confirmDisableControlPointPanel.SetActive(false);
		}

		// Token: 0x06005285 RID: 21125 RVA: 0x002497D0 File Offset: 0x002479D0
		public void ConfirmSelfDisableControlPoints()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			base.activePlayer.playerControl.StartAction(new SelfDisableControlPoints(base.activePlayer, this.nation));
			this.AssignControlPoints();
			this.confirmDisableControlPointPanel.SetActive(false);
		}

		// Token: 0x06005286 RID: 21126 RVA: 0x0024981C File Offset: 0x00247A1C
		public void OnToggleAutoAbandon()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			base.activePlayer.playerControl.StartAction(new SetNationAutoAbandon(base.activePlayer, this.nation, this.autoAbandonToggle.isOn));
		}

		// Token: 0x06005287 RID: 21127 RVA: 0x00249858 File Offset: 0x00247A58
		private void ShowMapObjectPanel(TIRegionEntityState regionEntity)
		{
			if (!this.Visible() || !this.mapObjectDetailCanvas.enabled)
			{
				if (!this.Visible())
				{
					this.Show();
				}
				this.mapObjectDetailCanvas.enabled = true;
				base.canvasManager.SetActiveInfoPanel(InfoPanel.EarthMapObjectDetail, 0f);
				GameControl.eventManager.AddListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.AutocloseMapObjectPanel), null, null, true, false);
				GameControl.eventManager.AddListener<RegionDataUpdated>(new EventManager.EventDelegate<RegionDataUpdated>(this.UpdateMapObjectPanel), null, null, true, false);
			}
			this.displayedRegionLocationState = regionEntity;
			this.UpdateMapObjectPanel(regionEntity);
			this.HideTutorials();
		}

		// Token: 0x06005288 RID: 21128 RVA: 0x002498F0 File Offset: 0x00247AF0
		private void CloseMapObjectPanel()
		{
			if (this.mapObjectDetailCanvas != null)
			{
				GeneralControlsController.ConditionalCancelSelectedOtherState(this.displayedRegionLocationState);
				this.mapObjectDetailCanvas.enabled = false;
				this.displayedRegionLocationState = null;
				GameControl.eventManager.RemoveListener<InfoScreenOpened>(new EventManager.EventDelegate<InfoScreenOpened>(this.AutocloseMapObjectPanel), null);
				GameControl.eventManager.RemoveListener<RegionDataUpdated>(new EventManager.EventDelegate<RegionDataUpdated>(this.UpdateMapObjectPanel), null);
				GameControl.eventManager.RemoveListener<AlienRegionEntityUpdated>(new EventManager.EventDelegate<AlienRegionEntityUpdated>(this.OnRegionEntityUpdated), null);
				this.CheckforMainCanvasClose();
				this.HideTutorials();
			}
		}

		// Token: 0x06005289 RID: 21129 RVA: 0x0024997C File Offset: 0x00247B7C
		private void UpdateMapObjectPanel(TIRegionEntityState regionEntity)
		{
			GameControl.eventManager.RemoveListener<AlienRegionEntityUpdated>(new EventManager.EventDelegate<AlienRegionEntityUpdated>(this.OnRegionEntityUpdated), null);
			if (!regionEntity.Extant())
			{
				this.CloseMapObjectPanel();
				return;
			}
			this.mapObjectDetailMainHeadline.SetText(regionEntity.displayName);
			this.mapObjectFlag.sprite = regionEntity.ref_region.nation.flag;
			this.mapObjectDetailHeader.SetText(regionEntity.descriptor);
			this.mapObjectDetailLocation.SetText(Loc.T("UI.Nation.MapObjectLocation", new object[]
			{
				regionEntity.ref_region.displayName,
				regionEntity.ref_region.nation.displayNameWithArticle
			}));
			this.mapObjectDetailExplainerText.SetText(regionEntity.description);
			if (regionEntity.isRegionSpaceFacility)
			{
				TIRegionSpaceFacilityState ref_regionSpaceFacility = regionEntity.ref_regionSpaceFacility;
				switch (ref_regionSpaceFacility.spaceFacilityType)
				{
				case SpaceFacilityType.launchFacility:
					GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathBoostIcon, this.statPanelIcon);
					this.statPanelValueText.SetText(TIUtilities.FormatBigOrSmallNumber(regionEntity.ref_region.boostPerYear_dekatons, 1, 7, 0, false, false));
					this.statPanelObject.SetActive(true);
					if (regionEntity.ref_region.nation.canBuildSTOSquadrons)
					{
						this.statPanelObject2.SetActive(true);
						this.statPanelValueText2.SetText(Loc.T("UI.NationPriorityAccumulation", new object[]
						{
							ref_regionSpaceFacility.region.availableSTOFighters.ToString(),
							ref_regionSpaceFacility.region.numSTOFighters.ToString()
						}));
						this.mapObjectButtonPanelButton2.gameObject.SetActive(true);
						LaunchSTOInterceptorsOperation launchSTOInterceptorsOperation = new LaunchSTOInterceptorsOperation();
						this.mapObjectButtonPanelButton2.interactable = ref_regionSpaceFacility.region.numSTOFighters > 0 && ref_regionSpaceFacility.region.nation.executiveFaction == base.activePlayer && launchSTOInterceptorsOperation.OpVisibleToActor(base.activePlayer, ref_regionSpaceFacility.ref_spaceBody);
					}
					else
					{
						this.statPanelObject2.SetActive(false);
						this.mapObjectButtonPanelButton2.gameObject.SetActive(false);
					}
					this.mapObjectButtonPanelObject.SetActive(true);
					this.mapObjectButtonPanelButton.interactable = regionEntity.ref_region.nation.FactionHasControlPoint(base.activePlayer);
					this.mapObjectButtonText.SetText(Loc.T("UI.Nation.MapObjectPanelLaunch"));
					this.mapObjectButtonText2.SetText(Loc.T("UI.Nation.MapObjectPanelFighters"));
					break;
				case SpaceFacilityType.missionControlFacility:
					GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathMissionControlIcon, this.statPanelIcon);
					this.statPanelValueText.SetText(regionEntity.ref_region.missionControl.ToString("N0"));
					this.statPanelObject.SetActive(true);
					this.statPanelObject2.SetActive(false);
					this.mapObjectButtonPanelObject.SetActive(false);
					break;
				case SpaceFacilityType.spaceDefenseFacility:
					this.statPanelObject.SetActive(false);
					this.statPanelObject2.SetActive(false);
					this.mapObjectButtonPanelObject.SetActive(false);
					break;
				}
			}
			else
			{
				TIRegionAlienEntityState tiregionAlienEntityState = regionEntity as TIRegionAlienEntityState;
				if (tiregionAlienEntityState != null)
				{
					GameControl.eventManager.AddListener<AlienRegionEntityUpdated>(new EventManager.EventDelegate<AlienRegionEntityUpdated>(this.OnRegionEntityUpdated), null, tiregionAlienEntityState, false, false);
				}
				this.statPanelObject.SetActive(false);
				this.statPanelObject2.SetActive(false);
				this.mapObjectButtonPanelObject.SetActive(false);
			}
			if (!string.IsNullOrEmpty(regionEntity.GetIllustrationPath(base.activePlayer)))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(regionEntity.GetIllustrationPath(base.activePlayer), this.mapObjectDetailIllustration);
				this.mapObjectDetailIllustration.color = Color.white;
				return;
			}
			this.mapObjectDetailIllustration.color = Color.black;
		}

		// Token: 0x0600528A RID: 21130 RVA: 0x00249D14 File Offset: 0x00247F14
		public string RegionSTOFightersTip(TIRegionState region)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Loc.T("UI.Nation.MapObjectPanelFightersTip", new object[]
			{
				region.availableSTOFighters.ToString(),
				region.numSTOFighters.ToString(),
				region.maxSTOFighters.ToString()
			}));
			if (region.STOFighterCooldownExpiry.Count > 0)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.Nation.FighterAvailDate", new object[]
				{
					region.STOFighterCooldownExpiry.Min<TIDateTime>().ToCustomDateString(),
					region.STOFighterCooldownExpiry.Max<TIDateTime>().ToCustomDateString()
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600528B RID: 21131 RVA: 0x00249DC4 File Offset: 0x00247FC4
		public void OnRegionEntityUpdated(AlienRegionEntityUpdated e)
		{
			if (e.alienEntityState == this.displayedRegionLocationState && !e.alienEntityState.VisibleToFaction(GameControl.control.activePlayer))
			{
				this.CloseMapObjectPanel();
			}
		}

		// Token: 0x0600528C RID: 21132 RVA: 0x00249DF6 File Offset: 0x00247FF6
		public void OnMapObjectLaunchButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIUtilities.GotoGameState(this.displayedRegionLocationState.ref_region.ref_spaceBody, true, true, true, false, true, -1f);
		}

		// Token: 0x0600528D RID: 21133 RVA: 0x00249E23 File Offset: 0x00248023
		public void OnMapObjectLaunchButton2Pressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Acknowledge", false, false);
			TIUtilities.GotoGameState(this.displayedRegionLocationState.ref_region.ref_spaceBody, true, true, true, false, true, -1f);
		}

		// Token: 0x0600528E RID: 21134 RVA: 0x00249E50 File Offset: 0x00248050
		public void OnMapObjectPanelExitSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			base.canvasManager.SetActiveInfoPanel(InfoPanel.None, 0f);
		}

		// Token: 0x0600528F RID: 21135 RVA: 0x00249E6F File Offset: 0x0024806F
		public void OnMapObjectFlagSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			TIUtilities.GotoGameState(this.displayedRegionLocationState.ref_region, true, true, true, true, false, -1f);
		}

		// Token: 0x06005290 RID: 21136 RVA: 0x00249E97 File Offset: 0x00248097
		public void OnGotoMapObjectSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericSelect", false, false);
			TIUtilities.GotoGameState(this.displayedRegionLocationState, true, false, true, true, false, -1f);
		}

		// Token: 0x17000EEA RID: 3818
		// (get) Token: 0x06005291 RID: 21137 RVA: 0x00249EBA File Offset: 0x002480BA
		private float policyChangeInfluenceCost
		{
			get
			{
				return TIFactionState.setPolicyMission.cost.value;
			}
		}

		// Token: 0x06005292 RID: 21138 RVA: 0x00249ECC File Offset: 0x002480CC
		public void InitializeRelationsPanel()
		{
			this.nationRelationsManagerPanel.SetActive(false);
			int num = GameStateManager.AllFactions().Length;
			this.relationsTabIndices = new Dictionary<int, TIFactionState>();
			this.relationsTabIndices.Add(0, base.activePlayer);
			int num2 = 1;
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState != base.activePlayer)
				{
					this.relationsTabIndices.Add(num2++, tifactionState);
				}
			}
			this.relationsTabIndices.Add(8, null);
			for (int j = 0; j < num; j++)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.relationsTabIndices[j].factionIcon64path, this.relationsTabImages[j]);
			}
			for (int k = 0; k < this.relationsTabButtonObjects.Count; k++)
			{
				if (k >= num && k != 8)
				{
					this.relationsTabButtonObjects[k].SetActive(false);
				}
			}
			this.unalignedNationsTabText.SetText(Loc.T("UI.Nation.Relations.Unaligned"));
			this.massChangesText.SetText(Loc.T("UI.Nation.Relations.ChangeAll"));
			this.allyText.SetText(Loc.T("UI.Nation.Relations.Ally"));
			this.normalText.SetText(Loc.T("UI.Nation.Relations.Normal"));
			this.rivalText.SetText(Loc.T("UI.Nation.Relations.Rival"));
			this.warText.SetText(Loc.T("UI.Nation.Relations.Wars"));
			this.improveRelationsColumnHeaderText.SetText(Loc.T("UI.Nation.Relations.Cooldown"));
			this.relations_acceptButtonText.SetText(Loc.T("UI.Nation.DI.Confirm"));
			this.relations_resetButtonText.SetText(Loc.T("UI.Nation.DI.Reset"));
			this.relations_closeButtonText.SetText(Loc.T("UI.Nation.DI.Cancel"));
		}

		// Token: 0x06005293 RID: 21139 RVA: 0x0024A098 File Offset: 0x00248298
		public void OpenRelationsPanel()
		{
			this.CloseAnySecondaryPanels(this.nationRelationsManagerPanel, false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.nationRelationsHeader.SetText(Loc.T("UI.Nation.Relations.Header", new object[] { this.nation.displayName }));
			GameStateManager.AllFactions().ToDictionary<TIFactionState, TIFactionState, List<TINationState>>((TIFactionState x) => x, (TIFactionState x) => new List<TINationState>(x.executiveNations));
			int num = GameStateManager.AllFactions().Length;
			for (int i = 0; i <= 8; i++)
			{
				if (i == 8)
				{
					this.factionRelationsPaneControllers[i].SetFactionAndNation(this.relationsTabIndices[8], this.nation, this);
				}
				else if (i < num)
				{
					if (this.relationsTabIndices[i].IsAlienFaction)
					{
						this.relationsTabButtonObjects[i].SetActive(GameStateManager.AlienNation().extant);
					}
					this.factionRelationsPaneControllers[i].SetFactionAndNation(this.relationsTabIndices[i], this.nation, this);
				}
			}
			this.ResetProposedChanges();
			this.nationRelationsManagerPanel.SetActive(true);
			this.tabManager.Toggle(this.factionRelationsPaneControllers[0].gameObject.GetComponent<TabbedPaneController>());
			this.ignoreToggles = true;
			this.AllyAllToggle.isOn = false;
			this.AllyToNormalAllToggle.isOn = false;
			this.RivalToNormalAllToggle.isOn = false;
			this.RivalAllToggle.isOn = false;
			this.ignoreToggles = false;
		}

		// Token: 0x06005294 RID: 21140 RVA: 0x0024A23C File Offset: 0x0024843C
		public void UpdateRelationsPanel()
		{
			this.ignoreToggles = true;
			if (this.proposedRelationsChanges.Count > 0)
			{
				this.costProposalText.SetText(Loc.T("UI.Nation.Relations.TotalCost", new object[] { this.relationshipChangesCost.ToString("Relevant", false, false, null, false, FactionResource.None) }));
				this.acceptChangesButton.interactable = this.relationshipChangesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				this.ignoreToggles = true;
				NationRelationsPaneController component = this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>();
				Toggle allyAllToggle = this.AllyAllToggle;
				bool flag;
				if (component.Allof(RelationChange.NormalToAlly))
				{
					flag = this.proposedRelationsChanges.All<KeyValuePair<TINationState, RelationChange>>((KeyValuePair<TINationState, RelationChange> x) => x.Value == RelationChange.NormalToAlly);
				}
				else
				{
					flag = false;
				}
				allyAllToggle.isOn = flag;
				Toggle allyToNormalAllToggle = this.AllyToNormalAllToggle;
				bool flag2;
				if (component.Allof(RelationChange.AllyToNormal))
				{
					flag2 = this.proposedRelationsChanges.All<KeyValuePair<TINationState, RelationChange>>((KeyValuePair<TINationState, RelationChange> x) => x.Value == RelationChange.AllyToNormal);
				}
				else
				{
					flag2 = false;
				}
				allyToNormalAllToggle.isOn = flag2;
				Toggle rivalToNormalAllToggle = this.RivalToNormalAllToggle;
				bool flag3;
				if (component.Allof(RelationChange.RivalToNormal))
				{
					flag3 = this.proposedRelationsChanges.All<KeyValuePair<TINationState, RelationChange>>((KeyValuePair<TINationState, RelationChange> x) => x.Value == RelationChange.RivalToNormal);
				}
				else
				{
					flag3 = false;
				}
				rivalToNormalAllToggle.isOn = flag3;
				Toggle rivalAllToggle = this.RivalAllToggle;
				bool flag4;
				if (component.Allof(RelationChange.NormalToRival))
				{
					flag4 = this.proposedRelationsChanges.All<KeyValuePair<TINationState, RelationChange>>((KeyValuePair<TINationState, RelationChange> x) => x.Value == RelationChange.NormalToRival);
				}
				else
				{
					flag4 = false;
				}
				rivalAllToggle.isOn = flag4;
			}
			else
			{
				this.costProposalText.SetText(string.Empty);
				this.acceptChangesButton.interactable = false;
				this.AllyAllToggle.isOn = false;
				this.AllyToNormalAllToggle.isOn = false;
				this.RivalToNormalAllToggle.isOn = false;
				this.RivalAllToggle.isOn = false;
			}
			this.ignoreToggles = false;
		}

		// Token: 0x06005295 RID: 21141 RVA: 0x0024A431 File Offset: 0x00248631
		public void UpdateRelationsList()
		{
			if (this.tabManager.activeTab != null)
			{
				this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>().UpdateNationRelationsList();
			}
		}

		// Token: 0x06005296 RID: 21142 RVA: 0x0024A460 File Offset: 0x00248660
		public void ResetProposedChanges()
		{
			this.proposedRelationsChanges = new Dictionary<TINationState, RelationChange>();
			this.relationshipChangesCost = new TIResourcesCost();
			if (this.tabManager.activeTab != null)
			{
				this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>().UpdateNationRelationsList();
			}
			this.UpdateRelationsPanel();
		}

		// Token: 0x06005297 RID: 21143 RVA: 0x0024A4B6 File Offset: 0x002486B6
		public void AddProposedRelationshipChange(TINationState nation, RelationChange change)
		{
			this.proposedRelationsChanges[nation] = change;
			this.relationshipChangesCost.AddCost(FactionResource.Influence, this.policyChangeInfluenceCost, true);
			this.UpdateRelationsPanel();
		}

		// Token: 0x06005298 RID: 21144 RVA: 0x0024A4DE File Offset: 0x002486DE
		public void RemoveProposedRelationshipChange(TINationState nation)
		{
			this.proposedRelationsChanges.Remove(nation);
			this.relationshipChangesCost.AddCost(FactionResource.Influence, -this.policyChangeInfluenceCost, true);
			this.UpdateRelationsPanel();
		}

		// Token: 0x06005299 RID: 21145 RVA: 0x0024A508 File Offset: 0x00248708
		public void OnClickAllyAllToggle()
		{
			if (!this.ignoreToggles && this.tabManager.activeTab != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				using (IEnumerator<object> enumerator = this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>().nationsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (NationInfoController.<>o__438.<>p__0 == null)
						{
							NationInfoController.<>o__438.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationInfoController)));
						}
						NationRelationsListItemController nationRelationsListItemController = NationInfoController.<>o__438.<>p__0.Target(NationInfoController.<>o__438.<>p__0, enumerator.Current);
						if (this.nation.CanAlly(nationRelationsListItemController.otherNation, false))
						{
							nationRelationsListItemController.AllyToggleChange();
						}
					}
				}
			}
		}

		// Token: 0x0600529A RID: 21146 RVA: 0x0024A5EC File Offset: 0x002487EC
		public void OnClickAllyToNormalAllToggle()
		{
			if (!this.ignoreToggles && this.tabManager.activeTab != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				using (IEnumerator<object> enumerator = this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>().nationsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (NationInfoController.<>o__439.<>p__0 == null)
						{
							NationInfoController.<>o__439.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationInfoController)));
						}
						NationRelationsListItemController nationRelationsListItemController = NationInfoController.<>o__439.<>p__0.Target(NationInfoController.<>o__439.<>p__0, enumerator.Current);
						if (this.nation.CanEndAlliance(nationRelationsListItemController.otherNation))
						{
							nationRelationsListItemController.NormalToggleChange();
						}
					}
				}
			}
		}

		// Token: 0x0600529B RID: 21147 RVA: 0x0024A6CC File Offset: 0x002488CC
		public void OnClickRivalToNormalAllToggle()
		{
			if (!this.ignoreToggles && this.tabManager.activeTab != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				using (IEnumerator<object> enumerator = this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>().nationsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (NationInfoController.<>o__440.<>p__0 == null)
						{
							NationInfoController.<>o__440.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationInfoController)));
						}
						NationRelationsListItemController nationRelationsListItemController = NationInfoController.<>o__440.<>p__0.Target(NationInfoController.<>o__440.<>p__0, enumerator.Current);
						if (this.nation.CanEndRivalry(nationRelationsListItemController.otherNation))
						{
							nationRelationsListItemController.NormalToggleChange();
						}
					}
				}
			}
		}

		// Token: 0x0600529C RID: 21148 RVA: 0x0024A7AC File Offset: 0x002489AC
		public void OnClickRivalAllToggle()
		{
			if (!this.ignoreToggles && this.tabManager.activeTab != null)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				using (IEnumerator<object> enumerator = this.tabManager.activeTab.gameObject.GetComponent<NationRelationsPaneController>().nationsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (NationInfoController.<>o__441.<>p__0 == null)
						{
							NationInfoController.<>o__441.<>p__0 = CallSite<Func<CallSite, object, NationRelationsListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(NationRelationsListItemController), typeof(NationInfoController)));
						}
						NationRelationsListItemController nationRelationsListItemController = NationInfoController.<>o__441.<>p__0.Target(NationInfoController.<>o__441.<>p__0, enumerator.Current);
						if (this.nation.CanRival(nationRelationsListItemController.otherNation))
						{
							nationRelationsListItemController.RivalToggleChange();
						}
					}
				}
			}
		}

		// Token: 0x0600529D RID: 21149 RVA: 0x0024A88C File Offset: 0x00248A8C
		public void OnClickAcceptRelationsChangesButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			foreach (TINationState tinationState in this.proposedRelationsChanges.Keys)
			{
				this.nation.HandleFactionLevelRelationshipChanges(tinationState, this.proposedRelationsChanges[tinationState]);
			}
			this.ResetProposedChanges();
		}

		// Token: 0x0600529E RID: 21150 RVA: 0x0024A908 File Offset: 0x00248B08
		public void OnClickResetRelationsButton()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Cancel", false, false);
			this.ResetProposedChanges();
		}

		// Token: 0x0600529F RID: 21151 RVA: 0x0024A91C File Offset: 0x00248B1C
		public void OnClickCloseRelationsPanel()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseRelationsPanel();
		}

		// Token: 0x060052A0 RID: 21152 RVA: 0x0024A930 File Offset: 0x00248B30
		public void CloseRelationsPanel()
		{
			if (this.nationRelationsManagerPanel != null)
			{
				this.nationRelationsManagerPanel.SetActive(false);
			}
		}

		// Token: 0x17000EEB RID: 3819
		// (get) Token: 0x060052A1 RID: 21153 RVA: 0x0024A94C File Offset: 0x00248B4C
		private bool currentlyNuclearTargeting
		{
			get
			{
				return this.nuclearTargeting != null && this.nuclearTargeting.activated;
			}
		}

		// Token: 0x060052A2 RID: 21154 RVA: 0x0024A963 File Offset: 0x00248B63
		private void InitNuclearOption()
		{
			this.nuclearWeaponsPanel.SetActive(false);
			this.nuclearConfirmButtonText.SetText(Loc.T("UI.Nation.Nuclear.Confirm"));
			this.nuclearCancelButtonText.SetText(Loc.T("UI.Nation.Nuclear.Cancel"));
		}

		// Token: 0x060052A3 RID: 21155 RVA: 0x0024A99C File Offset: 0x00248B9C
		public void OnNuclearWeaponsSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_Query", false, false);
			this.nukingNation = this.nation;
			this.CloseAnySecondaryPanels(this.nuclearWeaponsPanel, false);
			if (GeneralControlsController.UIPlayerInTargetingMode)
			{
				if (GeneralControlsController.UITargetingMode is TIOperationTargeting)
				{
					(base.canvasManager.OperationCanvasController as OperationCanvasController).DisableCurrentOperation(true);
				}
				else if (GeneralControlsController.UITargetingMode is TIMissionTargeting)
				{
					(base.canvasManager.CouncilorMissionController as CouncilorMissionCanvasController).ShutdownTargetSelection(true);
				}
			}
			this.nuclearWeaponsPanelHeader.SetText(Loc.T("UI.Nation.Nuclear.Header", new object[] { this.nation.displayName }));
			this.nationFlag.sprite = this.nation.flag;
			this.nationFlag2.sprite = this.nation.flag;
			this.currentNuclearTarget = null;
			if (this.nuclearTargeting == null)
			{
				this.nuclearTargeting = Activator.CreateInstance(typeof(TIOperationTargeting_Region)) as TIOperationTargeting_Region;
			}
			this.nuclearTargeting.Init(OperationsManager.nationOperations[0], this.nation, null);
			this.nuclearTargeting.Activate(null);
			GameControl.eventManager.AddListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.NewNuclearTarget), null, null, true, false);
			this.nuclearConfirmButton.interactable = false;
			this.nuclearWeaponsPanel.SetActive(true);
			string text;
			if (!this.nation.spaceFlightProgram)
			{
				if (this.nation.navalFreedom)
				{
					text = Loc.T("UI.Nation.Nuclear.Desc.Naval");
				}
				else
				{
					text = Loc.T("UI.Nation.Nuclear.Desc.Adjacent");
				}
			}
			else
			{
				text = Loc.T("UI.Nation.Nuclear.Desc.ICBM");
			}
			this.nuclearWeaponsPanelText.SetText(Loc.T("UI.Nation.Nuclear.Desc", new object[] { text }));
			if (this.nuclearTargeting.GetPossibleTargets.Count == 0)
			{
				this.nuclearWeaponsTargetText.SetText(TIUtilities.HighlightLine(Loc.T("UI.Nation.Nuclear.NoTargets")));
				return;
			}
			this.nuclearWeaponsTargetText.SetText(Loc.T("UI.Nation.Nuclear.CurrentTarget", new object[] { Loc.T("UI.Nation.Nuclear.NoTarget") }));
		}

		// Token: 0x060052A4 RID: 21156 RVA: 0x0024ABA8 File Offset: 0x00248DA8
		public void NewNuclearTarget(OperationTargettedEvent e)
		{
			this.currentNuclearTarget = e.target.ref_region;
			this.nuclearConfirmButton.interactable = this.currentNuclearTarget != null;
			if (this.currentNuclearTarget == null)
			{
				this.nuclearWeaponsTargetText.SetText(Loc.T("UI.Nation.Nuclear.CurrentTarget", new object[] { Loc.T("UI.Nation.Nuclear.NoTarget") }));
				return;
			}
			this.nuclearWeaponsTargetText.SetText(TIUtilities.RedLine(Loc.T("UI.Nation.Nuclear.CurrentTarget", new object[] { Loc.T("UI.Nation.Nuclear.Target", new object[]
			{
				this.currentNuclearTarget.displayName,
				this.currentNuclearTarget.nation.displayName
			}) })));
		}

		// Token: 0x060052A5 RID: 21157 RVA: 0x0024AC68 File Offset: 0x00248E68
		public void OnConfirmLaunch()
		{
			if (this.nukingNation.executiveFaction == GameControl.control.activePlayer)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ConfirmArmyOperation", false, false);
				TIPolicyOption tipolicyOption = new EmployNuclearWeaponsOption();
				this.nuclearTargeting.Shutdown();
				this.nuclearWeaponsPanel.SetActive(false);
				tipolicyOption.OnConfirm(this.nukingNation, this.currentNuclearTarget);
				base.activePlayer.UnlockAchievement("fireNukeBarrage");
				MusicController.Instance.PlayFanfare("event:/Music/Fanfares/trig_Nuclear_Warfare");
				GameControl.eventManager.RemoveListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.NewNuclearTarget), null);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
			this.CloseNuclearWeaponsPanel();
		}

		// Token: 0x060052A6 RID: 21158 RVA: 0x0024AD13 File Offset: 0x00248F13
		public void OnCancelLaunch()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
			this.CloseNuclearWeaponsPanel();
		}

		// Token: 0x060052A7 RID: 21159 RVA: 0x0024AD27 File Offset: 0x00248F27
		public void CloseNuclearWeaponsPanel()
		{
			this.nuclearWeaponsPanel.SetActive(false);
			if (this.nuclearTargeting != null)
			{
				GameControl.eventManager.RemoveListener<OperationTargettedEvent>(new EventManager.EventDelegate<OperationTargettedEvent>(this.NewNuclearTarget), null);
				this.nuclearTargeting.Shutdown();
			}
		}

		// Token: 0x060052A8 RID: 21160 RVA: 0x0024AD5F File Offset: 0x00248F5F
		public void StartPrioritiesTutorial()
		{
			this.NationInfoCanvasPrioritiesUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_NationsInfoCanvas_Priorities, false, true);
		}

		// Token: 0x060052A9 RID: 21161 RVA: 0x0024AD73 File Offset: 0x00248F73
		public void StartNationPanelTutorial()
		{
			this.NationInfoCanvasUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_NationsInfoCanvas_NationPanel, false, true);
		}

		// Token: 0x060052AA RID: 21162 RVA: 0x0024AD87 File Offset: 0x00248F87
		public void StartBuildExofighterTutorial()
		{
			this.ExofightersUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_NationInfoCanvas_BuildExofighters, false, true);
		}

		// Token: 0x060052AB RID: 21163 RVA: 0x0024AD9B File Offset: 0x00248F9B
		public void StartDesignPresetTutorial()
		{
			this.DesignPresetUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_NationsScreenCanvas_DesignPreset, true, true);
		}

		// Token: 0x060052AC RID: 21164 RVA: 0x0024ADAF File Offset: 0x00248FAF
		public void HidePrioritiesTutorial()
		{
			this.NationInfoCanvasPrioritiesUITutorialController.HideTutorial();
		}

		// Token: 0x060052AD RID: 21165 RVA: 0x0024ADBC File Offset: 0x00248FBC
		public void HideTutorials()
		{
			this.NationInfoCanvasUITutorialController.HideTutorial();
			this.HidePrioritiesTutorial();
			this.ExofightersUITutorialController.HideTutorial();
			this.DesignPresetUITutorialController.HideTutorial();
		}

		// Token: 0x060052AE RID: 21166 RVA: 0x0024ADE5 File Offset: 0x00248FE5
		public void OpenPrioritiesTab()
		{
			if (this.nationTabManager.activeTab != this.prioritiesTabController)
			{
				this.prioritiesTabButtonObject.GetComponent<Button>().onClick.Invoke();
			}
		}

		// Token: 0x060052AF RID: 21167 RVA: 0x0024AE14 File Offset: 0x00249014
		public void OpenPolicyTab()
		{
			if (this.nationTabManager.activeTab != this.policiesTabController)
			{
				this.policyTabButtonObject.GetComponent<Button>().onClick.Invoke();
			}
		}

		// Token: 0x060052B0 RID: 21168 RVA: 0x0024AE43 File Offset: 0x00249043
		public void OpenRegionsTab()
		{
			if (this.nationTabManager.activeTab != this.regionsTabController)
			{
				this.regionsTabButtonObject.GetComponent<Button>().onClick.Invoke();
			}
		}

		// Token: 0x060052B1 RID: 21169 RVA: 0x0024AE72 File Offset: 0x00249072
		public void OpenRelationsTab()
		{
			if (this.nationTabManager.activeTab != this.relationsTabController)
			{
				this.relationsTabButtonObject.GetComponent<Button>().onClick.Invoke();
			}
		}

		// Token: 0x060052B2 RID: 21170 RVA: 0x0024AEA1 File Offset: 0x002490A1
		public void OpenArmiesTab()
		{
			if (this.armyTabButtonObject.activeSelf && this.nationTabManager.activeTab != this.armiesTabController)
			{
				this.armyTabButtonObject.GetComponent<Button>().onClick.Invoke();
			}
		}

		// Token: 0x060052B3 RID: 21171 RVA: 0x0024AEDD File Offset: 0x002490DD
		public void OpenCouncilorsTab()
		{
			if (this.councilorsTabButtonObject.activeSelf && this.nationTabManager.activeTab != this.councilorsTabController)
			{
				this.councilorsTabButtonObject.GetComponent<Button>().onClick.Invoke();
			}
		}

		// Token: 0x060052B4 RID: 21172 RVA: 0x0024AF1C File Offset: 0x0024911C
		public void Tutorial_TargetConstructOrbitalFightersPriority()
		{
			this.OpenPrioritiesTab();
			GameObject gameObject = null;
			bool flag = false;
			if (this.priorityList != null && this.priorityList.size > 0)
			{
				using (IEnumerator<object> enumerator = this.priorityList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (NationInfoController.<>o__478.<>p__0 == null)
						{
							NationInfoController.<>o__478.<>p__0 = CallSite<Func<CallSite, object, PriorityListItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PriorityListItemController), typeof(NationInfoController)));
						}
						PriorityListItemController priorityListItemController = NationInfoController.<>o__478.<>p__0.Target(NationInfoController.<>o__478.<>p__0, enumerator.Current);
						if (priorityListItemController.priority == PriorityType.Military_BuildSTOSquadron)
						{
							gameObject = priorityListItemController.gameObject;
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				return;
			}
			if (gameObject != null)
			{
				RectTransform rectTransform = this.constructExofighterPriorityHighlightDummy.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.SetParent(gameObject.transform, false);
					rectTransform.anchorMin = Vector2.zero;
					rectTransform.anchorMax = Vector2.one;
					rectTransform.offsetMin = Vector2.zero;
					rectTransform.offsetMax = Vector2.zero;
				}
			}
		}

		// Token: 0x040036A6 RID: 13990
		public Canvas nationPanelCanvas;

		// Token: 0x040036A7 RID: 13991
		[Header("Tutorials")]
		public UITutorialController NationInfoCanvasUITutorialController;

		// Token: 0x040036A8 RID: 13992
		public UITutorialController NationInfoCanvasPrioritiesUITutorialController;

		// Token: 0x040036A9 RID: 13993
		public UITutorialController ExofightersUITutorialController;

		// Token: 0x040036AA RID: 13994
		public GameObject constructExofighterPriorityHighlightDummy;

		// Token: 0x040036AB RID: 13995
		[Header("NationInfo")]
		public TMP_Text nationNameText;

		// Token: 0x040036AC RID: 13996
		public TMP_Text regionNameText;

		// Token: 0x040036AD RID: 13997
		public TMP_Text regionIconsText;

		// Token: 0x040036AE RID: 13998
		public TooltipTrigger regionNameTooltipTrigger;

		// Token: 0x040036AF RID: 13999
		public TooltipTrigger regionIconsTooltipTrigger;

		// Token: 0x040036B0 RID: 14000
		public Image flagImage;

		// Token: 0x040036B1 RID: 14001
		public GameObject executiveLeaderImageObject;

		// Token: 0x040036B2 RID: 14002
		public Image executiveLeaderImage;

		// Token: 0x040036B3 RID: 14003
		public Image executiveLeaderBackground;

		// Token: 0x040036B4 RID: 14004
		public TooltipTrigger executiveLeaderTooltipTrigger;

		// Token: 0x040036B5 RID: 14005
		public Button executiveLeaderRelationsButton;

		// Token: 0x040036B6 RID: 14006
		public Image executiveLeaderConsolidatedVisualization;

		// Token: 0x040036B7 RID: 14007
		public TMP_Text executiveLeaderCountdown;

		// Token: 0x040036B8 RID: 14008
		public GameObject nukeButtonPanel;

		// Token: 0x040036B9 RID: 14009
		public GameObject nukeChevrons;

		// Token: 0x040036BA RID: 14010
		public Button nukeButton;

		// Token: 0x040036BB RID: 14011
		public TMP_Text overviewHeaderText;

		// Token: 0x040036BC RID: 14012
		public TMP_Text militaryHeaderText;

		// Token: 0x040036BD RID: 14013
		public TMP_Text developmentHeaderText;

		// Token: 0x040036BE RID: 14014
		public TMP_Text peopleHeaderText;

		// Token: 0x040036BF RID: 14015
		public TMP_Text publicOpinionText;

		// Token: 0x040036C0 RID: 14016
		public GameObject specialRelationshipPanelObject;

		// Token: 0x040036C1 RID: 14017
		public Image specialRelationshipImage;

		// Token: 0x040036C2 RID: 14018
		public TMP_Text specialRelationshipName;

		// Token: 0x040036C3 RID: 14019
		public TooltipTrigger specialRelationshipTooltipTrigger;

		// Token: 0x040036C4 RID: 14020
		public Image headerBackground;

		// Token: 0x040036C5 RID: 14021
		public Color breakawayColor;

		// Token: 0x040036C6 RID: 14022
		public TooltipTrigger policyTooltipTrigger;

		// Token: 0x040036C7 RID: 14023
		public TMP_Text democracyText;

		// Token: 0x040036C8 RID: 14024
		public TooltipTrigger democracyTooltipTrigger;

		// Token: 0x040036C9 RID: 14025
		public TMP_Text stabilityText;

		// Token: 0x040036CA RID: 14026
		public TooltipTrigger stabilityTooltipTrigger;

		// Token: 0x040036CB RID: 14027
		public TMP_Text GDPValue;

		// Token: 0x040036CC RID: 14028
		public TooltipTrigger GDPTooltipTrigger;

		// Token: 0x040036CD RID: 14029
		public Image conflictStatusImage;

		// Token: 0x040036CE RID: 14030
		public TooltipTrigger conflictStatusTooltipTrigger;

		// Token: 0x040036CF RID: 14031
		public TMP_Text milTechText;

		// Token: 0x040036D0 RID: 14032
		public TooltipTrigger milTechTooltipTrigger;

		// Token: 0x040036D1 RID: 14033
		public TMP_Text numNukesValue;

		// Token: 0x040036D2 RID: 14034
		public TooltipTrigger nukesTooltipTrigger;

		// Token: 0x040036D3 RID: 14035
		public Image navalStatusIcon;

		// Token: 0x040036D4 RID: 14036
		public TMP_Text navalScoreText;

		// Token: 0x040036D5 RID: 14037
		public TooltipTrigger navalScoreTooltipTrigger;

		// Token: 0x040036D6 RID: 14038
		public TMP_Text numArmiesText;

		// Token: 0x040036D7 RID: 14039
		public GameObject numSTOsIconObject;

		// Token: 0x040036D8 RID: 14040
		public TooltipTrigger numArmiesTooltipTrigger;

		// Token: 0x040036D9 RID: 14041
		public TMP_Text numSTOsText;

		// Token: 0x040036DA RID: 14042
		public GameObject numSTOsObject;

		// Token: 0x040036DB RID: 14043
		public TooltipTrigger numSTOFightersTooltipTrigger;

		// Token: 0x040036DC RID: 14044
		public Image[] tinyControlPointImage;

		// Token: 0x040036DD RID: 14045
		public TooltipTrigger[] tinyControlPointTooltip;

		// Token: 0x040036DE RID: 14046
		public Button[] tinyControlPointButton;

		// Token: 0x040036DF RID: 14047
		public TMP_Text GDPPerCapitaValue;

		// Token: 0x040036E0 RID: 14048
		public TooltipTrigger GDPPerCapitaTooltipTrigger;

		// Token: 0x040036E1 RID: 14049
		public TMP_Text inequalityText;

		// Token: 0x040036E2 RID: 14050
		public TooltipTrigger inequalityTooltipTrigger;

		// Token: 0x040036E3 RID: 14051
		public TMP_Text populationValueText;

		// Token: 0x040036E4 RID: 14052
		public TooltipTrigger populationTooltipTrigger;

		// Token: 0x040036E5 RID: 14053
		public List<Image> nationIdeologyPortions;

		// Token: 0x040036E6 RID: 14054
		public TooltipTrigger publicOpinionTooltipTrigger;

		// Token: 0x040036E7 RID: 14055
		public TMP_Text cohesionText;

		// Token: 0x040036E8 RID: 14056
		public TooltipTrigger cohesionTooltipTrigger;

		// Token: 0x040036E9 RID: 14057
		public TMP_Text educationText;

		// Token: 0x040036EA RID: 14058
		public TooltipTrigger educationTooltipTrigger;

		// Token: 0x040036EB RID: 14059
		public Image sustainabilityIcon;

		// Token: 0x040036EC RID: 14060
		public TMP_Text sustainabilityText;

		// Token: 0x040036ED RID: 14061
		public TooltipTrigger sustainabilityTooltipTrigger;

		// Token: 0x040036EE RID: 14062
		public Image statsFlagImage;

		// Token: 0x040036EF RID: 14063
		public GameObject statsFederationImageObject;

		// Token: 0x040036F0 RID: 14064
		public Image statsFederationImage;

		// Token: 0x040036F1 RID: 14065
		public Image statsCouncilImage;

		// Token: 0x040036F2 RID: 14066
		public TooltipTrigger developmentSummaryTooltipTrigger;

		// Token: 0x040036F3 RID: 14067
		public GameObject federationValuesObject;

		// Token: 0x040036F4 RID: 14068
		public TMP_Text spaceFundingNationValue;

		// Token: 0x040036F5 RID: 14069
		public TMP_Text spaceFundingFederationValue;

		// Token: 0x040036F6 RID: 14070
		public TMP_Text spaceFundingCouncilValue;

		// Token: 0x040036F7 RID: 14071
		public TooltipTrigger spaceFundingTooltipTrigger;

		// Token: 0x040036F8 RID: 14072
		public TMP_Text investmentNationValue;

		// Token: 0x040036F9 RID: 14073
		public TMP_Text investmentCouncilValue;

		// Token: 0x040036FA RID: 14074
		public TooltipTrigger investmentNationTooltipTrigger;

		// Token: 0x040036FB RID: 14075
		public TMP_Text scienceNationValue;

		// Token: 0x040036FC RID: 14076
		public TMP_Text scienceCouncilValue;

		// Token: 0x040036FD RID: 14077
		public TooltipTrigger scienceNationTooltipTrigger;

		// Token: 0x040036FE RID: 14078
		public TMP_Text boostNationValue;

		// Token: 0x040036FF RID: 14079
		public TMP_Text boostFederationValue;

		// Token: 0x04003700 RID: 14080
		public TMP_Text boostCouncilValue;

		// Token: 0x04003701 RID: 14081
		public TooltipTrigger boostNationTooltipTrigger;

		// Token: 0x04003702 RID: 14082
		public TMP_Text missionControlNationValue;

		// Token: 0x04003703 RID: 14083
		public TMP_Text missionControlCouncilValue;

		// Token: 0x04003704 RID: 14084
		public TooltipTrigger missionControlNationTooltipTrigger;

		// Token: 0x04003705 RID: 14085
		public TabbedPaneManager nationTabManager;

		// Token: 0x04003706 RID: 14086
		public TabbedPaneController armiesTabController;

		// Token: 0x04003707 RID: 14087
		public TabbedPaneController policiesTabController;

		// Token: 0x04003708 RID: 14088
		public TabbedPaneController regionsTabController;

		// Token: 0x04003709 RID: 14089
		public TabbedPaneController councilorsTabController;

		// Token: 0x0400370A RID: 14090
		public TabbedPaneController prioritiesTabController;

		// Token: 0x0400370B RID: 14091
		public TabbedPaneController relationsTabController;

		// Token: 0x0400370C RID: 14092
		public GameObject prioritiesTabButtonObject;

		// Token: 0x0400370D RID: 14093
		public GameObject policyTabButtonObject;

		// Token: 0x0400370E RID: 14094
		public GameObject regionsTabButtonObject;

		// Token: 0x0400370F RID: 14095
		public GameObject relationsTabButtonObject;

		// Token: 0x04003710 RID: 14096
		public GameObject armyTabButtonObject;

		// Token: 0x04003711 RID: 14097
		public GameObject councilorsTabButtonObject;

		// Token: 0x04003712 RID: 14098
		public Button relationsButton1;

		// Token: 0x04003713 RID: 14099
		public TMP_Text manageRelationsButtonText;

		// Token: 0x04003714 RID: 14100
		public TooltipTrigger manageRelationsButtonTooltipTrigger;

		// Token: 0x04003715 RID: 14101
		[Header("Tabs")]
		public TMP_Text armiesTabText;

		// Token: 0x04003716 RID: 14102
		public TMP_Text prioritiesTabText;

		// Token: 0x04003717 RID: 14103
		public TMP_Text regionsTabText;

		// Token: 0x04003718 RID: 14104
		public TMP_Text policiesTabText;

		// Token: 0x04003719 RID: 14105
		public TMP_Text councilorsTabText;

		// Token: 0x0400371A RID: 14106
		public TMP_Text relationsTabText;

		// Token: 0x0400371B RID: 14107
		public ListManagerBase armyList;

		// Token: 0x0400371C RID: 14108
		public List<NationInfoRegionListItemModel> regionListItemModels = new List<NationInfoRegionListItemModel>();

		// Token: 0x0400371D RID: 14109
		public NationInfoRegionListAdapter nationInfoRegionListAdapter;

		// Token: 0x0400371E RID: 14110
		public ListManagerBase councilorList;

		// Token: 0x0400371F RID: 14111
		public ListManagerBase priorityList;

		// Token: 0x04003720 RID: 14112
		public ListManagerBase policyList;

		// Token: 0x04003721 RID: 14113
		public TMP_Text policyHeaderText;

		// Token: 0x04003722 RID: 14114
		public TooltipTrigger armyTabTooltipTrigger;

		// Token: 0x04003723 RID: 14115
		public TooltipTrigger prioritiesTabTooltipTrigger;

		// Token: 0x04003724 RID: 14116
		public TooltipTrigger policiesTabTooltipTrigger;

		// Token: 0x04003725 RID: 14117
		public TooltipTrigger regionTabTooltipTrigger;

		// Token: 0x04003726 RID: 14118
		public TooltipTrigger councilorTabTooltipTrigger;

		// Token: 0x04003727 RID: 14119
		public TooltipTrigger relationsTabTooltipTrigger;

		// Token: 0x04003728 RID: 14120
		public Image abductionsHeader;

		// Token: 0x04003729 RID: 14121
		public TMP_Text populationHeaderText;

		// Token: 0x0400372A RID: 14122
		public TMP_Text claimsHeaderText;

		// Token: 0x0400372B RID: 14123
		public TooltipTrigger occupationHeaderTooltipTrigger;

		// Token: 0x0400372C RID: 14124
		public TooltipTrigger boostHeaderTooltipTrigger;

		// Token: 0x0400372D RID: 14125
		public TooltipTrigger MCHeaderTooltipTrigger;

		// Token: 0x0400372E RID: 14126
		public TooltipTrigger claimsHeaderTooltipTrigger;

		// Token: 0x0400372F RID: 14127
		[Header("Priorities Tab")]
		public TabbedPaneController prioritiesTab;

		// Token: 0x04003730 RID: 14128
		public TMP_Dropdown priorityPresetDropdown;

		// Token: 0x04003731 RID: 14129
		private Dictionary<TIPriorityPresetTemplate, int> priorityPresetDictionary;

		// Token: 0x04003732 RID: 14130
		public TMP_Text priorityHeader1;

		// Token: 0x04003733 RID: 14131
		public TMP_Text priorityHeader2;

		// Token: 0x04003734 RID: 14132
		public TMP_Text directInvestButtonText;

		// Token: 0x04003735 RID: 14133
		public TMP_Text proportionColumnButtonText;

		// Token: 0x04003737 RID: 14135
		[Header("Relations Display")]
		public GameObject allyGrid;

		// Token: 0x04003738 RID: 14136
		public GameObject allyGridItem;

		// Token: 0x04003739 RID: 14137
		public GameObject rivalGrid;

		// Token: 0x0400373A RID: 14138
		public GameObject rivalGridItem;

		// Token: 0x0400373B RID: 14139
		public GameObject warGrid;

		// Token: 0x0400373C RID: 14140
		public GameObject warGridItem;

		// Token: 0x0400373D RID: 14141
		public TMP_Text alliesHeader;

		// Token: 0x0400373E RID: 14142
		public TMP_Text rivalsHeaders;

		// Token: 0x0400373F RID: 14143
		public TMP_Text warsHeader;

		// Token: 0x04003740 RID: 14144
		public ListManagerBase controlPointGrid;

		// Token: 0x04003741 RID: 14145
		public static readonly string[] weightStr = new string[4];

		// Token: 0x04003742 RID: 14146
		public static readonly Sprite[] weightSprite = new Sprite[4];

		// Token: 0x04003743 RID: 14147
		[Header("Earth Map Object Detail Panel")]
		public Canvas mapObjectDetailCanvas;

		// Token: 0x04003744 RID: 14148
		public Image mapObjectDetailIllustration;

		// Token: 0x04003745 RID: 14149
		public Image mapObjectFlag;

		// Token: 0x04003746 RID: 14150
		public TMP_Text mapObjectDetailHeader;

		// Token: 0x04003747 RID: 14151
		public GameObject statPanelObject;

		// Token: 0x04003748 RID: 14152
		public Image statPanelIcon;

		// Token: 0x04003749 RID: 14153
		public TMP_Text statPanelValueText;

		// Token: 0x0400374A RID: 14154
		public GameObject statPanelObject2;

		// Token: 0x0400374B RID: 14155
		public Image statPanelIcon2;

		// Token: 0x0400374C RID: 14156
		public TMP_Text statPanelValueText2;

		// Token: 0x0400374D RID: 14157
		public TMP_Text mapObjectDetailMainHeadline;

		// Token: 0x0400374E RID: 14158
		public TMP_Text mapObjectDetailLocation;

		// Token: 0x0400374F RID: 14159
		public TMP_Text mapObjectDetailExplainerText;

		// Token: 0x04003750 RID: 14160
		public GameObject mapObjectButtonPanelObject;

		// Token: 0x04003751 RID: 14161
		public GameObject mapObjectButtonPanelObject2;

		// Token: 0x04003752 RID: 14162
		public Button mapObjectButtonPanelButton;

		// Token: 0x04003753 RID: 14163
		public Button mapObjectButtonPanelButton2;

		// Token: 0x04003754 RID: 14164
		public TMP_Text mapObjectButtonText;

		// Token: 0x04003755 RID: 14165
		public TMP_Text mapObjectButtonText2;

		// Token: 0x04003756 RID: 14166
		private TIRegionEntityState displayedRegionLocationState;

		// Token: 0x04003757 RID: 14167
		[HideInInspector]
		public bool targetingOwnedCPs;

		// Token: 0x04003758 RID: 14168
		[HideInInspector]
		public bool targetingNeutralCP;

		// Token: 0x04003759 RID: 14169
		[HideInInspector]
		public TIMissionTemplate currentMission;

		// Token: 0x0400375A RID: 14170
		[HideInInspector]
		public TICouncilorState currentMissionCouncilor;

		// Token: 0x0400375B RID: 14171
		private float timeToNextUpdate_s;

		// Token: 0x0400375C RID: 14172
		private const float updateDelta_s = 15f;

		// Token: 0x0400375D RID: 14173
		[SerializeField]
		private bool wasActive;

		// Token: 0x0400375E RID: 14174
		private TIRegionState region;

		// Token: 0x04003760 RID: 14176
		private bool nationDataDirty;

		// Token: 0x04003761 RID: 14177
		private bool councilorListDataDirty;

		// Token: 0x04003762 RID: 14178
		private static bool playDropdownAudio = true;

		// Token: 0x04003763 RID: 14179
		[Header("Direct Invest UI")]
		public GameObject directInvestPanel;

		// Token: 0x04003764 RID: 14180
		public ListManagerBase directInvestListManager;

		// Token: 0x04003765 RID: 14181
		public TMP_Text directInvestPanelHeader;

		// Token: 0x04003766 RID: 14182
		public TMP_Text DIHeader_PriorityName;

		// Token: 0x04003767 RID: 14183
		public TMP_Text DIHeader_PerIPCost;

		// Token: 0x04003768 RID: 14184
		public TMP_Text DIHeader_CurrentSetting;

		// Token: 0x04003769 RID: 14185
		public TMP_Text DIHeader_PlannedCost;

		// Token: 0x0400376A RID: 14186
		public TMP_Text DIHeader_CompletionOutcome;

		// Token: 0x0400376B RID: 14187
		public Button confirmButton;

		// Token: 0x0400376C RID: 14188
		public TMP_Text directInvestConfirmButtonText;

		// Token: 0x0400376D RID: 14189
		public TMP_Text directInvestResetButtonText;

		// Token: 0x0400376E RID: 14190
		public TMP_Text directInvestCancelButtonText;

		// Token: 0x0400376F RID: 14191
		public TMP_Text directInvestTotalSpendText;

		// Token: 0x04003770 RID: 14192
		public TMP_Text directInvestTotalSpendValue;

		// Token: 0x04003771 RID: 14193
		public TMP_Text directInvestAnnualText;

		// Token: 0x04003772 RID: 14194
		public TMP_Text directInvestAnnualIPs;

		// Token: 0x04003773 RID: 14195
		public TMP_Text freeInfluenceHeadsUp;

		// Token: 0x04003774 RID: 14196
		public Dictionary<PriorityType, float> plannedDirectInvestments;

		// Token: 0x04003775 RID: 14197
		[Header("Priority Preset Builder")]
		public UITutorialController DesignPresetUITutorialController;

		// Token: 0x04003776 RID: 14198
		public GameObject designPresetPanel;

		// Token: 0x04003777 RID: 14199
		public TMP_Text designPresetPanelButtonText;

		// Token: 0x04003778 RID: 14200
		private TIPriorityPresetTemplate proposedPriorityPreset;

		// Token: 0x04003779 RID: 14201
		private TIPriorityPresetTemplate duplicatedPreset;

		// Token: 0x0400377A RID: 14202
		public TMP_Text presetBuilderHeaderText;

		// Token: 0x0400377B RID: 14203
		public TMP_InputField inputPresetName;

		// Token: 0x0400377C RID: 14204
		public TMP_Text inputPresetDefaultText;

		// Token: 0x0400377D RID: 14205
		public TMP_Text designPriorityPresetDropdownLabel;

		// Token: 0x0400377E RID: 14206
		public TMP_Text savePresetButtonText;

		// Token: 0x0400377F RID: 14207
		public TMP_Text resetPresetButtonText;

		// Token: 0x04003780 RID: 14208
		public TMP_Text setAsDefaultPresetButtonText;

		// Token: 0x04003781 RID: 14209
		public TMP_Text applyPresetGloballyButtonText;

		// Token: 0x04003782 RID: 14210
		public Button savePresetButton;

		// Token: 0x04003783 RID: 14211
		public Button resetPresetButton;

		// Token: 0x04003784 RID: 14212
		public Button deletePresetButton;

		// Token: 0x04003785 RID: 14213
		public Button setAsDefaultPresetButton;

		// Token: 0x04003786 RID: 14214
		public Button applyGloballyButton;

		// Token: 0x04003787 RID: 14215
		public TMP_Dropdown designPriorityPresetDropdown;

		// Token: 0x04003788 RID: 14216
		private Dictionary<int, TIPriorityPresetTemplate> designPriorityPresetDictionary = new Dictionary<int, TIPriorityPresetTemplate>();

		// Token: 0x04003789 RID: 14217
		public ListManagerBase designPriorityPresetListManager;

		// Token: 0x0400378A RID: 14218
		public TooltipTrigger applyGloballyTip;

		// Token: 0x0400378B RID: 14219
		public TooltipTrigger setAsDefaultTip;

		// Token: 0x0400378C RID: 14220
		public TooltipTrigger deleteTooltip;

		// Token: 0x0400378D RID: 14221
		private string deleteTooltipText;

		// Token: 0x0400378E RID: 14222
		public Button disableControlPointsButton;

		// Token: 0x0400378F RID: 14223
		public TMP_Text disableControlPointsButtonText;

		// Token: 0x04003790 RID: 14224
		public GameObject confirmDisableControlPointPanel;

		// Token: 0x04003791 RID: 14225
		public TMP_Text confirmDisableControlPointHeaderText;

		// Token: 0x04003792 RID: 14226
		public TMP_Text confirmDisableControlPointBodyText;

		// Token: 0x04003793 RID: 14227
		public TMP_Text confirmDisableControlConfirmButtonText;

		// Token: 0x04003794 RID: 14228
		public TMP_Text confirmDisableControlCancelButtonText;

		// Token: 0x04003795 RID: 14229
		public TooltipTrigger disbaleControlPointsTip;

		// Token: 0x04003796 RID: 14230
		public TMP_Text autoAbandonToggleText;

		// Token: 0x04003797 RID: 14231
		public Toggle autoAbandonToggle;

		// Token: 0x04003798 RID: 14232
		[Header("Relations Manager")]
		public GameObject nationRelationsManagerPanel;

		// Token: 0x04003799 RID: 14233
		private const int maxFactions = 8;

		// Token: 0x0400379A RID: 14234
		public Dictionary<TINationState, RelationChange> proposedRelationsChanges;

		// Token: 0x0400379B RID: 14235
		public TabbedPaneManager tabManager;

		// Token: 0x0400379C RID: 14236
		public TMP_Text nationRelationsHeader;

		// Token: 0x0400379D RID: 14237
		public TMP_Text costProposalText;

		// Token: 0x0400379E RID: 14238
		public TMP_Text massChangesText;

		// Token: 0x0400379F RID: 14239
		public TMP_Text unalignedNationsTabText;

		// Token: 0x040037A0 RID: 14240
		public Button acceptChangesButton;

		// Token: 0x040037A1 RID: 14241
		public TMP_Text relations_acceptButtonText;

		// Token: 0x040037A2 RID: 14242
		public TMP_Text relations_resetButtonText;

		// Token: 0x040037A3 RID: 14243
		public TMP_Text relations_closeButtonText;

		// Token: 0x040037A4 RID: 14244
		public TMP_Text improveRelationsColumnHeaderText;

		// Token: 0x040037A5 RID: 14245
		public TMP_Text allyText;

		// Token: 0x040037A6 RID: 14246
		public TMP_Text normalText;

		// Token: 0x040037A7 RID: 14247
		public TMP_Text rivalText;

		// Token: 0x040037A8 RID: 14248
		public TMP_Text warText;

		// Token: 0x040037A9 RID: 14249
		public Dictionary<int, TIFactionState> relationsTabIndices;

		// Token: 0x040037AA RID: 14250
		public List<GameObject> relationsTabButtonObjects;

		// Token: 0x040037AB RID: 14251
		public List<Image> relationsTabImages;

		// Token: 0x040037AC RID: 14252
		public List<NationRelationsPaneController> factionRelationsPaneControllers;

		// Token: 0x040037AD RID: 14253
		public Toggle AllyAllToggle;

		// Token: 0x040037AE RID: 14254
		public Toggle AllyToNormalAllToggle;

		// Token: 0x040037AF RID: 14255
		public Toggle RivalToNormalAllToggle;

		// Token: 0x040037B0 RID: 14256
		public Toggle RivalAllToggle;

		// Token: 0x040037B1 RID: 14257
		public Image AllyAllCheckmark;

		// Token: 0x040037B2 RID: 14258
		public Image AllyToNormalAllCheckmark;

		// Token: 0x040037B3 RID: 14259
		public Image RivalToNormalAllCheckmark;

		// Token: 0x040037B4 RID: 14260
		public Image RivalAllCheckmark;

		// Token: 0x040037B5 RID: 14261
		public TIResourcesCost relationshipChangesCost;

		// Token: 0x040037B6 RID: 14262
		private bool ignoreToggles;

		// Token: 0x040037B7 RID: 14263
		public GameObject nuclearWeaponsPanel;

		// Token: 0x040037B8 RID: 14264
		public TMP_Text nuclearWeaponsPanelHeader;

		// Token: 0x040037B9 RID: 14265
		public TMP_Text nuclearWeaponsPanelText;

		// Token: 0x040037BA RID: 14266
		public TMP_Text nuclearWeaponsTargetText;

		// Token: 0x040037BB RID: 14267
		public TMP_Text nuclearConfirmButtonText;

		// Token: 0x040037BC RID: 14268
		public TMP_Text nuclearCancelButtonText;

		// Token: 0x040037BD RID: 14269
		public TIRegionState currentNuclearTarget;

		// Token: 0x040037BE RID: 14270
		public Button nuclearConfirmButton;

		// Token: 0x040037BF RID: 14271
		public Image nationFlag;

		// Token: 0x040037C0 RID: 14272
		public Image nationFlag2;

		// Token: 0x040037C1 RID: 14273
		private TINationState nukingNation;

		// Token: 0x040037C2 RID: 14274
		private TIOperationTargeting_Region nuclearTargeting;

		// Token: 0x020010E2 RID: 4322
		public enum WhatIsGood
		{
			// Token: 0x04006586 RID: 25990
			upIsGood,
			// Token: 0x04006587 RID: 25991
			downIsGood,
			// Token: 0x04006588 RID: 25992
			middleIsGood,
			// Token: 0x04006589 RID: 25993
			upOrMiddleIsGood
		}

		// Token: 0x020010E3 RID: 4323
		public enum TrackedValue
		{
			// Token: 0x0400658B RID: 25995
			GDP,
			// Token: 0x0400658C RID: 25996
			Inequality,
			// Token: 0x0400658D RID: 25997
			Cohesion,
			// Token: 0x0400658E RID: 25998
			Unrest,
			// Token: 0x0400658F RID: 25999
			Education,
			// Token: 0x04006590 RID: 26000
			Government,
			// Token: 0x04006591 RID: 26001
			GHGs
		}
	}
}
