using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A2A RID: 2602
	public class UpdateDefensiveTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006471 RID: 25713 RVA: 0x002F6602 File Offset: 0x002F4802
		protected UpdateDefensiveTrajectoryLeafNode()
		{
		}

		// Token: 0x06006472 RID: 25714 RVA: 0x002F660A File Offset: 0x002F480A
		public UpdateDefensiveTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006473 RID: 25715 RVA: 0x002F6620 File Offset: 0x002F4820
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
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
			AccelerationConstraints dvconservingAccelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(false);
			if (!this._sharedData.ShipController.TryAssignDefensivePosition(allThreateningProjectiles, dvconservingAccelerationConstraints))
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046D0 RID: 18128
		private TIDateTime trajectoryValidUntilTime;
	}
}
