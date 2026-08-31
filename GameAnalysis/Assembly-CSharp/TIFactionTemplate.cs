using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class TIFactionTemplate : TIDataTemplate
{
	// Token: 0x170000C7 RID: 199
	// (get) Token: 0x060005BF RID: 1471 RVA: 0x0001A918 File Offset: 0x00018B18
	public HabPreferences AdjustedHabPreferences
	{
		get
		{
			if (this.adjustedHabPreferences == null)
			{
				if (this.habPreferences != null)
				{
					this.adjustedHabPreferences = this.habPreferences.Copy();
				}
				else
				{
					this.adjustedHabPreferences = new HabPreferences();
				}
				HabPreferences habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Research] = habPreferences[HabMetric.Research] * this.AIValues[0].gatherScience;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Operations] = habPreferences[HabMetric.Operations] * this.AIValues[0].gatherOps;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Influence] = habPreferences[HabMetric.Influence] * this.AIValues[0].gatherInfluence;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Money] = habPreferences[HabMetric.Money] * this.AIValues[0].gatherMoney;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Boost] = habPreferences[HabMetric.Boost] * this.AIValues[0].wantSpaceFacilities;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.MissionControl] = habPreferences[HabMetric.MissionControl] * this.AIValues[0].wantSpaceFacilities;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.SpaceResources] = habPreferences[HabMetric.SpaceResources] * this.AIValues[0].wantSpaceFacilities;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Shipbuilding] = habPreferences[HabMetric.Shipbuilding] * this.AIValues[0].wantSpaceWarCapability;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.LEO] = habPreferences[HabMetric.LEO] * this.AIValues[0].wantSpaceFacilities;
				habPreferences = this.adjustedHabPreferences;
				habPreferences[HabMetric.Defense] = habPreferences[HabMetric.Defense] * this.AIValues[0].wantSpaceWarCapability;
				this.adjustedHabPreferences = this.adjustedHabPreferences.Normalized();
			}
			return this.adjustedHabPreferences;
		}
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x0001AAF8 File Offset: 0x00018CF8
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIFactionState>();
		}
		return tigameState;
	}

	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x060005C1 RID: 1473 RVA: 0x0001AB1C File Offset: 0x00018D1C
	public Color brightColor
	{
		get
		{
			float num = (this.color.r + this.color.g + this.color.b) / 3f;
			if (num >= 0.6f)
			{
				return this.color;
			}
			return Color.Lerp(this.color, Color.white, (0.6f - num) / (1f - num));
		}
	}

	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x060005C2 RID: 1474 RVA: 0x0001AB81 File Offset: 0x00018D81
	public string inlineColorString
	{
		get
		{
			return TIUtilities.GetColorString(this.color);
		}
	}

	// Token: 0x170000CA RID: 202
	// (get) Token: 0x060005C3 RID: 1475 RVA: 0x0001AB8E File Offset: 0x00018D8E
	public string brightInlineColorString
	{
		get
		{
			return TIUtilities.GetColorString(this.brightColor);
		}
	}

	// Token: 0x170000CB RID: 203
	// (get) Token: 0x060005C4 RID: 1476 RVA: 0x0001AB9B File Offset: 0x00018D9B
	public string capitalizedFactionName
	{
		get
		{
			return Utilities.Capitalize(this.displayName);
		}
	}

	// Token: 0x170000CC RID: 204
	// (get) Token: 0x060005C5 RID: 1477 RVA: 0x0001ABA8 File Offset: 0x00018DA8
	public string capitalizedFactionNameCurrent
	{
		get
		{
			return Utilities.Capitalize(base.displayNameCurrentForStartScreen());
		}
	}

	// Token: 0x170000CD RID: 205
	// (get) Token: 0x060005C6 RID: 1478 RVA: 0x0001ABB5 File Offset: 0x00018DB5
	public string adjective
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.adjective.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000CE RID: 206
	// (get) Token: 0x060005C7 RID: 1479 RVA: 0x0001ABD6 File Offset: 0x00018DD6
	public string leaderAddress
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.leaderAddress.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000CF RID: 207
	// (get) Token: 0x060005C8 RID: 1480 RVA: 0x0001ABF7 File Offset: 0x00018DF7
	public string campaignPlayerIntroPath
	{
		get
		{
			return new StringBuilder("TIFactionTemplate.CampaignStart.").Append(base.dataName).ToString();
		}
	}

	// Token: 0x170000D0 RID: 208
	// (get) Token: 0x060005C9 RID: 1481 RVA: 0x0001AC13 File Offset: 0x00018E13
	public string campaignStartHeadline
	{
		get
		{
			return new StringBuilder("TIFactionTemplate.CampaignStartHeadline.").Append(base.dataName).ToString();
		}
	}

	// Token: 0x170000D1 RID: 209
	// (get) Token: 0x060005CA RID: 1482 RVA: 0x0001AC2F File Offset: 0x00018E2F
	public string leaderName
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.leader.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x060005CB RID: 1483 RVA: 0x0001AC50 File Offset: 0x00018E50
	public string leaderBorn
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.leader.born.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x060005CC RID: 1484 RVA: 0x0001AC71 File Offset: 0x00018E71
	public string leaderBackground
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.leader.background.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x060005CD RID: 1485 RVA: 0x0001AC94 File Offset: 0x00018E94
	public string introduction
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.Introduction.").Append(base.dataName).ToString(), new object[]
			{
				this.displayName,
				this.adjective,
				this.leaderAddress,
				new StringBuilder("<align=\"right\">").Append(this.leaderName).ToString(),
				new StringBuilder("<align=\"right\">").Append(this.quote)
			});
		}
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x060005CE RID: 1486 RVA: 0x0001AD16 File Offset: 0x00018F16
	public string goal
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.Goal.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x060005CF RID: 1487 RVA: 0x0001AD37 File Offset: 0x00018F37
	public string victory
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.Victory.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000D7 RID: 215
	// (get) Token: 0x060005D0 RID: 1488 RVA: 0x0001AD58 File Offset: 0x00018F58
	public string quote
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.Quote.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x060005D1 RID: 1489 RVA: 0x0001AD7C File Offset: 0x00018F7C
	public string victoryAnnouncement
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.VictoryNotification.").Append(base.dataName).ToString(), new object[]
			{
				this.leaderAddress,
				this.leaderName,
				this.displayName,
				GameStateManager.AlienFaction().leaderName
			});
		}
	}

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x060005D2 RID: 1490 RVA: 0x0001ADD6 File Offset: 0x00018FD6
	public string winIllustration
	{
		get
		{
			return new StringBuilder("illustrations/Victory_").Append(base.dataName).ToString();
		}
	}

	// Token: 0x170000DA RID: 218
	// (get) Token: 0x060005D3 RID: 1491 RVA: 0x0001ADF2 File Offset: 0x00018FF2
	public string fleetNameBase
	{
		get
		{
			return Loc.T(new StringBuilder("TIFactionTemplate.Fleet.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x170000DB RID: 219
	// (get) Token: 0x060005D4 RID: 1492 RVA: 0x0001AE13 File Offset: 0x00019013
	public TICouncilorAppearanceTemplate leaderAppearance
	{
		get
		{
			return TemplateManager.Find<TICouncilorAppearanceTemplate>(this.leaderDataname, false);
		}
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x0001AE24 File Offset: 0x00019024
	public float GetStartingResource(FactionResource resource)
	{
		return this.startingResources.Where<ResourceValue>((ResourceValue item) => item.resource == resource).Sum<ResourceValue>((ResourceValue item2) => item2.value);
	}

	// Token: 0x170000DC RID: 220
	// (get) Token: 0x060005D6 RID: 1494 RVA: 0x0001AE79 File Offset: 0x00019079
	public string fleetIcon1Resource
	{
		get
		{
			return new StringBuilder(this.fleetIcon).Append("_LVL_1").ToString();
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x060005D7 RID: 1495 RVA: 0x0001AE95 File Offset: 0x00019095
	public string fleetIcon2Resource
	{
		get
		{
			return new StringBuilder(this.fleetIcon).Append("_LVL_2").ToString();
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x060005D8 RID: 1496 RVA: 0x0001AEB1 File Offset: 0x000190B1
	public string fleetIcon3Resource
	{
		get
		{
			return new StringBuilder(this.fleetIcon).Append("_LVL_3").ToString();
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x060005D9 RID: 1497 RVA: 0x0001AECD File Offset: 0x000190CD
	public string leaderDescription
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder("TIFactionTemplate.leader.description.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x0001AEF0 File Offset: 0x000190F0
	public static string GetShipMaterialSuffix(TIFactionState designingFaction)
	{
		string text = designingFaction.template.hullSkinBase;
		if (designingFaction.IsAlienFaction)
		{
			text = GameStateManager.AlienProxy().template.hullSkinBase;
		}
		else
		{
			string text2 = designingFaction.template.shipMaterialBundlePath;
			foreach (TIFactionTemplate tifactionTemplate in TemplateManager.IterateByClass<TIFactionTemplate>(true))
			{
				if (text2.Contains(tifactionTemplate.hullSkinBase))
				{
					text = tifactionTemplate.hullSkinBase;
					break;
				}
			}
		}
		return text;
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x0001AF80 File Offset: 0x00019180
	public string GetShipMaterialBundlePath(int hullIndex)
	{
		if (hullIndex < 2)
		{
			return this.shipMaterialBundlePath;
		}
		if (hullIndex == 2 || hullIndex == 3)
		{
			return this.shipMaterialBundlePath + TIUtilities.GetContentBundleSuffix(hullIndex);
		}
		return this.shipMaterialBundlePath;
	}

	// Token: 0x040005C3 RID: 1475
	public Color color;

	// Token: 0x040005C4 RID: 1476
	public float colorIntensity;

	// Token: 0x040005C5 RID: 1477
	public string backgroundColor;

	// Token: 0x040005C6 RID: 1478
	public string ideologyName;

	// Token: 0x040005C7 RID: 1479
	public bool isAlien;

	// Token: 0x040005C8 RID: 1480
	public bool activePlayerAllowed;

	// Token: 0x040005C9 RID: 1481
	public bool tutorialAllowed;

	// Token: 0x040005CA RID: 1482
	public bool allowedSoleAntiAlien;

	// Token: 0x040005CB RID: 1483
	public bool defaultAntiAlien;

	// Token: 0x040005CC RID: 1484
	public string victoryTemplateName;

	// Token: 0x040005CD RID: 1485
	public string spaceOrg;

	// Token: 0x040005CE RID: 1486
	public string winningOrg;

	// Token: 0x040005CF RID: 1487
	public string hullSkinBase;

	// Token: 0x040005D0 RID: 1488
	public string armySkinBase;

	// Token: 0x040005D1 RID: 1489
	public string leaderDataname;

	// Token: 0x040005D2 RID: 1490
	public string defaultPresetName;

	// Token: 0x040005D3 RID: 1491
	public int difficulty;

	// Token: 0x040005D4 RID: 1492
	public int playerMood;

	// Token: 0x040005D5 RID: 1493
	public int encMood;

	// Token: 0x040005D6 RID: 1494
	public int hullIndex_default;

	// Token: 0x040005D7 RID: 1495
	public int hullIndex_chem;

	// Token: 0x040005D8 RID: 1496
	public int hullIndex_electric;

	// Token: 0x040005D9 RID: 1497
	public int hullIndex_fission;

	// Token: 0x040005DA RID: 1498
	public int hullIndex_fusion;

	// Token: 0x040005DB RID: 1499
	public int hullIndex_fusion_adv;

	// Token: 0x040005DC RID: 1500
	public int hullIndex_amat;

	// Token: 0x040005DD RID: 1501
	public string councilIcon64;

	// Token: 0x040005DE RID: 1502
	public string councilIcon64_ui;

	// Token: 0x040005DF RID: 1503
	public string councilIcon128;

	// Token: 0x040005E0 RID: 1504
	public string councilIcon128_ui;

	// Token: 0x040005E1 RID: 1505
	public string councilIcon256;

	// Token: 0x040005E2 RID: 1506
	public string councilIcon256_ui;

	// Token: 0x040005E3 RID: 1507
	public string fleetIcon;

	// Token: 0x040005E4 RID: 1508
	public string stationIcon;

	// Token: 0x040005E5 RID: 1509
	public string baseIcon;

	// Token: 0x040005E6 RID: 1510
	public string genericCouncilorIcon;

	// Token: 0x040005E7 RID: 1511
	public string habSectorIcon;

	// Token: 0x040005E8 RID: 1512
	public string cursorPath;

	// Token: 0x040005E9 RID: 1513
	public string cinematicsPath;

	// Token: 0x040005EA RID: 1514
	public string gradientPath;

	// Token: 0x040005EB RID: 1515
	public string winMissionPath;

	// Token: 0x040005EC RID: 1516
	public string fanfarePath;

	// Token: 0x040005ED RID: 1517
	public string shipMaterialBundlePath;

	// Token: 0x040005EE RID: 1518
	public List<ResourceValue> startingResources = new List<ResourceValue>();

	// Token: 0x040005EF RID: 1519
	public List<ResourceValue> baseAnnualIncomes = new List<ResourceValue>();

	// Token: 0x040005F0 RID: 1520
	public List<AIValues> AIValues = new List<AIValues>();

	// Token: 0x040005F1 RID: 1521
	public string smallShipNameListIdx = "Alien1";

	// Token: 0x040005F2 RID: 1522
	public string mediumShipNameListIdx = "Alien1";

	// Token: 0x040005F3 RID: 1523
	public string largeShipNameListIdx = "Alien1";

	// Token: 0x040005F4 RID: 1524
	public string habNameListIdx = "Alien1";

	// Token: 0x040005F5 RID: 1525
	public List<List<string>> guaranteedMissions = new List<List<string>>();

	// Token: 0x040005F6 RID: 1526
	public List<string> firstTechNames = new List<string>();

	// Token: 0x040005F7 RID: 1527
	public List<string> winnerTechNames = new List<string>();

	// Token: 0x040005F8 RID: 1528
	public HabPreferences habPreferences;

	// Token: 0x040005F9 RID: 1529
	private HabPreferences adjustedHabPreferences;
}
