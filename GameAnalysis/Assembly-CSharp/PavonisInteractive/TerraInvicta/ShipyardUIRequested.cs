using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005D8 RID: 1496
	public class ShipyardUIRequested : GameEvent
	{
		// Token: 0x060027FD RID: 10237 RVA: 0x000D9BE7 File Offset: 0x000D7DE7
		public ShipyardUIRequested(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001DF8 RID: 7672
		public TIFactionState faction;
	}
}
