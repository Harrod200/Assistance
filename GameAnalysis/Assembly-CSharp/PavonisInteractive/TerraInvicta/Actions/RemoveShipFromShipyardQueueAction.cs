using System;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A7C RID: 2684
	public class RemoveShipFromShipyardQueueAction : PlayerAction
	{
		// Token: 0x0600654C RID: 25932 RVA: 0x002FC2B9 File Offset: 0x002FA4B9
		public RemoveShipFromShipyardQueueAction(TIHabModuleState shipyard, ShipConstructionQueueItem item)
		{
			this.shipyard = shipyard;
			this.item = item;
		}

		// Token: 0x0600654D RID: 25933 RVA: 0x002FC2D0 File Offset: 0x002FA4D0
		public override void Execute()
		{
			if (this.item.isRefit)
			{
				this.shipyard.ref_faction.CompleteShipConstruction(this.shipyard, true, this.item);
			}
			this.shipyard.sector.faction.RemoveShipFromShipyardQueue(this.shipyard, this.item);
		}

		// Token: 0x04004778 RID: 18296
		private TIHabModuleState shipyard;

		// Token: 0x04004779 RID: 18297
		private ShipConstructionQueueItem item;
	}
}
