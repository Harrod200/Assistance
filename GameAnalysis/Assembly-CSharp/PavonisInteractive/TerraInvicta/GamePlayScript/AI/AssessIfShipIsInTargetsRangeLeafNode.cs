using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A42 RID: 2626
	public class AssessIfShipIsInTargetsRangeLeafNode : LeafNode
	{
		// Token: 0x060064C1 RID: 25793 RVA: 0x002F89F3 File Offset: 0x002F6BF3
		protected AssessIfShipIsInTargetsRangeLeafNode()
		{
		}

		// Token: 0x060064C2 RID: 25794 RVA: 0x002F89FB File Offset: 0x002F6BFB
		public AssessIfShipIsInTargetsRangeLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064C3 RID: 25795 RVA: 0x002F8A08 File Offset: 0x002F6C08
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			CombatShipController shipController = this._sharedData.ShipController;
			Vector3 vector = default(Vector3);
			float num = 0f;
			if (this._localData.TargetShip != null)
			{
				num = this._localData.TargetShip.GetShipMaxScaledCombatRange();
				vector = shipController.position - this._localData.TargetShip.position;
			}
			else if (this._localData.TargetModule != null)
			{
				num = this._localData.TargetModule.GetHabModuleEffectiveScaledCombatRange();
				vector = shipController.position - this._localData.TargetModule.position;
			}
			if (vector.sqrMagnitude > num * num)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
