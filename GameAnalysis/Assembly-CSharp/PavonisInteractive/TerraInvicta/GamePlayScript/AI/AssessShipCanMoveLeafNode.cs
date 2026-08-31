using System;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A39 RID: 2617
	public class AssessShipCanMoveLeafNode : LeafNode
	{
		// Token: 0x060064A2 RID: 25762 RVA: 0x002F8270 File Offset: 0x002F6470
		protected AssessShipCanMoveLeafNode()
		{
		}

		// Token: 0x060064A3 RID: 25763 RVA: 0x002F82A7 File Offset: 0x002F64A7
		public AssessShipCanMoveLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064A4 RID: 25764 RVA: 0x002F82E0 File Offset: 0x002F64E0
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.ShipController.ShipState.ThrustEffectivenessRatio <= 0f || this._sharedData.ShipController.InCollisionAvoidanceManeuver)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x040046E1 RID: 18145
		private IShipCommand disengageCommand = ShipCommandsManager.shipCommands.Find((IShipCommand x) => x is DisengageCommand);
	}
}
