using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000400 RID: 1024
public class TIMetaTemplate : TIDataTemplate
{
	// Token: 0x060014FE RID: 5374 RVA: 0x00066487 File Offset: 0x00064687
	public TIMetaTemplate()
	{
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x0006649A File Offset: 0x0006469A
	public TIMetaTemplate(string templateName)
		: base(templateName)
	{
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x000664B0 File Offset: 0x000646B0
	public static void LoadMetaTemplates(IEnumerable<string> names)
	{
		Log.Time("<color=#00cc00>LoadTime:</color> Load MetaTemplates", delegate
		{
			TemplateManager.ResolveScenarioTemplates(GameControl.control.scenarioTemplate);
			foreach (string text in names)
			{
				TIMetaTemplate.LoadMetaTemplate(text);
			}
			TIMetaTemplate.PostLoadMetaTemplates();
		}, true, true);
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x000664E4 File Offset: 0x000646E4
	public static List<TIDataTemplate> GetTemplatesOfTypeFromMeta(string metaTemplateName, Type t)
	{
		List<TIDataTemplate> list = new List<TIDataTemplate>();
		foreach (string text in TemplateManager.Find<TIMetaTemplate>(metaTemplateName, false).templateNames)
		{
			TIDataTemplate tidataTemplate = TemplateManager.Find<TIDataTemplate>(text, true);
			if (tidataTemplate == null)
			{
				Log.Error("Can't find template " + text + " in metaTemplateName", Array.Empty<object>());
			}
			else if (tidataTemplate.GetType() == typeof(TIMetaTemplate))
			{
				list.AddRange(TIMetaTemplate.GetTemplatesOfTypeFromMeta(text, t));
			}
			else
			{
				TIDataTemplate tidataTemplate2 = TemplateManager.Find(text, t, true);
				if (tidataTemplate2 != null)
				{
					list.Add(tidataTemplate2);
				}
			}
		}
		return list;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x000665A0 File Offset: 0x000647A0
	private static void LoadMetaTemplate(string metaTemplateName)
	{
		TIMetaTemplate timetaTemplate = TemplateManager.Find<TIMetaTemplate>(metaTemplateName, false);
		if (Error.IsNull<TIMetaTemplate>(timetaTemplate, "Could not find MetaTemplate {0}", new object[] { metaTemplateName }))
		{
			return;
		}
		Type type = timetaTemplate.templateType;
		foreach (string text in timetaTemplate.templateNames)
		{
			if (type == typeof(TIMetaTemplate))
			{
				TIMetaTemplate.LoadMetaTemplate(text);
			}
			else
			{
				TIDataTemplate tidataTemplate = TemplateManager.Find(text, type, true);
				if (!Error.IsNull<TIDataTemplate>(tidataTemplate, "Could not find template for {0} {1}", new object[] { type, text }) && (GameControl.control.skirmishMode || (((!(type == typeof(TIFactionTemplate)) && !(type == typeof(TIPlayerTemplate))) || TIMetaTemplate.ValidateFaction(tidataTemplate, type)) && (!(type == typeof(TISpaceFleetTemplate)) || TIMetaTemplate.ValidateFleet(tidataTemplate, type)) && (!(type == typeof(TIHabTemplate)) || TIMetaTemplate.ValidateHab(tidataTemplate, type)))))
				{
					TIGameState tigameState = tidataTemplate.CreateGameState();
					tigameState.exists = true;
					if (!Error.IsNull<TIGameState>(tigameState, "Failed to create GameState for {0} {1}", new object[] { type, text }))
					{
						tigameState.InitWithTemplate(tidataTemplate);
					}
				}
			}
		}
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x00066714 File Offset: 0x00064914
	public static void PostLoadMetaTemplates()
	{
		List<TIGameState> list = new List<TIGameState>();
		foreach (TIGameState tigameState in GameStateManager.IterateByClass<TIGameState>(true))
		{
			list.Add(tigameState);
		}
		for (int i = 0; i < list.Count; i++)
		{
			list[i].PostGameStateCreateInit_OnCreationOnly_1();
		}
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x00066784 File Offset: 0x00064984
	private static bool ValidateFaction(TIDataTemplate template, Type templateType)
	{
		return (!(templateType == typeof(TIFactionTemplate)) || GameControl.control.scenarioCustomizationsStartup.selectedFactionsForScenario.Contains(template.dataName)) && (!(templateType == typeof(TIPlayerTemplate)) || GameControl.control.scenarioCustomizationsStartup.selectedFactionsForScenario.Contains((template as TIPlayerTemplate).council));
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x000667F8 File Offset: 0x000649F8
	private static bool ValidateFleet(TIDataTemplate template, Type templateType)
	{
		return (!(template.dataName == "alienInvasionFleet72020") || GameControl.control.scenarioCustomizationsStartup.addAlienAssaultCarrierFleet) && (GameStateManager.Time().template.distributeFactionlessHabsAndFleets || GameControl.control.scenarioCustomizationsStartup.selectedFactionsForScenario.Contains((template as TISpaceFleetTemplate).factionName));
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x00066860 File Offset: 0x00064A60
	private static bool ValidateHab(TIDataTemplate template, Type templateType)
	{
		return GameStateManager.Time().template.distributeFactionlessHabsAndFleets || GameControl.control.scenarioCustomizationsStartup.selectedFactionsForScenario.Contains((template as TIHabTemplate).sectors[0].faction);
	}

	// Token: 0x04001290 RID: 4752
	public Type templateType;

	// Token: 0x04001291 RID: 4753
	public List<string> templateNames;

	// Token: 0x04001292 RID: 4754
	public List<string> requiredDLC = new List<string>();

	// Token: 0x04001293 RID: 4755
	public int listPriority;

	// Token: 0x04001294 RID: 4756
	public int optionPriority;

	// Token: 0x04001295 RID: 4757
	public bool isNewCampaignOption;

	// Token: 0x04001296 RID: 4758
	public bool tutorialAllowed;

	// Token: 0x04001297 RID: 4759
	public string newCampaignOptionCategory;

	// Token: 0x04001298 RID: 4760
	public List<string> templatesToUseDefaultLocalization;

	// Token: 0x04001299 RID: 4761
	public string scenarioLocalizationPostfix;
}
