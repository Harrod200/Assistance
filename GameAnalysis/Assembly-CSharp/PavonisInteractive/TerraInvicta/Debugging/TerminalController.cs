using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000910 RID: 2320
	public class TerminalController
	{
		// Token: 0x14000024 RID: 36
		// (add) Token: 0x060058AC RID: 22700 RVA: 0x00289E84 File Offset: 0x00288084
		// (remove) Token: 0x060058AD RID: 22701 RVA: 0x00289EBC File Offset: 0x002880BC
		public event Action<string> OnOutputError = delegate
		{
		};

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x060058AE RID: 22702 RVA: 0x00289EF4 File Offset: 0x002880F4
		// (remove) Token: 0x060058AF RID: 22703 RVA: 0x00289F2C File Offset: 0x0028812C
		public event Action<string> OnOutput = delegate
		{
		};

		// Token: 0x060058B0 RID: 22704 RVA: 0x00289F64 File Offset: 0x00288164
		public TerminalController()
		{
			this.builder = new StringBuilder();
			this.RegisterCommand("help", new CommandHandler(this.Help), "Prints commands");
		}

		// Token: 0x060058B1 RID: 22705 RVA: 0x00289FF3 File Offset: 0x002881F3
		public void RegisterCommand(string command, CommandHandler handler, string help)
		{
			this.commands.Add(command, new CommandRegistration(command, handler, help));
		}

		// Token: 0x060058B2 RID: 22706 RVA: 0x0028A00C File Offset: 0x0028820C
		public void ParseCommand(string commandString)
		{
			foreach (KeyValuePair<string, CommandRegistration> keyValuePair in this.commands)
			{
				int num = commandString.IndexOf(keyValuePair.Key, StringComparison.OrdinalIgnoreCase);
				if (num >= 0)
				{
					int num2 = num + keyValuePair.Key.Length;
					string text = commandString.Substring(num2);
					this.ProcessArguments(keyValuePair.Value, text);
					return;
				}
			}
			this.OutputError(string.Format("Unknown command '{0}', type 'help' for list.", commandString));
		}

		// Token: 0x060058B3 RID: 22707 RVA: 0x0028A0A8 File Offset: 0x002882A8
		private void ProcessArguments(CommandRegistration cmd, string argString)
		{
			string[] array = (from p in argString.Split(new char[] { ',' })
				select p.Trim()).ToArray<string>();
			if (array.Length == 0)
			{
				return;
			}
			cmd.handler(array);
		}

		// Token: 0x060058B4 RID: 22708 RVA: 0x0028A104 File Offset: 0x00288304
		private void Help(string[] args)
		{
			this.builder.Clear();
			foreach (KeyValuePair<string, CommandRegistration> keyValuePair in this.commands)
			{
				this.builder.AppendLine(keyValuePair.Value.command.PadRight(10) + "\t\t" + keyValuePair.Value.help);
			}
			this.Output(this.builder.ToString());
		}

		// Token: 0x060058B5 RID: 22709 RVA: 0x0028A1A4 File Offset: 0x002883A4
		public void OutputError(string line)
		{
			this.OnOutputError(line);
		}

		// Token: 0x060058B6 RID: 22710 RVA: 0x0028A1B2 File Offset: 0x002883B2
		public void Output(string line)
		{
			this.OnOutput(line);
		}

		// Token: 0x060058B7 RID: 22711 RVA: 0x0028A1C0 File Offset: 0x002883C0
		public void Destroy()
		{
			this.commands = null;
		}

		// Token: 0x0400406C RID: 16492
		private Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

		// Token: 0x0400406D RID: 16493
		private StringBuilder builder;
	}
}
