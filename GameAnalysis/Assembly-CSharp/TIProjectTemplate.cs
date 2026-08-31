using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x02000358 RID: 856
public class TIProjectTemplate : TIGenericTechTemplate
{
	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000EE5 RID: 3813 RVA: 0x000495E0 File Offset: 0x000477E0
	public TechCategory TechCategory
	{
		get
		{
			return this.techCategory;
		}
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x000495E8 File Offset: 0x000477E8
	public override bool isGlobalTech()
	{
		return false;
	}

	// Token: 0x06000EE7 RID: 3815 RVA: 0x000495EB File Offset: 0x000477EB
	public override bool isProject()
	{
		return true;
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06000EE8 RID: 3816 RVA: 0x000495EE File Offset: 0x000477EE
	public override TIProjectTemplate ref_project
	{
		get
		{
			return this;
		}
	}

	// Token: 0x06000EE9 RID: 3817 RVA: 0x000495F4 File Offset: 0x000477F4
	public bool SomeoneHasDoneIt()
	{
		return GameStateManager.AllHumanFactions().Any<TIFactionState>((TIFactionState x) => x.completedProjects.Contains(this)) || base.dataName == TemplateManager.global.alienMasterProject || base.dataName == TemplateManager.global.alienAdvancedMasterProject;
	}

	// Token: 0x06000EEA RID: 3818 RVA: 0x00049647 File Offset: 0x00047847
	public override float GetResearchCost(TIFactionState faction)
	{
		return (this.repeatable ? (this.researchCost * (float)(1 + faction.completedProjects.Count<TIProjectTemplate>((TIProjectTemplate x) => x == this))) : this.researchCost) / TIGlobalValuesState.GetResearchSpeedModifier();
	}

	// Token: 0x1700018C RID: 396
	// (get) Token: 0x06000EEB RID: 3819 RVA: 0x00049680 File Offset: 0x00047880
	public override string summary
	{
		get
		{
			string text = Loc.T(new StringBuilder(base.GetType().Name).Append(".summary.").Append(base.localizationName).ToString());
			if (text != null)
			{
				if (!(text == "<habmodule>"))
				{
					if (!(text == "<shipmodule>"))
					{
						return text;
					}
					List<TIShipPartTemplate> shipPartUnlocks = this.ShipPartUnlocks;
					if (shipPartUnlocks.Count > 0)
					{
						return shipPartUnlocks[0].description;
					}
				}
				else
				{
					List<TIHabModuleTemplate> list = this.HabModuleUnlocks();
					if (list.Count == 1)
					{
						return list[0].description;
					}
				}
				return text;
			}
			return text;
		}
	}

	// Token: 0x06000EEC RID: 3820 RVA: 0x0004971C File Offset: 0x0004791C
	public string AllUnlocksDetails(bool includeHeader, bool truncateDescriptions = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<TIHabModuleTemplate> list = this.HabModuleUnlocks();
		List<TIShipPartTemplate> shipPartUnlocks = this.ShipPartUnlocks;
		if (list.Count > 0 || shipPartUnlocks.Count > 0)
		{
			if (includeHeader)
			{
				stringBuilder.AppendLine(TIUtilities.GreenLine(Loc.T("UI.Science.UnlockDetails")));
			}
			foreach (TIHabModuleTemplate tihabModuleTemplate in list)
			{
				stringBuilder.AppendLine(tihabModuleTemplate.displayName);
				stringBuilder.AppendLine(Loc.T("UI.Habs.CostFromSpace", new object[] { tihabModuleTemplate.BuildMaterials(0f, GameStateManager.Luna(), GameStateManager.Luna(), GameControl.control.activePlayer, 1f).ToResourcesCost(1f).GetString("Relevant", false, true, false, 2, false, false, null, false, FactionResource.None) }));
				stringBuilder.AppendLine(tihabModuleTemplate.benefitsAndCostsDescription(GameControl.control.activePlayer, null, false));
				stringBuilder.AppendLine(tihabModuleTemplate.extendedDescription);
				stringBuilder.AppendLine();
			}
			int num = 1;
			foreach (TIShipPartTemplate tishipPartTemplate in shipPartUnlocks)
			{
				if ((!truncateDescriptions || !tishipPartTemplate.isDrive || tishipPartTemplate.ref_drive.thrusters <= 1) && (!tishipPartTemplate.isWeapon || !tishipPartTemplate.ref_weapon.fighterOnlyWeapon || TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron)))
				{
					stringBuilder.AppendLine(tishipPartTemplate.displayName);
					if (truncateDescriptions && tishipPartTemplate.isWeapon && num > 1)
					{
						stringBuilder.AppendLine(tishipPartTemplate.ref_weapon.GetTruncatedDescriptionData(null, null, true, ShipModuleSlotType.None));
					}
					else
					{
						stringBuilder.AppendLine(tishipPartTemplate.GetFullDescription(null, null, true, ShipModuleSlotType.None, false));
						if (tishipPartTemplate.isWeapon || tishipPartTemplate is TIShipHullTemplate || tishipPartTemplate.isUtilityModule)
						{
							stringBuilder.AppendLine();
						}
					}
					if (tishipPartTemplate.exoFighterPart && TIGlobalValuesState.CanAnyHumanNationUsePriority(PriorityType.Military_BuildSTOSquadron))
					{
						stringBuilder.AppendLine(Loc.T("UI.Science.UnlockFighterParts"));
					}
					num++;
				}
			}
		}
		return stringBuilder.ToString().Trim();
	}

	// Token: 0x06000EED RID: 3821 RVA: 0x0004996C File Offset: 0x00047B6C
	protected override string filteredDescription(TechBenefitsContext context)
	{
		if (!(this.description == "<skip/>") && !(this.description == base.descriptionPath) && context != TechBenefitsContext.Prospective)
		{
			return this.description;
		}
		return string.Empty;
	}

	// Token: 0x06000EEE RID: 3822 RVA: 0x000499A2 File Offset: 0x00047BA2
	public override string GetCompletedIllustrationPath()
	{
		if (!string.IsNullOrEmpty(this.completedIllustrationPath))
		{
			return this.completedIllustrationPath;
		}
		return TemplateManager.global.illus_projectCompletePath[this.techCategory];
	}

	// Token: 0x1700018D RID: 397
	// (get) Token: 0x06000EEF RID: 3823 RVA: 0x000499D0 File Offset: 0x00047BD0
	public List<TIBilateralTemplate> associatedBilaterals
	{
		get
		{
			if (this._associatedBilatals == null)
			{
				this._associatedBilatals = new List<TIBilateralTemplate>();
				foreach (TIBilateralTemplate tibilateralTemplate in TemplateManager.IterateByClass<TIBilateralTemplate>(true))
				{
					if (tibilateralTemplate.projectUnlockName == base.dataName && tibilateralTemplate.BilateralIsInScenario())
					{
						this._associatedBilatals.Add(tibilateralTemplate);
					}
				}
			}
			return this._associatedBilatals;
		}
	}

	// Token: 0x1700018E RID: 398
	// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x00049A58 File Offset: 0x00047C58
	public List<TIBilateralTemplate> associatedClaims
	{
		get
		{
			if (this._associatedClaims == null)
			{
				this._associatedClaims = this.associatedBilaterals.Where<TIBilateralTemplate>((TIBilateralTemplate x) => x.relationType == BilateralRelationType.Claim).ToList<TIBilateralTemplate>();
			}
			return this._associatedClaims;
		}
	}

	// Token: 0x1700018F RID: 399
	// (get) Token: 0x06000EF1 RID: 3825 RVA: 0x00049AA8 File Offset: 0x00047CA8
	public TIOrgTemplate OrgGranted
	{
		get
		{
			if (!string.IsNullOrEmpty(this.orgGranted))
			{
				TIOrgTemplate tiorgTemplate = TemplateManager.Find<TIOrgTemplate>(this.orgGranted, false);
				if (tiorgTemplate != null)
				{
					return tiorgTemplate;
				}
				Log.Error("Bad org template name " + this.orgGranted + " in " + base.dataName, Array.Empty<object>());
			}
			return null;
		}
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x00049AFC File Offset: 0x00047CFC
	public TIObjectiveTemplate FulfillsObjective(TIFactionState faction, bool ignoreLocked)
	{
		foreach (TIObjectiveTemplate tiobjectiveTemplate in TemplateManager.IterateByClass<TIObjectiveTemplate>(true))
		{
			if (tiobjectiveTemplate.factionDataNames.Contains(faction.templateName) && (ignoreLocked || faction.GetObjectiveStatus(tiobjectiveTemplate) != ObjectiveStatus.Locked))
			{
				if (tiobjectiveTemplate.targetProjectTemplate == this && (tiobjectiveTemplate.objectiveType != ObjectiveType.Tutorial || TIGlobalValuesState.isTutorialActive))
				{
					return tiobjectiveTemplate;
				}
				if (tiobjectiveTemplate.targetProjectTemplate != null)
				{
					foreach (TIGenericTechTemplate tigenericTechTemplate in tiobjectiveTemplate.targetProjectTemplate.TechPrereqs)
					{
						if (tigenericTechTemplate.isProject() && tigenericTechTemplate == this && !faction.completedProjects.Contains(tigenericTechTemplate.ref_project))
						{
							return tiobjectiveTemplate;
						}
					}
				}
				TIHabModuleTemplate targetHabModuleTemplate = tiobjectiveTemplate.targetHabModuleTemplate;
				if (targetHabModuleTemplate != null && targetHabModuleTemplate.objectiveModule && targetHabModuleTemplate.requiredProjectName == base.dataName)
				{
					return tiobjectiveTemplate;
				}
			}
		}
		return null;
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x00049C28 File Offset: 0x00047E28
	public bool IsVictoryRelated(TIFactionState faction)
	{
		TIObjectiveTemplate objective = this.FulfillsObjective(faction, false);
		return objective != null && (objective.objectiveType == ObjectiveType.Victory || faction.GetObjectivesByType(ObjectiveType.Victory).Any<TIObjectiveTemplate>((TIObjectiveTemplate x) => x.unlockingObjectiveNames.Contains(objective.dataName)));
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x00049C80 File Offset: 0x00047E80
	public override string WarningsDescription(TIFactionState faction, TechBenefitsContext context)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (this.oneTimeGlobally && context != TechBenefitsContext.JustCompleted)
		{
			stringBuilder.AppendLine(TIUtilities.RedLine(Loc.T("UI.Science.UniqueProject")));
		}
		else if (this.repeatable)
		{
			float num = this.researchCost * (float)(1 + faction.completedProjects.Count<TIProjectTemplate>((TIProjectTemplate x) => x == this));
			stringBuilder.AppendLine(TIUtilities.GreenLine(Loc.T("UI.Science.Repeating", new object[]
			{
				(this.researchCost / TIGlobalValuesState.GetResearchSpeedModifier()).ToString("N0"),
				(num / TIGlobalValuesState.GetResearchSpeedModifier()).ToString("N0"),
				TemplateManager.global.researchInlineSpritePath
			})));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x00049D48 File Offset: 0x00047F48
	public override bool ShouldHide(TIFactionState faction)
	{
		return !faction.completedProjects.Contains(this) && (!this.FactionPrereqsSatisfied(faction) || ((this.techCategory == TechCategory.Xenology || this.HasUncompletedXenologyInChain(faction)) && !faction.availableProjects.Contains(this) && !faction.completedProjects.Contains(this)) || !this.ObjectivePrereqsSatisfied(faction) || (!this.MilestoneReqsSatisfied(faction) || (this.HasUncompletedMilestoneInChain(faction) && !faction.availableProjects.Contains(this))) || (TIGlobalResearchState.UseHarshTechTree && !this.TechPrereqsSatisfied(TIGlobalResearchState.FinishedTechs(), faction.completedProjects)));
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x00049DEC File Offset: 0x00047FEC
	public bool HasUncompletedXenologyInChain(TIFactionState faction)
	{
		if (base.TechPrereqs.Count > 0)
		{
			foreach (TIGenericTechTemplate tigenericTechTemplate in base.TechPrereqs)
			{
				if (tigenericTechTemplate.techCategory == TechCategory.Xenology && !faction.completedProjects.Contains(tigenericTechTemplate) && !faction.availableProjects.Contains(tigenericTechTemplate))
				{
					if (base.AltTechPrereq0 == null && base.AltTechPrereq1 == null)
					{
						return true;
					}
					if (!faction.completedProjects.Contains(base.AltTechPrereq0) && !faction.availableProjects.Contains(base.AltTechPrereq0))
					{
						return true;
					}
					if (!faction.completedProjects.Contains(base.AltTechPrereq1) && !faction.availableProjects.Contains(base.AltTechPrereq1))
					{
						return true;
					}
				}
				if (tigenericTechTemplate.TechPrereqs.Count > 0)
				{
					foreach (TIGenericTechTemplate tigenericTechTemplate2 in tigenericTechTemplate.TechPrereqs)
					{
						if (tigenericTechTemplate2.ref_project != null && tigenericTechTemplate2.ref_project.HasUncompletedXenologyInChain(faction))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x00049F68 File Offset: 0x00048168
	public bool HasUncompletedMilestoneInChain(TIFactionState faction)
	{
		if (base.TechPrereqs.Count > 0)
		{
			foreach (TIGenericTechTemplate tigenericTechTemplate in base.TechPrereqs)
			{
				if (tigenericTechTemplate.requiredMilestone != CampaignMilestone.None && !faction.MilestoneCompleted(tigenericTechTemplate.requiredMilestone) && !faction.completedProjects.Contains(tigenericTechTemplate) && !faction.availableProjects.Contains(tigenericTechTemplate))
				{
					if (base.AltTechPrereq0 == null && base.AltTechPrereq1 == null)
					{
						return true;
					}
					if (!faction.completedProjects.Contains(base.AltTechPrereq0) && !faction.availableProjects.Contains(base.AltTechPrereq0))
					{
						return true;
					}
					if (!faction.completedProjects.Contains(base.AltTechPrereq1) && !faction.availableProjects.Contains(base.AltTechPrereq1))
					{
						return true;
					}
				}
				if (tigenericTechTemplate.TechPrereqs.Count > 0 && tigenericTechTemplate.isProject() && tigenericTechTemplate.ref_project.HasUncompletedMilestoneInChain(faction))
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x0004A09C File Offset: 0x0004829C
	public override string BenefitsDescription(TIFactionState faction, TechBenefitsContext benefitsContext, TIOrgState newOrg = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TIObjectiveTemplate tiobjectiveTemplate = this.FulfillsObjective(faction, false);
		if (tiobjectiveTemplate != null)
		{
			stringBuilder.AppendLine(TIUtilities.HighlightLine(Loc.T("UI.Science.FulfillsObjective", new object[] { tiobjectiveTemplate.displayName(faction) })));
		}
		foreach (TIEffectTemplate tieffectTemplate in base.Effects)
		{
			stringBuilder.AppendLine(tieffectTemplate.description(faction, null)).AppendLine();
		}
		if (newOrg != null && benefitsContext == TechBenefitsContext.JustCompleted)
		{
			stringBuilder.AppendLine(Loc.T("UI.Notifications.ProjectComplete.OrgGained", new object[] { newOrg.displayNameWithArticle })).AppendLine();
		}
		else if (this.OrgGranted != null)
		{
			stringBuilder.AppendLine(Loc.T("UI.Science.Spinoff")).AppendLine();
		}
		if (this.resourcesGranted.Any<ResourceValue>((ResourceValue x) => x.resource != FactionResource.None && x.value > 0f))
		{
			stringBuilder.AppendLine(Loc.T("UI.Science.GrantsResources", new object[] { TIUtilities.BuildResourceValueString(this.resourcesGranted.ToArray()) })).AppendLine();
		}
		List<TIHabModuleTemplate> list = this.HabModuleUnlocks();
		if (list.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksHabModules"));
			for (int i = 0; i < list.Count; i++)
			{
				stringBuilder.Append(list[i].displayName);
			}
			stringBuilder.AppendLine().AppendLine();
		}
		List<TIShipPartTemplate> shipPartUnlocks = this.ShipPartUnlocks;
		if (shipPartUnlocks.Count > 0)
		{
			stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksShipComponents"));
			foreach (TIShipPartTemplate tishipPartTemplate in shipPartUnlocks)
			{
				stringBuilder.AppendLine(tishipPartTemplate.displayName);
			}
			stringBuilder.AppendLine();
			if (benefitsContext == TechBenefitsContext.JustCompleted)
			{
				List<TIShipWeaponTemplate> list2 = new List<TIShipWeaponTemplate>();
				foreach (TIShipPartTemplate tishipPartTemplate2 in shipPartUnlocks)
				{
					TIShipWeaponTemplate tishipWeaponTemplate = tishipPartTemplate2 as TIShipWeaponTemplate;
					if (tishipWeaponTemplate != null)
					{
						list2.Add(tishipWeaponTemplate);
					}
				}
				foreach (TIShipWeaponTemplate tishipWeaponTemplate2 in list2)
				{
					for (int j = 1; j <= 3; j++)
					{
						if (faction.GetBestHabWeapon(false, j, tishipWeaponTemplate2.weaponClass, null, list2) == tishipWeaponTemplate2.dataName)
						{
							stringBuilder.AppendLine(Loc.T("UI.Science.StationWeapon", new object[]
							{
								tishipWeaponTemplate2.displayName,
								j.ToString()
							}));
						}
					}
				}
			}
			stringBuilder.AppendLine();
		}
		List<TITraitTemplate> list3 = this.CyberneticUnlocks();
		if (list3.Count > 0)
		{
			stringBuilder.AppendLine().AppendLine(Loc.T("UI.Science.UnlocksCouncilorUpgrades"));
			foreach (TITraitTemplate titraitTemplate in list3)
			{
				stringBuilder.AppendLine(titraitTemplate.displayName);
			}
			stringBuilder.AppendLine();
		}
		if (this.oneTimeGlobally)
		{
			bool flag = false;
			Dictionary<TINationState, List<TIGameState>> dictionary = new Dictionary<TINationState, List<TIGameState>>();
			Dictionary<TINationState, List<TIGameState>> dictionary2 = new Dictionary<TINationState, List<TIGameState>>();
			Dictionary<TINationState, TIRegionState> dictionary3 = new Dictionary<TINationState, TIRegionState>();
			foreach (TIBilateralTemplate tibilateralTemplate in this.associatedBilaterals)
			{
				flag = true;
				if (tibilateralTemplate.relationType == BilateralRelationType.Claim)
				{
					if (!dictionary.ContainsKey(tibilateralTemplate.nationState1))
					{
						dictionary.Add(tibilateralTemplate.nationState1, new List<TIGameState>());
					}
					dictionary[tibilateralTemplate.nationState1].Add(tibilateralTemplate.regionState1);
					if (tibilateralTemplate.capitalClaim)
					{
						dictionary3.Add(tibilateralTemplate.nationState1, tibilateralTemplate.regionState1);
					}
					if (tibilateralTemplate.hostileClaim)
					{
						if (!dictionary2.ContainsKey(tibilateralTemplate.nationState1))
						{
							dictionary2.Add(tibilateralTemplate.nationState1, new List<TIGameState>());
						}
						dictionary2[tibilateralTemplate.nationState1].Add(tibilateralTemplate.regionState1);
					}
				}
				else if (tibilateralTemplate.relationType == BilateralRelationType.PhysicalAdjacency)
				{
					if (tibilateralTemplate.friendlyOnly)
					{
						stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksFriendlyAdjacency", new object[]
						{
							tibilateralTemplate.regionState1.displayName,
							tibilateralTemplate.regionState2.displayName
						}));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksAllAdjacency", new object[]
						{
							tibilateralTemplate.regionState1.displayName,
							tibilateralTemplate.regionState2.displayName
						}));
					}
				}
			}
			if (dictionary.Count > 0)
			{
				foreach (TINationState tinationState in dictionary.Keys)
				{
					List<string> list4 = new List<string>();
					foreach (TIGameState tigameState in dictionary[tinationState])
					{
						if (dictionary2.ContainsKey(tinationState) && dictionary2[tinationState].Contains(tigameState))
						{
							list4.Add(new StringBuilder(TemplateManager.global.unrestInlineSpritePath).Append(TIUtilities.RedLine(tigameState.displayName)).ToString());
						}
						else
						{
							list4.Add(tigameState.displayName);
						}
					}
					if (!tinationState.extant && dictionary3.ContainsKey(tinationState))
					{
						stringBuilder.Append(Loc.T("UI.Science.UnlocksClaim", new object[]
						{
							tinationState.displayNameWithArticleCapitalized,
							TIUtilities.ConstructTextList(list4, false, false)
						}));
						stringBuilder.AppendLine(Loc.T("UI.Science.CapitalClaim", new object[]
						{
							tinationState.displayName,
							dictionary3[tinationState].displayName
						}));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksClaim", new object[]
						{
							tinationState.displayNameWithArticleCapitalized,
							TIUtilities.ConstructTextList(list4, false, false)
						}));
					}
				}
			}
			if (flag)
			{
				stringBuilder.AppendLine();
			}
		}
		if (benefitsContext == TechBenefitsContext.JustCompleted || benefitsContext == TechBenefitsContext.Archive)
		{
			List<TIDataTemplate> list5 = base.CodexUnlocks();
			if (list5.Count > 0)
			{
				stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksCodexEntries"));
				foreach (TIDataTemplate tidataTemplate in list5)
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
							stringBuilder.AppendLine(Loc.T("UI.Science.UnlocksCodexMission", new object[]
							{
								Loc.T("UI.Codex.codex_missionList_alien.Title"),
								timissionTemplate.displayName
							}));
						}
					}
				}
				stringBuilder.AppendLine();
			}
		}
		switch (benefitsContext)
		{
		case TechBenefitsContext.Prospective:
		{
			string text = base.UnlockableTechString(faction, benefitsContext);
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.AppendLine(text).AppendLine();
			}
			break;
		}
		case TechBenefitsContext.JustCompleted:
		{
			string text2 = base.UnlockableTechString(faction, benefitsContext);
			if (!string.IsNullOrEmpty(text2))
			{
				stringBuilder.AppendLine(text2);
			}
			break;
		}
		case TechBenefitsContext.Archive:
		{
			string text3 = base.PrereqForStr_Archive(faction, false);
			if (!string.IsNullOrEmpty(text3))
			{
				stringBuilder.AppendLine(text3);
			}
			break;
		}
		}
		return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0004A904 File Offset: 0x00048B04
	public List<TIHabModuleTemplate> HabModuleUnlocks()
	{
		if (this._habModuleUnlocks == null)
		{
			this._habModuleUnlocks = new List<TIHabModuleTemplate>();
			foreach (TIHabModuleTemplate tihabModuleTemplate in TemplateManager.IterateByClass<TIHabModuleTemplate>(true))
			{
				if (tihabModuleTemplate.RequiredProject == this)
				{
					this._habModuleUnlocks.Add(tihabModuleTemplate);
				}
			}
		}
		return this._habModuleUnlocks.ToList<TIHabModuleTemplate>();
	}

	// Token: 0x17000190 RID: 400
	// (get) Token: 0x06000EFA RID: 3834 RVA: 0x0004A980 File Offset: 0x00048B80
	public List<TIShipPartTemplate> ShipPartUnlocks
	{
		get
		{
			if (this._shipPartUnlocks == null)
			{
				this._shipPartUnlocks = new List<TIShipPartTemplate>();
				foreach (TIShipPartTemplate tishipPartTemplate in TemplateManager.IterateByClass<TIShipPartTemplate>(true))
				{
					if (tishipPartTemplate != null && tishipPartTemplate.requiredProject == this)
					{
						this._shipPartUnlocks.AddUnique(tishipPartTemplate);
					}
				}
			}
			return this._shipPartUnlocks.ToList<TIShipPartTemplate>();
		}
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x0004AA00 File Offset: 0x00048C00
	public List<TIShipPartTemplate> ChildProjectShipPartUnlocks(TIFactionState faction)
	{
		if (this._childShipPartUnlocks == null)
		{
			this._childShipPartUnlocks = new List<TIShipPartTemplate>();
			TemplateManager.IterateByClass<TIShipPartTemplate>(true);
			foreach (TIProjectTemplate tiprojectTemplate in from x in base.AllPrereqFor(faction, true)
				select x.ref_project)
			{
				this._childShipPartUnlocks.AddRangeUnique<TIShipPartTemplate>(tiprojectTemplate.ShipPartUnlocks);
				this._childShipPartUnlocks.AddRangeUnique<TIShipPartTemplate>(tiprojectTemplate.ChildProjectShipPartUnlocks(faction));
			}
		}
		return this._childShipPartUnlocks.ToList<TIShipPartTemplate>();
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x0004AAB8 File Offset: 0x00048CB8
	public List<TITraitTemplate> CyberneticUnlocks()
	{
		List<TITraitTemplate> list = new List<TITraitTemplate>();
		foreach (TITraitTemplate titraitTemplate in TemplateManager.IterateByClass<TITraitTemplate>(true))
		{
			if (titraitTemplate.IsMatchingProject(this))
			{
				list.Add(titraitTemplate);
			}
		}
		return list;
	}

	// Token: 0x17000191 RID: 401
	// (get) Token: 0x06000EFD RID: 3837 RVA: 0x0004AB18 File Offset: 0x00048D18
	private TIObjectiveTemplate requiredObjective
	{
		get
		{
			if (!string.IsNullOrEmpty(this.requiredObjectiveName))
			{
				TIObjectiveTemplate tiobjectiveTemplate = TemplateManager.Find<TIObjectiveTemplate>(this.requiredObjectiveName, false);
				if (tiobjectiveTemplate == null)
				{
					Log.Error("Bad " + this.requiredObjectiveName + "in Project json", Array.Empty<object>());
				}
				return tiobjectiveTemplate;
			}
			return null;
		}
	}

	// Token: 0x17000192 RID: 402
	// (get) Token: 0x06000EFE RID: 3838 RVA: 0x0004AB57 File Offset: 0x00048D57
	private TIObjectiveTemplate altRequiredObjective
	{
		get
		{
			if (!string.IsNullOrEmpty(this.altRequiredObjectiveName))
			{
				TIObjectiveTemplate tiobjectiveTemplate = TemplateManager.Find<TIObjectiveTemplate>(this.altRequiredObjectiveName, false);
				if (tiobjectiveTemplate == null)
				{
					Log.Error("Bad " + this.altRequiredObjectiveName + "in Project json", Array.Empty<object>());
				}
				return tiobjectiveTemplate;
			}
			return null;
		}
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0004AB98 File Offset: 0x00048D98
	public bool ObjectivePrereqsSatisfied(TIFactionState faction)
	{
		if (this.requiredObjective == null)
		{
			return true;
		}
		List<TIObjectiveTemplate> objectives = faction.GetObjectives();
		return (objectives.Contains(this.requiredObjective) && faction.GetObjectiveStatus(this.requiredObjective) == ObjectiveStatus.Completed) || (this.altRequiredObjective != null && objectives.Contains(this.altRequiredObjective) && faction.GetObjectiveStatus(this.altRequiredObjective) == ObjectiveStatus.Completed);
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x0004AC00 File Offset: 0x00048E00
	public bool FactionPrereqsSatisfied(TIFactionState faction)
	{
		List<string> list = this.factionPrereq.Where<string>((string x) => !string.IsNullOrEmpty(x)).ToList<string>();
		if (list.Count == 0)
		{
			return true;
		}
		using (List<string>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (GameStateManager.FindByTemplate<TIFactionState>(enumerator.Current, false) == faction)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x0004AC98 File Offset: 0x00048E98
	public override bool IsEverAvailableToFaction(TIFactionState faction)
	{
		List<string> list = this.factionPrereq;
		if (list != null && list.Count > 0 && !this.factionPrereq.Contains(faction.templateName))
		{
			return false;
		}
		if (base.TechPrereqs.Count > 0)
		{
			bool flag = base.TechPrereqs[0].IsEverAvailableToFaction(faction);
			if (!flag && base.AltTechPrereq0 != null)
			{
				flag = base.AltTechPrereq0.IsEverAvailableToFaction(faction);
			}
			if (!flag)
			{
				return false;
			}
			if (base.TechPrereqs.Count > 1)
			{
				bool flag2 = base.TechPrereqs[1].IsEverAvailableToFaction(faction);
				if (!flag2 && base.AltTechPrereq1 != null)
				{
					flag2 = base.AltTechPrereq1.IsEverAvailableToFaction(faction);
				}
				if (!flag2)
				{
					return false;
				}
			}
			for (int i = 2; i < base.TechPrereqs.Count; i++)
			{
				if (!base.TechPrereqs[i].IsEverAvailableToFaction(faction))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x0004AD80 File Offset: 0x00048F80
	public bool TechPrereqsSatisfied(List<TITechTemplate> finishedTechs, List<TIProjectTemplate> finishedProjects)
	{
		List<TIGenericTechTemplate> techPrereqs = base.TechPrereqs;
		for (int i = 0; i < techPrereqs.Count; i++)
		{
			if (!finishedTechs.Contains(techPrereqs[i].ref_tech) && !finishedProjects.Contains(techPrereqs[i].ref_project) && !TIGlobalResearchState.globalResearch.finishedOneTimeOnlyProjects.Contains(techPrereqs[i].ref_project))
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
				if (tigenericTechTemplate == null || (!finishedTechs.Contains(tigenericTechTemplate) && !finishedProjects.Contains(tigenericTechTemplate) && !TIGlobalResearchState.globalResearch.finishedOneTimeOnlyProjects.Contains(techPrereqs[i].ref_project)))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x17000193 RID: 403
	// (get) Token: 0x06000F03 RID: 3843 RVA: 0x0004AE3E File Offset: 0x0004903E
	public TINationState requiredNationState
	{
		get
		{
			if (string.IsNullOrEmpty(this.requiresNation))
			{
				return null;
			}
			return GameStateManager.AllNations().FirstOrDefault<TINationState>((TINationState x) => x.template.referenceName == this.requiresNation);
		}
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x0004AE65 File Offset: 0x00049065
	public bool MilestoneReqsSatisfied(TIFactionState faction)
	{
		return this.requiredMilestone == CampaignMilestone.None || faction.MilestoneCompleted(this.requiredMilestone);
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x0004AE7D File Offset: 0x0004907D
	public bool UniquenessReqsSatisfied()
	{
		return !this.oneTimeGlobally || !this.SomeoneHasDoneIt();
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x0004AE94 File Offset: 0x00049094
	public bool PrereqsSatisfied(List<TITechTemplate> finishedTechs, List<TIProjectTemplate> finishedProjects, TIFactionState faction)
	{
		TINationState requiredNationState = this.requiredNationState;
		return (!(requiredNationState != null) || requiredNationState.extant) && this.UniquenessReqsSatisfied() && this.FactionPrereqsSatisfied(faction) && this.ObjectivePrereqsSatisfied(faction) && this.MilestoneReqsSatisfied(faction) && this.TechPrereqsSatisfied(finishedTechs, finishedProjects);
	}

	// Token: 0x04000EC8 RID: 3784
	public float factionAvailableChance = 100f;

	// Token: 0x04000EC9 RID: 3785
	public string factionAlways;

	// Token: 0x04000ECA RID: 3786
	public float initialUnlockChance = 100f;

	// Token: 0x04000ECB RID: 3787
	public float deltaUnlockChance;

	// Token: 0x04000ECC RID: 3788
	public float maxUnlockChance = 100f;

	// Token: 0x04000ECD RID: 3789
	public string requiredObjectiveName;

	// Token: 0x04000ECE RID: 3790
	public string altRequiredObjectiveName;

	// Token: 0x04000ECF RID: 3791
	public new CampaignMilestone requiredMilestone;

	// Token: 0x04000ED0 RID: 3792
	public string requiresNation;

	// Token: 0x04000ED1 RID: 3793
	public bool oneTimeGlobally;

	// Token: 0x04000ED2 RID: 3794
	public bool repeatable;

	// Token: 0x04000ED3 RID: 3795
	public string orgGranted;

	// Token: 0x04000ED4 RID: 3796
	public List<string> factionPrereq = new List<string>();

	// Token: 0x04000ED5 RID: 3797
	public List<ResourceValue> resourcesGranted = new List<ResourceValue>();

	// Token: 0x04000ED6 RID: 3798
	public ProjectRole AI_projectRole;

	// Token: 0x04000ED7 RID: 3799
	private List<TIBilateralTemplate> _associatedBilatals;

	// Token: 0x04000ED8 RID: 3800
	private List<TIBilateralTemplate> _associatedClaims;

	// Token: 0x04000ED9 RID: 3801
	private List<TIHabModuleTemplate> _habModuleUnlocks;

	// Token: 0x04000EDA RID: 3802
	private List<TIShipPartTemplate> _shipPartUnlocks;

	// Token: 0x04000EDB RID: 3803
	private List<TIShipPartTemplate> _childShipPartUnlocks;
}
