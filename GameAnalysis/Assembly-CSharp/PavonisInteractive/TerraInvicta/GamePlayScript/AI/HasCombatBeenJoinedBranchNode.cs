using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A17 RID: 2583
	public class HasCombatBeenJoinedBranchNode : BranchNode
	{
		// Token: 0x0600642B RID: 25643 RVA: 0x002F459F File Offset: 0x002F279F
		protected HasCombatBeenJoinedBranchNode()
		{
		}

		// Token: 0x0600642C RID: 25644 RVA: 0x002F45A7 File Offset: 0x002F27A7
		public HasCombatBeenJoinedBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x0600642D RID: 25645 RVA: 0x002F45B4 File Offset: 0x002F27B4
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.FleetController.HasFleetFiredThisCombat || this._sharedData.OpposingFleetController.HasFleetFiredThisCombat || GameControl.spaceCombat.setup == CombatSetup.Fleet0ChaseFleet1 || GameControl.spaceCombat.setup == CombatSetup.Fleet1ChaseFleet0)
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
