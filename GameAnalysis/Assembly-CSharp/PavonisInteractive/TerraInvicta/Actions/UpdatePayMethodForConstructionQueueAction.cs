using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA5 RID: 2725
	public class UpdatePayMethodForConstructionQueueAction : PlayerAction
	{
		// Token: 0x060065A3 RID: 26019 RVA: 0x002FD820 File Offset: 0x002FBA20
		public UpdatePayMethodForConstructionQueueAction(TIHabModuleState shipyard, bool allowPayFromEarth)
		{
			this.shipyard = shipyard;
			this.allowPayFromEarth = allowPayFromEarth;
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x002FD838 File Offset: 0x002FBA38
		public override void Execute()
		{
			TIFactionState faction = this.shipyard.sector.faction;
			List<ShipConstructionQueueItem> shipyardQueue = faction.GetShipyardQueue(this.shipyard);
			this.shipyard.shipyardAllowPayFromEarth = this.allowPayFromEarth;
			for (int i = 0; i < shipyardQueue.Count; i++)
			{
				if (!shipyardQueue[i].costPaid)
				{
					TIResourcesCost tiresourcesCost = (shipyardQueue[i].isRefit ? shipyardQueue[i].shipDesign.RefitResourceCost(this.shipyard, shipyardQueue[i].refit_originalShipDesign, true, true, shipyardQueue[i].originalSpaceShipState) : shipyardQueue[i].shipDesign.spaceResourceConstructionCost(false, this.shipyard, true, false, false));
					shipyardQueue[i].UpdateResourcesCost(this.allowPayFromEarth ? TISpaceShipTemplate.MixedResourceConstructionCost(faction, this.shipyard.hab, tiresourcesCost, faction.AvailableSpaceResources(1f), false) : tiresourcesCost);
				}
			}
			if (shipyardQueue.Count > 0 && !shipyardQueue[0].costPaid && this.allowPayFromEarth)
			{
				faction.AttemptInitiateShipConstruction(this.shipyard);
			}
		}

		// Token: 0x040047F4 RID: 18420
		private TIHabModuleState shipyard;

		// Token: 0x040047F5 RID: 18421
		private bool allowPayFromEarth;
	}
}
