using System;
using UnityEngine;
using Zenject;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x0200090D RID: 2317
	public class TerminalButtonListener : IInitializable, ITickable
	{
		// Token: 0x060058A4 RID: 22692 RVA: 0x00289D9F File Offset: 0x00287F9F
		[Inject]
		public TerminalButtonListener(Terminal terminal)
		{
			this.terminal = terminal;
		}

		// Token: 0x060058A5 RID: 22693 RVA: 0x00289DAE File Offset: 0x00287FAE
		public void Initialize()
		{
			this.terminal.Hide();
		}

		// Token: 0x060058A6 RID: 22694 RVA: 0x00289DBC File Offset: 0x00287FBC
		public void Tick()
		{
			if (TemplateManager.global.debug_ConsoleActive && (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Tilde) || (TIGlobalConfig.globalConfig.terminalKeyCode != -1 && Input.GetKeyDown((KeyCode)TIGlobalConfig.globalConfig.terminalKeyCode))))
			{
				this.isShowingConsole = !this.isShowingConsole;
				if (this.isShowingConsole)
				{
					this.terminal.Show();
				}
				else
				{
					this.terminal.Hide();
				}
			}
			if (this.isShowingConsole)
			{
				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					this.terminal.PreviousCommand();
				}
				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					this.terminal.NextCommand();
				}
			}
		}

		// Token: 0x04004065 RID: 16485
		private Terminal terminal;

		// Token: 0x04004066 RID: 16486
		private bool isShowingConsole;
	}
}
