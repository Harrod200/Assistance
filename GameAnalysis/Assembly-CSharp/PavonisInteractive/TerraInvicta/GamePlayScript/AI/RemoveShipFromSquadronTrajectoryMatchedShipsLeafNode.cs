using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A2D RID: 2605
	public class RemoveShipFromSquadronTrajectoryMatchedShipsLeafNode : LeafNode
	{
		// Token: 0x0600647A RID: 25722 RVA: 0x002F6817 File Offset: 0x002F4A17
		protected RemoveShipFromSquadronTrajectoryMatchedShipsLeafNode()
		{
		}

		// Token: 0x0600647B RID: 25723 RVA: 0x002F681F File Offset: 0x002F4A1F
		public RemoveShipFromSquadronTrajectoryMatchedShipsLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x0600647C RID: 25724 RVA: 0x002F6829 File Offset: 0x002F4A29
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.SquadronController.ShipIsTrajectoryMatched(this._sharedData.ShipController))
			{
				this._localData.SquadronController.UpdateTrajectoryMatchedShips(this._sharedData.ShipController, false);
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
