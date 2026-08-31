using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006E0 RID: 1760
	public class ShipPadlockStateChanged : GameEvent
	{
		// Token: 0x0600290C RID: 10508 RVA: 0x000DAFB3 File Offset: 0x000D91B3
		public ShipPadlockStateChanged(bool padlockEnabled)
		{
			this.padlockEnabled = padlockEnabled;
		}

		// Token: 0x04001F70 RID: 8048
		public bool padlockEnabled;
	}
}
