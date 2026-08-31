using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A11 RID: 2577
	public class SelectRunningOrSuccessBranchNode : BranchNode
	{
		// Token: 0x06006418 RID: 25624 RVA: 0x002F40E8 File Offset: 0x002F22E8
		protected SelectRunningOrSuccessBranchNode()
		{
		}

		// Token: 0x06006419 RID: 25625 RVA: 0x002F40F0 File Offset: 0x002F22F0
		public SelectRunningOrSuccessBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x0600641A RID: 25626 RVA: 0x002F40FC File Offset: 0x002F22FC
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			CombatShipBehaviourTree.ConditionResponse conditionResponse = CombatShipBehaviourTree.ConditionResponse.Failed;
			ITreeNode[] childNodes = this._childNodes;
			for (int i = 0; i < childNodes.Length; i++)
			{
				conditionResponse = childNodes[i].Execute();
				if (conditionResponse - CombatShipBehaviourTree.ConditionResponse.Running <= 1)
				{
					return CombatShipBehaviourTree.ConditionResponse.Success;
				}
			}
			return conditionResponse;
		}
	}
}
