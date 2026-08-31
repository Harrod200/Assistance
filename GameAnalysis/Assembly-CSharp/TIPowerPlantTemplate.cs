using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020003D0 RID: 976
public class TIPowerPlantTemplate : TIShipPartTemplate
{
	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06001283 RID: 4739 RVA: 0x00058CD5 File Offset: 0x00056ED5
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType> { ShipModuleSlotType.PowerPlant };
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06001284 RID: 4740 RVA: 0x00058CE3 File Offset: 0x00056EE3
	public override TIPowerPlantTemplate ref_powerPlant
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06001285 RID: 4741 RVA: 0x00058CE6 File Offset: 0x00056EE6
	public override bool isPowerPlant
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06001286 RID: 4742 RVA: 0x00058CE9 File Offset: 0x00056EE9
	public string PowerPlantClassStr
	{
		get
		{
			return Loc.T(new StringBuilder("TIPowerPlantTemplate.PowerPlantRequirement.").Append(this.powerPlantClass.ToString()).ToString());
		}
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x00058D15 File Offset: 0x00056F15
	public override float buildMass_tons(float power_GW, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		if (!bValue)
		{
			return Mathf.Max(1f, this.specificPower_tGW * power_GW);
		}
		return this.specificPower_tGW * power_GW;
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x00058D38 File Offset: 0x00056F38
	public override TIResourcesCost buildCost(float power_GW, float value2 = 0f)
	{
		float num = this.buildMass_tons(power_GW, value2, 0f, 0f, false);
		return this.weightedBuildMaterials.ToResourcesCost(num * TemplateManager.global.spaceResourceToTons);
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x00058D70 File Offset: 0x00056F70
	public float WasteHeat_GW(bool openCycleDriveCooling, float drivePowerRequirement_GW, float systemsAndWeaponsRequirement_GW)
	{
		float num;
		if (openCycleDriveCooling)
		{
			num = systemsAndWeaponsRequirement_GW;
		}
		else
		{
			num = drivePowerRequirement_GW + systemsAndWeaponsRequirement_GW;
		}
		return num * (1f - this.efficiency);
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x0600128A RID: 4746 RVA: 0x00058D98 File Offset: 0x00056F98
	public bool fissionPlant
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.powerPlantClass;
			return powerPlantRequirement - PowerPlantRequirement.Solid_Core_Fission <= 3;
		}
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x0600128B RID: 4747 RVA: 0x00058DB8 File Offset: 0x00056FB8
	public bool magneticFusionPlant
	{
		get
		{
			PowerPlantRequirement powerPlantRequirement = this.powerPlantClass;
			return powerPlantRequirement == PowerPlantRequirement.Any_Magnetic_Confinement_Fusion || powerPlantRequirement - PowerPlantRequirement.Hybrid_Confinement_Fusion <= 2;
		}
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00058DDA File Offset: 0x00056FDA
	public bool IsCompatible(TIDriveTemplate drive)
	{
		return drive.IsCompatible(this);
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x00058DE3 File Offset: 0x00056FE3
	public bool IsValidRefitPart(TISpaceShipTemplate originalShipTemplate)
	{
		return this.powerPlantClass == originalShipTemplate.powerPlantTemplate.powerPlantClass;
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00058DF8 File Offset: 0x00056FF8
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedPowerPlantType());
		if (!prospective && shipTemplate != null && shipTemplate.powerPlantMass_tons != 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedOutput(shipTemplate));
			stringBuilder.AppendLine(this.GetLocalizedMass(shipTemplate));
			if (shipTemplate.driveTemplate != null)
			{
				stringBuilder.AppendLine(this.GetLocalizedWasteHeat(shipTemplate));
			}
		}
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedEfficiency());
		stringBuilder.AppendLine(this.GetLocalizedSpecificPower());
		stringBuilder.AppendLine(this.GetLocalizedMaximumOutput());
		if (!prospective && shipTemplate != null && shipTemplate.powerPlantMass_tons != 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedCost(shipTemplate));
		}
		else
		{
			stringBuilder.AppendLine(this.GetLocalizedCostPerGW());
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00058ED0 File Offset: 0x000570D0
	public override float AIScoringValueForResearch()
	{
		return 1f / this.specificPower_tGW;
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06001290 RID: 4752 RVA: 0x00058EDE File Offset: 0x000570DE
	public override bool exoFighterPart
	{
		get
		{
			return this.powerPlantClass == PowerPlantRequirement.Fuel_Cell || (this.powerPlantClass == PowerPlantRequirement.Solid_Core_Fission && this.specificPower_tGW <= 6f);
		}
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x00058F06 File Offset: 0x00057106
	public string GetLocalizedPowerPlantType()
	{
		return Loc.T("TIPowerPlantTemplate.PowerPlantType", new object[] { this.PowerPlantClassStr });
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x00058F21 File Offset: 0x00057121
	public string GetLocalizedOutput(TISpaceShipTemplate shipTemplate)
	{
		return TIUtilities.LocalizeGW("TIPowerPlantTemplate.OutputGW", shipTemplate.shipPowerProductionRequirement_GW);
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x00058F34 File Offset: 0x00057134
	public string GetLocalizedMass(TISpaceShipTemplate shipTemplate)
	{
		return Loc.T("UI.Fleets.Mass", new object[] { shipTemplate.powerPlantMass_tons.ToString("N0") });
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x00058F67 File Offset: 0x00057167
	public string GetLocalizedWasteHeat(TISpaceShipTemplate shipTemplate)
	{
		return TIUtilities.LocalizeGW("TIPowerPlantTemplate.WasteHeatGW", shipTemplate.wasteHeat_GW);
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x00058F79 File Offset: 0x00057179
	public string GetLocalizedEfficiency()
	{
		return Loc.T("TIPowerPlantTemplate.Efficiency", new object[] { this.efficiency.ToPercent("P1") });
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x00058F9E File Offset: 0x0005719E
	public string GetLocalizedSpecificPower()
	{
		return Loc.T("TIPowerPlantTemplate.SpecificPower", new object[] { this.specificPower_tGW.ToString(TIUtilities.DecimalPlaces((double)this.specificPower_tGW, 7, 0)) });
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x00058FCC File Offset: 0x000571CC
	public string GetLocalizedMaximumOutput()
	{
		return TIUtilities.LocalizeGW("TIPowerPlantTemplate.MaximumOutputGW", this.maxOutput_GW);
	}

	// Token: 0x06001298 RID: 4760 RVA: 0x00058FE0 File Offset: 0x000571E0
	public string GetLocalizedCost(TISpaceShipTemplate shipTemplate)
	{
		return Loc.T("UI.Fleets.Cost", new object[] { shipTemplate.powerPlantBuildCost.ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x06001299 RID: 4761 RVA: 0x00059018 File Offset: 0x00057218
	public string GetLocalizedCostPerGW()
	{
		float num = this.buildMass_tons(1f, 0f, 0f, 0f, true);
		TIResourcesCost tiresourcesCost = this.weightedBuildMaterials.ToResourcesCost(num * TemplateManager.global.spaceResourceToTons);
		return Loc.T("TIPowerPlantTemplate.SpecificResources", new object[] { tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x040010F9 RID: 4345
	public float maxOutput_GW;

	// Token: 0x040010FA RID: 4346
	public float specificPower_tGW;

	// Token: 0x040010FB RID: 4347
	public PowerPlantRequirement powerPlantClass;

	// Token: 0x040010FC RID: 4348
	public float efficiency;
}
