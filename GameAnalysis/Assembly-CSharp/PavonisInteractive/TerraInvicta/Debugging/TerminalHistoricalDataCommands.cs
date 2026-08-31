using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x02000914 RID: 2324
	public class TerminalHistoricalDataCommands
	{
		// Token: 0x060058DF RID: 22751 RVA: 0x0028C13D File Offset: 0x0028A33D
		public TerminalHistoricalDataCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x060058E0 RID: 22752 RVA: 0x0028C154 File Offset: 0x0028A354
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("ExportFactionHD", new CommandHandler(this.ExportHistoricalFactionData), "Exports CSV of all Historical Faction Data.");
			this.terminalController.RegisterCommand("ExportFactionStrengthHD", new CommandHandler(this.ExportHistoricalFactionStrengthData), "Exports CSV of all Historical Faction Strength Data.");
		}

		// Token: 0x060058E1 RID: 22753 RVA: 0x0028C1A3 File Offset: 0x0028A3A3
		private void ExportHistoricalFactionData(string[] args)
		{
			TIHistoricalData.ExportFactionCSV(null);
		}

		// Token: 0x060058E2 RID: 22754 RVA: 0x0028C1AB File Offset: 0x0028A3AB
		private void ExportHistoricalFactionStrengthData(string[] args)
		{
			TIHistoricalData.ExportFactionCSV(new HashSet<string> { "Strength" });
		}

		// Token: 0x04004072 RID: 16498
		private TerminalController terminalController;
	}
}
