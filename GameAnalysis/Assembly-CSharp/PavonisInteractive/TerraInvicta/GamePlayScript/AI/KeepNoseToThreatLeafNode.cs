using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A2E RID: 2606
	public class KeepNoseToThreatLeafNode : LeafNode
	{
		// Token: 0x0600647D RID: 25725 RVA: 0x002F6865 File Offset: 0x002F4A65
		protected KeepNoseToThreatLeafNode()
		{
		}

		// Token: 0x0600647E RID: 25726 RVA: 0x002F686D File Offset: 0x002F4A6D
		public KeepNoseToThreatLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x0600647F RID: 25727 RVA: 0x002F6884 File Offset: 0x002F4A84
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps() < this._sharedData.MinimumDVThreshold)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			if (this._sharedData.CurrentTime <= this.trajectoryValidUntilTime)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			if (this._sharedData.ShipController.InCollisionAvoidanceManeuver)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			List<ProjectileController> allThreateningProjectiles = this._sharedData.ShipController.GetAllThreateningProjectiles();
			this._sharedData.ShipController.FilterForImminentImpactThreats(ref allThreateningProjectiles);
			if (!this._sharedData.ShipController.TryToKeepNoseTowardsThreat(allThreateningProjectiles))
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046D3 RID: 18131
		private TIDateTime trajectoryValidUntilTime;
	}
}
