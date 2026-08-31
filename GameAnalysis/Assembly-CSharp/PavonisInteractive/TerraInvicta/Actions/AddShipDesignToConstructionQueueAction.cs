using System;
using System.Collections.Generic;

namespace PavonisInteractive.TerraInvicta.Actions
{
	// Token: 0x02000A4A RID: 2634
	public class AddShipDesignToConstructionQueueAction : PlayerAction
	{
		// Token: 0x060064E0 RID: 25824 RVA: 0x002FA2C8 File Offset: 0x002F84C8
		public AddShipDesignToConstructionQueueAction(TIHabModuleState shipyard, TISpaceShipTemplate ship, bool allowPayFromEarth, float resourceFraction, FactionGoal_Fleet goal, bool isRefit = false, TISpaceShipTemplate originalShipDesign = null, TISpaceShipState originalShipState = null)
		{
			this.shipyard = shipyard;
			this.ship = ship;
			this.allowPayFromEarth = allowPayFromEarth;
			this.resourceFraction = resourceFraction;
			this.goal = goal;
			this.isRefit = isRefit;
			this.originalShipDesign = originalShipDesign;
			this.originalShipState = originalShipState;
		}

		// Token: 0x060064E1 RID: 25825 RVA: 0x002FA318 File Offset: 0x002F8518
		public override void Execute()
		{
			this.shipyard.sector.faction.AddShipToShipyardQueue(this.shipyard, this.ship, this.allowPayFromEarth, this.resourceFraction, this.goal, this.isRefit, this.originalShipDesign, this.originalShipState);
			if (this.isRefit)
			{
				this.originalShipState.councilorPassengers.ForEach(delegate(TICouncilorState x)
				{
					x.SetLocation(this.shipyard.hab);
				});
				this.originalShipState.fleet.RemoveShipsFromFleet(new List<TISpaceShipState> { this.originalShipState }, null);
			}
			this.shipyard.sector.faction.CompleteMilestone(CampaignMilestone.TutorialBuildShip);
		}

		// Token: 0x040046F4 RID: 18164
		private TIHabModuleState shipyard;

		// Token: 0x040046F5 RID: 18165
		private TISpaceShipTemplate ship;

		// Token: 0x040046F6 RID: 18166
		private bool allowPayFromEarth;

		// Token: 0x040046F7 RID: 18167
		private float resourceFraction;

		// Token: 0x040046F8 RID: 18168
		private FactionGoal_Fleet goal;

		// Token: 0x040046F9 RID: 18169
		private bool isRefit;

		// Token: 0x040046FA RID: 18170
		public TISpaceShipTemplate originalShipDesign;

		// Token: 0x040046FB RID: 18171
		public TISpaceShipState originalShipState;
	}
}
