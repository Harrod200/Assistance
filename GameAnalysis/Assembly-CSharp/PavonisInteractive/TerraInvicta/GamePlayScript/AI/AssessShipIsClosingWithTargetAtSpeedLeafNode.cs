using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A3C RID: 2620
	public class AssessShipIsClosingWithTargetAtSpeedLeafNode : AssessShipIsClosingWithTargetLeafNode
	{
		// Token: 0x060064AE RID: 25774 RVA: 0x002F8584 File Offset: 0x002F6784
		protected AssessShipIsClosingWithTargetAtSpeedLeafNode()
		{
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x002F858C File Offset: 0x002F678C
		public AssessShipIsClosingWithTargetAtSpeedLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x002F8596 File Offset: 0x002F6796
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			return base.Execute();
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x002F85A0 File Offset: 0x002F67A0
		protected override bool IsClosingWithTarget(CombatantController target)
		{
			float num = 0.5f;
			Vector3 vector = target.velocityVector_kps - this._sharedData.ShipController.velocityVector_kps;
			Vector3 vector2 = target.position - this._sharedData.ShipController.position;
			float num2 = vector.Dot(vector2);
			bool flag = Vector3.Angle(vector2, -vector) < this._localData.TargetHeadingTestAngle;
			return vector.sqrMagnitude >= num * num && num2 <= 0f && (num2 < 0f && flag);
		}
	}
}
