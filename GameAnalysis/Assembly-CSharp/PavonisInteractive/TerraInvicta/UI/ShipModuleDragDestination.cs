using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.UI
{
	// Token: 0x02000920 RID: 2336
	public class ShipModuleDragDestination : DragDestination
	{
		// Token: 0x17000F31 RID: 3889
		// (get) Token: 0x0600593C RID: 22844 RVA: 0x0028F65A File Offset: 0x0028D85A
		// (set) Token: 0x0600593D RID: 22845 RVA: 0x0028F662 File Offset: 0x0028D862
		[HideInInspector]
		public bool empty { get; private set; }

		// Token: 0x17000F32 RID: 3890
		// (get) Token: 0x0600593E RID: 22846 RVA: 0x0028F66B File Offset: 0x0028D86B
		// (set) Token: 0x0600593F RID: 22847 RVA: 0x0028F673 File Offset: 0x0028D873
		[HideInInspector]
		public bool blocked { get; private set; }

		// Token: 0x17000F33 RID: 3891
		// (get) Token: 0x06005940 RID: 22848 RVA: 0x0028F67C File Offset: 0x0028D87C
		public bool IsArmor
		{
			get
			{
				return this.shipModuleSlotType == ShipModuleSlotType.NoseArmor || this.shipModuleSlotType == ShipModuleSlotType.LateralArmor || this.shipModuleSlotType == ShipModuleSlotType.TailArmor;
			}
		}

		// Token: 0x17000F34 RID: 3892
		// (get) Token: 0x06005941 RID: 22849 RVA: 0x0028F69B File Offset: 0x0028D89B
		public Vector2Int SlotCoordinates
		{
			get
			{
				return this.slotCoordinates;
			}
		}

		// Token: 0x17000F35 RID: 3893
		// (get) Token: 0x06005942 RID: 22850 RVA: 0x0028F6A3 File Offset: 0x0028D8A3
		public bool hasSpinner
		{
			get
			{
				return this.spinnerPanel != null;
			}
		}

		// Token: 0x17000F36 RID: 3894
		// (get) Token: 0x06005943 RID: 22851 RVA: 0x0028F6B1 File Offset: 0x0028D8B1
		public FleetsScreenController FleetsScreenController
		{
			get
			{
				return base.GetComponentInParent<FleetsScreenController>();
			}
		}

		// Token: 0x06005944 RID: 22852 RVA: 0x0028F6BC File Offset: 0x0028D8BC
		private void Awake()
		{
			this.slotImage = base.GetComponent<Image>();
			this.tooltip = base.GetComponent<TooltipTrigger>();
			this.tooltip.enabled = false;
			if (base.transform.childCount > 0)
			{
				this.spinnerPanel = base.transform.GetChild(0).gameObject;
				this.spinnerPanel.SetActive(false);
			}
		}

		// Token: 0x06005945 RID: 22853 RVA: 0x0028F71E File Offset: 0x0028D91E
		public override void SetControllerBase(CanvasControllerBase canvasControllerBase)
		{
			this.fleetsController = (FleetsScreenController)canvasControllerBase;
		}

		// Token: 0x06005946 RID: 22854 RVA: 0x0028F72C File Offset: 0x0028D92C
		public static string EmptySlotIconName(ShipModuleSlotType shipModuleSlotType)
		{
			switch (shipModuleSlotType)
			{
			case ShipModuleSlotType.Utility:
				return "shipbuildericons/empty_Utility";
			case ShipModuleSlotType.PowerPlant:
				return "shipbuildericons/empty_Power_Plant";
			case ShipModuleSlotType.Radiator:
				return "shipbuildericons/empty_Radiators";
			case ShipModuleSlotType.Drive:
				return "shipbuildericons/empty_Drive";
			case ShipModuleSlotType.NoseArmor:
			case ShipModuleSlotType.LateralArmor:
			case ShipModuleSlotType.TailArmor:
				return "shipbuildericons/empty_Armor";
			case ShipModuleSlotType.NoseHardPoint:
				return "shipbuildericons/empty_Nose_Hard_Point";
			case ShipModuleSlotType.HullHardPoint:
				return "shipbuildericons/empty_Hull_Hard_Point";
			default:
				return "";
			}
		}

		// Token: 0x06005947 RID: 22855 RVA: 0x0028F798 File Offset: 0x0028D998
		public static string HighlightSlotIconName(ShipModuleSlotType shipModuleSlotType)
		{
			switch (shipModuleSlotType)
			{
			case ShipModuleSlotType.Utility:
				return "shipbuildericons/select_Utility";
			case ShipModuleSlotType.PowerPlant:
				return "shipbuildericons/select_Power_Plant";
			case ShipModuleSlotType.Radiator:
				return "shipbuildericons/select_Radiators";
			case ShipModuleSlotType.Drive:
				return "shipbuildericons/select_Drive";
			case ShipModuleSlotType.NoseArmor:
			case ShipModuleSlotType.LateralArmor:
			case ShipModuleSlotType.TailArmor:
				return "shipbuildericons/select_Armor";
			case ShipModuleSlotType.NoseHardPoint:
				return "shipbuildericons/select_Nose_Hard_Point";
			case ShipModuleSlotType.HullHardPoint:
				return "shipbuildericons/select_Hull_Hard_Point";
			default:
				return "";
			}
		}

		// Token: 0x06005948 RID: 22856 RVA: 0x0028F804 File Offset: 0x0028DA04
		public void OnIncreasePressed()
		{
			int num = 1;
			if (TIInputManager.IsShiftKeyDown)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					num = 50;
				}
				else
				{
					num = 10;
				}
			}
			else if (TIInputManager.IsControlKeyDown)
			{
				num = 5;
			}
			switch (this.shipModuleSlotType)
			{
			case ShipModuleSlotType.Drive:
				if (this.fleetsController.newShipTemplate.driveTemplate != null && this.fleetsController.newShipTemplate.thrusterCount < 6)
				{
					TIDriveTemplate tidriveTemplate = this.fleetsController.newShipTemplate.driveTemplate.AddThruster(this.fleetsController.newShipTemplate, 1);
					if (tidriveTemplate == null)
					{
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
					}
					else
					{
						this.fleetsController.SetModuleInSlot(tidriveTemplate, this, true);
						AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
					}
				}
				else
				{
					if (this.fleetsController.newShipTemplate.driveTemplate == null)
					{
						this.spinnerValueInput.SetTextWithoutNotify("0");
					}
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.NoseArmor:
				if (this.fleetsController.newShipTemplate.TryAddArmorPoints(this.shipModuleSlotType, num) > 0)
				{
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.noseArmor.armorValue);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.LateralArmor:
				if (this.fleetsController.newShipTemplate.TryAddArmorPoints(this.shipModuleSlotType, num) > 0)
				{
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.lateralArmor.armorValue);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.TailArmor:
				if (this.fleetsController.newShipTemplate.TryAddArmorPoints(this.shipModuleSlotType, num) > 0)
				{
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.tailArmor.armorValue);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.Propellant:
				this.fleetsController.newShipTemplate.propellantTanks += num;
				this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.propellantTanks);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
				break;
			}
			this.fleetsController.changesMadeToExistingClass = true;
			this.fleetsController.designerSaveDesignButton.interactable = this.fleetsController.CanSaveCurrentDesign;
			this.fleetsController.SetSelectedDragDestination(this);
			this.fleetsController.newShipTemplate.CacheTemplateValues(false);
			this.fleetsController.UpdateShipDesignDataPanelAndImage(this.shipModuleSlotType == ShipModuleSlotType.Drive, false, false);
			this.FleetsScreenController.UpdateTransferInfo();
		}

		// Token: 0x06005949 RID: 22857 RVA: 0x0028FAB0 File Offset: 0x0028DCB0
		public void OnDecreasePressed()
		{
			int num = 1;
			if (TIInputManager.IsShiftKeyDown)
			{
				if (TIInputManager.IsControlKeyDown)
				{
					num = 50;
				}
				else
				{
					num = 10;
				}
			}
			else if (TIInputManager.IsControlKeyDown)
			{
				num = 5;
			}
			switch (this.shipModuleSlotType)
			{
			case ShipModuleSlotType.Drive:
				if (this.fleetsController.newShipTemplate.driveTemplate != null && this.fleetsController.newShipTemplate.thrusterCount > 1)
				{
					this.fleetsController.SetModuleInSlot(this.fleetsController.newShipTemplate.driveTemplate.RemoveThruster(this.fleetsController.newShipTemplate, 1), this, true);
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.thrusterCount);
					this.fleetsController.UpdateShipDesignDataPanelAndImage(true, false, false);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
				}
				else
				{
					if (this.fleetsController.newShipTemplate.driveTemplate == null)
					{
						this.spinnerValueInput.SetTextWithoutNotify("0");
					}
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.NoseArmor:
				if (this.fleetsController.newShipTemplate.noseArmor.armorValue > 0)
				{
					TISpaceShipTemplate newShipTemplate = this.fleetsController.newShipTemplate;
					newShipTemplate.noseArmor.armorValue = newShipTemplate.noseArmor.armorValue - Mathf.Min(num, this.fleetsController.newShipTemplate.noseArmor.armorValue);
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.noseArmor.armorValue);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
					FleetsScreenController fleetsScreenController = this.fleetsController;
					TIShipArmorTemplate materialTemplate = this.fleetsController.newShipTemplate.noseArmor.materialTemplate;
					fleetsScreenController.UpdateShipDesignDataPanelAndImage(materialTemplate != null && materialTemplate.hasModel, false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.LateralArmor:
				if (this.fleetsController.newShipTemplate.lateralArmor.armorValue > 0)
				{
					TISpaceShipTemplate newShipTemplate2 = this.fleetsController.newShipTemplate;
					newShipTemplate2.lateralArmor.armorValue = newShipTemplate2.lateralArmor.armorValue - Mathf.Min(num, this.fleetsController.newShipTemplate.lateralArmor.armorValue);
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.lateralArmor.armorValue);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
					FleetsScreenController fleetsScreenController2 = this.fleetsController;
					TIShipArmorTemplate materialTemplate2 = this.fleetsController.newShipTemplate.lateralArmor.materialTemplate;
					fleetsScreenController2.UpdateShipDesignDataPanelAndImage(materialTemplate2 != null && materialTemplate2.hasModel, false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.TailArmor:
				if (this.fleetsController.newShipTemplate.tailArmor.armorValue > 0)
				{
					TISpaceShipTemplate newShipTemplate3 = this.fleetsController.newShipTemplate;
					newShipTemplate3.tailArmor.armorValue = newShipTemplate3.tailArmor.armorValue - Mathf.Min(num, this.fleetsController.newShipTemplate.tailArmor.armorValue);
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.tailArmor.armorValue);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
					FleetsScreenController fleetsScreenController3 = this.fleetsController;
					TIShipArmorTemplate materialTemplate3 = this.fleetsController.newShipTemplate.tailArmor.materialTemplate;
					fleetsScreenController3.UpdateShipDesignDataPanelAndImage(materialTemplate3 != null && materialTemplate3.hasModel, false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			case ShipModuleSlotType.Propellant:
				if (this.fleetsController.newShipTemplate.propellantTanks > 0)
				{
					this.fleetsController.newShipTemplate.propellantTanks -= Mathf.Min(num, this.fleetsController.newShipTemplate.propellantTanks);
					this.UpdateSpinnerValue(this.fleetsController.newShipTemplate.propellantTanks);
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleBack", false, false);
					this.fleetsController.UpdateShipDesignDataPanelAndImage(false, false, false);
				}
				else
				{
					AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
				}
				break;
			}
			this.fleetsController.newShipTemplate.CacheTemplateValues(false);
			this.fleetsController.changesMadeToExistingClass = true;
			this.fleetsController.SetSelectedDragDestination(this);
			this.fleetsController.designerSaveDesignButton.interactable = this.fleetsController.CanSaveCurrentDesign;
			this.FleetsScreenController.UpdateTransferInfo();
		}

		// Token: 0x0600594A RID: 22858 RVA: 0x0028FEC8 File Offset: 0x0028E0C8
		public void OnAmountChanged()
		{
			string text = this.spinnerValueInput.text.Replace("-", "");
			if (text == null || text == string.Empty)
			{
				return;
			}
			int num = int.Parse(text);
			switch (this.shipModuleSlotType)
			{
			case ShipModuleSlotType.Drive:
				if (this.fleetsController.newShipTemplate.driveTemplate != null)
				{
					int num2 = num - this.fleetsController.newShipTemplate.driveTemplate.thrusters;
					TIDriveTemplate tidriveTemplate = null;
					if (num2 > 0)
					{
						tidriveTemplate = this.fleetsController.newShipTemplate.driveTemplate.AddThruster(this.fleetsController.newShipTemplate, num2);
					}
					else if (num2 < 0)
					{
						tidriveTemplate = this.fleetsController.newShipTemplate.driveTemplate.RemoveThruster(this.fleetsController.newShipTemplate, Mathf.Abs(num2));
					}
					if (tidriveTemplate != null)
					{
						this.fleetsController.SetModuleInSlot(tidriveTemplate, this, true);
						this.spinnerValueInput.SetTextWithoutNotify(tidriveTemplate.thrusters.ToString());
					}
					else
					{
						this.spinnerValueInput.SetTextWithoutNotify(this.fleetsController.newShipTemplate.driveTemplate.thrusters.ToString());
					}
				}
				else
				{
					this.spinnerValueInput.SetTextWithoutNotify("0");
				}
				break;
			case ShipModuleSlotType.NoseArmor:
			{
				int num3 = num - this.fleetsController.newShipTemplate.noseArmor.armorValue;
				this.fleetsController.newShipTemplate.TryAddArmorPoints(this.shipModuleSlotType, num3);
				this.spinnerValueInput.SetTextWithoutNotify(this.fleetsController.newShipTemplate.noseArmor.armorValue.ToString());
				break;
			}
			case ShipModuleSlotType.LateralArmor:
			{
				int num4 = num - this.fleetsController.newShipTemplate.lateralArmor.armorValue;
				this.fleetsController.newShipTemplate.TryAddArmorPoints(this.shipModuleSlotType, num4);
				this.spinnerValueInput.SetTextWithoutNotify(this.fleetsController.newShipTemplate.lateralArmor.armorValue.ToString());
				break;
			}
			case ShipModuleSlotType.TailArmor:
			{
				int num5 = num - this.fleetsController.newShipTemplate.tailArmor.armorValue;
				this.fleetsController.newShipTemplate.TryAddArmorPoints(this.shipModuleSlotType, num5);
				this.spinnerValueInput.SetTextWithoutNotify(this.fleetsController.newShipTemplate.tailArmor.armorValue.ToString());
				break;
			}
			case ShipModuleSlotType.Propellant:
				this.fleetsController.newShipTemplate.propellantTanks = num;
				this.spinnerValueInput.SetTextWithoutNotify(this.fleetsController.newShipTemplate.propellantTanks.ToString());
				break;
			}
			this.fleetsController.changesMadeToExistingClass = true;
			this.fleetsController.designerSaveDesignButton.interactable = this.fleetsController.CanSaveCurrentDesign;
			this.fleetsController.SetSelectedDragDestination(this);
			this.fleetsController.newShipTemplate.CacheTemplateValues(false);
			this.fleetsController.UpdateShipDesignDataPanelAndImage(false, false, false);
			this.FleetsScreenController.UpdateTransferInfo();
		}

		// Token: 0x0600594B RID: 22859 RVA: 0x002901CA File Offset: 0x0028E3CA
		public void OnClickDestination()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.fleetsController.SetSelectedDragDestination(this);
		}

		// Token: 0x0600594C RID: 22860 RVA: 0x002901E4 File Offset: 0x0028E3E4
		public void OnRightClickDestination()
		{
			if (!this.empty)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ShipDesignerClearModule", false, false);
				this.fleetsController.ClearSlot(this.SlotCoordinates);
			}
		}

		// Token: 0x0600594D RID: 22861 RVA: 0x0029020B File Offset: 0x0028E40B
		public override void OnPointerEnter(PointerEventData eventData)
		{
			if (DragManager.currentItem != null && DragManager.currentItem as ShipModuleListItem != null && (DragManager.currentItem as ShipModuleListItem).draggable)
			{
				base.OnPointerEnter(eventData);
				this.HighlightDestination();
			}
		}

		// Token: 0x0600594E RID: 22862 RVA: 0x0029024A File Offset: 0x0028E44A
		public override void OnPointerExit(PointerEventData eventData)
		{
			this.DeHiglightDestination();
			base.OnPointerExit(eventData);
		}

		// Token: 0x0600594F RID: 22863 RVA: 0x00290259 File Offset: 0x0028E459
		private void SetEmptyTooltipValue()
		{
			this.tooltip.SetDelegate("BodyText", () => Loc.T(new StringBuilder("UI.Fleets.").Append(this.shipModuleSlotType.ToString()).ToString()));
		}

		// Token: 0x06005950 RID: 22864 RVA: 0x00290278 File Offset: 0x0028E478
		public void EnableDestination(ShipModuleSlotType slotType, Vector2Int slotCoordinates)
		{
			this.shipModuleSlotType = slotType;
			this.slotCoordinates = slotCoordinates;
			this.slotImage.rectTransform.localPosition = this.defaultPosition;
			this.slotImage.enabled = true;
			this.SetImage(ShipModuleDragDestination.EmptySlotIconName(this.shipModuleSlotType), Mount.Standard);
			this.empty = true;
			this.blocked = false;
			this.tooltip.enabled = true;
			this.SetEmptyTooltipValue();
			if (this.IsArmor || slotType == ShipModuleSlotType.Propellant || slotType == ShipModuleSlotType.Drive)
			{
				this.spinnerPanel.SetActive(true);
				this.UpdateSpinnerValue(0);
				if (slotType != ShipModuleSlotType.Drive)
				{
					this.spinnerValueText.raycastTarget = false;
					return;
				}
			}
			else if (this.hasSpinner)
			{
				this.spinnerPanel.SetActive(false);
			}
		}

		// Token: 0x06005951 RID: 22865 RVA: 0x00290330 File Offset: 0x0028E530
		public void UpdateSpinnerValue(int value)
		{
			if (this.shipModuleSlotType == ShipModuleSlotType.Propellant)
			{
				if (value == -1)
				{
					value = this.fleetsController.newShipTemplate.propellantTanks;
				}
				this.spinnerValueInput.text = value.ToString();
				this.spinnerValueText.SetText(Loc.T("UI.Fleets.DesignerPropellantTanks").Split(new char[] { '\n' })[0]);
				return;
			}
			if (this.IsArmor)
			{
				float num;
				int maxAllowedArmorBySlot = this.fleetsController.newShipTemplate.GetMaxAllowedArmorBySlot(this.shipModuleSlotType, out num, null);
				if (this.fleetsController.newShipTemplate.GetArmorFacingTemplateInSlot(this.shipModuleSlotType).materialTemplate != null)
				{
					if (value == -1)
					{
						value = this.fleetsController.newShipTemplate.GetArmorFacingTemplateInSlot(this.shipModuleSlotType).armorValue;
					}
					value = Mathf.Clamp(value, 0, maxAllowedArmorBySlot);
				}
				else
				{
					value = 0;
				}
				this.spinnerValueInput.text = value.ToString();
				this.spinnerValueText.SetText(Loc.T("UI.Fleets.ArmorOnFacing", new object[] { maxAllowedArmorBySlot }));
				return;
			}
			if (this.shipModuleSlotType == ShipModuleSlotType.Drive && value == -1)
			{
				value = this.fleetsController.newShipTemplate.driveTemplate.thrusters;
			}
			this.spinnerValueInput.text = value.ToString();
		}

		// Token: 0x06005952 RID: 22866 RVA: 0x00290479 File Offset: 0x0028E679
		public void HighlightDestination()
		{
			if (this.slotImage.enabled && this.empty)
			{
				this.SetImage(ShipModuleDragDestination.HighlightSlotIconName(this.shipModuleSlotType), Mount.Standard);
			}
		}

		// Token: 0x06005953 RID: 22867 RVA: 0x002904A4 File Offset: 0x0028E6A4
		public void DeHiglightDestination()
		{
			if (this.slotImage.enabled && this.empty && (this.fleetsController.selectedDragDestination != null || this.fleetsController.selectedShipPart == null || !this.fleetsController.selectedShipPart.allowedSlots.Contains(this.shipModuleSlotType)))
			{
				this.SetImage(ShipModuleDragDestination.EmptySlotIconName(this.shipModuleSlotType), Mount.Standard);
			}
		}

		// Token: 0x06005954 RID: 22868 RVA: 0x00290518 File Offset: 0x0028E718
		public void DisableDestination()
		{
			this.slotImage.enabled = false;
			this.shipModuleSlotType = ShipModuleSlotType.None;
			this.cornerIcon.gameObject.SetActive(false);
			this.tooltip.enabled = false;
			if (this.hasSpinner)
			{
				this.spinnerPanel.SetActive(false);
			}
		}

		// Token: 0x06005955 RID: 22869 RVA: 0x00290569 File Offset: 0x0028E769
		public void BlockDestination()
		{
			this.blocked = true;
			this.slotImage.enabled = false;
		}

		// Token: 0x06005956 RID: 22870 RVA: 0x0029057E File Offset: 0x0028E77E
		public void UnBlockDestination()
		{
			this.blocked = false;
			this.slotImage.enabled = true;
		}

		// Token: 0x06005957 RID: 22871 RVA: 0x00290594 File Offset: 0x0028E794
		public void SetEmpty()
		{
			this.empty = true;
			this.currentPart = null;
			this.cornerIcon.gameObject.SetActive(false);
			Mount mount = Mount.Standard;
			if (this.shipModuleSlotType == ShipModuleSlotType.NoseHardPoint)
			{
				mount = Mount.OneNose;
			}
			else if (this.shipModuleSlotType == ShipModuleSlotType.HullHardPoint)
			{
				mount = Mount.OneHull;
			}
			this.SetImage(ShipModuleDragDestination.EmptySlotIconName(this.shipModuleSlotType), mount);
			this.SetEmptyTooltipValue();
			if (this.IsArmor)
			{
				this.UpdateSpinnerValue(0);
			}
			if (this.blocked)
			{
				this.UnBlockDestination();
			}
		}

		// Token: 0x06005958 RID: 22872 RVA: 0x00290610 File Offset: 0x0028E810
		public void SetFilled()
		{
			this.empty = false;
			if (this.shipModuleSlotType == ShipModuleSlotType.NoseHardPoint || this.shipModuleSlotType == ShipModuleSlotType.HullHardPoint)
			{
				this.cornerIcon.gameObject.SetActive(true);
			}
		}

		// Token: 0x06005959 RID: 22873 RVA: 0x00290640 File Offset: 0x0028E840
		public void SetImage(string iconPath, Mount mount = Mount.Standard)
		{
			if (this.blocked || this.shipModuleSlotType == ShipModuleSlotType.Propellant || iconPath == string.Empty)
			{
				this.slotImage.enabled = false;
				return;
			}
			this.slotImage.enabled = true;
			GameControl.assetLoader.LoadAssetForImageAssignment(iconPath, this.slotImage);
			int num = this.iconSize;
			int num2 = this.iconSize;
			switch (mount)
			{
			default:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
				return;
			case Mount.OneHull:
			case Mount.OneNose:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)num, (float)num2);
				this.slotImage.rectTransform.localPosition = this.defaultPosition;
				return;
			case Mount.TwoHullHoriz:
			case Mount.TwoNoseHoriz:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)(num * 2), (float)num2);
				this.slotImage.rectTransform.localPosition = Vector2.zero + Vector2.right * (float)(this.iconSize / 2);
				return;
			case Mount.TwoHullVert:
			case Mount.TwoNoseVert:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)num, (float)(num2 * 2));
				this.slotImage.rectTransform.localPosition = Vector2.zero + Vector2.down * (float)(this.iconSize / 2);
				return;
			case Mount.ThreeHullHoriz:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)(num * 3), (float)num2);
				return;
			case Mount.FourHull:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)(num * 2), (float)(num2 * 2));
				this.slotImage.rectTransform.localPosition = Vector2.zero + Vector2.down * (float)(this.iconSize / 2) + Vector2.right * (float)(this.iconSize / 2);
				return;
			case Mount.ThreeNoseAngle:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)(num * 2), (float)(num2 * 2));
				this.slotImage.rectTransform.localPosition = Vector2.zero + Vector2.down * (float)(this.iconSize / 2) + Vector2.right * ((float)this.iconSize * 0.375f);
				return;
			case Mount.FourNose:
				this.slotImage.rectTransform.sizeDelta = new Vector2((float)(num * 2), (float)(num2 * 4));
				this.slotImage.rectTransform.localPosition = Vector2.zero + Vector2.down * (float)this.iconSize + Vector2.right * (float)(this.iconSize / 2);
				return;
			}
		}

		// Token: 0x0600595A RID: 22874 RVA: 0x00290919 File Offset: 0x0028EB19
		public void SetLayoutOffset(float xOffset, float yOffset)
		{
			this.slotImage.rectTransform.localPosition = Vector2.zero + Vector2.right * xOffset + Vector2.down * yOffset;
		}

		// Token: 0x0600595B RID: 22875 RVA: 0x00290955 File Offset: 0x0028EB55
		public override void OnDrop(PointerEventData eventData)
		{
			if (!base.gameObject.activeInHierarchy || !DragManager.canDropCurrentItem)
			{
				return;
			}
			this.fleetsController.OnDropModuleInSlot(this);
		}

		// Token: 0x0600595C RID: 22876 RVA: 0x00290978 File Offset: 0x0028EB78
		protected override bool CanDropItemHere()
		{
			return base.gameObject.activeSelf && DragManager.currentDragItemType == this.dragItemType && DragManager.currentItem.GetComponent<ShipModuleListItem>().GetModuleTemplate().allowedSlots.Contains(this.shipModuleSlotType) && !this.blocked;
		}

		// Token: 0x0600595D RID: 22877 RVA: 0x002909CC File Offset: 0x0028EBCC
		public bool LegalModuleForSlot(TIShipPartTemplate moduleTemplate, bool allowAlts, out Vector2Int coordinates)
		{
			if (!moduleTemplate.allowedSlots.Contains(this.shipModuleSlotType))
			{
				coordinates = this.slotCoordinates;
				return false;
			}
			if (this.shipModuleSlotType != ShipModuleSlotType.NoseHardPoint && this.shipModuleSlotType != ShipModuleSlotType.HullHardPoint)
			{
				coordinates = this.slotCoordinates;
				return true;
			}
			TIShipWeaponTemplate tishipWeaponTemplate = moduleTemplate as TIShipWeaponTemplate;
			if (tishipWeaponTemplate.mount == Mount.OneHull || tishipWeaponTemplate.mount == Mount.OneNose)
			{
				coordinates = this.slotCoordinates;
				return true;
			}
			List<List<TIShipHullTemplate.ShipModuleSlot>> list = this.fleetsController.newShipTemplate.hullTemplate.ValidBigWeaponSlotSets(tishipWeaponTemplate.mount);
			if (list.Count > 0)
			{
				foreach (List<TIShipHullTemplate.ShipModuleSlot> list2 in list)
				{
					if (list2[0].slotPosition == this.slotCoordinates && list2.All<TIShipHullTemplate.ShipModuleSlot>((TIShipHullTemplate.ShipModuleSlot x) => this.fleetsController.newShipTemplate.GetPartInHullSlot(new Vector2((float)x.x, (float)x.y), true) == null))
					{
						coordinates = this.slotCoordinates;
						return true;
					}
				}
				if (allowAlts)
				{
					foreach (List<TIShipHullTemplate.ShipModuleSlot> list3 in list)
					{
						foreach (TIShipHullTemplate.ShipModuleSlot shipModuleSlot in list3)
						{
							if (shipModuleSlot.slotPosition == this.slotCoordinates && list3.All<TIShipHullTemplate.ShipModuleSlot>((TIShipHullTemplate.ShipModuleSlot x) => this.fleetsController.newShipTemplate.GetPartInHullSlot(new Vector2((float)x.x, (float)x.y), true) == null))
							{
								coordinates = list3[0].slotPosition;
								return true;
							}
						}
					}
				}
			}
			IL_0190:
			coordinates = this.slotCoordinates;
			return false;
		}

		// Token: 0x04004080 RID: 16512
		public ShipModuleSlotType shipModuleSlotType;

		// Token: 0x04004081 RID: 16513
		public TooltipTrigger tooltip;

		// Token: 0x04004082 RID: 16514
		public GameObject spinnerPanel;

		// Token: 0x04004083 RID: 16515
		public TMP_Text spinnerValueText;

		// Token: 0x04004084 RID: 16516
		public TMP_InputField spinnerValueInput;

		// Token: 0x04004085 RID: 16517
		public Image cornerIcon;

		// Token: 0x04004086 RID: 16518
		private Image slotImage;

		// Token: 0x04004087 RID: 16519
		private Vector2Int slotCoordinates;

		// Token: 0x04004088 RID: 16520
		private FleetsScreenController fleetsController;

		// Token: 0x0400408A RID: 16522
		[HideInInspector]
		public Vector3 defaultPosition;

		// Token: 0x0400408B RID: 16523
		[HideInInspector]
		public TIShipPartTemplate currentPart;

		// Token: 0x0400408D RID: 16525
		public int iconSize = 72;
	}
}
