using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A16 RID: 2582
	public class ShipHasMatchedSquadronLeaderBranchNode : BranchNode
	{
		// Token: 0x06006428 RID: 25640 RVA: 0x002F453E File Offset: 0x002F273E
		protected ShipHasMatchedSquadronLeaderBranchNode()
		{
		}

		// Token: 0x06006429 RID: 25641 RVA: 0x002F4546 File Offset: 0x002F2746
		public ShipHasMatchedSquadronLeaderBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x0600642A RID: 25642 RVA: 0x002F4554 File Offset: 0x002F2754
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.SquadronController.ShipIsTrajectoryMatched(this._sharedData.ShipController))
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
