using System;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Ship;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A07 RID: 2567
	public class FriendlyShipListItemController : CombatantListItemController
	{
		// Token: 0x060062FF RID: 25343 RVA: 0x002E9B48 File Offset: 0x002E7D48
		public override void Init(SpaceCombatCanvasController masterController, CombatantController combatantController, int position)
		{
			this.RemoveListeners();
			base.Init(masterController, combatantController, position);
			IDamageableType combatantType = base.combatantType;
			if (combatantType != IDamageableType.Ship)
			{
				if (combatantType == IDamageableType.StationModule)
				{
					this.AIControlIcon.enabled = false;
					this.batteryWarningIcon.enabled = false;
					this.DVStaticImage.enabled = false;
					this.deltaVValue.enabled = false;
					this.heatImage.enabled = false;
					this.coolingImage.enabled = false;
					this.warningIcon_Alert.enabled = false;
					this.warningIcon_None.enabled = false;
					this.warningIcon_Warn.enabled = false;
					this.systemStatus_Icon.enabled = false;
					this.weaponStatus_Icon.enabled = false;
					this.rotationStatus_Icon.enabled = false;
					this.damConStatus_Icon.enabled = false;
					this.damConDisabled_Icon.enabled = false;
					this.maneuverList.gameObject.SetActive(false);
					this.targetingImage.enabled = false;
					this.groupMembershipString.enabled = false;
				}
			}
			else
			{
				this.SetAIControl(this.shipState.combatAIControl);
				this.DVStaticImage.enabled = true;
				this.deltaVValue.enabled = true;
				this.heatImage.enabled = true;
				this.coolingImage.enabled = true;
				this.systemStatus_Icon.enabled = true;
				this.weaponStatus_Icon.enabled = true;
				this.rotationStatus_Icon.enabled = true;
				this.SetDeltaVValue();
				this.UpdatePowerSystemStatus();
				this.noseWeaponComponents = (from x in combatantController.ref_shipController.hull.IterateByClass<IWeapon>()
					where (x as Weapon).weaponTemplate.noseWeapon
					select x).ToArray<IWeapon>();
				this.hullWeaponComponents = (from x in combatantController.ref_shipController.hull.IterateByClass<IWeapon>()
					where (x as Weapon).weaponTemplate.hullWeapon
					select x).ToArray<IWeapon>();
				IWeapon[] array = this.noseWeaponComponents;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].currentFireMode is FocusFireMode && this.shipState.combatPrimaryTarget == null)
					{
						this.alertWeaponNoTarget = true;
					}
				}
				array = this.hullWeaponComponents;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].currentFireMode is FocusFireMode && this.shipState.combatPrimaryTarget == null)
					{
						this.alertWeaponNoTarget = true;
					}
				}
				this.alertShipAvoiding = combatantController.ref_shipController.InCollisionAvoidanceManeuver;
				this.UpdateWeaponNoTargetAlert();
				this.UpdateWeaponOperationalStatus();
				this.UpdateCriticalShipSystemsStatus();
				this.UpdateShipThrustStatus();
				this.UpdateShipRotationStatus();
				this.UpdateDamConStatus(false);
				this.UpdateHeatStatus();
				this.SetGroupMembershipString();
				this.maneuverList.gameObject.SetActive(true);
				SpaceCombatCanvasController.UpdateManeuverList(this.shipState, combatantController.ref_shipController, this.maneuverList);
				GameControl.eventManager.AddListener<ShipHeatChange>(new EventManager.EventDelegate<ShipHeatChange>(this.OnHeatChange), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipAIControlChange>(new EventManager.EventDelegate<ShipAIControlChange>(this.OnAIControlChanged), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipCommandExecuted>(new EventManager.EventDelegate<ShipCommandExecuted>(this.OnShipCommandExecuted), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<CombatManeuverComplete>(new EventManager.EventDelegate<CombatManeuverComplete>(this.OnCombatManenuverCompleted), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<CombatCollisionAvoidanceStatusChange>(new EventManager.EventDelegate<CombatCollisionAvoidanceStatusChange>(this.OnCollisionAvoidanceActivated), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.OnShipDeltaVChange), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipPowerSystemsChargeChange>(new EventManager.EventDelegate<ShipPowerSystemsChargeChange>(this.OnShipPowerStorageChange), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnShipWeaponFired), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnShipSystemDamaged), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.OnShipPartDamaged), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipWeaponModeChanged>(new EventManager.EventDelegate<ShipWeaponModeChanged>(this.OnShipWeaponModeChanged), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipDisengageChange>(new EventManager.EventDelegate<ShipDisengageChange>(this.OnShipDisengageChange), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipPrimaryTargetDestroyed>(new EventManager.EventDelegate<ShipPrimaryTargetDestroyed>(this.OnShipPrimaryTargetDestroyed), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<CombatShipGroupChange>(new EventManager.EventDelegate<CombatShipGroupChange>(this.OnUpdateGroupMembershipString), null, this.shipState, false, false);
			}
			this.SetGroupSelected(masterController.groupSelectedFriendlyShips.Contains(combatantController));
			this.SetPrimarySelected(masterController.selectedFriendlyShip == combatantController);
		}

		// Token: 0x06006300 RID: 25344 RVA: 0x002EA010 File Offset: 0x002E8210
		private void UpdateWeaponNoTargetAlert()
		{
			this.alertWeaponNoTarget = false;
			bool flag = this.shipState.combatPrimaryTarget == null;
			bool flag2 = false;
			bool flag3 = false;
			if (!flag)
			{
				if (this.shipState.combatPrimaryTarget.GetTargetableState().ref_ship != null)
				{
					flag2 = this.shipState.combatPrimaryTarget.GetTargetableState().ref_ship.ShipDestroyed();
				}
				else if (this.shipState.combatPrimaryTarget.GetTargetableState().ref_habModule != null)
				{
					flag3 = this.shipState.combatPrimaryTarget.GetTargetableState().ref_habModule.destroyed;
				}
			}
			IWeapon[] array = this.noseWeaponComponents;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].currentFireMode is FocusFireMode && this.shipState.AnyOffensiveWeaponCanFire() && !this.shipState.combatAIControl && (flag || flag2 || flag3))
				{
					this.alertWeaponNoTarget = true;
					break;
				}
			}
			if (!this.alertWeaponNoTarget)
			{
				array = this.hullWeaponComponents;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].currentFireMode is FocusFireMode && this.shipState.AnyOffensiveWeaponCanFire() && !this.shipState.combatAIControl && (flag || flag2 || flag3))
					{
						this.alertWeaponNoTarget = true;
					}
				}
			}
			if (this.shipState.canSuicide)
			{
				this.alertWeaponNoTarget = this.alertWeaponNoTarget && this.shipState.canSuicide;
			}
			this.UpdateAlerts();
		}

		// Token: 0x06006301 RID: 25345 RVA: 0x002EA188 File Offset: 0x002E8388
		private void UpdateWeaponOperationalStatus()
		{
			bool flag = false;
			bool flag2 = true;
			this.alertWeaponNoAmmo = false;
			StringBuilder stringBuilder = new StringBuilder().AppendLine();
			StringBuilder stringBuilder2 = new StringBuilder().AppendLine();
			foreach (ModuleDataEntry moduleDataEntry in this.shipState.AllWeaponModuleData())
			{
				if (!this.shipState.WeaponHasAmmo(moduleDataEntry))
				{
					this.alertWeaponNoAmmo = true;
					stringBuilder2.AppendLine(moduleDataEntry.weaponTemplate.displayName);
				}
				if (this.shipState.WeaponDestroyed(moduleDataEntry))
				{
					flag2 = false;
					stringBuilder.AppendLine(moduleDataEntry.weaponTemplate.displayName);
				}
				else if (this.shipState.WeaponIsOperable(moduleDataEntry))
				{
					flag = true;
				}
			}
			StringBuilder stringBuilder3 = new StringBuilder();
			if (this.shipState.SystemDestroyed(ShipSystem.FireControl))
			{
				stringBuilder3.Append(Loc.T("UI.SpaceCombat.Offline"));
			}
			else if (this.shipState.SystemDamaged(ShipSystem.FireControl))
			{
				stringBuilder3.Append(Loc.T("UI.SpaceCombat.Damaged"));
			}
			else
			{
				stringBuilder3.Append(Loc.T("UI.SpaceCombat.Online"));
			}
			if (flag2 && !this.shipState.SystemDamaged(ShipSystem.FireControl))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_A", this.weaponStatus_Icon);
				this.weaponStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.WeaponStatus.Good"));
			}
			else if ((flag && !flag2) || (flag && this.shipState.SystemDamagedButNotDestroyed(ShipSystem.FireControl)))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_B", this.weaponStatus_Icon);
				this.weaponStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.WeaponStatus.Damaged", new object[]
				{
					stringBuilder.ToString(),
					stringBuilder3.ToString()
				}));
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_C", this.weaponStatus_Icon);
				this.weaponStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.WeaponStatus.Critical", new object[]
				{
					stringBuilder.ToString(),
					stringBuilder3.ToString()
				}));
			}
			if (this.alertWeaponNoAmmo)
			{
				this.warningNoAmmoTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.EmptyMagazine", new object[] { stringBuilder2.ToString() }));
			}
			this.UpdateAlerts();
		}

		// Token: 0x06006302 RID: 25346 RVA: 0x002EA3E0 File Offset: 0x002E85E0
		private void UpdateCriticalShipSystemsStatus()
		{
			if (this.shipState.SystemDestroyed(ShipSystem.Bridge) || (this.shipState.SystemDestroyed(ShipSystem.LifeSupportMain) && this.shipState.SystemDestroyed(ShipSystem.LifeSupportBackup)) || this.shipState.PartDestroyed(this.shipState.powerPlantModule))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_C", this.systemStatus_Icon);
				this.systemStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipSystemStatus.Critical"));
				return;
			}
			if (this.shipState.SystemDestroyed(ShipSystem.LifeSupportMain) || this.shipState.SystemDestroyed(ShipSystem.LifeSupportBackup) || this.shipState.SystemDestroyed(ShipSystem.DamageControl) || this.shipState.SystemDamaged(ShipSystem.PowerCoupling) || this.shipState.PartDamaged(this.shipState.powerPlantModule))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_B", this.systemStatus_Icon);
				this.systemStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipSystemStatus.Damaged"));
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_A", this.systemStatus_Icon);
			this.systemStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipSystemStatus.Good"));
		}

		// Token: 0x06006303 RID: 25347 RVA: 0x002EA514 File Offset: 0x002E8714
		private void UpdateDamConStatus(bool rotationDisabled = false)
		{
			if (this.shipState.SystemDestroyed(ShipSystem.DamageControl))
			{
				this.damConStatus_Icon.enabled = true;
				this.damConStatus_Icon.color = this.damConRed;
				this.damConTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.DamConStatus.Critical"));
				this.damConDisabled_Icon.enabled = false;
				return;
			}
			if (this.shipState.SystemDamaged(ShipSystem.DamageControl))
			{
				this.damConStatus_Icon.enabled = true;
				this.damConStatus_Icon.color = this.damConYellow;
				if (rotationDisabled)
				{
					this.damConTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.DamConStatus.DamagedAndDisabled"));
					this.damConDisabled_Icon.enabled = true;
					return;
				}
				this.damConTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.DamConStatus.Damaged"));
				this.damConDisabled_Icon.enabled = false;
				return;
			}
			else
			{
				if (rotationDisabled)
				{
					this.damConStatus_Icon.enabled = true;
					this.damConStatus_Icon.color = Color.white;
					this.damConTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.DamConStatus.Disabled"));
					this.damConDisabled_Icon.enabled = true;
					return;
				}
				this.damConStatus_Icon.enabled = false;
				this.damConDisabled_Icon.enabled = false;
				return;
			}
		}

		// Token: 0x06006304 RID: 25348 RVA: 0x002EA659 File Offset: 0x002E8859
		private void UpdateShipThrustStatus()
		{
			this.SetDeltaVValue();
			if (!this.shipState.CanSetWaypoints() || this.shipState.currentDeltaV_kps <= 0f)
			{
				this.alertShipNoThrust = true;
			}
			else
			{
				this.alertShipNoThrust = false;
			}
			this.UpdateAlerts();
		}

		// Token: 0x06006305 RID: 25349 RVA: 0x002EA698 File Offset: 0x002E8898
		private void UpdateShipRotationStatus()
		{
			if (this.shipState.SystemDestroyed(ShipSystem.VectorThrusters) || this.shipState.PartDestroyed(this.shipState.powerPlantModule))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_C", this.rotationStatus_Icon);
				this.rotationStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.RotationStatus.Critical"));
			}
			else if (this.shipState.SystemDamaged(ShipSystem.VectorThrusters) || this.shipState.PartDamaged(this.shipState.powerPlantModule))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_B", this.rotationStatus_Icon);
				this.rotationStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.RotationStatus.Damaged"));
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_comp_A", this.rotationStatus_Icon);
				this.rotationStatusTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.RotationStatus.Good"));
			}
			this.UpdateAlerts();
		}

		// Token: 0x06006306 RID: 25350 RVA: 0x002EA790 File Offset: 0x002E8990
		private void UpdateHeatStatus()
		{
			SpaceCombatCanvasController.SetHeatIcon(this.shipState, this.heatImage, this.coolingImage);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Loc.T("UI.Objectives.FleetScreenCanvas.DesignDataShipHeatSink.Name"));
			stringBuilder.Append(string.Format(": {0}/{1} GJ", TIUtilities.FormatSmallNumber(this.shipState.accumulatedHeat_GJ, 1, 0, true, false), TIUtilities.FormatSmallNumber(this.shipState.currentHeatSinkCapacity_GJ, 1, 0, true, false)));
			if (this.coolingImage.enabled)
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.SpaceCombat.RadiatorStatus.Cooling"));
			}
			else
			{
				stringBuilder.AppendLine().AppendLine(Loc.T("UI.SpaceCombat.RadiatorStatus.Heating"));
			}
			this.heatTooltip.SetText("BodyText", stringBuilder.ToString());
		}

		// Token: 0x06006307 RID: 25351 RVA: 0x002EA858 File Offset: 0x002E8A58
		private void UpdateAlerts()
		{
			if (this.alertWeaponNoAmmo)
			{
				this.warningIcon_None.enabled = true;
			}
			else
			{
				this.warningIcon_None.enabled = false;
			}
			if (this.alertWeaponNoTarget)
			{
				this.warningIcon_Warn.enabled = true;
				this.warningWarnTooltip.SetDelegate("BodyText", () => Loc.T("UI.SpaceCombat.Warning.WeaponNoTarget"));
			}
			else
			{
				this.warningIcon_Warn.enabled = false;
			}
			if (this.alertShipNoThrust)
			{
				if (!this.warningIcon_Alert.enabled)
				{
					Mood.TriggerEvent(Mood.Event.SDKL_AlertEdgesRed);
				}
				this.warningIcon_Alert.enabled = true;
				this.warningAlertTooltip.SetDelegate("BodyText", () => Loc.T("UI.SpaceCombat.Warning.ShipNoThrust"));
			}
			else if (this.alertShipAvoiding)
			{
				this.warningIcon_Alert.enabled = true;
				this.warningAlertTooltip.SetDelegate("BodyText", () => Loc.T("UI.SpaceCombat.Warning.Avoidance"));
			}
			else
			{
				this.warningIcon_Alert.enabled = false;
			}
			bool flag = this.shipState.combatPrimaryTarget == null;
			bool flag2 = false;
			bool flag3 = false;
			if (!flag)
			{
				if (this.shipState.combatPrimaryTarget.GetTargetableState().ref_ship != null)
				{
					flag2 = this.shipState.combatPrimaryTarget.GetTargetableState().ref_ship.ShipDestroyed();
				}
				else if (this.shipState.combatPrimaryTarget.GetTargetableState().ref_habModule != null)
				{
					flag3 = this.shipState.combatPrimaryTarget.GetTargetableState().ref_habModule.destroyed;
				}
			}
			if (flag || flag2 || flag3)
			{
				this.targetingImage.enabled = false;
				this.targetingTooltip.SetText("BodyText", "");
				return;
			}
			TIGameState targetableState = this.shipState.combatPrimaryTarget.GetTargetableState();
			if (targetableState.isHabModuleState)
			{
				TIHabModuleState tihabModuleState = targetableState as TIHabModuleState;
				this.targetingTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipTarget", new object[] { tihabModuleState.displayName }));
			}
			else
			{
				TISpaceShipState tispaceShipState = targetableState as TISpaceShipState;
				this.targetingTooltip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ShipTarget", new object[] { tispaceShipState.displayName }));
			}
			this.targetingImage.enabled = true;
		}

		// Token: 0x06006308 RID: 25352 RVA: 0x002EAAB9 File Offset: 0x002E8CB9
		public void OnShipSystemDamaged(ShipSystemDamageChange e)
		{
			this.UpdateShipRotationStatus();
			this.UpdateShipThrustStatus();
			this.UpdateWeaponOperationalStatus();
			this.UpdateCriticalShipSystemsStatus();
			this.UpdateHeatStatus();
			this.UpdateDamConStatus(false);
		}

		// Token: 0x06006309 RID: 25353 RVA: 0x002EAAE0 File Offset: 0x002E8CE0
		private void OnShipPartDamaged(ShipPartDamageChange e)
		{
			if (e.partData.moduleTemplate is TIDriveTemplate)
			{
				this.UpdateShipThrustStatus();
			}
			if (e.partData.moduleTemplate is TIPowerPlantTemplate)
			{
				this.UpdateShipThrustStatus();
				this.UpdateShipRotationStatus();
				this.UpdateCriticalShipSystemsStatus();
			}
			if (e.partData.moduleTemplate is TIRadiatorTemplate || e.partData.moduleTemplate is TIHeatSinkTemplate)
			{
				this.UpdateHeatStatus();
			}
			if (e.partData.moduleTemplate is TIShipWeaponTemplate)
			{
				this.UpdateWeaponOperationalStatus();
			}
			if (e.partData.moduleTemplate is TIBatteryTemplate)
			{
				this.UpdatePowerSystemStatus();
			}
		}

		// Token: 0x0600630A RID: 25354 RVA: 0x002EAB83 File Offset: 0x002E8D83
		public void OnShipWeaponModeChanged(ShipWeaponModeChanged e)
		{
			this.UpdateWeaponNoTargetAlert();
		}

		// Token: 0x0600630B RID: 25355 RVA: 0x002EAB8B File Offset: 0x002E8D8B
		public void OnShipDisengageChange(ShipDisengageChange e)
		{
		}

		// Token: 0x0600630C RID: 25356 RVA: 0x002EAB8D File Offset: 0x002E8D8D
		public void OnShipPrimaryTargetDestroyed(ShipPrimaryTargetDestroyed e)
		{
			this.UpdateWeaponNoTargetAlert();
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x002EAB95 File Offset: 0x002E8D95
		public void OnShipWeaponFired(ShipWeaponFired e)
		{
			this.UpdateWeaponOperationalStatus();
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x002EAB9D File Offset: 0x002E8D9D
		public void OnShipDamageControlRotationStatusChanged(ShipDamageControlRotationStatusChanged e)
		{
			this.UpdateDamConStatus(!e.damageControlEnabled);
		}

		// Token: 0x0600630F RID: 25359 RVA: 0x002EABB0 File Offset: 0x002E8DB0
		public void OnClickListItem()
		{
			if (!TIGameState.Valid(this.shipState) || this == null || base.gameObject == null)
			{
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CombatFriendlyShipSelect", false, false);
			GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(base.combatantController.GetCombatantState(), false, false), null, Array.Empty<object>());
			if (base.combatantController.GetCombatantType() == IDamageableType.Ship)
			{
				ShipModelController modelController = base.combatantController.ref_shipController.ModelController;
				if (!base.combatantController.UIController().maintainAnimation || !modelController.selectionAnimating)
				{
					base.combatantController.ref_shipController.visualizationController.UIController.UIcanvas.enabled = false;
				}
			}
		}

		// Token: 0x06006310 RID: 25360 RVA: 0x002EAC68 File Offset: 0x002E8E68
		public override void OnDoubleClick()
		{
			this.spaceCombat.combatCamera.LookAtCombatant(base.combatantController);
		}

		// Token: 0x06006311 RID: 25361 RVA: 0x002EAC80 File Offset: 0x002E8E80
		private void OnHeatChange(ShipHeatChange e)
		{
			this.UpdateHeatStatus();
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x002EAC88 File Offset: 0x002E8E88
		public void OnAIControlChanged(ShipAIControlChange e)
		{
			this.SetAIControl(e.AIInControl);
			this.UpdateWeaponNoTargetAlert();
		}

		// Token: 0x06006313 RID: 25363 RVA: 0x002EAC9C File Offset: 0x002E8E9C
		public void SetAIControl(bool AIControlled)
		{
			this.AIControlIcon.enabled = AIControlled;
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x002EACAA File Offset: 0x002E8EAA
		public void OnShipDeltaVChange(ShipDeltaVChange e)
		{
			this.UpdateShipThrustStatus();
		}

		// Token: 0x06006315 RID: 25365 RVA: 0x002EACB4 File Offset: 0x002E8EB4
		public void SetDeltaVValue()
		{
			float num = this.shipState.AvailableDeltaVForCombat_kps();
			string text = TIUtilities.FormatBigOrSmallNumber(num, 1, 1, 0, false, false);
			this.deltaVValue.SetText(Loc.T("UI.SpaceCombat.DeltaVCap", new object[]
			{
				(num <= 0f) ? TIUtilities.RedLine(text) : text,
				TIUtilities.FormatBigOrSmallNumber(base.combatantController.combatMgr.combatState.maxDeltaVAvailableForCombat_kps[this.shipState], 1, 7, 0, false, false)
			}));
			if (this.shipState.AvailableDeltaVForCombat_kps() / base.combatantController.combatMgr.combatState.maxDeltaVAvailableForCombat_kps[this.shipState] <= 0.05f)
			{
				this.deltaVValue.SetText(TIUtilities.RedLine(this.deltaVValue.text));
				return;
			}
			if (this.shipState.AvailableDeltaVForCombat_kps() / base.combatantController.combatMgr.combatState.maxDeltaVAvailableForCombat_kps[this.shipState] <= 0.25f)
			{
				this.deltaVValue.SetText(TIUtilities.YellowLine(this.deltaVValue.text));
			}
		}

		// Token: 0x06006316 RID: 25366 RVA: 0x002EADD2 File Offset: 0x002E8FD2
		public void OnShipPowerStorageChange(ShipPowerSystemsChargeChange e)
		{
			this.UpdatePowerSystemStatus();
		}

		// Token: 0x06006317 RID: 25367 RVA: 0x002EADDC File Offset: 0x002E8FDC
		public void UpdatePowerSystemStatus()
		{
			float availablePowerFraction = this.shipState.availablePowerFraction;
			this.batteryWarningIcon.enabled = availablePowerFraction <= 0.3f;
			if (availablePowerFraction <= 0f)
			{
				this.batteryWarningIcon.color = this.batteryColorDestroyed;
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_battery_destroyed", this.batteryWarningIcon);
				return;
			}
			if (availablePowerFraction <= 0.1f)
			{
				this.batteryWarningIcon.color = this.batteryColorRed;
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_battery_crit", this.batteryWarningIcon);
				return;
			}
			if (availablePowerFraction <= 0.3f)
			{
				this.batteryWarningIcon.color = this.batteryColorYellow;
				GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_battery_low", this.batteryWarningIcon);
				return;
			}
			this.batteryWarningIcon.color = Color.white;
			GameControl.assetLoader.LoadAssetForImageAssignment("ui_spacecombat/ICO_battle_battery", this.batteryWarningIcon);
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x002EAECC File Offset: 0x002E90CC
		private void OnShipCommandExecuted(ShipCommandExecuted e)
		{
			if (e.command is TIShipManeuverCommandTemplate || e.command is RammingSpeedCommand || e.command is CancelRammingSpeedCommand || e.command is DisengageCommand || e.command is CancelDisengageCommand)
			{
				SpaceCombatCanvasController.UpdateManeuverList(this.shipState, base.combatantController.ref_shipController, this.maneuverList);
			}
			if (e.command is SelectTargetCommand || e.command is ClearTargetCommand || e.command is RammingSpeedCommand || e.command is CancelRammingSpeedCommand)
			{
				this.UpdateWeaponNoTargetAlert();
			}
		}

		// Token: 0x06006319 RID: 25369 RVA: 0x002EAF70 File Offset: 0x002E9170
		private void OnCombatManenuverCompleted(CombatManeuverComplete e)
		{
			SpaceCombatCanvasController.UpdateManeuverList(this.shipState, base.combatantController.ref_shipController, this.maneuverList);
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x002EAF90 File Offset: 0x002E9190
		private void OnCollisionAvoidanceActivated(CombatCollisionAvoidanceStatusChange e)
		{
			SpaceCombatCanvasController.UpdateManeuverList(this.shipState, base.combatantController.ref_shipController, this.maneuverList);
			bool flag = this.alertShipAvoiding;
			this.alertShipAvoiding = base.combatantController.ref_shipController.InCollisionAvoidanceManeuver;
			if (flag != this.alertShipAvoiding)
			{
				this.UpdateAlerts();
			}
		}

		// Token: 0x0600631B RID: 25371 RVA: 0x002EAFE3 File Offset: 0x002E91E3
		public void SetGroupSelected(bool value)
		{
			if (value)
			{
				this.frameImage.sprite = this.groupSelectedFrameImage;
				return;
			}
			this.frameImage.sprite = this.defaultFrameImage;
		}

		// Token: 0x0600631C RID: 25372 RVA: 0x002EB00C File Offset: 0x002E920C
		private void SetGroupMembershipString()
		{
			if (base.combatantController.ref_shipController.controlGroups.Count > 0)
			{
				this.groupMembershipString.SetText(base.combatantController.ref_shipController.GetGroupMembershipString());
				this.groupMembershipString.enabled = true;
				return;
			}
			this.groupMembershipString.SetText("");
			this.groupMembershipString.enabled = false;
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x002EB075 File Offset: 0x002E9275
		private void OnUpdateGroupMembershipString(CombatShipGroupChange e)
		{
			this.SetGroupMembershipString();
		}

		// Token: 0x0600631E RID: 25374 RVA: 0x002EB07D File Offset: 0x002E927D
		public void SetPrimarySelected(bool value)
		{
			this.selectedHighlight.enabled = value;
		}

		// Token: 0x0600631F RID: 25375 RVA: 0x002EB08C File Offset: 0x002E928C
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<ShipHeatChange>(new EventManager.EventDelegate<ShipHeatChange>(this.OnHeatChange), null);
			GameControl.eventManager.RemoveListener<ShipAIControlChange>(new EventManager.EventDelegate<ShipAIControlChange>(this.OnAIControlChanged), null);
			GameControl.eventManager.RemoveListener<ShipArmorFacingStruckInCombat>(new EventManager.EventDelegate<ShipArmorFacingStruckInCombat>(base.OnArmorHit), null);
			GameControl.eventManager.RemoveListener<ShipCommandExecuted>(new EventManager.EventDelegate<ShipCommandExecuted>(this.OnShipCommandExecuted), null);
			GameControl.eventManager.RemoveListener<CombatManeuverComplete>(new EventManager.EventDelegate<CombatManeuverComplete>(this.OnCombatManenuverCompleted), null);
			GameControl.eventManager.RemoveListener<CombatCollisionAvoidanceStatusChange>(new EventManager.EventDelegate<CombatCollisionAvoidanceStatusChange>(this.OnCollisionAvoidanceActivated), null);
			GameControl.eventManager.RemoveListener<ShipDeltaVChange>(new EventManager.EventDelegate<ShipDeltaVChange>(this.OnShipDeltaVChange), null);
			GameControl.eventManager.RemoveListener<ShipPowerSystemsChargeChange>(new EventManager.EventDelegate<ShipPowerSystemsChargeChange>(this.OnShipPowerStorageChange), null);
			GameControl.eventManager.RemoveListener<ShipWeaponFired>(new EventManager.EventDelegate<ShipWeaponFired>(this.OnShipWeaponFired), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnShipSystemDamaged), null);
			GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.OnShipPartDamaged), null);
			GameControl.eventManager.RemoveListener<ShipWeaponModeChanged>(new EventManager.EventDelegate<ShipWeaponModeChanged>(this.OnShipWeaponModeChanged), null);
			GameControl.eventManager.RemoveListener<ShipDisengageChange>(new EventManager.EventDelegate<ShipDisengageChange>(this.OnShipDisengageChange), null);
			GameControl.eventManager.RemoveListener<ShipPrimaryTargetDestroyed>(new EventManager.EventDelegate<ShipPrimaryTargetDestroyed>(this.OnShipPrimaryTargetDestroyed), null);
			GameControl.eventManager.RemoveListener<ShipDamageControlRotationStatusChanged>(new EventManager.EventDelegate<ShipDamageControlRotationStatusChanged>(this.OnShipDamageControlRotationStatusChanged), null);
			GameControl.eventManager.RemoveListener<CombatShipGroupChange>(new EventManager.EventDelegate<CombatShipGroupChange>(this.OnUpdateGroupMembershipString), null);
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x002EB209 File Offset: 0x002E9409
		public override void OnDisable()
		{
			this.RemoveListeners();
			base.OnDisable();
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x002EB217 File Offset: 0x002E9417
		public override void OnDestroy()
		{
			this.RemoveListeners();
			base.OnDestroy();
		}

		// Token: 0x040045D2 RID: 17874
		public Image targetingImage;

		// Token: 0x040045D3 RID: 17875
		public Image heatImage;

		// Token: 0x040045D4 RID: 17876
		public Image coolingImage;

		// Token: 0x040045D5 RID: 17877
		public Image AIControlIcon;

		// Token: 0x040045D6 RID: 17878
		public Image DVStaticImage;

		// Token: 0x040045D7 RID: 17879
		public Image warningIcon_None;

		// Token: 0x040045D8 RID: 17880
		public Image warningIcon_Warn;

		// Token: 0x040045D9 RID: 17881
		public Image warningIcon_Alert;

		// Token: 0x040045DA RID: 17882
		public Image rotationStatus_Icon;

		// Token: 0x040045DB RID: 17883
		public Image systemStatus_Icon;

		// Token: 0x040045DC RID: 17884
		public Image weaponStatus_Icon;

		// Token: 0x040045DD RID: 17885
		public Image damConStatus_Icon;

		// Token: 0x040045DE RID: 17886
		public Image damConDisabled_Icon;

		// Token: 0x040045DF RID: 17887
		public Image selectedHighlight;

		// Token: 0x040045E0 RID: 17888
		public Sprite defaultFrameImage;

		// Token: 0x040045E1 RID: 17889
		public Sprite groupSelectedFrameImage;

		// Token: 0x040045E2 RID: 17890
		public TooltipTrigger warningNoAmmoTooltip;

		// Token: 0x040045E3 RID: 17891
		public TooltipTrigger warningWarnTooltip;

		// Token: 0x040045E4 RID: 17892
		public TooltipTrigger warningAlertTooltip;

		// Token: 0x040045E5 RID: 17893
		public TooltipTrigger rotationStatusTooltip;

		// Token: 0x040045E6 RID: 17894
		public TooltipTrigger systemStatusTooltip;

		// Token: 0x040045E7 RID: 17895
		public TooltipTrigger weaponStatusTooltip;

		// Token: 0x040045E8 RID: 17896
		public TooltipTrigger targetingTooltip;

		// Token: 0x040045E9 RID: 17897
		public TooltipTrigger damConTooltip;

		// Token: 0x040045EA RID: 17898
		public TooltipTrigger heatTooltip;

		// Token: 0x040045EB RID: 17899
		public TMP_Text deltaVValue;

		// Token: 0x040045EC RID: 17900
		public ListManagerBase maneuverList;

		// Token: 0x040045ED RID: 17901
		public const float ySize = 106f;

		// Token: 0x040045EE RID: 17902
		public Image batteryWarningIcon;

		// Token: 0x040045EF RID: 17903
		private IWeapon[] noseWeaponComponents;

		// Token: 0x040045F0 RID: 17904
		private IWeapon[] hullWeaponComponents;

		// Token: 0x040045F1 RID: 17905
		public TMP_Text groupMembershipString;

		// Token: 0x040045F2 RID: 17906
		private bool alertWeaponNoAmmo;

		// Token: 0x040045F3 RID: 17907
		private bool alertWeaponNoTarget;

		// Token: 0x040045F4 RID: 17908
		private bool alertShipNoThrust;

		// Token: 0x040045F5 RID: 17909
		private bool alertShipAvoiding;

		// Token: 0x040045F6 RID: 17910
		private bool alertShipDisengaging;

		// Token: 0x040045F7 RID: 17911
		private Color32 batteryColorDestroyed = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

		// Token: 0x040045F8 RID: 17912
		private Color32 batteryColorRed = new Color32(byte.MaxValue, 194, 194, byte.MaxValue);

		// Token: 0x040045F9 RID: 17913
		private Color32 batteryColorYellow = new Color32(236, 236, 0, byte.MaxValue);

		// Token: 0x040045FA RID: 17914
		private Color32 damConYellow = new Color32(byte.MaxValue, 199, 0, byte.MaxValue);

		// Token: 0x040045FB RID: 17915
		private Color32 damConRed = new Color32(byte.MaxValue, 62, 33, byte.MaxValue);
	}
}
