using System;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A29 RID: 2601
	public class UpdateWithdrawTrajectoryLeafNode : LeafNode
	{
		// Token: 0x0600646C RID: 25708 RVA: 0x002F6286 File Offset: 0x002F4486
		protected UpdateWithdrawTrajectoryLeafNode()
		{
		}

		// Token: 0x0600646D RID: 25709 RVA: 0x002F628E File Offset: 0x002F448E
		public UpdateWithdrawTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x0600646E RID: 25710 RVA: 0x002F62A4 File Offset: 0x002F44A4
		public override CombatShipBehaviourTree.ConditionResponse Execute()
		{
			if (this._sharedData.CurrentTime <= this.trajectoryValidUntilTime)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			if (this._sharedData.ShipController.InCollisionAvoidanceManeuver)
			{
				return CombatShipBehaviourTree.ConditionResponse.Running;
			}
			this.trajectoryValidUntilTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			this.trajectoryValidUntilTime.AddSeconds(300.0);
			float num = this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps();
			if (!this.TryAssignWithdrawPosition(num))
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x0600646F RID: 25711 RVA: 0x002F6330 File Offset: 0x002F4530
		private bool TryAssignWithdrawPosition(float remainingDeltaV)
		{
			float num = float.MaxValue;
			CombatantController combatantController = null;
			if (this._sharedData.OpposingFleetController != null)
			{
				foreach (CombatShipController combatShipController in this._sharedData.OpposingFleetController.activeShipControllers)
				{
					if (!combatShipController.isDestroyed && !combatShipController.destructionTriggered)
					{
						float sqrMagnitude = (combatShipController.position - this._sharedData.ShipController.position).sqrMagnitude;
						if (num > sqrMagnitude)
						{
							num = sqrMagnitude;
							combatantController = combatShipController;
						}
					}
				}
				foreach (CombatHabModuleController combatHabModuleController in this._sharedData.HabModuleControllers)
				{
					if (this._sharedData.FactionState != combatHabModuleController.faction && !combatHabModuleController.isDestroyed && !combatHabModuleController.destructionTriggered)
					{
						float sqrMagnitude2 = (combatHabModuleController.position - this._sharedData.ShipController.position).sqrMagnitude;
						if (sqrMagnitude2 < num)
						{
							num = sqrMagnitude2;
							combatantController = combatHabModuleController;
						}
					}
				}
			}
			if (combatantController != null && !this._sharedData.ShipController.ShipState.overheated)
			{
				float num2 = 250f;
				Vector3 vector = this._sharedData.ShipController.position - combatantController.position;
				if (vector.sqrMagnitude >= num2 * num2)
				{
					return true;
				}
				if (remainingDeltaV > this._sharedData.MinimumDVThreshold && this.TryAssignPathToPosition(this._sharedData.ShipController.position, vector * 2f, this._localData.MinimumScaledCombatRange))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006470 RID: 25712 RVA: 0x002F64F4 File Offset: 0x002F46F4
		private bool TryAssignPathToPosition(Vector3 startPosition, Vector3 endPosition, float scaledCombatRange)
		{
			AccelerationConstraints dvconservingAccelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(false);
			TIDateTime tidateTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			tidateTime.AddSeconds(300.0);
			Vector3 vector = (endPosition - startPosition).normalized * scaledCombatRange;
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				Timing = tidateTime,
				Velocity = this._sharedData.ShipController.velocityVector,
				Position = endPosition - vector,
				Rotation = Quaternion.LookRotation(vector.normalized)
			};
			Vector3[] array = new Vector3[10];
			this._sharedData.PathFinder.FindPath(this._sharedData.ShipController.position, this._sharedData.ShipController.velocityVector.normalized, proposedWaypoint.Position, this._sharedData.FleetController, ref array);
			if (array.Length > 1)
			{
				this._sharedData.ShipController.ProposePath(array, proposedWaypoint, dvconservingAccelerationConstraints);
			}
			return true;
		}

		// Token: 0x040046CF RID: 18127
		private TIDateTime trajectoryValidUntilTime;
	}
}
