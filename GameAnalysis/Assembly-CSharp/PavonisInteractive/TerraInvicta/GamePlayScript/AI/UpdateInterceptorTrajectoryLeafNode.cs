using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A24 RID: 2596
	public class UpdateInterceptorTrajectoryLeafNode : LeafNode
	{
		// Token: 0x0600645A RID: 25690 RVA: 0x002F5A6C File Offset: 0x002F3C6C
		protected UpdateInterceptorTrajectoryLeafNode()
		{
		}

		// Token: 0x0600645B RID: 25691 RVA: 0x002F5A74 File Offset: 0x002F3C74
		public UpdateInterceptorTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x0600645C RID: 25692 RVA: 0x002F5A8C File Offset: 0x002F3C8C
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
			this.trajectoryValidUntilTime.AddSeconds((double)this._localData.SecondsPerTrajectoryUpdate);
			float num = this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps();
			if (!this.AssignOffensivePosition(num))
			{
				return CombatShipBehaviourTree.ConditionResponse.Failed;
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x0600645D RID: 25693 RVA: 0x002F5B1C File Offset: 0x002F3D1C
		private bool AssignOffensivePosition(float remainingDeltaV)
		{
			if (remainingDeltaV > this._sharedData.MinimumDVThreshold)
			{
				if (this._localData.TargetShip)
				{
					return this.TryAssignPathToTarget(this._localData.TargetShip.position, this._localData.TargetShip.velocityVector, this._localData.MinimumScaledCombatRange);
				}
				if (this._localData.TargetModule)
				{
					return this.TryAssignPathToTarget(this._localData.TargetModule.position, this._localData.TargetModule.velocityVector, this._localData.MinimumScaledCombatRange);
				}
			}
			return false;
		}

		// Token: 0x0600645E RID: 25694 RVA: 0x002F5BC4 File Offset: 0x002F3DC4
		private bool TryAssignPathToTarget(Vector3 position, Vector3 velocity, float scaledCombatRange)
		{
			AccelerationConstraints dvconservingAccelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(true);
			TIDateTime tidateTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			tidateTime.AddSeconds(60.0);
			Vector3 vector = PhysicsHelpers.PositionFromVelocityAndTime(position, velocity, (float)(tidateTime - this._sharedData.CurrentTime).TotalSeconds);
			Vector3 vector2 = this._sharedData.OpposingFleetController.GetCenterOfMass() - position;
			Vector3 vector3 = ((this._sharedData.OpposingFleetController.activeShipControllers.Count != 1) ? (vector2.normalized * (scaledCombatRange * 0.9f)) : (global::UnityEngine.Random.onUnitSphere * (scaledCombatRange * 0.9f)));
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				Timing = tidateTime,
				Velocity = this._sharedData.ShipController.velocityVector,
				Position = vector - vector3,
				Rotation = Quaternion.LookRotation(vector3.normalized)
			};
			Vector3.Angle((proposedWaypoint.Position - this._sharedData.ShipController.position).normalized, this._sharedData.ShipController.velocityVector.normalized);
			Vector3[] array = new Vector3[10];
			this._sharedData.PathFinder.FindPath(this._sharedData.ShipController.position, this._sharedData.ShipController.velocityVector.normalized, proposedWaypoint.Position, this._sharedData.FleetController, ref array);
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length; i++)
				{
				}
				this._sharedData.ShipController.ProposePath(array, proposedWaypoint, dvconservingAccelerationConstraints);
			}
			return true;
		}

		// Token: 0x0600645F RID: 25695 RVA: 0x002F5D98 File Offset: 0x002F3F98
		private void FilterForImminentImpactThreats(ref List<ProjectileController> projectiles)
		{
			List<ProjectileController> list = new List<ProjectileController>(4);
			foreach (ProjectileController projectileController in projectiles)
			{
				if ((this._sharedData.ShipController.position - projectileController.position).magnitude / SpaceCombatManager.vector_km_to_scale(projectileController.velocityVector_kps).magnitude > this._sharedData.SecondsBetweenWaypoints)
				{
					list.Add(projectileController);
				}
			}
			foreach (ProjectileController projectileController2 in list)
			{
				projectiles.Remove(projectileController2);
			}
		}

		// Token: 0x040046CA RID: 18122
		private TIDateTime trajectoryValidUntilTime;
	}
}
