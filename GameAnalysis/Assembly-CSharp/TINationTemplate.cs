using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class TINationTemplate : TIDataTemplate
{
	// Token: 0x1700013D RID: 317
	// (get) Token: 0x06000971 RID: 2417 RVA: 0x0002FD34 File Offset: 0x0002DF34
	public Color UIColor
	{
		get
		{
			return new Color(this.color.r, this.color.g, this.color.b, 0.65f);
		}
	}

	// Token: 0x06000972 RID: 2418 RVA: 0x0002FD64 File Offset: 0x0002DF64
	public override TIGameState CreateGameState()
	{
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TINationState>();
		}
		return tigameState;
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x0002FD88 File Offset: 0x0002DF88
	public string startUpDisplayName()
	{
		return base.displayNameCurrentForStartScreen();
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x0002FD90 File Offset: 0x0002DF90
	public string startUpUnionDisplayName()
	{
		return this.unionDisplayName;
	}

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000975 RID: 2421 RVA: 0x0002FD98 File Offset: 0x0002DF98
	public override string displayName
	{
		get
		{
			if (this._displayName == null)
			{
				this._displayName = Loc.T_Scenario(new StringBuilder("TINationTemplate.displayName.").Append(base.localizationName).ToString());
			}
			return this._displayName;
		}
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0002FDD0 File Offset: 0x0002DFD0
	public bool IsStartingUnion(List<TINationTemplate> nationsInScenario, List<string> completedProjectsInScenario)
	{
		return this.unionTrigger > 0 && (from x in TemplateManager.IterateByClass<TIBilateralTemplate>(false)
			where x.nation1 == this.dataName && x.relationType == BilateralRelationType.Claim && x.initialOwner && (this.unionTrigger == 2 || !x.initialColony) && x.BilateralIsInScenario_FromTemplates(nationsInScenario, completedProjectsInScenario, false)
			select x).Count<TIBilateralTemplate>() >= this.unionTrigger;
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0002FE2C File Offset: 0x0002E02C
	public int StartingClaims(List<TINationTemplate> nationsInScenario, List<string> completedProjectsInScenario, bool countLockedClaims)
	{
		return (from x in TemplateManager.IterateByClass<TIBilateralTemplate>(false)
			where x.nation1 == this.dataName && x.relationType == BilateralRelationType.Claim && x.BilateralIsInScenario_FromTemplates(nationsInScenario, completedProjectsInScenario, countLockedClaims)
			select x).Count<TIBilateralTemplate>();
	}

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000978 RID: 2424 RVA: 0x0002FE77 File Offset: 0x0002E077
	public string displayNameWithArticle
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayNameWithArticle.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000140 RID: 320
	// (get) Token: 0x06000979 RID: 2425 RVA: 0x0002FEA8 File Offset: 0x0002E0A8
	public string nationAdjective
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".nationAdjective.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000141 RID: 321
	// (get) Token: 0x0600097A RID: 2426 RVA: 0x0002FED9 File Offset: 0x0002E0D9
	public string unionDisplayName
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".unionDisplayName.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000142 RID: 322
	// (get) Token: 0x0600097B RID: 2427 RVA: 0x0002FF0A File Offset: 0x0002E10A
	public string unionDisplayNameWithArticle
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".unionDisplayNameWithArticle.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x0600097C RID: 2428 RVA: 0x0002FF3B File Offset: 0x0002E13B
	public string unionAdjective
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".unionAdjective.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x0600097D RID: 2429 RVA: 0x0002FF6C File Offset: 0x0002E16C
	public string displayNameWithArticleAndPlacePrep
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".displayNameWithArticlePrep.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000145 RID: 325
	// (get) Token: 0x0600097E RID: 2430 RVA: 0x0002FF9D File Offset: 0x0002E19D
	public string unionDisplayNameWithArticleAndPlacePrep
	{
		get
		{
			return Loc.T_Scenario(new StringBuilder(base.GetType().Name).Append(".unionDisplayNameWithArticlePrep.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000146 RID: 326
	// (get) Token: 0x0600097F RID: 2431 RVA: 0x0002FFCE File Offset: 0x0002E1CE
	public TIFactionState initialFaction
	{
		get
		{
			if (!(this.initialFactionStr != string.Empty))
			{
				return null;
			}
			return GameStateManager.FindByTemplate<TIFactionState>(this.initialFactionStr, false);
		}
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x0002FFF0 File Offset: 0x0002E1F0
	public string GetUnionFlagResource()
	{
		if (!string.IsNullOrEmpty(this.unionFlagResource))
		{
			return this.unionFlagResource;
		}
		return this.flagResource;
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x0003000C File Offset: 0x0002E20C
	public TIPriorityPresetTemplate priorityPreset(int index)
	{
		return TemplateManager.Find<TIPriorityPresetTemplate>(this.initialPriorityPreset[index], false);
	}

	// Token: 0x0400081D RID: 2077
	public Color color;

	// Token: 0x0400081E RID: 2078
	public int unionTrigger;

	// Token: 0x0400081F RID: 2079
	public bool aggregateNation;

	// Token: 0x04000820 RID: 2080
	public string flagResource = "";

	// Token: 0x04000821 RID: 2081
	public string unionFlagResource = "";

	// Token: 0x04000822 RID: 2082
	public float popGrowthModifier;

	// Token: 0x04000823 RID: 2083
	public float greenEconomy = 1f;

	// Token: 0x04000824 RID: 2084
	public double? initialGDP;

	// Token: 0x04000825 RID: 2085
	public float? cohesion;

	// Token: 0x04000826 RID: 2086
	public float? unrest;

	// Token: 0x04000827 RID: 2087
	public float? inequality;

	// Token: 0x04000828 RID: 2088
	public float? democracy;

	// Token: 0x04000829 RID: 2089
	public float? education;

	// Token: 0x0400082A RID: 2090
	public string spaceProgram;

	// Token: 0x0400082B RID: 2091
	public float? spaceFunding_year;

	// Token: 0x0400082C RID: 2092
	public float? miltech;

	// Token: 0x0400082D RID: 2093
	public float? nuclearWeapons;

	// Token: 0x0400082E RID: 2094
	public float? foundMilitaryIPs;

	// Token: 0x0400082F RID: 2095
	public float? initSpaceIPs;

	// Token: 0x04000830 RID: 2096
	public float? nuclearProgramIPs;

	// Token: 0x04000831 RID: 2097
	public float? buildNukeIPs;

	// Token: 0x04000832 RID: 2098
	public float? buildArmyIPs;

	// Token: 0x04000833 RID: 2099
	public float? buildNavyIPs;

	// Token: 0x04000834 RID: 2100
	public string[] initialPriorityPreset = new string[6];

	// Token: 0x04000835 RID: 2101
	public string[] tankSeries = new string[8];

	// Token: 0x04000836 RID: 2102
	public string initialFactionStr;

	// Token: 0x04000837 RID: 2103
	public int? yearofHighestGDP;

	// Token: 0x04000838 RID: 2104
	public float highestPerCapitaGDP = 1f;

	// Token: 0x04000839 RID: 2105
	public List<string> ISOCodes = new List<string>();

	// Token: 0x0400083A RID: 2106
	public string solarBody;

	// Token: 0x0400083B RID: 2107
	public int group;

	// Token: 0x0400083C RID: 2108
	private string _dName;
}
