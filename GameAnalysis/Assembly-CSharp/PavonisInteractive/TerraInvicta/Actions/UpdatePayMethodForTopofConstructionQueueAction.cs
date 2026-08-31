using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000AA6 RID: 2726
	public class UpdatePayMethodForTopofConstructionQueueAction : PlayerAction
	{
		// Token: 0x060065A5 RID: 26021 RVA: 0x002FD95A File Offset: 0x002FBB5A
		public UpdatePayMethodForTopofConstructionQueueAction(TIHabModuleState shipyard, TIFactionGoalState goal)
		{
			this.shipyard = shipyard;
			this.goal = goal;
		}

		// Token: 0x060065A6 RID: 26022 RVA: 0x002FD970 File Offset: 0x002FBB70
		public override void Execute()
		{
			TIFactionState faction = this.shipyard.sector.faction;
			List<ShipConstructionQueueItem> shipyardQueue = faction.GetShipyardQueue(this.shipyard);
			if (shipyardQueue.Count > 0 && !shipyardQueue[0].costPaid)
			{
				TIResourcesCost tiresourcesCost = (shipyardQueue[0].isRefit ? shipyardQueue[0].shipDesign.RefitResourceCost(this.shipyard, shipyardQueue[0].refit_originalShipDesign, true, true, shipyardQueue[0].originalSpaceShipState) : shipyardQueue[0].shipDesign.spaceResourceConstructionCost(false, this.shipyard, true, false, false));
				shipyardQueue[0].UpdateResourcesCost(TISpaceShipTemplate.MixedResourceConstructionCost(faction, this.shipyard.hab, tiresourcesCost, faction.AvailableSpaceResources(faction.player.isAI ? AIEvaluators.SpaceResourcesForShipBuild(this.goal) : 1f), false));
				faction.AttemptInitiateShipConstruction(this.shipyard);
			}
		}

		// Token: 0x040047F6 RID: 18422
		private TIHabModuleState shipyard;

		// Token: 0x040047F7 RID: 18423
		private TIFactionGoalState goal;
	}
}
