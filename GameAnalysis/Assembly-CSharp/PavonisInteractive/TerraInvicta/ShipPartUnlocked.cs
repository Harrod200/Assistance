using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005F1 RID: 1521
	public class ShipPartUnlocked : GameEvent
	{
		// Token: 0x06002816 RID: 10262 RVA: 0x000D9D82 File Offset: 0x000D7F82
		public ShipPartUnlocked(TIFactionState faction)
		{
			this.faction = faction;
		}

		// Token: 0x04001E16 RID: 7702
		public TIFactionState faction;
	}
}
