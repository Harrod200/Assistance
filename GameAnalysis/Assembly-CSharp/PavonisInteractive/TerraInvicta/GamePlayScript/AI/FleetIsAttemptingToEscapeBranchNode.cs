using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A18 RID: 2584
	public class FleetIsAttemptingToEscapeBranchNode : BranchNode
	{
		// Token: 0x0600642E RID: 25646 RVA: 0x002F4620 File Offset: 0x002F2820
		protected FleetIsAttemptingToEscapeBranchNode()
		{
		}

		// Token: 0x0600642F RID: 25647 RVA: 0x002F4628 File Offset: 0x002F2828
		public FleetIsAttemptingToEscapeBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x002F4634 File Offset: 0x002F2834
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (GameControl.spaceCombat.combatState.stances[this._sharedData.FleetController.faction] == CombatStance.Evade)
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
