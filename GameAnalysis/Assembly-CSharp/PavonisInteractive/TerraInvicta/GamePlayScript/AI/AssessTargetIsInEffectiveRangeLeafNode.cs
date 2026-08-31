using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A40 RID: 2624
	public class AssessTargetIsInEffectiveRangeLeafNode : LeafNode
	{
		// Token: 0x060064BB RID: 25787 RVA: 0x002F888C File Offset: 0x002F6A8C
		protected AssessTargetIsInEffectiveRangeLeafNode()
		{
		}

		// Token: 0x060064BC RID: 25788 RVA: 0x002F8894 File Offset: 0x002F6A94
		public AssessTargetIsInEffectiveRangeLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064BD RID: 25789 RVA: 0x002F88A0 File Offset: 0x002F6AA0
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			CombatShipController shipController = this._sharedData.ShipController;
			Vector3 vector = default(Vector3);
			if (this._localData.TargetShip != null)
			{
				vector = shipController.position - this._localData.TargetShip.position;
			}
			else if (this._localData.TargetModule != null)
			{
				vector = shipController.position - this._localData.TargetModule.position;
			}
			if (vector.sqrMagnitude > this._localData.MinimumScaledCombatRange * this._localData.MinimumScaledCombatRange)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
