using System;
using System.Collections.Generic;
using PavonisInteractive.TerraInvicta.Audio;
using UnityEngine;

namespace PavonisInteractive.TerraInvicta.SpaceCombat
{
	// Token: 0x020009F7 RID: 2551
	public class WaypointController
	{
		// Token: 0x060060E4 RID: 24804 RVA: 0x002D81B6 File Offset: 0x002D63B6
		private float WaypointRotationSnap()
		{
			return (float)TIPlayerProfileManager.waypointAngleSnap;
		}

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x060060E5 RID: 24805 RVA: 0x002D81C0 File Offset: 0x002D63C0
		// (remove) Token: 0x060060E6 RID: 24806 RVA: 0x002D81F8 File Offset: 0x002D63F8
		public event Action<WaypointController> OnWaypointReadyForInput;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x060060E7 RID: 24807 RVA: 0x002D8230 File Offset: 0x002D6430
		// (remove) Token: 0x060060E8 RID: 24808 RVA: 0x002D8268 File Offset: 0x002D6468
		public event Action<WaypointController> OnWaypointEndingInput;

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x060060E9 RID: 24809 RVA: 0x002D82A0 File Offset: 0x002D64A0
		// (remove) Token: 0x060060EA RID: 24810 RVA: 0x002D82D8 File Offset: 0x002D64D8
		public event Action<AdjustableWaypoint> OnWaypointRemovalRequested;

		// Token: 0x170010AD RID: 4269
		// (get) Token: 0x060060EB RID: 24811 RVA: 0x002D830D File Offset: 0x002D650D
		public int BaseColorIndex
		{
			get
			{
				return this._visual.BaseColorIndex;
			}
		}

		// Token: 0x170010AE RID: 4270
		// (get) Token: 0x060060EC RID: 24812 RVA: 0x002D831A File Offset: 0x002D651A
		// (set) Token: 0x060060ED RID: 24813 RVA: 0x002D8327 File Offset: 0x002D6527
		private bool IsInputLocked
		{
			get
			{
				return this._waypoint.IsInputLocked;
			}
			set
			{
				if (this._waypoint.IsInputLocked == value)
				{
					return;
				}
				this._waypoint.IsInputLocked = value;
				this.RefreshVisualColor();
			}
		}

		// Token: 0x170010AF RID: 4271
		// (get) Token: 0x060060EE RID: 24814 RVA: 0x002D834A File Offset: 0x002D654A
		private bool IsPositionallyLocked
		{
			get
			{
				return this.BaseColorIndex <= 1;
			}
		}

		// Token: 0x170010B0 RID: 4272
		// (get) Token: 0x060060EF RID: 24815 RVA: 0x002D8358 File Offset: 0x002D6558
		// (set) Token: 0x060060F0 RID: 24816 RVA: 0x002D8360 File Offset: 0x002D6560
		public bool IsDvLocked
		{
			get
			{
				return this._isDvLocked;
			}
			set
			{
				if (this._isDvLocked == value)
				{
					return;
				}
				this._isDvLocked = value;
				this.RefreshVisualColor();
			}
		}

		// Token: 0x170010B1 RID: 4273
		// (get) Token: 0x060060F1 RID: 24817 RVA: 0x002D8379 File Offset: 0x002D6579
		// (set) Token: 0x060060F2 RID: 24818 RVA: 0x002D8381 File Offset: 0x002D6581
		public bool IsSystemFailureLocked
		{
			get
			{
				return this._isSystemFailureLocked;
			}
			set
			{
				if (this._isSystemFailureLocked == value)
				{
					return;
				}
				this._isSystemFailureLocked = value;
				this.RefreshVisualColor();
			}
		}

		// Token: 0x170010B2 RID: 4274
		// (set) Token: 0x060060F3 RID: 24819 RVA: 0x002D839A File Offset: 0x002D659A
		public float ColorInterpolationRatio
		{
			set
			{
				this._visual.ColorInterpolationRatio = value;
			}
		}

		// Token: 0x170010B3 RID: 4275
		// (get) Token: 0x060060F4 RID: 24820 RVA: 0x002D83A8 File Offset: 0x002D65A8
		public bool IsHandlingRotationInput
		{
			get
			{
				return this._isHandlingYawInput || this._isHandlingPitchInput || this._isHandlingRollInput;
			}
		}

		// Token: 0x170010B4 RID: 4276
		// (get) Token: 0x060060F5 RID: 24821 RVA: 0x002D83C2 File Offset: 0x002D65C2
		public bool IsHandlingMovementInput
		{
			get
			{
				return this._isHandlingBurnInput || this._isHandlingAltitudeInput || this._isHandlingLateralInput || this._isHandlingDragInput;
			}
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x002D83E4 File Offset: 0x002D65E4
		public WaypointController(AdjustableWaypoint waypoint, int index, WaypointSharedData waypointSharedData, Transform parent, Vector3 initialForward, bool isCoreWaypoint, TISpaceShipState shipState)
		{
			this._waypointSharedData = waypointSharedData;
			this._shipState = shipState;
			this._combatCamera = GameControl.control.mainCamera.GetComponent<SpaceCombatCameraController>();
			this._combatCameraTransform = this._combatCamera.transform;
			this._waypoint = waypoint;
			this.AddListeners();
			this._visual = WaypointVisual.Create(waypoint, this._waypointSharedData.WaypointPrefab, index, parent, initialForward, isCoreWaypoint, this._shipState);
			this.AddVisualListeners();
			this.IsInputLocked = this.IsPositionallyLocked;
			this._cachedState = new WaypointController.WaypointState
			{
				Position = this._waypoint.Position,
				Rotation = this._waypoint.Rotation,
				Normal = this._waypoint.Rotation * Vector3.up
			};
			this.waypointProjectionPlane = new Plane(Vector3.up, Vector3.zero);
		}

		// Token: 0x060060F7 RID: 24823 RVA: 0x002D84E5 File Offset: 0x002D66E5
		private void SetWaypointState(Vector3 worldPosition, Quaternion worldRotation)
		{
			this._cachedState.WorldPosition = worldPosition;
			this._cachedState.WorldRotation = worldRotation;
			this.SetWaypointState();
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x002D8508 File Offset: 0x002D6708
		private void SetWaypointState()
		{
			this._cachedState.Position = this._waypoint.Position;
			this._cachedState.Rotation = this._waypoint.Rotation;
			this._cachedState.Normal = this._waypoint.Rotation * Vector3.up;
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x002D8561 File Offset: 0x002D6761
		private void RefreshVisualColor()
		{
			if (this.IsInputLocked)
			{
				this._visual.ShowLockedColor();
				return;
			}
			if (this.IsSystemFailureLocked)
			{
				this._visual.ShowSystemFailureLockedColor();
				return;
			}
			this._visual.ShowBaseColor();
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x002D8596 File Offset: 0x002D6796
		private void AddListeners()
		{
			this._waypoint.OnPositionRotationChange += this.UpdateVisualPositionRotation;
			this._combatCamera.OnCameraMovementFinished += this.SetWaypointState;
		}

		// Token: 0x060060FB RID: 24827 RVA: 0x002D85C6 File Offset: 0x002D67C6
		private void RemoveListeners()
		{
			this._waypoint.OnPositionRotationChange -= this.UpdateVisualPositionRotation;
			this._combatCamera.OnCameraMovementFinished -= this.SetWaypointState;
		}

		// Token: 0x060060FC RID: 24828 RVA: 0x002D85F6 File Offset: 0x002D67F6
		public void UpdateVisualPositionRotation()
		{
			this._visual.SetPositionRotation(this._waypoint.Position, this._waypoint.Rotation);
			this._shouldUpdateWaypointScale = true;
			this.RefreshVisualColor();
		}

		// Token: 0x060060FD RID: 24829 RVA: 0x002D8626 File Offset: 0x002D6826
		private void AddVisualListeners()
		{
			this._visual.OnWaypointMouseOverBegin += this.HandleOnWaypointMouseOverBegin;
			this._visual.OnWaypointMouseOverEnd += this.HandleOnWaypointMouseOverEnd;
		}

		// Token: 0x060060FE RID: 24830 RVA: 0x002D8656 File Offset: 0x002D6856
		private void RemoveVisualListeners()
		{
			this._visual.OnWaypointMouseOverBegin -= this.HandleOnWaypointMouseOverBegin;
			this._visual.OnWaypointMouseOverEnd -= this.HandleOnWaypointMouseOverEnd;
		}

		// Token: 0x060060FF RID: 24831 RVA: 0x002D8688 File Offset: 0x002D6888
		private void HandleOnWaypointMouseOverBegin()
		{
			if (Input.GetMouseButton(0))
			{
				return;
			}
			if (TIStandaloneInputModule.current.IsPointerOverUIGameObject())
			{
				return;
			}
			if ((this.IsPositionallyLocked && this._visual._isPlacementWaypoint) || this.IsSystemFailureLocked)
			{
				return;
			}
			this._isMouseOverEndEventPending = false;
			Action<WaypointController> onWaypointReadyForInput = this.OnWaypointReadyForInput;
			if (onWaypointReadyForInput == null)
			{
				return;
			}
			onWaypointReadyForInput(this);
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x002D86E4 File Offset: 0x002D68E4
		private void HandleOnWaypointMouseOverEnd()
		{
			if ((this.IsPositionallyLocked && this._visual._isPlacementWaypoint) || this.IsSystemFailureLocked)
			{
				return;
			}
			if (this._isActiveInputHandler)
			{
				this._isMouseOverEndEventPending = true;
				return;
			}
			this._isMouseOverEndEventPending = false;
			Action<WaypointController> onWaypointEndingInput = this.OnWaypointEndingInput;
			if (onWaypointEndingInput == null)
			{
				return;
			}
			onWaypointEndingInput(this);
		}

		// Token: 0x06006101 RID: 24833 RVA: 0x002D8737 File Offset: 0x002D6937
		public void RotateColorIndex()
		{
			this.DecrementColorIndex();
			this.UpdateVisualPositionRotation();
		}

		// Token: 0x06006102 RID: 24834 RVA: 0x002D8745 File Offset: 0x002D6945
		private void DecrementColorIndex()
		{
			this._visual.DecrementColorIndex();
			this.IsInputLocked |= this.IsPositionallyLocked;
		}

		// Token: 0x06006103 RID: 24835 RVA: 0x002D8765 File Offset: 0x002D6965
		public void SetActive(bool isActive)
		{
			this._visual.gameObject.SetActive(isActive);
		}

		// Token: 0x06006104 RID: 24836 RVA: 0x002D8778 File Offset: 0x002D6978
		public void SetRenderer(bool isActive)
		{
			this._visual.SetRendererEnabled(isActive);
		}

		// Token: 0x06006105 RID: 24837 RVA: 0x002D8786 File Offset: 0x002D6986
		public void ToggleRenderer()
		{
			this._visual.ToggleRenderer();
		}

		// Token: 0x06006106 RID: 24838 RVA: 0x002D8794 File Offset: 0x002D6994
		public void UpdateVisuals()
		{
			if (this.IsWaypointScaleUpdateRequired())
			{
				this._shouldUpdateWaypointScale = false;
				this._lastCameraPosition = this._combatCameraTransform.position;
				if (this._visual.isPlayerWaypoint)
				{
					if (this.IsHandlingRotationInput)
					{
						this._visual.ShowYawGizmoHighlight(this._isHandlingYawInput);
						this._visual.ShowPitchGizmoHighlight(this._isHandlingPitchInput);
						this._visual.ShowRollGizmoHighlight(this._isHandlingRollInput);
						this._visual.ShowBurnGizmo(false);
						this._visual.ShowAltitudeGizmo(false);
						this._visual.ShowLateralGizmo(false);
						this._visual.ShowMovementGizmo(false, false);
						return;
					}
					if (!this.IsHandlingRotationInput && this._wasDragStartedThisFrame)
					{
						if (this._isHandlingBurnInput)
						{
							this._visual.ShowBurnHighlight(true);
							return;
						}
						if (this._isHandlingAltitudeInput)
						{
							this._visual.ShowAltitudeGizmoHighlight(true);
							return;
						}
						if (this._isHandlingLateralInput)
						{
							this._visual.ShowLateralGizmoHighlight(true);
							return;
						}
						this._visual.ShowMovementGizmo(true, true);
						return;
					}
					else if (this._wasDragEndedThisFrame)
					{
						this._visual.ShowYawGizmo(false);
						this._visual.ShowYawGizmoHighlight(false);
						this._visual.ShowPitchGizmo(false);
						this._visual.ShowPitchGizmoHighlight(false);
						this._visual.ShowRollGizmo(false);
						this._visual.ShowRollGizmoHighlight(false);
						this._visual.ShowBurnGizmo(false);
						this._visual.ShowBurnHighlight(false);
						this._visual.ShowAltitudeGizmo(false);
						this._visual.ShowAltitudeGizmoHighlight(false);
						this._visual.ShowLateralGizmo(false);
						this._visual.ShowLateralGizmoHighlight(false);
						this._visual.ShowMovementGizmo(false, false);
						this._visual.LockMovementGizmoRotation(false);
						this._wasDragEndedThisFrame = false;
					}
				}
			}
		}

		// Token: 0x06006107 RID: 24839 RVA: 0x002D895C File Offset: 0x002D6B5C
		private void UpdateInputVisuals(bool isYawPressed, bool isPitchPressed, bool isAltitudePressed, bool isLateralPressed, bool isBurnPressed, bool isRollPressed)
		{
			if ((this.IsPositionallyLocked && this._visual._isPlacementWaypoint) || this.IsSystemFailureLocked)
			{
				return;
			}
			bool flag = !isYawPressed && isPitchPressed;
			bool flag2 = !isYawPressed && !flag && isRollPressed;
			bool flag3 = !isYawPressed && !flag && !flag2 && isBurnPressed;
			bool flag4 = !isYawPressed && !flag && !flag2 && !flag3 && isAltitudePressed;
			bool flag5 = !isYawPressed && !flag && !flag2 && !flag3 && !isAltitudePressed && isLateralPressed;
			bool flag6 = !isYawPressed && !flag && !flag2 && !flag3 && !flag4 && !flag5;
			if (isYawPressed)
			{
				this._visual.ShowYawGizmo(true);
			}
			else
			{
				this._visual.ShowYawGizmo(false);
			}
			if (flag)
			{
				this._visual.ShowPitchGizmo(true);
			}
			else
			{
				this._visual.ShowPitchGizmo(false);
			}
			if (flag2)
			{
				this._visual.ShowRollGizmo(true);
			}
			else
			{
				this._visual.ShowRollGizmo(false);
			}
			if (flag3 && !this._isDvLocked)
			{
				this._visual.ShowBurnGizmo(true);
			}
			else
			{
				this._visual.ShowBurnGizmo(false);
			}
			if (flag4 && !this._isDvLocked)
			{
				this._visual.ShowAltitudeGizmo(true);
			}
			else
			{
				this._visual.ShowAltitudeGizmo(false);
			}
			if (flag5 && !this._isDvLocked)
			{
				this._visual.ShowLateralGizmo(true);
			}
			else
			{
				this._visual.ShowLateralGizmo(false);
			}
			if (flag6 && !this._isDvLocked)
			{
				this._visual.ShowMovementGizmo(true, this._isHandlingDragInput);
				return;
			}
			this._visual.ShowMovementGizmo(false, false);
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x002D8AE4 File Offset: 0x002D6CE4
		public void ClearGizmoVisuals()
		{
			this._visual.ShowYawGizmo(false);
			this._visual.ShowPitchGizmo(false);
			this._visual.ShowRollGizmo(false);
			this._visual.ShowBurnGizmo(false);
			this._visual.ShowAltitudeGizmo(false);
			this._visual.ShowLateralGizmo(false);
			this._visual.ShowMovementGizmo(false, false);
		}

		// Token: 0x06006109 RID: 24841 RVA: 0x002D8B46 File Offset: 0x002D6D46
		private bool IsWaypointScaleUpdateRequired()
		{
			return this._shouldUpdateWaypointScale || this._lastCameraPosition != this._combatCameraTransform.position;
		}

		// Token: 0x0600610A RID: 24842 RVA: 0x002D8B68 File Offset: 0x002D6D68
		public void ProcessInput()
		{
			if (this._lastUpdateFrame != TIFrameCounter.FrameCount)
			{
				if (!this.IsInputLocked)
				{
					this.DetectInput();
					this.ResolveInput();
					this.ResolveInput();
				}
				this._lastUpdateFrame = TIFrameCounter.FrameCount;
			}
		}

		// Token: 0x0600610B RID: 24843 RVA: 0x002D8B9C File Offset: 0x002D6D9C
		private void DetectInput()
		{
			if (this._waypoint.PadlockEnabled)
			{
				return;
			}
			if (this._waypoint.AllStopEnabled)
			{
				return;
			}
			if (this._waypoint.MatchVelocityEnabled)
			{
				return;
			}
			if (this._waypoint.DefensiveManueversEnabled)
			{
				return;
			}
			if (!this._visual.isPlayerWaypoint)
			{
				return;
			}
			bool mouseButton = Input.GetMouseButton(0);
			bool mouseButtonDown = Input.GetMouseButtonDown(0);
			bool mouseButtonUp = Input.GetMouseButtonUp(0);
			bool mouseButtonUp2 = Input.GetMouseButtonUp(1);
			bool flag = TIInputManager.IsHotkeyTriggered(TIInputManager.altitudeControl, TIInputManager.KeyPressMode.Continous);
			bool flag2 = TIInputManager.IsHotkeyTriggered(TIInputManager.lateralControl, TIInputManager.KeyPressMode.Continous);
			bool flag3 = TIInputManager.IsHotkeyTriggered(TIInputManager.burnControl, TIInputManager.KeyPressMode.Continous);
			bool flag4 = TIInputManager.IsHotkeyTriggered(TIInputManager.yawControl, TIInputManager.KeyPressMode.Continous);
			bool flag5 = TIInputManager.IsHotkeyTriggered(TIInputManager.pitchControl, TIInputManager.KeyPressMode.Continous);
			bool flag6 = TIInputManager.IsHotkeyTriggered(TIInputManager.rollControl, TIInputManager.KeyPressMode.Continous);
			GameControl.spaceCombat.SetWaypointDragging(mouseButton);
			if (!this._isDvLocked)
			{
				this.EvaluateForDragInput(mouseButton, mouseButtonDown, mouseButtonUp, flag, flag2, flag3);
				this.EvaluateForRotationInput(mouseButton, flag4, flag5, flag6);
			}
			this.EvaluateForResetInput(mouseButtonUp2);
			bool flag7 = false;
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				using (List<CombatShipController>.Enumerator enumerator = GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.ShipState == this._shipState)
						{
							flag7 = true;
							break;
						}
					}
				}
			}
			if (!flag7 && mouseButtonDown)
			{
				GameControl.spaceCombat.combatHUD.ClearGroupSelect();
			}
			this.UpdateInputVisuals(flag4, flag5, flag, flag2, flag3, flag6);
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0 && flag7)
			{
				foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
				{
					if (combatShipController.ShipState != this._shipState)
					{
						WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
						if (waypointControllerByColor != null)
						{
							waypointControllerByColor.UpdateInputVisuals(flag4, flag5, flag, flag2, flag3, flag6);
						}
					}
				}
			}
		}

		// Token: 0x0600610C RID: 24844 RVA: 0x002D8DD4 File Offset: 0x002D6FD4
		private void EvaluateForDragInput(bool isMouseLeftPressed, bool isMouseLeftPressedThisFrame, bool isMouseLeftReleased, bool isAltitudePressed, bool isLateralPressed, bool isBurnPressed)
		{
			bool flag = isAltitudePressed && isMouseLeftPressed;
			bool flag2 = isLateralPressed && isMouseLeftPressed;
			bool flag3 = isBurnPressed && isMouseLeftPressed;
			if (isMouseLeftPressed && !this._isHandlingDragInput && !this.IsHandlingRotationInput)
			{
				this._isHandlingDragInput = true;
				this._shouldUpdateWaypointScale = true;
				this.SetWaypointState();
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
					{
						if (combatShipController.ShipState != this._shipState)
						{
							WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
							if (waypointControllerByColor != null)
							{
								waypointControllerByColor.SetWaypointState();
							}
						}
					}
				}
			}
			if (this._isHandlingDragInput)
			{
				if (flag && !this._isHandlingAltitudeInput)
				{
					this._isHandlingAltitudeInput = true;
				}
				else if (!flag && this._isHandlingAltitudeInput)
				{
					this._isHandlingAltitudeInput = false;
				}
				if (flag2 && !this._isHandlingLateralInput)
				{
					this._isHandlingLateralInput = true;
				}
				else if (!flag2 && this._isHandlingLateralInput)
				{
					this._isHandlingLateralInput = false;
				}
				if (flag3 && !this._isHandlingBurnInput)
				{
					this._isHandlingBurnInput = true;
				}
				else if (!flag3 && this._isHandlingBurnInput)
				{
					this._isHandlingBurnInput = false;
				}
			}
			if (!isMouseLeftPressed)
			{
				this._isTerminatingInput = true;
			}
			if (isMouseLeftPressedThisFrame || isMouseLeftReleased)
			{
				AudioManager.PlayOneShot("event:/SFX/UI_SFX/NEW_UI_SFX/trig_SFX_WaypointMovement&Rotation", false, false);
			}
			if (isMouseLeftPressedThisFrame)
			{
				this.mouseStartDragPixelCoord = Input.mousePosition;
				this._wasDragStartedThisFrame = true;
			}
			else
			{
				this._wasDragStartedThisFrame = false;
			}
			if (isMouseLeftReleased)
			{
				this._wasDragEndedThisFrame = true;
				return;
			}
			this._wasDragEndedThisFrame = false;
		}

		// Token: 0x0600610D RID: 24845 RVA: 0x002D8F78 File Offset: 0x002D7178
		private void EvaluateForRotationInput(bool isMouseLeftPressed, bool isYawPressed, bool isPitchPressed, bool isRollPressed)
		{
			bool flag = isYawPressed && isMouseLeftPressed;
			bool flag2 = isPitchPressed && isMouseLeftPressed;
			bool flag3 = isRollPressed && isMouseLeftPressed;
			if (!flag && !flag2 && !flag3 && this.IsHandlingRotationInput)
			{
				this._isTerminatingInput = true;
			}
			if (flag && !this._isHandlingYawInput)
			{
				this._isHandlingYawInput = true;
			}
			else if (!flag && this._isHandlingYawInput)
			{
				this._isHandlingYawInput = false;
			}
			if (flag2 && !this._isHandlingPitchInput)
			{
				this._isHandlingPitchInput = true;
			}
			else if (!flag2 && this._isHandlingPitchInput)
			{
				this._isHandlingPitchInput = false;
			}
			if (flag3 && !this._isHandlingRollInput)
			{
				this._isHandlingRollInput = true;
				return;
			}
			if (!flag3 && this._isHandlingRollInput)
			{
				this._isHandlingRollInput = false;
			}
		}

		// Token: 0x0600610E RID: 24846 RVA: 0x002D901B File Offset: 0x002D721B
		private void EvaluateForResetInput(bool isMouseRightPressed)
		{
			if (isMouseRightPressed && !this.IsHandlingMovementInput && !this.IsHandlingRotationInput)
			{
				this._isHandlingResetInput = true;
			}
		}

		// Token: 0x0600610F RID: 24847 RVA: 0x002D9037 File Offset: 0x002D7237
		private void ResolveInput()
		{
			if (this._isHandlingResetInput)
			{
				this.HandleResetInput();
				return;
			}
			if (this._isTerminatingInput)
			{
				this.HandleTerminateInput(false);
				return;
			}
			if (this.IsHandlingRotationInput)
			{
				this.HandleRotationInput();
				return;
			}
			if (this.IsHandlingMovementInput)
			{
				this.HandleMovementInput();
			}
		}

		// Token: 0x06006110 RID: 24848 RVA: 0x002D9078 File Offset: 0x002D7278
		private void HandleResetInput()
		{
			this._waypoint.ResetCurrentWaypointSequence();
			this.HandleTerminateInput(false);
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
				{
					if (combatShipController.ShipState != this._shipState)
					{
						WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
						if (waypointControllerByColor != null)
						{
							waypointControllerByColor._waypoint.ResetCurrentWaypointSequence();
						}
					}
				}
			}
		}

		// Token: 0x06006111 RID: 24849 RVA: 0x002D912C File Offset: 0x002D732C
		private void HandleTerminateInput(bool forceEnd = false)
		{
			this._isHandlingDragInput = false;
			this._isHandlingYawInput = false;
			this._isHandlingPitchInput = false;
			this._isHandlingRollInput = false;
			this._isTerminatingInput = false;
			this._isHandlingResetInput = false;
			this._isHandlingAltitudeInput = false;
			this._isHandlingLateralInput = false;
			this.SetInputHandling(false);
			if (forceEnd || this._isMouseOverEndEventPending)
			{
				this.HandleOnWaypointMouseOverEnd();
			}
		}

		// Token: 0x06006112 RID: 24850 RVA: 0x002D9189 File Offset: 0x002D7389
		private void HandleRotationInput()
		{
			if (this._isHandlingYawInput)
			{
				this.HandleYawInput();
				return;
			}
			if (this._isHandlingPitchInput)
			{
				this.HandlePitchInput();
				return;
			}
			if (this._isHandlingRollInput)
			{
				this.HandleRollInput();
			}
		}

		// Token: 0x06006113 RID: 24851 RVA: 0x002D91B8 File Offset: 0x002D73B8
		private void HandleMovementInput()
		{
			if (this._isHandlingBurnInput)
			{
				this.HandleBurnDragInput();
				return;
			}
			if (this._isHandlingAltitudeInput)
			{
				this.HandleMovementDragInput(Vector3.right, false);
				return;
			}
			if (this._isHandlingLateralInput)
			{
				this.HandleMovementDragInput(Vector3.forward, false);
				return;
			}
			if (this._isHandlingDragInput)
			{
				this.HandleMovementDragInput(Vector3.up, false);
			}
		}

		// Token: 0x06006114 RID: 24852 RVA: 0x002D9214 File Offset: 0x002D7414
		private void HandleYawInput()
		{
			this.SetInputHandling(true);
			if (Input.GetAxis("Mouse X") != 0f)
			{
				Vector3 vector = ((this._cachedState.Rotation.eulerAngles.x < 90f || this._cachedState.Rotation.eulerAngles.x > 270f) ? (this._cachedState.Rotation * Vector3.down) : (this._cachedState.Rotation * Vector3.up));
				Vector3 vector2 = this._cachedState.Position + this._cachedState.WorldPosition;
				Vector3 vector3 = GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector2, vector, Input.mousePosition);
				Vector3 vector4 = vector3 - vector2;
				float num = Vector3.SignedAngle(Vector3.ProjectOnPlane(this._cachedState.Rotation * Vector3.forward, vector), vector4, vector);
				Quaternion quaternion = ((num != 0f) ? Quaternion.AngleAxis(num, vector) : Quaternion.Euler(vector4)) * this._cachedState.Rotation;
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					if (TIInputManager.IsShiftKeyDown)
					{
						this._waypoint.AdjustRotation(quaternion, null);
						this._waypoint.CacheWaypointOrientationRecursively();
						Debug.DrawRay(this._cachedState.Position, this._waypoint.Rotation * Vector3.forward * 100f);
					}
					else
					{
						this._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
						this._waypoint.CacheWaypointOrientationRecursively();
						Debug.DrawRay(this._cachedState.Position, this._waypoint.Rotation * Vector3.forward * 100f);
					}
				}
				else
				{
					this._waypoint.AdjustRotation(quaternion, null);
					this._waypoint.CacheWaypointOrientationRecursively();
					Debug.DrawRay(this._cachedState.Position, this._waypoint.Rotation * Vector3.forward * 100f);
				}
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
					{
						if (combatShipController.ShipState != this._shipState)
						{
							WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
							if (waypointControllerByColor != null && !waypointControllerByColor.IsInputLocked && !waypointControllerByColor.IsSystemFailureLocked)
							{
								if (TIInputManager.IsControlKeyDown)
								{
									vector4 = vector3 - waypointControllerByColor._cachedState.Position + waypointControllerByColor._cachedState.WorldPosition;
									num = Vector3.SignedAngle(Vector3.ProjectOnPlane(waypointControllerByColor._cachedState.Rotation * Vector3.forward, vector), vector4, vector);
									quaternion = ((num != 0f) ? Quaternion.AngleAxis(num, vector) : Quaternion.Euler(vector4)) * waypointControllerByColor._cachedState.Rotation;
									if (TIInputManager.IsShiftKeyDown)
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, null);
									}
									else
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
									}
									waypointControllerByColor._waypoint.CacheWaypointOrientation();
									Debug.DrawRay(this._cachedState.Position, waypointControllerByColor._waypoint.Rotation * Vector3.forward * 100f);
								}
								else
								{
									if (TIInputManager.IsShiftKeyDown)
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, null);
									}
									else
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
									}
									waypointControllerByColor._waypoint.CacheWaypointOrientation();
									Debug.DrawRay(this._cachedState.Position, waypointControllerByColor._waypoint.Rotation * Vector3.forward * 100f);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006115 RID: 24853 RVA: 0x002D9664 File Offset: 0x002D7864
		private void HandlePitchInput()
		{
			this.SetInputHandling(true);
			if (Input.GetAxis("Mouse X") != 0f)
			{
				Vector3 vector = ((this._cachedState.Rotation.eulerAngles.y < 90f || this._cachedState.Rotation.eulerAngles.y > 270f) ? (this._cachedState.Rotation * Vector3.left) : (this._cachedState.Rotation * Vector3.right));
				Vector3 vector2 = this._cachedState.Position + this._cachedState.WorldPosition;
				Vector3 vector3 = GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector2, vector, Input.mousePosition);
				Vector3 vector4 = vector3 - vector2;
				float num = Vector3.SignedAngle(Vector3.ProjectOnPlane(this._cachedState.Rotation * Vector3.forward, vector), vector4, vector);
				Quaternion quaternion = ((num != 0f) ? Quaternion.AngleAxis(num, vector) : Quaternion.Euler(vector4)) * this._cachedState.Rotation;
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					if (TIInputManager.IsShiftKeyDown)
					{
						this._waypoint.AdjustRotation(quaternion, null);
						this._waypoint.CacheWaypointOrientationRecursively();
						Debug.DrawRay(this._cachedState.Position, this._waypoint.Rotation * Vector3.forward * 100f);
					}
					else
					{
						this._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
						this._waypoint.CacheWaypointOrientationRecursively();
						Debug.DrawRay(this._cachedState.Position, this._waypoint.Rotation * Vector3.forward * 100f);
					}
				}
				else
				{
					this._waypoint.AdjustRotation(quaternion, null);
					this._waypoint.CacheWaypointOrientationRecursively();
					Debug.DrawRay(this._cachedState.Position, this._waypoint.Rotation * Vector3.forward * 100f);
				}
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
					{
						if (combatShipController.ShipState != this._shipState)
						{
							WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
							if (waypointControllerByColor != null && !waypointControllerByColor.IsInputLocked && !waypointControllerByColor.IsSystemFailureLocked)
							{
								if (TIInputManager.IsControlKeyDown)
								{
									vector4 = vector3 - waypointControllerByColor._cachedState.Position + waypointControllerByColor._cachedState.WorldPosition;
									num = Vector3.SignedAngle(Vector3.ProjectOnPlane(waypointControllerByColor._cachedState.Rotation * Vector3.forward, vector), vector4, vector);
									quaternion = ((num != 0f) ? Quaternion.AngleAxis(num, vector) : Quaternion.Euler(vector4)) * waypointControllerByColor._cachedState.Rotation;
									if (TIInputManager.IsShiftKeyDown)
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, null);
									}
									else
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
									}
									waypointControllerByColor._waypoint.CacheWaypointOrientation();
									Debug.DrawRay(this._cachedState.Position, waypointControllerByColor._waypoint.Rotation * Vector3.forward * 100f);
								}
								else
								{
									if (TIInputManager.IsShiftKeyDown)
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, null);
									}
									else
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
									}
									waypointControllerByColor._waypoint.CacheWaypointOrientation();
									Debug.DrawRay(this._cachedState.Position, waypointControllerByColor._waypoint.Rotation * Vector3.forward * 100f);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006116 RID: 24854 RVA: 0x002D9AB4 File Offset: 0x002D7CB4
		private void HandleRollInput()
		{
			this.SetInputHandling(true);
			if (Input.GetAxis("Mouse X") != 0f)
			{
				Vector3 vector = this._cachedState.Rotation * ((this._cachedState.Rotation.eulerAngles.z < 90f || this._cachedState.Rotation.eulerAngles.z > 270f) ? Vector3.back : Vector3.forward);
				Vector3 vector2 = this._cachedState.Position + this._cachedState.WorldPosition;
				Vector3 vector3 = GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector2, vector, Input.mousePosition);
				Vector3 vector4 = vector3 - vector2;
				float num = Vector3.SignedAngle(Vector3.ProjectOnPlane(this._cachedState.Rotation * Vector3.right, vector), vector4, vector);
				Quaternion quaternion = ((num != 0f) ? Quaternion.AngleAxis(num, vector) : Quaternion.Euler(vector4)) * this._cachedState.Rotation;
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					if (TIInputManager.IsShiftKeyDown)
					{
						this._waypoint.AdjustRotation(quaternion, null);
					}
					else
					{
						this._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
					}
				}
				else
				{
					this._waypoint.AdjustRotation(quaternion, null);
				}
				this._waypoint.CacheWaypointOrientationRecursively();
				if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
				{
					foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
					{
						if (combatShipController.ShipState != this._shipState)
						{
							WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
							if (waypointControllerByColor != null && !waypointControllerByColor.IsInputLocked && !waypointControllerByColor.IsSystemFailureLocked)
							{
								if (TIInputManager.IsControlKeyDown)
								{
									vector4 = vector3 - waypointControllerByColor._cachedState.Position + waypointControllerByColor._cachedState.WorldPosition;
									num = Vector3.SignedAngle(Vector3.ProjectOnPlane(waypointControllerByColor._cachedState.Rotation * Vector3.right, vector), vector4, vector);
									quaternion = ((num != 0f) ? Quaternion.AngleAxis(num, vector) : Quaternion.Euler(vector4)) * waypointControllerByColor._cachedState.Rotation;
									if (TIInputManager.IsShiftKeyDown)
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, null);
									}
									else
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
									}
									waypointControllerByColor._waypoint.CacheWaypointOrientation();
								}
								else
								{
									if (TIInputManager.IsShiftKeyDown)
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, null);
									}
									else
									{
										waypointControllerByColor._waypoint.AdjustRotation(quaternion, GameControl.spaceCombat.combatHUD.GroupConstraints);
									}
									waypointControllerByColor._waypoint.CacheWaypointOrientation();
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006117 RID: 24855 RVA: 0x002D9DEC File Offset: 0x002D7FEC
		private void HandleMovementDragInput(Vector3 dragPlaneNormal, bool isRelativeMovment = false)
		{
			this.SetInputHandling(true);
			Vector3 vector = this._cachedState.Position + this._cachedState.WorldPosition;
			Vector3 vector2 = GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector, dragPlaneNormal, this.mouseStartDragPixelCoord);
			Vector3 vector3 = GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector, dragPlaneNormal, Input.mousePosition);
			Vector3 vector4 = vector3 - vector2;
			Vector3 vector5 = this._cachedState.Position + vector4 * this._dragInputScaler;
			if (TIInputManager.IsControlKeyDown)
			{
				vector5 = vector3;
			}
			else if (TIInputManager.IsAltKeyDown)
			{
				vector5 = this._cachedState.Position - vector4;
			}
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				if (TIInputManager.IsShiftKeyDown)
				{
					this._waypoint.ProposePlacement(vector5, null, false, -1f);
				}
				else
				{
					this._waypoint.ProposePlacement(vector5, GameControl.spaceCombat.combatHUD.GroupConstraints, false, -1f);
				}
			}
			else
			{
				this._waypoint.ProposePlacement(vector5, null, false, -1f);
			}
			this._waypoint.CacheWaypointOrientationRecursively();
			Utilities.DebugDrawPlane(this._cachedState.Position, dragPlaneNormal, Color.green, 1f);
			Utilities.DebugDrawPoint(vector2, 0.5f, Color.red, 0f);
			Utilities.DebugDrawPoint(vector5, 0.5f, Color.green, 0f);
			Debug.DrawLine(vector2, vector5, Color.white);
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
				{
					if (combatShipController.ShipState != this._shipState)
					{
						WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
						if (waypointControllerByColor != null && !waypointControllerByColor.IsInputLocked && !waypointControllerByColor.IsSystemFailureLocked && !waypointControllerByColor.IsDvLocked)
						{
							if (TIInputManager.IsControlKeyDown)
							{
								this.waypointProjectionPlane.SetNormalAndPosition(dragPlaneNormal, waypointControllerByColor._cachedState.Position);
								vector5 = this.waypointProjectionPlane.ClosestPointOnPlane(vector3);
								if (TIInputManager.IsShiftKeyDown)
								{
									waypointControllerByColor._waypoint.ProposePlacement(vector5, null, false, -1f);
								}
								else
								{
									waypointControllerByColor._waypoint.ProposePlacement(vector5, GameControl.spaceCombat.combatHUD.GroupConstraints, false, -1f);
								}
								waypointControllerByColor._waypoint.CacheWaypointOrientationRecursively();
								Utilities.DebugDrawPlane(waypointControllerByColor._cachedState.Position, dragPlaneNormal, Color.green, 1f);
								Utilities.DebugDrawPoint(vector5, 0.5f, Color.green, 0f);
								Debug.DrawLine(waypointControllerByColor._cachedState.Position, vector5, Color.white);
							}
							else if (TIInputManager.IsAltKeyDown)
							{
								this.waypointProjectionPlane.SetNormalAndPosition(dragPlaneNormal, waypointControllerByColor._cachedState.Position);
								Vector3 vector6 = this.waypointProjectionPlane.ClosestPointOnPlane(vector3);
								vector5 = waypointControllerByColor._cachedState.Position + (waypointControllerByColor._cachedState.Position - vector6);
								if (TIInputManager.IsShiftKeyDown)
								{
									waypointControllerByColor._waypoint.ProposePlacement(vector5, null, false, -1f);
								}
								else
								{
									waypointControllerByColor._waypoint.ProposePlacement(vector5, GameControl.spaceCombat.combatHUD.GroupConstraints, false, -1f);
								}
								waypointControllerByColor._waypoint.CacheWaypointOrientationRecursively();
								Utilities.DebugDrawPlane(waypointControllerByColor._cachedState.Position, dragPlaneNormal, Color.green, 1f);
								Utilities.DebugDrawPoint(vector5, 0.5f, Color.green, 0f);
								Debug.DrawLine(waypointControllerByColor._cachedState.Position, vector5, Color.white);
							}
							else
							{
								vector5 = waypointControllerByColor._cachedState.Position + vector4 * this._dragInputScaler;
								if (TIInputManager.IsShiftKeyDown)
								{
									waypointControllerByColor._waypoint.ProposePlacement(vector5, null, false, -1f);
								}
								else
								{
									waypointControllerByColor._waypoint.ProposePlacement(vector5, GameControl.spaceCombat.combatHUD.GroupConstraints, false, -1f);
								}
								waypointControllerByColor._waypoint.CacheWaypointOrientationRecursively();
								Utilities.DebugDrawPlane(waypointControllerByColor._cachedState.Position, dragPlaneNormal, Color.green, 1f);
								Utilities.DebugDrawPoint(waypointControllerByColor._cachedState.Position + vector4 * this._dragInputScaler, 0.5f, Color.green, 0f);
								Debug.DrawLine(waypointControllerByColor._cachedState.Position, waypointControllerByColor._cachedState.Position + vector4 * this._dragInputScaler, Color.white);
							}
						}
					}
				}
			}
		}

		// Token: 0x06006118 RID: 24856 RVA: 0x002DA2E0 File Offset: 0x002D84E0
		private void HandleBurnDragInput()
		{
			this.SetInputHandling(true);
			Vector3 vector = this._cachedState.Rotation * Vector3.up;
			Vector3 vector2 = this._cachedState.Rotation * Vector3.forward;
			Vector3 vector3 = this._cachedState.Position + this._cachedState.WorldPosition;
			Vector3 vector4 = GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector3, vector, this.mouseStartDragPixelCoord);
			float num = Vector3.Dot(GameControl.spaceCombat.combatGrid.CursorPositionRelativeToPlaneAt(vector3, vector, Input.mousePosition) - vector4, vector2);
			Vector3 vector5 = vector2 * num;
			Vector3 vector6 = this._cachedState.Position + vector5 * this._dragInputScaler;
			this._visual.LockMovementGizmoRotation(true);
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				if (TIInputManager.IsShiftKeyDown)
				{
					this._waypoint.ProposePlacement(vector6, null, true, -1f);
				}
				else
				{
					this._waypoint.ProposePlacement(vector6, GameControl.spaceCombat.combatHUD.GroupConstraints, true, -1f);
				}
			}
			else
			{
				this._waypoint.ProposePlacement(vector6, null, true, -1f);
			}
			this._waypoint.CacheWaypointOrientationRecursively();
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
				{
					if (combatShipController.ShipState != this._shipState)
					{
						WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
						if (waypointControllerByColor != null && !waypointControllerByColor.IsInputLocked && !waypointControllerByColor.IsSystemFailureLocked && !waypointControllerByColor.IsDvLocked)
						{
							vector2 = waypointControllerByColor._cachedState.Rotation * Vector3.forward;
							vector5 = vector2 * num;
							vector6 = waypointControllerByColor._cachedState.Position + vector5 * this._dragInputScaler;
							if (TIInputManager.IsShiftKeyDown)
							{
								waypointControllerByColor._waypoint.ProposePlacement(vector6, null, true, -1f);
							}
							else
							{
								waypointControllerByColor._waypoint.ProposePlacement(vector6, GameControl.spaceCombat.combatHUD.GroupConstraints, true, -1f);
							}
							waypointControllerByColor._waypoint.CacheWaypointOrientationRecursively();
						}
					}
				}
			}
		}

		// Token: 0x06006119 RID: 24857 RVA: 0x002DA584 File Offset: 0x002D8784
		public void BeginHandleInput()
		{
			this.SetWaypointState();
			this.ToggleHighlight();
		}

		// Token: 0x0600611A RID: 24858 RVA: 0x002DA594 File Offset: 0x002D8794
		public void EndHandleInput()
		{
			this.ToggleHighlight();
			this.ClearGizmoVisuals();
			if (GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips.Count > 0)
			{
				foreach (CombatShipController combatShipController in GameControl.spaceCombat.combatHUD.groupSelectedFriendlyShips)
				{
					if (combatShipController.ShipState != this._shipState)
					{
						WaypointController waypointControllerByColor = combatShipController._waypointNavigationController.GetWaypointControllerByColor(this.BaseColorIndex);
						if (waypointControllerByColor != null)
						{
							waypointControllerByColor.ClearGizmoVisuals();
						}
					}
				}
			}
			GameControl.spaceCombat.SetWaypointDragging(false);
			this.HandleTerminateInput(false);
		}

		// Token: 0x0600611B RID: 24859 RVA: 0x002DA650 File Offset: 0x002D8850
		private void ToggleHighlight()
		{
			if (!this.IsInputLocked)
			{
				this._visual.ToggleHighlight();
			}
		}

		// Token: 0x0600611C RID: 24860 RVA: 0x002DA665 File Offset: 0x002D8865
		public void SetInputHandling(bool isActiveInputHandler)
		{
			if (this._isActiveInputHandler == isActiveInputHandler)
			{
				return;
			}
			this._isActiveInputHandler = isActiveInputHandler;
			this._visual.SetInputHandling(this._isActiveInputHandler);
			this._shouldUpdateWaypointScale = true;
		}

		// Token: 0x0600611D RID: 24861 RVA: 0x002DA690 File Offset: 0x002D8890
		public void Destroy()
		{
			this.OnWaypointReadyForInput = null;
			this.OnWaypointEndingInput = null;
			this.OnWaypointRemovalRequested = null;
			this.RemoveListeners();
			this.RemoveVisualListeners();
			global::UnityEngine.Object.Destroy(this._visual.gameObject);
			this._visual = null;
		}

		// Token: 0x0600611E RID: 24862 RVA: 0x002DA6CA File Offset: 0x002D88CA
		public void ToggleHeightLine(bool shouldRenderHeightLine)
		{
			this._visual.ToggleHeightLine(shouldRenderHeightLine);
		}

		// Token: 0x0600611F RID: 24863 RVA: 0x002DA6D8 File Offset: 0x002D88D8
		public void ToggleWaypointDVCost(bool shouldShowDVCost)
		{
			this._visual.ToggleWaypointDVCost(shouldShowDVCost);
		}

		// Token: 0x170010B5 RID: 4277
		// (get) Token: 0x06006120 RID: 24864 RVA: 0x002DA6E6 File Offset: 0x002D88E6
		public bool showingDVText
		{
			get
			{
				return this._visual.waypointUI.showingDVText;
			}
		}

		// Token: 0x0400443E RID: 17470
		private readonly WaypointSharedData _waypointSharedData;

		// Token: 0x0400443F RID: 17471
		private AdjustableWaypoint _waypoint;

		// Token: 0x04004440 RID: 17472
		private TISpaceShipState _shipState;

		// Token: 0x04004441 RID: 17473
		public WaypointVisual _visual;

		// Token: 0x04004442 RID: 17474
		private SpaceCombatCameraController _combatCamera;

		// Token: 0x04004443 RID: 17475
		private Transform _combatCameraTransform;

		// Token: 0x04004444 RID: 17476
		private Vector3 mouseStartDragPixelCoord;

		// Token: 0x04004445 RID: 17477
		private Vector3 _lastCameraPosition;

		// Token: 0x04004446 RID: 17478
		private bool _shouldUpdateWaypointScale;

		// Token: 0x04004447 RID: 17479
		private WaypointController.WaypointState _cachedState;

		// Token: 0x04004448 RID: 17480
		private bool _isHandlingAltitudeInput;

		// Token: 0x04004449 RID: 17481
		private bool _isHandlingLateralInput;

		// Token: 0x0400444A RID: 17482
		private bool _isHandlingDragInput;

		// Token: 0x0400444B RID: 17483
		private bool _isHandlingYawInput;

		// Token: 0x0400444C RID: 17484
		private bool _isHandlingPitchInput;

		// Token: 0x0400444D RID: 17485
		private bool _isHandlingRollInput;

		// Token: 0x0400444E RID: 17486
		private bool _isTerminatingInput;

		// Token: 0x0400444F RID: 17487
		private bool _isHandlingResetInput;

		// Token: 0x04004450 RID: 17488
		private bool _isHandlingBurnInput;

		// Token: 0x04004451 RID: 17489
		private bool _wasDragStartedThisFrame;

		// Token: 0x04004452 RID: 17490
		private bool _wasDragEndedThisFrame;

		// Token: 0x04004453 RID: 17491
		private bool _isActiveInputHandler;

		// Token: 0x04004454 RID: 17492
		private bool _isMouseOverEndEventPending;

		// Token: 0x04004455 RID: 17493
		private int _lastUpdateFrame = -1;

		// Token: 0x04004456 RID: 17494
		private bool _isSystemFailureLocked;

		// Token: 0x04004457 RID: 17495
		private bool _isDvLocked;

		// Token: 0x04004458 RID: 17496
		private readonly float _dragInputScaler = 0.38f;

		// Token: 0x04004459 RID: 17497
		private Plane waypointProjectionPlane;

		// Token: 0x02001387 RID: 4999
		private struct WaypointState
		{
			// Token: 0x040071BD RID: 29117
			public Vector3 Position;

			// Token: 0x040071BE RID: 29118
			public Quaternion Rotation;

			// Token: 0x040071BF RID: 29119
			public Vector3 Normal;

			// Token: 0x040071C0 RID: 29120
			public Vector3 WorldPosition;

			// Token: 0x040071C1 RID: 29121
			public Quaternion WorldRotation;
		}
	}
}
