using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A41 RID: 2625
	public class AssessTargetIsGreaterThan2000kmAwayLeafNode : LeafNode
	{
		// Token: 0x060064BE RID: 25790 RVA: 0x002F8944 File Offset: 0x002F6B44
		protected AssessTargetIsGreaterThan2000kmAwayLeafNode()
		{
		}

		// Token: 0x060064BF RID: 25791 RVA: 0x002F894C File Offset: 0x002F6B4C
		public AssessTargetIsGreaterThan2000kmAwayLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064C0 RID: 25792 RVA: 0x002F8958 File Offset: 0x002F6B58
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			float num = SpaceCombatManager.km_to_scale(2000f);
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
			if (vector.sqrMagnitude < num * num)
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
