using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PavonisInteractive.TerraInvicta;

// Token: 0x020002DC RID: 732
public class TIObjectiveTemplate : TIDataTemplate
{
	// Token: 0x06000AD4 RID: 2772 RVA: 0x0003BC40 File Offset: 0x00039E40
	public new string displayName(TIFactionState faction)
	{
		if (this.objectiveType != ObjectiveType.General)
		{
			return Loc.T(new StringBuilder("TIObjectiveTemplate.displayName.").Append(base.localizationName).Append(".").Append(faction.templateName)
				.ToString());
		}
		return Loc.T(new StringBuilder("TIObjectiveTemplate.displayName.").Append(base.localizationName).ToString());
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0003BCAC File Offset: 0x00039EAC
	public string description(TIFactionState faction)
	{
		if (this.objectiveType != ObjectiveType.General)
		{
			return TIObjectiveTemplate.ParseObjectiveTags(faction, Loc.T(new StringBuilder("TIObjectiveTemplate.description.").Append(base.localizationName).Append(".").Append(faction.templateName)
				.ToString()));
		}
		return Loc.T(new StringBuilder("TIObjectiveTemplate.description.").Append(base.localizationName).ToString());
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0003BD1C File Offset: 0x00039F1C
	public string solution(TIFactionState faction)
	{
		if (this.objectiveType != ObjectiveType.General)
		{
			return TIUtilities.GreenLine(TIObjectiveTemplate.ParseObjectiveTags(faction, Loc.T(new StringBuilder("TIObjectiveTemplate.solution.").Append(base.localizationName).Append(".").Append(faction.templateName)
				.ToString())));
		}
		return TIUtilities.GreenLine(Loc.T(new StringBuilder("TIObjectiveTemplate.solution.").Append(base.localizationName).ToString()));
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0003BD98 File Offset: 0x00039F98
	public string solutionUnresolved(TIFactionState faction)
	{
		if (this.objectiveType != ObjectiveType.General)
		{
			return TIUtilities.YellowLine(TIObjectiveTemplate.ParseObjectiveTags(faction, Loc.T(new StringBuilder("TIObjectiveTemplate.solution.").Append(base.localizationName).Append(".").Append(faction.templateName)
				.ToString())));
		}
		return TIUtilities.YellowLine(Loc.T(new StringBuilder("TIObjectiveTemplate.solution.").Append(base.localizationName).ToString()));
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x0003BE12 File Offset: 0x0003A012
	public string resolution(TIFactionState faction)
	{
		return TIObjectiveTemplate.ParseObjectiveTags(faction, Loc.T(new StringBuilder("TIObjectiveTemplate.resolution.").Append(base.localizationName).Append(".").Append(faction.templateName)
			.ToString()));
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000AD9 RID: 2777 RVA: 0x0003BE4E File Offset: 0x0003A04E
	public bool grantsResources
	{
		get
		{
			return this.resourcesGranted.Any<ResourceValue>((ResourceValue x) => x.resource != FactionResource.None && x.value > 0f);
		}
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0003BE7C File Offset: 0x0003A07C
	public string fullDescription(TIFactionState faction, bool unresolvedSolution = false)
	{
		StringBuilder stringBuilder = new StringBuilder(this.description(faction)).AppendLine().AppendLine().Append(unresolvedSolution ? this.solutionUnresolved(faction) : this.solution(faction));
		if (this.targetTechTemplate != null || this.targetProjectTemplate != null || this.targetHabModuleTemplate != null)
		{
			stringBuilder.AppendLine().AppendLine().AppendLine(this.NeededTechsAndProjects(faction))
				.AppendLine();
		}
		if (this.objectiveType == ObjectiveType.Victory)
		{
			TIVictoryTemplate victoryTemplate = faction.victoryTemplate;
			List<TIVictoryTemplate.VictoryCondition> list = victoryTemplate.victoryConditions.Where<TIVictoryTemplate.VictoryCondition>((TIVictoryTemplate.VictoryCondition x) => x.conditionType > TIVictoryTemplate.VictoryConditionType.none).ToList<TIVictoryTemplate.VictoryCondition>();
			if (list.Any<TIVictoryTemplate.VictoryCondition>())
			{
				stringBuilder.AppendLine().AppendLine();
				stringBuilder.AppendLine(Loc.T((list.Count == 1) ? "UI.Objectives.VictoryConditions1" : "UI.Objectives.VictoryConditionsMult")).AppendLine().AppendLine();
				foreach (TIVictoryTemplate.VictoryCondition victoryCondition in list)
				{
					List<TISpaceAssetState> list2;
					stringBuilder.AppendLine(victoryTemplate.SingleVictoryConditionDescriptionWithScore(faction, victoryCondition, out list2));
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0003BFC4 File Offset: 0x0003A1C4
	public string fullParentMilestoneDescription(TIFactionState faction)
	{
		StringBuilder stringBuilder = new StringBuilder(this.description(faction));
		int num = 1;
		foreach (TIObjectiveTemplate tiobjectiveTemplate in this.GetChildMilestones(faction, this))
		{
			stringBuilder.AppendLine().AppendLine().Append(num)
				.Append(") ");
			if (faction.milestones.Contains(faction.GetMileStoneFromObjective(tiobjectiveTemplate)))
			{
				stringBuilder.Append(tiobjectiveTemplate.solution(faction));
			}
			else
			{
				stringBuilder.Append(tiobjectiveTemplate.solutionUnresolved(faction));
			}
			num++;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0003C07C File Offset: 0x0003A27C
	public string milestoneDescription(TIFactionState faction)
	{
		if (this.objectiveType != ObjectiveType.General)
		{
			return TIObjectiveTemplate.ParseObjectiveTags(faction, Loc.T(new StringBuilder("TIObjectiveTemplate.solution.").Append(base.localizationName).Append(".").Append(faction.templateName)
				.ToString()));
		}
		return Loc.T(new StringBuilder("TIObjectiveTemplate.solution.").Append(base.localizationName).ToString());
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0003C0EC File Offset: 0x0003A2EC
	public string VictorySummary(TIFactionState faction)
	{
		TIVictoryTemplate victoryTemplate = faction.victoryTemplate;
		List<TIVictoryTemplate.VictoryCondition> list = victoryTemplate.victoryConditions.Where<TIVictoryTemplate.VictoryCondition>((TIVictoryTemplate.VictoryCondition x) => x.conditionType > TIVictoryTemplate.VictoryConditionType.none).ToList<TIVictoryTemplate.VictoryCondition>();
		if (list.Any<TIVictoryTemplate.VictoryCondition>())
		{
			StringBuilder stringBuilder = new StringBuilder(Loc.T((list.Count == 1) ? "UI.Objectives.VictoryConditions1" : "UI.Objectives.VictoryConditionsMult")).AppendLine().AppendLine();
			foreach (TIVictoryTemplate.VictoryCondition victoryCondition in list)
			{
				List<TISpaceAssetState> list2;
				stringBuilder.AppendLine(victoryTemplate.SingleVictoryConditionDescriptionWithScore(faction, victoryCondition, out list2));
			}
			return stringBuilder.ToString();
		}
		return Loc.T("TIVictoryCondition.None");
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0003C1C0 File Offset: 0x0003A3C0
	public static string ParseObjectiveTags(TIFactionState faction, string inputString)
	{
		StringBuilder stringBuilder = new StringBuilder(inputString);
		stringBuilder.Replace("{factionName}", faction.displayName).Replace("{factionNameCapitalized}", faction.displayNameCapitalized).Replace("{leaderAddress}", faction.leaderAddress)
			.Replace("{winnerOrg}", faction.winningOrgTemplate.displayName)
			.Replace("{winnerOrgMission}", faction.winningOrgTemplate.missionsGranted[0].displayName);
		return stringBuilder.ToString();
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0003C240 File Offset: 0x0003A440
	public string NeededTechsAndProjects(TIFactionState faction)
	{
		StringBuilder stringBuilder = new StringBuilder();
		TIGenericTechTemplate tigenericTechTemplate = this.targetProjectTemplate;
		if (tigenericTechTemplate == null)
		{
			tigenericTechTemplate = this.targetTechTemplate;
		}
		if (tigenericTechTemplate == null && this.targetHabModuleTemplate != null)
		{
			tigenericTechTemplate = this.targetHabModuleTemplate.RequiredProject;
		}
		if (tigenericTechTemplate != null)
		{
			int num = 0;
			foreach (TIGenericTechTemplate tigenericTechTemplate2 in tigenericTechTemplate.TechPrereqs)
			{
				TIGenericTechTemplate tigenericTechTemplate3 = tigenericTechTemplate2;
				bool flag = false;
				TIGenericTechTemplate tigenericTechTemplate4 = null;
				if (num == 0 && tigenericTechTemplate.AltTechPrereq0 != null)
				{
					tigenericTechTemplate4 = tigenericTechTemplate.AltTechPrereq0;
				}
				else if (num == 1 && tigenericTechTemplate.AltTechPrereq1 != null)
				{
					tigenericTechTemplate4 = tigenericTechTemplate.AltTechPrereq1;
				}
				if (tigenericTechTemplate4 != null)
				{
					bool flag2 = tigenericTechTemplate2.IsEverAvailableToFaction(faction);
					bool flag3 = tigenericTechTemplate4.IsEverAvailableToFaction(faction);
					if (flag2 && flag3)
					{
						flag = true;
					}
					else if (!flag2 && !flag3)
					{
						Log.Error("Neither objective prereq is ever available to " + faction.templateName, Array.Empty<object>());
					}
					else if (!flag2)
					{
						tigenericTechTemplate3 = tigenericTechTemplate4;
					}
				}
				if (num == 0)
				{
					stringBuilder.AppendLine(Loc.T("UI.Objectives.TechProjectPrereqs"));
					if (this.targetHabModuleTemplate != null)
					{
						if (faction.completedProjects.Contains(this.targetHabModuleTemplate.RequiredProject))
						{
							stringBuilder.AppendLine(TIUtilities.GreenLine(this.targetHabModuleTemplate.RequiredProject.displayName));
						}
						else
						{
							stringBuilder.AppendLine(TIUtilities.RedLine(this.targetHabModuleTemplate.RequiredProject.displayName));
						}
					}
				}
				if ((tigenericTechTemplate3.isGlobalTech() && TIGlobalResearchState.FinishedTechs().Contains(tigenericTechTemplate3.ref_tech)) || (tigenericTechTemplate3.isProject() && faction.completedProjects.Contains(tigenericTechTemplate3.ref_project)))
				{
					stringBuilder.Append(TIUtilities.GreenLine(tigenericTechTemplate3.displayName));
				}
				else
				{
					stringBuilder.Append(TIUtilities.RedLine(tigenericTechTemplate3.displayName));
				}
				if (num == 0 && flag)
				{
					if ((tigenericTechTemplate.AltTechPrereq0.isGlobalTech() && TIGlobalResearchState.FinishedTechs().Contains(tigenericTechTemplate.AltTechPrereq0.ref_tech)) || (tigenericTechTemplate.AltTechPrereq0.isProject() && faction.completedProjects.Contains(tigenericTechTemplate.AltTechPrereq0.ref_project)))
					{
						stringBuilder.AppendLine(Loc.T("UI.Objectives.TechProjectPrereq_or", new object[] { TIUtilities.GreenLine(tigenericTechTemplate.AltTechPrereq0.displayName) }));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Objectives.TechProjectPrereq_or", new object[] { TIUtilities.RedLine(tigenericTechTemplate.AltTechPrereq0.displayName) }));
					}
				}
				else if (num == 1 && flag)
				{
					if ((tigenericTechTemplate.AltTechPrereq1.isGlobalTech() && TIGlobalResearchState.FinishedTechs().Contains(tigenericTechTemplate.AltTechPrereq1.ref_tech)) || (tigenericTechTemplate.AltTechPrereq1.isProject() && faction.completedProjects.Contains(tigenericTechTemplate.AltTechPrereq1.ref_project)))
					{
						stringBuilder.AppendLine(Loc.T("UI.Objectives.TechProjectPrereq_or", new object[] { TIUtilities.GreenLine(tigenericTechTemplate.AltTechPrereq1.displayName) }));
					}
					else
					{
						stringBuilder.AppendLine(Loc.T("UI.Objectives.TechProjectPrereq_or", new object[] { TIUtilities.RedLine(tigenericTechTemplate.AltTechPrereq1.displayName) }));
					}
				}
				else
				{
					stringBuilder.AppendLine();
				}
				num++;
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000AE0 RID: 2784 RVA: 0x0003C5A0 File Offset: 0x0003A7A0
	public List<TIFactionState> factions
	{
		get
		{
			if (this._factions == null)
			{
				this._factions = new List<TIFactionState>();
				foreach (string text in this.factionDataNames)
				{
					if (!string.IsNullOrEmpty(text))
					{
						TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(text, false);
						this._factions.Add(tifactionState);
					}
				}
			}
			return this._factions;
		}
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x0003C624 File Offset: 0x0003A824
	public List<TIObjectiveTemplate> unlockingObjectives
	{
		get
		{
			List<TIObjectiveTemplate> list = new List<TIObjectiveTemplate>();
			foreach (string text in this.unlockingObjectiveNames)
			{
				if (!string.IsNullOrEmpty(text))
				{
					TIObjectiveTemplate tiobjectiveTemplate = TemplateManager.Find<TIObjectiveTemplate>(text, false);
					if (tiobjectiveTemplate == null)
					{
						Log.Error(base.dataName + " has bad unlocking objective template: " + text, Array.Empty<object>());
					}
					list.Add(tiobjectiveTemplate);
				}
			}
			return list;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000AE2 RID: 2786 RVA: 0x0003C6AC File Offset: 0x0003A8AC
	public TIProjectTemplate targetProjectTemplate
	{
		get
		{
			return TemplateManager.Find<TIProjectTemplate>(this.targetProjectTemplateName, false);
		}
	}

	// Token: 0x17000177 RID: 375
	// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x0003C6BA File Offset: 0x0003A8BA
	public TITechTemplate targetTechTemplate
	{
		get
		{
			return TemplateManager.Find<TITechTemplate>(this.targetTechTemplateName, false);
		}
	}

	// Token: 0x17000178 RID: 376
	// (get) Token: 0x06000AE4 RID: 2788 RVA: 0x0003C6C8 File Offset: 0x0003A8C8
	public TIMissionTemplate targetMissionTemplate
	{
		get
		{
			return TemplateManager.Find<TIMissionTemplate>(this.targetMissionTemplateName, false);
		}
	}

	// Token: 0x17000179 RID: 377
	// (get) Token: 0x06000AE5 RID: 2789 RVA: 0x0003C6D6 File Offset: 0x0003A8D6
	public TIHabModuleTemplate targetHabModuleTemplate
	{
		get
		{
			return TemplateManager.Find<TIHabModuleTemplate>(this.targetHabModuleName, false);
		}
	}

	// Token: 0x1700017A RID: 378
	// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x0003C6E4 File Offset: 0x0003A8E4
	public TIGameState targetHabLocationState
	{
		get
		{
			return GameStateManager.FindByTemplate<TIGameState>(this.targetHabLocation, true);
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0003C6F2 File Offset: 0x0003A8F2
	public ObjectiveStatus GetObjectiveStatus(TIFactionState faction)
	{
		return faction.GetObjectiveStatus(this);
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003C6FB File Offset: 0x0003A8FB
	public bool IsObjectiveComplete(TIFactionState faction)
	{
		return faction.IsObjectiveComplete(this);
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003C704 File Offset: 0x0003A904
	public bool passedUnlockingObjectives(TIFactionState faction)
	{
		if (this.unlockingObjectivesConjunction == Conjunction.And)
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate in this.unlockingObjectives)
			{
				if (faction.GetObjectiveStatus(tiobjectiveTemplate) != ObjectiveStatus.Completed)
				{
					return false;
				}
			}
			return true;
		}
		if (this.unlockingObjectivesConjunction == Conjunction.Or)
		{
			foreach (TIObjectiveTemplate tiobjectiveTemplate2 in this.unlockingObjectives)
			{
				if (faction.GetObjectiveStatus(tiobjectiveTemplate2) == ObjectiveStatus.Completed)
				{
					return true;
				}
			}
			return false;
		}
		return true;
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x0003C7C0 File Offset: 0x0003A9C0
	public bool ValidObjectiveTarget(TIGameState candidate, TIFactionState faction)
	{
		switch (this.targetMissionTarget)
		{
		case ObjectiveMissionTargetType.UFOCrashdown:
			return candidate.isRegionUFOCrashdown;
		case ObjectiveMissionTargetType.Abductions:
			return candidate.isRegionAlienActivity && candidate.ref_regionAlienActivity.MissionDetectedByFaction(faction, TIFactionState.abductionsMission.dataName);
		case ObjectiveMissionTargetType.Xenoforming:
			return candidate.isRegionXenoformingState && candidate.ref_xenoforming.xenoformingLevel > 0f;
		case ObjectiveMissionTargetType.EnthrallMission:
			return candidate.isRegionAlienActivity && (candidate.ref_regionAlienActivity.MissionDetectedByFaction(faction, TIFactionState.enthrallElitesMission.dataName) || candidate.ref_regionAlienActivity.MissionDetectedByFaction(faction, TIFactionState.enthrallOrgMission.dataName) || candidate.ref_regionAlienActivity.MissionDetectedByFaction(faction, TIFactionState.enthrallNonAlignedElitesMission.dataName) || candidate.ref_regionAlienActivity.MissionDetectedByFaction(faction, TIFactionState.enthrallPublicMission.dataName));
		case ObjectiveMissionTargetType.AlienTech:
			return (candidate.isRegionAlienFacility && candidate.ref_alienFacility.built) || candidate.isRegionLandedUFO;
		case ObjectiveMissionTargetType.LandedUFO:
			return candidate.isRegionLandedUFO;
		case ObjectiveMissionTargetType.HumanCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isHuman;
		case ObjectiveMissionTargetType.HumanExtremistCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isHuman && candidate.ref_councilor.faction.extremist;
		case ObjectiveMissionTargetType.HumanAntiAlienCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isHuman && candidate.ref_councilor.faction.antiAlien;
		case ObjectiveMissionTargetType.HumanProAlienCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isHuman && candidate.ref_councilor.faction.proAlien;
		case ObjectiveMissionTargetType.HumanAppeaserCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isHuman && candidate.ref_councilor.faction.isAlienAppeaser;
		case ObjectiveMissionTargetType.HumanProxyCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isHuman && candidate.ref_councilor.faction.IsAlienProxy;
		case ObjectiveMissionTargetType.HydraCouncilor:
			return candidate.isCouncilorState && candidate.ref_councilor.isAlien;
		case ObjectiveMissionTargetType.AlienHQ:
			return candidate.isHabModuleState && candidate.ref_hab == GameStateManager.AlienFaction().primaryHab;
		case ObjectiveMissionTargetType.NewYorkRegion:
			return candidate.isRegionState && candidate.ref_region.mapRegionTemplateName == "map_NewYork";
		case ObjectiveMissionTargetType.EscapeLaunchSite:
			return candidate.isHabModuleState && candidate.ref_hab.ActiveSpecialAbilities(faction).Contains(HabModuleSpecialRule.InterstellarLaunchModule);
		case ObjectiveMissionTargetType.AppeaseSentinel:
			return candidate.isHabModuleState && candidate.ref_hab.ActiveSpecialAbilities(faction).Contains(HabModuleSpecialRule.SentinelModule);
		default:
			switch (this.targetMilestone)
			{
			case CampaignMilestone.AccessHydraCorpus:
			case CampaignMilestone.AccessLiveHydra:
				return candidate.isCouncilorState && candidate.ref_councilor.isAlien;
			case CampaignMilestone.AccessAlienTech:
				return candidate.isRegionLandedUFO || candidate.isRegionAlienFacility || (candidate.isHabState && candidate.ref_faction.IsAlienFaction);
			case CampaignMilestone.AccessAlienShip:
				return candidate.isRegionLandedUFO || (candidate.isSpaceShipState && candidate.ref_ship.faction.IsAlienFaction);
			}
			Log.Error(string.Concat(new string[]
			{
				faction.displayName,
				" ",
				base.dataName,
				" ",
				this.targetMissionTarget.ToString(),
				" missing"
			}), Array.Empty<object>());
			return false;
		}
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x0003CB48 File Offset: 0x0003AD48
	public List<TIGameState> ValidObjectiveTargets(List<TIGameState> candidateList, TIFactionState faction)
	{
		return candidateList.Where<TIGameState>((TIGameState x) => this.ValidObjectiveTarget(x, faction)).ToList<TIGameState>();
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0003CB80 File Offset: 0x0003AD80
	public static bool IsTutorialMilestone(CampaignMilestone milestone)
	{
		return milestone >= CampaignMilestone.TutorialSelectCouncilor && milestone < CampaignMilestone.UITutorial_END;
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0003CB91 File Offset: 0x0003AD91
	public static bool MilestoneRequiresDeadHydraAccess(CampaignMilestone milestone)
	{
		return milestone == CampaignMilestone.AccessGriffinCorpus || milestone == CampaignMilestone.AccessLiveGriffin || milestone == CampaignMilestone.AccessSalamanderCorpus || milestone == CampaignMilestone.AccessLiveSalamander || milestone == CampaignMilestone.AccessWarDogCorpus;
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0003CBAB File Offset: 0x0003ADAB
	public static bool MilestoneRequiresLiveAlienAccess(CampaignMilestone milestone)
	{
		return milestone == CampaignMilestone.AccessLiveHydra || milestone == CampaignMilestone.AccessLiveSalamander || milestone == CampaignMilestone.AccessLiveGriffin;
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x0003CBBC File Offset: 0x0003ADBC
	public static bool SuppressMilestoneReporting(CampaignMilestone milestone)
	{
		if (milestone == CampaignMilestone.AlienInvasionPlanDiscovered || milestone == CampaignMilestone.AlienAwareness_Public || milestone == CampaignMilestone.AlienOvertAggression)
		{
			return !TIGlobalValuesState.IsQuietAlienCampaign();
		}
		return TIObjectiveTemplate.IsTutorialMilestone(milestone) || milestone == CampaignMilestone.AlienDiplomacy || milestone == CampaignMilestone.DetectXenoforming || milestone == CampaignMilestone.AlienArmyDestroyed || milestone == CampaignMilestone.TargetedByTerrorMission || milestone == CampaignMilestone.AccessAlienLanguage || milestone == CampaignMilestone.AlienCouncilorSighted || milestone == CampaignMilestone.AlienArmySighted || milestone == CampaignMilestone.AlienSpaceshipSighted || milestone == CampaignMilestone.AlienWarshipSighted || milestone == CampaignMilestone.AlienInvasionShipSighted || milestone == CampaignMilestone.AlienHabSighted || milestone == CampaignMilestone.AlienNationWasFounded || milestone == CampaignMilestone.EquippedNuclearWeaponInSpace || milestone == CampaignMilestone.UsedNuclearWeaponInSpace || milestone == CampaignMilestone.AssaultedAlienFacility || milestone == CampaignMilestone.DestoyedAlienFacility;
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x0003CC3C File Offset: 0x0003AE3C
	public static bool HasChildMilestone(TIFactionState faction, TIObjectiveTemplate objective)
	{
		foreach (TIObjectiveTemplate tiobjectiveTemplate in faction.GetObjectives())
		{
			if (tiobjectiveTemplate.targetMilestone == objective.targetMilestone && tiobjectiveTemplate.isChildObjective)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0003CCA8 File Offset: 0x0003AEA8
	public List<TIObjectiveTemplate> GetChildMilestones(TIFactionState faction, TIObjectiveTemplate objective)
	{
		List<TIObjectiveTemplate> list = new List<TIObjectiveTemplate>();
		foreach (TIObjectiveTemplate tiobjectiveTemplate in faction.GetObjectives())
		{
			if (tiobjectiveTemplate.targetMilestone == objective.targetMilestone)
			{
				list.Add(tiobjectiveTemplate);
			}
		}
		return list;
	}

	// Token: 0x04000DB9 RID: 3513
	public ObjectiveType objectiveType;

	// Token: 0x04000DBA RID: 3514
	public List<string> factionDataNames;

	// Token: 0x04000DBB RID: 3515
	public bool starter;

	// Token: 0x04000DBC RID: 3516
	public Conjunction unlockingObjectivesConjunction;

	// Token: 0x04000DBD RID: 3517
	public List<string> unlockingObjectiveNames;

	// Token: 0x04000DBE RID: 3518
	public string targetMissionTemplateName;

	// Token: 0x04000DBF RID: 3519
	public ObjectiveMissionTargetType targetMissionTarget;

	// Token: 0x04000DC0 RID: 3520
	public string targetProjectTemplateName;

	// Token: 0x04000DC1 RID: 3521
	public string targetTechTemplateName;

	// Token: 0x04000DC2 RID: 3522
	public CampaignMilestone targetMilestone;

	// Token: 0x04000DC3 RID: 3523
	public string targetHabModuleName;

	// Token: 0x04000DC4 RID: 3524
	public string targetHabLocation;

	// Token: 0x04000DC5 RID: 3525
	public int targetCount = 1;

	// Token: 0x04000DC6 RID: 3526
	public ResourceValue[] resourcesGranted;

	// Token: 0x04000DC7 RID: 3527
	public int AIValuesIndex;

	// Token: 0x04000DC8 RID: 3528
	public bool setsWinConditionForFaction;

	// Token: 0x04000DC9 RID: 3529
	public string assignedIllustrationResource;

	// Token: 0x04000DCA RID: 3530
	public string completedIllustrationResource;

	// Token: 0x04000DCB RID: 3531
	public string completedVoicePathAppease;

	// Token: 0x04000DCC RID: 3532
	public string completedVoicePathCooperate;

	// Token: 0x04000DCD RID: 3533
	public string completedVoicePathDestroy;

	// Token: 0x04000DCE RID: 3534
	public string completedVoicePathEscape;

	// Token: 0x04000DCF RID: 3535
	public string completedVoicePathExploit;

	// Token: 0x04000DD0 RID: 3536
	public string completedVoicePathResist;

	// Token: 0x04000DD1 RID: 3537
	public string completedVoicePathSubmit;

	// Token: 0x04000DD2 RID: 3538
	public string completedVoicePathAlien;

	// Token: 0x04000DD3 RID: 3539
	public string completedVoicePathMod1;

	// Token: 0x04000DD4 RID: 3540
	public string completedVoicePathMod2;

	// Token: 0x04000DD5 RID: 3541
	public string completedVoicePathMod3;

	// Token: 0x04000DD6 RID: 3542
	public string completedVoicePathMod4;

	// Token: 0x04000DD7 RID: 3543
	public string completedVoicePathMod5;

	// Token: 0x04000DD8 RID: 3544
	public string completedVoicePathMod6;

	// Token: 0x04000DD9 RID: 3545
	public string completedVoicePathMod7;

	// Token: 0x04000DDA RID: 3546
	public string completedVoicePathMod8;

	// Token: 0x04000DDB RID: 3547
	public bool isChildObjective;

	// Token: 0x04000DDC RID: 3548
	private List<TIFactionState> _factions;
}
