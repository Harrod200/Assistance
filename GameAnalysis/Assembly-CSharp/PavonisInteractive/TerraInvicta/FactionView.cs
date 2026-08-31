using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200071F RID: 1823
	public struct FactionView
	{
		// Token: 0x06002C28 RID: 11304 RVA: 0x000F18C1 File Offset: 0x000EFAC1
		public FactionView(TIFactionState faction, TIFactionState playerFaction)
		{
			this.faction = faction;
			this.playerFaction = playerFaction;
		}

		// Token: 0x1700061F RID: 1567
		// (get) Token: 0x06002C29 RID: 11305 RVA: 0x000F18D1 File Offset: 0x000EFAD1
		public bool showLeader
		{
			get
			{
				return this.playerFaction.GetHighestIntel(this.faction) >= TemplateManager.global.intelToSeeFactionBasicData;
			}
		}

		// Token: 0x17000620 RID: 1568
		// (get) Token: 0x06002C2A RID: 11306 RVA: 0x000F18F3 File Offset: 0x000EFAF3
		public string leader
		{
			get
			{
				if (!this.showLeader)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.faction.leaderName;
			}
		}

		// Token: 0x17000621 RID: 1569
		// (get) Token: 0x06002C2B RID: 11307 RVA: 0x000F1913 File Offset: 0x000EFB13
		public string fullLeader
		{
			get
			{
				if (!this.showLeader)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return this.faction.leaderNameWithAddress;
			}
		}

		// Token: 0x17000622 RID: 1570
		// (get) Token: 0x06002C2C RID: 11308 RVA: 0x000F1933 File Offset: 0x000EFB33
		public string goal
		{
			get
			{
				if (this.playerFaction.GetHighestIntel(this.faction) < TemplateManager.global.intelToSeeFactionBasicData)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				return Utilities.Capitalize(this.faction.goal);
			}
		}

		// Token: 0x17000623 RID: 1571
		// (get) Token: 0x06002C2D RID: 11309 RVA: 0x000F1970 File Offset: 0x000EFB70
		public List<TIOrgState> knownUnassignedOrgsPool
		{
			get
			{
				TIFactionState pf = this.playerFaction;
				if (pf.GetIntel(this.faction) >= TemplateManager.global.intelToSeeFactionUnassignedOrgs || this.faction.councilors.Any<TICouncilorState>((TICouncilorState x) => pf.GetIntel(x) >= TemplateManager.global.intelToSeeCouncilorDetails))
				{
					return new List<TIOrgState>(this.faction.unassignedOrgs);
				}
				return new List<TIOrgState>();
			}
		}

		// Token: 0x17000624 RID: 1572
		// (get) Token: 0x06002C2E RID: 11310 RVA: 0x000F19E0 File Offset: 0x000EFBE0
		public string victory
		{
			get
			{
				if (this.playerFaction.GetHighestIntel(this.faction) < TemplateManager.global.intelToSeeFactionObjectives)
				{
					return Loc.T("UI.CouncilorView.Unknown");
				}
				if (this.faction.GetObjectivesByTypeAndStatus(ObjectiveType.Victory, ObjectiveStatus.Unlocked).Count > 0)
				{
					return Loc.T(new StringBuilder("TIObjectiveTemplate.VictorySummary.").Append(this.faction.templateName).ToString());
				}
				return Loc.T("UI.CouncilView.UndefinedVictoryCondition");
			}
		}

		// Token: 0x06002C2F RID: 11311 RVA: 0x000F1A59 File Offset: 0x000EFC59
		public List<TIObjectiveTemplate> GetObjectives(ObjectiveType objectiveType, ObjectiveStatus status)
		{
			if (this.playerFaction.GetIntel(this.faction) >= TemplateManager.global.intelToSeeFactionObjectives)
			{
				return this.faction.GetObjectivesByTypeAndStatus(objectiveType, status);
			}
			return new List<TIObjectiveTemplate>();
		}

		// Token: 0x06002C30 RID: 11312 RVA: 0x000F1A8B File Offset: 0x000EFC8B
		public string GetResourceString(FactionResource resource)
		{
			if (this.playerFaction.GetIntel(this.faction) >= TemplateManager.global.intelToSeeFactionResources)
			{
				return GeneralControlsController.ResourceReportString(this.faction, resource);
			}
			return Loc.T("UI.CouncilorView.UnknownSymbol");
		}

		// Token: 0x17000625 RID: 1573
		// (get) Token: 0x06002C31 RID: 11313 RVA: 0x000F1AC1 File Offset: 0x000EFCC1
		public List<ProjectProgress> currentProjectProgress
		{
			get
			{
				if (this.playerFaction.GetHighestIntel(this.faction) < TemplateManager.global.intelToSeeFactionProjects)
				{
					return new List<ProjectProgress>();
				}
				return this.faction.currentProjectProgress;
			}
		}

		// Token: 0x17000626 RID: 1574
		// (get) Token: 0x06002C32 RID: 11314 RVA: 0x000F1AF1 File Offset: 0x000EFCF1
		public List<TIProjectTemplate> completedProjectsDistinct
		{
			get
			{
				if (this.playerFaction.GetHighestIntel(this.faction) < TemplateManager.global.intelToSeeFactionProjects)
				{
					return new List<TIProjectTemplate>();
				}
				return this.faction.completedProjectsDistinct;
			}
		}

		// Token: 0x17000627 RID: 1575
		// (get) Token: 0x06002C33 RID: 11315 RVA: 0x000F1B21 File Offset: 0x000EFD21
		public List<TIProjectTemplate> availableProjects
		{
			get
			{
				if (this.playerFaction.GetHighestIntel(this.faction) < TemplateManager.global.intelToSeeFactionProjects)
				{
					return new List<TIProjectTemplate>();
				}
				return this.faction.availableProjects;
			}
		}

		// Token: 0x04002174 RID: 8564
		private readonly TIFactionState playerFaction;

		// Token: 0x04002175 RID: 8565
		private readonly TIFactionState faction;
	}
}
