using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D7 RID: 1495
	public class SellSpaceResourcesRequested : GameEvent
	{
		// Token: 0x060027FC RID: 10236 RVA: 0x000D9BD8 File Offset: 0x000D7DD8
		public SellSpaceResourcesRequested(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001DF7 RID: 7671
		public TIFactionState faction;
	}
}
