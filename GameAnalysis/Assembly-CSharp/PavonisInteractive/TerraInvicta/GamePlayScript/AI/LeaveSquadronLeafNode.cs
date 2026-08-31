using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A36 RID: 2614
	public class LeaveSquadronLeafNode : LeafNode
	{
		// Token: 0x06006499 RID: 25753 RVA: 0x002F7BB3 File Offset: 0x002F5DB3
		protected LeaveSquadronLeafNode()
		{
		}

		// Token: 0x0600649A RID: 25754 RVA: 0x002F7BBB File Offset: 0x002F5DBB
		public LeaveSquadronLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x0600649B RID: 25755 RVA: 0x002F7BC5 File Offset: 0x002F5DC5
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.SquadronController != null)
			{
				this._localData.SquadronController.RemoveShipFromSquadron(this._sharedData.ShipController.ShipState);
				this._localData.SquadronController = null;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
