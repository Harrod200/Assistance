using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200035A RID: 858
public class TIStartTimeTemplate : TIDataTemplate
{
	// Token: 0x06000F0C RID: 3852 RVA: 0x0004AF64 File Offset: 0x00049164
	public override TIGameState CreateGameState()
	{
		return base.CreateGameState() ?? GameStateManager.CreateNewGameState<TITimeState>();
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x0004AF75 File Offset: 0x00049175
	public TIRegionState InitialCrashdownRegion()
	{
		if (!string.IsNullOrEmpty(this.initialCrashdownRegionTemplateName))
		{
			return GameStateManager.FindByTemplate<TIRegionState>(this.initialCrashdownRegionTemplateName, false);
		}
		return null;
	}

	// Token: 0x04000F00 RID: 3840
	public int year;

	// Token: 0x04000F01 RID: 3841
	public int month;

	// Token: 0x04000F02 RID: 3842
	public int day;

	// Token: 0x04000F03 RID: 3843
	public int hour;

	// Token: 0x04000F04 RID: 3844
	public int minute;

	// Token: 0x04000F05 RID: 3845
	public int second;

	// Token: 0x04000F06 RID: 3846
	public float bonusMoney;

	// Token: 0x04000F07 RID: 3847
	public float bonusInfluence;

	// Token: 0x04000F08 RID: 3848
	public float bonusOps;

	// Token: 0x04000F09 RID: 3849
	public float bonusBoost;

	// Token: 0x04000F0A RID: 3850
	public int bonusMissionControl;

	// Token: 0x04000F0B RID: 3851
	public float bonusWater;

	// Token: 0x04000F0C RID: 3852
	public float bonusVolatiles;

	// Token: 0x04000F0D RID: 3853
	public float bonusMetals;

	// Token: 0x04000F0E RID: 3854
	public float bonusNobles;

	// Token: 0x04000F0F RID: 3855
	public float bonusFissiles;

	// Token: 0x04000F10 RID: 3856
	public float bonusAntimatter;

	// Token: 0x04000F11 RID: 3857
	public float bonusExotics;

	// Token: 0x04000F12 RID: 3858
	public string initialCrashdownRegionTemplateName;

	// Token: 0x04000F13 RID: 3859
	public float initialAtmosphericCO2_ppm = 280f;

	// Token: 0x04000F14 RID: 3860
	public float initialAtmosphericCH4_ppm = 0.75f;

	// Token: 0x04000F15 RID: 3861
	public float initialAtmosphericN2O_ppm = 0.27f;

	// Token: 0x04000F16 RID: 3862
	public float initialStratosphericAerosols_ppm;

	// Token: 0x04000F17 RID: 3863
	public float initialGlobalSeaLevelAnomaly_cm;

	// Token: 0x04000F18 RID: 3864
	public float globalStartingGDPScaling = 1f;

	// Token: 0x04000F19 RID: 3865
	public bool distributeFactionlessHabsAndFleets = true;

	// Token: 0x04000F1A RID: 3866
	public int? initialLooseNukes;

	// Token: 0x04000F1B RID: 3867
	public string[] startingTechs;

	// Token: 0x04000F1C RID: 3868
	public string[] techTreeUIStarters;

	// Token: 0x04000F1D RID: 3869
	public List<string> globalTechsCompleted = new List<string>();

	// Token: 0x04000F1E RID: 3870
	public List<string> projectsCompleted = new List<string>();

	// Token: 0x04000F1F RID: 3871
	public List<string> startingShipDesigns = new List<string>();

	// Token: 0x04000F20 RID: 3872
	public List<string> startingSurveyedSpaceBodies = new List<string>();

	// Token: 0x04000F21 RID: 3873
	public List<string> startingAlienCouncilorFleets = new List<string>();

	// Token: 0x04000F22 RID: 3874
	public float orgGlobalResearchSensitivity;

	// Token: 0x04000F23 RID: 3875
	public float orgGlobalGDPSensitivity;

	// Token: 0x04000F24 RID: 3876
	public float populationRegressionPeriod_years = 20f;

	// Token: 0x04000F25 RID: 3877
	public float alienSurveillanceDelay_years;

	// Token: 0x04000F26 RID: 3878
	public float alienQuietDuration_years;

	// Token: 0x04000F27 RID: 3879
	public float alienSetupDuration_years;

	// Token: 0x04000F28 RID: 3880
	public float alienSetupStartIncome;

	// Token: 0x04000F29 RID: 3881
	public float alienSetupEndIncome;

	// Token: 0x04000F2A RID: 3882
	public float alienProgressionModifier = 1f;

	// Token: 0x04000F2B RID: 3883
	public float alienStartingProgression_years;

	// Token: 0x04000F2C RID: 3884
	public bool scaleCPMaintenanceWithStartingGDP = true;

	// Token: 0x04000F2D RID: 3885
	public bool scaleEconomyDefenseWithStartingGDP = true;

	// Token: 0x04000F2E RID: 3886
	public float GDPDefenseModifier = 1f;

	// Token: 0x04000F2F RID: 3887
	public float CPMaintenanceModifier = 1f;

	// Token: 0x04000F30 RID: 3888
	public bool invasionFocusedAliens;
}
