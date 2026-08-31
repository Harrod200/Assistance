using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000838 RID: 2104
	public class LedgerListItem_Data
	{
		// Token: 0x06004C37 RID: 19511 RVA: 0x00200E89 File Offset: 0x001FF089
		public void SetCommonData(LedgerListItem_Data data, bool collapsible = false, TIGameState associatedState = null, TIDataTemplate associatedTemplate = null, TIGameState parentGameState = null, int which = 0)
		{
			data.collapsible = collapsible;
			data.associatedState = associatedState;
			data.associatedTemplate = associatedTemplate;
			data.parentGameState = parentGameState;
			data.which = which;
		}

		// Token: 0x06004C38 RID: 19512 RVA: 0x00200EB4 File Offset: 0x001FF0B4
		private void SetLedgerEntryData(LedgerEntryCategory category, float value, bool inactive = false, bool cost = false, bool percent = false)
		{
			this.ledgerValues[category] = value;
			if (value == 0f)
			{
				this.SetEmptyLedgerValue(category);
				return;
			}
			string text;
			if (percent)
			{
				text = TIUtilities.ForceValueSign(value, false, true, "P0");
				if (inactive)
				{
					text = TIUtilities.HighlightLine(text);
				}
			}
			else
			{
				text = TIUtilities.FormatBigOrSmallNumber(value, 1, 7, 0, true, false);
				if (inactive)
				{
					text = TIUtilities.HighlightLine(text);
				}
				else if (cost)
				{
					text = TIUtilities.RedLine(text);
				}
				else
				{
					text = TIUtilities.GreenLine(text);
				}
			}
			this.ledgerValueText[(int)category] = text;
		}

		// Token: 0x06004C39 RID: 19513 RVA: 0x00200F31 File Offset: 0x001FF131
		private void SetEmptyLedgerValue(LedgerEntryCategory category)
		{
			this.ledgerValues[category] = 0f;
			this.ledgerValueText[(int)category] = string.Empty;
		}

		// Token: 0x06004C3A RID: 19514 RVA: 0x00200F54 File Offset: 0x001FF154
		public void SetItemData(TIFactionState faction, int which)
		{
			this.entryIconSprite = faction.factionIcon128UI;
			if (which == 0)
			{
				this.entryName = Loc.T("UI.Council.Ledger.FactionTotals");
				this.sortOverride = 1;
				this.SetLedgerEntryData(LedgerEntryCategory.money_Income, faction.GetMonthlyGrossRevenue(FactionResource.Money), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Money), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, faction.GetMonthlyGrossRevenue(FactionResource.Influence), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Influence), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, faction.GetMonthlyGrossRevenue(FactionResource.Operations), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.boost_income, faction.GetMonthlyGrossRevenue(FactionResource.Boost), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, faction.GetMonthlyGrossExpenses(FactionResource.Boost), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, faction.GetMonthlyIncome(FactionResource.MissionControl, false, false), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, (float)faction.GetMissionControlUsage(), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.research_Income, faction.GetMonthlyIncome(FactionResource.Research, false, false), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.projects_Income, faction.GetYearlyRevenue(FactionResource.Projects, false, false), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Gain, faction.GetControlPointMaintenanceFreebieCap(), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Cost, faction.GetBaselineControlPointMaintenanceCost(false), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.water_Income, faction.GetMonthlyGrossRevenue(FactionResource.Water), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.water_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Water), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Income, faction.GetMonthlyGrossRevenue(FactionResource.Volatiles), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Volatiles), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.metals_Income, faction.GetMonthlyGrossRevenue(FactionResource.Metals), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.metals_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Metals), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.nobles_Income, faction.GetMonthlyGrossRevenue(FactionResource.NobleMetals), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.nobles_Cost, faction.GetMonthlyGrossExpenses(FactionResource.NobleMetals), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Income, faction.GetMonthlyGrossRevenue(FactionResource.Fissiles), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Fissiles), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Income, faction.GetMonthlyGrossRevenue(FactionResource.Antimatter), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Cost, faction.GetMonthlyGrossExpenses(FactionResource.Antimatter), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.energy_Bonus, faction.SumCategoryModifiers(TechCategory.Energy), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.materials_Bonus, faction.SumCategoryModifiers(TechCategory.Materials), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, faction.SumCategoryModifiers(TechCategory.SpaceScience), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.lifeScience_Bonus, faction.SumCategoryModifiers(TechCategory.LifeScience), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.infoScience_Bonus, faction.SumCategoryModifiers(TechCategory.InformationScience), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.militaryScience_Bonus, faction.SumCategoryModifiers(TechCategory.MilitaryScience), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.socialScience_Bonus, faction.SumCategoryModifiers(TechCategory.SocialScience), false, false, true);
				this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, faction.SumCategoryModifiers(TechCategory.Xenology), false, false, true);
				return;
			}
			if (which == 1)
			{
				this.entryName = Loc.T("UI.Council.Ledger.FactionHQ", new object[] { faction.adjectiveWithColor });
				this.SetLedgerEntryData(LedgerEntryCategory.money_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Money) + faction.GetMonthlyIncomeFromExcessMissionControl(FactionResource.Money), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.money_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Influence) + faction.GetMonthlyIncomeFromNations(FactionResource.Influence, false), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, faction.GetAnnualControlPointMaintenanceCost() / 12f, false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Operations), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.boost_income, faction.GetMonthlyIncomeFromHQ(FactionResource.Boost), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.boost_cost);
				this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.MissionControl), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, (float)faction.GetMissionControlRequirementFromMineNetwork(-1), false, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.research_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Research) + faction.GetMonthlyIncomeFromExcessMissionControl(FactionResource.Money), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.projects_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Projects), false, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Gain, (float)TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies - TIEffectsState.SumEffectsModifiers(Context.ControlPointMaintenance, faction, (float)TIGlobalValuesState.GlobalValues.controlPointMaintenanceFreebies, null), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.water_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Water), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Volatiles), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.metals_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Metals), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.nobles_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.NobleMetals), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Fissiles), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Income, faction.GetMonthlyIncomeFromHQ(FactionResource.Antimatter), false, false, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.energy_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.materials_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.spaceScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.lifeScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.infoScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.militaryScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.socialScience_Bonus);
				this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, faction.InvestigationsModifier(TechCategory.Xenology), false, false, true);
				return;
			}
			if (which == 2)
			{
				this.entryName = Loc.T("UI.Council.Ledger.UnassignedOrgPool");
				this.SetEmptyLedgerValue(LedgerEntryCategory.money_Income);
				this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, -faction.GetNegativeMonthlyIncomeFromUnassignedOrgs(FactionResource.Money), false, true, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.influence_Income);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, -faction.GetNegativeMonthlyIncomeFromUnassignedOrgs(FactionResource.Influence), false, true, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.ops_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.boost_income);
				this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, -faction.GetNegativeMonthlyIncomeFromUnassignedOrgs(FactionResource.Boost), false, true, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Income);
				this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, -faction.GetNegativeMonthlyIncomeFromUnassignedOrgs(FactionResource.MissionControl), false, true, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.research_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.projects_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
				this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.energy_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.materials_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.spaceScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.lifeScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.infoScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.militaryScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.socialScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.xenology_Bonus);
				return;
			}
			if (which == 3 && faction != GameControl.control.activePlayer)
			{
				bool flag = faction.AI_AtWarWithFaction(GameControl.control.activePlayer);
				this.SetLedgerEntryData(LedgerEntryCategory.money_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Money, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Money, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Influence, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Influence, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Operations, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.boost_income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Boost, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Boost, faction, true), flag, true, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Cost);
				this.SetEmptyLedgerValue(LedgerEntryCategory.research_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.projects_Income);
				this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
				this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
				this.SetLedgerEntryData(LedgerEntryCategory.water_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Water, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.water_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Water, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Volatiles, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Volatiles, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.metals_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Metals, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.metals_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Metals, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.nobles_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.NobleMetals, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.nobles_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.NobleMetals, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Fissiles, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Fissiles, faction, true), flag, true, false);
				this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Income, GameControl.control.activePlayer.GetMonthlyTransferInFromResourceTransfers(FactionResource.Antimatter, faction, true), flag, false, false);
				this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Cost, GameControl.control.activePlayer.GetMonthlyTransferOutFromResourceTransfers(FactionResource.Antimatter, faction, true), flag, true, false);
				this.SetEmptyLedgerValue(LedgerEntryCategory.energy_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.materials_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.spaceScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.lifeScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.infoScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.militaryScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.socialScience_Bonus);
				this.SetEmptyLedgerValue(LedgerEntryCategory.xenology_Bonus);
			}
		}

		// Token: 0x06004C3B RID: 19515 RVA: 0x002017FC File Offset: 0x001FF9FC
		public void SetItemData(TIHabModuleState habModule)
		{
			this.entryName = habModule.displayName;
			Sprite sprite;
			if (habModule.hab.habType == HabType.Station)
			{
				AssetCacheManager.habStationModuleIcons.TryGetValue(habModule.moduleTemplate.iconResource(habModule.hab.habType), out sprite);
			}
			else
			{
				AssetCacheManager.habBaseModuleIcons.TryGetValue(habModule.moduleTemplate.iconResource(habModule.hab.habType), out sprite);
			}
			this.entryIconSprite = sprite;
			this.SetLedgerEntryData(LedgerEntryCategory.money_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Money, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Money, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Influence, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Influence, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Operations, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_income);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Boost, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, (float)((habModule.moduleTemplate.missionControl > 0) ? habModule.moduleTemplate.missionControl : 0), !habModule.active, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, (float)((habModule.moduleTemplate.missionControl < 0) ? Mathf.Abs(habModule.moduleTemplate.missionControl) : 0), !habModule.active, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.research_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Research, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.projects_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Projects, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Gain, (float)habModule.moduleTemplate.ControlPointCapacity(habModule.hab.inEarthLEO), false, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.water_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Water, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.water_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Water, false, habModule.ref_faction, habModule.ref_hab) + habModule.moduleTemplate.MonthlyCrewSupportCost(FactionResource.Water, habModule.ref_faction, habModule.hab) * (1f - habModule.ref_hab.FarmCrewCoveredPct()), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Volatiles, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Volatiles, false, habModule.ref_faction, habModule.ref_hab) + habModule.moduleTemplate.MonthlyCrewSupportCost(FactionResource.Volatiles, habModule.ref_faction, habModule.hab) * (1f - habModule.ref_hab.FarmCrewCoveredPct()), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.metals_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Metals, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.metals_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Metals, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.nobles_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.NobleMetals, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.nobles_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.NobleMetals, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Fissiles, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Fissiles, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Income, habModule.moduleTemplate.MonthlyResourceRevenue(FactionResource.Antimatter, habModule.hab.location, habModule.ref_faction), !habModule.active, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Cost, habModule.moduleTemplate.MonthlySupportCost(FactionResource.Antimatter, true, habModule.ref_faction, habModule.ref_hab), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.energy_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.Energy), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.materials_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.Materials), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.SpaceScience), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.lifeScience_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.LifeScience), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.infoScience_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.InformationScience), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.militaryScience_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.MilitaryScience), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.socialScience_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.SocialScience), !habModule.active, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, habModule.moduleTemplate.GetTechBonusByCategory(TechCategory.Xenology), !habModule.active, false, true);
		}

		// Token: 0x06004C3C RID: 19516 RVA: 0x00201DB0 File Offset: 0x001FFFB0
		public void SetItemData(TISpaceFleetState fleet)
		{
			this.entryName = fleet.GetDisplayName(fleet.faction);
			this.entryIconSprite = fleet.icon;
			this.SetEmptyLedgerValue(LedgerEntryCategory.money_Income);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, fleet.ships.Sum<TISpaceShipState>((TISpaceShipState x) => x.GetMonthlyExpenses(FactionResource.Money)), false, true, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.influence_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.influence_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.ops_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Income);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, (float)fleet.MissionControlConsumption(), false, true, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.research_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.projects_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.energy_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.materials_Bonus);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, fleet.ships.Sum<TISpaceShipState>((TISpaceShipState y) => y.spaceScienceResearchBonus), false, false, true);
			this.SetEmptyLedgerValue(LedgerEntryCategory.lifeScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.infoScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.militaryScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.socialScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.xenology_Bonus);
		}

		// Token: 0x06004C3D RID: 19517 RVA: 0x00201F40 File Offset: 0x00200140
		public void SetItemData(TIHabState hab)
		{
			this.entryName = hab.displayName;
			this.entryIconSprite = hab.icon;
			this.SetLedgerEntryData(LedgerEntryCategory.money_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Money, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, hab.GetMonthlySupportCost(FactionResource.Money, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Influence, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, hab.GetMonthlySupportCost(FactionResource.Influence, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Operations, false), false, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_income);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, hab.GetMonthlySupportCost(FactionResource.Boost, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, (float)(from x in hab.ActiveModules()
				where x.moduleTemplate.missionControl > 0
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.missionControl), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, (float)Mathf.Abs((from x in hab.ActiveModules()
				where x.moduleTemplate.missionControl < 0
				select x).Sum<TIHabModuleState>((TIHabModuleState x) => x.moduleTemplate.missionControl)), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.research_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Research, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.projects_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Projects, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Gain, (float)hab.controlPointCapacityValue, false, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.water_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Water, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.water_Cost, hab.GetMonthlySupportCost(FactionResource.Water, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Volatiles, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.volatiles_Cost, hab.GetMonthlySupportCost(FactionResource.Volatiles, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.metals_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Metals, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.metals_Cost, hab.GetMonthlySupportCost(FactionResource.Metals, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.nobles_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.NobleMetals, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.nobles_Cost, hab.GetMonthlySupportCost(FactionResource.NobleMetals, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Fissiles, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.fissiles_Cost, hab.GetMonthlySupportCost(FactionResource.Fissiles, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Income, hab.GetMonthlyRevenue_WithAdviser(FactionResource.Antimatter, false), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.antimatter_Cost, hab.GetMonthlySupportCost(FactionResource.Antimatter, false), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.energy_Bonus, hab.GetNetTechBonusByFaction(TechCategory.Energy, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.materials_Bonus, hab.GetNetTechBonusByFaction(TechCategory.Materials, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, hab.GetNetTechBonusByFaction(TechCategory.SpaceScience, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.lifeScience_Bonus, hab.GetNetTechBonusByFaction(TechCategory.LifeScience, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.infoScience_Bonus, hab.GetNetTechBonusByFaction(TechCategory.InformationScience, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.militaryScience_Bonus, hab.GetNetTechBonusByFaction(TechCategory.MilitaryScience, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.socialScience_Bonus, hab.GetNetTechBonusByFaction(TechCategory.SocialScience, hab.faction, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, hab.GetNetTechBonusByFaction(TechCategory.Xenology, hab.faction, false), false, false, true);
		}

		// Token: 0x06004C3E RID: 19518 RVA: 0x00202288 File Offset: 0x00200488
		public void SetItemData(TISpaceShipState ship)
		{
			this.entryName = ship.displayName;
			this.SetEmptyLedgerValue(LedgerEntryCategory.money_Income);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, ship.GetMonthlyExpenses(FactionResource.Money), false, true, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.influence_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.influence_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.ops_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Income);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Cost, (float)ship.missionControlConsumption, false, true, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.research_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.projects_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.energy_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.materials_Bonus);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, ship.spaceScienceResearchBonus, false, false, true);
			this.SetEmptyLedgerValue(LedgerEntryCategory.lifeScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.infoScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.militaryScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.socialScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.xenology_Bonus);
		}

		// Token: 0x06004C3F RID: 19519 RVA: 0x002023C0 File Offset: 0x002005C0
		public void SetItemData(TICouncilorState councilor)
		{
			this.entryName = councilor.displayName;
			this.entryIconSprite = councilor.GetIcon(false);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.Money), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, councilor.GetMonthlyIncome_NegativeOnly(FactionResource.Money, true), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.Influence), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, councilor.GetMonthlyIncome_NegativeOnly(FactionResource.Influence, true), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.Operations), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.Boost), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, councilor.GetMonthlyIncome_NegativeOnly(FactionResource.Boost, true), false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.MissionControl), false, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.research_Income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.Research), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.projects_Income, councilor.GetMonthlyIncome_PositiveOnly(FactionResource.Projects), false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Gain, (float)councilor.controlPointCapacity, false, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.energy_Bonus, councilor.TotalTechBonus(TechCategory.Energy, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.materials_Bonus, councilor.TotalTechBonus(TechCategory.Materials, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, councilor.TotalTechBonus(TechCategory.SpaceScience, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.lifeScience_Bonus, councilor.TotalTechBonus(TechCategory.LifeScience, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.infoScience_Bonus, councilor.TotalTechBonus(TechCategory.InformationScience, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.militaryScience_Bonus, councilor.TotalTechBonus(TechCategory.MilitaryScience, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.socialScience_Bonus, councilor.TotalTechBonus(TechCategory.SocialScience, false), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, councilor.TotalTechBonus(TechCategory.Xenology, false), false, false, true);
		}

		// Token: 0x06004C40 RID: 19520 RVA: 0x002025B0 File Offset: 0x002007B0
		public void SetItemData(TINationState nation, TIFactionState faction)
		{
			this.entryName = nation.displayName;
			this.entryIconSprite = nation.flag;
			bool flag = nation.FactionControlPoints(faction, true, false, true).All<TIControlPoint>((TIControlPoint x) => x.benefitsDisabled);
			int num = nation.CountFactionControlPoints(faction, flag, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Income, nation.GetMonthlyCouncilResourceShare(faction, FactionResource.Money, flag), flag, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.money_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, nation.GetMonthlyCouncilResourceShare(faction, FactionResource.Influence, flag), flag, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.influence_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.ops_Income);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_income, nation.GetMonthlyCouncilResourceShare(faction, FactionResource.Boost, flag), flag, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.boost_cost);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, nation.GetMonthlyCouncilResourceShare(faction, FactionResource.MissionControl, flag), flag, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.research_Income, nation.GetMonthlyResearchFromControlPoint(faction) * (float)num, flag, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.projects_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
			this.SetLedgerEntryData(LedgerEntryCategory.CPCapacity_Cost, nation.FactionControlPoints(faction, false, false, true).Sum<TIControlPoint>((TIControlPoint x) => x.CurrentMaintenanceCost), false, true, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.energy_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.materials_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.spaceScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.lifeScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.infoScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.militaryScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.socialScience_Bonus);
			this.SetEmptyLedgerValue(LedgerEntryCategory.xenology_Bonus);
		}

		// Token: 0x06004C41 RID: 19521 RVA: 0x00202780 File Offset: 0x00200980
		public void SetItemData(TIOrgState org)
		{
			this.entryName = org.displayName;
			this.entryIconSprite = org.icon;
			this.SetLedgerEntryData(LedgerEntryCategory.money_Income, (org.adjustedIncomeMoney_month > 0f) ? org.adjustedIncomeMoney_month : 0f, !org.applyingBonuses, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, (org.adjustedIncomeMoney_month < 0f) ? (-org.adjustedIncomeMoney_month) : 0f, !org.applyingBonuses, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, (org.adjustedIncomeInfluence_month > 0f) ? org.adjustedIncomeInfluence_month : 0f, !org.applyingBonuses, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, (org.adjustedIncomeInfluence_month < 0f) ? (-org.adjustedIncomeInfluence_month) : 0f, !org.applyingBonuses, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, org.adjustedIncomeOps_month, !org.applyingBonuses, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_income, (org.adjustedIncomeBoost_month > 0f) ? org.adjustedIncomeBoost_month : 0f, !org.applyingBonuses, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, (org.adjustedIncomeBoost_month < 0f) ? (-org.adjustedIncomeBoost_month) : 0f, !org.applyingBonuses, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.missionControl_Income, org.incomeMissionControl, !org.applyingBonuses, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.research_Income, org.adjustedIncomeResearch_month, !org.applyingBonuses, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.projects_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.energy_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.Energy).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.materials_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.Materials).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.SpaceScience).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.lifeScience_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.LifeScience).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.infoScience_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.InformationScience).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.militaryScience_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.MilitaryScience).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.socialScience_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.SocialScience).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, org.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.Xenology).Sum<TechBonus>((TechBonus x) => x.bonus), !org.applyingBonuses, false, true);
		}

		// Token: 0x06004C42 RID: 19522 RVA: 0x00202C84 File Offset: 0x00200E84
		public void SetItemData(TITraitTemplate trait, TICouncilorState councilor)
		{
			this.entryName = trait.displayName;
			this.SetLedgerEntryData(LedgerEntryCategory.money_Income, (trait.incomeMoney > 0f) ? trait.incomeMoney : 0f, false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.money_Cost, (trait.incomeMoney < 0f) ? trait.incomeMoney : 0f, false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Income, (trait.incomeInfluence > 0f) ? trait.incomeInfluence : 0f, false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.influence_Cost, (trait.incomeInfluence < 0f) ? trait.incomeInfluence : 0f, false, true, false);
			this.SetLedgerEntryData(LedgerEntryCategory.ops_Income, trait.incomeOps, false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_income, (trait.incomeBoost > 0f) ? trait.incomeBoost : 0f, false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.boost_cost, (trait.incomeBoost < 0f) ? trait.incomeBoost : 0f, false, true, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.missionControl_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.research_Income, trait.incomeResearch, false, false, false);
			this.SetLedgerEntryData(LedgerEntryCategory.projects_Income, (float)trait.incomeProjects, false, false, false);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Gain);
			this.SetEmptyLedgerValue(LedgerEntryCategory.CPCapacity_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.water_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.volatiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.metals_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.nobles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.fissiles_Cost);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Income);
			this.SetEmptyLedgerValue(LedgerEntryCategory.antimatter_Cost);
			this.SetLedgerEntryData(LedgerEntryCategory.energy_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.Energy).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.materials_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.Materials).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.spaceScience_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.SpaceScience).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.lifeScience_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.LifeScience).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.infoScience_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.InformationScience).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.militaryScience_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.MilitaryScience).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.socialScience_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.SocialScience).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
			this.SetLedgerEntryData(LedgerEntryCategory.xenology_Bonus, trait.techBonuses.Where<TechBonus>((TechBonus x) => x.category == TechCategory.Xenology).Sum<TechBonus>((TechBonus x) => x.bonus), false, false, true);
		}

		// Token: 0x04002DE5 RID: 11749
		public TIGameState associatedState;

		// Token: 0x04002DE6 RID: 11750
		public TIDataTemplate associatedTemplate;

		// Token: 0x04002DE7 RID: 11751
		public TIGameState parentGameState;

		// Token: 0x04002DE8 RID: 11752
		public Sprite entryIconSprite;

		// Token: 0x04002DE9 RID: 11753
		public string entryName;

		// Token: 0x04002DEA RID: 11754
		public int which;

		// Token: 0x04002DEB RID: 11755
		public int sortOverride;

		// Token: 0x04002DEC RID: 11756
		public bool collapsible;

		// Token: 0x04002DED RID: 11757
		public bool collapsed;

		// Token: 0x04002DEE RID: 11758
		public string[] ledgerValueText = new string[36];

		// Token: 0x04002DEF RID: 11759
		[fsIgnore]
		public Dictionary<LedgerEntryCategory, float> ledgerValues = new Dictionary<LedgerEntryCategory, float>();
	}
}
