using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A1B RID: 2587
	public class InverseResultDecoratorNode : BranchNode
	{
		// Token: 0x06006438 RID: 25656 RVA: 0x002F4837 File Offset: 0x002F2A37
		protected InverseResultDecoratorNode()
		{
		}

		// Token: 0x06006439 RID: 25657 RVA: 0x002F483F File Offset: 0x002F2A3F
		public InverseResultDecoratorNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x0600643A RID: 25658 RVA: 0x002F484C File Offset: 0x002F2A4C
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._childNodes.Length < 1)
			{
				Debug.LogError("Too many children. Decorator Nodes can only have one child");
			}
			CombatShipBehaviourTree.ConditionResponse conditionResponse = this._childNodes[0].Execute();
			switch (conditionResponse)
			{
			case CombatShipBehaviourTree.ConditionResponse.Failed:
				return CombatShipBehaviourTree.ConditionResponse.Success;
			case CombatShipBehaviourTree.ConditionResponse.Running:
				return CombatShipBehaviourTree.ConditionResponse.Running;
			case CombatShipBehaviourTree.ConditionResponse.Success:
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			default:
				return conditionResponse;
			}
		}
	}
}
