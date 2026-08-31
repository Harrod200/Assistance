using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A0F RID: 2575
	public class SequenceBranchNode : BranchNode
	{
		// Token: 0x06006412 RID: 25618 RVA: 0x002F405A File Offset: 0x002F225A
		protected SequenceBranchNode()
		{
		}

		// Token: 0x06006413 RID: 25619 RVA: 0x002F4062 File Offset: 0x002F2262
		public SequenceBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006414 RID: 25620 RVA: 0x002F4070 File Offset: 0x002F2270
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			ITreeNode[] childNodes = this._childNodes;
			for (int i = 0; i < childNodes.Length; i++)
			{
				if (childNodes[i].Execute() == CombatShipBehaviourTree.ConditionResponse.Failed)
				{
					return CombatShipBehaviourTree.ConditionResponse.Failed;
				}
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
