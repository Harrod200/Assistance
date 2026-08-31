using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A10 RID: 2576
	public class SelectBranchNode : BranchNode
	{
		// Token: 0x06006415 RID: 25621 RVA: 0x002F409F File Offset: 0x002F229F
		protected SelectBranchNode()
		{
		}

		// Token: 0x06006416 RID: 25622 RVA: 0x002F40A7 File Offset: 0x002F22A7
		public SelectBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006417 RID: 25623 RVA: 0x002F40B4 File Offset: 0x002F22B4
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			CombatShipBehaviourTree.ConditionResponse conditionResponse = CombatShipBehaviourTree.ConditionResponse.Failed;
			ITreeNode[] childNodes = this._childNodes;
			for (int i = 0; i < childNodes.Length; i++)
			{
				conditionResponse = childNodes[i].Execute();
				if (conditionResponse == CombatShipBehaviourTree.ConditionResponse.Success)
				{
					return CombatShipBehaviourTree.ConditionResponse.Success;
				}
			}
			return conditionResponse;
		}
	}
}
