using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.Systems.Camera;
using Unity.Entities;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x0200070F RID: 1807
	public class StrategyShipController : CombatantShipController
	{
		// Token: 0x170005DF RID: 1503
		// (get) Token: 0x06002B09 RID: 11017 RVA: 0x000E9EC9 File Offset: 0x000E80C9
		// (set) Token: 0x06002B0A RID: 11018 RVA: 0x000E9ED1 File Offset: 0x000E80D1
		public Hull hull { get; private set; }

		// Token: 0x170005E0 RID: 1504
		// (get) Token: 0x06002B0B RID: 11019 RVA: 0x000E9EDA File Offset: 0x000E80DA
		// (set) Token: 0x06002B0C RID: 11020 RVA: 0x000E9EE2 File Offset: 0x000E80E2
		public override List<Collider> hitColliders { get; protected set; }

		// Token: 0x170005E1 RID: 1505
		// (get) Token: 0x06002B0D RID: 11021 RVA: 0x000E9EEB File Offset: 0x000E80EB
		// (set) Token: 0x06002B0E RID: 11022 RVA: 0x000E9EF3 File Offset: 0x000E80F3
		public override TISpaceShipState ShipState { get; protected set; }

		// Token: 0x170005E2 RID: 1506
		// (get) Token: 0x06002B0F RID: 11023 RVA: 0x000E9EFC File Offset: 0x000E80FC
		// (set) Token: 0x06002B10 RID: 11024 RVA: 0x000E9F04 File Offset: 0x000E8104
		public ShipVisController VisController { get; private set; }

		// Token: 0x170005E3 RID: 1507
		// (get) Token: 0x06002B11 RID: 11025 RVA: 0x000E9F0D File Offset: 0x000E810D
		public override IDamageableType damageableType
		{
			get
			{
				return IDamageableType.Ship;
			}
		}

		// Token: 0x170005E4 RID: 1508
		// (get) Token: 0x06002B12 RID: 11026 RVA: 0x000E9F10 File Offset: 0x000E8110
		// (set) Token: 0x06002B13 RID: 11027 RVA: 0x000E9F1D File Offset: 0x000E811D
		public override ShipModelController ModelController
		{
			get
			{
				return this.VisController.ModelController;
			}
			protected set
			{
				this.VisController.ModelController = value;
			}
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x000E9F2B File Offset: 0x000E812B
		public override CombatTargetableState GetCombatantState()
		{
			return this.ShipState;
		}

		// Token: 0x06002B15 RID: 11029 RVA: 0x000E9F33 File Offset: 0x000E8133
		public override IDamageableType GetCombatantType()
		{
			return IDamageableType.Ship;
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x000E9F36 File Offset: 0x000E8136
		public override SpaceCombatAssetUIController UIController()
		{
			return this.VisController.UIController;
		}

		// Token: 0x06002B17 RID: 11031 RVA: 0x000E9F43 File Offset: 0x000E8143
		public override Vector3 positionAtTime(DateTime currentTime)
		{
			return (Vector3)this.ShipState.currentFleetOffset;
		}

		// Token: 0x170005E5 RID: 1509
		// (get) Token: 0x06002B18 RID: 11032 RVA: 0x000E9F55 File Offset: 0x000E8155
		// (set) Token: 0x06002B19 RID: 11033 RVA: 0x000E9F5D File Offset: 0x000E815D
		public override Vector3 velocityVector { get; protected set; } = Vector3.zero;

		// Token: 0x170005E6 RID: 1510
		// (get) Token: 0x06002B1A RID: 11034 RVA: 0x000E9F66 File Offset: 0x000E8166
		public override Vector3 velocityVector_kps { get; } = Vector3.zero;

		// Token: 0x170005E7 RID: 1511
		// (get) Token: 0x06002B1B RID: 11035 RVA: 0x000E9F6E File Offset: 0x000E816E
		public override Vector3 accelerationVector { get; } = Vector3.zero;

		// Token: 0x170005E8 RID: 1512
		// (get) Token: 0x06002B1C RID: 11036 RVA: 0x000E9F76 File Offset: 0x000E8176
		public override Vector3 accelerationVector_kps { get; } = Vector3.zero;

		// Token: 0x170005E9 RID: 1513
		// (get) Token: 0x06002B1D RID: 11037 RVA: 0x000E9F7E File Offset: 0x000E817E
		public override Transform GetDamageableTransform
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x000E9F88 File Offset: 0x000E8188
		public void Initialize(GameObject visPrefab, TISpaceShipState shipState, FleetVisController fleetVisController, bool uiOnly = false)
		{
			base.WeaponCarrierState = shipState;
			this.ShipState = shipState;
			this.FleetVisController = fleetVisController;
			this.initializedUiOnly = uiOnly;
			this.cameraManager = World.Active.GetExistingManager<CameraManager>();
			base.transform.SetLayer(LayerMask.NameToLayer("Solar System"), true);
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(visPrefab, base.transform, false);
			this.VisController = gameObject.GetComponent<ShipVisController>();
			this.VisController.InitializeShipVisualizer(this.ShipState.template, this.ShipState, this.FleetVisController, this, true);
			this.ShipState.CreateVisualizer(this.VisController);
			this.hitColliders = this.ModelController.GetComponentsInChildren<Collider>().ToList<Collider>();
			this.hull = StrategyShipController.CreateHull(shipState);
			for (int i = 0; i < shipState.noseWeapons.Count; i++)
			{
				this.hull.AddComponentMap<IWeapon>(ComponentMap.single, "Nose" + i.ToString());
			}
			for (int j = 0; j < shipState.hullWeapons.Count; j++)
			{
				this.hull.AddComponentMap<IWeapon>(ComponentMap.single, "Lateral" + j.ToString());
			}
			this.UpdateAllCouncilors(null);
			this.SetAllCouncilorsActive(!this.initializedUiOnly);
			this.AddListeners();
			bool activeSelf = base.gameObject.activeSelf;
			base.gameObject.SetActive(true);
			base.gameObject.SetActive(false);
			base.gameObject.SetActive(activeSelf);
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000EA105 File Offset: 0x000E8305
		public static Hull CreateHull(TISpaceShipState ship)
		{
			return new Hull(TISpaceShipState.SetUpArmorSections(ship), ship);
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x000EA113 File Offset: 0x000E8313
		public void DisableStratController()
		{
			base.enabled = false;
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x000EA11C File Offset: 0x000E831C
		private void AddListeners()
		{
			if (!this.initializedUiOnly)
			{
				GameControl.eventManager.AddListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateAllCouncilors), null, this.ShipState, true, false);
				GameControl.eventManager.AddListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.UpdateAllCouncilors), null, this.ShipState, true, false);
				GameControl.eventManager.AddListener<ShipEntersCombat>(new EventManager.EventDelegate<ShipEntersCombat>(this.EnterCombat), this.ShipState.ID.ToString(), this.ShipState, true, false);
				GameControl.eventManager.AddListener<ShipLeavesCombat>(new EventManager.EventDelegate<ShipLeavesCombat>(this.PostCombat), this.ShipState.ID.ToString(), this.ShipState, false, false);
				GameControl.eventManager.AddListener<ShipDestroyedByHeat>(new EventManager.EventDelegate<ShipDestroyedByHeat>(this.OnShipDestroyedByHeat), null, this.ShipState, false, false);
			}
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x000EA200 File Offset: 0x000E8400
		private void RemoveListeners()
		{
			if (!this.initializedUiOnly)
			{
				GameControl.eventManager.RemoveListener<CouncilorPositionUpdated>(new EventManager.EventDelegate<CouncilorPositionUpdated>(this.UpdateAllCouncilors), null);
				GameControl.eventManager.RemoveListener<CouncilorDepartsShip>(new EventManager.EventDelegate<CouncilorDepartsShip>(this.UpdateAllCouncilors), null);
				GameControl.eventManager.RemoveListener<ShipEntersCombat>(new EventManager.EventDelegate<ShipEntersCombat>(this.EnterCombat), null);
				GameControl.eventManager.RemoveListener<ShipLeavesCombat>(new EventManager.EventDelegate<ShipLeavesCombat>(this.PostCombat), null);
				GameControl.eventManager.RemoveListener<ShipDestroyedByHeat>(new EventManager.EventDelegate<ShipDestroyedByHeat>(this.OnShipDestroyedByHeat), null);
			}
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x000EA288 File Offset: 0x000E8488
		public void OnShipVisualizationDataDirty(ShipVisualizationDataDirty e)
		{
			this.dataDirty = true;
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x000EA291 File Offset: 0x000E8491
		public void EnterCombat(ShipEntersCombat e)
		{
			this.SetAllCouncilorsActive(false);
			this.DisableStratController();
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x000EA2A0 File Offset: 0x000E84A0
		public void PostCombat(ShipLeavesCombat e)
		{
			if (TIGameState.Valid(e.shipState) && !e.shipState.hull.simpleHull && !e.shipState.ShipDestroyed() && !this.Equals(null))
			{
				this.SetAllCouncilorsActive(true);
				base.enabled = true;
				this.VisController.transform.SetParent(base.transform);
				this.VisController.transform.localPosition = Vector3.zero;
				this.VisController.transform.localRotation = Quaternion.identity;
				return;
			}
			string[] array = new string[1];
			int num = 0;
			string text = "PostCombat failed to process for: ";
			TISpaceShipState shipState = e.shipState;
			array[num] = text + ((shipState != null) ? new GameStateID?(shipState.ID) : null).ToString();
			Debug.LogError(TIUtilities.CombineStrings(array));
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x000EA37C File Offset: 0x000E857C
		private void UpdateManeuverThrusterFX(TIDateTime currentTime)
		{
			ITrajectory trajectory = this.ShipState.CurrentManeuverSequence.TrajectoryAt(currentTime);
			if (this.previousManeuver != trajectory)
			{
				this.ModelController.DeactivateAllVectorThrusters();
				this.inPreviousCounterBurn = false;
				this.inPreviousBurn = false;
			}
			DriftTrajectory driftTrajectory = trajectory as DriftTrajectory;
			if (driftTrajectory == null)
			{
				if (!(trajectory is BurnTrajectory))
				{
					RotationTrajectory rotationTrajectory = trajectory as RotationTrajectory;
					if (rotationTrajectory != null)
					{
						Vector3 normalized = (this.ShipState.currentRotation * Vector3.forward - base.transform.forward).normalized;
						Vector3 normalized2 = (this.ShipState.currentRotation * Vector3.up - base.transform.up).normalized;
						bool flag = rotationTrajectory.InCounterBurn(currentTime);
						if (this.inPreviousCounterBurn != flag)
						{
							this.ModelController.DeactivateRollRightVectorThrusters();
							this.ModelController.DeactivateRollLeftVectorThrusters();
							this.ModelController.DeactivatePitchUpVectorThrusters();
							this.ModelController.DeactivatePitchDownVectorThrusters();
							this.ModelController.DeactivateRightTurnVectorThrusters();
							this.ModelController.DeactivateLeftTurnVectorThrusters();
						}
						this.inPreviousCounterBurn = flag;
						if (normalized2.x > 0f)
						{
							if (flag)
							{
								this.ModelController.ActivateRollLeftVectorThrusters();
							}
							else
							{
								this.ModelController.ActivateRollRightVectorThrusters();
							}
						}
						else if (normalized2.x < 0f)
						{
							if (flag)
							{
								this.ModelController.ActivateRollRightVectorThrusters();
							}
							else
							{
								this.ModelController.ActivateRollLeftVectorThrusters();
							}
						}
						else
						{
							this.ModelController.DeactivateRollRightVectorThrusters();
							this.ModelController.DeactivateRollLeftVectorThrusters();
						}
						if (normalized.y > 0f)
						{
							if (flag)
							{
								this.ModelController.ActivatePitchDownVectorThrusters();
							}
							else
							{
								this.ModelController.ActivatePitchUpVectorThrusters();
							}
						}
						else if (normalized.y < 0f)
						{
							if (flag)
							{
								this.ModelController.ActivatePitchUpVectorThrusters();
							}
							else
							{
								this.ModelController.ActivatePitchDownVectorThrusters();
							}
						}
						else
						{
							this.ModelController.DeactivatePitchUpVectorThrusters();
							this.ModelController.DeactivatePitchDownVectorThrusters();
						}
						if (normalized.x > 0f)
						{
							if (flag)
							{
								this.ModelController.ActivateRightTurnVectorThrusters();
							}
							else
							{
								this.ModelController.ActivateLeftTurnVectorThrusters();
							}
						}
						else if (normalized.x < 0f)
						{
							if (flag)
							{
								this.ModelController.ActivateLeftTurnVectorThrusters();
							}
							else
							{
								this.ModelController.ActivateRightTurnVectorThrusters();
							}
						}
						else
						{
							this.ModelController.DeactivateRightTurnVectorThrusters();
							this.ModelController.DeactivateLeftTurnVectorThrusters();
						}
					}
				}
				else
				{
					if (trajectory.IsInBurn(currentTime))
					{
						if (!this.inPreviousBurn)
						{
							this.ModelController.ActivateThrusters(true);
							this.inPreviousBurn = true;
						}
					}
					else
					{
						this.ModelController.DeactivateThrusters(false);
						this.inPreviousBurn = false;
					}
					this.inPreviousCounterBurn = false;
				}
			}
			else
			{
				bool flag2 = driftTrajectory.InCounterBurn(currentTime);
				if (this.inPreviousCounterBurn != flag2)
				{
					this.ModelController.DeactivateSlideUpVectorThrusters();
					this.ModelController.DeactivateSlideDownVectorThrusters();
					this.ModelController.DeactivateSlideRightVectorThrusters();
					this.ModelController.DeactivateSlideLeftVectorThrusters();
				}
				this.inPreviousCounterBurn = flag2;
				Vector3 normalized3 = (this.cameraManager.ScaledPosition_DoNotTouchCache(this.ShipState.globalPosition) - base.transform.position).normalized;
				if (normalized3.y > 0.01f)
				{
					if (flag2)
					{
						this.ModelController.ActivateSlideDownVectorThrusters();
					}
					else
					{
						this.ModelController.ActivateSlideUpVectorThrusters();
					}
				}
				else if (normalized3.y < -0.01f)
				{
					if (flag2)
					{
						this.ModelController.ActivateSlideUpVectorThrusters();
					}
					else
					{
						this.ModelController.ActivateSlideDownVectorThrusters();
					}
				}
				else
				{
					this.ModelController.DeactivateSlideUpVectorThrusters();
					this.ModelController.DeactivateSlideDownVectorThrusters();
				}
				if (normalized3.x > 0.01f)
				{
					if (flag2)
					{
						this.ModelController.ActivateSlideLeftVectorThrusters();
					}
					else
					{
						this.ModelController.ActivateSlideRightVectorThrusters();
					}
				}
				else if (normalized3.x < -0.01f)
				{
					if (flag2)
					{
						this.ModelController.ActivateSlideRightVectorThrusters();
					}
					else
					{
						this.ModelController.ActivateSlideLeftVectorThrusters();
					}
				}
				else
				{
					this.ModelController.DeactivateSlideRightVectorThrusters();
					this.ModelController.DeactivateSlideLeftVectorThrusters();
				}
			}
			this.previousManeuver = trajectory;
		}

		// Token: 0x06002B27 RID: 11047 RVA: 0x000EA79C File Offset: 0x000E899C
		private void UpdateFleetOffsetFX(TIDateTime currentTime)
		{
			double num = this.ShipState.CurrentManeuverCompletePercentage(currentTime);
			Vector3 eulerAngles = (base.transform.rotation * Quaternion.Inverse(this.ShipState.currentRotation)).eulerAngles;
			if (num < 0.25)
			{
				if (this._stratLayerThrusterPhase != 1)
				{
					this._stratLayerThrusterPhase = 1;
					if (eulerAngles.y > 0f)
					{
						this.ModelController.ActivateRightTurnVectorThrusters();
					}
					else if (eulerAngles.y < 0f)
					{
						this.ModelController.ActivateLeftTurnVectorThrusters();
					}
					if (eulerAngles.x > 0f)
					{
						this.ModelController.ActivatePitchUpVectorThrusters();
						return;
					}
					if (eulerAngles.x < 0f)
					{
						this.ModelController.ActivatePitchDownVectorThrusters();
						return;
					}
				}
			}
			else if (num > 0.75 && num < 0.9)
			{
				if (this._stratLayerThrusterPhase != 2)
				{
					this._stratLayerThrusterPhase = 2;
					if (eulerAngles.y < 0f)
					{
						this.ModelController.ActivateRightTurnVectorThrusters();
					}
					else if (eulerAngles.y > 0f)
					{
						this.ModelController.ActivateLeftTurnVectorThrusters();
					}
					if (eulerAngles.x < 0f)
					{
						this.ModelController.ActivatePitchUpVectorThrusters();
						return;
					}
					if (eulerAngles.x > 0f)
					{
						this.ModelController.ActivatePitchDownVectorThrusters();
						return;
					}
				}
			}
			else if (this._stratLayerThrusterPhase != 0)
			{
				this._stratLayerThrusterPhase = 0;
				this.ModelController.DeactivateLeftTurnVectorThrusters();
				this.ModelController.DeactivateRightTurnVectorThrusters();
				this.ModelController.DeactivateRollLeftVectorThrusters();
				this.ModelController.DeactivateRollRightVectorThrusters();
				this.ModelController.DeactivatePitchDownVectorThrusters();
				this.ModelController.DeactivatePitchUpVectorThrusters();
			}
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x000EA948 File Offset: 0x000E8B48
		public void SetAllCouncilorsActive(bool active)
		{
			if (this.VisController != null && this.VisController.UIVisualizationOnly)
			{
				foreach (SpaceCouncilorController spaceCouncilorController in base.transform.GetComponentsInChildren<SpaceCouncilorController>(true))
				{
					spaceCouncilorController.gameObject.SetActive(active && TIGameState.Valid(spaceCouncilorController.councilor));
				}
				return;
			}
			foreach (SpaceCouncilorController spaceCouncilorController2 in this.councilorControllers.Keys)
			{
				if (spaceCouncilorController2 != null)
				{
					spaceCouncilorController2.gameObject.SetActive(active && TIGameState.Valid(spaceCouncilorController2.councilor));
				}
			}
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x000EAA1C File Offset: 0x000E8C1C
		public void UpdateAllCouncilors(CouncilorPositionUpdated e)
		{
			this.UpdateAllCouncilors(e.councilor);
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x000EAA2A File Offset: 0x000E8C2A
		public void UpdateAllCouncilors(CouncilorDepartsShip e)
		{
			this.UpdateAllCouncilors(e.councilor);
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x000EAA38 File Offset: 0x000E8C38
		public void UpdateAllCouncilors(TICouncilorState conditionalCouncilor = null)
		{
			if (!this.VisController.UIVisualizationOnly)
			{
				List<TICouncilorState> list = this.ShipState.CouncilorStatesPresentAndKnownToFaction(GameControl.control.activePlayer);
				List<TICouncilorState> list2 = (from x in this.councilorControllers.Keys
					where x.councilor != null
					select x.councilor).ToList<TICouncilorState>();
				if (!(conditionalCouncilor == null) && !list.Contains(conditionalCouncilor) && !list2.Contains(conditionalCouncilor))
				{
					return;
				}
				List<TICouncilorState> list3 = list2.Except<TICouncilorState>(list).ToList<TICouncilorState>();
				foreach (SpaceCouncilorController spaceCouncilorController in this.councilorControllers.Keys)
				{
					if (list3.Contains(spaceCouncilorController.councilor))
					{
						spaceCouncilorController.currentlyActive = false;
						spaceCouncilorController.primaryCanvas.enabled = false;
						spaceCouncilorController.StopCentralIconAnimation();
						spaceCouncilorController.councilor = null;
						spaceCouncilorController.gameObject.SetActive(false);
					}
					if (list.Contains(spaceCouncilorController.councilor))
					{
						spaceCouncilorController.currentlyActive = true;
						spaceCouncilorController.primaryCanvas.enabled = true;
						spaceCouncilorController.gameObject.SetActive(true);
					}
				}
				using (List<TICouncilorState>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						TICouncilorState ticouncilorState = enumerator2.Current;
						if (!list2.Contains(ticouncilorState))
						{
							int num = 0;
							for (int i = 0; i <= 48; i++)
							{
								if (!this.councilorControllers.Values.Contains(i))
								{
									num = i;
									break;
								}
							}
							this.councilorControllers.Add(this.ModelController.AddCouncilorMarker(ticouncilorState, this.ShipState, num), num);
						}
					}
					return;
				}
			}
			this.SetAllCouncilorsActive(false);
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x000EAC44 File Offset: 0x000E8E44
		public override float ApplyDamage(DamageSource source)
		{
			float num = 0f;
			if (base.destructionTriggered)
			{
				return num;
			}
			num += this.hull.ApplyDamage(source, base.transform);
			if (this.hull.IsDestroyed())
			{
				this.TriggerShipDestruction();
			}
			return num;
		}

		// Token: 0x06002B2D RID: 11053 RVA: 0x000EAC8C File Offset: 0x000E8E8C
		private void TriggerShipDestruction()
		{
			GameControl.eventManager.TriggerEvent(new ShipDestroyed(this.ShipState, null, null, null), null, Array.Empty<object>());
			base.destructionTriggered = true;
			this.VisController.DisableAllThrusterFX();
			this.ShipState.DeactivateThrusters();
			this.DestroyShipVisualization();
		}

		// Token: 0x06002B2E RID: 11054 RVA: 0x000EACDA File Offset: 0x000E8EDA
		private void OnShipDestroyedByHeat(ShipDestroyedByHeat e)
		{
			this.TriggerShipDestruction();
		}

		// Token: 0x06002B2F RID: 11055 RVA: 0x000EACE4 File Offset: 0x000E8EE4
		private void DestroyShipVisualization()
		{
			if (this.ModelController.destructionEffectController)
			{
				this.ModelController.StartDestructionSequence();
				return;
			}
			this.ToggleExplosions();
			base.Invoke("DestroyShipParts", 0.75f);
			base.StartCoroutine(this.RemoveShipObject());
		}

		// Token: 0x06002B30 RID: 11056 RVA: 0x000EAD34 File Offset: 0x000E8F34
		private void ToggleExplosions()
		{
			foreach (ParticleSystem particleSystem in this.ModelController.smallExplosionParticleSystems)
			{
				particleSystem.gameObject.SetActive(false);
				particleSystem.transform.localPosition = new Vector3(particleSystem.transform.localPosition.x - 0.1f + TIUtilities.RandomRange(0f, 0.2f), particleSystem.transform.localPosition.y - 0.1f + TIUtilities.RandomRange(0f, 0.2f), particleSystem.transform.localPosition.z - 0.2f + TIUtilities.RandomRange(0f, 0.4f));
				particleSystem.transform.localScale = particleSystem.transform.localScale * TIUtilities.RandomRange(0.5f, 1.5f);
				base.StartCoroutine(this.Boom(particleSystem, TIUtilities.RandomRange(0f, 2f)));
				base.StartCoroutine(this.Boom(particleSystem, TIUtilities.RandomRange(2f, 4f)));
			}
		}

		// Token: 0x06002B31 RID: 11057 RVA: 0x000EAE8C File Offset: 0x000E908C
		private IEnumerator Boom(ParticleSystem explosion, float delay)
		{
			yield return new WaitForSeconds(delay);
			while (explosion.isPlaying)
			{
				yield return null;
			}
			explosion.gameObject.SetActive(false);
			explosion.gameObject.SetActive(true);
			yield break;
		}

		// Token: 0x06002B32 RID: 11058 RVA: 0x000EAEA4 File Offset: 0x000E90A4
		private void DestroyShipParts()
		{
			this.ModelController.destructionExplosionParticleSystem.transform.localScale = this.ModelController.destructionExplosionParticleSystem.transform.localScale * TIUtilities.RandomRange(2f, 3f);
			this.ModelController.destructionExplosionParticleSystem.gameObject.SetActive(true);
			this.ModelController.OnDestructionStart();
			this.visualizationOff = true;
		}

		// Token: 0x06002B33 RID: 11059 RVA: 0x000EAF17 File Offset: 0x000E9117
		private IEnumerator RemoveShipObject()
		{
			while (!this.visualizationOff)
			{
				yield return null;
			}
			while (this.ModelController.destructionExplosionParticleSystem.isPlaying)
			{
				yield return null;
			}
			this.ModelController.OnDestructionComplete();
			yield break;
		}

		// Token: 0x06002B34 RID: 11060 RVA: 0x000EAF28 File Offset: 0x000E9128
		private void Update()
		{
			if (this.VisController == null)
			{
				this.DisableStratController();
				return;
			}
			if (!this.VisController.UIVisualizationOnly)
			{
				TISpaceShipState shipState = this.ShipState;
				if (shipState != null && !shipState.ShipDestroyed())
				{
					if (this.dataDirty)
					{
						if (this.ShipState.fleet != null && this.ShipState.fleet.inTransfer && (this.ShipState.fleet.inAccelerationPhase || this.ShipState.fleet.inDecelerationPhase))
						{
							this.ModelController.ActivateThrusters(true);
						}
						else
						{
							this.ModelController.DeactivateThrusters(false);
							this.ModelController.DeactivateAllVectorThrusters();
						}
						this.dataDirty = false;
					}
					if (this.ShipState.inManeuver)
					{
						TIDateTime tidateTime = TITimeState.Now();
						if (this.ShipState.inManeuverSequence)
						{
							this.UpdateManeuverThrusterFX(tidateTime);
						}
						else
						{
							this.UpdateFleetOffsetFX(tidateTime);
						}
					}
					else if (this.previousManeuver != null)
					{
						this.ModelController.DeactivateAllVectorThrusters();
						this.previousManeuver = null;
					}
					this.VisController.transform.localPosition = (Vector3)this.ShipState.currentFleetOffset;
					base.transform.localRotation = this.ShipState.currentRotation;
					return;
				}
			}
			this.VisController.DisableAllThrusterFX();
			this.DisableStratController();
		}

		// Token: 0x06002B35 RID: 11061 RVA: 0x000EB07C File Offset: 0x000E927C
		public void SetDirty()
		{
			this.dataDirty = true;
		}

		// Token: 0x06002B36 RID: 11062 RVA: 0x000EB085 File Offset: 0x000E9285
		private void OnEnable()
		{
			this.dataDirty = true;
			this.SetAllCouncilorsActive(true);
			GameControl.eventManager.AddListener<ShipVisualizationDataDirty>(new EventManager.EventDelegate<ShipVisualizationDataDirty>(this.OnShipVisualizationDataDirty), null, this.ShipState, true, false);
		}

		// Token: 0x06002B37 RID: 11063 RVA: 0x000EB0B4 File Offset: 0x000E92B4
		private void OnDisable()
		{
			if (this.VisController != null && this.ModelController != null)
			{
				this.ModelController.StopThrusterAudio();
			}
			GameControl.eventManager.RemoveListener<ShipVisualizationDataDirty>(new EventManager.EventDelegate<ShipVisualizationDataDirty>(this.OnShipVisualizationDataDirty), null);
		}

		// Token: 0x06002B38 RID: 11064 RVA: 0x000EB0F4 File Offset: 0x000E92F4
		private void OnDestroy()
		{
			this.RemoveListeners();
		}

		// Token: 0x04002109 RID: 8457
		private const int Accelerate_Thrusters_Phase = 1;

		// Token: 0x0400210A RID: 8458
		private const int Deccelerate_Thrusters_Phase = 2;

		// Token: 0x0400210B RID: 8459
		private const int Idle_Thrusters_Phase = 0;

		// Token: 0x0400210C RID: 8460
		private CameraManager cameraManager;

		// Token: 0x0400210D RID: 8461
		private bool dataDirty;

		// Token: 0x0400210E RID: 8462
		private bool initializedUiOnly;

		// Token: 0x0400210F RID: 8463
		private int _stratLayerThrusterPhase;

		// Token: 0x04002110 RID: 8464
		private bool inPreviousBurn;

		// Token: 0x04002111 RID: 8465
		private bool inPreviousCounterBurn;

		// Token: 0x04002112 RID: 8466
		private ITrajectory previousManeuver;

		// Token: 0x04002113 RID: 8467
		private bool visualizationOff;

		// Token: 0x04002115 RID: 8469
		public FleetVisController FleetVisController;

		// Token: 0x04002116 RID: 8470
		public Dictionary<SpaceCouncilorController, int> councilorControllers = new Dictionary<SpaceCouncilorController, int>();
	}
}
