using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ModelShark;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PavonisInteractive.TerraInvicta.UI.Canvas_Prefabs.FleetsScreen
{
	// Token: 0x02000928 RID: 2344
	public class ShipModuleListItem : DragItem
	{
		// Token: 0x17000F3B RID: 3899
		// (get) Token: 0x06005980 RID: 22912 RVA: 0x002912F5 File Offset: 0x0028F4F5
		public ShipModuleTable table
		{
			get
			{
				return base.GetComponentInParent<ShipModuleTable>();
			}
		}

		// Token: 0x17000F3C RID: 3900
		// (get) Token: 0x06005981 RID: 22913 RVA: 0x002912FD File Offset: 0x0028F4FD
		public IEnumerable<ShipModuleListItemEntry> entries
		{
			get
			{
				return base.GetComponentsInChildren<ShipModuleListItemEntry>();
			}
		}

		// Token: 0x06005982 RID: 22914 RVA: 0x00291305 File Offset: 0x0028F505
		protected override void Awake()
		{
			base.Awake();
			this.Init();
		}

		// Token: 0x06005983 RID: 22915 RVA: 0x00291313 File Offset: 0x0028F513
		private void Start()
		{
			this.Init();
		}

		// Token: 0x06005984 RID: 22916 RVA: 0x0029131B File Offset: 0x0028F51B
		private void Init()
		{
			if (this.hasInit)
			{
				return;
			}
			this.hasInit = true;
			this.tooltip.enabled = false;
		}

		// Token: 0x06005985 RID: 22917 RVA: 0x00291339 File Offset: 0x0028F539
		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (this.isRow)
			{
				return;
			}
			this.controller.SetSelectedShipPartFromMenu(this.moduleTemplate);
			base.OnBeginDrag(eventData);
		}

		// Token: 0x06005986 RID: 22918 RVA: 0x0029135C File Offset: 0x0028F55C
		public override void OnDrag(PointerEventData eventData)
		{
			if (this.isRow)
			{
				return;
			}
			if (this.dragging)
			{
				base.transform.position = eventData.position + Vector2.left * (this.moduleIcon.rectTransform.sizeDelta.x / 2f) + Vector2.up * (this.moduleIcon.rectTransform.sizeDelta.y / 2f);
				if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
				{
					base.EndDragCleanup();
				}
			}
		}

		// Token: 0x06005987 RID: 22919 RVA: 0x002913FD File Offset: 0x0028F5FD
		public void SetController(FleetsScreenController controller)
		{
			this.controller = controller;
		}

		// Token: 0x06005988 RID: 22920 RVA: 0x00291406 File Offset: 0x0028F606
		public void OnEnable()
		{
			this.tooltip.enabled = true;
		}

		// Token: 0x06005989 RID: 22921 RVA: 0x00291414 File Offset: 0x0028F614
		public void OnDisable()
		{
			this.tooltip.enabled = false;
		}

		// Token: 0x0600598A RID: 22922 RVA: 0x00291424 File Offset: 0x0028F624
		public void SetModuleTemplate(TIShipPartTemplate moduleTemplate)
		{
			this.dragItemType = DragItemType.SHIP;
			this.moduleTemplate = moduleTemplate;
			this.tooltip.SetDelegate("BodyText", () => this.ModuleTTString(moduleTemplate));
			this.obsoleteToggleTooltip.SetText("BodyText", Loc.T("UI.Science.ObsoleteTooltip"));
			this.ModuleSlotType = moduleTemplate.allowedSlots[0];
			this.UpdateItem();
		}

		// Token: 0x0600598B RID: 22923 RVA: 0x002914AB File Offset: 0x0028F6AB
		public string ModuleTTString(TIShipPartTemplate module)
		{
			return new StringBuilder(module.displayName).AppendLine().AppendLine(module.GetFullDescription(null, null, true, ShipModuleSlotType.None, false)).AppendLine(Loc.T("UI.Fleets.RightToPlace"))
				.ToString();
		}

		// Token: 0x0600598C RID: 22924 RVA: 0x002914E1 File Offset: 0x0028F6E1
		public TIShipPartTemplate GetModuleTemplate()
		{
			return this.moduleTemplate;
		}

		// Token: 0x0600598D RID: 22925 RVA: 0x002914EC File Offset: 0x0028F6EC
		private void UpdateItem()
		{
			if (!string.IsNullOrEmpty(this.moduleTemplate.iconResource))
			{
				GameControl.assetLoader.LoadAssetForImageAssignment(this.moduleTemplate.iconResource, this.moduleIcon);
				this.moduleIcon.preserveAspect = true;
			}
			if (this.ModuleSlotType == ShipModuleSlotType.Drive && this.moduleTemplate.ref_drive.thrusters > 1)
			{
				base.gameObject.SetActive(false);
			}
			if (this.isRow)
			{
				this.GenerateEntries();
			}
		}

		// Token: 0x0600598E RID: 22926 RVA: 0x00291568 File Offset: 0x0028F768
		private void GenerateEntries()
		{
			this.AddEntry("", this.moduleTemplate.displayName, null, true);
			switch (this.ModuleSlotType)
			{
			case ShipModuleSlotType.Utility:
			{
				TIShipModuleTemplate tishipModuleTemplate = this.moduleTemplate as TIShipModuleTemplate;
				this.AddEntry("UI.Fleets.ModuleTable.Mass", tishipModuleTemplate.GetLocalizedMass(), tishipModuleTemplate.buildMass_tons(0f, 0f, 0f, 0f, false), true);
				if (tishipModuleTemplate.isHeatSink)
				{
					TIHeatSinkTemplate ref_heatSink = tishipModuleTemplate.ref_heatSink;
					this.AddEntry("UI.Fleets.ModuleTable.HeatSinkCapacity", ref_heatSink.GetLocalizedCapacity(), ref_heatSink.heatCapacity_GJ, true);
				}
				else if (tishipModuleTemplate.isBattery)
				{
					TIBatteryTemplate ref_battery = this.moduleTemplate.ref_battery;
					this.AddEntry("UI.Fleets.ModuleTable.BatteryCapacity", ref_battery.GetLocalizedCapacity(false), ref_battery.GetCapacity(false), true);
				}
				else
				{
					this.AddEntry("UI.Fleets.ModuleTable.HeatSinkCapacity", "", 0f, true);
				}
				this.AddEntry("UI.Fleets.ModuleTable.Materials", tishipModuleTemplate.GetLocalizedCost(), tishipModuleTemplate.buildCost(0f, 0f), true);
				return;
			}
			case ShipModuleSlotType.PowerPlant:
			{
				TIPowerPlantTemplate ref_powerPlant = this.moduleTemplate.ref_powerPlant;
				this.AddEntry("UI.Fleets.ModuleTable.Efficiency", ref_powerPlant.GetLocalizedEfficiency(), ref_powerPlant.efficiency, true);
				this.AddEntry("UI.Fleets.ModuleTable.SpecificPower", ref_powerPlant.GetLocalizedSpecificPower(), ref_powerPlant.specificPower_tGW, true);
				this.AddEntry("UI.Fleets.ModuleTable.MaximumOutput", ref_powerPlant.GetLocalizedMaximumOutput(), ref_powerPlant.maxOutput_GW, true);
				this.AddEntry("UI.Fleets.ModuleTable.MaterialsPerGW", ref_powerPlant.GetLocalizedCostPerGW(), ref_powerPlant.buildCost(1f, 0f), true);
				return;
			}
			case ShipModuleSlotType.Radiator:
			{
				TIRadiatorTemplate ref_radiator = this.moduleTemplate.ref_radiator;
				float num = ref_radiator.buildMass_tons(1f, 0f, 0f, 0f, false);
				this.AddEntry("UI.Fleets.ModuleTable.TonsPerGW", Loc.T("UI.Fleets.Tons", new object[] { num.ToString("N1") }), num, true);
				this.AddEntry("UI.Fleets.ModuleTable.Vulnerability", ref_radiator.GetLocalizedVulnerability(), ref_radiator.vulnerability, true);
				this.AddEntry("UI.Fleets.ModuleTable.MaterialsPerGW", ref_radiator.GetLocalizedCostPerGW(), ref_radiator.buildCost(1f, 0f), true);
				return;
			}
			case ShipModuleSlotType.Drive:
			{
				TIDriveTemplate ref_drive = this.moduleTemplate.ref_drive;
				this.AddEntry("UI.Fleets.ModuleTable.Thrust", ref_drive.GetLocalizedThrust(), ref_drive.thrust_N, true);
				this.AddEntry("UI.Fleets.ModuleTable.ExhaustVelocity", ref_drive.GetLocalizedExhaustVelocity(), ref_drive.EV_kps, true);
				this.AddEntry("UI.Fleets.ModuleTable.Efficiency", ref_drive.GetLocalizedEfficiency(), ref_drive.efficiency, true);
				this.AddEntry("UI.Fleets.ModuleTable.RequiredPower", ref_drive.GetLocalizedRequiredPower(), ref_drive.powerRequirement_GW, true);
				this.AddEntry("UI.Fleets.ModuleTable.Materials", ref_drive.GetLocalizedCost(), ref_drive.buildCost(0f, 0f), true);
				return;
			}
			case ShipModuleSlotType.NoseArmor:
			case ShipModuleSlotType.LateralArmor:
			case ShipModuleSlotType.TailArmor:
			{
				TIShipArmorTemplate ref_armor = this.moduleTemplate.ref_armor;
				this.AddEntry("UI.Fleets.ModuleTable.TonsPerSquareMeterPerPoint", ref_armor.GetLocalizedTonsPerSquareMeterPerPoint(), ref_armor.single_armor_point_mass_tons, true);
				this.AddEntry("UI.Fleets.ModuleTable.Speciality", ref_armor.GetLocalizedSpecialties(), null, false);
				this.AddEntry("UI.Fleets.ModuleTable.MaterialsPerTon", ref_armor.GetLocalizeCostPerTon(), ref_armor.buildCost(1f, 0f), true);
				this.AddEntry("TIShipArmorTemplate.XRayResistance_Header", ref_armor.GetLocalizedXRayValue(), null, true);
				this.AddEntry("TIShipArmorTemplate.BaryonicResistance_Header", ref_armor.GetLocalizedBaryonicValue(), null, true);
				return;
			}
			case ShipModuleSlotType.NoseHardPoint:
			case ShipModuleSlotType.HullHardPoint:
			{
				TIShipWeaponTemplate ref_weapon = this.moduleTemplate.ref_weapon;
				float num2 = 400f;
				this.AddEntry("UI.Fleets.ModuleTable.FireModes", ref_weapon.GetLocalizedFireModes(), null, true);
				this.AddEntry("UI.Fleets.ModuleTable.Cooldown", ref_weapon.GetLocalizedCooldown(), ref_weapon.cooldown_s, true);
				if (ref_weapon.isLaserWeapon || ref_weapon.isParticleWeapon)
				{
					float num3 = ref_weapon.BaseDamageAtRange_points(ref_weapon.ref_beamWeapon.shortRange, false);
					float num4 = ref_weapon.BaseDamageAtRange_points(ref_weapon.ref_beamWeapon.mediumRange, false);
					float num5 = ref_weapon.BaseDamageAtRange_points(ref_weapon.ref_beamWeapon.longRange, false);
					this.AddEntry("UI.Fleets.ModuleTable.ShortRangeDamage", num3.ToString("N1"), num3, true);
					this.AddEntry("UI.Fleets.ModuleTable.MediumRangeDamage", num4.ToString("N1"), num4, true);
					this.AddEntry("UI.Fleets.ModuleTable.LongRangeDamage", num5.ToString("N1"), num5, true);
				}
				else
				{
					float num6 = ref_weapon.BaseDamageAtRange_points(num2, false);
					this.AddEntry("UI.Fleets.ModuleTable.Damage", num6.ToString("N1"), num6, true);
				}
				if (ref_weapon.hasMagazine())
				{
					this.AddEntry("UI.Fleets.ModuleTable.Magazine", ref_weapon.GetLocalizedMagazineMaxAmmoCount(null), null, true);
				}
				if (ref_weapon.magazineRequiresResources())
				{
					this.AddEntry("UI.Fleets.ModuleTable.MagazineMaterials", ref_weapon.GetLocalizedMagazineCost(null), ref_weapon.ref_projectileWeapon.magazineCost(0f), true);
				}
				this.AddEntry("UI.Fleets.ModuleTable.Range", ref_weapon.GetLocalizedTargetingRange(), ref_weapon.targetingRange_km, true);
				this.AddEntry("UI.Fleets.ModuleTable.Mass", ref_weapon.GetLocalizedMass(null), ref_weapon.buildMass_tons(0f, 0f, 0f, 0f, false), true);
				if (ref_weapon.EnergyUsage_GJ(0f) > 0f)
				{
					this.AddEntry("UI.Fleets.ModuleTable.EnergyUsage", ref_weapon.GetLocalizedEnergyUsage(), ref_weapon.EnergyUsage_GJ(0f), true);
				}
				TIBeamWeaponTemplate tibeamWeaponTemplate = ref_weapon as TIBeamWeaponTemplate;
				if (tibeamWeaponTemplate != null)
				{
					this.AddEntry("UI.Fleets.ModuleTable.ShotPower", tibeamWeaponTemplate.GetLocalizedShotPower(), tibeamWeaponTemplate.shotPower_MJ, true);
					if (ref_weapon.isLaserWeapon)
					{
						TILaserWeaponTemplate ref_laserWeapon = ref_weapon.ref_laserWeapon;
						this.AddEntry("UI.Fleets.ModuleTable.ShortRangeArmorEffectiveness", ref_laserWeapon.GetLocalizedArmorEffectivenessAtRange(ref_laserWeapon.shortRange), ref_laserWeapon.ArmorEffectivenessAtRange(ref_laserWeapon.shortRange), true);
						this.AddEntry("UI.Fleets.ModuleTable.MediumRangeArmorEffectiveness", ref_laserWeapon.GetLocalizedArmorEffectivenessAtRange(ref_laserWeapon.mediumRange), ref_laserWeapon.ArmorEffectivenessAtRange(ref_laserWeapon.mediumRange), true);
						this.AddEntry("UI.Fleets.ModuleTable.LongRangeArmorEffectiveness", ref_laserWeapon.GetLocalizedArmorEffectivenessAtRange(ref_laserWeapon.longRange), ref_laserWeapon.ArmorEffectivenessAtRange(ref_laserWeapon.longRange), true);
					}
				}
				else if (ref_weapon.isProjectileWeapon)
				{
					TIProjectileWeaponTemplate ref_projectileWeapon = ref_weapon.ref_projectileWeapon;
					TIPlasmaWeaponTemplate tiplasmaWeaponTemplate = ref_projectileWeapon as TIPlasmaWeaponTemplate;
					if (tiplasmaWeaponTemplate != null)
					{
						this.AddEntry("UI.Fleets.ModuleTable.ChargingEnergy", tiplasmaWeaponTemplate.GetLocalizedChargingEnergy(), tiplasmaWeaponTemplate.chargingEnergy_GJ, true);
					}
					else
					{
						this.AddEntry("UI.Fleets.ModuleTable.Salvo", ref_weapon.GetLocalizedSalvoShotCount(), (ref_projectileWeapon.salvo_shots == 0) ? 1 : ref_projectileWeapon.salvo_shots, true);
						string text = "UI.Fleets.ModuleTable.SalvoCooldown";
						this.AddEntry(text, ref_projectileWeapon.GetLocalizedSalvoCooldown(), (ref_projectileWeapon.intraSalvoCooldown_s == 0f) ? ref_weapon.cooldown_s : ref_projectileWeapon.intraSalvoCooldown_s, true);
					}
					if (ref_weapon.isGunTypeWeapon)
					{
						TIGunTypeWeaponTemplate ref_gunWeapon = ref_weapon.ref_gunWeapon;
						this.AddEntry("UI.Fleets.ModuleTable.MuzzleVelocity", ref_gunWeapon.GetLocalizedMuzzleVelocity(), ref_gunWeapon.muzzleVelocity_kps, true);
					}
					else if (ref_weapon.isMissileWeapon)
					{
						TIMissileTemplate ref_missileWeapon = ref_weapon.ref_missileWeapon;
						this.AddEntry("UI.Fleets.ModuleTable.MissileType", ref_missileWeapon.GetLocalizedWarheadType(), ref_missileWeapon.warheadClass, true);
						this.AddEntry("UI.Fleets.ModuleTable.MissileAcceleration", ref_missileWeapon.GetLocalizedAcceleration(), ref_missileWeapon.acceleration_g, true);
						this.AddEntry("UI.Fleets.ModuleTable.MissileDV", ref_missileWeapon.GetLocalizedDV(), ref_missileWeapon.deltaV_kps, true);
					}
				}
				else if (ref_weapon.isParticleWeapon)
				{
					this.AddEntry("UI.Fleets.ModuleTable.Salvo", ref_weapon.GetLocalizedSalvoShotCount(), (ref_weapon.salvo_shots == 0) ? 1 : ref_weapon.salvo_shots, true);
				}
				this.AddEntry("UI.Fleets.ModuleTable.Materials", ref_weapon.GetLocalizedCost(), ref_weapon.buildCost(0f, 0f), true);
				return;
			}
			default:
				return;
			}
		}

		// Token: 0x0600598F RID: 22927 RVA: 0x00291D5C File Offset: 0x0028FF5C
		private void AddEntry(string labelLocalizationKey, string text = "", object value = null, bool sanitizeText = true)
		{
			if (sanitizeText)
			{
				text = Regex.Replace(text, "\\t|\\n|\\r", "");
				if (!(value is TIResourcesCost))
				{
					text = Regex.Replace(text, "<.*?>", "");
				}
				else
				{
					text = Regex.Replace(text, "</?align.*?>", "");
					text = Regex.Replace(text, "</?line-height.*?>", "");
					text = Regex.Replace(text, "(<sprite.*?>.*?){3}.*?[0-9.]+", (Match match) => match.Value + "\n");
				}
				if (text.Contains(":"))
				{
					text = text.Split(new char[] { ':' }, 2).Last<string>();
				}
				text = Regex.Replace(text, ".*?,.*?, ", (Match match) => match.Value.Trim(new char[] { ' ' }) + "\n");
			}
			if (value == null)
			{
				value = text;
			}
			IComparable comparable = 0;
			if (value is IComparable)
			{
				comparable = value as IComparable;
			}
			else
			{
				TIResourcesCost tiresourcesCost = value as TIResourcesCost;
				if (tiresourcesCost != null)
				{
					comparable = tiresourcesCost.resourceCosts.Sum<ResourceValue>((ResourceValue resourceCost) => resourceCost.value);
				}
			}
			ShipModuleListItemEntry shipModuleListItemEntry = global::UnityEngine.Object.Instantiate<ShipModuleListItemEntry>(this.entryPrefab, base.transform);
			Loc.SwapFonts(shipModuleListItemEntry.gameObject);
			shipModuleListItemEntry.textElement.text = text;
			shipModuleListItemEntry.value = comparable;
			if (labelLocalizationKey != null)
			{
				string text2 = Loc.T(labelLocalizationKey);
				if (this.table.labels.entries.Count<ShipModuleListItemEntry>() < this.entries.Count<ShipModuleListItemEntry>())
				{
					this.table.labels.AddEntry(null, text2, null, false);
				}
			}
		}

		// Token: 0x06005990 RID: 22928 RVA: 0x00291F06 File Offset: 0x00290106
		public void SetTooltipText(string text)
		{
			if (this.tooltip != null)
			{
				this.tooltip.SetText("BodyText", text);
			}
		}

		// Token: 0x06005991 RID: 22929 RVA: 0x00291F27 File Offset: 0x00290127
		public void SetAlpha(bool fullyVisible)
		{
			this.moduleIcon.color = new Color(1f, 1f, 1f, fullyVisible ? 1f : 0.3f);
		}

		// Token: 0x06005992 RID: 22930 RVA: 0x00291F57 File Offset: 0x00290157
		public void OnClickItem()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_ListLineSelect", false, false);
			this.controller.SetSelectedShipPartFromMenu(this.moduleTemplate);
		}

		// Token: 0x06005993 RID: 22931 RVA: 0x00291F78 File Offset: 0x00290178
		public void OnRightClickItem()
		{
			this.controller.SetSelectedShipPartFromMenu(this.moduleTemplate);
			ShipModuleDragDestination bestDropDestinationForModule = this.controller.GetBestDropDestinationForModule(this.moduleTemplate);
			if (bestDropDestinationForModule != null)
			{
				this.controller.SetModuleInSlot(this.moduleTemplate, bestDropDestinationForModule, true);
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_DropModuleInShipDesignSlot", false, false);
				this.controller.SetSelectedDragDestination(bestDropDestinationForModule);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06005994 RID: 22932 RVA: 0x00291FE9 File Offset: 0x002901E9
		public void OnObsoleteToggle()
		{
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_CycleForward", false, false);
			this.UpdateIcon();
			this.controller.OnPartObsoleteToggle(this.moduleTemplate, this.obsoleteToggle.isOn);
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x00292019 File Offset: 0x00290219
		public void UpdateIcon()
		{
			if (this.obsoleteToggle.isOn)
			{
				this.obsoleteIcon.sprite = this.obsolete_on;
				return;
			}
			this.obsoleteIcon.sprite = this.obsolete_off;
		}

		// Token: 0x06005996 RID: 22934 RVA: 0x0029204C File Offset: 0x0029024C
		public void UpdateToggle(TIFactionState faction)
		{
			if (faction.obsoletedShipParts.Contains(this.moduleTemplate.dataName) && !this.obsoleteToggle.isOn)
			{
				this.obsoleteToggle.SetIsOnWithoutNotify(true);
				this.obsoleteIcon.sprite = this.obsolete_on;
			}
		}

		// Token: 0x040040A8 RID: 16552
		public ShipModuleSlotType ModuleSlotType;

		// Token: 0x040040A9 RID: 16553
		public FleetsScreenController controller;

		// Token: 0x040040AA RID: 16554
		public Button addModuleButton;

		// Token: 0x040040AB RID: 16555
		public Image moduleIcon;

		// Token: 0x040040AC RID: 16556
		public Image obsoleteIcon;

		// Token: 0x040040AD RID: 16557
		public Toggle obsoleteToggle;

		// Token: 0x040040AE RID: 16558
		private bool hasInit;

		// Token: 0x040040AF RID: 16559
		private TIShipPartTemplate moduleTemplate;

		// Token: 0x040040B0 RID: 16560
		public bool isRow;

		// Token: 0x040040B1 RID: 16561
		public HorizontalLayoutGroup layout;

		// Token: 0x040040B2 RID: 16562
		[Header("Tooltips")]
		public TooltipTrigger tooltip;

		// Token: 0x040040B3 RID: 16563
		public TooltipTrigger obsoleteToggleTooltip;

		// Token: 0x040040B4 RID: 16564
		[Header("Sprites")]
		public Sprite obsolete_on;

		// Token: 0x040040B5 RID: 16565
		public Sprite obsolete_off;

		// Token: 0x040040B6 RID: 16566
		public ShipModuleListItemEntry entryPrefab;
	}
}
