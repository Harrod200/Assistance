using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.UI;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000913 RID: 2323
	public class TerminalFleetCommands
	{
		// Token: 0x060058D2 RID: 22738 RVA: 0x0028B3F2 File Offset: 0x002895F2
		public TerminalFleetCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x060058D3 RID: 22739 RVA: 0x0028B408 File Offset: 0x00289608
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("SetDV", new CommandHandler(this.SetDV), "Set all ships in the currently selected fleet Delta V to the specified fraction of their max");
			this.terminalController.RegisterCommand("Involuntary", new CommandHandler(this.Involuntary), "Make the currently selected fleet's trajectory involuntary: it can launch from this trajectory");
			this.terminalController.RegisterCommand("KillShip", new CommandHandler(this.KillShip), "COMBAT ONLY: Kill primary enemy target; if none, kill selected player ship");
			this.terminalController.RegisterCommand("KillEnemyFleet", new CommandHandler(this.KillEnemyFleet), "COMBAT ONLY: Kill all enemy ships");
			this.terminalController.RegisterCommand("DamageShip", new CommandHandler(this.DamageShip), "Apply (value) points of internal damage to a ship in the selected fleet");
			this.terminalController.RegisterCommand("DestroyPart", new CommandHandler(this.DestroyPart), "Destroy a specific part of the primary enemy target; if none, destroy specified part of the selected player ship, set true to allow secondary explosions: 'DestroyPart FireControl, false'");
			this.terminalController.RegisterCommand("DamagePart", new CommandHandler(this.DamagePart), "Damages a specific part of the primary enemy target; if none, destroy specified part of the selected player ship: 'DamagePart VectorThrusters, 0.5'");
			this.terminalController.RegisterCommand("UnlockAllShipParts", new CommandHandler(this.UnlockAllShipParts), "Unlocks all ship parts for construction");
			this.terminalController.RegisterCommand("DestroyModule", new CommandHandler(this.DestroyModule), "Destroy Selected Hab Module in Habs UI");
			this.terminalController.RegisterCommand("CompleteModule", new CommandHandler(this.CompleteModule), "Complete Selected Hab Module in Habs UI");
			this.terminalController.RegisterCommand("AddOfficer", new CommandHandler(this.AddOfficer), "Add an officer to the indicated ship: AddOfficer shipName,officerDataName");
		}

		// Token: 0x060058D4 RID: 22740 RVA: 0x0028B580 File Offset: 0x00289780
		public void AddOfficer(string[] args)
		{
			if (args.Length < 2)
			{
				this.terminalController.OutputError("Insufficient Input: AddOfficer shipName,officerDataName");
				return;
			}
			TISpaceShipState tispaceShipState = GameStateManager.IterateByClass<TISpaceShipState>(false).FirstOrDefault<TISpaceShipState>((TISpaceShipState x) => x.displayName.ToUpper() == args[0].ToUpper());
			if (!(tispaceShipState != null))
			{
				this.terminalController.OutputError("Ship not found: " + args[0]);
				return;
			}
			TIOfficerTemplate tiofficerTemplate = TemplateManager.Find<TIOfficerTemplate>(args[1], false);
			if (tiofficerTemplate == null)
			{
				args[1] = "Officer_" + args[1];
				tiofficerTemplate = TemplateManager.Find<TIOfficerTemplate>(args[1], false);
			}
			if (tiofficerTemplate == null)
			{
				this.terminalController.OutputError("Officer template not found: " + args[1]);
				return;
			}
			if (tiofficerTemplate.OfficerTypeAllowedForShipFailReasons(tispaceShipState, false, 0).Count == 0)
			{
				tispaceShipState.CreateOfficer(args[1]);
				return;
			}
			string text = string.Empty;
			foreach (OfficerRequirement officerRequirement in tiofficerTemplate.OfficerTypeAllowedForShipFailReasons(tispaceShipState, false, 0))
			{
				text = text + officerRequirement.requirement.ToString() + " ";
			}
			this.terminalController.OutputError("Officer type not allowed for ship: " + text);
		}

		// Token: 0x060058D5 RID: 22741 RVA: 0x0028B6F8 File Offset: 0x002898F8
		private void UnlockAllShipParts(string[] args)
		{
			TemplateManager.global.debug_showAllShipPartsIncludingAlien = true;
			TemplateManager.global.debug_showAllShipParts = true;
			IEnumerable<TIShipPartTemplate> enumerable = TemplateManager.IterateByClass<TIShipPartTemplate>(true);
			List<TIShipPartTemplate> list = new List<TIShipPartTemplate>();
			foreach (TIShipPartTemplate tishipPartTemplate in enumerable)
			{
				list.Add(tishipPartTemplate);
			}
			GameControl.control.activePlayer.UpdateAllowedShipParts(list);
		}

		// Token: 0x060058D6 RID: 22742 RVA: 0x0028B774 File Offset: 0x00289974
		private void SetDV(string[] args)
		{
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			TISpaceFleetState tispaceFleetState = ((uiselectedAssetState != null) ? uiselectedAssetState.ref_fleet : null);
			if (tispaceFleetState == null)
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				tispaceFleetState = ((uiotherSelectedState != null) ? uiotherSelectedState.ref_fleet : null);
			}
			if (tispaceFleetState != null)
			{
				float value = TIUtilities.GetFloatValue(args[0]);
				if (value > 1f)
				{
					value /= 100f;
				}
				value = Mathf.Clamp01(value);
				tispaceFleetState.ships.ForEach(delegate(TISpaceShipState x)
				{
					x.RePropellantToMax();
				});
				if (value > 0f)
				{
					tispaceFleetState.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.ConsumeDeltaV(x.currentMaxDeltaV_kps * (1f - value), true);
					});
				}
				else
				{
					tispaceFleetState.ships.ForEach(delegate(TISpaceShipState x)
					{
						x.ConsumeDeltaV(x.currentMaxDeltaV_kps, true);
					});
				}
				if (tispaceFleetState.transferAssigned)
				{
					double num = tispaceFleetState.trajectory.DVConsumedOnTrajectory_mps(TITimeState.Now());
					tispaceFleetState.fleetTrajectoryData.initialDeltaV_mps = (double)tispaceFleetState.currentDeltaV_mps + num;
				}
				tispaceFleetState.VerifyAssignedTransfer(false);
				return;
			}
			this.terminalController.OutputError("Could not get a fleet");
		}

		// Token: 0x060058D7 RID: 22743 RVA: 0x0028B8C0 File Offset: 0x00289AC0
		private void Involuntary(string[] args)
		{
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			TISpaceFleetState tispaceFleetState = ((uiselectedAssetState != null) ? uiselectedAssetState.ref_fleet : null);
			if (tispaceFleetState == null)
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				tispaceFleetState = ((uiotherSelectedState != null) ? uiotherSelectedState.ref_fleet : null);
			}
			if (tispaceFleetState == null)
			{
				this.terminalController.OutputError("Could not get a fleet");
				return;
			}
			if (tispaceFleetState.trajectory == null)
			{
				this.terminalController.OutputError(tispaceFleetState.displayName + " lacks a trajectory");
				return;
			}
			tispaceFleetState.trajectory.involuntary = true;
		}

		// Token: 0x060058D8 RID: 22744 RVA: 0x0028B944 File Offset: 0x00289B44
		private void KillShip(string[] args)
		{
			SpaceCombatManager spaceCombat = GameControl.spaceCombat;
			if (spaceCombat.enabled && spaceCombat.combatHUD.selectedFriendlyShip != null)
			{
				if (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget != null && spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.Ship)
				{
					float num;
					(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.NoseStructure, 10000f, out num);
					float num2;
					(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.CentralStructure, 10000f, out num2);
					float num3;
					(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.TailStructure, 10000f, out num3);
					(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget as CombatShipController).TriggerShipDestruction(null, null);
					return;
				}
				if (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget == null)
				{
					float num4;
					(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.NoseStructure, 10000f, out num4);
					float num5;
					(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.CentralStructure, 10000f, out num5);
					float num6;
					(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.TailStructure, 10000f, out num6);
					spaceCombat.combatHUD.selectedFriendlyShip.TriggerShipDestruction(null, null);
				}
			}
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x0028BAD8 File Offset: 0x00289CD8
		private void KillEnemyFleet(string[] args)
		{
			SpaceCombatManager spaceCombat = GameControl.spaceCombat;
			if (spaceCombat.enabled)
			{
				foreach (CombatShipController combatShipController in spaceCombat.activeShips.Where<CombatShipController>((CombatShipController x) => !x.faction.isActivePlayer))
				{
					float num;
					(combatShipController.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.NoseStructure, 10000f, out num);
					(combatShipController.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.CentralStructure, 10000f, out num);
					(combatShipController.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(ShipSystem.TailStructure, 10000f, out num);
					combatShipController.TriggerShipDestruction(null, null);
				}
			}
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x0028BBA4 File Offset: 0x00289DA4
		private void DamageShip(string[] args)
		{
			TIGameState uiselectedAssetState = GeneralControlsController.UISelectedAssetState;
			TISpaceFleetState tispaceFleetState = ((uiselectedAssetState != null) ? uiselectedAssetState.ref_fleet : null);
			if (tispaceFleetState == null)
			{
				TIGameState uiotherSelectedState = GeneralControlsController.UIOtherSelectedState;
				tispaceFleetState = ((uiotherSelectedState != null) ? uiotherSelectedState.ref_fleet : null);
			}
			if (tispaceFleetState != null && args.Length != 0)
			{
				float floatValue = TIUtilities.GetFloatValue(args[0]);
				tispaceFleetState.ships.SelectRandomItem<TISpaceShipState>().ApplyInternalDamage(ArmorFacing.Core, ArmorFacing.Core, floatValue, false, 0f, 0f);
				return;
			}
			this.terminalController.OutputError("Could not get a fleet or value");
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x0028BC24 File Offset: 0x00289E24
		private void DestroyPart(string[] args)
		{
			SpaceCombatManager spaceCombat = GameControl.spaceCombat;
			ShipSystem shipSystem;
			bool flag = Enum.TryParse<ShipSystem>(args[0], out shipSystem);
			bool flag3;
			bool flag2 = args.Length == 2 && bool.TryParse(args[1], out flag3);
			if (flag && shipSystem != ShipSystem.None)
			{
				if (!spaceCombat.enabled)
				{
					this.terminalController.OutputError("Command only valid in combat.");
					return;
				}
				if (!(spaceCombat.combatHUD.selectedFriendlyShip != null))
				{
					this.terminalController.OutputError("No Ship Selected.");
					return;
				}
				if (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget != null && spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.Ship)
				{
					ModuleDataEntry partToDamage = (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).GetPartToDamage(shipSystem, true);
					if (partToDamage == null || partToDamage.moduleTemplate == null)
					{
						float num;
						(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(shipSystem, 10000f, out num);
						return;
					}
					bool flag4;
					float num3;
					float num2 = (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyDamageToPart(partToDamage, partToDamage.moduleTemplate.hitPoints, out flag4, out num3);
					if (num2 > 0f && flag2)
					{
						(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyInternalDamage(ArmorFacing.Right, ArmorFacing.Right, num2, flag4, 0f, 0f);
						return;
					}
				}
				else if (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget == null)
				{
					ModuleDataEntry partToDamage2 = (spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).GetPartToDamage(shipSystem, true);
					if (partToDamage2 == null || partToDamage2.moduleTemplate == null)
					{
						float num4;
						(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyDamageToSystem(shipSystem, 10000f, out num4);
						return;
					}
					bool flag5;
					float num6;
					float num5 = (spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyDamageToPart(partToDamage2, partToDamage2.moduleTemplate.hitPoints, out flag5, out num6);
					if (num5 > 0f && flag2)
					{
						(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyInternalDamage(ArmorFacing.Right, ArmorFacing.Right, num5, flag5, 0f, 0f);
						return;
					}
				}
			}
			else
			{
				this.terminalController.OutputError("Invalid ship part. Valid Ship Parts: NoseStructure, CentralStructure, TailStructure, Bridge, FireControl, PowerCoupling, DriveCoupling, VectorThrusters, LifeSupportMain, LifeSupportBackup, DamageControl, Propellant, NoseWeapons, HullWeapons, UtilityModules, Radiators, PowerPlant, Drive, Battery");
			}
		}

		// Token: 0x060058DC RID: 22748 RVA: 0x0028BE90 File Offset: 0x0028A090
		private void DamagePart(string[] args)
		{
			if (args.Length <= 1)
			{
				this.terminalController.OutputError("Requires Part, PercentDamage");
				return;
			}
			SpaceCombatManager spaceCombat = GameControl.spaceCombat;
			ShipSystem shipSystem;
			bool flag = Enum.TryParse<ShipSystem>(args[0], out shipSystem);
			float num = 0f;
			float.TryParse(args[1], out num);
			if (num < 0f || num > 1f)
			{
				this.terminalController.OutputError("Invalid damage percentage. Input a value between 0.0 and 1.0");
			}
			if (flag && shipSystem != ShipSystem.None)
			{
				if (!spaceCombat.enabled)
				{
					this.terminalController.OutputError("Command only valid in combat.");
					return;
				}
				if (!(spaceCombat.combatHUD.selectedFriendlyShip != null))
				{
					this.terminalController.OutputError("No Ship Selected.");
					return;
				}
				if (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget != null && spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.GetCombatantType() == IDamageableType.Ship)
				{
					ModuleDataEntry partToDamage = (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).GetPartToDamage(shipSystem, true);
					if (partToDamage != null && partToDamage.moduleTemplate != null)
					{
						(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyPercentDamageToPart(partToDamage, num);
						return;
					}
					(spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget.WeaponCarrierState as TISpaceShipState).ApplyPercentDamageToSystem(shipSystem, num);
					return;
				}
				else if (spaceCombat.combatHUD.selectedFriendlyShip.primaryTarget == null)
				{
					ModuleDataEntry partToDamage2 = (spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).GetPartToDamage(shipSystem, true);
					if (partToDamage2 != null && partToDamage2.moduleTemplate != null)
					{
						(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyPercentDamageToPart(partToDamage2, num);
						return;
					}
					(spaceCombat.combatHUD.selectedFriendlyShip.WeaponCarrierState as TISpaceShipState).ApplyPercentDamageToSystem(shipSystem, num);
					return;
				}
			}
			else
			{
				this.terminalController.OutputError("Invalid ship part. Valid Ship Parts: NoseStructure, CentralStructure, TailStructure, Bridge, FireControl, PowerCoupling, DriveCoupling, VectorThrusters, LifeSupportMain, LifeSupportBackup, DamageControl, Propellant, NoseWeapons, HullWeapons, UtilityModules, Radiators, PowerPlant, Drive, Battery");
			}
		}

		// Token: 0x060058DD RID: 22749 RVA: 0x0028C078 File Offset: 0x0028A278
		private void DestroyModule(string[] args)
		{
			HabitatsScreenController infoScreen = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<HabitatsScreenController>();
			if (infoScreen.habToDisplay != null && infoScreen.selectedModule != null)
			{
				infoScreen.habToDisplay.DestroyModule(GameControl.control.activePlayer, infoScreen.selectedModule.habModule, true, false, false, 0f, false, false);
			}
		}

		// Token: 0x060058DE RID: 22750 RVA: 0x0028C0DC File Offset: 0x0028A2DC
		private void CompleteModule(string[] args)
		{
			HabitatsScreenController infoScreen = World.Active.GetExistingManager<CanvasManager>().GetInfoScreen<HabitatsScreenController>();
			if (infoScreen.habToDisplay != null && infoScreen.selectedModule != null && infoScreen.selectedModule.habModule.underConstruction)
			{
				infoScreen.habToDisplay.CompleteModuleConstruction(infoScreen.selectedModule.habModule);
			}
		}

		// Token: 0x04004071 RID: 16497
		private TerminalController terminalController;
	}
}
