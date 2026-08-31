using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PavonisInteractive.TerraInvicta.Audio;
using TMPro;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A03 RID: 2563
	public class BattleLogController : MonoBehaviour
	{
		// Token: 0x060062A4 RID: 25252 RVA: 0x002E64D0 File Offset: 0x002E46D0
		public void Init()
		{
			this.battleLogInstance.SetActive(false);
			this.battleLogWindowTitle.text = Loc.T("UI.SpaceCombat.BattleLog.Title");
			this.battleLogHeader.text = Loc.T("UI.SpaceCombat.BattleLog.Log");
			this.shipsOutOfDV = new HashSet<TISpaceShipState>();
			this.sortMostRecentLogsFirst = true;
			this.InitFilter();
			this.includeAllDamage = TemplateManager.global.logAllDamageInCombat;
			GameControl.eventManager.AddListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.AddDestroyLog), null, null, true, false);
			GameControl.eventManager.AddListener<HabModuleDestroyedInCombat>(new EventManager.EventDelegate<HabModuleDestroyedInCombat>(this.AddDestroyHabLog), null, null, true, false);
			GameControl.eventManager.AddListener<BattleGroupReinforcementArrived>(new EventManager.EventDelegate<BattleGroupReinforcementArrived>(this.AddBattleGroupReinforcemntLog), null, null, true, false);
			GameControl.eventManager.AddListener<ReinforcementArrived>(new EventManager.EventDelegate<ReinforcementArrived>(this.AddShipReinforcementLog), null, null, true, false);
			GameControl.eventManager.AddListener<ShipRetreatsFromCombat>(new EventManager.EventDelegate<ShipRetreatsFromCombat>(this.AddDisengageLog), null, null, true, false);
			GameControl.eventManager.AddListener<ShipWeaponOutOfAmmo>(new EventManager.EventDelegate<ShipWeaponOutOfAmmo>(this.AddShipWeaponOutOfAmmo), null, null, true, false);
			GameControl.eventManager.AddListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.AddShipOutOfDVLog), null, null, true, false);
			GameControl.eventManager.AddListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.AddShipPartDestroyedLog), null, null, true, false);
			GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.AddShipSystemDamageUpdateLog), null, null, true, false);
			GameControl.eventManager.AddListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.AddShipOfficerKilledLog), null, null, true, false);
			if (this.includeAllDamage)
			{
				GameControl.eventManager.AddListener<ShipArmorFacingStruckInCombat>(new EventManager.EventDelegate<ShipArmorFacingStruckInCombat>(this.AddShipDamagedLog), null, null, true, false);
				GameControl.eventManager.AddListener<HabModuleDamagedInCombat>(new EventManager.EventDelegate<HabModuleDamagedInCombat>(this.AddHabDamagedLog), null, null, true, false);
			}
			this.eventListenersCleanedUp = false;
		}

		// Token: 0x060062A5 RID: 25253 RVA: 0x002E6682 File Offset: 0x002E4882
		private void OnDestroy()
		{
			this.PostCombatCleanup();
		}

		// Token: 0x060062A6 RID: 25254 RVA: 0x002E668C File Offset: 0x002E488C
		public void PostCombatCleanup()
		{
			this.battleLogListModels.Clear();
			this.UpdateBattleLogData();
			if (this.eventListenersCleanedUp)
			{
				return;
			}
			GameControl.eventManager.RemoveListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.AddDestroyLog), null);
			GameControl.eventManager.RemoveListener<HabModuleDestroyedInCombat>(new EventManager.EventDelegate<HabModuleDestroyedInCombat>(this.AddDestroyHabLog), null);
			GameControl.eventManager.RemoveListener<BattleGroupReinforcementArrived>(new EventManager.EventDelegate<BattleGroupReinforcementArrived>(this.AddBattleGroupReinforcemntLog), null);
			GameControl.eventManager.RemoveListener<ReinforcementArrived>(new EventManager.EventDelegate<ReinforcementArrived>(this.AddShipReinforcementLog), null);
			GameControl.eventManager.RemoveListener<ShipRetreatsFromCombat>(new EventManager.EventDelegate<ShipRetreatsFromCombat>(this.AddDisengageLog), null);
			GameControl.eventManager.RemoveListener<ShipWeaponOutOfAmmo>(new EventManager.EventDelegate<ShipWeaponOutOfAmmo>(this.AddShipWeaponOutOfAmmo), null);
			GameControl.eventManager.RemoveListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.AddShipOutOfDVLog), null);
			GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.AddShipPartDestroyedLog), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.AddShipSystemDamageUpdateLog), null);
			GameControl.eventManager.RemoveListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.AddShipOfficerKilledLog), null);
			if (this.includeAllDamage)
			{
				GameControl.eventManager.RemoveListener<ShipArmorFacingStruckInCombat>(new EventManager.EventDelegate<ShipArmorFacingStruckInCombat>(this.AddShipDamagedLog), null);
				GameControl.eventManager.RemoveListener<HabModuleDamagedInCombat>(new EventManager.EventDelegate<HabModuleDamagedInCombat>(this.AddHabDamagedLog), null);
			}
			this.eventListenersCleanedUp = true;
		}

		// Token: 0x060062A7 RID: 25255 RVA: 0x002E67D8 File Offset: 0x002E49D8
		private void InitFilter()
		{
			this.filterType = BattleLogController.BattleLogType.All;
			this.filterDropdown.ClearOptions();
			foreach (object obj in Enum.GetValues(typeof(BattleLogController.BattleLogType)))
			{
				BattleLogController.BattleLogType battleLogType = (BattleLogController.BattleLogType)obj;
				if (this.includeAllDamage || (battleLogType != BattleLogController.BattleLogType.ShipDamaged && battleLogType != BattleLogController.BattleLogType.ArmorHolding))
				{
					string text;
					string text2;
					switch (battleLogType)
					{
					case BattleLogController.BattleLogType.All:
						text = Loc.T("UI.SpaceCombat.BattleLog.AllLogs.Title");
						text2 = "icons_2d/ICO_education";
						break;
					case BattleLogController.BattleLogType.Destruction:
						text = Loc.T("UI.SpaceCombat.BattleLog.Destruction.Title");
						text2 = "icons_2d/ICO_army_battle";
						break;
					case BattleLogController.BattleLogType.ShipDamaged:
						text = Loc.T("UI.SpaceCombat.BattleLog.ShipDamaged.Title");
						text2 = "icons_2d/ICO_ship_damage";
						break;
					case BattleLogController.BattleLogType.ArmorHolding:
						text = Loc.T("UI.SpaceCombat.BattleLog.ArmorHolding.Title");
						text2 = "icons_2d/ICO_armor";
						break;
					case BattleLogController.BattleLogType.PartDestroyed:
						text = Loc.T("UI.SpaceCombat.BattleLog.PartDestroyed.Title");
						text2 = "ui_spacecombat/ICO_battle_radar_dot_B";
						break;
					case BattleLogController.BattleLogType.WeaponDestroyed:
						text = Loc.T("UI.SpaceCombat.BattleLog.WeaponDestroyed.Title");
						text2 = "ui_spacecombat/ICO_weaponBollixed";
						break;
					case BattleLogController.BattleLogType.CriticalWarning:
						text = Loc.T("UI.SpaceCombat.BattleLog.AllWeaponsDestroyed.Title");
						text2 = "ui/ICO_critical";
						break;
					case BattleLogController.BattleLogType.PartRepaired:
						text = Loc.T("UI.SpaceCombat.BattleLog.PartRepaired.Title");
						text2 = "ui_spacecombat/ICO_Wrench";
						break;
					case BattleLogController.BattleLogType.OfficerKilled:
						text = Loc.T("UI.SpaceCombat.BattleLog.OfficerKilled.Title");
						text2 = "icons_2d/Ship_Officer_Admiral_1";
						break;
					case BattleLogController.BattleLogType.OutOfAmmo:
						text = Loc.T("UI.SpaceCombat.BattleLog.OutOfAmmo.Title");
						text2 = "icons_2d/ICO_none";
						break;
					case BattleLogController.BattleLogType.OutOfDV:
						text = Loc.T("UI.SpaceCombat.BattleLog.OutOfDV.Title");
						text2 = "ui_spacecombat/ICO_battle_Delta_V_text";
						break;
					case BattleLogController.BattleLogType.Retreat:
						text = Loc.T("UI.SpaceCombat.BattleLog.Retreat.Title");
						text2 = "icons_2d/ICO_escape";
						break;
					case BattleLogController.BattleLogType.Reinforcements:
						text = Loc.T("UI.SpaceCombat.BattleLog.Reinforcements.Title");
						text2 = "icons_2d/ICO_fleet_finder";
						break;
					default:
						text = "Missing Option";
						text2 = "icons_2d/ICO_warning";
						break;
					}
					TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData
					{
						text = text,
						image = GameControl.assetLoader.LoadAsset<Sprite>(text2)
					};
					this.filterDropdown.options.Add(optionData);
					this.filterDropdown.SetValueWithoutNotify(0);
				}
			}
		}

		// Token: 0x060062A8 RID: 25256 RVA: 0x002E6A04 File Offset: 0x002E4C04
		public void OnBattleLogFilterChanged()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.filterType = (BattleLogController.BattleLogType)this.filterDropdown.value;
			if (!this.includeAllDamage && this.filterDropdown.value >= 2)
			{
				this.filterType = this.filterDropdown.value + BattleLogController.BattleLogType.ShipDamaged;
			}
			this.SortBattleLogEntries();
		}

		// Token: 0x060062A9 RID: 25257 RVA: 0x002E6A5D File Offset: 0x002E4C5D
		public void OnTimeSortButtonPressed()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.sortMostRecentLogsFirst = !this.sortMostRecentLogsFirst;
			this.SortBattleLogEntries();
		}

		// Token: 0x060062AA RID: 25258 RVA: 0x002E6A80 File Offset: 0x002E4C80
		private void AddLog(string message, BattleLogController.BattleLogType type)
		{
			if (!TIGlobalValuesState.isSpaceCombatEnabled)
			{
				return;
			}
			string combatTimeString = GameControl.spaceCombat.combatHUD.clockController.combatTimeString;
			string text;
			switch (type)
			{
			case BattleLogController.BattleLogType.Destruction:
				text = "icons_2d/ICO_army_battle";
				break;
			case BattleLogController.BattleLogType.ShipDamaged:
				text = "icons_2d/ICO_ship_damage";
				break;
			case BattleLogController.BattleLogType.ArmorHolding:
				text = "icons_2d/ICO_armor";
				break;
			case BattleLogController.BattleLogType.PartDestroyed:
				text = "ui_spacecombat/ICO_battle_radar_dot_B";
				break;
			case BattleLogController.BattleLogType.WeaponDestroyed:
				text = "ui_spacecombat/ICO_weaponBollixed";
				break;
			case BattleLogController.BattleLogType.CriticalWarning:
				text = "ui/ICO_critical";
				break;
			case BattleLogController.BattleLogType.PartRepaired:
				text = "ui_spacecombat/ICO_Wrench";
				break;
			case BattleLogController.BattleLogType.OfficerKilled:
				text = "icons_2d/Ship_Officer_Admiral_1";
				break;
			case BattleLogController.BattleLogType.OutOfAmmo:
				text = "icons_2d/ICO_none";
				break;
			case BattleLogController.BattleLogType.OutOfDV:
				text = "ui_spacecombat/ICO_battle_Delta_V_text";
				break;
			case BattleLogController.BattleLogType.Retreat:
				text = "icons_2d/ICO_escape";
				break;
			case BattleLogController.BattleLogType.Reinforcements:
				text = "icons_2d/ICO_fleet_finder";
				break;
			default:
				text = "icons_2d/ICO_warning";
				break;
			}
			BattleLogListItemModel battleLogListItemModel = new BattleLogListItemModel();
			BattleLogEntry.BattleLogEntry_Data battleLogEntry_Data = new BattleLogEntry.BattleLogEntry_Data();
			string[] array = combatTimeString.Split(new char[] { ':' });
			int num = 0;
			int num2;
			int num3;
			if (array.Length == 3)
			{
				num = int.Parse(array[0]);
				num2 = int.Parse(array[1]);
				num3 = int.Parse(array[2]);
			}
			else
			{
				num2 = int.Parse(array[0]);
				num3 = int.Parse(array[1]);
			}
			battleLogEntry_Data.timeStampSeconds = num * 3600 + num2 * 60 + num3;
			battleLogEntry_Data.timeStampText = combatTimeString;
			battleLogEntry_Data.battleLogText = message;
			battleLogEntry_Data.imageTypeName = text;
			battleLogEntry_Data.enableKIAIcon = type == BattleLogController.BattleLogType.OfficerKilled;
			battleLogEntry_Data.logType = type;
			battleLogEntry_Data.showInList = this.filterType == BattleLogController.BattleLogType.All || this.filterType == battleLogEntry_Data.logType;
			battleLogListItemModel.battleLogEntryData = battleLogEntry_Data;
			this.battleLogListModels.Add(battleLogListItemModel);
			this.SortBattleLogEntries();
		}

		// Token: 0x060062AB RID: 25259 RVA: 0x002E6C28 File Offset: 0x002E4E28
		public void SortBattleLogEntries()
		{
			if (this.sortMostRecentLogsFirst)
			{
				this.battleLogListModels = this.battleLogListModels.OrderByDescending<BattleLogListItemModel, int>((BattleLogListItemModel x) => x.battleLogEntryData.timeStampSeconds).ToList<BattleLogListItemModel>();
			}
			else
			{
				this.battleLogListModels = this.battleLogListModels.OrderBy<BattleLogListItemModel, int>((BattleLogListItemModel x) => x.battleLogEntryData.timeStampSeconds).ToList<BattleLogListItemModel>();
			}
			foreach (BattleLogListItemModel battleLogListItemModel in this.battleLogListModels)
			{
				battleLogListItemModel.battleLogEntryData.showInList = this.filterType == BattleLogController.BattleLogType.All || this.filterType == battleLogListItemModel.battleLogEntryData.logType;
			}
			this.UpdateBattleLogData();
		}

		// Token: 0x060062AC RID: 25260 RVA: 0x002E6D18 File Offset: 0x002E4F18
		private void UpdateBattleLogData()
		{
			this.battleLogListAdapter.SetItems(this.battleLogListModels);
		}

		// Token: 0x060062AD RID: 25261 RVA: 0x002E6D2C File Offset: 0x002E4F2C
		public void AddDestroyLog(ShipDestroyed destroyedEvent)
		{
			TISpaceShipState ship = destroyedEvent.ship;
			if (!TIGameState.Valid(ship))
			{
				return;
			}
			if (ship.hasDisengaged)
			{
				return;
			}
			if (ship.faction == null || ship.hull == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (TIGameState.Valid(destroyedEvent.killer) && destroyedEvent.killer.isSpaceShipState && destroyedEvent.killerWeapon != null && destroyedEvent.killer.ref_ship.hull != null)
			{
				TISpaceShipState ref_ship = destroyedEvent.killer.ref_ship;
				TIShipWeaponTemplate killerWeapon = destroyedEvent.killerWeapon;
				string adjectiveWithColor = destroyedEvent.killer.ref_faction.adjectiveWithColor;
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipDestruction", new object[]
				{
					ship.faction.adjectiveWithColor,
					ship.hull.displayName,
					ship.displayName,
					killerWeapon.displayName,
					adjectiveWithColor,
					ref_ship.hull.displayName,
					ref_ship.displayName
				}));
			}
			else
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipDestructionNoKiller", new object[]
				{
					ship.faction.adjectiveWithColor,
					ship.hull.displayName,
					ship.displayName
				}));
			}
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.Destruction);
		}

		// Token: 0x060062AE RID: 25262 RVA: 0x002E6E80 File Offset: 0x002E5080
		public void AddDestroyHabLog(HabModuleDestroyedInCombat e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (TIGameState.Valid(e.habModule) && TIGameState.Valid(e.habModule.hab) && e.damageSource != null && e.damageSource.attacker != null && e.damageSource.attacker.GetFaction() != null && e.damageSource.damage.weapon != null && e.damageSource.attacker.ref_shipCarrier() != null && TIGameState.Valid(e.damageSource.attacker.ref_shipCarrier().ref_ship) && e.damageSource.attacker.ref_shipCarrier().ref_ship.hull != null)
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.HabDestruction", new object[]
				{
					e.habModule.displayName,
					e.habModule.hab.displayName,
					e.damageSource.damage.weapon.displayName,
					e.damageSource.attacker.GetFaction().adjectiveWithColor,
					e.damageSource.attacker.ref_shipCarrier().ref_ship.hull.displayName,
					e.damageSource.attacker.ref_shipCarrier().ref_ship.displayName
				}));
				this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.Destruction);
			}
		}

		// Token: 0x060062AF RID: 25263 RVA: 0x002E7018 File Offset: 0x002E5218
		private void AddBattleGroupReinforcemntLog(BattleGroupReinforcementArrived e)
		{
			if (!TIGameState.Valid(e.shipState))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.BattleGroupReinforced", new object[]
			{
				e.battleGroupSize,
				e.shipState.faction.displayNameWithColor,
				e.shipState.hull.displayName,
				e.shipState.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.Reinforcements);
		}

		// Token: 0x060062B0 RID: 25264 RVA: 0x002E70A0 File Offset: 0x002E52A0
		private void AddShipReinforcementLog(ReinforcementArrived e)
		{
			if (!TIGameState.Valid(e.shipState))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipReinforced", new object[]
			{
				e.shipState.faction.adjectiveWithColor,
				e.shipState.hull.displayName,
				e.shipState.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.Reinforcements);
		}

		// Token: 0x060062B1 RID: 25265 RVA: 0x002E711C File Offset: 0x002E531C
		private void AddDisengageLog(ShipRetreatsFromCombat e)
		{
			if (!TIGameState.Valid(e.shipState))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipDisengaged", new object[]
			{
				e.shipState.faction.adjectiveWithColor,
				e.shipState.hull.displayName,
				e.shipState.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.Retreat);
		}

		// Token: 0x060062B2 RID: 25266 RVA: 0x002E7198 File Offset: 0x002E5398
		private void AddShipWeaponOutOfAmmo(ShipWeaponOutOfAmmo e)
		{
			if (!TIGameState.Valid(e.shipState) || GameControl.control.activePlayer != e.shipState.faction)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.OutOfAmmo", new object[]
			{
				e.shipState.faction.adjectiveWithColor,
				e.shipState.hull.displayName,
				e.shipState.displayName,
				e.weaponData.weaponTemplate.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.OutOfAmmo);
		}

		// Token: 0x060062B3 RID: 25267 RVA: 0x002E7244 File Offset: 0x002E5444
		private void AddShipOutOfDVLog(ShipDeltaVChange e)
		{
			if (!TIGameState.Valid(e.ship) || e.ship.faction == null || this.shipsOutOfDV.Contains(e.ship) || e.ship.currentDeltaV_kps > 0f)
			{
				return;
			}
			this.shipsOutOfDV.Add(e.ship);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.OutOfDV", new object[]
			{
				e.ship.faction.adjectiveWithColor,
				e.ship.hull.displayName,
				e.ship.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.OutOfDV);
		}

		// Token: 0x060062B4 RID: 25268 RVA: 0x002E7308 File Offset: 0x002E5508
		private void AddShipOfficerKilledLog(ShipOfficerKilled e)
		{
			if (GameControl.control.activePlayer != e.ship.faction)
			{
				return;
			}
			if (!TIGameState.Valid(e.ship))
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.OfficerKilled", new object[]
			{
				e.officerNameAndJob,
				e.ship.faction.displayNameWithColor,
				e.ship.hull.displayName,
				e.ship.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.OfficerKilled);
		}

		// Token: 0x060062B5 RID: 25269 RVA: 0x002E73A8 File Offset: 0x002E55A8
		private void AddShipPartDestroyedLog(ShipPartDamageChange e)
		{
			if (!TIGameState.Valid(e.ship) || e.ship.faction == null || e.ship.ShipDestroyed())
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (e.ship.PartDestroyed(e.partData))
			{
				if (GameControl.control.activePlayer == e.ship.faction)
				{
					stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.PartDestroyed", new object[]
					{
						e.ship.faction.adjectiveWithColor,
						e.ship.hull.displayName,
						e.ship.displayName,
						e.partData.moduleTemplate.displayName
					}));
				}
				else if (e.partData.moduleTemplate.displayName == "Unknown")
				{
					stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.PartDestroyedEnemyUnknown", new object[]
					{
						e.ship.faction.adjectiveWithColor,
						e.ship.hull.displayName,
						e.ship.displayName
					}));
				}
				else
				{
					stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.PartDestroyedEnemy", new object[]
					{
						e.ship.faction.adjectiveWithColor,
						e.ship.hull.displayName,
						e.ship.displayName,
						e.partData.moduleTemplate.displayName
					}));
				}
				BattleLogController.BattleLogType battleLogType = (e.partData.moduleTemplate.isWeapon ? BattleLogController.BattleLogType.WeaponDestroyed : BattleLogController.BattleLogType.PartDestroyed);
				this.AddLog(stringBuilder.ToString(), battleLogType);
				if (e.partData.moduleTemplate.isWeapon && e.ship.AllWeaponsDestroyed())
				{
					this.AddAllShipWeaponsDisabled(e.ship);
					return;
				}
			}
			else if (e.partRepaired && e.ship.GetPartDamage(e.partData) <= 0f)
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.PartRepaired", new object[]
				{
					e.ship.faction.adjectiveWithColor,
					e.ship.hull.displayName,
					e.ship.displayName,
					e.partData.moduleTemplate.displayName
				}));
				this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.PartRepaired);
			}
		}

		// Token: 0x060062B6 RID: 25270 RVA: 0x002E762C File Offset: 0x002E582C
		private void AddAllShipWeaponsDisabled(TISpaceShipState shipState)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.AllWeaponsDestroyed", new object[]
			{
				shipState.faction.adjectiveWithColor,
				shipState.hull.displayName,
				shipState.displayName
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.CriticalWarning);
		}

		// Token: 0x060062B7 RID: 25271 RVA: 0x002E7688 File Offset: 0x002E5888
		private void AddShipSystemDamageUpdateLog(ShipSystemDamageChange e)
		{
			if (!TIGameState.Valid(e.ship) || e.ship.faction == null || e.ship.ShipDestroyed())
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (!e.ship.SystemDestroyed(e.system))
			{
				if (e.systemRepaired && e.ship.GetSystemDamage(e.system) <= 0f)
				{
					stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.PartRepaired", new object[]
					{
						e.ship.faction.adjectiveWithColor,
						e.ship.hull.displayName,
						e.ship.displayName,
						Loc.T(new StringBuilder("UI.SpaceCombat.").Append(e.system.ToString()).ToString())
					}));
					this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.PartRepaired);
				}
				return;
			}
			if (e.system == ShipSystem.NoseStructure || e.system == ShipSystem.TailStructure || e.system == ShipSystem.CentralStructure)
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipInternalStructureDestroyed", new object[]
				{
					e.ship.faction.adjectiveWithColor,
					e.ship.hull.displayName,
					e.ship.displayName,
					Loc.T(new StringBuilder("UI.SpaceCombat.").Append(e.system.ToString()).ToString())
				}));
				this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.PartDestroyed);
				return;
			}
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.PartDestroyed", new object[]
			{
				e.ship.faction.adjectiveWithColor,
				e.ship.hull.displayName,
				e.ship.displayName,
				Loc.T(new StringBuilder("UI.SpaceCombat.").Append(e.system.ToString()).ToString())
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.PartDestroyed);
		}

		// Token: 0x060062B8 RID: 25272 RVA: 0x002E78B4 File Offset: 0x002E5AB4
		private void AddShipDamagedLog(ShipArmorFacingStruckInCombat e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			float num = e.rawDamage - e.penetratedDamage;
			if (!TIGameState.Valid(e.ship) || e.ship.faction == null)
			{
				return;
			}
			if (e.rawDamage == num && e.radiationDamage <= 0f)
			{
				this.<AddShipDamagedLog>g__AddArmorHoldingLog|33_0(e.ship, e.armorFacing, e.weapon, e.rawDamage, e.penetratedDamage);
				return;
			}
			if (e.radiationDamage > 0f)
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipDamagedRadiation", new object[]
				{
					e.ship.faction.adjectiveWithColor,
					e.ship.hull.displayName,
					e.ship.displayName,
					e.weapon.displayName,
					TIUtilities.FormatSmallNumber(e.rawDamage, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(num, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(e.penetratedDamage, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(e.radiationDamage, 1, 0, true, false)
				}));
			}
			else if (e.weapon == null)
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipDamagedNoWeapon", new object[]
				{
					e.ship.faction.adjectiveWithColor,
					e.ship.hull.displayName,
					e.ship.displayName,
					TIUtilities.FormatSmallNumber(e.rawDamage, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(num, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(e.penetratedDamage, 1, 0, true, false)
				}));
			}
			else
			{
				stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ShipDamaged", new object[]
				{
					e.ship.faction.adjectiveWithColor,
					e.ship.hull.displayName,
					e.ship.displayName,
					e.weapon.displayName,
					TIUtilities.FormatSmallNumber(e.rawDamage, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(num, 1, 0, true, false),
					TIUtilities.FormatSmallNumber(e.penetratedDamage, 1, 0, true, false)
				}));
			}
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.ShipDamaged);
		}

		// Token: 0x060062B9 RID: 25273 RVA: 0x002E7B08 File Offset: 0x002E5D08
		private void AddHabDamagedLog(HabModuleDamagedInCombat e)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.HabDamaged", new object[]
			{
				e.habModule.displayName,
				e.habModule.hab.displayName,
				e.weapon.displayName,
				TIUtilities.FormatSmallNumber(e.rawDamage, 1, 0, true, false)
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.ShipDamaged);
		}

		// Token: 0x060062BB RID: 25275 RVA: 0x002E7B94 File Offset: 0x002E5D94
		[CompilerGenerated]
		private void <AddShipDamagedLog>g__AddArmorHoldingLog|33_0(TISpaceShipState ship, ArmorFacing armorFacing, TIShipWeaponTemplate weapon, float rawDamage, float penetratedDamage)
		{
			float armorIntegrity = ship.armor[armorFacing].GetArmorIntegrity();
			string text = armorIntegrity.ToPercent(TIUtilities.DecimalPlaces_P((double)armorIntegrity, 7, 0));
			string text2 = "";
			switch (armorFacing)
			{
			case ArmorFacing.Nose:
				text2 = Loc.T("UI.SpaceCombat.NoseArmorIntegrity");
				break;
			case ArmorFacing.Right:
				text2 = Loc.T("UI.SpaceCombat.PortArmorIntegrity");
				break;
			case ArmorFacing.Left:
				text2 = Loc.T("UI.SpaceCombat.StarboardArmorIntegrity");
				break;
			case ArmorFacing.Tail:
				text2 = Loc.T("UI.SpaceCombat.TailArmorIntegrity");
				break;
			}
			StringBuilder stringBuilder = new StringBuilder();
			float num = rawDamage - penetratedDamage;
			stringBuilder.Append(Loc.T("UI.SpaceCombat.BattleLog.ArmorHolding", new object[]
			{
				ship.faction.adjectiveWithColor,
				ship.hull.displayName,
				ship.displayName,
				weapon.displayName,
				TIUtilities.FormatSmallNumber(rawDamage, 1, 0, true, false),
				TIUtilities.FormatSmallNumber(num, 1, 0, true, false),
				text2,
				text
			}));
			this.AddLog(stringBuilder.ToString(), BattleLogController.BattleLogType.ArmorHolding);
		}

		// Token: 0x04004591 RID: 17809
		public GameObject battleLogInstance;

		// Token: 0x04004592 RID: 17810
		public GameObject logWindow;

		// Token: 0x04004593 RID: 17811
		public TMP_Text battleLogWindowTitle;

		// Token: 0x04004594 RID: 17812
		public TMP_Text battleLogHeader;

		// Token: 0x04004595 RID: 17813
		public TMP_Dropdown filterDropdown;

		// Token: 0x04004596 RID: 17814
		public BattleLogListAdapter battleLogListAdapter;

		// Token: 0x04004597 RID: 17815
		public List<BattleLogListItemModel> battleLogListModels = new List<BattleLogListItemModel>();

		// Token: 0x04004598 RID: 17816
		private HashSet<TISpaceShipState> shipsOutOfDV;

		// Token: 0x04004599 RID: 17817
		private BattleLogController.BattleLogType filterType;

		// Token: 0x0400459A RID: 17818
		private bool eventListenersCleanedUp;

		// Token: 0x0400459B RID: 17819
		private bool sortMostRecentLogsFirst;

		// Token: 0x0400459C RID: 17820
		private bool includeAllDamage;

		// Token: 0x0200139D RID: 5021
		public enum BattleLogType
		{
			// Token: 0x04007232 RID: 29234
			All,
			// Token: 0x04007233 RID: 29235
			Destruction,
			// Token: 0x04007234 RID: 29236
			ShipDamaged,
			// Token: 0x04007235 RID: 29237
			ArmorHolding,
			// Token: 0x04007236 RID: 29238
			PartDestroyed,
			// Token: 0x04007237 RID: 29239
			WeaponDestroyed,
			// Token: 0x04007238 RID: 29240
			CriticalWarning,
			// Token: 0x04007239 RID: 29241
			PartRepaired,
			// Token: 0x0400723A RID: 29242
			OfficerKilled,
			// Token: 0x0400723B RID: 29243
			OutOfAmmo,
			// Token: 0x0400723C RID: 29244
			OutOfDV,
			// Token: 0x0400723D RID: 29245
			Retreat,
			// Token: 0x0400723E RID: 29246
			Reinforcements
		}
	}
}
