using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A1A RID: 2586
	public class ShipManeuverabilityIsSignificantlyDimishedBranchNode : BranchNode
	{
		// Token: 0x06006435 RID: 25653 RVA: 0x002F47D2 File Offset: 0x002F29D2
		protected ShipManeuverabilityIsSignificantlyDimishedBranchNode()
		{
		}

		// Token: 0x06006436 RID: 25654 RVA: 0x002F47DA File Offset: 0x002F29DA
		public ShipManeuverabilityIsSignificantlyDimishedBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006437 RID: 25655 RVA: 0x002F47E8 File Offset: 0x002F29E8
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if ((double)this._sharedData.ShipController.ShipState.ManeuverEffectivenessRatio < 0.5)
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
