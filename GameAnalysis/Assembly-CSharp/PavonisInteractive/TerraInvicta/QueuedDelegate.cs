using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E3 RID: 1763
	public struct QueuedDelegate
	{
		// Token: 0x04001F73 RID: 8051
		public Delegate del;

		// Token: 0x04001F74 RID: 8052
		public GameEvent evt;

		// Token: 0x04001F75 RID: 8053
		public object[] sourceObjects;

		// Token: 0x04001F76 RID: 8054
		public string eventName;
	}
}
