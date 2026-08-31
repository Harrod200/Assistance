using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002D0 RID: 720
public abstract class TIGenericTechTemplate : TIDataTemplate
{
	// Token: 0x06000A75 RID: 2677 RVA: 0x00033659 File Offset: 0x00031859
	public string GetCategoryIconPath()
	{
		return TIGenericTechTemplate.PathTechCategoryIcon(this.techCategory);
	}

	// Token: 0x17000163 RID: 355
	// (get) Token: 0x06000A76 RID: 2678 RVA: 0x00033666 File Offset: 0x00031866
	public string categoryString
	{
		get
		{
			return TIGenericTechTemplate.GetTechCategoryString(this.techCategory);
		}
	}

	// Token: 0x17000164 RID: 356
	// (get) Token: 0x06000A77 RID: 2679 RVA: 0x00033673 File Offset: 0x00031873
	public string categoryDescription
	{
		get
		{
			return Loc.T(new StringBuilder("UI.Science.CategoryDescription.").Append(this.techCategory.ToString()).ToString());
		}
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x0003369F File Offset: 0x0003189F
	protected virtual string filteredDescription(TechBenefitsContext context)
	{
		if (!(this.description == "<skip/>") && context != TechBenefitsContext.Prospective)
		{
			return this.description;
		}
		return string.Empty;
	}

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x06000A79 RID: 2681 RVA: 0x000336C2 File Offset: 0x000318C2
	protected virtual string description
	{
		get
		{
			return Loc.T(this.descriptionPath);
		}
	}

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000A7A RID: 2682 RVA: 0x000336CF File Offset: 0x000318CF
	protected string descriptionPath
	{
		get
		{
			return new StringBuilder(base.GetType().Name).Append(".description.").Append(base.localizationName).ToString();
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000A7B RID: 2683 RVA: 0x000336FB File Offset: 0x000318FB
	public virtual string summary
	{
		get
		{
			return Loc.T(new StringBuilder(base.GetType().Name).Append(".summary.").Append(base.localizationName).ToString());
		}
	}

	// Token: 0x17000168 RID: 360
	// (get) Token: 0x06000A7C RID: 2684 RVA: 0x0003372C File Offset: 0x0003192C
	public string IconResource
	{
		get
		{
			if (!string.IsNullOrEmpty(this.iconResource))
			{
				return this.iconResource;
			}
			return this.GetCategoryIconPath();
		}
	}

	// Token: 0x17000169 RID: 361
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00033748 File Offset: 0x00031948
	public string GetVoiceoverPath
	{
		get
		{
			if (!string.IsNullOrEmpty(this.voiceoverPath))
			{
				return this.voiceoverPath;
			}
			return base.dataName;
		}
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x00033764 File Offset: 0x00031964
	public static string GetTechCategoryString(TechCategory category)
	{
		return Loc.T(new StringBuilder("UI.Science.Category.").Append(category.ToString()).ToString());
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0003378C File Offset: 0x0003198C
	public static string GetTechCategoryDescription(TechCategory category)
	{
		return Loc.T(new StringBuilder("UI.Science.CategoryDescription.").Append(category.ToString()).ToString());
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x000337B4 File Offset: 0x000319B4
	public virtual string BenefitsDescription(TIFactionState faction, TechBenefitsContext benefitsContext, TIOrgState newOrg = null)
	{
		return string.Empty;
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x000337BB File Offset: 0x000319BB
	public virtual string WarningsDescription(TIFactionState faction, TechBenefitsContext context)
	{
		return string.Empty;
	}

	// Token: 0x06000A82 RID: 2690
	public abstract string GetCompletedIllustrationPath();

	// Token: 0x06000A83 RID: 2691
	public abstract bool isGlobalTech();

	// Token: 0x06000A84 RID: 2692
	public abstract bool isProject();

	// Token: 0x1700016A RID: 362
	// (get) Token: 0x06000A85 RID: 2693 RVA: 0x000337C2 File Offset: 0x000319C2
	public virtual TITechTemplate ref_tech
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700016B RID: 363
	// (get) Token: 0x06000A86 RID: 2694 RVA: 0x000337C5 File Offset: 0x000319C5
	public virtual TIProjectTemplate ref_project
	{
		get
		{
			return null;
		}
	}

	// Token: 0x1700016C RID: 364
	// (get) Token: 0x06000A87 RID: 2695 RVA: 0x000337C8 File Offset: 0x000319C8
	public bool noPrereqs
	{
		get
		{
			return this.prereqs.Count == 0 || string.IsNullOrEmpty(this.prereqs[0]);
		}
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x000337EA File Offset: 0x000319EA
	public virtual bool ShouldHide(TIFactionState faction)
	{
		return false;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x000337ED File Offset: 0x000319ED
	public void CachePrereqs()
	{
		List<TIGenericTechTemplate> techPrereqs = this.TechPrereqs;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x000337F8 File Offset: 0x000319F8
	public string GetFullDescription(TIFactionState faction, TechBenefitsContext context, TIOrgState newOrg = null, bool truncatedDescriptions = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = this.filteredDescription(context);
		if (!string.IsNullOrEmpty(text))
		{
			stringBuilder.AppendLine(text).AppendLine();
		}
		string text2 = this.WarningsDescription(faction, context);
		if (!string.IsNullOrEmpty(text2))
		{
			stringBuilder.AppendLine(text2);
		}
		string text3 = this.BenefitsDescription(faction, context, newOrg);
		if (!string.IsNullOrEmpty(text3))
		{
			stringBuilder.AppendLine(text3);
		}
		if (this.isProject())
		{
			stringBuilder.AppendLine().AppendLine(this.ref_project.AllUnlocksDetails(true, truncatedDescriptions));
		}
		return stringBuilder.Replace("\r\n\r\n\r\n", "\r\n\r\n").ToString();
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00033894 File Offset: 0x00031A94
	public static string PathTechCategoryIcon(TechCategory category)
	{
		switch (category)
		{
		case TechCategory.Materials:
			return TemplateManager.global.pathMaterialsIcon;
		case TechCategory.SpaceScience:
			return TemplateManager.global.pathSpaceScienceIcon;
		case TechCategory.Energy:
			return TemplateManager.global.pathEnergyIcon;
		case TechCategory.LifeScience:
			return TemplateManager.global.pathLifeScienceIcon;
		case TechCategory.MilitaryScience:
			return TemplateManager.global.pathMilitaryScienceIcon;
		case TechCategory.InformationScience:
			return TemplateManager.global.pathInformationScienceIcon;
		case TechCategory.SocialScience:
			return TemplateManager.global.pathSocialScienceIcon;
		case TechCategory.Xenology:
			return TemplateManager.global.pathXenologyIcon;
		default:
			return string.Empty;
		}
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x00033928 File Offset: 0x00031B28
	public static string categoryInlineSprite(TechCategory category)
	{
		switch (category)
		{
		case TechCategory.Materials:
			return TemplateManager.global.materialsTechInlineSpritePath;
		case TechCategory.SpaceScience:
			return TemplateManager.global.spaceTechInlineSpritePath;
		case TechCategory.Energy:
			return TemplateManager.global.energyTechInlineSpritePath;
		case TechCategory.LifeScience:
			return TemplateManager.global.lifeTechInlineSpritePath;
		case TechCategory.MilitaryScience:
			return TemplateManager.global.militaryTechInlineSpritePath;
		case TechCategory.InformationScience:
			return TemplateManager.global.informationTechInlineSpritePath;
		case TechCategory.SocialScience:
			return TemplateManager.global.socialTechInlineSpritePath;
		case TechCategory.Xenology:
			return TemplateManager.global.xenologyTechInlineSpritePath;
		default:
			return string.Empty;
		}
	}

	// Token: 0x06000A8D RID: 2701
	public abstract float GetResearchCost(TIFactionState faction);

	// Token: 0x1700016D RID: 365
	// (get) Token: 0x06000A8E RID: 2702 RVA: 0x000339BC File Offset: 0x00031BBC
	public List<TIEffectTemplate> Effects
	{
		get
		{
			List<TIEffectTemplate> list = new List<TIEffectTemplate>();
			foreach (string text in this.effects)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TIEffectTemplate tieffectTemplate = TemplateManager.Find<TIEffectTemplate>(text, false);
					if (tieffectTemplate != null)
					{
						list.Add(tieffectTemplate);
					}
					else
					{
						Log.Error("Bad effect Name in json: " + text + " in template " + base.dataName, Array.Empty<object>());
					}
				}
			}
			return list;
		}
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x00033A4C File Offset: 0x00031C4C
	public string UnlockableTechString(TIFactionState faction, TechBenefitsContext benefitsContext)
	{
		List<TIGenericTechTemplate> list = new List<TIGenericTechTemplate>();
		List<TIProjectTemplate> list2 = new List<TIProjectTemplate>();
		list = (from x in this.UniqueGlobalTechUnlocks(faction)
			where !x.ShouldHide(faction)
			select x).ToList<TITechTemplate>().ConvertAll<TIGenericTechTemplate>((TITechTemplate x) => x);
		if (benefitsContext == TechBenefitsContext.JustCompleted)
		{
			list2 = (from x in this.UniqueProjectUnlocks(faction)
				where !x.ShouldHide(faction)
				select x).ToList<TIProjectTemplate>();
		}
		else
		{
			list.AddRange(from x in this.UniqueProjectUnlocks(faction)
				where !x.ShouldHide(faction)
				select x);
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (list.Count > 0)
		{
			if (benefitsContext == TechBenefitsContext.JustCompleted)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.CompletionUnlockedTech"));
			}
			else
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.CompletionWillUnlock"));
			}
			foreach (TIGenericTechTemplate tigenericTechTemplate in list)
			{
				if (tigenericTechTemplate.isProject())
				{
					stringBuilder.Append(TemplateManager.global.projectsInlineSpritePath).Append(tigenericTechTemplate.displayName).Append(TIGenericTechTemplate.GetUnlockChanceString(tigenericTechTemplate.ref_project, faction))
						.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
				else
				{
					stringBuilder.Append(tigenericTechTemplate.displayName).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2).AppendLine();
		}
		if (list2.Count > 0)
		{
			if (list.Count > 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine(Loc.T("UI.Science.CompletionUnlockedProject"));
			foreach (TIProjectTemplate tiprojectTemplate in list2)
			{
				stringBuilder.Append(TemplateManager.global.projectsInlineSpritePath).Append(tiprojectTemplate.displayName).Append(TIGenericTechTemplate.GetUnlockChanceString(tiprojectTemplate, faction))
					.Append(Loc.T("UI.Global.SerialDividerWithSpace"));
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2).AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x00033CBC File Offset: 0x00031EBC
	public static string GetUnlockChanceString(TIProjectTemplate project, TIFactionState faction)
	{
		float num = faction.GetProjectUnlockChance(project, faction.TechContributionBonus(project)) / 100f;
		if (TIGlobalResearchState.UseHarshTechTree || faction.completedProjects.Contains(project))
		{
			return string.Empty;
		}
		if (num == 1f)
		{
			return TIUtilities.GreenLine(Loc.T("UI.Science.UnlockChance", new object[] { num.ToPercent("P0") }));
		}
		return TIUtilities.RedLine(Loc.T("UI.Science.UnlockChance", new object[] { num.ToPercent("P0") }));
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x00033D48 File Offset: 0x00031F48
	public List<TITechTemplate> UniqueGlobalTechUnlocks(TIFactionState faction)
	{
		List<TITechTemplate> list = new List<TITechTemplate>();
		if (this.isGlobalTech())
		{
			List<TITechTemplate> list2 = new List<TITechTemplate>(TIGlobalResearchState.UnlockedTechs);
			List<TITechTemplate> list3 = new List<TITechTemplate>(TIGlobalResearchState.UnlockedTechs);
			List<TITechTemplate> list4 = new List<TITechTemplate>(TIGlobalResearchState.FinishedTechs());
			list4.Remove(this.ref_tech);
			foreach (TITechTemplate titechTemplate in list3)
			{
				if (titechTemplate.TechPrereqsSatisfied(list4))
				{
					list2.Remove(titechTemplate);
				}
			}
			list.AddRange(list2);
		}
		return list;
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00033DE8 File Offset: 0x00031FE8
	public List<TIProjectTemplate> UniqueProjectUnlocks(TIFactionState faction)
	{
		List<TIProjectTemplate> list = new List<TIProjectTemplate>(faction.completedProjects);
		List<TITechTemplate> list2 = new List<TITechTemplate>(TIGlobalResearchState.FinishedTechs());
		if (this.isProject() && !list.Contains(this.ref_project))
		{
			list.Add(this.ref_project);
		}
		else if (this.isGlobalTech() && !list2.Contains(this.ref_tech))
		{
			list2.Add(this.ref_tech);
		}
		List<TIProjectTemplate> list3 = new List<TIProjectTemplate>();
		foreach (TIProjectTemplate tiprojectTemplate in TemplateManager.IterateByClass<TIProjectTemplate>(true))
		{
			if ((tiprojectTemplate.TechPrereqs.Contains(this) || tiprojectTemplate.AltTechPrereq0 == this || tiprojectTemplate.AltTechPrereq1 == this) && tiprojectTemplate.TechPrereqsSatisfied(list2, list) && tiprojectTemplate.FactionPrereqsSatisfied(faction) && tiprojectTemplate.MilestoneReqsSatisfied(faction) && tiprojectTemplate.ObjectivePrereqsSatisfied(faction) && tiprojectTemplate.UniquenessReqsSatisfied())
			{
				list3.Add(tiprojectTemplate);
			}
		}
		return list3;
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00033EF0 File Offset: 0x000320F0
	public List<TIGenericTechTemplate> CompletionWillUnlock(TIFactionState faction)
	{
		List<TIGenericTechTemplate> list = new List<TIGenericTechTemplate>();
		List<TIProjectTemplate> list2 = new List<TIProjectTemplate>(faction.completedProjects);
		List<TITechTemplate> list3 = new List<TITechTemplate>(TIGlobalResearchState.FinishedTechs());
		if (this.isProject() && !list2.Contains(this.ref_project))
		{
			list2.Add(this.ref_project);
		}
		else if (this.isGlobalTech() && !list3.Contains(this.ref_tech))
		{
			list3.Add(this.ref_tech);
		}
		if (this.isGlobalTech())
		{
			List<TITechTemplate> list4 = new List<TITechTemplate>();
			List<TITechTemplate> list5 = new List<TITechTemplate>(TIGlobalResearchState.UnlockedTechs);
			foreach (TITechTemplate titechTemplate in TemplateManager.IterateByClass<TITechTemplate>(true))
			{
				if (!list5.Contains(titechTemplate) && !list3.Contains(titechTemplate) && (this.TechPrereqs.Contains(titechTemplate) || this.AltTechPrereq0 == titechTemplate || this.AltTechPrereq1 == titechTemplate) && titechTemplate.TechPrereqsSatisfied(list3))
				{
					list4.Add(titechTemplate);
				}
			}
			list.AddRange(list4);
		}
		List<TIProjectTemplate> list6 = new List<TIProjectTemplate>(faction.TriggeredProjects);
		List<TIProjectTemplate> list7 = new List<TIProjectTemplate>();
		foreach (TIProjectTemplate tiprojectTemplate in TemplateManager.IterateByClass<TIProjectTemplate>(true))
		{
			if (!list6.Contains(tiprojectTemplate) && !list2.Contains(tiprojectTemplate) && (this.TechPrereqs.Contains(tiprojectTemplate) || this.AltTechPrereq0 == tiprojectTemplate || this.AltTechPrereq1 == tiprojectTemplate) && tiprojectTemplate.TechPrereqsSatisfied(list3, list2) && tiprojectTemplate.FactionPrereqsSatisfied(faction) && tiprojectTemplate.MilestoneReqsSatisfied(faction) && tiprojectTemplate.ObjectivePrereqsSatisfied(faction) && tiprojectTemplate.UniquenessReqsSatisfied())
			{
				list7.Add(tiprojectTemplate);
			}
		}
		list.AddRange(list7);
		return list;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x000340DC File Offset: 0x000322DC
	public IEnumerable<TIGenericTechTemplate> GetAllDescendents()
	{
		new List<TIProjectTemplate>();
		return from x in TemplateManager.IterateByClass<TIProjectTemplate>(true)
			where x.TechPrereqs.Contains(this) || x.AltTechPrereq0 == this || x.AltTechPrereq1 == this
			select x;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000340FC File Offset: 0x000322FC
	public IEnumerable<TIGenericTechTemplate> GetAllLockedDescendents(TIFactionState faction)
	{
		return from x in this.GetAllDescendents()
			where !faction.completedProjects.Contains(x)
			select x;
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x00034130 File Offset: 0x00032330
	public bool LeadsToObjectiveProjects(TIFactionState faction)
	{
		TIGenericTechTemplate.<>c__DisplayClass56_0 CS$<>8__locals1 = new TIGenericTechTemplate.<>c__DisplayClass56_0();
		CS$<>8__locals1.faction = faction;
		IEnumerable<TIGenericTechTemplate> enumerable;
		bool flag = CS$<>8__locals1.<LeadsToObjectiveProjects>g__DirectlyGivesObjectiveProjects|0(this, out enumerable);
		enumerable = enumerable.ToList<TIGenericTechTemplate>();
		if (!flag)
		{
			flag = enumerable.Any<TIGenericTechTemplate>(delegate(TIGenericTechTemplate x)
			{
				IEnumerable<TIGenericTechTemplate> enumerable2;
				return base.<LeadsToObjectiveProjects>g__DirectlyGivesObjectiveProjects|0(x, out enumerable2);
			});
		}
		return flag;
	}

	// Token: 0x06000A97 RID: 2711 RVA: 0x00034174 File Offset: 0x00032374
	public List<TIGenericTechTemplate> AllPrereqFor(TIFactionState filterFaction, bool filterUnknownXenoProjects)
	{
		if (this._allPrereqFor == null)
		{
			List<TIGenericTechTemplate> list = new List<TIGenericTechTemplate>();
			if (!this.isProject())
			{
				foreach (TITechTemplate titechTemplate in TemplateManager.IterateByClass<TITechTemplate>(true))
				{
					if (titechTemplate.TechPrereqs.Contains(this))
					{
						list.Add(titechTemplate);
					}
					if (titechTemplate.AltTechPrereq0 == this)
					{
						list.Add(titechTemplate);
					}
					if (titechTemplate.AltTechPrereq1 == this)
					{
						list.Add(titechTemplate);
					}
				}
			}
			foreach (TIProjectTemplate tiprojectTemplate in TemplateManager.IterateByClass<TIProjectTemplate>(true))
			{
				if (!(filterFaction != null) || (tiprojectTemplate.FactionPrereqsSatisfied(filterFaction) && (!filterUnknownXenoProjects || tiprojectTemplate.techCategory != TechCategory.Xenology || filterFaction.availableProjects.Contains(tiprojectTemplate))))
				{
					if (tiprojectTemplate.TechPrereqs.Contains(this))
					{
						list.Add(tiprojectTemplate);
					}
					if (tiprojectTemplate.AltTechPrereq0 == this)
					{
						list.Add(tiprojectTemplate);
					}
					if (tiprojectTemplate.AltTechPrereq1 == this)
					{
						list.Add(tiprojectTemplate);
					}
				}
			}
			this._allPrereqFor = list.ToList<TIGenericTechTemplate>();
		}
		return this._allPrereqFor;
	}

	// Token: 0x06000A98 RID: 2712 RVA: 0x000342BC File Offset: 0x000324BC
	public string PrereqForStr_Archive(TIFactionState faction, bool withholdDirectUnlocks)
	{
		List<TIGenericTechTemplate> list = this.CompletionWillUnlock(faction);
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		StringBuilder stringBuilder2 = new StringBuilder();
		bool flag2 = false;
		IEnumerable<TITechTemplate> enumerable = TemplateManager.IterateByClass<TITechTemplate>(true);
		Func<TITechTemplate, bool> <>9__0;
		Func<TITechTemplate, bool> func;
		if ((func = <>9__0) == null)
		{
			func = (<>9__0 = (TITechTemplate x) => !x.ShouldHide(faction));
		}
		foreach (TITechTemplate titechTemplate in enumerable.Where<TITechTemplate>(func).ToList<TITechTemplate>())
		{
			if (!withholdDirectUnlocks || !list.Contains(titechTemplate))
			{
				bool flag3 = false;
				if ((titechTemplate.AltTechPrereq0 != null || titechTemplate.AltTechPrereq1 != null) && (titechTemplate.TechPrereqs[0] == this || titechTemplate.AltTechPrereq0 == this || titechTemplate.AltTechPrereq1 == this))
				{
					if (!flag2)
					{
						stringBuilder2.AppendLine(Loc.T("UI.Science.PrereqFor.Or.Tech"));
						flag2 = true;
					}
					stringBuilder2.Append(titechTemplate.displayName).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
					flag3 = true;
				}
				if (!flag3 && titechTemplate.TechPrereqs.Contains(this))
				{
					if (!flag)
					{
						stringBuilder.AppendLine(Loc.T("UI.Science.PrereqFor.And.Tech"));
						flag = true;
					}
					stringBuilder.Append(titechTemplate.displayName).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
			}
		}
		StringBuilder stringBuilder3 = new StringBuilder();
		bool flag4 = false;
		StringBuilder stringBuilder4 = new StringBuilder();
		bool flag5 = false;
		IEnumerable<TIProjectTemplate> enumerable2 = TemplateManager.IterateByClass<TIProjectTemplate>(true);
		Func<TIProjectTemplate, bool> <>9__1;
		Func<TIProjectTemplate, bool> func2;
		if ((func2 = <>9__1) == null)
		{
			func2 = (<>9__1 = (TIProjectTemplate x) => !x.ShouldHide(faction));
		}
		foreach (TIProjectTemplate tiprojectTemplate in enumerable2.Where<TIProjectTemplate>(func2).ToList<TIProjectTemplate>())
		{
			if (!withholdDirectUnlocks || !list.Contains(tiprojectTemplate))
			{
				bool flag6 = false;
				if ((tiprojectTemplate.AltTechPrereq0 != null || tiprojectTemplate.AltTechPrereq1 != null) && (tiprojectTemplate.TechPrereqs[0] == this || tiprojectTemplate.AltTechPrereq0 == this || tiprojectTemplate.AltTechPrereq1 == this))
				{
					if (!flag5)
					{
						stringBuilder4.AppendLine(Loc.T("UI.Science.PrereqFor.Or.Project"));
						flag5 = true;
					}
					stringBuilder4.Append(tiprojectTemplate.displayName).Append(TIGenericTechTemplate.GetUnlockChanceString(tiprojectTemplate, faction)).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
					flag6 = true;
				}
				if (!flag6 && tiprojectTemplate.TechPrereqs.Contains(this))
				{
					if (!flag4)
					{
						stringBuilder3.AppendLine(Loc.T("UI.Science.PrereqFor.And.Project"));
						flag4 = true;
					}
					stringBuilder3.Append(tiprojectTemplate.displayName).Append(TIGenericTechTemplate.GetUnlockChanceString(tiprojectTemplate, faction)).Append(Loc.T("UI.Global.SerialDividerWithSpace"));
				}
			}
		}
		if (flag)
		{
			stringBuilder.Remove(stringBuilder.Length - 2, 2).AppendLine();
		}
		if (flag2)
		{
			stringBuilder.AppendLine().Append(stringBuilder2.Remove(stringBuilder2.Length - 2, 2)).AppendLine();
		}
		if (flag4)
		{
			stringBuilder.AppendLine().Append(stringBuilder3.Remove(stringBuilder3.Length - 2, 2)).AppendLine();
		}
		if (flag5)
		{
			stringBuilder.AppendLine().Append(stringBuilder4.Remove(stringBuilder4.Length - 2, 2)).AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x1700016E RID: 366
	// (get) Token: 0x06000A99 RID: 2713 RVA: 0x0003464C File Offset: 0x0003284C
	public List<TIGenericTechTemplate> TechPrereqs
	{
		get
		{
			if (this.cachedTechPrereqs == null)
			{
				List<TIGenericTechTemplate> list = new List<TIGenericTechTemplate>();
				foreach (string text in this.prereqs)
				{
					if (!string.IsNullOrEmpty(text) && !(text == "PLACEHOLDER"))
					{
						TIGenericTechTemplate tigenericTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(text, true);
						if (tigenericTechTemplate == null)
						{
							Log.Error("Bad tech prereq in json: " + text + " in template " + base.dataName, Array.Empty<object>());
						}
						else
						{
							list.Add(tigenericTechTemplate);
						}
					}
				}
				this.cachedTechPrereqs = list;
			}
			return this.cachedTechPrereqs;
		}
	}

	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000A9A RID: 2714 RVA: 0x00034700 File Offset: 0x00032900
	public TIGenericTechTemplate AltTechPrereq0
	{
		get
		{
			if (!this.cachedAltPrereq0)
			{
				if (!string.IsNullOrEmpty(this.altPrereq0))
				{
					TIGenericTechTemplate tigenericTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(this.altPrereq0, true);
					if (tigenericTechTemplate != null)
					{
						this.cachedAltTechPrereq0 = tigenericTechTemplate;
						this.cachedAltPrereq0 = true;
						return tigenericTechTemplate;
					}
					Log.Error("Bad tech prereq in json: " + this.altPrereq0 + " in template " + base.dataName, Array.Empty<object>());
				}
				this.cachedAltPrereq0 = true;
				return null;
			}
			return this.cachedAltTechPrereq0;
		}
	}

	// Token: 0x17000170 RID: 368
	// (get) Token: 0x06000A9B RID: 2715 RVA: 0x00034778 File Offset: 0x00032978
	public TIGenericTechTemplate AltTechPrereq1
	{
		get
		{
			if (!this.cachedAltPrereq1)
			{
				if (!string.IsNullOrEmpty(this.altPrereq1))
				{
					TIGenericTechTemplate tigenericTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(this.altPrereq1, true);
					if (tigenericTechTemplate != null)
					{
						this.cachedAltTechPrereq1 = tigenericTechTemplate;
						this.cachedAltPrereq1 = true;
						return tigenericTechTemplate;
					}
					Log.Error("Bad tech prereq in json: " + this.altPrereq1 + " in template " + base.dataName, Array.Empty<object>());
				}
				this.cachedAltPrereq1 = true;
				return null;
			}
			return this.cachedAltTechPrereq1;
		}
	}

	// Token: 0x06000A9C RID: 2716 RVA: 0x000347F0 File Offset: 0x000329F0
	public bool IsAnAltPrereqOf(TIGenericTechTemplate techToCheck)
	{
		return (techToCheck.AltTechPrereq0 != null && (techToCheck.AltTechPrereq0 == this || techToCheck.TechPrereqs[0] == this)) || (techToCheck.AltTechPrereq1 != null && (techToCheck.AltTechPrereq1 == this || techToCheck.TechPrereqs[1] == this));
	}

	// Token: 0x06000A9D RID: 2717
	public abstract bool IsEverAvailableToFaction(TIFactionState faction);

	// Token: 0x06000A9E RID: 2718 RVA: 0x00034840 File Offset: 0x00032A40
	protected List<TIDataTemplate> CodexUnlocks()
	{
		List<TIDataTemplate> list = new List<TIDataTemplate>();
		foreach (TICodexEntryTemplate ticodexEntryTemplate in TemplateManager.IterateByClass<TICodexEntryTemplate>(true))
		{
			if (ticodexEntryTemplate.unlockTech == base.dataName)
			{
				list.Add(ticodexEntryTemplate);
			}
		}
		foreach (TIMissionTemplate timissionTemplate in TemplateManager.IterateByClass<TIMissionTemplate>(true))
		{
			if (timissionTemplate.knowledgeProject == base.dataName)
			{
				list.Add(timissionTemplate);
			}
		}
		return list;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x000348FC File Offset: 0x00032AFC
	public bool SpaceExplorationTech()
	{
		return this.Effects.SelectMany<TIEffectTemplate, Context>((TIEffectTemplate x) => x.GetContexts()).Any<Context>((Context x) => TIFactionState.spaceRangeContexts.Contains(x));
	}

	// Token: 0x04000937 RID: 2359
	public TechCategory techCategory;

	// Token: 0x04000938 RID: 2360
	public TechRole AI_techRole;

	// Token: 0x04000939 RID: 2361
	public bool AI_criticalTech;

	// Token: 0x0400093A RID: 2362
	public List<string> prereqs = new List<string>();

	// Token: 0x0400093B RID: 2363
	public string altPrereq0;

	// Token: 0x0400093C RID: 2364
	public string altPrereq1;

	// Token: 0x0400093D RID: 2365
	public float researchCost;

	// Token: 0x0400093E RID: 2366
	public List<string> effects = new List<string>();

	// Token: 0x0400093F RID: 2367
	public CampaignMilestone requiredMilestone;

	// Token: 0x04000940 RID: 2368
	public string iconResource;

	// Token: 0x04000941 RID: 2369
	public string completedIllustrationPath;

	// Token: 0x04000942 RID: 2370
	public string voiceoverPath;

	// Token: 0x04000943 RID: 2371
	private List<TIGenericTechTemplate> _allPrereqFor;

	// Token: 0x04000944 RID: 2372
	private List<TIGenericTechTemplate> cachedTechPrereqs;

	// Token: 0x04000945 RID: 2373
	private TIGenericTechTemplate cachedAltTechPrereq0;

	// Token: 0x04000946 RID: 2374
	private bool cachedAltPrereq0;

	// Token: 0x04000947 RID: 2375
	private TIGenericTechTemplate cachedAltTechPrereq1;

	// Token: 0x04000948 RID: 2376
	private bool cachedAltPrereq1;
}
