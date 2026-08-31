using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A25 RID: 2597
	public class UpdateInterceptCourseTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006460 RID: 25696 RVA: 0x002F5E74 File Offset: 0x002F4074
		protected UpdateInterceptCourseTrajectoryLeafNode()
		{
		}

		// Token: 0x06006461 RID: 25697 RVA: 0x002F5E7C File Offset: 0x002F407C
		public UpdateInterceptCourseTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006462 RID: 25698 RVA: 0x002F5E94 File Offset: 0x002F4094
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
			this._sharedData.ShipController._waypointNavigationController.InterceptCourse(accelerationConstraints);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046CB RID: 18123
		private TIDateTime trajectoryValidUntilTime;
	}
}
