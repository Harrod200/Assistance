using System;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x0200091B RID: 2331
	public class TerminalNarrativeEventsCommands
	{
		// Token: 0x0600592A RID: 22826 RVA: 0x0028EF8C File Offset: 0x0028D18C
		public TerminalNarrativeEventsCommands(TerminalController terminalController)
		{
			this.terminalController = terminalController;
			this.RegisterCommands();
		}

		// Token: 0x0600592B RID: 22827 RVA: 0x0028EFA1 File Offset: 0x0028D1A1
		private void RegisterCommands()
		{
			this.terminalController.RegisterCommand("triggerevent", new CommandHandler(this.TriggerEvent), "Trigger Narrative Event. Force Event: 'triggerevent dataName' - Don't Force Event: 'triggerevent dataName, nf'");
		}

		// Token: 0x0600592C RID: 22828 RVA: 0x0028EFC4 File Offset: 0x0028D1C4
		public void TriggerEvent(string[] args)
		{
			if (args.Length < 1)
			{
				this.terminalController.OutputError("triggernarrativeevent Error: Missing event name");
				return;
			}
			TINarrativeEventTemplate tinarrativeEventTemplate = TemplateManager.Find<TINarrativeEventTemplate>(args[0], false);
			if (tinarrativeEventTemplate == null)
			{
				tinarrativeEventTemplate = TemplateManager.Find<TINarrativeEventTemplate>("event_" + args[0], false);
			}
			bool flag = args.Length == 2 && args[1] == "nf";
			if (tinarrativeEventTemplate != null)
			{
				GameStateManager.GlobalValues().TriggerNarrativeEvent(tinarrativeEventTemplate, GameControl.control.activePlayer, !flag);
				return;
			}
			this.terminalController.OutputError("triggernarrativeevent Error: Couldn't parse event dataName");
		}

		// Token: 0x04004078 RID: 16504
		private TerminalController terminalController;
	}
}
