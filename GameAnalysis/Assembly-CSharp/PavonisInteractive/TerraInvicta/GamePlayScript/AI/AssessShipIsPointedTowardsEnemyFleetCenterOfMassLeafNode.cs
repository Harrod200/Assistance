using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A3F RID: 2623
	public class AssessShipIsPointedTowardsEnemyFleetCenterOfMassLeafNode : LeafNode
	{
		// Token: 0x060064B8 RID: 25784 RVA: 0x002F8791 File Offset: 0x002F6991
		protected AssessShipIsPointedTowardsEnemyFleetCenterOfMassLeafNode()
		{
		}

		// Token: 0x060064B9 RID: 25785 RVA: 0x002F8799 File Offset: 0x002F6999
		public AssessShipIsPointedTowardsEnemyFleetCenterOfMassLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x060064BA RID: 25786 RVA: 0x002F87A4 File Offset: 0x002F69A4
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			Vector3 vector = default(Vector3);
			if (this._sharedData.OpposingFleetController == null && this._sharedData.HabModuleControllers.Length != 0)
			{
				foreach (CombatHabModuleController combatHabModuleController in this._sharedData.HabModuleControllers)
				{
					vector += combatHabModuleController.position;
				}
				vector /= (float)this._sharedData.HabModuleControllers.Length;
			}
			else
			{
				if (this._sharedData.OpposingFleetController != null && this._sharedData.OpposingFleetController.AllActiveShipsDestroyed())
				{
					return CombatShipBehaviourTree.ConditionResponse.Success;
				}
				vector = this._sharedData.OpposingFleetController.GetCenterOfMass();
			}
			CombatShipController shipController = this._sharedData.ShipController;
			if (Vector3.Angle((vector - shipController.position).normalized, shipController.heading) < this._localData.TargetHeadingTestAngle)
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			return CombatShipBehaviourTree.ConditionResponse.Failed;
		}
	}
}
