using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200085D RID: 2141
	public class FinderListItemController : MonoBehaviour
	{
		// Token: 0x06004E8C RID: 20108 RVA: 0x0021CD7C File Offset: 0x0021AF7C
		public void Initialize(TIGameState item, GeneralControlsController controller)
		{
			base.name = new StringBuilder(item.displayName).Append(" Finder List Item").ToString();
			this.controller = controller;
			this.RemoveListeners(true);
			if (!TIGameState.Valid(item))
			{
				return;
			}
			this.army = null;
			this.councilor = null;
			this.fleet = null;
			this.hab = null;
			this.gamestate = item;
			if (item.isCouncilorState)
			{
				this.councilor = item.ref_councilor;
				this.itemType = FinderListItemType.Councilor;
				this.itemBackground.enabled = true;
				this.itemLocation.enabled = true;
				this.AddCouncilorListeners();
				this.UpdateListItem(this.councilor, false);
				return;
			}
			if (item.isArmyState)
			{
				this.army = item.ref_army;
				this.itemType = FinderListItemType.Army;
				this.itemIcon.enabled = true;
				this.itemBackground.enabled = true;
				this.itemLocation.enabled = true;
				this.statusIcon.enabled = false;
				this.AddArmyListeners();
				this.UpdateListItem(this.army);
				return;
			}
			if (item.isSpaceFleetState)
			{
				this.fleet = item.ref_fleet;
				this.itemType = FinderListItemType.Fleet;
				this.itemIcon.enabled = true;
				this.itemLocation.enabled = true;
				this.itemBackground.enabled = false;
				this.statusIcon.enabled = false;
				this.itemIcon.sprite = this.fleet.icon;
				this.AddFleetListeners();
				this.UpdateListItem(this.fleet);
				return;
			}
			if (item.isHabState)
			{
				this.hab = item.ref_hab;
				this.itemType = FinderListItemType.Hab;
				this.itemIcon.enabled = true;
				this.itemLocation.enabled = true;
				this.itemBackground.enabled = false;
				this.itemLocation.sprite = this.hab.ref_naturalSpaceObject.icon;
				this.AddHabListeners();
				this.UpdateListItem(this.hab);
			}
		}

		// Token: 0x06004E8D RID: 20109 RVA: 0x0021CF68 File Offset: 0x0021B168
		public void AddCouncilorListeners()
		{
			if (this.councilor != null)
			{
				GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.councilor, false, true);
				GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateCouncilorListItem), null, this.councilor, true, false);
				GameControl.eventManager.AddListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateCouncilorListItem), null, this.councilor, true, false);
				GameControl.eventManager.AddListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateCouncilorListItem), null, this.councilor, true, false);
				GameControl.eventManager.AddListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateCouncilorListItem), null, this.councilor, true, false);
			}
		}

		// Token: 0x06004E8E RID: 20110 RVA: 0x0021D024 File Offset: 0x0021B224
		public void RemoveCouncilorListeners(bool noNullCheck = true)
		{
			if (this.councilor != null || noNullCheck)
			{
				GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
				GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateCouncilorListItem), null);
				GameControl.eventManager.RemoveListener<CouncilorMissionUpdated>(new EventManager.EventDelegate<CouncilorMissionUpdated>(this.UpdateCouncilorListItem), null);
				GameControl.eventManager.RemoveListener<CouncilorVisibilityChanged>(new EventManager.EventDelegate<CouncilorVisibilityChanged>(this.UpdateCouncilorListItem), null);
				GameControl.eventManager.RemoveListener<CouncilorValuesChanged>(new EventManager.EventDelegate<CouncilorValuesChanged>(this.UpdateCouncilorListItem), null);
			}
		}

		// Token: 0x06004E8F RID: 20111 RVA: 0x0021D0B4 File Offset: 0x0021B2B4
		private void UpdateCouncilorListItem(CouncilorPositionUpdated e)
		{
			this.UpdateListItem(this.councilor, false);
		}

		// Token: 0x06004E90 RID: 20112 RVA: 0x0021D0C3 File Offset: 0x0021B2C3
		private void UpdateCouncilorListItem(CouncilorMissionUpdated e)
		{
			this.UpdateListItem(this.councilor, false);
		}

		// Token: 0x06004E91 RID: 20113 RVA: 0x0021D0D2 File Offset: 0x0021B2D2
		private void UpdateCouncilorListItem(CouncilorVisibilityChanged e)
		{
			this.UpdateListItem(this.councilor, false);
		}

		// Token: 0x06004E92 RID: 20114 RVA: 0x0021D0E1 File Offset: 0x0021B2E1
		private void UpdateCouncilorListItem(CouncilorValuesChanged e)
		{
			this.UpdateListItem(this.councilor, true);
		}

		// Token: 0x06004E93 RID: 20115 RVA: 0x0021D0F0 File Offset: 0x0021B2F0
		public void AddArmyListeners()
		{
			if (this.army != null)
			{
				GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.army, false, true);
				GameControl.eventManager.AddListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.UpdateArmyListItem), this.army.armyStatusUpdateEventName, this.army, true, false);
				GameControl.eventManager.AddListener<StartArmyOperation>(new EventManager.EventDelegate<StartArmyOperation>(this.UpdateArmyListItem), null, this.army, true, false);
				GameControl.eventManager.AddListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.UpdateArmyListItem), null, this.army, true, false);
			}
		}

		// Token: 0x06004E94 RID: 20116 RVA: 0x0021D194 File Offset: 0x0021B394
		public void RemoveArmyListeners(bool noNullCheck = true)
		{
			if (this.army != null)
			{
				GameControl.eventManager.RemoveListener<ArmyStatusUpdate>(new EventManager.EventDelegate<ArmyStatusUpdate>(this.UpdateArmyListItem), this.army.armyStatusUpdateEventName);
			}
			if (this.army != null || noNullCheck)
			{
				GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
				GameControl.eventManager.RemoveListener<StartArmyOperation>(new EventManager.EventDelegate<StartArmyOperation>(this.UpdateArmyListItem), null);
				GameControl.eventManager.RemoveListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.UpdateArmyListItem), null);
			}
		}

		// Token: 0x06004E95 RID: 20117 RVA: 0x0021D225 File Offset: 0x0021B425
		private void UpdateArmyListItem(ArmyStatusUpdate e)
		{
			this.UpdateListItem(this.army);
		}

		// Token: 0x06004E96 RID: 20118 RVA: 0x0021D233 File Offset: 0x0021B433
		private void UpdateArmyListItem(StartArmyOperation e)
		{
			this.UpdateListItem(this.army);
		}

		// Token: 0x06004E97 RID: 20119 RVA: 0x0021D241 File Offset: 0x0021B441
		private void UpdateArmyListItem(OperationExecuted e)
		{
			this.UpdateListItem(this.army);
		}

		// Token: 0x06004E98 RID: 20120 RVA: 0x0021D24F File Offset: 0x0021B44F
		private void OnGameStateArchived(GameStateArchived e)
		{
			this.RemoveListeners(true);
			base.gameObject.SetActive(false);
			this.controller.UpdateFinderList();
		}

		// Token: 0x06004E99 RID: 20121 RVA: 0x0021D270 File Offset: 0x0021B470
		public void AddHabListeners()
		{
			if (this.hab != null)
			{
				GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.hab, false, true);
				GameControl.eventManager.AddListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabModuleConstructionStatusChange), null, this.hab, true, false);
				GameControl.eventManager.AddListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnSectorAssignedToFaction), null, this.hab, true, false);
				if (this.hab.IsBase)
				{
					GameControl.eventManager.AddListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnBeginBombardment), null, this.hab, false, false);
					GameControl.eventManager.AddListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnEndBombardment), null, this.hab, false, false);
				}
				GameControl.eventManager.AddListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnBeginAssault), null, this.hab, false, false);
				GameControl.eventManager.AddListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnEndAssault), null, this.hab, false, false);
			}
		}

		// Token: 0x06004E9A RID: 20122 RVA: 0x0021D374 File Offset: 0x0021B574
		public void RemoveHabListeners(bool noNullCheck = true)
		{
			if (this.hab != null || noNullCheck)
			{
				GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
				GameControl.eventManager.RemoveListener<HabModuleConstructionStatusChange>(new EventManager.EventDelegate<HabModuleConstructionStatusChange>(this.OnHabModuleConstructionStatusChange), null);
				GameControl.eventManager.RemoveListener<SectorAssignedToFaction>(new EventManager.EventDelegate<SectorAssignedToFaction>(this.OnSectorAssignedToFaction), null);
				GameControl.eventManager.RemoveListener<BeginBombardment>(new EventManager.EventDelegate<BeginBombardment>(this.OnBeginBombardment), null);
				GameControl.eventManager.RemoveListener<EndBombardment>(new EventManager.EventDelegate<EndBombardment>(this.OnEndBombardment), null);
				GameControl.eventManager.RemoveListener<BeginHabAssault>(new EventManager.EventDelegate<BeginHabAssault>(this.OnBeginAssault), null);
				GameControl.eventManager.RemoveListener<EndHabAssault>(new EventManager.EventDelegate<EndHabAssault>(this.OnEndAssault), null);
			}
		}

		// Token: 0x06004E9B RID: 20123 RVA: 0x0021D438 File Offset: 0x0021B638
		public void AddFleetListeners()
		{
			if (this.fleet != null)
			{
				GameControl.eventManager.AddListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null, this.fleet, false, true);
				GameControl.eventManager.AddListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateFleetListItem), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.UpdateFleetListItem), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.UpdateFleetListItem), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.UpdateFleetListItem), null, this.fleet, true, false);
				GameControl.eventManager.AddListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.UpdateFleetListItem), null, this.fleet, true, false);
			}
		}

		// Token: 0x06004E9C RID: 20124 RVA: 0x0021D510 File Offset: 0x0021B710
		public void RemoveFleetListeners(bool noNullCheck = true)
		{
			if (this.fleet != null || noNullCheck)
			{
				GameControl.eventManager.RemoveListener<GameStateArchived>(new EventManager.EventDelegate<GameStateArchived>(this.OnGameStateArchived), null);
				GameControl.eventManager.RemoveListener<FleetArrivesAtDestination>(new EventManager.EventDelegate<FleetArrivesAtDestination>(this.UpdateFleetListItem), null);
				GameControl.eventManager.RemoveListener<StartFleetOperation>(new EventManager.EventDelegate<StartFleetOperation>(this.UpdateFleetListItem), null);
				GameControl.eventManager.RemoveListener<OperationExecuted>(new EventManager.EventDelegate<OperationExecuted>(this.UpdateFleetListItem), null);
				GameControl.eventManager.RemoveListener<FleetUndocks>(new EventManager.EventDelegate<FleetUndocks>(this.UpdateFleetListItem), null);
				GameControl.eventManager.RemoveListener<FleetAvailabilityChange>(new EventManager.EventDelegate<FleetAvailabilityChange>(this.UpdateFleetListItem), null);
			}
		}

		// Token: 0x06004E9D RID: 20125 RVA: 0x0021D5BA File Offset: 0x0021B7BA
		private void UpdateFleetListItem(FleetArrivesAtDestination e)
		{
			this.UpdateListItem(this.fleet);
		}

		// Token: 0x06004E9E RID: 20126 RVA: 0x0021D5C8 File Offset: 0x0021B7C8
		private void UpdateFleetListItem(StartFleetOperation e)
		{
			this.UpdateListItem(this.fleet);
		}

		// Token: 0x06004E9F RID: 20127 RVA: 0x0021D5D6 File Offset: 0x0021B7D6
		private void UpdateFleetListItem(FleetAvailabilityChange e)
		{
			this.UpdateListItem(this.fleet);
		}

		// Token: 0x06004EA0 RID: 20128 RVA: 0x0021D5E4 File Offset: 0x0021B7E4
		private void UpdateFleetListItem(OperationExecuted e)
		{
			if (this.fleet != null && this.fleet.faction == this.controller.activePlayer)
			{
				this.UpdateListItem(this.fleet);
				return;
			}
			this.RemoveFleetListeners(true);
		}

		// Token: 0x06004EA1 RID: 20129 RVA: 0x0021D630 File Offset: 0x0021B830
		private void UpdateFleetListItem(FleetUndocks e)
		{
			this.UpdateListItem(this.fleet);
		}

		// Token: 0x06004EA2 RID: 20130 RVA: 0x0021D63E File Offset: 0x0021B83E
		public void RemoveListeners(bool noNullCheck)
		{
			this.RemoveCouncilorListeners(noNullCheck);
			this.RemoveHabListeners(noNullCheck);
			this.RemoveFleetListeners(noNullCheck);
			this.RemoveArmyListeners(noNullCheck);
		}

		// Token: 0x06004EA3 RID: 20131 RVA: 0x0021D65C File Offset: 0x0021B85C
		public void SelectFinderObject()
		{
			switch (this.itemType)
			{
			case FinderListItemType.Councilor:
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyCouncilorSelect", false, false);
				TIUtilities.GotoGameState(this.councilor, false, true, true);
				return;
			case FinderListItemType.Fleet:
				TIUtilities.GotoGameState(this.fleet, false, true, true, true, false, -1f);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyFleetSelect", false, false);
				return;
			case FinderListItemType.Army:
				SoundEffectController.PlaySelectSound(this.army);
				TIUtilities.GotoGameState(this.army, false, true, true, true, false, -1f);
				return;
			case FinderListItemType.Hab:
				TIUtilities.GotoGameState(this.hab, false, true, true, true, false, -1f);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_MyHabSelect", false, false);
				return;
			default:
				return;
			}
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x0021D708 File Offset: 0x0021B908
		public void UpdateListItem(FinderListItem_Data data)
		{
			if (base.gameObject != null)
			{
				this.editModeCanvas.SetActive(data.showEditMode);
				switch (this.itemType)
				{
				case FinderListItemType.Councilor:
					this.UpdateListItem(this.councilor, false);
					return;
				case FinderListItemType.Fleet:
					this.UpdateListItem(this.fleet);
					return;
				case FinderListItemType.Army:
					this.UpdateListItem(this.army);
					return;
				case FinderListItemType.Hab:
					this.UpdateListItem(this.hab);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06004EA5 RID: 20133 RVA: 0x0021D788 File Offset: 0x0021B988
		private void UpdateListItem(TIArmyState army)
		{
			if (army == null || army.archived || army.destroyed || army.faction != this.controller.activePlayer || this.statusIcon == null)
			{
				return;
			}
			this.itemName.SetText(Loc.T("UI.GeneralControls.FinderArmyLine", new object[]
			{
				(army.deploymentType == DeploymentType.Naval) ? new StringBuilder(army.displayName).Append(TemplateManager.global.navyInlineSpritePath).ToString() : army.displayName,
				army.strength.ToPercent("P0")
			}));
			this.itemIcon.sprite = army.GetForegroundIcon();
			this.itemBackground.sprite = army.GetIconBackgroundSprite;
			this.itemBackground.color = army.faction.template.color;
			this.itemLocation.sprite = army.currentRegion.nation.flag;
			List<OperationData> list = army.CurrentOperations();
			if (list != null && list.Count > 0)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(army.CurrentOperations()[0].operation.GetOperationIconImagePath_Off(), this.statusIcon);
				this.statusIcon.enabled = true;
				return;
			}
			if (army.InBattleWithArmiesOrRegionDefenses())
			{
				this.statusIcon.sprite = AssetCacheManager.armyCombatIcon;
				this.statusIcon.enabled = true;
				return;
			}
			if (army.huntingXenofauna)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("operations/ICO_SetHuntXenoformingOperation_off", this.statusIcon);
				this.statusIcon.enabled = true;
				return;
			}
			this.statusIcon.enabled = false;
		}

		// Token: 0x06004EA6 RID: 20134 RVA: 0x0021D934 File Offset: 0x0021BB34
		private void UpdateListItem(TICouncilorState councilor, bool forceUpdate = false)
		{
			if (councilor == null || councilor.faction == null || councilor.status == CouncilorStatus.Dead || !TIGameState.Valid(councilor))
			{
				return;
			}
			this.itemName.SetText(councilor.displayName);
			this.itemIcon.sprite = councilor.GetIcon(forceUpdate);
			GameControl.assetLoader.LoadAssetForImageAssignment(councilor.iconBackground, this.itemBackground);
			this.itemBackground.color = councilor.faction.template.color;
			TIGameState tigameState = TIMissionPhaseState.CouncilorLastKnownLocation(GameControl.control.activePlayer, councilor);
			if (tigameState.isRegionState)
			{
				this.itemLocation.sprite = tigameState.ref_nation.flag;
			}
			else if (tigameState.ref_spaceBody != null)
			{
				this.itemLocation.sprite = tigameState.ref_spaceBody.icon;
			}
			else if (tigameState.ref_fleet != null && !tigameState.ref_fleet.archived)
			{
				this.itemLocation.sprite = tigameState.ref_fleet.icon;
			}
			if (councilor.faction == this.controller.activePlayer && councilor.HasMission)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(councilor.GetCurrentMissionIcon(false), this.statusIcon);
				this.statusIcon.enabled = true;
				return;
			}
			if (this.statusIcon != null)
			{
				this.statusIcon.enabled = false;
			}
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x0021DAA4 File Offset: 0x0021BCA4
		private void OnHabModuleConstructionStatusChange(HabModuleConstructionStatusChange e)
		{
			this.UpdateListItem(this.hab);
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x0021DAB2 File Offset: 0x0021BCB2
		private void OnSectorAssignedToFaction(SectorAssignedToFaction e)
		{
			if (e.sector.coreSector)
			{
				this.UpdateListItem(this.hab);
			}
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x0021DACD File Offset: 0x0021BCCD
		private void OnBeginBombardment(BeginBombardment e)
		{
			this.statusIcon.sprite = AssetCacheManager.armyCombatIcon;
			this.statusIcon.enabled = true;
		}

		// Token: 0x06004EAA RID: 20138 RVA: 0x0021DAEB File Offset: 0x0021BCEB
		private void OnEndBombardment(EndBombardment e)
		{
			if (!this.hab.deleted && this.hab != null && !this.hab.archived)
			{
				this.UpdateListItem(this.hab);
			}
		}

		// Token: 0x06004EAB RID: 20139 RVA: 0x0021DB21 File Offset: 0x0021BD21
		private void OnBeginAssault(BeginHabAssault e)
		{
			this.statusIcon.sprite = AssetCacheManager.armyCombatIcon;
			this.statusIcon.enabled = true;
		}

		// Token: 0x06004EAC RID: 20140 RVA: 0x0021DB40 File Offset: 0x0021BD40
		private void OnEndAssault(EndHabAssault e)
		{
			if (!this.hab.deleted && this.hab != null && !this.hab.archived && this.hab.faction == GameControl.control.activePlayer)
			{
				this.UpdateListItem(this.hab);
			}
		}

		// Token: 0x06004EAD RID: 20141 RVA: 0x0021DBA0 File Offset: 0x0021BDA0
		private void UpdateListItem(TIHabState hab)
		{
			if (!TIGameState.Valid(hab) || hab.archived || this.statusIcon == null)
			{
				return;
			}
			this.itemName.text = hab.displayName;
			if (!string.IsNullOrEmpty(hab.customHabIconResource))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(hab.customHabIconResource, this.itemIcon);
			}
			else
			{
				this.itemIcon.sprite = hab.icon;
			}
			if (hab.underAssault || (hab.IsBase && hab.underBombardment) || (hab.IsStation && hab.dockedFleets.Any<TISpaceFleetState>((TISpaceFleetState x) => x.faction != hab.faction)))
			{
				this.statusIcon.sprite = AssetCacheManager.armyCombatIcon;
				this.statusIcon.enabled = true;
				return;
			}
			if (hab.activeSectors.Any<TISectorState>((TISectorState x) => x.habModules.Any<TIHabModuleState>((TIHabModuleState y) => y.underConstruction)))
			{
				this.statusIcon.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathUnderConstructionIcon, this.statusIcon);
				return;
			}
			if (hab.activeSectors.Any<TISectorState>((TISectorState x) => x.habModules.Any<TIHabModuleState>((TIHabModuleState y) => y.empty || y.destroyed)))
			{
				this.statusIcon.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment("icons_2d/ICO_new_module_upgrade", this.statusIcon);
				return;
			}
			if (hab.activeSectors.Any<TISectorState>((TISectorState x) => x.habModules.Any<TIHabModuleState>((TIHabModuleState y) => !y.powered)))
			{
				this.statusIcon.enabled = true;
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathHabPowerAlertIcon, this.statusIcon);
				return;
			}
			this.statusIcon.enabled = false;
		}

		// Token: 0x06004EAE RID: 20142 RVA: 0x0021DDBC File Offset: 0x0021BFBC
		public void ForceHighlight(bool highlight = true)
		{
			finderHighlightToggle component = base.GetComponent<finderHighlightToggle>();
			if (highlight)
			{
				ColorBlock colors = this.finderButton.colors;
				colors.normalColor = this.finderButton.colors.highlightedColor;
				this.finderButton.colors = colors;
				component.highlightTimer = 2f;
				component.enabled = true;
				return;
			}
			ColorBlock colors2 = this.finderButton.colors;
			colors2.normalColor = this.finderButton.colors.disabledColor;
			this.finderButton.colors = colors2;
			component.enabled = false;
		}

		// Token: 0x06004EAF RID: 20143 RVA: 0x0021DE54 File Offset: 0x0021C054
		private void UpdateListItem(TISpaceFleetState fleet)
		{
			if (!TIGameState.Valid(fleet) || fleet.dummyFleet || fleet.archived || fleet.ships.Count == 0 || this.statusIcon == null)
			{
				return;
			}
			this.itemName.SetText(fleet.GetDisplayName(fleet.faction));
			if (!fleet.transferAssigned)
			{
				this.itemLocation.sprite = fleet.barycenter.icon;
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(TemplateManager.global.pathFleetInTransitIcon, this.itemLocation);
			}
			if (fleet.dockedOrLanded)
			{
				this.statusIcon.sprite = (fleet.dockedLocation.isHabState ? fleet.dockedLocation.ref_hab.icon : fleet.dockedLocation.ref_habSite.parentBody.icon);
				this.statusIcon.enabled = true;
				return;
			}
			List<OperationData> list = fleet.CurrentOperations();
			if (list != null && list.Count > 0)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(fleet.CurrentOperations()[0].operation.GetOperationIconImagePath_Off(), this.statusIcon);
				this.statusIcon.enabled = true;
				return;
			}
			if (fleet.unavailableForOperations)
			{
				this.statusIcon.sprite = AssetCacheManager.spaceCombatIcon;
				this.statusIcon.enabled = true;
				return;
			}
			if (fleet.huntingXenofauna)
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("operations/ICO_SetContinuousBombardXenoformingOperation_off", this.statusIcon);
				this.statusIcon.enabled = true;
				return;
			}
			this.statusIcon.enabled = false;
		}

		// Token: 0x06004EB0 RID: 20144 RVA: 0x0021DFE0 File Offset: 0x0021C1E0
		public void OnClickChangeSortValue(int value)
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			int finderSortOverride = this.gamestate.finderSortOverride;
			this.gamestate.finderSortOverride += value;
			this.SwapSortIndex(finderSortOverride);
			this.controller.UpdateFinderList();
		}

		// Token: 0x06004EB1 RID: 20145 RVA: 0x0021E02C File Offset: 0x0021C22C
		public void OnClickSetMaxSortValue()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			int finderSortOverride = this.gamestate.finderSortOverride;
			this.gamestate.finderSortOverride = 0;
			this.PushSortIndex(true, finderSortOverride);
			this.controller.UpdateFinderList();
		}

		// Token: 0x06004EB2 RID: 20146 RVA: 0x0021E070 File Offset: 0x0021C270
		public void OnClickSetMinSortValue()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			int finderSortOverride = this.gamestate.finderSortOverride;
			int num = this.SetToMaxSortIndex(finderSortOverride);
			if (num > finderSortOverride)
			{
				this.gamestate.finderSortOverride = num;
				this.PushSortIndex(false, finderSortOverride);
			}
			this.controller.UpdateFinderList();
		}

		// Token: 0x06004EB3 RID: 20147 RVA: 0x0021E0C0 File Offset: 0x0021C2C0
		public void SwapSortIndex(int originalIndex)
		{
			foreach (TIGameState tigameState in this.controller.FinderItems(false))
			{
				if (this.gamestate.isCouncilorState && tigameState.isCouncilorState && this.gamestate.ID != tigameState.ID && this.gamestate.finderSortOverride == tigameState.finderSortOverride)
				{
					tigameState.finderSortOverride = originalIndex;
					return;
				}
				if (this.gamestate.isArmyState && tigameState.isArmyState && this.gamestate.ID != tigameState.ID && this.gamestate.finderSortOverride == tigameState.finderSortOverride)
				{
					tigameState.finderSortOverride = originalIndex;
					return;
				}
				if (this.gamestate.isHabState && tigameState.isHabState && this.gamestate.ID != tigameState.ID && this.gamestate.finderSortOverride == tigameState.finderSortOverride)
				{
					tigameState.finderSortOverride = originalIndex;
					return;
				}
				if (this.gamestate.isSpaceFleetState && tigameState.isSpaceFleetState && this.gamestate.ID != tigameState.ID && this.gamestate.finderSortOverride == tigameState.finderSortOverride)
				{
					tigameState.finderSortOverride = originalIndex;
					return;
				}
			}
			this.gamestate.finderSortOverride = originalIndex;
		}

		// Token: 0x06004EB4 RID: 20148 RVA: 0x0021E25C File Offset: 0x0021C45C
		public void PushSortIndex(bool up, int originalIndex)
		{
			foreach (TIGameState tigameState in this.controller.FinderItems(false))
			{
				if (up)
				{
					if (this.gamestate.isCouncilorState && tigameState.isCouncilorState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride < originalIndex)
					{
						TIGameState tigameState2 = tigameState;
						int num = tigameState2.finderSortOverride;
						tigameState2.finderSortOverride = num + 1;
					}
					if (this.gamestate.isArmyState && tigameState.isArmyState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride < originalIndex)
					{
						TIGameState tigameState3 = tigameState;
						int num = tigameState3.finderSortOverride;
						tigameState3.finderSortOverride = num + 1;
					}
					if (this.gamestate.isHabState && tigameState.isHabState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride < originalIndex)
					{
						TIGameState tigameState4 = tigameState;
						int num = tigameState4.finderSortOverride;
						tigameState4.finderSortOverride = num + 1;
					}
					if (this.gamestate.isSpaceFleetState && tigameState.isSpaceFleetState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride < originalIndex)
					{
						TIGameState tigameState5 = tigameState;
						int num = tigameState5.finderSortOverride;
						tigameState5.finderSortOverride = num + 1;
					}
				}
				else
				{
					if (this.gamestate.isCouncilorState && tigameState.isCouncilorState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > originalIndex)
					{
						TIGameState tigameState6 = tigameState;
						int num = tigameState6.finderSortOverride;
						tigameState6.finderSortOverride = num - 1;
					}
					if (this.gamestate.isArmyState && tigameState.isArmyState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > originalIndex)
					{
						TIGameState tigameState7 = tigameState;
						int num = tigameState7.finderSortOverride;
						tigameState7.finderSortOverride = num - 1;
					}
					if (this.gamestate.isHabState && tigameState.isHabState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > originalIndex)
					{
						TIGameState tigameState8 = tigameState;
						int num = tigameState8.finderSortOverride;
						tigameState8.finderSortOverride = num - 1;
					}
					if (this.gamestate.isSpaceFleetState && tigameState.isSpaceFleetState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > originalIndex)
					{
						TIGameState tigameState9 = tigameState;
						int num = tigameState9.finderSortOverride;
						tigameState9.finderSortOverride = num - 1;
					}
				}
			}
		}

		// Token: 0x06004EB5 RID: 20149 RVA: 0x0021E508 File Offset: 0x0021C708
		public int SetToMaxSortIndex(int originalValue)
		{
			int num = -1;
			foreach (TIGameState tigameState in this.controller.FinderItems(false))
			{
				if (this.gamestate.isCouncilorState && tigameState.isCouncilorState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > num)
				{
					num = tigameState.finderSortOverride;
				}
				if (this.gamestate.isArmyState && tigameState.isArmyState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > num)
				{
					num = tigameState.finderSortOverride;
				}
				if (this.gamestate.isHabState && tigameState.isHabState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > num)
				{
					num = tigameState.finderSortOverride;
				}
				if (this.gamestate.isSpaceFleetState && tigameState.isSpaceFleetState && this.gamestate.ID != tigameState.ID && tigameState.finderSortOverride > num)
				{
					num = tigameState.finderSortOverride;
				}
			}
			if (num > originalValue)
			{
				return num;
			}
			return originalValue;
		}

		// Token: 0x06004EB6 RID: 20150 RVA: 0x0021E66C File Offset: 0x0021C86C
		private void OnDestroy()
		{
			this.RemoveListeners(true);
		}

		// Token: 0x04003224 RID: 12836
		public Image itemIcon;

		// Token: 0x04003225 RID: 12837
		public Image itemBackground;

		// Token: 0x04003226 RID: 12838
		public TMP_Text itemName;

		// Token: 0x04003227 RID: 12839
		public Image itemLocation;

		// Token: 0x04003228 RID: 12840
		public Image statusIcon;

		// Token: 0x04003229 RID: 12841
		public Button finderButton;

		// Token: 0x0400322A RID: 12842
		public GameObject editModeCanvas;

		// Token: 0x0400322B RID: 12843
		private FinderListItemType itemType;

		// Token: 0x0400322C RID: 12844
		private GeneralControlsController controller;

		// Token: 0x0400322D RID: 12845
		private TIGameState gamestate;

		// Token: 0x0400322E RID: 12846
		private TIArmyState army;

		// Token: 0x0400322F RID: 12847
		private TICouncilorState councilor;

		// Token: 0x04003230 RID: 12848
		private TISpaceFleetState fleet;

		// Token: 0x04003231 RID: 12849
		private TIHabState hab;
	}
}
