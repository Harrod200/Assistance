using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003D2 RID: 978
public class TIRadiatorTemplate : TIShipPartTemplate
{
	// Token: 0x17000238 RID: 568
	// (get) Token: 0x060012A7 RID: 4775 RVA: 0x00059234 File Offset: 0x00057434
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType> { ShipModuleSlotType.Radiator };
		}
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x060012A8 RID: 4776 RVA: 0x00059242 File Offset: 0x00057442
	public override TIRadiatorTemplate ref_radiator
	{
		get
		{
			return this;
		}
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x060012A9 RID: 4777 RVA: 0x00059245 File Offset: 0x00057445
	public override bool isRadiator
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x060012AA RID: 4778 RVA: 0x00059248 File Offset: 0x00057448
	public override bool hasModel
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060012AB RID: 4779 RVA: 0x0005924B File Offset: 0x0005744B
	public string combatUIPath_On_OK(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_").Append(this.combatUIpath).Append("_ON_A")
			.ToString();
	}

	// Token: 0x060012AC RID: 4780 RVA: 0x0005927E File Offset: 0x0005747E
	public string combatUIPath_Off_OK(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_radiator_OFF_A").ToString();
	}

	// Token: 0x060012AD RID: 4781 RVA: 0x0005929C File Offset: 0x0005749C
	public string combatUIPath_On_Damaged(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_").Append(this.combatUIpath).Append("_ON_B")
			.ToString();
	}

	// Token: 0x060012AE RID: 4782 RVA: 0x000592CF File Offset: 0x000574CF
	public string combatUIPath_Off_Damaged(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_radiator_OFF_B").ToString();
	}

	// Token: 0x060012AF RID: 4783 RVA: 0x000592ED File Offset: 0x000574ED
	public string combatUIPath_On_Destroyed(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_").Append(this.combatUIpath).Append("_ON_C")
			.ToString();
	}

	// Token: 0x060012B0 RID: 4784 RVA: 0x00059320 File Offset: 0x00057520
	public string combatUIPath_Off_Destroyed(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_mini_radiator_OFF_C").ToString();
	}

	// Token: 0x060012B1 RID: 4785 RVA: 0x0005933E File Offset: 0x0005753E
	public string largecombatUI_On(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_").Append(this.combatUIpath).Append("_ON")
			.ToString();
	}

	// Token: 0x060012B2 RID: 4786 RVA: 0x00059371 File Offset: 0x00057571
	public string largecombatUI_Off(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("_radiator_OFF").ToString();
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x00059390 File Offset: 0x00057590
	public string radiatorUIResourcePath(TIShipHullTemplate hull, int idx)
	{
		return new StringBuilder(hull.combatUIpath[idx]).Append("/radiators/").Append(hull.path2[idx]).Append("_mini_")
			.Append(this.combatUIpath)
			.Append("_ON_A")
			.ToString();
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x000593E5 File Offset: 0x000575E5
	public float radiatorSurfaceArea_m2(float wasteHeat_GW)
	{
		return this.buildMass_kg(wasteHeat_GW) / this.specificMass_2s_kgm2;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x000593F5 File Offset: 0x000575F5
	public float radiatorArea_m2(float wasteHeat_GW)
	{
		return this.radiatorSurfaceArea_m2(wasteHeat_GW) / 2f;
	}

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x060012B6 RID: 4790 RVA: 0x00059404 File Offset: 0x00057604
	public float specificMass_tonsm2
	{
		get
		{
			return this.specificMass_2s_kgm2 / 1000f;
		}
	}

	// Token: 0x060012B7 RID: 4791 RVA: 0x00059412 File Offset: 0x00057612
	public float buildMass_kg(float wasteHeat_GW)
	{
		return wasteHeat_GW * 1000000f / this.specificPower_2s_KWkg;
	}

	// Token: 0x060012B8 RID: 4792 RVA: 0x00059422 File Offset: 0x00057622
	public override float buildMass_tons(float wasteHeat_GW, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.buildMass_kg(wasteHeat_GW) / 1000f;
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x060012B9 RID: 4793 RVA: 0x00059431 File Offset: 0x00057631
	public float tonsPerGW
	{
		get
		{
			return this.buildMass_tons(1f, 0f, 0f, 0f, false);
		}
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x060012BA RID: 4794 RVA: 0x0005944E File Offset: 0x0005764E
	public override bool exoFighterPart
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060012BB RID: 4795 RVA: 0x00059454 File Offset: 0x00057654
	public override TIResourcesCost buildCost(float wasteHeat_GW, float value2 = 0f)
	{
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		tiresourcesCost.SumCosts_NoDuration(this.weightedBuildMaterials.ToResourcesCost(this.buildMass_tons(wasteHeat_GW, value2, 0f, 0f, false) * TemplateManager.global.spaceResourceToTons));
		return tiresourcesCost;
	}

	// Token: 0x060012BC RID: 4796 RVA: 0x00059498 File Offset: 0x00057698
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!prospective && shipTemplate.radiatorMass_tons != 0f)
		{
			stringBuilder.AppendLine(this.GetLocalizedMass(shipTemplate));
			stringBuilder.AppendLine(this.GetLocalizedCost(shipTemplate));
		}
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedVulnerability());
		stringBuilder.AppendLine(this.GetLocalizedTonsPerGW());
		stringBuilder.AppendLine(this.GetLocalizedCostPerGW());
		return stringBuilder.ToString();
	}

	// Token: 0x060012BD RID: 4797 RVA: 0x0005951A File Offset: 0x0005771A
	public override float AIScoringValueForResearch()
	{
		return 10f * (this.specificPower_2s_KWkg * this.specificPower_2s_KWkg * this.specificPower_2s_KWkg) / (float)this.vulnerability;
	}

	// Token: 0x060012BE RID: 4798 RVA: 0x00059540 File Offset: 0x00057740
	public string GetLocalizedMass(TISpaceShipTemplate shipTemplate)
	{
		return Loc.T("UI.Fleets.Mass", new object[] { TIUtilities.FormatBigOrSmallNumber(shipTemplate.radiatorMass_tons, 1, 7, 0, false, false) });
	}

	// Token: 0x060012BF RID: 4799 RVA: 0x00059570 File Offset: 0x00057770
	public string GetLocalizedCost(TISpaceShipTemplate shipTemplate)
	{
		return Loc.T("UI.Fleets.Cost", new object[] { shipTemplate.radiatorsBuildCost.ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x060012C0 RID: 4800 RVA: 0x000595A8 File Offset: 0x000577A8
	public string GetLocalizedVulnerability()
	{
		int num = this.vulnerability;
		string text;
		if (num <= 1)
		{
			text = Loc.T("TIRadiatorTemplate.VeryLow");
		}
		else
		{
			int num2 = num;
			if (num2 >= 2 && num2 <= 4)
			{
				text = Loc.T("TIRadiatorTemplate.Low");
			}
			else
			{
				int num3 = num;
				if (num3 >= 5 && num3 <= 10)
				{
					text = Loc.T("TIRadiatorTemplate.Moderate");
				}
				else
				{
					int num4 = num;
					if (num4 >= 11 && num4 <= 20)
					{
						text = Loc.T("TIRadiatorTemplate.High");
					}
					else
					{
						text = Loc.T("TIRadiatorTemplate.VeryHigh");
					}
				}
			}
		}
		return Loc.T("TIRadiatorTemplate.CombatVulnerability", new object[] { text });
	}

	// Token: 0x060012C1 RID: 4801 RVA: 0x00059638 File Offset: 0x00057838
	public string GetLocalizedTonsPerGW()
	{
		return Loc.T("TIRadiatorTemplate.TonsPerGW", new object[] { this.tonsPerGW.ToString(TIUtilities.DecimalPlaces((double)this.tonsPerGW, 7, 0)) });
	}

	// Token: 0x060012C2 RID: 4802 RVA: 0x00059674 File Offset: 0x00057874
	public string GetLocalizedCostPerGW()
	{
		return Loc.T("TIRadiatorTemplate.CostPerGW", new object[] { this.buildCost(1f, 0f).ToString("Relevant", false, false, null, false, FactionResource.None) });
	}

	// Token: 0x04001100 RID: 4352
	public RadiatorType radiatorType;

	// Token: 0x04001101 RID: 4353
	public float operatingTemp_K;

	// Token: 0x04001102 RID: 4354
	public float specificMass_2s_kgm2;

	// Token: 0x04001103 RID: 4355
	public float specificPower_2s_KWkg;

	// Token: 0x04001104 RID: 4356
	public int vulnerability;

	// Token: 0x04001105 RID: 4357
	public bool collector;
}
