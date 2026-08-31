using System;
using System.Linq;
using PavonisInteractive.TerraInvicta.Assets;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000912 RID: 2322
	public class TerminalAutopilotCommands
	{
		// Token: 0x17000F30 RID: 3888
		// (get) Token: 0x060058CA RID: 22730 RVA: 0x0028B049 File Offset: 0x00289249
		public Autopilot Autopilot
		{
			get
			{
				if (this.autopilot == null)
				{
					this.autopilot = new GameObject("Autopilot").AddComponent<Autopilot>();
				}
				return this.autopilot;
			}
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x0028B074 File Offset: 0x00289274
		public TerminalAutopilotCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x0028B08C File Offset: 0x0028928C
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("autopilot", new CommandHandler(this.AutopilotActivation), "Plays game automatically.");
			this.terminalController.RegisterCommand("stats", new CommandHandler(this.StatsActivation), "Record numerical data over time.");
			this.terminalController.RegisterCommand("extendedFleetUI", new CommandHandler(this.ExtendedFleetUIActivation), "Record numerical data over time.");
			this.terminalController.RegisterCommand("DEV_ResetAllGOGAchievements", new CommandHandler(this.ResetGogAchievements), "");
		}

		// Token: 0x060058CD RID: 22733 RVA: 0x0028B11D File Offset: 0x0028931D
		private void DumpEventTracker(string[] args)
		{
		}

		// Token: 0x060058CE RID: 22734 RVA: 0x0028B120 File Offset: 0x00289320
		private void AutopilotActivation(string[] args)
		{
			TIUtilities.InitRandom((int)DateTime.Now.Ticks);
			if (!this.Autopilot.Activated)
			{
				this.Autopilot.Activated = true;
			}
			else
			{
				if (args.Length != 0)
				{
					if (!args.All<string>((string x) => x == ""))
					{
						goto IL_0065;
					}
				}
				this.Autopilot.Activated = false;
			}
			IL_0065:
			for (int i = 0; i < args.Length; i++)
			{
				if (!(args[i] == ""))
				{
					int num;
					if (int.TryParse(args[i], out num))
					{
						this.Autopilot.SaveRate = Mathf.Clamp(num, 1, int.MaxValue);
					}
					else
					{
						string text = args[i].ToLowerInvariant();
						if (text != null)
						{
							if (text == "on")
							{
								this.Autopilot.Activated = true;
								goto IL_0155;
							}
							if (text == "off")
							{
								this.Autopilot.Activated = false;
								goto IL_0155;
							}
							if (text == "ignoreexceptions")
							{
								this.Autopilot.IgnoreExeptions = true;
								goto IL_0155;
							}
							if (text == "dontignoreexceptions")
							{
								this.Autopilot.IgnoreExeptions = false;
								goto IL_0155;
							}
							if (text == "stats")
							{
								TIHistoricalData.RecordDebugData = true;
								goto IL_0155;
							}
						}
						this.terminalController.OutputError("\"" + args[i] + "\" is invalid. Valid arguments are \"on\", \"off\", \"IgnoreExceptions\", \"DontIgnoreExceptions\",\nor an integer value to set # of cycles between saves.");
					}
				}
				IL_0155:;
			}
		}

		// Token: 0x060058CF RID: 22735 RVA: 0x0028B290 File Offset: 0x00289490
		private void StatsActivation(string[] args)
		{
			if (args.Length != 0)
			{
				if (!args.All<string>((string x) => x == ""))
				{
					goto IL_0031;
				}
			}
			TIHistoricalData.RecordDebugData = true;
			IL_0031:
			for (int i = 0; i < args.Length; i++)
			{
				if (!(args[i] == ""))
				{
					string text = args[i].ToLowerInvariant();
					if (text != null)
					{
						if (text == "on")
						{
							TIHistoricalData.RecordDebugData = true;
							goto IL_0099;
						}
						if (text == "off")
						{
							TIHistoricalData.RecordDebugData = false;
							goto IL_0099;
						}
					}
					this.terminalController.OutputError("\"" + args[i] + "\" is invalid. Valid arguments are \"on\" or \"off\"");
				}
				IL_0099:;
			}
		}

		// Token: 0x060058D0 RID: 22736 RVA: 0x0028B340 File Offset: 0x00289540
		private void ExtendedFleetUIActivation(string[] args)
		{
			if (args.Length != 0)
			{
				if (!args.All<string>((string x) => x == ""))
				{
					goto IL_0031;
				}
			}
			SpaceObjectDetailController.DisplayExtendedFleetUI = true;
			IL_0031:
			for (int i = 0; i < args.Length; i++)
			{
				if (!(args[i] == ""))
				{
					string text = args[i].ToLowerInvariant();
					if (text != null)
					{
						if (text == "on")
						{
							SpaceObjectDetailController.DisplayExtendedFleetUI = true;
							goto IL_0099;
						}
						if (text == "off")
						{
							SpaceObjectDetailController.DisplayExtendedFleetUI = false;
							goto IL_0099;
						}
					}
					this.terminalController.OutputError("\"" + args[i] + "\" is invalid. Valid arguments are \"on\" or \"off\"");
				}
				IL_0099:;
			}
		}

		// Token: 0x060058D1 RID: 22737 RVA: 0x0028B3F0 File Offset: 0x002895F0
		private void ResetGogAchievements(string[] args)
		{
		}

		// Token: 0x0400406F RID: 16495
		private TerminalController terminalController;

		// Token: 0x04004070 RID: 16496
		private Autopilot autopilot;
	}
}
