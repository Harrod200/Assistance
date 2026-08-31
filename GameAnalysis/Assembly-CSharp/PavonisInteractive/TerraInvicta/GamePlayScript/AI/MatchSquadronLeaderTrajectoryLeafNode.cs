using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A2C RID: 2604
	public class MatchSquadronLeaderTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006477 RID: 25719 RVA: 0x002F675D File Offset: 0x002F495D
		protected MatchSquadronLeaderTrajectoryLeafNode()
		{
		}

		// Token: 0x06006478 RID: 25720 RVA: 0x002F6765 File Offset: 0x002F4965
		public MatchSquadronLeaderTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006479 RID: 25721 RVA: 0x002F677C File Offset: 0x002F497C
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.CurrentTime <= this.trajectoryValidUntilTime)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			bool flag;
			this._sharedData.ShipController._waypointNavigationController.MatchRelativeTrajectory(this._localData.SquadronController.SquadLeader._waypointNavigationController, this._localData.SquadronController.ManeuverConstraints, out flag);
			this._localData.SquadronController.UpdateTrajectoryMatchedShips(this._sharedData.ShipController, flag);
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046D2 RID: 18130
		private TIDateTime trajectoryValidUntilTime;
	}
}
