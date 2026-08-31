using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Ship;
using UnityEngine;

// Token: 0x02000368 RID: 872
public class TIHabModuleTemplate : TIDataTemplate
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06000F7A RID: 3962 RVA: 0x0004F6C4 File Offset: 0x0004D8C4
	public int slotsProvided
	{
		get
		{
			if (!this.coreModule)
			{
				return 0;
			}
			switch (this.tier)
			{
			case 1:
				return 4;
			case 2:
				return 12;
			case 3:
				return 20;
			default:
				return 0;
			}
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x06000F7B RID: 3963 RVA: 0x0004F704 File Offset: 0x0004D904
	public int weaponMounts
	{
		get
		{
			if (this.spaceCombatModule)
			{
				int num = this.tier;
				if (num - 1 <= 1)
				{
					return 3;
				}
				if (num == 3)
				{
					if (!this.alienModule)
					{
						return 4;
					}
					return 3;
				}
			}
			return 0;
		}
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x06000F7C RID: 3964 RVA: 0x0004F73B File Offset: 0x0004D93B
	public bool constructionModule
	{
		get
		{
			return !this.allowsShipConstruction && this.constructionTimeModifier < 1f;
		}
	}

	// Token: 0x170001B7 RID: 439
	// (get) Token: 0x06000F7D RID: 3965 RVA: 0x0004F754 File Offset: 0x0004D954
	public bool EnablesLocalFounding
	{
		get
		{
			return this.SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier1Habs) || this.SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier2Habs) || this.SpecialRules.Contains(HabModuleSpecialRule.CanFoundTier3Habs);
		}
	}

	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06000F7E RID: 3966 RVA: 0x0004F782 File Offset: 0x0004D982
	public bool IsSolarPower
	{
		get
		{
			return this.powerSource && this.SpecialRules.Contains(HabModuleSpecialRule.Solar_Power_Variable_Output);
		}
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06000F7F RID: 3967 RVA: 0x0004F79A File Offset: 0x0004D99A
	public bool IsNonSolarPower
	{
		get
		{
			return this.powerSource && !this.IsSolarPower;
		}
	}

	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06000F80 RID: 3968 RVA: 0x0004F7AF File Offset: 0x0004D9AF
	public bool IsFarm
	{
		get
		{
			return this.specialRules.Contains(HabModuleSpecialRule.Farm);
		}
	}

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06000F81 RID: 3969 RVA: 0x0004F7BE File Offset: 0x0004D9BE
	public float FarmValue
	{
		get
		{
			if (!this.IsFarm)
			{
				return 0f;
			}
			return this.specialRulesValue;
		}
	}

	// Token: 0x06000F82 RID: 3970 RVA: 0x0004F7D4 File Offset: 0x0004D9D4
	public float GetFarmResourceValue(FactionResource resource)
	{
		if (resource == FactionResource.Water)
		{
			return this.FarmValue * TemplateManager.global.crewWaterConsumptionTons_year / 12f * TemplateManager.global.spaceResourceToTons;
		}
		if (resource != FactionResource.Volatiles)
		{
			return 0f;
		}
		return this.FarmValue * TemplateManager.global.crewVolatilesConsumptionTons_year / 12f * TemplateManager.global.spaceResourceToTons;
	}

	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06000F83 RID: 3971 RVA: 0x0004F837 File Offset: 0x0004DA37
	public float dimension_m
	{
		get
		{
			return (float)this.tier * 20f - 10f;
		}
	}

	// Token: 0x06000F84 RID: 3972 RVA: 0x0004F84C File Offset: 0x0004DA4C
	public float GetCrossSectionalArea_m2(float angle = 3.4028235E+38f)
	{
		return this.dimension_m;
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0004F854 File Offset: 0x0004DA54
	public float GetSpecialRuleValue(HabModuleSpecialRule rule)
	{
		if (!this.specialRules.Contains(rule))
		{
			return 0f;
		}
		return this.specialRulesValue;
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06000F86 RID: 3974 RVA: 0x0004F870 File Offset: 0x0004DA70
	public float StationModuleArmorPoints
	{
		get
		{
			return (float)(this.tier * 5);
		}
	}

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06000F87 RID: 3975 RVA: 0x0004F87B File Offset: 0x0004DA7B
	public float AlienDetectionBonus
	{
		get
		{
			return (float)(this.specialRules.Contains(HabModuleSpecialRule.LEOBonusAlienDetection) ? this.tier : 0);
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06000F88 RID: 3976 RVA: 0x0004F896 File Offset: 0x0004DA96
	public float HumanDetectionBonus
	{
		get
		{
			return (float)(this.specialRules.Contains(HabModuleSpecialRule.LEOBonusHumanDetection) ? this.tier : 0);
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x06000F89 RID: 3977 RVA: 0x0004F8B1 File Offset: 0x0004DAB1
	public float PropandaStrengthBonus
	{
		get
		{
			return this.GetSpecialRuleValue(HabModuleSpecialRule.LEOBonusPropagandaStrength);
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06000F8A RID: 3978 RVA: 0x0004F8BB File Offset: 0x0004DABB
	public float ArmyCombatValueBonus
	{
		get
		{
			return this.GetSpecialRuleValue(HabModuleSpecialRule.LEOBonusArmyCombatValue);
		}
	}

	// Token: 0x170001C2 RID: 450
	// (get) Token: 0x06000F8B RID: 3979 RVA: 0x0004F8C5 File Offset: 0x0004DAC5
	public float EfficiencyBonus
	{
		get
		{
			return this.GetSpecialRuleValue(HabModuleSpecialRule.Efficiency);
		}
	}

	// Token: 0x170001C3 RID: 451
	// (get) Token: 0x06000F8C RID: 3980 RVA: 0x0004F8CF File Offset: 0x0004DACF
	public bool PowerFirst
	{
		get
		{
			return this.specialRules.Contains(HabModuleSpecialRule.PowerFirst);
		}
	}

	// Token: 0x170001C4 RID: 452
	// (get) Token: 0x06000F8D RID: 3981 RVA: 0x0004F8DE File Offset: 0x0004DADE
	public string description
	{
		get
		{
			return Loc.T(new StringBuilder("TIHabModuleTemplate.description.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170001C5 RID: 453
	// (get) Token: 0x06000F8E RID: 3982 RVA: 0x0004F900 File Offset: 0x0004DB00
	public List<HabModuleSpecialRule> SpecialRules
	{
		get
		{
			if (this._specialRules == null)
			{
				this._specialRules = this.specialRules.Where<HabModuleSpecialRule>((HabModuleSpecialRule x) => x > HabModuleSpecialRule.none).ToList<HabModuleSpecialRule>();
			}
			return this._specialRules;
		}
	}

	// Token: 0x170001C6 RID: 454
	// (get) Token: 0x06000F8F RID: 3983 RVA: 0x0004F950 File Offset: 0x0004DB50
	public string extendedDescription
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(this.description);
			if (this.onePerHab)
			{
				stringBuilder.AppendLine(Loc.T("TIHabModuleTemplate.OnePerHab"));
			}
			if (this.habType == HabType.Base)
			{
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("TIHabModuleTemplate.BaseOnly")));
			}
			else if (this.habType == HabType.Station)
			{
				stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("TIHabModuleTemplate.StationOnly")));
			}
			foreach (HabModuleSpecialRule habModuleSpecialRule in this.SpecialRules)
			{
				string text = Loc.T(new StringBuilder("HabModuleSpecialRule.").Append(habModuleSpecialRule.ToString()).ToString(), new object[]
				{
					this.specialRulesValue.ToString("N0"),
					this.specialRulesValue.ToPercent("P0"),
					this.specialRulesValue.ToString("N2"),
					this.tier.ToString("N0"),
					8.ToString("N0"),
					(this.power * 8).ToString("N0")
				});
				if (habModuleSpecialRule == HabModuleSpecialRule.Farm)
				{
					TIResourcesCost tiresourcesCost = new TIResourcesCost();
					tiresourcesCost.AddCost(FactionResource.Water, this.specialRulesValue * TemplateManager.global.crewWaterConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f, true);
					tiresourcesCost.AddCost(FactionResource.Volatiles, this.specialRulesValue * TemplateManager.global.crewVolatilesConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f, true);
					text = new StringBuilder(text).Append(Loc.T("UI.Habs.Paren", new object[] { tiresourcesCost.ToString("Relevant", false, false, null, false, FactionResource.None) })).ToString();
				}
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendLine(text);
				}
			}
			return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
		}
	}

	// Token: 0x06000F90 RID: 3984 RVA: 0x0004FB78 File Offset: 0x0004DD78
	public string benefitsAndCostsDescription(TIFactionState faction, TIHabState hab, bool prospectiveForHab = false)
	{
		TIGlobalConfig global = TemplateManager.global;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Loc.T("UI.Habs.SimpleList", new object[]
		{
			Loc.T("UI.Habs.Tier", new object[] { this.tier.ToString("N0") }),
			Loc.T("UI.Habs.Crew", new object[] { this.crew.ToString("N0") }),
			Loc.T("UI.Habs.Mass", new object[] { (hab == null) ? this.Mass_tons(0f, GameStateManager.Luna(), GameStateManager.Luna(), faction).ToString("N0") : this.Mass_tons(hab.irradiatedMultiplier, ((hab != null) ? hab.ref_spaceBody : null) ?? null, ((hab != null) ? hab.ref_naturalSpaceObject : null) ?? null, faction).ToString("N0") })
		}));
		List<TIHabModuleTemplate.IncomeEntry> list = new List<TIHabModuleTemplate.IncomeEntry>();
		List<TIHabModuleTemplate.IncomeEntry> list2 = new List<TIHabModuleTemplate.IncomeEntry>();
		List<TIHabModuleTemplate.IncomeEntry> list3 = new List<TIHabModuleTemplate.IncomeEntry>();
		int num;
		if (hab == null)
		{
			num = this.ProspectivePower(GameStateManager.Luna(), faction);
		}
		else
		{
			num = this.ProspectivePower(hab);
		}
		if (num > 0)
		{
			list.Add(new TIHabModuleTemplate.IncomeEntry(global.habPowerInlineSpritePath, num.ToString("N0")));
			if (this.IsSolarPower && hab == null)
			{
				num = this.ProspectivePower(GameStateManager.Earth().orbits.MaxBy<TIOrbitState, double>((TIOrbitState x) => x.altitude_km), faction);
				list.Add(new TIHabModuleTemplate.IncomeEntry(global.orbitInlineSpritePath, num.ToString("N0")));
			}
		}
		else
		{
			list2.Add(new TIHabModuleTemplate.IncomeEntry(global.habPowerAlertInlineSpritePath, num.ToString("N0")));
		}
		Dictionary<FactionResource, float> dictionary = new Dictionary<FactionResource, float>();
		Dictionary<FactionResource, float> dictionary2 = new Dictionary<FactionResource, float>();
		Dictionary<FactionResource, float> dictionary3 = new Dictionary<FactionResource, float>();
		foreach (FactionResource factionResource in Enums.FactionResources)
		{
			dictionary[factionResource] = this.MonthlyResourceIncome(factionResource, hab, faction);
			if (dictionary[factionResource] != 0f)
			{
				string text = TIUtilities.FormatBigOrSmallNumber(dictionary[factionResource], 1, 3, 0, true, false);
				if (dictionary[factionResource] > 0f)
				{
					list.Add(new TIHabModuleTemplate.IncomeEntry(TIUtilities.InlineResourceStr(factionResource), text));
				}
				else
				{
					list2.Add(new TIHabModuleTemplate.IncomeEntry(TIUtilities.InlineResourceStr(factionResource), text));
				}
			}
			dictionary2[factionResource] = this.MonthlySupportCost(factionResource, false, faction, hab);
			if (dictionary2[factionResource] != 0f)
			{
				string text2 = TIUtilities.FormatBigOrSmallNumber(dictionary2[factionResource], 1, 3, 0, true, false);
				list2.Add(new TIHabModuleTemplate.IncomeEntry(TIUtilities.InlineResourceStr(factionResource), text2));
			}
			dictionary3[factionResource] = this.MonthlyCrewSupportCost(factionResource, faction, hab);
			if (dictionary3[factionResource] != 0f)
			{
				string text3 = TIUtilities.FormatBigOrSmallNumber(dictionary3[factionResource], 1, 3, 0, true, false);
				list3.Add(new TIHabModuleTemplate.IncomeEntry(TIUtilities.InlineResourceStr(factionResource), text3));
			}
		}
		int num2 = this.ControlPointCapacity(hab == null || hab.inEarthLEO);
		if (num2 > 0)
		{
			list.Add(new TIHabModuleTemplate.IncomeEntry(((faction != null) ? faction.inlineControlPointCapIcon : null) ?? TemplateManager.global.controlPointInlineSpritePath_empty, num2.ToString("N0")));
		}
		foreach (TechCategory techCategory in Enums.TechCategories)
		{
			float techBonusByCategory = this.GetTechBonusByCategory(techCategory);
			string text4 = techBonusByCategory.ToPercent((techBonusByCategory * 100f % 1f > 0f) ? "P1" : "P0");
			if (techBonusByCategory != 0f)
			{
				list.Add(new TIHabModuleTemplate.IncomeEntry(TIGenericTechTemplate.categoryInlineSprite(techCategory), text4));
			}
		}
		if (this.allowsResupply)
		{
			list.Add(new TIHabModuleTemplate.IncomeEntry(global.habResupplyPresentInlineSpritePath, string.Empty));
		}
		float moduleConstructionSpeedModifier = this.moduleConstructionSpeedModifier;
		if (moduleConstructionSpeedModifier > 1f)
		{
			list.Add(new TIHabModuleTemplate.IncomeEntry(global.habModuleConstructionInlineSpritePath, (1f - moduleConstructionSpeedModifier).ToPercent("P0")));
		}
		if (this.allowsShipConstruction)
		{
			list.Add(new TIHabModuleTemplate.IncomeEntry(global.habShipyardPresentInlineSpritePath, string.Empty));
		}
		float num3 = this.SpaceCombatValue(faction, hab, hab != null);
		if (num3 != 0f)
		{
			string text5 = num3.ToString("N0");
			list.Add(new TIHabModuleTemplate.IncomeEntry(global.habDefenseScoreInlineSpritePath, text5));
		}
		if (this.CombatTroops())
		{
			list.Add(new TIHabModuleTemplate.IncomeEntry(global.spaceAssaultValueInlineSpritePath, this.specialRulesValue.ToString("N0")));
		}
		if (list.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Habs.IncomeAndBonuses"));
			foreach (TIHabModuleTemplate.IncomeEntry incomeEntry in list)
			{
				stringBuilder.Append(incomeEntry.inlinePath).Append(incomeEntry.value).Append(" ");
			}
			stringBuilder.AppendLine();
		}
		if (list2.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Habs.Costs"));
			foreach (TIHabModuleTemplate.IncomeEntry incomeEntry2 in list2)
			{
				stringBuilder.Append(incomeEntry2.inlinePath).Append(incomeEntry2.value).Append(" ");
			}
			stringBuilder.AppendLine();
		}
		if (list3.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Habs.CrewCosts"));
			foreach (TIHabModuleTemplate.IncomeEntry incomeEntry3 in list3)
			{
				stringBuilder.Append(incomeEntry3.inlinePath).Append(incomeEntry3.value).Append(" ");
			}
			if (hab != null)
			{
				int num4 = hab.FarmCrewDiscount();
				if (num4 > 0 && hab.crew > 0)
				{
					float num5 = Mathf.Clamp01((float)num4 / (float)(hab.crew + (prospectiveForHab ? this.crew : 0)));
					StringBuilder stringBuilder2 = new StringBuilder();
					foreach (FactionResource factionResource2 in Enums.FactionResources)
					{
						if ((factionResource2 == FactionResource.Water || factionResource2 == FactionResource.Volatiles) && dictionary3[factionResource2] > 0f)
						{
							stringBuilder2.Append(TIUtilities.InlineResourceStr(factionResource2)).Append(dictionary3[factionResource2] * (1f - num5)).Append(" ");
						}
					}
					stringBuilder.AppendLine().Append(Loc.T("UI.Habs.FarmDiscount", new object[]
					{
						num5.ToPercent("P0"),
						stringBuilder2.ToString()
					}));
				}
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x170001C7 RID: 455
	// (get) Token: 0x06000F91 RID: 3985 RVA: 0x000502A4 File Offset: 0x0004E4A4
	public string constructionModelResource
	{
		get
		{
			switch (this.tier)
			{
			default:
				if (this.alienModule)
				{
					return TemplateManager.global.station_alien_underconstruction_t1_module;
				}
				return TemplateManager.global.station_human_underconstruction_t1_module;
			case 2:
				if (this.alienModule)
				{
					return TemplateManager.global.station_alien_underconstruction_t2_module;
				}
				return TemplateManager.global.station_human_underconstruction_t2_module;
			case 3:
				if (this.alienModule)
				{
					return TemplateManager.global.station_alien_underconstruction_t3_module;
				}
				return TemplateManager.global.station_human_underconstruction_t3_module;
			}
		}
	}

	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00050328 File Offset: 0x0004E528
	public string constructionModelDestructionResource
	{
		get
		{
			switch (this.tier)
			{
			default:
				if (this.alienModule)
				{
					return TemplateManager.global.station_alien_underconstruction_t1_module_destruction;
				}
				return TemplateManager.global.station_human_underconstruction_t1_module_destruction;
			case 2:
				if (this.alienModule)
				{
					return TemplateManager.global.station_alien_underconstruction_t2_module_destruction;
				}
				return TemplateManager.global.station_human_underconstruction_t2_module_destruction;
			case 3:
				if (this.alienModule)
				{
					return TemplateManager.global.station_alien_underconstruction_t3_module_destruction;
				}
				return TemplateManager.global.station_human_underconstruction_t3_module_destruction;
			}
		}
	}

	// Token: 0x06000F93 RID: 3987 RVA: 0x000503A9 File Offset: 0x0004E5A9
	public string iconResource(HabType habType)
	{
		if (habType != HabType.Station)
		{
			return this.baseIconResource;
		}
		return this.stationIconResource;
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x000503BC File Offset: 0x0004E5BC
	public string constructionIconResource(HabType habType)
	{
		string text = string.Empty;
		if (habType == HabType.Station)
		{
			switch (this.tier)
			{
			case 1:
				text = (this.alienModule ? TemplateManager.global.station_alien_underconstruction_t1_icon : TemplateManager.global.station_human_underconstruction_t1_icon);
				break;
			case 2:
				text = (this.alienModule ? TemplateManager.global.station_alien_underconstruction_t2_icon : TemplateManager.global.station_human_underconstruction_t2_icon);
				break;
			case 3:
				text = (this.alienModule ? TemplateManager.global.station_alien_underconstruction_t3_icon : TemplateManager.global.station_human_underconstruction_t3_icon);
				break;
			}
		}
		else if (this.mine)
		{
			text = (this.alienModule ? "habmodules/base_T3_AlienUnderconstruction" : "habmodules/base_T3_underconstruction");
		}
		else
		{
			switch (this.tier)
			{
			case 1:
				text = (this.alienModule ? "habmodules/base_T1_AlienUnderconstruction" : "habmodules/base_T1_underconstruction");
				break;
			case 2:
				text = (this.alienModule ? "habmodules/base_T2_AlienUnderconstruction" : "habmodules/base_T2_underconstruction");
				break;
			case 3:
				text = (this.alienModule ? "habmodules/base_T3_AlienUnderconstruction" : "habmodules/base_T3_underconstruction");
				break;
			}
		}
		return text;
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x000504E0 File Offset: 0x0004E6E0
	public float Mass_tons(float irradiatedValue, TISpaceBodyState surfaceBody, TINaturalSpaceObjectState barycenter, TIFactionState faction)
	{
		float num = this.baseMass_tons;
		if (this.SpecialRules.Contains(HabModuleSpecialRule.Cost_Scales_With_Gravity) && surfaceBody != null)
		{
			num = num * 0.5f + num * 0.5f * (float)surfaceBody.relativeEnergyForMining(faction);
		}
		if (irradiatedValue > 1f)
		{
			num *= irradiatedValue;
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.SolarMirror))
		{
			num *= (float)(barycenter.GetSunOrbitingRelatedObject.semiMajorAxis_AU * barycenter.GetSunOrbitingRelatedObject.semiMajorAxis_AU);
		}
		if (this.mine)
		{
			float mineSizeModifier = faction.GetMineSizeModifier();
			num *= mineSizeModifier;
		}
		return num;
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x00050570 File Offset: 0x0004E770
	public int BaseStationModuleHitPoints(TIFactionState faction, TIHabState hab)
	{
		return (int)(0.3f * this.baseMass_tons / (float)this.tier / faction.GetBestArmor(faction.IsAlienFaction && hab.ref_naturalSpaceObject == faction.primaryHab.ref_naturalSpaceObject).mass_damagePoint_kg);
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x000505BF File Offset: 0x0004E7BF
	public TIShipArmorTemplate GetBestArmor(TIFactionState faction, TINaturalSpaceObjectState location)
	{
		return faction.GetBestArmor(faction.IsAlienFaction && location == faction.primaryHab.ref_naturalSpaceObject);
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x000505E4 File Offset: 0x0004E7E4
	public float TargetingBonus(TIFactionState faction, TIHabState alliedHab)
	{
		float specialRuleValue = this.GetSpecialRuleValue(HabModuleSpecialRule.FleetTargeting);
		return specialRuleValue + TIEffectsState.SumEffectsModifiers(Context.TargetingComputerBonus, faction, specialRuleValue, null);
	}

	// Token: 0x06000F99 RID: 3993 RVA: 0x00050609 File Offset: 0x0004E809
	public float ECMValue(TIFactionState faction, TIHabState alliedHab)
	{
		return 0f;
	}

	// Token: 0x06000F9A RID: 3994 RVA: 0x00050610 File Offset: 0x0004E810
	public List<TIShipWeaponTemplate> NotionalWeaponsList(TIFactionState faction, bool isBase, TISpaceBodyState spacebody, bool includePD = true)
	{
		List<TIShipWeaponTemplate> list = new List<TIShipWeaponTemplate>();
		string bestHabWeapon = faction.GetBestHabWeapon(isBase, this.tier, WeaponClass.Laser, spacebody, null);
		if (!string.IsNullOrEmpty(bestHabWeapon))
		{
			list.Add(TemplateManager.Find<TIShipWeaponTemplate>(bestHabWeapon, true));
		}
		string bestHabWeapon2 = faction.GetBestHabWeapon(isBase, this.tier, isBase ? WeaponClass.Laser : WeaponClass.Magnetic, spacebody, null);
		if (!string.IsNullOrEmpty(bestHabWeapon2))
		{
			list.Add(TemplateManager.Find<TIShipWeaponTemplate>(bestHabWeapon2, true));
		}
		if (this.weaponMounts > 3)
		{
			string bestHabWeapon3 = faction.GetBestHabWeapon(isBase, this.tier, isBase ? WeaponClass.Laser : WeaponClass.Plasma, spacebody, null);
			if (!string.IsNullOrEmpty(bestHabWeapon3))
			{
				list.Add(TemplateManager.Find<TIShipWeaponTemplate>(bestHabWeapon3, true));
			}
		}
		if (includePD)
		{
			TIShipWeaponTemplate tishipWeaponTemplate = TemplateManager.Find<TIShipWeaponTemplate>(faction.GetBestPointDefenseWeaponTemplateName(), true);
			list.Add(tishipWeaponTemplate);
		}
		return list;
	}

	// Token: 0x06000F9B RID: 3995 RVA: 0x000506C8 File Offset: 0x0004E8C8
	public float SpaceCombatValue(TIFactionState faction, TIHabState hab, bool fullyCalculate)
	{
		if (!this.spaceCombatModule)
		{
			return 0f;
		}
		if (hab == null || !fullyCalculate)
		{
			return Mathf.Pow((float)this.tier, 2f) * (float)((faction != null && faction.IsAlienFaction) ? 2 : 1);
		}
		if (hab.IsBase)
		{
			return this.SpaceCombatValue_Base(faction, hab);
		}
		return this.SpaceCombatValue_Station(faction, hab);
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0005072C File Offset: 0x0004E92C
	public static void InvalidateHabDefenseNumbers(TIFactionState faction)
	{
		Dictionary<int, float> dictionary;
		if (TIHabModuleTemplate.cachedStationCombatModuleStrengths.TryGetValue(faction, out dictionary))
		{
			dictionary.Clear();
		}
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x00050750 File Offset: 0x0004E950
	public float SpaceCombatValue_Station(TIFactionState faction, TIHabState hab)
	{
		Dictionary<int, float> dictionary;
		if (!TIHabModuleTemplate.cachedStationCombatModuleStrengths.TryGetValue(faction, out dictionary))
		{
			dictionary = (TIHabModuleTemplate.cachedStationCombatModuleStrengths[faction] = new Dictionary<int, float>());
		}
		float num;
		if (!dictionary.TryGetValue(this.tier, out num))
		{
			TIUtilities.PushRandomState(new int?(base.dataName.GetHashCode()));
			List<TIShipWeaponTemplate> list = this.NotionalWeaponsList(faction, false, hab.ref_spaceBody, false);
			TIShipArmorTemplate bestArmor = this.GetBestArmor(faction, hab.ref_naturalSpaceObject);
			int num2 = this.BaseStationModuleHitPoints(faction, hab);
			float stationModuleArmorPoints = this.StationModuleArmorPoints;
			float num3 = 0f;
			foreach (TISpaceShipTemplate.TestCombat testCombat in TISpaceShipTemplate.TestCombats)
			{
				SimulatedCombat.SimulatedCombatHabModule simulatedCombatHabModule = new SimulatedCombat.SimulatedCombatHabModule(faction, this, bestArmor, (float)num2, stationModuleArmorPoints, 1f, list);
				List<TISpaceShipTemplate.TestCombat.Attack> list2 = testCombat.Attacks.Where<TISpaceShipTemplate.TestCombat.Attack>((TISpaceShipTemplate.TestCombat.Attack x) => x.Weapon.isAlien != this.alienModule).ToList<TISpaceShipTemplate.TestCombat.Attack>();
				float num4 = list2.Sum<TISpaceShipTemplate.TestCombat.Attack>((TISpaceShipTemplate.TestCombat.Attack x) => x.Weapon.averageCooldown_s);
				float num5 = 0f;
				foreach (TISpaceShipTemplate.TestCombat.Attack attack in list2)
				{
					num5 += attack.Weapon.averageCooldown_s;
					float num6 = 1f;
					TIProjectileWeaponTemplate tiprojectileWeaponTemplate = attack.Weapon as TIProjectileWeaponTemplate;
					if (tiprojectileWeaponTemplate != null)
					{
						if (tiprojectileWeaponTemplate.isMissileWeapon)
						{
							num6 *= 0.1f;
						}
						else
						{
							num6 *= 0.15f;
						}
					}
					if (TIUtilities.RandomFloatValue() <= num6)
					{
						float num7 = 0f;
						TIProjectileWeaponTemplate tiprojectileWeaponTemplate2 = attack.Weapon as TIProjectileWeaponTemplate;
						if (tiprojectileWeaponTemplate2 != null)
						{
							num7 = tiprojectileWeaponTemplate2.EstimatedImpactVelocity_kps;
						}
						DamageSource damageSource = SimulatedCombat.GetDamageSource(null, null, attack.Weapon, simulatedCombatHabModule, Vector3.forward, attack.ArmorFacing, attack.Range_km, num7);
						simulatedCombatHabModule.ApplyDamage(damageSource);
						if (simulatedCombatHabModule.isDestroyed)
						{
							break;
						}
					}
				}
				float num8 = num5 / num4;
				num3 += num8;
			}
			float num9 = TISpaceShipTemplate.TestCombats.Sum<TISpaceShipTemplate.TestCombat>((TISpaceShipTemplate.TestCombat x) => x.Attacks.Sum<TISpaceShipTemplate.TestCombat.Attack>((TISpaceShipTemplate.TestCombat.Attack y) => y.Weapon.averageCooldown_s)) / (float)TISpaceShipTemplate.TestCombats.Count<TISpaceShipTemplate.TestCombat>() * num3 / (float)TISpaceShipTemplate.TestCombats.Count<TISpaceShipTemplate.TestCombat>();
			float num10 = list.Sum<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.GenericScore());
			float unnormalizedSpaceCombatValueFromParameters = TISpaceShipTemplate.GetUnnormalizedSpaceCombatValueFromParameters(num9, Enumerable.Repeat<ValueTuple<float, float, float>>(new ValueTuple<float, float, float>(num10, float.PositiveInfinity, float.PositiveInfinity), 1), 1f);
			num = (dictionary[this.tier] = TISpaceShipTemplate.GetNormalizedSpaceCombatValue(unnormalizedSpaceCombatValueFromParameters, 0.1f));
			TIUtilities.PopRandomState();
		}
		return num;
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x00050A40 File Offset: 0x0004EC40
	public float SpaceCombatValue_Base(TIFactionState faction, TIHabState hab)
	{
		float num = (float)this.tier * this.NotionalWeaponsList(faction, hab.IsBase, hab.ref_spaceBody, false).Sum<TIShipWeaponTemplate>((TIShipWeaponTemplate x) => x.GenericScore());
		if (hab != null)
		{
			if (hab.IsStation)
			{
				num += 0.05f * (float)this.BaseStationModuleHitPoints(faction, hab);
				num *= 4f;
			}
			else
			{
				num *= 2f * ((float)this.tier / hab.faction.GetBestArmor(faction.IsAlienFaction && hab.ref_naturalSpaceObject == faction.primaryHab.ref_naturalSpaceObject).mass_damagePoint_kg);
			}
		}
		if (faction.IsActiveHumanFaction)
		{
			TIFactionState tifactionState = GameStateManager.AlienFaction();
			float num2;
			if (tifactionState != null && tifactionState.ships.Count > 0)
			{
				num2 = tifactionState.ships.Average<TISpaceShipState>((TISpaceShipState x) => x.SpaceCombatValue(false, 0f));
			}
			else
			{
				num2 = 250f;
			}
			float num3 = num2 * 1.6f;
			if (num > num3)
			{
				num = num3 * Mathf.Pow(num / num3, 0.5f);
			}
		}
		return num;
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x00050B74 File Offset: 0x0004ED74
	public ResourceCostBuilder BuildMaterials(float irradiatedValue, TISpaceBodyState spaceBody, TINaturalSpaceObjectState naturalSpaceObject, TIFactionState faction, float multiplier)
	{
		float num = this.Mass_tons(1f, spaceBody, naturalSpaceObject, faction);
		float num2 = this.Mass_tons(irradiatedValue, spaceBody, naturalSpaceObject, faction) - num;
		return new ResourceCostBuilder
		{
			water = this.weightedBuildMaterials.water * num * TemplateManager.global.spaceResourceToTons * multiplier + ((this.specialRules.Contains(HabModuleSpecialRule.UsesHelium3) && faction.He3Access) ? (this.weightedBuildMaterials.fissiles * num * TemplateManager.global.spaceResourceToTons * multiplier) : 0f),
			volatiles = this.weightedBuildMaterials.volatiles * num * TemplateManager.global.spaceResourceToTons * multiplier,
			metals = (this.weightedBuildMaterials.metals * num + num2) * TemplateManager.global.spaceResourceToTons * multiplier,
			nobleMetals = this.weightedBuildMaterials.nobleMetals * num * TemplateManager.global.spaceResourceToTons * multiplier,
			fissiles = ((this.specialRules.Contains(HabModuleSpecialRule.UsesHelium3) && faction.He3Access) ? 0f : (this.weightedBuildMaterials.fissiles * num * TemplateManager.global.spaceResourceToTons * multiplier)),
			antimatter = this.weightedBuildMaterials.antimatter * num * TemplateManager.global.spaceResourceToTons * multiplier,
			exotics = this.weightedBuildMaterials.exotics * num * TemplateManager.global.spaceResourceToTons * multiplier
		};
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x00050CF4 File Offset: 0x0004EEF4
	public float MoneyCost(float irradiatedValue, TISpaceBodyState spaceBody, TINaturalSpaceObjectState naturalSpaceObject, TIFactionState faction, float rateMultiplier, List<ResourceValue> preSuppliedResources = null)
	{
		float num = 0f;
		ResourceCostBuilder resourceCostBuilder = this.BuildMaterials(irradiatedValue, spaceBody, naturalSpaceObject, faction, rateMultiplier);
		if (preSuppliedResources != null)
		{
			foreach (ResourceValue resourceValue in preSuppliedResources)
			{
				switch (resourceValue.resource)
				{
				case FactionResource.Water:
					resourceCostBuilder.water -= resourceValue.value;
					break;
				case FactionResource.Volatiles:
					resourceCostBuilder.volatiles -= resourceValue.value;
					break;
				case FactionResource.Metals:
					resourceCostBuilder.metals -= resourceValue.value;
					break;
				case FactionResource.NobleMetals:
					resourceCostBuilder.nobleMetals -= resourceValue.value;
					break;
				case FactionResource.Fissiles:
					resourceCostBuilder.fissiles -= resourceValue.value;
					break;
				case FactionResource.Antimatter:
					resourceCostBuilder.antimatter -= resourceValue.value;
					break;
				case FactionResource.Exotics:
					resourceCostBuilder.exotics -= resourceValue.value;
					break;
				}
			}
		}
		num += resourceCostBuilder.water * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Water);
		num += resourceCostBuilder.volatiles * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Volatiles);
		num += resourceCostBuilder.metals * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Metals);
		num += resourceCostBuilder.nobleMetals * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.NobleMetals);
		num += resourceCostBuilder.fissiles * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Fissiles);
		num += resourceCostBuilder.antimatter * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Antimatter);
		num += resourceCostBuilder.exotics * TIGlobalValuesState.GlobalValues.GetPurchaseResourceMarketValue(FactionResource.Exotics);
		return num;
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x00050EAC File Offset: 0x0004F0AC
	public float BoostCostFromEarth(float irradiatedValue, TISpaceBodyState spaceBody, TIFactionState faction, TIGameState destination, float rateMultiplier, List<ResourceValue> preSuppliedResources = null)
	{
		float num = 0f;
		ResourceCostBuilder resourceCostBuilder = this.BuildMaterials(irradiatedValue, spaceBody, destination.ref_naturalSpaceObject, faction, rateMultiplier);
		if (preSuppliedResources != null)
		{
			foreach (ResourceValue resourceValue in preSuppliedResources)
			{
				switch (resourceValue.resource)
				{
				case FactionResource.Water:
					resourceCostBuilder.water -= resourceValue.value;
					break;
				case FactionResource.Volatiles:
					resourceCostBuilder.volatiles -= resourceValue.value;
					break;
				case FactionResource.Metals:
					resourceCostBuilder.metals -= resourceValue.value;
					break;
				case FactionResource.NobleMetals:
					resourceCostBuilder.nobleMetals -= resourceValue.value;
					break;
				case FactionResource.Fissiles:
					resourceCostBuilder.fissiles -= resourceValue.value;
					break;
				case FactionResource.Antimatter:
					resourceCostBuilder.antimatter -= resourceValue.value;
					break;
				case FactionResource.Exotics:
					resourceCostBuilder.exotics -= resourceValue.value;
					break;
				}
			}
		}
		num += resourceCostBuilder.water;
		num += resourceCostBuilder.volatiles;
		num += resourceCostBuilder.metals;
		num += resourceCostBuilder.nobleMetals;
		num += resourceCostBuilder.fissiles;
		num += resourceCostBuilder.antimatter;
		num += resourceCostBuilder.exotics;
		return (float)TISpaceObjectState.GenericTransferBoostFromEarthSurface(faction, destination, num / TemplateManager.global.spaceResourceToTons);
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x00051024 File Offset: 0x0004F224
	public TIResourcesCost CostFromEarth(TIFactionState faction, TIGameState destinationState, bool isUpgrade)
	{
		float num = TIUtilities.IrradiatedMultiplier(destinationState);
		float num2 = (isUpgrade ? 0.6666667f : 1f);
		float num3 = (isUpgrade ? 0.6666667f : 1f);
		TIResourcesCost tiresourcesCost = new TIResourcesCost();
		TISpaceBodyState tispaceBodyState = destinationState.ref_spaceBody;
		float num4 = 1f;
		TIHabState tihabState = destinationState as TIHabState;
		if (tihabState != null)
		{
			num4 = tihabState.GetModuleConstructionTimeModifier(false, null);
			if (tihabState.IsBase)
			{
				tispaceBodyState = tihabState.habSite.parentBody;
			}
		}
		else
		{
			TIHabSiteState tihabSiteState = destinationState as TIHabSiteState;
			if (tihabSiteState != null)
			{
				tispaceBodyState = tihabSiteState.parentBody;
			}
		}
		if (tispaceBodyState == null)
		{
			tispaceBodyState = destinationState.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.ref_spaceBody;
		}
		tiresourcesCost.AddCost(FactionResource.Boost, this.BoostCostFromEarth(num, tispaceBodyState, faction, destinationState, num3, null), true);
		tiresourcesCost.AddCost(FactionResource.Money, this.MoneyCost(num, tispaceBodyState, destinationState.ref_naturalSpaceObject, faction, num3, null), true);
		TIResourcesCost tiresourcesCost2 = this.BuildMaterials(num, tispaceBodyState, destinationState.ref_naturalSpaceObject, faction, num3).ToResourcesCost(1f);
		foreach (FactionResource factionResource in TIResourcesCost.irreplaceableSpaceResources)
		{
			tiresourcesCost.AddCost(factionResource, tiresourcesCost2.GetSingleCostValue(factionResource), true);
		}
		float num5 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
		float num6 = this.buildTime_Days * TIGlobalValuesState.GetHabModuleConstructionTimeSettingsModifier(faction) * num2 * num4 * faction.GetHabConstructionDurationModifier() + num5 + TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num5, null);
		if (tihabState != null && tihabState.coreModule.underConstruction && tihabState.tier <= this.tier)
		{
			num6 = Mathf.Max(num6, -(float)TITimeState.Now().DifferenceInDays(new TIDateTime(tihabState.coreModule.completionDate)));
		}
		tiresourcesCost.SetCompletionTime_Days(num6);
		return tiresourcesCost;
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x00051208 File Offset: 0x0004F408
	public TIResourcesCost CostFromSpace(TIFactionState faction, TIGameState destinationState, bool isUpgrade, bool substituteBoost, int maxDaysToSave = 0, bool dontRecalculateIncome = false)
	{
		float num = (faction.IsAlienFaction ? 1f : TIUtilities.IrradiatedMultiplier(destinationState));
		float num2 = (isUpgrade ? 0.6666667f : 1f);
		float num3 = (isUpgrade ? 0.6666667f : 1f);
		TISpaceBodyState tispaceBodyState = destinationState.ref_spaceBody;
		float num4 = 1f;
		TIHabState tihabState = null;
		if (destinationState.isHabSiteState)
		{
			tispaceBodyState = destinationState.ref_habSite.ref_spaceBody;
		}
		else if (destinationState.isHabState)
		{
			tihabState = destinationState.ref_hab;
			num4 = tihabState.GetModuleConstructionTimeModifier(false, null);
			if (tihabState.IsBase)
			{
				tispaceBodyState = tihabState.habSite.ref_spaceBody;
			}
		}
		if (tispaceBodyState == null)
		{
			tispaceBodyState = destinationState.ref_naturalSpaceObject.GetSunOrbitingRelatedObject.ref_spaceBody;
		}
		TIResourcesCost tiresourcesCost = this.BuildMaterials(num, tispaceBodyState, destinationState.ref_naturalSpaceObject, faction, num3).ToResourcesCost(1f);
		float num5 = 0f;
		if (substituteBoost && !tiresourcesCost.CanAfford(faction, 1f, null, float.PositiveInfinity) && (faction.IsActiveHumanFaction || GameStateManager.AlienNation().extant))
		{
			tiresourcesCost = tiresourcesCost.GetBoostSubstitutedCost(faction, destinationState, false, null);
			float num6 = TISpaceObjectState.GenericTransferTimeFromEarthsSurface_d(faction, destinationState);
			num6 += TIEffectsState.SumEffectsModifiers(Context.GenericModuleTransferTime, faction, num6, null);
			if (num6 > num5)
			{
				num5 = num6;
			}
		}
		float num7 = this.buildTime_Days * TIGlobalValuesState.GetHabModuleConstructionTimeSettingsModifier(faction) * num2 * num4 * faction.GetHabConstructionDurationModifier() + num5;
		if (tihabState != null && tihabState.coreModule.underConstruction && tihabState.tier <= this.tier)
		{
			num7 = Mathf.Max(num7, -(float)TITimeState.Now().DifferenceInDays(new TIDateTime(tihabState.coreModule.completionDate)));
		}
		tiresourcesCost.SetCompletionTime_Days(num7);
		return tiresourcesCost;
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x000513BC File Offset: 0x0004F5BC
	public TIResourcesCost MinimumBoostCost(TIFactionState faction, TIGameState location, bool isUpgrade = false, int maxDaysToSave = 180)
	{
		if (!this.coreModule || isUpgrade || faction.CanFoundHabFromHabAtLocation(location, false, false))
		{
			TIResourcesCost tiresourcesCost = this.CostFromSpace(faction, location, isUpgrade, true, maxDaysToSave, true);
			if (maxDaysToSave == 0)
			{
				if (tiresourcesCost.CanAfford_AI(faction, this, location, 1, false, false, 1f, null, float.PositiveInfinity))
				{
					return tiresourcesCost;
				}
			}
			else if (tiresourcesCost.CanPayInFuture(faction, maxDaysToSave))
			{
				return tiresourcesCost;
			}
		}
		return this.CostFromEarth(faction, location, isUpgrade);
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x00051424 File Offset: 0x0004F624
	public TIResourcesCost MinimumBoostCostToday(TIFactionState faction, TIGameState location, bool isUpgrade = false)
	{
		return this.MinimumBoostCost(faction, location, isUpgrade, 0);
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x00051430 File Offset: 0x0004F630
	public TIProjectTemplate RequiredProject
	{
		get
		{
			if (!string.IsNullOrEmpty(this.requiredProjectName))
			{
				TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(this.requiredProjectName, false);
				if (tiprojectTemplate == null)
				{
					Log.Error("Bad project templateName " + this.requiredProjectName + " in " + base.dataName, Array.Empty<object>());
				}
				return tiprojectTemplate;
			}
			return null;
		}
	}

	// Token: 0x170001CA RID: 458
	// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x00051480 File Offset: 0x0004F680
	public TIHabModuleTemplate UpgradesFrom
	{
		get
		{
			if (this._upgradesFrom == null && !string.IsNullOrEmpty(this.upgradesFromName))
			{
				TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(this.upgradesFromName, false);
				if (tihabModuleTemplate == null)
				{
					Log.Error("Bad upgradesFromName " + this.upgradesFromName + " in " + base.dataName, Array.Empty<object>());
				}
				this._upgradesFrom = tihabModuleTemplate;
			}
			return this._upgradesFrom;
		}
	}

	// Token: 0x170001CB RID: 459
	// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x000514E4 File Offset: 0x0004F6E4
	public TIHabModuleTemplate UpgradesTo
	{
		get
		{
			if (!this._upgradeToChecked)
			{
				this._upgradeToChecked = true;
				foreach (TIHabModuleTemplate tihabModuleTemplate in TemplateManager.IterateByClass<TIHabModuleTemplate>(true))
				{
					if (tihabModuleTemplate.UpgradesFrom == this)
					{
						this._upgradesTo = tihabModuleTemplate;
						break;
					}
				}
			}
			return this._upgradesTo;
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x00051554 File Offset: 0x0004F754
	public bool OnFutureUpgradePath(TIHabModuleTemplate moduleToCheck)
	{
		switch (this.tier)
		{
		case 1:
			return false;
		case 2:
			return this.UpgradesFrom == moduleToCheck;
		case 3:
		{
			if (this.UpgradesFrom == moduleToCheck)
			{
				return true;
			}
			TIHabModuleTemplate upgradesFrom = this.UpgradesFrom;
			return ((upgradesFrom != null) ? upgradesFrom.UpgradesFrom : null) == moduleToCheck;
		}
		default:
			return false;
		}
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x000515B0 File Offset: 0x0004F7B0
	public bool OnFutureOrPastUpgradePath(TIHabModuleTemplate moduleToCheck)
	{
		if (moduleToCheck == null)
		{
			return false;
		}
		if (this.OnFutureUpgradePath(moduleToCheck))
		{
			return true;
		}
		switch (this.tier)
		{
		case 1:
			if (this.UpgradesTo != moduleToCheck)
			{
				TIHabModuleTemplate upgradesTo = this.UpgradesTo;
				return ((upgradesTo != null) ? upgradesTo.UpgradesTo : null) == moduleToCheck;
			}
			return true;
		case 2:
			return this.UpgradesTo == moduleToCheck;
		case 3:
			return false;
		default:
			return false;
		}
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x00051618 File Offset: 0x0004F818
	public bool SharesUpgradePath(TIHabModuleTemplate moduleToCheck)
	{
		return moduleToCheck == this || this.OnFutureOrPastUpgradePath(moduleToCheck);
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00051627 File Offset: 0x0004F827
	public bool IsForHabType(HabType testHabType)
	{
		return this.habType == testHabType || this.habType == HabType.Any;
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x0005163D File Offset: 0x0004F83D
	public bool ModuleTierIsAllowed(TIHabState hab)
	{
		return this.tier <= hab.tier || this.coreModule;
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x00051658 File Offset: 0x0004F858
	public bool AllowedForHabAutomatedStatus(TIHabState hab)
	{
		if (this.automated)
		{
			return hab.sectors[0].habModules[0].moduleTemplate.automated || hab.sectors[0].habModules[0].moduleTemplate == null;
		}
		return !hab.sectors[0].habModules[0].moduleTemplate.automated || hab.sectors[0].habModules[0].moduleTemplate == null;
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x000516F8 File Offset: 0x0004F8F8
	public bool AllowedLocation(TIGameState habLocation, TIHabState hab)
	{
		if (habLocation.ref_orbit != null && this.habType == HabType.Base)
		{
			return false;
		}
		if (habLocation.ref_habSite != null && this.habType == HabType.Station)
		{
			return false;
		}
		bool flag = true;
		if (this.SpecialRules.Contains(HabModuleSpecialRule.EarthLEOOnly) && !habLocation.ref_orbit.isEarthLEO)
		{
			return false;
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.Requires_Colonized_Body) && !habLocation.ref_naturalSpaceObject.Colonized())
		{
			return false;
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.Requires_Inhabited_Body) && !habLocation.ref_naturalSpaceObject.Populous())
		{
			return false;
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.NotInIrradiated))
		{
			if (habLocation.ref_orbit != null)
			{
				flag = !habLocation.ref_orbit.IsIrradiated();
			}
			else if (habLocation.ref_habSite != null)
			{
				flag = !habLocation.ref_habSite.IsIrradiated();
			}
			else if (habLocation.ref_spaceBody != null)
			{
				flag = !habLocation.ref_spaceBody.IsIrradiated();
			}
			else if (habLocation.ref_lagrangePoint != null)
			{
				flag = !habLocation.ref_lagrangePoint.IsIrradiated();
			}
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.Requires_Interface_Orbit))
		{
			if (habLocation.ref_orbit == null)
			{
				return false;
			}
			if (!habLocation.ref_orbit.interfaceOrbit)
			{
				return false;
			}
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.Requires_GasGiant_Orbit))
		{
			if (habLocation.ref_orbit == null)
			{
				return false;
			}
			if (habLocation.ref_spaceBody == null)
			{
				return false;
			}
			if (habLocation.ref_spaceBody.atmosphere != Atmosphere.Massive)
			{
				return false;
			}
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter))
		{
			if (habLocation.ref_orbit.amat_ugpy <= 0f)
			{
				return false;
			}
			List<TIHabState> stationsInOrbit = habLocation.ref_orbit.stationsInOrbit;
			stationsInOrbit.Remove(hab);
			if (stationsInOrbit.Any<TIHabState>((TIHabState x) => x.AllModules().Any<TIHabModuleState>((TIHabModuleState u) => u != null && u.moduleTemplate.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter))))
			{
				return false;
			}
		}
		if (this.SpecialRules.Contains(HabModuleSpecialRule.SolarMirror))
		{
			if (habLocation.ref_naturalSpaceObject.isLagrangePointState)
			{
				TILagrangePointState ref_lagrangePoint = habLocation.ref_lagrangePoint;
				if (ref_lagrangePoint.lagrangeValue == LagrangeValue.L2 || ref_lagrangePoint.lagrangeValue == LagrangeValue.L3)
				{
					return false;
				}
				if (ref_lagrangePoint.barycenter.isSun && (ref_lagrangePoint.lagrangeValue == LagrangeValue.L4 || ref_lagrangePoint.lagrangeValue == LagrangeValue.L5))
				{
					return false;
				}
			}
			else if (habLocation.ref_orbit.barycenter.isSpaceBodyState && habLocation.ref_orbit.barycenter.ref_spaceBody.habSites.Length == 0)
			{
				return false;
			}
		}
		return flag;
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x00051967 File Offset: 0x0004FB67
	public bool EverAllowedForFaction(TIFactionState faction)
	{
		return this.alienModule == faction.IsAlienFaction;
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x00051978 File Offset: 0x0004FB78
	public bool FactionCanBuild(TIFactionState faction)
	{
		if (this.noBuild || !this.EverAllowedForFaction(faction))
		{
			return false;
		}
		if (this.RequiredProject == null)
		{
			return true;
		}
		if (this.hasBeenResearchedCachedFrame != TIFrameCounter.FrameCount)
		{
			this.cachedHasBeenResearched.Clear();
			this.hasBeenResearchedCachedFrame = TIFrameCounter.FrameCount;
		}
		bool flag;
		if (!this.cachedHasBeenResearched.TryGetValue(faction, out flag))
		{
			flag = faction.completedProjects.Contains(this.RequiredProject);
			this.cachedHasBeenResearched[faction] = flag;
		}
		return flag;
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x000519F5 File Offset: 0x0004FBF5
	public float GetMonthlyRecyclableConsumption(FactionResource resource, TIFactionState faction = null, TIHabState hab = null)
	{
		if (resource == FactionResource.Water || resource == FactionResource.Volatiles)
		{
			return this.MonthlyCrewSupportCost(resource, faction, hab);
		}
		return 0f;
	}

	// Token: 0x06000FB3 RID: 4019 RVA: 0x00051A10 File Offset: 0x0004FC10
	public float MonthlySupportCost(FactionResource resource, bool includeCrewSupportCost = true, TIFactionState faction = null, TIHabState hab = null)
	{
		switch (resource)
		{
		case FactionResource.Money:
			return this.supportMaterials_month.money + (includeCrewSupportCost ? this.MonthlyCrewSupportCost(resource, faction, hab) : 0f);
		case FactionResource.Boost:
			return this.supportMaterials_month.boost;
		case FactionResource.Water:
			return this.supportMaterials_month.water + (includeCrewSupportCost ? this.MonthlyCrewSupportCost(resource, faction, hab) : 0f) + ((this.specialRules.Contains(HabModuleSpecialRule.UsesHelium3) && faction != null && faction.He3Access) ? this.supportMaterials_month.fissiles : 0f);
		case FactionResource.Volatiles:
			return this.supportMaterials_month.volatiles + (includeCrewSupportCost ? this.MonthlyCrewSupportCost(resource, faction, hab) : 0f);
		case FactionResource.Metals:
			return this.supportMaterials_month.metals;
		case FactionResource.NobleMetals:
			return this.supportMaterials_month.nobleMetals;
		case FactionResource.Fissiles:
			if (this.specialRules.Contains(HabModuleSpecialRule.UsesHelium3) && faction != null && faction.He3Access)
			{
				return 0f;
			}
			return this.supportMaterials_month.fissiles;
		case FactionResource.Antimatter:
			return this.supportMaterials_month.antimatter;
		case FactionResource.Exotics:
			return this.supportMaterials_month.exotics;
		}
		return 0f;
	}

	// Token: 0x06000FB4 RID: 4020 RVA: 0x00051B60 File Offset: 0x0004FD60
	public float MonthlyCrewSupportCost(FactionResource resource, TIFactionState faction = null, TIHabState hab = null)
	{
		if (resource != FactionResource.Money)
		{
			if (resource == FactionResource.Water)
			{
				return (float)this.crew * TemplateManager.global.crewWaterConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f;
			}
			if (resource != FactionResource.Volatiles)
			{
				return 0f;
			}
			return (float)this.crew * TemplateManager.global.crewVolatilesConsumptionTons_year * TemplateManager.global.spaceResourceToTons / 12f;
		}
		else
		{
			if ((faction != null && !faction.IsActiveHumanFaction) || this.specialRules.Contains(HabModuleSpecialRule.Stability))
			{
				return 0f;
			}
			return (float)this.crew * TemplateManager.global.crewSalary_year / 12f;
		}
	}

	// Token: 0x06000FB5 RID: 4021 RVA: 0x00051C08 File Offset: 0x0004FE08
	public float YearlySupportCost(FactionResource resource, bool includeCrewSupportCost = true, TIFactionState faction = null, TIHabState hab = null)
	{
		return this.MonthlySupportCost(resource, includeCrewSupportCost, faction, hab) * 12f;
	}

	// Token: 0x06000FB6 RID: 4022 RVA: 0x00051C1B File Offset: 0x0004FE1B
	public float DailySupportCost(FactionResource resource, bool includeCrewSupportCost = true, TIFactionState faction = null, TIHabState hab = null)
	{
		return this.YearlySupportCost(resource, includeCrewSupportCost, faction, hab) / 365.2422f;
	}

	// Token: 0x06000FB7 RID: 4023 RVA: 0x00051C2E File Offset: 0x0004FE2E
	public float YearlyResourceIncome(FactionResource resource, TIHabState hab = null, TIFactionState faction = null)
	{
		if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
		{
			return this.MonthlyResourceIncome(resource, hab, faction);
		}
		return this.MonthlyResourceIncome(resource, hab, faction) * 12f;
	}

	// Token: 0x06000FB8 RID: 4024 RVA: 0x00051C51 File Offset: 0x0004FE51
	public float DailyResourceIncome(FactionResource resource, TIHabState hab = null, TIFactionState faction = null)
	{
		if (resource == FactionResource.Projects || resource == FactionResource.MissionControl)
		{
			return this.MonthlyResourceIncome(resource, hab, faction);
		}
		return this.YearlyResourceIncome(resource, hab, faction) / 365.2422f;
	}

	// Token: 0x06000FB9 RID: 4025 RVA: 0x00051C74 File Offset: 0x0004FE74
	public float MonthlyResourceIncome(FactionResource resource, TIGameState location = null, TIFactionState faction = null)
	{
		TIHabState tihabState = ((location != null) ? location.ref_hab : null);
		switch (resource)
		{
		case FactionResource.Money:
			if (this.SpecialRules.Contains(HabModuleSpecialRule.MoneyIfNotBuilding) && tihabState != null)
			{
				if (tihabState.AllModules().Any<TIHabModuleState>((TIHabModuleState x) => x.underConstruction))
				{
					return 0f;
				}
			}
			if (faction != null && tihabState != null && this.MonthlySupportCost(FactionResource.Boost, true, faction, tihabState) > 0f && faction.GetCurrentResourceAmount(FactionResource.Boost) <= 0f && faction.GetDailyIncome(FactionResource.Boost, false, false) <= 0f)
			{
				return 0f;
			}
			return this.incomeMoney_month;
		case FactionResource.Influence:
			return this.incomeInfluence_month;
		case FactionResource.Operations:
			return this.incomeOps_month;
		case FactionResource.Research:
			return this.incomeResearch_month;
		case FactionResource.Projects:
			return (float)this.incomeProjects;
		case FactionResource.Boost:
			return this.incomeBoost_month;
		case FactionResource.MissionControl:
			return (float)this.missionControl;
		case FactionResource.Water:
			if (faction == null || location == null)
			{
				return 0f;
			}
			return this.incomeWater_month + this.GetMiningIncome_Month(faction, location.ref_habSite, resource);
		case FactionResource.Volatiles:
			if (faction == null || location == null)
			{
				return 0f;
			}
			return this.incomeVolatiles_month + this.GetMiningIncome_Month(faction, location.ref_habSite, resource);
		case FactionResource.Metals:
			if (faction == null || location == null)
			{
				return 0f;
			}
			return this.incomeMetals_month + this.GetMiningIncome_Month(faction, location.ref_habSite, resource);
		case FactionResource.NobleMetals:
			if (faction == null || location == null)
			{
				return 0f;
			}
			return this.incomeNobles_month + this.GetMiningIncome_Month(faction, location.ref_habSite, resource);
		case FactionResource.Fissiles:
			if (faction == null || location == null)
			{
				return 0f;
			}
			return this.incomeFissiles_month + this.GetMiningIncome_Month(faction, location.ref_habSite, resource);
		case FactionResource.Antimatter:
		{
			float num = this.incomeAntimatter_month;
			if (location != null && this.SpecialRules.Contains(HabModuleSpecialRule.HarvestAntimatter))
			{
				num += this.specialRulesValue * location.ref_orbit.antimatterPerMonth_dekatonnes;
			}
			return num;
		}
		case FactionResource.Exotics:
			if (faction != null && faction.IsAlienFaction)
			{
				return this.incomeExotics_month * TemplateManager.global.AI_GetExoticsMultiplier();
			}
			return this.incomeExotics_month;
		default:
			return 0f;
		}
	}

	// Token: 0x06000FBA RID: 4026 RVA: 0x00051EDC File Offset: 0x000500DC
	public float MonthlyResourceRevenue(FactionResource resource, TIGameState location = null, TIFactionState faction = null)
	{
		return Mathf.Max(0f, this.MonthlyResourceIncome(resource, location, faction));
	}

	// Token: 0x06000FBB RID: 4027 RVA: 0x00051EF1 File Offset: 0x000500F1
	public TIHabModuleTemplate UpgradeModuleTemplate(TIFactionState faction, bool checkUnlocked)
	{
		if (checkUnlocked && this.UpgradesTo != null && this.UpgradesTo.FactionCanBuild(faction))
		{
			return this.UpgradesTo;
		}
		return null;
	}

	// Token: 0x06000FBC RID: 4028 RVA: 0x00051F14 File Offset: 0x00050114
	public bool CanUpgrade(TIFactionState faction)
	{
		return this.UpgradeModuleTemplate(faction, true) != null;
	}

	// Token: 0x06000FBD RID: 4029 RVA: 0x00051F24 File Offset: 0x00050124
	public float GetTechBonusByCategory(TechCategory category)
	{
		float num = 0f;
		foreach (TechBonus techBonus in this.techBonuses)
		{
			if (techBonus.category == category)
			{
				num += techBonus.bonus;
			}
		}
		return num;
	}

	// Token: 0x06000FBE RID: 4030 RVA: 0x00051F67 File Offset: 0x00050167
	public TIProjectTemplate GetProjectUnlocked()
	{
		if (!string.IsNullOrEmpty(this.unlocksProjectName))
		{
			return TemplateManager.Find<TIProjectTemplate>(this.unlocksProjectName, false);
		}
		return null;
	}

	// Token: 0x06000FBF RID: 4031 RVA: 0x00051F84 File Offset: 0x00050184
	public float GetMiningIncome_Day(TIFactionState faction, TIHabSiteState habSite, FactionResource resource)
	{
		if (!this.mine)
		{
			return 0f;
		}
		float currentMiningMultiplierFromOrgsAndEffects = faction.GetCurrentMiningMultiplierFromOrgsAndEffects(resource);
		float mineSizeModifier = faction.GetMineSizeModifier();
		return habSite.GetDailyProduction(resource) * this.miningModifier * currentMiningMultiplierFromOrgsAndEffects * TIGlobalValuesState.GetMiningRateSettingsModifier(faction) * mineSizeModifier;
	}

	// Token: 0x06000FC0 RID: 4032 RVA: 0x00051FC7 File Offset: 0x000501C7
	public float GetMiningIncome_Year(TIFactionState faction, TIHabSiteState habSite, FactionResource resource)
	{
		if (!this.mine)
		{
			return 0f;
		}
		return this.GetMiningIncome_Day(faction, habSite, resource) * 365.2422f;
	}

	// Token: 0x06000FC1 RID: 4033 RVA: 0x00051FE6 File Offset: 0x000501E6
	public float GetMiningIncome_Month(TIFactionState faction, TIHabSiteState habSite, FactionResource resource)
	{
		if (!this.mine)
		{
			return 0f;
		}
		return this.GetMiningIncome_Year(faction, habSite, resource) / 12f;
	}

	// Token: 0x06000FC2 RID: 4034 RVA: 0x00052008 File Offset: 0x00050208
	public float ShipyardConstructionSpeedModifier(TIShipHullTemplate hullTemplate)
	{
		if (this.allowsShipConstruction)
		{
			int num = this.tier - hullTemplate.consTier;
			if (num > 0)
			{
				return Mathf.Pow(this.constructionTimeModifier, (float)num);
			}
			if (num < 0)
			{
				return Mathf.Pow(TemplateManager.global.smallShipyardPenaltyPowerPerTier, (float)(-(float)num));
			}
		}
		return 1f;
	}

	// Token: 0x170001CC RID: 460
	// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x00052059 File Offset: 0x00050259
	public float moduleConstructionSpeedModifier
	{
		get
		{
			if (this.allowsShipConstruction)
			{
				return 1f;
			}
			return this.constructionTimeModifier;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06000FC4 RID: 4036 RVA: 0x0005206F File Offset: 0x0005026F
	public bool powerSource
	{
		get
		{
			return this.power > 0;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x0005207A File Offset: 0x0005027A
	public int powerConsumed
	{
		get
		{
			if (this.power <= 0)
			{
				return -this.power;
			}
			return 0;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x0005208E File Offset: 0x0005028E
	public bool CanTurnOff
	{
		get
		{
			return !this.coreModule;
		}
	}

	// Token: 0x06000FC7 RID: 4039 RVA: 0x00052099 File Offset: 0x00050299
	public bool CombatTroops()
	{
		return this.SpecialRules.Intersect<HabModuleSpecialRule>(TIHabModuleTemplate.combatTroopsRules).Any<HabModuleSpecialRule>();
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x000520B0 File Offset: 0x000502B0
	public float spaceAssaultValue
	{
		get
		{
			if (!this.CombatTroops())
			{
				return 0f;
			}
			return this.specialRulesValue;
		}
	}

	// Token: 0x06000FC9 RID: 4041 RVA: 0x000520C8 File Offset: 0x000502C8
	public int ProspectivePower(TISpaceBodyState spaceBody, TIFactionState faction)
	{
		for (int i = 0; i < this.SpecialRules.Count; i++)
		{
			if (this.SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output)
			{
				return TIHabModuleState.SolarPowerOutput(spaceBody, (float)this.power, faction, this.tier, false);
			}
			if (this.SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity)
			{
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(spaceBody, this, faction);
			}
		}
		return this.power;
	}

	// Token: 0x06000FCA RID: 4042 RVA: 0x00052130 File Offset: 0x00050330
	public int ProspectivePower(TIHabSiteState site, TIFactionState faction)
	{
		for (int i = 0; i < this.SpecialRules.Count; i++)
		{
			if (this.SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output)
			{
				return TIHabModuleState.SolarPowerOutput(site, (float)this.power, faction, this.tier, false);
			}
			if (this.SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity)
			{
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(site, this, faction);
			}
		}
		return this.power;
	}

	// Token: 0x06000FCB RID: 4043 RVA: 0x00052198 File Offset: 0x00050398
	public int ProspectivePower(TIOrbitState orbit)
	{
		for (int i = 0; i < this.SpecialRules.Count; i++)
		{
			if (this.SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output)
			{
				return TIHabModuleState.SolarPowerOutput(orbit, (float)this.power, null, this.tier, false);
			}
		}
		return this.power;
	}

	// Token: 0x06000FCC RID: 4044 RVA: 0x000521E8 File Offset: 0x000503E8
	public int ProspectivePower(TIHabState hab)
	{
		for (int i = 0; i < this.SpecialRules.Count; i++)
		{
			if (this.SpecialRules[i] == HabModuleSpecialRule.Solar_Power_Variable_Output)
			{
				return TIHabModuleState.SolarPowerOutput(hab, (float)this.power, hab.faction, this.tier, false);
			}
			if (this.SpecialRules[i] == HabModuleSpecialRule.Cost_Scales_With_Gravity)
			{
				return TIHabModuleState.EscapeVelocityBasedPowerRequirement(hab, this, hab.faction);
			}
		}
		return this.power;
	}

	// Token: 0x06000FCD RID: 4045 RVA: 0x00052258 File Offset: 0x00050458
	public int ProspectivePower(TIGameState location, TIFactionState faction)
	{
		if (location.ref_habSite != null)
		{
			return this.ProspectivePower(location.ref_habSite, faction);
		}
		if (location.ref_orbit != null)
		{
			return this.ProspectivePower(location.ref_orbit);
		}
		return this.ProspectivePower(location.ref_system, faction);
	}

	// Token: 0x06000FCE RID: 4046 RVA: 0x000522A9 File Offset: 0x000504A9
	public int ControlPointCapacity(bool habInEarthLEO)
	{
		if (!habInEarthLEO && this.specialRules.Contains(HabModuleSpecialRule.LEOControlPointCapacity))
		{
			return 0;
		}
		return this.controlPointCapacity;
	}

	// Token: 0x06000FCF RID: 4047 RVA: 0x000522C8 File Offset: 0x000504C8
	public bool HasLEOBonus()
	{
		foreach (HabModuleSpecialRule habModuleSpecialRule in this.specialRules)
		{
			if (habModuleSpecialRule == HabModuleSpecialRule.EarthLEOOnly || habModuleSpecialRule - HabModuleSpecialRule.LEOBonusArmyCombatValue <= 14)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000FD0 RID: 4048 RVA: 0x00052328 File Offset: 0x00050528
	public static void ClearStaticData()
	{
		TIHabModuleTemplate.cachedStationCombatModuleStrengths.Clear();
	}

	// Token: 0x04000F98 RID: 3992
	public bool coreModule;

	// Token: 0x04000F99 RID: 3993
	public HabType habType;

	// Token: 0x04000F9A RID: 3994
	public bool onePerHab;

	// Token: 0x04000F9B RID: 3995
	public bool automated;

	// Token: 0x04000F9C RID: 3996
	public bool alienModule;

	// Token: 0x04000F9D RID: 3997
	public bool noBuild;

	// Token: 0x04000F9E RID: 3998
	public string upgradesFromName;

	// Token: 0x04000F9F RID: 3999
	public int tier;

	// Token: 0x04000FA0 RID: 4000
	public string requiredProjectName;

	// Token: 0x04000FA1 RID: 4001
	public int crew;

	// Token: 0x04000FA2 RID: 4002
	public int power;

	// Token: 0x04000FA3 RID: 4003
	public float baseMass_tons;

	// Token: 0x04000FA4 RID: 4004
	public float constructionTimeModifier = 1f;

	// Token: 0x04000FA5 RID: 4005
	public float miningModifier;

	// Token: 0x04000FA6 RID: 4006
	public bool allowsShipConstruction;

	// Token: 0x04000FA7 RID: 4007
	public bool allowsResupply;

	// Token: 0x04000FA8 RID: 4008
	public bool mine;

	// Token: 0x04000FA9 RID: 4009
	public bool destroyed;

	// Token: 0x04000FAA RID: 4010
	public float buildTime_Days;

	// Token: 0x04000FAB RID: 4011
	public bool spaceCombatModule;

	// Token: 0x04000FAC RID: 4012
	public float incomeMoney_month;

	// Token: 0x04000FAD RID: 4013
	public float incomeInfluence_month;

	// Token: 0x04000FAE RID: 4014
	public float incomeOps_month;

	// Token: 0x04000FAF RID: 4015
	public float incomeBoost_month;

	// Token: 0x04000FB0 RID: 4016
	public int missionControl;

	// Token: 0x04000FB1 RID: 4017
	public float incomeResearch_month;

	// Token: 0x04000FB2 RID: 4018
	public int incomeProjects;

	// Token: 0x04000FB3 RID: 4019
	public float incomeWater_month;

	// Token: 0x04000FB4 RID: 4020
	public float incomeVolatiles_month;

	// Token: 0x04000FB5 RID: 4021
	public float incomeMetals_month;

	// Token: 0x04000FB6 RID: 4022
	public float incomeNobles_month;

	// Token: 0x04000FB7 RID: 4023
	public float incomeFissiles_month;

	// Token: 0x04000FB8 RID: 4024
	public float incomeAntimatter_month;

	// Token: 0x04000FB9 RID: 4025
	public float incomeExotics_month;

	// Token: 0x04000FBA RID: 4026
	public int controlPointCapacity;

	// Token: 0x04000FBB RID: 4027
	public TechBonus[] techBonuses;

	// Token: 0x04000FBC RID: 4028
	public string unlocksProjectName;

	// Token: 0x04000FBD RID: 4029
	public List<HabModuleSpecialRule> specialRules = new List<HabModuleSpecialRule>();

	// Token: 0x04000FBE RID: 4030
	public float specialRulesValue;

	// Token: 0x04000FBF RID: 4031
	public ResourceCostBuilder weightedBuildMaterials;

	// Token: 0x04000FC0 RID: 4032
	public ResourceCostBuilder supportMaterials_month;

	// Token: 0x04000FC1 RID: 4033
	public string baseIconResource;

	// Token: 0x04000FC2 RID: 4034
	public string stationIconResource;

	// Token: 0x04000FC3 RID: 4035
	public string stationModelResource;

	// Token: 0x04000FC4 RID: 4036
	public string stationDestructionResource;

	// Token: 0x04000FC5 RID: 4037
	public bool objectiveModule;

	// Token: 0x04000FC6 RID: 4038
	public bool alertWorthy;

	// Token: 0x04000FC7 RID: 4039
	public const float upgradeCostDiscount = 0.6666667f;

	// Token: 0x04000FC8 RID: 4040
	public const float upgradeSpeedDiscount = 0.6666667f;

	// Token: 0x04000FC9 RID: 4041
	public static readonly List<HabModuleSpecialRule> combatTroopsRules = new List<HabModuleSpecialRule>
	{
		HabModuleSpecialRule.DropTroops,
		HabModuleSpecialRule.Griffins,
		HabModuleSpecialRule.MarineCompany,
		HabModuleSpecialRule.MarinePlatoon,
		HabModuleSpecialRule.MarineBattalion,
		HabModuleSpecialRule.Salamanders,
		HabModuleSpecialRule.WarDogs
	};

	// Token: 0x04000FCA RID: 4042
	private List<HabModuleSpecialRule> _specialRules;

	// Token: 0x04000FCB RID: 4043
	public static Dictionary<TIFactionState, Dictionary<int, float>> cachedStationCombatModuleStrengths = new Dictionary<TIFactionState, Dictionary<int, float>>();

	// Token: 0x04000FCC RID: 4044
	private Dictionary<TIFactionState, bool> cachedHasBeenResearched = new Dictionary<TIFactionState, bool>();

	// Token: 0x04000FCD RID: 4045
	private int hasBeenResearchedCachedFrame = -1;

	// Token: 0x04000FCE RID: 4046
	private TIHabModuleTemplate _upgradesFrom;

	// Token: 0x04000FCF RID: 4047
	private TIHabModuleTemplate _upgradesTo;

	// Token: 0x04000FD0 RID: 4048
	private bool _upgradeToChecked;

	// Token: 0x02000BB1 RID: 2993
	private struct IncomeEntry
	{
		// Token: 0x060069C3 RID: 27075 RVA: 0x00302E3B File Offset: 0x0030103B
		public IncomeEntry(string ip, string v)
		{
			this.inlinePath = ip;
			this.value = v;
		}

		// Token: 0x04004BC3 RID: 19395
		public string inlinePath;

		// Token: 0x04004BC4 RID: 19396
		public string value;
	}
}
