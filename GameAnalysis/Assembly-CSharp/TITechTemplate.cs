using System;
using System.Collections.Generic;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200035B RID: 859
public class TITechTemplate : TIGenericTechTemplate
{
	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06000F0F RID: 3855 RVA: 0x0004B04B File Offset: 0x0004924B
	public TechCategory TechCategory
	{
		get
		{
			return this.techCategory;
		}
	}

	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06000F10 RID: 3856 RVA: 0x0004B054 File Offset: 0x00049254
	protected override string description
	{
		get
		{
			string quote = this.quote;
			if (string.IsNullOrEmpty(quote))
			{
				return Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.localizationName).ToString());
			}
			return new StringBuilder(quote).AppendLine().AppendLine().AppendLine(Loc.T(new StringBuilder(base.GetType().Name).Append(".description.").Append(base.localizationName).ToString()))
				.ToString();
		}
	}

	// Token: 0x06000F11 RID: 3857 RVA: 0x0004B0E9 File Offset: 0x000492E9
	public override bool isGlobalTech()
	{
		return true;
	}

	// Token: 0x06000F12 RID: 3858 RVA: 0x0004B0EC File Offset: 0x000492EC
	public override bool isProject()
	{
		return false;
	}

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06000F13 RID: 3859 RVA: 0x0004B0EF File Offset: 0x000492EF
	public override TITechTemplate ref_tech
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000F14 RID: 3860 RVA: 0x0004B0F2 File Offset: 0x000492F2
	public override float GetResearchCost(TIFactionState faction)
	{
		return this.researchCost * (float)(this.endGameTech ? (1 + TIGlobalResearchState.globalResearch.endGameTechsCompletedByCategory[this.techCategory]) : 1) / TIGlobalValuesState.GetResearchSpeedModifier();
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06000F15 RID: 3861 RVA: 0x0004B124 File Offset: 0x00049324
	private string quote
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T(new StringBuilder("TITechTemplate.quote.").Append(base.localizationName).ToString()));
			stringBuilder.Replace("{appeaseLeader}", Loc.T("TIFactionTemplate.fullLeader.AppeaseCouncil")).Replace("{destroyLeader}", Loc.T("TIFactionTemplate.fullLeader.DestroyCouncil")).Replace("{exploitLeader}", Loc.T("TIFactionTemplate.fullLeader.ExploitCouncil"))
				.Replace("{submitLeader}", Loc.T("TIFactionTemplate.fullLeader.SubmitCouncil"))
				.Replace("{resistLeader}", Loc.T("TIFactionTemplate.fullLeader.ResistCouncil"))
				.Replace("{cooperateLeader}", Loc.T("TIFactionTemplate.fullLeader.CooperateCouncil"))
				.Replace("{escapeLeader}", Loc.T("TIFactionTemplate.fullLeader.EscapeCouncil"));
			return stringBuilder.ToString();
		}
	}

	// Token: 0x06000F16 RID: 3862 RVA: 0x0004B1E8 File Offset: 0x000493E8
	public override string GetCompletedIllustrationPath()
	{
		if (!string.IsNullOrEmpty(this.completedIllustrationPath))
		{
			return this.completedIllustrationPath;
		}
		return TemplateManager.global.illus_techCompletePath[this.techCategory];
	}

	// Token: 0x06000F17 RID: 3863 RVA: 0x0004B213 File Offset: 0x00049413
	public bool FinishedBeforeCampaignStart(int startYear)
	{
		return this.year > 0 && this.year < startYear;
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x06000F18 RID: 3864 RVA: 0x0004B22C File Offset: 0x0004942C
	public List<string> orgTypeUnlocks
	{
		get
		{
			if (this._orgDataNameUnlocks == null)
			{
				this._orgDataNameUnlocks = new List<string>();
				foreach (TIOrgTemplate tiorgTemplate in TemplateManager.IterateByClass<TIOrgTemplate>(true))
				{
					if (tiorgTemplate.requiredTechName == base.dataName)
					{
						this._orgDataNameUnlocks.Add(tiorgTemplate.dataName);
					}
				}
			}
			return this._orgDataNameUnlocks;
		}
	}

	// Token: 0x06000F19 RID: 3865 RVA: 0x0004B2B0 File Offset: 0x000494B0
	public bool TechPrereqsSatisfied(List<TITechTemplate> finishedTechs)
	{
		List<TIGenericTechTemplate> techPrereqs = base.TechPrereqs;
		for (int i = 0; i < techPrereqs.Count; i++)
		{
			if (!finishedTechs.Contains(techPrereqs[i].ref_tech))
			{
				TIGenericTechTemplate tigenericTechTemplate = null;
				if (i == 0)
				{
					tigenericTechTemplate = base.AltTechPrereq0;
				}
				else if (i == 1)
				{
					tigenericTechTemplate = base.AltTechPrereq1;
				}
				if (tigenericTechTemplate == null || !tigenericTechTemplate.isGlobalTech() || !finishedTechs.Contains(tigenericTechTemplate.ref_tech))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000F1A RID: 3866 RVA: 0x0004B31E File Offset: 0x0004951E
	public override bool IsEverAvailableToFaction(TIFactionState faction)
	{
		return true;
	}

	// Token: 0x06000F1B RID: 3867 RVA: 0x0004B324 File Offset: 0x00049524
	public override string BenefitsDescription(TIFactionState faction, TechBenefitsContext benefitsContext, TIOrgState newOrg = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		foreach (TIEffectTemplate tieffectTemplate in base.Effects)
		{
			string text = tieffectTemplate.description(faction, null);
			if (!string.IsNullOrEmpty(text))
			{
				flag = true;
				stringBuilder.AppendLine(text);
			}
		}
		if (base.Effects.Count > 0 && flag)
		{
			stringBuilder.AppendLine();
		}
		if (this.endGameTech)
		{
			stringBuilder.AppendLine(Loc.T("TITechTemplate.desc.EndGameReroll"));
		}
		if (benefitsContext == TechBenefitsContext.JustCompleted || benefitsContext == TechBenefitsContext.Archive)
		{
			if (this.orgTypeUnlocks.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksOrgs")).AppendLine();
			}
			List<TIDataTemplate> list = base.CodexUnlocks();
			if (list.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksCodexEntries"));
				foreach (TIDataTemplate tidataTemplate in list)
				{
					TICodexEntryTemplate ticodexEntryTemplate = tidataTemplate as TICodexEntryTemplate;
					if (ticodexEntryTemplate != null)
					{
						stringBuilder.AppendLine(ticodexEntryTemplate.titleText);
					}
					else
					{
						TIMissionTemplate timissionTemplate = tidataTemplate as TIMissionTemplate;
						if (timissionTemplate != null)
						{
							stringBuilder.AppendLine(timissionTemplate.displayName);
						}
					}
				}
				stringBuilder.AppendLine();
			}
		}
		switch (benefitsContext)
		{
		case TechBenefitsContext.Prospective:
			stringBuilder.AppendLine(base.UnlockableTechString(faction, benefitsContext));
			stringBuilder.AppendLine(base.PrereqForStr_Archive(faction, false));
			break;
		case TechBenefitsContext.JustCompleted:
			stringBuilder.AppendLine(base.UnlockableTechString(faction, benefitsContext));
			break;
		case TechBenefitsContext.Archive:
			stringBuilder.AppendLine(base.PrereqForStr_Archive(faction, false));
			break;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04000F31 RID: 3889
	public int year;

	// Token: 0x04000F32 RID: 3890
	public bool endGameTech;

	// Token: 0x04000F33 RID: 3891
	private List<string> _orgDataNameUnlocks;
}
