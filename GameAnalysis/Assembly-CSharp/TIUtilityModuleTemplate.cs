using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020003D4 RID: 980
public class TIUtilityModuleTemplate : TIShipModuleTemplate
{
	// Token: 0x17000242 RID: 578
	// (get) Token: 0x060012CB RID: 4811 RVA: 0x00059768 File Offset: 0x00057968
	public float marineOpsValue
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.Assault && this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.MarineOpsDefenseOnly)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000243 RID: 579
	// (get) Token: 0x060012CC RID: 4812 RVA: 0x00059793 File Offset: 0x00057993
	public float ECMValue
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.ECM)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000244 RID: 580
	// (get) Token: 0x060012CD RID: 4813 RVA: 0x000597B0 File Offset: 0x000579B0
	public float targetingValue
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.TargetingComputer)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000245 RID: 581
	// (get) Token: 0x060012CE RID: 4814 RVA: 0x000597CD File Offset: 0x000579CD
	public float fleetMCValue
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.ReduceFleetMCConsumption)
			{
				return 1f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000246 RID: 582
	// (get) Token: 0x060012CF RID: 4815 RVA: 0x000597EA File Offset: 0x000579EA
	public float thrustMultiplier
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.ThrustMultiplier)
			{
				return 1f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000247 RID: 583
	// (get) Token: 0x060012D0 RID: 4816 RVA: 0x00059806 File Offset: 0x00057A06
	public float EVMultiplier
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.EVMultiplier)
			{
				return 1f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000248 RID: 584
	// (get) Token: 0x060012D1 RID: 4817 RVA: 0x00059822 File Offset: 0x00057A22
	public float laserPowerBonus_MW
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.LaserPowerBonus)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000249 RID: 585
	// (get) Token: 0x060012D2 RID: 4818 RVA: 0x0005983E File Offset: 0x00057A3E
	public float shipSpaceScienceModuleResearchBonus
	{
		get
		{
			if (!this.specialModuleRules.Contains(SpecialModuleRule.GenerateSpaceScienceBonus))
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x1700024A RID: 586
	// (get) Token: 0x060012D3 RID: 4819 RVA: 0x0005985B File Offset: 0x00057A5B
	public float salvageBonus
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.SalvageBonus)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x1700024B RID: 587
	// (get) Token: 0x060012D4 RID: 4820 RVA: 0x00059878 File Offset: 0x00057A78
	public float armorMaxBonus
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.ArmorStruts)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x1700024C RID: 588
	// (get) Token: 0x060012D5 RID: 4821 RVA: 0x00059895 File Offset: 0x00057A95
	public float componentArmorValue
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.ComponentArmor)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x1700024D RID: 589
	// (get) Token: 0x060012D6 RID: 4822 RVA: 0x000598B2 File Offset: 0x00057AB2
	public float vectorThrustBonus
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.RotationalThrust)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x1700024E RID: 590
	// (get) Token: 0x060012D7 RID: 4823 RVA: 0x000598CF File Offset: 0x00057ACF
	public float particleBeamPowerBonus_MW
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.ParticleBeamPowerBonus)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x1700024F RID: 591
	// (get) Token: 0x060012D8 RID: 4824 RVA: 0x000598EC File Offset: 0x00057AEC
	public float magazineAmmoBonus
	{
		get
		{
			if (this.specialModuleRules.FirstOrDefault<SpecialModuleRule>() != SpecialModuleRule.Magazine)
			{
				return 0f;
			}
			return this.specialModuleValue;
		}
	}

	// Token: 0x17000250 RID: 592
	// (get) Token: 0x060012D9 RID: 4825 RVA: 0x00059909 File Offset: 0x00057B09
	public bool requiresHydrogenPropellant
	{
		get
		{
			return this.specialModuleRules.Contains(SpecialModuleRule.RequiresHydrogenPropellant);
		}
	}

	// Token: 0x17000251 RID: 593
	// (get) Token: 0x060012DA RID: 4826 RVA: 0x00059918 File Offset: 0x00057B18
	public bool requiresNuclearDrive
	{
		get
		{
			return this.specialModuleRules.Contains(SpecialModuleRule.RequiresNuclearDrive);
		}
	}

	// Token: 0x17000252 RID: 594
	// (get) Token: 0x060012DB RID: 4827 RVA: 0x00059927 File Offset: 0x00057B27
	public bool requiresFissionDrive
	{
		get
		{
			return this.specialModuleRules.Contains(SpecialModuleRule.RequiresFissionDrive);
		}
	}

	// Token: 0x17000253 RID: 595
	// (get) Token: 0x060012DC RID: 4828 RVA: 0x00059936 File Offset: 0x00057B36
	public bool requiresFusionDrive
	{
		get
		{
			return this.specialModuleRules.Contains(SpecialModuleRule.RequiresFusionDrive);
		}
	}

	// Token: 0x17000254 RID: 596
	// (get) Token: 0x060012DD RID: 4829 RVA: 0x00059945 File Offset: 0x00057B45
	public bool requiresNonISRUDrive
	{
		get
		{
			return this.specialModuleRules.Contains(SpecialModuleRule.RequiresNonISRUDrive);
		}
	}

	// Token: 0x17000255 RID: 597
	// (get) Token: 0x060012DE RID: 4830 RVA: 0x00059954 File Offset: 0x00057B54
	public override List<ShipModuleSlotType> allowedSlots
	{
		get
		{
			return new List<ShipModuleSlotType> { ShipModuleSlotType.Utility };
		}
	}

	// Token: 0x17000256 RID: 598
	// (get) Token: 0x060012DF RID: 4831 RVA: 0x00059962 File Offset: 0x00057B62
	public override TIUtilityModuleTemplate ref_utilityModule
	{
		get
		{
			return this;
		}
	}

	// Token: 0x17000257 RID: 599
	// (get) Token: 0x060012E0 RID: 4832 RVA: 0x00059965 File Offset: 0x00057B65
	public override bool isUtilityModule
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000258 RID: 600
	// (get) Token: 0x060012E1 RID: 4833 RVA: 0x00059968 File Offset: 0x00057B68
	public override string description
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.dataName).ToString(), new object[]
			{
				this.laserPowerBonus_MW.ToString(),
				(this.thrustMultiplier - 1f).ToPercent("P0"),
				(this.EVMultiplier - 1f).ToPercent("P0"),
				this.marineOpsValue.ToString(),
				this.powerRequirement_MW.ToString(),
				this.shipSpaceScienceModuleResearchBonus.ToPercent("P0"),
				this.ECMValue.ToPercent("P0"),
				(1f - this.fleetMCValue).ToPercent("P0"),
				this.salvageBonus.ToPercent("P0"),
				this.targetingValue.ToPercent("P0"),
				(1f - this.componentArmorValue).ToPercent("P0"),
				this.armorMaxBonus.ToPercent("P0"),
				TIUtilities.FormatBigNumber((double)this.vectorThrustBonus, 1, false),
				this.particleBeamPowerBonus_MW.ToString(),
				this.magazineAmmoBonus.ToPercent("P0")
			});
		}
	}

	// Token: 0x060012E2 RID: 4834 RVA: 0x00059AD9 File Offset: 0x00057CD9
	public override float buildMass_tons(float value1 = 0f, float value2 = 0f, float value3 = 0f, float value4 = 0f, bool bValue = false)
	{
		return this.mass_tons;
	}

	// Token: 0x060012E3 RID: 4835 RVA: 0x00059AE4 File Offset: 0x00057CE4
	public override string GetDescriptionData(TISpaceShipState ship, TISpaceShipTemplate shipTemplate = null, bool prospective = false, ShipModuleSlotType slot = ShipModuleSlotType.None, bool splitFireModes = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(this.GetLocalizedMass());
		if (this.crew > 0)
		{
			stringBuilder.AppendLine(base.GetLocalizedCrew());
		}
		stringBuilder.AppendLine(this.GetLocalizedCost());
		if (this.requiresHydrogenPropellant)
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.RequiresHydrogenPropellant"));
		}
		if (this.requiresFusionDrive)
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.RequiresFusionDrive"));
		}
		else if (this.requiresFissionDrive)
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.RequiresFissionDrive"));
		}
		else if (this.requiresNuclearDrive)
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.RequiresNuclearDrive"));
		}
		if (this.requiresNonISRUDrive)
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.RequiresNonISRUDrive"));
		}
		if (this.minConsTier > 1)
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.ConsTier"));
		}
		if (this.specialModuleRules.Contains(SpecialModuleRule.ImmuneToDamage))
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.ImmuneToDamage"));
		}
		if (this.specialModuleRules.Contains(SpecialModuleRule.FullRepairCost))
		{
			stringBuilder.AppendLine(Loc.T("TIUtilityModuleTemplate.description.FullRepairCost", new object[] { TemplateManager.global.shipPartRepairBaseCostMultiplier.ToPercent("P0") }));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x17000259 RID: 601
	// (get) Token: 0x060012E4 RID: 4836 RVA: 0x00059C25 File Offset: 0x00057E25
	public override float repairCostMultipler
	{
		get
		{
			if (!this.specialModuleRules.Contains(SpecialModuleRule.FullRepairCost))
			{
				return TemplateManager.global.shipPartRepairBaseCostMultiplier;
			}
			return 1f;
		}
	}

	// Token: 0x04001107 RID: 4359
	public int grouping = -1;

	// Token: 0x04001108 RID: 4360
	public float powerRequirement_MW;

	// Token: 0x04001109 RID: 4361
	public float specialModuleValue;

	// Token: 0x0400110A RID: 4362
	public int minConsTier;

	// Token: 0x0400110B RID: 4363
	public List<SpecialModuleRule> specialModuleRules = new List<SpecialModuleRule>();
}
