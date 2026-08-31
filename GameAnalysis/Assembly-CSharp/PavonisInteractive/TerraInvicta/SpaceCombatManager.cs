using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using PavonisInteractive.TerraInvicta.Actions;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.GamePlayScript.AI;
using PavonisInteractive.TerraInvicta.Jobs;
using PavonisInteractive.TerraInvicta.Ship;
using PavonisInteractive.TerraInvicta.SpaceCombat;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Shapes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace PavonisInteractive.TerraInvicta
{
	// Token: 0x020006F0 RID: 1776
	public class SpaceCombatManager : MonoBehaviour
	{
		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x06002957 RID: 10583 RVA: 0x000DC36E File Offset: 0x000DA56E
		// (set) Token: 0x06002958 RID: 10584 RVA: 0x000DC376 File Offset: 0x000DA576
		public CombatGrid combatGrid { get; private set; }

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06002959 RID: 10585 RVA: 0x000DC37F File Offset: 0x000DA57F
		// (set) Token: 0x0600295A RID: 10586 RVA: 0x000DC387 File Offset: 0x000DA587
		public TISpaceCombatState combatState { get; private set; }

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x0600295B RID: 10587 RVA: 0x000DC390 File Offset: 0x000DA590
		// (set) Token: 0x0600295C RID: 10588 RVA: 0x000DC398 File Offset: 0x000DA598
		public SpaceCombatCanvasController combatHUD { get; private set; }

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x0600295D RID: 10589 RVA: 0x000DC3A1 File Offset: 0x000DA5A1
		// (set) Token: 0x0600295E RID: 10590 RVA: 0x000DC3A9 File Offset: 0x000DA5A9
		public SpaceCombatCameraController combatCamera { get; private set; }

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x0600295F RID: 10591 RVA: 0x000DC3B2 File Offset: 0x000DA5B2
		// (set) Token: 0x06002960 RID: 10592 RVA: 0x000DC3BA File Offset: 0x000DA5BA
		public Camera mainCamera { get; private set; }

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x06002961 RID: 10593 RVA: 0x000DC3C3 File Offset: 0x000DA5C3
		// (set) Token: 0x06002962 RID: 10594 RVA: 0x000DC3CB File Offset: 0x000DA5CB
		public Transform mainCameraTransform { get; private set; }

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x06002963 RID: 10595 RVA: 0x000DC3D4 File Offset: 0x000DA5D4
		public float modelScalingFactor
		{
			get
			{
				return 0.01f * SpaceCombatManager.GetScalingAdjustmentFactor();
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x06002964 RID: 10596 RVA: 0x000DC3E1 File Offset: 0x000DA5E1
		public float projectileScalingFactor
		{
			get
			{
				return 0.025f * SpaceCombatManager.GetScalingAdjustmentFactor();
			}
		}

		// Token: 0x06002965 RID: 10597 RVA: 0x000DC3EE File Offset: 0x000DA5EE
		public static float GetScalingAdjustmentFactor()
		{
			return SpaceCombatManager._cachedScalingAdjustmentFactor;
		}

		// Token: 0x06002966 RID: 10598 RVA: 0x000DC3F5 File Offset: 0x000DA5F5
		public static void SetScalingAdjustmentFactor()
		{
			if (GameStateManager.GlobalValues() == null || GameStateManager.GlobalValues().scenarioCustomizations.cinematicCombatRealismScale)
			{
				SpaceCombatManager._cachedScalingAdjustmentFactor = 1f;
				return;
			}
			SpaceCombatManager._cachedScalingAdjustmentFactor = 0.1f;
		}

		// Token: 0x06002967 RID: 10599 RVA: 0x000DC42A File Offset: 0x000DA62A
		public static float GetFormationScalingFactor()
		{
			return SpaceCombatManager.GetScalingAdjustmentFactor();
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06002968 RID: 10600 RVA: 0x000DC431 File Offset: 0x000DA631
		public TIPromptQueueState promptQueue
		{
			get
			{
				return GameStateManager.PromptQueue();
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06002969 RID: 10601 RVA: 0x000DC438 File Offset: 0x000DA638
		public GameObjectDictionary<string> container
		{
			get
			{
				if (this._container == null)
				{
					this._container = new GameObjectDictionary<string>("Space Combat Container");
				}
				return this._container;
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x0600296A RID: 10602 RVA: 0x000DC458 File Offset: 0x000DA658
		public bool IsHandlingWaypointInput
		{
			get
			{
				return this._waypointInputDragging;
			}
		}

		// Token: 0x0600296B RID: 10603 RVA: 0x000DC460 File Offset: 0x000DA660
		public void SetWaypointDragging(bool value)
		{
			this._waypointInputDragging = value;
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x0600296C RID: 10604 RVA: 0x000DC469 File Offset: 0x000DA669
		public bool IsInFormationSelectionMode
		{
			get
			{
				return this._inFormationSelectionMode;
			}
		}

		// Token: 0x170005BE RID: 1470
		// (get) Token: 0x0600296D RID: 10605 RVA: 0x000DC471 File Offset: 0x000DA671
		public bool IsDragSelecting
		{
			get
			{
				return this._isDragSelecting;
			}
		}

		// Token: 0x0600296E RID: 10606 RVA: 0x000DC479 File Offset: 0x000DA679
		public static Vector3 km_to_scale_vec3(Vector3 distance_km)
		{
			return distance_km * 0.05f;
		}

		// Token: 0x0600296F RID: 10607 RVA: 0x000DC486 File Offset: 0x000DA686
		public static float km_to_scale(float distance_km)
		{
			return distance_km * 0.05f;
		}

		// Token: 0x06002970 RID: 10608 RVA: 0x000DC48F File Offset: 0x000DA68F
		public static float kps_to_scale(float velocity_kps)
		{
			return velocity_kps * 0.05f;
		}

		// Token: 0x06002971 RID: 10609 RVA: 0x000DC498 File Offset: 0x000DA698
		public static float scale_to_kps(float velocity)
		{
			return velocity / 0.05f;
		}

		// Token: 0x06002972 RID: 10610 RVA: 0x000DC4A1 File Offset: 0x000DA6A1
		public static float scale_to_km(float distance)
		{
			return distance / 0.05f;
		}

		// Token: 0x06002973 RID: 10611 RVA: 0x000DC4AA File Offset: 0x000DA6AA
		public static Vector3 scale_to_km_vec3(Vector3 value)
		{
			return value / 0.05f;
		}

		// Token: 0x06002974 RID: 10612 RVA: 0x000DC4B7 File Offset: 0x000DA6B7
		public static Vector3 vector_km_to_scale(Vector3 vector_km)
		{
			return vector_km * 0.05f;
		}

		// Token: 0x06002975 RID: 10613 RVA: 0x000DC4C4 File Offset: 0x000DA6C4
		public static float g_to_kps2(float gs)
		{
			return gs * 0.00980665f;
		}

		// Token: 0x06002976 RID: 10614 RVA: 0x000DC4CD File Offset: 0x000DA6CD
		public static float kps2_to_scale(float accel_kps2)
		{
			return accel_kps2 * 0.05f;
		}

		// Token: 0x06002977 RID: 10615 RVA: 0x000DC4D6 File Offset: 0x000DA6D6
		public static float g_to_scale(float gs)
		{
			return SpaceCombatManager.kps2_to_scale(SpaceCombatManager.g_to_kps2(gs));
		}

		// Token: 0x06002978 RID: 10616 RVA: 0x000DC4E3 File Offset: 0x000DA6E3
		public static float scale_to_kps2(float acceleration)
		{
			return acceleration / 0.05f;
		}

		// Token: 0x06002979 RID: 10617 RVA: 0x000DC4EC File Offset: 0x000DA6EC
		public static float kps2_to_g(float acceleration_kps2)
		{
			return acceleration_kps2 * 1000f / 9.80665f;
		}

		// Token: 0x0600297A RID: 10618 RVA: 0x000DC4FC File Offset: 0x000DA6FC
		public static float acceleration_kps(Vector3 oldVector, Vector3 newVector)
		{
			return (newVector - oldVector).magnitude / 0.05f;
		}

		// Token: 0x0600297B RID: 10619 RVA: 0x000DC520 File Offset: 0x000DA720
		public static float DVconsumption_kps(Vector3 oldVector, Vector3 newVector, TISpaceShipState ship, float acceleration, float massPriorToBurn_kg)
		{
			float num = SpaceCombatManager.acceleration_kps(oldVector, newVector);
			float num2 = acceleration / 0.05f;
			return ship.DVconsumedInCombat(num, num2, massPriorToBurn_kg);
		}

		// Token: 0x0600297C RID: 10620 RVA: 0x000DC547 File Offset: 0x000DA747
		public void SetCombat(TISpaceCombatState combat)
		{
			this.combatState = combat;
		}

		// Token: 0x0600297D RID: 10621 RVA: 0x000DC550 File Offset: 0x000DA750
		public bool HasActiveState()
		{
			return TIGameState.Valid(this.combatState) && this.combatState.active;
		}

		// Token: 0x0600297E RID: 10622 RVA: 0x000DC56C File Offset: 0x000DA76C
		private void InitializeProjectilePool(int count)
		{
			for (int i = 0; i < count; i++)
			{
				TISpaceCombatProjectileState tispaceCombatProjectileState = GameStateManager.CreateNewGameState<TISpaceCombatProjectileState>();
				tispaceCombatProjectileState.Initialize();
				this._projectiles.Add(tispaceCombatProjectileState, null);
			}
		}

		// Token: 0x0600297F RID: 10623 RVA: 0x000DC5A0 File Offset: 0x000DA7A0
		private TISpaceCombatProjectileState GetAvailableProjectileState(ShipWeaponVisController visController)
		{
			TISpaceCombatProjectileState tispaceCombatProjectileState = null;
			foreach (KeyValuePair<ProjectileController, TISpaceCombatProjectileState> keyValuePair in this._reverseProjectiles)
			{
				if (keyValuePair.Key != null && !keyValuePair.Key.gameObject.activeSelf)
				{
					if (keyValuePair.Key.weaponController == visController)
					{
						return keyValuePair.Value;
					}
					if (tispaceCombatProjectileState == null)
					{
						tispaceCombatProjectileState = keyValuePair.Value;
					}
				}
			}
			if (tispaceCombatProjectileState != null)
			{
				return tispaceCombatProjectileState;
			}
			foreach (KeyValuePair<TISpaceCombatProjectileState, ProjectileController> keyValuePair2 in this._projectiles)
			{
				if (keyValuePair2.Value == null)
				{
					return keyValuePair2.Key;
				}
			}
			TISpaceCombatProjectileState tispaceCombatProjectileState2 = GameStateManager.CreateNewGameState<TISpaceCombatProjectileState>();
			tispaceCombatProjectileState2.Initialize();
			this._projectiles.Add(tispaceCombatProjectileState2, null);
			return tispaceCombatProjectileState2;
		}

		// Token: 0x06002980 RID: 10624 RVA: 0x000DC6C4 File Offset: 0x000DA8C4
		public ProjectileController SetProjectile(ShipWeaponVisController visController)
		{
			GameObject gameObject = null;
			TISpaceCombatProjectileState availableProjectileState = this.GetAvailableProjectileState(visController);
			ProjectileController projectileController = this._projectiles[availableProjectileState];
			if (projectileController != null)
			{
				gameObject = projectileController.gameObject;
			}
			bool flag = projectileController != null && projectileController.weaponController.projectileResource == visController.projectileResource;
			bool flag2 = projectileController == null;
			if (flag2 || !flag)
			{
				if (!flag2)
				{
					this._reverseProjectiles.Remove(projectileController);
					global::UnityEngine.Object.Destroy(projectileController.gameObject);
				}
				gameObject = global::UnityEngine.Object.Instantiate<GameObject>(visController.projectilePrefab, this._projectileContainer.transform);
				projectileController = gameObject.GetComponent<ProjectileController>();
				this._projectiles[availableProjectileState] = projectileController;
				this._reverseProjectiles.Add(projectileController, availableProjectileState);
			}
			projectileController.Initialize(this._projectileJobContainer, visController, availableProjectileState);
			gameObject.transform.position = visController.firePoint.transform.position;
			gameObject.transform.localPosition = Vector3.zero;
			if (gameObject.transform.GetComponent<MeshRenderer>() != null)
			{
				gameObject.transform.localScale = gameObject.transform.localScale * this.projectileScalingFactor;
			}
			gameObject.SetActive(false);
			return projectileController;
		}

		// Token: 0x06002981 RID: 10625 RVA: 0x000DC7F4 File Offset: 0x000DA9F4
		public void AddPath(Vector3 start, Vector3 end, LineEndCap endCap, Color color)
		{
			SpaceCombatManager.CombatPathLine combatPathLine = new SpaceCombatManager.CombatPathLine
			{
				start = start,
				end = end,
				endCap = endCap,
				color = color
			};
			this.combatPathLines.Add(combatPathLine);
		}

		// Token: 0x06002982 RID: 10626 RVA: 0x000DC830 File Offset: 0x000DAA30
		public void SetPathEndPointToShipPosition(Vector3 newPosition)
		{
			SpaceCombatManager.CombatPathLine combatPathLine = this.combatPathLines[this.combatPathLines.Count - 1];
			combatPathLine.end = newPosition;
			this.combatPathLines.RemoveAt(this.combatPathLines.Count - 1);
			this.combatPathLines.Add(combatPathLine);
		}

		// Token: 0x06002983 RID: 10627 RVA: 0x000DC884 File Offset: 0x000DAA84
		public void RenderLines(Camera cam)
		{
			if (cam != this.mainCamera)
			{
				return;
			}
			Draw.LineGeometry = LineGeometry.Billboard;
			Draw.ThicknessSpace = ThicknessSpace.Meters;
			Plane plane = new Plane(this.mainCamera.transform.forward, this.mainCamera.transform.position);
			float num = 0.05f * SpaceCombatManager.GetScalingAdjustmentFactor();
			float num2 = 0.1f;
			using (Draw.Command(cam, CameraEvent.BeforeImageEffects))
			{
				foreach (SpaceCombatManager.CombatPathLine combatPathLine in this.combatPathLines)
				{
					Draw.Thickness = Mathf.Clamp(plane.GetDistanceToPoint(combatPathLine.start) * num, num, 0.5f) * num2 * 0.9f;
					Draw.Line(combatPathLine.start, combatPathLine.end, combatPathLine.endCap, combatPathLine.color);
				}
			}
			this.combatPathLines.Clear();
		}

		// Token: 0x06002984 RID: 10628 RVA: 0x000DC99C File Offset: 0x000DAB9C
		private void ApplyCombatCameraEffect()
		{
			if (this.combatCameraBlendEffect == null)
			{
				this.combatCameraBlendEffect = new Material(Shader.Find("Unlit/CustomCameraLayerBlend"));
			}
			this.backgroundCamera = new GameObject("Combat Background Camera").AddComponent<Camera>();
			this.backgroundCamera.CopyFrom(this.mainCamera);
			this.backgroundCamera.clearFlags = CameraClearFlags.Skybox;
			this.backgroundCamera.cullingMask = 32768;
			this.backgroundCamera.depth = -2f;
			this.backgroundCamera.nearClipPlane = 10f;
			this.backgroundCamera.farClipPlane = 25000f;
			this.backgroundCameraTransform = this.backgroundCamera.transform;
			this.backgroundCameraTransform.SetParent(null);
			this.backgroundCameraTransform.localRotation = Quaternion.identity;
			this.backgroundCameraTransform.localPosition = Vector3.zero;
			this.initialCameraClearFlags = this.mainCamera.clearFlags;
			this.initialMainCameraCullingMask = this.mainCamera.cullingMask;
			this.mainCamera.clearFlags = CameraClearFlags.Color;
			this.mainCamera.cullingMask &= -32769;
			this.mainCamera.cullingMask &= -2049;
			this.combatCameraBlend = this.mainCamera.gameObject.AddComponent<SpaceCombatCameraBlend>();
			this.combatCameraBlend.SetUp(this.combatCameraBlendEffect, this.backgroundCamera);
		}

		// Token: 0x06002985 RID: 10629 RVA: 0x000DCB04 File Offset: 0x000DAD04
		private void RemoveCombatCameraEffect()
		{
			this.combatCameraBlend.CleanUp();
			global::UnityEngine.Object.Destroy(this.combatCameraBlend);
			global::UnityEngine.Object.Destroy(this.backgroundCamera.gameObject);
			this.combatCameraBlendEffect = null;
			this.mainCamera.cullingMask = this.initialMainCameraCullingMask;
			this.mainCamera.clearFlags = this.initialCameraClearFlags;
			this.minimumDistanceToPrimarySaceBody = 0f;
		}

		// Token: 0x06002986 RID: 10630 RVA: 0x000DCB6C File Offset: 0x000DAD6C
		public void Initialize()
		{
			if (this.initialized)
			{
				Log.Warn("Space Combat Manager attempting to initialize again", Array.Empty<object>());
				return;
			}
			SpaceCombatManager.SetScalingAdjustmentFactor();
			this.mainCamera = Camera.main;
			this.mainCameraTransform = this.mainCamera.transform;
			this.originalFarClipPlane = this.mainCamera.farClipPlane;
			this.mainCamera.farClipPlane = this.spaceCombatFarClipPlane;
			this.combatEndTriggered = false;
			this.combatEnded = false;
			this.shotFired = false;
			this.shipDestroyed = false;
			this.timeOfLastShotFired = null;
			this.priorIntensity = AudioManager.GetIntensity();
			AudioManager.SetIntensity(0f);
			this.combatState.votedEndCombat.Clear();
			this.combatState.votedEndCombatFirst = null;
			this._sendInPlayerReinforcements = false;
			this.endCombatTime = 0.0;
			this.forceEndCombatTime = -1.0;
			this._isDragSelecting = false;
			this._boxSelectedUIControllers = new List<ShipUIController>();
			this._controlGroups = new Dictionary<int, List<TISpaceShipState>>();
			this.waypointsVisible = true;
			this.combatState.votedEndCombat.Add(this.combatState.factions[0], false);
			this.combatState.votedEndCombat.Add(this.combatState.factions[1], false);
			if (this.combatState.stances[this.combatState.factions[0]] == CombatStance.Evade)
			{
				this.setup = CombatSetup.Fleet1ChaseFleet0;
				if (this.combatState.factions[0] != GameControl.control.activePlayer)
				{
					this.combatState.factions[0].playerControl.StartAction(new SetEndCombatVoteAction(this.combatState.factions[0], true));
				}
			}
			else if (this.combatState.stances[this.combatState.factions[1]] == CombatStance.Evade)
			{
				this.setup = CombatSetup.Fleet0ChaseFleet1;
				if (this.combatState.factions[1] != GameControl.control.activePlayer)
				{
					this.combatState.factions[1].playerControl.StartAction(new SetEndCombatVoteAction(this.combatState.factions[1], true));
				}
			}
			else
			{
				this.setup = CombatSetup.Confrontation;
			}
			this.ships = new List<CombatShipController>();
			this.activeShips = new List<CombatShipController>();
			this.fleetControllers = new List<CombatFleetController>();
			this.combatHabModuleControllers = new List<CombatHabModuleController>();
			this.combatantLookup = new Dictionary<CombatTargetableState, CombatantController>();
			float num = 0f;
			for (int i = 0; i < this.combatState.fleets.Length; i++)
			{
				float num2;
				if (this.combatState.fleets[i] != null && this.combatState.hab != null && this.combatState.hab.faction == this.combatState.fleets[i].faction)
				{
					num2 = Mathf.Max(this.combatState.hab.CombatRange_km(), this.combatState.fleets[i].CombatRange_km());
				}
				else
				{
					num2 = ((this.combatState.fleets[i] == null) ? this.combatState.hab.CombatRange_km() : this.combatState.fleets[i].CombatRange_km());
				}
				num = Mathf.Max(num, num2);
			}
			num += TIGlobalConfig.globalConfig.extraStartingCombatDistance_km;
			this.secondFleetOffset = new Vector3(0f, 0f, ((this.setup == CombatSetup.Confrontation) ? 1.225f : 1.05f) * Mathf.Max(num, 500f));
			this.secondFleetOffset += global::UnityEngine.Random.insideUnitCircle.normalized * TIUtilities.RandomRange(25f, 35f);
			this.scaledSecondFleetOffset = this.secondFleetOffset * 0.05f;
			if (this.combatState.hab != null)
			{
				if (this.combatState.hab.faction.player.isAI || GameControl.control.skirmishMode)
				{
					this.combatState.hab.UpdatePowerManagement(true, null, this.combatState.hab.faction.player.isAI);
				}
				List<TIHabModuleState> list = this.combatState.hab.ActiveCombatModules();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (TIHabModuleState tihabModuleState in list)
				{
					HabModuleController habModuleController = tihabModuleState.ref_habModule.HabModuleController;
					CombatHabModuleController combatHabModuleController = habModuleController.CombatHabModuleController;
					combatHabModuleController.InitializeForCombat(tihabModuleState, habModuleController);
					this.combatHabModuleControllers.Add(combatHabModuleController);
					this.combatantLookup.Add(tihabModuleState, combatHabModuleController);
				}
				Debug.Log(stringBuilder.ToString());
				foreach (CombatHabModuleController combatHabModuleController2 in this.combatHabModuleControllers)
				{
					combatHabModuleController2.alliedCombatants.AddRange(this.combatHabModuleControllers);
				}
			}
			this._reinforcementCount = new Dictionary<TIFactionState, int>(2);
			int num3 = 0;
			TISpaceFleetState tispaceFleetState = this.combatState.fleets[0];
			if (((tispaceFleetState != null) ? tispaceFleetState.ships : null) != null)
			{
				num3 += this.combatState.fleets[0].ships.Count;
			}
			TISpaceFleetState tispaceFleetState2 = this.combatState.fleets[1];
			if (((tispaceFleetState2 != null) ? tispaceFleetState2.ships : null) != null)
			{
				num3 += this.combatState.fleets[1].ships.Count;
			}
			this._maxShipsInBattle = new Dictionary<TISpaceFleetState, int>();
			for (int j = 0; j < this.combatState.fleets.Length; j++)
			{
				TISpaceFleetState tispaceFleetState3 = this.combatState.fleets[j];
				if (tispaceFleetState3 != null)
				{
					TISpaceFleetState tispaceFleetState4 = this.combatState.fleets[(j + 1) % this.combatState.fleets.Length];
					int num4 = tispaceFleetState3.ships.Count;
					if (tispaceFleetState4 != null && num3 > TIPlayerProfileManager.maxShipsInCombat)
					{
						if (j != 0)
						{
							if (j == 1)
							{
								num4 = TIPlayerProfileManager.maxShipsInCombat - this._maxShipsInBattle[tispaceFleetState4];
								this._maxShipsInBattle.Add(tispaceFleetState3, num4);
							}
						}
						else
						{
							float num5 = Mathf.Clamp((float)tispaceFleetState3.ships.Count / (float)num3, 0.33333334f, 0.6666667f);
							num4 = Math.Min(num4, (int)((float)TIPlayerProfileManager.maxShipsInCombat * num5));
							if (num4 + tispaceFleetState4.ships.Count < TIPlayerProfileManager.maxShipsInCombat)
							{
								num4 = TIPlayerProfileManager.maxShipsInCombat - tispaceFleetState4.ships.Count;
							}
							this._maxShipsInBattle.Add(tispaceFleetState3, num4);
						}
					}
					if (num4 > TIPlayerProfileManager.maxShipsInCombat)
					{
						num4 = TIPlayerProfileManager.maxShipsInCombat;
					}
					this._reinforcementCount[tispaceFleetState3.faction] = 0;
					if (tispaceFleetState4 != null)
					{
						this._reinforcementCount[tispaceFleetState4.faction] = 0;
					}
					else if (this.combatState.hab != null && this.combatState.hab.faction != tispaceFleetState3.faction)
					{
						this._reinforcementCount[this.combatState.hab.faction] = 0;
					}
					List<CombatShipController> list2 = new List<CombatShipController>(num4);
					List<TISpaceShipState> list3 = new List<TISpaceShipState>(tispaceFleetState3.ships.Count);
					bool flag = this.combatState.hab != null && this.combatState.hab.faction == tispaceFleetState3.ref_faction;
					Vector3 vector = Vector3.zero;
					if (j == 0)
					{
						switch (this.setup)
						{
						case CombatSetup.Confrontation:
							if (flag)
							{
								vector = new Vector3(0f, 0f, 0.1f) * 0.05f;
							}
							else
							{
								vector = new Vector3(0f, 0f, 0.5f) * 0.05f;
							}
							break;
						case CombatSetup.Fleet0ChaseFleet1:
							vector = new Vector3(0f, 0f, 0.6f) * 0.05f;
							break;
						case CombatSetup.Fleet1ChaseFleet0:
							vector = new Vector3(0f, 0f, -0.1f) * 0.05f;
							break;
						}
					}
					else
					{
						switch (this.setup)
						{
						case CombatSetup.Confrontation:
							if (flag)
							{
								vector = new Vector3(0f, 0f, -0.1f) * 0.05f;
							}
							else
							{
								vector = new Vector3(0f, 0f, -0.5f) * 0.05f;
							}
							break;
						case CombatSetup.Fleet0ChaseFleet1:
							vector = new Vector3(0f, 0f, 0.1f) * 0.05f;
							break;
						case CombatSetup.Fleet1ChaseFleet0:
							vector = new Vector3(0f, 0f, -0.6f) * 0.05f;
							break;
						}
					}
					tispaceFleetState3.ships.Sort(delegate(TISpaceShipState a, TISpaceShipState b)
					{
						int num6 = a.nonCombatant.CompareTo(b.nonCombatant);
						if (num6 == 0)
						{
							num6 = -1 * a.SpaceCombatValue(false, 0f).CompareTo(b.SpaceCombatValue(false, 0f));
						}
						return num6;
					});
					if (!tispaceFleetState3.faction.isActivePlayer)
					{
						tispaceFleetState3.AssignFormation(this.SetAIFormation(tispaceFleetState3), j == 1, false, false, true, false);
					}
					else
					{
						tispaceFleetState3.AssignFormation(tispaceFleetState3.defaultHumanCombatFormation, j == 1, false, false, true, false);
					}
					List<TISpaceShipState> list4 = new List<TISpaceShipState>();
					for (int k = 0; k < num4; k++)
					{
						list4.Add(tispaceFleetState3.ships[k]);
					}
					foreach (TISpaceShipState tispaceShipState in list4)
					{
						tispaceShipState.SetCombatFormationOffset(list4, tispaceFleetState3.formation, num4, j == 1, true);
					}
					foreach (TISpaceShipState tispaceShipState2 in tispaceFleetState3.ships)
					{
						if (tispaceShipState2.ShipDestroyed())
						{
							Log.Warn("Destroyed Ship " + tispaceShipState2.displayName + " entering combat for some reason.", Array.Empty<object>());
						}
						if (list2.Count < num4)
						{
							Vector3 vector2 = Vector3.zero;
							if (j != 0)
							{
								if (j == 1)
								{
									vector2 = (Vector3)(tispaceShipState2.fleetFormationOffset + this.secondFleetOffset) * 0.05f;
									if (vector2.z < this.scaledSecondFleetOffset.z)
									{
										Log.Warn("Fleet 1 Formation placed ship with bad z: " + vector2.z.ToString(), Array.Empty<object>());
										for (int l = 0; l < 10; l++)
										{
											vector2 = new Vector3(vector2.x, vector2.y, vector2.z + this.scaledSecondFleetOffset.z);
											if (vector2.z >= this.scaledSecondFleetOffset.z)
											{
												break;
											}
										}
									}
								}
							}
							else
							{
								vector2 = (Vector3)tispaceShipState2.fleetFormationOffset * 0.05f;
								if (vector2.z > 0f)
								{
									Log.Warn("Fleet 0 Formation placed ship with bad z: " + vector2.z.ToString() + " Setting to 0", Array.Empty<object>());
									vector2.z = 0f;
								}
							}
							CombatShipController combatShipController = this.CreateShip(vector2, vector, tispaceShipState2);
							list2.Add(combatShipController);
						}
						else
						{
							list3.Add(tispaceShipState2);
						}
					}
					CombatFleetController combatFleetController = new CombatFleetController(j, vector.magnitude, tispaceFleetState3, ((tispaceFleetState3 != null) ? tispaceFleetState3.faction : null) ?? this.combatState.hab.faction, list2, list3, tispaceFleetState3.gameObjectLink);
					this.fleetControllers.Add(combatFleetController);
					MusicController.Instance.ChangeMusicScene();
				}
			}
			Log.Time("<color=#00cc00>LoadTime:</color> CreateCombatVFXBuffer", delegate
			{
				TIVFXManager.Instance.CreateCombatVFXBuffer(this.ships);
			}, true, true);
			if (this.combatState.hab != null)
			{
				this.ShowHab(this.combatState.hab, this.fleetControllers);
			}
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this._combatAIController = new CombatAIController(this, this.gameTime.currentTime, this.fleetControllers.ToArray(), this.combatHabModuleControllers.ToArray());
			if (this._projectileContainer == null)
			{
				this._projectileContainer = new GameObject("Projectile Container");
				this._projectileContainer.transform.SetParent(this.container.transform);
				this._projectileContainer.transform.localScale = Vector3.one;
				this._projectileJobContainer = this._projectileContainer.AddComponent<ProjectileJobContainer>();
			}
			this._projectiles = new Dictionary<TISpaceCombatProjectileState, ProjectileController>();
			this._reverseProjectiles = new Dictionary<ProjectileController, TISpaceCombatProjectileState>();
			this.liveMissiles = new Dictionary<TIFactionState, int>();
			this.liveBallistics = new Dictionary<TIFactionState, int>();
			for (int m = 0; m < this.combatState.factions.Length; m++)
			{
				this.liveMissiles.Add(this.combatState.factions[m], 0);
				this.liveBallistics.Add(this.combatState.factions[m], 0);
			}
			this.InitializeProjectilePool(100);
			this.ApplyCombatCameraEffect();
			Camera.onPreRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPreRender, new Camera.CameraCallback(this.RenderLines));
			this.TurnOnFormationSelectionMode();
			this.initialized = true;
		}

		// Token: 0x06002987 RID: 10631 RVA: 0x000DD944 File Offset: 0x000DBB44
		public void LockSegmentSelection()
		{
			this._isSegmentSelectionComplete = true;
			this._isChangePending = false;
			this._pendingWaypointPlacementShip = null;
			this._pendingNearestSegmentWaypointId = -1;
		}

		// Token: 0x06002988 RID: 10632 RVA: 0x000DD962 File Offset: 0x000DBB62
		public void FinalizeWaypointPlacement()
		{
			if (this._activeWaypointPlacementShip != null)
			{
				this._activeWaypointPlacementShip.FinalizeWaypointPlacement();
			}
			this.EndWaypointPlacementHandling();
		}

		// Token: 0x06002989 RID: 10633 RVA: 0x000DD984 File Offset: 0x000DBB84
		public void EndWaypointPlacementHandling()
		{
			this._isSegmentSelectionComplete = false;
			this._isChangePending = false;
			this._pendingWaypointPlacementShip = null;
			this._pendingNearestSegmentWaypointId = -1;
			if (this._activeWaypointPlacementShip != null)
			{
				this._activeWaypointPlacementShip.ClearActiveWaypointPlacementSegment();
			}
			this._activeWaypointPlacementShip = null;
			this._pendingWaypointPlacementShip = null;
			this._activeNearestSegmentWaypointId = -1;
			this._pendingNearestSegmentWaypointId = -1;
		}

		// Token: 0x0600298A RID: 10634 RVA: 0x000DD9E2 File Offset: 0x000DBBE2
		public void ToggleWaypointVisibility()
		{
			this.waypointsVisible = !this.waypointsVisible;
		}

		// Token: 0x0600298B RID: 10635 RVA: 0x000DD9F3 File Offset: 0x000DBBF3
		private void HandleAddWaypointInput()
		{
			if (!this._isSegmentSelectionComplete)
			{
				this.SelectNearestSegment();
				return;
			}
			this.SelectNearestPlacementForSegment();
		}

		// Token: 0x0600298C RID: 10636 RVA: 0x000DDA0C File Offset: 0x000DBC0C
		private void SelectNearestSegment()
		{
			foreach (CombatShipController combatShipController in this.activeShips)
			{
				if (combatShipController.activePlayerShip)
				{
					this._shipToNearestSegment[combatShipController] = combatShipController.FindNearestSegment();
				}
			}
			CombatShipController combatShipController2 = null;
			float num = float.PositiveInfinity;
			int num2 = -1;
			foreach (KeyValuePair<CombatShipController, SegmentProximityData> keyValuePair in this._shipToNearestSegment)
			{
				if (!keyValuePair.Key.isDestroyed && num > keyValuePair.Value.DistanceToSegment)
				{
					num = keyValuePair.Value.DistanceToSegment;
					combatShipController2 = keyValuePair.Key;
					num2 = keyValuePair.Value.WaypointID;
				}
			}
			if (combatShipController2 == this._activeWaypointPlacementShip && num2 == this._activeNearestSegmentWaypointId)
			{
				this._isChangePending = false;
				return;
			}
			if (!this._isChangePending || this._pendingWaypointPlacementShip != combatShipController2 || this._pendingNearestSegmentWaypointId != num2)
			{
				this._isChangePending = true;
				this._initialFrameChangeRequest = (float)TIFrameCounter.FrameCount;
				this._pendingWaypointPlacementShip = combatShipController2;
				this._pendingNearestSegmentWaypointId = num2;
				return;
			}
			if ((float)TIFrameCounter.FrameCount - this._initialFrameChangeRequest > 3f)
			{
				this.SetActiveWaypointPlacementShip();
				this._isChangePending = false;
				this._pendingWaypointPlacementShip = null;
				this._pendingNearestSegmentWaypointId = -1;
			}
		}

		// Token: 0x0600298D RID: 10637 RVA: 0x000DDB8C File Offset: 0x000DBD8C
		private void SetActiveWaypointPlacementShip()
		{
			if (this._pendingWaypointPlacementShip != this._activeWaypointPlacementShip)
			{
				if (this._activeWaypointPlacementShip != null)
				{
					this._activeWaypointPlacementShip.ClearActiveWaypointPlacementSegment();
				}
				this._activeWaypointPlacementShip = this._pendingWaypointPlacementShip;
			}
			if (this._activeWaypointPlacementShip != null)
			{
				this._activeWaypointPlacementShip.UpdateActiveWaypointPlacementSegment();
			}
			this._activeNearestSegmentWaypointId = this._pendingNearestSegmentWaypointId;
		}

		// Token: 0x0600298E RID: 10638 RVA: 0x000DDBF6 File Offset: 0x000DBDF6
		private void SelectNearestPlacementForSegment()
		{
			if (!(this._activeWaypointPlacementShip != null) || !this._activeWaypointPlacementShip.UpdateWaypointPlacementLocation())
			{
				this.EndWaypointPlacementHandling();
			}
		}

		// Token: 0x0600298F RID: 10639 RVA: 0x000DDC1C File Offset: 0x000DBE1C
		public void SendInPlayerReinforcements()
		{
			this._sendInPlayerReinforcements = true;
		}

		// Token: 0x06002990 RID: 10640 RVA: 0x000DDC25 File Offset: 0x000DBE25
		public int GetAvailableReinforcementsCount(TIFactionState faction)
		{
			if (this._reinforcementCount.ContainsKey(faction))
			{
				return this._reinforcementCount[faction];
			}
			return -1;
		}

		// Token: 0x06002991 RID: 10641 RVA: 0x000DDC44 File Offset: 0x000DBE44
		private void GetReinforcementPositionAndVelocity(CombatFleetController fleetController, out Vector3 spawnPosition, out Vector3 velocity)
		{
			int num = 0;
			bool flag = fleetController.faction == GameControl.control.activePlayer;
			DateTime dateTime = TITimeState.Now().ExportTime();
			Vector3[] array = new Vector3[this.fleetControllers.Count];
			for (int i = 0; i < this.fleetControllers.Count; i++)
			{
				if (this.fleetControllers[i] == fleetController)
				{
					num = i;
				}
				int num2 = 1;
				foreach (CombatShipController combatShipController in this.fleetControllers[i].activeShipControllers)
				{
					if (!combatShipController.isDestroyed)
					{
						array[i] += combatShipController.position + combatShipController.velocityAtTime(dateTime) * 60f;
						num2++;
					}
				}
				if (this.combatState.hab != null && this.combatState.hab.faction.permanentAlly(this.fleetControllers[i].faction))
				{
					array[i] += this.habModelController.transform.position;
					num2++;
				}
				array[i] /= (float)num2;
			}
			bool flag2 = array[num] == Vector3.zero;
			Vector3 vector = (flag2 ? array[(num + 1) % array.Length] : array[num]);
			Vector3 vector2 = array[(num + 1) % array.Length];
			float num3 = 1000f;
			Vector3 vector3;
			if (flag2)
			{
				if (flag)
				{
					if (this._playerRandomReinforcementPosition == Vector3.zero)
					{
						this._playerRandomReinforcementPosition = global::UnityEngine.Random.onUnitSphere;
					}
					vector3 = (vector2 - (vector2 + this._playerRandomReinforcementPosition * num3)).normalized;
				}
				else
				{
					if (this._opposingRandomReinforcementPosition == Vector3.zero)
					{
						this._opposingRandomReinforcementPosition = global::UnityEngine.Random.onUnitSphere;
					}
					vector3 = (vector2 - (vector2 + this._opposingRandomReinforcementPosition * num3)).normalized;
				}
			}
			else
			{
				vector3 = (vector2 - vector).normalized;
			}
			if (GameControl.spaceCombat.combatState.stances[this.fleetControllers[num].faction] != CombatStance.Evade)
			{
				velocity = fleetController.InitialVelocty * vector3;
				spawnPosition = vector - vector3 * SpaceCombatManager.km_to_scale(num3);
			}
			else
			{
				velocity = -1f * fleetController.InitialVelocty * vector3;
				spawnPosition = vector2 - vector3 * SpaceCombatManager.km_to_scale(num3);
			}
			IList<CombatShipController> activeShipControllers = this.fleetControllers[(num + 1) % this.fleetControllers.Count].activeShipControllers;
			if (activeShipControllers.Count > 0)
			{
				for (int j = 0; j < 10; j++)
				{
					float num4 = float.MaxValue;
					Vector3 vector4 = Vector3.zero;
					foreach (CombatShipController combatShipController2 in activeShipControllers)
					{
						Vector3 vector5 = combatShipController2.position + 60f * combatShipController2.velocityAtTime(dateTime);
						float sqrMagnitude = (vector5 - spawnPosition).sqrMagnitude;
						if (sqrMagnitude < num4)
						{
							num4 = sqrMagnitude;
							vector4 = vector5;
						}
					}
					Vector3 vector6 = spawnPosition - vector4;
					float num5 = SpaceCombatManager.km_to_scale(num3);
					if (vector6.sqrMagnitude > num5 * num5)
					{
						break;
					}
					float num6 = 2f * (-vector3).Dot(vector6);
					float num7 = vector6.sqrMagnitude - num5 * num5;
					float num8 = num6 * num6 - 4f * num7;
					if (num8 <= 0f)
					{
						break;
					}
					float num9 = -num6 + Mathf.Sqrt(num8);
					if (num9 < 0f)
					{
						break;
					}
					spawnPosition -= vector3 * num9;
				}
			}
		}

		// Token: 0x06002992 RID: 10642 RVA: 0x000DE0A8 File Offset: 0x000DC2A8
		private bool TryReinforceFleet(TIFactionState faction, int numReinforcingShips, Vector3 reinforcementSpawnPosition, Vector3 reinforcementVelocityVector, out List<CombatShipController> addedReinforcements)
		{
			addedReinforcements = new List<CombatShipController>();
			Func<CombatFleetController, bool> <>9__0;
			for (int i = 0; i < this.fleetControllers.Count; i++)
			{
				CombatFleetController combatFleetController = this.fleetControllers[i];
				if (combatFleetController.reinforcements.Count != 0 && combatFleetController.fleetState.faction == faction)
				{
					if (numReinforcingShips > combatFleetController.reinforcements.Count<TISpaceShipState>())
					{
						numReinforcingShips = combatFleetController.reinforcements.Count<TISpaceShipState>();
					}
					List<TISpaceShipState> list = new List<TISpaceShipState>();
					for (int j = 0; j < numReinforcingShips; j++)
					{
						list.Add(combatFleetController.reinforcements[j]);
					}
					foreach (TISpaceShipState tispaceShipState in list)
					{
						tispaceShipState.SetCombatFormationOffset(list, combatFleetController.fleetState.formation, numReinforcingShips, false, false);
					}
					List<CombatShipController> list2 = new List<CombatShipController>();
					for (int k = 0; k < numReinforcingShips; k++)
					{
						Vector3 vector = default(Vector3);
						if (GameControl.spaceCombat.combatState.stances[this.fleetControllers[i].faction] != CombatStance.Evade)
						{
							vector = reinforcementSpawnPosition + SpaceCombatManager.km_to_scale_vec3((Vector3)combatFleetController.reinforcements[k].fleetFormationOffset);
						}
						else
						{
							vector = reinforcementSpawnPosition + SpaceCombatManager.km_to_scale_vec3((Vector3)combatFleetController.reinforcements[k].fleetFormationOffset);
						}
						CombatShipController combatShipController = this.CreateShip(vector, reinforcementVelocityVector, combatFleetController.reinforcements[k]);
						list2.Add(combatShipController);
						combatFleetController.activeShipControllers.Add(combatShipController);
						addedReinforcements.Add(combatShipController);
						if (combatFleetController.IsActivePlayerFleet && combatFleetController.IsUnderAIControl)
						{
							combatShipController.ShipState.SetAIControl(true);
						}
						if (!this.waypointsVisible)
						{
							combatShipController.ToggleWaypointVisualization();
						}
					}
					if (numReinforcingShips > 1)
					{
						GameControl.eventManager.TriggerEvent(new BattleGroupReinforcementArrived(numReinforcingShips, list2[0].ShipState), null, Array.Empty<object>());
					}
					else
					{
						GameControl.eventManager.TriggerEvent(new ReinforcementArrived(list2[0].ShipState), null, Array.Empty<object>());
					}
					CombatAIController combatAIController = this._combatAIController;
					IList<CombatShipController> list3 = list2;
					CombatFleetController combatFleetController2 = combatFleetController;
					IEnumerable<CombatFleetController> enumerable = this.fleetControllers;
					Func<CombatFleetController, bool> func;
					if ((func = <>9__0) == null)
					{
						func = (<>9__0 = (CombatFleetController x) => x.faction != faction);
					}
					combatAIController.DivideShipsIntoSquadronsAndConfigureAI(list3, combatFleetController2, enumerable.FirstOrDefault<CombatFleetController>(func), this.gameTime.currentTime);
					for (int l = 0; l < numReinforcingShips; l++)
					{
						if (combatFleetController.reinforcements.Count > 0)
						{
							combatFleetController.reinforcements.RemoveAt(0);
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002993 RID: 10643 RVA: 0x000DE36C File Offset: 0x000DC56C
		private CombatShipController CreateShip(Vector3 position, Vector3 velocity, TISpaceShipState shipState)
		{
			CombatShipController component = global::UnityEngine.Object.Instantiate<Transform>(this.shipPrefab, position, Quaternion.identity).GetComponent<CombatShipController>();
			component.Initialize(shipState, velocity);
			this.combatantLookup.Add(shipState, component);
			this.ships.Add(component);
			this.activeShips.Add(component);
			Debug.Log(TIUtilities.CombineStrings(new string[]
			{
				"Adding ship to CombatManager as ActiveShip(CreateShip): ",
				component.ShipState.ID.ToString(),
				", ",
				component.ShipState.displayName
			}));
			foreach (CombatShipController combatShipController in this.activeShips)
			{
				if (!combatShipController.destructionTriggered && !combatShipController.isDestroyed)
				{
					if (combatShipController.faction == this.combatState.primaryCombatFaction(component.faction) || component.faction == this.combatState.primaryCombatFaction(combatShipController.faction))
					{
						combatShipController.alliedCombatants.Add(component);
						if (component != combatShipController)
						{
							component.alliedCombatants.Add(combatShipController);
						}
					}
					else
					{
						combatShipController.enemyCombatants.Add(component);
						component.enemyCombatants.Add(combatShipController);
					}
				}
			}
			foreach (CombatHabModuleController combatHabModuleController in this.combatHabModuleControllers)
			{
				if (!combatHabModuleController.destructionTriggered && !combatHabModuleController.isDestroyed)
				{
					if (combatHabModuleController.faction == this.combatState.primaryCombatFaction(component.faction) || component.faction == this.combatState.primaryCombatFaction(combatHabModuleController.faction))
					{
						combatHabModuleController.alliedCombatants.Add(component);
						component.alliedCombatants.Add(combatHabModuleController);
					}
					else
					{
						combatHabModuleController.enemyCombatants.Add(component);
						component.enemyCombatants.Add(combatHabModuleController);
					}
				}
			}
			this.container.Add(component.name + shipState.ID.ToString(), component.transform.gameObject, false, false);
			shipState.EnterCombat();
			GameControl.eventManager.TriggerEvent(new ShipEntersCombat(this.combatState, shipState), shipState.ID.ToString(), new object[] { shipState });
			return component;
		}

		// Token: 0x06002994 RID: 10644 RVA: 0x000DE610 File Offset: 0x000DC810
		public void Precombat_SwapShipPositions(CombatShipController ship1, CombatShipController ship2)
		{
			Transform combatantTransform = ship2.combatantTransform;
			Transform combatantTransform2 = ship1.combatantTransform;
			Vector3 position = ship1.combatantTransform.position;
			Vector3 position2 = ship2.combatantTransform.position;
			combatantTransform.position = position;
			combatantTransform2.position = position2;
			ship1.ReinitializeWaypoints();
			ship2.ReinitializeWaypoints();
		}

		// Token: 0x06002995 RID: 10645 RVA: 0x000DE660 File Offset: 0x000DC860
		public void Precombat_SwapShipToReinforcements(CombatShipController activeShip, TISpaceShipState reinforcementShip)
		{
			CombatShipController newShip = this.CreateShip(activeShip.position, activeShip.velocityVector, reinforcementShip);
			this.combatantLookup.Remove(activeShip.ShipState);
			this._combatAIController.RemoveShipBehaviour(activeShip);
			CombatFleetController combatFleetController = this.fleetControllers.First<CombatFleetController>((CombatFleetController x) => x.faction == activeShip.faction);
			combatFleetController.activeShipControllers.Remove(activeShip);
			combatFleetController.reinforcements.Remove(reinforcementShip);
			int num = activeShip.ShipState.fleet.ships.IndexOf(activeShip.ShipState);
			Dictionary<TISpaceShipState, int> dictionary = activeShip.ShipState.fleet.ships.ToDictionary<TISpaceShipState, TISpaceShipState, int>((TISpaceShipState x) => x, (TISpaceShipState x) => activeShip.ShipState.fleet.ships.IndexOf(x));
			bool flag = false;
			foreach (TISpaceShipState tispaceShipState in combatFleetController.reinforcements)
			{
				if (num < dictionary[tispaceShipState])
				{
					combatFleetController.reinforcements.Insert(combatFleetController.reinforcements.IndexOf(tispaceShipState), activeShip.ShipState);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				combatFleetController.reinforcements.Add(activeShip.ShipState);
			}
			activeShip._waypointNavigationController.CleanUpWaypoints();
			foreach (CombatantController combatantController in this.combatantLookup.Values)
			{
				combatantController.alliedCombatants.Remove(activeShip);
				combatantController.enemyCombatants.Remove(activeShip);
			}
			this.activeShips.Remove(activeShip);
			this.ships.Remove(activeShip);
			activeShip.gameObject.SetActive(false);
			activeShip.ShipState.PostCombatVis();
			activeShip.ReturnToStrategyLayerFleet();
			Debug.Log(TIUtilities.CombineStrings(new string[]
			{
				"removing ship from CombatManager ActiveShip(SwapToReinforcements): ",
				activeShip.ShipState.ID.ToString(),
				", ",
				activeShip.ShipState.displayName
			}));
			combatFleetController.activeShipControllers.Add(newShip);
			this._combatAIController.AddShipBehaviour(newShip, combatFleetController, this.fleetControllers.FirstOrDefault<CombatFleetController>((CombatFleetController x) => x.faction != newShip.faction), this.gameTime.currentTime);
		}

		// Token: 0x06002996 RID: 10646 RVA: 0x000DE954 File Offset: 0x000DCB54
		public void PreRemoveShip(CombatShipController ship)
		{
			foreach (CombatantController combatantController in this.combatantLookup.Values)
			{
				combatantController.alliedCombatants.Remove(ship);
				combatantController.enemyCombatants.Remove(ship);
			}
			foreach (CombatFleetController combatFleetController in this.fleetControllers)
			{
				if (combatFleetController.faction == ship.faction)
				{
					this._reinforcementCount[ship.faction] = Mathf.Min(this._reinforcementCount[ship.faction] + 1, combatFleetController.reinforcements.Count);
				}
			}
		}

		// Token: 0x06002997 RID: 10647 RVA: 0x000DEA40 File Offset: 0x000DCC40
		public void PreDestroyHabModule(CombatHabModuleController habModule)
		{
			foreach (CombatantController combatantController in this.combatantLookup.Values)
			{
				combatantController.alliedCombatants.Remove(habModule);
				combatantController.enemyCombatants.Remove(habModule);
			}
		}

		// Token: 0x06002998 RID: 10648 RVA: 0x000DEAAC File Offset: 0x000DCCAC
		public void OnShipDestroyed(ShipDestroyed e)
		{
			this.RemoveShipFromControlGroups(e.ship);
			this.CheckEndCombatAfterDestruction(e.ship);
		}

		// Token: 0x06002999 RID: 10649 RVA: 0x000DEAC8 File Offset: 0x000DCCC8
		public void DestroyShip(CombatShipController ship, TIGameState killer, TIFactionState killerFaction, TIShipWeaponTemplate killerWeapon)
		{
			this.activeShips.Remove(ship);
			Debug.Log(TIUtilities.CombineStrings(new string[]
			{
				"removing ship from CombatManager ActiveShip(DestroyShip): ",
				ship.ShipState.ID.ToString(),
				", ",
				ship.ShipState.displayName
			}));
			this._combatAIController.RemoveShipBehaviour(ship);
			this.combatState.RecordShipDestroyed(ship.ShipState, killer, killerFaction, killerWeapon);
			this.combatCamera.OnShipDestroyed(ship);
			ship.gameObject.SetActive(false);
			ship.ShipState.DestroyShip(true, killerFaction);
			this.CheckEndCombatAfterDestruction(ship);
			if (!this.shipDestroyed)
			{
				AudioManager.SetIntensity(1f);
				this.shipDestroyed = true;
			}
		}

		// Token: 0x0600299A RID: 10650 RVA: 0x000DEB94 File Offset: 0x000DCD94
		public void RemoveShipFromCombat(CombatShipController ship)
		{
			GameControl.eventManager.TriggerEvent(new ShipDestroyed(ship.ShipState, null, null, null), null, Array.Empty<object>());
			ship.ModelController.DeactivateThrusters(false);
			ship.ShipState.DeactivateThrusters();
			this.PreRemoveShip(ship);
			ship.ShipDepartureCleanup();
			this.activeShips.Remove(ship);
			Debug.Log(TIUtilities.CombineStrings(new string[]
			{
				"removing ship from CombatManager ActiveShip(RemoveShipFromCombat - Disengaged/Retreated): ",
				ship.ShipState.ID.ToString(),
				", ",
				ship.ShipState.displayName
			}));
			this._combatAIController.RemoveShipBehaviour(ship);
			ship.gameObject.SetActive(false);
			this.CheckEndCombatAfterDestruction(ship);
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x000DEC59 File Offset: 0x000DCE59
		public void DestroyHabModule(TIFactionState destroyer, CombatHabModuleController module)
		{
			this.PreDestroyHabModule(module);
			module.DestroyHabModule(destroyer);
			this.CheckEndCombatAfterDestruction(module);
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x000DEC70 File Offset: 0x000DCE70
		private void FullEndCombatCheck()
		{
			List<TIFactionState> list = new List<TIFactionState>();
			foreach (CombatFleetController combatFleetController in this.fleetControllers)
			{
				if (combatFleetController.activeShipControllers.Count > 0 || combatFleetController.reinforcements.Count > 0)
				{
					list.AddUnique(combatFleetController.faction);
				}
			}
			if (this.combatHabModuleControllers.Count > 0)
			{
				if (this.combatHabModuleControllers.Any<CombatHabModuleController>((CombatHabModuleController x) => !x.isDestroyed))
				{
					list.AddUnique(this.combatHabModuleControllers.First<CombatHabModuleController>().faction);
				}
			}
			if (list.Count < 2)
			{
				this.TriggerCombatEnd();
				Debug.Log("FECC: Combat End Triggered at: " + this.combatDuration_s.ToString() + "\nCombat Will End at: " + this.endCombatTime.ToString());
			}
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x000DED78 File Offset: 0x000DCF78
		private void CheckEndCombatAfterDestruction(CombatantController controller)
		{
			List<CombatantController> list = new List<CombatantController>(controller.alliedCombatants);
			list.Remove(controller);
			if (list.Count > 0)
			{
				return;
			}
			foreach (CombatFleetController combatFleetController in this.fleetControllers)
			{
				if (combatFleetController.activeShipControllers.Count <= 0 && combatFleetController.reinforcements.Count <= 0)
				{
					this.TriggerCombatEnd();
					Debug.Log("FLTS: Combat End Triggered at: " + this.combatDuration_s.ToString() + "\nCombat Will End at: " + this.endCombatTime.ToString());
					return;
				}
			}
			if (controller.ref_habModuleController != null)
			{
				this.TriggerCombatEnd();
				Debug.Log("HM: Combat End Triggered at: " + this.combatDuration_s.ToString() + "\nCombat Will End at: " + this.endCombatTime.ToString());
				return;
			}
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x000DEE74 File Offset: 0x000DD074
		private void CheckEndCombatAfterDestruction(TISpaceShipState state)
		{
			if (TIGlobalValuesState.isSpaceCombatEnabled && TIGameState.Valid(state))
			{
				CombatantController combatantController = null;
				foreach (CombatantController combatantController2 in this.activeShips)
				{
					if (combatantController2 != null && combatantController2.WeaponCarrierState.ref_shipCarrier() == state)
					{
						combatantController = combatantController2;
					}
				}
				if (combatantController == null)
				{
					return;
				}
				this.CheckEndCombatAfterDestruction(combatantController);
			}
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x000DEF00 File Offset: 0x000DD100
		public void InvokeEndCombat()
		{
			this.endCombatTime = 0.0;
			this.EndCombat(false);
		}

		// Token: 0x060029A0 RID: 10656 RVA: 0x000DEF18 File Offset: 0x000DD118
		public void EndCombatWithAutoresolve()
		{
			foreach (CombatShipController combatShipController in this.ships)
			{
				combatShipController.FinishUpImmediately();
			}
			this.combatState.mayRejectAutoresolve = false;
			this.endCombatTime = 0.0;
			this.EndCombat(true);
		}

		// Token: 0x170005BF RID: 1471
		// (get) Token: 0x060029A1 RID: 10657 RVA: 0x000DEF8C File Offset: 0x000DD18C
		// (set) Token: 0x060029A2 RID: 10658 RVA: 0x000DEF94 File Offset: 0x000DD194
		public bool combatEndTriggered { get; private set; }

		// Token: 0x170005C0 RID: 1472
		// (get) Token: 0x060029A3 RID: 10659 RVA: 0x000DEF9D File Offset: 0x000DD19D
		// (set) Token: 0x060029A4 RID: 10660 RVA: 0x000DEFA5 File Offset: 0x000DD1A5
		public bool combatEnded { get; private set; }

		// Token: 0x060029A5 RID: 10661 RVA: 0x000DEFB0 File Offset: 0x000DD1B0
		public void EndCombat(bool autoresolve = false)
		{
			if (!this.combatEnded)
			{
				this.RemoveCombatCameraEffect();
				TIInputManager.CancelBoxSelect();
				if (!autoresolve)
				{
					this.combatState.RecordSurvivors();
				}
				this.combatHUD.PostCombatCleanup();
				this.combatHUD.Hide();
				this.combatHUD.Canvas.gameObject.SetActive(false);
				if (!GameControl.control.skirmishMode)
				{
					foreach (CombatFleetController combatFleetController in this.fleetControllers)
					{
						foreach (CombatShipController combatShipController in combatFleetController.disengagedShips)
						{
							this.activeShips.Add(combatShipController);
							Debug.Log(TIUtilities.CombineStrings(new string[]
							{
								"Adding ship to CombatManager as ActiveShip(Retreated Ship): ",
								combatShipController.ShipState.ID.ToString(),
								", ",
								combatShipController.ShipState.displayName
							}));
						}
						if (combatFleetController.IsFleetDestroyed)
						{
							global::UnityEngine.Object.Destroy(combatFleetController.strategyFleetObject);
						}
					}
					foreach (CombatShipController combatShipController2 in this.activeShips)
					{
						foreach (IWeapon weapon in combatShipController2.hull.IterateByClass<IWeapon>())
						{
							Weapon weapon2 = weapon as Weapon;
							weapon2.weaponVisualization.CeaseBeamFire();
							if (weapon2.altWeaponVisualization != null)
							{
								weapon2.weaponVisualization.CeaseBeamFire();
							}
						}
						if (!combatShipController2.destructionTriggered)
						{
							combatShipController2._waypointNavigationController.CleanUpWaypoints();
							combatShipController2.ShipState.PostCombatVis();
							combatShipController2.ReturnToStrategyLayerFleet();
							GameControl.eventManager.TriggerEvent(new ShipLeavesCombat(this.combatState, combatShipController2.ShipState), combatShipController2.ShipState.ID.ToString(), new object[] { combatShipController2.ShipState });
						}
						else
						{
							string[] array = new string[4];
							array[0] = "cant process postcombat on a ship that has been destroyed: ";
							array[1] = combatShipController2.ShipState.ID.ToString();
							array[2] = ",";
							int num = 3;
							TISpaceShipState shipState = combatShipController2.ShipState;
							array[num] = ((shipState != null) ? shipState.displayName : null);
							Debug.LogError(TIUtilities.CombineStrings(array));
						}
					}
					if (this.combatState.hab != null)
					{
						foreach (CombatHabModuleController combatHabModuleController in this.combatHabModuleControllers)
						{
							foreach (ShipWeaponVisController shipWeaponVisController in combatHabModuleController.dorsalWeaponControllers)
							{
								shipWeaponVisController.CeaseBeamFire();
							}
							foreach (ShipWeaponVisController shipWeaponVisController2 in combatHabModuleController.ventralWeaponControllers)
							{
								shipWeaponVisController2.CeaseBeamFire();
							}
						}
						this.ReturnHabToStrategyLayer();
					}
					List<string> list = new List<string>();
					foreach (GameObject gameObject in this.container)
					{
						if (gameObject != null)
						{
							list.Add(gameObject.name);
						}
					}
					Debug.Log("PostCombat Container Destruction: " + JsonConvert.SerializeObject(list));
					foreach (GameObject gameObject2 in this.container)
					{
						global::UnityEngine.Object.Destroy(gameObject2);
					}
					this.container.Clear(true);
					this.gameTime.Pause();
					double combatDuration_s = this.combatDuration_s;
					this.gameTime.SetTime(this.combatState.combatStartDateTime);
					this.gameTime.UpdateCurrentSpeedState(SpeedSettingState.Strategy);
					this.combatCamera.enabled = false;
					if (!autoresolve)
					{
						this.combatState.EndCombatForStrategyGame(combatDuration_s);
						this.combatState.active = false;
					}
					this._projectileJobContainer.ClearAllJobs();
					global::UnityEngine.Object.Destroy(this._projectileContainer);
					this._projectiles.Clear();
					this._reverseProjectiles.Clear();
					ViewControl.SetEnableAllStrategyShipModels(true);
					GameControl.control.viewMgr.GotoView(ViewType.SolarSystem);
					GameControl.canvasStack.RestoreStrategyLayerUIs();
					MusicController.Instance.ChangeMusicScene();
					if (this.combatState.winner == GameControl.control.activePlayer)
					{
						MusicController.Instance.PlayFanfare(this.combatState.factions[1].IsAlienFaction ? "event:/Music/Fanfares/trig_Combat_Aliens_Win" : "event:/Music/Fanfares/trig_Combat_Humans_Win");
					}
					else
					{
						MusicController.Instance.PlayFanfare("event:/Music/Fanfares/trig_Player_Defeat");
					}
					BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.effectsVolumeModifier());
					this.mainCameraTransform.SetPositionAndRotation(this.storedStratCameraPosition, this.storedStratCameraRotation);
					this.mainCamera.farClipPlane = this.originalFarClipPlane;
				}
				else
				{
					this.combatState.SetWinnerAndLoser();
					if (this.combatState.winner == GameControl.control.activePlayer)
					{
						MusicController.Instance.PlayFanfare(this.combatState.factions[1].IsAlienFaction ? "event:/Music/Fanfares/trig_Combat_Aliens_Win" : "event:/Music/Fanfares/trig_Combat_Humans_Win");
					}
					else
					{
						MusicController.Instance.PlayFanfare("event:/Music/Fanfares/trig_Player_Defeat");
					}
					foreach (CombatShipController combatShipController3 in this.activeShips)
					{
						if (!combatShipController3.destructionTriggered)
						{
							combatShipController3.ShipState.ClearRadiatorAudio();
						}
					}
					GameControl.eventManager.TriggerEvent(new CombatEnds(this.combatState), null, Array.Empty<object>());
					List<string> list2 = new List<string>();
					foreach (GameObject gameObject3 in this.container)
					{
						if (gameObject3 != null)
						{
							list2.Add(gameObject3.name);
						}
					}
					Debug.Log("PostCombat Container Destruction: " + JsonConvert.SerializeObject(list2));
					foreach (GameObject gameObject4 in this.container)
					{
						global::UnityEngine.Object.Destroy(gameObject4);
					}
					this.container.Clear(true);
					this.gameTime.Pause();
					this._projectileJobContainer.ClearAllJobs();
					global::UnityEngine.Object.Destroy(this._projectileContainer);
					this._projectiles.Clear();
					this._reverseProjectiles.Clear();
				}
				foreach (CombatFleetController combatFleetController2 in this.fleetControllers)
				{
					combatFleetController2.EndCombatCleanUp();
				}
				if (autoresolve)
				{
					this.combatState.autoresolve = true;
					this.combatState.Autoresolve();
				}
				this.combatEnded = true;
			}
		}

		// Token: 0x060029A6 RID: 10662 RVA: 0x000DF810 File Offset: 0x000DDA10
		private void CombatInit(SpaceCombatInitiated e)
		{
			if (!GameControl.control.skirmishMode)
			{
				if (Error.IsNull<TISpaceCombatState>(this.combatState) || Error.IsInvalid(this.combatState))
				{
					return;
				}
				GameControl.eventManager.AddListener<CombatStanceSelected>(new EventManager.EventDelegate<CombatStanceSelected>(this.StanceSubmitted), null, null, false, false);
				GameControl.eventManager.AddListener<PrecombatComplete>(new EventManager.EventDelegate<PrecombatComplete>(this.OnPrecombatComplete), null, null, true, false);
				this.promptQueue.AddPrompt(this.combatState.factions[0], null, this.combatState, "PromptSelectSpaceCombatStance", 0);
				this.promptQueue.AddPrompt(this.combatState.factions[1], null, this.combatState, "PromptSelectSpaceCombatStance", 0);
			}
		}

		// Token: 0x060029A7 RID: 10663 RVA: 0x000DF8C8 File Offset: 0x000DDAC8
		private void StanceSubmitted(CombatStanceSelected e)
		{
			TISpaceCombatState combatState = this.combatState;
			if (combatState != null && combatState.HaveStancesBeenSelected)
			{
				GameControl.eventManager.RemoveListener<CombatStanceSelected>(new EventManager.EventDelegate<CombatStanceSelected>(this.StanceSubmitted), null);
				if (this.combatState.requiresBidding)
				{
					this.promptQueue.AddPrompt(this.combatState.factions[0], null, this.combatState, "PromptSelectSpaceCombatBid", 0);
					this.promptQueue.AddPrompt(this.combatState.factions[1], null, this.combatState, "PromptSelectSpaceCombatBid", 0);
				}
			}
		}

		// Token: 0x060029A8 RID: 10664 RVA: 0x000DF958 File Offset: 0x000DDB58
		private void OnPrecombatComplete(PrecombatComplete e)
		{
			while (!this.combatState.HaveStancesBeenSelected && (!this.combatState.requiresBidding || !this.combatState.HaveBidsBeenSubmitted))
			{
				this.promptQueue.HandlePrompts();
			}
			this.combatState.HandlePrecombat();
			if (this.combatState.combatOccurs)
			{
				if (this.combatState.hab != null && this.combatState.fleets.FirstOrDefault<TISpaceFleetState>((TISpaceFleetState x) => !this.combatState.hab.faction.permanentAlly(x.faction)) != null)
				{
					this.combatState.hab.AddConflictFleet(this.combatState.attacker);
				}
				if (this.combatState.autoDestroyHab)
				{
					this.combatState.RecordSurvivors();
					this.combatState.EndCombatForStrategyGame(0.0);
				}
				else if (this.combatState.autoresolve)
				{
					this.combatState.Autoresolve();
				}
				else
				{
					this.StartCombat();
				}
			}
			else
			{
				this.combatState.RecordSurvivors();
				this.combatState.EndCombatForStrategyGame(0.0);
			}
			GameControl.eventManager.RemoveListener<PrecombatComplete>(new EventManager.EventDelegate<PrecombatComplete>(this.OnPrecombatComplete), null);
		}

		// Token: 0x060029A9 RID: 10665 RVA: 0x000DFA8C File Offset: 0x000DDC8C
		public void AutoresolveRejected()
		{
			this.StartCombat();
		}

		// Token: 0x060029AA RID: 10666 RVA: 0x000DFA94 File Offset: 0x000DDC94
		private IEnumerator WaitForCombatReady()
		{
			yield return this.briefWait;
			yield break;
		}

		// Token: 0x060029AB RID: 10667 RVA: 0x000DFAA4 File Offset: 0x000DDCA4
		private void TriggerCombatEnd()
		{
			this.endCombatTime = this.combatDuration_s + this.endCombatDuration_s;
			this.forceEndCombatTime = this.combatDuration_s + this.forceEndCombatDuration_s;
			this.combatEndTriggered = true;
			GameControl.eventManager.TriggerEvent(new CombatEndTriggered(), null, Array.Empty<object>());
		}

		// Token: 0x060029AC RID: 10668 RVA: 0x000DFAF4 File Offset: 0x000DDCF4
		private void OnEndCombatStanceChanged(EndCombatStanceChanged e)
		{
			using (Dictionary<TIFactionState, bool>.ValueCollection.Enumerator enumerator = this.combatState.votedEndCombat.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current)
					{
						return;
					}
				}
			}
			this.TriggerCombatEnd();
			Debug.Log("Both Sides Voted To End Combat at: " + this.combatDuration_s.ToString());
		}

		// Token: 0x060029AD RID: 10669 RVA: 0x000DFB70 File Offset: 0x000DDD70
		private void OnGameTimeSpeedChanged(GameTimeSpeedChanged e)
		{
			if (!TIGlobalValuesState.isSpaceCombatEnabled)
			{
				return;
			}
			switch (GameTimeManager.Singleton.currentSpeedIndex)
			{
			case 1:
				BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.effectsVolumeModifier());
				return;
			case 2:
				BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.effectsVolumeModifier() * 0.92f);
				return;
			case 3:
				BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.effectsVolumeModifier() * 0.84f);
				return;
			case 4:
				BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.effectsVolumeModifier() * 0.76f);
				return;
			case 5:
				BusManager.SetVolume(BusManager.SFX, TIPlayerProfileManager.effectsVolumeModifier() * 0.68f);
				return;
			default:
				return;
			}
		}

		// Token: 0x060029AE RID: 10670 RVA: 0x000DFC14 File Offset: 0x000DDE14
		private void StartCombat()
		{
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.combatState.active = true;
			this.initialized = false;
			GameControl.control.viewMgr.GotoView(ViewType.SpaceCombat);
		}

		// Token: 0x060029AF RID: 10671 RVA: 0x000DFC49 File Offset: 0x000DDE49
		private void OnDestroy()
		{
			this.RemoveEventListener();
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x000DFC51 File Offset: 0x000DDE51
		public void ResetCombatManager()
		{
			this.combatState = null;
			this.initialized = false;
		}

		// Token: 0x060029B1 RID: 10673 RVA: 0x000DFC61 File Offset: 0x000DDE61
		public void SetupEventListener()
		{
			GameControl.eventManager.AddListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.CombatInit), null, null, false, false);
			GameControl.eventManager.AddListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null, null, true, false);
		}

		// Token: 0x060029B2 RID: 10674 RVA: 0x000DFC97 File Offset: 0x000DDE97
		private void RemoveEventListener()
		{
			GameControl.eventManager.RemoveListener<SpaceCombatInitiated>(new EventManager.EventDelegate<SpaceCombatInitiated>(this.CombatInit), null);
			GameControl.eventManager.RemoveListener<GameTimeSpeedChanged>(new EventManager.EventDelegate<GameTimeSpeedChanged>(this.OnGameTimeSpeedChanged), null);
		}

		// Token: 0x060029B3 RID: 10675 RVA: 0x000DFCC8 File Offset: 0x000DDEC8
		private void ShowHab(TIHabState hab, List<CombatFleetController> fleetControllers)
		{
			this.strategyHabObject = hab.gameObjectLink;
			this.habModelController = this.strategyHabObject.GetComponentInChildren<HabModelController>(true);
			this.habModelObject = this.habModelController.gameObject;
			this.strategyHabModelLocalPosition = this.habModelObject.transform.localPosition;
			this.strategyHabModelLocalRotation = this.habModelObject.transform.localRotation;
			this.strategyHabModelLocalScale = this.habModelObject.transform.localScale;
			float num = 0f;
			float num2 = 1f;
			if (this.combatState.assets[this.combatState.factions[0]].Contains(hab))
			{
				if (this.combatState.assets[this.combatState.factions[0]].Any<TISpaceAssetState>(delegate(TISpaceAssetState x)
				{
					if (x.isSpaceFleetState)
					{
						List<TISpaceShipState> list = x.ref_fleet.ships;
						return list != null && list.Count > 0;
					}
					return false;
				}))
				{
					num = fleetControllers[0].activeShipControllers.Min<CombatShipController>((CombatShipController x) => x.position.z) - SpaceCombatManager.km_to_scale(150f) * this.modelScalingFactor;
				}
				num2 = -1f;
			}
			else if (this.combatState.assets[this.combatState.factions[1]].Any<TISpaceAssetState>(delegate(TISpaceAssetState x)
			{
				if (x.isSpaceFleetState)
				{
					List<TISpaceShipState> list2 = x.ref_fleet.ships;
					return list2 != null && list2.Count > 0;
				}
				return false;
			}))
			{
				num = fleetControllers[1].activeShipControllers.Max<CombatShipController>((CombatShipController x) => x.position.z) + SpaceCombatManager.km_to_scale(150f) * this.modelScalingFactor;
			}
			else
			{
				num = this.scaledSecondFleetOffset.z;
			}
			Vector2 vector = global::UnityEngine.Random.insideUnitCircle.normalized * TIUtilities.RandomRange(1f, 2f);
			this.habModelObject.transform.SetPositionAndRotation(new Vector3(vector.x, vector.y, num), Quaternion.Euler(num2 * (float)(-25 - TIUtilities.RandomRange(0, 10)), (float)(5 - TIUtilities.RandomRange(0, 10)), (float)(25 - TIUtilities.RandomRange(0, 50))));
			this.habModelObject.transform.SetParent(this.container.transform);
			this.habModelObject.transform.localScale = Vector3.one * this.modelScalingFactor;
			this.ConfigureHabCollisionObjectsForCombat();
			this.habModelObject.SetActive(true);
		}

		// Token: 0x060029B4 RID: 10676 RVA: 0x000DFF5C File Offset: 0x000DE15C
		private void ConfigureHabCollisionObjectsForCombat()
		{
			if (this.habModelController == null)
			{
				return;
			}
			foreach (HabModuleController habModuleController in this.habModelController.GetModuleControllers())
			{
				foreach (Collider collider in habModuleController.gameObject.GetComponentsInChildren<Collider>())
				{
					if (!(collider.gameObject.name != "CollisionObject"))
					{
						Collider collider2 = collider;
						if (!this.initialized)
						{
							collider2.enabled = false;
						}
						else if (habModuleController.habModule.empty)
						{
							collider2.enabled = false;
						}
						else
						{
							collider2.enabled = true;
						}
					}
				}
			}
		}

		// Token: 0x060029B5 RID: 10677 RVA: 0x000E0028 File Offset: 0x000DE228
		private void ReturnHabToStrategyLayer()
		{
			this.habModelObject.transform.SetParent(this.strategyHabObject.transform);
			this.habModelObject.transform.SetSiblingIndex(1);
			this.habModelObject.transform.localPosition = this.strategyHabModelLocalPosition;
			this.habModelObject.transform.localRotation = this.strategyHabModelLocalRotation;
			this.habModelObject.transform.localScale = this.strategyHabModelLocalScale;
		}

		// Token: 0x060029B6 RID: 10678 RVA: 0x000E00A4 File Offset: 0x000DE2A4
		private GameObject ShowSpaceBody(TISpaceBodyState spaceBody, float realDistance, float maxDistance, out float maxInnerDistance, out float radius)
		{
			GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(GameControl.assetLoader.LoadAsset<GameObject>(spaceBody.modelResource));
			GameObject gameObject2 = null;
			float num2;
			if (spaceBody.isaMoon)
			{
				float num = (float)TISpaceObjectState.ExactDistanceBetweenTwoSpaceObjects_m(spaceBody.barycenter, spaceBody) / 1000f;
				float num3;
				gameObject2 = this.ShowSpaceBody(spaceBody.barycenter as TISpaceBodyState, num, maxDistance, out num2, out num3);
			}
			else if (spaceBody.objectType == SpaceObjectType.Comet)
			{
				num2 = maxDistance / 2f;
			}
			else
			{
				num2 = maxDistance;
			}
			float num4 = 2f * Mathf.Asin((float)spaceBody.meanRadius_km / realDistance);
			float num5 = Mathf.Sin(num4 / 2f);
			radius = 0.7f * num2 * num5;
			float num6 = SpaceCombatManager.km_to_scale(radius) / spaceBody.modelScale;
			float num7 = TIUtilities.RandomFloatValue() * 360f;
			if (gameObject2 != null)
			{
				float num8 = Vector3.Angle(new Vector3(1f, 0f, 0f), gameObject2.transform.localPosition);
				if (gameObject2.transform.localPosition.z < 0f)
				{
					num8 = 360f - num8;
				}
				float num9 = num4 / 0.017453292f;
				num7 = num8 + num9 / 2f + TIUtilities.RandomFloatValue() * (360f - num9);
			}
			Vector3 vector = Quaternion.Euler(0f, -num7, 0f) * new Vector3(1f, 0f, 0f) * SpaceCombatManager.km_to_scale(num2);
			gameObject.transform.parent = this.container.transform;
			gameObject.transform.SetLayer(15, true);
			gameObject.transform.localScale = num6 * Vector3.one;
			gameObject.transform.localPosition = vector;
			gameObject.SetActive(true);
			this.combatSpaceBodies.Add(gameObject);
			this.container.Add(spaceBody.displayName, gameObject, false, false);
			if (GameControl.control.skirmishMode && !TemplateManager.global.debug_suppressSkirmishRotatePlanet)
			{
				gameObject.transform.Rotate(new Vector3(0f, (float)TIUtilities.RandomRange(0, 360), 0f));
			}
			Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			float num10 = 0.6f;
			maxInnerDistance = (num2 - radius) * num10;
			if (spaceBody.objectType == SpaceObjectType.Comet)
			{
				CometController cometController = global::UnityEngine.Object.Instantiate<CometController>(spaceBody.controller.GetComponentInChildren<CometController>(true));
				cometController.transform.SetParent(gameObject.transform, false);
				cometController.transform.localPosition = Vector3.zero;
				Transform[] componentsInChildren2 = gameObject.GetComponentsInChildren<Transform>(true);
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					componentsInChildren2[i].gameObject.layer = 15;
				}
				cometController.InitiateOverrideRenderMode(spaceBody, this.backgroundCamera, false);
			}
			return gameObject;
		}

		// Token: 0x060029B7 RID: 10679 RVA: 0x000E0374 File Offset: 0x000DE574
		private void OnEnable()
		{
			TIUtilities.InitRandom(DateTime.Now.Millisecond + global::UnityEngine.Random.state.GetHashCode());
			this.storedStratCameraPosition = new Vector3(this.mainCameraTransform.position.x, this.mainCameraTransform.position.y, this.mainCameraTransform.position.z);
			this.storedStratCameraRotation = new Quaternion(this.mainCameraTransform.rotation.x, this.mainCameraTransform.rotation.y, this.mainCameraTransform.rotation.z, this.mainCameraTransform.rotation.w);
			this.combatCamera = this.mainCamera.GetComponent<SpaceCombatCameraController>();
			this.combatCamera.enabled = true;
			this.gameTime = World.Active.GetExistingManager<GameTimeManager>();
			this.gameTime.UpdateCurrentSpeedState(SpeedSettingState.SpaceCombat);
			GameControl.canvasStack.HideAll();
			this.combatHUD = GameControl.canvasStack.CombatHud as SpaceCombatCanvasController;
			this.combatHUD.Canvas.gameObject.SetActive(true);
			this.combatHUD.Show();
			this.combatCamera.OnHudEnabled();
			this.container.gameObject.SetActive(true);
			Transform transform = global::UnityEngine.Object.Instantiate<Transform>(this.combatGridPrefab, Vector3.zero, Quaternion.identity);
			transform.transform.parent = this.container.transform;
			this.container.Add("Grid", transform.gameObject, false, false);
			transform.SetLayer(LayerMask.NameToLayer("Space Combat UI"), true);
			transform.localScale = 25f * this.modelScalingFactor * Vector3.one;
			this.combatGrid = transform.GetComponent<CombatGrid>();
			GameObject modelLink = GameStateManager.Sol().controller.modelLink;
			modelLink.SetActive(true);
			modelLink.transform.GetChild(0).gameObject.SetActive(false);
			MeshRenderer[] componentsInChildren = GameStateManager.Sol().controller.modelLink.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			TIOrbitState tiorbitState = null;
			TISpaceFleetState tispaceFleetState = this.combatState.fleets[0];
			if (tispaceFleetState != null && !tispaceFleetState.inTransfer)
			{
				TIOrbitState orbitState = this.combatState.fleets[0].orbitState;
				if (((orbitState != null) ? orbitState.ref_spaceBody : null) != null)
				{
					tiorbitState = this.combatState.fleets[0].orbitState;
					goto IL_0307;
				}
			}
			TISpaceFleetState tispaceFleetState2 = this.combatState.fleets[1];
			if (tispaceFleetState2 != null && !tispaceFleetState2.inTransfer)
			{
				TIOrbitState orbitState2 = this.combatState.fleets[1].orbitState;
				if (((orbitState2 != null) ? orbitState2.ref_spaceBody : null) != null)
				{
					tiorbitState = this.combatState.fleets[1].orbitState;
					goto IL_0307;
				}
			}
			TIHabState hab = this.combatState.hab;
			if (((hab != null) ? hab.ref_spaceBody : null) != null)
			{
				tiorbitState = this.combatState.hab.orbitState;
			}
			IL_0307:
			bool flag = tiorbitState != null && tiorbitState.ref_naturalSpaceObject.isEarth;
			if (tiorbitState != null)
			{
				float num = 0.95f * this.mainCamera.farClipPlane - transform.localScale.x * 2f * Mathf.Sqrt(2f);
				num = SpaceCombatManager.scale_to_km(num);
				float num2 = (float)tiorbitState.semiMajorAxis_km;
				if (flag)
				{
					num2 = Mathf.Max(num2, (float)tiorbitState.ref_spaceBody.meanRadius_km + 1000f);
				}
				float num3;
				float num4;
				this.primarySpaceBody = this.ShowSpaceBody(tiorbitState.barycenter.ref_spaceBody, num2, num, out num3, out num4);
				this.minimumDistanceToPrimarySaceBody = SpaceCombatManager.km_to_scale(num4) * 1.1f;
			}
			GameObject gameObject = modelLink.transform.GetChild(1).gameObject;
			gameObject.SetActive(true);
			Quaternion quaternion;
			if (this.primarySpaceBody != null)
			{
				float num5;
				if (flag)
				{
					num5 = 10f;
				}
				else
				{
					num5 = 60f;
				}
				quaternion = Quaternion.LookRotation(this.primarySpaceBody.transform.position) * Quaternion.Euler(0f, num5, 0f);
			}
			else
			{
				quaternion = Quaternion.Euler(0f, 50f, 0f);
			}
			gameObject.transform.rotation = quaternion;
			this.dateTimeofLastQuarterSecondUpdateLoop = default(DateTime);
			this.dateTimeofLastQuarterSecondUpdateLoop = this.gameTime.currentTime.ExportTime();
			this.timeOfLastUpdate_s = 0.0;
			this.timeOfLastSecondUpdate_s = 0.0;
			this.timeOfLastQuarterSecondUpdate_s = 0.0;
			GameControl.eventManager.AddListener<EndCombatStanceChanged>(new EventManager.EventDelegate<EndCombatStanceChanged>(this.OnEndCombatStanceChanged), null, null, true, false);
			GameControl.eventManager.AddListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipDestroyed), null, null, false, false);
			this.stratLayerHabActiveStatus.Clear();
			foreach (TIHabState tihabState in GameStateManager.IterateByClass<TIHabState>(false))
			{
				if (tihabState.IsStation && this.combatState.hab != tihabState)
				{
					this.stratLayerHabActiveStatus.Add(tihabState, tihabState.controller.gameObject.activeSelf);
					tihabState.controller.gameObject.SetActive(false);
				}
			}
			this.combatStartDateTime = this.combatState.combatStartDateTime ?? TITimeState.Now();
		}

		// Token: 0x060029B8 RID: 10680 RVA: 0x000E08FC File Offset: 0x000DEAFC
		private void OnDisable()
		{
			if (this.combatCamera != null)
			{
				this.combatCamera.enabled = false;
			}
			if (this.container.gameObject != null)
			{
				this.container.gameObject.SetActive(false);
			}
			this.container.Clear(true);
			if (this._projectileContainer.gameObject != null)
			{
				foreach (ProjectileController projectileController in this._projectileContainer.GetComponentsInChildren<ProjectileController>(true))
				{
					projectileController.gameObject.SetActive(false);
					global::UnityEngine.Object.Destroy(projectileController.gameObject);
				}
			}
			GameControl.eventManager.RemoveListener<EndCombatStanceChanged>(new EventManager.EventDelegate<EndCombatStanceChanged>(this.OnEndCombatStanceChanged), null);
			GameControl.eventManager.RemoveListener<ShipDestroyed>(new EventManager.EventDelegate<ShipDestroyed>(this.OnShipDestroyed), null);
			GameObject modelLink = GameStateManager.Sol().controller.modelLink;
			if (modelLink != null)
			{
				modelLink.transform.GetChild(0).gameObject.SetActive(true);
				modelLink.transform.GetChild(1).gameObject.SetActive(false);
				MeshRenderer[] componentsInChildren2 = modelLink.GetComponentsInChildren<MeshRenderer>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].enabled = true;
				}
			}
			foreach (TIHabState tihabState in this.stratLayerHabActiveStatus.Keys)
			{
				if (tihabState.IsStation && this.combatState.hab != tihabState && tihabState.controller != null)
				{
					tihabState.controller.gameObject.SetActive(this.stratLayerHabActiveStatus[tihabState]);
				}
			}
			this.gameTime.ClearCombatTimeEvents();
			AudioManager.SetIntensity(this.priorIntensity);
		}

		// Token: 0x060029B9 RID: 10681 RVA: 0x000E0ADC File Offset: 0x000DECDC
		public void CombatQuarterSecond(DateTime currentTime, int kounter, bool updateUI)
		{
			if (kounter == 0 || kounter == 1)
			{
				using (List<CombatShipController>.Enumerator enumerator = this.activeShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CombatShipController combatShipController = enumerator.Current;
						if (!combatShipController.destructionTriggered)
						{
							combatShipController.ShipState.CombatPerQuarterSecondChanges(updateUI);
							int num = 0;
							foreach (IWeapon weapon in combatShipController.hull.IterateByClass<IWeapon>())
							{
								if (num % 2 == kounter && weapon.AcquireTarget(currentTime))
								{
									Weapon weapon2 = weapon as Weapon;
									weapon2.SelectWeaponVisualization(weapon2.targetedPosition).RotateToTarget(false);
									if (weapon.TryFire(currentTime))
									{
										if (!this.shotFired)
										{
											AudioManager.SetIntensity(0.45f);
											this.shotFired = true;
										}
										this.timeOfLastShotFired = TITimeState.Now();
									}
								}
								num++;
							}
						}
					}
					goto IL_011B;
				}
			}
			foreach (CombatShipController combatShipController2 in this.activeShips)
			{
				combatShipController2.ShipState.CombatPerQuarterSecondChanges(updateUI);
			}
			IL_011B:
			foreach (CombatHabModuleController combatHabModuleController in this.combatHabModuleControllers)
			{
				if (!combatHabModuleController.destructionTriggered)
				{
					int num2 = 0;
					foreach (IWeapon weapon3 in combatHabModuleController.weapons)
					{
						if (num2 == kounter && weapon3.AcquireTarget(currentTime))
						{
							Weapon weapon4 = weapon3 as Weapon;
							weapon4.SelectWeaponVisualization(weapon4.targetedPosition).RotateToTarget(false);
							if (weapon3.TryFire(currentTime))
							{
								this.timeOfLastShotFired = TITimeState.Now();
							}
						}
						num2++;
					}
				}
			}
		}

		// Token: 0x170005C1 RID: 1473
		// (get) Token: 0x060029BA RID: 10682 RVA: 0x000E0CFC File Offset: 0x000DEEFC
		private float scaledDepartureRange
		{
			get
			{
				return SpaceCombatManager.km_to_scale(2000f);
			}
		}

		// Token: 0x060029BB RID: 10683 RVA: 0x000E0D08 File Offset: 0x000DEF08
		public void CombatSecond(bool updateUI)
		{
			this.shipsToDisengage.Clear();
			foreach (CombatShipController combatShipController in this.activeShips)
			{
				combatShipController.SetAccelerationVector();
				if (!combatShipController.destructionTriggered)
				{
					if (combatShipController.ShipState.ShipDestroyed())
					{
						combatShipController.TriggerShipDestruction(null, null);
					}
					else
					{
						combatShipController.ShipState.CombatPerSecondChanges(updateUI);
						if (combatShipController.ShipState.disengageFromCombat && this.combatDuration_s > 1800.0)
						{
							List<CombatantController> enemyCombatants = combatShipController.enemyCombatants;
							bool flag = true;
							foreach (CombatantController combatantController in enemyCombatants)
							{
								if (Vector3.Distance(combatShipController.position, combatantController.position) < this.scaledDepartureRange && (combatantController.GetCombatantType() != IDamageableType.Ship || !combatantController.GetCombatantState().GetTargetableState().ref_ship.disengageFromCombat))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								this.shipsToDisengage.Add(combatShipController);
							}
						}
					}
				}
			}
			using (List<CombatShipController>.Enumerator enumerator = this.shipsToDisengage.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CombatShipController ship = enumerator.Current;
					this.fleetControllers.First<CombatFleetController>((CombatFleetController x) => x.fleetState == ship.ShipState.fleet).disengagedShips.Add(ship);
					ship.departed = true;
					ship.ShipState.CompleteDisengage();
					this.combatState.RecordShipDisengaged(ship.ShipState);
					this.RemoveShipFromCombat(ship);
				}
			}
		}

		// Token: 0x060029BC RID: 10684 RVA: 0x000E0F08 File Offset: 0x000DF108
		public void CombatFractionalSecond(double timeElapsed_s)
		{
			foreach (CombatShipController combatShipController in this.activeShips)
			{
				combatShipController.ShipState.CombatFractionalSecondChanges(timeElapsed_s);
			}
		}

		// Token: 0x170005C2 RID: 1474
		// (get) Token: 0x060029BD RID: 10685 RVA: 0x000E0F60 File Offset: 0x000DF160
		public double combatDuration_s
		{
			get
			{
				return this.gameTime.currentTime.DifferenceInSeconds(this.combatStartDateTime);
			}
		}

		// Token: 0x060029BE RID: 10686 RVA: 0x000E0F78 File Offset: 0x000DF178
		private void Update()
		{
			if (TIGlobalValuesState.isSpaceCombatEnabled && !this.combatEnded && !this._inFormationSelectionMode)
			{
				double combatDuration_s = this.combatDuration_s;
				double num = combatDuration_s - this.timeOfLastQuarterSecondUpdate_s;
				double num2 = Math.Truncate(num / 0.25);
				bool flag = false;
				if (num2 >= 1.0)
				{
					int num3 = 0;
					while ((double)num3 < num2)
					{
						this.dateTimeofLastQuarterSecondUpdateLoop = this.dateTimeofLastQuarterSecondUpdateLoop.AddMilliseconds(250.0);
						this.CombatQuarterSecond(this.dateTimeofLastQuarterSecondUpdateLoop, this.quarterSecondCounter % 4, (double)num3 == num2 - 1.0);
						this.quarterSecondCounter++;
						if (this.quarterSecondCounter % 240 == 0)
						{
							flag = true;
						}
						num3++;
					}
					this.timeOfLastQuarterSecondUpdate_s = combatDuration_s - (num - num2 * 0.25);
				}
				double num4 = combatDuration_s - this.timeOfLastUpdate_s;
				double num5 = combatDuration_s - this.timeOfLastSecondUpdate_s;
				foreach (CombatFleetController combatFleetController in this.fleetControllers)
				{
					TIFactionState faction = combatFleetController.fleetState.faction;
					if (!(faction == null) && !(combatFleetController.fleetState == null) && this._maxShipsInBattle.ContainsKey(combatFleetController.fleetState) && this._reinforcementCount.ContainsKey(faction))
					{
						if (faction == GameControl.control.activePlayer)
						{
							if (this._sendInPlayerReinforcements || (combatFleetController.AllActiveShipsDestroyed() && combatFleetController.reinforcements.Count > 0))
							{
								Vector3 vector = default(Vector3);
								Vector3 vector2 = default(Vector3);
								this.GetReinforcementPositionAndVelocity(combatFleetController, out vector, out vector2);
								if (Math.Round(this.timeOfLastUpdate_s) % 60.0 == 0.0)
								{
									this._sendInPlayerReinforcements = false;
									List<CombatShipController> list;
									if (this.TryReinforceFleet(faction, this._reinforcementCount[faction], vector, vector2, out list))
									{
										Dictionary<TIFactionState, int> dictionary = this._reinforcementCount;
										TIFactionState tifactionState = faction;
										dictionary[tifactionState] -= list.Count;
										this.combatHUD.UpdateReinforcementUI(faction, combatFleetController, list);
										this.combatHUD.UpdateFleetCommandPanel();
										this.combatHUD.UpdateCommandPanel(this.combatHUD.groupSelectedFriendlyShips.Count > 1);
										break;
									}
								}
								else
								{
									this._fleetMarker.transform.position = vector;
									this._fleetMarker.SetActive(true);
									this.combatHUD.UpdateReinforcmentTimerText((float)(60.0 - Math.Round(this.timeOfLastUpdate_s) % 60.0), combatFleetController.faction == GameControl.control.activePlayer);
								}
							}
							else
							{
								this._fleetMarker.SetActive(false);
								this._playerRandomReinforcementPosition = Vector3.zero;
								this.combatHUD.ShowReinforcementTimer(false, combatFleetController.faction == GameControl.control.activePlayer);
							}
						}
						else
						{
							bool flag2 = this._reinforcementCount[faction] > this._maxShipsInBattle[combatFleetController.fleetState] / 2 || combatFleetController.reinforcements.Count == this._reinforcementCount[faction];
							if (this._reinforcementCount[faction] > 0 && combatFleetController.reinforcements.Count > 0 && flag2)
							{
								Vector3 vector3 = default(Vector3);
								Vector3 vector4 = default(Vector3);
								this.GetReinforcementPositionAndVelocity(combatFleetController, out vector3, out vector4);
								if (Math.Round(this.timeOfLastUpdate_s) % 60.0 == 0.0)
								{
									List<CombatShipController> list2;
									if (this.TryReinforceFleet(faction, this._reinforcementCount[faction], vector3, vector4, out list2))
									{
										Dictionary<TIFactionState, int> dictionary = this._reinforcementCount;
										TIFactionState tifactionState = faction;
										dictionary[tifactionState] -= list2.Count;
										this.combatHUD.UpdateReinforcementUI(faction, combatFleetController, list2);
										break;
									}
								}
								else
								{
									this._opposingFleetMarker.transform.position = vector3;
									this._opposingFleetMarker.SetActive(true);
									this.combatHUD.UpdateReinforcmentTimerText((float)(60.0 - Math.Round(this.timeOfLastUpdate_s) % 60.0), combatFleetController.faction == GameControl.control.activePlayer);
								}
							}
							else
							{
								this._opposingFleetMarker.SetActive(false);
								this._opposingRandomReinforcementPosition = Vector3.zero;
								this.combatHUD.ShowReinforcementTimer(false, combatFleetController.faction == GameControl.control.activePlayer);
							}
						}
					}
				}
				double num6 = Math.Truncate(num5);
				if (num6 >= 1.0)
				{
					int num7 = 0;
					while ((double)num7 < num6)
					{
						this.CombatSecond((double)num7 == num6 - 1.0);
						num7++;
					}
					GameControl.eventManager.TriggerEvent(new CombatSecond(), null, Array.Empty<object>());
					this.timeOfLastSecondUpdate_s = combatDuration_s - (num5 - num6);
				}
				this.CombatFractionalSecond(num4);
				this._projectileJobContainer.UpdateControllers();
				this.UpdateShipControllers(num4);
				this.UpdateCombatHabModuleControllers();
				this.timeOfLastUpdate_s = combatDuration_s;
				this.HandleControlGroupInputs();
				bool flag3 = true;
				for (int i = 0; i < this.fleetControllers.Count; i++)
				{
					if (this.liveMissiles[this.fleetControllers[i].faction] > 0)
					{
						flag3 = false;
					}
				}
				int num8 = this.combatHabModuleControllers.Count<CombatHabModuleController>((CombatHabModuleController x) => x.destructionTriggered && !x.habModule.destroyed);
				if (this.combatDuration_s > this.endCombatTime && this.combatEndTriggered && this.combatState.shipDestroyedTriggers <= this.combatState.shipDestructionsRecorded && flag3 && num8 == 0)
				{
					this.InvokeEndCombat();
					return;
				}
				if (this.forceEndCombatTime > 0.0 && this.combatDuration_s > this.forceEndCombatTime)
				{
					Log.Error(string.Concat(new string[]
					{
						"Force end combat used: ",
						this.combatState.shipDestroyedTriggers.ToString(),
						"/",
						this.combatState.shipDestructionsRecorded.ToString(),
						":",
						flag3.ToString(),
						":",
						num8.ToString()
					}), Array.Empty<object>());
					this.InvokeEndCombat();
					return;
				}
				if (!this.combatEndTriggered && flag)
				{
					this.FullEndCombatCheck();
				}
			}
		}

		// Token: 0x060029BF RID: 10687 RVA: 0x000E1648 File Offset: 0x000DF848
		private void LateUpdate()
		{
			if (this.backgroundCamera != null)
			{
				if (this.primarySpaceBody != null)
				{
					Vector3 vector = this.mainCameraTransform.position - this.primarySpaceBody.transform.position;
					Vector3 vector2;
					if (vector.magnitude < this.minimumDistanceToPrimarySaceBody)
					{
						vector2 = vector.normalized * this.minimumDistanceToPrimarySaceBody + this.primarySpaceBody.transform.position;
					}
					else
					{
						vector2 = this.mainCameraTransform.position;
					}
					if (this.backgroundCameraTransform.position != vector2)
					{
						this.backgroundCameraTransform.position = vector2;
					}
				}
				this.backgroundCameraTransform.rotation = this.mainCameraTransform.rotation;
			}
		}

		// Token: 0x060029C0 RID: 10688 RVA: 0x000E1714 File Offset: 0x000DF914
		private void UpdateShipControllers(double deltaTime_s)
		{
			if (deltaTime_s > 0.0)
			{
				this._combatAIController.Update(this.gameTime.currentTime);
			}
			foreach (CombatShipController combatShipController in this.ships)
			{
				combatShipController.UpdateShip();
			}
		}

		// Token: 0x060029C1 RID: 10689 RVA: 0x000E1788 File Offset: 0x000DF988
		private void UpdateCombatHabModuleControllers()
		{
			foreach (CombatHabModuleController combatHabModuleController in this.combatHabModuleControllers)
			{
				combatHabModuleController.UpdateHab();
			}
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x000E17D8 File Offset: 0x000DF9D8
		private void HandleControlGroupInputs()
		{
			if (TIInputManager.ControlGroupKeyPressedThisFrame)
			{
				int num = -1;
				if (Input.GetKeyDown(TIInputManager.controlGroup0))
				{
					num = 0;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup1))
				{
					num = 1;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup2))
				{
					num = 2;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup3))
				{
					num = 3;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup4))
				{
					num = 4;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup5))
				{
					num = 5;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup6))
				{
					num = 6;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup7))
				{
					num = 7;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup8))
				{
					num = 8;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup9))
				{
					num = 9;
				}
				else if (Input.GetKeyDown(TIInputManager.controlGroup0))
				{
					num = 0;
				}
				if (TIInputManager.IsControlKeyDown)
				{
					if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
					{
						this.SetControlGroup(num, GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips);
						return;
					}
					if (GameControl.spaceCombat.combatHUD.selectedFriendlyShip != null)
					{
						this.SetControlGroup(num, GameControl.spaceCombat.combatHUD.selectedFriendlyShip);
						return;
					}
				}
				else
				{
					this.SelectControlGroup(num);
				}
			}
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x000E1910 File Offset: 0x000DFB10
		private void SetControlGroup(int groupNumber, List<CombatShipController> ships)
		{
			if (this._controlGroups.ContainsKey(groupNumber))
			{
				foreach (TISpaceShipState tispaceShipState in this._controlGroups[groupNumber])
				{
					(this.combatantLookup[tispaceShipState] as CombatShipController).controlGroups.Remove(groupNumber);
					GameControl.eventManager.TriggerEvent(new CombatShipGroupChange(tispaceShipState, groupNumber), null, new object[] { tispaceShipState });
				}
				this._controlGroups.Remove(groupNumber);
			}
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			foreach (CombatShipController combatShipController in ships)
			{
				list.Add(combatShipController.ShipState);
				combatShipController.controlGroups.Add(groupNumber);
			}
			this._controlGroups.Add(groupNumber, list);
			foreach (CombatShipController combatShipController2 in ships)
			{
				GameControl.eventManager.TriggerEvent(new CombatShipGroupChange(combatShipController2.ShipState, groupNumber), null, new object[] { combatShipController2.ShipState });
			}
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x000E1A7C File Offset: 0x000DFC7C
		private void SetControlGroup(int groupNumber, CombatShipController ship)
		{
			if (this._controlGroups.ContainsKey(groupNumber))
			{
				foreach (TISpaceShipState tispaceShipState in this._controlGroups[groupNumber])
				{
					(this.combatantLookup[tispaceShipState] as CombatShipController).controlGroups.Remove(groupNumber);
					GameControl.eventManager.TriggerEvent(new CombatShipGroupChange(tispaceShipState, groupNumber), null, new object[] { tispaceShipState });
				}
				this._controlGroups.Remove(groupNumber);
			}
			List<TISpaceShipState> list = new List<TISpaceShipState> { ship.ShipState };
			this._controlGroups.Add(groupNumber, list);
			ship.controlGroups.Add(groupNumber);
			GameControl.eventManager.TriggerEvent(new CombatShipGroupChange(ship.ShipState, groupNumber), null, new object[] { ship.ShipState });
		}

		// Token: 0x060029C5 RID: 10693 RVA: 0x000E1B74 File Offset: 0x000DFD74
		private void RemoveShipFromControlGroups(TISpaceShipState ship)
		{
			for (int i = 0; i <= 9; i++)
			{
				if (this._controlGroups.ContainsKey(i) && this._controlGroups[i].Contains(ship))
				{
					(this.combatantLookup[ship] as CombatShipController).controlGroups.Remove(i);
					GameControl.eventManager.TriggerEvent(new CombatShipGroupChange(ship, i), null, new object[] { ship });
					this._controlGroups[i].Remove(ship);
				}
			}
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x000E1BFC File Offset: 0x000DFDFC
		public void SelectControlGroup(int groupNumber)
		{
			if (this._controlGroups.ContainsKey(groupNumber))
			{
				GameControl.spaceCombat.combatHUD.ClearGroupSelect();
				List<TISpaceShipState> list = this._controlGroups[groupNumber];
				for (int i = 0; i < list.Count; i++)
				{
					TISpaceShipState tispaceShipState = list[i];
					GameControl.eventManager.TriggerEvent(new CombatTargetedableStateSelected(tispaceShipState, true, i == 0), null, Array.Empty<object>());
				}
			}
			if (GameControl.spaceCombat.combatHUD.selectedFriendlyShip == null)
			{
				return;
			}
			this._controlDoubleClickCount++;
			if (this._controlDoubleClickCount == 1)
			{
				this._lastClickTime = Time.time;
			}
			if (this._controlDoubleClickCount > 1 && Time.time - this._lastClickTime <= this._controlDoubleClickWindow)
			{
				this._controlDoubleClickCount = 0;
				this._lastClickTime = 0f;
				this.combatCamera.LookAtCombatant(GameControl.spaceCombat.combatHUD.selectedFriendlyShip);
				return;
			}
			this._controlDoubleClickCount = 1;
			this._lastClickTime = Time.time;
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x000E1CFD File Offset: 0x000DFEFD
		public Dictionary<int, List<TISpaceShipState>> GetControlGroups()
		{
			return this._controlGroups;
		}

		// Token: 0x060029C8 RID: 10696 RVA: 0x000E1D08 File Offset: 0x000DFF08
		public void ArrangePlayerFleetInFormation(CombatFleetController playerFleet, bool spawnInFrontOfHabIfPresent)
		{
			List<CombatShipController> list = new List<CombatShipController>();
			CombatHabModuleController combatHabModuleController = null;
			int num = 0;
			if (num < this.combatHabModuleControllers.Count && this.combatHabModuleControllers[num].faction.isActivePlayer)
			{
				combatHabModuleController = this.combatHabModuleControllers[num];
			}
			float num2 = 0f;
			if (combatHabModuleController != null)
			{
				List<CombatShipController> list2 = playerFleet.activeShipControllers.Where<CombatShipController>((CombatShipController x) => x.ShipState.combatant).ToList<CombatShipController>();
				if (list2.Count == 0)
				{
					list2 = playerFleet.activeShipControllers.ToList<CombatShipController>();
				}
				float num3 = Mathf.Abs(list2.Min<CombatShipController>((CombatShipController x) => (float)(x.ShipState.fleetFormationOffset * 0.05000000074505806).z)) + list2.Max<CombatShipController>((CombatShipController x) => (float)(x.ShipState.fleetFormationOffset * 0.05000000074505806).z);
				if (playerFleet.FleetIndex == 0)
				{
					if (spawnInFrontOfHabIfPresent)
					{
						num2 = combatHabModuleController.position.z + SpaceCombatManager.km_to_scale(150f) + Mathf.Min(num3, SpaceCombatManager.km_to_scale(150f));
					}
					else
					{
						num2 = combatHabModuleController.position.z - SpaceCombatManager.km_to_scale(150f);
					}
				}
				else if (playerFleet.FleetIndex == 1)
				{
					if (spawnInFrontOfHabIfPresent)
					{
						num2 = combatHabModuleController.position.z - SpaceCombatManager.km_to_scale(150f) - Mathf.Min(num3, SpaceCombatManager.km_to_scale(150f));
					}
					else
					{
						num2 = combatHabModuleController.position.z + SpaceCombatManager.km_to_scale(150f);
					}
				}
			}
			foreach (CombatShipController combatShipController in playerFleet.activeShipControllers)
			{
				Vector3 vector = (Vector3)combatShipController.ShipState.fleetFormationOffset * 0.05f;
				int fleetIndex = playerFleet.FleetIndex;
				if (fleetIndex != 0)
				{
					if (fleetIndex == 1)
					{
						if (combatHabModuleController != null)
						{
							vector = (Vector3)combatShipController.ShipState.fleetFormationOffset * 0.05f + new Vector3(0f, 0f, num2);
						}
						else
						{
							vector = (Vector3)(combatShipController.ShipState.fleetFormationOffset + this.secondFleetOffset) * 0.05f;
							if (vector.z < this.scaledSecondFleetOffset.z)
							{
								Log.Warn("Fleet 1 Formation placed ship with bad z: " + vector.z.ToString(), Array.Empty<object>());
								for (int i = 0; i < 10; i++)
								{
									vector = new Vector3(vector.x, vector.y, vector.z + this.scaledSecondFleetOffset.z);
									if (vector.z >= this.scaledSecondFleetOffset.z)
									{
										break;
									}
								}
							}
						}
					}
				}
				else
				{
					vector = (Vector3)combatShipController.ShipState.fleetFormationOffset * 0.05f + new Vector3(0f, 0f, num2);
					if (vector.z > 0f)
					{
						Log.Warn("Fleet 0 Formation placed ship with bad z: " + vector.z.ToString() + " Setting to 0", Array.Empty<object>());
						vector.z = 0f;
					}
				}
				combatShipController.combatantTransform.position = vector;
				combatShipController.ReinitializeWaypoints();
				list.Add(combatShipController.ref_shipController);
			}
		}

		// Token: 0x060029C9 RID: 10697 RVA: 0x000E20B0 File Offset: 0x000E02B0
		private void TurnOnFormationSelectionMode()
		{
			this._inFormationSelectionMode = true;
			CombatFleetController combatFleetController = null;
			CombatFleetController combatFleetController2 = null;
			foreach (CombatFleetController combatFleetController3 in this.fleetControllers)
			{
				if (!combatFleetController3.faction.isActivePlayer)
				{
					combatFleetController2 = combatFleetController3;
				}
				else
				{
					combatFleetController = combatFleetController3;
				}
			}
			if (combatFleetController2 == null)
			{
				return;
			}
			this._opposingFleetMarker = global::UnityEngine.Object.Instantiate<Transform>(this.fleetMarkerPrefab, combatFleetController2.GetCenterOfMass(), Quaternion.identity).gameObject;
			this._opposingFleetMarker.GetComponent<FleetFormationPositionMarkerController>().Initialize(combatFleetController2.fleetState);
			foreach (CombatShipController combatShipController in combatFleetController2.activeShipControllers)
			{
				combatShipController._waypointNavigationController.ToggleWaypointVisualization();
				combatShipController.gameObject.SetActive(false);
			}
			if (combatFleetController == null)
			{
				return;
			}
			this._fleetMarker = global::UnityEngine.Object.Instantiate<Transform>(this.fleetMarkerPrefab, Vector3.zero, Quaternion.identity).gameObject;
			this._fleetMarker.GetComponent<FleetFormationPositionMarkerController>().Initialize(combatFleetController.fleetState);
			this._fleetMarker.SetActive(false);
		}

		// Token: 0x060029CA RID: 10698 RVA: 0x000E21EC File Offset: 0x000E03EC
		public void TurnOffFormationSelectionMode()
		{
			this._inFormationSelectionMode = false;
			CombatFleetController combatFleetController = null;
			foreach (CombatFleetController combatFleetController2 in this.fleetControllers)
			{
				if (!combatFleetController2.faction.isActivePlayer)
				{
					combatFleetController = combatFleetController2;
				}
			}
			this.combatHUD.UpdateFleetCommandPanel();
			this.combatHUD.UpdateCommandPanel(this.combatHUD.groupSelectedFriendlyShips.Count > 1);
			if (combatFleetController == null)
			{
				return;
			}
			this._opposingFleetMarker.SetActive(false);
			foreach (CombatShipController combatShipController in combatFleetController.activeShipControllers)
			{
				combatShipController._waypointNavigationController.ToggleWaypointVisualization();
				combatShipController.gameObject.SetActive(true);
			}
			foreach (CombatShipController combatShipController2 in this.activeShips)
			{
				combatShipController2.EnableRootCollider();
			}
			if (this.combatState.hab != null)
			{
				this.ConfigureHabCollisionObjectsForCombat();
			}
		}

		// Token: 0x060029CB RID: 10699 RVA: 0x000E232C File Offset: 0x000E052C
		private Formation SetAIFormation(TISpaceFleetState fleetState)
		{
			bool flag = false;
			FormationSpacing formationSpacing = FormationSpacing.Loose;
			if (fleetState != null && fleetState.ships.Count > 0)
			{
				float num = fleetState.ships.Max<TISpaceShipState>((TISpaceShipState x) => x.hull.width_m);
				if (num >= 60f)
				{
					flag = true;
					formationSpacing = FormationSpacing.Close;
				}
				else if (num >= 30f)
				{
					flag = true;
					formationSpacing = FormationSpacing.Close;
				}
			}
			Formation formation2;
			if (this.combatState.hab != null && this.combatState.hab.ref_faction == fleetState.faction)
			{
				List<TIFormationTemplate> list = TemplateManager.IterateByClass<TIFormationTemplate>(true).Where<TIFormationTemplate>(delegate(TIFormationTemplate x)
				{
					if (x.AICombatBaseWeight > 0f)
					{
						if (x.pos.All<Vector3>((Vector3 y) => y.z == 0f))
						{
							return fleetState.ships.Count < x.AIMaximumAllowedShips || x.AIMaximumAllowedShips == -1;
						}
					}
					return false;
				}).ToList<TIFormationTemplate>();
				Formation formation;
				switch (TIUtilities.RandomRange(0, 5))
				{
				case 0:
				{
					formation = default(Formation);
					string text;
					if (list.Count <= 0)
					{
						text = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
					}
					else
					{
						text = list.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
					}
					formation.patternDataName = text;
					formation.concentration = FormationConcentration.Center;
					formation.focus = (FormationFocus)TIUtilities.RandomRange(0, 5);
					formation.spacing = (flag ? formationSpacing : FormationSpacing.Tight);
					formation2 = formation;
					goto IL_059D;
				}
				case 1:
				{
					formation = default(Formation);
					string text2;
					if (list.Count <= 0)
					{
						text2 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
					}
					else
					{
						text2 = list.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
					}
					formation.patternDataName = text2;
					formation.concentration = FormationConcentration.Dispersed;
					formation.focus = FormationFocus.PointDefense;
					formation.spacing = (flag ? formationSpacing : FormationSpacing.Tight);
					formation2 = formation;
					goto IL_059D;
				}
				case 2:
				{
					formation = default(Formation);
					string text3;
					if (list.Count <= 0)
					{
						text3 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
					}
					else
					{
						text3 = list.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
					}
					formation.patternDataName = text3;
					formation.concentration = (FormationConcentration)TIUtilities.RandomRange(8, 11);
					formation.focus = FormationFocus.Swift;
					formation.spacing = (flag ? formationSpacing : ((FormationSpacing)TIUtilities.RandomRange(0, 3)));
					formation2 = formation;
					goto IL_059D;
				}
				}
				formation = default(Formation);
				string text4;
				if (list.Count <= 0)
				{
					text4 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
				}
				else
				{
					text4 = list.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
				}
				formation.patternDataName = text4;
				formation.concentration = (FormationConcentration)TIUtilities.RandomRange(2, 6);
				formation.focus = FormationFocus.Swift;
				formation.spacing = (flag ? formationSpacing : ((FormationSpacing)TIUtilities.RandomRange(0, 3)));
				formation2 = formation;
			}
			else
			{
				List<TIFormationTemplate> list2 = (from x in TemplateManager.IterateByClass<TIFormationTemplate>(true)
					where x.AICombatBaseWeight > 0f && (fleetState.ships.Count < x.AIMaximumAllowedShips || x.AIMaximumAllowedShips == -1)
					select x).ToList<TIFormationTemplate>();
				Formation formation;
				switch (TIUtilities.RandomRange(0, 5))
				{
				case 0:
				{
					formation = default(Formation);
					string text5;
					if (list2.Count <= 0)
					{
						text5 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
					}
					else
					{
						text5 = list2.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
					}
					formation.patternDataName = text5;
					formation.concentration = FormationConcentration.Center;
					formation.focus = (FormationFocus)TIUtilities.RandomRange(0, 5);
					formation.spacing = (flag ? formationSpacing : FormationSpacing.Tight);
					formation2 = formation;
					goto IL_059D;
				}
				case 1:
				{
					formation = default(Formation);
					string text6;
					if (list2.Count <= 0)
					{
						text6 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
					}
					else
					{
						text6 = list2.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
					}
					formation.patternDataName = text6;
					formation.concentration = FormationConcentration.Dispersed;
					formation.focus = FormationFocus.PointDefense;
					formation.spacing = (flag ? formationSpacing : FormationSpacing.Tight);
					formation2 = formation;
					goto IL_059D;
				}
				case 2:
				{
					formation = default(Formation);
					string text7;
					if (list2.Count <= 0)
					{
						text7 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
					}
					else
					{
						text7 = list2.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
					}
					formation.patternDataName = text7;
					formation.concentration = (FormationConcentration)TIUtilities.RandomRange(8, 11);
					formation.focus = FormationFocus.Swift;
					formation.spacing = (flag ? formationSpacing : FormationSpacing.Tight);
					formation2 = formation;
					goto IL_059D;
				}
				}
				formation = default(Formation);
				string text8;
				if (list2.Count <= 0)
				{
					text8 = TemplateManager.IterateByClass<TIFormationTemplate>(true).SelectRandomItem<TIFormationTemplate>().dataName;
				}
				else
				{
					text8 = list2.SelectRandomWeightedItem<TIFormationTemplate>((TIFormationTemplate x) => x.AICombatBaseWeight, -1f, 1E-37f).dataName;
				}
				formation.patternDataName = text8;
				formation.concentration = (FormationConcentration)TIUtilities.RandomRange(2, 6);
				formation.focus = FormationFocus.Swift;
				formation.spacing = (flag ? formationSpacing : FormationSpacing.Tight);
				formation2 = formation;
			}
			IL_059D:
			Debug.Log(fleetState.faction.displayNameCapitalizedWithColor + " AI Formation: " + formation2.displayName);
			Debug.Log("MaxShipsInCombat: " + TIPlayerProfileManager.maxShipsInCombat.ToString());
			return formation2;
		}

		// Token: 0x04001FC4 RID: 8132
		private const float INITIAL_COMBAT_OFFSET_MODIFIER_CONFRONTATION = 1.225f;

		// Token: 0x04001FC5 RID: 8133
		private const float INITIAL_COMBAT_OFFSET_MODIFIER_CHASE = 1.05f;

		// Token: 0x04001FC6 RID: 8134
		private const float INITIAL_FORMATION_OFFSET_MIN_km = 25f;

		// Token: 0x04001FC7 RID: 8135
		private const float INITIAL_FORMATION_OFFSET_MAX_km = 35f;

		// Token: 0x04001FC8 RID: 8136
		public const float EXTREME_COMBAT_DISTANCE_km = 31315f;

		// Token: 0x04001FC9 RID: 8137
		public const float REENGAGE_COMBAT_DISTANCE_km = 2250f;

		// Token: 0x04001FCA RID: 8138
		public float waypointTimeDelta;

		// Token: 0x04001FCB RID: 8139
		public int waypointCount;

		// Token: 0x04001FCD RID: 8141
		public Transform shipPrefab;

		// Token: 0x04001FCE RID: 8142
		public Transform shipModelPrefab;

		// Token: 0x04001FCF RID: 8143
		public Transform waypointPrefab;

		// Token: 0x04001FD0 RID: 8144
		public Transform enemyWaypointPrefab;

		// Token: 0x04001FD1 RID: 8145
		public Transform combatGridPrefab;

		// Token: 0x04001FD2 RID: 8146
		public Transform fleetMarkerPrefab;

		// Token: 0x04001FD8 RID: 8152
		public List<CombatShipController> ships = new List<CombatShipController>();

		// Token: 0x04001FD9 RID: 8153
		public List<CombatShipController> activeShips = new List<CombatShipController>();

		// Token: 0x04001FDA RID: 8154
		public List<CombatFleetController> fleetControllers = new List<CombatFleetController>();

		// Token: 0x04001FDB RID: 8155
		public List<CombatHabModuleController> combatHabModuleControllers = new List<CombatHabModuleController>();

		// Token: 0x04001FDC RID: 8156
		private List<GameObject> combatSpaceBodies = new List<GameObject>();

		// Token: 0x04001FDD RID: 8157
		public Dictionary<CombatTargetableState, CombatantController> combatantLookup;

		// Token: 0x04001FDE RID: 8158
		private Dictionary<CombatShipController, SegmentProximityData> _shipToNearestSegment = new Dictionary<CombatShipController, SegmentProximityData>();

		// Token: 0x04001FDF RID: 8159
		private CombatAIController _combatAIController;

		// Token: 0x04001FE0 RID: 8160
		private GameTimeManager gameTime;

		// Token: 0x04001FE1 RID: 8161
		public CombatSetup setup;

		// Token: 0x04001FE2 RID: 8162
		public bool initialized;

		// Token: 0x04001FE3 RID: 8163
		public TIDateTime timeOfLastShotFired;

		// Token: 0x04001FE4 RID: 8164
		public Dictionary<TIFactionState, int> liveMissiles = new Dictionary<TIFactionState, int>();

		// Token: 0x04001FE5 RID: 8165
		public Dictionary<TIFactionState, int> liveBallistics = new Dictionary<TIFactionState, int>();

		// Token: 0x04001FE6 RID: 8166
		public bool waypointsVisible = true;

		// Token: 0x04001FE7 RID: 8167
		private readonly float spaceCombatFarClipPlane = 10000f;

		// Token: 0x04001FE8 RID: 8168
		private float originalFarClipPlane;

		// Token: 0x04001FE9 RID: 8169
		private bool _isSegmentSelectionComplete;

		// Token: 0x04001FEA RID: 8170
		private bool _isChangePending;

		// Token: 0x04001FEB RID: 8171
		private float _initialFrameChangeRequest;

		// Token: 0x04001FEC RID: 8172
		private CombatShipController _pendingWaypointPlacementShip;

		// Token: 0x04001FED RID: 8173
		private int _pendingNearestSegmentWaypointId = -1;

		// Token: 0x04001FEE RID: 8174
		private CombatShipController _activeWaypointPlacementShip;

		// Token: 0x04001FEF RID: 8175
		private int _activeNearestSegmentWaypointId = -1;

		// Token: 0x04001FF0 RID: 8176
		private bool shotFired;

		// Token: 0x04001FF1 RID: 8177
		private bool shipDestroyed;

		// Token: 0x04001FF2 RID: 8178
		[SerializeField]
		private Vector3 storedStratCameraPosition;

		// Token: 0x04001FF3 RID: 8179
		[SerializeField]
		private Quaternion storedStratCameraRotation;

		// Token: 0x04001FF4 RID: 8180
		public const float UNIT_SCALING_FACTOR = 0.05f;

		// Token: 0x04001FF5 RID: 8181
		private static float _cachedScalingAdjustmentFactor;

		// Token: 0x04001FF6 RID: 8182
		private double endCombatTime;

		// Token: 0x04001FF7 RID: 8183
		private double forceEndCombatTime;

		// Token: 0x04001FF8 RID: 8184
		private readonly double forceEndCombatDuration_s = 900.0;

		// Token: 0x04001FF9 RID: 8185
		private readonly double endCombatDuration_s = 60.0;

		// Token: 0x04001FFA RID: 8186
		public SkirmishModeSettings prevSkirmishSettings;

		// Token: 0x04001FFB RID: 8187
		private int _controlDoubleClickCount;

		// Token: 0x04001FFC RID: 8188
		private float _lastClickTime;

		// Token: 0x04001FFD RID: 8189
		private float _controlDoubleClickWindow = 0.5f;

		// Token: 0x04001FFE RID: 8190
		private Vector3 scaledSecondFleetOffset;

		// Token: 0x04001FFF RID: 8191
		private Vector3 secondFleetOffset;

		// Token: 0x04002000 RID: 8192
		private Camera backgroundCamera;

		// Token: 0x04002001 RID: 8193
		private Transform backgroundCameraTransform;

		// Token: 0x04002002 RID: 8194
		public GameObjectDictionary<string> _container;

		// Token: 0x04002003 RID: 8195
		private bool _waypointInputDragging;

		// Token: 0x04002004 RID: 8196
		public GameObject _fleetMarker;

		// Token: 0x04002005 RID: 8197
		public GameObject _opposingFleetMarker;

		// Token: 0x04002006 RID: 8198
		private bool _inFormationSelectionMode;

		// Token: 0x04002007 RID: 8199
		private Dictionary<int, List<TISpaceShipState>> _controlGroups;

		// Token: 0x04002008 RID: 8200
		private RaycastHit hit;

		// Token: 0x04002009 RID: 8201
		private bool _dragSelectValid;

		// Token: 0x0400200A RID: 8202
		private bool _isDragSelecting;

		// Token: 0x0400200B RID: 8203
		private Vector3 _boxSelectStartPosition;

		// Token: 0x0400200C RID: 8204
		private Vector3 _boxSelectEndPosition;

		// Token: 0x0400200D RID: 8205
		private Color _boxColor = new Color(0.843f, 0.98f, 0.988f);

		// Token: 0x0400200E RID: 8206
		public List<ShipUIController> _boxSelectedUIControllers;

		// Token: 0x0400200F RID: 8207
		private MeshCollider _selectionBox;

		// Token: 0x04002010 RID: 8208
		private Mesh _selectionMesh;

		// Token: 0x04002011 RID: 8209
		private Vector2[] _corners;

		// Token: 0x04002012 RID: 8210
		private Vector3[] _verts;

		// Token: 0x04002013 RID: 8211
		private Vector3[] _vecs;

		// Token: 0x04002014 RID: 8212
		public GameObject _projectileContainer;

		// Token: 0x04002015 RID: 8213
		private ProjectileJobContainer _projectileJobContainer;

		// Token: 0x04002016 RID: 8214
		public Dictionary<TISpaceCombatProjectileState, ProjectileController> _projectiles;

		// Token: 0x04002017 RID: 8215
		public Dictionary<ProjectileController, TISpaceCombatProjectileState> _reverseProjectiles;

		// Token: 0x04002018 RID: 8216
		public List<SpaceCombatManager.CombatPathLine> combatPathLines = new List<SpaceCombatManager.CombatPathLine>();

		// Token: 0x04002019 RID: 8217
		private float minimumDistanceToPrimarySaceBody;

		// Token: 0x0400201A RID: 8218
		private int initialMainCameraCullingMask;

		// Token: 0x0400201B RID: 8219
		private Material combatCameraBlendEffect;

		// Token: 0x0400201C RID: 8220
		private SpaceCombatCameraBlend combatCameraBlend;

		// Token: 0x0400201D RID: 8221
		private GameObject primarySpaceBody;

		// Token: 0x0400201E RID: 8222
		private CameraClearFlags initialCameraClearFlags;

		// Token: 0x0400201F RID: 8223
		private float priorIntensity;

		// Token: 0x04002020 RID: 8224
		private Dictionary<TIFactionState, int> _reinforcementCount;

		// Token: 0x04002021 RID: 8225
		private Dictionary<TISpaceFleetState, int> _maxShipsInBattle;

		// Token: 0x04002022 RID: 8226
		private bool _sendInPlayerReinforcements;

		// Token: 0x04002023 RID: 8227
		private Vector3 _playerRandomReinforcementPosition;

		// Token: 0x04002024 RID: 8228
		private Vector3 _opposingRandomReinforcementPosition;

		// Token: 0x04002027 RID: 8231
		private readonly WaitForSeconds briefWait = new WaitForSeconds(0.05f);

		// Token: 0x04002028 RID: 8232
		private GameObject strategyHabObject;

		// Token: 0x04002029 RID: 8233
		private GameObject habModelObject;

		// Token: 0x0400202A RID: 8234
		public HabModelController habModelController;

		// Token: 0x0400202B RID: 8235
		private Vector3 strategyHabModelLocalPosition;

		// Token: 0x0400202C RID: 8236
		private Quaternion strategyHabModelLocalRotation;

		// Token: 0x0400202D RID: 8237
		private Vector3 strategyHabModelLocalScale;

		// Token: 0x0400202E RID: 8238
		private Dictionary<TIHabState, bool> stratLayerHabActiveStatus = new Dictionary<TIHabState, bool>();

		// Token: 0x0400202F RID: 8239
		private TIDateTime combatStartDateTime;

		// Token: 0x04002030 RID: 8240
		private List<CombatShipController> shipsToDisengage = new List<CombatShipController>();

		// Token: 0x04002031 RID: 8241
		private double timeOfLastUpdate_s;

		// Token: 0x04002032 RID: 8242
		private double timeOfLastSecondUpdate_s;

		// Token: 0x04002033 RID: 8243
		private double timeOfLastQuarterSecondUpdate_s;

		// Token: 0x04002034 RID: 8244
		private DateTime dateTimeofLastQuarterSecondUpdateLoop;

		// Token: 0x04002035 RID: 8245
		private int quarterSecondCounter;

		// Token: 0x04002036 RID: 8246
		private int kount;

		// Token: 0x02000D12 RID: 3346
		public class CombatPathLine
		{
			// Token: 0x04005059 RID: 20569
			public Vector3 start;

			// Token: 0x0400505A RID: 20570
			public Vector3 end;

			// Token: 0x0400505B RID: 20571
			public LineEndCap endCap;

			// Token: 0x0400505C RID: 20572
			public Color color;
		}
	}
}
