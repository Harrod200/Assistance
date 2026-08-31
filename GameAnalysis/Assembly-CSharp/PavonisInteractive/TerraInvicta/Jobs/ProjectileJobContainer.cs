using System;
using System.Collections.Generic;
using System.Numerics;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace PavonisInteractive.TerraInvicta.Jobs
{
	// Token: 0x02000986 RID: 2438
	public class ProjectileJobContainer : MonoBehaviour
	{
		// Token: 0x06005CBD RID: 23741 RVA: 0x002C2F18 File Offset: 0x002C1118
		public void AddProjectile(ProjectileController controller, Transform projectile, ProjectileJobData.MovementType type, TISpaceCombatProjectileState state, float dv, float maxAcceleration, float terminalVelocity, global::UnityEngine.Vector3 velocityVector, global::UnityEngine.Vector3 originPosition, IDamageable target, float manuverAngle_deg, float thrustRamp_s, float turnRamp_s, float turnRate_deg_s)
		{
			ProjectileJobData projectileJobData = new ProjectileJobData
			{
				Movement = type,
				MaxAcceleration = maxAcceleration,
				TurnRate = 0f,
				ManeuverAngle_rad = 0.017453292f * manuverAngle_deg,
				ManeuverParameter = ((0.017453292f * manuverAngle_deg != 0f) ? ((float)(1.0 / Math.Tan((double)(0.017453292f * manuverAngle_deg)))) : 0f),
				CurrentDv = dv,
				ElapseTime = 0f,
				TerminalVelocity = terminalVelocity,
				CurrentAcceleration = 0f,
				ThrustRamp_s = thrustRamp_s,
				TurnRamp_s = turnRamp_s,
				MaxTurnRate_deg = turnRate_deg_s,
				AccelerationVector = new global::System.Numerics.Vector3(0f, 0f, 0f),
				VelocityVector = new global::System.Numerics.Vector3(velocityVector.x, velocityVector.y, velocityVector.z),
				LaunchVelocity = new global::System.Numerics.Vector3(velocityVector.x, velocityVector.y, velocityVector.z),
				OriginPosition = new global::System.Numerics.Vector3(originPosition.x, originPosition.y, originPosition.z),
				TargetPosition = ((target != null) ? new global::System.Numerics.Vector3(target.position.x, target.position.y, target.position.z) : new global::System.Numerics.Vector3(0f, 0f, 0f))
			};
			ProjectileReferences projectileReferences = new ProjectileReferences
			{
				Controller = controller,
				State = state,
				Target = target
			};
			this._projectiles.Add(projectile);
			this._projectileData.Add(projectileJobData);
			this._projectileReferences.Add(projectileReferences);
		}

		// Token: 0x06005CBE RID: 23742 RVA: 0x002C30E8 File Offset: 0x002C12E8
		public void RemoveProjectile(Transform projectile)
		{
			int num = this._projectiles.IndexOf(projectile);
			if (num >= 0)
			{
				this._projectiles.RemoveAt(num);
				this._projectileData.RemoveAt(num);
				this._projectileReferences.RemoveAt(num);
			}
		}

		// Token: 0x06005CBF RID: 23743 RVA: 0x002C312C File Offset: 0x002C132C
		public void SetProjectileTarget(Transform projectile, IDamageable target)
		{
			int num = this._projectiles.IndexOf(projectile);
			if (num >= 0)
			{
				ProjectileReferences projectileReferences = this._projectileReferences[num];
				projectileReferences = new ProjectileReferences
				{
					State = projectileReferences.State,
					Controller = projectileReferences.Controller,
					Target = target
				};
				this._projectileReferences[num] = projectileReferences;
			}
		}

		// Token: 0x06005CC0 RID: 23744 RVA: 0x002C3190 File Offset: 0x002C1390
		public void ClearAllJobs()
		{
			this._projectiles.Clear();
			this._projectileData.Clear();
			this._projectileReferences.Clear();
		}

		// Token: 0x06005CC1 RID: 23745 RVA: 0x002C31B3 File Offset: 0x002C13B3
		private void Awake()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this._lastUpdateTime = new TIDateTime(this.gameTime.currentTime);
		}

		// Token: 0x06005CC2 RID: 23746 RVA: 0x002C31DC File Offset: 0x002C13DC
		public void UpdateControllers()
		{
			for (int i = 0; i < this._projectileReferences.Count; i++)
			{
				this._projectileReferences[i].Controller.UpdateController();
			}
		}

		// Token: 0x06005CC3 RID: 23747 RVA: 0x002C3218 File Offset: 0x002C1418
		private void Update()
		{
			if (this._projectiles.Count > 0)
			{
				this._projectilesNativeArray = new TransformAccessArray(this._projectiles.ToArray(), 100);
				this._projectileDataNativeArray = new NativeArray<ProjectileJobData>(this._projectileData.ToArray(), Allocator.TempJob);
				this._projectileMovementJob = new ProjectilMovementJob
				{
					ElapsedTimeSinceLastUpdate = (float)this.gameTime.currentTime.DifferenceInSeconds(this._lastUpdateTime),
					ScalingFactor = 0.05f,
					_projectileData = this._projectileDataNativeArray
				};
				this._jobHandle = this._projectileMovementJob.Schedule(this._projectilesNativeArray, default(JobHandle));
			}
			this._lastUpdateTime = new TIDateTime(this.gameTime.currentTime);
		}

		// Token: 0x06005CC4 RID: 23748 RVA: 0x002C32E4 File Offset: 0x002C14E4
		private void LateUpdate()
		{
			if (this._projectiles.Count > 0)
			{
				this._jobHandle.Complete();
				for (int i = 0; i < this._projectileData.Count; i++)
				{
					ProjectileJobData projectileJobData = this._projectileMovementJob._projectileData[i];
					ProjectileReferences projectileReferences = this._projectileReferences[i];
					if (!(this._projectiles[i] == null))
					{
						if (projectileJobData.Movement == ProjectileJobData.MovementType.Missile)
						{
							if (projectileReferences.Target == null)
							{
								goto IL_01CB;
							}
							global::UnityEngine.Vector3 position = projectileReferences.Target.position;
							projectileJobData.TargetPosition = new global::System.Numerics.Vector3(position.x, position.y, position.z);
							global::UnityEngine.Vector3 velocityVector = projectileReferences.Target.velocityVector;
							projectileJobData.TargetVelocity = new global::System.Numerics.Vector3(velocityVector.x, velocityVector.y, velocityVector.z);
							projectileReferences.State.thrustersEnabled = !Mathf.Approximately(projectileJobData.CurrentAcceleration, 0f);
							projectileReferences.State.thrustAmount = projectileJobData.thrustFraction;
							projectileReferences.Controller.v3_accelerationVector = new global::UnityEngine.Vector3(projectileJobData.AccelerationVector.X, projectileJobData.AccelerationVector.Y, projectileJobData.AccelerationVector.Z);
							projectileReferences.State.velocityVector_kps = new global::UnityEngine.Vector3(projectileJobData.VelocityVector.X, projectileJobData.VelocityVector.Y, projectileJobData.VelocityVector.Z) / 0.05f;
							projectileReferences.State.deltaV = projectileJobData.CurrentDv;
						}
						projectileJobData.ElapseTime = (float)this.gameTime.currentTime.DifferenceInSeconds(this._projectileReferences[i].State.launchTime);
						projectileReferences.State.UpdatePosition(this._projectiles[i].position);
						this._projectileData[i] = projectileJobData;
					}
					IL_01CB:;
				}
				this._projectilesNativeArray.Dispose();
				this._projectileDataNativeArray.Dispose();
			}
		}

		// Token: 0x06005CC5 RID: 23749 RVA: 0x002C34E8 File Offset: 0x002C16E8
		private void OnDestroy()
		{
			if (this._projectiles.Count > 0)
			{
				this._jobHandle.Complete();
				if (this._projectilesNativeArray.isCreated)
				{
					this._projectilesNativeArray.Dispose();
				}
				if (this._projectileDataNativeArray.IsCreated)
				{
					this._projectileDataNativeArray.Dispose();
				}
			}
		}

		// Token: 0x04004204 RID: 16900
		private List<Transform> _projectiles = new List<Transform>(100);

		// Token: 0x04004205 RID: 16901
		private List<ProjectileJobData> _projectileData = new List<ProjectileJobData>(100);

		// Token: 0x04004206 RID: 16902
		private List<ProjectileReferences> _projectileReferences = new List<ProjectileReferences>(100);

		// Token: 0x04004207 RID: 16903
		private TransformAccessArray _projectilesNativeArray;

		// Token: 0x04004208 RID: 16904
		private NativeArray<ProjectileJobData> _projectileDataNativeArray;

		// Token: 0x04004209 RID: 16905
		private JobHandle _jobHandle;

		// Token: 0x0400420A RID: 16906
		private ProjectilMovementJob _projectileMovementJob;

		// Token: 0x0400420B RID: 16907
		private TIDateTime _lastUpdateTime;

		// Token: 0x0400420C RID: 16908
		private GameTimeManager gameTime;
	}
}
