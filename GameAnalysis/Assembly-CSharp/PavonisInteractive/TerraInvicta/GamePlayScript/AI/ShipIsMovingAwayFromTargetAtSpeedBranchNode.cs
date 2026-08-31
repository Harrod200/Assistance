using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A19 RID: 2585
	public class ShipIsMovingAwayFromTargetAtSpeedBranchNode : BranchNode
	{
		// Token: 0x06006431 RID: 25649 RVA: 0x002F4689 File Offset: 0x002F2889
		protected ShipIsMovingAwayFromTargetAtSpeedBranchNode()
		{
		}

		// Token: 0x06006432 RID: 25650 RVA: 0x002F4691 File Offset: 0x002F2891
		public ShipIsMovingAwayFromTargetAtSpeedBranchNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local, params ITreeNode[] children)
			: base(shared, local, children)
		{
		}

		// Token: 0x06006433 RID: 25651 RVA: 0x002F469C File Offset: 0x002F289C
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
			if (this.IsMovingAwayFromTarget(combatantController))
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

		// Token: 0x06006434 RID: 25652 RVA: 0x002F4718 File Offset: 0x002F2918
		protected virtual bool IsMovingAwayFromTarget(CombatantController target)
		{
			float num = 1f;
			Vector3 vector = target.velocityVector_kps - this._sharedData.ShipController.velocityVector_kps;
			Vector3 vector2 = target.position - this._sharedData.ShipController.position;
			float num2 = vector.Dot(vector2);
			bool flag = Vector3.Angle(vector2.normalized, this._sharedData.ShipController.velocityVector.normalized) < this._localData.TargetHeadingTestAngle;
			if (this._sharedData.ShipController.velocityVector_kps.sqrMagnitude < num * num)
			{
				return false;
			}
			if (num2 > 0f)
			{
				return true;
			}
			bool flag2 = num2 < 0f && flag;
			return false;
		}
	}
}
