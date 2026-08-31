using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A3D RID: 2621
	public class AssessShipIsHeadedInDirectionOfTargetLeafNode : LeafNode
	{
		// Token: 0x060064B2 RID: 25778 RVA: 0x002F8636 File Offset: 0x002F6836
		protected AssessShipIsHeadedInDirectionOfTargetLeafNode()
		{
		}

		// Token: 0x060064B3 RID: 25779 RVA: 0x002F863E File Offset: 0x002F683E
		public AssessShipIsHeadedInDirectionOfTargetLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064B4 RID: 25780 RVA: 0x002F8648 File Offset: 0x002F6848
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
			if (Vector3.Angle((combatantController.position - shipController.position).normalized, this._sharedData.ShipController.velocityVector.normalized) < this._localData.TargetHeadingTestAngle)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}
	}
}
