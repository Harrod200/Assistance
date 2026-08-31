using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A3E RID: 2622
	public class AssessShipIsPointedTowardsTargetLeafNode : LeafNode
	{
		// Token: 0x060064B5 RID: 25781 RVA: 0x002F86EB File Offset: 0x002F68EB
		protected AssessShipIsPointedTowardsTargetLeafNode()
		{
		}

		// Token: 0x060064B6 RID: 25782 RVA: 0x002F86F3 File Offset: 0x002F68F3
		public AssessShipIsPointedTowardsTargetLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064B7 RID: 25783 RVA: 0x002F8700 File Offset: 0x002F6900
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			CombatShipController shipController = this._sharedData.ShipController;
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
			if (Vector3.Angle((combatantController.position - shipController.position).normalized, shipController.heading) < this._localData.TargetHeadingTestAngle)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}
	}
}
