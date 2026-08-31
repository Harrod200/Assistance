using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005E9 RID: 1513
	public class FactionResourcesUpdated : GameEvent
	{
		// Token: 0x0600280E RID: 10254 RVA: 0x000D9CFC File Offset: 0x000D7EFC
		public FactionResourcesUpdated(TIFactionState council)
		{
			this.council = council;
		}

		// Token: 0x04001E0C RID: 7692
		public TIFactionState council;
	}
}
