using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;
using ModelShark;
using PavonisInteractive.TerraInvicta.Ship;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.SpaceCombat.UI
{
	// Token: 0x02000A05 RID: 2565
	public abstract class CombatantListItemController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
	{
		// Token: 0x17001105 RID: 4357
		// (get) Token: 0x060062BF RID: 25279 RVA: 0x002E7D13 File Offset: 0x002E5F13
		// (set) Token: 0x060062C0 RID: 25280 RVA: 0x002E7D1B File Offset: 0x002E5F1B
		[HideInInspector]
		public int position { get; set; }

		// Token: 0x17001106 RID: 4358
		// (get) Token: 0x060062C1 RID: 25281 RVA: 0x002E7D24 File Offset: 0x002E5F24
		// (set) Token: 0x060062C2 RID: 25282 RVA: 0x002E7D2C File Offset: 0x002E5F2C
		[HideInInspector]
		public CombatantController combatantController { get; set; }

		// Token: 0x17001107 RID: 4359
		// (get) Token: 0x060062C3 RID: 25283 RVA: 0x002E7D35 File Offset: 0x002E5F35
		// (set) Token: 0x060062C4 RID: 25284 RVA: 0x002E7D3D File Offset: 0x002E5F3D
		public IDamageableType combatantType { get; private set; }

		// Token: 0x060062C5 RID: 25285 RVA: 0x002E7D48 File Offset: 0x002E5F48
		public virtual void Init(SpaceCombatCanvasController masterController, CombatantController combatantController, int position)
		{
			this.RemoveListeners();
			base.name = combatantController.name + " UI";
			this.masterController = masterController;
			this.combatantController = combatantController;
			this.spaceCombat = GameControl.spaceCombat;
			this.combatantType = ((IDamageable)combatantController).damageableType;
			this.position = position;
			this.hitReportObject.SetActive(false);
			this.radiationDamageText.gameObject.SetActive(false);
			this.radiationDamageImage.gameObject.SetActive(false);
			this.critText.gameObject.SetActive(false);
			this.critText.SetText(Loc.T("UI.SpaceCombat.Critical"));
			this.noseArmorImage.color = Color.clear;
			this.portArmorImage.color = Color.clear;
			this.starboardArmorImage.color = Color.clear;
			this.tailArmorImage.color = Color.clear;
			IDamageableType combatantType = this.combatantType;
			if (combatantType != IDamageableType.Ship)
			{
				if (combatantType == IDamageableType.StationModule)
				{
					this.habModuleState = combatantController.ref_habModuleController.habModule;
					this.noseImage.enabled = false;
					this.lateralImage.enabled = true;
					this.tailImage.enabled = false;
					this.radiatorImage.enabled = false;
					this.driveImage.enabled = false;
					this.maneuverTargetImage.enabled = false;
					this.personnelIconGrid.SetListSize<ShipPersonnelGridItemController>(0, false, false);
					this.personnelIconGrid.gameObject.SetActive(false);
					this.shipName.SetText(this.habModuleState.displayName);
					this.className.SetText(this.habModuleState.ref_hab.displayName);
					GameControl.eventManager.AddListener<HabModuleDamagedInCombat>(new EventManager.EventDelegate<HabModuleDamagedInCombat>(this.OnHabModuleHit), null, this.habModuleState, false, false);
					this.shipSummaryTip.SetDelegate("BodyText", () => this.habModuleState.GetCombatSummary());
					this.shipSummaryTip.enabled = true;
					this.shipSummaryTip.minTextWidth = 0;
					this.rawDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildRawDamageTooltip());
					this.absorbedDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildAbsorbedDamageTooltip());
					this.penetratedDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildPenetratedDamageTooltip());
					this.radiationDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildRadiationDamageTooltip());
				}
			}
			else
			{
				this.shipState = combatantController.ref_shipController.ShipState;
				this.noseImage.enabled = true;
				this.lateralImage.enabled = true;
				this.tailImage.enabled = true;
				this.radiatorImage.enabled = true;
				this.driveImage.enabled = true;
				this.maneuverTargetImage.enabled = false;
				GameControl.assetLoader.LoadAssetForImageAssignment(this.shipState.hull.combatUINoseArmorPath_OK(this.shipState.template.GetHullAppearanceIndex), this.noseArmorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.shipState.hull.combatUIPortArmorPath_OK(this.shipState.template.GetHullAppearanceIndex), this.portArmorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.shipState.hull.combatUIStarboardArmorPath_OK(this.shipState.template.GetHullAppearanceIndex), this.starboardArmorImage);
				GameControl.assetLoader.LoadAssetForImageAssignment(this.shipState.hull.combatUITailArmorPath_OK(this.shipState.template.GetHullAppearanceIndex), this.tailArmorImage);
				GameControl.eventManager.AddListener<CompleteExtendRadiatorsEvent>(new EventManager.EventDelegate<CompleteExtendRadiatorsEvent>(this.SetRadiatorImageOn), null, this.shipState, true, false);
				GameControl.eventManager.AddListener<CompleteRetractRadiatorsEvent>(new EventManager.EventDelegate<CompleteRetractRadiatorsEvent>(this.SetRadiatorImageOff), null, this.shipState, true, false);
				GameControl.eventManager.AddListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.OnShipPartDamageChange), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnShipSystemDamageChange), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipArmorFacingStruckInCombat>(new EventManager.EventDelegate<ShipArmorFacingStruckInCombat>(this.OnArmorHit), null, this.shipState, false, false);
				GameControl.eventManager.AddListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.OnShipOfficerKilled), null, this.shipState, true, false);
				this.shipName.SetText(new StringBuilder(this.shipState.GetDisplayName(GameControl.control.activePlayer)));
				if (this.shipState.hull.noShipyardBuild)
				{
					if (this.shipState.isAlien)
					{
						this.className.SetText(Loc.T("UI.Precombat.AlienExofighter"));
					}
					else
					{
						this.className.SetText(Loc.T("UI.Precombat.Exofighter"));
					}
				}
				else
				{
					this.className.SetText(new StringBuilder(this.shipState.template.className).Append(" ").Append(this.shipState.hull.displayName));
				}
				bool hideAlienData = this.shipState.isAlien && !GameControl.control.activePlayer.finishedProjectNames.Contains("Project_TheirWarships");
				this.shipSummaryTip.SetDelegate("BodyText", () => this.shipState.template.quickSummary(hideAlienData, this.shipState, false, false, false));
				this.shipSummaryTip.enabled = true;
				this.shipSummaryTip.minTextWidth = this._shipTooltipMinWidth;
				this.rawDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildRawDamageTooltip());
				this.absorbedDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildAbsorbedDamageTooltip());
				this.penetratedDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildPenetratedDamageTooltip());
				this.radiationDamageTip.SetDelegate("BodyText", () => CombatantListItemController.BuildRadiationDamageTooltip());
			}
			this.UpdateListItem();
		}

		// Token: 0x060062C6 RID: 25286 RVA: 0x002E83C3 File Offset: 0x002E65C3
		public void UpdateListItem()
		{
			if (this.combatantType == IDamageableType.Ship)
			{
				this.UpdateShipListItem();
				return;
			}
			this.UpdateHabModuleListItem();
		}

		// Token: 0x060062C7 RID: 25287 RVA: 0x002E83DA File Offset: 0x002E65DA
		public void OnArmorHit(ShipArmorFacingStruckInCombat e)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				this.OnArmorHit(e.armorFacing, e.rawDamage, e.penetratedDamage, e.radiationDamage);
			}
		}

		// Token: 0x060062C8 RID: 25288 RVA: 0x002E8418 File Offset: 0x002E6618
		public virtual void OnArmorHit(ArmorFacing facing, float rawDamage, float penetratedDamage, float radiationDamage)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				IEnumerator enumerator = this.ShowHitData(rawDamage, penetratedDamage, radiationDamage);
				base.StartCoroutine(enumerator);
				Color color = ((rawDamage > penetratedDamage) ? Color.red : Color.yellow);
				switch (facing)
				{
				case ArmorFacing.Nose:
				{
					IEnumerator enumerator2 = this.FlashArmorFacing(this.noseArmorImage, color);
					base.StartCoroutine(enumerator2);
					return;
				}
				case ArmorFacing.Right:
				{
					IEnumerator enumerator2 = this.FlashArmorFacing(this.starboardArmorImage, color);
					base.StartCoroutine(enumerator2);
					break;
				}
				case ArmorFacing.Left:
				{
					IEnumerator enumerator2 = this.FlashArmorFacing(this.portArmorImage, color);
					base.StartCoroutine(enumerator2);
					return;
				}
				case ArmorFacing.Dorsal:
				case ArmorFacing.Ventral:
					break;
				case ArmorFacing.Tail:
				{
					IEnumerator enumerator2 = this.FlashArmorFacing(this.tailArmorImage, color);
					base.StartCoroutine(enumerator2);
					return;
				}
				default:
					return;
				}
			}
		}

		// Token: 0x060062C9 RID: 25289 RVA: 0x002E84E4 File Offset: 0x002E66E4
		private IEnumerator FlashArmorFacing(Image facing, Color color)
		{
			facing.color = color;
			yield return CombatantListItemController.delay8;
			while (this.spaceCombat.combatHUD.clockController.IsPaused)
			{
				yield return null;
			}
			facing.color = Color.clear;
			yield break;
		}

		// Token: 0x060062CA RID: 25290 RVA: 0x002E8501 File Offset: 0x002E6701
		private IEnumerator ShowHitData(float rawDamage, float penetratedDamage, float radiationDamage)
		{
			float num = rawDamage - penetratedDamage;
			this.rawDamageTxt.SetText(TIUtilities.FormatSmallNumber(rawDamage, 1, 0, true, false));
			this.absorbedDamageTxt.SetText(TIUtilities.FormatSmallNumber(num, 1, 0, true, false));
			this.penetratedDamageTxt.SetText(TIUtilities.FormatSmallNumber(penetratedDamage, 7, 0, true, false), 1f);
			if (num < 0f)
			{
				this.absorbedDamageTxt.color = this.redDamageColor;
				this.critText.gameObject.SetActive(true);
			}
			else
			{
				this.absorbedDamageTxt.color = this.normalDamageColor;
				this.critText.gameObject.SetActive(false);
			}
			if (radiationDamage > 0f)
			{
				this.radiationDamageText.SetText(TIUtilities.FormatSmallNumber(radiationDamage, 7, 0, true, false), 0f);
				this.radiationDamageImage.gameObject.SetActive(true);
				this.radiationDamageText.gameObject.SetActive(true);
				this.critText.gameObject.SetActive(false);
			}
			else
			{
				this.radiationDamageImage.gameObject.SetActive(false);
				this.radiationDamageText.gameObject.SetActive(false);
			}
			this.hitReportObject.SetActive(true);
			yield return CombatantListItemController.delay8;
			while (this.spaceCombat.combatHUD.clockController.IsPaused)
			{
				yield return null;
			}
			this.hitReportObject.SetActive(false);
			yield break;
		}

		// Token: 0x060062CB RID: 25291 RVA: 0x002E8528 File Offset: 0x002E6728
		public virtual void OnHabModuleHit(HabModuleDamagedInCombat e)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				IEnumerator enumerator = this.ShowHitData(e.rawDamage, e.absorbedDamage, 0f);
				if (enumerator != null)
				{
					base.StartCoroutine(enumerator);
				}
				else if (this.combatantController != null && this.combatantController.ref_habModuleController != null)
				{
					Debug.Log("Error - Hit Text Coroutine is null. \nCombatant: " + this.combatantController.ref_habModuleController.habModule.displayName + "\nCombatant Destroyed?: " + this.combatantController.isDestroyed.ToString());
				}
				else
				{
					Debug.Log("Error - Hit Text Coroutine is null. ");
				}
				this.UpdateHabModuleListItem();
			}
		}

		// Token: 0x060062CC RID: 25292 RVA: 0x002E85EC File Offset: 0x002E67EC
		private void SetRadiatorImageOn(CompleteExtendRadiatorsEvent e)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy && TIGameState.Valid(e.ship) && !e.ship.IsAlien() && !e.ship.hull.simpleHull)
			{
				CombatantListItemController.SetRadiatorImageOn(this.shipState, this.radiatorImage);
			}
		}

		// Token: 0x060062CD RID: 25293 RVA: 0x002E8654 File Offset: 0x002E6854
		private static void SetRadiatorImageOn(TISpaceShipState shipState, Image radiatorImage)
		{
			if (!shipState.hull.simpleHull)
			{
				if (shipState.PartDestroyed(shipState.radiatorModule) || shipState.ShipDestroyed())
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.radiators.combatUIPath_On_Destroyed(shipState.hull, shipState.template.GetHullAppearanceIndex), radiatorImage);
				}
				else if (shipState.PartDamagedButNotDestroyed(shipState.radiatorModule))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.radiators.combatUIPath_On_Damaged(shipState.hull, shipState.template.GetHullAppearanceIndex), radiatorImage);
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.radiators.combatUIPath_On_OK(shipState.hull, shipState.template.GetHullAppearanceIndex), radiatorImage);
				}
				radiatorImage.gameObject.SetActive(true);
				return;
			}
			radiatorImage.gameObject.SetActive(false);
		}

		// Token: 0x060062CE RID: 25294 RVA: 0x002E8728 File Offset: 0x002E6928
		private void SetRadiatorImageOff(CompleteRetractRadiatorsEvent e)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy && TIGameState.Valid(e.ship) && !e.ship.IsAlien() && !e.ship.hull.simpleHull)
			{
				CombatantListItemController.SetRadiatorImageOff(this.shipState, this.radiatorImage);
			}
		}

		// Token: 0x060062CF RID: 25295 RVA: 0x002E8790 File Offset: 0x002E6990
		private static void SetRadiatorImageOff(TISpaceShipState shipState, Image radiatorImage)
		{
			if (!shipState.hull.simpleHull)
			{
				if (shipState.PartDestroyed(shipState.radiatorModule) || shipState.ShipDestroyed())
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.radiators.combatUIPath_Off_Destroyed(shipState.hull, shipState.template.GetHullAppearanceIndex), radiatorImage);
				}
				else if (shipState.PartDamaged(shipState.radiatorModule))
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.radiators.combatUIPath_Off_Damaged(shipState.hull, shipState.template.GetHullAppearanceIndex), radiatorImage);
				}
				else
				{
					GameControl.assetLoader.LoadAssetForImageAssignment(shipState.radiators.combatUIPath_Off_OK(shipState.hull, shipState.template.GetHullAppearanceIndex), radiatorImage);
				}
				radiatorImage.gameObject.SetActive(true);
				return;
			}
			radiatorImage.gameObject.SetActive(false);
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x002E8864 File Offset: 0x002E6A64
		private void OnShipPartDamageChange(ShipPartDamageChange e)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				if (!e.ship.isAlien && e.partData.moduleTemplate is TIRadiatorTemplate)
				{
					CombatantListItemController.SetRadiatorImage(this.shipState, this.radiatorImage);
				}
				if (!e.ship.isAlien && e.partData.moduleTemplate is TIDriveTemplate)
				{
					CombatantListItemController.SetDriveImage(this.shipState, this.driveImage);
				}
			}
		}

		// Token: 0x060062D1 RID: 25297 RVA: 0x002E88EC File Offset: 0x002E6AEC
		public static void SetRadiatorImage(TISpaceShipTemplate template, Image radiatorImage)
		{
			if (template.hullTemplate.simpleHull)
			{
				radiatorImage.gameObject.SetActive(false);
				return;
			}
			if (radiatorImage == null)
			{
				Debug.LogError(template.displayName + " Tried to set radiator Image but Tail Image is Null.");
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(template.radiatorTemplate.combatUIPath_On_OK(template.hullTemplate, template.GetHullAppearanceIndex), radiatorImage);
			radiatorImage.gameObject.SetActive(true);
		}

		// Token: 0x060062D2 RID: 25298 RVA: 0x002E8960 File Offset: 0x002E6B60
		public static void SetRadiatorImage(TISpaceShipState shipState, Image radiatorImage)
		{
			if (radiatorImage == null)
			{
				Debug.LogError(shipState.displayName + " Tried to set radiator Image but Tail Image is Null.");
				return;
			}
			if (shipState.radiatorsExtended)
			{
				CombatantListItemController.SetRadiatorImageOn(shipState, radiatorImage);
				return;
			}
			CombatantListItemController.SetRadiatorImageOff(shipState, radiatorImage);
		}

		// Token: 0x060062D3 RID: 25299 RVA: 0x002E8998 File Offset: 0x002E6B98
		public static void SetDriveImage(TISpaceShipTemplate template, Image driveImage)
		{
			if (driveImage == null)
			{
				Debug.LogError(template.displayName + " Tried to set drive Image but Image is Null.");
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(template.driveTemplate.combatUIPath_OK(template.hullTemplate, template.GetHullAppearanceIndex), driveImage);
		}

		// Token: 0x060062D4 RID: 25300 RVA: 0x002E89E8 File Offset: 0x002E6BE8
		public static void SetDriveImage(TISpaceShipState shipState, Image driveImage)
		{
			if (driveImage == null)
			{
				Debug.LogError(shipState.displayName + " Tried to set drive Image but Image is Null.");
				return;
			}
			if (shipState.isAlien)
			{
				return;
			}
			if (shipState.hull.simpleHull)
			{
				driveImage.gameObject.SetActive(false);
				return;
			}
			if (shipState.PartDestroyed(shipState.driveModule) || shipState.PartDestroyed(shipState.powerPlantModule) || shipState.SystemDestroyed(ShipSystem.DriveCoupling) || shipState.ShipDestroyed())
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(shipState.drive.combatUIPath_Destroyed(shipState.hull, shipState.template.GetHullAppearanceIndex), driveImage);
			}
			else if (shipState.PartDamagedButNotDestroyed(shipState.driveModule) || shipState.PartDamagedButNotDestroyed(shipState.powerPlantModule) || shipState.SystemDamagedButNotDestroyed(ShipSystem.DriveCoupling))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(shipState.drive.combatUIPath_Damaged(shipState.hull, shipState.template.GetHullAppearanceIndex), driveImage);
			}
			else
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(shipState.drive.combatUIPath_OK(shipState.hull, shipState.template.GetHullAppearanceIndex), driveImage);
			}
			driveImage.gameObject.SetActive(true);
		}

		// Token: 0x060062D5 RID: 25301 RVA: 0x002E8B10 File Offset: 0x002E6D10
		public static void SetHabImage(TIHabState habState, Image habImage)
		{
			if (habImage == null)
			{
				Debug.LogError(habState.displayName + " Tried to set hab Image but Image is Null.");
				return;
			}
			TIHabModuleState tihabModuleState = null;
			bool flag = false;
			int num = 0;
			int num2 = 0;
			foreach (TIHabModuleState tihabModuleState2 in habState.AllModuleStates())
			{
				if (tihabModuleState2.isCombatModule || (tihabModuleState2.destroyed && tihabModuleState2.priorModuleTemplate.spaceCombatModule))
				{
					num++;
					if (tihabModuleState2.destroyed)
					{
						flag = true;
						num2++;
					}
					if (tihabModuleState == null)
					{
						tihabModuleState = tihabModuleState2;
					}
					else if (tihabModuleState.SpaceCombatValue() < tihabModuleState2.SpaceCombatValue())
					{
						tihabModuleState = tihabModuleState2;
					}
				}
			}
			if (tihabModuleState == null)
			{
				habImage.enabled = false;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(CombatantListItemController.CombatModuleUIPath(tihabModuleState));
			if (num == num2)
			{
				Color color = new Color(0.7f, 0.7f, 0.7f, 1f);
				habImage.color = color;
				stringBuilder.Append("C");
			}
			else if (flag)
			{
				habImage.color = Color.white;
				stringBuilder.Append("B");
			}
			else
			{
				habImage.color = Color.white;
				stringBuilder.Append("A");
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(stringBuilder.ToString(), habImage);
		}

		// Token: 0x060062D6 RID: 25302 RVA: 0x002E8C74 File Offset: 0x002E6E74
		private void OnShipSystemDamageChange(ShipSystemDamageChange e)
		{
			if (base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				ShipSystem system = e.system;
				switch (system)
				{
				case ShipSystem.NoseStructure:
					CombatantListItemController.SetNoseImage(this.shipState, this.noseImage);
					return;
				case ShipSystem.CentralStructure:
					CombatantListItemController.SetMidImage(this.shipState, this.lateralImage);
					return;
				case ShipSystem.TailStructure:
					CombatantListItemController.SetTailImage(this.shipState, this.tailImage);
					return;
				default:
					if (system != ShipSystem.DriveCoupling)
					{
						return;
					}
					CombatantListItemController.SetDriveImage(this.shipState, this.driveImage);
					break;
				}
			}
		}

		// Token: 0x060062D7 RID: 25303 RVA: 0x002E8D03 File Offset: 0x002E6F03
		public static void SetNoseImage(TISpaceShipTemplate template, Image noseImage)
		{
			if (noseImage == null)
			{
				Debug.LogError(template.displayName + " Tried to set nose Image but Image is Null.");
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(template.hullTemplate.combatUINosePath_OK(template.GetHullAppearanceIndex), noseImage);
		}

		// Token: 0x060062D8 RID: 25304 RVA: 0x002E8D40 File Offset: 0x002E6F40
		public static void SetNoseImage(TISpaceShipState shipState, Image noseImage)
		{
			if (noseImage == null)
			{
				Debug.LogError(shipState.displayName + " Tried to set nose Image but Image is Null.");
				return;
			}
			string text = shipState.hull.combatUINosePath_OK(shipState.template.GetHullAppearanceIndex);
			if (shipState.SystemDestroyed(ShipSystem.NoseStructure))
			{
				text = shipState.hull.combatUINosePath_Destroyed(shipState.template.GetHullAppearanceIndex);
			}
			else if (shipState.SystemDamaged(ShipSystem.NoseStructure))
			{
				text = shipState.hull.combatUINosePath_Damaged(shipState.template.GetHullAppearanceIndex);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, noseImage);
		}

		// Token: 0x060062D9 RID: 25305 RVA: 0x002E8DD1 File Offset: 0x002E6FD1
		public static void SetMidImage(TISpaceShipTemplate template, Image lateralImage)
		{
			if (lateralImage == null)
			{
				Debug.LogError(template.displayName + " Tried to set mid Image but mid Image is Null.");
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(template.hullTemplate.combatUIMidPath_OK(template.GetHullAppearanceIndex), lateralImage);
		}

		// Token: 0x060062DA RID: 25306 RVA: 0x002E8E10 File Offset: 0x002E7010
		public static void SetMidImage(CombatHabModuleController controller, Image lateralImage)
		{
			if (lateralImage == null)
			{
				Debug.LogError(controller.habModule.displayName + " Tried to set mid Image but mid Image is Null.");
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(CombatantListItemController.CombatModuleUIPath(controller.habModule));
			if (controller.hitPoints < controller.baseHitPoints)
			{
				if (controller.hitPoints < controller.baseHitPoints / 2f)
				{
					stringBuilder.Append("C");
				}
				else
				{
					stringBuilder.Append("B");
				}
			}
			else
			{
				stringBuilder.Append("A");
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(stringBuilder.ToString(), lateralImage);
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x002E8EB0 File Offset: 0x002E70B0
		public static void SetMidImage(TISpaceShipState shipState, Image lateralImage)
		{
			if (lateralImage == null)
			{
				Debug.LogError(shipState.displayName + " Tried to set mid Image but Image is Null.");
				return;
			}
			string text = shipState.hull.combatUIMidPath_OK(shipState.template.GetHullAppearanceIndex);
			if (shipState.SystemDestroyed(ShipSystem.CentralStructure))
			{
				text = shipState.hull.combatUIMidPath_Destroyed(shipState.template.GetHullAppearanceIndex);
			}
			else if (shipState.SystemDamaged(ShipSystem.CentralStructure))
			{
				text = shipState.hull.combatUIMidPath_Damaged(shipState.template.GetHullAppearanceIndex);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, lateralImage);
		}

		// Token: 0x060062DC RID: 25308 RVA: 0x002E8F41 File Offset: 0x002E7141
		public static void SetTailImage(TISpaceShipTemplate template, Image tailImage)
		{
			if (tailImage == null)
			{
				Debug.LogError(template.displayName + " Tried to set Tail Image but Tail Image is Null.");
				return;
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(template.hullTemplate.combatUITailPath_OK(template.GetHullAppearanceIndex), tailImage);
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x002E8F80 File Offset: 0x002E7180
		public static void SetTailImage(TISpaceShipState shipState, Image tailImage)
		{
			if (tailImage == null)
			{
				Debug.LogError(shipState.displayName + " Tried to set Tail Image but Tail Image is Null.");
				return;
			}
			string text = shipState.hull.combatUITailPath_OK(shipState.template.GetHullAppearanceIndex);
			if (shipState.SystemDestroyed(ShipSystem.TailStructure))
			{
				text = shipState.hull.combatUITailPath_Destroyed(shipState.template.GetHullAppearanceIndex);
			}
			else if (shipState.SystemDamaged(ShipSystem.TailStructure))
			{
				text = shipState.hull.combatUITailPath_Damaged(shipState.template.GetHullAppearanceIndex);
			}
			GameControl.assetLoader.LoadAssetForImageAssignment(text, tailImage);
		}

		// Token: 0x060062DE RID: 25310 RVA: 0x002E9011 File Offset: 0x002E7211
		public void OnShipOfficerKilled(ShipOfficerKilled e)
		{
			if (TIGameState.Valid(this.shipState) && this != null && base.gameObject != null && base.gameObject.activeInHierarchy)
			{
				this.UpdatePersonnelList();
			}
		}

		// Token: 0x060062DF RID: 25311 RVA: 0x002E904C File Offset: 0x002E724C
		public void UpdatePersonnelList()
		{
			TIFactionState activePlayer = GameControl.control.activePlayer;
			List<TIOfficerState> list = new List<TIOfficerState>();
			List<TIOfficerState> list2 = new List<TIOfficerState>();
			if (activePlayer == this.shipState.faction)
			{
				list = new List<TIOfficerState>(this.shipState.officers);
				list2 = new List<TIOfficerState>();
				if (GameControl.spaceCombat.combatState.deadOfficers.ContainsKey(this.shipState))
				{
					list2 = GameControl.spaceCombat.combatState.deadOfficers[this.shipState].ToList<TIOfficerState>();
					list.AddRange(list2);
				}
				list = list.OrderBy<TIOfficerState, int>((TIOfficerState x) => x.template.sortOrder).ToList<TIOfficerState>();
			}
			List<TICouncilorState> list3 = this.shipState.CouncilorStatesPresentAndKnownToFaction(activePlayer);
			List<CouncilorView> list4 = new List<CouncilorView>();
			foreach (TICouncilorState ticouncilorState in list3)
			{
				list4.Add(activePlayer.GetViewofCouncilor(ticouncilorState));
			}
			int num = 0;
			int num2 = list4.Count + list.Count;
			this.personnelIconGrid.SetListSize<ShipPersonnelGridItemController>(num2, false, false);
			using (IEnumerator<object> enumerator2 = this.personnelIconGrid.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (CombatantListItemController.<>o__78.<>p__0 == null)
					{
						CombatantListItemController.<>o__78.<>p__0 = CallSite<Func<CallSite, object, ShipPersonnelGridItemController>>.Create(Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(ShipPersonnelGridItemController), typeof(CombatantListItemController)));
					}
					ShipPersonnelGridItemController shipPersonnelGridItemController = CombatantListItemController.<>o__78.<>p__0.Target(CombatantListItemController.<>o__78.<>p__0, enumerator2.Current);
					if (num < list4.Count)
					{
						shipPersonnelGridItemController.UpdateGridItem(list4[num]);
					}
					else
					{
						int num3 = num - list4.Count;
						shipPersonnelGridItemController.UpdateGridItem(list[num3], list2.Contains(list[num3]));
					}
					num++;
				}
			}
			GridLayoutGroup component = this.personnelIconGrid.GetComponent<GridLayoutGroup>();
			if (num2 > 6)
			{
				float num4 = component.cellSize.x - component.cellSize.x * (float)num2 / 6f;
				component.spacing = new Vector2(num4, 0f);
				return;
			}
			this.personnelIconGrid.GetComponent<GridLayoutGroup>().spacing = new Vector2(0f, 0f);
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x002E92BC File Offset: 0x002E74BC
		protected virtual void UpdateShipListItem()
		{
			CombatantListItemController.SetNoseImage(this.shipState, this.noseImage);
			CombatantListItemController.SetMidImage(this.shipState, this.lateralImage);
			CombatantListItemController.SetTailImage(this.shipState, this.tailImage);
			if (!this.shipState.hull.alien)
			{
				CombatantListItemController.SetRadiatorImage(this.shipState, this.radiatorImage);
				CombatantListItemController.SetDriveImage(this.shipState, this.driveImage);
				this.radiatorImage.enabled = true;
				this.driveImage.enabled = true;
			}
			else
			{
				this.radiatorImage.enabled = false;
				this.driveImage.enabled = false;
			}
			this.UpdatePersonnelList();
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x002E9368 File Offset: 0x002E7568
		protected virtual void UpdateHabModuleListItem()
		{
			CombatantListItemController.SetMidImage(this.combatantController.ref_habModuleController, this.lateralImage);
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x002E9380 File Offset: 0x002E7580
		private static string CombatModuleUIPath(TIHabModuleState habModule)
		{
			if (habModule.IsAlien())
			{
				switch (habModule.moduleTemplate.tier)
				{
				case 1:
					return "ui_spacecombat/OBJ_battle_alien_T1_point_defense_array_small_";
				case 2:
					return "ui_spacecombat/OBJ_battle_alien_T2_layered_defense_array_small_";
				case 3:
					return "ui_spacecombat/OBJ_battle_alien_T3_battlestations_small_";
				}
			}
			else
			{
				switch (habModule.moduleTemplate.tier)
				{
				case 1:
					return "ui_spacecombat/OBJ_battle_earth_T1_point_defense_array_small_";
				case 2:
					return "ui_spacecombat/OBJ_battle_earth_T2_layered_defense_array_small_";
				case 3:
					return "ui_spacecombat/OBJ_battle_earth_T3_battlestations_small_";
				}
			}
			return string.Empty;
		}

		// Token: 0x060062E3 RID: 25315 RVA: 0x002E9402 File Offset: 0x002E7602
		public static string BuildRawDamageTooltip()
		{
			return new StringBuilder(Loc.T("UI.SpaceCombat.RawDamage")).ToString().Trim();
		}

		// Token: 0x060062E4 RID: 25316 RVA: 0x002E941D File Offset: 0x002E761D
		public static string BuildAbsorbedDamageTooltip()
		{
			return new StringBuilder(Loc.T("UI.SpaceCombat.AbsorbedDamage")).ToString().Trim();
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x002E9438 File Offset: 0x002E7638
		public static string BuildPenetratedDamageTooltip()
		{
			return new StringBuilder(Loc.T("UI.SpaceCombat.PenetratedDamage")).ToString().Trim();
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x002E9453 File Offset: 0x002E7653
		public static string BuildRadiationDamageTooltip()
		{
			return new StringBuilder(Loc.T("UI.SpaceCombat.RadiationDamage")).ToString().Trim();
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x002E9470 File Offset: 0x002E7670
		public void OnManeuverTargetSelected()
		{
			if (this.masterController.selectedFriendlyShip != null && this.masterController.selectedFriendlyShip.maneuverTarget != null)
			{
				this.maneuverTargetImage.enabled = this.masterController.selectedFriendlyShip.maneuverTarget == this.combatantController;
				this.maneuverTargetTip.SetText("BodyText", Loc.T("UI.SpaceCombat.Warning.ManeuverTarget", new object[] { this.masterController.selectedFriendlyShip.ShipState.displayName }));
			}
		}

		// Token: 0x060062E8 RID: 25320 RVA: 0x002E9506 File Offset: 0x002E7706
		public void ClearManeuverTarget()
		{
			this.maneuverTargetImage.enabled = false;
		}

		// Token: 0x060062E9 RID: 25321 RVA: 0x002E9514 File Offset: 0x002E7714
		public void OnPointerClick(PointerEventData e)
		{
			this._clickCount += 1f;
			if (this._clickCount == 1f)
			{
				this._lastClickTime = Time.time;
			}
			if (this._clickCount > 1f && Time.time - this._lastClickTime <= this._doubleClickWindow)
			{
				this._clickCount = 0f;
				this._lastClickTime = 0f;
				this.OnDoubleClick();
				return;
			}
			this._clickCount = 1f;
			this._lastClickTime = Time.time;
		}

		// Token: 0x060062EA RID: 25322 RVA: 0x002E959F File Offset: 0x002E779F
		public virtual void OnDoubleClick()
		{
		}

		// Token: 0x060062EB RID: 25323 RVA: 0x002E95A4 File Offset: 0x002E77A4
		public void OnPointerEnter(PointerEventData e)
		{
			if (this.combatantController.GetCombatantType() == IDamageableType.Ship)
			{
				ShipModelController modelController = this.combatantController.ref_shipController.ModelController;
				if (!this.combatantController.UIController().maintainAnimation || !modelController.selectionAnimating)
				{
					modelController.StartSelectionAnimation();
				}
				if (this.combatantController.ref_shipController.visualizationController.UIController.UIcanvas != null)
				{
					this.combatantController.ref_shipController.visualizationController.UIController.UIcanvas.enabled = true;
					return;
				}
			}
			else if (this.combatantController.GetCombatantType() == IDamageableType.StationModule)
			{
				HabModuleUIElementController habModuleUIElementController = (HabModuleUIElementController)this.combatantController.ref_habModuleController.UIController();
				if (habModuleUIElementController.canvas != null)
				{
					habModuleUIElementController.canvas.enabled = true;
				}
			}
		}

		// Token: 0x060062EC RID: 25324 RVA: 0x002E9670 File Offset: 0x002E7870
		public void OnPointerExit(PointerEventData e)
		{
			if (this.combatantController.GetCombatantType() == IDamageableType.Ship)
			{
				if (!this.combatantController.UIController().maintainAnimation)
				{
					this.combatantController.ref_shipController.ModelController.StopSelectionAnimation();
				}
				if (this.combatantController.ref_shipController.visualizationController.UIController.UIcanvas != null)
				{
					this.combatantController.ref_shipController.visualizationController.UIController.UIcanvas.enabled = false;
					return;
				}
			}
			else if (this.combatantController.GetCombatantType() == IDamageableType.StationModule)
			{
				HabModuleUIElementController habModuleUIElementController = (HabModuleUIElementController)this.combatantController.ref_habModuleController.UIController();
				if (habModuleUIElementController.canvas != null)
				{
					habModuleUIElementController.canvas.enabled = false;
				}
			}
		}

		// Token: 0x060062ED RID: 25325 RVA: 0x002E9734 File Offset: 0x002E7934
		private void RemoveListeners()
		{
			GameControl.eventManager.RemoveListener<CompleteExtendRadiatorsEvent>(new EventManager.EventDelegate<CompleteExtendRadiatorsEvent>(this.SetRadiatorImageOn), null);
			GameControl.eventManager.RemoveListener<CompleteRetractRadiatorsEvent>(new EventManager.EventDelegate<CompleteRetractRadiatorsEvent>(this.SetRadiatorImageOff), null);
			GameControl.eventManager.RemoveListener<ShipPartDamageChange>(new EventManager.EventDelegate<ShipPartDamageChange>(this.OnShipPartDamageChange), null);
			GameControl.eventManager.RemoveListener<ShipSystemDamageChange>(new EventManager.EventDelegate<ShipSystemDamageChange>(this.OnShipSystemDamageChange), null);
			GameControl.eventManager.RemoveListener<ShipArmorFacingStruckInCombat>(new EventManager.EventDelegate<ShipArmorFacingStruckInCombat>(this.OnArmorHit), null);
			GameControl.eventManager.RemoveListener<HabModuleDamagedInCombat>(new EventManager.EventDelegate<HabModuleDamagedInCombat>(this.OnHabModuleHit), null);
			GameControl.eventManager.RemoveListener<ShipOfficerKilled>(new EventManager.EventDelegate<ShipOfficerKilled>(this.OnShipOfficerKilled), null);
		}

		// Token: 0x060062EE RID: 25326 RVA: 0x002E97E3 File Offset: 0x002E79E3
		public virtual void OnDisable()
		{
			this.RemoveListeners();
		}

		// Token: 0x060062EF RID: 25327 RVA: 0x002E97EB File Offset: 0x002E79EB
		public virtual void OnDestroy()
		{
			base.StopAllCoroutines();
			this.RemoveListeners();
		}

		// Token: 0x040045A2 RID: 17826
		protected SpaceCombatCanvasController masterController;

		// Token: 0x040045A3 RID: 17827
		protected SpaceCombatManager spaceCombat;

		// Token: 0x040045A6 RID: 17830
		public TMP_Text shipName;

		// Token: 0x040045A7 RID: 17831
		public TMP_Text className;

		// Token: 0x040045A8 RID: 17832
		public Image noseImage;

		// Token: 0x040045A9 RID: 17833
		public Image lateralImage;

		// Token: 0x040045AA RID: 17834
		public Image tailImage;

		// Token: 0x040045AB RID: 17835
		public Image driveImage;

		// Token: 0x040045AC RID: 17836
		public Image radiatorImage;

		// Token: 0x040045AD RID: 17837
		public Image noseArmorImage;

		// Token: 0x040045AE RID: 17838
		public Image portArmorImage;

		// Token: 0x040045AF RID: 17839
		public Image starboardArmorImage;

		// Token: 0x040045B0 RID: 17840
		public Image tailArmorImage;

		// Token: 0x040045B1 RID: 17841
		public Button button;

		// Token: 0x040045B2 RID: 17842
		public Image frameImage;

		// Token: 0x040045B3 RID: 17843
		public ListManagerBase personnelIconGrid;

		// Token: 0x040045B4 RID: 17844
		public GameObject hitReportObject;

		// Token: 0x040045B5 RID: 17845
		public TMP_Text rawDamageTxt;

		// Token: 0x040045B6 RID: 17846
		public TMP_Text absorbedDamageTxt;

		// Token: 0x040045B7 RID: 17847
		public TMP_Text penetratedDamageTxt;

		// Token: 0x040045B8 RID: 17848
		public TMP_Text radiationDamageText;

		// Token: 0x040045B9 RID: 17849
		public TMP_Text critText;

		// Token: 0x040045BA RID: 17850
		public GameObject highlightObject;

		// Token: 0x040045BB RID: 17851
		public Image radiationDamageImage;

		// Token: 0x040045BC RID: 17852
		public Image maneuverTargetImage;

		// Token: 0x040045BD RID: 17853
		public Color redDamageColor;

		// Token: 0x040045BE RID: 17854
		public Color normalDamageColor;

		// Token: 0x040045C0 RID: 17856
		public TooltipTrigger shipSummaryTip;

		// Token: 0x040045C1 RID: 17857
		public TooltipTrigger rawDamageTip;

		// Token: 0x040045C2 RID: 17858
		public TooltipTrigger absorbedDamageTip;

		// Token: 0x040045C3 RID: 17859
		public TooltipTrigger penetratedDamageTip;

		// Token: 0x040045C4 RID: 17860
		public TooltipTrigger radiationDamageTip;

		// Token: 0x040045C5 RID: 17861
		public TooltipTrigger maneuverTargetTip;

		// Token: 0x040045C6 RID: 17862
		private float _clickCount;

		// Token: 0x040045C7 RID: 17863
		private float _lastClickTime;

		// Token: 0x040045C8 RID: 17864
		private float _doubleClickWindow = 0.5f;

		// Token: 0x040045C9 RID: 17865
		private int _shipTooltipMinWidth = 350;

		// Token: 0x040045CA RID: 17866
		protected TISpaceShipState shipState;

		// Token: 0x040045CB RID: 17867
		protected TIHabModuleState habModuleState;

		// Token: 0x040045CC RID: 17868
		private static WaitForSeconds delay8 = new WaitForSeconds(8f);
	}
}
