using System;
using System.Collections.Generic;
using System.Linq;
using PavonisInteractive.TerraInvicta.Audio;
using PavonisInteractive.TerraInvicta.SpaceCombat.UI;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009FE RID: 2558
	public class SpaceCombatCameraController : MonoBehaviour
	{
		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06006230 RID: 25136 RVA: 0x002E04AC File Offset: 0x002DE6AC
		// (remove) Token: 0x06006231 RID: 25137 RVA: 0x002E04E4 File Offset: 0x002DE6E4
		public event SpaceCombatCameraController.CameraMovement OnCameraMovementFinished;

		// Token: 0x170010EB RID: 4331
		// (get) Token: 0x06006232 RID: 25138 RVA: 0x002E0519 File Offset: 0x002DE719
		private float maxZoom
		{
			get
			{
				return TemplateManager.global.combatCamera_maxZoom;
			}
		}

		// Token: 0x170010EC RID: 4332
		// (get) Token: 0x06006233 RID: 25139 RVA: 0x002E0525 File Offset: 0x002DE725
		private float minZoom
		{
			get
			{
				return TemplateManager.global.combatCamera_minZoom;
			}
		}

		// Token: 0x170010ED RID: 4333
		// (get) Token: 0x06006234 RID: 25140 RVA: 0x002E0531 File Offset: 0x002DE731
		private float maxPan
		{
			get
			{
				return TemplateManager.global.combatCamera_maxPan;
			}
		}

		// Token: 0x170010EE RID: 4334
		// (get) Token: 0x06006235 RID: 25141 RVA: 0x002E053D File Offset: 0x002DE73D
		private float minPan
		{
			get
			{
				return TemplateManager.global.combatCamera_minPan;
			}
		}

		// Token: 0x170010EF RID: 4335
		// (get) Token: 0x06006236 RID: 25142 RVA: 0x002E0549 File Offset: 0x002DE749
		private float minCameraMovementSpeed
		{
			get
			{
				return TemplateManager.global.combatCamera_minCameraMovementSpeed;
			}
		}

		// Token: 0x170010F0 RID: 4336
		// (get) Token: 0x06006237 RID: 25143 RVA: 0x002E0555 File Offset: 0x002DE755
		private float maxCameraMovementSpeed
		{
			get
			{
				return TemplateManager.global.combatCamera_maxCameraMovementSpeed;
			}
		}

		// Token: 0x170010F1 RID: 4337
		// (get) Token: 0x06006238 RID: 25144 RVA: 0x002E0561 File Offset: 0x002DE761
		private float minScrollSpeedOffset
		{
			get
			{
				return TemplateManager.global.combatCamera_minScrollSpeedOffset;
			}
		}

		// Token: 0x170010F2 RID: 4338
		// (get) Token: 0x06006239 RID: 25145 RVA: 0x002E056D File Offset: 0x002DE76D
		private float maxScrollSpeedOffset
		{
			get
			{
				return TemplateManager.global.combatCamera_maxScrollSpeedOffset;
			}
		}

		// Token: 0x170010F3 RID: 4339
		// (get) Token: 0x0600623A RID: 25146 RVA: 0x002E0579 File Offset: 0x002DE779
		private float mouseRotateSpeedOffset
		{
			get
			{
				return TemplateManager.global.combatCamera_mouseRotateSpeedOffset;
			}
		}

		// Token: 0x170010F4 RID: 4340
		// (get) Token: 0x0600623B RID: 25147 RVA: 0x002E0585 File Offset: 0x002DE785
		private float keyRotateSpeedOffset
		{
			get
			{
				return TemplateManager.global.combatCamera_keyRotateSpeedOffset;
			}
		}

		// Token: 0x170010F5 RID: 4341
		// (get) Token: 0x0600623C RID: 25148 RVA: 0x002E0591 File Offset: 0x002DE791
		private Vector3 Position
		{
			get
			{
				return this._cameraFocalPoint + (Vector3)this._polarOffset.ToCartesian();
			}
		}

		// Token: 0x170010F6 RID: 4342
		// (get) Token: 0x0600623D RID: 25149 RVA: 0x002E05AE File Offset: 0x002DE7AE
		private float Scale
		{
			get
			{
				return this._scale;
			}
		}

		// Token: 0x170010F7 RID: 4343
		// (get) Token: 0x0600623E RID: 25150 RVA: 0x002E05B6 File Offset: 0x002DE7B6
		public bool IsDragging
		{
			get
			{
				return this._dragging;
			}
		}

		// Token: 0x170010F8 RID: 4344
		// (get) Token: 0x0600623F RID: 25151 RVA: 0x002E05BE File Offset: 0x002DE7BE
		private float ScaledMaxZoom
		{
			get
			{
				return this.maxZoom * this.Scale;
			}
		}

		// Token: 0x170010F9 RID: 4345
		// (get) Token: 0x06006240 RID: 25152 RVA: 0x002E05CD File Offset: 0x002DE7CD
		private float ScaledMinZoom
		{
			get
			{
				return this.minZoom * this.Scale;
			}
		}

		// Token: 0x06006241 RID: 25153 RVA: 0x002E05DC File Offset: 0x002DE7DC
		private void Awake()
		{
			this._spaceCombatCamera = base.GetComponent<Camera>();
			this._mainCamera = Camera.main;
			this._layersMask = LayerMask.NameToLayer("Solar System") | LayerMask.NameToLayer("Selection") | LayerMask.NameToLayer("HurtBox") | LayerMask.NameToLayer("Ignore Raycast");
			this._spaceCombatRoot = GameControl.spaceCombat.container.gameObject.transform;
		}

		// Token: 0x06006242 RID: 25154 RVA: 0x002E064B File Offset: 0x002DE84B
		private void OnEnable()
		{
			this.InitializeCamera();
		}

		// Token: 0x06006243 RID: 25155 RVA: 0x002E0654 File Offset: 0x002DE854
		private void Update()
		{
			if (this._spaceCombat.combatEnded)
			{
				return;
			}
			if (this._spaceCombat.combatGrid == null)
			{
				return;
			}
			this.HandleSpecializedInput();
			if (this.IsCameraMovementBlocked)
			{
				return;
			}
			bool flag;
			bool flag2;
			bool flag3;
			bool flag4;
			bool flag5;
			bool flag6;
			float num;
			float num2;
			float num3;
			float num4;
			float num5;
			float num6;
			this.CheckInput(out flag, out flag2, out flag3, out flag4, out flag5, out flag6, out num, out num2, out num3, out num4, out num5, out num6);
			if ((this._focusedTarget == null || this._focusedTarget.isDestroyed) && this._cameraMovement == SpaceCombatCameraController.Movement.Follow)
			{
				this.SetCameraState(SpaceCombatCameraController.Movement.FreeLook);
			}
			SpaceCombatCameraController.Movement cameraMovement = this._cameraMovement;
			if (cameraMovement != SpaceCombatCameraController.Movement.FreeLook)
			{
				if (cameraMovement == SpaceCombatCameraController.Movement.Follow)
				{
					if (flag6)
					{
						this._followCameraOffset += this._cameraMovementSpeed * Input.GetAxis("Mouse Y") * this._spaceCombatCamera.transform.up;
						this._followCameraOffset += this._cameraMovementSpeed * 1.5f * Input.GetAxis("Mouse X") * this._spaceCombatCamera.transform.right;
						this._spaceCombatCamera.transform.position += this._followCameraOffset;
					}
					this.FollowTarget(num4, num5);
					this.HandleZoom(num6);
					float num7 = 0f;
					if (Input.GetKey(TIInputManager.cameraRight))
					{
						num7 += 1f;
					}
					if (Input.GetKey(TIInputManager.cameraLeft))
					{
						num7 -= 1f;
					}
					float num8 = 0f;
					if (Input.GetKey(TIInputManager.cameraUp))
					{
						num8 += 1f;
					}
					if (Input.GetKey(TIInputManager.cameraDown))
					{
						num8 -= 1f;
					}
					if (!Mathf.Approximately(num7, 0f) || !Mathf.Approximately(num8, 0f))
					{
						this.SetCameraState(SpaceCombatCameraController.Movement.FreeLook);
					}
				}
			}
			else
			{
				this.HandleMovement(num, num2, num3, flag6, flag5);
				this.HandleRotation(num4, num5);
				this.HandleZoom(num6);
			}
			if (this._cameraOrientationChanged)
			{
				SpaceCombatCameraController.CameraMovement onCameraMovementFinished = this.OnCameraMovementFinished;
				if (onCameraMovementFinished == null)
				{
					return;
				}
				onCameraMovementFinished(this._spaceCombatRoot.position, this._spaceCombatRoot.rotation);
			}
		}

		// Token: 0x06006244 RID: 25156 RVA: 0x002E0880 File Offset: 0x002DEA80
		private void InitializeCamera()
		{
			SpaceCombatManager.SetScalingAdjustmentFactor();
			this._scale = 0.05f * SpaceCombatManager.GetScalingAdjustmentFactor();
			this._spaceCombat = GameControl.spaceCombat;
			this._activePlayerFleetControllerIdx = (this._spaceCombat.fleetControllers[0].IsActivePlayerFleet ? 0 : 1);
			int num = ((!this._spaceCombat.fleetControllers[0].IsActivePlayerFleet) ? 0 : 1);
			CombatHabModuleController combatHabModuleController = null;
			int num2 = 0;
			if (num2 < this._spaceCombat.combatHabModuleControllers.Count && this._spaceCombat.combatHabModuleControllers[num2].faction.isActivePlayer)
			{
				combatHabModuleController = this._spaceCombat.combatHabModuleControllers[num2];
			}
			if (combatHabModuleController)
			{
				this._focusedTarget = combatHabModuleController;
				this._cameraFocalPoint = combatHabModuleController.transform.parent.parent.position + new Vector3(0f, 20f * SpaceCombatManager.GetScalingAdjustmentFactor(), -40f * SpaceCombatManager.GetScalingAdjustmentFactor());
				this._polarOffset = new Polar((double)(400f * this.Scale), 68.0, (double)(this._spaceCombat.fleetControllers[num].activeShipControllers[0].rotation.eulerAngles.y + 90f));
				this._targetPolarOffset = this._polarOffset.radius;
			}
			else
			{
				CombatShipController combatShipController = this._spaceCombat.fleetControllers[this._activePlayerFleetControllerIdx].activeShipControllers[0];
				Vector3 averagePosition = this._spaceCombat.fleetControllers[this._activePlayerFleetControllerIdx].GetAveragePosition();
				this._focusedTarget = combatShipController;
				this._cameraFocalPoint = averagePosition + new Vector3(0f, 3f * SpaceCombatManager.GetScalingAdjustmentFactor(), -6f * SpaceCombatManager.GetScalingAdjustmentFactor());
				this._polarOffset = new Polar((double)(400f * this.Scale), 68.0, (double)(this._focusedTarget.transform.rotation.eulerAngles.y + 270f));
				this._targetPolarOffset = this._polarOffset.radius;
			}
			this._previousTarget = null;
			this._spaceCombatCamera.transform.position = this.Position;
			this._spaceCombatCamera.transform.LookAt(this._cameraFocalPoint);
			this._cameraEulerAngles = this._spaceCombatCamera.transform.eulerAngles;
			float num3 = (float)(1.0 - ((double)this.ScaledMaxZoom - this._polarOffset.radius) / (double)(this.ScaledMaxZoom - this.ScaledMinZoom));
			this._cameraMovementSpeed = Mathf.Lerp(this.minCameraMovementSpeed, this.maxCameraMovementSpeed, num3) * this.Scale;
			this.SetCameraState(SpaceCombatCameraController.Movement.FreeLook);
		}

		// Token: 0x06006245 RID: 25157 RVA: 0x002E0B60 File Offset: 0x002DED60
		private void SetCameraState(SpaceCombatCameraController.Movement state)
		{
			if (this._cameraMovement != state)
			{
				this._followCameraOffset = Vector3.zero;
				this._cameraMovement = state;
				SpaceCombatCameraController.CameraMovement onCameraMovementFinished = this.OnCameraMovementFinished;
				if (onCameraMovementFinished == null)
				{
					return;
				}
				onCameraMovementFinished(this._spaceCombatRoot.position, this._spaceCombatRoot.rotation);
			}
		}

		// Token: 0x06006246 RID: 25158 RVA: 0x002E0BAE File Offset: 0x002DEDAE
		public void OnHudEnabled()
		{
			if (this._clockController == null)
			{
				this._clockController = (GameControl.canvasStack.CombatHud as SpaceCombatCanvasController).clockController;
			}
		}

		// Token: 0x06006247 RID: 25159 RVA: 0x002E0BD8 File Offset: 0x002DEDD8
		public void LookAtCombatant(CombatantController combatant)
		{
			this._previousTarget = this._focusedTarget;
			this._focusedTarget = combatant;
			this._cameraFocalPoint = combatant.position;
			this._followCameraOffset = Vector3.zero;
			this._spaceCombatCamera.transform.LookAt(this._cameraFocalPoint);
			this._targetPolarOffset = Mathd.Clamp((double)(this.ScaledMinZoom + (this.ScaledMaxZoom - this.ScaledMinZoom) * this._spaceCombat.modelScalingFactor), (double)this.ScaledMinZoom, (double)this.ScaledMaxZoom);
			this._zoomTimeRemaining = 0.15f;
			this.SetCameraState(SpaceCombatCameraController.Movement.Follow);
		}

		// Token: 0x06006248 RID: 25160 RVA: 0x002E0C74 File Offset: 0x002DEE74
		public void LookAtObject(GameObject target)
		{
			this._previousTarget = this._focusedTarget;
			this._cameraFocalPoint = target.transform.position;
			this._followCameraOffset = Vector3.zero;
			this._spaceCombatCamera.transform.LookAt(this._cameraFocalPoint);
			this._targetPolarOffset = Mathd.Clamp((double)(this.ScaledMinZoom + (this.ScaledMaxZoom - this.ScaledMinZoom) * this._spaceCombat.modelScalingFactor), (double)this.ScaledMinZoom, (double)this.ScaledMaxZoom);
			this._zoomTimeRemaining = 0.15f;
			this.ClearFocusedTarget();
			this._cameraEulerAngles = this._spaceCombatCamera.transform.eulerAngles;
		}

		// Token: 0x06006249 RID: 25161 RVA: 0x002E0D20 File Offset: 0x002DEF20
		public void ClearFocusedTarget()
		{
			this._focusedTarget = null;
			this.SetCameraState(SpaceCombatCameraController.Movement.FreeLook);
		}

		// Token: 0x0600624A RID: 25162 RVA: 0x002E0D30 File Offset: 0x002DEF30
		public void OnShipDestroyed(CombatShipController ship)
		{
			if (ship == this._focusedTarget)
			{
				this.ClearFocusedTarget();
			}
		}

		// Token: 0x0600624B RID: 25163 RVA: 0x002E0D48 File Offset: 0x002DEF48
		private void CycleShipController(bool up)
		{
			if (!this._spaceCombat.initialized || this._spaceCombat.activeShips == null || this._spaceCombat.activeShips.Count < 1)
			{
				return;
			}
			List<CombatShipController> activeShips = this._spaceCombat.activeShips;
			CombatantController previousTarget = this._previousTarget;
			int num = activeShips.IndexOf((previousTarget != null) ? previousTarget.ref_shipController : null);
			if (num == -1)
			{
				num = 0;
			}
			else if (up)
			{
				num++;
				if (num >= this._spaceCombat.activeShips.Count)
				{
					num = 0;
				}
			}
			else
			{
				num--;
				if (num < 0)
				{
					num = this._spaceCombat.activeShips.Count - 1;
				}
			}
			this.LookAtCombatant(this._spaceCombat.activeShips[num]);
		}

		// Token: 0x0600624C RID: 25164 RVA: 0x002E0E00 File Offset: 0x002DF000
		private void CheckInput(out bool rightDown, out bool middleDown, out bool rightUp, out bool middleUp, out bool right, out bool middle, out float xAxis, out float yAxis, out float zAxis, out float pAxis, out float aAxis, out float rAxis)
		{
			rightDown = Input.GetMouseButtonDown(1);
			middleDown = Input.GetMouseButtonDown(2);
			rightUp = Input.GetMouseButtonUp(1);
			middleUp = Input.GetMouseButtonUp(2);
			right = Input.GetMouseButton(1);
			middle = Input.GetMouseButton(2);
			if (this._dragging & middle)
			{
				xAxis = Input.GetAxis("Mouse X");
				zAxis = Input.GetAxis("Mouse Y");
			}
			else
			{
				xAxis = 0f;
				if (Input.GetKey(TIInputManager.cameraRight))
				{
					xAxis += 1f;
				}
				if (Input.GetKey(TIInputManager.cameraLeft))
				{
					xAxis -= 1f;
				}
				zAxis = 0f;
				if (Input.GetKey(TIInputManager.cameraUp))
				{
					zAxis += 1f;
				}
				if (Input.GetKey(TIInputManager.cameraDown))
				{
					zAxis -= 1f;
				}
			}
			if (this._dragging & right)
			{
				pAxis = Input.GetAxis("Mouse Y") / this.mouseRotateSpeedOffset;
				aAxis = Input.GetAxis("Mouse X") / this.mouseRotateSpeedOffset;
			}
			else
			{
				pAxis = 0f;
				aAxis = 0f;
			}
			if (this._dragging & middle)
			{
				yAxis = Input.GetAxis("Mouse Y");
			}
			else
			{
				yAxis = 0f;
			}
			rAxis = Input.GetAxis("Mouse ScrollWheel");
			if (Input.GetKey(TIInputManager.cameraZoomIn))
			{
				rAxis = TemplateManager.global.combatCamera_keyZoomSpeed;
			}
			if (Input.GetKey(TIInputManager.cameraZoomOut))
			{
				rAxis = -TemplateManager.global.combatCamera_keyZoomSpeed;
			}
			if (UIMagnifier.IsMagnifierActive)
			{
				rAxis = 0f;
			}
			if (this._gridCollider == null && !this._gridColliderFound)
			{
				this._gridCollider = GameControl.spaceCombat.combatGrid.GetComponent<Collider>();
				this._gridColliderFound = true;
			}
			if (!TIStandaloneInputModule.current.IsPointerOverUIGameObject() && !this._dragging && !TIInputManager.IsShiftKeyDown && (rightDown | middleDown))
			{
				Ray ray = this._mainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit raycastHit;
				this._dragging = (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, this._layersMask) && raycastHit.collider == this._gridCollider) || raycastHit.collider == null;
			}
			if (this._dragging && ((rightUp | middleUp) || TIInputManager.IsShiftKeyDown))
			{
				this._dragging = false;
			}
			this._cameraOrientationChanged = rightUp | middleUp;
		}

		// Token: 0x0600624D RID: 25165 RVA: 0x002E105E File Offset: 0x002DF25E
		private void HandleDebug()
		{
			if (Input.GetKeyUp(KeyCode.F12))
			{
				this._spaceCombat.EndCombat(false);
				return;
			}
		}

		// Token: 0x0600624E RID: 25166 RVA: 0x002E107C File Offset: 0x002DF27C
		private void HandleSpecializedInput()
		{
			if (TIInputManager.acceptingInput && !GameControl.handlingException)
			{
				if (!this._spaceCombat.IsInFormationSelectionMode)
				{
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.IncreaseSpeed, TIInputManager.KeyPressMode.Up))
					{
						this._clockController.IncreaseSpeed();
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.DecreaseSpeed, TIInputManager.KeyPressMode.Up))
					{
						this._clockController.DecreaseSpeed();
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.PauseSpeed, TIInputManager.KeyPressMode.Up))
					{
						this._clockController.TogglePause();
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.PauseSpeedNoToggle, TIInputManager.KeyPressMode.Up))
					{
						this._clockController.PauseNoToggle();
					}
					if (Input.GetKeyUp(KeyCode.F1))
					{
						this._clockController.SetSpeed(1);
					}
					if (Input.GetKeyUp(KeyCode.F2))
					{
						this._clockController.SetSpeed(2);
					}
					if (Input.GetKeyUp(KeyCode.F3))
					{
						this._clockController.SetSpeed(3);
					}
					if (Input.GetKeyUp(KeyCode.F4))
					{
						this._clockController.SetSpeed(4);
					}
					if (Input.GetKeyUp(KeyCode.F5))
					{
						this._clockController.SetSpeed(5);
					}
					if (Input.GetKeyUp(KeyCode.F6))
					{
						this._clockController.SetSpeed(6);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.cycleShipsUp, TIInputManager.KeyPressMode.Up))
					{
						this.CycleShipController(true);
					}
					if (TIInputManager.IsHotkeyTriggered(TIInputManager.cycleShipsDown, TIInputManager.KeyPressMode.Up))
					{
						this.CycleShipController(false);
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.toggleGrid, TIInputManager.KeyPressMode.Up))
				{
					this._spaceCombat.combatGrid.ToggleGrid();
				}
				if (Input.GetKeyUp(KeyCode.Escape) && !GameControl.canvasStack.CombatHud.Canvas.enabled)
				{
					this.HandleDebugHideUI();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.toggleCombatUI, TIInputManager.KeyPressMode.Up))
				{
					if (!TIInputManager.IsShiftKeyDown)
					{
						this.HandleDebugHideUI();
					}
					else
					{
						TIInputManager.ToggleCursorVisibility();
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.toggleShipWaypoints, TIInputManager.KeyPressMode.Up) && !this._spaceCombat.IsInFormationSelectionMode)
				{
					this._spaceCombat.ToggleWaypointVisibility();
					foreach (CombatShipController combatShipController in this._spaceCombat.activeShips)
					{
						if (combatShipController.AlwaysShowWaypoints())
						{
							combatShipController.SetWaypointVisualization(true);
						}
						else
						{
							combatShipController.ToggleWaypointVisualization();
						}
					}
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.fleetCommandSelectPrimaryTarget, TIInputManager.KeyPressMode.Up))
				{
					this.TryIssueCombatCommand<SelectTargetCommand, FleetSelectTargetCommand>();
				}
				if (TIInputManager.IsHotkeyTriggered(TIInputManager.fleetCommandLaunchMissileSalvo, TIInputManager.KeyPressMode.Up))
				{
					this.TryIssueCombatCommand<SelectSalvoTargetCommand, FleetSelectSalvoTargetCommand>();
				}
			}
		}

		// Token: 0x0600624F RID: 25167 RVA: 0x002E12D0 File Offset: 0x002DF4D0
		private void HandleDebugHideUI()
		{
			GameControl.canvasStack.CombatHud.Canvas.enabled = !GameControl.canvasStack.CombatHud.Canvas.enabled;
			this._spaceCombat.combatHUD.ToggleDebugHideUI();
			foreach (CombatShipController combatShipController in this._spaceCombat.activeShips)
			{
				if (!GameControl.canvasStack.CombatHud.Canvas.enabled)
				{
					combatShipController.ModelController.selectionAnimObject.SetActive(false);
					combatShipController.ModelController.groupSelectionAnimObject.SetActive(false);
					combatShipController.ModelController.padlockIconObject.SetActive(false);
				}
				else if (!combatShipController.isDestroyed)
				{
					combatShipController.ModelController.padlockIconObject.SetActive(combatShipController._waypointNavigationController.PadlockEnabled);
				}
			}
			if (GameControl.canvasStack.CombatHud.Canvas.enabled)
			{
				CombatShipController selectedFriendlyShip = this._spaceCombat.combatHUD.selectedFriendlyShip;
				if (selectedFriendlyShip != null)
				{
					selectedFriendlyShip.ModelController.StartSelectionAnimation();
				}
				foreach (CombatShipController combatShipController2 in this._spaceCombat.combatHUD.groupSelectedFriendlyShips)
				{
					combatShipController2.ModelController.groupSelectionAnimObject.SetActive(true);
				}
			}
		}

		// Token: 0x06006250 RID: 25168 RVA: 0x002E1454 File Offset: 0x002DF654
		private void TryIssueCombatCommand<TShip, TFleet>() where TShip : IShipCommand where TFleet : IFleetCommand
		{
			SpaceCombatCanvasController combatHUD = GameControl.spaceCombat.combatHUD;
			if (combatHUD == null)
			{
				return;
			}
			List<TISpaceShipState> list = new List<TISpaceShipState>();
			List<CombatShipController> groupSelectedFriendlyShips = combatHUD.groupSelectedFriendlyShips;
			if (groupSelectedFriendlyShips != null && groupSelectedFriendlyShips.Count > 1)
			{
				IShipCommand shipCommand = ShipCommandsManager.shipCommands.FirstOrDefault<IShipCommand>((IShipCommand x) => x is TShip);
				list = (from x in combatHUD.groupSelectedFriendlyShips
					select x.GetCombatantState() as TISpaceShipState into y
					where y != null && !y.ShipDestroyed() && !y.hasDisengaged
					select y).ToList<TISpaceShipState>();
				this.IssueCommandToShips(list, shipCommand);
				return;
			}
			if (combatHUD.selectedFriendlyShipState != null)
			{
				IShipCommand shipCommand2 = ShipCommandsManager.shipCommands.FirstOrDefault<IShipCommand>((IShipCommand x) => x is TShip);
				this.IssueCommandToShip(combatHUD.selectedFriendlyShipState, shipCommand2);
				return;
			}
			IFleetCommand fleetCommand = ShipCommandsManager.fleetCommands.FirstOrDefault<IFleetCommand>((IFleetCommand x) => x is TFleet);
			this.IssueCommandToFleet(fleetCommand);
		}

		// Token: 0x06006251 RID: 25169 RVA: 0x002E1598 File Offset: 0x002DF798
		private void IssueCommandToShip(TISpaceShipState shipToReceiveCommands, IShipCommand fleetSelectPrimaryTargetCommand)
		{
			if (fleetSelectPrimaryTargetCommand.ActorCanPerformCommand(shipToReceiveCommands))
			{
				if (fleetSelectPrimaryTargetCommand.RequiresTarget())
				{
					(fleetSelectPrimaryTargetCommand as IShipCommandWithTarget).InitiateTargeting(shipToReceiveCommands);
				}
				else
				{
					fleetSelectPrimaryTargetCommand.OnCommandExecute(shipToReceiveCommands, null);
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteShipCommand", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06006252 RID: 25170 RVA: 0x002E15E8 File Offset: 0x002DF7E8
		private void IssueCommandToShips(List<TISpaceShipState> shipsToRecieveCommands, IShipCommand fleetSelectPrimaryTargetCommand)
		{
			bool flag = false;
			foreach (TISpaceShipState tispaceShipState in shipsToRecieveCommands)
			{
				if (fleetSelectPrimaryTargetCommand.ActorCanPerformCommand(tispaceShipState))
				{
					if (fleetSelectPrimaryTargetCommand.RequiresTarget())
					{
						(fleetSelectPrimaryTargetCommand as IShipCommandWithTarget).InitiateTargeting(tispaceShipState);
					}
					else
					{
						fleetSelectPrimaryTargetCommand.OnCommandExecute(tispaceShipState, null);
					}
					flag = true;
				}
			}
			if (flag)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteShipCommand", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06006253 RID: 25171 RVA: 0x002E1678 File Offset: 0x002DF878
		private void IssueCommandToFleet(IFleetCommand fleetSelectPrimaryTargetCommand)
		{
			List<TISpaceShipState> list = (from x in GameControl.spaceCombat.activeShips
				select x.GetCombatantState() as TISpaceShipState into y
				where y.ref_faction.isActivePlayer && y != null && !y.ShipDestroyed() && !y.hasDisengaged
				select y).ToList<TISpaceShipState>();
			if (fleetSelectPrimaryTargetCommand.PlayerCanIssueCommand(list))
			{
				if (fleetSelectPrimaryTargetCommand.RequiresTarget())
				{
					(fleetSelectPrimaryTargetCommand as IFleetCommandWithTarget).InitiateTargeting(list);
				}
				else
				{
					fleetSelectPrimaryTargetCommand.OnExecuteFleetCommand(list, null);
				}
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_ExecuteFleetCommand", false, false);
				return;
			}
			AudioManager.PlayOneShot("event:/SFX/UI_SFX/trig_SFX_BadUI", false, false);
		}

		// Token: 0x06006254 RID: 25172 RVA: 0x002E171D File Offset: 0x002DF91D
		private void HandleWaypointPlacement(bool rightUp, bool rightDown, bool altDown)
		{
			if (!this._dragging && altDown)
			{
				this._placingWaypoint = true;
				return;
			}
		}

		// Token: 0x06006255 RID: 25173 RVA: 0x002E1734 File Offset: 0x002DF934
		private void FollowTarget(float pAxis, float aAxis)
		{
			if (this._spaceCombat.combatEnded)
			{
				return;
			}
			double num = Mathd.Clamp(this._polarOffset.inclination - (double)pAxis, 1.0, 179.0);
			double num2 = this._polarOffset.azimuth + (double)aAxis;
			this._polarOffset.inclination = num;
			this._polarOffset.azimuth = num2;
			this._cameraFocalPoint = this._focusedTarget.position;
			this._spaceCombatCamera.transform.position = this.Position + this._followCameraOffset;
			this._spaceCombatCamera.transform.LookAt(this._focusedTarget.position + this._followCameraOffset);
			this._cameraEulerAngles = this._spaceCombatCamera.transform.eulerAngles;
		}

		// Token: 0x06006256 RID: 25174 RVA: 0x002E180C File Offset: 0x002DFA0C
		private void HandleMovement(float xAxis, float yAxis, float zAxis, bool middle, bool right)
		{
			if (this._spaceCombat.combatEnded || !TIInputManager.acceptingInput)
			{
				return;
			}
			if (this._dragging && right)
			{
				this._spaceCombatCamera.transform.position += this._cameraMovementSpeed * 1.5f * zAxis * this._spaceCombatCamera.transform.forward;
				this._spaceCombatCamera.transform.position += this._cameraMovementSpeed * 1.5f * xAxis * this._spaceCombatCamera.transform.right;
			}
			else if (this._dragging && !TIInputManager.IsControlKeyDown && middle)
			{
				Quaternion rotation = this._spaceCombatCamera.transform.rotation;
				this._spaceCombatCamera.transform.Rotate(-rotation.eulerAngles.x, 0f, 0f);
				this._spaceCombatCamera.transform.position += this._cameraMovementSpeed * 1.5f * xAxis * this._spaceCombatCamera.transform.right;
				this._spaceCombatCamera.transform.position += this._cameraMovementSpeed * 1.5f * yAxis * this._spaceCombatCamera.transform.up;
				this._spaceCombatCamera.transform.rotation = rotation;
			}
			else
			{
				Quaternion rotation2 = this._spaceCombatCamera.transform.rotation;
				this._spaceCombatCamera.transform.Rotate(-rotation2.eulerAngles.x, 0f, 0f);
				this._spaceCombatCamera.transform.position += this._cameraMovementSpeed * 1.5f * zAxis * this._spaceCombatCamera.transform.forward;
				this._spaceCombatCamera.transform.position += this._cameraMovementSpeed * 1.5f * xAxis * this._spaceCombatCamera.transform.right;
				this._spaceCombatCamera.transform.rotation = rotation2;
			}
			Ray ray = this._spaceCombatCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(Vector3.zero, Vector3.up, Input.mousePosition);
			float num;
			GameControl.spaceCombat.combatGrid.GetDistanceToPointOfIntersection(ray, out num);
			Vector3 vector = this._spaceCombatCamera.transform.forward * num;
			vector.x = Mathf.Clamp(vector.x, this.minPan, this.maxPan);
			vector.z = Mathf.Clamp(vector.z, this.minPan, this.maxPan);
			this._cameraFocalPoint = vector;
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x002E1B14 File Offset: 0x002DFD14
		private void HandleRotation(float pAxis, float aAxis)
		{
			if ((pAxis != 0f || aAxis != 0f) && this._dragging)
			{
				Vector3 vector = Input.mousePosition - TIInputManager.lastMousePos;
				if (vector.magnitude != 0f)
				{
					float num = this.mouseRotateSpeedOffset / 6f * vector.x;
					float num2 = this.mouseRotateSpeedOffset / 6f * vector.y;
					this._cameraEulerAngles += new Vector3(-num2, num, 0f);
					this._cameraEulerAngles.x = Mathf.Clamp((this._cameraEulerAngles.x + 180f) % 360f - 180f, -89f, 89f);
					this._cameraEulerAngles.y = this._cameraEulerAngles.y % 360f;
					this._spaceCombatCamera.transform.eulerAngles = this._cameraEulerAngles;
				}
			}
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x002E1C08 File Offset: 0x002DFE08
		private void HandleZoom(float rAxis)
		{
			if (!TIInputManager.acceptingInput || TIInputManager.blockCombatZoom)
			{
				return;
			}
			if (!Mathf.Approximately(rAxis, 0f) && this._screenRect.Contains(Input.mousePosition))
			{
				rAxis *= this._scrollSpeedOffset;
				if ((this._zoomTimeRemaining > 0f && rAxis < 0f && this._isTargetPolarOffsetIncreasing) || (rAxis > 0f && !this._isTargetPolarOffsetIncreasing))
				{
					this._targetPolarOffset = Mathd.Clamp(this._polarOffset.radius + (double)rAxis, (double)this.ScaledMinZoom, (double)this.ScaledMaxZoom);
					this._isTargetPolarOffsetIncreasing = !this._isTargetPolarOffsetIncreasing;
				}
				else
				{
					this._targetPolarOffset = Mathd.Clamp(this._targetPolarOffset + (double)rAxis, (double)this.ScaledMinZoom, (double)this.ScaledMaxZoom);
				}
				this._zoomTimeRemaining = 0.15f;
				if (this._cameraMovement == SpaceCombatCameraController.Movement.FreeLook)
				{
					float num = this._freelookZoomMultiplier;
					if (TIInputManager.IsControlKeyDown)
					{
						num *= 0.25f;
					}
					this._targetCameraPos = this._spaceCombatCamera.transform.position + this._spaceCombatCamera.transform.forward * (-rAxis * this.maxCameraMovementSpeed * num);
					this._freelookZoomTimer = 0.1f;
				}
			}
			if (this._zoomTimeRemaining > 0f)
			{
				float num2 = Mathf.Min(Time.unscaledDeltaTime, this._zoomTimeRemaining);
				float num3 = num2 / this._zoomTimeRemaining;
				this._zoomTimeRemaining -= num2;
				if (this._zoomTimeRemaining < 0.001f)
				{
					this._polarOffset.radius = this._targetPolarOffset;
					this._zoomTimeRemaining = 0f;
				}
				else
				{
					double num4 = (this._targetPolarOffset - this._polarOffset.radius) * (double)num3;
					this._polarOffset.radius += num4;
				}
				float num5 = (float)(1.0 - ((double)this.ScaledMaxZoom - this._polarOffset.radius) / (double)(this.ScaledMaxZoom - this.ScaledMinZoom));
				this._scrollSpeedOffset = Mathf.Lerp(this.minScrollSpeedOffset, this.maxScrollSpeedOffset, num5) * this.Scale;
				this._cameraMovementSpeed = Mathf.Lerp(this.minCameraMovementSpeed, this.maxCameraMovementSpeed, num5) * this.Scale;
			}
			if (this._freelookZoomTimer > 0f)
			{
				this._freelookZoomTimer -= Time.deltaTime;
				this._spaceCombatCamera.transform.position = Vector3.Lerp(this._spaceCombatCamera.transform.position, this._targetCameraPos, Time.deltaTime * 10f);
			}
		}

		// Token: 0x040044FA RID: 17658
		private const float ZOOM_TIME = 0.15f;

		// Token: 0x040044FB RID: 17659
		private Camera _spaceCombatCamera;

		// Token: 0x040044FC RID: 17660
		private Vector3 _cameraFocalPoint;

		// Token: 0x040044FD RID: 17661
		private float _scrollSpeedOffset;

		// Token: 0x040044FE RID: 17662
		private float _cameraMovementSpeed;

		// Token: 0x040044FF RID: 17663
		private float _freelookZoomMultiplier = 0.25f;

		// Token: 0x04004500 RID: 17664
		private bool _dragging;

		// Token: 0x04004501 RID: 17665
		private bool _placingWaypoint;

		// Token: 0x04004502 RID: 17666
		private SpaceCombatManager _spaceCombat;

		// Token: 0x04004503 RID: 17667
		private int _activePlayerFleetControllerIdx;

		// Token: 0x04004504 RID: 17668
		private float _zoomTimeRemaining;

		// Token: 0x04004505 RID: 17669
		private float _freelookZoomTimer;

		// Token: 0x04004506 RID: 17670
		private Vector3 _targetCameraPos;

		// Token: 0x04004507 RID: 17671
		private double _targetPolarOffset;

		// Token: 0x04004508 RID: 17672
		private bool _isTargetPolarOffsetIncreasing;

		// Token: 0x04004509 RID: 17673
		private float _scale;

		// Token: 0x0400450A RID: 17674
		private Polar _polarOffset;

		// Token: 0x0400450B RID: 17675
		public bool IsCameraMovementBlocked;

		// Token: 0x0400450C RID: 17676
		private Camera _mainCamera;

		// Token: 0x0400450D RID: 17677
		private CombatantController _focusedTarget;

		// Token: 0x0400450E RID: 17678
		private CombatantController _previousTarget;

		// Token: 0x0400450F RID: 17679
		private SpaceCombatSpeedController _clockController;

		// Token: 0x04004510 RID: 17680
		private Vector3 _followCameraOffset = Vector3.zero;

		// Token: 0x04004511 RID: 17681
		private Vector3 _cameraEulerAngles = Vector3.zero;

		// Token: 0x04004512 RID: 17682
		private Transform _spaceCombatRoot;

		// Token: 0x04004513 RID: 17683
		private Collider _gridCollider;

		// Token: 0x04004514 RID: 17684
		private readonly Rect _screenRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);

		// Token: 0x04004515 RID: 17685
		private SpaceCombatCameraController.Movement _cameraMovement;

		// Token: 0x04004516 RID: 17686
		private int _layersMask;

		// Token: 0x04004517 RID: 17687
		private bool _gridColliderFound;

		// Token: 0x04004518 RID: 17688
		private bool _cameraOrientationChanged;

		// Token: 0x0200138E RID: 5006
		// (Invoke) Token: 0x06009195 RID: 37269
		public delegate void CameraMovement(Vector3 worldOffset, Quaternion worldRotation);

		// Token: 0x0200138F RID: 5007
		private enum Movement
		{
			// Token: 0x040071DC RID: 29148
			FreeLook,
			// Token: 0x040071DD RID: 29149
			Follow
		}
	}
}
