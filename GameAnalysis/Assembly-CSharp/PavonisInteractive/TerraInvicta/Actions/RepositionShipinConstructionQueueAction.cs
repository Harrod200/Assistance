using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A7E RID: 2686
	public class RepositionShipinConstructionQueueAction : PlayerAction
	{
		// Token: 0x06006550 RID: 25936 RVA: 0x002FC366 File Offset: 0x002FA566
		public RepositionShipinConstructionQueueAction(TIHabModuleState shipyard, ShipConstructionQueueItem item, int newIndex)
		{
			this.shipyard = shipyard;
			this.item = item;
			this.newIndex = newIndex;
		}

		// Token: 0x06006551 RID: 25937 RVA: 0x002FC383 File Offset: 0x002FA583
		public override void Execute()
		{
			this.shipyard.sector.faction.RepositionShipinShipyardQueue(this.shipyard, this.item, this.newIndex);
		}

		// Token: 0x0400477C RID: 18300
		private TIHabModuleState shipyard;

		// Token: 0x0400477D RID: 18301
		private ShipConstructionQueueItem item;

		// Token: 0x0400477E RID: 18302
		private int newIndex;
	}
}
