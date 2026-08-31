using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using PavonisInteractive.TerraInvicta.Actions;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200077D RID: 1917
	public class TIGlobalResearchState : TIGameState, IGameStateVisualizer
	{
		// Token: 0x17000A90 RID: 2704
		// (get) Token: 0x06003B93 RID: 15251 RVA: 0x001677D3 File Offset: 0x001659D3
		// (set) Token: 0x06003B94 RID: 15252 RVA: 0x001677DB File Offset: 0x001659DB
		public List<string> finishedTechsNames { get; private set; }

		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x06003B95 RID: 15253 RVA: 0x001677E4 File Offset: 0x001659E4
		// (set) Token: 0x06003B96 RID: 15254 RVA: 0x001677EC File Offset: 0x001659EC
		public List<string> finishedOneTimeOnlyProjectNames { get; private set; }

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06003B97 RID: 15255 RVA: 0x001677F5 File Offset: 0x001659F5
		public static bool UseHarshTechTree
		{
			get
			{
				return GameStateManager.GlobalResearch().useHarshTree;
			}
		}

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x06003B98 RID: 15256 RVA: 0x00167801 File Offset: 0x00165A01
		public static TIGlobalResearchState globalResearch
		{
			get
			{
				return GameStateManager.GlobalResearch();
			}
		}

		// Token: 0x06003B99 RID: 15257 RVA: 0x00167808 File Offset: 0x00165A08
		public override bool Initialize()
		{
			this.finishedTechsNames = new List<string>();
			this.displayName = "Global Research";
			return true;
		}

		// Token: 0x06003B9A RID: 15258 RVA: 0x00167824 File Offset: 0x00165A24
		public override void PostGameStateCreateInit_OnCreationOnly_1()
		{
			if (!this.gameStateSubjectCreated)
			{
				this.campaignStartYear = GameStateManager.Time().template.year;
				this.techProgress = new TechProgress[3];
				this.techProgress[0] = new TechProgress(GameStateManager.Time().template.startingTechs[0]);
				this.techProgress[1] = new TechProgress(GameStateManager.Time().template.startingTechs[1]);
				this.techProgress[2] = new TechProgress(GameStateManager.Time().template.startingTechs[2]);
				base.PostGameStateCreateInit_OnCreationOnly_1();
			}
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x001678C0 File Offset: 0x00165AC0
		public override void PostGlobalGameStateCreateInit_2()
		{
			this._allTechs = new List<TITechTemplate>();
			this._allProjects = new List<TIProjectTemplate>();
			if (this.finishedOneTimeOnlyProjectNames == null)
			{
				this.finishedOneTimeOnlyProjectNames = new List<string>();
			}
			if (this.endGameTechsCompletedByCategory == null)
			{
				this.endGameTechsCompletedByCategory = Enums.TechCategories.ToDictionary<TechCategory, TechCategory, int>((TechCategory x) => x, (TechCategory y) => 0);
			}
			foreach (TITechTemplate titechTemplate in TemplateManager.IterateByClass<TITechTemplate>(true))
			{
				this._allTechs.Add(titechTemplate);
			}
			foreach (TIProjectTemplate tiprojectTemplate in TemplateManager.IterateByClass<TIProjectTemplate>(true))
			{
				this._allProjects.Add(tiprojectTemplate);
			}
			int k;
			Action<TIFactionState> <>9__2;
			int i;
			for (k = 0; k <= 2; k = i + 1)
			{
				if (float.IsNaN(this.techProgress[k].accumulatedResearch) || float.IsInfinity(this.techProgress[k].accumulatedResearch))
				{
					this.techProgress[k].accumulatedResearch = 0f;
					List<TIFactionState> list = this.techProgress[k].factionContributions.Keys.ToList<TIFactionState>();
					Action<TIFactionState> action;
					if ((action = <>9__2) == null)
					{
						action = (<>9__2 = delegate(TIFactionState x)
						{
							this.techProgress[k].factionContributions[x] = 0f;
						});
					}
					list.ForEach(action);
				}
				i = k;
			}
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x00167AA0 File Offset: 0x00165CA0
		public override void PostInitializationInit_4()
		{
			this.finishedTechs = new List<TITechTemplate>();
			this.finishedOneTimeOnlyProjects = new List<TIProjectTemplate>();
			if (!this.gameStateSubjectCreated)
			{
				foreach (TITechTemplate titechTemplate in TemplateManager.IterateByClass<TITechTemplate>(true))
				{
					if (titechTemplate.FinishedBeforeCampaignStart(this.campaignStartYear))
					{
						this.GrantTech(titechTemplate.dataName, false, true);
					}
				}
				foreach (string text in GameStateManager.Time().template.globalTechsCompleted.Where<string>((string x) => !string.IsNullOrEmpty(x)))
				{
					this.GrantTech(text, false, true);
				}
				this.gameStateSubjectCreated = true;
			}
			else
			{
				foreach (string text2 in this.finishedTechsNames)
				{
					this.AddFinishedTech(text2, true);
				}
				foreach (string text3 in this.finishedOneTimeOnlyProjectNames)
				{
					TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(text3, false);
					if (tiprojectTemplate != null)
					{
						this.finishedOneTimeOnlyProjects.AddUnique(tiprojectTemplate);
					}
				}
			}
			foreach (TITechTemplate titechTemplate2 in this.finishedTechs.ToList<TITechTemplate>())
			{
				if (!titechTemplate2.TechPrereqsSatisfied(this.finishedTechs))
				{
					foreach (TIGenericTechTemplate tigenericTechTemplate in titechTemplate2.TechPrereqs)
					{
						if (!this.finishedTechs.Contains(tigenericTechTemplate))
						{
							this.GrantTech(tigenericTechTemplate.dataName, false, true);
							Log.Error("Save repair: granting " + tigenericTechTemplate.dataName + " as prereq for already finished tech " + titechTemplate2.dataName, Array.Empty<object>());
						}
					}
				}
			}
		}

		// Token: 0x06003B9D RID: 15261 RVA: 0x00167D10 File Offset: 0x00165F10
		public static List<TITechTemplate> GetAllTechs()
		{
			return GameStateManager.GlobalResearch()._allTechs;
		}

		// Token: 0x06003B9E RID: 15262 RVA: 0x00167D1C File Offset: 0x00165F1C
		public static List<TIProjectTemplate> GetAllProjects()
		{
			return GameStateManager.GlobalResearch()._allProjects;
		}

		// Token: 0x06003B9F RID: 15263 RVA: 0x00167D28 File Offset: 0x00165F28
		private void AddFinishedTech(string templateName, bool duringInit = false)
		{
			TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(templateName, false);
			if (titechTemplate != null)
			{
				this.AddFinishedTech(titechTemplate, duringInit);
				return;
			}
			Log.Error("Bad or finished templateName " + templateName + " passed to Set Finished Techs", Array.Empty<object>());
		}

		// Token: 0x06003BA0 RID: 15264 RVA: 0x00167D64 File Offset: 0x00165F64
		private void AddFinishedTech(TITechTemplate finishedTech, bool duringInit = false)
		{
			if (finishedTech != null)
			{
				if (!this.finishedTechsNames.Contains(finishedTech.dataName))
				{
					this.finishedTechsNames.Add(finishedTech.dataName);
				}
				if (!this.finishedTechs.Contains(finishedTech))
				{
					this.finishedTechs.Add(finishedTech);
				}
				if (finishedTech.endGameTech && !duringInit)
				{
					Dictionary<TechCategory, int> dictionary = this.endGameTechsCompletedByCategory;
					TechCategory techCategory = finishedTech.techCategory;
					dictionary[techCategory]++;
				}
				if (this.AllTechsFinished())
				{
					GameControl.control.activePlayer.UnlockAchievement("researchAllTechs");
				}
			}
		}

		// Token: 0x06003BA1 RID: 15265 RVA: 0x00167DFA File Offset: 0x00165FFA
		public void AddFinishedOneTimeOnlyProject(TIProjectTemplate finishedProject)
		{
			if (finishedProject != null)
			{
				this.finishedOneTimeOnlyProjectNames.AddUnique(finishedProject.dataName);
				this.finishedOneTimeOnlyProjects.AddUnique(finishedProject);
			}
		}

		// Token: 0x06003BA2 RID: 15266 RVA: 0x00167E20 File Offset: 0x00166020
		private bool AllTechsFinished()
		{
			List<TITechTemplate> list = (from x in TIGlobalResearchState.GetAllTechs()
				where !x.endGameTech
				select x).ToList<TITechTemplate>();
			List<TITechTemplate> list2 = this.finishedTechs.Where<TITechTemplate>((TITechTemplate x) => !x.endGameTech).ToList<TITechTemplate>();
			return list.Count == list2.Count;
		}

		// Token: 0x06003BA3 RID: 15267 RVA: 0x00167E9C File Offset: 0x0016609C
		public string TechCompletionDate(int slot)
		{
			TIDateTime tidateTime = TITimeState.Now();
			TechProgress techProgress = this.GetTechProgress(slot);
			float num = techProgress.techTemplate.GetResearchCost(null) - techProgress.accumulatedResearch;
			if (num > 0f)
			{
				float num2 = 0f;
				foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions())
				{
					num2 += tifactionState.PointsToSlot(slot, tifactionState.GetDailyIncome(FactionResource.Research, false, false), (float)tifactionState.TotalResearchWeights(tifactionState.OrgProjectAllowed(), tifactionState.HabProjectAllowed()));
				}
				if (num2 <= 0f)
				{
					return string.Empty;
				}
				float num3 = num / num2;
				if (!tidateTime.TryAddDays(num3))
				{
					return string.Empty;
				}
			}
			return tidateTime.ToCustomDateString();
		}

		// Token: 0x06003BA4 RID: 15268 RVA: 0x00167F50 File Offset: 0x00166150
		public void AddResearchToTech(int slot, float contribution, TIFactionState factionState)
		{
			TIHistoricalData.Record_Sum(factionState, "Effective research per day", contribution / 60f, 60f, true);
			TIHistoricalData.Record_Sum(factionState, "Effective global research per day", contribution / 60f, 60f, true);
			this.techProgress[slot].accumulatedResearch += contribution;
			if (!this.techProgress[slot].factionContributions.Keys.Contains(factionState))
			{
				this.techProgress[slot].factionContributions.Add(factionState, contribution);
				return;
			}
			Dictionary<TIFactionState, float> factionContributions = this.techProgress[slot].factionContributions;
			factionContributions[factionState] += contribution;
		}

		// Token: 0x06003BA5 RID: 15269 RVA: 0x00167FF1 File Offset: 0x001661F1
		public static List<TITechTemplate> FinishedTechs()
		{
			return GameStateManager.GlobalResearch().finishedTechs;
		}

		// Token: 0x06003BA6 RID: 15270 RVA: 0x00167FFD File Offset: 0x001661FD
		public static bool TechFinished(TITechTemplate tech)
		{
			return GameStateManager.GlobalResearch().finishedTechs.Contains(tech);
		}

		// Token: 0x06003BA7 RID: 15271 RVA: 0x0016800F File Offset: 0x0016620F
		public bool IsTechFinished(TITechTemplate tech)
		{
			return this.finishedTechs.Contains(tech);
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06003BA8 RID: 15272 RVA: 0x0016801D File Offset: 0x0016621D
		public static TITechTemplate MostRecentlyFinishedTech
		{
			get
			{
				return TIGlobalResearchState.FinishedTechs().LastOrDefault<TITechTemplate>();
			}
		}

		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x06003BA9 RID: 15273 RVA: 0x0016802C File Offset: 0x0016622C
		public static List<TITechTemplate> UnlockedTechs
		{
			get
			{
				TIGlobalResearchState tiglobalResearchState = GameStateManager.GlobalResearch();
				List<TITechTemplate> list = new List<TITechTemplate>();
				List<TITechTemplate> list2 = new List<TITechTemplate>();
				foreach (TITechTemplate titechTemplate in TIGlobalResearchState.GetAllTechs())
				{
					if (titechTemplate.endGameTech)
					{
						list.Add(titechTemplate);
					}
					else if (!tiglobalResearchState.finishedTechs.Contains(titechTemplate) && titechTemplate.TechPrereqsSatisfied(tiglobalResearchState.finishedTechs))
					{
						list2.Add(titechTemplate);
					}
				}
				if (list2.Count == 0)
				{
					list2.AddRange(list);
				}
				return list2;
			}
		}

		// Token: 0x06003BAA RID: 15274 RVA: 0x001680D4 File Offset: 0x001662D4
		public static List<TITechTemplate> AvailableTechs()
		{
			TIGlobalResearchState tiglobalResearchState = GameStateManager.GlobalResearch();
			List<TITechTemplate> list = new List<TITechTemplate>();
			List<TITechTemplate> inProgress = TIGlobalResearchState.CurrentResearchingTechs;
			List<TITechTemplate> list2 = new List<TITechTemplate>();
			foreach (TITechTemplate titechTemplate in TIGlobalResearchState.GetAllTechs())
			{
				if (titechTemplate.endGameTech)
				{
					list2.Add(titechTemplate);
				}
				else if (!tiglobalResearchState.finishedTechs.Contains(titechTemplate) && !inProgress.Contains(titechTemplate) && titechTemplate.TechPrereqsSatisfied(tiglobalResearchState.finishedTechs))
				{
					list.Add(titechTemplate);
				}
			}
			if (list.Count == 0)
			{
				list.AddRange(list2.Where<TITechTemplate>((TITechTemplate x) => !inProgress.Contains(x)));
			}
			return list;
		}

		// Token: 0x06003BAB RID: 15275 RVA: 0x001681B0 File Offset: 0x001663B0
		public int GetSlotForFactionCompletedTechs(TIFactionState faction)
		{
			foreach (FinishedTechData finishedTechData in this.finishedTechData)
			{
				if (finishedTechData.winningCouncil == faction)
				{
					return finishedTechData.slot;
				}
			}
			return -1;
		}

		// Token: 0x06003BAC RID: 15276 RVA: 0x00168218 File Offset: 0x00166418
		public void AssignNewTechToSlot(TITechTemplate template, int slot)
		{
			TITechTemplate techTemplate = this.techProgress[slot].techTemplate;
			this.techProgress[slot].techTemplateName = template.dataName;
			FinishedTechData finishedTechData = default(FinishedTechData);
			foreach (FinishedTechData finishedTechData2 in this.finishedTechData)
			{
				if (finishedTechData2.slot == slot)
				{
					finishedTechData = finishedTechData2;
					break;
				}
			}
			this.finishedTechData.Remove(finishedTechData);
			if (finishedTechData.winningCouncil == null || techTemplate == null || template == null)
			{
				Log.Error("Nullage in AssignnewTechToSlot", Array.Empty<object>());
				return;
			}
			this.techProgress[slot].selector = finishedTechData.winningCouncil;
			TINotificationQueueState.LogTechCompleteAndNewTechSelected(finishedTechData.winningCouncil, techTemplate, template);
		}

		// Token: 0x06003BAD RID: 15277 RVA: 0x001682EC File Offset: 0x001664EC
		public TechProgress GetTechProgress(int slot)
		{
			return this.techProgress[slot];
		}

		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x06003BAE RID: 15278 RVA: 0x001682F8 File Offset: 0x001664F8
		public static List<TITechTemplate> CurrentResearchingTechs
		{
			get
			{
				TIGlobalResearchState tiglobalResearchState = GameStateManager.GlobalResearch();
				return new List<TITechTemplate>
				{
					tiglobalResearchState.techProgress[0].techTemplate,
					tiglobalResearchState.techProgress[1].techTemplate,
					tiglobalResearchState.techProgress[2].techTemplate
				};
			}
		}

		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x06003BAF RID: 15279 RVA: 0x00168349 File Offset: 0x00166549
		// (set) Token: 0x06003BB0 RID: 15280 RVA: 0x00168351 File Offset: 0x00166551
		public List<string> FinishedTechDataNames
		{
			get
			{
				return this.finishedTechsNames;
			}
			set
			{
				this.finishedTechsNames = value;
			}
		}

		// Token: 0x06003BB1 RID: 15281 RVA: 0x0016835C File Offset: 0x0016655C
		public static float GetAccumulatedResearchByTech(TITechTemplate tech)
		{
			TIGlobalResearchState tiglobalResearchState = GameStateManager.GlobalResearch();
			for (int i = 0; i <= 2; i++)
			{
				if (tiglobalResearchState.techProgress[i].techTemplate == tech)
				{
					return tiglobalResearchState.techProgress[i].accumulatedResearch;
				}
			}
			return 0f;
		}

		// Token: 0x06003BB2 RID: 15282 RVA: 0x001683A0 File Offset: 0x001665A0
		public TIFactionState Leader(int slot)
		{
			if (this.techProgress[slot].factionContributions.Count > 0)
			{
				return this.techProgress[slot].factionContributions.Aggregate<KeyValuePair<TIFactionState, float>>(delegate(KeyValuePair<TIFactionState, float> l, KeyValuePair<TIFactionState, float> r)
				{
					if (l.Value <= r.Value)
					{
						return r;
					}
					return l;
				}).Key;
			}
			return null;
		}

		// Token: 0x06003BB3 RID: 15283 RVA: 0x00168400 File Offset: 0x00166600
		public int GetSlotForTech(TITechTemplate tech)
		{
			foreach (TechProgress techProgress in this.techProgress)
			{
				if (techProgress.techTemplate == tech)
				{
					return this.techProgress.IndexOf(techProgress);
				}
			}
			return -1;
		}

		// Token: 0x06003BB4 RID: 15284 RVA: 0x00168440 File Offset: 0x00166640
		public void GrantTech(string techName, bool logit = false, bool startup = false)
		{
			TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(techName, false);
			if (titechTemplate != null)
			{
				this.AddFinishedTech(techName, false);
				int num = (TIGlobalResearchState.CurrentResearchingTechs.Contains(titechTemplate) ? this.GetSlotForTech(titechTemplate) : (-1));
				this.finishedTechData.Add(new FinishedTechData
				{
					winningCouncil = null,
					slot = num
				});
				foreach (TIFactionState tifactionState in GameStateManager.AllHumanFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
				{
					tifactionState.OnPublicTechCompleted(titechTemplate, 0f);
					GameControl.eventManager.TriggerEvent(new ResearchUpdated(tifactionState), null, new object[] { tifactionState });
				}
				if (logit)
				{
					TINotificationQueueState.LogTechComplete(GameControl.control.activePlayer, titechTemplate, num, true, "", "");
				}
				foreach (TIEffectTemplate tieffectTemplate in titechTemplate.Effects)
				{
					TIEffectsState.AddEffect(tieffectTemplate, null, null, null, techName);
				}
				foreach (TIFactionState tifactionState2 in GameStateManager.AllHumanFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
				{
					tifactionState2.OnPublicTechCompleted_PostEffectsApplied(titechTemplate, startup);
				}
			}
		}

		// Token: 0x06003BB5 RID: 15285 RVA: 0x001685C0 File Offset: 0x001667C0
		public void OnTechFinished(int slot)
		{
			TIFactionState tifactionState = this.Leader(slot);
			FinishedTechData finishedTechData = new FinishedTechData(slot, tifactionState);
			this.finishedTechData.Add(finishedTechData);
			this.AddFinishedTech(this.techProgress[slot].techTemplate, false);
			string longtermTechTarget = tifactionState.longtermTechTarget;
			this.CheckForAutoPickTech(tifactionState);
			TIGenericTechTemplate tigenericTechTemplate = null;
			if (!string.IsNullOrEmpty(longtermTechTarget))
			{
				tigenericTechTemplate = (TITechTemplate)TIGlobalResearchState.globalResearch.nextPrereqTechToTarget(longtermTechTarget, tifactionState, true);
			}
			TIFactionState tifactionState2 = tifactionState;
			TITechTemplate techTemplate = this.techProgress[slot].techTemplate;
			bool flag = false;
			string text = (tifactionState.isActivePlayer ? ((tigenericTechTemplate != null) ? tigenericTechTemplate.displayName : null) : "");
			TIGenericTechTemplate tigenericTechTemplate2 = TemplateManager.Find<TIGenericTechTemplate>(longtermTechTarget, true);
			TINotificationQueueState.LogTechComplete(tifactionState2, techTemplate, slot, flag, text, (tigenericTechTemplate2 != null) ? tigenericTechTemplate2.displayName : null);
			foreach (TIFactionState tifactionState3 in GameStateManager.AllHumanFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
			{
				tifactionState3.OnPublicTechCompleted(this.techProgress[slot].techTemplate, this.techProgress[slot].factionContributions.ContainsKey(tifactionState3) ? (this.techProgress[slot].factionContributions[tifactionState3] / this.techProgress[slot].accumulatedResearch) : 0f);
				GameControl.eventManager.TriggerEvent(new ResearchUpdated(tifactionState3), null, new object[] { tifactionState3 });
			}
			foreach (TIEffectTemplate tieffectTemplate in this.techProgress[slot].techTemplate.Effects)
			{
				TIEffectsState.AddEffect(tieffectTemplate, tifactionState, null, null, this.techProgress[slot].techTemplate.dataName);
			}
			foreach (TIFactionState tifactionState4 in GameStateManager.AllHumanFactions().ToList<TIFactionState>().Shuffle<TIFactionState>())
			{
				tifactionState4.OnPublicTechCompleted_PostEffectsApplied(this.techProgress[slot].techTemplate, false);
			}
			this.techProgress[slot].accumulatedResearch = 0f;
			foreach (TIFactionState tifactionState5 in GameStateManager.AllHumanFactions())
			{
				this.techProgress[slot].factionContributions[tifactionState5] = 0f;
				tifactionState5.EndTechRace();
				tifactionState5.ClearPassiveTechSlot();
			}
			float num = this.finishedTechs.Sum<TITechTemplate>((TITechTemplate x) => x.GetResearchCost(null));
			float num2 = GameStateManager.AllHumanFactions().Sum<TIFactionState>((TIFactionState x) => x.completedProjects.Sum<TIProjectTemplate>((TIProjectTemplate y) => y.GetResearchCost(x)));
			TIHistoricalData.Record(this, "Total tech investment", num, 0f, true);
			TIHistoricalData.Record(this, "Total project investment", num2, 0f, true);
			TIHistoricalData.Record(this, "Total tech investment ratio", num / (num + num2), 0f, true);
			if (tigenericTechTemplate != null)
			{
				PlayerAction playerAction = new SelectTechAction(tifactionState, slot, (TITechTemplate)tigenericTechTemplate);
				tifactionState.playerControl.StartAction(playerAction);
			}
		}

		// Token: 0x06003BB6 RID: 15286 RVA: 0x001688F4 File Offset: 0x00166AF4
		public bool CheckForAutoPickTech(TIFactionState faction)
		{
			if (string.IsNullOrEmpty(faction.longtermTechTarget))
			{
				return false;
			}
			string targetTechName = faction.longtermTechTarget;
			if (!this.FinishedTechDataNames.Contains(targetTechName) && faction.completedProjectsDistinct.Where<TIProjectTemplate>((TIProjectTemplate x) => x.dataName == targetTechName).ToList<TIProjectTemplate>().Count == 0)
			{
				return true;
			}
			faction.SetLongTermTechTarget("");
			return false;
		}

		// Token: 0x06003BB7 RID: 15287 RVA: 0x00168968 File Offset: 0x00166B68
		public TIGenericTechTemplate nextPrereqTechToTarget(string targetTechName, TIFactionState faction, bool needTechReturned = false)
		{
			TIGenericTechTemplate targetTechTemplate = TemplateManager.Find<TIGenericTechTemplate>(targetTechName, true);
			targetTechTemplate.AllPrereqFor(faction, true);
			if (targetTechTemplate.isProject())
			{
				if (faction.availableProjects.Contains(targetTechTemplate) && !faction.CurrentlyActiveProjects().Contains(targetTechTemplate) && ((!needTechReturned && targetTechTemplate.isProject()) || (needTechReturned && targetTechTemplate.isGlobalTech())))
				{
					return targetTechTemplate;
				}
				TIGenericTechTemplate tigenericTechTemplate;
				if (needTechReturned)
				{
					tigenericTechTemplate = (from x in TIGlobalResearchState.AvailableTechs()
						where this.GetDescendentTechs(new TITechTemplate[] { x }, faction, 99).Contains(targetTechTemplate) && !TIGlobalResearchState.CurrentResearchingTechs.Contains(x)
						select x).FirstOrDefault<TITechTemplate>();
				}
				else
				{
					tigenericTechTemplate = faction.availableProjects.Where<TIProjectTemplate>((TIProjectTemplate x) => this.GetDescendentTechs(new TIProjectTemplate[] { x }, faction, 99).Contains(targetTechTemplate) && x.ref_project != null && !faction.CurrentlyActiveProjects().Contains(x)).FirstOrDefault<TIProjectTemplate>();
				}
				return tigenericTechTemplate;
			}
			else
			{
				if (TIGlobalResearchState.AvailableTechs().Contains(targetTechTemplate) && !TIGlobalResearchState.CurrentResearchingTechs.Contains(targetTechTemplate))
				{
					return targetTechTemplate;
				}
				return (from x in TIGlobalResearchState.AvailableTechs()
					where this.GetDescendentTechs(new TITechTemplate[] { x }, faction, 99).Contains(targetTechTemplate) && !TIGlobalResearchState.CurrentResearchingTechs.Contains(x)
					select x).FirstOrDefault<TITechTemplate>();
			}
		}

		// Token: 0x06003BB8 RID: 15288 RVA: 0x00168AA0 File Offset: 0x00166CA0
		public IEnumerable<TIGenericTechTemplate> GetDescendentTechs(IEnumerable<TIGenericTechTemplate> ancestors, TIFactionState faction, int generationCount)
		{
			if (generationCount == 0)
			{
				return Enumerable.Empty<TIGenericTechTemplate>();
			}
			IEnumerable<TIGenericTechTemplate> enumerable = ancestors.SelectMany<TIGenericTechTemplate, TIGenericTechTemplate>((TIGenericTechTemplate x) => x.AllPrereqFor(faction, false));
			if (generationCount == 1)
			{
				return enumerable;
			}
			return enumerable.Union<TIGenericTechTemplate>(this.GetDescendentTechs(enumerable, faction, generationCount - 1));
		}

		// Token: 0x06003BB9 RID: 15289 RVA: 0x00168AF4 File Offset: 0x00166CF4
		public void CheckForCompletedTechs()
		{
			int num = 0;
			foreach (TechProgress techProgress in this.techProgress)
			{
				if (techProgress.accumulatedResearch >= techProgress.techTemplate.GetResearchCost(null))
				{
					this.OnTechFinished(num);
				}
				num++;
			}
		}

		// Token: 0x06003BBA RID: 15290 RVA: 0x00168B3B File Offset: 0x00166D3B
		public void DailyResearchUpdate(TimeEventStart e)
		{
			GameControl.StartSimulationAction(this.dailyResearchCmd);
		}

		// Token: 0x06003BBB RID: 15291 RVA: 0x00168B48 File Offset: 0x00166D48
		public void CreateVisualizer(TIDataTemplate myTemplate)
		{
			this.dailyResearchCmd.researchState = this;
			GameControl.eventManager.AddListener<TimeEventStart>(new EventManager.EventDelegate<TimeEventStart>(this.DailyResearchUpdate), "DailyUpdate", null, true, false);
		}

		// Token: 0x040025D3 RID: 9683
		[SerializeField]
		private TechProgress[] techProgress;

		// Token: 0x040025D4 RID: 9684
		[SerializeField]
		private int campaignStartYear;

		// Token: 0x040025D7 RID: 9687
		private List<FinishedTechData> finishedTechData = new List<FinishedTechData>();

		// Token: 0x040025D8 RID: 9688
		[SerializeField]
		private bool gameStateSubjectCreated;

		// Token: 0x040025D9 RID: 9689
		private List<TITechTemplate> finishedTechs;

		// Token: 0x040025DA RID: 9690
		[fsIgnore]
		public List<TIProjectTemplate> finishedOneTimeOnlyProjects;

		// Token: 0x040025DB RID: 9691
		private List<TITechTemplate> _allTechs;

		// Token: 0x040025DC RID: 9692
		private List<TIProjectTemplate> _allProjects;

		// Token: 0x040025DD RID: 9693
		[SerializeField]
		private bool useHarshTree;

		// Token: 0x040025DE RID: 9694
		public Dictionary<TechCategory, int> endGameTechsCompletedByCategory;

		// Token: 0x040025DF RID: 9695
		[fsIgnore]
		protected JointResearchDailyUpdate dailyResearchCmd = new JointResearchDailyUpdate();
	}
}
