using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A2B RID: 2603
	public class FollowSquadronLeaderTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006474 RID: 25716 RVA: 0x002F66C2 File Offset: 0x002F48C2
		protected FollowSquadronLeaderTrajectoryLeafNode()
		{
		}

		// Token: 0x06006475 RID: 25717 RVA: 0x002F66CA File Offset: 0x002F48CA
		public FollowSquadronLeaderTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006476 RID: 25718 RVA: 0x002F66E0 File Offset: 0x002F48E0
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.CurrentTime <= this.trajectoryValidUntilTime)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			this._sharedData.ShipController._waypointNavigationController.FollowControllerTrajectory(this._localData.SquadronController.SquadLeader._waypointNavigationController, this._localData.SquadronController.ManeuverConstraints);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046D1 RID: 18129
		private TIDateTime trajectoryValidUntilTime;
	}
}
