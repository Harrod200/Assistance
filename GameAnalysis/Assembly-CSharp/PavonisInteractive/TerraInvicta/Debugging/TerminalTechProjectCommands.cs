using System;
using System.Collections.Generic;
using System.Linq;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x0200091A RID: 2330
	public class TerminalTechProjectCommands
	{
		// Token: 0x06005923 RID: 22819 RVA: 0x0028E965 File Offset: 0x0028CB65
		public TerminalTechProjectCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x06005924 RID: 22820 RVA: 0x0028E97C File Offset: 0x0028CB7C
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("givetech", new CommandHandler(this.GiveTech), "Give tech to council. 'givetech dataName'");
			this.terminalController.RegisterCommand("giveproject", new CommandHandler(this.GiveProject), "Give finished project to council. 'giveproject dataName (opt: factionName)'");
			this.terminalController.RegisterCommand("completeproject", new CommandHandler(this.CompleteProject), "Complete Current Project in slot 3 for faction. 'completeproject EscapeCouncil'");
			this.terminalController.RegisterCommand("givealltechs", new CommandHandler(this.GiveAllTechsAndProjects), "Complete all techs and non-repeatable projects. 'givealltechs (opt: factionName)'");
			this.terminalController.RegisterCommand("DumpTechScores", new CommandHandler(this.DumpTechScores), "Dump all tech scores for faction. 'DumpTechScores ResistCouncil'");
		}

		// Token: 0x06005925 RID: 22821 RVA: 0x0028EA30 File Offset: 0x0028CC30
		private void DumpTechScores(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("DumpTechScores Error: Missing faction name");
				return;
			}
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			if (tifactionState != null)
			{
				Dictionary<TITechTemplate, float> dictionary = new Dictionary<TITechTemplate, float>();
				List<TITechTemplate> list = TIGlobalResearchState.AvailableTechs();
				list.Any<TITechTemplate>((TITechTemplate x) => x.AI_criticalTech);
				bool shipBuilding = tifactionState.shipBuilding;
				TIFactionState.LogAI(tifactionState.displayName + " tech scores:", false);
				string cheapestForcedTechName = tifactionState.cheapestForcedTechName;
				IEnumerable<TIMissionTemplate> allPossibleMissions = tifactionState.GetAllPossibleMissions();
				foreach (TITechTemplate titechTemplate in list)
				{
					dictionary.Add(titechTemplate, AIEvaluators.ScoreTech(tifactionState, titechTemplate, true, cheapestForcedTechName == titechTemplate.dataName, shipBuilding, allPossibleMissions));
				}
				foreach (KeyValuePair<TITechTemplate, float> keyValuePair in dictionary.OrderByDescending<KeyValuePair<TITechTemplate, float>, float>((KeyValuePair<TITechTemplate, float> x) => x.Value))
				{
					TIFactionState.LogAI(keyValuePair.Key.displayName + "\t" + keyValuePair.Value.ToString("N2"), false);
				}
				TIFactionState.LogAI(tifactionState.displayName + " project scores:", false);
				List<TIProjectTemplate> availableProjects = tifactionState.availableProjects;
				Dictionary<TIProjectTemplate, float> dictionary2 = new Dictionary<TIProjectTemplate, float>();
				list.Any<TITechTemplate>((TITechTemplate x) => x.AI_criticalTech);
				foreach (TIProjectTemplate tiprojectTemplate in availableProjects)
				{
					dictionary2.Add(tiprojectTemplate, AIEvaluators.ScoreTech(tifactionState, tiprojectTemplate, true, cheapestForcedTechName == tiprojectTemplate.dataName, shipBuilding, allPossibleMissions));
				}
				using (IEnumerator<KeyValuePair<TIProjectTemplate, float>> enumerator4 = dictionary2.OrderByDescending<KeyValuePair<TIProjectTemplate, float>, float>((KeyValuePair<TIProjectTemplate, float> x) => x.Value).GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<TIProjectTemplate, float> keyValuePair2 = enumerator4.Current;
						TIFactionState.LogAI(keyValuePair2.Key.displayName + "\t" + keyValuePair2.Value.ToString("N2"), false);
					}
					return;
				}
			}
			this.terminalController.OutputError("DumpTechScores Error: no faction found");
		}

		// Token: 0x06005926 RID: 22822 RVA: 0x0028ECF4 File Offset: 0x0028CEF4
		public void GiveTech(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("givetech Error: Missing tech name");
				return;
			}
			TITechTemplate titechTemplate = TemplateManager.Find<TITechTemplate>(args[0], false);
			if (titechTemplate != null)
			{
				GameStateManager.GlobalResearch().GrantTech(titechTemplate.dataName, true, false);
				return;
			}
			this.terminalController.OutputError("givetech Error: couldn't parse tech: " + args[0]);
		}

		// Token: 0x06005927 RID: 22823 RVA: 0x0028ED50 File Offset: 0x0028CF50
		public void GiveProject(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("giveproject Error: Missing project name");
				return;
			}
			TIProjectTemplate tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(args[0], false);
			if (tiprojectTemplate == null)
			{
				args[0] = "Project_" + args[0];
				tiprojectTemplate = TemplateManager.Find<TIProjectTemplate>(args[0], false);
			}
			TIFactionState tifactionState = ((args.Length == 2) ? GameStateManager.FindByTemplate<TIFactionState>(args[1], false) : GameControl.control.activePlayer);
			if (tiprojectTemplate != null)
			{
				tifactionState.OnProjectComplete(tiprojectTemplate, tifactionState.GetSlotForProject(tiprojectTemplate), false, false);
				return;
			}
			this.terminalController.OutputError("giveproject Error: couldn't parse project: " + args[0]);
		}

		// Token: 0x06005928 RID: 22824 RVA: 0x0028EDE4 File Offset: 0x0028CFE4
		public void CompleteProject(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("giveproject Error: Missing faction dataname");
			}
			TIFactionState tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			if (tifactionState == null)
			{
				tifactionState = GameControl.control.activePlayer;
			}
			if (tifactionState.IsActiveHumanFaction)
			{
				ProjectProgress projectProgressInSlot = tifactionState.GetProjectProgressInSlot(3);
				this.terminalController.Output("Completing " + projectProgressInSlot.projectTemplate.displayName);
				tifactionState.AddResearchToProject(3, projectProgressInSlot.projectTemplate.GetResearchCost(tifactionState) - projectProgressInSlot.accumulatedResearch + 1f);
				tifactionState.OnProjectCompleteInSlot(3);
			}
		}

		// Token: 0x06005929 RID: 22825 RVA: 0x0028EE7C File Offset: 0x0028D07C
		public void GiveAllTechsAndProjects(string[] args)
		{
			TIFactionState tifactionState = GameControl.control.activePlayer;
			if (args.Length == 1)
			{
				tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[0], false);
			}
			foreach (TITechTemplate titechTemplate in TIGlobalResearchState.GetAllTechs())
			{
				if (!TIGlobalResearchState.TechFinished(titechTemplate) && !titechTemplate.ref_tech.endGameTech)
				{
					GameStateManager.GlobalResearch().GrantTech(titechTemplate.dataName, false, false);
				}
			}
			if (tifactionState != null)
			{
				using (List<TIProjectTemplate>.Enumerator enumerator2 = TIGlobalResearchState.GetAllProjects().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TIProjectTemplate tiprojectTemplate = enumerator2.Current;
						if (tiprojectTemplate.FactionPrereqsSatisfied(tifactionState) && !tiprojectTemplate.ref_project.repeatable)
						{
							tifactionState.OnProjectComplete(tiprojectTemplate, tifactionState.GetSlotForProject(tiprojectTemplate), true, false);
						}
					}
					return;
				}
			}
			if (args.Length != 0)
			{
				this.terminalController.OutputError("GiveAllTechsAndProjects Error: bad faction dataname");
			}
		}

		// Token: 0x04004077 RID: 16503
		private TerminalController terminalController;
	}
}
