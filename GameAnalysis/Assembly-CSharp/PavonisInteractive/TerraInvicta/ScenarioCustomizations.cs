using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006ED RID: 1773
	public class ScenarioCustomizations
	{
		// Token: 0x06002942 RID: 10562 RVA: 0x000DBE4C File Offset: 0x000DA04C
		public ScenarioCustomizations Clone()
		{
			return (ScenarioCustomizations)base.MemberwiseClone();
		}

		// Token: 0x04001F94 RID: 8084
		public bool usingCustomizations;

		// Token: 0x04001F95 RID: 8085
		public bool customDifficulty;

		// Token: 0x04001F96 RID: 8086
		public Dictionary<string, ScenarioCustomizations.CustomFactionText> customFactionText = new Dictionary<string, ScenarioCustomizations.CustomFactionText>();

		// Token: 0x04001F97 RID: 8087
		public Dictionary<string, int> customFactionStartingNationGroup = new Dictionary<string, int>();

		// Token: 0x04001F98 RID: 8088
		public List<TICouncilorTypeTemplate> startingCouncilorProfessions = new List<TICouncilorTypeTemplate>();

		// Token: 0x04001F99 RID: 8089
		public List<bool> skipStartingCouncilors = new List<bool> { false, false };

		// Token: 0x04001F9A RID: 8090
		public bool usePlayerCountryForStartingCouncilor = true;

		// Token: 0x04001F9B RID: 8091
		public bool variableProjectUnlocks = true;

		// Token: 0x04001F9C RID: 8092
		public bool showTriggeredProjects;

		// Token: 0x04001F9D RID: 8093
		public bool addAlienAssaultCarrierFleet;

		// Token: 0x04001F9E RID: 8094
		public bool otherFactionStartingNations;

		// Token: 0x04001F9F RID: 8095
		public List<string> selectedFactionsForScenario = new List<string>();

		// Token: 0x04001FA0 RID: 8096
		public float researchSpeedMultiplier = 1f;

		// Token: 0x04001FA1 RID: 8097
		public int controlPointMaintenanceFreebieBonus;

		// Token: 0x04001FA2 RID: 8098
		public int controlPointMaintenanceFreebieBonusAI;

		// Token: 0x04001FA3 RID: 8099
		public float missionControlBonus;

		// Token: 0x04001FA4 RID: 8100
		public float missionControlBonusAI;

		// Token: 0x04001FA5 RID: 8101
		public float alienProgressionSpeed = 1f;

		// Token: 0x04001FA6 RID: 8102
		public float miningProductivityMultiplier = 1f;

		// Token: 0x04001FA7 RID: 8103
		public float nationalIPMultiplier = 1f;

		// Token: 0x04001FA8 RID: 8104
		public int averageMonthlyEvents = 5;

		// Token: 0x04001FA9 RID: 8105
		public bool cinematicCombatRealismDV;

		// Token: 0x04001FAA RID: 8106
		public bool cinematicCombatRealismScale;

		// Token: 0x04001FAB RID: 8107
		public bool canDisableFactions;

		// Token: 0x04001FAC RID: 8108
		public float miningRatePlayer = 1f;

		// Token: 0x04001FAD RID: 8109
		public float miningRateHumanAI = 1f;

		// Token: 0x04001FAE RID: 8110
		public float miningRateAlien = 1f;

		// Token: 0x04001FAF RID: 8111
		public float habConstructionSpeedPlayer = 1f;

		// Token: 0x04001FB0 RID: 8112
		public float habConstructionSpeedHumanAI = 1f;

		// Token: 0x04001FB1 RID: 8113
		public float habConstructionSpeedAlien = 1f;

		// Token: 0x04001FB2 RID: 8114
		public float shipConstructionSpeedPlayer = 1f;

		// Token: 0x04001FB3 RID: 8115
		public float shipConstructionSpeedHumanAI = 1f;

		// Token: 0x04001FB4 RID: 8116
		public float shipConstructionSpeedAlien = 1f;

		// Token: 0x04001FB5 RID: 8117
		public bool randomizeMap;

		// Token: 0x04001FB6 RID: 8118
		public int randomizedMapSeed;

		// Token: 0x02000D11 RID: 3345
		public struct CustomFactionText
		{
			// Token: 0x06006EFE RID: 28414 RVA: 0x0030D9E4 File Offset: 0x0030BBE4
			public CustomFactionText(string customDisplayName, string customAdjective, string customLeaderAddress, string customFleetNameBase, string customSmallShipNameListIdx, string customMediumShipNameListIdx, string customLargeShipNameListIdx, string customHabNameListIdx)
			{
				this.customDisplayName = customDisplayName;
				this.customAdjective = customAdjective;
				this.customLeaderAddress = customLeaderAddress;
				this.customFleetNameBase = customFleetNameBase;
				this.customSmallShipNameListIdx = customSmallShipNameListIdx;
				this.customMediumShipNameListIdx = customMediumShipNameListIdx;
				this.customLargeShipNameListIdx = customLargeShipNameListIdx;
				this.customHabNameListIdx = customHabNameListIdx;
			}

			// Token: 0x04005051 RID: 20561
			public string customDisplayName;

			// Token: 0x04005052 RID: 20562
			public string customAdjective;

			// Token: 0x04005053 RID: 20563
			public string customLeaderAddress;

			// Token: 0x04005054 RID: 20564
			public string customFleetNameBase;

			// Token: 0x04005055 RID: 20565
			public string customSmallShipNameListIdx;

			// Token: 0x04005056 RID: 20566
			public string customMediumShipNameListIdx;

			// Token: 0x04005057 RID: 20567
			public string customLargeShipNameListIdx;

			// Token: 0x04005058 RID: 20568
			public string customHabNameListIdx;
		}
	}
}
