using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F8 RID: 2552
	public class WaypointNavigationController
	{
		// Token: 0x170010B6 RID: 4278
		// (get) Token: 0x06006121 RID: 24865 RVA: 0x002DA6F8 File Offset: 0x002D88F8
		// (set) Token: 0x06006122 RID: 24866 RVA: 0x002DA700 File Offset: 0x002D8900
		private AccelerationConstraints _accelerationConstraints { get; set; }

		// Token: 0x170010B7 RID: 4279
		// (get) Token: 0x06006123 RID: 24867 RVA: 0x002DA709 File Offset: 0x002D8909
		public bool CanWaypointsBeAdjusted
		{
			get
			{
				return this._canWaypointsBeAdjusted;
			}
		}

		// Token: 0x170010B8 RID: 4280
		// (get) Token: 0x06006124 RID: 24868 RVA: 0x002DA711 File Offset: 0x002D8911
		public GameObject WaypointContainer
		{
			get
			{
				return this._waypointContainer.gameObject;
			}
		}

		// Token: 0x170010B9 RID: 4281
		// (get) Token: 0x06006125 RID: 24869 RVA: 0x002DA71E File Offset: 0x002D891E
		// (set) Token: 0x06006126 RID: 24870 RVA: 0x002DA728 File Offset: 0x002D8928
		public bool PadlockEnabled
		{
			get
			{
				return this._padlockEnabled;
			}
			set
			{
				for (int i = 0; i < this._waypoints.Length; i++)
				{
					this._waypoints[i].PadlockEnabled = value;
				}
				this._padlockEnabled = value;
				GameControl.eventManager.TriggerEvent(new ShipPadlockStateChanged(value), null, new object[] { this._shipState });
			}
		}

		// Token: 0x170010BA RID: 4282
		// (get) Token: 0x06006127 RID: 24871 RVA: 0x002DA77D File Offset: 0x002D897D
		// (set) Token: 0x06006128 RID: 24872 RVA: 0x002DA788 File Offset: 0x002D8988
		public bool AllStopEnabled
		{
			get
			{
				return this._allStopEnabled;
			}
			set
			{
				for (int i = 0; i < this._waypoints.Length; i++)
				{
					this._waypoints[i].AllStopEnabled = value;
				}
				this._allStopEnabled = value;
			}
		}

		// Token: 0x170010BB RID: 4283
		// (get) Token: 0x06006129 RID: 24873 RVA: 0x002DA7BD File Offset: 0x002D89BD
		// (set) Token: 0x0600612A RID: 24874 RVA: 0x002DA7C8 File Offset: 0x002D89C8
		public bool MatchVelocityEnabled
		{
			get
			{
				return this._matchVelocityEnabled;
			}
			set
			{
				for (int i = 0; i < this._waypoints.Length; i++)
				{
					this._waypoints[i].MatchVelocityEnabled = value;
				}
				this._matchVelocityEnabled = value;
			}
		}

		// Token: 0x170010BC RID: 4284
		// (get) Token: 0x0600612B RID: 24875 RVA: 0x002DA7FD File Offset: 0x002D89FD
		// (set) Token: 0x0600612C RID: 24876 RVA: 0x002DA808 File Offset: 0x002D8A08
		public bool DefensiveManueversEnabled
		{
			get
			{
				return this._defensiveManueversEnabled;
			}
			set
			{
				for (int i = 0; i < this._waypoints.Length; i++)
				{
					this._waypoints[i].DefensiveManueversEnabled = value;
				}
				this._defensiveManueversEnabled = value;
			}
		}

		// Token: 0x170010BD RID: 4285
		// (get) Token: 0x0600612D RID: 24877 RVA: 0x002DA83D File Offset: 0x002D8A3D
		// (set) Token: 0x0600612E RID: 24878 RVA: 0x002DA845 File Offset: 0x002D8A45
		public bool _allowPathDrawing { get; private set; }

		// Token: 0x170010BE RID: 4286
		// (get) Token: 0x0600612F RID: 24879 RVA: 0x002DA84E File Offset: 0x002D8A4E
		// (set) Token: 0x06006130 RID: 24880 RVA: 0x002DA856 File Offset: 0x002D8A56
		public CombatantController PrimaryTarget
		{
			get
			{
				return this._primaryTarget;
			}
			set
			{
				this._primaryTarget = value;
			}
		}

		// Token: 0x170010BF RID: 4287
		// (get) Token: 0x06006131 RID: 24881 RVA: 0x002DA85F File Offset: 0x002D8A5F
		// (set) Token: 0x06006132 RID: 24882 RVA: 0x002DA867 File Offset: 0x002D8A67
		public CombatantController ManeuverTarget
		{
			get
			{
				return this._maneuverTarget;
			}
			set
			{
				this._maneuverTarget = value;
			}
		}

		// Token: 0x170010C0 RID: 4288
		// (get) Token: 0x06006133 RID: 24883 RVA: 0x002DA870 File Offset: 0x002D8A70
		public bool EnrouteIntentionalCollision
		{
			get
			{
				return this._shipState.canSuicide && this._primaryTarget != null;
			}
		}

		// Token: 0x170010C1 RID: 4289
		// (get) Token: 0x06006134 RID: 24884 RVA: 0x002DA88D File Offset: 0x002D8A8D
		private float WaypointTimeDelta
		{
			get
			{
				return this._waypointSharedData.WaypointTimeDelta;
			}
		}

		// Token: 0x170010C2 RID: 4290
		// (get) Token: 0x06006135 RID: 24885 RVA: 0x002DA89A File Offset: 0x002D8A9A
		// (set) Token: 0x06006136 RID: 24886 RVA: 0x002DA8A2 File Offset: 0x002D8AA2
		public TIDateTime TimeOfCollisionPassed { get; private set; }

		// Token: 0x170010C3 RID: 4291
		// (get) Token: 0x06006137 RID: 24887 RVA: 0x002DA8AB File Offset: 0x002D8AAB
		public TIDateTime TimeOfFirstWaypoint
		{
			get
			{
				return new TIDateTime(this._waypoints[0].Timing);
			}
		}

		// Token: 0x170010C4 RID: 4292
		// (get) Token: 0x06006138 RID: 24888 RVA: 0x002DA8C0 File Offset: 0x002D8AC0
		private float maxClosestApproachDistance
		{
			get
			{
				if (this._maxClosestApproachDistance <= 0f)
				{
					this._maxClosestApproachDistance = 0.75f * SpaceCombatManager.km_to_scale((float)TIFormationTemplate.GetSpacingOffset_km(true, false).Min<Vector3d>((Vector3d a) => a.x));
				}
				return this._maxClosestApproachDistance;
			}
		}

		// Token: 0x06006139 RID: 24889 RVA: 0x002DA920 File Offset: 0x002D8B20
		public WaypointNavigationController(string name, int waypointCount, Vector3 currentVelocity, Vector3 currentPosition, TIDateTime currentTime, WaypointSharedData sharedData, TISpaceShipState shipState, Vector3 collisionBoxSize, Camera mainCamera, CombatShipController combatant)
		{
			this._name = name;
			this._waypointCount = waypointCount;
			this._waypointSharedData = sharedData;
			this._shipState = shipState;
			this._thisCombatant = combatant;
			this._accelerationConstraints = new AccelerationConstraints(this._waypointSharedData.LinearAcceleration, this._waypointSharedData.CruiseAcceleration, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity);
			this.cached_acceleration = this._waypointSharedData.LinearAcceleration;
			this.cached_angular_acceleration_rads2 = this._waypointSharedData.AngularAccelerationRads;
			this.cached_max_angular_velocity_rads2 = this._waypointSharedData.MaxAngularVelocity;
			this._propulsionValuesDirty = false;
			this._propulsionValuesImproved = false;
			this._mainCamera = mainCamera;
			this._spaceCombatCameraController = this._mainCamera.GetComponent<SpaceCombatCameraController>();
			this._gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.InitializeWaypointCollections(currentPosition, currentVelocity, currentTime, !this._shipState.faction.player.isAI);
			WaypointNavigationController._waypointPlacementVisual = WaypointVisual.Create(this._waypointSharedData.WaypointPrefab, 1, this._waypointContainer.transform, shipState);
			WaypointNavigationController._waypointPlacementVisual.gameObject.SetActive(false);
			this._collisionBoxSize = collisionBoxSize;
			this._agentShipControllers = new List<CombatShipController>();
			this._habModuleControllers = new Dictionary<HabModuleController, Collider>();
			this._allowPathDrawing = true;
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.OnPreRenderCallback));
		}

		// Token: 0x0600613A RID: 24890 RVA: 0x002DAAF0 File Offset: 0x002D8CF0
		private void InitializeWaypointCollections(Vector3 initialPosition, Vector3 initialVelocity, TIDateTime initialTime, bool isPlayer)
		{
			this._waypointContainer = new GameObjectDictionary<string>(this._name + " Waypoint Container");
			this._waypoints = new AdjustableWaypoint[this._waypointCount];
			this._waypointControllers = new Dictionary<AdjustableWaypoint, WaypointController>();
			this._initialWaypoint = new WaypointNavigationController.InitialWaypoint(initialPosition, initialVelocity, Quaternion.LookRotation(initialVelocity.normalized), initialTime, 1f)
			{
				Timing = new TIDateTime(initialTime)
			};
			IPreviousWaypoint previousWaypoint = this._initialWaypoint;
			for (int i = 1; i < this._waypointCount + 1; i++)
			{
				TIDateTime tidateTime = new TIDateTime(initialTime);
				tidateTime.AddSeconds((double)((float)i * this.WaypointTimeDelta));
				Vector3 vector = PhysicsHelpers.PositionFromVelocityAndTime(initialPosition, initialVelocity, this.WaypointTimeDelta * (float)i);
				float num = 0.1f + 0.6f * (1f - (float)i / (float)(this._waypointCount - 1));
				AdjustableWaypoint adjustableWaypoint = new AdjustableWaypoint(previousWaypoint, this._accelerationConstraints, (isPlayer || i <= 2) ? 1f : num);
				adjustableWaypoint.ProposePlacement(vector, tidateTime, null, false, -1f);
				this._waypoints[i - 1] = adjustableWaypoint;
				WaypointController waypointController = new WaypointController(adjustableWaypoint, i, this._waypointSharedData, this._waypointContainer.transform, initialVelocity.normalized, true, this._shipState);
				waypointController.OnWaypointReadyForInput += this.HandleOnWaypointReadyForInput;
				waypointController.OnWaypointEndingInput += this.HandleOnWaypointEndingInput;
				waypointController.OnWaypointRemovalRequested += this.HandleOnWaypointRemovalRequested;
				this._waypointControllers.Add(adjustableWaypoint, waypointController);
				previousWaypoint = adjustableWaypoint;
			}
		}

		// Token: 0x0600613B RID: 24891 RVA: 0x002DAC77 File Offset: 0x002D8E77
		public void OnShipDestructionTriggered()
		{
			this._allowPathDrawing = false;
		}

		// Token: 0x0600613C RID: 24892 RVA: 0x002DAC80 File Offset: 0x002D8E80
		public void CachePropulsionValues(float new_acceleration, float new_cruise_acceleration, float new_angular_acceleration_rads2, float new_max_angular_velocity_rads2)
		{
			this._propulsionValuesImproved = new_acceleration >= this.cached_acceleration && new_cruise_acceleration >= this.cached_cruise_acceleration && new_angular_acceleration_rads2 >= this.cached_angular_acceleration_rads2 && new_max_angular_velocity_rads2 >= this.cached_max_angular_velocity_rads2;
			this.cached_acceleration = new_acceleration;
			this.cached_cruise_acceleration = new_cruise_acceleration;
			this.cached_angular_acceleration_rads2 = new_angular_acceleration_rads2;
			this.cached_max_angular_velocity_rads2 = new_max_angular_velocity_rads2;
			this._propulsionValuesDirty = true;
		}

		// Token: 0x0600613D RID: 24893 RVA: 0x002DACE4 File Offset: 0x002D8EE4
		public void UpdatePropulsionValues()
		{
			this._waypointSharedData.UpdatePropulsionValues(this.cached_acceleration, this.cached_cruise_acceleration, this.cached_angular_acceleration_rads2, this.cached_max_angular_velocity_rads2);
			this._accelerationConstraints.UpdateAccelerationConstraits(this.cached_acceleration, this.cached_cruise_acceleration, this.cached_angular_acceleration_rads2, this.cached_max_angular_velocity_rads2);
			for (int i = 0; i < this._waypoints.Length; i++)
			{
				this._waypoints[i].UpdateAccelerationConstraints(this._accelerationConstraints);
			}
		}

		// Token: 0x0600613E RID: 24894 RVA: 0x002DAD60 File Offset: 0x002D8F60
		public void SetAppendWaypointRotation(Quaternion rotation)
		{
			this._appendWaypointRotation *= rotation;
			Quaternion[] array = new Quaternion[this._waypoints.Length];
			for (int i = 0; i < this._waypoints.Length; i++)
			{
				array[i] = this._waypoints[i].Rotation;
			}
			for (int j = 0; j < this._waypoints.Length; j++)
			{
				if (!this._waypoints[j].IsInputLocked && !this._waypoints[j].IsRecursivelyLocked)
				{
					Quaternion quaternion = array[j];
					int num = 0;
					while (j > num)
					{
						quaternion *= rotation;
						num++;
					}
					this._waypoints[j].AdjustRotation(quaternion, null);
				}
			}
		}

		// Token: 0x0600613F RID: 24895 RVA: 0x002DAE18 File Offset: 0x002D9018
		public void ProposePath(TIDateTime currentTime, Vector3[] path, ProposedWaypoint target, AccelerationConstraints constraints)
		{
			if (constraints == null)
			{
				constraints = this._accelerationConstraints;
			}
			float linearAcceleration = constraints.LinearAcceleration;
			if (this._canWaypointsBeAdjusted)
			{
				if (path == null || path.Length <= 1)
				{
					this._waypoints[0].ProposeWaypoint(target, constraints);
					return;
				}
				int num = 1;
				float num2 = 0f;
				Vector3 vector = this.VelocityAtTime(currentTime);
				AdjustableWaypoint adjustableWaypoint = this._waypoints[1];
				float num3 = 0f;
				for (int i = 1; i < path.Length; i++)
				{
					num3 += Vector3.Distance(path[i - 1], path[i]);
					if (num3 >= linearAcceleration)
					{
						float num4 = num3 / vector.magnitude;
						if (target.Velocity.magnitude > vector.magnitude)
						{
							num4 -= PhysicsHelpers.DisplacementFromAccelerationAndTime(constraints.LinearAcceleration, num4);
						}
						num2 += num4;
						float num5 = this._waypointSharedData.WaypointTimeDelta * (float)num;
						if (num2 - num5 >= 0f)
						{
							adjustableWaypoint.ProposePlacement(path[i], constraints, false, -1f);
							Utilities.DebugDrawPoint(adjustableWaypoint.Position, 1f, Color.green, 5f);
							num++;
							if (num >= this._waypoints.Length - 1)
							{
								break;
							}
							adjustableWaypoint = this._waypoints[num];
						}
						num3 = 0f;
					}
				}
			}
		}

		// Token: 0x06006140 RID: 24896 RVA: 0x002DAF68 File Offset: 0x002D9168
		public void ProposeWaypoint(ProposedWaypoint proposed)
		{
			this._waypoints[0].ProposeWaypoint(proposed, null);
		}

		// Token: 0x06006141 RID: 24897 RVA: 0x002DAF7A File Offset: 0x002D917A
		public void ProposeRotation(Quaternion newRotation)
		{
			this._waypoints[1].AdjustRotation(newRotation, null);
		}

		// Token: 0x06006142 RID: 24898 RVA: 0x002DAF8C File Offset: 0x002D918C
		public void ProposePlacement(Vector3 position)
		{
			this._waypoints[1].ProposePlacement(position, null, false, -1f);
		}

		// Token: 0x06006143 RID: 24899 RVA: 0x002DAFA4 File Offset: 0x002D91A4
		public void ResetWaypoints()
		{
			this._waypoints[1].ResetCurrentWaypointSequence();
		}

		// Token: 0x06006144 RID: 24900 RVA: 0x002DAFB4 File Offset: 0x002D91B4
		public bool IsEffectivelyStopped()
		{
			return this._waypoints[0].VelocityAt(this._waypoints[0].Timing).sqrMagnitude < this._waypointSharedData.LinearAcceleration * this._waypointSharedData.LinearAcceleration;
		}

		// Token: 0x06006145 RID: 24901 RVA: 0x002DAFFC File Offset: 0x002D91FC
		public void AllStop(AccelerationConstraints accelerationConstraints)
		{
			if (this._isOutOfCombatDV)
			{
				this._shipState.RemoveCombatManeuver(CombatManeuver.AllStop);
				this._allStopCalculatedThisCycle = false;
				this.AllStopEnabled = false;
				return;
			}
			if (this._allStopCalculatedThisCycle)
			{
				return;
			}
			if (this.IsEffectivelyStopped())
			{
				this._shipState.RemoveCombatManeuver(CombatManeuver.AllStop);
				this._allStopCalculatedThisCycle = false;
				this.AllStopEnabled = false;
				return;
			}
			this._waypoints[1].ResetCurrentWaypointSequence();
			for (int i = 1; i < this._waypoints.Length; i++)
			{
				Vector3 vector = this._waypoints[i - 1].VelocityAt(this._waypoints[i - 1].Timing);
				Quaternion quaternion = Quaternion.LookRotation(-vector);
				float num = -vector.magnitude / (-1f * this._waypointSharedData.LinearAcceleration);
				float num2 = 0f;
				if (Math.Abs(PhysicsHelpers.RadianAngleBetweenVectors(this._waypoints[i - 1].Rotation * Vector3.forward, quaternion * Vector3.forward)) > 1E-45f)
				{
					float num3 = WaypointTrajectorySequence.TimeRequiredForHeadingRotation(this._waypoints[i - 1].Rotation, quaternion, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity);
					float num4 = this._waypointSharedData.MaxAngularVelocity / this._waypointSharedData.AngularAccelerationRads;
					float num5 = Mathf.Max(num3 - num4, 0f) * 2f;
					num2 = num4 + num5;
				}
				float num6 = num + num2;
				float num7 = (float)(this._waypoints[i].Timing - this._waypoints[i - 1].Timing).TotalSeconds;
				if (num6 < num7)
				{
					Vector3 vector2 = PhysicsHelpers.PositionFromVelocityAndTime(this._waypoints[i - 1].Position, vector, num2);
					Vector3 vector3;
					if (vector.magnitude / (num7 - num2) > accelerationConstraints.CruiseLinearAcceleration)
					{
						vector3 = vector2 + vector * (num7 - num2) / 2f;
					}
					else
					{
						vector3 = vector2 + vector * vector.magnitude / (2f * accelerationConstraints.CruiseLinearAcceleration);
					}
					this._waypoints[i].ProposePlacement(vector3, accelerationConstraints, false, -1f);
					this._allStopCalculatedThisCycle = true;
					this.AllStopEnabled = true;
					break;
				}
				this._waypoints[i].ProposePlacement(this._waypoints[i - 1].Position, accelerationConstraints, false, -1f);
			}
			this._allStopCalculatedThisCycle = true;
			this.AllStopEnabled = true;
		}

		// Token: 0x06006146 RID: 24902 RVA: 0x002DB262 File Offset: 0x002D9462
		public void CancelAllStop()
		{
			this._allStopCalculatedThisCycle = false;
			this.AllStopEnabled = false;
		}

		// Token: 0x06006147 RID: 24903 RVA: 0x002DB274 File Offset: 0x002D9474
		public void SetBreakingTrajectory(AccelerationConstraints accelerationConstraints)
		{
			if (this._isOutOfCombatDV || this.IsEffectivelyStopped())
			{
				return;
			}
			this._waypoints[1].ResetCurrentWaypointSequence();
			for (int i = 1; i < this._waypoints.Length; i++)
			{
				Vector3 vector = this._waypoints[i - 1].VelocityAt(this._waypoints[i - 1].Timing);
				Quaternion quaternion = Quaternion.LookRotation(-vector);
				float num = -vector.magnitude / (-1f * this._waypointSharedData.LinearAcceleration);
				float num2 = 0f;
				if (Math.Abs(PhysicsHelpers.RadianAngleBetweenVectors(this._waypoints[i - 1].Rotation * Vector3.forward, quaternion * Vector3.forward)) > 1E-45f)
				{
					float num3 = WaypointTrajectorySequence.TimeRequiredForHeadingRotation(this._waypoints[i - 1].Rotation, quaternion, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity);
					float num4 = this._waypointSharedData.MaxAngularVelocity / this._waypointSharedData.AngularAccelerationRads;
					float num5 = Mathf.Max(num3 - num4, 0f) * 2f;
					num2 = num4 + num5;
				}
				float num6 = num + num2;
				float num7 = (float)(this._waypoints[i].Timing - this._waypoints[i - 1].Timing).TotalSeconds;
				if (num6 < num7)
				{
					Vector3 vector2 = PhysicsHelpers.PositionFromVelocityAndTime(this._waypoints[i - 1].Position, vector, num2) + -1f * vector.sqrMagnitude * -1f * vector.normalized / (2f * this._waypointSharedData.LinearAcceleration);
					this._waypoints[i].ProposePlacement(vector2, accelerationConstraints, false, -1f);
					return;
				}
				this._waypoints[i].ProposePlacement(this._waypoints[i - 1].Position, accelerationConstraints, false, -1f);
			}
		}

		// Token: 0x06006148 RID: 24904 RVA: 0x002DB45C File Offset: 0x002D965C
		public void FullSpeedAhead(AccelerationConstraints accelerationConstraints)
		{
			if (!this._isOutOfCombatDV)
			{
				for (int i = 1; i < this._waypoints.Length; i++)
				{
					float num = ((accelerationConstraints == null) ? this._waypointSharedData.LinearAcceleration : accelerationConstraints.LinearAcceleration);
					Vector3 vector = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._waypoints[i].Heading.normalized, num, GameControl.spaceCombat.waypointTimeDelta);
					this._waypoints[i].ProposePlacement(this._waypoints[i].Position + vector, accelerationConstraints, false, -1f);
				}
			}
			if (this._shipState.activeCombatManeuvers.Contains(CombatManeuver.FullSpeedAhead))
			{
				this._shipState.RemoveCombatManeuver(CombatManeuver.FullSpeedAhead);
			}
		}

		// Token: 0x06006149 RID: 24905 RVA: 0x002DB50C File Offset: 0x002D970C
		public void BurnAlongCurrentVelocity(AccelerationConstraints accelerationConstraints)
		{
			for (int i = 1; i < this._waypoints.Length; i++)
			{
				Vector3 vector = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._waypoints[0].Velocity.normalized, this._waypointSharedData.LinearAcceleration, GameControl.spaceCombat.waypointTimeDelta);
				this._waypoints[i].ProposePlacement(this._waypoints[i].Position + vector, accelerationConstraints, false, -1f);
			}
		}

		// Token: 0x0600614A RID: 24906 RVA: 0x002DB584 File Offset: 0x002D9784
		public void InterceptCourse(AccelerationConstraints accelerationConstraints)
		{
			if (this._primaryTarget != null && !this._primaryTarget.destructionTriggered && !this._isOutOfCombatDV)
			{
				this.ResetWaypoints();
				for (int i = 1; i < this._waypoints.Length; i++)
				{
					if (!this._waypoints[i].IsRecursivelyLocked && !this._waypoints[i].IsInputLocked)
					{
						if (this._waypointControllers[this._waypoints[i]].IsSystemFailureLocked)
						{
							break;
						}
						Vector3 vector = this._primaryTarget.positionAtTime(this._waypoints[5].Timing.ExportTime());
						this._waypoints[i].ProposeHeading((vector - this._waypoints[i].Position).normalized);
						Vector3 vector2 = PhysicsHelpers.DisplacementFromAccelerationAndTime(this._waypoints[i].Heading.normalized, this._waypointSharedData.LinearAcceleration, GameControl.spaceCombat.waypointTimeDelta);
						this._waypoints[i].ProposePlacement(this._waypoints[i].Position + vector2, accelerationConstraints, false, -1f);
					}
				}
			}
			if (this._shipState.activeCombatManeuvers.Contains(CombatManeuver.InterceptCourse))
			{
				this._shipState.RemoveCombatManeuver(CombatManeuver.InterceptCourse);
			}
		}

		// Token: 0x0600614B RID: 24907 RVA: 0x002DB6E0 File Offset: 0x002D98E0
		public void MatchVelocity()
		{
			if (this._maneuverTarget != null && !this._maneuverTarget.destructionTriggered)
			{
				if (this._isOutOfCombatDV)
				{
					this._shipState.RemoveCombatManeuver(CombatManeuver.MatchVelocity);
					this._matchVelocityCalculatedThisCycle = false;
					this.MatchVelocityEnabled = false;
					return;
				}
				if (this._matchVelocityCalculatedThisCycle)
				{
					return;
				}
				Vector3 vector = ((this._maneuverTarget.ref_shipController != null) ? this._maneuverTarget.ref_shipController.velocityAtTime(this._waypoints[0].Timing.ExportTime()) : this._maneuverTarget.ref_habModuleController.velocityVector);
				if (this._waypoints[0].VelocityAt(this._waypoints[0].Timing) == vector)
				{
					this._shipState.RemoveCombatManeuver(CombatManeuver.MatchVelocity);
					this._matchVelocityCalculatedThisCycle = false;
					this.MatchVelocityEnabled = false;
					return;
				}
				this._waypoints[1].ResetCurrentWaypointSequence();
				for (int i = 1; i < this._waypoints.Length; i++)
				{
					if (!this._waypoints[i].IsRecursivelyLocked && !this._waypoints[i].IsInputLocked)
					{
						if (this._waypointControllers[this._waypoints[i]].IsSystemFailureLocked)
						{
							break;
						}
						Vector3 vector2 = vector - this._waypoints[i - 1].Velocity;
						Quaternion quaternion = Quaternion.LookRotation(vector2);
						float num = (vector - this._waypoints[i - 1].Velocity).magnitude / this._waypointSharedData.LinearAcceleration;
						float num2 = 0f;
						if (Math.Abs(PhysicsHelpers.RadianAngleBetweenVectors(this._waypoints[i - 1].Rotation * Vector3.forward, quaternion * Vector3.forward)) > 1E-45f)
						{
							float num3 = RotationTrajectory.TopAngularVelocityForHeadingRotationLimitedByTime(this._waypoints[i - 1].Rotation, quaternion, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity, GameControl.spaceCombat.waypointTimeDelta);
							float num4 = RotationTrajectory.TimeRequiredForHeadingRotationLimitedByTime(this._waypoints[i - 1].Rotation, quaternion, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity, GameControl.spaceCombat.waypointTimeDelta);
							float num5 = num3 / this._waypointSharedData.AngularAccelerationRads;
							float num6 = Mathf.Max(num4 - num5, 0f) * 2f;
							num2 = num5 + num6;
						}
						float num7 = num + num2;
						float waypointTimeDelta = GameControl.spaceCombat.waypointTimeDelta;
						if (num7 < waypointTimeDelta)
						{
							Vector3 vector3 = PhysicsHelpers.PositionFromVelocityAndTime(this._waypoints[i - 1].Position, this._waypoints[i - 1].Velocity, num2);
							Vector3 vector4;
							if (vector2.magnitude / (waypointTimeDelta - num2) > this._waypointSharedData.CruiseAcceleration)
							{
								vector4 = PhysicsHelpers.PositionFromVelocityAndTime(this._waypoints[i - 1].Position, this._waypoints[i - 1].Velocity, waypointTimeDelta) + vector2 * (waypointTimeDelta - num2) / 2f;
							}
							else
							{
								float num8 = (vector - this._waypoints[i - 1].Velocity).magnitude / this._waypointSharedData.CruiseAcceleration;
								float num9 = num8 + num2;
								vector4 = vector3 + this._waypoints[i - 1].Velocity * num8 + 0.5f * vector2 * num8 + vector * (waypointTimeDelta - num9);
							}
							this._waypoints[i].ProposePlacement(vector4, null, false, -1f);
							return;
						}
						Vector3 vector5 = PhysicsHelpers.DisplacementFromAccelerationAndTime(vector2.normalized, this._waypointSharedData.LinearAcceleration, GameControl.spaceCombat.waypointTimeDelta);
						this._waypoints[i].ProposePlacement(this._waypoints[i].Position + vector5, null, false, -1f);
						this._matchVelocityCalculatedThisCycle = true;
						this.MatchVelocityEnabled = true;
					}
				}
			}
		}

		// Token: 0x0600614C RID: 24908 RVA: 0x002DBACD File Offset: 0x002D9CCD
		public void CancelMatchVelocity()
		{
			this._matchVelocityCalculatedThisCycle = false;
			this.MatchVelocityEnabled = false;
		}

		// Token: 0x0600614D RID: 24909 RVA: 0x002DBADD File Offset: 0x002D9CDD
		public void BeginDefensiveManeuvers()
		{
			this.DefensiveManueversEnabled = true;
		}

		// Token: 0x0600614E RID: 24910 RVA: 0x002DBAE6 File Offset: 0x002D9CE6
		public void CancelDefensiveManeuvers()
		{
			this.DefensiveManueversEnabled = false;
		}

		// Token: 0x0600614F RID: 24911 RVA: 0x002DBAF0 File Offset: 0x002D9CF0
		public bool RotateToFaceTarget()
		{
			if (this._canWaypointsBeAdjusted && !this._isOutOfCombatDV && this._primaryTarget != null && !this._primaryTarget.destructionTriggered)
			{
				for (int i = 1; i < this._waypoints.Length; i++)
				{
					if (!this._waypoints[i].IsRecursivelyLocked && !this._waypoints[i].IsInputLocked)
					{
						if (this._waypointControllers[this._waypoints[i]].IsSystemFailureLocked)
						{
							return false;
						}
						if (i == 1)
						{
							this.ResetWaypoints();
						}
						Vector3 vector = this._primaryTarget.positionAtTime(this._waypoints[i].Timing.ExportTime());
						this._waypoints[i].ProposeHeading((vector - this._waypoints[i].Position).normalized);
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x06006150 RID: 24912 RVA: 0x002DBBDC File Offset: 0x002D9DDC
		public void FaceVelocityVector()
		{
			if (this._canWaypointsBeAdjusted)
			{
				for (int i = 1; i < this._waypoints.Length; i++)
				{
					if (!this._waypoints[i].IsRecursivelyLocked && !this._waypoints[i].IsInputLocked)
					{
						if (this._waypointControllers[this._waypoints[i]].IsSystemFailureLocked)
						{
							return;
						}
						Vector3 normalized = this._waypoints[i].Velocity.normalized;
						this._waypoints[i].ProposeHeading(normalized);
					}
				}
			}
			if (this._shipState.activeCombatManeuvers.Contains(CombatManeuver.FaceVelocityVector))
			{
				this._shipState.RemoveCombatManeuver(CombatManeuver.FaceVelocityVector);
			}
		}

		// Token: 0x06006151 RID: 24913 RVA: 0x002DBC84 File Offset: 0x002D9E84
		public void MatchRelativeTrajectory(WaypointNavigationController controllerToMatch, AccelerationConstraints constraints, out bool hasMatchedTrajectory)
		{
			hasMatchedTrajectory = false;
			float num = 1f;
			int num2 = 1;
			while (controllerToMatch._waypoints.Length > num2)
			{
				Vector3 vector = controllerToMatch._waypoints[num2].VelocityAt(controllerToMatch._waypoints[num2].Timing);
				if (this._waypoints[num2].VelocityAt(this._waypoints[num2].Timing).sqrMagnitude - vector.sqrMagnitude < 1E-05f)
				{
					if (Quaternion.Angle(controllerToMatch._waypoints[num2].Rotation, this._waypoints[num2].Rotation) < num)
					{
						hasMatchedTrajectory = true;
						return;
					}
					this._waypoints[num2].ProposeRotation(controllerToMatch._waypoints[num2].Rotation, constraints);
				}
				else
				{
					if (num2 == 1)
					{
						this._waypoints[num2].ResetCurrentWaypointSequence();
					}
					Vector3 vector2 = vector - this._waypoints[num2 - 1].Velocity;
					Quaternion quaternion = Quaternion.LookRotation(vector2);
					float num3 = (vector - this._waypoints[num2 - 1].Velocity).magnitude / this._waypointSharedData.LinearAcceleration;
					float num4 = 0f;
					if (Math.Abs(PhysicsHelpers.RadianAngleBetweenVectors(this._waypoints[num2 - 1].Rotation * Vector3.forward, quaternion * Vector3.forward)) > 1E-45f)
					{
						float num5 = RotationTrajectory.TopAngularVelocityForHeadingRotationLimitedByTime(this._waypoints[num2 - 1].Rotation, quaternion, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity, GameControl.spaceCombat.waypointTimeDelta);
						float num6 = RotationTrajectory.TimeRequiredForHeadingRotationLimitedByTime(this._waypoints[num2 - 1].Rotation, quaternion, this._waypointSharedData.AngularAccelerationRads, this._waypointSharedData.MaxAngularVelocity, GameControl.spaceCombat.waypointTimeDelta);
						float num7 = num5 / this._waypointSharedData.AngularAccelerationRads * 2f;
						float num8 = Mathf.Max(num6 - num7, 0f);
						num4 = num7 + num8;
					}
					float num9 = num3 + num4;
					if (num9 < GameControl.spaceCombat.waypointTimeDelta)
					{
						Vector3 vector3 = PhysicsHelpers.PositionFromVelocityAndTime(this._waypoints[num2 - 1].Position, this._waypoints[num2 - 1].Velocity, num4);
						Vector3 vector4 = vector2.normalized * this._waypointSharedData.LinearAcceleration;
						float num10 = ((Mathf.Abs(vector4.x) > 0f) ? ((vector.x * vector.x - this._waypoints[num2 - 1].Velocity.x * this._waypoints[num2 - 1].Velocity.x) / (2f * vector4.x)) : 0f);
						float num11 = ((Mathf.Abs(vector4.y) > 0f) ? ((vector.y * vector.y - this._waypoints[num2 - 1].Velocity.y * this._waypoints[num2 - 1].Velocity.y) / (2f * vector4.y)) : 0f);
						float num12 = ((Mathf.Abs(vector4.z) > 0f) ? ((vector.z * vector.z - this._waypoints[num2 - 1].Velocity.z * this._waypoints[num2 - 1].Velocity.z) / (2f * vector4.z)) : 0f);
						Vector3 vector5 = vector3 + new Vector3(num10, num11, num12);
						float num13 = PhysicsHelpers.DisplacementFromVelocityAndTime(vector.magnitude, GameControl.spaceCombat.waypointTimeDelta - num9);
						Vector3 vector6 = vector5 + vector.normalized * num13;
						Utilities.DebugDrawSphere(vector6, Quaternion.identity, 0.15f, Color.green, 4, 10f);
						this._waypoints[num2].ProposePlacement(vector6, null, false, -1f);
					}
					else
					{
						Vector3 vector7 = PhysicsHelpers.DisplacementFromAccelerationAndTime(vector2.normalized, this._waypointSharedData.LinearAcceleration, GameControl.spaceCombat.waypointTimeDelta);
						Utilities.DebugDrawSphere(this._waypoints[num2].Position + vector7, Quaternion.identity, 0.15f, Color.yellow, 4, 10f);
						this._waypoints[num2].ProposePlacement(this._waypoints[num2].Position + vector7, null, false, -1f);
					}
				}
				num2++;
			}
		}

		// Token: 0x06006152 RID: 24914 RVA: 0x002DC0E4 File Offset: 0x002DA2E4
		public void FollowControllerTrajectory(WaypointNavigationController controllerToMatch, AccelerationConstraints constraints)
		{
			float num = 0.06f;
			float num2 = 1f;
			this.ResetWaypoints();
			int num3 = 1;
			while (controllerToMatch._waypoints.Length > num3)
			{
				Vector3 vector = controllerToMatch._waypoints[num3].Position - controllerToMatch._waypoints[num3 - 1].Position;
				Vector3 vector2 = this._waypoints[num3].Position - this._waypoints[num3 - 1].Position;
				Vector3 vector3 = vector - vector2;
				float num4 = Quaternion.Angle(controllerToMatch._waypoints[num3].Rotation, this._waypoints[num3].Rotation);
				if (vector3.sqrMagnitude > num * num)
				{
					Utilities.DebugDrawSphere(this._waypoints[num3].Position + vector3, Quaternion.identity, 0.15f, Color.yellow, 4, 10f);
					this._waypoints[num3].ProposePlacement(this._waypoints[num3].Position + vector3, constraints, false, -1f);
				}
				if (num4 > num2)
				{
					this._waypoints[num3].ProposeRotation(controllerToMatch._waypoints[num3].Rotation, constraints);
				}
				num3++;
			}
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x002DC208 File Offset: 0x002DA408
		private void RedrawWaypointPath(TIDateTime timingCutoff, Camera cam, Vector3 shipPosition)
		{
			this._waypoints[0].UpdatePathRender(timingCutoff, cam, shipPosition);
		}

		// Token: 0x06006154 RID: 24916 RVA: 0x002DC21A File Offset: 0x002DA41A
		private void HandleOnWaypointReadyForInput(WaypointController controller)
		{
			if (this.CanBeginInputHandling())
			{
				this._spaceCombatCameraController.IsCameraMovementBlocked = true;
				this.BeginInputHandling(controller);
			}
		}

		// Token: 0x06006155 RID: 24917 RVA: 0x002DC237 File Offset: 0x002DA437
		private bool CanBeginInputHandling()
		{
			return WaypointNavigationController._activeInputHandlingController == null;
		}

		// Token: 0x06006156 RID: 24918 RVA: 0x002DC241 File Offset: 0x002DA441
		private void BeginInputHandling(WaypointController controller)
		{
			WaypointNavigationController._activeInputHandlingController = controller;
			WaypointNavigationController._activeInputHandlingController.BeginHandleInput();
			this.ToggleHeightLines(true);
			this.GetDVCost(this._shipState.AvailableDeltaVForCombat_kps());
		}

		// Token: 0x06006157 RID: 24919 RVA: 0x002DC26C File Offset: 0x002DA46C
		public void ToggleHeightLines(bool shouldShow)
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.ToggleHeightLine(shouldShow);
			}
		}

		// Token: 0x06006158 RID: 24920 RVA: 0x002DC2C8 File Offset: 0x002DA4C8
		public void ToggleDVCost(bool shouldShow)
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.ToggleWaypointDVCost(shouldShow);
			}
		}

		// Token: 0x06006159 RID: 24921 RVA: 0x002DC324 File Offset: 0x002DA524
		private void HandleOnWaypointEndingInput(WaypointController controller)
		{
			if (this.CanEndInputHandling(controller))
			{
				this.EndInputHandling();
			}
		}

		// Token: 0x0600615A RID: 24922 RVA: 0x002DC335 File Offset: 0x002DA535
		private bool CanEndInputHandling(WaypointController controller)
		{
			return WaypointNavigationController._activeInputHandlingController == controller;
		}

		// Token: 0x0600615B RID: 24923 RVA: 0x002DC340 File Offset: 0x002DA540
		private void EndInputHandling()
		{
			this.ToggleHeightLines(false);
			this._spaceCombatCameraController.IsCameraMovementBlocked = false;
			WaypointController activeInputHandlingController = WaypointNavigationController._activeInputHandlingController;
			if (activeInputHandlingController != null)
			{
				activeInputHandlingController.EndHandleInput();
			}
			WaypointController activeInputHandlingController2 = WaypointNavigationController._activeInputHandlingController;
			if (activeInputHandlingController2 != null)
			{
				activeInputHandlingController2.SetInputHandling(false);
			}
			WaypointNavigationController._activeInputHandlingController = null;
			if (!Input.GetMouseButton(0) && this.eventInstance.isValid())
			{
				this.eventInstance.Stop(STOP_MODE.IMMEDIATE);
			}
		}

		// Token: 0x0600615C RID: 24924 RVA: 0x002DC3AC File Offset: 0x002DA5AC
		private void HandleOnWaypointRemovalRequested(AdjustableWaypoint adjustableWaypoint)
		{
			for (int i = 0; i < this._waypoints.Length; i++)
			{
				if (this._waypoints[i].UID == adjustableWaypoint.UID)
				{
					this._waypointControllers.Remove(this._waypoints[i]);
					this._waypoints[i] = null;
					for (int j = i + 1; j < this._waypoints.Length; j++)
					{
						this._waypoints[j - 1] = this._waypoints[j];
					}
				}
			}
			Array.Resize<AdjustableWaypoint>(ref this._waypoints, this._waypoints.Length - 1);
		}

		// Token: 0x0600615D RID: 24925 RVA: 0x002DC43C File Offset: 0x002DA63C
		private void OnPreRenderCallback(Camera cam)
		{
			if (cam != this._mainCamera)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			if (this._thisCombatant != null)
			{
				vector = this._thisCombatant.position;
			}
			this.RedrawWaypointPath(this._gameTime.currentTime, cam, vector);
		}

		// Token: 0x0600615E RID: 24926 RVA: 0x002DC48C File Offset: 0x002DA68C
		public void UpdateWaypointNavigation(TIDateTime currentTime)
		{
			float num = this._shipState.AvailableDeltaVForCombat_kps();
			if (this.HasNextWaypointBeenReached(currentTime))
			{
				this.AdvanceWaypoints();
			}
			else
			{
				bool flag = num <= 0f && !this._isOutOfCombatDV;
				bool flag2 = this._waypoints[0].IsInBurn(currentTime) && this._shipState.DoesDriveHeatExceedRadiatorAndOverheatInOneSecond();
				if (flag || flag2)
				{
					float num2 = (float)(this._waypoints[0].Timing - currentTime).TotalSeconds;
					Vector3 vector = this.VelocityAtTime(currentTime) * num2 + this.PositionAtTime(currentTime);
					this._waypoints[0].ProposePlacement(vector, null, false, this._waypoints[0].LinearAcceleration());
					if (flag)
					{
						this.ToggleControllerIsSystemFailureLocked(true);
					}
				}
				else if (this._canWaypointsBeAdjusted && this._primaryTarget != null && !this._primaryTarget.destructionTriggered && this.PadlockEnabled)
				{
					for (int i = 1; i < this._waypoints.Length; i++)
					{
						if (!this._waypoints[i].IsRecursivelyLocked && !this._waypoints[i].IsInputLocked)
						{
							if (this._waypointControllers[this._waypoints[i]].IsSystemFailureLocked)
							{
								this.PadlockEnabled = false;
								break;
							}
							Vector3 vector2 = this._primaryTarget.positionAtTime(this._waypoints[i].Timing.ExportTime());
							this._waypoints[i].ProposeHeading((vector2 - this._waypoints[i].Position).normalized);
						}
					}
				}
				this.UpdateWaypoints();
				this.GetDVCost(num);
			}
			this.ClearWaypointCollisionWarnings();
			if (num > 0f)
			{
				this.UpdateWaypointEngineEffectivenessState();
				this.UpdateWaypointAdjustmentState();
				if (this._canWaypointsBeAdjusted)
				{
					if (this.EnrouteIntentionalCollision)
					{
						new TIDateTime(currentTime).AddSeconds((double)this._waypointSharedData.WaypointTimeDelta);
						for (int j = 1; j < this._waypoints.Length; j++)
						{
							Vector3 vector3 = this._primaryTarget.positionAtTime(this._waypoints[j].Timing.ExportTime());
							this._waypoints[j].ProposePlacement(vector3, null, false, -1f);
						}
					}
					else
					{
						if (this._waypointControllers.Any<KeyValuePair<AdjustableWaypoint, WaypointController>>((KeyValuePair<AdjustableWaypoint, WaypointController> x) => x.Value.IsHandlingMovementInput))
						{
							this.TimeOfCollisionPassed = null;
						}
						if (this.TimeOfCollisionPassed != null && !this._recalculateAvoidancePath && this.TimeOfCollisionPassed < currentTime)
						{
							this._recalculateAvoidancePath = true;
						}
						if (this._recalculateAvoidancePath && this._agentShipControllers.Count > 0)
						{
							if (this.TimeOfNextCollisionCheck == null)
							{
								bool flag3;
								this.CheckForCollisions(currentTime, out flag3);
								if (!flag3)
								{
									this.TimeOfNextCollisionCheck = new TIDateTime(currentTime);
									this.TimeOfNextCollisionCheck.AddSeconds((double)((int)this._shipState.ID % 10));
								}
							}
							else if (this.TimeOfNextCollisionCheck <= currentTime)
							{
								bool flag4;
								this.CheckForCollisions(currentTime, out flag4);
								if (!flag4)
								{
									this.TimeOfNextCollisionCheck.AddSeconds(10.0);
								}
							}
							else if (this.TimeOfCollisionPassed == null)
							{
								this.TimeOfNextCollisionCheck = new TIDateTime(currentTime);
								this.TimeOfNextCollisionCheck.AddSeconds((double)(-10 + (int)this._shipState.ID % 10));
							}
						}
					}
					if (this.TimeOfCollisionPassed != null && this.TimeOfCollisionPassed < currentTime)
					{
						this.TimeOfCollisionPassed = null;
						GameControl.eventManager.TriggerEvent(new CombatCollisionAvoidanceStatusChange(this._shipState), null, new object[] { this._shipState });
					}
				}
			}
			this.ShowNeededWaypointCollisionWarnings();
		}

		// Token: 0x0600615F RID: 24927 RVA: 0x002DC86C File Offset: 0x002DAA6C
		private void ClearWaypointCollisionWarnings()
		{
			AdjustableWaypoint[] waypoints = this._waypoints;
			for (int i = 0; i < waypoints.Length; i++)
			{
				waypoints[i].CollisionWarningNeeded = false;
			}
		}

		// Token: 0x06006160 RID: 24928 RVA: 0x002DC898 File Offset: 0x002DAA98
		private void ShowNeededWaypointCollisionWarnings()
		{
			foreach (AdjustableWaypoint adjustableWaypoint in this._waypoints)
			{
				WaypointController waypointController;
				if (this._waypointControllers.TryGetValue(adjustableWaypoint, out waypointController))
				{
					WaypointVisual visual = waypointController._visual;
					if (visual != null)
					{
						WaypointUIController waypointUI = visual.waypointUI;
						if (waypointUI != null)
						{
							waypointUI.SetCollisionWarningFlag(adjustableWaypoint.CollisionWarningNeeded);
						}
					}
					if (visual != null)
					{
						WaypointUIController waypointUI2 = visual.waypointUI;
						if (waypointUI2 != null)
						{
							waypointUI2.ToggleCollisionWarning(adjustableWaypoint.CollisionWarningNeeded);
						}
					}
				}
			}
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x002DC910 File Offset: 0x002DAB10
		private void UpdateWaypointEngineEffectivenessState()
		{
			float thrustEffectivenessRatio = this._shipState.ThrustEffectivenessRatio;
			bool flag = false;
			if (thrustEffectivenessRatio > 0.5f && this._accelerationEffectiveness != WaypointNavigationController.AccelerationEffectiveness.FULL_POWER)
			{
				flag = true;
				this._accelerationEffectiveness = WaypointNavigationController.AccelerationEffectiveness.FULL_POWER;
				this._accelerationEffectivenessRatio = 1f;
				this.ToggleControllerIsSystemFailureLocked(!this._canWaypointsBeAdjusted);
			}
			else if (thrustEffectivenessRatio > 0f && thrustEffectivenessRatio < 0.5f && this._accelerationEffectiveness != WaypointNavigationController.AccelerationEffectiveness.HALF_POWER)
			{
				flag = true;
				this._accelerationEffectiveness = WaypointNavigationController.AccelerationEffectiveness.HALF_POWER;
				this._accelerationEffectivenessRatio = 0.5f;
				this.ToggleControllerIsSystemFailureLocked(!this._canWaypointsBeAdjusted);
			}
			else if (thrustEffectivenessRatio == 0f && this._accelerationEffectiveness != WaypointNavigationController.AccelerationEffectiveness.DISABLED)
			{
				flag = true;
				this._accelerationEffectiveness = WaypointNavigationController.AccelerationEffectiveness.DISABLED;
				this._accelerationEffectivenessRatio = 0f;
				this.ToggleControllerIsSystemFailureLocked(!this._canWaypointsBeAdjusted);
			}
			if (flag)
			{
				GameControl.spaceCombat.EndWaypointPlacementHandling();
				this.EndInputHandling();
				this._waypoints[0].RecalculateTrajectoryPathRecursive();
			}
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x002DC9F4 File Offset: 0x002DABF4
		private void UpdateWaypointAdjustmentState()
		{
			this._canWaypointsBeAdjusted = this._shipState.CanSetWaypoints();
			if (this._accelerationEffectiveness == WaypointNavigationController.AccelerationEffectiveness.DISABLED || this._canWaypointsBeAdjusted)
			{
				return;
			}
			if (!this._canWaypointsBeAdjusted)
			{
				GameControl.spaceCombat.EndWaypointPlacementHandling();
				foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
				{
					if (this.CanEndInputHandling(keyValuePair.Value))
					{
						this.EndInputHandling();
					}
				}
			}
			this.ToggleControllerIsSystemFailureLocked(!this._canWaypointsBeAdjusted);
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x002DCA98 File Offset: 0x002DAC98
		private void ToggleControllerIsSystemFailureLocked(bool isSystemFailureLocked)
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.IsSystemFailureLocked = isSystemFailureLocked;
			}
			if (isSystemFailureLocked)
			{
				this.ResetWaypoints();
			}
		}

		// Token: 0x06006164 RID: 24932 RVA: 0x002DCAFC File Offset: 0x002DACFC
		private void SetControllerIsDvLocked(bool isDvLocked)
		{
			this._isOutOfCombatDV = isDvLocked;
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.IsDvLocked = isDvLocked;
			}
		}

		// Token: 0x06006165 RID: 24933 RVA: 0x002DCB5C File Offset: 0x002DAD5C
		private bool HasNextWaypointBeenReached(TIDateTime currentTime)
		{
			return currentTime > this._waypoints[0].Timing;
		}

		// Token: 0x06006166 RID: 24934 RVA: 0x002DCB71 File Offset: 0x002DAD71
		private void AdvanceWaypoints()
		{
			this.EndInputHandling();
			this.CycleWaypoints();
		}

		// Token: 0x06006167 RID: 24935 RVA: 0x002DCB80 File Offset: 0x002DAD80
		private void CycleWaypoints()
		{
			this.UpdatePropulsionValues();
			Quaternion[] array = new Quaternion[this._waypoints.Length];
			Vector3[] array2 = new Vector3[this._waypoints.Length];
			int num = this._waypoints.Length - 1;
			if (this._propulsionValuesDirty && !this._propulsionValuesImproved)
			{
				for (int i = 0; i < this._waypoints.Length; i++)
				{
					array2[i] = this._waypoints[i].Position;
					array[i] = this._waypoints[i].Rotation;
				}
				int num2 = this._waypoints.Length - 1;
				while (num2 >= 0 && this._waypoints[num2].IsCoastOnly)
				{
					num = num2 - 1;
					num2--;
				}
			}
			AdjustableWaypoint adjustableWaypoint = this._waypoints[0];
			this._initialWaypoint.Timing = new TIDateTime(adjustableWaypoint.Timing);
			this._initialWaypoint.Rotation = adjustableWaypoint.Rotation;
			this._initialWaypoint.Position = adjustableWaypoint.Position;
			this._initialWaypoint.Velocity = adjustableWaypoint.Velocity;
			float alphaBlendValue = this._waypoints[this._waypointCount - 1].AlphaBlendValue;
			for (int j = this._waypointCount - 1; j > 0; j--)
			{
				this._waypoints[j].AlphaBlendValue = this._waypoints[j - 1].AlphaBlendValue;
			}
			for (int k = 1; k < this._waypointCount; k++)
			{
				this._waypoints[k - 1] = this._waypoints[k];
			}
			this._waypoints[0].SetPreviousWaypoint(this._initialWaypoint);
			adjustableWaypoint.AlphaBlendValue = alphaBlendValue;
			adjustableWaypoint.EstablishDesiredPreviousPoint(this._waypoints[this._waypointCount - 1]);
			adjustableWaypoint.IsInputLocked = false;
			Vector3 vector = this._waypoints[this._waypointCount - 1].Velocity * this.WaypointTimeDelta + this._waypoints[this._waypointCount - 1].Position;
			TIDateTime tidateTime = new TIDateTime(this._waypoints[this._waypointCount - 1].Timing);
			tidateTime.AddSeconds((double)this.WaypointTimeDelta);
			adjustableWaypoint.Timing = tidateTime;
			adjustableWaypoint.Velocity = this._waypoints[this._waypointCount - 1].Velocity;
			adjustableWaypoint.Rotation = this._waypoints[this._waypointCount - 1].Rotation;
			this._waypoints[this._waypointCount - 1] = adjustableWaypoint;
			this._waypoints[this._waypointCount - 1].ProposePlacement(vector, tidateTime, null, false, -1f);
			this._waypoints[this._waypointCount - 1].ProposeRotation(this._waypoints[this._waypointCount - 1].Rotation * this._appendWaypointRotation, null);
			if (this._propulsionValuesDirty && !this._propulsionValuesImproved)
			{
				for (int l = 0; l < num; l++)
				{
					this._waypoints[l].ProposePlacement(array2[l + 1], null, false, -1f);
					this._waypoints[l].ProposeRotation(array[l + 1], null);
					this._propulsionValuesDirty = false;
					this._propulsionValuesImproved = false;
				}
			}
			this.ControllerCoreWaypointRotation();
			this._allStopCalculatedThisCycle = false;
			this._matchVelocityCalculatedThisCycle = false;
			GameControl.eventManager.TriggerEvent(new WaypointsCycled(), null, new object[] { this._shipState });
		}

		// Token: 0x06006168 RID: 24936 RVA: 0x002DCED8 File Offset: 0x002DB0D8
		private void ControllerCoreWaypointRotation()
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.RotateColorIndex();
			}
		}

		// Token: 0x06006169 RID: 24937 RVA: 0x002DCF30 File Offset: 0x002DB130
		private void UpdateWaypoints()
		{
			WaypointController activeInputHandlingController = WaypointNavigationController._activeInputHandlingController;
			if (activeInputHandlingController != null)
			{
				activeInputHandlingController.ProcessInput();
			}
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.UpdateVisuals();
			}
		}

		// Token: 0x0600616A RID: 24938 RVA: 0x002DCF98 File Offset: 0x002DB198
		public void ToggleWaypointVisualization()
		{
			this.ToggleWaypointRenderers();
			this.TogglePathRenderer();
		}

		// Token: 0x0600616B RID: 24939 RVA: 0x002DCFA8 File Offset: 0x002DB1A8
		private void ToggleWaypointRenderers()
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.ToggleRenderer();
			}
		}

		// Token: 0x0600616C RID: 24940 RVA: 0x002DD000 File Offset: 0x002DB200
		public void TogglePathRenderer()
		{
			this._allowPathDrawing = !this._allowPathDrawing;
			for (int i = 0; i < this._waypoints.Length; i++)
			{
				this._waypoints[i].RenderTrajectoryLines = this._allowPathDrawing;
			}
		}

		// Token: 0x0600616D RID: 24941 RVA: 0x002DD042 File Offset: 0x002DB242
		public void SetWaypointVisualization(bool setActive)
		{
			this.SetWaypointRenderers(setActive);
			this.SetPathRenderer(setActive);
		}

		// Token: 0x0600616E RID: 24942 RVA: 0x002DD054 File Offset: 0x002DB254
		private void SetWaypointRenderers(bool setActive)
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.SetRenderer(setActive);
			}
		}

		// Token: 0x0600616F RID: 24943 RVA: 0x002DD0B0 File Offset: 0x002DB2B0
		public void SetPathRenderer(bool setActive)
		{
			this._allowPathDrawing = setActive;
			for (int i = 0; i < this._waypoints.Length; i++)
			{
				this._waypoints[i].RenderTrajectoryLines = this._allowPathDrawing;
			}
		}

		// Token: 0x06006170 RID: 24944 RVA: 0x002DD0EC File Offset: 0x002DB2EC
		public void CleanUpWaypoints()
		{
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPreRender, new Camera.CameraCallback(this.OnPreRenderCallback));
			this.EndInputHandling();
			this.ClearAllWaypoints();
			this._waypointContainer.Clear(true);
			this._waypointContainer.gameObject.SetActive(false);
		}

		// Token: 0x06006171 RID: 24945 RVA: 0x002DD144 File Offset: 0x002DB344
		private void ClearAllWaypoints()
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				keyValuePair.Value.Destroy();
			}
			this._waypointControllers.Clear();
		}

		// Token: 0x06006172 RID: 24946 RVA: 0x002DD1A8 File Offset: 0x002DB3A8
		public void ResetLocksRecursive()
		{
			this._waypoints[0].ResetLocksRecursive();
		}

		// Token: 0x06006173 RID: 24947 RVA: 0x002DD1B7 File Offset: 0x002DB3B7
		public HoldTrajectory GetTrajectoryAtTime(TIDateTime time)
		{
			return this.GetNextWaypoint(time).TrajectoryAt(time);
		}

		// Token: 0x06006174 RID: 24948 RVA: 0x002DD1C8 File Offset: 0x002DB3C8
		private AdjustableWaypoint GetNextWaypoint(TIDateTime time)
		{
			for (int i = this._waypointCount - 2; i >= 0; i--)
			{
				if (this._waypoints[i].Timing < time)
				{
					return this._waypoints[i + 1];
				}
			}
			return this._waypoints[0];
		}

		// Token: 0x06006175 RID: 24949 RVA: 0x002DD210 File Offset: 0x002DB410
		public bool IsInBurn(TIDateTime currentTime)
		{
			return this._waypoints[0].IsInBurn(currentTime);
		}

		// Token: 0x06006176 RID: 24950 RVA: 0x002DD220 File Offset: 0x002DB420
		public bool IsAcceleratingRight(TIDateTime currentTime)
		{
			return this._waypoints[0].IsAcceleratingRight(currentTime);
		}

		// Token: 0x06006177 RID: 24951 RVA: 0x002DD230 File Offset: 0x002DB430
		public bool IsAcceleratingLeft(TIDateTime currentTime)
		{
			return this._waypoints[0].IsAcceleratingLeft(currentTime);
		}

		// Token: 0x06006178 RID: 24952 RVA: 0x002DD240 File Offset: 0x002DB440
		public bool IsAcceleratingUp(TIDateTime currentTime)
		{
			return this._waypoints[0].IsAcceleratingUp(currentTime);
		}

		// Token: 0x06006179 RID: 24953 RVA: 0x002DD250 File Offset: 0x002DB450
		public bool IsAcceleratingDown(TIDateTime currentTime)
		{
			return this._waypoints[0].IsAcceleratingDown(currentTime);
		}

		// Token: 0x0600617A RID: 24954 RVA: 0x002DD260 File Offset: 0x002DB460
		public bool IsAcceleratingRollRight(TIDateTime currentTime)
		{
			return this._waypoints[0].IsAcceleratingRollRight(currentTime);
		}

		// Token: 0x0600617B RID: 24955 RVA: 0x002DD270 File Offset: 0x002DB470
		public bool IsAcceleratingRollLeft(TIDateTime currentTime)
		{
			return this._waypoints[0].IsAcceleratingRollLeft(currentTime);
		}

		// Token: 0x0600617C RID: 24956 RVA: 0x002DD280 File Offset: 0x002DB480
		public Vector3 PositionAtTime(TIDateTime currentTime)
		{
			return this._waypoints[0].PositionAt(currentTime);
		}

		// Token: 0x0600617D RID: 24957 RVA: 0x002DD290 File Offset: 0x002DB490
		public Vector3 VelocityAtTime(TIDateTime currentTime)
		{
			return this._waypoints[0].VelocityAt(currentTime);
		}

		// Token: 0x0600617E RID: 24958 RVA: 0x002DD2A0 File Offset: 0x002DB4A0
		public Vector3 AccelerationAtTime(TIDateTime currentTime)
		{
			return this._waypoints[0].AccelerationAt(currentTime);
		}

		// Token: 0x0600617F RID: 24959 RVA: 0x002DD2B0 File Offset: 0x002DB4B0
		public Vector3 HeadingAtTime(TIDateTime currentTime)
		{
			return this._waypoints[0].HeadingAt(currentTime);
		}

		// Token: 0x06006180 RID: 24960 RVA: 0x002DD2C0 File Offset: 0x002DB4C0
		public float CurrentAcceleration()
		{
			return this._waypoints[0].LinearAcceleration();
		}

		// Token: 0x06006181 RID: 24961 RVA: 0x002DD2CF File Offset: 0x002DB4CF
		public Quaternion RotationAtTime(TIDateTime currentTime)
		{
			return this._waypoints[0].RotationAt(currentTime);
		}

		// Token: 0x06006182 RID: 24962 RVA: 0x002DD2DF File Offset: 0x002DB4DF
		public float AngularVelocityAt_Rad(TIDateTime currentTime)
		{
			return this._waypoints[0].AngularVelocityAt_Rad(currentTime);
		}

		// Token: 0x06006183 RID: 24963 RVA: 0x002DD2F0 File Offset: 0x002DB4F0
		public SegmentProximityData FindNearestSegment()
		{
			this._pendingSegment = SegmentProximityData.DefaultData;
			this._pendingSegmentWaypointIndex = -1;
			for (int i = this._waypointCount - 1; i > 0; i--)
			{
				SegmentProximityData segmentProximityData = this.EvaluateDistanceToSegment(this._waypoints[i], this._waypoints[i - 1], true);
				if (segmentProximityData.DistanceToSegment < this._pendingSegment.DistanceToSegment)
				{
					this._pendingSegment = segmentProximityData;
					this._pendingSegmentWaypointIndex = i;
				}
			}
			return this._pendingSegment;
		}

		// Token: 0x06006184 RID: 24964 RVA: 0x002DD364 File Offset: 0x002DB564
		public bool UpdateWaypointPlacementLocation()
		{
			if (this._isActiveSegmentWaypointPlacementViable && this._activeSegmentWaypointIndex > 0)
			{
				AdjustableWaypoint adjustableWaypoint = this._waypoints[this._activeSegmentWaypointIndex - 1];
				AdjustableWaypoint adjustableWaypoint2 = this._waypoints[this._activeSegmentWaypointIndex];
				float num = (float)(adjustableWaypoint2.Timing - adjustableWaypoint.Timing).TotalSeconds;
				SegmentProximityData segmentProximityData = this.EvaluateDistanceToSegment(adjustableWaypoint, adjustableWaypoint2, false);
				float num2 = Vector3.Distance(segmentProximityData.PointOnSegment, adjustableWaypoint.Position);
				float num3 = Mathf.Clamp01(num2 / segmentProximityData.FullSegmentDistance);
				float num4 = num * num3;
				num4 = Mathf.Clamp(num4, 10f, num - 10f);
				this._activeSegmentTimingForPlacement = new TIDateTime(adjustableWaypoint.Timing);
				this._activeSegmentTimingForPlacement.AddSeconds((double)num4);
				Vector3 vector = this.PositionAtTime(this._activeSegmentTimingForPlacement);
				WaypointNavigationController._waypointPlacementVisual.gameObject.SetActive(true);
				WaypointNavigationController._waypointPlacementVisual.transform.position = vector;
				float num5 = 10f / num * segmentProximityData.FullSegmentDistance;
				num2 = Mathf.Clamp(num2, num5, segmentProximityData.FullSegmentDistance - num5);
				float num6 = this.CalculateRelativeDistanceAlongCoreToCoreSegment(num2, segmentProximityData.FullSegmentDistance);
				WaypointNavigationController._waypointPlacementVisual.SetColorIndex(this._waypointControllers[this._waypoints[this._activeSegmentWaypointIndex]].BaseColorIndex, num6);
				return true;
			}
			return false;
		}

		// Token: 0x06006185 RID: 24965 RVA: 0x002DD4B8 File Offset: 0x002DB6B8
		private SegmentProximityData EvaluateDistanceToSegment(AdjustableWaypoint currentWaypoint, AdjustableWaypoint previousWaypoint, bool shouldCheckForEarlyAbort = true)
		{
			Ray ray = this._mainCamera.ScreenPointToRay(Input.mousePosition);
			float num = Vector3.Distance(currentWaypoint.Position, previousWaypoint.Position);
			if (!shouldCheckForEarlyAbort || (Math3d.PointLineDistance(ray, currentWaypoint.Position) < num && Math3d.PointLineDistance(ray, previousWaypoint.Position) < num))
			{
				Vector3 vector = currentWaypoint.Position - previousWaypoint.Position;
				Vector3 vector2;
				Vector3 vector3;
				Math3d.ClosestPointsOnTwoLines(out vector2, out vector3, ray.origin, ray.direction, currentWaypoint.Position, vector);
				float num2 = Vector3.Distance(vector2, vector3);
				return new SegmentProximityData(currentWaypoint.UID, num2, num, vector3);
			}
			return SegmentProximityData.DefaultData;
		}

		// Token: 0x06006186 RID: 24966 RVA: 0x002DD55C File Offset: 0x002DB75C
		public void FinalizeWaypointPlacement()
		{
			if (!this._isActiveSegmentWaypointPlacementViable || this._activeSegmentTimingForPlacement == null)
			{
				return;
			}
			AdjustableWaypoint adjustableWaypoint = this._waypoints[this._activeSegmentWaypointIndex - 1];
			AdjustableWaypoint adjustableWaypoint2 = this._waypoints[this._activeSegmentWaypointIndex];
			SegmentProximityData segmentProximityData = this.EvaluateDistanceToSegment(adjustableWaypoint, adjustableWaypoint2, false);
			int baseColorIndex = this._waypointControllers[this._waypoints[this._activeSegmentWaypointIndex]].BaseColorIndex;
			float num = (float)(adjustableWaypoint2.Timing - adjustableWaypoint.Timing).TotalSeconds;
			float num2 = Vector3.Distance(segmentProximityData.PointOnSegment, adjustableWaypoint.Position);
			float num3 = 10f / num * segmentProximityData.FullSegmentDistance;
			num2 = Mathf.Clamp(num2, num3, segmentProximityData.FullSegmentDistance - num3);
			float num4 = this.CalculateRelativeDistanceAlongCoreToCoreSegment(num2, segmentProximityData.FullSegmentDistance);
			AdjustableWaypoint adjustableWaypoint3 = new AdjustableWaypoint(this._accelerationConstraints)
			{
				Position = this.PositionAtTime(this._activeSegmentTimingForPlacement),
				Velocity = this.VelocityAtTime(this._activeSegmentTimingForPlacement),
				Rotation = this.RotationAtTime(this._activeSegmentTimingForPlacement),
				Timing = this._activeSegmentTimingForPlacement
			};
			this._waypoints[this._activeSegmentWaypointIndex].InsertBefore(adjustableWaypoint3);
			Array.Resize<AdjustableWaypoint>(ref this._waypoints, this._waypoints.Length + 1);
			for (int i = this._waypoints.Length - 1; i >= this._activeSegmentWaypointIndex - 1; i--)
			{
				this._waypoints[i] = this._waypoints[i - 1];
			}
			this._waypoints[this._activeSegmentWaypointIndex - 1] = adjustableWaypoint3;
			WaypointController waypointController = new WaypointController(adjustableWaypoint3, baseColorIndex, this._waypointSharedData, this._waypointContainer.transform, adjustableWaypoint3.Velocity.normalized, false, this._shipState)
			{
				ColorInterpolationRatio = num4
			};
			waypointController.OnWaypointReadyForInput += this.HandleOnWaypointReadyForInput;
			waypointController.OnWaypointEndingInput += this.HandleOnWaypointEndingInput;
			waypointController.OnWaypointRemovalRequested += this.HandleOnWaypointRemovalRequested;
			waypointController.UpdateVisualPositionRotation();
			this._waypointControllers.Add(adjustableWaypoint3, waypointController);
		}

		// Token: 0x06006187 RID: 24967 RVA: 0x002DD774 File Offset: 0x002DB974
		private float CalculateRelativeDistanceAlongCoreToCoreSegment(float initialDistanceAlongSegment, float initialFullDistanceAlongSegment)
		{
			float num = initialDistanceAlongSegment;
			float num2 = initialFullDistanceAlongSegment;
			for (int i = this._activeSegmentWaypointIndex - 1; i > 0; i--)
			{
				if (!this._waypoints[i].IsCoreWaypoint)
				{
					float num3 = Vector3.Distance(this._waypoints[i].Position, this._waypoints[i - 1].Position);
					num += num3;
					num2 += num3;
				}
			}
			for (int j = this._activeSegmentWaypointIndex; j < this._waypoints.Length - 1; j++)
			{
				if (!this._waypoints[j].IsCoreWaypoint)
				{
					num2 += Vector3.Distance(this._waypoints[j].Position, this._waypoints[j + 1].Position);
				}
			}
			return Mathf.Clamp01(num / num2);
		}

		// Token: 0x06006188 RID: 24968 RVA: 0x002DD830 File Offset: 0x002DBA30
		public void UpdateActiveWaypointPlacementSegment()
		{
			bool flag = this._canWaypointsBeAdjusted && this._accelerationEffectiveness > WaypointNavigationController.AccelerationEffectiveness.DISABLED;
			if (this._pendingSegmentWaypointIndex > 0 && flag)
			{
				AdjustableWaypoint adjustableWaypoint = this._waypoints[this._pendingSegmentWaypointIndex - 1];
				float num = (float)(this._waypoints[this._pendingSegmentWaypointIndex].Timing - adjustableWaypoint.Timing).TotalSeconds;
				flag &= num > 20f;
				int num2 = 0;
				for (int i = 0; i < this._waypoints.Length; i++)
				{
					if (!this._waypoints[0].IsCoreWaypoint)
					{
						num2++;
					}
				}
				flag &= num2 < 3;
			}
			this._activeSegment = this._pendingSegment;
			this._activeSegmentWaypointIndex = this._pendingSegmentWaypointIndex;
			this._isActiveSegmentWaypointPlacementViable = flag;
		}

		// Token: 0x06006189 RID: 24969 RVA: 0x002DD8F4 File Offset: 0x002DBAF4
		public void ClearActiveWaypointPlacementSegment()
		{
			this._activeSegment = SegmentProximityData.DefaultData;
			WaypointNavigationController._waypointPlacementVisual.gameObject.SetActive(false);
		}

		// Token: 0x0600618A RID: 24970 RVA: 0x002DD911 File Offset: 0x002DBB11
		public void AddAgentShipController(CombatShipController ctrl)
		{
			if (!this._agentShipControllers.Contains(ctrl))
			{
				this._agentShipControllers.Add(ctrl);
				this._recalculateAvoidancePath = true;
			}
		}

		// Token: 0x0600618B RID: 24971 RVA: 0x002DD934 File Offset: 0x002DBB34
		public void RemoveAgentShipController(CombatShipController ctrl)
		{
			this._agentShipControllers.Remove(ctrl);
			if (this._agentShipControllers.Count == 0)
			{
				this._recalculateAvoidancePath = false;
			}
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x002DD957 File Offset: 0x002DBB57
		public void AddHabModuleController(HabModuleController ctrl, Collider col)
		{
			if (!this._habModuleControllers.ContainsKey(ctrl))
			{
				this._habModuleControllers.Add(ctrl, col);
				this._recalculateAvoidancePath = true;
			}
		}

		// Token: 0x0600618D RID: 24973 RVA: 0x002DD97B File Offset: 0x002DBB7B
		public void RemoveHabModuleController(HabModuleController ctrl)
		{
			this._habModuleControllers.Remove(ctrl);
			if (this._habModuleControllers.Count == 0)
			{
				this._recalculateAvoidancePath = false;
			}
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x002DD9A0 File Offset: 0x002DBBA0
		public void GetDVCost(float availDV_kps)
		{
			if (this._shipState.faction != GameControl.control.activePlayer)
			{
				return;
			}
			int num = 0;
			float num2 = 0f;
			List<Vector3> list = new List<Vector3>();
			float num3 = 0f;
			for (int i = 0; i < this._waypoints.Length; i++)
			{
				AdjustableWaypoint adjustableWaypoint = this._waypoints[i];
				WaypointController waypointController;
				if (!this._waypointControllers.TryGetValue(adjustableWaypoint, out waypointController))
				{
					return;
				}
				WaypointVisual visual = waypointController._visual;
				if (!visual.waypointUI.showingDVText)
				{
					return;
				}
				list.Add(adjustableWaypoint.Velocity);
				float num4;
				if (num > 0)
				{
					float totalMassFromDVRemaining = this._shipState.GetTotalMassFromDVRemaining(availDV_kps - num2);
					num4 = SpaceCombatManager.DVconsumption_kps(list[num - 1], adjustableWaypoint.Velocity, this._shipState, this._waypoints[i].LinearAcceleration(), totalMassFromDVRemaining);
				}
				else
				{
					num4 = SpaceCombatManager.DVconsumption_kps(adjustableWaypoint.VelocityAt(this._gameTime.currentTime), adjustableWaypoint.Velocity, this._shipState, this._waypoints[i].LinearAcceleration(), this._shipState.currentMass_kg);
				}
				num2 += num4;
				if (num2 > 0f && num2 != num3)
				{
					string text = Loc.T("UI.SpaceCombat.DeltaV", new object[] { num2.ToString("F1") }) + "[" + Loc.T("UI.SpaceCombat.DeltaV", new object[] { (availDV_kps - num2).ToString("F1") }) + "]";
					visual.waypointUI.dvText.SetText(text);
				}
				else if (!visual.waypointUI.dvText.text.Equals(""))
				{
					visual.waypointUI.dvText.SetText("");
				}
				num3 = num2;
				num++;
			}
		}

		// Token: 0x0600618F RID: 24975 RVA: 0x002DDB72 File Offset: 0x002DBD72
		public static Vector3 EstimateFutureEnemyPosition(CombatShipController enemyShip, DateTime fromTime, float futureTime_s)
		{
			return enemyShip.positionAtTime(fromTime) + futureTime_s * enemyShip.velocityAtTime(fromTime);
		}

		// Token: 0x06006190 RID: 24976 RVA: 0x002DDB90 File Offset: 0x002DBD90
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		private List<ValueTuple<TIDateTime, bool>> Simplify([TupleElementNames(new string[] { "time", "isBurn" })] List<ValueTuple<TIDateTime, bool>> a)
		{
			for (int i = a.Count - 2; i >= 0; i--)
			{
				if (a[i].Item1 == a[i + 1].Item1)
				{
					if (a[i].Item2)
					{
						a.RemoveAt(i + 1);
					}
					else
					{
						a.RemoveAt(i);
					}
				}
			}
			for (int j = a.Count - 3; j >= 0; j--)
			{
				if (!a[j].Item2 && !a[j + 1].Item2)
				{
					a.RemoveAt(j + 1);
				}
			}
			return a;
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x002DDC2C File Offset: 0x002DBE2C
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		private List<ValueTuple<TIDateTime, bool>> Consolidate([TupleElementNames(new string[] { "time", "isBurn" })] List<ValueTuple<TIDateTime, bool>> a, [TupleElementNames(new string[] { "time", "isBurn" })] List<ValueTuple<TIDateTime, bool>> b)
		{
			List<ValueTuple<TIDateTime, bool>> list = new List<ValueTuple<TIDateTime, bool>>();
			int num = 0;
			int num2 = 0;
			bool flag = false;
			bool flag2 = false;
			while (num < a.Count && num2 < b.Count)
			{
				if (a[num].Item1 == b[num2].Item1)
				{
					flag = a[num].Item2;
					flag2 = b[num2].Item2;
					list.Add(new ValueTuple<TIDateTime, bool>(a[num].Item1, flag || flag2));
					num++;
					num2++;
				}
				else if (a[num].Item1 < b[num2].Item1)
				{
					flag = a[num].Item2;
					list.Add(new ValueTuple<TIDateTime, bool>(a[num].Item1, flag || flag2));
					num++;
				}
				else
				{
					flag2 = b[num2].Item2;
					list.Add(new ValueTuple<TIDateTime, bool>(b[num2].Item1, flag || flag2));
					num2++;
				}
			}
			return list;
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x002DDD3C File Offset: 0x002DBF3C
		[return: TupleElementNames(new string[] { "time", "isBurn" })]
		private List<ValueTuple<TIDateTime, bool>> GetBurnTimings()
		{
			List<ValueTuple<TIDateTime, bool>> list = new List<ValueTuple<TIDateTime, bool>>();
			for (int i = 0; i < this._waypointCount; i++)
			{
				list.AddRange(this._waypoints[i].GetBurnTimings());
			}
			return this.Simplify(list);
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x002DDD7C File Offset: 0x002DBF7C
		private TIDateTime ClosestApproachTime(TIDateTime startTime, TIDateTime endTime, CombatShipController otherShip)
		{
			if (startTime == null || endTime == null)
			{
				Log.Error("CollisionDetector: one time was null.  startTime = " + ((startTime != null) ? startTime.ToString() : null) + ", endTime = " + ((endTime != null) ? endTime.ToString() : null), Array.Empty<object>());
				return null;
			}
			if (startTime > endTime)
			{
				Log.Error("CollisionDetector: attempted to detect a collision in a negative time span.  startTime = " + ((startTime != null) ? startTime.ToString() : null) + ", endTime = " + ((endTime != null) ? endTime.ToString() : null), Array.Empty<object>());
				return null;
			}
			if (startTime == endTime)
			{
				Log.Warn("CollisionDetector: attempted to detect a collision in a zero time span.  startTime = endTime = " + ((startTime != null) ? startTime.ToString() : null), Array.Empty<object>());
				return null;
			}
			double num = endTime.DifferenceInSeconds(startTime);
			TIDateTime tidateTime = new TIDateTime(startTime, num / 2.0);
			Vector3 vector = otherShip._waypointNavigationController.AccelerationAtTime(tidateTime);
			Vector3 vector2 = this.AccelerationAtTime(tidateTime);
			Vector3 vector3 = vector - vector2;
			Vector3 vector4 = otherShip._waypointNavigationController.VelocityAtTime(startTime);
			Vector3 vector5 = this.VelocityAtTime(startTime);
			Vector3 vector6 = vector4 - vector5;
			Vector3 vector7 = otherShip._waypointNavigationController.PositionAtTime(startTime);
			Vector3 vector8 = this.PositionAtTime(startTime);
			Vector3 vector9 = vector7 - vector8;
			float num2 = ((vector3 == Vector3.zero) ? this.LinearClosestApproach(vector6, vector9, (float)num) : this.AcceleratingClosestApproach(vector3, vector6, vector9, (float)num));
			return new TIDateTime(startTime, (double)num2);
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x002DDEE0 File Offset: 0x002DC0E0
		private float LinearClosestApproach(Vector3 relativeVelocity, Vector3 relativeStartPosition, float duration)
		{
			if (relativeVelocity == Vector3.zero)
			{
				return duration / 2f;
			}
			float num = -relativeStartPosition.Dot(relativeVelocity) / relativeVelocity.sqrMagnitude;
			if (num < 0f)
			{
				return 0f;
			}
			if (num > duration)
			{
				return duration;
			}
			return num;
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x002DDF28 File Offset: 0x002DC128
		private float AcceleratingClosestApproach(Vector3 relativeAcceleration, Vector3 relativeStartVelocity, Vector3 relativeStartPosition, float duration)
		{
			Vector3 vector = (relativeStartPosition + duration * relativeStartVelocity + duration * duration * 0.5f * relativeAcceleration - relativeStartPosition) / duration;
			return this.LinearClosestApproach(vector, relativeStartPosition, duration);
		}

		// Token: 0x06006196 RID: 24982 RVA: 0x002DDF74 File Offset: 0x002DC174
		private float CollisionDistance(CombatShipController ship)
		{
			float z = this._collisionBoxSize.z;
			float z2 = ship._waypointNavigationController._collisionBoxSize.z;
			return Mathf.Min((z + z2) / 2f, this.maxClosestApproachDistance);
		}

		// Token: 0x06006197 RID: 24983 RVA: 0x002DDFB0 File Offset: 0x002DC1B0
		[return: TupleElementNames(new string[] { "time", "displacement", "ship" })]
		private ValueTuple<TIDateTime, Vector3, CombatShipController> DetectCollisions(TIDateTime currentTime)
		{
			List<ValueTuple<TIDateTime, bool>> burnTimings = this.GetBurnTimings();
			foreach (CombatShipController combatShipController in this._agentShipControllers)
			{
				if (!combatShipController.isDestroyed && !(combatShipController.ShipState == this._shipState))
				{
					float num = this.CollisionDistance(combatShipController);
					float num2 = num * num;
					List<ValueTuple<TIDateTime, bool>> burnTimings2 = combatShipController._waypointNavigationController.GetBurnTimings();
					List<ValueTuple<TIDateTime, bool>> list = this.Consolidate(burnTimings, burnTimings2);
					Vector3 zero = Vector3.zero;
					for (int i = 0; i < list.Count - 1; i++)
					{
						TIDateTime tidateTime = this.ClosestApproachTime(list[i].Item1, list[i + 1].Item1, combatShipController);
						if (!(tidateTime == null))
						{
							Vector3 vector = combatShipController._waypointNavigationController.PositionAtTime(tidateTime) - this.PositionAtTime(tidateTime);
							if (vector.sqrMagnitude < num2)
							{
								return new ValueTuple<TIDateTime, Vector3, CombatShipController>(tidateTime, vector, combatShipController);
							}
						}
					}
				}
			}
			return new ValueTuple<TIDateTime, Vector3, CombatShipController>(null, Vector3.zero, null);
		}

		// Token: 0x06006198 RID: 24984 RVA: 0x002DE0E0 File Offset: 0x002DC2E0
		private void CheckForCollisions(TIDateTime currentTime, out bool continueChecking)
		{
			ValueTuple<TIDateTime, Vector3, CombatShipController> valueTuple = this.DetectCollisions(currentTime);
			if (valueTuple.Item1 != null && valueTuple.Item3 != null)
			{
				Vector3 vector = valueTuple.Item3._waypointNavigationController.VelocityAtTime(valueTuple.Item1) - this.VelocityAtTime(valueTuple.Item1);
				bool flag = this._waypointControllers.Any<KeyValuePair<AdjustableWaypoint, WaypointController>>((KeyValuePair<AdjustableWaypoint, WaypointController> x) => x.Value.IsHandlingMovementInput);
				bool flag2 = valueTuple.Item3._waypointNavigationController._waypointControllers.Any<KeyValuePair<AdjustableWaypoint, WaypointController>>((KeyValuePair<AdjustableWaypoint, WaypointController> x) => x.Value.IsHandlingMovementInput);
				if (this._gameTime.Paused || flag || flag2)
				{
					AdjustableWaypoint nextWaypoint = this.GetNextWaypoint(valueTuple.Item1);
					if (!valueTuple.Item3._waypointNavigationController.GetNextWaypoint(valueTuple.Item1).CollisionWarningNeeded)
					{
						nextWaypoint.CollisionWarningNeeded = true;
					}
					continueChecking = true;
					return;
				}
				this.AssignAvoidanceReposition(valueTuple.Item2, this.CollisionDistance(valueTuple.Item3), vector, valueTuple.Item1, currentTime);
			}
			bool flag3;
			if (!this._gameTime.Paused)
			{
				flag3 = this._waypointControllers.Any<KeyValuePair<AdjustableWaypoint, WaypointController>>((KeyValuePair<AdjustableWaypoint, WaypointController> x) => x.Value.IsHandlingMovementInput);
			}
			else
			{
				flag3 = true;
			}
			continueChecking = flag3;
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x002DE243 File Offset: 0x002DC443
		private bool Intersects(Bounds a, Bounds b)
		{
			return a.Intersects(b) || a.Contains(b.center) || b.Intersects(a) || b.Contains(a.center);
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x002DE27C File Offset: 0x002DC47C
		private void AssignAvoidanceReposition(Vector3 averageCollisionDirection, float minDistToAvoidCollision, Vector3 relativeVelocityAtCollision, TIDateTime timeAt, TIDateTime currentTime)
		{
			Vector3 vector = -averageCollisionDirection;
			if (Vector3.Cross(vector, relativeVelocityAtCollision).sqrMagnitude < 1E-45f)
			{
				vector = this.RotationAtTime(timeAt) * new Vector3(0f, 0f, 1f);
			}
			Vector3.OrthoNormalize(ref relativeVelocityAtCollision, ref vector);
			if (vector.magnitude < 1E-45f)
			{
				string text = "Impossible collision: direction = ";
				Vector3 vector2 = averageCollisionDirection;
				string text2 = vector2.ToString();
				string text3 = ", velocity = ";
				vector2 = relativeVelocityAtCollision;
				Log.Warn(text + text2 + text3 + vector2.ToString(), Array.Empty<object>());
				vector = new Vector3(0f, 1f, 0f);
			}
			AdjustableWaypoint nextWaypoint = this.GetNextWaypoint(timeAt);
			float num = (float)nextWaypoint.Timing.DifferenceInSeconds(currentTime);
			float num2 = (float)timeAt.DifferenceInSeconds(currentTime);
			float num3 = minDistToAvoidCollision * num / num2;
			Vector3 vector3 = vector * num3;
			Vector3 vector4 = nextWaypoint.Position + vector3;
			Vector3 vector5 = this.PositionAtTime(timeAt);
			vector5 + vector * minDistToAvoidCollision;
			vector5 + vector;
			ProposedWaypoint proposedWaypoint = new ProposedWaypoint
			{
				Timing = new TIDateTime(currentTime),
				Position = this.PositionAtTime(currentTime),
				Rotation = this.RotationAtTime(currentTime),
				Velocity = this.VelocityAtTime(currentTime),
				RotationAllowed = false
			};
			ProposedWaypoint proposedWaypoint2 = new ProposedWaypoint();
			proposedWaypoint2.SetData(nextWaypoint);
			proposedWaypoint2.Position = vector4;
			proposedWaypoint2.RotationAllowed = false;
			this._waypoints[0].AdjustPlacement(proposedWaypoint, proposedWaypoint2, nextWaypoint.Position);
			TIDateTime timeOfCollisionPassed = this.TimeOfCollisionPassed;
			this.TimeOfCollisionPassed = nextWaypoint.Timing;
			if (timeOfCollisionPassed != this.TimeOfCollisionPassed)
			{
				GameControl.eventManager.TriggerEvent(new CombatCollisionAvoidanceStatusChange(this._shipState), null, new object[] { this._shipState });
			}
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x002DE450 File Offset: 0x002DC650
		public WaypointController GetWaypointControllerByColor(int baseColorIndex)
		{
			foreach (KeyValuePair<AdjustableWaypoint, WaypointController> keyValuePair in this._waypointControllers)
			{
				if (keyValuePair.Value.BaseColorIndex == baseColorIndex)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x0600619C RID: 24988 RVA: 0x002DE4B8 File Offset: 0x002DC6B8
		public void ClearWaypointGizmos()
		{
			foreach (AdjustableWaypoint adjustableWaypoint in this._waypoints)
			{
				if (this._waypointControllers.ContainsKey(adjustableWaypoint))
				{
					this._waypointControllers[adjustableWaypoint].ClearGizmoVisuals();
				}
			}
		}

		// Token: 0x0400445A RID: 17498
		private const string WAYPOINT_CONTAINER_NAME_SUFFIX = " Waypoint Container";

		// Token: 0x0400445B RID: 17499
		private const string PATH = " Path ";

		// Token: 0x0400445C RID: 17500
		private const float MIN_TIME_IN_SECONDS_BETWEEN_WAYPOINTS = 10f;

		// Token: 0x0400445D RID: 17501
		private const int MAX_INTERIM_WAYPOINTS_BETWEEN_CORE_WAYPOINTS = 3;

		// Token: 0x0400445E RID: 17502
		public static Color32 waypointGreenLine = new Color(0f, 0.5f, 0f, 1f);

		// Token: 0x0400445F RID: 17503
		private EventInstance eventInstance;

		// Token: 0x04004460 RID: 17504
		private string _name;

		// Token: 0x04004461 RID: 17505
		private int _waypointCount;

		// Token: 0x04004462 RID: 17506
		private WaypointSharedData _waypointSharedData;

		// Token: 0x04004463 RID: 17507
		private TISpaceShipState _shipState;

		// Token: 0x04004464 RID: 17508
		private SpaceCombatCameraController _spaceCombatCameraController;

		// Token: 0x04004465 RID: 17509
		private GameTimeManager _gameTime;

		// Token: 0x04004467 RID: 17511
		private bool _canWaypointsBeAdjusted = true;

		// Token: 0x04004468 RID: 17512
		private bool _isOutOfCombatDV;

		// Token: 0x04004469 RID: 17513
		private WaypointNavigationController.AccelerationEffectiveness _accelerationEffectiveness = WaypointNavigationController.AccelerationEffectiveness.FULL_POWER;

		// Token: 0x0400446A RID: 17514
		private float _accelerationEffectivenessRatio = 1f;

		// Token: 0x0400446B RID: 17515
		private Vector3 _collisionBoxSize;

		// Token: 0x0400446C RID: 17516
		private List<CombatShipController> _agentShipControllers;

		// Token: 0x0400446D RID: 17517
		private Dictionary<HabModuleController, Collider> _habModuleControllers;

		// Token: 0x0400446E RID: 17518
		private LinkedList<WaypointTrajectorySequence> _targetTrajectoryPath;

		// Token: 0x0400446F RID: 17519
		private bool _recalculateAvoidancePath;

		// Token: 0x04004470 RID: 17520
		private bool _allStopCalculatedThisCycle;

		// Token: 0x04004471 RID: 17521
		private bool _matchVelocityCalculatedThisCycle;

		// Token: 0x04004472 RID: 17522
		private bool _defensiveManueversCalculatedThisCycle;

		// Token: 0x04004473 RID: 17523
		private float cached_acceleration;

		// Token: 0x04004474 RID: 17524
		private float cached_cruise_acceleration;

		// Token: 0x04004475 RID: 17525
		private float cached_angular_acceleration_rads2;

		// Token: 0x04004476 RID: 17526
		private float cached_max_angular_velocity_rads2;

		// Token: 0x04004477 RID: 17527
		private bool _propulsionValuesDirty;

		// Token: 0x04004478 RID: 17528
		private bool _propulsionValuesImproved;

		// Token: 0x04004479 RID: 17529
		private GameObjectDictionary<string> _waypointContainer;

		// Token: 0x0400447A RID: 17530
		private bool _padlockEnabled;

		// Token: 0x0400447B RID: 17531
		private bool _allStopEnabled;

		// Token: 0x0400447C RID: 17532
		private bool _matchVelocityEnabled;

		// Token: 0x0400447D RID: 17533
		private bool _defensiveManueversEnabled;

		// Token: 0x0400447F RID: 17535
		private CombatantController _thisCombatant;

		// Token: 0x04004480 RID: 17536
		private CombatantController _primaryTarget;

		// Token: 0x04004481 RID: 17537
		private CombatantController _maneuverTarget;

		// Token: 0x04004482 RID: 17538
		private AdjustableWaypoint[] _waypoints;

		// Token: 0x04004483 RID: 17539
		private Dictionary<AdjustableWaypoint, WaypointController> _waypointControllers;

		// Token: 0x04004484 RID: 17540
		private static WaypointController _activeInputHandlingController;

		// Token: 0x04004485 RID: 17541
		private Quaternion _appendWaypointRotation = Quaternion.identity;

		// Token: 0x04004486 RID: 17542
		private WaypointNavigationController.InitialWaypoint _initialWaypoint;

		// Token: 0x04004487 RID: 17543
		private Camera _mainCamera;

		// Token: 0x04004488 RID: 17544
		private TIDateTime _activeSegmentTimingForPlacement;

		// Token: 0x04004489 RID: 17545
		private static WaypointVisual _waypointPlacementVisual;

		// Token: 0x0400448A RID: 17546
		private SegmentProximityData _pendingSegment = SegmentProximityData.DefaultData;

		// Token: 0x0400448B RID: 17547
		private int _pendingSegmentWaypointIndex;

		// Token: 0x0400448C RID: 17548
		private SegmentProximityData _activeSegment = SegmentProximityData.DefaultData;

		// Token: 0x0400448D RID: 17549
		private int _activeSegmentWaypointIndex;

		// Token: 0x0400448E RID: 17550
		private TIDateTime TimeOfNextCollisionCheck;

		// Token: 0x04004490 RID: 17552
		private float _maxClosestApproachDistance = -1f;

		// Token: 0x04004491 RID: 17553
		private bool _isActiveSegmentWaypointPlacementViable;

		// Token: 0x04004492 RID: 17554
		private const int MAX_STEP_COUNT = 6;

		// Token: 0x04004493 RID: 17555
		private readonly float STEP_TIME_LENGTH = GameControl.spaceCombat.waypointTimeDelta / 2f;

		// Token: 0x02001388 RID: 5000
		private enum AccelerationEffectiveness
		{
			// Token: 0x040071C3 RID: 29123
			DISABLED,
			// Token: 0x040071C4 RID: 29124
			HALF_POWER,
			// Token: 0x040071C5 RID: 29125
			FULL_POWER
		}

		// Token: 0x02001389 RID: 5001
		private class InitialWaypoint : BasicWaypoint, IPreviousWaypoint, IWaypoint
		{
			// Token: 0x06009176 RID: 37238 RVA: 0x00347687 File Offset: 0x00345887
			public InitialWaypoint(Vector3 position, Vector3 velocity, Quaternion rotation, TIDateTime timing, float alphaBlendValue)
			{
				base.SetData(position, velocity, rotation, timing, alphaBlendValue);
			}

			// Token: 0x06009177 RID: 37239 RVA: 0x0034769C File Offset: 0x0034589C
			public void SetNextWaypoint(INextWaypoint nextWaypoint)
			{
			}

			// Token: 0x06009178 RID: 37240 RVA: 0x0034769E File Offset: 0x0034589E
			public void ResetNextWaypointSequence()
			{
			}
		}
	}
}
