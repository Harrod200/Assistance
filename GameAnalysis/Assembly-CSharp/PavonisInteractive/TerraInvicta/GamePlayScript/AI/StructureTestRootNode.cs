using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A45 RID: 2629
	public class StructureTestRootNode : RootNode
	{
		// Token: 0x060064CA RID: 25802 RVA: 0x002F8BE1 File Offset: 0x002F6DE1
		protected StructureTestRootNode()
		{
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x002F8BE9 File Offset: 0x002F6DE9
		public StructureTestRootNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x060064CC RID: 25804 RVA: 0x002F8BF4 File Offset: 0x002F6DF4
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (!this._sharedData.ShipController.isDestroyed && !this._sharedData.ShipController.destructionTriggered && this._sharedData.ShipController.ShipState.combatAIControl)
			{
				for (int i = 0; i < this._childNodes.Length; i++)
				{
					if (this._childNodes[i].Execute() == CombatShipBehaviourTree.ConditionResponse.Failed)
					{
						return CombatShipBehaviourTree.ConditionResponse.Failed;
					}
				}
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}
	}
}
