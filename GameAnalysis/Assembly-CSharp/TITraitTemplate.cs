using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000297 RID: 663
public class TITraitTemplate : TIDataTemplate
{
	// Token: 0x0600091C RID: 2332 RVA: 0x0002AF24 File Offset: 0x00029124
	public bool CouncilorCanAddByAugment(TICouncilorState councilor)
	{
		return (this.XPCost > 0 || this.moneyCost > 0 || this.influenceCost > 0 || this.opsCost > 0 || this.boostCost > 0 || this.requiresProject) && !councilor.traits.Contains(this);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0002AF77 File Offset: 0x00029177
	public bool CouncilorCanRemoveByAugment(TICouncilorState councilor)
	{
		return (this.XPCost < 0 || this.moneyCost < 0 || this.influenceCost < 0 || this.opsCost < 0 || this.boostCost < 0) && councilor.traits.Contains(this);
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x0600091E RID: 2334 RVA: 0x0002AFB4 File Offset: 0x000291B4
	public bool requiresProject
	{
		get
		{
			return !string.IsNullOrEmpty(this.projectDataName);
		}
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x0600091F RID: 2335 RVA: 0x0002AFC4 File Offset: 0x000291C4
	public TITraitTemplate requiredTraitForUpgrade
	{
		get
		{
			return TemplateManager.Find<TITraitTemplate>(this.upgradesFrom, true);
		}
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000920 RID: 2336 RVA: 0x0002AFD2 File Offset: 0x000291D2
	public string description
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.dataName).ToString());
		}
	}

	// Token: 0x17000123 RID: 291
	// (get) Token: 0x06000921 RID: 2337 RVA: 0x0002B003 File Offset: 0x00029203
	public bool isGovernmentTrait
	{
		get
		{
			return this.specialTraitRule == SpecialTraitRule.Government;
		}
	}

	// Token: 0x17000124 RID: 292
	// (get) Token: 0x06000922 RID: 2338 RVA: 0x0002B00E File Offset: 0x0002920E
	public bool isCriminalTrait
	{
		get
		{
			return this.specialTraitRule == SpecialTraitRule.Criminal;
		}
	}

	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000923 RID: 2339 RVA: 0x0002B01C File Offset: 0x0002921C
	public bool incomeTrait
	{
		get
		{
			if (this.incomeMoney == 0f && this.incomeInfluence == 0f && this.incomeOps == 0f && this.incomeBoost == 0f && this.incomeResearch == 0f && this.incomeProjects == 0)
			{
				return this.techBonuses.Any<TechBonus>((TechBonus x) => x.bonus != 0f);
			}
			return true;
		}
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x0002B0A0 File Offset: 0x000292A0
	public bool CouncilorCanHave(TICouncilorState councilor, TIFactionState forFaction, bool grantedByEffect = false)
	{
		if (councilor.traits.Contains(this))
		{
			return false;
		}
		if (grantedByEffect && this.alwaysGrantFromEffect)
		{
			return true;
		}
		if (councilor.GetIndividualTraitChance(this, forFaction) > 0f)
		{
			int? num = this.grouping;
			int num2 = 0;
			return ((num.GetValueOrDefault() == num2) & (num != null)) || councilor.traits.None<TITraitTemplate>(delegate(TITraitTemplate x)
			{
				int? num3 = x.grouping;
				int? num4 = this.grouping;
				return (num3.GetValueOrDefault() == num4.GetValueOrDefault()) & (num3 != null == (num4 != null));
			});
		}
		return false;
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000925 RID: 2341 RVA: 0x0002B114 File Offset: 0x00029314
	public List<TIMissionTemplate> RestrictedMissions
	{
		get
		{
			if (this._restrictedMissions == null)
			{
				this._restrictedMissions = new List<TIMissionTemplate>();
				foreach (string text in this.restrictedMissionNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TIMissionTemplate timissionTemplate = TemplateManager.Find<TIMissionTemplate>(text, false);
						if (timissionTemplate != null)
						{
							this._restrictedMissions.Add(timissionTemplate);
						}
						else
						{
							Log.Error("Bad mission name in restrictedMissionNames in " + base.dataName, Array.Empty<object>());
						}
					}
				}
			}
			return this._restrictedMissions;
		}
	}

	// Token: 0x17000127 RID: 295
	// (get) Token: 0x06000926 RID: 2342 RVA: 0x0002B1B4 File Offset: 0x000293B4
	public List<TIMissionTemplate> MissionsGranted
	{
		get
		{
			if (this._missionsGranted == null)
			{
				this._missionsGranted = new List<TIMissionTemplate>();
				foreach (string text in this.missionsGrantedNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TIMissionTemplate timissionTemplate = TemplateManager.Find<TIMissionTemplate>(text, false);
						if (timissionTemplate != null)
						{
							this._missionsGranted.Add(timissionTemplate);
						}
						else
						{
							Log.Error("Bad mission name in missionsGrantedNames in " + base.dataName, Array.Empty<object>());
						}
					}
				}
			}
			return this._missionsGranted;
		}
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x0002B254 File Offset: 0x00029454
	public bool IsMatchingProject(TIProjectTemplate project)
	{
		return this.requiresProject && project.referenceName == this.projectDataName;
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x0002B274 File Offset: 0x00029474
	public bool RerollTrait(TICouncilorState councilor, TIFactionState forFaction)
	{
		if (!string.IsNullOrEmpty(this.rerollTrait))
		{
			TITraitTemplate traitToReroll = TemplateManager.Find<TITraitTemplate>(this.rerollTrait, false);
			if (traitToReroll != null)
			{
				if (councilor.traits.Contains(traitToReroll) && this.rerollTraitBonus < 0f)
				{
					float num = councilor.GetIndividualTraitChance(traitToReroll, forFaction) + this.rerollTraitBonus;
					if (TIUtilities.RandomFloatValue() * 100f > num)
					{
						councilor.RemoveTrait(traitToReroll);
						return true;
					}
				}
				else if (!councilor.traits.Contains(traitToReroll) && this.rerollTraitBonus >= 0f)
				{
					int? num2 = traitToReroll.grouping;
					int num3 = 0;
					if (((num2.GetValueOrDefault() == num3) & (num2 != null)) || councilor.traits.None<TITraitTemplate>(delegate(TITraitTemplate x)
					{
						int? num5 = x.grouping;
						int? num6 = traitToReroll.grouping;
						return (num5.GetValueOrDefault() == num6.GetValueOrDefault()) & (num5 != null == (num6 != null));
					}))
					{
						float num4 = councilor.GetIndividualTraitChance(traitToReroll, forFaction) + this.rerollTraitBonus;
						if (TIUtilities.RandomFloatValue() * 100f <= num4)
						{
							councilor.AddTrait(traitToReroll, false);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x0002B39C File Offset: 0x0002959C
	public string GetPerValueString(StatModVariable statModVariable)
	{
		switch (statModVariable)
		{
		case StatModVariable.MyFactionAtrocities:
			return Loc.T("UI.Traits.MyFactionAtrocities");
		case StatModVariable.Loyalty:
			return TIUtilities.GetAttributeString(CouncilorAttribute.Loyalty);
		case StatModVariable.MyFactionCouncilors:
			return Loc.T("UI.Traits.MyFactionCouncilors");
		case StatModVariable.MyFactionTurnedCouncilors:
			return Loc.T("UI.Traits.MyFactionTurnedCouncilors");
		case StatModVariable.MyFactionArmies:
			return Loc.T("UI.Traits.MyFactionArmies");
		case StatModVariable.MyFactionArmiesLost:
			return Loc.T("UI.Traits.MyFactionArmiesLost");
		case StatModVariable.MyFactionAliensRemoved:
			return Loc.T("UI.Traits.MyFactionAliensRemoved");
		case StatModVariable.MyFactionMaxProjects:
			return Loc.T("UI.Traits.MyFactionMaxProjects");
		case StatModVariable.HomeNationWars:
			return Loc.T("UI.Traits.HomeNationWars");
		case StatModVariable.HomeNationFederationMembers:
			return Loc.T("UI.Traits.HomeNationFederationMembers");
		case StatModVariable.HomeNationUnrest:
			return Loc.T("UI.Traits.HomeNationUnrest", new object[] { TemplateManager.global.unrestInlineSpritePath });
		case StatModVariable.HomeNationArmies:
			return Loc.T("UI.Traits.HomeNationArmies");
		case StatModVariable.HomeNationDemocracy:
			return Loc.T("UI.Traits.HomeNationDemocracy", new object[] { TemplateManager.global.democracyInlineSpritePath });
		case StatModVariable.HomeNationEducation:
			return Loc.T("UI.Traits.HomeNationEducation", new object[] { TemplateManager.global.educationInlineSpritePath });
		case StatModVariable.HomeNationMiltech:
			return Loc.T("UI.Traits.HomeNationMiltech", new object[] { TemplateManager.global.miltechInlineSpritePath });
		case StatModVariable.GlobalMaxMiltech:
			return Loc.T("UI.Traits.GlobalMaxMiltech", new object[] { TemplateManager.global.miltechInlineSpritePath });
		case StatModVariable.GlobalTemperatureAnomaly:
			return Loc.T("UI.Traits.GlobalTemperatureAnomaly");
		}
		return "Missing pervalue string";
	}

	// Token: 0x17000128 RID: 296
	// (get) Token: 0x0600092A RID: 2346 RVA: 0x0002B51C File Offset: 0x0002971C
	public string fullTraitSummary
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(1200);
			stringBuilder.AppendLine(this.displayName);
			stringBuilder.AppendLine(this.description);
			StringBuilder stringBuilder2 = new StringBuilder(256);
			if (this.specialTraitRule != SpecialTraitRule.None)
			{
				string text = Loc.T(new StringBuilder("UI.Traits.").Append(this.specialTraitRule.ToString()).ToString(), new object[]
				{
					this.specialTraitRuleValue,
					(this.specialTraitRuleValue / 100f).ToPercent("P0")
				});
				if (!string.IsNullOrEmpty(text))
				{
					stringBuilder.AppendLine(text);
				}
			}
			bool flag = false;
			if (this.incomeMoney != 0f)
			{
				stringBuilder2.Append(TemplateManager.global.moneyInlineSpritePath + this.incomeMoney.ToString());
				flag = true;
			}
			if (this.incomeInfluence != 0f)
			{
				stringBuilder2.Append(TemplateManager.global.influenceInlineSpritePath + this.incomeInfluence.ToString());
				flag = true;
			}
			if (this.incomeOps != 0f)
			{
				stringBuilder2.Append(TemplateManager.global.opsInlineSpritePath + this.incomeOps.ToString());
				flag = true;
			}
			if (this.incomeBoost != 0f)
			{
				stringBuilder2.Append(TemplateManager.global.boostInlineSpritePath + this.incomeBoost.ToString());
				flag = true;
			}
			if (this.incomeResearch != 0f)
			{
				stringBuilder2.Append(TemplateManager.global.researchInlineSpritePath + this.incomeResearch.ToString());
				flag = true;
			}
			if (this.incomeProjects != 0)
			{
				stringBuilder2.Append(TemplateManager.global.projectsInlineSpritePath + this.incomeProjects.ToString());
				flag = true;
			}
			if (flag)
			{
				string text2 = Loc.T("UI.Councilor.MonthlyIncome", new object[] { stringBuilder2.ToString() });
				stringBuilder.AppendLine(text2);
			}
			if (this.detectionInvBonus > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.DetectionBonus", new object[] { this.detectionInvBonus }));
			}
			else if (this.detectionInvBonus < 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.DetectionMalus", new object[] { this.detectionInvBonus }));
			}
			if (this.detectionEspBonus > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.EnemyDetectionMalus", new object[] { this.detectionEspBonus }));
			}
			else if (this.detectionEspBonus < 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.EnemyDetectionBonus", new object[] { this.detectionEspBonus }));
			}
			foreach (StatModifier statModifier in this.statMods)
			{
				stringBuilder2.Clear();
				if (statModifier.stat != CouncilorAttribute.None)
				{
					string attributeString = TIUtilities.GetAttributeString(statModifier.stat);
					switch (statModifier.operation)
					{
					case StatModSetOperation.SetToFixedValue:
						stringBuilder2.Append(Loc.T("UI.Traits.SetToFixedValue", new object[] { attributeString, statModifier.strValue }));
						break;
					case StatModSetOperation.IncreaseToValue:
						stringBuilder2.Append(Loc.T("UI.Traits.IncreaseToValue", new object[] { attributeString, statModifier.strValue }));
						break;
					case StatModSetOperation.DecreaseToValue:
						stringBuilder2.Append(Loc.T("UI.Traits.DecreaseToValue", new object[] { attributeString, statModifier.strValue }));
						break;
					case StatModSetOperation.Additive:
						if (statModifier.modifierValue >= 0)
						{
							stringBuilder2.Append(Loc.T("UI.Traits.AdditiveStatBonusPlus", new object[] { statModifier.strValue, attributeString }));
						}
						else
						{
							stringBuilder2.Append(Loc.T("UI.Traits.AdditiveStatBonusMinus", new object[] { statModifier.strValue, attributeString }));
						}
						break;
					case StatModSetOperation.AdditivePer:
						stringBuilder2.Append(Loc.T("UI.Traits.AdditivePer", new object[]
						{
							attributeString,
							this.GetPerValueString(this.GetStatModVariable(statModifier))
						}));
						break;
					case StatModSetOperation.SubtractivePer:
						stringBuilder2.Append(Loc.T("UI.Traits.SubtractivePer", new object[]
						{
							attributeString,
							this.GetPerValueString(this.GetStatModVariable(statModifier))
						}));
						break;
					case StatModSetOperation.Multiplicative:
						stringBuilder2.Append(Loc.T("UI.Traits.Multiplicative", new object[]
						{
							attributeString,
							statModifier.modifierValue.ToPercent("P0")
						}));
						break;
					case StatModSetOperation.SetToAnotherAttribute:
						stringBuilder2.Append(Loc.T("UI.Traits.SetToAnotherAttribute", new object[]
						{
							attributeString,
							TIUtilities.GetAttributeString(statModifier.strValue.ToEnum(CouncilorAttribute.None))
						}));
						break;
					}
					if (statModifier.conditionalModifier)
					{
						stringBuilder2.Append(statModifier.condition.GetDescription());
					}
					stringBuilder.AppendLine(stringBuilder2.ToString());
				}
			}
			foreach (TechBonus techBonus in this.techBonuses)
			{
				if (techBonus.bonus > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Traits.TechBonus", new object[]
					{
						techBonus.bonus.ToPercent("P0"),
						TIGenericTechTemplate.GetTechCategoryString(techBonus.category)
					}));
				}
				else if (techBonus.bonus < 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Traits.TechMalus", new object[]
					{
						techBonus.bonus.ToPercent("P0"),
						TIGenericTechTemplate.GetTechCategoryString(techBonus.category)
					}));
				}
			}
			foreach (PriorityBonus priorityBonus in this.priorityBonuses)
			{
				if (priorityBonus.bonus > 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Traits.PriorityBonus", new object[]
					{
						priorityBonus.bonus.ToPercent("P0"),
						TIUtilities.GetPriorityString(priorityBonus.priority, true)
					}));
				}
				else if (priorityBonus.bonus < 0f)
				{
					stringBuilder.AppendLine(Loc.T("UI.Traits.PriorityMalus", new object[]
					{
						priorityBonus.bonus.ToPercent("P0"),
						TIUtilities.GetPriorityString(priorityBonus.priority, true)
					}));
				}
			}
			if (this.XPModifier > 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.XPPenalty", new object[] { this.XPModifier.ToPercent("P0") }));
			}
			else if (this.XPModifier < 0f)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.XPBonus", new object[] { this.XPModifier.ToPercent("P0") }));
			}
			List<TIMissionTemplate> missionsGranted = this.MissionsGranted;
			if (missionsGranted.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.MissionsGranted"));
				foreach (TIMissionTemplate timissionTemplate in missionsGranted)
				{
					stringBuilder.AppendLine(timissionTemplate.displayName);
				}
			}
			List<TIMissionTemplate> restrictedMissions = this.RestrictedMissions;
			if (restrictedMissions.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Traits.MissionsProhibited"));
				foreach (TIMissionTemplate timissionTemplate2 in restrictedMissions)
				{
					stringBuilder.AppendLine(timissionTemplate2.displayName);
				}
			}
			if (this.restrictedLocations != RestrictedLocations.None)
			{
				stringBuilder.AppendLine(TITraitTemplate.RestrictedLocationString(this));
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x0002BD28 File Offset: 0x00029F28
	public static string RestrictedLocationString(TITraitTemplate trait)
	{
		RestrictedLocations restrictedLocations = trait.restrictedLocations;
		if (restrictedLocations == RestrictedLocations.None)
		{
			return string.Empty;
		}
		if (restrictedLocations != RestrictedLocations.HighUnrestNations)
		{
			return Loc.T(new StringBuilder("UI.Traits." + trait.restrictedLocations.ToString()).ToString());
		}
		return Loc.T(new StringBuilder("UI.Traits." + trait.restrictedLocations.ToString()).ToString(), new object[]
		{
			TemplateManager.global.HighUnrestDefinition.ToString("N1"),
			TemplateManager.global.unrestInlineSpritePath
		});
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x0002BDCC File Offset: 0x00029FCC
	public static void ProcessLoyaltyChangeFromTraits(TIFactionState faction, SpecialTraitRule rule, int multiplier = 1)
	{
		foreach (TICouncilorState ticouncilorState in faction.councilors)
		{
			TITraitTemplate.ProcessLoyaltyChangeFromTraits(ticouncilorState, rule, multiplier);
		}
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x0002BE20 File Offset: 0x0002A020
	public static void ProcessPropagandaFromTraits(TIFactionState faction, SpecialTraitRule rule, float value)
	{
		if (rule == SpecialTraitRule.GlobalPropagandaIfKilled)
		{
			TINationState.GlobalPropaganda(faction.ideology, value);
		}
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x0002BE34 File Offset: 0x0002A034
	public static void ProcessLoyaltyChangeFromTraits(TICouncilorState councilor, SpecialTraitRule rule, int multiplier = 1)
	{
		TITraitTemplate traitWithSpecialTraitRule = councilor.GetTraitWithSpecialTraitRule(rule);
		if (traitWithSpecialTraitRule != null)
		{
			councilor.ModifyAttribute(CouncilorAttribute.Loyalty, (int)traitWithSpecialTraitRule.specialTraitRuleValue * multiplier);
			councilor.ModifyAttribute(CouncilorAttribute.ApparentLoyalty, (int)traitWithSpecialTraitRule.specialTraitRuleValue * multiplier);
		}
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x0002BE70 File Offset: 0x0002A070
	private StatModVariable GetStatModVariable(StatModifier statModifier)
	{
		string text = statModifier.strValue.ToLowerInvariant();
		if (text != null)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 2184636054U)
			{
				if (num <= 1342290998U)
				{
					if (num <= 1200442904U)
					{
						if (num != 131291080U)
						{
							if (num == 1200442904U)
							{
								if (text == "homenationarmies")
								{
									return StatModVariable.HomeNationArmies;
								}
							}
						}
						else if (text == "myfactionatrocities")
						{
							return StatModVariable.MyFactionAtrocities;
						}
					}
					else if (num != 1285318787U)
					{
						if (num == 1342290998U)
						{
							if (text == "myfactioncouncilors")
							{
								return StatModVariable.MyFactionCouncilors;
							}
						}
					}
					else if (text == "myfactionmaxprojects")
					{
						return StatModVariable.MyFactionMaxProjects;
					}
				}
				else if (num <= 1964499971U)
				{
					if (num != 1364683584U)
					{
						if (num == 1964499971U)
						{
							if (text == "myfactionaliensremoved")
							{
								return StatModVariable.MyFactionAliensRemoved;
							}
						}
					}
					else if (text == "myfactionturnedcouncilors")
					{
						return StatModVariable.MyFactionTurnedCouncilors;
					}
				}
				else if (num != 2001299409U)
				{
					if (num == 2184636054U)
					{
						if (text == "homenationdemocracy")
						{
							return StatModVariable.HomeNationDemocracy;
						}
					}
				}
				else if (text == "homenationmiltech")
				{
					return StatModVariable.HomeNationMiltech;
				}
			}
			else if (num <= 3700752394U)
			{
				if (num <= 3255149469U)
				{
					if (num != 3101693376U)
					{
						if (num == 3255149469U)
						{
							if (text == "globaltemperatureanomaly")
							{
								return StatModVariable.GlobalTemperatureAnomaly;
							}
						}
					}
					else if (text == "myfactionarmies")
					{
						return StatModVariable.MyFactionArmies;
					}
				}
				else if (num != 3535564509U)
				{
					if (num == 3700752394U)
					{
						if (text == "homenationwars")
						{
							return StatModVariable.HomeNationWars;
						}
					}
				}
				else if (text == "homenationeducation")
				{
					return StatModVariable.HomeNationEducation;
				}
			}
			else if (num <= 4083736278U)
			{
				if (num != 3790840016U)
				{
					if (num == 4083736278U)
					{
						if (text == "myfactionarmieslost")
						{
							return StatModVariable.MyFactionArmiesLost;
						}
					}
				}
				else if (text == "homenationunrest")
				{
					return StatModVariable.HomeNationUnrest;
				}
			}
			else if (num != 4139179537U)
			{
				if (num != 4192374462U)
				{
					if (num == 4231009087U)
					{
						if (text == "homenationfederationmembers")
						{
							return StatModVariable.HomeNationFederationMembers;
						}
					}
				}
				else if (text == "globalmaxmiltech")
				{
					return StatModVariable.GlobalMaxMiltech;
				}
			}
			else if (text == "loyalty")
			{
				return StatModVariable.Loyalty;
			}
		}
		return StatModVariable.none;
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0002C11C File Offset: 0x0002A31C
	public int ApplyTraitStatValue(CouncilorAttribute attribute, TICouncilorState councilorWithTrait, TIFactionState viewingFaction, WhichStatModifier whichStatModifier, bool missionTargeting, TIGameState missionTarget = null)
	{
		int num = 0;
		if (attribute != CouncilorAttribute.None)
		{
			List<StatModifier> list = (from x in this.statMods
				where x.stat == attribute
				orderby (int)x.operation
				select x).ToList<StatModifier>();
			if (list.Count > 0)
			{
				TIFactionState tifactionState = ((missionTarget != null) ? missionTarget.ref_faction : null) ?? null;
				TINationState tinationState = councilorWithTrait.homeNation ?? null;
				int attribute2 = councilorWithTrait.GetAttribute(attribute, false, false, true, false, false, false);
				foreach (StatModifier statModifier in list)
				{
					bool flag = false;
					bool conditionalModifier = statModifier.conditionalModifier;
					if (whichStatModifier != WhichStatModifier.UnconditionalOnly && conditionalModifier)
					{
						flag = TITraitTemplate.<ApplyTraitStatValue>g__PassesTraitCondition|64_2(statModifier, viewingFaction, councilorWithTrait, missionTargeting, missionTarget);
					}
					else if (whichStatModifier != WhichStatModifier.ConditionalOnly && !conditionalModifier)
					{
						flag = true;
					}
					if (flag)
					{
						switch (statModifier.operation)
						{
						case StatModSetOperation.SetToFixedValue:
							num = statModifier.modifierValue - attribute2;
							break;
						case StatModSetOperation.Additive:
							num += statModifier.modifierValue;
							break;
						case StatModSetOperation.AdditivePer:
							switch (this.GetStatModVariable(statModifier))
							{
							case StatModVariable.MyFactionAtrocities:
								num += ((tifactionState != null) ? Math.Min(tifactionState.atrocities, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeRegionNuked:
							{
								int num2 = num;
								TIRegionState homeRegion = councilorWithTrait.homeRegion;
								num = num2 + ((homeRegion != null) ? homeRegion.nuclearDetonations : 0);
								break;
							}
							case StatModVariable.MyFactionCouncilors:
								num += ((tifactionState != null) ? Math.Min(tifactionState.numActiveCouncilors, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.MyFactionTurnedCouncilors:
								num += ((tifactionState != null) ? Math.Min(tifactionState.turnedCouncilors.Count, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.MyFactionArmies:
								num += ((tifactionState != null) ? Math.Min(tifactionState.armies.Count, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.MyFactionArmiesLost:
							{
								int num3 = num;
								int num4;
								if (!(tifactionState != null))
								{
									num4 = 0;
								}
								else
								{
									num4 = Math.Min(tifactionState.armiesLost.Sum<KeyValuePair<ArmyType, int>>((KeyValuePair<ArmyType, int> x) => x.Value), TemplateManager.global.additivePerModifierCap);
								}
								num = num3 + num4;
								break;
							}
							case StatModVariable.MyFactionAliensRemoved:
								num += ((tifactionState != null) ? Math.Min(tifactionState.aliensRemoved, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.MyFactionMaxProjects:
								num += ((tifactionState != null) ? tifactionState.GetMaxSimultaneousProjects() : 1);
								break;
							case StatModVariable.HomeNationWars:
								num += ((tinationState != null) ? Math.Min(tinationState.wars.Count, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeNationFederationMembers:
								num += ((tinationState != null && tinationState.inFederation) ? Math.Min(tinationState.federation.members.Count, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeNationUnrest:
								num += ((tinationState != null) ? Math.Min((int)tinationState.unrest, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeNationArmies:
								num += ((tinationState != null) ? Math.Min(tinationState.armies.Count, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeNationDemocracy:
								num += ((tinationState != null) ? Math.Min((int)tinationState.democracy, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeNationEducation:
								num += ((tinationState != null) ? Math.Min((int)tinationState.education, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.HomeNationMiltech:
								num += ((tinationState != null) ? Math.Min((int)tinationState.militaryTechLevel, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.GlobalMaxMiltech:
								num += ((tinationState != null) ? Math.Min((int)tinationState.maxMilitaryTechLevel, TemplateManager.global.additivePerModifierCap) : 0);
								break;
							case StatModVariable.GlobalTemperatureAnomaly:
								num += (int)TIGlobalValuesState.GlobalValues.temperatureAnomaly_C;
								break;
							}
							break;
						case StatModSetOperation.SubtractivePer:
							switch (this.GetStatModVariable(statModifier))
							{
							case StatModVariable.MyFactionAtrocities:
								num -= ((tifactionState != null) ? tifactionState.atrocities : 0);
								break;
							case StatModVariable.HomeRegionNuked:
							{
								int num5 = num;
								TIRegionState homeRegion2 = councilorWithTrait.homeRegion;
								num = num5 - ((homeRegion2 != null) ? homeRegion2.nuclearDetonations : 0);
								break;
							}
							case StatModVariable.MyFactionCouncilors:
								num -= ((tifactionState != null) ? tifactionState.numActiveCouncilors : 0);
								break;
							case StatModVariable.MyFactionTurnedCouncilors:
								num -= ((tifactionState != null) ? tifactionState.turnedCouncilors.Count : 0);
								break;
							case StatModVariable.MyFactionArmies:
								num -= ((tifactionState != null) ? tifactionState.armies.Count : 0);
								break;
							case StatModVariable.MyFactionArmiesLost:
							{
								int num6 = num;
								int num7;
								if (!(tifactionState != null))
								{
									num7 = 0;
								}
								else
								{
									num7 = tifactionState.armiesLost.Sum<KeyValuePair<ArmyType, int>>((KeyValuePair<ArmyType, int> x) => x.Value);
								}
								num = num6 - num7;
								break;
							}
							case StatModVariable.MyFactionAliensRemoved:
								num -= ((tifactionState != null) ? tifactionState.aliensRemoved : 0);
								break;
							case StatModVariable.MyFactionMaxProjects:
								num -= ((tifactionState != null) ? tifactionState.GetMaxSimultaneousProjects() : 1);
								break;
							case StatModVariable.HomeNationWars:
								num -= ((tinationState != null) ? tinationState.wars.Count : 0);
								break;
							case StatModVariable.HomeNationFederationMembers:
								num -= ((tinationState != null && tinationState.inFederation) ? tinationState.federation.members.Count : 0);
								break;
							case StatModVariable.HomeNationUnrest:
								num -= ((tinationState != null) ? ((int)tinationState.unrest) : 0);
								break;
							case StatModVariable.HomeNationArmies:
								num -= ((tinationState != null) ? tinationState.armies.Count : 0);
								break;
							case StatModVariable.HomeNationDemocracy:
								num -= ((tinationState != null) ? ((int)tinationState.democracy) : 0);
								break;
							case StatModVariable.HomeNationEducation:
								num -= ((tinationState != null) ? ((int)tinationState.education) : 0);
								break;
							case StatModVariable.HomeNationMiltech:
								num -= ((tinationState != null) ? ((int)tinationState.militaryTechLevel) : 0);
								break;
							case StatModVariable.GlobalMaxMiltech:
								num -= ((tinationState != null) ? ((int)tinationState.maxMilitaryTechLevel) : 0);
								break;
							case StatModVariable.GlobalTemperatureAnomaly:
								num -= (int)TIGlobalValuesState.GlobalValues.temperatureAnomaly_C;
								break;
							}
							break;
						case StatModSetOperation.Multiplicative:
							num = attribute2 * statModifier.modifierValue - attribute2;
							break;
						case StatModSetOperation.SetToAnotherAttribute:
							if (this.GetStatModVariable(statModifier) == StatModVariable.Loyalty)
							{
								num = councilorWithTrait.GetAttribute(CouncilorAttribute.Loyalty, false, false, true, false, false, false) - attribute2;
							}
							break;
						}
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x0002C924 File Offset: 0x0002AB24
	[CompilerGenerated]
	internal static bool <ApplyTraitStatValue>g__PassesTraitCondition|64_2(StatModifier statModifier, TIFactionState viewingFaction, TICouncilorState councilorWithTrait, bool missionTargeting, TIGameState missionTarget)
	{
		TICondition condition = statModifier.condition;
		if (condition == null)
		{
			return true;
		}
		if (!missionTargeting)
		{
			return condition.PassesCondition(councilorWithTrait);
		}
		if (viewingFaction != councilorWithTrait.faction && (condition.ConditionTarget() == ConditionTargetType.nation || condition.ConditionTarget() == ConditionTargetType.region))
		{
			return condition.TargetPassesCondition(councilorWithTrait, TIMissionPhaseState.CouncilorLastKnownLocation(viewingFaction, councilorWithTrait));
		}
		return condition.TargetPassesCondition(councilorWithTrait, missionTarget);
	}

	// Token: 0x040006F5 RID: 1781
	public int? grouping;

	// Token: 0x040006F6 RID: 1782
	public int XPCost;

	// Token: 0x040006F7 RID: 1783
	public int moneyCost;

	// Token: 0x040006F8 RID: 1784
	public int influenceCost;

	// Token: 0x040006F9 RID: 1785
	public int opsCost;

	// Token: 0x040006FA RID: 1786
	public int boostCost;

	// Token: 0x040006FB RID: 1787
	public string projectDataName;

	// Token: 0x040006FC RID: 1788
	public string upgradesFrom;

	// Token: 0x040006FD RID: 1789
	public float incomeMoney;

	// Token: 0x040006FE RID: 1790
	public float incomeInfluence;

	// Token: 0x040006FF RID: 1791
	public float incomeOps;

	// Token: 0x04000700 RID: 1792
	public float incomeBoost;

	// Token: 0x04000701 RID: 1793
	public float incomeResearch;

	// Token: 0x04000702 RID: 1794
	public int incomeProjects;

	// Token: 0x04000703 RID: 1795
	public int detectionInvBonus;

	// Token: 0x04000704 RID: 1796
	public int detectionEspBonus;

	// Token: 0x04000705 RID: 1797
	public float XPModifier;

	// Token: 0x04000706 RID: 1798
	public string rerollTrait;

	// Token: 0x04000707 RID: 1799
	public float rerollTraitBonus;

	// Token: 0x04000708 RID: 1800
	public bool randomCouncilorsOnly;

	// Token: 0x04000709 RID: 1801
	public StatModifier[] statMods;

	// Token: 0x0400070A RID: 1802
	public List<string> missionsGrantedNames = new List<string>();

	// Token: 0x0400070B RID: 1803
	public List<string> restrictedMissionNames = new List<string>();

	// Token: 0x0400070C RID: 1804
	public float? baseChance = new float?(0f);

	// Token: 0x0400070D RID: 1805
	public RestrictedLocations restrictedLocations;

	// Token: 0x0400070E RID: 1806
	public List<TechBonus> techBonuses = new List<TechBonus>();

	// Token: 0x0400070F RID: 1807
	public List<PriorityBonus> priorityBonuses = new List<PriorityBonus>();

	// Token: 0x04000710 RID: 1808
	public List<ClassChance> classChance = new List<ClassChance>();

	// Token: 0x04000711 RID: 1809
	public bool easilyVisible;

	// Token: 0x04000712 RID: 1810
	public SpecialTraitRule specialTraitRule;

	// Token: 0x04000713 RID: 1811
	public float specialTraitRuleValue;

	// Token: 0x04000714 RID: 1812
	public List<string> tags = new List<string>();

	// Token: 0x04000715 RID: 1813
	public bool alwaysGrantFromEffect;

	// Token: 0x04000716 RID: 1814
	private List<TIMissionTemplate> _restrictedMissions;

	// Token: 0x04000717 RID: 1815
	private List<TIMissionTemplate> _missionsGranted;
}
