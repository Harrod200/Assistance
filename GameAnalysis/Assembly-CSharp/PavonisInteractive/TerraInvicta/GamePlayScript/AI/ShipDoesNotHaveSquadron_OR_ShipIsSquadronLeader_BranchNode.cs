using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A14 RID: 2580
	public class ShipDoesNotHaveSquadron_OR_ShipIsSquadronLeader_BranchNode : BranchNode
	{
		// Token: 0x06006422 RID: 25634 RVA: 0x002F4450 File Offset: 0x002F2650
		protected ShipDoesNotHaveSquadron_OR_ShipIsSquadronLeader_BranchNode()
		{
		}

		// Token: 0x06006423 RID: 25635 RVA: 0x002F4458 File Offset: 0x002F2658
		public ShipDoesNotHaveSquadron_OR_ShipIsSquadronLeader_BranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006424 RID: 25636 RVA: 0x002F4464 File Offset: 0x002F2664
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.SquadronController == null || (this._localData.SquadronController != null && this._localData.SquadronController.SquadLeader == this._sharedData.ShipController))
			{
				ITreeNode[] childNodes = this._childNodes;
				for (int i = 0; i < childNodes.Length; i++)
				{
					childNodes[i].Execute();
				}
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}
	}
}
