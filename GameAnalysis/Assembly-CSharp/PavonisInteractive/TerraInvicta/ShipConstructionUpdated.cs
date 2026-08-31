using System;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020005EE RID: 1518
	public class ShipConstructionUpdated : GameEvent
	{
		// Token: 0x06002813 RID: 10259 RVA: 0x000D9D47 File Offset: 0x000D7F47
		public ShipConstructionUpdated(TIFactionState faction, TIHabModuleState shipyard, ShipConstructionQueueItem queueItem)
		{
			this.faction = faction;
			this.shipyard = shipyard;
			this.queueItem = queueItem;
		}

		// Token: 0x04001E11 RID: 7697
		public TIFactionState faction;

		// Token: 0x04001E12 RID: 7698
		public TIHabModuleState shipyard;

		// Token: 0x04001E13 RID: 7699
		public ShipConstructionQueueItem queueItem;
	}
}
