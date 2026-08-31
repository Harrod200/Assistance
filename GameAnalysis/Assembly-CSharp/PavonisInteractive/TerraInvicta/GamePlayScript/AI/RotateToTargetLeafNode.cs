using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A2F RID: 2607
	public class RotateToTargetLeafNode : LeafNode
	{
		// Token: 0x06006480 RID: 25728 RVA: 0x002F6937 File Offset: 0x002F4B37
		protected RotateToTargetLeafNode()
		{
		}

		// Token: 0x06006481 RID: 25729 RVA: 0x002F693F File Offset: 0x002F4B3F
		public RotateToTargetLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x002F6958 File Offset: 0x002F4B58
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
			if (this._sharedData.ShipController.ShipState.CanRotateAndRoll())
			{
				this._sharedData.ShipController._waypointNavigationController.RotateToFaceTarget();
				this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x040046D4 RID: 18132
		private TIDateTime trajectoryValidUntilTime;
	}
}
