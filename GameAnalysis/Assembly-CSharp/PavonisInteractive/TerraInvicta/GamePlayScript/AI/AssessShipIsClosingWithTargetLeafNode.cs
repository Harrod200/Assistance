using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A3B RID: 2619
	public class AssessShipIsClosingWithTargetLeafNode : LeafNode
	{
		// Token: 0x060064AA RID: 25770 RVA: 0x002F8495 File Offset: 0x002F6695
		protected AssessShipIsClosingWithTargetLeafNode()
		{
		}

		// Token: 0x060064AB RID: 25771 RVA: 0x002F849D File Offset: 0x002F669D
		public AssessShipIsClosingWithTargetLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064AC RID: 25772 RVA: 0x002F84A8 File Offset: 0x002F66A8
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			CombatantController combatantController;
			if (this._localData.TargetShip)
			{
				combatantController = this._localData.TargetShip;
			}
			else
			{
				if (!this._localData.TargetModule)
				{
					return CombatShipBehaviourTree.ConditionResponse.Failed;
				}
				combatantController = this._localData.TargetModule;
			}
			if (this.IsClosingWithTarget(combatantController))
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x060064AD RID: 25773 RVA: 0x002F8504 File Offset: 0x002F6704
		protected virtual bool IsClosingWithTarget(CombatantController target)
		{
			Vector3 vector = target.velocityVector_kps - this._sharedData.ShipController.velocityVector_kps;
			Vector3 vector2 = target.position - this._sharedData.ShipController.position;
			float num = vector.Dot(vector2);
			bool flag = Vector3.Angle(vector2, -vector) < this._localData.TargetHeadingTestAngle;
			return num <= 0f && (num < 0f && flag);
		}
	}
}
