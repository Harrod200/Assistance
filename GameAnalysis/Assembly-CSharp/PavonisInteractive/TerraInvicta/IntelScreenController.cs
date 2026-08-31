using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.UI;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000881 RID: 2177
	public class IntelScreenController : CanvasControllerBase, IInfoScreen, ICanvas
	{
		// Token: 0x17000ED2 RID: 3794
		// (get) Token: 0x06005144 RID: 20804 RVA: 0x0023889D File Offset: 0x00236A9D
		public static IntelScreenController Singleton
		{
			get
			{
				return World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<IntelScreenController>();
			}
		}

		// Token: 0x06005145 RID: 20805 RVA: 0x002388B0 File Offset: 0x00236AB0
		public override void Initialize()
		{
			base.Initialize();
			this.headerText.SetText(Loc.T("UI.Intel.Header"));
			this.globalPublicOpinionHeader.SetText(Loc.T("UI.Intel.PublicOpinionHeader"));
			this.globalCommodityPricesHeader.SetText(Loc.T("UI.Intel.CommodityPricesHeader"));
			this.globalCommodityPricesHeaderDescription.SetText(Loc.T("UI.Intel.Prices"));
			this.globalEnvironmentalDamageHeader.SetText(Loc.T("UI.Intel.EnvironmentHeader"));
			this.globalDataHeader.SetText(Loc.T("UI.Intel.GlobalDataHeader"));
			this.alienTabText.SetText(Loc.T("UI.Intel.AlienTabText"));
			this.factionTabText.SetText(Loc.T("UI.Intel.FactionTabText"));
			this.globalTabText.SetText(Loc.T("UI.Intel.GlobalTabText"));
			this.spaceBodyTabText.SetText(Loc.T("UI.Intel.SpaceBodies"));
			this.habSiteTabText.SetText(Loc.T("UI.Intel.HabSiteTabText"));
			this.transferPlannerTabText.SetText(Loc.T("UI.Intel.TransferPlannerHeader"));
			this.filterProspectedText.SetText(Loc.T("UI.Intel.FilterProspected"));
			this.globalWarsHeader.SetText(Loc.T("UI.Intel.WarsHeader"));
			this.alienCouncilorsHeaderText.SetText(Loc.T("UI.Intel.AlienCouncilorsHeader"));
			this.alienEventsHeaderText.SetText(Loc.T("UI.Intel.AlienEventsHeader"));
			this.alienSitesHeaderText.SetText(Loc.T("UI.Intel.AlienAssetsHeader"));
			this.alienFleetsHeaderText.SetText(Loc.T("UI.Intel.AlienFleetsHeader"));
			this.alienHabsHeaderText.SetText(Loc.T("UI.Intel.AlienHabsHeader"));
			this.spacebodyHeaderName.SetText(Loc.T("UI.Nations.Name"));
			this.spacebodyHeaderDescription.SetText(Loc.T("UI.Intel.Spacebody.Description"));
			this.spacebodyHeaderMiningDescription.SetText(Loc.T("UI.Intel.Spacebody.MiningDescription"));
			this.spacebodyHeaderOrbit.SetText(Loc.T("UI.Intel.Spacebody.Orbit"));
			this.spacebodyHeaderDimensions.SetText(Loc.T("UI.Intel.Spacebody.Dimensions"));
			this.spacebodyLaunchWindowHeader.SetText(Loc.T("UI.Intel.EarthLaunchWindow"));
			this.spaceBodyTagWindowHeader.SetText(Loc.T("UI.Intel.SpaceBody.TagWindowHeader"));
			this.habSiteLaunchWindowHeader.SetText(Loc.T("UI.Intel.EarthLaunchWindow"));
			this.habSiteHeaderName.SetText(Loc.T("UI.Nations.Name"));
			this.basesHeaderName.SetText(Loc.T("UI.Habs.BasesHeader"));
			this.stationsHeaderName.SetText(Loc.T("UI.Habs.StationsHeader"));
			this.habSiteHeaderDescription.SetText(Loc.T("UI.Intel.Spacebody.MiningDescription"));
			this.habSiteHeaderSpacebodyName.SetText(Loc.T("UI.Intel.HabSite.Spacebody"));
			this.habSiteHeaderHabName.SetText(Loc.T("UI.Intel.HabSite.HabName"));
			this.globalIdeologyPortions[0].fillAmount = 1f;
			this.globalIdeologyPortions[0].color = new Color(0.14117648f, 0.19215687f, 0.23137255f);
			this.globalIdeologyPortions[0].enabled = true;
			for (int i = 1; i < this.globalIdeologyPortions.Count; i++)
			{
				this.globalIdeologyPortions[i].enabled = false;
			}
			this.globalAtrocitiesHeader.SetText(Loc.T("UI.Intel.AtrocitiesHeader"));
			TIFactionState[] array = GameStateManager.AllHumanFactions();
			this.atrocitiesGrid.SetListSize<IntelAtrocitiesGridItemController>(array.Length, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.atrocitiesGrid.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__134.<>p__0 == null)
					{
						IntelScreenController.<>o__134.<>p__0 = CallSite<Func<CallSite, object, IntelAtrocitiesGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAtrocitiesGridItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__134.<>p__0.Target(IntelScreenController.<>o__134.<>p__0, enumerator.Current).InitListItem(array[num++]);
				}
			}
			Func<IntelAtrocitiesGridItemController, IComparable> func = (IntelAtrocitiesGridItemController gridItem) => gridItem.faction.atrocities;
			this.atrocitiesGrid.transform.SortChildren<IntelAtrocitiesGridItemController>(func, false);
			int num2 = 0;
			this.globalPublicOpinionList.SetListSize<PublicOpinionListItemController>(array.Length + 1, false, false);
			this.enviroTip.SetDelegate("BodyText", () => Loc.T("UI.Nation.GHGChangeHeader"));
			NationInfoController.SetGHGTableTipDelegates(this.enviroTip);
			using (IEnumerator<object> enumerator = this.globalPublicOpinionList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__134.<>p__1 == null)
					{
						IntelScreenController.<>o__134.<>p__1 = CallSite<Func<CallSite, object, PublicOpinionListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PublicOpinionListItemController), typeof(IntelScreenController)));
					}
					PublicOpinionListItemController publicOpinionListItemController = IntelScreenController.<>o__134.<>p__1.Target(IntelScreenController.<>o__134.<>p__1, enumerator.Current);
					if (num2 < array.Length)
					{
						publicOpinionListItemController.InitListItem(array[num2], array[num2].ideology.ideology);
					}
					else
					{
						publicOpinionListItemController.InitListItem(null, FactionIdeology.Undecided);
					}
					num2++;
				}
			}
			this.PopulatePermanentDropdowns();
			this.factionTab.gameObject.SetActive(true);
			this.alienTab.gameObject.SetActive(true);
			this.globalTab.gameObject.SetActive(true);
			this.spaceBodyTab.gameObject.SetActive(true);
			this.transferTab.gameObject.SetActive(true);
			this.leaderBioPanelObject.SetActive(false);
			this.leaderBioDimmerObject.SetActive(false);
			this.UpdateActivePlayerUIElements(true);
			this.sitesTabButtonObject.SetActive(false);
		}

		// Token: 0x06005146 RID: 20806 RVA: 0x00238E58 File Offset: 0x00237058
		public void SetSpacebodyListModelData()
		{
			this.spacebodyModels.Clear();
			for (int i = 0; i < this.allSpacebodies.Count; i++)
			{
				IntelScreenSpacebodyListItemModel intelScreenSpacebodyListItemModel = new IntelScreenSpacebodyListItemModel();
				IntelScreenSpacebodyListItem_Data intelScreenSpacebodyListItem_Data = new IntelScreenSpacebodyListItem_Data
				{
					controller = this,
					showInList = true
				};
				intelScreenSpacebodyListItem_Data.SetData(this.allSpacebodies[i]);
				intelScreenSpacebodyListItemModel.IntelScreenSpacebodyListItemData = intelScreenSpacebodyListItem_Data;
				this.spacebodyModels.Add(intelScreenSpacebodyListItemModel);
			}
		}

		// Token: 0x06005147 RID: 20807 RVA: 0x00238EC5 File Offset: 0x002370C5
		public void UpdateSpaceBodyListModelData()
		{
			this.spacebodyListAdapter.SetItems(this.spacebodyModels);
		}

		// Token: 0x06005148 RID: 20808 RVA: 0x00238ED8 File Offset: 0x002370D8
		public void UpdateSpaceBodyListSortTag()
		{
			if (this.currentSpaceSort == SortSpaceDataBy.Tag)
			{
				this.sortAscend = !this.sortAscend;
				this.UpdateSpaceBodySort();
				this.sortAscend = !this.sortAscend;
			}
		}

		// Token: 0x06005149 RID: 20809 RVA: 0x00238F08 File Offset: 0x00237108
		public void SetHabSiteListModelData()
		{
			this.habSiteModels.Clear();
			List<TIHabSiteState> list = new List<TIHabSiteState>();
			list = GameControl.control.activePlayer.ProspectedSpaceBodies().SelectMany<TISpaceBodyState, TIHabSiteState>((TISpaceBodyState x) => x.habSites).ToList<TIHabSiteState>();
			for (int i = 0; i < list.Count; i++)
			{
				IntelScreenHabSiteListItemModel intelScreenHabSiteListItemModel = new IntelScreenHabSiteListItemModel();
				IntelScreenHabSiteListItem_Data intelScreenHabSiteListItem_Data = new IntelScreenHabSiteListItem_Data();
				intelScreenHabSiteListItem_Data.controller = this;
				intelScreenHabSiteListItem_Data.showInList = true;
				intelScreenHabSiteListItem_Data.SetData(list[i]);
				intelScreenHabSiteListItemModel.IntelScreenHabSiteListItemData = intelScreenHabSiteListItem_Data;
				this.habSiteModels.Add(intelScreenHabSiteListItemModel);
			}
		}

		// Token: 0x0600514A RID: 20810 RVA: 0x00238FAA File Offset: 0x002371AA
		public void UpdateHabSiteListModelData()
		{
			this.habSiteListAdapter.SetItems(this.habSiteModels);
		}

		// Token: 0x0600514B RID: 20811 RVA: 0x00238FBD File Offset: 0x002371BD
		public void UpdateHabSiteListSortTag()
		{
			if (this.currentHabSiteSort == SortSpaceDataBy.Tag)
			{
				this.UpdateHabSiteSort();
			}
		}

		// Token: 0x0600514C RID: 20812 RVA: 0x00238FD0 File Offset: 0x002371D0
		public override void UpdateActivePlayerUIElements(bool startup)
		{
			List<TIFactionState> list = (from x in GameStateManager.AllHumanFactions()
				where x != base.activePlayer
				select x).ToList<TIFactionState>();
			this.habSiteTab.gameObject.SetActive(base.activePlayer.ProspectedSpaceBodies().Count > 0);
			this.factionsList.SetListSize<IntelFactionGridItemController>(list.Count, false, false);
			this.factionListControllers.Clear();
			int num = 0;
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__141.<>p__0 == null)
					{
						IntelScreenController.<>o__141.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelFactionGridItemController intelFactionGridItemController = IntelScreenController.<>o__141.<>p__0.Target(IntelScreenController.<>o__141.<>p__0, enumerator.Current);
					intelFactionGridItemController.Initialize(list[num++], this);
					this.factionListControllers.Add(intelFactionGridItemController);
					intelFactionGridItemController.Refresh();
				}
			}
			if (!startup)
			{
				using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (IntelScreenController.<>o__141.<>p__1 == null)
						{
							IntelScreenController.<>o__141.<>p__1 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
						}
						IntelScreenController.<>o__141.<>p__1.Target(IntelScreenController.<>o__141.<>p__1, enumerator.Current).factionTabbedPaneManager.ClearActiveTab();
					}
				}
			}
		}

		// Token: 0x0600514D RID: 20813 RVA: 0x0023915C File Offset: 0x0023735C
		private void PopulatePermanentDropdowns()
		{
			this.OverrideDropdownMultiselectLabels();
			List<TIFactionState> list = (from x in GameStateManager.AllFactions()
				orderby x == GameControl.control.activePlayer descending, x.IsAlienFaction
				select x).ToList<TIFactionState>();
			this.factionsDropdowns[0].captionText.SetText(Loc.T("UI.Habs.SelectFaction"));
			this.factionsDropdowns[1].captionText.SetText(Loc.T("UI.Habs.SelectFaction"));
			this.factionsDropdowns[0].ClearOptions();
			this.factionsDropdowns[1].ClearOptions();
			this.factionDropdownLookup = new Dictionary<int, TIFactionState>();
			TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.AllFactions")
			};
			this.factionsDropdowns[0].options.Add(optionData);
			this.factionsDropdowns[1].options.Add(optionData);
			this.factionDropdownLookup.Add(0, null);
			TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Habs.AllHumanFactions")
			};
			this.factionsDropdowns[0].options.Add(optionData2);
			this.factionsDropdowns[1].options.Add(optionData2);
			this.factionDropdownLookup.Add(1, null);
			TMP_Dropdown.OptionData optionData3 = new TMP_Dropdown.OptionData
			{
				text = Loc.T("UI.Intel.NoFactionPresence")
			};
			this.factionsDropdowns[0].options.Add(optionData3);
			this.factionsDropdowns[1].options.Add(optionData3);
			this.factionDropdownLookup.Add(2, null);
			TMP_Dropdown.OptionData optionData4 = new TMP_Dropdown.OptionData
			{
				text = base.activePlayer.displayNameCapitalizedWithColor,
				image = base.activePlayer.factionIcon64UI
			};
			this.factionsDropdowns[0].options.Add(optionData4);
			this.factionsDropdowns[1].options.Add(optionData4);
			this.factionDropdownLookup.Add(3, base.activePlayer);
			int num = 4;
			foreach (TIFactionState tifactionState in list)
			{
				if (tifactionState != base.activePlayer)
				{
					TMP_Dropdown.OptionData optionData5 = new TMP_Dropdown.OptionData
					{
						text = tifactionState.displayNameCapitalizedWithColor,
						image = tifactionState.factionIcon64UI
					};
					this.factionsDropdowns[0].options.Add(optionData5);
					this.factionsDropdowns[1].options.Add(optionData5);
					this.factionDropdownLookup.Add(num++, tifactionState);
				}
			}
			this.locationDropdowns_High[0].captionText.SetText(Loc.T("UI.Habs.SelectLocation"));
			this.locationDropdowns_High[1].captionText.SetText(Loc.T("UI.Habs.SelectLocation"));
			this.locationDropdowns_High[0].ClearOptions();
			this.locationDropdowns_High[1].ClearOptions();
			num = 0;
			this.highLocationDropdownLookup = new Dictionary<int, TISpaceBodyState>();
			foreach (string text in TargetSelectionTool.primaryNavigatorBodyTemplateNames)
			{
				TISpaceBodyState tispaceBodyState = GameStateManager.FindByTemplate<TISpaceBodyState>(text, false);
				if (tispaceBodyState != null)
				{
					TMP_Dropdown.OptionData optionData6 = new TMP_Dropdown.OptionData();
					switch (tispaceBodyState.objectType)
					{
					case SpaceObjectType.Star:
						continue;
					default:
						optionData6.text = tispaceBodyState.displayName;
						break;
					case SpaceObjectType.DwarfPlanet:
					case SpaceObjectType.Asteroid:
					case SpaceObjectType.Comet:
						if (GameStateManager.InnerSystemAsteroids(true).Contains(tispaceBodyState))
						{
							optionData6.text = Loc.T("UI.Habs.InnerSystemAsteroids");
						}
						else if (GameStateManager.InnerAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData6.text = Loc.T("UI.Habs.InnerBelt");
						}
						else if (GameStateManager.MidAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData6.text = Loc.T("UI.Habs.MidBelt");
						}
						else if (GameStateManager.OuterAsteroidBelt(true).Contains(tispaceBodyState))
						{
							optionData6.text = Loc.T("UI.Habs.FarBelt");
						}
						else if (GameStateManager.Centaurs(true).Contains(tispaceBodyState))
						{
							optionData6.text = Loc.T("UI.Habs.Centaurs");
						}
						else if (GameStateManager.KuiperBeltObjects(true).Contains(tispaceBodyState))
						{
							optionData6.text = Loc.T("UI.Habs.KBO");
						}
						else
						{
							optionData6.text = Loc.T("UI.Habs.Other");
						}
						break;
					}
					optionData6.image = tispaceBodyState.icon;
					this.locationDropdowns_High[0].options.Add(optionData6);
					this.locationDropdowns_High[1].options.Add(optionData6);
					this.highLocationDropdownLookup.Add(num++, tispaceBodyState);
				}
			}
		}

		// Token: 0x0600514E RID: 20814 RVA: 0x00239698 File Offset: 0x00237898
		private void OverrideDropdownMultiselectLabels()
		{
			Type typeFromHandle = typeof(TMP_Dropdown);
			FieldInfo field = typeFromHandle.GetField("k_NothingOption", BindingFlags.Static | BindingFlags.NonPublic);
			FieldInfo field2 = typeFromHandle.GetField("k_EverythingOption", BindingFlags.Static | BindingFlags.NonPublic);
			FieldInfo field3 = typeFromHandle.GetField("k_MixedOption", BindingFlags.Static | BindingFlags.NonPublic);
			TMP_Dropdown.OptionData optionData = ((field != null) ? field.GetValue(null) : null) as TMP_Dropdown.OptionData;
			if (optionData != null)
			{
				optionData.text = Loc.T("UI.Habs.NoLocations");
			}
			TMP_Dropdown.OptionData optionData2 = ((field2 != null) ? field2.GetValue(null) : null) as TMP_Dropdown.OptionData;
			if (optionData2 != null)
			{
				optionData2.text = Loc.T("UI.Habs.AllLocations");
			}
			TMP_Dropdown.OptionData optionData3 = ((field3 != null) ? field3.GetValue(null) : null) as TMP_Dropdown.OptionData;
			if (optionData3 != null)
			{
				optionData3.text = Loc.T("UI.Habs.MixedLocations");
			}
		}

		// Token: 0x0600514F RID: 20815 RVA: 0x0023974C File Offset: 0x0023794C
		public override void Show()
		{
			base.Show();
			if (!GameControl.control.skirmishMode)
			{
				this.allSpacebodies = (from n in GameStateManager.AllSpaceBodies()
					where n.objectType != SpaceObjectType.Star
					select n).ToList<TISpaceBodyState>();
				this.SetSpacebodyListModelData();
			}
			this.tabManager.Toggle(this.factionTab);
			this.ShowFactionTabUITutorial();
			this.RefreshAll();
		}

		// Token: 0x06005150 RID: 20816 RVA: 0x002397C4 File Offset: 0x002379C4
		public override void Hide()
		{
			this.CleanUpDisplay();
			this.TransferPlanner.OnNextClose = null;
			this.alienTabUITutorialController.HideTutorial();
			this.factionTabUITutorialController.HideTutorial();
			this.globalTabUITutorialController.HideTutorial();
			this.spaceTabUITutorialController.HideTutorial();
			this.transferPlannerUITutorialController.HideTutorial();
			this.prospectingUITutorialController.HideTutorial();
			base.Hide();
		}

		// Token: 0x06005151 RID: 20817 RVA: 0x0023982C File Offset: 0x00237A2C
		public void RefreshAll()
		{
			this.cachedKnownStationsList = base.activePlayer.KnownHabs;
			this.cachedKnownHabsList = base.activePlayer.KnownHabs;
			if (base.activePlayer.ProspectedSpaceBodies().Count > 0)
			{
				this.habSiteTab.gameObject.SetActive(true);
				this.sitesTabButtonObject.SetActive(true);
			}
			else
			{
				this.habSiteTab.gameObject.SetActive(false);
				this.sitesTabButtonObject.SetActive(false);
			}
			this.RefreshActiveTab(true);
		}

		// Token: 0x06005152 RID: 20818 RVA: 0x002398B1 File Offset: 0x00237AB1
		public override void Refresh()
		{
			if (!base.Paused && TIFrameCounter.FrameCount % 2999 == 0)
			{
				this.RefreshActiveTab(false);
			}
		}

		// Token: 0x06005153 RID: 20819 RVA: 0x002398CF File Offset: 0x00237ACF
		public void CloseInfoScreen(bool toggle = false)
		{
			this.CleanUpDisplay();
			base.canvasManager.HideInfoScreen<IntelScreenController>(toggle);
		}

		// Token: 0x06005154 RID: 20820 RVA: 0x002398E3 File Offset: 0x00237AE3
		public override void UpdateUIScaling()
		{
			base.UpdateUIScaling();
			this.primaryPanelTransform.anchoredPosition = new Vector2(0f, (float)((base.VerticalScaleValueLimit() >= 940f) ? (-100) : (-85)));
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x00239914 File Offset: 0x00237B14
		public void RefreshActiveTab(bool includingSpaceBodies)
		{
			if (this.tabManager.activeTab == this.alienTab)
			{
				this.RefreshAlienTab();
				return;
			}
			if (this.tabManager.activeTab == this.factionTab)
			{
				this.RefreshFactionTab();
				return;
			}
			if (this.tabManager.activeTab == this.globalTab)
			{
				this.RefreshGlobalTab();
				return;
			}
			if (this.tabManager.activeTab == this.spaceBodyTab && includingSpaceBodies)
			{
				this.RefreshSpaceBodies();
				return;
			}
			if (this.tabManager.activeTab == this.habSiteTab)
			{
				this.RefreshHabSites();
			}
		}

		// Token: 0x06005156 RID: 20822 RVA: 0x002399C0 File Offset: 0x00237BC0
		public void ForceActiveTab(TIGameState stateForTab)
		{
			if (stateForTab is TIGlobalValuesState)
			{
				if (this.tabManager.activeTab != this.globalTab)
				{
					this.tabManager.Toggle(this.globalTab);
				}
				this.RefreshActiveTab(false);
				return;
			}
			if (!stateForTab.isFactionState)
			{
				if (stateForTab.isSpaceObjectState)
				{
					if (this.tabManager.activeTab != this.spaceBodyTab)
					{
						this.tabManager.Toggle(this.spaceBodyTab);
					}
					this.RefreshActiveTab(true);
				}
				return;
			}
			if (stateForTab.ref_faction.IsAlienFaction)
			{
				if (this.tabManager.activeTab != this.alienTab)
				{
					this.tabManager.Toggle(this.alienTab);
				}
				this.RefreshActiveTab(false);
				return;
			}
			if (this.tabManager.activeTab != this.factionTab)
			{
				this.tabManager.Toggle(this.factionTab);
			}
			this.RefreshActiveTab(false);
		}

		// Token: 0x06005157 RID: 20823 RVA: 0x00239AB5 File Offset: 0x00237CB5
		public void Close()
		{
			this.HideTutorials();
			if (this.OnExit != null)
			{
				this.OnExit();
			}
			this.CloseInfoScreen(false);
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x00239AD7 File Offset: 0x00237CD7
		public void OnExitButtonClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CloseLarge", false, false);
			this.Close();
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x00239AEB File Offset: 0x00237CEB
		public void OnCloseAndPlayButtonSelected()
		{
			this.OnExitButtonClicked();
			base.gameTime.Play();
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x00239B00 File Offset: 0x00237D00
		public void CleanUpDisplay()
		{
			if (this.tabManager != null && this.tabManager.activeTab != null)
			{
				this.tabManager.Toggle(this.tabManager.activeTab);
			}
			if (this.leaderBioPanelObject.activeSelf)
			{
				this.leaderBioPanelObject.SetActive(false);
				this.leaderBioDimmerObject.SetActive(false);
			}
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x00239B6C File Offset: 0x00237D6C
		public void RefreshFactionTab()
		{
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__158.<>p__0 == null)
					{
						IntelScreenController.<>o__158.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__158.<>p__0.Target(IntelScreenController.<>o__158.<>p__0, enumerator.Current).Refresh();
				}
			}
		}

		// Token: 0x0600515C RID: 20828 RVA: 0x00239BF8 File Offset: 0x00237DF8
		public void RefreshGlobalTab()
		{
			Dictionary<FactionIdeology, float> globalPublicOpinionProportions = TIGlobalValuesState.GlobalValues.GetGlobalPublicOpinionProportions();
			float num = globalPublicOpinionProportions[FactionIdeology.Undecided];
			int num2 = 1;
			foreach (FactionIdeology factionIdeology in from x in globalPublicOpinionProportions.Keys
				where x != FactionIdeology.Undecided
				select x into y
				orderby TIFactionIdeologyTemplate.GetIdeologyTemplate(y).sortOrder descending
				select y)
			{
				this.globalIdeologyPortions[num2].color = TIFactionIdeologyTemplate.GetFactionByIdeology(factionIdeology).template.color;
				this.globalIdeologyPortions[num2].fillAmount = 1f - num;
				this.globalIdeologyPortions[num2++].enabled = true;
				num += globalPublicOpinionProportions[factionIdeology];
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (TIFactionIdeologyTemplate tifactionIdeologyTemplate in GameStateManager.ActiveHumanIdeologies())
			{
				if (tifactionIdeologyTemplate.undecided)
				{
					stringBuilder.AppendLine(Loc.T("UI.Nation.PublicOpinionLineNoFaction", new object[]
					{
						GameStateManager.UndecidedIdeology().ideologyStrPublicOpinion,
						globalPublicOpinionProportions[tifactionIdeologyTemplate.ideology].ToPercent("P0"),
						string.Empty
					}));
				}
				else
				{
					TIFactionState factionByIdeologyTemplate = TIFactionIdeologyTemplate.GetFactionByIdeologyTemplate(tifactionIdeologyTemplate);
					stringBuilder.AppendLine(Loc.T("UI.Nation.PublicOpinionLineFaction", new object[]
					{
						factionByIdeologyTemplate.ideology.ideologyStrPublicOpinion,
						factionByIdeologyTemplate.template.inlineColorString,
						factionByIdeologyTemplate.displayName,
						globalPublicOpinionProportions[tifactionIdeologyTemplate.ideology].ToPercent("P0"),
						string.Empty
					}));
				}
			}
			this.globalPublicOpinionBreakdown.SetText(stringBuilder.ToString());
			stringBuilder.Clear();
			using (IEnumerator<object> enumerator3 = this.globalPublicOpinionList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (IntelScreenController.<>o__159.<>p__0 == null)
					{
						IntelScreenController.<>o__159.<>p__0 = CallSite<Func<CallSite, object, PublicOpinionListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(PublicOpinionListItemController), typeof(IntelScreenController)));
					}
					PublicOpinionListItemController publicOpinionListItemController = IntelScreenController.<>o__159.<>p__0.Target(IntelScreenController.<>o__159.<>p__0, enumerator3.Current);
					publicOpinionListItemController.UpdateListItem(globalPublicOpinionProportions[publicOpinionListItemController.ideology].ToPercent("P0"));
				}
			}
			TIGlobalValuesState tiglobalValuesState = GameStateManager.GlobalValues();
			int num3 = base.gameTime.currentTime.month - 1;
			this.globalEnvironmentalDamage_GTA.SetText(Loc.T("UI.Intel.GlobalTemperatureAnomaly", new object[]
			{
				TIUtilities.ForceValueSign(tiglobalValuesState.temperatureAnomaly_C, false, false, ""),
				TIUtilities.ForceValueSign(tiglobalValuesState.temperatureAnomaly_F, false, false, "")
			}));
			this.globalEnvironmentalDamage_GSLA.SetText(Loc.T("UI.Intel.GlobalSeaLevelAnomaly", new object[] { TIUtilities.ForceValueSign(tiglobalValuesState.globalSeaLevelAnomaly_cm, false, false, "") }));
			TMP_Text tmp_Text = this.globalEnvironmentalDamage_MAGDPI;
			string text = "UI.Intel.MeanAnnualGDPImpact";
			object[] array = new object[1];
			array[0] = TINationState.MeanAnnualGDPDamage(tiglobalValuesState.temperatureAnomaly_C, GameStateManager.AllExtantNations().Average<TINationState>((TINationState x) => x.inequality)).ToPercent("P2").Replace(" ", "");
			tmp_Text.SetText(Loc.T(text, array));
			this.globalEnvironmentalDamage_ACD.SetText(Loc.T("UI.Intel.GlobalCarbonDioxide"));
			this.globalEnvironmentalDamage_ACDC.SetText(Loc.T("UI.Intel.CurrentTemperatureImpact", new object[]
			{
				tiglobalValuesState.earthAtmosphericCO2_ppm.ToString("N2"),
				TIUtilities.ForceValueSign(tiglobalValuesState.temperatureAnomalyCO2_C, false, false, "")
			}));
			this.globalEnvironmentalDamage_ACDS.SetText(Loc.T("UI.Intel.SafeGreenhouseGasLevel", new object[] { 325.68f.ToString("N2") }));
			this.globalEnvironmentalDamage_ACDY.transform.parent.gameObject.SetActive(tiglobalValuesState.pastEarthAtmosphericCO2_ppm[num3] > 0f);
			if (tiglobalValuesState.pastEarthAtmosphericCO2_ppm[num3] > 0f)
			{
				this.globalEnvironmentalDamage_ACDY.SetText(Loc.T("UI.Intel.PreviousTemperatureImpact", new object[] { tiglobalValuesState.pastEarthAtmosphericCO2_ppm[num3].ToString("N2") }));
			}
			this.globalEnvironmentalDamage_AM.SetText(Loc.T("UI.Intel.GlobalMethane"));
			this.globalEnvironmentalDamage_AMC.SetText(Loc.T("UI.Intel.CurrentTemperatureImpact", new object[]
			{
				tiglobalValuesState.earthAtmosphericCH4_ppm.ToString("N2"),
				TIUtilities.ForceValueSign(tiglobalValuesState.temperatureAnomalyCH4_C, false, false, "")
			}));
			this.globalEnvironmentalDamage_AMS.SetText(Loc.T("UI.Intel.SafeGreenhouseGasLevel", new object[] { 1.3f.ToString("N2") }));
			this.globalEnvironmentalDamage_AMY.transform.parent.gameObject.SetActive(tiglobalValuesState.pastEarthAtmosphericCH4_ppm[num3] > 0f);
			if (tiglobalValuesState.pastEarthAtmosphericCH4_ppm[num3] > 0f)
			{
				this.globalEnvironmentalDamage_AMY.SetText(Loc.T("UI.Intel.PreviousTemperatureImpact", new object[] { tiglobalValuesState.pastEarthAtmosphericCH4_ppm[num3].ToString("N2") }));
			}
			this.globalEnvironmentalDamage_ANO.SetText(Loc.T("UI.Intel.GlobalNitrousOxide"));
			this.globalEnvironmentalDamage_ANOC.SetText(Loc.T("UI.Intel.CurrentTemperatureImpact", new object[]
			{
				tiglobalValuesState.earthAtmosphericN2O_ppm.ToString("N2"),
				TIUtilities.ForceValueSign(tiglobalValuesState.temperatureAnomalyN2O_C, false, false, "")
			}));
			this.globalEnvironmentalDamage_ANOS.SetText(Loc.T("UI.Intel.SafeGreenhouseGasLevel", new object[] { 0.29f.ToString("N2") }));
			this.globalEnvironmentalDamage_ANOY.transform.parent.gameObject.SetActive(tiglobalValuesState.pastEarthAtmosphericN2O_ppm[num3] > 0f);
			if (tiglobalValuesState.pastEarthAtmosphericN2O_ppm[num3] > 0f)
			{
				this.globalEnvironmentalDamage_ANOY.SetText(Loc.T("UI.Intel.PreviousTemperatureImpact", new object[] { tiglobalValuesState.pastEarthAtmosphericN2O_ppm[num3].ToString("N2") }));
			}
			this.globalEnvironmentalDamage_ESA.SetText(Loc.T("UI.Intel.StratosphericAerosols"));
			this.globalEnvironmentalDamage_ESAC.SetText(Loc.T("UI.Intel.CurrentTemperatureImpact", new object[]
			{
				TIUtilities.FormatSmallNumber(tiglobalValuesState.stratosphericAerosols_ppm, 7, 2, true, false),
				TIUtilities.ForceValueSign(tiglobalValuesState.temperatureAnomalyStratosphericAerosols_C, false, false, "")
			}));
			this.globalCommodityPricesText_Water.SetText(Loc.T("UI.Intel.NonSellableResource", new object[]
			{
				TemplateManager.global.waterInlineSpritePath,
				Loc.T("UI.Global.Water"),
				TemplateManager.global.moneyInlineSpritePath,
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetPurchaseResourceMarketValue(FactionResource.Water), 1, 7, 0, false, false)
			}));
			this.globalCommodityPricesText_Volatiles.SetText(Loc.T("UI.Intel.NonSellableResource", new object[]
			{
				TemplateManager.global.volatilesInlineSpritePath,
				Loc.T("UI.Global.Volatiles"),
				TemplateManager.global.moneyInlineSpritePath,
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetPurchaseResourceMarketValue(FactionResource.Volatiles), 1, 7, 0, false, false)
			}));
			this.globalCommodityPricesText_Metals.SetText(Loc.T("UI.Intel.SellableResource", new object[]
			{
				TemplateManager.global.metalsInlineSpritePath,
				Loc.T("UI.Global.Metals"),
				TemplateManager.global.moneyInlineSpritePath,
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetPurchaseResourceMarketValue(FactionResource.Metals), 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetModifiedResourceMarketValueForSelling(base.activePlayer, FactionResource.Metals), 1, 7, 0, false, false)
			}));
			this.globalCommodityPricesText_NobleMetals.SetText(Loc.T("UI.Intel.SellableResource", new object[]
			{
				TemplateManager.global.noblesInlineSpritePath,
				Loc.T("UI.Global.NobleMetals"),
				TemplateManager.global.moneyInlineSpritePath,
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetPurchaseResourceMarketValue(FactionResource.NobleMetals), 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetModifiedResourceMarketValueForSelling(base.activePlayer, FactionResource.NobleMetals), 1, 7, 0, false, false)
			}));
			this.globalCommodityPricesText_Fissiles.SetText(Loc.T("UI.Intel.SellableResource", new object[]
			{
				TemplateManager.global.fissilesInlineSpritePath,
				Loc.T("UI.Global.Fissiles"),
				TemplateManager.global.moneyInlineSpritePath,
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetPurchaseResourceMarketValue(FactionResource.Fissiles), 1, 7, 0, false, false),
				TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetModifiedResourceMarketValueForSelling(base.activePlayer, FactionResource.Fissiles), 1, 7, 0, false, false)
			}));
			this.globalCommodityPricesText_Antimatter.transform.parent.gameObject.SetActive(base.activePlayer.UnlockedAntimatter);
			if (base.activePlayer.UnlockedAntimatter)
			{
				this.globalCommodityPricesText_Antimatter.SetText(Loc.T("UI.Intel.NonBuyableResource", new object[]
				{
					TemplateManager.global.antimatterInlineSpritePath,
					Loc.T("UI.Global.Antimatter"),
					TemplateManager.global.moneyInlineSpritePath,
					TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetModifiedResourceMarketValueForSelling(base.activePlayer, FactionResource.Antimatter), 1, 7, 0, false, false)
				}));
			}
			this.globalCommodityPricesText_Exotics.transform.parent.gameObject.SetActive(base.activePlayer.UnlockedExotics);
			if (base.activePlayer.UnlockedExotics)
			{
				this.globalCommodityPricesText_Exotics.SetText(Loc.T("UI.Intel.NonBuyableResource", new object[]
				{
					TemplateManager.global.exoticsInlineSpritePath,
					Loc.T("UI.Global.Exotics"),
					TemplateManager.global.moneyInlineSpritePath,
					TIUtilities.FormatBigOrSmallNumber(tiglobalValuesState.GetModifiedResourceMarketValueForSelling(base.activePlayer, FactionResource.Exotics), 1, 7, 0, false, false)
				}));
			}
			this.globalWarsList.SetListSize<IntelWarsListItemController>(tiglobalValuesState.interstateWars.Count, false, false);
			int num4 = 0;
			using (IEnumerator<object> enumerator3 = this.globalWarsList.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (IntelScreenController.<>o__159.<>p__1 == null)
					{
						IntelScreenController.<>o__159.<>p__1 = CallSite<Func<CallSite, object, IntelWarsListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelWarsListItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__159.<>p__1.Target(IntelScreenController.<>o__159.<>p__1, enumerator3.Current).SetListItem(tiglobalValuesState.interstateWars[num4++]);
				}
			}
			double num5 = (double)GameStateManager.AllRegions().Sum<TIRegionState>((TIRegionState x) => x.populationInMillions) * 1000000.0;
			double num6 = GameStateManager.AllNations().Sum<TINationState>((TINationState x) => x.GDP);
			double num7 = num6 / num5;
			int num8 = (from x in GameStateManager.IterateByClass<TIHabState>(false)
				where !x.IsAlien()
				select x).Sum<TIHabState>((TIHabState x) => x.crew);
			this.globalData_EarthPop.SetText(Loc.T("UI.Intel.GlobalDataEarthPop", new object[] { num5.ToString("N0") }));
			this.globalData_SpacePop.SetText(Loc.T("UI.Intel.GlobalDataSpacePop", new object[] { num8.ToString("N0") }));
			this.globalData_GDP.SetText(Loc.T("UI.Intel.GlobalDataGDP", new object[] { TIUtilities.FormatBigNumber(num6, 1, false) }));
			this.globalData_PerCapitaGDP.SetText(Loc.T("UI.Intel.GlobalDataPerCapitaGDP", new object[] { num7.ToString("N0") }));
			using (IEnumerator<object> enumerator3 = this.atrocitiesGrid.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					if (IntelScreenController.<>o__159.<>p__2 == null)
					{
						IntelScreenController.<>o__159.<>p__2 = CallSite<Func<CallSite, object, IntelAtrocitiesGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAtrocitiesGridItemController), typeof(IntelScreenController)));
					}
					IntelAtrocitiesGridItemController intelAtrocitiesGridItemController = IntelScreenController.<>o__159.<>p__2.Target(IntelScreenController.<>o__159.<>p__2, enumerator3.Current);
					intelAtrocitiesGridItemController.SetListItem(intelAtrocitiesGridItemController.faction.atrocities == 0);
				}
			}
			this.atrocitiesGrid.transform.SortChildren<IntelAtrocitiesGridItemController>(new Func<IntelAtrocitiesGridItemController, IComparable>(IntelScreenController.<RefreshGlobalTab>g__Evaluate|159_5), false);
		}

		// Token: 0x0600515D RID: 20829 RVA: 0x0023A8EC File Offset: 0x00238AEC
		public void RefreshSpaceBodies()
		{
			this.SetSpacebodyListModelData();
			this.UpdateSpaceBodySort();
			this.SetProbeAllButton();
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x0023A900 File Offset: 0x00238B00
		public void RefreshHabSites()
		{
			this.SetHabSiteListModelData();
			this.UpdateHabSiteSort();
		}

		// Token: 0x0600515F RID: 20831 RVA: 0x0023A910 File Offset: 0x00238B10
		public void SetProbeAllButton()
		{
			LaunchAllProbeOperation launchAllProbeOperation = new LaunchAllProbeOperation();
			List<TIGameState> possibleTargets = launchAllProbeOperation.GetPossibleTargets(base.activePlayer, null);
			if (possibleTargets.Count > 0)
			{
				if (possibleTargets.Count > 1)
				{
					this.probeAllButtonText.SetText(Loc.T("UI.Intel.ProbeAll", new object[] { possibleTargets.Count.ToString("N0") }));
				}
				else
				{
					this.probeAllButtonText.SetText(Loc.T("UI.Intel.ProbeAllSingle", new object[] { possibleTargets.Count.ToString("N0") }));
				}
				TIResourcesCost tiresourcesCost = launchAllProbeOperation.ResourceCostOptions(base.activePlayer, null, base.activePlayer, false)[0];
				this.probeAllButtonCost.SetText(Loc.T("UI.Intel.ProbeAllCost", new object[] { tiresourcesCost.ToString("Relevant", false, false, base.activePlayer, false, FactionResource.None) }));
				this.probeAllButton.interactable = tiresourcesCost.CanAfford(base.activePlayer, 1f, null, float.PositiveInfinity);
				return;
			}
			this.probeAllButton.interactable = false;
			this.probeAllButtonText.SetText(Loc.T("UI.Intel.ProbeAllNone"));
			this.probeAllButtonCost.SetText(Loc.T("UI.Intel.ProbeAllCostNone"));
		}

		// Token: 0x06005160 RID: 20832 RVA: 0x0023AA54 File Offset: 0x00238C54
		public void OnProbeAllClicked()
		{
			AudioManager.PlayOneShot("event:/SFX/Game_SFX/Guns/trig_SFX_Missile_Launch", false, false);
			LaunchAllProbeOperation launchAllProbeOperation = new LaunchAllProbeOperation();
			TIResourcesCost tiresourcesCost = launchAllProbeOperation.ResourceCostOptions(base.activePlayer, null, base.activePlayer, false)[0];
			launchAllProbeOperation.OnOperationConfirm(base.activePlayer, launchAllProbeOperation.GetPossibleTargets(base.activePlayer, null)[0], tiresourcesCost, null);
			this.RefreshSpaceBodies();
			this.SetProbeAllButton();
		}

		// Token: 0x06005161 RID: 20833 RVA: 0x0023AABC File Offset: 0x00238CBC
		public void RefreshAlienTab()
		{
			List<CouncilorView> list = base.activePlayer.EverKnownCouncilors(GameStateManager.AlienFaction());
			this.alienCouncilorsList.SetListSize<IntelAlienCouncilorListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.alienCouncilorsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__175.<>p__0 == null)
					{
						IntelScreenController.<>o__175.<>p__0 = CallSite<Func<CallSite, object, IntelAlienCouncilorListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAlienCouncilorListItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__175.<>p__0.Target(IntelScreenController.<>o__175.<>p__0, enumerator.Current).UpdateListItem(list[num++]);
				}
			}
			List<TISpaceFleetState> list2 = (from x in base.activePlayer.KnownFleets
				where x.faction == GameStateManager.AlienFaction()
				orderby x.GetDisplayName(base.activePlayer)
				select x).ToList<TISpaceFleetState>();
			num = 0;
			this.alienFleetsList.SetListSize<IntelAlienFleetListItemController>(list2.Count, false, false);
			using (IEnumerator<object> enumerator = this.alienFleetsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__175.<>p__1 == null)
					{
						IntelScreenController.<>o__175.<>p__1 = CallSite<Func<CallSite, object, IntelAlienFleetListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAlienFleetListItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__175.<>p__1.Target(IntelScreenController.<>o__175.<>p__1, enumerator.Current).UpdateListItem(list2[num++]);
				}
			}
			List<TIHabState> list3 = (from x in base.activePlayer.KnownHabs
				where x.faction == GameStateManager.AlienFaction()
				orderby x.displayName
				select x).ToList<TIHabState>();
			num = 0;
			this.alienHabsList.SetListSize<IntelAlienHabListItemController>(list3.Count, false, false);
			using (IEnumerator<object> enumerator = this.alienHabsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__175.<>p__2 == null)
					{
						IntelScreenController.<>o__175.<>p__2 = CallSite<Func<CallSite, object, IntelAlienHabListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAlienHabListItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__175.<>p__2.Target(IntelScreenController.<>o__175.<>p__2, enumerator.Current).UpdateListItem(list3[num++]);
				}
			}
			List<TIRegionAlienEntityState> list4 = new List<TIRegionAlienEntityState>();
			list4.AddRange(from x in GameStateManager.AllAlienEntities()
				where x.VisibleToFaction(base.activePlayer)
				select x);
			num = 0;
			this.alienEarthAssetsList.SetListSize<IntelAlienEarthAssetListItemController>(list4.Count, false, false);
			using (IEnumerator<object> enumerator = this.alienEarthAssetsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__175.<>p__3 == null)
					{
						IntelScreenController.<>o__175.<>p__3 = CallSite<Func<CallSite, object, IntelAlienEarthAssetListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAlienEarthAssetListItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__175.<>p__3.Target(IntelScreenController.<>o__175.<>p__3, enumerator.Current).UpdateListItem(list4[num++]);
				}
			}
			this.UpdateAlienEvents();
		}

		// Token: 0x06005162 RID: 20834 RVA: 0x0023AE24 File Offset: 0x00239024
		public void RefreshTransferTab()
		{
			this.TransferPlanner.targetSelectionTool.SetTargetsToAllOrbitsAndSpaceAssets();
			this.TransferPlanner.targetSelectionTool.UpdateListUI();
		}

		// Token: 0x06005163 RID: 20835 RVA: 0x0023AE48 File Offset: 0x00239048
		public void ShowAllCouncilorTabs()
		{
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__177.<>p__0 == null)
					{
						IntelScreenController.<>o__177.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelFactionGridItemController intelFactionGridItemController = IntelScreenController.<>o__177.<>p__0.Target(IntelScreenController.<>o__177.<>p__0, enumerator.Current);
					intelFactionGridItemController.factionTabbedPaneManager.Toggle(intelFactionGridItemController.councilorTab);
				}
			}
			this.RefreshActiveTab(false);
		}

		// Token: 0x06005164 RID: 20836 RVA: 0x0023AEE8 File Offset: 0x002390E8
		public void ShowAllResourcesTabs()
		{
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__178.<>p__0 == null)
					{
						IntelScreenController.<>o__178.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelFactionGridItemController intelFactionGridItemController = IntelScreenController.<>o__178.<>p__0.Target(IntelScreenController.<>o__178.<>p__0, enumerator.Current);
					intelFactionGridItemController.factionTabbedPaneManager.Toggle(intelFactionGridItemController.resourcesTab);
				}
			}
			this.RefreshActiveTab(false);
		}

		// Token: 0x06005165 RID: 20837 RVA: 0x0023AF88 File Offset: 0x00239188
		public void ShowAllObjectivesTabs()
		{
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__179.<>p__0 == null)
					{
						IntelScreenController.<>o__179.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelFactionGridItemController intelFactionGridItemController = IntelScreenController.<>o__179.<>p__0.Target(IntelScreenController.<>o__179.<>p__0, enumerator.Current);
					intelFactionGridItemController.factionTabbedPaneManager.Toggle(intelFactionGridItemController.objectivesTab);
				}
			}
			this.RefreshActiveTab(false);
		}

		// Token: 0x06005166 RID: 20838 RVA: 0x0023B028 File Offset: 0x00239228
		public void ShowAllRelationsTabs()
		{
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__180.<>p__0 == null)
					{
						IntelScreenController.<>o__180.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelFactionGridItemController intelFactionGridItemController = IntelScreenController.<>o__180.<>p__0.Target(IntelScreenController.<>o__180.<>p__0, enumerator.Current);
					intelFactionGridItemController.factionTabbedPaneManager.Toggle(intelFactionGridItemController.relationsTab);
				}
			}
			this.RefreshActiveTab(false);
		}

		// Token: 0x06005167 RID: 20839 RVA: 0x0023B0C8 File Offset: 0x002392C8
		public void ShowAllProjectsTabs()
		{
			using (IEnumerator<object> enumerator = this.factionsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__181.<>p__0 == null)
					{
						IntelScreenController.<>o__181.<>p__0 = CallSite<Func<CallSite, object, IntelFactionGridItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelFactionGridItemController), typeof(IntelScreenController)));
					}
					IntelFactionGridItemController intelFactionGridItemController = IntelScreenController.<>o__181.<>p__0.Target(IntelScreenController.<>o__181.<>p__0, enumerator.Current);
					intelFactionGridItemController.factionTabbedPaneManager.Toggle(intelFactionGridItemController.projectsTab);
				}
			}
			this.RefreshActiveTab(false);
		}

		// Token: 0x06005168 RID: 20840 RVA: 0x0023B168 File Offset: 0x00239368
		public void UpdateAlienEvents()
		{
			List<NotificationSummaryItem> list = GameStateManager.NotificationQueue().notificationSummaryQueue.Where<NotificationSummaryItem>(delegate(NotificationSummaryItem x)
			{
				if (!x.alienRelated)
				{
					return false;
				}
				List<TIFactionState> summaryLogFactions = x.summaryLogFactions;
				if (summaryLogFactions == null || !summaryLogFactions.Contains(base.activePlayer))
				{
					List<TIFactionState> timerFactions = x.timerFactions;
					return timerFactions != null && timerFactions.Contains(base.activePlayer);
				}
				return true;
			}).Skip<NotificationSummaryItem>(this.alienSkipEventsValue).Take<NotificationSummaryItem>(30)
				.ToList<NotificationSummaryItem>();
			this.alienEventsList.SetListSize<IntelAlienEventListItemController>(list.Count, false, false);
			int num = 0;
			using (IEnumerator<object> enumerator = this.alienEventsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (IntelScreenController.<>o__182.<>p__0 == null)
					{
						IntelScreenController.<>o__182.<>p__0 = CallSite<Func<CallSite, object, IntelAlienEventListItemController>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(IntelAlienEventListItemController), typeof(IntelScreenController)));
					}
					IntelScreenController.<>o__182.<>p__0.Target(IntelScreenController.<>o__182.<>p__0, enumerator.Current).UpdateListItem(list[num++]);
				}
			}
			this.backInTimeButton.interactable = GameStateManager.NotificationQueue().alienEvents > this.alienSkipEventsValue + 30;
			this.forwardInTimeButton.interactable = this.alienSkipEventsValue > 0;
		}

		// Token: 0x06005169 RID: 20841 RVA: 0x0023B27C File Offset: 0x0023947C
		public void AlienEventsBackInTime()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
			this.alienSkipEventsValue += 30;
			this.UpdateAlienEvents();
		}

		// Token: 0x0600516A RID: 20842 RVA: 0x0023B2A0 File Offset: 0x002394A0
		public void AlienEventsForwardInTime()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.alienSkipEventsValue = Mathf.Max(0, this.alienSkipEventsValue - 30);
			this.backInTimeButton.interactable = GameStateManager.NotificationQueue().alienEvents > 30;
			this.UpdateAlienEvents();
		}

		// Token: 0x0600516B RID: 20843 RVA: 0x0023B2ED File Offset: 0x002394ED
		public void OnClickSpaceSortButton(int sortValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.OnChangeSpaceSort(sortValue);
		}

		// Token: 0x0600516C RID: 20844 RVA: 0x0023B304 File Offset: 0x00239504
		public void OnChangeSpaceSort(int sortBy)
		{
			if (sortBy != this.lastSort || sortBy == -1)
			{
				this.sortAscend = false;
				if (sortBy == -1)
				{
					sortBy = this.lastSort;
				}
			}
			this.currentSpaceSort = (SortSpaceDataBy)sortBy;
			this.UpdateSpaceBodySort();
			this.lastSort = sortBy;
			this.sortAscend = !this.sortAscend;
		}

		// Token: 0x0600516D RID: 20845 RVA: 0x0023B354 File Offset: 0x00239554
		public void UpdateSpaceBodySort()
		{
			switch (this.currentSpaceSort)
			{
			case SortSpaceDataBy.Orbit:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.orbitSortWeight).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.orbitSortWeight).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Alfa:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, string>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.displayName).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, string>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.displayName).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Size:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sizeValue).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sizeValue).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Habs:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, int>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.habSites.Length).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, int>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.habSites.Length).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Water:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumWater).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumWater).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Volatiles:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumVolatiles).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumVolatiles).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Metals:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumMetals).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumMetals).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Nobles:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumNobles).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumNobles).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Fertiles:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumFissiles).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, SiteProfileRating>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumFissiles).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Description:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.DescSortWeight).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.DescSortWeight).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.DescriptionMining:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, string>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.GetMiningPotentialString()).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, string>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.GetMiningPotentialString()).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Solar:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, float>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumSolar).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, float>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.sumSolar).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.Tag:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, PlayerTag>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.playerTag).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, PlayerTag>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.spacebodyState.playerTag).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			case SortSpaceDataBy.LaunchWindow:
				if (!this.sortAscend)
				{
					this.spacebodyModels = this.spacebodyModels.OrderByDescending<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.launchWindowSort).ToList<IntelScreenSpacebodyListItemModel>();
				}
				else
				{
					this.spacebodyModels = this.spacebodyModels.OrderBy<IntelScreenSpacebodyListItemModel, double>((IntelScreenSpacebodyListItemModel o) => o.IntelScreenSpacebodyListItemData.launchWindowSort).ToList<IntelScreenSpacebodyListItemModel>();
				}
				break;
			}
			this.UpdateSpaceBodiesListVisibility();
			this.UpdateSpaceBodyListModelData();
		}

		// Token: 0x0600516E RID: 20846 RVA: 0x0023BA74 File Offset: 0x00239C74
		public void OnFactionDropdownChanged(bool spacebody)
		{
			if (spacebody)
			{
				this.spaceBody_filterForFaction = this.factionDropdownLookup[this.factionsDropdowns[0].value];
				this.spaceBody_filterHumanFactionsOnly = this.factionsDropdowns[0].value == 1;
				this.spaceBody_filterNoFactionsOnly = this.factionsDropdowns[0].value == 2;
				this.OnChangeSpaceSort(-1);
				return;
			}
			this.habSite_filterForFaction = this.factionDropdownLookup[this.factionsDropdowns[1].value];
			this.habSite_filterHumanFactionsOnly = this.factionsDropdowns[1].value == 1;
			this.habSite_filterNoFactionsOnly = this.factionsDropdowns[1].value == 2;
			this.OnChangeHabSiteSort(-1);
		}

		// Token: 0x0600516F RID: 20847 RVA: 0x0023BB40 File Offset: 0x00239D40
		public void OnLocationDropdownChanged(bool spacebody)
		{
			if (spacebody)
			{
				List<int> bitIndices = this.locationDropdowns_High[0].value.GetBitIndices();
				this.spaceBody_highFilterForSpaceBody = new List<TISpaceBodyState>();
				foreach (int num in bitIndices)
				{
					this.spaceBody_highFilterForSpaceBody.Add(this.highLocationDropdownLookup[num]);
				}
				this.OnChangeSpaceSort(-1);
				return;
			}
			List<int> bitIndices2 = this.locationDropdowns_High[1].value.GetBitIndices();
			this.habSite_highFilterForSpaceBody = new List<TISpaceBodyState>();
			foreach (int num2 in bitIndices2)
			{
				this.habSite_highFilterForSpaceBody.Add(this.highLocationDropdownLookup[num2]);
			}
			this.OnChangeHabSiteSort(-1);
		}

		// Token: 0x06005170 RID: 20848 RVA: 0x0023BC40 File Offset: 0x00239E40
		public void OnUpdateNameSortFilterToggle(bool spacebody)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.UpdateNameSortFilter(spacebody);
		}

		// Token: 0x06005171 RID: 20849 RVA: 0x0023BC55 File Offset: 0x00239E55
		public void UpdateNameSortFilter(bool spacebody)
		{
			if (spacebody)
			{
				this.spaceBody_nameFilterForSpacebody = this.spaceBody_filterNameInputField.text;
				this.OnChangeSpaceSort(-1);
				return;
			}
			this.habSite_nameFilterForHabSite = this.habSite_filterNameInputField.text;
			this.OnChangeHabSiteSort(-1);
		}

		// Token: 0x06005172 RID: 20850 RVA: 0x0023BC8C File Offset: 0x00239E8C
		public void UpdateSpaceBodiesListVisibility()
		{
			for (int i = 0; i < this.spacebodyModels.Count; i++)
			{
				this.spacebodyModels[i].IntelScreenSpacebodyListItemData.showInList = true;
				bool flag = true;
				IntelScreenSpacebodyListItem_Data intelScreenSpacebodyListItemData = this.spacebodyModels[i].IntelScreenSpacebodyListItemData;
				if (intelScreenSpacebodyListItemData != null)
				{
					if (this.filterProspected.isOn && !base.activePlayer.Prospected(intelScreenSpacebodyListItemData.spacebodyState))
					{
						this.spacebodyModels[i].IntelScreenSpacebodyListItemData.showInList = false;
					}
					else
					{
						if (this.factionsDropdowns[0].value != 0)
						{
							if (this.spaceBody_filterHumanFactionsOnly)
							{
								flag &= intelScreenSpacebodyListItemData.HasHumanHab();
							}
							else if (this.spaceBody_filterNoFactionsOnly)
							{
								flag &= !intelScreenSpacebodyListItemData.HasHab();
							}
							else
							{
								flag &= intelScreenSpacebodyListItemData.HasFactionHab(this.spaceBody_filterForFaction);
							}
							if (!flag)
							{
								this.spacebodyModels[i].IntelScreenSpacebodyListItemData.showInList = false;
								goto IL_02E9;
							}
						}
						if (this.spaceBody_highFilterForSpaceBody != null && this.spaceBody_highFilterForSpaceBody.Count > 0)
						{
							bool flag2 = false;
							using (List<int>.Enumerator enumerator = this.locationDropdowns_High[0].value.GetBitIndices().GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									switch (enumerator.Current)
									{
									case 0:
									case 1:
									case 2:
									case 3:
									case 5:
									case 9:
									case 10:
									case 12:
									case 13:
										flag2 |= this.spaceBody_highFilterForSpaceBody.Contains(intelScreenSpacebodyListItemData.spacebodyState) || (intelScreenSpacebodyListItemData.spacebodyState.isaMoon && this.spaceBody_highFilterForSpaceBody.Contains(intelScreenSpacebodyListItemData.spacebodyState.GetSunOrbitingRelatedObject));
										break;
									case 4:
										flag2 |= intelScreenSpacebodyListItemData.spacebodyState.innerSystemAsteroid(true);
										break;
									case 6:
										flag2 |= intelScreenSpacebodyListItemData.spacebodyState.innerMainBeltAsteroid(true);
										break;
									case 7:
										flag2 |= intelScreenSpacebodyListItemData.spacebodyState.midMainBeltAsteroid(true);
										break;
									case 8:
										flag2 |= intelScreenSpacebodyListItemData.spacebodyState.outerMainBeltAsteroid(true);
										break;
									case 11:
										flag2 |= intelScreenSpacebodyListItemData.spacebodyState.centaur(true);
										break;
									case 14:
										flag2 |= intelScreenSpacebodyListItemData.spacebodyState.kuiperBeltObject(true);
										break;
									default:
										flag2 |= true;
										break;
									}
								}
							}
							flag = flag && flag2;
							if (!flag)
							{
								this.spacebodyModels[i].IntelScreenSpacebodyListItemData.showInList = false;
								goto IL_02E9;
							}
						}
						if (!string.IsNullOrEmpty(this.spaceBody_nameFilterForSpacebody))
						{
							TISpaceBodyState spacebodyState = intelScreenSpacebodyListItemData.spacebodyState;
							flag &= spacebodyState.displayName.ToLowerInvariant().Contains(this.spaceBody_nameFilterForSpacebody.ToLowerInvariant()) || spacebodyState.GetMiningPotentialString().ToLower().Contains(this.spaceBody_nameFilterForSpacebody.ToLower()) || spacebodyState.template.descriptor1.ToLower().Contains(this.spaceBody_nameFilterForSpacebody.ToLower());
						}
						if (!flag)
						{
							this.spacebodyModels[i].IntelScreenSpacebodyListItemData.showInList = false;
						}
					}
				}
				IL_02E9:;
			}
		}

		// Token: 0x06005173 RID: 20851 RVA: 0x0023BFB4 File Offset: 0x0023A1B4
		public void UpdateHabSitesListVisibility()
		{
			for (int i = 0; i < this.habSiteModels.Count; i++)
			{
				bool flag = true;
				this.habSiteModels[i].IntelScreenHabSiteListItemData.showInList = true;
				IntelScreenHabSiteListItem_Data intelScreenHabSiteListItemData = this.habSiteModels[i].IntelScreenHabSiteListItemData;
				if (intelScreenHabSiteListItemData != null)
				{
					if (this.factionsDropdowns[1].value != 0)
					{
						if (this.habSite_filterHumanFactionsOnly)
						{
							flag &= intelScreenHabSiteListItemData.habSiteState.hab != null && !intelScreenHabSiteListItemData.habSiteState.hab.ref_faction.IsAlienFaction;
						}
						else if (this.habSite_filterNoFactionsOnly)
						{
							flag &= intelScreenHabSiteListItemData.habSiteState.hab == null;
						}
						else
						{
							flag &= intelScreenHabSiteListItemData.habSiteState.hab != null && intelScreenHabSiteListItemData.habSiteState.hab.ref_faction.Equals(this.habSite_filterForFaction);
						}
						if (!flag)
						{
							this.habSiteModels[i].IntelScreenHabSiteListItemData.showInList = false;
							goto IL_0372;
						}
					}
					if (this.habSite_highFilterForSpaceBody != null && this.habSite_highFilterForSpaceBody.Count > 0)
					{
						bool flag2 = false;
						using (List<int>.Enumerator enumerator = this.locationDropdowns_High[1].value.GetBitIndices().GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								switch (enumerator.Current)
								{
								case 0:
								case 1:
								case 2:
								case 3:
								case 5:
								case 9:
								case 10:
								case 12:
								case 13:
									flag2 |= this.habSite_highFilterForSpaceBody.Contains(intelScreenHabSiteListItemData.habSiteState.ref_spaceBody) || (intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.isaMoon && this.habSite_highFilterForSpaceBody.Contains(intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.GetSunOrbitingRelatedObject));
									break;
								case 4:
									flag2 |= intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.innerSystemAsteroid(true);
									break;
								case 6:
									flag2 |= intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.innerMainBeltAsteroid(true);
									break;
								case 7:
									flag2 |= intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.midMainBeltAsteroid(true);
									break;
								case 8:
									flag2 |= intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.outerMainBeltAsteroid(true);
									break;
								case 11:
									flag2 |= intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.centaur(true);
									break;
								case 14:
									flag2 |= intelScreenHabSiteListItemData.habSiteState.ref_spaceBody.kuiperBeltObject(true);
									break;
								default:
									flag2 |= true;
									break;
								}
							}
						}
						flag = flag && flag2;
						if (!flag)
						{
							this.habSiteModels[i].IntelScreenHabSiteListItemData.showInList = false;
							goto IL_0372;
						}
					}
					if (!string.IsNullOrEmpty(this.habSite_nameFilterForHabSite))
					{
						TIHabSiteState habSiteState = intelScreenHabSiteListItemData.habSiteState;
						flag &= habSiteState.displayName.ToLower().Contains(this.habSite_nameFilterForHabSite.ToLower()) || habSiteState.miningProfile.description.ToLower().Contains(this.habSite_nameFilterForHabSite.ToLower()) || habSiteState.parentBody.displayName.ToLower().Contains(this.habSite_nameFilterForHabSite.ToLower()) || (habSiteState.hab != null && habSiteState.hab.displayName.ToLower().Contains(this.habSite_nameFilterForHabSite.ToLower()));
					}
					if (!flag)
					{
						this.habSiteModels[i].IntelScreenHabSiteListItemData.showInList = false;
					}
				}
				IL_0372:;
			}
		}

		// Token: 0x06005174 RID: 20852 RVA: 0x0023C364 File Offset: 0x0023A564
		public void OnSelectInputBox()
		{
			TIInputManager.BlockKeybindings();
		}

		// Token: 0x06005175 RID: 20853 RVA: 0x0023C36B File Offset: 0x0023A56B
		public void OnDeSelectInputBox(bool spacebody)
		{
			TIInputManager.RestoreKeybindings();
			if (string.IsNullOrEmpty(spacebody ? this.spaceBody_filterNameInputField.text : this.habSite_filterNameInputField.text))
			{
				this.UpdateNameSortFilter(spacebody);
			}
		}

		// Token: 0x06005176 RID: 20854 RVA: 0x0023C39B File Offset: 0x0023A59B
		public void UpdateProspectedFilter()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.OnChangeSpaceSort(-1);
		}

		// Token: 0x06005177 RID: 20855 RVA: 0x0023C3B0 File Offset: 0x0023A5B0
		public void OnClickHabSortButton(int sortValue)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.OnChangeHabSiteSort(sortValue);
		}

		// Token: 0x06005178 RID: 20856 RVA: 0x0023C3C8 File Offset: 0x0023A5C8
		public void OnChangeHabSiteSort(int sortBy)
		{
			if (sortBy != this.lastSitesSort || sortBy == -1)
			{
				this.sortSitesAscend = false;
				if (sortBy == -1)
				{
					sortBy = this.lastSitesSort;
				}
			}
			else
			{
				this.sortSitesAscend = !this.sortSitesAscend;
			}
			this.currentHabSiteSort = (SortSpaceDataBy)sortBy;
			this.UpdateHabSiteSort();
			this.lastSitesSort = sortBy;
		}

		// Token: 0x06005179 RID: 20857 RVA: 0x0023C41C File Offset: 0x0023A61C
		public void UpdateHabSiteSort()
		{
			switch (this.currentHabSiteSort)
			{
			case SortSpaceDataBy.Orbit:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, double>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.orbitValue).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, double>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.orbitValue).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Alfa:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, string>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.displayName).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, string>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.displayName).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Size:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, double>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.parentBody.meanRadius_km).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, double>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.parentBody.meanRadius_km).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Habs:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, bool>((IntelScreenHabSiteListItemModel o) => string.IsNullOrEmpty(o.IntelScreenHabSiteListItemData.habNameSortString)).ThenByDescending<IntelScreenHabSiteListItemModel, string>(delegate(IntelScreenHabSiteListItemModel o)
					{
						TIHabState hab = o.IntelScreenHabSiteListItemData.habSiteState.hab;
						return ((hab != null) ? hab.displayName : null) ?? "";
					}).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, bool>((IntelScreenHabSiteListItemModel o) => string.IsNullOrEmpty(o.IntelScreenHabSiteListItemData.habNameSortString)).ThenBy<IntelScreenHabSiteListItemModel, string>(delegate(IntelScreenHabSiteListItemModel o)
					{
						TIHabState hab2 = o.IntelScreenHabSiteListItemData.habSiteState.hab;
						return ((hab2 != null) ? hab2.displayName : null) ?? "";
					}).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Water:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Water)).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Water)).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Volatiles:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Volatiles)).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Volatiles)).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Metals:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Metals)).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Metals)).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Nobles:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.NobleMetals)).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.NobleMetals)).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Fertiles:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Fissiles)).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, float>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.GetDailyProduction(FactionResource.Fissiles)).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Description:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, string>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.miningProfile.description).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, string>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.miningProfile.description).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.Tag:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, PlayerTag>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.ref_spaceBody.playerTag).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, PlayerTag>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.habSiteState.ref_spaceBody.playerTag).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			case SortSpaceDataBy.LaunchWindow:
				if (!this.sortSitesAscend)
				{
					this.habSiteModels = this.habSiteModels.OrderByDescending<IntelScreenHabSiteListItemModel, double>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.launchWindowSort).ToList<IntelScreenHabSiteListItemModel>();
				}
				else
				{
					this.habSiteModels = this.habSiteModels.OrderBy<IntelScreenHabSiteListItemModel, double>((IntelScreenHabSiteListItemModel o) => o.IntelScreenHabSiteListItemData.launchWindowSort).ToList<IntelScreenHabSiteListItemModel>();
				}
				break;
			}
			this.UpdateHabSitesListVisibility();
			this.UpdateHabSiteListModelData();
		}

		// Token: 0x0600517A RID: 20858 RVA: 0x0023CA8C File Offset: 0x0023AC8C
		public void UpdateLeaderPopup(TIFactionState faction)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_OpenSmall", false, false);
			this.mediumFactionIcon.sprite = faction.factionIcon128UI;
			this.smallFactionIcon.sprite = faction.factionIcon64UI;
			this.leaderPanelFactionName.SetText(Loc.T("UI.Intel.LeaderBioHeader", new object[] { faction.adjective }));
			this.leaderName.SetText(faction.leaderName);
			this.leaderAddress.SetText(Loc.T("UI.Intel.LeaderAddress", new object[] { faction.leaderAddress }));
			this.leaderBirth.SetText(Loc.T("UI.Intel.LeaderBirth", new object[] { faction.template.leaderBorn }));
			this.leaderJob.SetText(Loc.T("UI.Intel.LeaderJob", new object[] { faction.template.leaderBackground }));
			this.leaderAgenda.SetText(Loc.T("UI.Intel.LeaderAgenda", new object[] { Utilities.Capitalize(faction.goal) }));
			this.leaderBackground.SetText(faction.template.leaderDescription);
			this.leaderQuote.SetText(faction.template.quote);
			if (TIPlayerProfileManager.useCouncilorVideo)
			{
				this.leaderVideo.gameObject.SetActive(true);
				this.leaderBioPortrait.gameObject.SetActive(false);
				VideoClip videoClip = GameControl.assetLoader.LoadAsset<VideoClip>(faction.pathLeaderHeadVideo);
				if (this.leaderVideo.clip != videoClip)
				{
					this.leaderVideo.clip = videoClip;
				}
				if (!this.leaderVideo.isPlaying)
				{
					TIUtilities.TryPlayVideo(this.leaderVideo);
				}
			}
			else
			{
				this.leaderVideo.gameObject.SetActive(false);
				this.leaderBioPortrait.gameObject.SetActive(true);
				this.leaderBioPortrait.sprite = GameControl.assetLoader.LoadAssetForSpriteAssignment(faction.pathLeaderHeadPortrait);
			}
			this.leaderBioDimmerObject.SetActive(true);
			this.leaderBioPanelObject.SetActive(true);
		}

		// Token: 0x0600517B RID: 20859 RVA: 0x0023CC8F File Offset: 0x0023AE8F
		public void OnCloseLeaderPopupClicked()
		{
			this.leaderVideo.Stop();
			this.leaderBioPanelObject.SetActive(false);
			this.leaderBioDimmerObject.SetActive(false);
		}

		// Token: 0x0600517C RID: 20860 RVA: 0x0023CCB4 File Offset: 0x0023AEB4
		public void ShowAlienTabUITutorial()
		{
			this.HideTutorials();
			this.alienTabUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_Aliens, false, true);
		}

		// Token: 0x0600517D RID: 20861 RVA: 0x0023CCCE File Offset: 0x0023AECE
		public void ShowFactionTabUITutorial()
		{
			this.HideTutorials();
			this.factionTabUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_Factions, false, true);
		}

		// Token: 0x0600517E RID: 20862 RVA: 0x0023CCE8 File Offset: 0x0023AEE8
		public void ShowGlobalTabUITutorial()
		{
			this.HideTutorials();
			this.globalTabUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_Global, false, true);
		}

		// Token: 0x0600517F RID: 20863 RVA: 0x0023CD02 File Offset: 0x0023AF02
		public void ShowSpaceTabUITutorial()
		{
			this.HideTutorials();
			this.spaceTabUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_SolarSystem, false, true);
		}

		// Token: 0x06005180 RID: 20864 RVA: 0x0023CD1C File Offset: 0x0023AF1C
		public void ShowTransferPlannerTabUITutorial()
		{
			this.HideTutorials();
			this.transferPlannerUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_TransferPlanner, false, true);
		}

		// Token: 0x06005181 RID: 20865 RVA: 0x0023CD36 File Offset: 0x0023AF36
		public void ShowProspectingTabUITutorial()
		{
			this.HideTutorials();
			this.prospectingUITutorialController.HoldTutorial(CampaignMilestone.UITutorial_IntelScreenCanvas_Prospecting, false, true);
		}

		// Token: 0x06005182 RID: 20866 RVA: 0x0023CD50 File Offset: 0x0023AF50
		private void HideTutorials()
		{
			this.alienTabUITutorialController.HideTutorial();
			this.factionTabUITutorialController.HideTutorial();
			this.globalTabUITutorialController.HideTutorial();
			this.spaceTabUITutorialController.HideTutorial();
			this.transferPlannerUITutorialController.HideTutorial();
			this.prospectingUITutorialController.HideTutorial();
		}

		// Token: 0x06005183 RID: 20867 RVA: 0x0023CDA0 File Offset: 0x0023AFA0
		public void Tutorial_ConfigureExampleTransfer()
		{
			TIGameState tigameState = GameStateManager.IterateByClass<TIOrbitState>(false).AsEnumerable<TIGameState>().First<TIGameState>((TIGameState x) => x.ref_spaceBody.isEarth);
			this.TransferPlanner.originButton.SelectedLocation = tigameState as ITransferTarget;
			TIGameState tigameState2 = GameStateManager.IterateByClass<TIOrbitState>(false).AsEnumerable<TIGameState>().First<TIGameState>((TIGameState x) => x.ref_spaceBody.isLuna);
			this.TransferPlanner.destinationButton.SelectedLocation = tigameState2 as ITransferTarget;
			this.TransferPlanner.accelerationInputField.SetTextWithoutNotify("1000");
			this.TransferPlanner.dvInputField.SetTextWithoutNotify("20");
			this.TransferPlanner.UpdateThrustProfile();
		}

		// Token: 0x06005186 RID: 20870 RVA: 0x0023CEE9 File Offset: 0x0023B0E9
		[CompilerGenerated]
		internal static IComparable <RefreshGlobalTab>g__Evaluate|159_5(IntelAtrocitiesGridItemController gridItem)
		{
			return gridItem.faction.atrocities;
		}

		// Token: 0x0400355B RID: 13659
		public TransferPlanner TransferPlanner;

		// Token: 0x0400355C RID: 13660
		public TMP_Text headerText;

		// Token: 0x0400355D RID: 13661
		public RectTransform primaryPanelTransform;

		// Token: 0x0400355E RID: 13662
		public TabbedPaneController alienTab;

		// Token: 0x0400355F RID: 13663
		public TabbedPaneController factionTab;

		// Token: 0x04003560 RID: 13664
		public TabbedPaneController globalTab;

		// Token: 0x04003561 RID: 13665
		public TabbedPaneController spaceBodyTab;

		// Token: 0x04003562 RID: 13666
		public TabbedPaneController transferTab;

		// Token: 0x04003563 RID: 13667
		public TabbedPaneController habSiteTab;

		// Token: 0x04003564 RID: 13668
		public GameObject sitesTabButtonObject;

		// Token: 0x04003565 RID: 13669
		public UITutorialController alienTabUITutorialController;

		// Token: 0x04003566 RID: 13670
		public UITutorialController factionTabUITutorialController;

		// Token: 0x04003567 RID: 13671
		public UITutorialController globalTabUITutorialController;

		// Token: 0x04003568 RID: 13672
		public UITutorialController spaceTabUITutorialController;

		// Token: 0x04003569 RID: 13673
		public UITutorialController transferPlannerUITutorialController;

		// Token: 0x0400356A RID: 13674
		public UITutorialController prospectingUITutorialController;

		// Token: 0x0400356B RID: 13675
		public ListManagerBase factionsList;

		// Token: 0x0400356C RID: 13676
		public List<IntelFactionGridItemController> factionListControllers;

		// Token: 0x0400356D RID: 13677
		public List<TISpaceBodyState> allSpacebodies = new List<TISpaceBodyState>();

		// Token: 0x0400356E RID: 13678
		public List<IntelScreenSpacebodyListItemModel> spacebodyModels = new List<IntelScreenSpacebodyListItemModel>();

		// Token: 0x0400356F RID: 13679
		public IntelScreenSpacebodyListAdapter spacebodyListAdapter;

		// Token: 0x04003570 RID: 13680
		public List<TIHabSiteState> allHabSites = new List<TIHabSiteState>();

		// Token: 0x04003571 RID: 13681
		public List<IntelScreenHabSiteListItemModel> habSiteModels = new List<IntelScreenHabSiteListItemModel>();

		// Token: 0x04003572 RID: 13682
		public IntelScreenHabSiteListAdapter habSiteListAdapter;

		// Token: 0x04003573 RID: 13683
		public Toggle filterProspected;

		// Token: 0x04003574 RID: 13684
		public List<TMP_Dropdown> factionsDropdowns;

		// Token: 0x04003575 RID: 13685
		public List<TMP_Dropdown> locationDropdowns_High;

		// Token: 0x04003576 RID: 13686
		private Dictionary<int, TIFactionState> factionDropdownLookup;

		// Token: 0x04003577 RID: 13687
		private Dictionary<int, TISpaceBodyState> highLocationDropdownLookup;

		// Token: 0x04003578 RID: 13688
		private TIFactionState spaceBody_filterForFaction;

		// Token: 0x04003579 RID: 13689
		private bool spaceBody_filterHumanFactionsOnly;

		// Token: 0x0400357A RID: 13690
		private bool spaceBody_filterNoFactionsOnly;

		// Token: 0x0400357B RID: 13691
		private List<TISpaceBodyState> spaceBody_highFilterForSpaceBody;

		// Token: 0x0400357C RID: 13692
		public TMP_InputField spaceBody_filterNameInputField;

		// Token: 0x0400357D RID: 13693
		private string spaceBody_nameFilterForSpacebody = "";

		// Token: 0x0400357E RID: 13694
		private TIFactionState habSite_filterForFaction;

		// Token: 0x0400357F RID: 13695
		private bool habSite_filterHumanFactionsOnly;

		// Token: 0x04003580 RID: 13696
		private bool habSite_filterNoFactionsOnly;

		// Token: 0x04003581 RID: 13697
		private List<TISpaceBodyState> habSite_highFilterForSpaceBody;

		// Token: 0x04003582 RID: 13698
		public TMP_InputField habSite_filterNameInputField;

		// Token: 0x04003583 RID: 13699
		private string habSite_nameFilterForHabSite = "";

		// Token: 0x04003584 RID: 13700
		public TMP_Text filterProspectedText;

		// Token: 0x04003585 RID: 13701
		public bool sortAscend;

		// Token: 0x04003586 RID: 13702
		private int lastSort;

		// Token: 0x04003587 RID: 13703
		private SortSpaceDataBy currentSpaceSort;

		// Token: 0x04003588 RID: 13704
		public List<TIHabState> cachedKnownStationsList = new List<TIHabState>();

		// Token: 0x04003589 RID: 13705
		public List<TIHabState> cachedKnownHabsList = new List<TIHabState>();

		// Token: 0x0400358A RID: 13706
		public bool sortSitesAscend;

		// Token: 0x0400358B RID: 13707
		private int lastSitesSort;

		// Token: 0x0400358C RID: 13708
		private SortSpaceDataBy currentHabSiteSort;

		// Token: 0x0400358D RID: 13709
		public TabbedPaneManager tabManager;

		// Token: 0x0400358E RID: 13710
		public TMP_Text alienTabText;

		// Token: 0x0400358F RID: 13711
		public TMP_Text factionTabText;

		// Token: 0x04003590 RID: 13712
		public TMP_Text globalTabText;

		// Token: 0x04003591 RID: 13713
		public TMP_Text spaceBodyTabText;

		// Token: 0x04003592 RID: 13714
		public TMP_Text transferPlannerTabText;

		// Token: 0x04003593 RID: 13715
		public TMP_Text habSiteTabText;

		// Token: 0x04003594 RID: 13716
		public TMP_Text globalPublicOpinionHeader;

		// Token: 0x04003595 RID: 13717
		public TMP_Text globalPublicOpinionBreakdown;

		// Token: 0x04003596 RID: 13718
		public ListManagerBase globalPublicOpinionList;

		// Token: 0x04003597 RID: 13719
		public TMP_Text globalEnvironmentalDamageHeader;

		// Token: 0x04003598 RID: 13720
		public TMP_Text globalEnvironmentalDamage_GTA;

		// Token: 0x04003599 RID: 13721
		public TMP_Text globalEnvironmentalDamage_GSLA;

		// Token: 0x0400359A RID: 13722
		public TMP_Text globalEnvironmentalDamage_MAGDPI;

		// Token: 0x0400359B RID: 13723
		public TMP_Text globalEnvironmentalDamage_ACD;

		// Token: 0x0400359C RID: 13724
		public TMP_Text globalEnvironmentalDamage_ACDC;

		// Token: 0x0400359D RID: 13725
		public TMP_Text globalEnvironmentalDamage_ACDS;

		// Token: 0x0400359E RID: 13726
		public TMP_Text globalEnvironmentalDamage_ACDY;

		// Token: 0x0400359F RID: 13727
		public TMP_Text globalEnvironmentalDamage_AM;

		// Token: 0x040035A0 RID: 13728
		public TMP_Text globalEnvironmentalDamage_AMC;

		// Token: 0x040035A1 RID: 13729
		public TMP_Text globalEnvironmentalDamage_AMS;

		// Token: 0x040035A2 RID: 13730
		public TMP_Text globalEnvironmentalDamage_AMY;

		// Token: 0x040035A3 RID: 13731
		public TMP_Text globalEnvironmentalDamage_ANO;

		// Token: 0x040035A4 RID: 13732
		public TMP_Text globalEnvironmentalDamage_ANOC;

		// Token: 0x040035A5 RID: 13733
		public TMP_Text globalEnvironmentalDamage_ANOS;

		// Token: 0x040035A6 RID: 13734
		public TMP_Text globalEnvironmentalDamage_ANOY;

		// Token: 0x040035A7 RID: 13735
		public TMP_Text globalEnvironmentalDamage_ESA;

		// Token: 0x040035A8 RID: 13736
		public TMP_Text globalEnvironmentalDamage_ESAC;

		// Token: 0x040035A9 RID: 13737
		public TMP_Text globalCommodityPricesHeader;

		// Token: 0x040035AA RID: 13738
		public TMP_Text globalCommodityPricesHeaderDescription;

		// Token: 0x040035AB RID: 13739
		public TMP_Text globalCommodityPricesText_Water;

		// Token: 0x040035AC RID: 13740
		public TMP_Text globalCommodityPricesText_Volatiles;

		// Token: 0x040035AD RID: 13741
		public TMP_Text globalCommodityPricesText_Metals;

		// Token: 0x040035AE RID: 13742
		public TMP_Text globalCommodityPricesText_NobleMetals;

		// Token: 0x040035AF RID: 13743
		public TMP_Text globalCommodityPricesText_Fissiles;

		// Token: 0x040035B0 RID: 13744
		public TMP_Text globalCommodityPricesText_Antimatter;

		// Token: 0x040035B1 RID: 13745
		public TMP_Text globalCommodityPricesText_Exotics;

		// Token: 0x040035B2 RID: 13746
		public TMP_Text globalWarsHeader;

		// Token: 0x040035B3 RID: 13747
		public ListManagerBase globalWarsList;

		// Token: 0x040035B4 RID: 13748
		public List<Image> globalIdeologyPortions;

		// Token: 0x040035B5 RID: 13749
		public TMP_Text globalAtrocitiesHeader;

		// Token: 0x040035B6 RID: 13750
		public ListManagerBase atrocitiesGrid;

		// Token: 0x040035B7 RID: 13751
		public TMP_Text globalDataHeader;

		// Token: 0x040035B8 RID: 13752
		public TMP_Text globalDataData;

		// Token: 0x040035B9 RID: 13753
		public TMP_Text globalData_EarthPop;

		// Token: 0x040035BA RID: 13754
		public TMP_Text globalData_SpacePop;

		// Token: 0x040035BB RID: 13755
		public TMP_Text globalData_GDP;

		// Token: 0x040035BC RID: 13756
		public TMP_Text globalData_PerCapitaGDP;

		// Token: 0x040035BD RID: 13757
		public TooltipTrigger enviroTip;

		// Token: 0x040035BE RID: 13758
		public TMP_Text alienCouncilorsHeaderText;

		// Token: 0x040035BF RID: 13759
		public TMP_Text alienEventsHeaderText;

		// Token: 0x040035C0 RID: 13760
		public TMP_Text alienSitesHeaderText;

		// Token: 0x040035C1 RID: 13761
		public TMP_Text alienFleetsHeaderText;

		// Token: 0x040035C2 RID: 13762
		public TMP_Text alienHabsHeaderText;

		// Token: 0x040035C3 RID: 13763
		[Header("LeaderBioPanel")]
		public GameObject leaderBioDimmerObject;

		// Token: 0x040035C4 RID: 13764
		public GameObject leaderBioPanelObject;

		// Token: 0x040035C5 RID: 13765
		public Image mediumFactionIcon;

		// Token: 0x040035C6 RID: 13766
		public Image smallFactionIcon;

		// Token: 0x040035C7 RID: 13767
		public TMP_Text leaderPanelFactionName;

		// Token: 0x040035C8 RID: 13768
		public TMP_Text leaderName;

		// Token: 0x040035C9 RID: 13769
		public TMP_Text leaderAddress;

		// Token: 0x040035CA RID: 13770
		public TMP_Text leaderBirth;

		// Token: 0x040035CB RID: 13771
		public TMP_Text leaderJob;

		// Token: 0x040035CC RID: 13772
		public TMP_Text leaderAgenda;

		// Token: 0x040035CD RID: 13773
		public TMP_Text leaderBackground;

		// Token: 0x040035CE RID: 13774
		public TMP_Text leaderQuote;

		// Token: 0x040035CF RID: 13775
		public VideoPlayer leaderVideo;

		// Token: 0x040035D0 RID: 13776
		public Image leaderBioPortrait;

		// Token: 0x040035D1 RID: 13777
		[Header("SpacebodyTab")]
		public TMP_Text spacebodyHeaderName;

		// Token: 0x040035D2 RID: 13778
		public TMP_Text spacebodyHeaderDescription;

		// Token: 0x040035D3 RID: 13779
		public TMP_Text spacebodyHeaderMiningDescription;

		// Token: 0x040035D4 RID: 13780
		public TMP_Text spacebodyHeaderOrbit;

		// Token: 0x040035D5 RID: 13781
		public TMP_Text spacebodyHeaderDimensions;

		// Token: 0x040035D6 RID: 13782
		public TMP_Text habSiteHeaderName;

		// Token: 0x040035D7 RID: 13783
		public TMP_Text habSiteHeaderDescription;

		// Token: 0x040035D8 RID: 13784
		public TMP_Text habSiteHeaderSpacebodyName;

		// Token: 0x040035D9 RID: 13785
		public TMP_Text habSiteHeaderHabName;

		// Token: 0x040035DA RID: 13786
		public TMP_Text basesHeaderName;

		// Token: 0x040035DB RID: 13787
		public TMP_Text stationsHeaderName;

		// Token: 0x040035DC RID: 13788
		public TMP_Text spacebodyLaunchWindowHeader;

		// Token: 0x040035DD RID: 13789
		public TMP_Text habSiteLaunchWindowHeader;

		// Token: 0x040035DE RID: 13790
		public TMP_Text spaceBodyTagWindowHeader;

		// Token: 0x040035DF RID: 13791
		public IntelScreenController.OnExitCallback OnExit;

		// Token: 0x040035E0 RID: 13792
		public Button probeAllButton;

		// Token: 0x040035E1 RID: 13793
		public TMP_Text probeAllButtonText;

		// Token: 0x040035E2 RID: 13794
		public TMP_Text probeAllButtonCost;

		// Token: 0x040035E3 RID: 13795
		public ListManagerBase alienCouncilorsList;

		// Token: 0x040035E4 RID: 13796
		public ListManagerBase alienEarthAssetsList;

		// Token: 0x040035E5 RID: 13797
		public ListManagerBase alienEventsList;

		// Token: 0x040035E6 RID: 13798
		public ListManagerBase alienFleetsList;

		// Token: 0x040035E7 RID: 13799
		public ListManagerBase alienHabsList;

		// Token: 0x040035E8 RID: 13800
		private int alienSkipEventsValue;

		// Token: 0x040035E9 RID: 13801
		public Button forwardInTimeButton;

		// Token: 0x040035EA RID: 13802
		public Button backInTimeButton;

		// Token: 0x020010C5 RID: 4293
		// (Invoke) Token: 0x06008531 RID: 34097
		public delegate void OnExitCallback();
	}
}
