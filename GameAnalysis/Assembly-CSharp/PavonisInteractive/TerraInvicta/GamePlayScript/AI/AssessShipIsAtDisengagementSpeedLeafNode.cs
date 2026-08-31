using System;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A43 RID: 2627
	public class AssessShipIsAtDisengagementSpeedLeafNode : LeafNode
	{
		// Token: 0x060064C4 RID: 25796 RVA: 0x002F8AC0 File Offset: 0x002F6CC0
		protected AssessShipIsAtDisengagementSpeedLeafNode()
		{
		}

		// Token: 0x060064C5 RID: 25797 RVA: 0x002F8AD3 File Offset: 0x002F6CD3
		public AssessShipIsAtDisengagementSpeedLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064C6 RID: 25798 RVA: 0x002F8AE8 File Offset: 0x002F6CE8
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			if (this._sharedData.OpposingFleetController != null && !this._sharedData.OpposingFleetController.AllActiveShipsDestroyed())
			{
				vector = this._sharedData.OpposingFleetController.GetFleetVelocityVector();
				vector2 = this._sharedData.OpposingFleetController.GetCenterOfMass();
			}
			else if (this._sharedData.HabModuleControllers.Length != 0)
			{
				vector2 = this._sharedData.HabModuleControllers[0].position;
			}
			Vector3 vector3 = vector - this._sharedData.ShipController.velocityVector;
			Vector3 vector4 = vector2 - this._sharedData.ShipController.position;
			if (vector3.Dot(vector4) > 0f && vector.sqrMagnitude * this.escapeVelocityMultiplier < this._sharedData.ShipController.velocityVector.sqrMagnitude)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}

		// Token: 0x040046E2 RID: 18146
		private float escapeVelocityMultiplier = 2f;
	}
}
