using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.GamePlayScript.AI
{
	// Token: 0x02000A23 RID: 2595
	public class UpdateShipTrajectoryLeafNode : LeafNode
	{
		// Token: 0x06006453 RID: 25683 RVA: 0x002F5320 File Offset: 0x002F3520
		protected UpdateShipTrajectoryLeafNode()
		{
		}

		// Token: 0x06006454 RID: 25684 RVA: 0x002F5328 File Offset: 0x002F3528
		public UpdateShipTrajectoryLeafNode(CombatShipBehaviourTree.SharedBehaviourData shared, CombatShipBehaviourTree.LocalBehaviourData local)
			: base(shared, local)
		{
			this.trajectoryValidUntilTime = shared.CurrentTime;
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x002F5340 File Offset: 0x002F3540
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
			List<ProjectileController> allThreateningProjectiles = this._sharedData.ShipController.GetAllThreateningProjectiles();
			this._sharedData.ShipController.FilterForImminentImpactThreats(ref allThreateningProjectiles);
			float num = this._sharedData.ShipController.ShipState.AvailableDeltaVForCombat_kps();
			if (allThreateningProjectiles.Count > 0)
			{
				if (!this.TryAssignDefensivePosition(allThreateningProjectiles))
				{
					return CombatShipBehaviourTree.ConditionResponse.Failed;
				}
			}
			else
			{
				bool flag = false;
				float num2 = 45f;
				if (this._localData.TargetShip != null)
				{
					Vector3 vector = this._localData.TargetShip.velocityVector_kps - this._sharedData.ShipController.velocityVector_kps;
					flag = Vector3.Angle(this._localData.TargetShip.position - this._sharedData.ShipController.position, -vector) < num2;
				}
				else if (this._localData.TargetModule != null)
				{
					flag = Vector3.Angle((this._localData.TargetModule.position - this._sharedData.ShipController.position).normalized, this._sharedData.ShipController.velocityVector.normalized) < num2;
				}
				if ((num > this._sharedData.MinimumDVThreshold || !flag) && !this.AssignOffensivePosition())
				{
					return CombatShipBehaviourTree.ConditionResponse.Failed;
				}
			}
			return CombatShipBehaviourTree.ConditionResponse.Success;
		}

		// Token: 0x06006456 RID: 25686 RVA: 0x002F54F8 File Offset: 0x002F36F8
		private bool TryAssignDefensivePosition(List<ProjectileController> threateningProjectiles)
		{
			AccelerationConstraints dvconservingAccelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(false);
			Vector3 vector = Vector3.zero;
			float num = float.MaxValue;
			foreach (ProjectileController projectileController in threateningProjectiles)
			{
				float num2 = (this._sharedData.ShipController.position - projectileController.position).magnitude / SpaceCombatManager.vector_km_to_scale(projectileController.velocityVector_kps).magnitude;
				num = Mathf.Min(num, num2);
				vector += (this._sharedData.ShipController.position - projectileController.position).normalized;
			}
			num = Mathf.Min(num, (float)(this._sharedData.CurrentTime - this._sharedData.ShipController.TimeOfNextWaypoint).TotalSeconds);
			vector.Normalize();
			float num3 = PhysicsHelpers.DisplacementFromAccelerationAndTime(dvconservingAccelerationConstraints.LinearAcceleration, num);
			Vector3 vector2 = Vector3.Cross(vector, this._sharedData.ShipController.velocityVector.normalized).normalized * num3;
			Vector3 vector3 = vector2;
			if (this._localData.TargetShip)
			{
				vector3 = this._localData.TargetShip.position;
			}
			else if (this._localData.TargetModule)
			{
				vector3 = this._localData.TargetModule.position;
			}
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				Timing = this._sharedData.ShipController.TimeOfNextWaypoint,
				Rotation = Quaternion.LookRotation((vector3 - this._sharedData.ShipController.position).normalized),
				Position = vector2,
				Velocity = this._sharedData.ShipController.velocityVector
			};
			Vector3[] array = new Vector3[10];
			this._sharedData.PathFinder.FindPath(this._sharedData.ShipController.position, this._sharedData.ShipController.velocityVector.normalized, proposedWaypoint.Position, this._sharedData.FleetController, ref array);
			if (array.Length > 1)
			{
				this._sharedData.ShipController.ProposePath(array, proposedWaypoint, dvconservingAccelerationConstraints);
			}
			return true;
		}

		// Token: 0x06006457 RID: 25687 RVA: 0x002F5770 File Offset: 0x002F3970
		private bool AssignOffensivePosition()
		{
			if (this._localData.TargetShip)
			{
				return this.TryAssignPathToTarget(this._localData.TargetShip.position, this._localData.TargetShip.velocityVector, this._localData.MinimumScaledCombatRange);
			}
			return this._localData.TargetModule && this.TryAssignPathToTarget(this._localData.TargetModule.position, this._localData.TargetModule.velocityVector, this._localData.MinimumScaledCombatRange);
		}

		// Token: 0x06006458 RID: 25688 RVA: 0x002F5808 File Offset: 0x002F3A08
		private bool TryAssignPathToTarget(Vector3 position, Vector3 velocity, float scaledCombatRange)
		{
			AccelerationConstraints dvconservingAccelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(true);
			TIDateTime tidateTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			tidateTime.AddSeconds(300.0);
			Vector3 vector = PhysicsHelpers.PositionFromVelocityAndTime(position, velocity, (float)(tidateTime - this._sharedData.CurrentTime).TotalSeconds);
			Vector3 vector2 = (this._sharedData.ShipController.positionAtTime(tidateTime.ExportTime()) - vector).normalized * scaledCombatRange;
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				Timing = tidateTime,
				Velocity = this._sharedData.ShipController.velocityVector,
				Position = vector - vector2,
				Rotation = Quaternion.LookRotation(vector2.normalized)
			};
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

		// Token: 0x06006459 RID: 25689 RVA: 0x002F5964 File Offset: 0x002F3B64
		private bool TryAssignPathToPosition(Vector3 startPosition, Vector3 endPosition, float scaledCombatRange)
		{
			AccelerationConstraints dvconservingAccelerationConstraints = this._sharedData.ShipController.GetDVConservingAccelerationConstraints(true);
			TIDateTime tidateTime = new TIDateTime(this._sharedData.ShipController.TimeOfNextWaypoint);
			tidateTime.AddSeconds(300.0);
			Vector3 vector = (endPosition - startPosition).normalized * scaledCombatRange;
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				Timing = tidateTime,
				Velocity = this._sharedData.ShipController.velocityVector,
				Position = endPosition,
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

		// Token: 0x040046C9 RID: 18121
		private TIDateTime trajectoryValidUntilTime;
	}
}
