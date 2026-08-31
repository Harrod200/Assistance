using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E4 RID: 1764
	public class GameEvent
	{
		// Token: 0x0600290F RID: 10511 RVA: 0x000DAFE0 File Offset: 0x000D91E0
		public override string ToString()
		{
			return base.GetType().ToString();
		}

		// Token: 0x04001F77 RID: 8055
		public List<QueuedDelegate> immediateQueue;
	}
}
