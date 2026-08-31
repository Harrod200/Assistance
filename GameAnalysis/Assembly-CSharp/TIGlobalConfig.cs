using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class TIGlobalConfig : TIDataTemplate
{
	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000AA2 RID: 2722 RVA: 0x00034999 File Offset: 0x00032B99
	public static TIGlobalConfig globalConfig
	{
		get
		{
			return TemplateManager.global;
		}
	}

	// Token: 0x06000AA3 RID: 2723 RVA: 0x000349A0 File Offset: 0x00032BA0
	public float GetRequiredInvestmentPoints(PriorityType priority)
	{
		if (this.requiredInvestmentPoints == null)
		{
			this.requiredInvestmentPoints = new Dictionary<PriorityType, float>
			{
				{
					PriorityType.Economy,
					this.priority_ECO
				},
				{
					PriorityType.Welfare,
					this.priority_WEL
				},
				{
					PriorityType.Environment,
					this.priority_ENV
				},
				{
					PriorityType.Knowledge,
					this.priority_KNO
				},
				{
					PriorityType.Government,
					this.priority_DEM
				},
				{
					PriorityType.Unity,
					this.priority_UNI
				},
				{
					PriorityType.Military,
					this.priority_MIL
				},
				{
					PriorityType.Oppression,
					this.priority_OPP
				},
				{
					PriorityType.Spoils,
					this.priority_SPO
				},
				{
					PriorityType.Funding,
					this.priority_DEV
				},
				{
					PriorityType.LaunchFacilities,
					this.priority_BOO
				},
				{
					PriorityType.MissionControl,
					this.priority_MC
				},
				{
					PriorityType.Military_FoundMilitary,
					this.priority_FMI
				},
				{
					PriorityType.Military_BuildArmy,
					this.priority_ARM
				},
				{
					PriorityType.Military_BuildNavy,
					this.priority_NAV
				},
				{
					PriorityType.Military_InitiateNuclearProgram,
					this.priority_NUC
				},
				{
					PriorityType.Military_BuildNuclearWeapons,
					this.priority_NUK
				},
				{
					PriorityType.Military_BuildSpaceDefenses,
					this.priority_DEF
				},
				{
					PriorityType.Military_BuildSTOSquadron,
					this.priority_STO
				},
				{
					PriorityType.Civilian_InitiateSpaceflightProgram,
					this.priority_FLI
				}
			};
		}
		return this.requiredInvestmentPoints[priority] / (TIGlobalValuesState.Customizations.usingCustomizations ? TIGlobalValuesState.Customizations.nationalIPMultiplier : 1f);
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00034AFC File Offset: 0x00032CFC
	public float GetPassiveTechInvestmentDifficultyScaling()
	{
		switch (TIGlobalValuesState.GlobalValues.difficulty)
		{
		case 1:
			return this.passiveTechInvestment_C;
		case 3:
			return this.passiveTechInvestment_V;
		case 4:
			return this.passiveTechInvestment_B;
		}
		return this.passiveTechInvestment_N;
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00034B4C File Offset: 0x00032D4C
	public float GetActiveTechInvestmentDifficultyScaling()
	{
		switch (TIGlobalValuesState.GlobalValues.difficulty)
		{
		case 1:
			return this.activeTechInvestment_C;
		case 3:
			return this.activeTechInvestment_V;
		case 4:
			return this.activeTechInvestment_B;
		}
		return this.activeTechInvestment_N;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00034B9C File Offset: 0x00032D9C
	public float GetAICriticalCouncilorStatChasingAggressivenessDifficultyScaling()
	{
		switch (TIGlobalValuesState.GlobalValues.difficulty)
		{
		case 1:
			return this.AI_CouncilorStatChasingMultiplier_C;
		case 3:
			return this.AI_CouncilorStatChasingMultiplier_V;
		case 4:
			return this.AI_CouncilorStatChasingMultiplier_B;
		}
		return this.AI_CouncilorStatChasingMultiplier_N;
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00034BEC File Offset: 0x00032DEC
	public float GetXenoformingAttributeBonusDifficultyScaling()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		if (this.xenoformingAttributeBonusDifficultyScaling == null)
		{
			this.xenoformingAttributeBonusDifficultyScaling = new Dictionary<int, float>
			{
				{ 1, this.TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_C },
				{ 2, this.TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_N },
				{ 3, this.TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_V },
				{ 4, this.TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_B }
			};
		}
		return this.xenoformingAttributeBonusDifficultyScaling[num];
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00034C6C File Offset: 0x00032E6C
	public float GetAbductionMissionBonusDifficultyScaling()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		if (this.abductionMissionBonusDifficultyScaling == null)
		{
			this.abductionMissionBonusDifficultyScaling = new Dictionary<int, float>
			{
				{ 1, this.TIMissionModifier_AbductionValueScaling_C },
				{ 2, this.TIMissionModifier_AbductionValueScaling_N },
				{ 3, this.TIMissionModifier_AbductionValueScaling_V },
				{ 4, this.TIMissionModifier_AbductionValueScaling_B }
			};
		}
		return this.abductionMissionBonusDifficultyScaling[num] * TIGlobalValuesState.Customizations.alienProgressionSpeed;
	}

	// Token: 0x06000AA9 RID: 2729 RVA: 0x00034CF8 File Offset: 0x00032EF8
	public float GetAIShipbuildingCostDifficultyScaling(TIFactionState faction)
	{
		if (faction == GameControl.control.activePlayer)
		{
			return 1f;
		}
		if (faction.IsActiveHumanFaction)
		{
			if (this.AI_HumanShipbuildingCostDifficultyScaling == null)
			{
				this.AI_HumanShipbuildingCostDifficultyScaling = new Dictionary<int, float>
				{
					{ 1, this.TIModifier_HumanAIShipBuildingScaling_C },
					{ 2, this.TIModifier_HumanAIShipBuildingScaling_N },
					{ 3, this.TIModifier_HumanAIShipBuildingScaling_V },
					{ 4, this.TIModifier_HumanAIShipBuildingScaling_B }
				};
			}
			return this.AI_HumanShipbuildingCostDifficultyScaling[TIGlobalValuesState.GlobalValues.difficulty];
		}
		if (faction.IsAlienFaction)
		{
			int num = TIGlobalValuesState.GlobalValues.difficulty;
			if (GameControl.control.activePlayer.IsAlienProxy)
			{
				num = 5 - num;
			}
			if (this.AI_AlienShipbuildingCostDifficultyScaling == null)
			{
				this.AI_AlienShipbuildingCostDifficultyScaling = new Dictionary<int, float>
				{
					{ 1, this.TIModifier_AlienAIShipBuildingScaling_C },
					{ 2, this.TIModifier_AlienAIShipBuildingScaling_N },
					{ 3, this.TIModifier_AlienAIShipBuildingScaling_V },
					{ 4, this.TIModifier_AlienAIShipBuildingScaling_B }
				};
			}
			return this.AI_AlienShipbuildingCostDifficultyScaling[num];
		}
		return 1f;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00034E04 File Offset: 0x00033004
	public float GetCampaignDurationBeforeAlienAdvancedTech()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return (float)this.yearsBeforeAlienAdvancedTech_C;
		case 3:
			return (float)this.yearsBeforeAlienAdvancedTech_V;
		case 4:
			return (float)this.yearsBeforeAlienAdvancedTech_B;
		}
		return (float)this.yearsBeforeAlienAdvancedTech_N;
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00034E6C File Offset: 0x0003306C
	public bool UseAlternateTriggersForAlienAdvancedTech()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.useAlternateTriggersForAlienAdvancedTech_C;
		case 3:
			return this.useAlternateTriggersForAlienAdvancedTech_V;
		case 4:
			return this.useAlternateTriggersForAlienAdvancedTech_B;
		}
		return this.useAlternateTriggersForAlienAdvancedTech_N;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00034ED0 File Offset: 0x000330D0
	public float GetCampaignDurationBeforeAlienInnerSystemExoticAttacks()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return (float)this.yearsBeforeAlienInnerSystemExoticAttacks_C;
		case 3:
			return (float)this.yearsBeforeAlienInnerSystemExoticAttacks_V;
		case 4:
			return (float)this.yearsBeforeAlienInnerSystemExoticAttacks_B;
		}
		return (float)this.yearsBeforeAlienInnerSystemExoticAttacks_N;
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00034F38 File Offset: 0x00033138
	public float GetCampaignDurationBeforeAlienInnerSystemOffensives()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return (float)this.yearsBeforeInnerSystemOffensives_C;
		case 3:
			return (float)this.yearsBeforeInnerSystemOffensives_V;
		case 4:
			return (float)this.yearsBeforeInnerSystemOffensives_B;
		}
		return (float)this.yearsBeforeInnerSystemOffensives_N;
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000AAE RID: 2734 RVA: 0x00034F9E File Offset: 0x0003319E
	public static bool AlienInnerSystemExoticAttacksAreActive
	{
		get
		{
			return TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact() >= TIGlobalConfig.globalConfig.GetCampaignDurationBeforeAlienInnerSystemExoticAttacks();
		}
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x00034FB4 File Offset: 0x000331B4
	public float GetCampaignDurationBeforeAlienTotalWar()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return (float)this.yearsBeforeAlienTotalWarAllowed_C;
		case 3:
			return (float)this.yearsBeforeAlienTotalWarAllowed_V;
		case 4:
			return (float)this.yearsBeforeAlienTotalWarAllowed_B;
		}
		return (float)this.yearsBeforeAlienTotalWarAllowed_N;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x0003501A File Offset: 0x0003321A
	public bool IsAlienTotalWarPossible()
	{
		return TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact() >= this.GetCampaignDurationBeforeAlienTotalWar();
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x0003502C File Offset: 0x0003322C
	public float GetMaxAlienBases(float campaignDuration_y)
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		int num2;
		switch (num)
		{
		case 1:
			num2 = this.maxAlienBaseGoals_C;
			goto IL_005C;
		case 3:
			num2 = this.maxAlienBaseGoals_V;
			goto IL_005C;
		case 4:
			num2 = this.maxAlienBaseGoals_B;
			goto IL_005C;
		}
		num2 = this.maxAlienBaseGoals_N;
		IL_005C:
		if (campaignDuration_y >= this.GetCampaignDurationBeforeAlienTotalWar())
		{
			switch (num)
			{
			case 1:
				num2 += this.extraMaxAlienBaseGoals_TotalWarEra_C;
				goto IL_00A9;
			case 3:
				num2 += this.extraMaxAlienBaseGoals_TotalWarEra_V;
				goto IL_00A9;
			case 4:
				num2 += this.extraMaxAlienBaseGoals_TotalWarEra_B;
				goto IL_00A9;
			}
			num2 += this.extraMaxAlienBaseGoals_TotalWarEra_N;
		}
		IL_00A9:
		return (float)num2;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x000350E4 File Offset: 0x000332E4
	public float GetYearsUntilFirstAlienInvasionDifficultyScaling()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return (float)this.extraYearsToDelayAlienInvasion_C;
		case 3:
			return (float)this.extraYearsToDelayAlienInvasion_V;
		case 4:
			return (float)this.extraYearsToDelayAlienInvasion_B;
		}
		return (float)this.extraYearsToDelayAlienInvasion_N;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x0003514C File Offset: 0x0003334C
	public float GetAlienSteadyHateGainModifier(int difficulty = -1)
	{
		if (difficulty < 0)
		{
			difficulty = TIGlobalValuesState.GlobalValues.difficulty;
		}
		switch (difficulty)
		{
		case 1:
			return this.steadyAlienHateGainModifier_C;
		case 3:
			return this.steadyAlienHateGainModifier_V;
		case 4:
			return this.steadyAlienHateGainModifier_B;
		}
		return this.steadyAlienHateGainModifier_N;
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x000351A0 File Offset: 0x000333A0
	public bool DoAliensHaveReducedWarAttacks()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.alienReducedWarAttacks_C;
		case 3:
			return this.alienReducedWarAttacks_V;
		case 4:
			return this.alienReducedWarAttacks_B;
		}
		return this.alienReducedWarAttacks_N;
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00035204 File Offset: 0x00033404
	public int GetAlienMaxExtraWarAttacks()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.alienMaxExtraWarAttacks_C;
		case 3:
			return this.alienMaxExtraWarAttacks_V;
		case 4:
			return this.alienMaxExtraWarAttacks_B;
		}
		return this.alienMaxExtraWarAttacks_N;
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x00035268 File Offset: 0x00033468
	public float GetAlienStartingHateMaximum()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.alienStartingHateMaxmum_C;
		case 3:
			return this.alienStartingHateMaxmum_V;
		case 4:
			return this.alienStartingHateMaxmum_B;
		}
		return this.alienStartingHateMaxmum_N;
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x000352CC File Offset: 0x000334CC
	public float GetAlienHateMaximumIncreasePerYear()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.alienHateMaximumIncreasePerYear_C;
		case 3:
			return this.alienHateMaximumIncreasePerYear_V;
		case 4:
			return this.alienHateMaximumIncreasePerYear_B;
		}
		return this.alienHateMaximumIncreasePerYear_N;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00035330 File Offset: 0x00033530
	public float GetAlienHateMaximum()
	{
		float alienStartingHateMaximum = this.GetAlienStartingHateMaximum();
		float alienHateMaximumIncreasePerYear = this.GetAlienHateMaximumIncreasePerYear();
		float alienProgressionModifiedDuration_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact();
		float num = alienStartingHateMaximum + alienHateMaximumIncreasePerYear * alienProgressionModifiedDuration_years_exact;
		float campaignDurationBeforeAlienTotalWar = this.GetCampaignDurationBeforeAlienTotalWar();
		if (alienProgressionModifiedDuration_years_exact >= campaignDurationBeforeAlienTotalWar)
		{
			num = Mathf.Max(num, FactionGoal_WarOnFaction.AlienTotalWarHateThreshold);
		}
		return num;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00035374 File Offset: 0x00033574
	public float GetAlienCallOffWarAttacksThreshold()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.alienCallOffWarAttacksThreshold_C;
		case 3:
			return this.alienCallOffWarAttacksThreshold_V;
		case 4:
			return this.alienCallOffWarAttacksThreshold_B;
		}
		return this.alienCallOffWarAttacksThreshold_N;
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x000353D6 File Offset: 0x000335D6
	public bool DoAliensGiveHateReprieveAfterKnockdown()
	{
		return this.AlienHateReprieveAfterKnockdown() > 0f;
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x000353E8 File Offset: 0x000335E8
	public float AlienHateReprieveAfterKnockdown()
	{
		switch (TIGlobalValuesState.GlobalValues.difficulty)
		{
		case 1:
			return this.alienHateReprieveAfterKnockdown_C;
		case 3:
			return this.alienHateReprieveAfterKnockdown_V;
		case 4:
			return this.alienHateReprieveAfterKnockdown_B;
		}
		return this.alienHateReprieveAfterKnockdown_N;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x00035438 File Offset: 0x00033638
	public float AI_GetDifficultyBasedMaxAttackFleetStrengthRatio(bool alien)
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		float alienProgressionModifiedDuration_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_years_exact();
		if (alien)
		{
			if (GameControl.control.activePlayer.IsAlienProxy)
			{
				num = 5 - num;
			}
			switch (num)
			{
			case 1:
				return this.initialMaxAlienAttackFleetStrengthRatio_C * (1f + ((this.increaseAlienMaxAttackFleetStrengthRatioOverTime_C > 0f) ? (alienProgressionModifiedDuration_years_exact / this.increaseAlienMaxAttackFleetStrengthRatioOverTime_C) : 0f));
			case 3:
				return this.initialMaxAlienAttackFleetStrengthRatio_V * (1f + ((this.increaseAlienMaxAttackFleetStrengthRatioOverTime_V > 0f) ? (alienProgressionModifiedDuration_years_exact / this.increaseAlienMaxAttackFleetStrengthRatioOverTime_V) : 0f));
			case 4:
				return this.initialMaxAlienAttackFleetStrengthRatio_B * (1f + ((this.increaseAlienMaxAttackFleetStrengthRatioOverTime_B > 0f) ? (alienProgressionModifiedDuration_years_exact / this.increaseAlienMaxAttackFleetStrengthRatioOverTime_B) : 0f));
			}
			return this.initialMaxAlienAttackFleetStrengthRatio_N * (1f + ((this.increaseAlienMaxAttackFleetStrengthRatioOverTime_N > 0f) ? (alienProgressionModifiedDuration_years_exact / this.increaseAlienMaxAttackFleetStrengthRatioOverTime_N) : 0f));
		}
		switch (num)
		{
		case 1:
			return this.maxHumanAttackFleetStrengthRatio_C;
		case 3:
			return this.maxHumanAttackFleetStrengthRatio_V;
		case 4:
			return this.maxHumanAttackFleetStrengthRatio_B;
		}
		return this.maxHumanAttackFleetStrengthRatio_N;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00035568 File Offset: 0x00033768
	public float GetDifficultyBasedYearsToDelayAlienMiddleColonization()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.yearsToDelayAlienMiddleColonization_C;
		case 3:
			return this.yearsToDelayAlienMiddleColonization_V;
		case 4:
			return this.yearsToDelayAlienMiddleColonization_B;
		}
		return this.yearsToDelayAlienMiddleColonization_N;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x000355CC File Offset: 0x000337CC
	public int GetDifficultyBasedAlienNonPlanetaryOuterSystemColonizationLimit()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.alienNonPlanetaryOuterSystemColonizationLimit_C;
		case 3:
			return this.alienNonPlanetaryOuterSystemColonizationLimit_V;
		case 4:
			return this.alienNonPlanetaryOuterSystemColonizationLimit_B;
		}
		return this.alienNonPlanetaryOuterSystemColonizationLimit_N;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x00035630 File Offset: 0x00033830
	public float AI_GetHateBurnoffFromKillingHabmodulesDivisor(bool alien)
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (alien && GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.hateBurnoffFromKillingHabmodulesDivisor_C;
		case 3:
			return this.hateBurnoffFromKillingHabmodulesDivisor_V;
		case 4:
			return this.hateBurnoffFromKillingHabmodulesDivisor_B;
		}
		return this.hateBurnoffFromKillingHabmodulesDivisor_N;
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x00035698 File Offset: 0x00033898
	public float AI_GlobalMissionDifficultyModifier_Att(TICouncilorState attackingCouncilor, TIGameState target)
	{
		if (attackingCouncilor.faction.player.isAI)
		{
			bool? flag;
			if (target == null)
			{
				flag = null;
			}
			else
			{
				TIFactionState ref_faction = target.ref_faction;
				flag = ((ref_faction != null) ? new bool?(!ref_faction.player.isAI) : null);
			}
			if (flag ?? true)
			{
				switch (TIGlobalValuesState.GlobalValues.difficulty)
				{
				case 1:
					return this.AI_MissionAttackerBonus_C;
				case 3:
					return this.AI_MissionAttackerBonus_V;
				case 4:
					return this.AI_MissionAttackerBonus_B;
				}
				return this.AI_MissionAttackerBonus_N;
			}
		}
		return 0f;
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0003574C File Offset: 0x0003394C
	public float AI_GlobalMissionDifficultyModifier_Def(TICouncilorState attackingCouncilor, TIGameState target)
	{
		if (!attackingCouncilor.faction.player.isAI)
		{
			bool? flag;
			if (target == null)
			{
				flag = null;
			}
			else
			{
				TIFactionState ref_faction = target.ref_faction;
				flag = ((ref_faction != null) ? new bool?(ref_faction.player.isAI) : null);
			}
			bool? flag2 = flag;
			if (flag2.GetValueOrDefault())
			{
				switch (TIGlobalValuesState.GlobalValues.difficulty)
				{
				case 1:
					return this.AI_MissionDefenderBonus_C;
				case 3:
					return this.AI_MissionDefenderBonus_V;
				case 4:
					return this.AI_MissionDefenderBonus_B;
				}
				return this.AI_MissionDefenderBonus_N;
			}
		}
		return 0f;
	}

	// Token: 0x06000AC2 RID: 2754 RVA: 0x000357F0 File Offset: 0x000339F0
	public float AI_AlienHatePerMCUtilitizedMultiplier()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.AI_AlienHatePerMCUtilitizedMultiplier_C;
		case 3:
			return this.AI_AlienHatePerMCUtilitizedMultiplier_V;
		case 4:
			return this.AI_AlienHatePerMCUtilitizedMultiplier_B;
		}
		return this.AI_AlienHatePerMCUtilitizedMultiplier_N;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x00035854 File Offset: 0x00033A54
	public float AI_GangUpOnLeaderBehavior_MinIdeologicalDistance_Difficulty()
	{
		switch (TIGlobalValuesState.GlobalValues.difficulty)
		{
		case 1:
			return this.AI_GangUpOnLeaderMinimumIdeologicalDistance_C;
		case 3:
			return this.AI_GangUpOnLeaderMinimumIdeologicalDistance_V;
		case 4:
			return this.AI_GangUpOnLeaderMinimumIdeologicalDistance_B;
		}
		return this.AI_GangUpOnLeaderMinimumIdeologicalDistance_N;
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x000358A4 File Offset: 0x00033AA4
	public float AI_BonusFreeMissionControl_Difficulty(int difficulty)
	{
		switch (difficulty)
		{
		case 1:
			return this.AI_BonusMissionControl_C;
		case 3:
			return this.AI_BonusMissionControl_V;
		case 4:
			return this.AI_BonusMissionControl_B;
		}
		return this.AI_BonusMissionControl_N;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x000358E8 File Offset: 0x00033AE8
	public float AI_BonusFreeCPCap_Difficulty(int difficulty)
	{
		switch (difficulty)
		{
		case 1:
			return this.AI_BonusCPCap_C;
		case 3:
			return this.AI_BonusCPCap_V;
		case 4:
			return this.AI_BonusCPCap_B;
		}
		return this.AI_BonusCPCap_N;
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x0003592C File Offset: 0x00033B2C
	public float AI_BonusInfluenceOnPlayerCouncilorSelect()
	{
		switch (TIGlobalValuesState.GlobalValues.difficulty)
		{
		case 1:
			return 0f;
		case 3:
			return 45f;
		case 4:
			return 60f;
		}
		return 30f;
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x00035978 File Offset: 0x00033B78
	public float AI_GetExoticsMultiplier()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.AI_AlienExoticMultiplier_C * TIGlobalValuesState.Customizations.alienProgressionSpeed;
		case 3:
			return this.AI_AlienExoticMultiplier_V * TIGlobalValuesState.Customizations.alienProgressionSpeed;
		case 4:
			return this.AI_AlienExoticMultiplier_B * TIGlobalValuesState.Customizations.alienProgressionSpeed;
		}
		return this.AI_AlienExoticMultiplier_N * TIGlobalValuesState.Customizations.alienProgressionSpeed;
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00035A08 File Offset: 0x00033C08
	public float AI_AlienEarthFleetSizeModifier()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.AI_AlienEarthFleetSizeModifier_C;
		case 3:
			return this.AI_AlienEarthFleetSizeModifier_V;
		case 4:
			return this.AI_AlienEarthFleetSizeModifier_B;
		}
		return this.AI_AlienEarthFleetSizeModifier_N;
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x00035A6C File Offset: 0x00033C6C
	public float AI_AlienEarthFleetExcessModifier()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.AI_AlienEarthFleetExcessModifier_C;
		case 3:
			return this.AI_AlienEarthFleetExcessModifier_V;
		case 4:
			return this.AI_AlienEarthFleetExcessModifier_B;
		}
		return this.AI_AlienEarthFleetExcessModifier_N;
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x00035AD0 File Offset: 0x00033CD0
	public float Diff_GetExoticsSalvageRate()
	{
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return this.Diff_ExoticsSalvageRate_C / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		case 3:
			return this.Diff_ExoticsSalvageRate_V / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		case 4:
			return this.Diff_ExoticsSalvageRate_B / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		}
		return this.Diff_ExoticsSalvageRate_N / TIGlobalValuesState.Customizations.alienProgressionSpeed;
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x00035B60 File Offset: 0x00033D60
	public float AI_AlienSurveillanceDelay_years()
	{
		float alienSurveillanceDelay_years = GameStateManager.Time().template.alienSurveillanceDelay_years;
		int num = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num = 5 - num;
		}
		switch (num)
		{
		case 1:
			return alienSurveillanceDelay_years * this.AI_AlienSurveillanceDelayModifier_C / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		case 3:
			return alienSurveillanceDelay_years * this.AI_AlienSurveillanceDelayModifier_V / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		case 4:
			return alienSurveillanceDelay_years * this.AI_AlienSurveillanceDelayModifier_B / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		}
		return alienSurveillanceDelay_years * this.AI_AlienSurveillanceDelayModifier_N / TIGlobalValuesState.Customizations.alienProgressionSpeed;
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x00035C08 File Offset: 0x00033E08
	public bool AI_AliensMaySurveil()
	{
		float alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact();
		float num = this.AI_AlienSurveillanceDelay_years();
		return alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact >= num;
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x00035C28 File Offset: 0x00033E28
	public float AI_AlienBaseQuietness()
	{
		float num = GameStateManager.Time().template.alienQuietDuration_years / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		if (num <= 0f)
		{
			return 0f;
		}
		int num2 = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num2 = 5 - num2;
		}
		switch (num2)
		{
		case 1:
			num *= this.AI_AlienQuiescence_C;
			goto IL_008D;
		case 3:
			num *= this.AI_AlienQuiescence_V;
			goto IL_008D;
		case 4:
			num *= this.AI_AlienQuiescence_B;
			goto IL_008D;
		}
		num *= this.AI_AlienQuiescence_N;
		IL_008D:
		float alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact();
		return 1f - Mathf.Clamp01(alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact / num);
	}

	// Token: 0x06000ACE RID: 2766 RVA: 0x00035CD8 File Offset: 0x00033ED8
	public float AI_AliensWormholeSetupFraction()
	{
		float num = GameStateManager.Time().template.alienSetupDuration_years / TIGlobalValuesState.Customizations.alienProgressionSpeed;
		if (num <= 0f)
		{
			return 1f;
		}
		float alienSetupStartIncome = GameStateManager.Time().template.alienSetupStartIncome;
		float alienSetupEndIncome = GameStateManager.Time().template.alienSetupEndIncome;
		int num2 = TIGlobalValuesState.GlobalValues.difficulty;
		if (GameControl.control.activePlayer.IsAlienProxy)
		{
			num2 = 5 - num2;
		}
		switch (num2)
		{
		case 1:
			num *= this.AI_WormholeSetupSpeed_C;
			goto IL_00AD;
		case 3:
			num *= this.AI_WormholeSetupSpeed_V;
			goto IL_00AD;
		case 4:
			num *= this.AI_WormholeSetupSpeed_B;
			goto IL_00AD;
		}
		num *= this.AI_WormholeSetupSpeed_N;
		IL_00AD:
		float alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact = TIGlobalValuesState.GetAlienProgressionModifiedDuration_IgnoreStartingProgression_years_exact();
		return Mathf.Lerp(alienSetupStartIncome, alienSetupEndIncome, Mathf.Clamp01(alienProgressionModifiedDuration_IgnoreStartingProgression_years_exact / num));
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x00035DAC File Offset: 0x00033FAC
	public TIGlobalConfig()
	{
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x00038CE8 File Offset: 0x00036EE8
	public TIGlobalConfig(string templateName)
		: base(templateName)
	{
	}

	// Token: 0x04000961 RID: 2401
	public static string globalName = "globalConfig";

	// Token: 0x04000962 RID: 2402
	public const int difficulties = 4;

	// Token: 0x04000963 RID: 2403
	public const int invertDiffCap = 5;

	// Token: 0x04000964 RID: 2404
	private Dictionary<PriorityType, float> requiredInvestmentPoints;

	// Token: 0x04000965 RID: 2405
	private Dictionary<int, float> xenoformingAttributeBonusDifficultyScaling;

	// Token: 0x04000966 RID: 2406
	private Dictionary<int, float> abductionMissionBonusDifficultyScaling;

	// Token: 0x04000967 RID: 2407
	private Dictionary<int, float> AI_HumanShipbuildingCostDifficultyScaling;

	// Token: 0x04000968 RID: 2408
	private Dictionary<int, float> AI_AlienShipbuildingCostDifficultyScaling;

	// Token: 0x04000969 RID: 2409
	public List<ControlPointChangeCause> consolidationRequiredExecChange = new List<ControlPointChangeCause>
	{
		ControlPointChangeCause.Politics,
		ControlPointChangeCause.Event,
		ControlPointChangeCause.Trade,
		ControlPointChangeCause.Growth,
		ControlPointChangeCause.Enthrall
	};

	// Token: 0x0400096A RID: 2410
	public string[] canvasesToLoad;

	// Token: 0x0400096B RID: 2411
	public string[] skirmishCanvasesToLoad;

	// Token: 0x0400096C RID: 2412
	public int terminalKeyCode = -1;

	// Token: 0x0400096D RID: 2413
	public int quotes = 28;

	// Token: 0x0400096E RID: 2414
	public int creditsEntries = 1351;

	// Token: 0x0400096F RID: 2415
	public int numberOfLoadingScreenTips = 23;

	// Token: 0x04000970 RID: 2416
	public List<int> strategyLayerSpeedSettings;

	// Token: 0x04000971 RID: 2417
	public List<int> combatLayerSpeedSettings;

	// Token: 0x04000972 RID: 2418
	public bool diff_initialCouncilorsFavoredStat = true;

	// Token: 0x04000973 RID: 2419
	public int controlPointMaintenanceFreebies = 125;

	// Token: 0x04000974 RID: 2420
	public int controlPointBonusMaintenanceFreebiesPerRemovedFaction = 50;

	// Token: 0x04000975 RID: 2421
	public int spaceMineFreebies;

	// Token: 0x04000976 RID: 2422
	public bool dontStopBimonthlyMissions;

	// Token: 0x04000977 RID: 2423
	public bool useSiteNameWhenNamingBases;

	// Token: 0x04000978 RID: 2424
	public int researchSpeedSliderMax = 80;

	// Token: 0x04000979 RID: 2425
	public int controlPointFreebieSliderMax = 50;

	// Token: 0x0400097A RID: 2426
	public int controlPointAIFreebieSliderMax = 50;

	// Token: 0x0400097B RID: 2427
	public int missionControlFreebieSliderMax = 25;

	// Token: 0x0400097C RID: 2428
	public int missionControlAIFreebieSliderMax = 25;

	// Token: 0x0400097D RID: 2429
	public int miningProductivitySliderMax = 100;

	// Token: 0x0400097E RID: 2430
	public int alienProgressionRateSliderMax = 80;

	// Token: 0x0400097F RID: 2431
	public int miningRateSliderMax = 80;

	// Token: 0x04000980 RID: 2432
	public int habConstructionSpeedSliderMax = 80;

	// Token: 0x04000981 RID: 2433
	public int shipConstructionSpeedSliderMax = 80;

	// Token: 0x04000982 RID: 2434
	public int IPMultiplierSliderMax = 8;

	// Token: 0x04000983 RID: 2435
	public int randomEventsPerMonthSliderMax = 12;

	// Token: 0x04000984 RID: 2436
	public int defaultRandomEventsPerMonth = 5;

	// Token: 0x04000985 RID: 2437
	public int pointsPerCPSliderTick = 25;

	// Token: 0x04000986 RID: 2438
	public int pointsPerMCSliderTick = 10;

	// Token: 0x04000987 RID: 2439
	public int defaultMiningProductivity = 20;

	// Token: 0x04000988 RID: 2440
	public int defaultSpaceBodyCapForMiningProductivityBonus = 150;

	// Token: 0x04000989 RID: 2441
	public bool defaultDisableFactionValue;

	// Token: 0x0400098A RID: 2442
	public bool immediateNewsAlert = true;

	// Token: 0x0400098B RID: 2443
	public bool verboseStatDescriptions = true;

	// Token: 0x0400098C RID: 2444
	public int[] uiScaleValues = new int[] { 1080, 1060, 1040, 1020, 1000, 980, 960, 940, 930 };

	// Token: 0x0400098D RID: 2445
	public int initialMaxOrgsAvailableToCouncil = 5;

	// Token: 0x0400098E RID: 2446
	public float ExcessMCToMoneyConversion_Day = 0.2f;

	// Token: 0x0400098F RID: 2447
	public float ExcessMCToResearchConversion_Day = 0.075f;

	// Token: 0x04000990 RID: 2448
	public int maxFactionOrgPoolSize = 10;

	// Token: 0x04000991 RID: 2449
	public float atrocityPOMultiplier = 1f;

	// Token: 0x04000992 RID: 2450
	public int maxFactionCouncilorCandidatePool = 8;

	// Token: 0x04000993 RID: 2451
	public int maxFactionCouncilorCandidatePoolVariance = 2;

	// Token: 0x04000994 RID: 2452
	public bool allowNegativeInfluenceBaseIncome;

	// Token: 0x04000995 RID: 2453
	public float chanceCouncilorTemplate = 0.15f;

	// Token: 0x04000996 RID: 2454
	public int maxCouncilorAttribute = 25;

	// Token: 0x04000997 RID: 2455
	public float characterGenRegionCoreEcoModifier = 1.1f;

	// Token: 0x04000998 RID: 2456
	public float characterGenRegionHighEducationModifer = 1f;

	// Token: 0x04000999 RID: 2457
	public float characterGenRegionHighEducationThreshhold = 8f;

	// Token: 0x0400099A RID: 2458
	public float characterGenRegionVeryHighEducationModifier = 1f;

	// Token: 0x0400099B RID: 2459
	public float characterGenRegionVeryHighEducationThreshhold = 9f;

	// Token: 0x0400099C RID: 2460
	public string alienShockTroopOrgDataName = "SalamanderShockTroopUnit";

	// Token: 0x0400099D RID: 2461
	public int antiAffinityCouncilorRecruitCost_influence = 120;

	// Token: 0x0400099E RID: 2462
	public int baseCouncilorRecruitCost_influence = 60;

	// Token: 0x0400099F RID: 2463
	public int affinityCouncilorRecruitCost_influence = 30;

	// Token: 0x040009A0 RID: 2464
	public int skipCouncilorInfluenceBonus = 30;

	// Token: 0x040009A1 RID: 2465
	public int monthsOfDetectionDefenseAfterRecruiting = 6;

	// Token: 0x040009A2 RID: 2466
	public float postRecruitingDetectionDefenseMultiplier = 2f;

	// Token: 0x040009A3 RID: 2467
	public float alienDetectionBonusCapFromLEOHabs = 9f;

	// Token: 0x040009A4 RID: 2468
	public float humanDetectionBonusCapFromLEOHabs = 9f;

	// Token: 0x040009A5 RID: 2469
	public int councilorMaxOrgs = 15;

	// Token: 0x040009A6 RID: 2470
	public int XPToLevelUp = 20;

	// Token: 0x040009A7 RID: 2471
	public int initialXPPerYearAge = 2;

	// Token: 0x040009A8 RID: 2472
	public int minAgeForXPBonus = 30;

	// Token: 0x040009A9 RID: 2473
	public float HighUnrestDefinition = 6f;

	// Token: 0x040009AA RID: 2474
	public int additivePerModifierCap = 10;

	// Token: 0x040009AB RID: 2475
	public float sellOrgDiscount = 0.333334f;

	// Token: 0x040009AC RID: 2476
	public float transferOrgCostMultiplier = 0.2f;

	// Token: 0x040009AD RID: 2477
	public float priority_ECO = 1f;

	// Token: 0x040009AE RID: 2478
	public float priority_WEL = 1f;

	// Token: 0x040009AF RID: 2479
	public float priority_ENV = 1f;

	// Token: 0x040009B0 RID: 2480
	public float priority_KNO = 1f;

	// Token: 0x040009B1 RID: 2481
	public float priority_DEM = 1f;

	// Token: 0x040009B2 RID: 2482
	public float priority_UNI = 2f;

	// Token: 0x040009B3 RID: 2483
	public float priority_MIL = 1f;

	// Token: 0x040009B4 RID: 2484
	public float priority_OPP = 1f;

	// Token: 0x040009B5 RID: 2485
	public float priority_SPO = 1f;

	// Token: 0x040009B6 RID: 2486
	public float priority_DEV = 1f;

	// Token: 0x040009B7 RID: 2487
	public float priority_BOO = 2f;

	// Token: 0x040009B8 RID: 2488
	public float priority_MC = 25f;

	// Token: 0x040009B9 RID: 2489
	public float priority_FLI = 50f;

	// Token: 0x040009BA RID: 2490
	public float priority_FMI = 40f;

	// Token: 0x040009BB RID: 2491
	public float priority_ARM = 60f;

	// Token: 0x040009BC RID: 2492
	public float priority_NAV = 100f;

	// Token: 0x040009BD RID: 2493
	public float priority_NUC = 80f;

	// Token: 0x040009BE RID: 2494
	public float priority_NUK = 25f;

	// Token: 0x040009BF RID: 2495
	public float priority_DEF = 50f;

	// Token: 0x040009C0 RID: 2496
	public float priority_STO = 10f;

	// Token: 0x040009C1 RID: 2497
	public int numEcosForCoreEcoRegion = 1200;

	// Token: 0x040009C2 RID: 2498
	public int numEcosForCoreMiningRegion = 750;

	// Token: 0x040009C3 RID: 2499
	public int numEcosForCoreOilRegion = 500;

	// Token: 0x040009C4 RID: 2500
	public int numPrioritiesForLegitimize = 200;

	// Token: 0x040009C5 RID: 2501
	public float nationalInvestmentArmyFactorHome = 0.5f;

	// Token: 0x040009C6 RID: 2502
	public float nationalInvestmentArmyFactorAway = 1f;

	// Token: 0x040009C7 RID: 2503
	public float nationalInvestmentNavyFactor = 0.5f;

	// Token: 0x040009C8 RID: 2504
	public float maxMonthlyCohesionIncrease_normal = 0.1f;

	// Token: 0x040009C9 RID: 2505
	public float maxMonthlyCohesionDecrease_normal = 0.1f;

	// Token: 0x040009CA RID: 2506
	public float maxMonthlyCohesionDecrease_cap = 0.25f;

	// Token: 0x040009CB RID: 2507
	public float maxMonthlyUnrestMovement_normal = 0.25f;

	// Token: 0x040009CC RID: 2508
	public float maxMonthlyUnrestMovement_rapidIncrease = 1f;

	// Token: 0x040009CD RID: 2509
	public float democracyDecreaseToMakeHostileClaim = 1.5f;

	// Token: 0x040009CE RID: 2510
	public float maxCombinedImpactFromHostileClaims = 16f;

	// Token: 0x040009CF RID: 2511
	public bool fullQuarterlyTracking;

	// Token: 0x040009D0 RID: 2512
	public float badInequality = 4f;

	// Token: 0x040009D1 RID: 2513
	public float severeInequality = 4.75f;

	// Token: 0x040009D2 RID: 2514
	public float cohesionImpactPerKMtoPopCenter = 0.0025f;

	// Token: 0x040009D3 RID: 2515
	public float maxDistanceImpactOnCohesion = -7.5f;

	// Token: 0x040009D4 RID: 2516
	public float cohesionImpactMultiplierIfSeparatistMovement = 1f;

	// Token: 0x040009D5 RID: 2517
	public float inequalityCohesionMultiplier = 2.25f;

	// Token: 0x040009D6 RID: 2518
	public float populationCohesionImpactPower = 0.2f;

	// Token: 0x040009D7 RID: 2519
	public float publicEliteIdeologicalDistanceCohesionMultiplier = 2f;

	// Token: 0x040009D8 RID: 2520
	public float publicOpinionDispersionCohesionMultiplier = 6f;

	// Token: 0x040009D9 RID: 2521
	public float controlPointCountScaling = 0.18f;

	// Token: 0x040009DA RID: 2522
	public float controlPointScalingDivisor = 1.09f;

	// Token: 0x040009DB RID: 2523
	public float controlPointIPScaling = 0.35f;

	// Token: 0x040009DC RID: 2524
	public float controlPointIPFactor = 1f;

	// Token: 0x040009DD RID: 2525
	public float controlPointCostScaling = 0.6f;

	// Token: 0x040009DE RID: 2526
	public float controlPointMaintenanceDivisor = 2f;

	// Token: 0x040009DF RID: 2527
	public float populationBasedIPEffectScaling = -0.35f;

	// Token: 0x040009E0 RID: 2528
	public float coreMineralBuildMilitaryModifier = 0.05f;

	// Token: 0x040009E1 RID: 2529
	public float federationGDPEconomyBonus = 0.01f;

	// Token: 0x040009E2 RID: 2530
	public float fedLeaderDemocracyScoreToLeaveFederationFreely = 4f;

	// Token: 0x040009E3 RID: 2531
	public float coreEcoRegionGDPModifier = 1.25f;

	// Token: 0x040009E4 RID: 2532
	public float coreResourceRegionGDPModifier = 1.25f;

	// Token: 0x040009E5 RID: 2533
	public float colonyRegionGDPModifier = 0.5f;

	// Token: 0x040009E6 RID: 2534
	public float minMilitaryTechLevel = 2f;

	// Token: 0x040009E7 RID: 2535
	public int minControlPointsForNavy = 4;

	// Token: 0x040009E8 RID: 2536
	public int minControlPointsForNavyException = 3;

	// Token: 0x040009E9 RID: 2537
	public float PCGDPForNavyException = 40000f;

	// Token: 0x040009EA RID: 2538
	public float minPopulationForFirstArmy_millions = 5f;

	// Token: 0x040009EB RID: 2539
	public float minPopulationForAdditionalArmiesPer_millions = 25f;

	// Token: 0x040009EC RID: 2540
	public float economyPriorityPerCapitaIncomeChange_base = 3f;

	// Token: 0x040009ED RID: 2541
	public float economyPriorityPerCapitaIncomeChange_perCoreEcoRegion = 1.5f;

	// Token: 0x040009EE RID: 2542
	public float economyPriorityPerCapitaIncomeChange_perResourceRegion = 1.5f;

	// Token: 0x040009EF RID: 2543
	public float economyPriorityInequalityIncrease = 0.00015f;

	// Token: 0x040009F0 RID: 2544
	public float economyPriorityInequalityIncrease_perResourceRegion = 0.0001f;

	// Token: 0x040009F1 RID: 2545
	public float welfarePriorityInequalityChange = -0.005f;

	// Token: 0x040009F2 RID: 2546
	public float environmentPrioritySustainabilityChange = -0.005f;

	// Token: 0x040009F3 RID: 2547
	public float knowledgePriorityEducationIncrease = 0.005f;

	// Token: 0x040009F4 RID: 2548
	public float governmentPriorityDemocracyIncrease = 0.01f;

	// Token: 0x040009F5 RID: 2549
	public float militaryPriorityMiltechIncrease = 0.00125f;

	// Token: 0x040009F6 RID: 2550
	public float oppressionPriorityUnrestMultiplier = 3f;

	// Token: 0x040009F7 RID: 2551
	public float oppressionPriorityDemocracyDecrease = -0.0025f;

	// Token: 0x040009F8 RID: 2552
	public float conditionalOppressionPriorityCohesionDecrease = -0.025f;

	// Token: 0x040009F9 RID: 2553
	public float boostPriorityIncreaseAtEquator = 4f;

	// Token: 0x040009FA RID: 2554
	public float boostLatitudeDivisor = 25f;

	// Token: 0x040009FB RID: 2555
	public float fundingPriorityBaseIncomeIncrease = 10f;

	// Token: 0x040009FC RID: 2556
	public float spoilsPriorityMoneyPerInvestmentPoint = 5f;

	// Token: 0x040009FD RID: 2557
	public float spoilsPriorityMoneyPerResourceRegion = 5f;

	// Token: 0x040009FE RID: 2558
	public float spoilsDemocracyMoneyModifier = 2.5f;

	// Token: 0x040009FF RID: 2559
	public float spoilsPriorityBaseInequalityChange = 0.0025f;

	// Token: 0x04000A00 RID: 2560
	public float spoilsPriorityInequalityChange_perResourceRegion = 0.0015f;

	// Token: 0x04000A01 RID: 2561
	public float spoilsPriorityDemocracyChange = -0.0005f;

	// Token: 0x04000A02 RID: 2562
	public float spoilsPrioritySustainabilityChange = 0.0005f;

	// Token: 0x04000A03 RID: 2563
	public float spoilsPrioritySustainabilityChange_perResourceRegion = 0.00075f;

	// Token: 0x04000A04 RID: 2564
	public float spoilsPriorityPublicOpinionScaling = -0.15f;

	// Token: 0x04000A05 RID: 2565
	public float unityPublicOpinionBaseStrength = 5f;

	// Token: 0x04000A06 RID: 2566
	public float unityPriorityEducationChange = -0.001f;

	// Token: 0x04000A07 RID: 2567
	public float unityBaseCohesionChange = 0.1f;

	// Token: 0x04000A08 RID: 2568
	public float unityMinCohesionChange = 0.025f;

	// Token: 0x04000A09 RID: 2569
	public float DI_baseInvestmentPointCost_Influence = -1f;

	// Token: 0x04000A0A RID: 2570
	public float DI_perControlPointIPMultiplier_Influence = 3f;

	// Token: 0x04000A0B RID: 2571
	public float maxInvestmentPointDiscountfromControlPoints = 0.5f;

	// Token: 0x04000A0C RID: 2572
	public float daysOfFreeDirectInvestAfterRegimeChange = 365.25f;

	// Token: 0x04000A0D RID: 2573
	public float nationalDirectInvestmentCapGlobalMultiplier = 1f;

	// Token: 0x04000A0E RID: 2574
	public float LEOHabModulePriorityBonusCap = 0.3f;

	// Token: 0x04000A0F RID: 2575
	public float minGDPFracIncreaseFromFederation = 0.01f;

	// Token: 0x04000A10 RID: 2576
	public float maxGDPFracIncreaseFromFederation = 0.05f;

	// Token: 0x04000A11 RID: 2577
	public float minInequalityIncreaseFromFederation;

	// Token: 0x04000A12 RID: 2578
	public float maxInequalityIncreaseFromFederation = 0.05f;

	// Token: 0x04000A13 RID: 2579
	public bool prohibitCapitalShenanigans = true;

	// Token: 0x04000A14 RID: 2580
	public float inequalityHitFromResourceOrColonyAnnexation = 0.25f;

	// Token: 0x04000A15 RID: 2581
	public float cohesionHitFromRegionAnnexation = -0.25f;

	// Token: 0x04000A16 RID: 2582
	public float corporationsOrgMoneyDiscount = 0.8f;

	// Token: 0x04000A17 RID: 2583
	public float tradeUnionsOrgInfluenceDiscount = 0.8f;

	// Token: 0x04000A18 RID: 2584
	public float aristoracySpoilsMult = 1.2f;

	// Token: 0x04000A19 RID: 2585
	public float defenseSectorArmyBuff = 0.1f;

	// Token: 0x04000A1A RID: 2586
	public int religionUnityPublicOpinionBonusStrength = 3;

	// Token: 0x04000A1B RID: 2587
	public float extractiveSpoilsBonusPerResourceRegion = 2f;

	// Token: 0x04000A1C RID: 2588
	public float defenseSectorHealBonus = 0.005f;

	// Token: 0x04000A1D RID: 2589
	public float financialSectorFundingBonus = 1.05f;

	// Token: 0x04000A1E RID: 2590
	public float knowledgeSectorResearchBonus = 1.05f;

	// Token: 0x04000A1F RID: 2591
	public float globalEnergyCrisisBaseGDPLoss = -0.07f;

	// Token: 0x04000A20 RID: 2592
	public float globalEnergyCrisisBaseInequalityGain = 1.5f;

	// Token: 0x04000A21 RID: 2593
	public float globalEnergyCrisisOilRegionGDPGain = 0.025f;

	// Token: 0x04000A22 RID: 2594
	public int improveRelationsCooldown_ImprovementDeclined_d = 60;

	// Token: 0x04000A23 RID: 2595
	public int improveRelationsCooldown_EndAlliance_d = 90;

	// Token: 0x04000A24 RID: 2596
	public int improveRelationsCooldown_FormRivalry_d = 90;

	// Token: 0x04000A25 RID: 2597
	public int newRivalryCohesionPenaltyWindow_d = 90;

	// Token: 0x04000A26 RID: 2598
	public int improveRelationsCooldown_LeaveFederation_d = 720;

	// Token: 0x04000A27 RID: 2599
	public int improveRelationsCooldown_Independence_d_amicable = 180;

	// Token: 0x04000A28 RID: 2600
	public int improveRelationsCooldown_Independence_d_nonAmicable = 720;

	// Token: 0x04000A29 RID: 2601
	public int improveRelationsCooldown_EndRivalry_d = 90;

	// Token: 0x04000A2A RID: 2602
	public int improveRelationsCooldown_FormAlliance_d = 90;

	// Token: 0x04000A2B RID: 2603
	public int improveRelationsCooldown_JoinFederation_d = 360;

	// Token: 0x04000A2C RID: 2604
	public float consolidateExecControl_d = 75f;

	// Token: 0x04000A2D RID: 2605
	public float consolidateExecControl_perCP = 15f;

	// Token: 0x04000A2E RID: 2606
	public float smallRegionDefinition_km2 = 150000f;

	// Token: 0x04000A2F RID: 2607
	public float looseNukeFromRevolutionChancePerNuke = 0.01f;

	// Token: 0x04000A30 RID: 2608
	public int selfDisableControlPointDuration_months = 6;

	// Token: 0x04000A31 RID: 2609
	public float maxArmyCombatBonusFromLEOHabs = 0.3f;

	// Token: 0x04000A32 RID: 2610
	public float baseCohesionLossWhenDeclaringWarOnNewRival = 5f;

	// Token: 0x04000A33 RID: 2611
	public float maxCohesionLossWhenDeclaringWarOnRival = 10f;

	// Token: 0x04000A34 RID: 2612
	public float cohesionGainFromDeclaringWarOnOldRival = 1f;

	// Token: 0x04000A35 RID: 2613
	public float cohesionGainFromBeingTargetOfWar = 3f;

	// Token: 0x04000A36 RID: 2614
	public float cohesionGainFromAnsweringAllyCallToDefensiveWar = 1f;

	// Token: 0x04000A37 RID: 2615
	public float cohesionGainFromAnsweringAllyCallToOffensiveWar = 0.5f;

	// Token: 0x04000A38 RID: 2616
	public float basePassiveDemocracyIncreaseFromNeighbor = 0.005f;

	// Token: 0x04000A39 RID: 2617
	public float SpoCO2_ppm = 0.0006124f;

	// Token: 0x04000A3A RID: 2618
	public float SpoCH4_ppm = 8.32E-05f;

	// Token: 0x04000A3B RID: 2619
	public float SpoN2O_ppm = 2E-05f;

	// Token: 0x04000A3C RID: 2620
	public float SpoResCO2_ppm = 0.0003062f;

	// Token: 0x04000A3D RID: 2621
	public float SpoResCH4_ppm = 4.16E-05f;

	// Token: 0x04000A3E RID: 2622
	public float SpoResN2O_ppm = 1E-05f;

	// Token: 0x04000A3F RID: 2623
	public float WelCO2_ppm = -0.000325f;

	// Token: 0x04000A40 RID: 2624
	public float WelCH4_ppm = -2.5E-06f;

	// Token: 0x04000A41 RID: 2625
	public float WelN2O_ppm = -2.5E-06f;

	// Token: 0x04000A42 RID: 2626
	public float occupationSpeed = 1f;

	// Token: 0x04000A43 RID: 2627
	public float battleDamageEffectivenessFactor = 0.5f;

	// Token: 0x04000A44 RID: 2628
	public float localDefensesDamageEffectivenessFactor = 0.33333334f;

	// Token: 0x04000A45 RID: 2629
	public float ruggedTerrainDefenseBonus = 0.2f;

	// Token: 0x04000A46 RID: 2630
	public float coreEconomicRegionDefenseBonus = 0.1f;

	// Token: 0x04000A47 RID: 2631
	public float baseRegionDefenseBonus = 0.1f;

	// Token: 0x04000A48 RID: 2632
	public float armyRegionDefenseBonus = 0.2f;

	// Token: 0x04000A49 RID: 2633
	public float armyCrackdownMalus = 0.2f;

	// Token: 0x04000A4A RID: 2634
	public float adjacentFriendlyForcesRegionMiltechMultiplier = 0.1f;

	// Token: 0x04000A4B RID: 2635
	public float defenseCohesionMultiplier = 0.05f;

	// Token: 0x04000A4C RID: 2636
	public float defenseUnrestMultiplier = -0.05f;

	// Token: 0x04000A4D RID: 2637
	public float armyStrengthToLiberate = 0.8f;

	// Token: 0x04000A4E RID: 2638
	public string SuezCanalRegion = "map_Egypt";

	// Token: 0x04000A4F RID: 2639
	public string PanamaCanalRegion = "map_Panama";

	// Token: 0x04000A50 RID: 2640
	public string TurkishStraitsRegion = "map_Istanbul";

	// Token: 0x04000A51 RID: 2641
	public float habDefensesPDDPSMultiplier = 60f;

	// Token: 0x04000A52 RID: 2642
	public float regionDefensesPDAMultiplier_Self = 30f;

	// Token: 0x04000A53 RID: 2643
	public float regionDefensesPDAMultiplier_Region = 10f;

	// Token: 0x04000A54 RID: 2644
	public float first20ExtraProjectBonusPct = 0.05f;

	// Token: 0x04000A55 RID: 2645
	public float second20ExtraProjectBonusPct = 0.03f;

	// Token: 0x04000A56 RID: 2646
	public float overageExtraProjectBonusPct = 0.01f;

	// Token: 0x04000A57 RID: 2647
	public float researchBonusPerSlotInUse = 0.05f;

	// Token: 0x04000A58 RID: 2648
	public float categoryBonusPenaltyPerExtraSlot = 0.9f;

	// Token: 0x04000A59 RID: 2649
	public float passiveTechInvestment_C = 0.5f;

	// Token: 0x04000A5A RID: 2650
	public float passiveTechInvestment_N = 0.25f;

	// Token: 0x04000A5B RID: 2651
	public float passiveTechInvestment_V = 0.15f;

	// Token: 0x04000A5C RID: 2652
	public float passiveTechInvestment_B = 0.05f;

	// Token: 0x04000A5D RID: 2653
	public float activeTechInvestment_C;

	// Token: 0x04000A5E RID: 2654
	public float activeTechInvestment_N = 0.15f;

	// Token: 0x04000A5F RID: 2655
	public float activeTechInvestment_V = 0.25f;

	// Token: 0x04000A60 RID: 2656
	public float activeTechInvestment_B = 0.25f;

	// Token: 0x04000A61 RID: 2657
	public int initialMaxAllowedResourceSteps = 7;

	// Token: 0x04000A62 RID: 2658
	public float TIMissionModifier_TargetNationGDP_Multiplier = 1f;

	// Token: 0x04000A63 RID: 2659
	public float TIMissionModifier_DisabledControlPoint = -10f;

	// Token: 0x04000A64 RID: 2660
	public float TIMissionModifier_AdditionalDisabledControlPoints = -5f;

	// Token: 0x04000A65 RID: 2661
	public float TIMissionModifier_DefendedAsset = 10f;

	// Token: 0x04000A66 RID: 2662
	public float TIMissionModifier_AliensRemoved_Scaling = 0.1f;

	// Token: 0x04000A67 RID: 2663
	public float TIMissionModifier_NationalRivalries_Multiplier = 0.33333334f;

	// Token: 0x04000A68 RID: 2664
	public float TIMissionModifier_ControlPointOverage_Multiplier = 0.33333334f;

	// Token: 0x04000A69 RID: 2665
	public float TIMissionModifier_DefendedAssetConditionalAliens = 5f;

	// Token: 0x04000A6A RID: 2666
	public float TIMissionModifier_NationEconomyPower = 0.33333334f;

	// Token: 0x04000A6B RID: 2667
	public float TIMissionModifier_OrgDefenses = 4f;

	// Token: 0x04000A6C RID: 2668
	public float TIMissionModifier_NationalIndustries = 3f;

	// Token: 0x04000A6D RID: 2669
	public float maxValueFromAttackerAdjacentControlPoints = 6f;

	// Token: 0x04000A6E RID: 2670
	public float MaxSabotageProjectRPDamage = 5000f;

	// Token: 0x04000A6F RID: 2671
	public float MaxSabotageProjectAccumulatedHit = 0.5f;

	// Token: 0x04000A70 RID: 2672
	public float missionMoneyMultiplier = 10f;

	// Token: 0x04000A71 RID: 2673
	public float basePropagandaStrength = 10f;

	// Token: 0x04000A72 RID: 2674
	public float maxLEOHabPropagandaStrengthBonus = 9f;

	// Token: 0x04000A73 RID: 2675
	public float exoticsFromAlienFacilityRaid = 3f;

	// Token: 0x04000A74 RID: 2676
	public float abductionsCancelledFactorOnFacilityAssault = 0.2f;

	// Token: 0x04000A75 RID: 2677
	public float maxAbductionMissionImpact = 60f;

	// Token: 0x04000A76 RID: 2678
	public float enthrallElitesBySizeMultiplier = 1f;

	// Token: 0x04000A77 RID: 2679
	public int defendInterestPerCPDuration_days = 90;

	// Token: 0x04000A78 RID: 2680
	public int defendInterestDistributableDuration_days = 360;

	// Token: 0x04000A79 RID: 2681
	public string alienNationDataName = "ALN";

	// Token: 0x04000A7A RID: 2682
	public string alienMasterProject = "Project_AlienMasterProject";

	// Token: 0x04000A7B RID: 2683
	public string alienAdvancedMasterProject = "Project_AlienAdvancedMasterProject";

	// Token: 0x04000A7C RID: 2684
	public int globalAbductionsThreshhold_Higher = 500;

	// Token: 0x04000A7D RID: 2685
	public int globalAbductionsThreshhold_Lower = 6;

	// Token: 0x04000A7E RID: 2686
	public float influenceGainFromAbductions = 1f;

	// Token: 0x04000A7F RID: 2687
	public float moneyGainFromAbductions_Success = 0.1f;

	// Token: 0x04000A80 RID: 2688
	public float moneyGainFromAbductions_CriticalSuccess = 5f;

	// Token: 0x04000A81 RID: 2689
	public float influenceGainFromEnthrallPublic = 1f;

	// Token: 0x04000A82 RID: 2690
	public float moneyGainFromEnthrallPublic_Success = 5f;

	// Token: 0x04000A83 RID: 2691
	public float moneyGainFromEnthrallPublic_CriticalSuccess = 20f;

	// Token: 0x04000A84 RID: 2692
	public int daysToFieldArmyFromUFO = 32;

	// Token: 0x04000A85 RID: 2693
	public int daysToPrepareFullArmyFromUFO = 1;

	// Token: 0x04000A86 RID: 2694
	public float alienArmyTechCap = 9f;

	// Token: 0x04000A87 RID: 2695
	public float alienArmyTechLevel = 6.75f;

	// Token: 0x04000A88 RID: 2696
	public float alienArmyTechFromAbductions = 0.0002f;

	// Token: 0x04000A89 RID: 2697
	public int alienArmiesFromLanding = 3;

	// Token: 0x04000A8A RID: 2698
	public int AI_invaderArmiesLostBeforeBuildup = 3;

	// Token: 0x04000A8B RID: 2699
	public int extraYearsToDelayAlienInvasion_C = 16;

	// Token: 0x04000A8C RID: 2700
	public int extraYearsToDelayAlienInvasion_N = 12;

	// Token: 0x04000A8D RID: 2701
	public int extraYearsToDelayAlienInvasion_V = 6;

	// Token: 0x04000A8E RID: 2702
	public int extraYearsToDelayAlienInvasion_B;

	// Token: 0x04000A8F RID: 2703
	public int yearsBeforeAlienTotalWarAllowed_C = 25;

	// Token: 0x04000A90 RID: 2704
	public int yearsBeforeAlienTotalWarAllowed_N = 20;

	// Token: 0x04000A91 RID: 2705
	public int yearsBeforeAlienTotalWarAllowed_V = 12;

	// Token: 0x04000A92 RID: 2706
	public int yearsBeforeAlienTotalWarAllowed_B;

	// Token: 0x04000A93 RID: 2707
	public int yearsBeforeAlienAdvancedTech_C = 35;

	// Token: 0x04000A94 RID: 2708
	public int yearsBeforeAlienAdvancedTech_N = 25;

	// Token: 0x04000A95 RID: 2709
	public int yearsBeforeAlienAdvancedTech_V = 16;

	// Token: 0x04000A96 RID: 2710
	public int yearsBeforeAlienAdvancedTech_B = 10;

	// Token: 0x04000A97 RID: 2711
	public bool useAlternateTriggersForAlienAdvancedTech_C;

	// Token: 0x04000A98 RID: 2712
	public bool useAlternateTriggersForAlienAdvancedTech_N;

	// Token: 0x04000A99 RID: 2713
	public bool useAlternateTriggersForAlienAdvancedTech_V = true;

	// Token: 0x04000A9A RID: 2714
	public bool useAlternateTriggersForAlienAdvancedTech_B = true;

	// Token: 0x04000A9B RID: 2715
	public int yearsBeforeAlienInnerSystemExoticAttacks_C = 25;

	// Token: 0x04000A9C RID: 2716
	public int yearsBeforeAlienInnerSystemExoticAttacks_N = 16;

	// Token: 0x04000A9D RID: 2717
	public int yearsBeforeAlienInnerSystemExoticAttacks_V = 10;

	// Token: 0x04000A9E RID: 2718
	public int yearsBeforeAlienInnerSystemExoticAttacks_B;

	// Token: 0x04000A9F RID: 2719
	public int yearsBeforeInnerSystemOffensives_C = 25;

	// Token: 0x04000AA0 RID: 2720
	public int yearsBeforeInnerSystemOffensives_N = 20;

	// Token: 0x04000AA1 RID: 2721
	public int yearsBeforeInnerSystemOffensives_V = 15;

	// Token: 0x04000AA2 RID: 2722
	public int yearsBeforeInnerSystemOffensives_B = 10;

	// Token: 0x04000AA3 RID: 2723
	public float steadyAlienHateGainModifier_C;

	// Token: 0x04000AA4 RID: 2724
	public float steadyAlienHateGainModifier_N = 1f;

	// Token: 0x04000AA5 RID: 2725
	public float steadyAlienHateGainModifier_V = 1.9f;

	// Token: 0x04000AA6 RID: 2726
	public float steadyAlienHateGainModifier_B = 3.3f;

	// Token: 0x04000AA7 RID: 2727
	public bool alienReducedWarAttacks_C = true;

	// Token: 0x04000AA8 RID: 2728
	public bool alienReducedWarAttacks_N = true;

	// Token: 0x04000AA9 RID: 2729
	public bool alienReducedWarAttacks_V;

	// Token: 0x04000AAA RID: 2730
	public bool alienReducedWarAttacks_B;

	// Token: 0x04000AAB RID: 2731
	public int alienMaxExtraWarAttacks_C;

	// Token: 0x04000AAC RID: 2732
	public int alienMaxExtraWarAttacks_N = 2;

	// Token: 0x04000AAD RID: 2733
	public int alienMaxExtraWarAttacks_V = 4;

	// Token: 0x04000AAE RID: 2734
	public int alienMaxExtraWarAttacks_B = 8;

	// Token: 0x04000AAF RID: 2735
	public float alienStartingHateMaxmum_C = 70f;

	// Token: 0x04000AB0 RID: 2736
	public float alienStartingHateMaxmum_N = 1000f;

	// Token: 0x04000AB1 RID: 2737
	public float alienStartingHateMaxmum_V = 1000f;

	// Token: 0x04000AB2 RID: 2738
	public float alienStartingHateMaxmum_B = 1000f;

	// Token: 0x04000AB3 RID: 2739
	public float alienHateMaximumIncreasePerYear_C = 2f;

	// Token: 0x04000AB4 RID: 2740
	public float alienHateMaximumIncreasePerYear_N = 100f;

	// Token: 0x04000AB5 RID: 2741
	public float alienHateMaximumIncreasePerYear_V = 100f;

	// Token: 0x04000AB6 RID: 2742
	public float alienHateMaximumIncreasePerYear_B = 100f;

	// Token: 0x04000AB7 RID: 2743
	public float alienCallOffWarAttacksThreshold_C = 0.9f;

	// Token: 0x04000AB8 RID: 2744
	public float alienCallOffWarAttacksThreshold_N = 0.8f;

	// Token: 0x04000AB9 RID: 2745
	public float alienCallOffWarAttacksThreshold_V = 0.5f;

	// Token: 0x04000ABA RID: 2746
	public float alienCallOffWarAttacksThreshold_B;

	// Token: 0x04000ABB RID: 2747
	public float alienHateReprieveAfterKnockdown_C = 0.5f;

	// Token: 0x04000ABC RID: 2748
	public float alienHateReprieveAfterKnockdown_N = 0.35f;

	// Token: 0x04000ABD RID: 2749
	public float alienHateReprieveAfterKnockdown_V = 0.15f;

	// Token: 0x04000ABE RID: 2750
	public float alienHateReprieveAfterKnockdown_B;

	// Token: 0x04000ABF RID: 2751
	public float TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_C = 0.05f;

	// Token: 0x04000AC0 RID: 2752
	public float TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_N = 0.1f;

	// Token: 0x04000AC1 RID: 2753
	public float TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_V = 0.15f;

	// Token: 0x04000AC2 RID: 2754
	public float TIMissionModifier_XenoformingAttributeBonus_DifficultyScaling_B = 0.2f;

	// Token: 0x04000AC3 RID: 2755
	public float TIMissionModifier_AbductionValueScaling_C = 0.05f;

	// Token: 0x04000AC4 RID: 2756
	public float TIMissionModifier_AbductionValueScaling_N = 0.15f;

	// Token: 0x04000AC5 RID: 2757
	public float TIMissionModifier_AbductionValueScaling_V = 0.35f;

	// Token: 0x04000AC6 RID: 2758
	public float TIMissionModifier_AbductionValueScaling_B = 0.5f;

	// Token: 0x04000AC7 RID: 2759
	public int maxAlienBaseGoals_C = 10;

	// Token: 0x04000AC8 RID: 2760
	public int maxAlienBaseGoals_N = 12;

	// Token: 0x04000AC9 RID: 2761
	public int maxAlienBaseGoals_V = 18;

	// Token: 0x04000ACA RID: 2762
	public int maxAlienBaseGoals_B = 25;

	// Token: 0x04000ACB RID: 2763
	public int extraMaxAlienBaseGoals_TotalWarEra_C;

	// Token: 0x04000ACC RID: 2764
	public int extraMaxAlienBaseGoals_TotalWarEra_N = 3;

	// Token: 0x04000ACD RID: 2765
	public int extraMaxAlienBaseGoals_TotalWarEra_V = 7;

	// Token: 0x04000ACE RID: 2766
	public int extraMaxAlienBaseGoals_TotalWarEra_B = 10;

	// Token: 0x04000ACF RID: 2767
	public float AI_AlienHatePerMCUtilitizedMultiplier_C = 0.05f;

	// Token: 0x04000AD0 RID: 2768
	public float AI_AlienHatePerMCUtilitizedMultiplier_N = 0.3f;

	// Token: 0x04000AD1 RID: 2769
	public float AI_AlienHatePerMCUtilitizedMultiplier_V = 0.6f;

	// Token: 0x04000AD2 RID: 2770
	public float AI_AlienHatePerMCUtilitizedMultiplier_B = 1f;

	// Token: 0x04000AD3 RID: 2771
	public float AI_MissionAttackerBonus_C = -3f;

	// Token: 0x04000AD4 RID: 2772
	public float AI_MissionAttackerBonus_N;

	// Token: 0x04000AD5 RID: 2773
	public float AI_MissionAttackerBonus_V = 1f;

	// Token: 0x04000AD6 RID: 2774
	public float AI_MissionAttackerBonus_B = 2f;

	// Token: 0x04000AD7 RID: 2775
	public float AI_MissionDefenderBonus_C = -3f;

	// Token: 0x04000AD8 RID: 2776
	public float AI_MissionDefenderBonus_N;

	// Token: 0x04000AD9 RID: 2777
	public float AI_MissionDefenderBonus_V = 1f;

	// Token: 0x04000ADA RID: 2778
	public float AI_MissionDefenderBonus_B = 2f;

	// Token: 0x04000ADB RID: 2779
	public float AI_GangUpOnLeaderMinimumIdeologicalDistance_C = 2.5f;

	// Token: 0x04000ADC RID: 2780
	public float AI_GangUpOnLeaderMinimumIdeologicalDistance_N = 1.8f;

	// Token: 0x04000ADD RID: 2781
	public float AI_GangUpOnLeaderMinimumIdeologicalDistance_V = 1.2f;

	// Token: 0x04000ADE RID: 2782
	public float AI_GangUpOnLeaderMinimumIdeologicalDistance_B;

	// Token: 0x04000ADF RID: 2783
	public float AI_BonusMissionControl_C;

	// Token: 0x04000AE0 RID: 2784
	public float AI_BonusMissionControl_N;

	// Token: 0x04000AE1 RID: 2785
	public float AI_BonusMissionControl_V = 20f;

	// Token: 0x04000AE2 RID: 2786
	public float AI_BonusMissionControl_B = 50f;

	// Token: 0x04000AE3 RID: 2787
	public float AI_BonusCPCap_C;

	// Token: 0x04000AE4 RID: 2788
	public float AI_BonusCPCap_N;

	// Token: 0x04000AE5 RID: 2789
	public float AI_BonusCPCap_V = 50f;

	// Token: 0x04000AE6 RID: 2790
	public float AI_BonusCPCap_B = 100f;

	// Token: 0x04000AE7 RID: 2791
	public float AI_AlienExoticMultiplier_C = 1f;

	// Token: 0x04000AE8 RID: 2792
	public float AI_AlienExoticMultiplier_N = 2f;

	// Token: 0x04000AE9 RID: 2793
	public float AI_AlienExoticMultiplier_V = 3f;

	// Token: 0x04000AEA RID: 2794
	public float AI_AlienExoticMultiplier_B = 4f;

	// Token: 0x04000AEB RID: 2795
	public float AI_AlienEarthFleetSizeModifier_C = 0.5f;

	// Token: 0x04000AEC RID: 2796
	public float AI_AlienEarthFleetSizeModifier_N = 1f;

	// Token: 0x04000AED RID: 2797
	public float AI_AlienEarthFleetSizeModifier_V = 1f;

	// Token: 0x04000AEE RID: 2798
	public float AI_AlienEarthFleetSizeModifier_B = 1f;

	// Token: 0x04000AEF RID: 2799
	public float AI_AlienEarthFleetExcessModifier_C;

	// Token: 0x04000AF0 RID: 2800
	public float AI_AlienEarthFleetExcessModifier_N = 0.6f;

	// Token: 0x04000AF1 RID: 2801
	public float AI_AlienEarthFleetExcessModifier_V = 1f;

	// Token: 0x04000AF2 RID: 2802
	public float AI_AlienEarthFleetExcessModifier_B = 1.25f;

	// Token: 0x04000AF3 RID: 2803
	public float AI_AlienSurveillanceDelayModifier_C = 1.2f;

	// Token: 0x04000AF4 RID: 2804
	public float AI_AlienSurveillanceDelayModifier_N = 1f;

	// Token: 0x04000AF5 RID: 2805
	public float AI_AlienSurveillanceDelayModifier_V = 0.85f;

	// Token: 0x04000AF6 RID: 2806
	public float AI_AlienSurveillanceDelayModifier_B = 0.75f;

	// Token: 0x04000AF7 RID: 2807
	public float AI_AlienQuiescence_C = 1.2f;

	// Token: 0x04000AF8 RID: 2808
	public float AI_AlienQuiescence_N = 1f;

	// Token: 0x04000AF9 RID: 2809
	public float AI_AlienQuiescence_V = 0.85f;

	// Token: 0x04000AFA RID: 2810
	public float AI_AlienQuiescence_B = 0.7f;

	// Token: 0x04000AFB RID: 2811
	public float AI_WormholeSetupSpeed_C = 1.5f;

	// Token: 0x04000AFC RID: 2812
	public float AI_WormholeSetupSpeed_N = 1f;

	// Token: 0x04000AFD RID: 2813
	public float AI_WormholeSetupSpeed_V = 0.75f;

	// Token: 0x04000AFE RID: 2814
	public float AI_WormholeSetupSpeed_B = 0.5f;

	// Token: 0x04000AFF RID: 2815
	public float Diff_ExoticsSalvageRate_C = 2f;

	// Token: 0x04000B00 RID: 2816
	public float Diff_ExoticsSalvageRate_N = 1f;

	// Token: 0x04000B01 RID: 2817
	public float Diff_ExoticsSalvageRate_V = 0.5f;

	// Token: 0x04000B02 RID: 2818
	public float Diff_ExoticsSalvageRate_B = 0.33333334f;

	// Token: 0x04000B03 RID: 2819
	public int minAbductionsinRegionForFacility = 15;

	// Token: 0x04000B04 RID: 2820
	public float monthlyChanceAbductionPerSurveillanceHabEye = 0.02f;

	// Token: 0x04000B05 RID: 2821
	public float increaseAlienMaxAttackFleetStrengthRatioOverTime_C = -1f;

	// Token: 0x04000B06 RID: 2822
	public float increaseAlienMaxAttackFleetStrengthRatioOverTime_N = 30f;

	// Token: 0x04000B07 RID: 2823
	public float increaseAlienMaxAttackFleetStrengthRatioOverTime_V = 25f;

	// Token: 0x04000B08 RID: 2824
	public float increaseAlienMaxAttackFleetStrengthRatioOverTime_B = 20f;

	// Token: 0x04000B09 RID: 2825
	public float initialMaxAlienAttackFleetStrengthRatio_C = 1.05f;

	// Token: 0x04000B0A RID: 2826
	public float initialMaxAlienAttackFleetStrengthRatio_N = 1.25f;

	// Token: 0x04000B0B RID: 2827
	public float initialMaxAlienAttackFleetStrengthRatio_V = 2f;

	// Token: 0x04000B0C RID: 2828
	public float initialMaxAlienAttackFleetStrengthRatio_B = 2.5f;

	// Token: 0x04000B0D RID: 2829
	public float yearsToDelayAlienMiddleColonization_C = 5f;

	// Token: 0x04000B0E RID: 2830
	public float yearsToDelayAlienMiddleColonization_N = 3.5f;

	// Token: 0x04000B0F RID: 2831
	public float yearsToDelayAlienMiddleColonization_V = 2.5f;

	// Token: 0x04000B10 RID: 2832
	public float yearsToDelayAlienMiddleColonization_B;

	// Token: 0x04000B11 RID: 2833
	public int alienNonPlanetaryOuterSystemColonizationLimit_C;

	// Token: 0x04000B12 RID: 2834
	public int alienNonPlanetaryOuterSystemColonizationLimit_N = 1;

	// Token: 0x04000B13 RID: 2835
	public int alienNonPlanetaryOuterSystemColonizationLimit_V = 2;

	// Token: 0x04000B14 RID: 2836
	public int alienNonPlanetaryOuterSystemColonizationLimit_B = 3;

	// Token: 0x04000B15 RID: 2837
	public string size5Project = "Project_ClandestineCells";

	// Token: 0x04000B16 RID: 2838
	public string size6Project = "Project_CovertOperations";

	// Token: 0x04000B17 RID: 2839
	public float intelToSeeNeutralPawn = 0.1f;

	// Token: 0x04000B18 RID: 2840
	public float intelToSeeCouncilorBasicData = 0.25f;

	// Token: 0x04000B19 RID: 2841
	public float intelToSeeCouncilorDetails = 0.5f;

	// Token: 0x04000B1A RID: 2842
	public float intelToSeeCouncilorMission = 0.75f;

	// Token: 0x04000B1B RID: 2843
	public float intelToSeeCouncilorSecrets = 1f;

	// Token: 0x04000B1C RID: 2844
	public float myCouncilorBaselineIntel = 0.75f;

	// Token: 0x04000B1D RID: 2845
	public float intelToSeeFactionBasicData = 0.25f;

	// Token: 0x04000B1E RID: 2846
	public float intelToSeeFactionObjectives = 0.5f;

	// Token: 0x04000B1F RID: 2847
	public float intelToSeeFactionProjects = 0.75f;

	// Token: 0x04000B20 RID: 2848
	public float intelToSeeFactionResources = 0.25f;

	// Token: 0x04000B21 RID: 2849
	public float intelToSeeFactionUnassignedOrgs = 0.25f;

	// Token: 0x04000B22 RID: 2850
	public float myFactionBaselineIntel = 1f;

	// Token: 0x04000B23 RID: 2851
	public float humanSpaceAssetBaselineIntel = 0.1f;

	// Token: 0x04000B24 RID: 2852
	public float humanMySpaceAssetBaselineIntel = 0.5f;

	// Token: 0x04000B25 RID: 2853
	public float alienSpaceAssetBaselineIntel;

	// Token: 0x04000B26 RID: 2854
	public float alienMySpaceAssetBaselineIntel = 1f;

	// Token: 0x04000B27 RID: 2855
	public float intelToSeeSpaceAssetLocationandComposition = 0.1f;

	// Token: 0x04000B28 RID: 2856
	public float intelToSeeFleetShipDetails = 0.5f;

	// Token: 0x04000B29 RID: 2857
	public float intelToSeeSpaceAssetUndercoverEnemyCouncilors = 0.8f;

	// Token: 0x04000B2A RID: 2858
	public float baselineAlienAssetDetectionRange_AU = 2f;

	// Token: 0x04000B2B RID: 2859
	public float totalSystemDetection_AU = 60f;

	// Token: 0x04000B2C RID: 2860
	public float factionHateForHabAssaultOperationPerTier = 8f;

	// Token: 0x04000B2D RID: 2861
	public float factionHateForHabDestructionOperationPerTier = 3f;

	// Token: 0x04000B2E RID: 2862
	public float factionHateMultiplierPerModuleDestroyedPerTier = 1f;

	// Token: 0x04000B2F RID: 2863
	public float factionHateForDestroyingArmyOutsideofWar = 30f;

	// Token: 0x04000B30 RID: 2864
	public float factionHateSIFactorPerShipDestroyed = 0.35f;

	// Token: 0x04000B31 RID: 2865
	public float factionHateForDeclaringWarCPMultiplier = 3f;

	// Token: 0x04000B32 RID: 2866
	public float factionHateForInitiatingBombardment_AnyTarget = 1f;

	// Token: 0x04000B33 RID: 2867
	public float factionHateForTrade = -1f;

	// Token: 0x04000B34 RID: 2868
	public float factionHateForTradeTreaty = -8f;

	// Token: 0x04000B35 RID: 2869
	public float factionHateConflictThreshold = 20f;

	// Token: 0x04000B36 RID: 2870
	public float factionHateWarThreshold = 50f;

	// Token: 0x04000B37 RID: 2871
	public float goodTradeThreshold = 0.3f;

	// Token: 0x04000B38 RID: 2872
	public float meaningfulTradeThreshold = 8000f;

	// Token: 0x04000B39 RID: 2873
	public int tradeAcceptanceTextVariants = 7;

	// Token: 0x04000B3A RID: 2874
	public float factionHateWarDeterminantDivisor = 100f;

	// Token: 0x04000B3B RID: 2875
	public float alienFactionHateWarValue = 50f;

	// Token: 0x04000B3C RID: 2876
	public float factionHateStealResources = 1f;

	// Token: 0x04000B3D RID: 2877
	public float minimumFleetStrength = 1f;

	// Token: 0x04000B3E RID: 2878
	public float minimumAssaultStrength = 1f;

	// Token: 0x04000B3F RID: 2879
	public float factionHateForBurnXenoforming;

	// Token: 0x04000B40 RID: 2880
	public float factionHateForDestroyLandedUFO = 80f;

	// Token: 0x04000B41 RID: 2881
	public float factionHateForDestroyAlienFacility = 10f;

	// Token: 0x04000B42 RID: 2882
	public float divisibleHateForDestroyingAlienNation = 80f;

	// Token: 0x04000B43 RID: 2883
	public int AI_BaseAllowedOverageCPMaintenance = 25;

	// Token: 0x04000B44 RID: 2884
	public float hateVariance = 0.2f;

	// Token: 0x04000B45 RID: 2885
	public float maxHumanAttackFleetStrengthRatio_C = 2f;

	// Token: 0x04000B46 RID: 2886
	public float maxHumanAttackFleetStrengthRatio_N = 2f;

	// Token: 0x04000B47 RID: 2887
	public float maxHumanAttackFleetStrengthRatio_V = 2.5f;

	// Token: 0x04000B48 RID: 2888
	public float maxHumanAttackFleetStrengthRatio_B = 2.5f;

	// Token: 0x04000B49 RID: 2889
	public float maxAttackFleetRatio_AllCases = 3.5f;

	// Token: 0x04000B4A RID: 2890
	public float hateBurnoffFromKillingHabmodulesDivisor_C = 1f;

	// Token: 0x04000B4B RID: 2891
	public float hateBurnoffFromKillingHabmodulesDivisor_N = 2f;

	// Token: 0x04000B4C RID: 2892
	public float hateBurnoffFromKillingHabmodulesDivisor_V = 3.5f;

	// Token: 0x04000B4D RID: 2893
	public float hateBurnoffFromKillingHabmodulesDivisor_B = 5f;

	// Token: 0x04000B4E RID: 2894
	public float AI_CouncilorStatChasingMultiplier_C = 0.5f;

	// Token: 0x04000B4F RID: 2895
	public float AI_CouncilorStatChasingMultiplier_N = 1f;

	// Token: 0x04000B50 RID: 2896
	public float AI_CouncilorStatChasingMultiplier_V = 1f;

	// Token: 0x04000B51 RID: 2897
	public float AI_CouncilorStatChasingMultiplier_B = 1f;

	// Token: 0x04000B52 RID: 2898
	public float spaceResourceToTons = 0.1f;

	// Token: 0x04000B53 RID: 2899
	public float crewWaterConsumptionTons_year = 3.5f;

	// Token: 0x04000B54 RID: 2900
	public float crewVolatilesConsumptionTons_year = 3.5f;

	// Token: 0x04000B55 RID: 2901
	public float crewSalary_year = 0.1f;

	// Token: 0x04000B56 RID: 2902
	public float crewBaselineWater_tons = 2f;

	// Token: 0x04000B57 RID: 2903
	public float crewBaselineVolatiles_tons = 2f;

	// Token: 0x04000B58 RID: 2904
	public float decomissionModuleRefundRate = 0.1f;

	// Token: 0x04000B59 RID: 2905
	public ulong colonizedSpaceObjectValue = 10000UL;

	// Token: 0x04000B5A RID: 2906
	public ulong populousSpaceObjectValue = 50000UL;

	// Token: 0x04000B5B RID: 2907
	public float innerMiddleBeltLine = 2.5f;

	// Token: 0x04000B5C RID: 2908
	public float middleOuterBeltLine = 2.82f;

	// Token: 0x04000B5D RID: 2909
	public float maxHabBoostFromEarthDuration_days = 750f;

	// Token: 0x04000B5E RID: 2910
	public float probeConstructionTime_d = 14f;

	// Token: 0x04000B5F RID: 2911
	public float probeMetalsPayloadMassFraction = 0.9f;

	// Token: 0x04000B60 RID: 2912
	public float probeVolatilesPayloadMassFraction;

	// Token: 0x04000B61 RID: 2913
	public float probeNoblesPayloadMassFraction = 0.09f;

	// Token: 0x04000B62 RID: 2914
	public float probeFissilesPayloadMassFraction = 0.01f;

	// Token: 0x04000B63 RID: 2915
	public float probeWaterPropellantMassFraction = 0.75f;

	// Token: 0x04000B64 RID: 2916
	public float probeVolatilesPropellantMassFraction = 0.25f;

	// Token: 0x04000B65 RID: 2917
	public float probePayloadBaseline_tons = 0.5f;

	// Token: 0x04000B66 RID: 2918
	public float probePayloadPerHabSite_tons = 0.5f;

	// Token: 0x04000B67 RID: 2919
	public double maxMassforMiningResourceMalus = 50000000000000000.0;

	// Token: 0x04000B68 RID: 2920
	public float metalsBonusDensityCutPoint = 3f;

	// Token: 0x04000B69 RID: 2921
	public float metalsMalusDensityCutPoint = 1.25f;

	// Token: 0x04000B6A RID: 2922
	public float initialWaterValue = 1f;

	// Token: 0x04000B6B RID: 2923
	public float initialVolatilesValue = 5f;

	// Token: 0x04000B6C RID: 2924
	public float initialMetalsValue = 10f;

	// Token: 0x04000B6D RID: 2925
	public float initialNobleMetalsValue = 50f;

	// Token: 0x04000B6E RID: 2926
	public float initialFissilesValue = 100f;

	// Token: 0x04000B6F RID: 2927
	public float initialAntimatterValue = 50000f;

	// Token: 0x04000B70 RID: 2928
	public float initialExoticsValue = 1500f;

	// Token: 0x04000B71 RID: 2929
	public float baseEarthSaleInefficiency = 0.05f;

	// Token: 0x04000B72 RID: 2930
	public float scuttlePerCrewMassCost = 0.1f;

	// Token: 0x04000B73 RID: 2931
	public float scuttleRefund = 0.25f;

	// Token: 0x04000B74 RID: 2932
	public float refitBuildTimeCap = 0.75f;

	// Token: 0x04000B75 RID: 2933
	public float smallShipyardPenaltyPowerPerTier = 1.5f;

	// Token: 0x04000B76 RID: 2934
	public float TIModifier_HumanAIShipBuildingScaling_C = 1f;

	// Token: 0x04000B77 RID: 2935
	public float TIModifier_HumanAIShipBuildingScaling_N = 1f;

	// Token: 0x04000B78 RID: 2936
	public float TIModifier_HumanAIShipBuildingScaling_V = 0.75f;

	// Token: 0x04000B79 RID: 2937
	public float TIModifier_HumanAIShipBuildingScaling_B = 0.5f;

	// Token: 0x04000B7A RID: 2938
	public float TIModifier_AlienAIShipBuildingScaling_C = 1.25f;

	// Token: 0x04000B7B RID: 2939
	public float TIModifier_AlienAIShipBuildingScaling_N = 1f;

	// Token: 0x04000B7C RID: 2940
	public float TIModifier_AlienAIShipBuildingScaling_V = 1f;

	// Token: 0x04000B7D RID: 2941
	public float TIModifier_AlienAIShipBuildingScaling_B = 0.75f;

	// Token: 0x04000B7E RID: 2942
	public float desiredSTOFighterWetMass_tons = 250f;

	// Token: 0x04000B7F RID: 2943
	public float AssaultValue_AlienArmy = 50f;

	// Token: 0x04000B80 RID: 2944
	public float extraStartingCombatDistance_km;

	// Token: 0x04000B81 RID: 2945
	public float influenceCostBaseForRammingSpeed = 200f;

	// Token: 0x04000B82 RID: 2946
	public float baselineMaxHumanCruiseAcceleration_g = 2f;

	// Token: 0x04000B83 RID: 2947
	public float baselineMaxHumanCombatAcceleration_g = 3f;

	// Token: 0x04000B84 RID: 2948
	public float maxAlienCruiseAcceleration_g = 2.5f;

	// Token: 0x04000B85 RID: 2949
	public float maxAlienCombatAcceleration_g = 4f;

	// Token: 0x04000B86 RID: 2950
	public float shipPartRepairBaseCostMultiplier = 0.25f;

	// Token: 0x04000B87 RID: 2951
	public float daysToRefuelAPropellantTank = 0.005f;

	// Token: 0x04000B88 RID: 2952
	public float daysToReloadAShipWeaponStep = 0.025f;

	// Token: 0x04000B89 RID: 2953
	public float daysToRepairSystem = 6f;

	// Token: 0x04000B8A RID: 2954
	public float daysToRepairPart = 3f;

	// Token: 0x04000B8B RID: 2955
	public float basicSalvageRecoveryCap = 0.25f;

	// Token: 0x04000B8C RID: 2956
	public float antimatterSalvageChance = 0.25f;

	// Token: 0x04000B8D RID: 2957
	public float exoticsSalvageRecoveryCap = 0.85f;

	// Token: 0x04000B8E RID: 2958
	public float DP_DestroyMissile = 0.15f;

	// Token: 0x04000B8F RID: 2959
	public float DP_FireAtMagRound = 0.5f;

	// Token: 0x04000B90 RID: 2960
	public int maxShipsAllowedInCombat = 90;

	// Token: 0x04000B91 RID: 2961
	public float ExoticsPerAlienHabTier = 3f;

	// Token: 0x04000B92 RID: 2962
	public float ECM_SecondsBollixedPerPointMissed = 5f;

	// Token: 0x04000B93 RID: 2963
	public float ECM_SecondsBollixedPerPointMissed_Missile = 10f;

	// Token: 0x04000B94 RID: 2964
	public float attackBonusPerTargetECMDefeat = 0.02f;

	// Token: 0x04000B95 RID: 2965
	public float highBombardmentAltitude_km = 600f;

	// Token: 0x04000B96 RID: 2966
	public float medBombardmentAltitude_km = 400f;

	// Token: 0x04000B97 RID: 2967
	public float lowBombardmentAltitude_km = 200f;

	// Token: 0x04000B98 RID: 2968
	public float armyBombardmentDamageDivisor_InBattle = 9000f;

	// Token: 0x04000B99 RID: 2969
	public float armyBombardmentDamageDivisor_Dispersed = 18000f;

	// Token: 0x04000B9A RID: 2970
	public float officerTransferCostPerRank = 10f;

	// Token: 0x04000B9B RID: 2971
	public bool alwaysFireAtSaturated;

	// Token: 0x04000B9C RID: 2972
	public bool logAllDamageInCombat;

	// Token: 0x04000B9D RID: 2973
	public float duration_scaling_divisor = 2.5f;

	// Token: 0x04000B9E RID: 2974
	public int randomEventsPerMonthVariability = 2;

	// Token: 0x04000B9F RID: 2975
	public int maxRandomEventsPerMonth = 10;

	// Token: 0x04000BA0 RID: 2976
	public float notificationReceiveInputDelay = 0.5f;

	// Token: 0x04000BA1 RID: 2977
	public bool importAllAIShipDesignsInSkirmish;

	// Token: 0x04000BA2 RID: 2978
	public string pathNoCouncilFlag = "faction_logos/NoCouncilFlag";

	// Token: 0x04000BA3 RID: 2979
	public string pathCircle = "icons_2d/circle";

	// Token: 0x04000BA4 RID: 2980
	public string pathEmptyControlPoint = "icons_2d/ICO_ControlPoint_empty";

	// Token: 0x04000BA5 RID: 2981
	public string pathNavalArmyIcon = "icons_2d/ICO_naval_army";

	// Token: 0x04000BA6 RID: 2982
	public string pathNoNavyMovementIcon = "icons_2d/ICO_no_naval_mov";

	// Token: 0x04000BA7 RID: 2983
	public string pathWarIcon = "icons_2d/ICO_nation_at_war";

	// Token: 0x04000BA8 RID: 2984
	public string pathPeaceIcon = "icons_2d/ICO_nation_at_peace";

	// Token: 0x04000BA9 RID: 2985
	public string pathWaterIcon = "icons_2d/ICO_water";

	// Token: 0x04000BAA RID: 2986
	public string pathVolatilesIcon = "icons_2d/ICO_volatiles";

	// Token: 0x04000BAB RID: 2987
	public string pathBaseMetalsIcon = "icons_2d/ICO_metal";

	// Token: 0x04000BAC RID: 2988
	public string pathNobleMetalsIcon = "icons_2d/ICO_metal_noble";

	// Token: 0x04000BAD RID: 2989
	public string pathFissilesIcon = "icons_2d/ICO_fissile";

	// Token: 0x04000BAE RID: 2990
	public string pathAntimatterIcon = "icons_2d/ICO_antimatter";

	// Token: 0x04000BAF RID: 2991
	public string pathExoticsIcon = "icons_2d/ICO_exotics";

	// Token: 0x04000BB0 RID: 2992
	public string pathProjectsIcon = "icons_2d/ICO_projects";

	// Token: 0x04000BB1 RID: 2993
	public string pathMoneyIcon = "icons_2d/ICO_currency";

	// Token: 0x04000BB2 RID: 2994
	public string pathInfluenceIcon = "icons_2d/ICO_influence";

	// Token: 0x04000BB3 RID: 2995
	public string pathOpsIcon = "icons_2d/ICO_ops";

	// Token: 0x04000BB4 RID: 2996
	public string pathResearchIcon = "icons_2d/ICO_research";

	// Token: 0x04000BB5 RID: 2997
	public string pathBoostIcon = "icons_2d/ICO_boost";

	// Token: 0x04000BB6 RID: 2998
	public string pathMissionControlIcon = "icons_2d/ICO_mission_control";

	// Token: 0x04000BB7 RID: 2999
	public string pathNukesIcon = "icons_2d/ICO_nukes";

	// Token: 0x04000BB8 RID: 3000
	public string pathHabPowerIcon = "icons_2d/ICO_hab_power";

	// Token: 0x04000BB9 RID: 3001
	public string pathHabPowerAlertIcon = "icons_2d/ICO_hab_power_alert";

	// Token: 0x04000BBA RID: 3002
	public string pathHabResupplyIcon = "icons_2d/ICO_supply";

	// Token: 0x04000BBB RID: 3003
	public string pathHabShipyardIcon = "icons_2d/ICO_construction_shipyard";

	// Token: 0x04000BBC RID: 3004
	public string pathHabModuleConstructionIcon = "icons_2d/ICO_construction_module";

	// Token: 0x04000BBD RID: 3005
	public string pathHabDefenseIcon = "icons_2d/ICO_combat_score";

	// Token: 0x04000BBE RID: 3006
	public string pathUnderConstructionIcon = "icons_2d/ICO_under_construction";

	// Token: 0x04000BBF RID: 3007
	public string pathArmyStrengthIcon = "icons_2d/ICO_army_strength";

	// Token: 0x04000BC0 RID: 3008
	public string pathOccupationIcon = "icons_2d/ICO_region_occupation";

	// Token: 0x04000BC1 RID: 3009
	public string pathArmyCombatIcon = "icons_2d/ICO_army_battle";

	// Token: 0x04000BC2 RID: 3010
	public string pathFleetInTransitIcon = "icons_2d/ICO_fleet_in_transit";

	// Token: 0x04000BC3 RID: 3011
	public string pathFleetCombatIcon = "icons_2d/ICO_pending_combat";

	// Token: 0x04000BC4 RID: 3012
	public string pathFleetIcon = "icons_2d/ICO_fleet";

	// Token: 0x04000BC5 RID: 3013
	public string pathWarningIcon = "ui/ICO_warning";

	// Token: 0x04000BC6 RID: 3014
	public string pathOrbitIcon = "icons_2d/ICO_radius_of_orbit";

	// Token: 0x04000BC7 RID: 3015
	public string pathProspectedHabSite = "mapicons/ICO_geoscape_habSite_prospected";

	// Token: 0x04000BC8 RID: 3016
	public string pathNotProspectedHabSite = "mapicons/ICO_geoscape_habSite_notProspected";

	// Token: 0x04000BC9 RID: 3017
	public string pathBeyondRangeHabSite = "mapicons/ICO_geoscape_habSite_beyondRange";

	// Token: 0x04000BCA RID: 3018
	public string pathSpaceCombatScoreIcon = "icons_2d/ICO_combat_score";

	// Token: 0x04000BCB RID: 3019
	public string pathColonyIcon = "icons_2d/ICO_colony";

	// Token: 0x04000BCC RID: 3020
	public string pathUnrestIcon = "icons_2d/ICO_nation_unrest";

	// Token: 0x04000BCD RID: 3021
	public string pathSpaceAssaultScoreIcon = "icons_2d/ICO_space_assault_score";

	// Token: 0x04000BCE RID: 3022
	public string pathSpaceMiningIcon = "icons_2d/ICO_hab_mining";

	// Token: 0x04000BCF RID: 3023
	public string pathSTOFighter = "icons_2d/ICO_OrbitalFighter";

	// Token: 0x04000BD0 RID: 3024
	public string pathSunStylized = "icons_2d/ICO_SunStylized";

	// Token: 0x04000BD1 RID: 3025
	public string pathUndecidedGradient = "ui/UndecidedGradient";

	// Token: 0x04000BD2 RID: 3026
	public string ECO_IconPath = "icons_2d/ICO_economy_priority";

	// Token: 0x04000BD3 RID: 3027
	public string WEL_IconPath = "icons_2d/ICO_welfare_priority";

	// Token: 0x04000BD4 RID: 3028
	public string KNO_IconPath = "icons_2d/ICO_knowledge_priority";

	// Token: 0x04000BD5 RID: 3029
	public string UNI_IconPath = "icons_2d/ICO_unity_priority";

	// Token: 0x04000BD6 RID: 3030
	public string FMI_IconPath = "icons_2d/ICO_found_military_priority";

	// Token: 0x04000BD7 RID: 3031
	public string MIL_IconPath = "icons_2d/ICO_military_priority";

	// Token: 0x04000BD8 RID: 3032
	public string SPO_IconPath = "icons_2d/ICO_spoils_priority";

	// Token: 0x04000BD9 RID: 3033
	public string FLI_IconPath = "icons_2d/ICO_spaceflightProgram_priority";

	// Token: 0x04000BDA RID: 3034
	public string DEV_IconPath = "icons_2d/ICO_funding_priority";

	// Token: 0x04000BDB RID: 3035
	public string BOO_IconPath = "icons_2d/ICO_launchFacilities_Priority";

	// Token: 0x04000BDC RID: 3036
	public string MC_IconPath = "icons_2d/ICO_missionControl_priority";

	// Token: 0x04000BDD RID: 3037
	public string ARM_IconPath = "icons_2d/ICO_buildArmy_priority";

	// Token: 0x04000BDE RID: 3038
	public string NAV_IconPath = "icons_2d/ICO_buildNavy_priority";

	// Token: 0x04000BDF RID: 3039
	public string NUC_IconPath = "icons_2d/ICO_develop_atomic_bomb_priority";

	// Token: 0x04000BE0 RID: 3040
	public string NUK_IconPath = "icons_2d/ICO_buildNuclearWeapons_priority";

	// Token: 0x04000BE1 RID: 3041
	public string DEF_IconPath = "icons_2d/ICO_buildSpaceDefenses_priority";

	// Token: 0x04000BE2 RID: 3042
	public string GOV_IconPath = "icons_2d/ICO_government_priority";

	// Token: 0x04000BE3 RID: 3043
	public string SUB_IconPath = "icons_2d/ICO_submarines_priority";

	// Token: 0x04000BE4 RID: 3044
	public string ENV_IconPath = "icons_2d/ICO_environment_priority";

	// Token: 0x04000BE5 RID: 3045
	public string pathPersuasionIcon = "icons_2d/ICO_persuasion";

	// Token: 0x04000BE6 RID: 3046
	public string pathInvestigationIcon = "icons_2d/ICO_investigation";

	// Token: 0x04000BE7 RID: 3047
	public string pathEspionageIcon = "icons_2d/ICO_espionage";

	// Token: 0x04000BE8 RID: 3048
	public string pathCommandIcon = "icons_2d/ICO_command";

	// Token: 0x04000BE9 RID: 3049
	public string pathAdministrationIcon = "icons_2d/ICO_administration";

	// Token: 0x04000BEA RID: 3050
	public string pathScienceIcon = "icons_2d/ICO_science";

	// Token: 0x04000BEB RID: 3051
	public string pathSecurityIcon = "icons_2d/ICO_security";

	// Token: 0x04000BEC RID: 3052
	public string pathLoyaltyIcon = "icons_2d/ICO_loyalty";

	// Token: 0x04000BED RID: 3053
	public string pathResNoneIcon = "icons_2d/ICO_res_none";

	// Token: 0x04000BEE RID: 3054
	public string pathResPossibleIcon = "icons_2d/ICO_res_maybe";

	// Token: 0x04000BEF RID: 3055
	public string pathResLowIcon = "icons_2d/ICO_res_1";

	// Token: 0x04000BF0 RID: 3056
	public string pathResMedIcon = "icons_2d/ICO_res_2";

	// Token: 0x04000BF1 RID: 3057
	public string pathResHighIcon = "icons_2d/ICO_res_3";

	// Token: 0x04000BF2 RID: 3058
	public string pathResMaxIcon = "icons_2d/ICO_res_4";

	// Token: 0x04000BF3 RID: 3059
	public string pathArmyIconBackground = "mapicons/ICO_geoscape_army";

	// Token: 0x04000BF4 RID: 3060
	public string pathCouncilorIconBackground = "mapicons/Master_councilor_background";

	// Token: 0x04000BF5 RID: 3061
	public string pathCoreEconomicRegion = "mapicons/ICO_geoscape_core_eco";

	// Token: 0x04000BF6 RID: 3062
	public string pathCoreResourceRegion_Oil = "mapicons/ICO_geoscape_core_resources";

	// Token: 0x04000BF7 RID: 3063
	public string pathCoreResourceRegion_Mining = "mapicons/ICO_geoscape_core_resources_mining";

	// Token: 0x04000BF8 RID: 3064
	public string pathCoreResourceRegion_PotentialOil = "mapicons/ICO_potential_oil_region";

	// Token: 0x04000BF9 RID: 3065
	public string pathAlienArmy_attacking = "mapicons/ICO_geoscape_Alien_army_att";

	// Token: 0x04000BFA RID: 3066
	public string pathAlienArmy_defending = "mapicons/ICO_geoscape_Alien_army_def";

	// Token: 0x04000BFB RID: 3067
	public string pathAlienMegafaunaArmy = "mapicons/AlienFaunaArmyBase";

	// Token: 0x04000BFC RID: 3068
	public int minArmyBaseTechLevel = 2;

	// Token: 0x04000BFD RID: 3069
	public int maxArmyBaseTechLevel = 7;

	// Token: 0x04000BFE RID: 3070
	public string pathArmy0_attacking = "mapicons/ICO_geoscape_TechLvl2_army_att";

	// Token: 0x04000BFF RID: 3071
	public string pathArmy1_attacking = "mapicons/ICO_geoscape_TechLvl2_army_att";

	// Token: 0x04000C00 RID: 3072
	public string pathArmy2_attacking = "mapicons/ICO_geoscape_TechLvl2_army_att";

	// Token: 0x04000C01 RID: 3073
	public string pathArmy3_attacking = "mapicons/ICO_geoscape_TechLvl3_army_att";

	// Token: 0x04000C02 RID: 3074
	public string pathArmy4_attacking = "mapicons/ICO_geoscape_TechLvl4_army_att";

	// Token: 0x04000C03 RID: 3075
	public string pathArmy5_attacking = "mapicons/ICO_geoscape_TechLvl6_army_att";

	// Token: 0x04000C04 RID: 3076
	public string pathArmy6_attacking = "mapicons/ICO_geoscape_TechLvl5_army_att";

	// Token: 0x04000C05 RID: 3077
	public string pathArmy7_attacking = "mapicons/ICO_geoscape_TechLvl7_army_att";

	// Token: 0x04000C06 RID: 3078
	public string pathArmy0_defending = "mapicons/ICO_geoscape_TechLvl2_army_def";

	// Token: 0x04000C07 RID: 3079
	public string pathArmy1_defending = "mapicons/ICO_geoscape_TechLvl2_army_def";

	// Token: 0x04000C08 RID: 3080
	public string pathArmy2_defending = "mapicons/ICO_geoscape_TechLvl2_army_def";

	// Token: 0x04000C09 RID: 3081
	public string pathArmy3_defending = "mapicons/ICO_geoscape_TechLvl3_army_def";

	// Token: 0x04000C0A RID: 3082
	public string pathArmy4_defending = "mapicons/ICO_geoscape_TechLvl4_army_def";

	// Token: 0x04000C0B RID: 3083
	public string pathArmy5_defending = "mapicons/ICO_geoscape_TechLvl6_army_def";

	// Token: 0x04000C0C RID: 3084
	public string pathArmy6_defending = "mapicons/ICO_geoscape_TechLvl5_army_def";

	// Token: 0x04000C0D RID: 3085
	public string pathArmy7_defending = "mapicons/ICO_geoscape_TechLvl7_army_def";

	// Token: 0x04000C0E RID: 3086
	public string pathArmy0_sea = "mapicons/ICO_geoscape_ship__00000";

	// Token: 0x04000C0F RID: 3087
	public string pathArmy2_sea = "mapicons/ICO_geoscape_ship__00000";

	// Token: 0x04000C10 RID: 3088
	public string pathAlienArmy_sea = "mapicons/ICO_geoscape_alien_ ship";

	// Token: 0x04000C11 RID: 3089
	public string pathGeoscapeStation = "mapicons/ICO_geoscape_hum_station";

	// Token: 0x04000C12 RID: 3090
	public string pathGeoscapeBase = "mapicons/ICO_geoscape_hum_base";

	// Token: 0x04000C13 RID: 3091
	public string pathGeoscapeUnidentifiedCouncilor = "mapicons/ICO_geoscape_councilor_unknown_generic";

	// Token: 0x04000C14 RID: 3092
	public string pathGeoscapeCrashdown = "mapicons/ICO_geoscape_alien_crashdown";

	// Token: 0x04000C15 RID: 3093
	public string pathGeoscapeUFOLanding = "mapicons/ICO_geoscape_alien_UFO";

	// Token: 0x04000C16 RID: 3094
	public string pathGeoscapeAbductions = "mapicons/ICO_geoscape_alien_abductions";

	// Token: 0x04000C17 RID: 3095
	public string pathGeoscapeEnthrallPublic = "mapicons/ICO_geoscape_alien_enthrallpublic";

	// Token: 0x04000C18 RID: 3096
	public string pathGeoscapeEnthrallElites = "mapicons/ICO_geoscape_alien_enthrallelites";

	// Token: 0x04000C19 RID: 3097
	public string pathGeoscapeAlienActivity = "mapicons/ICO_geoscape_alien_activity";

	// Token: 0x04000C1A RID: 3098
	public string pathGeoscapeTerrorize = "mapicons/ICO_geoscape_alien_terrorizeregion";

	// Token: 0x04000C1B RID: 3099
	public string pathGeoscapeXenoform = "mapicons/ICO_geoscape_alien_xenoform_1";

	// Token: 0x04000C1C RID: 3100
	public string pathGeoscapeAlienFacility = "mapicons/ICO_geoscape_alien_facilities";

	// Token: 0x04000C1D RID: 3101
	public string pathGeoscapeXenoform1 = "mapicons/ICO_geoscape_alien_xenoform_1";

	// Token: 0x04000C1E RID: 3102
	public string pathGeoscapeXenoform2 = "mapicons/ICO_geoscape_alien_xenoform_2";

	// Token: 0x04000C1F RID: 3103
	public string pathGeoscapeXenoform3 = "mapicons/ICO_geoscape_alien_xenoform_3";

	// Token: 0x04000C20 RID: 3104
	public string pathGeoscapeLaunchSite1 = "mapicons/ICO_geoscape_space_launch_small";

	// Token: 0x04000C21 RID: 3105
	public string pathGeoscapeLaunchSite2 = "mapicons/ICO_geoscape_space_launch_medium";

	// Token: 0x04000C22 RID: 3106
	public string pathGeoscapeLaunchSite3 = "mapicons/ICO_geoscape_space_launch_large";

	// Token: 0x04000C23 RID: 3107
	public string pathGeoscapeMissionControl1 = "mapicons/ICO_geoscape_mission_ctrl";

	// Token: 0x04000C24 RID: 3108
	public string pathGeoscapeMissionControl2 = "mapicons/ICO_geoscape_mission_ctrl";

	// Token: 0x04000C25 RID: 3109
	public string pathGeoscapeMissionControl3 = "mapicons/ICO_geoscape_mission_ctrl";

	// Token: 0x04000C26 RID: 3110
	public string pathGeoscapeSpaceDefenses = "mapicons/ICO_geoscape_laser_orbit";

	// Token: 0x04000C27 RID: 3111
	public string pathGeoscapeAirliner1 = "mapicons/ICO_airplane_10";

	// Token: 0x04000C28 RID: 3112
	public string pathGeoscapePrivateJet1 = "mapicons/ICO_airplane_20";

	// Token: 0x04000C29 RID: 3113
	public string pathGeoscapeAirliner2 = "mapicons/ICO_airplane_30";

	// Token: 0x04000C2A RID: 3114
	public string pathGeoscapePrivateJet2 = "mapicons/ICO_airplane_40";

	// Token: 0x04000C2B RID: 3115
	public string pathEnergyIcon = "icons_2d/tech_energy_icon";

	// Token: 0x04000C2C RID: 3116
	public string pathInformationScienceIcon = "icons_2d/tech_info_icon";

	// Token: 0x04000C2D RID: 3117
	public string pathLifeScienceIcon = "icons_2d/tech_life_icon";

	// Token: 0x04000C2E RID: 3118
	public string pathMaterialsIcon = "icons_2d/tech_material_icon";

	// Token: 0x04000C2F RID: 3119
	public string pathMilitaryScienceIcon = "icons_2d/tech_military_icon";

	// Token: 0x04000C30 RID: 3120
	public string pathSocialScienceIcon = "icons_2d/tech_social_icon";

	// Token: 0x04000C31 RID: 3121
	public string pathSpaceScienceIcon = "icons_2d/tech_space_icon";

	// Token: 0x04000C32 RID: 3122
	public string pathXenologyIcon = "icons_2d/tech_xeno_icon";

	// Token: 0x04000C33 RID: 3123
	public string pathProbeComplete = "icons_2d/ICO_probe";

	// Token: 0x04000C34 RID: 3124
	public string pathProbeEnRoute = "icons_2d/ICO_probe_en_route";

	// Token: 0x04000C35 RID: 3125
	public string greenUpArrow = "icons_2d/ICO_arrow_green";

	// Token: 0x04000C36 RID: 3126
	public string greenDownArrow = "icons_2d/ICO_arrow_green_down";

	// Token: 0x04000C37 RID: 3127
	public string redUpArrow = "icons_2d/ICO_arrow_red";

	// Token: 0x04000C38 RID: 3128
	public string redDownArrow = "icons_2d/ICO_arrow_red_down";

	// Token: 0x04000C39 RID: 3129
	public string pathNoneIcon = "icons_2d/ICO_none";

	// Token: 0x04000C3A RID: 3130
	public string crackdownMissionIconPath = "councilor_missions/ICO_crackdown_off";

	// Token: 0x04000C3B RID: 3131
	public string defendInterestsMissionIconPath = "councilor_missions/ICO_defendinterest_off";

	// Token: 0x04000C3C RID: 3132
	public string friendlyRelationsInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"nation_at_peace\"></color>";

	// Token: 0x04000C3D RID: 3133
	public string smallCrackdownMissionIconPath = "icons_2d/ICO_crackdown_small_on";

	// Token: 0x04000C3E RID: 3134
	public string smallDefendInterestsMissionIconPath = "icons_2d/ICO_defend_interest_small_on";

	// Token: 0x04000C3F RID: 3135
	public string pathPlayButton = "icons_2d/BUT_play_normal";

	// Token: 0x04000C40 RID: 3136
	public string pathPauseButton = "icons_2d/BUT_pause_normal";

	// Token: 0x04000C41 RID: 3137
	public string pathPlusButtonIconPath = "icons_2d/BUT_plus_normal";

	// Token: 0x04000C42 RID: 3138
	public string pathMinusButtonIconPath = "icons_2d/BUT_minus_normal";

	// Token: 0x04000C43 RID: 3139
	public string pathPlusHoverButtonIconPath = "icons_2d/BUT_plus_hover";

	// Token: 0x04000C44 RID: 3140
	public string pathMinusHoverButtonIconPath = "icons_2d/BUT_minus_hover";

	// Token: 0x04000C45 RID: 3141
	public string pathNotificationPlusButtonIconPath = "icons_2d/BUT_maximizegold_normal";

	// Token: 0x04000C46 RID: 3142
	public string pathNotificationPlusHoverButtonIconPath = "icons_2d/BUT_maximizegold_hover";

	// Token: 0x04000C47 RID: 3143
	public string pathNotificationMinusButtonIconPath = "icons_2d/BUT_minimizegold_normal";

	// Token: 0x04000C48 RID: 3144
	public string pathNotificationMinusHoverButtonIconPath = "icons_2d/BUT_minimizegold_hover";

	// Token: 0x04000C49 RID: 3145
	public string pathMaximizeButtonIconPath = "icons_2d/BUT_maximize_normal";

	// Token: 0x04000C4A RID: 3146
	public string pathMaximizeHoverButtonIconPath = "icons_2d/BUT_maximize_hover";

	// Token: 0x04000C4B RID: 3147
	public string pathMinimizeButtonIconPath = "icons_2d/BUT_minimize_normal";

	// Token: 0x04000C4C RID: 3148
	public string pathMinimizeHoverButtonIconPath = "icons_2d/BUT_minimize_hover";

	// Token: 0x04000C4D RID: 3149
	public string pathMaxTier1Hab = "icons_2d/ICO_MaxTier1";

	// Token: 0x04000C4E RID: 3150
	public string pathMaxTier2Hab = "icons_2d/ICO_MaxTier2";

	// Token: 0x04000C4F RID: 3151
	public string pathMaxTier3Hab = "icons_2d/ICO_MaxTier3";

	// Token: 0x04000C50 RID: 3152
	public string pathMaxTier4Hab = "icons_2d/ICO_MaxTier4";

	// Token: 0x04000C51 RID: 3153
	public string pathWeaponExplosion = "spacecombat/WeaponDestructionExplosion";

	// Token: 0x04000C52 RID: 3154
	public string pathAlienThrusterVFX = "ships/AlienThrusterVector";

	// Token: 0x04000C53 RID: 3155
	public string pathHumanThrusterBasicVFX = "ships/HumanThrusterVectorBasic";

	// Token: 0x04000C54 RID: 3156
	public string pathHumanThrusterAdvancedVFX = "ships/HumanThrusterVectorAdvanced";

	// Token: 0x04000C55 RID: 3157
	public string pathFallbackLaserVFX = "spaceCombat/Standard Laser Beam Red";

	// Token: 0x04000C56 RID: 3158
	public string pathFallbackMuzzleFlashVFX = "spaceCombat/MuzzleFlashFlame";

	// Token: 0x04000C57 RID: 3159
	public string pathFallbackProjectileVFX = "spaceCombat/BulletOrange";

	// Token: 0x04000C58 RID: 3160
	public string pathGeoscapeCrashdown_gui = "geoscape_gui/ICO_geoscape_alien_crashdown";

	// Token: 0x04000C59 RID: 3161
	public string pathGeoscapeStation_gui = "geoscape_gui/ICO_geoscape_hum_station";

	// Token: 0x04000C5A RID: 3162
	public string pathGeoscapeBase_gui = "geoscape_gui/ICO_geoscape_hum_base";

	// Token: 0x04000C5B RID: 3163
	public string pathGeoscapeUFOLanding_gui = "geoscape_gui/ICO_geoscape_alien_UFO";

	// Token: 0x04000C5C RID: 3164
	public string pathGeoscapeAbductions_gui = "geoscape_gui/ICO_geoscape_alien_abductions";

	// Token: 0x04000C5D RID: 3165
	public string pathGeoscapeEnthrallPublic_gui = "geoscape_gui/ICO_geoscape_alien_enthrallpublic";

	// Token: 0x04000C5E RID: 3166
	public string pathGeoscapeEnthrallElites_gui = "geoscape_gui/ICO_geoscape_alien_enthrallelites";

	// Token: 0x04000C5F RID: 3167
	public string pathGeoscapeAlienActivity_gui = "geoscape_gui/ICO_geoscape_alien_activity";

	// Token: 0x04000C60 RID: 3168
	public string pathGeoscapeTerrorize_gui = "geoscape_gui/ICO_geoscape_alien_terrorizeregion";

	// Token: 0x04000C61 RID: 3169
	public string pathGeoscapeXenoform_gui = "geoscape_gui/ICO_geoscape_alien_xenoform_1";

	// Token: 0x04000C62 RID: 3170
	public string pathGeoscapeAlienFacility_gui = "geoscape_gui/ICO_geoscape_alien_facilities";

	// Token: 0x04000C63 RID: 3171
	public string investmentInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"investments\"></color>";

	// Token: 0x04000C64 RID: 3172
	public string educationInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"education\"></color>";

	// Token: 0x04000C65 RID: 3173
	public string cohesionInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"culture\"></color>";

	// Token: 0x04000C66 RID: 3174
	public string democracyInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"gov_type\"></color>";

	// Token: 0x04000C67 RID: 3175
	public string unrestInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"nation_unrest\"></color>";

	// Token: 0x04000C68 RID: 3176
	public string perCapitaGDPInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"per_capita_GDP\"></color>";

	// Token: 0x04000C69 RID: 3177
	public string inequalityInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"inequality\"></color>";

	// Token: 0x04000C6A RID: 3178
	public string populationInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"population\"></color>";

	// Token: 0x04000C6B RID: 3179
	public string nukesInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"nukes\"></color>";

	// Token: 0x04000C6C RID: 3180
	public string miltechInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"army_level\"></color>";

	// Token: 0x04000C6D RID: 3181
	public string persuasionInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_persuasion\"></color>";

	// Token: 0x04000C6E RID: 3182
	public string investigationInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_investigation\"></color>";

	// Token: 0x04000C6F RID: 3183
	public string espionageInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_espionage\"></color>";

	// Token: 0x04000C70 RID: 3184
	public string commandInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_command\"></color>";

	// Token: 0x04000C71 RID: 3185
	public string administrationInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_administration\"></color>";

	// Token: 0x04000C72 RID: 3186
	public string scienceInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_science\"></color>";

	// Token: 0x04000C73 RID: 3187
	public string securityInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_security\"></color>";

	// Token: 0x04000C74 RID: 3188
	public string loyaltyInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"attribute_loyalty\"></color>";

	// Token: 0x04000C75 RID: 3189
	public string controlPointInlineSpritePath_empty = "<color=#FFFFFFFF><sprite name=\"control_point\"></color>";

	// Token: 0x04000C76 RID: 3190
	public string controlPointInlineSpritePath_color = "<sprite name=\"control_point\">";

	// Token: 0x04000C77 RID: 3191
	public string boostInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"boost\"></color>";

	// Token: 0x04000C78 RID: 3192
	public string missionControlInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"mission_control\"></color>";

	// Token: 0x04000C79 RID: 3193
	public string moneyInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"currency\"></color>";

	// Token: 0x04000C7A RID: 3194
	public string influenceInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"influence\"></color>";

	// Token: 0x04000C7B RID: 3195
	public string opsInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"ops\"></color>";

	// Token: 0x04000C7C RID: 3196
	public string researchInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"research\"></color>";

	// Token: 0x04000C7D RID: 3197
	public string projectsInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"projects\"></color>";

	// Token: 0x04000C7E RID: 3198
	public string waterInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"water\"></color>";

	// Token: 0x04000C7F RID: 3199
	public string metalsInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"metal\"></color>";

	// Token: 0x04000C80 RID: 3200
	public string noblesInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"metal_noble\"></color>";

	// Token: 0x04000C81 RID: 3201
	public string volatilesInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"volatiles\"></color>";

	// Token: 0x04000C82 RID: 3202
	public string fissilesInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"radioactive\"></color>";

	// Token: 0x04000C83 RID: 3203
	public string exoticsInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"exotics\"></color>";

	// Token: 0x04000C84 RID: 3204
	public string antimatterInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"antimatter\"></color>";

	// Token: 0x04000C85 RID: 3205
	public string upGreenArrowInlineSpritePath = "<color=#FFFFFFFF><sprite name=arrow_green_up></color>";

	// Token: 0x04000C86 RID: 3206
	public string downGreenArrowInlineSpritePath = "<color=#FFFFFFFF><sprite name=arrow_green_down></color>";

	// Token: 0x04000C87 RID: 3207
	public string upRedArrowInlineSpritePath = "<color=#FFFFFFFF><sprite name=arrow_red_up></color>";

	// Token: 0x04000C88 RID: 3208
	public string downRedArrowInlineSpritePath = "<color=#FFFFFFFF><sprite name=arrow_red_down></color>";

	// Token: 0x04000C89 RID: 3209
	public string spaceCombatScoreInlineSpritePath = "<color=#FFFFFFFF><sprite name=combat_score></color>";

	// Token: 0x04000C8A RID: 3210
	public string spaceDebrisInlineSpritePath = "<color=#FFFFFFFF><sprite name=space_debris></color>";

	// Token: 0x04000C8B RID: 3211
	public string tutorialInlineSpritePath = "<color=#FFFFFFFF><sprite name=tutorial></color>";

	// Token: 0x04000C8C RID: 3212
	public string spaceAssaultValueInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"space_assault_score\"></color>";

	// Token: 0x04000C8D RID: 3213
	public string pathInlineSpaceMiningIcon = "<color=#FFFFFFFF><sprite name=core_res></color>";

	// Token: 0x04000C8E RID: 3214
	public string pathInlineSolarIcon = "<color=#FFFFFFFF><sprite name=sun></color>";

	// Token: 0x04000C8F RID: 3215
	public string shipDamageInlineSpritePath = "<color=#FFFFFFFF><sprite name=ship_damage></color>";

	// Token: 0x04000C90 RID: 3216
	public string armorInlineSpritePath = "<color=#FFFFFFFF><sprite name=armor></color>";

	// Token: 0x04000C91 RID: 3217
	public string armyBattleInlineSpritePath = "<color=#FFFFFFFF><sprite name=army_battle></color>";

	// Token: 0x04000C92 RID: 3218
	public string pathInlineHabStationIcon = "<color=#FFFFFFFF><sprite name=hum_station></color>";

	// Token: 0x04000C93 RID: 3219
	public string pathInlineHabBaseIcon = "<color=#FFFFFFFF><sprite name=hum_base></color>";

	// Token: 0x04000C94 RID: 3220
	public string pathInlineEscapeVelocityIcon = "<color=#FFFFFFFF><sprite name=escape></color>";

	// Token: 0x04000C95 RID: 3221
	public string orbitInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"radius_of_orbit\"></color>";

	// Token: 0x04000C96 RID: 3222
	public string zeroResourcesInlineSpritePath = "<color=#FFFFFFFF><sprite=4></color>";

	// Token: 0x04000C97 RID: 3223
	public string noneIconInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"ico_none\"></color>";

	// Token: 0x04000C98 RID: 3224
	public string unknownResourcesInlineSpritePath = "<color=#FFFFFFFF><sprite=5></color>";

	// Token: 0x04000C99 RID: 3225
	public string level1ResourcesInlineSpritePath = "<color=#FFFFFFFF><sprite=8></color>";

	// Token: 0x04000C9A RID: 3226
	public string level2ResourcesInlineSpritePath = "<color=#FFFFFFFF><sprite=7></color>";

	// Token: 0x04000C9B RID: 3227
	public string level3ResourcesInlineSpritePath = "<color=#FFFFFFFF><sprite=6></color>";

	// Token: 0x04000C9C RID: 3228
	public string level4ResourcesInlineSpritePath = "<color=#FFFFFFFF><sprite name=\"res_4\"></color>";

	// Token: 0x04000C9D RID: 3229
	public string armyInlineSpritePath = "<color=#FFFFFFFF><sprite name=army2></color>";

	// Token: 0x04000C9E RID: 3230
	public string navyInlineSpritePath = "<color=#FFFFFFFF><sprite name=naval_army></color>";

	// Token: 0x04000C9F RID: 3231
	public string noNavyInlineSpritePath = "<color=#FFFFFFFF><sprite name=no_naval_mov></color>";

	// Token: 0x04000CA0 RID: 3232
	public string capitalRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=capital_region></color>";

	// Token: 0x04000CA1 RID: 3233
	public string coreEconomicRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=eco_region></color>";

	// Token: 0x04000CA2 RID: 3234
	public string miningRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=mining></color>";

	// Token: 0x04000CA3 RID: 3235
	public string coreOilRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=oil></color>";

	// Token: 0x04000CA4 RID: 3236
	public string potentialMiningRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=mining_potential></color>";

	// Token: 0x04000CA5 RID: 3237
	public string potentialCoreOilRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=oil_potential></color>";

	// Token: 0x04000CA6 RID: 3238
	public string colonyRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=colony></color>";

	// Token: 0x04000CA7 RID: 3239
	public string ecologicallyVulnerableRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=eco_vul></color>";

	// Token: 0x04000CA8 RID: 3240
	public string ecologicallySafeRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=eco_immune></color>";

	// Token: 0x04000CA9 RID: 3241
	public string ruggedRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=rugged_terrain></color>";

	// Token: 0x04000CAA RID: 3242
	public string nukedRegionInlineSpritePath = "<color=#FFFFFFFF><sprite name=geoscape_irradiated></color>";

	// Token: 0x04000CAB RID: 3243
	public string alienEntityInlineSpritePath = "<color=#FFFFFFFF><sprite name=alien_marker></color>";

	// Token: 0x04000CAC RID: 3244
	public string antiSpaceDefensesInlineSpritePath = "<color=#FFFFFFFF><sprite name=laser_orbit></color>";

	// Token: 0x04000CAD RID: 3245
	public string occupationInlineSpritePath = "<color=#FFFFFFFF><sprite name=region_occupation></color>";

	// Token: 0x04000CAE RID: 3246
	public string energyTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_Energy></color>";

	// Token: 0x04000CAF RID: 3247
	public string informationTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_InformationScience></color>";

	// Token: 0x04000CB0 RID: 3248
	public string militaryTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_MilitaryScience></color>";

	// Token: 0x04000CB1 RID: 3249
	public string materialsTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_Materials></color>";

	// Token: 0x04000CB2 RID: 3250
	public string lifeTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_LifeScience></color>";

	// Token: 0x04000CB3 RID: 3251
	public string socialTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_SocialScience></color>";

	// Token: 0x04000CB4 RID: 3252
	public string spaceTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_SpaceScience></color>";

	// Token: 0x04000CB5 RID: 3253
	public string xenologyTechInlineSpritePath = "<color=#FFFFFFFF><sprite name=tech_Xenology></color>";

	// Token: 0x04000CB6 RID: 3254
	public string victoryItemInlineSpritePath = "<color=#FFFFFFFF><sprite name=victory></color>";

	// Token: 0x04000CB7 RID: 3255
	public string ECO_InlineSpritePath = "<color=#FFFFFFFF><sprite name=economy></color>";

	// Token: 0x04000CB8 RID: 3256
	public string WEL_InlineSpritePath = "<color=#FFFFFFFF><sprite name=welfare></color>";

	// Token: 0x04000CB9 RID: 3257
	public string ENV_InlineSpritePath = "<color=#FFFFFFFF><sprite name=environment></color>";

	// Token: 0x04000CBA RID: 3258
	public string GOV_InlineSpritePath = "<color=#FFFFFFFF><sprite name=democracy></color>";

	// Token: 0x04000CBB RID: 3259
	public string KNO_InlineSpritePath = "<color=#FFFFFFFF><sprite name=knowledge></color>";

	// Token: 0x04000CBC RID: 3260
	public string UNI_InlineSpritePath = "<color=#FFFFFFFF><sprite name=unity></color>";

	// Token: 0x04000CBD RID: 3261
	public string MIL_InlineSpritePath = "<color=#FFFFFFFF><sprite name=military_org></color>";

	// Token: 0x04000CBE RID: 3262
	public string OPP_InlineSpritePath = "<color=#FFFFFFFF><sprite name=oppression></color>";

	// Token: 0x04000CBF RID: 3263
	public string SPO_InlineSpritePath = "<color=#FFFFFFFF><sprite name=spoils></color>";

	// Token: 0x04000CC0 RID: 3264
	public string DEV_InlineSpritePath = "<color=#FFFFFFFF><sprite name=funding></color>";

	// Token: 0x04000CC1 RID: 3265
	public string BOO_InlineSpritePath = "<color=#FFFFFFFF><sprite name=boost_org></color>";

	// Token: 0x04000CC2 RID: 3266
	public string MC_InlineSpritePath = "<color=#FFFFFFFF><sprite name=mission_control_org></color>";

	// Token: 0x04000CC3 RID: 3267
	public string FMI_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_military></color>";

	// Token: 0x04000CC4 RID: 3268
	public string SUB_InlineSpritePath = "<color=#FFFFFFFF><sprite name=submarine></color>";

	// Token: 0x04000CC5 RID: 3269
	public string ARM_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_army></color>";

	// Token: 0x04000CC6 RID: 3270
	public string NAV_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_navy></color>";

	// Token: 0x04000CC7 RID: 3271
	public string NUC_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_nukes></color>";

	// Token: 0x04000CC8 RID: 3272
	public string NUK_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_nuclear_weapons></color>";

	// Token: 0x04000CC9 RID: 3273
	public string DEF_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_space_defense></color>";

	// Token: 0x04000CCA RID: 3274
	public string STO_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_sto></color>";

	// Token: 0x04000CCB RID: 3275
	public string FLI_InlineSpritePath = "<color=#FFFFFFFF><sprite name=spaceflight_program></color>";

	// Token: 0x04000CCC RID: 3276
	public string CEC_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_core_economic_region></color>";

	// Token: 0x04000CCD RID: 3277
	public string CMI_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_core_mining_region></color>";

	// Token: 0x04000CCE RID: 3278
	public string OIL_InlineSpritePath = "<color=#FFFFFFFF><sprite name=build_core_oil_region></color>";

	// Token: 0x04000CCF RID: 3279
	public string DCL_InlineSpritePath = "<color=#FFFFFFFF><sprite name=decolonize_region></color>";

	// Token: 0x04000CD0 RID: 3280
	public string DCT_InlineSpritePath = "<color=#FFFFFFFF><sprite name=decontaminate></color>";

	// Token: 0x04000CD1 RID: 3281
	public string sustainabilityInlineSpritePath_Red = "<color=#FFFFFFFF><sprite name=\"greeneconomy_red\"></color>";

	// Token: 0x04000CD2 RID: 3282
	public string sustainabilityInlineSpritePath_Orange = "<color=#FFFFFFFF><sprite name=\"greeneconomy_orange\"></color>";

	// Token: 0x04000CD3 RID: 3283
	public string sustainabilityInlineSpritePath_Yellow = "<color=#FFFFFFFF><sprite name=\"greeneconomy_yellow\"></color>";

	// Token: 0x04000CD4 RID: 3284
	public string sustainabilityInlineSpritePath_Blue = "<color=#FFFFFFFF><sprite name=\"greeneconomy_blue\"></color>";

	// Token: 0x04000CD5 RID: 3285
	public string sustainabilityInlineSpritePath_Green = "<color=#FFFFFFFF><sprite name=\"greeneconomy_green\"></color>";

	// Token: 0x04000CD6 RID: 3286
	public string habShipyardPresentInlineSpritePath = "<color=#FFFFFFFF><sprite name=construction_shipyard></color>";

	// Token: 0x04000CD7 RID: 3287
	public string habResupplyPresentInlineSpritePath = "<color=#FFFFFFFF><sprite name=supply></color>";

	// Token: 0x04000CD8 RID: 3288
	public string habModuleConstructionInlineSpritePath = "<color=#FFFFFFFF><sprite name=construction_module></color>";

	// Token: 0x04000CD9 RID: 3289
	public string habDefenseScoreInlineSpritePath = "<color=#FFFFFFFF><sprite name=combat_score></color>";

	// Token: 0x04000CDA RID: 3290
	public string habPowerInlineSpritePath = "<color=#FFFFFFFF><sprite name=hab_power></color>";

	// Token: 0x04000CDB RID: 3291
	public string habPowerAlertInlineSpritePath = "<color=#FFFFFFFF><sprite name=hab_power_alert></color>";

	// Token: 0x04000CDC RID: 3292
	public string irradiatedInlineSpritePath = "<color=#FFFFFFFF><sprite name=irradiated_site></color>";

	// Token: 0x04000CDD RID: 3293
	public string deltaInlineSpritePath = "<color=#FFFFFFFF><sprite name=delta></color>";

	// Token: 0x04000CDE RID: 3294
	public string deltaVInlineSpritePath = "<color=#FFFFFFFF><sprite name=delta_v></color>";

	// Token: 0x04000CDF RID: 3295
	public string lessThanOrEqualToInlineSpritePath = "<sprite name=sprite_sheet_inline_80>";

	// Token: 0x04000CE0 RID: 3296
	public string greaterThanOrEqualToInlineSpritePath = "<sprite name=sprite_sheet_inline_81>";

	// Token: 0x04000CE1 RID: 3297
	public string warningInlineSpritePath = "<color=#FFFFFFFF><sprite name=warning></color>";

	// Token: 0x04000CE2 RID: 3298
	public string starInlineSpritePath = "<color=#FFFFFFFF><sprite name=star></color>";

	// Token: 0x04000CE3 RID: 3299
	public string grayStarInlineSpritePath = "<color=#FFFFFFFF><sprite name=empty_star></color>";

	// Token: 0x04000CE4 RID: 3300
	public string starInlineSpritePath_sizeOverride60 = "<size=60%><color=#FFFFFFFF><sprite name=star></color></size>";

	// Token: 0x04000CE5 RID: 3301
	public string underConstructionInlineSpritePath = "<color=#FFFFFFFF><sprite name=under_construction></color>";

	// Token: 0x04000CE6 RID: 3302
	public string probeCompleteInlineSpritePath = "<color=#FFFFFFFF><sprite name=probe></color>";

	// Token: 0x04000CE7 RID: 3303
	public string probeEnRouteInlineSpritePath = "<color=#FFFFFFFF><sprite name=probe_en_route></color>";

	// Token: 0x04000CE8 RID: 3304
	public string gravityInlineSpritePath = "<color=#FFFFFFFF><sprite name=gravity></color>";

	// Token: 0x04000CE9 RID: 3305
	public string keyboard_AltInlineSpritePath = "<color=#FFFFFFFF><sprite name=keyboard_alt></color>";

	// Token: 0x04000CEA RID: 3306
	public string keyboard_CtrlInlineSpritePath = "<color=#FFFFFFFF><sprite name=keyboard_ctrl></color>";

	// Token: 0x04000CEB RID: 3307
	public string keyboard_StrgInlineSpritePath = "<color=#FFFFFFFF><sprite name=keyboard_strg></color>";

	// Token: 0x04000CEC RID: 3308
	public string keyboard_ShiftInlineSpritePath = "<color=#FFFFFFFF><sprite name=keyboard_shift></color>";

	// Token: 0x04000CED RID: 3309
	public string station_human_underconstruction_t1_icon = "habmodules/station_T1_underconstruction";

	// Token: 0x04000CEE RID: 3310
	public string station_human_underconstruction_t2_icon = "habmodules/station_T2_underconstruction";

	// Token: 0x04000CEF RID: 3311
	public string station_human_underconstruction_t3_icon = "habmodules/station_T3_underconstruction";

	// Token: 0x04000CF0 RID: 3312
	public string station_alien_underconstruction_t1_icon = "habmodules/station_T1_AlienUnderconstruction";

	// Token: 0x04000CF1 RID: 3313
	public string station_alien_underconstruction_t2_icon = "habmodules/station_T2_AlienUnderconstruction";

	// Token: 0x04000CF2 RID: 3314
	public string station_alien_underconstruction_t3_icon = "habmodules/station_T3_AlienUnderconstruction";

	// Token: 0x04000CF3 RID: 3315
	public string station_human_underconstruction_t1_module = "habmodules/station_T1_underconstruction_Module";

	// Token: 0x04000CF4 RID: 3316
	public string station_human_underconstruction_t2_module = "habmodules/station_T2_underconstruction_Module";

	// Token: 0x04000CF5 RID: 3317
	public string station_human_underconstruction_t3_module = "habmodules/station_T3_underconstruction_Module";

	// Token: 0x04000CF6 RID: 3318
	public string station_alien_underconstruction_t1_module = "habModules/station_T1_AlienUnderconstruction_Module";

	// Token: 0x04000CF7 RID: 3319
	public string station_alien_underconstruction_t2_module = "habmodules/station_T2_AlienUnderconstruction_Module";

	// Token: 0x04000CF8 RID: 3320
	public string station_alien_underconstruction_t3_module = "habmodules/station_T3_AlienUnderconstruction_Module";

	// Token: 0x04000CF9 RID: 3321
	public string station_human_underconstruction_t1_module_destruction = "habmodules/stationdestruction_T1_generic";

	// Token: 0x04000CFA RID: 3322
	public string station_human_underconstruction_t2_module_destruction = "habmodules/stationdestruction_T2_generic";

	// Token: 0x04000CFB RID: 3323
	public string station_human_underconstruction_t3_module_destruction = "habmodules/stationdestruction_T3_generic";

	// Token: 0x04000CFC RID: 3324
	public string station_alien_underconstruction_t1_module_destruction = "habmodules/stationdestruction_alien_T1_generic";

	// Token: 0x04000CFD RID: 3325
	public string station_alien_underconstruction_t2_module_destruction = "habmodules/stationdestruction_alien_T2_generic";

	// Token: 0x04000CFE RID: 3326
	public string station_alien_underconstruction_t3_module_destruction = "habmodules/stationdestruction_alien_T3_generic";

	// Token: 0x04000CFF RID: 3327
	public string rammingSpeedIcon = "ui_spacecombat/ICO_RammingSpeedCommand_off";

	// Token: 0x04000D00 RID: 3328
	public string AllStopCommandOffIcon = "ui_spacecombat/AllStopCommand_off";

	// Token: 0x04000D01 RID: 3329
	public string disengageIcon = "ui_spacecombat/ICO_DisengageCommand_off";

	// Token: 0x04000D02 RID: 3330
	public string alarmClockIcon = "icons_2d/ICO_alarmClock";

	// Token: 0x04000D03 RID: 3331
	public string alarmClockInlineSpritePath = "<color=#FFFFFFFF><sprite name=alarmClock></color>";

	// Token: 0x04000D04 RID: 3332
	public string illus_launchFacilitySmallPath = "illustrations/Location_MediumLaunchFacility";

	// Token: 0x04000D05 RID: 3333
	public string illus_launchFacilityMediumPath = "illustrations/Location_MediumLaunchFacility";

	// Token: 0x04000D06 RID: 3334
	public string illus_launchFacilityLargePath = "illustrations/Location_MediumLaunchFacility";

	// Token: 0x04000D07 RID: 3335
	public string illus_missionControlFacilitySmallPath = "illustrations/Location_MediumMissionControlFacility";

	// Token: 0x04000D08 RID: 3336
	public string illus_missionControlFacilityMediumPath = "illustrations/Location_MediumMissionControlFacility";

	// Token: 0x04000D09 RID: 3337
	public string illus_missionControlFacilityLargePath = "illustrations/Location_MediumMissionControlFacility";

	// Token: 0x04000D0A RID: 3338
	public string illus_spaceDefensesPath = "illustrations/Location_AntiSpaceDefenses";

	// Token: 0x04000D0B RID: 3339
	public string illus_xenoformingStage1 = "illustrations/Location_XenoformingLow";

	// Token: 0x04000D0C RID: 3340
	public string illus_xenoformingStage2 = "illustrations/Location_XenoformingMed";

	// Token: 0x04000D0D RID: 3341
	public string illus_xenoformingStage3 = "illustrations/Location_XenoformingHigh";

	// Token: 0x04000D0E RID: 3342
	public string illus_landedUFO = "illustrations/Event_LandedUFO";

	// Token: 0x04000D0F RID: 3343
	public string illus_crashedUFO = "illustrations/Objective_InvestigateAlienCrashdown";

	// Token: 0x04000D10 RID: 3344
	public string illus_alienActivity = "illustrations/AlienActivity_Generic";

	// Token: 0x04000D11 RID: 3345
	public string illus_alienFacility = "illustrations/Location_AlienFacility";

	// Token: 0x04000D12 RID: 3346
	public string illus_enthrallPublic = "illustrations/AlienActivity_EnthrallPublic";

	// Token: 0x04000D13 RID: 3347
	public string illus_enthrallElites = "illustrations/AlienActivity_EnthrallElites";

	// Token: 0x04000D14 RID: 3348
	public string illus_abductions = "illustrations/AlienActivity_Abductions";

	// Token: 0x04000D15 RID: 3349
	public string illus_terrorize = "illustrations/AlienActivity_TerrorizeRegion";

	// Token: 0x04000D16 RID: 3350
	public string illus_humanArmy0 = "illustrations/Army_Tech1";

	// Token: 0x04000D17 RID: 3351
	public string illus_humanArmy1 = "illustrations/Army_Tech1";

	// Token: 0x04000D18 RID: 3352
	public string illus_humanArmy2 = "illustrations/Army_Tech2";

	// Token: 0x04000D19 RID: 3353
	public string illus_humanArmy3 = "illustrations/Army_Tech3";

	// Token: 0x04000D1A RID: 3354
	public string illus_humanArmy4 = "illustrations/Army_Tech4";

	// Token: 0x04000D1B RID: 3355
	public string illus_humanArmy5 = "illustrations/Army_Tech5";

	// Token: 0x04000D1C RID: 3356
	public string illus_humanArmy6 = "illustrations/Army_Tech6";

	// Token: 0x04000D1D RID: 3357
	public string illus_humanArmy7 = "illustrations/Army_Tech7";

	// Token: 0x04000D1E RID: 3358
	public string illus_humanNavyTransport0 = "illustrations/Army_SeaTech2";

	// Token: 0x04000D1F RID: 3359
	public string illus_humanNavyTransport2 = "illustrations/Army_SeaTech2";

	// Token: 0x04000D20 RID: 3360
	public string illus_humanNavyTransport6 = "illustrations/Army_SeaTech6";

	// Token: 0x04000D21 RID: 3361
	public string illus_armyConstructed = "illustrations/Army_NewConstructed";

	// Token: 0x04000D22 RID: 3362
	public string illus_armyAssigned = "illustrations/Army_AssignedToFaction";

	// Token: 0x04000D23 RID: 3363
	public string illus_humanOutpost = "";

	// Token: 0x04000D24 RID: 3364
	public string illus_humanSettlement = "";

	// Token: 0x04000D25 RID: 3365
	public string illus_humanColony = "";

	// Token: 0x04000D26 RID: 3366
	public string illus_alienOutpost = "";

	// Token: 0x04000D27 RID: 3367
	public string illus_alienSettlement = "";

	// Token: 0x04000D28 RID: 3368
	public string illus_alienColony = "";

	// Token: 0x04000D29 RID: 3369
	public string illus_alienNationFounded = "illustrations/Event_AlienNationFounded";

	// Token: 0x04000D2A RID: 3370
	public string illus_alienFaunaSpawn = "illustrations/Event_AlienMegafaunaArmySpawn";

	// Token: 0x04000D2B RID: 3371
	public string illus_xenofaunaArmy = "illustrations/Army_AlienMegafauna";

	// Token: 0x04000D2C RID: 3372
	public string illus_alienArmy = "illustrations/Army_Alien";

	// Token: 0x04000D2D RID: 3373
	public string illus_alienNavyTransport = "illustrations/Army_SeaAlien";

	// Token: 0x04000D2E RID: 3374
	public string illus_assaultStation = "illustrations/Mission_SeizeSpaceAsset";

	// Token: 0x04000D2F RID: 3375
	public string illus_assaultBase = "illustrations/Mission_SeizeSpaceAsset";

	// Token: 0x04000D30 RID: 3376
	public string illus_assaultXenoforming = "illustrations/Mission_AssaultAlienAsset_Xenoforming";

	// Token: 0x04000D31 RID: 3377
	public string illus_assaultXenoFacility = "";

	// Token: 0x04000D32 RID: 3378
	public string illus_alienCrashdown = "illustrations/Event_AlienCrashdown";

	// Token: 0x04000D33 RID: 3379
	public string illus_alienLandedUFOBombed = "illustrations/Event_LandedWarshipDefeated";

	// Token: 0x04000D34 RID: 3380
	public string illus_alienFacilityBombed = "";

	// Token: 0x04000D35 RID: 3381
	public string illus_myCouncilorAssassinated_Earth = "illustrations/Event_MyCouncilorAssassinated_Earth";

	// Token: 0x04000D36 RID: 3382
	public string illus_myCouncilorAssassinated_Earth_alt = "illustrations/Event_MyCouncilorAssassinated_Earth_Alt";

	// Token: 0x04000D37 RID: 3383
	public string illus_myCouncilorAssassinated_Space = "illustrations/Event_MyCouncilorAssassinated_Space";

	// Token: 0x04000D38 RID: 3384
	public string illus_myCouncilorDetected_Earth = "illustrations/Event_MyCouncilorDetected_Earth";

	// Token: 0x04000D39 RID: 3385
	public string illus_spyDiscovered = "illustrations/Event_SpyDiscovered";

	// Token: 0x04000D3A RID: 3386
	public string illus_myCouncilorDetained = "illustrations/Event_MyCouncilorDetained";

	// Token: 0x04000D3B RID: 3387
	public string illus_myOrgStolen = "illustrations/Event_MyOrgStolen";

	// Token: 0x04000D3C RID: 3388
	public string illus_myControlPointCrackdown = "illustrations/Event_HitByCrackdown";

	// Token: 0x04000D3D RID: 3389
	public string illus_myControlPointPurged = "";

	// Token: 0x04000D3E RID: 3390
	public string illus_habLostToPolitics = "illustrations/Event_HabLostToPolitics";

	// Token: 0x04000D3F RID: 3391
	public string illus_habLostToAssault = "illustrations/Event_HabLostToAssault";

	// Token: 0x04000D40 RID: 3392
	public string illus_war = "illustrations/Event_War";

	// Token: 0x04000D41 RID: 3393
	public string illus_peace = "illustrations/Event_Peace";

	// Token: 0x04000D42 RID: 3394
	public string illus_federation = "illustrations/Event_Federate";

	// Token: 0x04000D43 RID: 3395
	public string illus_unification = "illustrations/Event_MergeNations";

	// Token: 0x04000D44 RID: 3396
	public string illus_independence = "illustrations/Event_Independence";

	// Token: 0x04000D45 RID: 3397
	public string illus_nuclearWeaponsLaunch = "illustrations/Event_NuclearWeaponsLaunched";

	// Token: 0x04000D46 RID: 3398
	public string illus_coup = "illustrations/Event_Coup";

	// Token: 0x04000D47 RID: 3399
	public string illus_annexation = "illustrations/Event_Annexation";

	// Token: 0x04000D48 RID: 3400
	public string illus_revolution = "illustrations/Event_Revolution";

	// Token: 0x04000D49 RID: 3401
	public string illus_regimeChange = "illustrations/Event_RegimeChange";

	// Token: 0x04000D4A RID: 3402
	public string illus_nuclearProgram = "illustrations/Event_NuclearWeaponsTest";

	// Token: 0x04000D4B RID: 3403
	public string illus_spaceProgram = "illustrations/Event_SpaceProgramInitiated";

	// Token: 0x04000D4C RID: 3404
	public string illus_probelaunched = "illustrations/Event_ProbeLaunched";

	// Token: 0x04000D4D RID: 3405
	public string illus_BSBE_preCrashIntro = "illustrations/BSBE_Event_broken_earth";

	// Token: 0x04000D4E RID: 3406
	public string[] illus_controlPointPaths = new string[]
	{
		"", "", "illustrations/ControlPoint_Executive", "illustrations/ControlPoint_Legislature", "illustrations/ControlPoint_TheParty", "illustrations/ControlPoint_Oligarchs", "illustrations/ControlPoint_Aristocracy", "illustrations/ControlPoint_NationalIndustries", "illustrations/ControlPoint_Corporations", "illustrations/ControlPoint_TradeUnions",
		"illustrations/ControlPoint_MassMedia", "illustrations/ControlPoint_Religion", "illustrations/ControlPoint_SecurityApparatus", "illustrations/ControlPoint_Bureaucracy", "illustrations/ControlPoint_RegionalAuthorities", "", "illustrations/ControlPoint_Warlords", "illustrations/ControlPoint_FinancialSector", "illustrations/ControlPoint_KnowledgeSector", "illustrations/ControlPoint_DefenseSector",
		"illustrations/ControlPoint_ExtractiveSector", "illustrations/ControlPoint_AgricultureSector"
	};

	// Token: 0x04000D4F RID: 3407
	public Color32[] techColor = new Color32[]
	{
		new Color32(214, 120, 52, byte.MaxValue),
		new Color32(70, 170, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, 196, 64, byte.MaxValue),
		new Color32(72, 201, 142, byte.MaxValue),
		new Color32(74, 110, 58, byte.MaxValue),
		new Color32(214, 110, 170, byte.MaxValue),
		new Color32(120, 220, 235, byte.MaxValue),
		new Color32(155, 110, byte.MaxValue, byte.MaxValue)
	};

	// Token: 0x04000D50 RID: 3408
	public Dictionary<TechCategory, string> gradientTechCategoryPath = new Dictionary<TechCategory, string>(8)
	{
		{
			TechCategory.Energy,
			"ui/EnergyGradient"
		},
		{
			TechCategory.InformationScience,
			"ui/InformationScienceGradient"
		},
		{
			TechCategory.LifeScience,
			"ui/LifeScienceGradient"
		},
		{
			TechCategory.Materials,
			"ui/MaterialsGradient"
		},
		{
			TechCategory.MilitaryScience,
			"ui/MilitaryScienceGradient"
		},
		{
			TechCategory.SocialScience,
			"ui/SocialScienceGradient"
		},
		{
			TechCategory.SpaceScience,
			"ui/SpaceScienceGradient"
		},
		{
			TechCategory.Xenology,
			"ui/XenologyGradient"
		}
	};

	// Token: 0x04000D51 RID: 3409
	public Dictionary<TechCategory, string> illus_techCompletePath = new Dictionary<TechCategory, string>(8)
	{
		{
			TechCategory.Energy,
			"illustrations/TechComplete_Energy"
		},
		{
			TechCategory.InformationScience,
			"illustrations/TechComplete_InformationScience"
		},
		{
			TechCategory.LifeScience,
			"illustrations/TechComplete_LifeScience"
		},
		{
			TechCategory.Materials,
			"illustrations/TechComplete_Materials"
		},
		{
			TechCategory.MilitaryScience,
			"illustrations/TechComplete_MilitaryScience"
		},
		{
			TechCategory.SocialScience,
			"illustrations/TechComplete_SocialScience"
		},
		{
			TechCategory.SpaceScience,
			"illustrations/TechComplete_SpaceScience"
		},
		{
			TechCategory.Xenology,
			"illustrations/TechComplete_Materials"
		}
	};

	// Token: 0x04000D52 RID: 3410
	public Dictionary<TechCategory, string> illus_projectCompletePath = new Dictionary<TechCategory, string>(8)
	{
		{
			TechCategory.Energy,
			"illustrations/ProjectComplete_Energy"
		},
		{
			TechCategory.InformationScience,
			"illustrations/ProjectComplete_InformationScience"
		},
		{
			TechCategory.LifeScience,
			"illustrations/ProjectComplete_LifeScience"
		},
		{
			TechCategory.Materials,
			"illustrations/ProjectComplete_Materials"
		},
		{
			TechCategory.MilitaryScience,
			"illustrations/ProjectComplete_MilitaryScience"
		},
		{
			TechCategory.SocialScience,
			"illustrations/ProjectComplete_SocialScience"
		},
		{
			TechCategory.SpaceScience,
			"illustrations/ProjectComplete_SpaceScience"
		},
		{
			TechCategory.Xenology,
			"illustrations/ProjectComplete_Xenology"
		}
	};

	// Token: 0x04000D53 RID: 3411
	public List<string> illus_EarthStationPaths = new List<string>(2) { "illustrations/illus_StationInterior_Earth_0", "illustrations/illus_StationInterior_Earth_1" };

	// Token: 0x04000D54 RID: 3412
	public List<string> illus_StationInteriorPaths = new List<string>(3) { "illustrations/illus_HabInterior_0", "illustrations/illus_HabInterior_1", "illustrations/illus_HabInterior_2" };

	// Token: 0x04000D55 RID: 3413
	public List<string> illus_BaseInteriorPaths = new List<string>(3) { "illustrations/illus_HabInterior_0", "illustrations/illus_HabInterior_1", "illustrations/illus_HabInterior_2" };

	// Token: 0x04000D56 RID: 3414
	public List<string> illus_ShipInteriorPaths = new List<string>(1) { "illustrations/illus_ShipInterior_0" };

	// Token: 0x04000D57 RID: 3415
	public List<string> illus_UnknownOnEarth = new List<string> { "illustrations/illus_unknown_Earth_0", "illustrations/illus_unknown_Earth_1", "illustrations/illus_unknown_Earth_2", "illustrations/illus_unknown_Earth_3", "illustrations/illus_unknown_Earth_4", "illustrations/illus_unknown_Earth_5", "illustrations/illus_unknown_Earth_6", "illustrations/illus_unknown_Earth_7", "illustrations/illus_unknown_Earth_8", "illustrations/illus_unknown_Earth_9" };

	// Token: 0x04000D58 RID: 3416
	public List<string> illus_detainedEarth = new List<string> { "illustrations/illus_detainedEarth_0", "illustrations/illus_detainedEarth_1", "illustrations/illus_detainedEarth_2", "illustrations/illus_detainedEarth_3", "illustrations/illus_detainedEarth_4", "illustrations/illus_detainedEarth_5", "illustrations/illus_detainedEarth_6" };

	// Token: 0x04000D59 RID: 3417
	public List<string> illus_alienEarth = new List<string>
	{
		"illustrations/earth_alienBackground_0", "illustrations/earth_alienBackground_1", "illustrations/earth_alienBackground_2", "illustrations/earth_alienBackground_3", "illustrations/earth_alienBackground_4", "illustrations/earth_alienBackground_5", "illustrations/earth_alienBackground_6", "illustrations/earth_alienBackground_7", "illustrations/earth_alienBackground_8", "illustrations/earth_alienBackground_9",
		"illustrations/earth_alienBackground_10", "illustrations/earth_alienBackground_11", "illustrations/earth_alienBackground_12"
	};

	// Token: 0x04000D5A RID: 3418
	public List<string> illus_UnknownInSpace = new List<string> { "illustrations/illus_unknown_Earth_0" };

	// Token: 0x04000D5B RID: 3419
	public List<string> illus_loadingScreens = new List<string> { "illustrations/loading_1", "illustrations/loading_2", "illustrations/loading_3", "illustrations/loading_4", "illustrations/loading_5", "illustrations/loading_6", "illustrations/loading_7" };

	// Token: 0x04000D5C RID: 3420
	public List<string> skyboxes = new List<string> { "skyboxes/PrimarySkybox", "skyboxes/AltSkybox1", "skyboxes/AltSkybox2", "skyboxes/AltSkybox3", "skyboxes/AltSkybox4", "skyboxes/RealSkyBox_A" };

	// Token: 0x04000D5D RID: 3421
	public bool debug_ConsoleActive;

	// Token: 0x04000D5E RID: 3422
	public bool debug_fullAIDump;

	// Token: 0x04000D5F RID: 3423
	public bool debug_advancedFactionStart;

	// Token: 0x04000D60 RID: 3424
	public bool debug_spaceDetection;

	// Token: 0x04000D61 RID: 3425
	public bool debug_shipDesignAI;

	// Token: 0x04000D62 RID: 3426
	public bool debug_fullAIKnowledge;

	// Token: 0x04000D63 RID: 3427
	public bool debug_AINeverFleesPrecombat;

	// Token: 0x04000D64 RID: 3428
	public bool debug_AIAlwaysFleesPrecombat;

	// Token: 0x04000D65 RID: 3429
	public bool debug_showAllShipPartsIncludingAlien;

	// Token: 0x04000D66 RID: 3430
	public bool debug_showAllShipParts;

	// Token: 0x04000D67 RID: 3431
	public bool debug_showAllHabParts;

	// Token: 0x04000D68 RID: 3432
	public bool debug_AICombatPathing;

	// Token: 0x04000D69 RID: 3433
	public bool debug_suppressCombatAI;

	// Token: 0x04000D6A RID: 3434
	public int debug_suppressCombatAIAfterXPasses;

	// Token: 0x04000D6B RID: 3435
	public bool debug_suppressSkirmishRotatePlanet;

	// Token: 0x04000D6C RID: 3436
	public float debug_skirmishSpaceBodyZoomMult = 1f;

	// Token: 0x04000D6D RID: 3437
	public bool debug_noMissionFail;

	// Token: 0x04000D6E RID: 3438
	public bool debug_alwaysCritFail;

	// Token: 0x04000D6F RID: 3439
	public bool debug_showHateValues;

	// Token: 0x04000D70 RID: 3440
	public int targetFrameRate = 90;

	// Token: 0x04000D71 RID: 3441
	public bool dontPlayCinematicVideos;

	// Token: 0x04000D72 RID: 3442
	public bool alwaysAllowIncreaseUIScale;

	// Token: 0x04000D73 RID: 3443
	public bool smoothAIMissionPlanning;

	// Token: 0x04000D74 RID: 3444
	public int smoothingMSPerFrame = 16;

	// Token: 0x04000D75 RID: 3445
	public float combatCamera_maxZoom = 2000f;

	// Token: 0x04000D76 RID: 3446
	public float combatCamera_minZoom = 100f;

	// Token: 0x04000D77 RID: 3447
	public float combatCamera_maxPan = 8000f;

	// Token: 0x04000D78 RID: 3448
	public float combatCamera_minPan = -8000f;

	// Token: 0x04000D79 RID: 3449
	public float combatCamera_minCameraMovementSpeed = 3f;

	// Token: 0x04000D7A RID: 3450
	public float combatCamera_maxCameraMovementSpeed = 60f;

	// Token: 0x04000D7B RID: 3451
	public float combatCamera_minScrollSpeedOffset = -400f;

	// Token: 0x04000D7C RID: 3452
	public float combatCamera_maxScrollSpeedOffset = -1000f;

	// Token: 0x04000D7D RID: 3453
	public float combatCamera_mouseRotateSpeedOffset = 0.9f;

	// Token: 0x04000D7E RID: 3454
	public float combatCamera_keyRotateSpeedOffset = 50f;

	// Token: 0x04000D7F RID: 3455
	public float combatCamera_keyZoomSpeed = 0.01f;

	// Token: 0x04000D80 RID: 3456
	public double strategyCamera_DragRateNormal = 1.25;

	// Token: 0x04000D81 RID: 3457
	public double strategyCamera_DragRateSlow = 0.25;

	// Token: 0x04000D82 RID: 3458
	public double strategyCamera_ZoomRateNormal = 5.0;

	// Token: 0x04000D83 RID: 3459
	public double strategyCamera_ZoomRateSlow = 0.5;

	// Token: 0x04000D84 RID: 3460
	public double strategyCamera_ZoomLongDistanceThreshold = 400000000.0;

	// Token: 0x04000D85 RID: 3461
	public double strategyCamera_ZoomMediumDistanceThreshold = 10000000.0;

	// Token: 0x04000D86 RID: 3462
	public double strategyCamera_ZoomShortDistanceThreshold = 20000.0;

	// Token: 0x04000D87 RID: 3463
	public double strategyCamera_ZoomLongDistanceMultiplier = 1.25;

	// Token: 0x04000D88 RID: 3464
	public double strategyCamera_ZoomMediumDistanceMultiplier = 2.75;

	// Token: 0x04000D89 RID: 3465
	public double strategyCamera_ZoomLimit = 0.2;

	// Token: 0x04000D8A RID: 3466
	public double strategyCamera_ZoomLimitEarth = 0.1;

	// Token: 0x04000D8B RID: 3467
	public double strategyCamera_MaxZoomStep = 0.9;

	// Token: 0x04000D8C RID: 3468
	public double strategyCamera_MinDistanceFromCamera = 20.0;

	// Token: 0x04000D8D RID: 3469
	public double strategyCamera_LogScaleDistanceFromCamera = 100.0;

	// Token: 0x04000D8E RID: 3470
	public float distanceToViewSurfaceBases = 9.75f;

	// Token: 0x04000D8F RID: 3471
	public string homeCountryThreeLetterISOCodeOverride;

	// Token: 0x04000D90 RID: 3472
	public int campaignStartSeed = -1;

	// Token: 0x04000D91 RID: 3473
	public string savePath;
}
