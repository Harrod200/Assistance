using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A27 RID: 2599
	public class UpdateFullSpeedAheadTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006466 RID: 25702 RVA: 0x002F607E File Offset: 0x002F427E
		protected UpdateFullSpeedAheadTrajectoryLeafNode()
		{
		}

		// Token: 0x06006467 RID: 25703 RVA: 0x002F6086 File Offset: 0x002F4286
		public UpdateFullSpeedAheadTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006468 RID: 25704 RVA: 0x002F609C File Offset: 0x002F429C
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
			this._sharedData.ShipController._waypointNavigationController.FullSpeedAhead(accelerationConstraints);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046CD RID: 18125
		private TIDateTime trajectoryValidUntilTime;
	}
}
