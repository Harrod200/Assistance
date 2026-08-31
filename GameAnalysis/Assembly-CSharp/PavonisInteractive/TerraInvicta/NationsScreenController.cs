using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200089A RID: 2202
	public class NationsScreenController : CanvasControllerBase, IInfoScreen, ICanvas
	{
		// Token: 0x06005310 RID: 21264 RVA: 0x0024DBA0 File Offset: 0x0024BDA0
		public override void Initialize()
		{
			base.Initialize();
			this.nationItemDictionary = new Dictionary<NationsScreenNationListItemController, TIGameState>();
			this.nationOpenedStatus = new Dictionary<NationsScreenNationListItemController, bool>();
			this.filterFaction = null;
			this.CPBreakdown.SetActive(false);
			this.fullScreenPanel.SetActive(true);
			this.NationsPanelHeader.SetText(Loc.T("UI.Nations.Header", new object[] { base.activePlayer.adjective }));
			this.showAllNationsText.SetText(Loc.T("UI.Nations.ShowAllNations"));
			this.CPBreakdownHeader.SetText(Loc.T("UI.Nations.CPMaintHeader"));
			this.OpenCPBreakdownButtonText.SetText(Loc.T("UI.Nations.CPMaintHeader"));
			this.CloseCPBreakdownButtonText.SetText(Loc.T("UI.Nations.CPMaintClose"));
			this.controlPointColumnText.SetText(Loc.T("UI.Nations.CPMaintButton"));
			this.nameColumnText.SetText(Loc.T("UI.Nations.Name"));
			this.playerFactionIcon.sprite = base.activePlayer.factionIcon64UI;
			this.reverseSort = false;
		}

		// Token: 0x06005311 RID: 21265 RVA: 0x0024DCAC File Offset: 0x0024BEAC
		private void InitializeMainList()
		{
			Log.Time("<color=#00cc00>LoadTime:</color> Initialize NationsScreen", delegate
			{
				TINationState[] array = GameStateManager.AllNations();
				this.BuildMainList(array.ToList<TINationState>(), true);
				this.initialized = true;
			}, true, true);
		}

		// Token: 0x06005312 RID: 21266 RVA: 0x0024DCC8 File Offset: 0x0024BEC8
		public void BuildMainList(List<TINationState> allNations, bool initialization)
		{
			this.allNationsList.Clear();
			foreach (TINationState tinationState in allNations)
			{
				this.allNationsList.Add(tinationState);
			}
			if (initialization)
			{
				this.SortNationDictionary(0, true);
				return;
			}
			this.SortNationDictionary((int)this.currentNationSort, true);
		}

		// Token: 0x06005313 RID: 21267 RVA: 0x0024DD40 File Offset: 0x0024BF40
		public void SetNationListModelData()
		{
			this.suppressDropDownAudio = true;
			this.nationModels.Clear();
			for (int i = 0; i < this.allNationsList.Count; i++)
			{
				if (this.allNationsList[i].ref_nation.extant)
				{
					NationScreenNationListItemModel nationScreenNationListItemModel = new NationScreenNationListItemModel();
					NationsScreenNationListItem_Data nationsScreenNationListItem_Data = new NationsScreenNationListItem_Data();
					nationsScreenNationListItem_Data.nationLine = true;
					nationsScreenNationListItem_Data.controller = this;
					nationsScreenNationListItem_Data.showInList = this.allNationsList[i].ref_nation != null && (this.filterFaction == null || this.allNationsList[i].ref_nation.controlPointOwnersByPoint.Contains(this.filterFaction));
					nationsScreenNationListItem_Data.SetNationData(this.allNationsList[i].ref_nation);
					nationScreenNationListItemModel.NationScreenNationListItemData = nationsScreenNationListItem_Data;
					this.nationModels.Add(nationScreenNationListItemModel);
				}
			}
			this.nationListAdapter.SetItems(this.nationModels);
			this.suppressDropDownAudio = false;
		}

		// Token: 0x06005314 RID: 21268 RVA: 0x0024DE48 File Offset: 0x0024C048
		public override void Show()
		{
			base.Show();
			GameControl.eventManager.AddListener<NationShedsControlPoint>(new EventManager.EventDelegate<NationShedsControlPoint>(this.OnNationShedsControlPoint), null, null, true, false);
			GameControl.eventManager.AddListener<NationGrowsNewControlPoint>(new EventManager.EventDelegate<NationGrowsNewControlPoint>(this.OnNationGrowsControlPoint), null, null, true, false);
			if (!this.initialized)
			{
				Log.Time("<color=#00cc00>LoadTime:</color> Initialize Nations list", delegate
				{
					this.InitializeMainList();
				}, true, true);
			}
			this.primaryCanvas.enabled = true;
			this.fullScreenPanel.SetActive(true);
			this.primaryCanvas.gameObject.GetComponent<Image>().enabled = true;
			this.primaryList.gameObject.SetActive(true);
			if (!this.firstSort)
			{
				this.currentNationSort = SortNationDataBy.Armies;
				this.SortNationDictionary(0, true);
				if (base.activePlayer.controlPoints.Count > 0)
				{
					this.filterFactionToggle.SetIsOnWithoutNotify(false);
					this.ToggleFilterFaction(false);
				}
				this.firstSort = true;
			}
			base.StartCoroutine(this.UpdateFullListGradual());
			this.SetNationListModelData();
			this.NationsScreenUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_NationsScreenCanvas_Nations, false, true);
			this.canViewSTOFighters = TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron);
			this.stoFightersPanelButton.interactable = this.canViewSTOFighters;
			this.stoFightersPanelImage.enabled = this.canViewSTOFighters;
			if (this.CPBreakdown.activeInHierarchy)
			{
				this.UpdateCPBreakdown();
			}
		}

		// Token: 0x06005315 RID: 21269 RVA: 0x0024DF9C File Offset: 0x0024C19C
		public override void Hide()
		{
			GameControl.eventManager.RemoveListener<NationShedsControlPoint>(new EventManager.EventDelegate<NationShedsControlPoint>(this.OnNationShedsControlPoint), null);
			GameControl.eventManager.RemoveListener<NationGrowsNewControlPoint>(new EventManager.EventDelegate<NationGrowsNewControlPoint>(this.OnNationGrowsControlPoint), null);
			this.primaryCanvas.enabled = false;
			this.NationsScreenUITutorialController.HideTutorial();
			base.Hide();
		}

		// Token: 0x06005316 RID: 21270 RVA: 0x0024DFF4 File Offset: 0x0024C1F4
		public override void Refresh()
		{
			if (!base.Paused)
			{
				if (this.timeToNextUpdate_s <= 0f)
				{
					base.StartCoroutine(this.UpdateFullListGradual());
					this.timeToNextUpdate_s = 5f;
					return;
				}
				this.timeToNextUpdate_s -= Time.unscaledDeltaTime;
			}
		}

		// Token: 0x06005317 RID: 21271 RVA: 0x0024E041 File Offset: 0x0024C241
		public void OnNationGrowsControlPoint(NationGrowsNewControlPoint e)
		{
			this.BuildMainList(GameStateManager.AllNations().ToList<TINationState>(), false);
			if (this.Visible())
			{
				this.UpdateFullList();
			}
		}

		// Token: 0x06005318 RID: 21272 RVA: 0x0024E062 File Offset: 0x0024C262
		public void OnNationShedsControlPoint(NationShedsControlPoint e)
		{
			this.BuildMainList(GameStateManager.AllNations().ToList<TINationState>(), false);
			if (this.Visible())
			{
				this.UpdateFullList();
			}
		}

		// Token: 0x06005319 RID: 21273 RVA: 0x0024E083 File Offset: 0x0024C283
		public override bool Visible()
		{
			return base.Visible() && base.canvasManager.IsShowingInfoScreen<NationsScreenController>();
		}

		// Token: 0x0600531A RID: 21274 RVA: 0x0024E09A File Offset: 0x0024C29A
		public void CloseInfoScreen(bool toggle = false)
		{
			if (this.primaryCanvas != null)
			{
				this.primaryCanvas.enabled = false;
				base.canvasManager.HideInfoScreen<NationsScreenController>(toggle);
			}
		}

		// Token: 0x0600531B RID: 21275 RVA: 0x0024E0C2 File Offset: 0x0024C2C2
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.primaryCanvas.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x0600531C RID: 21276 RVA: 0x0024E0FD File Offset: 0x0024C2FD
		public void OnExitButtonSelected()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.NationsScreenUITutorialController.HideTutorial();
			this.CloseInfoScreen(false);
		}

		// Token: 0x0600531D RID: 21277 RVA: 0x0024E11D File Offset: 0x0024C31D
		public void ToggleFilterFaction(bool playAudio = true)
		{
			if (playAudio)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			}
			this.filterFaction = ((this.filterFaction == null) ? base.activePlayer : null);
			this.BuildMainList(GameStateManager.AllNations().ToList<TINationState>(), false);
		}

		// Token: 0x0600531E RID: 21278 RVA: 0x0024E15C File Offset: 0x0024C35C
		public void SortNationDictionary(int sortBy)
		{
			this.SortNationDictionary(sortBy, false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
		}

		// Token: 0x0600531F RID: 21279 RVA: 0x0024E174 File Offset: 0x0024C374
		public void SortNationDictionary(int sortBy, bool forceSameOrder)
		{
			SortNationDataBy sortNationDataBy = this.currentNationSort;
			this.currentNationSort = (SortNationDataBy)sortBy;
			if (this.currentNationSort == sortNationDataBy && !forceSameOrder)
			{
				this.reverseSort = !this.reverseSort;
			}
			else if (!forceSameOrder)
			{
				this.reverseSort = false;
			}
			switch (this.currentNationSort)
			{
			case SortNationDataBy.Alfa:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, string>((TIGameState o) => o.ref_nation.displayName).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, string>((TIGameState o) => o.ref_nation.displayName).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.MyControlPoints:
				if (!this.reverseSort)
				{
					this.allNationsList = (from o in this.allNationsList
						orderby o.ref_nation.CountFactionControlPoints(GameControl.control.activePlayer, true, false, true) descending, o.ref_nation.GDP descending
						select o).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = (from o in this.allNationsList
						orderby o.ref_nation.CountFactionControlPoints(GameControl.control.activePlayer, true, false, true), o.ref_nation.GDP
						select o).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.HighestPopularity:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.GetMostPopularFactionValue(false)).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.GetMostPopularFactionValue(false)).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.MyPopularity:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.GetPublicOpinionOfFaction(GameControl.control.activePlayer.ideology)).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.GetPublicOpinionOfFaction(GameControl.control.activePlayer.ideology)).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Population:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.population_Millions).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.population_Millions).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.GDP:
			case SortNationDataBy.Difficulty:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, double>((TIGameState o) => o.ref_nation.GDP).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, double>((TIGameState o) => o.ref_nation.GDP).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.PerCapitaGDP:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.perCapitaGDP).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.perCapitaGDP).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Government:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.democracy).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.democracy).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Education:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.education).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.education).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Inequality:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.inequality).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.inequality).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Cohesion:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.cohesion).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.cohesion).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Unrest:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.unrest).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.unrest).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Funding:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.spaceFunding_month).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.spaceFunding_month).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Research:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.research_month).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.research_month).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Boost:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.boostIncome_month_dekatons).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.boostIncome_month_dekatons).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.MissionControl:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, int>((TIGameState o) => o.ref_nation.missionControl).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, int>((TIGameState o) => o.ref_nation.missionControl).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Wars:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, int>((TIGameState o) => o.ref_nation.wars.Count).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, int>((TIGameState o) => o.ref_nation.wars.Count).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Miltech:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, bool>((TIGameState x) => x.ref_nation.military).ThenByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.militaryTechLevel).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, bool>((TIGameState x) => x.ref_nation.military).ThenByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.militaryTechLevel).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Armies:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, int>((TIGameState o) => o.ref_nation.armies.Count).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, int>((TIGameState o) => o.ref_nation.armies.Count).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.InvestmentPoints:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.BaseInvestmentPoints_month()).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.BaseInvestmentPoints_month()).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Sustainability:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, float>((TIGameState o) => o.ref_nation.sustainability).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, float>((TIGameState o) => o.ref_nation.sustainability).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.NuclearWeapons:
				if (!this.reverseSort)
				{
					this.allNationsList = (from o in this.allNationsList
						orderby o.ref_nation.nuclearProgram descending, o.ref_nation.numNuclearWeapons descending
						select o).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = (from o in this.allNationsList
						orderby o.ref_nation.nuclearProgram, o.ref_nation.numNuclearWeapons
						select o).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.Navies:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, int>((TIGameState o) => o.ref_nation.numNavies).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, int>((TIGameState o) => o.ref_nation.numNavies).ToList<TIGameState>();
				}
				break;
			case SortNationDataBy.STOFighters:
				if (!this.reverseSort)
				{
					this.allNationsList = this.allNationsList.OrderByDescending<TIGameState, int>((TIGameState o) => o.ref_nation.numSTOFighters).ToList<TIGameState>();
				}
				else
				{
					this.allNationsList = this.allNationsList.OrderBy<TIGameState, int>((TIGameState o) => o.ref_nation.numSTOFighters).ToList<TIGameState>();
				}
				break;
			}
			this.SetNationListModelData();
		}

		// Token: 0x06005320 RID: 21280 RVA: 0x0024EEA0 File Offset: 0x0024D0A0
		public List<NationsScreenNationListItemController> GetControlPointLinesForNation(TINationState nation)
		{
			List<NationsScreenNationListItemController> list = new List<NationsScreenNationListItemController>();
			int num = 0;
			foreach (NationsScreenNationListItemController nationsScreenNationListItemController in this.nationItemDictionary.Keys)
			{
				if (!nationsScreenNationListItemController.nationLine && nation.controlPoints.Contains(nationsScreenNationListItemController.controlPoint))
				{
					list.Add(nationsScreenNationListItemController);
					num++;
					if (num >= 6)
					{
						break;
					}
				}
			}
			return list;
		}

		// Token: 0x06005321 RID: 21281 RVA: 0x0024EF28 File Offset: 0x0024D128
		public NationsScreenNationListItemController GetNationLineForControlPoint(TIControlPoint controlPoint)
		{
			foreach (NationsScreenNationListItemController nationsScreenNationListItemController in this.nationItemDictionary.Keys)
			{
				if (nationsScreenNationListItemController.nationLine && nationsScreenNationListItemController.nation == controlPoint.nation)
				{
					return nationsScreenNationListItemController;
				}
			}
			return null;
		}

		// Token: 0x06005322 RID: 21282 RVA: 0x0024EF9C File Offset: 0x0024D19C
		public void UpdateFullList()
		{
			foreach (NationsScreenNationListItemController nationsScreenNationListItemController in this.nationItemDictionary.Keys)
			{
				TINationState tinationState = this.nationItemDictionary[nationsScreenNationListItemController] as TINationState;
				if (this.nationOpenedStatus[nationsScreenNationListItemController] || (tinationState != null && tinationState.extant && (this.filterFaction == null || tinationState.controlPointOwnersByPoint.Contains(this.filterFaction))))
				{
					nationsScreenNationListItemController.UpdateListItem();
					nationsScreenNationListItemController.gameObject.SetActive(true);
				}
				else
				{
					nationsScreenNationListItemController.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06005323 RID: 21283 RVA: 0x0024F064 File Offset: 0x0024D264
		public IEnumerator UpdateFullListGradual()
		{
			yield return null;
			this.filterFactionToggle.interactable = false;
			foreach (NationsScreenNationListItemController nationsScreenNationListItemController in this.nationItemDictionary.Keys)
			{
				TINationState tinationState = this.nationItemDictionary[nationsScreenNationListItemController] as TINationState;
				if (this.nationOpenedStatus[nationsScreenNationListItemController] || (tinationState != null && tinationState.extant && (this.filterFaction == null || tinationState.controlPointOwnersByPoint.Contains(this.filterFaction))))
				{
					nationsScreenNationListItemController.canvasGroup.alpha = 1f;
					nationsScreenNationListItemController.layoutElement.ignoreLayout = false;
				}
				else
				{
					nationsScreenNationListItemController.canvasGroup.alpha = 0f;
					nationsScreenNationListItemController.layoutElement.ignoreLayout = true;
				}
			}
			int i = 0;
			foreach (NationsScreenNationListItemController nationsScreenNationListItemController2 in this.nationItemDictionary.Keys)
			{
				TINationState tinationState2 = this.nationItemDictionary[nationsScreenNationListItemController2] as TINationState;
				if (this.nationOpenedStatus[nationsScreenNationListItemController2] || (tinationState2 != null && tinationState2.extant && (this.filterFaction == null || tinationState2.controlPointOwnersByPoint.Contains(this.filterFaction))))
				{
					nationsScreenNationListItemController2.UpdateListItem();
					if (!nationsScreenNationListItemController2.gameObject.activeSelf)
					{
						nationsScreenNationListItemController2.gameObject.SetActive(true);
					}
					int num = i;
					i = num + 1;
				}
				else if (nationsScreenNationListItemController2.gameObject.activeSelf)
				{
					nationsScreenNationListItemController2.gameObject.SetActive(false);
					int num = i;
					i = num + 1;
				}
				if (i == 10)
				{
					i = 0;
					yield return null;
				}
			}
			Dictionary<NationsScreenNationListItemController, TIGameState>.KeyCollection.Enumerator enumerator2 = default(Dictionary<NationsScreenNationListItemController, TIGameState>.KeyCollection.Enumerator);
			yield return null;
			this.filterFactionToggle.interactable = true;
			yield break;
			yield break;
		}

		// Token: 0x06005324 RID: 21284 RVA: 0x0024F073 File Offset: 0x0024D273
		public void OnOpenCPBreakdown()
		{
			this.CPBreakdown.SetActive(true);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_GenericConfirm", false, false);
			this.UpdateCPBreakdown();
		}

		// Token: 0x06005325 RID: 21285 RVA: 0x0024F093 File Offset: 0x0024D293
		public void OnCloseCPBreakdown()
		{
			this.CPBreakdown.SetActive(false);
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseSmall", false, false);
		}

		// Token: 0x06005326 RID: 21286 RVA: 0x0024F0B0 File Offset: 0x0024D2B0
		public void UpdateCPBreakdown()
		{
			StringBuilder stringBuilder = new StringBuilder();
			float annualControlPointMaintenanceCost = base.activePlayer.GetAnnualControlPointMaintenanceCost();
			stringBuilder.AppendLine(Loc.T("UI.Nations.CPMaint1", new object[]
			{
				TemplateManager.global.influenceInlineSpritePath,
				(annualControlPointMaintenanceCost / 12f).ToString("N1")
			}));
			float controlPointMaintenanceFreebieCap = base.activePlayer.GetControlPointMaintenanceFreebieCap();
			int num = (int)TIEffectsState.SumEffectsModifiers(Context.ControlPointMaintenance, base.activePlayer, controlPointMaintenanceFreebieCap, null);
			stringBuilder.AppendLine();
			stringBuilder.AppendLine(TIUtilities.GreenLine(Loc.T("UI.Nations.CPMaint2", new object[]
			{
				controlPointMaintenanceFreebieCap.ToString("N0"),
				base.activePlayer.inlineControlPointCapIcon
			})));
			stringBuilder.AppendLine(Loc.T("UI.Nations.CPMaint3", new object[] { TemplateManager.global.influenceInlineSpritePath }));
			stringBuilder.AppendLine(Loc.T("UI.Nations.CPMaint4", new object[]
			{
				TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies.ToString("N0"),
				base.activePlayer.inlineControlPointCapIcon
			}));
			StringBuilder stringBuilder2 = stringBuilder;
			string text = "UI.Nations.CPMaint10";
			object[] array = new object[2];
			array[0] = base.activePlayer.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Administration, true, true, true, false, false, false)).ToString("N0");
			array[1] = base.activePlayer.inlineControlPointCapIcon;
			stringBuilder2.AppendLine(Loc.T(text, array));
			StringBuilder stringBuilder3 = stringBuilder;
			string text2 = "UI.Nations.CPMaint11";
			object[] array2 = new object[2];
			array2[0] = base.activePlayer.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Command, true, true, true, false, false, false)).ToString("N0");
			array2[1] = base.activePlayer.inlineControlPointCapIcon;
			stringBuilder3.AppendLine(Loc.T(text2, array2));
			StringBuilder stringBuilder4 = stringBuilder;
			string text3 = "UI.Nations.CPMaint12";
			object[] array3 = new object[2];
			array3[0] = base.activePlayer.activeCouncilors.Sum<TICouncilorState>((TICouncilorState x) => x.GetAttribute(CouncilorAttribute.Persuasion, true, true, true, false, false, false)).ToString("N0");
			array3[1] = base.activePlayer.inlineControlPointCapIcon;
			stringBuilder4.AppendLine(Loc.T(text3, array3));
			StringBuilder stringBuilder5 = stringBuilder;
			string text4 = "UI.Nations.CPMaint13";
			object[] array4 = new object[2];
			array4[0] = base.activePlayer.habs.Sum<TIHabState>((TIHabState x) => x.controlPointCapacityValue).ToString("N0");
			array4[1] = base.activePlayer.inlineControlPointCapIcon;
			stringBuilder5.AppendLine(Loc.T(text4, array4));
			stringBuilder.AppendLine(Loc.T("UI.Nations.CPMaint5", new object[]
			{
				(-num).ToString("N0"),
				base.activePlayer.inlineControlPointCapIcon
			}));
			stringBuilder.AppendLine();
			string text5 = Loc.T("UI.Nations.CPMaint6", new object[]
			{
				base.activePlayer.GetBaselineControlPointMaintenanceCost(false).ToString("N0"),
				base.activePlayer.inlineControlPointCapIcon
			});
			if (annualControlPointMaintenanceCost > 0f)
			{
				stringBuilder.AppendLine(TIUtilities.RedLine(text5));
			}
			else
			{
				stringBuilder.AppendLine(TIUtilities.GreenLine(text5));
			}
			Dictionary<float, Dictionary<TINationState, int>> dictionary = new Dictionary<float, Dictionary<TINationState, int>>();
			List<float> list = new List<float>();
			foreach (TIControlPoint ticontrolPoint in base.activePlayer.controlPoints)
			{
				if (!ticontrolPoint.benefitsDisabled)
				{
					if (!dictionary.ContainsKey(ticontrolPoint.nation.ControlPointMaintenanceCost))
					{
						dictionary.Add(ticontrolPoint.nation.ControlPointMaintenanceCost, new Dictionary<TINationState, int>());
						list.Add(ticontrolPoint.nation.ControlPointMaintenanceCost);
					}
					if (!dictionary[ticontrolPoint.nation.ControlPointMaintenanceCost].ContainsKey(ticontrolPoint.nation))
					{
						dictionary[ticontrolPoint.nation.ControlPointMaintenanceCost].Add(ticontrolPoint.nation, 0);
					}
					Dictionary<TINationState, int> dictionary2 = dictionary[ticontrolPoint.nation.ControlPointMaintenanceCost];
					TINationState tinationState = ticontrolPoint.nation;
					dictionary2[tinationState]++;
				}
				else
				{
					if (!dictionary.ContainsKey(0f))
					{
						dictionary.Add(0f, new Dictionary<TINationState, int>());
						list.Add(0f);
					}
					if (!dictionary[0f].ContainsKey(ticontrolPoint.nation))
					{
						dictionary[0f].Add(ticontrolPoint.nation, 0);
					}
					Dictionary<TINationState, int> dictionary2 = dictionary[0f];
					TINationState tinationState = ticontrolPoint.nation;
					dictionary2[tinationState]++;
				}
			}
			list = list.OrderByDescending<float, float>((float x) => x).ToList<float>();
			foreach (float num2 in list)
			{
				if (num2 == 0f)
				{
					stringBuilder.Append(Loc.T("UI.Nations.CPMaint7", new object[]
					{
						num2,
						base.activePlayer.inlineControlPointCapIcon
					})).AppendLine(Loc.T("UI.Nations.CPMaint9"));
				}
				else
				{
					StringBuilder stringBuilder6 = stringBuilder;
					string text6 = "UI.Nations.CPMaint7";
					object[] array5 = new object[2];
					int num3 = 0;
					float num4 = num2;
					array5[num3] = num4.ToString("N2");
					array5[1] = base.activePlayer.inlineControlPointCapIcon;
					stringBuilder6.AppendLine(Loc.T(text6, array5));
				}
				stringBuilder.Append("  ");
				foreach (TINationState tinationState2 in dictionary[num2].Keys)
				{
					stringBuilder.Append(Loc.T("UI.Nations.CPMaint8", new object[]
					{
						tinationState2.displayName,
						dictionary[num2][tinationState2].ToString("N0"),
						num2.ToString("N2"),
						(num2 * (float)dictionary[num2][tinationState2]).ToString("N2")
					}));
				}
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.AppendLine().AppendLine();
			}
			this.CPMaintenanceText.SetText(stringBuilder.ToString());
		}

		// Token: 0x04003819 RID: 14361
		public TMP_Text NationsPanelHeader;

		// Token: 0x0400381A RID: 14362
		public Canvas primaryCanvas;

		// Token: 0x0400381B RID: 14363
		public Toggle filterFactionToggle;

		// Token: 0x0400381C RID: 14364
		public UITutorialController NationsScreenUITutorialController;

		// Token: 0x0400381D RID: 14365
		[HideInInspector]
		public Dictionary<NationsScreenNationListItemController, TIGameState> nationItemDictionary;

		// Token: 0x0400381E RID: 14366
		[HideInInspector]
		public Dictionary<NationsScreenNationListItemController, bool> nationOpenedStatus = new Dictionary<NationsScreenNationListItemController, bool>();

		// Token: 0x0400381F RID: 14367
		[HideInInspector]
		public TIFactionState filterFaction;

		// Token: 0x04003820 RID: 14368
		public ListManagerBase primaryList;

		// Token: 0x04003821 RID: 14369
		public List<NationScreenNationListItemModel> nationModels = new List<NationScreenNationListItemModel>();

		// Token: 0x04003822 RID: 14370
		public NationScreenNationListAdapter nationListAdapter;

		// Token: 0x04003823 RID: 14371
		private bool initialized;

		// Token: 0x04003824 RID: 14372
		private SortNationDataBy currentNationSort;

		// Token: 0x04003825 RID: 14373
		private bool reverseSort;

		// Token: 0x04003826 RID: 14374
		public GameObject fullScreenPanel;

		// Token: 0x04003827 RID: 14375
		public TMP_Text showAllNationsText;

		// Token: 0x04003828 RID: 14376
		public TMP_Text nameColumnText;

		// Token: 0x04003829 RID: 14377
		public TMP_Text controlPointColumnText;

		// Token: 0x0400382A RID: 14378
		public Image playerFactionIcon;

		// Token: 0x0400382B RID: 14379
		public Image highestPopularityIcon;

		// Token: 0x0400382C RID: 14380
		public Image stoFightersPanelImage;

		// Token: 0x0400382D RID: 14381
		public Button stoFightersPanelButton;

		// Token: 0x0400382E RID: 14382
		private const float updateDelta_s = 5f;

		// Token: 0x0400382F RID: 14383
		private float timeToNextUpdate_s;

		// Token: 0x04003830 RID: 14384
		private bool firstSort;

		// Token: 0x04003831 RID: 14385
		public bool canViewSTOFighters;

		// Token: 0x04003832 RID: 14386
		private List<TIGameState> allNationsList = new List<TIGameState>();

		// Token: 0x04003833 RID: 14387
		[HideInInspector]
		public bool suppressDropDownAudio;

		// Token: 0x04003834 RID: 14388
		[Header("CP Breakdown")]
		public GameObject CPBreakdown;

		// Token: 0x04003835 RID: 14389
		public TMP_Text CPBreakdownHeader;

		// Token: 0x04003836 RID: 14390
		public TMP_Text CPMaintenanceText;

		// Token: 0x04003837 RID: 14391
		public TMP_Text OpenCPBreakdownButtonText;

		// Token: 0x04003838 RID: 14392
		public TMP_Text CloseCPBreakdownButtonText;
	}
}
