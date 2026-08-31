using System;

namespace PavonisInteractive.TerraInvicta.Debugging
{
	// Token: 0x0200090F RID: 2319
	public struct CommandRegistration
	{
		// Token: 0x060058AB RID: 22699 RVA: 0x00289E6A File Offset: 0x0028806A
		public CommandRegistration(string command, CommandHandler handler, string help)
		{
			this.command = command;
			this.handler = handler;
			this.help = help;
		}

		// Token: 0x04004067 RID: 16487
		public string command;

		// Token: 0x04004068 RID: 16488
		public CommandHandler handler;

		// Token: 0x04004069 RID: 16489
		public string help;
	}
}
