using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A26 RID: 2598
	public class UpdateBurnAlongCurrentVelocityTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006463 RID: 25699 RVA: 0x002F5F7A File Offset: 0x002F417A
		protected UpdateBurnAlongCurrentVelocityTrajectoryLeafNode()
		{
		}

		// Token: 0x06006464 RID: 25700 RVA: 0x002F5F82 File Offset: 0x002F4182
		public UpdateBurnAlongCurrentVelocityTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006465 RID: 25701 RVA: 0x002F5F98 File Offset: 0x002F4198
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
			this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			this.trajectoryValidUntilTime.AddSeconds((double)this._localData.SecondsPerTrajectoryUpdate);
			if (this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps() < this._sharedData.MinimumDVThreshold)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			CombatSquadronController squadronController = this._localData.SquadronController;
			AccelerationConstraints accelerationConstraints;
			if (((squadronController != null) ? squadronController.ManeuverConstraints : null) != null)
			{
				accelerationConstraints = this._localData.SquadronController.ManeuverConstraints;
			}
			else
			{
				accelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(true);
			}
			this._sharedData.ShipController._waypointNavigationController.BurnAlongCurrentVelocity(accelerationConstraints);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046CC RID: 18124
		private TIDateTime trajectoryValidUntilTime;
	}
}
