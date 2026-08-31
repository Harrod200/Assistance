using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A28 RID: 2600
	public class SetBrakingTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006469 RID: 25705 RVA: 0x002F6182 File Offset: 0x002F4382
		protected SetBrakingTrajectoryLeafNode()
		{
		}

		// Token: 0x0600646A RID: 25706 RVA: 0x002F618A File Offset: 0x002F438A
		public SetBrakingTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x0600646B RID: 25707 RVA: 0x002F61A0 File Offset: 0x002F43A0
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
			this._sharedData.ShipController._waypointNavigationController.SetBreakingTrajectory(accelerationConstraints);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046CE RID: 18126
		private TIDateTime trajectoryValidUntilTime;
	}
}
