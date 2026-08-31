using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000360 RID: 864
public class TIDriveTemplate : TIShipPartTemplate
{
	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06000F39 RID: 3897 RVA: 0x0004D87F File Offset: 0x0004BA7F
	public override TIDriveTemplate ref_drive
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06000F3A RID: 3898 RVA: 0x0004D882 File Offset: 0x0004BA82
	public override bool isDrive
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700019B RID: 411
	// (get) Token: 0x06000F3B RID: 3899 RVA: 0x0004D885 File Offset: 0x0004BA85
	public override int internalSize
	{
		get
		{
			return this.thrusters;
		}
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x0004D890 File Offset: 0x0004BA90
	public string combatUIPath_OK(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_").Append(this.nozzleStr).Append("x")
			.Append(this.thrusters.ToString())
			.Append("_A")
			.ToString();
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x0004D8E8 File Offset: 0x0004BAE8
	public string combatUIPath_Damaged(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_").Append(this.nozzleStr).Append("x")
			.Append(this.thrusters.ToString())
			.Append("_B")
			.ToString();
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x0004D940 File Offset: 0x0004BB40
	public string combatUIPath_Destroyed(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_").Append(this.nozzleStr).Append("x")
			.Append(this.thrusters.ToString())
			.Append("_C")
			.ToString();
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x0004D998 File Offset: 0x0004BB98
	public string largeCombatUIPath(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_").Append(this.nozzleStr).Append("x")
			.Append(this.thrusters.ToString())
			.ToString();
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x0004D9E8 File Offset: 0x0004BBE8
	public string driveUIResourcePath(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("/drives/").Append(hull.path2[idx]).Append("_mini_")
			.Append(this.nozzleStr)
			.Append("x")
			.Append(this.thrusters.ToString())
			.Append("_A")
			.ToString();
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x0004DA58 File Offset: 0x0004BC58
	public new string modelResource(TIShipHullTemplate hull, int appearanceIndex = 0)
	{
		string text;
		if (hull.alien)
		{
			text = new StringBuilder("ships/Thruster_").Append(hull.dataName).Append("x").Append(this.thrusters.ToString())
				.ToString();
		}
		else if (appearanceIndex == 0)
		{
			text = new StringBuilder("ships/Earth_").Append(hull.dataName).Append("_").Append(this.nozzleStr)
				.Append("x")
				.Append(this.thrusters.ToString())
				.ToString();
		}
		else if (appearanceIndex == 1)
		{
			text = new StringBuilder("ships/Earth_").Append(hull.path1[appearanceIndex].Replace("earth_", "").ToUpper().Replace("/", "")).Append("_").Append(this.nozzleStr)
				.Append("x")
				.Append(this.thrusters.ToString())
				.ToString();
		}
		else
		{
			text = new StringBuilder("ships_prm/Earth_").Append(hull.path1[appearanceIndex].Replace("earth_", "").ToUpper().Replace("/", "")).Append("_").Append(this.nozzleStr)
				.Append("x")
				.Append(this.thrusters.ToString())
				.ToString();
		}
		return text;
	}

	// Token: 0x1700019C RID: 412
	// (get) Token: 0x06000F42 RID: 3906 RVA: 0x0004DBDE File Offset: 0x0004BDDE
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType> { ShipModuleSlotType.Drive };
		}
	}

	// Token: 0x1700019D RID: 413
	// (get) Token: 0x06000F43 RID: 3907 RVA: 0x0004DBEC File Offset: 0x0004BDEC
	public float thrustPower_GW
	{
		get
		{
			return this.thrust_N * this.EV_kps * 0.5f / 1000000f;
		}
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x0004DC07 File Offset: 0x0004BE07
	public static string propellantStr(Propellant propellant)
	{
		return Loc.T(new StringBuilder("TIDriveTemplate.").Append(propellant.ToString()).ToString());
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x0004DC30 File Offset: 0x0004BE30
	public string MainThrusterFXResource(bool alien)
	{
		if (alien)
		{
			return "ships/HumanThrusterBasic";
		}
		Nozzle nozzle = this.nozzle;
		if (nozzle > Nozzle.Magnetic && nozzle == Nozzle.Pulsed)
		{
			return "ships/NuclearThruster";
		}
		switch (this.driveClassification)
		{
		case DriveClassification.Chemical:
			return "ships/HumanThruster_Chemical";
		case DriveClassification.Electromagnetic:
			if (this.specificPower_kgMW > 0f)
			{
				return "ships/HumanThruster_MassDriver";
			}
			break;
		case DriveClassification.Fission_Thermal:
			if (this.perTankPropellantMaterials.metals > 0f || this.perTankPropellantMaterials.nobleMetals > 0f)
			{
				return "ships/HumanThruster_Fission";
			}
			if (this.propellant == Propellant.ReactionProducts)
			{
				return "ships/HumanThruster_FissionFrag";
			}
			break;
		case DriveClassification.NuclearSaltWater:
			return "ships/HumanThruster_NuclearSalt";
		case DriveClassification.Fusion_Thermal:
			if (this.propellant == Propellant.ReactionProducts)
			{
				if (this.requiredPowerPlant == PowerPlantRequirement.Inertial_Confinement_Fusion)
				{
					return "ships/HumanThruster_PCT";
				}
				return "ships/HumanThruster_ReactionProducts";
			}
			break;
		case DriveClassification.Antimatter:
			if (this.propellant == Propellant.ReactionProducts)
			{
				return "ships/HumanThruster_FissionFrag";
			}
			break;
		}
		Propellant propellant = this.propellant;
		if (propellant == Propellant.Hydrogen)
		{
			return "ships/HumanThruster_Hydrogen";
		}
		if (propellant == Propellant.NobleGases)
		{
			return "ships/HumanThruster_Noble";
		}
		Log.Info("Default thruster hit: " + this.displayName, Array.Empty<object>());
		return "ships/AlienThruster";
	}

	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06000F46 RID: 3910 RVA: 0x0004DD54 File Offset: 0x0004BF54
	public string nozzleStr
	{
		get
		{
			switch (this.nozzle)
			{
			default:
				return "DeLaval";
			case Nozzle.Magnetic:
				return "Magnetic";
			case Nozzle.Pulsed:
				return "Pulse";
			}
		}
	}

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06000F47 RID: 3911 RVA: 0x0004DD8C File Offset: 0x0004BF8C
	public bool selfPowered
	{
		get
		{
			DriveClassification driveClassification = this.driveClassification;
			return driveClassification == DriveClassification.Chemical || driveClassification - DriveClassification.Fission_Pulse <= 1 || driveClassification == DriveClassification.Fusion_Pulse;
		}
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0004DDB0 File Offset: 0x0004BFB0
	public string VectorThrusterFXResource(bool alien)
	{
		if (alien)
		{
			return TemplateManager.global.pathAlienThrusterVFX;
		}
		if (this.nozzle != Nozzle.Magnetic)
		{
			return TemplateManager.global.pathHumanThrusterBasicVFX;
		}
		return TemplateManager.global.pathHumanThrusterAdvancedVFX;
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x0004DDDE File Offset: 0x0004BFDE
	public override float buildMass_tons(float value1 = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.flatMass_tons + this.thrustPower_GW * this.specificPower_kgMW;
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x0004DDF4 File Offset: 0x0004BFF4
	public override TIResourcesCost buildCost(float value = 0f, float value2 = 0f)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(this.buildMass_tons(value, value2, 0f, 0f, false) * TemplateManager.global.spaceResourceToTons));
		return tiresourcesCost;
	}

	// Token: 0x170001A0 RID: 416
	// (get) Token: 0x06000F4B RID: 3915 RVA: 0x0004DE35 File Offset: 0x0004C035
	public float powerRequirement_GW
	{
		get
		{
			if (!this.selfPowered)
			{
				return this.thrustPower_GW / this.efficiency;
			}
			return 0f;
		}
	}

	// Token: 0x170001A1 RID: 417
	// (get) Token: 0x06000F4C RID: 3916 RVA: 0x0004DE52 File Offset: 0x0004C052
	public Nozzle nozzle
	{
		get
		{
			if (this.pulsedDrive)
			{
				return Nozzle.Pulsed;
			}
			if (this.singleThrusterTemplate.thrustPower_GW < 1f || (double)this.EV_kps < 87.5 || !this.singleThrusterTemplate.antimatterOrNuclearDrive)
			{
				return Nozzle.DeLaval;
			}
			return Nozzle.Magnetic;
		}
	}

	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06000F4D RID: 3917 RVA: 0x0004DE92 File Offset: 0x0004C092
	public float massFlow_kgs
	{
		get
		{
			return this.thrust_N / (this.EV_kps * 1000f);
		}
	}

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06000F4E RID: 3918 RVA: 0x0004DEA7 File Offset: 0x0004C0A7
	public bool openCycleCooling
	{
		get
		{
			return this.cooling == CoolingCycle.Open || (this.cooling == CoolingCycle.Calc && (this.pulsedDrive || this.singleThrusterTemplate.massFlow_kgs >= 3f));
		}
	}

	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06000F4F RID: 3919 RVA: 0x0004DEDD File Offset: 0x0004C0DD
	public TIDriveTemplate singleThrusterTemplate
	{
		get
		{
			if (this.thrusters <= 1)
			{
				return this;
			}
			return TemplateManager.Find<TIDriveTemplate>(this.singleThrusterTemplateName, false);
		}
	}

	// Token: 0x170001A5 RID: 421
	// (get) Token: 0x06000F50 RID: 3920 RVA: 0x0004DEF6 File Offset: 0x0004C0F6
	public string singleThrusterTemplateName
	{
		get
		{
			if (this.thrusters != 1)
			{
				StringBuilder stringBuilder = new StringBuilder(base.dataName);
				stringBuilder[base.dataName.Length - 1] = '1';
				return stringBuilder.ToString();
			}
			return base.dataName;
		}
	}

	// Token: 0x170001A6 RID: 422
	// (get) Token: 0x06000F51 RID: 3921 RVA: 0x0004DF30 File Offset: 0x0004C130
	public string maxThrustersTemplateName
	{
		get
		{
			string text = this.singleThrusterTemplateName;
			for (int i = 2; i <= 6; i++)
			{
				StringBuilder stringBuilder = new StringBuilder(base.dataName);
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder.Append(i.ToString());
				if (TemplateManager.Find<TIDriveTemplate>(stringBuilder.ToString(), false) == null)
				{
					break;
				}
				text = stringBuilder.ToString();
			}
			return text;
		}
	}

	// Token: 0x170001A7 RID: 423
	// (get) Token: 0x06000F52 RID: 3922 RVA: 0x0004DF90 File Offset: 0x0004C190
	public override bool exoFighterPart
	{
		get
		{
			return (this.driveClassification == DriveClassification.Chemical || this.requiredPowerPlant == PowerPlantRequirement.Solid_Core_Fission) && this.thrusters == 1;
		}
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x0004DFB0 File Offset: 0x0004C1B0
	public string GetMaterialPath(TIFactionState faction, int hullAppearanceIndex)
	{
		StringBuilder stringBuilder = new StringBuilder(faction.template.GetShipMaterialBundlePath(hullAppearanceIndex)).Append("/MAT_");
		switch (this.nozzle)
		{
		case Nozzle.DeLaval:
			stringBuilder.Append("EngineDeLavalx");
			break;
		case Nozzle.Magnetic:
			stringBuilder.Append("Magneticx");
			break;
		case Nozzle.Pulsed:
			stringBuilder.Append("Pulsex");
			break;
		}
		string text = "";
		if (hullAppearanceIndex == 1)
		{
			text = "_ALT1";
		}
		else if (hullAppearanceIndex == 2 || hullAppearanceIndex == 3)
		{
			text = TIUtilities.ContentBundleShipAbbreviation(hullAppearanceIndex).ToUpperInvariant();
		}
		stringBuilder.Append(this.thrusters.ToString()).Append(text).Append(TIFactionTemplate.GetShipMaterialSuffix(faction));
		return stringBuilder.ToString();
	}

	// Token: 0x170001A8 RID: 424
	// (get) Token: 0x06000F54 RID: 3924 RVA: 0x0004E06B File Offset: 0x0004C26B
	public string driveTypeName
	{
		get
		{
			return base.dataName.Substring(0, base.dataName.Length - 2);
		}
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x0004E088 File Offset: 0x0004C288
	public TIDriveTemplate GetVariation(int thrusterCount)
	{
		if (thrusterCount < 1 || thrusterCount > 6)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(base.dataName);
		stringBuilder[base.dataName.Length - 1] = thrusterCount.ToString()[0];
		return TemplateManager.Find<TIDriveTemplate>(stringBuilder.ToString(), false);
	}

	// Token: 0x170001A9 RID: 425
	// (get) Token: 0x06000F56 RID: 3926 RVA: 0x0004E0D5 File Offset: 0x0004C2D5
	public IEnumerable<TIDriveTemplate> Variations
	{
		get
		{
			return from x in 6.Range()
				select this.GetVariation(x + 1) into x
				where x != null
				select x;
		}
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x0004E114 File Offset: 0x0004C314
	public TIDriveTemplate AddThruster(TISpaceShipTemplate ship, int amount = 1)
	{
		TIDriveTemplate variation = this.GetVariation(this.thrusters + amount);
		if (variation == null || !ship.validDriveForShipsPowerPlant(variation))
		{
			return null;
		}
		return variation;
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x0004E140 File Offset: 0x0004C340
	public TIDriveTemplate RemoveThruster(TISpaceShipTemplate ship, int amount = 1)
	{
		if (this.thrusters == 1)
		{
			return this;
		}
		TIDriveTemplate variation = this.GetVariation(this.thrusters - amount);
		if (variation == null || !ship.validDriveForShipsPowerPlant(variation))
		{
			return null;
		}
		return variation;
	}

	// Token: 0x170001AA RID: 426
	// (get) Token: 0x06000F59 RID: 3929 RVA: 0x0004E176 File Offset: 0x0004C376
	public float thrustRating
	{
		get
		{
			if (this._thrustRating == -1f)
			{
				this._thrustRating = 1f + Mathf.Log(this.thrust_N) / 0.6931472f - this.log1000 / 0.6931472f;
			}
			return this._thrustRating;
		}
	}

	// Token: 0x170001AB RID: 427
	// (get) Token: 0x06000F5A RID: 3930 RVA: 0x0004E1B5 File Offset: 0x0004C3B5
	public float EVRating
	{
		get
		{
			if (this._EVRating == -1f)
			{
				this._EVRating = Mathf.Log(this.EV_kps) / 0.6931472f;
			}
			return this._EVRating;
		}
	}

	// Token: 0x170001AC RID: 428
	// (get) Token: 0x06000F5B RID: 3931 RVA: 0x0004E1E4 File Offset: 0x0004C3E4
	public bool nuclearThermalDrive
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.requiredPowerPlant;
			return powerPlantRequirement - PowerPlantRequirement.Solid_Core_Fission <= 10;
		}
	}

	// Token: 0x170001AD RID: 429
	// (get) Token: 0x06000F5C RID: 3932 RVA: 0x0004E204 File Offset: 0x0004C404
	public bool antimatterOrNuclearDrive
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.requiredPowerPlant;
			return powerPlantRequirement - PowerPlantRequirement.Solid_Core_Fission <= 14 || this.driveClassification == DriveClassification.NuclearSaltWater;
		}
	}

	// Token: 0x170001AE RID: 430
	// (get) Token: 0x06000F5D RID: 3933 RVA: 0x0004E22C File Offset: 0x0004C42C
	public bool fissionDrive
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.requiredPowerPlant;
			return powerPlantRequirement - PowerPlantRequirement.Solid_Core_Fission <= 3;
		}
	}

	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06000F5E RID: 3934 RVA: 0x0004E24C File Offset: 0x0004C44C
	public bool fusionDrive
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.requiredPowerPlant;
			return powerPlantRequirement - PowerPlantRequirement.Z_Pinch_Fusion <= 6;
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06000F5F RID: 3935 RVA: 0x0004E26C File Offset: 0x0004C46C
	public bool magneticFusionDrive
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.requiredPowerPlant;
			return powerPlantRequirement == PowerPlantRequirement.Any_Magnetic_Confinement_Fusion || powerPlantRequirement - PowerPlantRequirement.Hybrid_Confinement_Fusion <= 2;
		}
	}

	// Token: 0x170001B1 RID: 433
	// (get) Token: 0x06000F60 RID: 3936 RVA: 0x0004E290 File Offset: 0x0004C490
	public bool pulsedDrive
	{
		get
		{
			DriveClassification driveClassification = this.driveClassification;
			return driveClassification == DriveClassification.Fission_Pulse || driveClassification == DriveClassification.Fusion_Pulse;
		}
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x0004E2B0 File Offset: 0x0004C4B0
	public bool IsCompatible(TIPowerPlantTemplate powerPlant)
	{
		bool flag = this.requiredPowerPlant == PowerPlantRequirement.Any_General || this.requiredPowerPlant == powerPlant.powerPlantClass || (this.requiredPowerPlant == PowerPlantRequirement.Any_Magnetic_Confinement_Fusion && powerPlant.magneticFusionPlant) || (powerPlant.powerPlantClass == PowerPlantRequirement.Molten_Salt_Core_Fission && (this.requiredPowerPlant == PowerPlantRequirement.Solid_Core_Fission || this.requiredPowerPlant == PowerPlantRequirement.Liquid_Core_Fission));
		bool flag2 = powerPlant.maxOutput_GW >= this.powerRequirement_GW;
		return flag && flag2;
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x0004E31C File Offset: 0x0004C51C
	public bool IsValidRefitPart(TISpaceShipTemplate oldShipTemplate)
	{
		return this.driveClassification == oldShipTemplate.driveTemplate.driveClassification && this.requiredPowerPlant == oldShipTemplate.driveTemplate.requiredPowerPlant && this.propellant == oldShipTemplate.driveTemplate.propellant;
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x0004E359 File Offset: 0x0004C559
	public bool IsSameDriveWithDifferentThrusterCount(TIDriveTemplate other)
	{
		return base.dataName.Substring(0, base.dataName.Length - 1) == other.dataName.Substring(0, other.dataName.Length - 1);
	}

	// Token: 0x170001B2 RID: 434
	// (get) Token: 0x06000F64 RID: 3940 RVA: 0x0004E392 File Offset: 0x0004C592
	public override string description
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(this.singleThrusterTemplateName).ToString());
		}
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x0004E3C4 File Offset: 0x0004C5C4
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedClassification());
		if (this.requiredPowerPlant != PowerPlantRequirement.Any_General)
		{
			stringBuilder.AppendLine(this.GetLocalizedRequiredPowerPlant());
		}
		stringBuilder.AppendLine(this.GetLocalizedRequiredPower());
		if (this.buildMass_tons(0f, 0f, 0f, 0f, false) > 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedMass());
		}
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedThrust());
		stringBuilder.AppendLine(this.GetLocalizedCombatThrust(ship));
		stringBuilder.AppendLine(this.GetLocalizedExhaustVelocity());
		stringBuilder.AppendLine(this.GetLocalizedEfficiency());
		stringBuilder.AppendLine(this.GetLocalizedShipPowerRule());
		stringBuilder.AppendLine(this.GetLocalizedPropellantType());
		stringBuilder.AppendLine(this.GetLocalizedPropellantMaterials((shipTemplate != null) ? shipTemplate.designingFaction : null));
		stringBuilder.AppendLine(Loc.T(this.openCycleCooling ? "TIDriveTemplate.OpenCycle" : "TIDriveTemplate.ClosedCycle"));
		if (this.freeISRU)
		{
			stringBuilder.AppendLine(Loc.T("TIDriveTemplate.ISRU"));
		}
		if (this.helium3Fuel)
		{
			stringBuilder.AppendLine(Loc.T("TIDriveTemplate.UsesHelium3"));
		}
		if (!GameStateManager.GlobalValues().scenarioCustomizations.cinematicCombatRealismDV && (this.driveClassification == DriveClassification.Fission_Pulse || this.driveClassification == DriveClassification.Fusion_Pulse))
		{
			stringBuilder.AppendLine(Loc.T("TIDriveTemplate.NoEVPenalty"));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x0004E540 File Offset: 0x0004C740
	public string GetLocalizedRequiredPowerPlant()
	{
		string text = Loc.T("TIPowerPlantTemplate.PowerPlantRequirement." + this.requiredPowerPlant.ToString());
		return Loc.T("TIDriveTemplate.RequiredPowerPlant", new object[] { text });
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x0004E582 File Offset: 0x0004C782
	public string GetLocalizedRequiredPower()
	{
		return TIUtilities.LocalizeGW("UI.Fleets.RequiredPowerGW", this.powerRequirement_GW);
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x0004E594 File Offset: 0x0004C794
	public string GetLocalizedThrust()
	{
		return Loc.T("TIDriveTemplate.Thrust", new object[]
		{
			this.thrust_N.ToString("N0"),
			this.thrustRating.ToString("N1")
		});
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x0004E5DC File Offset: 0x0004C7DC
	public string GetLocalizedCombatThrust(TISpaceShipState ship)
	{
		return Loc.T("TIDriveTemplate.CombatThrust", new object[]
		{
			TIUtilities.FormatBigOrSmallNumber((ship == null) ? this.thrustCap : ship.modifiedThrustCap, 1, 7, 0, false, false),
			TIUtilities.FormatBigOrSmallNumber(this.thrust_N * ((ship == null) ? this.thrustCap : ship.modifiedThrustCap), 1, 7, 0, false, false).ToString()
		});
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x0004E650 File Offset: 0x0004C850
	public string GetLocalizedExhaustVelocity()
	{
		return Loc.T("TIDriveTemplate.EV", new object[]
		{
			TIUtilities.FormatBigOrSmallNumber(this.EV_kps, 1, 7, 1, false, false),
			this.EVRating.ToString("N1")
		});
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x0004E696 File Offset: 0x0004C896
	public string GetLocalizedEfficiency()
	{
		return Loc.T("TIDriveTemplate.Efficiency", new object[] { this.efficiency.ToPercent("P1") });
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0004E6BB File Offset: 0x0004C8BB
	public string GetLocalizedShipPowerRule()
	{
		return Loc.T("TIDriveTemplate.PowerGeneration", new object[] { Loc.T(new StringBuilder("TIDriveTemplate.PowerGenerationType.").Append(this.powerGen.ToString()).ToString()) });
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0004E6FA File Offset: 0x0004C8FA
	public string GetLocalizedPropellantType()
	{
		return Loc.T("UI.Fleets.PropellantType", new object[] { TIDriveTemplate.propellantStr(this.propellant) });
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x0004E71A File Offset: 0x0004C91A
	public string GetLocalizedPropellantMaterials(TIFactionState faction)
	{
		return Loc.T("UI.Fleets.PropellantMaterials", new object[] { this.PropellantIcons(false, faction) });
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x0004E737 File Offset: 0x0004C937
	public string GetLocalizedClassification()
	{
		return Loc.T("UI.Fleets.Drive.Classification", new object[] { Loc.T(new StringBuilder("TIDriveTemplate.Class.").Append(this.driveClassification).ToString()) });
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x0004E770 File Offset: 0x0004C970
	public string PropellantIcons(bool iconsOnly, TIFactionState faction)
	{
		return this.GetPerTankPropellantMaterials(faction).ToResourcesCost(100f * TemplateManager.global.spaceResourceToTons).ToString("Relevant", false, false, null, iconsOnly, FactionResource.None);
	}

	// Token: 0x06000F71 RID: 3953 RVA: 0x0004E7AC File Offset: 0x0004C9AC
	public ResourceCostBuilder GetPerTankPropellantMaterials(TIFactionState faction)
	{
		if (this.helium3Fuel && faction != null && faction.He3Access)
		{
			ResourceCostBuilder resourceCostBuilder = this.perTankPropellantMaterials;
			resourceCostBuilder.water += resourceCostBuilder.fissiles;
			resourceCostBuilder.fissiles = 0f;
			return resourceCostBuilder;
		}
		return this.perTankPropellantMaterials;
	}

	// Token: 0x04000F48 RID: 3912
	public int thrusters;

	// Token: 0x04000F49 RID: 3913
	public DriveClassification driveClassification;

	// Token: 0x04000F4A RID: 3914
	public float thrust_N;

	// Token: 0x04000F4B RID: 3915
	public float EV_kps;

	// Token: 0x04000F4C RID: 3916
	public float specificPower_kgMW;

	// Token: 0x04000F4D RID: 3917
	public float flatMass_tons;

	// Token: 0x04000F4E RID: 3918
	public float thrustCap;

	// Token: 0x04000F4F RID: 3919
	public float efficiency;

	// Token: 0x04000F50 RID: 3920
	public PowerPlantRequirement requiredPowerPlant;

	// Token: 0x04000F51 RID: 3921
	public Propellant propellant;

	// Token: 0x04000F52 RID: 3922
	public bool freeISRU;

	// Token: 0x04000F53 RID: 3923
	public bool helium3Fuel;

	// Token: 0x04000F54 RID: 3924
	public string notes;

	// Token: 0x04000F55 RID: 3925
	public ResourceCostBuilder perTankPropellantMaterials;

	// Token: 0x04000F56 RID: 3926
	public CoolingCycle cooling;

	// Token: 0x04000F57 RID: 3927
	public PowerGenerationType powerGen;

	// Token: 0x04000F58 RID: 3928
	private float _thrustRating = -1f;

	// Token: 0x04000F59 RID: 3929
	private float _EVRating = -1f;

	// Token: 0x04000F5A RID: 3930
	private float log1000 = 3f;
}
