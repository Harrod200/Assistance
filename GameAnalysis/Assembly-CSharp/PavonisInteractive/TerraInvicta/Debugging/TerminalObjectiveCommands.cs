using System;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000918 RID: 2328
	public class TerminalObjectiveCommands
	{
		// Token: 0x0600590B RID: 22795 RVA: 0x0028D8D1 File Offset: 0x0028BAD1
		public TerminalObjectiveCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x0600590C RID: 22796 RVA: 0x0028D8E8 File Offset: 0x0028BAE8
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("completeobjective", new CommandHandler(this.CompleteObjective), "Add objective dataName and optionally faction dataName to complete an objective eg; 'completeobjective ResearchExotics,ResistCouncil'");
			this.terminalController.RegisterCommand("completemilestone", new CommandHandler(this.CompleteMilestone), "Add milestone enum and optionally faction dataname to complete a milestone");
			this.terminalController.RegisterCommand("dumpgoals", new CommandHandler(this.DumpGoals), "Dump all faction goals to the log");
			this.terminalController.RegisterCommand("dumpshipyards", new CommandHandler(this.DumpShipyards), "Dump all shipyard queues to the log");
			this.terminalController.RegisterCommand("dumpsavings", new CommandHandler(this.DumpSavings), "Dump faction saving data to the log");
			this.terminalController.RegisterCommand("dumpmissions", new CommandHandler(this.DumpMissions), "Dump all mission data to the log");
			this.terminalController.RegisterCommand("resetachievement", new CommandHandler(this.ResetAchievement), "resets an achievement eg; resetachievement winCombat");
			this.terminalController.RegisterCommand("resetallachievements", new CommandHandler(this.ResetAchievements), "resets all achievements");
		}

		// Token: 0x0600590D RID: 22797 RVA: 0x0028DA00 File Offset: 0x0028BC00
		private void CompleteObjective(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Missing Objective DataName. Defaults to activeplayer faction if no or bad faction entered.");
				return;
			}
			TIObjectiveTemplate tiobjectiveTemplate = TemplateManager.Find<TIObjectiveTemplate>(args[0], false);
			if (tiobjectiveTemplate == null)
			{
				this.terminalController.OutputError("Bad objective dataName " + args[0] + " entered.");
				return;
			}
			TIFactionState tifactionState = null;
			if (args.Length >= 2)
			{
				tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[1], false);
			}
			if (tifactionState == null)
			{
				tifactionState = GameControl.control.activePlayer;
			}
			tifactionState.CompleteObjective(tiobjectiveTemplate);
		}

		// Token: 0x0600590E RID: 22798 RVA: 0x0028DA7C File Offset: 0x0028BC7C
		private void CompleteMilestone(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("Missing Milestone enum. Defaults to activeplayer faction if no or bad faction entered.");
				return;
			}
			CampaignMilestone campaignMilestone = args[0].ToEnum(CampaignMilestone.None);
			if (campaignMilestone == CampaignMilestone.None)
			{
				this.terminalController.OutputError("Bad milestone name " + args[0] + " entered.");
				return;
			}
			TIFactionState tifactionState = null;
			if (args.Length >= 2)
			{
				tifactionState = GameStateManager.FindByTemplate<TIFactionState>(args[1], false);
			}
			if (tifactionState == null)
			{
				tifactionState = GameControl.control.activePlayer;
			}
			tifactionState.CompleteMilestone(campaignMilestone);
		}

		// Token: 0x0600590F RID: 22799 RVA: 0x0028DAF8 File Offset: 0x0028BCF8
		private void DumpGoals(string[] args)
		{
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				TIFactionState.DumpGoals(array[i]);
			}
		}

		// Token: 0x06005910 RID: 22800 RVA: 0x0028DB24 File Offset: 0x0028BD24
		private void DumpShipyards(string[] args)
		{
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				TIFactionState.DumpShipyards(array[i]);
			}
		}

		// Token: 0x06005911 RID: 22801 RVA: 0x0028DB50 File Offset: 0x0028BD50
		private void DumpMissions(string[] args)
		{
			TIFactionState[] array = GameStateManager.AllFactions();
			for (int i = 0; i < array.Length; i++)
			{
				TIFactionState.DumpMissions(array[i]);
			}
		}

		// Token: 0x06005912 RID: 22802 RVA: 0x0028DB7C File Offset: 0x0028BD7C
		private void DumpSavings(string[] args)
		{
			foreach (TIFactionState tifactionState in GameStateManager.AllFactions())
			{
				if (tifactionState.AISavingTarget.active)
				{
					tifactionState.AISavingTarget.LogSavingData();
				}
			}
		}

		// Token: 0x06005913 RID: 22803 RVA: 0x0028DBB9 File Offset: 0x0028BDB9
		private void ResetAchievement(string[] args)
		{
			if (args.Length != 0)
			{
				GameControl.control.activePlayer.ResetAchievement(args[0]);
			}
		}

		// Token: 0x06005914 RID: 22804 RVA: 0x0028DBD1 File Offset: 0x0028BDD1
		private void ResetAchievements(string[] args)
		{
			GameControl.control.activePlayer.ResetAllSteamUserStats(true);
		}

		// Token: 0x04004075 RID: 16501
		private TerminalController terminalController;
	}
}
