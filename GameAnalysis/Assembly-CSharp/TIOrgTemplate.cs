using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000295 RID: 661
public class TIOrgTemplate : TIDataTemplate
{
	// Token: 0x06000911 RID: 2321 RVA: 0x0002AC3C File Offset: 0x00028E3C
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIOrgState>();
		}
		return tigameState;
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x0002AC60 File Offset: 0x00028E60
	public override bool IsValid(out string error)
	{
		error = string.Empty;
		if (this.orgType == OrgType.Any)
		{
			error = "Must set orgType for " + base.dataName;
			return false;
		}
		return true;
	}

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x06000913 RID: 2323 RVA: 0x0002AC86 File Offset: 0x00028E86
	public TITechTemplate requiredTechTemplate
	{
		get
		{
			if (!string.IsNullOrEmpty(this.requiredTechName))
			{
				return TemplateManager.Find<TITechTemplate>(this.requiredTechName, false);
			}
			return null;
		}
	}

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x06000914 RID: 2324 RVA: 0x0002ACA3 File Offset: 0x00028EA3
	public TIProjectTemplate projectGranted
	{
		get
		{
			if (!string.IsNullOrEmpty(this.projectGrantedName))
			{
				return TemplateManager.Find<TIProjectTemplate>(this.projectGrantedName, false);
			}
			return null;
		}
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000915 RID: 2325 RVA: 0x0002ACC0 File Offset: 0x00028EC0
	public List<TITraitTemplate> requiredTraitTemplates
	{
		get
		{
			List<TITraitTemplate> list = new List<TITraitTemplate>();
			foreach (string text in this.requiredOwnerTraits)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(text, false);
					if (titraitTemplate != null)
					{
						list.Add(titraitTemplate);
					}
					else
					{
						Log.Error("Bad traitName " + text + " in requiredTraitTemplates in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			return list;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000916 RID: 2326 RVA: 0x0002AD2C File Offset: 0x00028F2C
	public List<TITraitTemplate> prohibitedTraitTemplates
	{
		get
		{
			List<TITraitTemplate> list = new List<TITraitTemplate>();
			foreach (string text in this.prohibitedOwnerTraits)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TITraitTemplate titraitTemplate = TemplateManager.Find<TITraitTemplate>(text, false);
					if (titraitTemplate != null)
					{
						list.Add(titraitTemplate);
					}
					else
					{
						Log.Error("Bad traitName " + text + " in prohibitedOwnerTraits in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			return list;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000917 RID: 2327 RVA: 0x0002AD98 File Offset: 0x00028F98
	public override string displayName
	{
		get
		{
			if (this._displayName == null)
			{
				this._displayName = Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayName.").Append(base.localizationName).ToString());
			}
			return this._displayName;
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000918 RID: 2328 RVA: 0x0002ADE8 File Offset: 0x00028FE8
	public string displayNameWithArticle
	{
		get
		{
			string text = new StringBuilder("TIOrgTemplate.displayNameWithArticle.").Append(base.localizationName).ToString();
			string text2 = Loc.T_Scenario(text);
			if (!(text2 == text))
			{
				return text2;
			}
			return this.displayName;
		}
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x0002AE28 File Offset: 0x00029028
	public bool CanSpawn()
	{
		return this.requiredTechTemplate == null || (this.requiredTechTemplate != null && GameStateManager.GlobalResearch().IsTechFinished(this.requiredTechTemplate));
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x0600091A RID: 2330 RVA: 0x0002AE50 File Offset: 0x00029050
	public List<TIMissionTemplate> missionsGranted
	{
		get
		{
			List<TIMissionTemplate> list = new List<TIMissionTemplate>();
			foreach (string text in this.missionsGrantedNames)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TIMissionTemplate timissionTemplate = TemplateManager.Find<TIMissionTemplate>(text, false);
					if (timissionTemplate != null)
					{
						if (!list.Contains(timissionTemplate))
						{
							list.Add(timissionTemplate);
						}
					}
					else
					{
						Log.Error("Bad mission entry " + text + " in " + base.dataName, Array.Empty<object>());
					}
				}
			}
			return list;
		}
	}

	// Token: 0x04000677 RID: 1655
	public bool randomized;

	// Token: 0x04000678 RID: 1656
	public OrgType orgType;

	// Token: 0x04000679 RID: 1657
	public int tier;

	// Token: 0x0400067A RID: 1658
	public float takeoverDefense;

	// Token: 0x0400067B RID: 1659
	public string homeRegionMapTemplateName;

	// Token: 0x0400067C RID: 1660
	public bool requiresNationality;

	// Token: 0x0400067D RID: 1661
	public string[] requiredOwnerTraits = new string[0];

	// Token: 0x0400067E RID: 1662
	public string[] prohibitedOwnerTraits = new string[0];

	// Token: 0x0400067F RID: 1663
	public string requiredTechName;

	// Token: 0x04000680 RID: 1664
	public bool allowedOnMarket;

	// Token: 0x04000681 RID: 1665
	public List<FactionIdeology> affinities = new List<FactionIdeology>();

	// Token: 0x04000682 RID: 1666
	public List<FactionIdeology> restricted = new List<FactionIdeology>();

	// Token: 0x04000683 RID: 1667
	public float costMoney;

	// Token: 0x04000684 RID: 1668
	public int randCostMoney;

	// Token: 0x04000685 RID: 1669
	public float costInfluence;

	// Token: 0x04000686 RID: 1670
	public int randCostInfluence;

	// Token: 0x04000687 RID: 1671
	public float costOps;

	// Token: 0x04000688 RID: 1672
	public int randCostOps;

	// Token: 0x04000689 RID: 1673
	public float costBoost;

	// Token: 0x0400068A RID: 1674
	public int randCostBoost;

	// Token: 0x0400068B RID: 1675
	public float chanceIncomeMoney;

	// Token: 0x0400068C RID: 1676
	public float incomeMoney;

	// Token: 0x0400068D RID: 1677
	public int randIncomeMoney;

	// Token: 0x0400068E RID: 1678
	public float chanceIncomeInfluence;

	// Token: 0x0400068F RID: 1679
	public float incomeInfluence;

	// Token: 0x04000690 RID: 1680
	public int randIncomeInfluence;

	// Token: 0x04000691 RID: 1681
	public float chanceIncomeOps;

	// Token: 0x04000692 RID: 1682
	public float incomeOps;

	// Token: 0x04000693 RID: 1683
	public int randIncomeOps;

	// Token: 0x04000694 RID: 1684
	public float chanceIncomeBoost;

	// Token: 0x04000695 RID: 1685
	public float incomeBoost;

	// Token: 0x04000696 RID: 1686
	public int randIncomeBoost;

	// Token: 0x04000697 RID: 1687
	public float chanceIncomeMissionControl;

	// Token: 0x04000698 RID: 1688
	public float incomeMissionControl;

	// Token: 0x04000699 RID: 1689
	public int randIncomeMissionControl;

	// Token: 0x0400069A RID: 1690
	public float chanceIncomeResearch;

	// Token: 0x0400069B RID: 1691
	public float incomeResearch;

	// Token: 0x0400069C RID: 1692
	public int randIncomeResearch;

	// Token: 0x0400069D RID: 1693
	public int projectsGranted;

	// Token: 0x0400069E RID: 1694
	public float XPModifier;

	// Token: 0x0400069F RID: 1695
	public float chancePersuasion;

	// Token: 0x040006A0 RID: 1696
	public int persuasion;

	// Token: 0x040006A1 RID: 1697
	public int randPersuasion;

	// Token: 0x040006A2 RID: 1698
	public float chanceCommand;

	// Token: 0x040006A3 RID: 1699
	public int command;

	// Token: 0x040006A4 RID: 1700
	public int randCommand;

	// Token: 0x040006A5 RID: 1701
	public float chanceInvestigation;

	// Token: 0x040006A6 RID: 1702
	public int investigation;

	// Token: 0x040006A7 RID: 1703
	public int randInvestigation;

	// Token: 0x040006A8 RID: 1704
	public float chanceEspionage;

	// Token: 0x040006A9 RID: 1705
	public int espionage;

	// Token: 0x040006AA RID: 1706
	public int randEspionage;

	// Token: 0x040006AB RID: 1707
	public float chanceAdministration;

	// Token: 0x040006AC RID: 1708
	public int administration;

	// Token: 0x040006AD RID: 1709
	public int randAdministration;

	// Token: 0x040006AE RID: 1710
	public float chanceScience;

	// Token: 0x040006AF RID: 1711
	public int science;

	// Token: 0x040006B0 RID: 1712
	public int randScience;

	// Token: 0x040006B1 RID: 1713
	public float chanceSecurity;

	// Token: 0x040006B2 RID: 1714
	public int security;

	// Token: 0x040006B3 RID: 1715
	public int randSecurity;

	// Token: 0x040006B4 RID: 1716
	public float chanceEconomyBonus;

	// Token: 0x040006B5 RID: 1717
	public float economyBonus;

	// Token: 0x040006B6 RID: 1718
	public float randEconomyBonus;

	// Token: 0x040006B7 RID: 1719
	public float chanceWelfareBonus;

	// Token: 0x040006B8 RID: 1720
	public float welfareBonus;

	// Token: 0x040006B9 RID: 1721
	public float randWelfareBonus;

	// Token: 0x040006BA RID: 1722
	public float chanceEnvironmentBonus;

	// Token: 0x040006BB RID: 1723
	public float environmentBonus;

	// Token: 0x040006BC RID: 1724
	public float randEnvironmentBonus;

	// Token: 0x040006BD RID: 1725
	public float chanceKnowledgeBonus;

	// Token: 0x040006BE RID: 1726
	public float knowledgeBonus;

	// Token: 0x040006BF RID: 1727
	public float randKnowledgeBonus;

	// Token: 0x040006C0 RID: 1728
	public float chanceGovernmentBonus;

	// Token: 0x040006C1 RID: 1729
	public float governmentBonus;

	// Token: 0x040006C2 RID: 1730
	public float randGovernmentBonus;

	// Token: 0x040006C3 RID: 1731
	public float chanceUnityBonus;

	// Token: 0x040006C4 RID: 1732
	public float unityBonus;

	// Token: 0x040006C5 RID: 1733
	public float randUnityBonus;

	// Token: 0x040006C6 RID: 1734
	public float chanceMilitaryBonus;

	// Token: 0x040006C7 RID: 1735
	public float militaryBonus;

	// Token: 0x040006C8 RID: 1736
	public float randMilitaryBonus;

	// Token: 0x040006C9 RID: 1737
	public float chanceOppressionBonus;

	// Token: 0x040006CA RID: 1738
	public float oppressionBonus;

	// Token: 0x040006CB RID: 1739
	public float randOppressionBonus;

	// Token: 0x040006CC RID: 1740
	public float chanceSpoilsBonus;

	// Token: 0x040006CD RID: 1741
	public float spoilsBonus;

	// Token: 0x040006CE RID: 1742
	public float randSpoilsBonus;

	// Token: 0x040006CF RID: 1743
	public float chanceSpaceDevBonus;

	// Token: 0x040006D0 RID: 1744
	public float spaceDevBonus;

	// Token: 0x040006D1 RID: 1745
	public float randSpaceDevBonus;

	// Token: 0x040006D2 RID: 1746
	public float chanceSpaceflightBonus;

	// Token: 0x040006D3 RID: 1747
	public float spaceflightBonus;

	// Token: 0x040006D4 RID: 1748
	public float randSpaceflightBonus;

	// Token: 0x040006D5 RID: 1749
	public float chanceMCBonus;

	// Token: 0x040006D6 RID: 1750
	public float MCBonus;

	// Token: 0x040006D7 RID: 1751
	public float randMCBonus;

	// Token: 0x040006D8 RID: 1752
	public float chanceMiningBonus;

	// Token: 0x040006D9 RID: 1753
	public float miningBonus;

	// Token: 0x040006DA RID: 1754
	public float randMiningBonus;

	// Token: 0x040006DB RID: 1755
	public TechBonus[] techBonuses = new TechBonus[0];

	// Token: 0x040006DC RID: 1756
	public string[] missionsGrantedNames = new string[0];

	// Token: 0x040006DD RID: 1757
	public bool grantsMarked;

	// Token: 0x040006DE RID: 1758
	public string projectGrantedName;

	// Token: 0x040006DF RID: 1759
	public string iconResource;
}
