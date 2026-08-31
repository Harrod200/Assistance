using System;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200086B RID: 2155
	public class HabListItem : MonoBehaviour
	{
		// Token: 0x06004FF0 RID: 20464 RVA: 0x0022804E File Offset: 0x0022624E
		private void Awake()
		{
			this.Init();
		}

		// Token: 0x06004FF1 RID: 20465 RVA: 0x00228056 File Offset: 0x00226256
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06004FF2 RID: 20466 RVA: 0x00228060 File Offset: 0x00226260
		private void Init()
		{
			if (this.hasInit)
			{
				return;
			}
			this.button.onClick.AddListener(new UnityAction(this.OnSelect));
			this.hasInit = true;
			this.defendedExpiration.SetDelegate("BodyText", () => this.defendedTip());
		}

		// Token: 0x06004FF3 RID: 20467 RVA: 0x002280B8 File Offset: 0x002262B8
		private void AddListeners()
		{
			GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<HabPowerManagementUpdated>(new EventManager.EventDelegate<HabPowerManagementUpdated>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<HabDefendInterestsUpdated>(new EventManager.EventDelegate<HabDefendInterestsUpdated>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.UpdateData), null, this.habState, true, false);
			GameControl.eventManager.AddListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.UpdateData), null, this.habState, true, false);
		}

		// Token: 0x06004FF4 RID: 20468 RVA: 0x0022821C File Offset: 0x0022641C
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<HabPowerManagementUpdated>(new EventManager.EventDelegate<HabPowerManagementUpdated>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyed>(new EventManager.EventDelegate<HabModuleDestroyed>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<HabDefendInterestsUpdated>(new EventManager.EventDelegate<HabDefendInterestsUpdated>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.UpdateData), null);
			GameControl.eventManager.RemoveListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.UpdateData), null);
		}

		// Token: 0x06004FF5 RID: 20469 RVA: 0x00228328 File Offset: 0x00226528
		private void OnSelect()
		{
			HabitatsScreenController habitatsScreenController = this.Previewer as HabitatsScreenController;
			if (habitatsScreenController != null && habitatsScreenController.applyingMassTemplates)
			{
				habitatsScreenController.SetSelectedStatus(this.habState, !habitatsScreenController.selectedHabList.Contains(this.habState), false);
				habitatsScreenController.MassHabTemplateUpdateManagementQuery();
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				return;
			}
			this.Previewer.SelectHabFromMenu(this.habState);
		}

		// Token: 0x06004FF6 RID: 20470 RVA: 0x00228391 File Offset: 0x00226591
		public void SetHabState(TIHabState habState, HabitatsScreenController controller, IHabitatsPreviewer previewer)
		{
			this.RemoveListeners();
			this.habState = habState;
			this.controller = controller;
			this.Previewer = previewer;
			this.AddListeners();
			this.UpdateItem();
		}

		// Token: 0x06004FF7 RID: 20471 RVA: 0x002283BA File Offset: 0x002265BA
		public void SetTextColor(Color color)
		{
			this.habName.color = color;
			this.habLocation.color = color;
		}

		// Token: 0x06004FF8 RID: 20472 RVA: 0x002283D4 File Offset: 0x002265D4
		public void SetHighlight(bool highlight)
		{
			this.button.image.sprite = (highlight ? this.buttonSelectedBackground : this.buttonDefaultBackground);
		}

		// Token: 0x06004FF9 RID: 20473 RVA: 0x002283F7 File Offset: 0x002265F7
		public void UpdateData(HabModuleConstructionStatusChange e)
		{
			this.UpdateItem();
		}

		// Token: 0x06004FFA RID: 20474 RVA: 0x002283FF File Offset: 0x002265FF
		public void UpdateData(HabPowerManagementUpdated e)
		{
			this.UpdateItem();
		}

		// Token: 0x06004FFB RID: 20475 RVA: 0x00228407 File Offset: 0x00226607
		public void UpdateData(HabModuleDestroyed e)
		{
			this.UpdateItem();
		}

		// Token: 0x06004FFC RID: 20476 RVA: 0x0022840F File Offset: 0x0022660F
		public void UpdateData(FleetArrivesAtDestination e)
		{
			this.UpdateItem();
		}

		// Token: 0x06004FFD RID: 20477 RVA: 0x00228417 File Offset: 0x00226617
		public void UpdateData(FleetUndocks e)
		{
			this.UpdateItem();
		}

		// Token: 0x06004FFE RID: 20478 RVA: 0x0022841F File Offset: 0x0022661F
		public void UpdateData(HabDefendInterestsUpdated e)
		{
			this.UpdateItem();
		}

		// Token: 0x06004FFF RID: 20479 RVA: 0x00228427 File Offset: 0x00226627
		public void UpdateData(SectorAssignedToFaction e)
		{
			this.UpdateItem();
		}

		// Token: 0x06005000 RID: 20480 RVA: 0x0022842F File Offset: 0x0022662F
		public void UpdateData(BeginBombardment e)
		{
			this.UpdateItem();
		}

		// Token: 0x06005001 RID: 20481 RVA: 0x00228437 File Offset: 0x00226637
		public void UpdateData(EndBombardment e)
		{
			this.UpdateItem();
		}

		// Token: 0x06005002 RID: 20482 RVA: 0x0022843F File Offset: 0x0022663F
		public void UpdateData(BeginHabAssault e)
		{
			this.UpdateItem();
		}

		// Token: 0x06005003 RID: 20483 RVA: 0x00228447 File Offset: 0x00226647
		public void UpdateData(EndHabAssault e)
		{
			this.UpdateItem();
		}

		// Token: 0x06005004 RID: 20484 RVA: 0x0022844F File Offset: 0x0022664F
		public string defendedTip()
		{
			return Loc.T("UI.Habs.DefendTip", new object[]
			{
				this.habState.displayName,
				this.habState.coreDefendExpiration.ToCustomDateString()
			});
		}

		// Token: 0x06005005 RID: 20485 RVA: 0x00228484 File Offset: 0x00226684
		public void UpdateItem()
		{
			if (this.habState == null || this.habState.deleted || this.habState.archived)
			{
				return;
			}
			base.gameObject.name = this.habState.displayName;
			bool flag = GameControl.control.activePlayer.victoryTemplate.GetConditionBlockingSpaceAssets(GameControl.control.activePlayer).Contains(this.habState);
			StringBuilder stringBuilder = new StringBuilder(this.habState.displayName);
			if (flag)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.victoryItemInlineSpritePath);
			}
			if (this.habState.underAssault || this.habState.underBombardment || (this.habState.IsStation && this.habState.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => !x.faction.permanentAlly(this.habState.faction))))
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.armyBattleInlineSpritePath);
			}
			this.habName.SetText(stringBuilder.ToString());
			this.habLocation.SetText(this.habState.LocationName);
			this.habIcon.sprite = this.habState.icon;
			this.defendedObjectIcon.SetActive(this.habState.coreDefended);
			this.habPowerAlertIcon.SetActive(this.habState.FunctionalModules().Any<TIHabModuleState>((TIHabModuleState x) => !x.powered));
			this.underConstructionIcon.SetActive(this.habState.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.underConstruction));
			GameControl.assetLoader.LoadAssetForImageAssignment(string.Format("icons_2d/ICO_MaxTier{0}", this.habState.tier), this.tierIcon);
			if (this.habState.dockedFleets.Count > 0)
			{
				this.dockedFleetIconObject.SetActive(true);
				this.dockedFleetIcon.sprite = this.habState.dockedFleets[0].icon;
			}
			else
			{
				this.dockedFleetIconObject.SetActive(false);
			}
			if (GameControl.control.activePlayer == this.habState.faction)
			{
				if (this.habState.AvailableSlots().Count > 0 || this.habState.OkayModules().Any<TIHabModuleState>((TIHabModuleState x) => x.CanUpgrade(this.habState.faction)))
				{
					this.availableBuildSlotsObject.SetActive(true);
				}
				else
				{
					this.availableBuildSlotsObject.SetActive(false);
				}
			}
			else
			{
				this.availableBuildSlotsObject.SetActive(false);
			}
			if (this.habState.customHabIconResource != "")
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.habState.customHabIconResource, this.customHabIcon);
				this.customHabIcon.gameObject.SetActive(true);
			}
			else
			{
				this.customHabIcon.gameObject.SetActive(false);
			}
			if (this.habState.IsStation)
			{
				for (int i = 1; i <= 4; i++)
				{
					if (this.habState.sectors[i].active)
					{
						this.SectorIcon[i - 1].enabled = true;
						GameControl.assetLoader.LoadAssetForImageAssignment(this.habState.sectors[i].iconResource, this.SectorIcon[i - 1]);
					}
					else
					{
						this.SectorIcon[i - 1].enabled = false;
					}
				}
			}
			else
			{
				for (int j = 1; j <= 4; j++)
				{
					switch (j)
					{
					case 1:
						this.SectorIcon[0].enabled = this.habState.sectors[4].active;
						if (this.habState.sectors[4].active)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment(this.habState.sectors[4].iconResource, this.SectorIcon[0]);
						}
						break;
					case 2:
						this.SectorIcon[1].enabled = this.habState.sectors[2].active;
						if (this.habState.sectors[2].active)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment(this.habState.sectors[2].iconResource, this.SectorIcon[1]);
						}
						break;
					case 3:
						this.SectorIcon[2].enabled = this.habState.sectors[3].active;
						if (this.habState.sectors[3].active)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment(this.habState.sectors[3].iconResource, this.SectorIcon[2]);
						}
						break;
					case 4:
						this.SectorIcon[3].enabled = this.habState.sectors[1].active;
						if (this.habState.sectors[1].active)
						{
							GameControl.assetLoader.LoadAssetForImageAssignment(this.habState.sectors[1].iconResource, this.SectorIcon[3]);
						}
						break;
					}
				}
			}
			TISpaceBodyState ref_spaceBody = this.habState.ref_spaceBody;
			if (ref_spaceBody != null && ref_spaceBody.isaMoon)
			{
				this.locationSortValue = this.habState.ref_spaceBody.semiMajorAxis_AU + this.habState.ref_spaceBody.barycenter.semiMajorAxis_AU;
			}
			else
			{
				this.locationSortValue = this.habState.ref_naturalSpaceObject.semiMajorAxis_AU;
			}
			this.MCSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.MissionControl, false, false);
			this.WaterSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Water, false, false);
			this.VolatilesSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Volatiles, false, false);
			this.MetalsSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Metals, false, false);
			this.NobleMetalsSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.NobleMetals, false, false);
			this.FissilesSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Fissiles, false, false);
			this.AntimatterSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Antimatter, false, false);
			this.ExoticsSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Exotics, false, false);
			this.ResupplySortValue = this.habState.AllowsResupply(this.habState.coreFaction, false, false);
			this.ShipyardSortValue = this.habState.AllowsShipConstruction(this.habState.coreFaction, false, false);
			this.ConstructionSortValue = this.habState.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.underConstruction);
			this.TierSortValue = this.habState.tier;
			this.PopulationSortValue = this.habState.crew;
			this.PowerSortValue = this.habState.FunctionalModules().Any<TIHabModuleState>((TIHabModuleState x) => !x.powered);
			this.ModuleConstructionSortValue = this.habState.GetModuleConstructionTimeModifier(false, null) < 1f;
			this.MoneySortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Money, false, false);
			this.InfluenceSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Influence, false, false);
			this.OpsSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Operations, false, false);
			this.ResearchSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Research, false, false);
			this.ProjectsSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Projects, false, false);
			this.BoostSortValue = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, FactionResource.Boost, false, false);
			this.SetHighlight(this.controller.selectedHabList.Contains(this.habState));
			this.UpdateHabData();
		}

		// Token: 0x06005006 RID: 20486 RVA: 0x00228CE8 File Offset: 0x00226EE8
		private void UpdateHabData()
		{
			if (!this.habState.IsAlien())
			{
				this.populationText.SetText(this.PopulationSortValue.ToString("N0"));
				int num = 0;
				foreach (FactionResource factionResource in Enums.FactionResources)
				{
					StringBuilder stringBuilder = new StringBuilder();
					float netCurrentMonthlyIncome = this.habState.GetNetCurrentMonthlyIncome(this.habState.coreFaction, factionResource, false, false);
					if (netCurrentMonthlyIncome != 0f)
					{
						stringBuilder.Append(TIUtilities.InlineResourceStr(factionResource)).Append(TIUtilities.FormatSmallNumber(netCurrentMonthlyIncome, 1, 0, true, false));
					}
					else
					{
						stringBuilder.Append(string.Empty);
					}
					if ((factionResource == FactionResource.Exotics && (!GameControl.control.activePlayer.ref_faction.UnlockedExotics || GameControl.control.activePlayer.ref_faction.GetCurrentResourceAmount(FactionResource.Exotics) <= 0f)) || (factionResource == FactionResource.Antimatter && (!GameControl.control.activePlayer.ref_faction.UnlockedAntimatter || GameControl.control.activePlayer.ref_faction.GetDailyIncome(FactionResource.Antimatter, false, false) <= 0f)))
					{
						this.resourcesText[num].SetText(string.Empty);
					}
					else
					{
						this.resourcesText[num].SetText(stringBuilder);
					}
					num++;
				}
				StringBuilder stringBuilder2 = new StringBuilder();
				foreach (TechCategory techCategory in Enums.TechCategories)
				{
					float netTechBonusByFaction = this.habState.GetNetTechBonusByFaction(techCategory, this.habState.coreFaction, false);
					if (netTechBonusByFaction != 0f)
					{
						stringBuilder2.Append(TIGenericTechTemplate.categoryInlineSprite(techCategory)).Append(netTechBonusByFaction.ToPercent("P0")).Append(" ");
					}
				}
				this.techText.SetText(stringBuilder2);
				this.resupplyImage.enabled = this.ResupplySortValue;
				this.shipConstructionImage.enabled = this.ShipyardSortValue;
				this.moduleConstructionTimeImage.enabled = this.ModuleConstructionSortValue;
				this.defenseText.SetText(this.habState.SpaceCombatValue().ToString("N0"));
				this.assaultText.SetText(this.habState.AssaultCombatValue(true).ToString("N0"));
				return;
			}
			this.populationText.SetText(string.Empty);
			for (int j = 0; j < this.resourcesText.Length; j++)
			{
				this.resourcesText[j].SetText(string.Empty);
			}
			this.techText.SetText(string.Empty);
			this.resupplyImage.enabled = false;
			this.shipConstructionImage.enabled = false;
			this.moduleConstructionTimeImage.enabled = false;
			this.defenseText.SetText(string.Empty);
			this.assaultText.SetText(string.Empty);
		}

		// Token: 0x06005007 RID: 20487 RVA: 0x00228FBC File Offset: 0x002271BC
		private void OnDestroy()
		{
			this.RemoveListeners();
			this.RemoveButtonListeners();
		}

		// Token: 0x06005008 RID: 20488 RVA: 0x00228FCA File Offset: 0x002271CA
		private void RemoveButtonListeners()
		{
			this.button.onClick.RemoveListener(new UnityAction(this.OnSelect));
		}

		// Token: 0x04003352 RID: 13138
		public IHabitatsPreviewer Previewer;

		// Token: 0x04003353 RID: 13139
		private bool hasInit;

		// Token: 0x04003354 RID: 13140
		public HabitatsScreenController controller;

		// Token: 0x04003355 RID: 13141
		public Image habIcon;

		// Token: 0x04003356 RID: 13142
		public Image[] SectorIcon = new Image[4];

		// Token: 0x04003357 RID: 13143
		public TMP_Text habName;

		// Token: 0x04003358 RID: 13144
		public TMP_Text habLocation;

		// Token: 0x04003359 RID: 13145
		public Button button;

		// Token: 0x0400335A RID: 13146
		public Sprite buttonDefaultBackground;

		// Token: 0x0400335B RID: 13147
		public Sprite buttonSelectedBackground;

		// Token: 0x0400335C RID: 13148
		public TIHabState habState;

		// Token: 0x0400335D RID: 13149
		public TooltipTrigger defendedExpiration;

		// Token: 0x0400335E RID: 13150
		[Header("Icon Container")]
		public Image tierIcon;

		// Token: 0x0400335F RID: 13151
		public GameObject availableBuildSlotsObject;

		// Token: 0x04003360 RID: 13152
		public GameObject dockedFleetIconObject;

		// Token: 0x04003361 RID: 13153
		public Image dockedFleetIcon;

		// Token: 0x04003362 RID: 13154
		public GameObject habPowerAlertIcon;

		// Token: 0x04003363 RID: 13155
		public GameObject underConstructionIcon;

		// Token: 0x04003364 RID: 13156
		public GameObject defendedObjectIcon;

		// Token: 0x04003365 RID: 13157
		public Image customHabIcon;

		// Token: 0x04003366 RID: 13158
		[Header("Data Container")]
		public TMP_Text populationText;

		// Token: 0x04003367 RID: 13159
		public TMP_Text[] resourcesText;

		// Token: 0x04003368 RID: 13160
		public TMP_Text techText;

		// Token: 0x04003369 RID: 13161
		public Image resupplyImage;

		// Token: 0x0400336A RID: 13162
		public Image shipConstructionImage;

		// Token: 0x0400336B RID: 13163
		public Image moduleConstructionTimeImage;

		// Token: 0x0400336C RID: 13164
		public TMP_Text defenseText;

		// Token: 0x0400336D RID: 13165
		public TMP_Text assaultText;

		// Token: 0x0400336E RID: 13166
		public double locationSortValue;

		// Token: 0x0400336F RID: 13167
		public float MCSortValue;

		// Token: 0x04003370 RID: 13168
		public float WaterSortValue;

		// Token: 0x04003371 RID: 13169
		public float VolatilesSortValue;

		// Token: 0x04003372 RID: 13170
		public float MetalsSortValue;

		// Token: 0x04003373 RID: 13171
		public float NobleMetalsSortValue;

		// Token: 0x04003374 RID: 13172
		public float FissilesSortValue;

		// Token: 0x04003375 RID: 13173
		public float AntimatterSortValue;

		// Token: 0x04003376 RID: 13174
		public float ExoticsSortValue;

		// Token: 0x04003377 RID: 13175
		public bool ResupplySortValue;

		// Token: 0x04003378 RID: 13176
		public bool ShipyardSortValue;

		// Token: 0x04003379 RID: 13177
		public bool ConstructionSortValue;

		// Token: 0x0400337A RID: 13178
		public int TierSortValue;

		// Token: 0x0400337B RID: 13179
		public int PopulationSortValue;

		// Token: 0x0400337C RID: 13180
		public bool PowerSortValue;

		// Token: 0x0400337D RID: 13181
		public bool ModuleConstructionSortValue;

		// Token: 0x0400337E RID: 13182
		public float MoneySortValue;

		// Token: 0x0400337F RID: 13183
		public float InfluenceSortValue;

		// Token: 0x04003380 RID: 13184
		public float OpsSortValue;

		// Token: 0x04003381 RID: 13185
		public float ResearchSortValue;

		// Token: 0x04003382 RID: 13186
		public float ProjectsSortValue;

		// Token: 0x04003383 RID: 13187
		public float BoostSortValue;
	}
}
