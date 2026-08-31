using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005BC RID: 1468
	public class SavingGameEvent : GameEvent
	{
		// Token: 0x060027E1 RID: 10209 RVA: 0x000D9A12 File Offset: 0x000D7C12
		public SavingGameEvent(bool start)
		{
			this.start = start;
		}

		// Token: 0x04001DD5 RID: 7637
		public bool start;
	}
}
