using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003D5 RID: 981
public class TIShipArmorTemplate : TIShipPartTemplate
{
	// Token: 0x1700025A RID: 602
	// (get) Token: 0x060012E6 RID: 4838 RVA: 0x00059C60 File Offset: 0x00057E60
	public override TIShipArmorTemplate ref_armor
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x060012E7 RID: 4839 RVA: 0x00059C63 File Offset: 0x00057E63
	public override bool isArmor
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x060012E8 RID: 4840 RVA: 0x00059C66 File Offset: 0x00057E66
	public override bool exoFighterPart
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x060012E9 RID: 4841 RVA: 0x00059C69 File Offset: 0x00057E69
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType>
			{
				ShipModuleSlotType.NoseArmor,
				ShipModuleSlotType.LateralArmor,
				ShipModuleSlotType.TailArmor
			};
		}
	}

	// Token: 0x1700025E RID: 606
	// (get) Token: 0x060012EA RID: 4842 RVA: 0x00059C85 File Offset: 0x00057E85
	public float mass_damagePoint_kg
	{
		get
		{
			return 20f / this.heatofVaporization_MJkg;
		}
	}

	// Token: 0x1700025F RID: 607
	// (get) Token: 0x060012EB RID: 4843 RVA: 0x00059C93 File Offset: 0x00057E93
	public float volume_damagePoint_m3
	{
		get
		{
			return this.mass_damagePoint_kg / this.density_kgm3;
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x060012EC RID: 4844 RVA: 0x00059CA2 File Offset: 0x00057EA2
	public float plate_thickness_m
	{
		get
		{
			return this.volume_damagePoint_m3 / 0.005f;
		}
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00059CB0 File Offset: 0x00057EB0
	public float armor_section_thickness_m(float armorPoints)
	{
		return this.plate_thickness_m * armorPoints;
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x060012EE RID: 4846 RVA: 0x00059CBA File Offset: 0x00057EBA
	public float single_armor_point_mass_tons
	{
		get
		{
			return this.armor_section_thickness_m(1f) * this.density_kgm3 / 1000f;
		}
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00059CD4 File Offset: 0x00057ED4
	public float armor_section_volume(float armorPoints, float hullLength_m, float hullWidth_m, float lateralArmorDepth_m, bool lateral)
	{
		float num = 3.1415927f * ((hullWidth_m + lateralArmorDepth_m + lateralArmorDepth_m) / 2f) * ((hullWidth_m + lateralArmorDepth_m + lateralArmorDepth_m) / 2f);
		TIGlobalValuesState globalValues = TIGlobalValuesState.GlobalValues;
		bool? flag;
		if (globalValues == null)
		{
			flag = null;
		}
		else
		{
			ScenarioCustomizations scenarioCustomizations = globalValues.scenarioCustomizations;
			flag = ((scenarioCustomizations != null) ? new bool?(scenarioCustomizations.cinematicCombatRealismScale) : null);
		}
		if (flag ?? StartMenuController.CinematicScalingMode)
		{
			if (lateral)
			{
				float num2 = 3.1415927f * (hullWidth_m / 2f) * (hullWidth_m / 2f) * hullLength_m;
				return (num * hullLength_m - num2) * 0.75f;
			}
			return this.armor_section_thickness_m(armorPoints) * num;
		}
		else
		{
			if (lateral)
			{
				float num3 = 3.1415927f * (hullWidth_m / 2f) * (hullWidth_m / 2f) * hullLength_m;
				return (num * hullLength_m - num3) / 2f;
			}
			return this.armor_section_thickness_m(armorPoints) * num * 3f;
		}
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00059DBC File Offset: 0x00057FBC
	public float GetSpecialtyModifiers(ArmorSpecialty specialty)
	{
		float num = 1f;
		foreach (ArmorSpecialties armorSpecialties in this.specialties)
		{
			if (armorSpecialties.armorSpecialty == specialty)
			{
				num *= armorSpecialties.value;
			}
		}
		return num;
	}

	// Token: 0x060012F1 RID: 4849 RVA: 0x00059E24 File Offset: 0x00058024
	public override float buildMass_tons(float shipLateralArmorDepth_m, float armorPoints, float hullLength_m, float hullWidth_m, bool lateral)
	{
		if (lateral && shipLateralArmorDepth_m == -1f)
		{
			shipLateralArmorDepth_m = this.armor_section_thickness_m(armorPoints);
		}
		return this.armor_section_volume(armorPoints, hullLength_m, hullWidth_m, shipLateralArmorDepth_m, lateral) * this.density_kgm3 / 1000f;
	}

	// Token: 0x060012F2 RID: 4850 RVA: 0x00059E55 File Offset: 0x00058055
	public override TIResourcesCost buildCost(float armor_facing_mass_tons, float value2 = 0f)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(armor_facing_mass_tons * TemplateManager.global.spaceResourceToTons));
		return tiresourcesCost;
	}

	// Token: 0x060012F3 RID: 4851 RVA: 0x00059E7C File Offset: 0x0005807C
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!prospective)
		{
			stringBuilder.AppendLine(this.GetLocalizedMass(shipTemplate, slot));
			string localizedCost = this.GetLocalizedCost(shipTemplate, slot);
			if (localizedCost != string.Empty)
			{
				stringBuilder.AppendLine(Loc.T("UI.Fleets.Cost", new object[] { localizedCost }));
			}
		}
		stringBuilder.Append(this.GetLocalizedSpecialties());
		if (shipTemplate != null)
		{
			stringBuilder.AppendLine(this.GetLocalizedFrontBackTonsPerPoint(shipTemplate));
			stringBuilder.AppendLine(this.GetLocalizedLateralTonsPerPoint(shipTemplate));
		}
		stringBuilder.AppendLine(this.GetLocalizedDepth());
		if (shipTemplate != null)
		{
			if (!prospective)
			{
				if (shipTemplate.noseArmorTemplate == this)
				{
					stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.NoseArmor, false));
				}
				if (shipTemplate.lateralArmorTemplate == this)
				{
					stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.LateralArmor, false));
				}
				if (shipTemplate.tailArmorTemplate == this)
				{
					stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.TailArmor, false));
				}
			}
			else
			{
				stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.NoseArmor, true));
				stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.LateralArmor, true));
				stringBuilder.AppendLine(this.GetLocalizedMaximums(shipTemplate, ShipModuleSlotType.TailArmor, true));
			}
		}
		stringBuilder.AppendLine(this.GetLocalizedXRayValue());
		stringBuilder.AppendLine(this.GetLocalizedBaryonicValue());
		stringBuilder.AppendLine(this.GetLocalizeCostPerTon());
		return stringBuilder.ToString();
	}

	// Token: 0x060012F4 RID: 4852 RVA: 0x00059FC0 File Offset: 0x000581C0
	public override float AIScoringValueForResearch()
	{
		return 1f / this.mass_damagePoint_kg;
	}

	// Token: 0x060012F5 RID: 4853 RVA: 0x00059FD0 File Offset: 0x000581D0
	public string GetLocalizedMaximums(TISpaceShipTemplate ship, ShipModuleSlotType slot, bool prospective = false)
	{
		if (((ship != null) ? ship.hullTemplate : null) != null)
		{
			switch (slot)
			{
			case ShipModuleSlotType.NoseArmor:
			{
				float num;
				return Loc.T("TIShipArmorTemplate.MaxArmorPoints_Nose", new object[] { ship.GetMaxAllowedArmorBySlot(slot, out num, prospective ? this : null) });
			}
			case ShipModuleSlotType.LateralArmor:
			{
				float num;
				return Loc.T("TIShipArmorTemplate.MaxArmorPoints_Lateral", new object[] { ship.GetMaxAllowedArmorBySlot(slot, out num, prospective ? this : null) });
			}
			case ShipModuleSlotType.TailArmor:
			{
				float num;
				return Loc.T("TIShipArmorTemplate.MaxArmorPoints_Tail", new object[] { ship.GetMaxAllowedArmorBySlot(slot, out num, prospective ? this : null) });
			}
			}
		}
		return string.Empty;
	}

	// Token: 0x060012F6 RID: 4854 RVA: 0x0005A084 File Offset: 0x00058284
	public string GetLocalizedMass(TISpaceShipTemplate shipTemplate, ShipModuleSlotType slot)
	{
		string text = "";
		switch (slot)
		{
		case ShipModuleSlotType.NoseArmor:
			text = shipTemplate.noseArmorMass_tons.ToString("N0");
			break;
		case ShipModuleSlotType.LateralArmor:
			text = shipTemplate.lateralArmorMass_tons.ToString("N0");
			break;
		case ShipModuleSlotType.TailArmor:
			text = shipTemplate.tailArmorMass_tons.ToString("N0");
			break;
		}
		return Loc.T("UI.Fleets.Mass", new object[] { text });
	}

	// Token: 0x060012F7 RID: 4855 RVA: 0x0005A104 File Offset: 0x00058304
	public string GetLocalizedDepth()
	{
		return Loc.T("TIShipArmorTemplate.ThicknessPerPoint", new object[] { (this.plate_thickness_m * 100f).ToString("N2") });
	}

	// Token: 0x060012F8 RID: 4856 RVA: 0x0005A140 File Offset: 0x00058340
	public string GetLocalizedCost(TISpaceShipTemplate shipTemplate, ShipModuleSlotType slot)
	{
		switch (slot)
		{
		case ShipModuleSlotType.NoseArmor:
			if (shipTemplate.noseArmorValue != 0)
			{
				return shipTemplate.noseArmorBuildCost.ToString("Relevant", false, false, null, false, FactionResource.None);
			}
			return "";
		case ShipModuleSlotType.LateralArmor:
			if (shipTemplate.lateralArmorValue != 0)
			{
				return shipTemplate.lateralArmorBuildCost.ToString("Relevant", false, false, null, false, FactionResource.None);
			}
			return "";
		case ShipModuleSlotType.TailArmor:
			if (shipTemplate.tailArmorValue != 0)
			{
				return shipTemplate.tailArmorBuildCost.ToString("Relevant", false, false, null, false, FactionResource.None);
			}
			return "";
		default:
			return "";
		}
	}

	// Token: 0x060012F9 RID: 4857 RVA: 0x0005A1D4 File Offset: 0x000583D4
	public string GetLocalizedSpecialties()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ArmorSpecialties armorSpecialties in this.specialties)
		{
			if (armorSpecialties.armorSpecialty != ArmorSpecialty.None && armorSpecialties.value != 1f)
			{
				switch (armorSpecialties.armorSpecialty)
				{
				case ArmorSpecialty.KineticsResistance:
					stringBuilder.AppendLine((armorSpecialties.value <= 1f) ? Loc.T("TIShipArmorTemplate.KineticsResistance", new object[] { (1f - armorSpecialties.value).ToPercent("P0") }) : Loc.T("TIShipArmorTemplate.KineticsVulnerability", new object[] { (armorSpecialties.value - 1f).ToPercent("P0") }));
					break;
				case ArmorSpecialty.LaserResistance:
					stringBuilder.AppendLine((armorSpecialties.value <= 1f) ? Loc.T("TIShipArmorTemplate.LaserResistance", new object[] { (1f - armorSpecialties.value).ToPercent("P0") }) : Loc.T("TIShipArmorTemplate.LaserVulnerability", new object[] { (armorSpecialties.value - 1f).ToPercent("P0") }));
					break;
				case ArmorSpecialty.ChippingResistance:
					stringBuilder.AppendLine((armorSpecialties.value <= 1f) ? Loc.T("TIShipArmorTemplate.ChippingResistance", new object[] { (1f - armorSpecialties.value).ToPercent("P0") }) : Loc.T("TIShipArmorTemplate.ChippingVulnerability", new object[] { (armorSpecialties.value - 1f).ToPercent("P0") }));
					break;
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060012FA RID: 4858 RVA: 0x0005A3B8 File Offset: 0x000585B8
	public string GetLocalizedXRayValue()
	{
		ArmorSpecialties armorSpecialties = this.specialties.FirstOrDefault<ArmorSpecialties>((ArmorSpecialties x) => x.armorSpecialty == ArmorSpecialty.XRayResistance);
		if (armorSpecialties.armorSpecialty == ArmorSpecialty.XRayResistance)
		{
			return Loc.T("TIShipArmorTemplate.XRayResistance", new object[] { armorSpecialties.value.ToString("N2") });
		}
		return string.Empty;
	}

	// Token: 0x060012FB RID: 4859 RVA: 0x0005A424 File Offset: 0x00058624
	public string GetLocalizedBaryonicValue()
	{
		ArmorSpecialties armorSpecialties = this.specialties.FirstOrDefault<ArmorSpecialties>((ArmorSpecialties x) => x.armorSpecialty == ArmorSpecialty.BaryonicResistance);
		if (armorSpecialties.armorSpecialty == ArmorSpecialty.BaryonicResistance)
		{
			return Loc.T("TIShipArmorTemplate.BaryonicResistance", new object[] { armorSpecialties.value.ToString("N2") });
		}
		return string.Empty;
	}

	// Token: 0x060012FC RID: 4860 RVA: 0x0005A490 File Offset: 0x00058690
	public string GetLocalizedFrontBackTonsPerPoint(TISpaceShipTemplate shipTemplate)
	{
		float num = this.buildMass_tons(shipTemplate.lateralArmorThickness_m, 1f, shipTemplate.hullTemplate.length_m, shipTemplate.hullTemplate.width_m, false);
		return Loc.T("TIShipArmorTemplate.NoseTailTonsPerPoint", new object[] { num.ToString(TIUtilities.DecimalPlaces((double)num, 7, 0)) });
	}

	// Token: 0x060012FD RID: 4861 RVA: 0x0005A4EC File Offset: 0x000586EC
	public string GetLocalizedLateralTonsPerPoint(TISpaceShipTemplate shipTemplate)
	{
		float num = this.buildMass_tons(this.armor_section_thickness_m(1f), 1f, shipTemplate.hullTemplate.length_m, shipTemplate.hullTemplate.width_m, true);
		return Loc.T("TIShipArmorTemplate.LateralTonsPerPoint", new object[] { num.ToString(TIUtilities.DecimalPlaces((double)num, 7, 0)) });
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x0005A54C File Offset: 0x0005874C
	public string GetLocalizedTonsPerSquareMeterPerPoint()
	{
		float single_armor_point_mass_tons = this.single_armor_point_mass_tons;
		return Loc.T("UI.Fleets.Tons", new object[] { single_armor_point_mass_tons.ToString(TIUtilities.DecimalPlaces((double)single_armor_point_mass_tons, 7, 0)) });
	}

	// Token: 0x060012FF RID: 4863 RVA: 0x0005A584 File Offset: 0x00058784
	public string GetLocalizeCostPerTon()
	{
		return Loc.T("TIShipArmorTemplate.ResourcesPerTon", new object[] { this.buildCost(1f, 0f).ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x0400110C RID: 4364
	public const float plateArea_m2 = 0.005f;

	// Token: 0x0400110D RID: 4365
	public const float damagePoint_MJ = 20f;

	// Token: 0x0400110E RID: 4366
	public float density_kgm3;

	// Token: 0x0400110F RID: 4367
	public float heatofVaporization_MJkg;

	// Token: 0x04001110 RID: 4368
	public float xRayHalfValue_cm;

	// Token: 0x04001111 RID: 4369
	public float baryonicHalfValue_cm;

	// Token: 0x04001112 RID: 4370
	public List<ArmorSpecialties> specialties = new List<ArmorSpecialties>();

	// Token: 0x04001113 RID: 4371
	public const float CINEMATIC_LATERAL_ARMOR_VOLUME_SCALING = 0.75f;

	// Token: 0x04001114 RID: 4372
	public const float REALISTIC_CAP_ARMOR_ANGLE_MULTIPLIER = 2f;

	// Token: 0x04001115 RID: 4373
	public const float REALISTIC_CAP_ARMOR_ANGLE_VOLUME_MULTIPLIER = 3f;
}
