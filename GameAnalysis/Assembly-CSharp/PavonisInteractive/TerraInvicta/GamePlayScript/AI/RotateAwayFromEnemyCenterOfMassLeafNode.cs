using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A30 RID: 2608
	public class RotateAwayFromEnemyCenterOfMassLeafNode : LeafNode
	{
		// Token: 0x06006483 RID: 25731 RVA: 0x002F69EE File Offset: 0x002F4BEE
		protected RotateAwayFromEnemyCenterOfMassLeafNode()
		{
		}

		// Token: 0x06006484 RID: 25732 RVA: 0x002F69F6 File Offset: 0x002F4BF6
		public RotateAwayFromEnemyCenterOfMassLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
		}

		// Token: 0x06006485 RID: 25733 RVA: 0x002F6A00 File Offset: 0x002F4C00
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps() < this._sharedData.MinimumDVThreshold || !this._sharedData.ShipController.ShipState.CanRotateAndRoll())
			{
				return CombatShipBehaviourTree.ConditionResponse.Success;
			}
			Vector3 vector = default(Vector3);
			if (this._sharedData.OpposingFleetController != null && !this._sharedData.OpposingFleetController.AllActiveShipsDestroyed())
			{
				vector = this._sharedData.OpposingFleetController.GetCenterOfMass();
			}
			else if (this._sharedData.HabModuleControllers.Length != 0)
			{
				foreach (CombatHabModuleController combatHabModuleController in this._sharedData.HabModuleControllers)
				{
					vector += combatHabModuleController.position;
				}
				vector /= (float)this._sharedData.HabModuleControllers.Length;
			}
			CombatShipController shipController = this._sharedData.ShipController;
			Vector3 vector2 = vector - shipController.position;
			this._sharedData.ShipController._waypointNavigationController.ProposeRotation(Quaternion.LookRotation(-vector2));
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}
	}
}
