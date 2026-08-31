using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FullSerializer;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x02000764 RID: 1892
	public class TIOrgState : TIGameState
	{
		// Token: 0x17000860 RID: 2144
		// (get) Token: 0x0600352A RID: 13610 RVA: 0x00130B45 File Offset: 0x0012ED45
		// (set) Token: 0x0600352B RID: 13611 RVA: 0x00130B4D File Offset: 0x0012ED4D
		[SerializeField]
		public string orgIconTemplateName { get; private set; }

		// Token: 0x17000861 RID: 2145
		// (get) Token: 0x0600352C RID: 13612 RVA: 0x00130B56 File Offset: 0x0012ED56
		// (set) Token: 0x0600352D RID: 13613 RVA: 0x00130B5E File Offset: 0x0012ED5E
		[SerializeField]
		public string orgIconPath { get; private set; }

		// Token: 0x17000862 RID: 2146
		// (get) Token: 0x0600352E RID: 13614 RVA: 0x00130B67 File Offset: 0x0012ED67
		// (set) Token: 0x0600352F RID: 13615 RVA: 0x00130B6F File Offset: 0x0012ED6F
		public string displayNameWithArticle { get; private set; }

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x06003530 RID: 13616 RVA: 0x00130B78 File Offset: 0x0012ED78
		public float adjustedIncomeMoney_month
		{
			get
			{
				float num = this.incomeMoney_month;
				float orgGlobalGDPSensitivity = GameStateManager.Time().template.orgGlobalGDPSensitivity;
				if (orgGlobalGDPSensitivity > 0f)
				{
					float globalGDPFractionOfBaseline = TIGlobalValuesState.globalGDPFractionOfBaseline;
					num = (float)(num * Mathf.Lerp(1f, globalGDPFractionOfBaseline, orgGlobalGDPSensitivity)).RoundUp();
				}
				return num;
			}
		}

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06003531 RID: 13617 RVA: 0x00130BC0 File Offset: 0x0012EDC0
		public float adjustedIncomeInfluence_month
		{
			get
			{
				return this.incomeInfluence_month;
			}
		}

		// Token: 0x17000865 RID: 2149
		// (get) Token: 0x06003532 RID: 13618 RVA: 0x00130BC8 File Offset: 0x0012EDC8
		public float adjustedIncomeOps_month
		{
			get
			{
				return this.incomeOps_month;
			}
		}

		// Token: 0x17000866 RID: 2150
		// (get) Token: 0x06003533 RID: 13619 RVA: 0x00130BD0 File Offset: 0x0012EDD0
		public float adjustedIncomeBoost_month
		{
			get
			{
				return this.incomeBoost_month;
			}
		}

		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x06003534 RID: 13620 RVA: 0x00130BD8 File Offset: 0x0012EDD8
		public float adjustedIncomeResearch_month
		{
			get
			{
				float num = this.incomeResearch_month;
				float orgGlobalResearchSensitivity = GameStateManager.Time().template.orgGlobalResearchSensitivity;
				if (orgGlobalResearchSensitivity > 0f)
				{
					float globalResearchFractionOfBaseline = TIGlobalValuesState.globalResearchFractionOfBaseline;
					num = (float)(num * Mathf.Lerp(1f, globalResearchFractionOfBaseline, orgGlobalResearchSensitivity)).RoundUp();
				}
				return num;
			}
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x06003535 RID: 13621 RVA: 0x00130C20 File Offset: 0x0012EE20
		public bool grantsMarked
		{
			get
			{
				return this.template.grantsMarked;
			}
		}

		// Token: 0x17000869 RID: 2153
		// (get) Token: 0x06003536 RID: 13622 RVA: 0x00130C2D File Offset: 0x0012EE2D
		// (set) Token: 0x06003537 RID: 13623 RVA: 0x00130C35 File Offset: 0x0012EE35
		public bool applyingBonuses { get; private set; }

		// Token: 0x1700086A RID: 2154
		// (get) Token: 0x06003538 RID: 13624 RVA: 0x00130C3E File Offset: 0x0012EE3E
		// (set) Token: 0x06003539 RID: 13625 RVA: 0x00130C46 File Offset: 0x0012EE46
		public TICouncilorState assignedCouncilor { get; private set; }

		// Token: 0x1700086B RID: 2155
		// (get) Token: 0x0600353A RID: 13626 RVA: 0x00130C4F File Offset: 0x0012EE4F
		// (set) Token: 0x0600353B RID: 13627 RVA: 0x00130C57 File Offset: 0x0012EE57
		public TIFactionState factionOrbit { get; private set; }

		// Token: 0x1700086C RID: 2156
		// (get) Token: 0x0600353C RID: 13628 RVA: 0x00130C60 File Offset: 0x0012EE60
		// (set) Token: 0x0600353D RID: 13629 RVA: 0x00130C68 File Offset: 0x0012EE68
		[fsIgnore]
		public TIOrgIconTemplate orgIconTemplate { get; private set; }

		// Token: 0x1700086D RID: 2157
		// (get) Token: 0x0600353E RID: 13630 RVA: 0x00130C71 File Offset: 0x0012EE71
		// (set) Token: 0x0600353F RID: 13631 RVA: 0x00130C79 File Offset: 0x0012EE79
		[fsIgnore]
		public List<TITraitTemplate> requiredOwnerTraits { get; private set; }

		// Token: 0x1700086E RID: 2158
		// (get) Token: 0x06003540 RID: 13632 RVA: 0x00130C82 File Offset: 0x0012EE82
		// (set) Token: 0x06003541 RID: 13633 RVA: 0x00130C8A File Offset: 0x0012EE8A
		[fsIgnore]
		public List<TITraitTemplate> prohibitedOwnerTraits { get; private set; }

		// Token: 0x1700086F RID: 2159
		// (get) Token: 0x06003542 RID: 13634 RVA: 0x00130C93 File Offset: 0x0012EE93
		// (set) Token: 0x06003543 RID: 13635 RVA: 0x00130C9B File Offset: 0x0012EE9B
		public TIRegionState homeRegion { get; private set; }

		// Token: 0x17000870 RID: 2160
		// (get) Token: 0x06003544 RID: 13636 RVA: 0x00130CA4 File Offset: 0x0012EEA4
		public override bool isOrgState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000871 RID: 2161
		// (get) Token: 0x06003545 RID: 13637 RVA: 0x00130CA7 File Offset: 0x0012EEA7
		public override Searchable searchable
		{
			get
			{
				return Searchable.withIntel;
			}
		}

		// Token: 0x17000872 RID: 2162
		// (get) Token: 0x06003546 RID: 13638 RVA: 0x00130CAA File Offset: 0x0012EEAA
		public override TIFactionState ref_faction
		{
			get
			{
				return this.factionOrbit;
			}
		}

		// Token: 0x17000873 RID: 2163
		// (get) Token: 0x06003547 RID: 13639 RVA: 0x00130CB2 File Offset: 0x0012EEB2
		public override TINationState ref_nation
		{
			get
			{
				return this.homeNation;
			}
		}

		// Token: 0x17000874 RID: 2164
		// (get) Token: 0x06003548 RID: 13640 RVA: 0x00130CBA File Offset: 0x0012EEBA
		public override TIRegionState ref_region
		{
			get
			{
				TIRegionState tiregionState;
				if ((tiregionState = this.homeRegion) == null)
				{
					TICouncilorState assignedCouncilor = this.assignedCouncilor;
					if (assignedCouncilor == null)
					{
						return null;
					}
					tiregionState = assignedCouncilor.ref_region;
				}
				return tiregionState;
			}
		}

		// Token: 0x17000875 RID: 2165
		// (get) Token: 0x06003549 RID: 13641 RVA: 0x00130CD7 File Offset: 0x0012EED7
		public override TISpaceBodyState ref_spaceBody
		{
			get
			{
				TIRegionState ref_region = this.ref_region;
				if (ref_region == null)
				{
					return null;
				}
				return ref_region.spaceBody;
			}
		}

		// Token: 0x17000876 RID: 2166
		// (get) Token: 0x0600354A RID: 13642 RVA: 0x00130CEA File Offset: 0x0012EEEA
		public override TICouncilorState ref_councilor
		{
			get
			{
				return this.assignedCouncilor;
			}
		}

		// Token: 0x17000877 RID: 2167
		// (get) Token: 0x0600354B RID: 13643 RVA: 0x00130CF2 File Offset: 0x0012EEF2
		public override TIOrgState ref_org
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000878 RID: 2168
		// (get) Token: 0x0600354C RID: 13644 RVA: 0x00130CF5 File Offset: 0x0012EEF5
		public override bool hasMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000879 RID: 2169
		// (get) Token: 0x0600354D RID: 13645 RVA: 0x00130CF8 File Offset: 0x0012EEF8
		public override bool hasEarthMapObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x0600354E RID: 13646 RVA: 0x00130CFB File Offset: 0x0012EEFB
		public TIOrgTemplate template
		{
			get
			{
				return this.GetMyTemplate<TIOrgTemplate>();
			}
		}

		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x0600354F RID: 13647 RVA: 0x00130D03 File Offset: 0x0012EF03
		public TechBonus[] techBonuses
		{
			get
			{
				return this.template.techBonuses.Where<TechBonus>((TechBonus x) => x.bonus > 0f).ToArray<TechBonus>();
			}
		}

		// Token: 0x1700087C RID: 2172
		// (get) Token: 0x06003550 RID: 13648 RVA: 0x00130D39 File Offset: 0x0012EF39
		public bool hasCouncilor
		{
			get
			{
				return this.assignedCouncilor != null;
			}
		}

		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06003551 RID: 13649 RVA: 0x00130D47 File Offset: 0x0012EF47
		public bool hasFactionbutNoCouncilor
		{
			get
			{
				return this.unassignedCouncil != null;
			}
		}

		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06003552 RID: 13650 RVA: 0x00130D55 File Offset: 0x0012EF55
		public TIProjectTemplate projectGranted
		{
			get
			{
				return this.template.projectGranted;
			}
		}

		// Token: 0x1700087F RID: 2175
		// (get) Token: 0x06003553 RID: 13651 RVA: 0x00130D62 File Offset: 0x0012EF62
		public OrgType orgType
		{
			get
			{
				return this.template.orgType;
			}
		}

		// Token: 0x17000880 RID: 2176
		// (get) Token: 0x06003554 RID: 13652 RVA: 0x00130D6F File Offset: 0x0012EF6F
		public bool requiresNationInterest
		{
			get
			{
				return this.template.requiresNationality;
			}
		}

		// Token: 0x17000881 RID: 2177
		// (get) Token: 0x06003555 RID: 13653 RVA: 0x00130D7C File Offset: 0x0012EF7C
		public TIFactionState unassignedCouncil
		{
			get
			{
				if (!(this.factionOrbit != null) || !this.factionOrbit.unassignedOrgs.Contains(this))
				{
					return null;
				}
				return this.factionOrbit;
			}
		}

		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x06003556 RID: 13654 RVA: 0x00130DA7 File Offset: 0x0012EFA7
		public TINationState homeNation
		{
			get
			{
				TIRegionState homeRegion = this.homeRegion;
				if (homeRegion == null)
				{
					return null;
				}
				return homeRegion.nation;
			}
		}

		// Token: 0x17000883 RID: 2179
		// (get) Token: 0x06003557 RID: 13655 RVA: 0x00130DBA File Offset: 0x0012EFBA
		public string displayNameWithArticleCapitalized
		{
			get
			{
				return Utilities.Capitalize(this.displayNameWithArticle);
			}
		}

		// Token: 0x06003558 RID: 13656 RVA: 0x00130DC8 File Offset: 0x0012EFC8
		public override void InitWithTemplate(TIDataTemplate rawTemplate)
		{
			if (!this.gameStateSubjectCreated)
			{
				TIOrgState.<>c__DisplayClass134_0 CS$<>8__locals1 = new TIOrgState.<>c__DisplayClass134_0();
				CS$<>8__locals1.<>4__this = this;
				base.InitWithTemplate(rawTemplate);
				this.templateName = rawTemplate.dataName;
				TIOrgState.<>c__DisplayClass134_0 CS$<>8__locals2 = CS$<>8__locals1;
				TIOrgTemplate tiorgTemplate = rawTemplate as TIOrgTemplate;
				if (tiorgTemplate == null)
				{
					throw new Exception("Invalid template type: " + rawTemplate.GetType().Name);
				}
				CS$<>8__locals2.template = tiorgTemplate;
				if (CS$<>8__locals1.template.randomized)
				{
					int i = 0;
					while (i <= 10)
					{
						bool flag = true;
						i++;
						OrgName orgName = new OrgName(CS$<>8__locals1.template.orgType, "first");
						OrgName orgName2 = new OrgName(CS$<>8__locals1.template.orgType, "second");
						string text;
						if (!GameControl.namelists.TryGetName<OrgName>(orgName, out text))
						{
							Error.Log(string.Format("Error getting org first name for {0}", CS$<>8__locals1.template.orgType), Array.Empty<object>());
						}
						string text2;
						if (!GameControl.namelists.TryGetName<OrgName>(orgName2, out text2))
						{
							Error.Log(string.Format("Error getting org second name for {0}", CS$<>8__locals1.template.orgType), Array.Empty<object>());
						}
						using (IEnumerator<TIOrgState> enumerator = (from x in GameStateManager.IterateByClass<TIOrgState>(false)
							where x != this
							select x).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								if (enumerator.Current.displayName.Contains(text))
								{
									flag = false;
									break;
								}
							}
						}
						if (flag || i >= 10)
						{
							if (!text2.Contains("/"))
							{
								this.displayName = Loc.T("TIOrgTemplate.displayName.generic", new object[] { text, text2 });
								this.displayNameWithArticle = this.displayName;
								break;
							}
							string[] array = text2.Split(new char[] { '/' });
							if (char.IsUpper(array[0][0]))
							{
								this.displayName = Loc.T("TIOrgTemplate.displayName.genericWithArticle", new object[]
								{
									array[0],
									text,
									array[1]
								});
								this.displayNameWithArticle = this.displayName;
								break;
							}
							this.displayName = Loc.T("TIOrgTemplate.displayName.generic", new object[]
							{
								text,
								array[1]
							});
							this.displayNameWithArticle = Loc.T("TIOrgTemplate.displayName.genericWithArticle", new object[]
							{
								array[0],
								text,
								array[1]
							});
							break;
						}
					}
				}
				else
				{
					this.displayName = CS$<>8__locals1.template.displayName;
					this.displayNameWithArticle = CS$<>8__locals1.template.displayNameWithArticle;
				}
				this.tier = CS$<>8__locals1.template.tier;
				if (string.IsNullOrEmpty(CS$<>8__locals1.template.iconResource))
				{
					this.orgIconPath = "placeholders/icon_WIP";
					List<TIOrgIconTemplate> list = TemplateManager.IterateByClass<TIOrgIconTemplate>(true).ToList<TIOrgIconTemplate>();
					List<TIOrgIconTemplate> list2 = list.Where<TIOrgIconTemplate>((TIOrgIconTemplate x) => x.primaryOrgType == CS$<>8__locals1.template.orgType).ToList<TIOrgIconTemplate>();
					IEnumerable<string> usedIconTemplates = from x in GameStateManager.IterateByClass<TIOrgState>(false)
						select x.orgIconTemplateName;
					list2.RemoveAll((TIOrgIconTemplate x) => usedIconTemplates.Contains(x.dataName));
					if (list2.Count > 0)
					{
						this.orgIconTemplate = list2.SelectRandomItem<TIOrgIconTemplate>();
					}
					else
					{
						this.orgIconTemplate = list.FirstOrDefault<TIOrgIconTemplate>((TIOrgIconTemplate x) => x.ValidIconForOrg(CS$<>8__locals1.<>4__this.displayName, CS$<>8__locals1.template.orgType, CS$<>8__locals1.<>4__this.tier) && !usedIconTemplates.Contains(x.dataName));
						if (this.orgIconTemplate == null)
						{
							Log.Warn("Could not find unique icon for " + CS$<>8__locals1.template.orgType.ToString() + " T" + this.tier.ToString(), Array.Empty<object>());
							this.orgIconTemplate = list.Where<TIOrgIconTemplate>((TIOrgIconTemplate x) => x.ValidIconForOrg(CS$<>8__locals1.<>4__this.displayName, CS$<>8__locals1.template.orgType, CS$<>8__locals1.<>4__this.tier)).SelectRandomItem<TIOrgIconTemplate>();
							if (this.orgIconTemplate == null)
							{
								this.orgIconTemplate = list.SelectRandomItem<TIOrgIconTemplate>();
							}
						}
					}
					if (this.orgIconTemplate != null)
					{
						this.orgIconPath = this.orgIconTemplate.path;
					}
				}
				else
				{
					this.orgIconPath = CS$<>8__locals1.template.iconResource;
				}
				this.costMoney = CS$<>8__locals1.template.costMoney + (float)((CS$<>8__locals1.template.randCostMoney > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randCostMoney + 1) : 0);
				this.costInfluence = CS$<>8__locals1.template.costInfluence + (float)((CS$<>8__locals1.template.randCostInfluence > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randCostInfluence + 1) : 0);
				this.costOps = CS$<>8__locals1.template.costOps + (float)((CS$<>8__locals1.template.randCostOps > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randCostOps + 1) : 0);
				this.costBoost = CS$<>8__locals1.template.costBoost + (float)((CS$<>8__locals1.template.randCostBoost > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randCostBoost + 1) : 0);
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceIncomeMoney)
				{
					this.incomeMoney_month = CS$<>8__locals1.template.incomeMoney + (float)((CS$<>8__locals1.template.randIncomeMoney > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randIncomeMoney + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceIncomeInfluence)
				{
					this.incomeInfluence_month = CS$<>8__locals1.template.incomeInfluence + (float)((CS$<>8__locals1.template.randIncomeInfluence > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randIncomeInfluence + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceIncomeOps)
				{
					this.incomeOps_month = CS$<>8__locals1.template.incomeOps + (float)((CS$<>8__locals1.template.randIncomeOps > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randIncomeOps + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceIncomeResearch)
				{
					this.incomeResearch_month = CS$<>8__locals1.template.incomeResearch + (float)((CS$<>8__locals1.template.randIncomeResearch > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randIncomeResearch + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceIncomeBoost)
				{
					this.incomeBoost_month = CS$<>8__locals1.template.incomeBoost + (float)((CS$<>8__locals1.template.randIncomeBoost > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randIncomeBoost + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceIncomeMissionControl)
				{
					this.incomeMissionControl = CS$<>8__locals1.template.incomeMissionControl + (float)((CS$<>8__locals1.template.randIncomeMissionControl > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randIncomeMissionControl + 1) : 0);
				}
				this.projectCapacityGranted = CS$<>8__locals1.template.projectsGranted;
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chancePersuasion)
				{
					this.persuasion = CS$<>8__locals1.template.persuasion + ((CS$<>8__locals1.template.randPersuasion > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randPersuasion + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceCommand)
				{
					this.command = CS$<>8__locals1.template.command + ((CS$<>8__locals1.template.randCommand > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randCommand + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceInvestigation)
				{
					this.investigation = CS$<>8__locals1.template.investigation + ((CS$<>8__locals1.template.randInvestigation > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randInvestigation + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceEspionage)
				{
					this.espionage = CS$<>8__locals1.template.espionage + ((CS$<>8__locals1.template.randEspionage > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randEspionage + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceAdministration)
				{
					this.administration = CS$<>8__locals1.template.administration + ((CS$<>8__locals1.template.randAdministration > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randAdministration + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceSecurity)
				{
					this.security = CS$<>8__locals1.template.security + ((CS$<>8__locals1.template.randSecurity > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randSecurity + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceScience)
				{
					this.science = CS$<>8__locals1.template.science + ((CS$<>8__locals1.template.randScience > 0) ? TIUtilities.RandomRange(0, CS$<>8__locals1.template.randScience + 1) : 0);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceEconomyBonus)
				{
					this.economyBonus = (float)Math.Round((double)(CS$<>8__locals1.template.economyBonus + ((CS$<>8__locals1.template.randEconomyBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randEconomyBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceWelfareBonus)
				{
					this.welfareBonus = (float)Math.Round((double)(CS$<>8__locals1.template.welfareBonus + ((CS$<>8__locals1.template.randWelfareBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randWelfareBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceEnvironmentBonus)
				{
					this.environmentBonus = (float)Math.Round((double)(CS$<>8__locals1.template.environmentBonus + ((CS$<>8__locals1.template.randEnvironmentBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randEnvironmentBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceKnowledgeBonus)
				{
					this.knowledgeBonus = (float)Math.Round((double)(CS$<>8__locals1.template.knowledgeBonus + ((CS$<>8__locals1.template.randKnowledgeBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randKnowledgeBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceGovernmentBonus)
				{
					this.governmentBonus = (float)Math.Round((double)(CS$<>8__locals1.template.governmentBonus + ((CS$<>8__locals1.template.randGovernmentBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randGovernmentBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceUnityBonus)
				{
					this.unityBonus = (float)Math.Round((double)(CS$<>8__locals1.template.unityBonus + ((CS$<>8__locals1.template.randUnityBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randUnityBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceMilitaryBonus)
				{
					this.militaryBonus = (float)Math.Round((double)(CS$<>8__locals1.template.militaryBonus + ((CS$<>8__locals1.template.randMilitaryBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randMilitaryBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceSpoilsBonus)
				{
					this.spoilsBonus = (float)Math.Round((double)(CS$<>8__locals1.template.spoilsBonus + ((CS$<>8__locals1.template.randSpoilsBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randSpoilsBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceOppressionBonus)
				{
					this.oppressionBonus = (float)Math.Round((double)(CS$<>8__locals1.template.oppressionBonus + ((CS$<>8__locals1.template.randOppressionBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randOppressionBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceSpaceDevBonus)
				{
					this.spaceDevBonus = (float)Math.Round((double)(CS$<>8__locals1.template.spaceDevBonus + ((CS$<>8__locals1.template.randSpaceDevBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randSpaceDevBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceSpaceflightBonus)
				{
					this.spaceflightBonus = (float)Math.Round((double)(CS$<>8__locals1.template.spaceflightBonus + ((CS$<>8__locals1.template.randSpaceflightBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randSpaceflightBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceMCBonus)
				{
					this.MCBonus = (float)Math.Round((double)(CS$<>8__locals1.template.MCBonus + ((CS$<>8__locals1.template.randMCBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randMCBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				if ((float)Mathd.d100() <= CS$<>8__locals1.template.chanceMiningBonus)
				{
					this.miningBonus = (float)Math.Round((double)(CS$<>8__locals1.template.miningBonus + ((CS$<>8__locals1.template.randMiningBonus > 0f) ? TIUtilities.RandomRange(0f, CS$<>8__locals1.template.randMiningBonus) : 0f)), 2, MidpointRounding.ToEven);
				}
				this.XPModifier = CS$<>8__locals1.template.XPModifier;
				this.SetHomeRegion(null);
				this.takeoverDefense = CS$<>8__locals1.template.takeoverDefense;
			}
		}

		// Token: 0x06003559 RID: 13657 RVA: 0x00131B74 File Offset: 0x0012FD74
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			this.gameStateSubjectCreated = true;
		}

		// Token: 0x0600355A RID: 13658 RVA: 0x00131B80 File Offset: 0x0012FD80
		public void SetHomeRegion(TIRegionState overrideRegion = null)
		{
			if (overrideRegion != null)
			{
				this.homeRegion = overrideRegion;
			}
			else
			{
				this.homeRegion = this.SelectHomeRegion();
			}
			if (!this.template.randomized && this.template.requiresNationality && this.homeRegion == null)
			{
				TICouncilorState assignedCouncilor = this.assignedCouncilor;
				if (assignedCouncilor != null)
				{
					assignedCouncilor.orgs.Remove(this);
				}
				this.assignedCouncilor = null;
				base.ArchiveState(true);
				GameStateManager.RemoveGameState<TIOrgState>(base.ID, false);
			}
		}

		// Token: 0x0600355B RID: 13659 RVA: 0x00131C08 File Offset: 0x0012FE08
		public TIRegionState SelectHomeRegion()
		{
			if (!string.IsNullOrEmpty(this.template.homeRegionMapTemplateName))
			{
				if (TemplateManager.Find<TIMapRegionTemplate>(this.template.homeRegionMapTemplateName, false) == null)
				{
					return null;
				}
				TIRegionState tiregionState = GameStateManager.MapRegionLookup(this.template.homeRegionMapTemplateName);
				if (tiregionState != null)
				{
					return tiregionState;
				}
			}
			if (this.requiredNationInterest != null)
			{
				if (this.requiredNationInterest.extant)
				{
					return this.requiredNationInterest.capital;
				}
				return this.requiredNationInterest.originalCapital;
			}
			else
			{
				if (this.template.orgType == OrgType.Faction)
				{
					return null;
				}
				Dictionary<TIRegionState, float> dictionary = TIRegionState.GlobalGDPProportions();
				foreach (TIRegionState tiregionState2 in dictionary.Keys.ToList<TIRegionState>())
				{
					if (tiregionState2.nation.alienNation)
					{
						Dictionary<TIRegionState, float> dictionary2 = dictionary;
						TIRegionState tiregionState3 = tiregionState2;
						dictionary2[tiregionState3] /= 1E+12f;
					}
					else
					{
						if (this.tier >= 2 && tiregionState2.colonyRegion)
						{
							List<TITraitTemplate> requiredOwnerTraits = this.requiredOwnerTraits;
							bool flag;
							if (requiredOwnerTraits == null)
							{
								flag = true;
							}
							else
							{
								flag = requiredOwnerTraits.None<TITraitTemplate>((TITraitTemplate x) => x.isCriminalTrait);
							}
							if (flag)
							{
								Dictionary<TIRegionState, float> dictionary2 = dictionary;
								TIRegionState tiregionState3 = tiregionState2;
								dictionary2[tiregionState3] /= 1E+12f;
								continue;
							}
						}
						if (this.orgType == OrgType.Resource && !tiregionState2.coreResourceRegion)
						{
							Dictionary<TIRegionState, float> dictionary2 = dictionary;
							TIRegionState tiregionState3 = tiregionState2;
							dictionary2[tiregionState3] /= 1E+12f;
						}
						else if (this.tier >= 3 && !tiregionState2.coreEconomicRegion)
						{
							if (this.requiredOwnerTraits != null)
							{
								if (!this.requiredOwnerTraits.None<TITraitTemplate>((TITraitTemplate x) => x.isCriminalTrait && x.isGovernmentTrait))
								{
									continue;
								}
							}
							Dictionary<TIRegionState, float> dictionary2 = dictionary;
							TIRegionState tiregionState3 = tiregionState2;
							dictionary2[tiregionState3] /= 1E+12f;
						}
					}
				}
				return dictionary.SelectRandomWeightedItem<KeyValuePair<TIRegionState, float>>((KeyValuePair<TIRegionState, float> k) => k.Value, -1f, 1E-37f).Key;
			}
		}

		// Token: 0x0600355C RID: 13660 RVA: 0x00131E5C File Offset: 0x0013005C
		public void InitRunTimeValues()
		{
			this.missionsGranted = this.template.missionsGranted.ToList<TIMissionTemplate>();
			this.requiredOwnerTraits = new List<TITraitTemplate>(this.template.requiredTraitTemplates);
			this.affinities = new List<FactionIdeology>();
			this.restrictedIdeologies = new List<FactionIdeology>();
			this.orgIconTemplate = (string.IsNullOrEmpty(this.orgIconTemplateName) ? null : TemplateManager.Find<TIOrgIconTemplate>(this.orgIconTemplateName, false));
			this.prohibitedOwnerTraits = new List<TITraitTemplate>(this.template.prohibitedTraitTemplates);
			foreach (FactionIdeology factionIdeology in this.template.affinities)
			{
				if (factionIdeology != FactionIdeology.None)
				{
					this.affinities.Add(factionIdeology);
				}
			}
			foreach (FactionIdeology factionIdeology2 in this.template.restricted)
			{
				if (factionIdeology2 != FactionIdeology.None)
				{
					this.restrictedIdeologies.Add(factionIdeology2);
				}
			}
		}

		// Token: 0x0600355D RID: 13661 RVA: 0x00131F88 File Offset: 0x00130188
		public override void PostGlobalGameStateCreateInit_2()
		{
			if (this.template == null)
			{
				Log.Error(this.templateName + " orgTemplate is not config. This may be due to mods. Removing.", Array.Empty<object>());
				if (this.assignedCouncilor != null)
				{
					this.ref_councilor.RemoveOrg(this);
				}
				else if (this.factionOrbit != null)
				{
					this.factionOrbit.RemoveOrgFromUnassignedPool(this);
					this.factionOrbit.availableOrgs.Remove(this);
				}
				this.killMe = true;
				return;
			}
			if (!string.IsNullOrEmpty(this.template.iconResource))
			{
				this.orgIconPath = this.template.iconResource;
			}
			if (this.takeoverDefense == 0f && this.template.takeoverDefense != 0f)
			{
				this.takeoverDefense = this.template.takeoverDefense;
			}
			this.InitRunTimeValues();
		}

		// Token: 0x0600355E RID: 13662 RVA: 0x00132060 File Offset: 0x00130260
		public override void PostCanvasManagerCreateInit_3()
		{
			if (this.homeRegion == null && (!this.gameStateSubjectCreated || this.template.orgType != OrgType.Faction))
			{
				this.SetHomeRegion(null);
			}
		}

		// Token: 0x0600355F RID: 13663 RVA: 0x00132090 File Offset: 0x00130290
		public override void PostVisualizerCreationInit_7()
		{
			if (this.assignedCouncilor != null && (!TIGameState.Valid(this.assignedCouncilor) || !this.assignedCouncilor.orgs.Contains(this)))
			{
				Log.Error(this.displayName + " was assigned to councilor ID " + this.assignedCouncilor.ID.ToString() + "but councilor was invalid or did not record this. This is an error.", Array.Empty<object>());
				this.UnassignCouncilor(this.assignedCouncilor);
			}
		}

		// Token: 0x06003560 RID: 13664 RVA: 0x0013210F File Offset: 0x0013030F
		public override void PostEverythingSaveRepair_8()
		{
			if (this.killMe)
			{
				GameStateManager.RemoveGameState<TIOrgState>(base.ID, false);
			}
		}

		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06003561 RID: 13665 RVA: 0x00132126 File Offset: 0x00130326
		public Sprite icon
		{
			get
			{
				if (this._icon == null)
				{
					this._icon = GameControl.assetLoader.LoadAsset<Sprite>(this.orgIconPath);
				}
				return this._icon;
			}
		}

		// Token: 0x06003562 RID: 13666 RVA: 0x00132154 File Offset: 0x00130354
		public int GetStatBonus(CouncilorAttribute stat)
		{
			switch (stat)
			{
			case CouncilorAttribute.Persuasion:
				return this.persuasion;
			case CouncilorAttribute.Investigation:
				return this.investigation;
			case CouncilorAttribute.Espionage:
				return this.espionage;
			case CouncilorAttribute.Command:
				return this.command;
			case CouncilorAttribute.Administration:
				return this.administration;
			case CouncilorAttribute.Science:
				return this.science;
			case CouncilorAttribute.Security:
				return this.security;
			default:
				return 0;
			}
		}

		// Token: 0x06003563 RID: 13667 RVA: 0x001321BC File Offset: 0x001303BC
		public TIResourcesCost GetPurchaseCost(TIFactionState faction)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			float num = this.costMoney;
			TIRegionState homeRegion = this.homeRegion;
			TIGameState tigameState;
			if (homeRegion == null)
			{
				tigameState = null;
			}
			else
			{
				TINationState nation = homeRegion.nation;
				tigameState = ((nation != null) ? nation.GetControlPointTypeOwner(ControlPointType.Corporations) : null);
			}
			if (tigameState == faction)
			{
				num *= TemplateManager.global.corporationsOrgMoneyDiscount;
			}
			tiresourcesCost.AddCost(FactionResource.Money, (float)((int)(num + TIEffectsState.SumEffectsModifiers(Context.OrgPurchaseCost, faction, num, FactionResource.Money.ToString()))), true);
			float num2 = this.costInfluence;
			TIRegionState homeRegion2 = this.homeRegion;
			TIGameState tigameState2;
			if (homeRegion2 == null)
			{
				tigameState2 = null;
			}
			else
			{
				TINationState nation2 = homeRegion2.nation;
				tigameState2 = ((nation2 != null) ? nation2.GetControlPointTypeOwner(ControlPointType.TradeUnions) : null);
			}
			if (tigameState2 == faction)
			{
				num2 *= TemplateManager.global.tradeUnionsOrgInfluenceDiscount;
			}
			tiresourcesCost.AddCost(FactionResource.Influence, (float)((int)(num2 + TIEffectsState.SumEffectsModifiers(Context.OrgPurchaseCost, faction, num2, FactionResource.Influence.ToString()))), true);
			tiresourcesCost.AddCost(FactionResource.Operations, (float)((int)(this.costOps + TIEffectsState.SumEffectsModifiers(Context.OrgPurchaseCost, faction, this.costOps, FactionResource.Operations.ToString()))), true);
			tiresourcesCost.AddCost(FactionResource.Boost, (float)((int)(this.costBoost + TIEffectsState.SumEffectsModifiers(Context.OrgPurchaseCost, faction, this.costBoost, FactionResource.Boost.ToString()))), true);
			return tiresourcesCost;
		}

		// Token: 0x06003564 RID: 13668 RVA: 0x001322EC File Offset: 0x001304EC
		public TIResourcesCost GetSalePrice(bool negative = false)
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			float num = this.costMoney * TemplateManager.global.sellOrgDiscount;
			if (negative)
			{
				num *= -1f;
			}
			tiresourcesCost.AddCost(FactionResource.Money, num, true);
			return tiresourcesCost;
		}

		// Token: 0x06003565 RID: 13669 RVA: 0x00132324 File Offset: 0x00130524
		public TIResourcesCost GetTransferCost()
		{
			TIResourcesCost tiresourcesCost = new TIResourcesCost();
			if (this.costMoney > 0f || this.costInfluence > 0f)
			{
				tiresourcesCost.AddCost(FactionResource.Money, this.costMoney * TemplateManager.global.transferOrgCostMultiplier, true);
				tiresourcesCost.AddCost(FactionResource.Influence, this.costInfluence * TemplateManager.global.transferOrgCostMultiplier, true);
			}
			else
			{
				tiresourcesCost.AddCost(FactionResource.Money, 1f, true);
			}
			return tiresourcesCost;
		}

		// Token: 0x06003566 RID: 13670 RVA: 0x00132393 File Offset: 0x00130593
		public TIResourcesCost GetPurchaseOrTransferCost(TIFactionState faction)
		{
			if (this.hasCouncilor || faction.unassignedOrgs.Contains(this))
			{
				return this.GetTransferCost();
			}
			return this.GetPurchaseCost(faction);
		}

		// Token: 0x06003567 RID: 13671 RVA: 0x001323B9 File Offset: 0x001305B9
		public void SetFactionOrbit(TIFactionState faction)
		{
			this.factionOrbit = faction;
		}

		// Token: 0x06003568 RID: 13672 RVA: 0x001323C2 File Offset: 0x001305C2
		public void ClearFactionOrbit()
		{
			this.factionOrbit = null;
		}

		// Token: 0x06003569 RID: 13673 RVA: 0x001323CB File Offset: 0x001305CB
		public void AssignCouncilor(TICouncilorState councilor)
		{
			this.assignedCouncilor = councilor;
			if (councilor != null && councilor.faction != null)
			{
				this.SetFactionOrbit(councilor.faction);
			}
		}

		// Token: 0x0600356A RID: 13674 RVA: 0x001323F7 File Offset: 0x001305F7
		public void UnassignCouncilor(TICouncilorState councilor)
		{
			if (this.assignedCouncilor == councilor)
			{
				this.assignedCouncilor = null;
			}
		}

		// Token: 0x17000885 RID: 2181
		// (get) Token: 0x0600356B RID: 13675 RVA: 0x0013240E File Offset: 0x0013060E
		public TINationState requiredNationInterest
		{
			get
			{
				if (!this.requiresNationInterest)
				{
					return null;
				}
				return this.homeRegion.nation;
			}
		}

		// Token: 0x0600356C RID: 13676 RVA: 0x00132425 File Offset: 0x00130625
		public float AvailabilityModifier(TIFactionState faction)
		{
			return (float)(this.affinities.Contains(faction.ideology.ideology) ? 2 : 1);
		}

		// Token: 0x0600356D RID: 13677 RVA: 0x00132444 File Offset: 0x00130644
		public bool AllowedOnFactionMarket(TIFactionState faction)
		{
			return this.template.allowedOnMarket && this.HasRequiredTech() && this.MeetsIdeologyRequirement(faction) && this.orgType != OrgType.Faction;
		}

		// Token: 0x0600356E RID: 13678 RVA: 0x00132472 File Offset: 0x00130672
		public bool HasRequiredTech()
		{
			return this.template.requiredTechTemplate == null || (this.template.requiredTechTemplate != null && TIGlobalResearchState.TechFinished(this.template.requiredTechTemplate));
		}

		// Token: 0x17000886 RID: 2182
		// (get) Token: 0x0600356F RID: 13679 RVA: 0x001324A2 File Offset: 0x001306A2
		public bool restrictiveOwnership
		{
			get
			{
				return this.requiresNationInterest || this.requiredOwnerTraits.Count > 0;
			}
		}

		// Token: 0x06003570 RID: 13680 RVA: 0x001324BC File Offset: 0x001306BC
		public bool IsEligibleForFaction(TIFactionState faction)
		{
			return (this.AllowedOnFactionMarket(faction) || (this.orgType == OrgType.Faction && this.affinities.Contains(faction.ideology.ideology)) || (!this.template.allowedOnMarket && this.factionOrbit == faction && this.HasRequiredTech() && this.MeetsIdeologyRequirement(faction))) && this.MeetsNationInterestRequirement(faction);
		}

		// Token: 0x06003571 RID: 13681 RVA: 0x00132528 File Offset: 0x00130728
		private bool MeetsIdeologyRequirement(TIFactionState faction)
		{
			return !this.restrictedIdeologies.Contains(faction.ideology.ideology);
		}

		// Token: 0x06003572 RID: 13682 RVA: 0x00132544 File Offset: 0x00130744
		private bool MeetsNationInterestRequirement(TIFactionState faction)
		{
			if (!this.requiresNationInterest)
			{
				return true;
			}
			IEnumerable<TINationState> enumerable = faction.controlPoints.Select<TIControlPoint, TINationState>((TIControlPoint x) => x.nation);
			List<TIRegionState> list;
			if (enumerable == null)
			{
				list = null;
			}
			else
			{
				list = enumerable.SelectMany<TINationState, TIRegionState>((TINationState y) => y.regions).ToList<TIRegionState>();
			}
			List<TIRegionState> list2 = list;
			if (faction.IsAlienProxy)
			{
				list2.AddRange(GameStateManager.AlienNation().regions);
			}
			if (list2.Contains(this.homeRegion))
			{
				return true;
			}
			return faction.councilors.Select<TICouncilorState, TINationState>((TICouncilorState x) => x.homeNation).SelectMany<TINationState, TIRegionState>((TINationState y) => y.regions).ToList<TIRegionState>()
				.Contains(this.homeRegion);
		}

		// Token: 0x06003573 RID: 13683 RVA: 0x0013263F File Offset: 0x0013083F
		private bool HasAllRequiredTraits(TICouncilorState councilor)
		{
			return !this.requiredOwnerTraits.Except<TITraitTemplate>(councilor.traits).Any<TITraitTemplate>();
		}

		// Token: 0x06003574 RID: 13684 RVA: 0x0013265A File Offset: 0x0013085A
		private bool HasNoProhibitedTraits(TICouncilorState councilor)
		{
			return !councilor.traits.Any<TITraitTemplate>((TITraitTemplate x) => this.prohibitedOwnerTraits.Contains(x));
		}

		// Token: 0x06003575 RID: 13685 RVA: 0x00132676 File Offset: 0x00130876
		public bool IsEligibleForCouncilor(TICouncilorState councilor)
		{
			return this.IsEligibleForFaction(councilor.faction) && this.HasNoProhibitedTraits(councilor) && (this.requiredOwnerTraits == null || this.HasAllRequiredTraits(councilor));
		}

		// Token: 0x06003576 RID: 13686 RVA: 0x001326A4 File Offset: 0x001308A4
		public string IneligibleReasonString(TICouncilorState councilor)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.IsEligibleForFaction(councilor.faction))
			{
				if (this.templateName == TIGlobalConfig.globalConfig.alienShockTroopOrgDataName)
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Orgs.Salamanders", new object[] { councilor.displayName }));
				}
				else if (!this.MeetsIdeologyRequirement(councilor.faction))
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Orgs.Ideology", new object[] { councilor.faction.displayNameCapitalized }));
				}
				else if (!this.template.allowedOnMarket || this.orgType == OrgType.Faction)
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Orgs.CantTransfer"));
				}
				else if (this.requiresNationInterest && this.homeRegion != null && !this.MeetsNationInterestRequirement(councilor.faction))
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Orgs.RequiresNation", new object[] { this.homeNation.displayNameWithArticle }));
				}
				else
				{
					stringBuilder.AppendLine().Append(Loc.T("UI.Orgs.IneligibleFaction", new object[] { councilor.faction.displayNameCapitalized }));
				}
			}
			if (!this.HasNoProhibitedTraits(councilor))
			{
				StringBuilder stringBuilder2 = stringBuilder.AppendLine();
				string text = "UI.Orgs.ProhibitedTraits";
				object[] array = new object[1];
				array[0] = TIUtilities.ConstructTextList(this.prohibitedOwnerTraits.ConvertAll<TIDataTemplate>((TITraitTemplate x) => x), false, false);
				stringBuilder2.Append(Loc.T(text, array));
			}
			if (this.requiredOwnerTraits != null && !this.HasAllRequiredTraits(councilor))
			{
				StringBuilder stringBuilder3 = stringBuilder.AppendLine();
				string text2 = "UI.Orgs.RequiresTraits";
				object[] array2 = new object[1];
				array2[0] = TIUtilities.ConstructTextList(this.requiredOwnerTraits.ConvertAll<TIDataTemplate>((TITraitTemplate x) => x), false, false);
				stringBuilder3.Append(Loc.T(text2, array2));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06003577 RID: 13687 RVA: 0x001328AE File Offset: 0x00130AAE
		public bool CouncilorCanAcquire(TICouncilorState councilor)
		{
			if (this.IsEligibleForCouncilor(councilor) && !councilor.detained)
			{
				TISpaceObjectState ref_spaceObject = councilor.ref_spaceObject;
				return ref_spaceObject != null && ref_spaceObject.inEarthSystem;
			}
			return false;
		}

		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x06003578 RID: 13688 RVA: 0x001328D4 File Offset: 0x00130AD4
		public TIFactionState miningFaction
		{
			get
			{
				if (!this.assignedCouncilor.isAlien)
				{
					return this.assignedCouncilor.faction;
				}
				return GameStateManager.AlienProxy();
			}
		}

		// Token: 0x06003579 RID: 13689 RVA: 0x001328F4 File Offset: 0x00130AF4
		public void SetOrgActivationStatus(bool activate)
		{
			if (this.applyingBonuses != activate && this.assignedCouncilor != null && this.assignedCouncilor.faction != null)
			{
				TIFactionState faction = this.assignedCouncilor.faction;
				this.applyingBonuses = activate;
				faction.SetResourceIncomeDataDirty();
				TIOrgTemplate template = this.template;
				if (template != null && template.projectsGranted > 0)
				{
					faction.CheckForOrgProjectStatusChange();
				}
				if (this.miningBonus > 0f)
				{
					this.miningFaction.habs.ForEach(delegate(TIHabState x)
					{
						x.UpdateCurrentAnnualNetResourceIncomes(false);
					});
				}
				this.assignedCouncilor.SetAttributesDirty();
				GameControl.eventManager.TriggerEvent(new CouncilorValuesChanged(this.assignedCouncilor), null, new object[] { this.assignedCouncilor, faction });
			}
		}

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x0600357A RID: 13690 RVA: 0x001329DA File Offset: 0x00130BDA
		public string tierStars
		{
			get
			{
				return Loc.T(new StringBuilder("UI.Councilor.Orgs.Tier").Append(this.tier.ToString()).ToString());
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x0600357B RID: 13691 RVA: 0x00132A00 File Offset: 0x00130C00
		public string tierStarsInline
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.tier; i++)
				{
					stringBuilder.Append(TemplateManager.global.starInlineSpritePath);
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x1700088A RID: 2186
		// (get) Token: 0x0600357C RID: 13692 RVA: 0x00132A3C File Offset: 0x00130C3C
		public string smallTierStarsInline
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < this.tier; i++)
				{
					stringBuilder.Append(TemplateManager.global.starInlineSpritePath_sizeOverride60);
				}
				return stringBuilder.ToString();
			}
		}

		// Token: 0x0600357D RID: 13693 RVA: 0x00132A78 File Offset: 0x00130C78
		public string QuickDescription(bool insertSpaces = false)
		{
			TIOrgState.<>c__DisplayClass179_0 CS$<>8__locals1;
			CS$<>8__locals1.insertSpaces = insertSpaces;
			CS$<>8__locals1.sb = new StringBuilder();
			CS$<>8__locals1.iconsAdded = 0;
			if (this.adjustedIncomeMoney_month > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.moneyInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.adjustedIncomeInfluence_month > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.influenceInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.adjustedIncomeOps_month > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.opsInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.adjustedIncomeBoost_month > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.boostInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.incomeMissionControl > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.missionControlInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.adjustedIncomeResearch_month > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.researchInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.projectCapacityGranted > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.projectsInlineSpritePath, ref CS$<>8__locals1);
			}
			if (this.persuasion > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Persuasion), ref CS$<>8__locals1);
			}
			if (this.investigation > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Investigation), ref CS$<>8__locals1);
			}
			if (this.espionage > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Espionage), ref CS$<>8__locals1);
			}
			if (this.command > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Command), ref CS$<>8__locals1);
			}
			if (this.administration > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Administration), ref CS$<>8__locals1);
			}
			if (this.science > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Science), ref CS$<>8__locals1);
			}
			if (this.security > 0)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TIUtilities.InlineAttributeStr(CouncilorAttribute.Security), ref CS$<>8__locals1);
			}
			for (int i = 0; i < this.techBonuses.Length; i++)
			{
				if (this.techBonuses[i].bonus > 0f)
				{
					TIOrgState.<QuickDescription>g__AddIcon|179_0(TIGenericTechTemplate.categoryInlineSprite(this.techBonuses[i].category), ref CS$<>8__locals1);
				}
			}
			if (this.economyBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Economy), ref CS$<>8__locals1);
			}
			if (this.welfareBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Welfare), ref CS$<>8__locals1);
			}
			if (this.environmentBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Environment), ref CS$<>8__locals1);
			}
			if (this.knowledgeBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Knowledge), ref CS$<>8__locals1);
			}
			if (this.governmentBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Government), ref CS$<>8__locals1);
			}
			if (this.unityBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Unity), ref CS$<>8__locals1);
			}
			if (this.oppressionBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Oppression), ref CS$<>8__locals1);
			}
			if (this.spoilsBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Spoils), ref CS$<>8__locals1);
			}
			if (this.militaryBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Military), ref CS$<>8__locals1);
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Military_FoundMilitary), ref CS$<>8__locals1);
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Military_BuildArmy), ref CS$<>8__locals1);
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Military_BuildNavy), ref CS$<>8__locals1);
				if (TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSpaceDefenses))
				{
					TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Military_BuildSpaceDefenses), ref CS$<>8__locals1);
				}
			}
			if (this.spaceDevBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Funding), ref CS$<>8__locals1);
			}
			if (this.spaceflightBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Civilian_InitiateSpaceflightProgram), ref CS$<>8__locals1);
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.LaunchFacilities), ref CS$<>8__locals1);
				if (TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron))
				{
					TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.Military_BuildSTOSquadron), ref CS$<>8__locals1);
				}
			}
			if (this.MCBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TINationState.GetInlinePriorityIcon(PriorityType.MissionControl), ref CS$<>8__locals1);
			}
			if (this.miningBonus > 0f)
			{
				TIOrgState.<QuickDescription>g__AddIcon|179_0(TemplateManager.global.pathInlineSpaceMiningIcon, ref CS$<>8__locals1);
			}
			return CS$<>8__locals1.sb.ToString();
		}

		// Token: 0x0600357E RID: 13694 RVA: 0x00132E1C File Offset: 0x0013101C
		public string description(bool includeDisplayName, TIFactionState viewingFaction, bool includeOwnership = false, bool includeCost = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (includeDisplayName)
			{
				stringBuilder = stringBuilder.Append(this.displayName).AppendLine();
				stringBuilder.AppendLine(this.tierStarsInline);
			}
			if (includeOwnership)
			{
				if (viewingFaction.GetViewofCouncilor(this.assignedCouncilor).orgs.Contains(this))
				{
					stringBuilder.AppendLine(Loc.T("UI.Orgs.Ownership", new object[]
					{
						this.assignedCouncilor.displayName,
						this.assignedCouncilor.faction.displayNameWithColor
					}));
				}
				else if (viewingFaction.GetViewofFaction(this.factionOrbit).knownUnassignedOrgsPool.Contains(this))
				{
					stringBuilder.AppendLine(Loc.T("UI.Orgs.PoolOrg", new object[] { this.factionOrbit.adjectiveWithColor }));
				}
			}
			if (this.assignedCouncilor != null && this.assignedCouncilor.faction != null && !this.applyingBonuses)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.NotApplyingBonuses"));
			}
			if (this.homeRegion != null)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.HomeRegion", new object[] { this.homeRegion.displayName })).AppendLine();
			}
			if (this.requiredOwnerTraits.Count > 0)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				string text = "UI.Orgs.RequiresTraits";
				object[] array = new object[1];
				array[0] = TIUtilities.ConstructTextList(this.requiredOwnerTraits.ConvertAll<TIDataTemplate>((TITraitTemplate x) => x), false, false);
				stringBuilder2.AppendLine(Loc.T(text, array));
			}
			if (this.prohibitedOwnerTraits.Count > 0)
			{
				StringBuilder stringBuilder3 = stringBuilder;
				string text2 = "UI.Orgs.ProhibitedTraits";
				object[] array2 = new object[1];
				array2[0] = TIUtilities.ConstructTextList(this.prohibitedOwnerTraits.ConvertAll<TIDataTemplate>((TITraitTemplate x) => x), false, false);
				stringBuilder3.AppendLine(Loc.T(text2, array2));
			}
			if (this.requiresNationInterest && this.homeRegion != null)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.RequiresNation", new object[] { this.homeNation.displayNameWithArticle }));
			}
			bool flag = false;
			TICouncilorState assignedCouncilor = this.assignedCouncilor;
			if (((assignedCouncilor != null) ? assignedCouncilor.faction : null) != viewingFaction && !this.IsEligibleForFaction(viewingFaction))
			{
				if (this.templateName == TIGlobalConfig.globalConfig.alienShockTroopOrgDataName)
				{
					stringBuilder.AppendLine().AppendLine(TIUtilities.RedLine(Loc.T("UI.Orgs.Salamanders", new object[] { viewingFaction.displayNameCapitalized })));
				}
				else if (!this.MeetsIdeologyRequirement(viewingFaction))
				{
					stringBuilder.AppendLine().AppendLine(TIUtilities.RedLine(Loc.T("UI.Orgs.Ideology", new object[] { viewingFaction.displayNameCapitalized })));
				}
				else if (!this.template.allowedOnMarket || this.orgType == OrgType.Faction)
				{
					stringBuilder.AppendLine().AppendLine(TIUtilities.RedLine(Loc.T("UI.Orgs.CantTransfer")));
					flag = true;
				}
				else
				{
					stringBuilder.AppendLine().AppendLine(TIUtilities.RedLine(Loc.T("UI.Orgs.IneligibleFaction", new object[] { viewingFaction.displayNameCapitalized })));
				}
			}
			StringBuilder stringBuilder4 = new StringBuilder(256);
			if (this.missionsGranted.Count > 0)
			{
				stringBuilder4.AppendLine(Loc.T("UI.Orgs.MissionsGranted"));
				foreach (TIMissionTemplate timissionTemplate in this.missionsGranted)
				{
					stringBuilder4.Append("  ").AppendLine(timissionTemplate.displayName);
				}
				stringBuilder.Append(stringBuilder4.ToString());
			}
			if (this.projectGranted != null)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.ProjectGranted", new object[] { this.projectGranted.displayName }));
			}
			StringBuilder stringBuilder5 = new StringBuilder(256);
			bool flag2 = false;
			if (this.adjustedIncomeMoney_month != 0f)
			{
				stringBuilder5.Append((this.adjustedIncomeMoney_month > 0f) ? "+" : "-").Append(TemplateManager.global.moneyInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeMoney_month).ToString())
					.Append("  ");
				flag2 = true;
			}
			if (this.adjustedIncomeInfluence_month != 0f)
			{
				stringBuilder5.Append((this.adjustedIncomeInfluence_month > 0f) ? "+" : "-").Append(TemplateManager.global.influenceInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeInfluence_month).ToString())
					.Append("  ");
				flag2 = true;
			}
			if (this.adjustedIncomeOps_month != 0f)
			{
				stringBuilder5.Append((this.adjustedIncomeOps_month > 0f) ? "+" : "-").Append(TemplateManager.global.opsInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeOps_month))
					.Append("  ");
				flag2 = true;
			}
			if (this.adjustedIncomeBoost_month != 0f)
			{
				stringBuilder5.Append((this.adjustedIncomeBoost_month > 0f) ? "+" : "-").Append(TemplateManager.global.boostInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeBoost_month))
					.Append("  ");
				flag2 = true;
			}
			if (this.incomeMissionControl != 0f)
			{
				stringBuilder5.Append((this.incomeMissionControl > 0f) ? "+" : "-").Append(TemplateManager.global.missionControlInlineSpritePath).Append(Mathf.Abs(this.incomeMissionControl))
					.Append("  ");
				flag2 = true;
			}
			if (this.adjustedIncomeResearch_month != 0f)
			{
				stringBuilder5.Append((this.adjustedIncomeResearch_month > 0f) ? "+" : "-").Append(TemplateManager.global.researchInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeResearch_month))
					.Append("  ");
				flag2 = true;
			}
			if (this.projectCapacityGranted > 0)
			{
				stringBuilder5.Append("+").Append(TemplateManager.global.projectsInlineSpritePath + this.projectCapacityGranted.ToString());
				flag2 = true;
			}
			if (flag2)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.Income", new object[] { stringBuilder5.ToString() }));
			}
			StringBuilder stringBuilder6 = new StringBuilder();
			bool flag3 = false;
			if (this.persuasion != 0)
			{
				stringBuilder6.Append(this.persuasion.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Persuasion)).Append("  ");
				flag3 = true;
			}
			if (this.investigation != 0)
			{
				stringBuilder6.Append(this.investigation.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Investigation)).Append("  ");
				flag3 = true;
			}
			if (this.espionage != 0)
			{
				stringBuilder6.Append(this.espionage.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Espionage)).Append("  ");
				flag3 = true;
			}
			if (this.command != 0)
			{
				stringBuilder6.Append(this.command.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Command)).Append("  ");
				flag3 = true;
			}
			if (this.administration != 0)
			{
				stringBuilder6.Append(this.administration.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Administration)).Append("  ");
				flag3 = true;
			}
			if (this.science != 0)
			{
				stringBuilder6.Append(this.science.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Science)).Append("  ");
				flag3 = true;
			}
			if (this.security != 0)
			{
				stringBuilder6.Append(this.security.ToString("+0;-#") + " " + TIUtilities.InlineAttributeStr(CouncilorAttribute.Security)).Append("  ");
				flag3 = true;
			}
			if (flag3)
			{
				stringBuilder.Append(stringBuilder6.ToString()).AppendLine();
			}
			StringBuilder stringBuilder7 = new StringBuilder();
			bool flag4 = false;
			if (this.economyBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.economyBonus > 0f) ? "+" : "-").Append(this.economyBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Economy, true));
				flag4 = true;
			}
			if (this.welfareBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.welfareBonus > 0f) ? "+" : "-").Append(this.welfareBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Welfare, true));
				flag4 = true;
			}
			if (this.environmentBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.environmentBonus > 0f) ? "+" : "-").Append(this.environmentBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Environment, true));
				flag4 = true;
			}
			if (this.knowledgeBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.knowledgeBonus > 0f) ? "+" : "-").Append(this.knowledgeBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Knowledge, true));
				flag4 = true;
			}
			if (this.governmentBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.governmentBonus > 0f) ? "+" : "-").Append(this.governmentBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Government, true));
				flag4 = true;
			}
			if (this.oppressionBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.oppressionBonus > 0f) ? "+" : "-").Append(this.oppressionBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Oppression, true));
				flag4 = true;
			}
			if (this.unityBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.unityBonus > 0f) ? "+" : "-").Append(this.unityBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Unity, true));
				flag4 = true;
			}
			if (this.militaryBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.militaryBonus > 0f) ? "+" : "-").Append(this.militaryBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Military, true))
					.AppendLine()
					.Append((this.militaryBonus > 0f) ? "+" : "-")
					.Append(this.militaryBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Military_FoundMilitary, true))
					.AppendLine()
					.Append((this.militaryBonus > 0f) ? "+" : "-")
					.Append(this.militaryBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Military_BuildArmy, true))
					.AppendLine()
					.Append((this.militaryBonus > 0f) ? "+" : "-")
					.Append(this.militaryBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Military_BuildNavy, true));
				if (TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSpaceDefenses))
				{
					stringBuilder7.AppendLine().Append((this.militaryBonus > 0f) ? "+" : "-").Append(this.militaryBonus.ToPercent("P0"))
						.Append(" ")
						.Append(TIUtilities.GetPriorityString(PriorityType.Military_BuildSpaceDefenses, true));
				}
				flag4 = true;
			}
			if (this.spoilsBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.spoilsBonus > 0f) ? "+" : "-").Append(this.spoilsBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Spoils, true));
				flag4 = true;
			}
			if (this.spaceDevBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.spaceDevBonus > 0f) ? "+" : "-").Append(this.spaceDevBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Funding, true));
				flag4 = true;
			}
			if (this.spaceflightBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.spaceflightBonus > 0f) ? "+" : "-").Append(this.spaceflightBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.Civilian_InitiateSpaceflightProgram, true))
					.AppendLine()
					.Append((this.spaceflightBonus > 0f) ? "+" : "-")
					.Append(this.spaceflightBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.LaunchFacilities, true));
				if (TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron))
				{
					stringBuilder7.AppendLine().Append((this.spaceflightBonus > 0f) ? "+" : "-").Append(this.spaceflightBonus.ToPercent("P0"))
						.Append(" ")
						.Append(TIUtilities.GetPriorityString(PriorityType.Military_BuildSTOSquadron, true));
				}
				flag4 = true;
			}
			if (this.MCBonus != 0f)
			{
				stringBuilder7.AppendLine().Append((this.MCBonus > 0f) ? "+" : "-").Append(this.MCBonus.ToPercent("P0"))
					.Append(" ")
					.Append(TIUtilities.GetPriorityString(PriorityType.MissionControl, true));
				flag4 = true;
			}
			if (flag4)
			{
				stringBuilder.AppendLine(stringBuilder7.ToString());
			}
			for (int i = 0; i < this.techBonuses.Length; i++)
			{
				if (this.techBonuses[i].bonus > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Orgs.TechBonus", new object[]
					{
						this.techBonuses[i].bonus.ToPercent("P0"),
						TIGenericTechTemplate.GetTechCategoryString(this.techBonuses[i].category)
					}));
				}
			}
			if (this.miningBonus != 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.SpaceMiningBonus", new object[]
				{
					this.miningBonus.ToPercent("P0"),
					TemplateManager.global.pathInlineSpaceMiningIcon
				}));
			}
			if (this.grantsMarked)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.GrantsMarked", new object[] { TemplateManager.Find<TITraitTemplate>("Marked", false).displayName }));
			}
			if (this.template.allowedOnMarket)
			{
				if (this.innateDefenses != 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Councilor.Orgs.InnateDefenses", new object[] { this.innateDefenses.ToString() }));
				}
			}
			else if (!flag)
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.CantTransfer"));
			}
			if (this.hasCouncilor && this.AllowedOnFactionMarket(this.factionOrbit))
			{
				stringBuilder.AppendLine(Loc.T("UI.Orgs.SalePrice", new object[] { this.GetSalePrice(false).ToString("N0", false, false, null, false, FactionResource.Money) }));
			}
			else if (includeCost && viewingFaction != null)
			{
				if (this.factionOrbit != null && this.factionOrbit.unassignedOrgs.Contains(this))
				{
					stringBuilder.AppendLine(Loc.T("UI.Orgs.EquipCost", new object[] { this.GetTransferCost().ToString("N0", false, false, null, false, FactionResource.Money) }));
					stringBuilder.AppendLine(Loc.T("UI.Orgs.SalePrice", new object[] { this.GetSalePrice(false).ToString("N0", false, false, null, false, FactionResource.Money) }));
				}
				else
				{
					stringBuilder.AppendLine(this.GetPurchaseCost(viewingFaction).GetString("Relevant", true, false, false, 2, false, false, null, false, FactionResource.None));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600357F RID: 13695 RVA: 0x00133F6C File Offset: 0x0013216C
		public string descriptionTruncated()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.adjustedIncomeMoney_month != 0f)
			{
				stringBuilder.Append((this.adjustedIncomeMoney_month > 0f) ? "+" : "-").Append(TemplateManager.global.moneyInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeMoney_month).ToString())
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.adjustedIncomeInfluence_month != 0f)
			{
				stringBuilder.Append((this.adjustedIncomeInfluence_month > 0f) ? "+" : "-").Append(TemplateManager.global.influenceInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeInfluence_month).ToString())
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.adjustedIncomeOps_month != 0f)
			{
				stringBuilder.Append((this.adjustedIncomeOps_month > 0f) ? "+" : "-").Append(TemplateManager.global.opsInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeOps_month).ToString())
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.adjustedIncomeBoost_month != 0f)
			{
				stringBuilder.Append((this.adjustedIncomeBoost_month > 0f) ? "+" : "-").Append(TemplateManager.global.boostInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeBoost_month).ToString())
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.incomeMissionControl != 0f)
			{
				stringBuilder.Append((this.incomeMissionControl > 0f) ? "+" : "-").Append(TemplateManager.global.missionControlInlineSpritePath).Append(Mathf.Abs(this.incomeMissionControl).ToString())
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.adjustedIncomeResearch_month > 0f)
			{
				stringBuilder.Append((this.adjustedIncomeResearch_month > 0f) ? "+" : "-").Append(TemplateManager.global.researchInlineSpritePath).Append(Mathf.Abs(this.adjustedIncomeResearch_month).ToString())
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.projectCapacityGranted > 0)
			{
				stringBuilder.Append("+").Append(TemplateManager.global.projectsInlineSpritePath + this.projectCapacityGranted.ToString()).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.persuasion > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.persuasion.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Persuasion))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.investigation > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.investigation.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Investigation))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.espionage > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.espionage.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Espionage))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.command > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.command.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Command))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.administration > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.administration.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Administration))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.science > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.science.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Science))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.security > 0)
			{
				stringBuilder.Append("<nobr>").Append(this.security.ToString("+0;-#")).Append(" ")
					.Append(TIUtilities.InlineAttributeStr(CouncilorAttribute.Security))
					.Append("</nobr>")
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			for (int i = 0; i < this.techBonuses.Length; i++)
			{
				if (this.techBonuses[i].bonus != 0f)
				{
					stringBuilder.Append((this.techBonuses[i].bonus > 0f) ? "+" : "-").Append(this.techBonuses[i].bonus.ToPercent("P0") + TIGenericTechTemplate.categoryInlineSprite(this.techBonuses[i].category)).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
			}
			if (this.economyBonus != 0f)
			{
				stringBuilder.Append((this.economyBonus > 0f) ? "+" : "-").Append(this.economyBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Economy))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.welfareBonus != 0f)
			{
				stringBuilder.Append((this.welfareBonus > 0f) ? "+" : "-").Append(this.welfareBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Welfare))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.environmentBonus != 0f)
			{
				stringBuilder.Append((this.environmentBonus > 0f) ? "+" : "-").Append(this.environmentBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Environment))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.knowledgeBonus != 0f)
			{
				stringBuilder.Append((this.knowledgeBonus > 0f) ? "+" : "-").Append(this.knowledgeBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Knowledge))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.governmentBonus != 0f)
			{
				stringBuilder.Append((this.governmentBonus > 0f) ? "+" : "-").Append(this.governmentBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Government))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.unityBonus != 0f)
			{
				stringBuilder.Append((this.unityBonus > 0f) ? "+" : "-").Append(this.unityBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Unity))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.oppressionBonus != 0f)
			{
				stringBuilder.Append((this.oppressionBonus > 0f) ? "+" : "-").Append(this.oppressionBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Oppression))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.spoilsBonus != 0f)
			{
				stringBuilder.Append((this.spoilsBonus > 0f) ? "+" : "-").Append(this.spoilsBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Spoils))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.militaryBonus > 0f)
			{
				stringBuilder.Append((this.militaryBonus > 0f) ? "+" : "-").Append(this.militaryBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Military));
			}
			if (this.spaceDevBonus > 0f)
			{
				stringBuilder.Append((this.spaceDevBonus > 0f) ? "+" : "-").Append(this.spaceDevBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.Funding));
			}
			if (this.spaceflightBonus > 0f)
			{
				stringBuilder.Append((this.spaceflightBonus > 0f) ? "+" : "-").Append(this.spaceflightBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.LaunchFacilities));
			}
			if (this.MCBonus > 0f)
			{
				stringBuilder.Append((this.MCBonus > 0f) ? "+" : "-").Append(this.MCBonus.ToPercent("P0")).Append(TINationState.GetInlinePriorityIcon(PriorityType.MissionControl))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.miningBonus > 0f)
			{
				stringBuilder.Append((this.miningBonus > 0f) ? "+" : "-").Append(this.miningBonus.ToPercent("P0")).Append(TemplateManager.global.pathInlineSpaceMiningIcon)
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			if (this.grantsMarked)
			{
				stringBuilder.Append(TIGlobalConfig.globalConfig.warningInlineSpritePath);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x1700088B RID: 2187
		// (get) Token: 0x06003580 RID: 13696 RVA: 0x001349C7 File Offset: 0x00132BC7
		private float innateDefenses
		{
			get
			{
				return (float)this.tier * TemplateManager.global.TIMissionModifier_OrgDefenses + (float)this.administration + this.takeoverDefense;
			}
		}

		// Token: 0x06003581 RID: 13697 RVA: 0x001349EC File Offset: 0x00132BEC
		public float GetMonthlyIncome(FactionResource resource)
		{
			switch (resource)
			{
			case FactionResource.Money:
				return this.adjustedIncomeMoney_month;
			case FactionResource.Influence:
				return this.adjustedIncomeInfluence_month;
			case FactionResource.Operations:
				return this.adjustedIncomeOps_month;
			case FactionResource.Research:
				return this.adjustedIncomeResearch_month;
			case FactionResource.Projects:
				return (float)this.projectCapacityGranted;
			case FactionResource.Boost:
				return this.adjustedIncomeBoost_month;
			case FactionResource.MissionControl:
				return this.incomeMissionControl;
			default:
				return 0f;
			}
		}

		// Token: 0x06003582 RID: 13698 RVA: 0x00134A56 File Offset: 0x00132C56
		public float GetDailyIncome(FactionResource resource)
		{
			return this.GetMonthlyIncome(resource) / 30.436874f;
		}

		// Token: 0x06003587 RID: 13703 RVA: 0x00134A9C File Offset: 0x00132C9C
		[CompilerGenerated]
		internal static void <QuickDescription>g__AddIcon|179_0(string path, ref TIOrgState.<>c__DisplayClass179_0 A_1)
		{
			A_1.sb.Append(path);
			A_1.iconsAdded++;
			if (A_1.insertSpaces && A_1.iconsAdded % 7 == 0)
			{
				A_1.sb.Append(" ");
			}
		}

		// Token: 0x040023DC RID: 9180
		public const int minTier = 1;

		// Token: 0x040023DD RID: 9181
		public const int maxTier = 3;

		// Token: 0x040023DE RID: 9182
		public int tier;

		// Token: 0x040023DF RID: 9183
		public float takeoverDefense;

		// Token: 0x040023E3 RID: 9187
		public float costMoney;

		// Token: 0x040023E4 RID: 9188
		public float costInfluence;

		// Token: 0x040023E5 RID: 9189
		public float costOps;

		// Token: 0x040023E6 RID: 9190
		public float costBoost;

		// Token: 0x040023E7 RID: 9191
		[SerializeField]
		private float incomeMoney_month;

		// Token: 0x040023E8 RID: 9192
		[SerializeField]
		private float incomeInfluence_month;

		// Token: 0x040023E9 RID: 9193
		[SerializeField]
		private float incomeOps_month;

		// Token: 0x040023EA RID: 9194
		[SerializeField]
		private float incomeBoost_month;

		// Token: 0x040023EB RID: 9195
		[SerializeField]
		private float incomeResearch_month;

		// Token: 0x040023EC RID: 9196
		public float incomeMissionControl;

		// Token: 0x040023ED RID: 9197
		public int projectCapacityGranted;

		// Token: 0x040023EE RID: 9198
		public int persuasion;

		// Token: 0x040023EF RID: 9199
		public int command;

		// Token: 0x040023F0 RID: 9200
		public int investigation;

		// Token: 0x040023F1 RID: 9201
		public int espionage;

		// Token: 0x040023F2 RID: 9202
		public int administration;

		// Token: 0x040023F3 RID: 9203
		public int science;

		// Token: 0x040023F4 RID: 9204
		public int security;

		// Token: 0x040023F5 RID: 9205
		public float economyBonus;

		// Token: 0x040023F6 RID: 9206
		public float welfareBonus;

		// Token: 0x040023F7 RID: 9207
		public float environmentBonus;

		// Token: 0x040023F8 RID: 9208
		public float knowledgeBonus;

		// Token: 0x040023F9 RID: 9209
		public float governmentBonus;

		// Token: 0x040023FA RID: 9210
		public float unityBonus;

		// Token: 0x040023FB RID: 9211
		public float militaryBonus;

		// Token: 0x040023FC RID: 9212
		public float oppressionBonus;

		// Token: 0x040023FD RID: 9213
		public float spoilsBonus;

		// Token: 0x040023FE RID: 9214
		public float spaceDevBonus;

		// Token: 0x040023FF RID: 9215
		public float spaceflightBonus;

		// Token: 0x04002400 RID: 9216
		public float MCBonus;

		// Token: 0x04002401 RID: 9217
		public float miningBonus;

		// Token: 0x04002402 RID: 9218
		public float XPModifier;

		// Token: 0x04002406 RID: 9222
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x04002407 RID: 9223
		[fsIgnore]
		public List<TIMissionTemplate> missionsGranted;

		// Token: 0x04002409 RID: 9225
		private List<FactionIdeology> affinities;

		// Token: 0x0400240A RID: 9226
		private List<FactionIdeology> restrictedIdeologies;

		// Token: 0x0400240E RID: 9230
		private Sprite _icon;

		// Token: 0x0400240F RID: 9231
		public static readonly FactionResource[] orgNegativeResources = new FactionResource[]
		{
			FactionResource.Money,
			FactionResource.Influence,
			FactionResource.Operations,
			FactionResource.Boost,
			FactionResource.MissionControl
		};

		// Token: 0x04002410 RID: 9232
		private bool killMe;
	}
}
