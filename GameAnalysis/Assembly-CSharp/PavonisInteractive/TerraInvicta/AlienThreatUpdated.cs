using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200063C RID: 1596
	public class AlienThreatUpdated : GameEvent
	{
		// Token: 0x06002861 RID: 10337 RVA: 0x000DA2F4 File Offset: 0x000D84F4
		public AlienThreatUpdated(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E82 RID: 7810
		public TIFactionState faction;
	}
}
