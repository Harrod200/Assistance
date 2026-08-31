using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x0200036D RID: 877
public class TIHabTemplate : TISpaceAssetTemplate
{
	// Token: 0x06000FD8 RID: 4056 RVA: 0x0005241C File Offset: 0x0005061C
	public void SetDisplayName(string set)
	{
		this._displayName = set;
	}

	// Token: 0x06000FD9 RID: 4057 RVA: 0x00052428 File Offset: 0x00050628
	public override TIGameState CreateGameState()
	{
		this.objectType = SpaceObjectType.Hab;
		this.modelScale = 1000f;
		TIGameState tigameState = base.CreateGameState();
		if (tigameState == null)
		{
			tigameState = GameStateManager.CreateNewGameState<TIHabState>();
		}
		return tigameState;
	}

	// Token: 0x06000FDA RID: 4058 RVA: 0x00052460 File Offset: 0x00050660
	public List<TIHabModuleTemplate> AllModuleTemplates(bool uniquesOnly)
	{
		List<TIHabModuleTemplate> list = new List<TIHabModuleTemplate>();
		SectorTemplate[] array = this.sectors;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (string text in array[i].habModuleNames)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TIHabModuleTemplate tihabModuleTemplate = TemplateManager.Find<TIHabModuleTemplate>(text, false);
					if (tihabModuleTemplate != null)
					{
						if (uniquesOnly)
						{
							list.AddUnique(tihabModuleTemplate);
						}
						else
						{
							list.Add(tihabModuleTemplate);
						}
					}
				}
			}
		}
		return list;
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06000FDB RID: 4059 RVA: 0x000524DB File Offset: 0x000506DB
	public TIHabSiteState habSiteState
	{
		get
		{
			return GameStateManager.FindByTemplate<TIHabSiteState>(this.habSite, false);
		}
	}

	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06000FDC RID: 4060 RVA: 0x000524E9 File Offset: 0x000506E9
	public TINaturalSpaceObjectState naturalSpaceObject
	{
		get
		{
			if (this.habType != HabType.Station)
			{
				return this.habSiteState.parentBody;
			}
			return base.orbit.barycenter;
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06000FDD RID: 4061 RVA: 0x0005250C File Offset: 0x0005070C
	public string simpleBenefitsString
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<TIHabModuleTemplate> list = this.AllModuleTemplates(true);
			FactionResource[] factionResources = Enums.FactionResources;
			for (int i = 0; i < factionResources.Length; i++)
			{
				FactionResource resource = factionResources[i];
				if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.MonthlyResourceIncome(resource, null, null) > 0f))
				{
					stringBuilder.Append(TIUtilities.InlineResourceStr(resource));
				}
			}
			TechCategory[] techCategories = Enums.TechCategories;
			for (int i = 0; i < techCategories.Length; i++)
			{
				TechCategory category = techCategories[i];
				if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.GetTechBonusByCategory(category) > 0f))
				{
					stringBuilder.Append(TIGenericTechTemplate.categoryInlineSprite(category));
				}
			}
			if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsResupply))
			{
				stringBuilder.Append(TemplateManager.global.habResupplyPresentInlineSpritePath);
			}
			if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.allowsShipConstruction))
			{
				stringBuilder.Append(TemplateManager.global.habShipyardPresentInlineSpritePath);
			}
			if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.moduleConstructionSpeedModifier > 1f))
			{
				stringBuilder.Append(TemplateManager.global.habModuleConstructionInlineSpritePath);
			}
			if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.spaceCombatModule))
			{
				stringBuilder.Append(TemplateManager.global.habDefenseScoreInlineSpritePath);
			}
			if (list.Any<TIHabModuleTemplate>((TIHabModuleTemplate x) => x.CombatTroops()))
			{
				stringBuilder.Append(TemplateManager.global.spaceAssaultValueInlineSpritePath);
			}
			return stringBuilder.ToString();
		}
	}

	// Token: 0x04001019 RID: 4121
	public HabType habType;

	// Token: 0x0400101A RID: 4122
	public int tier;

	// Token: 0x0400101B RID: 4123
	public bool alien;

	// Token: 0x0400101C RID: 4124
	public string habSite;

	// Token: 0x0400101D RID: 4125
	public SectorTemplate[] sectors;
}
