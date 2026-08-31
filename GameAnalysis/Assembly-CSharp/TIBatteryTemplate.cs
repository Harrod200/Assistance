using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003D1 RID: 977
public class TIBatteryTemplate : TIUtilityModuleTemplate
{
	// Token: 0x17000234 RID: 564
	// (get) Token: 0x0600129B RID: 4763 RVA: 0x00059084 File Offset: 0x00057284
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType> { ShipModuleSlotType.Utility };
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x0600129C RID: 4764 RVA: 0x00059092 File Offset: 0x00057292
	public override TIBatteryTemplate ref_battery
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x0600129D RID: 4765 RVA: 0x00059095 File Offset: 0x00057295
	public override bool isBattery
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600129E RID: 4766 RVA: 0x00059098 File Offset: 0x00057298
	public float GetTimeToFullCharge_minutes(bool fighterHull)
	{
		return this.GetCapacity(fighterHull) / this.rechargeRate_GJs / 60f;
	}

	// Token: 0x0600129F RID: 4767 RVA: 0x000590AE File Offset: 0x000572AE
	public float GetCapacity(bool fighterHull)
	{
		return this.energyCapacity_GJ * (fighterHull ? 0.1f : 1f);
	}

	// Token: 0x060012A0 RID: 4768 RVA: 0x000590C6 File Offset: 0x000572C6
	public override float buildMass_tons(float value1 = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.mass_tons * (bValue ? 0.1f : 1f);
	}

	// Token: 0x060012A1 RID: 4769 RVA: 0x000590E0 File Offset: 0x000572E0
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag3;
		if (ship == null)
		{
			bool? flag;
			if (shipTemplate == null)
			{
				flag = null;
			}
			else
			{
				TIShipHullTemplate hullTemplate = shipTemplate.hullTemplate;
				flag = ((hullTemplate != null) ? new bool?(hullTemplate.noShipyardBuild) : null);
			}
			bool? flag2 = flag;
			flag3 = flag2.GetValueOrDefault();
		}
		else
		{
			flag3 = ship.template.hullTemplate.noShipyardBuild;
		}
		bool flag4 = flag3;
		stringBuilder.AppendLine(this.GetLocalizedMass());
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedCapacity(flag4));
		stringBuilder.AppendLine(this.GetLocalizedRechargeRateInMinutes(flag4));
		stringBuilder.AppendLine(this.GetLocalizedCost());
		return stringBuilder.ToString();
	}

	// Token: 0x060012A2 RID: 4770 RVA: 0x00059190 File Offset: 0x00057390
	public string GetLocalizedCapacity(bool fighterHull)
	{
		return Loc.T("TIBatteryTemplate.Capacity", new object[] { this.GetCapacity(fighterHull).ToString("N0") });
	}

	// Token: 0x060012A3 RID: 4771 RVA: 0x000591C4 File Offset: 0x000573C4
	public string GetLocalizedRechargeRateInMinutes(bool fighterHull)
	{
		return Loc.T("TIBatteryTemplate.RechargeRateMin", new object[] { this.GetTimeToFullCharge_minutes(fighterHull).ToString("N0") });
	}

	// Token: 0x060012A4 RID: 4772 RVA: 0x000591F8 File Offset: 0x000573F8
	public override float AIScoringValueForResearch()
	{
		return this.energyCapacity_GJ * this.rechargeRate_GJs * 100f / this.buildMass_tons(0f, 0f, 0f, 0f, false);
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x060012A5 RID: 4773 RVA: 0x00059229 File Offset: 0x00057429
	public override bool exoFighterPart
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040010FD RID: 4349
	public const float fighterVariantSizeReduction = 0.1f;

	// Token: 0x040010FE RID: 4350
	public float energyCapacity_GJ;

	// Token: 0x040010FF RID: 4351
	public float rechargeRate_GJs;
}
