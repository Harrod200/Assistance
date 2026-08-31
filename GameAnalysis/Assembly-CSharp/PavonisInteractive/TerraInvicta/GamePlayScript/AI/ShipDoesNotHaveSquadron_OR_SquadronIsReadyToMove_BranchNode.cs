using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A15 RID: 2581
	public class ShipDoesNotHaveSquadron_OR_SquadronIsReadyToMove_BranchNode : BranchNode
	{
		// Token: 0x06006425 RID: 25637 RVA: 0x002F44CE File Offset: 0x002F26CE
		protected ShipDoesNotHaveSquadron_OR_SquadronIsReadyToMove_BranchNode()
		{
		}

		// Token: 0x06006426 RID: 25638 RVA: 0x002F44D6 File Offset: 0x002F26D6
		public ShipDoesNotHaveSquadron_OR_SquadronIsReadyToMove_BranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006427 RID: 25639 RVA: 0x002F44E4 File Offset: 0x002F26E4
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._localData.SquadronController == null || (this._localData.SquadronController != null && this._localData.SquadronController.SquadronReadyToManeuver))
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
